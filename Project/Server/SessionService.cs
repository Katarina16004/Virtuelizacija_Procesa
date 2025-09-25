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
        public delegate void EventHandler(object sender, SampleEventArgs e);

        public static event EventHandler VoltageSpike;
        public static event EventHandler CurrentSpike;
        public static event EventHandler PowerFactorWarning;

        public static event EventHandler OnTransferStarted;
        public static event EventHandler OnSampleReceived;
        public static event EventHandler OnTransferCompleted;
        public static event EventHandler OnWarningRaised;

        private static StreamWriter sw;
        private static StreamWriter rejected;
        private static int vehicleID;
        bool first = true;
        public static string path;
        private static Sample previousSample=null;
        const float VoltageSpikeConst = 0.1f;
        const float CurrentSpikeConst = 0.1f;
        const float PowerFactorConst = 1f;


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
            if (OnTransferCompleted != null)
                OnTransferCompleted(this, new SampleEventArgs(vehicleID, 0, "Transfer finished"));
        }

        public void PushSampleHeaders(string line)
        {
            sw.WriteLine(line);
            rejected.WriteLine(line);
        }

        public OperationResult PushSample(Sample sample)
        {
           OperationResult or = new OperationResult();
            int rejectedRowNum = 0;
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
                    sample.Timestamp.ToString("yyyy-MM-dd hh:mm:ss"),
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
                    int temp = sample.RowIndex;
                    sample.RowIndex = temp - rejectedRowNum;
                    sw.WriteLine(sample.ToString());
                    sw.Flush();

                    if (previousSample != null)
                    {
                        CheckSpikes(sample);
                    }

                    previousSample = sample;
                    AnalyzePowerFactor(sample);

                    or.ResultMessage = "Sample added successfully!";
                    or.ResultType = ResultType.Success;

                    if (OnSampleReceived != null)
                        OnSampleReceived(this, new SampleEventArgs(sample.vehicleId, sample.RowIndex, "Sample received"));

                }
                else
                {
                    sample.RowIndex = ++rejectedRowNum;
                    rejected.WriteLine($"Invalid sample: {sample.ToString()}");
                    rejected.WriteLine($"Reason: {result.ResultMessage}");
                    rejected.Flush();
                    or.ResultType = ResultType.Failed;
                    or.ResultMessage = result.ResultMessage;

                    if (OnWarningRaised != null)
                        OnWarningRaised(this, new SampleEventArgs(sample.vehicleId, sample.RowIndex, "Sample rejected"));
                }
            }
            catch (Exception ex)
            {
                or.ResultType = ResultType.Failed;
                or.ResultMessage = "Error while pushing sample!" + ex.Message;
                return or;
            }
            
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
                    Console.WriteLine("Transfer Started!");

                    if (OnTransferStarted != null)
                        OnTransferStarted(this, new SampleEventArgs(vehicleID, 0, "Transfer started"));

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
        private void CheckSpikes(Sample sample)
        {
            float deltaV = Math.Abs(sample.Voltage_RMS_Avg - previousSample.Voltage_RMS_Avg);
            float deltaI = Math.Abs(sample.Current_RMS_Avg_A - previousSample.Current_RMS_Avg_A);

            if (deltaV > VoltageSpikeConst && VoltageSpike != null)
            {
                VoltageSpike(this, new SampleEventArgs(sample.vehicleId, sample.RowIndex, $"Voltage spike detected deltaV={deltaV}"));
            }

            if (deltaI > CurrentSpikeConst && CurrentSpike != null)
            {
                CurrentSpike(this, new SampleEventArgs(sample.vehicleId, sample.RowIndex, $"Current spike detected deltaI={deltaI}"));
            }
        }
        private void AnalyzePowerFactor(Sample sample)
        {
            if (sample.Apparent_Power_Avg_kVA == 0) 
                return; 

            float pf = (float)(sample.Real_Power_Avg_kW / sample.Apparent_Power_Avg_kVA);

            if (pf < PowerFactorConst)
            {
                if (PowerFactorWarning!=null)
                    PowerFactorWarning(this, new SampleEventArgs(sample.vehicleId, sample.RowIndex, $"Power Factor warning detected PF={pf}"));
            }
        }

    }
}
