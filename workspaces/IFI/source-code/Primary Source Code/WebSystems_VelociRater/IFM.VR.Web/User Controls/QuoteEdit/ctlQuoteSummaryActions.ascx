<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlQuoteSummaryActions.ascx.vb" Inherits="IFM.VR.Web.ctlShowACCORD" %>

<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>
<script type="text/javascript">

    $(document).ready(function () {

        $("#<%=If(IsOnAppPage AndAlso HasLinkedQuotesOrLinkedApps, "divPopupLinkedQuotes", "divPopupAccordPrint")%>").dialog({
            title: "<%=If(IsOnAppPage AndAlso HasLinkedQuotesOrLinkedApps, "Linked Quotes/Applications", "Print " + AcordAppText + "/Finalize App")%>",
            width: <%=If(IsOnAppPage AndAlso HasLinkedQuotesOrLinkedApps, 700, 390)%>,
            height: <%=If(IsOnAppPage AndAlso HasLinkedQuotesOrLinkedApps, 480, If(IsOnAppPage AndAlso Me.Quote IsNot Nothing AndAlso Me.Quote.LobType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm, 180, 130))%>,
            autoOpen: false,
            dialogClass: "no-close",
            open: function (type, data) { $(this).parent().appendTo("form"); }//$(this).parent().css({ "position": "fixed" })
            , close: function () { } 
        });

    });

    function OpenACORDPopup() {
        $("#<%=If(IsOnAppPage AndAlso HasLinkedQuotesOrLinkedApps, "divPopupLinkedQuotes", "divPopupAccordPrint")%>").dialog("open");
    }

    function OpenVehicleDataList() {
        var win = window.open('<%= ResolveUrl("~/Reports/CAP/VehicleData.aspx") %>' + '?quoteid=' + master_quoteIdOrPolicyIdAndImageNum <%=If(IsQuoteEndorsement(), "+ '&EndorsementPolicyIdAndImageNum=' + master_quoteIdOrPolicyIdAndImageNum", "")%>, '_blank');
        //var win = window.open('<%= ResolveUrl("~/Reports/CAP/VehicleData.aspx") %>' + '?quoteid=' + master_quoteID, '_blank');
        //var win = window.open('VehicleData.aspx?quoteid=' + master_quoteID, '_blank');
        if (win) {
            //Browser has allowed it to be opened
            win.opener.focus();
        } else {
            //Browser has blocked it now
            alert('Please allow popups for this website'); 
        }
        return false;
    };

    function FinalizeClick(el) {
        if (el) {
            el.disabled = true;
            el.style.pointerEvents = "none";
            el.text = "Processing...";
        }
    }
</script>

<style>
    .linkedQuotesAppTable {
        width: 100%;
        margin-top: 15px;
        border: 1px solid black;
    }

    .StandardSaveButton {
        margin: 1px 0;
    }
</style>

<div id="divPopupAccordPrint" style="display: none;">
    <center>
        Make sure you print your <%=AcordAppText%>
    </center>
    <center style="margin-top: 20px;">
        <asp:HyperLink ID="linkPrintAccord" runat="server" Target="_blank">Print ACORD App</asp:HyperLink>
        <span style="margin-left: 30px;">
            <asp:LinkButton ID="lnkFinalize" OnClientClick="javascript:FinalizeClick(this);" runat="server">Continue to Make a Payment Page</asp:LinkButton>
        </span>        
        <div runat="server" id="DivFarmMessage" style="margin-top:15px; max-width:90%">By clicking on the “Continue to Make a Payment Page” you are binding coverage.  Please send photos, blanket inventory, and all other supporting documents within 2 business days.</div>        
    </center>
</div>

<div id="divPopupLinkedQuotes" style="display: none;">

    <asp:Literal ID="tblLinedApps" runat="server"></asp:Literal>
    <asp:Literal ID="tblLinkedQuotes" runat="server"></asp:Literal>
</div>

