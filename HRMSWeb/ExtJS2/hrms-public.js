/*
*Define public variables&constants 
*/

var EmpAdvQryReturnParams;

DATE_FORMAT={
    DATEONLY:'Y-m-d',
    DATE_ONLY_WITH_SPILITOR_1: 'Y-m-d',
    DATETIME:'Y-m-d G:i:s',
    DATETIME_NOSECOND:'Y-m-d G:i',
    TIMEONLY:'G:i:s',
    TIMENOSECOND:'G:i',
    YEARONLY:'Y'
}

PeriodStatus={
    Open:'Open',
    Closed:'Closed',
    Unused:'Unused'
}

Pagination={
    pagingSize:50
}

Tabs={
    openedTabs:5
}


CompletedFlag={
    Yes:'Y',
    No:'N'
}

LockedFlag={
    Yes:'Y',
    No:'N'
}

ValueType={
    Value:'Value',
    Formula:'Formula',
    Sum:'Sum',
    Customization:'Customization'
}


FlagYesNoStore = new Ext.data.SimpleStore({
    fields: ['value','text'],
    data : [
        ['Y','Yes'],
        ['N','No']
    ]
});

FlagValidStore = new Ext.data.SimpleStore({
    fields: ['value','text'],
    data : [
        ['Valid',HRMSRes.Public_Label_Valid],
        ['Invalid',HRMSRes.Public_Label_Invalid]
    ]
});

LimitStore = new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['Year',HRMSRes.Public_Label_YearLimit],
        ['Month',HRMSRes.Public_Label_MonthLimit],
        ['Week',HRMSRes.Public_Label_WeekLimit]    
    ]
});

LeaveStatusStore = new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['1',HRMSRes.Public_Label_Unapproved],
        ['2',HRMSRes.Public_Label_Approved],
        ['3',HRMSRes.Public_Label_Rejected]
    ]
});

OvertimeStatusStore = new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['Unapproved',HRMSRes.Public_Label_Unapproved],
        ['Approved',HRMSRes.Public_Label_Approved],
        ['Rejected',HRMSRes.Public_Label_Rejected]
    ]
});

MaritalStatusStore = new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['1',HRMSRes.Public_Label_Married],
        ['2',HRMSRes.Public_Label_Single],
        ['3',HRMSRes.Public_Label_Bigamous],
        ['4',HRMSRes.Public_Label_Divorced],
        ['5',HRMSRes.Public_Label_Unknown]
    ]
});

BloodTypeStore = new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['1','A'],
        ['2','B'],
        ['3','AB'],
        ['4','O'],
        ['5',HRMSRes.Public_Label_Unknown]
    ]
});

SexStore = new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['M',HRMSRes.Public_Label_Male],
        ['F',HRMSRes.Public_Label_Female]
    ]
});

SaluStore = new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['1',HRMSRes.Public_Label_Mr],
        ['2',HRMSRes.Public_Label_Ms]
    ]
});

EmpStateStore= new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['1',HRMSRes.Public_Label_OTJ],
        ['2',HRMSRes.Public_Label_Terminated]
    ]
});

EarylyStatStore= new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['N',HRMSRes.Attendance_Label_ontm],
        ['W',HRMSRes.Attendance_Label_wegh]
    ]
});

LateStatStore= new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['N',HRMSRes.Attendance_Label_ontm],
        ['W',HRMSRes.Attendance_Label_wegh]
    ]
});

RosterRestTypeStore= new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['Calendar',HRMSRes.Attendance_Label_caon],
        ['Roster',HRMSRes.Attendance_Label_rson],
        ['Both',HRMSRes.Attendance_Label_both]
    ]
});

SalaryItemOPTypeStore= new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['A','Addition'],
        ['D','Deduction'],
        ['P','Display']
    ]
});

VariableValueTypeStore= new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['S','String'],
        ['D','Datetime'],
        ['N','Number']
    ]
});

FormulaFuncTypeStore= new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['N/A','N/A'],
        ['Sum','Sum'],
        ['Count','Count'],
        ['Max','Max'],
        ['Min','Min'],
        ['Avg','Avg']
    ]
});

OperatorTypeStore= new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['=','='],
        ['<','<'],
        ['<=','<='],
        ['>','>'],
        ['>=','>='],
        ['>=','>='],
        ['<>','<>'],
        ['In','In'],
        ['Like','Like'],
        ['Not In','Not In'],
        ['Exists','Exists'],
        ['Not Exists','Not Exists']
    ]
});

