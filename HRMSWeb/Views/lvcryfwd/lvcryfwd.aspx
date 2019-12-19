<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="lvcryfwd.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.lvcryfwd.lvcryfwd" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var lvcryfwdConfig=Ext.decode('<%=ViewData["config"] %>'); 
lvcryfwdConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var lvcryfwdPageName = 'lvcryfwd';
var lvcryfwdKeyColumn='emno';
            
var lvcryfwdQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	lvcryfwdQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(lvcryfwdQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Leave_Label_year,
                        name: 'year',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leave_Label_ltcd ,
                        name: 'ltcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getLeaveType',
		                    listeners:{
                                load:function(){f = this.form.findField('ltcd');f.setValue(f.getValue());},
                                scope:this}})
    		          }]},

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

var carryforwardDateWindow=function(){
	this.init();	
	
	carryforwardDateWindow.superclass.constructor.call(this,{
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

Ext.extend(carryforwardDateWindow,Ext.Window,{
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
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Leave_Label_year + '' + HRMSRes.Public_Label_From,id:'from|year',
                        name:'from|year',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.YEARONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Leave_Label_year + '' + HRMSRes.Public_Label_To,id:'to|year',
                        name:'to|year',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.YEARONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]}

            ] 
       })
	},	
	
	Confirm:function(){	    
	    var params=[];	 
	           
        params[params.length]={ColumnName:'from|year',ColumnValue:this.form.findField('from|year').getValue()};                  
        params[params.length]={ColumnName:'to|year',ColumnValue:this.form.findField('to|year').getValue()};                  

        if ((params[0]=='') || (params[1]=='')) {
   		    Ext.MessageBox.show({
	            title: HRMSRes.Public_Message_Error,
	            msg:HRMSRes.Public_Message_NoCarryYear,
	            buttons: Ext.MessageBox.OK,
	            icon:Ext.MessageBox.ERROR
            });
        }
        
        carryforwardrange=params;
		this.close();
	},
	cleanQuery:function(){
	    cleanQueryCriterias(this.form);
	},
	scope:this
});

var carryforwardStatusWindow=function(grid){
    this.grid = grid;
	this.init();	
	
	carryforwardStatusWindow.superclass.constructor.call(this,{
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
            	this.carryforward();
            }
        },
        scope:this    
	});
}

Ext.extend(carryforwardStatusWindow,Ext.Window,{
    init:function(){
		this.formPanel=this.createFormPanel();
		//this.form=this.formPanel.getForm();
	},
	
	createFormPanel:function(){	        
		return new Ext.Panel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         html:'<div class=\"loading-indicator\"><img src=\"/ExtJS/ext/resources/images/default/grid/loading.gif" width=\"16\" height=\"16\"  style=\"margin-right: 8px;\" /><font color=\'red\'>' + HRMSRes.Public_Message_Carryforwarding + '</font></div>'
       })
	},
	carryforward:function(){
		this.grid.getBottomToolbar().diplayMsg.update('');       
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + lvcryfwdPageName + '.mvc/carryforward',
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
		   params: {record:Ext.encode({carryforwardparams:carryforwardrange,personalparams:empAdvQryResult})}
		});	    
	},
	scope:this
});

var lvcryfwdEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	
                      
	lvcryfwdEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:350, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.basisFormPanel,
        listeners:{
            show:function(){              
		        var keyField = this.basisForm.findField(lvcryfwdKeyColumn);

                if(!this.isNew){	
			        var data=this.grid.getSelectionModel().getSelected();
                    this.basisForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            if (f.xtype=='timefield') value = formatTime(value);
                            if (f.xtype=='datefield') value = formatDateNoTime(value);
                            f.setValue(value);                    	            
                        }
                    });   
		        }
		        var keyValue = keyField.getValue();
                setLastModifiedInfo(lvcryfwdConfig,this.basisForm);
            },
            scope:this
        },
        buttons: [{ 
            text:HRMSRes.Public_Button_Confirm, 
            iconCls:'icon-save', 
            handler: this.save,
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

Ext.extend(lvcryfwdEditWindow,Ext.Window,{
    init:function(){
		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();
	},
	createBasisFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
     		 items: [
      		    {
      		    layout:'column',
      		    items:[
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Public_Label_staff + '(<font color=red>*</font>)',
                        name: 'emno',stateful:false,disabled:!this.isNew,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'sfnm',valueField:'emno',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['sfnm','emno']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/ListAllPersonal'}),
                            listeners:{}
    		          }]},
      		        
       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Leave_Label_year + '(<font color=red>*</font>)',disabled:!this.isNew,allowBlank:false,
                        name: 'year',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leave_Lable_ltnm + '(<font color=red>*</font>)' ,disabled:!this.isNew,allowBlank:false,
                        name: 'ltcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getLeaveType',
		                    listeners:{load: function(){var v = this.basisForm.findField('ltcd').getValue();this.basisForm.findField('ltcd').setValue(v);},scope:this
		                    }})
    		          }]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Leave_Label_hors,disabled:false,
                        name: 'hors',allowBlank:false,stateful:false,anchor:'95%',decimalPrecision:1,keepZero:true}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Leave_Label_days,disabled:false,
                        name: 'days',stateful:false,anchor:'95%',decimalPrecision:1,keepZero:true }]},
                        
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:'Hours Consumed',disabled:true,
                        name: 'cnsu',stateful:false,anchor:'95%',decimalPrecision:1,keepZero:true}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:'Days Entitlement',disabled:true,
                        name: 'enti',stateful:false,anchor:'95%',decimalPrecision:1,keepZero:true}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:'Hours Limitation',disabled:true,
                        name: 'limt',stateful:false,anchor:'95%',decimalPrecision:1,keepZero:true}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Leave_Label_daro,id:'daro',
                        name:'daro',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,disabled:true,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_lmtm,id:'lmtm',
                        name:'lmtm',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATETIME,minValue: '1980/01/01',stateful:false,disabled:true,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_lmur,disabled:true,
                        name: 'lmur',stateful:false,anchor:'95%'}]}

      		    ]
      		 }] 
       })
	},
	save: function(){
		if(!this.basisForm.isValid()) return;
		this.grid.getBottomToolbar().diplayMsg.update('');
		var params={};
		this.basisForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });
        var keyparams=[];
        keyparams[0]={ColumnName:'emno',ColumnValue:this.basisForm.findField('emno').getValue()};
        keyparams[1]={ColumnName:'year',ColumnValue:this.basisForm.findField('year').getValue()};
        keyparams[2]={ColumnName:'ltcd',ColumnValue:this.basisForm.findField('ltcd').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + lvcryfwdPageName + '.mvc/'+method,
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		   		
		   		if (o.status=='success'){
		   		    var store=this.grid.store;		   		    
		   		    if(!this.isNew){
		   		        var sm=this.grid.getSelectionModel();
    	   		        var record=sm.getSelected();
		   		        for(var p in params){
		   		            record.set(p,params[p]);
		   		        }
		   		    }else{
		   		        //store.insert(0,new store.recordType(params));
		   		        //this.grid.store.totalLength+=1;
				        //this.grid.getBottomToolbar().updateInfo();
		   		    }
		   		    Ext.MessageBox.show({
			            title: HRMSRes.Public_Message_EditWell,
			            msg:o.msg,
			            buttons: Ext.MessageBox.OK,
			            icon:Ext.MessageBox.INFO
		            });		   		    
		   		    this.close();
		   		}else{
		   		    Ext.MessageBox.show({
			            title: HRMSRes.Public_Message_Error,
			            msg:o.msg,
			            buttons: Ext.MessageBox.OK,
			            icon:Ext.MessageBox.ERROR
		            });
		   		}
		   		this.grid.getBottomToolbar().diplayMsg.update(o.msg);	   		
		   },
		   scope:this,
		   params: {record:Ext.encode({params:params,keycolumns:keyparams})}
		});
	}
});


