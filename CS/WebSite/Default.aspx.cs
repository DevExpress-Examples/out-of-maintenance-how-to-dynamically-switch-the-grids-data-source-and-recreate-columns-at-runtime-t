using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DevExpress.Web.ASPxGridView;

public partial class _Default : System.Web.UI.Page {
    string DataSessionKey {
        get {
            object value = Session["DataSessionKey"];
            return (value != null) ? (string)value : String.Empty;
        }
        set {
            Session["DataSessionKey"] = value;
        }
    }


    DataSet GetCategoryProductData() {
        DataSet data = (DataSet)Session["CategoryProduct"];
        if (data == null) {
            DataTable masterData = DataHelper.ProcessSelectCommand("SELECT [CategoryID], [CategoryName], [Description] FROM [Categories]");
            masterData.PrimaryKey = new DataColumn[] { masterData.Columns["CategoryID"] };
            masterData.TableName = "Master";

            DataTable detailData = DataHelper.ProcessSelectCommand("SELECT [ProductID], [ProductName], [UnitPrice], [Discontinued], [CategoryID] FROM [Products]");
            detailData.PrimaryKey = new DataColumn[] { detailData.Columns["ProductID"] };
            detailData.TableName = "Detail";

            data = new DataSet();
            data.Tables.Add(masterData);
            data.Tables.Add(detailData);
            data.Relations.Add("MasterDetail", masterData.Columns["CategoryID"], detailData.Columns["CategoryID"]);

            Session["CategoryProduct"] = data;
        }
        return data;
    }

    DataSet GetCustomerOrderData() {
        DataSet data = (DataSet)Session["CustomerOrder"];
        if (data == null) {
            DataTable masterData = DataHelper.ProcessSelectCommand("SELECT [CustomerID], [CompanyName], [ContactTitle], [City], [Country] FROM [Customers]");
            masterData.PrimaryKey = new DataColumn[] { masterData.Columns["CustomerID"] };
            masterData.TableName = "Master";

            DataTable detailData = DataHelper.ProcessSelectCommand("SELECT [OrderID], [OrderDate], [ShipName], [ShipCity], [ShipCountry], [CustomerID] FROM [Orders]");
            detailData.PrimaryKey = new DataColumn[] { detailData.Columns["OrderID"] };
            detailData.TableName = "Detail";

            data = new DataSet();
            data.Tables.Add(masterData);
            data.Tables.Add(detailData);
            data.Relations.Add("MasterDetail", masterData.Columns["CustomerID"], detailData.Columns["CustomerID"]);

            Session["CustomerOrder"] = data;
        }
        return data;
    }

    DataSet GetData() {
        switch (DataSessionKey) {
            case "CategoryProduct":
                return GetCategoryProductData();
            case "CustomerOrder":
                return GetCustomerOrderData();
            default:
                return null;
        }
    }

    DataTable GetMasterData() {
        DataSet data = GetData();
        if (data != null)
            return data.Tables["Master"];
        else {
            return null;
        }
    }

    DataView GetDetailData(object masterRowKey) {
        DataSet data = GetData();
        if (data != null) {
            DataTable detail = data.Tables["Detail"];
            string columnName = data.Relations["MasterDetail"].ParentColumns[0].ColumnName;
            return new DataView(detail, String.Format("[{0}] = '{1}'", columnName, masterRowKey), String.Empty, DataViewRowState.CurrentRows);
        }
        else {
            return null;
        }
    }    

    void Page_Init(object sender, EventArgs e) {
        if (!IsPostBack) {
            DataSessionKey = "CategoryProduct";
            gvMaster.DataBind();
        }
    }

    void Page_Load(object sender, EventArgs e) {
        string dataKey = (rbCatProd.Checked) ? "CategoryProduct" : "CustomerOrder";
        if (dataKey != DataSessionKey) {
            DataSessionKey = dataKey;            
            gvMaster.DataBind();
        }
    }
    
    protected void gvMaster_DataBinding(object sender, EventArgs e) {
        ASPxGridView gridView = sender as ASPxGridView;

        gridView.Columns.Clear();
        gridView.AutoGenerateColumns = true;
        DataTable data = GetMasterData();
        gridView.KeyFieldName = data.PrimaryKey[0].ColumnName;
        gridView.DataSource = data;
    }

    protected void gvDetail_DataBinding(object sender, EventArgs e) {
        ASPxGridView gridView = sender as ASPxGridView;

        DataView dataView = GetDetailData(gridView.GetMasterRowKeyValue());
        gridView.KeyFieldName = dataView.Table.PrimaryKey[0].ColumnName;
        gridView.DataSource = dataView;
    }
    protected void gvDetail_DataBound(object sender, EventArgs e) {
        ASPxGridView gridView = sender as ASPxGridView;
        GridViewDetailRowTemplateContainer container = gridView.NamingContainer as GridViewDetailRowTemplateContainer;

        gridView.Columns[container.Grid.KeyFieldName].Visible = false;
    }
}