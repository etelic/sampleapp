//ding and vibarte
function NotificationAlert() {
    var button = document.getElementById('btn-chat');
    var audio = document.getElementById('audio');
    var onClick = function () {
        audio.play(); // audio will load and then play
        navigator.vibrate([1000, 500, 1000, 500, 2000]);
    };
    button.addEventListener('click', onClick, false);
}
//

// show messages dispaly in phone for group and personal
function ShowSendMessages(getLastItemPath, logedinuserid, logedinpersonId, GroupId, ImgUrl, popImgUrl, PersonId, PersonUserId, Codeprefix) {


    var Url = getLastItemPath;
    $.ajax({
        type: "GET",
        url: Url,
        contentType: "application/json; charset=utf-8",
        global: false,
        async: false,
        cache: false,
        dataType: "json",
        success: function (jsonObj) {
            var MsgItems = "";
            if (jsonObj.Id != undefined) {
                if (jsonObj.GroupMessagesID == 0) {
                    AddChatOfPerson(jsonObj, logedinuserid, logedinpersonId, ImgUrl, popImgUrl, PersonId, PersonUserId);
                }
                else {
                    AddChatOfGroup(jsonObj, logedinuserid, logedinpersonId, ImgUrl, GroupId, popImgUrl, PersonId, PersonUserId);
                }
            }
            $("#" + Codeprefix + "Picture").val('');


        }
    });

}
function AddChatOfPerson(jsonObj, logedinuserid, logedinpersonId, ImgUrl, popImgUrl, PersonId, PersonUserId) {


    var MsgItems = "";
    var MsgItemsPicPop = "";
    if (jsonObj.Picture != 0) {

        MsgItemsPicPop += "<div class='modal fade' style='cursor:default;' onclick=ClosePopUpImageChat(event,'Picture_pop_" + jsonObj.Id + "') id='Picture_pop_" + jsonObj.Id + "' tabindex='-1' role='dialog' aria-hidden='true'>" +
     "<br />" +
     "<div class='modal-dialog'>" +
         "<div class='modal-content'>" +
             "<div class='modal-header brandcolor' style='background-color:black;opacity: 0.3;'>" +
                 "<h1 id='myModalLabel' class='modal-title'>" +
                     "<a id='close_Picture_" + jsonObj.Id + "' onclick=ClosePopUpImageChat(event,'Picture_pop_" + jsonObj.Id + "')" +
      " data-dismiss='modal' aria-hidden='true' style='color:white'>" +
       "<i class='fa fa-arrow-left' style='color:white'></i></a></h1></div><div id='dvPopup_Picture_" + jsonObj.Id + "'><img id='img_Picture_" + jsonObj.Id + "' style='min-width:100%' /></div></div></div></div>";

        $('#divUserSenderPop').html(MsgItemsPicPop);

        if (logedinuserid == jsonObj.MessageUserID) {


            MsgItems += "<div class='row msg_container base_sent'>" +
         "<div class='Uploadimg-in'>"
         + "<label>" + jsonObj.Message + "</label>" +
          "<a data-target='#dvPopup_Picture_" + jsonObj.Id + "' onclick=OpenPopUpImageByteForChat(event," + encodeURIComponent("'dvPopup_Picture_" + jsonObj.Id + "'") + "," + encodeURIComponent("'Picture_pop_" + jsonObj.Id + "'") + "," + encodeURIComponent("'img_Picture_" + jsonObj.Id + "'") + "," + encodeURIComponent("'" + jsonObj.Picture + "'") + ",'" + popImgUrl + "')>" +
            "<img src='" + ImgUrl + "/" + jsonObj.Picture + "?maxSize = 200' style='width:200px'/>" +
             "</a>" +
          "<div class='clearfix'>" +
            "</div>" +
            "</div>" +
         "</div>";

        }
        else {
            MsgItems += "<div class='row msg_container base_sent'>" +
        "<div class='Uploadimg-out'>"
        + "<label>" + jsonObj.Message + "</label>" +
        "<a data-target='#dvPopup_Picture_" + jsonObj.Id + "' onclick=OpenPopUpImageByteForChat(event," + encodeURIComponent("'dvPopup_Picture_" + jsonObj.Id + "'") + "," + encodeURIComponent("'Picture_pop_" + jsonObj.Id + "'") + "," + encodeURIComponent("'img_Picture_" + jsonObj.Id + "'") + "," + encodeURIComponent("'" + jsonObj.Picture + "'") + ",'" + popImgUrl + "')>" +
           "<img src='" + ImgUrl + "/" + jsonObj.Picture + "?maxSize = 200'  style='width:200px'/>" +
            "</a>" +
         "<div class='clearfix'>" +
           "</div>" +
           "</div>" +
        "</div>";


        }
    }
    else {

        if (logedinuserid == jsonObj.MessageUserID) {
            MsgItems += "<div class='row msg_container base_receive'>" +
                     "<div class='col-xs-10 col-md-10'>" +
                "<div class='messages msg_receive brandSecondarycolor'>" +
                    "<p>" + jsonObj.Message + "</p>" +
                    "<time>" + jsonObj.Timestamp + "</time>  <span class='pull-right name'>" + jsonObj.Name + "</span>" +
                "</div></div></div>";

        }
        else {
            MsgItems += "<div class='row msg_container base_sent'>" +
               "<div class='col-md-10 col-xs-10'>" +
                   "<div class='messages msg_sent'>" +
                       "<p>" + jsonObj.Message + "</p>" +
                       "<span class='pull-left name'>You</span><time>" + jsonObj.Timestamp + "</time>" +
                   "</div>" +
               "</div></div>";
        }
    }
    if ((jsonObj.MessagesSenderID == PersonId && jsonObj.MessageUserID == logedinuserid) ||
                   (jsonObj.MessagesSenderID == logedinpersonId && jsonObj.MessageUserID == PersonUserId)) {
        $('#ModelBody').append(MsgItems);
        $("#ModelBody").scrollTop($("#ModelBody").prop('scrollHeight') - $("#ModelBody").position().top);

    }
    if (jsonObj.MessagesSenderID == PersonId && jsonObj.MessageUserID == logedinuserid) {

        var button = document.getElementById('btn-chat');
        //var audio = document.getElementById('audio');
        //var onClick = function () {
        //    audio.play(); // audio will load and then play
        //};
        //navigator.vibrate([1000, 500, 1000, 500, 2000]);
        //button.addEventListener('click', onClick, true);
    }


}
function AddChatOfGroup(jsonObj, logedinuserid, logedinpersonId, ImgUrl, GroupId, popImgUrl, PersonId, PersonUserId) {


    var MsgItems = "";
    var MsgItemsPicPop = "";

    if (jsonObj.Picture != 0) {
        MsgItemsPicPop += "<div class='modal fade' style='cursor:default;' onclick=ClosePopUpImageChat(event,'Picture_pop_" + jsonObj.Id + "') id='Picture_pop_" + jsonObj.Id + "' tabindex='-1' role='dialog' aria-hidden='true'>" +
    "<br />" +
    "<div class='modal-dialog'>" +
        "<div class='modal-content'>" +
            "<div class='modal-header brandcolor' style='background-color:black;opacity: 0.3;'>" +
                "<h1 id='myModalLabel' class='modal-title'>" +
                    "<a id='close_Picture_" + jsonObj.Id + "' onclick=ClosePopUpImageChat(event,'Picture_pop_" + jsonObj.Id + "')" +
     " data-dismiss='modal' aria-hidden='true' style='color:white'>" +
      "<i class='fa fa-arrow-left' style='color:white'></i></a></h1></div><div id='dvPopup_Picture_" + jsonObj.Id + "'> <img id='img_Picture_" + jsonObj.Id + "' style='min-width:100%' /></div></div></div></div>";

        $('#divUserSenderPop').html(MsgItemsPicPop);
        if (logedinuserid == jsonObj.MessageUserID) {
            MsgItems += "<div class='row msg_container base_sent'>" +
     "<div class='Uploadimg-in'>"
     + "<label>" + jsonObj.Message + "</label>" +
    "<a data-target='#dvPopup_Picture_" + jsonObj.Id + "' onclick=OpenPopUpImageByteForChat(event," + encodeURIComponent("'dvPopup_Picture_" + jsonObj.Id + "'") + "," + encodeURIComponent("'Picture_pop_" + jsonObj.Id + "'") + "," + encodeURIComponent("'img_Picture_" + jsonObj.Id + "'") + "," + encodeURIComponent("'" + jsonObj.Picture + "'") + ",'" + popImgUrl + "')>" +
        "<img src='" + ImgUrl + "/" + jsonObj.Picture + "?maxSize = 200'  style='width:200px'/>" +
         "</a>" +
      "<div class='clearfix'>" +
        "</div>" +
        "</div>" +
     "</div>";
        }
        else {
            MsgItems += "<div class='row msg_container base_sent'>" +
     "<div class='Uploadimg-out'>"
     + "<label>" + jsonObj.Message + "</label>" +
      "<a data-target='#dvPopup_Picture_" + jsonObj.Id + "' onclick='OpenPopUpImageByteForChat(event," + encodeURIComponent("'dvPopup_Picture_" + jsonObj.Id + "'") + "," + encodeURIComponent("'Picture_pop_" + jsonObj.Id + "'") + "," + encodeURIComponent("'img_Picture_" + jsonObj.Id + "'") + "," + encodeURIComponent("'" + jsonObj.Picture + "'") + ",'" + popImgUrl + "')>" +
        "<img src='" + ImgUrl + "/" + jsonObj.Picture + "?maxSize = 200'  style='width:200px'/>" +
         "</a>" +
      "<div class='clearfix'>" +
        "</div>" +
        "</div>" +
     "</div>";

        }

    }
    else {

        if (logedinuserid == jsonObj.MessageUserID) {
            MsgItems += "<div class='row msg_container base_sent'>" +
              "<div class='col-md-10 col-xs-10'>" +
                  "<div class='messages msg_sent'>" +
                      "<p>" + jsonObj.Message + "</p>" +
                      "<span class='pull-left name'>You</span><time>" + jsonObj.Timestamp + "</time>" +
                  "</div>" +
              "</div></div>";

        }
        else {
            MsgItems += "<div class='row msg_container base_receive'>" +
                     "<div class='col-xs-10 col-md-10'>" +
                "<div class='messages msg_receive brandSecondarycolor'>" +
                    "<p>" + jsonObj.Message + "</p>" +
                    "<time>" + jsonObj.Timestamp + "</time>  <span class='pull-right name'>" + jsonObj.Name + "</span>" +
                "</div></div></div>";



        }
    }
    if (jsonObj.GroupMessagesID != 0 && GroupId == jsonObj.GroupMessagesID) {
        $('#GroupBody' + GroupId).append(MsgItems);
        $('#GroupBody' + GroupId).scrollTop($('#GroupBody' + GroupId).prop('scrollHeight') - $('#GroupBody' + GroupId).position().top)
        var button = document.getElementById('btn-chat');
        //var audio = document.getElementById('audio');
        //var onClick = function () {
        //    audio.play(); // audio will load and then play
        //};
        //navigator.vibrate([1000, 500, 1000, 500, 2000]);
        //button.addEventListener('click', onClick, true);

    }
}
function PerformIsReadOperation(url1, personid, userUid) {
    var Url = url1 + "?personId=" + personid + "&loginUid=" + userUid
    $.ajax({
        type: "GET",
        url: Url,
        contentType: "application/json; charset=utf-8",
        global: false,
        async: false,
        cache: false,
        dataType: "json",
        success: function (msg) {
            // $("#" + entity + "SearchCancel").click();
        }
    });
}
function PerformIsReadOperationGroup(url1, GroupId, userUid) {
    var Url = url1 + "?groupId=" + GroupId + "&loginUid=" + userUid
    $.ajax({
        type: "GET",
        url: Url,
        contentType: "application/json; charset=utf-8",
        global: false,
        async: false,
        cache: false,
        dataType: "json",
        success: function (msg) {
            // $("#" + entity + "SearchCancel").click();
        }
    });
}
//

