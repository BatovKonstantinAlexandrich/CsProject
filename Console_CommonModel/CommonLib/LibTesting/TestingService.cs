public abstract class TestingService<T>: TestingElement where T : class
{
    public TestingService(): base()
    {
    }

    public TestingService(IServiceProvider sp) : base(sp)
    {
    }

    public T Get() => (T)this.provider.GetService(typeof(T));

    public override void OnTest()
    {
        LogMessage($"Класс теста: {GetType().GetTypeName()}");
        AssertService<T>(
            service =>
            {
                LogMessage("Вызов OnServiceTest…");
                var ok = OnServiceTest(service);
                LogMessage(ok ? "OnServiceTest завершён успешно" : "OnServiceTest вернул false");
                return ok;
            },
            GetSuccessTestingMessage(),
            GetFailedTestingMessage());
    }

    public virtual string GetFailedTestingMessage()
    {
        return $"Сервис типа {typeof(T).GetTypeName()} завершил тестирование с отрицательным результатом.";
    }

    public virtual string GetSuccessTestingMessage()
    {
        return $"Сервис типа {typeof(T).GetTypeName()} завершил тестирование с положительным результатом.";
    }

    public abstract bool OnServiceTest(T service);
}

