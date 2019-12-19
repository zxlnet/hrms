<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="GotWell.HRMSWeb.Views.Home.Index"  Buffer="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Human Resource Management System</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    
    <link rel="stylesheet" type="text/css" href='<%=ViewData["contextPath"] %>/ExtJS/ext/resources/css/ext-all.css'/>
    <link rel="stylesheet" type="text/css" href='<%=ViewData["contextPath"] %>/ExtJS/resources/docs.css' />
    <link rel="stylesheet" type="text/css" href='<%=ViewData["contextPath"] %>/ExtJS/resources/style.css' />
</head>
<body id="docs">
    <div id="loading-mask" style="">
    </div>
    <div id="loading">
        <div class="loading-indicator">
            <img src="~/ExtJS/resources/extanim32.gif" width="32" height="32"  style="margin-right: 8px;" runat="server"
                alt="" />GotWell HRMS Loading...</div>
    </div>   
    
    <script type="text/javascript">  
        var period= '<%=ViewData["currentPeriod"] %>';
        period=eval('('+period+')');
        
        ContextInfo={
            contextPath:location.protocol+'//'+location.host+'<%=ViewData["contextPath"] %>',
            currentUser:'<%=ViewData["currentUser"] %>',
            currentPeriod:{
                fullPeriod:period.fullPeriod,
                start:period.start,
                end:period.end,
                year:period.year,
                period:period.period
            },
            currentEnvironment:'<%=ViewData["currentEnvironment"] %>'
        };
    </script>

    <form id="Form1" action="" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="true">
        <Scripts>            
            <asp:ScriptReference Assembly="GotWell.LanguageResources" Name="GotWell.LanguageResources.HRMSLanguage.js" />
            <asp:ScriptReference Path="~/ExtJS/ext/adapter/ext/ext-base.js" />
            <asp:ScriptReference Path="~/ExtJS/ext/ext-all.js" />
            <asp:ScriptReference Path="~/ExtJS/resources/TabCloseMenu.js" />
            <asp:ScriptReference Path="~/ExtJS/hrms-public.js" />
            <asp:ScriptReference Path="~/ExtJS/error-grid.js" />
            <asp:ScriptReference Path="~/ExtJS/employment-advqry.js" />
            <asp:ScriptReference Path="~/ExtJS/employment-advqry-nogrid.js" />
            <asp:ScriptReference Path="~/ExtJS/resources/docs.js" />
            <asp:ScriptReference Path="~/ExtJS/ext-util.js" />
            <asp:ScriptReference Path="~/ExtJS/expression-builder.js" />
            <asp:ScriptReference Path="~/ExtJS/log-viewer.js" />
            <asp:ScriptReference Path="~/ExtJS/html-text-viewer.js" />
            <asp:ScriptReference Path="~/ExtJS/check_idcard.js" />
            <asp:ScriptReference Path="~/ExtJS/row-expander.js" />
            
        </Scripts>
    </asp:ScriptManager>
    </form>

    <script type="text/javascript">            
        ContextInfo['sysCfg']=Ext.util.JSON.decode('<%=ViewData["sysCfg"] %>');
    </script>

    <div id="header">
        <div class="api-title">
            <img src="~/ExtJS/resources/logo.png" style="position: absolute;top: 1 in;left: 1 in;" runat="server" alt=""/>
        </div>
        
        <!--        <div class="api-title">
            <b>GotWell HRMS</b>
        </div> 
        -->
    </div>
    <div id="classes">
    </div>
    <div id="main">
    </div>
</body>
</html>
