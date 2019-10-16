using CommunicationsPlatform.BasicAuthentication.Filters;
using Jose;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LavoroAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuthenticationController : ApiController
    {
        public class LoginCreds
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public class CXAuthRequest
        {
            public string SSOKey { get; set; } // From SAML attributes (Worker Attributes)
            public string UserName { get; set; } // From Flex Agent Dashboard Redux Store.
        }
        public class CXAuthCheckRequest
        {
            public string UserName { get; set; } // From SAML attributes (Worker Attributes)
            public string Token { get; set; } // Current Bearer Token
        }
        public class CXAuthTokenCheckRequest
        {
            public string Token { get; set; } // Current Bearer Token
        }

        private readonly byte[] _secret = System.Text.Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"));
        private readonly int _ttl = 43200; // timeout after 12h
        private readonly int _preExpiryReissue = 3600; // reissue if the token is about to expire 

        // GET: api/Authentication/{username}
        public string Get(string username)
        {
            return (DbManager.CheckUser(username)) ? "This username has already been taken" : "This username is available";
        }

        // POST: api/Authentication
        public HttpResponseMessage Post([FromBody] LoginCreds login)
        {
            string json = JWT.Decode(login.Password, _secret, JwsAlgorithm.HS384);

            // need to return something to the app
            int key = DbManager.Login(login.UserName, login.Password); // needs to return the sso key

            if (key == 0) { /*handle a failed auth*/ }
            // Then we need to use the sso key to get a token and return it to the FE

            var claims = new CX_JWT_Payload { sub = key, exp = _ttl };

            var token = JWT.Encode(claims, _secret, JwsAlgorithm.HS384);
            var response = JsonConvert.SerializeObject(token);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
        }

        [Route("CX/Auth/Check")]
        [HttpPost]
        public HttpResponseMessage AuthCheck(CXAuthCheckRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.UserName))
                {
                    // Make sure that CX_Agent is still active. If not, exception will be thrown
                    // and they cannot get a new token.
                    int agent = DbManager.GetIdByUser(request.UserName);
                    var json = JWT.Decode(request.Token, _secret, JwsAlgorithm.HS384);
                    var claim = JsonConvert.DeserializeObject<CX_JWT_Payload>(json);
                    if (claim.sub != agent)
                    {
                        // Using someone else's token!
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "You are not authorized!");
                    }
                    DateTime utcNow = DateTime.UtcNow;
                    long unixTime = ((DateTimeOffset)utcNow).ToUnixTimeSeconds();
                    // If current token is expired or about to expire, get a new one.
                    if (unixTime > (claim.exp - _preExpiryReissue))
                    {
                        var claims = new CX_JWT_Payload
                        {
                            sub = agent,
                            exp = unixTime + _ttl
                        };
                        var token = JWT.Encode(claims, _secret, JwsAlgorithm.HS384);
                        var response = JsonConvert.SerializeObject(token);
                        return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
                    }
                    else
                    {
                        // Use existing good token 
                        return new HttpResponseMessage() { Headers = { }, Content = new StringContent(request.Token) };
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Missing SSOKey parameter");
                }

            }
            catch (InvalidOperationException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Agent could not be authorized for access");
            }
            catch (IntegrityException)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Token Integrity Compromised");
            }
            catch (Exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "A problem occurred and access cannot be authorized");
            }
        }

        [Route("CX/Auth/TokenCheck")]
        [HttpPost]
        public HttpResponseMessage Auth(CXAuthTokenCheckRequest request)
        {
            try
            {
                // This can be used to ascertain whether a given token is valid
                var json = JWT.Decode(request.Token, _secret, JwsAlgorithm.HS384);
                var claim = JsonConvert.DeserializeObject<CX_JWT_Payload>(json);
                DateTime utcNow = DateTime.UtcNow;
                long unixTime = ((DateTimeOffset)utcNow).ToUnixTimeSeconds();
                if (unixTime < claim.exp)
                {
                    return new HttpResponseMessage() { Headers = { }, Content = new StringContent(request.Token) };
                }
            }
            catch (Exception)
            {
            }
            // Return a 404 to hide the existence of the route
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not Found");
        }
    }
}
