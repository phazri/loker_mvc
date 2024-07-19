using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LokerMVC.Models;

public class ResetPassword
{
    [Required(ErrorMessage = "Email wajib di isi")]
    [RegularExpression(@"^[\d\w._-]+@[\d\w._-]+\.[\w]+$", ErrorMessage = "Email is invalid")]
    [Remote("IsEmailRegistered", "RemoteValidation", ErrorMessage = "Email Belum Tidak Dikenal", HttpMethod = "POST")]
    public string Email { get; set; }

    [DisplayName("Kirim Via Email")]
    public bool IsViaEmail { get; set; }
}