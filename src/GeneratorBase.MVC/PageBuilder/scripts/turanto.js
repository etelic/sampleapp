

$.cssHooks.backgroundColor = {
    get: function (elem) {
        if (elem.currentStyle)
            var bg = elem.currentStyle["background-color"];
        else if (window.getComputedStyle)
            var bg = document.defaultView.getComputedStyle(elem,
                    null).getPropertyValue("background-color");
        if (bg.search("rgb") == -1)
            return bg;
        else {
            bg = bg.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
            function hex(x) {
                return ("0" + parseInt(x).toString(16)).slice(-2);
            }

        }
    }
}

'use strict';
//Make sure jQuery has been loaded before app.js
if (typeof jQuery === "undefined") {
    throw new Error("HtmlEditor requires jQuery");
}

$(function () {
    //Set up the object
    _init();
});

/* ----------------------------------
 * ----------------------------------
 * All HTMLeditor functions are implemented below.
 */


function _init() {

    $(window).resize(function () {
        $("body").css("min-height", $(window).height() - 90);
        $(".TurantoPage").css("min-height", $(window).height() - 160)
    });

    tinymce.init({
        menubar: false,
        force_br_newlines: false,
        force_p_newlines: false,
        forced_root_block:'',
        extended_valid_elements: "*[*]",
        valid_elements: "*[*]",
        selector: "#html5editor",
        plugins: [
            "advlist autolink lists link charmap anchor",
            "visualblocks code ",
            "insertdatetime  table contextmenu paste textcolor colorpicker"
        ],
        toolbar: "styleselect | bold italic |  forecolor backcolor |alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link code",
    });

    tinymce.init({
        menubar: false,
        force_br_newlines: false,
        force_p_newlines: false,
        forced_root_block: '',
        extended_valid_elements: "*[*]",
        valid_elements: "*[*]",
        selector: "#html5editorLite",
        plugins: [
        ],
        toolbar: "forecolor backcolor | alignleft aligncenter alignright alignjustify code",
    });

    $("body").css("min-height", $(window).height() - 90);
    $(".TurantoPage").css("min-height", $(window).height() - 160);
    $(".TurantoPage, .TurantoPage .column").sortable({ connectWith: ".column", opacity: .35, handle: ".drag" });
    $(".sidebar-builder .TurantoRow").draggable({
        connectToSortable: ".TurantoPage", helper: "clone", handle: ".drag", drag: function (e, t) {
            t.helper.width(400); contentCopy();
        }, stop: function (e, t) {
            $(".TurantoPage .column").sortable({ opacity: .35, connectWith: ".column" });
            contentCopy();
        }
    });
    $(".sidebar-builder .TurantoRow").droppable({
        connectToSortable: ".TurantoPage",
        revert: "invalid",
        handle: ".drag",
        drop: function (e, t) {
            contentCopy();
        }
    });

    $(".sidebar-builder .box").draggable({
        connectToSortable: ".column", helper: "clone", handle: ".preview", drag: function (e, t) {
            t.helper.width(400)
            contentCopy();
        }, stop: function (e, t) {
            /* if ( t.helper.data('type')==="map"|| t.helper.data('type')==="youtube" ) {
             var iframe = t.helper.find('div.view > iframe');

             var iframeId = iframe.assignId();
             $('#'+iframeId).attr('src',iframe.data('url'));
             }
             */

        }
    });


    //function editlable() {
    //    var label = $(this);
    //    alert(label);
    //    //Add a TextBox next to the Label.
    //    label.after("<input type = 'text' style = 'display:none' />");

    //    //Reference the TextBox.
    //    var textbox = $(this).next();

    //    //Set the name attribute of the TextBox.
    //    textbox[0].name = this.id.replace("lbl", "txt");

    //    //Assign the value of Label to TextBox.
    //    textbox.val(label.html());

    //    //When Label is clicked, hide Label and show TextBox.
    //    label.click(function () {

    //        $(this).hide();
    //        $(this).next().show();
    //    });

    //    //When focus is lost from TextBox, hide TextBox and show Label.
    //    textbox.focusout(function () {
    //        $(this).hide();
    //        $(this).prev().html($(this).val());
    //        $(this).prev().show();
    //    });
    //}

    //$(".editable").each(function () {
    //    var label = $(this);

    //    //Add a TextBox next to the Label.
    //    label.after("<input type = 'text' style = 'display:none' />");

    //    //Reference the TextBox.
    //    var textbox = $(this).next();

    //    //Set the name attribute of the TextBox.
    //    textbox[0].name = this.id.replace("lbl", "txt");

    //    //Assign the value of Label to TextBox.
    //    textbox.val(label.html());

    //    //When Label is clicked, hide Label and show TextBox.
    //    label.click(function () {

    //        $(this).hide();
    //        $(this).next().show();
    //    });

    //    //When focus is lost from TextBox, hide TextBox and show Label.
    //    textbox.focusout(function () {
    //        $(this).hide();
    //        $(this).prev().html($(this).val());
    //        $(this).prev().show();
    //    });
    //});

    $(document).on('click', '.editable', function (e) {
        var label = $(this);

        //Add a TextBox next to the Label.
        label.after("<input type = 'text' style = 'display:none' />");

        //Reference the TextBox.
        var textbox = $(this).next();

        //Set the name attribute of the TextBox.
        textbox[0].name = this.id.replace("lbl", "txt");

        //Assign the value of Label to TextBox.
        textbox.val(label.html());

        //When Label is clicked, hide Label and show TextBox.
        //label.click(function () {
        $(this).html("");
        $(this).hide();
        $(this).next().show();
        //});

        //When focus is lost from TextBox, hide TextBox and show Label.
        textbox.focusout(function () {
            $(this).hide();
            if ($(this).val() == "")
                $(this).val("Heading");
            $(this).prev().html($(this).val());
            $(this).prev().show();
            $(this).remove();
            contentCopy();
        });
    });

    $(document).on('click', 'a.clone', function (e) {
        e.preventDefault();
        var _s = $(this);

        var _row = _s.parent().clone();
        _row.hide();
        _row.insertAfter(_s.parent());
        _row.slideDown();

    });

    $(document).on('click', 'a.settings', function (e) {
        e.preventDefault();
        var _s = $(this);
        //alert(_s.parent().parent().assignId());
        var part_id = _s.parent().parent().assignId();

        var part = _s.parent().parent();
        var column = _s.parent().parent().parent('.column');
        var row = _s.parent().parent().parent().parent('.row');

        prepareEditor(part, row, column);
    });

    $('a.btnpropa').on('click', function () {
        var rel = $(this).attr('rel');
        $('#buttonContainer a.btn').removeClass('btn-default')
                .removeClass('btn-success')
                .removeClass('btn-info')
                .removeClass('btn-danger')
                .removeClass('btn-info')
                .removeClass('btn-primary')
                .removeClass('btn-link')
                .addClass(rel);
    });
    $('a.btnpropb').on('click', function () {
        var rel = $(this).attr('rel');
        $('#buttonContainer a.btn').removeClass('btn-lg')
                .removeClass('btn-md')
                .removeClass('btn-sm')
                .removeClass('btn-xs')
                .addClass(rel);

    });

    $('a.btnprop').on('click', function () {
        var rel = $(this).attr('rel');
        $('#buttonContainer a.btn').toggleClass(rel);

    });

    $('#preferences').on('hidden.bs.modal', function () {
        $('#youtube').hide();
        $('#map').hide();
        $('#image').hide();
        $('#text').hide();
        $('#code').hide();
        $('#buttons').hide();
        // $('.active').removeClass('active');
    });

    $("#save").click(function (e) {
        downloadLayoutSrc();
    });


    $("#clear").click(function (e) {
        e.preventDefault();
        clearDemo()
    });
    $("#devpreview").click(function () {
        $("body").removeClass("edit sourcepreview");
        $("body").addClass("devpreview");
        removeMenuClasses();
        $(this).addClass("active");
        return false
    });


    $("#edit").click(function () {
        $('#add').hide();
        $("body").removeClass("devpreview sourcepreview");
        $("body").removeClass("tablet mobile");
        $("body").addClass("edit");
        removeMenuClasses();
        $(this).addClass("active");
        return false
    });


    $('#gallery').click(function () {
        $('#thepref').slideUp();
        $('#mediagallery').slideDown();
    });


    $("#sourcepreview").click(function () {
        $('#pc').addClass('active');
        $('#add').show();
        $("body").removeClass("edit");
        $("body").addClass("devpreview sourcepreview");
        removeMenuClasses();
        $(this).addClass("active");
        return false
    });



    $('#pc').click(function () {
        $("body").removeClass("tablet mobile");
        $('#app button').removeClass('active');
        $(this).addClass('active');
    });


    $('#mobile').click(function () {
        $("body").removeClass("tablet");
        $('#app button').removeClass('active');
        $(this).addClass('active');
        $("body").addClass("mobile");
    });


    $('#tablet').click(function () {
        $("body").removeClass("mobile");
        $('#app button').removeClass('active');
        $(this).addClass('active');
        $("body").addClass("tablet");
    });

    $(".nav-header").click(function () {
        $(".sidebar-builder .boxes, .sidebar-builder .rows").hide();
        $(this).next().slideDown()

    });


    removeElm();
    gridSystemGenerator();

}

