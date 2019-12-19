<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="rpexrpdf.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.rpexrpdf.rpexrpdf" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var rpexrpdfConfig=Ext.decode('<%=ViewData["config"] %>'); 
rpexrpdfConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var rpexrpdfPageName = 'rpexrpdf';
var rpexrpdfKeyColumn='rpcd';
            
var rpexrpdfQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	rpexrpdfQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(rpexrpdfQueryWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Report_Label_rpcd,
                        name: 'rpcd',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Report_Label_rpnm,
                        name: 'rpnm',stateful:false,anchor:'95%'}]},

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


var rpexrpdfEditWindow=function(grid,isNew){
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
        	                id:this.tabId+'_exportexceldtl',
                            iconCls:'icon-export', 
                            text:HRMSRes.Public_Toolbar_ToExcel, 
                            //hidden:roleConfig.auth[this.tabId+'_rolemgt_exportexcel']!='True',
                            handler: this.exportExcelLine, 
                            scope: this 
                        },'->',{ 
	        	            id:this.tabId+'_updtl',
	                        iconCls:'icon-up', 
	                        text:HRMSRes.Public_Toolbar_Up, 
	                        handler: this.upline, 
	                        disabled:true,
	                        scope: this 
	                    },'-',{ 
	        	            id:this.tabId+'_downdtl',
	                        iconCls:'icon-down', 
	                        text:HRMSRes.Public_Toolbar_Down, 
	                        handler: this.downline, 
	                        disabled:true,
	                        scope: this 
	                    }],
		            items:this.dtlgrid
	            }]
            };
                       
	rpexrpdfEditWindow.superclass.constructor.call(this,{
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
		        var keyField = this.basisForm.findField(rpexrpdfKeyColumn);
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

                setLastModifiedInfo(rpexrpdfConfig,this.basisForm);
                
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

Ext.extend(rpexrpdfEditWindow,Ext.Window,{
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
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Report_Label_rpcd + '(<font color=red>*</font>)',
                        name: 'rpcd',allowBlank:false,stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Report_Label_rpnm + '(<font color=red>*</font>)',
                        name: 'rpnm',allowBlank:false,stateful:false,anchor:'95%'}]},

      		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Report_Label_rptt,
                        name: 'rptt',allowBlank:true,stateful:false,anchor:'97.5%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Report_Label_rpty  + '(<font color=red>*</font>)',
                        name: 'rpty',id:this.tabId + "_rpty",stateful:false,editable:false,typeAhead: true,value:'Personnel',
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: ExcelReportTypeStatStore,
                        listeners:{
                            select:function(p){
                                var f1=Ext.getCmp(this.tabId + '_itnm');
                                if (f1!=null){
                                    f1.store.baseParams={record:p.getValue()};
                                    f1.store.reload();
                                }
                            },
                            scope:this
                        }
                    }]},

       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Public_Label_remk,height:50,
                        name: 'remk',stateful:false,anchor:'97.5%'}]},

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
        var isorCheckColumn = new Ext.grid.CheckColumn({
               header: "Sort?",
               dataIndex: 'isor',
               width: 55
            });

        var isqrCheckColumn = new Ext.grid.CheckColumn({
               header: "Query?",
               dataIndex: 'isqr',
               width: 55
            });

        var ismeCheckColumn = new Ext.grid.CheckColumn({
               header: "Merge?",
               dataIndex: 'isme',
               width: 55
            });

        var issuCheckColumn = new Ext.grid.CheckColumn({
               header: "Sum?",
               dataIndex: 'issu',
               width: 55
            });
            
        //get def based on report type    
        var method = 'getDefForExcelReport';    
