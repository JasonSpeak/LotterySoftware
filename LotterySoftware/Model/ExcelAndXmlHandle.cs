using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using LotterySoftware.Properties;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace LotterySoftware.Model
{
    public class ExcelAndXmlHandle
    {
        private const int OfReadwrite = 2;
        private const int OfShareDenyNone = 0x40;
        [DllImport("kernel32.dll")]
        private static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);
        private static readonly IntPtr FileError = new IntPtr(-1);
        
        private static string GetValue(CellType cell, SharedStringTablePart stringTablePart)
        {
            if (cell.ChildElements.Count == 0)
            {
                return null;
            }
            var value = cell.CellValue.InnerText;
            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
            {
                value = stringTablePart.SharedStringTable.ChildElements[int.Parse(value)].InnerText;
            }
            return value;
        }

        public static void GetDrawers(List<Drawer> excelValueList)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Excel Files(*.xlsx)|*.xlsx"
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName == "") return ;
            var handle = _lopen(openFileDialog.FileName, OfReadwrite | OfShareDenyNone);
            if (handle == FileError)
            {
                MessageBox.Show(Resources.ExcelAndXmlHandle_GetDrawers_,Resources.ExcelAndXmlHandle_GetDrawers_文件正在使用);
                return;
            }
            CloseHandle(handle);
            var document = SpreadsheetDocument.Open(openFileDialog.FileName, false);
            var sheet = document.WorkbookPart.Workbook.Descendants<Sheet>().First();
            var worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id);
            var worksheet = worksheetPart.Worksheet;
            var rows = worksheet.Descendants<Row>();
            excelValueList.Clear();
            foreach (var row in rows)
            {
                var cellValues = new string[row.Count()];
                var i = 0;
                foreach (var openXmlElement in row)
                {
                    var cell = (Cell)openXmlElement;
                    var columnValues = GetValue(cell, document.WorkbookPart.SharedStringTablePart);
                    cellValues[i] = columnValues;
                    i++;
                }
                var drawer = new Drawer(cellValues[1], cellValues[0]);
                if (!string.IsNullOrEmpty(drawer.DrawCode))
                {
                    excelValueList.Add(drawer);
                }
            }
        }

        public static void ExportExcelList(ObservableCollection<Drawer> listBoxDrawer)
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "Excel Files(*.xlsx)|*.xlsx"
            };
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                var app = new Application
                {
                    Visible = false
                };
                var workbook = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                Microsoft.Office.Interop.Excel.Worksheet worksheet = workbook.Worksheets[1];
                worksheet.Range["A1"].Value = "奖项";
                for (var i = 2; i < listBoxDrawer.Count + 2; i++)
                {
                    worksheet.Range["A" + i].Value = listBoxDrawer[i - 2].AwardName;
                }
                worksheet.Range["B1"].Value = "奖品";
                for (var j = 2; j < listBoxDrawer.Count + 2; j++)
                {
                    worksheet.Range["B" + j].Value = listBoxDrawer[j - 2].AwardPrize;
                }
                worksheet.Range["C1"].Value = "身份证";
                for (var k = 2; k < listBoxDrawer.Count + 2; k++)
                {
                    worksheet.Range["C" + k].NumberFormatLocal = "@";
                    worksheet.Range["C" + k].Value = listBoxDrawer[k - 2].DrawCode;
                }
                worksheet.Range["D1"].Value = "中奖人";
                for (var r = 2; r < listBoxDrawer.Count + 2; r++)
                {
                    worksheet.Range["D" + r].Value = listBoxDrawer[r - 2].DrawName;
                }
                worksheet.Application.DisplayAlerts = true;
                worksheet.SaveAs(saveFileDialog.FileName);

                Marshal.ReleaseComObject(worksheet);
                workbook.Close();
                Marshal.ReleaseComObject(workbook);
                app.Quit();
                Marshal.ReleaseComObject(app);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}