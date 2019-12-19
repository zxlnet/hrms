var expression_builder_text_start=0;
var expression_builder_text_end=0;	
this.ebTextResult;
this.ebValueResult; 

var exprbuilderWindow=function(grid,rec,fn,value){
	this.dtlgrid=grid;
	this.init();
	this.feekbackCtl = rec;
	this.feekbackFn = fn;
	
    this.fullFormPanel = {
                xtype:'panel',
	            layout:'form',
	            autoHeight:true,
	            border:false,
	            frame:false,
	            items:[
	                this.formButtonPanel,
	                this.formOPPanel//,
	                //this.formPanel
	            ]
            };

	
	exprbuilderWindow.superclass.constructor.call(this,{
        layout:'fit', 
        width:600, 
        height:435, 
        closeAction:'close', 
        modal: true, 
        resizable: false, 
        buttonAlign: 'center', 
        items: this.fullFormPanel,
        title:HRMSRes.Public_Title_exbu,
        buttons: [{ 
            text:HRMSRes.Public_Toolbar_CleanQuery, 
            iconCls:'icon-clean', 
            handler: this.clean,
            scope: this
        },{ 
            text:HRMSRes.Public_Button_Confirm, 
            iconCls:'icon-accept', 
            handler: this.Confirm,
            scope: this
        },{ 
            text: HRMSRes.Public_Button_Close, 
            iconCls: 'icon-exit', 
            handler: function(){                  
                this.close();
            },
            scope:this
        }],
       listeners:{
            show:function(){
                var f1 = this.formOP.findField('expressionBuilderText');
                f1.setValue(value);
                
                this.registerEvents();
            },
            scope:this
       }
	})
	

}

Ext.extend(exprbuilderWindow,Ext.Window,{
    init:function(){
		//this.formPanel=this.createFormPanel();
		//this.form=this.formPanel.getForm();
		
		this.formOPPanel=this.createOPFormPanel();
		this.formOP=this.formOPPanel.getForm();
		
		this.formButtonPanel=this.createButtonFormPanel();
		this.formButton=this.formButtonPanel.getForm();
		
	},
	
	createOPFormPanel:function(){	        
		return new Ext.FormPanel({   
	         labelWidth:120,
	         header:true,
	         frame:true,
	         border:false,
	         layout:'column',
             items: [
                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_sfat ,
                        name: 'empatt',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getChangeField'}),
		                listeners:{select:function(p,r){this.update2(p,r)},scope:this}
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_vari ,
                        name: 'vari',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getVariables'}),
		                listeners:{select:function(p,r){this.update2(p,r)},scope:this}
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_sait ,
                        name: 'salitm',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: new Ext.data.Store({ 
		                    reader: new Ext.data.JsonReader({totalProperty: "results",root: "rows",
			                    fields: ['ValueField','DisplayField']}), autoLoad:true,
		                    url:ContextInfo.contextPath+'/dropdown.mvc/getSalaryItems'}),
		                listeners:{select:function(p,r){this.update2(p,r)},scope:this}
    		          }]},

                    {columnWidth:.5,layout: 'form',
                    items: [{xtype:'combo',fieldLabel:HRMSRes.Payroll_Label_func ,
                        name: 'chfn',stateful:false,typeAhead: true,
                        triggerAction: 'all',mode: 'local',maxHeight:150,anchor:'95%',
                        displayField: 'DisplayField',valueField:'ValueField',
                        store: OperatorTypeStore,
                        listeners:{select:function(p,r){this.update3(p,r)},scope:this}
    		          }]},
    		          
                    {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Payroll_Label_cdtx,
                        id:'expressionBuilderText',stateful:false,anchor:'98%',height:130}]},

       		        {columnWidth:1,layout: 'form',
                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Payroll_Label_cdva ,disabled:true,
                        id:'expressionBuilderValue',stateful:false,anchor:'98%',height:130}]}    		          

            ] 
       })
	},	
//	createFormPanel:function(){	        
//		return new Ext.FormPanel({   
//	         frame:true, 
//	         labelWidth:120,
//	         header:false,
//	         border:false,
//             items: [
////       		        {columnWidth:1,layout: 'form',
////                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Payroll_Label_cdtx,
////                        id:'expressionBuilderText',stateful:false,anchor:'98%',height:130}]},

