using ConsumeAPINet6.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
namespace ConsumeAPINet6.Controllers
{
    public class UserValidationController : Controller
    {
        public string baseaddress = "https://putri.hotweeldev.cloud/api/";
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        public UserValidationController(ILogger<HomeController> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(User req)
        {
            if (req != null)
            {

                User user = new User
                {
                    name=req.name,
                    email=req.email,
                    password=req.password,
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseaddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage getData = await client.PostAsJsonAsync<User>("Register", user);
                    if (getData.IsSuccessStatusCode)
                    {
                        return View("~/Views/UserValidation/Login.cshtml");
                    }
                    else
                    {
                        ViewData["Error"] = "Email already exist! Please use another email";
                    }
                }
            }
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(User req)
        {
            User obj = new User();
            if (req!=null)
            {
                User user = new User
                {
                    email = req.email,
                    password = req.password,
                };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseaddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage getData = await client.PostAsJsonAsync<User>("LoginUser",user);
                    if (getData.IsSuccessStatusCode)
                    {
                        string result = getData.Content.ReadAsStringAsync().Result;
                        obj = JsonConvert.DeserializeObject<User>(result);
                        var session = _contextAccessor.HttpContext.Session;
                        session.SetString("Username", obj.name);
                        session.SetString("Userid", obj.id.ToString());
                        ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
                        return RedirectToAction("Index","Home");
                    }
                    else
                    {
                        ViewData["Error"] = "Username or Password is Invalid!!! Please Input Correctly!";
                    }
                }
            }
            return View();
        }
        public IActionResult Logout()
        {
            _contextAccessor.HttpContext.Session.Clear();
            ViewBag.Layout = "~/Views/Shared/_LayoutAwal.cshtml";
            return RedirectToAction("Login", "UserValidation");
        }
    }
}
