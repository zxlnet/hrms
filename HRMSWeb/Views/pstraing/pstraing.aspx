<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pstraing.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.pstraing.pstraing" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var pstraingConfig=Ext.decode('<%=ViewData["config"] %>'); 
pstraingConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var pstraingPageName = 'pstraing';
var pstraingKeyColumn='emno';
            
var pstraingQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	pstraingQueryWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:400, 
        height:350, 
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

Ext.extend(pstraingQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_cunm,
                        name: 'cunm',stateful:false,anchor:'95%'}]},
            
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_orga,
                        name: 'orga',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_tmnm ,
                        name: 'ttcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getTrainingType',
		                    listeners:{load:function(){f = this.form.findField('ttcd');f.setValue(f.getValue());},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_incm ,
                        name: 'incm',stateful:false,editable:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

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

var pstraingEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	
                      
    this.trainingTab=new Ext.TabPanel({
                autoTabs:true,
                activeTab:0,
                border:false,
                layoutOnTabChange:true,
                frame:true,
                autoHeight:true,
                items:[{id:'tab1',title: 'Basis',items: this.basisFormPanel},
                       {id:'tab2',title: 'Others',items: this.otherFormPanel}]}),
                           
	pstraingEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:400, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.trainingTab,
        listeners:{
            show:function(){              
		        var keyField = this.basisForm.findField(pstraingKeyColumn);

                if(!this.isNew){	
			        var data=this.grid.getSelectionModel().getSelected();
                    this.trainingTab.setActiveTab('tab2');
                    this.otherForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            f.setValue(value);                    	            
                        }
                    });   

			        this.trainingTab.setActiveTab('tab1');
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
		            keyField.setValue(pstraingConfig.emno);
		            if (pstraingConfig.emno!='')
		                getMaxsqno(pstraingConfig.emno,pstraingConfig.tableName,this.basisForm);
		        }
		        var keyValue = keyField.getValue();
                setLastModifiedInfo(pstraingConfig,this.basisForm);
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

