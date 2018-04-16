using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DataProvider
/// </summary>
public class DataProvider
{
	public static string DataSessionKey {
		get {
			object value = HttpContext.Current.Session["DataSessionKey"];
			return (value != null) ? (string)value : "CategoryProduct";
		}
		set {
			HttpContext.Current.Session["DataSessionKey"] = value;
		}
	}

	public static DataTable GetMasterData() {
		DataSet data = GetData();
		if (data != null)
			return data.Tables["Master"];
		else
			return null;
	}

	public static DataView GetDetailData(object masterRowKey) {
		DataSet data = GetData();
		if (data != null) {
			DataTable detail = data.Tables["Detail"];
			string columnName = data.Relations["MasterDetail"].ParentColumns[0].ColumnName;
			return new DataView(detail, String.Format("[{0}] = '{1}'", columnName, masterRowKey), String.Empty, DataViewRowState.CurrentRows);
		}
		else
			return null;
	}

	private static DataSet GetData() {
		switch (DataSessionKey) {
			case "CategoryProduct":
				return GetCategoryProductData();
			case "CustomerOrder":
				return GetCustomerOrderData();
			default:
				return null;
		}
	}

	private static DataSet GetCategoryProductData() {
		DataSet data = (DataSet)HttpContext.Current.Session["CategoryProduct"];
		if (data == null) {
			data = RequestData("SELECT [CategoryID], [CategoryName], [Description] FROM [Categories]"
				, "CategoryID"
				, "SELECT [ProductID], [ProductName], [UnitPrice], [Discontinued], [CategoryID] FROM [Products]"
				, "ProductID");
			HttpContext.Current.Session["CategoryProduct"] = data;
		}
		return data;
	}

	private static DataSet GetCustomerOrderData() {
		DataSet data = (DataSet)HttpContext.Current.Session["CustomerOrder"];
		if (data == null) {
			data = RequestData("SELECT [CustomerID], [CompanyName], [ContactTitle], [City], [Country] FROM [Customers]"
				, "CustomerID"
				, "SELECT [OrderID], [OrderDate], [ShipName], [ShipCity], [ShipCountry], [CustomerID] FROM [Orders]"
				, "OrderID");

			HttpContext.Current.Session["CustomerOrder"] = data;
		}
		return data;
	}

	private static DataSet RequestData(string masterSelectCommand, string masterTableKeyField, string detailSelectCommand, string detailTableKeyField) {
		DataTable masterData = DataHelper.ProcessSelectCommand(masterSelectCommand);
		masterData.PrimaryKey = new DataColumn[] { masterData.Columns[masterTableKeyField] };
		masterData.TableName = "Master";

		DataTable detailData = DataHelper.ProcessSelectCommand(detailSelectCommand);
		detailData.PrimaryKey = new DataColumn[] { detailData.Columns[detailTableKeyField] };
		detailData.TableName = "Detail";

		DataSet data = new DataSet();
		data.Tables.Add(masterData);
		data.Tables.Add(detailData);
		data.Relations.Add("MasterDetail", masterData.Columns[masterTableKeyField], detailData.Columns[masterTableKeyField]);

		return data;
	}
}