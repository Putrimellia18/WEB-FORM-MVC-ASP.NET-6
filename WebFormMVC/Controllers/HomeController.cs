using ConsumeAPINet6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace ConsumeAPINet6.Controllers
{
    public class HomeController : Controller
    {
        public string baseaddress= "https://putri.hotweeldev.cloud/api/";
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        public User user = new User();
        public string name;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            name = contextAccessor.HttpContext.Session.GetString("Username");
        }
        public async Task<IActionResult> Index()
        {
            user.name = name;
            IList<Course> course = new List<Course>();
            Console.WriteLine(user.name);
            if (user.name != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseaddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage getData = await client.GetAsync("Course/WithoutUsers");
                    if (getData.IsSuccessStatusCode)
                    {
                        string result = getData.Content.ReadAsStringAsync().Result;
                        course = JsonConvert.DeserializeObject<List<Course>>(result);
                        List<SelectListItem> selectListItems = course.Select(course =>
                        new SelectListItem
                        {
                            Text = course.courseName,
                            Value = course.courseid.ToString()
                        }).ToList();

                        ViewBag.CourseList = selectListItems;
                    }
                    else
                    {
                        Console.WriteLine("Gagal Hit API Course Without Users!");
                    }
                }
        }
            else
            {
                return RedirectToAction("Login","UserValidation");
    }
            return View(user);
        }

        public IActionResult Privacy()
        {
            if(name == null)
            {
                return RedirectToAction("Login", "UserValidation");
            }
            return View();
        }
        public IActionResult Users()
        {
            if (name == null)
            {
                return RedirectToAction("Login", "UserValidation");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        /*        public async Task GetUsersByCourseId(int CourseId)
                {
                    IList <User> users = new List<User>();
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseaddress);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage getData = await client.GetAsync($"User/UserByCourseId?courseId={CourseId}");
                        if (getData != null)
                        {
                            string result = getData.Content.ReadAsStringAsync().Result;
                            users= JsonConvert.DeserializeObject<IList<User>>(result);
        *//*                    List<SelectListItem> selectList = users.Select(users => new SelectListItem
                            {
                                Text = users.name,
                                Value= users.id.ToString()
                            }).ToList();
                            ViewBag.UserList = selectList;*//*

                        }
                        else
                        {
                            Console.WriteLine("Gagal Hit API");
                        }
                    }

                }*/
        /*        public JsonResult UserByCourseId (int CourseId)
                {
                    return (GetUsersByCourseId(CourseId));

                }*/

        public async Task<JsonResult> GetUsersByCourseIdAsync(int CourseId)
        {
            IList<User> users = new List<User>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseaddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage getData = await client.GetAsync($"User/UserByCourseId?courseId={CourseId}");
                if (getData != null)
                {
                    string result = await getData.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<IList<User>>(result);
                    return Json(users); // Mengembalikan data pengguna dalam format JSON
                }
                else
                {
                    Console.WriteLine("Gagal Hit API");
                    return Json(new { success = false, message = "Gagal Hit API" }); // Mengembalikan pesan kesalahan jika gagal
                }
            }
        }
        public IActionResult Try()
        {
            return View();
        }

    }
}
