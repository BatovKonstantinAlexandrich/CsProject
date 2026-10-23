using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageModel;

using Microsoft.EntityFrameworkCore;

using pickpoint_delivery_service;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryTests
{
    /// <summary>
    /// Развёртывание тестовой компании (6 сотрудников), ролей <see cref="BusinessResource"/>,
    /// учётных записей <see cref="UserContext"/> и бизнес-процесса подбора персонала.
    /// </summary>
    [Label("Компания: подбор персонала (DeliveryDbContext)")]
    [Description(
        "Создаёт компанию из 6 человек, регистрирует BusinessResource и UserContext, " +
        "настраивает BusinessProcess «Подбор персонала» с цепочкой BusinessFunction: " +
        "пользователь сайта → HR → руководитель подразделения.")]
    public class RecruitmentCompanyBusinessProcessUnit : TestingElement
    {
        private const string ProcessName = "Recruitment";
        private const string DefaultPassword = "Test123!";

        public RecruitmentCompanyBusinessProcessUnit(IServiceProvider provider) : base(provider) { }

        public override void OnTest()
        {
            LogMessage("Развёртывание компании, ролей и процесса «Подбор персонала»");
            AssertService<DeliveryDbContext>(db =>
            {
                LogMessage("EnsureMessageInfrastructure…");
                EnsureMessageInfrastructure(db);
                var roles = EnsureBusinessResources(db);
                var team = EnsureCompanyTeam(db, roles);
                var process = EnsureRecruitmentProcess(db, roles);
                VerifyRecruitmentSetup(db, roles, team, process);
                return true;
            },
            "Компания, роли, пользователи и процесс подбора персонала зарегистрированы",
            "Не удалось развернуть тестовую компанию и бизнес-процесс подбора персонала");
        }

        private static void EnsureMessageInfrastructure(DeliveryDbContext db)
        {
            if (db.MessageAttributes.Any())
                return;

            var init = new MessagesDbContextInitializer();
            init.Init(db);
        }

        private static Dictionary<string, BusinessResource> EnsureBusinessResources(DeliveryDbContext db)
        {
            var specs = new (string Code, string Name, string Label)[]
            {
                ("Users", "Users", "Пользователи системы"),
                ("SiteApplicant", "SiteApplicant", "Пользователь сайта (кандидат)"),
                ("HrStaff", "HrStaff", "Сотрудник отдела персонала"),
                ("HiringManager", "HiringManager", "Руководитель подразделения"),
                ("CompanyStaff", "CompanyStaff", "Сотрудник компании"),
                ("CompanyExecutive", "CompanyExecutive", "Руководство компании"),
            };

            var map = new Dictionary<string, BusinessResource>(StringComparer.OrdinalIgnoreCase);
            foreach (var spec in specs)
            {
                var role = db.BusinessResources.FirstOrDefault(r => r.Code == spec.Code);
                if (role is null)
                {
                    role = new BusinessResource
                    {
                        Code = spec.Code,
                        Name = spec.Name,
                        Label = spec.Label,
                        Description = $"Тестовая роль: {spec.Label}",
                    };
                    db.BusinessResources.Add(role);
                    db.SaveChanges();
                }

                map[spec.Code] = role;
            }

            return map;
        }

        private static List<UserContext> EnsureCompanyTeam(
            DeliveryDbContext db,
            IReadOnlyDictionary<string, BusinessResource> roles)
        {
            var signup = AppProviderService.GetInstance().Get<SignupUser<DeliveryDbContext>>();
            signup.SetDbContext(db);

            var members = new[]
            {
                new MemberSpec("site.applicant@demo-company.local", "Анна", "Смирнова", "Игоревна", "79000000001", "SiteApplicant", "Кандидат с сайта"),
                new MemberSpec("hr.manager@demo-company.local", "Елена", "Козлова", "Петровна", "79000000002", "HrStaff", "Директор по персоналу"),
                new MemberSpec("hr.recruiter@demo-company.local", "Ольга", "Морозова", "Сергеевна", "79000000003", "HrStaff", "Рекрутер"),
                new MemberSpec("dept.manager@demo-company.local", "Дмитрий", "Волков", "Андреевич", "79000000004", "HiringManager", "Руководитель отдела разработки"),
                new MemberSpec("ceo@demo-company.local", "Игорь", "Новиков", "Владимирович", "79000000005", "CompanyExecutive", "Генеральный директор"),
                new MemberSpec("accountant@demo-company.local", "Мария", "Белова", "Николаевна", "79000000006", "CompanyStaff", "Главный бухгалтер"),
            };

            var users = new List<UserContext>();
            foreach (var member in members)
            {
                var user = EnsureCompanyMember(db, signup, member);
                AssignRole(db, user, roles[member.RoleCode]);
                users.Add(user);
            }

            return users;
        }

        private static UserContext EnsureCompanyMember(
            DeliveryDbContext db,
            SignupUser<DeliveryDbContext> signup,
            MemberSpec member)
        {
            var existing = db.UserContexts
                .Include(u => u.Account)
                .Include(u => u.Person)
                .Include(u => u.UsersResources)
                .FirstOrDefault(u => u.Account.Email == member.Email);

            if (existing is not null)
                return existing;

            var result = signup.Signup(
                new UserAccount { Email = member.Email, Password = DefaultPassword },
                new UserPerson
                {
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    SurName = member.SurName,
                    Birthday = new DateTime(1985, 6, 15),
                    Tel = member.Phone,
                });

            if (!result.Succeeded || result.Result is null)
                throw new InvalidOperationException(
                    $"Не удалось зарегистрировать {member.Title}: {result.Exception}");

            return result.Result;
        }

        private static void AssignRole(DeliveryDbContext db, UserContext user, BusinessResource role)
        {
            if (db.UsersResources.Any(ur => ur.UserId == user.Id && ur.RoleId == role.Id))
                return;

            user.AddBusinessFunctions(role);
            db.SaveChanges();
        }

        private static BusinessProcess EnsureRecruitmentProcess(
            DeliveryDbContext db,
            IReadOnlyDictionary<string, BusinessResource> roles)
        {
            var process = db.BusinessProcesses.FirstOrDefault(p => p.Name == ProcessName);
            if (process is null)
            {
                process = new BusinessProcess
                {
                    Name = ProcessName,
                    Label = "Подбор персонала",
                    Description =
                        "Цепочка: кандидат с сайта подаёт заявку → HR проводит первичный отбор → " +
                        "руководитель подразделения принимает решение о найме.",
                };
                db.BusinessProcesses.Add(process);
                db.SaveChanges();
            }

            if (db.BusinessFunctions.Any(f => f.Name == "SubmitJobApplication"))
                return process;

            var msgInit = new MessagesDbContextInitializer();

            var submit = CreateBusinessFunction(
                db, msgInit, roles["SiteApplicant"],
                name: "SubmitJobApplication",
                label: "Подача заявки с сайта",
                inputType: typeof(RecruitmentEnvironment),
                outputType: typeof(JobApplication),
                parentId: null);

            var hrScreening = CreateBusinessFunction(
                db, msgInit, roles["HrStaff"],
                name: "HrInitialScreening",
                label: "Первичный отбор HR",
                inputType: typeof(JobApplication),
                outputType: typeof(HrScreeningDecision),
                parentId: submit.Id);

            var managerDecision = CreateBusinessFunction(
                db, msgInit, roles["HiringManager"],
                name: "ManagerHiringDecision",
                label: "Решение руководителя о найме",
                inputType: typeof(HrScreeningDecision),
                outputType: typeof(HiringDecision),
                parentId: hrScreening.Id);

            LinkProtocolsToFunction(db, msgInit, typeof(RecruitmentEnvironment), typeof(JobApplication), submit);
            LinkProtocolsToFunction(db, msgInit, typeof(JobApplication), typeof(HrScreeningDecision), hrScreening);
            LinkProtocolsToFunction(db, msgInit, typeof(HrScreeningDecision), typeof(HiringDecision), managerDecision);

            return process;
        }

        private static BusinessFunction CreateBusinessFunction(
            DeliveryDbContext db,
            MessagesDbContextInitializer msgInit,
            BusinessResource role,
            string name,
            string label,
            Type inputType,
            Type outputType,
            int? parentId)
        {
            var input = msgInit.ToMessageProtocol(db, inputType);
            msgInit.AddOrUpdate(db, input);
            var output = msgInit.ToMessageProtocol(db, outputType);
            msgInit.AddOrUpdate(db, output);

            var function = new BusinessFunction
            {
                Name = name,
                Label = label,
                Description = label,
                BusinessResourceId = role.Id,
                InputId = input.Id,
                OutputId = output.Id,
                ParentId = parentId,
            };
            db.BusinessFunctions.Add(function);
            db.SaveChanges();
            return function;
        }

        private static void LinkProtocolsToFunction(
            DeliveryDbContext db,
            MessagesDbContextInitializer msgInit,
            Type inputType,
            Type outputType,
            BusinessFunction function)
        {
            var input = db.MessageProtocols.First(p => p.Name == inputType.GetTypeName());
            var output = db.MessageProtocols.First(p => p.Name == outputType.GetTypeName());

            input.ToBusinessFunctionId = function.Id;
            output.FromBusinessFunctionId = function.Id;
            input.BusinessResourceId = function.BusinessResourceId;
            output.BusinessResourceId = function.BusinessResourceId;
            db.SaveChanges();
        }

        private void VerifyRecruitmentSetup(
            DeliveryDbContext db,
            IReadOnlyDictionary<string, BusinessResource> roles,
            List<UserContext> team,
            BusinessProcess process)
        {
            var signin = provider.Get<SigninUser<DeliveryDbContext>>();
            signin.SetDbContext(db);

            Report(team.Count == 6, "в компании 6 учётных записей", $"ожидалось 6 пользователей, найдено {team.Count}");
            Report(roles.Count >= 6, "зарегистрированы роли BusinessResource", "не все роли созданы");

            foreach (var user in team)
            {
                var signinResult = signin.Signin(user.Account.Email, DefaultPassword);
                Report(signinResult.Succeeded,
                    $"авторизация {user.Account.Email}",
                    $"не удалось войти как {user.Account.Email}");
            }

            var functions = db.BusinessFunctions
                .Where(f => f.Name == "SubmitJobApplication"
                    || f.Name == "HrInitialScreening"
                    || f.Name == "ManagerHiringDecision")
                .ToList();

            Report(functions.Count == 3,
                "созданы 3 BusinessFunction процесса подбора",
                $"ожидалось 3 функции, найдено {functions.Count}");

            var submit = functions.First(f => f.Name == "SubmitJobApplication");
            var hr = functions.First(f => f.Name == "HrInitialScreening");
            var manager = functions.First(f => f.Name == "ManagerHiringDecision");

            Report(submit.BusinessResourceId == roles["SiteApplicant"].Id,
                "SubmitJobApplication назначена пользователю сайта",
                "неверная роль у SubmitJobApplication");
            Report(hr.BusinessResourceId == roles["HrStaff"].Id,
                "HrInitialScreening назначена отделу персонала",
                "неверная роль у HrInitialScreening");
            Report(manager.BusinessResourceId == roles["HiringManager"].Id,
                "ManagerHiringDecision назначена руководителю",
                "неверная роль у ManagerHiringDecision");
            Report(hr.ParentId == submit.Id && manager.ParentId == hr.Id,
                "BusinessFunction выстроены в цепочку сайт → HR → руководитель",
                "цепочка BusinessFunction нарушена");

            Report(db.BusinessProcesses.Any(p => p.Id == process.Id && p.Name == ProcessName),
                "BusinessProcess «Подбор персонала» зарегистрирован",
                "BusinessProcess не найден");

            Messages.Add($"Процесс «{process.Label}»: {submit.Label} → {hr.Label} → {manager.Label}.");
            Messages.Add("Состав компании: кандидат, 2 HR, руководитель отдела, CEO, бухгалтер.");
        }

        private void Report(bool passed, string ok, string fail)
        {
            Messages.Add(passed ? $"OK: {ok}" : $"FAIL: {fail}");
            if (!passed)
                Failed = true;
        }

        private sealed record MemberSpec(
            string Email,
            string FirstName,
            string LastName,
            string SurName,
            string Phone,
            string RoleCode,
            string Title);

        [Label("Среда подачи заявки")]
        public class RecruitmentEnvironment
        {
            public int Id { get; set; }

            [InputRusWord]
            [Icon("web")]
            public string Channel { get; set; }
        }

        [Label("Заявка кандидата")]
        public class JobApplication
        {
            public int Id { get; set; }

            [InputRusText]
            [Icon("person")]
            public string FullName { get; set; }

            [InputRusText]
            [Icon("work")]
            public string Position { get; set; }

            [InputEmail]
            [Icon("email")]
            public string Email { get; set; }
        }

        [Label("Решение HR после скрининга")]
        public class HrScreeningDecision
        {
            public int Id { get; set; }

            [InputRusText]
            [Icon("person")]
            public string CandidateName { get; set; }

            [InputRusText]
            [Icon("check")]
            public string ScreeningResult { get; set; }

            [InputRusText]
            [Icon("text")]
            public string HrComment { get; set; }
        }

        [Label("Решение руководителя о найме")]
        public class HiringDecision
        {
            public int Id { get; set; }

            [InputRusText]
            [Icon("person")]
            public string CandidateName { get; set; }

            [InputRusText]
            [Icon("done")]
            public string Decision { get; set; }

            [InputDate]
            [Icon("date")]
            public DateTime? StartDate { get; set; }
        }
    }
}
