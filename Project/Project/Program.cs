using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<ISessionService> factory =new ChannelFactory<ISessionService>("SessionService");
            Console.WriteLine("Welcome to Client!");
            Console.WriteLine("Type the name of the vehicle you want to charge:");
            string[] fileNames = Directory.GetDirectories("../../../Dataset/Cars");

            string basePath = "../../../Dataset/Cars/";
            foreach (string f in fileNames)
            {
                string folderName = f.Substring(basePath.Length );
                Console.WriteLine(" --> " + folderName);
            }
            Console.ReadKey();
        }
    }
}
