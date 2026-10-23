

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using static System.Net.Mime.MediaTypeNames;

public class RegistrationService : APIRegistration
{
    public static int Counter = 0;

    private readonly ILogger<RegistrationService> _logger;
    private ApplicationDbContext _db;
    private readonly IEmailService _email;
    private readonly AuthorizationOptions _options;

    public RegistrationService(
            ILogger<RegistrationService> logger,
            ApplicationDbContext db,
            AuthorizationOptions options,
            IEmailService email)
    {
        _logger = logger;
        _db = db;
        _email = email;
        _options = options;
    }


    /// <summary>
    /// Восстановление пароля по электронной почте
    /// </summary>
    /// <param name="Email"></param>
    public void RestorePasswordByEmail(string Email)
    {
        if (_options.LoggingAuth)
            _logger.LogInformation($"RestorePasswordByEmail({Email})");
        UserContext user = this.GetUserByEmail(Email);
        if (user == null)
        {
            throw new Exception("Пользователь с таким адресом электронной почты не зарегистрирован");
        }
        string password = GenerateRandomPassword(10);
        _db.UserAccounts.Find(user.Account.Id).Hash = GetHashSha256(password);
        _db.SaveChanges();
        _email.SendEmail(Email, "Восстановление пароля", "Пароль от Вашей учетной записи: " + password);
    }

    /// <summary>
    /// Проверка регистрации пользователя с заданным электронным адресом
    /// </summary>
    /// <param name="Email">электронный адрес</param>
    /// <returns></returns>
    public bool HasUserWithEmail(string Email)
    {
        if (string.IsNullOrWhiteSpace(Email))
            return false;
        if (_options.LoggingAuth)
            _logger.LogInformation($"HasUserWithEmail({Email})");
        var normalized = Email.Trim().ToLower();
        return _db.UserAccounts
            .AsNoTracking()
            .Any(a => a.Email != null && a.Email.ToLower() == normalized);
    }


    /// <summary>
    /// Проверка наличия пользователя с заданным номером телефона
    /// </summary>
    /// <param name="Tel">номер телефона в формате x-xxx-xxx-xxxx </param>
    /// <returns></returns>
    public bool HasUserWithTel(string Tel)
    {
        if (string.IsNullOrWhiteSpace(Tel))
            return false;
        if (_options.LoggingAuth)
            _logger.LogInformation($"HasUserWithTel({Tel})");
        return _db.UserPersons.AsNoTracking().Any(p => p.Tel == Tel);
    }


    /// <summary>
    /// Получение данных пользователя по адресу электронной почты,
    /// данный метод регистрозависимый, т.е для поиска нужно указать адрес электронной почты 
    /// в том же регистре в котором он был зарегистрирован.
    /// </summary>
    /// <param name="Email">адрес электронной почты</param>
    /// <returns></returns>
    public UserContext GetUserByEmail(string Email)
    {
        if (string.IsNullOrWhiteSpace(Email))
            return null;
        if (_options.LoggingAuth)
            _logger.LogInformation($"GetUserByEmail({Email})");

        var normalized = Email.Trim().ToLower();
        var accountId = _db.UserAccounts
            .AsNoTracking()
            .Where(a => a.Email != null && a.Email.ToLower() == normalized)
            .Select(a => (int?)a.Id)
            .FirstOrDefault();
        if (accountId == null)
            return null;

        return _db.UserContexts

            .Include(a => a.Account)
            .Include(a => a.Settings)
            .Include(a => a.Person)
            //.Include(a => a.Role)
            .Include(a => a.UserGroups)
            .FirstOrDefault(p => p.AccountId == accountId.Value);
    }


    /// <summary>
    /// Получение данных пользователя по номеру телефона, номер телефона регистрируется в формате 7-XXX-XXX-XXXX
    /// </summary>
    /// <param name="Tel">номер телефона</param>
    /// <returns></returns>
    public UserContext GetUserByTel(string Tel)
    {
        if (_options.LoggingAuth)
            _logger.LogInformation($"GetUserByTel({Tel})");
        UserPerson person = (from p in _db.UserPersons where p.Tel == Tel select p).SingleOrDefault();
        if (person == null)
        {
            return null;
        }
        else
        {
            return (from p in _db.UserContexts
                                .Include(a => a.Account)
                                .Include(a => a.Settings)
                                .Include(a => a.Person)
                                //.Include(a => a.Role)
                                .Include(a => a.UserGroups)
                    where p.PersonId == person.Id
                    select p).SingleOrDefault();
        }
    }


