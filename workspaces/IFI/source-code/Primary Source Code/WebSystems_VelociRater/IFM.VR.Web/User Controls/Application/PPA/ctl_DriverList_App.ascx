<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_DriverList_App.ascx.vb" Inherits="IFM.VR.Web.ctl_DriverList_App" %>
<%@ Register Src="~/User Controls/Application/PPA/ctl_Driver_App.ascx" TagPrefix="uc1" TagName="ctl_Driver_App" %>


<div runat="server" id="divDriver_App" class="standardSubSection">
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_Driver_App runat="server" ID="ctl_Driver_App" />
        </ItemTemplate>
    </asp:Repeater>
</div>

<asp:HiddenField ID="hidden_divDriver_App" Value="0" runat="server" />