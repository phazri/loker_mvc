using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using LokerMVC.Attribute;

namespace LokerMVC.Models;

public class ProfileRequest
{
    [Required(ErrorMessage = "Nama wajib di isi")]
    [RegularExpression(@"^[^0-9]+$", ErrorMessage = "Do not use digits in the Name.")]
    public string Nama { get; set; }

    [Required(ErrorMessage = "Alamat wajib di isi")]
    public string Alamat { get; set; }

    public string Email { get; set; }
 
    [DisplayName("Nomor Handphone (Minimal 10 digit, Maksimal 13 digit)")]
    [Required(ErrorMessage = "No Tlp wajib di isi")]
    [RegularExpression(@"^\d+$", ErrorMessage = "No Telp hanya boleh diisi dengan angka.")]
    public string NoTlp { get; set; }

    [DisplayName("Tempat Lahir")]
    [Required(ErrorMessage = "Tempat Lahir wajib di isi")]
    [RegularExpression("^[^0-9]+$", ErrorMessage = "Tempat Lahir tidak bisa di isi angka.")]
    public string TempatLahir { get; set; }

    [DisplayName("Tanggal Lahir")]
    [Required(ErrorMessage = "Tanggal Lahir wajib di isi")]
    [VerifyAge(17, 50, ErrorMessage = "Umur Minimal {0} dan umur maksimal {1}")]
    public DateTime TglLahir { get; set; }
    
    [DisplayName("Ceritakan Tentang Diri Anda / potensi / cita2 dll")]
    public string Note { get; set; }

 
}