using static TextNamingExtensions;

[Label("Инициализация минимального набора данных пользователей")]
public class DbContextUserInitializer: DbContextInitiliazer<DbContextUser>
{
    
    public override Dictionary<string, int> Init(DbContextUser context)
    {
        context.ValidateOnChanges = false;
        var result = new Dictionary<string, int>();
        //this.Info(context.Database.GetConnectionString());
        var roles = new string[] {   "Transport", "Holder", "Customer", "WebUser" };
        try
        {
            result["UserRoles"] = InitRoles(context, roles);
            result["UserGroups"] = InitGroups(context, roles);
            result[nameof(UserContext)] = InitUsers(context);
            result[nameof(UserGroupMessage)] = InitUserGroupMessages(context, roles);
            result[nameof(UserMessage)] = InitUserMessages(context);
        }
        catch (Exception ex) 
        {
            this.Error($"Исключение при инициализации первичных данных: \n{ex.ToTextDocument()}");
            throw;
        }

        return result;
    }

    private int InitUserMessages(DbContextUser context)
    {
        int result = 0;
        if (context.UserMessages.Count()==0)
        foreach(var user in context.UserContexts.ToList())
        {
            UserMessage message = new()
            {
                FromUserId = user.Id,
                ToUserId = user.Id
            };
            context.UserMessages.Add(message);
            result += context.SaveChanges();
            context.UserMessageFiles.Add(new()
            {
                FileName = "readme.txt",
                ContentType = "text",
                FileData = new byte[0],
                UserMessageId = message.Id
            });
            result += context.SaveChanges();
        }

        return result+=context.SaveChanges();
    }
     
    private int InitUserGroupMessages(DbContextUser context, string[] roles)
    {
        if (roles.Length == 0)
            throw new ArgumentException("Не заданы роли пользователей");
        if(context.UserGroupMessages.Count()==0)
        {
            foreach(var role in roles)
            {
                var group = context.CheckRegistration<UserGroup>(role.ToLower().Trim());
                var user = context.UserContexts.FirstOrDefault();
                if (user is null)
                    throw new ArgumentNullException("Нет зарегистрированных пользователей");
                context.Add(new UserGroupMessage()
                {
                    GroupId = group.Id,
                    FromUserId = user.Id,
                    Subject = "Приветствие",
                    Text = "Добро пожаловать"
                });
            }
        
            context.AddRange();
            return context.SaveChanges();
        }
        return 0;
    }


