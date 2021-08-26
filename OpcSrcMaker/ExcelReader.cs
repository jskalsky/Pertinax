using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace OpcSrcMaker
{
    public class ExcelReader
    {
        public DataSet Read(string fileName)
        {
            DataSet result = new DataSet(Path.GetFileNameWithoutExtension(fileName));
            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Open(fileName);
            try
            {
                for (int i = 1; i <= workbook.Sheets.Count; ++i)
                {
                    Excel._Worksheet worksheet = workbook.Sheets[i];
                    Excel.Range range = worksheet.UsedRange;
                    DataTable dt = new DataTable(worksheet.Name);
                    result.Tables.Add(dt);
                    int rows = range.Rows.Count;
                    int cols = range.Columns.Count;
                    Debug.Print($"{worksheet.Name}: Col= {cols}, {rows}");
                    for (int col = 0; col < cols; ++col)
                    {
                        dt.Columns.Add(new DataColumn(col.ToString(), typeof(string)));
                    }
                    for (int row = 1; row <= rows; ++row)
                    {
                        Debug.Print($"row= {row}");
                        DataRow dataRow = dt.NewRow();
                        for (int col = 1; col <= cols; ++col)
                        {
                            string val = string.Empty;
                            if (range.Cells[row, col].Value2 != null)
                            {
                                val = range.Cells[row, col].Value2.ToString();
                            }
                            dataRow[col - 1] = val;
                        }
                        dt.Rows.Add(dataRow);
                    }
                    Console.WriteLine($"{worksheet.Name} END");
                    Marshal.ReleaseComObject(range);
                    Marshal.ReleaseComObject(worksheet);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Exc= {exc.Message}");
                StackTrace stackTrace = new StackTrace(exc, true);
                for (int i = 0; i < stackTrace.FrameCount; ++i)
                {
                    Console.WriteLine($"  {stackTrace.GetFrame(i).GetFileName()}, {stackTrace.GetFrame(i).GetFileLineNumber()} : {stackTrace.GetFrame(i).GetMethod().Name}");
                }
            }
            finally
            {
                workbook.Close();
                Marshal.ReleaseComObject(workbook);
                excelApp.Quit();
                Marshal.ReleaseComObject(excelApp);
            }
            return result;
        }
    }
}
