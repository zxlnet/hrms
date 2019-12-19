<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="stperiod.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.stperiod.stperiod" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var stperiodConfig=Ext.decode('<%=ViewData["config"] %>');   
stperiodConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');

/***************************************
    Edit windows
****************************************/
var stperiodEditWindows = function(isNew, grid) {
    this.isNew = isNew;
    this.grid = grid;
    this.store = this.grid.getStore();
    this.record;
    this.init();

    stperiodEditWindows.superclass.constructor.call(this, {
        layout: 'fit',
        width: 600,
        height: 380,
        closeAction: 'hide',
        modal: true,
        resizable: false,
        buttonAlign: 'center',
        items: this.formPanel,
        title: HRMSRes.Public_Edit_WindowTitle,
        buttons: [
			{
			    text: HRMSRes.Public_Button_Save,
			    iconCls: 'icon-save',
			    handler: this.save,
			    scope: this
			},
			{
			    text: HRMSRes.Public_Button_Close,
			    iconCls: 'icon-exit',
			    handler: function() { this.close(); },
			    scope: this
			}
		],
        listeners: {
            show: function() {
                var data = this.grid.getSelectionModel().getSelected();
                this.form.items.each(function(f) {
                    if (f.isFormField) {
                        var value = data.get(f.getName());
                        if (f.xtype == 'timefield') {
                            value = formatTime(value);
                        }

                        if (f.xtype == 'datefield') {
                            value = formatDateNoTime(value);
                        }

                        f.setValue(value);
                    }
                });

                this.form.findField('year').setDisabled(true);
                if (this.grid.getSelectionModel().getSelected().get("psts") == PeriodStatus.Open) {
                    this.form.findField('pest').setDisabled(false);
                    this.form.findField('peen').setDisabled(false);
                    this.form.findField('remk').setDisabled(false);
                }
                else {
                    if (this.grid.getSelectionModel().getSelected().get("psts") == PeriodStatus.Closed) {
                        this.form.findField('pest').setDisabled(true);
                        this.form.findField('peen').setDisabled(true);
                        this.form.findField('remk').setDisabled(true);
                    }
                    else {
                        //Unused
                        this.form.findField('pest').setDisabled(false);
                        this.form.findField('peen').setDisabled(false);
                        this.form.findField('remk').setDisabled(false);
                    }
                }
            },
            scope: this
        }
    })
}

