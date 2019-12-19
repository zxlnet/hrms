<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="psperson.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.psperson.psperson" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var pspersonConfig=Ext.decode('<%=ViewData["config"] %>'); 
pspersonConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var pspersonPageName = 'psperson';
var pspersonKeyColumn='emno';
            
var pspersonQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	pspersonQueryWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:600, 
        height:320, 
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

Ext.extend(pspersonQueryWindow,Ext.Window,{
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
                {layout:'column',
      		    items:[
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sfid,
                        name: 'sfid',stateful:false,anchor:'95%'}]},
            
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sftn,
                        name: 'ntnm',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_ennm,
                        name: 'ennm',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_frnm,
                        name: 'frnm',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_brpl,
                        name: 'brpl',stateful:false,anchor:'95%'}]},
            
     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Personal_Label_brdt + '' + HRMSRes.Public_Label_From,id:'from|brdt',
                        name:'from|brdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Personal_Label_brdt + '' + HRMSRes.Public_Label_To,id:'to|brdt',
                        name:'to|brdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},
           	 	        
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
           	 	        
            ]}] 
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

var pspersonEditWindow = function(grid, isNew) {
    this.grid = grid;
    this.isNew = isNew || false;
    this.store = this.grid.getStore();
    this.init();

    this.personalTab = new Ext.TabPanel({
        autoTabs: true,
        activeTab: 0,
        border: false,
        layoutOnTabChange: true,
        frame: true,
        autoHeight: true,
        items: [{ id: 'tab1', title: HRMSRes.Personal_Label_Basis, items: this.basisFormPanel },
                       { id: 'tab2', title: HRMSRes.Personal_Label_Identity, items: this.identityFormPanel },
                       { id: 'tab3', title: HRMSRes.Personal_Label_Health, items: this.healthFormPanel },
                       { id: 'tab4', title: HRMSRes.Personal_Label_Others, items: this.othersFormPanel}]
    }),

	pspersonEditWindow.superclass.constructor.call(this, {
	    layout: 'fit',
	    width: 650,
	    height: 380,
	    title: this.isNew ? HRMSRes.Public_Add_WindowTitle : HRMSRes.Public_Edit_WindowTitle,
	    closeAction: 'close',
	    modal: true,
	    resizable: false,
	    buttonAlign: 'center',
	    items: this.personalTab,
	    listeners: {
	        show: function() {
	            var keyField = this.basisForm.findField(pspersonKeyColumn);

	            if (!this.isNew) {
	                var data = this.grid.getSelectionModel().getSelected();
	                this.personalTab.setActiveTab('tab3');
	                this.healthForm.items.each(function(f) {
	                    if (f.isFormField) {
	                        var value = data.get(f.getName());
	                        if (f.xtype == 'timefield')
	                            value = formatTime(value);

	                        if (f.xtype == 'datefield')
	                            value = formatDateNoTime(value);

	                        f.setValue(value);
	                    }
	                });
	                this.personalTab.setActiveTab('tab2');
	                this.identityForm.items.each(function(f) {
	                    if (f.isFormField) {
	                        var value = data.get(f.getName());
	                        if (f.xtype == 'timefield')
	                            value = formatTime(value);

	                        if (f.xtype == 'datefield')
	                            value = formatDateNoTime(value);

	                        f.setValue(value);
	                    }
	                });
	                this.personalTab.setActiveTab('tab4');
	                this.othersForm.items.each(function(f) {
	                    if (f.isFormField) {
	                        var value = data.get(f.getName());
	                        if (f.xtype == 'timefield')
	                            value = formatTime(value);

	                        if (f.xtype == 'datefield')
	                            value = formatDateNoTime(value);

	                        f.setValue(value);
	                    }
	                });
	                setLastModifiedInfo(pspersonConfig, this.othersForm);

	                this.personalTab.setActiveTab('tab1');
	                this.basisForm.items.each(function(f) {
	                    if (f.isFormField) {
	                        var value = data.get(f.getName());
	                        if (f.xtype == 'timefield')
	                            value = formatTime(value);

	                        if (f.xtype == 'datefield')
	                            value = formatDateNoTime(value);

	                        f.setValue(value);
	                    }
	                });
	            }
	            else {
	                this.personalTab.setActiveTab('tab4');
	                setLastModifiedInfo(pspersonConfig, this.othersForm);
	                this.personalTab.setActiveTab('tab1');
	                //this.getMaxemno();
	                this.getStaffID();
	            }

	            var keyValue = keyField.getValue();

	        },
	        scope: this
	    },
	    buttons: [{
	        text: HRMSRes.Public_Button_Confirm,
	        iconCls: 'icon-save',
	        handler: this.save,
	        scope: this
	    }, {
	        text: HRMSRes.Public_Button_Close,
	        iconCls: 'icon-exit',
	        handler: function() {
	            this.close();
	        },
	        scope: this
}]
	    })
}

