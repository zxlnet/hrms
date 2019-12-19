<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rcintsch.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.rcintsch.rcintsch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var rcintschConfig=Ext.decode('<%=ViewData["config"] %>'); 
rcintschConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var rcintschPageName = 'rcintsch';
var rcintschKeyColumn='apno';
            
var rcintschQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	rcintschQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(rcintschQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_apno,
                        name: 'apno',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_jbcd,
                        name: 'jbcd',stateful:false,anchor:'95%'}]},
                        
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_jbnm,
                        name: 'jbnm',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_dpnm ,
                        name: 'dpcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getDepartment',
		                    listeners:{load:function(){var f=this.form.findField('dpcd');f.setValue(f.getValue())},scope:this}})
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
var rcintschEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	
    this.dtldeletedline=[]; 

    this.fullFormPanel = {
                xtype:'panel',
	            layout:'fit',
	            autoHeight:true,
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
	                tbar: [/*{ 
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
	                            },'-',*/{ 
        	                        id:this.tabId+'_exportexceldtl',
                                    iconCls:'icon-export', 
                                    text:HRMSRes.Public_Toolbar_ToExcel, 
                                    handler: this.exportExcelDtl, 
                                    scope: this 
                                }],
      		         items: this.dtlgrid
	            }]
            };

	rcintschEditWindow.superclass.constructor.call(this,{
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
		        var keyField = this.basisForm.findField(rcintschKeyColumn);
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

Ext.extend(rcintschEditWindow,Ext.Window,{
    init:function(){
		this.dtlgrid = this.createGridPanel();

		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();
	},
	createBasisFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:140,
	         header:true,
	         autoHeight:true,
      		 items: [
      		    {
      		    layout:'column',
      		    items:[
       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_apno ,
                     allowBlank:false,name: 'apno',stateful:false,anchor:'95%',disabled:true}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Recruitment_Label_apdt,id:'apdt',
                        name:'apdt',editable:false,height:22,anchor:'95%',disabled:true,
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_dpnm ,disabled:true,
                        name: 'dpcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getDepartment',
		                    listeners:{load:function(){var f=this.basisForm.findField('dpcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_jbcd ,
                     allowBlank:false,name: 'jbcd',stateful:false,anchor:'95%',disabled:true}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_jbnm,
                     allowBlank:false,name: 'jbnm',stateful:false,anchor:'95%',disabled:true}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Recruitment_Label_vacn,
                        name: 'vacn',stateful:false,anchor:'95%',decimalPrecision:0,keepZero:false,allowBlank:false,disabled:true}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.TableDef_Label_rtcd,disabled:true,
                        name: 'rtcd',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getRecruitmentType',
		                    listeners:{load:function(){var f=this.basisForm.findField('rtcd');f.setValue(f.getValue())},scope:this}})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_sftm,disabled:true,
                        name: 'stcd',stateful:false,allowBlank:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getStaffType',
		                    listeners:{load:function(){var f=this.basisForm.findField('stcd');f.setValue(f.getValue())},scope:this}})
    		          }]}
      		    ]
      		 }] 
       })
	},
    createGridPanel:function(){

		this.rcintschEditDtlStoreType=Ext.data.Record.create([
            {name:'isno'},
            {name:'aino'},
            {name:'frtm'},
            {name:'totm'},
            {name:'hrof'},
            {name:'otof'},
            {name:'iacd'},
             {name:'iade'},
            {name:'remk'},
            {name:'stus'},
            {name:'lmtm'},
            {name:'lmur'},
            {name:'rfid'},
            {name:'cnnm'},
            {name:'ennm'}
		]);
        
		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},this.rcintschEditDtlStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:rcintschKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + rcintschPageName + '.mvc/getScheduleList',
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
    		border:true, height:236,monitorResize:false, loadMask:true,ds: store, frame:false,collapsible: true,
            animCollapse: false,editable:true,clicksToEdit:1,
            viewConfig: { 
		        //forceFit: true 
		    }, 
            listeners:{
	            rowclick:function(){
	                this.controlButton(this.tabId);
	            },
	            scope:this
            },                  
            sm: new Ext.grid.RowSelectionModel(),              
            cm: new Ext.grid.ColumnModel([ 
            {header:HRMSRes.Recruitment_Label_aino,sortable: true, dataIndex: 'aino'},
            {header:HRMSRes.Recruitment_Label_cnnm,sortable: true, dataIndex: 'cnnm'},
            {header:HRMSRes.Recruitment_Label_ennm,sortable: true, dataIndex: 'ennm'},
            {id:'frtm',header: HRMSRes.Recruitment_Label_frtm,dataIndex: 'frtm', renderer:formatDateTime,width:140,
                            editor: new Ext.form.DateField({format: DATE_FORMAT.DATETIME_NOSECOND,minValue: '1980/01/01'})},
            {id:'totm',header: HRMSRes.Recruitment_Label_totm,dataIndex: 'totm', renderer:formatDateTime,width:140,
                            editor: new Ext.form.DateField({format: DATE_FORMAT.DATETIME_NOSECOND,minValue: '1980/01/01'})},                
            {id:'hrof',header: HRMSRes.Recruitment_Label_hrof,dataIndex: 'hrof', editor: new Ext.form.TextField({})},
            {id:'otof',header: HRMSRes.Recruitment_Label_otof,dataIndex: 'otof', editor: new Ext.form.TextField({})},
            {header: HRMSRes.Recruitment_Label_iade,dataIndex: 'iade',
                editor: new Ext.form.ComboBox({typeAhead: true,triggerAction: 'all',mode:'local',id:'iade',
                lazyRender:false,listClass: 'x-combo-list-small',
                displayField: 'DisplayField',valueField:'DisplayField',
                store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getRecruitmentAssessment'}),
		        listeners:{
		            select:function(c,r,i)
		            {var sm = this.dtlgrid.getSelectionModel();var rec= sm.getSelected();rec.set('iacd',r.data['ValueField'])},
                    scope:this
                    }
		     })}, 
		    {header:HRMSRes.Recruitment_Label_iacd,sortable: true, dataIndex: 'iacd',hidden:true},
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
		this.grid.getBottomToolbar().diplayMsg.update('');
        var dtlparams=[];       
        var sm=this.dtlgrid.store.getModifiedRecords();
        for (var i =0; i< sm.length;i++ ){
            var x = sm[i];
            var p = {isno:'',aino:x.get('aino'),
                          frtm:x.get('frtm'),totm:x.get('totm'),    
                          hrof:x.get('hrof'),otof:x.get('otof'),  
                          iacd:x.get('iacd'),remk:x.get('remk'),  
                          stus:x.get('stus')  };
           
            dtlparams[dtlparams.length] = p;
        };
        
        var keyparams=[];
        keyparams[0]={ColumnName:'apno',ColumnValue:this.basisForm.findField('apno').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + rcintschPageName + '.mvc/'+method,
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		   		
		   		if (o.status=='success'){
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
		   params: {record:Ext.encode({keycolumns:keyparams,dtlparams:dtlparams})}
		});
	},
    query:function(){	    
        var params=[];	
        var f = this.basisForm.findField(rcintschKeyColumn);        
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
    /*addline:function(){	    
        var n=this.dtlgrid.getStore().getCount() + 1;
        var params=[];
        params['sqno']= n.toString();
        params['itnm']= ''
        params['itde']='';
        params['rtcd']='';
        params['remk']='';

        var store = this.dtlgrid.getStore();
        store.add(new store.recordType(params));
    },
    deleteline:function(){
        var sm=this.dtlgrid.getSelectionModel();
	    
        if (sm.hasSelection())
        {
            this.resortsqno();
            this.dtlgrid.store.totalLength-=1;
            this.dtlgrid.getBottomToolbar().updateInfo();   
        }
    },*/
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
        var params={record:Ext.encode({params:[{ColumnName:rcintschKeyColumn,ColumnValue:''}],headers:header})};
	    
        if(this.dtlgrid.queryParams){
            this.dtlgrid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.dtlgrid.queryParams);
            delete this.dtlgrid.queryParams.header;
        }
	    
        var form=document.createElement('form');
        form.name='excelForm';
        form.method='post';
        form.action=ContextInfo.contextPath+ '/' + rcintschPageName + '.mvc/exportexceldtl';
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
        //Ext.getCmp(id+ '_deletedtl').setDisabled(enabled);
    }
    /*,
    resortsqno:function(){
        var st = this.dtlgrid.getStore();
        for (var i =1; i<= st.getCount();i++ ){
            st.getAt(i-1).set('sqno',i.toString());
        }
    }*/
});