function loadRowSettings(row) {
    //RowSettings
    // paddings
    $('#tabRow input[data-ref="padding-top"]').val(row.css('padding-top'));
    $('#tabRow input[data-ref="padding-left"]').val(row.css('padding-left'));
    $('#tabRow input[data-ref="padding-right"]').val(row.css('padding-right'));
    $('#tabRow input[data-ref="padding-bottom"]').val(row.css('padding-bottom'));
    // margin
    $('#tabRow input[data-ref="margin-top"]').val(row.css('margin-top'));
    $('#tabRow input[data-ref="margin-left"]').val(row.css('margin-left'));
    $('#tabRow input[data-ref="margin-right"]').val(row.css('margin-right'));
    $('#tabRow input[data-ref="margin-bottom"]').val(row.css('margin-bottom'));
    // backgroundColor
    $('#rowbg').val(row.css('background-color'));
    // image
    //$('#rowbgimage').val(row.css('background-image').replace(/^url\(['"]?/,'').replace(/['"]?\)$/,''));
    // css class
    $('#rowcss').val(row.attr('class'));
}

function saveRowSettings(row) {
    //RowSettings
    //padding
    row.css('padding-top', $('#tabRow input[data-ref="padding-top"]').val());
    row.css('padding-left', $('#tabRow input[data-ref="padding-left"]').val());
    row.css('padding-right', $('#tabRow input[data-ref="padding-right"]').val());
    row.css('padding-bottom', $('#tabRow input[data-ref="padding-bottom"]').val());
    // margin
    row.css('margin-top', $('#tabRow input[data-ref="margin-top"]').val());
    row.css('margin-left', $('#tabRow input[data-ref="margin-left"]').val());
    row.css('margin-right', $('#tabRow input[data-ref="margin-right"]').val());
    row.css('margin-bottom', $('#tabRow input[data-ref="margin-bottom"]').val());
    // backgroundColor
    row.css('background-color', $('#rowbg').val());
    // image
    if ($("#rowbgimage").val() != "none")
        row.css('background-image', 'url("' + $("#rowbgimage").val() + '")');
    // css class
    row.removeClass();
    row.addClass($('#rowcss').val());
    //row.attr('css', $('#rowcss').val());
}

