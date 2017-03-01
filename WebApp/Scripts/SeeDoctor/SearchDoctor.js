var favDoctorsList=[];
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
                        //console.log("Patient Last ROV:"+" "+response.Object.rov);
                        return $(this).text() == response.Object.rov;
                    }).prop('selected', true);
                    console.log($("#ROV").find(":selected").text());
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

                    $("#ROV option").filter(function () {
                        //may want to use $.trim in here
                        //alert(response.Object.rov);
                       
                        return $(this).text() == response.Object.rov;
                    }).prop('selected', true);
                    
                    //$('#ROV option').filter(function () { return $(this).html() == response.Object.rov; }).val();
                    console.log($("#ROV").find(":selected").text());
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
function favDoctors(doctorID) {
    if (document.getElementById("" + doctorID + "nofav") != undefined)
    {
        document.getElementById("" + doctorID + "nofav").style.display = "none";
    }
    if (document.getElementById("" + doctorID + "fav") != undefined) {
        document.getElementById("" + doctorID + "fav").style.display = "block";
    }
   
   
   
    //$.ajax({
    //    type: 'POST',
    //    //url: '@Url.Action("GetAllLanguages", "SeeDoctor")',
    //    url: '/SeeDoctor/GetFavDoctors',
    //    data: { 'patientID': patientID },
    //    dataType: 'json',
    //    success: function (response) {
    //        $.each(response.Object, function (item) {
    //            document.getElementById("" + doctorID + "nofav").style.display = "none";
    //            document.getElementById("" + doctorID + "fav").style.display = "block";

    //        });

    //    },
    //    error: errorRes

    //});
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
                        type: 'info',addclass: 'dark',
                        styling: 'bootstrap3'
                    });
                    reloadContainer(function () {

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
                        type: 'info',addclass: 'dark',
                        styling: 'bootstrap3'
                    });
                    reloadContainer(function () {

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

    showLoader();
    var _objSearch = {};
    var offset=getTimeZoneOffset();
    _objSearch["language"] = $("#Language").find(":selected").text();
    _objSearch["speciality"] = $("#Speciality").find(":selected").text();
    _objSearch["appDate"] = $("#searchdate").val();
    var appTime = $('select[name="Shift"]').val();
    if (appTime == "A")
    {
        _objSearch["appTime"] = "ALL";
    }
    if (appTime == "M")
    {
        var utcdate = new Date(); utcdate.setHours(8);
        var isoDate = new Date(utcdate).toISOString();
        var fromTime = new Date(isoDate).getUTCHours();
         utcdate = new Date(); utcdate.setHours(16);
         isoDate = new Date(utcdate).toISOString();
        var toTime = new Date(isoDate).getUTCHours();
        //alert(dd.getUTCHours());
        _objSearch["appTime"] = fromTime + ":" + toTime;
    }
    if (appTime == "E") {
        var utcdate = new Date(); utcdate.setHours(16);
        var isoDate = new Date(utcdate).toISOString();
        var fromTime = new Date(isoDate).getUTCHours();
        utcdate = new Date(); utcdate.setHours(23);
        isoDate = new Date(utcdate).toISOString();
        var toTime = new Date(isoDate).getUTCHours();
        _objSearch["appTime"] = fromTime + ":" + toTime;
    }
    if (appTime == "N") {
        var utcdate = new Date(); utcdate.setHours(23);
        var isoDate = new Date(utcdate).toISOString();
        var fromTime = new Date(isoDate).getUTCHours();
        utcdate = new Date(); utcdate.setHours(8);
        isoDate = new Date(utcdate).toISOString();
        var toTime = new Date(isoDate).getUTCHours();
        _objSearch["appTime"] = fromTime + ":" + toTime;
    }
   
    _objSearch["name"] = $("#providerName").val();
    _objSearch["gender"] = $("#Gender").find(":selected").text();

    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/SearchDoctor',
        data: _objSearch,
        dataType: 'json',
        success: function (response) {

            var tableHtml = "";// "<div class='row'>";
            if (response.Success == true) {
                if (response.Message != null) {

                    new PNotify({
                        title: 'Error',
                        text: response.Message,
                        type: 'error',
                        styling: 'bootstrap3'
                    });
                }
                else {
                    if (response.DoctorModel != undefined && response.DoctorModel != null) {

                        $.each(response.DoctorModel.doctor, function (item) {

                            
                            tableHtml = tableHtml + "<li><div class='well profile_view clsDivDocList' style='position:relative;'>" +
                                   " <a class='hrt' href='' style='color: #5A738E;'><span id='" + response.DoctorModel.doctor[item].doctorID + "nofav' class='fa fa-heart-o'  onclick='toggle(" + response.DoctorModel.doctor[item].doctorID + "," + patientID + ");return false;' style='display:block'></span></a>" +
                                   "<a class='hrt' href='' style='color: #5A738E;'><span id='" + response.DoctorModel.doctor[item].doctorID + "fav' class='fa fa-heart'  onclick='untoggle(" + response.DoctorModel.doctor[item].doctorID + "," + patientID + ");return false;' style='display:none'></span></a>" +
                                    "<i class='crl fa fa-circle clsAvailableSpot' aria-hidden='true' style='color: red; cursor: pointer;font-size: 12px; position:absolute; right:2%;'></i>" +
                                    "<img src='"+ response.DoctorModel.doctor[item].ProfilePhotoBase64 + "' alt='' class='img-circle img-responsive m-b-10 m-t-0' style='margin: 0 auto;display: inline-block;'>" +
                                    "<h2 class='m-0'>" +
                                     " <a href='' onclick='showProfile("+response.DoctorModel.doctor[item].doctorID+")' id='viewprofile' data-toggle='modal' data-target='#myModal8' style='word-wrap: break-word;font-size: 12px'>Dr." +
                                      response.DoctorModel.doctor[item].firstName + " " + response.DoctorModel.doctor[item].lastName + "</a>" +
                                      
                                     "</h2>" +
                                    "<h4 class='brief m-0 clsAvailbleForCall' style='color: green; display:none;' style='font-size: 14px'>Available</h4><h4 class='brief m-0 clsNotAvailbleForCall' style='color: red;font-size: 14px'>Not Available</h4>" +
                                    "<p class='ratings m-t-5 m-b-0' style='text-align: center;'>" +
                                    "<a style='word-wrap:word-break;' href='' class='thumbnail-col-inner' data-toggle='modal' data-target='#myModal1' data-todo='{\"id\":" + response.DoctorModel.doctor[item].doctorID + ",\"doctorName\":\"" + response.DoctorModel.doctor[item].firstName +
                                    "&nbsp;" + response.DoctorModel.doctor[item].lastName + "\"}'  onclick='showDoctorTimings(" + response.DoctorModel.doctor[item].doctorID + ")' id='" + response.DoctorModel.doctor[item].doctorID + "'>" +
                                      "  <i class='fa fa-calendar' aria-hidden='true'></i>" +
                                      "</a>" +
                                      "&nbsp;<a href='#'><i class='fa fa-star'></i></a>" +
                                      "&nbsp;<a href='#'><i class='fa fa-star'></i></a>" +
                                      "&nbsp;<a href='#'><i class='fa fa-star'></i></a>" +
                                      "&nbsp;<a href='#'><i class='fa fa-star'></i></a>" +
                                      "&nbsp;<a href='#'><i class='fa fa-star'></i></a>" +
                                      "&nbsp;<i class='fa fa-phone clsNotMakePhone' aria-hidden='true'></i><a  class='clsMakeCall' title='Call doctor " + response.DoctorModel.doctor[item].firstName + "&nbsp;" + response.DoctorModel.doctor[item].lastName + "' onclick='makeCallToDoctor(this)' data-doctorid='" + response.DoctorModel.doctor[item].doctorID + "' style='display:none;' href=''>" +
                                      " <i class='fa fa-phone' aria-hidden='true'></i>" +
                            "</a>" +
                            "</p>" +
                                  "</div>" +
                                "</li>";
                            
                           
                        });
                        if (response.DoctorModel.favdoctor.length > 0) {
                            
                            $.each(response.DoctorModel.favdoctor, function (item) {
                                favDoctorsList.push(response.DoctorModel.favdoctor[item].docID);
                                
                            });
                        }
                        
                    }
                }

                if (response.DoctorModel.doctor.length > 0)
                {
                    document.getElementById("docList").innerHTML = tableHtml; checkAnyDoctorAvailableNow();
                    
                    for(var i=0;i<favDoctorsList.length;i++)
                    {
                        
                        favDoctors(favDoctorsList[i]);
                    }
                }
                else { document.getElementById("docList").innerHTML = "No record found"; }

                document.getElementById("mainpanel").style.display = "block";

            }

        },
        error: errorRes

    });
    
    hideLoader();
    

}
function fetchTimings(fetchdate) {
    showLoader();
    var _objSearch = {};
    _objSearch["appDate"] = fetchdate.trim();//$("#fetchdate").val();
    _objSearch["doctorID"] = $("#doctorid").val();
    
    var div = "";
    $.ajax({
        type: 'POST',
        //url: '@Url.Action("GetAllLanguages", "SeeDoctor")',
        url: '/SeeDoctor/FetchDoctorTimings',
        data: _objSearch,
        dataType: 'json',
        success: function (response) {
            if (response.Message != undefined) {
                //alert(response.Message.ReasonPhrase);
                new PNotify({
                    title: 'Error',
                    text: response.Message.ReasonPhrase,
                    type: 'error',
                    styling: 'bootstrap3'
                });
            }
            else
            {
                var tablehtml = "";
                $.each(response.Object, function (item) {
                   
                    var tolocalTime = converttoLocal(response.Object[item]);
                   
                    var flag = compareDateTime(fetchdate.trim(), tolocalTime);
                    if (flag)
                    {
                        
                        tablehtml = tablehtml + " <li><button id ='" + converttoLocal(response.Object[item]) + "' type='button' class='btn btn-primary' onclick='setDateTime(\"" + converttoLocal(response.Object[item]) + "\",\"" + fetchdate + "\")' style='width:85px'>" + converttoLocal(response.Object[item]) + "</button></li>";
                    }
                        

                });

                if (tablehtml == "") { document.getElementById("TimingsData").innerHTML = "No record found"; }
                else { document.getElementById("TimingsData").innerHTML = tablehtml; }
            }
            
        },
        error: errorRes

    });
    hideLoader();
}