Ext.extend(stperiodEditWindows,Ext.Window,{
	init:function(){
		this.formPanel = this.createFormPanel();
		this.form = this.formPanel.getForm();
	},
	createFormPanel:function(){
		return new Ext.FormPanel({
			frame:true,
			labelWidth:100,
			header:true,
			items:[
				{
					layout:'column',
					items:[
						{
							columnWidth:.5,
							layout:'form',
							items:{
								xtype:'textfield',
								fieldLabel:HRMSRes.Period_Label_Period+'(<font color=red>*</font>)',
								name:'perd',
								allowBlank:false,
								disabled:true,
								anchor:'95%'
							}	
						},
							{
							columnWidth:.5,
							layout:'form',
							items:{
								xtype:'textfield',
								fieldLabel:HRMSRes.Period_Label_Year+'(<font color=red>*</font>)',
								name:'year',
								allowBlank:false,
								disabled:true,
								anchor:'95%'
							}	
						}
					]	
				},
				{
					layout:'column',
					items:[
						{
							columnWidth:.5,
							layout:'form',
							items:{
								xtype:'datefield',
								fieldLabel:HRMSRes.Period_Label_Start+'(<font color=red>*</font>)',
								name: 'pest',
								id: 'pest',
								allowBlank:false,
								format: DATE_FORMAT.DATEONLY,
								height:22,
								anchor:'95%'	
							}	
						},
						{
							columnWidth:.5,
							layout:'form',
							items:{
								xtype:'datefield',
								fieldLabel:HRMSRes.Period_Label_End+'(<font color=red>*</font>)',
								name: 'peen',
								id: 'peen',
								allowBlank:false,
								format: DATE_FORMAT.DATEONLY,
								height:22,
								anchor:'95%'	
							}	
						}
					]	
				},
				{
					layout:'column',
					items:[
						{
							columnWidth:.5,
							layout:'form',
							items:{
								xtype:'combo',
								fieldLabel:HRMSRes.Period_Label_Status+'(<font color=red>*</font>)',
								name: 'psts',
								store: new Ext.data.SimpleStore({
                                            fields: ['value', 'name'],
                                            data : PeriodStatusData
                                }),
								height:22,
								mode:'local',
								displayField:'value',
								allowBlank:false,
								disabled:true,
								anchor:'95%'
							}	
						},
						{
							columnWidth:.5,
							layout:'form',
							items:{
								xtype:'textfield',
								fieldLabel:HRMSRes.Period_Label_Month,
								name: 'mnth',
								height:22,
								allowBlank:true,
								disabled:true,
								anchor:'95%'	
							}	
						}
					]	
				},{
					layout:'column',
					items:[
						{
							columnWidth:.5,
							layout:'form',
							items:{
								xtype:'textfield',
								fieldLabel:HRMSRes.Period_Label_ClosedTime,
								name: 'cstm',
								id: 'cstm',
								allowBlank:true,
								height:22,
								disabled:true,
								anchor:'95%'	
							}	
						},
						{
							columnWidth:.5,
							layout:'form',
							items:{
								xtype:'textfield',
								fieldLabel:HRMSRes.Period_Label_ClosedBy,
								name: 'csby',
								height:22,
								allowBlank:true,
								disabled:true,
								anchor:'95%'	
							}	
						}
					]	
				},
				{
					xtype:'textarea',
					fieldLabel:HRMSRes.Period_Label_Remark,
					name:'remk',
				    height:150,
					anchor:'98%'	
				}
			]
		})
	},
	save: function(){
		if(!this.form.isValid()) return;
		
		var params={};//this.form.getValues();
		params['perd']=this.form.findField('perd').getValue();
		var url=ContextInfo.contextPath+'/stperiod.mvc/';
		url+=this.isNew?'create':'edit';
	    params['year']=this.form.findField('year').getValue();
	    params['pest'] = Ext.util.Format.date(this.form.findField('pest').getValue(), DATE_FORMAT.DATEONLY);
	    params['peen'] = Ext.util.Format.date(this.form.findField('peen').getValue(), DATE_FORMAT.DATEONLY);
	    params['psts'] = this.form.findField('psts').getValue()
	    params['mnth'] = this.form.findField('mnth').getValue()
	    params['csby'] = this.form.findField('csby').getValue()
		params['cstm'] = formatDate(this.form.findField('cstm').getValue());
		params['remk'] = this.form.findField('remk').getValue();

		Ext.Ajax.request({
		   url:url,
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);
		   		if(o.status=='success'){
			        if(this.isNew){	
			            this.store.reload();
			            this.grid.getBottomToolbar().updateInfo();
		            }else{
		                this.store.reload();		    			
			            this.grid.getBottomToolbar().updateInfo();
		            }  
		            this.close();
			    }else{
			        var msg=this.isNew?HRMSRes.Public_Message_AddBad:HRMSRes.Public_Message_EditBad;
			        Ext.MessageBox.show({
   					    title: HRMSRes.Public_Message_Error,
   					    msg:msg,
   					    buttons: Ext.MessageBox.OK,
   					    icon:Ext.MessageBox.ERROR
   				    });
			    }			    
			    this.grid.getBottomToolbar().diplayMsg.update(o.msg);		    			   		
		   },
		   failure: function(response){
		   		Ext.MessageBox.show({
   					title: HRMSRes.Public_Message_Error,
   					msg:response.responseText,
   					buttons: Ext.MessageBox.OK,
   					icon:Ext.MessageBox.ERROR
   				});
		   },
		   scope:this,
		   params: {record:Ext.util.JSON.encode(params)}
		});
		
	},
	scope:this
});

var ststperiodQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	ststperiodQueryWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:300, 
        height:180, 
        closeAction:'hide', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.formPanel,
        title:HRMSRes.Public_Query_WindowTitle,
        buttons: [{ 
            text:HRMSRes.Public_Toolbar_Query, 
            iconCls:'icon-query', 
            handler: this.Query,
            scope: this
        },{ 
            text: HRMSRes.Public_Button_Close, 
            iconCls: 'icon-exit', 
            handler: function(){                  
                this.hide();
            },
            scope:this
        }]
	})
}

Ext.extend(ststperiodQueryWindow,Ext.Window,{
    init:function(){
		this.formPanel=this.createFormPanel();
		this.form=this.formPanel.getForm();
		this.year='';
		this.period='';
	},
	createFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:90,
	         header:true,
      		 items: [{ 
                xtype:'combo', 
                fieldLabel:HRMSRes.Public_Label_Year, 
                name: 'PeriodCombo', 
                editable:true,
                typeAhead: true,
                triggerAction: 'all',
                stateful:false,
                mode: 'local',
                allowBlank:false,
                store: new Ext.data.Store({ 
				    reader: new Ext.data.JsonReader({
			 		    totalProperty: "results",
					    root: "rows",
					    fields: [
						    'year'
					    ]               
				    }), 
				    url:ContextInfo.contextPath+'/stperiod.mvc/listPeriodYear',
				    autoLoad:true   					    
			    }),
                displayField: 'year',
		        valueField: 'year',
		        listeners:{
	    			select:function(period_combo,record){
	    				this.year=record.get('year');
	    				}
	    			},
                anchor:'95%'
            }] 
       })
	},	
	Query:function(){	    
	    this.grid.getBottomToolbar().diplayMsg.update('');
	    if(!this.form.isValid()) return;
	    var params={};
	    this.year = this.form.findField('PeriodCombo').getValue();
		params['year']=this.year;
		var loadParams={start:0,limit:Pagination.pagingSize};
		this.grid.queryParams=params;
		this.store.baseParams={record:Ext.util.JSON.encode(params)};
		this.store.load({params:loadParams});
		this.hide();
	},
	scope:this
});

var PeriodPanel=function(){
    this.stperiodQueryWin;
    this.tabId=stperiodConfig.tabId;
	this.init();	
	PeriodPanel.superclass.constructor.call(this,{
		applyTo:stperiodConfig.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	autoScroll:true,      
	        tbar: [{ 
	        	id:this.tabId+'_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:stperiodConfig.auth[this.tabId+'_query']!='True',
	            handler: function(){
	            	if (!this.stperiodQueryWin){
	            		this.stperiodQueryWin=new ststperiodQueryWindow(this.grid);
	            	}
	            	this.stperiodQueryWin.show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+'_update',
	            iconCls:'icon-update', 
	            text: HRMSRes.Public_Toolbar_Edit, 
	            hidden:stperiodConfig.auth[this.tabId+'_update']!='True',
	            handler:this.edit, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+'_openperiod',
	            iconCls:'icon-query', 
	            text:HRMSRes.Period_Toolbar_OpenPeriod,
	            hidden:stperiodConfig.auth[this.tabId+'_openperiod']!='True',
	            handler:function(){
	                    Ext.MessageBox.confirm('Confirm', HRMSRes.Public_Message_ConfirmOpenPeriod, showOpenPeriodResult,this);
                },	             
	            scope: this
	        },'-',{ 
	        	id:this.tabId+'_closeperiod',
	            iconCls:'icon-query', 
	            text:HRMSRes.Period_Toolbar_ClosePeriod,
	            hidden:stperiodConfig.auth[this.tabId+'_closeperiod']!='True',
	            handler:function() {
	                    Ext.MessageBox.confirm('Confirm', HRMSRes.Public_Message_ConfirmClosePeriod, showClosePeriodResult,this);
	                },
	            scope: this
	        },'-',{ 
	        	id:this.tabId+'_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:stperiodConfig.auth[this.tabId+'_exportexcel']!='True',
	            handler: this.ExportExcel, 
	            scope: this 
	        }], 
	        items:this.grid
       }]
	})
}

function showClosePeriodResult(btn)
{
    if (btn=='yes')
    {
       this.ClosePeriod();
    }
};

