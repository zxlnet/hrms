<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="otaplctn.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.otaplctn.otaplctn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var otaplctnConfig=Ext.decode('<%=ViewData["config"] %>'); 
otaplctnConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var otaplctnPageName = 'otaplctn';
var otaplctnKeyColumn='apno';
            
var otaplctnQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	otaplctnQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(otaplctnQueryWindow,Ext.Window,{
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
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Overtime_Label_otcd ,
                        name: 'otcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store:new Ext.data.Store({ 
                            reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
                            fields: ['ValueField','DisplayField']}), autoLoad:true,
                            url:ContextInfo.contextPath+'/dropdown.mvc/getOvertimetype'})
    		          }]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Overtime_Label_apdt + '' + HRMSRes.Public_Label_From,id:'from|apdt',
                        name:'from|apdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Overtime_Label_apdt + '' + HRMSRes.Public_Label_To,id:'to|apdt',
                        name:'to|apdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Overtime_Label_otst,
                        name: 'otst',stateful:false,anchor:'95%'}]},

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

var otaplctnEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	


     
	otaplctnEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:700, 
        height:400, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        tbar: [{ 
	        	    id:this.tabId+ '_checkotsetting',
	                iconCls:'icon-add', 
	                text: HRMSRes.Public_Message_COS, 
	                handler: function(){
		   		        Ext.MessageBox.show({
			                title: HRMSRes.Public_Message_OTST,
			                msg:this.overtimeSummary,
			                buttons: Ext.MessageBox.OK,
			                icon:Ext.MessageBox.INFO
		                });
	                    
	                }, 
	                scope: this 
    	        },'-',{ 
	        	    id:this.tabId+ '_balance',
	                iconCls:'icon-update', 
	                text:HRMSRes.Public_Message_BALANCE,
	                scope: this
	        }],  
        items: this.basisFormPanel,
        listeners:{
            show:function(){              
		        var keyField = this.basisForm.findField(otaplctnKeyColumn);

                if(!this.isNew){	
			        var data=this.grid.getSelectionModel().getSelected();
                    this.basisForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            f.setValue(value);                    	            
                        }
                    });   
                    
                      this.callLeaveSetting();
                }
		        else{
                    this.getNextAppNo();
		        }
                setLastModifiedInfo(otaplctnConfig,this.basisForm);
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