function showDoctorTimings(doctorID) {
    showLoader();
    setAppType("S");
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
                var tolocalTime = converttoLocal(response.Object[item]);
                
                var flag = compareTime(tolocalTime);
                if (flag)
                {
                    
                    tablehtml = tablehtml + " <li><button id ='" + converttoLocal(response.Object[item]) + "' type='button' class='btn btn-primary' onclick='setDateTime(\"" + converttoLocal(response.Object[item]) + "\",\"" + $("#fetchdate").val() + "\")' style='width:85px'>" + converttoLocal(response.Object[item]) + "</button></li>";
                 }
            });

            if (tablehtml == "") { document.getElementById("TimingsData").innerHTML = "No record found"; }
            else { document.getElementById("TimingsData").innerHTML = tablehtml; }
        },
        error: errorRes

    });
    hideLoader();
}


function errorRes(httpObj) {
    //function (httpObj, textStatus) {
    var messages = $.parseJSON(httpObj.getResponseHeader('X-Responded-JSON'));
    if (messages != undefined && messages != null)
    {
        if (messages.status == 401) {
            //window.location.href = "/Account/PatientLogin";
            new PNotify({
                title: 'Error',
                text: messages.statusText,
                type: 'error',
                styling: 'bootstrap3'
            });
        }
    }
    else
    {
        
        new PNotify({
            title: 'Error',
            text: httpObj.statusText ,
            type: 'error',
            styling: 'bootstrap3'
        });
    }
    
        
}







