using AutoMapper;
using LokerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using MyAudit;
using MyIdentityServer.Attributes;
using SdmServiceApi.Loker.Interface;
using SdmServiceApi.Loker.Resources.Pelamar;

namespace LokerMVC.Controllers;

[AuthorizeClient]
[Route("[controller]/[action]")]

public class QuizController(ILoker loker, ILogin login, IBiodata biodata, IQuiz quiz, IMapper mapper) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(int lokerId)
    {
        try
        {
            var data = await loker.GetLoker(lokerId, default);
            var infoLoker = new InfoLokerSingleView { LokerOpen = data };
            var gambarMotivasi = await login.GambarMotivasi(default);
            infoLoker.GambarMotivasi = $"data:image/png;base64,{gambarMotivasi}";
            infoLoker.KalimatMotivasi = await login.KalimatMotivasi(default);
            return View(infoLoker);
        }
        catch (Exception ex)
        {
            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }



    public async Task<IActionResult> IsiQuiz(int lokerId)
    {
        try
        {
            var result = await quiz.Apply(lokerId, default);
            var data = mapper.Map<RespondQuizView>(result);
            var dataQuiz = mapper.Map<List<DataQuiz>>(result.FormQuizz);
            data.DataQuiz.AddRange(dataQuiz);

            foreach (var item in result.FormQuizz)
            {
                if (!item.FormIsianJawabanDetailResponses.Any())
                    continue;

                var pilihan = mapper.Map<List<PilihanJawaban>>(item.FormIsianJawabanDetailResponses);
                var dataQuizUpdate = data.DataQuiz.Single(x => x.DetailJawabanId == item.DetailJawabanId);
                dataQuizUpdate.PilihanJawaban.AddRange(pilihan);
            }


            return View(data);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Biodata"))
                return RedirectToAction("ProfileEditor", "Login", new { message = "Isi No Tlp" });
            
            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> KirimJawaban(RespondQuizView data)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest("Periksa kembali jawaban");

            var jawaban = data.DataQuiz;

            foreach (var item in jawaban)
            {
                if (item.JawabanCollection != null)
                {
                    item.Jawaban = string.Join(",", item.JawabanCollection);
                }
            }

            var listJawaban = mapper.Map<List<FormJawabanRequest>>(jawaban);
            await quiz.Jawaban(data.HeaderId, listJawaban, default);

            return RedirectToAction("Profile", "Login");

        }
        catch (Exception ex)
        {
            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }


    public async Task<IActionResult> DeleteJawaban(int headerId)
    {
        try
        {
            await quiz.DeleteDaftarLoker(headerId);

            return RedirectToAction("Profile", "Login");

        }
        catch (Exception ex)
        {
            return RedirectToAction("Index", "Error", new { message = ex.Message.ToGetErrorDescription() });
        }
    }
}