[Label($"Тестирование класса ChangingDictionary")]
public class ChangingDictionaryTest : TestingElement
{
    public ChangingDictionaryTest()
    {
    }

    public ChangingDictionaryTest(IServiceProvider sp) : base(sp)
    {
    }

    public override void OnTest()
    {
        Assert((p) =>
        {
            var sync = new Dictionary<string, string>();
            var map = new ChangingDictionary<string, string>();
            map.OnChanged += (s, e) =>
            {
                switch (e.Target.Action)
                {
                    case "added":
                        sync[e.Target.Key] = e.Target.Value;
                        break;
                    case "removed":
                        sync.Remove(e.Target.Key);
                        break;
                    case "updated":
                        sync[e.Target.Key] = e.Target.Value;
                        break;
                    default: throw new ArgumentException(nameof(Action));
                }
            };
            map["1"] = "this is a test";
            if (!sync.ContainsKey("1"))
                return false;
            else Messages.Add("Поддерживаются события добавления в справочник");
            map["1"] = "это тест";
            if (!sync.ContainsKey("1") || sync["1"] != "это тест")
            {
                Messages.Add("События изменения в справочнике не работают");
                return false;
            }
            else
            {
                Messages.Add("Поддерживаются изменения в справочнике поддерживаются");
            }
            map.Remove("1");
            if (sync.Count() != 0)
            {
                Messages.Add("События удаления из справочника не работают");
                return false;
            }
            else
            {
                Messages.Add("Поддерживаются события удаления из справочника");
            }
            return true;
        },
        "Реализован справочник с поддержкой событий",
        "Справочник ChangingDictionary содержит ошибки в реализации событий");
    }
}
