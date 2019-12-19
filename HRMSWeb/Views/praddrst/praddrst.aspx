<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="praddrst.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.praddrst.praddrst" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var praddrstConfig=Ext.decode('<%=ViewData["config"] %>'); 
praddrstConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var praddrstPageName = 'praddrst';
var praddrstKeyColumn='emno';
            
var praddrstQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	praddrstQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(praddrstQueryWindow,Ext.Window,{
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

var praddrstDtlEditWindow=function(rec,store,flag){
	this.feedbackStore=store;
	this.isNewDtl = flag;
	this.feedbackRec = rec;
	this.init();	

	praddrstDtlEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:300, 
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
                setLastModifiedInfo(praddrstConfig,this.basisDtlEditForm);
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

Ext.extend(praddrstDtlEditWindow,Ext.Window,{
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
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_perd + '(<font color=red>*</font>)',
                        name: 'perd',stateful:false,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'perd',valueField:'perd',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['perd','perd']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/public.mvc/listAllPeriod',
		                    listeners:{
                                load:function(t,r){f = this.basisDtlEditForm.findField('perd');
                                        var v = f.getValue();f.setValue(v);
                                },scope:this}})
    		          }]},     		        

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_itcd + '(<font color=red>*</font>)',
                        name: 'itcd',stateful:false,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['DisplayField','ValueField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getSalaryItems',
		                    listeners:{
                                load:function(t,r){f = this.basisDtlEditForm.findField('itcd');
                                        var v = f.getValue();f.setValue(v);var selIndex = -1;	 
                                        for (var i=0;i<t.getCount();i++){if (t.getAt(i).get('ValueField')==v){selIndex = i; break;}}
                                        f.fireEvent('select',f,t.getAt(selIndex));
                                },scope:this}}),
                            listeners:{select:function(c,r,i){this.selecteditnm = r.get('DisplayField');},
                                scope:this}
    		          }]},
      		        
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Payroll_Label_rlcd,
                        name: 'rlcd',stateful:false,anchor:'95%'}]},
      		        
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Payroll_Label_valu+'(<font color=red>*</font>)',
                        name: 'valu',stateful:false,anchor:'95%',allowBlank:false,keepZero:true}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_pdcd +'(<font color=red>*</font>)',
                        name: 'pdcd',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['DisplayField','ValueField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getPayDay',
		                    listeners:{
                                load:function(){f = this.basisDtlEditForm.findField('pdcd');f.setValue(f.getValue());},
                                scope:this}})
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
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_isca ,
                        name: 'isca',stateful:false,editable:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'text',valueField:'value',
                        store: FlagYesNoStore}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Public_Label_remk,height:70,
                        name: 'remk',stateful:false,anchor:'95%'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_lmtm,id:'itmlmtm',
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
	confirm:function(){
		var params={};
		this.basisDtlEditForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });
	    params['itnm'] = this.selecteditnm;
	    
	    if (this.isNewDtl) {
	        this.feedbackStore.add(new this.feedbackStore.recordType(params));
	    }
        else {
            this.feedbackRec.set('perd',params['perd']);
            this.feedbackRec.set('sqno',params['sqno']);
            this.feedbackRec.set('itcd',params['itcd']);
            this.feedbackRec.set('itnm',params['itnm']);
            this.feedbackRec.set('rlcd',params['rlcd']);
            this.feedbackRec.set('valu',params['valu']);
            this.feedbackRec.set('pdcd',params['pdcd']);
            this.feedbackRec.set('crcd',params['crcd']);
            this.feedbackRec.set('crnm',params['crnm']);
            this.feedbackRec.set('isca',params['isca']);
            this.feedbackRec.set('remk',params['remk']);
            this.feedbackRec.set('lmtm',params['lmtm']);
            this.feedbackRec.set('lmur',params['lmur']);
        }
	    this.close();
	}
});

var praddrstEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	

    this.fullFormPanel = {
                xtype:'panel',
	            layout:'fit',
	            items:[
	            {
		            xtype:'panel',
		            autoHeight:true,
		            bodyStyle:'padding:0px;',
		            border:false,
		            baseCls:'x-fieldset-noborder',
		            columnWidth: .50,
		            items:this.basisFormPanel
	            },
	            
	            {
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
	                    }],
		            items:this.dtlgrid
	            }]
            };
                       
	praddrstEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:750, 
        height:470, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.fullFormPanel,
        listeners:{
            show:function(){    
		        var keyField = this.basisForm.findField(praddrstKeyColumn);
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
                
                if (!this.isNew)
                   this.Query();
             },
            scope:this
        },
        buttons: [{ 
            text:HRMSRes.Public_Button_ApplyTo, 
            id: this.tabId + '_applyto',
            iconCls:'icon-applyto', 
            handler: this.applyto,
            //disabled:true,
            scope: this
        },{ 
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

Ext.extend(praddrstEditWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_emno,
                     name: 'emno',stateful:false,anchor:'95%',disabled:true}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sfid,
                     name: 'sfid',stateful:false,anchor:'95%',disabled:true}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sftn,disabled:true,
                        name: 'stfn',stateful:false,anchor:'95%'}]}
      		    ]
      		 }] 
       })
	},
	
    createGridPanel:function(){
		var praddrstDtlStoreType=Ext.data.Record.create([
            {name:'perd'},
            {name:'sqno'},
            {name:'itcd'},
            {name:'itnm'},
            {name:'rlcd'},
            {name:'valu'},
            {name:'pdcd'},
            {name:'isca'},
            {name:'crcd'},
            {name:'crnm'},
            {name:'remk'},
            {name:'lmtm'},
            {name:'lmur'}
		]);
        
		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},praddrstDtlStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:praddrstKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + praddrstPageName + '.mvc/getAddRuleSetDtl',
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
        
        var scm=new Ext.grid.CheckboxSelectionModel();
               
        return new Ext.grid.EditorGridPanel({
            id:'praddrstDtlGrid',
    		border:true, 
    		monitorResize:false, 
            height:310,
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
            sm: scm,//new Ext.grid.RowSelectionModel(),               
            cm: new Ext.grid.ColumnModel([ 
            scm,
            {header:HRMSRes.Public_Label_sqno,id:'sqno',sortable: true, dataIndex: 'sqno',width:30},
            {header:HRMSRes.Master_Label_perd,id:'perd',sortable: true, dataIndex: 'perd',width:80},
            {header:HRMSRes.Payroll_Label_itcd,id:'itcd',sortable: true, dataIndex: 'itcd',width:100},
            {header:HRMSRes.Payroll_Label_itnm,id:'itnm',sortable: true, dataIndex: 'itnm'},
            {header:HRMSRes.Payroll_Label_valu,id:'valu',sortable: true, dataIndex: 'valu'},
            {header:HRMSRes.Public_Label_remk,id:'remk',sortable: true, dataIndex: 'remk'},
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
//		alert(this.isNew);
//		this.basisForm.items.each(function(f){
//            if(f.isFormField){
//                params[f.getName()]=f.getValue();
//            }
//        });
        params['emno']=this.basisForm.findField('emno').getValue();
        var dtlparams=[];       
        var st = this.dtlgrid.getStore();
        for (var i =1; i<= st.getCount();i++ ){
            var x = st.getAt(i-1);
		    var v=x.get('valu').toString().indexOf('.')!=-1?x.get('valu').toString():(x.get('valu').toString()+'.00');
            var p = {emno:this.basisForm.findField('emno').getValue(),
                     sqno:x.get('sqno'),itcd:x.get('itcd'),
                     rlcd:x.get('rlcd'),valu:v,
                     pdcd:x.get('pdcd'),
                     isca:x.get('isca'),crcd:x.get('crcd'),
                     perd:x.get('perd'),remk:x.get('remk'),
                     lmtm:x.get('lmtm'),lmur:x.get('lmur')};
            
            dtlparams[dtlparams.length] = p;
        }
        
        var keyparams=[];
        keyparams[0]={ColumnName:'emno',ColumnValue:this.basisForm.findField('emno').getValue()};
        //keyparams[0]={ColumnName:'perd',ColumnValue:this.basisForm.findField('perd').getValue()};
        //keyparams[1]={ColumnName:'dasq',ColumnValue:this.basisForm.findField('dasq').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + praddrstPageName + '.mvc/'+method,
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

		   		    Ext.MessageBox.show({
			            title: HRMSRes.Public_Message_EditWell,
			            msg:HRMSRes.Public_Message_EditWell,
			            buttons: Ext.MessageBox.OK,
			            icon:Ext.MessageBox.INFO
		            });

		   		    //this.close();
		   		    Ext.getCmp(this.tabId + '_applyto').setDisabled(false);
		   		    
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
		   params: {record:Ext.encode({params:params['emno'],keycolumns:keyparams,dtlparams:dtlparams})}
		});
	},
	Query:function(){	    
        var params=[];	
        var f = this.basisForm.findField(praddrstKeyColumn);        
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
        params['emno']=this.basisForm.findField('emno').getValue();
        params['rlcd']='';
        params['valu']='';
        params['pdcd'] = ContextInfo.sysCfg['PrDFPD'];
        params['crcd'] = ContextInfo.sysCfg['PrDFCUR'];
        params['crnm']='';
        params['isca']='';
        params['perd']='';
        params['remk']='';
        params['lmtm']= formatDateTime(new Date());
        params['lmur']=praddrstConfig.currentUser;

        var store = this.dtlgrid.getStore();
        new praddrstDtlEditWindow(new store.recordType(params),store,true).show();
    },
    editline:function(){	    
        var n=this.dtlgrid.getStore().getCount() + 1;
        var rec = this.dtlgrid.getSelectionModel().getSelected();
        var store = this.dtlgrid.getStore();    
        new praddrstDtlEditWindow(rec,store,false).show();
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
	exportExcelLine:function(){
	    if (this.dtlgrid.getStore().getTotalCount()<=0){
	        this.dtlgrid.getBottomToolbar().diplayMsg.update('<font color=red>' + HRMSRes.Public_Message_ExcelNoRecord + '</font>');
	        return;
	    }
	    
	    var cm=this.dtlgrid.getColumnModel();
	    var header=[];	    
	    
	    for(var i=1;i<cm.config.length;i++){
	        if(!cm.isHidden(i)){
	            var cname=cm.config[i].header;	            
	            var mapping=cm.config[i].dataIndex;
	            header[header.length]={
	                ColumnDisplayName:cname,
	                ColumnName:mapping
	            };
	        }          
	    }
	    var params={record:Ext.encode({params:[{ColumnName:praddrstKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.dtlgrid.queryParams){
            this.dtlgrid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.dtlgrid.queryParams);
            delete this.dtlgrid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + praddrstPageName + '.mvc/exportexceldtl';
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
    },
    applyto:function(){
        var params={};

        var dtlparams=[];       
        var records = this.dtlgrid.getSelectionModel().getSelections()
        
        if (records.length<1) {
            this.dtlgrid.getBottomToolbar().diplayMsg.update('<font color=red>' + HRMSRes.Public_Message_NoSelectedRecords + '</font>');  		
            return;
        }
        
		for(var i=0;i<records.length;i++){
		    var x=records[i];
		    var v=x.get('valu').toString().indexOf('.')!=-1?x.get('valu').toString():(x.get('valu').toString()+'.00');
            var p = {emno:this.basisForm.findField('emno').getValue(),
                     sqno:x.get('sqno'),itcd:x.get('itcd'),
                     rlcd:x.get('rlcd'),
                     valu:v,
                     pdcd:x.get('pdcd'),
                     isca:x.get('isca'),
                     crcd:x.get('crcd'),
                     perd:x.get('perd'),
                     remk:x.get('remk'),
                     lmtm:x.get('lmtm'),lmur:x.get('lmur')};
            
            dtlparams[dtlparams.length] = p;
        }
        
        var url = ContextInfo.contextPath+'/' + praddrstPageName + '.mvc/applyto';
        new empAdvQryQueryWindow(this.dtlgrid,'1',url,dtlparams).show();
    }    
});


var praddrstPanel=function(){
    this.tabId=praddrstConfig.tabId;
	this.init();	
	
	praddrstPanel.superclass.constructor.call(this,{
		applyTo:this.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	autoScroll:true,      
	        tbar: [
	        
//	        { 
//	        	id:this.tabId+ '_add',
//	            iconCls:'icon-add', 
//	            text: HRMSRes.Public_Toolbar_Add, 
//	            hidden:praddrstConfig.auth[this.tabId+'_' + 'add']!='True',
//	            handler: function(){
//	                new praddrstEditWindow(this.grid,true).show();            	
//	            }, 
//	            scope: this 
//	        },'-',
	        
	        { 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden: praddrstConfig.auth[this.tabId + '_' + 'edit'] != 'True',
	            disabled:true,
	            handler: function(){
	            	new praddrstEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },
//	        { 
//	            id:this.tabId+ '_delete',
//                iconCls:'icon-remove', 
//                hidden:praddrstConfig.auth[this.tabId+'_' + 'delete']!='True',
//                text:HRMSRes.Public_Toolbar_Delete, 
//                handler: this.remove, 
//                scope: this 
//            },
            
            '-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:praddrstConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new praddrstQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:praddrstConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:praddrstConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,praddrstConfig.muf?'delete':'add',praddrstConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(praddrstPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var praddrstStoreType=Ext.data.Record.create([
            {name:'emno'},{name:'sfid'},{name:'stfn'},{name:'ptcd'},{name:'cccd'},
            {name:'pscd'},{name:'plcd'},{name:'jccd'},{name:'stcd'},{name:'dvcd'},
            {name:'sccd'},{name:'grcd'},{name:'dpcd'},{name:'ttcd'},{name:'bscd'},
            {name:'jtcd'},{name:'wscd'},{name:'wgcd'},{name:'glcd'},{name:'clcd'},
            {name:'trcd'},{name:'rscd'},{name:'jidt'},{name:'prdt'},{name:'tmdt'},
            {name:'emst'},{name:'titl'},{name:'rsid'},{name:'assc'},{name:'rtdt'},
            {name:'ssdt'},{name:'reto'},{name:'isrh'},{name:'isbl'},{name:'tmrm'},
            {name:'remk'},{name:'lmtm'},{name:'lmur'},{name:'elnm'},{name:'scnm'},
            {name:'bsnm'},{name:'psnm'},{name:'stnm'},{name:'jcnm'},{name:'jtnm'},
            {name:'dvnm'},{name:'dpnm'},{name:'rsnm'},{name:'arcd'},{name:'arnm'},
            {name:'ptnm'},{name:'wsnm'},{name:'grnm'},{name:'wgnm'},{name:'ccnm'},
            {name:'conm'},{name:'trnm'},{name:'frnm'},{name:'elcd'},{name:'ttnm'},
            {name:'salu'},{name:'napl'},{name:'ennm'},{name:'cdcd'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},praddrstStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:praddrstKeyColumn,ColumnValue:praddrstConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/psemplym.mvc/list',
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
            frame:false,
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
                {header:HRMSRes.Personal_Label_emno,sortable: true, dataIndex: 'emno',hidden:false,renderer:this.empStateRender},
                {header:HRMSRes.Public_Label_sfid,sortable: true, dataIndex: 'sfid',type:'date',renderer:this.empStateRender},
                {header:HRMSRes.Public_Label_sftn,sortable: true, dataIndex: 'stfn'},
                {header:HRMSRes.Personal_Label_sftm,sortable: true, dataIndex: 'stnm'},
                {header:HRMSRes.Personal_Label_emst,sortable: true, dataIndex: 'emst',renderer:this.empStateRender},
                {header:HRMSRes.Master_Label_dpnm,sortable: true, dataIndex: 'dpnm'},
                {header:HRMSRes.Master_Label_grnm,sortable: true, dataIndex: 'grnm'},
                {header:HRMSRes.Master_Label_psnm,sortable: true, dataIndex: 'psnm'},
                {header:HRMSRes.Master_Label_jcnm,sortable: true, dataIndex: 'jcnm'},
                {header:HRMSRes.Master_Label_rsnm,sortable: true, dataIndex: 'rsnm'},
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
        
		var keyparams=[];
        keyparams[0]={ColumnName:'rscd',ColumnValue:record.get('rscd')};
        
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ praddrstPageName + '.mvc/delete',
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
        //Ext.getCmp(id+ '_delete').setDisabled(enabled);
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
	    var params={record:Ext.encode({params:[{ColumnName:praddrstKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/psemplym.mvc/exportexcel';
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
    new praddrstPanel();
})

    </script>

</body>
</html>
