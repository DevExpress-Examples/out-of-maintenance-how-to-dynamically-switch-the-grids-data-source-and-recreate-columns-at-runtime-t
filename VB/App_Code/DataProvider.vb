Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Linq
Imports System.Web

''' <summary>
''' Summary description for DataProvider
''' </summary>
Public Class DataProvider
    Public Shared Property DataSessionKey() As String
        Get
            Dim value As Object = HttpContext.Current.Session("DataSessionKey")
            Return If(value IsNot Nothing, DirectCast(value, String), "CategoryProduct")
        End Get
        Set(ByVal value As String)
            HttpContext.Current.Session("DataSessionKey") = value
        End Set
    End Property

    Public Shared Function GetMasterData() As DataTable
        Dim data As DataSet = GetData()
        If data IsNot Nothing Then
            Return data.Tables("Master")
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function GetDetailData(ByVal masterRowKey As Object) As DataView
        Dim data As DataSet = GetData()
        If data IsNot Nothing Then
            Dim detail As DataTable = data.Tables("Detail")
            Dim columnName As String = data.Relations("MasterDetail").ParentColumns(0).ColumnName
            Return New DataView(detail, String.Format("[{0}] = '{1}'", columnName, masterRowKey), String.Empty, DataViewRowState.CurrentRows)
        Else
            Return Nothing
        End If
    End Function

    Private Shared Function GetData() As DataSet
        Select Case DataSessionKey
            Case "CategoryProduct"
                Return GetCategoryProductData()
            Case "CustomerOrder"
                Return GetCustomerOrderData()
            Case Else
                Return Nothing
        End Select
    End Function

    Private Shared Function GetCategoryProductData() As DataSet
        Dim data As DataSet = DirectCast(HttpContext.Current.Session("CategoryProduct"), DataSet)
        If data Is Nothing Then
            data = RequestData("SELECT [CategoryID], [CategoryName], [Description] FROM [Categories]", "CategoryID", "SELECT [ProductID], [ProductName], [UnitPrice], [Discontinued], [CategoryID] FROM [Products]", "ProductID")
            HttpContext.Current.Session("CategoryProduct") = data
        End If
        Return data
    End Function

    Private Shared Function GetCustomerOrderData() As DataSet
        Dim data As DataSet = DirectCast(HttpContext.Current.Session("CustomerOrder"), DataSet)
        If data Is Nothing Then
            data = RequestData("SELECT [CustomerID], [CompanyName], [ContactTitle], [City], [Country] FROM [Customers]", "CustomerID", "SELECT [OrderID], [OrderDate], [ShipName], [ShipCity], [ShipCountry], [CustomerID] FROM [Orders]", "OrderID")

            HttpContext.Current.Session("CustomerOrder") = data
        End If
        Return data
    End Function

    Private Shared Function RequestData(ByVal masterSelectCommand As String, ByVal masterTableKeyField As String, ByVal detailSelectCommand As String, ByVal detailTableKeyField As String) As DataSet
        Dim masterData As DataTable = DataHelper.ProcessSelectCommand(masterSelectCommand)
        masterData.PrimaryKey = New DataColumn() { masterData.Columns(masterTableKeyField) }
        masterData.TableName = "Master"

        Dim detailData As DataTable = DataHelper.ProcessSelectCommand(detailSelectCommand)
        detailData.PrimaryKey = New DataColumn() { detailData.Columns(detailTableKeyField) }
        detailData.TableName = "Detail"

        Dim data As New DataSet()
        data.Tables.Add(masterData)
        data.Tables.Add(detailData)
        data.Relations.Add("MasterDetail", masterData.Columns(masterTableKeyField), detailData.Columns(masterTableKeyField))

        Return data
    End Function
End Class