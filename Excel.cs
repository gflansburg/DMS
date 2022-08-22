using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Gafware.Modules.DMS
{
    public class Excel
    {
        public static ExcelPackage ExportListToExcel<T>(System.Collections.Generic.IList<T>[] list)
        {
            return ExportListToExcel(list, false);
        }

        public static ExcelPackage ExportListToExcel<T>(System.Collections.Generic.IList<T>[] list, bool useClassHeader)
        {
            ExcelPackage excel = new ExcelPackage();
            foreach (System.Collections.Generic.IList<T> obj in list)
            {
                string worksheetName = obj.GetType().ToString();
                if (worksheetName.Contains('.'))
                {
                    worksheetName = worksheetName.Substring(worksheetName.LastIndexOf('.') + 1);
                }
                excel.Workbook.Worksheets.Add(worksheetName);
                ExportToExcel(obj, excel, worksheetName, useClassHeader ? worksheetName : String.Empty);
            }
            return excel;
        }

        public static ExcelPackage ExportListToExcel<T>(System.Collections.Generic.IList<T>[] list, string header)
        {
            ExcelPackage excel = new ExcelPackage();
            foreach (System.Collections.Generic.IList<T> obj in list)
            {
                string worksheetName = obj.GetType().ToString();
                if (worksheetName.Contains('.'))
                {
                    worksheetName = worksheetName.Substring(worksheetName.LastIndexOf('.') + 1);
                }
                excel.Workbook.Worksheets.Add(worksheetName);
                ExportToExcel(obj, excel, worksheetName, header);
            }
            return excel;
        }

        public static ExcelPackage ExportListToExcel<T>(System.Collections.Generic.IList<T> list, bool useClassHeader)
        {
            string worksheetName = typeof(T).ToString();
            if (worksheetName.Contains('.'))
            {
                worksheetName = worksheetName.Substring(worksheetName.LastIndexOf('.') + 1);
            }
            ExcelPackage excel = new ExcelPackage();
            excel.Workbook.Worksheets.Add(worksheetName);
            ExportToExcel(list, excel, worksheetName, useClassHeader ? worksheetName : String.Empty);
            return excel;
        }

        public static ExcelPackage ExportListToExcel<T>(System.Collections.Generic.IList<T> list, string header)
        {
            string worksheetName = typeof(T).ToString();
            if (worksheetName.Contains('.'))
            {
                worksheetName = worksheetName.Substring(worksheetName.LastIndexOf('.') + 1);
            }
            ExcelPackage excel = new ExcelPackage();
            excel.Workbook.Worksheets.Add(worksheetName);
            ExportToExcel(list, excel, worksheetName, header);
            return excel;
        }

        public static void ExportToExcel<T>(System.Collections.Generic.IList<T> list, ExcelPackage excel)
        {
            string worksheetName = typeof(T).ToString();
            if (worksheetName.Contains('.'))
            {
                worksheetName = worksheetName.Substring(worksheetName.LastIndexOf('.') + 1);
            }
            ExportToExcel(list, excel, worksheetName);
        }

        public static void ExportToExcel<T>(System.Collections.Generic.IList<T> list, ExcelPackage excel, string worksheet)
        {
            ExportToExcel(list, excel, worksheet, String.Empty);
        }

        public static void ExportToExcel<T>(System.Collections.Generic.IList<T> list, ExcelPackage excel, string worksheet, string header)
        {
            System.ComponentModel.PropertyDescriptorCollection props = System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            List<string> headers = new List<string>();
            for (int x = 0; x < props.Count; x++)
            {
                System.ComponentModel.PropertyDescriptor prop = props[x];
                bool ignore = typeof(T).GetProperty(prop.Name).GetCustomAttributes(false).Any(a => a is System.Xml.Serialization.XmlIgnoreAttribute);
                if (!ignore)
                {
                    if (prop.PropertyType.IsPrimitive || prop.PropertyType.BaseType.Equals(typeof(System.Enum)) || prop.PropertyType.Equals(typeof(decimal)) || prop.PropertyType.Equals(typeof(String)) || (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Equals(typeof(DateTime)))
                    {
                        headers.Add(GetPropertyDisplayString<T>(prop.Name));
                    }
                }
            }
            var excelWorksheet = excel.Workbook.Worksheets[worksheet];
            List<string[]> headerRow = new List<string[]>();
            headerRow.Add(headers.ToArray());
            int row = 1;
            string lastColumn = ColumnIndexToColumnLetter(headerRow[0].Length);
            if (header.Length > 0)
            {
                string headerRowRange = "A1:" + lastColumn + "1";
                excelWorksheet.Cells[headerRowRange].Merge = true;
                excelWorksheet.Cells["A1"].Style.Font.Bold = true;
                excelWorksheet.Cells["A1"].Style.Font.Size = 18;
                excelWorksheet.Cells["A1"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                excelWorksheet.Cells["A1"].Value = header;
                row++;
            }
            string headerRange = String.Concat("A", row, ":", lastColumn, row);
            excelWorksheet.Cells[headerRange].LoadFromArrays(headerRow);
            excelWorksheet.Cells[headerRange].Style.Font.Bold = true;
            excelWorksheet.Cells[headerRange].Style.Font.Size = 14;
            excelWorksheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Black);
            List<object[]> data = new List<object[]>();
            foreach (T item in list)
            {
                List<object> obj = new List<object>();
                for (int y = 0; y < props.Count; y++)
                {
                    System.ComponentModel.PropertyDescriptor prop = props[y];
                    bool ignore = typeof(T).GetProperty(prop.Name).GetCustomAttributes(false).Any(a => a is System.Xml.Serialization.XmlIgnoreAttribute);
                    if (!ignore)
                    {
                        if (prop.PropertyType.IsPrimitive || prop.PropertyType.BaseType.Equals(typeof(System.Enum)) || prop.PropertyType.Equals(typeof(decimal)) || prop.PropertyType.Equals(typeof(String)) || (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Equals(typeof(DateTime)))
                        {
                            DataType dta = GetPropertyDataType<T>(prop.Name);
                            System.Type rowType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            switch (rowType.ToString())
                            {
                                case "System.DBNull":
                                    obj.Add(null);
                                    break;
                                default:
                                    obj.Add(prop.GetValue(item));
                                    break;
                            }
                        }
                    }
                }
                data.Add(obj.ToArray());
            }
            excelWorksheet.Cells[2, 1].LoadFromArrays(data);
            int rowCount = list.Count;
            if (rowCount > 0)
            {
                int col = 1;
                for (int x = 0; x < props.Count; x++)
                {
                    System.ComponentModel.PropertyDescriptor prop = props[x];
                    bool ignore = typeof(T).GetProperty(prop.Name).GetCustomAttributes(false).Any(a => a is System.Xml.Serialization.XmlIgnoreAttribute);
                    if (!ignore)
                    {
                        if (prop.PropertyType.IsPrimitive || prop.PropertyType.BaseType.Equals(typeof(System.Enum)) || prop.PropertyType.Equals(typeof(decimal)) || prop.PropertyType.Equals(typeof(String)) || (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType).Equals(typeof(DateTime)))
                        {
                            System.Type type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            DataType dta = GetPropertyDataType<T>(prop.Name);
                            switch (type.ToString())
                            {
                                case "System.DateTime":
                                    string DateCellFormat = "mm/dd/yyyy;@";
                                    using (ExcelRange Rng = excelWorksheet.Cells[String.Format("{0}{1}:{0}{2}", ColumnIndexToColumnLetter(col), row + 1, rowCount + row)])
                                    {
                                        Rng.Style.Numberformat.Format = DateCellFormat;
                                    }
                                    break;
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Byte":
                                    using (ExcelRange Rng = excelWorksheet.Cells[String.Format("{0}{1}:{0}{2}", ColumnIndexToColumnLetter(col), row + 1, rowCount + row)])
                                    {
                                        Rng.Style.Numberformat.Format = "@";
                                    }
                                    break;
                                case "System.Decimal":
                                case "System.Double":
                                case "System.Single":
                                    string cellFormat = "#0\\.0000";
                                    if (dta == DataType.Currency)
                                    {
                                        cellFormat = "$###,###,##0.00";
                                    }
                                    using (ExcelRange Rng = excelWorksheet.Cells[String.Format("{0}{1}:{0}{2}", ColumnIndexToColumnLetter(col), row + 1, rowCount + row)])
                                    {
                                        Rng.Style.Numberformat.Format = cellFormat;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            col++;
                        }
                    }
                }
            }
            excelWorksheet.Cells[String.Concat("A", row, ":", lastColumn, rowCount + row)].AutoFitColumns();
        }

        public static ExcelPackage ExportTableToExcel(System.Data.DataSet ds)
        {
            return ExportTableToExcel(ds, false);
        }

        public static ExcelPackage ExportTableToExcel(System.Data.DataSet ds, bool useTableHeader)
        {
            ExcelPackage excel = new ExcelPackage();
            foreach (System.Data.DataTable tbl in ds.Tables)
            {
                excel.Workbook.Worksheets.Add(tbl.TableName);
                string header = useTableHeader ? tbl.TableName : String.Empty;
                ExportToExcel(tbl, excel, tbl.TableName, (!String.IsNullOrEmpty(header) ? ds.DataSetName + (!String.IsNullOrEmpty(ds.DataSetName) ? ": " : String.Empty) + header : String.Empty));
            }
            return excel;
        }

        public static ExcelPackage ExportTableToExcel(System.Data.DataSet ds, string header)
        {
            ExcelPackage excel = new ExcelPackage();
            foreach (System.Data.DataTable tbl in ds.Tables)
            {
                excel.Workbook.Worksheets.Add(tbl.TableName);
                ExportToExcel(tbl, excel, tbl.TableName, header);
            }
            return excel;
        }

        public static ExcelPackage ExportTableToExcel(System.Data.DataTable tbl)
        {
            return ExportTableToExcel(tbl, String.Empty);
        }

        public static ExcelPackage ExportTableToExcel(System.Data.DataTable tbl, bool useTableHeader)
        {
            ExcelPackage excel = new ExcelPackage();
            excel.Workbook.Worksheets.Add(tbl.TableName);
            ExportToExcel(tbl, excel, tbl.TableName, useTableHeader ? tbl.TableName : String.Empty);
            return excel;
        }

        public static ExcelPackage ExportTableToExcel(System.Data.DataTable tbl, string header)
        {
            ExcelPackage excel = new ExcelPackage();
            excel.Workbook.Worksheets.Add(tbl.TableName);
            ExportToExcel(tbl, excel, tbl.TableName, header);
            return excel;
        }

        public static void ExportToExcel(System.Data.DataTable tbl, ExcelPackage excel)
        {
            ExportToExcel(tbl, excel, tbl.TableName);
        }

        public static void ExportToExcel(System.Data.DataTable tbl, ExcelPackage excel, string worksheet)
        {
            ExportToExcel(tbl, excel, worksheet, String.Empty);
        }

        public static ExcelPackage ExportViewToExcel(System.Data.DataView view)
        {
            return ExportViewToExcel(view, false);
        }

        public static ExcelPackage ExportViewToExcel(System.Data.DataView view, string[] ignoreFields)
        {
            return ExportViewToExcel(view, false, ignoreFields);
        }

        public static ExcelPackage ExportViewToExcel(System.Data.DataView view, bool useTableHeader)
        {
            ExcelPackage excel = new ExcelPackage();
            excel.Workbook.Worksheets.Add(view.Table.TableName);
            string header = useTableHeader ? view.Table.TableName : String.Empty;
            ExportToExcel(view, excel, view.Table.TableName, !String.IsNullOrEmpty(header) ? header : string.Empty, null);
            return excel;
        }

        public static ExcelPackage ExportViewToExcel(System.Data.DataView view, bool useTableHeader, string[] ignoreFields)
        {
            ExcelPackage excel = new ExcelPackage();
            excel.Workbook.Worksheets.Add(view.Table.TableName);
            string header = useTableHeader ? view.Table.TableName : String.Empty;
            ExportToExcel(view, excel, view.Table.TableName, !String.IsNullOrEmpty(header) ? header : string.Empty, ignoreFields);
            return excel;
        }

        public static ExcelPackage ExportViewToExcel(System.Data.DataView view, string header)
        {
            ExcelPackage excel = new ExcelPackage();
            excel.Workbook.Worksheets.Add(view.Table.TableName);
            ExportToExcel(view, excel, view.Table.TableName, header, null);
            return excel;
        }

        public static ExcelPackage ExportViewToExcel(System.Data.DataView view, string header, string[] ignoreFields)
        {
            ExcelPackage excel = new ExcelPackage();
            excel.Workbook.Worksheets.Add(view.Table.TableName);
            ExportToExcel(view, excel, view.Table.TableName, header, ignoreFields);
            return excel;
        }

        private static string ColumnIndexToColumnLetter(int colIndex)
        {
            int div = colIndex;
            string colLetter = String.Empty;
            int mod = 0;

            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        }

        public static void ExportToExcel(System.Data.DataTable tbl, ExcelPackage excel, string worksheet, string header)
        {
            var excelWorksheet = excel.Workbook.Worksheets[worksheet];
            List<string> headers = new List<string>();
            for (int x = 0; x < tbl.Columns.Count; x++)
            {
                headers.Add(tbl.Columns[x].ColumnName);
            }
            List<string[]> headerRow = new List<string[]>();
            headerRow.Add(headers.ToArray());
            int row = 1;
            string lastColumn = ColumnIndexToColumnLetter(headerRow[0].Length);
            if (header.Length > 0)
            {
                string headerRowRange = "A1:" + lastColumn + "1";
                excelWorksheet.Cells[headerRowRange].Merge = true;
                excelWorksheet.Cells["A1"].Style.Font.Bold = true;
                excelWorksheet.Cells["A1"].Style.Font.Size = 18;
                excelWorksheet.Cells["A1"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                excelWorksheet.Cells["A1"].Value = header;
                row++;
            }
            string headerRange = String.Concat("A", row, ":", lastColumn, row);
            excelWorksheet.Cells[headerRange].LoadFromArrays(headerRow);
            excelWorksheet.Cells[headerRange].Style.Font.Bold = true;
            excelWorksheet.Cells[headerRange].Style.Font.Size = 14;
            excelWorksheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Black);
            List<object[]> data = new List<object[]>();
            foreach (System.Data.DataRow x in tbl.Rows)
            {
                List<object> obj = new List<object>();
                for (int y = 0; y < tbl.Columns.Count; y++)
                {
                    System.Type rowType = x[y].GetType();
                    switch (rowType.ToString())
                    {
                        case "System.DBNull":
                            obj.Add(null);
                            break;
                        default:
                            obj.Add(x[y]);
                            break;
                    }
                }
                data.Add(obj.ToArray());
            }
            excelWorksheet.Cells[2, 1].LoadFromArrays(data);
            int rowCount = tbl.Rows.Count;
            if (rowCount > 0)
            {
                for (int x = 0; x < tbl.Columns.Count; x++)
                {
                    Type type = tbl.Columns[x].DataType;
                    switch (type.ToString())
                    {
                        case "System.DateTime":
                            string DateCellFormat = "mm/dd/yyyy;@";
                            using (ExcelRange Rng = excelWorksheet.Cells[String.Format("{0}{1}:{0}{2}", ColumnIndexToColumnLetter(x + 1), row + 1, rowCount + row)])
                            {
                                Rng.Style.Numberformat.Format = DateCellFormat;
                            }
                            break;
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            using (ExcelRange Rng = excelWorksheet.Cells[String.Format("{0}{1}:{0}{2}", ColumnIndexToColumnLetter(x + 1), row + 1, rowCount + row)])
                            {
                                Rng.Style.Numberformat.Format = "@";
                            }
                            break;
                        case "System.Decimal":
                        case "System.Double":
                        case "System.Single":
                            string PersentageCellFormat = "#0\\.0000";
                            using (ExcelRange Rng = excelWorksheet.Cells[String.Format("{0}{1}:{0}{2}", ColumnIndexToColumnLetter(x + 1), row + 1, rowCount + row)])
                            {
                                Rng.Style.Numberformat.Format = PersentageCellFormat;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            excelWorksheet.Cells[String.Concat("A", row, ":", lastColumn, rowCount + row)].AutoFitColumns();
        }

        public static void ExportToExcel(System.Data.DataView view, ExcelPackage excel)
        {
            ExportToExcel(view, excel, view.Table.TableName);
        }

        public static void ExportToExcel(System.Data.DataView view, ExcelPackage excel, string[] ignoreFields)
        {
            ExportToExcel(view, excel, view.Table.TableName, ignoreFields);
        }

        public static void ExportToExcel(System.Data.DataView view, ExcelPackage excel, string worksheet)
        {
            ExportToExcel(view, excel, worksheet, String.Empty, null);
        }

        public static void ExportToExcel(System.Data.DataView view, ExcelPackage excel, string worksheet, string[] ignoreFields)
        {
            ExportToExcel(view, excel, worksheet, String.Empty, ignoreFields);
        }

        public static void ExportToExcel(System.Data.DataView view, ExcelPackage excel, string worksheet, string header, string[] ignoreFields)
        {
            var excelWorksheet = excel.Workbook.Worksheets[worksheet];
            List<string> headers = new List<string>();
            for (int x = 0; x < view.Table.Columns.Count; x++)
            {
                if (ignoreFields == null || !ignoreFields.Contains(view.Table.Columns[x].ColumnName))
                {
                    headers.Add(view.Table.Columns[x].ColumnName);
                }
            }
            List<string[]> headerRow = new List<string[]>();
            headerRow.Add(headers.ToArray());
            int row = 1;
            int nCol = 0;
            string lastColumn = ColumnIndexToColumnLetter(headerRow[0].Length);
            if (header.Length > 0)
            {
                string headerRowRange = "A1:" + lastColumn + "1";
                excelWorksheet.Cells[headerRowRange].Merge = true;
                excelWorksheet.Cells["A1"].Style.Font.Bold = true;
                excelWorksheet.Cells["A1"].Style.Font.Size = 18;
                excelWorksheet.Cells["A1"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                excelWorksheet.Cells["A1"].Value = header;
                row++;
            }
            string headerRange = String.Concat("A", row, ":", lastColumn, row);
            excelWorksheet.Cells[headerRange].LoadFromArrays(headerRow);
            excelWorksheet.Cells[headerRange].Style.Font.Bold = true;
            excelWorksheet.Cells[headerRange].Style.Font.Size = 14;
            excelWorksheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Black);
            List<object[]> data = new List<object[]>();
            foreach (System.Data.DataRowView y in view)
            {
                List<object> obj = new List<object>();
                nCol = 0;
                for (int x = 0; x < view.Table.Columns.Count; x++)
                {
                    if (ignoreFields == null || !ignoreFields.Contains(view.Table.Columns[x].ColumnName))
                    {

                        System.Type rowType = y[x].GetType();
                        switch (rowType.ToString())
                        {
                            case "System.DBNull":
                                obj.Add(null);
                                break;
                            default:
                                obj.Add(y[x]);
                                break;
                        }
                    }
                }
                data.Add(obj.ToArray());
            }
            excelWorksheet.Cells[2, 1].LoadFromArrays(data);
            int rowCount = view.Count;
            nCol = 0;
            if (rowCount > 0)
            {
                for (int x = 0; x < view.Table.Columns.Count; x++)
                {
                    if (ignoreFields == null || !ignoreFields.Contains(view.Table.Columns[x].ColumnName))
                    {
                        Type type = view.Table.Columns[x].DataType;
                        switch (type.ToString())
                        {
                            case "System.DateTime":
                                string DateCellFormat = "mm/dd/yyyy;@";
                                using (ExcelRange Rng = excelWorksheet.Cells[String.Format("{0}{1}:{0}{2}", ColumnIndexToColumnLetter(nCol + 1), row + 1, rowCount + row)])
                                {
                                    Rng.Style.Numberformat.Format = DateCellFormat;
                                }
                                break;
                            case "System.Int16":
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Byte":
                                using (ExcelRange Rng = excelWorksheet.Cells[String.Format("{0}{1}:{0}{2}", ColumnIndexToColumnLetter(nCol + 1), row + 1, rowCount + row)])
                                {
                                    Rng.Style.Numberformat.Format = "@";
                                }
                                break;
                            case "System.Decimal":
                            case "System.Double":
                            case "System.Single":
                                string PersentageCellFormat = "#0\\.0000";
                                using (ExcelRange Rng = excelWorksheet.Cells[String.Format("{0}{1}:{0}{2}", ColumnIndexToColumnLetter(nCol + 1), row + 1, rowCount + row)])
                                {
                                    Rng.Style.Numberformat.Format = PersentageCellFormat;
                                }
                                break;
                            default:
                                break;
                        }
                        nCol++;
                    }
                }
            }
            excelWorksheet.Cells[String.Concat("A", row, ":", lastColumn, rowCount + row)].AutoFitColumns();
        }

        public static DataType GetPropertyDataType<T>(string propertyName)
        {
            bool isDataTypeAttributeDefined = typeof(T).GetProperty(propertyName).GetCustomAttributes(false).Any(a => a is DataTypeAttribute);
            if (isDataTypeAttributeDefined)
            {
                DataTypeAttribute dta = (DataTypeAttribute)typeof(T).GetProperty(propertyName).GetCustomAttribute(typeof(DataTypeAttribute));
                return dta.DataType;
            }
            return DataType.Currency;
        }

        public static string GetPropertyDisplayString<T>(string propertyName)
        {
            bool isDisplayNameAttributeDefined = typeof(T).GetProperty(propertyName).GetCustomAttributes(false).Any(a => a is DisplayNameAttribute);
            if (isDisplayNameAttributeDefined)
            {
                DisplayNameAttribute dna = (DisplayNameAttribute)typeof(T).GetProperty(propertyName).GetCustomAttribute(typeof(DisplayNameAttribute));
                return dna.DisplayName;
            }
            bool isDisplayAttributeDefined = typeof(T).GetProperty(propertyName).GetCustomAttributes(false).Any(a => a is DisplayAttribute);
            if (isDisplayAttributeDefined)
            {
                DisplayAttribute da = (DisplayAttribute)typeof(T).GetProperty(propertyName).GetCustomAttribute(typeof(DisplayAttribute));
                return da.Name;
            }
            return propertyName;
        }
    }
}