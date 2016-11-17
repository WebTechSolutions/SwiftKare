var _objAppointment = {}; //appointment data to hold and save at the end


function GetROV(patientid) {
      $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetROV',
        data: { 'patientid': patientid },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {
                
                if (response.Object != null){
                    //here I could call the properties of my object, as below:
                    $("#ROV").val(response.Object.rov);
                   
                }
            }
           // else {alert(data.ErrorMessage);}
      
        return false;
        },
        error: errorRes

    });

}

function uploadFiles()
{
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
function GetAllSpecialities() {
    var param = "{}";
    var listitems;
    var $select = $('#Speciality');
    $.ajax({
        type: 'POST',
        //url: '@Url.Action("GetAllSpecialities", "SeeDoctor")',
        url: '/SeeDoctor/GetAllSpecialities',
        data: param,
        dataType: 'json',
        success: function (response) {
            $.each(response, function (item) {
                listitems += '<option value=' + response[item].speciallityID + '>' + response[item].specialityName + '</option>';

            });
            $select.append(listitems);
        },
        error: errorRes

    });

}
function GetAllLanguages() {
    var param = "{}";
    var $select = $("#Language");
    var listitems;
    $.ajax({
        type: 'POST',
        //url: '@Url.Action("GetAllLanguages", "SeeDoctor")',
        url: '/SeeDoctor/GetAllLanguages',
        data: param,
        dataType: 'json',
        success: function (response) {
            $.each(response, function (item) {
                 listitems += '<option value=' + response[item].languageID + '>' + response[item].languageName + '</option>';
              
            });
           $select.append(listitems);
        },
        error: errorRes

    });

}
function AppointmentBasicInfo() {
   
    _objAppointment["doctorID"] = $("#doctorid").val();
    _objAppointment["userID"] = "1143ff06-4692-4590-bdde-891ed34797b5";
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
function SearchDoctor()
{
    //var param = "{}";
    var _objSearch = {};
    _objSearch["language"] = $("#Language").find(":selected").text();
    _objSearch["speciality"] = $("#Speciality").find(":selected").text();
    _objSearch["appDate"] = $("#searchdate").val();
    _objSearch["appTime"] = $("#time").val();
    _objSearch["name"] = $("#providerName").val();
    _objSearch["gender"] = $("#Gender").find(":selected").text();
    
    $.ajax({
        type: 'POST',
        //url: '@Url.Action("GetAllLanguages", "SeeDoctor")',
        url: '/SeeDoctor/SearchDoctor',
        data: _objSearch,
        dataType: 'json',
        success: function (response) {
            var div = "";

            $.each(response, function (item) {
               
                div = div + " <div class='col-lg-2 col-md-4 col-sm-6 col-xs-12 text-center'>" +
                              "<div class='well profile_view p-0'>" +
                               "<a class='hrt' href='#' style='color: #5A738E;'><span id='toggle1' class='fa fa-heart-o' aria-hidden='true'></span></a>"+
                                "<a class='hrt1' href='#' style='color: #5A738E;'><span id='toggle_1' class='fa fa-heart' aria-hidden='true'></span></a>"+
                                "<i class='crl fa fa-circle' aria-hidden='true' style='color: green; cursor: pointer;font-size: 12px;'></i>"+
                                "<img src='images/img.jpg' alt='' class='img-circle img-responsive m-b-10 m-t-20' style='margin: 0 auto;display: inline-block;'>"+
                                
                                 " <a href='#' class='thumbnail-col-inner' data-toggle='modal' data-target='#myModal1' id='"+response[item].doctorID+"' onclick='showDoctorTimings(" + response[item].doctorID +")'>" +
                                  "<i class='cl1 fa fa-calendar' aria-hidden='true'></i>"+
                                "</a>"+
                               
                                "<a href='#'>"+
                                 " <i class='ph1 fa fa-phone' aria-hidden='true'></i>"+
                                "</a>"+
                                
                                "<h2 class='m-0'>"+
                                "<a href='#' class='thumbnail-col-inner m-b-15' data-toggle='modal' data-target='#myModal8' id='" + response[item].doctorID + "'>" + response[item].firstName + " " + response[item].lastName + "</a>" +
                                "</h2>"+
                                "<h4 class='brief m-0' style='color: green'>Available for Call</h4>"+
                                "<p class='ratings m-t-5' style='text-align: center;'>"+
                                  
                                  "<a href='#'><span class='fa fa-star'></span></a>"+
                                  "<a href='#'><span class='fa fa-star'></span></a>"+
                                  "<a href='#'><span class='fa fa-star'></span></a>"+
                                  "<a href='#'><span class='fa fa-star'></span></a>"+
                                  "<a href='#'><span class='fa fa-star-o'></span></a>"+
                                "</p>"+
                              "</div></div>";

            });
            if (div == "") { document.getElementById("doctorList").innerHTML = "No record found"; }
            else { document.getElementById("doctorList").innerHTML = div; }
           
            document.getElementById("mainpanel").style.display = "block";
        },
        error: errorRes

    });
}
function showDoctorTimings(doctorID,appDate)
{
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

    today = mm + '/' + dd + '/' + yyyy;
    var _objSearch = {};
    _objSearch["appDate"] = today;
    _objSearch["doctorID"] = doctorID;
   
    var div = "";
    $.ajax({
        type: 'POST',
        //url: '@Url.Action("GetAllLanguages", "SeeDoctor")',
        url: '/SeeDoctor/FetchDoctorTimings',
        data: _objSearch,
        dataType: 'json',
        success: function (response) {
            
            var div = "";
            $.each(response, function (item) {
              
                div = div + "<button id ='" + response[item] + "' type='button' class='btn btn-primary'>"+response[item] +"</button>";

            });
           
            if (div == "") { document.getElementById("TimingsData").innerHTML = "No record found"; }
            else { document.getElementById("TimingsData").innerHTML = div; }
        },
        error: errorRes

    });
}


function errorRes(data) {
   // var err = eval("(" + data.responseText + ")");
    //alert(err.Message);
    new PNotify({
        title: 'Error',
        text: data.Message,
        type: 'info',
        styling: 'bootstrap3'
    });
}






