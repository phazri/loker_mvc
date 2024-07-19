using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LokerMVC.Models;

public class GantiPassword
{
    [Required(ErrorMessage = "Password wajib di isi")]
    [DisplayName("Password Lama")]
    public string PasswordLama { get; set; }


    [Required(ErrorMessage = "Password wajib di isi")]
    [DisplayName("Password Baru")]
    public string PasswordBaru { get; set; }


    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("PasswordBaru", ErrorMessage = "'Password Baru' and 'Confirm Password' do not match.")]
    public string ConfirmPassword { get; set; }

}