Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data
Imports DevExpress.Web

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsPostBack Then
            DataProvider.DataSessionKey = "CategoryProduct"
            gvMaster.DataBind()
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Dim dataKey As String = If(rbCatProd.Checked, "CategoryProduct", "CustomerOrder")
        If dataKey <> DataProvider.DataSessionKey Then
            ResetMasterDataSource(dataKey)
        End If
    End Sub

    Protected Sub gvMaster_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
        Dim gridView As ASPxGridView = TryCast(sender, ASPxGridView)

        Dim data As DataTable = DataProvider.GetMasterData()
        gridView.KeyFieldName = data.PrimaryKey(0).ColumnName
        gridView.DataSource = data
    End Sub

    Protected Sub gvDetail_DataBinding(ByVal sender As Object, ByVal e As EventArgs)
        Dim gridView As ASPxGridView = TryCast(sender, ASPxGridView)

        Dim dataView As DataView = DataProvider.GetDetailData(gridView.GetMasterRowKeyValue())
        gridView.KeyFieldName = dataView.Table.PrimaryKey(0).ColumnName
        gridView.DataSource = dataView
    End Sub
    Protected Sub gvDetail_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        Dim gridView As ASPxGridView = TryCast(sender, ASPxGridView)
        Dim container As GridViewDetailRowTemplateContainer = TryCast(gridView.NamingContainer, GridViewDetailRowTemplateContainer)

        gridView.Columns(container.Grid.KeyFieldName).Visible = False
    End Sub

    Private Sub ResetMasterDataSource(ByVal dataKey As String)
        DataProvider.DataSessionKey = dataKey
        gvMaster.Columns.Clear()
        gvMaster.AutoGenerateColumns = True
        gvMaster.DataBind()
    End Sub


End Class