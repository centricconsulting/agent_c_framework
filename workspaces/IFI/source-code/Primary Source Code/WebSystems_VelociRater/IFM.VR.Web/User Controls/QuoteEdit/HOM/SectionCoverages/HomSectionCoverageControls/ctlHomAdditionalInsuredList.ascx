<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomAdditionalInsuredList.ascx.vb" Inherits="IFM.VR.Web.ctlHomAdditionalInsuredList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomAdditionalInsured.ascx" TagPrefix="uc1" TagName="ctlHomAdditionalInsured" %>

<uc1:ctlHomAdditionalInsured runat="server" id="ctlHomAdditionalInsured" />
<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <div style="background-color:lightgray;">
            <uc1:ctlHomAdditionalInsured runat="server" id="ctlHomAdditionalInsured" />
        </div>        
    </ItemTemplate>
    <AlternatingItemTemplate>
        <uc1:ctlHomAdditionalInsured runat="server" id="ctlHomAdditionalInsured" />
    </AlternatingItemTemplate>
</asp:Repeater>
<div>
    <div style="float:right"><asp:LinkButton ID="lnkAddAddress" runat="server">Add Another Student</asp:LinkButton></div>
</div>