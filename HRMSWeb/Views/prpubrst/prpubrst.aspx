<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="prpubrst.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.prpubrst.prpubrst" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var prpubrstConfig=Ext.decode('<%=ViewData["config"] %>'); 
prpubrstConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var prpubrstPageName = 'prpubrst';
var prpubrstKeyColumn='rscd';
            
var prpubrstQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	prpubrstQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(prpubrstQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Payroll_Label_rscd,
                        name: 'rscd',stateful:false,anchor:'95%',UpperOnBlur:'true'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Payroll_Label_rsnm,
                        name: 'rsnm',stateful:false,anchor:'95%'}]}
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


var prrelItemEditWindow=function(rlcd,itcd,selsal){
    this.rlcd = rlcd;
    this.itcd = itcd;
    this.selsal = selsal;
	this.init();	

    this.fullFormPanel = {
                xtype:'panel',
	            layout:'fit',
	            items:[{
		            xtype:'panel',
		            layout:'fit',
		            autoHeight:true,
		            bodyStyle:'padding:0px;',
		            border:false,
		            baseCls:'x-fieldset-noborder',
		            columnWidth: .50,
		            items:this.relitemgrid
	            }]
            };
                       
	prrelItemEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:500, 
        height:400, 
        title:'Items to summary',
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.fullFormPanel,
        listeners:{
            show:function(){    
               this.relitemgrid.store.reload();
            },
            scope:this
        },
        buttons: [{ 
            text:HRMSRes.Public_Button_Confirm, 
            iconCls:'icon-save', 
            handler: this.confirm,
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

Ext.extend(prrelItemEditWindow,Ext.Window,{
    init:function(){
		this.relitemgrid = this.createGridPanel();
	},

    createGridPanel:function(){
		var prpubrstDtlStoreType=Ext.data.Record.create([
		    {name:'sele'},
            {name:'sqno'},
            {name:'itcd'},
            {name:'itnm'},
            {name:'srno'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},prpubrstDtlStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:'rlcd',ColumnValue:this.rlcd},{ColumnName:'rscd',ColumnValue:this.itcd}]})},   
	   		url:ContextInfo.contextPath+'/' + prpubrstPageName + '.mvc/getRelItem',
	   		listeners:{
                loadexception:function(o,t,response)
                {
                    var o= Ext.util.JSON.decode(response.responseText);		   		
                    this.relitemgrid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                load:function(t,r){
                    for (var i=0;i<t.getCount();i++)
                    {
                        if (t.getAt(i).get('sele')=='Y'){
                            this.relitemgrid.getSelectionModel().selectRow(i,true);
                        }

                        if (this.selsal.trim()!=''){
                            if (this.selsal.indexOf('{' + t.getAt(i).get('itcd') +'}')>0 )
                                this.relitemgrid.getSelectionModel().selectRow(i,true);
                            else
                                this.relitemgrid.getSelectionModel().deselectRow(i);
                        }
                    }
                },
                scope:this
            }
        });

        var params={
            start:0,
            limit:1000
        };
        
        var scm=new Ext.grid.CheckboxSelectionModel({sortable:true,singleSelect :false});
                       
        return new Ext.grid.EditorGridPanel({
            id:'pubrstrelitemgrid',
    		border:true, 
    		monitorResize:false, 
            height:330,
            loadMask:true,  		            
            ds: store, 
            frame:false,
            collapsible: true,
            animCollapse: false,
            editable:true,
            clicksToEdit:1,
            viewConfig: { 
		        forceFit: true 
		    }, 
            listeners:{
	            rowclick:function(g,r){
	                //this.controlButton(this.tabId);
	            },
	            scope:this
            },       
            sm: scm,               
            cm: new Ext.grid.ColumnModel([ 
            scm,
            {header:HRMSRes.Public_Label_sqno,id:'sqno',sortable: true, dataIndex: 'sqno',width:30},
            {header:HRMSRes.Payroll_Label_itcd,id:'itcd',sortable: true, dataIndex: 'itcd',width:100},
            {header:HRMSRes.Payroll_Label_itnm,id:'itnm',sortable: true, dataIndex: 'itnm'},
            {header:'srno',id:'srno',sortable: true, dataIndex: 'vtyp',width:60}
            ]),
            bbar: new Ext.PagingToolbar({
                pageSize:1000,//Pagination.pagingSize,
                store: store,
                displayInfo: true,
                displayMsg:HRMSRes.Public_PagingToolbar_Total+':{1}/{2}',
                emptyMsg: HRMSRes.Public_PagingToolbar_EmptyMsg                    
            })
	    })
	},
	
	confirm: function(){
	    var sel = this.relitemgrid.getSelectionModel().getSelections();
	    var selsal = '';
        for (var i=0;i<sel.length;i++)
        {
            selsal = selsal + (selsal==''?'':',') + '{' + sel[i].get('itcd') + '}';
        }
        var f = Ext.getCmp(this.tabId + 'valu');
        f.setValue(selsal);
        this.close();
	}

});


