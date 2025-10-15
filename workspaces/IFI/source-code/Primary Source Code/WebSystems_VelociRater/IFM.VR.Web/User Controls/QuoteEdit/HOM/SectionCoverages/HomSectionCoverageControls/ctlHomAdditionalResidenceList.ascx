<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomAdditionalResidenceList.ascx.vb" Inherits="IFM.VR.Web.ctlHomAdditionalResidenceList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomAdditionalResidence.ascx" TagPrefix="uc1" TagName="ctlHomAdditionalResidence" %>


<uc1:ctlHomAdditionalResidence runat="server" id="ctlHomAdditionalResidence" />
<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <div style="background-color:lightgray;">
            <uc1:ctlHomAdditionalResidence runat="server" id="ctlHomAdditionalResidence" />
        </div>        
    </ItemTemplate>
    <AlternatingItemTemplate>
        <uc1:ctlHomAdditionalResidence runat="server" id="ctlHomAdditionalResidence" />
    </AlternatingItemTemplate>
</asp:Repeater>
<div>
    <div style="float:right"><asp:LinkButton ID="lnkAddAddress" runat="server">Add Another Location</asp:LinkButton></div>
</div>