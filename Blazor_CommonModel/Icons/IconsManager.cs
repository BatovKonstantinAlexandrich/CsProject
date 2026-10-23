
namespace WinFormsApp.Icons
{
    public class IconsManager
    {

        public byte[] GetImageDataByName(string name)
        {
            var path = ResolveIconPath(name);
            if (path is null)
                throw new ArgumentException($"Не удалось найти файл иконки «{name}» в папке Icons (проверьте имя и расширение).");
            var data = File.ReadAllBytes(path);
            using (var stream = new MemoryStream())
            {
                stream.Write(data);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Путь к файлу в каталоге Icons или <see langword="null"/>, если не найден.
        /// Поддерживаются имена с расширением и без (.png, .ico, .jpg, …).
        /// </summary>
        public static string ResolveIconPath(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;
            var trimmed = name.Trim();
            var direct = Path.Combine("Icons", trimmed);
            if (File.Exists(direct))
                return direct;
            foreach (var ext in new[] { ".png", ".ico", ".jpg", ".jpeg", ".gif", ".bmp" })
            {
                var withExt = Path.Combine("Icons", trimmed + ext);
                if (File.Exists(withExt))
                    return withExt;
            }
            return null;
        }

        public List<string> GetIconNames()
        {
            var names = Directory.GetFiles("Icons").Select(name => name.Substring("Icons".Length + 1)).Where(name => !name.EndsWith(".cs")).ToList();
            return names;
        }

        public string GetImageByName(string name)
        {
            return name;
        }
    }


}
