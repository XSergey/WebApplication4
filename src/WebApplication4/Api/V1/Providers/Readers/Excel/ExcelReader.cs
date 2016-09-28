using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace WebApplication4.Api.V1.Providers.Readers.Excel
{
    public class ExcelReader
    {
        public string FilePath;
        public bool HasHeader = false;
        public static Dictionary<int, object> table;
        //public DataTable data = new DataTable();

        public ExcelReader(string path, bool hasHeader = true)
        {
            FilePath = path;
            HasHeader = hasHeader;
        }
        public async Task<Dictionary<int, object>> ReadExcel()
        {
            Dictionary<int, object> directory = new Dictionary<int, object>();
            FileInfo file = new FileInfo(FilePath);

            using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage(file))
            {

                /*using (var stream = File.OpenRead(file))
                {
                    pck.Load(stream);
                }*/

                var ws = pck.Workbook.Worksheets.First();
                //DataTable tbl = new DataTable();

                /*foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }*/

                var startRow = HasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    //DataRow row = tbl.Rows.Add();

                    Dictionary<int, object> _rows = new Dictionary<int, object>();

                    foreach (var cell in wsRow)
                    {
                        //row[cell.Start.Column - 1] = cell.Text;
                        _rows.Add(cell.Start.Column - 1, cell.Text);
                    }

                    directory.Add(
                        rowNum,
                        _rows
                    );
                }

                //table = directory;
                
            }
            return await Task.Run(() => { return directory; } );
        }
    }
}
