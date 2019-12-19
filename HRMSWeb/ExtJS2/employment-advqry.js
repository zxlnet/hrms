this.empAdvQryResult;

var empAdvQryQueryWindow=function(grid,returntype,url,param){
//	this.grid = grid;
//	this.store=this.grid.getStore();
    this.returntype = returntype;
    this.url = url;
    this.objparams=param;
    
	this.init();	
	
    this.fullEmpAdvQryFormPanel = {
            xtype:'panel',
            items:[{
	            xtype:'panel',
	            bodyStyle:'padding:0px;',
	            border:false,
	            baseCls:'x-fieldset-noborder',
	            columnWidth: .50,
	            items:this.empAdvQryFormPanel
            },{
	            xtype:'panel',
	            layout:'fit',
	            bodyStyle:'padding:0px;',
	            border:false,
	            baseCls:'x-fieldset-noborder',
	            columnWidth: .50,
	            items:this.empAdvQryGrid
            }]
        };
	
//    this.empAdvQryTab=new Ext.TabPanel({
//                autoTabs:true,
//                activeTab:0,
//                border:false,
//                frame:true,
//                items:[{
//                            title: 'Query Criteries',
//                            items: fullEmpAdvQryFormPanel
//                        }]
//            }),
//	
	empAdvQryQueryWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:600, 
        height:500, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.fullEmpAdvQryFormPanel,
        title:HRMSRes.Public_Query_WindowTitle,
        listeners:{
//            show:function(){
//                var queryParams=this.grid.queryParams;
//		        if(queryParams){
//		            queryParams=queryParams.params;
//		            for(var i=0;i<queryParams.length;i++){
//		                var queryParam=queryParams[i];
//		                var fieldName=queryParam.ColumnName;
//		                var value=queryParam.ColumnValue;
//		                var field=this.form.findField(fieldName);
//		                if(field){
//		                    field.setValue(value);
//		                }
//		            }
//		        }
//            }
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
            text: HRMSRes.Public_Button_Confirm, 
            iconCls: 'icon-accept', 
            handler: function(){                  
                this.Confirm();
            },
            scope:this
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

Ext.extend(empAdvQryQueryWindow,Ext.Window,{
    init:function(){
		this.empAdvQryFormPanel=this.createEmpAdvQryFormPanel();
		this.form=this.empAdvQryFormPanel.getForm();
		this.empAdvQryGrid = this.createEmpAdvQryGridPanel();
	},
	
	createEmpAdvQryFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
             items: [
                    {
      		    layout:'column',
      		    items:[
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sfid,
                        name: 'sfid',stateful:false,anchor:'95%',UpperOnBlur:'true'}]},

      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'textfield',fieldLabel:HRMSRes.Public_Label_sftn,
                        name: 'stfn',stateful:false,anchor:'95%'}]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_sftm,lazyRender:true,
                        name: 'stcd',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getStaffType'})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Personal_Label_emst,
                        name: 'emst',stateful:false,typeAhead: true,lazyRender:true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: EmpStateStore
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_dvnm ,
                        name: 'dvcd',stateful:false,typeAhead: true,lazyRender:true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getDivision'})
    		          }]},


                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_bsnm ,
                        name: 'bscd',stateful:false,typeAhead: true,lazyRender:true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getBusiness'})
    		          }]},


                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_dpnm ,
                        name: 'dpcd',stateful:false,typeAhead: true,lazyRender:true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getDepartment'})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_grnm ,
                        name: 'grcd',stateful:false,typeAhead: true,lazyRender:true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getGrade'})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_psnm ,
                        name: 'pscd',stateful:false,typeAhead: true,lazyRender:true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getPosition'})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_jcnm ,
                        name: 'jccd',stateful:false,typeAhead: true,lazyRender:true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getJobClass'})
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Master_Label_jtnm ,
                        name: 'jtcd',stateful:false,typeAhead: true,lazyRender:true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getJobType'})
    		          }]}
      		    ]
      		 }] 
       })
	},
	
	createEmpAdvQryGridPanel:function(){

		var empAdvQryQueryStoreType=Ext.data.Record.create([
            {name:'isselected'},
		    {name:'emno'},
            {name:'sfid'},
            {name:'stfn'},
            {name:'dvcd'},
            {name:'dvnm'},
            {name:'bscd'},
            {name:'bsnm'},
            {name:'dpcd'},
            {name:'dpnm'},
            {name:'grcd'},
            {name:'grnm'},
            {name:'pscd'},
            {name:'psnm'},
            {name:'jccd'},
            {name:'jcnm'},
            {name:'jtcd'},
            {name:'jtnm'},           
		]);

		var store=new Ext.data.Store({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},empAdvQryQueryStoreType), 	
	   	    baseParams:{record:Ext.encode({params:[{ColumnName:'emno',ColumnValue:''}]})},   
	   		url:ContextInfo.contextPath+'/psemplym.mvc/getAdvQryEmployment',
	   		listeners:{
                loadexception:function(o,t,response)
                {
                    var o= Ext.util.JSON.decode(response.responseText);		   		
                    this.empAdvQryGrid.getBottomToolbar().diplayMsg.update(o.msg);
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
        
        //store.load({params:params});
        var selectionModel=new Ext.grid.CheckboxSelectionModel();
        
        return new Ext.grid.GridPanel({
    		border:true, 
    		monitorResize:true, 
            loadMask:true,  		            
            ds: store, 
            height:265,
            viewConfig: { 
		        autoFill: true 
		    }, 
            listeners:{
	            rowclick:function(){
	                //this.controlButton(this.tabId);
	            },
	            scope:this
            },                      
            cm: new Ext.grid.ColumnModel([ 
                selectionModel,
                {header:HRMSRes.Public_Label_sfid,sortable: true, dataIndex: 'sfid'},
                {header:HRMSRes.Public_Label_sftn,sortable: true, dataIndex: 'stfn'},
                {header:HRMSRes.Master_Label_bsnm,sortable: true, dataIndex: 'bsnm'},
                {header:HRMSRes.Master_Label_dpnm,sortable: true, dataIndex: 'dpnm'},
                {header:HRMSRes.Master_Label_grnm,sortable: true, dataIndex: 'grnm'},
                {header:HRMSRes.Master_Label_psnm,sortable: true, dataIndex: 'psnm'},
                {header:HRMSRes.Master_Label_jcnm,sortable: true, dataIndex: 'jcnm'},
                {header:HRMSRes.Master_Label_jtnm,sortable: true, dataIndex: 'jtnm'}
            ]),
            sm:selectionModel,
            bbar: new Ext.PagingToolbar({
                pageSize:Pagination.pagingSize,
                store: store,
                displayInfo: true,
                displayMsg:HRMSRes.Public_PagingToolbar_Total+':{1}/{2}',
                emptyMsg: HRMSRes.Public_PagingToolbar_EmptyMsg                    
            })
	    })
	},
	Query:function(){	    
	    this.empAdvQryGrid.getBottomToolbar().diplayMsg.update('');
	    var params=[];	        
        this.form.items.each(function(f){
            if ((f.isFormField) && (f.getValue()!='')){
                var p={
                    ColumnName:f.getName(),
                    ColumnValue:f.getValue()                
                };
                params[params.length]=p;                  
            }
        });
        var loadParams={start:0,limit:Pagination.pagingSize};
        /***modified for adjquery**/
        this.empAdvQryGrid.queryParams={
            params:params
        };
        this.empAdvQryGrid.store.baseParams={record:Ext.util.JSON.encode(this.empAdvQryGrid.queryParams)};
		this.empAdvQryGrid.store.load({params:loadParams});
	},
	Confirm:function(){	 
	    if (this.returntype=='1')  //Employment List
	    {
	        var params=[];	
	        var sel = this.empAdvQryGrid.getSelectionModel().getSelections();
	        for (var i=0;i<sel.length;i++)
	        {
	            params[params.length] = {emno:sel[i].data['emno'],sfid:sel[i].data['sfid'],ntnm:sel[i].data['sfnm']};
	        }
	        
	        empAdvQryResult = params;
	        
	        if (this.url!='')
	        {
	            this.call(this.url,empAdvQryResult);
	        }
	    }
	    else if ((this.returntype=='2') || (this.returntype=='') || (this.returntype==undefined)) //Query criterias
	    {
	        var params=[];	        
            this.form.items.each(function(f){
                if ((f.isFormField) && (f.getValue()!='')){
                    var p={
                        ColumnName: "emp." + f.getName(),
                        ColumnValue:f.getValue()                
                    };
                    params[params.length]=p;                  
                }
            });
            
            empAdvQryResult=params;
            
            this.close();
        }
        else
        {
            //Default
        }
	},
	cleanQuery:function(){
	    cleanQueryCriterias(this.form);
	},
	call:function(url,params){
            Ext.Ajax.request({
	            url:url,
	            success: function(response){
		            var o= Ext.util.JSON.decode(response.responseText);
		                if(o.status=='success'){
		                Ext.MessageBox.show({
			                title:HRMSRes.Public_Message_EditWell,
			                msg:o.msg,
			                buttons: Ext.MessageBox.OK,
			                icon:Ext.MessageBox.INFO 
		                });
		                this.close();
                    }else {
		                Ext.MessageBox.show({
			                title:HRMSRes.Public_Message_Error,
			                msg:o.msg,
			                buttons: Ext.MessageBox.OK,
			                icon:Ext.MessageBox.ERROR 
		                });
                    }	 
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
	            params:{record:Ext.encode({empparams:params,objparams:this.objparams})}
            })	    
	},
	scope:this
});