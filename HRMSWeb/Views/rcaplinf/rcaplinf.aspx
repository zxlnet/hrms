<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rcaplinf.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.rcaplinf.rcaplinf" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var rcaplinfConfig=Ext.decode('<%=ViewData["config"] %>'); 
rcaplinfConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var rcaplinfPageName = 'rcaplinf';
var rcaplinfKeyColumn='aino';
            
var rcaplinfQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	rcaplinfQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(rcaplinfQueryWindow,Ext.Window,{
    init:function(){
		this.formPanel=this.createFormPanel();
		this.form=this.formPanel.getForm();
	},
	
	createFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:150,
	         header:true,
             items: [
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_apno,
                        name: 'apno',stateful:false,anchor:'95%',UpperOnBlur:'true'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_jbcd,
                        name: 'jbcd',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_jbnm,
                        name: 'jbnm',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_dpnm ,
                        name: 'dpcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getDepartment',
		                    listeners:{load:function(){var f=this.form.findField('dpcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_cnnm,
                        name: 'cnnm',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_ennm,
                        name: 'ennm',stateful:false,anchor:'95%'}]}

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

var rcaplinfEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	
	
    this.rcaplinfTab=new Ext.TabPanel({
            autoTabs:true,
            activeTab:0,
            border:false,
            layoutOnTabChange:true,
            frame:true,
            autoHeight:true,
            items:[{id:'tab1',title: HRMSRes.Personal_Label_Basis,items: this.basisFormPanel},
                   {id:'tab2',title: HRMSRes.Personal_Label_Others,items: this.otherFormPanel},
                   {id:'tab3',title: HRMSRes.Recruitment_Label_Contact,items: this.contactFormPanel},
                   {id:'tab4',title: HRMSRes.Recruitment_Label_Misc,items: this.miscFormPanel},
                   {id:'tab5',title: HRMSRes.Recruitment_Label_family,items: this.familyFormPanel},
                   {id:'tab6',title: HRMSRes.Recruitment_Label_education,items: this.educationFormPanel},
                   {id:'tab7',title: HRMSRes.Recruitment_Label_experience,items: this.experienceFormPanel},
                   {id:'tab8',title: HRMSRes.Recruitment_Label_language,items: this.languageFormPanel},
                   {id:'tab9',title: HRMSRes.Recruitment_Label_reference,items: this.referenceFormPanel} 
                   ]
        }),


	rcaplinfEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:390, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.rcaplinfTab,
        listeners:{
            show:function(){              
		        var keyField = this.basisForm.findField(rcaplinfKeyColumn);

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
                    
                    this.rcaplinfTab.setActiveTab('tab4');
                    this.miscForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            f.setValue(value);                    	            
                        }
                    }); 
                    
                    this.rcaplinfTab.setActiveTab('tab3');
                    this.contactForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            f.setValue(value);                    	            
                        }
                    }); 
                                                            
                    this.rcaplinfTab.setActiveTab('tab2');
                    this.otherForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            f.setValue(value);                    	            
                        }
                    }); 
                    
                    this.query_family();
                    this.query_education();
                    this.query_experience();
                    this.query_language();
                    this.query_reference();
                    
                    setLastModifiedInfo(rcaplinfConfig,this.basisForm);
                    this.rcaplinfTab.setActiveTab('tab1');                      
		        }
                else{
                    this.rcaplinfTab.setActiveTab('tab2');
	                setLastModifiedInfo(rcaplinfConfig,this.basisForm);
                    this.rcaplinfTab.setActiveTab('tab1');                      
                }    		        
		        var keyValue = keyField.getValue();
                
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

