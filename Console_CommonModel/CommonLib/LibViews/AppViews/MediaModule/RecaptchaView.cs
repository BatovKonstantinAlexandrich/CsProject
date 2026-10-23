using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.MediaModule;

/// <summary>
/// Виджет Google reCAPTCHA v2 (&quot;Я не робот&quot;) через WebView2.
/// После успешной проверки значение записывается в <see cref="ResponseToken"/> (поле формы <c>g-recaptcha-response</c>).
/// </summary>
[ViewUsageExample("var view = new RecaptchaView { Text = \"Google reCAPTCHA\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<recaptcha-view Text=\"Google reCAPTCHA\" Dock=\"Top\" />")]
[Label("Google reCAPTCHA")]
public class RecaptchaView : AppViewBase
{

    [Label("Ключ сайта (site key)")]
    public string SiteKey { get => Get<string>(nameof(SiteKey)); set => Set(nameof(SiteKey), value); }

    /// <summary>Токен от клиента для серверной проверки (<c>/api/siteverify</c>).</summary>

    [Label("Ответ виджета (токен)")]
    public string ResponseToken { get => Get<string>(nameof(ResponseToken)); set => Set(nameof(ResponseToken), value); }

    /// <summary>Язык интерфейса (например <c>ru</c>, <c>en</c>). Пусто — по умолчанию.</summary>

    [Label("Язык (hl)")]
    public string Language { get => Get<string>(nameof(Language)); set => Set(nameof(Language), value); }
    [Label("Тема")]
    [InputSelect("light,dark")]
    public string Theme { get => Get<string>(nameof(Theme)); set => Set(nameof(Theme), value); }
    [Label("Размер")]
    [InputSelect("normal,compact")]
    public string Size { get => Get<string>(nameof(Size)); set => Set(nameof(Size), value); }

    public RecaptchaView()
    {
        SiteKey = "";
        ResponseToken = "";
        Language = "";
        Theme = "light";
        Size = "normal";
        MinHeight = 90;
        MinWidth = 304;
    }
}