function loadColumnSettings(column) {
    // paddings
    $('#tabCol input[data-ref="padding-top"]').val(column.css('padding-top'));
    $('#tabCol input[data-ref="padding-left"]').val(column.css('padding-left'));
    $('#tabCol input[data-ref="padding-right"]').val(column.css('padding-right'));
    $('#tabCol input[data-ref="padding-bottom"]').val(column.css('padding-bottom'));
    // margin
    $('#tabCol input[data-ref="margin-top"]').val(column.css('margin-top'));
    $('#tabCol input[data-ref="margin-left"]').val(column.css('margin-left'));
    $('#tabCol input[data-ref="margin-right"]').val(column.css('margin-right'));
    $('#tabCol input[data-ref="margin-bottom"]').val(column.css('margin-bottom'));
    // backgroundColor
    $('#colbg').val(column.css('background-color'));
    // css class
    $('#colcss').val(column.attr('class'));
}
function saveColumnSettings(column) {
    //CellSettings
    //padding
    column.css('padding-top', $('#tabCol input[data-ref="padding-top"]').val());
    column.css('padding-left', $('#tabCol input[data-ref="padding-left"]').val());
    column.css('padding-right', $('#tabCol input[data-ref="padding-right"]').val());
    column.css('padding-bottom', $('#tabCol input[data-ref="padding-bottom"]').val());
    // margin
    column.css('margin-top', $('#tabCol input[data-ref="margin-top"]').val());
    column.css('margin-left', $('#tabCol input[data-ref="margin-left"]').val());
    column.css('margin-right', $('#tabCol input[data-ref="margin-right"]').val());
    column.css('margin-bottom', $('#tabCol input[data-ref="margin-bottom"]').val());
    // backgroundColor
    column.css('background-color', $('#colbg').val());
    // css class
    column.attr('class', $('#colcss').val());
}

