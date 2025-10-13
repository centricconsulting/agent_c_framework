<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPayPlanOptions.ascx.vb" Inherits="IFM.VR.Web.ctlPayPlanOptions" %>

<style>
    .PayPlanAmounts{
        float:right;
        margin-right: 25px;
    }
    .RadioButtonError{
        /*color:red;*/
        /*background-color: red;*/
        -moz-box-shadow: 0px 0px 6px red;
        -webkit-box-shadow: 0px 0px 6px red;
        box-shadow: 0px 0px 6px red;
        -webkit-border-radius: 6px;
        -moz-border-radius: 6px;
        border-radius: 6px; 
    }
    .PayPlanStandardButton{
        border: 1px solid #797777;
        color: White;
        background-color: #797777;
        -moz-border-radius: 4px;
        -webkit-border-radius: 4px;
        border-radius: 4px;
        height: 15px;
    }
    .PayPlanMessage{
        text-align: center;
        padding: 25px;
        color:blue;
    }
</style>

<script>
    $(document).ready(function () {
        var dialog
        $("#dvPopupErrorMessage").hide();

        function processRequest() {
            $("#dvPopupErrorMessage").hide();
            var checkedControl = $("input[type=radio][name$='popupPayPlanOptions']:checked");
            if (checkedControl) {
                var changeButtons = false;
                var checkedControlID = checkedControl.attr("id");
                switch (checkedControlID) {
                    case "<%= Me.rbNoRate.ClientID%>":
                        dialog.dialog("close");
                        break;
                    case "<%= Me.rbRate.ClientID%>":
                        $("#<%= Me.btnRate.ClientID%>").click();
                        changeButtons = true;
                        break;
                    case "<%= Me.rbRateAndPrintFriendly.ClientID%>":
                        $("#<%= Me.btnRateAndDisplayPrintFriendly.ClientID%>").click();
                        changeButtons = true;
                        break;
                    default:
                        $("input[type=radio][name$='popupPayPlanOptions']").each(function () { $(this).addClass("RadioButtonError") });
                        $("#dvPopupErrorMessage").show();
                        break;
                }
                if (changeButtons === true) {
                    $(".ui-dialog-buttonpane button:contains('OK')").attr('disabled', 'disabled');
                    $(".ui-dialog-buttonpane button:contains('OK') span").text("Rating...");
                    $(".ui-dialog-buttonpane button:contains('Cancel') span").hide();
                }
            }
        }

        function newSelectionIsNotCurrentlySelectedPayPlan(selectedRadioControlID) {
            var returnVar = true;
            if (selectedRadioControlID) {
                var currentlySelectedPayPlanID = $("#<%= Me.hfCurrentPayPlanID.ClientID%>").val()
                var newPayPlanID = $("#" + selectedRadioControlID).parent().data("payplanid");
                var currentlySelectedInt = parseInt(currentlySelectedPayPlanID, 10);
                var newInt = parseInt(newPayPlanID, 10);
                if (currentlySelectedInt !== "NaN" && newInt !== "NaN" && currentlySelectedInt > 0 && newInt > 0 && currentlySelectedInt === newInt) {
                    returnVar = false;
                }
            }
            return returnVar;
        }

        dialog = $("#popup-content").dialog({
            autoOpen: false,
            width: 350,
            modal: true,
            dialogClass: 'no-close',
            draggable: false,
            buttons: {
                "OK": processRequest,
                Cancel: function () {
                    $("#dvPopupErrorMessage").hide();
                    dialog.dialog("close");
                }
            },
            close: function () {
                $("input[type=radio][name$='popupPayPlanOptions']").each(function () { $(this).removeClass("RadioButtonError") });
                $("input[type=radio][name$='popupPayPlanOptions']").each(function () { this.checked = false});
            }
        });

        $("input[type=radio][name$='PayPlanOptions']").change(function () {
            newSelectionIsNotCurrentlySelectedPayPlan(this.id) && dialog.dialog("open") && $(".ui-dialog-buttonpane button span").addClass("PayPlanStandardButton");
        });

        if ($("#<%= Me.hfRequestPrintFriendly.ClientID%>").val() === "True") {
            $("#<%= Me.hfRequestPrintFriendly.ClientID%>").val("False");
            window.open("<%=ResolveClientUrl("~/Reports/PPA/PFQuoteSummary.aspx?")%><%= Me.hfQuoteId.Value%>&summarytype=<%= Me.hfSumType.Value%>");
        }
    });
</script>

