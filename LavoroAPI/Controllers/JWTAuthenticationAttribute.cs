using Jose;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace LavoroAPI.Controllers
{
    public class JWTAuthenticationAttribute : Attribute { 
     
        public bool AllowMultiple => false;
        public string Realm { get; set; }
        private readonly byte[] _secretKey;

        public JWTAuthenticationAttribute()
        {
            _secretKey = System.Text.Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("CX_JWT_SECRET"));
        }

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() =>
            {
                // 1. Look for credentials in the request.
                HttpRequestMessage request = context.Request;
                AuthenticationHeaderValue authorization = request.Headers.Authorization;

                // 2. If there are no credentials, do nothing.
                if (authorization == null)
                {
                    return;
                }

                // 3. If there are credentials but the filter does not recognize the 
                //    authentication scheme, do nothing.
                if (authorization.Scheme != "Bearer")
                {
                    return;
                }

                // 4. If there are credentials that the filter understands, try to validate them.
                // 5. If the credentials are bad, set the error result.
                if (string.IsNullOrEmpty(authorization.Parameter))
                {
                    context.ErrorResult = new AuthenticationFailureResult("Missing credentials", request);
                    return;
                }

                if (authorization.Parameter == null)
                {
                    context.ErrorResult = new AuthenticationFailureResult("Invalid credentials", request);
                }
                else
                {
                    try
                    {
                        string json = JWT.Decode(authorization.Parameter, _secretKey, JwsAlgorithm.HS384);

                        var claim = JsonConvert.DeserializeObject<CX_JWT_Payload>(json);

                        DateTime utcNow = DateTime.UtcNow;
                        long unixTime = ((DateTimeOffset)utcNow).ToUnixTimeSeconds();

                        if (unixTime > claim.exp)
                        {
                            context.ErrorResult = new AuthenticationFailureResult("Token Expired. Request a New One.", request);
                        }

                        GenericIdentity identity = new GenericIdentity(claim.sub.ToString(), "Bearer");
                        var roles = new string[] { "Standard" };
                        context.Principal = new GenericPrincipal(identity, roles);
                    }
                    catch (IntegrityException)
                    {
                        context.ErrorResult = new AuthenticationFailureResult("Token Integrity Compromised", request);
                    }
                    catch (Exception)
                    {
                        context.ErrorResult = new AuthenticationFailureResult("Unauthorized", request);
                    }
                }
            });
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() =>
            {
                context.ChallengeWith("Bearer");
            });
        }
    }
}
