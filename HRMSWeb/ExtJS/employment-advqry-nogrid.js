this.empAdvQryResult;

var empAdvQryQueryNoGridWindow=function(grid,returntype,url,param){
    this.returntype = returntype;
    this.url = url;
    this.objparams=param;
    
	this.init();	
	
    this.fullEmpAdvQryFormPanel = {
            xtype:'panel',
            autoHeight:true,
            items:[{
	            xtype:'panel',
	            bodyStyle:'padding:0px;',
	            border:false,
	            baseCls:'x-fieldset-noborder',
	            columnWidth: .50,
	            items:this.empAdvQryFormPanel
            }]
        };
	
	empAdvQryQueryNoGridWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:600, 
        height:300, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.fullEmpAdvQryFormPanel,
        title:HRMSRes.Public_Query_WindowTitle,
        listeners:{
        },
        buttons: [{ 
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

Ext.extend(empAdvQryQueryNoGridWindow,Ext.Window,{
    init:function(){
		this.empAdvQryFormPanel=this.createEmpAdvQryFormPanel();
		this.form=this.empAdvQryFormPanel.getForm();
	},
	
	createEmpAdvQryFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
             height:230,
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
	Confirm:function(){	 
	    if (this.returntype=='1')  //Employment List
	    {
//	        var params=[];	
//	        var sel = this.empAdvQryGrid.getSelectionModel().getSelections();
//	        for (var i=0;i<sel.length;i++)
//	        {
//	            params[params.length] = {emno:sel[i].data['emno'],sfid:sel[i].data['sfid'],ntnm:sel[i].data['sfnm']};
//	        }
//	        
//	        empAdvQryResult = params;
//	        
//	        if (this.url!='')
//	        {
//	            this.call(this.url,empAdvQryResult);
//	        }
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