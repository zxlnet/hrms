<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pshctval.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.pshctval.pshctval" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<body>   
    <script type="text/javascript" charset="utf-8">  
        var Maintain=function(config){
            this.config=config;
            this.tabId=config.tabId;
            this.init();    
        }

        Ext.extend(Maintain,Ext.util.Observable,{
            init:function(){
                this.createGridPanel();
                this.createPanel();
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
                            xtype:'combo', 
                            id: 'cmbHctValHeadCountCfg',
                            //hidden:psemplymConfig.auth[this.tabId+'_' + 'change']!='True',
                            store: new Ext.data.Store({ 
		                                reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                                fields: ['ValueField','DisplayField']}), autoLoad:true,
		                                url:ContextInfo.contextPath+'/dropdown.mvc/getHeadCoungCfg'
		                                }),
                            displayField: 'DisplayField',valueField: 'ValueField',stateful:false,typeAhead: true,mode: 'local',
                            editable:false,triggerAction: 'all', anchor:'95%',emptyText:'Choose a headcount config',
                            width:200,
                            listeners:{
                                select:function(p){
                                    //new psemplymEditWindow(this.grid,false,p.getValue()).show(); 
                                    this.loadHeadCountCfg(p.getValue());
	    			            },
	    			            scope:this
	    		            } 
                        },
//	    	            { 
//	        	            id:this.tabId+'_pshctvaldata_add',
//	                        iconCls:'icon-add', 
//	                        text: HRMSRes.Public_Toolbar_Add, 
//	                        hidden:this.config.auth[this.tabId+'_pshctvaldata_add']!='True',
//	                        disabled:this.config.locked=='True',
//	                        handler: function(){
//	            	            var commonWin=new pshctvalDataCommonWindow(true,this.grid,this.tabId);;
//	            	            commonWin.setTitle(HRMSRes.Public_Add_WindowTitle);
//	            	            commonWin.show();	            	
//	                        }, 
//	                        scope: this 
//	                    },
//	                    
	                    { 
	        	            id:this.tabId+'_save',
	                        iconCls:'icon-save',
	                        //hidden:this.config.auth[this.tabId+'_pshctvaldata_edit']!='True', 
	                        disabled:true,
	                        text:HRMSRes.Public_Toolbar_Save,
	                        handler: this.save, 
	                        scope: this 
	                    },
	                    { 
	        	            id:this.tabId+'_delete',
	                        iconCls:'icon-remove', 
	                        //hidden:this.config.auth[this.tabId+'_pshctvaldata_delete']!='True',
	                        disabled:true,
	                        text:HRMSRes.Public_Toolbar_Delete, 
	                        handler: this.remove, 
	                        scope: this 
	                    },
	                    
//	                    { 
//	        	            id:this.tabId+'_pshctvaldata_query',
//	                        iconCls:'icon-query', 
//	                        hidden:this.config.auth[this.tabId+'_pshctvaldata_query']!='True',
//	                        text:HRMSRes.Public_Toolbar_Query,
//	                        handler: function(){
//	            	            var queryWin=new pshctvalDataCommonQueryWindow(this.grid,this.tabId);
//	            	            queryWin.show();
//	                        }, 
//	                        scope: this
//	                    },
	                    
	                    { 
	        	            id:this.tabId+'_export',
	                        iconCls:'icon-export', 
	                        //hidden:this.config.auth[this.tabId+'_pshctvaldata_export']!='True',
	                        text:HRMSRes.Public_Toolbar_OutExcel, 
	                        handler: this.exportExcel, 
	                        scope: this 
	                    },
