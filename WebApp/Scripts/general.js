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
    var d = new Date(),
    s = timespan,
    parts = s.match(/(\d+)\:(\d+) (\w+)/),
    hours = /am/i.test(parts[3]) ? parseInt(parts[1], 10) : parseInt(parts[1], 10) + 12,
    minutes = parseInt(parts[2], 10);

    d.setUTCHours(hours);
    d.setUTCMinutes(minutes);
    d.setUTCSeconds(0);
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

    var dd = formatDate(new Date(localDate), "dd/MM/yyyy hh:mm:ssa");
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

