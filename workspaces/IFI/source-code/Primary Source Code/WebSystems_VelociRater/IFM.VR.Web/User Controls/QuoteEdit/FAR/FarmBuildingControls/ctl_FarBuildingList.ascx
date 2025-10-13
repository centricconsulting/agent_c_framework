<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_FarBuildingList.ascx.vb" Inherits="IFM.VR.Web.ctl_FarBuildingList" %>
<%@ Register Src="~/User Controls/QuoteEdit/FAR/FarmBuildingControls/ctl_FarBuilding.ascx" TagPrefix="uc1" TagName="ctl_FarBuilding" %>

<div runat="server" id="divNewBuilding">
    <uc1:ctl_FarBuilding runat="server" ID="ctl_FarBuildingNew" HideFromParent="True" MyBuildingIndex="-1" MyLocationIndex="-1" />
</div>

<div runat="server" id="divBuildingList">
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_FarBuilding runat="server" ID="ctl_FarBuilding" />
        </ItemTemplate>
    </asp:Repeater>
</div>
<asp:HiddenField ID="hiddenActiveBuilding" runat="server" />