Ext.extend(rcaplinfEditWindow,Ext.Window,{
    init:function(){
		this.familyDtlGrid=this.createFamilyGridPanel();
		this.experienceDtlGrid=this.createExpGridPanel();
		this.educationDtlGrid=this.createEducationGridPanel();
		this.languageDtlGrid=this.createLanguageGridPanel();
		this.referenceDtlGrid=this.createReferenceGridPanel();

		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();
		
		this.otherFormPanel=this.createOtherFormPanel();
		this.otherForm=this.otherFormPanel.getForm();
		
		this.contactFormPanel=this.createContactFormPanel();
		this.contactForm=this.contactFormPanel.getForm();

		this.miscFormPanel=this.createMiscFormPanel();
		this.miscForm=this.miscFormPanel.getForm();
		
		this.familyFormPanel=this.createFamilyFormPanel();
		this.familyForm=this.familyFormPanel.getForm();
		
		this.educationFormPanel=this.createEducationFormPanel();
		this.educationForm=this.educationFormPanel.getForm();

		this.experienceFormPanel=this.createExpFormPanel();
		this.experienceForm=this.experienceFormPanel.getForm();
	
		this.languageFormPanel=this.createLanguageFormPanel();
		this.languageForm=this.languageFormPanel.getForm();

		this.referenceFormPanel=this.createReferenceFormPanel();
		this.referenceForm=this.referenceFormPanel.getForm();
	},
	createBasisFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:130,
	         header:true,
      		 items: [
      		    {},{
      		    layout:'column',
      		    items:[
       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_aino + '(<font color=red>*</font>)',
                     allowBlank:false,name: 'aino',stateful:false,anchor:'95%',disabled:!this.isNew}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Recruitment_Label_apno ,
                        name: 'apno',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField',"MiscField1","MiscField2"]}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getRecruitmentApplication',
		                    listeners:{
		                        load:function(){
		                            var f=this.basisForm.findField('apno');
		                            f.setValue(f.getValue())
		                        },
		                        scope:this}}),
		                listeners:{
		                        select:function(c,r,i){
		                            var f = this.basisForm.findField('jbcd');
		                            f.setValue(r.get('MiscField1'));
		                            f = this.basisForm.findField('jbnm');
		                            f.setValue(r.get('MiscField2'));
		                        },
		                        scope:this		                
		                 },
		                 scope:this
    		          }]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_jbcd + '(<font color=red>*</font>)',
                     allowBlank:false,name: 'jbcd',stateful:false,anchor:'95%'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_jbnm + '(<font color=red>*</font>)',
                     allowBlank:false,name: 'jbnm',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Recruitment_Label_jbty ,
                        name: 'jbty',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getRecruitmentJobType',
		                    listeners:{load:function(){var f=this.basisForm.findField('jbty');f.setValue(f.getValue())},scope:this}})
    		          }]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_exsa + '(<font color=red>*</font>)',
                     allowBlank:false,name: 'exsa',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Recruitment_Label_doav + '(<font color=red>*</font>)',
                        name: 'doav',stateful:false,anchor:'95%',decimalPrecision:0,keepZero:false}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_cnnm + '(<font color=red>*</font>)',
                     allowBlank:false,name: 'cnnm',stateful:false,anchor:'95%',disabled:!this.isNew}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_ennm ,
                     allowBlank:true,name: 'ennm',stateful:false,anchor:'95%'}]},

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Personal_Label_sex + '(<font color=red>*</font>)',
                            name: 'sex', stateful: false, typeAhead: true, allowBlank: false,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: SexStore
                     }]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Recruitment_Label_brdt+ '(<font color=red>*</font>)',id:'brdt' ,
                        name:'brdt',editable:false,height:22,anchor:'95%',allowBlank:false,
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Personal_Label_mast + '(<font color=red>*</font>)',
                            name: 'mast', stateful: false, typeAhead: true,allowBlank:false,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: MaritalStatusStore
                     }]},
                                            
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Personal_Label_hght ,
                        name: 'hght',stateful:false,anchor:'95%',decimalPrecision:0,keepZero:false}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Personal_Label_wght,
                        name: 'wght',stateful:false,anchor:'95%',decimalPrecision:0,keepZero:false}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Recruitment_Label_apdt,id:'apdt',
                        name:'apdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Recruitment_Label_stus,
                        name: 'stus',stateful:false,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getRecruitmentAppStatus',
		                    listeners:{load:function(){var f=this.basisForm.findField('stus');f.setValue(f.getValue())},scope:this}})
    		          }]},

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
	createOtherFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
      		 items: [
      		    {},{
      		    layout:'column',
      		    items:[

      		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Recruitment_Label_Skll,height:125,
                        name: 'skll',stateful:false,anchor:'98%'}]},

      		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Recruitment_Label_inho,height:125,
                        name: 'inho',stateful:false,anchor:'98%'}]}

      		    ]
      		 }] 
       })
	},	
	
    createContactFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
      		 items: [
      		    {},{
      		    layout:'column',
      		    items:[

       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Recruitment_Label_hmad,height:50,
                        name: 'hmad',stateful:false,anchor:'98%'}]},
                    
       		        {columnWidth:0.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_hmpc,
                        name: 'hmpc',stateful:false,anchor:'95%'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_hmtp,
                        name: 'hmtp',stateful:false,anchor:'95%'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_wktp,
                        name: 'wktp',stateful:false,anchor:'95%'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_mobi,
                        name: 'mobi',stateful:false,anchor:'95%'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_emal,
                        name: 'emal',stateful:false,anchor:'95%'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_emrl,
                        name: 'emrl',stateful:false,anchor:'95%'}]},
                        
       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_emnm,
                        name: 'emnm',stateful:false,anchor:'95%'}]},
                        
       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_emad,
                        name: 'emad',stateful:false,anchor:'95%'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_emtp,
                        name: 'emtp',stateful:false,anchor:'95%'}]},
                        
       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Public_Label_remk,height:50,
                        name: 'remk',stateful:false,anchor:'98%'}]}
                        
      		    ]
      		 }] 
       })
	},	
	
	createMiscFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:500,
	         header:true,
      		 items: [
      		    {},{
      		    layout:'column',
      		    items:[

                    { columnWidth: 1, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Recruitment_Label_oth1,
                            name: 'oth1', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '98%',
                            displayField: 'text', valueField: 'value',
                            store: FlagYesNoStore}]},

      		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Recruitment_Label_oth1,height:32,hideLabel:true,
                        name: 'otr1',stateful:false,anchor:'98%'}]},

                    { columnWidth: 1, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Recruitment_Label_oth2,
                            name: 'oth2', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '98%',
                            displayField: 'text', valueField: 'value',
                            store: FlagYesNoStore}]},

      		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Recruitment_Label_oth2,height:32,hideLabel:true,
                        name: 'otr2',stateful:false,anchor:'98%'}]},

                    { columnWidth: 1, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Recruitment_Label_oth3,
                            name: 'oth3', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '98%',
                            displayField: 'text', valueField: 'value',
                            store: FlagYesNoStore}]},

      		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Recruitment_Label_oth3,height:32,hideLabel:true,
                        name: 'otr3',stateful:false,anchor:'98%'}]},

                    { columnWidth: 1, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Recruitment_Label_oth4,
                            name: 'oth4', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '98%',
                            displayField: 'text', valueField: 'value',
                            store: FlagYesNoStore}]},

      		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Recruitment_Label_oth4,height:32,hideLabel:true,
                        name: 'otr4',stateful:false,anchor:'98%'}]}
      		    ]
      		 }] 
       })
	},	
	createFamilyFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         tbar: [{ 
	        	            id:this.tabId+'_adddtl_family',
	                        iconCls:'icon-add', 
	                        text: HRMSRes.Public_Toolbar_Add, 
	                        handler: function(){
	                            this.addline_family();	                         
	                        }, 
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_deletedtl_family',
	                        iconCls:'icon-remove', 
	                        disabled:true,
	                        text:HRMSRes.Public_Toolbar_Delete, 
	                        handler: this.deleteline_family, 
	                        scope: this 
	                    },'-',{ 
        	                id:this.tabId+'_exportexceldtl_family',
                            iconCls:'icon-export', 
                            text:HRMSRes.Public_Toolbar_ToExcel, 
                            handler: this.exportExcelLine_family, 
                            scope: this 
                        }],
      		 items: this.familyDtlGrid
       })
	},	
    createFamilyGridPanel:function(){
		    this.rcaplinfFamilyStoreType=Ext.data.Record.create([
                {name:'aino'},
                {name:'sqno'},
                {name:'rlsp'},
                {name:'nanm'},
                {name:'occp'},
                {name:'posi'},
                {name:'telp'},
                {name:'otct'},
                {name:'rfid'}
		    ]);
            
		    var store=new Ext.data.Store({ 
        	    reader: new Ext.data.JsonReader({
	    		    totalProperty: "results",
	    		    root: "rows"               
	   	 	    },this.rcaplinfFamilyStoreType), 	
	   	        baseParams:{record:Ext.encode({params:[{ColumnName:rcaplinfKeyColumn,ColumnValue:''}]})},   
	   		    url:ContextInfo.contextPath+'/' + rcaplinfPageName + '.mvc/getApplicantFamily',
	   		    listeners:{
                    loadexception:function(o,t,response)
                    {
                        var o= Ext.util.JSON.decode(response.responseText);		   		
                    },
                    load:function(p){
                        //this.controlButton_family(this.tabId);
                    },
                    add:function(p,r,i){
                    },
                    scope:this
                }
            });
            var params={
                start:0,
                limit:Pagination.pagingSize
            };
            
            //store.load({params:params});
            
            return new Ext.grid.EditorGridPanel({
                id:'familyDtlGrid',
    		    border:true, 
                height:255,
    		    monitorResize:false, 
                loadMask:true,  		            
                ds: store, 
                frame:false,
                collapsible: true,
                animCollapse: false,
                editable:true,
                clicksToEdit:1,
                viewConfig: { 
		            //forceFit: true 
		        }, 
                listeners:{
	                rowclick:function(t,i,r){
	                    this.controlButton_family(this.tabId);
	                },
	                scope:this
                },       
                sm: new Ext.grid.RowSelectionModel(),              
                cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno',width: 50},
                        {id:'rlsp',header: HRMSRes.Recruitment_Label_rlspo,dataIndex: 'rlsp', editor: new Ext.form.TextField({})},
                        {id:'nanm',header: HRMSRes.Recruitment_Label_nanm,dataIndex: 'nanm', editor: new Ext.form.TextField({})},
                        {id:'occp',header: HRMSRes.Recruitment_Label_occp,dataIndex: 'occp', editor: new Ext.form.TextField({})},
                        {id:'posi',header: HRMSRes.Recruitment_Label_posi,dataIndex: 'posi', editor: new Ext.form.TextField({})},
                        {id:'telp',header: HRMSRes.Recruitment_Label_telp,dataIndex: 'telp', editor: new Ext.form.TextField({})},
                        {id:'otct',header: HRMSRes.Recruitment_Label_otct,dataIndex: 'otct', editor: new Ext.form.TextField({})}
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
	    query_family:function(){	    
            var params=[];	
            var f = this.basisForm.findField(rcaplinfKeyColumn);        
            var p={
                ColumnName:f.getName(),
                ColumnValue:f.getValue()                
            };

            params[params.length]=p;                  

            var loadParams={start:0,limit:Pagination.pagingSize};
            /***modified for adjquery**/
            this.familyDtlGrid.queryParams={
                params:params
            };
            this.familyDtlGrid.store.baseParams={record:Ext.util.JSON.encode(this.familyDtlGrid.queryParams)};
	        this.familyDtlGrid.getStore().load({params:loadParams});
        },
        addline_family:function(){	    
            var n=this.familyDtlGrid.getStore().getCount() + 1;

            var params=[];
            params['sqno']= n.toString();
            params['rlsp']= ''
            params['nanm']='';
            params['occp']='';
            params['posi']='';
            params['telp']= '';
            params['otct']='';

            var store = this.familyDtlGrid.getStore();
            store.add(new store.recordType(params));
        },
	    deleteline_family:function(){
	        var sm=this.familyDtlGrid.getSelectionModel();
    	    
	        if (sm.hasSelection())
	        {
                this.resortsqno_family();
                this.familyDtlGrid.store.totalLength-=1;
                this.familyDtlGrid.getBottomToolbar().updateInfo();   
            }
	    },
	    exportExcelLine_family:function(){
	        if (this.familyDtlGrid.getStore().getTotalCount()<=0){
	            this.familyDtlGrid.getBottomToolbar().diplayMsg.update('<font color=red>' + HRMSRes.Public_Message_ExcelNoRecord + '</font>');
	            return;
	        }
    	    
	        var cm=this.familyDtlGrid.getColumnModel();
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
	        var params={record:Ext.encode({params:[{ColumnName:rcaplinfKeyColumn,ColumnValue:''}],headers:header})};
    	    
	        if(this.familyDtlGrid.queryParams){
                this.familyDtlGrid.queryParams['headers']=header;
                delete params.record;
                params.record=Ext.encode(this.familyDtlGrid.queryParams);
                delete this.familyDtlGrid.queryParams.header;
            }
    	    
	        var form=document.createElement('form');
	        form.name='excelForm';
	        form.method='post';
	        form.action=ContextInfo.contextPath+ '/' + rcaplinfPageName + '.mvc/exportexcel_family';
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
	    controlButton_family:function(id){
            var enabled=!this.familyDtlGrid.getSelectionModel().hasSelection();	    
            //Ext.getCmp(id+ '_editdtl').setDisabled(enabled);
            Ext.getCmp(id+ '_deletedtl_family').setDisabled(enabled);
        },
        resortsqno_family:function(){
            var st = this.familyDtlGrid.getStore();
            for (var i =1; i<= st.getCount();i++ ){
                st.getAt(i-1).set('sqno',i.toString());
            }
    } ,       
	
	createEducationFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         tbar: [{ 
	        	            id:this.tabId+'_adddtl_education',
	                        iconCls:'icon-add', 
	                        text: HRMSRes.Public_Toolbar_Add, 
	                        handler: function(){
	                            this.addline_education();	                         
	                        }, 
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_deletedtl_education',
	                        iconCls:'icon-remove', 
	                        disabled:true,
	                        text:HRMSRes.Public_Toolbar_Delete, 
	                        handler: this.deleteline_education, 
	                        scope: this 
	                    },'-',{ 
        	                id:this.tabId+'_exportexceldtl_education',
                            iconCls:'icon-export', 
                            text:HRMSRes.Public_Toolbar_ToExcel, 
                            handler: this.exportExcelLine_education, 
                            scope: this 
                        }],
      		 items: this.educationDtlGrid
       })
	},	
	
    createEducationGridPanel:function(){
		    this.rcaplinfEducationStoreType=Ext.data.Record.create([
                {name:'aino'},
                {name:'sqno'},
                {name:'elcd'},
                {name:'scnm'},
                {name:'frdt'},
                {name:'todt'},
                {name:'spec'},
                {name:'dgcd'},
                {name:'isgd'},
                {name:'remk'},
                {name:'rfid'}
		    ]);
            
		    var store=new Ext.data.Store({ 
        	    reader: new Ext.data.JsonReader({
	    		    totalProperty: "results",
	    		    root: "rows"               
	   	 	    },this.rcaplinfEducationStoreType), 	
	   	        baseParams:{record:Ext.encode({params:[{ColumnName:rcaplinfKeyColumn,ColumnValue:''}]})},   
	   		    url:ContextInfo.contextPath+'/' + rcaplinfPageName + '.mvc/getApplicantEducation',
	   		    listeners:{
                    loadexception:function(o,t,response)
                    {
                        var o= Ext.util.JSON.decode(response.responseText);		   		
                    },
                    load:function(p,r){
                        //this.controlButton_education(this.tabId);
                    },
                    add:function(p,r,i){
                    },
                    scope:this
                }
            });
            var params={
                start:0,
                limit:Pagination.pagingSize
            };
            
            //store.load({params:params});
            
            return new Ext.grid.EditorGridPanel({
                id:'educationDtlGrid',
    		    border:true, 
                height:255,
    		    monitorResize:false, 
                loadMask:true,  		            
                ds: store, 
                frame:false,
                collapsible: true,
                animCollapse: false,
                editable:true,
                clicksToEdit:1,
                viewConfig: { 
		            //forceFit: true 
		        }, 
                listeners:{
	                rowclick:function(t,i,r){
	                    this.controlButton_education(this.tabId);
	                },
	                scope:this
                },       
                sm: new Ext.grid.RowSelectionModel(),              
                cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno',width: 50},
                        {id:'elcd',header: HRMSRes.Recruitment_Label_elcd,dataIndex: 'elcd', editor: new Ext.form.TextField({})},
                        {id:'scnm',header: HRMSRes.Recruitment_Label_scnm,dataIndex: 'scnm', editor: new Ext.form.TextField({})},
                        {id:'frdt',header: HRMSRes.Recruitment_Label_frdt,dataIndex: 'frdt', renderer:formatDateNoTime,
                            editor: new Ext.form.DateField({format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01'})},
                        {id:'todt',header: HRMSRes.Recruitment_Label_todti,dataIndex: 'todt', renderer:formatDateNoTime,
                            editor: new Ext.form.DateField({format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01'})},
                        {id:'spec',header: HRMSRes.Recruitment_Label_spec,dataIndex: 'spec', editor: new Ext.form.TextField({})},
                        {id:'dgcd',header: HRMSRes.Recruitment_Label_dgcd,dataIndex: 'dgcd', editor: new Ext.form.TextField({})},
                        {id:'isgd',header: HRMSRes.Recruitment_Label_isgd,dataIndex: 'isgd', editor: new Ext.form.TextField({})},
                        {id:'remk',header: HRMSRes.Public_Label_remk,dataIndex: 'remk', editor: new Ext.form.TextField({})}
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
	    query_education:function(){	    
            var params=[];	
            var f = this.basisForm.findField(rcaplinfKeyColumn);        
            var p={
                ColumnName:f.getName(),
                ColumnValue:f.getValue()                
            };

            params[params.length]=p;                  

            var loadParams={start:0,limit:Pagination.pagingSize};
            /***modified for adjquery**/
            this.educationDtlGrid.queryParams={
                params:params
            };
            this.educationDtlGrid.store.baseParams={record:Ext.util.JSON.encode(this.educationDtlGrid.queryParams)};
	        this.educationDtlGrid.getStore().load({params:loadParams});
        },
        addline_education:function(){	    
            var n=this.educationDtlGrid.getStore().getCount() + 1;
            var params=[];
            params['sqno']= n.toString();
            params['elcd']= ''
            params['scnm']='';
            params['frdt']='';
            params['todt']='';
            params['spec']= '';
            params['dgcd']='';
            params['isgd']='';
            params['remk']='';

            var store = this.educationDtlGrid.getStore();
            store.add(new store.recordType(params));
        },
	    deleteline_education:function(){
	        var sm=this.educationDtlGrid.getSelectionModel();
    	    
	        if (sm.hasSelection())
	        {
                this.resortsqno_education();
                this.educationDtlGrid.store.totalLength-=1;
                this.educationDtlGrid.getBottomToolbar().updateInfo();   
            }
	    },
	    exportExcelLine_education:function(){
	        if (this.educationDtlGrid.getStore().getTotalCount()<=0){
	            this.educationDtlGrid.getBottomToolbar().diplayMsg.update('<font color=red>' + HRMSRes.Public_Message_ExcelNoRecord + '</font>');
	            return;
	        }
    	    
	        var cm=this.educationDtlGrid.getColumnModel();
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
	        var params={record:Ext.encode({params:[{ColumnName:rcaplinfKeyColumn,ColumnValue:''}],headers:header})};
    	    
	        if(this.educationDtlGrid.queryParams){
                this.educationDtlGrid.queryParams['headers']=header;
                delete params.record;
                params.record=Ext.encode(this.educationDtlGrid.queryParams);
                delete this.educationDtlGrid.queryParams.header;
            }
    	    
	        var form=document.createElement('form');
	        form.name='excelForm';
	        form.method='post';
	        form.action=ContextInfo.contextPath+ '/' + rcaplinfPageName + '.mvc/exportexcel_education';
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
	    controlButton_education:function(id){
            var enabled=!this.educationDtlGrid.getSelectionModel().hasSelection();	    
            //Ext.getCmp(id+ '_editdtl').setDisabled(enabled);
            Ext.getCmp(id+ '_deletedtl_education').setDisabled(enabled);
        },
        resortsqno_education:function(){
            var st = this.educationDtlGrid.getStore();
            for (var i =1; i<= st.getCount();i++ ){
                st.getAt(i-1).set('sqno',i.toString());
            }
    } ,  	
    
	createExpFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         tbar: [{ 
	        	            id:this.tabId+'_adddtl_experience',
	                        iconCls:'icon-add', 
	                        text: HRMSRes.Public_Toolbar_Add, 
	                        handler: function(){
	                            this.addline_experience();	                         
	                        }, 
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_deletedtl_experience',
	                        iconCls:'icon-remove', 
	                        disabled:true,
	                        text:HRMSRes.Public_Toolbar_Delete, 
	                        handler: this.deleteline_experience, 
	                        scope: this 
	                    },'-',{ 
        	                id:this.tabId+'_exportexceldtl_experience',
                            iconCls:'icon-export', 
                            text:HRMSRes.Public_Toolbar_ToExcel, 
                            handler: this.exportExcelLine_experience, 
                            scope: this 
                        }],
      		 items: this.experienceDtlGrid
       })
	},	
    createExpGridPanel:function(){
		    this.rcaplinfExpStoreType=Ext.data.Record.create([
                {name:'aino'},
                {name:'sqno'},
                {name:'cpnm'},
                {name:'frdt'},
                {name:'todt'},
                {name:'posi'},
                {name:'lssa'},
                {name:'trcd'},
                {name:'remk'},
                {name:'rfid'}
		    ]);
            
		    var store=new Ext.data.Store({ 
        	    reader: new Ext.data.JsonReader({
	    		    totalProperty: "results",
	    		    root: "rows"               
	   	 	    },this.rcaplinfExpStoreType), 	
	   	        baseParams:{record:Ext.encode({params:[{ColumnName:rcaplinfKeyColumn,ColumnValue:''}]})},   
	   		    url:ContextInfo.contextPath+'/' + rcaplinfPageName + '.mvc/getApplicantExperience',
	   		    listeners:{
                    loadexception:function(o,t,response)
                    {
                        var o= Ext.util.JSON.decode(response.responseText);		   		
                    },
                    load:function(p){
                        //this.controlButton_experience(this.tabId);
                    },
                    add:function(p,r,i){
                    },
                    scope:this
                }
            });
            var params={
                start:0,
                limit:Pagination.pagingSize
            };
            
            //store.load({params:params});
            
            return new Ext.grid.EditorGridPanel({
                id:'experienceDtlGrid',
    		    border:true, 
                height:255,
    		    monitorResize:false, 
                loadMask:true,  		            
                ds: store, 
                frame:false,
                collapsible: true,
                animCollapse: false,
                editable:true,
                clicksToEdit:1,
                viewConfig: { 
		            //forceFit: true 
		        }, 
                listeners:{
	                rowclick:function(t,i,r){
	                    this.controlButton_experience(this.tabId);
	                },
	                scope:this
                },       
                sm: new Ext.grid.RowSelectionModel(),              
                cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno',width: 50},
                        {id:'cpnm',header: HRMSRes.Recruitment_Label_cpnm,dataIndex: 'cpnm', editor: new Ext.form.TextField({})},
                        {id:'frdt',header: HRMSRes.Recruitment_Label_frdt,dataIndex: 'frdt', renderer:formatDateNoTime,
                            editor: new Ext.form.DateField({format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01'})},
                        {id:'todt',header: HRMSRes.Recruitment_Label_todti,dataIndex: 'todt', renderer:formatDateNoTime,
                            editor: new Ext.form.DateField({format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01'})},
                        {id:'posi',header: HRMSRes.Recruitment_Label_posi,dataIndex: 'posi', editor: new Ext.form.TextField({})},
                        {id:'lssa',header: HRMSRes.Recruitment_Label_lssa,dataIndex: 'lssa', editor: new Ext.form.NumberField({decimalPrecision:2,keepZero:true})},
                        {id:'trcd',header: HRMSRes.Recruitment_Label_trcd,dataIndex: 'trcd', editor: new Ext.form.TextField({})},
                        {id:'remk',header: HRMSRes.Public_Label_remk,dataIndex: 'remk', editor: new Ext.form.TextField({})}
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
	    query_experience:function(){	    
            var params=[];	
            var f = this.basisForm.findField(rcaplinfKeyColumn);        
            var p={
                ColumnName:f.getName(),
                ColumnValue:f.getValue()                
            };

            params[params.length]=p;                  

            var loadParams={start:0,limit:Pagination.pagingSize};
            /***modified for adjquery**/
            this.experienceDtlGrid.queryParams={
                params:params
            };
            this.experienceDtlGrid.store.baseParams={record:Ext.util.JSON.encode(this.experienceDtlGrid.queryParams)};
	        this.experienceDtlGrid.getStore().load({params:loadParams});
        },
        addline_experience:function(){	    
            var n=this.experienceDtlGrid.getStore().getCount() + 1;
            var params=[];
            params['sqno']= n.toString();
            params['cpnm']= ''
            params['frdt']='';
            params['todt']='';
            params['posi']='';
            params['lssa']= '';
            params['trcd']='';
            params['remk']='';

            var store = this.experienceDtlGrid.getStore();
            store.add(new store.recordType(params));
        },
	    deleteline_experience:function(){
	        var sm=this.experienceDtlGrid.getSelectionModel();
    	    
	        if (sm.hasSelection())
	        {
                this.resortsqno_experience();
                this.experienceDtlGrid.store.totalLength-=1;
                this.experienceDtlGrid.getBottomToolbar().updateInfo();   
            }
	    },
	    exportExcelLine_experience:function(){
	        if (this.experienceDtlGrid.getStore().getTotalCount()<=0){
	            this.experienceDtlGrid.getBottomToolbar().diplayMsg.update('<font color=red>' + HRMSRes.Public_Message_ExcelNoRecord + '</font>');
	            return;
	        }
    	    
	        var cm=this.experienceDtlGrid.getColumnModel();
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
	        var params={record:Ext.encode({params:[{ColumnName:rcaplinfKeyColumn,ColumnValue:''}],headers:header})};
    	    
	        if(this.experienceDtlGrid.queryParams){
                this.experienceDtlGrid.queryParams['headers']=header;
                delete params.record;
                params.record=Ext.encode(this.experienceDtlGrid.queryParams);
                delete this.experienceDtlGrid.queryParams.header;
            }
    	    
	        var form=document.createElement('form');
	        form.name='excelForm';
	        form.method='post';
	        form.action=ContextInfo.contextPath+ '/' + rcaplinfPageName + '.mvc/exportexcel_experience';
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
	    controlButton_experience:function(id){
            var enabled=!this.experienceDtlGrid.getSelectionModel().hasSelection();	    
            //Ext.getCmp(id+ '_editdtl').setDisabled(enabled);
            Ext.getCmp(id+ '_deletedtl_experience').setDisabled(enabled);
        },
        resortsqno_experience:function(){
            var st = this.experienceDtlGrid.getStore();
            for (var i =1; i<= st.getCount();i++ ){
                st.getAt(i-1).set('sqno',i.toString());
            }
    } ,  	
        
	createLanguageFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         tbar: [{ 
	        	            id:this.tabId+'_adddtl_language',
	                        iconCls:'icon-add', 
	                        text: HRMSRes.Public_Toolbar_Add, 
	                        handler: function(){
	                            this.addline_language();	                         
	                        }, 
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_deletedtl_language',
	                        iconCls:'icon-remove', 
	                        disabled:true,
	                        text:HRMSRes.Public_Toolbar_Delete, 
	                        handler: this.deleteline_language, 
	                        scope: this 
	                    },'-',{ 
        	                id:this.tabId+'_exportexceldtl_language',
                            iconCls:'icon-export', 
                            text:HRMSRes.Public_Toolbar_ToExcel, 
                            handler: this.exportExcelLine_language, 
                            scope: this 
                        }],
      		 items: this.languageDtlGrid
       })
	},	
    createLanguageGridPanel:function(){
		    this.rcaplinfLanguageStoreType=Ext.data.Record.create([
                {name:'aino'},
                {name:'sqno'},
                {name:'lang'},
                {name:'spkn'},
                {name:'rdng'},
                {name:'wrtn'},
                {name:'rfid'}
		    ]);
            
		    var store=new Ext.data.Store({ 
        	    reader: new Ext.data.JsonReader({
	    		    totalProperty: "results",
	    		    root: "rows"               
	   	 	    },this.rcaplinfLanguageStoreType), 	
	   	        baseParams:{record:Ext.encode({params:[{ColumnName:rcaplinfKeyColumn,ColumnValue:''}]})},   
	   		    url:ContextInfo.contextPath+'/' + rcaplinfPageName + '.mvc/getApplicantLanguage',
	   		    listeners:{
                    loadexception:function(o,t,response)
                    {
                        var o= Ext.util.JSON.decode(response.responseText);		   		
                    },
                    load:function(p){
                        //this.controlButton_language(this.tabId);
                    },
                    add:function(p,r,i){
                    },
                    scope:this
                }
            });
            var params={
                start:0,
                limit:Pagination.pagingSize
            };
            
            //store.load({params:params});
            
            return new Ext.grid.EditorGridPanel({
                id:'languageDtlGrid',
    		    border:true, 
                height:255,
    		    monitorResize:false, 
                loadMask:true,  		            
                ds: store, 
                frame:false,
                collapsible: true,
                animCollapse: false,
                editable:true,
                clicksToEdit:1,
                viewConfig: { 
		            //forceFit: true 
		        }, 
                listeners:{
	                rowclick:function(t,i,r){
	                    this.controlButton_language(this.tabId);
	                },
	                scope:this
                },       
                sm: new Ext.grid.RowSelectionModel(),              
                cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno',width: 50},
                        {id:'lang',header: HRMSRes.Recruitment_Label_lang,dataIndex: 'lang', editor: new Ext.form.TextField({})},
                        {id:'spkn',header: HRMSRes.Recruitment_Label_spkn,dataIndex: 'spkn', editor: new Ext.form.TextField({})},
                        {id:'rdng',header: HRMSRes.Recruitment_Label_rdng,dataIndex: 'rdng', editor: new Ext.form.TextField({})},
                        {id:'wrtn',header: HRMSRes.Recruitment_Label_wrtn,dataIndex: 'wrtn', editor: new Ext.form.TextField({})}
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
	    query_language:function(){	    
            var params=[];	
            var f = this.basisForm.findField(rcaplinfKeyColumn);        
            var p={
                ColumnName:f.getName(),
                ColumnValue:f.getValue()                
            };

            params[params.length]=p;                  

            var loadParams={start:0,limit:Pagination.pagingSize};
            /***modified for adjquery**/
            this.languageDtlGrid.queryParams={
                params:params
            };
            this.languageDtlGrid.store.baseParams={record:Ext.util.JSON.encode(this.languageDtlGrid.queryParams)};
	        this.languageDtlGrid.getStore().load({params:loadParams});
        },
        addline_language:function(){	    
            var n=this.languageDtlGrid.getStore().getCount() + 1;
            var params=[];
            
            params['sqno']= n.toString();
            params['lang']= ''
            params['spkn']='';
            params['rdng']='';
            params['wrtn']='';

            var store = this.languageDtlGrid.getStore();
            store.add(new store.recordType(params));
        },
	    deleteline_language:function(){
	        var sm=this.languageDtlGrid.getSelectionModel();
    	    
	        if (sm.hasSelection())
	        {
                this.resortsqno_language();
                this.languageDtlGrid.store.totalLength-=1;
                this.languageDtlGrid.getBottomToolbar().updateInfo();   
            }
	    },
	    exportExcelLine_language:function(){
	        if (this.languageDtlGrid.getStore().getTotalCount()<=0){
	            this.languageDtlGrid.getBottomToolbar().diplayMsg.update('<font color=red>' + HRMSRes.Public_Message_ExcelNoRecord + '</font>');
	            return;
	        }
    	    
	        var cm=this.languageDtlGrid.getColumnModel();
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
	        var params={record:Ext.encode({params:[{ColumnName:rcaplinfKeyColumn,ColumnValue:''}],headers:header})};
    	    
	        if(this.languageDtlGrid.queryParams){
                this.languageDtlGrid.queryParams['headers']=header;
                delete params.record;
                params.record=Ext.encode(this.languageDtlGrid.queryParams);
                delete this.languageDtlGrid.queryParams.header;
            }
    	    
	        var form=document.createElement('form');
	        form.name='excelForm';
	        form.method='post';
	        form.action=ContextInfo.contextPath+ '/' + rcaplinfPageName + '.mvc/exportexcel_language';
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
	    controlButton_language:function(id){
            var enabled=!this.languageDtlGrid.getSelectionModel().hasSelection();	    
            //Ext.getCmp(id+ '_editdtl').setDisabled(enabled);
            Ext.getCmp(id+ '_deletedtl_language').setDisabled(enabled);
        },
        resortsqno_language:function(){
            var st = this.languageDtlGrid.getStore();
            for (var i =1; i<= st.getCount();i++ ){
                st.getAt(i-1).set('sqno',i.toString());
            }
    } ,  	
            
	createReferenceFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         tbar: [{ 
	        	            id:this.tabId+'_adddtl_reference',
	                        iconCls:'icon-add', 
	                        text: HRMSRes.Public_Toolbar_Add, 
	                        handler: function(){
	                            this.addline_reference();	                         
	                        }, 
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_deletedtl_reference',
	                        iconCls:'icon-remove', 
	                        disabled:true,
	                        text:HRMSRes.Public_Toolbar_Delete, 
	                        handler: this.deleteline_reference, 
	                        scope: this 
	                    },'-',{ 
        	                id:this.tabId+'_exportexceldtl_reference',
                            iconCls:'icon-export', 
                            text:HRMSRes.Public_Toolbar_ToExcel, 
                            handler: this.exportExcelLine_reference, 
                            scope: this 
                        }],
      		 items: this.referenceDtlGrid
       })
	},	
    createReferenceGridPanel:function(){
		    this.rcaplinfReferenceStoreType=Ext.data.Record.create([
                {name:'aino'},
                {name:'sqno'},
                {name:'nanm'},
                {name:'occp'},
                {name:'addr'},
                {name:'telp'},
                {name:'otct'},
                {name:'rlsp'},
                {name:'remk'},
                {name:'rfid'}
		    ]);
            
		    var store=new Ext.data.Store({ 
        	    reader: new Ext.data.JsonReader({
	    		    totalProperty: "results",
	    		    root: "rows"               
	   	 	    },this.rcaplinfReferenceStoreType), 	
	   	        baseParams:{record:Ext.encode({params:[{ColumnName:rcaplinfKeyColumn,ColumnValue:''}]})},   
	   		    url:ContextInfo.contextPath+'/' + rcaplinfPageName + '.mvc/getApplicantReference',
	   		    listeners:{
                    loadexception:function(o,t,response)
                    {
                        var o= Ext.util.JSON.decode(response.responseText);		   		
                    },
                    load:function(p){
                        //this.controlButton_reference(this.tabId);
                    },
                    add:function(p,r,i){
                    },
                    scope:this
                }
            });
            var params={
                start:0,
                limit:Pagination.pagingSize
            };
            
            //store.load({params:params});
            
            return new Ext.grid.EditorGridPanel({
                id:'referenceDtlGrid',
    		    border:true, 
                height:255,
    		    monitorResize:false, 
                loadMask:true,  		            
                ds: store, 
                frame:false,
                collapsible: true,
                animCollapse: false,
                editable:true,
                clicksToEdit:1,
                viewConfig: { 
		            //forceFit: true 
		        }, 
                listeners:{
	                rowclick:function(t,i,r){
	                    this.controlButton_reference(this.tabId);
	                },
	                scope:this
                },       
                sm: new Ext.grid.RowSelectionModel(),              
                cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno',width: 50},
                        {id:'nanm',header: HRMSRes.Recruitment_Label_nanm,dataIndex: 'nanm', editor: new Ext.form.TextField({})},
                        {id:'occp',header: HRMSRes.Recruitment_Label_occp,dataIndex: 'occp', editor: new Ext.form.TextField({})},
                        {id:'addr',header: HRMSRes.Recruitment_Label_addr,dataIndex: 'addr', editor: new Ext.form.TextField({})},
                        {id:'telp',header: HRMSRes.Recruitment_Label_telp,dataIndex: 'telp', editor: new Ext.form.TextField({})},
                        {id:'otct',header: HRMSRes.Recruitment_Label_otct,dataIndex: 'otct', editor: new Ext.form.TextField({})},
                        {id:'rlsp',header: HRMSRes.Recruitment_Label_rlsp,dataIndex: 'rlsp', editor: new Ext.form.TextField({})},
                        {id:'remk',header: HRMSRes.Public_Label_remk,dataIndex: 'remk', editor: new Ext.form.TextField({})}
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
	    query_reference:function(){	    
            var params=[];	
            var f = this.basisForm.findField(rcaplinfKeyColumn);        
            var p={
                ColumnName:f.getName(),
                ColumnValue:f.getValue()                
            };

            params[params.length]=p;                  

            var loadParams={start:0,limit:Pagination.pagingSize};
            /***modified for adjquery**/
            this.referenceDtlGrid.queryParams={
                params:params
            };
            this.referenceDtlGrid.store.baseParams={record:Ext.util.JSON.encode(this.referenceDtlGrid.queryParams)};
	        this.referenceDtlGrid.getStore().load({params:loadParams});
        },
        addline_reference:function(){	    
            var n=this.referenceDtlGrid.getStore().getCount() + 1;
            var params=[];
            
            params['sqno']= n.toString();
            params['nanm']= ''
            params['occp']='';
            params['addr']='';
            params['telp']='';
            params['otct']='';
            params['rlsp']='';
            params['remk']='';

            var store = this.referenceDtlGrid.getStore();
            store.add(new store.recordType(params));
        },
	    deleteline_reference:function(){
	        var sm=this.referenceDtlGrid.getSelectionModel();
    	    
	        if (sm.hasSelection())
	        {
                this.resortsqno_reference();
                this.referenceDtlGrid.store.totalLength-=1;
                this.referenceDtlGrid.getBottomToolbar().updateInfo();   
            }
	    },
	    exportExcelLine_reference:function(){
	        if (this.referenceDtlGrid.getStore().getTotalCount()<=0){
	            this.referenceDtlGrid.getBottomToolbar().diplayMsg.update('<font color=red>' + HRMSRes.Public_Message_ExcelNoRecord + '</font>');
	            return;
	        }
    	    
	        var cm=this.referenceDtlGrid.getColumnModel();
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
	        var params={record:Ext.encode({params:[{ColumnName:rcaplinfKeyColumn,ColumnValue:''}],headers:header})};
    	    
	        if(this.referenceDtlGrid.queryParams){
                this.referenceDtlGrid.queryParams['headers']=header;
                delete params.record;
                params.record=Ext.encode(this.referenceDtlGrid.queryParams);
                delete this.referenceDtlGrid.queryParams.header;
            }
    	    
	        var form=document.createElement('form');
	        form.name='excelForm';
	        form.method='post';
	        form.action=ContextInfo.contextPath+ '/' + rcaplinfPageName + '.mvc/exportexcel_reference';
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
	    controlButton_reference:function(id){
            var enabled=!this.referenceDtlGrid.getSelectionModel().hasSelection();	    
            //Ext.getCmp(id+ '_editdtl').setDisabled(enabled);
            Ext.getCmp(id+ '_deletedtl_reference').setDisabled(enabled);
        },
        resortsqno_reference:function(){
            var st = this.referenceDtlGrid.getStore();
            for (var i =1; i<= st.getCount();i++ ){
                st.getAt(i-1).set('sqno',i.toString());
            }
    } ,
                
	save: function(){
		if(!this.basisForm.isValid()) return;
		if(!this.otherForm.isValid()) return;
		this.grid.getBottomToolbar().diplayMsg.update('');
		var params={};
		this.rcaplinfTab.setActiveTab('tab1');
		this.basisForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });
        this.rcaplinfTab.setActiveTab('tab2');
		this.otherForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });

        this.rcaplinfTab.setActiveTab('tab3');
		this.contactForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });

        this.rcaplinfTab.setActiveTab('tab4');
		this.miscForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });

        var familyDtlParams=[];       
        var st=this.familyDtlGrid.getStore();
        for (var i =0; i< st.getCount();i++ ){
            var x = st.getAt(i);
            var p = {aino:this.basisForm.findField('aino').getValue(),
                          sqno:x.get('sqno').toString(),
                          rlsp:x.get('rlsp'),
                          nanm:x.get('nanm'),
                          occp:x.get('occp'),
                          posi:x.get('posi'),
                          telp:x.get('telp'),
                          otct:x.get('otct')};
            familyDtlParams[familyDtlParams.length] = p;
        };
        
        var educationDtlParams=[];       
        var st=this.educationDtlGrid.getStore();
        for (var i =0; i< st.getCount();i++ ){
            var x = st.getAt(i);
            var p = {aino:this.basisForm.findField('aino').getValue(),
                          sqno:x.get('sqno').toString(),
                          elcd:x.get('elcd'),
                          scnm:x.get('scnm'),
                          frdt:x.get('frdt'),
                          todt:x.get('todt'),
                          spec:x.get('spec'),
                          dgcd:x.get('dgcd'),
                          isgd:x.get('isgd'),
                          remk:x.get('remk')};
            educationDtlParams[educationDtlParams.length] = p;
        };

        var experienceDtlParams=[];       
        var st=this.experienceDtlGrid.getStore();
        for (var i =0; i< st.getCount();i++ ){
            var x = st.getAt(i);
            var p = {aino:this.basisForm.findField('aino').getValue(),
                          sqno:x.get('sqno').toString(),
                          cpnm:x.get('cpnm'),
                          frdt:x.get('frdt'),
                          todt:x.get('todt'),
                          posi:x.get('posi'),
                          lssa:x.get('lssa'),
                          trcd:x.get('trcd'),
                          remk:x.get('remk')};
            experienceDtlParams[experienceDtlParams.length] = p;
        };

        var languageDtlParams=[];       
        var st=this.languageDtlGrid.getStore();
        for (var i =0; i< st.getCount();i++ ){
            var x = st.getAt(i);
            var p = {aino:this.basisForm.findField('aino').getValue(),
                          sqno:x.get('sqno').toString(),
                          lang:x.get('lang'),
                          spkn:x.get('spkn'),
                          rdng:x.get('rdng'),
                          wrtn:x.get('wrtn')};
            languageDtlParams[languageDtlParams.length] = p;
        };

        var referenceDtlParams=[];       
        var st=this.referenceDtlGrid.getStore();
        for (var i =0; i< st.getCount();i++ ){
            var x = st.getAt(i);
            var p = {aino:this.basisForm.findField('aino').getValue(),
                          sqno:x.get('sqno').toString(),
                          nanm:x.get('nanm'),
                          occp:x.get('occp'),
                          addr:x.get('addr'),
                          telp:x.get('telp'),
                          otct:x.get('otct'),
                          rlsp:x.get('rlsp'),
                          remk:x.get('remk')
                          };
            referenceDtlParams[referenceDtlParams.length] = p;
        };

        var keyparams=[];
        keyparams[0]={ColumnName:'aino',ColumnValue:this.basisForm.findField('aino').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + rcaplinfPageName + '.mvc/'+method,
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
		   params: {record:Ext.encode({params:params,keycolumns:keyparams,
		                    family:familyDtlParams,education:educationDtlParams,experience:experienceDtlParams,
		                    language:languageDtlParams,reference:referenceDtlParams})}
		});
	}
});


var rcaplinfPanel=function(){
    this.tabId=rcaplinfConfig.tabId;
	this.init();	
	
	rcaplinfPanel.superclass.constructor.call(this,{
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
	            hidden:rcaplinfConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new rcaplinfEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden:rcaplinfConfig.auth[this.tabId+'_' + 'edit']!='True',
	            disabled:true,
	            handler: function(){
	            	new rcaplinfEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:rcaplinfConfig.auth[this.tabId+'_' + 'delete']!='True',
                disabled:true,
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:rcaplinfConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new rcaplinfQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:rcaplinfConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:rcaplinfConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,rcaplinfConfig.muf?'delete':'add',rcaplinfConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(rcaplinfPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){
		var rcaplinfStoreType=Ext.data.Record.create([
            {name:'aino'},
            {name:'apno'},
            {name:'jbcd'},
            {name:'jbnm'},
            {name:'jbty'},
            {name:'exsa'},
            {name:'doav'},
            {name:'cnnm'},
            {name:'ennm'},
            {name:'brdt'},
            {name:'mast'},
            {name:'sex'},
            {name:'hmad'},           
            {name:'hght'},
            {name:'wght'},
            {name:'skll'},
            {name:'inho'},
            {name:'oth1'},
            {name:'oth2'},
            {name:'oth3'},
            {name:'oth4'},
            {name:'otr1'},
            {name:'otr2'},
            {name:'otr3'},
            {name:'otr4'},
            {name:'hmtp'},
            {name:'wktp'},
            {name:'mobi'},
            {name:'emal'},
            {name:'emrl'},
            {name:'emnm'},
            {name:'emad'},
            {name:'emtp'},
            {name:'apdt'},
            {name:'stus'},
            {name:'remk'},
            {name:'lmtm'},
            {name:'lmur'},
            {name:'rfid'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},rcaplinfStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:rcaplinfKeyColumn,ColumnValue:rcaplinfConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + rcaplinfPageName + '.mvc/list',
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
		        //forceFit: true 
		    }, 
            listeners:{
	            rowclick:function(){
	                this.controlButton(this.tabId);
	            },
	            scope:this
            },                      
            cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Recruitment_Label_jbcd,sortable: true, dataIndex: 'jbcd'},
                {header:HRMSRes.Recruitment_Label_jbnm,sortable: true, dataIndex: 'jbnm'},
                {header:HRMSRes.Recruitment_Label_aino,sortable: true, dataIndex: 'aino'},
                {header:HRMSRes.Recruitment_Label_cnnm,sortable: true, dataIndex: 'cnnm'},
                {header:HRMSRes.Recruitment_Label_ennm,sortable: true, dataIndex: 'ennm'},
                {header:HRMSRes.Personal_Label_sex,sortable: true, dataIndex: 'sex'},
                { header: HRMSRes.Recruitment_Label_brdt, sortable: true, dataIndex: 'brdt',renderer:formatDateNoTime },
                {header:HRMSRes.Recruitment_Label_apno,sortable: true, dataIndex: 'apno'},
                { header: HRMSRes.Recruitment_Label_exsa, sortable: true, dataIndex: 'exsa' },
                {header:HRMSRes.Personal_Label_mast,sortable: true, dataIndex: 'mast', renderer: mastRender },
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
        keyparams[0]={ColumnName:'aino',ColumnValue:record.get('aino')};
        
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ rcaplinfPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:rcaplinfKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + rcaplinfPageName + '.mvc/exportexcel';
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
    new rcaplinfPanel();
})

    </script>

</body>
</html>