<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomFarmLandList.ascx.vb" Inherits="IFM.VR.Web.ctlHomFarmLandList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomFarmLand.ascx" TagPrefix="uc1" TagName="ctlHomFarmLand" %>




<uc1:ctlHomFarmLand runat="server" id="ctlHomFarmLand" />
<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <div style="background-color:lightgray;">
            <uc1:ctlHomFarmLand runat="server" id="ctlHomFarmLand" />
        </div>        
    </ItemTemplate>
    <AlternatingItemTemplate>
        <uc1:ctlHomFarmLand runat="server" id="ctlHomFarmLand" />
    </AlternatingItemTemplate>
</asp:Repeater>
<div>
    <div style="float:right"><asp:LinkButton ID="lnkAddAddress" runat="server">Add Another Location</asp:LinkButton></div>
</div>