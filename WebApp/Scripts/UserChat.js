
var UserChat = function (apiKey, sessionId, token) {

    //Declaration - Starts
    var session = null;
    var publisher = null;
    var subscriber = null;
    var streamCreateTime = null;
    var callInterval = null;
    var curStream = null;
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
                    alert("You are not connected to the internet. Check your network connection.");
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
        $('#aStrtCall').show();
        $('#aStopCall').hide();

        $(".clsCallActive").addClass("clsHide");

        endSession();


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
        SaveSessionEnd(sessionId, new Date());
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
        alert("There is problem with your network connection. Please check your network connection.");
    }

    function intentetConnectivityRegain() {
        alert("Network connection has been restored. Conneting your call.");
        session.connect(token);
        session.publish(publisher);
        publisher.publishVideo(true);
    }

    //Functions - Ends


    //Database connection end points - Starts
    function SaveSessionStart(sessionId, startTime, callerId, calleId) {
        //TODO: implement save to database logic here
    }

    function SaveSessionEnd(sessionId, endTime) {
        //TODO: implement save to database logic here
    }

    function SaveChatMessage(sessionId, senderId, receiverId, message, receivedTime) {
        //TODO: implement save to database logic here
    }
    //Database connection end points - Ends


    //External Functions - Starts
    this.initialize = function () {

        // Initialize Session Object
        session = OT.initSession(apiKey, sessionId)
        .on('streamCreated', function (event) {
            try {

                curStream = event.stream;
                streamCreateTime = new Date(curStream.creationTime);
                callInterval = setInterval(showTimeElapsed, 1000);

                subscriber = session.subscribe(event.stream, 'divSecondaryVideo', {
                    insertMode: 'append',
                    buttonDisplayMode: 'off',
                    height: '100%',
                    width: '100%',
                    style: { buttonDisplayMode: 'off' }
                });

                subscriber.on("disconnected", function (event) {
                    alert("It seems another user has network issue, please wait till resolution.");
                });

                //For callerId use publisher's id; for calle id use subscriber's id
                SaveSessionStart(sessionId, new Date(), 1, 1);

            } catch (e) { }
        })
        .on("streamDestroyed", function (event) {
            try {
                //alert(event.reason);

                if (event.reason == "networkDisconnected") {
                    //Try reconnecting till network available
                } else {
                    stopCall();
                }

            } catch (e) { }
        })
        .on('signal:chat', function (event) {

            var oMsgHtml = "";

            //console.log(event);
            if (event.from.connectionId === session.connection.connectionId) {
                //my message
                oMsgHtml = "<div class='sent-item text-right m-b-10'><img src='/Content/images/img.jpg' alt=''><div class='talk-bubble tri-right right-in margin-r'><div class='talktext'><p>";
                oMsgHtml += event.data;
                oMsgHtml += " <span> ";
                oMsgHtml += getFormattedTime(new Date());
                oMsgHtml += " </span></p> </div> </div> </div>";
            } else {
                //their message
                oMsgHtml += "<div class='inbox-item m-b-10'><img src='/Content/images/img.jpg' alt=''><div class='talk-bubble tri-right left-in'><div class='talktext'> <p>";
                oMsgHtml += event.data;
                oMsgHtml += " <span>";
                oMsgHtml += getFormattedTime(new Date());
                oMsgHtml += "</span></p> </div> </div> </div>";

                //Log received message
                SaveChatMessage(sessionId, 1, 1, event.data, new Date());
            }

            $("#divMessageContainer").append(oMsgHtml);
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
            $('#aShrinkExpandVideo').show();
            $('#aExpandVideo').hide();
            return false;
        });

        $('#aShrinkExpandVideo').click(function () {
            $('#aShrinkExpandVideo').hide();
            $('#aExpandVideo').show();
            return false;
        });

        $('#aHideMyCamera').click(hideMyCamera);

        $('#aShowMyCamera').click(showMyCamera);

        $('#video-message').click(function () {
            $('#messages-sec').show();
            $('#edit-sec').hide();
            $('.comments').addClass('comments-m');
            $('.list-alt').addClass('list-alt-m');
            $('.list-alt').removeClass('comments-m');
            return false;
        });

        // Update the online status icon based on connectivity
        window.addEventListener('online', intentetConnectivityRegain);
        window.addEventListener('offline', intentetConnectivityLost);

        //Event Binding - Ends
    }
    //External Functions - Ends


}
