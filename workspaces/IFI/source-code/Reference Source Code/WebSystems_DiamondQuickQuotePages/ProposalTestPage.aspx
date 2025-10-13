<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ProposalTestPage.aspx.vb" Inherits="ProposalTestPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Proposal Test Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=1921" target="_blank">Normal Proposal Page</a>
    <br />
    <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=497859&generatePDF=No" target="_blank">Proposal Page w/o PDF</a>
    <br />
    <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=497859&generatePDF=No" target="_blank">MARTIN TEST</a>
    <br />
    <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=380|314|1368|394|1281|1307&lookForExistingPDF=No" target="_blank">Normal Proposal Page w/o lookup for existing</a>
    <br />
    <a href="DiamondQuoteProposalLoader.aspx?proposalId=803" target="_blank">Proposal Loader</a>
    <br />
    <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=34317&generatePDF=No" target="_blank">Test CIM/CRM 34317</a>
    <br />
    <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=34384&generatePDF=No" target="_blank">Test CIM/CRM 34384</a>
    <br />
    <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=34446&generatePDF=No" target="_blank">Test CIM/CRM 34446</a>
    <br />
    <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=34384&lookForExistingPDF=No" target="_blank">34384 new pdf</a>
        <br />
    <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=34384&generatePDF=No" target="_blank">34384 no pdf</a>
    <br />
    <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=1921&lookForExistingPDF=No" target="_blank">1921 new pdf</a>
        <br />
    <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=395744&generatePDF=No" target="_blank">395744 no pdf</a>
    <br />
     <a href="DiamondQuoteProposalLoader.aspx?VRProposal_AboutUs.ascx" target="_blank">About Us</a>
    <br />
     <a href="DiamondQuoteProposalLoader.aspx?VRProposal_Terrorism_Notice.ascx" target="_blank">Notice of Terrorism</a>
    <br />
     <a href="DiamondQuoteProposalLoader.aspx?VRPersonal_NewPayment_Page.ascx?" target="_blank">Payment Page</a>
    <br />
    <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=506754&generatePDF=No" target="_blank">Mary Test</a>
    <br />
        <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=1921&generatePDF=No" target="_blank">Chad Test - Web Edit</a>
    <br /><br />
        <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=1921&lookForExistingPDF=No" target="_blank">Chad Test - Gen New PDF</a>
    <br />
     <a href="DiamondQuoteProposal.aspx?PrinterFriendlyQuoteIds=5267&lookForExistingPDF=No" target="_blank">Bouba Test - Gen New PDF</a>
<br />
        
    </div>
    </form>
</body>
</html>
