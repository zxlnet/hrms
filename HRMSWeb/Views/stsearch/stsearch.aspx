<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="stsearch.aspx.cs" Inherits="GotWell.HRMS.HRMSWeb.Views.stsearch.stsearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
<script type="text/javascript"> 
Ext.Ajax.timeout = 1000*3600;

var stsearchConfig=Ext.decode('<%=ViewData["config"] %>'); 
stsearchConfig.auth=Ext.decode('<%=ViewData["authorization"] %>');
var stsearchPageName = 'stsearch';

var stsearchPanel = function() {
    this.tabId = stsearchConfig.tabId;
    this.init();

    stsearchPanel.superclass.constructor.call(this, {
        applyTo: this.tabId,
        layout: 'fit',
        id: this.tabId + '_panel',
        monitorResize: true,
        items: [{
            xtype: 'panel', layout: 'fit',
            tbar: ['Search: ', ' ',
                   new Ext.form.TriggerField({ typeAhead: true, triggerAction: 'all', id: 'search_textbox', triggerClass: 'x-form-search-trigger',
                       onTriggerClick: this.search, width: 300,style:'padding: 2px 2px 2px 2px'
                   }),
                    ' ',
                   new Ext.form.Radio({ boxLabel: 'By staff', id: 'rbbystaff', inputValue: 'Y', checked: true,
                       listeners: {
                           check: function(t, c) {
                               if (c) {
                                   var f = Ext.getCmp('rbbyothers').setValue(false);
                               }
                           }
                       }
                   }), '  ',
                    new Ext.form.Radio({ boxLabel: 'By others', id: 'rbbyothers', inputValue: 'Y',
                        listeners: {
                            check: function(t, c) {
                                if (c) {
                                    var f = Ext.getCmp('rbbystaff').setValue(false);
                                }
                            }
                        }
                    }), ''
            //'(search by staffid,staff name,english name,id)'
            ],
            items: this.grid}]
        })
    }

    var expander = new Ext.grid.RowExpander({
        tpl: new Ext.Template(
            '<p>{text}</p><br>'
        ),lazyRender:false
    })

    Ext.extend(stsearchPanel, Ext.Panel, {
        init: function() {
            this.grid = this.createGridPanel();
        },

        renderTopic: function(value, p, record) {
            return String.format(
                '<a href="javascript:mainPanel.loadClass(\'{0}\',\'{1}\',\'\',\'{2}\')">' + '<span style=\'color:blue\'>{3}</span>' + '</a>',
                record.get("url"), record.get("menu"), record.get("ttds"), record.get("ttle"));
        },

        createGridPanel: function() {
            var stsearchStoreType = Ext.data.Record.create([
            { name: 'emno' },
            { name: 'url' },
            { name: 'menu' },
            { name: 'ttds' },
            { name: 'ttle' },
            { name: 'text' }
		]);

            var store = new Ext.data.Store({
                reader: new Ext.data.JsonReader({
                    totalProperty: "results",
                    root: "rows"
                }, stsearchStoreType),
                baseParams: { record: Ext.encode({ params: [{ ColumnName: '', ColumnValue: ''}] }) },
                url: ContextInfo.contextPath + '/stsearch.mvc/search',
                listeners: {
                    loadexception: function(o, t, response) {
                        var o = Ext.util.JSON.decode(response.responseText);
                        this.grid.getBottomToolbar().diplayMsg.update(o.msg);
                    },
                    load: function() {
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
                id: 'searchgrid',
                monitorResize: true,
                loadMask: true,
                border: false,
                ds: store,
                plugins: expander,
                collapsible: false,
                animCollapse: false,
                renderTo: document.body,
                viewConfig: {
                    forceFit: true
                },
                listeners: {
                    rowclick: function() {
                    },
                    scope: this
                },
                cm: new Ext.grid.ColumnModel([
                expander,
                { header: 'Search Result', sortable: true, dataIndex: 'emno', renderer: this.renderTopic }
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
        search: function() {
            //this.grid.getBottomToolbar().diplayMsg.update('');

            var params = [];
            params[0] = Ext.getDom('search_textbox').value;

            params[1] = Ext.getDom('rbbystaff').checked ? "Y" : "N";

            if (params[0].trim() == '') {
                Ext.MessageBox.show({
                    title: HRMSRes.Public_Message_Error,
                    msg: 'No keywords to search. Please input keyword first.',
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.ERROR
                });
                return;
            }

            var grid = Ext.getCmp('searchgrid');

            var loadParams = { start: 0, limit: Pagination.pagingSize };

            /*grid.queryParams = {
            params: params
            };*/

            grid.getStore().baseParams = { record: params[0], mode: params[1] };
            grid.getStore().load({ params: loadParams });
        },
        scope: this
    })


Ext.onReady(function(){ 
    new stsearchPanel();
})

    </script>

</body>
</html>