function prepareEditor(part, row, column) {
    var clone = part.clone();
    var confirm = $('#applyChanges');
    $('#preferencesTitle').html(part.data('type'));

    $('.column .box').removeClass('active');
    part.addClass('active');
    confirm.unbind('click');

    var clonedPart = clone.find('div.view').html();
    var type = part.data('type');
    var panel = $('#Settings');

    loadRowSettings(row);
    loadColumnSettings(column);

    var o = part.find('div.view').children();
    var oid = o.assignId();
    $('#id').val(oid);
    $('#class').val(o.parent().parent().css('class'));
    $('#class').parent().hide();
    $('#id').parent().hide();
    switch (type) {

        /*
        case 'header':
            var editor = tinyMCE.get('html5editor');
            editor.setContent(clonedPart);
            $('#text').show();

            confirm.bind('click', function (e) {
                e.preventDefault();
                saveRowSettings(row);
                saveColumnSettings(column);
                o.html(editor.getContent());
                o.attr('id', $('#id').val());
                o.attr('class', $('#class').val());
            });
            break;
        */
        case 'Editor':

            var editor = tinyMCE.get('html5editor');
            editor.setContent(clonedPart);
            $('#text').show();

            var o = part.find('div.view');
            confirm.bind('click', function (e) {
                e.preventDefault();
                saveRowSettings(row);
                saveColumnSettings(column);
                o.html(editor.getContent());
                o.attr('id', $('#id').val());
                //o.attr('class', $('#class').val());
                contentCopy();
            });

            break;

        case 'image':
            var img = part.find('img');

            $('#imgContent').html(img.clone().attr('width', '200'));
            $('#img-url').val(img.attr('src'));
            $('#img-width').val(img.innerWidth());
            $('#img-height').val(img.innerHeight());
            $('#img-title').val(img.attr('title'));
            $('#class').val(img.attr('class'));
            $('#img-rel').val(img.attr('rel'));
            $('#img-title').val(img.attr('title'));
            // $('#img-clickurl').val(img.attr('onClick'));
            $('#image').show();

            confirm.bind('click', function (e) {
                e.preventDefault();
                saveRowSettings(row);
                saveColumnSettings(column);
                img.attr('title', $('#img-title').val());
                img.attr('src', $('#img-url').val());
                img.css('width', $('#img-width').val());
                img.css('height', $('#img-height').val());
                img.attr('class', $('#class').val());
                img.attr('rel', $('#img-rel').val());
                //    img.attr('onClick', $('#img-clickurl').val());
                o.attr('id', $('#id').val());
                o.removeClass();
                o.addClass($('#class').val());
            });

            break;
        case 'youtube':
            var iframe = part.find('iframe');
            $('#youtube-video').html(iframe.clone().css('width', '100%'));
            $('#video-url').val(iframe.attr('src'));
            $('#video-width').val(iframe.innerWidth());
            $('#video-height').val(iframe.innerHeight());
            $('#youtube').show();

            confirm.bind('click', function (e) {
                e.preventDefault();
                saveRowSettings(row);
                saveColumnSettings(column);
                o.attr('src', $('#video-url').val());
                o.css('width', $('#video-width').val());
                o.css('height', $('#video-height').val());
                o.attr('id', $('#id').val());
                o.attr('class', $('#class').val());
            });
            break;
        case 'map':
            var iframe = part.find('iframe');
            var c = iframe.clone();
            $('#map-content').html(c.attr('width', '100%'));
            $('#address');
            $('#map').show();
            $('#map-width').val(iframe.innerWidth());
            $('#map-height').val(iframe.innerHeight());
            var url = iframe.attr('src');
            var latlon = gup('q', url).split(',');
            var z = gup('z', url);

            $('#latitude').val(latlon[0]);
            $('#longitude').val(latlon[1]);
            $('#zoom').val(z);

            //http://maps.google.com/maps?q=12.927923,77.627108&z=15&output=embed
            $('#latitude, #longitude, #zoom').change(function () {
                c.attr('src', 'http://maps.google.com/maps?q=' + $('#latitude').val() + ',' + $('#longitude').val() + '&z=' + $('#zoom').val() + '&output=embed');
            });

            confirm.bind('click', function (e) {
                e.preventDefault();
                saveRowSettings(row);
                saveColumnSettings(column);
                iframe.css('width', $('#map-width').val());
                iframe.css('height', $('#map-height').val());
                iframe.attr('src', 'http://maps.google.com/maps?q=' + $('#latitude').val() + ',' + $('#longitude').val() + '&z=' + $('#zoom').val() + '&output=embed');
                o.attr('id', $('#id').val());
                o.attr('class', $('#class').val());
            });
            break;
        case 'code':
            $('#class').parent().hide();
            $('#id').parent().hide();
            var txt = $('#code');
            $('#codeeditor').remove();
            txt.append('<textarea id="codeeditor" style="min-height:150px;width:100%; display:block;">' + style_html(part.find('div.view').html()) + '</textarea>');
            txt.show();


            /*

            var code = part.find('div.code');
            var txt = $('#code');
            $('#codeeditor').remove();
            txt.append('<textarea id="codeeditor" style="min-height:150px;width:100%; display:block;">'+style_html(code.data('code'))+'</textarea>');
            txt.show();

            txt.append('<div id="codeeditor" style="min-height:150px;width:100%; display:block;">' + code.data('code') + '</div>');
            var editor = ace.edit("codeeditor");
            editor.setTheme("ace/theme/monokai");
            editor.getSession().setMode("ace/mode/html");
            txt.show();
            confirm.bind('click', function (e) {
                e.preventDefault();
                saveRowSettings(row);
                saveColumnSettings(column);
                code.data('code', editor.getValue());
                code.html(editor.getValue());
            });
            */



            confirm.bind('click', function (e) {
                e.preventDefault();
                saveRowSettings(row);
                saveColumnSettings(column);
                //code.data('code', $('#codeeditor').val());
                part.find('div.view').html($('#codeeditor').val());

            });
            break;
        case 'button':

            var btn = part.find('.view > a.btn');
            var btn_id = btn.assignId();
            var clone = btn.clone();
            $('#buttonContainer').html(clone);
            $('#buttonId').val(btn_id);
            $('#buttonLabel').val(btn.text());
            $('#buttonHref').val(btn.attr('href'));
            $('#buttons').show();

            confirm.bind('click', function (e) {
                e.preventDefault();
                saveRowSettings(row);
                saveColumnSettings(column);
                btn.text($('#buttonLabel').val());
                btn.attr('href', $('#buttonHref').val());
                btn.css('background', $('#colbtn').val());
                btn.css('width', $('#custombtnwidth').val());
                btn.css('height', $('#custombtnheight').val());
                btn.css('font-size', $('#custombtnfont').val());
                btn.css('padding-top', $('#custombtnpaddingtop').val());
                btn.css('color', $('#colbtncol').val());
                //btn.css('align', $('#btnalign').val());

                o.attr('id', $('#id').val());
                btn.attr('class', $('#buttonContainer > a.btn').attr('class'));
                o.attr('class', $('#class').val());
                contentCopy();
            });


            break;
    }
    contentCopy();
    $('#preferences').modal('show').draggable();
}

