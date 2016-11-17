var _objUpdate = null;
var _objAdd = null;
var _patientId = 0;
var rowID = null;
var _allergyTable = null;

function GetAllergies() {
    var param = {};

    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetAllergies',
        data: param,
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Allergies != null) {
                    allergies = response.Allergies;
                    bindtoTextBox(allergies);
                }

            }
            return false;
            // else {alert(data.ErrorMessage);}


        },
        error: errorRes

    });

}

function GetPatientAllergies(patientid) {
    $("tbody[id$='allergiesTable']").html('');
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetAllergies',
        data: { 'patientid': patientid },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Allergies != null) {
                    //_objUpdate = response.Conditions;
                    _patientId = response.Allergies[0].patientID;
                    bindAllergiesTable(response.Allergies);

                }
            }
            // else {alert(data.ErrorMessage);}

            return false;
        },
        error: errorRes

    });

}

function bindtoTextBox(allergies) {

    var allergiesArray = $.map(allergies, function (el) {
        return el.allergyName;
    });

    $('#myAllergies').autocomplete({
        lookup: medicinesArray
    });
}
function bindAllergiesTable(Allergies) {
  
     for (var i = 0; i < Allergies.length; i++) {
        $('#allergiestable').dataTable().fnAddData([
                   i + 1,
                   Allergies[i].allergyName,
                    Allergies[i].frequency,
                    Allergies[i].severity,
                    ToJavaScriptDate(Allergies[i].reporteddate),
                    "<div class='btn-group'> <button type='button' class='btn btn-primary'>Action</button>" +
                                                  "<button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown' aria-expanded='false'>" +
                                                      "<span class='caret'></span>" +
                                                      "<span class='sr-only'>Toggle Dropdown</span>" +
                                                  "</button>" +
                                                  "<ul class='dropdown-menu' role='menu'>" +
                                                     "<li>" +
                                                      "<a class='editbtn' onclick='editMedicine(" + JSON.stringify(Allergies[i]) + ",this);'>Edit</a>" +
                                                      "</li>" +
                                                      "<li>" +
                                                       "<button id='delete' type='button' class='btn btn-link submit' style='border-bottom:none' onclick='deleteMedicine(" + Allergies[i].allergiesID + ");'>Delete</button></li>" +
                                                  "</ul>" +
                                              "</div>"
        ]);
    }

}
function ToJavaScriptDate(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
}
function editObj(objAllergy) {
    $("#myAllergy").val(objAllergy.allergyName);
    $("#mySeverity").val(objAllergy.severity);
    $("#myReaction").val(objAllergy.reaction);
    $("#allergiesID").val(objAllergy.allergiesID);
    _objUpdate = {};
    _objUpdate["allergyName"] = (objAllergy.allergyName);
    _objUpdate["severity"] = (objAllergy.severity);
    _objUpdate["reaction"] = (objAllergy.reaction);
    _objUpdate["allergiesID"] = (objAllergy.allergiesID);
    _objUpdate["patientID"] = (objAllergy.patientID);
    rowID = objAllergy.allergiesID;
}
function resetAllergies() {

    $("#myAllergy").val('');
    $("#mySeverity").val('');
    $("#myReaction").val('');
    $("#allergiesID").val('0');
    //_objUpdate = null;
    // _objAdd = null;

}



//
function addupdateAllergies() {
    var msg = ValidateForm();
    if (msg == "" || msg == undefined) {
        fillObj();

        var allergy;
        if (_objUpdate == null) {
            allergy = _objAdd;
        }
        else {
            _objUpdate.allergyName = $("#myAllergy").val();
            _objUpdate.severity = $("#mySeverity").val();
            _objUpdate.reaction = $("#myReaction").val();
            allergy = _objUpdate;
        }

        $.ajax({
            type: 'POST',
            url: '/SeeDoctor/AddUpdateAllergy',
            data: allergy,
            dataType: 'json',
            success: function (response) {
                if (response.Success == true) {
                    if (response.AllergyID < 0) {
                        new PNotify({
                            title: 'Error',
                            text: "Invalid allergy name",
                            type: 'success',
                            styling: 'bootstrap3'
                        });
                    }
                    if (response.AllergyID > 0) {
                        new PNotify({
                            title: 'Success',
                            text: "Allergy is saved successfully.",
                            type: 'success',
                            styling: 'bootstrap3'
                        });
                        if (_objAdd != null) {
                            var _newObj = {};
                            _newObj["allergiesID"] = response.AllergyID;
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
                            changeAllergy(response.AllergyID, _objUpdate.allergyName, _objUpdate.severity, _objUpdate.reaction);
                            bindAllergiesTable(_allergyTable);
                            _objUpdate = null;
                            
                        }

                    }



                }

            },
            error: errorRes

        });

    }
    else {
        alert(msg);
    }


}

function deleteObj(allergiesID) {
    var confirmMessage = confirm("Are you sure you want to delete?");
    if (confirmMessage == false)
        return false;


    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/DeleteAllergies',
        data: { 'allergiesID': parseInt(allergiesID) },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {
                if (response.AllergyID != 0) {
                    new PNotify({
                        title: 'Success',
                        text: "Allergy is deleted successfully.",
                        type: 'success',
                        styling: 'bootstrap3'
                    });
                    removeAllergy(response.AllergyID);
                    bindAllergiesTable(_allergyTable);

                }
                else {
                    new PNotify({
                        title: 'Information',
                        text: "Allergy is not found.",
                        type: 'info',
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
function fillObj() {

    if (_objUpdate == null) {
        _objAdd = {};
        _objAdd["allergyName"] = $("#myAllergy").val();
        _objAdd["severity"] = $("#mySeverity").val();
        _objAdd["reaction"] = $("#myReaction").val();
        _objAdd["patientID"] = _patientId;
    }

}
function ValidateForm() {
    var success = "";

    if ($("#myAllergy").val() == "") {
        success = "Please enter allergy name";

    }
    return success;

}

function changeAllergy(value, allergyName, severity, reaction) {
    for (var i in _allergiesTable) {
        if (_allergyTable[i].allergyID == value) {
            _allergyTable[i].allergyName = allergyName;
            _allergyTable[i].severity = severity;
            _allergyTable[i].reaction = reaction;
            break;
        }
    }
}
function removeAllergy(value) {
    for (var i in _allergyTable) {
        if (_allergyTable[i].allergyID == value) {
            _allergyTable.splice(i, 1);
            break;
        }
    }
}






