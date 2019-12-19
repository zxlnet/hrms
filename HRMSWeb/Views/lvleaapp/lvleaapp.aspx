<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="lvleaapp.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.lvleaapp.lvleaapp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var lvleaappConfig=Ext.decode('<%=ViewData["config"] %>'); 
lvleaappConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var lvleaappPageName = 'lvleaapp';
var lvleaappKeyColumn='apno';
            
var lvleaappQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	lvleaappQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(lvleaappQueryWindow,Ext.Window,{
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
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leave_Label_ltcd ,
                        name: 'ltcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store:new Ext.data.Store({ 
                            reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
                            fields: ['ValueField','DisplayField']}), autoLoad:true,
                            url:ContextInfo.contextPath+'/dropdown.mvc/getleavetype'})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_lrcd ,
                        name: 'lrcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store:new Ext.data.Store({ 
                            reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
                            fields: ['ValueField','DisplayField']}), autoLoad:true,
                            url:ContextInfo.contextPath+'/dropdown.mvc/getLeaveReason'})
    		          }]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Leave_Label_apdt + '' + HRMSRes.Public_Label_From,id:'from|apdt',
                        name:'from|apdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Leave_Label_apdt + '' + HRMSRes.Public_Label_To,id:'to|apdt',
                        name:'to|apdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leave_Label_lvst ,
                        name: 'lvst',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store:LeaveStatusStore
    		          }]},

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

var lvleaappEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	

	lvleaappEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:700, 
        height:400, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        tbar: [{ 
	        	    id:this.tabId+ '_checkleavesetting',
	                iconCls:'icon-add', 
	                text:HRMSRes.Public_Message_CLS, 
	                handler: function(){
		   		        Ext.MessageBox.show({
			                title: HRMSRes.Public_Message_LVST,
			                msg:this.leaveSummary,
			                buttons: Ext.MessageBox.OK,
			                icon:Ext.MessageBox.INFO
		                });
	                    
	                }, 
	                scope: this 
    	        },'-',{ 
	        	    id:this.tabId+ '_balance',
	                iconCls:'icon-update', 
	                text: HRMSRes.Public_Message_BALANCE,
	                scope: this
	        }],  
        items: this.basisFormPanel,
        listeners:{
            show:function(){              
                if(!this.isNew){	
			        var data=this.grid.getSelectionModel().getSelected();
                    this.basisForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            f.setValue(value);                    	            
                        }
                    });   
                    
                    var e= this.basisForm.findField('emno').getValue();
                    var l = this.basisForm.findField('ltcd').getValue();
                    var t = formatDateNoTime(this.basisForm.findField('fromtime_date').getValue()) + ' ' + 
                                formatTime(this.basisForm.findField('fromtime_time').getValue());
                    
                    this.GetLeaveSetting(e,l,t);
                
		        }
		        else
		        {
                    this.getNextAppNo();
		        }
		        
                setLastModifiedInfo(lvleaappConfig,this.basisForm);
            },
            scope:this
        },
        buttons: [{ 
            text:HRMSRes.Public_Button_Confirm, 
            iconCls:'icon-save', 
            handler: this.checkbeforesave,
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

Ext.extend(lvleaappEditWindow,Ext.Window,{
    init:function(){
        this.leaveSettings=null;
        this.leaveSummary=HRMSRes.Public_Message_NLSF;
		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();
	},
    
	createBasisFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
      		 items: [{
      		    layout:'column',
      		    items:[
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Leave_Label_apno+'(<font color=red>*</font>)',
                        name: 'apno',allowBlank:false,disabled:true,stateful:false,anchor:'95%'}]},
                    
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Public_Label_staff + '(<font color=red>*</font>)',
                        name: 'emno',stateful:false,disabled:!this.isNew,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'sfnm',valueField:'emno',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['sfnm','emno']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/ListValidPersonal'}),
                            listeners:{
                                select:function(p)
                                {
                                    var emno = p.getValue();
                                    if (emno!='')
                                    {
                                        var e = p.getValue();
                                        var l = this.basisForm.findField('ltcd').getValue();
                                        var t = formatDateNoTime(this.basisForm.findField('fromtime_date').getValue()) + ' ' + 
                                                    formatTime(this.basisForm.findField('fromtime_time').getValue());
                                        this.GetLeaveSetting(e,l,t);
                                    }
    			                },
    			                scope:this}
    		          }]},
      		        
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leave_Label_ltcd +'(<font color=red>*</font>)',
                        name: 'ltcd',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store:new Ext.data.Store({ 
                            reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
                            fields: ['ValueField','DisplayField']}), autoLoad:true,
                            url:ContextInfo.contextPath+'/dropdown.mvc/getLeaveType',
		                    listeners:{
                                    load: function(){
                                        var v = this.basisForm.findField('ltcd').getValue();
                                        this.basisForm.findField('ltcd').setValue(v);  		                            
                                    },scope:this
                            }}),
                        listeners:{
                            select:function(p){
                                var e = this.basisForm.findField('emno').getValue();
                                var l = p.getValue();
                                var t = formatDateNoTime(this.basisForm.findField('fromtime_date').getValue()) + ' ' + 
                                            formatTime(this.basisForm.findField('fromtime_time').getValue());
                                this.GetLeaveSetting(e,l,t);
                            },
                            scope:this
                        }
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_lrcd+'(<font color=red>*</font>)' ,
                        name: 'lrcd',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store:new Ext.data.Store({ 
                            reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
                            fields: ['ValueField','DisplayField']}), autoLoad:true,
                            url:ContextInfo.contextPath+'/dropdown.mvc/getLeaveReason',
		                    listeners:{
                                    load: function(){
                                        var v = this.basisForm.findField('lrcd').getValue();
                                        this.basisForm.findField('lrcd').setValue(v);  		                            
                                    },scope:this
                            }})
    		          }]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Leave_Label_apdt+'(<font color=red>*</font>)',id:'apdt',
                        name:'apdt',height:22,anchor:'95%',allowBlank:false,
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leave_Label_lvst+'(<font color=red>*</font>)' ,
                        name: 'lvst',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store:LeaveStatusStore,
                        value:'1'  
    		          }]},

 		            {columnWidth:.5,layout: 'form',
 		                items:{layout:'column',
 		                    items:[{
 		                        columnWidth:.7,
 		                        layout:'form',
 		                        items:{xtype:'datefield',fieldLabel:HRMSRes.Leave_Label_frtm+'(<font color=red>*</font>)',id:'fromtime_date',
                                    name:'fromtime_date',height:22,anchor:'100%',allowBlank:false,
                                    format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
       	 	                        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}',
       	 	                        listeners:{
       	 	                                blur:function(){
                                                var e = this.basisForm.findField('emno').getValue();
                                                var l = this.basisForm.findField('ltcd').getValue();
                                                var t = formatDateNoTime(this.basisForm.findField('fromtime_date').getValue()) + ' ' + 
                                                            formatTime(this.basisForm.findField('fromtime_time').getValue());
                                                this.GetLeaveSetting(e,l,t);

                                                var s = formatDateNoTime(this.basisForm.findField('totime_date').getValue()) + ' ' + 
                                                            formatTime(this.basisForm.findField('totime_time').getValue());
                                                
                                                this.CalcLeaveTime(e,t,s);
       	 	                                },
       	 	                                scope:this}}},
 		                        {columnWidth:.3,
 		                        layout:'form',
 		                        items:{xtype:'timefield',fieldLabel:'',format:DATE_FORMAT.TIMEONLY,
 		                        hideLabel:true,labelSeparator:'',name: 'fromtime_time',stateful:false,anchor:'82%',
 		                        listeners:{
       	 	                                blur:function(){
                                                var e = this.basisForm.findField('emno').getValue();
                                                var t = formatDateNoTime(this.basisForm.findField('fromtime_date').getValue()) + ' ' + 
                                                            formatTime(this.basisForm.findField('fromtime_time').getValue());

                                                var s = formatDateNoTime(this.basisForm.findField('totime_date').getValue()) + ' ' + 
                                                            formatTime(this.basisForm.findField('totime_time').getValue());
                                                
                                                this.CalcLeaveTime(e,t,s);
       	 	                                },
       	 	                                scope:this}
 		                        }
 		            }]}},

 		            {columnWidth:.5,layout: 'form',
 		                items:{layout:'column',
 		                    items:[{
 		                        columnWidth:.7,
 		                        layout:'form',
 		                        items:{xtype:'datefield',fieldLabel:HRMSRes.Leave_Label_totm+'(<font color=red>*</font>)',id:'totime_date',
                                    name:'totime_date',height:22,anchor:'100%',allowBlank:false,
                                    format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
       	 	                        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}',
       	 	                        listeners:{
       	 	                                blur:function(){
                                                var e = this.basisForm.findField('emno').getValue();
                                                var t = formatDateNoTime(this.basisForm.findField('fromtime_date').getValue()) + ' ' + 
                                                            formatTime(this.basisForm.findField('fromtime_time').getValue());

                                                var s = formatDateNoTime(this.basisForm.findField('totime_date').getValue()) + ' ' + 
                                                            formatTime(this.basisForm.findField('totime_time').getValue());
                                                
                                                this.CalcLeaveTime(e,t,s);
       	 	                                },
       	 	                                scope:this}}},
 		                        {columnWidth:.3,
 		                        layout:'form',
 		                        items:{xtype:'timefield',fieldLabel:'',format:DATE_FORMAT.TIMEONLY,
 		                        hideLabel:true,labelSeparator:'',name: 'totime_time',stateful:false,anchor:'82%',
   	 	                        listeners:{
   	 	                                blur:function(){
                                            var e = this.basisForm.findField('emno').getValue();
                                            var t = formatDateNoTime(this.basisForm.findField('fromtime_date').getValue()) + ' ' + 
                                                        formatTime(this.basisForm.findField('fromtime_time').getValue());

                                            var s = formatDateNoTime(this.basisForm.findField('totime_date').getValue()) + ' ' + 
                                                        formatTime(this.basisForm.findField('totime_time').getValue());
                                            
                                            this.CalcLeaveTime(e,t,s);
   	 	                                },
   	 	                                scope:this} 		                        
 		                        }
 		            }]}},

 		            
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Leave_Label_hurs,disabled:true,keepZero:true,
                        name: 'hurs',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Leave_Label_days,disabled:true,keepZero:true,
                        name: 'days',stateful:false,anchor:'95%'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Leave_Label_retm,id:'retm',
                        name:'retm',height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Leave_Label_csaf,disabled:true,keepZero:true,
                        name: 'csaf',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Leave_Label_baaf,disabled:true,keepZero:true,
                        name: 'baaf',stateful:false,anchor:'95%'}]},

       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Public_Label_remk,height:60,
                        name: 'remk',stateful:false,anchor:'95%'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_lmtm,id:'lmtm',
                        name:'lmtm',disabled:true,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATETIME,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_lmur,disabled:true,
                        name: 'lmur',stateful:false,anchor:'95%'}]}

      		    ]
      		 }] 
       })
	},
	checkbeforesave:function(){
	    if (ContextInfo.sysCfg['LvCLB']=='N') {
	        this.save();
	        return;
	    }
	    
        var lvhours = this.basisForm.findField('hurs').getValue();
        if (lvhours=='') lvhours = 0;
        
        var nobalance = false;

        if ((lvhours > this.leaveSettings.WeekBalance) && (this.leaveSettings.WeekBalance!=-1))
            nobalance=true;
        
        if ((lvhours > this.leaveSettings.MonthBalance) && (this.leaveSettings.MonthBalance!=-1))
            nobalance=true;

        if ((lvhours > this.leaveSettings.YearBalance) && (this.leaveSettings.YearBalance!=-1))
            nobalance=true;

        if (nobalance==true)
        {
            if (ContextInfo.sysCfg['LvAOLL']=='Y'){
   		        Ext.MessageBox.show({
	                title: HRMSRes.Public_Confirm_Title,
	                msg:HRMSRes.Public_Message_NEBFYAC,
	                buttons: Ext.MessageBox.YESNO,
	                icon:Ext.MessageBox.QUESTION,
	                fn:this.save,
	                scope:this
                });
            }
            else if (ContextInfo.sysCfg['LvAOLL']=='N'){
   		        Ext.MessageBox.show({
	                title: HRMSRes.Public_Message_Error,
	                msg:HRMSRes.Public_Message_NEBFYAA,
	                buttons: Ext.MessageBox.OK,
	                icon:Ext.MessageBox.ERROR,
	                scope:this
                });
            }
            
        }
        else
            this.save();
        
	},
	save: function(){
		if(!this.basisForm.isValid()) return;
		
		this.grid.getBottomToolbar().diplayMsg.update('');
		var params={};
		this.basisForm.items.each(function(f){
            if(f.isFormField){
                if ((f.getName()!='fromtime_date') && (f.getName()!='fromtime_time') && 
                    (f.getName()!='totime_date') && (f.getName()!='totime_time')) {
                    params[f.getName()]=f.getValue();
                }
            }
        });

        var v1 = formatDateNoTime(this.basisForm.findField('fromtime_date').getValue());
        var v2 = formatTimeOnly(this.basisForm.findField('fromtime_time').getValue());
        params['frtm'] = v1 + ' ' + v2;
        v1 = formatDateNoTime(this.basisForm.findField('totime_date').getValue());
        v2 = formatTimeOnly(this.basisForm.findField('totime_time').getValue());
        params['totm'] = v1 + ' ' + v2;

        var keyparams=[];
        keyparams[0]={ColumnName:'apno',ColumnValue:this.basisForm.findField('apno').getValue()};

	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + lvleaappPageName + '.mvc/'+method,
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		   		
		   		if (o.status=='success'){
		   		    var store=this.grid.store;		   		    
		   		    if(!this.isNew){
		   		        //var sm=this.grid.getSelectionModel();
    	   		        //var record=sm.getSelected();
		   		        //for(var p in params){
		   		        //    record.set(p,params[p]);
		   		        //}
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
		   params: {record:Ext.util.JSON.encode({params:params,keycolumns:keyparams})}
		});
	},
    getNextAppNo:function(){
        var params={};
	    Ext.Ajax.request({
	       url: ContextInfo.contextPath+'/lvleaapp.mvc/getNewAppNo',
	       success: function(response){
	   		    var o= Ext.util.JSON.decode(response.responseText);		
	   		    if (o.status=='success'){
                    this.basisForm.findField('apno').setValue(o.msg);
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
	       params: {record:Ext.util.JSON.encode(params)}
	    });
    },    
    GetLeaveSetting:function(emno,ltcd,fromdate){
        if (emno.trim()=='') return;    
        if (ltcd.trim()=='') return;
        if (fromdate.trim()=='') return;
        
	    var params={};
	    params['emno'] = emno;
	    params['ltcd'] = ltcd;
	    params['fromdate'] = fromdate;
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/lvleaapp.mvc/getEmpLeaveSettings',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		
		   		if (o.status=='success'){
                    this.leaveSettings = Ext.decode(o.msg);
                    this.FormatMessage();
		   		}else{
		   		    Ext.MessageBox.show({
			            title: HRMSRes.Public_Message_Error,
			            msg:o.msg,
			            buttons: Ext.MessageBox.OK,
			            icon:Ext.MessageBox.ERROR
		            });
		   		}
		   },
		   failure:function(response){
		    var o= Ext.util.JSON.decode(response.responseText);
		   },
		   scope:this,
		   params: {record:Ext.util.JSON.encode(params)}
		});
    },
    FormatMessage:function(){
        var w = HRMSRes.Leave_Label_wkba + ":"  + "<font color=red>" + (this.leaveSettings.WeekBalance=="-1"?HRMSRes.Leave_Label_unli:this.leaveSettings.WeekBalance) + " hrs</font>";
        var m = HRMSRes.Leave_Label_mnba + ":" + "<font color=red>" + (this.leaveSettings.MonthBalance=="-1"?HRMSRes.Leave_Label_unli:this.leaveSettings.MonthBalance) + " hrs</font>";
        var y = HRMSRes.Leave_Label_yrba + ":"  + "<font color=red>" + (this.leaveSettings.YearBalance=="-1"?HRMSRes.Leave_Label_unli:this.leaveSettings.YearBalance) + " hrs</font>";
        
        Ext.getCmp(this.tabId+ '_balance').setText(w + "," + m + "," + y);

        this.leaveSummary='';
        
        for (var i=0;i<this.leaveSettings.SummaryText.length;i++)
        {
            this.leaveSummary = this.leaveSummary + "</br>" + this.leaveSettings.SummaryText[i];
        }
    },
    CalcLeaveTime:function(emno,fromdate,todate){
        if (emno.trim()=='') return;       
        if (fromdate.trim()=='') return;
        if (todate.trim()=='') return;
        
	    var params={};
	    params['emno'] = emno;
	    params['fromdate'] = fromdate;
	    params['todate'] = todate;
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/lvleaapp.mvc/CalcLeaveTime',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		
		   		if (o.status=='success'){
                    //this.leaveSettings = Ext.decode(o.msg);
                    this.basisForm.findField('hurs').setValue(o.totallvhours);
                    this.basisForm.findField('days').setValue(o.totallvdays);
                    
                    this.basisForm.findField('baaf').setValue(this.leaveSettings.YearBalance - o.totallvhours);
                    this.basisForm.findField('csaf').setValue(this.leaveSettings.YearConsume + o.totallvhours);
                    
		   		}else{
		   		    Ext.MessageBox.show({
			            title: HRMSRes.Public_Message_Error,
			            msg:o.msg,
			            buttons: Ext.MessageBox.OK,
			            icon:Ext.MessageBox.ERROR
		            });
		   		}
		   },
		   failure:function(response){
		    var o= Ext.util.JSON.decode(response.responseText);
		   },
		   scope:this,
		   params: {record:Ext.util.JSON.encode(params)}
		});
    }
});


