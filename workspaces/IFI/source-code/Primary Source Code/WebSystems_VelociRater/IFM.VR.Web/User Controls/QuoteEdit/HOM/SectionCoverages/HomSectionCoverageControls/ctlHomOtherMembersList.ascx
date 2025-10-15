<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomOtherMembersList.ascx.vb" Inherits="IFM.VR.Web.ctlHomOtherMembersList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomOtherMembers.ascx" TagPrefix="uc1" TagName="ctlHomOtherMembers" %>


<uc1:ctlHomOtherMembers runat="server" id="ctlHomOtherMembers" />
<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <div style="background-color:lightgray;">
            <uc1:ctlHomOtherMembers runat="server" id="ctlHomOtherMembers" />
        </div>        
    </ItemTemplate>
    <AlternatingItemTemplate>
        <uc1:ctlHomOtherMembers runat="server" id="ctlHomOtherMembers" />
    </AlternatingItemTemplate>
</asp:Repeater>
<div>
    <div style="float:right"><asp:LinkButton ID="lnkAddAddress" runat="server">Add Another Member</asp:LinkButton></div>
</div>