    private static string MANS_NAMES_INPUT = "А АаронАбрамАвазАвгустинАвраамАгапАгапитАгатАгафонАдамАдрианАзаматАзатАзизАидАйдарАйратАкакийАкимАланАлександрАлексейАлиАликАлимАлиханАлишерАлмазАльбертАмирАмирамАмиранАнарАнастасийАнатолийАнварАнгелАндрейАнзорАнтонАнфимАрамАристархАркадийАрманАрменАрсенАрсенийАрсланАртёмАртемийАртурАрхипАскарАсланАсханАсхатАхметАшот Б БахрамБенджаминБлезБогданБорисБориславБрониславБулат В ВадимВалентинВалерийВальдемарВарданВасилийВениаминВикторВильгельмВитВиталийВладВладимирВладиславВладленВласВсеволодВячеслав Г ГавриилГамлетГарриГеннадийГенриГенрихГеоргийГерасимГерманГерманнГлебГордейГригорийГустав Д ДавидДавлатДамирДанаДаниилДанилДанисДаниславДаниэльДаниярДарийДауренДемидДемьянДенисДжамалДжанДжеймсДжереми ИеремияДжозефДжонатанДикДинДинарДиноДмитрийДобрыняДоминик Е ЕвгенийЕвдокимЕвсейЕвстахийЕгорЕлисейЕмельянЕремейЕфимЕфрем Ж ЖданЖерарЖигер З ЗакирЗаурЗахарЗенонЗигмундЗиновийЗурабЗуфар И ИбрагимИванИгнатИгнатийИгорьИероним ДжеромИисусИльгизИльнурИльшатИльяИльясИмранИннокентийИраклийИсаакИсаакийИсидорИскандерИсламИсмаилИтан К КазбекКамильКаренКаримКарлКимКирКириллКлаусКлимКонрадКонстантинКореКорнелийКристианКузьма Л ЛаврентийЛадоЛевЛенарЛеонЛеонардЛеонидЛеопольдЛоренсЛукаЛукиллианЛукьянЛюбомирЛюдвигЛюдовикЛюций М МаджидМайклМакарМакарийМаксимМаксимилианМаксудМансурМарМаратМаркМарсельМартин МартынМатвейМахмудМикаМикулаМилославМиронМирославМихаилМоисейМстиславМуратМуслимМухаммедМэтью Н НазарНаильНариманНатанНесторНикНикитаНикодимНиколаНиколайНильсНурлан О ОгюстОлегОливерОрестОрландоОсип ИосифОскарОсманОстапОстин П ПавелПанкратПатрикПедроПерриПётрПлатонПотапПрохор Р РавильРадийРадикРадомирРадославРазильРаильРаифРайанРаймондРамазанРамзесРамизРамильРамонРанельРасимРасулРатиборРатмирРаушанРафаэльРафикРашидРинат РенатРичардРобертРодимРодионРожденРоланРоманРостиславРубенРудольфРусланРустамРэй С СавваСавелийСаидСалаватСаматСамвелСамирСамуилСанжарСаниСаянСвятославСевастьянСемёнСерафимСергейСидорСимбаСоломонСпартакСтаниславСтепанСулейманСултанСурен Т ТагирТаирТайлерТалгатТамазТамерланТарасТахирТигранТимофейТимурТихонТомасТрофим У УинслоуУмарУстин Ф ФазильФаридФархадФёдорФедотФеликсФилиппФлорФомаФредФридрих Х ХабибХакимХаритонХасан Ц ЦезарьЦефасЦецилий СесилЦицерон Ч ЧарльзЧеславЧингиз Ш ШамильШарльШерлок Э ЭдгарЭдуардЭльдарЭмильЭминЭрикЭркюльЭрминЭрнестЭузебио Ю ЮлианЮлийЮнусЮрийЮстинианЮстус Я ЯковЯнЯромирЯрослав";
    private static string WOMANS_NAMES_INPUT = "А АваАвгустаАвгустинаАвдотьяАврораАгапияАгатаАгафьяАглаяАгнияАгундаАдаАделаидаАделинаАдельАдиляАдрианаАзаАзалияАзизаАидаАишаАйАйаруАйгеримАйгульАйлинАйнагульАйнурАйсельАйсунАйсылуАксиньяАланаАлевтинаАлександраАлексияАлёнаАлестаАлинаАлисаАлияАллаАлсуАлтынАльбаАльбинаАльфияАляАмалияАмальАминаАмираАнаитАнастасияАнгелинаАнжелаАнжеликаАнисьяАнитаАннаАнтонинаАнфисаАполлинарияАрабеллаАриаднаАрианаАриандаАринаАрияАсельАсияАстридАсяАфинаАэлитаАяАяна Б БаженаБеатрисБелаБелиндаБелла БэллаБертаБогданаБоженаБьянка В ВалентинаВалерияВандаВанессаВарвараВасилинаВасилисаВенераВераВероникаВестаВетаВикторинаВикторияВиленаВиолаВиолеттаВитаВиталина ВиталияВладаВладанаВладислава Г ГабриэллаГалинаГалияГаянаГаянэГенриеттаГлафираГоарГретаГульзираГульмираГульназГульнараГульшатГюзель Д ДалидаДамираДанаДаниэлаДанияДараДаринаДарьяДаянаДжамиляДженнаДженниферДжессикаДжиневраДианаДильназДильнараДиляДилярамДинаДинараДолоресДоминикаДомнаДомника Е ЕваЕвангелинаЕвгенияЕвдокияЕкатеринаЕленаЕлизаветаЕсенияЕя Ж ЖаклинЖаннаЖансаяЖасминЖозефинаЖоржина З ЗабаваЗаираЗалинаЗамираЗараЗаремаЗаринаЗемфираЗинаидаЗитаЗлатаЗлатославаЗорянаЗояЗульфияЗухра И Иветта ИветаИзабеллаИлинаИллирикаИлонаИльзираИлюзаИнгаИндираИнессаИннаИоаннаИраИрадаИраидаИринаИрмаИскраИя К КамилаКамиллаКараКареКаримаКаринаКаролинаКираКлавдияКлараКонстанцияКораКорнелияКристинаКсения Л ЛадаЛанаЛараЛарисаЛаураЛейлаЛеонаЛераЛесяЛетаЛианаЛидияЛизаЛикаЛилиЛилианаЛилитЛилияЛинаЛиндаЛиораЛираЛияЛолаЛолитаЛораЛуизаЛукерьяЛукияЛунаЛюбаваЛюбовьЛюдмилаЛюсильЛюсьенаЛюцияЛючеЛяйсанЛяля М МавилеМавлюдаМагдаМагдалeнаМадинаМадленМайяМакарияМаликаМараМаргаритаМарианнаМарикаМаринаМарияМариямМартаМарфаМеланияМелиссаМехриМикаМилаМиладаМиланаМиленМиленаМилицаМилославаМинаМираМирославаМирраМихримахМишельМияМоникаМуза Н НадеждаНаиляНаимаНанаНаомиНаргизаНатальяНеллиНеяНикаНикольНинаНинельНоминаНоннаНораНурия О ОдеттаОксанаОктябринаОлесяОливияОльгаОфелия П ПавлинаПамелаПатрицияПаулаПейтонПелагеяПеризатПлатонидаПолинаПрасковья Р РавшанаРадаРазинаРаиляРаисаРаифаРалинаРаминаРаянаРебеккаРегинаРезедаРенаРенатаРианаРианнаРикардаРиммаРинаРитаРогнедаРозаРоксанаРоксоланаРузалияРузаннаРусалинаРусланаРуфинаРуфь С СабинаСабринаСажидаСаидаСалимаСаломеяСальмаСамираСандраСанияСараСатиСаулеСафияСафураСаянаСветланаСевараСеленаСельмаСерафимаСильвияСимонаСнежанаСоняСофьяСтеллаСтефанияСусанна Т ТаисияТамараТамилаТараТатьянаТаяТаянаТеонаТерезаТеяТинаТиффаниТомирисТораТэмми У УльянаУмаУрсулаУстинья Ф ФазиляФаинаФаридаФаризаФатимаФедораФёклаФелиситиФелицияФерузаФизалияФирузаФлораФлорентинаФлоренция ФлоренсФлорианаФредерикаФрида Х ХадияХилариХлояХюррем Ц ЦаганаЦветанаЦецилия СесилияЦиара Сиара Ч ЧелсиЧеславаЧулпан Ш ШакираШарлоттаШахинаШейлаШеллиШерил Э ЭвелинаЭвитаЭлеонораЭлианаЭлизаЭлинаЭллаЭльвинаЭльвираЭльмираЭльнараЭляЭмилиЭмилияЭммаЭнжеЭрикаЭрминаЭсмеральдаЭсмираЭстерЭтельЭтери Ю ЮлианнаЮлияЮнаЮнияЮнона Я ЯдвигаЯнаЯнинаЯринаЯрославаЯсмина";
    private static string MANS_SECONDNAMES_INPUT = "АлексеевичАнатольевичАндреевичАнтоновичАркадьевичАртемовичБедросовичБогдановичБорисовичВалентиновичВалерьевичВасильевичВикторовичВитальевичВладимировичВладиславовичВольфовичВячеславовичГеннадиевичГеоргиевичГригорьевичДаниловичДенисовичДмитриевичЕвгеньевичЕгоровичЕфимовичИвановичИванычИгнатьевичИгоревичИльичИосифовичИсааковичКирилловичКонстантиновичЛеонидовичЛьвовичМаксимовичМатвеевичМихайловичНиколаевичОлеговичПавловичПалычПетровичПлатоновичРобертовичРомановичСанычСевериновичСеменовичСергеевичСтаниславовичСтепановичТарасовичТимофеевичФедоровичФеликсовичФилипповичЭдуардовичЮрьевичЯковлевичЯрославович";
    private static string MANS_SURNAMES_INPUT = "СмирновИвановКузнецовСоколовПоповЛебедевКозловНовиковМорозовПетровВолковСоловьёвВасильевЗайцевПавловСемёновГолубевВиноградовБогдановВоробьёвФёдоровМихайловБеляевТарасовБеловКомаровОрловКиселёвМакаровАндреевКовалёвИльинГусевТитовКузьминКудрявцевБарановКуликовАлексеевСтепановЯковлевСорокинСергеевРомановЗахаровБорисовКоролёвГерасимовПономарёвГригорьевЛазаревМедведевЕршовНикитинСоболевРябовПоляковЦветковДаниловЖуковФроловЖуравлёвНиколаевКрыловМаксимовСидоровОсиповБелоусовФедотовДорофеевЕгоровМатвеевБобровДмитриевКалининАнисимовПетуховАнтоновТимофеевНикифоровВеселовФилипповМарковБольшаковСухановМироновШиряевАлександровКоноваловШестаковКазаковЕфимовДенисовГромовФоминДавыдовМельниковЩербаковБлиновКолесниковКарповАфанасьевВласовМасловИсаковТихоновАксёновГавриловРодионовКотовГорбуновКудряшовБыковЗуевТретьяковСавельевПановРыбаковСуворовАбрамовВороновМухинАрхиповТрофимовМартыновЕмельяновГоршковЧерновОвчинниковСелезнёвПанфиловКопыловМихеевГалкинНазаровЛобановЛукинБеляковПотаповНекрасовХохловЖдановНаумовШиловВоронцовЕрмаковДроздовИгнатьевСавинЛогиновСафоновКапустинКирилловМоисеевЕлисеевКошелевКостинГорбачёвОреховЕфремовИсаевЕвдокимовКалашниковКабановНосковЮдинКулагинЛапинПрохоровНестеровХаритоновАгафоновМуравьёвЛарионовФедосеевЗиминПахомовШубинИгнатовФилатовКрюковРоговКулаковТерентьевМолчановВладимировАртемьевГурьевЗиновьевГришинКононовДементьевСитниковСимоновМишинФадеевКомиссаровМамонтовНосовГуляевШаровУстиновВишняковЕвсеевЛаврентьевБрагинКонстантиновКорниловАвдеевЗыковБирюковШараповНиконовЩукинДьячковОдинцовСазоновЯкушевКрасильниковГордеевСамойловКнязевБеспаловУваровШашковБобылёвДоронинБелозёровРожковСамсоновМясниковЛихачёвБуровСысоевФомичёвРусаковСтрелковГущинТетеринКолобовСубботинФокинБлохинСеливерстовПестовКондратьевСилинМеркушевЛыткинТуров";

