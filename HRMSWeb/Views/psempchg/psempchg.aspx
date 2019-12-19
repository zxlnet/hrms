<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="psempchg.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.psempchg.psempchg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var psempchgConfig=Ext.decode('<%=ViewData["config"] %>'); 
psempchgConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var psempchgPageName = 'psempchg';
var psempchgKeyColumn='emno';
            
var psempchgQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	psempchgQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(psempchgQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_chre,
                        name: 'chre',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_chnm,
                        name: 'ctcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/public.mvc/getChangeType',
		                    listeners:{load:function(){var f=this.basisForm.findField('ctcd');f.setValue(f.getValue())},scope:this}})
    		          }]},   		                      

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_efdt,id:'from|efdt',
                        name:'from|efdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_efdt,id:'to|efdt',
                        name:'to|efdt',editable:false,height:22,anchor:'95%',
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

var psempchgDtlEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	

	psempchgDtlEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:350, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.basisDtlFormPanel,
        listeners:{
            show:function(){              
                if(!this.isNew){	
			        var data=this.grid.getSelectionModel().getSelected();
                    this.basisDtlForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            if (f.xtype=='timefield')
                                value = formatTime(value);
                            if (f.xtype=='datefield')
                                value = formatDateNoTime(value);
                            f.setValue(value);}});}
               
                setLastModifiedInfo(psempchgConfig,this.basisDtlForm);
                
                if (data.get('issu')=='Y') Ext.getCmp('dtladd').setDisabled(true); else  Ext.getCmp('dtladd').setDisabled(false);

            },
            scope:this
        },
        buttons: [{ 
            text:HRMSRes.Public_Button_Confirm, 
            id:'dtladd',
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

Ext.extend(psempchgDtlEditWindow,Ext.Window,{
    init:function(){
		this.basisDtlFormPanel=this.createBasisFormPanel();
		this.basisDtlForm=this.basisDtlFormPanel.getForm();
	},
	createBasisFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
      		 items: [
      		    {},{
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
		                    listeners:{load:function(){f = this.basisDtlForm.findField('emno');f.setValue(f.getValue());},scope:this}})
    		          }]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sqno,
                        name: 'sqno',stateful:false,anchor:'95%',disabled:true}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_chnm,
                        name: 'ctcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getChangeType',
		                    listeners:{load:function(){var f=this.basisDtlForm.findField('ctcd');f.setValue(f.getValue())},scope:this}})
    		          }]},   		                      

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_chfi,
                        name: 'chby',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField','MiscField1','MiscField2']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getChangeField',
		                    listeners:{load:function(t,r){
                                        var f = this.basisDtlForm.findField('chby');var v = f.getValue();f.setValue(v);var selIndex = -1;	 
                                        for (var i=0;i<t.getCount();i++){if (t.getAt(i).get('ValueField')==v){selIndex = i; break;}}f.fireEvent('select',f,t.getAt(selIndex));
		                    },scope:this}}),
		                listeners:{
		                    select:function(c,r,i){
                                var f = this.basisDtlForm.findField('neva');f.store.baseParams={record:Ext.encode(r.get('MiscField1'))};f.store.load();this.basisDtlForm.findField('chfi').setValue(r.get('MiscField2'));},
		                    scope:this
		                }		                    
    		          }]},   		                      

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_neva,
                        name: 'neva',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/executeAction',
		                    listeners:{load:function(){var f=this.basisDtlForm.findField('neva');f.setValue(f.getValue())},scope:this}}),
		                listeners:{
		                    select:function(c,r,i){
                                var f = this.basisDtlForm.findField('nevt');f.setValue(r.get('ValueField'));},
		                    scope:this
		                }}]},   		                      

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_isim ,
                        name: 'isim',stateful:false,editable:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_efdt,id:'efdt',
                        name:'efdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_gpid,
                        name: 'gpid',stateful:false,anchor:'95%',disabled:true}]},

       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Personal_Label_chre,height:70,
                        name: 'chre',stateful:false,anchor:'98%'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_lmtm,id:'lmtm',
                        name:'lmtm',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,disabled:true,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_lmur,disabled:true,
                        name: 'lmur',stateful:false,anchor:'95%'}]},
                        
       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:'nevt',disabled:true,hidden:true,hideLabel:true,
                        name: 'nevt',stateful:false,anchor:'95%'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:'chfi',disabled:true,hidden:true,hideLabel:true,
                        name: 'chfi',stateful:false,anchor:'95%'}]}
      		    ]
      		 }] 
       })
	},
	save: function(){
		if(!this.basisDtlForm.isValid()) return;
		this.grid.getBottomToolbar().diplayMsg.update('');
		var params={};
		this.basisDtlForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + psempchgPageName + '.mvc/'+method,
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

var psempchgEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	
    this.dtldeletedline=[]; 

    this.fullFormPanel = {
                xtype:'panel',
	            layout:'fit',
	            autoHeight:true,
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
                            //hidden:roleConfig.auth[this.tabId+'_rolemgt_add']!='True',
	                        handler: function(){
	                            this.addline();	                         
	                        }, 
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_deletedtl',
	                        iconCls:'icon-remove', 
                            //hidden:roleConfig.auth[this.tabId+'_rolemgt_delete']!='True',
	                        text:HRMSRes.Public_Toolbar_Delete, 
	                        handler: this.deleteline, 
	                        scope: this 
	                    }],
		            items:this.dtlgrid
	            }]
            };

           
	psempchgEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:500, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.fullFormPanel,
        listeners:{
            show:function(){    
		        var keyField = this.basisForm.findField(psempchgKeyColumn);
                if(!this.isNew){	
			        var data=this.grid.getSelectionModel().getSelected();
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

Ext.extend(psempchgEditWindow,Ext.Window,{
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
	         autoHeight:true,
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
		                    url:ContextInfo.contextPath+'/dropdown.mvc/ListValidPersonal',
		                    listeners:{load:function(){f = this.basisForm.findField('emno');f.setValue(f.getValue());},scope:this}})
    		          }]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_gpid,
                        name: 'gpid',stateful:false,anchor:'95%',disabled:true}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_chnm,
                        name: 'ctcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getChangeType',
		                    listeners:{load:function(){var f=this.basisForm.findField('ctcd');f.setValue(f.getValue())},scope:this}})
    		          }]},   		                      

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_isim ,
                        name: 'isim',stateful:false,editable:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_efdt,id:'efdt',
                        name:'efdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_chre,
                        name: 'chre',stateful:false,anchor:'95%'}]},

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
	
    createGridPanel:function(){

		this.psempchgEditDtlStoreType=Ext.data.Record.create([
            {name:'chfi'},{name:'neva'},{name:'nevt'},{name:'isim'},{name:'efdt'},{name:'chre'},{name:'chby'},{name:'dfbytext'},
            {name:'chfi'},{name:'dasc'}
            
		]);
        
		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},this.psempchgEditDtlStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:psempchgKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + psempchgPageName + '.mvc/getChgDtl',
	   		listeners:{
                loadexception:function(o,t,response)
                {
                    var o= Ext.util.JSON.decode(response.responseText);		   		
                    //this.dtlgrid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                load:function(p){
                    this.controlButton(this.tabId);
                    
                    for (var i=0;i<p.getCount();i++){
                        var r = p.getAt(i);
                        r.set('efdt',formatDateNoTime(r.get('efdt')));}
                    p.commitChanges();
                    
                },
                add:function(p,r,i){
                    var v = p.getAt(i);
                    //v.set('remark',v.get('remark') +' ');
                },
                scope:this
            }
        });
        //alert(store.url);
        var params={
            start:0,
            limit:Pagination.pagingSize
        };
        
        //store.load({params:params});
        var isimCheckColumn = new Ext.grid.CheckColumn({
               header: HRMSRes.Personal_Label_isim,
               dataIndex: 'isim',
               width: 55
            });
        
        return new Ext.grid.EditorGridPanel({
            id:'chgDtlGrid',
    		border:true, 
            height:290,
    		monitorResize:false, 
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
	            rowclick:function(){
	                this.controlButton(this.tabId);
	            },
	            scope:this
            },                  
            sm: new Ext.grid.RowSelectionModel(),              
            cm: new Ext.grid.ColumnModel([ 
            {header: HRMSRes.Personal_Label_chfi,dataIndex: 'chby',width: 150,
                editor: new Ext.form.ComboBox({typeAhead: true,triggerAction: 'all',mode:'local',
                lazyRender:false,listClass: 'x-combo-list-small',
                displayField: 'DisplayField',valueField:'DisplayField',
                store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField','MiscField1','MiscField2']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getChangeField'}),
		        listeners:{
		            select:function(c,r,i)
		            {var sm = this.dtlgrid.getSelectionModel();var rec= sm.getSelected();rec.set('chfi',r.data['MiscField2']);rec.set('cbyv',r.data['ValueField']);
                        var f = Ext.getCmp('nevaedt');f.store.baseParams={record:Ext.encode(r.get('MiscField1'))};f.store.load();},scope:this}
		     })},             
            {header: HRMSRes.Personal_Label_neva,dataIndex: 'neva',width: 150,id:'neva',
                editor: new Ext.form.ComboBox({typeAhead: true,triggerAction: 'all',mode:'local',id:'nevaedt',
                lazyRender:false,listClass: 'x-combo-list-small',
                displayField: 'DisplayField',valueField:'DisplayField',
                store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/executeAction'}),
		        listeners:{select:function(c,r,i){var sm = this.dtlgrid.getSelectionModel();var rec= sm.getSelected();rec.set('chfv',r.data['ValueField']);},scope:this}
		     })},

            isimCheckColumn,
            
            {header: HRMSRes.Public_Label_efdt,dataIndex: 'efdt',width: 130,renderer: formatDateNoTime,
                editor: new Ext.form.DateField({format: DATE_FORMAT.DATEONLY,minValue: '01/01/06',disabledDays: [0, 6],
                disabledDaysText: ''})},
            
            {header:'chfi',id:'chfi',sortable: true, dataIndex: 'chfi',hidden:true},
            {header:'chfv',id:'chfv',sortable: true, dataIndex: 'chfi',hidden:true},
            {header:'cbyv',id:'cbyv',sortable: true, dataIndex: 'cbyv',hidden:true},

            {id:'chre',header: HRMSRes.Personal_Label_chre,dataIndex: 'chre',width: 220,
                editor: new Ext.form.TextField({})}
            
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
	
	save: function(){
		if(!this.basisForm.isValid()) return;
		this.grid.getBottomToolbar().diplayMsg.update('');
		var params={};
        var dtlparams=[];       
        var sm=this.dtlgrid.store.getModifiedRecords();
        for (var i =0; i< sm.length;i++ ){
            var x = sm[i];
            var p = {emno:this.basisForm.findField('emno').getValue(),
                     ctcd:this.basisForm.findField('ctcd').getValue(),
                     chre:x.get('chre').trim()==''?this.basisForm.findField('chre').getValue():x.get('chre'),
                     chfi:x.get('chfi'),chby:x.get('cbyv'),neva:x.get('chfv'),isim:x.get('isim'),efdt:x.get('efdt'),
                     nevt:x.get('neva')};
            
            dtlparams[dtlparams.length] = p;
        };
        
        var keyparams=[];
        keyparams[0]={ColumnName:'gpid',ColumnValue:this.basisForm.findField('gpid').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + psempchgPageName + '.mvc/'+method,
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
		   params: {record:Ext.encode({params:params,keycolumns:keyparams,dtlparams:dtlparams,dtldeletedline:this.dtldeletedline})}
		});
	},
	Query:function(){	    
	    this.dtlgrdAction = 'query';
        var params=[];	
        var f = this.basisForm.findField('gpid');        
        var p={
            ColumnName:f.getName(),
            ColumnValue:f.getValue()                
        };

        params[params.length]=p;                  
        //params[params.length]={ColumnName:'ActionType',ColumnValue:'Query'};                  

        var loadParams={start:0,limit:Pagination.pagingSize};
        /***modified for adjquery**/
        this.dtlgrid.queryParams={
            params:params
        };
        this.dtlgrid.store.baseParams={record:Ext.util.JSON.encode(this.dtlgrid.queryParams),action:'query'};
	    this.dtlgrid.getStore().load({params:loadParams});
    },
    addline:function(){	    
        var n=this.dtlgrid.getStore().count;
        var params=[];
        params['chfi']='';
        params['neva']='' ;
        params['isim']=this.basisForm.findField('isim').getValue();
        params['efdt']=this.basisForm.findField('efdt').getValue();
        params['chre']='';

        var store = this.dtlgrid.getStore();
        store.add(new store.recordType(params));
;
    },
	deleteline:function(){
	    var sm=this.dtlgrid.getSelectionModel();
	    
	    if (sm.hasSelection())
	    {
	        var record=sm.getSelected();
            
            this.dtlgrid.store.remove(record);
            this.dtlgrid.store.totalLength-=1;
            this.dtlgrid.getBottomToolbar().updateInfo();   
        }
        
        this.controlButton(this.tabId);
	},
	controlButton:function(id){
        var enabled=!this.dtlgrid.getSelectionModel().hasSelection();	    
        Ext.getCmp(id+ '_deletedtl').setDisabled(enabled);
    }    
});

