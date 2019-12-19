<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rpexrpdp.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.rpexrpdp.rpexrpdp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;


 var rpexrpdpQueryWindow=function(rptdefConfig,tabId){
    //get config
    this.tabId=tabId;
    this.cm=new Ext.grid.ColumnModel(rptdefConfig.columns);
    this.rpcd = rptdefConfig.rpcd;
    
    if (this.cm.config.length<5)
        this.height=150;
    else
        this.height=250;
            
    this.init();
	
    rpexrpdpQueryWindow.superclass.constructor.call(this,{
        title: HRMSRes.Public_Query_WindowTitle, 
        layout:'fit', 
        width:400, 
        id:this.tabId+'_QueryWindow',
        height:this.height, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.formPanel,
        buttons: [{ 
            text:HRMSRes.Public_Toolbar_Query, 
            iconCls:'icon-query', 
            handler: this.query,
            scope: this
        },{ 
            text:HRMSRes.Public_Toolbar_CleanQuery, 
            iconCls:'icon-dashboard', 
            handler: this.cleanQuery,
            scope: this
        },{ 
            text: HRMSRes.Public_Button_Close, 
            iconCls: 'icon-exit', 
            handler: function(){ this.close();},
            scope:this
        }],
        listeners:{
	        show:function(){
	            //var queryParams=this.grid.queryParams;
//		        if(queryParams){
//		            for(var i=0;i<queryParams.length;i++){
//		                var queryParam=queryParams[i];
//		                var fieldName=queryParam.ColumnName;
//		                var value=queryParam.ColumnValue;
//		                var field=this.form.findField(fieldName);
//		                if(field){
//		                    field.setValue(value);
//		                }
//		            }
//		        }
	        },
	        scope:this
        }
    })
}

Ext.extend(rpexrpdpQueryWindow,Ext.Window,{
    init:function(){
        this.formPanel=this.createFormPanel();
        this.form=this.formPanel.getForm();
    },
	
    createFormPanel:function(){
        return new Ext.FormPanel({   
             frame:true, 
             header:true,
             labelWidth: 140,
	         items: { 
   		        layout:'form',
   		        items:this.createItems()
	        } 
       })
    },
	
    clone:function(obj){
        var o={};
        for(var p in obj){
            o[p]=obj[p];
        }
        return o;
    },
	
    createItems:function(){
        var cm=this.cm;
        var firstColumn=[];
        var secondColumn=[];
        var queryColumn=[];
	    
        for(var i=0;i<cm.config.length;i++){
            var config=cm.config[i];
            var type=config.type;
            var size=0;
            var obj=this.clone(config);
	        
            queryColumn[queryColumn.length]=obj;

//            if(type=='datetime'){	                   
//                queryColumn[queryColumn.length]=obj;	
//                var to=this.clone(obj);
//                obj['header']+='<br/>from';	
//                obj['dataIndex']='from#'+obj['dataIndex'];                        
//                to['dataIndex']='to#'+to['dataIndex']; 
//                to['header']+='<br/>to';         
//                queryColumn[queryColumn.length]=to;
//            }else if(type=='string' && size<100){
//                queryColumn[queryColumn.length]=obj;
//            }
        } 

        for(var i=0;i<queryColumn.length;i++){
            var config=queryColumn[i];	        
            var name=config.dataIndex;
            var labelValue=config.header;
            var type=config.type;
            var field;
            var fieldConfig={
                fieldLabel:labelValue,
                type:type,
                name:name,
                stateful:false,
                height:22,
                anchor:'95%',
                table:config.table
            };  
                                
            if(typeof type!='undefinded'){
                if(type=='datetime'){
                    fieldConfig['format']=DATE_FORMAT.DATEONLY; 
                    field=new Ext.form.DateField(fieldConfig);	                                                  
                }else if(type=='string'){
                    //fieldConfig['UpperOnBlur']=false;
                    field=new Ext.form.TextField(fieldConfig);
                } 
            }  
                                            
            firstColumn[firstColumn.length]=field;
            this.height+=25;

            //if(i%2==0){	   
            //    firstColumn[firstColumn.length]=field;
            //    this.height+=25;
            //}else{
            //    secondColumn[secondColumn.length]=field;
            //}
        }

        var first={
            columnWidth:1,
            layout: 'form',
            items:firstColumn
        };
	    
//        var second={
//            columnWidth:.5,
//            layout: 'form',
//            items:secondColumn
//        };
	    
        return [first];
    },
	
    query: function(){
        var param='';
        this.form.items.each(function(f){
            if(f.isFormField){
                if(f.getValue()){
                    param+=(param==''?'':'&') + f.getName() + '=' + f.getValue();
                }                	            
            }
        });
        
        rpexrpdpParam = param;
    	var src = ContextInfo.contextPath + "/Reporting/rpexrpdp.aspx?action=query&rpcd=" + this.rpcd + (param==''?'':'&') + param;
        Ext.getDom(this.tabId + '_inner').src=src;
        
        this.close();
    },
	cleanQuery:function(){
	    cleanQueryCriterias(this.form);
	},
    scope:this
});


