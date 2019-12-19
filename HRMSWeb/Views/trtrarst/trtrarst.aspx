<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="trtrarst.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.trtrarst.trtrarst" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var trtrarstConfig=Ext.decode('<%=ViewData["config"] %>'); 
trtrarstConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var trtrarstPageName = 'trtrarst';
var trtrarstKeyColumn='trcd';
            
var trtrarstQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	trtrarstQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(trtrarstQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Personal_Label_trcd,
                        name: 'trcd',stateful:false,anchor:'95%',UpperOnBlur:'true'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Training_Label_trnm,
                        name: 'trnm',stateful:false,anchor:'95%',UpperOnBlur:'true'}]},

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

var psskillEditWindow=function(emno,trcd,stcd){
	//this.grid=grid;
	//this.isNew=isNew||false;
	//this.store=this.grid.getStore();
	
	this.emno = emno;
	this.trcd = trcd;
	this.stcd = stcd;
	
	this.init();	
    this.dtldeletedline=[]; 

    this.fullFormPanel = {
                xtype:'panel',
	            layout:'fit',
	            autoHeight:true,
	            items:[
//	            {
//		            xtype:'panel',
//		            autoHeight:true,
//		            bodyStyle:'padding:0px;',
//		            border:false,
//		            baseCls:'x-fieldset-noborder',
//		            columnWidth: .50,
//		            items:this.basisFormPanel
//	            },
	            
	            {
		            xtype:'panel',
		            layout:'fit',
		            autoHeight:true,
		            bodyStyle:'padding:0px;',
		            border:false,
		            baseCls:'x-fieldset-noborder',
		            columnWidth: .50,
		            items:this.dtlgrid
	            }]
            };

	psskillEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:650, 
        height:350, 
        //title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        title:'Skill Details',
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.fullFormPanel,
        listeners:{
            show:function(){    
//		        var keyField = this.basisForm.findField(psskillKeyColumn);
//                if(!this.isNew){	
//			        var data=this.grid.getSelectionModel().getSelected();
//                    this.basisForm.items.each(function(f){
//                        if(f.isFormField){
//                            var value=data.get(f.getName());
//                            if (f.xtype=='timefield')
//                                value = formatTime(value);
//                            
//                            if (f.xtype=='datefield')
//                                value = formatDateNoTime(value);
//                                    
//                            f.setValue(value);                                               	            
//                        }});  }
                
                //setLastModifiedInfo(psskillConfig,this.basisForm);
                
//                if (!this.isNew)
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

Ext.extend(psskillEditWindow,Ext.Window,{
    init:function(){
		//this.basisFormPanel=this.createBasisFormPanel();
		//this.basisForm=this.basisFormPanel.getForm();
		this.dtlgrid = this.createGridPanel();
	},

    createGridPanel:function(){

		this.psskillEditDtlStoreType=Ext.data.Record.create([
            {name:'sinm'},{name:'racd'},{name:'remk'},{name:'sicd'},{name:'ranm'},{name:'rtnm'},{name:'rtcd'},{name:'racd1'}
		]);
        
		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},this.psskillEditDtlStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:'',ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + trtrarstPageName + '.mvc/getSkillItems',
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
            id:'chgDtlGrid',
    		border:true, height:280,monitorResize:false, loadMask:true,ds: store, frame:false,collapsible: true,
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
            {header:HRMSRes.Personal_Label_skit,id:'sinm',sortable: true, dataIndex: 'sinm',hidden:false},
            {header:'sicd',id:'sicd',sortable: true, dataIndex: 'sicd',hidden:true},

            {header: HRMSRes.Personal_Label_ratn,dataIndex: 'rtnm',width: 150,
                editor: new Ext.form.ComboBox({typeAhead: true,triggerAction: 'all',mode:'local',id:'rtdd',
                lazyRender:false,listClass: 'x-combo-list-small',
                displayField: 'DisplayField',valueField:'DisplayField',
                store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField','MiscField1','MiscField2']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getRatingDtl'}),
		        listeners:{
		            select:function(c,r,i)
		            {var sm = this.dtlgrid.getSelectionModel();var rec= sm.getSelected();rec.set('rtcd',r.data['ValueField']);rec.set('rtnm1',r.data['DisplayField']);},
                    focus:function(p)
                    {var f = Ext.getCmp('rtdd');var sm = this.dtlgrid.getSelectionModel();var rec= sm.getSelected();
                        f.store.baseParams={record:rec.get('racd')};f.store.load();},scope:this}
		     })}, 
		    {header:'racd',id:'racd',sortable: true, dataIndex: 'racd',hidden:true},
		    {header:'rtnm1',id:'rtnm',sortable: true, dataIndex: 'rtnm1',hidden:true},
            {header:'rtcd',id:'rtcd',sortable: true, dataIndex: 'rtcd',hidden:true},
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
            var p = {emno:this.basisForm.findField('emno').getValue(),
                     sqno:this.basisForm.findField('sqno').getValue(),
                     sicd:x.get('sicd'),racd:x.get('racd'),rtcd:x.get('rtcd'),remk:x.get('remk'),
                     lmtm:this.basisForm.findField('lmtm').getValue(),
                     lmur:this.basisForm.findField('lmur').getValue()};
            
            dtlparams[dtlparams.length] = p;
        };
        
        var keyparams=[];
        keyparams[0]={ColumnName:'emno',ColumnValue:this.basisForm.findField('emno').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + psskillPageName + '.mvc/'+method,
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
	Query:function(){	    
        var params=[];	
        //params[params.length]={ColumnName:'emno',ColumnValue:this.basisForm.findField('emno').getValue()}; 
        //params[params.length]={ColumnName:'stcd',ColumnValue:this.basisForm.findField('stcd').getValue()};
        params[params.length]={ColumnName:'emno',ColumnValue:this.emno}; 
        params[params.length]={ColumnName:'trcd',ColumnValue:this.trcd}; 
        params[params.length]={ColumnName:'stcd',ColumnValue:this.stcd};

        var loadParams={start:0,limit:Pagination.pagingSize};
        /***modified for adjquery**/
        this.dtlgrid.queryParams={
            params:params
        };
        this.dtlgrid.store.baseParams={record:Ext.util.JSON.encode(this.dtlgrid.queryParams)};
	    this.dtlgrid.getStore().load({params:loadParams});
    },
	controlButton:function(id){
        //var enabled=!this.dtlgrid.getSelectionModel().hasSelection();	    
        //Ext.getCmp(id+ '_deletedtl').setDisabled(enabled);
    }    
});

var trtrarstEditWindow=function(grid,isNew){
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
	        	            id:this.tabId+'_applytoall',
	                        iconCls:'icon-apply', 
	                        text: HRMSRes.Public_Toolbar_ApplyToAll, 
	                        handler: function(){
	                            this.applytoall();	                         
	                        }, 
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_clear',
	                        iconCls:'icon-dashboard', 
	                        text:HRMSRes.Public_Toolbar_Clear, 
	                        handler: this.clear, 
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_skdt',
	                        iconCls:'icon-dashboard', 
	                        text:'Skill Detail', 
	                        handler: this.skilldtl, 
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_reload',
	                        iconCls:'icon-query', 
	                        text:HRMSRes.Public_Toolbar_Reload, 
	                        handler: this.Query, 
	                        scope: this 
	                    }],
                        items:this.dtlgrid
	            }]
            };
                       
	trtrarstEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:750, 
        height:430, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.fullFormPanel,
        listeners:{
            show:function(){    
		        var keyField = this.basisForm.findField(trtrarstKeyColumn);
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

Ext.extend(trtrarstEditWindow,Ext.Window,{
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
      		    {layout:'column',
      		    items:[
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_trcd,
                        name: 'trcd',stateful:false,typeAhead: true,disabled:!this.isNew,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getCompanyTraining',
		                    listeners:{load: function(){var v = this.basisForm.findField('trcd').getValue();this.basisForm.findField('trcd').setValue(v);this.Query();},scope:this}}),
		                    listeners:{
		                        select:function(p){
		                            this.Query();
		                        },
		                        scope:this
		                    }
    		          }]}
      		    ]
      		 }] 
       })
	},
	
    createGridPanel:function(){
		var trtrarstDtlStoreType=Ext.data.Record.create([
		    {name:'sfid'},
		    {name:'stfn'},
            {name:'trcd'},
            {name:'emno'},
            {name:'athr'},
            {name:'isce'},
            {name:'ispa'},
            {name:'issk'},
            {name:'scor'},
            {name:'remk'},
            {name:'lmur'},
            {name:'lmtm'},
            {name:'rfid'},
            {name:'trnm'},
            {name:'stcd'},
            {name:'ifce'},
            {name:'ifsk'}
		]);
        
		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},trtrarstDtlStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:trtrarstKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + trtrarstPageName + '.mvc/getTrainingResult',
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
               
        var ispaCheckColumn = new Ext.grid.CheckColumn({
               header: HRMSRes.Training_Label_ispa,
               dataIndex: 'ispa',
               width: 70
            });
               
        var isceCheckColumn = new Ext.grid.CheckColumn({
               header: HRMSRes.Training_Label_isce,
               dataIndex: 'isce',
               width: 70
            });

        var isskCheckColumn = new Ext.grid.CheckColumn({
               header: HRMSRes.Training_Label_issk,
               dataIndex: 'issk',
               width: 70
            });

        return new Ext.grid.EditorGridPanel({
            id:'trtrarstDtlGrid',
    		border:true, 
    		monitorResize:false, 
            height:295,
            loadMask:true,  		            
            ds: store, 
            frame:false,
            collapsible: true,
            animCollapse: false,
            editable:true,
            clicksToEdit:1,
            plugins:[ispaCheckColumn,isceCheckColumn,isskCheckColumn],
            viewConfig: { 
		        //forceFit: true 
		    }, 
            listeners:{
	            rowclick:function(g,r){
	                this.controlButton(this.tabId);
	            },
	            scope:this
            },       
            sm: new Ext.grid.RowSelectionModel(),               
            cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Personal_Label_sfid,sortable: true, dataIndex: 'sfid'},
                {header:HRMSRes.Personal_Label_ntnm,sortable: true, dataIndex: 'stfn'},
                
                {header:HRMSRes.Training_Label_athr,sortable: true, dataIndex: 'athr',align:'right',type:'float',
                    editor:new Ext.form.NumberField({keepZero:true})},
                
                ispaCheckColumn,
                isceCheckColumn,
                isskCheckColumn,  

                {header:HRMSRes.Training_Label_scor,sortable: true, dataIndex: 'scor',
                    editor:new Ext.form.TextField({})
                },
                {header:HRMSRes.Public_Label_remk,sortable: true, dataIndex: 'remk',width:200,
                    editor:new Ext.form.TextField({})
                }
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
        for (var i =1; i<= st.getCount();i++ ){
            var x = st.getAt(i-1);
            var p = {trcd:this.basisForm.findField('trcd').getValue(),
                     emno:x.get('emno'),athr:x.get('athr'),
                     isce:x.get('isce'),ispa:x.get('ispa'),
                     issk:x.get('issk'),
                     scor:x.get('scor'),remk:x.get('remk'),
                     lmtm:x.get('lmtm'),lmur:x.get('lmur')};
           
            dtlparams[dtlparams.length] = p;
        }
        
        var keyparams=[];
        keyparams[0]={ColumnName:'trcd',ColumnValue:this.basisForm.findField('trcd').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + trtrarstPageName + '.mvc/'+method,
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		   		
		   		if (o.status=='success'){
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
    clear: function(){
		if(!this.basisForm.isValid()) return;
		this.grid.getBottomToolbar().diplayMsg.update('');
        
        var keyparams=[];
        keyparams[0]={ColumnName:'trcd',ColumnValue:this.basisForm.findField('trcd').getValue()};
        
        Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Clear,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
		            Ext.Ajax.request({
		               url: ContextInfo.contextPath+'/' + trtrarstPageName + '.mvc/clear',
		               success: function(response){
		   		            var o= Ext.util.JSON.decode(response.responseText);		   		
		   		            if (o.status=='success'){
		   		                Ext.MessageBox.show({
			                        title: HRMSRes.Public_Message_Info,
			                        msg:o.msg,
			                        buttons: Ext.MessageBox.OK,
			                        icon:Ext.MessageBox.INFO
		                        });
            		            
		                        this.Query();
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
		               params: {record:Ext.encode({keycolumns:keyparams})}
		            })
				}
	        },
	        icon:Ext.MessageBox.QUESTION,
	        scope:this
        });        
	},
	Query:function(){	 
        var params=[];	
        var f = this.basisForm.findField(trtrarstKeyColumn);     
        
        if (f.getValue()=='') return;
        
        //alert(f.getValue());  
        var p={
            ColumnName:f.getName(),
            ColumnValue:f.getValue()                
        };

        params[params.length]=p;                  

        var loadParams={start:0,limit:1000};//Pagination.pagingSize

        this.dtlgrid.queryParams={
            params:params
        };
        this.dtlgrid.store.baseParams={record:Ext.util.JSON.encode(this.dtlgrid.queryParams)};
	    this.dtlgrid.getStore().load({params:loadParams});
    },
    skilldtl:function(){
        var data=this.dtlgrid.getSelectionModel().getSelected();
        
        var emno = data.get('emno');
        var trcd = data.get('trcd');
        var stcd = data.get('stcd');
        new psskillEditWindow().show(emno,trcd,stcd);
    },
    applytoall:function(){
        
        if (!this.dtlgrid.getSelectionModel().hasSelection())
            this.grid.getBottomToolbar().diplayMsg.update('Select a row as source first.');
            
        var st = this.dtlgrid.getStore();
        var data=this.dtlgrid.getSelectionModel().getSelected();
        
        for (var i =1; i<= st.getCount();i++ ){
            st.getAt(i-1).set('athr',data.get('athr'));
            st.getAt(i-1).set('isce',data.get('isce'));
            st.getAt(i-1).set('ispa',data.get('ispa'));
            st.getAt(i-1).set('isat',data.get('isat'));
            st.getAt(i-1).set('issk',data.get('issk'));
            st.getAt(i-1).set('scor',data.get('scor'));
            st.getAt(i-1).set('remk',data.get('remk'));
        }
    },    
	controlButton:function(id){
        var enabled=!this.dtlgrid.getSelectionModel().hasSelection();
        //Ext.getCmp(id+ '_editdtl').setDisabled(enabled);
        //Ext.getCmp(id+ '_deletedtl').setDisabled(enabled);
        //Ext.getCmp(id+ '_updtl').setDisabled(enabled);
        //Ext.getCmp(id+ '_downdtl').setDisabled(enabled);
    }
});


