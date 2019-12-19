<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="atanaatt.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.atanaatt.atanaatt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var atanaattConfig=Ext.decode('<%=ViewData["config"] %>'); 
atanaattConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var atanaattPageName = 'atanaatt';
var atanaattKeyColumn='emno';
            
var atanaattQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	atanaattQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(atanaattQueryWindow,Ext.Window,{
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
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Attendance_Label_atdt + '' + HRMSRes.Public_Label_From,id:'from|atdt',
                        name:'from|atdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Attendance_Label_atdt + '' + HRMSRes.Public_Label_To,id:'to|atdt',
                        name:'to|atdt',editable:false,height:22,anchor:'95%',
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

var atanalattATDateWindow=function(){
	this.init();	
	
	atanalattATDateWindow.superclass.constructor.call(this,{
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
                if (atanalatrange==null) return;
            
                var f1 = this.form.findField('from|atdt');
                f1.setValue(atanalatrange[0].ColumnValue);
                
                var f2 = this.form.findField('to|atdt');
                f2.setValue(atanalatrange[1].ColumnValue);
            }
        },
        buttons: [{ 
            text:HRMSRes.Public_Toolbar_Confirm, 
            iconCls:'icon-query', 
            handler: this.Confirm,
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

Ext.extend(atanalattATDateWindow,Ext.Window,{
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
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Attendance_Label_atdt + '' + HRMSRes.Public_Label_From,id:'from|atdt',
                        name:'from|atdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Attendance_Label_atdt + '' + HRMSRes.Public_Label_To,id:'to|atdt',
                        name:'to|atdt',editable:false,height:22,anchor:'95%',
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]}

            ] 
       })
	},	
	
	Confirm:function(){	    
	    var params=[];	        

        params[params.length]={ColumnName:'from|atdt',ColumnValue:this.form.findField('from|atdt').getValue()};                  
        params[params.length]={ColumnName:'to|atdt',ColumnValue:this.form.findField('to|atdt').getValue()};                  

        if ((params[0]=='') || (params[1]=='')) {
   		    Ext.MessageBox.show({
	            title: HRMSRes.Public_Message_Error,
	            msg:HRMSRes.Public_Message_NoAnalDate,
	            buttons: Ext.MessageBox.OK,
	            icon:Ext.MessageBox.ERROR
            });
        }
            
        atanalatrange=params;
		this.close();
	},
	cleanQuery:function(){
	    cleanQueryCriterias(this.form);
	},
	scope:this
});

var atanalattStatusWindow=function(grid){
    this.grid = grid;
	this.init();	
	
	atanalattStatusWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:400, 
        height:150, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.formPanel,
        title:HRMSRes.Public_Processing_WindowTitle,
        listeners:{
            show:function(){
            	this.Analyze();
            }
        },
        scope:this    
	});
}

Ext.extend(atanalattStatusWindow,Ext.Window,{
    init:function(){
		this.formPanel=this.createFormPanel();
		this.form=this.formPanel.getForm();
	},
	
	createFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         html:'<div class=\"loading-indicator\"><img src=\"/ExtJS/ext/resources/images/default/grid/loading.gif" width=\"16\" height=\"16\"  style=\"margin-right: 8px;\" /><font color=\'red\'>' + HRMSRes.Public_Message_Analyzing + '</font></div>',
	         items:[{text: HRMSRes.Public_Message_CLR}]
       })
	},
	Analyze:function(){
		this.grid.getBottomToolbar().diplayMsg.update('');       
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + atanaattPageName + '.mvc/analyzeAttendance',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		   		

		   		this.grid.getBottomToolbar().diplayMsg.update(o.msg);	   		

		   		if (o.status=='success'){
		   		    this.close();
		   		    this.query("All");
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
		   params: {record:Ext.encode({atdateparams:atanalatrange,personalparams:empAdvQryResult,includelv:analLeave,includeot:analOverTime})}
		});	    
	},
	query:function(scope){	    
	    this.grid.getBottomToolbar().diplayMsg.update('');
	    var params=[];	        
        var loadParams={start:0,limit:Pagination.pagingSize};
        this.grid.store.baseParams={record:Ext.encode({scope:scope,atdateparams:atanalatrange,personalparams:empAdvQryResult})};
		this.grid.store.load({params:loadParams});
	},
	scope:this
});

