<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Coverages_HOM_App_Item.ascx.vb" Inherits="IFM.VR.Web.ctl_Coverages_HOM_App_Item" %>
<%@ Register Src="~/User Controls/Application/HOM/ctl_CoverageAddress_HOM_App_Item.ascx" TagPrefix="uc1" TagName="ctl_CoverageAddress_HOM_App_Item" %>
<%@ Register Src="~/User Controls/Application/HOM/ctl_CoveragesAdditionalInterestsList_HOM_App.ascx" TagPrefix="uc1" TagName="ctl_CoveragesAdditionalInterestsList_HOM_App" %>

<div id="divCoverageMain" runat="server">
    <h3 id="headerBar" runat="server">
        <asp:Label ID="lblHeader" runat="server" Text="Header"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkClearCoverage" CssClass="RemovePanelLink" ToolTip="Clear this Coverage" runat="server">Clear</asp:LinkButton>
            <asp:LinkButton ID="lnkSaveCoverages" CssClass="RemovePanelLink" ToolTip="Save ALL Coverages" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="divCoverageContent" runat="server">
        <div runat="server" id="divName" visible="false" style="display:inline-block;">
            <asp:Label ID="lblName" runat="server">Name</asp:Label>
            <br />
            <asp:TextBox ID="txtName" TextMode="MultiLine" Width="200" runat="server"></asp:TextBox>
            <br /><br />
        </div>
        <div runat="server" id="divDescription" visible="false" style="display:inline-block;">
            <asp:Label ID="lblDescription" runat="server">*Description</asp:Label>
            <br />
            <asp:TextBox ID="txtDescription" TextMode="MultiLine" Width="200" runat="server"></asp:TextBox>
            <br />
            <asp:Label ID="lblMaxChar" runat="server" Text="Max Characters:" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>&nbsp;
            <asp:Label ID="lblMaxCharCount" runat="server" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>
        </div>
        <div runat="server" id="divDescription2" visible="false" style="display:inline-block;">
             <asp:Label ID="lblDescription2" runat="server">Other Structures:<br />*Building Description</asp:Label>
            <br />
            <asp:TextBox ID="txtDescription2" TextMode="MultiLine" Width="200" runat="server"></asp:TextBox>            
            <br />
            <asp:Label ID="lblMaxChar2" runat="server" Text="Max Characters:" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>&nbsp;
            <asp:Label ID="lblMaxCharCount2" runat="server" Font-Bold="True" Font-Size="XX-Small" ForeColor="Red"></asp:Label>
        </div>


        <asp:HiddenField ID="hiddenCoverageAccordActive" runat="server" />
        <asp:HiddenField ID="hiddenActiveCoverageItem" runat="server" />
        <asp:HiddenField ID="hiddenMaxCharCount" runat="server" />
        <asp:HiddenField ID="hiddenMaxCharCount2" runat="server" />

        <div runat="server" id="divAddress" visible="false">
            <uc1:ctl_CoverageAddress_HOM_App_Item runat="server" ID="ctl_CoverageAddress_HOM_App_Item" />
        </div>
        <div id="divAppSpecialText" runat="server" style="display:none;">
            <center><div style="color:blue;text-align:center;width:365px;"><asp:Label ID="lblAppSpecialText" runat="server" Text=""></asp:Label></div></center>
        </div>

    </div>
    
</div>
<div id="divAdditionalInterest" runat="server">
    <uc1:ctl_CoveragesAdditionalInterestsList_HOM_App runat="server" ID="ctl_CoveragesAdditionalInterestsList_HOM_App" />
</div>