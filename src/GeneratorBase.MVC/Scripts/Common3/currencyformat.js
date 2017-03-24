
$(document).ready(function ()
{
    $(".tabbable.responsive").responsiveTabs();

    $('.chosen-single').hover(function () {
        $(this).attr("title", $(this).text());
    });
    try {
        $("area[rel^='prettyPhoto']").prettyPhoto();
        $(".gallery:first a[rel^='prettyPhoto']").prettyPhoto({ animation_speed: 'normal', theme: 'light_square', slideshow: 3000, autoplay_slideshow: false });
        $(".gallery:gt(0) a[rel^='prettyPhoto']").prettyPhoto({ animation_speed: 'fast', slideshow: 10000, hideflash: true });
        $("#custom_content a[rel^='prettyPhoto']:first").prettyPhoto({
            custom_markup: '<div id="map_canvas" style="width:260px; height:265px"></div>',
            changepicturecallback: function () { initialize(); }
        });
        $("#custom_content a[rel^='prettyPhoto']:last").prettyPhoto({
            custom_markup: '<div id="bsap_1259344" class="bsarocks bsap_d49a0984d0f377271ccbf01a33f2b6d6"></div><div id="bsap_1237859" class="bsarocks bsap_d49a0984d0f377271ccbf01a33f2b6d6" style="height:260px"></div><div id="bsap_1251710" class="bsarocks bsap_d49a0984d0f377271ccbf01a33f2b6d6"></div>',
            changepicturecallback: function () { _bsap.exec(); }
        });
    } catch (ex) { }

    $('#divTurantoGrid').TurantoGrid({
        no_columns: 4,
        padding_x: 10,
        padding_y: 10,
        margin_bottom: 50,
        single_column_breakpoint: 700
    });

    loadGallery(true, 'a.thumbnail');
    //This function disables buttons when needed
    function disableButtons(counter_max, counter_current) {
        $('#show-previous-image, #show-next-image').show();
        if (counter_max == counter_current) {
            $('#show-next-image').hide();
        } else if (counter_current == 1) {
            $('#show-previous-image').hide();
        }
    }
    function loadGallery(setIDs, setClickAttr) {
        var current_image,
            selector,
            counter = 0;
        $('#show-next-image, #show-previous-image').click(function () {
            if ($(this).attr('id') == 'show-previous-image') {
                current_image--;
            } else {
                current_image++;
            }
            selector = $('[data-image-id="' + current_image + '"]');
            updateGallery(selector);
        });
        function updateGallery(selector) {
            var $sel = selector;
            current_image = $sel.data('image-id');
            $('#image-gallery-caption').text($sel.data('caption'));
            $('#image-gallery-title').text($sel.data('title'));
            $('#image-gallery-image').attr('src', $sel.data('image'));
            disableButtons(counter, $sel.data('image-id'));
        }
        if (setIDs == true) {
            $('[data-image-id]').each(function () {
                counter++;
                $(this).attr('data-image-id', counter);
            });
        }
        $(setClickAttr).on('click', function () {
            updateGallery($(this));
        });
    }


    $("span.input-group-addon.btn-default.calendar").next("input").each(function (i, obj) {
        $(this).on('keydown', function (e) {

            if (this.selectionStart || this.selectionStart == 0) {
                // selectionStart won't work in IE < 9
                var key = e.which;
                var prevDefault = true;

                var thouSep = ",";  // your seperator for thousands
                var deciSep = ".";  // your seperator for decimals
                var deciNumber = 2; // how many numbers after the comma

                var thouReg = new RegExp(thouSep, "g");
                var deciReg = new RegExp(deciSep, "g");

                function spaceCaretPos(val, cPos) {
                    /// get the right caret position without the spaces

                    if (cPos > 0 && val.substring((cPos - 1), cPos) == thouSep)
                        cPos = cPos - 1;

                    if (val.substring(0, cPos).indexOf(thouSep) >= 0) {
                        cPos = cPos - val.substring(0, cPos).match(thouReg).length;
                    }

                    return cPos;
                }

                function spaceFormat(val, pos) {
                    /// add spaces for thousands

                    if (val.indexOf(deciSep) >= 0) {
                        var comPos = val.indexOf(deciSep);
                        var int = val.substring(0, comPos);
                        var dec = val.substring(comPos);
                    } else {
                        var int = val;
                        var dec = "";
                    }
                    var ret = [val, pos];

                    if (int.length > 3) {

                        var newInt = "";
                        var spaceIndex = int.length;

                        while (spaceIndex > 3) {
                            spaceIndex = spaceIndex - 3;
                            newInt = thouSep + int.substring(spaceIndex, spaceIndex + 3) + newInt;
                            if (pos > spaceIndex) pos++;
                        }
                        ret = [int.substring(0, spaceIndex) + newInt + dec, pos];
                    }
                    return ret;
                }

                $(this).on('keyup', function (ev) {

                    if (ev.which == 8) {
                        // reformat the thousands after backspace keyup

                        var value = this.value;
                        var caretPos = this.selectionStart;

                        caretPos = spaceCaretPos(value, caretPos);
                        value = value.replace(thouReg, '');

                        var newValues = spaceFormat(value, caretPos);
                        this.value = newValues[0];
                        this.selectionStart = newValues[1];
                        this.selectionEnd = newValues[1];
                    }
                });

                if ((e.ctrlKey && (key == 65 || key == 67 || key == 86 || key == 88 || key == 89 || key == 90)) ||
                   (e.shiftKey && key == 9)) // You don't want to disable your shortcuts!
                    prevDefault = false;

                if ((key < 37 || key > 40) && key != 8 && key != 9 && key != 13 && key != 46 && prevDefault) {
                    e.preventDefault();

                    if (!e.altKey && !e.shiftKey && !e.ctrlKey) {

                        var value = this.value;
                        if ((key > 95 && key < 106) || (key > 47 && key < 58) ||
                          (deciNumber > 0 && (key == 110 || key == 188 || key == 190))) {

                            var keys = { // reformat the keyCode
                                48: 0, 49: 1, 50: 2, 51: 3, 52: 4, 53: 5, 54: 6, 55: 7, 56: 8, 57: 9,
                                96: 0, 97: 1, 98: 2, 99: 3, 100: 4, 101: 5, 102: 6, 103: 7, 104: 8, 105: 9,
                                110: deciSep, 188: deciSep, 190: deciSep
                            };

                            var caretPos = this.selectionStart;
                            var caretEnd = this.selectionEnd;

                            if (caretPos != caretEnd) // remove selected text
                                value = value.substring(0, caretPos) + value.substring(caretEnd);

                            caretPos = spaceCaretPos(value, caretPos);

                            value = value.replace(thouReg, '');

                            var before = value.substring(0, caretPos);
                            var after = value.substring(caretPos);
                            var newPos = caretPos + 1;

                            if (keys[key] == deciSep && value.indexOf(deciSep) >= 0) {
                                if (before.indexOf(deciSep) >= 0) { newPos--; }
                                before = before.replace(deciReg, '');
                                after = after.replace(deciReg, '');
                            }
                            var newValue = before + keys[key] + after;

                            if (newValue.substring(0, 1) == deciSep) {
                                newValue = "0" + newValue;
                                newPos++;
                            }

                            while (newValue.length > 1 &&
                              newValue.substring(0, 1) == "0" && newValue.substring(1, 2) != deciSep) {
                                newValue = newValue.substring(1);
                                newPos--;
                            }

                            if (newValue.indexOf(deciSep) >= 0) {
                                var newLength = newValue.indexOf(deciSep) + deciNumber + 1;
                                if (newValue.length > newLength) {
                                    newValue = newValue.substring(0, newLength);
                                }
                            }

                            var newValues = spaceFormat(newValue, newPos);

                            this.value = newValues[0];
                            this.selectionStart = newValues[1];
                            this.selectionEnd = newValues[1];
                        }
                    }
                }

                $(this).on('blur', function (e) {

                    if (deciNumber > 0) {
                        var value = this.value;

                        var noDec = "";
                        for (var i = 0; i < deciNumber; i++)
                            noDec += "0";

                        if (value == "0" + deciSep + noDec)
                            this.value = ""; //<-- put your default value here
                        else
                            if (value.length > 0) {
                                if (value.indexOf(deciSep) >= 0) {
                                    var newLength = value.indexOf(deciSep) + deciNumber + 1;
                                    if (value.length < newLength) {
                                        while (value.length < newLength) { value = value + "0"; }
                                        this.value = value.substring(0, newLength);
                                    }
                                }
                                else this.value = value + deciSep + noDec;
                            }
                    }
                });
            }
        });
    });
    $("span.viewcurrencylabel").digits();
    $("span.input-group-addon.btn-default.calendar").next("input").each(function (i, obj) {
        $(this).inputdigits();
    });
    $("input[type='submit']").click(function () {
        $("span.input-group-addon.btn-default.calendar").next("input").each(function (i, obj) {
            $(this).inputremove();
        });
    });
});