RuleSetValueTypeStore= new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['Value','Value'],
        ['Formula','Formula'],
        ['Sum','Sum'],
        ['Customization','Customization']
    ]
});

AllocateValueTypeStore= new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['Value','Value'],
        ['Percentage','Percentage']
    ]
});

ExcelReportTypeStatStore= new Ext.data.SimpleStore({
    fields: ['ValueField','DisplayField'],
    data : [
        ['Personnel','Personnel'],
        ['Attendance','Attendance'],
        ['Payroll','Payroll'],
        ['PayrollBankAlloc','PayrollBankAlloc'],
        ['PayrollCCAlloc','PayrollCCAlloc'],
        ['Leave','Leave'],
        ['Overtime','Overtime']
    ]
});

PeriodStatusData = [
        [PeriodStatus.Open, PeriodStatus.Open],
        [PeriodStatus.Closed, PeriodStatus.Closed],
        [PeriodStatus.Unused, PeriodStatus.Unused]
    ];            


function getMaxsqno(emno,tableName,form){
	    var params={};
	    params['tablename']= tableName;
	    params['emno']= emno;
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/public.mvc/GetMaxsqno',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		
		   		if (o.status=='success'){
                    form.findField("sqno").setValue(o.msg);
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
}

function getMaxNo(tableName,field,form,control){
	    var params={};
	    params['tablename']= tableName;
	    params['field']= field;
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/public.mvc/GetMaxNo',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		
		   		if (o.status=='success'){
                    control.setValue(o.msg);
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
}


function setLastModifiedInfo(config,form){
        var modifiedTimeField = form.findField('lmtm');
        var d = new Date();
        modifiedTimeField.setValue(Ext.util.Format.dateRenderer(DATE_FORMAT.DATETIME)(d));

        var modifierField = form.findField('lmur');       
        modifierField.setValue(config.currentUser);
}

function setCreatedInfo(config,form){
        var modifiedTimeField = form.findField('createdtime');
        var d = new Date();
        modifiedTimeField.setValue(Ext.util.Format.dateRenderer(DATE_FORMAT.DATETIME)(d));

        var modifierField = form.findField('createdby');       
        modifierField.setValue(config.currentUser);
}

function formatDate(dd,p,rec){
    var x = new RegExp('/','g');//replace all
    if (dd==null) return;
    
    if (typeof(dd)=='object') {//Date
        return '' + Ext.util.Format.dateRenderer(DATE_FORMAT.DATEONLY)(dd) + '';
    }
    else{
   
        if (dd.indexOf('Date')>0){
            x = eval('new '+ dd.replace(x,''));
            return ''+Ext.util.Format.dateRenderer(DATE_FORMAT.DATETIME)(x)+'';
        }
        else{
            return ''+ dd +'';
        }
    }
}

function formatTime(dd,p,rec){
    var x = new RegExp('/','g');//replace all
    if (dd==null) return;
    
    if (typeof(dd)=='object') {//Date
        return ''+Ext.util.Format.dateRenderer(DATE_FORMAT.TIMEONLY)(dd)+'';
    }
    else{
   
        if (dd.indexOf('Date')>0){
            x = eval('new '+ dd.replace(x,''));
            return ''+Ext.util.Format.dateRenderer(DATE_FORMAT.TIMEONLY)(x)+'';
        }
        else{
            return ''+ dd +'';
        }
    }
}

function formatDateNoTime(dd,p,rec){
    var x = new RegExp('/','g');//replace all
    if (dd==null) return;

    if (typeof(dd)=='object') {//Date
        return '' + Ext.util.Format.dateRenderer(DATE_FORMAT.DATEONLY)(dd) + '';
    }
    else{
        if (dd.indexOf('Date')>0){
            x = eval('new '+ dd.replace(x,''));
            return '' + Ext.util.Format.dateRenderer(DATE_FORMAT.DATEONLY)(x) + '';
        }
        else{
            return ''+ dd +'';
        }
    }
}

function formatDateTimeNoSecond(dd,p,rec){
    var x = new RegExp('/','g');//replace all
    if (dd==null) return;

    if (typeof(dd)=='object') {//Date
        return '' + Ext.util.Format.dateRenderer(DATE_FORMAT.DATETIME_NOSECOND)(dd) + '';
    }
    else{
        if (dd.indexOf('Date')>0){
            x = eval('new '+ dd.replace(x,''));
            return '' + Ext.util.Format.dateRenderer(DATE_FORMAT.DATETIME_NOSECOND)(x) + '';
        }
        else{
            return ''+ dd +'';
        }
    }
}


function formatDateCustom(dd, p, rec,formatstr) {
    var x = new RegExp('/', 'g'); //replace all
    if (dd == null) return;

    if (typeof (dd) == 'object') {//Date
        return '' + Ext.util.Format.dateRenderer(formatstr)(dd) + '';
    }
    else {
        if (dd.indexOf('Date') > 0) {
            x = eval('new ' + dd.replace(x, ''));
            return '' + Ext.util.Format.dateRenderer(formatstr)(x) + '';
        }
        else {
            return '' + dd + '';
        }
    }
}


function formatDateTime(dd,p,rec){
    var x = new RegExp('/','g');//replace all
    if (dd==null) return;

    if (typeof(dd)=='object') {//Date
        return ''+Ext.util.Format.dateRenderer(DATE_FORMAT.DATETIME)(dd)+'';
    }
    else{
        if (dd.indexOf('Date')>0){
            x = eval('new '+ dd.replace(x,''));
            return ''+Ext.util.Format.dateRenderer(DATE_FORMAT.DATETIME)(x)+'';
        }
        else{
            return ''+ dd +'';
        }
    }
}

function formatTimeOnly(dd,p,rec){
    var x = new RegExp('/','g');//replace all
    if (dd==null) return;

    if (typeof(dd)=='object') {//Date
        return ''+Ext.util.Format.dateRenderer(DATE_FORMAT.TIMEONLY)(dd)+'';
    }
    else{
        if (dd.indexOf('Date')>0){
            x = eval('new '+ dd.replace(x,''));
            return ''+Ext.util.Format.dateRenderer(DATE_FORMAT.TIMEONLY)(x)+'';
        }
        else{
            return ''+ dd +'';
        }
    }
}

function formatCheckBox(val){
    if(val=='Y'){
        return '<input type="checkbox" name="uid"  checked></input>';
    }else{
        return '<input type="checkbox" name="uid"></input>';
    }
}

function cleanQueryCriterias(form){
    form.items.each(function(f){
        if(f.isFormField){
            f.setValue('');                    	            
        }
    });   
}

function updateMUF(muid,action,conf,grid){
	    var params={};
	    params['muid']= muid;
	    params['action']= action;
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/menuconfig.mvc/updatemuf',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		
		   		if (o.status=='success'){
                    var f = Ext.getCmp(muid+'_muf');
                    if (conf.muf)
                        f.setIconClass('icon-mufadd');
                    else
                        f.setIconClass('icon-mufdelete');
                    
                    conf.muf = !conf.muf;
                    
                    if (grid!=null)
                        grid.getBottomToolbar().diplayMsg.update(o.msg);
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
}

function confirmRecord(grid){
	    var params={};
	    var data=grid.getSelectionModel().getSelected();
	    if (data==null) return; 
	    if (data.get('stus')=="Confirmed")
	    {
	        grid.getBottomToolbar().diplayMsg.update("The record already is confirmed.");
	        return;
	    }
	    
	    var rfid = data.get('rfid');
	    
	    params['rfid']= rfid;
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/public.mvc/confirmrecord',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		
		   		if (o.status=='success'){
                    grid.getBottomToolbar().diplayMsg.update(o.msg);
                    //var data=grid.getSelectionModel().getSelected();
                    data.set('stus',"Confirmed");
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
}

function handleMessage(adid){
	    var params={};

	    params['adid']= adid;
		Ext.Ajax.request({
		   url: ContextInfo.contextPath+'/staldlyb.mvc/handleMesssage',
		   success: function(response){
		   		var o= Ext.util.JSON.decode(response.responseText);		
		   		if (o.status=='success'){
                    //grid.getBottomToolbar().diplayMsg.update(o.msg);
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
}

function mastRender(val, m, r) {
        var r = '';
        if (val == "1") r = HRMSRes.Public_Label_Married;
        if (val == "2") r = HRMSRes.Public_Label_Single;
        if (val == "3") r = HRMSRes.Public_Label_Bigamous;
        if (val == "4") r = HRMSRes.Public_Label_Divorced;
        if (val == "5") r = HRMSRes.Public_Label_Unknown;

        return r;
}

function empStateRender(val, m, r) {
        if (r.get("emst") == "2") { return '<span style=\'color:blue\'>' + val + '</span>'; }
        else { return '<span style=\'color:black\'>' + val + '</span>'; }
}

function empStateRender1(val, m, r) {
        if (r.get("emst") == "2") { return '' + HRMSRes.Public_Label_Terminated + ''; }
        else { return '' + HRMSRes.Public_Label_OTJ + ''; }
}
