
var _objAppointment = {}; //appointment data to hold and save at the end
var _objPharmacy = {}; //pharmacy data to hold and save at the end
var _selecteddoctorID;
var myappTime;


function setAppType(type)
{
    appType = type;
}


function setDoctorID(doctorID)
{
   
    _selecteddoctorID = doctorID;// $("#doctorid").val();
    $("#doctorid").val(_selecteddoctorID);
    _objAppointment["appTime"] = getClockTime();
   
}
function setDateTime(myappTime,myappDate)
{
   
    _objAppointment["appDate"] = myappDate;
    _objAppointment["appTime"] = myappTime;
    _selecteddoctorID = $("#doctorid").val();
    $('#myModal1').modal('hide');
    $('#wizard').smartWizard('goToStep', 2);
    if ($("#wizard").smartWizard('currentStep', '2')) {
        //alert('2');
        $('.buttonNext').show();
        $('.buttonPrevious').show();

    }
   // showDoctorInfo();
    //showApppointmentSummary(myappTime, myappDate);
    return false;
   // $('#step-2').show();
}

function showApppointmentSummary() {
    var ROV = $("#ROV option:selected").text()
    if (ROV == "Choose ROV") {

        ROV = "";
    }
    if (_objAppointment["appTime"] == null) {

        _objAppointment["appTime"] = getClockTime();
        //alert(_objAppointment["appTime"]);
    }
   // _objAppointment["appDate"] = myappDate;
    // _objAppointment["appTime"] = myappTime;
    var pharmacy = 'N.A';
    if ($('#h2SelPharmacyName').html() != '')
    {
        pharmacy = $('#h2SelPharmacyName').html();
    }
    customDateFormat(_objAppointment["appDate"]);
    var tableHtml = "<address>" +
                     "<strong>Date: </strong>&nbsp;" + customDateFormat(_objAppointment["appDate"]) +
                     "<br><strong>Time:</strong>&nbsp;" + _objAppointment["appTime"] +
                     "<br><strong>Reason for Visit: </strong>&nbsp;" + ROV +
                     "<br><strong>Chief Complaints: </strong>&nbsp;" + $("#chiefcomplaints").val() +
                     "<br><strong>Pharmacy: </strong>&nbsp;" + pharmacy +
                    "</address>";
                    document.getElementById("appsummary").innerHTML = tableHtml;
                 

}
function customDateFormat(s) {
    if (s == null) {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!
        var yyyy = today.getFullYear();

        if (dd < 10) {
            dd = '0' + dd
        }

        if (mm < 10) {
            mm = '0' + mm
        }

        today = dd + '/' + mm + '/' + yyyy;
        s = today;
    }
   
    var b = s.split(/\D/);
    
    var d = new Date(b[2], --b[1], b[0]);

    // Replace with day names in appropriate language
    var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday',
                'Thursday', 'Friday', 'Saturday'];

    // Replace with month names in appropriate language
    var months = ['January', 'February', 'March', 'April', 'May', 'June', 'July',
                  'August', 'September', 'October', 'November', 'December'];

    // Helper for padding single digit values
    function z(n) { return (n < 10 ? '0' : '') + n; }
   
    return days[d.getDay()] + ' ' + d.getDate() + ' ' +
           months[d.getMonth()] + ' ' + d.getFullYear();
}
function getClockTime()
{
    var now    = new Date();
    var hour   = now.getHours();
    var minute = now.getMinutes();
    var second = now.getSeconds();
    var ap = "AM";
    if (hour   > 11) { ap = "PM";             }
    if (hour   > 12) { hour = hour - 12;      }
    if (hour   == 0) { hour = 12;             }
    if (hour   < 10) { hour   = "0" + hour;   }
    if (minute < 10) { minute = "0" + minute; }
    if (second < 10) { second = "0" + second; }
    var timeString = hour +
                     ':' +
                     minute+
                     //':' +
                     //second +
                     " " +
                     ap;
    return timeString;
} // function getClockTime()

//-->

function loadPatientPharmacy(patientID) {
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetPatientPharmacy',
        data: { 'patientID': patientID },
        dataType: 'json',
        success: function (response) {
            if (response.Message != undefined) {
                console.log(response.Message);
            }
            else {
                if (response.Success == true) {

                    if (response.Object != null) {

                        $("#PharmacyID").val(response.Object.pharmacyid);
                        $("#h2SelPharmacyName").html(response.Object.pharmacy);
                        $("#pSelPharmacyAddress").html(response.Object.pharmacyaddress);
                        $("#pSelPharmacyCityStateZip").html(response.Object.pharmacycitystatezip);

                        $("#divPharmacyResultContainer").show();
                    }
                }
            }

            //else {response.Message;}

            return false;
        },
        error: errorRes

    });



}

function savePatientPharmacy(patientID) {

    var objPharmacy = {};
    objPharmacy["patientID"] = patientID;
    objPharmacy["pharmacyid"] = $("#PharmacyID").val();
    objPharmacy["pharmacy"] = $("#h2SelPharmacyName").html();
    objPharmacy["pharmacyaddress"] = $("#pSelPharmacyAddress").html();
    objPharmacy["pharmacycitystatezip"] = $("#pSelPharmacyCityStateZip").html();

    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/SavePatientPharmacy',
        data: objPharmacy,
        dataType: 'json',
        success: function (response) {
            if (response.Message != undefined) {
                new PNotify({
                    title: 'Error',
                    text: response.Message,
                    type: 'error',
                    styling: 'bootstrap3'
                });
            }
            else {
                if (response.Success == true) {
                    if (response.ApiResultModel.message != "") {
                        new PNotify({
                            title: 'Error',
                            text: response.ApiResultModel.message,
                            type: 'error',
                            styling: 'bootstrap3'
                        });
                    }
                    else if (response.ApiResultModel.message == "") {
                        new PNotify({
                            title: 'Success',
                            text: "Pharmacy is saved successfully.",
                            type: 'info', addclass: 'dark',
                            styling: 'bootstrap3'
                        });
                    }

                }
            }

            //else {response.Message;}

            return false;
        },
        error: function (httpObj) {
            new PNotify({
                title: 'Error',
                text: httpObj.statusText,
                type: 'error',
                styling: 'bootstrap3'
            });
        }

    });



}

function errorRes(data) {
     new PNotify({
        title: 'Error',
        text: data.Message,
        type: 'error',
        styling: 'bootstrap3'
    });
}