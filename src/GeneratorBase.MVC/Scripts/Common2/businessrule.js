function ApplyBusinessRuleOnSubmit(dataurl, entityname, isinline, lblerrormsg, form, assocname, assocdispname) {
    var flagvalidate = true;
    var flagmandatory = true;
    flagvalidate = ValidateBeforeSavePropertiesRule(dataurl.replace("businessruletype", "GetValidateBeforeSaveProperties"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    flagmandatory = MandatoryPropertiesRule(dataurl.replace("businessruletype", "GetMandatoryProperties"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    return flagmandatory && flagvalidate;
}

function ApplyBusinessRuleOnPageLoad(dataurl, entityname, isinline, lblerrormsg, form, assocname, assocdispname) {
    LockBusinessRules(dataurl.replace("businessruletype", "GetLockBusinessRules"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
    ReadOnlyPropertiesRule(dataurl.replace("businessruletype", "GetReadOnlyProperties"), entityname, isinline, lblerrormsg, form, assocname, assocdispname);
}

function MandatoryPropertiesRule(dataurl, entityname, isinline, lblerrormsg, form, assocname, assocdispname) {
    document.getElementById(lblerrormsg).value = "";
    var flag = true;
    var failuremsg = "";
    var infomsg = "";
    $.ajax({
        async: false,
        type: "GET",
        url: dataurl,
        data: form,
        success: function (data) {
            $('[businessrule="mandatory"]').each(function () {
                $(this).removeAttr('required');
            });
           
            for (var key in data) {
                var propKey = key;
                if (isinline)
                    propKey = assocname.toLowerCase() + "_" + key;

                if (!(key.toLowerCase().indexOf("informationmessage") >= 0)) {
                    if ($.trim($('#' + propKey).val()).length == 0 && $.trim($("input[type='radio'][name='" + propKey + "']:checked").val()).length == 0) {
                        if (key.toLowerCase().indexOf("failuremessage") >= 0) {
                            if (data[key] != null)
                                failuremsg += "<br/>" + data[key] + "<br/>";
                        }
                        else {
                            if (isinline)
                                failuremsg += assocdispname + "." + data[key] + ",";
                            else
                                failuremsg += data[key] + ",";

                            $('#' + propKey).attr('required', 'required');
                            $('#' + propKey).attr('businessrule', 'mandatory');
                            $("input[type='radio'][name='" + propKey + "']").attr('required', 'required');
                            $("input[type='radio'][name='" + propKey + "']").attr('businessrule', 'mandatory');
                            flag = false;
                        }
                    }
                }
                else {
                    if (data[key] != null)
                        infomsg += data[key] + " | ";
                }
            }
            if (infomsg != "")
                if (flag)
                    alert(infomsg.replace(infomsg.substring(infomsg.lastIndexOf(" | ")), ""));
            if (failuremsg != "")
                if (!flag) {
                    document.getElementById(lblerrormsg).value += failuremsg;
                    $("#divDisplayBRmsgMandatory").removeAttr("style");
                    $("#divDisplayBRmsgMandatory").html(getMsgTableMandatory());
                    document.getElementById("ErrMsgMandatory").innerHTML = document.getElementById(lblerrormsg).value

                }
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
    return flag;
}

function ValidateBeforeSavePropertiesRule(dataurl, entityname, isinline, lblerrormsg, form, assocname, assocdispname) {
    document.getElementById(lblerrormsg).value = "";
    var flag = true;
    var failuremsg = "";
    var infomsg = "";
    $.ajax({
        async: false,
        type: "GET",
        url: dataurl,
        data: form,
        success: function (data) {
            //for (var key in data) {
            //    flag = false;
            //    document.getElementById('ErrMsg').innerHTML += key + "Record is not saved as - ( " + data[key] + " ).<br/>";
            //}
            for (var key in data) {
                if (!(key.toLowerCase().indexOf("informationmessage") >= 0)) {
                    flag = false;;
                    if (key.toLowerCase().indexOf("failuremessage") >= 0) {
                        if (data[key] != null)
                            //document.getElementById(lblerrormsg).innerHTML += data[key] + ".<br/>";
                            failuremsg += "<br/>" + data[key] + "<br/>";
                    }
                    else {

                        if (!isinline)
                            failuremsg += key + data[key] + ",";
                        else
                            failuremsg += key + assocdispname + "." + data[key] + ",";
                    }
                }
                else {
                    if (data[key] != null)
                        infomsg += data[key] + " | ";
                }

            }
            if (infomsg != "")
                if (flag)
                    alert(infomsg.replace(infomsg.substring(infomsg.lastIndexOf(" | ")), ""));
            if (failuremsg != "")
                if (!flag) {
                    document.getElementById(lblerrormsg).value += failuremsg;
                    $("#divDisplayBRmsgBeforeSaveProp").removeAttr("style");
                    $("#divDisplayBRmsgBeforeSaveProp").html(getMsgTableBeforeSaveProp());
                    document.getElementById("ErrMsgRuleBeforeSaveProp").innerHTML = document.getElementById(lblerrormsg).value

                }
        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
    return flag;
}

function LockBusinessRules(dataurl, entityname, isinline, lblerrormsg, form, assocname, assocdispname) {
    document.getElementById(lblerrormsg).value = "";
    var flag = true;
    var failuremsg = "";
    var infomsg = "";
    $.ajax({
        async: false,
        type: "GET",
        url: dataurl,
        data: form,
        success: function (data) {
            //if (data.length > 0) {
            //    $(':input:not([readonly])', 'form').attr('disabled', 'disabled').attr('readonly', 'readonly').trigger("chosen:updated");
            //    document.getElementById('ErrMsg').innerHTML = data + " Rules applied";
            //}
           
            for (var key in data) {
                if (!(key.toLowerCase().indexOf("informationmessage") >= 0)) {
                    if ($.trim($('#' + key).val()).length == 0 && $.trim($("input[type='radio'][name='" + key + "']:checked").val()).length == 0) {
                        flag = false;
                        if (key.toLowerCase().indexOf("failuremessage") >= 0) {
                            if (data[key] != null)
                                failuremsg += "<br/>" + data[key] + "<br/>";
                        }
                        else
                            failuremsg += key + " - " + data[key] + ",";
                    }
                }
                else {
                    if (data[key] != null)
                        infomsg += data[key] + " | ";
                }

            }
            if (!flag) {
                if (!isinline)
                    $(':input:not([readonly])', 'form').attr('disabled', 'disabled').attr('readonly', 'readonly').trigger("chosen:updated");
                else
                    $('#dv' + assocname + 'ID :input:not([readonly])', 'form').attr('disabled', 'disabled').attr('readonly', 'readonly').trigger("chosen:updated");
                //dvT_CurrentEmployeeJobAssignmentID
            }
            if (infomsg != "")
                if (flag)
                    alert(infomsg.replace(infomsg.substring(infomsg.lastIndexOf(" | ")), ""));
            if (failuremsg != "")
                if (!flag) {
                    document.getElementById(lblerrormsg).value += failuremsg;
                    $("#divDisplayLockRecord").removeAttr("style");
                    $("#divDisplayLockRecord").html(getMsgTableLockBR());
                    document.getElementById("ErrmsgLockRecord").innerHTML = document.getElementById(lblerrormsg).value

                }

        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}

function ReadOnlyPropertiesRule(dataurl, entityname, isinline, lblerrormsg, form, assocname, assocdispname) {
    document.getElementById(lblerrormsg).value = "";
    var flag = true;
    var failuremsg = "";
    var infomsg = "";
    $.ajax({
        async: false,
        type: "GET",
        url: dataurl,
        data: form,
        success: function (data) {
           
            for (var key in data) {
                if (!(key.toLowerCase().indexOf("informationmessage") >= 0)) {
                    flag = false;
                    if (isinline)
                        key = assocname.toLowerCase() + "_" + key;
                    $('#' + key).attr('disabled', 'disabled').attr('readonly', 'readonly').trigger("chosen:updated");
                    $("#dv" + key + " :input").attr("disabled", "disabled").attr('readonly', 'readonly').trigger("chosen:updated");
                    $("input[type='radio'][name='" + key + "']").attr('disabled', 'disabled').attr('readonly', 'readonly');
                    $('form').append('<input type="hidden" name="' + key + '" id="' + key + '" value="' + $('#' + key).val() + '" />');

                    if (key.toLowerCase().indexOf("failuremessage") >= 0) {
                        if (data[key] != null)
                            failuremsg += "<br/>" + data[key] + "<br/>";
                    }
                    //else
                    //    failuremsg += data[key] + ",";
                }
                else {
                    if (data[key] != null)
                        infomsg += data[key] + " | ";
                }

            }
            if (infomsg != "")
                if (flag) {
                    alert(infomsg.replace(infomsg.substring(infomsg.lastIndexOf(" | ")), ""));
                }
            if (failuremsg != "")
                if (!flag) {
                    document.getElementById(lblerrormsg).value += failuremsg;
                    $("#divDisplayBRReadOnly").removeAttr("style");
                    $("#divDisplayBRReadOnly").html(getMsgTableReadOnly());
                    document.getElementById("ErrmsgtrRuleReadOnlyProp").innerHTML = document.getElementById(lblerrormsg).value
                }

        },
        error: function (jqXHR, textStatus, errorThrown) {
        }
    });
}

function Check1MThresholdLimit(e, form, dataurl, entityname, lblerrormsg) {
    document.getElementById(lblerrormsg).value = "";
	var flag = true;
    $.ajax({
        async: false,
        type: "GET",
        url: dataurl,
        data: form,
        success: function (data) {
            var list = document.createElement('ul');
            $.each(data, function (key, value) {
                flag = false;
                document.getElementById(lblerrormsg).value += "Threshold limit <u>[" + value + "]</u> is reached for <u>[" + key + "]</u><br/>";
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //alert(errorThrown + '-' + textStatus);
        }
    });
    if (!flag) {
        e.preventDefault();
        $("#divDisplayThresholdLimit").removeAttr("style");
        $("#divDisplayThresholdLimit").html(getMsgTableThresholdLimit());
        document.getElementById("ErrmsgtrThresholdLimit").innerHTML = document.getElementById(lblerrormsg).value;
    }
    return flag;
}

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

//dispaly msg
function getMsgTableMandatory() {
    var Msgtable = "<table class='table table-bordered table-hover table-condensed'>" +
                    "<tr class='header expand' id='trRuleMandatory'>" +
                    "<td colspan='8' style='background: #EDF5FA; font-weight:bold'><span class='sign'></span>Mandatory Fields</td>" +
                    "</tr><tr>" +
                    "<td><label id='ErrMsgMandatory' class='text-primary small pull-left' style='color:brown;vertical-align:middle;font-size: 12px;'></label></td>" +
                    "</tr>" +
                   "</table>" +
                "<script>" +
                "$('#trRuleMandatory').click(function () {" +
                    "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
                "</script>";

    return Msgtable;
}
function getMsgTableBeforeSaveProp() {
    var Msgtable = "<table class='table table-bordered table-hover table-condensed'>" +
                "<tr class='header expand' id='trRuleBeforeSaveProp'>" +
                "<td colspan='8' style='background: #EDF5FA; font-weight:bold'><span class='sign'></span>Record Is Not Saved As</td>" +
                "</tr><tr>" +
                "<td><label id='ErrMsgRuleBeforeSaveProp' class='text-primary small pull-left' style='color:brown;vertical-align:middle;font-size: 12px;'></label></td>" +
                "</tr>" +
                "</table>" +
                "<script>" +
                "$('#trRuleBeforeSaveProp').click(function () {" +
                    "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
                "</script>";

    return Msgtable;

}
function getMsgTableLockBR() {
    var Msgtable = "<table class='table table-bordered table-hover table-condensed'>" +
               "<tr class='header expand' id='trRuleRLockRecord'>" +
               "<td colspan='8' style='background: #EDF5FA; font-weight:bold'><span class='sign'></span>Lock Record</td>" +
               "</tr><tr>" +
               "<td><label id='ErrmsgLockRecord' class='text-primary small pull-left' style='color:brown;vertical-align:middle;font-size: 12px;'></label></td>" +
               "</tr>" +
               "</table>" +
               "<script>" +
               "$('#trRuleRLockRecord').click(function () {" +
                   "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
               "</script>";

    return Msgtable;
}
function getMsgTableReadOnly() {
    var Msgtable = "<table class='table table-bordered table-hover table-condensed'>" +
               "<tr class='header expand' id='trRuleReadOnlyProp'>" +
               "<td colspan='8' style='background: #EDF5FA; font-weight:bold'><span class='sign'></span>ReadOnly Properties</td>" +
               "</tr><tr>" +
               "<td><label id='ErrmsgtrRuleReadOnlyProp' class='text-primary small pull-left' style='color:brown;vertical-align:middle;font-size: 12px;'></label></td>" +
               "</tr>" +
               "</table>" +
               "<script>" +
               "$('#trRuleReadOnlyProp').click(function () {" +
                   "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
               "</script>";

    return Msgtable;
}
function getMsgTableThresholdLimit() {
    var Msgtable = "<table class='table table-bordered table-hover table-condensed'>" +
               "<tr class='header expand' id='trThresholdLimit'>" +
               "<td colspan='8' style='background: #EDF5FA; font-weight:bold'><span class='sign'></span>Threshold Limit Message</td>" +
               "</tr><tr>" +
               "<input  name='hdntxt' type='text' style='border:none;width:0px;height:0px' readonly required>" +
               "<td><label id='ErrmsgtrThresholdLimit' class='text-primary small pull-left' style='color:brown;vertical-align:middle;font-size: 12px;'></label></td>" +
               "</tr>" +
               "</table>" +
               "<script>" +
               "$('#trThresholdLimit').click(function () {" +
                   "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
               "</script>";

    return Msgtable;
}
function getMsgTableCodeFragment() {
    var Msgtable = "<table class='table table-bordered table-hover table-condensed'>" +
               "<tr class='header expand' id='trCodeFragment'>" +
               "<td colspan='8' style='background: #EDF5FA; font-weight:bold'><span class='sign'></span></td>" +
               "</tr><tr>" +
               "<input  name='hdntxt' type='text' style='border:none;width:0px;height:0px' readonly required>" +
               "<td><label id='ErrmsgtrCodeFragment' class='text-primary small pull-left' style='color:brown;vertical-align:middle;font-size: 12px;'></label></td>" +
               "</tr>" +
               "</table>" +
               "<script>" +
               "$('#trCodeFragment').click(function () {" +
                   "$(this).toggleClass('expand').nextUntil('tr.header').slideToggle(500);});" +
               "</script>";

    return Msgtable;
}