
using Core.Services.CoreAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using NetCoreConstructorAngular;


using Blazor_UserInterface.Shared.Data;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RootForms
{
    public class Program: BackgroundService
    {



        public static void Main(string[] args)
        {
            System.Console.OutputEncoding = Encoding.ASCII;
            foreach (var type in typeof(Microsoft.AspNetCore.SignalR.IHubContext<ResourceHub>).GenericTypeArguments)
            {
                Console.WriteLine(type.Name);
            }


            StartupApplication.OnStart();
            var builder = Host.CreateDefaultBuilder(args);
            builder.ConfigureServices(ConfigureBackground);
            builder.ConfigureServices(ctx => {
                //ctx.AddHostedService<>();
            });
            builder.ConfigureWebHostDefaults(webBuilder => {

                webBuilder.UseStartup<Startup>();
            });
            StartupApplication.ConfigurationBackgroundServices.ForEach(next=> builder.ConfigureServices(next));

            var host = builder.Build();
            using (var scope = host.Services.CreateScope())
            {
                TestContainer(scope);
            };
            host.Run();
        }

        private static void ConfigureBackground(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {

            services.AddHostedService<Program>();
        }

        private static void TestContainer(IServiceScope scope)
        {
            Console.WriteLine("TestContainer()...");
            bool TestContainerIntegration = true;
            IEnumerable<ServiceDescriptor> descriptors =
                scope.ServiceProvider.GetService<IEnumerable<ServiceDescriptor>>();
            if (TestContainerIntegration)
            {
                foreach (var desc in descriptors)
                {
                    if(desc.Lifetime == ServiceLifetime.Singleton)
                    {
                        try
                        {
                            Console.WriteLine($"\t{desc.ServiceType.GetTypeName()} ���������� {desc.Lifetime}: {scope.ServiceProvider.GetServices(desc.ServiceType).Count()}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"\t�������� � ����������� � ���������� {desc.ServiceType.GetTypeName()} => {ex.Message}");
                        }
                    }


                }
            }
            try
            {
                ApplicationDbContext context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                APIRegistration registration = scope.ServiceProvider.GetService<APIRegistration>();
                if (registration != null && registration.HasUserWithEmail("eckumocuk@gmail.com") == false)
                {
                    registration.Signup(
                        "eckumocuk@gmail.com", "Password1!", "Password1!",
                        "Батов", "Константин", "Алексадрович", DateTime.Parse("26.08.1989"), "7-904-334-1127"
                    );
                }
                APIAuthorization authorization = scope.ServiceProvider.GetService<APIAuthorization>();
                var user = authorization.Signin("eckumocuk@gmail.com", "Password1!");
                Console.WriteLine(user.Person.ToJsonOnScreen());
            }
            catch (Exception ex)
            {
                Console.WriteLine("�������� � �������������� ���� ������: " + ex.Message);
                Writing.ToConsole(ex);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }


        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine(DateTime.Now);
                await Task.Delay(100);
            }

        }
    }
}