<div runat="server" id="divPayPlanOptions">
    <h3 id="h3AccordHeader" runat="server">
        <asp:Label ID="lblHeader" runat="server" Text="Pay Plan Options"></asp:Label>
<%--        <span style="float: right;">
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span>--%>
    </h3>
    <div id="divPPOSection" runat="server">
        <div class="informationalText" style="margin-bottom: 10px;">
            Select your preferred pay plan
        </div>
        <div>
            <table style="width: 100%; margin-bottom: 30px">
                <tr>
                    <td>
                        <span class="tblCovNameLabel"><asp:RadioButton Text="Annual:" ID="rbAnnual" GroupName="PayPlanOptions" runat="server" /></span>
                        <span class="PayPlanAmounts tblCovNameLabel"><asp:Label id="lblPayPlanAnnualAmount" runat="server" Text="" /></span>
                    </td>
                    <td>
                        <span class="tblCovNameLabel"><asp:RadioButton Text="Monthly:" ID="rbMonthly" GroupName="PayPlanOptions" runat="server" /></span>
                        <span class="PayPlanAmounts tblCovNameLabel"><asp:Label id="lblPayPlanMonthlyAmount" runat="server" Text="" /></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="tblCovNameLabel"><asp:RadioButton Text="Semi Annual:" ID="rbSemiAnnual" GroupName="PayPlanOptions" runat="server" /></span>
                        <span class="PayPlanAmounts tblCovNameLabel"><asp:Label id="lblPayPlanSemiAnnualAmount" runat="server" Text="" /></span>
                    </td>
                    <td>
                        <span class="tblCovNameLabel"><asp:RadioButton Text="Monthly EFT:" ID="rbMonthlyEFT" GroupName="PayPlanOptions" runat="server" /></span>
                        <span class="PayPlanAmounts tblCovNameLabel"><asp:Label id="lblPayPlanMonthlyEFTAmount" runat="server" Text="" /></span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <span class="tblCovNameLabel"><asp:RadioButton Text="Quarterly:" ID="rbQuarterly" GroupName="PayPlanOptions" runat="server" /></span>
                        <span class="PayPlanAmounts tblCovNameLabel"><asp:Label id="lblPayPlanQuarterlyAmount" runat="server" Text="" /></span>
                    </td>
                    <td runat="server" id="RccOptions" visible="false">
                        <span class="tblCovNameLabel"><asp:RadioButton Text="Recurring Credit Card:" ID="rbMonthlyRCC" GroupName="PayPlanOptions" runat="server" /></span>
                        <span class="PayPlanAmounts tblCovNameLabel"><asp:Label id="lblPayPlayMonthlyRccAmount" runat="server" Text="" /></span>
                    </td>
                </tr>
            </table>
        </div>
        <div class="informationalText" style="font-size:smaller;margin-left:25px;margin-right:25px;">
            The rate reflected for the rated payment plan is accurate. All other payment plans are estimated. Rounding might apply in payment plans with higher frequency.
        </div>
    </div>
    <div id="divPPONotAvailable" runat="server">
        <div class="PayPlanMessage">
            Personal Auto pay plan changes must be made through Billing Updates.
        </div>
    </div>
</div>

<div id="popup-content" title="Re-rating Options">
    <div style="padding:10px;">You have selected a different payment plan.</div>
    <div style="padding-left:15px;"><asp:RadioButton ID="rbNoRate" GroupName="popupPayPlanOptions" runat="server" Text="Do not re-rate" /></div>
    <div style="padding-left:15px;"><asp:RadioButton ID="rbRate" GroupName="popupPayPlanOptions" runat="server" Text="Re-rate only" /></div>
    <div style="padding-left:15px;"><asp:RadioButton ID="rbRateAndPrintFriendly" GroupName="popupPayPlanOptions" runat="server" Text="Re-rate and view the Quote Summary Print" /></div>
    <div style="padding:10px;display:none;color:red;" id="dvPopupErrorMessage">Select a rating option</div>
</div>

<asp:Button ID="btnRate" runat="server" style="display:none;"/>
<asp:Button ID="btnRateAndDisplayPrintFriendly" runat="server" style="display:none;" />
<asp:Button ID="btnDisplayPrintFriendly" runat="server" style="display:none;" />

<asp:HiddenField ID="HiddenField1" Value="0" runat="server" />
<asp:HiddenField ID="hfCurrentPayPlanID" Value="0" runat="server" />
<asp:HiddenField ID="hfRequestPrintFriendly" Value="False" runat="server" />
<asp:HiddenField ID="hfQuoteId" Value="0" runat="server" />
<asp:HiddenField ID="hfSumType" Value="" runat="server" />