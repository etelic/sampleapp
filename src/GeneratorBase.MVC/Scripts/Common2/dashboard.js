$(function () {
    $(window).bind("load resize", function () {
        if ($(this).width() < 768) {
            $('div.sidebar-collapse').addClass('collapse')
        } else {
            $('div.sidebar-collapse').removeClass('collapse')
        }
    })
})
function FacetedSearch(Entity, Asso, Prop, viewtype, sortby, isAsc, currentFilter, page) {
    var urlstring = $("#" + "fSearch" + Entity).attr("dataurl");
    var association = Asso.split(",");
    var property = Prop.split(",");
    var firstparam = 0;
    for (i = 0; i < association.length; i++) {
        var vals = "";
        ele = document.getElementById(association[i]);
        if (ele != null)
            for (var o = 0; o < ele.options.length; o++) {
                if (ele.options[o].selected)
                    vals += ele.options[o].value + ",";
            }
        if (vals.length > 0) {
            if (firstparam == 0)
                urlstring += "?" + association[i] + "=" + vals;
            else
                urlstring += "&" + association[i] + "=" + vals;
            firstparam = 1;
        }
    }
    for (i = 0; i < property.length; i++) {
        ele = document.getElementById(property[i]);
        if (ele != null)
            if (ele.value.length > 0) {
                if (firstparam == 0)
                    urlstring += "?" + property[i] + "=" + ele.value;
                else
                    urlstring += "&" + property[i] + "=" + ele.value;
                firstparam = 1;
            }
    }
    var page_sortstring = "";
    if (sortby != "") {
        page_sortstring = "&sortBy=" + sortby;
        if (isAsc != "")
            page_sortstring += "&isAsc=" + isAsc;
    }
    // if (viewtype != '') {
    //    page_sortstring += "&viewtype=" + viewtype;
    // }
    if (currentFilter != '') {
        if (firstparam == 0)
            urlstring += "?searchString=" + currentFilter + page_sortstring;
        else
            urlstring += "&searchString=" + currentFilter + page_sortstring;
    }
    if (firstparam == 0)
        urlstring += "?search=" + SanitizeURLString(document.getElementById("FSearch").value) + page_sortstring;
    else
        urlstring += "&search=" + SanitizeURLString(document.getElementById("FSearch").value) + page_sortstring;
    urlstring = addParameterToURL(urlstring, "SortOrder", $("#SortOrder").val());
    urlstring = addParameterToURL(urlstring, "GroupByColumn", $("#hdnGroupByColumn").val());
    urlstring = addParameterToURL(urlstring, "HideColumns", $("#HideColumns").val());
    urlstring = addParameterToURL(urlstring, "viewtype", $("#DisplayLayout").val());
    urlstring = addParameterToURL(urlstring, "FilterCondition", $("#FilterCondition").val());
    window.location = (urlstring);
}
function ClearChildDD(child, obj, isreverse) {
    $("#" + child).html("<option value=''>--Select--</option>");
    $("#" + child).val('');
    var IsReverse = isreverse;//$(obj).attr("IsReverse");
    if (IsReverse == "true" || IsReverse == "True" && $(obj).val().length > 0) {
        $("#" + child).removeAttr("lock");
        $("#" + child).trigger('chosen:updated');
        $("#" + child).trigger('chosen:open');
        $("#" + child).trigger('change');
        $("#" + child).attr("lock", "true");
    }
    else {
        // if ($(obj).val().length > 0)
        $("#" + child).removeAttr("lock");
        // else
        //  $("#" + child).attr("lock", "true");
    }
    $("#" + child).trigger('chosen:updated');
}
function ClearMultiSelectChild(child) {
    $('#' + child).multiselect('clearSelection');
}
function FillCascadingDD(parent, child, childName, assoName) {
    {
        var parentDD = $("#" + parent);
        var childDD = $("#" + child);
        var selectedval = $("option:selected", parentDD).val();
        $.ajax({
            type: "GET",
            url: "/" + childName + "/GetValuesByAssociation/" + selectedval + "?HostedBy=" + assoName,
            contentType: "application/json; charset=utf-8",
            global: false,
            cache: false,
            async: true,
            dataType: "json",
            beforeSend: function () {
                $("#" + child).html("<option value=''>Please wait...</option>")
                $("body").css('cursor', 'wait');
                $("#" + child).attr("size", "1");
            },
            complete: function () {
                $("body").css('cursor', 'default');
            },
            success: function (jsonObj) {
                var listItems = "";
                $("#" + child).empty();
                listItems += "<option value=''>--None--</option>";
                for (i in jsonObj) {
                    listItems += "<option value='" + jsonObj[i].Id + "'>" + jsonObj[i].Name + "</option>";
                }
                $("#" + child).html(listItems);
                $("#" + child).focus(down).blur(up).focus();
            }
        });
        $("#" + child).click(function () {
            up();
        });
    } return false;
    function down() {
        var pos = $("#" + child).offset();
        var len = $("#" + child).find("option").length;
        if (len > 10) {
            len = 10;
        }
        $("#" + child).css("position", "absolute");
        $("#" + child).css("zIndex", 100);
        $("#" + child).offset(pos);   // reset position
        $("#" + child).attr("size", len); // open dropdown
        $("#" + child).unbind("focus", down);
        $("#" + child).focus();
    }
    function up() {
        $("#" + child).css("position", "static");
        $("#" + child).attr("size", "1");  // close dropdown
        $("#" + child).unbind("blur", up);
        $("#" + child).focus(); $("#" + child).removeAttr("size");
    }
}
function getPath() {
    var path = "";
    nodes = window.location.pathname.split('/');
    for (var index = 0; index < nodes.length - 3; index++) {
        path += "../";
    }
    return path;
}
function OpenQuickQddPopUp(dvName) {
    var url = $("#" + dvName).data("url");
    $("#" + dvName).load(url);
}
function NavigateToUrl(url) {
    window.location.replace(url);
}
function OpenDashBoard(dvName) {
    var url = $("#" + dvName).data("url");//+"?TS="+Date.now();
    $("#" + dvName).load(url);
}
function OpenDashBoardFromHome(obj, dvName, entName) {
    var url = $(obj).attr("dataurl");//+"?TS="+Date.now();
    $("#EntityGraphLabel").html(entName)
    $("#" + dvName).load(url);
}
function OpenPopUpGraph(Popup, dvName, url) {
    $("#" + Popup).modal('show');
    $("#" + Popup).find('.modal-dialog.ui-draggable').attr("style", "width:90%");
    $("#" + dvName).html('');
    $("#" + dvName).load(url);
}
//Refresh index list 
function RefreshGrid(dvName, url) {
    $("#" + dvName).load(url);
}
function ClearTabCookie(username) {
    $.cookie(username + "TabCookie", null);
}
function LoadTab(dvName, username, url) {
    if (dvName.length > 0)
        $.cookie(username + "TabCookie", dvName);
    if ($.trim($("#" + dvName).html()).length == 0) {
        $("#" + dvName).html('Please wait..');
        $("#" + dvName).load(url);
    }
}
function LoadTabMobile(dvName, url) {
    if ($.trim($("#" + dvName).html()).length == 0) {
        $("#" + dvName).html('Please wait..');
        $("#" + dvName).load(url);
    }
}
function LoadTabTemplate(dvName, url) {
    $("#" + dvName).html('Please wait..');
    $("#" + dvName).load(url);
}
//Refresh index list 
function CancelSearch(dvName, url, UserName) {
    //remove pagination cookies 
    if ($.cookie("pagination" + UserName + dvName) != null)
        $.removeCookie("pagination" + UserName + dvName);
    var host = (getHostingEntityID(url)["AssociatedType"]);
    var IsFilter = (getHostingEntityID(url)["IsFilter"]);
    var IsdrivedTab = (getHostingEntityID(url)["IsdrivedTab"]);
    $.ajax({
        url: url,
        cache: false,
        success: function (data) {
            if (data != null) {
                if (host != undefined && IsFilter != "True" && $('#' + host).length > 0) {
                    $('#' + dvName, $('#' + host)).html(data);
                    if (IsdrivedTab) {
                        $("a[href='" + host + "']").trigger("click");
                    }
                }
                else {
                    try {
                        $('#' + dvName).html(data);
                        if (IsdrivedTab) {
                            $("a[href='" + host + "']").trigger("click");
                        }
                    } catch (ex) { }
                }
            }
        }
    })
    return false;
}
function CancelSearchBulk(dvName, url, UserName) {
    //remove pagination cookies 
    if ($.cookie("pagination" + UserName + dvName) != null)
        $.removeCookie("pagination" + UserName + dvName);
    var host = (getHostingEntityID(url)["AssociatedType"]);
    var IsFilter = (getHostingEntityID(url)["IsFilter"]);
    $.ajax({
        url: url,
        cache: false,
        success: function (data) {
            if (data != null) {
                if (host != undefined && IsFilter != "True" && $('#' + host).length > 0) {
                    $($("." + dvName)[1]).html(data);
                    //$('#' + dvName, $('#' + host)).html(data);
                }
                else {
                    try {
                        $('#' + dvName).html(data);
                    } catch (ex) { }
                }
            }
        }
    })
    return false;
}
function OpenPopUpEntityBR(Popup, EntityName, dvName, url) {
    $("#" + Popup + 'Label').html();
    $("#" + Popup + 'Label').html("" + EntityName);
    $("#" + Popup).modal('show');
    $("#" + dvName).html('');
    $("#" + dvName).load(url);
}
//
//Quick Add From pop window for drop down
function OpenPopUpEntity(Popup, EntityName, dvName, url) {
    $("#" + Popup + 'Label').html();
    if ((EntityName.indexOf("Edit") == -1) && (EntityName.indexOf("Delete") == -1)) {
        $("#" + Popup + 'Label').html("Add " + EntityName);
    }
    else {
        $("#" + Popup + 'Label').html(EntityName);
    }
    $("#" + Popup).modal('show');
    $("#" + dvName).html('');
    $("#" + dvName + "Error").html('');
    $("#" + dvName).load(url);
}
function OpenPopUpEntity1M(obj, Popup, EntityName, dvName, url) {
    $("#" + Popup + 'Label').html();
    var HostingDispVal = ($("#HostingEntityDisplayValue").html());
    var value1 = $(obj).attr("data-original-title");
    $("#" + Popup + 'Label').html(value1 + " " + HostingDispVal);
    $("#" + Popup).modal('show');
    $("#" + dvName).html('');
    $("#" + dvName + "Error").html('');
    $("#" + dvName).load(url);
}
function OpenPopUpCopyEntity(Popup, EntityName, dvName, url) {
    $("#" + Popup + 'Label').html('');
    //$("#" + Popup + 'Label').html("Add " + EntityName);
    $("#" + Popup).modal('show');
    $("#" + dvName).html('');
    $("#" + dvName + "Error").html('');
    $("#" + dvName).load(url);
}
function OpenPopUpEntityMobile(Popup, EntityName, dvName, ddname, url) {
    $("#" + Popup + 'Label').html();
    $("#" + Popup + 'Label').css({ 'color': "black" });
    $("#" + Popup + 'Label').html("Add " + EntityName);
    $("#" + Popup).modal('show');
    $("#" + dvName).html('');
    $("#" + dvName + "Error").html('');
    $("#" + dvName).load(url);
    $("#" + dvName).attr('accesskey', ddname);
}
function OpenPopUpBulkOperation(Popup, EntityName, DispName, dvName, url) {
    $("#" + Popup + 'Label').html();
    $("#" + Popup + 'Label').attr('class', EntityName);
    $("#" + Popup + 'Label').html(DispName);
    $("#" + Popup).modal('show');
    $("#" + dvName).html('');
    $("#" + dvName).load(url);
}
function OpenPopUpBulkOperationBR(Popup, EntityName, DispName, dvName, url) {
    $("#" + Popup + 'Label').html();
    $("#" + Popup + 'Label').attr('class', "SuggestedPropertyValue");
    $("#" + Popup + 'Label').html(DispName);
    $("#" + Popup).modal('show');
    $("#" + dvName).html('');
    $("#" + dvName).load(url);
}
function QuickAdd(e, EntityName, bisrule, biscount, ruleType, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname) {
    $(e.currentTarget).attr('disabled', 'disabled');
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    //var p = BusineesRule(fd, url, bisrule, biscount, form);
    var p = BusineesRule(fd, url, bisrule, biscount, form, ruleType, EntityName, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname);
    if (!p)
        $(target).removeAttr("disabled");
    if (!form.valid()) { $(target).removeAttr("disabled"); return; }
    SaveServerTimeQuickAdd(form);
    try {
        var fd = new FormData(form[0]);
        $('input[type="file"]').each(function () {
            var file = $('#' + $(this)[0].id)[0].files;
            if (file.length) {
                fd.append($(this)[0].id, file[0]);
            }
        });
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            processData: false,
            contentType: false,
            success: function (result) {
                $(target).removeAttr("disabled");
                if (result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                } else {
                    $('#dvPopupError').html(result);
                }
            }
        });
    }
    catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            success: function (result) {
		 $(target).removeAttr("disabled");
                if (result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                } else {
                    $('#dvPopupError').html(result);
                }
            }
        });
    }
}
function QuickAddMobile(e, bisrule, biscount) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    BusineesRule(fd, url, bisrule, biscount, form)
    if (!form.valid()) return;
    try {
        var fd = new FormData(form[0]);
        $('input[type="file"]').each(function () {
            var file = $('#' + $(this)[0].id)[0].files;
            if (file.length) {
                fd.append($(this)[0].id, file[0]);
            }
        });
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            processData: false,
            contentType: false,
            success: function (result) {
                if (result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    FillDropdownMobile(dropdown);
                }
            }
        });
    }
    catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            success: function (result) {
                if (result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    FillDropdownMobile(dropdown);
                }
            }
        });
    }
    var dropdown = ($('#dvPopup').attr('accesskey'));
    if (dropdown != undefined) {
        FillDropdownMobile(dropdown);
        $("#" + dropdown).change();
    }
}
function QuickAddFromIndex(e, isrefresh, Entity, host, bisrule, biscount, ruleType, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname) {
    $(e.currentTarget).attr('disabled', 'disabled')
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    var p = BusineesRule(fd, url, bisrule, biscount, form, ruleType, Entity, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname);
    if (!p) {
        $(target).removeAttr("disabled");
        return false;
    }
    if (!form.valid()) { $(target).removeAttr("disabled"); return; }
    SaveServerTimeQuickAdd(form);
    try {
        fd = new FormData(form[0]);
        $('input[type="file"]').each(function () {
            var file = $('#' + $(this)[0].id)[0].files;
            if (file.length) {
                fd.append($(this)[0].id, file[0]);
            }
        });
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                $(target).removeAttr("disabled");
                if (result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                        }
                        else
                            $('#' + Entity + 'SearchCancel').click();
                    }
                } else {
                    $('#dvPopupError').html(result);
                }
            }
        });
    } catch (ex) {
        $(target).removeAttr("disabled");
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            success: function (result) {
                if (result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                        }
                        else
                            $('#' + Entity + 'SearchCancel').click();
                    }
                } else {
                    $('#dvPopupError').html(result);
                }
            }
        });
    }
    if (Entity.indexOf('Events') > -1) {
        location.reload();
    }
}
function BusineesRule(fd, url, bisrule, bisCount, form, ruleType, entityname, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname) {

    var BruleUrlMandatory = url;
    var BruleUrlValidate = url;
    var BruleUrl = url;
    if (url.indexOf("CreateQuick") >= 0) {
        BruleUrl = url.replace("CreateQuick", "businessruletype");
        BruleUrl = BruleUrl + "?ruleType=" + ruleType;
    }
    else if (url.indexOf("EditQuick") >= 0) {
        BruleUrl = url.replace("EditQuick", "businessruletype");
        BruleUrl = BruleUrl + "?ruleType=" + ruleType;
    }
    var flag = true;
    
    if (bisrule != null && bisCount > 0) {
        flag = ApplyBusinessRuleOnSubmit(BruleUrl, entityname, isinline, lblerrormsg, fd, "", "");
    }
    if (lstinlineassocname != "" && lstinlineassocname != null) {
        lstinlineassocname = lstinlineassocname.trim(',');
        var arrassoc = lstinlineassocname.split(',');

        lstinlineassocdispname = lstinlineassocdispname.trim(',');
        var arrassocdispname = lstinlineassocdispname.split(',');

        lstinlineentityname = lstinlineentityname.trim(',');
        var arrassocentityname = lstinlineentityname.split(',');

        $.each(arrassoc, function (index, value) {
            var formdiv = form.find('#dv' + value + 'ID :input').serialize();
            formdiv = formdiv.replaceAll(value.toLowerCase() + ".", "");
            flag = flag && ApplyBusinessRuleOnSubmit(BruleUrl.replace(entityname, arrassocentityname[index]), arrassocentityname[index], true, lblerrormsg, formdiv, value, arrassocdispname[index]);
        });
    }
    return flag;
}
function getHostingEntityID(url) {
    var vars = [], hash;
    var hashes = url.slice(url.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        if ($.inArray(hash[0], vars) > -1) {
            vars[hash[0]] += "," + hash[1];
        }
        else {
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
    }
    return vars;
}
function EntityFilter(EntityName, url, dataurl, UserName) {
    var _username = UserName;
    UserName = (encodeURI(UserName));

    if ($.cookie("pagination" + _username + EntityName) != null)
        $.removeCookie("pagination" + _username + EntityName);
    var FilterHostingEntityID = (dataurl.indexOf("FirstCall=True") > 0) ? undefined : getHostingEntityID(dataurl)["FilterHostingEntityID"];

    var IsWorkFlow = getHostingEntityID(dataurl)["IsWorkFlow"] == undefined ? false : true;
    var html = "<ul class=\"nav nav-tabs\">";
    var otherhtml = "<li><a class=\"hidden\" id=\"hiddendatatoggle\" data-toggle=\"tab\"></a><a onclick=\"$('#hiddendatatoggle').click();\" data-original-title=\"Filter-Groupby\" data-toggle=\"dropdown\" href=\"#\"> <span id=\"filtertabOther\">Other</span></a>";
    otherhtml += "<ul class=\"dropdown-menu\" role=\"menu\" style=\"max-height: 400px;overflow-y: auto;\">";
    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        dataType: "json",
        async: false,
        complete: function (result) { ClickFilterTabBtn(); },
        success: function (result) {
            var firstClick = "";
            var isother = false;
            for (i in result) {
                if (result[i].Id == undefined) continue;
                var isactive = false;
                if (FilterHostingEntityID != undefined) {
                    if (result[i].Id == FilterHostingEntityID) {
                        if (i < 10)
                            html += "<li name=\"" + result[i].Id + "\" class=\"active\">";
                        else {
                            otherhtml += "<li name=\"" + result[i].Id + "\" class=\"active\">";
                            isother = true;
                        }
                        isactive = true;
                    }
                    else {
                        if (i < 10)
                            html += "<li name=\"" + result[i].Id + "\">";
                        else {
                            otherhtml += "<li  name=\"" + result[i].Id + "\">";
                            isother = true;
                        }
                    }
                } else {
                    if (i == 0) {
                        html += "<li name=\"" + result[i].Id + "\" class=\"active\">";
                        isactive = true;
                    }
                    else {
                        if (i < 10)
                            html += "<li name=\"" + result[i].Id + "\">";
                        else {
                            otherhtml += "<li name=\"" + result[i].Id + "\">";
                            isother = true;
                        }
                    }
                }
                if (isother) {
                    if (IsWorkFlow)
                        otherhtml += "<a id=\"flt" + result[i].Id + "\" onclick=\"SetCookieFltTab('flt" + result[i].Id + "');$('#filtertabOther').html('Other-" + result[i].Name + "');CancelSearch('" + EntityName + "','" + dataurl + "&HostingEntityID=" + result[i].Id + "&Wfsearch=" + result[i].Name + "')\">" + result[i].Name + "</a>";
                    else
                        otherhtml += "<a id=\"flt" + result[i].Id + "\" onclick=\"SetCookieFltTab('flt" + result[i].Id + "');$('#filtertabOther').html('Other-" + result[i].Name + "');CancelSearch('" + EntityName + "','" + dataurl + "&HostingEntityID=" + result[i].Id + "')\">" + result[i].Name + "</a>";
                }
                else {
                    if (IsWorkFlow) {
                        html += "<a id=\"flt" + result[i].Id + "\" data-toggle=\"tab\"  onclick=\"SetCookieFltTab('flt" + result[i].Id + "');CancelSearch('" + EntityName + "','" + dataurl + "&HostingEntityID=" + result[i].Id + "&Wfsearch=" + result[i].Name + "','" + UserName + "')\">" + result[i].Name + "</a>";
                    }
                    else
                        html += "<a id=\"flt" + result[i].Id + "\" data-toggle=\"tab\"  onclick=\"SetCookieFltTab('flt" + result[i].Id + "');CancelSearch('" + EntityName + "','" + dataurl + "&HostingEntityID=" + result[i].Id + "','" + UserName + "')\">" + result[i].Name + "</a>";
                }
                html += "</li>";
                if (isother)
                    otherhtml += "</li>";
                if (isactive) {
                    if (IsWorkFlow) {
                        firstClick = dataurl + "&HostingEntityID=" + result[i].Id + "&Wfsearch=" + result[i].Name;
                    }
                    else
                        firstClick = dataurl + "&HostingEntityID=" + result[i].Id;
                }
            }
            html += "<li name=\"" + "null" + "\">";
            html += "<a data-toggle=\"tab\" id='None'  onclick=\"SetCookieFltTab('None');CancelSearch('" + EntityName + "','" + dataurl + "','" + UserName + "')\">" + "None" + "</a>";
            html += "</li>";
            //
            if (isother) {
                otherhtml += "</ul></li>";
                html += "<li>";
                html += otherhtml;
                html += "</li>";
            }
            //
            html += "</ul>";
            $("#dv" + EntityName + "Filter").html($(html));
            if (firstClick.length > 0 && firstClick.indexOf("FirstCall=True") > 0) {
                CancelSearch(EntityName, firstClick.replace("FirstCall=True", ""));
            }
        }
    });
}


