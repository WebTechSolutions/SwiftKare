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
            $.each(response.Object, function (item) {
                listitems += '<option value=' + response.Object[item].speciallityID + '>' + response.Object[item].specialityName + '</option>';

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
            $.each(response.Object, function (item) {
                listitems += '<option value=' + response.Object[item].languageID + '>' + response.Object[item].languageName + '</option>';
              
            });
           $select.append(listitems);
        },
        error: errorRes

    });

}
function toggle(id)
{
        document.getElementById(""+id+"nofav").style.display = "none";
        document.getElementById("" + id + "fav").style.display = "block";

        return false;
    
   
}
function untoggle(id) {
    document.getElementById("" + id + "nofav").style.display = "block";
    document.getElementById("" + id + "fav").style.display = "none";

    return false;


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
            
            var tableHtml = "";
            if (response.Success == true)
            {
                if (response.DoctorModel .length>0) {

                    $.each(response.DoctorModel, function (item) {
                       
                         tableHtml = tableHtml + " <li>" +
                                      "<div class='well profile_view p-0' style='display: inline-block'>" +
                                       "<a class='hrt' href='' style='color: #5A738E;'><span id='" + response.DoctorModel[item].doctorID + "nofav' class='fa fa-heart-o'  onclick='toggle(" + response.DoctorModel[item].doctorID + ");return false;' style='display:block'></span></a>" +
                                        "<a class='hrt' href='' style='color: #5A738E;'><span id='" + response.DoctorModel[item].doctorID + "fav' class='fa fa-heart'  onclick='untoggle(" + response.DoctorModel[item].doctorID + ");return false;' style='display:none'></span></a>" +
                                       "<i class='crl fa fa-circle' aria-hidden='true' style='color: red; cursor: pointer;font-size: 12px;'></i>" +
                                        "<img src='../Content/images/img.jpg' alt='' class='img-circle img-responsive m-b-10 m-t-20' style='margin: 0 auto;display: inline-block;'>" +
                                        "<h2 class='m-0'>"+
                                         " <a href='' class='thumbnail-col-inner m-b-15' data-toggle='modal' data-target='#myModal2' id='" + response.DoctorModel[item].doctorID + "' onclick='showDoctorTimings(" + response.DoctorModel[item].doctorID + ")'>Dr." +
                                         response.DoctorModel[item].firstName+"&nbsp;"+ response.DoctorModel[item].lastName+"</a>" +
                                          "</h2>"+
                                          "<h4 class='brief m-0' style='color: green'>Available for Call</h4>"+

                                         "<p class='ratings m-t-5 m-b-0' style='text-align: center;'>"+
                                         "<a href='' class='thumbnail-col-inner' data-toggle='modal' data-target='#myModal1'>"+
                                         "<i class='fa fa-calendar' aria-hidden='true'></i>"+
                                         "</a>"+
                                  "<a href=''><i class='fa fa-star-o'></i></a>"+
                                  "<a href=''><i class='fa fa-star-o'></i></a>"+
                                  "<a href=''><i class='fa fa-star-o'></i></a>"+
                                  "<a href=''><i class='fa fa-star-o'></i></a>"+
                                  "<a href=''><i class='fa fa-star-o'></i></a>"+
                                  "<a href=''>"+
                                    "<i class='fa fa-phone' aria-hidden='true'></i>"+
                                  "</a>"+
                                  "</p>" +
                                  "</div></li>";

                    });
                   
                    if (response.DoctorModel.length > 0) { document.getElementById("docList").innerHTML = tableHtml; }
                    else { document.getElementById("docList").innerHTML = "No record found";; }

                    document.getElementById("mainpanel").style.display = "block";
                }
            }
            
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
        type: 'error',
        styling: 'bootstrap3'
    });
}






