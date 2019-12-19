<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="praccalc.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.praccalc.praccalc" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var praccalcConfig=Ext.decode('<%=ViewData["config"] %>'); 
praccalcConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var praccalcPageName = 'praccalc';
var praccalcKeyColumn='emno';
            
var praccalcQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	praccalcQueryWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:400, 
        height:220, 
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

Ext.extend(praccalcQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Payroll_Label_rnno,
                        name: 'rnno',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Public_Label_perd ,
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
                        }]},

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

var calculationStatusWindow=function(rnno,perd,cond,custcond){
    this.rnno = rnno;
    this.cond = cond;
    this.custcond = custcond;
    this.perd = perd;

	this.init();	
	
	calculationStatusWindow.superclass.constructor.call(this,{
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
            	this.calculation();
            }
        },
        scope:this    
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
		   url: ContextInfo.contextPath+'/' + praccalcPageName + '.mvc/calculation',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		   		

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
		   params: {record:Ext.encode({rnno:this.rnno,cond:this.cond,custcond:this.custcond,perd:this.perd})}
		});	    
	},
	scope:this
});

var calcWindow=function(){
	this.init();	
	
	calcWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:450, 
        height:320, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.formPanel,
        title:HRMSRes.Public_Message_calcwin,
        buttons: [{ 
            text:HRMSRes.Payroll_Label_docalculate, 
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

Ext.extend(calcWindow,Ext.Window,{
    init:function(){
		this.formPanel=this.createFormPanel();
		this.form=this.formPanel.getForm();
	},
	
	createFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:100,
	         header:true,
             items: [
                    {xtype:'fieldset',title: 'Calculation Condition',height:230,layout:'form',
                    items: [
                        {columnWidth:.5,layout: 'form',
                        items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_rnno ,
                            name: 'rnno',stateful:false,typeAhead: true,allowBlank:false,
                            triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                            displayField: 'DisplayField',valueField:'ValueField',
                            store: new Ext.data.Store({ 
		                        reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                        fields: ['ValueField','DisplayField']}), autoLoad:true,
		                        url:ContextInfo.contextPath+'/dropdown.mvc/getRun'
		                        //listeners:{load: function(){var v = this.form.findField('rnno').getValue();this.form.findField('rnno').setValue(v);},scope:this
		                        })
    		              }]},
    		              
                        {columnWidth:.5,layout: 'form',
                        items: [{xtype:'combo',fieldLabel:HRMSRes.Public_Label_perd ,allowBlank:false,
                            name: 'perd',stateful:false,typeAhead: true,
                            triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                            displayField: 'DisplayField',valueField:'ValueField',
                            store: new Ext.data.Store({ 
		                        reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                        fields: ['ValueField','DisplayField']}), autoLoad:true,
		                        url:ContextInfo.contextPath+'/dropdown.mvc/getValidPeriod',
		                        listeners:{load: function(){var v = this.form.findField('perd').getValue();this.form.findField('perd').setValue(v);},scope:this
		                        }})
    		              }]},
    		              
                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Master_Label_pdcd,
                            name: 'pdcd', stateful: false, typeAhead: true, allowBlank: true,
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
                        }]},
    		              
                        {columnWidth:.5,layout: 'form',
                        items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_condition ,
                            name: 'cond',stateful:false,typeAhead: true,
                            triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                            displayField: 'DisplayField',valueField:'ValueField',
                            store: new Ext.data.Store({ 
		                        reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                        fields: ['ValueField','DisplayField']}), autoLoad:true,
		                        url:ContextInfo.contextPath+'/dropdown.mvc/getCondition',
		                        listeners:{load: function(){var v = this.form.findField('cond').getValue();this.form.findField('cond').setValue(v);},scope:this
		                        }})
    		              }]},
    		              
                        {layout:'form',columnWidth:.2,
                        items:[{xtype:'button',text:HRMSRes.Payroll_Label_custom,id:'custom',name: 'custom',stateful:false,
                                listeners:{click:function(p){new empAdvQryQueryNoGridWindow().show()},scope:this}}]}            
    		              
                    ]}
                    
                    

            ] 
       })
	},	
	Confirm:function(){	    
	    if(!this.form.isValid()) return;
	    
	    var params=[];	 
	           
	    var rnno = this.form.findField('rnno').getValue();
	    var perd = this.form.findField('perd').getValue();
	    var cond = this.form.findField('cond').getValue();
	    var custcond = empAdvQryResult;
	    new calculationStatusWindow(rnno,perd,cond,custcond).show();       
	},
	scope:this
});

