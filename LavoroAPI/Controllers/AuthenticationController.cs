using System.Collections.Generic;
using System.Web.Http;

namespace LavoroAPI.Controllers
{
    public class AuthenticationController : ApiController
    {
        // GET: api/Authentication/{username}
        public string Get(string username)
        {
            return (DbManager.CheckUser(username)) ? "This username has already been taken" : "This username is available";
        }

        // POST: api/Authentication
        public void Post([FromBody] LoginCreds login)
        {
            // need to return something to the app
            DbManager.Login(login.UserName, login.Password);
        }
    }

    public class LoginCreds
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
