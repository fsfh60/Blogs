using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace BlogRUS.API.Call.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    
    public class BlogController : Controller
    {

        readonly HttpClient httpClient = new HttpClient();
        [AllowAnonymous]
        [Route("controller")]


        [HttpGet]
        public async Task<IActionResult> CallAPI()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", _options.Key); var json = JsonConvert.SerializeObject(requestBody);
            //var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await client.GetAsync("http://localhost:5136/api/Blog/BlogList");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // cast


                return Ok();
            }

            return BadRequest();
        }
        //[HttpGet]
        //public async Task<string> Get(string searching)
        //{
        //    HttpClient httpClient = new HttpClient();
        //    var URL = $"http://localhost:7136/api/Blog/BlogList?{searching}";
            
        //    var response = await httpClient.GetAsync(URL);  
            
        //    return await response.Content.ReadAsStringAsync();

        //    //var response = await httpClient.GetAsync($"/api/Blog/BlogList/{searching}");
        //    //httpClient.BaseAddress = new Uri("https://localhost:7136/api/Blog/BlogList/{searching}");
        //    //return await response.Content.ReadAsStringAsync();


        //}
    }
}
 