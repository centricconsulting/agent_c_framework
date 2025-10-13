<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CAP_VehicleList.ascx.vb" Inherits="IFM.VR.Web.ctl_CAP_VehicleList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CAP/ctl_CAP_Vehicle.ascx" TagPrefix="uc1" TagName="ctl_CAP_Vehicle" %>
<%@ Register Src="~/User Controls/VR Commercial/Common/ctl_Comm_EmailUW.ascx" TagPrefix="uc1" TagName="ctl_Email_UW" %>
<%@ Register Src="~/User Controls/Application/PPA/ctlVehicleAdditionalInterestList.ascx" TagPrefix="uc1" TagName="ctlVehicleAdditionalInterestList" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>
<%@ Register Src="~/User Controls/Application/ctl_RouteToUw.ascx" TagPrefix="uc1" TagName="ctl_RouteToUw" %>

<div>
    <uc1:ctlVehicleAdditionalInterestList runat="server" ID="ctlVehicleAdditionalInterestList" />
    <uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSerach" />   
    <div id="divMainList" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:ctl_CAP_Vehicle runat="server" id="ctl_CAP_Vehicle" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <uc1:ctl_Email_UW ID="ctl_EmailUW" runat="server"></uc1:ctl_Email_UW>
    <asp:HiddenField ID="hdnAccord" runat="server" />
    <div id="divEndorsementMaxTransactionsMessage" runat="server" class="" style="width: 100%; text-align: center;font-weight:bold;color:blue;" visible="false">
        <br />Only 3 vehicles can be added or deleted per transaction, contact your underwriter for changes involving more than 3.
    </div>
    <div id="divUseVINLookupMessage" runat="server" style="width:100%; text-align:center;font-weight:bold;" class="informationalText">
        <br />To rate, you must select a vehicle from the VIN Lookup.
    </div>
    <div id="divActionButtons" runat="server" style="margin-top: 20px; width: 100%; text-align:center;">
        <asp:Button ID="btnAddVehicle" runat="server" Text="Add Vehicle" CssClass="StandardSaveButton" />
        <asp:Button ID="btnSaveVehicles" runat="server" Text="Save Vehicles" CssClass="StandardSaveButton" />
        <asp:Button ID="btnSaveAndRate" runat="server" Text="Rate This Quote" CssClass="StandardSaveButton" />
        <input type="button" id="btnEmailForUWAssistance" runat="server" onclick="InitEmailToUW();" title="Email for UW Assistance" value="Email for UW Assistance" class="StandardSaveButton" />
        <span id="spanRouteToUWContainer" style="display:none;"><uc1:ctl_RouteToUw runat="server" id="ctl_RouteToUw" Visible="false" /></span>                    
    </div>
    <div id="divEndorsementButtons" runat="server" class="standardSubSection center" style="width: 100%; text-align: center;" visible="false">
        <asp:Button ID="btnMakeAChange" runat="server" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" ToolTip="Make a Change to this policy" Text="Make A Change"/>
        <asp:Button ID="btnViewBillingInfo" OnClientClick="DisableFormOnSaveRemoves();" CssClass="StandardSaveButton" runat="server" Text="Billing Information" />
    </div>
</div>
