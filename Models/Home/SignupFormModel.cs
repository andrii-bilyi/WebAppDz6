using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAppDz4.Models.Home
{
    public class SignupFormModel
    {
        //[Required(ErrorMessage = "Поле Login обов'язкове")]
        [FromForm(Name = "signup-login")]
        public String Login { get; set; } = null!;

        //[Required(ErrorMessage = "Поле ПІБ обов'язкове")]
        [FromForm(Name = "signup-name")]
        public String Name { get; set; } = null!;


        //[Required(ErrorMessage = "Поле Email обов'язкове")]
        //[EmailAddress(ErrorMessage = "Введіть коректну адресу електронної пошти")]
        [FromForm(Name = "signup-email")]
        public String Email { get; set; } = null!;

        [FromForm(Name = "signup-password")]
        public String Password { get; set; } = null!;

        [FromForm(Name = "signup-repeat")]
        public String Repeat { get; set; } = null!;

        [FromForm(Name = "signup-avatar")]
        [JsonIgnore]
        public IFormFile Avatar { get; set; } = null!;
    }
}
