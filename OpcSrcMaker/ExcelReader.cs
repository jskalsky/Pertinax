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
                    for (int row = 1; row <= rows; ++row)
                    {
                        DataRow dataRow = new DataTable().NewRow();
                        dt.Rows.Add(dataRow);
                        for (int col = 1; col <= cols; ++col)
                        {
                            string val = string.Empty;
                            if (range.Cells[row, col].Value2 != null)
                            {
                                val = range.Cells[row, col].Value2.ToString();
                            }
                            dataRow[col] = val;
                        }
                    }
                    Console.WriteLine($"{worksheet.Name} END");
                    Marshal.ReleaseComObject(range);
                    Marshal.ReleaseComObject(worksheet);
                }
            }
            catch (Exception exc)
            {
                Debug.Print($"Exc= {exc.Message}");
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
