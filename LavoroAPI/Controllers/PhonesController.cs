using System.Collections.Generic;
using System.Web.Http;

namespace LavoroAPI.Controllers
{
    public class PhonesController : ApiController
    {
        // GET: api/Phones
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Phones/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Phones
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Phones/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Phones/5
        public void Delete(int id)
        {
        }
    }
}
