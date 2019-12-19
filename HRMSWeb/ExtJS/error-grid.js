
Ext.namespace('GotWell.Error');
  
GotWell.Error.ErrorGrid={
     /// <summary>
    /// Action for creating confirm window for showing message to user
    /// </summary>
    /// <Remarks>
    CreateConfirmWindow:function(result,title,fn,parentobj){	        
        var win=new Ext.Window({
            title:title || HRMSRes.Public_Grid_CalculationData,
            layout:'fit', 
            width:700, 
            height:400, 
            closeAction:'close', 
            modal: true, 
            resizable: false,
            buttonAlign: 'center',  
            buttons:[{
                text: HRMSRes.Public_Button_Continue, 
                iconCls: 'icon-exit', 
                handler: function(){ 
                    win.close();
                    fn(parentobj);                    
                }
            },{
                text: HRMSRes.Public_Button_Close, 
                iconCls: 'icon-exit', 
                handler: function(){ win.close();}
            }],                                 
            items: this.CreateErrorGrid(result)
        });
        win.show();
    },
    
    /// <summary>
    /// Action for creating error window for showing message to user
    /// </summary>
    /// <Remarks>
    CreateErrorWindow:function(result,title,fn,fnParam){	        
        var win=new Ext.Window({
            title:title,
            layout:'fit', 
            width:700, 
            height:400, 
            closeAction:'close', 
            modal: true, 
            resizable: false,
            buttonAlign: 'center',  
            buttons:[{
                text: HRMSRes.Public_Button_Close, 
                iconCls: 'icon-exit', 
                handler: function(){ 
                    if (fn){
                        fn(fnParam);
                    }
                    win.close();
                }
            }],                                 
            items: this.CreateErrorGrid(result)
        });
        win.show();
    },

    /// <summary>
    /// Action for creating the column for the error grid
    /// </summary>
    /// <Remarks>
    CreateFieldsAndModels:function(result){           
        var cms=[];
        cms[cms.length]=new Ext.grid.RowNumberer();
        var fields=[];
        for(var p in result.rows[0]){
            cms[cms.length]={
                header:p,
                sortable:true,
                dataIndex:p
            };
            fields[fields.length]=p;
        }
        return {model:cms,fields:fields};
    },

    /// <summary>
    /// Action for getting data from server 
    /// </summary>
    /// <Remarks>
    GetDataFromServer:function(rows){
        var data=[];
        for(var i=0;i<rows.length;i++){
            var row=rows[i];
            var d=[];
            for(var p in row){
                d[d.length]=row[p];
            }
            data[data.length]=d;
        }
        return data;
    },

    /// <summary>
    /// Action for creating the grid 
    /// </summary>
    /// <Remarks>
    CreateErrorGrid:function(result){   
        var fields=this.CreateFieldsAndModels(result);
        
        var store=new Ext.data.SimpleStore({ 
            data:this.GetDataFromServer(result.rows),
            fields:fields.fields
        });
        
        var grid=new Ext.grid.GridPanel({
            store:store,
            title:HRMSRes.Public_PagingToolbar_Total+':'+result.results,
            cm:new Ext.grid.ColumnModel(fields.model),
            border:true, 
            monitorResize:true, 
            loadMask:true,  		             
            viewConfig: { 
                forceFit: true 
            }
        });
        
        return grid;
    } 
}
    