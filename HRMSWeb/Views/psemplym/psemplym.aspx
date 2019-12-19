<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="psemplym.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.psemplym.psemplym" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var psemplymConfig=Ext.decode('<%=ViewData["config"] %>'); 
psemplymConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var psemplymPageName = 'psemplym';
var psemplymKeyColumn='emno';
            
var psemplymQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	psemplymQueryWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:400, 
        height:250, 
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

Ext.extend(psemplymQueryWindow,Ext.Window,{
    init:function(){
		this.formPanel=this.createFormPanel();
		this.form=this.formPanel.getForm();
	},
	
	createFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:200,
	         header:true,
             items: [
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sfid,
                        name: 'sfid',stateful:false,anchor:'95%'}]},
            
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

var psemplymEditWindow=function(grid,isNew,action){
	this.grid=grid;
	this.editAction = action;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	
    
    this.employmentTab=new Ext.TabPanel({
                autoTabs:true,
                activeTab:0,
                border:false,
                layoutOnTabChange:true,
                frame:true,
                autoHeight:true,
                id:'employmentTab',
                items:[{id:'tab1',title: HRMSRes.Personal_Label_Basis,items: this.basisFormPanel},
                       {id:'tab3',title: HRMSRes.Personal_Label_Others,items: this.othersFormPanel},
                       {id:'tab2',title: HRMSRes.Personal_Label_termination,items: this.terminationFormPanel}]}),
    
                       
	psemplymEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:380, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.employmentTab,
        listeners:{
            show:function(){              
		        var keyField = this.basisForm.findField(psemplymKeyColumn);
                if(!this.isNew){	
			        var data=this.grid.getSelectionModel().getSelected();
                    this.employmentTab.setActiveTab('tab2');
                    this.terminationForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            if (f.xtype=='timefield')
                                value = formatTime(value);
                            
                            if (f.xtype=='datefield')
                                value = formatDateNoTime(value);
                                    
                            f.setValue(value);                                               	            
                        }
                    });   
                    this.employmentTab.setActiveTab('tab3');
                    this.othersForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            if (f.xtype=='timefield')
                                value = formatTime(value);
                            
                            if (f.xtype=='datefield')
                                value = formatDateNoTime(value);
                                    
                            f.setValue(value);                                               	            
                        }
                    });   
                    this.employmentTab.setActiveTab('tab1');
                    this.basisForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            if (f.xtype=='timefield')
                                value = formatTime(value);
                            
                            if (f.xtype=='datefield')
                                value = formatDateNoTime(value);
                                    
                            f.setValue(value);                                               	            
                        }
                    });  
		        }
		        else
		        {
		            this.employmentTab.setActiveTab('tab2');
		            this.employmentTab.setActiveTab('tab3');
		            this.employmentTab.setActiveTab('tab1');

		            keyField.setValue(psemplymConfig.emno);

		        }
		        
		        psemplymConfig.emno='';
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

