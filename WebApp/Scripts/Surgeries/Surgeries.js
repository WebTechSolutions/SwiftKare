var _objUpdate = null;
var _objAdd = null;
var _patientId = 0;
var rowID = null;
var surgeries = null;
var _surgeryTable = [];
var surgeryID = 0;
var locationinGS = 0;
var locationinPS = 0;
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
        return el.surgeryName;
    });

    $('#myBodyPart').autocomplete({
        lookup: surgeriesArray
    });
}

function GetPatientSurgeries(patientid) {
    _patientId = patientid;
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/LoadPatientSurgeries',
        data: { 'patientid': patientid },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Surgeries != null) {
                    
                    _surgeryTable = response.Surgeries;
                    removeDuplicateSurgeries(surgeries);
                    bindingSurgeriesTable(_surgeryTable,surgeries);

                }
            }
           return false;
        },
        error: errorRes

    });

}

function bindingSurgeriesTable(PSurgeries, GSurgeries) {
    var rowCount = $('#surgeriestable tr').length;
    if (rowCount == 4) {
        $('#surgeriestable tr:last').remove();
    }
    var tableHtml = "<tr><td>";
    for (var i = 0; i < PSurgeries.length; i++) {
        tableHtml=tableHtml+"<div class='checkbox' style='display: inline-block; width: 150px;'>"+
                                                                "<label>"+
                                                                "<input onclick='deleteObjSurgery(" + PSurgeries[i].surgeryID + ")' style='margin-left:-15px' id='" + PSurgeries[i].surgeryID + "' type='checkbox' class='flat' checked='checked'>&nbsp" + PSurgeries[i].bodyPart +
                                                                "</label></div>";
                                                           
       
    }
    for (var i = 0; i < GSurgeries.length; i++) {
        var existingSurgery = GSurgeries[i].surgeryName;
        tableHtml = tableHtml + "<div class='checkbox' style='display: inline-block; width: 150px;'><label>" +
                                "<input  onclick='addupdatepredefinedSurgery(this,\"" + existingSurgery + "\")' style='margin-left:-15px' id='" + GSurgeries[i].surgeryName + "' type='checkbox' class='flat'>&nbsp " + GSurgeries[i].surgeryName +
                                "</label></div>";
                                
    }

    tableHtml = tableHtml + "</td> <td></td></tr>";
    $('#surgeriestable tr:last').after(tableHtml);
    //$("tbody[id$='surgeriestable']").append(tableHtml);
    //$("#surgeriestable").append(tableHtml);
}

function removeDuplicateSurgeries(surgeries)
{
    for (var i = 0; i < _surgeryTable.length;i++) {
        for (var j = 0; j < surgeries.length;j++) {
            if (_surgeryTable[i].bodyPart == surgeries[j].surgeryName) {
                surgeries.splice(j, 1);
                locationinGS = j;
                break;
            }
        }
    }
}

function reset() {

    $("#myBodyPart").val('');
    $("#surgeryID").val('0');
    
}

function addupdatepredefinedSurgery(chkbox,surgeryName)
{
    
    var ischecked = $(chkbox).is(':checked');
    if (ischecked) {
        _objAdd = {};
        _objAdd["bodyPart"] = surgeryName;
        _objAdd["patientID"] = _patientId;
        surgery = _objAdd;
        surgeryID = 0;
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
                            $(chkbox).prop('checked', true);
                            if (_objAdd != null) {
                                var _newObj = {};
                                _newObj["surgeryID"] = response.ApiResultModel.ID;
                                _newObj["patientId"] = _objAdd.patientId;
                                _newObj["bodyPart"] = _objAdd.bodyPart;
                                _surgeryTable.splice(_surgeryTable.length+1, 0, _newObj);
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
           
       

        //alert(returnVal);
    }
    //else
    //{
    //    var returnVal = confirm("Are you sure?");
    //    if (returnVal == true) {
    //        $(this).attr("checked", false);
    //        deleteObjSurgery(this.id);
    //    }
    //}
}

function addupdateSurgery(patientid) {
    var msg = ValidateFormSurgery();
    if (msg == "" || msg == undefined) {
        fillObjSurgery(patientid);

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
                            _surgeryTable.splice(0, 0, _newObj);
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
    var confirmMessage = confirm("Are you sure you want to delete?");
    if (confirmMessage == false)
        return false;
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

       },
        error: errorRes

    });
}

    function fillObjSurgery(patientid) {

    if (_objUpdate == null) {
        _objAdd = {};
        _objAdd["bodyPart"] = $("#myBodyPart").val();
        _objAdd["patientID"] = patientid;
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
    //surgeries.splice(locationinGS, 0, _newObj);
    for (var i in _surgeryTable) {
        if (_surgeryTable[i].surgeryID == value) {
            var obj = [];
            obj["surgeryName"] = _surgeryTable[i].bodyPart;
            surgeries.splice(locationinGS, 0, obj);
            _surgeryTable.splice(i, 1);
           
            break;
        }
    }
}






