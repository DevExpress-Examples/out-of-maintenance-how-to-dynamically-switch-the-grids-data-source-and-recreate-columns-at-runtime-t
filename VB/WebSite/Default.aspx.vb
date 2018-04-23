Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data
Imports DevExpress.Web.ASPxGridView

Partial Public Class _Default
	Inherits System.Web.UI.Page
	Private Property DataSessionKey() As String
		Get
			Dim value As Object = Session("DataSessionKey")
			If (value IsNot Nothing) Then
				Return CStr(value)
			Else
				Return String.Empty
			End If
		End Get
		Set(ByVal value As String)
			Session("DataSessionKey") = value
		End Set
	End Property


	Private Function GetCategoryProductData() As DataSet
		Dim data As DataSet = CType(Session("CategoryProduct"), DataSet)
		If data Is Nothing Then
			Dim masterData As DataTable = DataHelper.ProcessSelectCommand("SELECT [CategoryID], [CategoryName], [Description] FROM [Categories]")
			masterData.PrimaryKey = New DataColumn() { masterData.Columns("CategoryID") }
			masterData.TableName = "Master"

			Dim detailData As DataTable = DataHelper.ProcessSelectCommand("SELECT [ProductID], [ProductName], [UnitPrice], [Discontinued], [CategoryID] FROM [Products]")
			detailData.PrimaryKey = New DataColumn() { detailData.Columns("ProductID") }
			detailData.TableName = "Detail"

			data = New DataSet()
			data.Tables.Add(masterData)
			data.Tables.Add(detailData)
			data.Relations.Add("MasterDetail", masterData.Columns("CategoryID"), detailData.Columns("CategoryID"))

			Session("CategoryProduct") = data
		End If
		Return data
	End Function

	Private Function GetCustomerOrderData() As DataSet
		Dim data As DataSet = CType(Session("CustomerOrder"), DataSet)
		If data Is Nothing Then
			Dim masterData As DataTable = DataHelper.ProcessSelectCommand("SELECT [CustomerID], [CompanyName], [ContactTitle], [City], [Country] FROM [Customers]")
			masterData.PrimaryKey = New DataColumn() { masterData.Columns("CustomerID") }
			masterData.TableName = "Master"

			Dim detailData As DataTable = DataHelper.ProcessSelectCommand("SELECT [OrderID], [OrderDate], [ShipName], [ShipCity], [ShipCountry], [CustomerID] FROM [Orders]")
			detailData.PrimaryKey = New DataColumn() { detailData.Columns("OrderID") }
			detailData.TableName = "Detail"

			data = New DataSet()
			data.Tables.Add(masterData)
			data.Tables.Add(detailData)
			data.Relations.Add("MasterDetail", masterData.Columns("CustomerID"), detailData.Columns("CustomerID"))

			Session("CustomerOrder") = data
		End If
		Return data
	End Function

	Private Function GetData() As DataSet
		Select Case DataSessionKey
			Case "CategoryProduct"
				Return GetCategoryProductData()
			Case "CustomerOrder"
				Return GetCustomerOrderData()
			Case Else
				Return Nothing
		End Select
	End Function

	Private Function GetMasterData() As DataTable
		Dim data As DataSet = GetData()
		If data IsNot Nothing Then
			Return data.Tables("Master")
		Else
			Return Nothing
		End If
	End Function

	Private Function GetDetailData(ByVal masterRowKey As Object) As DataView
		Dim data As DataSet = GetData()
		If data IsNot Nothing Then
			Dim detail As DataTable = data.Tables("Detail")
			Dim columnName As String = data.Relations("MasterDetail").ParentColumns(0).ColumnName
			Return New DataView(detail, String.Format("[{0}] = '{1}'", columnName, masterRowKey), String.Empty, DataViewRowState.CurrentRows)
		Else
			Return Nothing
		End If
	End Function

	Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
		If (Not IsPostBack) Then
			DataSessionKey = "CategoryProduct"
			gvMaster.DataBind()
		End If
	End Sub

	Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		Dim dataKey As String
		If (rbCatProd.Checked) Then
			dataKey = "CategoryProduct"
		Else
			dataKey = "CustomerOrder"
		End If
		If dataKey <> DataSessionKey Then
			DataSessionKey = dataKey
			gvMaster.DataBind()
		End If
	End Sub

	Protected Sub gvMaster_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
		Dim gridView As ASPxGridView = TryCast(sender, ASPxGridView)

		gridView.Columns.Clear()
		gridView.AutoGenerateColumns = True
		Dim data As DataTable = GetMasterData()
		gridView.KeyFieldName = data.PrimaryKey(0).ColumnName
		gridView.DataSource = data
	End Sub

	Protected Sub gvDetail_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
		Dim gridView As ASPxGridView = TryCast(sender, ASPxGridView)

		Dim dataView As DataView = GetDetailData(gridView.GetMasterRowKeyValue())
		gridView.KeyFieldName = dataView.Table.PrimaryKey(0).ColumnName
		gridView.DataSource = dataView
	End Sub
	Protected Sub gvDetail_DataBound(ByVal sender As Object, ByVal e As EventArgs)
		Dim gridView As ASPxGridView = TryCast(sender, ASPxGridView)
		Dim container As GridViewDetailRowTemplateContainer = TryCast(gridView.NamingContainer, GridViewDetailRowTemplateContainer)

		gridView.Columns(container.Grid.KeyFieldName).Visible = False
	End Sub
End Class