//Entity Filter For BizRule
function EntityFilterBizRuleDisable(EntityName, dataurl, UserName) {
    var html = "<ul class=\"nav nav-tabs\">" +
    "<li name='false' class='active'>" +
    "<a data-toggle='tab'  onclick=\"CancelSearchBizRuleDisable('" + EntityName + "','" + dataurl + "&IsDisable=False','" + UserName + "')\">Enabled</a></li>" +
    "<li name='true'>" +
    "<a data-toggle='tab'  onclick=\"CancelSearchBizRuleDisable('" + EntityName + "','" + dataurl + "&IsDisable=True','" + UserName + "')\">Disabled</a></li>";
    //"<li name='null'><a data-toggle='tab' onclick=\"CancelSearchBizRuleDisable('" + EntityName + "','" + dataurl + "','" + UserName + "')\">All Record</a></li></ul>";
    $("#dv" + EntityName + "Filter").html($(html));
    var isactive = true;
    if (isactive) {
        firstClick = dataurl + "&IsDisable=false";
    }
    if (firstClick.length > 0 && firstClick.indexOf("FirstCall=True") > 0) {
        CancelSearchBizRuleDisable(EntityName, firstClick.replace("FirstCall=True&", ""));
    }
}
function CancelSearchBizRuleDisable(dvName, url, UserName) {
    //remove pagination cookies 
    if ($.cookie("pagination" + UserName + dvName) != null)
        $.removeCookie("pagination" + UserName + dvName);
    var IsFilter = (getHostingEntityID(url)["IsFilter"]);
    $.ajax({
        url: url,
        cache: false,
        success: function (data) {
            if (data != null) {
                try {
                    $('#' + dvName).html(data);
                } catch (ex) { }
            }
        }
    })
    return false;
}
function EntityFilterBizRule(EntityName, url, dataurl, UserName) {
    var _username = UserName;
    UserName = (encodeURI(UserName));
    if ($.cookie("pagination" + _username + EntityName) != null)
        $.removeCookie("pagination" + _username + EntityName);
    var FilterHostingEntityID = (dataurl.indexOf("FirstCall=True") > 0) ? undefined : getHostingEntityID(dataurl)["FilterHostingEntityID"];
    var html = "<ul class=\"nav nav-tabs\">";
    var otherhtml = "<li><a class=\"hidden\" id=\"hiddendatatoggle\" data-toggle=\"tab\"></a><a onclick=\"$('#hiddendatatoggle').click();\" data-original-title=\"Filter-Groupby\" data-toggle=\"dropdown\" href=\"#\"> <span id=\"filtertabOther\">Other</span></a>";
    otherhtml += "<ul class=\"dropdown-menu\" role=\"menu\" style=\"max-height: 400px;overflow-y: auto;\">";
    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        dataType: "json",
        success: function (result) {
            var firstClick = "";
            var isother = false;
            var i = 0;
            $.each(result, function (key, value) {
                var isactive = false;
                if (i == 0) {
                    html += "<li name=\"" + key + "\" class=\"active\">";
                    isactive = true;
                }
                else {
                    if (i < 10)
                        html += "<li name=\"" + key + "\">";
                    else {
                        otherhtml += "<li name=\"" + key + "\">";
                        isother = true;
                    }
                }
                if (isother) {
                    otherhtml += "<a onclick=\"$('#filtertabOther').html('Other-" + value + "');CancelSearchBizRule('" + EntityName + "','" + dataurl + "&HostingEntityName=" + key + "')\">" + value + "</a>";
                }
                else {
                    html += "<a data-toggle=\"tab\"  onclick=\"CancelSearchBizRule('" + EntityName + "','" + dataurl + "&HostingEntityName=" + key + "','" + UserName + "')\">" + value + "</a>";
                }
                html += "</li>";
                if (isother)
                    otherhtml += "</li>";
                if (isactive) {
                    firstClick = dataurl + "&HostingEntityName=" + key;
                }
                i += 1;
            });
            //html += "<li name=\"" + "null" + "\">";
            //html += "<a data-toggle=\"tab\"  onclick=\"CancelSearchBizRule('" + EntityName + "','" + dataurl + "','" + UserName + "')\">" + "All Record" + "</a>";
            //html += "</li>";
            //
            if (isother) {
                otherhtml += "</ul></li>";
                html += "<li>";
                html += otherhtml;
                html += "</li>";
            }
            //
            html += "</ul>";
            $("#dv" + EntityName + "Filter").html($(html));
            if (firstClick.length > 0 && firstClick.indexOf("FirstCall=True") > 0) {
                CancelSearchBizRule(EntityName, firstClick.replace("FirstCall=true&", ""));
            }
        }
    });
}
//Refresh index list 
function CancelSearchBizRule(dvName, url, UserName) {
    //remove pagination cookies 
    if ($.cookie("pagination" + UserName + dvName) != null)
        $.removeCookie("pagination" + UserName + dvName);
    var IsFilter = (getHostingEntityID(url)["IsFilter"]);
    $.ajax({
        url: url,
        cache: false,
        success: function (data) {
            if (data != null) {
                try {
                    $('#' + dvName).html(data);
                } catch (ex) { }
            }
        }
    })
    return false;
}
//
function cancelQuickAdd() {
    $("#CancelQuickAdd").click();
}
function Associate(e, entityName) {
    var id = $(e.target).attr("id");
    var moveRight = "MoveRight" + entityName;
    var idAvailable = "#" + entityName + "IDAvailable";
    var idSelected = "#" + entityName + "IDSelected";
    var selectFrom = id == moveRight ? idAvailable : idSelected;
    var moveTo = id == moveRight ? idSelected : idAvailable;
    var selectedItems = $(selectFrom + " :selected").toArray();
    $(moveTo).append(selectedItems);
    selectedItems.remove;
    $(idSelected + " option").prop("selected", "selected");
}
function SearchList(id, val, selectCtrl) {
    if (val == "") {
        if (navigator.userAgent.indexOf('Trident') != -1 || navigator.userAgent.indexOf('MSIE') != -1) {
            $("#" + selectCtrl + " option").each(function () {
                if (this.nodeName.toUpperCase() === 'OPTION') {
                    var span = $(this).parent();
                    var opt = this;
                    if ($("#" + selectCtrl + " option[value='" + this.value + "']").parent().is('span')) {
                        $(opt).show();
                        $(span).replaceWith(opt);
                    }
                }
            });
        } else {
            $('#' + selectCtrl).find("option").show();
        }
    } else {
        $("#" + selectCtrl + " option").each(function () {
            if (!(this.text.toLowerCase().match(val.toLowerCase()))) {
                if ($("#" + selectCtrl + " option[value='" + this.value + "']").is('option') && (!$("#" + selectCtrl + " option[value='" + this.value + "']").parent().is('span')))
                    $(this).wrap((navigator.userAgent.indexOf('Trident') != -1 || navigator.userAgent.indexOf('MSIE') != -1) ? '<span>' : null).hide();
            }
            else {
                if (navigator.userAgent.indexOf('Trident') != -1 || navigator.userAgent.indexOf('MSIE') != -1) {
                    if (this.nodeName.toUpperCase() === 'OPTION') {
                        var span = $(this).parent();
                        var opt = this;
                        if ($("#" + selectCtrl + " option[value='" + this.value + "']").parent().is('span')) {
                            $(opt).show();
                            $(span).replaceWith(opt);
                        }
                    }
                } else {
                    $("#" + selectCtrl + " option[value='" + this.value + "']").show(); //all other browsers use standard .show()
                }
            }
        });
    }
}
function SearchListNew(obj, searchstring, hostingentityname, hostingentityid, associatedtype, selectCtrl) {
    var srchstring = $(obj).val();
    var url1 = ($(obj).attr("dataurl"));
    url1 = addParameterToURL(url1, "SearchString", searchstring);
    url1 = addParameterToURL(url1, "HostingEntityName", hostingentityname);
    url1 = addParameterToURL(url1, "HostingEntityID", hostingentityid);
    url1 = addParameterToURL(url1, "AssociatedType", associatedtype);
    $.ajax({
        async: false,
        type: "GET",
        url: url1,
        success: function (data) {
            searchstr = searchstring;
            $('#' + selectCtrl + ' > option[val!=""]').remove();
            $.each(data.data, function (index, value) {
                $('#' + selectCtrl).append('<option value="' + value.Value + '">' + value.Text + '</option>');
            });
            if (data.Results)
                $('#lbl' + selectCtrl).css('display', 'block');
            else
                $('#lbl' + selectCtrl).css('display', 'none');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //alert(errorThrown + '-' + textStatus);
        }
    });
}
function SearchListNew1(obj, dataurl, searchstring, hostingentityname, hostingentityid, associatedtype, selectCtrl, Asso) {
    var srchstring = $(obj).val();
    var url1 = dataurl;
    url1 = addParameterToURL(url1, "SearchString", searchstring);
    url1 = addParameterToURL(url1, "HostingEntityName", hostingentityname);
    url1 = addParameterToURL(url1, "HostingEntityID", hostingentityid);
    url1 = addParameterToURL(url1, "AssociatedType", associatedtype);
    var association = Asso.split(",");
    var firstparam = 0;
    for (i = 0; i < association.length; i++) {
        var vals = "";
        ele = document.getElementById(association[i]);
        if (ele != null)
            for (var o = 0; o < ele.options.length; o++) {
                if (ele.options[o].selected)
                    vals += ele.options[o].value + ",";
            }
        vals = vals.replace(/^,|,$/g, '');
        if (vals.length > 0) {
            //url1 += "&" + association[i] + "=" + vals;
            url1 = addParameterToURL(url1, association[i], vals);
        }
    }


    $.ajax({
        async: false,
        type: "GET",
        url: url1,
        success: function (data) {
            searchstr = searchstring;
            $('#' + selectCtrl + ' > option[val!=""]').remove();
            $.each(data.data, function (index, value) {
                $('#' + selectCtrl).append('<option value="' + value.Value + '">' + value.Text + '</option>');
            });
            if (data.Results)
                $('#lbl' + selectCtrl).css('display', 'block');
            else
                $('#lbl' + selectCtrl).css('display', 'none');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //alert(errorThrown + '-' + textStatus);
        }
    });

}
function check1MThresholdValueQuickAdd(e, entityName) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    url = url.replace("CreateQuick", "Check1MThresholdValue");
    $("#errors").html("");
    $("#errorsMsg").html("");
    var flag = true;
	flag = Check1MThresholdLimit(e, fd, url, entityName, "ErrMsgQuickAdd");
    return flag;
}
function check1MThresholdValue(e, entityName) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    url = url.replace("CreateQuick", "Check1MThresholdValue");
    $("#errors").html("");
    $("#errorsMsg").html("");
    var flag = true;
    flag = Check1MThresholdLimit(e, fd, url, entityName, "ErrMsg");
    return flag;
}
// Radio Button List
function ClearChildRBDD(child, obj) {
    var ctrlId = $(obj).attr("id");
    var ctrlName = $(obj).attr("name").substr(2, $(obj).attr("name").length - 1);
    var childCtrlId = child.split(',');
    var firstChildCtrlId = "";
    var firstChildCtrlName = "";
    var isRequired = "";
    $.each(childCtrlId, function (i, ctrl) {
        if (i == 0) {
            firstChildCtrlId = ctrl.replace('.', '_');
            firstChildCtrlName = ctrl;
        }
        ctrl = ctrl.replace('.', '_');
        if ($('#ul' + ctrl).find('li').length > 0) {
            $('#ul' + ctrl).html("");
        }
    });
    if (firstChildCtrlId != "") {
        var selectedval = $("input:radio[name='" + ctrlName + "']:checked").val();
        isRequired = $("#ul" + firstChildCtrlId).attr("required") == "required";
        var finalUrl = $("#ul" + firstChildCtrlId).attr("dataurl");
        var AssociationName = $("#ul" + firstChildCtrlId).attr("AssoNameWithParent");
        if (AssociationName.length > 0)
            finalUrl = addParameterToURL(finalUrl, 'AssoNameWithParent', AssociationName);
        if (selectedval.length > 0)
            finalUrl = addParameterToURL(finalUrl, 'AssociationID', selectedval);
    }
    $.ajax({
        type: "GET",
        url: finalUrl,
        contentType: "application/json; charset=utf-8",
        global: false,
        async: false,
        cache: false,
        dataType: "json",
        success: function (jsonData) {
            var listItems = "";
            var tagrequired = isRequired ? "required='required'" : "";
            $.each(jsonData, function (i, ctrlValue) {
                listItems += "<li style='list-style-type:none;'><label><input name='" + firstChildCtrlName + "' " + tagrequired + " type='radio' value='" + ctrlValue.Id + "'>";
                listItems += " <span>" + ctrlValue.Name + "</span>";
                listItems += "</label></li>";
            });
            if (listItems == "") {
                listItems += "<input name='" + firstChildCtrlName + "' " + tagrequired + " style='width:0px; height:0px; border:0px solid #fff !important;' type='text'>";
            }
            $("#ul" + firstChildCtrlId).html(listItems);
        }
    });
}
//
// Inline Add Edit Cancel
function quickEdit(e, entityname, url, form) {
    var thelink = url;
    $.ajax({
        async: false,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: url,
        success: function (data) {
            var responcedata = data;
            var trEdit = $("#Des_Table").find('table').find('tr:eq(1)');
            $(trEdit).find('td').each(function (index) {
                gettagType($(this), responcedata);
            });
            var formacn = $("#" + form).attr("action").substr(0, $("#" + form).attr("action").lastIndexOf("/") + 1);
            $("#" + form).attr("action", formacn + "EditQuick");
            $("#" + form).append("<input type='hidden' name='Id' value='" + responcedata["Id"] + "' />")
            $("#" + form).append("<input type='hidden' name='ConcurrencyKey' value='" + responcedata["ConcurrencyKeyBase64"] + "' />");
            $("#quickAdd" + entityname).hide();
            $("#quickEdit" + entityname).show();
            $("#quickCancel" + entityname).show();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown + '-' + textStatus);
        }
    });
}
function gettagType(tdEdit, responcedata) {
    if ($(tdEdit).html() != "") {
        var tag = $(tdEdit).find('input:text, input:file, input:radio, input:checkbox, select, textarea');
        if (tag.length > 0) {
            var tagType = $(tag)[0].type;
            var control = $($(tag)[0]).prop('name');
            valEdit = responcedata[control];
            switch (tagType) {
                case "text":
                    $("#" + control).val(valEdit);
                    break;
                case "select-one":
                    $("#" + control).val(valEdit);
                    $("#" + control).trigger('chosen:updated');
                    break;
                case "radio":
                    $('input[name=' + control + '][value=' + valEdit + ']').attr('checked', 'checked');
                    break;
            }
        }
    }
}
function QuickAddFromGrid(e, isrefresh, Entity, host) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    if (!form.valid()) return;
    try {
        fd = new FormData(form[0]);
        $('input[type="file"]').each(function () {
            var file = $('#' + $(this)[0].id)[0].files;
            if (file.length) {
                fd.append($(this)[0].id, file[0]);
            }
        });
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                if (result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                        }
                        else
                            $('#' + Entity + 'SearchCancel').click();
                    }
                } else {
                    $('#dvPopupError').html(result);
                }
            }
        });
    } catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            success: function (result) {
                if (result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                        }
                        else
                            $('#' + Entity + 'SearchCancel').click();
                    }
                } else {
                    $('#dvPopupError').html(result);
                }
            }
        });
    }
}
function QuickEditFromGrid(e, isrefresh, Entity, host, IsWf, bisrule, biscount, ruleType, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname) {
    if (IsWf) {
        $('input:hidden[name="hdncommand"]').val(e.currentTarget.value);
    }
    $("textarea.richtext").each(function () {
        $(this).val($(this).code());
    });
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    var url = $(target).closest('form').attr("action");
    var p = BusineesRule(fd, url, bisrule, biscount, form, ruleType, Entity, lblerrormsg, isinline, lstinlineassocname, lstinlineassocdispname, lstinlineentityname);
    if (!p) {
        $(target).removeAttr("disabled");
        return false;
    }
    if (!form.valid()) { $(target).removeAttr("disabled"); return; }
    SaveServerTimeQuickEdit(form);
    try {
        fd = new FormData(form[0]);
        $('input[type="file"]').each(function () {
            var file = $('#' + $(this)[0].id)[0].files;
            if (file.length) {
                fd.append($(this)[0].id, file[0]);
            }
        });
        url = addParameterToURL(url, "IsAddPop", true);
        url = addParameterToURL(url, "RenderPartial", true);

        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {

                if (result.Result.trim().toLowerCase() == "succeed") {
                    form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                        }
                        else
                            $('#' + Entity).load(result.UrlRefr);
                    }
                } else {
                    $('#dvPopupError').html(result.UrlRefr);
                    //form.find('button[aria-hidden="true"]').click();
                    //if (isrefresh) {
                    //    CancelSearch(Entity, url, host);
                    //}
                }
            }
        });
    } catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url + "?IsAddPop=" + true,
            type: "POST",
            cache: false,
            data: fd,
            success: function (result) {
                if (result == "FROMPOPUP") {
                    form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        if (host != undefined && host.length > 0 && $('#' + host).length > 0) {
                            $('#' + Entity + 'SearchCancel', $('#' + host)).click();
                            $('#dvcnt_' + host).load(location.href + " #dvcnt_" + host);
                        }
                        else
                            $('#' + Entity + 'SearchCancel').click();
                    }
                } else {
                    form.find('button[aria-hidden="true"]').click();
                    if (isrefresh) {
                        $('#' + Entity + 'SearchCancel').click();
                    }

                }
            }
        });
    }
    //alert(Entity);
    if (Entity.indexOf('Events') > -1 || Entity == 'T_Schedule') {
        location.reload();
    }
}
function QuickCancelFromGrid(e, isrefresh, Entity, host) {
    $('#' + Entity + 'SearchCancel').click();
}
// End
//Default Entity Page Js...
function SelectPageType(selectedval, SelectUrl, DirectUrl, DefaultUrl, Favorites) {
    DirectUrl.removeProp("required");
    SelectUrl.hide();
    DefaultUrl.hide();
    Favorites.hide();
    DirectUrl.hide();
    DefaultUrl.removeAttr("required");
    Favorites.removeAttr("required");
    DirectUrl.removeAttr("required");
    if (selectedval == "Default") {
        DefaultUrl.show();
        SelectUrl.show();
        DefaultUrl.attr("required", "required");
    }
    else if (selectedval == "Favorite") {
        Favorites.show();
        SelectUrl.show();
        Favorites.attr("required", "required");
    } else if (selectedval == "Url") {
        DirectUrl.show();
        SelectUrl.show();
        DirectUrl.attr("required", "required");
    }
}
function SaveDefaultPage(e, selectedval, DefaultUrl, PageUrl, Other, Favorites, DirectUrl, EntityName, RoleList, Roles) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    var form = $(target).closest('form');
    if (!form.valid()) return;
    if (RoleList.val() != null) {
        Roles.val(RoleList.val());
        if (selectedval == "Default") {
            if (DefaultUrl.val() == undefined || DefaultUrl.val().length == 0)
            { alert("Default Value Required.."); return false; }
            var defaultselected = DefaultUrl.val();
            var pageurl = '/_ControllerName/_ActionName';
            PageUrl.val(pageurl.replace("_ActionName", defaultselected).replace("_ControllerName", EntityName.val()));
            Other.val($("option:selected", DefaultUrl).text());
        }
        else if (selectedval == "Favorite") {
            if (Favorites.val() == undefined || Favorites.val().length == 0)
            { alert("Favorites Value Required.."); return false; }
            PageUrl.val(Favorites.val());
            Other.val($("option:selected", Favorites).text());
        } else if (selectedval == "Url") {
            if (DirectUrl.val() == undefined || DirectUrl.val().length == 0)
            { alert("Url Value Required.."); return false; }
            PageUrl.val(DirectUrl.val());
            Other.val("Link");
        }
        form.submit();
    }
    else {
        alert("Roles Value Required..");
        return false;
    }
}
//
function GetCalculationValue(e, url) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
   // e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    // var url = $(target).closest('form').attr("action");
    try {
        fd = new FormData(form[0]);
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            async:false,
            success: function (result) {               
                $.each(result, function (key, value) {
                    $("#" + key).val(value);
                })
            }
        });
    } catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                $.each(result, function (key, value) {
                    $("#" + key).val(value);
                })
            }
        });
    }
}
function FillDerivedDetailsInline(obj, e, suffix, host) {
    var url = $(obj).attr("derivedurl");
    var hostvalue = $(obj).val();
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    //if popup
    if (e.target) target = e.target;
    //
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    url = addParameterToURL(url, "host", host);
    url = addParameterToURL(url, "value", hostvalue);
    try {
        fd = new FormData(form[0]);
        //fd $("#dvT_mainlineitemassociationID").find("select, textarea, input").serialize();
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                $.each(result, function (key, value) {
                    if (suffix != undefined && suffix.length > 0) {

                        key = suffix + "_" + key;

                    }
                    if ($("#" + key).is('select')) {
                        $("#" + key).trigger("chosen:open");
                        $("#" + key).val(value);
                        $("#" + key).trigger("chosen:updated");
                        $("#" + key).trigger("click.chosen");
                        $("#" + key).trigger("change");
                    }
                    else $("#" + key).val(value);
                })
            }
        });
    } catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                $.each(result, function (key, value) {
                    if ($("#" + key).is('select')) {
                        $("#" + key).trigger("chosen:open");
                        $("#" + key).val(value);
                        $("#" + key).trigger("chosen:updated");
                        $("#" + key).trigger("click.chosen");
                        $("#" + key).trigger("change");
                    }
                    else $("#" + key).val(value);
                })
            }
        });
    }
}
function FillDerivedDetails(obj, e) {
    var url = $(obj).attr("derivedurl");
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    //if popup
    if (e.target) target = e.target;
    //
    e.preventDefault();
    var form = $(target).closest('form');
    var fd = $(target).closest('form').serialize();
    try {
        fd = new FormData(form[0]);
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                $.each(result, function (key, value) {
                    if ($("#" + key).is('select')) {
                        $("#" + key).trigger("chosen:open");
                        $("#" + key).val(value);
                        $("#" + key).trigger("chosen:updated");
                        $("#" + key).trigger("click.chosen");
                        $("#" + key).trigger("change");
                    }
                    else $("#" + key).val(value);
                })
            }
        });
    } catch (ex) {
        fd = $(target).closest('form').serialize();
        $.ajax({
            url: url,
            type: "POST",
            cache: false,
            data: fd,
            dataType: "json",
            processData: false,
            contentType: false,
            success: function (result) {
                $.each(result, function (key, value) {
                    if ($("#" + key).is('select')) {
                        $("#" + key).trigger("chosen:open");
                        $("#" + key).val(value);
                        $("#" + key).trigger("chosen:updated");
                        $("#" + key).trigger("click.chosen");
                        $("#" + key).trigger("change");
                    }
                    else $("#" + key).val(value);
                })
            }
        });
    }
}
function ListBoxPagination(obj, searchstring, hostingentityname, hostingentityid, associatedtype, selectCtrl) {
    var url1 = ($(obj).attr("dataurl"));
    size = $(obj).val();
    url1 = addParameterToURL(url1, "SearchString", searchstring);
    url1 = addParameterToURL(url1, "HostingEntityName", hostingentityname);
    url1 = addParameterToURL(url1, "HostingEntityID", hostingentityid);
    url1 = addParameterToURL(url1, "AssociatedType", associatedtype);
    url1 = addParameterToURL(url1, "Size", size);
    $.ajax({
        async: false,
        type: "GET",
        url: url1,
        success: function (data) {
            searchstr = searchstring;
            $('#' + selectCtrl + ' > option[val!=""]').remove();
            $.each(data.data, function (index, value) {
                $('#' + selectCtrl).append('<option value="' + value.Value + '">' + value.Text + '</option>');
            });
            if (data.Results)
                $('#lbl' + selectCtrl).css('display', 'block');
            else
                $('#lbl' + selectCtrl).css('display', 'none');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //alert(errorThrown + '-' + textStatus);
        }
    });
}