var praccalcPanel=function(){
    this.tabId=praccalcConfig.tabId;
	this.init();	
	
	praccalcPanel.superclass.constructor.call(this,{
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
	            text:HRMSRes.Public_Toolbar_Calculate,
	            //hidden:praccalcConfig.auth[this.tabId+'_' + 'calculate']!='True',
	            handler: function(){
	            	new calcWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },'-',{ 
                xtype:'combo', 
                id: 'praccalcQueryType',
                store: new Ext.data.SimpleStore({
                    fields: ['value', 'name'],
                    data : [
                        ['Summary',HRMSRes.Payroll_Label_summary],
                        ['Details',HRMSRes.Payroll_Label_details] 
                      ]
                }),
                displayField: 'name',
		        valueField: 'value',
		        stateful:false,
                typeAhead: true,
                mode: 'local',
                editable:false,
                triggerAction: 'all',
                allowBlank:false, 
                anchor:'95%',
			    emptyText:HRMSRes.Public_Label_Choose,
                width:200,
                listeners:{
                    select:function(QueryType_combo)
                    {
                       if (QueryType_combo.getValue()=='Summary')
                       {
                            this.LoadSummaryGrid(this.grid);
                       }
                       
                       if (QueryType_combo.getValue()=='Details') 
                       {
                             this.LoadDetailsGrid(this.grid);                      
                       }
                       this.grid.view.fitColumns();
                       
	    			},
	    			scope:this
	    		  } 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:praccalcConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new praccalcQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:praccalcConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:praccalcConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,praccalcConfig.muf?'delete':'add',praccalcConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(praccalcPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

	    var praccalcStoreType=Ext.data.Record.create([
		    {name:'emno'},
            {name:'sfid'},
            {name:'sfnm'},
            {name:'perd'},
            {name:'sqno'},
            {name:'perd'},
            {name:'itcd'},
            {name:'itnm'},
            {name:'acno'},
            {name:'pkty'},
            {name:'amnt'},
            {name:'crcd'},
            {name:'crnm'},
            {name:'rnno'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},praccalcStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:praccalcKeyColumn,ColumnValue:praccalcConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + praccalcPageName + '.mvc/listdetails',
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
                {header:HRMSRes.Public_Label_sftn,sortable: true, dataIndex: 'sfnm',hidden:false},
                {header:HRMSRes.Public_Label_perd,sortable: true, dataIndex: 'perd'},
                {header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno'},
                {header:HRMSRes.Payroll_Label_rnno,sortable: true, dataIndex: 'rnno'},
                {header:HRMSRes.Payroll_Label_itcd,sortable: true, dataIndex: 'itcd'},
                {header:HRMSRes.Payroll_Label_itnm,sortable: true, dataIndex: 'itnm'},
                {header:HRMSRes.Payroll_Label_acno,sortable: true, dataIndex: 'acno'},
                {header:HRMSRes.Payroll_Label_amnt,sortable: true, dataIndex: 'amnt'},
                {header:HRMSRes.Master_Label_crnm,sortable: true, dataIndex: 'crnm'}
            ]),
            bbar: new Ext.PagingToolbar({
                pageSize:Pagination.pagingSize,
                store: store,
                displayInfo: true,
                displayMsg:HRMSRes.Public_PagingToolbar_Total+':{1}/{2}',
                emptyMsg: HRMSRes.Public_PagingToolbar_EmptyMsg                    
            })
	    });
	    
	},
    LoadDetailsGrid:function(grid){
        var store;
        var praccalc_SummaryType=Ext.data.Record.create([
		    {name:'emno'},
            {name:'sfid'},
            {name:'sfnm'},
            {name:'perd'},
            {name:'sqno'},
            {name:'perd'},
            {name:'itcd'},
            {name:'itnm'},
            {name:'acno'},
            {name:'pkty'},
            {name:'amnt'},
            {name:'crcd'},
            {name:'crnm'},
            {name:'rnno'}            
        ]);

        store=new Ext.data.Store({ 
            reader: new Ext.data.JsonReader({
                totalProperty: "results",
                root: "rows"               
            },praccalc_SummaryType), 		    
            url:ContextInfo.contextPath+'/' + praccalcPageName + '.mvc/listdetails',
            listeners:
            {
                loadexception:function(o,t,response)
                {
                    var o= Ext.util.JSON.decode(response.responseText);		   		
                    grid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                scope:this
            }
        });
       
        //this.grid.getBottomToolbar().store=store;

        grid.reconfigure(store,
            new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Public_Label_sfid,sortable: true, dataIndex: 'sfid',hidden:false},
                {header:HRMSRes.Public_Label_sftn,sortable: true, dataIndex: 'sfnm',hidden:false},
                {header:HRMSRes.Public_Label_perd,sortable: true, dataIndex: 'perd'},
                {header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno'},
                {header:HRMSRes.Payroll_Label_rnno,sortable: true, dataIndex: 'rnno'},
                {header:HRMSRes.Payroll_Label_itcd,sortable: true, dataIndex: 'itcd'},
                {header:HRMSRes.Payroll_Label_itnm,sortable: true, dataIndex: 'itnm'},
                {header:HRMSRes.Payroll_Label_acno,sortable: true, dataIndex: 'acno'},
                {header:HRMSRes.Payroll_Label_amnt,sortable: true, dataIndex: 'amnt'},
                {header:HRMSRes.Master_Label_crnm,sortable: true, dataIndex: 'crnm'}
            ])
        );
        grid.getBottomToolbar().bind(store);
    },
	LoadSummaryGrid:function(grid){
        var store;
        var praccalc_DetailsType=Ext.data.Record.create([
		    {name:'emno'},
            {name:'sfid'},
            {name:'sfnm'},
            {name:'perd'},
            {name:'perd'},
            {name:'acno'},
            {name:'amnt'},
            {name:'crcd'},
            {name:'crnm'},
            {name:'rnno'}
        ]);

        store=new Ext.data.Store({ 
            reader: new Ext.data.JsonReader({
                totalProperty: "results",
                root: "rows"               
            },praccalc_DetailsType), 		    
            url:ContextInfo.contextPath+'/' + praccalcPageName + '.mvc/listsummary',
            listeners:
            {
                loadexception:function(o,t,response)
                {
                    var o= Ext.util.JSON.decode(response.responseText);		   		
                    grid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                scope:this
            }
        });
       
        //this.grid.getBottomToolbar().store=store;

        grid.reconfigure(store,
            new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Public_Label_sfid,sortable: true, dataIndex: 'sfid',hidden:false},
                {header:HRMSRes.Public_Label_sftn,sortable: true, dataIndex: 'sfnm',hidden:false},
                {header:HRMSRes.Public_Label_perd,sortable: true, dataIndex: 'perd'},
                {header:HRMSRes.Payroll_Label_rnno,sortable: true, dataIndex: 'rnno'},
                {header:HRMSRes.Payroll_Label_acno,sortable: true, dataIndex: 'acno'},
                {header:HRMSRes.Payroll_Label_amnt,sortable: true, dataIndex: 'amnt'},
                {header:HRMSRes.Master_Label_crnm,sortable: true, dataIndex: 'crnm'}
            ])
        );
        grid.getBottomToolbar().bind(store);
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
	    var params={record:Ext.encode({params:[{ColumnName:praccalcKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + praccalcPageName + '.mvc/exportexcel';
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
    new praccalcPanel();
    
    this.empAdvQryResult = [];
    
})

    </script>

</body>
</html>