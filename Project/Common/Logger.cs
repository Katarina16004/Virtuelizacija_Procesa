using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Logger
    {
        public static string fileName { get; set; }

        public static DateTime date { get; set; }
        public static string sample { get; set; }

        private static StreamWriter sw;
        static Logger()
        {
            FileStream fs = new FileStream("Log.csv", FileMode.Create, FileAccess.ReadWrite);
             sw = new StreamWriter(fs);
        }
        public static void Log(string sample)
        {
            date = DateTime.Now;    
            sw.WriteLine(date.ToString("yyyy/MM/dd HH:mm:ss =>") + sample);
            sw.Flush();
        }

        public static void Dispose()
        {
            if(sw != null)
            {
                sw.Dispose();
                sw = null;
            }
        }
    }
}
