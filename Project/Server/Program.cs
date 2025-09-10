using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(SessionService));
            Console.WriteLine("Welcome to Server!");
            host.Open();
            Console.ReadKey();
            host.Close();
        }
    }
}
