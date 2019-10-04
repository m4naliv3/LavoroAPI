using Newtonsoft.Json;
using System.Net.Http;
using System.Web.Http;

namespace LavoroAPI.Controllers
{
    public class SignupController : ApiController
    {
        // GET: api/Signup/{username}
        public HttpResponseMessage Get()
        {
            string stuff = true ? "This username has already been taken" : "This username is available";
            var response = JsonConvert.SerializeObject(stuff);
            return new HttpResponseMessage() { Headers = { }, Content = new StringContent(response) };
        }

        // POST: api/Authentication
        public void Post([FromBody]NewUser user)
        {
            // need to return something to the app
            DbManager.CreateAccount(
                user.Username, 
                user.Password,
                user.BusinessName,
                user.Email,
                user.Avatar
            );
        }
    }

    public class NewUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string BusinessName { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
    }
}
