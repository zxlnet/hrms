<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rphctctl.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.rphctctl.rphctctl" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var rphctctlPanel=function(rphctctlConfig){
	var iframeURL ="<iframe id='" + rphctctlConfig.tabId  + "_inner' src='#' marginwidth='0' marginheight='0' frameborder='0' scrolling='Yes' width='100%' height='600px'></iframe>";
    this.tabId=rphctctlConfig.tabId;
    
    rphctctlPanel.superclass.constructor.call(this,{
		applyTo:rphctctlConfig.tabId, 
	    layout:'fit',
	    id: rphctctlConfig.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	height:300,
	    	id: rphctctlConfig.tabId+'_contentpanel',
	    	//layout:'fit',  
	    	html:iframeURL,
	        tbar: [{ 
                    xtype:'combo', 
                    id: 'cmbRpHctCtlHeadCountCfg',
                    //hidden:psemplymConfig.auth[this.tabId+'_' + 'change']!='True',
                    store: new Ext.data.Store({ 
                                reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
	                                fields: ['ValueField','DisplayField']}), autoLoad:true,
                                url:ContextInfo.contextPath+'/dropdown.mvc/getHeadCoungCfg'
                                }),
                    displayField: 'DisplayField',valueField: 'ValueField',stateful:false,typeAhead: true,mode: 'local',
                    editable:false,triggerAction: 'all', anchor:'95%',emptyText:'Choose a headcount config',
                    width:200,
                    listeners:{
                        select:function(p){
                            this.hccd = p.getValue();
                        	var src = ContextInfo.contextPath + "/Reporting/rphctctl.aspx?action=query&hccd=" + this.hccd ;
                            Ext.getDom(rphctctlConfig.tabId + '_inner').src=src;
			            },
			            scope:this
		            } 
                },{ 
    	            id:this.tabId+'_export',
                    iconCls:'icon-export', 
                    //hidden:this.config.auth[this.tabId+'_pshctvaldata_export']!='True',
                    text:HRMSRes.Public_Toolbar_OutExcel, 
                    handler: function(){
                        if (this.hccd==null) return;
                        if (this.hccd=='undefined') return;
                        
                    	var src = ContextInfo.contextPath + "/Reporting/rphctctl.aspx?action=export&hccd=" + this.hccd ;
                        Ext.getDom(rphctctlConfig.tabId + '_inner').src=src;
                    },
                    scope: this 
                },'->',
                { 
    	            id:this.tabId+'_muf',
                    iconCls:rphctctlConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                    text:'', 
                    handler: function(){updateMUF(this.tabId,rphctctlConfig.muf?'delete':'add',rphctctlConfig,this.grid);},
                    scope: this 
                }]
       }]
	});
}

Ext.extend(rphctctlPanel,Ext.Panel,{

})


Ext.onReady(function(){ 
    var rphctctlConfig=Ext.decode('<%=ViewData["config"] %>'); 
    rphctctlConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
    
    new rphctctlPanel(rphctctlConfig);
})

    </script>

</body>
</html>