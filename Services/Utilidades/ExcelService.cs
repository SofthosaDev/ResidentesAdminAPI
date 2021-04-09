
using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using ExcelDataReader;
using WsAdminResidentes.Helpers;

namespace WsAdminResidentes.Services.Utilidades
{
    public interface IExcelService
    {
        List<T> ConvertirALista<T>(string ruta_archivo) where T : class, new();
    }
    public class ExcelService : IExcelService
    {
        private DataTable dt;
        public List<T> ConvertirALista<T>(string ruta_archivo) where T : class, new()
        {
            this.dt = this.ConvertirATable(ruta_archivo);
            return dt.ToList<T>();
        }

        private DataTable ConvertirATable(string ruta)
        {

            DataTable dt = new DataTable();
            dt.Clear();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(ruta, FileMode.Open, FileAccess.Read))
            {
                Boolean primera = true;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {

                        if (primera)
                        {
                            for (int column = 0; column < reader.FieldCount; column++)
                            {
                                Object raw_value = reader.GetValue(column);
                                string value = "";
                                if (raw_value != null)
                                {
                                    value = raw_value.ToString();
                                }
                                dt.Columns.Add(value);
                            }
                        }
                        else
                        {
                            DataRow dr = dt.NewRow();
                            for (int column = 0; column < reader.FieldCount; column++)
                            {
                                Object raw_value = reader.GetValue(column);
                                string value = "";
                                if (raw_value != null)
                                {
                                    value = raw_value.ToString();
                                }

                                dr[column] = value;
                            }
                            dt.Rows.Add(dr);
                        }
                        primera = false;
                    }
                }
            }
            return dt;
            /* Microsoft.Office.Interop.Excel.Application objXL = null;
            Microsoft.Office.Interop.Excel.Workbook objWB = null;
            objXL = new Microsoft.Office.Interop.Excel.Application();
            objWB = objXL.Workbooks.Open(path);
            Microsoft.Office.Interop.Excel.Worksheet objSHT = objWB.Worksheets[0] as Microsoft.Office.Interop.Excel.Worksheet;

            int rows = objSHT.UsedRange.Rows.Count;
            int cols = objSHT.UsedRange.Columns.Count;
            DataTable dt = new DataTable();
            int noofrow = 1;

            for (int c = 1; c <= cols; c++)
            {
                string colname = objSHT.Cells[1, c].ToString();
                dt.Columns.Add(colname);
                noofrow = 2;
            }

            for (int r = noofrow; r <= rows; r++)
            {
                DataRow dr = dt.NewRow();
                for (int c = 1; c <= cols; c++)
                {
                    dr[c - 1] = objSHT.Cells[r, c].ToString();
                }
                dt.Rows.Add(dr);
            }

            objWB.Close();
            objXL.Quit();
            return dt; */
        }

    }

}
