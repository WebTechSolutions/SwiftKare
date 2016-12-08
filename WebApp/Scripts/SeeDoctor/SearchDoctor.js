
function GetROV() {
    var param = "{}";
    var listitems;
    var $select = $('#ROV');
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetROVList',
        data: param,
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Object != null) {
                    $.each(response.Object, function (item) {
                        listitems += '<option value=' + response.Object[item].rov + '>' + response.Object[item].rov + '</option>';

                    });
                    $select.append(listitems);
                    //here I could call the properties of my object, as below:
                    //$("#ROV").val(response.Object.rov);

                }
            }
            //else {response.Message;}

            return false;
        },
        error: errorRes

    });

}
function GetPatientROV(patientID) {
    var param = "{}";
    var listitems;
    var $select = $('#ROV');
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetPatientROV',
        data: { 'patientid': patientID },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {
                //alert(response.Object.rov);
                if (response.Object != null) {

                    $("#ROV option").filter(function () {
                        //may want to use $.trim in here
                        //alert(response.Object.rov);
                        return $(this).text() == response.Object.rov;
                    }).prop('selected', true);
                    //$("#ROV option:contains(" + response.Object.rov + ")").attr('selected', 'selected');
                    //alert();
                    //$("#ROV").val(response.Object.rov);

                }
            }
            //else {response.Message;}

            return false;
        },
        error: errorRes

    });

}
function GetPatientChiefComplaints(patientID) {
    var param = "{}";

    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetPatientChiefComplaints',
        data: { 'patientid': patientID },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Object != null) {

                    //here I could call the properties of my object, as below:
                    $("#chiefcomplaints").val(response.Object.chiefComplaints);


                    //$('#ROV option').filter(function () { return $(this).html() == response.Object.rov; }).val();
                }
            }
            //else {response.Message;}

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
function favDoctors(patientID) {
    
    $.ajax({
        type: 'POST',
        //url: '@Url.Action("GetAllLanguages", "SeeDoctor")',
        url: '/SeeDoctor/GetFavDoctors',
        data: { 'patientID': patientID },
        dataType: 'json',
        success: function (response) {
            $.each(response.Object, function (item) {
                document.getElementById("" + response.Object[item].docID + "nofav").style.display = "none";
                document.getElementById("" + response.Object[item].docID + "fav").style.display = "block";

            });

        },
        error: errorRes

    });
   // $.unblockUI();
}
function toggle(docid, patid) {

    _objFav = {};
    _objFav["docID"] = docid;
    _objFav["patID"] = patid;
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/AddFavourite',
        data: _objFav,
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
                        text: "Doctor is made favourite successfully.",
                        type: 'success',
                        styling: 'bootstrap3'
                    });
                    document.getElementById("" + docid + "nofav").style.display = "none";
                    document.getElementById("" + docid + "fav").style.display = "block";

                }



            }

        },
        error: errorRes

    });
    return false;

}
function untoggle(docid, patid) {

    _objFav = {};
    _objFav["docID"] = docid;
    _objFav["patID"] = patid;
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/UpdateFavourite',
        data: _objFav,
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
                        text: "Doctor is made unfavourite successfully.",
                        type: 'success',
                        styling: 'bootstrap3'
                    });
                    document.getElementById("" + docid + "nofav").style.display = "block";
                    document.getElementById("" + docid + "fav").style.display = "none";

                }



            }

        },
        error: errorRes

    });

    return false;


}
function SearchDoctor(patientID) {

    //blockUI();
    //$.blockUI({ message: "<h2>test</h2>" });
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

            var tableHtml = "";// "<div class='row'>";
            if (response.Success == true) {
                if (response.DoctorModel.length > 0) {

                    $.each(response.DoctorModel, function (item) {

                        // tableHtml = tableHtml + "<div class='col'>" +
                        tableHtml = tableHtml + "<li><div class='well profile_view clsDivDocList' style='position:relative;'>" +
                               " <a class='hrt' href='' style='color: #5A738E;'><span id='" + response.DoctorModel[item].doctorID + "nofav' class='fa fa-heart-o'  onclick='toggle(" + response.DoctorModel[item].doctorID + "," + patientID + ");return false;' style='display:block'></span></a>" +
                               "<a class='hrt' href='' style='color: #5A738E;'><span id='" + response.DoctorModel[item].doctorID + "fav' class='fa fa-heart'  onclick='untoggle(" + response.DoctorModel[item].doctorID + "," + patientID + ");return false;' style='display:none'></span></a>" +
                                "<i class='crl fa fa-circle clsAvailableSpot' aria-hidden='true' style='color: red; cursor: pointer;font-size: 12px; position:absolute; right:2%;'></i>" +
                                "<img src='../Content/images/img.jpg' alt='' class='img-circle img-responsive m-b-10 m-t-0' style='margin: 0 auto;display: inline-block;'>" +
                                "<h2 class='m-0'>" +
                                 " <a href='#' class='thumbnail-col-inner m-b-15' data-toggle='modal' data-target='#myModal2' style='word-wrap: break-word;font-size: 15px'>Dr." +
                                  response.DoctorModel[item].firstName + "&nbsp;" + response.DoctorModel[item].lastName + "</a>" +
                                 "</h2>" +
                                "<h4 class='brief m-0 clsAvailbleForCall' style='color: green; display:none;' style='font-size: 14px'>Available</h4><h4 class='brief m-0 clsNotAvailbleForCall' style='color: red;font-size: 14px'>Not Available</h4>" +
                                "<p class='ratings m-t-5 m-b-0' style='text-align: center;'>" +
                                "<a style='word-wrap:word-break;' href='' class='thumbnail-col-inner' data-toggle='modal' data-target='#myModal1' data-todo='{\"id\":" + response.DoctorModel[item].doctorID + ",\"doctorName\":\"" + response.DoctorModel[item].firstName +
                                "&nbsp;" + response.DoctorModel[item].lastName + "\"}'  onclick='showDoctorTimings(" + response.DoctorModel[item].doctorID + ")' id='" + response.DoctorModel[item].doctorID + "'>" +
                                  "  <i class='fa fa-calendar' aria-hidden='true'></i>" +
                                  "</a>" +
                                  "&nbsp;<a href='#'><i class='fa fa-star'></i></a>" +
                                  "&nbsp;<a href='#'><i class='fa fa-star'></i></a>" +
                                  "&nbsp;<a href='#'><i class='fa fa-star'></i></a>" +
                                  "&nbsp;<a href='#'><i class='fa fa-star'></i></a>" +
                                  "&nbsp;<a href='#'><i class='fa fa-star'></i></a>" +
                                  "&nbsp;<i class='fa fa-phone clsNotMakePhone' aria-hidden='true'></i><a  class='clsMakeCall' title='Call doctor " + response.DoctorModel[item].firstName + "&nbsp;" + response.DoctorModel[item].lastName + "' onclick='makeCallToDoctor(this)' data-doctorid='" + response.DoctorModel[item].doctorID + "' style='display:none;' href='javascript:'>" +
                                  " <i class='fa fa-phone' aria-hidden='true'></i>" +
                        "</a>" +
                        "</p>" +
                              "</div>" +
                            "</li>";
                        //tableHtml = tableHtml + " <li>" +
                        //             "<div class='well profile_view p-0' style='display: inline-block'>" +
                        //              "<a class='hrt' href='' style='color: #5A738E;'><span id='" + response.DoctorModel[item].doctorID + "nofav' class='fa fa-heart-o'  onclick='toggle(" + response.DoctorModel[item].doctorID + ");return false;' style='display:block'></span></a>" +
                        //               "<a class='hrt' href='' style='color: #5A738E;'><span id='" + response.DoctorModel[item].doctorID + "fav' class='fa fa-heart'  onclick='untoggle(" + response.DoctorModel[item].doctorID + ");return false;' style='display:none'></span></a>" +
                        //              "<i class='crl fa fa-circle' aria-hidden='true' style='color: red; cursor: pointer;font-size: 12px;'></i>" +
                        //               "<img src='../Content/images/img.jpg' alt='' class='img-circle img-responsive m-b-10 m-t-20' style='margin: 0 auto;display: inline-block;'>" +
                        //               "<h2 class='m-0'>"+
                        //                " <a href='' class='thumbnail-col-inner m-b-15' data-toggle='modal' data-target='#myModal8' id='" + response.DoctorModel[item].doctorID + "' onclick='showDoctorTimings(" + response.DoctorModel[item].doctorID + "," + $("#appdate").val() + ")'>Dr." +
                        //                response.DoctorModel[item].firstName+"&nbsp;"+ response.DoctorModel[item].lastName+"</a>" +
                        //                 "</h2>"+
                        //                 "<h4 class='brief m-0' style='color: green'>Available for Call</h4>"+

                        //                "<p class='ratings m-t-5 m-b-0' style='text-align: center;'>"+
                        //                "<a href='' class='thumbnail-col-inner' data-toggle='modal' data-target='#myModal1' id='" + response.DoctorModel[item].doctorID + "' onclick='showDoctorTimings(" + response.DoctorModel[item].doctorID + "," + $("#appdate").val() + ")'>" +
                        //                "<i class='fa fa-calendar' aria-hidden='true'></i>"+
                        //                "</a>"+
                        //         "<a href=''><i class='fa fa-star-o'></i></a>"+
                        //         "<a href=''><i class='fa fa-star-o'></i></a>"+
                        //         "<a href=''><i class='fa fa-star-o'></i></a>"+
                        //         "<a href=''><i class='fa fa-star-o'></i></a>"+
                        //         "<a href=''><i class='fa fa-star-o'></i></a>"+
                        //         "<a href=''>"+
                        //           "<i class='fa fa-phone' aria-hidden='true'></i>"+
                        //         "</a>"+
                        //         "</p>" +
                        //         "</div></li>";

                    });
                    //tableHtml = tableHtml+"</div>";
                    favDoctors(patientID);
                }
                if (response.DoctorModel.length > 0) { document.getElementById("docList").innerHTML = tableHtml; checkAnyDoctorAvailableNow(); }
                else { document.getElementById("docList").innerHTML = "No record found";; }

                document.getElementById("mainpanel").style.display = "block";
                $.unblockUI();
            }

        },
        error: errorRes

    });

    $.unblockUI();
}
function fetchTimings(fetchdate) {

    var _objSearch = {};
    _objSearch["appDate"] = fetchdate;//$("#fetchdate").val();
    _objSearch["doctorID"] = $("#doctorid").val();

    var div = "";
    $.ajax({
        type: 'POST',
        //url: '@Url.Action("GetAllLanguages", "SeeDoctor")',
        url: '/SeeDoctor/FetchDoctorTimings',
        data: _objSearch,
        dataType: 'json',
        success: function (response) {

            var tablehtml = "";
            $.each(response.Object, function (item) {

                tablehtml = tablehtml + " <li><button id ='" + response.Object[item] + "' type='button' class='btn btn-primary' onclick='setDateTime(\"" + response.Object[item] + "\",\"" + fetchdate + "\")'>" + response.Object[item] + "</button></li>";

            });

            if (tablehtml == "") { document.getElementById("TimingsData").innerHTML = "No record found"; }
            else { document.getElementById("TimingsData").innerHTML = tablehtml; }
        },
        error: errorRes

    });
}