var psempchgPanel=function(){
    this.tabId=psempchgConfig.tabId;
	this.init();	
	
	psempchgPanel.superclass.constructor.call(this,{
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
	            //hidden:psempchgConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new psempchgEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden: psempchgConfig.auth[this.tabId + '_' + 'edit'] != 'True',
	            disabled:true,
	            handler: function(){
	            	new psempchgDtlEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove',
                hidden: psempchgConfig.auth[this.tabId + '_' + 'delete'] != 'True',
                disabled: true,
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:psempchgConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new psempchgQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:psempchgConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
	            handler: this.exportExcel, 
	            scope: this 
	        },'->',{ 
	        	xtype:'label', 
	            html:  '<b><font color=green>employee no: ' + psempchgConfig.emno + '</font></b>',
	            hidden: psempchgConfig.emno==''
	        },'->',{ 
                id:this.tabId+'_logview',
                iconCls:'icon-logview', 
                text:'', 
                handler: function(){
                        var data=this.grid.getSelectionModel().getSelected();if (data==null) return;var rfid = data.get('rfid');new logViewerWindow(rfid).show();},
                scope: this 
            },{ 
                id:this.tabId+'_muf',
                iconCls:psempchgConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){
                        updateMUF(this.tabId,psempchgConfig.muf?'delete':'add',psempchgConfig,this.grid);
                        },
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(psempchgPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var psempchgStoreType=Ext.data.Record.create([
		    {name:'emno'},
            {name:'sfid'},
            {name:'stfn'},
            {name:'sqno'},
            
            {name:'chfi'},
            {name:'chby'},
            {name:'chre'},
            {name:'ctcd'},
            {name:'ctnm'},
            {name:'lmtm'},
            {name:'lmur'},
            {name:'efdt'},
            {name:'neva'},
            {name:'isim'},
            {name:'issu'},
            {name:'isdt'},
            {name:'isby'},
            {name:'gpid'},
            {name:'dfrs'},
            {name:'firs'},
            {name:'finm'},
            {name:'dfnm'},
            {name:'chft'},
            {name:'nevt'},
            {name:'dfbytext'},
            {name:'rfid'}
            
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},psempchgStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:psempchgKeyColumn,ColumnValue:psempchgConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + psempchgPageName + '.mvc/list',
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
	            scope:this
            },                      
            cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Public_Label_sfid,sortable: true, dataIndex: 'sfid',hidden:false},
                {header:HRMSRes.Public_Label_sftn,sortable: true, dataIndex: 'stfn',hidden:false},
                //{header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno'},

                //{header:HRMSRes.rsid',sortable: true, dataIndex: 'rsid'},
                {header:HRMSRes.Master_Label_chnm,sortable: true, dataIndex: 'ctnm'},
                {header:HRMSRes.Personal_Label_chfi,sortable: true, dataIndex: 'dfbytext'},
                {header:HRMSRes.Personal_Label_neva,sortable: true, dataIndex: 'nevt'},
                {header:HRMSRes.Personal_Label_chre,sortable: true, dataIndex: 'chre'},
                {header:HRMSRes.Personal_Label_isim,sortable: true, dataIndex: 'isim'},
                {header:HRMSRes.Public_Label_efdt,sortable: true, dataIndex: 'efdt',renderer:formatDateNoTime},
                {header:HRMSRes.Personal_Label_issu,sortable: true, dataIndex: 'issu'},
                {header:HRMSRes.Public_Label_isdt,sortable: true, dataIndex: 'isdt',renderer:formatDateNoTime},
                {header:HRMSRes.Public_Label_isby,sortable: true, dataIndex: 'isby'}
//                {header:HRMSRes.Public_Label_lmur,sortable: true, dataIndex: 'lmur'},
//                {header:HRMSRes.Public_Label_lmtm,sortable: true, dataIndex: 'lmtm',renderer:formatDateNoTime}

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

        if (record.get('issu')=='Y'){
	        Ext.MessageBox.show({
		        title:HRMSRes.Public_Message_Error,
		        msg:'The record you select was issued,delete denied.',
		        buttons: Ext.MessageBox.OK,
		        icon:Ext.MessageBox.ERROR
	        });     
	        return;   
        }

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
	   			        url:ContextInfo.contextPath+ '/'+ psempchgPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:psempchgKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + psempchgPageName + '.mvc/exportexcel';
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
    new psempchgPanel();
})

    </script>

</body>
</html>