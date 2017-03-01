//Center the element
$.fn.center = function () {
    this.css("position", "absolute");
    this.css("top", ($(window).height() - this.height()) / 2 + $(window).scrollTop() + "px");
    this.css("left", ($(window).width() - this.width()) / 2 + $(window).scrollLeft() + "px");
    return this;
}

//blockUI
function blockUI() {
    $.blockUI({
        css: {
            backgroundColor: 'transparent',
            border: 'none'
        },
        message: '<div class="spinner"></div>',
        baseZ: 1500,
        overlayCSS: {
            backgroundColor: '#FFFFFF',
            opacity: 0.7,
            cursor: 'wait'
        }
    });
    $('.blockUI.blockMsg').center();
}//end Blockui

function showLoader() {
    blockUI();
}

function hideLoader() {
    $.unblockUI();
}

function cancelFullScreen(el) {
    var requestMethod = el.cancelFullScreen || el.webkitCancelFullScreen || el.mozCancelFullScreen || el.exitFullscreen;
    if (requestMethod) { // cancel full screen.
        requestMethod.call(el);
    } else if (typeof window.ActiveXObject !== "undefined") { // Older IE.
        var wscript = new ActiveXObject("WScript.Shell");
        if (wscript !== null) {
            wscript.SendKeys("{F11}");
        }
    }
}

function requestFullScreen(el) {
    // Supports most browsers and their versions.
    var requestMethod = el.requestFullScreen || el.webkitRequestFullScreen || el.mozRequestFullScreen || el.msRequestFullscreen;

    if (requestMethod) { // Native full screen.
        requestMethod.call(el);
    } else if (typeof window.ActiveXObject !== "undefined") { // Older IE.
        var wscript = new ActiveXObject("WScript.Shell");
        if (wscript !== null) {
            wscript.SendKeys("{F11}");
        }
    }
    return false
}

function toggleFullScreen() {
    var elem = document.body; // Make the body go full screen.
    var isInFullScreen = (document.fullScreenElement && document.fullScreenElement !== null) || (document.mozFullScreen || document.webkitIsFullScreen);

    if (isInFullScreen) {
        cancelFullScreen(document);
    } else {
        requestFullScreen(elem);
    }
    return false;
}
//for doctor timings
function formatAMPM(date) {
    //console.log(date);
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}
function converttoLocal(timespan) {
    var d = new Date();
    var s = timespan;
    var hours;
    var parts = s.match(/(\d+)\:(\d+) (\w+)/);
    if (parts[1] == '12' && parts[3]=='PM')
    {
        hours = parseInt(parts[1], 10);
        var minutes = parseInt(parts[2], 10);
        d.setUTCHours(hours);
        d.setUTCMinutes(minutes);
        d.setUTCSeconds(0);
    }
    else if (parts[1] == '12' && parts[3] == 'AM') {
        var minutes = parseInt(parts[2], 10);
        d.setUTCHours(0);
        d.setUTCMinutes(minutes);
        d.setUTCSeconds(0);
        
    }
    else 
    {
        hours = /am/i.test(parts[3]) ? parseInt(parts[1], 10) : parseInt(parts[1], 10) + 12;
        var minutes = parseInt(parts[2], 10);
        d.setUTCHours(hours);
        d.setUTCMinutes(minutes);
        d.setUTCSeconds(0);
    }
   
    

    //d.setUTCHours(hours);
    //d.setUTCMinutes(minutes);
    //d.setUTCSeconds(0);
    console.log(d);
    return formatAMPM(d);
}

//for patient modules
//how to call showLocal("2012-11-29 22:30:00")
function showLocal(utcdate) {

    var myDate = new Date(new Date(utcdate));
    var timeString = myDate.getFullYear() + '-' + (myDate.getMonth() + 1) + '-'
    + myDate.getDate() + ' ' + myDate.getHours() + ':' + myDate.getMinutes();
    console.log(timeString);
    var dbDate = new Date(Date.UTC(myDate.getFullYear(), myDate.getMonth(), myDate.getDate(), myDate.getHours(), myDate.getMinutes(), 0));
    var options = {
        weekday: "long", year: "numeric", month: "short",
        day: "numeric", hour: "2-digit", minute: "2-digit"
    };

    var localDate = (dbDate.toLocaleTimeString("en-us", options));

    var dd = formatDate(new Date(localDate), "dd/MM/yyyy hh:mm a");
    dd;

    return dd;
}
function createDate(appDate,appTime)
{
     
    if (appTime.length > 0) {
        
        newRegtext = appTime.substr(0, 4) + " " + appTime.substr(4);
        console.log(newRegtext);
    }
    var appdatetime = appDate + ' ' + newRegtext;
    var d = new Date(appdatetime),
    s = newRegtext,
    parts = s.match(/(\d+)\:(\d+) (\w+)/),
    hours = /am/i.test(parts[3]) ? parseInt(parts[1], 10) : parseInt(parts[1], 10) + 12,
    minutes = parseInt(parts[2], 10);

d.setUTCHours(hours);
d.setUTCMinutes(minutes);
d.setUTCSeconds(0);
var addAMPM = formatAMPM(d);
var myDate = new Date(new Date('29/11/2011 12:30PM'+ 'UTC'));

return appDate + ' ' + addAMPM;
}

function getTimeZoneOffset()
{
    var now = new Date()
    var offset = now.getTimezoneOffset() / 60;
    return offset;
}
function compareTime(time) {
    var localDate = new Date();
   
    var dbDate = localDate.getFullYear() + '/' + (localDate.getMonth()+1) + '/' + localDate.getDate() +' ' + time;
    var dbDateTime = new Date(dbDate);
    //var dbtime = dbDate+compareDate.getHours() + ':' + compareDate.getMinutes() + ' ' + time.slice(-2);

    var hours = (localDate.getHours() + 24 - 2) % 24;
    var mid = 'am';
    if (hours == 0) { //At 00 hours we need to show 12 am
        hours = 12;
    }
    else if (hours > 12) {
        hours = hours % 12;
        mid = 'pm';
    }
    var localtime = localDate.getFullYear() + '/' + (localDate.getMonth() +1)+ '/' + localDate.getDate() + ' ' + localDate.getHours() + ':' + localDate.getMinutes();
    var localDateTime = new Date(localtime);
    if (dbDateTime > localDateTime) {
        console.log(dbDateTime + ' is later than ' + localDateTime);
        return true;
    }

    else {
        console.log(localDateTime + ' is later than ' + dbDateTime);
        return false;
    }

}

function compareDateTime(fdate,time)
{
   
    var dateString = fdate;// in format "17/01/2017"
    var dateParts = dateString.split("/");
    var dateObject = new Date(dateParts[2], dateParts[1] - 1, dateParts[0]);
    var dbDate = dateObject.getFullYear() + '/' + (dateObject.getMonth() + 1) + '/' + dateObject.getDate() + ' ' + time ;
    var dbDateTime = new Date(dbDate);
    var localDate = new Date();
    var localtime = localDate.getFullYear() + '/' + (localDate.getMonth() + 1) + '/' + localDate.getDate() + ' ' + localDate.getHours() + ':' + localDate.getMinutes();
    var localDateTime = new Date(localtime);
    if (dbDateTime > localDateTime) {
        console.log(dbDateTime + ' is later than ' + localDateTime);
        return true;
    }

    else {
        console.log(localDateTime + ' is later than ' + dbDateTime);
        return false;
    }
}
