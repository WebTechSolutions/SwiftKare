var _objUpdate = null;
var _objAdd = null;
var _patientId = 0;
var _conditionID = null;
var _conditionTable =[];
var date = new Date();
var ticks = date.getTime();


function GetHealthConditions(patientid) {
    $("tbody[id$='conditionsTable']").html('');
    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/GetHealthConditions',
        data: { 'patientid': patientid },
        dataType: 'json',
        success: function (response) {
            if (response.Success == true) {

                if (response.Conditions.length>0) {
                    //_objUpdate = response.Conditions;
                    _conditionTable =response.Conditions;
                    _patientId = patientid;
                     bindConditionsTable(response.Conditions);

                }
            }
            // else {alert(data.ErrorMessage);}

            return false;
        },
        //error: errorRes

    });

}
function bindConditionsTable(Conditions) {
  
    
    $('#conditionstable').DataTable().clear();
    for (var i = 0; i < Conditions.length; i++) {

        $('#conditionstable').dataTable().fnAddData([
             i + 1,
             Conditions[i].conditionName,
              ConverttoDateHealthConditions(Conditions[i].reportedDate),
              "<div class='btn-group'> <button  type='button' class='btn btn-primary'>Action</button>" +
                                            "<button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown' aria-expanded='false'>" +
                                                "<span class='caret'></span>" +
                                                "<span class='sr-only'>Toggle Dropdown</span>" +
                                            "</button>" +
                                            "<ul class='dropdown-menu' role='menu'>" +
                                               "<li>" +
                                                "<a class='editbtn' href='#'  onclick='editHealthConditions(" + JSON.stringify(Conditions[i]) + ",this);'>Edit</a>" +
                                                "</li>" +
                                                "<li>" +
                                                 "<button id='delete' type='button' class='btn btn-link submit' style='border-bottom:none' onclick='deleteHealthConditions(" + Conditions[i].conditionID + ");'>Delete</button></li>" +
                                            "</ul>" +
                                        "</div>"
        ]);

    }
    
    $('#conditionstable').DataTable().draw();

}
function ConverttoDateHealthConditions(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
}
function editHealthConditions(objCondition) {
    $("#myCondition").val(objCondition.conditionName);
    $("#conditionID").val(objCondition.conditionID);
    _objUpdate = {};
    _objUpdate["conditionName"] = (objCondition.conditionName);
    _objUpdate["conditionID"] = (objCondition.conditionID);
    _objUpdate["patientID"] = (objCondition.patientID);
    _conditionID = objCondition.conditionID;
   }
function resetCondition() {
   
    $("#myCondition").val('');
    $("#conditionID").val('0');
    _objUpdate = null;
   _objAdd = null;
    
}



//
function addupdateCondition(_patientId) {
   
    var msg = ValidateFormHealthConditions();
    if (msg == "" || msg == undefined) {
        fillObjHealthConditions(_patientId);

        var condition;
        if (_objUpdate == null) {
            condition = _objAdd;
           _conditionID = 0;
        }
        else {
            _objUpdate.conditionName = $("#myCondition").val();
            condition = _objUpdate;
        }
        
        $.ajax({
            type: 'POST',
            url: '/SeeDoctor/AddUpdateCondition',
            data: { 'conditionID': parseInt(_conditionID), 'condition': condition },
            dataType: 'json',
            success: function (response) {
                if (response.Message != undefined)
                {
                    new PNotify({
                        title: 'Error',
                        text: response.Message,
                        type: 'error',
                        styling: 'bootstrap3'
                    });
                }
                if (response.Success == true) {
                    if(response.ApiResultModel.message!="")
                   {
                        new PNotify({
                            title: 'Error',
                            text: response.ApiResultModel.message,
                            type: 'error',
                            styling: 'bootstrap3'
                        });
                    }
                    else if(response.ApiResultModel.message==""){
                        new PNotify({
                            title: 'Success',
                            text: "Condition is saved successfully.",
                            type: 'info',addclass: 'dark',
                            styling: 'bootstrap3'
                        });
                        if (_objAdd != null) {
                            var _newObj = {};
                            _newObj["conditionID"] = response.ApiResultModel.ID;
                            _newObj["patientID"] = _objAdd.patientID;
                            _newObj["conditionName"] = _objAdd.conditionName;
                            _newObj["reportedDate"] = "/Date(" + ticks + ")/";
                           _conditionTable.splice(0, 0, _newObj);
                           bindConditionsTable(_conditionTable);
                            //_objAdd = null;
                           resetCondition();
                           

                        }
                        else if (_objAdd == null) {
                            changeCondition(response.ApiResultModel.ID, _objUpdate.conditionName);
                            bindConditionsTable(_conditionTable);
                            resetCondition();
                           //_objUpdate = null;
                            
                        }
                       
                    }
                   
                   
                            
                }
               
            },
            //error: errorRes

        });

    }
    else {
        alert(msg);
    }

   
}

