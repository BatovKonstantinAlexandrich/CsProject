using System.Collections.Concurrent;
using System.Diagnostics;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.Connectors;


[Label("Управление ODBC источниками")]
public class OdbcDriverManager: MyValidatableObject
{
    private static ConcurrentBag<OdbcDatasourceInfo> DataSources;
    private static ConcurrentBag<OdbcDriverInfo> Drivers;
    

    /*[Label("Регистрация ODBC источника данных")]
    public string RegOdbcDataSource(string name, SqlServerConnectionString connectionstring, string driver)
    {
        if (!this.GetOdbcDriverNames().Contains(driver))
        {
            throw new ArgumentException(nameof(driver), $"Не найден драйвер с именем: {driver}");
        }
        
        var command = $"Add-OdbcDsn -DriverName \"{driver}\" -Name \"{name}\" -DsnType \"User\"  -SetPropertyValue @(\"Server={connectionstring.Server}\",\"Database={connectionstring.Database}\",\"Trusted_Connection={(connectionstring.TrustedConnection ? "True" : "False")}\")";
        return Execute(command);
    }*/

    [Label("Подключиться к источнику ODBC")]
    public OdbcDatabaseManager ConnectDatasource(string name, string user=null, string password=null) {
        var dss = GetOdbcDatasources();
        var ds = dss.FirstOrDefault(d => d.Name.ToLower() == name.ToLower());
        if (ds is null)
        {
            throw new ArgumentException(nameof(name), $"Источник ODBC с именем {name} не зарегистрирован");
        }
        if (user is null && password is null)
        {
            var datasource = new OdbcDataSource(
                $"DSN=" + name + ";UID=;PWD=;");
                //$"Driver={{{ds.DriverName}}};dsn=" + name + ";UId=" + user + ";PWD=" + password + ";");
            var manager = new OdbcDatabaseManager(datasource);
            return manager;
        }
        else
        {
            var datasource = new OdbcDataSource(
                $"DSN=" + name + ";UID=" + user + ";PWD=" + password);
                //$"Driver={{{ds.DriverName}}};dsn=" + name + ";UId=" + user + ";PWD=" + password + ";");
            var manager = new OdbcDatabaseManager(datasource);
            return manager;
        }
    }


    [Label("Имена драйверов")]
    public IEnumerable<string> GetOdbcDriverNames()
    {
        if(Drivers is null)
        {
            Drivers = new ConcurrentBag<OdbcDriverInfo>(GetOdbcDrivers());
        }
        return Drivers.Select(d => d.Name);
    }

        


    [Label("Получение списка источников ODBC")]
    public IEnumerable<string> GetOdbcDatasourceNames()
    {        
        if (DataSources is null)
        {
            DataSources = new ConcurrentBag<OdbcDatasourceInfo>(this.GetOdbcDatasources());
        }
        return DataSources.Select(x => x.Name);        
    }


    [Label("Получение сведения по источникам ODBC")]
    public IEnumerable<OdbcDatasourceInfo> GetOdbcDatasources()
    {
        if (DataSources is null)
        {              
            bool next = true;
            OdbcDatasourceInfo ds = null;
            var dss = new List<OdbcDatasourceInfo>();
            this.Execute("Get-OdbcDsn").ReplaceAll("\r", "").Split("\n").ToList().ForEach(line => {
                if (string.IsNullOrEmpty(line))
                {
                    if (ds != null)
                        dss.Add(ds);
                    next = true;

                }
                else
                {
                    //this.Info(line.Length+ " "+line);                
                    if (next || ds == null)
                        ds = new OdbcDatasourceInfo();
                    next = false;
                    string[] arr = line.Split(":");
                    string key = arr[0].Trim();
                    string value = arr[1].Trim();

                    //Info(key+"="+value);
                    ds.GetType().GetProperty(key).SetValue(ds, value);
                }
            });
            return DataSources = new ConcurrentBag<OdbcDatasourceInfo>(dss);
        }
        return DataSources;
    }


    [Label("Получение ODBC драйверов")]
    public IEnumerable<OdbcDriverInfo> GetOdbcDrivers()
    {
        if(Drivers is null)
        {
            bool next = true;
            OdbcDriverInfo driver = null;
            var drivers = new List<OdbcDriverInfo>();
            this.Execute("Get-OdbcDriver").ReplaceAll("\r", "").Split("\n").ToList().ForEach(line => {
                if (string.IsNullOrEmpty(line))
                {
                    if (driver != null)
                        drivers.Add(driver);
                    next = true;

                }
                else
                {
                    //this.Info(line.Length+ " "+line);                
                    if (next || driver == null)
                        driver = new OdbcDriverInfo();
                    next = false;
                    string[] arr = line.Split(":");
                    string key = arr[0].Trim();
                    string value = arr[1].Trim();

                    //Info(key+"="+value);
                    driver.GetType().GetProperty(key).SetValue(driver, value);
                }
            });
            return Drivers = new ConcurrentBag<OdbcDriverInfo>(drivers.ToArray());
        }
        return Drivers;
        
    }
    

    /// <summary>
    /// Выполнение команды PS
    /// </summary>
    private string Execute(string command)
    {
        //this.Info("Выполнение команды: "+command);

        ProcessStartInfo info = new ProcessStartInfo("PowerShell.exe", command);
        info.RedirectStandardError = true;
        info.RedirectStandardOutput = true;
        info.UseShellExecute = false;
                    
        System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
        System.IO.StreamReader reader = process.StandardOutput;

        string result = reader.ReadToEnd();

        //this.Info("Результат выполнения: \n"+result);
        return result;
    }
}