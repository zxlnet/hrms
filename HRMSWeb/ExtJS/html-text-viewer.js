var HTMLTextViewer_Text;
var HTMLTextViewer_Rfid;

var HTMLTextViewerWindow=function(showSaveButton){
    this.showSaveButton = showSaveButton;
	this.init();	
	
	HTMLTextViewerWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:520, 
        height:380, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.dtlFormPanel,
        title:'Text Viewer',
        listeners:{
            show:function(){
                //var f = this.dtlForm.findField('noteText');
                //f.setValue(HTMLTextViewer_Text);
            },
            scope:this
        },
        buttons: [{ 
            text: HRMSRes.Public_Button_Confirm, 
            iconCls: 'icon-save', 
            hidden: !this.showSaveButton,
            handler: function(){    
                var f = this.dtlForm.findField('noteText');
                var note = f.getRawValue();
                this.updateNote(note,HTMLTextViewer_Rfid);
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

Ext.extend(HTMLTextViewerWindow,Ext.Window,{
    init:function(){
        if (this.showSaveButton)
        {
		    this.dtlFormPanel=this.createHTMLEditorFormPanel();
		    this.dtlForm=this.dtlFormPanel.getForm();
		}
		else
		{
		    this.dtlFormPanel=this.createLabelFormPanel();
		    this.dtlForm=this.dtlFormPanel.getForm();
		}
	},
	
	createLabelFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
	         items:[{
	    	    xtype:'panel',
	    	    height:300,
                html:"<div>" + HTMLTextViewer_Text + "</div>"
              }]  
       })
	},
	createHTMLEditorFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         labelWidth:120,
	         header:true,
             items: [
      		        {columnWidth:.5,layout: 'form',
                     items: [{xtype:'htmleditor',
                              fieldLabel:'',
                              name: 'noteText',
                              stateful:false,
                              height:300, 
                              width:400,
                              enableLinks:true,
                              enableLists:false, 
                              hideLabel: true,
                              labelSeparator :'',
                              value: '',
                              anchor:'95%'}]}
            ] 
       })
	},
	
	updateNote: function(note,rfid){
	    var params={};
	    params['rfid']= rfid;
	    params['note']= note;
	    
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/strecnot.mvc/updateNote',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		
		   		if (o.status=='success'){
		   		    Ext.MessageBox.show({
			            title: HRMSRes.Public_Message_Success,
			            msg:o.msg,
			            buttons: Ext.MessageBox.OK,
			            icon:Ext.MessageBox.INFO
		            });
		   		}else{
		   		    Ext.MessageBox.show({
			            title: HRMSRes.Public_Message_Error,
			            msg:o.msg,
			            buttons: Ext.MessageBox.OK,
			            icon:Ext.MessageBox.ERROR
		            });
		   		}
		   },
		   scope:this,
		   params: {record:Ext.util.JSON.encode(params)}
		});
    },   
	scope:this
});

function showHTMLTextViewer(){
    new HTMLTextViewerWindow().show();
}

function checkNote(rfid){
    getNote(rfid);
}

function getNote(rfid){
	    var params={};

	    params['rfid']= rfid;
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/strecnot.mvc/getNote',
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
                HTMLTextViewer_Text = o.msg;
                HTMLTextViewer_Rfid = rfid;

                new HTMLTextViewerWindow(true).show();
                
		   },
		   scope:this,
		   params: {record:Ext.util.JSON.encode(params)}
		});
}