$.fn.inputremove = function () {
    return this.each(function () {
        $(this).val($(this).val().replace(/[^-\d\.]/g, ''));
    })
}
$.fn.inputdigits = function () {
    return this.each(function () {
        var amountSpn = $(this).val();
        if (amountSpn != '') {
            amountSpn = (Math.round(amountSpn * 100) / 100).toFixed(2);
        }
        $(this).val(amountSpn.toString().replace(/[,$]/g, '').replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,").replace(/(\.\d{1})$/, "$1" + "0"));
    })
}
$.fn.digits = function () {
    return this.each(function () {
        var amountSpn = $(this).text();
        if (amountSpn != '') {
            amountSpn = (Math.round(amountSpn * 100) / 100).toFixed(2);
        }
        $(this).text(amountSpn.toString().replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,").replace(/(\.\d{1})$/, "$1" + "0"));
    })
}

$('.closeall').click(function () {
    $('.panel-collapse.in')
      .collapse('hide');
});
$('.openall').click(function () {
    $('.panel-collapse:not(".in")')
      .collapse('show');
});
$('.collapse').on('shown.bs.collapse', function () {
    $(this).parent().find(".glyphicon-chevron-down").removeClass("glyphicon-chevron-down").addClass("glyphicon-chevron-up");
}).on('hidden.bs.collapse', function () {
    $(this).parent().find(".glyphicon-chevron-up").removeClass("glyphicon-chevron-up").addClass("glyphicon-chevron-down");
});

(function ($, window, document, undefined) {
    var pluginName = 'TurantoGrid',
        defaults = {
            padding_x: 10,
            padding_y: 10,
            no_columns: 3,
            margin_bottom: 50,
            single_column_breakpoint: 700
        },
        columns,
        $article,
        article_width;

    function Plugin(element, options) {
        this.element = element;
        this.options = $.extend({}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;
        this.init();
    }

    Plugin.prototype.init = function () {
        var self = this,
            resize_finish;

        $(window).resize(function () {
            clearTimeout(resize_finish);
            resize_finish = setTimeout(function () {
                self.make_layout_change(self);
            }, 11);
        });

        self.make_layout_change(self);

        setTimeout(function () {
            $(window).resize();
        }, 500);
    };

    Plugin.prototype.calculate = function (single_column_mode) {
        var self = this,
            tallest = 0,
            row = 0,
            $container = $(this.element),
            container_width = $container.width();
        $article = $(this.element).children();

        if (single_column_mode === true) {
            article_width = $container.width() - self.options.padding_x;
        } else {
            article_width = ($container.width() - self.options.padding_x * self.options.no_columns) / self.options.no_columns;
        }

        $article.each(function () {
            $(this).css('width', article_width);
        });

        columns = self.options.no_columns;

        $article.each(function (index) {
            var current_column,
                left_out = 0,
                top = 0,
                $this = $(this),
                prevAll = $this.prevAll(),
                tallest = 0;

            if (single_column_mode === false) {
                current_column = (index % columns);
            } else {
                current_column = 0;
            }

            for (var t = 0; t < columns; t++) {
                $this.removeClass('c' + t);
            }

            if (index % columns === 0) {
                row++;
            }

            $this.addClass('c' + current_column);
            $this.addClass('r' + row);

            prevAll.each(function (index) {
                if ($(this).hasClass('c' + current_column)) {
                    top += $(this).outerHeight() + self.options.padding_y;
                }
            });

            if (single_column_mode === true) {
                left_out = 0;
            } else {
                left_out = (index % columns) * (article_width + self.options.padding_x);
            }

            $this.css({
                'left': left_out,
                'top': top
            });
        });

        this.tallest($container);
        $(window).resize();
    };

    Plugin.prototype.tallest = function (_container) {
        var column_heights = [],
            largest = 0;

        for (var z = 0; z < columns; z++) {
            var temp_height = 0;
            _container.find('.c' + z).each(function () {
                temp_height += $(this).outerHeight();
            });
            column_heights[z] = temp_height;
        }

        largest = Math.max.apply(Math, column_heights);
        _container.css('height', largest + (this.options.padding_y + this.options.margin_bottom));
    };

    Plugin.prototype.make_layout_change = function (_self) {
        if ($(window).width() < _self.options.single_column_breakpoint) {
            _self.calculate(true);
        } else {
            _self.calculate(false);
        }
    };

    $.fn[pluginName] = function (options) {
        return this.each(function () {
            if (!$.data(this, 'plugin_' + pluginName)) {
                $.data(this, 'plugin_' + pluginName,
                new Plugin(this, options));
            }
        });
    }

})(jQuery, window, document);
