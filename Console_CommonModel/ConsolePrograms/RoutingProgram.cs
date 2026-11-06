using System.Collections.Concurrent;
using System;
using System.Linq;

namespace Console_InputApplication.ConsolePrograms
{


    /// <summary>
    /// 
    /// </summary>
    public class RoutingProgram : IDisposable
    {

        private static List<string> State = new List<string>();
        private string StateName { get; set; }


        public RoutingProgram() { }
        public RoutingProgram(string stateName)
        {
            BeginState(StateName = stateName);
        }


        /// <summary>
        /// route="{route}/{name}"
        /// </summary>        
        private static void BeginState(string name)
        {
            State.Add(name);
            string Title = "/";
            foreach (var next in State)
                Title += next + "/";
            Console.Title = Title;
        }


        /// <summary>
        /// route="{route}/{name}"
        /// </summary>     
        private static void EndState(string name)
        {
            
            while (State.Last() != name)
                State.RemoveAt(State.Count()-1 );
            State.RemoveAt(State.Count() - 1);
            string Title = "/";
            foreach (var next in State)
                Title += next + "/";
            Console.Title = Title;
        }

        public void Dispose() => EndState(StateName);

        
    }
}
