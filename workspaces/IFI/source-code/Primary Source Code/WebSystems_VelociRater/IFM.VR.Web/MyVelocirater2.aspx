<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MyVr.Master" CodeBehind="MyVelocirater2.aspx.vb" Inherits="IFM.VR.Web.MyVelocirater2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">       
   <div ng-view ></div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphBodyScripts" runat="server">
    <script src="Src/Routes.js"></script>
    <script src="Src/MyVr/controls/Home/Home.js"></script>
    
    <script src="Src/MyVr/controls/Quote/QuoteSearchController.js"></script>    

    <script src="Src/MyVr/controls/Endorsements/EndorsementMasterController.js"></script>
    <script src="Src/MyVr/controls/Endorsements/EndorsementSearcherController.js"></script>
    <script src="Src/MyVr/controls/Endorsements/EndorsementSearchResultController.js"></script>
</asp:Content>