Ext.extend(pspersonEditWindow, Ext.Window, {
    init: function() {
        this.basisFormPanel = this.createBasisFormPanel();
        this.basisForm = this.basisFormPanel.getForm();

        this.healthFormPanel = this.createHealthFormPanel();
        this.healthForm = this.healthFormPanel.getForm();

        this.identityFormPanel = this.createIdentityFormPanel();
        this.identityForm = this.identityFormPanel.getForm();

        this.othersFormPanel = this.createOthersFormPanel();
        this.othersForm = this.othersFormPanel.getForm();
    },
    createBasisFormPanel: function() {
        return new Ext.FormPanel({
            frame: true,
            labelWidth: 120,
            header: true,
            title: 'Basic',
            //autoHeight:true,
            items: [
      		    {
      		        layout: 'column',
      		        items: [
                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_emno + '(<font color=red>*</font>)',
                            name: 'emno', allowBlank: false, disabled: true, stateful: false, anchor: '95%'}]
                        },

      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Public_Label_sfid + '(<font color=red>*</font>)',
      		                name: 'sfid', allowBlank: false, stateful: false, anchor: '95%'}]
      		            },

       		        { columnWidth: .5, layout: 'form',
       		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_sunm + '(<font color=red>*</font>)',
       		                name: 'sunm', allowBlank: false, stateful: false, anchor: '95%',
       		                listeners: {
       		                    change: function(p) {
       		                        var f = this.basisForm.findField('ntnm');
       		                        var t1 = this.basisForm.findField('otnm');
       		                        f.setValue(p.getValue() + t1.getValue());
       		                    },
       		                    scope: this
}}]
       		                },

       		        { columnWidth: .5, layout: 'form',
       		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_otnm + '(<font color=red>*</font>)',
       		                name: 'otnm', allowBlank: false, stateful: false, anchor: '95%',
       		                listeners: {
       		                    change: function(p) {
       		                        var f = this.basisForm.findField('ntnm');
       		                        var t1 = this.basisForm.findField('sunm');
       		                        f.setValue(t1.getValue() + ' ' + p.getValue());
       		                    },
       		                    scope: this
}}]
       		                },

      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_ntnm + '(<font color=red>*</font>)',
      		                name: 'ntnm', allowBlank: false, stateful: false, anchor: '95%', disabled: true}]
      		            },

      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_frnm,
      		                name: 'frnm', stateful: false, anchor: '95%'}]
      		            },

      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_ennm,
      		                name: 'ennm', stateful: false, anchor: '95%'}]
      		            },

       		        { columnWidth: .5, layout: 'form',
       		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_napl,
       		                name: 'napl', stateful: false, anchor: '95%'}]
       		            },

       		        { columnWidth: .5, layout: 'form',
       		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_nknm,
       		                name: 'nknm', stateful: false, anchor: '95%'}]
       		            },

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Personal_Label_sex + '(<font color=red>*</font>)',
                            name: 'sex', stateful: false, typeAhead: true, allowBlank: false,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: SexStore
}]
                        },

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Master_Label_nanm,
                            name: 'nacd', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: new Ext.data.Store({
                                reader: new Ext.data.JsonReader({ totalProperty: "results", root: "rows",
                                    fields: ['ValueField', 'DisplayField']
                                }), autoLoad: true,
                                url: ContextInfo.contextPath + '/dropdown.mvc/getNation',
                                listeners: { load: function() { f = this.basisForm.findField('nacd'); f.setValue(f.getValue()); }, scope: this }
                            })
}]
                        },

     		        { columnWidth: .5, layout: 'form',
     		            items: [{ xtype: 'datefield', fieldLabel: HRMSRes.Personal_Label_brdt, id: 'brdt',
     		                name: 'brdt', editable: false, height: 22, anchor: '95%',
     		                format: DATE_FORMAT.DATEONLY, minValue: '1980/01/01', stateful: false,
     		                invalidText: '{0} ' + HRMSRes.Public_Messsage_FormatInCorrect + ' {1}'}]
     		            },

      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_brpl,
      		                name: 'brpl', stateful: false, anchor: '95%'}]
      		            },

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Master_Label_plnm,
                            name: 'plcd', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: new Ext.data.Store({
                                reader: new Ext.data.JsonReader({ totalProperty: "results", root: "rows",
                                    fields: ['ValueField', 'DisplayField']
                                }), autoLoad: true,
                                url: ContextInfo.contextPath + '/dropdown.mvc/getPolity',
                                listeners: { load: function() { f = this.basisForm.findField('plcd'); f.setValue(f.getValue()); }, scope: this }
                            })
}]
                        },

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Personal_Label_salu,
                            name: 'salu', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: SaluStore
}]
                        },

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Master_Label_elnm,
                            name: 'elcd', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: new Ext.data.Store({
                                reader: new Ext.data.JsonReader({ totalProperty: "results", root: "rows",
                                    fields: ['ValueField', 'DisplayField']
                                }), autoLoad: true,
                                url: ContextInfo.contextPath + '/dropdown.mvc/getEducationLevel',
                                listeners: { load: function() { f = this.basisForm.findField('elcd'); f.setValue(f.getValue()); }, scope: this }
                            })
}]
                        },

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Personal_Label_mast,
                            name: 'mast', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: MaritalStatusStore
}]
                        },

     		        { columnWidth: .5, layout: 'form',
     		            items: [{ xtype: 'datefield', fieldLabel: HRMSRes.Personal_Label_madt, id: 'madt',
     		                name: 'madt', editable: false, height: 22, anchor: '95%',
     		                format: DATE_FORMAT.DATEONLY, minValue: '1980/01/01', stateful: false,
     		                invalidText: '{0} ' + HRMSRes.Public_Messsage_FormatInCorrect + ' {1}'}]
     		            }
      		    ]
}]
        })
    },
    createHealthFormPanel: function() {
        return new Ext.FormPanel({
            frame: true,
            labelWidth: 120,
            header: true,
            title: 'Health',
            items: [{
                layout: 'column',
                items: [
                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_wght,
                            name: 'wght', stateful: false, anchor: '95%'}]
                        },

      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_hght,
      		                name: 'hght', stateful: false, anchor: '95%'}]
      		            },

      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_visi,
      		                name: 'visi', stateful: false, anchor: '95%'}]
      		            },

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Personal_Label_blty,
                            name: 'blty', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: BloodTypeStore
}]
                        },

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Personal_Label_isdi,
                            name: 'isdi', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'text', valueField: 'value',
                            store: FlagYesNoStore
}]
                        },

      		        { columnWidth: 1, layout: 'form',
      		            items: [{ xtype: 'textarea', fieldLabel: HRMSRes.Personal_Label_deds,
      		                name: 'deds', stateful: false, anchor: '95%'}]
      		            }
      		    ]
}]
            })
        },
        createIdentityFormPanel: function() {
            return new Ext.FormPanel({
                frame: true,
                labelWidth: 120,
                header: true,
                title: 'Identity',
                items: [{
                    layout: 'column',
                    items: [
                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_idno + '(<font color=red>*</font>)',
                            name: 'idno', allowBlank: false, stateful: false, anchor: '95%'}]
                        },

     		        { columnWidth: .5, layout: 'form',
     		            items: [{ xtype: 'datefield', fieldLabel: HRMSRes.Personal_Label_idsd, id: 'idsd',
     		                name: 'idsd', editable: false, height: 22, anchor: '95%',
     		                format: DATE_FORMAT.DATEONLY, minValue: '1980/01/01',
     		                invalidText: '{0} ' + HRMSRes.Public_Messsage_FormatInCorrect + ' {1}'}]
     		            },

     		        { columnWidth: .5, layout: 'form',
     		            items: [{ xtype: 'datefield', fieldLabel: HRMSRes.Personal_Label_ided, id: 'ided',
     		                name: 'ided', editable: false, height: 22, anchor: '95%',
     		                format: DATE_FORMAT.DATEONLY, minValue: '1980/01/01',
     		                invalidText: '{0} ' + HRMSRes.Public_Messsage_FormatInCorrect + ' {1}'}]
     		            },
     		            
      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_ppno,
      		                name: 'ppno', stateful: false, anchor: '95%'}]
      		            },

      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_ppip,
      		                name: 'ppip', stateful: false, anchor: '95%'}]
      		            },

     		        { columnWidth: .5, layout: 'form',
     		            items: [{ xtype: 'datefield', fieldLabel: HRMSRes.Personal_Label_ppid, id: 'ppid',
     		                name: 'ppid', editable: false, height: 22, anchor: '95%',
     		                format: DATE_FORMAT.DATEONLY, minValue: '1980/01/01',
     		                invalidText: '{0} ' + HRMSRes.Public_Messsage_FormatInCorrect + ' {1}'}]
     		            },

     		        { columnWidth: .5, layout: 'form',
     		            items: [{ xtype: 'datefield', fieldLabel: HRMSRes.Personal_Label_pped, id: 'pped',
     		                name: 'pped', editable: false, height: 22, anchor: '95%',
     		                format: DATE_FORMAT.DATEONLY, minValue: '1980/01/01',
     		                invalidText: '{0} ' + HRMSRes.Public_Messsage_FormatInCorrect + ' {1}'}]
     		            },

      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_wpno,
      		                name: 'wpno', stateful: false, anchor: '95%'}]
      		            },

     		        { columnWidth: .5, layout: 'form',
     		            items: [{ xtype: 'datefield', fieldLabel: HRMSRes.Personal_Label_wped, id: 'wped',
     		                name: 'wped', editable: false, height: 22, anchor: '95%',
     		                format: DATE_FORMAT.DATEONLY, minValue: '1980/01/01',
     		                invalidText: '{0} ' + HRMSRes.Public_Messsage_FormatInCorrect + ' {1}'}]
     		            },

      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_drno,
      		                name: 'drno', stateful: false, anchor: '95%'}]
      		            },

      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Personal_Label_drty,
      		                name: 'drty', stateful: false, anchor: '95%'}]
      		            },

     		        { columnWidth: .5, layout: 'form',
     		            items: [{ xtype: 'datefield', fieldLabel: HRMSRes.Personal_Label_dred, id: 'dred',
     		                name: 'dred', editable: false, height: 22, anchor: '95%',
     		                format: DATE_FORMAT.DATEONLY, minValue: '1980/01/01',
     		                invalidText: '{0} ' + HRMSRes.Public_Messsage_FormatInCorrect + ' {1}'}]
     		            }
      		    ]
}]
                    })
                },
                createOthersFormPanel: function() {
                    return new Ext.FormPanel({
                        frame: true,
                        labelWidth: 120,
                        header: true,
                        title: 'Others',
                        items: [{
                            layout: 'column',
                            items: [
                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Master_Label_rgcd,
                            name: 'rgcd', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: new Ext.data.Store({
                                reader: new Ext.data.JsonReader({ totalProperty: "results", root: "rows",
                                    fields: ['ValueField', 'DisplayField']
                                }), autoLoad: true,
                                url: ContextInfo.contextPath + '/dropdown.mvc/getReligion',
                                listeners: { load: function() { f = this.othersForm.findField('rgcd'); f.setValue(f.getValue()); }, scope: this }
                            })
}]
                        },

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Master_Label_cocd,
                            name: 'cocd', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: new Ext.data.Store({
                                reader: new Ext.data.JsonReader({ totalProperty: "results", root: "rows",
                                    fields: ['ValueField', 'DisplayField']
                                }), autoLoad: true,
                                url: ContextInfo.contextPath + '/dropdown.mvc/getCountry',
                                listeners: { load: function() { f = this.othersForm.findField('cocd'); f.setValue(f.getValue()); }, scope: this }
                            })
}]
                        },

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Personal_Label_occd,
                            name: 'occd', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: new Ext.data.Store({
                                reader: new Ext.data.JsonReader({ totalProperty: "results", root: "rows",
                                    fields: ['ValueField', 'DisplayField']
                                }), autoLoad: true,
                                url: ContextInfo.contextPath + '/dropdown.mvc/getCountry',
                                listeners: { load: function() { f = this.othersForm.findField('occd'); f.setValue(f.getValue()); }, scope: this }
                            })
}]
                        },

                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Master_Label_arnm,
                            name: 'arcd', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: new Ext.data.Store({
                                reader: new Ext.data.JsonReader({ totalProperty: "results", root: "rows",
                                    fields: ['ValueField', 'DisplayField']
                                }), autoLoad: true,
                                url: ContextInfo.contextPath + '/dropdown.mvc/getArea',
                                listeners: { load: function() { f = this.othersForm.findField('arcd'); f.setValue(f.getValue()); }, scope: this }
                            })
}]
                        },

      		        { columnWidth: 1, layout: 'form',
      		            items: [{ xtype: 'textarea', fieldLabel: HRMSRes.Personal_Label_itst, height: 50,
      		                name: 'itst', stateful: false, anchor: '98%'}]
      		            },

      		        { columnWidth: 1, layout: 'form',
      		            items: [{ xtype: 'textarea', fieldLabel: HRMSRes.Personal_Label_spec, height: 50,
      		                name: 'spec', stateful: false, anchor: '98%'}]
      		            },

       		        { columnWidth: 1, layout: 'form',
       		            items: [{ xtype: 'textarea', fieldLabel: HRMSRes.Public_Label_remk, height: 50,
       		                name: 'remk', stateful: false, anchor: '98%'}]
       		            },

     		        { columnWidth: .5, layout: 'form',
     		            items: [{ xtype: 'datefield', fieldLabel: HRMSRes.Public_Label_lmtm, disabled: true,
     		                name: 'lmtm', editable: false, height: 22, anchor: '95%',
     		                format: DATE_FORMAT.DATETIME, minValue: '1980/01/01',
     		                invalidText: '{0} ' + HRMSRes.Public_Messsage_FormatInCorrect + ' {1}'}]
     		            },

      		        { columnWidth: .5, layout: 'form',
      		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Public_Label_lmur, disabled: true,
      		                name: 'lmur', stateful: false, anchor: '95%'}]
      		            },

       		        { columnWidth: .5, layout: 'form',
       		            items: [{ xtype: 'textfield', fieldLabel: HRMSRes.Public_Label_rest, disabled: true,
       		                name: 'stus', stateful: false, anchor: '95%', nonUpdateField: true}]
       		            }
                    ]}]
                        })
                    },

                    save: function() {
                        if (!this.basisForm.isValid()) return;

                        if (ContextInfo.sysCfg['PsCKID'] == 'Y') {
                            var id = this.identityForm.findField('idno').getValue();
                            if (CheckIDCard(id) == false) {
                                Ext.MessageBox.show({
                                    title: HRMSRes.Public_Message_Error,
                                    msg: HRMSRes.Public_Message_WrongIDFormat,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                                return;
                            }
                        }

                        this.grid.getBottomToolbar().diplayMsg.update('');
                        var params = {};
                        this.basisForm.items.each(function(f) {
                            if (f.isFormField && !f.nonUpdateField) {
                                params[f.getName()] = f.getValue();
                            }
                        });
                        this.healthForm.items.each(function(f) {
                            if (f.isFormField && !f.nonUpdateField) {
                                params[f.getName()] = f.getValue();
                            }
                        });
                        this.identityForm.items.each(function(f) {
                            if (f.isFormField && !f.nonUpdateField) {
                                params[f.getName()] = f.getValue();
                            }
                        });
                        this.othersForm.items.each(function(f) {
                            if (f.isFormField && !f.nonUpdateField) {
                                params[f.getName()] = f.getValue();
                            }
                        });

                        var method = this.isNew ? 'new' : 'edit';
                        Ext.Ajax.request({
                            url: ContextInfo.contextPath + '/' + pspersonPageName + '.mvc/' + method,
                            success: function(response) {
                                var o = Ext.util.JSON.decode(response.responseText);
                                if (o.status == 'success') {
                                    var store = this.grid.store;
                                    if (!this.isNew) {
                                        var sm = this.grid.getSelectionModel();
                                        var record = sm.getSelected();
                                        for (var p in params) {
                                            record.set(p, params[p]);
                                        }
                                    } else {
                                        store.insert(0, new store.recordType(params));
                                        this.grid.store.totalLength += 1;
                                        this.grid.getBottomToolbar().updateInfo();
                                    }
                                    this.close();
                                } else {
                                    Ext.MessageBox.show({
                                        title: HRMSRes.Public_Message_Error,
                                        msg: o.msg,
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.ERROR
                                    });
                                }
                                this.grid.getBottomToolbar().diplayMsg.update(o.msg);
                            },
                            scope: this,
                            params: { record: Ext.util.JSON.encode(params) }
                        });
                    },
                    getMaxemno: function() {
                        var params = {};
                        Ext.Ajax.request({
                            url: ContextInfo.contextPath + '/public.mvc/GetMaxemno',
                            success: function(response) {
                                var o = Ext.util.JSON.decode(response.responseText);
                                if (o.status == 'success') {
                                    this.newemno = o.msg;
                                    this.basisForm.findField(pspersonKeyColumn).setValue(this.newemno);
                                } else {
                                    Ext.MessageBox.show({
                                        title: HRMSRes.Public_Message_Error,
                                        msg: o.msg,
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.ERROR
                                    });
                                    this.newemno = '';
                                }
                            },
                            scope: this,
                            params: { record: Ext.util.JSON.encode(params) }
                        });
                    },
                    getStaffID: function() {
                        var params = {};
                        Ext.Ajax.request({
                            url: ContextInfo.contextPath + '/psperson.mvc/GetAutoStaffId',
                            success: function(response) {
                                var o = Ext.util.JSON.decode(response.responseText);
                                if (o.status == 'success') {
                                    this.newemno = o.emno;
                                    this.basisForm.findField(pspersonKeyColumn).setValue(this.newemno);
                                    this.basisForm.findField('sfid').setValue(o.sfid);
                                } else {
                                    Ext.MessageBox.show({
                                        title: HRMSRes.Public_Message_Error,
                                        msg: o.msg,
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.ERROR
                                    });
                                    this.newemno = '';
                                }
                            },
                            scope: this,
                            params: { record: Ext.util.JSON.encode(params) }
                        });
                    }
                });


