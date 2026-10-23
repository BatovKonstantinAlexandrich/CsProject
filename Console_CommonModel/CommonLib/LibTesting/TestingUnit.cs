public class TestingUnit : TestingElement
{
    public TestingUnit(): this(AppProviderService.GetInstance())
    {
        Failed = false;
    }

    public TestingUnit(IServiceProvider parent) : base(parent)
    {
        Failed = false;
    }

    public override TestingReport DoTest(bool interactive = false, bool force = false)
    {
        this.Debug(Console.Title = this.Name);
        //if (interactive)
        //    InputConsole.ConfirmContinue();
        return base.DoTest(interactive, force);
    }

    public override void OnTest()
    {
        //this.Debug("OnTest()");
    }

    public List<string> GetTestPlan( )
    {
        var result = new List<string>();
        result.Add(GetType().GetTypeName());
        result.AddRange(GetElementNames(1));
        return result;
    }

    public List<string> GetTestPlanDescription()
    {
        var result = new List<string>();
        result.Add($"{this.GetLabel()}");
        result.Add($"{this.GetDescription()}");        
        result.AddRange(GetElementDescription("",1));
        return result;
    }

    public IEnumerable<string> GetElementDescription(string prefix, int level)
    {        
        var result = new List<string>();
        int line = 1;
        foreach (var p in Children)
        {
            string number = prefix==""? line.ToString(): prefix + "." + line;
            string s = "";
            for (int i = 0; i < level; i++)
            {
                s += "  ";
            }
            string text = s + $"{number} " + p.GetTypeName() + "\n";
            text += s + "                                                    ".Substring(0, $"{level}.{line}.".Length+1) + p.GetLabel()+"\n";
            text += s + "                                                    ".Substring(0, $"{level}.{line}.".Length+1) + p.GetDescription() + "\n";

            result.Add(text);

            result.AddRange(p.GetElementDescription(number, level + 1));
            line++;
        }
        return result;
    }


    /// <summary>
    /// Создания тестов для множества классов наследников 
    /// аргумента обобщенного типа.
    /// </summary>
    /// <param name="genericTestType">обобщённый тестовый тип</param>
    /// <param name="genericArguments">аргументы генерации</param>
    public void AppendTestGeneration(Type genericTestType, IEnumerable<Type> genericArguments)
    {
        foreach (var genericArgument in genericArguments)
        {
            try
            {
                this.Append(genericTestType.MakeGenericType(genericArgument).New<TestingElement>(new object[] { this.provider }));
            }
            catch(Exception ex)
            {
                this.Error($"Создание обобщённых тестов прервано на" +
                    $" {genericTestType.MakeGenericType(genericArgument).GetTypeName()}: " +
                    $" {ex.Message}");
                throw;
            }
        }
    }
}

