<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_BOP_App_AdditionalServices.ascx.vb" Inherits="IFM.VR.Web.ctl_BOP_App_AdditionalServices" %>

<div runat="server" id="divBeauticiansAdditionalServices" class="standardSubSection">
    <h3>
        <asp:Label ID="lblHeader" runat="server" Text="Beautician Professional Liability"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <asp:CheckBox id="chkManicures" runat="server" Text="Manicures" /><br />
        <asp:CheckBox id="chkPedicures" runat="server" Text="Pedicures" /><br />
        <asp:CheckBox id="chkWaxes" runat="server" Text="Waxes" /><br />             
        <asp:CheckBox id="chkThreading" runat="server" Text="Threading" /><br />
        <asp:CheckBox id="chkHairExt" runat="server" Text="Hair Extensions" /><br />
        <asp:CheckBox id="chkCosmetology" runat="server" Text="Cosmetology Services" /><br />    
        <asp:HiddenField ID="hdnAccord" runat="server" />
    </div>
</div>