    public static List<string> GetManNames()
    {
        List<string> names = new List<string>();
        foreach (string text in MANS_NAMES_INPUT.Split(" "))
        {
            if (text.Length > 1)
            {
                names.AddRange(new List<string>(TextNaming.SplitName(text)));
            }
        }
        return names;
    }
    public static List<string> GetManLastnames()
    {
        List<string> names = new List<string>();
        foreach (string text in MANS_SECONDNAMES_INPUT.Split(" "))
        {
            if (text.Length > 1)
            {
                names.AddRange(new List<string>(TextNaming.SplitName(text)));
            }
        }
        return names;
    }
    public static List<string> GetManSurnames()
    {
        List<string> names = new List<string>();
        foreach (string text in MANS_SURNAMES_INPUT.Split(" "))
        {
            if (text.Length > 1)
            {
                names.AddRange(new List<string>(TextNaming.SplitName(text)));
            }
        }
        return names;
    }
    public static List<string> GetWomanNames()
    {
        List<string> names = new List<string>();
        foreach (string text in WOMANS_NAMES_INPUT.Split(" "))
        {
            if (text.Length > 1)
            {
                names.AddRange(new List<string>(TextNaming.SplitName(text)));
            }
        }
        return names;
    }
    public static List<string> MANS_NAMES = GetManNames();
    public static List<string> MANS_SURNAMES = GetManSurnames();
    public static List<string> MANS_LASTNAMES = GetManLastnames();
    static int GetRandom(int max)
    {
        int res = new Random().Next(max);
        return res == 0 ? 1 : res;
    }

