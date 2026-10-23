using Console_CommonModel.ActionModels;


namespace Console_CommonModel.CommonLib.LibViews.ViewCustomizers;

/// <summary>
/// Кодирование одиночных клавиш (KeyChar) и сочетаний (Control+Shift+S).
/// </summary>
public static class KeyboardChordCodec
{
    static readonly string[] ModifierOrder = ["Control", "Alt", "Shift"];

    public static bool IsModifierKey(Keys key) =>
        key is Keys.ControlKey or Keys.LControlKey or Keys.RControlKey
            or Keys.Menu or Keys.LMenu or Keys.RMenu
            or Keys.ShiftKey or Keys.LShiftKey or Keys.RShiftKey;

    public static string? GetKeyName(Keys key) =>
        key == Keys.None ? null : Enum.GetName(typeof(Keys), key);

    /// <summary>Строка привязки из текущего нажатия (для KeyDown/KeyUp).</summary>
    public static string FormatFromKeyEvent(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu)
            return "";

        if (e.Control || e.Alt || e.Shift)
        {
            var parts = new List<string>();
            if (e.Control)
                parts.Add("Control");
            if (e.Alt)
                parts.Add("Alt");
            if (e.Shift)
                parts.Add("Shift");

            var main = GetKeyName(e.KeyCode);
            if (string.IsNullOrEmpty(main) || IsModifierKey(e.KeyCode))
                return "";

            parts.Add(main);
            return NormalizeChord(string.Join("+", parts));
        }

        var keyChar = TryGetKeyChar(e);
        if (keyChar.HasValue)
            return KeyboardService.FromKeyChar(keyChar.Value);

        return GetKeyName(e.KeyCode) ?? "";
    }

    /// <summary>Собрать сочетание из списка зажатых имён (как в TableControl).</summary>
    public static string FormatFromPressedNames(IEnumerable<string> pressedNames)
    {
        var parts = pressedNames
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (parts.Count == 0)
            return "";

        return NormalizeChord(string.Join("+", parts));
    }

    public static string NormalizeChord(string chord)
    {
        if (string.IsNullOrWhiteSpace(chord))
            return "";

        var parts = chord.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
            return "";

        if (parts.Length == 1)
            return parts[0];

        var mods = new List<string>();
        string main = null;
        foreach (var part in parts)
        {
            if (IsModifierName(part))
                mods.Add(NormalizeModifierName(part));
            else
                main ??= part;
        }

        mods = mods.Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(m => Array.IndexOf(ModifierOrder, m))
            .ToList();

        if (main is null && mods.Count > 0)
            return string.Join("+", mods);

        if (main is null)
            return "";

        if (mods.Count == 0)
            return main;

        return string.Join("+", mods.Concat([main]));
    }

    static bool IsModifierName(string name) =>
        name.Equals("Control", StringComparison.OrdinalIgnoreCase)
        || name.Equals("Ctrl", StringComparison.OrdinalIgnoreCase)
        || name.Equals("Alt", StringComparison.OrdinalIgnoreCase)
        || name.Equals("Shift", StringComparison.OrdinalIgnoreCase);

    static string NormalizeModifierName(string name) =>
        name.Equals("Ctrl", StringComparison.OrdinalIgnoreCase) ? "Control" :
        char.ToUpperInvariant(name[0]) + name[1..].ToLowerInvariant();

    static char? TryGetKeyChar(KeyEventArgs e)
    {
        var key = e.KeyCode;
        if (key >= Keys.A && key <= Keys.Z)
        {
            var c = (char)('A' + (key - Keys.A));
            return e.Shift ? c : char.ToLowerInvariant(c);
        }

        if (key >= Keys.D0 && key <= Keys.D9)
            return (char)('0' + (key - Keys.D0));

        if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
            return (char)('0' + (key - Keys.NumPad0));

        return key switch
        {
            Keys.Space => ' ',
            Keys.Oemtilde or Keys.Oem3 => e.Shift ? '~' : '`',
            Keys.OemMinus => e.Shift ? '_' : '-',
            Keys.Oemplus => e.Shift ? '+' : '=',
            Keys.Oemcomma => e.Shift ? '<' : ',',
            Keys.OemPeriod => e.Shift ? '>' : '.',
            Keys.OemQuestion or Keys.Oem2 => e.Shift ? '?' : '/',
            Keys.Oem1 or Keys.OemSemicolon => e.Shift ? ':' : ';',
            Keys.Oem7 or Keys.OemQuotes => e.Shift ? '"' : '\'',
            Keys.OemOpenBrackets or Keys.Oem4 => e.Shift ? '{' : '[',
            Keys.OemCloseBrackets or Keys.Oem6 => e.Shift ? '}' : ']',
            Keys.Oem5 or Keys.OemPipe => e.Shift ? '|' : '\\',
            _ => null,
        };
    }

    public static bool LooksLikeChord(string key) =>
        key.Contains('+', StringComparison.Ordinal);
}
