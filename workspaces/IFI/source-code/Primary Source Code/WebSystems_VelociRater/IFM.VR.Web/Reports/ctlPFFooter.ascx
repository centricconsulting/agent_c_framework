<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPFFooter.ascx.vb" Inherits="IFM.VR.Web.ctlPFFooter" %>
<br />
<section style="page-break-inside: avoid;">
<hr />
<p>
 Premiums shown are estimates only.  They are subject to change based on information that we may receive later 
in the application process.  Quoted premium does not include any service charges that may be applied for monthly, 
quarterly or semi-annual payment plans. This quote is subject to underwriting approval. </p>

<p runat="server" ID="ExpireContainer" Visible="false"><asp:Label runat="server" ID="ExpireNotice"></asp:Label></p>

<p><b>This is a proposal to provide insurance coverage.  This is not a contract of insurance</b></p>

<asp:Label runat="server" ID="DateToday"></asp:Label>
</section>