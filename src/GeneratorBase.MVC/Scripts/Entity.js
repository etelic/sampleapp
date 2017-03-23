function DoubleClickRow(RecordID, Url) {
    var url = Url;//"@Url.Action("Details", "Order", new { id = "_Id", UrlReferrer = Request.Url, AssociatedType=ViewData["AssociatedType"], HostingEntityName = @Convert.ToString(ViewData["HostingEntity"]), HostingEntityID = @Convert.ToString(ViewData["HostingEntityID"]) }, null)".replace("_Id", RecordID);
    window.location.replace(url);
}
function OpenNotes(url, fieldName, ctrl, ev) {
    if (ev.which == 3) {
        var Url = encodeURI(url.replace("_FieldName", fieldName).replace("_UIControlName", ctrl));
        OpenPopUpEntity('addPopup', 'Feedback', 'dvPopup', Url);
    }
}
function pagesizelistChange(e, EntityName, UserName) {
    //remove pagination cookies 
    if ($.cookie("pagination" + UserName + EntityName) != null)
        $.removeCookie("pagination" + UserName + EntityName);
    //
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = $(target).attr("Url");
    var pagesizeCookie = "pageSize" + UserName + EntityName;
    if ($.cookie(pagesizeCookie) != null) {
        $.removeCookie(pagesizeCookie);
    }
    var pageSizeValue = $(target).val();
    if (pageSizeValue > 10)
        $.cookie(pagesizeCookie, pageSizeValue);
    if ($.cookie(pagesizeCookie) != null)
        pageSizeValue = $.cookie(pagesizeCookie)
    $.ajax({
        url: thelink,
        cache: false,
        data: { searchString: $(target).closest('#SearchString' + EntityName).val(), itemsPerPage: pageSizeValue },
        success: function (data) {
            if (data != null) {
                try {
                    $(target).closest("#" + EntityName).html(data);
                } catch (ex) { }
            }
        }
    })
    return false;
}
function SortLinkClick(e, EntityName) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = e.target.href;
    eval("query = {" + thelink.split("?")[1].replace(/&/ig, "\",").replace(/=/ig, ":\"") + "\"};");
    e.preventDefault();
    e.stopPropagation();
    $.ajax({
        url: thelink,
        cache: false,
        data: { itemsPerPage: $("#pagesizelist" + EntityName, $(target).closest("#" + EntityName)).val() },
        success: function (data) {
            if (data != null) {
                try {
                    $(target).closest("#" + EntityName).html(data);
                } catch (ex) { }
                thelink = "";
            }
        }
    })
    return false;
}
function SearchClick(e, EntityName, Url, UserName) {
    //remove pagination cookies
    if ($.cookie("pagination" + UserName + EntityName) != null)
        $.removeCookie("pagination" + UserName + EntityName);
    //
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = Url;
    thelink = thelink.replace("_SearchString", $("#SearchString" + EntityName, $(target).closest("#" + EntityName)).val());
    $.ajax({
        url: thelink,
        cache: false,
        success: function (data) {
            if (data != null) {
                try {
                    $(target).closest("#" + EntityName).html(data);
                } catch (ex) { }
            }
        }
    })
    return false;
}
function PaginationClick(e, EntityName, UserName) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = e.target.href;
    if (thelink != '') {
        var queryStr = eval("query = {" + thelink.split("?")[1].replace(/&/ig, "\",").replace(/=/ig, ":\"") + "\"};");
        paginationcookies(e, EntityName, UserName, queryStr.page)
        e.preventDefault();
        e.stopPropagation();
        $.ajax({
            url: thelink,
            cache: false,
            data: { itemsPerPage: $("#pagesizelist" + EntityName, $(target).closest("#" + EntityName)).val() },
            success: function (data) {
                if (data != null) {
                    try {
                        $(target).closest("#" + EntityName).html(data);
                    } catch (ex) { }
                }
            }
        })
    }
    return false;
}
function paginationcookies(e, EntityName, UserName, page) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = $(target).attr("Url");
    var paginationCookie = "pagination" + UserName + EntityName;
    if ($.cookie(paginationCookie) != null) {
        $.removeCookie(paginationCookie);
    }
    var paginationValue = page;
    $.cookie(paginationCookie, paginationValue);
    if ($.cookie(paginationCookie) != null)
        paginationValue = $.cookie(paginationCookie)
}
function showhideColumns(e, EntityName) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var div = $("#ColumnShowHide" + EntityName, $(target).closest("#" + EntityName));
    var lbl = $("#lbl" + EntityName, $(target).closest("#" + EntityName));
    if (div.hasClass('collapse')) {
        div.toggleClass('in');
        lbl.css('display', 'none');
    }
    else {
        div.toggleClass('collapse');
        lbl.css('display', 'block');
    }
}
function showhideSaveCookies(e, EntityName, UserName, HostingEntity) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var myCookie = UserName + EntityName + HostingEntity;
    if ($.cookie(myCookie) != null) {
        $.removeCookie(myCookie);
    }
    var selected = [];
    $('#ColumnShowHide' + EntityName + ' input[type=checkbox]', $(target).closest("#" + EntityName)).each(function () {
        if ($(this).prop('checked') == false) {
            selected.push($(this).attr('name'));
        }
    });
    if (selected != "") {
        $.cookie(myCookie, selected);
        $('#lbl' + EntityName, $(target).closest("#" + EntityName)).css('display', 'block');
    }
}
function ColumnClick(e, EntityName) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var index = $(target).attr('name').substr(3);
    //index--;
    $('table tr', $(target).closest("#" + EntityName)).each(function () {
        $('td:eq(' + index + ')', this).toggle();
    });
    $('th.' + $(target).attr('name'), $(target).closest("#" + EntityName)).toggle();
}
function FillDropdownMobile(hostingentity) {
    //hostingentity = hostingentity.id;
    var selectedval = $("option:selected", $("#" + hostingentity)).val();
    var finalUrl = $("#" + hostingentity).attr("dataurl");
    var urlGetAll = $("#" + hostingentity).attr("dataurl").replace("GetAllValueMobile", "Index");
    urlGetAll = addParameterToURL(urlGetAll, 'BulkOperation', "single");
    var parentDDid = $("#" + hostingentity).attr("ParentDD");
    var hostingname = $("#" + hostingentity).attr("hostingname");
    var AssociationNames = $("#" + hostingentity).attr("AssoNameWithParent");
    var associationParam = "";
    if (parentDDid != null || parentDDid != undefined) {
        var Parents = parentDDid.split(",");
        var AssociationNameWithParent = "";
        var selectedParentVal = "";
        var parent = "";
        for (var i = 0; i < Parents.length; i++) {
            if ($("option:selected", $("#" + Parents[i])).val().length > 0) {
                AssociationNameWithParent = AssociationNames.split(",")[i];
                selectedParentVal = $("option:selected", $("#" + Parents[i])).val();
                parent = Parents[i];
            }
        }
        var IsReverse = $("#" + hostingentity).attr("IsReverse");
        if (IsReverse == "true" || IsReverse == "True") {
            if (selectedParentVal.length > 0) {
                var parent1 = $("#" + parent).attr("HostingName");
                var parentUrl = $("#" + parent).attr("dataurl").replace("GetAllValueMobile", "GetPropertyValueByEntityId");
                parentUrl = addParameterToURL(parentUrl, "Id", selectedParentVal);
                parentUrl = addParameterToURL(parentUrl, "PropName", AssociationNameWithParent);
                $.ajax({
                    type: "GET",
                    async: false,
                    url: parentUrl,
                    success: function (jsonObj) {
                        if (selectedParentVal.length > 0)
                            finalUrl = addParameterToURL(finalUrl, 'AssociationID', jsonObj);
                        if (AssociationNameWithParent.length > 0)
                            finalUrl = addParameterToURL(finalUrl, 'AssoNameWithParent', "Id");
                    }
                });
            }
        }
        else {
            if (AssociationNameWithParent.length > 0)
                finalUrl = addParameterToURL(finalUrl, 'AssoNameWithParent', AssociationNameWithParent);
            if (selectedParentVal.length > 0)
                finalUrl = addParameterToURL(finalUrl, 'AssociationID', selectedParentVal);
        }
    }
    if (parentDDid != null || parentDDid != undefined) {
        var Parents = parentDDid.split(",");
        var AssociationNameWithParent = "";
        var selectedParentVal = "";
        var hostingnameofparent = "";
        var parentdd = "";
        for (var i = 0; i < Parents.length; i++) {
            if ($("option:selected", $("#" + Parents[i])).val().length > 0) {
                AssociationNameWithParent = AssociationNames.split(",")[i];
                selectedParentVal = $("option:selected", $("#" + Parents[i])).val()
                parentdd = Parents[i];
            }
        }
        if (parentdd.length > 0)
            urlGetAll = addParameterToURL(urlGetAll, 'HostingEntity', $("#" + parentdd).attr("hostingname"));
        if (AssociationNameWithParent.length > 0)
            urlGetAll = addParameterToURL(urlGetAll, 'AssociatedType', AssociationNameWithParent.substring(0, AssociationNameWithParent.length - 2));
        if (selectedParentVal.length > 0)
            urlGetAll = addParameterToURL(urlGetAll, 'HostingEntityID', selectedParentVal);
    }
    var dispName = ($("label[for=\"" + hostingentity + "\"]").text());
    var bulkurl = "value=\"'PopupBulkOperation','" + hostingentity + "','" + dispName + "','dvPopupBulkOperation','" + urlGetAll + "'\"";
    $.ajax({
        type: "GET",
        url: finalUrl,
        contentType: "application/json; charset=utf-8",
        global: false,
        async: false,
        cache: false,
        dataType: "json",
        success: function (jsonObj) {
            var listItems = "";
            $("#" + hostingentity).empty();
            if (selectedval != '')
                listItems += "<option value=''>--None--</option>";
            else
                listItems += "<option selected='selected' value=''>--None--</option>";
            for (i in jsonObj) {
                if (jsonObj[i].Id != undefined && jsonObj[i].Name != undefined) {
                    if (selectedval == jsonObj[i].Id)
                        listItems += "<option selected='selected' value='" + jsonObj[i].Id + "'>" + jsonObj[i].Name + "</option>";
                    else
                        listItems += "<option value='" + jsonObj[i].Id + "'>" + jsonObj[i].Name + "</option>";
                }
            }
            if (jsonObj.length >= 10)
                listItems += "<option " + bulkurl + ">ViewAll</option>"
            $("#" + hostingentity).html(listItems);
        }
    });
}
function openbulk(ele) {
    var value = $(ele).val();
    var selectedtext = $(ele).find('option:selected').text();
    if (selectedtext.toLowerCase() == "viewall") {
        var split = value.split(",");
        var arg1 = split[0].replace(/'/g, '');
        var arg2 = split[1].replace(/'/g, '');
        var arg3 = split[2].replace(/'/g, '');
        var arg4 = split[3].replace(/'/g, '');
        var arg5 = split[4].replace(/'/g, '');
        OpenPopUpBulkOperation(arg1, arg2, arg3, arg4, arg5);
        $("#" + ele.id + " option:contains(--None--)").prop({ selected: true });
    }
}
$(document).ready(function () {
    $(".tip-top").tooltip({
        placement: 'top'
    });
    $(".tip-right").tooltip({
        placement: 'right'
    });
    $(".tip-bottom").tooltip({
        placement: 'bottom'
    });
    $(".tip-left").tooltip({
        placement: 'left'
    });
});
//open image for mobile......
function OpenPopUpImage(e, pop, pic) {
    e.preventDefault();
    var maxHeight = $(window).height() + "px";
    $("#" + pop).css("max-height", maxHeight);
    $("#" + pop + " img").css("max-height", maxHeight);
    $("#" + pic).modal('show');
    var maxWidth = $("#" + pop).width() + "px";
    $("#" + pop + " img").css("width", maxWidth);
}
function OpenPopUpImageByte(e, pop, pic, crlimg, docid) {
    e.preventDefault();
    var loc = window.location;
    var pathName = loc.pathname.substring(0, loc.pathname.lastIndexOf('/') + 1);
    $("#" + crlimg).attr("src", pathName + "Document/DisplayImageAfterClick/" + docid);
    var maxHeight = $(window).height() + "px";
    $("#" + pop).css("max-height", maxHeight);
    $("#" + pop + " img").css("max-height", maxHeight);
    $("#" + pic).modal('show');
    var maxWidth = $("#" + pop).width() + "px";
    $("#" + pop + " img").css("width", maxWidth);
}
function ClosePopUpImage(e, pic) {
    e.preventDefault();
    $("#" + pic).modal('hide');
}
function hideShowmore(e) {
    e.preventDefault();
}
function SelectAllRows(obj) {
    var checked = false;
    var table = $(obj).closest("#Des_Table");
    if ($(obj).is(":checked"))
        checked = true;
    else {
        $("#SelectedItems", table).val('');
    }
    $('input[type=checkbox]', table.find("tr").find("td:first")).each(function () {
        if (this != obj && !($(this).is(':disabled'))) {
            $(this).prop("checked", checked);
            SelectForBulkOperation(this, $(this).attr("id"));
        }
    });
}
function SelectForBulkOperation(source, id) {
    var table = $(source).closest("#Des_Table");
    var selectedobj = $("#SelectedItems", table);
    var value = selectedobj.val();
    if (value.length < 1 || value == undefined)
        value = ",";
    if ($(source).is(":checked"))
        value += id + ",";
    else
        value = value.replace("," + id + ",", ",");
    selectedobj.val(value);
}
function PerformBulkOperation(obj, entity, action, url1) {
    var val = $("#SelectedItems").val().substr(1).split(",");
    var r = confirm("Do you want to really execute " + action + "!");
    if (r == true) {
        $.ajax({
            type: "POST",
            data: { ids: val },
            url: url1,
            success: function (msg) {
                $("#" + entity + "SearchCancel").click();
            }
        });
    }
    else {
        return true;
    }
}
function OpenPopUpBulKUpdate(Popup, EntityName, dvName, url) {
    $("#" + Popup + 'Label').html();
    if ((EntityName.indexOf("Update") == -1) && (EntityName.indexOf("Delete") == -1)) {
        $("#" + Popup + 'Label').html("Add " + EntityName);
    }
    else {
        $("#" + Popup + 'Label').html(EntityName);
    }
    var val = $("#SelectedItems").val().substr(1).split(",");
    url = url + "&ids=" + val;
    $("#" + Popup).modal('show');
    $("#" + dvName).html('');
    $("#" + dvName + "Error").html('');
    $("#" + dvName).load(url);
}
function ExcuteSingleVerb(EntityName, obj) {
    var url1 = $(obj).attr("dataurl");
    $.ajax({
        url: url1,
        cache: false,
        success: function (result) {
	   if (result.isRedirect) {
                window.location.href = result.redirectUrl;
                return false;
            }
            if (result == "Success") {
                alert('Action executed sucessfully.');
                location.reload();
            } else {
                alert('Some error in executing action!')
            }
        }
    });
}



 	