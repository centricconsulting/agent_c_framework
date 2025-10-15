<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_LocationList.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_LocationList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/Locations/ctl_CPR_Location.ascx" TagPrefix="uc1" TagName="ctl_CPR_Location" %>

<div id="divNewLocation" runat="server">
    <uc1:ctl_CPR_Location runat="server" HideFromParent="True" MyLocationIndex="-1" id="ctl_CPR_Location" />
</div>

<div id="divLocationList" runat="server">
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_CPR_Location runat="server" id="ctl_CPR_Location" />
        </ItemTemplate>
    </asp:Repeater>

</div>

<div id="divActionButtons" runat="server" class="standardSubSection center" style="width:100%;">
    <asp:Button ID="btnAddLocation" runat="server" CssClass="StandardSaveButton" ToolTip="Add a Location" Text="Add Another Location" />
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Save Locations" Text="Save Locations" />
    <asp:Button ID="btnRate" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Rate This Quote" />
    <input type="button" id="btnEmailForUWAssistance" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" />
    <br />
</div>

<asp:HiddenField ID="hdnAccord" runat="server" />
