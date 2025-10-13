
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="VrArrays.js" />
///<reference path="VRDataFetcher.js" />
///<reference path="VrDate.js" />

if (!String.prototype.htmlEscape) {

    // Returns true if the element has the attribute name provided.
    $.fn.hasAttr = function (name) {
        return this.attr(name) !== undefined;
    };

    // Allows to put html special characters into a text field without breaking the DOM.
    // Usage: "Bob said "Will you be my friend?"."
    // Result: Bob said /%39;Will you be my friend?/%39;."
    String.prototype.htmlEscape = function () {
        return this.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/'/g, '&#39;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    };
    String.prototype.htmlUnescape = function () {
        return this.replace(/&quot;/g, '"').replace(/&#39;/g, "'").replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&amp;/g, '&');
    };
    // Escapes a string for use by a regular expression.    
    String.prototype.regexEscape = function () {
        return String(this).replace(/([-()\[\]{}+?*.$\^|,:#<!\\])/g, '\\$1').replace(/\x08/g, '\\x08'); // escape as needed
    };
    String.prototype.splitCSV = function (sep) {
        for (var foo = this.split(sep = sep || ","), x = foo.length - 1, tl; x >= 0; x--) {
            if (foo[x].replace(/"\s+$/, '"').charAt(foo[x].length - 1) == '"') {
                if ((tl = foo[x].replace(/^\s+"/, '"')).length > 1 && tl.charAt(0) == '"') {
                    foo[x] = foo[x].replace(/^\s*"|"\s*$/g, '').replace(/""/g, '"');
                } else if (x) {
                    foo.splice(x - 1, 2, [foo[x - 1], foo[x]].join(sep));
                } else foo = foo.shift().split(sep).concat(foo);
            } else foo[x].replace(/""/g, '"');
        } return foo;
    };
    String.prototype.trim = function () { return this.replace(/^\s+|\s+$/g, ''); };        
    String.prototype.toFloat = function () {
        var retValue = 0.0;
        var str = ifm.vr.stringFormating.asNumberNoCommas(this);
        if (str !== null) {
            if (str.length > 0) {
                if (!isNaN(str)) {
                    retValue = parseFloat(str);
                }
            }
        }
        return retValue;
    };
    String.prototype.toInt = function () {
        var retValue = 0;
        var str = ifm.vr.stringFormating.asNumberNoCommas(this);
        if (str !== null) {
            if (str.length > 0) {
                if (!isNaN(str)) {
                    retValue = parseInt(str);
                }
            }
        }
        return retValue;
    };
    String.prototype.isEmptyOrWhiteSpace = function () {
        
        return !(this.trim().length > 0);
    };

    // Returns a string where the first letter of each word is capitalized.
    // Usage: 'my name is tom.'
    // Result: 'My Name Is Tom.'
    String.prototype.capitalize = function () { return String(this).replace(/\b[a-z]/g, function (match) { return match.toUpperCase(); }); };    

    // Returns the provided string substring(ed) if it is beyond the provided maxLength.
    // Usage: "My name is Matt.".toMaxLength(4);
    // Result: "My n"
    String.prototype.toMaxLength = function (maxLength) {
        var formatted = this.toString();
        if (formatted.length > maxLength) {
            formatted = formatted.substring(0, maxLength)
        }
        return formatted;
    };

    // Returns the provided string substring(ed) if it is beyond the provided maxLength.
    // Usage: "My name is Matt.".toMaxLength(4);
    // Result: "M..."
    String.prototype.ellipsis = function (maxLength) {
        var formatted = this.toString();
        if (formatted.length > maxLength)
            formatted = formatted.toMaxLength(maxLength - 3) + '...';  
        return formatted;
    };

    // Does a simple find and replace. This will auto escape Regex special chars.
    // Usage: "123-11-1234.replaceAll('-','');
    // Result: "123111234"
    String.prototype.replaceAll = function (find, replace) {        
        return this.replace(new RegExp(find.regexEscape(), 'g'), replace);
    };

    // This will remove all instances of the character in the string. This will auto escape Regex special chars.
    // Usage: "123-11-1234.removeAll('-');
    // Result: "123111234"
    String.prototype.removeAll = function (find) {
        return this.replaceAll(find, '');
    };

    // Returns true if the string ends with the provided string.
    // Usage: var hasDotCom = 'the url is www.site.com'.endsWith('.com');
    // Result: true
    String.prototype.endsWith = function (suffix) {
        return this.match(suffix + "$") == suffix;
    };


    

}




// Think of all these items as static/shared/module type structures. Do not attempt to create anything that contains mutable state in here.
var ifm = new function () {
    this.vr = new function () {
        this.workflow = new function () {
            this.ppa_Input = '';
            this.ppa_App = '';
            this.hom_Input = '';
            this.hom_App = '';
            this.far_Input = '';
            this.far_App = '';
            this.dfr_Input = '';
            this.dfr_App = '';

            this.GetQuoteSideUrl = function (lobId)
            {
                switch(lobId)
                {
                    case "1":
                        return this.ppa_Input;
                        break;
                    case "2":
                        return this.hom_Input;
                        break;
                    case "3":
                        return this.dfr_Input;
                        break;
                    case "17":
                        return this.far_Input;
                        break;
                    default:
                        throw "This lob is not supported."
                }
                
            }; // GetQuoteSideUrl END

            this.GetAppSideUrl = function (lobId) {
                switch (lobId) {
                    case "1":
                        return this.ppa_App;
                        break;
                    case "2":
                        return this.hom_App;
                        break;
                    case "3":
                        return this.dfr_App;
                        break;
                    case "17":
                        return this.far_App;
                        break;
                    default:
                        throw "This lob is not supported."
                }
            }; // GetAppSideUrl END


        }; // workflow END
        this.currentQuote = new function () {
            this.agencyId = '';
            this.lobId = '';
            this.quoteID = '';
            this.effectiveDate = '';
            this.treeviewEffectiveDate = function () {
                if ($('[id*=hdnOriginalEffectiveDate]').val()) {
                    return $('[id*=hdnOriginalEffectiveDate]').val();
                } else {
                    return ifm.vr.currentQuote.effectiveDate;
                }
            };
            this.workflowPosition = '';
        };
        this.arrays = ifm_Array;
        this.numbers = new function () {
            //Rounds the number up
            //Usage: RoundUpBy100('1225');
            //Result: '1300'
            this.RoundUpBy100 = function (numAsString) {
                var numberInt = ifm.vr.stringFormating.asNumberNoCommas(numAsString).toString().toInt();
                var returnNum = 0;
                if (ifm.vr.currentQuote.isEndorsement) {
                    // Endorsement values get rounded to the nearest (5) 1/26/22 MGB Bug 72365
                    var returnNum = 0;

                    if (numberInt < 0) {
                        numberInt = numberInt * -1;
                        returnNum = Math.ceil(numberInt / 5) * 5;

                        returnNum = returnNum * -1;
                    }
                    else {
                        returnNum = Math.ceil(numberInt / 5) * 5;
                    }

                    return returnNum;
                    //returnNum = numberInt;
                }
                else {

                    if (numberInt < 0) {
                        numberInt = numberInt * -1;
                        returnNum = Math.ceil(numberInt / 100) * 100;

                        returnNum = returnNum * -1;
                    }
                    else {
                        returnNum = Math.ceil(numberInt / 100) * 100;
                    }
                    
                }
                return returnNum.toString();
            };
            this.RoundUpBy100But0isBlank = function (numAsString) {
                var numberInt = ifm.vr.stringFormating.asNumberNoCommas(numAsString).toString().toInt();
                var returnNum = 0;

                if (ifm.vr.currentQuote.isEndorsement) {
                    // Endorsement values get rounded to the nearest (5) 1/26/22 MGB Bug 72365
                    var returnNum = 0;

                    if (numberInt < 0) {
                        numberInt = numberInt * -1;
                        returnNum = Math.ceil(numberInt / 5) * 5;

                        returnNum = returnNum * -1;
                    }
                    else {
                        returnNum = Math.ceil(numberInt / 5) * 5;
                    }

                    return returnNum;
                    //returnNum = numberInt;
                }
                else {

                    if (numberInt < 0) {
                        numberInt = numberInt * -1;
                        returnNum = Math.ceil(numberInt / 100) * 100;

                        returnNum = returnNum * -1;
                    }
                    else {
                        returnNum = Math.ceil(numberInt / 100) * 100;
                    }


                }
                if (returnNum === 0) {
                    returnNum = '';
                }

                return returnNum.toString();
            };                      

        };//numbers END
        this.vrdata = VRData;
        this.stringFormating = new function () {
            //Formats a date string to MM/DD/YYYY.
            //Usage: '12-25-2015'
            //Result: '12/25/2015'
            this.asDate = function (val) {
                val = val.replaceAll("-", "");
                return val.replace(/^([\d]{2})([\d]{2})([\d]{4})$/, "$1/$2/$3");
            };

            // Removes everything except numeric digits then formats to ##### or #####-#### depending on length.
            this.asPostalCode = function (val) {                
                    var pcode = this.asPositiveNumberNoCommas(val);
                if (pcode.length > 6) {
                    if (pcode[6] == '-' || pcode[6] == ' ') {
                        // look for both because it either then you are done
                        pcode[6] = '-';
                    }
                    else {
                        if (pcode.length >= 10) {
                            // it doesn't have a - or a space so it has to many digits
                            pcode = pcode;
                        }
                        else {
                            var left = pcode.substring(0, 5);
                            var right = pcode.substring(5, (pcode.length > 9) ? 9 : pcode.length);
                            pcode = left + '-' + right;
                        }
                    }
                }

                return pcode;
            };

            // Removes spaces, alphabetic, currency, percent, and pound signs then formats with commas if needed.
            // Usage: asNumberWithCommas(' $2500.50 ');
            // Result: '2,500.50'
            this.asNumberWithCommas = function (nStr) {
                var returnNum = ''
                if (nStr) {
                    nStr = this.asNumberNoCommas(nStr);
                    x = nStr.split('.');
                    x1 = x[0]; // is left of the period
                    x2 = x.length > 1 ? '.' + x[1] : ''; // is anything right of the period
                    var rgx = /(\d+)(\d{3})/;
                    while (rgx.test(x1)) {
                        x1 = x1.replace(rgx, '$1' + ',' + '$2');
                    }
                    returnNum = x1 + x2;
                }
                return returnNum;
            };

            // Removes spaces, alphabetic, currency, percent, and pound signs then formats without commas.
            // Usage: asNumberNoCommas(' $2500.50 ');
            // Result: '2500.50'
            this.asNumberNoCommas = function (nStr) {
                var returnNum = '';
                nStr = nStr.toString();  // MUST be a string for this function to work!
                if (nStr) {
                    Exclude = " ,[A-Za-z],$,%,#";
                    nStr = nStr.replace(/,/gi, "");
                    var removes = new Array();
                    removes = Exclude.split(","); //split unwanted list
                    var l = nStr.length; //1-1-15
                    for (var bb = 0; bb < l; bb++) //1-1-15
                    {
                        for (var i = 0; i < removes.length; i++) {
                            // '$' needs an escape \
                            if (removes[i] == "$")
                            { nStr = nStr.replace(/\$/g, ""); } // remove the unwanted char
                            else {
                                var regExp = new RegExp(removes[i]); // create dynamic regex
                                nStr = nStr.replace(regExp, ""); // remove the unwanted char
                            }
                        }
                    }
                    returnNum = nStr;
                }
                return returnNum;
            };

            // Removes spaces, alphabetic, currency, percent, pound, and negative signs then formats without commas.
            // Usage: asPositiveNumberNoCommas(' -$2500.50 ');
            // Result: '2500.50'
            this.asPositiveNumberNoCommas = function (nStr) {
                Exclude = " ,[A-Za-z],$,-,#,%";
                nStr = nStr.replace(/,/gi, "");
                var removes = new Array();
                removes = Exclude.split(","); //split unwanted list
                var l = nStr.length; //1-1-15
                for (var bb = 0; bb < l; bb++) //1-1-15
                {
                    for (var i = 0; i < removes.length; i++) {
                        // '$' needs an escape \
                        if (removes[i] == "$")
                        { nStr = nStr.replace(/\$/g, ""); } // remove the unwanted char
                        else {
                            var regExp = new RegExp(removes[i]); // create dynamic regex
                            nStr = nStr.replace(regExp, ""); // remove the unwanted char
                        }
                    }
                }
                return nStr;
            };

            // Removes spaces, alphabetic, currency, percent, pound, and negative signs then formats with commas.
            // Usage: asPositiveNumberNoCommas(' -$2500.50 ');
            // Result: '2,500.50'
            this.asPositiveNumberWithCommas = function (nStr) {
                return this.asNumberWithCommas(this.asPositiveNumberNoCommas(nStr));
            };

            // Removes spaces, alphabetic, currency, percent, pound, and negative signs then formats with commas.
            // Usage: asPositiveNumberNoCommas(' -$2500.50 ');
            // Result: '2,500'
            this.asPositiveWholeNumberWithCommas = function (nStr) {
                var value = this.asNumberWithCommas(this.asPositiveNumberNoCommas(nStr));
                if (value.indexOf('.') >= 0)
                {
                    return value.split(".")[0]
                }
                return value;
            };


            // Removes spaces, alphabetic, currency, percent, pound, and negative signs then rounds to nearest 100
            // Usage: asRoundToNearest100(' -$2532.50 ');
            // Result: '2500'
            // Added 12/28/17 for HOM Upgrade MLW
            this.asRoundToNearest100 = function (nStr) {
                var value = this.asPositiveNumberNoCommas(nStr);
                if (value > 0)
                {
                    value = Math.round(value / 100) * 100;
                }
                return value;
            };

            // Removes spaces, alphabetic, currency, percent, pound, and negative signs then rounds to nearest 100
            // Usage: asRoundToNearest1000(' -$2832.50 ');
            // Result: '3000'
            // Added 12/28/17 for HOM Upgrade MLW
            this.asRoundToNearest1000 = function (nStr) {
                var value = this.asPositiveNumberNoCommas(nStr);
                if (value > 0) {
                    value = Math.round(value / 1000) * 1000;
                }
                return value;
            };

            // Round a number up to next whole number used for both NB and Endorsement
            // Usage: asRoundToNextNumber(' -$11,123 ');
            // Result: '12,000'
            // Added 10/18/22 for task 68166 BD
            this.asRoundToNextNumber100 = function (nStr) {
                var value = this.asPositiveNumberNoCommas(nStr);
                if (value > 0) {
                    value = Math.ceil(value / 100) * 100;
                }
                return value;
            };

            // Removes spaces, alphabetic, currency, percent, and pound signs then formats with commas.
            // Usage: asCurrency(' 2500.5 ');
            // Result: '$2,500.50'
            this.asCurrency = function (nStr) {
                var formatted = '$' + this.asNumberWithCommas(nStr);
                if (formatted != '$') {
                    x = formatted.split('.');
                    x1 = x[0]; // is left of the period
                    x2 = x.length > 1 ? '.' + x[1] : ''; // is anything right of the period
                    var allowedDigits = 2 + 1; // add one for period
                    if (x2.length > allowedDigits) {
                        x2 = x2.substring(0, allowedDigits);
                    }
                    if (x2.length == 2)
                        x2 = x2 + "0";
                    return x1 + x2;
                }
                return "";
            };

            this.asAlphabeticOnly = function (nStr) {
                return nStr.replace(/[^a-z' ']/gi, '')
            };

            this.asAlphabeticNumeric = function (nStr) {
                return nStr.replace(/[^a-z0-9' ']/gi, '')                
            };

            // Removes spaces, alphabetic, currency, percent, and pound signs then formats with commas.
            // Usage: asCurrencyNoCents(' 2500.5 ');
            // Result: '$2,500'
            this.asCurrencyNoCents = function(nStr) {
                try {
                    var formatted = '$' + this.asNumberWithCommas(nStr);
                    if (formatted != '$') {
                        x = formatted.split('.');
                        x1 = x[0]; // is left of the period
                        return x1;
                    }
                }
                catch (err)
                { }
                return "";
            };

            // Returns the provided string substring(ed) if it is beyond the provided maxLength.
            // Usage: "My name is Matt.".toMaxLength(4);
            // Result: "My n"
            // Just a wrapper for the prototype of the function.
            this.toMaxLength = function (nStr, maxLength) { return nStr.toMaxLength(maxLength); };

        }; // stringFormating END
        this.vrDateTime = VrDateTime;
        this.theming = new function () {
            this.themes = {
                NewBusinessQuote: 'NewBusinessQuote',
                Endorsement: 'Endorsement',
                ReadOnly: 'ReadOnly',
                PayplanChange: 'PayplanChange',
                SearchResults: 'SearchResults'
            }
            function RemoveThemes() {
                $("link[id^='Theme']").remove();
            };

            //this.ChangeTheme = function(themeLinkTag) {
            //    RemoveThemes();
            //    $('head').append(themeLinkTag);
            //    $.cookie('VR_theme', themeLinkTag, { expires: 7000 });
            //};
            //updated 4/2/2019
            this.ChangeTheme = function (themeLinkTag, themeType) {
                RemoveThemes();
                $('head').append(themeLinkTag);
                $.cookie(GetCookieThemeName(themeType), themeLinkTag, { expires: 7000 });
            };

            //this.LoadThemeFromCookie = function() {
            //    try {
            //        if ($.cookie('VR_theme') != undefined) {
            //            RemoveThemes();
            //            $('head').append($.cookie('VR_theme'));
            //        }
            //        else {
            //            RemoveThemes();
            //            $('head').append(ThemeOrange);
            //        }
            //    }
            //    catch (err) {
            //        RemoveThemes();
            //        $('head').append(ThemeOrange);
            //    }
            //};
            //updated 4/2/2019
            this.LoadThemeFromCookie = function (themeType) {
                try {
                    if ($.cookie(GetCookieThemeName(themeType)) != undefined) {
                        RemoveThemes();
                        $('head').append($.cookie(GetCookieThemeName(themeType)));
                    }
                    else {
                        RemoveThemes();
                        $('head').append(DefaultTheme(themeType));
                    }
                }
                catch (err) {
                    RemoveThemes();
                    $('head').append(DefaultTheme(themeType));
                }
            };


            //added 4/2/2019
            function GetCookieThemeName(themeType) {
                if (typeof (themeType) == 'undefined') {
                    themeType = '';
                }
                var cookieThemeName = 'VR_theme';
                if (themeType) {
                    if (themeType.length > 0) {
                        cookieThemeName = cookieThemeName + '_' + themeType;
                    }
                }
                return cookieThemeName;
            }
            function DefaultTheme(themeType) { // New Theme Names CAH
                if (typeof (themeType) == 'undefined') {
                    themeType = '';
                }
                if (themeType) {
                    if (themeType.length > 0) {
                        switch (themeType) {
                            case 'NewBusinessQuote':
                                return ThemeOrange;
                                break;
                            case 'Endorsement':
                                return ThemeEndorsement;
                                break;
                            case 'ReadOnly':
                                return ThemeReadOnly;
                                break;
                            case 'PayplanChange':
                                return ThemePayplanChange;
                                break;
                            case 'SearchResults':
                                return ThemeSearchResults;
                                break;
                            default:
                                return ThemeOrange;
                                break;
                        }
                    }
                }
                return ThemeOrange;
            }

        }; //END Theming
        this.ui = new function () {

            // Returns true if the provided keycode should initiate a format of the element.
            this.AllowFormatter = function (e) {
                return e.keyCode != 37 && e.keyCode != 91 && e.keyCode != 20 && e.keyCode != 32 && e.keyCode != 39 && e.keyCode != 17 && e.keyCode != 16 && e.keyCode != 46 && e.keyCode != 35 && e.keyCode != 36 && e.keyCode != 8 && e.keyCode != 9;
            };

            // Returns what ever element currently has focus.
            this.GetCurrentFocusedElementId = function () {
                var focused = document.activeElement;
                if (!focused || focused == document.body)
                    focused = null;
                else if (document.querySelector)
                    focused = document.querySelector(":focus");
                return $(focused).attr('id');
            };

            // This scrolls the element into view, gives it focus, then flashes the elements background from Red back to its original color.
            this.FlashFocusThenScrollToElement = function (elementID) {
                $("#" + elementID).scrollTop();
                $("#" + elementID).focus();
                var originalBackgroundColor = $("#" + elementID).css("backgroundColor")
                $("#" + elementID).css("backgroundColor", "red").stop().animate({ backgroundColor: originalBackgroundColor }, 1200);
            };

            this.FlashElement = function (jquerySelector) {                
                var originalBackgroundColor = $(jquerySelector).first().css("backgroundColor");
                $(jquerySelector).first().css("backgroundColor", "red").stop().animate({ backgroundColor: originalBackgroundColor }, 1200);
            };

            this.ScrollToWithOffset = function (elementId, offset) {
                if (Number.isInteger(offset) == false) {
                    offset = 0;
                };
                var position = $("#" + elementId).offset().top;
                var test = position + offset;
                $(window).scrollTop(position + offset);
            };

            this.DisableInputAreaAndTree = function() {
                EditModeaDiv("page", true);
            };

            // Re-enables the input area and tree if it was disabled with DisableInputAreaAndTree()
            this.EnableInputAreaAndTree = function() {
                EditModeaDiv("page", false);
            };

            // Disables the entire page. Usually used on button clicks that post back.
            this.DisableEntirePage = function() {
                EditModeaDiv("main", true);
            }

            // Re-enables the entire page if it was disabled via DisableEntirePage(). Caution: Usually once it is disabled you won't re-enable.
            this.EnableEntirePage = function () {
                EditModeaDiv("main", false);
            }

            // Private. Use the wrappers above to interact with this function.
            function EditModeaDiv(divId, apply) {
                var $somediv = $('#' + divId),
                pos = $.extend({
                    width: $somediv.outerWidth(),
                    height: $somediv.outerHeight()
                }, $somediv.position());

                if (apply) {
                    // if overlay does not exist already
                    if ($("#" + divId + "_overlay").length == 0) {
                        // create new div over tree to stop button clicks
                        $('<div>', {
                            id: divId + "_overlay",
                            title: 'Disabled until the current activity is completed.',
                            css: {
                                position: 'absolute',
                                top: pos.top,
                                left: pos.left,
                                width: pos.width,
                                height: pos.height,
                                backgroundColor: '#000'
                                , opacity: 0.0
                            }
                        }).appendTo($somediv);
                    }
                    $("#" + divId).addClass('disabledTree');
                }
                else {
                    $("#" + divId + "_overlay").remove();
                    $("#" + divId).removeClass('disabledTree');
                }
            }

            // Sets the zero based index of the active item.
            this.SetActiveIndexOfAccordion = function (accordionId, index) {
                $("#" + accordionId).accordion("option", "active", index);
            }; 

            // Returns the zero based index of the currently active item.
            this.GetActiveIndexOfAccordion = function (accordionId) {
                return $("#" + accordionId).accordion("option", "active");
            };

            // Cancels the propagation of a fired event.
            this.StopEventPropagation = function(e) {
                if (!e)
                    e = window.event;

                //IE9 & Other Browsers
                if (e.stopPropagation) {
                    e.stopPropagation();
                }
                    //IE8 and Lower
                else {
                    e.cancelBubble = true;
                }
            }

            // Only used by the master base page.
            this.InitDirtyForms = function(clientId) {
                var dirtyOptions = {
                    trimText: true, fieldChangeCallback: function (originalValue, isDirty) {
                        if (isDirty) {
                            ifm.vr.ui.LockTree();                            
                        }
                        else {
                            ifm.vr.ui.UnLockTree();
                        }
                    },
                    dirtyFieldsClass: "alteredField"
                };

                $("#" + clientId).dirtyFields(dirtyOptions);
            }

            this.LockTree = function () {
                // Bug 25181 - Requested more consistancy
                //ToggleEditMode(true);
                // this.LockTree_Freeze();
                
                //Allow individual page overrides from workflow see PPA/ctl_Master_Edit.ascx.vb
                if (DirtyFormException === undefined) {
                    var DirtyFormException = false
                }
                if (DirtyFormException === false) {
                    this.LockTree_Freeze();
                }
                
            };

            // Locks the tree and doesn't allow it to be unlocked until the next postback.
            this.LockTree_Freeze = function()
            {
                ToggleEditMode(true, true);
            };

            this.UnLockTree = function () { ToggleEditMode(false); };

            this.ScrollToWindowTop = function () {
                $(document).ready(function () {
                    $(window).load(function () {
                        $(window).scrollTop(0);
                    });
                });
            };

            // ifm.vr.ui.DisableContent(ArrayOfElementsByStrings)
            // Disables all elements within given containers.
            // var List = ['divInsuredInfo', 'MultiStateSection', 'divActionButtons'];
            // List.push('divMain') -- optional
            this.DisableContent = function (disableList) {

                $(document).ready(function () {
                    if (IsReadOnlyTransaction()) {
                        ifm.vr.ui.ForceDisableContent(disableList);
                    }
                });
            }

            //added 1/5/2021 (Interoperability) - breaking out logic from original DisableContent so that it can be called for cases other than IsReadOnlyTransaction
            this.ForceDisableContent = function (disableList) {

                // Exceptions - String containing html selector to an element
                var ExceptionList = [
                    'h3 a[id*="lnkViewSelection"]',
                    'td a[id*="lnkAddAdress"]',
                    'div a[id*="lnkView"]',
                    'a[id*="lnkViewEdit"]',
                    'a[id*="ctlFarmPersonalProperty_lnkMoreLess"]',
                    'table[id*="FarmPPNavButtons"]'
                ];


                $(document).ready(function () {

                    ifm.vr.ui.ContentDisabler(disableList, ExceptionList)
                    //if (disableList) {
                    //    disableList.forEach(function (element) {
                    //        //$('div[id*="' + element + '"] *').prop('disabled', true);
                    //        //updated 1/24/2020

                    //        $('div[id*="' + element + '"] *').children().each(function () {
                    //            var testElement = $(this);
                    //            ExceptionList.forEach(function (exElement) {
                    //                var test = $(exElement);
                    //                if ($(exElement).is(testElement[0])) {
                    //                    testElement.css('pointer-events', 'auto');
                    //                    testElement.parent().removeClass('ReadOnlyViewDisabledLook');
                    //                    return;
                    //                }
                    //                else {
                    //                    testElement.prop('readonly', true);
                    //                    testElement.focus(function () {
                    //                        $(this).blur();
                    //                    });
                    //                    testElement.addClass('ReadOnlyViewDisabledLook');
                    //                }
                    //            });
                    //        });

                    //        //$('div[id*="' + element + '"] *').prop('readonly', true);
                    //        //$('div[id*="' + element + '"] *').focus(function () {
                    //        //    $(this).blur();
                    //        //});
                    //        //$('div[id*="' + element + '"] *').addClass('ReadOnlyViewDisabledLook');
                    //    });
                    //}

                    $('h3 a').hide();


                    //$('h3 a[id*="lnkViewSelection"]').show().css('pointer-events', '');
                    //$('td a[id*="lnkAddAdress"]').prop('disabled', false).css('pointer-events', '');
                    //$('div a[id*="lnkView"]').prop('disabled', false).css('pointer-events', '');
                    //$('a[id*="lnkViewEdit"]').prop('disabled', false).css('pointer-events', '');
                    //$('a[id*="ctlFarmPersonalProperty_lnkMoreLess"]').removeClass('ReadOnlyViewDisabledLook').parent().removeClass('ReadOnlyViewDisabledLook');
                    //$('a[id*="ctlFarmPersonalProperty_lnkMoreLess"]').removeClass('ReadOnlyViewDisabledLook').css('pointer-events', 'auto').unbind("focus");
                });
            };

            // Used to disable the contents of an string array of element ID's
            // Exceptions will not be disabled
            // The ExceptionList is an array of jquery selectors (as strings) of things to ignore and it is optional 
            // Example Exception selector: 'h3 a[id*="lnkViewSelection"]'
            this.ContentDisabler = function (disableList, ExceptionList) {
                $(document).ready(function () {
                    if (disableList) {
                        disableList.forEach(function (element) {
                            $('[id*="' + element + '"] *').children().each(function () {
                                var testElement = $(this);
                                if (typeof ExceptionList !== 'undefined' && ExceptionList.length > 0) {
                                    ExceptionList.forEach(function (exElement) {
                                        var test = $(exElement);
                                        if ($(exElement).is(testElement[0])) {
                                            testElement.css('pointer-events', 'auto');
                                            testElement.parent().removeClass('ReadOnlyViewDisabledLook');
                                            return;
                                        }
                                        else {
                                            ifm.vr.ui.ElementDisabler(testElement)
                                        }
                                    });
                                }
                                else {
                                    ifm.vr.ui.ElementDisabler(testElement)
                                };
                                
                            });
                        });
                    };
                });

            };

            this.ContentEnabler = function (enableList) {
                $(document).ready(function () {
                    if (enableList) {
                        enableList.forEach(function (element) {
                            $('[id*="' + element + '"] *').children().each(function () {
                                ifm.vr.ui.ElementEnabler($(this))
                            });
                        });
                    };
                });

            };

            // Used to disable a Single Element
            this.ElementDisabler = function (Element) {
                Element.prop('readonly', true);
                Element.focus(function () {
                    $(this).blur();
                });
                Element.addClass('ReadOnlyViewDisabledLook');
                if (Element.is('a')) {
                    if (Element.parent().parent().is('h3')) {
                        Element.hide();
                    }
                }
                if (Element.is('select')) {
                    Element.on('mousedown selectstart', function (e) {
                        e.preventDefault();
                    });
                }
            }

            this.ElementEnabler = function (Element) {
                Element.prop('readonly', false);
                Element.off("focus");
                Element.removeClass('ReadOnlyViewDisabledLook');
                Element.css('pointer-events', 'auto');
                if (Element.is('a')) {
                    if (Element.parent().parent().is('h3')) {
                        Element.show();
                    }
                }
                if (Element.is('select')) {
                    Element.off('mousedown selectstart');
                }
            }

            // Used to disable content in a single container by Using that element's ID
            // The ExceptionList is an array of jquery selectors (as strings) of things to ignore and it is optional 
            // Example Exception selector: 'h3 a[id*="lnkViewSelection"]'
            this.SingleContainerContentDisable = function (Element, ExceptionList) {
                var disableList = [Element];
                ifm.vr.ui.ContentDisabler(disableList, ExceptionList);
            }

            //Used to Disable a Single element by it's Full or Partial ID
            //Will hide any <a> in an <h3>; Hides links in accord headers.
            this.SingleElementDisable = function (ID) {
                var Element = $('[id*="' + ID + '"]').filter(":first")
                ifm.vr.ui.ElementDisabler(Element);
            }

            //Used to ReEnable a single previously disabled element by it's Full or Partial ID
            this.SingleElementEnable = function (ID) {
                var Element = $('[id*="' + ID + '"]').filter(":first")
                ifm.vr.ui.ElementEnabler(Element);
            }

                        
        }; // this.UI END

    }; // END VR    
}; // END IFM

