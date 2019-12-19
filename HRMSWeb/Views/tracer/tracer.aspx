<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="tracer.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.tracer.tracer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var traceConfig='<%=ViewData["config"] %>';
traceConfig=Ext.decode(traceConfig);   
/// <summary>
/// Action for creating the panel
/// </summary>
/// <Remarks>
var TracePanel=function(){
    this.tabId=traceConfig.tabId;
    this.canExport=false;
	
	TracePanel.superclass.constructor.call(this,{
		applyTo:traceConfig.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	id:this.tabId+'_traceFitPanel',
	    	autoScroll:true,      
	        tbar: [{
	            xtype:'label',
                text:HRMSRes.Trace_Label_FileName           
	        },{
	            xtype:'textfield',
                id:this.tabId+'_tracefile',
                name: 'fileName',
                value:traceConfig.fileName,
                listeners:{
                    specialkey:function(field,e){
                        var keyCode=e.getKey();
                        if(keyCode==Ext.EventObject.ENTER){
                            this.showErrorData();                              
                        }
                    },
                    render:function(field){
                        if(field.value){
                            this.showErrorData();
                        }
                    },
                    scope:this
                },
                width:200            
	        },{ 
	        	id:this.tabId+'_trace',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query, 
	            handler:this.showErrorData,
	            scope: this 
	        },{ 
	        	id:this.tabId+'_traceFileList',
	            iconCls:'icon-save', 
	            text: HRMSRes.Public_Button_FileList, 
	            handler:this.getFileList,
	            scope: this 
	        },{ 
	        	id:this.tabId+'_trace_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            handler:this.exportExcel,
	            scope: this 
	        },{
	            xtype:'label',
                text:'',
                id:this.tabId+'_errorFileName'           
	        }]
       }]
	})
}

Ext.extend(TracePanel,Ext.Panel,{
    /// <summary>
    /// Action for creating the error grid
    /// </summary>
    /// <Remarks>
	showErrorData:function(){
	    var fileName=Ext.getCmp(this.tabId+'_tracefile').getValue().trim();
	    this.canExport=false;
	    if(fileName==''){
	        return;
	    }	    
	    var p=Ext.getCmp(this.tabId+'_traceFitPanel');
	    if(this.errorGrid){
	        p.remove(this.errorGrid,true);
	        delete this.errorGrid;
	    }
	    var url=ContextInfo.contextPath+'/tracer.mvc/showContent';
	    Ext.Ajax.request({
           url:url,
           success: function(response){
                var results=Ext.util.JSON.decode(response.responseText);
                var errorField=Ext.getDom(this.tabId+'_errorFileName');
                errorField.innerHTML='';
                if(typeof results.status=='undefined'){
                    this.canExport=true;
                    this.errorGrid=this.createErrorGrid(results,url); 
                    p.add(this.errorGrid);
                    p.doLayout();
                }else{                    
                    errorField.innerHTML=results.msg;
                }                  
           },
           scope:this,
           params: {fileName:fileName}
        }); 	    
	},
	
	/// <summary>
    /// Action for get all trace files
    /// </summary>
    /// <Remarks>
	getFileList:function(){
	    var url=ContextInfo.contextPath+'/tracer.mvc/getFileList';
	    this.canExport=false;
	    var p=Ext.getCmp(this.tabId+'_traceFitPanel');
	    if(this.errorGrid){
	        p.remove(this.errorGrid,true);
	        delete this.errorGrid;
	    }
	    Ext.Ajax.request({
           url:url,
           success: function(response){
                var results=Ext.util.JSON.decode(response.responseText);
                var errorField=Ext.getDom(this.tabId+'_errorFileName');
                errorField.innerHTML='';
                if(typeof results.status=='undefined'){
                    this.canExport=true;
                    this.errorGrid=this.createErrorGrid(results,url); 
                    p.add(this.errorGrid);
                    this.errorGrid.getSelectionModel().on('rowselect',function(sm,rowIndex,r){
                        var fileField=Ext.getCmp(this.tabId+'_tracefile');                        
                        fileField.setValue(r.get('fileName'));                                      
                    },this);
                    p.doLayout();
                }else{                    
                    errorField.innerHTML=results.msg;
                }                  
           },
           scope:this,
           params: {fileName:'fileName'}
        }); 
	},
	
	/// <summary>
    /// Action for exporting to excel
    /// </summary>
    /// <Remarks>
	exportExcel:function(){
	    if(!this.canExport){
	        var errorField=Ext.getDom(this.tabId+'_errorFileName');
	        if(errorField){
	            errorField.innerHTML='<font color=red>' + HRMSRes.Public_Message_ExcelNoRecord + '</font>';
	        }
	        return;
	    }
	    
	    if(this.errorGrid){
	        var params={};
	            var cm=this.errorGrid.getColumnModel();
	            var header=[];	    
        	    
	            for(var i=1;i<cm.config.length;i++){
	                if(!cm.isHidden(i)){
	                    var cname=cm.config[i].header;	            
	                    var mapping=cm.config[i].dataIndex;
	                    header[header.length]={
	                        ColumnDisplayName:cname,
	                        ColumnName:mapping
	                    };
	                }          
	            }
        	    
	            params.header=Ext.encode(header);
        	    
	            var form=document.createElement('form');
	            form.method='post';
	            form.action=ContextInfo.contextPath+ '/tracer.mvc/exportExcel';
	            for(var p in params){
	                var hd = document.createElement('input');
                    hd.type = 'hidden';
                    hd.name = p;
                    hd.value = params[p];
                    form.appendChild(hd);
	            }
	            document.body.appendChild(form);
	            form.submit();
	            document.body.removeChild(form);
	    }
	},
	
	createFieldsAndModels:function(result){           
        var cms=[];
        cms[cms.length]=new Ext.grid.RowNumberer();
        var fields=[];
        for(var p in result.rows[0]){
            cms[cms.length]={
                header:p,
                sortable:true,
                dataIndex:p
            };
            fields[fields.length]={name:p};
        }
        return {model:cms,fields:fields};
    },
    
    /// <summary>
    /// Action for creating the grid 
    /// </summary>
    /// <Remarks>
    createErrorGrid:function(result,url){   
        var fields=this.createFieldsAndModels(result);        
        var storeType=Ext.data.Record.create(fields.fields);
                        
        var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},storeType),
	   		url:url
        }); 
        
        var grid= new Ext.grid.GridPanel({
    		border:true, 
    		id:this.tabId+'_errorgridpanel',
    		monitorResize:true, 
            loadMask:true,  		            
            ds: store, 
            viewConfig: { 
		        autoFill : true 
		    },                          
            cm:new Ext.grid.ColumnModel(fields.model),
            bbar: new Ext.PagingToolbar({
                pageSize:Pagination.pagingSize,
                store: store,
                displayInfo: true,
                displayMsg:HRMSRes.Public_PagingToolbar_Total+':{1}/{2}',
                emptyMsg: HRMSRes.Public_PagingToolbar_EmptyMsg                    
            })
	    });
	    var params={
            start:0,
            limit:Pagination.pagingSize
        };
        store.load({params:params});
        return grid;
    }
})


Ext.onReady(function(){ 
//    var cp = new Ext.state.CookieProvider({
//       expires: new Date(new Date().getTime()+(1000*60*60*24*10*365))
//    });
//    Ext.state.Manager.setProvider(cp);
    new TracePanel();
})

</script>
</body>
</html>
