<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rphctctl.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Reporting.rphctctl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <style>
    .testCssStyle
        {
        color: #1f5ca9;
        background-color: #c5d4e6;
        table-layout: auto;
        border-collapse: separate;
        border-right: gray thin solid;
        border-top: gray thin solid;
        border-left: gray thin solid;
        border-bottom: gray thin solid;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="padding:10px;overflow:auto;width:auto">
        <asp:GridView ID="GridView1" runat="server" CellPadding="4" PageSize="1000"
            ForeColor="#333333" Font-Size="Small" onrowdatabound="GridView1_RowDataBound" 
            ShowFooter="False" GridLines="Both"
            Caption='<B>HeadCount Control Report</B>'
            CaptionAlign="Top" Font-Names="Arial"
            >
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
    
    </div>
    </form>
</body>
</html>