function showDoctorTimings(doctorID) {
    //document.getElementById("TimingsData").innerHTML = "No record found";
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
    $('#fetchdate').val(today);
    var _objSearch = {};
    _objSearch["appDate"] = today;
    // _objSearch["appDate"] = '27/11/2016';
    _objSearch["doctorID"] = doctorID;

    var div = "";
    $.ajax({
        type: 'POST',
        //url: '@Url.Action("GetAllLanguages", "SeeDoctor")',
        url: '/SeeDoctor/FetchDoctorTimings',
        data: _objSearch,
        dataType: 'json',
        success: function (response) {

            var tablehtml = "";
            $.each(response.Object, function (item) {

                tablehtml = tablehtml + " <li><button id ='" + response.Object[item] + "' type='button' class='btn btn-primary' onclick='setDateTime(\"" + response.Object[item] + "\",\"" + $("#fetchdate").val() + "\")'>" + response.Object[item] + "</button></li>";

            });

            if (tablehtml == "") { document.getElementById("TimingsData").innerHTML = "No record found"; }
            else { document.getElementById("TimingsData").innerHTML = tablehtml; }
        },
        error: errorRes

    });
}


//function errorRes(data) {
//   // var err = eval("(" + data.responseText + ")");
//    //alert(err.Message);
//    new PNotify({
//        title: 'Error',
//        text: data.Message,
//        type: 'error',
//        styling: 'bootstrap3'
//    });
//}






