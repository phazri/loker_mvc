using AutoMapper;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using LokerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyIdentityServer.Attributes;
using Newtonsoft.Json;
using SdmServiceApi.Loker.Interface;
using SdmServiceApi.Loker.Resources.Pelamar;

namespace LokerMVC.Controllers;

[AuthorizeClient]
public class PengalamanController(IBiodata biodata, IMapper mapper, IMemoryCache memory) : Controller
{

    [HttpGet]
    public async Task<object> Get(DataSourceLoadOptions loadOptions)
    {
        try
        {
            var historyKerja = await biodata.AllHistoryKerja(default);
            var historyPengalamanKerja = mapper.Map<List<PengalamanKerjaValidator>>(historyKerja);
            memory.Set("HistoryPengalamanKerja", historyPengalamanKerja, TimeSpan.FromMinutes(10));
            return DataSourceLoader.Load(historyPengalamanKerja, loadOptions);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post(string values)
    {
        var newPengalaman = new PengalamanKerjaValidator();
        JsonConvert.PopulateObject(values, newPengalaman);
        var data = mapper.Map<PengalamanKerjaDto>(newPengalaman);
        await biodata.AddHistoryKerja(data, default);
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Put(int key, string values)
    {
       
        
        if (!memory.TryGetValue("HistoryPengalamanKerja", out List<PengalamanKerjaValidator> historyPengalamanKerja))
        {
            var historyKerja = await biodata.AllHistoryKerja(default);
            historyPengalamanKerja = mapper.Map<List<PengalamanKerjaValidator>>(historyKerja);
            memory.Set("HistoryPengalamanKerja", historyPengalamanKerja, TimeSpan.FromMinutes(10));
        }

        var pengalaman = historyPengalamanKerja.FirstOrDefault(a => a.Id == key);

        if (pengalaman == null)
            return NotFound();
        
        JsonConvert.PopulateObject(values, pengalaman);
        memory.Set("HistoryPengalamanKerja", historyPengalamanKerja, TimeSpan.FromMinutes(10));
        var data = mapper.Map<PengalamanKerjaDto>(pengalaman);
        await biodata.UpdateHistoryKerja(data, default);

        return Ok();
    }

    [HttpDelete]
    public async Task Delete(int key)
    {
       await  biodata.DeleteHistoryKerja(key, default);
    }
}