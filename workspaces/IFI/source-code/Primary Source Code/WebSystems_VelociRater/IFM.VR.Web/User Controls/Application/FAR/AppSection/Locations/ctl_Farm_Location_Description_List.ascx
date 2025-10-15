<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Farm_Location_Description_List.ascx.vb" Inherits="IFM.VR.Web.ctl_Farm_Location_Description_List" %>
<%@ Register Src="~/User Controls/Application/FAR/AppSection/Locations/ctl_Farm_Location_Description.ascx" TagPrefix="uc1" TagName="ctl_Farm_Location_Description" %>

<div style="margin-bottom: 30px;">
    <span style="float: left;">Acreage Only Locations  </span>
    <div style="margin-left: 400px;">
        <asp:LinkButton ID="lnkAdd" ToolTip="Add New Acreage Only Location" runat="server">Add New</asp:LinkButton>
    </div>
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_Farm_Location_Description runat="server" ID="ctl_Farm_Location_Description" />
        </ItemTemplate>
        <AlternatingItemTemplate>
            <div style="background-color: lightgray;">
                <uc1:ctl_Farm_Location_Description runat="server" ID="ctl_Farm_Location_Description" />
            </div>
        </AlternatingItemTemplate>
    </asp:Repeater>
</div>