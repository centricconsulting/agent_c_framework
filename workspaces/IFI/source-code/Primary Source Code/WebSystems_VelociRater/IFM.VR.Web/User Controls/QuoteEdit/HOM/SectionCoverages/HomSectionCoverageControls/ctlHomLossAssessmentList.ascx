<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlHomLossAssessmentList.ascx.vb" Inherits="IFM.VR.Web.ctlHomLossAssessmentList" %>
<%@ Register Src="~/User Controls/QuoteEdit/HOM/SectionCoverages/HomSectionCoverageControls/ctlHomLossAssessment.ascx" TagPrefix="uc1" TagName="ctlHomLossAssessment" %>


<uc1:ctlHomLossAssessment runat="server" id="ctlHomLossAssessment" />
<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
        <div style="background-color:lightgray;">
            <uc1:ctlHomLossAssessment runat="server" id="ctlHomLossAssessment" />
        </div>        
    </ItemTemplate>
    <AlternatingItemTemplate>
        <uc1:ctlHomLossAssessment runat="server" id="ctlHomLossAssessment" />
    </AlternatingItemTemplate>
</asp:Repeater>
<div>
    <div style="float:right"><asp:LinkButton ID="lnkAddAddress" runat="server">Add Additional Location</asp:LinkButton></div>
</div>

