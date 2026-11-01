using Blazor_SiteModel.Models;

namespace Blazor_SiteModel.Services;

public interface ICatalogService
{
    IReadOnlyList<ServiceCategory> GetCategories();

    IReadOnlyList<ServiceItem> GetPopular();

    IReadOnlyList<PartCategory> GetParts();

    ServiceItem? Find(string slug);

    ServiceCategory? FindCategoryByItem(string slug);

    ServiceCategory? FindCategory(string slug);

    PartCategory? FindPart(string slug);
}

public sealed class CatalogService : ICatalogService
{
    private readonly IReadOnlyList<ServiceCategory> _categories = Build();
    private readonly IReadOnlyList<PartCategory> _parts = BuildParts();

    public IReadOnlyList<ServiceCategory> GetCategories() => _categories;

    public IReadOnlyList<PartCategory> GetParts() => _parts;

    public ServiceItem? Find(string slug) =>
        _categories.SelectMany(c => c.Items).FirstOrDefault(i => i.Slug == slug);

    public ServiceCategory? FindCategory(string slug) =>
        _categories.FirstOrDefault(c => c.Slug == slug);

    public ServiceCategory? FindCategoryByItem(string slug) =>
        _categories.FirstOrDefault(c => c.Items.Any(i => i.Slug == slug));

    public PartCategory? FindPart(string slug) =>
        _parts.FirstOrDefault(p => p.Slug == slug);

    public IReadOnlyList<ServiceItem> GetPopular()
    {
        string[] slugs =
        [
            "kompyuternaya-diagnostika",
            "zamena-akkumulyatora",
            "remont-dvigatelya",
            "zapravka-konditsionera",
            "diagnostika-podveski",
            "regulirovka-razvala",
            "shinomontazh",
            "moyka"
        ];

        return slugs.Select(Find).OfType<ServiceItem>().ToList();
    }

    private static ServiceItem S(string title, string slug, string icon, int price, int minutes, string summary) =>
        new(title, slug, icon, price, minutes, summary);

    private static ServiceCategory C(string title, string slug, string icon, params ServiceItem[] items) =>
        new(title, slug, icon, items);

