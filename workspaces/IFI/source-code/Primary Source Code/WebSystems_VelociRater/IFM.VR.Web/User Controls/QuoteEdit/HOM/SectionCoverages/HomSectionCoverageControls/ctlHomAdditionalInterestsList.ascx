<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomAdditionalInterestsList.ascx.vb" Inherits="IFM.VR.Web.ctlHomAdditionalInterestsList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomAdditionalInterests.ascx" TagPrefix="uc1" TagName="ctlHomAdditionalInterests" %>



<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <uc1:ctlHomAdditionalInterests runat="server" id="ctlHomAdditionalInterests" />
    </ItemTemplate>
    <AlternatingItemTemplate>
        <div style="background-color:lightgray;">
            <uc1:ctlHomAdditionalInterests runat="server" id="ctlHomAdditionalInterests" />
        </div>        
    </AlternatingItemTemplate>
</asp:Repeater>
<div>
    <div style="float:right"><asp:LinkButton ID="lnkAddAddress" runat="server">Add Additional Relative</asp:LinkButton></div>
</div>