    public static (string,string,string,DateTime,string) GetRandomPersonData()
    {
        int i1 = GetRandom(MANS_NAMES.Count() - 1);
        int i2 = GetRandom(MANS_SURNAMES.Count() - 1);
        int i3 = GetRandom(MANS_LASTNAMES.Count() - 1);
        if (i1 < 0 || i2 < 0 || i3 < 0)
        {
            throw new Exception("Индекс не может быть меньше нуля");
        }
        //System.Console.WriteLine($"{i1},{i2},{i3}");
        string[] names = MANS_NAMES.ToArray();
        string name = names[i1];
        string[] surnames = MANS_SURNAMES.ToArray();
        string surname = surnames[i2];
        string[] lastnames = MANS_LASTNAMES.ToArray();
        string lastname = lastnames[i3];
        var Birthday = new DateTime(DateTime.Now.Year - GetRandom(50), GetRandom(12), GetRandom(28));
        var Tel = $"79{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}";

        return (name,surname,lastname, Birthday, Tel);
    }
    public static UserPerson GetUserPerson()
    {
        int i1 = GetRandom(MANS_NAMES.Count() - 1);
        int i2 = GetRandom(MANS_SURNAMES.Count() - 1);
        int i3 = GetRandom(MANS_LASTNAMES.Count() - 1);
        if (i1 < 0 || i2 < 0 || i3 < 0)
        {
            throw new Exception("Индекс не может быть меньше нуля");
        }
        //System.Console.WriteLine($"{i1},{i2},{i3}");
        string[] names = MANS_NAMES.ToArray();
        string name = names[i1];
        string[] surnames = MANS_SURNAMES.ToArray();
        string surname = surnames[i2];
        string[] lastnames = MANS_LASTNAMES.ToArray();
        string lastname = lastnames[i3];
        return new UserPerson()
        {
            FirstName = name,
            SurName = surname,
            LastName = lastname,
            Birthday = new DateTime(DateTime.Now.Year - GetRandom(50), GetRandom(12), GetRandom(28)),
            Tel = $"79{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}",
        };
    }

