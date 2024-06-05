using ConsumeAPINet6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using NuGet.Common;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;

namespace ConsumeAPINet6.Controllers
{
    public class UsersController : Controller
    {
        public string baseaddress = "https://putri.hotweeldev.cloud/api/";
        private readonly ILogger<HomeController> _logger;
        public IHttpContextAccessor _contextAccessor;
        public string name;
        public string id;
        public UsersController(ILogger<HomeController> logger, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            name = _contextAccessor.HttpContext.Session.GetString("Username");
            id = _contextAccessor.HttpContext.Session.GetString("Userid");
        }

        public async Task<IActionResult> Index(int pg)
        {

            IList<User> user = new List<User>();
            if (name == null)
            {
                return RedirectToAction("Login", "UserValidation");
            }
            else
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseaddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage getData = await client.GetAsync("User");
                    if (getData.IsSuccessStatusCode)
                    {
                        string result = getData.Content.ReadAsStringAsync().Result;
                        user = JsonConvert.DeserializeObject<List<User>>(result);
                        const int pageSize = 10;
                        if (pg < 1) { pg = 1; }
                        var totitem = user.Count();
                        var pager = new Pager(totitem,pg,pageSize);
                        int recSkip = (pg-1)*pageSize;
                        
                        var data = user.Skip(recSkip).Take(pager.PageSize).ToList();

                        ViewBag.Pager = pager;

                        return View(data);

                    }
                    else
                    {
                        Console.WriteLine("Gagal Hit API!!!");
                    }
                    //ViewData.Model = user;
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IList<Course> course = new List<Course>();
/*            if (name == null)
            {
                return RedirectToAction("Login", "UserValidation");
            }*/
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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(User input)
        {
            User user = new User()
            {
                name = input.name,
                email = input.email,
                password = input.password,
                courseid = input.courseid,
            };
            if (input != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseaddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage getData = await client.PostAsJsonAsync<User>("User", user);
                    if (getData.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Users");
                    }
                    else
                    {
                        ViewData["Error"] = "Email Already Exist! Please use another email";
                    }
                }
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            UserUpdate user = new UserUpdate();
            User usermodel = new User();
            if (name == null)
            {
                return RedirectToAction("Login", "UserValidation");
            }
            if (id != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseaddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync($"User/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        user = JsonConvert.DeserializeObject<UserUpdate>(data);
                        usermodel.id = user.id;
                        usermodel.name = user.name;
                        usermodel.email= user.email;
                        usermodel.password = user.password;
                        var courseid = user.course.Select(x => x.courseid).FirstOrDefault();
                        usermodel.courseid = courseid;
                    }
                    else
                    {
                        Console.WriteLine("Gagal Hit API!!!");
                    }
                }
                IList<Course> course = new List<Course>();
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
            return View(usermodel);
        }
        [HttpPost]
        public async Task<IActionResult> Update(User input)
        {
            User user = new User()
            {
                id = input.id,
                name = input.name,
                email = input.email,
                password = input.password,
                courseid= input.courseid,
            };
            if (input != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseaddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage getData = await client.PutAsJsonAsync<User>($"User/{user.id}",user);
                    if (getData.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Users");
                    }
                    else
                    {
                        Console.WriteLine("Gagal Hit API!!!");
                    }
                }
            }
            return View();
        }

        /*[HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (name == null)
            {
                return RedirectToAction("Login", "UserValidation");
            }
            User user = new User();
            if (id != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseaddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.GetAsync($"User/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        string data = await response.Content.ReadAsStringAsync();
                        user = JsonConvert.DeserializeObject<User>(data);
                    }
                    else
                    {
                        Console.WriteLine("Gagal Hit API!!!");
                    }
                    //ViewData.Model = user;
                }
            }
            return View(user);
        }*/
/*        [HttpPost, ActionName("Delete")]*/
        public async Task<IActionResult> Delete(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseaddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage getData = await client.DeleteAsync($"User/{id}");
                if (getData.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Users");
                }
                else
                {
                    Console.WriteLine("Gagal Hit API!!!");
                }

            }
            return View();
        }
        public async Task<IActionResult> Export()
        {
            using(var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseaddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue ("application/json"));
                HttpResponseMessage getData = await client.GetAsync("User/ExportToExcel");
                if (getData.IsSuccessStatusCode)
                {
                    byte[] fileContents = await getData.Content.ReadAsByteArrayAsync();
                    string fileName = "export_data.xlsx";
                    return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
                else
                {
                    TempData["Error"] = "Data Gagal di Export!";
                    return RedirectToAction("Index", "Users");
                }
            }
            //return View();
        }
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseaddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
                HttpResponseMessage response = await client.GetAsync($"User/GetImage/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var image = response.Content.ReadAsStringAsync().Result.Trim('"');
                    ViewBag.Profile = $"data:image/jpeg;base64,{image}";
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(User user)
        {
            using (var client = new HttpClient())
            {
                if (user.image!=null && user.image.Length>0)
                {
                    user.id = int.Parse(id);
                    var formData = new MultipartFormDataContent();
                    formData.Add(new StreamContent(user.image.OpenReadStream()), "file",user.image.FileName);
                    client.BaseAddress = new Uri(baseaddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage getData = await client.PutAsync($"User/UploadImage?id={user.id}", formData);
                    if (getData.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Udah ke upload");
                    }
                }
                else
                {
                    TempData["Error"] = "Upload Image Gagal !!!";
                    return RedirectToAction("EditProfile", "Users");
                }
            }

            return View();
        }
        [HttpGet]
        public IActionResult ReadExcel()
        {
            return View();
        }
    }
    
}