$(document).on('focusin', function (e) {
    if ($(event.target).closest(".mce-window").length) {
        e.stopImmediatePropagation();
    }
});



function handleSaveLayout() {
    var e = $(".TurantoPage").html();
    if (e != window.TurantoPageHtml) {
        saveLayout();
        window.TurantoPageHtml = e
    }
}

function gridSystemGenerator() {
    $(".TurantoRow .preview input").bind("keyup", function () {
        var e = 0;
        var t = "";
        var n = false;
        var r = $(this).val().split(" ", 12);
        $.each(r, function (r, i) {
            if (!n) {
                if (parseInt(i) <= 0)
                    n = true;
                e = e + parseInt(i);
                t += '<div class="col-md-' + i + ' column"></div>'
            }
        });
        if (e == 12 && !n) {
            $(this).parent().next().children().html(t);
            $(this).parent().find('.drag').show()
        } else {
            $(this).parent().find('.drag').hide()
        }
    })
}
function removeElm() {
    $(".TurantoPage").delegate(".remove", "click", function (e) {
        var b = $(this).parent().css('border');
        $(this).parent().css('border', '2px solid red');

        if (confirm("Are u sure u want to delete?")) {
            e.preventDefault();
            $(this).parent().remove();

            if (!$(".TurantoPage .TurantoRow").length > 0) {
                clearDemo();
            }
        } else {
            $(this).parent().css('border', b);
        }
        contentCopy();
    })
}

