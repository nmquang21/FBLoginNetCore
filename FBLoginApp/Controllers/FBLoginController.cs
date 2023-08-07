using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FBLoginApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FBLoginController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private const string AppId = "2821475241215882";
        private const string AppSecret = "57426680c361ad057142264a63bb4f38";
        private const string RedirectUri = "https://localhost:44338/FBLogin/login-callback";

        private readonly ILogger<FBLoginController> _logger;

        public FBLoginController(ILogger<FBLoginController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [Route("facebook-login")]
        public IActionResult FacebookLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("login-callback")
            };
            return Challenge(properties, FacebookDefaults.AuthenticationScheme);
        }

        [Route("test-login")]
        public string TestFacebookLogin()
        {
            return "test-login";
        }

        [Route("login-callback")]
        public async Task<IActionResult> Callback(string code)
        {
            // Gọi API của Facebook để nhận mã thông báo truy cập (access token)
            string tokenUrl = $"https://graph.facebook.com/v13.0/oauth/access_token?client_id={AppId}&redirect_uri={RedirectUri}&client_secret={AppSecret}&code={code}&scope=email,user_birthday,user_photos";
            using (var httpClient = new HttpClient())
            {
                var tokenResponse = await httpClient.GetAsync(tokenUrl);
                if (tokenResponse.IsSuccessStatusCode)
                {
                    var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
                    var accessToken = JObject.Parse(tokenJson)["access_token"].ToString();

                    // Gọi API của Facebook để lấy thông tin người dùng
                    string userInfoUrl = $"https://graph.facebook.com/v13.0/me?fields=id,name,email,birthday,picture&access_token={accessToken}";
                    var userInfoResponse = await httpClient.GetAsync(userInfoUrl);
                    if (userInfoResponse.IsSuccessStatusCode)
                    {
                        var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
                        var userInfo = JObject.Parse(userInfoJson);

                        // Trả về thông tin người dùng
                        var user = new FacebookUserInfo
                        {
                            Id = userInfo["id"].ToString(),
                            Name = userInfo["name"].ToString(),
                            //FullName = userInfo["fullname"].ToString(),
                            Email = userInfo["email"]?.ToString(),
                            Birthday = userInfo["birthday"]?.ToString(),
                            PictureUrl = userInfo["picture"]?["data"]?["url"]?.ToString(),
                        };
                        var redirectUrl = "http://localhost:1999/login-callback?token=1233fefjnwfni43uifweufhwiuh4ui3h98f9aaaaaaaaa";
                        return Redirect(redirectUrl);
                    }
                }
            }

            return Redirect("hppts://google.com.vn"); // Xử lý lỗi hoặc xác thực không thành công
        }

    }
    public class FacebookUserInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Birthday { get; set; }
        public string PictureUrl { get; set; }
    }
}
