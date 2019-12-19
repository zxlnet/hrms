<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="lvdfbyot.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.lvdfbyot.lvdfbyot" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var lvdfbyotConfig=Ext.decode('<%=ViewData["config"] %>'); 
lvdfbyotConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var lvdfbyotPageName = 'lvdfbyot';
var lvdfbyotKeyColumn='dfno';
var dtlgrdAction = 'query';
            
var lvdfbyotQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	lvdfbyotQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(lvdfbyotQueryWindow,Ext.Window,{
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
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leavel_Label_ltcd,
                        name: 'ltcd',stateful:false,disabled:!this.isNew,allowBlank:false,typeAhead: true,
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


var lvdfbyotEditWindow=function(grid,isNew){
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

           
	lvdfbyotEditWindow.superclass.constructor.call(this,{
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
		        var keyField = this.basisForm.findField(lvdfbyotKeyColumn);
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
		            getMaxNo(lvdfbyotConfig.tableName,'dfno',this.basisForm,keyField);
		        }
		        //var keyValue = keyField.getValue();
                
                setLastModifiedInfo(lvdfbyotConfig,this.basisForm);
                
                //if (!this.isNew)
                    this.query();
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

Ext.extend(lvdfbyotEditWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Leavel_Label_dfno,disabled:true,
                        name: 'dfno',stateful:false,anchor:'95%'}]},
     		        
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_ltcd + '(<font color=red>*</font>)',
                        name: 'ltcd',stateful:false,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getLeaveType',
		                    listeners:{load: function(){var v = this.basisForm.findField('ltcd').getValue();this.basisForm.findField('ltcd').setValue(v);},scope:this
		                        }})}]},
                                        
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leavel_Label_dfby + '(<font color=red>*</font>)',
                        name: 'dfby',stateful:false,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',lazyRender:false,
                        displayField: 'DisplayField',valueField:'ValueField', 
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField','MiscField1']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getLvDef',
		                    listeners:{
                                    load: function(t,r)
                                    {
                                        var v = this.basisForm.findField('dfby').getValue();
                                        this.basisForm.findField('dfby').setValue(v);  	
                                        
                                        var selIndex = -1;	 
                                        for (var i=0;i<t.getCount();i++){
                                        	 if (t.getAt(i).get('ValueField')==v){
                                        	    selIndex = i; 
                                        	    break;
                                        	 }
                                        }
                                        	                            
                                        var f = this.basisForm.findField('dfby');
                                        f.fireEvent('select',f,t.getAt(selIndex));
                                    },
                                    scope:this
		                        }}),
                            listeners:{
                                    select:function(p,r)
                                    {
                                        if (r==null) return;
                                        var f = this.basisForm.findField('dfva');
                                        f.store.baseParams={record:Ext.encode(r.get('MiscField1'))};
                                        f.store.load();
                                    },
                                    scope:this
                               }
                      }]},
                      
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Leavel_Label_dfva ,mode:'local',
                        name: 'dfva',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store:new Ext.data.Store({ 
                            reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
                            fields: ['ValueField','DisplayField']}), autoLoad:true,
                            url:ContextInfo.contextPath + '/dropdown.mvc/executeAction',
                            listeners:{
                                    load: function(t,r)
                                    {
                                        var v = this.basisForm.findField('dfva').getValue();
                                        this.basisForm.findField('dfva').setValue(v);  	
                                    },
                                    scope:this
                            }})
    		          }]},

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
	
    createGridPanel:function(){
		this.lvdfbyotDtlStoreType=Ext.data.Record.create([
            {name:'sqno'},
            {name:'fryr'},
            {name:'toyr'},
            {name:'days'},
            {name:'remk'}
		]);
        
		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},this.lvdfbyotDtlStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:lvdfbyotKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + lvdfbyotPageName + '.mvc/getDefDtl',
	   		listeners:{
                loadexception:function(o,t,response)
                {
                    var o= Ext.util.JSON.decode(response.responseText);		   		
                    //this.dtlgrid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                load:function(p){
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
        
        store.load({params:params});
        
        return new Ext.grid.EditorGridPanel({
            id:'defDtlGrid',
    		border:true, 
            height:265,
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
	            rowclick:function(t,i,r){
	                this.controlButton(this.tabId);

                    var sm=this.dtlgrid.getSelectionModel();
                    var record=sm.getSelected();
                    
                    this.fryr = record.get('fryr');
                    this.toyr = record.get('toyr');
                    this.oldtoyr = this.toyr;
                    this.oldindex = i;
	            },
	            scope:this
            },       
            sm: new Ext.grid.RowSelectionModel(),              
            cm: new Ext.grid.ColumnModel([ 
            {header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno',width: 50},
            {header:HRMSRes.Leavel_Label_fryr,sortable: true, dataIndex: 'fryr'},
            {id:'toyr',header: HRMSRes.Leavel_Label_toyr,dataIndex: 'toyr',width: 100,
                editor: new Ext.form.NumberField({allowBlank:false,allowDecimals:false,allowNegative:false,
                            listeners:{
                                change:function(p,n,o){
                                    this.toyr = n;
                                    this.oldtoyr = o;
                                },
                                blur:function(p){
                                    if (this.toyr<this.fryr) {
                                        var r=this.dtlgrid.getStore().getAt(this.oldindex);
                                        r.set('toyr',this.oldtoyr);
                                    }
                                    else{
                                        this.rebuild(this.oldindex,this.toyr);
                                    }
                                },
                                scope:this
                            }})},

            {id:'days',header: HRMSRes.Leavel_Label_days,dataIndex: 'days',width: 100,
                editor: new Ext.form.NumberField({allowBlank:false,allowNegative:false})},

            {id:'remk',header: HRMSRes.Public_Label_remk,dataIndex: 'remk',width: 100,
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
	rebuild:function(s,v){
	    for (var i=(s+1);i<this.dtlgrid.getStore().getCount();i++)
	    {
	        var r = this.dtlgrid.getStore().getAt(i);
	        var diff = r.get('toyr') - r.get('fryr');
	        var x = r.get('fryr');
	        r.set('fryr',v+1);

	        if (r.get('toyr').toString()=='') return ;

	        r.set('toyr',v + 1 + diff );
	        s = i;
	        v = v + 1 + diff;
	    }
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
        params['dftx']=this.basisForm.findField('dfva').getRawValue();
        
        var dtlparams=[];       
        var st=this.dtlgrid.getStore();
        for (var i =0; i< st.getCount();i++ ){
            var x = st.getAt(i);
            var p = {dfno:this.basisForm.findField('dfno').getValue(),
                     sqno:x.get('sqno').toString(),fryr:x.get('fryr').toString(),
                     toyr:x.get('toyr').toString(),days:x.get('days').toString(),
                     remk:x.get('remk')};
            dtlparams[dtlparams.length] = p;
        };
        
        var keyparams=[];
        keyparams[0]={ColumnName:'dfno',ColumnValue:this.basisForm.findField('dfno').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + lvdfbyotPageName + '.mvc/'+method,
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
	query:function(){	
        var params=[];	
        var f = this.basisForm.findField(lvdfbyotKeyColumn);        
        var p={
            ColumnName:f.getName(),
            ColumnValue:f.getValue()                
        };
        params[params.length]=p;                  

        var loadParams={start:0,limit:Pagination.pagingSize};
        /***modified for adjquery**/
        this.dtlgrid.queryParams={
            params:params
        };
        this.dtlgrid.store.baseParams={record:Ext.util.JSON.encode(this.dtlgrid.queryParams)};
	    this.dtlgrid.getStore().load({params:loadParams});
    },
    addline:function(){	    
        var n=this.dtlgrid.getStore().getCount() + 1;
        var x = 0;
        var m =0;
        
        if (n<2)
            m = 0;
        else{
            x = this.dtlgrid.store.getAt(n-2).get('toyr');
            
            if (x.toString()=='') return;
            
            m = x+1;
        }

        var params=[];
        params['sqno']= n.toString();
        params['fryr']= m;
        params['toyr']='';
        params['days']='0';
        params['remk']=' ';

        var store = this.dtlgrid.getStore();
        store.add(new store.recordType(params));
        
        this.dtlgrid.store.getAt(this.dtlgrid.store.getCount()-1).set('remk',' ');
    },
	deleteline:function(){
	    var sm=this.dtlgrid.getSelectionModel();
	    
	    if (sm.hasSelection())
	    {
	        var record=sm.getSelected();
            this.dtlgrid.store.remove(record);
            
            var previndex = this.oldindex=0?-1:this.oldindex-1;
            
            if (previndex>=0){
	            var r = this.dtlgrid.getStore().getAt(previndex);
                var x = r.get('toyr');
                
                this.rebuild(this.oldindex,x);
            }
                        
            this.resortsqno();
            this.dtlgrid.store.totalLength-=1;
            this.dtlgrid.getBottomToolbar().updateInfo();   
        }
        
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
	    var params={record:Ext.encode({params:[{ColumnName:lvdfbyotKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.dtlgrid.queryParams){
            this.dtlgrid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.dtlgrid.queryParams);
            delete this.dtlgrid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + lvdfbyotPageName + '.mvc/exportexceldtl';
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
    resortsqno:function(){
        var st = this.dtlgrid.getStore();
        for (var i =1; i<= st.getCount();i++ ){
            st.getAt(i-1).set('sqno',i.toString());
        }
    }        
});


var lvdfbyotPanel=function(){
    this.tabId=lvdfbyotConfig.tabId;
	this.init();	
	
	lvdfbyotPanel.superclass.constructor.call(this,{
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
	            hidden:lvdfbyotConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new lvdfbyotEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden:lvdfbyotConfig.auth[this.tabId+'_' + 'edit']!='True',
	            handler: function(){
	            	new lvdfbyotEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:lvdfbyotConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:lvdfbyotConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new lvdfbyotQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:lvdfbyotConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:lvdfbyotConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,lvdfbyotConfig.muf?'delete':'add',lvdfbyotConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(lvdfbyotPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var lvdfbyotStoreType=Ext.data.Record.create([
            {name:'dfno'},
            {name:'ltcd'},
            {name:'ltnm'},
            {name:'dfby'},
            {name:'dfbytext'},
            {name:'dfva'},
            {name:'dftx'},
            {name:'remk'},
            {name:'lmtm'},
            {name:'lmur'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},lvdfbyotStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:lvdfbyotKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + lvdfbyotPageName + '.mvc/list',
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
                {header:HRMSRes.Leavel_Label_dfno,sortable: true, dataIndex: 'dfno'},
                {header:HRMSRes.Master_Label_ltnm,sortable: true, dataIndex: 'ltnm'},
                {header:HRMSRes.Leavel_Label_dfby,sortable: true, dataIndex: 'dfbytext'},
                {header:HRMSRes.Leavel_Label_dfva,sortable: true, dataIndex: 'dftx'},
                {header:HRMSRes.Public_Label_remk,sortable: true, dataIndex: 'remk'},
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
        keyparams[0]={ColumnName:'dfno',ColumnValue:record.get('dfno'),ColumnType:'int'};
        
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ lvdfbyotPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:lvdfbyotKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + lvdfbyotPageName + '.mvc/exportexcel';
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
    new lvdfbyotPanel();
})

    </script>

</body>
</html>