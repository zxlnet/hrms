Ext.BLANK_IMAGE_URL = ContextInfo.contextPath+'/ExtJS/resources/s.gif'; 

Ext.Ajax.abort();

Docs = {}; 
var win;


defaultTree=new Ext.tree.TreePanel({ 
    			loader: new Ext.tree.TreeLoader(), 
     			rootVisible:false, 
     			lines:false, 
     			autoScroll:true,    			
     			root: new Ext.tree.AsyncTreeNode({ 
           		}),
     	        listeners:{
     	        } 
 	});

 		

MenuPanel=function(){ 
MenuPanel.superclass.constructor.call(this,{ 
        id:'menu-tree', 
        region:'west', 
        width:200, 
        split:true,
        minSize: 200,
        maxSize: 350,  
        lines:false,
        autoScroll:true,
        animCollapse:false,
        animate: false,
        collapseMode:'mini',
        collapseFirst:false,
        layout:'accordion',         
      	items: [ 
     	    defaultTree 	
      ] 
    }); 
};  


Ext.extend(MenuPanel,Ext.Panel); 


DocPanel = Ext.extend(Ext.Panel, { 
    closable: true, 
    //autoScroll:true, 
		
    initComponent : function(){ 
        var ps = this.cclass.split('.'); 
        this.title = ps[ps.length-1]; 

        DocPanel.superclass.initComponent.call(this); 
  
    }, 

    scrollToMember : function(member){ 
        var el = Ext.fly(this.cclass + '-' + member); 
        if(el){ 
            var top = (el.getOffsetsTo(this.body)[1]) + this.body.dom.scrollTop; 
            this.body.scrollTo('top', top-25, {duration:.75, callback: this.hlMember.createDelegate(this, [member])}); 
        } 
    }, 

	scrollToSection : function(id){ 
		var el = Ext.getDom(id); 
		if(el){ 
			var top = (Ext.fly(el).getOffsetsTo(this.body)[1]) + this.body.dom.scrollTop; 
			this.body.scrollTo('top', top-25, {duration:.5}); 
		} 
	}, 

    hlMember : function(member){ 
        var el = Ext.fly(this.cclass + '-' + member); 
        if(el){ 
            el.up('tr').highlight('#cadaf9'); 
        } 
    } 
}); 


MainPanel = function(){ 

  this.init();

  this.formPanel = {
            xtype:'panel',
            layout:'fit',
            style: 'background:#FFFFFF',
            iconCls:'icon-dashboard', 
            border:false,
            title:'Message Center', 
            header:false,
            frame:false,
            items:this.grid
        };
    
  MainPanel.superclass.constructor.call(this, { 
        id:'doc-body', 
        region:'center', 
        margins:'0 5 5 0', 
        resizeTabs: true, 
        minTabWidth: 135, 
        tabWidth: 135, 
        plugins: new Ext.ux.TabCloseMenu(), 
        enableTabScroll: true, 
        activeTab: 0, 
        listeners:{
            resize:function(tab,adjWidth,adjHeight){              
                var ap=tab.getActiveTab();
                if(ap){
                    
                    var g=Ext.getCmp(ap.id+'_panel');
	                if(g){
	                    g.setWidth(adjWidth-2);
	                    var adHeight=29;
	                    var f=g.getComponent(0);
	                    if(f){
	                        var tbar=f.getTopToolbar();
	                        if(tbar){
	                            adHeight=tbar.getSize().height+2;
	                        }
	                    }
	                    g.setHeight(adjHeight-adHeight);
	                }
                }
            }
        },
        items:[{
	    	xtype:'panel',
	    	layout:'fit',  
	    	title:'Message Center', 
	    	autoScroll:true,      
	        tbar: [{ 
	        	id:this.tabId+ '_refresh',
	            iconCls:'icon-refresh', 
	            text: 'Refresh', 
	            handler: function(){
                    var params={
                        start:0,
                        limit:Pagination.pagingSize
                    };
                    
                    this.grid.store.load({params:params});
	            }, 
	            scope: this 
	        },'-',{ 
	        	id:this.tabId+ '_exportexcel',
	            iconCls:'icon-export', 
	            text:HRMSRes.Public_Toolbar_ToExcel, 
	            handler: this.exportExcel, 
	            scope: this 
	        }], 
	        items:this.grid
       }]
    }); 
}; 

