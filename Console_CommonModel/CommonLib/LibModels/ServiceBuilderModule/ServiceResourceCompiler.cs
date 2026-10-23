using NSwag;
using NSwag.CodeGeneration.CSharp;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule
{
    public class ServiceResourceCompiler
    {
        public static string ReplaceAll(string text, string s1, string s2)
        {
            string p = text;
            while (p.IndexOf(s1) != -1)
            {
                p = p.Replace(s1, s2);
            }
            return p;
        }
        public static string CmdExec(string command)
        {
            command = ReplaceAll(ReplaceAll(ReplaceAll(ReplaceAll(command, "\n", ""), "\r", ""), @"\\", @"\"), @"//", @"/");
            Console.WriteLine(command);
            ProcessStartInfo info = new ProcessStartInfo("CMD.exe", "/C " + command);

            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);

            string response = process.StandardOutput.ReadToEnd();
            string result = ReplaceAll(response, "\r", "\n");
            result = ReplaceAll(result, "\n\n", "\n");
            while (result.EndsWith("\n"))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }
        private static byte[] CompileCs(string code, string output)
        {
            var path = "D:\\project1";
            string filename = Path.Combine(path, output);
            var name = filename.Substring(0, filename.LastIndexOf(".") - 1);
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            //clear dir
            System.IO.Directory.GetDirectories(path).ToList().ForEach(p => System.IO.Directory.Delete(p, true));
            System.IO.Directory.GetFiles(path).ToList().ForEach(System.IO.File.Delete);

            //новый проект
            CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet new webapi -f net6.0 --force");
            CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet add package Newtonsoft");
            CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet add package Newtonsoft.Json");

            var fileout = Path.Combine(path, filename);
            System.IO.File.WriteAllText(fileout, code);

            Console.WriteLine(CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet build"));
            var dll_path = System.IO.Directory.GetFiles(path, "Project1.dll", SearchOption.AllDirectories).FirstOrDefault();
            path = Path.Combine(path, "bin");
            path = Path.Combine(path, "Debug");
            path = Path.Combine(path, "net6.0");
            dll_path = System.IO.Directory.GetFiles(path, "Project1.dll", SearchOption.AllDirectories).FirstOrDefault();
            var build = Assembly.LoadFile(dll_path);
            foreach (var type in build.GetTypes())
            {
                Console.WriteLine(type.GetTypeName());
            }

            Console.WriteLine($"Записано а {filename} {code.Length} байт");
            return System.IO.File.ReadAllBytes(dll_path);
        }
        private void OnOpenApiDocumentReaded(OpenApiDocument document, string url)
        {
            string output = "Client.cs";
            var shg = new NSwag.CodeGeneration.CSharp.CSharpClientGenerator(document, new CSharpClientGeneratorSettings() { });
            string code = shg.GenerateFile();
            var build = CompileCs(code, output);

            using (var db = new ServiceDbContext())
            {
                if (!db.ServiceModels.Any(x => x.Name == url))
                {
                    db.Add(new ServiceModel()
                    {
                        Name = url,
                        Schema = url,
                        Type = "OpenAPI",
                        Dll = build
                    });
                    db.SaveChanges();
                }
            }
        }


        public OpenApiDocument CreateOpenApiForUrl(string url)
        {
            OpenApiDocument document;
            var readUrl = NSwag.OpenApiDocument.FromUrlAsync(url);
            document = readUrl.Result;

            OnOpenApiDocumentReaded(document, url);
            return document;
        }


        public void AddOpenApi(string url)
        {

            OpenApiDocument document = null;

            if (url.IsFile())
            {
                var readFile = NSwag.OpenApiDocument.FromFileAsync(url);
                document = readFile.Result;
            }
            else if (url != null && url.ToLower().StartsWith("http"))
            {
                document = CreateOpenApiForUrl(url);
            }
            else
            {
                document = CreateOpenApiForUrl(url);
            }

            string output = "Client.cs";
            var shg = new NSwag.CodeGeneration.CSharp.CSharpClientGenerator(document, new CSharpClientGeneratorSettings() { });
            string code = shg.GenerateFile();
            var build = CompileCs(code, output);

            using (var db = new ServiceDbContext())
            {
                if (!db.ServiceModels.Any(x => x.Name == url))
                {
                    db.Add(new ServiceModel()
                    {
                        Name = url,
                        Schema = url,
                        Type = "OpenAPI",
                        Dll = build
                    });
                    db.SaveChanges();
                }
            }

            var assembly = Assembly.Load(build);
            var clients = assembly.GetTypes().Where(p => p.Name.IndexOf("Client") != -1);
            foreach (var client in clients)
            {
                foreach (var method in client.GetMethods())
                {
                    Console.WriteLine($"/{client.Name}/{method.Name}");
                }
            }
        }
        public string CreateOpenApiSourceFile(string url)
        {
            try
            {
                OpenApiDocument document;
                var readUrl = NSwag.OpenApiDocument.FromUrlAsync(url);
                document = readUrl.Result;

                string output = "Client.cs";
                var shg = new NSwag.CodeGeneration.CSharp.CSharpClientGenerator(document, new CSharpClientGeneratorSettings() { });
                string code = shg.GenerateFile();

                return code;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(CreateOpenApiSourceFile)}({url}) => {ex}");
            }
        }
        
    }
}
