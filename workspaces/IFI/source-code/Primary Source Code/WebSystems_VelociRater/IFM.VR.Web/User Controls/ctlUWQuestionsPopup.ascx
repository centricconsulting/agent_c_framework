<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlUWQuestionsPopup.ascx.vb" Inherits="IFM.VR.Web.ctlUWQuestionsPopup" %>

<%@ Register Assembly="BasicFrame.WebControls.BasicDatePicker" Namespace="BasicFrame.WebControls" TagPrefix="BDP" %>
<%--<%@ Register Src="~/User Controls/ctrlEffectiveDate.ascx" TagPrefix="uc1" TagName="ctrlEffectiveDate" %>--%>






<script type="text/javascript">
    var datepickerIsCreated = false;
    var kqLobId = <%=Me.NewQuoteRequestLOBID%>;
    var isSubmitting = false;
    var popupDialogWidth = 550;
    var IsHOMLossHistoryKillQuestionAvailableValue = "False";
    var IsDFRLossHistoryKillQuestionAvailableValue = "False";
    var newRAPASymbolsEnabledValue = "False";

    //This used to be called in the InitKillQuestions function when setting up the dialog... I don't understand why... it was just creating more work for seemingly no gain and the Personal Auto was being forced to scroll
    //Keeping this logic here for now incase a valid reason pops up that requires this.
