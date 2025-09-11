using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class SessionService : ISessionService, IDisposable
    {
        private StreamWriter sw;
        bool disposed = false;
        public static string path;

        public void Dispose()
        {
           if(disposed == false)
            {
                if (sw != null)
                {
                    sw.Dispose();
                    sw = null;
                }
                disposed = true;
            }
        }

        public void EndSession()
        {
            Dispose();
        }

        public OperationResult PushSample(FileWritterOptions fw)
        {
            string rejectsPath = "Data/" + fw.VehicleId + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/rejects.csv";
            OperationResult or = new OperationResult();
            try
            {
                string line = fw.line;
                Console.WriteLine("Procitao sam liniju" + line);

                if (path == null || path == "")
                {
                    Console.WriteLine("You must Start session first!");
                    or.ResultType = ResultType.Failed;
                }

                using (var fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {

                    if(line != null)
                    {

                        var fields = line.Split(',');
                        OperationResult opr = new OperationResult();
                        opr = Sample.validateSample(fields[0],
                            fields[1],
                            fields[2],
                            fields[3],
                            fields[4],
                            fields[5],
                            fields[6],
                            fields[7],
                            fields[8],
                            fields[9],
                            fields[10],
                            fields[11],
                            fields[12],
                            fields[13],
                            fields[14],
                            fields[15],
                            fields[16],
                            fields[17],
                            fields[18]);
                        if(opr.ResultType == ResultType.Failed)
                        {
                            if(!File.Exists("Data/" + fw.VehicleId + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/rejects.csv"))
                            {
                                using (File.Create("Data/" + fw.VehicleId + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/rejects.csv")) { }
                            }
                            File.AppendAllText("Data/" + fw.VehicleId + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/rejects.csv",
                            "\n Invalind Sample: " + line+ "\n Reason: \n"+ opr.ResultMessage);
                        }
                        else
                        {
                            sw.WriteLine(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                or.ResultType = ResultType.Failed;
                or.ResultMessage = "Error while pushing sample!" + ex.Message;
                return or;
            }

            or.ResultMessage = "Sample added successfully!";
            or.ResultType = ResultType.Success;
            return or;
        }

        public OperationResult StartSession(int vehicleId)
        {
            string directory = "Data/" + vehicleId + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "/";
            path = directory + "session.csv";
            Console.WriteLine(path);
            try
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                sw = new StreamWriter((new FileStream(path, FileMode.Create, FileAccess.Write)));

                OperationResult or = new OperationResult();

                if (File.Exists(path))
                {
                    or.ResultType = ResultType.Success;
                    or.ResultMessage = "Session opened successfully!";
                   
                    return or;
                }

                or.ResultType = ResultType.Failed;
                or.ResultMessage = "Problem while creating file!";
                return or;
            }
            catch (Exception ex)
            {
                OperationResult or = new OperationResult(ResultType.Failed, ex.Message);
                return or;
            }
            
        }
    }
}
