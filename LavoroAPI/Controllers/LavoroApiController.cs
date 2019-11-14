using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
namespace LavoroAPI.Controllers
{
    public class LavoroApiController : ApiController
    {
        [NonAction]
        public virtual HttpResponseMessage JsonResponse(string json)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }
        [NonAction]
        public virtual HttpResponseMessage JsonResponse(object data)
        {
            string json = JsonConvert.SerializeObject(data, settings: new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }
        [NonAction]
        public virtual HttpResponseMessage JsonResponse()
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent("{}", Encoding.UTF8, "application/json");
            return response;
        }
        [NonAction]
        public virtual HttpResponseMessage XmlResponse(string xml)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(xml, Encoding.UTF8, "application/xml");
            return response;
        }

    }
}