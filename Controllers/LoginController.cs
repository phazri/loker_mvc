using System.Security.Claims;
using AutoMapper;
using LokerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyAudit;
using MyIdentityServer.Attributes;
using MyShared.Gambar;
using MyShared.Helper;
using MyShared.Service;
using Newtonsoft.Json;
using SdmServiceApi.Loker.Interface;
using SdmServiceApi.Loker.Resources.Pelamar;
using TempDtoShared.Jwt;
using TempDtoShared.Login;
using TempDtoShared.Utility;

namespace LokerMVC.Controllers;

[Route("[controller]/[action]")]
public class LoginController(ILogin login, IBiodata biodata, IUtility utility, IQuiz quiz, IMapper mapper, IOptions<JwtSetting> options, IOptions<ApiOptions> apiOptions) : Controller
{
    private readonly JwtSetting _options = options.Value;
    private readonly ApiOptions _apiOptions = apiOptions.Value;

    public IActionResult Index(string message)
    {

        ViewBag.message = message;
        return View(new EmailLoginRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginUser(EmailLoginRequest loginRequest)
    {
        try
        {
            var ipAddres = await utility.GetIpPublic(default);
            loginRequest.IpAddress = ipAddres;
            loginRequest.Browser = HttpContext.Request.Headers["User-Agent"];

            var token = await login.Login(loginRequest, default);

            HttpContext.Response.Cookies
                       .Append(_options.TokenName, token, new CookieOptions
                       {
                           MaxAge = TimeSpan.FromMinutes(60),
                           HttpOnly = true,
                           SameSite = SameSiteMode.Strict,
                           Secure = true
                       });

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            return RedirectToAction("Index", "login", new { message = ex.Message.ToGetErrorDescription() });
        }
    }

    public IActionResult Register()
    {
        return View(new RegisterRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var data = mapper.Map<RegisterCalonPelamarDto>(request);

                var result = await login.RegisterPelamar(data, default);

                return RedirectToAction("Index", "Login");
            }

            return View(new RegisterRequest());
        }
        catch (Exception ex)
        {

            return BadRequest(ex.Message);
        }
    }

    public IActionResult LupaPassword(string message)
    {
        ViewBag.message = message;
        return View(new ResetPassword());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MintaOtp(ResetPassword request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("email", request.Email); // useful for confirm password

                if (request.IsViaEmail)
                    await login.RequestOtpResetPasswordViaEmail(request.Email);
                else
                    await login.RequestOtpResetPassword(request.Email, default);

                return RedirectToAction("InputOtp", "Login", new {isResetPassword = true});
            }

            return RedirectToAction("LupaPassword", "Login");

        }
        catch (Exception ex)
        {
            var error = ex.Message.ToGetErrorDescription();
            return RedirectToAction("LupaPassword", "Login", new { message = error });
        }
    }

