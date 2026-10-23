using Console_CommonModel.ActionModels;

using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ActionModels
{
    public enum Keys
    {
        None,
        Alt,
        Control,
        Shift,
        A,
        D0,
        ShiftKey,
        ControlKey,
        Menu,
        F1,
        F24,
        D9,
        Z,
        LShiftKey,
        LMenu,
        LControlKey,
        RControlKey,
        RMenu,
        RShiftKey,
        Space,
        Oemtilde,
        OemMinus,
        Oemplus,
        OemPeriod,
        OemQuestion,
        Oem2,
        OemQuotes,
        Oem1,
        Oem7,
        OemOpenBrackets,
        Oem4,
        OemCloseBrackets,
        Oem6,
        Oem5,
        OemPipe,
        OemSemicolon,
        Oemcomma,
        Oem3,
        NumPad0,
        NumPad9
    }
    /// <summary>
    /// Нормализация строк сочетаний клавиш и сопоставление с <see cref="KeyEventArgs"/> для <see cref="EventSupport.keyboardConfig"/>.
    /// </summary>
    public static class KeyboardShortcutKeyFormatter
    {
        public static string Normalize(string shortcut)
        {
            if (string.IsNullOrWhiteSpace(shortcut))
                return string.Empty;
            var parts = shortcut.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var hasCtrl = false;
            var hasAlt = false;
            var hasShift = false;
            string main = null;
            foreach (var raw in parts)
            {
                var p = raw.Trim();
                if (p.Length == 0)
                    continue;
                var l = p.ToLowerInvariant();
                if (l is "ctrl" or "control")
                {
                    hasCtrl = true;
                    continue;
                }
                if (l == "alt")
                {
                    hasAlt = true;
                    continue;
                }
                if (l == "shift")
                {
                    hasShift = true;
                    continue;
                }
                main = NormalizeMainToken(p);
            }
            if (main is null)
                return string.Empty;
            var sb = new StringBuilder();
            if (hasCtrl)
                AppendPlus(sb, "Ctrl");
            if (hasAlt)
                AppendPlus(sb, "Alt");
            if (hasShift)
                AppendPlus(sb, "Shift");
            AppendPlus(sb, main);
            return sb.ToString();
        }

        private static void AppendPlus(StringBuilder sb, string token)
        {
            if (sb.Length > 0)
                sb.Append('+');
            sb.Append(token);
        }

        private static string NormalizeMainToken(string p)
        {
            if (p.Length == 1 && char.IsLetter(p[0]))
                return char.ToUpperInvariant(p[0]).ToString(CultureInfo.InvariantCulture);
            if (p.Length >= 2 && (p[0] == 'f' || p[0] == 'F')
                && int.TryParse(p.AsSpan(1), NumberStyles.Integer, CultureInfo.InvariantCulture, out var fn)
                && fn >= 1 && fn <= 24)
                return "F" + fn.ToString(CultureInfo.InvariantCulture);
            return p;
        }

        public static bool TryParseKeys(string shortcut, out Keys keys)
        {
            keys = Keys.None;
            var norm = Normalize(shortcut);
            if (string.IsNullOrEmpty(norm))
                return false;
            var parts = norm.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Keys modifiers = Keys.None;
            Keys main = Keys.None;
            foreach (var part in parts)
            {
                var l = part.ToLowerInvariant();
                if (l is "ctrl" or "control")
                {
                    modifiers |= Keys.Control;
                    continue;
                }
                if (l == "alt")
                {
                    modifiers |= Keys.Alt;
                    continue;
                }
                if (l == "shift")
                {
                    modifiers |= Keys.Shift;
                    continue;
                }
                if (!TryParseSingleKey(part, out main))
                    return false;
            }
            if (main == Keys.None)
                return false;
            keys = main | modifiers;
            return true;
        }

        private static bool TryParseSingleKey(string token, out Keys key)
        {
            key = Keys.None;
            if (string.IsNullOrEmpty(token))
                return false;
            token = token.Trim();
            if (token.Length == 1)
            {
                var c = token[0];
                if (c >= '0' && c <= '9')
                {
                    key = Keys.D0 + (c - '0');
                    return true;
                }
                if (c >= 'a' && c <= 'z')
                {
                    key = Keys.A + (c - 'a');
                    return true;
                }
                if (c >= 'A' && c <= 'Z')
                {
                    key = Keys.A + (c - 'A');
                    return true;
                }
                return false;
            }
            if (Enum.TryParse(token, ignoreCase: true, out Keys parsed) && parsed != Keys.None)
            {
                key = parsed;
                return true;
            }
            return false;
        }


    

        /// <summary>
        /// Строит ключ словаря в том же виде, что и <see cref="Normalize"/> для зарегистрированной строки.
        /// </summary>
        public static bool TryFormatFromKeyEvent(KeyEventArgs e, out string key)
        {
            key = null;
            if (e.KeyCode is Keys.ShiftKey or Keys.ControlKey or Keys.Menu)
                return false;
            var parts = new List<string>(4);
            if (e.Control)
                parts.Add("Ctrl");
            if (e.Alt)
                parts.Add("Alt");
            if (e.Shift)
                parts.Add("Shift");
            var main = KeyCodeToDisplayToken(e.KeyCode);
            if (main is null)
                return false;
            parts.Add(main);
            key = Normalize(string.Join("+", parts));
            return !string.IsNullOrEmpty(key);
        }

        private static string KeyCodeToDisplayToken(Keys keyCode)
        {
            if (keyCode >= Keys.A && keyCode <= Keys.Z)
                return ((char)('A' + (keyCode - Keys.A))).ToString(CultureInfo.InvariantCulture);
            if (keyCode >= Keys.D0 && keyCode <= Keys.D9)
                return ((char)('0' + (keyCode - Keys.D0))).ToString(CultureInfo.InvariantCulture);
            if (keyCode >= Keys.F1 && keyCode <= Keys.F24)
                return "F" + (keyCode - Keys.F1 + 1);
            try
            {
                var name = ConvertToString(keyCode);
                return string.IsNullOrEmpty(name) ? null : name;
            }
            catch
            {
                return null;
            }
        }

        private static string? ConvertToString(Keys keyCode)
        {
            throw new NotImplementedException();
        }
    }
}