var rcintschPanel=function(){
    this.tabId=rcintschConfig.tabId;
	this.init();	
	
	rcintschPanel.superclass.constructor.call(this,{
		applyTo:this.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	autoScroll:true,      
	        tbar: [
	        /*{ 
	        	id:this.tabId+ '_add',
	            iconCls:'icon-add', 
	            text: HRMSRes.Public_Toolbar_Add, 
	            hidden:rcintschConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new rcintschEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',*/
	        { 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden: rcintschConfig.auth[this.tabId + '_' + 'edit'] != 'True',
	            disabled: true,
	            handler: function(){
	            	new rcintschEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },
	        /*{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove',
                hidden: rcintschConfig.auth[this.tabId + '_' + 'delete'] != 'True',
                disabled: true,
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },*/
            '-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:rcintschConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new rcintschQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:rcintschConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:rcintschConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,rcintschConfig.muf?'delete':'add',rcintschConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(rcintschPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var rcintschStoreType=Ext.data.Record.create([
            {name:'apno'},
            {name:'apdt'},
            {name:'dpcd'},
            {name:'jbcd'},
            {name:'jbnm'},
            {name:'hcnt'},
            {name:'vacn'},
            {name:'rtcd'},
            {name:'stcd'},
            {name:'hctl'},
            {name:'whhi'},
            {name:'jbde'},           
            {name:'jbre'},
            {name:'exdt'},
            {name:'stus'},
            {name:'remk'},
            {name:'lmtm'},
            {name:'lmur'},
            {name:'rfid'},
            {name:'dpnm'},
            {name:'rtnm'},
            {name:'stnm'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},rcintschStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:rcintschKeyColumn,ColumnValue:rcintschConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/rcapplic.mvc/GetActiveApplication',
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
		       //forceFit: true 
		    }, 
            listeners:{
	            rowclick:function(){
	                this.controlButton(this.tabId);
	            },
	            scope:this
            },                      
            cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Recruitment_Label_apno,sortable: true, dataIndex: 'apno'},
                { header: HRMSRes.Recruitment_Label_apdt, sortable: true, dataIndex: 'apdt',renderer:formatDateNoTime },
                { header: HRMSRes.Master_Label_dpnm, sortable: true, dataIndex: 'dpnm' },
                {header:HRMSRes.Recruitment_Label_jbcd,sortable: true, dataIndex: 'jbcd'},
                {header:HRMSRes.Recruitment_Label_jbnm,sortable: true, dataIndex: 'jbnm'},
                {header:HRMSRes.TableDef_Label_rtnm,sortable: true, dataIndex: 'rtnm'},
                {header:HRMSRes.Master_Label_stnm,sortable: true, dataIndex: 'stnm'},
                {header:HRMSRes.Recruitment_Label_vacn,sortable: true, dataIndex: 'vacn'},
                {header:HRMSRes.Recruitment_Label_stus,sortable: true, dataIndex: 'stus'},
                {header:HRMSRes.Public_Label_exdt,sortable: true, dataIndex: 'exdt',renderer:formatDateNoTime},
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
        keyparams[0]={ColumnName:'iacd',ColumnValue:record.get('iacd')};

	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ rcintschPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:rcintschKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + rcintschPageName + '.mvc/exportexcel';
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
    new rcintschPanel();
})

    </script>

</body>
</html>