//create messages on data base json
function CreateMessagesEvent(e, fromName, CodePrefix) {

    var target;
    if (e.srcElement) target = e.srcElement;
    e = $.event.fix(e);
    if (e.currentTarget) target = e.currentTarget;
    e.preventDefault();
    //for date in iOs
    var fullDate = new Date();
    var twoDigitMonth = fullDate.getMonth() + 1 + ""; if (twoDigitMonth.length == 1) twoDigitMonth = "0" + twoDigitMonth;
    var twoDigitDate = fullDate.getDate() + ""; if (twoDigitDate.length == 1) twoDigitDate = "0" + twoDigitDate;
    var currentDate = twoDigitMonth + "/" + twoDigitDate + "/" + fullDate.getFullYear();

    var h = fullDate.getHours();
    var m = fullDate.getMinutes();
    h = h % 12;
    h = h ? h : 12; // the hour '0' should be '12'
    var ampm = h >= 12 ? 'PM' : 'AM';
    m = m < 10 ? '0' + m : m;
    var currentTime = h + ':' + m + ' ' + ampm;
    //
    $("#" + CodePrefix + "Timestamp").val(currentDate + " " + currentTime);

    var form = "";

    if (fromName != "") {
        form = $("#" + fromName).closest('form');
    }
    else {
        form = $(target).closest('form');
    }
    if (form.valid()) {

        var fd = "";
        var url = "";
        if (fromName != "") {
            fd = $(form).closest('form').serialize();
            url = $(form).closest('form').attr("action");
            fd = new FormData(form[0]);
        }
        else {
            fd = $(target).closest('form').serialize();
            url = $(target).closest('form').attr("action");
            fd = new FormData(form[0]);
        }

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
                    $("#" + CodePrefix + "Message").val("");
                    if ($('input:hidden[name="CamerafileUploadPicture"]').val() != "")
                        $('input:hidden[name="CamerafileUploadPicture"]').val('')
                    //CallSignalr();

                } else {
                    //alert(result);
                    $("#" + CodePrefix + "Message").removeClass();
                    $("#" + CodePrefix + "Message").addClass("form-control input-validation-error");
                    $("#" + CodePrefix + "Message").val("");
                }
            }
        });
    }
}
//camera And imgaes
//set  button for cemare and gallary
var CameraDataMessagesPictures = "";

