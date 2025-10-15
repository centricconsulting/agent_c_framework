<%@ Page Async="true" Language="vb" AutoEventWireup="true" MasterPageFile="~/VelociRater.Master" MaintainScrollPositionOnPostback="true" CodeBehind="MyVelocirater.aspx.vb" Inherits="IFM.VR.Web.MyVelocirater" %>

<%@ Import Namespace="IFM.VR.Web" %>

<%@ Register Src="~/User Controls/MyVelocirater/ctlQuoteSearch.ascx" TagPrefix="uc1" TagName="ctlQuoteSearch" %>
<%@ Register Src="~/User Controls/MyVelocirater/ctlQuoteSearchResults.ascx" TagPrefix="uc1" TagName="ctlQuoteSearchResults" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/MyVelocirater/ctlVr3Stats.ascx" TagPrefix="uc1" TagName="ctlVr3Stats" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
    <uc1:ctlUWQuestionsPopup runat="server" ID="ctlUWQuestionsPopup" />
    
    <table style="width: 100%; min-height: 350px;">
        <tr>
            <td style="width: 200px; vertical-align: top;">
                <div style="height: 8px;"></div>
                <uc1:ctlQuoteSearch runat="server" ID="ctlQuoteSearch" />
            </td>
            <td style="vertical-align: top;">
                <div id="dvSplash" runat="server" class="myVRSplash">
                    <div style="text-align:center;">VelociRater does more than ever before!</div>
                    <div class="myVRSplashButtonsContainer">
                        <a id="lnkSavedQuotes" runat="server" class="myVRButton" style="background-color:#f9a11b;" href="MyVelocirater.aspx?PageView=savedQuotes">
                            <div class="myVRButtonContainer">
                                <div class="myVRButtonHeader">Quotes</div>
                                <hr class="hrWhiteLine" />
                                <div class="myVRButtonContent">
                                    <div>Create, search and all around manage your quotes in one fast and easy hub.</div>
                                    <div id="divSavedQuotesLOBs">
                                        <ul>
                                            <li>Personal Lines*</li>
                                            <li>Farm Lines</li>
                                            <li>Commercial Lines</li>
                                            <li>Personal/Farm Umbrella*</li>
                                        </ul>
                                    </div>
                                </div>
                                <div class="myVRButtonContentNote">*As offered by state</div>
                            </div>
                        </a>
                        <a id="lnkSavedChanges" runat="server" class="myVRButton" style="background-color:#7accc8;" href="MyVelocirater.aspx?PageView=savedChanges" >
                            <div class="myVRButtonContainer">
                                <div class="myVRButtonHeader">Policy Changes</div>
                                <hr class="hrWhiteLine" />
                                <div class="myVRButtonContent">
                                    <div>Start and manage policy changes and more.</div>
                                    <div id="divSavedChangesLOBs">
                                        <ul id="EndorsementCapableLOBs" runat="server">
                                            <li>Personal Auto</li>
                                            <li>Home</li>
                                            <li>Dwelling Fire</li>
                                            <li>Farm Lines</li>
                                            <li>Commercial Auto</li>
											<li id="liCommercialBOP" runat="server">Commercial BOP</li>
											<li id="liCommBopEndSoon" runat="server">Commercial BOP Coming Soon</li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </a>
                        <a id="lnkBillingUpdates" runat="server" class="myVRButton" style="background-color:#00afcf;" href="MyVelocirater.aspx?PageView=billingUpdates">
                            <div class="myVRButtonContainer">
                                <div class="myVRButtonHeader">Billing Updates</div>
                                <hr class="hrWhiteLine" />
                                <div class="myVRButtonContent">
                                    <div>Review and update billing information only.</div>
                                    <div id="divBillingUpdatesLOBs">
                                        <ul>
                                            <li>Personal Auto</li>
                                            <li>Home</li>
                                            <li>Dwelling Fire</li>
                                            <li>Farm Lines</li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </a>
                    </div>
                </div>
                <div id="dvSearchResults" runat="server">
                    <div style="text-align: center;text-decoration: underline;"><h2>Search Results</h2></div>
                    <uc1:ctlQuoteSearchResults runat="server" ID="ctlQuoteSearchResultsAuto" ResultType="Auto" PageNumber="1" PageSize="8" />
                    <uc1:ctlQuoteSearchResults runat="server" ID="ctlQuoteSearchResultsHome" ResultType="Home" PageNumber="1" PageSize="8" />
                    <uc1:ctlQuoteSearchResults runat="server" ID="ctlQuoteSearchResultsComm" ResultType="Comm" PageNumber="1" PageSize="8" />
                    <uc1:ctlQuoteSearchResults runat="server" ID="ctlQuoteSearchResultsFarm" ResultType="Farm" PageNumber="1" PageSize="8" />
                    <uc1:ctlQuoteSearchResults runat="server" ID="ctlQuoteSearchResultsUmbrella" ResultType="Umbrella" PageNumber="1" PageSize="8" />
                </div>
            </td>
        </tr>
    </table>
    <div>
        <uc1:ctlVr3Stats runat="server" ID="ctlVr3Stats" />
    </div>
	<%--Chad Test--%>