<%--    var UWQuestionsHeight = <% Select Case NewQuoteRequestLOBType
        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.HomePersonal
            Response.Write("450")
        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
            Response.Write("350")
        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
            Response.Write("560")
        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialBOP
            Response.Write("370")
        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
            Response.Write("365")
        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
            Response.Write("365")
        Case Else
            'Response.Write("height: 320,")
    End Select%>  --%> 


    function ShowHideFarmAdditionalSections() {
        if ($("#<%=rad_far_Dog14Yes.ClientID%>").is(':checked'))
        { $(".conditionalDogQuestion").show(); }
        else
        {
            $(".conditionalDogQuestion").hide();
        }

        if ($("#<%=rad_far_Dog14bYes.ClientID%>").is(":checked") && $("#<%=rad_far_Dog14Yes.ClientID%>").is(':checked')) {
            $("#<%=tr_far_Dog14bYes.ClientID%>").show();
        }
        else {
            $("#<%=tr_far_Dog14bYes.ClientID%>").hide();
        }
    }

     //Added 10/4/18 for multi state MLW
     function ShowHideAutoAdditionalSections() {
        var ef = $('#txtUWQuestionsEffectiveDate').val();
        if (new Date(ef).getTime() >= new Date("01/01/2019").getTime()) {
            if ($("#<%=radMultiPolicyYes.ClientID%>").is(':checked'))
            {
                $("#<%=tr_ppa_8MoreInfo.ClientID%>").show();
            }
            else
            {
            $("#<%=tr_ppa_8MoreInfo.ClientID%>").hide();
            }
        }
        else {
            $("#<%=tr_ppa_8MoreInfo.ClientID%>").hide();
        }        
    }

    function ShowHideBopAdditionalSections() {
            if ($("#<%=radBOPIncOccYes.ClientID%>").is(':checked')) {
                $("#<%=tr_BOP_IncidentalOccupancy.ClientID%>").show();
            }
            else
            {
            $("#<%=tr_BOP_IncidentalOccupancy.ClientID%>").hide();
            }
    }

    function ShowHideHOMAdditionalSections() {
        var ef = $('#txtUWQuestionsEffectiveDate').val();
        var date = <%= IFM.VR.Common.Helpers.HOM.HOMExistingAutoHelper.HOMExistingAutoEffDate().ToShortDateString() %>;
        if (new Date(ef).getTime() >= new Date(date).getTime()) {
            if ($("#<%=radHOMMultiPolicyYes.ClientID%>").is(':checked')) {
                $("#<%=tr_hom_MoreInfo.ClientID%>").show();
}
            else {
                $("#<%=tr_hom_MoreInfo.ClientID%>").hide();
            }
        }
        else {
            $("#<%=tr_hom_MoreInfo.ClientID%>").hide();
        } 
    }

    function ShowHideDFRAdditionalSections() {
        if ($("#<%=radDFRYes.ClientID%>").is(':checked')) {
            $("#<%=tr_dfr_MoreInfo.ClientID%>").show();
}
        else {
            $("#<%=tr_dfr_MoreInfo.ClientID%>").hide();
}
    }

    function ShowHideUmbrellaAdditionalSections() {
            if ($("#<%=UmbrellaTypeFarm.ClientID%>").is(':checked'))
            {
                $(".conditionalUmbrellaFarmTypeQuestion").show();
                $(".conditionalUmbrellaFarmSizeQuestion").show();
                $(".conditionalUmbrellaFarmSizeQuestionDetail").show();
                $(".NotingMinimumLimitTextTitleFarm").show();
                $(".NotingMinimumLimitTextTitlePersonal").hide();

            }
            else
            {
                $(".conditionalUmbrellaFarmTypeQuestion").hide();
                $(".conditionalUmbrellaFarmSizeQuestion").hide();
                $(".conditionalUmbrellaFarmSizeQuestionDetail").hide();
                $(".NotingMinimumLimitTextTitleFarm").hide();
                $(".NotingMinimumLimitTextTitlePersonal").show();

                $("#UmbrellaFarmTypePersonal").prop('checked', false);
                $("#UmbrellaFarmTypeCommercial").prop('checked', false);

                $("#UmbrellaFarmCommercialTypeIndividual").prop('checked', false);
                $("#UmbrellaFarmCommercialTypeFamilyPartnership").prop('checked', false);

                $("#UmbrellaFarmSizeSmall").prop('checked', false);
                $("#UmbrellaFarmSizeLarge").prop('checked', false);
            }
    }

    function InitKillQuestions() {
        if (DialogInitedKillQuestions == false) {
            DialogInitedKillQuestions = true;
            $("#<%=Me.divUwQuestions.ClientId%>").dialog({
                title: "Underwriting Questions",
                width: popupDialogWidth,              
                draggable: false,
                autoOpen: true,
                //modal: true,
                dialogClass: "no-close"
                //,open: function (type, data) { $(this).parent().appendTo("form"); }
            });
            //if ($.browser.msie)
            $("#<%=Me.divUwQuestions.ClientId%>").parent().appendTo(jQuery("form:first"));
        }
    }

    var DialogInitedKillQuestions = false;
    function ShowUwQuestions() {
        InitKillQuestions();        
        DisableFormOnSaveRemoves();
        
        //$("#<%=Me.divUwQuestions.ClientId%>").dialog("open");

        //added 11/7/17 for HOM Upgrade MLW - ddl in code behind pulls text from XML and not actual selected text, so need this hidden value to find if mobile in code behind
        if (kqLobId == IFMLOBEnum.HOM.LobId) {
            var selectedForm = $("#<%=Me.ddHomFormList.ClientID%> option:selected").text();
            $("#<%=Me.hiddenSelectedForm.ClientID%>").val(selectedForm);

            ShowHideHOMAdditionalSections();
            $("#<%=radHOMMultiPolicyYes.ClientID%>").change(function () { ShowHideHOMAdditionalSections(); });
            $("#<%=radHOMMultiPolicyNo.ClientID%>").change(function () { ShowHideHOMAdditionalSections(); });
        }

        if (kqLobId == IFMLOBEnum.DFR.LobId) {
            ShowHideDFRAdditionalSections();
            $("#<%=radDFRYes.ClientID%>").change(function () { ShowHideDFRAdditionalSections(); });
            $("#<%=radDFRNo.ClientID%>").change(function () { ShowHideDFRAdditionalSections(); });
        }

        
        ShowHideAdditionalInfo();
        

        if (kqLobId == IFMLOBEnum.FAR.LobId) { //farm            
            ShowHideFarmAdditionalSections();
            
            $("#<%=rad_far_Dog14Yes.ClientID%>").change(function () { ShowHideFarmAdditionalSections(); });
            $("#<%=rad_far_Dog14No.ClientID%>").change(function () { ShowHideFarmAdditionalSections(); });
            $("#<%=rad_far_Dog14bYes.ClientID%>").change(function () { ShowHideFarmAdditionalSections(); });
            $("#<%=rad_far_Dog14bNo.ClientID%>").change(function () { ShowHideFarmAdditionalSections(); });
        }

        if (kqLobId == IFMLOBEnum.BOP.LobId) { //bop   
            ShowHideBopAdditionalSections();

            $("#<%=radBOPIncOccYes.ClientID%>").change(function () { ShowHideBopAdditionalSections(); });
            $("#<%=radBOPIncOccNo.ClientID%>").change(function () { ShowHideBopAdditionalSections(); });

         }

        //Added 10/5/18 for multi state MLW       
        if (kqLobId == IFMLOBEnum.PPA.LobId) { 
            ShowHideAutoAdditionalSections();
            var ef = $("#txtUWQuestionsEffectiveDate").val();
            if (new Date(ef).getTime() >= new Date("01/01/2019").getTime()) {
                $("#<%=radMultiPolicyYes.ClientID%>").change(function () { ShowHideAutoAdditionalSections(); });
                $("#<%=radMultiPolicyNo.ClientID%>").change(function () { ShowHideAutoAdditionalSections(); });
            }         
            
        }

        if (kqLobId == IFMLOBEnum.PUP.LobId) { 
            ShowHideUmbrellaAdditionalSections();
            $("#<%=UmbrellaTypeFarm.ClientID%>").change(function () { ShowHideUmbrellaAdditionalSections(); });
            $("#<%=UmbrellaTypePersonal.ClientID%>").change(function () { ShowHideUmbrellaAdditionalSections(); });
        }

        
        //added 11/7/17 for HOM Upgrade MLW - ddl in code behind pulls text from XML and not actual selected text, so need this hidden value to find if mobile in code behind
        $("#<%=Me.ddHomFormList.ClientID%>").change(function () {
            var selectedForm = $("#<%=Me.ddHomFormList.ClientID%> option:selected").text();
            $("#<%=Me.hiddenSelectedForm.ClientID%>").val(selectedForm);
        });

        var LOBIsUsingDatePicker = true;//<%= IFM.VR.Common.Helpers.GenericHelper.LOBRequiresEffectiveDateAtQuoteStart(NewQuoteRequestLOBType).ToString().ToLower() %>;

        if (LOBIsUsingDatePicker === true) {
            var showDatePickerOnLoad = "<%= Me.hdnShowDatePickerOnPageLoad.Value %>";
            var min = $("#hdnAppMinimumEffectiveDate").val();
            var max = $("#hdnAppMaximumEffectiveDate").val();

            $("#txtUWQuestionsEffectiveDate").datepicker({
                changeMonth: true,
                changeYear: true,
                minDate: min,
                maxDate: max,
                showButtonPanel: true,
                gotoCurrent: true
            });             

            //$("#txtUWQuestionsEffectiveDate").datepicker('setDate', $('#txtUWQuestionsEffectiveDate').val());


            $("#txtUWQuestionsEffectiveDate").mask("00/00/0000");

            if (kqLobId === IFMLOBEnum.HOM.LobId || kqLobId === IFMLOBEnum.PPA.LobId) {                 
                //Because Personal Lines UW wants to make the site less incosistent... Awesome...
                var datepickerLeftPosition = 0;
                //TODO: DJG - Still has inconsistencies between IE and all other modern browsers. Would like to incorporate some kind of physical anchor to help tie down the datepicker... Not sure if I will have time for that.
                if (bowser.msie) {
                    datepickerLeftPosition = ((($(window).width() / 2) + (popupDialogWidth / 2) - 212)); //Viewport width + UWQuestionPopup width - the approximate datepicker width
                } else {
                    datepickerLeftPosition = ((($(window).width() / 2) + (popupDialogWidth / 2) - 208)); //Viewport width + UWQuestionPopup width - the approximate datepicker width
                }
                //var datepickerLeftPosition = ((($(window).width() / 2) + ($("#cphMain_ctlUWQuestionsPopup_divUwQuestions").width() / 2) - 208)); //Viewport width + UWQuestionPopup width - the approximate datepicker width
                $.extend($.datepicker, { _checkOffset: function (inst, offset, isFixed) { offset.left = datepickerLeftPosition; return offset; } }); //Adjust the datepicker to be along the top right hand corner of the UWQuestions dialog box

                $("#txtUWQuestionsEffectiveDate").css("color", "blue");                
            }

            if (showDatePickerOnLoad === "True") {
                $(document).ready(function () {
                    $("#lblUWQuestionsEffectiveDate").focus();
                    setTimeout(function () { $("#txtUWQuestionsEffectiveDate").datepicker("show"); }, 100);
                });
            }
        }

    }

    function HideUwQuestions() {
        InitKillQuestions();
        $("#<%=Me.divUwQuestions.ClientId%>").dialog("close");
        //EnableFormOnSaveRemoves();
    }

    function ShowHideAdditionalInfo() {

        var index = 0;
        $('#tblKillQuestions > tbody  > tr, .tblBOPKillQuestions > tbody  > tr').each(function () {
            if (index % 2 == 0) {
                if ($(this).find('input').first().is(':checked')) {
                    $(this).next().show();
                }
                else {
                    $(this).next().hide();
                }
            }
            index += 1;
        });
    }

    function AllAnswersAreAnswered() {

        var allHaveAnAnswer = true;
        var index_AllAnswers = 0;

        $('#tblKillQuestions > tbody  > tr, .tblBOPKillQuestions > tbody  > tr').each(function () {
            if (index_AllAnswers % 2 == 0) {
                if (!$(this).find('input').first().length) {
                    return true;
                }
                if ($(this).find('input').first().is(':checked') || $(this).find('input').last().is(':checked')) {
                    // atleast one is selected
                    //$(this).attr('style', 'height: 30px;');
                    $(this).attr('title', '');
                    $(this).find('span').first().hide();

                }
                else {
                    // neither is selected

                    allHaveAnAnswer = false;
                    //$(this).attr('style', 'border: 1.5px solid red;height: 30px;');
                    $(this).attr('title', 'Response Required');
                    $(this).find('span').first().show();
                }
            }
            index_AllAnswers += 1;
        });

        // Additional Kill Questions - Auto
        if (kqLobId == IFMLOBEnum.PPA.LobId) {
            // make sure additional not kill/warning question(s) are answered
            if ($("#radMultiPolicyYes").is(':checked') == false && $("#radMultiPolicyNo").is(':checked') == false) {
                // neither is selected
                $("#radMultiPolicyYes").prev().show();
                allHaveAnAnswer = false;
            }
            else {
                $("#radMultiPolicyYes").prev().hide();
            }
        }

        // Additional Kill Questions - HOM
        // Bug 3521 MGB 9/18/14
        if (kqLobId == IFMLOBEnum.HOM.LobId) {
            // make sure additional not kill/warning question(s) are answered
            if ($("#radHOMMultiPolicyYes").is(':checked') == false && $("#radHOMMultiPolicyNo").is(':checked') == false) {
                // neither is selected
                $("#radHOMMultiPolicyYes").prev().show();
                allHaveAnAnswer = false;
            }
            else {
                $("#radHOMMultiPolicyYes").prev().hide();
            }
            IsHOMLossHistoryKillQuestionAvailableValue = "<%= Me.hdnIsHOMLossHistoryKillQuestionAvailable.Value %>";
            if (IsHOMLossHistoryKillQuestionAvailableValue == "True") { 
                if ($("#radLossYes").is(':checked') == false && $("#radLossNo").is(':checked') == false) {
                    // neither is selected
                    $("#radLossYes").prev().show();
                    allHaveAnAnswer = false;
                }
                else {
                    $("#radLossYes").prev().hide();
                }
            }

        }

        // Additional Kill Questions - DFR
        if (kqLobId == IFMLOBEnum.DFR.LobId) {
            if ($("#radDFRYes").is(':checked') == false && $("#radDFRNo").is(':checked') == false) {
                // neither is selected
                $("#radDFRYes").prev().show();
                allHaveAnAnswer = false;
            }
            else {
                $("#radDFRYes").prev().hide();
            }
            IsDFRLossHistoryKillQuestionAvailableValue = "<%= Me.hdnIsDFRLossHistoryKillQuestionAvailable.Value %>";
            if (IsDFRLossHistoryKillQuestionAvailableValue == "True") {
                if ($("#radLossYes").is(':checked') == false && $("#radLossNo").is(':checked') == false) {
                    // neither is selected
                    $("#radLossYes").prev().show();
                    allHaveAnAnswer = false;
                }
                else {
                    $("#radLossYes").prev().hide();
                }
            }
            if ($("#radDwellingAgeYes").is(':checked') == false && $("#radDwellingAgeNo").is(':checked') == false) {
                // neither is selected
                $("#radDwellingAgeYes").prev().show();
                allHaveAnAnswer = false;
            }
            else {
                $("#radDwellingAgeYes").prev().hide();
            }
        }

        // Additional Kill Questions - Farm
        if (kqLobId == IFMLOBEnum.FAR.LobId) {
            // make sure additional not kill/warning question(s) are answered
            if ($("#<%=rad_far_Dog14Yes.ClientID%>").is(':checked') == false && $("#<%=rad_far_Dog14No.ClientID%>").is(':checked') == false)
            {
                // neither is selected
                $("#<%=rad_far_Dog14Yes.ClientID%>").prev().show();
                allHaveAnAnswer = false;
            }
            else
            {
                $("#<%=rad_far_Dog14Yes.ClientID%>").prev().hide();
                if ($("#<%=rad_far_Dog14Yes.ClientID%>").is(':checked'))
                {
                    // checked yes so....
                    // must have how many dogs entered
                    // must also have dog types checked yes or no
                    if ($("#<%=txtFar_How_Many_Dogs.ClientID%>").val() == ""){
                        allHaveAnAnswer = false;
                        $("#spanDogCountInfo").show();
                    }
                    else
                    {
                        $("#spanDogCountInfo").hide();
                    }
                }
            }

            // only check sub checks if 'Has dogs?' is yes
            if ($("#<%=rad_far_Dog14Yes.ClientID%>").is(':checked'))
            {
                if ($("#<%=rad_far_Dog14bYes.ClientID%>").is(':checked') == false && $("#<%=rad_far_Dog14bNo.ClientID%>").is(':checked') == false)
                {
                    // neither is selected
                    $("#<%=rad_far_Dog14bYes.ClientID%>").prev().show();
                    allHaveAnAnswer = false;
                }
                else
                {
                    $("#<%=rad_far_Dog14bYes.ClientID%>").prev().hide();
                    if ($("#<%=rad_far_Dog14bYes.ClientID%>").is(':checked'))
                    {
                        // checked yes so....
                        // must have how many dogs entered
                        // must also have dog types checked yes or no
                        if ($("#<%=txt_far_Dog14bMoreInfo.ClientID%>").val() == ""){
                            allHaveAnAnswer = false;
                            $("#spanDogBreadInfo").show();
                        }
                        else
                        {
                            $("#spanDogBreadInfo").hide();
                        }
                    }
                }
            }
        }

            // Additional Questions - Umbrella
        if (kqLobId == IFMLOBEnum.PUP.LobId) {
            $("#UmbrellaTypePersonal").prev().hide();
            $("#UmbrellaFarmTypePersonal").prev().hide();
            $("#UmbrellaFarmCommercialTypeIndividual").prev().hide();
            $("#UmbrellaFarmSizeSmall").prev().hide();
            // make sure additional not kill/warning question(s) are answered
            if ($("#UmbrellaTypePersonal").is(':checked') == false && $("#UmbrellaTypeFarm").is(':checked') == false)
            {
                // neither is selected
                $("#UmbrellaTypePersonal").prev().show();
                allHaveAnAnswer = false;
            }

            // only check sub checks if 'Farm' is yes
            if ($("#UmbrellaTypeFarm").is(':checked'))
            {
                if ($("#UmbrellaFarmTypePersonal").is(':checked') == false && $("#UmbrellaFarmTypeCommercial").is(':checked') == false)
                {
                    // neither is selected
                    $("#UmbrellaFarmTypePersonal").prev().show();
                    allHaveAnAnswer = false;
                }
                if ($("#UmbrellaFarmTypeCommercial").is(':checked') == true && $("#UmbrellaFarmCommercialTypeIndividual").is(':checked') == false && $("#UmbrellaFarmCommercialTypeFamilyPartnership").is(':checked') == false)
                {
                    // neither is selected
                    $("#UmbrellaFarmCommercialTypeIndividual").prev().show();
                    allHaveAnAnswer = false;
                }
                if ($("#UmbrellaFarmSizeSmall").is(':checked') == false && $("#UmbrellaFarmSizeLarge").is(':checked') == false)
                {
                    // neither is selected
                    $("#UmbrellaFarmSizeSmall").prev().show();
                    allHaveAnAnswer = false;
                }
            }
        }

        return allHaveAnAnswer;
    }

    function AllAdditionalInfoFieldsAreAnswered() {

        var All_IfYesHasAdditionalInfo = true;
        var index = 0;
        $('#tblKillQuestions > tbody  > tr').each(function () {
            if (index % 2 == 0) {
                if ($(this).find('input').first().is(':checked') || $(this).find('input').first().parent().hasAttr('alwaysRequiresAdditionalAnswer')) {
                    // yes is selected
                    if ($(this).next().find('textarea').first().val() == '') {
                        All_IfYesHasAdditionalInfo = false;
                        $(this).next().find('textarea').first().attr('style', 'border: 1px solid red; width: 100%;');
                        //Additional Information Response Required
                        $(this).next().find('span').first().css('color', 'red');
                        $(this).next().find('span').first().text("Additional Information Response Required");
                    }
                    else {
                        $(this).next().find('textarea').first().attr('style', 'width: 100%;');
                        $(this).next().find('span').first().css('color', 'black');
                        $(this).next().find('span').first().text("Additional Information");
                    }
                }
            }
            index += 1;
        });

        return All_IfYesHasAdditionalInfo;
    }

    //Added 10/5/18 for multi state MLW
    function AllPPAAdditionalInfoFieldsAreAnswered() {
        var All_IfYesHasAdditionalInfo = true;
        if (kqLobId == IFMLOBEnum.PPA.LobId) {
            var ef = $("#txtUWQuestionsEffectiveDate").val();
            if (new Date(ef).getTime() >= new Date("01/01/2019").getTime()) {           
                var index = 0;
                $('#tblPPAAdditionalQuestions > tbody  > tr').each(function () {
                    if (index % 2 == 0) {
                        if ($(this).find('input').first().is(':checked') || $(this).find('input').first().parent().hasAttr('alwaysRequiresAdditionalAnswer')) {
                            // yes is selected
                            if ($(this).next().find('textarea').first().val() == '') {
                                All_IfYesHasAdditionalInfo = false;
                                $(this).next().find('textarea').first().attr('style', 'border: 1px solid red; width: 100%;');
                                //Additional Information Response Required
                                $(this).next().find('span').first().css('color', 'red');
                                $(this).next().find('span').first().text("Additional Information Response Required");
                            }
                            else {
                                $(this).next().find('textarea').first().attr('style', 'width: 100%;');
                                $(this).next().find('span').first().css('color', 'black');
                                $(this).next().find('span').first().text("Additional Information");
                            }
                        }
                    }
                    index += 1;
                });
            }
        }
        return All_IfYesHasAdditionalInfo;      
    }

    function AllHOMAdditionalInfoFieldsAreAnswered() {
    var All_IfYesHasAdditionalInfo = true;
    if (kqLobId == IFMLOBEnum.HOM.LobId) {
        var index = 0;
        $('#tblHOMAdditionalQuestions > tbody  > tr').each(function () {
            if (index % 2 == 0) {
                if ($(this).find('input').first().is(':checked') || $(this).find('input').first().parent().hasAttr('alwaysRequiresAdditionalAnswer')) {
                    // yes is selected
                    if ($(this).next().find('textarea').first().val() == '') {
                        All_IfYesHasAdditionalInfo = false;
                        $(this).next().find('textarea').first().attr('style', 'border: 1px solid red; width: 100%;');
                        //Additional Information Response Required
                        $(this).next().find('span').first().css('color', 'red');
                        $(this).next().find('span').first().text("Additional Information Response Required");
                    }
                    else {
                        var radioValue = $(this).find('input').first().val();
                        if (radioValue == 'radHOMMultiPolicyYes') {
                            //Specific Validation for Question - Is there a related Auto quote or existing Indiana Farmers Personal Auto policy?
                            var autoValue = $(this).next().find('[id*=txtMoreInfoRelatedAuto]').val();
                            var isValidAutoResponse = true;
                            if (autoValue) {
                                var autoResponseLength = autoValue.length;
                                if (autoResponseLength < 9) {
                                    isValidAutoResponse = false;
                                } else {
                                    var quoteOrPolicy = autoValue.charAt(0).toUpperCase();
                                    var strBegin = '';
                                    var strEnd = '';
                                    if (quoteOrPolicy == 'Q') {
                                        strBegin = autoValue.substring(0, 4).toUpperCase();
                                        strEnd = autoValue.substring(4, autoResponseLength);
                                    } else if (quoteOrPolicy == 'P') {
                                        strBegin = autoValue.substring(0, 3).toUpperCase();
                                        strEnd = autoValue.substring(3, autoResponseLength);
                                    } else {
                                        isValidAutoResponse = false;
                                    }
                                    if (strBegin == 'QPPA' || strBegin == 'PPA') {
                                        if (isNaN(strEnd) || hasDashDotOrSpace(strEnd)) {
                                            isValidAutoResponse = false;
                                        } else {
                                            if (strEnd.length == 7 || strEnd.length == 6) {
                                                //good, let it through
                                            } else {
                                                isValidAutoResponse = false;
                                            }
                                        }
                                    } else {
                                        isValidAutoResponse = false;
                                    }
                                }
                            }

                            if (isValidAutoResponse == false) {
                                All_IfYesHasAdditionalInfo = false;
                                $(this).next().find('textarea').first().attr('style', 'border: 1px solid red; width: 100%;');
                                $(this).next().find('span').first().css('color', 'red');
                                $(this).next().find('span').first().text("Additional Information Invalid  Quote or Policy Number");
                            } else {
                                $(this).next().find('textarea').first().attr('style', 'width: 100%;');
                                $(this).next().find('span').first().css('color', 'black');
                                $(this).next().find('span').first().text("Additional Information");
                            }                           
                        } else {
                            $(this).next().find('textarea').first().attr('style', 'width: 100%;');
                            $(this).next().find('span').first().css('color', 'black');
                            $(this).next().find('span').first().text("Additional Information");
                        }
                    }
                }
            }
            index += 1;
        });
    }
    return All_IfYesHasAdditionalInfo;
    }

    function hasDashDotOrSpace(strToCompare) {
        var hasChar = false;
        if (strToCompare != null && strToCompare != '' && strToCompare.length > 0) {
            for (i = 0; i < strToCompare.length; i++) {
                if (strToCompare[i] == ' ' || strToCompare[i] == '-' || strToCompare[i] == '.') {
                    hasChar = true;
                }
            }
        }
        return hasChar;   
    }

    function AllDFRAdditionalInfoFieldsAreAnswered() {
        var All_IfYesHasAdditionalInfo = true;
        if (kqLobId == IFMLOBEnum.DFR.LobId) {
            var index = 0;
            $('#tblDFRAdditionalQuestions > tbody  > tr').each(function () {
                if (index % 2 == 0) {
                    if ($(this).find('input').first().is(':checked') || $(this).find('input').first().parent().hasAttr('alwaysRequiresAdditionalAnswer')) {
                        // yes is selected
                        if ($(this).next().find('textarea').first().val() == '') {
                            All_IfYesHasAdditionalInfo = false;
                            $(this).next().find('textarea').first().attr('style', 'border: 1px solid red; width: 100%;');
                            //Additional Information Response Required
                            $(this).next().find('span').first().css('color', 'red');
                            $(this).next().find('span').first().text("Additional Information Response Required");
                        }
                        else {
                            $(this).next().find('textarea').first().attr('style', 'width: 100%;');
                            $(this).next().find('span').first().css('color', 'black');
                            $(this).next().find('span').first().text("Additional Information");
                        }
                    }
                }
                index += 1;
            });
        }
        return All_IfYesHasAdditionalInfo;
    }

    function AllBOPAdditionalInfoFieldsAreAnswered() {
        var All_IfYesHasAdditionalInfo = true;
        if (kqLobId == IFMLOBEnum.BOP.LobId) {
            var index = 0;
            $('.tblBOPKillQuestions > tbody  > tr').each(function () {
                if (index % 2 == 0) {
                    if ($(this).find('input').first().is(':checked') || $(this).find('input').first().parent().hasAttr('alwaysRequiresAdditionalAnswer')) {
                        // yes is selected
                        if ($(this).next().find('textarea').first().val() == '') {
                            All_IfYesHasAdditionalInfo = false;
                            $(this).next().find('textarea').first().attr('style', 'border: 1px solid red; width: 90%;');
                            //Additional Information Response Required
                            $(this).next().find('span').first().css('color', 'red');
                            $(this).next().find('span').first().text("Additional Information Response Required");
                        }
                        else {
                            $(this).next().find('textarea').first().attr('style', 'width: 90%;');
                            $(this).next().find('span').first().css('color', 'black');
                            $(this).next().find('span').first().text("Additional Information");
                        }
                    }
                }
                index += 1;
            });
    }
        return All_IfYesHasAdditionalInfo;
    }

    function AtleastOneIsAnsweredYes() {

        var _AtleastOneIsAnsweredYes = false;
        var allHaveAnAnswer = true;
        var index_AllAnswers = 0;

        $('#tblKillQuestions > tbody  > tr').each(function () {
            if (index_AllAnswers % 2 == 0) {
                if ($(this).find('input').first().is(':checked') ) {
                    _AtleastOneIsAnsweredYes = true;

                }
            }
            index_AllAnswers += 1;
        });

        // Additional Kill Questions - Farm
        if (kqLobId == IFMLOBEnum.FAR.LobId) {
            // make sure additional not kill/warning question(s) are answered
            if ($("#<%=rad_far_Dog14Yes.ClientID%>").is(':checked') )
            {
                //_AtleastOneIsAnsweredYes = true; // having a dog is fine so no message for this - Matt A - 9-9-15
            }

            // only check sub checks if 'Has dogs?' is yes
            if ($("#<%=rad_far_Dog14Yes.ClientID%>").is(':checked'))
            {
                if ($("#<%=rad_far_Dog14bYes.ClientID%>").is(':checked'))
                {
                    _AtleastOneIsAnsweredYes = true;
                }
            }

        }

        return _AtleastOneIsAnsweredYes;
    }

    function ValidateEffectiveDate() {
        var returnVar = true

        //If the effectivedate textbox is visible, we will go through the validation for it. Otherwise, we will skip validation and allow the user through.
        if ($("#txtUWQuestionsEffectiveDate").is(":visible")) {
            var myDate = dateValueForJQueryFieldSelector("#txtUWQuestionsEffectiveDate");
            if (myDate) {
                //it is a date
                var errorMessage = "";
                errorMessage = ValidateEffectiveDateRange();
                if (errorMessage) {
                    //An error occurred
                    returnVar = false;
                    $("#<%= Me.lblEffectiveDateError.ClientID %>").text(errorMessage);
                }
            } else {
                //it is not a date
                $("#<%= Me.lblEffectiveDateError.ClientID %>").text("Missing Effective Date");
                returnVar = false;
            }

            if (returnVar === true) {
                var originalEffectiveDate = $("#hdnOriginalEffectiveDate").val();
                var newEffectiveDate = $("#txtUWQuestionsEffectiveDate").val();
                if (originalEffectiveDate) {
                    if (originalEffectiveDate === newEffectiveDate) {
                        if (isSubmitting === false) { //This works for not creating an unneeded postback, however it can cause an issue when trying to submit. This if statement should fix that.
                            returnVar = false; //we don't want to actually cause a post back. For some reason with the Jquery datepicker, if you select the same date, it causes a post back when it shouldn't.
                        }
                    } else {
                        $("#hdnOriginalEffectiveDate").val(newEffectiveDate);
                    }
                } else {
                    $("#hdnOriginalEffectiveDate").val(newEffectiveDate);
                }
                $("#<%= Me.lblEffectiveDateError.ClientID %>").hide();
                //$("#AdditionalEffectiveDateErrorSection").hide();
                $("#AdditionalEffectiveDateErrorSection").css("display", "none");
            } else {
                $("#<%= Me.lblEffectiveDateError.ClientID %>").show();
                if (StringHasSomething($("#<%= Me.lblAdditionalEffectiveDateError.ClientID %>").text()) == true) {
                    //$("#AdditionalEffectiveDateErrorSection").show();
                    $("#AdditionalEffectiveDateErrorSection").css("display", "block");
                } else {
                    //$("#AdditionalEffectiveDateErrorSection").hide();
                    $("#AdditionalEffectiveDateErrorSection").css("display", "none");
                }
            }
        }
        
        return returnVar;
    }

    function ValidateEffectiveDateRange() {
        var msg = "";
        var newDt = "";
        var setToNewDt = false;
        //var isReset = false;
        var effDt = dateValueForJQueryFieldSelector("#txtUWQuestionsEffectiveDate");
        $("#<%= Me.lblAdditionalEffectiveDateError.ClientID %>").text("");
        if (effDt) {
            var minEffDate = dateValueForJQueryFieldSelector("#hdnAppMinimumEffectiveDate");
            var maxEffDate = dateValueForJQueryFieldSelector("#hdnAppMaximumEffectiveDate");
            if (minEffDate && maxEffDate) {
                if (isDateBetweenMinimumAndMaximumDates(effDt, minEffDate, maxEffDate) == true) {
                    //valid date; everything okay
                } else {
                    //quote date is not between minDate and maxDate
                    msg = "effective date must be between " + minEffDate + " and " + maxEffDate;
                    if (isDate1LessThanDate2(effDt, minEffDate) == true) {
                        var updateToMinDate = false;
                        var minQuoteDateIsGreaterThanAllDate = boolValueForJQueryFieldSelector("#hdnAppMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes");
                        var minEffDtAllQuotes = dateValueForJQueryFieldSelector("#hdnAppMinimumEffectiveDateAllQuotes");
                        var quoteHasMinimunEffDt = boolValueForJQueryFieldSelector("#hdnAppQuoteHasMinimumEffectiveDate");
                        if (minQuoteDateIsGreaterThanAllDate == true) {
                            updateToMinDate = true;
                        } else if (minEffDate == minEffDtAllQuotes && quoteHasMinimunEffDt == true) {
                            updateToMinDate = true;
                        } else {
                            updateToMinDate = false; //redundant
                        }
                        if (updateToMinDate == true) {
                            var originalEffDate = dateValueForJQueryFieldSelector("#hdnAppOriginalEffectiveDate");
                            newDt = minEffDate;
                            setToNewDt = true;
                            if (isDate1EqualToDate2(originalEffDate, minEffDate) == true) {
                                //msg = "Contact Underwriting for quotes before " + minEffDate + "; date reverted back to " + minEffDate + ".";
                                msg = "Effective Date prior to " + minEffDate + " is not permitted. Choose a current date or greater to start your quote.";
                                //isReset = true;
                                $("#txtUWQuestionsEffectiveDate").text("");
                            } else {
                                //msg = "Contact Underwriting for quotes before " + minEffDate + "; date set to " + minEffDate + ".";
                                msg = "Effective Date prior to " + minEffDate + " is not permitted. Choose a current date or greater to start your quote.";
                                $("#txtUWQuestionsEffectiveDate").text("");
                            }
                        } else {
                            //quote date is less than minDate
                            //can customize message but already defaulted above
                            var beforeDtMsg = $("#hdnAppBeforeDateMsg").val();
                            if (StringHasSomething(beforeDtMsg) == true) {
                                //msg = appendText(msg, beforeDtMsg, "\n\n");
                                $("#<%= Me.lblAdditionalEffectiveDateError.ClientID %>").text(beforeDtMsg);
                            }
                        }
                    } else if (isDate1GreaterThanDate2(effDt, maxEffDate) == true) {
                        //quote date is more than maxDate
                        //can customize message but already defaulted above
                        var afterDtMsg = $("#hdnAppAfterDateMsg").val();
                        if (StringHasSomething(afterDtMsg) == true) {
                            //msg = appendText(msg, afterDtMsg, "\n\n");
                            $("#<%= Me.lblAdditionalEffectiveDateError.ClientID %>").text(afterDtMsg);
                        }
                    } else {
                        //shouldn't get here
                        //can customize message but already defaulted above
                    }
                }
            }
        } else {
            msg = "invalid effective date";
        }

        //if (msg) {
        //    if (msg.length > 0) {
        //        alert(msg);
        //        if (newDt && setToNewDt) {
        //            if (newDt.length > 0 && setToNewDt == true) {
        //                //set calendar date to newDt
        //                //******note 9/18/2015: still need to set calendar to new date here
        //                if (isReset == true) {
        //                    //date reverted back to previous date; just a note since msg already reflects this
        //                }
        //            }
        //        }
        //    }
        //}

        return msg;
    }

    // If a PPA copied quote or PPA compRater quote and the effective date is changed from a 2024 date to a 2025 date then show a message to rerate for the 3 new RAPA symbols
    function CheckRAPAEffectiveDate() {
        newRAPASymbolsEnabledValue = "<%= Me.hdnRAPANewSymbolsEnabled.Value %>";
        isNewQuoteValue = "<%= Me.hdnIsNewQuote.Value %>";
        if (kqLobId == IFMLOBEnum.PPA.LobId && newRAPASymbolsEnabledValue == "True" && isNewQuoteValue == "False") {
            var origEffDt = "<%= Me.hdnCopyQuoteEffectiveDate.Value %>";
            var newEffDt = dateValueForJQueryFieldSelector("#txtUWQuestionsEffectiveDate");
            if (origEffDt && newEffDt) {
                var origEffectiveDate_Date = ParseStringToDate(origEffDt);
                var newEffectiveDate_Date = ParseStringToDate(newEffDt);
                var RAPA_Date = ParseStringToDate('1/1/2025');
                if (origEffectiveDate_Date < RAPA_Date && newEffectiveDate_Date >= RAPA_Date) {
                    //date going forward, crossing date line - origEffDt 2024, newEffDt 2025
                    alert("Changing the effective date to January 1, 2025 or later requires you to select the lookup button for each quoted vehicle to ensure the correct rates and auto symbols are applied when re-rating your quote.");
                }
            }
            
        }
    }

    // If the governing state is Ohio and the effective date is less that 2/1/2021 then show a message
    // The call to this function can be removed after 3/1/2021
    function CheckOhioEffectiveDate() {
        var effDt = dateValueForJQueryFieldSelector("#txtUWQuestionsEffectiveDate");
        var GoverningStateId = $("#<%=ddlGoverningState.ClientID%>").val();

        //alert('eff: ' + effDt);
        //alert('gov: ' + GoverningStateId);
        var EffectiveDate_Date = ParseStringToDate(effDt);
        var OhioDate_Date = ParseStringToDate('2/1/2021');
        if (GoverningStateId == '36' && EffectiveDate_Date < OhioDate_Date) {
            alert("You have selected Ohio as your state.  We cannot write in Ohio prior to 2-1-2021.  Please change your effective date to 2-1-2021 or later.");
            return false;
        }
        return true;
    }

    // Parse a date string into a date value
    // Pass in date as m/d/yyyy or mm/dd/yyyy
    function ParseStringToDate(strDt) {
        var dt = null;
        var dtParts = strDt.split('/');
        if (dtParts.length == 3) {
            dt = new Date(dtParts[2], dtParts[0] - 1, dtParts[1]);
            //alert('dt: ' + dt)
        }
        return dt;
    }

    //function __doPostBack(eventTarget, eventArgument) {
    //    if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
    //        theForm.__EVENTTARGET.value = eventTarget;
    //        theForm.__EVENTARGUMENT.value = eventArgument;
    //        theForm.submit();
    //    }
    //}

    function ValidateUWForm() {
        isSubmitting = true;
        var a1 = AllAnswersAreAnswered()
        var a2 = AllAdditionalInfoFieldsAreAnswered();
        var a3 = ValidateEffectiveDate();       
        var a4 = AllPPAAdditionalInfoFieldsAreAnswered(); //Added 10/5/18 for multi state MLW
        var a5 = AllBOPAdditionalInfoFieldsAreAnswered();
        var a6 = AllDFRAdditionalInfoFieldsAreAnswered();
        var a7 = AllHOMAdditionalInfoFieldsAreAnswered();
        var hasGoverningState = true;
        var ohCheck = CheckOhioEffectiveDate();  // can be removed after 3/1/2021
  
        if (a1 && a2 && a4 && a5 && a6 && a7)
            $("#divWarningsQuestionsMsg").hide();
        else
            $("#divWarningsQuestionsMsg").show();

        if (kqLobId == IFMLOBEnum.FAR.LobId && AtleastOneIsAnsweredYes())
        {
            alert('<%= Me.ValMsg%>');
        }
        isSubmitting = false; //Just in case we come back with validations
        if ($("#<%=ddlGoverningState.ClientID%>").length) {
            if (!$("#<%=ddlGoverningState.ClientID%>").attr('disabled')) {
                var v = $("#<%=ddlGoverningState.ClientID%>").val();
                if (v == '') {
                    hasGoverningState = false;
                    alert('Missing Governing State');
                }
            }
        }
        
        return (a1 && a2 && a3 && a4 && a5 && a6 && a7 && hasGoverningState && ohCheck);
    }

    $(document).ready(function () {
        $("#UmbrellaTypePersonal, #UmbrellaTypeFarm").on('change', function () {
            if ($("#UmbrellaTypePersonal").is(":checked")) {
                $("#<%=ddlGoverningState.ClientID%>> option[value=16]").prop('selected', true).change();
                $("#<%=ddlGoverningState.ClientID%>>").prop('disabled', 'disabled').change();
            }
            else {
                $("#<%=ddlGoverningState.ClientID%>> option[value=16]").prop('selected', false).change();
                $("#<%=ddlGoverningState.ClientID%>>").prop('disabled', false).change();
            }
        });

         $("#UmbrellaFarmTypePersonal, #UmbrellaFarmTypeCommercial").on('change', function () {
             if ($("#UmbrellaFarmTypeCommercial").is(":checked")) {
                $(".conditionalUmbrellaFarmCommercialTypeQuestion").show();
            }
            else {
                 $(".conditionalUmbrellaFarmCommercialTypeQuestion").hide();
                 $("#UmbrellaFarmCommercialTypeIndividual").prop('checked', false);
                 $("#UmbrellaFarmCommercialTypeFamilyPartnership").prop('checked', false);

            }
	    });
    });
