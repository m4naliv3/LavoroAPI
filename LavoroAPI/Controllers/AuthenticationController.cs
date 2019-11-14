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
        private readonly byte[] _secret = System.Text.Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"));
        private readonly int _ttl = 43200; // timeout after 12h
        private readonly int _preExpiryReissue = 3600; // reissue if the token is about to expire 

        [Route("Auth/CheckUsername/{username}")]
        [HttpGet]
        public HttpResponseMessage UsernameAvailability(string username)
        {
            string thingy = string.Empty;
            thingy = (DbManager.CheckUser(username)) ? "This username has already been taken" : "This username is available";

            var response = JsonConvert.SerializeObject(thingy);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
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

            var token = JWT.Encode(claims, _secret, JwsAlgorithm.HS384);
            var response = JsonConvert.SerializeObject(token);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
        }
    }
}