    [HttpGet]
    public IActionResult InputOtp(bool isResetPassword)
    {
        ViewBag.flagIsResetPassword = isResetPassword;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InputOtp(OtpModel request)
    {
        try
        {
            if (request.IsResetPassword)
            {
                var result = await login.ResetPassword(request.Otp, default);
                return RedirectToAction("InputNewPassword", "Login");
            }
            else
            {
                await login.VerificationNoTlp(request.Otp);
                return RedirectToAction("Profile", "Login");
            }

        }
        catch (Exception ex)
        {

            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }

    public IActionResult InputNewPassword()
    {
        return View(new InputNewPassword());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InputNewPassword(InputNewPassword request)
    {
        try
        {
            var email = HttpContext.Session.GetString("email");

            if (string.IsNullOrWhiteSpace(email))
                email = HttpContext.User.FindFirst(ClaimTypes.Email)!.Value;

            var data = new EmailLoginRequest
            {
                Email = email,
                Password = request.ConfirmPassword
            };

            var result = await login.BuatPasswordBaru(data, default);

            HttpContext.Session.Remove("email");


            if (HttpContext.User.IsActiveSesion())
                return RedirectToAction("Index", "Home");

            return RedirectToAction("Index", "Login");
        }
        catch (Exception ex)
        {
            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }

    public IActionResult Logout()
    {
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }

        return RedirectToAction("Index", "Home");
    }



    #region Authorize Controller

    [AuthorizeClient]
    public async Task<IActionResult> GantiPassword(string message)
    {
        try
        {
            var result = await login.IsPelamarHasPassword();
            ViewBag.IsPelamarHasPassword = result;
            ViewBag.message = message;
            return View();
        }
        catch (Exception ex)
        {

            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }


    [AuthorizeClient]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> GantiPassword(GantiPassword request)
    {
        try
        {
            var data = mapper.Map<GantiPasswordRequest>(request);
            await login.UbahPassword(data, default);
            return RedirectToAction("Profile", "Login");
        }
        catch (Exception ex)
        {
            return RedirectToAction("GantiPassword", "Login", new { message = ex.Message.ToGetErrorDescription() });
        }
    }


    [AuthorizeClient]
    public async Task<IActionResult> Profile(bool isInfoEmail)
    {
        try
        {
            ViewBag.isInfoEmail = isInfoEmail;
            var result = await login.GetPelamar(default);
            var data = mapper.Map<CalonPelamarView>(result);

            var applyLoker = await quiz.HistoryApply(default);
            data.ListApply.AddRange(applyLoker);

            var profilePhoto = await biodata.DownloadFoto(default);
            ViewBag.photo = $"data:image/png;base64,{profilePhoto}";
            return View(data);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }

    [AuthorizeClient]
    public async Task<IActionResult> ProfileEditor(string message)
    {
        try
        {
            ViewBag.MyMessage = message;
            var result = await login.GetPelamar(default);
            var data = mapper.Map<ProfileRequest>(result);
            return View(data);
        }
        catch (Exception ex)
        {

            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }

    [AuthorizeClient]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProfileEditor(ProfileRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var data = mapper.Map<RegisterCalonPelamarDto>(request);
                var result = await login.UpdatePelamar(data, default);
                return RedirectToAction("Profile", "Login");
            }

            return View(new RegisterRequest());

        }
        catch (Exception ex)
        {

            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }

    [AuthorizeClient]
    public async Task<IActionResult> GetValidationEmail()
    {
        await login.GetVerficationLinkForEmail($"{_apiOptions.UrlMySite}");
        return RedirectToAction("Profile","login", new { isInfoEmail = true});
    }

    [AuthorizeClient]
    public async Task<IActionResult> GetValidationPhone()
    {
        try
        {
            await login.GetOtpForPhoneNumber(default);
            return RedirectToAction("InputOtp", "Login");
        }
        catch (Exception ex)
        {
            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }

    [AuthorizeClient]
    public IActionResult UploadPhoto()
    {
        return View();
    }

    [AuthorizeClient]
    [HttpPost]
    public async Task<IActionResult> UploadPhoto(IFormFile Photo)
    {
        try
        {
            var result = await biodata.UploadFoto(Photo);

            return RedirectToAction("Profile", "Login");
        }
        catch (Exception ex)
        {

            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }

    [AuthorizeClient]
    public IActionResult UploadCv()
    {
        return View();
    }

    [AuthorizeClient]
    [HttpPost]
    public async Task<IActionResult> UploadCv(IFormFile file)
    {
        var chunkMetadata = Request.Form["chunkMetadata"];
        try
        {
            if (!string.IsNullOrEmpty(chunkMetadata))
            {
                var metaDataObject = JsonConvert.DeserializeObject<ChunkMetadata>(chunkMetadata);

                if (!MyFileName.IsPdf(metaDataObject.FileName))
                    return RedirectToAction("Index", "Error", new { message = "Hanya file PDF yang bisa di upload" });

                await biodata.UploadCvChunk(file, chunkMetadata, default);
            }
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        return Ok("Ok");
    }

    [AuthorizeClient]
    public async Task<IActionResult> DownloadCv()
    {

        try
        {
            var result = await biodata.DownloadCv(default);

            var fileBytes = Convert.FromBase64String(result);

            var contentType = "application/pdf";

            var fileName = "cv.pdf";

            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }

    #endregion
}