<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CoveragesAdditionalInterests_HOM_App.ascx.vb" Inherits="IFM.VR.Web.ctl_CoveragesAdditionalInterests_HOM_App" %>
<%@ Register Src="~/User Controls/Application/HOM/ctl_CoverageAddress_HOM_App_Item.ascx" TagPrefix="uc1" TagName="ctl_CoverageAddress_HOM_App_Item" %>

<div id="divAICoverageMain" runat="server">
    <h3 id="headerBar" runat="server">
        <asp:Label ID="lblHeader" runat="server" Text="Header"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearCoverage" CssClass="RemovePanelLink" ToolTip="Clear this Coverage" runat="server">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveCoverages" CssClass="RemovePanelLink" ToolTip="Save ALL Coverages" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="divAICoverageContent" runat="server">

        <div runat="server" id="divName" visible="false" style="display:inline-block;">
            <asp:Label ID="lblName" runat="server">Name</asp:Label>
            <br />
            <asp:TextBox ID="txtName" TextMode="MultiLine" Width="200" runat="server"></asp:TextBox>
        </div>
        <div runat="server" id="divDescription" visible="false" style="display:inline-block;">
            <asp:Label ID="lblDescription" runat="server">Description</asp:Label>
            <br />
            <asp:TextBox ID="txtDescription" TextMode="MultiLine" Width="200" runat="server"></asp:TextBox>
        </div>
        <div runat="server" id="divTrust" visible="false" style="display:block;padding-bottom:5px;">
            <asp:Label ID="lblTrustName" runat="server">Trust Name</asp:Label>
            <br />
            <asp:TextBox ID="txtTrustName" TextMode="MultiLine" Width="250" runat="server"></asp:TextBox>
        </div>
        <div runat="server" id="divTrustee" visible="false" style="display:block;">
            <asp:Label ID="lblTrusteeName" runat="server">Trustee Name</asp:Label>
            <br />
            <asp:TextBox ID="txtTrusteeName" TextMode="MultiLine" Width="250" runat="server"></asp:TextBox>
        </div>


        <div runat="server" id="divAIAddress" visible="false">
            <uc1:ctl_CoverageAddress_HOM_App_Item runat="server" ID="ctl_CoverageAddress_HOM_App_Item" />
        </div>
        <asp:HiddenField ID="hiddenAICoverageAccordActive" runat="server" />
    </div>
</div>