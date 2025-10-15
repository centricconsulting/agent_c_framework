<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomeQuote.ascx.vb" Inherits="IFM.VR.Web.ctlHomeQuote" %>

<%@ Register Src="~/User Controls/QuoteEdit/HOM/ctlHomeCoverages.ascx" TagPrefix="uc1" TagName="ctlCoverages" %>

<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/Property/ctlProperty_HOM.ascx" TagPrefix="uc1" TagName="ctlProperty_HOM" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/ctlQuoteSummary_HOM.ascx" TagPrefix="uc1" TagName="ctlQuoteSummary_HOM" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/Application/ctl_OrderClueAndOrMVR.ascx" TagPrefix="uc1" TagName="ctl_OrderClueAndOrMVR" %>
<%@ Register Src="~/User Controls/Endorsements/ctlBillingInfo.ascx" TagPrefix="uc1" TagName="ctlBillingInfo" %>
<%@ Register Src="~/User Controls/Endorsements/ctlPrintHistory.ascx" TagPrefix="uc1" TagName="ctlPrintHistory" %>
<%@ Register Src="~/User Controls/Endorsements/ctlPolicyHistory.ascx" TagPrefix="uc1" TagName="ctlPolicyHistory" %>
<%@ Register Src="~/User Controls/Application/ctl_AttachmentUpload.ascx" TagPrefix="uc1" TagName="ctl_AttachmentUpload" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctl_Personal_NewQuoteForClient.ascx" TagPrefix="uc1" TagName="ctl_Personal_NewQuoteForClient" %>

<uc1:ctl_Personal_NewQuoteForClient runat="server" ID="ctl_Personal_NewQuoteForClient" />
<uc1:ctlUWQuestionsPopup runat="server" ID="ctlUWQuestionsPopup" />
<uc1:ctlDisplayDiamondRatingErrors runat="server" id="ctlDisplayDiamondRatingErrors" />

<div id="divEntireWorkFlow" style="display: none;">
    <uc1:ctlEffectiveDateChecker runat="server" id="ctlEffectiveDateChecker" />
    <uc1:ctlIsuredList runat="server" ID="ctlIsuredList" />
    <uc1:ctlProperty_HOM runat="server" MyLocationIndex="0" id="ctlProperty_HOM" />
    <uc1:ctlCoverages runat="server" ID="ctlCoverages_HOM" />
    <uc1:ctlQuoteSummary_HOM runat="server" id="ctlQuoteSummary_HOM" />   
    <uc1:ctlBillingInfo runat="server" ID="ctlBillingInfo" />
    <uc1:ctlPrintHistory runat="server" ID="ctlPrintHistory" />
    <uc1:ctlPolicyHistory runat="server" ID="ctlPolicyHistory" />
    <uc1:ctl_AttachmentUpload runat="server" ID="ctl_AttachmentUpload" />
</div>

<uc1:ctl_OrderClueAndOrMVR runat="server" id="ctl_OrderClueAndOrMVR" />