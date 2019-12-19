function CheckIDCard(StrNumber) {
    //判断身份证号码格式函数
    //公民身份号码是特征组合码，
    //排列顺序从左至右依次为：六位数字地址码，八位数字出生日期码，三位数字顺序码和一位数字校验码

    //身份证号码长度判断
    if (StrNumber.length < 15 || StrNumber.length == 16 || StrNumber.length == 17 || StrNumber.length > 18) {
        CheckIDCard = false;
    }

    //身份证号码最后一位可能是超过100岁老年人的X
    //所以排除掉最后一位数字进行数字格式测试
    //全部换算成17位数字格式
    var Ai;
    if (StrNumber.length == 18) {
        Ai = StrNumber.substr(0, 17);
    }
    else {
        Ai = StrNumber.substr(0, 6) + "19" + StrNumber.substr(6, 9);
    }
    if (IsNumeric(Ai) == false) {
        return false;
    }

    var strYear, strMonth, strDay, strBirthDay;
    strYear = parseInt(Ai.substr(6, 4));
    strMonth = parseInt(Ai.substr(10, 2));
    strDay = parseInt(Ai.substr(12, 2));

    if (IsValidDate(strYear, strMonth, strDay) == false) {
        return false;
    }

    var arrVerifyCode = new Array('1','0','x','9','8','7','6','5','4','3','2');
    var Wi = new Array(7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2);
    var k,TotalmulAiWi=0;
    for (k=0; k<17;k++){ 
        TotalmulAiWi = TotalmulAiWi + parseInt(Ai.substr(k,1)) * Wi[k];
    }
    
    //alert(\"身份证号码最后一位的识别码是:\"+TotalmulAiWi);
    var modValue =TotalmulAiWi % 11 ;
    var strVerifyCode = arrVerifyCode[modValue];
    Ai = Ai+strVerifyCode;
    
    //alert(\"身份证号码\"+StrNumber+\"与正确的号码\"+Ai+\"一致!\");
    if((StrNumber.length==18)&&(StrNumber!=Ai))
    {
    //alert(\"身份证号码\"+StrNumber+\"与正确的号码\"+Ai+\"不一致，请重新填写!\");
        return false;
    }
    
    return true;
}

function IsNumeric(oNum) {
    if (!oNum) return false;
    var strP = /^(-?\d+)(\.\d+)?$/;
                    
    if (!strP.test(oNum)) return false;
    try {
        if (parseFloat(oNum) != oNum) return false;
    }
    catch (ex) {
        return false;
    }
    return true;
}

//有效年份判断函数IsValidYear()
function IsValidYear(psYear)
{
    var sYear = new String(psYear);
    if(psYear==null)
    {
        //alert('身份证号码出生日期中年份为Null，请重新填写!');
        return false;
    }


    if(isNaN(psYear)==true)
    {
        //alert('身份证号码出生日期中年份必须为数字，请重新填写!');
        return false;
    }

    if(sYear =='')
    {
        //alert('身份证号码出生日期中年份为空，请重新填写!');
        return true;
    }

    if(sYear.match(/[^0-9]/g)!=null)
    {
        //alert('身份证号码出生日期中年份必须为0-9之间的数字组成，请重新填写!');
        return false;
    }

    var nYear = parseInt(sYear,10);

    if((nYear < 0) || (9999 < nYear))
    {
        //alert(nYear +'身份证号码出生日期中年份必须为正常的正整数，请重新填写!');
        return false;
    }

    return true;
}

//有效月份判断函数IsValidMonth()
function IsValidMonth(psMonth)
{
    var sMonth = new String(psMonth);

    if(psMonth==null)
    {
        return false;
    }

    if(isNaN(psMonth)==true)
    {
        return false;
    }

    if(sMonth == '')
    {
        return true;
    }

    if(sMonth.match(/[^0-9]/g)!=null)
    {
        return false;
    }

    var nMonth = parseInt(sMonth,10);

    if((nMonth < 0) || (12 < nMonth))
    {
        return false;
    }

    return true;
}

//有效日判断函数IsValidDay()
function IsValidDay(psDay)
{
    var sDay  = new String(psDay);

    if(psDay==null){return false;}

    if(isNaN(psDay)==true){return false;}

    if(sDay == '') {return true;}

    if(sDay.match(/[^0-9]/g)!=null){return false;}

    var nDay = parseInt(psDay, 10);

    if((nDay < 0) || (31 < nDay))
    {[Page]
        return false;
    }
    return true;
}

//有效日期判断函数IsValidDate()
function IsValidDate(psYear, psMonth, psDay)
{
    if(psYear==null || psMonth==null || psDay==null){return false;}

    var sYear  = new String(psYear);
    var sMonth = new String(psMonth);
    var sDay   = new String(psDay);

    if(IsValidYear(sYear)==false){return false;}

    if(IsValidMonth(sMonth)==false){return false;}

    if(IsValidDay(sDay)==false){return false;}

    var nYear  = parseInt(sYear,  10);
    var nMonth = parseInt(sMonth, 10);
    var nDay   = parseInt(sDay,   10);

    if(sYear=='' &&  sMonth=='' && sDay==''){return true;}

    if(sYear==''|| sMonth=='' || sDay==''){return false;}
   
    if(nMonth < 1 || 12 < nMonth){return false;}
    
    if(nDay < 1 || 31 < nDay){return false;}
    
    if(nMonth == 2)
    {
        if((nYear % 400 == 0) || (nYear % 4 == 0) && (nYear % 100 != 0))
        {
            if((nDay < 1) || (nDay > 29)){
            return false;}
        }
        else
        {
            if((nDay < 1) || (nDay > 28))[Page]
            {
                return false;
            }
        }
    }
    else if((nMonth == 1)  ||
            (nMonth == 3)  ||
            (nMonth == 5)  ||
            (nMonth == 7)  ||
            (nMonth == 8)  ||
            (nMonth == 10) ||
            (nMonth == 12))
    {
        if((nDay < 1) || (31 < nDay))
        {
            return false;
        }
    }
    else
    {
        if((nDay < 1) || (30 < nDay))
        {
            return false;
        }
    }

    return true;

}