var rpexrpdpPanel=function(rpexrpdpConfig){
	var iframeURL ="<iframe id='" + rpexrpdpConfig.tabId  + "_inner' src='#' marginwidth='0' marginheight='0' frameborder='0' scrolling='Yes' width='100%' height='100%'></iframe>";
    this.tabId=rpexrpdpConfig.tabId;
   
    rpexrpdpPanel.superclass.constructor.call(this,{
		applyTo:rpexrpdpConfig.tabId, 
	    layout:'fit',
	    id: rpexrpdpConfig.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	height:300,
	    	id: rpexrpdpConfig.tabId+'_contentpanel',
	    	//layout:'fit',  
	    	html:iframeURL,
	        tbar: [{ 
                        xtype:'combo', 
                        id: 'cmbrpexrpdpReportDef',
                        //hidden:psemplymConfig.auth[this.tabId+'_' + 'change']!='True',
                        store: new Ext.data.Store({ 
	                                reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
		                                fields: ['ValueField','DisplayField']}), autoLoad:true,
	                                url:ContextInfo.contextPath+'/dropdown.mvc/getExcelReportDef'
	                                }),
                        displayField: 'DisplayField',valueField: 'ValueField',stateful:false,typeAhead: true,mode: 'local',
                        editable:false,triggerAction: 'all', anchor:'95%',emptyText:'Choose a report to display',
                        width:200,
                        listeners:{
                            select:function(p){
                                //new psemplymEditWindow(this.grid,false,p.getValue()).show(); 
                                rpexrpdpRpcd = p.getValue();
                                this.loadReportDef(p.getValue());
    			            },
    			            scope:this
    		            } 
                    },'-',{ 
	        	        id:this.tabId+ '_query',
	                    iconCls:'icon-query', 
	                    text:HRMSRes.Public_Toolbar_Query,
	                    //hidden:pstraingConfig.auth[this.tabId+'_' + 'query']!='True',
	                    handler: function(){
                            new rpexrpdpQueryWindow(this.rptdefConfig,rpexrpdpConfig.tabId).show();
	                    }, 
	                    scope: this
	                },'-',
                    { 
    	            id:this.tabId+'_export',
                    iconCls:'icon-export', 
                    //hidden:this.config.auth[this.tabId+'_pshctvaldata_export']!='True',
                    text:HRMSRes.Public_Toolbar_OutExcel, 
                    handler: function(){
                        if (rpexrpdpRpcd==null) return;
                        if (rpexrpdpRpcd==undefined) return;
                        if (rpexrpdpRpcd=='') return;
                        
    	                var src = ContextInfo.contextPath + "/Reporting/rpexrpdp.aspx?action=export&rpcd=" + rpexrpdpRpcd + (rpexrpdpParam==''?'':'&') + rpexrpdpParam;
                        Ext.getDom(this.tabId + '_inner').src=src;
                    },
                    scope: this 
                },'->',
                { 
    	            id:this.tabId+'_muf',
                    iconCls:rpexrpdpConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                    text:'', 
                    handler: function(){updateMUF(this.tabId,rpexrpdpConfig.muf?'delete':'add',rpexrpdpConfig,null);},
                    scope: this 
                }]
       }]
	});
}

Ext.extend(rpexrpdpPanel,Ext.Panel,{
    rebuildQueryWindow:function(){     

    },
                
    loadReportDef:function(rpcd){
        Ext.Ajax.request({
	        url:ContextInfo.contextPath+ '/rpexrpdp.mvc/loadReportDef',
	        success: function(response){
		        var o= Ext.util.JSON.decode(response.responseText);
		        if(o.status=='success'){
		            this.rptdefConfig=Ext.decode(o.msg);
		            //new this.rebuildQueryWindow(this.rptdefConfig,this.rptdefConfig.tabId);
                }	 
	        },
	        failure: function(response){
		        Ext.MessageBox.show({
			        title:HRMSRes.Public_Message_Error,
			        msg:response.responseText,
			        buttons: Ext.MessageBox.OK,
			        icon:Ext.MessageBox.ERROR
		        });
            },	
            scope:this,	  		    
	        params:{record:rpcd}
        })
    }
})


Ext.onReady(function(){ 
    var rpexrpdpConfig=Ext.decode('<%=ViewData["config"] %>'); 
    rpexrpdpConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
    
    rpexrpdpParam='';
    rpexrpdpRpcd='';
    new rpexrpdpPanel(rpexrpdpConfig);
})

    </script>

</body>
</html>