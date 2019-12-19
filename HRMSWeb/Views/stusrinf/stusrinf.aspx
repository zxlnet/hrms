<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="stusrinf.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.stusrinf.stusrinf" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
   <script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var stusrinfConfig=Ext.decode('<%=ViewData["config"] %>'); 
stusrinfConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');       

var stusrinfPanel=function(){
    this.tabId=stusrinfConfig.tabId;
	this.init();	
	
	stusrinfPanel.superclass.constructor.call(this,{
		applyTo:stusrinfConfig.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:this.formPanel
	})

	Ext.get(this.tabId + '_userinfo_div').load({
	    url: ContextInfo.contextPath + '/stusrinf.mvc/getUserInfo',
	    params: { record: '' }
	});
}

Ext.extend(stusrinfPanel, Ext.Panel, {
    init: function() {
        this.formPanel = this.createFormPanel();
        this.form = this.formPanel.getForm();
    },

    createFormPanel: function() {
        return new Ext.FormPanel({
            labelWidth: 100,
            frame: true,
            bodyStyle: 'padding:15px 15px 30px 30px',
            id: this.tabId + '_formPanel',
            defaults: { width: 550 },
            defaultType: 'textfield',
            items: [
                { xtype: 'label', id: this.tabId + '_errorMsg', stateful: false
                },
                { xtype: 'fieldset', title: HRMSRes.Public_Label_usrinfo, autoHeight: true, layout: 'column',
                items: [
                    { layout: 'form', columnWidth: 1,
                        items: [{ xtype: 'label', id: this.tabId + '_userinfo_div', stateful: false, style: '' }
                        ]
                    }
                    ]
            },
                { xtype: 'fieldset', title: HRMSRes.Public_Label_changepwd, autoHeight: true, layout: 'column', style: 'padding:15px 15px 15px 15px',
                    items: [
                    { layout: 'form', columnWidth: 1,
                        items: [{ xtype: 'label', id: this.tabId + '_ChangePwdMsg', stateful: false, style: '',
                            html: ContextInfo.sysCfg['ScSBAD'] == 'Y' ? '<font color=red>' + HRMSRes.Public_Message_nalcp + '</font>' : '<font color=green>' + HRMSRes.Public_Message_alcp + '</font>'}]
                        },
	                { layout: 'form', columnWidth: .7,
	                    items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Public_Label_oldpwd, name: 'opwd',
	                        disabled: ContextInfo.sysCfg['ScSBAD'] == 'Y' ? true : false, allowBlank: false,
	                        inputType: 'password', stateful: false, anchor: '98%'}]
	                    },

	                { layout: 'form', columnWidth: .7,
	                    items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Public_Label_newpwd, name: 'npwd',
	                        disabled: ContextInfo.sysCfg['ScSBAD'] == 'Y' ? true : false, allowBlank: false,
	                        inputType: 'password', stateful: false, anchor: '98%'}]
	                    },

	                { layout: 'form', columnWidth: .7,
	                    items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Public_Label_cfnpwd, name: 'rpwd',
	                        disabled: ContextInfo.sysCfg['ScSBAD'] == 'Y' ? true : false, allowBlank: false,
	                        inputType: 'password', stateful: false, anchor: '98%'}]
	                    },
	                 { layout: 'form', columnWidth: .7,
	                     items: [{ text: HRMSRes.Public_Button_Confirm, xtype: 'button',
	                         disabled: ContextInfo.sysCfg['ScSBAD'] == 'Y' ? true : false,
	                         id: this.tabId + '_confirmpwd', stateful: false, minWidth: 100, iconCls: 'icon-confirm', handler: this.ConfirmPwd,
	                         scope: this}]
	                     }
                ]
                    }
            ]
        });
    },
    ConfirmPwd: function(url) {
        if (!this.form.isValid()) return;

        if (this.form.findField('npwd').getValue() != this.form.findField('rpwd').getValue()) {
            Ext.MessageBox.show({
                title: HRMSRes.Public_Message_Error,
                msg: '[' + HRMSRes.Public_Label_newpwd + ' ' + HRMSRes.Public_Message_Notequal + ' ' + '[' + HRMSRes.Public_Label_oldpwd + ']',
                buttons: Ext.MessageBox.OK,
                icon: Ext.MessageBox.ERROR
            });
            return;
        }
        var params = this.form.getValues();
        params['opwd'] = this.form.findField('opwd').getValue();
        params['npwd'] = this.form.findField('npwd').getValue();

        var msgField = Ext.getDom(this.tabId + '_ChangePwdMsg');
        msgField.innerHTML = '';
        Ext.Ajax.request({
            url: ContextInfo.contextPath + '/stusrinf.mvc/changePassword',
            success: function(response) {
                var o = Ext.util.JSON.decode(response.responseText);
                if (o.status == 'success') {
                }
                msgField.innerHTML = '<font color=red>' + o.msg + '</font>';
            },
            scope: this,
            params: { record: Ext.util.JSON.encode(params) }
        });
    } 
}
)


Ext.onReady(function(){ 
    new stusrinfPanel();
})

</script>
</body>
</html>