//        var f = this.basisForm.findField('rpty');
//        if (f!=null)
//        {
//            var v = f.getValue();
//            if (v=="Payroll")
//                method = "getDefForPayrollExcelReport";
//        }
//        alert(method);            

		var rpexrpdfDtlStoreType=Ext.data.Record.create([
            {name:'rpcd'},
            {name:'sqno'},
            {name:'itnm'},
            {name:'itfd'},
            {name:'itrs'},
            {name:'tbnm'},
            {name:'itty'},
            {name:'isor'},
            {name:'isqr'},
            {name:'isme'},
            {name:'issu'},
            {name:'lmtm'},
            {name:'lmur'},
            {name:'ittx'}
		]);
        
		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},rpexrpdfDtlStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:rpexrpdfKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + rpexrpdfPageName + '.mvc/getReportDtl',
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
            id:'reportDtlGrid',
    		border:true, 
    		monitorResize:false, 
            height:230,
            loadMask:true,  		            
            ds: store, 
            frame:false,
            collapsible: true,
            animCollapse: false,
            editable:true,
            clicksToEdit:1,
            plugins:[isorCheckColumn,isqrCheckColumn,ismeCheckColumn,issuCheckColumn],
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
            {header:'sqno',sortable: true, dataIndex: 'sqno',width: 50},
            
            {header: HRMSRes.Report_Label_itnm,dataIndex: 'itnm',width: 130,
                editor: new Ext.form.ComboBox({typeAhead: true,triggerAction: 'all',mode:'local',maxHeight:200,
                listClass: 'x-combo-list-small',id:this.tabId + '_itnm',
                displayField: 'DisplayField',valueField:'DisplayField',
                store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
		                    fields: ['ValueField','DisplayField','MiscField1','MiscField2','MiscField3']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/'+ method,
		                    baseParams:{record:'Personal'}}),
		        listeners:{
		            select:function(c,r,i){var sm = this.dtlgrid.getSelectionModel();var rec= sm.getSelected();
		                    rec.set('itfd',r.data['MiscField2']);rec.set('tbnm',r.data['MiscField1']);
		                    rec.set('itty',r.data['MiscField3']);},scope:this}
            })},
		     
		    {header: HRMSRes.Report_Label_ittx,dataIndex: 'ittx',width: 130,
		      editor: new Ext.form.TextField({})}, 
		      
            isorCheckColumn,
            isqrCheckColumn,
            ismeCheckColumn,
            issuCheckColumn,                  
            
            //{header:HRMSRes.Public_Label_lmtm,sortable: true, dataIndex: 'lmtm',renderer:formatDate},
            //{header:HRMSRes.Public_Label_lmur,sortable: true, dataIndex: 'lmur'},
            {header:'itfd',sortable: true, dataIndex: 'itfd',hidden:true},
            {header:'tbnm',sortable: true, dataIndex: 'tbnm',hidden:true},
            {header:'itrs',sortable: true, dataIndex: 'itrs',hidden:true},
            {header:'itty',sortable: true, dataIndex: 'itty',hidden:true}
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
            var p = {rpcd:this.basisForm.findField('rpcd').getValue(),
                     sqno:x.get('sqno'),itnm:x.get('itnm'),
                     itfd:x.get('itfd'),itrs:x.get('itrs'),
                     tbnm:x.get('tbnm'),itty:x.get('itty'),
                     isor:x.get('isor'),isqr:x.get('isqr'),
                     isme:x.get('isme'),issu:x.get('issu'),
                     ittx:x.get('ittx'),
                     lmtm:this.basisForm.findField('lmtm').getValue(),
                     lmur:this.basisForm.findField('lmur').getValue()};

            
            dtlparams[dtlparams.length] = p;
        }
        
        var keyparams=[];
        keyparams[0]={ColumnName:'rpcd',ColumnValue:this.basisForm.findField('rpcd').getValue()};
        //keyparams[1]={ColumnName:'dasq',ColumnValue:this.basisForm.findField('dasq').getValue()};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + rpexrpdfPageName + '.mvc/'+method,
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
        var f = this.basisForm.findField(rpexrpdfKeyColumn);        
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
        params['itnm']='';
        params['itfd']='';
        params['itrs']='';
        params['tbnm']='';
        params['itty']='';
        params['ittx']='';
        params['isor']='N';
        params['isqr']='N';
        params['isme']='N';
        params['issu']='N';

        var store = this.dtlgrid.getStore();
        store.add(new store.recordType(params));
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
	upline:function(){
	    var sm=this.dtlgrid.getSelectionModel();
	    var record=sm.getSelected();
        var idx = this.dtlgrid.store.indexOf(record);
        
        if (idx>0) {
            this.dtlgrid.store.remove(record);        
            this.dtlgrid.store.insert(idx-1,record);
            sm.selectRange(idx-1,idx-1);
        }else{
            sm.selectRange(idx,idx);
        }
        
        this.resortdasq();
        this.dtlgrid.store.commitChanges();
        
        this.controlButton(this.tabId);
	},
    downline:function(){
	    var sm=this.dtlgrid.getSelectionModel();
	    var record=sm.getSelected();
        var idx = this.dtlgrid.store.indexOf(record);
        
        if (idx<this.dtlgrid.store.totalLength-1) {
            this.dtlgrid.store.remove(record);        
            this.dtlgrid.store.insert(idx+1,record);
            sm.selectRange(idx+1,idx+1);
        }else{
            sm.selectRange(idx,idx);
        }
        
        this.resortdasq();
        this.dtlgrid.store.commitChanges();
        
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
	    var params={record:Ext.encode({params:[{ColumnName:rpexrpdfKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.dtlgrid.queryParams){
            this.dtlgrid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.dtlgrid.queryParams);
            delete this.dtlgrid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + rpexrpdfPageName + '.mvc/exportexceldtl';
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
        Ext.getCmp(id+ '_deletedtl').setDisabled(enabled);
        Ext.getCmp(id+ '_updtl').setDisabled(enabled);
        Ext.getCmp(id+ '_downdtl').setDisabled(enabled);
    },
    resortdasq:function(){
        var st = this.dtlgrid.getStore();
        for (var i =1; i<= st.getCount();i++ ){
            st.getAt(i-1).set('sqno',i.toString());
        }
    }    
});


var rpexrpdfPanel=function(){
    this.tabId=rpexrpdfConfig.tabId;
	this.init();	
	
	rpexrpdfPanel.superclass.constructor.call(this,{
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
	            hidden:rpexrpdfConfig.auth[this.tabId+'_' + 'add']!='True',
	            handler: function(){
	                new rpexrpdfEditWindow(this.grid,true).show();            	
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden:rpexrpdfConfig.auth[this.tabId+'_' + 'edit']!='True',
	            handler: function(){
	            	new rpexrpdfEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },{ 
	            id:this.tabId+ '_delete',
                iconCls:'icon-remove', 
                hidden:rpexrpdfConfig.auth[this.tabId+'_' + 'delete']!='True',
                text:HRMSRes.Public_Toolbar_Delete, 
                handler: this.remove, 
                scope: this 
            },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:rpexrpdfConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new rpexrpdfQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:rpexrpdfConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:rpexrpdfConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,rpexrpdfConfig.muf?'delete':'add',rpexrpdfConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(rpexrpdfPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},

	createGridPanel:function(){

		var rpexrpdfStoreType=Ext.data.Record.create([
            {name:'rpcd'},
            {name:'rpnm'},
            {name:'rptt'},
            {name:'rpty'},
            {name:'remk'},
            {name:'lmtm'},
            {name:'lmur'},
            {name:'rfid'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},rpexrpdfStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:rpexrpdfKeyColumn,ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/' + rpexrpdfPageName + '.mvc/list',
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
                {header:HRMSRes.Report_Label_rpcd,sortable: true, dataIndex: 'rpcd',hidden:false},
                {header:HRMSRes.Report_Label_rpnm,sortable: true, dataIndex: 'rpnm',hidden:false},
                {header:HRMSRes.Report_Label_rptt,sortable: true, dataIndex: 'rptt',hidden:false},
                {header:HRMSRes.Report_Label_rpty,sortable: true, dataIndex: 'rpty'},
                {header:HRMSRes.Public_Label_remk,sortable: true, dataIndex: 'remk'},
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
        keyparams[0]={ColumnName:'clcd',ColumnValue:record.get('rpcd')};
        
	    Ext.MessageBox.show({
	        title:HRMSRes.Public_Confirm_Title,
	        msg:HRMSRes.Public_Confirm_Delete,
	        buttons: Ext.Msg.YESNO,
	        fn: function(btn, text){
		        if (btn=='yes'){
			        Ext.Ajax.request({
	   			        url:ContextInfo.contextPath+ '/'+ rpexrpdfPageName + '.mvc/delete',
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
	    var params={record:Ext.encode({params:[{ColumnName:rpexrpdfKeyColumn,ColumnValue:''}],headers:header})};
	    
	    if(this.grid.queryParams){
            this.grid.queryParams['headers']=header;
            delete params.record;
            params.record=Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }
	    
	    var form=document.createElement('form');
	    form.name='excelForm';
	    form.method='post';
	    form.action=ContextInfo.contextPath+ '/' + rpexrpdfPageName + '.mvc/exportexcel';
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
    new rpexrpdfPanel();
})

    </script>

</body>
</html>