namespace Console_CommonModel.CommonLib.LibTesting
{
    public class TestingElementService
    {

        public static void SetReportToFile(TestingElement element)
        {
            var filename = $"D:\\TestingReports\\{element.GetTypeName().Replace("<", "_").Replace(">", "_")}.json";
            if (!Directory.Exists("D:\\TestingReports"))
            {
                Directory.CreateDirectory("D:\\TestingReports");
            }
            var json = element.ToJsonOnScreen();
            //element.Info(json);
            File.WriteAllText(filename, json);
        }
        public static TestingReport GetReportFromFile(TestingElement element)
        {
            var filename = $"D:\\TestingReports\\{element.GetTypeName().Replace("<", "_").Replace(">", "_")}.json";
            if (!Directory.Exists("D:\\TestingReports"))
            {
                Directory.CreateDirectory("D:\\TestingReports");
            }
            if (!File.Exists(filename))
            {
                return null;
            }
            else
            {
                var json = File.ReadAllText(filename);
                return json.FromJson<TestingReport>();
            }
        }
    }
}
