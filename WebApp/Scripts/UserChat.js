$(document).ready(function () {
 
    $.fn.animateHighlight = function (highlightColor, duration) {
        var highlightBg = highlightColor || "#4198e6";
        var animateMs = duration || 500;
        var originalBg = this.css("background-color");

        if (!originalBg || originalBg == highlightBg)
            originalBg = "#1266B1"; // default to white

        jQuery(this)
            .css("backgroundColor", highlightBg)
            .animate({ backgroundColor: originalBg }, animateMs, null, function () {
                //jQuery(this).css("backgroundColor", originalBg);
                jQuery(this).removeAttr("style");
            });
    };
});


var UserChat = function (apiKey, sessionId, token) {

    //Declaration - Starts
    var session = null;
    var publisher = null;
    var subscriber = null;
    var streamCreateTime = null;
    var callInterval = null;
    var curStream = null;

    var audioInputDevices;
    var videoInputDevices;

    //Declaration - Ends


    //Functions - Starts
    function connectSession() {
        session.connect(token, function (error) {

            publisher = OT.initPublisher('divPrimaryVideo', {
                insertMode: 'append',
                buttonDisplayMode: 'off',
                height: '100%',
                width: '100%',
                style: { buttonDisplayMode: 'off' }
            });

            if (error) {

                //console.log('There was an error connecting to the session:', error.code, error.message);

                if (error.code == 1006) {
                    new PNotify({ title: 'Error', text: "You are not connected to the internet. Check your network connection.", type: 'error', styling: 'bootstrap3' });
                 //   alert();
                }
            }
            else {
                session.publish(publisher);
                showMyCamera();
            }
        });
    }

    function endSession() {
        session.disconnect();
    }

    function sendChatMessage() {
        session.signal({
            type: 'chat',
            data: $("#txtMsg").val()
        },
          function (error) {
              if (!error) {
                  $("#txtMsg").val('');
              }
          }
        );
    }

    function startCall() {
        $('#aStopCall').show();
        $('#aStrtCall').hide();

        $(".clsCallActive").removeClass("clsHide");

        connectSession();

        $("#imgPrimaryVideo").hide();
        $("#divPrimaryVideo").show();
    }

    function stopCall() {
        session.signal({
            type: 'chat',
            data: "#endconsult#"
        },
                 function (error) {
                     if (!error) {
                        console.log('Error in sending end consult signal to other party')
                     }
                 }
               );

        $('#aStrtCall').show();
        $('#aStopCall').hide();

        $(".clsCallActive").addClass("clsHide");

       endSession();
        localStorage.setItem("livecall", "0");


        streamCreateTime = null;
        clearInterval(callInterval);
        callInterval = null;

        hideMyCamera();

        $("#imgPrimaryVideo").show();
        $("#divPrimaryVideo").hide();

        $("#divMessageContainer").empty();
        $("#pTimeCounter").empty();

        showEditSection();

        //Log session ended
        SaveSessionEnd(sessionId);
    }

    function stopAudioPublish() {
        $('#aStartAudioPublish').show();
        $('#aStopAudioPublish').hide();

        publisher.publishAudio(false);
    }

    function startAudioPublish() {
        $('#aStartAudioPublish').hide();
        $('#aStopAudioPublish').show();

        publisher.publishAudio(true);
    }

    function stopVideoPublish() {
        $('#aStartVideoPublish').show();
        $('#aStopVideoPublish').hide();

        publisher.publishVideo(false);
    }
    function startVideoPublish() {
        $('#aStartVideoPublish').hide();
        $('#aStopVideoPublish').show();

        publisher.publishVideo(true);
    }

    function muteAudio() {
        $('#aUnMudeAudio').show();
        $('#aMudeAudio').hide();
        subscriber.setAudioVolume(0);
    }
    function unmuteAudio() {
        $('#aUnMudeAudio').hide();
        $('#aMudeAudio').show();
        subscriber.setAudioVolume(100);
    }

    function showEditSection() {
        $('#edit-sec').show();
        $('#messages-sec').hide();
        $('.list-alt').addClass('comments-m');
        $('.comments').removeClass('comments-m');
        $('.comments').removeClass('list-alt-m');
    }

    function showTimeElapsed() {

        if (streamCreateTime != null) {
            var today = new Date();
            var finalDisplay = "";

            var date1_ms = streamCreateTime.getTime();
            var date2_ms = today.getTime();

            var difference_sec = (date2_ms - date1_ms) / 1000;
            var seconds = parseInt(Math.floor(difference_sec % 60));
            var minutes = parseInt(Math.floor((difference_sec / 60) % 60));
            var hours = parseInt(Math.floor((difference_sec / 3600) % 60));

            if (hours > 0) {
                finalDisplay += (hours > 9 ? hours.toString() : "0" + hours.toString()) + ":";
            }

            finalDisplay += (minutes > 9 ? minutes.toString() : "0" + minutes.toString()) + ":" + (seconds > 9 ? seconds.toString() : "0" + seconds.toString());

            $("#pTimeCounter").html(finalDisplay);
        }
    }


    function hideMyCamera() {
        $('#aShowMyCamera').show();
        $('#aHideMyCamera').hide();
    }

    function showMyCamera() {
        $('#aShowMyCamera').hide();
        $('#aHideMyCamera').show();
    }


    function getFormattedTime(date) {
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    }

    function intentetConnectivityLost() {
      //  alert("There is problem with your network connection. Please check your network connection.");
        new PNotify({ title: 'Error', text: "There is problem with your network connection. Please check your network connection.", type: 'error', styling: 'bootstrap3' });
    }

    function intentetConnectivityRegain() {
      //  alert("Network connection has been restored. Conneting your call.");
        new PNotify({ title: 'Success', text: "Network connection has been restored. Conneting your call.", type: 'info', addclass: 'dark', styling: 'bootstrap3' });
        session.connect(token);
        session.publish(publisher);
        publisher.publishVideo(true);
    }


    function showVideoMessages() {
        $('#messages-sec').show();
        $('#edit-sec').hide();
        $('.comments').addClass('comments-m');
        $('.list-alt').addClass('list-alt-m');
        $('.list-alt').removeClass('comments-m');
        return false;
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



    //Functions - Ends


    //Database connection end points - Starts

    function networkDisconntected() {

      
        new PNotify({ title: 'Error', text: "It seems other user has some network issue.Please try later.", type: 'error', styling: 'bootstrap3' });
        //Get Consult Id From localStorage
        var consultationId = localStorage.getItem('consultationKey');

        var cUrl = '/UserChat/AddVCLog?consultId=' + consultationId + '&endReason=Other user Disconnected&callDuration=' + parseInt($('#pTimeCounter').text());
        $.post(cUrl, function () {
           // alert('VC Log added for network disconnection');
            window.location = cEndCallUrl;
        });
    }

    function SaveSessionStart(sessionId, callerId, calleId) {
        //Get Consult Id From localStorage
        var consultationId = localStorage.getItem('consultationKey');
        debugger;
        var cUrl = '/UserChat/StartConsultation?consultId=' + consultationId;
        $.post(cUrl);
    }

    function SaveSessionEnd(sessionId) {
        debugger;
        //Get Consult Id From localStorage
        var consultationId = localStorage.getItem('consultationKey');
       
       
        
       

        localStorage.removeItem('consultationKey');
      //  alert("Consult is completed successfully by End call button.");
        new PNotify({ title: 'Success', text: "Consult Completed Successfully..", type: 'info', addclass: 'dark', styling: 'bootstrap3' });
        //cEndCallUrl is defined in page
        // alert(cEndCallUrl)
        var cUrl = '/UserChat/StopConsultation?consultId=' + consultationId;
        $.post(cUrl, function () {
            //Redirect patient to call
            window.location = cEndCallUrl;
        });

    }

    function SaveChatMessage(sessionId, senderId, receiverId, message) {
        //Get Consult Id From localStorage
        var consultationId = localStorage.getItem('consultationKey');

        var cUrl = '/UserChat/AddChatMessages?consultId=' + consultationId + '&message=' + message + '&sender=' + senderId + '&receiver=' + receiverId;
        $.post(cUrl);
    }
    //Database connection end points - Ends


    //External Functions - Starts
    this.initialize = function () {
        debugger;
        // Initialize Session Object
        session = OT.initSession(apiKey, sessionId)
        .on('streamCreated', function (event) {
            try {
                alert('consultationId @ Stream Created:' + localStorage.getItem('consultationKey'))
                var cUrl = '/UserChat/AddVCLog?consultId=' + localStorage.getItem('consultationKey') + '&endReason=VC Started ';
                $.post(cUrl);
                curStream = event.stream;
                //streamCreateTime = new Date(curStream.creationTime);
                streamCreateTime = new Date();

                callInterval = setInterval(showTimeElapsed, 1000);
                debugger;
                subscriber = session.subscribe(event.stream, 'divSecondaryVideo', {
                    insertMode: 'append',
                    buttonDisplayMode: 'off',
                    height: '100%',
                    width: '100%',
                    style: { buttonDisplayMode: 'off' }
                });

                $("#h1Name").html($("#h1Name").data("name"));
                SaveSessionStart(sessionId, 1, 1);
                subscriber.on("disconnected", function (event) {
                  //  alert("It seems another user has network issue, please wait till resolution.");
                    networkDisconntected();
                });
                debugger;
                //For callerId use publisher's id; for calle id use subscriber's id
                

            } catch (e) { }
        })
        .on("streamDestroyed", function (event) {
            try {
                
                debugger;
               // alert(isConsultComplete);
                if (isConsultComplete == false) {
                    //alert(event.reason);
                    
                    networkDisconntected();
                    /*
                                        if (event.reason == "networkDisconnected") {
                                            
                                            networkDisconntected();
                    
                                            //Try reconnecting till network available
                                        } else {
                                            // stopCall();
                                        }*/
                }
                else {
                    var cUrl = '/UserChat/AddVCLog?consultId=' + localStorage.getItem('consultationKey') + '&endReason=Consult compelted by other user ';
                    $.post(cUrl);
                    new PNotify({ title: 'Success', text: "Other user completed the consult.", type: 'info', addclass: 'dark', styling: 'bootstrap3' });
                    window.location = cEndCallUrl;
                }


            } catch (e) { }
        })
        .on('signal:chat', function (event) {
            debugger;
            var oMsgHtml = "";

            console.log(event);
            if (event.data.toString().trim() == "#endconsult#") {
                isConsultComplete = true;
              //  window.location = cEndCallUrl;
//alert('consult completed via chat signal')
            }
            else{
                if (event.from.connectionId === session.connection.connectionId) {
                    //my message <img src='/Content/images/img.jpg' alt=''>
                    oMsgHtml = "<div class='inbox-item  m-b-10'><div class='talk-bubble tri-right right-in margin-r'><div class='talktext'><p>";
                    oMsgHtml += event.data;
                    oMsgHtml += " <span> ";
                    oMsgHtml += getFormattedTime(new Date());
                    oMsgHtml += " </span></p> </div> </div> </div>";
                } else {
                    //their message<img src='/Content/images/img.jpg' alt=''>

                    oMsgHtml += "<div class='sent-item text-right m-b-10'><div class='talk-bubble tri-right left-in'><div class='talktext'> <p>";
                    oMsgHtml += event.data;
                    oMsgHtml += " <span>";
                    oMsgHtml += getFormattedTime(new Date());
                    oMsgHtml += "</span></p> </div> </div> </div>";

                    if (!$("#messages-sec").is(":visible")) {
                        showVideoMessages();
                    }

                    //Log received message
                    //AddSenderEmailID here
                    SaveChatMessage(sessionId, localStorage.getItem('otherUser'), 1, event.data);
                }
            }

            $("#divMessageContainer").append(oMsgHtml);
        });

        //Check whether device is connected to audio-video devices
        OT.getDevices(function (error, devices) {
            audioInputDevices = devices.filter(function (element) {
                return element.kind == "audioInput";
            });
            videoInputDevices = devices.filter(function (element) {
                return element.kind == "videoInput";
            });


            if (audioInputDevices.length == 0 && videoInputDevices.length == 0) {
                new PNotify({ title: 'Error', text: "Your computer is not connected to any audio or video device. Please connect these devices.", type: 'error', styling: 'bootstrap3' });
                var cUrl = '/UserChat/AddVCLog?consultId=' + localStorage.getItem('consultationKey') + '&endReason=Your computer is not connected to any audio or video device ';
                $.post(cUrl);
            }
            else if (audioInputDevices.length == 0) {
                new PNotify({ title: 'Error', text: "Your computer is not connected to any audio device.", type: 'error', styling: 'bootstrap3' });
                var cUrl = '/UserChat/AddVCLog?consultId=' + localStorage.getItem('consultationKey') + '&endReason=Your computer is not connected to any audio  ';
                $.post(cUrl);
            }
            else if (videoInputDevices.length == 0) {
                new PNotify({ title: 'Error', text: "Your computer is not connected to any video device.", type: 'error', styling: 'bootstrap3' });
                var cUrl = '/UserChat/AddVCLog?consultId=' + localStorage.getItem('consultationKey') + '&endReason=Your computer is not connected to any  video device ';
                $.post(cUrl);
            }
        });

        //Event Binding - Starts
        $('#aStrtCall').click(startCall);

        $('#aStopCall').click(stopCall);

        $('#aStopAudioPublish').click(stopAudioPublish);
        $('#aStartAudioPublish').click(startAudioPublish);

        $('#aStopVideoPublish').click(stopVideoPublish);
        $('#aStartVideoPublish').click(startVideoPublish);

        $('#aMudeAudio').click(muteAudio);
        $('#aUnMudeAudio').click(unmuteAudio);

        $('#edit').click(showEditSection);
        $("#btnSendMsg").click(sendChatMessage);

        $('#aExpandVideo').click(function () {
            toggleFullScreen();
            $('#aShrinkExpandVideo').show();
            $('#aExpandVideo').hide();
            return false;
        });

        $('#aShrinkExpandVideo').click(function () {
            toggleFullScreen();
            $('#aShrinkExpandVideo').hide();
            $('#aExpandVideo').show();
            return false;
        });

        $('#aHideMyCamera').click(hideMyCamera);

        $('#aShowMyCamera').click(showMyCamera);

        $('#video-message').click(showVideoMessages);

        // Update the online status icon based on connectivity
        window.addEventListener('online', intentetConnectivityRegain);
        window.addEventListener('offline', intentetConnectivityLost);

        //Event Binding - Ends

        startCall();
    }
    //External Functions - Ends
}
