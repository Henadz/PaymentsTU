using FrameworkExtend;
using PaymentsTU.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PaymentsTU.ViewModel
{
	internal sealed class PaymentReportViewModel : ViewModelBase, IReport
	{
		public string Title => "Отчет по платежам";

		public ObservableCollection<PaymentReportRow> Rows { get; private set; }
		public ObservableCollection<ColumnDescriptor> Columns { get; private set; }
	

		public PaymentReportViewModel()
		{
			Rows = new ObservableCollection<PaymentReportRow>();
			Columns = new ObservableCollection<ColumnDescriptor>();
		}

		public void Run<T>(T parameters) where T: IPeriodReportParams
		{
			var reportData = Dal.Instance.PaymentReport(parameters.StartDate, parameters.EndDate);

			var cols = new List<ColumnDescriptor>
			{
				new ColumnDescriptor{ HeaderText = "Id", DisplayMember = "EmployeeId" },
				new ColumnDescriptor{ HeaderText = "Ф.И.О.", DisplayMember = "Employee" },
				new ColumnDescriptor{ HeaderText = "DepId", DisplayMember = "DepartmentId" },
				new ColumnDescriptor{ HeaderText = "Подразделение", DisplayMember = "Department" },
			};

			var columnRowMap = new Dictionary<string, int>();
			var paymentTypeMap = new Dictionary<int, string>();

			foreach (var col in reportData.Columns)
			{
				columnRowMap.Add(col.ColumnName, col.Ordinal);
				if (col.IsVisible && col.ColumnName.StartsWith("PaymentType"))
				{
					paymentTypeMap.Add(col.Ordinal, col.ColumnName);
					cols.Add(new ColumnDescriptor { HeaderText = col.Caption, DisplayMember = "Cells[" + col.ColumnName + "]" });
				}
			}

			var rows = new List<PaymentReportRow>();
			foreach (var row in reportData.Rows)
			{
				var r = new PaymentReportRow
				{
					RowId = row.RowId,
					Cells = new Dictionary<string, object>(paymentTypeMap.Count)
				};
				foreach (var cell in row.Cells)
				{
					if (cell.ColumnId == columnRowMap["EmployeeId"])
						r.EmployeeId = cell.Value == null ? null : (int?)Convert.ToInt32(cell.Value);
					if (cell.ColumnId == columnRowMap["Surname"])
						r.Employee = (string)cell.Value;
					if (cell.ColumnId == columnRowMap["DepartmentId"])
						r.DepartmentId = cell.Value == null ? null : (int?)Convert.ToInt32(cell.Value);
					if (cell.ColumnId == columnRowMap["Department"])
						r.Department = (string)cell.Value;

					if (paymentTypeMap.TryGetValue(cell.ColumnId, out string t))
						r.Cells.Add(t, cell.Value);
				}
				rows.Add(r);
			}

			Columns = new ObservableCollection<ColumnDescriptor>(cols);
			Rows = new ObservableCollection<PaymentReportRow>(rows);
		}
	}

	internal sealed class PaymentReportRow
	{
		public int RowId { get; set; }
		public int? EmployeeId { get; set; }
		public string Employee { get; set; }
		public int? DepartmentId { get; set; }
		public string Department { get; set; }
		public Dictionary<string, object> Cells { get; set; } = new Dictionary<string, object>();
	}

	internal class PaymentReportParams: IPeriodReportParams
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}

	internal class ColumnDescriptor
	{
		public string HeaderText { get; set; }
		public string DisplayMember { get; set; }
	}
}