var prpubrstDtlEditWindow=function(rec,store,flag){
	this.feedbackStore=store;
	this.isNewDtl = flag;
	this.feedbackRec = rec;
	this.init();	

	prpubrstDtlEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:320, 
        title:HRMSRes.Payroll_Label_rsit,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.basisDtlEditFormPanel,
        listeners:{
            show:function(){              
		        var data = rec;
                this.basisDtlEditForm.items.each(function(f){
                    if(f.isFormField){
                        var value=data.get(f.getName());
                        if (f.xtype=='timefield') value = formatTime(value);
                        if (f.xtype=='datefield') value = formatDateNoTime(value);
                        f.setValue(value);   
                    }
                });   
                
                this.originalTriggerScript = this.basisDtlEditForm.findField('vtyp').onTriggerClick;
                this.originalFormulaStore = this.basisDtlEditForm.findField('vtyp').store;
                setLastModifiedInfo(prpubrstConfig,this.basisDtlEditForm);
                
                var f1 = this.basisDtlEditForm.findField('vtyp');  
                var f2 = this.basisDtlEditForm.findField('valu');  
                
                //if (f1.getValue()==ValueType.Sum || f1.getValue()==ValueType.Formula)
                //    f2.setDisabled(true);
                //else
                //    f2.setDisabled(false);
                    

            },
            scope:this
        },
        buttons: [{ 
            text:HRMSRes.Public_Button_Confirm, 
            iconCls:'icon-accept', 
            handler: this.confirm,
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

Ext.extend(prpubrstDtlEditWindow,Ext.Window,{
    init:function(){
		this.basisDtlEditFormPanel=this.createBasisDtlEditFormPanel();
		this.basisDtlEditForm=this.basisDtlEditFormPanel.getForm();
	},
	
	createBasisDtlEditFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
      		 items: [
      		    {},{
      		    layout:'column',
      		    items:[
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sqno+'(<font color=red>*</font>)',
                        name: 'sqno',allowBlank:false,disabled:true,stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_itcd + '(<font color=red>*</font>)',
                        name: 'itcd',id:this.tabId + 'itcd',stateful:false,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['DisplayField','ValueField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getSalaryItems',
		                    listeners:{
                                load:function(t,r){
                                        f = this.basisDtlEditForm.findField('itcd');
                                        var v = f.getValue();f.setValue(v);
                                },scope:this}}),
                            listeners:{
                                select:function(c,r,i){
                                },
                                scope:this
                            }
    		          }]},
      		        
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_crcd +'(<font color=red>*</font>)',
                        name: 'crcd',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['DisplayField','ValueField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getCurrency',
		                    listeners:{
                                load:function(){f = this.basisDtlEditForm.findField('crcd');f.setValue(f.getValue());},
                                scope:this}})
    		          }]},
     		        
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_vtyp +'(<font color=red>*</font>)',
                        name: 'vtyp',id: this.tabId + 'vtyp',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: RuleSetValueTypeStore,
                        listeners:{
                            select:function(c,r,i){
                                  var f = this.basisDtlEditForm.findField('valu');  
                                  if (c.getValue()==ValueType.Formula) {
                                      //f.store.loadData(this.originalFormulaStore.data);
                                      //f.setDisabled(false);   
                                      f.store.reload();
                                  }
                                  else if (c.getValue()==ValueType.Sum){    
                                       //f.setDisabled(true);  
                                       f.store.removeAll();
                                  }else{
                                       //f.setDisabled(false);   
                                       f.store.removeAll();
                                  }
                            },
                            scope:this
                        }
    		          }]},

                    {columnWidth:1,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_valu +'(<font color=red>*</font>)',
                        name: 'valu',id:this.tabId + 'valu',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'97.5%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['DisplayField','ValueField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getFormula',
		                    listeners:{
                                load:function(p){
                                f = this.basisDtlEditForm.findField('valu');f.setValue(f.getValue());},
                                scope:this}}),
                        onTriggerClick:this.triggerClick
    		          }]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_efdt+'(<font color=red>*</font>)',id:'efdt',
                        name:'efdt',editable:false,height:22,anchor:'95%',allowBlank:false,
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,disabled:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_exdt,id:'exdt',
                        name:'exdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,disabled:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

