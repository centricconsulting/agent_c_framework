<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Master_Edit.ascx.vb" Inherits="IFM.VR.Web.ctl_Master_Edit" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlDriverList.ascx" TagPrefix="uc1" TagName="ctlDriverList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlVehicleList.ascx" TagPrefix="uc1" TagName="ctlVehicleList" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlCoverage_PPA.ascx" TagPrefix="uc1" TagName="ctlCoverage_PPA" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlPrefill_PPA.ascx" TagPrefix="uc1" TagName="ctlPrefill_PPA" %>

<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlQsummary_PPA.ascx" TagPrefix="uc1" TagName="ctlQsummary_PPA" %>
<%--<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlCoverage_PPA_Vehicle_List.ascx" TagPrefix="uc1" TagName="ctlCoverage_PPA_Vehicle_List" %>--%>

<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/Endorsements/ctlPrintHistory.ascx" TagPrefix="uc1" TagName="ctlPrintHistory" %>
<%@ Register Src="~/User Controls/Endorsements/ctlPolicyHistory.ascx" TagPrefix="uc1" TagName="ctlPolicyHistory" %>
<%@ Register Src="~/User Controls/Endorsements/ctlBillingInfo.ascx" TagPrefix="uc1" TagName="ctlBillingInfo" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctl_Personal_NewQuoteForClient.ascx" TagPrefix="uc1" TagName="ctl_Personal_NewQuoteForClient" %>



<uc1:ctlUWQuestionsPopup runat="server" ID="ctlUWQuestionsPopup" />
<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />
<uc1:ctlEffectiveDateChecker runat="server" ID="ctlEffectiveDateChecker" />
<uc1:ctl_Personal_NewQuoteForClient runat="server" ID="ctl_Personal_NewQuoteForClient" />

<div id="MasterEdit" style="display: none;">
    <uc1:ctlIsuredList runat="server" ID="ctlIsuredList" />
    <uc1:ctlPrefill_PPA runat="server" ID="ctlPrefill_PPA" />
    <uc1:ctlDriverList runat="server" ID="ctlDriverList" />
    <uc1:ctlVehicleList runat="server" ID="ctlVehicleList" />
    <uc1:ctlCoverage_PPA runat="server" ID="ctlCoverage_PPA" />
    <uc1:ctlQsummary_PPA runat="server" ID="ctlQsummary_PPA" />
    <uc1:ctlPrintHistory runat="server" ID="ctlPrintHistory" />
    <uc1:ctlPolicyHistory runat="server" ID="ctlPolicyHistory" />
    <uc1:ctlBillingInfo runat="server" ID="ctlBillingInfo" />
    <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
    <%--<uc1:ctlCoverage_PPA_Vehicle_List runat="server" ID="ctlCoverage_PPA_Vehicle_List" />--%>
</div>