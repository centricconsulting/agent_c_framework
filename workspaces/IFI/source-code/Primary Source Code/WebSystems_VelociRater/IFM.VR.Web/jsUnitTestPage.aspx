<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" CodeBehind="jsUnitTestPage.aspx.vb" Inherits="IFM.VR.Web.jsUnitTestPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">    
        <link href="styles/qunit-1.20.0.css" rel="stylesheet" />    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphBodyScripts" runat="server">
      <div id="qunit"></div>
  <div id="qunit-fixture"></div>    
        <script src="<%=ResolveClientUrl("~/js/3rdParty/qunit-1.20.0.js")%>"></script>    
    <script src="<%=ResolveClientUrl("~/js/unitTests/jsIFMVR.js")%>"></script>    
    
</asp:Content>
