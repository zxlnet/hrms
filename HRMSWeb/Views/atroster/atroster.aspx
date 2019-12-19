﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="atroster.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.atroster.atroster" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var atrosterConfig=Ext.decode('<%=ViewData["config"] %>'); 
atrosterConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var atrosterPageName = 'atroster';
var atrosterKeyColumn='rscd';
            
var atrosterQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	atrosterQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(atrosterQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Attendance_Label_rscd,
                        name: 'rscd',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Attendance_Label_rsnm,
                        name: 'rsnm',stateful:false,anchor:'95%'}]},

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


var atrosterEditWindow=function(grid,isNew){
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
	                    },'-',{ 
        	                id:this.tabId+'_autodtl',
                            iconCls:'icon-query', 
                            text:'Auto',
                            //hidden:roleConfig.auth[this.tabId+'_rolemgt_query']!='True',
                            handler: function(){
            	                new atrosterAutoWindow(this.dtlgrid,false).show();
                            }, 
                            scope: this
                        },'-',{ 
        	                id:this.tabId+'_exportexceldtl',
                            iconCls:'icon-export', 
                            text:HRMSRes.Public_Toolbar_ToExcel, 
                            //hidden:roleConfig.auth[this.tabId+'_rolemgt_exportexcel']!='True',
                            handler: this.exportExcelLine, 
                            scope: this 
                        }],
		            items:this.dtlgrid
	            }]
            };
                       
	atrosterEditWindow.superclass.constructor.call(this,{
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
		        var keyField = this.basisForm.findField(atrosterKeyColumn);
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

                setLastModifiedInfo(atrosterConfig,this.basisForm);
                
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

Ext.extend(atrosterEditWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Attendance_Label_rscd + '(<font color=red>*</font>)',
                        name: 'rscd',allowBlank:false,stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Attendance_Label_rsnm + '(<font color=red>*</font>)',
                        name: 'rsnm',allowBlank:false,stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Attendance_Label_rtty  + '(<font color=red>*</font>)',
                        name: 'rtty',stateful:false,editable:false,typeAhead: true,value:'B',
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: RosterRestTypeStore}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_efdt + '(<font color=red>*</font>)',id:'efdt',
                        name:'efdt',editable:false,height:22,anchor:'95%',allowBlank:false,
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Public_Label_exdt,id:'exdt',
                        name:'exdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

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
        var isrtCheckColumn = new Ext.grid.CheckColumn({
               header: "Is Rest Day",
               dataIndex: 'isrt',
               width: 55
            });

		var atrosterDtlStoreType=Ext.data.Record.create([
            {name:'rscd'},
            {name:'dasq'},
            {name:'isrt'},
            {name:'sfcd'},
            {name:'sfnm'},
            {name:'otcd'},
            {name:'otnm'},
            {name:'lmtm'},
            {name:'lmur'}
		]);
        
		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},atrosterDtlStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:atrosterKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + atrosterPageName + '.mvc/getRosterDtl',
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
        
        //store.load({params:params});
        
        return new Ext.grid.EditorGridPanel({
            id:'rosterDtlGrid',
    		border:true, 
    		monitorResize:false, 
            height:285,
            loadMask:true,  		            
            ds: store, 
            frame:false,
            collapsible: true,
            animCollapse: false,
            editable:true,
            clicksToEdit:1,
            plugins:isrtCheckColumn,
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
            {header:'dasq',sortable: true, dataIndex: 'dasq',width: 50},
            
            {header: HRMSRes.Attendance_Label_sfnm,dataIndex: 'sfnm',width: 130,
                editor: new Ext.form.ComboBox({typeAhead: true,triggerAction: 'all',mode:'local',
                listClass: 'x-combo-list-small',
                displayField: 'DisplayField',valueField:'DisplayField',
                store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getShift'}),
		        listeners:{
		            select:function(c,r,i){var sm = this.dtlgrid.getSelectionModel();var rec= sm.getSelected();rec.set('sfcd',r.data['ValueField']);},scope:this}
		     })},
		     
             {header: HRMSRes.Attendance_Label_otnm,dataIndex: 'otnm',width: 130,
                editor: new Ext.form.ComboBox({typeAhead: true,triggerAction: 'all',mode:'local',
                listClass: 'x-combo-list-small',
                displayField: 'DisplayField',valueField:'DisplayField',
                store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getOverTimeType'}),
		        listeners:{select:function(c,r,i){var sm = this.dtlgrid.getSelectionModel();var rec= sm.getSelected();rec.set('otcd',r.data['ValueField']);},scope:this}
		     })},

            isrtCheckColumn
            //{header:HRMSRes.Public_Label_lmtm,sortable: true, dataIndex: 'lmtm',renderer:formatDate},
            //{header:HRMSRes.Public_Label_lmur,sortable: true, dataIndex: 'lmur'},
            //{header:HRMSRes.Attendance_Label_sfcd,sortable: true, dataIndex: 'sfcd'},
            //{header:HRMSRes.Attendance_Label_otcd,sortable: true, dataIndex: 'otcd'}
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
                     dasq:x.get('dasq'),sfcd:x.get('sfcd'),
                     otcd:x.get('otcd'),isrt:x.get('isrt'),
                     lmtm:x.get('lmtm'),lmur:x.get('lmur')};
            
            dtlparams[dtlparams.length] = p;
        }
        
        var keyparams=[];
        keyparams[0]={ColumnName:'rscd',ColumnValue:this.basisForm.findField('rscd').getValue()};
        //keyparams[1]={ColumnName:'dasq',ColumnValue:this.basisForm.findField('dasq').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + atrosterPageName + '.mvc/'+method,
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
        var f = this.basisForm.findField(atrosterKeyColumn);        
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
        params['dasq']= n.toString();
        params['sfnm']='';
        params['sfcd']='';
        params['otnm']='';
        params['otcd']='';
        params['isrt']='N';
        params['lmtm']= formatDateTime(new Date());
        params['lmur']=atrosterConfig.currentUser;

        var store = this.dtlgrid.getStore();
        store.add(new store.recordType(params));
        //this.dtlgrid.getStore().insert();
        //uuGrid.view.refresh();
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
	    var params={record:Ext.encode({params:[{ColumnName:atrosterKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.dtlgrid.queryParams){
            this.dtlgrid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.dtlgrid.queryParams);
            delete this.dtlgrid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + atrosterPageName + '.mvc/exportexceldtl';
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
        //Ext.getCmp(id+ '_editdtl').setDisabled(enabled);
        Ext.getCmp(id+ '_deletedtl').setDisabled(enabled);
    },
    resortdasq:function(){
        var st = this.dtlgrid.getStore();
        for (var i =1; i<= st.getCount();i++ ){
            st.getAt(i-1).set('dasq',i.toString());
        }
    }    
});


var atrosterPanel=function(){
    this.tabId=atrosterConfig.tabId;
	this.init();	
	
	atrosterPanel.superclass.constructor.call(this,{
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
	            hidden:atrosterConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new atrosterEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden:atrosterConfig.auth[this.tabId+'_' + 'edit']!='True',
	            handler: function(){
	            	new atrosterEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:atrosterConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:atrosterConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new atrosterQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:atrosterConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:atrosterConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,atrosterConfig.muf?'delete':'add',atrosterConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(atrosterPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var atrosterStoreType=Ext.data.Record.create([
            {name:'rscd'},
            {name:'rsnm'},
            {name:'rtty'},
            {name:'efdt'},
            {name:'exdt'},
            {name:'lmtm'},
            {name:'lmur'},
            {name:'rfid'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},atrosterStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:atrosterKeyColumn,ColumnValue:atrosterConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + atrosterPageName + '.mvc/list',
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
                {header:HRMSRes.Attendance_Label_rscd,sortable: true, dataIndex: 'rscd',hidden:false},
                {header:HRMSRes.Attendance_Label_rsnm,sortable: true, dataIndex: 'rsnm',hidden:false},
                {header:HRMSRes.Attendance_Label_rtty,sortable: true, dataIndex: 'rtty',hidden:false},
                {header:HRMSRes.Public_Label_efdt,sortable: true, dataIndex: 'efdt',hidden:false,renderer:formatDateNoTime},
                {header:HRMSRes.Public_Label_exdt,sortable: true, dataIndex: 'exdt',hidden:false,renderer:formatDateNoTime},
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
        keyparams[0]={ColumnName:'clcd',ColumnValue:record.get('clcd')};
        
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ atrosterPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:atrosterKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + atrosterPageName + '.mvc/exportexcel';
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
    new atrosterPanel();
})

    </script>

</body>
</html>