var pspersonPanel=function(){
    this.tabId=pspersonConfig.tabId;
	this.init();	
	
	var relatedFuncMenuPerson = new Ext.menu.Menu({
    items: [
        {text: HRMSRes.Public_Menu_psemplym,handler: function() {if (this.grid.getSelectionModel().hasSelection()==false) return;var emno = this.grid.getSelectionModel().getSelected().get('emno');mainPanel.loadClass(ContextInfo.contextPath+'/psemplym.mvc/index?menuId=M2210&helpId=psemplym&emno=' + emno,'M2210','',HRMSRes.Public_Menu_psemplym,'','true');},scope:this},      
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
    
	pspersonPanel.superclass.constructor.call(this,{
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
	            hidden:pspersonConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new pspersonEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden: pspersonConfig.auth[this.tabId + '_' + 'edit'] != 'True',
	            disabled:true,
	            handler: function(){
	            	new pspersonEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove',
                hidden: pspersonConfig.auth[this.tabId + '_' + 'delete'] != 'True',
                disabled:true,
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:pspersonConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new pspersonQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:pspersonConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
	            handler: this.exportExcel, 
	            scope: this 
	        },'-',{ 
	            text:HRMSRes.Public_Toolbar_Related,
	            iconCls:'icon-related',
	            id: 'relatedFuncMenuPerson',
	            menu: relatedFuncMenuPerson
	        },'->',{ 
                id:this.tabId+'_recconfirm',
                iconCls:'icon-recconfirm', 
                text:'', 
                handler: function(){
                        confirmRecord(this.grid);},
                scope: this 
            },{ 
                id:this.tabId+'_logview',
                iconCls:'icon-logview', 
                text:'', 
                handler: function(){
                        var data=this.grid.getSelectionModel().getSelected();if (data==null) return;var rfid = data.get('rfid');new logViewerWindow(rfid).show();},
                scope: this 
            },{ 
                id:this.tabId+'_muf',
                iconCls:pspersonConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,pspersonConfig.muf?'delete':'add',pspersonConfig,this.grid);},
                scope: this 
            }],
	        items:this.grid
       }]
	})
}