var trtrarstPanel=function(){
    this.tabId=trtrarstConfig.tabId;
	this.init();	
	
	trtrarstPanel.superclass.constructor.call(this,{
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
	            hidden:trtrarstConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new trtrarstEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden:trtrarstConfig.auth[this.tabId+'_' + 'edit']!='True',
	            handler: function(){
	            	new trtrarstEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:trtrarstConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:trtrarstConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new trtrarstQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:trtrarstConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:trtrarstConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,trtrarstConfig.muf?'delete':'add',trtrarstConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(trtrarstPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var trtrarstStoreType=Ext.data.Record.create([
		    {name:'sfid'},
		    {name:'stfn'},
            {name:'trcd'},
            {name:'emno'},
            {name:'athr'},
            {name:'isce'},
            {name:'ispa'},
            {name:'isat'},
            {name:'issk'},
            {name:'scor'},
            {name:'remk'},
            {name:'lmur'},
            {name:'lmtm'},
            {name:'rfid'},
            {name:'trnm'}
        ]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},trtrarstStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:trtrarstKeyColumn,ColumnValue:trtrarstConfig.trcd}]})},   
	   		url:ContextInfo.contextPath+'/' + trtrarstPageName + '.mvc/list',
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
                {header:HRMSRes.Personal_Label_sfid,sortable: true, dataIndex: 'sfid'},
                {header:HRMSRes.Personal_Label_ntnm,sortable: true, dataIndex: 'stfn'},
                {header:HRMSRes.Personal_Label_trcd,sortable: true, dataIndex: 'trcd'},
                {header:HRMSRes.Training_Label_trnm,sortable: true, dataIndex: 'trnm'},
                {header:HRMSRes.Training_Label_ispa,sortable: true, dataIndex: 'ispa'},
                {header:HRMSRes.Training_Label_isce,sortable: true, dataIndex: 'isce'},
                {header:HRMSRes.Training_Label_issk,sortable: true, dataIndex: 'issk'},
                {header:HRMSRes.Training_Label_scor,sortable: true, dataIndex: 'scor'},
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
        keyparams[0]={ColumnName:'trcd',ColumnValue:record.get('trcd')};
        keyparams[1]={ColumnName:'emno',ColumnValue:record.get('emno')};
        
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ trtrarstPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:trtrarstKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + trtrarstPageName + '.mvc/exportexcel';
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
    new trtrarstPanel();
})

    </script>

</body>
</html>