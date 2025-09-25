using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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

            Thread.Sleep(1500);

            int selected_vehicle = 0;
            do
            {
                selected_vehicle = ShowAllEV();
                if(selected_vehicle == 12)
                {
                    Console.WriteLine("Program closed!");
                  
                    break;
                } 

                service = factory.CreateChannel();
                

                OperationResult or = service.StartSession(selected_vehicle);
                Console.WriteLine(or.ResultMessage);

                SendSamples(selected_vehicle);

                service.EndSession();
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
            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    string line;
                    int rowIndex = 0;
                    bool first = true;

                    while ((line = sr.ReadLine()) != null)
                    {
                        rowIndex++;
                        var fields = line.Split(',');
                        if (first == true)
                        {
                            service.PushSampleHeaders(line);
                            first = false;
                            rowIndex--;
                            continue;
                        }

                        bool valid = true;
                        for (int i = 1; i < fields.Length; i++)
                        {
                            if (!double.TryParse(fields[i], NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                            {
                                Console.WriteLine($"Invalid number at line {rowIndex}, column {i}: {fields[i]}");
                                valid = false;
                                rowIndex--;
                                Logger.Log(line);
                                break;
                            }
                        }

                        if (!valid) continue;


                        Sample sample = new Sample(selected_vehicle, rowIndex, fields[0], fields[1], fields[2], fields[3], fields[4], fields[5], fields[6],
                            fields[7], fields[8], fields[9], fields[10], fields[11], fields[12], fields[13], fields[14], fields[15], fields[16], fields[17], fields[18]);

                        var result = service.PushSample(sample);

                        if (result.ResultType == ResultType.Failed)
                        {
                            Console.WriteLine($"Server rejected line {rowIndex}: {result.ResultMessage}");
                        }
                    }
                }
                Console.WriteLine("Press ENTER to continue");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending CSV lines: " + ex.Message);
                Console.ReadKey(); 
            }
        }
    }
}