var lvcryfwdPanel=function(){
    this.tabId=lvcryfwdConfig.tabId;
	this.init();	
	
	var carryforwardScopeMenu = new Ext.menu.Menu({
    id: 'carryforwardScopeMenu',
    items: [
        {
            text: HRMSRes.Public_Message_CDR,
            handler: function() {new carryforwardDateWindow().show();}
        },'-',
        {
            text: HRMSRes.Public_Message_EF,
            handler: function() {new empAdvQryQueryWindow(this.grid).show();}
        }        
        ]
    });
	
	lvcryfwdPanel.superclass.constructor.call(this,{
		applyTo:this.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	autoScroll:true,      
	        tbar: [{ 
                text:HRMSRes.Public_Message_CS,
                iconCls: 'icon-query',  
                menu: carryforwardScopeMenu  
	        },'-',{ 
	        	id:this.tabId+ '_carryforward',
	            iconCls:'icon-add', 
	            text: HRMSRes.Public_Message_DCF, 
	            hidden:lvcryfwdConfig.auth[this.tabId+'_' + 'carryforward']!='True',
	            handler: function(){
	                Ext.MessageBox.show({
	                title:HRMSRes.Public_Confirm_Title,
	                msg:HRMSRes.Public_Message_AYSCTC,
	                buttons: Ext.Msg.YESNO,
	                icon:Ext.MessageBox.QUESTION,
	                fn: function(btn, text){                
		                if (btn=='yes'){
                            new carryforwardStatusWindow(this.grid).show(); 
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
	            hidden:lvcryfwdConfig.auth[this.tabId+'_' + 'edit']!='True',
	            handler: function(){
	            	new lvcryfwdEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:lvcryfwdConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:lvcryfwdConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new lvcryfwdQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:lvcryfwdConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
	            handler: this.exportExcel, 
	            scope: this 
	        },'->',{ 
                id:this.tabId+'_logview',
                iconCls:'icon-logview', 
                text:'', 
                handler: function(){
                        var data=this.grid.getSelectionModel().getSelected();if (data==null) return;var rfid = data.get('rfid');new logViewerWindow(rfid).show();},
                scope: this 
            },{ 
                id:this.tabId+'_muf',
                iconCls:lvcryfwdConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,lvcryfwdConfig.muf?'delete':'add',lvcryfwdConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(lvcryfwdPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var lvcryfwdStoreType=Ext.data.Record.create([
		    {name:'emno'},
            {name:'sfid'},
            {name:'stfn'},
            {name:'year'},
            {name:'ltcd'},
            {name:'ltnm'},
            {name:'hors'},
            {name:'days'},
            {name:'hrcs'},
            {name:'daro'},
            {name:'lmtm'},
            {name:'lmur'},
            {name:'cnsu'},
            {name:'enti'},
            {name:'limt'},
            {name:'rfid'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},lvcryfwdStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:lvcryfwdKeyColumn,ColumnValue:lvcryfwdConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + lvcryfwdPageName + '.mvc/list',
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
		        //forceFit: true 
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

                {header:HRMSRes.Leave_Label_year,sortable: true, dataIndex: 'year'},
                {header:HRMSRes.Leave_Label_ltnm,sortable: true, dataIndex: 'ltnm'},
                {header:HRMSRes.Leave_Label_hors,sortable: true, dataIndex: 'hors'},
                {header:HRMSRes.Leave_Label_days,sortable: true, dataIndex: 'days'},
                {header:'Hours Consumed',sortable: true, dataIndex: 'cnsu'},
                {header:'Days Entitlement',sortable: true, dataIndex: 'enti'},
                {header:'Hours Limitation',sortable: true, dataIndex: 'limt'},

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
	
	remove:function(){
	    var sm=this.grid.getSelectionModel();
	    var record=sm.getSelected();
        
		var keyparams=[];
        keyparams[0]={ColumnName:'emno',ColumnValue:record.get('emno')};
        keyparams[1]={ColumnName:'year',ColumnValue:record.get('year')};
        keyparams[2]={ColumnName:'ltcd',ColumnValue:record.get('ltcd')};
        
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ lvcryfwdPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:lvcryfwdKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + lvcryfwdPageName + '.mvc/exportexcel';
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
    new lvcryfwdPanel();
    
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
    this.carryforwardrange=params;

    this.empAdvQryResult = [];
    
})

    </script>

</body>
</html>