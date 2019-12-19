<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="otttlv.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.otttlv.otttlv" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var otttlvConfig=Ext.decode('<%=ViewData["config"] %>'); 
otttlvConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var otttlvPageName = 'otttlv';
var otttlvKeyColumn='emno';
            
var otttlvQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	otttlvQueryWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:400, 
        height:300, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.formPanel,
        title:HRMSRes.Public_Query_WindowTitle,
        listeners:{
            show:function(){
                var queryParams=this.grid.queryParams;
		        if(queryParams){
		            queryParams=queryParams.params;
		            for(var i=0;i<queryParams.length;i++){
		                var queryParam=queryParams[i];
		                var fieldName=queryParam.ColumnName;
		                var value=queryParam.ColumnValue;
		                var field=this.form.findField(fieldName);
		                if(field){
		                    field.setValue(value);
		                }
		            }
		        }
            }
        },
        buttons: [{ 
            text:HRMSRes.Public_Toolbar_Query, 
            iconCls:'icon-query', 
            handler: this.Query,
            scope: this
        },{ 
            text:HRMSRes.Public_Toolbar_CleanQuery, 
            iconCls:'icon-dashboard', 
            handler: this.cleanQuery,
            scope: this
        },{ 
            text: HRMSRes.Public_Button_Close, 
            iconCls: 'icon-exit', 
            handler: function(){                  
                this.close();
            },
            scope:this
        }]
	})
}

Ext.extend(otttlvQueryWindow,Ext.Window,{
    init:function(){
		this.formPanel=this.createFormPanel();
		this.form=this.formPanel.getForm();
	},
	
	createFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
             items: [
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sfid,
                        name: 'sfid',stateful:false,anchor:'95%',UpperOnBlur:'true'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sftn,
                        name: 'stfn',stateful:false,anchor:'95%'}]}

            ] 
       })
	},	
	
	Query:function(){	    
	    this.grid.getBottomToolbar().diplayMsg.update('');
	    var params=[];	        
        this.form.items.each(function(f){
            if(f.isFormField){
                var p={
                    ColumnName:f.getName(),
                    ColumnValue:f.getValue()                
                };
                params[params.length]=p;                  
            }
        });
        var loadParams={start:0,limit:Pagination.pagingSize};
        /***modified for adjquery**/
        this.grid.queryParams={
            params:params
        };
        this.grid.store.baseParams={record:Ext.util.JSON.encode(this.grid.queryParams)};
		this.store.load({params:loadParams});
		this.close();
	},
	cleanQuery:function(){
	    cleanQueryCriterias(this.form);
	},
	scope:this
});

var otttlvDateWindow=function(){
	this.init();	
	
	otttlvDateWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:400, 
        height:270, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.formPanel,
        title:HRMSRes.Public_Query_WindowTitle,
        buttons: [{ 
            text:HRMSRes.Public_Toolbar_Confirm, 
            iconCls:'icon-query', 
            handler: this.Confirm,
            scope: this
        },{ 
            text:HRMSRes.Public_Toolbar_CleanQuery, 
            iconCls:'icon-dashboard', 
            handler: this.cleanQuery,
            scope: this
        },{ 
            text: HRMSRes.Public_Button_Close, 
            iconCls: 'icon-exit', 
            handler: function(){                  
                this.close();
            },
            scope:this
        }]
	})
}

Ext.extend(otttlvDateWindow,Ext.Window,{
    init:function(){
		this.formPanel=this.createFormPanel();
		this.form=this.formPanel.getForm();
	},
	
	createFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
             items: [
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Message_trfr,id:'from|date',
                        name:'from|date',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Message_trto,id:'to|date',
                        name:'to|date',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]}

            ] 
       })
	},	
	
	Confirm:function(){	    
	    var params=[];	        
        this.form.items.each(function(f){
            if(f.isFormField){
                var p={
                    ColumnName:f.getName(),
                    ColumnValue:f.getValue()                
                };
                params[params.length]=p;                  
            }
        });
        otttlvrange=params;
		this.close();
	},
	cleanQuery:function(){
	    cleanQueryCriterias(this.form);
	},
	scope:this
});