Ext.extend(pstraingEditWindow,Ext.Window,{
    init:function(){
		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();
		this.otherFormPanel=this.createOtherFormPanel()
		this.otherForm=this.otherFormPanel.getForm();
	},
	createBasisFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         title:'Basis',
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
                            listeners:{select:function(p){var emno = p.getValue();if (emno!=''){getMaxsqno(emno,pstraingConfig.tableName,this.basisForm);}},scope:this}
    		          }]},
      		        
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sqno+'(<font color=red>*</font>)',
                        name: 'sqno',allowBlank:false,disabled:true,stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_cunm+'(<font color=red>*</font>)',
                        name: 'cunm',allowBlank:false,stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_trcd,
                        name: 'trcd',allowBlank:true,stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_tmnm ,
                        name: 'ttcd',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getTrainingType',
		                    listeners:{load:function(){f = this.basisForm.findField('ttcd');f.setValue(f.getValue());},scope:this}})
    		          }]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_orga,
                        name: 'orga',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_loca,
                        name: 'loca',stateful:false,anchor:'95%'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Personal_Label_frdt,id:'stdt',
                        name:'stdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,disabled:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Personal_Label_todt,id:'endt',
                        name:'endt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,disabled:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Personal_Label_cufe,keepZero:true,
                        name: 'cufe',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_crnm ,
                        name: 'crcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getCurrency',
		                    listeners:{load:function(){f = this.basisForm.findField('crcd');f.setValue(f.getValue());},scope:this}})
    		          }]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_natu,
                        name: 'natu',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_paby,
                        name: 'paby',stateful:false,anchor:'95%'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Personal_Label_rldt,id:'redt',
                        name:'redt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,disabled:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_incm ,
                        name: 'incm',stateful:false,editable:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Personal_Label_tttm,keepZero:true,
                        name: 'tttm',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Personal_Label_ctpw,keepZero:true,
                        name: 'ctpw',stateful:false,anchor:'95%'}]},

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
	         title:'Others',
      		 items: [
      		    {},{
      		    layout:'form',
      		    items:[
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Personal_Label_resu,height:70,
                        name: 'resu',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Personal_Label_bnag,height:70,
                        name: 'bnag',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Personal_Label_tmpe,keepZero:true,
                        name: 'tmpe',stateful:false,anchor:'95%'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Public_Label_remk,height:70,
                        name: 'remk',stateful:false,anchor:'95%'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_rest,disabled:true,
                        name: 'stus',stateful:false,anchor:'95%',nonUpdateField:true}]}

      		    ]
      		 }] 
       })
	},	
	save: function(){
		if(!this.basisForm.isValid()) return;
		this.grid.getBottomToolbar().diplayMsg.update('');
		var params={};
		this.trainingTab.setActiveTab('tab1');
		this.basisForm.items.each(function(f){
            if(f.isFormField && !f.nonUpdateField){
                params[f.getName()]=f.getValue();
            }
        });
        this.trainingTab.setActiveTab('tab2');
		this.otherForm.items.each(function(f){
            if(f.isFormField && !f.nonUpdateField){
                params[f.getName()]=f.getValue();
            }
        });
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + pstraingPageName + '.mvc/'+method,
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


var pstraingPanel=function(){
    this.tabId=pstraingConfig.tabId;
	this.init();	
	
	pstraingPanel.superclass.constructor.call(this,{
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
	            hidden:pstraingConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new pstraingEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden: pstraingConfig.auth[this.tabId + '_' + 'edit'] != 'True',
	            disabled: true,
	            handler: function(){
	            	new pstraingEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove',
                hidden: pstraingConfig.auth[this.tabId + '_' + 'delete'] != 'True',
                disabled: true,
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:pstraingConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new pstraingQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:pstraingConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
	            handler: this.exportExcel, 
	            scope: this 
	        },'->',{ 
	        	xtype:'label', 
	            html:  '<b><font color=green>employee no: ' + pstraingConfig.emno + '</font></b>',
	            hidden: pstraingConfig.emno==''
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
                iconCls:pstraingConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,pstraingConfig.muf?'delete':'add',pstraingConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(pstraingPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var pstraingStoreType=Ext.data.Record.create([
		    {name:'emno'},
            {name:'sfid'},
            {name:'stfn'},
            {name:'sqno'},
            
            {name:'bnag'},
            {name:'cufe'},
            {name:'cunm'},
            {name:'ctpw'},
            {name:'crcd'},
            {name:'endt'},
            {name:'loca'},
            {name:'natu'},
            {name:'orga'},
            {name:'paby'},
            {name:'redt'},
            {name:'resu'},
            {name:'stdt'},
            {name:'tmpe'},
            {name:'tttm'},
            {name:'trcd'},
            {name:'ttcd'},
            {name:'trnm'},
            {name:'ttnm'},         
            {name:'incm'},
               
            {name:'remk'},                     
            {name:'lmtm'},
            {name:'lmur'},
            {name:'stus'},
            {name:'rfid'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},pstraingStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:pstraingKeyColumn,ColumnValue:pstraingConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + pstraingPageName + '.mvc/list',
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
		       // forceFit: true 
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

                {header:HRMSRes.Personal_Label_cunm,sortable: true, dataIndex: 'cunm'},
                {header:HRMSRes.Personal_Label_orga,sortable: true, dataIndex: 'orga'},
                {header:HRMSRes.Master_Label_tmnm,sortable: true, dataIndex: 'ttnm'},
                {header:HRMSRes.Personal_Label_frdt,sortable: true, dataIndex: 'stdt',renderer:formatDateNoTime},
                {header:HRMSRes.Personal_Label_todt,sortable: true, dataIndex: 'endt',renderer:formatDateNoTime},
                {header:HRMSRes.Personal_Label_resu,sortable: true, dataIndex: 'resu'},
                {header:HRMSRes.Personal_Label_incm,sortable: true, dataIndex: 'incm'},
                
                {header:HRMSRes.Public_Label_lmtm,sortable: true, dataIndex: 'lmtm',renderer:formatDate},
                {header:HRMSRes.Public_Label_lmur,sortable: true, dataIndex: 'lmur'},
                {header:HRMSRes.Public_Label_rest,sortable: true, dataIndex: 'stus'}
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
	   			        url:ContextInfo.contextPath+ '/'+ pstraingPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:pstraingKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + pstraingPageName + '.mvc/exportexcel';
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
    new pstraingPanel();
})

    </script>

</body>
</html>