function getIndent(level) {
    var result = '',
            i = level * 4;
    if (level < 0) {
        throw "Level is below 0";
    }
    while (i--) {
        result += ' ';
    }
    return result;
}

function style_html(html) {
    html = html.trim();
    var result = '',
            indentLevel = 0,
            tokens = html.split(/</);
    for (var i = 0, l = tokens.length; i < l; i++) {
        var parts = tokens[i].split(/>/);
        if (parts.length === 2) {
            if (tokens[i][0] === '/') {
                indentLevel--;
            }
            result += getIndent(indentLevel);
            if (tokens[i][0] !== '/') {
                indentLevel++;
            }

            if (i > 0) {
                result += '<';
            }

            result += parts[0].trim() + ">\n";
            if (parts[1].trim() !== '') {
                result += getIndent(indentLevel) + parts[1].trim().replace(/\s+/g, ' ') + "\n";
            }

            if (parts[0].match(/^(img|hr|br)/)) {
                indentLevel--;
            }
        } else {
            result += getIndent(indentLevel) + parts[0] + "\n";
        }
    }
    return result;
}

function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
}

function gup(name, url) {
    if (!url)
        url = location.href;
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(url);
    return results == null ? null : results[1];
}

(function ($) {
    $.fn.assignId = function () {
        var _self = $(this);
        var id = _self.attr('id');
        if (typeof id === typeof undefined || id === false) {
            id = s4() + '-' + s4() + '-' + s4() + '-' + s4();
            _self.attr('id', id);
        }
        return id;
    };
})(jQuery);

function contentCopy() {
    $("#download-layout").children().html($(".TurantoPage").html());
    var t = $("#download-layout").children();
    $("#PageContent").val(style_html($("#download-layout").html()));
}