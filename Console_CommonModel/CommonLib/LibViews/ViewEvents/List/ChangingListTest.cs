namespace Console_CommonModel.CommonLib.LibViews.ViewEvents.List
{
    [Label("Проверка поддержки событий в ChangingList и ChangingList<T>")]
    public class ChangingListTest : TestingElement
    {

        public ChangingListTest(IServiceProvider sp) : base(sp)
        {
        }

        public override void OnTest()
        {
            TestChangingListGeneric();
            TestChangingListBase();
        }

        private void TestChangingListGeneric()
        {
            AssertService<ChangingList<string>>(
                (list) =>
                {
                    List<string> sunc = new List<string>();
                    list.OnChanged += (s, e) =>
                    {
                        switch (e.Target.Action)
                        {
                            case "added":
                                sunc.Add((string)e.Target.Item);
                                break;
                            case "removed":
                                sunc.Remove((string)e.Target.Item);
                                break;
                            default: throw new ArgumentException(nameof(Action));
                        }
                    };
                    list.Add("this is a test");
                    if (!sunc.Contains("this is a test"))
                        return false;
                    else Messages.Add("Поддерживаются события добавления элементов");
                    list.Remove("this is a test");
                    if (sunc.Contains("this is a test"))
                        return false;
                    else Messages.Add("Поддерживаются события удаления элементов");
                    return true;
                },
                "ChingingList<T> реализует поддержку событий",
                "ChingingList<T> реализует поддержку событий не корректно"
            );
        }

        private void TestChangingListBase()
        {
            AssertService<ChangingList>(
                (list) =>
                {
                    List<object> sunc = new List<object>();
                    list.OnChanged += (s, e) =>
                    {
                        switch (e.Target.Action)
                        {
                            case "added":
                                sunc.Add(e.Target.Item);
                                break;
                            case "removed":
                                sunc.Remove(e.Target.Item);
                                break;
                            default: throw new ArgumentException(nameof(Action));
                        }
                    };
                    list.Add("this is a test");
                    if (!sunc.Contains("this is a test"))
                        return false;
                    else Messages.Add("Поддерживаются события добавления элементов");
                    list.Remove("this is a test");
                    if (sunc.Contains("this is a test"))
                        return false;
                    else Messages.Add("Поддерживаются события удаления элементов");
                    return true;
                },
                "ChingingList реализует поддержку событий",
                "ChingingList реализует поддержку событий не корректно"
            );
        }
    }
}