function deleteHealthConditions(conditionID) {
    var confirmMessage = confirm("Are you sure you want to delete?");
    if (confirmMessage == false)
        return false;


    $.ajax({
        type: 'POST',
        url: '/SeeDoctor/DeleteCondition',
        data: { 'conditionID': parseInt(conditionID) },
        dataType: 'json',
        success: function (response) {
            if (response.Message != undefined) {
                new PNotify({
                    title: 'Error',
                    text: response.Message,
                    type: 'error',
                    styling: 'bootstrap3'
                });
            }
            if (response.Success == true)
            {
                
                if(response.ApiResultModel.message=="")
                    new PNotify({
                        title: 'Success',
                        text: "Condition is deleted successfully.",
                        type: 'info',addclass: 'dark',
                        styling: 'bootstrap3'
                    });
                    removeCondition(response.ApiResultModel.ID);
                    bindConditionsTable(_conditionTable);
                   
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
        //error: errorRes

    });
}


function UpdateConditionTable(Condition)
{
    var tableHtml = "";
    var table = $('#conditionstable').DataTable();
   // table.row.add($("<tr class='even pointer' id='row" + Condition.conditionID + "'>"
   //     + "<td>" + Condition.conditionID + "</td>"
   // + "<td style='word-break:break-all'>" + Condition.conditionName + "</td>"
   //+"<td>" + getCurrentDate() + "</td>"
   //+ "<td style='width:100px'> <div class='btn-group'> <button type='button' class='btn btn-primary'>Action</button>" +
   //                                     "<button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown' aria-expanded='false'>" +
   //                                         "<span class='caret'></span>" +
   //                                         "<span class='sr-only'>Toggle Dropdown</span>" +
   //                                     "</button>" +
   //                                     "<ul class='dropdown-menu' role='menu'>" +
   //                                        "<li>" +
   //                                         "<a class='editbtn' href='#'  onclick='editObj(" + JSON.stringify(Condition) + ",this);'>Edit</a>" +
   //                                         "</li>" +
   //                                         "<li>" +
   //                                          "<button id='delete' type='button' class='btn btn-link submit' style='border-bottom:none' onclick='deleteObj(" + Condition.conditionID + ")'>Delete</button></li>" +
   //                                     "</ul>" +
    //                                 "</div></td></tr>")).draw(true);
    $('#conditionstable').dataTable().fnAddData([
             i + 1,
             Conditions.conditionName,
              ToJavaScriptDate(Conditions[i].reportedDate),
              "<div class='btn-group'> <button type='button' class='btn btn-primary'>Action</button>" +
                                            "<button type='button' class='btn btn-primary dropdown-toggle' data-toggle='dropdown' aria-expanded='false'>" +
                                                "<span class='caret'></span>" +
                                                "<span class='sr-only'>Toggle Dropdown</span>" +
                                            "</button>" +
                                            "<ul class='dropdown-menu' role='menu'>" +
                                               "<li>" +
                                                "<a class='editbtn' href='#'  onclick='editObj(" + JSON.stringify(Conditions[i]) + ",this);'>Edit</a>" +
                                                "</li>" +
                                                "<li>" +
                                                 "<button id='delete' type='button' class='btn btn-link submit' style='border-bottom:none' onclick='deleteObj(" + Conditions[i].conditionID + ");'>Delete</button></li>" +
                                            "</ul>" +
                                        "</div>"
    ]);
   
}
function getCurrentDateHealthConditions()
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

    return today = mm + '/' + dd + '/' + yyyy;
}
function fillObjHealthConditions(_patientId) {

    if (_objUpdate == null) {
        _objAdd = {};
        _objAdd["conditionName"] = $("#myCondition").val();
        _objAdd["patientID"] = _patientId;
    }
    
}
function ValidateFormHealthConditions() {
    var success = "";

    if ($("#myCondition").val() == "") {
        success = "Please enter condition name";
       
    }
    return success;
    
}

function changeCondition(value, desc) {
    for (var i in _conditionTable) {
        if (_conditionTable[i].conditionID == value) {
            _conditionTable[i].conditionName = desc;
            break;
        }
    }
}
function removeCondition(value)
{
    for (var i in _conditionTable) {
        if (_conditionTable[i].conditionID == value) {
            _conditionTable.splice(i, 1);
            break;
        }
    }
}






