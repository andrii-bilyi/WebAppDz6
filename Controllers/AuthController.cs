using WebAppDz4.Data;
using WebAppDz4.Services.Hash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace ASP_SPD_111.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IHashService _hashService;

        public AuthController(DataContext dataContext, IHashService hashService)
        {
            _dataContext = dataContext;
            _hashService = hashService;
        }

        [HttpDelete]
        public object SignOut()
        {
            HttpContext.Session.Remove("AuthUserId");
            return new { status = "OK" };
        }

        [HttpGet]
        public object Authenticate(String login, String password)
        {
            // шукаємо користувача з заданим логіном
            var user = _dataContext
                .Users
                .FirstOrDefault(u => u.Login == login);
            // якщо не знаходимо - надсилаємо відмову
            if(user == null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

                return new { status = "Credentials rejected" };
            }
            // користувач знайдений, формуємо DK з паролю, що надіслано, та солі, що зберігається у БД
            String dk = _hashService.HexString(user.PasswordSalt + password);
            if (user.PasswordDk != dk)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

                return new { status = "Credentials rejected" };
            }
            // перевірка на видалення аккаунта
            if(user.DeleteDt != null)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status410Gone;

                return new { status = "Credentials rejected" };
            }

            //зберігаємо у сесії факт успішної автентифікації
            HttpContext.Session.SetString("AuthUserId", user.Id.ToString());
            return new { status = "OK" };
        }
    }
}
/* Контролери
 * поділяються на MVC та API
 * MVC: різні endpoint (адреси), однакові методи запиту (GET, рідше POST)
 *   GET  /home/index
 *   GET  /home/privacy
 *   тип повернення - IActionResult (View - частіше за все)
 * API: один endpoint, різні методи запиту
 *   GET  /api/auth
 *   POST /api/auth
 *   PUT  /api/auth
 *   GET, POST, PUT, PATCH, DELETE, - для використання
 *   OPTIONS, HEAD, CONNECT, TRACE - службові, краще не чіпати
 *   тип повернення - "сирі" дані: текст або JSON
 *   якщо тип повернення String, то передається текст,
 *   інакше - автоматична JSON серіалізація повернутого об'єкту
 *   + використання статус-кодів для відповіді
 */
/* Авторизація та утримання автентифікації
* Для того, щоб знати про попередні результати автентифікації
* (не вимагати вводити пароль з кожним оновленням сторінки)
* сервер зберігає ці відомості у себе.
* При надходженні нових запитів попередньо проводиться перевірка
* на наявність збережених даних.
* AuthSessionMiddleware
*/