var atanaattPanel=function(){
    this.tabId=atanaattConfig.tabId;
	this.init();	
	
    var queryMenu = new Ext.menu.Menu({
    id: 'queryMenu',
    items: [
        {
            text: HRMSRes.Public_Message_CLR,
            handler: function() {
                if (!this.grid.getSelectionModel().hasSelection())
                {   
                    this.grid.getBottomToolbar().diplayMsg.update("You should select a row first.");
                    return;
                }
                var emno = this.grid.getSelectionModel().getSelected().get('emno');
                var atdt = this.grid.getSelectionModel().getSelected().get('atdt')
                mainPanel.loadClass(ContextInfo.contextPath+'/lvleaapp.mvc/index?menuId=M4070&emno='+emno+"&atdt="+atdt,'M4070','','Leave applications','','true');
            },
            scope:this
        },'-',
        {
            text: HRMSRes.Public_Message_COTR,
            handler: function() {
                if (!this.grid.getSelectionModel().hasSelection()) 
                {   
                    this.grid.getBottomToolbar().diplayMsg.update("You should select a row first.");
                    return;
                }
                var emno = this.grid.getSelectionModel().getSelected().get('emno');
                var atdt = this.grid.getSelectionModel().getSelected().get('atdt')
                mainPanel.loadClass(ContextInfo.contextPath+'/otdetail.mvc/index?menuId=M5060&emno='+emno+"&atdt="+atdt,'M5060','','Overtime Details','','true');
            },
            scope:this           
        },'-',
        {
            text: HRMSRes.Public_Message_CRH,
            handler: function() {
                if (!this.grid.getSelectionModel().hasSelection())
                {   
                    this.grid.getBottomToolbar().diplayMsg.update("You should select a row first.");
                    return;
                }
                var emno = this.grid.getSelectionModel().getSelected().get('emno');
                mainPanel.loadClass(ContextInfo.contextPath+'/atroshis.mvc/index?menuId=M3180&emno='+emno,'M3180','',HRMSRes.Public_Menu_atroshis,'','true');
            },
            scope:this            
        },'-',
        {
            text: HRMSRes.Public_Message_CMAD,
            handler: function() {
                if (!this.grid.getSelectionModel().hasSelection()) 
                {   
                    this.grid.getBottomToolbar().diplayMsg.update("You should select a row first.");
                    return;
                }
                var emno = this.grid.getSelectionModel().getSelected().get('emno');
                var atdt = this.grid.getSelectionModel().getSelected().get('atdt')
                mainPanel.loadClass(ContextInfo.contextPath+'/atdatmnu.mvc/index?menuId=M3120&emno='+emno + "&atdt=" + atdt,'M3120','',HRMSRes.Public_Menu_atdatmnu,'','true');
            },
            scope:this 
        },'-',
        {
            text: HRMSRes.Public_Message_COSD,
            handler: function() {
                if (!this.grid.getSelectionModel().hasSelection()) 
                {   
                    this.grid.getBottomToolbar().diplayMsg.update("You should select a row first.");
                    return;
                }
                var emno = this.grid.getSelectionModel().getSelected().get('emno');
                var atdt = this.grid.getSelectionModel().getSelected().get('atdt')
                mainPanel.loadClass(ContextInfo.contextPath+'/atoridat.mvc/index?menuId=M3130&emno='+emno + "&atdt=" + atdt,'M3130','',HRMSRes.Public_Menu_atoridat,'','true');
            },
            scope:this 
        }
        ]
    });
    
    var abnormalActionMenu = new Ext.menu.Menu({
    id: 'abnormalActionMenu',
    items: [
        {
            text: HRMSRes.Public_Message_TATL
        }
        ]
    });
        
    var analyzeScopeMenu = new Ext.menu.Menu({
    id: 'analyzeScopeMenu',
    items: [
        {
            text: HRMSRes.Public_Message_ADR,
            handler: function() {new atanalattATDateWindow().show();}
        },'-',
        {
            text: HRMSRes.Public_Message_EF,
            handler: function() {new empAdvQryQueryWindow(this.grid).show();}
        },'-',
        {
            text: HRMSRes.Public_Message_ALT,
            checked: true,
            checkHandler: function(item, checked){
                        analLeave=checked?'Y':'N';
                }
        },'-',
        {
            text: HRMSRes.Public_Message_AOT,
            checked: true,
            checkHandler: function(item, checked){
                        analOverTime=checked?'Y':'N';
                }
        }        
        ]
    });
    
    var filterMenu = new Ext.menu.Menu({
    id: 'filterMenu',
    items: [
        {
            text: HRMSRes.Public_Message_AR,
            handler: function() {this.query('All');},
            scope: this
        },'-',
        {
            text: HRMSRes.Public_Message_ABR,
            handler: function() {this.query('Abnormal');},
            scope: this
        }
        
        ]
    });
    
	atanaattPanel.superclass.constructor.call(this,{
		applyTo:this.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	autoScroll:true,      
	        tbar: [{ 
                text:HRMSRes.Public_Message_AS,
                iconCls: 'icon-query',  
                menu: analyzeScopeMenu  
	        },'-',{ 
	        	id:this.tabId+ '_analyze',
	            iconCls:'icon-add', 
	            text: HRMSRes.Public_Message_DA, 
	            //hidden:atanaattConfig.auth[this.tabId+'_' + 'analyze']!='True',
	            handler: function(){
	                new atanalattStatusWindow(this.grid).show();                          	
	            }, 
	            scope: this 
	        },'-',{ 
                text:HRMSRes.Public_Message_CA,
                //hidden:atanaattConfig.auth[this.tabId+'_' + 'delete']!='True',
                iconCls: 'icon-query',  
                menu: filterMenu  
            },'-',{ 
                text:HRMSRes.Public_Message_HA,
                //hidden:atanaattConfig.auth[this.tabId+'_' + 'delete']!='True',
                iconCls: 'icon-query',  
                menu: abnormalActionMenu  
            },'-',{ 
                text:HRMSRes.Public_Message_RI,
                //hidden:atanaattConfig.auth[this.tabId+'_' + 'delete']!='True',
                iconCls: 'icon-query',  
                menu: queryMenu  
            },'-',{ 
	            id:this.tabId+ '_save',
                iconCls:'icon-save', 
                //hidden:atanaattConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Save, 
                handler: this.save, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            //hidden:atanaattConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
	            handler: this.exportExcel, 
	            scope: this 
	        },'->',{ 
                id:this.tabId+'_muf',
                iconCls:atanaattConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,atanaattConfig.muf?'delete':'add',atanaattConfig,this.grid);},
                scope: this 
            }],  
	        items:this.grid
       }]
	})
}

