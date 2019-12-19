<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="prsalhis.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.prsalhis.prsalhis" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var prsalhisConfig=Ext.decode('<%=ViewData["config"] %>'); 
prsalhisConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var prsalhisPageName = 'prsalhis';
var prsalhisKeyColumn='rnno';
            
var prsalhisQueryWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	prsalhisQueryWindow.superclass.constructor.call(this,{
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

Ext.extend(prsalhisQueryWindow,Ext.Window,{
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
                        name: 'sfid',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sftn,
                        name: 'sftn',stateful:false,anchor:'95%'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Payroll_Label_rnno,
                        name: 'rnno',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Public_Label_perd ,
                        name: 'perd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getValidPeriod',
		                    listeners:{
                                load:function(){f = this.form.findField('perd');f.setValue(f.getValue());},
                                scope:this}})
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

var prsalhisEditWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	

	prsalhisEditWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:350, 
        height:150, 
        title:this.isNew?HRMSRes.Public_Add_WindowTitle:HRMSRes.Public_Edit_WindowTitle,
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.basisFormPanel,
        listeners:{
            show:function(){              
		        //var keyField = this.basisForm.findField(prsalhisKeyColumn);

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
		        //var keyValue = keyField.getValue();
                //setLastModifiedInfo(prsalhisConfig,this.basisForm);
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

Ext.extend(prsalhisEditWindow,Ext.Window,{
    init:function(){
		this.basisFormPanel=this.createBasisFormPanel();
		this.basisForm=this.basisFormPanel.getForm();
	},
	createBasisFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
      		 items: [
      		    {},{
      		    layout:'column',
      		    items:[

      		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'numberfield',fieldLabel:HRMSRes.Payroll_Label_ajva,
                        name: 'ajva',stateful:false,anchor:'95%',decimalPrecision:0,keepZero:false,value:'2'}]}

      		    ]
      		 }] 
       })
	},
	save: function(){
		if(!this.basisForm.isValid()) return;
		this.grid.getBottomToolbar().diplayMsg.update('');
//		var params={};
//		this.basisForm.items.each(function(f){
//            if(f.isFormField){
//                params[f.getName()]=f.getValue();
//            }
//        });
		var params={};
        params['ajva'] = this.basisForm.findField('ajva').getValue();    
        var keyparams=[];
        
        var r = this.grid.getSelectionModel().getSelected();
        keyparams[0]={ColumnName:'emno',ColumnValue:r.get('emno')};
        keyparams[1]={ColumnName:'itcd',ColumnValue:r.get('itcd')};
        keyparams[2]={ColumnName:'rscd',ColumnValue:r.get('rscd')};
        keyparams[3]={ColumnName:'sqno',ColumnValue:r.get('sqno'),ColumnType:'int'};
        keyparams[4]={ColumnName:'pkty',ColumnValue:r.get('pkty')};
        keyparams[5]={ColumnName:'perd',ColumnValue:r.get('perd')};
        
	    var method=this.isNew?'new':'edit';
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/' + prsalhisPageName + '.mvc/edit',
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
		   params: {record:Ext.encode({params:params['ajva'],keycolumns:keyparams})}
		});
	}
});


var prsalhisPanel=function(){
    this.tabId=prsalhisConfig.tabId;
	this.init();	
	
	prsalhisPanel.superclass.constructor.call(this,{
		applyTo:this.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	autoScroll:true,      
	        tbar: [{ 
	        	id:this.tabId+ '_edit',
	            iconCls:'icon-update', 
	            text:HRMSRes.Public_Toolbar_Edit,
	            hidden: prsalhisConfig.auth[this.tabId + '_' + 'edit'] != 'True',
	            disabled:true,
	            handler: function(){
	            	new prsalhisEditWindow(this.grid).show(); 
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_query',
	            iconCls:'icon-query', 
	            text:HRMSRes.Public_Toolbar_Query,
	            hidden:prsalhisConfig.auth[this.tabId+'_' + 'query']!='True',
	            handler: function(){
	            	new prsalhisQueryWindow(this.grid).show();
	            }, 
	            scope: this
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            hidden:prsalhisConfig.auth[this.tabId+'_' + 'exportexcel']!='True',
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
                iconCls:prsalhisConfig.muf=='True'?'icon-mufdelete':'icon-mufadd', 
                text:'', 
                handler: function(){updateMUF(this.tabId,prsalhisConfig.muf?'delete':'add',prsalhisConfig,this.grid);},
                scope: this 
            }], 
	        items:this.grid
       }]
	})
}

