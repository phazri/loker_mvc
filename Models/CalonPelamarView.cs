using SdmServiceApi.Loker.Resources.Pelamar;

namespace LokerMVC.Models;

public class CalonPelamarView
{
    public string Nama { get; set; }
    public string Alamat { get; set; }
    public string Email { get; set; }
    public string NoTlp { get; set; }
    public string TempatLahir { get; set; }
    public DateTime TglLahir { get; set; }
    public string Note { get; set; }
    public bool IsCvUploades { get; set; }
    public bool IsTgLahirValid { get; set; }
    public bool PhoneNumberConfirm { get; set; }
    public bool EmailConfirm { get; set; }
    public List<HistoryHeaderJawabanResponse> ListApply { get; set; } = new();

}

