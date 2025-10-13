<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_ENDO_BuildingList.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_ENDO_BuildingList" %>
<%@ Register Src="~/User Controls/Endorsements/Application/BOP/Locations/Buildings/ctl_BOP_ENDO_Building.ascx" TagPrefix="uc2" TagName="ctl_BOP_ENDO_Building" %>



<div id="divBuildingList" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc2:ctl_BOP_ENDO_Building runat="server" ID="ctl_BOP_ENDO_Building" />
            </ItemTemplate>
        </asp:Repeater>
    </div>    
    <asp:HiddenField ID="hdnAccord" runat="server" />