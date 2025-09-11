using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project
{
    public class Program
    {
        public static string basePath = ConfigurationManager.AppSettings["basePath"];
        public static string filePath;
        public static ISessionService service;

        static void Main(string[] args)
        {
            ChannelFactory<ISessionService> factory =new ChannelFactory<ISessionService>("SessionService");
            Console.WriteLine("Welcome to Client!");

            int selected_vehicle = 0;
            do
            {
                selected_vehicle = ShowAllEV();
                if(selected_vehicle == 12)
                {
                    Console.WriteLine("Closing program");
                    break;
                } 

                service = factory.CreateChannel();
                

                OperationResult or = service.StartSession(selected_vehicle);
                Console.WriteLine(or.ResultMessage);

                SendSamples(selected_vehicle);

                service.EndSession();
                Console.WriteLine("Session ended successfully!");
                Thread.Sleep(1000);
            }
            while (selected_vehicle != 12);

            Console.ReadKey();
        }

        private static int ShowAllEV()
        {

            string[] fileNames = Directory.GetDirectories(basePath);

            

            List<string> folders = new List<string>();

            foreach (string f in fileNames)
            {
                string folderName = f.Substring(basePath.Length);
                folders.Add(folderName);
            }

            folders.Add("EXIT");
            int index = 0;
            ConsoleKey key;
            Thread.Sleep(1500);
            do
            {
                Console.Clear();
                Console.WriteLine("\nSelect vehicle (use \u2191 \u2193, Enter to confirm):\n");

                for (int i = 0; i < folders.Count; i++)
                {
                    if (i == index)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" --> ");
                        Console.ResetColor();
                        Console.WriteLine(folders[i]);
                    }
                    else
                    {
                        Console.WriteLine("     " + folders[i]);
                    }
                }

                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow)
                {
                    index = (index == 0) ? folders.Count - 1 : index - 1;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    index = (index == folders.Count - 1) ? 0 : index + 1;
                }
            } while (key != ConsoleKey.Enter);

            Console.Clear();

            filePath = basePath + folders[index] + "/Charging_Profile.csv";

            return index;
        }

        public static void SendSamples(int selected_vehicle)
        {
            int roxIndex = -1;
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        sr.ReadLine();//Headers

                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            Console.WriteLine("Poslao sam liniju:" + line);
                            FileWritterOptions fw = new FileWritterOptions( ++roxIndex, selected_vehicle, line);
                            OperationResult or = service.PushSample(fw);
                            if (or.ResultType == ResultType.Success)
                            {
                                Console.WriteLine("Data transmitted successfully!");
                            }
                            else
                            {
                                Console.WriteLine("Error: " + or.ResultMessage);
                            }
                        }

                    }
                }

            }
            catch(Exception ex)
            {   
                Console.WriteLine(ex.Message);
            }   
        }
    }
}
