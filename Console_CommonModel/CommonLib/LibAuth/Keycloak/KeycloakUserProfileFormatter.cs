using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.Keycloak
{
    /// <summary>
    /// Текстовое представление профиля Keycloak для UI.
    /// </summary>
    public static class KeycloakUserProfileFormatter
    {
        private static readonly (string ClaimType, string Label)[] Known =
        {
            ("name", "Имя"),
            ("given_name", "Имя (given_name)"),
            ("family_name", "Фамилия"),
            ("middle_name", "Отчество"),
            ("preferred_username", "Логин"),
            ("email", "Email"),
            ("email_verified", "Email подтверждён"),
            ("sub", "Идентификатор (sub)"),
            ("phone_number", "Телефон"),
            ("locale", "Локаль"),
            ("zoneinfo", "Часовой пояс"),
        };

        /// <summary>Полный многострочный профиль: известные поля сверху, остальные claims ниже.</summary>
        public static string ToMultiline(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return "(нет данных пользователя)";
            }

            var sb = new StringBuilder();
            var shown = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var (type, label) in Known)
            {
                var values = principal.FindAll(type).Select(c => c.Value).Where(v => !string.IsNullOrWhiteSpace(v)).Distinct().ToList();
                if (values.Count == 0)
                {
                    continue;
                }

                sb.AppendLine($"{label}: {string.Join("; ", values)}");
                shown.Add(type);
            }

            foreach (var group in principal.Claims.GroupBy(c => c.Type).OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
            {
                if (shown.Contains(group.Key))
                {
                    continue;
                }

                var val = string.Join("; ", group.Select(c => c.Value).Where(v => !string.IsNullOrWhiteSpace(v)).Distinct());
                if (string.IsNullOrWhiteSpace(val))
                {
                    continue;
                }

                sb.AppendLine($"{group.Key}: {val}");
            }

            var text = sb.ToString().TrimEnd();
            return string.IsNullOrEmpty(text) ? "(claims пусты — проверьте scope openid profile email)" : text;
        }

        /// <summary>Краткая строка для заголовка окна / баннера.</summary>
        public static string ToBannerLine(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return "";
            }

            var title = KeycloakUserDisplayName.FromPrincipal(principal);
            var email = principal.FindFirst("email")?.Value;
            if (!string.IsNullOrWhiteSpace(email)
                && !string.Equals(email, title, StringComparison.OrdinalIgnoreCase)
                && title != "(имя не найдено в claims — проверьте scope profile)"
                && title != "(нет данных пользователя)")
            {
                return $"{title} · {email}";
            }

            return title;
        }
    }
}
