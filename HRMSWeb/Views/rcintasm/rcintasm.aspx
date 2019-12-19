<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rcintasm.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.rcintasm.rcintasm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var rcintasmConfig=Ext.decode('<%=ViewData["config"] %>'); 
rcintasmConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var rcintasmPageName = 'rcintasm';
var rcintasmKeyColumn='iacd';
            
var rcintasmQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	rcintasmQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(rcintasmQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_iacd,
                        name: 'iacd',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_iade,
                        name: 'iade',stateful:false,anchor:'95%'}]}
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
var rcintasmEditWindow=function(grid,isNew){
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
	                                handler: function(){
	                                    this.addline();	                         
	                                }, 
	                                scope: this 
	                            },'-',{ 
	        	                    id:this.tabId+'_deletedtl',
	                                iconCls:'icon-remove', 
	                                disabled:true,
	                                text:HRMSRes.Public_Toolbar_Delete, 
	                                handler: this.deleteline, 
	                                scope: this 
	                            },'-',{ 
        	                        id:this.tabId+'_exportexceldtl',
                                    iconCls:'icon-export', 
                                    text:HRMSRes.Public_Toolbar_ToExcel, 
                                    handler: this.exportExcelDtl, 
                                    scope: this 
                                }],
      		         items: this.dtlgrid
	            }]
            };

	rcintasmEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:445, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.fullFormPanel,
        listeners:{
            show:function(){    
		        var keyField = this.basisForm.findField(rcintasmKeyColumn);
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
                        }});  }
                
                setLastModifiedInfo(rcintasmConfig,this.basisForm);
                
                if (!this.isNew)
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

