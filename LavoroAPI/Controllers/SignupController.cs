using System.Web.Http;

namespace LavoroAPI.Controllers
{
    public class SignupController : ApiController
    {
        // GET: api/Signup/{username}
        public string Get(string username)
        {
            return (DbManager.CheckUser(username)) ? "This username has already been taken" : "This username is available";
        }

        // POST: api/Authentication
        public void Post([FromBody]string username, string password)
        {
            // need to return something to the app
            DbManager.CreateUser(username, password);
        }
    }
}