function showOpenPeriodResult(btn)
{
    if (btn=='yes')
    {
       this.OpenPeriod();
    }
};

function renderColor(val,m,r){
    if (r.get("psts") == "Open")
    {
        return '<span style=\'color:red\'>'+val+'</span>';
    }
    else if (r.get("psts") == "Closed")
    {
        return '<span style=\'color:gray\'>'+val+'</span>';
    }
    else
    {
        return '<span style=\'color:black\'>'+val+'</span>';
    }
}

/***************************************
    Grid definition
****************************************/
Ext.extend(PeriodPanel, Ext.Panel, {
    init: function() {
        this.grid = this.createGridPanel();
    },
    createGridPanel: function() {
        var PeriodType = Ext.data.Record.create([
		    { name: 'year' },
		    { name: 'perd' },
		    { name: 'mnth' },
            { name: 'pest' },
            { name: 'peen' },
            { name: 'psts' },
            { name: 'cstm' },
            { name: 'csby' },
            { name: 'remk' }
		]);

        var store = new Ext.data.Store({
            reader: new Ext.data.JsonReader({
                totalProperty: "results",
                root: "rows"
            }, PeriodType),
            url: ContextInfo.contextPath + '/stperiod.mvc/list',
            listeners: {
                loadexception: function(o, t, response) {
                    var o = Ext.util.JSON.decode(response.responseText);
                    this.grid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                scope: this
            }
        });
        store.type = PeriodType;

        store.load({ params: { start: 0, limit: Pagination.pagingSize} });

        return new Ext.grid.GridPanel({
            border: true,
            monitorResize: true,
            loadMask: true,
            ds: store,
            viewConfig: {
                forceFit: true
            },
            cm: new Ext.grid.ColumnModel([
					{ id: 'perd', header: HRMSRes.Period_Label_Period, sortable: true, dataIndex: 'perd', align: 'center', renderer: renderColor },
                    { id: 'year', header: HRMSRes.Period_Label_Year, sortable: true, dataIndex: 'year', align: 'center', renderer: renderColor },
					{ header: HRMSRes.Period_Label_Month, sortable: true, dataIndex: 'mnth', renderer: renderColor },
					{ header: HRMSRes.Period_Label_Start, sortable: true, dataIndex: 'pest', align: 'center', type: 'date', renderer: formatDateNoTime },
					{ header: HRMSRes.Period_Label_End, sortable: true, dataIndex: 'peen', align: 'center', type: 'date', renderer: formatDateNoTime },
					{ header: HRMSRes.Period_Label_Status, sortable: true, dataIndex: 'psts', renderer: renderColor },
					{ header: HRMSRes.Period_Label_ClosedTime, sortable: true, dataIndex: 'cstm', align: 'center', type: 'datetime', renderer: formatDateTime },
					{ header: HRMSRes.Period_Label_ClosedBy, sortable: true, dataIndex: 'csby', renderer: renderColor },
					{ header: HRMSRes.Period_Label_Remark, sortable: true, dataIndex: 'remk', renderer: renderColor }
                ]),
            bbar: new Ext.PagingToolbar({
                pageSize: Pagination.pagingSize,
                store: store,
                displayInfo: true,
                displayMsg: HRMSRes.Public_PagingToolbar_Total + ':{1}/{2}',
                emptyMsg: HRMSRes.Public_PagingToolbar_EmptyMsg
            }),
            listeners: {
                //rowdblclick: this.edit,
                render: function(p_grid) {
                    controlButton(p_grid, this.tabId);
                },
                rowclick: function(p_grid) {
                    controlButton(p_grid, this.tabId);
                },
                scope: this
            }
        })
    },
    edit: function() {
        this.PeriodCommonEditWin = new stperiodEditWindows(false, this.grid);
        this.PeriodCommonEditWin.show();
    },
    OpenPeriod: function() {
        if (!this.grid.getSelectionModel().hasSelection()) {
            this.grid.getBottomToolbar().diplayMsg.update(HRMSRes.Public_Message_SelectPeriodFirst);
            return;
        }
        var params = {};

        params["perd"] = this.grid.getSelectionModel().getSelected().get("perd");
        Ext.Ajax.request({
            url: ContextInfo.contextPath + '/stperiod.mvc/openPeriod',
            success: function(response) {
                var o = Ext.util.JSON.decode(response.responseText);
                if (o.status == 'success') {
                    this.grid.getStore().reload();
                }
                this.grid.getBottomToolbar().diplayMsg.update(o.msg);
            },

            scope: this,
            params: { record: Ext.util.JSON.encode(params) }
        });
    },
    ClosePeriod: function() {
        if (!this.grid.getSelectionModel().hasSelection()) {
            this.grid.getBottomToolbar().diplayMsg.update(HRMSRes.Public_Message_SelectPeriodFirst);
            return;
        }
        var params = {};

        params["perd"] = this.grid.getSelectionModel().getSelected().get("perd");
        Ext.Ajax.request({
            url: ContextInfo.contextPath + '/stperiod.mvc/closePeriod',
            success: function(response) {
                var o = Ext.util.JSON.decode(response.responseText);
                if (o.status == 'success') {
                    this.grid.getStore().reload();
                }
                this.grid.getBottomToolbar().diplayMsg.update(o.msg);
            },
            scope: this,
            params: { record: Ext.util.JSON.encode(params) }
        });
    },
    ExportExcel: function() {
        if (this.grid.getStore().getTotalCount() <= 0) {
            this.grid.getBottomToolbar().diplayMsg.update('<font color=red>' + HRMSRes.Public_Message_ExcelNoRecord + '</font>');
            return;
        }
        var params = {};
        var cm = this.grid.getColumnModel();
        var header = [];

        for (var i = 0; i < cm.config.length; i++) {
            if (!cm.isHidden(i)) {
                var cname = cm.config[i].header;
                var mapping = cm.config[i].dataIndex;
                var datatype = cm.config[i].type;
                header[header.length] = {
                    ColumnDisplayName: cname,
                    ColumnName: mapping,
                    ColumnType: datatype
                };
            }
        }

        params.header = Ext.encode(header);
        if (this.grid.queryParams) {
            params.record = Ext.encode(this.grid.queryParams);
        }

        var form = document.createElement('form');
        form.name = 'excelForm';
        form.method = 'post';
        form.action = ContextInfo.contextPath + '/stperiod.mvc/exportExcel';
        for (var p in params) {
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

/***************************************
    Button control
****************************************/
function controlButton(grid,tabId){
	//Ext.getCmp('Period_Update').setDisabled(false);
	
	if (!grid.getSelectionModel().hasSelection()){
		Ext.getCmp(tabId+'_update').setDisabled(true);
		Ext.getCmp(tabId+'_closeperiod').setDisabled(true);
		Ext.getCmp(tabId+'_openperiod').setDisabled(true);
	}
	else
	{
	    if ((grid.getSelectionModel().getSelected().get("psts") == PeriodStatus.Open))
	    {
		    Ext.getCmp(tabId+'_closeperiod').setDisabled(false);
		    Ext.getCmp(tabId+'_openperiod').setDisabled(true);
		    Ext.getCmp(tabId+'_update').setDisabled(false);
	    }
	    else
	    {
	        if ((grid.getSelectionModel().getSelected().get("psts") == PeriodStatus.Closed))
	        {
		        Ext.getCmp(tabId+'_closeperiod').setDisabled(true);
		        Ext.getCmp(tabId+'_update').setDisabled(true);
		        Ext.getCmp(tabId+'_openperiod').setDisabled(false);
	        }
	        else
	        {
		        Ext.getCmp(tabId+'_closeperiod').setDisabled(true);
		        Ext.getCmp(tabId+'_update').setDisabled(false);
		        Ext.getCmp(tabId+'_openperiod').setDisabled(false);
		    }
	    }
	}
}

Ext.onReady(function(){ 
//    var cp = new Ext.state.CookieProvider({
//       expires: new Date(new Date().getTime()+(1000*60*60*24*10*365))
//    });
//    Ext.state.Manager.setProvider(cp);
    new PeriodPanel();
})

</script>