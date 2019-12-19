<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rcintrst.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.rcintrst.rcintrst" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var rcintrstConfig=Ext.decode('<%=ViewData["config"] %>'); 
rcintrstConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var rcintrstPageName = 'rcintrst';
var rcintrstKeyColumn='isno';
            
var rcintrstQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	rcintrstQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(rcintrstQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_cnnm,
                        name: 'cnnm',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_ennm,
                        name: 'ennm',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_jbcd,
                        name: 'jbcd',stateful:false,anchor:'95%'}]},
                        
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_jbnm,
                        name: 'jbnm',stateful:false,anchor:'95%'}]}
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
var rcintrstEditWindow=function(grid,isNew){
	this.grid=grid;
	this.isNew=isNew||false;
	this.store=this.grid.getStore();
	this.init();	
    this.dtldeletedline=[]; 

    this.rcintrstTab=new Ext.TabPanel({
            autoTabs:true,
            activeTab:0,
            border:false,
            layoutOnTabChange:true,
            frame:true,
            autoHeight:true,
            items:[{id:'tab1',title: HRMSRes.Personal_Label_Basis,items: this.basisFormPanel},
                       {id:'tab2',title: HRMSRes.Personal_Label_Others,items: this.assFormPanel},
                       {id:'tab3',title: HRMSRes.Recruitment_Label_assm,items: this.ratingFormPanel}
                   ]
        }),

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
		            items:this.rcintrstTab
	            }]
            };

	rcintrstEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:400, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.fullFormPanel,
        listeners:{
            show:function(){    
		        var keyField = this.basisForm.findField(rcintrstKeyColumn);
                if(!this.isNew){	
			        var data=this.grid.getSelectionModel().getSelected();
                    this.basisForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            if (f.xtype=='timefield')
                                value = formatTime(value);
                            
                            if (f.xtype=='datefield')
                                value = formatDateTimeNoSecond(value);
                                    
                            f.setValue(value);                                               	            
                        }});  

                    this.rcintrstTab.setActiveTab('tab2');
                    this.assForm.items.each(function(f){
                        if(f.isFormField){
                            var value=data.get(f.getName());
                            f.setValue(value);                    	            
                        }
                    }); 
                }
                
                if (!this.isNew)
                    this.query();
                
                this.rcintrstTab.setActiveTab('tab1');      
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

