using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SdmServiceApi.Loker.Interface;

namespace LokerMVC.Controllers;

public class RemoteValidationController(ILogin login, IMemoryCache memoryCache) : Controller
{
    [HttpPost]
    public async Task<JsonResult> CheckUniqueEmailAddress(string email)
    {     
        // Mengecek apakah email ada dalam cache
        if (memoryCache.TryGetValue<List<string>>("RegisteredEmails", out var emailRegister))
        {
            // Jika email ada dalam cache, lakukan validasi
            var isValid = !emailRegister.Any(x =>
            {
                bool isEqual = string.Equals(x, email, StringComparison.OrdinalIgnoreCase);
                return isEqual;
            });

            return Json(isValid);
        }
        else
        {
            // Jika email belum ada dalam cache, ambil dari sumber data
            emailRegister = await login.RegisterEmail(default);

            // Simpan emailRegister dalam cache untuk penggunaan selanjutnya
            memoryCache.Set("RegisteredEmails", emailRegister);

            // Lakukan validasi email
            var isValid = !emailRegister.Any(x =>
            {
                bool isEqual = string.Equals(x, email, StringComparison.OrdinalIgnoreCase);
                return isEqual;
            });

            return Json(isValid);
        }
    }


    [HttpPost]
    public async Task<JsonResult> IsEmailRegistered(string email)
    {
        // Mengecek apakah email ada dalam cache
        if (memoryCache.TryGetValue<List<string>>("RegisteredEmails", out var registeredEmails))
        {
            // Jika email ada dalam cache, lakukan validasi
            var isRegistered = registeredEmails.Any(x =>
            {
                return string.Equals(x, email, StringComparison.OrdinalIgnoreCase);
            });

            return Json(isRegistered);
        }
        else
        {
            // Jika email belum ada dalam cache, ambil dari sumber data
            registeredEmails = await login.RegisterEmail(default);

            // Simpan registeredEmails dalam cache untuk penggunaan selanjutnya
            memoryCache.Set("RegisteredEmails", registeredEmails);

            // Lakukan validasi email
            var isRegistered = registeredEmails.Any(x =>
            {
                return string.Equals(x, email, StringComparison.OrdinalIgnoreCase);
            });

            return Json(isRegistered);
        }
    }

}