Ext.extend(otaplctnEditWindow,Ext.Window,{
    init:function(){
        this.overtimeSettings=null;
        this.overtimeSummary=HRMSRes.Public_Message_NOSF;
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Overtime_Label_apno+'(<font color=red>*</font>)',
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
                                {this.getNextAppNo();var emno = p.getValue();if (emno!=''){this.callLeaveSetting();}},scope:this}
    		          }]},
      		        
     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Overtime_Label_apdt+'(<font color=red>*</font>)',id:'apdt',
                        name:'apdt',height:22,anchor:'95%',allowBlank:false,
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Overtime_Label_otst+'(<font color=red>*</font>)' ,
                        name: 'otst',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store:OvertimeStatusStore,
                        value:'1'  
    		          }]},

 		            {columnWidth:.5,layout: 'form',
 		                items:{layout:'column',
 		                    items:[{
 		                        columnWidth:.7,
 		                        layout:'form',
 		                        items:{xtype:'datefield',fieldLabel:HRMSRes.Overtime_Label_frtm +'(<font color=red>*</font>)',id:'fromtime_date',
                                    name:'fromtime_date',height:22,anchor:'100%',allowBlank:false,
                                    format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
       	 	                        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}',
       	 	                        listeners:{
       	 	                                blur:function(){
                                                var e = this.basisForm.findField('emno').getValue();
                                                var l = this.basisForm.findField('otcd').getValue();
                                                var t = formatDateNoTime(this.basisForm.findField('fromtime_date').getValue()) + ' ' + 
                                                            formatTime(this.basisForm.findField('fromtime_time').getValue());
                                                this.GetovertimeSetting(e,l,t);

                                                var s = formatDateNoTime(this.basisForm.findField('totime_date').getValue()) + ' ' + 
                                                            formatTime(this.basisForm.findField('totime_time').getValue());
                                                
                                                this.CalcOTTime(e,t,s);
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
                                                
                                                this.CalcOTTime(e,t,s);
       	 	                                },
       	 	                                scope:this}
 		                        }
 		            }]}},

 		            {columnWidth:.5,layout: 'form',
 		                items:{layout:'column',
 		                    items:[{
 		                        columnWidth:.7,
 		                        layout:'form',
 		                        items:{xtype:'datefield',fieldLabel:HRMSRes.Overtime_Label_totm+'(<font color=red>*</font>)',id:'totm_date',
                                    name:'totm_date',height:22,anchor:'100%',allowBlank:false,
                                    format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
       	 	                        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}',
       	 	                        listeners:{
       	 	                                blur:function(){
                                                var e = this.basisForm.findField('emno').getValue();
                                                var t = formatDateNoTime(this.basisForm.findField('fromtime_date').getValue()) + ' ' + 
                                                            formatTime(this.basisForm.findField('fromtime_time').getValue());

                                                var s = formatDateNoTime(this.basisForm.findField('totime_date').getValue()) + ' ' + 
                                                            formatTime(this.basisForm.findField('totime_time').getValue());
                                                
                                                this.CalcOTTime(e,t,s);
       	 	                                },
       	 	                                scope:this}}},
 		                        {columnWidth:.3,
 		                        layout:'form',
 		                        items:{xtype:'timefield',fieldLabel:'',format:DATE_FORMAT.TIMEONLY,
 		                        hideLabel:true,labelSeparator:'',name: 'totm_time',stateful:false,anchor:'82%',
   	 	                        listeners:{
   	 	                                blur:function(){
                                            var e = this.basisForm.findField('emno').getValue();
                                            var t = formatDateNoTime(this.basisForm.findField('fromtime_date').getValue()) + ' ' + 
                                                        formatTime(this.basisForm.findField('fromtime_time').getValue());

                                            var s = formatDateNoTime(this.basisForm.findField('totime_date').getValue()) + ' ' + 
                                                        formatTime(this.basisForm.findField('totime_time').getValue());
                                            
                                            this.CalcOTTime(e,t,s);
   	 	                                },
   	 	                                scope:this} 		                        
 		                        }
 		            }]}},

 		            
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Overtime_Label_othr,disabled:true,keepzero:true,
                        name: 'othr',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Overtime_Label_othm,disabled:false,keepzero:true,
                        name: 'othm',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Overtime_Label_otcd +'(<font color=red>*</font>)',
                        name: 'otcd',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store:new Ext.data.Store({ 
                            reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
                            fields: ['ValueField','DisplayField']}), autoLoad:true,
                            url:ContextInfo.contextPath+'/dropdown.mvc/getOvertimeType',
		                    listeners:{
                                    load: function(){
                                        var v = this.basisForm.findField('otcd').getValue();
                                        this.basisForm.findField('otcd').setValue(v);  		                            
                                        this.callLeaveSetting();
                                    },scope:this
                            }}),
                        listeners:{
                            select:function(p){
                                  this.callLeaveSetting();
                            },
                            scope:this
                        }
    		          }]},


       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Public_Label_remk,height:70,
                        name: 'remk',stateful:false,anchor:'98%'}]},

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
	    if (ContextInfo.sysCfg['OtCOTB']=='N') {
	        this.save();
	        return;
	    }
	    
        var lvhours = this.basisForm.findField('othr').getValue();
        if (lvhours=='') lvhours = 0;
        
        var nobalance = false;

        if ((lvhours > this.overtimeSettings.WeekBalance) && (this.overtimeSettings.WeekBalance!=-1))
            nobalance=true;
        
        if ((lvhours > this.overtimeSettings.MonthBalance) && (this.overtimeSettings.MonthBalance!=-1))
            nobalance=true;

        if ((lvhours > this.overtimeSettings.YearBalance) && (this.overtimeSettings.YearBalance!=-1))
            nobalance=true;

        if (nobalance==true)
        {
            if (ContextInfo.sysCfg['OtAOOTL']=='Y'){
   		        Ext.MessageBox.show({
	                title: HRMSRes.Public_Confirm_Title,
	                msg:HRMSRes.Public_Message_NEBFYAC,
	                buttons: Ext.MessageBox.YESNO,
	                icon:Ext.MessageBox.QUESTION,
	                fn:this.save,
	                scope:this
                });
            }
            else if (ContextInfo.sysCfg['AtAOOTL']=='N'){
   		        Ext.MessageBox.show({
	                title: HRMSRes.Public_Message_Error,
	                msg:HRMSRes.Public_Message_NEBFYAAS,
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
		   url: ContextInfo.contextPath+'/' + otaplctnPageName + '.mvc/'+method,
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
		   url: ContextInfo.contextPath+'/otaplctn.mvc/getNewAppNo',
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
		   failure:function(response){
		        //alert('failure');
		        var o= Ext.util.JSON.decode(response.responseText);
//	   		    Ext.MessageBox.show({
//		            title: HRMSRes.Public_Message_Error,
//		            msg:o.msg,
//		            buttons: Ext.MessageBox.OK,
//		            icon:Ext.MessageBox.ERROR
//	            });
		   },
		   scope:this,
		   params: {record:Ext.util.JSON.encode(params)}
		});
    },
    GetovertimeSetting:function(emno,otcd,fromdate){
        if (emno.trim()=='') return;    
        if (otcd.trim()=='') return;
        if (fromdate.trim()=='') return;
        
	    var params={};
	    params['emno'] = emno;
	    params['otcd'] = otcd;
	    params['frtm'] = fromdate;
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/otaplctn.mvc/getEmpOTSettings',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		
		   		if (o.status=='success'){
                    this.overtimeSettings = Ext.decode(o.msg);
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
        var w = HRMSRes.Overtime_Label_wkba + ":"  + "<font color=red>" + (this.overtimeSettings.WeekBalance=="-1"?HRMSRes.Overtime_Label_unli:this.overtimeSettings.WeekBalance) + " hrs</font>";
        var m = HRMSRes.Overtime_Label_mnba + ":" + "<font color=red>" + (this.overtimeSettings.MonthBalance=="-1"?HRMSRes.Overtime_Label_unli:this.overtimeSettings.MonthBalance) + " hrs</font>";
        var y = HRMSRes.Overtime_Label_yrba + ":"  + "<font color=red>" + (this.overtimeSettings.YearBalance=="-1"?HRMSRes.Overtime_Label_unli:this.overtimeSettings.YearBalance) + " hrs</font>";
        
        Ext.getCmp(this.tabId+ '_balance').setText(w + "," + m + "," + y);

        this.overtimeSummary='';
        
        for (var i=0;i<this.overtimeSettings.SummaryText.length;i++)
        {
            this.overtimeSummary = this.overtimeSummary + "</br>" + this.overtimeSettings.SummaryText[i];
        }
    },
    CalcOTTime:function(emno,fromdate,todate){
        if (emno.trim()=='') return;       
        if (fromdate.trim()=='') return;
        if (todate.trim()=='') return;
        
	    var params={};
	    params['emno'] = emno;
	    params['frtm'] = fromdate;
	    params['totm'] = todate;
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/otaplctn.mvc/CalcOTTime',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		
		   		if (o.status=='success'){
                    //this.overtimeSettings = Ext.decode(o.msg);
                    this.basisForm.findField('othr').setValue(o.totalothr);
                    this.basisForm.findField('otcd').setValue(o.otcd);
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
    callLeaveSetting:function(){
        var e = this.basisForm.findField('emno').getValue();
        var l = this.basisForm.findField('otcd').getValue();
        var t = formatDateNoTime(this.basisForm.findField('fromtime_date').getValue()) + ' ' + 
                    formatTime(this.basisForm.findField('fromtime_time').getValue());
        this.GetovertimeSetting(e,l,t);
    } 
});


var otaplctnPanel=function(){
    this.tabId=otaplctnConfig.tabId;
	this.init();	
	
	otaplctnPanel.superclass.constructor.call(this,{
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
	            hidden:otaplctnConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new otaplctnEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden: otaplctnConfig.auth[this.tabId + '_' + 'edit'] != 'True',
	            disabled:true,
	            handler: function(){
	            	new otaplctnEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove',
                hidden: otaplctnConfig.auth[this.tabId + '_' + 'delete'] != 'True',
                disabled:true,
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:otaplctnConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new otaplctnQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:otaplctnConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
	            handler: this.exportExcel, 
	            scope: this 
	        },'->',{ 
	        	xtype:'label', 
	            html:  '<b><font color=green>employee no: ' + otaplctnConfig.emno + '</font></b>',
	            hidden: otaplctnConfig.emno==''
	        },'->',{ 
                id:this.tabId+'_logview',
                iconCls:'icon-logview', 
                text:'', 
                handler: function(){
                        var data=this.grid.getSelectionModel().getSelected();if (data==null) return;var rfid = data.get('rfid');new logViewerWindow(rfid).show();},
                scope: this 
            },{ 
                id:this.tabId+'_muf',
                iconCls:otaplctnConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,otaplctnConfig.muf?'delete':'add',otaplctnConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(otaplctnPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var otaplctnStoreType=Ext.data.Record.create([
		    {name:'emno'},
            {name:'sfid'},
            {name:'stfn'},
            {name:'apno'},
            {name:'otcd'},
            {name:'otnm'},
            {name:'apdt'},
            {name:'otst'},
            {name:'frtm'},
            {name:'fromtime_date'},
            {name:'fromtime_time'},
            {name:'totm'},
            {name:'totime_date'},
            {name:'totime_time'},
            {name:'othr'},
            {name:'othm'},
            {name:'remk'},
            {name:'lmtm'},
            {name:'lmur'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},otaplctnStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:otaplctnKeyColumn,ColumnValue:otaplctnConfig.emno}]})},   
	   	    //baseParams:{record:Ext.encode({params:[{ColumnName:"emno",ColumnValue:atoridatConfig.emno},
	   	    //                                       {ColumnName:"from|frtm",ColumnValue:atoridatConfig.atdt},
	   	    //                                       {ColumnName:"to|frtm",ColumnValue:atoridatConfig.atdt}
	   	    //                                       ]})},   
	   		url:ContextInfo.contextPath+'/' + otaplctnPageName + '.mvc/list',
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
                {header:HRMSRes.Overtime_Label_apno,sortable: true, dataIndex: 'apno'},
                {header:HRMSRes.Public_Label_sfid,sortable: true, dataIndex: 'sfid'},
                {header:HRMSRes.Public_Label_sftn,sortable: true, dataIndex: 'stfn'},
                {header:HRMSRes.Overtime_Label_otnm,sortable: true, dataIndex: 'otnm'},
                {header:HRMSRes.Overtime_Label_apdt,sortable: true, dataIndex: 'apdt',renderer:formatDateNoTime},
                {header:HRMSRes.Overtime_Label_frtm,sortable: true, dataIndex: 'frtm',renderer:formatDate},
                {header:HRMSRes.Overtime_Label_totm,sortable: true, dataIndex: 'totm',renderer:formatDate},
                {header:HRMSRes.Overtime_Label_othr,sortable: true, dataIndex: 'othr'},
                {header:HRMSRes.Overtime_Label_othm,sortable: true, dataIndex: 'othm'},
                {header:HRMSRes.Overtime_Label_otst,sortable: true, dataIndex: 'otst'},
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
	   			        url:ContextInfo.contextPath+ '/'+ otaplctnPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:otaplctnKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + otaplctnPageName + '.mvc/exportexcel';
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
    new otaplctnPanel();
})

    </script>

</body>
</html>