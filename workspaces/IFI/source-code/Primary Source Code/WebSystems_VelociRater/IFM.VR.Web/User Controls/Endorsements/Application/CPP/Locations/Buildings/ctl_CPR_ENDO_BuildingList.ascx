<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_ENDO_BuildingList.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_ENDO_BuildingList" %>
<%@ Register Src="~/User Controls/Endorsements/Application/CPP/Locations/Buildings/ctl_CPR_ENDO_Building.ascx" TagPrefix="uc1" TagName="ctl_CPR_ENDO_Building" %>


<div id="divBuildingList" runat="server">
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_CPR_ENDO_Building runat="server" ID="ctl_CPR_ENDO_Building" />
        </ItemTemplate>
    </asp:Repeater>
</div>    
<asp:HiddenField ID="hdnAccord" runat="server" />
