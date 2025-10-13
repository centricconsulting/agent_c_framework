<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CGL_ClassCodeList.ascx.vb" Inherits="IFM.VR.Web.ctl_CGL_ClassCodeList" %>
<%@ Register Src="~/User Controls/VR Commercial/Quote/CGL/ClassCode/ctl_CGL_Classcode.ascx" TagPrefix="uc1" TagName="ctl_CGL_Classcode" %>

<div runat="server" id="divNewClassCode">
    <uc1:ctl_CGL_Classcode runat="server" HideFromParent="True" id="ctl_CGL_Classcode" />
</div>

<div runat="server" id="divAccord">
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <uc1:ctl_CGL_Classcode runat="server" id="ctl_CGL_Classcode" />
        </ItemTemplate>
    </asp:Repeater>
</div>
<asp:HiddenField ID="hdnAccord" runat="server" />
