﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="prcalcul.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.prcalcul.prcalcul" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var prcalculConfig=Ext.decode('<%=ViewData["config"] %>'); 
prcalculConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var prcalculPageName = 'prcalcul';
var prcalculKeyColumn='emno';
            
var prcalculQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	prcalculQueryWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:400, 
        height:200, 
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

Ext.extend(prcalculQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:'Run No',
                        name: 'rnno',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:'Period' ,
                        name: 'perd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getValidPeriod',
		                    listeners:{
                                load:function(){f = this.form.findField('perd');f.setValue(f.getValue());},
                                scope:this}})
    		          }]}
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

var calculationStatusWindow = function(type, cond, custcond, perd, icnj, ictm, pdcd) {
    this.type = type;
    this.cond = cond;
    this.custcond = custcond;
    this.perd = perd;
    this.icnj = icnj;
    this.ictm = ictm;
    this.pdcd = pdcd;

    this.init();

    calculationStatusWindow.superclass.constructor.call(this, {
        layout: 'fit',
        width: 400,
        height: 80,
        closeAction: 'close',
        modal: true,
        resizable: false,
        buttonAlign: 'center',
        items: this.formPanel,
        title: 'Calculation Window',
        listeners: {
            show: function() {
                this.calculation();
            }
        },
        scope: this
    });
}

Ext.extend(calculationStatusWindow,Ext.Window,{
    init:function(){
		this.formPanel=this.createFormPanel();
		//this.form=this.formPanel.getForm();
	},
	
	createFormPanel:function(){	        
		return new Ext.Panel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         html:'<div class=\"loading-indicator\"><img src=\"/ExtJS/ext/resources/images/default/grid/loading.gif" width=\"16\" height=\"16\"  style=\"margin-right: 8px;\" /><font color=\'red\'>' + 'Performance calculation,don\'t close it.' + '</font></div>'
       })
	},
	calculation:function(){
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + prcalculPageName + '.mvc/calculation',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		   		

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
		   params: {record:Ext.encode({type:this.type,cond:this.cond,custcond:this.custcond,perd:this.perd,icnj:this.icnj,ictm:this.ictm,pdcd:this.pdcd})}
		});	    
	},
	scope:this
});

