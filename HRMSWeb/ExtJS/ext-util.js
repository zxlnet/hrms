Ext.grid.CheckColumn = function(config){
    Ext.apply(this, config);
    if(!this.id){
        this.id = Ext.id();
    }
    this.renderer = this.renderer.createDelegate(this);
};

Ext.grid.CheckColumn.prototype ={
    init : function(grid){
        this.grid = grid;
        this.grid.on('render', function(){
            var view = this.grid.getView();
            view.mainBody.on('mousedown', this.onMouseDown, this);
        }, this);
    },

    onMouseDown : function(e, t){
        if(t.className && t.className.indexOf('x-grid3-cc-'+this.id) != -1){
            e.stopEvent();
            var index = this.grid.getView().findRowIndex(t);
            var record = this.grid.store.getAt(index);
            //record.set(this.dataIndex, !record.data[this.dataIndex]);
            
            var x = record.data[this.dataIndex]=='Y'?'N':'Y';           
            record.set(this.dataIndex, x);
        }
    },

    renderer : function(v, p, record){
        var x = v=='Y'?true:false;
        p.css += ' x-grid3-check-col-td'; 
        return '<div class="x-grid3-check-col'+(x?'-on':'')+' x-grid3-cc-'+this.id+'">&#160;</div>';
    }
};

Ext.app.SearchField = Ext.extend(Ext.form.TwinTriggerField, {
    initComponent : function(){
        if(!this.store.baseParams){
	        this.store.baseParams = {};
        }
        Ext.app.SearchField.superclass.initComponent.call(this);
        this.on('specialkey', function(f, e){
            if(e.getKey() == e.ENTER){
                this.onTrigger2Click();
            }
        }, this);
    },

    validationEvent:false,
    validateOnBlur:false,
    trigger1Class:'x-form-clear-trigger',
    trigger2Class:'x-form-search-trigger',
    hideTrigger1:true,
    width:180,
    hasSearch : false,
    paramName : 'query',

    onTrigger1Click : function(){
        if(this.hasSearch){
            this.store.baseParams[this.paramName] = '';
	        this.store.removeAll();
	        this.el.dom.value = '';
            this.triggers[0].hide();
            this.hasSearch = false;
	        this.focus();
        }
    },

    onTrigger2Click : function(){
        var v = this.getRawValue();
        this.store.baseParams[this.paramName] = v;
        var o = {start: 0};
        this.store.reload({params:o});
        this.hasSearch = true;
        this.triggers[0].show();
        this.focus();
    }
});