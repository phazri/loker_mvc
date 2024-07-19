using LokerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyShared.Service;
using SdmServiceApi.Loker.Interface;
using TempDtoShared.Jwt;
using TempDtoShared.Login;

namespace LokerMVC.Controllers;

[Route("[controller]/[action]")]
public class HomeController(ILoker loker, ILogin login, IUtility utility, IOptions<JwtSetting> options) : Controller
{
    private readonly JwtSetting _options = options.Value;


    [Route("/")]
    public async Task<IActionResult> Index(bool isLoginFromGoogle)
    {

        if (isLoginFromGoogle)
        {
            var ipAddress = await utility.GetIpPublic(default);

            var req = new LoginHistoryRecord
            {
                Browser = HttpContext.Request.Headers["User-Agent"],
                IpAddress = ipAddress,
                Latitude = HttpContext.Session.GetString("latitude"),
                Longitude = HttpContext.Session.GetString("longitude"),
                Remarks = HttpContext.Session.GetString("remarks"),
            };
            
            await login.HistoryLogin(req);
        }

        var lokerOpen = await loker.GetAllOpenLoker(default);
        var infoLoker = new InfoLokerView();
        infoLoker.LokerOpen.AddRange(lokerOpen);

        var gambarMotivasi = await login.GambarMotivasi(default);
        infoLoker.GambarMotivasi = $"data:image/png;base64,{gambarMotivasi}";
        infoLoker.KalimatMotivasi = await login.KalimatMotivasi(default);

        return View("Index", infoLoker);
    }

    [HttpPost]
    public IActionResult SetLocation(string latitude = "", string longitude = "", string remarks = "")
    {
        HttpContext.Session.SetString("latitude", latitude);
        HttpContext.Session.SetString("longitude", longitude);
        HttpContext.Session.SetString("remarks", remarks);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult SetCookies()
    {
        if (HttpContext.Request.Method == "GET")
        {
            if (HttpContext.Request.Query.ContainsKey("token"))
            {
                var token = HttpContext.Request.Query["token"];

                HttpContext.Response.Cookies
                           .Append(_options.TokenName, token, new CookieOptions
                           {
                               MaxAge = TimeSpan.FromMinutes(60),
                               HttpOnly = true,
                               SameSite = SameSiteMode.Strict,
                               Secure = true
                           });

            }
        }

        return View();
    }
    
    public async Task<IActionResult> RefreshMotivasiImage()
    {
        var gambarMotivasi = await login.GambarMotivasi(default);
        var imageUrl = $"data:image/png;base64,{gambarMotivasi}";
        return Json(imageUrl);
    }

    public async Task<IActionResult> RefreshMotivasiText()
    {
        var newMotivasiText = await login.KalimatMotivasi(default);
        return Json(newMotivasiText);
    }
}

