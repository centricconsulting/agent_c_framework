<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="" CodeFile="DiamondQuoteProposal.aspx.vb" Inherits="DiamondQuoteProposal" %>

<%@ Register src="controls/Proposal/VRProposal_ClientAndAgencyInfo.ascx" tagname="VRProposal_ClientAndAgencyInfo" tagprefix="VR" %>
<%@ Register src="controls/Proposal/VRProposal_PaymentOptions.ascx" tagname="VRProposal_PaymentOptions" tagprefix="VR" %>
<%@ Register src="controls/Proposal/VRProposal_TitlePage.ascx" tagname="VRProposal_TitlePage" tagprefix="VR" %>
<%@ Register src="controls/Proposal/VRProposal_Summary.ascx" tagname="VRProposal_Summary" tagprefix="VR" %>
<%@ Register src="controls/Proposal/VRProposal_PaymentOptions_Generic.ascx" tagname="VRProposal_PaymentOptions_Generic" tagprefix="VR" %><%--added 7/11/2017--%>
<%@ Register src="controls/Proposal/VRProposal_PaymentOptions_Actual.ascx" tagname="VRProposal_PaymentOptions_Actual" tagprefix="VR" %><%--added 9/16/2017--%>
<%@ Register src="controls/Proposal/VRProposal_AboutUs.ascx" tagName="VRProposal_AboutUs" tagPrefix="VR" %> <%--added 2/15/2021--%>
<%@ Register src="controls/Proposal/VRProposal_Terrorism_notice.ascx" tagname="VRProposal_Terrorism_notice" tagprefix="VR" %><%--added 2/15/2021--%>
<%@ Register src="controls/Proposal/VRProposal_NewPayment_Page.ascx" tagname="VRProposal_NewPayment_Page" tagprefix="VR" %><%--added 9/16/2017--%>


<asp:Content ID="Content2" ContentPlaceHolderID="Scripts" runat="server">
    <title>Diamond Quote Proposal</title>
<script type="text/javascript" src="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.2.min.js"></script>
<%--    <a href="controls/Proposal/VRProposal_PaymentOptions_Generic.ascx.vb">controls/Proposal/VRProposal_PaymentOptions_Generic.ascx.vb</a>--%>
    <reference path="https://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.8.2-vsdoc.js"/>
    <script type="text/javascript" src="https://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.0/jquery-ui.min.js"></script>
    <link rel="stylesheet" type="text/css" href="https://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.0/themes/smoothness/jquery-ui.css" />
    <script type="text/javascript" src="js/CGL-general.js"></script>    
    <link rel="stylesheet" type="text/css" href="Styles/CGLStyle.css" />
    <link rel="stylesheet" type="text/css" href="Styles/Vr_NearMono.css" />
     <!--[if lt IE 9]>
	    <link rel="stylesheet" type="text/css" href="Styles/Vr_NearMono_Legacy.css" />
    <![endif]-->
    
    <!--[if gte IE 9]>
        <link rel="stylesheet" type="text/css" href="Styles/Vr_NearMono_IE_9_Only.css" />
    <![endif]-->
    <script type="text/javascript">
        //changed = to #; need Page.Header.DataBind() on Page_Load
        var hdnPdfFilePath = '<%#hdnPdfFilePath.ClientID%>';
        var pdfButtonArea = '<%#pdfButtonArea.ClientID%>';

//        function openPdf() {
//            if (document.getElementById(hdnPdfFilePath)) {
//                var PdfFilePath = document.getElementById(hdnPdfFilePath).value;
//                if (PdfFilePath == (0 || "0" || "")) {
//                    return;
//                } else {
//                    if (UrlExists(PdfFilePath)) {
//                        //if (fileExists(PdfFilePath)) {
//                        window.open(PdfFilePath);
//                    } else {
//                        return;
//                    }
//                }
//            }
//        }
        //function openPdf() {
        //renamed 5/30/2013
        function showButtonToOpenPdfWindow() {
            if (document.getElementById(pdfButtonArea)) {
                if (document.getElementById(hdnPdfFilePath)) {
                    var PdfFilePath = document.getElementById(hdnPdfFilePath).value;
                    if (PdfFilePath == (0 || "0" || "")) {
                        return;
                    } else {
                        if (UrlExists(PdfFilePath)) {
                            //if (fileExists(PdfFilePath)) {
                            //window.open(PdfFilePath);
                            document.getElementById(pdfButtonArea).style.display = "table";
                        } else {
                            return;
                        }
                    }
                }                
            }
        }
        function openPdfWindow() {
            if (document.getElementById(hdnPdfFilePath)) {
                var PdfFilePath = document.getElementById(hdnPdfFilePath).value;
                if (PdfFilePath == (0 || "0" || "")) {
                    return;
                } else {
                    if (UrlExists(PdfFilePath)) {
                        //if (fileExists(PdfFilePath)) {
                        window.open(PdfFilePath);
                    } else {
                        return;
                    }
                }
            }
        }
        function openPdf() {
            if (document.getElementById(hdnPdfFilePath)) {
                var PdfFilePath = document.getElementById(hdnPdfFilePath).value;
                if (PdfFilePath == (0 || "0" || "")) {
                    return;
                } else {
                    if (UrlExists(PdfFilePath)) {
                        //if (fileExists(PdfFilePath)) {
                        window.location = PdfFilePath;
                    } else {
                        return;
                    }
                }
            }
        }

        function UrlExists(url) {
            var http = new XMLHttpRequest();
            http.open('HEAD', url, false);
            http.send();
            return http.status != 404;
        }
        function fileExists(url) {
            if (url) {
                var req = new XMLHttpRequest();
                req.open('GET', url, false);
                req.send();
                return req.status == 200;
            } else {
                return false;
            }
        }
    </script>
       

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Application" Runat="Server">
<%--<br />--%>
<div class="tableField" style="width:900px; margin-left:auto; margin-right:auto;">

    <asp:PlaceHolder runat="server" ID="phControls"></asp:PlaceHolder>
    <input id="hdnPdfFilePath" name="hdnPdfFilePath" type="hidden" runat="server" />
    <div id="pdfButtonArea" style="display:none; width:100%;" runat="server" class="hideFromPrinter">
        <br />
        <center><input type="button" value="Open as PDF" onclick="openPdfWindow()" class="quickQuoteButton hideFromPrinter" /></center>
    </div>
<!--using styles here to overwrite css file-->
<style type="text/css">
.tableFieldValue
{
font-family: Calibri;
font-size: 13pt;/*normally 10pt*/
font-weight: normal;
}
table.quickQuoteSectionTable 
{
font-family: Calibri;
font-size: 13pt;/*normally 10pt*/
/*font-weight: normal;*//*not needed*/
border-collapse: collapse;
}
.tableFieldHeader
{
font-family: Calibri;
/*font-size: 14pt;*//*normally 10pt*/
font-size: 13pt;/*changed*/
font-weight: bold;
}
.tableFieldHeaderLarger
{
font-family: "Segoe UI";
font-size: 14pt;/*normally 12pt*/
font-weight: bold;
}
.tableRowHeader
{
font-family: "Segoe UI";
font-size: 14pt;/*normally 12pt*/
font-weight: bolder;
}
.tableField
{
font-family: "Segoe UI";
font-size: 13pt;/*normally 10pt*/
}

/*added*/
.tableRowHeaderLarger
{
    font-family:"Segoe UI";
    font-size:15pt;/*normally 14pt*/
    font-weight:bolder;
}
</style>
</div>
</asp:Content>