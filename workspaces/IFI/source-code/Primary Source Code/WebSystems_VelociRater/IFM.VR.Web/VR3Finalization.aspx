<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/VelociRater.Master" CodeBehind="VR3Finalization.aspx.vb" Inherits="IFM.VR.Web.VR3Finalization" %>

<%@ Import Namespace="IFM.VR.Web" %>

<asp:Content ID="script" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script>
     $(document).ready(function () {
         $("#divFinalize").accordion({ heightStyle: "content", active: 0, collapsible: false });
     });
    </script>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="cphMain" runat="server">
    <br />
    <h3>The following applications have been finalized.</h3>
    <br />
    <div id="divFinalize">
        <asp:Literal ID="litFrames" runat="server"></asp:Literal>
    </div>
</asp:Content>