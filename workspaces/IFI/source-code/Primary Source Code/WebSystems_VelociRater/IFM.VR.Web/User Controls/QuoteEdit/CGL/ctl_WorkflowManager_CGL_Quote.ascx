<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_WorkflowManager_CGL_Quote.ascx.vb" Inherits="IFM.VR.Web.ctl_WorkflowManager_CGL_Quote" %>

<%@ Register Src="~/User Controls/ctlTreeView.ascx" TagPrefix="uc1" TagName="ctlTreeView" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/ctlEffectiveDateChecker.ascx" TagPrefix="uc1" TagName="ctlEffectiveDateChecker" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlApplicantList.ascx" TagPrefix="uc1" TagName="ctlApplicantList" %>
<%@ Register Src="~/User Controls/ctlUWQuestionsPopup.ascx" TagPrefix="uc1" TagName="ctlUWQuestionsPopup" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlDisplayDiamondRatingErrors.ascx" TagPrefix="uc1" TagName="ctlDisplayDiamondRatingErrors" %>

<%@ Register Src="~/User Controls/QuoteEdit/ctlRiskGradeSearch.ascx" TagPrefix="uc1" TagName="ctlRiskGradeSearch" %>
<%@ Register Src="~/User Controls/QuoteEdit/CGL/PolicyLevelCoverages/ctl_CGL_PolicyLevelCoverages.ascx" TagPrefix="uc1" TagName="ctl_CGL_PolicyLevelCoverages" %>
<%@ Register Src="~/User Controls/QuoteEdit/CGL/Locations/ctl_CGL_LocationsWF.ascx" TagPrefix="uc1" TagName="ctl_CGL_LocationsWF" %>
<%@ Register Src="~/User Controls/QuoteEdit/CGL/ctl_Summary_CGL.ascx" TagPrefix="uc1" TagName="ctl_Summary_CGL" %>









<uc1:ctlEffectiveDateChecker runat="server" ID="ctlEffectiveDateChecker" />


<uc1:ctlUWQuestionsPopup runat="server" ID="ctlUWQuestionsPopup" />
<uc1:ctlRiskGradeSearch runat="server" id="ctlRiskGradeSearch" />
<uc1:ctlDisplayDiamondRatingErrors runat="server" ID="ctlDisplayDiamondRatingErrors" />
<table style="width: 100%;">
    <tr>
        <td style="width: 250px; vertical-align: top;">
            <uc1:ctlTreeView runat="server" ID="ctlTreeView" />

        </td>
        <td style="vertical-align: top;">
            <div id="divEditControls" style="display: none;">
                <uc1:ctlIsuredList runat="server" ID="ctlIsuredList" />
                
                <uc1:ctl_CGL_PolicyLevelCoverages runat="server" id="ctl_CGL_PolicyLevelCoverages" />

                <uc1:ctl_CGL_LocationsWF runat="server" id="ctl_CGL_LocationsWF" />

                <uc1:ctl_Summary_CGL runat="server" id="ctl_Summary_CGL" />
                
            </div>
        </td>
    </tr>
</table>