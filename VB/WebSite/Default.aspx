<%@ Page Language="vb" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.v12.2, Version=12.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
	Namespace="DevExpress.Web.ASPxGridView" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v12.2, Version=12.2.5.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
	Namespace="DevExpress.Web.ASPxEditors" TagPrefix="dx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<dx:ASPxGridView ID="gvMaster" runat="server" AutoGenerateColumns="True" OnDataBinding="gvMaster_DataBinding">
			<Templates>
				<DetailRow>
					<dx:ASPxGridView ID="gvDetail" runat="server" AutoGenerateColumns="True" OnDataBinding="gvDetail_DataBinding"
						OnDataBound="gvDetail_DataBound">
					</dx:ASPxGridView>
				</DetailRow>
			</Templates>
			<SettingsDetail ShowDetailRow="true" />
		</dx:ASPxGridView>
		<dx:ASPxRadioButton ID="rbCatProd" runat="server" AutoPostBack="True" Checked="True"
			GroupName="1" Text="Categories-Products data">
		</dx:ASPxRadioButton>
		<dx:ASPxRadioButton ID="rbCustOrd" runat="server" AutoPostBack="True"
			GroupName="1" Text="Customers-Orders data">
		</dx:ASPxRadioButton>
	</div>
	</form>
</body>
</html>