<%--	<hr />
	<style type="text/css">
        .ui-combobox-list {
		  display: none;
		}

		.ui-combobox-toggle.ui-corner-all {
		  border-top-left-radius: 0;
		  border-bottom-left-radius: 0;
		  border-top-right-radius: 0;
		  border-bottom-right-radius: 0;
		}

		.ui-combobox {
		  position: relative;
		}

		.ui-combobox-toggle {
		  position: absolute;
		  top: -1px;
		  bottom: 0;
		  right: 0;
		  width: 14px;
		  height: 100%;
		  margin-right: 3px;
		  padding-bottom: 1px;
		  /* adjust styles for IE 6/7 */
		  *height: 1.7em;
		  *top: 0.1em;
		}

		.ui-combobox-input {
			width: 30em;
		}

		.ui-combobox-toggle:hover {
		  cursor: default;
		}

		.ui-autocomplete .ui-state-hover {
		  background-color: #3399ff !important;
		  background-image: none !important;
		  color: White;
		}

		.ui-autocomplete.ui-menu .ui-menu-item a {
		  padding: 2 0 0 0 !important;
		  line-height: 1 !important;
		}

		.ui-autocomplete.ui-widget {
		  font-family: Sans-Serif !important;
		  font-size: 1em !important;
		}

		.ui-autocomplete.ui-corner-all {
		  border-radius: 0 !important;
		}

	</style>
	<script>
        //https://jsfiddle.net/ze7fgby7/
	//https://github.com/steelheaddigital/jquery.ui.combify
        (function($) {
  $.widget("ui.combify", {
    options: {
      capitalizeInput: false,
      maxLength: 0
    },
    _create: function() {
      var self = this,
        select = self.element,
        options = self.options,
        id = select.prop('id'),
        hiddenInputSelector = "#" + id,
        textInputId = "CombifyInput-" + id,
        textInputSelector = "#" + textInputId,
        name = select.prop('name'),
        selectedValue = select.find(':selected').val(),
        selectedText = select.find(':selected').text(),
        selectOptions = select.find('option'),
        optionArray = new Array();

      //Hide the original select
      select.hide();

      //Insert new HTML for a text input and a button to trigger the dropdown
      select.before('<div>' +
        '<span class="ui-combobox">' +
        '<input type="hidden" class="insertedInput" id="' + id + '" name="' + name + '" value="' + selectedValue + '">' +
        '<input type="text" id="' + textInputId + '" class="ui-combobox-input" value="' + selectedText + '">' +
        '<a class="ui-combobox-toggle"></a>' +
        '</span></div>');

      //Remove the the id and name from the original select since they are now on the hidden input so that posted forms will get the correct value
      select.removeAttr('id', null)
        .removeAttr('name', null)
        .on('change', function(event) {
          event.stopPropagation();
        });

      //Get all the options from the select list and put them in an array for use in the autocomplete data source
      selectOptions.each(function(i) {
        optionArray.push($(this).text());
      });

      //Add autocomplete to the new input
      $(textInputSelector).autocomplete({
          source: optionArray,
          select: function(event, ui) {
            //For some reason selecting a value doesn't automatically trigger the change event on the input, so trigger it here
            this.value = ui.item.value;
            var option = $(select).find('option').filter(function() {
              return $(this).html() == ui.item.value;
            }).first()
            var selectValue = option.val();

            //set the value of the hidden input to the option value that matches the selected autocomplete value
            $(hiddenInputSelector).val(selectValue);
            $(this).trigger('change');
          }
        })
        .on('change', function() {
          var value = $(this).val();
          var option = $(select).find('option').filter(function() {
            return $(this).html() == value;
          }).first()

          //If no matching option is found in the select list, then set the hidden input to the entered value
          if (!option.length) {
            $(hiddenInputSelector).val(value);
          } else {
            $(hiddenInputSelector).val(option.val());
          }
        })
        .on('blur', function() {
          $(this).trigger('change');
        });

      //Convert entered values to upper case if capitalizeInput option is true
      if (options.capitalizeInput) {
        var input = $(textInputSelector);
        input.css("text-transform", "uppercase").data('val', input.val()).on('keyup', function() {
          //Make the value upper case if it has changed
          var theInput = $(this);
          if (theInput.data('val') != this.value) {
            theInput.val(this.value.toUpperCase());
          }
          //Store the current value for comparison on next change
          theInput.data('val', this.value);

          theInput = null;
        });

        input = null;
      }

      //If maximum length is required
      if (options.maxLength > 0) {
        $(textInputSelector).keyup(function() {
          if ($(this).val().length > options.maxLength) {
            var TempVal = $(this).val();
            $(this).data('val', TempVal.substring(0, options.maxLength));
            $(this).val(TempVal.substring(0, options.maxLength));
          }
        });
      }

      //Attach a change event to the select list to put the selected value in the new text input
      select.on('change', function() {
        var hiddenInput = $(this).prev().find("#" + id).first(); //hidden input
        var selectedValue = $(this).val();
        var text = $(this).find("option:selected").text();

        //find the option that matches the value
        var option = $(select).find('option').filter(function() {
          return $(this).html() == text
        }).first()
        hiddenInput.val(selectedValue);
        $(textInputSelector).val(option.text()); //set the visible textbox to the value of the options text
        hiddenInput.trigger('change');
      });

      //Add the button to trigger the dropdown
      var button = select.prev().first().find(".ui-combobox-toggle");
      button.button({
        icons: {
          primary: "ui-icon-triangle-1-s"
        },
        text: false
      });

      //Attach the click event to the button to trigger the dropdown.
      button.click(function(event) {
        event.stopPropagation();
        var minWidth = $(this).prev().first().width();
        ExpandSelectList($(this), event, minWidth);
      });

      //Attach an event to expand the select list if the user presses Alt + DownArrow
      $(textInputSelector).keydown(function(event) {
        var list = $(this).parent().parent().next();

        if (event.which === 40 && event.altKey) {
          //If the list is already visible then just hide it
          event.preventDefault();
          event.stopPropagation();
          if (list.is(":visible")) {
            list.hide();
          } else {
            if (options.capitalizeInput) {
              this.value = this.value.toUpperCase();
            }
            $(textInputSelector).autocomplete("close");
            ExpandSelectList($(this), event, $(this).width());
          }
        }
      });

      //Attach an event to close the list after a selection is made
      var list = select.prev().first().find(".ui-combobox-list");
      list.change(function() {
        $(this).hide();
      });

      //private methods
      function ExpandSelectList(element, event, minWidth) {
        var list = element.parent().parent().next();

        //If the list is already open or the autocomplete list is open then close the list.
        if (list.is(":visible") || $(textInputSelector).autocomplete("widget").is(":visible")) {
          list.hide();
        } else {
          //Set the length of the select list to either the number of items in the list or 30, whichever is smaller
          var size;
          if (optionArray.length <= 30) {
            size = optionArray.length;
          } else {
            size = 30;
          }

          var sizeAttr = size === 1 ? 2 : size;
          list.css({
              "width": "auto",
              "position": "absolute",
              "z-index": "1"
            }) //Puts the list on top of all other elements
            .prop("size", sizeAttr) //changes the select list to a listbox so that it will "expand"
            .show();

          if (minWidth > list.width()) {
            list.css("width", minWidth);
          }

          var listLineHeight = parseInt(list.find('option').first().css('font-size'), 10);
          list.css("height", listLineHeight * (size + 1));

          //Attach a one-time event to the document to close the list if the user clicks anywhere else on the page.
          $(document).one("click", function() {
            list.hide();
          });

          function nextItem(event) {
            var down = "down",
              up = "up"

            //If the user presses up arrow move to the previous item in the list
            if (event.which === 38 && list.is(":visible")) {
              move(up);
              return;
            }

            //If user presess down arrow move to the next item in the list
            if (event.which === 40 && list.is(":visible")) {
              move(down);
              return;
            }

            //if the user presses enter trigger the change event on the input to set it's value to the selected value
            if (event.which === 13 && list.is(":visible")) {
              list.trigger("change");
              list.hide();
              return;
            }

            if (list.is(":visible")) {
              list.hide();
            }

            function move(direction) {
              event.preventDefault();
              var selected = list.find(":selected");
              if (direction === down) {
                var nextItem = selected.next();
              }
              if (direction === up) {
                var nextItem = selected.prev();
              }
              selected.prop('selected', false);
              nextItem.prop('selected', "selected");
            }
          }

          //Attach an event to move through the list with the arrow keys
          $(document).off("keydown.combifySelect")
            .on("keydown.combifySelect", nextItem);
        }
      }
    },

    _destroy: function() {
      var select = this.element,
        inputObj = select.prev().find('.insertedInput'),
        id = "",
        name = "";
      id = inputObj.prop('id');
      name = inputObj.prop('name');

      select.attr({
        id: id,
        name: name,
      });
      select.off('change');
      select.prev().remove();
      select.show();
    }
  });
})(jQuery);


