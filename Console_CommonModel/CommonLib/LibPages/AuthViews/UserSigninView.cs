using Console_CommonModel.CommonLib.LibAuth.AppAuth.Keycloak;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibPages.AuthViews
{

    

    [Label("Страница авторизации")]
    public class UserSigninView<TDbContext> : AppViewBase
        where TDbContext : DeliveryDbContext

    {
        public string password, login;

        public UserAccount account = new();
        public ControlView passwordInput;
        public TextView passwordText;
        public ControlView loginInput;
        public TextView loginText;
        private TextView registrationLink;
        private TextView titleText;
        private TextView errorMessage;
        public ButtonView submitButton;
        private ButtonView loginKeyCloak;
        private TextView keycloakLink;
        private TextView loginError;
        private TextView passwordError;
        private readonly Action<UserContext> onsuccess;

        [Label("User")]
        public UserContext User { get; private set; }

        public UserSigninView() : this(_ => { }) { }

        public UserSigninView(Action<UserContext> onsuccess)
        {
            
            ShowLoginForm();
            this.onsuccess = onsuccess;
            this.Dock = "Fill";
        }

        private void ShowLoginForm()
        {
            Padding = 20;
            this.account.OnValidationStateChanged += state => { 
                if(state)
                {
                    submitButton.Enabled = true;
                }
                else
                {
                    submitButton.Enabled = false;
                }
            };
            LayoutTop(titleText = CreateTitle("Авторизация", () => { }));
            LayoutTop(loginText = CreateText("пользователь", () => { }));
            LayoutTop(loginInput = CreateTextbox(login, OnLoginChanged));
            LayoutTop(loginError = CreateRedText(" "));
            LayoutTop(passwordText = CreateText("пароль"));
            LayoutTop(passwordInput = CreateTextbox(password, OnPasswordChanged));
            LayoutTop(passwordError = CreateRedText(" "));

            LayoutTop(submitButton = CreateButton("войти", OnSubmitButtonClicked));


            LayoutTop(registrationLink = CreateTextCenterLink("регистрация", () =>
            {
                LayoutFill(new UserSignupView<TDbContext>((s, e) =>
                {
                    ShowLoginForm();
                }));
            }));
           
            account.Email = login;
            account.Password = password;

            var vr = account.Validate();

            var sb = new StringBuilder();
            if (vr.ContainsKey(nameof(account.Email)))
            {
                foreach (var error in vr[nameof(account.Email)])
                {
                    sb.AppendLine(error + "\n");
                }
            }
            loginError.Text = sb.ToString();

            sb = new StringBuilder();
            if (vr.ContainsKey(nameof(account.Password)))
            {
                loginError.Text = sb.ToString();
                sb = new StringBuilder();
                foreach (var error in vr[nameof(account.Password)])
                {
                    sb.AppendLine(error + "\n");
                }
            }
            passwordError.Text = sb.ToString();
            this.account.Validate();

            Action signinKeycloak = () =>
            {
                RunLoginAsync(keycloakUserInfo =>
                {
                    try
                    {
                        GetUserContext(keycloakUserInfo);
                    }
                    catch (Exception ex)
                    {
                        void showErr()
                        {
                            errorMessage.Text = ex.Message;
                        }
                        var ctrl = ToControl();
                        if (ctrl?.InvokeRequired == true)
                            ctrl.Invoke(showErr);
                        else
                            showErr();
                    }
                });
            };
            LayoutTop(keycloakLink = CreateTextCenterLink("войти через KeyCloak", signinKeycloak));            
            LayoutTop(errorMessage = CreateRedText(""));
            var temp = Controls.ToList();
            Controls.Clear();
            temp.Reverse();
            Controls.AddRange(temp);


            OnLayout += _ => AdjustSigninFormHeight();
            OnLoad += (_, _) => AdjustSigninFormHeight();
        }

        private void AdjustSigninFormHeight()
        {
            var form = control?.GetAppForm();
            if (form is null)
                return;
            form.Height = Controls.Sum(c =>
            {
                var child = (AppViewBase)c;
                return child.control?.Height ?? child.Height;
            }) + 200;
        }



        /// <summary>
        /// Находит локального пользователя по почте из Keycloak или открывает регистрацию с предзаполнением из claims.
        /// </summary>
        private void GetUserContext(ClaimsPrincipal keycloakUserInfo)
        {
            if (keycloakUserInfo is null)
                throw new ArgumentNullException(nameof(keycloakUserInfo));

            var email = ResolveEmailFromKeycloakClaims(keycloakUserInfo);
            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException(
                    "В ответе Keycloak не найдена электронная почта (ожидаются claims «email» или «" + ClaimTypes.Email + "»).");

            email = email.Trim();

                using var db = typeof(TDbContext).New<TDbContext>();
            var user = db.UserContexts
                .Include(u => u.Account)
                .FirstOrDefault(u =>
                    u.Account != null
                    && u.Account.Email != null
                    && u.Account.Email.ToLower() == email.ToLower());

            if (user is not null)
            {
                void ok()
                {
                    onsuccess(user);
                }
                var c = ToControl();
                if (c?.InvokeRequired == true)
                    c.Invoke(ok);
                else
                    ok();
                return;
            }

            void openSignup()
            { 
                var signupview = new UserSignupView<TDbContext>(
                    (_, _) => ShowLoginForm(),
                    keycloakUserInfo,
                    registeredUser => onsuccess(registeredUser));
                LayoutFill(signupview);
            }
            var sync = ToControl();
            if (sync?.InvokeRequired == true)
                sync.Invoke(openSignup);
            else
                openSignup();
        }

        /// <summary>Email из OIDC/Keycloak: стандартные claim-типы.</summary>
        private static string ResolveEmailFromKeycloakClaims(ClaimsPrincipal principal)
        {
            string v =
                principal.FindFirst(ClaimTypes.Email)?.Value
                ?? principal.FindFirst("email")?.Value
                ?? principal.FindFirst("upn")?.Value;
            return string.IsNullOrWhiteSpace(v) ? null : v.Trim();
        }

        private static async Task RunLoginAsync(Action<ClaimsPrincipal> onsuccess)
        {
            var keycloakOptions = KeycloakAuthOptions.LoadFromAppSettings();
            keycloakOptions.ClientId = "buhreport";
            keycloakOptions.ClientSecret = "yrWO3szptvTTSEFUtZs341OvwWk3hh0Z";
            keycloakOptions.Authority = "https://auth.euroauto.ru/realms/webapp";
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            var result = await KeycloakPkceLogin.LoginAsync(result =>
            {
                //System.Console.WriteLine(JsonConvert.SerializeObject(result));
                KeycloakAuthSession.IsAuthenticated = true;
                KeycloakAuthSession.AccessToken = result.AccessToken;
                KeycloakAuthSession.IdToken = result.IdToken;
                KeycloakAuthSession.RefreshToken = result.RefreshToken;
                KeycloakAuthSession.AccessTokenExpiresAt = result.ExpiresAt;

                ClaimsPrincipal user = result.User;
                var userInfoLines = new List<string>();
                try
                {
                    var userInfoId = KeycloakUserInfo.FetchClaimsIdentityAsync(
                            keycloakOptions,
                            result.AccessToken,
                            cts.Token).Result;
                    user = KeycloakUserInfo.Merge(result.User, userInfoId);
                    userInfoLines.Add("Профиль UserInfo загружен.");

                    if (onsuccess is not null)
                    {
                        onsuccess(user);
                    }
                }
                catch (Exception ex)
                {
                    userInfoLines.Add("UserInfo не загружен (показаны только данные из id_token):");
                    userInfoLines.Add(ex.Message);
                }

                KeycloakAuthSession.User = user;
                KeycloakAuthSession.DisplayName = KeycloakUserDisplayName.FromPrincipal(user);
            }, keycloakOptions, cts.Token);//.ConfigureAwait(true);


        }


        private void OnSubmitButtonClicked()
        {
            try
            {
                this.Info(typeof(SigninUser<TDbContext>).GetTypeName());
                var signin = AppProviderService.GetInstance().Get<SigninUser<TDbContext>>();
                this.Info(signin);
                var result = signin.Signin( login, password );
                User = result.Result;
                if (User is not null && result.Succeeded)
                {
                    onsuccess(User);
                }
                else
                {
                    throw new Exception(result.Exception);
                }
            }
            catch (Exception ex)
            {
                errorMessage.Text = ex.Message;
            }
            finally
            {
                
            }
        }

        private void OnPasswordChanged(string obj)
        {
            account.Password = password = obj;
            var vr = account.Validate();

            var sb = new StringBuilder();
            if (vr.ContainsKey(nameof(account.Email)))
            {
                foreach (var error in vr[nameof(account.Email)])
                {
                    sb.AppendLine(error + "\n");
                }
            }
            loginError.Text = sb.ToString();

            sb = new StringBuilder();
            if (vr.ContainsKey(nameof(account.Password)))
            {
                loginError.Text = sb.ToString();
                sb = new StringBuilder();
                foreach (var error in vr[nameof(account.Password)])
                {
                    sb.AppendLine(error + "\n");
                }
            }
            passwordError.Text = sb.ToString();
        }

        private void OnLoginChanged(string obj)
        {
            account.Email = login = obj;
            
            var vr = account.Validate();

            var sb = new StringBuilder();
            if(vr.ContainsKey(nameof(account.Email)))
            {
                foreach (var error in vr[nameof(account.Email)])
                {
                    sb.AppendLine(error + "\n");
                }
            }
            loginError.Text = sb.ToString();

            sb = new StringBuilder();
            if (vr.ContainsKey(nameof(account.Password)))
            {
                loginError.Text = sb.ToString();
                sb = new StringBuilder();
                foreach (var error in vr[nameof(account.Password)])
                {
                    sb.AppendLine(error + "\n");
                }
            }
            passwordError.Text = sb.ToString();
        }
    }



}

