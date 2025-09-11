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
    public class SessionService : ISessionService
    {
        private static StreamWriter sw;
        private static StreamWriter rejected;
        private static int vehicleID;
        bool first = true;
        public static string path;


        public void EndSession()
        {
            if (sw != null) { 
                sw.Dispose();
                sw = null;
            }
            if (rejected != null)
            {
                rejected.Dispose();
                rejected = null;
            }
            Console.WriteLine("Transfer finished!");
        }

        public OperationResult PushSample(Sample sample)
        {
           OperationResult or = new OperationResult();
            try
            {

                if (path == null || path == "")
                {
                    Console.WriteLine("You must Start session first!");
                    or.ResultType = ResultType.Failed;
                }

                
                if(sw == null || rejected == null)
                {
                    Console.WriteLine("Problem with initialization!");
                    or.ResultType = ResultType.Failed;
                    or.ResultMessage = "Problem with initialization!";
                    return or;
                }

                var result = Sample.validateSample(
                    sample.Timestamp.ToString("yyyy/MM/dd hh:mm:ss"),
                    sample.Voltage_RMS_Min.ToString(),
                    sample.Voltage_RMS_Avg.ToString(),
                    sample.Voltage_RMS_Max.ToString(),
                    sample.Current_RMS_Min_A.ToString(),
                    sample.Current_RMS_Avg_A.ToString(),
                    sample.Current_RMS_Max_A.ToString(),
                    sample.Real_Power_Min_kW.ToString(),
                    sample.Real_Power_Avg_kW.ToString(),
                    sample.Real_Power_Max_kW.ToString(),
                    sample.Reactive_Power_Min_kVAR.ToString(),
                    sample.Reactive_Power_Avg_kVAR.ToString(),
                    sample.Reactive_Power_Max_kVAR.ToString(),
                    sample.Apparent_Power_Min_kVA.ToString(),
                    sample.Apparent_Power_Avg_kVA.ToString(),
                    sample.Apparent_Power_Max_kVA.ToString(),
                    sample.Frequency_Min_Hz.ToString(),
                    sample.Frequency_Avg_Hz.ToString(),
                    sample.Frequency_Max_Hz.ToString()
                );

                if (result.ResultType == ResultType.Success)
                {
                    sw.WriteLine(sample.ToString());
                    sw.Flush();
                }
                else
                {
                    rejected.WriteLine($"Invalid sample: {sample.ToString()}");
                    rejected.WriteLine($"Reason: {result.ResultMessage}");
                    rejected.Flush();
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
            vehicleID = vehicleId;
            Console.WriteLine(path);
            try
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                
                sw = new StreamWriter((new FileStream(path, FileMode.Create, FileAccess.Write)));
                rejected = new StreamWriter(new FileStream(directory+"rejects.csv",FileMode.Create,FileAccess.Write));

                OperationResult or = new OperationResult();

                if (File.Exists(path))
                {
                    or.ResultType = ResultType.Success;
                    or.ResultMessage = "Transfer Started!";
                   
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