$(document).ready(function() {
  $("#MySelect").combify();
});

	</script>
    <select id="MySelect">
	<option value="938">1ST PRIORITY INSURANCE | 6464-3536</option>
		<option value="566">1ST SECURITY INSURANCE | 6273-3220</option>
		<option value="567">1ST SECURITY INSURANCE | 6273-3221</option>
		<option value="568">1ST SECURITY INSURANCE | 6273-3222</option>
		<option value="44">1ST SOURCE INSURANCE, INC. | 6040-2544</option>
		<option value="414">A-1 INSURANCE AGENCY | 6978-1978</option>
		<option value="415">A-1 INSURANCE AGENCY | 6978-2555</option>
		<option value="666">ACCEPTABLE ANSWERS TO INSURANCE | 6319-3320 (closed)</option>
		<option value="593">ACOSTA INSURANCE AGENCY, INC. | 6289-3246</option>
		<option value="600">AG PRODUCERS INSURANCE INC. | 6293-3253</option>
		<option value="441">AGENCY ACCOUNT | 6000-3000</option>
		<option value="949">AGSURANCE LLC | 6466-5061</option>
		<option value="8">ALEXANDER INS. AGENCY, INC. | 6004-1842</option>
		<option value="732">ALGATE INSURANCE ADVISORS, LLC | 6363-3385</option>
		<option value="372">ALL AMERICAN INSURANCE | 6774-2774</option>
		<option value="685">ALL INSURANCE SERVICES, LLC | 6333-3339</option>
		<option value="782">ALLEGIANCE INSURANCE SERVICES INC. | 6297-3437</option>
		<option value="9">ALLIED INSURANCE AGENCY | 6005-2767</option>
		<option value="349">AMERICAN BUSINESS INSURANCE, INC. | 6718-2718</option>
		<option value="700">AMERICAN BUSINESS INSURANCE, INC.* | 6718-3354</option>
		<option value="456">AMERICAN TRUST INSURANCE SERVICES LLC | 6839-2952</option>
		<option value="596">AMSTUTZ &amp; WELKER INSURANCE | 6290-3249</option>
		<option value="667">AMSTUTZ &amp; WELKER INSURANCE | 6290-3321</option>
		<option value="598">AMSTUTZ INSURANCE | 6290-3251</option>
		<option value="595">AMSTUTZ INSURANCE INC. | 6290-3248</option>
		<option value="418">ANDERSON INSURANCE, INC. | 6998-2911</option>
		<option value="60">ANDY HARMON INS. AG. INC. | 6054-2397</option>
		<option value="678">ASSOCIATED INSURANCE SERVICES LLC | 6326-3332</option>
		<option value="772">ASSURED PARTNERS NL LLC | 6288-3426</option>
		<option value="495">ASSURED PARTNERS NL LLC | 6288-2992</option>
		<option value="496">ASSURED PARTNERS NL LLC | 6288-2993</option>
		<option value="497">ASSURED PARTNERS NL LLC | 6288-2994</option>
		<option value="528">ASSURED PARTNERS NL LLC | 6288-3181</option>
		<option value="959">ASSUREDPARTNERS GREAT PLAINS LLC | 6288-7002</option>
		<option value="921">ATKINS INSURANCE ADVISORS | 6438-3526</option>
		<option value="285">AUSTIN INSURANCE AGENCY, INC. | 6273-1559</option>
		<option value="611">AVON INSURANCE ASSOCIATES INC. | 6299-3266</option>
		<option value="898">B-MAC AGENCY | 6438-3512</option>
		<option value="212">BAKER AGENCY | 6315-2194</option>
		<option value="10">BAKER AND COMPANY INSURANCE INC. | 6006-2807</option>
		<option value="213">BAKER-REIMER INSURANCE AGENCY | 6315-2772</option>
		<option value="555">BARNUM-BROWN INSURANCE, INC. | 6266-3209</option>
		<option value="812">BARRETT AND ASSOCIATES | 6409-3466</option>
		<option value="912">BEACON INSURANCE GROUP INC. | 6452-5046</option>
		<option value="901">BEARD INSURANCE AGENCY | 6432-5040</option>
		<option value="809">BEATTY INSURANCE, INC. | 6407-3463</option>
		<option value="805">BECK CURRY INSURANCE GROUP INC. | 6406-3459</option>
		<option value="896">BECK INSURANCE AGENCY | 6432-5039</option>
		<option value="780">BENNETT AND SHEPHERD INSURANCE | 6353-3434</option>
		<option value="211">BILL BAKER INSURANCE AGENCY | 6315-1315</option>
		<option value="40">BILL EVANS INSURANCE, INC. | 6036-1800</option>
		<option value="738">BIXLER INSURANCE INC. | 6370-3391</option>
		<option value="935">BLACK &amp; RAMER INSURANCE LLC | 6459-3533</option>
		<option value="14">BLAKE INSURANCE AGENCY INC. | 6010-2520</option>
		<option value="858">BLANK'S INSURANCE AGENCY, LLC | 6427-5019</option>
		<option value="15">BLOOMINGTON INS. AG | 6011-2061</option>
		<option value="897">BLUFFTON INSURANCE SERVICES | 6438-3511</option>
		<option value="737">BOLLWERK INSURANCE GROUP LLC | 6369-3390</option>
		<option value="498">BOWMAN &amp; THALLS INS INC | 6232-2995</option>
		<option value="316">BRACKNEY INSURANCE GROUP INC. | 6632-2632</option>
		<option value="799">BRACKNEY INSURANCE GROUP INC. | 6632-3453</option>
		<option value="16">BRANSON INSURANCE &amp; BONDS | 6012-2110</option>
		<option value="6">BREWSTER INS AGCY INC | 6003-1003</option>
		<option value="17">BREWTON INSURANCE | 6013-1840</option>
		<option value="612">BRIAN SCHULZE INSURANCE AND ASSOCIATES | 6297-3265</option>
		<option value="608">BRIAN SCHULZE INSURANCE AND ASSOCIATES | 6297-3262</option>
		<option value="363">BRIGHT AND WILLIAMSON INSURANCE | 6756-2846</option>
		<option value="514">BROOKS INSURANCE PROFESSIONALS, INC. | 6242-3167</option>
		<option value="674">BROWN INSURANCE GROUP | 6323-3328</option>
		<option value="627">BUNDY MCNEAR INSURANCE AGENCY INC. | 6311-3281</option>
		<option value="622">BURKHART INSURANCE | 6308-3276</option>
		<option value="629">BURKMAN INSURANCE | 6393-3283</option>
		<option value="705">BURNS FAMILY INSURANCE, INC. | 6350-3359</option>
		<option value="20">BURPO-GOSE INSURANCE AG. INC. | 6016-2648</option>
		<option value="952">BUTLER INSURANCE AGENCY | 6453-5063</option>
		<option value="846">C F &amp; H INSURANCE AGENCY, INC. | 6423-5010 (closed)</option>
		<option value="847">C F &amp; H INSURANCE AGENCY, INC. | 6423-5011 (closed)</option>
		<option value="422">CALLISTUS SMITH AGENCY | 6134-2089</option>
		<option value="987">CAMPBELL SHUCK INSURANCE LLC | 6250-3564</option>
		<option value="855">CANNON-MASON AGENCY | 6425-3492</option>
		<option value="884">CARMICHAEL INSURANCE LLC | 6437-3502</option>
		<option value="870">CENTRAL ILLINOIS AGENTS GROUP, LLC | 6432-5025</option>
		<option value="23">CENTRAL INDIANA INSURANCE SERV.INC. | 6019-2303</option>
		<option value="24">CENTRAL INSURANCE ASSOCIATES, INC. | 6020-3008</option>
		<option value="826">CFD GROUP, INC. | 6418-3480</option>
		<option value="852">CHANEY &amp; KARCH DBA A&amp;E INSURANCE | 6424-5015</option>
		<option value="850">CHANEY &amp; KARCH DBA FRIEDRICH INSURANCE | 6424-5013</option>
		<option value="851">CHANEY &amp; KARCH DBA FRIEDRICH INSURANCE | 6424-5014</option>
		<option value="853">CHANEY &amp; KARCH DBA LEADENDECKER INSURANCE | 6424-5016</option>
		<option value="614">CHRIS STONEBRAKER INSURANCE | 6302-3268</option>
		<option value="868">CHRISTINE J NEWTON AGENCY / SOUTH SHORE INSURANCE | 6297-3497</option>
		<option value="832">CI INSURANCE | 6419-3486</option>
		<option value="834">CI INSURANCE | 6419-3488</option>
		<option value="835">CI INSURANCE | 6419-3489</option>
		<option value="955">CINDY WILKINS INSURANCE AGENCY INC. | 6469-3549</option>
		<option value="808">CITIZENS INSURANCE AGENCY | 6388-3462</option>
		<option value="794">CLARK INSURANCE GROUP INC | 6385-3448</option>
		<option value="762">CLARK INSURANCE GROUP INC | 6385-3416</option>
		<option value="235">CLARK INSURANCE GROUP INC | 6385-2385</option>
		<option value="239">CLARK INSURANCE GROUP INC | 6385-2904</option>
		<option value="706">CLARK INSURANCE GROUP INC DBA BONHAM INSURANCE | 6385-3360</option>
		<option value="795">CLARK INSURANCE GROUP INC DBA BONHAM INSURANCE | 6385-3449</option>
		<option value="793">CLARK INSURANCE GROUP INC DBA COUNTRY CORNER INS | 6385-3447</option>
		<option value="236">CLARK INSURANCE GROUP INC DBA COUNTRY CORNER INS | 6385-2707</option>
		<option value="403">CLAXTON AND ESTELLE INSURANCE | 6850-2924</option>
		<option value="836">CLEMENS &amp; ASSOCIATES, INC. | 6421-5000</option>
		<option value="710">CLINTON COUNTY INSURANCE | 6316-3364</option>
		<option value="876">COCHRAN INSURANCE AGENCY, INC. | 6434-5031</option>
		<option value="845">COMPASS INSURANCE PARTNERS | 6422-5009</option>
		<option value="837">COMPASS INSURANCE PARTNERS | 6422-5001</option>
		<option value="838">COMPASS INSURANCE PARTNERS | 6422-5002</option>
		<option value="839">COMPASS INSURANCE PARTNERS | 6422-5003</option>
		<option value="840">COMPASS INSURANCE PARTNERS | 6422-5004</option>
		<option value="841">COMPASS INSURANCE PARTNERS | 6422-5005</option>
		<option value="843">COMPASS INSURANCE PARTNERS | 6422-5007</option>
		<option value="848">COMPASS INSURANCE PARTNERS | 6422-5012</option>
		<option value="28">COOPER INSURANCE SERVICE, INC. | 6025-2075</option>
		<option value="942">COURI INSURANCE AGENCY | 6465-3541</option>
		<option value="768">COVENANT RISK MANAGEMENT GROUP LLC | 6389-3422</option>
		<option value="537">CRAWFORD CO. SECURITY COMPANY, INC. | 6259-3191 (closed)</option>
		<option value="18">D. E. BROWN, INC. | 6014-1544</option>
		<option value="32">DALE INSURANCE | 6028-2889</option>
		<option value="756">DALE STATE AGENCY, INC. | 6353-3410</option>
		<option value="900">DAN SHIELDS INSURANCE LLC | 6445-3514</option>
		<option value="878">DANSIG INC. | 6436-5033</option>
		<option value="985">DAVID L CAMPBELL INSURANCE | 6486-7008</option>
		<option value="943">DENSTORFF INSURANCE AGENCY | 6415-3542</option>
		<option value="821">DENSTORFF INSURANCE AGENCY | 6415-3475</option>
		<option value="680">DEWITT INSURANCE GROUP, LLC | 6328-3334</option>
		<option value="656">DISCONTINUED AGENTS TERRITORY 1 | 6317-3311</option>
		<option value="657">DISCONTINUED AGENTS TERRITORY 2 | 6317-3312</option>
		<option value="658">DISCONTINUED AGENTS TERRITORY 3 | 6317-3313</option>
		<option value="659">DISCONTINUED AGENTS TERRITORY 4 | 6317-3314</option>
		<option value="519">DIVERSIFIED INSURANCE GROUP | 6246-3172</option>
		<option value="406">DOLSON INSURANCE AGENCY, INC. | 6879-2879</option>
		<option value="941">DONAVAN INSURANCE SERVICES | 6438-3538</option>
		<option value="513">DOUBLE EAGLE INSURANCE | 6241-3166</option>
		<option value="902">DPCM INSURANCE AGENCY, INC. | 6432-5041</option>
		<option value="919">DULING INSURANCE AGENCY | 6011-3525</option>
		<option value="222">DUNAWAY INSURANCE AGENCY, INC. | 6359-2359</option>
		<option value="240">EDINBURGH INSURANCE | 6393-2393</option>
		<option value="701">EDINBURGH INSURANCE | 6393-3355</option>
		<option value="975">ELLINGER RIGGS INSURANCE | 6480-3554</option>
		<option value="976">ELLINGER RIGGS INSURANCE | 6480-3555</option>
		<option value="650">ELLINGER RIGGS INSURANCE | 6314-3304 (closed)</option>
		<option value="651">ELLINGER RIGGS INSURANCE | 6314-3305 (closed)</option>
		<option value="642">ENCORE INSURANCE GROUP-BRAD SAMPLES AGENCY | 6313-3296</option>
		<option value="641">ENCORE INSURANCE GROUP-KNUEVEN AGENCY | 6313-3295</option>
		<option value="982">EPIC INSURANCE BROKERS &amp; CONSULTANTS | 6485-3560</option>
		<option value="983">EPIC INSURANCE BROKERS &amp; CONSULTANTS | 6485-3561</option>
		<option value="984">EPIC INSURANCE BROKERS &amp; CONSULTANTS | 6485-3562</option>
		<option value="605">EPIC INSURANCE MIDWEST/EVANSVILLE | 6295-3258</option>
		<option value="606">EPIC INSURANCE MIDWEST/FORT WAYNE | 6295-3260</option>
		<option value="801">EPIC INSURANCE MIDWEST/FRANKLIN | 6295-3455</option>
		<option value="126">EPIC INSURANCE MIDWEST/HBG | 6115-2782</option>
		<option value="123">EPIC INSURANCE MIDWEST/HBG | 6115-2115</option>
		<option value="127">EPIC INSURANCE MIDWEST/HBG-WIGGINS | 6115-2812</option>
		<option value="348">EPIC INSURANCE MIDWEST/HEINY | 6717-2797</option>
		<option value="603">EPIC INSURANCE MIDWEST/INDIANAPOLIS | 6295-3257</option>
		<option value="452">EPIC INSURANCE MIDWEST/LIBERTY | 6120-2948 (closed)</option>
		<option value="347">EPIC INSURANCE MIDWEST/MBAH | 6717-2717</option>
		<option value="131">EPIC INSURANCE MIDWEST/RICHMOND | 6120-2838</option>
		<option value="604">EPIC INSURANCE MIDWEST/TERRE HAUTE | 6295-3259</option>
		<option value="39">ERNSBERGER INSURANCE | 6035-2235</option>
		<option value="986">ETHOS INSURANCE &amp; RISK MANAGEMENT INC | 6487-3563</option>
		<option value="692">EUGENE F. SARKEY INS. AGENCY INC. | 6341-3346</option>
		<option value="602">EVANS AGENCY LLC | 6294-3256</option>
		<option value="740">EXCEED INSURANCE | 6373-3394</option>
		<option value="81">F. NEAL JOHNSON AGENCY | 6414-2231</option>
		<option value="554">FAMILY INSURANCE SERVICES, INC. | 6265-3208</option>
		<option value="615">FAMILY SOURCE INSURANCE | 6303-3269</option>
		<option value="869">FARLEY INSURANCE AGENCY, INC. | 6431-5024</option>
		<option value="344">FEARRIN INSURANCE AGENCY | 6712-2882</option>
		<option value="41">FERDINAND FARMERS INS. AGENCY | 6037-2580</option>
		<option value="872">FERGURSON INSURANCE AGENCY | 6432-5027</option>
		<option value="43">FIRST ADVANTAGE INSURANCE | 6039-2792</option>
		<option value="564">FIRST INSURANCE GROUP INC. | 6273-3218</option>
		<option value="662">FIRST INSURANCE GROUP INC. | 6273-3316</option>
		<option value="965">FIRST INSURANCE GROUP OF THE MIDWEST INC. | 6473-7004</option>
		<option value="966">FIRST INSURANCE GROUP OF THE MIDWEST INC. | 6473-7005</option>
		<option value="967">FIRST INSURANCE GROUP OF THE MIDWEST INC. | 6473-7006</option>
		<option value="956">FIRST MID INSURANCE GROUP INC | 6470-5065</option>
		<option value="833">FLEECE INSURANCE, INC. | 6420-3487</option>
		<option value="46">FOX INSURANCE AGENCY | 6042-2732</option>
		<option value="806">FRANKLIN INS. AGENCY | 6273-3460</option>
		<option value="880">FRANZMAN INSURANCE AGENCY | 6353-3499</option>
		<option value="981">FREIBURG INSURANCE AGENCY | 6484-5072</option>
		<option value="586">FRENCH INSURANCE AGENCY | 6283-3240</option>
		<option value="51">FRIENDSHIP INSURANCE | 6046-2545</option>
		<option value="703">FRIENDSHIP INSURANCE | 6046-3357</option>
		<option value="969">G.A. MACDONALD ASSOCIATES INC. | 6475-3551</option>
		<option value="99">G.A. MACDONALD ASSOCIATES, INC. | 6092-2251 (closed)</option>
		<option value="747">GADBERRY INSURANCE | 6380-3400 (closed)</option>
		<option value="275">GANGWER GALLIPO INSURANCE AGENCY | 6525-2203</option>
		<option value="276">GANGWER INS. AG. INC. | 6525-2525</option>
		<option value="277">GANGWER INS. AG. INC. | 6525-2526</option>
		<option value="278">GANGWER INS. AG. INC. | 6525-2527</option>
		<option value="649">GANGWER INSURANCE AGENCY | 6525-3303</option>
		<option value="279">GANGWER POWERS INSURANCE AGENCY | 6525-2877</option>
		<option value="887">GEORGE INSURANCE GROUP | 6438-3505</option>
		<option value="304">GERMAN AMERICAN INSURANCE | 6603-2554</option>
		<option value="420">GERMAN MUTUAL INSURANCE AGENCY | 6999-2655</option>
		<option value="760">GETTELFINGER INSURANCE | 6386-3414</option>
		<option value="773">GILL INSURANCE ADVISOR'S INC. | 6394-3427</option>
		<option value="52">GLEN FURR AGCY | 6047-1044</option>
		<option value="696">GLOBALNET INSURANCE LLC | 6345-3350</option>
		<option value="562">GMI AGENCY, LLC | 6271-3216</option>
		<option value="228">GODSCHALK &amp; ASSOCIATES, INC. | 6366-2366</option>
		<option value="730">GOODING BROWN &amp; COMPANY, INC. | 6360-3383</option>
		<option value="61">GRABER INS INC | 6055-2055</option>
		<option value="770">GRANVILLE INSURANCE, LLC | 6390-3423</option>
		<option value="715">GREDY &amp; EVANS INSURANCE, INC. | 6756-3369</option>
		<option value="362">GREDY INSURANCE AGENCY, INC. | 6756-2756</option>
		<option value="971">GREGORY &amp; APPEL INC | 6477-3552</option>
		<option value="953">GTPS INSURANCE AGENCY | 6468-5064</option>
		<option value="823">GUTWEIN &amp; RISNER INSURANCE AGENCY | 6291-3477</option>
		<option value="599">GUTWEIN &amp; RISNER INSURANCE AGENCY | 6291-3252</option>
		<option value="798">GUTWEIN &amp; RISNER WHITE CO INSURANCE AGENCY | 6291-3452</option>
		<option value="55">GUTWEIN INS. &amp; FINANCIAL SERVICES | 6050-2615</option>
		<option value="56">GUTWEIN-KOOY INSURANCE | 6050-2920</option>
		<option value="149">H.J. SPIER CO. INC. | 6139-2342</option>
		<option value="57">HADLEY INSURANCE, INC. | 6051-2413</option>
		<option value="248">HAGUE INSURANCE AGENCY | 6396-2831</option>
		<option value="249">HAGUE INSURANCE AGENCY | 6396-2832</option>
		<option value="764">HANNUM INSURANCE | 6314-3418 (closed)</option>
		<option value="977">HANNUM INSURANCE | 6480-3556</option>
		<option value="509">HARDY INSURANCE GROUP, INC. | 6238-3162</option>
		<option value="804">HARRINGTON-HOCH, INC. | 6411-3458</option>
		<option value="63">HARRIS INSURANCE AGENCY | 6056-2122</option>
		<option value="64">HARRITT INSURANCE INC. | 6057-1191</option>
		<option value="891">HEARTLAND INSURANCE PARTNERS LLC | 6441-3509</option>
		<option value="280">HEATON INSURANCE, INC. | 6533-2533</option>
		<option value="207">HECKAMAN INSURANCE AGENCY, INC. | 6292-2292</option>
		<option value="310">HENTHORN AGENCY, INC. | 6620-2620</option>
		<option value="455">HENTHORN SAPPENFIELD AGENCY, LLC | 6620-2951</option>
		<option value="751">HITCHCOCK AGENCY | 6382-3405</option>
		<option value="68">HOBSON INSURANCE AGENCY, LLC | 6061-1918</option>
		<option value="70">HOCH INSURANCE AGENCY INC. | 6062-2324</option>
		<option value="968">HOLMAN-DAHMS INC | 6474-5069</option>
		<option value="419">HOME INSURANCE AGENCY | 6999-1999</option>
		<option value="911">HOMETOWN INSURANCE | 6451-3519</option>
		<option value="71">HOOSIER ASSOCIATES, INC. | 6063-2380</option>
		<option value="769">HOOSIER FAMILY INSURANCE | 6391-3424</option>
		<option value="699">HOOSIER INSURANCE AGENCY | 6347-3353</option>
		<option value="72">HOSTETLER &amp; YOUNG INSURANCE AGENCY | 6064-2215</option>
		<option value="73">HOWARD INSURANCE INC. | 6065-2350</option>
		<option value="820">HOWARD WEBB INS. &amp; REAL ESTATE INC. | 6414-3474</option>
		<option value="446">HOWE INSURANCE AGENCY | 6206-2942</option>
		<option value="682">HOWE INSURANCE SERVICES | 6330-3336</option>
		<option value="758">HOWE-LAGRANGE INSURANCE AGENCY | 6384-3412</option>
		<option value="757">HOWE-LAGRANGE-SILVEUS INS. AG. | 6384-3413</option>
		<option value="707">HUGHES INSURANCE GROUP | 6260-3361</option>
		<option value="783">HUMMEL GROUP INC. | 6200-3436</option>
		<option value="913">HUMMEL GROUP INC. | 6200-3520</option>
		<option value="45">HUMMEL GROUP INC. | 6041-2407</option>
		<option value="161">HUMMEL INSURANCE INC. | 6148-2151</option>
		<option value="160">HUMMEL WINTERS INSURANCE | 6148-2148</option>
		<option value="162">HUMMEL-VEVAY INSURANCE INC. | 6148-2332</option>
		<option value="881">HUPE INSURANCE SERVICES | 6244-3500</option>
		<option value="395">IMEL INSURANCE AGENCY INC. | 6787-2787</option>
		<option value="529">IMG CRIMANS INSURANCE AGENCY | 6252-3182</option>
		<option value="940">IMMING INSURANCE AGENCY | 6462-5056</option>
		<option value="828">INDIAN CREEK INSURANCE | 6393-3482</option>
		<option value="954">INSURANCE BROKER DIRECT INC. | 6465-3548</option>
		<option value="904">INSURANCE CENTER | 6447-5043</option>
		<option value="527">INSURANCE MANAGEMENT GROUP | 6252-3180</option>
		<option value="862">INSURANCE PROVIDERS GROUP OF G.C., INC. | 6428-5023</option>
		<option value="859">INSURANCE PROVIDERS GROUP OF IL, LLC | 6428-5020</option>
		<option value="860">INSURANCE PROVIDERS GROUP OF IL, LLC | 6428-5021</option>
		<option value="861">INSURANCE PROVIDERS GROUP OF IL, LLC | 6428-5022</option>
		<option value="946">INSURANCE PROVIDERS GROUP OF MAHOMET LLC | 6428-5058</option>
		<option value="565">INSURANCE SERVICES, INC. | 6273-3219</option>
		<option value="788">INSURANCE TRUSTEES, INC. | 6382-3443</option>
		<option value="791">INSURERS INC. | 6353-3445</option>
		<option value="731">INTEGRITY INSURANCE &amp; ASSOC. LLC | 6361-3384</option>
		<option value="621">ISU MEEKS INSURANCE | 6250-3275</option>
		<option value="652">ISU-BRIGHT AGENCY | 6069-3306</option>
		<option value="76">ISU-BRIGHT AGENCY | 6069-2346</option>
		<option value="807">J &amp; S FARMERS INSURANCE AGENCY | 6273-3461</option>
		<option value="117">JACK OGREN &amp; CO., INC. | 6109-2531</option>
		<option value="894">JAMES UNLAND &amp; CO INC | 6443-5037</option>
		<option value="893">JAMES UNLAND &amp; COMPANY INC. | 6443-5036</option>
		<option value="477">JEFFREY A. KLEINSCHMIDT, INC. | 6224-2974</option>
		<option value="65">JEFFRIES BROOKSTON INSURANCE | 6058-2443</option>
		<option value="227">JENNINGS INS. AG. INC. | 6362-2405</option>
		<option value="226">JENNINGS INS. AGENCY, INC. | 6362-2363</option>
		<option value="225">JENNINGS INS. AGENCY, INC./ARGOS | 6362-2362</option>
		<option value="80">JENSEN FORD INSURANCE | 6073-3088</option>
		<option value="89">JIM KITCHELL INSURANCE AGENCY | 6082-1002</option>
		<option value="624">JIM LAWRENCE INSURANCE AGENCY | 6310-3278</option>
		<option value="394">JOHNSON INSURANCE AGENCY | 6785-2917</option>
		<option value="462">JOHNSON-WITKEMPER INC. | 6213-2959</option>
		<option value="229">KENDALLVILLE INSURANCE | 6366-2791</option>
		<option value="335">KEY HENSON JACKSON INSURANCE | 6712-2716</option>
		<option value="392">KFG INSURANCE AGENCY LLC | 6785-2785</option>
		<option value="426">KFG INSURANCE AGENCY, LLC | 6785-2786 (closed)</option>
		<option value="86">KILLINGBECK INSURANCE, INC. | 6079-2626</option>
		<option value="87">KINCAID INSURANCE GROUP, INC. | 6080-2720</option>
		<option value="88">KINDELL INSURANCE SERVICES, LLC | 6081-2693</option>
		<option value="90">KLEMME INSURANCE SERVICES, INC. | 6083-2874</option>
		<option value="463">KLING FAMILY INSURANCE, INC. | 6214-2960</option>
		<option value="91">KNAPP-MILLER-BROWN INSURANCE | 6084-2685</option>
		<option value="815">KNAPP-MILLER-BROWN INSURANCE | 6084-3469</option>
		<option value="487">KNEBEL INSURANCE AGENCY | 6082-2984</option>
		<option value="282">KOCHERT INSURANCE | 6549-1549</option>
		<option value="284">KOCHERT INSURANCE | 6549-2828</option>
		<option value="318">KOZLOWSKI &amp; ASSOCIATES | 6642-2642</option>
		<option value="320">KOZLOWSKI &amp; ASSOCIATES | 6642-2896</option>
		<option value="704">KUNKEL INSURANCE AGENCY, INC. | 6349-3358</option>
		<option value="743">L.E. &amp; BRET BROWN INS. AGENCY, INC. | 6375-3397</option>
		<option value="250">LAPORTE INSURANCE AGENCY | 6396-2833</option>
		<option value="93">LARKEY INSURANCE &amp; REAL ESTATE,INC. | 6086-2067</option>
		<option value="909">LAYMAN HUMMEL INSURANCE INC | 6148-3517</option>
		<option value="199">LEAKEY INSURANCE AGENCY, INC. | 6277-2277</option>
		<option value="948">LEDGESTONE INSURANCE GROUP | 6466-5060</option>
		<option value="970">LELAND SMITH INSURANCE SERVICES INC | 6476-7007</option>
		<option value="973">LIGHTHOUSE INSURANCE AGENCY | 6479-5071</option>
		<option value="864">LILLY OPTIMUM SOLUTION | 6241-3494</option>
		<option value="933">LINCOLNWAY INSURANCE SERVICES INC. | 6334-3531</option>
		<option value="686">LINCOLNWAY INSURANCE SERVICES, INC. | 6334-3340</option>
		<option value="945">LINDENMEYER INSURANCE AGENCY | 6422-5057</option>
		<option value="558">LLOYD INSURANCE, INC. | 6269-3212</option>
		<option value="792">LOGAN LAVELLE HUNT INSURANCE AGENCY, LLC | 6353-3446</option>
		<option value="947">LOMAN-RAY INSURANCE GROUP LLC | 6458-5059</option>
		<option value="960">LOMAN-RAY INSURANCE GROUP LLC | 6458-5066</option>
		<option value="961">LOMAN-RAY INSURANCE GROUP LLC | 6458-5067</option>
		<option value="926">LOMAN-RAY INSURANCE GROUP LLC | 6458-5049</option>
		<option value="927">LOMAN-RAY INSURANCE GROUP LLC | 6458-5050</option>
		<option value="928">LOMAN-RAY INSURANCE GROUP LLC | 6458-5051</option>
		<option value="929">LOMAN-RAY INSURANCE GROUP LLC | 6458-5052</option>
		<option value="930">LOMAN-RAY INSURANCE GROUP LLC | 6458-5053</option>
		<option value="931">LOMAN-RAY INSURANCE GROUP LLC | 6458-5054</option>
		<option value="932">LOMAN-RAY INSURANCE GROUP LLC | 6458-5055</option>
		<option value="964">LOMAN-RAY INSURANCE GROUP LLC | 6458-5068</option>
		<option value="95">LUMP INSURANCE AGENCY, INC. | 6088-2098</option>
		<option value="219">M W INSURANCE AGENCY | 6356-2356</option>
		<option value="811">MAKI INSURANCE GROUP LLC | 6335-3465</option>
		<option value="592">MAPLECREST INSURANCE, LLC | 6287-3245</option>
		<option value="817">MARTIN-SERRIN COMPANY, INC. | 6412-3471</option>
		<option value="563">MATCHETT &amp; WARD INSURANCE | 6272-3217</option>
		<option value="681">MAX ADAMS INSURANCE AGENCY INC. | 6329-3335</option>
		<option value="570">MAY-WOLF INSURANCE | 6274-3224</option>
		<option value="209">MCCORMICK INS AGENCY, INC. | 6301-2301</option>
		<option value="910">MCGOWAN INSURANCE GROUP LLC | 6439-3518</option>
		<option value="888">MCGOWAN INSURANCE GROUP LLC | 6439-3506</option>
		<option value="889">MCGOWAN INSURANCE GROUP LLC | 6439-3507</option>
		<option value="890">MCGOWAN INSURANCE GROUP LLC | 6439-3508</option>
		<option value="98">MCQUISTON AGCY INC | 6091-1005</option>
		<option value="163">MEISBERGER AGENCY | 6148-2489</option>
		<option value="951">MENTOR INSURANCE LLC | 6467-3547</option>
		<option value="490">MERRILL AND MERRILL INSURANCE INC. | 6211-2987</option>
		<option value="102">MERRITT HALL ENTERPRISES, INC. | 6095-2479</option>
		<option value="323">METTLER AGENCY, INC. | 6666-2666</option>
		<option value="533">MG MYERS INSURANCE, LLC | 6256-3186</option>
		<option value="664">MICHIANA INSURANCE SERVICES INC | 6096-3318</option>
		<option value="752">MICHIANA INSURANCE SERVICES OF PLYMOUTH, LLC | 6096-3406</option>
		<option value="866">MICHIANA INSURANCE SERVICES, INC. | 6096-3498</option>
		<option value="481">MICHIANA INSURANCE SERVICES, INC. | 6096-2978</option>
		<option value="103">MICHIANA INSURANCE SERVICES, INC. | 6096-2541</option>
		<option value="950">MID AMERICA INSURANCE | 6466-5062</option>
		<option value="688">MIDWEST FAMILY INSURANCE | 6336-3342 (closed)</option>
		<option value="784">MILLER INSURANCE GROUP | 6401-3438</option>
		<option value="785">MILLER INSURANCE GROUP | 6401-3439</option>
		<option value="786">MILLER INSURANCE GROUP | 6401-3440</option>
		<option value="883">MILLER INSURANCE GROUP | 6401-3501</option>
		<option value="814">MJS INSURANCE INC. | 6001-3468</option>
		<option value="723">MOORE &amp; SHEPHERD INSURANCE | 6353-3377</option>
		<option value="754">MORGAN INSURANCE GROUP | 6270-3408</option>
		<option value="560">MORGAN INSURANCE GROUP | 6270-3214</option>
		<option value="561">MORGAN INSURANCE GROUP | 6270-3215</option>
		<option value="944">MORROW INSURANCE | 6401-3543</option>
		<option value="107">MORROW INSURANCE AGENCY | 6099-2569 (closed)</option>
		<option value="597">MURPHY INSURANCE, INC. | 6290-3250</option>
		<option value="875">MUSSO INSURANCE AGENCY LLC | 6432-5030</option>
		<option value="110">MUTUAL HOME INSURANCE AGENCY | 6102-2547</option>
		<option value="501">MYERS INSURANCE &amp; REAL ESTATE LLC | 6103-2998</option>
		<option value="827">NFP PROPERTY &amp; CASUALTY SERVICES INC. | 6416-3481</option>
		<option value="824">NFP PROPERTY &amp; CASUALTY SERVICES INC. | 6416-3478</option>
		<option value="822">NFP PROPERTY &amp; CASUALTY SERVICES, INC. | 6416-3476</option>
		<option value="623">NICHOLS INSURANCE AGENCY, INC. | 6309-3277</option>
		<option value="113">NIETERT INS. &amp; FIN. SERVICES INC. | 6105-2577</option>
		<option value="594">NORTHEAST INSURANCE GROUP, LLC | 6290-3247</option>
		<option value="922">NSB INSURANCE &amp; RISK MANAGEMENT | 6438-3527</option>
		<option value="778">ODYSSEY INSURANCE GROUP | 6399-3432</option>
		<option value="871">OGDEN INSURANCE AGENCY, INC. | 6432-5026</option>
		<option value="521">OLINGER INSURANCE AGENCY, INC. | 6248-3174</option>
		<option value="937">ONEINDIANA PROPERTY &amp; CASUALTY LLC | 6297-3535</option>
		<option value="936">OSUNA-ROLAND &amp; ASSOCIATES INC. | 6461-3534</option>
		<option value="800">OVATION INSURANCE | 6405-3454</option>
		<option value="796">OVATION INSURANCE | 6403-3450 (closed)</option>
		<option value="727">OWENS INSURANCE AGENCY | 6355-3380</option>
		<option value="744">PAM COOK INSURANCE LLC | 6378-3398</option>
		<option value="118">PARKER GROUP | 6110-2703</option>
		<option value="545">PATOKA INSURANCE CENTER, INC. | 6261-3199</option>
		<option value="749">PATRIOT INSURANCE | 6382-3403</option>
		<option value="746">PATRIOT INSURANCE &amp; RISK MANAGEMENT | 6379-3399 (closed)</option>
		<option value="844">PAYNE INS. AGENCY A COMPASS INSURANCE PARTNER | 6422-5008</option>
		<option value="905">PECK &amp; WOOD INSURANCE | 6449-3515</option>
		<option value="620">PEMBERTON INSURANCE AND BENEFITS | 6250-3274</option>
		<option value="719">PENCE BROOKS BOLANDER &amp; SHEPHERD | 6353-3373</option>
		<option value="120">PETERS INS. &amp; R.E. AG., INC. | 6112-2485</option>
		<option value="874">PETERSON INS. SERVS INC. DBA MCAFEE INS. SERVICES | 6432-5029</option>
		<option value="873">PETERSON INSURANCE SERVICES, INC. | 6432-5028</option>
		<option value="655">PGH INSURANCE GROUP LLC | 6316-3309</option>
		<option value="958">PHELAN INSURANCE AGENCY INC. | 6472-7001</option>
		<option value="689">PILOT INSURANCE AGENCY INC. | 6337-3343</option>
		<option value="916">PINNACLE INSURANCE GROUP OF INDIANA INC. | 6454-3522</option>
		<option value="917">PINNACLE INSURANCE GROUP OF INDIANA INC. | 6454-3523</option>
		<option value="128">PORTLAND INSURANCE | 6116-1627</option>
		<option value="957">PREFERRED INSURANCE CENTER | 6471-7000</option>
		<option value="918">PREFERRED INSURANCE SOLUTIONS INC. | 6455-3524</option>
		<option value="914">PRESTON INSURANCE AGENCY ILLINOIS LLC | 6453-5047</option>
		<option value="923">PRIORITY RISK MANAGEMENT | 6457-3528</option>
		<option value="924">PRIORITY RISK MANAGEMENT | 6457-3529</option>
		<option value="534">R&amp;R INS AGENCY, A DIVISION OF MG MYERS INS. | 6256-3187</option>
		<option value="734">R. MYERS INSURANCE | 6365-3387</option>
		<option value="582">R.J. FUHS AGENCY | 6261-3236</option>
		<option value="854">RASLER INSURANCE SERVICES LLC | 6297-3491</option>
		<option value="865">RAWLINGS AND WYATT INSURANCE | 6381-3495</option>
		<option value="748">RAWLINGS INSURANCE | 6381-3402</option>
		<option value="777">RAYLS INSURANCE AGENCY | 6353-3431</option>
		<option value="767">RAYMOND &amp; SPENCE INSURANCE GROUP, LLC | 6297-3421</option>
		<option value="819">RAYMOND &amp; SPENCE INSURANCE GROUP, LLC | 6297-3473</option>
		<option value="925">RAYMOND AND SPENCE INSURANCE GROUP LLC | 6297-3530</option>
		<option value="617">REALTY GROUP INSURANCE | 6304-3270</option>
		<option value="397">REES &amp; COMP INSURANCE, INC. | 6805-1805</option>
		<option value="759">REICK INSURANCE AGENCY | 6384-3411</option>
		<option value="892">RELIABLE INSURANCE SOLUTIONS INC | 6442-3510</option>
		<option value="132">RELIANCE INSURANCE AGENCY | 6121-2436 (closed)</option>
		<option value="660">RICHARD BROWN INSURANCE | 6129-3315</option>
		<option value="133">RITCHIE INSURANCE, INC | 6122-1153</option>
		<option value="359">RIVER VALLEY INSURANCE | 6749-2749</option>
		<option value="198">RME INSURANCE | 6257-2500</option>
		<option value="196">RME INSURANCE | 6257-2256</option>
		<option value="761">ROBERTSON INSURANCE &amp; FINANCIAL, LLC | 6225-3415</option>
		<option value="369">ROBLEY INSURANCE SERVICES | 6770-2770</option>
		<option value="842">ROGER SCHULDT INS. AG. COMPASS INSURANCE PARTNER | 6422-5006</option>
		<option value="134">ROSEMEYER AGENCY | 6123-2842</option>
		<option value="504">RT INSURANCE | 6001-3004</option>
		<option value="813">RT INSURANCE LLC | 6001-3467</option>
		<option value="530">SCHATZ INSURANCE AGENCY, INC. | 6253-3183</option>
		<option value="531">SCHMUTZLER AGENCY, INC. | 6254-3184</option>
		<option value="507">SCHULTHEIS AGENCY, INC. | 6236-3160</option>
		<option value="578">SCHULTHEIS INSURANCE AGENCY, INC. | 6395-3232</option>
		<option value="247">SCHULTHEIS INSURANCE AGENCY, INC. | 6395-2395</option>
		<option value="246">SCHULTHEIS-MT. VERNON | 6395-2320</option>
		<option value="522">SECURITY FIRST INSURANCE SERVICE | 6712-3175</option>
		<option value="256">SECURITY INS., INC. | 6440-2440</option>
		<option value="139">SELECT INS. SERVICES, INC. | 6129-2392</option>
		<option value="879">SHELBYVILLE INSURANCE SERVICES | 6436-5034</option>
		<option value="816">SHEPHERD INSURANCE | 6353-3470</option>
		<option value="724">SHEPHERD INSURANCE | 6353-3378</option>
		<option value="720">SHEPHERD INSURANCE | 6353-3374</option>
		<option value="721">SHEPHERD INSURANCE | 6353-3375</option>
		<option value="722">SHEPHERD INSURANCE | 6353-3376</option>
		<option value="717">SHEPHERD INSURANCE | 6353-3371</option>
		<option value="718">SHEPHERD INSURANCE | 6353-3372</option>
		<option value="716">SHEPHERD INSURANCE LLC | 6353-3370</option>
		<option value="442">SHIRER INSURANCE SERVICES, LLC | 6204-2938</option>
		<option value="503">SIEKMAN-STEARNS-GREGORY INSURANCE | 6148-3001</option>
		<option value="899">SMITH INSURANCE SERVICES | 6314-3513 (closed)</option>
		<option value="978">SMITH INSURANCE SERVICES | 6480-3557</option>
		<option value="619">SMITH INSURANCE, INC. | 6307-3273</option>
		<option value="454">SOUTHEASTERN INSURANCE SERVICES | 6210-2950</option>
		<option value="576">SPITZ &amp; MILLER ADVANTAGE INSURANCE AGENCY | 6203-3230</option>
		<option value="569">SPRINGER-SPRINGER INSURANCE | 6141-3223</option>
		<option value="547">SPRINGER-SPRINGER INSURANCE | 6141-3201</option>
		<option value="151">SPRINGER-SPRINGER INSURANCE | 6141-2539</option>
		<option value="483">SPRINGMEYER FAMILY INSURANCE | 6227-2980</option>
		<option value="972">SQUIER INSURANCE AGENCY INC | 6478-5070</option>
		<option value="489">ST. JOSEPH AGENCY | 6230-2986 (closed)</option>
		<option value="297">STANLEY JONES AG INC | 6580-1580</option>
		<option value="962">STAR INSURANCE AGENCY | 6382-3550</option>
		<option value="885">STEWART, BRIMNER, PETERS &amp; CO. INC. | 6438-3503</option>
		<option value="886">STEWART, BRIMNER, PETERS &amp; LEAR | 6438-3504</option>
		<option value="857">SUMMER &amp; ASSOCIATES LLC A DIVISION OF TROXELL | 6426-5018</option>
		<option value="974">SUPERIOR INSURANCE PARTNERS LLC | 6480-3553</option>
		<option value="156">SURFACE INS. AGENCY, INC. | 6145-3137</option>
		<option value="725">SYCAMORE INSURANCE ASSOCIATES LLC | 6374-3393</option>
		<option value="574">SYNERGY INSURANCE GROUP | 6279-3228</option>
		<option value="668">TANNER &amp; ASSOCIATES INSURANCE, INC. | 6320-3322</option>
		<option value="445">TANNER &amp; SERVIES INSURANCE | 6619-2941</option>
		<option value="308">TANNER &amp; SERVIES INSURANCE | 6619-2619</option>
		<option value="934">TATEM &amp; ASSOCIATES | 6308-3532</option>
		<option value="684">TC &amp; SONS INSURANCE SERVICES | 6332-3338</option>
		<option value="334">THE BROWN INSURANCE AGCY | 6712-2715</option>
		<option value="333">THE BROWN INSURANCE AGENCY | 6712-2714</option>
		<option value="141">THE BROWNFIELD INSURANCE AGENCY LLC | 6131-2653</option>
		<option value="19">THE BUDD AGENCY, INC. | 6015-2119</option>
		<option value="907">THE CORNERSTONE AGENCY, INC. | 6450-5044</option>
		<option value="550">THE CRITCHLOW AGENCY | 6263-3204</option>
		<option value="753">THE DEHAYES GROUP | 6383-3407</option>
		<option value="733">THE GOODMAN INSURANCE AGENCY LLC | 6364-3386 (closed)</option>
		<option value="591">THE HOOSIER INSURANCE AGENCY | 6359-3255</option>
		<option value="265">THE INSURANCE SHOP, INC. | 6463-2463</option>
		<option value="766">THE PURCIFULL AGENCY | 6388-3420</option>
		<option value="610">THE RALSTON GROUP, LLC | 6298-3264</option>
		<option value="980">THE SMITH SAWYER SMITH AGENCY | 6483-3559</option>
		<option value="979">THE SMITH SAWYER SMITH AGENCY LLC | 6483-3558</option>
		<option value="735">THE WRIGHT INSURANCE COMPANY | 6367-3388</option>
		<option value="920">THORN CREEK INSURANCE SERVICES INC. | 6456-5048</option>
		<option value="251">THORNE INSURANCE | 6410-2410</option>
		<option value="584">THORNE INSURANCE AGENCY NORTH MANCHESTER, INC. | 6410-3238</option>
		<option value="253">THORNE INSURANCE AGENCY WABASH INC. | 6410-2923</option>
		<option value="525">THORNE INSURANCE AGENCY WARSAW INC. | 6410-3178</option>
		<option value="742">THORNE INSURANCE WALTHER AGENCY | 6410-3396</option>
		<option value="702">TILLINGHAST INSURANCE AGENCY | 6348-3356</option>
		<option value="121">TOM PLUMMER AGENCY | 6113-2414</option>
		<option value="739">TOM STECKLER AGENCY INC. | 6371-3392</option>
		<option value="626">TOM WALLACE INSURANCE AGENCY, LLC | 6297-3280</option>
		<option value="157">TREVINO INSURANCE GROUP, INC. | 6146-2739</option>
		<option value="797">TRIPP INSURANCE | 6404-3451</option>
		<option value="856">TROXELL | 6426-5017</option>
		<option value="366">UEBER INSURANCE, INC. | 6768-2768</option>
		<option value="863">UNDERWOOD AGENCY | 6429-3493</option>
		<option value="677">UNDERWRITERS ALLIANCE OF INDIANA, INC. | 6325-3331</option>
		<option value="158">UPHAUS INSURANCE AGENCY | 6147-1854</option>
		<option value="672">USI INSURANCE SERVICES LLC | 6312-3326</option>
		<option value="628">USI INSURANCE SERVICES LLC | 6312-3282</option>
		<option value="661">VALLEY INSURANCE | 6318-3310</option>
		<option value="166">VALPARAISO FIRST INSURANCE INC. | 6150-2705</option>
		<option value="882">VOLDICO INSURANCE - FREEDOM AGENCY | 6313-5035</option>
		<option value="774">VOLDICO INSURANCE - GREG NIESE AGENCY | 6313-3428</option>
		<option value="755">VOLDICO INSURANCE - NATE MCKEON AGENCY | 6313-3409</option>
		<option value="638">VOLDICO INSURANCE-BRENT TURNER AGENCY | 6313-3292</option>
		<option value="631">VOLDICO INSURANCE-CHANDA GATTON AGENCY | 6313-3285</option>
		<option value="647">VOLDICO INSURANCE-ERIC ROHLS AGENCY | 6313-3301</option>
		<option value="633">VOLDICO INSURANCE-JAY WHITEHEAD AGENCY | 6313-3287</option>
		<option value="632">VOLDICO INSURANCE-LINTON AGENCY | 6313-3286</option>
		<option value="634">VOLDICO INSURANCE-MICHELLE BOYD AGENCY | 6313-3288</option>
		<option value="646">VOLDICO INSURANCE-PAUL BRANHAM AGENCY | 6313-3300</option>
		<option value="643">VOLDICO INSURANCE-STEVE STOCK AGENCY | 6313-3297</option>
		<option value="640">VOLDICO INSURANCE-TEKULVE-VANKIRK AGENCY | 6313-3294</option>
		<option value="630">VOLDICO, LLC | 6313-3284</option>
		<option value="523">W&amp;F - BURTON | 6250-3176</option>
		<option value="66">W.C. HESS INSURANCE INC. | 6059-2042</option>
		<option value="58">W.R. HALL INSURANCE GROUP | 6052-2664</option>
		<option value="714">W.R. SLAUGHTER AGENCY | 6352-3368</option>
		<option value="825">WALKERHUGHES GROUP, LLC | 6417-3479</option>
		<option value="829">WALKERHUGHES INSURANCE | 6346-3483</option>
		<option value="831">WALKERHUGHES INSURANCE | 6346-3485</option>
		<option value="849">WALKERHUGHES INSURANCE | 6260-3490</option>
		<option value="789">WALKERHUGHES INSURANCE | 6346-3444</option>
		<option value="708">WALKERHUGHES INSURANCE | 6260-3362</option>
		<option value="709">WALKERHUGHES INSURANCE | 6260-3363</option>
		<option value="697">WALKERHUGHES INSURANCE | 6346-3351</option>
		<option value="571">WALKERHUGHES INSURANCE | 6260-3225</option>
		<option value="552">WALKERHUGHES INSURANCE | 6260-3206</option>
		<option value="671">WALKERHUGHES INSURANCE | 6260-3325</option>
		<option value="625">WALKERHUGHES INSURANCE | 6260-3279</option>
		<option value="609">WALKERHUGHES INSURANCE | 6260-3263</option>
		<option value="790">WALLS INSURANCE GROUP LLC | 6402-3442</option>
		<option value="577">WALTER COOK INSURANCE AGENCY, INC. | 6024-3231</option>
		<option value="27">WALTER COOK INSURANCE AGENCY, INC. | 6024-2613</option>
		<option value="482">WALTERS AND GROUNDS INSURANCE AGENCY | 6226-2979</option>
		<option value="663">WALTERS AND GROUNDS INSURANCE AGENCY | 6226-3317</option>
		<option value="170">WATSON INSURANCE, INC. | 6154-2511</option>
		<option value="906">WAYNE FERGUSON AGENCY | 6141-3516</option>
		<option value="694">WEATHERVANE INSURANCE SOLUTIONS | 6343-3348</option>
		<option value="590">WEBSTER INSURANCE AGENCY | 6286-3244 (closed)</option>
		<option value="903">WEIS INSURANCE AGENCY LLC | 6446-5042</option>
		<option value="526">WELDY INSURANCE AGENCY | 6156-3179</option>
		<option value="220">WENINGER INSURANCE AGENCY | 6356-2530</option>
		<option value="551">WILBUR KAHLE AGENCY | 6261-3205</option>
		<option value="174">WILKINSON INSURANCE AGENCY | 6158-2441</option>
		<option value="963">WILLIS MURPHY ADVANTAGE INSURANCE AGENCY | 6203-7003</option>
		<option value="175">WILLSEY AGENCY, INC. | 6159-2382</option>
		<option value="439">WILSON LAWSON MYERS INSURANCE AGENCY | 6203-2936</option>
		<option value="695">WINCHELL INSURANCE GROUP | 6344-3349</option>
		<option value="895">WINTER INSURANCE | 6443-5038</option>
		<option value="877">WINTERS LLP | 6435-5032</option>
		<option value="448">WITKEMPER INSURANCE GROUP | 6208-2944</option>
		<option value="470">WORMAN-LIGHTFOOT-HOCH INSURANCE | 6062-2967</option>
		<option value="908">ZOBRIST INSURANCE | 6443-5045</option>
		<option selected="selected" value="null">Show All</option>
 </select>--%>

    <asp:HiddenField ID="hdnMyVRView" Value="splash" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="<%=ResolveClientUrl("~/js/QuoteSearchResults.js")%>"></script>
   <style>
       .myVRSplash{
           margin-top: 50px;
       }

       .myVRSplashButtonsContainer{
           text-align:center;
           margin:auto;
       }

       .myVRButton {
           display:inline-block;
           width:175px;
           height:250px;
           border-radius:20px;
           border:1px solid white;
           box-shadow:5px 10px 5px #aaaaaa;
           padding-left:10px;
           padding-right:10px;
           margin-top:10px;
           margin-left:8px;
           margin-right:8px;
           vertical-align:top;
           color:white;
           text-decoration:none;
       }

       .myVRButtonContainer{
           height:100%;
           position:relative;
       }

       .myVRButtonHeader{
           text-align:center;
           font-size: 15px;
           margin-top:20px;
           margin-bottom:20px;
       }

       .myVRButtonContent{
           margin-top: 10px;
           text-align:left;
       }

       .myVRButtonContentNote{
           position:absolute;
           bottom:23px;
           text-align:left;
       }

       .hrWhiteLine{
           border: 1px solid white;
       }
   </style>
</asp:Content>