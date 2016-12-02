var _objUpdate = null;
var _objAdd = null;
var _patientId = 0;
var allergyID = null;

var _allergyTable = [];

function GetAllergies() {
    var param = {};

    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetAllergies',
        data: param,
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Allergies.length>0) {
                    allergies = response.Allergies;
                    bindtoTextBoxAllergies(allergies);
                }

            }
            return false;
            // else {alert(data.ErrorMessage);}


        },
        error: errorRes

    });

}

function bindtoTextBoxAllergies(allergies) {

    var allergiesArray = $.map(allergies, function (el) {
        return el.allergyName;
    });

    $('#myAllergy').autocomplete({
        lookup: allergiesArray
    });
}

function GetSensitivities() {
    var param = "{}";
    var listitems;
    var $select = $('#Sensitivity');
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetSensitivities',
        data: param,
        dataType: 'json',
        success: function (response) {
            $.each(response.Object, function (item) {
                listitems += '<option value=' + response.Object[item].severityID + '>' + response.Object[item].sensitivityName + '</option>';

            });
            $select.append(listitems);
        },
        error: errorRes

    });

}

function GetReactions() {
    var param = "{}";
    var listitems;
    var $select = $('#Reaction');
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetReactions',
        data: param,
        dataType: 'json',
        success: function (response) {
            $.each(response.Object, function (item) {
                listitems += '<option value=' + response.Object[item].reactionID + '>' + response.Object[item].reactionName + '</option>';

            });
            $select.append(listitems);
        },
        error: errorRes

    });

}

function GetPatientAllergies(patientid) {
   
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/LoadPatientAllergies',
        data: { 'patientid': patientid },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Allergies.length>0) {
                    _patientId = patientid;
                    _allergyTable = response.Allergies;
                    bindAllergiesTable(_allergyTable);

                }
            }
            // else {alert(data.ErrorMessage);}

            return false;
        },
        error: errorRes

    });

}


function bindAllergiesTable(Allergies) {
    $('#allergiestable').DataTable().clear();
   
     for (var i = 0; i < Allergies.length; i++) {
        $('#allergiestable').dataTable().fnAddData([
                   i + 1,
                   Allergies[i].allergyName,
                    Allergies[i].severity,
                    Allergies[i].reaction,
                    ToJavaScriptDateAllergies(Allergies[i].reporteddate),
                    "<div class='btn-group'> <button style='width:60px'type='button' class='btn btn-primary'>Action</button>" +
                                                  "<button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown' aria-expanded='false'>" +
                                                      "<span class='caret'></span>" +
                                                      "<span class='sr-only'>Toggle Dropdown</span>" +
                                                  "</button>" +
                                                  "<ul class='dropdown-menu' role='menu'>" +
                                                     "<li>" +
                                                      "<a class='editbtn' onclick='editAllergies(" + JSON.stringify(Allergies[i]) + ",this);'>Edit</a>" +
                                                      "</li>" +
                                                      "<li>" +
                                                       "<button id='delete' type='button' class='btn btn-link submit' style='border-bottom:none' onclick='deleteAllergies(" + Allergies[i].allergiesID + ");'>Delete</button></li>" +
                                                  "</ul>" +
                                              "</div>"
        ]);
    }
     $('#allergiestable').DataTable().draw();
}
function ToJavaScriptDateAllergies(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
}
function editAllergies(objAllergy) {
    $("#myAllergy").val(objAllergy.allergyName);
    if (objAllergy.severity != "")
    {
        //alert(objAllergy.severity);
        //$("#Sensitivity option:contains(" + objAllergy.severity + ")").attr('selected', 'selected');
        $("#Sensitivity option").filter(function () {
            //may want to use $.trim in here
            return $(this).text() == objAllergy.severity;
        }).prop('selected', true);
    }
    else
    {
        $("#Sensitivity").val($("#Sensitivity option:first").val());
    }
    if (objAllergy.reaction != "") {
        //alert(objAllergy.reaction);
        //$("#Reaction option:contains(" + objAllergy.reaction + ")").attr('selected', 'selected');
        $("#Reaction option").filter(function () {
            //may want to use $.trim in here
            return $(this).text() == objAllergy.reaction;
        }).prop('selected', true);
    }
    else {
        $("#Reaction").val($("#Reaction option:first").val());
    }
   
    _objUpdate = {};
    _objUpdate["allergyName"] = (objAllergy.allergyName);
    _objUpdate["severity"] = (objAllergy.severity);
    _objUpdate["reaction"] = (objAllergy.reaction);
    _objUpdate["allergiesID"] = (objAllergy.allergiesID);
    _objUpdate["patientID"] = (objAllergy.patientID);
   
    allergyID = objAllergy.allergiesID;
}
function resetAllergies() {

    $("#myAllergy").val('');
    $("#Sensitivity").val($("#Sensitivity option:first").val());
    $("#Reaction").val($("#Reaction option:first").val());
    allergyID = 0;
    _objUpdate = null;
    _objAdd = null;

}



