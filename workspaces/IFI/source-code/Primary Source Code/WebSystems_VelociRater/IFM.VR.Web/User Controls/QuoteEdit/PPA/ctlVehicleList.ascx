<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlVehicleList.ascx.vb" Inherits="IFM.VR.Web.ctlVehicleList" %>

<%@ Register Src="~/User Controls/QuoteEdit/PPA/ctlVehicle_PPA.ascx" TagPrefix="uc1" TagName="ctlVehicle_PPA" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>
<%@ Register Src="~/User Controls/Endorsements/ctlEndorsementReplaceVehicle.ascx" TagPrefix="uc1" TagName="ctlEndorsementReplaceVehicle" %>
<%@ Register Src="~/User Controls/Endorsements/ctlRecentManufacturedVehicle.ascx" TagPrefix="uc1" TagName="ctlRecentManufacturedVehicle" %>
<%@ Register Src="~/User Controls/Endorsements/ctlEndorsementRemoveVehicle.ascx" TagPrefix="uc1" TagName="ctlEndorsementRemoveVehicle" %>



<div id="VehicleListControlDivTopMost" runat="server">
    <asp:Panel ID="Panel1" runat="server" DefaultButton="btnSubmit">
        <div id="divVehicles" runat="server">
            <asp:Repeater ID="Repeater1" runat="server">
                <ItemTemplate>
                    <uc1:ctlVehicle_PPA runat="server" ID="ctlVehicle_PPAControl" />
                </ItemTemplate>
            </asp:Repeater>
        </div>

        <div id="divActionButtons" runat="server" style="width: 100%; text-align: center; margin-top: 10px;">
            <div style="float: left;">
                <asp:Button ID="btnEndorsementReplaceVehicle" ToolTip="Replace Vehicle" runat="server" OnClientClick="DisableFormOnSaveRemoves();StopEventPropagation(event);" CssClass="StandardSaveButton" Text="Replace Vehicle" Visible="false" />
                <asp:Button ID="btnAddvehicle" ToolTip="Add a new Vehicle" runat="server" OnClientClick="DisableFormOnSaveRemoves();StopEventPropagation(event);" CssClass="StandardSaveButton" Text="Add Vehicle" />
                <asp:Button ID="btnEndorsementRemoveVehicle" ToolTip="Remove Vehicle" runat="server" OnClientClick="DisableFormOnSaveRemoves();StopEventPropagation(event);" CssClass="StandardSaveButton" Text="Remove Vehicle" Visible="false" />
            </div>
            <asp:Button ID="btnSubmit" ToolTip="Save All Vehicle Information" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Save Vehicles" />
            <asp:Button ID="btnSaveandGotoCoverages" ToolTip="Save All Vehicle Information and Navigate to the Coverages Page" OnClientClick="DisableFormOnSaveRemoves();" runat="server" CssClass="StandardSaveButton" Text="Coverages Page" /><br /><br />
        </div>
        <div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
            <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
            <asp:Button ID="btnViewGotoCoverages" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Coverages Page" />
        </div>
    </asp:Panel>
</div>
<uc1:ctlEndorsementReplaceVehicle runat="server" id="ctlEndorsementReplaceVehicle" />
<uc1:ctlEndorsementRemoveVehicle runat="server" id="ctlEndorsementRemoveVehicle" />
<uc1:ctlRecentManufacturedVehicle runat="server" id="ctlRecentManufacturedVehicle" />
<uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSerach" />
<asp:HiddenField ID="hidden_VehicleListActive" runat="server" />
<asp:HiddenField ID="hdnHasReplacedVehicle" runat="server" />