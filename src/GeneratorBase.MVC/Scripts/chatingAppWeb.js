var ImgUrl = ""
var pId = "";
var messageUserId = "";
var MsgCounturl = "";
var _groupid = ""
var _groupImagUrl = "";
var fromGroup = false;
var _CountPath = "";
var _username = "";
function ShowCountMessages(path, UserUid, id, fromSendBtn) {
    var Url = path + "&loggedinId=" + UserUid;
    $.ajax({
        type: "GET",
        url: Url,
        contentType: "application/json; charset=utf-8",
        global: false,
        async: false,
        cache: false,
        dataType: "json",
        success: function (jsonObj) {
            var _count = jsonObj.length;
            var msgCount = 0;
            for (i in jsonObj) {
                if (!jsonObj[i].IsRead) {
                    msgCount += 1;
                }
            }
            if (msgCount > 0)
                $("#lbl" + id).html(msgCount);

            if (_count > 0) {
                $("#p" + id).text(jsonObj[_count - 1].Message);
                $("#time" + id).text(jsonObj[_count - 1].Timestamp);
            }

            var GroupItems = "";
        }
    });
}
//for group chating
function ShowGroup(path, ImgUrl, pathGroup, UserUId, loggedPersonId, ImgMsgUrlGroup, CountPath, IsreadPathGroup, username, defalutImg) {
    _CountPath = CountPath;
    _groupImagUrl = ImgMsgUrlGroup
    _username = username;
    $("#GroupItems").html("");
    var Url = path;
    $.ajax({
        type: "GET",
        url: Url,
        contentType: "application/json; charset=utf-8",
        global: false,
        async: false,
        cache: false,
        dataType: "json",
        success: function (jsonObj) {
            var GroupItems = "";
            for (i in jsonObj) {
                if (jsonObj[i].Id != undefined) {
                    GroupItems += "<a data-toggle='modal' id=" + jsonObj[i].Id + " onclick=NavigateToUrl('" + pathGroup + "&PersonName=" + encodeURIComponent(jsonObj[i].DisplayValue) + "&GroupId=" + jsonObj[i].Id + "') dataurl1=" + CountPath + " dataurl2=" + jsonObj[i].Id + ">" +
                        "<div class='msg_container base_receive' style='margin:0px; padding:5px 5px 5px 5px; border:1px solid #c3ddec; background:#edf5fa'>";
                    if (jsonObj[i].GroupIcon != undefined && jsonObj[i].GroupIcon != 0) {
                        GroupItems += "<div class='col-md-2 col-xs-2 avatar'>" +
                            "<img src='" + ImgUrl + "/" + jsonObj[i].GroupIcon + "' class='pricon' style='margin-right: 5px;'>" +
                        "</div>" +
                 "<div class='col-xs-10 col-md-10'>" +
                     "<div class='Listcontact'>" +
                        "<h5>" + jsonObj[i].DisplayValue +
                         " <label class='badge pull-right badge-warning' id='lblGroup" + jsonObj[i].Id + "'></label>" +
                        "</h5>" +
                        "<p id='pGroup" + jsonObj[i].Id + "'></p>" +
                        "<time id='timeGroup" + jsonObj[i].Id + "'></time>" +
                        "</div>" +
                        "</div>" +
                    "</div></a>";
                    }
                    else {
                        GroupItems += "<div class='col-md-1 col-xs-1' style='margin:0px !important; padding:0px !important; width:60px;'>" +
                            "<img src='" + defalutImg + "' class='pricon' style='margin:0px !important; padding:0px !important; width:50px; height:50px;'>" +
                        "</div>" +

                           "<div class='col-xs-11 col-md-11' style='margin:0px !important; padding:0px !important;'>" +
                              "<div class='Listcontact'>" +
                            "<h5>" + jsonObj[i].DisplayValue +
                             " <label class='badge badge-warning' id='lblGroup" + jsonObj[i].Id + "'></label>" +
                            "</h5>" +
                            "<p id='pGroup" + jsonObj[i].Id + "'></p>" +
                            "<time id='timeGroup" + jsonObj[i].Id + "'></time>" +
                        "</div>" +
                  "</div>" +
                  "</div></a>";

                    }

                }
            }

            $("#GroupItems").html(GroupItems);
            for (i in jsonObj) {
                if (jsonObj[i].Id != undefined) {

                    ShowCountMessagesGroup(CountPath, jsonObj[i].Id);
                }
            }
        }
    });


}
function ShowCountMessagesGroup(path, groupId) {
    var Url = path + "?groupid=" + groupId;

    $.ajax({
        type: "GET",
        url: Url,
        contentType: "application/json; charset=utf-8",
        global: false,
        async: false,
        cache: false,
        dataType: "json",
        success: function (jsonObj) {

            var _count = jsonObj.length;
            var msgCount = 0;
            for (i in jsonObj) {
                if (!jsonObj[i].IsRead) {
                    msgCount += 1;
                }
            }
            if (msgCount > 0) {
                $("#lblGroup" + groupId).html(msgCount);
            }

            if (_count > 0) {
                $("#pGroup" + groupId).text(jsonObj[_count - 1].Message);
                $("#timeGroup" + groupId).text(jsonObj[_count - 1].Timestamp);
            }
        }
    });
}
