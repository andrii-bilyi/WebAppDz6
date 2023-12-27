using WebAppDz4.Data;
using System.Security.Claims;

namespace ASP_SPD_111.Middleware
{
    public class AuthSessionMiddleware
    {
        // ланцюг Middleware утворюється при інсталяції проєкту
        // і кожен учасник (ланка) Middleware одержує при створенні
        // посилання на наступну ланку (_next). Підключення Middleware
        // здійснюється у Program.cs
        private readonly RequestDelegate _next;

        public AuthSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // є дві схеми включення Middleware - синхронна та асинхронна
        // Для них передбачені методи Invoke або InvokeAsync
        public async Task Invoke(HttpContext context, DataContext _dataContext)
        {
            // Задача - перевірити наявність у сесії збереженого AuthUserId,
            // за наявності - перевірити валідність шляхом пошуку у БД
            if (context.Session.Keys.Contains("AuthUserId"))
            {
                var user = _dataContext
                    .Users
                    .Find(Guid.Parse(context.Session.GetString("AuthUserId")!));
                if (user != null)
                {
                    // перекладаємо відомості про користувача до контексту HTTP у формалізмі Claims
                    Claim[] claims = new Claim[]
                    {
                        new Claim(ClaimTypes.Sid, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Name ?? ""),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.UserData, user.Avatar ?? "")
                    };
                    context.User = new ClaimsPrincipal(
                        new ClaimsIdentity(claims, nameof(AuthSessionMiddleware)
                        )
                    );
                }
            }
            // тіло Middleware ділиться на дві частини:
            // "прямий" хід (до виклику наступної ланки) ...
            await _next(context);
            // ... та зворотній хід - після виклику.
        }        
    }
}
