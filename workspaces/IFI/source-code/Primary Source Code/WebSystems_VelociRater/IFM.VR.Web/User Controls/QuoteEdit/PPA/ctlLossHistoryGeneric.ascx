<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlLossHistoryGeneric.ascx.vb" Inherits="IFM.VR.Web.ctlLossHistoryGeneric" %>

<div runat="server" id="divLossHistoryGenericParent" class="standardSubSection">
    <h3>
        <asp:Label ID="lblHeader" runat="server" Text="Loss History"></asp:Label>

        <span style="float: right">
            <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span>
    </h3>

    <div id="divLossHistoryGenericChild" runat="server">
        <asp:CheckBox runat="server" ID="cbOrderClueAtRate" Text="There has been an accident in the last three years." />
    </div>
</div>
<asp:HiddenField ID="accordActive" runat="server" />