//	                    { 
//	        	            id:this.tabId+'_pshctvaldata_import',
//	        	            hidden:this.config.auth[this.tabId+'_pshctvaldata_import']!='True',
//	        	            disabled:this.config.locked=='True',
//	                        iconCls:'icon-import', 
//	                        text:HRMSRes.Public_Toolbar_InExcel, 
//	                        handler: this.importExcel, 
//	                        scope: this 
//	                    },
	                    
	                    '->',{ 
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
                var storeType=Ext.data.Record.create([
                    {name:'col1'}
                ]);
        		
		        this.remoteStore=new Ext.data.Store({ 
        	        reader: new Ext.data.JsonReader({
	    		        totalProperty: "results",
	    		        root: "rows"               
	   	 	        },storeType), 		    
	   		        url:ContextInfo.contextPath+'/pshctval.mvc/list',
	   		        baseParams:{record:''},
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
                       
                this.remoteStore.type=storeType;
                var params={
                    start:0,
                    limit:1000
                };
                
                this.grid=new Ext.grid.EditorGridPanel({
                    border:true, 
    		        monitorResize:true,
    		        id:this.tabId+'_GridPanel', 
                    loadMask:true,  
                    layout:'fit',		            
                    ds: this.remoteStore, 
                    clicksToEdit:1,
                    viewConfig: { 
		                //forceFit: true 
		            },                          
                    cm: new Ext.grid.ColumnModel(
                        {header:'',  sortable: true, dataIndex: 'col1',align:'center',hidden:true}
                    ),      
                    bbar: new Ext.PagingToolbar({
                        pageSize:1000,
                        store: this.remoteStore,
                        displayInfo: true,
                        displayMsg:HRMSRes.Public_PagingToolbar_Total+':{1}/{2}',
                        emptyMsg: HRMSRes.Public_PagingToolbar_EmptyMsg                    
                    })
	            });
            },
            
            reloadGrid:function(){     
		        var storeType=Ext.data.Record.create(this.hctConfig.records);
        		
		        this.remoteStore=new Ext.data.Store({ 
        	        reader: new Ext.data.JsonReader({
	    		        totalProperty: "results",
	    		        root: "rows"               
	   	 	        },storeType), 		    
	   		        url:ContextInfo.contextPath+'/pshctval.mvc/list',
	   		        baseParams:{record:this.hctConfig.hccd},
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
                    limit:1000
                };
                this.remoteStore.load({params:params});
                
                this.grid.reconfigure(this.remoteStore,
                    new Ext.grid.ColumnModel(this.hctConfig.columns));
            },
                        
            loadHeadCountCfg:function(hccd){
		        Ext.Ajax.request({
   			        url:ContextInfo.contextPath+ '/pshctval.mvc/load',
   			        success: function(response){
   				        var o= Ext.util.JSON.decode(response.responseText);
   				        if(o.status=='success'){
   				            this.hctConfig=Ext.decode(o.msg);
   				            this.reloadGrid();
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
   			        params:{record:hccd}
   		        })
            },
            
            save:function(){
	            this.grid.getBottomToolbar().diplayMsg.update('');
                
                var dtlparams=[];       
                var st = this.grid.getStore();
                var cm = this.grid.getColumnModel();
                for (var i =1; i<= st.getCount();i++ ){
                    var y = st.getAt(i-1);
                    var t = [];
                    for (var j=2;j<cm.getColumnCount();j++){
                        if (y.get(cm.getDataIndex(j))=='') {
		   		            Ext.MessageBox.show({
			                    title: HRMSRes.Public_Message_Error,
			                    msg:'Data cannot be empty.',
			                    buttons: Ext.MessageBox.OK,
			                    icon:Ext.MessageBox.ERROR
		                    });
                            return;
                        }
                        
                        t = {X:cm.config[j].actualValue, Y:y.get('ItemValue'),V:y.get(cm.getDataIndex(j))};
                        dtlparams[dtlparams.length] = t;
                    }
                }

                var keyparams=[];
                keyparams[0]={ColumnName:'hccd',ColumnValue:this.hctConfig.hccd};

	            var method='edit';
		        Ext.Ajax.request({
		           url: ContextInfo.contextPath+'/pshctval.mvc/'+method,
		           success: function(response){
		   		        var o= Ext.util.JSON.decode(response.responseText);		   		
		   		        if (o.status=='success'){
		   		            this.grid.getStore().commitChanges();
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
        	    	    
                params.record = this.hctConfig.hccd;
                                 
	            var form=document.createElement('form');
	            form.name='excelForm';
	            form.method='post';
	            form.action=ContextInfo.contextPath+ '/pshctval.mvc/exportExcel';
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

	        remove:function(){
        	    var hccd = this.hctConfig.hccd;
        	    
		        Ext.MessageBox.show({
			        title:HRMSRes.Public_Confirm_Title,
			        msg:HRMSRes.Public_Confirm_Delete,
			        buttons: Ext.Msg.YESNO,
			        fn: function(btn, text){
				        if (btn=='yes'){
					        Ext.Ajax.request({
			   			        url:ContextInfo.contextPath+ '/pshctval.mvc/delete',
			   			        success: function(response){
			   				        var o= Ext.util.JSON.decode(response.responseText);
			   				        if(o.status=='success'){
			   				            //need to reload
			   				            this.reloadGrid();
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
			   			        params:{record:hccd}
			   		        })
				        }
			        },
			        icon:Ext.MessageBox.QUESTION,
			        scope:this
		        });
	        },
        	
	        controlButton:function(id){
	            var enabled=false;  
	            if (this.grid.store.getCount()>0)
	                enabled=true;    
                
                Ext.getCmp(id+ '_save').setDisabled(!enabled);
                Ext.getCmp(id+ '_delete').setDisabled(!enabled);	                
            }
        });
        
        
        Ext.onReady(function(){ 
//            var cp = new Ext.state.CookieProvider({
//               expires: new Date(new Date().getTime()+(1000*60*60*24*10*365))
//            });
//            Ext.state.Manager.setProvider(cp);
                  
            var pshctvalConfig=Ext.decode('<%=ViewData["config"] %>'); 
            pshctvalConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');          
            new Maintain(pshctvalConfig);
        })
        
    </script>
</body>
</html>