////       		        {columnWidth:1,layout: 'form',
////                     items: [{xtype:'textarea',fieldLabel:HRMSRes.Payroll_Label_cdva ,disabled:true,
////                        id:'expressionBuilderValue',stateful:false,anchor:'98%',height:130}]}
//            ] 
//       })
//	},	
    createButtonFormPanel:function(){	        
		return new Ext.FormPanel({   
	         frame:true, 
	         header:true,
	         border:false,
             items: [{
                 layout:'column',
                 items:[{layout:'form',columnWidth:.08,items:[{xtype:'button',text:'+',id:'plus',name: 'plus',stateful:false,listeners:{click:function(p){this.update1(p)},scope:this}}]},
                    {layout:'form',columnWidth:.08,items:[{xtype:'button',text:'-',id:'minus',name: 'minus',stateful:false,listeners:{click:function(p){this.update1(p)},scope:this}}]},            
                    {layout:'form',columnWidth:.08,items:[{xtype:'button',text:'*',id:'multi',name: 'multi',stateful:false,listeners:{click:function(p){this.update1(p)},scope:this}}]},            
                    {layout:'form',columnWidth:.08,items:[{xtype:'button',text:'/',id:'divis',name: 'divis',stateful:false,listeners:{click:function(p){this.update1(p)},scope:this}}]},            
                    {layout:'form',columnWidth:.08,items:[{xtype:'button',text:'(',id:'left_parentheses',name: 'left_parentheses',stateful:false,listeners:{click:function(p){this.update1(p)},scope:this}}]},            
                    {layout:'form',columnWidth:.08,items:[{xtype:'button',text:')',id:'right_parentheses',name: 'right_parentheses',stateful:false,listeners:{click:function(p){this.update1(p)},scope:this}}]},            
                    {layout:'form',columnWidth:.08,items:[{xtype:'button',text:' =',id:'equal',name: 'equal',stateful:false,listeners:{click:function(p){this.update1(p)},scope:this}}]},            
                    {layout:'form',columnWidth:.08,items:[{xtype:'button',text:'\'',id:'quotation',name: 'quotation',stateful:false,listeners:{click:function(p){this.update1(p)},scope:this}}]},            
                    {layout:'form',columnWidth:.08,items:[{xtype:'button',text:'And',id:'and',name: 'and',stateful:false,listeners:{click:function(p){this.update1(p)},scope:this}}]},            
                    {layout:'form',columnWidth:.08,items:[{xtype:'button',text:'Or',id:'or',name: 'or',stateful:false,listeners:{click:function(p){this.update1(p)},scope:this}}]}
            ]}] 
       })
	},	
	addtext:function(v){        
        var ctl = Ext.getCmp("expressionBuilderText");
        
        var pre = ctl.getValue().substr(0, expression_builder_text_start);
        var post = ctl.getValue().substr(expression_builder_text_end);
        ctl.setValue(pre + v + post);

        expression_builder_text_start = pre.length + v.length;
        expression_builder_text_end = pre.length + v.length;
    },
    savePos:function(){
        var ctl = Ext.getDom("expressionBuilderText");
        if(typeof(ctl.selectionStart) == "number"){
           expression_builder_text_start = ctl.selectionStart;
           expression_builder_text_end = ctl.selectionEnd;
        }
        else if(document.selection){
            var range = document.selection.createRange();
            if(range.parentElement().id == ctl.id){
                var range_all = document.body.createTextRange();
                range_all.moveToElementText(ctl);
                for (expression_builder_text_start=0; range_all.compareEndPoints("StartToStart", range) < 0; expression_builder_text_start++)
                    range_all.moveStart('character', 1);
                for (var i = 0; i <= expression_builder_text_start; i ++){
                    if (ctl.value.charAt(i) == '\n')
                        expression_builder_text_start++;
                }
                 var range_all = document.body.createTextRange();
                 range_all.moveToElementText(ctl);
                 for (expression_builder_text_end = 0; range_all.compareEndPoints('StartToEnd', range) < 0; expression_builder_text_end++)
                     range_all.moveStart('character', 1);
                 for (var i = 0; i <= expression_builder_text_end; i ++){
                     if (ctl.value.charAt(i) == '\n')
                         expression_builder_text_end++;
                 }
             }
        }
        Ext.getCmp("expressionBuilderValue").setValue(expression_builder_text_start);
    },
	registerEvents:function(){
            Ext.get('expressionBuilderText').on('click',this.savePos);
            Ext.get('expressionBuilderText').on('keydown',this.savePos);
            Ext.get('expressionBuilderText').on('keyup',this.savePos);
            Ext.get('expressionBuilderText').on('mousedown',this.savePos);
            Ext.get('expressionBuilderText').on('mouseup',this.savePos);
            Ext.get('expressionBuilderText').on('focus',this.savePos);
	},	
    update1:function(p){
        var f1 = this.formOP.findField('expressionBuilderValue');
        var f2 = this.formOP.findField('expressionBuilderText');
        
        this.addtext(p.getText());
    },
    update2:function(p,r){
        var f1 = this.formOP.findField('expressionBuilderValue');
        var f2 = this.formOP.findField('expressionBuilderText');
        
        this.addtext('{' + r.get('DisplayField') + '}');
    },
    update3:function(p,r){
        var f1 = this.formOP.findField('expressionBuilderValue');
        var f2 = this.formOP.findField('expressionBuilderText');
        
        this.addtext(r.get('DisplayField'));
    },
	Confirm:function(){	   
        var f1 = this.formOP.findField('expressionBuilderText');
        var f2 = this.formOP.findField('expressionBuilderValue');
        this.ebTextResult = f1.getValue();
        this.ebValueResult= f2.getValue();
        
        this.feekbackCtl.set(this.feekbackFn,this.ebTextResult);
        this.close();
	},
	clean:function(){
        var f1 = this.formOP.findField('expressionBuilderText');
        var f2 = this.formOP.findField('expressionBuilderValue');
        f1.setValue('');
        f2.setValue('');
	},
	scope:this
});

Ext.onReady(function() {
});