Ext.extend(rcintrstEditWindow,Ext.Window,{
    init:function(){
		this.dtlgrid = this.createRatingGridPanel();

		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();
		
		this.assFormPanel=this.createAssFormPanel();
		this.assForm=this.assFormPanel.getForm();

		this.ratingFormPanel=this.createRatingFormPanel();
		this.ratingForm=this.ratingFormPanel.getForm();
	},
	createBasisFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:false, 
	         labelWidth:140,
	         header:true,
	         autoHeight:true,
      		 items: [
      		    {
      		    layout:'column',
      		    items:[
       		        /*{columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_apno ,
                     allowBlank:false,name: 'apno',stateful:false,anchor:'95%',disabled:true}]},

     		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'datefield',fieldLabel:HRMSRes.Recruitment_Label_apdt,id:'apdt',
                        name:'apdt',editable:false,height:22,anchor:'95%',disabled:true,
                        format: DATE_FORMAT.DATEONLY,minValue: '1980/01/01',stateful:false,
           	 	        invalidText : '{0} '+ HRMSRes.Public_Messsage_FormatInCorrect +' {1}'}]},
                    */    
       		        {columnWidth:0,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_isno ,hidden:true,hideLabel:true,
                     allowBlank:false,name: 'isno',stateful:false,anchor:'95%',disabled:true,updateField:true}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_jbcd ,
                     allowBlank:false,name: 'jbcd',stateful:false,anchor:'95%',disabled:true}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_jbnm,
                     allowBlank:false,name: 'jbnm',stateful:false,anchor:'95%',disabled:true}]},

      		        /*{columnWidth:.5,layout: 'form',
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
    		          }]},*/
    		          
       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_cnnm ,
                     allowBlank:false,name: 'cnnm',stateful:false,anchor:'95%',disabled:true}]},

       		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_ennm ,
                     allowBlank:false,name: 'ennm',stateful:false,anchor:'95%',disabled:true}]},
    		         
     		        { columnWidth: .5, layout: 'form',
     		            items: [{ xtype: 'datefield', fieldLabel: HRMSRes.Recruitment_Label_frtm, id: 'frtm',updateField:true,
     		                name: 'frtm', editable: false, height: 22, anchor: '95%',
     		                format: DATE_FORMAT.DATETIME_NOSECOND, minValue: '1980/01/01', stateful: false,
     		                invalidText: '{0} ' + HRMSRes.Public_Messsage_FormatInCorrect + ' {1}'}]
     		            },
     		            
     		        { columnWidth: .5, layout: 'form',
     		            items: [{ xtype: 'datefield', fieldLabel: HRMSRes.Recruitment_Label_totm, id: 'totm',updateField:true,
     		                name: 'totm', editable: false, height: 22, anchor: '95%',
     		                format: DATE_FORMAT.DATETIME_NOSECOND, minValue: '1980/01/01', stateful: false,
     		                invalidText: '{0} ' + HRMSRes.Public_Messsage_FormatInCorrect + ' {1}'}]
     		            },
     		            
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Recruitment_Label_itrs,updateField:true,
                        name: 'itrs',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getInterviewResult',
		                    listeners:{load:function(){var f=this.basisForm.findField('itrs');f.setValue(f.getValue())},scope:this}})
    		          }]},

       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Recruitment_Label_hrof,height:30,updateField:true,
                        name: 'hrof',stateful:false,anchor:'98%'}]},
    		          
       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Recruitment_Label_otof,height:30,updateField:true,
                        name: 'otof',stateful:false,anchor:'98%'}]},
    		         
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Recruitment_Label_scty,updateField:true,
                        name: 'scty',stateful:false,typeAhead: true,allowBlank:false,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getRecruitmentScheduleType',
		                    listeners:{load:function(){var f=this.basisForm.findField('scty');f.setValue(f.getValue())},scope:this}})
    		          }]},
    		          
       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Public_Label_remk,height:50,updateField:true,
                        name: 'remk',stateful:false,anchor:'98%'}]},
                        
       		        {columnWidth:0,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Recruitment_Label_iacd ,
                     allowBlank:false,name: 'iacd',stateful:false,anchor:'95%',hidden:true,hideLabel:true}]}
    		          
      		    ]
      		 }] 
       })
	},
	createAssFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
      		 items: [
      		    {},{
      		    layout:'column',
      		    items:[
       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Recruitment_Label_ovas,height:120,updateField:true,
                        name: 'ovas',stateful:false,anchor:'98%'}]},
                        
       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Recruitment_Label_recm,height:120,updateField:true,
                        name: 'recm',stateful:false,anchor:'98%'}]}
      		    ]
      		 }] 
       })
	},	
	createRatingFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         tbar: [
	                   /*{ 
	        	            id:this.tabId+'_adddtl_education',
	                        iconCls:'icon-add', 
	                        text: HRMSRes.Public_Toolbar_Add, 
	                        handler: function(){
	                            this.addline();	                         
	                        }, 
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_deletedtl_education',
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
       })
	},		
    createRatingGridPanel:function(){
		this.rcintrstRatingDtlStoreType=Ext.data.Record.create([
            {name:'isno'},
            {name:'iacd'},
            {name:'sqno'},
            {name:'itnm'},
            {name:'itde'},
            {name:'rtcd'},
            {name:'racd'},
            {name:'rtnm'},
            {name:'remk'},
            {name:'rfid'}
		]);
        
		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},this.rcintrstRatingDtlStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:rcintrstKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + rcintrstPageName + '.mvc/getRatingDtl',
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
    		border:true, height:256,monitorResize:false, loadMask:true,ds: store, frame:false,collapsible: true,
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
                {header:HRMSRes.Public_Label_sqno,sortable: true, dataIndex: 'sqno',width: 50},
                {id:'itnm',header: HRMSRes.Recruitment_Label_itnm,dataIndex: 'itnm', editor: new Ext.form.TextField({})},
                {id:'itde',header: HRMSRes.Recruitment_Label_itde,dataIndex: 'itde', editor: new Ext.form.TextField({})},           
                {header: HRMSRes.Master_Label_ranm,dataIndex: 'rtnm',width: 150,
                    editor: new Ext.form.ComboBox({typeAhead: true,triggerAction: 'all',mode:'local',id:'rtdd',
                    lazyRender:false,listClass: 'x-combo-list-small',
                    displayField: 'DisplayField',valueField:'DisplayField',
                    store: new Ext.data.Store({ 
		                        reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                        fields: ['ValueField','DisplayField','MiscField1','MiscField2']}), autoLoad:true,
		                        url:ContextInfo.contextPath+'/dropdown.mvc/getRatingDtl'}),
		            listeners:{
		                select:function(c,r,i)
		                {var sm = this.dtlgrid.getSelectionModel();var rec= sm.getSelected();rec.set('rtcd',r.data['ValueField'])},
		                focus:function(p)
                            {var f = Ext.getCmp('rtdd');var sm = this.dtlgrid.getSelectionModel();var rec= sm.getSelected();
                                f.store.baseParams={record:rec.get('racd')};f.store.load();},
                        scope:this
                        }
		         })}, 
                {id:'rtcd',header: HRMSRes.Recruitment_Label_rtcd,dataIndex: 'rtcd', hidden:true},  
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
		this.rcintrstTab.setActiveTab('tab1');
		this.basisForm.items.each(function(f){
            if(f.isFormField && f.updateField){
                params[f.getName()]=f.getValue();
            }
        });
        this.rcintrstTab.setActiveTab('tab2');
		this.assForm.items.each(function(f){
            if(f.isFormField && f.updateField){
                params[f.getName()]=f.getValue();
            }
        });
		
        var dtlparams=[];       
        var sm=this.dtlgrid.store.getModifiedRecords();
        for (var i =0; i< sm.length;i++ ){
            var x = sm[i];
            var p = {isno:this.basisForm.findField('isno').getValue(),
                          iacd:this.basisForm.findField('iacd').getValue(),
                          sqno:x.get('sqno'),itnm:x.get('itnm'),    
                          itde:x.get('itde'),rtcd:x.get('rtcd'),  
                          racd:x.get('racd'),remk:x.get('remk')};
           
            dtlparams[dtlparams.length] = p;
        };
        
        var keyparams=[];
        keyparams[0]={ColumnName:'isno',ColumnValue:this.basisForm.findField('isno').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + rcintrstPageName + '.mvc/'+method,
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
		   params: {record:Ext.encode({params:params,keycolumns:keyparams,dtlparams:dtlparams})}
		});
	},
    query:function(){	    
        var params=[];	
        var f = this.basisForm.findField('iacd');       
        var p={
            ColumnName:f.getName(),
            ColumnValue:f.getValue()                
        };
        params[params.length]=p;                  

        f = this.basisForm.findField('isno');     
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
        var params={record:Ext.encode({params:[{ColumnName:rcintrstKeyColumn,ColumnValue:''}],headers:header})};
	    
        if(this.dtlgrid.queryParams){
            this.dtlgrid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.dtlgrid.queryParams);
            delete this.dtlgrid.queryParams.header;
        }
	    
        var form=document.createElement('form');
        form.name='excelForm';
        form.method='post';
        form.action=ContextInfo.contextPath+ '/' + rcintrstPageName + '.mvc/exportexceldtl';
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

var rcintrstPanel=function(){
    this.tabId=rcintrstConfig.tabId;
	this.init();	
	
	rcintrstPanel.superclass.constructor.call(this,{
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
	            hidden:rcintrstConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new rcintrstEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',*/
	        { 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden: rcintrstConfig.auth[this.tabId + '_' + 'edit'] != 'True',
	            disabled: true,
	            handler: function(){
	            	new rcintrstEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove',
                hidden: rcintrstConfig.auth[this.tabId + '_' + 'delete'] != 'True',
                disabled: true,
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },
            '-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:rcintrstConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new rcintrstQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:rcintrstConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:rcintrstConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,rcintrstConfig.muf?'delete':'add',rcintrstConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(rcintrstPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var rcintrstStoreType=Ext.data.Record.create([
            {name:'isno'},
            {name:'frtm'},
            {name:'totm'},
            {name:'hrof'},
            {name:'otof'},
            {name:'itrs'},
            {name:'ovas'},
             {name:'recm'},
            {name:'scty'},
            {name:'remk'},
            {name:'lmtm'},
            {name:'lmur'},
            {name:'rfid'},
            {name:'cnnm'},
            {name:'ennm'},
            {name:'jbcd'},
            {name:'jbnm'},
            {name:'iacd'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},rcintrstStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:rcintrstKeyColumn,ColumnValue:rcintrstConfig.emno}]})},   
	   		url:ContextInfo.contextPath+'/rcintrst.mvc/list',
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
                {header:HRMSRes.Recruitment_Label_cnnm,sortable: true, dataIndex: 'cnnm'},
                {header:HRMSRes.Recruitment_Label_ennm,sortable: true, dataIndex: 'ennm'},
                { header: HRMSRes.Recruitment_Label_frtm, sortable: true, dataIndex: 'frtm',renderer:formatDateTimeNoSecond },
                { header: HRMSRes.Recruitment_Label_totm, sortable: true, dataIndex: 'totm',renderer:formatDateTimeNoSecond },
                { header: HRMSRes.Recruitment_Label_itrs, sortable: true, dataIndex: 'itrs' },
                { header: HRMSRes.Recruitment_Label_jbcd, sortable: true, dataIndex: 'jbcd' },
                { header: HRMSRes.Recruitment_Label_jbnm, sortable: true, dataIndex: 'jbnm' },
                {header:HRMSRes.Recruitment_Label_scty,sortable: true, dataIndex: 'scty'},
                {header:HRMSRes.Recruitment_Label_hrof,sortable: true, dataIndex: 'hrof'},
                {header:HRMSRes.Recruitment_Label_otof,sortable: true, dataIndex: 'otof'},
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
        keyparams[0]={ColumnName:'isno',ColumnValue:record.get('isno')};

	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ rcintrstPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:rcintrstKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + rcintrstPageName + '.mvc/exportexcel';
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
    new rcintrstPanel();
})

    </script>

</body>
</html>
