<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.Error.Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Human Resource Management System</title>
    <link rel="stylesheet" type="text/css" href='<%=ViewData["contextPath"] %>/ExtJS/ext/resources/css/ext-all.css'/>
    <link rel="stylesheet" type="text/css" href='<%=ViewData["contextPath"] %>/ExtJS/resources/docs.css' />
    <link rel="stylesheet" type="text/css" href='<%=ViewData["contextPath"] %>/ExtJS/resources/style.css' />
</head>
<body>
    <form id="Form1" action="" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server" EnableScriptGlobalization="true">
        <Scripts>
            <asp:ScriptReference Assembly="GotWell.LanguageResources" Name="GotWell.LanguageResources.HRMSLanguage.js" />
            <asp:ScriptReference Path="~/ExtJS/ext/adapter/ext/ext-base.js" />
            <asp:ScriptReference Path="~/ExtJS/ext/ext-all.js" />
        </Scripts>
    </asp:ScriptManager>
    </form>
    <center>
        <table width="100%" height="70%">
            <tr valign="middle">
                <td align="center">
                    <div id="container">
                    </div>
                </td>
            </tr>
        </table>
    </center>

    <script type="text/javascript">
        var errorConfig=Ext.decode('<%=ViewData["config"] %>'); 
        //HRMSRes.Public_Message_SessionTimeOut
        Ext.onReady(function(){
            var p = new Ext.Panel({
                title: HRMSRes.Public_Message_Info,
                collapsible:false,
                renderTo: 'container',
                width:400,
                height:80,
                html: '<table height=\'40\'><tr valign=\'middle\'><td>'+ errorConfig.msg +'</td></tr></table>'
            });
        });
    </script>

</body>
</html>
