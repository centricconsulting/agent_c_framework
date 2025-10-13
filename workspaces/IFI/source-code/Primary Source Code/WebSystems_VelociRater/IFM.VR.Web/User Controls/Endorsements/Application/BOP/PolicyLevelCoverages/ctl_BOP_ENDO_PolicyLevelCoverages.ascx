<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_ENDO_PolicyLevelCoverages.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_ENDO_PolicyLevelCoverages" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>
<%@ Register Src="~/User Controls/Endorsements/CommonControls/ctl_Endo_VehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctl_Endo_VehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Endorsements/CommonControls/ctl_Endo_CTEQList.ascx" TagPrefix="uc1" TagName="ctl_Endo_CTEQList" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/Buildings/ctl_Endo_AppliedAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctl_Endo_AppliedAdditionalInterestList" %>





<uc1:ctl_Endo_CTEQList runat="server" ID="ctl_Endo_CTEQList" />
<uc1:ctl_Endo_VehicleAdditionalInterestList runat="server" ID="ctl_Endo_VehicleAdditionalInterestList" />
<uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSerach" />
<uc1:ctl_Endo_AppliedAdditionalInterestList runat="server" ID="ctl_Endo_AppliedAdditionalInterestList" />

<div id="divActionButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;">
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Saves any coverages entered." Text="Save Coverages" />
    <asp:Button ID="btnRatePolicyholder" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Rate Quote" Text="Rate this Quote"/>
</div>
<div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
    <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
</div>
