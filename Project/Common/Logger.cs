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
        public static Sample sample { get; set; }

        public static void Log(string file, Sample sample)
        {
            date = DateTime.Now;
            using(FileStream fs = new FileStream(file, FileMode.Append, FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(date.ToString("yyyy/MM/dd HH:mm:ss =>") + sample.ToString());
            }
        }
    }
}