Ext.extend(atanaattPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},
    
//    function rendererByatst(val,m,r)
//    {
//        if (r.get("atst")=="0")
//        {
//            return '<span style=\'color:green\'>'+val+'</span>';
//        }
//        else if (r.get("period_status")=="1")
//        {
//            return '<span style=\'color:red\'>'+val+'</span>';
//        }
//    },
//    
	createGridPanel:function(){

		var atanaattStoreType=Ext.data.Record.create([
		    {name:'emno'},
            {name:'sfid'},
            {name:'stfn'},
            {name:'dpcd'},
            {name:'dpnm'},
            {name:'atdt'},
            {name:'atst'},
            {name:'athr'},
            {name:'ahrs'},
            {name:'ahrr'},
            {name:'ahrm'},
            {name:'lact'},
            {name:'lctm'},
            {name:'eact'},
            {name:'ectm'},
            {name:'abda'},
            {name:'adam'},
            {name:'ehrm'},
            {name:'rsnm'},
            {name:'stfn'},
            {name:'rscd'},
            {name:'sfcd'},
            {name:'intm'},
            {name:'brst'},
            {name:'bret'},
            {name:'ottm'},
            {name:'remk'},
            {name:'iscf'},
            {name:'sfnm'},
            {name:'eahr'},
            {name:'lahr'},
            {name:'lvhr'},
            {name:'othr'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},atanaattStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:atanaattKeyColumn,ColumnValue:atanaattConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + atanaattPageName + '.mvc/listAnalResult',
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
                        p.getAt(i).set('atdt',formatDateNoTime(p.getAt(i).get('atdt')));   
                        p.getAt(i).set('intm',formatTime(p.getAt(i).get('intm')));   
                        p.getAt(i).set('brst',formatTime(p.getAt(i).get('brst')));   
                        p.getAt(i).set('bret',formatTime(p.getAt(i).get('bret')));   
                        p.getAt(i).set('ottm',formatTime(p.getAt(i).get('ottm')));   
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
        
        return new Ext.grid.EditorGridPanel({
    		border:true, 
    		monitorResize:false, 
            loadMask:true,  		            
            frame:false,
            collapsible: true,
            animCollapse: false,
            editable:true,
            clicksToEdit:1,
            ds: store, 
            viewConfig: { 
		        //autoFill: true 
		    }, 
		    sm: new Ext.grid.RowSelectionModel({singleSelect:true}),
            listeners:{
	            rowclick:function(){
	                this.controlButton(this.tabId);
	            },
	            scope:this
            },                      
            cm: new Ext.grid.ColumnModel([ 
                {id:"iscf",header: HRMSRes.Attendance_Label_iscf,dataIndex: 'iscf',width: 80,
                    editor: new Ext.form.Checkbox({}),renderer:formatCheckBox},

                {header:HRMSRes.Master_Label_dpnm,sortable: true, dataIndex: 'dpnm'},
                {header:HRMSRes.Public_Label_sfid,sortable: true, dataIndex: 'sfid'},
                {header:HRMSRes.Public_Label_sftn,sortable: true, dataIndex: 'stfn'},
                {header:HRMSRes.Attendance_Label_atdt,sortable: true, dataIndex: 'atdt',renderer:formatDateNoTime},               
                {header:HRMSRes.Attendance_Label_athr,sortable: true, dataIndex: 'athr'},
                
                {id:'intm',header: HRMSRes.Attendance_Label_intm,dataIndex: 'intm',renderer:formatTimeOnly,
                    editor: new Ext.form.TimeField({format:DATE_FORMAT.TIMEONLY})},

                {id:'brst',header: HRMSRes.Attendance_Label_brst,dataIndex: 'brst',renderer:formatTimeOnly,
                    editor: new Ext.form.TimeField({format:DATE_FORMAT.TIMEONLY})},

                {id:'bret',header: HRMSRes.Attendance_Label_bret,dataIndex: 'bret',renderer:formatTimeOnly,
                    editor: new Ext.form.TimeField({format:DATE_FORMAT.TIMEONLY})},

                {id:'ottm',header: HRMSRes.Attendance_Label_ottm,dataIndex: 'ottm',renderer:formatTimeOnly,
                    editor: new Ext.form.TimeField({format:DATE_FORMAT.TIMEONLY})},

                {header:HRMSRes.Attendance_Label_abhr,sortable: true, dataIndex: 'ahrs'},
                {header:HRMSRes.Attendance_Label_abda,sortable: true, dataIndex: 'abda'},
                //{header:HRMSRes.Attendance_Label_ahrr,sortable: true, dataIndex: 'ahrr'},
                {header:HRMSRes.Attendance_Label_eahr,sortable: true, dataIndex: 'eahr'},
                {header:HRMSRes.Attendance_Label_lahr,sortable: true, dataIndex: 'lahr'},
                //{header:HRMSRes.Attendance_Label_ahrm,sortable: true, dataIndex: 'ahrm'},
                {header:HRMSRes.Attendance_Label_lact,sortable: true, dataIndex: 'lact'},
                //{header:HRMSRes.Attendance_Label_lctm,sortable: true, dataIndex: 'lctm'},
                {header:HRMSRes.Attendance_Label_eact,sortable: true, dataIndex: 'eact'},
                { header: HRMSRes.Attendance_Label_lvhr, sortable: true, dataIndex: 'lvhr' },
                { header: HRMSRes.Attendance_Label_othr, sortable: true, dataIndex: 'othr' },
                //{header:HRMSRes.Attendance_Label_ectm,sortable: true, dataIndex: 'ectm'},
                //{header:HRMSRes.Attendance_Label_adam,sortable: true, dataIndex: 'adam'},
                //{header:HRMSRes.Attendance_Label_ehrm,sortable: true, dataIndex: 'ehrm'},
                {header:HRMSRes.Attendance_Label_rsnm,sortable: true, dataIndex: 'rsnm'},
                {header:HRMSRes.Attendance_Label_sfnm,sortable: true, dataIndex: 'sfnm'},
                {id:'remk',header: HRMSRes.Public_Label_remk,dataIndex: 'remk',width: 100,renderer:formatTimeOnly,
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
	
	query:function(scope){	  
	    this.queryscope = scope;  
	    this.grid.getBottomToolbar().diplayMsg.update('');
	    var params=[];	        
        var loadParams={start:0,limit:Pagination.pagingSize};
        this.grid.store.baseParams={record:Ext.encode({scope:scope,atdateparams:atanalatrange,personalparams:empAdvQryResult})};
		this.grid.store.load({params:loadParams});
	},
	save:function()
	{
	    var sm=this.grid.store.getModifiedRecords();
        var params=[];
        var paramsIscf=[];
        
	    if (sm.length<1)
	    {
            Ext.MessageBox.show({
               title:'Warning',
               msg: HRMSRes.Public_Message_NC,
               buttons: Ext.MessageBox.OK,
               icon: Ext.MessageBox.OK
            });	    
        }
        else
        {
            for (var i=0;i<sm.length;i++)
            {   
                alert(sm[i].modified['intm']);
                 if (sm[i].modified['intm']!=undefined ||
                     sm[i].modified['ottm']!=undefined ||
                     sm[i].modified['brst']!=undefined ||
                     sm[i].modified['bret']!=undefined)
                 {  
                     var p={
                              emno:sm[i].get('emno'),
                              atdt:sm[i].get('atdt'),
                              ittm:sm[i].get('atdt') +' '+sm[i].get('intm'),
                              ottm:sm[i].get('atdt') +' '+sm[i].get('ottm'),
                              brst:sm[i].get('brst')==null?sm[i].get('brst'):sm[i].get('atdt') +' '+sm[i].get('brst'),
                              bstm:sm[i].get('bret')==null?sm[i].get('bret'):sm[i].get('atdt') +' '+sm[i].get('bret'),
                              scdm:sm[i].get('sfcd'),
                              adam:sm[i].get('adam')=="0"?"0.0":sm[i].get('adam') * 1.0,
                              ahrm:sm[i].get('ahrm')=="0"?"0.0":sm[i].get('ahrm') * 1.0,
                              lctm:sm[i].get('lctm')=="0"?"0.0":sm[i].get('lctm') * 1.0,
                              ectm:sm[i].get('ectm')=="0"?"0.0":sm[i].get('ectm') * 1.0,
                              remk:sm[i].get('remk'),
                              lmtm:new Date(),
                              lmur:atanaattConfig.currentUser
                    };
                    params[params.length]=p;
                }
                
                if (sm[i].modified['iscf']!=undefined)
                {
                    var p= [{ColumnName:"emno",ColumnValue:sm[i].get('emno')},
                            {ColumnName:"atdt",ColumnValue:sm[i].get('atdt'),ColumnType:"datetime"},
                            {ColumnName:"iscf",ColumnValue:sm[i].get('iscf')}]

                    paramsIscf[paramsIscf.length]=p;
                }
            }	
	    
            Ext.Ajax.request({
	            url:ContextInfo.contextPath+ '/'+ atanaattPageName + '.mvc/save',
	            success: function(response){
		            var o= Ext.util.JSON.decode(response.responseText);
		            if(o.status=='success'){
		                //this.grid.store.remove(record);
	                    //this.grid.store.totalLength-=1;
	                    this.grid.store.commitChanges();
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
	            params:{record:Ext.encode({params:params,paramsIscf:paramsIscf})}
            })	    
        }
	
	},
		
	controlButton:function(id){
        var enabled=!this.grid.getSelectionModel().hasSelection();	    
        //Ext.getCmp(id+ '_edit').setDisabled(enabled);
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
    
	    var params={record:Ext.encode({scope:this.queryscope,atdateparams:atanalatrange,personalparams:empAdvQryResult,headers:header})};
	    
//	    if(this.grid.queryParams){
//            this.grid.queryParams['headers']=header;
//            delete params.record;
//            params.record=Ext.encode(this.grid.queryParams);
//            delete this.grid.queryParams.header;
//        }
//	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + atanaattPageName + '.mvc/exportexcel';
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
    new atanaattPanel();

    var params=[];	        
    var dd = formatDateNoTime(new Date());
    
    var p1={
        ColumnName:'from|atdt',
        ColumnValue:dd
    };
    
    var p2={
        ColumnName:'to|atdt',
        ColumnValue:dd                
    };
    
    params[0] = p1;
    params[1] = p2;
    this.atanalatrange=params;

    this.empAdvQryResult = [];
    
    this.analLeave='Y';
    this.analOverTime='Y';
})

    </script>

</body>
</html>