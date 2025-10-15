<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPR_App_BuildingList.ascx.vb" Inherits="IFM.VR.Web.ctl_CPR_App_BuildingList" %>
<%@ Register Src="~/User Controls/VR Commercial/Application/CPR/ctl_CPR_App_Building.ascx" TagPrefix="uc1" TagName="ctl_CPR_App_Building" %>

    <div id="divBuildingList" runat="server">
        <asp:Repeater ID="Repeater1" runat="server">
            <ItemTemplate>
                <uc1:ctl_CPR_App_Building runat="server" ID="ctl_CPR_App_Building" />
            </ItemTemplate>
        </asp:Repeater>
        <asp:HiddenField ID="hdnAccord" runat="server" />
    </div>    
