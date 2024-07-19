using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyShared.Helper;

namespace LokerMVC.Models;

public class PengalamanKerjaValidator
{
   
    public int Id { get; set; }
    public string TempatKerja { get; set; }
    public string Posisi { get; set; }
    public string Tugas { get; set; }
    [RegularExpression(@"^\d+$", ErrorMessage = "Salary terakhir harus di isi'")]
    public decimal SalaryTerakhir { get; set; }

    [DisplayName("Tgl Masuk")]
    public DateTime? TglAwal { get; set; }

    [DisplayName("Tgl Keluar")]
    public DateTime? TglAkhir { get; set; }
    public string LamaKerja => TglAwal.HasValue ? MyDateTime.HitungWaktu(TglAwal.Value.Date, TglAkhir!.Value.Date) : string.Empty;

}