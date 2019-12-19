<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="logon.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.logon.logon" %>

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
        Ext.onReady(function(){
            var formPanel = new Ext.FormPanel({
                baseCls: 'x-plain',
                labelWidth: 120,
                //url:'save-form.php',
                defaultType: 'textfield',

                items: [{
                    fieldLabel: 'User Id',
                    name: 'logon_userid',
                    id:'logon_userid',
                    allowBlank:false,
                    anchor:'95%'  
                },{
                    fieldLabel: 'Password',
                    name: 'logon_password',
                    id:'logon_password',
                    inputType:'password',
                    allowBlank:false,
                    anchor: '95%'  
                },{
                    xtype:'label',
                    id:'logon_msg',
                    name: 'subject',
                    anchor: '95%'  
                }]
            });

            var window = new Ext.Window({
                title: 'Logon',
                width: 400,
                height:200,
                layout: 'fit',
                plain:true,
                bodyStyle:'padding:5px;',
                buttonAlign:'center',
                items: formPanel,

                buttons: [
                  {text: 'Logon',
                    xtype:'button',
                    id:'logon_logon',
                    stateful:false,
                    minWidth:100,
                    iconCls:'icon-export', 
                    handler:function(){
                            var msgField=Ext.getDom('logon_msg'); 
                            msgField.innerHTML='';
                            
                            this.form = formPanel.getForm();
                            
                            if(!this.form.isValid()) return;

                            msgField.innerHTML='<font color=green>'+'Processing, please wait.'+'</font>';
                            var params={};
                            params['urid']=this.form.findField('logon_userid').getValue();
                            params['pswd']=this.form.findField('logon_password').getValue();
                            var logonButton=Ext.getCmp('logon_logon');
                            logonButton.setDisabled(true);
                            Ext.Ajax.request({
                               url:'<%=ViewData["contextPath"] %>'+'/logon.mvc/logon',
                               success: function(response){
   		                            var o= Ext.util.JSON.decode(response.responseText);	
   		                            if (o.status=='success'){
   		                                msgField.innerHTML='<font color=green>'+o.msg + 'Redirecting to Homepage...'+'</font>';
                                        var form=document.createElement('form');
	                                    form.name='chinaForm';
	                                    form.method='post';
	                                    form.action= location.protocol+'//'+location.host+'<%=ViewData["contextPath"] %>' + '/Home.mvc/index';
	                                    document.body.appendChild(form);
	                                    form.submit();
	                                    document.body.removeChild(form);
   		                                
   		                            }
   		                            else{
   		                                msgField.innerHTML='<font color=red>'+o.msg+'</font>';
   		                                logonButton.setDisabled(false);
   		                            }
                               },
                               failure:function(){
                                    logonButton.setDisabled(false);
   		                            var o= Ext.util.JSON.decode(response.responseText);	
   		                            msgField.innerHTML='<font color=red>'+o.msg+'</font>';
                               },
                               scope:this,
                               params: {record:Ext.util.JSON.encode(params)}
                            }); 
                        },
                        scope:this
                },{
                    text: 'Exit',
                    xtype:'button',
                    id:'logon_exit',
                    stateful:false,
                    minWidth:100,
                    iconCls:'icon-export', 
                    handler:function(){
                        this.close();
                    },
                    scope:this
                }]
            });

            window.show();
                    
        });

    </script>

</body>
</html>