var lvleaappPanel=function(){
    this.tabId=lvleaappConfig.tabId;
	this.init();	
	
	lvleaappPanel.superclass.constructor.call(this,{
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
	            hidden:lvleaappConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new lvleaappEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden:lvleaappConfig.auth[this.tabId+'_' + 'edit']!='True',
	            handler: function(){
	            	new lvleaappEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:lvleaappConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:lvleaappConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new lvleaappQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:lvleaappConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
	            handler: this.exportExcel, 
	            scope: this 
	        },'->',{ 
	        	xtype:'label', 
	            html:  '<b><font color=green>employee no: ' + lvleaappConfig.emno + '</font></b>',
	            hidden: lvleaappConfig.emno==''
	        },'->',{ 
                id:this.tabId+'_logview',
                iconCls:'icon-logview', 
                text:'', 
                handler: function(){
                        var data=this.grid.getSelectionModel().getSelected();if (data==null) return;var rfid = data.get('rfid');new logViewerWindow(rfid).show();},
                scope: this 
            },{ 
                id:this.tabId+'_muf',
                iconCls:lvleaappConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,lvleaappConfig.muf?'delete':'add',lvleaappConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(lvleaappPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var lvleaappStoreType=Ext.data.Record.create([
		    {name:'emno'},
            {name:'sfid'},
            {name:'stfn'},
            {name:'apno'},
            {name:'ltcd'},
            {name:'ltnm'},
            {name:'lrcd'},
            {name:'lrnm'},
            {name:'apdt'},
            {name:'lvst'},
            {name:'frtm'},
            {name:'fromtime_date'},
            {name:'fromtime_time'},
            {name:'totm'},
            {name:'totime_date'},
            {name:'totime_time'},
            {name:'hurs'},
            {name:'days'},
            {name:'retm'},
            {name:'csaf'},
            {name:'baaf'},
            {name:'remk'},
            {name:'lmtm'},
            {name:'lmur'},
            {name:'rfid'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},lvleaappStoreType), 	
	   	    //baseParams:{record:Ext.encode({params:[{ColumnName:lvleaappKeyColumn,ColumnValue:lvleaappConfig.emno}]})},   
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:"emno",ColumnValue:lvleaappConfig.emno},
	   	                                           {ColumnName:"from|frtm",ColumnValue:lvleaappConfig.lvdt},
	   	                                           {ColumnName:"to|frtm",ColumnValue:lvleaappConfig.lvdt}
	   	                                           ]})},   
	   		url:ContextInfo.contextPath+'/' + lvleaappPageName + '.mvc/list',
	   		listeners:{
                loadexception:function(o,t,response)
                {
                    var o= Ext.util.JSON.decode(response.responseText);		   		
                    this.grid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                load:function(p){
                    this.controlButton(this.tabId);

                    for (var i=0;i<p.getCount();i++)
                    {
                        p.getAt(i).set('apdt',formatDateNoTime(p.getAt(i).get('apdt')));   
                        p.getAt(i).set('frtm',formatDate(p.getAt(i).get('frtm')));   
                        p.getAt(i).set('totm',formatDate(p.getAt(i).get('totm')));   
                    }
                    p.commitChanges();

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
	            render:function(){
            		this.controlButton(this.tabId);
	            },
	            scope:this
            },                      
            cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Leave_Label_apno,sortable: true, dataIndex: 'apno'},
                {header:HRMSRes.Public_Label_sfid,sortable: true, dataIndex: 'sfid'},
                {header:HRMSRes.Public_Label_sftn,sortable: true, dataIndex: 'stfn'},
                {header:HRMSRes.Leave_Label_ltnm,sortable: true, dataIndex: 'ltnm'},
                {header:HRMSRes.Master_Label_lrnm,sortable: true, dataIndex: 'lrnm'},
                {header:HRMSRes.Leave_Label_apdt,sortable: true, dataIndex: 'apdt',renderer:formatDateNoTime},
                {header:HRMSRes.Leave_Label_frtm,sortable: true, dataIndex: 'frtm',renderer:formatDate},
                {header:HRMSRes.Leave_Label_totm,sortable: true, dataIndex: 'totm',renderer:formatDate},
                {header:HRMSRes.Leave_Label_hurs,sortable: true, dataIndex: 'hurs'},
                {header:HRMSRes.Leave_Label_days,sortable: true, dataIndex: 'days'},
                {header:HRMSRes.Leave_Label_lvst,sortable: true, dataIndex: 'lvst'},
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
        keyparams[0]={ColumnName:'apno',ColumnValue:record.get('apno')};

	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ lvleaappPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:lvleaappKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + lvleaappPageName + '.mvc/exportexcel';
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
    new lvleaappPanel();
})

    </script>

</body>
</html>