Ext.extend(pspersonPanel, Ext.Panel, {
    init: function() {
        this.grid = this.createGridPanel();
    },

    createGridPanel: function() {
        var pspersonStoreType = Ext.data.Record.create([
		    { name: 'emno' },
            { name: 'sfid' },
            { name: 'elcd' },
            { name: 'idno' },
            { name: 'itst' },
            { name: 'brdt' },
            { name: 'brpl' },
            { name: 'occd' },
            { name: 'ocnm' },
            { name: 'cocd' },
            { name: 'conm' },
            { name: 'mast' },
            { name: 'ntnm' },
            { name: 'frnm' },
            { name: 'ennm' },
            { name: 'napl' },
            { name: 'nknm' },
            { name: 'otnm' },
            { name: 'plcd' },
            { name: 'plnm' },
            { name: 'rlcd' },
            { name: 'rlnm' },
            { name: 'remk' },
            { name: 'salu' },
            { name: 'sex' },
            { name: 'wght' },
            { name: 'hght' },
            { name: 'sunm' },
            { name: 'visi' },
            { name: 'nacd' },
            { name: 'nanm' },
            { name: 'arcd' },
            { name: 'arnm' },
            { name: 'madt' },
            { name: 'blty' },
            { name: 'isdi' },
            { name: 'deds' },
            { name: 'spec' },
            { name: 'idsd' },
            { name: 'ppno' },
            { name: 'ppid' },
            { name: 'ppip' },
            { name: 'pped' },
            { name: 'wpno' },
            { name: 'wped' },
            { name: 'drno' },
            { name: 'drty' },
            { name: 'dred' },
            { name: 'lmtm' },
            { name: 'lmur' },
            { name: 'stus' },
            { name: 'rfid' },
            { name: 'emst' },
            { name: 'ided' }
		]);

        var store = new Ext.data.Store({
            reader: new Ext.data.JsonReader({
                totalProperty: "results",
                root: "rows"
            }, pspersonStoreType),
            baseParams: { record: Ext.encode({ params: [{ ColumnName: pspersonKeyColumn, ColumnValue: pspersonConfig.emno}] }) },
            url: ContextInfo.contextPath + '/' + pspersonPageName + '.mvc/list',
            listeners: {
                loadexception: function(o, t, response) {
                    var o = Ext.util.JSON.decode(response.responseText);
                    this.grid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                load: function() {
                    this.controlButton(this.tabId);
                    if (pspersonConfig.emno != '') {
                        if (this.grid.getStore().getTotalCount() > 0) {
                            this.grid.getSelectionModel().selectRow(0, true);
                            new pspersonEditWindow(this.grid, false).show();
                        }
                        else {
                            new pspersonEditWindow(this.grid, true).show();
                        }
                    }
                },
                scope: this
            }
        });

        var params = {
            start: 0,
            limit: Pagination.pagingSize
        };

        if (pspersonConfig.emno.trim() != '')
            store.load({ params: params });

        return new Ext.grid.GridPanel({
            border: true,
            monitorResize: true,
            loadMask: true,
            ds: store,
            viewConfig: {
            //forceFit: true 
        },
        listeners: {
            rowclick: function() {
                this.controlButton(this.tabId);
            },
            scope: this
        },
        cm: new Ext.grid.ColumnModel([
                { header: HRMSRes.Personal_Label_emno, sortable: true, dataIndex: 'emno', hidden: false, renderer: empStateRender },
                { header: HRMSRes.Public_Label_sfid, sortable: true, dataIndex: 'sfid' },
                { header: HRMSRes.Personal_Label_ntnm, sortable: true, dataIndex: 'ntnm' },
                { header: HRMSRes.Personal_Label_ennm, sortable: true, dataIndex: 'ennm' },
                { header: HRMSRes.Personal_Label_sex, sortable: true, dataIndex: 'sex' },
                { header: HRMSRes.Personal_Label_idno, sortable: true, dataIndex: 'idno' },
                { header: HRMSRes.Personal_Label_brdt, sortable: true, dataIndex: 'brdt', type: 'date', renderer: formatDateNoTime },
                { header: HRMSRes.Personal_Label_brpl, sortable: true, dataIndex: 'brpl' },
                { header: HRMSRes.Personal_Label_mast, sortable: true, dataIndex: 'mast', renderer: mastRender },
                { header: HRMSRes.Personal_Label_emst, sortable: true, dataIndex: 'emst', renderer: empStateRender1 },
                { header: HRMSRes.Public_Label_lmtm, sortable: true, dataIndex: 'lmtm', renderer: formatDate },
                { header: HRMSRes.Public_Label_lmur, sortable: true, dataIndex: 'lmur' },
                { header: HRMSRes.Public_Label_rest, sortable: true, dataIndex: 'stus' }
            ]),
        bbar: new Ext.PagingToolbar({
            pageSize: Pagination.pagingSize,
            store: store,
            displayInfo: true,
            displayMsg: HRMSRes.Public_PagingToolbar_Total + ':{1}/{2}',
            emptyMsg: HRMSRes.Public_PagingToolbar_EmptyMsg
        })
    })
},

remove: function() {
    var sm = this.grid.getSelectionModel();
    var record = sm.getSelected();
    Ext.MessageBox.show({
        title: HRMSRes.Public_Confirm_Title,
        msg: HRMSRes.Public_Confirm_Delete,
        buttons: Ext.Msg.YESNO,
        fn: function(btn, text) {
            if (btn == 'yes') {
                Ext.Ajax.request({
                    url: ContextInfo.contextPath + '/' + pspersonPageName + '.mvc/delete',
                    success: function(response) {
                        var o = Ext.util.JSON.decode(response.responseText);
                        if (o.status == 'success') {
                            this.grid.store.remove(record);
                            this.grid.store.totalLength -= 1;
                            this.grid.getBottomToolbar().updateInfo();
                        }
                        this.grid.getBottomToolbar().diplayMsg.update(o.msg);
                        this.controlButton(this.tabId);
                    },
                    failure: function(response) {
                        Ext.MessageBox.show({
                            title: HRMSRes.Public_Message_Error,
                            msg: response.responseText,
                            buttons: Ext.MessageBox.OK,
                            icon: Ext.MessageBox.ERROR
                        });
                    },
                    scope: this,
                    params: { record: record.get(pspersonKeyColumn) }
                })
            }
        },
        icon: Ext.MessageBox.QUESTION,
        scope: this
    });
},

controlButton: function(id) {
    var enabled = !this.grid.getSelectionModel().hasSelection();
    Ext.getCmp(id + '_edit').setDisabled(enabled);
    Ext.getCmp(id + '_delete').setDisabled(enabled);
},

exportExcel: function() {
    if (this.grid.getStore().getTotalCount() <= 0) {
        this.grid.getBottomToolbar().diplayMsg.update('<font color=red>' + HRMSRes.Public_Message_ExcelNoRecord + '</font>');
        return;
    }

    var cm = this.grid.getColumnModel();
    var header = [];

    for (var i = 0; i < cm.config.length; i++) {
        if (!cm.isHidden(i)) {
            var cname = cm.config[i].header;
            var mapping = cm.config[i].dataIndex;
            header[header.length] = {
                ColumnDisplayName: cname,
                ColumnName: mapping,
                ColumnType: cm.config[i].type
            };
        }
    }
    var params = { record: Ext.encode({ params: [{ ColumnName: pspersonKeyColumn, ColumnValue: ''}], headers: header }) };

    if (this.grid.queryParams) {
        this.grid.queryParams['headers'] = header;
        delete params.record;
        params.record = Ext.encode(this.grid.queryParams);
        delete this.grid.queryParams.header;
    }

    var form = document.createElement('form');
    form.name = 'excelForm';
    form.method = 'post';
    form.action = ContextInfo.contextPath + '/' + pspersonPageName + '.mvc/exportexcel';
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


Ext.onReady(function(){ 
//    var cp = new Ext.state.CookieProvider({
//       expires: new Date(new Date().getTime()+(1000*60*60*24*10*365))
//    });
//    Ext.state.Manager.setProvider(cp);
    new pspersonPanel();
})

    </script>

</body>
</html>
