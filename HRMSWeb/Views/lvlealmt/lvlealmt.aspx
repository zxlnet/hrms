<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="lvlealmt.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.lvlealmt.lvlealmt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var lvlealmtConfig=Ext.decode('<%=ViewData["config"] %>'); 
lvlealmtConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var lvlealmtPageName = 'lvlealmt';
var lvlealmtKeyColumn='lmno';
var dtlgrdAction = 'query';
            
var lvlealmtQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	lvlealmtQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(lvlealmtQueryWindow,Ext.Window,{
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
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leave_Label_ltcd ,
                        name: 'ltcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getLeaveType'})
    		          }]}
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


var lvlealmtEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	

           
	lvlealmtEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:300, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.basisFormPanel,
        listeners:{
            show:function(){    
		        var keyField = this.basisForm.findField(lvlealmtKeyColumn);
                if(!this.isNew){	
			        var data=this.grid.getSelectionModel().getSelected();
                    this.basisForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            f.setValue(value);                    	            
                        }
                    });   
		        }
		        else
		        {
		            getMaxNo(lvlealmtConfig.tableName,'lmno',this.basisForm,keyField);
		        }
                setLastModifiedInfo(lvlealmtConfig,this.basisForm);
                
                //if (!this.isNew)
                //    this.query();
             },
            scope:this
        },
        buttons: [{ 
            text:HRMSRes.Public_Button_ApplyTo, 
            iconCls:'icon-applyto', 
            id:this.tabId+ '_applyto',
            disabled:true,
            handler: function(){
		            var params={};
		            this.basisForm.items.each(function(f){
                        if(f.isFormField){
                            params[f.getName()]=f.getValue();
                        }
                    });
                    
                    var url = ContextInfo.contextPath+'/' + lvlealmtPageName + '.mvc/applyto';
                    new empAdvQryQueryWindow(this.grid,'1',url,params).show();
                },            
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

Ext.extend(lvlealmtEditWindow,Ext.Window,{
    init:function(){
		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();
		//this.dtlgrid = this.createGridPanel();
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Leave_Label_lmno,disabled:true,
                        name: 'lmno',stateful:false,anchor:'95%'}]},
     		        
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leave_Label_ltnm + '(<font color=red>*</font>)',
                        name: 'ltcd',stateful:false,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getLeaveType',
		                    listeners:{
                                    load: function()
                                    {
                                        var v = this.basisForm.findField('ltcd').getValue();
                                        this.basisForm.findField('ltcd').setValue(v);  		                            
                                    },
                                    scope:this
		                        }})}]},
                                        
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leave_Label_lmby + '(<font color=red>*</font>)',
                        name: 'lmby',stateful:false,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',lazyRender:false,
                        displayField: 'DisplayField',valueField:'ValueField', 
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField','MiscField1']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getLvdffrLimit',
		                    listeners:{
                                    load: function(t,r)
                                    {
                                        var f = this.basisForm.findField('lmby');
                                        var v = f.getValue();
                                        f.setValue(v);  	
                                        
                                        var selIndex = -1;	 
                                        for (var i=0;i<t.getCount();i++){
                                        	 if (t.getAt(i).get('ValueField')==v){
                                        	    selIndex = i; 
                                        	    break;
                                        	 }
                                        }
                                        
                                        f.fireEvent('select',f,t.getAt(selIndex));
                                    },
                                    scope:this
		                        }}),
                            listeners:{
                                    select:function(p,r)
                                    {
                                        if (r==null) return;
                                        var f = this.basisForm.findField('lmva');
                                        f.store.baseParams={record:Ext.encode(r.get('MiscField1'))};
                                        f.store.load();
                                    },
                                    scope:this
                               }
                      }]},
                      
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leave_Label_lmva ,
                        name: 'lmva',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store:new Ext.data.Store({ 
                            reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
                            fields: ['ValueField','DisplayField']}), 
                            url:ContextInfo.contextPath + '/dropdown.mvc/executeAction',
                            listeners:{
                                    load: function(t,r)
                                    {
                                        var v = this.basisForm.findField('lmva').getValue();
                                        this.basisForm.findField('lmva').setValue(v);  	
                                    },
                                    scope:this
                            }})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leave_Label_lmsc ,
                        name: 'lmsp',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: LimitStore 
    		          }]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Leave_Label_mxlh,disabled:false,
                        name: 'mxlh',allowBlank:false,stateful:false,anchor:'95%',decimalPrecision:1,keepZero:true}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Leave_Label_mxch,disabled:false,
                        name: 'mxch',allowBlank:false,stateful:false,anchor:'95%',decimalPrecision:1,keepZero:true}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Public_Label_remk,height:70,
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
	

	save: function(){
		if(!this.basisForm.isValid()) return;
		this.grid.getBottomToolbar().diplayMsg.update('');
		var params={};
		this.basisForm.items.each(function(f){
            if(f.isFormField){
                params[f.getName()]=f.getValue();
            }
        });
        params['lmtx']=this.basisForm.findField('lmva').getRawValue();
 
        var keyparams=[];
        keyparams[0]={ColumnName:'lmno',ColumnValue:this.basisForm.findField('lmno').getValue(),ColumnType:'int'};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + lvlealmtPageName + '.mvc/'+method,
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
		   		    //this.close();
                    if (this.basisForm.findField('lmby').getValue()=='E')
                        Ext.getCmp(this.tabId+ '_applyto').setDisabled(false);
                    else
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
	},
	controlButton:function(id){
        var enabled=!this.dtlgrid.getSelectionModel().hasSelection();	    
        //Ext.getCmp(id+ '_editdtl').setDisabled(enabled);
        //Ext.getCmp(id+ '_deletedtl').setDisabled(enabled);
    }
});


