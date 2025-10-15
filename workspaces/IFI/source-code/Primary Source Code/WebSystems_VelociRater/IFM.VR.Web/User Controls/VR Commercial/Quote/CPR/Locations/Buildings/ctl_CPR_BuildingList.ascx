<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_BuildingList.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_BuildingList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CPR/Locations/Buildings/ctl_CPR_Building.ascx" TagPrefix="uc1" TagName="ctl_Building" %>

<div id="divBuildingList" runat="server">
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_Building id="ctl_CPR_Building" runat="server"></uc1:ctl_Building>
        </ItemTemplate>
    </asp:Repeater>
</div>    
<asp:HiddenField ID="hdnAccord" runat="server" />
