
var _objAppointment = {}; //appointment data to hold and save at the end
var _objPharmacy = {}; //pharmacy data to hold and save at the end
function uploadFiles() {
    var form = $('#FormUpload')[0];
    var dataString = new FormData(form);
    $.ajax({
        url: '/Uploader/Upload',  //Server script to process data
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

function AppointmentBasicInfo() {

    _objAppointment["doctorID"] = $("#doctorid").val();
    _objAppointment["patientID"] = "1143ff06-4692-4590-bdde-891ed34797b5";
    _objAppointment["appDate"] = $("#appdate").val();
    _objAppointment["appTime"] = "10:15";


    //$.ajax({
    //    type: 'POST',
    //    url: '/SeeDoctor/SaveAppointment',
    //    data: _objSearch,
    //    dataType: 'json',
    //    success: function (response) {

    //            new PNotify({
    //                title: 'Success',
    //                text: "Appointment is created successfully.",
    //                type: 'success',
    //                styling: 'bootstrap3'
    //            });

    //            $("#appointmentID").val(response.appID);

    //    },
    //    error: errorRes

    //});

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