</script>

<style>
    #tblKillQuestions tr, .tblBOPKillQuestions tr {
        height: auto;
    }
</style>

<%--<input id="btnKillQuestions" type="button" value="Show Kill Questions" onclick="ShowUwQuestions();" />--%>
<div id="divUwQuestions" runat="server" title="Underwriting Questions" style="margin-left: auto; margin-right: auto; display: none;">   
    <%--<uc1:ctrlEffectiveDate runat="server" ID="ctrlEffectiveDate" PostBackOnChange="true" AllowOffsetDatePicker="true" />--%>
    <asp:HiddenField ID="hdnIsHOMLossHistoryKillQuestionAvailable" runat="server" />
    <asp:HiddenField ID="hdnIsDFRLossHistoryKillQuestionAvailable" runat="server" />
    <asp:HiddenField ID="hdnRAPANewSymbolsEnabled" runat="server" />
    <asp:HiddenField ID="hdnIsNewQuote" runat="server" />
    <asp:HiddenField ID="hdnCopyQuoteEffectiveDate" runat="server" />

    <div id="div_EffectiveDate" runat="server" style="margin-bottom: 30px">
        <div style="overflow:hidden;">
            <div style="float:left;width:auto;">
                <asp:Label ID="lblUWQuestionsEffectiveDate" runat="server" ClientIDMode="Static" TabIndex="1" style="outline:none;">*Effective Date:</asp:Label><asp:TextBox ID="txtUWQuestionsEffectiveDate" onblur="$(this).val(dateFormat($(this).val()));" onchange="if(ValidateEffectiveDate(this) === true){__doPostBack(this.id, 0);}CheckRAPAEffectiveDate(this);" style="border-color: red" ClientIDMode="Static" runat="server"></asp:TextBox>
            </div>
            <div style="padding-left:10px;overflow:hidden;">
                <asp:label runat="server" ID="lblEffectiveDateError" ForeColor="Red" />
            </div>
            <p id="AdditionalEffectiveDateErrorSection" style="display:none;">
                <asp:label runat="server" ID="lblAdditionalEffectiveDateError" ForeColor="Red" />
            </p>
        </div>

        <input id="hdnOriginalEffectiveDate" name="hdnOriginalEffectiveDate" type="hidden" ClientIDMode="Static" runat="server" />
        <input id="hdnAppMinimumEffectiveDate" name="hdnAppMinimumEffectiveDate" type="hidden" ClientIDMode="Static" runat="server" />
        <input id="hdnAppMaximumEffectiveDate" name="hdnAppMaximumEffectiveDate" type="hidden" ClientIDMode="Static" runat="server" />
        <input id="hdnAppMinimumEffectiveDateAllQuotes" name="hdnAppMinimumEffectiveDateAllQuotes" type="hidden" ClientIDMode="Static" runat="server" />
        <input id="hdnAppMaximumEffectiveDateAllQuotes" name="hdnAppMaximumEffectiveDateAllQuotes" type="hidden" ClientIDMode="Static" runat="server" />
        <input id="hdnAppQuoteHasMinimumEffectiveDate" name="hdnAppQuoteHasMinimumEffectiveDate" type="hidden" ClientIDMode="Static" runat="server" />
        <input id="hdnAppMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes" name="hdnAppMinimumQuoteEffectiveDateIsGreaterThanDateForAllQuotes" type="hidden" ClientIDMode="Static" runat="server" />
        <input id="hdnAppBeforeDateMsg" name="hdnAppBeforeDateMsg" type="hidden" ClientIDMode="Static" runat="server" />
        <input id="hdnAppAfterDateMsg" name="hdnAppAfterDateMsg" type="hidden" ClientIDMode="Static" runat="server" />
    </div>
    <div id="divDwellingAgeDFR" runat="server" style="width: 100%">
        <table style="border-collapse: collapse; height: 30px;">
            <tr style="background-color: lightgray;">
                <td style="width: 400px;">Is the age of the dwelling 75 years or older?</td>
                <td style="width: 70px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="dfr_dwelling" ID="radDwellingAgeYes" Text="Yes" runat="server" /></td>
                <td>
                    <asp:RadioButton ClientIDMode="Static" GroupName="dfr_dwelling" ID="radDwellingAgeNo" Text="No" runat="server" /></td>
            </tr>
        </table>
    </div>

    <div id="divLossHistoryHOM" runat="server" style="width: 100%">
        <table style="border-collapse: collapse; height: 30px;">
            <tr style="background-color: lightgray;">
                <td style="width: 400px;">Has any applicant had more than one loss in the past five years?</td>
                <td style="width: 70px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="hom_losshistory" ID="radLossYes" Text="Yes" runat="server" /></td>
                <td>
                    <asp:RadioButton ClientIDMode="Static" GroupName="hom_losshistory" ID="radLossNo" Text="No" runat="server" /></td>
            </tr>
        </table>
    </div>
    <table id="tblKillQuestions" style="border-collapse: collapse;">
        <asp:Repeater ID="Repeater1" runat="server">
            <AlternatingItemTemplate>
                <tr style="background-color: lightgray;">
                    <td style="width: 400px;"><%# DataBinder.Eval(Container.DataItem, "Description_NoQuestionNumber")%></td>
                    <td style="width: 70px;">
                        <span style="display: none; font-size: 15pt; color: red;">*</span>
                        <asp:RadioButton ID="radYes" Text="Yes" Checked='<%# DataBinder.Eval(Container.DataItem, "QuestionAnswerYes")%>' runat="server" /></td>
                    <td>
                        <asp:RadioButton ID="radNo" Text="No" Checked='<%# DataBinder.Eval(Container.DataItem, "QuestionAnswerNo")%>' runat="server" /></td>
                </tr>
                <tr style="background-color: lightgray;">
                    <td colspan="3">
                        <div style="margin-left: 30px;">
                            <span style="margin-bottom: 5px;">Additional Information</span>
                            <br />
                            <asp:TextBox ID="txtMoreInfo" Width="90%" Heigth="50px;" runat="server" TextMode="MultiLine" Text='<%# DataBinder.Eval(Container.DataItem, "PolicyUnderwritingExtraAnswer")%>'></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <asp:HiddenField ID="hdnPolicyUnderwritingCodeId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "PolicyUnderwritingCodeId") %>' />
            </AlternatingItemTemplate>
            <ItemTemplate>
                <tr>
                    <td style="width: 400px;"><%# DataBinder.Eval(Container.DataItem, "Description_NoQuestionNumber")%></td>
                    <td style="width: 70px;">
                        <span style="display: none; font-size: 15pt; color: red;">*</span>
                        <asp:RadioButton ID="radYes" Text="Yes" Checked='<%# DataBinder.Eval(Container.DataItem, "QuestionAnswerYes")%>' runat="server" /></td>
                    <td>
                        <asp:RadioButton ID="radNo" Text="No" Checked='<%# DataBinder.Eval(Container.DataItem, "QuestionAnswerNo")%>' runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="3">
                        <div style="margin-left: 30px;">
                            <span style="margin-bottom: 5px;">Additional Information</span>
                            <br />
                            <asp:TextBox ID="txtMoreInfo" Width="90%" Heigth="50px;" runat="server" TextMode="MultiLine" Text='<%# DataBinder.Eval(Container.DataItem, "PolicyUnderwritingExtraAnswer")%>'></asp:TextBox>
                        </div>
                    </td>
                </tr>
                <asp:HiddenField ID="hdnPolicyUnderwritingCodeId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "PolicyUnderwritingCodeId") %>' />
            </ItemTemplate>
        </asp:Repeater>
    </table>
    <div id="divAddtionalAutoQuestions" runat="server" style="width: 100%">
        <table id="tblPPAAdditionalQuestions" style="border-collapse: collapse; height: auto;">
            <tr>
                <td style="width: 400px;">Is there a related Homeowners/Farmowners quote or existing Indiana Farmers policy?</td>
                <td style="width: 70px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="ppa_multi" ID="radMultiPolicyYes" Text="Yes" runat="server" /></td>
                <td>
                    <asp:RadioButton ClientIDMode="Static" GroupName="ppa_multi" ID="radMultiPolicyNo" Text="No" runat="server" /></td>
            </tr>
            <!-- Added 10/4/18 for multi state MLW --->
            <tr id="tr_ppa_8MoreInfo" runat="server">
                    <td colspan="3">
                        <div style="margin-left: 30px;">
                            <span style="margin-bottom: 5px;">Additional Information</span>
                            <br />
                            <asp:TextBox ID="txtMoreInfoRelated" Width="90%" Heigth="50px;" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </div>
                    </td>
                </tr>
        </table>
    </div>

    <!-- Added per Bug 3521 MGB 9/18/14 -->
    <div id="divAdditionalHomeQuestions" runat="server" style="width: 100%">
        <table id="tblHOMAdditionalQuestions" style="border-collapse: collapse; height: auto">
            <tr style="background-color: lightgray;">
                <td style="width: 400px;">Is there a related Auto quote or existing Indiana Farmers Personal Auto policy?</td>
                <td style="width: 70px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="hom_multi" ID="radHOMMultiPolicyYes" Text="Yes" runat="server" /></td>
                <td>
                    <asp:RadioButton ClientIDMode="Static" GroupName="hom_multi" ID="radHOMMultiPolicyNo" Text="No" runat="server" /></td>
            </tr>
            <tr id="tr_hom_MoreInfo" runat="server">
                <td colspan="3">
                    <div style="margin-left: 30px;">
                        <span style="margin-bottom: 5px;">Additional Information</span>
                        <br />
                        <asp:TextBox ID="txtMoreInfoRelatedAuto" Width="90%" Heigth="50px;" runat="server" TextMode="MultiLine" MaxLength="11"></asp:TextBox>
                    </div>
                </td>
            </tr>
        </table>
    </div>

    <div id="divAdditionalDFRQuestions" runat="server" style="width: 100%">
    <table id="tblDFRAdditionalQuestions" style="border-collapse: collapse; height: auto;">
        <tr>
            <td style="width: 400px;">Any other insurance with this company?</td>
            <td style="width: 70px;">
                <span style="display: none; font-size: 15pt; color: red;">*</span>
                <asp:RadioButton ClientIDMode="Static" GroupName="dfr_multi" ID="radDFRYes" Text="Yes" runat="server"/></td>
            <td>
                <asp:RadioButton ClientIDMode="Static" GroupName="dfr_multi" ID="radDFRNo" Text="No" runat="server" /></td>
            </tr>
            <tr id="tr_dfr_MoreInfo" runat="server">
                <td colspan="3">
                    <div style="margin-left: 30px;">
                        <span style="margin-bottom: 5px;">Additional Information</span>
                        <br />
                        <asp:TextBox ID="txtMoreInfoDFR" Width="90%" Heigth="50px;" runat="server" TextMode="MultiLine"></asp:TextBox>
                    </div>
                </td>
            </tr>
        </table>
    </div>

    <!-- Added 09/02/2021 for bug 51550 MLW -->
    <div id="divAdditionalBOPQuestions" runat="server" style="width:100%;">
        <table style="border-collapse: collapse; height: auto;" class="tblBOPKillQuestions">
            <tr>
                <td style="width: 400px;">Is this risk a condominium or homeowner association that requires Condo Directors & Officers coverage, and you want to place with us?</td>
                <td style="width: 70px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="bop_multi" ID="radBOPCondoDandOYes" Text="Yes" runat="server" /></td>
                <td>
                    <asp:RadioButton ClientIDMode="Static" GroupName="bop_multi" ID="radBOPCondoDandONo" Text="No" runat="server" /></td>
            </tr>
            <tr style="height: 0px;"></tr>

            <tr style="background-color: lightgray;" runat="server" ID="trBOPOver35k">
                <td style="width: 400px;">Is any building over 35,000 feet in total area?</td>
                <td style="width: 70px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="bop_Over35k" ID="radBOPOver35kYes" Text="Yes" runat="server" /></td>
                <td>
                    <asp:RadioButton ClientIDMode="Static" GroupName="bop_Over35k" ID="radBOPOver35kNo" Text="No" runat="server" /></td>
            </tr>
            <tr style="height: 0px;"></tr>

            <tr runat="server" ID="trBOPOver3Stories">
                <td style="width: 400px;">Does any building exceed 3 stories?</td>
                <td style="width: 70px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="bop_Over3Stories" ID="radBOPOver3StoriesYes" Text="Yes" runat="server" /></td>
                <td>
                    <asp:RadioButton ClientIDMode="Static" GroupName="bop_Over3Stories" ID="radBOPOver3StoriesNo" Text="No" runat="server" /></td>
            </tr>
            <tr style="height: 0px;"></tr>

            <tr style="background-color: lightgray;" runat="server" ID="trBOPSales6M">
                <td style="width: 400px;">Are gross sales $6,000,000 or more?</td>
                <td style="width: 70px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="bop_Sales6M" ID="radBOPSales6MYes" Text="Yes" runat="server" /></td>
                <td>
                    <asp:RadioButton ClientIDMode="Static" GroupName="bop_Sales6M" ID="radBOPSales6MNo" Text="No" runat="server" /></td>
            </tr>
            <tr style="height: 0px;"></tr>

            <tr runat="server" ID="radBOPIncOcc">
                <td style="width: 400px;">Are there any incidental occupancies?</td>
                <td style="width: 70px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="bop_IncOcc" ID="radBOPIncOccYes" Text="Yes" runat="server" /></td>
                <td>
                    <asp:RadioButton ClientIDMode="Static" GroupName="bop_IncOcc" ID="radBOPIncOccNo" Text="No" runat="server" /></td>
            </tr>
            <tr id="tr_BOP_IncidentalOccupancy" runat="server">
                    <td colspan="3">
                        <div style="margin-left: 30px;">
                            <span style="margin-bottom: 5px;">Additional Information</span>
                            <br />
                            <asp:TextBox ID="txtIncidentalOccupancyRelated" Width="90%" Heigth="50px;" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </div>
                    </td>
                </tr>
        </table>
    </div>

    <!-- Added 7/18/2023 for CommDataPrefill -->
    <div id="divCommDataPrefill" runat="server" style="width: 100%">
        <table style="border-collapse: collapse; height: auto;">
            <tr runat="server" id="CommDataPrefillTableRow">
                <td style="width: 400px;">Would you like to utilize Commercial Data Prefill?</td>
                <td style="width: 70px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="commDataPrefill_multi" ID="radCommDataPrefillYes" Text="Yes" runat="server" /></td>
                <td>
                    <asp:RadioButton ClientIDMode="Static" GroupName="commDataPrefill_multi" ID="radCommDataPrefillNo" Text="No" runat="server" /></td>
            </tr>
        </table>
    </div>

    <div id="divFormType" runat="server" style="text-align: center; margin-top: 40px;">
        Select Form
        <asp:DropDownList ID="ddHomFormList" runat="server"></asp:DropDownList>
    </div>

    <div id="divFarmDogQuestions" runat="server" style="width: 100%">
        <table style="border-collapse: collapse; height: auto;">
            <tr>
                <td style="width: 400px;">Any dogs on premises?</td>
                <td style="width: 70px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="far_Dog14" ID="rad_far_Dog14Yes" Text="Yes" runat="server" /></td>
                <td>
                    <asp:RadioButton ClientIDMode="Static" GroupName="far_Dog14" ID="rad_far_Dog14No" Text="No" runat="server" /></td>
            </tr>
            <tr class="conditionalDogQuestion" style="background-color: lightgray;">
                <td colspan="3" style="width: 400px;">If yes, how many?
                    <div style="margin-left: 30px; margin-bottom: 10px;">
                        <span id="spanDogCountInfo" style="display: none; font-size: 15pt; color: red;">*</span>
                        <asp:TextBox ID="txtFar_How_Many_Dogs" Width="90%" Heigth="50px;" runat="server" TextMode="MultiLine"></asp:TextBox>
                    </div>
                </td>
            </tr>
            <tr class="conditionalDogQuestion">
                <td style="width: 400px;">Is any dog present either full or part breed of the following: Pit Bull, Rottweiler, Husky, Wolf Hybrid, German Shepherd, Chow, or Doberman If yes, describe:</td>
                <td style="width: 70px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="far_Dog14b" ID="rad_far_Dog14bYes" Text="Yes" runat="server" /></td>
                <td>
                    <asp:RadioButton ClientIDMode="Static" GroupName="far_Dog14b" ID="rad_far_Dog14bNo" Text="No" runat="server" /></td>
            </tr>
            <tr id="tr_far_Dog14bYes" runat="server">
                <td colspan="2">
                    <div style="margin-left: 30px; margin-bottom: 10px;">
                        <span id="spanDogBreadInfo" style="display: none; font-size: 15pt; color: red;">*</span>
                        <asp:TextBox ID="txt_far_Dog14bMoreInfo" runat="server" Width="90%" Heigth="50px;" TextMode="MultiLine"></asp:TextBox>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="divUmbrellaSection" runat="server" style="width: 100%">
        <table style="border-collapse: collapse; height: auto;">
            <tr>
                <td style="width: 300px; padding: 10px 0px;">Is this for Personal or Farm?</td>
                <td style="width: 100px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="UmbrellaType" ID="UmbrellaTypePersonal" Text="Personal" runat="server" /></td>
                <td style="width: 100px;">
                    <asp:RadioButton ClientIDMode="Static" GroupName="UmbrellaType" ID="UmbrellaTypeFarm" Text="Farm" runat="server" /></td>
            </tr>
            <tr class="conditionalUmbrellaFarmTypeQuestion" style="display: none">
                <td style="width: 300px; padding: 10px 0px 10px 15px;">Liability Type:</td>
                <td style="width: 100px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="UmbrellaFarmType" ID="UmbrellaFarmTypePersonal" Text="Personal" runat="server" /></td>
                <td style="width: 100px;">
                    <asp:RadioButton ClientIDMode="Static" GroupName="UmbrellaFarmType" ID="UmbrellaFarmTypeCommercial" Text="Commercial" runat="server" /></td>
            </tr>
            <tr class="conditionalUmbrellaFarmCommercialTypeQuestion" style="display: none">
                <td style="width: 300px; padding: 10px 0px 10px 15px;">Farm Type:</td>
                <td style="width: 100px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="UmbrellaFarmCommercialType" ID="UmbrellaFarmCommercialTypeIndividual" Text="Individual" runat="server" /></td>
                <td style="width: 210px;">
                    <asp:RadioButton ClientIDMode="Static" GroupName="UmbrellaFarmCommercialType" ID="UmbrellaFarmCommercialTypeFamilyPartnership" Text="Farm Corp / Partnership" runat="server" /></td>
            </tr>
        </table>
        <div class="NotingMinimumLimitTextTitleFarm" runat="server" style="width: 100%; margin-top: 10px; display: none;">
        <div>
            <center>
                Minimum Liability Requirements:
            </center>
        </div>
        <table runat="server" style="width: 100%;">
            <tr>
                <td style="vertical-align: top; width: 260px;">
                        <ul style="margin-left: -20px;">
                            <li>Farm and Dwelling Fire policies is $300,000.</li>
                            <li>Personal Auto: <div style="margin-left: 10px;">Split limits of $250,000/$500,000.</div></li>
                        </ul>
                </td>
                <td style="vertical-align: top; width: 250px;">
                        <ul> 
                            <li>Personal and Commercial Auto:<div style="margin-left: 10px;">Combined single limit of $300,000.</div></li>
                        </ul>
                </td>
            </tr>
        </table>
        </div>
        <div class="NotingMinimumLimitTextTitlePersonal" runat="server" style="width: 100%; margin-top: 10px; display: none;">
            <div >
                <center>
                    Minimum Liability Requirements:
                </center>
            </div>
            &nbsp;
        <table runat="server" style="width: 100%;">
            <tr>
                <td style="vertical-align: top;">
                    <div>
                        <span>&#8226;</span> Home and Dwelling Fire policies is $300,000.
                    </div>
                </td>
                <td>
                    <div>
                        <span>&#8226;</span> Personal Auto:
                        <div style="margin-left: 20px;">
                            <div>Split limits of $250,000/$500,000.</div>
                            <div>Combined single limit of $300,000.</div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
        </div>
        <table class="conditionalUmbrellaFarmSizeQuestionDetail" style="display: none"; width: 100%">
            <tr class="conditionalUmbrellaFarmSizeQuestion" style="display: none">
                <td style="width: 300px; padding: 10px 0px 5px 15px;">Farm Size:</td>
                <td style="width: 100px;">
                    <span style="display: none; font-size: 15pt; color: red;">*</span>
                    <asp:RadioButton ClientIDMode="Static" GroupName="UmbrellaFarmSize" ID="UmbrellaFarmSizeSmall" Text="Small" runat="server" /></td>
                <td style="width: 100px;">
                    <asp:RadioButton ClientIDMode="Static" GroupName="UmbrellaFarmSize" ID="UmbrellaFarmSizeLarge" Text="Large" runat="server" /></td>
            </tr>

            <tr id="Tr1" runat="server">
                <td>
                    <div style="height: 85px; margin-left: 30px;">
                        Small Farm:<br />
                        1. Named Insured is an individual<br />
                        2. Fewer than 1,000 acres<br />
                        3. No commercial vehicles (ie. heavy or extra-heavy trucks)<br />
                        4. No custom farming<br />
                    </div>
                    
                </td>
                <td colspan="2">
                    <div style="height: 85px; margin-left: 30px; width: 200px;">
                        Large Farm:<br />
                        Any farm that does not meet all the “Small Farm” guidelines.
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="GoverningStateSection" runat="server" visible="false" style="width: 100%; margin-top: 10px;">
        <center>
            Governing State:
            &nbsp;&nbsp;
            <asp:DropDownList runat="server" ID="ddlGoverningState">                
            </asp:DropDownList>
            <p>Governing State - Main location, domiciled location.<br />The policy can only have one governing state.</p>
        </center>
    </div>

    <asp:HiddenField ID="hiddenSelectedForm" runat="server" /><!-- added 11/7/17 for HOM Upgrade MLW -->
    <asp:HiddenField ID="hdnShowDatePickerOnPageLoad" runat="server" />

    <div id="divAction" style="text-align: center; margin-top: 15px;">
        <div id="divWarningsQuestionsMsg" style="color: red; display: none;">All Questions Must Be Answered</div>
        <br />        
        <asp:Button ID="btnCancel" CssClass="StandardButton" runat="server" Text="Cancel" />
        <asp:Button ID="btnSave" CssClass="StandardButton" OnClientClick="return ValidateUWForm();" runat="server" Text="Continue to Quote" />
    </div>
</div>