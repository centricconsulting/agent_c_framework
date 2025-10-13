<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Insured_Applicant_Manager.ascx.vb" Inherits="IFM.VR.Web.ctl_Insured_Applicant_Manager" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlIsuredList.ascx" TagPrefix="uc1" TagName="ctlIsuredList" %>
<%@ Register Src="~/User Controls/QuoteEdit/ctlApplicantList.ascx" TagPrefix="uc1" TagName="ctlApplicantList" %>
<%@ Register Src="~/User Controls/Application/ctl_OrderClueAndOrMVR.ascx" TagPrefix="uc1" TagName="ctl_OrderClueAndOrMVR" %>

<uc1:ctlIsuredList runat="server" ID="ctlIsuredList" ShowActionButtons="false" />
<uc1:ctlApplicantList runat="server" ID="ctlApplicantList" ShowActionButtons="false" />
<uc1:ctl_OrderClueAndOrMVR runat="server" ID="ctl_OrderClueAndOrMVR" />

<div id="divActionButtons" runat="server" style="margin-top: 20px; width: 100%; text-align: center;">
    <div style="float: left;">
        <asp:Button ID="bnAddApplicant" ToolTip="Add an Applicant" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Add Applicant" />
    </div>

    <asp:Button ID="btnSave" ToolTip="Save all Applicant Information" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Save Policyholders/Applicants" />
    <asp:Button ID="btnSaveAndGotoNextPage" ToolTip="Save all Applicant Information and Navigate to the next page" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Policy Level Coverages Page" />
    <asp:Button ID="btnRate" ToolTip="Save all Applicant Information and Rate" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Rate this Quote" />
</div>
<div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
    <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change" />
    <asp:Button ID="btnViewGotoDrivers" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Policy Level Coverages Page" />
    <asp:Button ID="btnRate_Endorsements" ToolTip="Save all Applicant Information and Rate" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Rate this Quote" />
</div>
