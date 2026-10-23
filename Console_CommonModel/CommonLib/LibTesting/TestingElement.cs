using Console_CommonModel.CommonLib.LibTesting;
using Console_CommonModel.CommonLib.Others;

using Microsoft.EntityFrameworkCore;

public abstract class TestingElement: TestingReport
{
    
    protected IServiceProvider provider;

    protected TestingElement(IServiceProvider provider)
    {
        this.provider = provider;
        this.Messages.Clear();
    }

    public TestingElement() : this(AppProviderService.GetInstance())
    {
    }


    public TestingElement(TestingUnit parent)
    {
        Parent = parent;
        this.Messages.Clear();
    }

    public static string Run<TTestingElement>() where TTestingElement: TestingElement => 
        typeof(TTestingElement).New<TestingElement>(new object[] { AppProviderService.GetInstance() }).DoTest().ToDocument();

    private IEnumerable<TestingElement> GetChildren() => Children;
    public void Push(TestingElement pchild) => Append(pchild);

    public global::TypeNode<global::TestingElement> ToTreeNode()
    {
        TypeNode<TestingElement> result = new TypeNode<TestingElement>(
            $"[{this.GetTypeName()}]: {this.GetType().GetLabel()}", this, null);
        foreach (TestingElement PChild in Children)
        {
            var ChildNode = PChild.ToTreeNode();            
            ChildNode.Parent = result;
        }        
        return result;
    }

    public IEnumerable<string> GetElementDescription(string prefix, int level)
    {
        var result = new List<string>();
        int line = 1;
        foreach (var p in Children)
        {
            string number = prefix == "" ? line.ToString() : prefix + "." + line;
            string s = "";
            for (int i = 0; i < level; i++)
            {
                s += "  ";
            }
            string text = s + $"{number} " + p.GetTypeName() + "\n";
            text += s + "                                                    ".Substring(0, $"{level}.{line}.".Length + 1) + p.GetLabel() + "\n";
            text += s + "                                                    ".Substring(0, $"{level}.{line}.".Length + 1) + p.GetDescription() + "\n";

            result.Add(text);

            result.AddRange(p.GetElementDescription(number, level + 1));
            line++;
        }
        return result;
    }

    public virtual IEnumerable<string> GetElementDescription(int level)
    {
        var result = new List<string>();
        int line = 1;
        foreach (var p in Children)
        {
            string s = "";
            for (int i = 0; i < level; i++)
            {
                s += "  ";
            }
            string text = s + $"{level}.{line}." + p.GetTypeName() + "\n";
            text += s + "                                                    ".Substring(0, $"{level}.{line}.".Length + 1) + p.GetLabel() + "\n";
            text += s + "                                                    ".Substring(0, $"{level}.{line}.".Length + 1) + p.GetDescription() + "\n";
            result.Add(text);

            result.AddRange(p.GetElementDescription(level+1));
            line++;
        }
        return result;
    }

    public IEnumerable<string> GetElementNames(int level)
    {
        var result = new List<string>();
        int line = 1;
        foreach (var p in Children)
        {
            string s = "";
            for (int i = 0; i < level; i++)
            {
                s += "  ";
            }
            result.Add(s + $"{level}.{line}." + p.GetTypeName());
            result.Add(s + $"  " + p.GetLabel());
            result.Add(s + $"  " + p.GetDescription());
            result.AddRange(p.GetElementNames(level + 1));
            line++;
        }
        return result;
    }

    protected List<string> Messages = new List<string>();

