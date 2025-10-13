<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_LocationsWF.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_LocationsWF" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/Locations/ctl_CPR_LocationList.ascx" TagPrefix="uc1" TagName="ctl_CPR_LocationList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/ClassCode/ctl_CPR_ClassCodeList.ascx" TagPrefix="uc1" TagName="ctl_CPR_ClassCodeList" %>

<uc1:ctl_CPR_LocationList runat="server" ID="ctl_CPR_LocationList" />
<uc1:ctl_CPR_ClassCodeList runat="server" ID="ctl_CPR_ClassCodeList" />

<div id="divActionButtons" runat="server" class="standardSubSection center" style="width:100%;">
    <asp:Button ID="btnAddLocation" runat="server" CssClass="StandardSaveButton" ToolTip="Add a Location" Text="Add Another Location" />
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Save Locations" Text="Save Locations" />
    <asp:Button ID="btnRate" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Rate This Quote" />
    <input type="button" id="btnEmailForUWAssistance" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" />
    <br />
</div>