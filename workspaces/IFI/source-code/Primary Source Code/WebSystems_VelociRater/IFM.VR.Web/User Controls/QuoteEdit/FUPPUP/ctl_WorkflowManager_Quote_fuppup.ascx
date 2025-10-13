<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_Quote_fuppup.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_Quote_fuppup" %>

<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/ctl_Insured_Applicant_Manager.ascx" TagPrefix="uc1" TagName="ctl_Insured_Applicant_Manager" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/ctlUmbrellaPolicyCoverages.ascx" TagPrefix="uc1" TagName="ctlUmbrellaPolicyCoverages" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/ctl_FUPPUP_QuoteSummary.ascx" TagPrefix="uc1" TagName="ctl_FUPPUP_QuoteSummary" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>
<%@ Register Src="~/User Controls/QuoteEdit/FUPPUP/ctl_FUPPUP_PFSummary.ascx" TagPrefix="uc1" TagName="ctl_FUPPUP_PFSummary" %>


<uc1:ctlEffectiveDateChecker runat="server" ID="ctlEffectiveDateChecker" />
<uc1:ctlUWQuestionsPopup runat="server" ID="ctlUWQuestionsPopup" />
<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />

<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />
        </td>
        <td style="vertical-align: top;">
            <div id="divAppEditControls" style="display: none;">
                <uc1:ctl_Insured_Applicant_Manager runat="server" id="ctl_Insured_Applicant_Manager" />
                <uc1:ctlUmbrellaPolicyCoverages runat="server" id="ctlUmbrellaPolicyCoverages" />
                <uc1:ctl_FUPPUP_QuoteSummary runat="server" ID="ctl_FUPPUP_QuoteSummary" />
                <uc1:ctl_FUPPUP_PFSummary runat="server" ID="ctl_FUPPUP_PFSummary" />
            </div>
        </td>
    </tr>
</table>