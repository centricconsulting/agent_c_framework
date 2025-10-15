<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Coverages_HOM_App_Section.ascx.vb" Inherits="IFM.VR.Web.ctl_Coverages_HOM_App_Section" %>

<div id="divCoverageMain" runat="server">
    <h3 id="headerBar" runat="server">
        <asp:Label ID="lblHeader" runat="server" Text="Header"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearCoverage" CssClass="RemovePanelLink" ToolTip="Clear this Coverage" runat="server">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveCoverages" CssClass="RemovePanelLink" ToolTip="Save ALL Coverages" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="divCoverageContent" runat="server">


        <asp:HiddenField ID="hiddenCoverageAccordActive" runat="server" />
        <asp:HiddenField ID="hiddenActiveCoverageItem" runat="server" />

    </div>
</div>