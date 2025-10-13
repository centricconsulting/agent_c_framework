<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_LocationList.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_LocationList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/Locations/ctl_CPR_Location.ascx" TagPrefix="uc1" TagName="ctl_CPR_Location" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlVehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctlVEHAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/Buildings/ctl_Endo_AppliedAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctl_Endo_AppliedAdditionalInterestList" %>


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
<div>
    <uc1:ctlVEHAdditionalInterestList runat="server" ID="ctl_AdditionalInterestList" />
    <uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSearch" />
    <uc1:ctl_Endo_AppliedAdditionalInterestList runat="server" ID="ctl_Endo_AppliedAdditionalInterestList" />
</div>
<div id="divActionButtons" runat="server" class="standardSubSection center" style="width:100%;text-align:center;">
    <asp:Button ID="btnAddLocation" runat="server" CssClass="StandardSaveButton" ToolTip="Add a Location" Text="Add Another Location" />
    <asp:Button ID="btnSubmit" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Save Locations" Text="Save Locations" />
    <asp:Button ID="btnRate" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Rate This Quote" />
    <asp:Button ID="btnContinue" runat="server" Text="Continue to General Liability" CssClass="StandardSaveButton" />
    <input type="button" id="btnEmailForUWAssistance" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" />
    <br />
</div>
<div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
    <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
    <asp:Button ID="btnViewGenLiab" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="General Liability" />
</div>

<asp:HiddenField ID="hdnAccord" runat="server" />