[Label("Регистрация пользователя")]
public class UserSignupView<TDbContext> : AppViewBase
    where TDbContext: DeliveryDbContext
{
    private UserPerson person;
    private UserAccount account;
    private readonly Action<bool, object> oncompleted;
    private Action<UserContext> _afterSignupWithUser;
    private ClaimsPrincipal _keycloakClaims;

    public UserSignupView()
    {
        var editView = EditView.CreateEditViewBase(new UserPerson(), (s, e) =>
        {
            OnPersonInputCompleted(s, e);
        });
        editView.OnEditCompleted += (s, e) =>
        {
            OnPersonInputCompleted(s, e);
        };

        editView.Parent = this;
        this.SetFullSize();
        this.Padding = 20;
        this.Margin = 20;
        LayoutFill(editView);
    }

    /// <summary>
    /// Регистрация с полями из профиля Keycloak; после успешной регистрации вызывается <paramref name="afterSignupWithUser"/>.
    /// </summary>
    public UserSignupView(Action<bool, object> onCompleted, ClaimsPrincipal keycloakClaims, Action<UserContext> afterSignupWithUser)
    {
        oncompleted = onCompleted;
        _afterSignupWithUser = afterSignupWithUser;
        _keycloakClaims = keycloakClaims;
        var initialPerson = CreateUserPersonFromKeycloak(keycloakClaims);
        var editView = EditView.CreateEditViewBase(initialPerson, OnPersonInputCompleted);
        editView.OnEditCompleted += (s, e) => OnPersonInputCompleted(s, e);
        LayoutFill(editView.CreateSubmitView());
    }

    private static UserPerson CreateUserPersonFromKeycloak(ClaimsPrincipal p)
    {
        string C(string type) => p?.FindFirst(type)?.Value?.Trim() ?? "";

        var given = C("given_name");
        var ln = given.Substring(given.LastIndexOf(" ") + 1);
        given = given.Substring(given.IndexOf(" ") > 0 ? given.IndexOf(" ") : given.Length).Trim();
        given = given.Substring(0, given.IndexOf(" ") > 0 ? given.IndexOf(" ") : given.Length);

        if (string.IsNullOrEmpty(given))
            given = C(ClaimTypes.GivenName);
        var family = C("family_name");
        if (string.IsNullOrEmpty(family))
            family = C(ClaimTypes.Surname);
        var middle = C("middle_name");
        var full = C("name");
        if (string.IsNullOrEmpty(family) && string.IsNullOrEmpty(given) && !string.IsNullOrEmpty(full))
        {
            var parts = full.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length >= 2)
            {
                family = parts[0];
                given = parts[1];
                if (parts.Length > 2)
                    middle = string.Join(' ', parts.Skip(2));
            }
            else if (parts.Length == 1)
                given = parts[0];
        }

        var rnd = DbContextUserInitializer.GetRandomPersonData();
        family = string.IsNullOrEmpty(family) ? rnd.Item2 : family;
        given = string.IsNullOrEmpty(given) ? rnd.Item1 : given;
        middle = string.IsNullOrEmpty(middle) ? rnd.Item3 : middle;

        DateTime? birthday = null;
        var birthStr = C("birthdate");
        if (!string.IsNullOrEmpty(birthStr)
            && DateTime.TryParse(birthStr, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var bd))
            birthday = bd;
        else
            birthday = rnd.Item4;

        var telRaw = C("phone_number");
        var digits = string.IsNullOrEmpty(telRaw) ? "" : new string(telRaw.Where(char.IsDigit).ToArray());
        string tel;
        if (digits.Length == 10 && digits[0] == '9')
            tel = "7" + digits;
        else if (digits.Length == 11 && digits[0] == '8')
            tel = "7" + digits.Substring(1);
        else if (digits.Length == 11 && digits[0] == '7')
            tel = digits;
        else
            tel = string.IsNullOrEmpty(digits) ? rnd.Item5 : digits.PadLeft(11, '7');

        if (tel.Length < 10)
            tel = rnd.Item5;

        var rp = new UserPerson(family, given, middle, birthday, tel);
        rp.LastName = ln;
        return rp;
    }

    private static UserAccount CreateUserAccountFromKeycloak(ClaimsPrincipal p)
    {
        var acc = new UserAccount();
        var email =
            p?.FindFirst(ClaimTypes.Email)?.Value
            ?? p?.FindFirst("email")?.Value
            ?? p?.FindFirst("upn")?.Value;
        if (!string.IsNullOrWhiteSpace(email))
            acc.Email = email.Trim();
        acc.Password = "";
        return acc;
    }

    private void OnPersonInputCompleted(bool success, object person)
    {
        if (success)
        {
            if (person is UserPerson p)
            {
                this.person = p;
                var accountModel = _keycloakClaims is not null
                    ? CreateUserAccountFromKeycloak(_keycloakClaims)
                    : new UserAccount();
                var accountEditView = EditView.CreateEditViewBase(accountModel, OnAccountInputCompleted);
                LayoutFill(accountEditView.CreateSubmitView());
                var root = accountEditView.GetAppRootView();


                if (root is null)
                    throw new Exception();
            }
            else
            {
                if (person is SubmitView s)
                {
                    var editViews = s.GetParentViews<EditView>();
                    if (editViews.Count() == 0)
                        return;
                    UserPerson sp = (UserPerson)((EditView)editViews.First()).Target;
                    this.person = sp;
                    var accountModel = _keycloakClaims is not null
                        ? CreateUserAccountFromKeycloak(_keycloakClaims)
                        : new UserAccount();
                    LayoutFill(EditView.CreateEditViewBase(accountModel, OnAccountInputCompleted));
                }
            }


        }
    }

    private void OnAccountInputCompleted(bool success, object account)
    {
        if (!success)
            return;

        this.account = (UserAccount)account;

        MethodResult<UserContext> signupResult;
        try
        {
            person.Validate(nameof(person.Data));
            signupResult = AppProviderService.GetInstance().Get<SignupUser<TDbContext>>().Signup(this.account, person);
        }
        catch (Exception ex)
        {
            signupResult = MethodResult<UserContext>.OnError(ex);
        }

        Controls.Clear();

        if (signupResult.Succeeded)
        {
            if (_afterSignupWithUser is not null)
            {
                _afterSignupWithUser(signupResult.Result);
                oncompleted?.Invoke(true, signupResult.Result);
                RemoveFromParent();
                return;
            }

            var email = signupResult.Result?.Account?.Email ?? this.account?.Email ?? "";
            LayoutTop(CreateText($"Регистрация выполнена успешно. Email: {email}"));

            LayoutTop(CreateButton($"Войти в приложение", () =>
            {
                RemoveFromParent();
            }));
        }
        else
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(signupResult.Exception))
                parts.Add(signupResult.Exception);
            if (signupResult.Validations is { Count: > 0 } validations)
                parts.Add(validations.ToJsonOnScreen());
            var message = parts.Count > 0 ? string.Join(Environment.NewLine, parts) : "Не удалось зарегистрировать пользователя.";
            var text = CreateText(message);
            text.MinHeight = 300;
            LayoutTop(text);
            LayoutTop(CreateButton("Повторить ввод учётной записи",
                () => LayoutFill(EditView.CreateEditViewBase(
                    _keycloakClaims is not null ? CreateUserAccountFromKeycloak(_keycloakClaims) : new UserAccount(),
                    OnAccountInputCompleted))));
        }

    }

    public UserSignupView(Action<bool, object> oncompleted) : this()
    {
        this.oncompleted = oncompleted;
        LayoutFill(EditView.CreateEditViewBase(new UserPerson(), OnPersonInputCompleted));
        
    }

}