// Filter Tab Cookies
function ClickFilterBtn() {
    if ($.cookie('fltCookie') != null) {
        var _fltId = $.cookie('fltCookie');
        SetCookieFlt("0");
        $("#" + _fltId + "").click();
    }
}
function ClickFilterTabBtn() {
    if ($.cookie('fltCookieFltTabId') != null) {
        var _fltTabId = $.cookie('fltCookieFltTabId');
        SetCookieFltTab("0");
        $("#" + _fltTabId).click();
    }
}
function SetCookieFlt(fltId) {
    if (fltId != "0") {
        $.cookie("fltCookie", fltId);
    }
    else {
        $.cookie("fltCookie", null)
    }
}
function ClearFilterCookies() {
    $.cookie("fltCookie", null);
    $.cookie("fltCookieFltTabId", null);
    var cookies = document.cookie.split(";");
    for (var i = 0; i < cookies.length; i++) {
        var equals = cookies[i].indexOf("=");
        var name = equals > -1 ? cookies[i].substr(0, equals) : cookies[i];
        document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
    }
}
function SetCookieFltTab(FltTabId) {
    if (FltTabId != "0") {
        $.cookie("fltCookieFltTabId", FltTabId);
    }
    else {
        $.cookie("fltCookieFltTabId", null)
    }
}
//
function togglesidebar(e, obj,user) {
   e.preventDefault();
    $("#wrapper").toggleClass("toggled-2");
    $.cookie("sidebartoggle" + user, $("#wrapper").attr("class"));
    $('#menu ul').hide();
}
function LoadReports(rptId, LoadReportsDiv, Entity, Asso, Prop, sortby, isAsc, currentFilter, page) {

    var urlstring = SetReportUrl(rptId, LoadReportsDiv, Entity, Asso, Prop, sortby, isAsc, currentFilter, page);
    $("#ShowReoprts").modal('show');
    $("#ShowReoprts").find('.modal-dialog.ui-draggable').attr("style", "width:90%");
    $("#" + LoadReportsDiv).html('Please wait..');
    $("#" + LoadReportsDiv).load(urlstring);
}
function SetReportUrl(rptId, LoadReportsDiv, Entity, Asso, Prop, sortby, isAsc, currentFilter, page) {
    var urlstring = $("#" + "fSearch" + Entity).attr("dataurl");
    var association = Asso.split(",");
    var property = Prop.split(",");
    var firstparam = 0;
    for (i = 0; i < association.length; i++) {
        var vals = "";
        ele = document.getElementById(association[i]);
        if (ele != null)
            for (var o = 0; o < ele.options.length; o++) {
                if (ele.options[o].selected)
                    vals += ele.options[o].value + ",";
            }
        if (vals.length > 0) {
            if (firstparam == 0)
                urlstring += "?" + association[i] + "=" + vals;
            else
                urlstring += "&" + association[i] + "=" + vals;
            firstparam = 1;
        }
    }
    for (i = 0; i < property.length; i++) {
        ele = document.getElementById(property[i]);
        if (ele != null)
            if (ele.value.length > 0) {
                if (firstparam == 0)
                    urlstring += "?" + property[i] + "=" + ele.value;
                else
                    urlstring += "&" + property[i] + "=" + ele.value;
                firstparam = 1;
            }
    }
    var page_sortstring = "";
    if (sortby != "") {
        page_sortstring = "&sortBy=" + sortby;
        if (isAsc != "")
            page_sortstring += "&isAsc=" + isAsc;
    }

    if (currentFilter != '') {
        if (firstparam == 0)
            urlstring += "?searchString=" + currentFilter + page_sortstring;
        else
            urlstring += "&searchString=" + currentFilter + page_sortstring;
    }
    if (firstparam == 0)
        urlstring += "?search=" + SanitizeURLString(document.getElementById("FSearch").value) + page_sortstring;
    else
        urlstring += "&search=" + SanitizeURLString(document.getElementById("FSearch").value) + page_sortstring;
    urlstring = addParameterToURL(urlstring, "SortOrder", $("#SortOrder").val());
    urlstring = addParameterToURL(urlstring, "GroupByColumn", $("#hdnGroupByColumn").val());
    urlstring = addParameterToURL(urlstring, "HideColumns", $("#HideColumns").val());
    urlstring = addParameterToURL(urlstring, "IsReports", true);
    urlstring = addParameterToURL(urlstring, "FilterCondition", $("#FilterCondition").val());
    if (rptId != "")
        urlstring = addParameterToURL(urlstring, "rptId", rptId);
    if ($("#ViewReport") != undefined)
        $("#ViewReport").val(urlstring);
    return urlstring;
}