function OpenPopUpImageByteForChat(e, pop, pic, crlimg, docid, pathname) {

    e.preventDefault();

    $("#" + crlimg).attr("src", pathname + "/" + docid);

    var maxHeight = $(window).height() + "px";

    $("#" + pop).css("max-height", maxHeight);
    $("#" + pop + " img").css("max-height", maxHeight);

    $("#" + pic).modal('show');

    var maxWidth = $("#" + pop).width() + "px";
    $("#" + pop + " img").css("width", maxWidth);

}


function ClosePopUpImageChat(e, pic) {
    e.preventDefault();
    $("#" + pic).modal('hide');
}
function fromapkornot(userName, CodePrefix) {
    var btnVar = "";
    var UserString = encodeURIComponent(userName);

    if (window.localStorage.getItem("fromapk") == "true") {
        btnVar = "<button type='button' class='btn' id='btngallery' onclick=uploadPicture('" + UserString + "','" + CodePrefix + "');><i class='fa fa-camera' style='color:#020202;'></i></button>" +
        "<button type='button' class='btn btn-primary' onclick=capturePhotoAddMessagesPictures(event,'" + UserString + "'," + CodePrefix + "')><i class='glyphicon glyphicon-camera'/></button>";
    }
    else {
        btnVar = "<button type='button' class='btn' id='btngallery' onclick=uploadPicture('" + UserString + "','" + CodePrefix + "');><i class='fa fa-camera' style='color:#020202;'></i></button>";
    }
    $("#btnspan").prepend(btnVar);
}
function ClosePopUpSend(e, popid, CodePrefix) {
    e.preventDefault();
    $("#" + CodePrefix + "Picture").val('');
    $("#" + popid).modal('hide');
    $("#PicMessage").val('');
    $("#" + CodePrefix + "Message").val('');

}
function SendPhoto(senderName, e, CodePrefix) {
    var picCaption = $("#PicMessage").val();
    if (picCaption.length == 0) {
        $("#" + CodePrefix + "Message").val(senderName)
    }
    else {
        $("#" + CodePrefix + "Message").val(picCaption)
    }
    CreateMessagesEvent(e, "MsgFrom", CodePrefix);
    $("#" + CodePrefix + "Message").val('');
    $("#Picture_pop").modal('hide');
    $("#PicMessage").val("");

}
function uploadPicture(userName, CodePrefix) {

    $("#" + CodePrefix + "Message").val(userName);
    $("#" + CodePrefix + "Picture").click();
}
function capturePhotoAddMessagesPictures(e, username, CodePrefix) {
    var destinationType = navigator.camera.DestinationType;
    navigator.camera.getPicture(onPhotoDataSuccessMessagesPictures, onFail, { quality: 10, destinationType: destinationType.DATA_URL, correctOrientation: true });
    $("#" + CodePrefix + "Message").val(username);
}
function onPhotoDataSuccessMessagesPictures(imageData) {
    // Get image handle
    var smallImage = document.getElementById('img_Picture');
    smallImage.src = "data:image/jpeg;base64," + imageData;
    CameraDataMessagesPictures = imageData;
    $('input:hidden[name="CamerafileUploadPicture"]').val(CameraDataMessagesPictures);
    document.getElementById('img_Picture').val('');

}
function onFail(message) {
    // alert('Response: ' + message);
}