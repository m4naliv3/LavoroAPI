using Jose;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LavoroAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuthenticationController : LavoroApiController
    {
        private readonly byte[] _secret = System.Text.Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("CX_JWT_SECRET"));
        private readonly int _ttl = 86400; // timeout after 24hr

        [Route("Auth/CheckUsername/{username}")]
        [HttpGet]
        public HttpResponseMessage UsernameAvailability(string username)
        {
            string thingy = (DbManager.CheckUser(username)) ? "This username has already been taken" : "This username is available";

            return JsonResponse(new { Message = thingy });
        }

        [Route("Auth/Login")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] LoginCreds login)
        {
            string json = JWT.Decode(login.Password, _secret, JwsAlgorithm.HS384);

            // need to return something to the app
            int key = DbManager.Login(login.UserName, login.Password); // needs to return the sso key

            if (key == 0) { /*handle a failed auth*/ }
            // Then we need to use the sso key to get a token and return it to the FE

            var claims = new CX_JWT_Payload { sub = key, exp = _ttl };

            return JsonResponse(new { Token = JWT.Encode(claims, _secret, JwsAlgorithm.HS384) });
        }
    }
}
