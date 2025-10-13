<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_ENDO_LocationList.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_ENDO_LocationList" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_EmailUW.ascx" TagPrefix="uc1" TagName="ctl_Email_UW" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/ctl_BOP_ENDO_Location.ascx" TagPrefix="uc2" TagName="ctl_BOP_ENDO_Location" %>
<%@ Register Src="~/User Controls/Endorsements/CommonControls/ctl_Endo_VehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctl_Endo_VehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/Buildings/ctl_Endo_AppliedAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctl_Endo_AppliedAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>



<div>
    <div id="divMainList" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc2:ctl_BOP_ENDO_Location runat="server" ID="ctl_BOP_ENDO_Location" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    
    <uc1:ctl_Endo_VehicleAdditionalInterestList runat="server" ID="ctl_Endo_VehicleAdditionalInterestList" />
    <uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSerach" />
    <uc1:ctl_Endo_AppliedAdditionalInterestList runat="server" ID="ctl_Endo_AppliedAdditionalInterestList" />
    
    <uc1:ctl_Email_UW ID="ctl_EmailUW" runat="server"></uc1:ctl_Email_UW>
    <asp:HiddenField ID="hdnAccord" runat="server" />

    <div style="margin-top: 20px; width: 100%; text-align:center;">
        <asp:Button ID="btnAddAnotherLocation" runat="server" Text="Add Another Location" CssClass="StandardSaveButton" />
        <asp:Button ID="btnSaveLocation" runat="server" Text="Save Location" CssClass="StandardSaveButton" />
        <asp:Button ID="btnSaveAndRate" runat="server" Text="Rate This Quote" CssClass="StandardSaveButton" />
        <uc1:ctl_RouteToUw runat="server" ID="ctl_RouteToUw" />
        <%--<input type="button" id="btnEmailForUWAssistance" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" />--%>
    </div>
</div>
