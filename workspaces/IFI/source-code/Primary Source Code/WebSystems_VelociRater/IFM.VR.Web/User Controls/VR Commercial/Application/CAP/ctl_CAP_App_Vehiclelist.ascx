<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CAP_App_Vehiclelist.ascx.vb" Inherits="IFM.VR.Web.ctl_CAP_App_Vehiclelist" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CAP/ctl_CAP_App_Vehicle.ascx" TagPrefix="uc1" TagName="ctlVehicle_CAP" %>

<div>
    <div id="divVehicles" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:ctlVehicle_CAP runat="server" ID="ctlVehicle" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <asp:HiddenField ID="hidden_VehicleListActive" runat="server" />
</div>