var otttlvStatusWindow=function(grid){
    this.grid = grid;
	this.init();	
	
	otttlvStatusWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:400, 
        height:80, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.formPanel,
        title:HRMSRes.Public_Message_calcwin,
        listeners:{
            show:function(){
            	this.otttlv();
            }
        },
        scope:this    
	});
}

Ext.extend(otttlvStatusWindow,Ext.Window,{
    init:function(){
		this.formPanel=this.createFormPanel();
		//this.form=this.formPanel.getForm();
	},
	
	createFormPanel:function(){	        
		return new Ext.Panel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         html:'<div class=\"loading-indicator\"><img src=\"/ExtJS/ext/resources/images/default/grid/loading.gif" width=\"16\" height=\"16\"  style=\"margin-right: 8px;\" /><font color=\'red\'>' + HRMSRes.Public_Message_transfering + '</font></div>'
       })
	},
	otttlv:function(){
		this.grid.getBottomToolbar().diplayMsg.update('');       
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + otttlvPageName + '.mvc/transfer',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		   		

		   		this.grid.getBottomToolbar().diplayMsg.update(o.msg);	   		

		   		if (o.status=='success'){
		   		    this.close();
//		   		    this.query("Abnormal");
		   		}else{
		   		    Ext.MessageBox.show({
			            title: HRMSRes.Public_Message_Error,
			            msg:o.msg,
			            buttons: Ext.MessageBox.OK,
			            icon:Ext.MessageBox.ERROR
		            });
		   		}
		   },
		   scope:this,
		   params: {record:Ext.encode({otttlvparams:otttlvrange,personalparams:empAdvQryResult})}
		});	    
	},
	scope:this
});

