using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using WebAppDz4.Data;
using WebAppDz4.Models;
using WebAppDz4.Models.Home;
using WebAppDz4.Services.Hash;

namespace WebAppDz4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHashService _hashService;        
        private readonly DataContext _dataContext;

        public HomeController(ILogger<HomeController> logger, DataContext dataContext, IHashService hashService)
        {
            _logger = logger;
            _dataContext = dataContext;
            _hashService = hashService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ViewResult Db()
        {
            var users = _dataContext.Users.ToList();
            return View(users);
            //return View();
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            var user = _dataContext.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            _dataContext.Users.Remove(user);
            _dataContext.SaveChanges();

            return RedirectToAction(nameof(Db));
        }

        public ViewResult SingUp()
        {
            SignupViewModel viewModel = new();

            // перевіряємо, чи є дані від форми
            if (HttpContext.Session.Keys.Contains("formStatus"))
            {
                // декодуємо статус
                viewModel.FormStatus = Convert.ToBoolean(
                    HttpContext.Session.GetString("formStatus"));
                HttpContext.Session.Remove("formStatus");

                // перевіряємо - якщо помилковий, то у сесії дані валідації і моделі
                if (viewModel.FormStatus ?? false)
                {
                    viewModel.FormModel = null;      
                    viewModel.FormValidation = null;
                    return View(viewModel);
                }
                else
                {
                    viewModel.FormModel = JsonSerializer
                        .Deserialize<SignupFormModel>(
                            HttpContext.Session.GetString("formModel")!);
                    HttpContext.Session.Remove("formModel");

                    viewModel.FormValidation = JsonSerializer
                        .Deserialize<SignupFormValidation>(
                            HttpContext.Session.GetString("formValidation")!);
                    HttpContext.Session.Remove("formValidation");
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        public RedirectToActionResult SignupForm(SignupFormModel model)
        {
            SignupFormValidation results = new();
            bool isFormValid = true;
            var users = _dataContext.Users.ToList();
            if (String.IsNullOrEmpty(model.Login))
            {
                results.LoginErrorMessage = "Логін не може бути порожнім";
                isFormValid = false;
            }
            else
            {
                foreach (var user in users)
                {
                    if (user.Login == model.Login)
                    {
                        results.LoginErrorMessage = "Помилка! Користувач з таким логіном вже існує!";
                        isFormValid = false;
                    }
                }
            }
            if (String.IsNullOrEmpty(model.Name))
            {
                results.NameErrorMessage = "ПІБ не може бути порожнім";
                isFormValid = false;
            }
            if (String.IsNullOrEmpty(model.Email))
            {
                results.EmailErrorMessage = "Email не може бути порожнім";
                isFormValid = false;
            }
            else
            {
                foreach (var user in users)
                {
                    if (user.Email == model.Email)
                    {
                        results.EmailErrorMessage = "Помилка! Користувача з цією поштою вже зареєстровано!";
                        isFormValid = false;
                    }
                }
            }
            if (String.IsNullOrEmpty(model.Password))
            {
                results.PasswordErrorMessage = "Пароль не може бути порожним";
                isFormValid = false;
            }
            if (model.Password != model.Repeat)
            {
                results.RepeatErrorMessage = "Повтор не збігається з паролем";
                isFormValid = false;
            }

            if (isFormValid && model.Avatar != null &&
                model.Avatar.Length > 0)  // поле не обов'язкове, але якщо є, то перевіряємо
            {
                // при збереженні (uploading) файлів слід міняти їх імена.
                int dotPosition = model.Avatar.FileName.LastIndexOf(".");
                if (dotPosition == -1)
                {
                    results.AvatarErrorMessage = "Файли без розширення не приймаються";
                    isFormValid = false;
                }
                else
                {

                    var extension = Path.GetExtension(model.Avatar.FileName).ToLowerInvariant();//отримуємо розширення
                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        String ext = model.Avatar.FileName.Substring(dotPosition);
                        // TODO: додати перевірку розширення на перелік дозволених

                        // генеруємо випадкове ім'я файлу, зберігаємо розширення
                        // контролюємо, що такого імені немає у сховищі
                        String dir = Directory.GetCurrentDirectory();
                        String savedName;
                        String fileName;
                        do
                        {
                            fileName = Guid.NewGuid() + ext;
                            savedName = Path.Combine(dir, "wwwroot", "avatars", fileName);
                        }
                        while (System.IO.File.Exists(savedName));
                        using Stream stream = System.IO.File.OpenWrite(savedName);
                        model.Avatar.CopyTo(stream);

                        // до цього місця доходимо у разі відсутності помилок валідації
                        // додаємо нового користувача до БД
                        String salt = _hashService.HexString(Guid.NewGuid().ToString());
                        String dk = _hashService.HexString(salt + model.Password);
                        _dataContext.Users.Add(new()
                        {
                            Id = Guid.NewGuid(),
                            Login = model.Login,
                            Name = model.Name,
                            Avatar = fileName,
                            RegisterDt = DateTime.Now,
                            DeleteDt = null,
                            Email = model.Email,
                            PasswordSalt = salt,
                            PasswordDk = dk,
                        });
                        _dataContext.SaveChanges();
                    }
                    else
                    {
                        results.AvatarErrorMessage = "Недійсний формат файлу. Дозволені лише JPG, JPEG і PNG.";
                        isFormValid = false;
                    }


                }
            }
            if (!isFormValid)
            {
                HttpContext.Session.SetString("formModel",
                    JsonSerializer.Serialize(model));

                HttpContext.Session.SetString("formValidation",
                    JsonSerializer.Serialize(results));
            }
            HttpContext.Session.SetString("formStatus",
                isFormValid.ToString());

            return RedirectToAction(nameof(SingUp));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}