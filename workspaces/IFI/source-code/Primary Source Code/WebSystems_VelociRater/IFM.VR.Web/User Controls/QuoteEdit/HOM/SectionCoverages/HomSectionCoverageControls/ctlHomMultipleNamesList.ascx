<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomMultipleNamesList.ascx.vb" Inherits="IFM.VR.Web.ctlHomMultipleNamesList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomMultipleNames.ascx" TagPrefix="uc1" TagName="ctlHomMultipleNames" %>


<uc1:ctlHomMultipleNames runat="server" id="ctlHomMultipleNames" />
<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <div style="background-color:lightgray;">
            <uc1:ctlHomMultipleNames runat="server" id="ctlHomMultipleNames" />
        </div>        
    </ItemTemplate>
    <AlternatingItemTemplate>
        <uc1:ctlHomMultipleNames runat="server" id="ctlHomMultipleNames" />
    </AlternatingItemTemplate>
</asp:Repeater>
<div>
    <div style="float:right"><asp:LinkButton ID="lnkAddAddress" runat="server">Add Additional Name</asp:LinkButton></div>
</div>
<div id="divSpecialText" runat="server" style="color:blue;float:left;text-align:center;width:365px;"><asp:Label ID="lblSpecialText" runat="server" Text=""></asp:Label></div>
