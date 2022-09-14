using BlogRUs.Api.Models;
using BlogRUs.Api.Models.V1;
using BlogRUs.Models;
using BlogRUs.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BlogRUs.Controllers
{
    public class BlogToApiController : Controller
    {
        private BlogContext _context;
        private readonly ApiOptions _apiOptions;
        HttpClientHandler _clientHandler = new HttpClientHandler();

        public BlogToApiController(BlogContext context, IOptions<ApiOptions> options)
        {
            _context = context;
            _apiOptions = options.Value;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string searching, int? page)
        {
            var viewModel = new ListBlogViewModel()
            {
                Searching = searching,
                PageNumber = page
            };

            var client = GetHttpClient();
            using HttpResponseMessage response = await client.GetAsync(GetFullUrl("/Blog/BlogList?searching={searching}&page={page}"));
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                var dataResult = JsonConvert.DeserializeObject<Pagination<DisplayBlog>>(responseContent, settings);

                viewModel.PageNumber = page;
                viewModel.PageSize = page;
                viewModel.CurrentPage = page + 1;
                viewModel.RecordsCount = dataResult.Count;
                viewModel.TotalPages = dataResult.TotalPages;
                viewModel.Records = dataResult.Records.Select(x => new DisplayBlogViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                });
            }

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new BlogViewModel());
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] BlogViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // validate blog
                using var client = GetHttpClient();

                var blog = new Blog
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                };
                var requestBody = new StringContent(JsonConvert.SerializeObject(blog), null, "application/json");
                using HttpResponseMessage response = await client.PostAsync(GetFullUrl("/Blog"), requestBody);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Unable to save. please try again.");
            }

            return View(viewModel);
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using HttpResponseMessage response = await client.DeleteAsync($"http://localhost:5136/api/Blog/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Unable to delete. please try again.");
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            var viewModel = new BlogViewModel();

            // get the id from the api
            //if the api return 404  return NotFound()
            // if the api worked, then you show the form

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BlogViewModel viewModel)
        {
            /// you have to make sure you validate first
            if (ModelState.IsValid)
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Clear();

                var request = new BlogRequest
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                };

                var requestBody = new StringContent(JsonConvert.SerializeObject(request), null, "application/json");

                using HttpResponseMessage response = await client.PutAsync("http://localhost:5136/api/Blog/" + id.ToString(), requestBody);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Something went wrong while trying to save the data.");
            }

            return View(viewModel);
        }

        private string GetFullUrl(string url)
        {
            return _apiOptions.Url?.TrimEnd('/') + "/" + url?.TrimStart('/');
        }

        private static HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}


