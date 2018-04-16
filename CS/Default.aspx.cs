using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using DevExpress.Web.ASPxGridView;

public partial class _Default : System.Web.UI.Page {

	protected void Page_Init(object sender, EventArgs e) {
		if (!IsPostBack) {
			DataProvider.DataSessionKey = "CategoryProduct";
			gvMaster.DataBind();	
		}
		
	}

	protected void Page_Load(object sender, EventArgs e) {
		string dataKey = (rbCatProd.Checked) ? "CategoryProduct" : "CustomerOrder";
		if (dataKey != DataProvider.DataSessionKey) {
			ResetMasterDataSource(dataKey);
		}
	}

	protected void gvMaster_DataBinding(object sender, EventArgs e) {
		ASPxGridView gridView = sender as ASPxGridView;
		
		DataTable data = DataProvider.GetMasterData();
		gridView.KeyFieldName = data.PrimaryKey[0].ColumnName;
		gridView.DataSource = data;
	}

	protected void gvDetail_DataBinding(object sender, EventArgs e) {
		ASPxGridView gridView = sender as ASPxGridView;

		DataView dataView = DataProvider.GetDetailData(gridView.GetMasterRowKeyValue());
		gridView.KeyFieldName = dataView.Table.PrimaryKey[0].ColumnName;
		gridView.DataSource = dataView;
	}
	protected void gvDetail_DataBound(object sender, EventArgs e) {
		ASPxGridView gridView = sender as ASPxGridView;
		GridViewDetailRowTemplateContainer container = gridView.NamingContainer as GridViewDetailRowTemplateContainer;

		gridView.Columns[container.Grid.KeyFieldName].Visible = false;
	}

	private void ResetMasterDataSource(string dataKey) {
		DataProvider.DataSessionKey = dataKey;
		gvMaster.Columns.Clear();
		gvMaster.AutoGenerateColumns = true;
		gvMaster.DataBind();
	}

	
}