//      		        {columnWidth:.5,layout: 'form',
//                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Payroll_Label_cred,
//                        name: 'cred',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_pdcd +'(<font color=red>*</font>)',
                        name: 'pdcd',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField', valueField: 'ValueField', 
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['DisplayField','ValueField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getPayDay',
		                    listeners:{
                                load:function(){f = this.basisDtlEditForm.findField('pdcd');f.setValue(f.getValue());},
                                scope:this}})
    		          }]},

      		        {columnWidth:.0,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Payroll_Label_rlcd,hidden:true,hideLabel:true,
                        name: 'rlcd',id: this.tabId + 'rlcd',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_isca ,
                        name: 'isca',stateful:false,editable:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

//      		        {columnWidth:.5,layout: 'form',
//                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Payroll_Label_ssty,
//                        name: 'ssty',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Payroll_Label_rlcd,
                        name: 'rlcd',stateful:false,anchor:'95%',disabled:true}]},

     		        {columnWidth: .5, layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_lmtm,id:'itmlmtm',
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
	confirm:function(){
		var params={};
		this.basisDtlEditForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });
	    var f1 = this.basisDtlEditForm.findField('vtyp');
	    var f2 = this.basisDtlEditForm.findField('valu');
	    //alert(this.basisDtlEditForm.findField('valu').getRawValue());
	    if (f1.getValue()==ValueType.Value)
	        params['valu'] = f2.getRawValue();
	    
        f2 = this.basisDtlEditForm.findField('itcd');
	    params['itnm'] = f2.getRawValue();

	    if (this.isNewDtl) {
	        this.feedbackStore.add(new this.feedbackStore.recordType(params));
	    }
        else {
            this.feedbackRec.set('sqno',params['sqno']);
            this.feedbackRec.set('itcd',params['itcd']);
            this.feedbackRec.set('itnm',params['itnm']);
            this.feedbackRec.set('rscd',params['rscd']);
            this.feedbackRec.set('rsnm',params['rsnm']);
            this.feedbackRec.set('efdt',params['efdt']);
            this.feedbackRec.set('rlcd',params['rlcd']);
            this.feedbackRec.set('vtyp',params['vtyp']);
            this.feedbackRec.set('valu',params['valu']);
            this.feedbackRec.set('cred',params['cred']);
            this.feedbackRec.set('pdcd',params['pdcd']);
            this.feedbackRec.set('crcd',params['crcd']);
            this.feedbackRec.set('crnm',params['crnm']);
            this.feedbackRec.set('exdt',params['exdt']);
            this.feedbackRec.set('isca',params['isca']);
            this.feedbackRec.set('ssty',params['ssty']);
            this.feedbackRec.set('lmtm',params['lmtm']);
            this.feedbackRec.set('lmur',params['lmur']);
        }
	    this.close();
	},
	triggerClick:function()
	{
	    
	    var f = Ext.getCmp(this.tabId + 'vtyp');
	    if (f.getValue()=='Formula') {
            if(this.disabled){
                return;
            }
            if(this.isExpanded()){
                this.collapse();
                this.el.focus();
            }else {
                this.onFocus({});
                if(this.triggerAction == 'all') {
                    this.doQuery(this.allQuery, true);
                } else {
                    this.doQuery(this.getRawValue());
                }
                this.el.focus();
            }
	    };
	    
	    if (f.getValue()=='Sum') {
	        var f1 = Ext.getCmp(this.tabId + 'rlcd').getValue();
	        var f2 = Ext.getCmp(this.tabId + 'itcd').getValue();
	        var f3 = Ext.getCmp(this.tabId + 'valu').getValue();
	        if (f1 == null) f1 = '';
	        if (f2 == null) f2 = '';
	        if (f3 == null) f3 = '';
	        new prrelItemEditWindow(f1,f2,f3).show();
	    };

	}

});

var prpubrstEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	

    this.fullFormPanel = {
                xtype:'panel',
	            layout:'fit',
	            items:[{
		            xtype:'panel',
		            autoHeight:true,
		            bodyStyle:'padding:0px;',
		            border:false,
		            baseCls:'x-fieldset-noborder',
		            columnWidth: .50,
		            items:this.basisFormPanel
	            },{
		            xtype:'panel',
		            layout:'fit',
		            autoHeight:true,
		            bodyStyle:'padding:0px;',
		            border:false,
		            baseCls:'x-fieldset-noborder',
		            columnWidth: .50,
		            tbar: [{ 
	        	            id:this.tabId+'_adddtl',
	                        iconCls:'icon-add', 
	                        text: HRMSRes.Public_Toolbar_Add, 
	                        handler: function(){
	                            this.addline();	                         
	                        }, 
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_editdtl',
	                        iconCls:'icon-update', 
	                        text: HRMSRes.Public_Toolbar_Edit, 
	                        disabled:true,
	                        handler: function(){
	                            this.editline();	                         
	                        }, 
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_deletedtl',
	                        iconCls:'icon-remove', 
	                        text:HRMSRes.Public_Toolbar_Delete, 
	                        handler: this.deleteline, 
	                        disabled:true,
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_exceldtl',
	                        iconCls:'icon-export', 
	                        text:HRMSRes.Public_Toolbar_ToExcel, 
	                        handler: this.exportExcelLine, 
	                        scope: this 
	                    },'->',{ 
	        	            id:this.tabId+'_updtl',
	                        iconCls:'icon-up', 
	                        text:HRMSRes.Public_Toolbar_Up, 
	                        handler: this.upline, 
	                        disabled:true,
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_downdtl',
	                        iconCls:'icon-down', 
	                        text:HRMSRes.Public_Toolbar_Down, 
	                        handler: this.downline, 
	                        disabled:true,
	                        scope: this 
	                    }],
		            items:this.dtlgrid
	            }]
            };
                       
	prpubrstEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:750, 
        height:490, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.fullFormPanel,
        listeners:{
            show:function(){    
		        var keyField = this.basisForm.findField(prpubrstKeyColumn);
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

                setLastModifiedInfo(prpubrstConfig,this.basisForm);
                
                if (!this.isNew)
                   this.Query();
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

Ext.extend(prpubrstEditWindow,Ext.Window,{
    init:function(){
		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();
		this.dtlgrid = this.createGridPanel();
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Payroll_Label_rscd + '(<font color=red>*</font>)',
                     allowBlank:false,name: 'rscd',id: this.tabId + 'rscd',stateful:false,anchor:'95%',disabled:!this.isNew}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Payroll_Label_rsnm + '(<font color=red>*</font>)',allowBlank:false,
                        name: 'rsnm',stateful:false,anchor:'95%'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Public_Label_remk,height:50,
                        name: 'remk',stateful:false,anchor:'95%'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_lmtm,id:'lmtm',
                        name:'lmtm',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,disabled:true,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_lmur,disabled:true,
                        name: 'lmur',stateful:false,anchor:'95%'}]}     		        
      		    ]
      		 }] 
       })
	},
	
    createGridPanel:function(){
		var prpubrstDtlStoreType=Ext.data.Record.create([
            {name:'sqno'},
            {name:'itcd'},
            {name:'itnm'},
            {name:'rscd'},
            {name:'rsnm'},
            {name:'efdt'},
            {name:'rlcd'},
            {name:'vtyp'},
            {name:'valu'},
            {name:'cred'},
            {name:'pdcd'},
            {name:'exdt'},
            {name:'isca'},
            {name:'ssty'},
            {name:'crcd'},
            {name:'crnm'},
            {name:'lmtm'},
            {name:'lmur'}
		]);
        
		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},prpubrstDtlStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:prpubrstKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + prpubrstPageName + '.mvc/getPubRuleSetDtl',
	   		listeners:{
                loadexception:function(o,t,response)
                {
                    var o= Ext.util.JSON.decode(response.responseText);		   		
                    this.dtlgrid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                load:function(){
                    this.controlButton(this.tabId);
                },
                scope:this
            }
        });
        //alert(store.url);
        var params={
            start:0,
            limit:Pagination.pagingSize
        };
               
        return new Ext.grid.EditorGridPanel({
            id:'prpubrstDtlGrid',
    		border:true, 
    		monitorResize:false, 
            height:295,
            loadMask:true,  		            
            ds: store, 
            frame:false,
            collapsible: true,
            animCollapse: false,
            editable:true,
            clicksToEdit:1,
            viewConfig: { 
		        forceFit: true 
		    }, 
            listeners:{
	            rowclick:function(g,r){
	                this.controlButton(this.tabId);
	            },
	            scope:this
            },       
            sm: new Ext.grid.RowSelectionModel(),               
            cm: new Ext.grid.ColumnModel([ 
            {header:HRMSRes.Public_Label_sqno,id:'sqno',sortable: true, dataIndex: 'sqno',width:30},
            {header:HRMSRes.Payroll_Label_itcd,id:'itcd',sortable: true, dataIndex: 'itcd',width:100},
            {header:HRMSRes.Payroll_Label_itnm,id:'itnm',sortable: true, dataIndex: 'itnm'},
            {header:HRMSRes.Payroll_Label_vtyp,id:'vtyp',sortable: true, dataIndex: 'vtyp',width:60},
            {header:HRMSRes.Payroll_Label_valu,id:'valu',sortable: true, dataIndex: 'valu'},
            {header:HRMSRes.Public_Label_efdt,id:'efdt',sortable: true, dataIndex: 'efdt',renderer:formatDateNoTime,width:60},
            {header:HRMSRes.Public_Label_exdt,id:'exdt',sortable: true, dataIndex: 'exdt',renderer:formatDateNoTime,width:60},
            {header:HRMSRes.Public_Label_lmtm,id:'lmtm',sortable: true, dataIndex: 'lmtm',renderer:formatDate,hidden:true},
            {header:HRMSRes.Public_Label_lmur,id:'lmur',sortable: true, dataIndex: 'lmur',hidden:true}
            ]),
            bbar: new Ext.PagingToolbar({
                pageSize:1000,//Pagination.pagingSize,
                store: store,
                displayInfo: true,
                displayMsg:HRMSRes.Public_PagingToolbar_Total+':{1}/{2}',
                emptyMsg: HRMSRes.Public_PagingToolbar_EmptyMsg                    
            })
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
        
        var dtlparams=[];       
        var st = this.dtlgrid.getStore();
        //alert( st.getCount());
        for (var i =1; i<= st.getCount();i++ ){
            var x = st.getAt(i-1);
            var p = {rscd:this.basisForm.findField('rscd').getValue(),
                     sqno:x.get('sqno'),itcd:x.get('itcd'),
                     efdt:x.get('efdt'),rlcd:x.get('rlcd'),
                     vtyp:x.get('vtyp'),valu:x.get('valu'),
                     cred:x.get('cred'),pdcd:x.get('pdcd'),exdt:x.get('exdt'),
                     isca:x.get('isca'),ssty:x.get('ssty'),crcd:x.get('crcd'),
                     lmtm:x.get('lmtm'),lmur:x.get('lmur')};
            
            dtlparams[dtlparams.length] = p;
        }
        
        var keyparams=[];
        keyparams[0]={ColumnName:'rscd',ColumnValue:this.basisForm.findField('rscd').getValue()};
        //keyparams[1]={ColumnName:'dasq',ColumnValue:this.basisForm.findField('dasq').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + prpubrstPageName + '.mvc/'+method,
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
		   params: {record:Ext.encode({params:params,keycolumns:keyparams,dtlparams:dtlparams})}
		});
	},
	Query:function(){	    
        var params=[];	
        var f = this.basisForm.findField(prpubrstKeyColumn);        
        var p={
            ColumnName:f.getName(),
            ColumnValue:f.getValue()                
        };

        params[params.length]=p;                  
        //params[params.length]={ColumnName:'ActionType',ColumnValue:'Query'};                  

        var loadParams={start:0,limit:1000};//Pagination.pagingSize
        /***modified for adjquery**/
        this.dtlgrid.queryParams={
            params:params
        };
        this.dtlgrid.store.baseParams={record:Ext.util.JSON.encode(this.dtlgrid.queryParams),action:'query'};
	    this.dtlgrid.getStore().load({params:loadParams});
    },
    addline:function(){	    
        var n=this.dtlgrid.getStore().getCount() + 1;
        var params=[];
        params['sqno']= n.toString();
        params['itcd']='';
        params['itnm']='';
        params['rscd']=this.basisForm.findField('rscd').getValue();
        params['rsnm']=this.basisForm.findField('rsnm').getValue();
        params['efdt']='';
        params['rlcd']='';
        params['vtyp']='';
        params['valu']='';
        params['cred']='';
        params['pdcd'] = ContextInfo.sysCfg['PrDFPD'];
        params['crcd'] = ContextInfo.sysCfg['PrDFCUR']; ;
        params['crnm']='';
        params['exdt']='';
        params['isca']='';
        params['ssty']='';
        params['lmtm']= formatDateTime(new Date());
        params['lmur']=prpubrstConfig.currentUser;

        var store = this.dtlgrid.getStore();
        new prpubrstDtlEditWindow(new store.recordType(params),store,true).show();
    },
    editline:function(){	    
        var n=this.dtlgrid.getStore().getCount() + 1;
        var rec = this.dtlgrid.getSelectionModel().getSelected();
        var store = this.dtlgrid.getStore();    
        new prpubrstDtlEditWindow(rec,store,false).show();
    },
	deleteline:function(){
	    var sm=this.dtlgrid.getSelectionModel();
	    var record=sm.getSelected();
        
        this.dtlgrid.store.remove(record);
        this.dtlgrid.store.totalLength-=1;
        this.dtlgrid.getBottomToolbar().updateInfo();   
        
        this.resortdasq();
        
        this.controlButton(this.tabId);
	},
	upline:function(){
	    var sm=this.dtlgrid.getSelectionModel();
	    var record=sm.getSelected();
        var idx = this.dtlgrid.store.indexOf(record);
        
        if (idx>0) {
            this.dtlgrid.store.remove(record);        
            this.dtlgrid.store.insert(idx-1,record);
            sm.selectRange(idx-1,idx-1);
        }else{
            sm.selectRange(idx,idx);
        }
        
        this.resortdasq();
        this.dtlgrid.store.commitChanges();
        
        this.controlButton(this.tabId);
	},
    downline:function(){
	    var sm=this.dtlgrid.getSelectionModel();
	    var record=sm.getSelected();
        var idx = this.dtlgrid.store.indexOf(record);
        
        if (idx<this.dtlgrid.store.totalLength-1) {
            this.dtlgrid.store.remove(record);        
            this.dtlgrid.store.insert(idx+1,record);
            sm.selectRange(idx+1,idx+1);
        }else{
            sm.selectRange(idx,idx);
        }
        
        this.resortdasq();
        this.dtlgrid.store.commitChanges();
        
        this.controlButton(this.tabId);
	},	
	exportExcelLine:function(){
	    if (this.dtlgrid.getStore().getTotalCount()<=0){
	        this.dtlgrid.getBottomToolbar().diplayMsg.update('<font color=red>' + HRMSRes.Public_Message_ExcelNoRecord + '</font>');
	        return;
	    }
	    
	    var cm=this.dtlgrid.getColumnModel();
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
	    var params={record:Ext.encode({params:[{ColumnName:prpubrstKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.dtlgrid.queryParams){
            this.dtlgrid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.dtlgrid.queryParams);
            delete this.dtlgrid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + prpubrstPageName + '.mvc/exportexceldtl';
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
	},		
	controlButton:function(id){
        var enabled=!this.dtlgrid.getSelectionModel().hasSelection();
        Ext.getCmp(id+ '_editdtl').setDisabled(enabled);
        Ext.getCmp(id+ '_deletedtl').setDisabled(enabled);
        Ext.getCmp(id+ '_updtl').setDisabled(enabled);
        Ext.getCmp(id+ '_downdtl').setDisabled(enabled);
    },
    resortdasq:function(){
        var st = this.dtlgrid.getStore();
        for (var i =1; i<= st.getCount();i++ ){
            st.getAt(i-1).set('sqno',i.toString());
        }
    }    
});


var prpubrstPanel=function(){
    this.tabId=prpubrstConfig.tabId;
	this.init();	
	
	prpubrstPanel.superclass.constructor.call(this,{
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
	            hidden:prpubrstConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new prpubrstEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden:prpubrstConfig.auth[this.tabId+'_' + 'edit']!='True',
	            handler: function(){
	            	new prpubrstEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:prpubrstConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:prpubrstConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new prpubrstQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:prpubrstConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:prpubrstConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,prpubrstConfig.muf?'delete':'add',prpubrstConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(prpubrstPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var prpubrstStoreType=Ext.data.Record.create([
            {name:'rscd'},
            {name:'rsnm'},
            {name:'remk'},
            {name:'lmtm'},
            {name:'lmur'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},prpubrstStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:prpubrstKeyColumn,ColumnValue:prpubrstConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + prpubrstPageName + '.mvc/list',
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
                {header:HRMSRes.Payroll_Label_rscd,sortable: true, dataIndex: 'rscd',hidden:false},
                {header:HRMSRes.Payroll_Label_rsnm,sortable: true, dataIndex: 'rsnm',hidden:false},
                {header:HRMSRes.Public_Label_remk,sortable: true, dataIndex: 'remk',hidden:false},
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
        keyparams[0]={ColumnName:'rscd',ColumnValue:record.get('rscd')};
        
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ prpubrstPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:prpubrstKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + prpubrstPageName + '.mvc/exportexcel';
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
    new prpubrstPanel();
})

    </script>

</body>
</html>