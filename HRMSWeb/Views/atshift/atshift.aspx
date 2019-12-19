<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="atshift.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.atshift.atshift" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var atshiftConfig=Ext.decode('<%=ViewData["config"] %>'); 
atshiftConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var atshiftPageName = 'atshift';
var atshiftKeyColumn='sfcd';
            
var atshiftQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	atshiftQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(atshiftQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Attendance_Label_sfcd,
                        name: 'sfcd',stateful:false,anchor:'95%',UpperOnBlur:'true'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Attendance_Label_sfnm,
                        name: 'sfnm',stateful:false,anchor:'95%'}]},

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

var atshiftEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	

    this.personalTab=new Ext.TabPanel({
                autoTabs:true,
                activeTab:0,
                border:false,
                layoutOnTabChange:true,
                frame:true,
                autoHeight:true,
                items:[{id:'tab1',title: HRMSRes.Personal_Label_Basis,items: this.basisFormPanel},
                       {id:'tab2',title: HRMSRes.Personal_Label_Others,items: this.otherFormPanel}]
            }),
                       
	atshiftEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:390, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.personalTab,
        listeners:{
            show:function(){              
		        var keyField = this.basisForm.findField(atshiftKeyColumn);

                if(!this.isNew){	
			        var data=this.grid.getSelectionModel().getSelected();
                    this.basisForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            f.setValue(value);                    	            
                        }
                    });   
                    this.personalTab.setActiveTab('tab2');
                    this.otherForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            f.setValue(value);                    	            
                        }
                    }); 
                    setLastModifiedInfo(atshiftConfig,this.otherForm);
                    this.personalTab.setActiveTab('tab1');                      
		        }
		        else
		        {
                    this.personalTab.setActiveTab('tab2');
	                setLastModifiedInfo(atshiftConfig,this.otherForm);
                    this.personalTab.setActiveTab('tab1');                      
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

Ext.extend(atshiftEditWindow,Ext.Window,{
    init:function(){
		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();
		
		this.otherFormPanel=this.createOtherFormPanel();
		this.otherForm=this.otherFormPanel.getForm();
		
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Attendance_Label_sfcd + '(<font color=red>*</font>)',disabled:!this.isNew,
                        name: 'sfcd',allowBlank:false,stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Attendance_Label_sfnm+ '(<font color=red>*</font>)',
                        name: 'sfnm',allowBlank:false,stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'timefield',fieldLabel:HRMSRes.Attendance_Label_tmin+ '(<font color=red>*</font>)',format:DATE_FORMAT.TIMEONLY,
                        name: 'tmin',allowBlank:false,stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'timefield',fieldLabel:HRMSRes.Attendance_Label_tmot+ '(<font color=red>*</font>)',format:DATE_FORMAT.TIMEONLY,
                        name: 'tmot',allowBlank:false,stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'timefield',fieldLabel:HRMSRes.Attendance_Label_eati,format:DATE_FORMAT.TIMEONLY,
                        name: 'eati',allowBlank:false,stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'timefield',fieldLabel:HRMSRes.Attendance_Label_lato,format:DATE_FORMAT.TIMEONLY,
                        name: 'lato',allowBlank:false,stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Attendance_Label_wkda+ '(<font color=red>*</font>)',disabled:false,
                        name: 'wkda',allowBlank:false,stateful:false,anchor:'95%',decimalPrecision:1,keepZero:true,value:'0'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Attendance_Label_wkhr+ '(<font color=red>*</font>)',disabled:false,
                        name: 'wkhr',allowBlank:false,stateful:false,anchor:'95%',decimalPrecision:1,keepZero:true,value:'0'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Attendance_Label_ifio ,
                        name: 'ifio',stateful:false,editable:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',value:'N',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Attendance_Label_bken + '(<font color=red>*</font>)',
                        name: 'bken',stateful:false,editable:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',value:'N',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'timefield',fieldLabel:HRMSRes.Attendance_Label_btst,format:DATE_FORMAT.TIMEONLY,
                        name: 'btst',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'timefield',fieldLabel:HRMSRes.Attendance_Label_bked,format:DATE_FORMAT.TIMEONLY,
                        name: 'bked',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Attendance_Label_bser ,
                        name: 'bser',stateful:false,editable:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',value:'N',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Attendance_Label_bker ,
                        name: 'bker',stateful:false,editable:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',value:'N',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

//      		        {columnWidth:.5,layout: 'form',
//                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Attendance_Label_eatm,disabled:false,
//                        name: 'eatm',allowBlank:false,stateful:false,anchor:'95%',decimalPrecision:0,value:'0'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Attendance_Label_ecty ,
                        name: 'ecty',stateful:false,editable:false,typeAhead: true,value:'N',
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: EarylyStatStore}]},

//      		        {columnWidth:.5,layout: 'form',
//                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Attendance_Label_latm,disabled:false,value:'0',
//                        name: 'latm',allowBlank:false,stateful:false,anchor:'95%',decimalPrecision:0}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Attendance_Label_lcty ,
                        name: 'lcty',stateful:false,editable:false,typeAhead: true,value:'N',
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: LateStatStore}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Attendance_Label_altm,disabled:false,value:'0',
                        name: 'altm',allowBlank:false,stateful:false,anchor:'95%',decimalPrecision:0}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Attendance_Label_aotm,disabled:false,value:'0',
                        name: 'aotm',allowBlank:false,stateful:false,anchor:'95%',decimalPrecision:0}]}

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
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Attendance_Label_miot,disabled:false,value:'0',
                        name: 'miot',stateful:false,anchor:'95%',decimalPrecision:0}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'timefield',fieldLabel:HRMSRes.Attendance_Label_nots,format:DATE_FORMAT.TIMEONLY,
                        name: 'nots',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Attendance_Label_otun,disabled:false,value:'0',
                        name: 'otun',stateful:false,anchor:'95%',decimalPrecision:0}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'timefield',fieldLabel:HRMSRes.Attendance_Label_r1st,format:DATE_FORMAT.TIMEONLY,
                        name: 'r1st',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'timefield',fieldLabel:HRMSRes.Attendance_Label_r1ed,format:DATE_FORMAT.TIMEONLY,
                        name: 'r1ed',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Attendance_Label_rttm1,disabled:false,value:'0',
                        name: 'rttm1',stateful:false,anchor:'95%',decimalPrecision:0}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'timefield',fieldLabel:HRMSRes.Attendance_Label_r2st,format:DATE_FORMAT.TIMEONLY,
                        name: 'r2st',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'timefield',fieldLabel:HRMSRes.Attendance_Label_r2ed,format:DATE_FORMAT.TIMEONLY,
                        name: 'r2ed',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Attendance_Label_rttm2,disabled:false,value:'0',
                        name: 'rttm2',stateful:false,anchor:'95%',decimalPrecision:0}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Attendance_Label_igbl ,
                        name: 'igbl',stateful:false,editable:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',value:'N',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Attendance_Label_igrl ,
                        name: 'igrl',stateful:false,editable:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',value:'Y',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

      		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Public_Label_remk,height:70,
                        name: 'remk',stateful:false,anchor:'98%'}]},

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
	save: function(){
		if(!this.basisForm.isValid()) return;
		if(!this.otherForm.isValid()) return;
		this.grid.getBottomToolbar().diplayMsg.update('');
		var params={};
		this.personalTab.setActiveTab('tab1');
		this.basisForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });
        this.personalTab.setActiveTab('tab2');
		this.otherForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });
        var keyparams=[];
        keyparams[0]={ColumnName:'sfcd',ColumnValue:this.basisForm.findField('sfcd').getValue()};
        this.personalTab.setActiveTab('tab1');
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + atshiftPageName + '.mvc/'+method,
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
		   params: {record:Ext.encode({params:params,keycolumns:keyparams})}
		});
	}
});


