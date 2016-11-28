
var _objAppointment = {}; //appointment data to hold and save at the end
var _objPharmacy = {}; //pharmacy data to hold and save at the end
var _selecteddoctorID;
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
function setDateTime(myappTime,myappDate)
{
   
    _objAppointment["appDate"] = myappDate;
    _objAppointment["appTime"] = myappTime;
    _selecteddoctorID = $("#doctorid").val();
    $('#myModal1').modal('hide');
    $('#wizard').smartWizard('goToStep', 2);
    showDoctorInfo();
    showApppointmentSummary(myappTime, myappDate);
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
            var spec = "";
           
           
            $.each(response.Object, function (item) {
                spec = spec+response.Object[item].specialityName +",";
          
            });
           
            var result = spec.substring(0, spec.length - 1);
            tableHtml = "<address>" +
                     "<strong>" + response.Object[0].firstName + response.Object[0].lastName + "</strong>" +
                     "<br><strong>Specaility: </strong>&nbsp;" + result +
                     "<br><strong>Phone: </strong>&nbsp;" + response.Object[0].cellPhone +
                      "<br><strong>Email: </strong>&nbsp; " + response.Object[0].email +
                      "</address>";
            document.getElementById("docInfo").innerHTML = tableHtml;
        },
        error: errorRes

    });
   
}
function showApppointmentSummary(myappTime, myappDate) {
    var ROV = $("#ROV option:selected").text()
    if (ROV == "Choose ROV") {

        ROV = "";
    }
    customDateFormat(myappDate);
    var tableHtml = "<address>" +
                     "<strong>Date: </strong>&nbsp;" +customDateFormat(myappDate) +
                     "<br><strong>Time:</strong>&nbsp;" + myappTime+
                     "<br><strong>Reason for Visit: </strong>&nbsp;" + ROV;
                    "</address>";
                    document.getElementById("appsummary").innerHTML = tableHtml;
       

}
function customDateFormat(s) {

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
function CreateAppointment(patientID) {

    _objAppointment["doctorID"] = $("#doctorid").val();
    _objAppointment["patientID"] = patientID;
    _objAppointment["rov"] = $("#ROV option:selected").text();
    _objAppointment["chiefComplaints"] = $("#chiefcomplaints").val();
    


    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/SaveAppointment',
        data: _objAppointment,
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
                        text: "Appointment is created successfully.",
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