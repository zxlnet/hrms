<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MasterData.aspx.cs" Inherits="GotWell.HRMSWeb.Views.MasterData.MasterData" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<body>   
    <script type="text/javascript" charset="utf-8">  
        /*
        *
        *new or edit window
        *
        */
         var masterDataCommonWindow=function(isNew,grid,tabId){
	        this.isNew=isNew;
            this.tabId=tabId;
	        this.grid=grid;
	        this.store=this.grid.getStore();
	        this.record;
	        this.height=250;
	        this.cm=this.grid.getColumnModel();
	        this.init();	
        	
	        masterDataCommonWindow.superclass.constructor.call(this,{
                layout:'fit', 
                width:620, 
                id:this.tabId+'_commonWindow',
                height:this.height, 
                closeAction:'close', 
                modal: true, 
                resizable: false, 
                buttonAlign: 'center', 
                items: this.formPanel,
                buttons: [{ 
                    text:HRMSRes.Public_Button_Save, 
                    iconCls:'icon-save', 
                    handler: this.save,
                    scope: this
                },{ 
                    text: HRMSRes.Public_Button_Close, 
                    iconCls: 'icon-exit', 
                    handler: function(){ this.close();},
                    scope:this
                }],
                listeners:{
	    	        show:function(){
	    		        if(!this.isNew){	
	    			        this.record=this.grid.getSelectionModel().getSelected(); 
	    			        var data=this.record;      
                            this.form.items.each(function(f){
                                if(f.isFormField){
                                    var value=data.get(f.getName());
                                    if(f.getValue()=='' || f.getName()=='createdby' || f.getName()=='createddate'){
                                        if(f.xtype=='numberfield'){
                                            if(value.toString().indexOf(',')!=-1){
                                               value=parseFloat(value.toString().replace(',','')); 
                                            }                                            
                                        }
                                        f.setValue(value);
                                    }	                    	            
                                }
                            });   
	    			        this.disablePrimaryKey(true);
	    		        }else{
	    			        this.disablePrimaryKey(false);
	    		        }
	    	        },
	    	        scope:this
	            }
	        })
        }

        Ext.extend(masterDataCommonWindow, Ext.Window, {
            init: function() {
                this.formPanel = this.createFormPanel();
                this.form = this.formPanel.getForm();
            },

            createFormPanel: function() {
                return new Ext.FormPanel({
                    frame: true,
                    labelWidth: 120,
                    header: true,
                    items: {
                        layout: 'column',
                        items: this.createItems()
                    }
                })
            },

            createItems: function() {
                var cm = this.cm;
                var firstColumn = [];
                var secondColumn = [];

                for (var i = 0; i < cm.config.length; i++) {
                    var config = cm.config[i];

                    var name = config.dataIndex;
                    var labelValue = config.header;
                    var type = config.type;
                    var size = config.size;
                    var isRequired = config.required;
                    var precision = config.precision;
                    var defaultValue = config.defaultValue;
                    var defaultValueFormula = config.defaultValueFormula;
                    var isDisplay = config.isDisplay;
                    var isPk = config.isPk;
                    var controltype = config.controlType;

                    var fieldConfig = {
                        fieldLabel: labelValue,
                        type: type,
                        stateful: false,
                        name: name,
                        disabled: isDisplay == 'True',
                        isPk: isPk,
                        height: 22,
                        anchor: '95%'
                    };


                    if (defaultValue != '') {
                        fieldConfig.value = defaultValue;
                    }

                    if (size != 0) {
                        fieldConfig['maxLength'] = size;
                    }

                    if (typeof isRequired != 'undefined') {
                        if (isRequired == 'True') {
                            fieldConfig['fieldLabel'] = labelValue + '(<font color=red>*</font>)';
                            fieldConfig['allowBlank'] = false;
                        } else {
                            fieldConfig['allowBlank'] = true;
                        }
                    }
                    if (typeof type != 'undefinded') {
                        if (type == 'datetime') {
                            switch (defaultValueFormula.toUpperCase()) {
                                case "DATE TIME":
                                case "SYSDATE":
                                    fieldConfig['format'] = DATE_FORMAT.DATETIME;
                                    break;
                                case "DATE ONLY":
                                    fieldConfig['format'] = DATE_FORMAT.DATEONLY;
                                    break;
                            }

                            delete fieldConfig.maxLength;
                            field = new Ext.form.DateField(fieldConfig);
                        } else if (type == 'int') {
                            fieldConfig['allowDecimals'] = false;
                            field = new Ext.form.NumberField(fieldConfig);
                        } else if (type == 'float') {
                            fieldConfig['xtype'] = 'numberfield';
                            fieldConfig['allowDecimals'] = true;
                            if (typeof precision != 'undefined') {
                                fieldConfig['decimalPrecision'] = precision;
                            }
                            field = new Ext.form.NumberField(fieldConfig);
                        } else {
                            if (controltype == 'combo') {
                                var comboName = fieldConfig['name'];
                                fieldConfig['triggerAction'] = 'all';
                                fieldConfig['mode'] = 'local';
                                fieldConfig['value'] = '';
                                fieldConfig['typeAhead'] = true;
                                var store = new Ext.data.Store({
                                    reader: new Ext.data.JsonReader({
                                        totalProperty: "results",
                                        root: "rows",
                                        fields: ['ValueField', 'DisplayField']
                                    }),
                                    autoLoad: true,
                                    baseParams: {},
                                    url: ContextInfo.contextPath + '/dropdown.mvc/' + defaultValue,
                                    listeners: { load: function() { f = this.form.findField(comboName); f.setValue(f.getValue()); }, scope: this }
                                });

                                fieldConfig['store'] = store;
                                fieldConfig['displayField'] = 'DisplayField';
                                fieldConfig['valueField'] = 'ValueField';
                                field = new Ext.form.ComboBox(fieldConfig);
                            }
                            else {
                                if (typeof size != 'undefined' && size >= 100) {
                                    fieldConfig.height = 70;
                                    field = new Ext.form.TextArea(fieldConfig);
                                } else {
                                    fieldConfig['UpperOnBlur'] = false;
                                    field = new Ext.form.TextField(fieldConfig);
                                }
                            }
                        }
                    }

                    if (i % 2 == 0) {
                        firstColumn[firstColumn.length] = field;
                        this.height += 25;
                    } else {
                        secondColumn[secondColumn.length] = field;
                    }
                }

                var first = {
                    columnWidth: .5,
                    layout: 'form',
                    items: firstColumn
                };

                var second = {
                    columnWidth: .5,
                    layout: 'form',
                    items: secondColumn
                };

                return [first, second];
            },

            disablePrimaryKey: function(disabled) {
                var pks = this.cm.primaryKey;
                for (var i = 0; i < pks.length; i++) {
                    var pk = pks[i];
                    var field = this.form.findField(pk.ColumnName);
                    if (field) {
                        field.setDisabled(disabled);
                    }
                }
            },

            isDflag: function() {
                var cm = this.grid.getColumnModel();
                for (var i = 0; i < cm.config.length; i++) {
                    var o = cm.config[i];
                    if (o.dataIndex === 'dflag') {
                        return true;
                    }
                }
                return false;
            },

            save: function() {
                if (!this.form.isValid()) return;
                var url = ContextInfo.contextPath + '/masterdata.mvc/';
                url += this.isNew ? 'create' : 'edit';
                var params = [];
                var param = {};
                var record = this.record;

                this.form.items.each(function(f) {
                    if (f.isFormField) {
                        var p = {
                            ColumnType: f.type,
                            ColumnName: f.getName(),
                            ColumnValue: f.getValue(),
                            IsPrimaryKey: f.isPk
                        };
                        params[params.length] = p;
                        if (f.type == 'datetime') {
                            param[f.getName()] = Ext.util.Format.date(f.getValue(), f.format);
                        } else {
                            param[f.getName()] = f.getValue();
                        }
                    }
                });

                if (record) {
                    param['rw'] = record.get('rw');
                    params[params.length] = {
                        ColumnType: 'string',
                        ColumnName: 'rowid',
                        ColumnValue: record.get('rw')
                    };
                }

                Ext.Ajax.request({
                    url: url,
                    success: function(response) {
                        var o = Ext.util.JSON.decode(response.responseText);
                        var msg = o.msg;
                        if (o.status == 'success') {
                            if (this.isNew) {
                                var ps = {
                                    start: 0,
                                    limit: Pagination.pagingSize
                                };
                                ps.tableName = this.grid.tableName;

                                if (this.isDflag()) {
                                    ps.record = Ext.encode([{
                                        ColumnName: 'dflag',
                                        ColumnType: 'string',
                                        ColumnValue: 'N'}]);
                                    }

                                    if (this.store.baseParams.record) {
                                        delete this.store.baseParams.record;
                                    }
                                    this.store.load({ params: ps });
                                } else {
                                    var index = this.store.indexOf(this.record);
                                    this.store.remove(this.record);
                                    this.record = new this.store.type(param);
                                    this.store.insert(index, this.record);
                                    this.grid.getSelectionModel().selectRow(index);
                                }
                                this.close();
                            } else {
                                if (o.code && o.code == 'IsDuplicate') {
                                    var pks = this.cm.primaryKey;
                                    var pk = '';
                                    for (var i = 0; i < pks.length; i++) {
                                        pk = pk + pks[i].ColumnDisplayName;
                                        if (i < pks.length - 1) {
                                            pk += ',';
                                        }
                                    }
                                    msg = msg + '(' + pk + HRMSRes.Public_Message_IsDuplicate + ')';
                                }
                                Ext.MessageBox.show({
                                    title: HRMSRes.Public_Message_Error,
                                    msg: msg,
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                            }
                            this.grid.getBottomToolbar().diplayMsg.update(msg);
                        },
                        failure: function(response) {
                            Ext.MessageBox.show({
                                title: HRMSRes.Public_Message_Error,
                                msg: response.responseText,
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                        },
                        scope: this,
                        params: { record: Ext.encode(params), tableName: this.grid.tableName }
                    });

                },
                scope: this
            });


        /*
        *
        *query window
        *
        */

         var masterDataCommonQueryWindow=function(grid,tabId){
	        this.grid=grid;
	        this.store=this.grid.getStore();
	        this.record;
	        this.tabId=tabId;
	        this.height=250;
	        this.cm=this.grid.getColumnModel();
	        this.init();
        	
	        masterDataCommonQueryWindow.superclass.constructor.call(this,{
		        title: HRMSRes.Public_Query_WindowTitle, 
                layout:'fit', 
                width:620, 
                id:this.tabId+'_CommonQueryWindow',
                height:this.height, 
                closeAction:'close', 
                modal: true, 
                resizable: false, 
                buttonAlign: 'center', 
                items: this.formPanel,
                buttons: [{ 
                    text:HRMSRes.Public_Toolbar_Query, 
                    iconCls:'icon-query', 
                    handler: this.query,
                    scope: this
                },{ 
                    text: HRMSRes.Public_Button_Close, 
                    iconCls: 'icon-exit', 
                    handler: function(){ this.close();},
                    scope:this
                }],
                listeners:{
	    	        show:function(){
	    	            var queryParams=this.grid.queryParams;
	    		        if(queryParams){
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
	    	        },
	    	        scope:this
	            }
	        })
        }

        Ext.extend(masterDataCommonQueryWindow, Ext.Window, {
            init: function() {
                this.formPanel = this.createFormPanel();
                this.form = this.formPanel.getForm();
            },

            createFormPanel: function() {
                return new Ext.FormPanel({
                    frame: true,
                    header: true,
                    labelWidth: 120,
                    items: {
                        layout: 'column',
                        items: this.createItems()
                    }
                })
            },

            clone: function(obj) {
                var o = {};
                for (var p in obj) {
                    o[p] = obj[p];
                }
                return o;
            },

            createItems: function() {
                var cm = this.cm;
                var firstColumn = [];
                var secondColumn = [];
                var queryColumn = [];

                for (var i = 0; i < cm.config.length; i++) {
                    var config = cm.config[i];
                    var type = config.type;
                    var size = config.size;
                    var obj = this.clone(config);

                    if (type == 'datetime') {
                        queryColumn[queryColumn.length] = obj;
                        var to = this.clone(obj);
                        obj['header'] += '<br/>from';
                        obj['dataIndex'] = 'from#' + obj['dataIndex'];
                        to['dataIndex'] = 'to#' + to['dataIndex'];
                        to['header'] += '<br/>to';
                        queryColumn[queryColumn.length] = to;
                    } else if (type == 'string' && size < 100) {
                        queryColumn[queryColumn.length] = obj;
                    }
                }

                for (var i = 0; i < queryColumn.length; i++) {
                    var config = queryColumn[i];
                    var name = config.dataIndex;
                    var labelValue = config.header;
                    var type = config.type;
                    var defaultValue = config.defaultValue;
                    var defaultValueFormula = config.defaultValueFormula;
                    var field;
                    var fieldConfig = {
                        fieldLabel: labelValue,
                        type: type,
                        name: name,
                        stateful: false,
                        height: 22,
                        anchor: '95%'
                    };
                    if (typeof type != 'undefinded') {
                        if (type == 'datetime') {
                            switch (defaultValueFormula.toUpperCase()) {
                                case "":
                                case "DATE TIME":
                                case "SYSDATE":
                                    fieldConfig['format'] = DATE_FORMAT.DATETIME;
                                    break;
                                case "DATE ONLY":
                                    fieldConfig['format'] = DATE_FORMAT.DATEONLY;
                                    break;
                            }
                            field = new Ext.form.DateField(fieldConfig);
                        } else if (type == 'string') {
                            fieldConfig['UpperOnBlur'] = false;
                            field = new Ext.form.TextField(fieldConfig);
                        }
                    }

                    if (i % 2 == 0) {
                        firstColumn[firstColumn.length] = field;
                        this.height += 25;
                    } else {
                        secondColumn[secondColumn.length] = field;
                    }
                }

                var first = {
                    columnWidth: .5,
                    layout: 'form',
                    items: firstColumn
                };

                var second = {
                    columnWidth: .5,
                    layout: 'form',
                    items: secondColumn
                };

                return [first, second];
            },

            query: function() {
                var params = [];
                var loadParams = { start: 0, tableName: this.grid.tableName, limit: Pagination.pagingSize };

                this.form.items.each(function(f) {
                    if (f.isFormField) {
                        if (f.getValue()) {
                            params[params.length] = {
                                ColumnType: f.type,
                                ColumnName: f.getName(),
                                ColumnValue: f.getValue()
                            };
                        }
                    }
                });

                if (params.length > 0) {
                    this.store.baseParams.record = Ext.encode(params); ;
                } else {
                    if (this.store.baseParams.record) {
                        delete this.store.baseParams.record;
                    }
                }

                this.grid.queryParams = params;
                this.store.load({ params: loadParams });
                this.close();
            },
            scope: this
        });


        var Maintain=function(config){
            this.config=config;
            this.tabId=config.tabId;
            this.init();    
        }

        Ext.extend(Maintain,Ext.util.Observable,{
            init:function(){
                this.createGridPanel();
                this.createPanel();
                var cm=this.grid.getColumnModel();
		        cm.primaryKey=this.getPrimaryKey();
		        this.grid.tableName=this.config.tableName;
            },
            
            createPanel:function(){
                this.panel=new Ext.Panel({
                    applyTo:this.tabId,
                    id:this.tabId+'_panel',
                    layout:'fit',
                    items:{
                        xtype:'panel',
	    	            layout:'fit',
	    	            monitorResize:true,
	    	            autoScroll:true,
	    	            tbar: [{ 
	        	            id:this.tabId+'_masterdata_add',
	                        iconCls:'icon-add', 
	                        text: HRMSRes.Public_Toolbar_Add, 
	                        hidden:this.config.auth[this.tabId+'_masterdata_add']!='True',
	                        disabled:this.config.locked=='True',
	                        handler: function(){
	            	            var commonWin=new masterDataCommonWindow(true,this.grid,this.tabId);;
	            	            commonWin.setTitle(HRMSRes.Public_Add_WindowTitle);
	            	            commonWin.show();	            	
	                        }, 
	                        scope: this 
	                    },{ 
	        	            id:this.tabId+'_masterdata_edit',
	                        iconCls:'icon-update',
	                        hidden:this.config.auth[this.tabId+'_masterdata_edit']!='True', 
	                        disabled:this.config.locked=='True',
	                        text:HRMSRes.Public_Toolbar_Edit,
	                        handler: this.edit, 
	                        scope: this 
	                    },{ 
	        	            id:this.tabId+'_masterdata_delete',
	                        iconCls:'icon-remove', 
	                        hidden:this.config.auth[this.tabId+'_masterdata_delete']!='True',
	                        disabled:this.config.locked=='True',
	                        text:HRMSRes.Public_Toolbar_Delete, 
	                        handler: this.remove, 
	                        scope: this 
	                    },{ 
	        	            id:this.tabId+'_masterdata_query',
	                        iconCls:'icon-query', 
	                        hidden:this.config.auth[this.tabId+'_masterdata_query']!='True',
	                        text:HRMSRes.Public_Toolbar_Query,
	                        handler: function(){
	            	            var queryWin=new masterDataCommonQueryWindow(this.grid,this.tabId);
	            	            queryWin.show();
	                        }, 
	                        scope: this
	                    },{ 
	        	            id:this.tabId+'_masterdata_export',
	                        iconCls:'icon-export', 
	                        hidden:this.config.auth[this.tabId+'_masterdata_export']!='True',
	                        text:HRMSRes.Public_Toolbar_OutExcel, 
	                        handler: this.exportExcel, 
	                        scope: this 
	                    },{ 
	        	            id:this.tabId+'_masterdata_import',
	        	            hidden:this.config.auth[this.tabId+'_masterdata_import']!='True',
	        	            disabled:this.config.locked=='True',
	                        iconCls:'icon-import', 
	                        text:HRMSRes.Public_Toolbar_InExcel, 
	                        handler: this.importExcel, 
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
	                        iconCls:this.config.muf=='True'?'icon-mufdelete':'icon-mufadd', 
	                        text:'', 
	                        handler: function(){updateMUF(this.tabId,this.config.muf?'delete':'add',this.config,this.grid);},
	                        scope: this 
	                    }],
	                    items:this.grid
                    }
                });
            },
            
            createGridPanel:function(){     
                for(var prop in this.config){
		            var value=this.config[prop];
		            if(typeof value=='undefined'){
		                return ;
		            }
		        }     

        		 
		        var storeType=Ext.data.Record.create(this.config.records);
        		
		        this.remoteStore=new Ext.data.Store({ 
        	        reader: new Ext.data.JsonReader({
	    		        totalProperty: "results",
	    		        root: "rows"               
	   	 	        },storeType), 		    
	   		        url:ContextInfo.contextPath+'/masterdata.mvc/list',
	   		        baseParams:{tableName:this.config.tableName},
	   		        listeners:
                    {
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
                       
                this.remoteStore.type=storeType;
                var params={
                    start:0,
                    limit:Pagination.pagingSize
                };
                if(this.isDflag()){
                    params.record=Ext.encode([{
                        ColumnName:'dflag',
                        ColumnType:'string',
                        ColumnValue:'N'
                    }]);
                }
                
                this.remoteStore.load({params:params});
                
                this.grid=new Ext.grid.GridPanel({
                    border:true, 
    		        monitorResize:true,
    		        id:this.tabId+'_GridPanel', 
                    loadMask:true,  
                    layout:'fit',		            
                    ds: this.remoteStore, 
                    viewConfig: { 
		                forceFit: true 
		            },                          
                    cm: new Ext.grid.ColumnModel(this.config.columns),      
                    bbar: new Ext.PagingToolbar({
                        pageSize:Pagination.pagingSize,
                        store: this.remoteStore,
                        displayInfo: true,
                        displayMsg:HRMSRes.Public_PagingToolbar_Total+':{1}/{2}',
                        emptyMsg: HRMSRes.Public_PagingToolbar_EmptyMsg                    
                    }),
                    listeners:{
        	            render : function(p_grid){
				            this.controlButton(this.tabId);
				            if(this.config.locked=='True'){
				                this.grid.getBottomToolbar().diplayMsg.update('<font color=red>'+HRMSRes.Public_Message_StepLocked+'</font>');
				            }
			            },
			            rowclick:function(){
	                        this.controlButton(this.tabId);
	                    },
			            scope:this
                    }
	            });
            },
            
            edit:function(){
                var sm=this.grid.getSelectionModel();
	            var rs=sm.getSelections();
	            var cm=this.grid.getColumnModel();
        	    
                for(var i=0;i<rs.length;i++){
	                var r=rs[i];
	                for(var j=0;j<cm.config.length;j++){
	                    var o=cm.config[j];
                        if(o.dataIndex==='dflag'){	
                            if(r.get(o.dataIndex)=='Y'){
                                Ext.MessageBox.show({
	   					            title:HRMSRes.Public_Message_Error,
	   					            msg:HRMSRes.MasterData_Message_EditDuplicate,
	   					            buttons: Ext.MessageBox.OK,
	   					            icon:Ext.MessageBox.ERROR
	   				            });
	   				            return;
                            }
                        }
	                }
	            }
        	    
                var commonWin=new masterDataCommonWindow(false,this.grid,this.tabId);;
                commonWin.setTitle(HRMSRes.Public_Edit_WindowTitle);
                commonWin.show();  	
            },
            
            exportExcel:function(){
	            var params={};
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
        	    
	            params.header=Ext.encode(header);	    
	            params.tableName=this.config.tableName;
        	    	    
	            if(this.grid.queryParams){
	                params.record=Ext.encode(this.grid.queryParams);
	            }else{
	                if(this.isDflag()){
                        params.record=Ext.encode([{
                            ColumnName:'dflag',
                            ColumnType:'string',
                            ColumnValue:'N'
                        }]);
                    }
	            }
                 
	            var form=document.createElement('form');
	            form.name='excelForm';
	            form.method='post';
	            form.action=ContextInfo.contextPath+ '/masterdata.mvc/exportExcel';
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
        	
        	
	        importExcel:function(){
	            var params=[];
	            var cm=this.grid.getColumnModel();
	            for(var i=0;i<cm.config.length;i++){
	                var o=cm.config[i];	            
	                params[params.length]={ColumnName:o.dataIndex,ColumnType:o.type,ColumnValue:o.defaultValue};        
	            }
        	    
	            var formPanel=new Ext.FormPanel({
                    labelWidth: 20,
                    frame:true,                
                    header: true,  
                    formId:this.tabId+'_importForm',              
                    bodyStyle:'padding:10px 5px 0',                
                    items:[{
                        xtype:'label',
                        text:HRMSRes.Public_Toolbar_ImportExcelLabel
                    },{
                        xtype:'textfield',
                        name:'fileImport',
                        labelSeparator:'',
                        stateful:false,
                        inputType:'file',                 
                        style:'margin-top:10px;align:left',
                        allowBlank:false,
                        width:330,
                        height:30
                    }]
                });
        	    
                var win=new Ext.Window({
                    title:HRMSRes.Public_Toolbar_InExcel,
                    layout:'fit', 
                    width:400, 
                    height:180, 
                    id:this.tabId+'_importExcelWindow',
                    closeAction:'close', 
                    modal: true, 
                    resizable: false,
                    buttonAlign: 'center',  
                    buttons:[{
                        text:HRMSRes.Public_Button_Confirm,
                        handler:function(){
                            var form=formPanel.getForm();
                            if(!form.isValid()) return ;
                            var field=form.findField('fileImport');
                            if(field){
                                var fileName=field.getValue();
                                fileName=fileName.substr(fileName.length-4,fileName.length);
                                if(fileName.toLowerCase()!='.xls'){  
                                    Ext.MessageBox.show({
		   					            title:HRMSRes.Public_Message_Error,
		   					            msg:HRMSRes.Public_Message_Excel,
		   					            buttons: Ext.MessageBox.OK,
		   					            icon:Ext.MessageBox.ERROR
		   				            });                        
                                    return;
                                }
                            }
                            
                            var uploadForm = new Ext.BasicForm(    
                                form.getEl(),
                                {    
                                      fileUpload: true,    
                                      url: ContextInfo.contextPath+ '/masterdata.mvc/importExcel'                
                                }    
                            );
                            var grid=this.grid;
                            uploadForm.submit({    
                                waitMsg:HRMSRes.Public_Message_Wating,    
                                waitTitle:HRMSRes.Public_Title_Upload, 
                                params:{tableName:this.config.tableName,cols:Ext.encode(params)}, 
                                reset: false,
                                success: function(form,action){                            
                                    win.close();                                  	        	   					        		   					
	   				                grid.getBottomToolbar().diplayMsg.update(action.result.msg);
                                },
                                failure:function(form,action){
                                    Ext.MessageBox.show({
		   					            title:HRMSRes.Public_Message_Error,
		   					            msg:action.result.msg,
		   					            buttons: Ext.MessageBox.OK,
		   					            icon:Ext.MessageBox.ERROR
		   				            });
		   				            grid.getBottomToolbar().diplayMsg.update(action.result.msg);
                                }
                            }); 
                        },
                        scope:this
                    },{
                        text: HRMSRes.Public_Button_Close, 
                        iconCls: 'icon-exit', 
                        handler: function(){ win.close();}
                    }],                                 
                    items: formPanel
                });
                win.show();
	        },
        	
	        remove:function(){
	            if(!this.grid.getSelectionModel().hasSelection()){
	                return;
	            }	    
	            var cm=this.grid.getColumnModel();
	            var sm=this.grid.getSelectionModel();
	            var rs=sm.getSelections();	   	    
	            var params=[];
        	    
	            for(var i=0;i<rs.length;i++){
	                var r=rs[i];
	                for(var j=0;j<cm.config.length;j++){
	                    var o=cm.config[j];
                        if(o.dataIndex==='dflag'){	
                            if(r.get(o.dataIndex)=='Y'){
                                Ext.MessageBox.show({
	   					            title:HRMSRes.Public_Message_Error,
	   					            msg:HRMSRes.MasterData_Message_DeleteDuplicate,
	   					            buttons: Ext.MessageBox.OK,
	   					            icon:Ext.MessageBox.ERROR
	   				            });
	   				            return;
                            }
                        }
	                }
	            }
        	    
	            var rows=[];
	            for(var i=0;i<rs.length;i++){
	                var r=rs[i];
	                var cols=[];
	                for(var j=0;j<cm.config.length;j++){	            
                        var o=cm.config[j];
                        if(o.isPk==='True'){
                            cols[cols.length]={
                                ColumnType:o.type,
                                ColumnName:o.dataIndex,
                                ColumnValue:r.get(o.dataIndex)	                
                            };
                        }                                        
                    }
                    rows[rows.length]=cols;
	            }
        	        
		        Ext.MessageBox.show({
			        title:HRMSRes.Public_Confirm_Title,
			        msg:HRMSRes.Public_Confirm_Delete,
			        buttons: Ext.Msg.YESNO,
			        fn: function(btn, text){
				        if (btn=='yes'){
					        Ext.Ajax.request({
			   			        url:ContextInfo.contextPath+ '/masterdata.mvc/delete',
			   			        success: function(response){
			   				        var o= Ext.util.JSON.decode(response.responseText);
			   				        if(o.status=='success'){
			   				            for(var i=0;i<rs.length;i++){
			   				                var r=rs[i];
	   					                    var index=this.grid.getStore().indexOf(r);
	   					                    this.grid.getStore().remove(r);
	   					                    sm.selectRow(index);
	   					                }
	   					                if (sm.getCount()==0){
	   						                sm.selectLastRow();
	   					                }
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
			   			        params:{tableName:this.config.tableName,record:Ext.encode(rows)}
			   		        })
				        }
			        },
			        icon:Ext.MessageBox.QUESTION,
			        scope:this
		        });
	        },
        	
	        getPrimaryKey:function(){
	           var keys=[];
	           var cm=this.grid.getColumnModel();
	           for(var i=0;i<cm.config.length;i++){
	                var o=cm.config[i];
	                if(o.isPk==='True'){	            
	                    keys[keys.length]={ColumnName:o.dataIndex,ColumnType:o.type,ColumnDisplayName:o.header};
	                }          
	            }
	            return keys;
	        },
        	
	        controlButton:function(id){
	            var enabled=!this.grid.getSelectionModel().hasSelection();        	    
        	    var locked=this.config.locked=='True';
        	    
	            Ext.getCmp(id+'_masterdata_add').setDisabled(locked || false);
                Ext.getCmp(id+'_masterdata_edit').setDisabled(locked || enabled);
                Ext.getCmp(id+'_masterdata_delete').setDisabled(locked || enabled);
            },
            
            isDflag:function(){        
                for(var i=0;i<this.config.columns.length;i++){
                    var o=this.config.columns[i];
                    if(o.dataIndex==='dflag'){	            
                        return true;
                    }          
                }
                return false;
            }
        });
        
        
        Ext.onReady(function(){                  
            var MasterConfig=Ext.decode('<%=ViewData["config"] %>'); 
            MasterConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');          
            new Maintain(MasterConfig);
        })
        
    </script>
</body>
</html>