var lvlealmtPanel=function(){
    this.tabId=lvlealmtConfig.tabId;
	this.init();	
	
	lvlealmtPanel.superclass.constructor.call(this,{
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
	            hidden:lvlealmtConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new lvlealmtEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden:lvlealmtConfig.auth[this.tabId+'_' + 'edit']!='True',
	            handler: function(){
	            	new lvlealmtEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:lvlealmtConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:lvlealmtConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new lvlealmtQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:lvlealmtConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:lvlealmtConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,lvlealmtConfig.muf?'delete':'add',lvlealmtConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(lvlealmtPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var lvlealmtStoreType=Ext.data.Record.create([
            {name:'lmno'},
            {name:'ltcd'},
            {name:'ltnm'},
            {name:'lmby'},
            {name:'lmbytext'},
            {name:'lmva'},
            {name:'lmtx'},
            {name:'lmsp'},
            {name:'mxlh'},
            {name:'mxch'},
            {name:'remk'},
            {name:'lmtm'},
            {name:'lmur'},
            {name:'rfid'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},lvlealmtStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:lvlealmtKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + lvlealmtPageName + '.mvc/list',
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
                {header:HRMSRes.Leave_Label_lmno,sortable: true, dataIndex: 'lmno'},
                {header:HRMSRes.Leave_Label_ltnm,sortable: true, dataIndex: 'ltnm'},
                {header:'Limit By Text',sortable: true, dataIndex: 'lmbytext'},
                {header:HRMSRes.Leave_Label_lmtx,sortable: true, dataIndex: 'lmtx'},
                {header:HRMSRes.Leave_Label_lmsc,sortable: true, dataIndex: 'lmsp'},
                {header:HRMSRes.Leave_Label_mxlh,sortable: true, dataIndex: 'mxlh'},
                {header:HRMSRes.Leave_Label_mxch,sortable: true, dataIndex: 'mxch'},
                {header:HRMSRes.Master_Label_remk,sortable: true, dataIndex: 'remk'},
                {header:HRMSRes.Public_Label_lmtm,sortable: true, dataIndex: 'lmtm',renderer:formatDate},
                {header:HRMSRes.Public_Label_lmur,sortable: true, dataIndex: 'lmur',hidden:false}
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
        keyparams[0]={ColumnName:'lmno',ColumnValue:record.get('lmno'),ColumnType:'int'};
        
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ lvlealmtPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:lvlealmtKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + lvlealmtPageName + '.mvc/exportexcel';
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
    new lvlealmtPanel();
})

    </script>

</body>
</html>