var otttlvPanel=function(){
    this.tabId=otttlvConfig.tabId;
	this.init();	
	
	var otttlvScopeMenu = new Ext.menu.Menu({
    id: 'otttlvScopeMenu',
    items: [
        {
            text: HRMSRes.Public_Message_TDR,
            handler: function() {new otttlvDateWindow().show();}
        },'-',
        {
            text: HRMSRes.Public_Message_EF,
            handler: function() {new empAdvQryQueryWindow(this.grid).show();}
        }        
        ]
    });
	
	otttlvPanel.superclass.constructor.call(this,{
		applyTo:this.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	autoScroll:true,      
	        tbar: [{ 
                text:HRMSRes.Public_Message_TS,
                iconCls: 'icon-query',  
                menu: otttlvScopeMenu  
	        },'-',{ 
	        	id:this.tabId+ '_transfer',
	            iconCls:'icon-add', 
	            text: HRMSRes.Public_Message_DOTR, 
	            hidden:otttlvConfig.auth[this.tabId+'_' + 'transfer']!='True',
	            handler: function(){
	                Ext.MessageBox.show({
	                title:HRMSRes.Public_Confirm_Title,
	                msg:HRMSRes.Public_Message_AYSTCTTO,
	                buttons: Ext.Msg.YESNO,
	                icon:Ext.MessageBox.QUESTION,
	                fn: function(btn, text){                
		                if (btn=='yes'){
                            new otttlvStatusWindow(this.grid).show(); 
		                }
	                },
	                icon:Ext.MessageBox.QUESTION,
	                scope:this});
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden:otttlvConfig.auth[this.tabId+'_' + 'edit']!='True',
	            handler: function(){
	            	new otttlvEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:otttlvConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:otttlvConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new otttlvQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:otttlvConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
	            handler: this.exportExcel, 
	            scope: this 
	        },'->',{ 
	        	xtype:'label', 
	            html:  '<b><font color=green>employee no: ' + otttlvConfig.emno + '</font></b>',
	            hidden: otttlvConfig.emno==''
	        },'->',{ 
                id:this.tabId+'_logview',
                iconCls:'icon-logview', 
                text:'', 
                handler: function(){
                        var data=this.grid.getSelectionModel().getSelected();if (data==null) return;var rfid = data.get('rfid');new logViewerWindow(rfid).show();},
                scope: this 
            },{ 
                id:this.tabId+'_muf',
                iconCls:otttlvConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,otttlvConfig.muf?'delete':'add',otttlvConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(otttlvPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var otttlvStoreType=Ext.data.Record.create([
		    {name:'emno'},
            {name:'sfid'},
            {name:'stfn'},
            {name:'sqno'},
            {name:'ltcd'},
            {name:'ltnm'},
            {name:'days'},
            {name:'exdt'},
            {name:'remk'},
            {name:'lmtm'},
            {name:'lmur'}
            
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},otttlvStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:otttlvKeyColumn,ColumnValue:otttlvConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + otttlvPageName + '.mvc/list',
	   		listeners:{
                loadexception:function(o,t,response)
                {
                    var o= Ext.util.JSON.decode(response.responseText);		   		
                    this.grid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                load:function(){
                    this.controlButton(this.tabId);
                },
                scope:this
            }
        });

        var params={
            start:0,
            limit:Pagination.pagingSize
        };
        
        //store.load({params:params});
        
        return new Ext.grid.GridPanel({
    		border:true, 
    		monitorResize:true, 
            loadMask:true,  		            
            ds: store, 
            viewConfig: { 
		        forceFit: true 
		    }, 
            listeners:{
	            rowclick:function(){
	                this.controlButton(this.tabId);
	            },
	            render:function(){
            		this.controlButton(this.tabId);
	            },
	            scope:this
            },             
            
            cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Public_Label_sfid,sortable: true, dataIndex: 'sfid',hidden:false},
                {header:HRMSRes.Public_Label_sftn,sortable: true, dataIndex: 'stfn',hidden:false},

                {header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno'},
                {header:HRMSRes.Leave_Label_ltnm,sortable: true, dataIndex: 'ltnm'},
                {header:HRMSRes.Overtime_Label_days,sortable: true, dataIndex: 'days'},
                {header:HRMSRes.Public_Label_exdt,sortable: true, dataIndex: 'exdt',renderer:formatDateNoTime},
                {header:HRMSRes.Public_Label_remk,sortable: true, dataIndex: 'remk'},

                {header:HRMSRes.Public_Label_lmtm,sortable: true, dataIndex: 'lmtm',renderer:formatDate},
                {header:HRMSRes.Public_Label_lmur,sortable: true, dataIndex: 'lmur'}
            ]),
            bbar: new Ext.PagingToolbar({
                pageSize:Pagination.pagingSize,
                store: store,
                displayInfo: true,
                displayMsg:HRMSRes.Public_PagingToolbar_Total+':{1}/{2}',
                emptyMsg: HRMSRes.Public_PagingToolbar_EmptyMsg                    
            })
	    })
	},
	
	controlButton:function(id){
        var enabled=!this.grid.getSelectionModel().hasSelection();	    
        Ext.getCmp(id+ '_edit').setDisabled(enabled);
        Ext.getCmp(id+ '_delete').setDisabled(enabled);
    },
            
	exportExcel:function(){
	    if (this.grid.getStore().getTotalCount()<=0){
	        this.grid.getBottomToolbar().diplayMsg.update('<font color=red>' + HRMSRes.Public_Message_ExcelNoRecord + '</font>');
	        return;
	    }
	    
	    var cm=this.grid.getColumnModel();
	    var header=[];	    
	    
	    for(var i=0;i<cm.config.length;i++){
	        if(!cm.isHidden(i)){
	            var cname=cm.config[i].header;	            
	            var mapping=cm.config[i].dataIndex;
	            header[header.length]={
	                ColumnDisplayName:cname,
	                ColumnName:mapping
	            };
	        }          
	    }
	    var params={record:Ext.encode({params:[{ColumnName:otttlvKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + otttlvPageName + '.mvc/exportexcel';
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
})


Ext.onReady(function(){ 
//    var cp = new Ext.state.CookieProvider({
//       expires: new Date(new Date().getTime()+(1000*60*60*24*10*365))
//    });
//    Ext.state.Manager.setProvider(cp);
    new otttlvPanel();
    
    var params=[];	        
    var dd = formatDateNoTime(new Date());
    
    var p1={
        ColumnName:'from|year',
        ColumnValue:formatDate(dd)
    };
    
    var p2={
        ColumnName:'to|year',
        ColumnValue:formatDate(dd)                
    };
    
    params[0] = p1;
    params[1] = p2;
    this.otttlvrange=params;

    this.empAdvQryResult = [];
    
})

    </script>

</body>
</html>