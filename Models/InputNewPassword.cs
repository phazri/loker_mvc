using System.ComponentModel.DataAnnotations;

namespace LokerMVC.Models;

public class InputNewPassword
{

    [Required(ErrorMessage = "Password wajib di isi")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [Compare("Password", ErrorMessage = "'Password' and 'Confirm Password' do not match.")]
    public string ConfirmPassword { get; set; }
}