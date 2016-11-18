var _objUpdate = null;
var _objAdd = null;
var _patientId = 0;
var rowID = null;
var surgeries = null;
var _surgeryTable = null;
var surgeryID = 0;

function GetSurgeries() {
    var param = {};

    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetSurgeries',
        data: param,
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Surgeries != null) {
                    surgeries = response.Surgeries;
                    bindtoTextBox(surgeries);
                }

            }
            return false;
            // else {alert(data.ErrorMessage);}


        },
        error: errorRes

    });

}

function bindtoTextBox(surgeries) {

    var surgeriesArray = $.map(surgeries, function (el) {
        return el.bodyPart;
    });

    $('#mySurgery').autocomplete({
        lookup: surgeriesArray
    });
}

function GetPatientSurgeries(patientid) {
    $("tbody[id$='medicinesTable']").html('');
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/LoadPatientSurgeries',
        data: { 'patientid': patientid },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Surgeries != null) {
                    _patientId = response.Surgeries[0].patientID;
                    _surgeryTable = Surgeries;
                    removeDuplicateSurgeries(surgeries);
                    bindingSurgeriesTable(_surgeryTable,surgeries);

                }
            }
           return false;
        },
        error: errorRes

    });

}

function bindingSurgeriesTable(PSurgeries,GSurgeies) {
    var table = $('#surgeriestable').DataTable();
    table.clear();
    for (var i = 0; i < PSurgeries.length; i++) {
        $('#surgeriestable').dataTable().fnAddData([
                  
                   "<div class='checkbox' style='display: inline-block; width: 150px;'>"+
                                                                "<label>"+
                                                                 "<input checked='checked' id='" + PSurgeries[i].surgeryID + "' type='checkbox' class='flat'>&nbsp" + PSurgeries[i].bodyPart +
                                                                "</label>"+
                                                            "</div>"
        ]);
    }
    for (var i = 0; i < GSurgeries.length; i++) {
        $('#surgeriestable').dataTable().fnAddData([

                   "<div class='checkbox' style='display: inline-block; width: 150px;'>" +
                                                                "<label>" +
                                                                 "<input id='" + GSurgeries[i].surgeryName + "' type='checkbox' class='flat'>&nbsp" + GSurgeries[i].surgeryName +
                                                                "</label>" +
                                                            "</div>"
        ]);
    }

}

function removeDuplicateSurgeries(surgeries)
{
    for (var i in _surgeryTable) {
        for (var i in surgeries) {
            if (_surgeryTable[i].bodyPart == surgeries[i].surgeryName) {
                _surgeryTable.splice(i, 1);
               
            }
        }
    }
}

function reset() {

    $("#myBodyPart").val('');
    $("#surgeryID").val('0');
    
}

function addupdateSurgery() {
    var msg = ValidateFormSurgery();
    if (msg == "" || msg == undefined) {
        fillObjSurgery();

        var surgery;
        if (_objUpdate == null) {
            surgery = _objAdd;
            surgeryID = 0
        }
       

        $.ajax({
            type: 'POST',
            url: '/SeeDoctor/AddUpdateSurgeries',
            data: { 'surgeryID': parseInt(surgeryID), 'surgery': surgery },
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
                            text: "Surgery is saved successfully.",
                            type: 'success',
                            styling: 'bootstrap3'
                        });
                        if (_objAdd != null) {
                            var _newObj = {};
                            _newObj["surgeryID"] = response.ApiResultModel.ID;
                            _newObj["patientId"] = _objAdd.patientId;
                            _newObj["bodyPart"] = _objAdd.bodyPart;
                            removeDuplicateSurgeries(surgeries);
                            bindingSurgeriesTable(_surgeryTable, surgeries);
                            _objAdd = null;

                        }
                        //else if (_objAdd == null) {
                        //    changeSurgery(response.AllergyID, _objUpdate.allergyName, _objUpdate.severity, _objUpdate.reaction);
                        //    bindAllergiesTable(_allergyTable);
                        //    _objUpdate = null;
                        //}

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

function deleteObjSurgery(surgeryID) {
     $.ajax({
        type: 'POST',
        url: '/SeeDoctor/DeleteSurgery',
        data: { 'surgeryID': parseInt(surgeryID) },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {
                if(response.ApiResultModel.message=="")
                    new PNotify({
                        title: 'Success',
                        text: "Surgery is deleted successfully.",
                        type: 'success',
                        styling: 'bootstrap3'
                    });
                removeSurgery(response.ApiResultModel.ID);
                bindingSurgeriesTable(_surgeryTable, surgeries);
                   
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

            }


        },
        error: errorRes

    });
}

function fillObjSurgery() {

    if (_objUpdate == null) {
        _objAdd = {};
        _objAdd["bodyPart"] = $("#myBodyPart").val();
        _objAdd["patientID"] = _patientId;
    }

}

function ValidateFormSurgery() {
    var success = "";

    if ($("#myBodyPart").val() == "") {
        success = "Please enter body part";

    }
    return success;

}

function removeSurgery(value) {
    for (var i in _surgeryTable) {
        if (_surgeryTable[i].surgeryID == value) {
            _surgeryTable.splice(i, 1);
            break;
        }
    }
}






