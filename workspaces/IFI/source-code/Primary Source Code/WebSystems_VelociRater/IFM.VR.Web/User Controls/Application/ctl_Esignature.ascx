<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Esignature.ascx.vb" Inherits="IFM.VR.Web.ctl_Esignature" %>

<div id="divEsignature" runat="server">
    <h3><asp:Label ID="lblEsignatureTitle" runat="server" Text="Signature Information"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
    </span>
    </h3>
    <div>
        <asp:Label ID="lblEsignature" runat="server">Sign application using eSignature:</asp:Label>
        <asp:RadioButtonList runat="server" ID="rblEsignature" RepeatLayout="Flow" RepeatDirection="Horizontal">
            <asp:ListItem>Yes</asp:ListItem>
            <asp:ListItem>No</asp:ListItem>
        </asp:RadioButtonList>
        <br />
        <div id="divEmail">
            <asp:Label ID="lblEsigEmail" runat="server">*Email:</asp:Label>
            <asp:TextBox ID="txtEsigEmail" runat="server" MaxLength="100" Width="300px"></asp:TextBox>
        </div>
    </div>
</div>
<asp:HiddenField ID="accordActive" runat="server" />
<asp:HiddenField ID="hdnPriorEsigOption" runat="server" />
<asp:HiddenField ID="hdnPriorEmailAddress" runat="server" />
<asp:HiddenField ID="hdnPriorZipCodeStandard" runat="server" />