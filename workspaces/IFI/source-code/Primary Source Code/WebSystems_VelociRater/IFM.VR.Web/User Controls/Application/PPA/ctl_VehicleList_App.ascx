<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_VehicleList_App.ascx.vb" Inherits="IFM.VR.Web.ctl_VehicleList_App" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Vehicle_App.ascx" TagPrefix="uc1" TagName="ctl_Vehicle_App" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_AdditionalInterest_MiniSerach.ascx" TagPrefix="uc1" TagName="ctl_AdditionalInterest_MiniSerach" %>

<%--<div id="divVehicles_App" runat="server" style="display: none;">
    <h3>Vehicles</h3>
    <div>
    </div>
</div>--%>

<div runat="server" id="divVehicle_App" class="standardSubSection">
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_Vehicle_App runat="server" ID="ctl_Vehicle_App" />
        </ItemTemplate>
    </asp:Repeater>
</div>
<uc1:ctl_AdditionalInterest_MiniSerach runat="server" ID="ctl_AdditionalInterest_MiniSerach" />

<asp:HiddenField ID="hiddenActiveVehicle" runat="server" />