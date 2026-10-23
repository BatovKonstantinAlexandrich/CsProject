using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibPages.AuthViews
{
    [Label("Личный кабинет пользователя")]

    public class UserHomeView : AppViewBase

    {

        [Label("User Context")]
        public UserContext UserContext { get; set; }

        [Label("menu View")]
        public MenuView menuView { get; private set; }

        public UserHomeView()
        {

        }

        public void GoToBusinessWork()
        {
            LayoutFill(this.Get<UserHomeService<DeliveryDbContext>>().GetBusinessNavView());
        }

        public void SetUserContext(UserContext UserContext, Action<UserSettings> applyUserSettings)
        {
            this.UserContext = UserContext;
            EditView p = null;
            LayoutFill(p = EditView.CreateEditViewBase(UserContext.Person, (success, settings) =>
            {
                if (success)
                {
                    using (var db = new DbContextUser())
                    {
                        db.Update(UserContext.Person);
                        db.SaveChanges();
                    }
                    ShowSaveSuccessSnackbar();
                }
            }));
            p.Editable = false;

            menuView = new MenuView();
            //menuView.AddMenuActions("Бизнес-функции", new(this.Get<UserBusinessService>().Get));
            menuView.AddMenuActions("Группы", new(GetActionMap(AppProviderService.GetInstance().Get<UserGroupsService<DeliveryDbContext>>(), OnMethodResult)));
            menuView.AddMenuActions("Сообщения", new(GetActionMap(AppProviderService.GetInstance().Get<UserMessagesService<DeliveryDbContext>>(), OnMethodResult)));
            menuView.AddMenuActions("Действия", new(GetActionMap(AppProviderService.GetInstance().Get<UserBusinessService<DeliveryDbContext>>(), OnMethodResult)));
            //menu.AddMenuActions("Статистика", new(this.GetActionMapObject(AppProviderService.GetInstance().Get<UserStatsService>())));
            Top = menuView;
            menuView.MinHeight = 20;
        }

        private void OnMethodResult(string title, object result)
        {
            if (result.GetType().IsExtends(typeof(MethodResult<>)))
            {
                result = result.GetValue("Result");
            }
            if (result.GetType().IsCollectionType())
            {
                List<object> items = new();
                foreach (var p in (dynamic)result)
                {
                    items.Add(p);
                }


                var pane = new AppViewBase();
                pane.LayoutFill(pane.CreateSelectList(
                    "Выберите из списка",
                    items.Select(i => (DescriptableObject)i).ToList(),
                    selectedGroup =>
                    {

                        var actions = AppProviderService.GetInstance().Get<UserGroupsService<DeliveryDbContext>>().IsUserInGroup(selectedGroup.Id) ?
                        new List<string>()
                        {
                            "покинуть группу",
                            "сообщения",
                            "состав"

                        }
                        : new List<string>()
                        {
                            "вступить",
                            "сообщения",
                            "состав"

                        };
                        CreateSelectList(selectedGroup.Label,
                            actions, selectedAction =>
                            {
                                switch (selectedAction)
                                {
                                    case "сообщения":
                                        var messages = AppProviderService.GetInstance().Get<UserGroupsService<DeliveryDbContext>>().GetGroupMessages(selectedGroup.Id, 1, 10);
                                        LayoutCollectionAsList("Сообщения", messages, message =>
                                        {

                                        });
                                        break;
                                    case "состав":
                                        List<UserPerson> persons = AppProviderService.GetInstance().Get<UserGroupsService<DeliveryDbContext>>().GetPersons(selectedGroup.Id);
                                        CreateSelectList("", persons, person =>
                                        {
                                        });
                                        break;
                                    case "вступить":
                                        AppProviderService.GetInstance().Get<UserGroupsService<DeliveryDbContext>>().JoinToGroup(selectedGroup.Id);
                                        break;
                                    case "покинуть группу":
                                        AppProviderService.GetInstance().Get<UserGroupsService<DeliveryDbContext>>().LeaveGroup(selectedGroup.Id);
                                        break;
                                }
                            });

                    }));
                pane.MinHeight = pane.MinWidth = 320;
                var f = new AppViewBase();
                f.LayoutFill(pane);
                var labelTitle = f.CreateTitleCentral(title);
                labelTitle.ContextMenu.Items.Add(new()
                {
                    Label = "по центру",
                    OnClick = (s, e) =>
                    {
                        labelTitle.TextAlign = "center";
                    }
                });
                f.Top = labelTitle;
                LayoutFill(f);
            }
            else
            {
                LayoutFill(EditView.CreateEditViewBase(result));
            }
        }

        private void ShowSaveSuccessSnackbar()
        {
            var form = ToControl()?.RunAtForm();
            form?.ShowSnakBar("Сохранение успешно завершено");
        }
    }
}