Ext.extend(rcintasmEditWindow,Ext.Window,{
    init:function(){
		this.dtlgrid = this.createGridPanel();

		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_iacd+'(<font color=red>*</font>)',
                        name: 'iacd',allowBlank:false,stateful:false,anchor:'95%',disabled:!this.isNew}]},
     		        
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_iade+'(<font color=red>*</font>)',
                        name: 'iade',allowBlank:false,stateful:false,anchor:'95%'}]},
     		        
       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Public_Label_remk,height:30,
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
    createGridPanel:function(){

		this.rcintasmEditDtlStoreType=Ext.data.Record.create([
            {name:'iacd'},
            {name:'sqno'},
            {name:'itnm'},
            {name:'itde'},
            {name:'racd'},
            {name:'rtnm'},
            {name:'remk'},
            {name:'rfid'}
		]);
        
		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},this.rcintasmEditDtlStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:rcintasmKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + rcintasmPageName + '.mvc/getRecruitmentAssessmentDtl',
	   		//autoLoad:true,
	   		listeners:{
                loadexception:function(o,t,response)
                {
                    var o= Ext.util.JSON.decode(response.responseText);		   		
                },
                load:function(p){
                    this.controlButton(this.tabId);
                },
                add:function(p,r,i){
                    var v = p.getAt(i);
                },
                scope:this
            }
        });
        
        var params={
            start:0,
            limit:Pagination.pagingSize
        };
        
        return new Ext.grid.EditorGridPanel({
            id:'dtlGrid',
    		border:true, height:235,monitorResize:false, loadMask:true,ds: store, frame:false,collapsible: true,
            animCollapse: false,editable:true,clicksToEdit:1,
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
                {header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno',width: 50},
                {id:'itnm',header: HRMSRes.Recruitment_Label_itnm,dataIndex: 'itnm', editor: new Ext.form.TextField({})},
                {id:'itde',header: HRMSRes.Recruitment_Label_itde,dataIndex: 'itde', editor: new Ext.form.TextField({})},           
                {header: HRMSRes.Master_Label_ranm,dataIndex: 'rtnm',width: 150,
                    editor: new Ext.form.ComboBox({typeAhead: true,triggerAction: 'all',mode:'local',id:'rtdd',
                    lazyRender:false,listClass: 'x-combo-list-small',
                    displayField: 'DisplayField',valueField:'DisplayField',
                    store: new Ext.data.Store({ 
		                        reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                        fields: ['ValueField','DisplayField']}), autoLoad:true,
		                        url:ContextInfo.contextPath+'/dropdown.mvc/getRating'}),
		            listeners:{
		                select:function(c,r,i)
		                {var sm = this.dtlgrid.getSelectionModel();var rec= sm.getSelected();rec.set('racd',r.data['ValueField'])},
                        scope:this
                        }
		         })}, 
                {id:'racd',header: HRMSRes.Recruitment_Label_rtcd,dataIndex: 'racd', hidden:true},  
                
                {id:'remk',header: HRMSRes.Public_Label_remk,dataIndex: 'remk',width: 220,
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
		this.basisForm.items.each(function(f){
            if(f.isFormField && !f.nonUpdateField){
                params[f.getName()]=f.getValue();
            }
        });
		
        var dtlparams=[];       
        var sm=this.dtlgrid.store.getModifiedRecords();
        for (var i =0; i< sm.length;i++ ){
            var x = sm[i];
            var p = {iacd:this.basisForm.findField('iacd').getValue(),
                     sqno:x.get('sqno'),
                     itnm:x.get('itnm'),itde:x.get('itde'),racd:x.get('racd'),remk:x.get('remk')};
            
            dtlparams[dtlparams.length] = p;
        };
        
        var keyparams=[];
        keyparams[0]={ColumnName:'iacd',ColumnValue:this.basisForm.findField('iacd').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + rcintasmPageName + '.mvc/'+method,
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
        var f = this.basisForm.findField(rcintasmKeyColumn);        
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
        var m=this.dtlgrid.getStore().getCount();
        var n = 1;
        if (m>0)
            n=parseInt(this.dtlgrid.getStore().getAt(this.dtlgrid.getStore().getCount()-1).get('sqno'))+1;
            
        var params=[];
        params['sqno']= n.toString();
        params['itnm']= ''
        params['itde']='';
        params['racd']='';
        params['remk']='';

        var store = this.dtlgrid.getStore();
        store.add(new store.recordType(params));
    },
    deleteline:function(){
        var sm=this.dtlgrid.getSelectionModel();
	    
        if (sm.hasSelection())
        {
            //this.resortsqno();
            
            var record=sm.getSelected();
            
            this.dtldeletedline[this.dtldeletedline.length]=record.get('sqno');
            this.dtlgrid.store.remove(record);
            
            this.dtlgrid.store.totalLength-=1;
            this.dtlgrid.getBottomToolbar().updateInfo();   
        }
    },
    exportExcelDtl:function(){
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
        var params={record:Ext.encode({params:[{ColumnName:rcintasmKeyColumn,ColumnValue:''}],headers:header})};
	    
        if(this.dtlgrid.queryParams){
            this.dtlgrid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.dtlgrid.queryParams);
            delete this.dtlgrid.queryParams.header;
        }
	    
        var form=document.createElement('form');
        form.name='excelForm';
        form.method='post';
        form.action=ContextInfo.contextPath+ '/' + rcintasmPageName + '.mvc/exportexceldtl';
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

var rcintasmPanel=function(){
    this.tabId=rcintasmConfig.tabId;
	this.init();	
	
	rcintasmPanel.superclass.constructor.call(this,{
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
	            hidden:rcintasmConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new rcintasmEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden: rcintasmConfig.auth[this.tabId + '_' + 'edit'] != 'True',
	            disabled: true,
	            handler: function(){
	            	new rcintasmEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove',
                hidden: rcintasmConfig.auth[this.tabId + '_' + 'delete'] != 'True',
                disabled: true,
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:rcintasmConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new rcintasmQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:rcintasmConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:rcintasmConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,rcintasmConfig.muf?'delete':'add',rcintasmConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(rcintasmPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var rcintasmStoreType=Ext.data.Record.create([
		    {name:'iacd'},{name:'iade'},{name:'remk'},{name:'lmtm'},
            {name:'lmur'},
            {name:'rfid'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},rcintasmStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:rcintasmKeyColumn,ColumnValue:rcintasmConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/' + rcintasmPageName + '.mvc/list',
	   		listeners:{
                loadexception:function(o,t,response)
                {var o= Ext.util.JSON.decode(response.responseText);		   		
                this.grid.getBottomToolbar().diplayMsg.update(o.msg);},
                load:function(){this.controlButton(this.tabId);},
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
                {header:HRMSRes.Recruitment_Label_iacd,sortable: true, dataIndex: 'iacd'},
                {header:HRMSRes.Recruitment_Label_iade,sortable: true, dataIndex: 'iade'},
                {header:HRMSRes.Public_Label_remk,sortable: true, dataIndex: 'remk'},
                {header:HRMSRes.Public_Label_lmur,sortable: true, dataIndex: 'lmur'},
                {header:HRMSRes.Public_Label_lmtm,sortable: true, dataIndex: 'lmtm',renderer:formatDate}
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
        keyparams[0]={ColumnName:'iacd',ColumnValue:record.get('iacd')};

	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ rcintasmPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:rcintasmKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + rcintasmPageName + '.mvc/exportexcel';
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
    new rcintasmPanel();
})

    </script>

</body>
</html>