Ext.extend(MainPanel, Ext.TabPanel, { 
    init:function(){
		this.grid=this.createGridPanel();
        this.refreshBoard(this.grid);
	},

    initEvents : function(){ 
        MainPanel.superclass.initEvents.call(this); 
    }, 

    refreshBoard: function(grid){
        Ext.TaskMgr.start({
            run:function(){
                var params={
                    start:0,
                    limit:Pagination.pagingSize
                };

                grid.store.load({params:params});
            },
            interval: 500000
    })},

    loadClass : function(href, cls, iconCls,text, member,forceLoad){        
        var id = cls; 
        var index=href.indexOf('helpId');
        var helpId='';
        if(index!=-1){            
            helpId=href.substring(index+7,href.length);
        }
        var tab = this.getComponent(id);   
         var autoLoad = {url: href,scripts:true};
        if(tab){ 
            this.setActiveTab(tab); 
            if(forceLoad){
                tab.load(autoLoad);
            }            
            if(member){ 
                tab.scrollToMember(member); 
            } 
        }else{   
            var openTabs=this.items.length;
            if(openTabs>Tabs.openedTabs){
                Ext.MessageBox.show({
                    title: HRMSRes.Public_Message_Error,
                    msg:HRMSRes.Public_Message_OpenTabsExceed+Tabs.openedTabs+')',
                    buttons: Ext.MessageBox.OK,
                    icon:Ext.MessageBox.ERROR
                });
                return;
            }       
            if(member){ 
                autoLoad.callback = function(){ 
                    Ext.getCmp(id).scrollToMember(member); 
                } 
            } 
            var p = this.add(new DocPanel({
                id: id,   
                helpId:helpId,             
                cclass : text, 
                autoLoad: autoLoad, 
                layout:'fit',
                iconCls: iconCls,
                style: 'background:#FFFFFF'
            })); 
            this.setActiveTab(p); 
        } 
    },
    
    renderURL:function(val,m,r,rowIndex,ds){
        var shortDesc = r.get('bdtx').substr(0,50) + '...';
        return '<a href=\'javascript:showHTMLTextViewer()\'>' + shortDesc + '</a>';
    },
    
    renderAction1:function(val,m,r,rowIndex,ds){
        return '<a href=\'javascript:handleMessage("' + r.get('adid') + '")\'>Handle</a>';
    },    
    
    renderAction2:function(val,m,r,rowIndex,ds){
        HTMLTextViewer_Text = '';
        return '<a href=\'javascript:checkNote("' + r.get('adid') + '")\'>Note</a>';
    },    
    
    createGridPanel:function(){
		var boardMessageStoreType=Ext.data.Record.create([
		    {name:'adid'},
		    {name:'subj'},{name:'reci'},{name:'bdtx'},
		    {name:'alst'},{name:'gpid'},{name:'crtm'},
            {name:'setm'},{name:'mtyp'},{name:'extm'},
            {name:'crur'}
		]);
		var store=new Ext.data.GroupingStore({ 
        	reader: new Ext.data.JsonReader({
	    		totalProperty: "results",
	    		root: "rows"               
	   	 	},boardMessageStoreType), 	
	   	 	groupField:'mtyp',
	   	 	sortInfo:{field: 'mtyp', direction: "ASC"},
	   	    baseParams:{record:Ext.encode({params:[{}]})},   
	   		url:ContextInfo.contextPath+'/staldlyb.mvc/listBoardMessage',
	   		listeners:{
                loadexception:function(o,t,response)
                {
                },
                load:function(){
                },
                scope:this
            }
        });

        var params={
            start:0,
            limit:Pagination.pagingSize
        };
        
        store.load({params:params});
        
        return new Ext.grid.EditorGridPanel({
    		border:true, 
    		monitorResize:true, 
            loadMask:true,  		            
            ds: store, 
            listeners:{
	            rowclick:function(t,r,e){
	                HTMLTextViewer_Text = t.getSelectionModel().getSelected().get('bdtx');
	            },
	            render:function(){
	            },
	            scope:this
            },    
            view: new Ext.grid.GroupingView({
                //forceFit:true,
                groupTextTpl: '{text} ({[values.rs.length]} {[values.rs.length > 1 ? "Items" : "Item"]})'
            }),
            sm: new Ext.grid.RowSelectionModel({singleSelect:true}),
            cm: new Ext.grid.ColumnModel([ 
                {header:'H',sortable: true, dataIndex: '',width:50,renderer:this.renderAction1},
                {header:'N',sortable: true, dataIndex: '',width:40,renderer:this.renderAction2},
                {header:'Category',sortable: true, dataIndex: 'mtyp',width:150,hidden:true},
                {header:'Subject',sortable: true, dataIndex: 'subj',width:250},
                {header:'Message body',sortable: true, dataIndex: 'bdtx',width:300,renderer:this.renderURL},
                {header:'Status',sortable: true, dataIndex: 'alst',width:100},
                {header:'Created time',sortable: true, dataIndex: 'crtm',renderer:formatDateNoTime,width:100},
                {header:'Creator',sortable: true, dataIndex: 'crur',width:100},
                {header:'Expired at',sortable: true, dataIndex: 'extm',renderer:formatDateNoTime,width:100}
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
    
}); 

var mainPanel = new MainPanel();

Ext.onReady(function() {

    Ext.QuickTips.init();

    infoPanel = new Ext.Panel({
        title: 'System Information',
        iconCls: 'icon-ocean',
        hidden: 'true',
        html: '<p style=padding:5px>Not implementation<br></p>'
    });

    var period = ContextInfo.currentPeriod.fullPeriod;
    var periodStart = ContextInfo.currentPeriod.start;
    var periodEnd = ContextInfo.currentPeriod.end;

    var hd = new Ext.Panel({
        border: false,
        layout: 'anchor',
        region: 'north',
        cls: 'docs-header',
        height: 60,
        items: [{
            xtype: 'box',
            el: 'header',
            border: false,
            anchor: 'none -25'
        },
        new Ext.Toolbar({
            cls: 'top-toolbar',
            items: [' ',
			{
			    text: HRMSRes.Public_Button_User + ':' + ContextInfo.currentUser,
			    iconCls: 'icon-user',
			    tooltip: 'Current logon user, click to check in more detail.', tooltipType: 'title',
			    handler: function() {
			    mainPanel.loadClass(ContextInfo.contextPath + '/stusrinf.mvc/index?menuId=MZ050', 'MZ050', '', HRMSRes.Public_Menu_stusrinf);
			    }
			}, '-',
		    {
		        text: "<b><font color='blue'>" + HRMSRes.Public_Button_CurrentPeriod + ': ' + period + '</font></b> [' + periodStart + ' - ' + periodEnd + ']',
		        tooltip: 'Current payroll period, click to go to period mgt page.', tooltipType: 'title',
		        handler: function() {
		            mainPanel.loadClass(ContextInfo.contextPath + '/stperiod.mvc/index?menuId=MZ040', 'MZ040', '', HRMSRes.Public_Menu_stperiod);
		        }
		    }, '-',
		    {
		        text: "<b><font color='darkyellow'>" + HRMSRes.Public_Button_Search + '</font></b>',tooltip:'Click to search staff information in system.',tooltipType:'title',
		        handler: function() {
		            mainPanel.loadClass(ContextInfo.contextPath + '/stsearch.mvc/index?menuId=MZ060', 'MZ060', '', HRMSRes.Public_Menu_stsearch);
		        }
		    }, '->',
            {
                text: "<b><font color='red'>" + ContextInfo.currentEnvironment + '</font></b>',
                tooltip: 'System environment.', tooltipType: 'title',
                handler: function() {
                },
                scope: this
            }, '-',
            {
                text: 'Logout',
                hidden: ContextInfo.sysCfg['ScSBAD'] == "Y" ? true : false,
                tooltip: 'Logout from current user.', tooltipType: 'title',
                handler: function() {
                    var form = document.createElement('form');
                    form.name = 'chinaForm';
                    form.method = 'post';
                    form.action = ContextInfo.contextPath + '/logon.mvc/logout';
                    document.body.appendChild(form);
                    form.submit();
                    document.body.removeChild(form);
                },
                scope: this
            },
            {
                text: HRMSRes.Public_Button_HROpen, tooltip: 'Sub-system based on logon user.', tooltipType: 'title',
                handler: function() {
                    location.href('http://localhost:2222');
                },
                scope: this
            }, '-',
            {
                text: HRMSRes.Public_Button_Authorization, tooltip: 'Click to go to authorization web-site.', tooltipType: 'title',
                handler: function() {
                    location.href('http://localhost:1111');
                },
                scope: this
            }, '-',
            {
                text: HRMSRes.Public_Button_Chinese, tooltip: 'Click system language to chinese.', tooltipType: 'title',
                handler: function() {
                    var form = document.createElement('form');
                    form.name = 'chinaForm';
                    form.method = 'post';
                    form.action = ContextInfo.contextPath + '/Home.mvc/changeLang';
                    var hd = document.createElement('input');
                    hd.type = 'hidden';
                    hd.name = 'lang';
                    hd.value = 'zh-cn';
                    form.appendChild(hd);
                    document.body.appendChild(form);
                    form.submit();
                    document.body.removeChild(form);
                },
                scope: this
            }, '-',
            {
                text: HRMSRes.Public_Button_English, tooltip: 'Change system language to english.', tooltipType: 'title',
                id: 'en',
                handler: function() {
                    var form = document.createElement('form');
                    form.name = 'englishForm';
                    form.method = 'post';
                    form.action = ContextInfo.contextPath + '/Home.mvc/changeLang';
                    var hd = document.createElement('input');
                    hd.type = 'hidden';
                    hd.name = 'lang';
                    hd.value = 'en';
                    form.appendChild(hd);
                    document.body.appendChild(form);
                    form.submit();
                    document.body.removeChild(form);
                },
                scope: this
            }, '-',
            {
                text: HRMSRes.Public_Button_Help, tooltip: 'Online help of active window.', tooltipType: 'title',
                handler: function() {
                    var activateTab = mainPanel.getActiveTab();
                    if (!activateTab || activateTab.id == 'main') {
                        return;
                    }
                    var tabId = activateTab.helpId;
                    var win = new Ext.Window({
                        layout: 'fit',
                        title: HRMSRes.Public_Help_WindowTitle,
                        width: 500,
                        height: 400,
                        buttonAlign: 'center',
                        closeAction: 'close',
                        plain: true,
                        items: { tag: 'div', id: tabId + '_helpWindow' },
                        listeners: {
                            show: function() {
                                Ext.get(tabId + '_helpWindow').load({
                                    url: ContextInfo.contextPath + '/help.mvc/help',
                                    params: { record: tabId }
                                });
                            }
                        },
                        buttons: [{
                            text: HRMSRes.Public_Button_Close,
                            handler: function() {
                                win.close();
                            }
}]
                        });
                        win.show();
                    }
}]
                })]
            });
            Ext.Ajax.request({
                url: ContextInfo.contextPath + '/menuconfig.mvc/listTopMenu',
                success: function(response) {
                    var api = new MenuPanel();
                    api.remove(defaultTree);
                    var o = Ext.util.JSON.decode(response.responseText);
                    for (var i = 0; i < o.length; i++) {
                        var mTree = new Ext.tree.TreePanel({
                            title: o[i].munm,
                            loader: new Ext.tree.TreeLoader({
                                dataUrl: ContextInfo.contextPath + '/menuconfig.mvc/getMenuTree',
                                baseParams: { pami: o[i].muid },
                                requestMethod: "GET"
                            }),
                            rootVisible: false,
                            lines: false,
                            autoScroll: true,
                            iconCls: 'icon-ocean',
                            stateful: false,
                            listeners: {
                                click: function(node, e) {
                                    e.stopEvent();
                                    mainPanel.loadClass(ContextInfo.contextPath + node.attributes.href, node.id, node.attributes.iconCls, node.attributes.text);
                                },
                                expand: function(p) {
                                    p.syncSize();
                                }
                            },
                            root: new Ext.tree.AsyncTreeNode()
                        });
                        api.add(mTree);
                    }
                    api.add(infoPanel);
                    var viewport = new Ext.Viewport({
                        layout: 'border',
                        items: [hd, api, mainPanel]
                    });

                    viewport.doLayout();
                },
                failure: function(response) {
                    Ext.MessageBox.show({
                        title: HRMSRes.Public_Message_Error,
                        msg: response.responseText,
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.ERROR
                    });
                },
                scope: this
            });

            setTimeout(function() {
                Ext.get('loading').remove();
                Ext.get('loading-mask').fadeOut({ remove: true });
            }, 250);


        }); 
