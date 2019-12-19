var logViewerWindow=function(rfid){
    this.rfid = rfid;    
	this.init();	
	
    this.logViewerWindowPanel = {
            xtype:'panel',
            items:[{
	            xtype:'panel',
	            layout:'form',
	            bodyStyle:'padding:0px;',
	            border:false,
	            baseCls:'x-fieldset-noborder',
	            columnWidth: .50,
	            items:this.logViewerGrid
            }]
        };
	
	logViewerWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:600, 
        height:400, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.logViewerWindowPanel,
        title:HRMSRes.Public_Query_WindowTitle,
        buttons: [{ 
            text: HRMSRes.Public_Button_Close, 
            iconCls: 'icon-exit', 
            handler: function(){                  
                this.close();
            },
            scope:this
        }]
	})
}

Ext.extend(logViewerWindow,Ext.Window,{
    init:function(){
		this.logViewerGrid = this.createLogViewerGridPanel();
	},
	
    renderURL:function(val,m,r,rowIndex,ds){
        return '<a href=\'javascript:showHTMLTextViewer()\'>Get more.</a>';
    },
	createLogViewerGridPanel:function(){

		var logViewerStoreType=Ext.data.Record.create([
            {name:'lgtm'},
		    {name:'lgur'},
            {name:'actn'},
            {name:'lgtx'}
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},logViewerStoreType), 	
	   	    baseParams:{record:Ext.encode({rfid:this.rfid})},   
	   		url:ContextInfo.contextPath+'/public.mvc/getRecordLogHistory',
	   		listeners:{
                loadexception:function(o,t,response)
                {
                    var o= Ext.util.JSON.decode(response.responseText);		   		
                    this.logViewerGrid.getBottomToolbar().diplayMsg.update(o.msg);
                },
                load:function(){
                    //this.controlButton(this.tabId);
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
    		id:'logViewGrid',
    		monitorResize:true, 
            loadMask:true,  		            
            ds: store, 
            height:330,
            viewConfig: { 
		        autoFill: true ,
		        enableRowBody:true,
		        showPreview:true,
		        getRowClass : function(record, rowIndex, p, store){
                if(this.showPreview){
                    //p.body = '<p>'+record.data.lgtx+'</p>';
                    return 'x-grid3-row-expanded';
                }
                return 'x-grid3-row-collapsed';
               }
		    }, 
            listeners:{
	            rowclick:function(t,r,e){
	                HTMLTextViewer_Text = t.getSelectionModel().getSelected().get('lgtx');
	            },
	            scope:this
            },                      
            cm: new Ext.grid.ColumnModel([ 
                {header:HRMSRes.Public_Label_lgtm,sortable: true, dataIndex: 'lgtm',renderer:formatDate},
                {header:HRMSRes.Public_Label_lgur,sortable: true, dataIndex: 'lgur'},
                {header:HRMSRes.Public_Label_actn,sortable: true, dataIndex: 'actn'},
                {header:HRMSRes.Public_Label_lgtx,sortable: true, dataIndex: 'lgtx',renderer:this.renderURL}
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
	scope:this
});