    /// <summary>
    /// Создание пользователей 
    /// </summary>
    public int InitUsers(DbContextUser context )
    {
        context.EnableDataChangedAttributes = false;
        if (context is DeliveryDbContext db)
        {
            //var walletService = AppProviderService.GetInstance().Get<UserWalletService>();
            var signup = AppProviderService.GetInstance().Get<SignupUser>();
            //var signin = AppProviderService.GetInstance().Get<SigninUser<DeliveryDbContext>>();
            signup.SetDbContext(db);
            //this.Info(context.Database.GetConnectionString());
            if (context.UserContexts.Count() == 0)
            {
                foreach (var role in context.BusinessResources.ToList())
                {
                    var account = new UserAccount(role.Code + "@gmail.com", role.Code + "@gmail.com");
                    var person = GetUserPerson();
                    var result = signup.SignupRoles(account, person, new string[] { "Customer", "WebUser" });
                    if (result.Succeeded)
                    {
                        //walletService.ChangeWallet(result.Result.Id, 100);

                        var signin = AppProviderService.GetInstance().Get<SigninUser<DeliveryDbContext>>();
                        signin.SetDbContext(db);
                        signin.Signin(account);
                        //var groups = AppProviderService.GetInstance().Get<UserGroupsService<DeliveryDbContext>>();
                        //groups.SetDbContext(context);
                        //groups.AddUserToGroup(result.Result.Id );
                    }
                    else
                    {
                        throw new Exception("Не удалось зарегистрировать учетную запись");
                    }




                }

            }
            if (context.UserAccounts.Count() == 0)
                throw new Exception("Нет зарегистрированных учётных записей");
            if (context.UserLogins.Count() == 0)
            {
                context.UserLogins.Add(new(context.UserAccounts.First()));

            }
            return context.SaveChanges();
            throw new Exception();
        }
        return 0;
    }


    /// <summary>
    /// Создание ролей 
    /// </summary>
    public int InitRoles(DbContextUser context, string[] roles)
    {
        int result = 0;
        if (context.BusinessResources.ToList().Count() == 0)
        {
            foreach(var name in roles)
            {
                context.Add(new BusinessResource()
                {
                    Name = name,
                    Code = name,
                    Label = name == "Holder" ? "Партнёр" :
                    name == "Transport" ? "Служба доставки" :
                    name == "Customer" ? "Клиент" :
                    name == "Employee" ? "Сотрудник" :
                    "Пользователь сайта"
                });
                result += context.SaveChanges();
            }
            
            
            /*context.AddRange(roles.Select(name => new RoleFunction()
            {
                 Name = "Наименование функции",
                 UserRoleId = context.UserRoles_.FirstOrDefault(r => r.Code==name).Id
            }));*/
            result += context.SaveChanges();
        }
        return result;
    }


    /// <summary>
    /// Создание групп польщователей 
    /// </summary>
    public int InitGroups(DbContextUser context, string[] roles)
    {
        int result = 0;
        if (context.UserGroups.Count() == 0)
        {
            context.AddRange(roles.Select(name => new UserGroup()
            {
                Name = name,                
                Code = name,
                Label = name == "Holder" ? "Партнёр" :
                    name == "Transport" ? "Службы доставки" :
                    name == "Customer" ? "Клиенты" :
                    name == "Employee" ? "Сотрудники" :
                    "Пользователи сайта"
            }));
            result = context.SaveChanges();            
        }        
        return result;
    }

 
}