    public void LogMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message) == false)
            this.Messages.Add(message);
    }
  
    protected TestingElement Parent;

    


    public abstract void OnTest();
    public virtual void Test() { }

    public virtual string GetTitle() => 
        $"[{this.Name}] {this.GetType().GetLabel()}";

    public virtual TestingReport DoTest(bool interactive = false, bool force = false)
    {
        var readed = TestingElementService.GetReportFromFile(this);
        if (this.Parent is not null && interactive && this is TestingUnit)
        {
            var lines = this.Cast<TestingUnit>().GetTestPlanDescription();
            List<string> messages = new();
            while(lines.Count()>0)
            {
                messages.Add(lines[0]); 
                lines.RemoveAt(0);
                if(messages.Count()==10)
                {
                    InputConsole.Clear();
                    foreach(var message in messages)
                    {
                        this.WriteLine(message);
                    }
                    if(interactive)
                        this.ConfirmContinue();
                    messages.Clear();
                }
            }
            if (messages.Count() > 0)
            {
                InputConsole.Clear();
                foreach (var message in messages)
                {
                    this.WriteLine(message);
                }
                if (interactive)
                    this.ConfirmContinue();
                messages.Clear();
            }            
        }
            
        this.Clear();
        this.Name = this.Name is not null? this.Name : this.GetType().GetTypeName();
        Console.Title = this.Name;
        this.Info($"[{Name}] {this.GetLabel()}");
        if (String.IsNullOrWhiteSpace(this.GetDescription())==false)
            this.Info($" {this.GetDescription()}");
     
        if (this.IsExtends(typeof(TestingUnit)))
        {
            var arr = new List<object>();
            arr.Add("=> Состав: ");
            arr.AddRange(this.Children.Select(p => $"   [{p.Name}] {p.GetLabel()}"));
            this.Info(arr.ToArray());            
        }

        //if ((readed is null || readed.Failed) && interactive)
        if (interactive)
            ConfirmContinue();
        
        TestingReport report = this;        
        report.Messages = this.Messages; 
        
        if( String.IsNullOrWhiteSpace(report.Name))
        {
            report.Name = this.Name;            
        }
        try
        {
            this.Started = report.Started = DateTime.Now;            
            
            if(force || readed is null || readed.Failed)
            {
                //if (this is not TestingUnit)
                //{
                //    var testLabel = this.GetLabel();
                //    if (string.IsNullOrWhiteSpace(testLabel) == false)
                //        this.Messages.Add($"Тест: {testLabel}");
                //    var testDescription = this.GetDescription();
                //    if (string.IsNullOrWhiteSpace(testDescription) == false)
                //        this.Messages.Add(testDescription);
                //}

                this.OnTest();
                this.Ended = report.Ended = DateTime.Now;
                TestingElementService.SetReportToFile(this);
            }
            
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) 
        {
            this.Error(ex.InnerException.Message);
        }
        catch (Exception ex)
        {
            this.WriteYellowLine("При выполнении теста "+this.GetTypeName()+" проброшено исключение: ", ex.ToDocument());
            
            this.Failed = report.Failed = true;            
            report.Messages.Add(ex.ToTextDocument());
            if (interactive)
                throw;
            else throw new Exception($"Не выполнен тест: {this.GetTypeName()}:\n{report.ToJsonOnScreen().ReplaceAll("\\n", "\n").ReplaceAll("\\r", "\n").ReplaceAll("\n\n", "\n")} => {ex.Message}");
        }
        finally
        {
            this.Ended = report.Ended = DateTime.Now;
            if (report.Failed)
            {
                this.WriteYellow($"Тест не выполнен {report.ToJsonOnScreen()}\n{report.ToJsonOnScreen()}");
                ConfirmContinue();
                throw new Exception($"Тест не выполнен {report.ToJsonOnScreen()}");
            }


            if (interactive && !(this is TestingUnit))
            {
                this.Clear();
                this.Info($"[{this.Name}]: {this.GetLabel()}");
                this.Debug(report.ToDocument());
                //this.Debug(Messages.ToJsonOnScreen());
                ConfirmContinue();
            }
                        
            //if ( (this.IsExtends(typeof(TestingUnit))) == false)
            //{
            //    /*if (interactive)
            //    {
            //        this.Info($"[{this.GetType().Name}]: {GetType().GetLabel()}");
            //        this.Info($"[{(Failed ? "-" : "+")}]Результаты тестов:\n" + Messages.ToJsonOnScreen());
            //        ConfirmContinue();
            //    }*/
            //    if (interactive) Clear();
            //    this.Info(report.ToDocument());
            //    if (interactive) ConfirmContinue();
            //}

            foreach (var p in GetChildren())
            { 
                if (interactive) 
                    Clear();
                p.Name = p.GetLabel();
                report.SubReports[p.Name] = p.DoTest(interactive, force);
                if (report.SubReports.ContainsKey(p.Name) && report.SubReports[p.Name].Failed)
                {
                    report.Failed = true;
                }
            }
            
        }
        return report;
    }

    private bool ConfirmContinue()
    {
        System.Console.WriteLine("\n\nЧтобы продолжить ENTER, чтобы отменить нажмите ESCAPE?\n\n");
        do
        {
            ConsoleKey k;
            ConsoleKeyInfo key = Console.ReadKey();
            switch (key.Key)
            {
                case ConsoleKey.Escape: return false;
                case ConsoleKey.Enter: return true;
            }
        } while(true);
    }

    public void Clear()
    {
        InputConsole.Clear();
    }

    public void Append(TestingElement pchild)
    {
        this.Children.AddLast(pchild);
        pchild.Parent = this;
    }

    private string GetTypeName(Type propertyType)
    {
        try
        {
            if (propertyType == null)
                throw new ArgumentNullException("type");
            string name = propertyType.Name;
            if (name == null) return "";
            if (name.IndexOf("`") != -1)
                name = name.Substring(0, name.IndexOf("`"));

            var arr = propertyType.GetGenericArguments();
            if (arr.Length > 0)
            {
                name += '<';
                foreach (var arg in arr)
                {
                    name += GetTypeName(arg) + ",";
                }
                name = name.Substring(0, name.Length - 1);
                name += '>';
            }
            return name;
        }
        catch (Exception ex)
        {
            throw new Exception(nameof(GetTypeName)+" => ",ex);
        }
       
    }

    /// <summary>
    /// Метод выполнение проверки функциональности объекта
    /// </summary>
    /// <typeparam name="T">Тип сервиса предоставляющего функциональность</typeparam>
    /// <param name="test">Функция проверки функций</param>
    /// <param name="success">Сообщение при успешной проверки</param>
    /// <param name="failed">Сообщение при провале проверки</param>
    public void AssertService<T>(Predicate<T> test, string success, string failed) where T : class
    {
        try
        {
            this.LogMessage($"Сервис: {typeof(T).GetTypeName()}");
            this.LogMessage("Получение экземпляра из DI…");
            var service = (T)this.provider.GetService(typeof(T));
            if (service is null)
                throw new NullReferenceException("Не удалось получить ссылку на объект типа "+typeof(T).GetTypeName());
            this.LogMessage($"Экземпляр получен, выполнение проверки… {success}");
            
            var result = test(service);
            if (result)
                this.Failed = false;
            else
            {
                this.Failed = true;
            }
        }
        catch (DbUpdateException ex)
        {
            string message = "";
            Exception p = ex;
            while(p != null)
            {
                message+= p.Message;
                p = p.InnerException;
            }
            Failed = true;
            this.Messages.Add($"{failed}: {message}");
        }
        catch (Exception ex)
        {
            Failed = true;
            InputConsole.Clear();
             
            this.Error(ex);
            ConfirmContinue();
            this.Messages.Add($"{failed}: {ex.Message}");
        }
    }



    /// <summary>
    /// Выполнение теста реалиованного в отдельнои методе.
    /// </summary>    
    public void Assert(string methodName)
    {      
        var message = this.GetType().GetLabel();

        try
        {
            var methodInfo = this.GetType().GetMethods().FirstOrDefault(m => m.Name == methodName);
            methodInfo.Invoke(this, new object[] { });
            this.Messages.Add($"Реализовано: {message}");
        }
        catch(Exception ex)
        {
            this.Failed = true;
            this.Messages.Add($"Не реализовано: {message}");
        }
    }



    public void Assert(Predicate<TestingElement> test, string success, string failed)
    {
        try
        {
            //this.LogMessage($"▶ Проверка: {success}");
            var result = test(this);
            if (result)
            {
                this.Failed = false;
                //this.LogMessage($"✓ {success}");
            }
            else
            {
                this.Failed = true;
                //this.LogMessage($"✗ {failed}");
            }
        }
        catch (DbUpdateException ex)
        {
            string message = "";
            Exception p = ex;
            while (p != null)
            {
                message += p.Message;
                p = p.InnerException;
            }
            Failed = true;
            this.Messages.Add($"{failed}: {message}");
            this.Error(this.Messages);
            this.Error(ex.ToDocument());
            InputConsole.ConfirmContinue();
        }
        catch (Exception ex)
        {
            this.Failed = true;
            this.Messages.Add($"{failed}: {ex.Message}\n\n{ex.ToTextDocument()}");
            
            this.Error(this.Messages);
            this.Error($"{failed}: {ex.Message}\n\n{ex.ToTextDocument()}");
            InputConsole.ConfirmContinue();
        }
    }


    public LinkedList<TestingElement> Children = new LinkedList<TestingElement>();

}

