<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Coverages_HOM_App.ascx.vb" Inherits="IFM.VR.Web.ctl_Coverages_HOM_App" %>
<%@ Register Src="~/User Controls/Application/HOM/ctl_Coverages_HOM_App_Item.ascx" TagPrefix="uc1" TagName="ctl_Coverages_HOM_App_Item" %>
<div id="divCoveragesMain" runat="server">
    <h3>
        <asp:Label ID="lblMainAccord" runat="server" Text="Updates"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkSave" runat="server" ToolTip="Save Page" CssClass="RemovePanelLink">Save</asp:LinkButton>
        </span>
    </h3>
    <div id="divCoveragesContent" runat="server">


        <div id="divCoveragesSections" runat="server" style="margin-top: 15px;">
            <div runat="server" id="divSectionI">
            <asp:Repeater ID="covSectionIRepeater" runat="server">
                <ItemTemplate>
                    <uc1:ctl_Coverages_HOM_App_Item runat="server" ID="ctl_Coverages_HOM_App_Item" />
                </ItemTemplate>
            </asp:Repeater>
            </div>            

            
            <div runat="server" id="divSectionII">
            <asp:Repeater ID="covSectionIIRepeater" runat="server">
                <ItemTemplate>
                    <uc1:ctl_Coverages_HOM_App_Item runat="server" ID="ctl_Coverages_HOM_App_Item" />
                </ItemTemplate>
            </asp:Repeater>
            </div>
            
            
            <div runat="server" id="divSectionIandII">
            <asp:Repeater ID="covSectionIAndIIRepeater" runat="server">
                <ItemTemplate>
                    <uc1:ctl_Coverages_HOM_App_Item runat="server" ID="ctl_Coverages_HOM_App_Item" />
                </ItemTemplate>
            </asp:Repeater>
            </div>
        </div>

        <asp:HiddenField ID="hiddenCoveragesAccordActive" runat="server" />
        <asp:HiddenField ID="hiddenActiveCoverage" runat="server" />
        <asp:HiddenField ID="hiddenHasAppGapCoverages" runat="server" />
    </div>
</div>