    /// <summary>
    /// Метод применения функции хеширования символов
    /// </summary>
    /// <param name="Password"></param>
    /// <returns></returns>
    public string GetHashSha256(string Password)
    {
        if (_options.LoggingAuth)
            _logger.LogInformation($"GetHashSha256({Password})");
        byte[] bytes = Encoding.Unicode.GetBytes(Password);
        SHA256Managed hashstring = new SHA256Managed();
        byte[] hash = hashstring.ComputeHash(bytes);
        string hashString = string.Empty;
        foreach (byte x in hash)
        {
            hashString += String.Format("{0:x2}", x);
        }
        return hashString;
    }


    /// <summary>
    /// Метод генерации случайного пароля заданной длины
    /// </summary>
    /// <param name="Length"></param>
    /// <returns></returns>
    public string GenerateRandomPassword(int Length)
    {
        if (_options.LoggingAuth)
            _logger.LogInformation($"GenerateRandomPassword({Length})");
        Random random = new Random();
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                        "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower() +
                        "0123456789";
        return new string(Enumerable.Repeat(chars, Length)
                            .Select(s => s[random.Next(s.Length)]).ToArray());

    }


    /// <summary>
    /// Проверка наличия пользователя с зарегистриваронным ключом активации
    /// </summary>
    /// <param name="ActivationKey">ключ актвации</param>
    /// <returns>true, если такой ключ уже зарегистрирован</returns>
    public bool HasUserWithActivationKey(string ActivationKey)
    {
        if (string.IsNullOrWhiteSpace(ActivationKey))
            return false;
        if (_options.LoggingAuth)
            _logger.LogInformation($"HasUserWithActivationKey({ActivationKey})");
        return _db.UserAccounts.Any(a => a.ActivationKey == ActivationKey);
    }


    /// <summary>
    /// Генерация уникального ключа активации учетной записи
    /// </summary>
    /// <param name="Length">длина ключа</param>
    /// <returns></returns>
    public string GenerateActivationKey(int Length)
    {
        if (_options.LoggingAuth)
            _logger.LogInformation($"GenerateActivationKey({Length})");

        string key = null;
        do
        {
            key = RandomString(Length);
        } while (this.HasUserWithActivationKey(key));
        return key;
    }

    /// <summary>
    /// Получение случайной последовательности символов
    /// </summary>
    /// <returns> последовательность символов </returns>
    private string RandomString(int KeyLength)
    {
        if (_options.LoggingAuth)
            _logger.LogInformation($"RandomString({KeyLength})");
        Random random = new Random();
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                        "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower() +
                        "0123456789";
        return new string(Enumerable.Repeat(chars, KeyLength)
                            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public BusinessResource GetDefaultRole()
    {
        if (_options.LoggingAuth)
            _logger.LogInformation($"GetDefaultRole( )");
        var role = (from p in _db.BusinessResources where p.Code == _options.PublicRole select p).FirstOrDefault();
        if (role == null)
        {
            _db.BusinessResources.Add(role = new BusinessResource()
            {
                Code = _options.PublicRole,
                Name = "Пользователь",
                Description = "Эта роль создана автоматически т.к. для корректной работы модуля авторизации необходимы эти данные",
                //SecretKey = Guid.NewGuid().ToString("N"),
                //LastActive = GetTimestamp(),
                //IsActive = true
            });
            _db.SaveChanges();
        }
        return role;
    }

    public string Signup(UserAccount account, UserPerson person)
    {
        Signup(account, person, GetDefaultRole());
        return "";
    }

    public void Signup(UserAccount account, UserPerson person, BusinessResource role)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));
        if (person == null)
            throw new ArgumentNullException(nameof(person));

        PrepareAccountForSignup(account);
        PreparePersonForSignup(person);

        if (role == null)
        {
            role = (from r in _db.BusinessResources
                    where r.Code == _options.PublicRole
                    select r).FirstOrDefault();
        }

