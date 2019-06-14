using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using log4net;
using Excel = Microsoft.Office.Interop.Excel;

namespace EngineEvaluation
{
    internal sealed class ExcelHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string m_FileName;

        private Dictionary<string, int> m_SheetRowCounts;  // Keep track of the rows we've written to for each sheet
        
        internal ExcelHandler(string fileName)
        {
            if (fileName == null)
            {
                Log.Error("No fileName was passed to ExcelHandler");
                throw new ArgumentNullException(nameof(fileName));
            }

            m_FileName = fileName;

            m_SheetRowCounts = new Dictionary<string, int>();
        }

        internal void CreateHeaders(string sheet, string[] headers)
        {
            Excel.Application excelApp       = null;
            Excel.Workbook    excelWorkbook  = null;
            Excel._Worksheet  excelWorksheet = null;

            try
            {
                excelApp = new Excel.Application
                {
                    Visible = false,
                    DisplayAlerts = false
                };

                excelWorkbook = excelApp.Workbooks.Open(m_FileName, ReadOnly: false);

                excelWorksheet = GetWorkSheet(excelWorkbook, sheet);
                
                for (var i = 0; i < headers.Length; i++)
                {
                    excelWorksheet.Cells[1, i+1] = headers[i];
                }
                
                excelWorkbook.Save();


                if (m_SheetRowCounts.ContainsKey(sheet))
                {
                    m_SheetRowCounts[sheet] = 2;
                }
                else
                {
                    m_SheetRowCounts.Add(sheet, 2);
                }

                excelApp.Workbooks.Close();
                excelApp.Quit();
            }
            catch (Exception exc)
            {
                Log.Error($"Error writing headers to excel log file: {m_FileName} - Sheet: {sheet}", exc);
            }
            finally
            {
                if(excelWorksheet != null) { Marshal.ReleaseComObject(excelWorksheet); }

                if(excelWorkbook != null) { Marshal.ReleaseComObject(excelWorkbook); }

                if(excelApp != null) { Marshal.ReleaseComObject(excelApp); }
            }
        }

        internal void AddDataToSheet(string sheet, string[] values)
        {
            Excel.Application excelApp = null;
            Excel.Workbook excelWorkbook = null;
            Excel._Worksheet excelWorksheet = null;

            try
            {
                excelApp = new Excel.Application
                {
                    Visible = false,
                    DisplayAlerts = false
                };

                excelWorkbook = excelApp.Workbooks.Open(m_FileName, ReadOnly: false);

                excelWorksheet = GetWorkSheet(excelWorkbook, sheet);

                int row;

                if (m_SheetRowCounts.ContainsKey(sheet))
                {
                    row = m_SheetRowCounts[sheet];

                    m_SheetRowCounts[sheet] = row + 1;
                }
                else
                {
                    row = 2;
                    m_SheetRowCounts.Add(sheet, 2);
                }

                for (var i = 0; i < values.Length; i++)
                {
                    excelWorksheet.Cells[row, i + 1] = values[i];
                }

                excelWorksheet.Columns.AutoFit();

                excelWorkbook.Save();

                excelApp.Workbooks.Close();
                excelApp.Quit();
            }
            catch (Exception exc)
            {
                Log.Error($"Error writing data to excel log file: {m_FileName} - Sheet: {sheet}", exc);
            }
            finally
            {
                if (excelWorksheet != null) { Marshal.ReleaseComObject(excelWorksheet); }

                if (excelWorkbook != null) { Marshal.ReleaseComObject(excelWorkbook); }

                if (excelApp != null) { Marshal.ReleaseComObject(excelApp); }
            }
        }

        private static Excel._Worksheet GetWorkSheet(Excel.Workbook workbook, string sheet)
        {
            Excel._Worksheet worksheet = null;

            var excelSheets = workbook.Worksheets;
            
            for (int i = 1; i <= excelSheets.Count; i++)
            {
                if (((Excel.Worksheet)excelSheets.Item[i]).Name == sheet)
                {
                    worksheet = (Excel.Worksheet)excelSheets.Item[i];
                    break;
                }
            }

            if (worksheet == null)
            {
                Log.Error($"Couldn't find sheet {sheet}");
                throw new ArgumentException($"Couldn't find sheet {sheet}");
            }

            return worksheet;
        }
    }
}
