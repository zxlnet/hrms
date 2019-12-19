<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="atabssum.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.atabssum.atabssum" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var atabssumConfig=Ext.decode('<%=ViewData["config"] %>'); 
atabssumConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var atabssumPageName = 'atabssum';
var atabssumKeyColumn='emno';
            
var atabssumQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	atabssumQueryWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:400, 
        height:270, 
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

Ext.extend(atabssumQueryWindow,Ext.Window,{
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
                        name: 'stfn',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_lmur,
                        name: 'lmur',stateful:false,anchor:'95%'}]},
            
     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_lmtm + '' + HRMSRes.Public_Label_From,
                        name:'from|lmtm',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_lmtm + '' + HRMSRes.Public_Label_To,
                        name:'to|lmtm',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]}
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


var atabssumDateWindow=function(){
	this.init();	
	
	atabssumDateWindow.superclass.constructor.call(this,{
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

Ext.extend(atabssumDateWindow,Ext.Window,{
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
//      		        {columnWidth:.5,layout: 'form',
//                     items: [{xtype:'datefield',fieldLabel:'Period' + '' + HRMSRes.Public_Label_From,id:'from|year',
//                        name:'from|year',editable:false,height:22,anchor:'95%',
//                        format: DATE_FORMAT.YEARONLY,minValue: '1980/01/01',
//           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

//     		        {columnWidth:.5,layout: 'form',
//                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Leave_Label_year + '' + HRMSRes.Public_Label_To,id:'to|year',
//                        name:'to|year',editable:false,height:22,anchor:'95%',
//                        format: DATE_FORMAT.YEARONLY,minValue: '1980/01/01',
//           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]}
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_perd + '(<font color=red>*</font>)',
                        name: 'perd',stateful:false,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'perd',valueField:'perd',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['perd','perd']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/public.mvc/listValidPeriod',
		                    listeners:{
                                load:function(t,r){f = this.form.findField('perd');
                                        var v = f.getValue();f.setValue(v);
                                },scope:this}})
    		          }]}	        
            ] 
       })
	},	
	
	Confirm:function(){	    
	    var params=[];	 
	           
        //params[params.length]={ColumnName:'from|year',ColumnValue:this.form.findField('from|year').getValue()};                  
        //params[params.length]={ColumnName:'to|year',ColumnValue:this.form.findField('to|year').getValue()};                  
        params[params.length]={ColumnName:'perd',ColumnValue:this.form.findField('perd').getValue()};                  
        
        if ((params[0]=='') || (params[1]=='')) {
   		    Ext.MessageBox.show({
	            title: HRMSRes.Public_Message_Error,
	            msg:HRMSRes.Public_Message_NoCarryYear,
	            buttons: Ext.MessageBox.OK,
	            icon:Ext.MessageBox.ERROR
            });
        }
        
        atabssumrange=params;
		this.close();
	},
	cleanQuery:function(){
	    cleanQueryCriterias(this.form);
	},
	scope:this
});

var atabssumStatusWindow=function(grid){
    this.grid = grid;
	this.init();	
	
	atabssumStatusWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:400, 
        height:80, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.formPanel,
        title:'Calculation Window',
        listeners:{
            show:function(){
            	this.calculate();
            }
        },
        scope:this    
	});
}

Ext.extend(atabssumStatusWindow,Ext.Window,{
    init:function(){
		this.formPanel=this.createFormPanel();
		//this.form=this.formPanel.getForm();
	},
	
	createFormPanel:function(){	        
		return new Ext.Panel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         html:'<div class=\"loading-indicator\"><img src=\"/ExtJS/ext/resources/images/default/grid/loading.gif" width=\"16\" height=\"16\"  style=\"margin-right: 8px;\" /><font color=\'red\'>' + 'Calculating...' + '</font></div>'
       })
	},
	calculate:function(){
		this.grid.getBottomToolbar().diplayMsg.update('');       
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + atabssumPageName + '.mvc/calculate',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		   		

		   		this.grid.getBottomToolbar().diplayMsg.update(o.msg);	   		

		   		if (o.status=='success'){
		   		    this.close();
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
		   params: {record:Ext.encode({atabssumparams:atabssumrange})}
		});	    
	},
	scope:this
});


