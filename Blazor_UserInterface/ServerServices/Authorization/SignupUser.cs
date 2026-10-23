using ApplicationDb.Entities;
using Core.Services.CoreAPI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

/// <summary>
/// Регистрация учётных записей с валидацией и сбором ошибок для UI.
/// </summary>
public class SignupUser
{
    public static int Counter = StartupApplication.AddScopedConfiguration<SignupUser>();

    private readonly APIRegistration _registration;

    public SignupUser(APIRegistration registration)
    {
        _registration = registration;
    }

    public SignupUserResult Register(RegistrationPageModel model)
    {
        if (model == null)
            return SignupUserResult.Fail("Не переданы данные для регистрации.");

        var person = model.Person ?? new UserPerson();
        var errors = CollectValidationErrors(model, person);
        if (errors.Count > 0)
            return SignupUserResult.Fail("Исправьте ошибки в форме.", errors);

        try
        {
            _registration.Signup(
                model.Email.Trim(),
                model.Password,
                model.PasswordConfirmation,
                person.SurName,
                person.FirstName,
                person.LastName,
                person.Birthday.Value,
                person.Tel);

            return SignupUserResult.Ok(
                "Учётная запись создана. Теперь вы можете войти, используя указанный e-mail и пароль.");
        }
        catch (ValidationException ex)
        {
            return SignupUserResult.Fail(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            return SignupUserResult.Fail(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            return SignupUserResult.Fail(ex.InnerException?.Message ?? ex.Message);
        }
    }

    private Dictionary<string, List<string>> CollectValidationErrors(RegistrationPageModel model, UserPerson person)
    {
        var errors = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(model.Password) || string.IsNullOrWhiteSpace(model.PasswordConfirmation))
        {
            AddError(errors, nameof(RegistrationPageModel.PasswordConfirmation), "Укажите пароль и подтверждение.");
        }
        else if (!string.Equals(model.Password, model.PasswordConfirmation, StringComparison.Ordinal))
        {
            AddError(errors, nameof(RegistrationPageModel.PasswordConfirmation), "Пароли не совпадают.");
        }

        var account = new UserAccount
        {
            Email = model.Email?.Trim(),
            Password = model.Password,
            Hash = string.IsNullOrEmpty(model.Password)
                ? null
                : UserAccount.GetHashSha256(model.Password)
        };

        MergeErrors(errors, account.Validate());
        MergeErrors(errors, person.Validate());

        if (!person.Birthday.HasValue)
            AddError(errors, nameof(UserPerson.Birthday), "Не указана дата рождения.");

        if (!string.IsNullOrWhiteSpace(model.Email) && _registration.HasUserWithEmail(model.Email.Trim()))
            AddError(errors, nameof(RegistrationPageModel.Email), "Этот адрес электронной почты уже зарегистрирован.");

        if (!string.IsNullOrWhiteSpace(person.Tel) && _registration.HasUserWithTel(person.Tel))
            AddError(errors, nameof(UserPerson.Tel), "Этот номер телефона уже зарегистрирован.");

        return errors;
    }

    private static void MergeErrors(Dictionary<string, List<string>> target, Dictionary<string, List<string>> source)
    {
        if (source == null)
            return;
        foreach (var entry in source)
            AddError(target, entry.Key, entry.Value);
    }

    private static void AddError(Dictionary<string, List<string>> errors, string key, string message)
    {
        if (string.IsNullOrEmpty(message))
            return;
        if (!errors.TryGetValue(key, out var list))
        {
            list = new List<string>();
            errors[key] = list;
        }
        if (!list.Contains(message))
            list.Add(message);
    }

    private static void AddError(Dictionary<string, List<string>> errors, string key, IEnumerable<string> messages)
    {
        foreach (var message in messages ?? Enumerable.Empty<string>())
            AddError(errors, key, message);
    }
}

public sealed class SignupUserResult
{
    public bool Success { get; init; }
    public string Message { get; init; }
    public IReadOnlyDictionary<string, List<string>> Errors { get; init; } =
        new Dictionary<string, List<string>>();

    public static SignupUserResult Ok(string message) =>
        new SignupUserResult { Success = true, Message = message };

    public static SignupUserResult Fail(string message, Dictionary<string, List<string>> errors = null) =>
        new SignupUserResult
        {
            Success = false,
            Message = message,
            Errors = errors ?? new Dictionary<string, List<string>>()
        };
}

public class RegistrationPageModel
{
    public UserPerson Person { get; set; } = new UserPerson();
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
}