//
function addupdateAllergies(patientid) {
    var msg = ValidateFormAllergies();
    if (msg == "" || msg == undefined) {
        fillObjAllergies(patientid);

        var allergy;
        if (_objUpdate == null) {
            allergy = _objAdd;
            allergyID=0
        }
        else {
            _objUpdate.allergyName = $("#myAllergy").val();
            _objUpdate.severity = $("#Sensitivity option:selected").text();
            _objUpdate.reaction = $("#Reaction option:selected").text();
            if ($("#Sensitivity option:selected").text() == "Choose Sensitivity") {
                _objUpdate.severity = "";

            }
            if ($("#Reaction option:selected").text() == "Choose Reaction") {

                _objUpdate.reaction = "";
            }
            allergy = _objUpdate;
        }

        $.ajax({
            type: 'POST',
            url: '/SeeDoctor/AddUpdateAllergies',
            data: { 'allergiesID': parseInt(allergyID), 'allergy': allergy },
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
                            text: "Allergy is saved successfully.",
                            type: 'success',
                            styling: 'bootstrap3'
                        });
                        if (_objAdd != null) {
                            var _newObj = {};
                            _newObj["allergiesID"] = response.ApiResultModel.ID;
                            _newObj["patientId"] = _objAdd.patientId;
                            _newObj["allergyName"] = _objAdd.allergyName;
                            _newObj["severity"] = _objAdd.severity;
                            _newObj["reaction"] = _objAdd.reaction;
                            _newObj["reporteddate"] = "/Date(" + ticks + ")/";
                            _allergyTable.splice(0, 0, _newObj);
                            bindAllergiesTable(_allergyTable);
                            _objAdd = null;


                        }
                        else if (_objAdd == null) {
                            changeAllergy(response.ApiResultModel.ID, _objUpdate.allergyName, _objUpdate.severity, _objUpdate.reaction);
                            bindAllergiesTable(_allergyTable);
                            _objUpdate = null;
                            
                        }

                    }



                }
                resetAllergies();

            },
            error: errorRes

        });

    }
    else {
        alert(msg);
    }


}

function deleteAllergies(allergyID) {
    var confirmMessage = confirm("Are you sure you want to delete?");
    if (confirmMessage == false)
        return false;


    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/DeleteAllergy',
        data: { 'allergyID': parseInt(allergyID) },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {
                if (response.ApiResultModel.message == "")
                {
                    new PNotify({
                        title: 'Success',
                        text: "Allergy is deleted successfully.",
                        type: 'success',
                        styling: 'bootstrap3'
                    });
                    removeAllergy(response.ApiResultModel.ID);
                    bindAllergiesTable(_allergyTable);
                }
                else if (response.ApiResultModel.message != "") {
                    new PNotify({
                        title: 'Error',
                        text: response.ApiResultModel.message,
                        type: 'error',
                        styling: 'bootstrap3'
                    });
                }
                    
                
                   
            }
           

            


        },
        error: errorRes

    });
}



function getCurrentDate() {
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

    return today = mm + '/' + dd + '/' + yyyy;
}
function fillObjAllergies(patientid) {

    if (_objUpdate == null) {
        _objAdd = {};

        _objAdd["allergyName"] = $("#myAllergy").val();
        _objAdd["severity"] = $("#Sensitivity option:selected").text();;
        _objAdd["reaction"] = $("#Reaction option:selected").text();
        _objAdd["patientID"] = patientid;
        allergyID = 0;
        if ($("#Sensitivity option:selected").text() == "Choose Sensitivity") {
            _objAdd.severity = "";

        }
        if ($("#Reaction option:selected").text() == "Choose Reaction") {

            _objAdd.reaction = "";
        }
    }

}
function ValidateFormAllergies() {
    var success = "";

    if ($("#myAllergy").val() == "") {
        success = "Please enter allergy name";

    }
    return success;

}

function changeAllergy(value, allergyName, severity, reaction) {
    for (var i in _allergyTable) {
        if (_allergyTable[i].allergiesID == value) {
            _allergyTable[i].allergyName = allergyName;
            _allergyTable[i].severity = severity;
            _allergyTable[i].reaction = reaction;
            break;
        }
    }
}
function removeAllergy(value) {
    for (var i in _allergyTable) {
        if (_allergyTable[i].allergiesID == value) {
            _allergyTable.splice(i, 1);
            break;
        }
    }
}






