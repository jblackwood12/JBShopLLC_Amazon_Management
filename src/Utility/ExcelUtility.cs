using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Utility
{
	public static class ExcelUtility
	{
		public static void WriteExcelFileToResponse<T>(HttpResponseBase response, IEnumerable<T> dataToWrite, string worksheetName, string fileName)
		{
			DataTable dt = dataToWrite.ToDataTable();

			using (ExcelPackage excel = new ExcelPackage())
			{
				ExcelWorksheet ws1 = excel.Workbook.Worksheets.Add(worksheetName);
				ws1.Cells["A1"].LoadFromDataTable(dt, true);

				if (Path.GetExtension(fileName) != ".xlsx")
					fileName = string.Format("{0}.xlsx", fileName);

				response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
				response.AddHeader("content-disposition", string.Format("attachment; filename={0}", fileName));
				response.BinaryWrite(excel.GetAsByteArray());
				response.End();
			}
		}

		public static void CreateExcelResponse(HttpResponseBase response, ExcelPackage excelPackage, string worksheetName)
		{
			if (Path.GetExtension(worksheetName) != ".xlsx")
				worksheetName = string.Format("{0}.xlsx", worksheetName);

			response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
			response.AddHeader("content-disposition", string.Format("attachment; filename={0}", worksheetName));
			response.BinaryWrite(excelPackage.GetAsByteArray());
			response.End();
		}

		// TODO: An out of range exception can occur when the Excel Spreadsheet is malformed.
		public static DataTable ConvertExcelFileToDataTable(HttpPostedFileBase excelStream)
		{
			if (excelStream == null)
				throw new ArgumentNullException("excelStream");

			DataTable dt = new DataTable();

			using (ExcelPackage excel = new ExcelPackage(excelStream.InputStream))
			{
				ExcelWorksheet ws = excel.Workbook.Worksheets.First();

				foreach (ExcelRangeBase firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
					dt.Columns.Add(firstRowCell.Text);

				const int startRow = 2;
				for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
				{
					ExcelRange excelRange = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
					DataRow row = dt.NewRow();
					foreach (var cell in excelRange)
						row[cell.Start.Column - 1] = cell.Text;
					dt.Rows.Add(row);
				}
			}

			return dt;
		}

		public static void ColorCells(this ExcelWorksheet excelWorksheet, string columnName, Func<string, Color> colorizer)
		{
			int? searchMethodColumnNum = null;

			int counter = 1;
			foreach (ExcelRangeBase firstRowCell in excelWorksheet.Cells[1, 1, 1, excelWorksheet.Dimension.End.Column])
			{
				if (firstRowCell.Text.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
					searchMethodColumnNum = counter;

				counter++;
			}

			if (searchMethodColumnNum.HasValue)
			{
				for (int i = 2; i <= excelWorksheet.Dimension.End.Row; i++)
				{
					string cellString = excelWorksheet.Cells[i, searchMethodColumnNum.Value].Text;

					excelWorksheet.Cells[i, searchMethodColumnNum.Value].Style.Fill.PatternType = ExcelFillStyle.Solid;

					Color searchMethodColor = colorizer.Invoke(cellString);

					excelWorksheet.Cells[i, searchMethodColumnNum.Value].Style.Fill.BackgroundColor.SetColor(searchMethodColor);
				}
			}
		}
	}
}