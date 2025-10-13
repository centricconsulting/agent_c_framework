<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_Summary.ascx.vb" Inherits="controls_Proposal_VRProposal_Summary" %>

<%@ Register src="VRProposal_ClientAndAgencyInfo.ascx" tagname="VRProposal_ClientAndAgencyInfo" tagprefix="VR" %>
<%@ Register src="VRProposal_Disclaimer.ascx" tagname="VRProposal_Disclaimer" tagprefix="VR" %>
<%@ Register src="VRProposal_BOP_Summary.ascx" tagname="VRProposal_BOP_Summary" tagprefix="VR" %>
<%@ Register src="VRProposal_WCP_Summary.ascx" tagname="VRProposal_WCP_Summary" tagprefix="VR" %>
<%@ Register src="VRProposal_CGL_Summary.ascx" tagname="VRProposal_CGL_Summary" tagprefix="VR" %>
<%@ Register src="VRProposal_CPR_Summary.ascx" tagname="VRProposal_CPR_Summary" tagprefix="VR" %>
<%@ Register src="VRProposal_CPP_Summary.ascx" tagname="VRProposal_CPP_Summary" tagprefix="VR" %>
<%@ Register src="VRProposal_CAP_Summary.ascx" tagname="VRProposal_CAP_Summary" tagprefix="VR" %>
<%--added CIM and CRM controls 8/19/2015; currently only used for CPP--%>
<%@ Register src="VRProposal_CIM_Summary.ascx" tagname="VRProposal_CIM_Summary" tagprefix="VR" %>
<%@ Register src="VRProposal_CRM_Summary.ascx" tagname="VRProposal_CRM_Summary" tagprefix="VR" %>
<%--added generic control 4/21/2017--%>
<%@ Register src="VRProposal_LOB_Generic_Summary.ascx" tagname="VRProposal_LOB_Generic_Summary" tagprefix="VR" %>
<%--added GAR control 4/22/2017--%>
<%@ Register src="VRProposal_GAR_Summary.ascx" tagname="VRProposal_GAR_Summary" tagprefix="VR" %>

<br />
<div class="tableField">
<img runat="server" id="HeaderLogo" class="QuickQuoteProposalHeaderLogo" />
<br />
<br />
<p width="100%" align="center" class="tableRowHeaderLarger">
    Commercial Insurance Quote Summary
</p>
<VR:VRProposal_ClientAndAgencyInfo runat="server" ID="VRProposal_ClientAndAgencyInfo1" />
<p width="100%" align="center" class="tableRowHeader">
    Total Quoted Premiums
</p>
<asp:PlaceHolder runat="server" ID="phControls"></asp:PlaceHolder>

</div>
<p width="100%" align="center" class="tableRowHeader">
    Total Combined Premium:  <asp:Label runat="server" ID="lblCombinedPremium"></asp:Label>
</p>
<br />
<br /><%--may comment these 2 breaks out--%>
<VR:VRProposal_Disclaimer runat="server" ID="VRProposal_Disclaimer1" />
<br />