var atshiftPanel=function(){
    this.tabId=atshiftConfig.tabId;
	this.init();	
	
	atshiftPanel.superclass.constructor.call(this,{
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
	            hidden:atshiftConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new atshiftEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden:atshiftConfig.auth[this.tabId+'_' + 'edit']!='True',
	            handler: function(){
	            	new atshiftEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:atshiftConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:atshiftConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new atshiftQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:atshiftConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
	            handler: this.exportExcel, 
	            scope: this 
	        },'-',{ 
                xtype:'combo', 
                id: 'atshiftRefFunc',
                store: new Ext.data.SimpleStore({
                    fields: ['value', 'name'],
                    data : [
                        ['1',HRMSRes.Public_Menu_atshfely],
                        ['2',HRMSRes.Public_Menu_atshflat],
                        ['3',HRMSRes.Public_Menu_atshfott]
                      ]
                }),
                displayField: 'name',
		        valueField: 'value',
		        stateful:false,
                typeAhead: true,
                mode: 'local',
                editable:false,
                triggerAction: 'all',
                anchor:'95%',
			    emptyText:HRMSRes.Public_Toolbar_Related,//HRMSRes.Public_Label_Choose,
                width:200,
                listeners:{
                    select:function(p)
                    {
                        var sfcd = this.grid.getSelectionModel().getSelected().get('sfcd');
                        if (this.grid.getSelectionModel().hasSelection()){
                           if (p.getValue()=='1'){mainPanel.loadClass(ContextInfo.contextPath+'/atshfely.mvc/index?menuId=M3070&sfcd='+sfcd,'M3070','',HRMSRes.Public_Menu_atshfely,'','true');}
                           if (p.getValue()=='2'){mainPanel.loadClass(ContextInfo.contextPath+'/atshflat.mvc/index?menuId=M3080&sfcd='+sfcd,'M3080','',HRMSRes.Public_Menu_atshflat,'','true');}
                           if (p.getValue()=='3') {mainPanel.loadClass(ContextInfo.contextPath+'/atshfott.mvc/index?menuId=M3090&sfcd='+sfcd,'M3090','',HRMSRes.Public_Menu_atshfott,'','true');}
                        }
	    			},
	    			scope:this
	    		  } 
            },'->',{ 
                id:this.tabId+'_logview',
                iconCls:'icon-logview', 
                text:'', 
                handler: function(){
                        var data=this.grid.getSelectionModel().getSelected();if (data==null) return;var rfid = data.get('rfid');new logViewerWindow(rfid).show();},
                scope: this 
            },{ 
                id:this.tabId+'_muf',
                iconCls:atshiftConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,atshiftConfig.muf?'delete':'add',atshiftConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(atshiftPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var atshiftStoreType=Ext.data.Record.create([
            {name:'altm'},
            {name:'aotm'},
            {name:'bken'},
            {name:'bked'},
            {name:'bker'},
            {name:'btst'},
            {name:'bser'},
            {name:'ecty'},
            {name:'eatm'},
            {name:'eati'},
            {name:'ifio'},
            {name:'lcty'},
            {name:'latm'},
            {name:'lato'},
            {name:'miot'},
            {name:'nots'},
            {name:'otun'},
            {name:'remk'},
            {name:'r1ed'},
            {name:'r1st'},
            {name:'r2ed'},
            {name:'r2st'},
            {name:'rttm1'},
            {name:'rttm2'},
            {name:'sfnm'},
            {name:'sfcd'},
            {name:'stfn'},
            {name:'tmin'},
            {name:'tmot'},
            {name:'wkda'},
            {name:'wkhr'},            
            {name:'lmtm'},
            {name:'lmur'},
            {name:'igbl'},
            {name:'igrl'},
            {name:'rfid'}
            
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},atshiftStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:atshiftKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + atshiftPageName + '.mvc/list',
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
                {header:HRMSRes.Attendance_Label_sfcd,sortable: true, dataIndex: 'sfcd',hidden:false},
                {header:HRMSRes.Attendance_Label_sfnm,sortable: true, dataIndex: 'sfnm',hidden:false},
                {header:HRMSRes.Attendance_Label_tmin,sortable: true, dataIndex: 'tmin',renderer:formatTimeOnly},
                {header:HRMSRes.Attendance_Label_tmot,sortable: true, dataIndex: 'tmot',renderer:formatTimeOnly},
                {header:HRMSRes.Attendance_Label_wkda,sortable: true, dataIndex: 'wkda'},
                {header:HRMSRes.Attendance_Label_wkhr,sortable: true, dataIndex: 'wkhr'},
                {header:HRMSRes.Attendance_Label_ifio,sortable: true, dataIndex: 'ifio'},
                {header:HRMSRes.Attendance_Label_bken,sortable: true, dataIndex: 'bken'},
                {header:HRMSRes.Attendance_Label_bker,sortable: true, dataIndex: 'bker'},
                {header:HRMSRes.Attendance_Label_bser,sortable: true, dataIndex: 'bser'}
//                {header:HRMSRes.Public_Label_lmtm,sortable: true, dataIndex: 'lmtm',renderer:formatDate},
//                {header:HRMSRes.Public_Label_lmur,sortable: true, dataIndex: 'lmur'}
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
        keyparams[0]={ColumnName:'sfcd',ColumnValue:record.get('sfcd')};
        
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ atshiftPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:atshiftKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + atshiftPageName + '.mvc/exportexcel';
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
    new atshiftPanel();
})

    </script>

</body>
</html>