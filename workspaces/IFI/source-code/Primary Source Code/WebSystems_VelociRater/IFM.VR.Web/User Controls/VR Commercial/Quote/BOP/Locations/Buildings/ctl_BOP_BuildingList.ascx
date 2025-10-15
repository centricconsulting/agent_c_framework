<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_BuildingList.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_BuildingList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/BOP/Locations/Buildings/ctl_BOP_Building.ascx" TagPrefix="uc1" TagName="ctl_BOP_Building" %>


<div id="divBuildingList" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:ctl_BOP_Building runat="server" ID="ctl_BOP_Building" />
            </ItemTemplate>
        </asp:Repeater>
    </div>    
    <asp:HiddenField ID="hdnAccord" runat="server" />