        if (_options.LoggingAuth)
            _logger.LogInformation($"Signup({account.ToJson()},{person.ToJson()},{role?.ToJson()})");

        UserSettings settings = CreateDefaultSettings();
        var displayName = person.GetFullName()?.Trim();
        if (string.IsNullOrWhiteSpace(displayName))
            displayName = account.Email;
        if (_db.UserContexts.Include(p => p.Person).AsNoTracking().Any(u => u.Person.GetFullName() == displayName))
            displayName = $"{displayName} ({account.Email})";

        UserContext user = new UserContext()
        {
            Person = person,
            Account = account,
            Settings = settings,
            //Role = role,
            //Name = displayName,
            SecretKey = Guid.NewGuid().ToString("N"),
            LastActive = GetTimestamp(),
            LoginCount = 0,
            IsActive = false
        };

        UserGroup group = GetPublicGroup();
        _db.UserPersons.Add(person);
        _db.UserAccounts.Add(account);
        _db.UserSettings.Add(settings);
        _db.UserContexts.Add(user);
        _db.UserGroupsUserGroup.Add(new UserGroupBinding()
        {
            Group = group,
            User = user
        });
        _db.SaveChanges();
    }

    private static UserSettings CreateDefaultSettings()
    {
        return new UserSettings
        {
            ColorTheme = "Light",
            VertialLayout = false,
            FocusControls = false,
            PublicOperations = false,
            SendNewsToEmail = false,
            ShowHelp = true,
            EvaluateMe = false
        };
    }

    private void PrepareAccountForSignup(UserAccount account)
    {
        if (string.IsNullOrEmpty(account.Hash) && !string.IsNullOrEmpty(account.Password))
            account.Hash = GetHashSha256(account.Password);
        if (string.IsNullOrEmpty(account.ActivationKey))
            account.ActivationKey = GenerateActivationKey(_options.KeyLength);
        account.RFId ??= Guid.NewGuid().ToString("N");
        account.Activated ??= DateTime.UtcNow;
    }

    private static void PreparePersonForSignup(UserPerson person)
    {
        if (!person.Birthday.HasValue)
            throw new ValidationException("Не указана дата рождения.");
        person.SurName = person.SurName?.Trim();
        person.FirstName = person.FirstName?.Trim();
        person.LastName = person.LastName?.Trim();
        person.Tel = person.Tel?.Trim();
        person.Data ??= Array.Empty<byte>();
    }

    private UserGroup GetPublicGroup()
    {
        if (_options.LoggingAuth)
            _logger.LogInformation($"GetPublicGroup( )");
        UserGroup group = (from g in _db.UserGroups
                       where g.Code == _options.PublicGroup
                       select g).FirstOrDefault();
        if (group == null)
        {
            _db.UserGroups.Add(group = new UserGroup()
            {
                Code = _options.PublicGroup,
                Name = "Пользователи",
                Description = "Эта роль создана автоматически она объединяет всех пользователей"
            });
            _db.SaveChanges();
        }
        return group;
    }

    public void Signup(string Email, string Password, string Confirmation,
        string SurName, string FirstName, string LastName, DateTime Birthday, string Tel)
    {
        if (_options.LoggingAuth)
            _logger.LogInformation($"Signup({Email},{Password},{Confirmation},{SurName},{FirstName},{LastName},{Birthday},{Tel})");
        UserAccount account = new UserAccount()
        {
            Email = Email?.Trim(),
            Password = Password,
            Hash = GetHashSha256(Password)
        };
        account.EnsureIsValide();
        UserPerson person = new UserPerson()
        {
            SurName = SurName,
            FirstName = FirstName,
            LastName = LastName,
            Birthday = Birthday,
            Tel = Tel
        };

        person.EnsureIsValide();
        if (!string.Equals(Password, Confirmation, StringComparison.Ordinal))
            throw new ValidationException("Пароли не совпадают.");
        Signup(account, person);
    }

    /// <summary>
    /// Получечние текущего времени в милисекундах
    /// </summary>
    /// <returns></returns>
    private long GetTimestamp()
    {
        return (long)(((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0))).TotalMilliseconds);
    }

    
}