var atabssumPanel=function(){
    this.tabId=atabssumConfig.tabId;
	this.init();	
    
    var atabssumScopeMenu = new Ext.menu.Menu({
    id: 'atabssumScopeMenu',
    items: [
        {
            text: 'Date range',
            handler: function() {new atabssumDateWindow().show();}
        }
        
//        ,'-',
//        {
//            text: HRMSRes.Public_Message_EF,
//            handler: function() {new empAdvQryQueryWindow(this.grid).show();}
//        }        
        ]
    });

	atabssumPanel.superclass.constructor.call(this,{
		applyTo:this.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	autoScroll:true,      
	        tbar: [
//	        { 
//	            id:this.tabId+ '_delete',
//                iconCls:'icon-remove', 
//                hidden:atabssumConfig.auth[this.tabId+'_' + 'delete']!='True',
//                text:'Calculate', 
//                handler: this.calculate, 
//                scope: this 
//            }
            { 
                text:'Calculation Range',
                iconCls: 'icon-query',  
                menu: atabssumScopeMenu  
	        },'-',{ 
	        	id:this.tabId+ '_calculate',
	            iconCls:'icon-add', 
	            text: 'Calculate', 
	            //hidden:lvcryfwdConfig.auth[this.tabId+'_' + 'carryforward']!='True',
	            handler: function(){
	                Ext.MessageBox.show({
	                title:HRMSRes.Public_Confirm_Title,
	                msg:'Are you sure you want to calculate absence summary?',
	                buttons: Ext.Msg.YESNO,
	                icon:Ext.MessageBox.QUESTION,
	                fn: function(btn, text){                
		                if (btn=='yes'){
                            new atabssumStatusWindow(this.grid).show(); 
		                }
	                },
	                icon:Ext.MessageBox.QUESTION,
	                scope:this});
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:atabssumConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new atabssumQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:atabssumConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
	            handler: this.exportExcel, 
	            scope: this 
	        },'->',{ 
	        	xtype:'label', 
	            html:  '<b><font color=green>employee no: ' + atabssumConfig.emno + '</font></b>',
	            hidden: atabssumConfig.emno==''
	        },'->',{ 
                id:this.tabId+'_logview',
                iconCls:'icon-logview', 
                text:'', 
                handler: function(){
                        var data=this.grid.getSelectionModel().getSelected();if (data==null) return;var rfid = data.get('rfid');new logViewerWindow(rfid).show();},
                scope: this 
            },{ 
                id:this.tabId+'_muf',
                iconCls:atabssumConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,atabssumConfig.muf?'delete':'add',atabssumConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(atabssumPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var atabssumStoreType=Ext.data.Record.create([
		    {name:'emno'},
            {name:'sfid'},
            {name:'stfn'},
            
            {name:'lact'},
            {name:'abda'},
            {name:'abhr'},
            {name:'ahrr'},
            {name:'eact'},
            {name:'perd'},
            
            {name:'lmtm'},
            {name:'lmur'},
            {name:'rfid'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},atabssumStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:atabssumKeyColumn,ColumnValue:atabssumConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + atabssumPageName + '.mvc/list',
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
        
        store.load({params:params});
        
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
	            scope:this
            },                      
            cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Public_Label_sfid,sortable: true, dataIndex: 'sfid',hidden:false},
                {header:HRMSRes.Public_Label_sftn,sortable: true, dataIndex: 'stfn',hidden:false},

                {header:HRMSRes.Public_Label_perd,sortable: true, dataIndex: 'perd'},
                {header:HRMSRes.Attendance_Label_lctm,sortable: true, dataIndex: 'lact'},
                {header:HRMSRes.Attendance_Label_abda,sortable: true, dataIndex: 'abda'},
                {header:HRMSRes.Attendance_Label_abhr,sortable: true, dataIndex: 'abhr'},
                {header:HRMSRes.Attendance_Label_ahrr,sortable: true, dataIndex: 'ahrr'},
                {header:HRMSRes.Attendance_Label_ectm,sortable: true, dataIndex: 'eact'}
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
	calculate:function(){
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:'Are you sure you want to calculate absence summary?',
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ atabssumPageName + '.mvc/calculate',
	   			        success: function(response){
	   				        var o= Ext.util.JSON.decode(response.responseText);
	   				        if(o.status=='success'){
	   				            //this.grid.store.remove(record);
				                //this.grid.store.totalLength-=1;
				                //this.grid.getBottomToolbar().updateInfo();   					        	   					        		   					
			                }	 
			                this.grid.getBottomToolbar().diplayMsg.update(o.msg);  		   				
	   				        this.controlButton(this.tabId);
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
	   			        params:{record:Ext.encode({})}
	   		        })
		        }
	        },
	        icon:Ext.MessageBox.QUESTION,
	        scope:this
        });
	},	
	remove:function(){
	    var sm=this.grid.getSelectionModel();
	    var record=sm.getSelected();

		var keyparams=[];
        keyparams[0]={ColumnName:'emno',ColumnValue:record.get('emno')};

	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ atabssumPageName + '.mvc/delete',
	   			        success: function(response){
	   				        var o= Ext.util.JSON.decode(response.responseText);
	   				        if(o.status=='success'){
	   				            this.grid.store.remove(record);
				                this.grid.store.totalLength-=1;
				                this.grid.getBottomToolbar().updateInfo();   					        	   					        		   					
			                }	 
			                this.grid.getBottomToolbar().diplayMsg.update(o.msg);  		   				
	   				        this.controlButton(this.tabId);
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
	   			        params:{record:Ext.encode({params:keyparams})}
	   		        })
		        }
	        },
	        icon:Ext.MessageBox.QUESTION,
	        scope:this
        });
	},
	
	controlButton:function(id){
        var enabled=!this.grid.getSelectionModel().hasSelection();	    
        //Ext.getCmp(id+ '_edit').setDisabled(enabled);
        //Ext.getCmp(id+ '_delete').setDisabled(enabled);
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
	    var params={record:Ext.encode({params:[{ColumnName:atabssumKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + atabssumPageName + '.mvc/exportexcel';
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
    new atabssumPanel();
    
    this.atabssumrange=[];
})

    </script>

</body>
</html>