    private static IReadOnlyList<ServiceCategory> Build()
    {
        var shinomontazh = S("Шиномонтаж", "shinomontazh", "tire", 2500, 40, "Снятие, установка и балансировка колёс на легковые автомобили.");
        var zamenaKoles = S("Замена колёс", "zamena-koles", "wheel", 1600, 30, "Сезонная перекидка комплекта и проверка давления.");
        var balansirovka = S("Балансировка", "balansirovka", "tire", 1200, 25, "Балансировка четырёх колёс на станке.");
        var remontShin = S("Ремонт шин", "remont-shin", "tire", 900, 30, "Жгуты и заплатки для проколов беговой дорожки.");
        var proverkaRazvala = S("Проверка развала-схождения", "proverka-razvala", "align", 1500, 30, "Замер углов установки колёс без регулировки.");
        var hranenie = S("Хранение колёс", "hranenie-koles", "storage", 3500, 20, "Сезонное хранение комплекта на складе сервиса.");
        var planovoeTo = S("Плановое ТО", "planovoe-to", "wrench", 8900, 120, "Регламентные работы по пробегу: масла, фильтры и осмотр.");
        var expressOil = S("Экспресс-замена масла", "ekspress-zamena-masla", "oil", 1900, 25, "Замена моторного масла и фильтра без записи на полный день.");
        var oil = S("Замена масла", "zamena-masla", "oil", 2200, 40, "Масло и фильтр с осмотром утечек.");
        var airFilter = S("Замена воздушного фильтра", "zamena-vozdushnogo-filtra", "filter", 800, 15, "Установка нового воздушного фильтра двигателя.");
        var cabinFilter = S("Замена салонного фильтра", "zamena-salonnogo-filtra", "filter", 800, 20, "Замена фильтра салона, в том числе угольного.");
        var fuelFilter = S("Замена топливного фильтра", "zamena-toplivnogo-filtra", "filter", 1500, 40, "Замена фильтра в баке или под днищем.");
        var plugs = S("Замена свечей зажигания", "zamena-svechey", "spark", 1800, 40, "Комплект свечей с проверкой катушек.");
        var brakeFluid = S("Замена тормозной жидкости", "zamena-tormoznoy-zhidkosti", "brake", 1600, 40, "Полная замена жидкости с прокачкой контура.");
        var belt = S("Замена ремня", "zamena-remnya", "belt", 2500, 60, "Приводной ремень и проверка роликов.");
        var timing = S("Замена ГРМ", "zamena-grm", "belt", 9800, 180, "Комплект ГРМ: ремень или цепь, ролики и помпа по необходимости.");
        var inspection = S("Техосмотр", "tehosmotr", "diag", 1200, 40, "Проверка автомобиля по пунктам диагностической карты.");
        var computer = S("Компьютерная диагностика", "kompyuternaya-diagnostika", "diag", 1900, 40, "Чтение ошибок, параметров и отчёт по электронным блокам.");
        var suspensionDiag = S("Диагностика подвески", "diagnostika-podveski", "suspension", 1500, 40, "Осмотр рычагов, сайлентблоков, стоек и пыльников.");
        var brakeCheck = S("Проверка тормозов", "proverka-tormozov", "brake", 1200, 30, "Замер колодок, дисков и состояния магистралей.");
        var acDiag = S("Диагностика кондиционера", "diagnostika-konditsionera", "ac", 1400, 30, "Проверка давления, утечек и работы компрессора.");
        var evDiag = S("Диагностика электромобилей", "diagnostika-elektromobiley", "diag", 3500, 60, "Высоковольтная батарея, зарядка и ошибки силовой электроники.");
        var wash = S("Мойка", "moyka", "wash", 700, 30, "Комплексная мойка кузова и ковриков.");
        var alignment = S("Регулировка развала-схождения", "regulirovka-razvala", "align", 2800, 50, "Регулировка углов на стенде с распечаткой.");
        var bearing = S("Замена подшипника", "zamena-podshipnika", "suspension", 4500, 90, "Ступичный подшипник одной стороны.");
        var hub = S("Замена ступицы", "zamena-stupitsy", "suspension", 5200, 90, "Ступица в сборе с подшипником.");
        var arms = S("Замена рычагов", "zamena-rychagov", "suspension", 4800, 100, "Рычаг подвески и сопутствующие сайлентблоки.");
        var shocks = S("Замена амортизаторов", "zamena-amortizatorov", "suspension", 6400, 120, "Пара амортизаторов одной оси.");
        var atf = S("Замена масла в АКПП", "zamena-masla-akpp", "gear", 4200, 70, "Частичная или аппаратная замена жидкости автомата.");
        var mtf = S("Замена масла в КПП", "zamena-masla-kpp", "gear", 2800, 50, "Масло механической коробки.");
        var diff = S("Замена масла в редукторе", "zamena-masla-reduktora", "gear", 2500, 40, "Масло заднего или переднего редуктора.");
        var transfer = S("Замена масла в раздаточной коробке", "zamena-masla-razdatki", "gear", 2500, 40, "Обслуживание раздатки полноприводных авто.");
        var coolant = S("Замена антифриза", "zamena-antifriza", "coolant", 1800, 40, "Промывка и заправка системы охлаждения.");
        var pump = S("Замена помпы", "zamena-pompy", "coolant", 4500, 120, "Водяной насос и новая жидкость.");
        var radiator = S("Замена радиатора", "zamena-radiatora", "coolant", 6200, 120, "Основной радиатор охлаждения.");
        var pads = S("Замена колодок", "zamena-kolodok", "brake", 2200, 50, "Комплект колодок одной оси.");
        var discs = S("Замена дисков", "zamena-diskov", "brake", 2800, 60, "Тормозные диски одной оси вместе с колодками по необходимости.");
        var engine = S("Ремонт двигателя", "remont-dvigatelya", "engine", 15000, 240, "Дефектовка и ремонт ДВС после диагностики.");
        var gasket = S("Замена прокладок", "zamena-prokladok", "engine", 5500, 150, "Прокладки клапанной крышки, поддона или ГБЦ по результатам осмотра.");
        var seals = S("Замена сальников двигателя", "zamena-salnikov", "engine", 4800, 120, "Сальники коленвала, распредвала и устранение запотевания.");
        var starter = S("Замена стартера", "zamena-startera", "battery", 3500, 70, "Снятие и установка стартера, проверка втягивающего.");
        var generator = S("Замена генератора", "zamena-generatora", "battery", 3800, 80, "Генератор и проверка зарядки аккумулятора.");
        var lamp = S("Замена лампы", "zamena-lampy", "lamp", 600, 20, "Лампы головного света, стоп-сигналов и салона.");
        var battery = S("Замена аккумулятора", "zamena-akkumulyatora", "battery", 900, 20, "Подбор АКБ по полярности и пусковому току, установка и регистрация.");
        var acCharge = S("Заправка кондиционера", "zapravka-konditsionera", "ac", 2500, 40, "Вакуумирование и заправка хладагентом.");
        var acClean = S("Антибактериальная обработка кондиционера", "obrabotka-konditsionera", "ac", 1800, 30, "Очистка испарителя и воздуховодов.");
        var acRepair = S("Ремонт кондиционера", "remont-konditsionera", "ac", 4500, 90, "Поиск утечки, замена уплотнений, радиатора или компрессора.");
        var body = S("Малярно-кузовной ремонт", "kuzovnoy-remont", "paint", 12000, 240, "Локальный ремонт элемента: рихтовка, грунт и окраска.");

        return
        [
            C("Шиномонтаж, колёса", "kolesa", "tire", shinomontazh, zamenaKoles, balansirovka, remontShin, proverkaRazvala, hranenie),
            C("Техническое обслуживание (ТО)", "to", "wrench", planovoeTo, expressOil, oil, airFilter, cabinFilter, fuelFilter, plugs, brakeFluid, belt, timing, inspection),
            C("Диагностика", "diagnostika", "diag", computer, suspensionDiag, brakeCheck, acDiag, proverkaRazvala, evDiag),
            C("Мойка", "moyka-razdel", "wash", wash),
            C("Развал-схождение", "razval", "align", alignment, suspensionDiag),
            C("Заправка кондиционера", "konditsioner", "ac", acCharge, acDiag, acClean),
            C("Подвеска, рулевое управление", "podveska", "suspension", alignment, suspensionDiag, bearing, hub, arms, shocks),
            C("Работы с КПП, АКПП, трансмиссия", "transmissiya", "gear", atf, mtf, diff, transfer),
            C("Система охлаждения", "ohlazhdenie", "coolant", coolant, pump, radiator),
            C("Тормозная система", "tormoza", "brake", pads, discs, brakeFluid, brakeCheck),
            C("Работы с двигателем", "dvigatel", "engine", engine, gasket, plugs, seals, belt, timing, starter, generator),
            C("Работы с электрикой", "elektrika", "lamp", lamp, starter, generator, battery),
            C("Ремонт кондиционера", "remont-ac", "ac", acRepair),
            C("Малярно-кузовной ремонт", "kuzov", "paint", body)
        ];
    }

    private static IReadOnlyList<PartCategory> BuildParts() =>
    [
        new("Новые и б/у запчасти", "zapchasti", "parts", "Оригинал, аналоги и проверенные узлы с разборки."),
        new("Каталог шин", "shiny", "tire", "Летние, зимние и всесезонные шины под размер автомобиля."),
        new("Моторные масла", "masla", "oil", "Масла по допуску производителя и вязкости."),
        new("Запчасти и расходники для ТО", "rashodniki", "filter", "Фильтры, свечи, колодки и жидкости для регламента."),
        new("Щётки стеклоочистителя", "schetki", "wiper", "Каркасные и бескаркасные щётки по длине поводка."),
        new("Двигатели", "dvigateli", "engine", "Контрактные двигатели и навесное оборудование."),
        new("Кузовные элементы", "kuzovnye", "paint", "Бамперы, крылья, фары и оптика.")
    ];
}
