using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcSrcMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "Pertinax";
            if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);
#if DEBUG
            FileStream myTraceLog = new FileStream(appFolder + System.IO.Path.DirectorySeparatorChar + "OpcSrcMaker.deb", FileMode.Create);
            TextWriterTraceListener myListener = new TextWriterTraceListener(myTraceLog);
            Debug.Listeners.Add(myListener);
            Debug.AutoFlush = true;
            Debug.WriteLine("Start");
#endif

            try
            {
                ExcelReader excelReader = new ExcelReader();
                DataSet ds = excelReader.Read(@"e:\Projects\Pertinax\OpcSrcMaker\Data\E5390-8FB0-350_Data_pro_OPC_UA_r10.xls");
                foreach(DataTable dt in ds.Tables)
                {
                    Console.WriteLine($"  {dt.TableName}");
                }
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Exception: {exc.Message}");
            }
        }
    }
}
