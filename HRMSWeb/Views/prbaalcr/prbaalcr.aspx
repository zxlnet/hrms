﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="prbaalcr.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.prbaalcr.prbaalcr" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var prbaalcrConfig=Ext.decode('<%=ViewData["config"] %>'); 
prbaalcrConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var prbaalcrPageName = 'prbaalcr';
var prbaalcrKeyColumn='emno';
            
var prbaalcrQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	prbaalcrQueryWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:400, 
        height:310, 
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

Ext.extend(prbaalcrQueryWindow,Ext.Window,{
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

var prbaalcrEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	
                     
	prbaalcrEditWindow.superclass.constructor.call(this,{
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
		        var keyField = this.basisForm.findField(prbaalcrKeyColumn);

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
		        else
		        {
		            keyField.setValue(prbaalcrConfig.emno);
		            if (prbaalcrConfig.emno!='')
		                getMaxsqno(prbaalcrConfig.emno,prbaalcrConfig.tableName,this.basisForm);
		        }
		        var keyValue = keyField.getValue();
                setLastModifiedInfo(prbaalcrConfig,this.basisForm);
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

Ext.extend(prbaalcrEditWindow,Ext.Window,{
    init:function(){
		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();
	},
	createBasisFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
      		 items: [{
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
		                    url:ContextInfo.contextPath+'/dropdown.mvc/ListValidPersonal',
		                    listeners:{load:function(){f = this.basisForm.findField('emno');f.setValue(f.getValue());},scope:this}}),
                            listeners:{select:function(p){
                                var emno = p.getValue();
                                if (emno!=''){
                                    getMaxsqno(emno,prbaalcrConfig.tableName,this.basisForm);
                                    var store = this.basisForm.findField('acno').store;
                                    store.baseParams={record:Ext.encode({emno:emno})};
                                    store.load();
                                }
                            },scope:this}
    		          }]},
      		        
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sqno+'(<font color=red>*</font>)',
                        name: 'sqno',allowBlank:false,disabled:true,stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_acno +'(<font color=red>*</font>)',
                        name: 'acno',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), 
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getAccount',
		                    listeners:{load:function(){var f = this.basisForm.findField('acno');f.setValue(f.getValue());},scope:this}})
    		          }]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_efdt+'(<font color=red>*</font>)',
                        name:'efdt',editable:false,height:22,anchor:'95%',allowBlank:false,
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_exdt,
                        name:'exdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_itcd,
                        name: 'itcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/prbaalcr.mvc/getSalaryItems',
		                    listeners:{load:function(){var f = this.basisForm.findField('itcd');f.setValue(f.getValue());},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_vtyp +'(<font color=red>*</font>)',
                        name: 'vtyp',stateful:false,editable:false,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: AllocateValueTypeStore}]},
      		        
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Payroll_Label_valu +'(<font color=red>*</font>)',disabled:false,
                        name: 'valu',allowBlank:false,stateful:false,anchor:'95%',decimalPrecision:2,keepZero:true,allowNegative:false}]},
      		        
     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_lmtm,
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
		var f1 = this.basisForm.findField('vtyp');
		var f2 = this.basisForm.findField('valu');
		if (f1.getValue()=='Percentage' && f2.getValue()>100){
   		    Ext.MessageBox.show({
	            title: HRMSRes.Public_Message_Error,
	            msg:HRMSRes.Public_Message_VILT100,
	            buttons: Ext.MessageBox.OK,
	            icon:Ext.MessageBox.ERROR
            });
		    return;
		}
		
		this.grid.getBottomToolbar().diplayMsg.update('');
		var params={};
		this.basisForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + prbaalcrPageName + '.mvc/'+method,
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
		   		        store.insert(0,new store.recordType(params));
		   		        this.grid.store.totalLength+=1;
				        this.grid.getBottomToolbar().updateInfo();
		   		    }
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
		   params: {record:Ext.util.JSON.encode(params)}
		});
	}
});


var prbaalcrPanel=function(){
    this.tabId=prbaalcrConfig.tabId;
	this.init();	
	
	prbaalcrPanel.superclass.constructor.call(this,{
		applyTo:this.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	autoScroll:true,      
	        tbar: [{ 
	        	id:this.tabId+ '_add',
	            iconCls:'icon-add', 
	            text: HRMSRes.Public_Toolbar_Add, 
	            hidden:prbaalcrConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new prbaalcrEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden:prbaalcrConfig.auth[this.tabId+'_' + 'edit']!='True',
	            handler: function(){
	            	new prbaalcrEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:prbaalcrConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:prbaalcrConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new prbaalcrQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:prbaalcrConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
	            handler: this.exportExcel, 
	            scope: this 
	        },'->',{ 
	        	xtype:'label', 
	            html:  '<b><font color=green>employee no: ' + prbaalcrConfig.emno + '</font></b>',
	            hidden: prbaalcrConfig.emno==''
	        },'->',{ 
                id:this.tabId+'_logview',
                iconCls:'icon-logview', 
                text:'', 
                handler: function(){
                        var data=this.grid.getSelectionModel().getSelected();if (data==null) return;var rfid = data.get('rfid');new logViewerWindow(rfid).show();},
                scope: this 
            },{ 
                id:this.tabId+'_muf',
                iconCls:prbaalcrConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,prbaalcrConfig.muf?'delete':'add',prbaalcrConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(prbaalcrPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var prbaalcrStoreType=Ext.data.Record.create([
		    {name:'emno'},
            {name:'sfid'},
            {name:'stfn'},
            {name:'sqno'},
            {name:'acno'},
            {name:'efdt'},
            {name:'exdt'},
            {name:'itcd'},
            {name:'vtyp'},
            {name:'valu'},
            {name:'lmtm'},
            {name:'lmur'},
            {name:'itnm'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},prbaalcrStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:prbaalcrKeyColumn,ColumnValue:prbaalcrConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + prbaalcrPageName + '.mvc/list',
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
                {header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno'},
                {header:HRMSRes.Payroll_Label_acno,sortable: true, dataIndex: 'acno'},
                {header:HRMSRes.Public_Label_efdt,sortable: true, dataIndex: 'efdt',renderer:formatDateNoTime},
                {header:HRMSRes.Public_Label_exdt,sortable: true, dataIndex: 'exdt',renderer:formatDateNoTime},
                {header:HRMSRes.Payroll_Label_itnm,sortable: true, dataIndex: 'itnm'},
                {header:HRMSRes.Payroll_Label_vtyp,sortable: true, dataIndex: 'vtyp'},
                {header:HRMSRes.Payroll_Label_valu,sortable: true, dataIndex: 'valu'},
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

		var params={};
        params['emno']=record.get('emno');
        params['sqno']=record.get('sqno');
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ prbaalcrPageName + '.mvc/delete',
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
	   			        params:{record:Ext.encode(params)}
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
	    var params={record:Ext.encode({params:[{ColumnName:prbaalcrKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + prbaalcrPageName + '.mvc/exportexcel';
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
    new prbaalcrPanel();
})

    </script>

</body>
</html>