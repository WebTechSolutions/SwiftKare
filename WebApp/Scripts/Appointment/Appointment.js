
var _objAppointment = {}; //appointment data to hold and save at the end
var _objPharmacy = {}; //pharmacy data to hold and save at the end
var _selecteddoctorID;
var myappTime;
function uploadFiles() {
    var form = $('#mydropzone')[0];
    var dataString = new FormData(form);
    $.ajax({
        url: '/SeeDoctor/UploadFiles',  //Server script to process data
        type: 'POST',
        xhr: function () {  // Custom XMLHttpRequest
            var myXhr = $.ajaxSettings.xhr();
            if (myXhr.upload) { // Check if upload property exists
                //myXhr.upload.onprogress = progressHandlingFunction
                myXhr.upload.addEventListener('progress', progressHandlingFunction,
				false); // For handling the progress of the upload
            }
            return myXhr;
        },
        //Ajax events
        success: successHandler,
        error: errorHandler,
        complete: completeHandler,
        // Form data
        data: dataString,
        //Options to tell jQuery not to process data or worry about content-type.
        cache: false,
        contentType: false,
        processData: false
    });
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
    showDoctorInfo();
    //showApppointmentSummary(myappTime, myappDate);
    return false;
   // $('#step-2').show();
}
function showDoctorInfo()
{
    var tableHtml = "";
    $.ajax({
        type: 'POST',
        //url: '@Url.Action("GetAllLanguages", "SeeDoctor")',
        url: '/SeeDoctor/FetchDoctoInfo',
        data:{'doctorID': _selecteddoctorID},
        dataType: 'json',
        success: function (response) {
            if (response.Object != null || response.Object != undefined)
            {
                
                var spec = "";
                var lang = "";
                for (var i = 0; i < response.Object.specialities.length; i++)
                {
                    spec = spec + response.Object.specialities[i].specialityName + ",";
                }
                for (var i = 0; i < response.Object.languages.length; i++) {
                    lang = lang + response.Object.languages[i].languageName + ",";
                }
              
                spec = spec.slice(0, -1);
                lang = lang.slice(0, -1);

                if (response.Object.specialities.length == 0) { spec = "N.A"; }
                if (response.Object.languages.length == 0) { lang = "N.A"; }
                var doccell = "N.A";
                var docemail = "N.A";
                var cc = "N.A";
                var docstate = "N.A";
                var gender = "";
                
                if (response.Object.cellPhone != null) { doccell = response.Object.cellPhone; }
                if (response.Object.email != null) { docemail = response.Object.email; }
                if (response.Object.consultCharges != null) { cc = response.Object.consultCharges; }
                if (response.Object.state != null) { docstate = response.Object.state; }
                if (response.Object.gender != null) { gender = " (" + response.Object.gender + ")"; }
                tableHtml = "<address>" +
                         "<strong>Dr. " + response.Object.doctorName + gender + "</strong>" +
                         "<br><strong>Specaility: </strong>&nbsp;" + spec +
                         "<br><strong>Languages: </strong>&nbsp;" + lang +
                         "<br><strong>State: </strong>&nbsp;" + docstate +
                         "<br><strong>Phone: </strong>&nbsp;" + doccell +
                         "<br><strong>Email: </strong>&nbsp; " + docemail +
                         "<br><strong>Consult Charges: </strong>&nbsp; " + cc +
                          "</address>";
                document.getElementById("docInfo").innerHTML = tableHtml;
            }
            
        },
        error: errorRes

    });
   
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
    customDateFormat(_objAppointment["appDate"]);
    var tableHtml = "<address>" +
                     "<strong>Date: </strong>&nbsp;" + customDateFormat(_objAppointment["appDate"]) +
                     "<br><strong>Time:</strong>&nbsp;" + _objAppointment["appTime"] +
                     "<br><strong>Reason for Visit: </strong>&nbsp;" + ROV +
                     "<br><strong>Chief Complaints: </strong>&nbsp;" + $("#chiefcomplaints").val() +
                     "<br><strong>Pharmacy: </strong>&nbsp;" + $("#pharmacy").val() +
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
function CreateAppointment(patientID,amount) {
   
    _objAppointment["appDate"] = $("#fetchdate").val();
    _objAppointment["doctorID"] = $("#doctorid").val();
    _objAppointment["patientID"] = patientID;
    _objAppointment["paymentAmt"] = amount;
    _objAppointment["rov"] = $("#ROV option:selected").text();
    _objAppointment["chiefComplaints"] = $("#chiefcomplaints").val();
    


    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/SaveAppointment',
        data: JSON.stringify(_objAppointment),
        contentType: "application/json",
        dataType: 'json',
        success: function (response) {

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
                        text: "Appointment is scheduled successfully.",
                        type: 'success',
                        styling: 'bootstrap3'
                    });
                   

                }



            }

        },
        error: errorRes

    });

}
function AddUpdatePharmacy(patientID,pharmacy) {

    _objPharmacy["patientID"] = patientID;
    _objPharmacy["pharmacy"] = pharmacy;//$("#pharmacy").val();
    _objAppointment["pharmacyid"] = 1;//$("#pharmacyid").val();


    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/AddUpdatePharmacy',
        data: _objPharmacy,
        dataType: 'json',
        success: function (response) {
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
                        type: 'success',
                        styling: 'bootstrap3'
                    });
                   
                }



            }
           
        },
        error: errorRes

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