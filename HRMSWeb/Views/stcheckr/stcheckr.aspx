<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="stcheckr.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.stcheckr.stcheckr" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var stcheckrConfig=Ext.decode('<%=ViewData["config"] %>'); 
stcheckrConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');	        
            
});

var stcheckrCheckWindow=function(grid){
	this.grid=grid;
	this.store=this.grid.getStore();
	this.init();	
	
	stcheckrCheckWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:300, 
        height:250, 
        closeAction:'hide', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.formPanel,
        title:HRMSRes.Public_Check_WindowTitle,
        listeners:{
            show:function(){
                var periodcombo=this.form.findField('PeriodCombo');
                periodcombo.setValue(ContextInfo.currentPeriod.fullPeriod);
                this.form.findField('FromDate').setValue(ContextInfo.currentPeriod.start);
				this.form.findField('ToDate').setValue(ContextInfo.currentPeriod.end);
            }
        },
        buttons: [{ 
            text:HRMSRes.Public_Button_Confirm, 
            iconCls:'icon-save', 
            handler: this.CheckPeriodStatus,
            scope: this,
            id:'stcheckr_Check_Confirm'
        },{ 
            text: HRMSRes.Public_Button_Close, 
            iconCls: 'icon-exit', 
            handler: function(){ this.hide();},
            scope:this
        }]
	})
}

Ext.extend(stcheckrCheckWindow,Ext.Window,{
    init:function(){
        this.pbar=new Ext.ProgressBar({
            anchor:'130%' 
        });
		this.formPanel=this.createFormPanel();
		this.form=this.formPanel.getForm();
	},
	createFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:90,
	         header:true,
      		 items: [
                    { columnWidth: .5, layout: 'form',
                        items: [{ xtype: 'combo', fieldLabel: HRMSRes.Public_Label_perd,
                            name: 'perd', stateful: false, typeAhead: true,
                            triggerAction: 'all', mode: 'local', maxHeight: 150, anchor: '95%',
                            displayField: 'DisplayField', valueField: 'ValueField',
                            store: new Ext.data.Store({
                                reader: new Ext.data.JsonReader({ totalProperty: "results", root: "rows",
                                    fields: ['ValueField', 'DisplayField']
                                }), autoLoad: true,
                                url: ContextInfo.contextPath + '/dropdown.mvc/getValidPeriod',
                                listeners: {
                                    load: function() { f = this.form.findField('perd'); f.setValue(f.getValue()); },
                                    scope: this}
                                })
                }]},
            { 
                xtype:'datefield', 
                fieldLabel:HRMSRes.Public_Label_FromDate, 
                name: 'FromDate', 
                allowBlank:false,
                stateful:false,
                disabled: true, 
                format:DATE_FORMAT.DATE_ONLY, 
                anchor:'95%' 
            },{ 
                xtype:'datefield', 
                fieldLabel:HRMSRes.Public_Label_ToDate, 
                name: 'ToDate', 
                allowBlank:false,
                stateful:false,
                disabled: true, 
                format:DATE_FORMAT.DATE_ONLY, 
                anchor:'95%' 
            },this.pbar] 
       })
	},
	
	Check:function(){

	    if(!this.form.isValid()) return;
   		Ext.getCmp('stcheckr_Check_Confirm').setDisabled(true);	
		var params={
		    year:this.year,
		    period:this.period
		};
		
		this.grid.getBottomToolbar().diplayMsg.update('');
		
		var loadParams={start:0,limit:Pagination.pagingSize}; 
		
		this.grid.queryParams=params;
		this.grid.getStore().baseParams={record:Ext.util.JSON.encode(params)};
		this.grid.store.on('load',function(){this.grid.getBottomToolbar().diplayMsg.update(HRMSRes.Public_Message_CheckDone);},this);
		this.grid.getStore().load({params:loadParams});
		
   		Ext.getCmp('stcheckr_Check_Confirm').setDisabled(false);	
		this.close();
		
	},
	stopPbar:function(){
        this.pbar.reset(false);
	    Ext.TaskMgr.stopAll();
    },
	scope:this
});

var stcheckrPanel=function(){
    this.stcheckrCalculationWin;
    this.stcheckrCheckWin;
    this.stcheckrLockWin;
    this.tabId=stcheckrConfig.tabId;
	this.init();	
	stcheckrPanel.superclass.constructor.call(this,{
		applyTo:stcheckrConfig.tabId, 
	    layout:'fit',
	    id:this.tabId+'_panel',
	    monitorResize:true,
	    items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	autoScroll:true,      
	        tbar: [{ 
	        	id:this.tabId+'_stcheckr_check',
	            iconCls:'icon-import', 
	            text: HRMSRes.Public_Toolbar_Check, 
	            hidden:stcheckrConfig.auth[this.tabId+'_check']!='True',
	            handler: function(){
	            	if (!this.stcheckrCheckWin){
	            		this.stcheckrCheckWin=new stcheckrCheckWindow(this.grid);
	            	}	            	
	            	this.stcheckrCheckWin.show();
	            }, 
	            scope: this 
	        },        	
	        items:this.grid
       }]
	})
}

function stcheckrRenderHref(val,m,r){
    var result=r.get("checkresult");
    var filename=r.get('filename');
    
    if (result=="Success")
    {
        return '<span style=\'color:green\'>'+val+'</span>';
    }
    else
    {
        if(filename!=''){
           var url=ContextInfo.contextPath+'/trace.mvc/index?menuId=Z900&fileName='+filename;
           var menu='Z900';
           var title=HRMSRes.Public_Menu_TraceViewer;
           var method="javascript:mainPanel.loadClass(\'"+url+'\',\''+menu+'\',\'\',\''+title+"\',\'\',true)";
           var href='<a href='+method+'>'+filename+'</a>';
           val=val.replace(filename,href);
        }           
        return '<span style=\'color:red\'>'+val+'</span>';
    }
}


Ext.extend(stcheckrPanel,Ext.Panel,{
    init:function(){
		this.grid=this.createGridPanel();
	},
	createGridPanel:function(){
		var stcheckrType=Ext.data.Record.create([
            {name: 'sqno'},
            {name: 'cknm'},
            {name: 'ckrs'},
            {name: 'finm'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},stcheckrType), 		    
	   		url:ContextInfo.contextPath+'/stcheckr.mvc/check'
        });
        store.type=stcheckrType;
        
        return new Ext.grid.GridPanel({
	    		border:true, 
	    		monitorResize:true, 
	            loadMask:true,  		            
                ds: store, 
                viewConfig: { 
			        forceFit: true 
			    },                          
                cm: new Ext.grid.ColumnModel([ 
                        {header:HRMSRes.P308_Label_Sequence,  sortable: true, dataIndex: 'sqnm',align:'left',width:20},
                        {header:HRMSRes.P308_Label_RuleName,  sortable: true, dataIndex: 'cknm'},
                        {header:HRMSRes.P308_Label_Result,  sortable: true, dataIndex: 'ckrs',renderer:stcheckrRenderHref}
                ]),
                bbar: new Ext.PagingToolbar({
                    pageSize:Pagination.pagingSize,
                    store: store,
                    displayInfo: true,
                    displayMsg:HRMSRes.Public_PagingToolbar_Total+':{1}/{2}',
                    emptyMsg: HRMSRes.Public_PagingToolbar_EmptyMsg                    
                })
	    })
	}
})


Ext.onReady(function(){ 
    new stcheckrPanel();
})

</script>