<center>
<div class="standardSubSection">

    <div id="divNewCoMessage" runat="server" visible="false" style="text-align:center;">
        <asp:Label ID="NewCoMessage" runat="server" CssClass="informationalTextRed"></asp:Label>
    </div>

    <div id="divTopMessage" runat="server" visible="false" style="text-align:center;">
        <asp:Label ID="topMessage" runat="server" CssClass="informationalText"></asp:Label>
    </div>

    <div id="divPersButtons" runat="server" visible="false">
        <asp:Button ID="btnViewAccord" runat="server" CssClass="StandardSaveButton" Text="Print ACORD App"></asp:Button>
        <%--<asp:Button ID="btnCreateHomeQuote" CssClass="StandardSaveButton" runat="server" Text="Begin Quote with Same Policyholder"></asp:Button>--%>
        <asp:Button ID="btnPolicyholderStartNewQuote" runat="server" CssClass="StandardSaveButton" OnClientClick="return InitNewQuote();" Text="Start New Quote for this Policyholder" Visible="false" />

        <asp:Button ID="btnShowIRPM" runat="server" CssClass="StandardSaveButton" Text="IRPM"></asp:Button>
        <asp:Button ID="btnCapVehicleListApp" runat="server" CssClass="StandardSaveButton" Text="Print list of Vehicles" OnClientClick="return OpenVehicleDataList();" visible="false" />
        <asp:Button ID="btnContinueToApp"  runat="server" CssClass="StandardSaveButton" Text="Continue To UW Questions"/>
        <asp:Button ID="btnDeleteEndorsement" runat="server" OnClientClick='var c = confirm("Continue to delete this Change?"); if (c){$("input[type=submit]").hide();return true;}else{return false;}' CssClass="StandardSaveButton" Text="Delete This Change"></asp:Button>
        <asp:Button ID="btnStartNewQuote" runat="server" CssClass="StandardSaveButton" OnClientClick="return InitNewQuote();" Text="Start New Quote for this Client" visible="false" />
    </div>

    <div id="divCommButtons" runat="server" visible="false">
        <table>
            <tr>
                <td style="text-align:center;">
                    <asp:Button ID="btnCommIRPM" runat="server" CssClass="StandardSaveButton" Text="IRPM" style="min-width: 70px"/>
                    <asp:Button ID="btnCommViewWorksheet" runat="server" CssClass="StandardSaveButton" Text="View Worksheet" />
                    <%--<asp:Button ID="btnCommEmailForUWAssistance" runat="server" CssClass="StandardSaveButton" Text="Email for UW Assistance" OnClientClick="InitEmailToUW();" />--%>
                    <input type="button" id="btnEmailForUWAssistance" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" />
                    <%--<button runat="server" onclick="InitEmailToUW();" class="StandardSaveButton">Email for UW Assistance</button>--%>
                    <asp:Button ID="btnCommStartNewQuote" runat="server" CssClass="StandardSaveButton" OnClientClick="return InitNewQuote();" Text="Start New Quote for this Client" />
                    <asp:Button ID="btnCapVehicleList" runat="server" CssClass="StandardSaveButton" Text="Print list of Vehicles" OnClientClick="return OpenVehicleDataList();" visible="false" />
                    <asp:Button ID="btnCommSubmitEndorsement" runat="server" CssClass="StandardSaveButton" Text="Submit" />
                    <asp:Button ID="btnCommDeleteEndorsement" runat="server" OnClientClick='var c = confirm("Continue to delete this Change?"); if (c){$("input[type=submit]").hide();return true;}else{return false;}' CssClass="StandardSaveButton" Text="Delete This Change"></asp:Button>
                </td>
            </tr>
            <tr>
                <td style="text-align:center;">
                    <asp:Button ID="btnCommPrepareProposal" runat="server" CssClass="StandardSaveButton" Text="Prepare Proposal" />
                    <asp:Button ID="btnCommContinueToApplication" runat="server" CssClass="StandardSaveButton" Text="Continue to Application" />
                </td>
            </tr>
        </table>
    </div>

    <%--<div id="divCommAppSummaryButtons" runat="server" visible="false" style="text-align:center;">
        <asp:Button ID="btnAppSumPrint" runat="server" CssClass="StandardSaveButton" Text="Print Documents"/>
        <asp:Button ID="btnAppSumIRPM" runat="server" CssClass="StandardSaveButton" Text="IRPM"/>
        <asp:Button ID="btnSubmit" runat="server" CssClass="StandardSaveButton" Text="Submit"/>
    </div>--%>

    <uc1:ctl_RouteToUw runat="server" id="ctl_RouteToUw" Visible="false" />

    <div id="divNewCoRedirectMessage" runat="server" visible="false" style="text-align:center;">
        <%--<asp:Label ID="NewCoRedirectMessage" runat="server" CssClass="informationalTextRed"></asp:Label>--%>
        <span ID="NewCoRedirectMessage" runat="server" class="informationalText">To ensure accurate coverage and premium charges, we need to re-rate your quote.  Please click <a href="#" id="NewCoRedirectLink" runat="server" class="informationalTextRed"><font style="color:red;font-weight:700;">Here</font></a> to begin the process.</span>
    </div>

    <div id="divSubmitDisabledMessage" runat="server" visible="false" style="text-align:center;">
        <asp:Label ID="lblSubmitDisabledMsg" runat="server" CssClass="informationalText"></asp:Label>
    </div>

    <div><asp:Label ID="lblInvalidEffectiveDate" ForeColor="Red" runat="server" Text="The effective date is no longer valid. Please update the effective date and rerate."></asp:Label></div>
</div>
    </center>