Ext.extend(psemplymEditWindow,Ext.Window,{
    init:function(){
		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();

		this.terminationFormPanel=this.createTerminationFormPanel();
		this.terminationForm = this.terminationFormPanel.getForm();

		this.othersFormPanel=this.createOthersFormPanel();
		this.othersForm = this.othersFormPanel.getForm();
	},
	createBasisFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         title:'Basic',
      		 items: [{
      		    layout:'column',
      		    items:[
                    {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_emno+'(<font color=red>*</font>)',
                        name: 'emno',allowBlank:false,disabled:true,stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sftn,id:'stfn',
                        name: 'stfn',disabled:true,stateful:false,anchor:'95%'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Personal_Label_jidt+'(<font color=red>*</font>)',id:'joindate',
                        name:'jidt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,allowBlank:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Personal_Label_prdt +'(<font color=red>*</font>)',id:'probationdate',
                        name:'prdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,allowBlank:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_sftm + '(<font color=red>*</font>)',
                        name: 'stcd',stateful:false,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getStaffType',
		                    listeners:{load:function(){var f=this.basisForm.findField('stcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',id:'emst',fieldLabel:HRMSRes.Personal_Label_emst + '(<font color=red>*</font>)',
                        name: 'emst',stateful:false,allowBlank:false,typeAhead: true,disabled:true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: EmpStateStore
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_dvnm ,
                        name: 'dvcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getDivision',
		                    listeners:{load:function(){var f=this.basisForm.findField('dvcd');f.setValue(f.getValue())},scope:this}})
    		          }]},


                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_bsnm ,
                        name: 'bscd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getBusiness',
		                    listeners:{load:function(){var f=this.basisForm.findField('bscd');f.setValue(f.getValue())},scope:this}})
    		          }]},


                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_dpnm ,
                        name: 'dpcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getDepartment',
		                    listeners:{load:function(){var f=this.basisForm.findField('dpcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_grnm ,
                        name: 'grcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getGrade',
		                    listeners:{load:function(){var f=this.basisForm.findField('grcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_glnm ,
                        name: 'glcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getGradeLevel',
		                    listeners:{load:function(){var f=this.basisForm.findField('glcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_psnm ,
                        name: 'pscd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getPosition',
		                    listeners:{load:function(){var f=this.basisForm.findField('pscd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_pslm ,
                        name: 'plcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getPositionLevel',
		                    listeners:{load:function(){var f=this.basisForm.findField('plcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_jcnm,
                        name: 'jccd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getJobClass',
		                    listeners:{load:function(){var f=this.basisForm.findField('jccd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_jtnm ,
                        name: 'jtcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getJobType',
		                    listeners:{load:function(){var f=this.basisForm.findField('jtcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_wgnm ,
                        name: 'wgcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getWorkGroup',
		                    listeners:{load:function(){var f=this.basisForm.findField('wgcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_wsnm ,
                        name: 'wscd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getWorkSite',
		                    listeners:{load:function(){var f=this.basisForm.findField('wscd');f.setValue(f.getValue())},scope:this}})
    		          }]}
      		    ]
      		 }] 
       })
	},
	createOthersFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         title:'Others',
      		 items: [{
      		    layout:'column',
      		    items:[
                    {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_titl,
                        name: 'titl',stateful:false,anchor:'95%'}]},
      		        
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_ccnm ,
                        name: 'cccd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getCostCenter',
		                    listeners:{load:function(){var f=this.othersForm.findField('cccd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_scnm ,
                        name: 'sccd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getSC',
		                    listeners:{load:function(){var f=this.othersForm.findField('sccd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Attendance_Label_clcd ,
                        name: 'clcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getCalendar',
		                    listeners:{load:function(){var f=this.othersForm.findField('clcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_ptnm ,
                        name: 'ptcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getPayType',
		                    listeners:{load:function(){var f=this.othersForm.findField('ptcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Attendance_Label_rsnm ,
                        name: 'rscd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getRoster',
		                    listeners:{load:function(){var f=this.othersForm.findField('rscd');f.setValue(f.getValue())},scope:this}})
    		          }]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Personal_Label_rrdt,id:'rtdt',
                        name:'rtdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_reto,
                        name: 'reto',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_cbno,
                        name: 'cbno',stateful:false,anchor:'95%'}]},

       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Public_Label_remk,height:70,
                        name: 'remk',stateful:false,anchor:'95%'}]},

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
	createTerminationFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         title:'Termination',
      		 items: [{
      		    layout:'column',
      		    items:[
      		    
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_trnm ,
                        name: 'trcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getTermReason',
		                    listeners:{load:function(){var f=this.terminationForm.findField('trcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_ttnm ,
                        name: 'ttcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getTermType',
		                    listeners:{load:function(){var f=this.terminationForm.findField('ttcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Personal_Label_tmdt,id:'tmdt',
                        name:'tmdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},     		    
      		    
                    {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_rsid,
                        name: 'rsid',stateful:false,anchor:'95%'}]},
      		        
     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Personal_Label_ssdt,id:'ssdt',
                        name:'ssdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_isbl ,
                        name: 'isbl',stateful:false,editable:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_isrh ,
                        name: 'isrh',stateful:false,editable:false,typeAhead: true,
                        isrh: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Personal_Label_tmrm,height:70,
                        name: 'tmrm',stateful:false,anchor:'98%'}]}

      		    ]
      		 }] 
       })
	},
	save: function(){
		if(!this.basisForm.isValid()) return;
		this.grid.getBottomToolbar().diplayMsg.update('');
		var params={};
		this.basisForm.items.each(function(f){
            if((f.isFormField) && (f.getId()!='stfn')){
                params[f.getName()]=f.getValue();
            }
        });
		this.terminationForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });
		this.othersForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + psemplymPageName + '.mvc/'+method,
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
		   params: {record:Ext.util.JSON.encode(params),action:this.editAction}
		});
	}
});


var psemplymPanel=function(){
    this.tabId=psemplymConfig.tabId;
	this.init();	

	var relatedFuncMenuEmploy = new Ext.menu.Menu({
    items: [
        {text: HRMSRes.Public_Menu_psperson,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/psperson.mvc/index?menuId=M2010&helpId=psperson&emno=' + emno,'M2010','',HRMSRes.Public_Menu_psperson,'','true');},scope:this},      
        {text: HRMSRes.Public_Menu_pscontct,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/pscontct.mvc/index?menuId=M2030&helpId=pscontct&emno=' + emno,'M2030','',HRMSRes.Public_Menu_pscontct,'','true');},scope:this},       
        {text: HRMSRes.Public_Menu_psaddres,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/psaddres.mvc/index?menuId=M2020&helpId=psaddres&emno=' + emno,'M2020','',HRMSRes.Public_Menu_psaddres,'','true');},scope:this},      
        {text: HRMSRes.Public_Menu_pseductn,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/pseductn.mvc/index?menuId=M2040&helpId=pseductn&emno=' + emno,'M2040','',HRMSRes.Public_Menu_pseductn,'','true');},scope:this},    
        {text: HRMSRes.Public_Menu_pscertfc,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/pscertfc.mvc/index?menuId=M2050&helpId=pscertfc&emno=' + emno,'M2050','',HRMSRes.Public_Menu_pscertfc,'','true');},scope:this},     
        {text: HRMSRes.Public_Menu_psquafca,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/psquafca.mvc/index?menuId=M2100&helpId=psquafca&emno=' + emno,'M2100','',HRMSRes.Public_Menu_psquafca,'','true');},scope:this},      
        {text: HRMSRes.Public_Menu_psexprnc,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/psexprnc.mvc/index?menuId=M2150&helpId=psexprnc&emno=' + emno,'M2150','',HRMSRes.Public_Menu_psexprnc,'','true');},scope:this},   
        {text: HRMSRes.Public_Menu_psrelshp,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/psrelshp.mvc/index?menuId=M2160&helpId=psrelshp&emno=' + emno,'M2160','',HRMSRes.Public_Menu_psrelshp,'','true');},scope:this},  
        {text: HRMSRes.Public_Menu_psaccont,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/psaccont.mvc/index?menuId=M2070&helpId=psaccont&emno=' + emno,'M2070','',HRMSRes.Public_Menu_psaccont,'','true');},scope:this}, 
        //{text: HRMSRes.'Skill',handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/masterdata.mvc/index?tableName=tbswlftyp&menuId=M1610&helpId=bswlftyp&emno=' + emno,'M1610','',HRMSRes.Public_Menu_bswlftyp,'','true');},scope:this},   
        {text: HRMSRes.Public_Menu_psinsurn,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/psinsurn.mvc/index?menuId=M2170&helpId=psinsurn&emno=' + emno,'M2170','',HRMSRes.Public_Menu_psinsurn,'','true');},scope:this},    
        {text: HRMSRes.Public_Menu_pstraing,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/pstraing.mvc/index?menuId=M2110&helpId=pstraing&emno=' + emno,'M2110','',HRMSRes.Public_Menu_pstraing,'','true');},scope:this}, 
        {text: HRMSRes.Public_Menu_psclub,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/psclub.mvc/index?menuId=M2060&helpId=psclub&emno=' + emno,'M2060','',HRMSRes.Public_Menu_psclub,'','true');},scope:this}
    ]});
    
	psemplymPanel.superclass.constructor.call(this,{
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
	            hidden: psemplymConfig.auth[this.tabId + '_' + 'add'] != 'True',
	            disabled:true,
	            handler: function(){
	                new psemplymEditWindow(this.grid,true,'sys$0').show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden: psemplymConfig.auth[this.tabId + '_' + 'edit'] != 'True',
	            disabled:true,
	            handler: function(){
	            	new psemplymEditWindow(this.grid,false,'unknown').show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:psemplymConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this
            }, '-', {
                id: this.tabId + '_query',
                iconCls: 'icon-query',
                text: HRMSRes.Public_Toolbar_Query,
                hidden: psemplymConfig.auth[this.tabId + '_' + 'query'] != 'True',
                handler: function() {
                    new psemplymQueryWindow(this.grid).show();
                },
                scope: this
            }, '-', { 
	        	id:this.tabId+ '_rehire',
	            iconCls:'icon-rehire', 
	            text:HRMSRes.Public_Toolbar_Rehire,
	            hidden:psemplymConfig.auth[this.tabId+'_' + 'rehire']!='True',
	            handler: function(){
	            	new psemplymEditWindow(this.grid,false,'sys$2').show(); 
	            	if (ContextInfo.sysCfg['PsROEIFR']=='Y'){
	            	    Ext.getCmp('emst').setValue('1');
	            	}
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_terminate',
	            iconCls:'icon-terminate', 
	            text:HRMSRes.Public_Toolbar_Terminate,
	            hidden:psemplymConfig.auth[this.tabId+'_' + 'terminate']!='True',
	            handler: function(){
	            	new psemplymEditWindow(this.grid,false,'sys$3').show(); 
	            	Ext.getCmp('employmentTab').setActiveTab('tab2');
	            	Ext.getCmp('emst').setValue('2');
	            }, 
	            scope: this
	        },'-',{ 
                xtype:'combo', 
                id: 'cmbChangeType',
                hidden:psemplymConfig.auth[this.tabId+'_' + 'change']!='True',
                store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getChangeTypeForEmployment'
		                    }),
                displayField: 'DisplayField',valueField: 'ValueField',stateful:false,typeAhead: true,mode: 'local',
                editable:false,triggerAction: 'all', anchor:'95%',emptyText:'Other changes',
                width:200,
                listeners:{
                    select:function(p){
                        new psemplymEditWindow(this.grid,false,p.getValue()).show(); 
	    			},
	    			scope:this
	    		} 
            },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:psemplymConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
	            handler: this.exportExcel, 
	            scope: this 
	        },'-',{ 
	            text:HRMSRes.Public_Toolbar_Related,
	            iconCls:'icon-related',
	            id: 'relatedFuncMenuEmploy',
	            menu: relatedFuncMenuEmploy
	        },'->',{ 
                id:this.tabId+'_logview',
                iconCls:'icon-logview', 
                text:'', 
                handler: function(){
                        var data=this.grid.getSelectionModel().getSelected();if (data==null) return;var rfid = data.get('rfid');new logViewerWindow(rfid).show();},
                scope: this 
            },{ 
                id:this.tabId+'_muf',
                iconCls:psemplymConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){
                        updateMUF(this.tabId,psemplymConfig.muf?'delete':'add',psemplymConfig,this.grid);
                        },
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(psemplymPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},
    
	createGridPanel:function(){
		var psemplymStoreType=Ext.data.Record.create([
	    {name:'emno'},
	    {name:'sfid'},
	    {name:'stfn'},
        {name:'ptcd'},
        {name:'cccd'},
        {name:'pscd'},
        {name:'plcd'},
        {name:'jccd'},
        {name:'stcd'},
        {name:'dvcd'},
        {name:'sccd'},
        {name:'grcd'},
        {name:'dpcd'},
        {name:'ttcd'},
        {name:'bscd'},
        {name:'jtcd'},
        {name:'wscd'},
        {name:'wgcd'},
        {name:'glcd'},
        {name:'clcd'},
        {name:'trcd'},
        {name:'rscd'},
        {name:'jidt'},
        {name:'prdt'},
        {name:'tmdt'},
        {name:'emst'},
        {name:'titl'},
        {name:'rsid'},
        {name:'assc'},
        {name:'rtdt'},
        {name:'ssdt'},
        {name:'reto'},
        {name:'isrh'},
        {name:'isbl'},
        {name:'tmrm'},
        {name:'remk'},
        {name:'lmtm'},
        {name:'lmur'},
        {name:'elnm'},
        {name:'scnm'},
        {name:'bsnm'},
        {name:'psnm'},
        {name:'stnm'},
        {name:'jcnm'},
        {name:'jtnm'},
        {name:'dvnm'},
        {name:'dpnm'},
        {name:'rsnm'},
        {name:'arcd'},
        {name:'arnm'},
        {name:'ptnm'},
        {name:'wsnm'},
        {name:'grnm'},
        {name:'wgnm'},
        {name:'ccnm'},
        {name:'conm'},
        {name:'trnm'},
        {name:'frnm'},
        {name:'elcd'},
        {name:'ttnm'},
        {name:'salu'},
        {name:'napl'},
        {name:'ennm'},
        {name:'cdcd'},
        {name:'rfid'}
        ]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},psemplymStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:psemplymKeyColumn,ColumnValue:psemplymConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + psemplymPageName + '.mvc/list',
	   		listeners:{
                loadexception:function(o,t,response)
                {
                    var o= Ext.util.JSON.decode(response.responseText);		   		
                    this.grid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                load:function(){
                    this.controlButton(this.tabId);
	                if (psemplymConfig.emno!='')
	                {
	                    if (this.grid.getStore().getTotalCount()>0){
                            this.grid.getSelectionModel().selectRow(0,true);
	                        new psemplymEditWindow(this.grid,false,'unknown').show();
	                    }
	                    else{
	                        new psemplymEditWindow(this.grid,true,"$sys0").show();
	                    }
	                }
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
		        //forceFit: false 
		    }, 
            listeners:{
	            rowclick:function(){
	                this.controlButton(this.tabId);
	            },
	            scope:this
            },                      
            cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Personal_Label_emno,sortable: true, dataIndex: 'emno',hidden:false,renderer:empStateRender},
                {header:HRMSRes.Public_Label_sfid,sortable: true, dataIndex: 'sfid',type:'date'},
                {header:HRMSRes.Public_Label_sftn,sortable: true, dataIndex: 'stfn'},
                {header:HRMSRes.Personal_Label_jidt,sortable: true, dataIndex: 'jidt',type:'date',renderer:formatDateNoTime},
                {header:HRMSRes.Personal_Label_prdt,sortable: true, dataIndex: 'prdt',renderer:formatDateNoTime},
                {header:HRMSRes.Personal_Label_sftm,sortable: true, dataIndex: 'stnm'},
                { header: HRMSRes.Personal_Label_emst, sortable: true, dataIndex: 'emst', renderer: empStateRender1 },
                {header:HRMSRes.Master_Label_dpnm,sortable: true, dataIndex: 'dpnm'},
                {header:HRMSRes.Master_Label_grnm,sortable: true, dataIndex: 'grnm'},
                {header:HRMSRes.Master_Label_psnm,sortable: true, dataIndex: 'psnm'},
                {header:HRMSRes.Master_Label_jcnm,sortable: true, dataIndex: 'jcnm'},
                {header:HRMSRes.Attendance_Label_rsnm,sortable: true, dataIndex: 'rsnm'},
                {header:HRMSRes.Master_Label_wsnm,sortable: true, dataIndex: 'wsnm'},
                {header:HRMSRes.Master_Label_cccd,sortable: true, dataIndex: 'cccd'}
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
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ psemplymPageName + '.mvc/delete',
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
        
        var rec = this.grid.getSelectionModel().getSelected();
        var termEnabled = false;
        if (rec!=null) termEnabled = rec.get('emst')=='2'?false:true;
        Ext.getCmp(id+ '_rehire').setDisabled(enabled || termEnabled);
        Ext.getCmp(id+ '_terminate').setDisabled(enabled || !termEnabled);
        Ext.getCmp('cmbChangeType').setDisabled(enabled || !termEnabled);
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
	    var params={record:Ext.encode({params:[{ColumnName:psemplymKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + psemplymPageName + '.mvc/exportexcel';
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
    new psemplymPanel();
})

    </script>

</body>
</html>
