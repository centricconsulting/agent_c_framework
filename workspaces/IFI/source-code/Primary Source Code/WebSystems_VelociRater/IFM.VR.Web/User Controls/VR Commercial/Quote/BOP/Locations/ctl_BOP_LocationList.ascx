<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_LocationList.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_LocationList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/Locations/ctl_BOP_Location.ascx" TagPrefix="uc1" TagName="ctl_BOP_Location" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_EmailUW.ascx" TagPrefix="uc1" TagName="ctl_Email_UW" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlVehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctlVEHAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/Buildings/ctl_Endo_AppliedAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctl_Endo_AppliedAdditionalInterestList" %>

<div>
    <div id="divMainList" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:ctl_BOP_Location runat="server" id="ctl_BOP_Location" />
            </ItemTemplate>
        </asp:Repeater>
        
    </div>
    <div>
        <uc1:ctlVEHAdditionalInterestList runat="server" ID="ctl_AdditionalInterestList" />
        <uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSearch" />
        <uc1:ctl_Endo_AppliedAdditionalInterestList runat="server" ID="ctl_Endo_AppliedAdditionalInterestList" />
    </div>
    <uc1:ctl_Email_UW ID="ctl_EmailUW" runat="server"></uc1:ctl_Email_UW>
    <asp:HiddenField ID="hdnAccord" runat="server" />

    <div id="divBOPLocationButtons" runat="server" style="margin-top: 20px; width: 100%; text-align:center;">
        <asp:Button ID="btnAddAnotherLocation" runat="server" Text="Add Another Location" CssClass="StandardSaveButton" />
        <asp:Button ID="btnSaveLocation" runat="server" Text="Save Location" CssClass="StandardSaveButton" />
        <asp:Button ID="btnSaveAndRate" runat="server" Text="Rate This Quote" CssClass="StandardSaveButton" />
        <input type="button" id="btnEmailForUWAssistance" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" />
    </div>
    <div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
        <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
        <asp:Button ID="btnViewBillingInfo" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Billing Information" />
    </div>
</div>
