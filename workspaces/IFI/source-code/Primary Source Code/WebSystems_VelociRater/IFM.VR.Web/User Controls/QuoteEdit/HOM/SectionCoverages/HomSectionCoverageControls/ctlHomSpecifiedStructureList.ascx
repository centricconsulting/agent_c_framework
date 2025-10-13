<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomSpecifiedStructureList.ascx.vb" Inherits="IFM.VR.Web.ctlHomSpecifiedStructureList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomSpecifiedStructure.ascx" TagPrefix="uc1" TagName="ctlHomSpecifiedStructure" %>


<uc1:ctlHomSpecifiedStructure runat="server" ID="ctlHomSpecifiedStructure" />

<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <div style="background-color:lightgray;">
            <uc1:ctlHomSpecifiedStructure runat="server" ID="ctlHomSpecifiedStructure" />
        </div>        
    </ItemTemplate>
    <AlternatingItemTemplate>
        <uc1:ctlHomSpecifiedStructure runat="server" ID="ctlHomSpecifiedStructure" />
    </AlternatingItemTemplate>
</asp:Repeater>
<div>
    <div style="float:right"><asp:LinkButton ID="lnkAddAddress" runat="server">Add Another Location</asp:LinkButton></div>
</div>