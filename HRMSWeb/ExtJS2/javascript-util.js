var start=0;
var end=0;
function add(){        
    var textBox = document.getElementById("ta");
    var pre = textBox.value.substr(0, start);
    var post = textBox.value.substr(end);
    textBox.value = pre + document.getElementById("inputtext").value + post;
}

function savePos(textBox){
    //如果是Firefox(1.5)的话，方法很简单
    if(typeof(textBox.selectionStart) == "number"){
        start = textBox.selectionStart;
        end = textBox.selectionEnd;
    }
    //下面是IE(6.0)的方法，麻烦得很，还要计算上'\n'
    else if(document.selection){
        var range = document.selection.createRange();
        if(range.parentElement().id == textBox.id){
            // create a selection of the whole textarea
            var range_all = document.body.createTextRange();
            range_all.moveToElementText(textBox);
            //两个range，一个是已经选择的text(range)，一个是整个textarea(range_all)
            //range_all.compareEndPoints()比较两个端点，如果range_all比range更往左(further to the left)，则                //返回小于0的值，则range_all往右移一点，直到两个range的start相同。
            // calculate selection start point by moving beginning of range_all to beginning of range
            for (start=0; range_all.compareEndPoints("StartToStart", range) < 0; start++)
                range_all.moveStart('character', 1);
            // get number of line breaks from textarea start to selection start and add them to start
            // 计算一下\n
            for (var i = 0; i <= start; i ++){
                if (textBox.value.charAt(i) == '\n')
                    start++;
            }
            // create a selection of the whole textarea
             var range_all = document.body.createTextRange();
             range_all.moveToElementText(textBox);
             // calculate selection end point by moving beginning of range_all to end of range
             for (end = 0; range_all.compareEndPoints('StartToEnd', range) < 0; end ++)
                 range_all.moveStart('character', 1);
                 // get number of line breaks from textarea start to selection end and add them to end
                 for (var i = 0; i <= end; i ++){
                     if (textBox.value.charAt(i) == '\n')
                         end ++;
                 }
            }
        }
    document.getElementById("start").value = start;
    document.getElementById("end").value = end;
}