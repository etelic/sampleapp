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
function FillFromTimeSlot(obj,e)
{
        var object = $(obj);
        var value = object.find('option:selected').text();
        var vals = value.split("-")
        $("[valuetype='T_StartTime']").val(vals[0]);
        $("[valuetype='T_EndTime']").val(vals[1]);

        $("[valuetype='T_StartTime']").data("DateTimePicker").date(vals[0]);
        $("[valuetype='T_EndTime']").data("DateTimePicker").date(vals[1]);

        $("[valuetype='T_StartTime']").attr("readonly", "readonly");
        $("[valuetype='T_EndTime']").attr("readonly", "readonly");
        e.preventDefault();
        e.stopPropagation();
        return false;
    }
function pagesizelistChange(e, EntityName, UserName,IsReports,fromview,rptId) {
    //remove pagination cookies 
    if ($.cookie("pagination" + UserName + EntityName) != null)
        $.removeCookie("pagination" + UserName + EntityName);
    //
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = $(target).attr("Url");
    if (IsReports)
        thelink = addParameterToURL(thelink, "IsReports", IsReports)
    if (fromview)
        thelink = addParameterToURL(thelink, "FromViewReport", fromview)
    if (rptId)
        thelink = addParameterToURL(thelink, "rptId", rptId)

    
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
function SortLinkClick(e, EntityName, IsReports, fromview, rptId) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = e.target.href;
    if (IsReports)
        thelink = addParameterToURL(thelink, "IsReports", IsReports)
    if (fromview)
        thelink = addParameterToURL(thelink, "FromViewReport", fromview)
    if (rptId)
        thelink = addParameterToURL(thelink, "rptId", rptId)

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

    var searchval = $("#SearchString" + EntityName, $(target).closest("#" + EntityName)).val()
    searchval = SanitizeURLString(searchval);
    thelink = thelink.replace("_SearchString", searchval);

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
function PaginationClick(e, EntityName, UserName, IsReports, fromview, rptId) {
    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    var thelink = e.target.href;
    if (IsReports)
        thelink = addParameterToURL(thelink, "IsReports", IsReports)
    if (fromview)
        thelink = addParameterToURL(thelink, "FromViewReport", fromview)
    if (rptId)
        thelink = addParameterToURL(thelink, "rptId", rptId)
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
    var divarr = $('div', $(target).closest("#" + EntityName))
    if (divarr != undefined) {
        divarr.each(function () {
            var innerDiv = $('div.' + 'col' + index, $("#" + EntityName))
            innerDiv.each(function () {
                if (this.style.display == 'none') {
                    this.style.display = "block";
                }
                else 
                {
                    this.style.display = "none";
                }
                
            });
        });
    }
}
function FSearchColumnsShowHide(indexes, EntityName) {
     if (indexes.length > 0) {
        var indexlist = indexes.split(',')
        for (var i = 0; i < indexlist.length; i++) {
            $('table tr', $("#" + EntityName)).each(function () {
                $('td:eq(' + indexlist[i] + ')', this).toggle();
            });
            $('th.' + 'col' + indexlist[i], $("#" + EntityName)).toggle();
        }
    }
}

function FSearchColumnsShowHideGalaryList(indexes, EntityName) {
    if (indexes.length > 0) {
        var indexlist = indexes.split(',')
        for (var i = 0; i < indexlist.length; i++) {
            var divarr = $('div.' + 'col' + indexlist[i], $("#" + EntityName))
            divarr.each(function () {
                this.style.display = "none";
            });
        }
    }
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
function OpenPopUpImageByte(e, pop, pic, crlimg, docid,rowId) {
    var mypop = "<div class='modal fade' style='cursor:default;' onclick=ClosePopUpImage(event,'Picture_pop_" + rowId + "')" +
    " id='"+pic+"' tabindex='-1' role='dialog' aria-hidden='true'><br />" +
   " <div class='modal-dialog'>" +
       " <div class='modal-content'>" +
           " <div class='modal-header'>" +
               " <button type='button' id='close_Picture_" + rowId + "' onclick=ClosePopUpImage(event,'Picture_pop_" + rowId + "')" +
                " data-dismiss='modal' class='close' aria-hidden='true'>&times;" +
                " </button>" +
                " <div id='" + pop + "'>" +
                 " <img id='" + crlimg + "' style='min-width:100%' /> </div></div></div></div></div>";
    $("#popupDiv").html(mypop);

    e.preventDefault();
    var loc = window.location;
    var pathName = loc.pathname.substring(0, loc.pathname.lastIndexOf('/') + 1);
    $("#" + crlimg).attr("src", pathName + "Document/DisplayImageAfterClick/" + docid);
    var maxHeight = $(window).height() + "px";
    $("#" + pop).css("max-height", maxHeight);
    $("#" + pop + " img").css("max-height", maxHeight);
    $("#" + pic).modal('show');
    var maxWidth = $("#" + pop).width() + "px";
    $("#" + pic+ " img").css("width", maxWidth);
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

function CommonSelectAllRows(obj, type) {
    var checked = false;
    var table = $(obj).closest("div").next("#" + type);
    if ($(obj).is(":checked"))
        checked = true;
    else {
        $("#SelectedItems", table).val('');
    }
    $('input[type=checkbox]', table.find("tr").find("td")).each(function () {
        if (this != obj && !($(this).is(':disabled'))) {
            $(this).prop("checked", checked);
            CommonSelectForBulkOperation(this, $(this).attr("id"), type);
        }
    });
}
function CommonSelectForBulkOperation(source, id, type) {
    var table = $(source).closest("#" + type);
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

//open Quickedit on click of idex records
var IsClicked = false;
function OpenQuickEdit(entityName, RecId, e) {
    if (e.target.tagName != "TD") {
        //e.preventDefault();
        return false;
    }
    if (!IsClicked) {
        e.preventDefault();
        IsClicked = true;
        if ($("#aBtnQuickEdit" + entityName + "_" + RecId).length != 0)
            $("#aBtnQuickEdit" + entityName + "_" + RecId).click();
    }
    IsClicked = false;
    return false;
}
//

 	//
function bindpages(obj, viewTemplates, listTemplates, editTemplates, searchTemplates,createTemplates) {
    var url1 = $(obj).attr("dataurl");
    url1 = url1 + "?EntityName=" + $(obj).val();
    $.ajax({
        url: url1,
        cache: false,
        success: function (result) {
            var indexpage = result.IndexPages;
            var detailspage = result.DetailsPage;
            var editpage = result.EditPage;
            var searchpage = result.SearchPage;
            var createpage = result.CreatePage;
            //
            var viewItems = "";
            var listItems = "";
            var editItems = "";
            var searchItems = "";
            var createItems = "";
            $("#" + viewTemplates).empty();
            $("#" + listTemplates).empty();
            $("#" + editTemplates).empty();
            $("#" + searchTemplates).empty();
            $("#" + createTemplates).empty();
            //
            //List page
            $.each(indexpage, function (dispayvalue, value) {
                if (dispayvalue == "Table(Default)")
                    listItems += "<option selected='selected' value='" + value + "'>" + dispayvalue + "</option>";
                else
                    listItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + listTemplates).html(listItems);
            //
            //Details page
            $.each(detailspage, function (dispayvalue, value) {
                if (dispayvalue == "(Detail)Default")
                    viewItems += "<option selected='selected' value='" + value + "'>" + dispayvalue + "</option>";
                else {
                    viewItems += "<option value='" + value + "'>" + dispayvalue + "</option>";
                }

            });
            $("#" + viewTemplates).html(viewItems);
            //
            //edit page
            $.each(editpage, function (dispayvalue, value) {
                if (dispayvalue == "(Edit)Default")
                    editItems += "<option selected='selected' value='" + value + "'>" + dispayvalue + "</option>";
                else
                    editItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + editTemplates).html(editItems);
            //
            //search page
            $.each(searchpage, function (dispayvalue, value) {
                if (dispayvalue == "Faceted Search(Default)")
                    searchItems += "<option selected='selected' value='" + value + "'>" + dispayvalue + "</option>";
                else
                    searchItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + searchTemplates).html(searchItems);
            //
            //create page
            $.each(createpage, function (dispayvalue, value) {
                if (dispayvalue == "(Create)Default")
                    createItems += "<option selected='selected' value='" + value + "'>" + dispayvalue + "</option>";
                else
                    createItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + createTemplates).html(createItems);
            //
        }
    })
}

function bindpagesFromEdit(obj, viewTemplates, listTemplates, editTemplates, searchTemplates,createTemplates, details, list, edit, search,create) {
    var url1 = $(obj).attr("dataurl");
    url1 = url1 + "?EntityName=" + $(obj).val();
    $.ajax({
        url: url1,
        cache: false,
        success: function (result) {
            var indexpage = result.IndexPages;
            var detailspage = result.DetailsPage;
            var editpage = result.EditPage;
            var searchpage = result.SearchPage;
            var createpage = result.CreatePage;
            //
            var viewItems = "";
            var listItems = "";
            var editItems = "";
            var searchItems = "";
            var createItems = "";
            $("#" + viewTemplates).empty();
            $("#" + listTemplates).empty();
            $("#" + editTemplates).empty();
            $("#" + searchTemplates).empty();
            $("#" + createTemplates).empty();
            //
            //List page
            $.each(indexpage, function (dispayvalue, value) {
                listItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + listTemplates).html(listItems);
            $("#" + listTemplates + " option[value='" + list + "']").attr("selected", "selected");
            //
            //Details page
            $.each(detailspage, function (dispayvalue, value) {
                viewItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + viewTemplates).html(viewItems);
            $("#" + viewTemplates + " option[value='" + details + "']").attr("selected", "selected");
            //
            //edit page
            $.each(editpage, function (dispayvalue, value) {
                editItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + editTemplates).html(editItems);
            $("#" + editTemplates + " option[value='" + edit + "']").attr("selected", "selected");
            //
            //search page
            $.each(searchpage, function (dispayvalue, value) {
                searchItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + searchTemplates).html(searchItems);
            $("#" + searchTemplates + " option[value='" + search + "']").attr("selected", "selected");
            //
            //create page
            $.each(createpage, function (dispayvalue, value) {
                createItems += "<option value='" + value + "'>" + dispayvalue + "</option>";

            });
            $("#" + createTemplates).html(createItems);
            $("#" + createTemplates + " option[value='" + create + "']").attr("selected", "selected");
            //
        }
    })
}
function displayDocumentName(url, Id) {
    $.get(url, {}, function (result) {
        $("#adownload" + Id).html(result);
    }, "json");
}
function displayDocumentNameEdit(url, Id) {
    $.get(url, {}, function (result) {
        var docName;
        var result = result.substr(result.lastIndexOf('\\') + 1)
        var fname = result.substr(0, result.lastIndexOf('.'))
        var ext = result.substr(result.lastIndexOf('.') + 1)
        var len = fname.length;

        if (len > 15) {
            docName = fname.substr(0, 15);
            docName = docName + "..." + ext;
        }
        else 
            docName = result;

        $("#adownload" + Id).html(docName);
        $("#adownload" + Id).attr("title", result)
    }, "json");
}
function uploadedFileName(dispalyobjid, result) {
    var docName;
    var result = result.substr(result.lastIndexOf('\\') + 1)
    var fname = result.substr(0, result.lastIndexOf('.'))
    var ext = result.substr(result.lastIndexOf('.') + 1)
    var len = fname.length;

    if (len > 15) {
        docName = fname.substr(0, 15);
        docName = docName + ".." + ext;
    }
    else
        docName = result;

    $("#" + dispalyobjid).html(docName);
    $("#" + dispalyobjid).attr("title", result)
}
function SanitizeURLString(searchString) {
    var Results = "";

    Results = searchString;

    Results = Results.replace("%", "%25")
    Results = Results.replace("<", "%3C")
    Results = Results.replace(">", "%3E")
    Results = Results.replace("#", "%23")
    Results = Results.replace("{", "%7B")
    Results = Results.replace("}", "%7D")
    Results = Results.replace("|", "%7C")
    Results = Results.replace("\\", "%5C")
    Results = Results.replace("^", "%5E")
    Results = Results.replace("~", "%7E")
    Results = Results.replace("[", "%5B")
    Results = Results.replace("]", "%5D")
    Results = Results.replace("`", "%60")
    Results = Results.replace(";", "%3B")
    Results = Results.replace("/", "%2F")
    Results = Results.replace("?", "%3F")
    Results = Results.replace(":", "%3A")
    Results = Results.replace("@", "%40")
    Results = Results.replace("=", "%3D")
    Results = Results.replace("&", "%26")
    Results = Results.replace("$", "%24")
    return Results
}
function AntiSanitizeURLString(searchString) {
    var Results = "";

    Results = searchString;

    Results = Results.replace("%25", "%")
    Results = Results.replace("%3C", "<")
    Results = Results.replace("%3E", ">")
    Results = Results.replace("%23", "#")
    Results = Results.replace("%7B", "{")
    Results = Results.replace("%7D", "}")
    Results = Results.replace("%7C", "|")
    Results = Results.replace("%5C", "\\")
    Results = Results.replace("%5E", "^")
    Results = Results.replace("%7E", "~")
    Results = Results.replace("%5B", "[")
    Results = Results.replace("%5D", "]")
    Results = Results.replace("%60", "`")
    Results = Results.replace("%3B", ";")
    Results = Results.replace("%2F", "/")
    Results = Results.replace("%3F", "?")
    Results = Results.replace("%3A", ":")
    Results = Results.replace("%40", "@")
    Results = Results.replace("%3D", "=")
    Results = Results.replace("&amp;", "&")
    Results = Results.replace("%24", "$")
    return Results
}
(function ($) {
    $.fn.localTimeFromUTCEdit = function (format) {
        return this.each(function () {
            var tagText = $(this).val();
            if (tagText != "" && tagText != undefined) {
                var givenDate = new Date(tagText);

                var amPm = "";
                var convertedTime = convertLocalDateToUTCDate(givenDate, false);
                if (!format.includes('HH'))
                    amPm = convertedTime.getHours() >= 12 ? "PM" : "AM";
                // format the date
                var localDateString = moment(convertedTime).format(format) + amPm;
                $(this).val(localDateString);
            }
        });
    };
    $.fn.localTimeFromUTCIndex = function (tdid, format, textDate, userName) {
        if (textDate != "" && textDate != undefined) {
           // textDate = new Date();

            var tagText = textDate;
            var givenDate = new Date(tagText);

            var amPm = "";
            var convertedTime = convertLocalDateToUTCDate(givenDate, false);
            if (!format.includes('HH'))
                amPm = convertedTime.getHours() >= 12 ? "PM" : "AM";

            // format the date
            var localDateString = moment(convertedTime).format(format) + amPm;
            $("#" + tdid).html(localDateString + " " + userName);
        }
    };
    $.fn.localTimeFromUTC = function (format) {
        return this.each(function () {
            var tagText = $(this).val();
            if (tagText != "" && tagText != undefined) {
                var givenDate = new Date(tagText);
                var amPm = "";
                var convertedTime = convertLocalDateToUTCDate(givenDate, false);
                if (!format.includes('HH'))
                    amPm = convertedTime.getHours() >= 12 ? "PM" : "AM";
                // format the date
                var localDateString = moment(convertedTime).format(format) + amPm;
                $(this).val(localDateString);
            }
        });
    };
})(jQuery);

function convertLocalDateToUTCDate(date, toUTC) {
    date = new Date(date);
    //Local time converted to UTC
    var localOffset = date.getTimezoneOffset() * 60000;
    //var localOffset = new Date().getTimezoneOffset() / 60000;
    var localTime = date.getTime();
    if (toUTC) {
        date = localTime + localOffset;
    } else {
        date = localTime - localOffset;
    }
    date = new Date(date);
    return date;
}function Set(source, id, DisplayValue) {
    var dropdown = ($('#PopupBulkOperationLabel').attr('class'));
    if (source.checked) {
        if ($('#' + dropdown).attr('multiple') == 'multiple' && $('#' + dropdown).attr('multipleText') != "multipleText") {
            var obj = document.getElementById(dropdown);
            var found = false;
            for (var o = 0; o < obj.options.length; o++) {
                if (obj.options[o].value == id) {
                    found = true;
                    obj.options[o].setAttribute('selected', "selected");
                }
            }
            if (!found) {
                $('#' + dropdown).append($('<option selected=\'selected\'></option>').val(id).html(DisplayValue));
            }
            $('#' + dropdown).multiselect('rebuild');
        }
        if ($('#' + dropdown).attr('multipleText') == "multipleText") {

            var obj = document.getElementById(dropdown);
            var found = false;
            for (var o = 0; o < obj.options.length; o++) {
                if (obj.options[o].value == id) {
                    found = true;
                    obj.options[o].selected = true;
                }
            }
            if (!found) {
                $('#' + dropdown).append($('<option selected=\'selected\'></option>').val(id).html(DisplayValue));
            }
            $('#' + dropdown).select().trigger('change');
        }
    }
    else {
        if ($('#' + dropdown).attr('multiple') == 'multiple' && $('#' + dropdown).attr('multipleText') != "multipleText") {
            var obj = document.getElementById(dropdown);
            for (var o = 0; o < obj.options.length; o++) {
                if (obj.options[o].value == id) {
                    obj.options[o].removeAttribute("selected");
                    $('#' + dropdown).multiselect('deselect', id);

                }
            }
        }
        if ($('#' + dropdown).attr('multipleText') == "multipleText") {
            var obj = document.getElementById(dropdown);
            for (var o = 0; o < obj.options.length; o++) {
                if (obj.options[o].value == id) {
                    obj.options[o].selected = false;
                }
            }
            $('#' + dropdown).select().trigger('change');
        }
    }
}
//time out popup alert
var sess_intervalID;
var sess_lastActivity;
function initSession() {
    sess_lastActivity = new Date();
    sessSetInterval();
    $(document).bind('keypress.session', function (ed, e) {
        sessKeyPressed(ed, e);
    });
}
function sessSetInterval() {
    sess_intervalID = setInterval('sessInterval()', sess_pollInterval);
}
function sessClearInterval() {
    clearInterval(sess_intervalID);

}
function sessKeyPressed(ed, e) {
    sess_lastActivity = new Date();
}
function sessLogOut() {
    ClearFilterCookies();
    $("#logoutForm").submit();
}
function sessInterval() {
    var now = new Date();
    //get milliseconds of differneces
    var diff = now - sess_lastActivity;
    //get minutes between differences
    var diffMins = (diff / 1000 / 60);
    if (diffMins >= sess_warningMinutes) {
        //warn before expiring
        //stop the timer
        sessClearInterval();
        var min = 5;
        if (sess_warningMinutes <= 5)
            min = 1;
        //prompt for attention
        var active = OkClick(min)
        if (active == 1) {
            now = new Date();
            diff = now - sess_lastActivity;
            diffMins = (diff / 1000 / 60);
            if (diffMins > sess_expirationMinutes) {
                sessLogOut();
            }
            else {
                initSession();
                sessSetInterval();
                sess_lastActivity = new Date();
            }
        }
        //else {
        //    sessLogOut();
        //}
    }
}
function OkClick(min) {
    alert('Your session will expire in ' + min + ' minutes. Click Ok to continue working');
    return 1;
}

function ViewReports(url, entityName) {
    $("#ShowReoprtsLabel").html("Report of " + entityName);
    $("#ShowReoprts").modal('show');
    $("#ShowReoprts").find('.modal-dialog.ui-draggable').attr("style", "width:90%");
    $("#LoadReportsDiv").html('Please wait..');
    $("#LoadReportsDiv").load(url);
}
function focusOnControl(formId) {
    var cltIds = $("#" + formId).find('input[type=text]:not([class=hidden]):not([readonly]),textarea:not([readonly])');
    var cltId = "";
    $(cltIds).each(function () {
        if ($(this).attr("id") == undefined)
            return
        var dvhidden = $("#dv" + $(this).attr("id"));
        var dvDate = $("#datetimepicker" + $(this).attr("id")).attr("id");
        if (!(dvhidden.css('display') == 'none') && dvDate == undefined) {
            cltId = $(this);
            return false;
        }
    });
    if (cltId != "" && cltId != undefined)
        setTimeout(function () { $(cltId).focus(); }, 500)
}