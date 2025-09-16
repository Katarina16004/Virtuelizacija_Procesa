using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Sample
    {
        public Sample() { }

        [DataMember]
        public int vehicleId { get; set; }

        [DataMember]
        public int RowIndex { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public float Voltage_RMS_Min { get; set; }

        [DataMember]
        public float Voltage_RMS_Avg { get; set; }

        [DataMember]
        public float Voltage_RMS_Max { get; set; }

        [DataMember]
        public float Current_RMS_Min_A { get; set; }

        [DataMember]
        public float Current_RMS_Avg_A { get; set; }

        [DataMember]
        public float Current_RMS_Max_A { get; set; }

        [DataMember]
        public float Real_Power_Min_kW { get; set; }

        [DataMember]
        public float Real_Power_Avg_kW { get; set; }

        [DataMember]
        public float Real_Power_Max_kW { get; set; }

        [DataMember]
        public float Reactive_Power_Min_kVAR { get; set; }

        [DataMember]
        public float Reactive_Power_Avg_kVAR { get; set; }
        
        [DataMember]
        public float Reactive_Power_Max_kVAR { get; set; }

        [DataMember]
        public float Apparent_Power_Min_kVA { get; set; }
        
        [DataMember]
        public float Apparent_Power_Avg_kVA { get; set; }
        
        [DataMember]
        public float Apparent_Power_Max_kVA { get; set; }

        [DataMember]
        public float Frequency_Min_Hz { get; set; }

        [DataMember]
        public float Frequency_Avg_Hz { get; set; }

        [DataMember]
        public float Frequency_Max_Hz { get; set; }


        public Sample
            (
            int vehicleId,
            int rowIndex,
            string timestamp, 
            string voltage_RMS_Min, 
            string voltage_RMS_Avg, 
            string voltage_RMS_Max, 
            string current_RMS_Min_A, 
            string current_RMS_Avg_A, 
            string current_RMS_Max_A, 
            string real_Power_Min_kW, 
            string real_Power_Avg_kW, 
            string real_Power_Max_kW, 
            string reactive_Power_Min_kVAR, 
            string reactive_Power_Avg_kVAR, 
            string reactive_Power_Max_kVAR, 
            string apparent_Power_Min_kVA, 
            string apparent_Power_Avg_kVA, 
            string apparent_Power_Max_kVA, 
            string frequency_Min_Hz, 
            string frequency_Avg_Hz, 
            string frequency_Max_Hz
            )
        {
            this.vehicleId = vehicleId;
            
            this.RowIndex = rowIndex;

            Timestamp = DateTime.ParseExact(timestamp, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);


            Voltage_RMS_Min = float.Parse(voltage_RMS_Min, CultureInfo.InvariantCulture);
            Voltage_RMS_Avg = float.Parse(voltage_RMS_Avg, CultureInfo.InvariantCulture);
            Voltage_RMS_Max = float.Parse(voltage_RMS_Max, CultureInfo.InvariantCulture);

            Current_RMS_Min_A = float.Parse(current_RMS_Min_A, CultureInfo.InvariantCulture);
            Current_RMS_Avg_A = float.Parse(current_RMS_Avg_A, CultureInfo.InvariantCulture);
            Current_RMS_Max_A = float.Parse(current_RMS_Max_A, CultureInfo.InvariantCulture);

            Real_Power_Min_kW = float.Parse(real_Power_Min_kW, CultureInfo.InvariantCulture);
            Real_Power_Avg_kW = float.Parse(real_Power_Avg_kW, CultureInfo.InvariantCulture);
            Real_Power_Max_kW = float.Parse(real_Power_Max_kW, CultureInfo.InvariantCulture);

            Reactive_Power_Min_kVAR = float.Parse(reactive_Power_Min_kVAR, CultureInfo.InvariantCulture);
            Reactive_Power_Avg_kVAR = float.Parse(reactive_Power_Avg_kVAR, CultureInfo.InvariantCulture);
            Reactive_Power_Max_kVAR = float.Parse(reactive_Power_Max_kVAR, CultureInfo.InvariantCulture);

            Apparent_Power_Min_kVA = float.Parse(apparent_Power_Min_kVA, CultureInfo.InvariantCulture);
            Apparent_Power_Avg_kVA = float.Parse(apparent_Power_Avg_kVA, CultureInfo.InvariantCulture);
            Apparent_Power_Max_kVA = float.Parse(apparent_Power_Max_kVA, CultureInfo.InvariantCulture);

            Frequency_Min_Hz = float.Parse(frequency_Min_Hz, CultureInfo.InvariantCulture);
            Frequency_Avg_Hz = float.Parse(frequency_Avg_Hz, CultureInfo.InvariantCulture);
            Frequency_Max_Hz = float.Parse(frequency_Max_Hz, CultureInfo.InvariantCulture);
                

        }

        public static OperationResult validateSample(params string[] parametrs )
        {
            OperationResult or = new OperationResult();
            if(parametrs.Length != 19)
            {
                or.ResultMessage = "Greska sa brojem prosledjenih parametara!";
                or.ResultType = ResultType.Failed;
                return or;
            }

            if (!DateTime.TryParseExact(parametrs[0], "yyyy-MM-dd HH:mm:ss",
                 CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                or.ResultMessage = "Greska sa formatom datuma i vremena (potrebno je da bude u fomratu yyyy-MM-dd HH:mm:ss !";
                or.ResultType = ResultType.Failed;
                return or;
            }

            bool[] MustBePositiveNumber = { true, true,true, false,false,false , false, false, false , false, false, false , false, false, false, true,true,true }; 
            for(int i = 1; i < parametrs.Length; i++)
            {
                if (!CheckNumber(parametrs[i], MustBePositiveNumber[i-1]))
                {
                    or.ResultMessage = "Greska prilikom validacije vrednosti!: "+ parametrs[i];
                    or.ResultType = ResultType.Failed;
                    return or;
                }
            }
            

            return or;

        }

        static bool CheckNumber(string value, bool mustBePositive)
        {
            if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
              
                return false;
            }

            if (mustBePositive && result < 0)
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return string.Join(",",
           RowIndex,
           vehicleId,
           Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
           Voltage_RMS_Min.ToString(),
           Voltage_RMS_Avg.ToString(),
           Voltage_RMS_Max.ToString(),
           Current_RMS_Min_A.ToString(),
           Current_RMS_Avg_A.ToString(),
           Current_RMS_Max_A.ToString(),
           Real_Power_Min_kW.ToString(),
           Real_Power_Avg_kW.ToString(),
           Real_Power_Max_kW.ToString(),
           Reactive_Power_Min_kVAR.ToString(),
           Reactive_Power_Avg_kVAR.ToString(),
           Reactive_Power_Max_kVAR.ToString(),
           Apparent_Power_Min_kVA.ToString(),
           Apparent_Power_Avg_kVA.ToString(),
           Apparent_Power_Max_kVA.ToString(),
           Frequency_Min_Hz.ToString(),
           Frequency_Avg_Hz.ToString(),
           Frequency_Max_Hz.ToString());
        }
    }
}
