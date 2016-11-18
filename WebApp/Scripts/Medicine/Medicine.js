var _objUpdate = null;
var _objAdd = null;
var _patientId = 0;
var _medicationID = null;
var _medicationTable = null;
var date = new Date();
var ticks = date.getTime();
var medicines = null;
function GetMedicines()
{
    var param = {};

    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetMedicines',
        data:param,
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Medicines.length>0) {
                    medicines = response.Medicines;
                    bindtoTextBoxMedicine(medicines);
                }
               
            }
            return false;
            // else {alert(data.ErrorMessage);}

            
        },
        error: errorRes

    });
      
}

function bindtoTextBoxMedicine(medicines)
{

    var medicinesArray = $.map(medicines, function (el) {
        return el.medicineName;
    });
    
    $('#myMedicine').autocomplete({
        lookup: medicinesArray
    });
}

function GetMedications(patientid) {
   
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetMedications',
        data: { 'patientid': patientid },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Medications.length > 0) {
                    _medicationTable = response.Medications;
                    _patientId = response.Medications[0].patientId;
                    bindMedicinesTable(response.Medications);

                }
            }
            // else {alert(data.ErrorMessage);}

            return false;
        },
        error: errorRes

    });

}

function bindMedicinesTable(Medications) {
    var table = $('#medicinestable').DataTable();
    table.clear();
    for (var i = 0; i < Medications.length; i++) {
        $('#medicinestable').dataTable().fnAddData([
                   i + 1,
                   Medications[i].medicineName,
                    Medications[i].frequency,
                    ToJavaScriptDateMedicine(Medications[i].reporteddate),
                    "<div class='btn-group'> <button type='button' class='btn btn-primary'>Action</button>" +
                                                  "<button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown' aria-expanded='false'>" +
                                                      "<span class='caret'></span>" +
                                                      "<span class='sr-only'>Toggle Dropdown</span>" +
                                                  "</button>" +
                                                  "<ul class='dropdown-menu' role='menu'>" +
                                                     "<li>" +
                                                      "<a class='editbtn' onclick='editMedicine(" + JSON.stringify(Medications[i]) + ",this);'>Edit</a>" +
                                                      "</li>" +
                                                      "<li>" +
                                                       "<button id='delete' type='button' class='btn btn-link submit' style='border-bottom:none' onclick='deleteMedicine(" + Medications[i].medicationID + ");'>Delete</button></li>" +
                                                  "</ul>" +
                                              "</div>"
        ]);
    }
}

function ToJavaScriptDateMedicine(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
}

function editMedicine(objMedicine) {
    $("#myMedicine").val(objMedicine.medicineName);
    $("#myFrequency").val(objMedicine.frequency);
    $("#medicationID").val(objMedicine.medicationID);
    _objUpdate = {};
    _objUpdate["medicineName"] = (objMedicine.medicineName);
    _objUpdate["frequency"] = (objMedicine.frequency);
    _objUpdate["medicationID"] = (objMedicine.medicationID);
    _objUpdate["patientId"] = (objMedicine.patientId);
    _medicationID = objMedicine.medicationID;
}

function resetMedicine() {

    $("#myMedicine").val('');
   // $("#medicationID").val('0');
    _objUpdate = null;
    _objAdd = null;

}

function addupdateMedicine() {
    var msg = ValidateFormMedicine();
    if (msg == "" || msg == undefined) {
        fillObjMedicine();

        var medication;
        if (_objUpdate == null) {
            _medicationID = 0;
            medication = _objAdd;
        }
        else {
            _objUpdate.medicineName = $("#myMedicine").val();
            medication = _objUpdate;
        }

        $.ajax({
            type: 'POST',
            url: '/SeeDoctor/AddUpdateMedications',
            data: { 'mid': parseInt(_medicationID), 'medication': medication },
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
                            text: "Medicine is saved successfully.",
                            type: 'success',
                            styling: 'bootstrap3'
                        });
                        if (_objAdd != null) {
                            var _newObj = {};
                            _newObj["medicationID"] = response.ApiResultModel.ID;
                            _newObj["patientId"] = _objAdd.patientId;
                            _newObj["medicineName"] = _objAdd.medicineName;
                            _newObj["frequency"] = _objAdd.frequency;
                            _newObj["reporteddate"] = "/Date(" + ticks + ")/";
                            _medicationTable.splice(0, 0, _newObj);
                            bindMedicinesTable(_medicationTable);
                           _objAdd = null;


                        }
                        else if (_objAdd == null) {
                            changeMedicine(response.ApiResultModel.ID, _objUpdate.medicineName, _objUpdate.frequency);
                            bindMedicinesTable(_medicationTable);
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

function deleteMedicine(medicationID) {
    var confirmMessage = confirm("Are you sure you want to delete?");
    if (confirmMessage == false)
        return false;


    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/DeleteMedications',
        data: { 'medicationID': parseInt(medicationID) },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {
                if(response.ApiResultModel.message=="")
                    new PNotify({
                        title: 'Success',
                        text: "Medicine is deleted successfully.",
                        type: 'success',
                        styling: 'bootstrap3'
                    });
                removeMedicine(response.ApiResultModel.ID);
                bindMedicinesTable(_medicationTable);
                   
            }
            else if(response.ApiResultModel.message!="")
            {
                new PNotify({
                    title: 'Error',
                    text: response.ApiResultModel.message,
                    type: 'error',
                    styling: 'bootstrap3'
                });
            }

            


        },
        error: errorRes

    });
}

function changeMedicine(value, medicineName,frequency) {
    for (var i in _medicationTable) {
        if (_medicationTable[i].medicationID == value) {
            _medicationTable[i].medicineName = medicineName;
            _medicationTable[i].frequency = frequency;
            break;
        }
    }
}

function removeMedicine(value) {
    for (var i in _medicationTable) {
        if (_medicationTable[i].medicationID == value) {
            _medicationTable.splice(i, 1);
            break;
        }
    }
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

function fillObjMedicine() {

    if (_objUpdate == null) {
        _objAdd = {};
        _objAdd["medicineName"] = $("#myMedicine").val();
        _objAdd["frequency"] = $("#myFrequency").val();
        _objAdd["patientId"] = _patientId;
    }

}

function ValidateFormMedicine() {
    var success = "";

    if ($("#myMedicine").val() == "") {
        success = "Please enter medicine name";

    }
    return success;

}






