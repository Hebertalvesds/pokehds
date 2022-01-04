using api.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    public abstract class BaseController : Controller
    {
        protected readonly Endpoint _endpoints;
        protected readonly Context db;
        protected JsonOptions options;

        private readonly HttpClient _client;

        public BaseController(Endpoint endpoint)
        {
            db = new Context();
            _endpoints = endpoint;
            _client = new HttpClient();
        }

        protected Task<HttpResponseMessage> GetUrl(string endpoint)
        {
            return _client.GetAsync(endpoint);
        }

        public override JsonResult Json(object data)
        {
            var serializer = new JsonSerializerOptions()
            {
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                
            };
            return base.Json(data, serializer);
        }

    }
}
