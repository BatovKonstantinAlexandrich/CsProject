using Console_CommonModel.CommonLib.LibData.AbstractionModel;

[Label("Настройки")]
public partial class UserSettings : BaseEntity
{

    [Label("Вертикальное позиционирование")]
    public bool VertialLayout { get; set; }

    [Label("Включить поведение не требующее действий со стороны пользователя")]
    public bool FocusControls { get; set; }

    [Label("Цветовая схема")]
    [InputSelect("Light,Dark,Blue,Modern")]
    public string ColorTheme { get; set; }


    [Label("Публиковать мои действия")]
    public bool PublicOperations { get; set; }


    [Label("Передавать сообщения на электронную почту")]
    public bool SendNewsToEmail { get; set; }


    [Label("Показывать справочную информацию в интерактивном режиме")]
    public bool ShowHelp { get; set; }


    [Label("Оценивать мои способности работы с системой")]

    public bool EvaluateMe { get; set; }
    [Label("Включить уведомления")]
    public bool EnableNotifications { get; set; }


    [Label("Отправлять новости на электронную почту")]
    public bool SendNews { get; set; }


    [Label("Размер текста")]
    [InputSelect("малеьнкий,средний,большой")]
    public string TextSize { get; set; } = "большой";


    [Label("Меню навигации")]
    [InputSelect("сверху,Снизу")]
    public string NavPosition { get; set; } = "сверху";


    [Label("Интерактивная помощь")]
    public bool InteractiveSupport { get; set; } = true;



}