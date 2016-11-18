var _objUpdate = null;
var _objAdd = null;
var _patientId = 0;
var rowID = null;
var systems = null;


function GetSystems() {
    var param = {};

    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetSystems',
        data: param,
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Systems != null) {
                    systems = response.Systems;
                    bindtoTextBox(systems);
                }

            }
            return false;
            // else {alert(data.ErrorMessage);}


        },
        error: errorRes

    });

}

function bindtoTextBox(systems) {

    var surgeriesArray = $.map(systems, function (el) {
        return el.bodyPart;
    });

    $('#myBodyPart').autocomplete({
        lookup: surgeriesArray
    });
}

function LoadSurgeries(patientid) {
    $("tbody[id$='medicinesTable']").html('');
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/LoadSurgeries',
        data: { 'patientid': patientid },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Surgeries != null) {
                    _patientId = response.Surgeries[0].patientID;
                    bindingSurgeriesTable(response.Surgeries);

                }
            }
           return false;
        },
        error: errorRes

    });

}

function bindingSurgeriesTable(Surgeries) {
    var table = $('#surgeriestable').DataTable();
    table.clear();
    for (var i = 0; i < Surgeries.length; i++) {
        $('#surgeriestable').dataTable().fnAddData([
                  
                   "<div class='checkbox' style='display: inline-block; width: 150px;'>"+
                                                                "<label>"+
                                                                 "<input type='checkbox' class='flat'>&nbsp"+ Surgeries[i].bodyPart+
                                                                "</label>"+
                                                            "</div>"
        ]);
    }

}

function reset() {

    $("#myBodyPart").val('');
    $("#surgeryID").val('0');
    
}

function addupdateSurgery() {
    var msg = ValidateForm();
    if (msg == "" || msg == undefined) {
        fillObj();

        var surgery;
        if (_objUpdate == null) {
            surgery = _objAdd;
        }
        else {
            _objUpdate.bodyPart = $("#myBodyPart").val();
            surgery = _objUpdate;
        }

        $.ajax({
            type: 'POST',
            url: '/SeeDoctor/AddUpdateSurgeries',
            data: surgery,
            dataType: 'json',
            success: function (response) {
                if (response.Success == true) {
                    if (response.SurgeryID < 0) {
                        new PNotify({
                            title: 'Error',
                            text: "Invalid body part.",
                            type: 'success',
                            styling: 'bootstrap3'
                        });
                    }
                    if (response.MedicineID > 0) {
                        new PNotify({
                            title: 'Success',
                            text: "Surgery is saved successfully.",
                            type: 'success',
                            styling: 'bootstrap3'
                        });
                        if (_objAdd != null) {
                            var _newObj = {};
                            _newObj["surgeryID"] = response.SurgeryID;
                            _newObj["patientId"] = _objAdd.patientId;
                            _newObj["bodyPart"] = _objAdd.bodyPart;
                            bindingSurgeriesTable(_surgeryTable);
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

function deleteObj(medicineID) {
    var confirmMessage = confirm("Are you sure you want to delete?");
    if (confirmMessage == false)
        return false;


    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/DeleteMedicine',
        data: { 'medicineID': parseInt(medicineID) },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {
                if (response.MedicineID != 0) {
                    new PNotify({
                        title: 'Success',
                        text: "Medicine is deleted successfully.",
                        type: 'success',
                        styling: 'bootstrap3'
                    });
                    document.getElementById("row" + response.MedicineID).outerHTML = "";

                }
                else {
                    new PNotify({
                        title: 'Information',
                        text: "Medicine is not found.",
                        type: 'info',
                        styling: 'bootstrap3'
                    });
                }

            }


        },
        error: errorRes

    });
}

function fillObj() {

    if (_objUpdate == null) {
        _objAdd = {};
        _objAdd["bodyPart"] = $("#myBodyPart").val();
        _objAdd["patientID"] = _patientId;
    }

}

function ValidateForm() {
    var success = "";

    if ($("#myBodyPart").val() == "") {
        success = "Please enter body part";

    }
    return success;

}