Ext.extend(prsalhisPanel, Ext.Panel, {
    init: function() {
        this.grid = this.createGridPanel();
    },

    createGridPanel: function() {

        var prsalhisStoreType = Ext.data.Record.create([
		    { name: 'emno' },
		    { name: 'sfid' },
		    { name: 'stfn' },
		    { name: 'perd' },
		    { name: 'itcd' },
		    { name: 'itnm' },
            { name: 'rscd' },
            { name: 'rsnm' },
            { name: 'sqno' },
            { name: 'pkty' },
            { name: 'crcd' },
            { name: 'crnm' },
            { name: 'valu' },
            { name: 'padt' },
            { name: 'rnno' },
            { name: 'ajva' },
            { name: 'lmtm' },
            { name: 'lmur' },
            { name: 'rfid' },
            { name: 'isca' }
		]);

        var store = new Ext.data.Store({
            reader: new Ext.data.JsonReader({
                totalProperty: "results",
                root: "rows"
            }, prsalhisStoreType),
            baseParams: { record: Ext.encode({ params: [{ ColumnName: prsalhisKeyColumn, ColumnValue: ''}] }) },
            url: ContextInfo.contextPath + '/' + prsalhisPageName + '.mvc/list',
            listeners: {
                loadexception: function(o, t, response) {
                    var o = Ext.util.JSON.decode(response.responseText);
                    this.grid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                load: function() {
                    this.controlButton(this.tabId);
                },
                scope: this
            }
        });

        var params = {
            start: 0,
            limit: Pagination.pagingSize
        };

        //store.load({params:params});

        return new Ext.grid.GridPanel({
            border: true,
            monitorResize: true,
            loadMask: true,
            ds: store,
            viewConfig: {
                //forceFit: true
            },
            listeners: {
                rowclick: function() {
                    this.controlButton(this.tabId);
                },
                scope: this
            },
            cm: new Ext.grid.ColumnModel([
                { header: HRMSRes.Public_Label_sfid, sortable: true, dataIndex: 'sfid' },
                { header: HRMSRes.Public_Label_sftn, sortable: true, dataIndex: 'stfn' },
                { header: HRMSRes.Public_Label_perd, sortable: true, dataIndex: 'perd' },
                { header: HRMSRes.Payroll_Label_rnno, sortable: true, dataIndex: 'rnno' },
                { header: HRMSRes.Payroll_Label_itnm, sortable: true, dataIndex: 'itnm' },
                { header: HRMSRes.Public_Label_sqno, sortable: true, dataIndex: 'sqno' },
                { header: HRMSRes.Payroll_Label_valu, sortable: true, dataIndex: 'valu' },
                { header: HRMSRes.Master_Label_crnm, sortable: true, dataIndex: 'crnm' },
                { header: HRMSRes.Payroll_Label_isca, sortable: true, dataIndex: 'isca' },
                { header: HRMSRes.Master_Label_padt, sortable: true, dataIndex: 'padt', renderer: formatDateNoTime },
                { header: HRMSRes.Payroll_Label_ajva, sortable: true, dataIndex: 'ajva' }
            ]),
            bbar: new Ext.PagingToolbar({
                pageSize: Pagination.pagingSize,
                store: store,
                displayInfo: true,
                displayMsg: HRMSRes.Public_PagingToolbar_Total + ':{1}/{2}',
                emptyMsg: HRMSRes.Public_PagingToolbar_EmptyMsg
            })
        })
    },

    controlButton: function(id) {
        var enabled = !this.grid.getSelectionModel().hasSelection();
        Ext.getCmp(id + '_edit').setDisabled(enabled);
        //Ext.getCmp(id+ '_delete').setDisabled(enabled);
    },

    exportExcel: function() {
        if (this.grid.getStore().getTotalCount() <= 0) {
            this.grid.getBottomToolbar().diplayMsg.update('<font color=red>' + HRMSRes.Public_Message_ExcelNoRecord + '</font>');
            return;
        }

        var cm = this.grid.getColumnModel();
        var header = [];

        for (var i = 0; i < cm.config.length; i++) {
            if (!cm.isHidden(i)) {
                var cname = cm.config[i].header;
                var mapping = cm.config[i].dataIndex;
                header[header.length] = {
                    ColumnDisplayName: cname,
                    ColumnName: mapping
                };
            }
        }
        var params = { record: Ext.encode({ params: [{ ColumnName: prsalhisKeyColumn, ColumnValue: ''}], headers: header }) };

        if (this.grid.queryParams) {
            this.grid.queryParams['headers'] = header;
            delete params.record;
            params.record = Ext.encode(this.grid.queryParams);
            delete this.grid.queryParams.header;
        }

        var form = document.createElement('form');
        form.name = 'excelForm';
        form.method = 'post';
        form.action = ContextInfo.contextPath + '/' + prsalhisPageName + '.mvc/exportexcel';
        for (var p in params) {
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
    new prsalhisPanel();
})

    </script>

</body>
</html>