var calcWindow=function(){
	this.init();	
	
	calcWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:550, 
        height:420, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.formPanel,
        title:'Calculate Window',
        buttons: [{ 
            text:'Do calculate', 
            iconCls:'icon-dashboard', 
            handler: this.Confirm,
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

Ext.extend(calcWindow, Ext.Window, {
    init: function() {
        this.formPanel = this.createFormPanel();
        this.form = this.formPanel.getForm();
    },

    createFormPanel: function() {
        return new Ext.FormPanel({
            frame: true,
            labelWidth: 100,
            header: true,
            items: [
                    { xtype: 'fieldset', title: 'Calculation Type', autoHeight: true, defaultType: 'radio',
                        items: [
                        { boxLabel: 'Simulation calculation', hideLabel: true, checked: true, name: 'calctype', inputValue: 'S' },
                        { boxLabel: 'Actual calculation', hideLabel: true, name: 'calctype', inputValue: 'A'
}]
                        },

                    { xtype: 'fieldset', title: 'Calculation Condition', autoHeight: true, layout: 'column',
                        items: [
                        { columnWidth: .8, layout: 'form',
                            items: [{ xtype: 'combo', fieldLabel: 'Condition',
                                name: 'cond', stateful: false, typeAhead: true,
                                triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                                displayField: 'DisplayField', valueField: 'ValueField',
                                store: new Ext.data.Store({
                                    reader: new Ext.data.JsonReader({ totalProperty: "results", root: "rows",
                                        fields: ['ValueField', 'DisplayField']
                                    }), autoLoad: true,
                                    url: ContextInfo.contextPath + '/dropdown.mvc/getCondition',
                                    listeners: { load: function() { var v = this.form.findField('cond').getValue(); this.form.findField('cond').setValue(v); }, scope: this
                                    }
                                })
}]
                            },

                        { layout: 'form', columnWidth: .2,
                            items: [{ xtype: 'button', text: 'Custom', id: 'custom', name: 'custom', stateful: false,
                                listeners: { click: function(p) { new empAdvQryQueryNoGridWindow().show() }, scope: this}}]
                            },

                        { columnWidth: .5, layout: 'form',
                            items: [{ xtype: 'combo', fieldLabel: HRMSRes.Period_Label_Period + '(<font color=red>*</font>)', allowBlank: false,
                                name: 'perd', stateful: false, typeAhead: true,
                                triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                                displayField: 'DisplayField', valueField: 'ValueField',
                                store: new Ext.data.Store({
                                    reader: new Ext.data.JsonReader({ totalProperty: "results", root: "rows",
                                        fields: ['ValueField', 'DisplayField']
                                    }), autoLoad: true,
                                    url: ContextInfo.contextPath + '/dropdown.mvc/getValidPeriod',
                                    listeners: { load: function() { var v = this.form.findField('perd').getValue(); this.form.findField('perd').setValue(v); }, scope: this
                                    }
                                })
}]
                            },

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Master_Label_pdcd + '(<font color=red>*</font>)',
                            name: 'pdcd', stateful: false, typeAhead: true, allowBlank: false,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: new Ext.data.Store({
                                reader: new Ext.data.JsonReader({ totalProperty: "results", root: "rows",
                                    fields: ['DisplayField', 'ValueField']
                                }), autoLoad: true,
                                url: ContextInfo.contextPath + '/dropdown.mvc/getPayDay',
                                listeners: {
                                    load: function() { f = this.basisDtlEditForm.findField('pdcd'); f.setValue(f.getValue()); },
                                    scope: this
                                }
                            })
                            }]
                        },

      		            { columnWidth: 1, layout: 'form',
      		                items: [{ xtype: 'checkbox', fieldLabel: '', labelSeparator: '', boxLabel: 'Include staffs termiating in the period?',
      		                    name: 'inte', stateful: false, anchor: '95%'}]
      		                },

      		            { columnWidth: 1, layout: 'form',
      		                items: [{ xtype: 'checkbox', fieldLabel: '', labelSeparator: '', boxLabel: 'Include staffs joining in the period?',
      		                    name: 'inji', stateful: false, anchor: '95%'}]
      		                }
                    ]
                        },

                    { xtype: 'fieldset', title: 'Calculation Result', autoHeight: true, layout: 'column',
                        items: [
  		                { layout: 'form', columnWidth: .5,
  		                    items: [{ xtype: 'textfield', fieldLabel: 'Run No', name: 'rnno', disabled: true, stateful: false, anchor: '95%'}]
  		                },

  		                { layout: 'form', columnWidth: .5,
  		                    items: [{ xtype: 'textfield', fieldLabel: 'Run date', name: 'rndt', disabled: true, stateful: false, anchor: '95%'}]
  		                },

  		                { layout: 'form', columnWidth: .5,
  		                    items: [{ xtype: 'textfield', fieldLabel: 'Run type', name: 'rnty', disabled: true, stateful: false, anchor: '95%'}]
  		                },

  		                { layout: 'form', columnWidth: .5,
  		                    items: [{ xtype: 'numberfield', fieldLabel: 'TTL amount', name: 'tamt', disabled: true, stateful: false, anchor: '95%'}]
  		                },

  		                { layout: 'form', columnWidth: .5,
  		                    items: [{ xtype: 'numberfield', fieldLabel: 'TTL staff count', name: 'tsct', disabled: true, stateful: false, anchor: '95%'}]
  		                }
                    ]
                    }

            ]
        })
    },

    Confirm: function() {
        var params = [];

        var type = this.form.findField('calctype').getGroupValue();
        var cond = this.form.findField('cond').getValue();
        var custcond = empAdvQryResult;
        var perd = this.form.findField('perd').getValue();
        var icnj = this.form.findField('inte').getValue() == true ? 'Y' : 'N';
        var ictm = this.form.findField('inji').getValue() == true ? 'Y' : 'N';
        var pdcd = this.form.findField('pdcd').getValue();
        //alert(this.form.findField('inte').getValue());
        new calculationStatusWindow(type, cond, custcond, perd, icnj, ictm, pdcd).show();
    },
    scope: this
});

var prcalculPanel=function(){
    this.tabId=prcalculConfig.tabId;
	this.init();	
	
	prcalculPanel.superclass.constructor.call(this,{
		applyTo:this.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	autoScroll:true,      
	        tbar: [{ 
	        	id:this.tabId+ '_calculate',
	            iconCls:'icon-update', 
	            text:'Calculate',
	            //hidden:prcalculConfig.auth[this.tabId+'_' + 'calculate']!='True',
	            handler: function(){
	            	new calcWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:prcalculConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new prcalculQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:prcalculConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:prcalculConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,prcalculConfig.muf?'delete':'add',prcalculConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(prcalculPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var prcalculStoreType=Ext.data.Record.create([
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
            {name:'lmur'}
            
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},prcalculStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:prcalculKeyColumn,ColumnValue:prcalculConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + prcalculPageName + '.mvc/list',
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

                {header:HRMSRes.Leave_Label_year,sortable: true, dataIndex: 'year'},
                {header:HRMSRes.Leave_Label_ltnm,sortable: true, dataIndex: 'ltnm'},
                {header:HRMSRes.Leave_Label_hors,sortable: true, dataIndex: 'hors'},
                {header:HRMSRes.Leave_Label_days,sortable: true, dataIndex: 'days'},
                {header:HRMSRes.Leave_Label_hrcs,sortable: true, dataIndex: 'hrcs'},
                {header:HRMSRes.Leave_Label_daro,sortable: true, dataIndex: 'daro',renderer:formatDateNoTime},

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
	   			        url:ContextInfo.contextPath+ '/'+ prcalculPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:prcalculKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + prcalculPageName + '.mvc/exportexcel';
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
    new prcalculPanel();
    
    this.empAdvQryResult = [];
    
})

    </script>

</body>
</html>