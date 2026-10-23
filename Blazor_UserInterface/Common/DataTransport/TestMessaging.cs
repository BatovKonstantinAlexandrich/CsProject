
using System;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading;
using System.Timers;
using Newtonsoft.Json.Linq;
 

public class TestMessaging
{    
    public static void Test(string[] args)
    {
        //StaticTest.Main(args);
        var client = new MessageClient("https://localhost:5001/ResourceHub");
        client.Connect().Wait();
        
        SetInterval(() => {
            client.Request(
                new
                {
                    Time = DateTime.Now
                },
                (messageWithTime) => {
                    Console.WriteLine("Responded message: \n" + JObject.FromObject(messageWithTime).ToString());
                }
            ).Wait();
        }, 1000);
        Thread.Sleep(Timeout.Infinite);
    }

    
    public static void SetInterval(Action action, long ms)
    {
        System.Timers.Timer aTimer = new System.Timers.Timer(ms);
        aTimer.Elapsed += (Object source, ElapsedEventArgs e) => {
            action();
        };
        aTimer.AutoReset = true;
        aTimer.Enabled = true;
    }

}

   
 