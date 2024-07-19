using SdmServiceApi.Loker.Resources.Loker;

namespace LokerMVC.Models;

public class InfoLokerView 
{
    public string KalimatMotivasi { get; set; }
    public string GambarMotivasi { get; set; }
    public List<LokerOpen> LokerOpen { get; set; } = new();
}