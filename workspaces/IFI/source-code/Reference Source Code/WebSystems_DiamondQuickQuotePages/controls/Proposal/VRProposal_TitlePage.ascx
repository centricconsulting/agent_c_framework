<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_TitlePage.ascx.vb" Inherits="controls_Proposal_VRProposal_TitlePage" %>


<%@ Register Src="~/controls/Proposal/VRProposal_Footer.ascx" TagPrefix="uc" TagName="VRProposal_Footer" %>
<%--added 3/22/2021--%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head>

     <style type="text/css">
        html,
        body {
            margin: 0;
            padding: 0;
            height: 100%;
        }

        #container {
            min-height: 100%;
            position: relative;
        }

        #body {
            /*	padding:10px;*/
            padding-bottom: 400px; /* Height of the footer */
        }

        #footer {
            position: absolute;
            bottom: 0px;
            width: 100%;
        }

        .rightAlign {
            text-align: right;
        }

        .pageTitlewidth {
            /*width:900px;*/
            margin-left: auto;
            margin-right: auto;
        }

        hr {
            color: burlywood;
            height: 1px;
            background-color: burlywood;
            border: none;
        }

        .proposalTitlePageSpacerAbove {
            margin-top: 100px;
        }

        .proposalTitlePageTitlebar {
            background-color: #00AFCF; 
            font-family: "Segoe UI";
            font-size: 24pt;
            font-weight: bolder;
            text-align: center; 
            padding: 10px; 
            color: white; 
            border-radius: 20px;
            margin-top: 24px;
        }

        .proposalTitlePageSectionHeader {
            color: #000000;
            font-family: "Segoe UI";
            font-size: 12pt;
            font-weight: normal;
            text-align: center;
            margin-bottom: 24px;
        }

        .proposalTitlePageClientInfo {
            color: #000000;
            font-family: "Segoe UI";
            font-size: 20pt;
            font-weight: bolder;
            text-align: center;
        }
        .proposalTitlePageAgencyInfo {
            color: #000000;
            font-family: "Segoe UI";
            font-size: 20pt;
            font-weight: bolder;
            text-align: center;
        }
        .proposalTitlePageName {
            color: #000000;
            font-family: "Segoe UI";
            font-size: 20pt;
            font-weight: bold;
            text-align: center;
        }
        .proposalTitlePageAddress {
            color: #000000;
            font-family: "Segoe UI";
            font-size: 14pt;
            font-weight: normal;
            text-align: center;
        }
        .proposalTitlePagePhoneNumber {
            color: #000000;
            font-family: "Segoe UI";
            font-size: 14pt;
            font-weight: normal;
            text-align: center;
        }

        .proposalTitlePageQuotedDateHeader {
            color: #000000;
            font-family: "Segoe UI";
            font-size: 12pt;
            font-weight: normal;
            text-align: center;
        }

        .proposalTitlePageQuotedDate {
            color: #000000;
            font-family: "Segoe UI";
            font-size: 12pt;
            font-weight: normal;
            text-align: center;
        }

        .proposalTitlePageAgencyLogo {
            max-height: 2in;
            max-width: 4in;
            object-fit: contain;
        }

    </style>
</head>
<body>
    <form id="form1">
        <div id="container">

            <div id="body">
                <div class="proposalTitlePageTitlebar">
                    Commercial Insurance Proposal
                </div>

                <div class="proposalTitlePageSectionHeader proposalTitlePageSpacerAbove">
                    Prepared for:
                </div>

                <div class="proposalTitlePageClientInfo">
                    <asp:Label runat="server" ID="lblClientInfo"></asp:Label>
                </div>
               
                <div class="proposalTitlePageSectionHeader proposalTitlePageSpacerAbove">
                    Prepared by:
                </div>

                <div class="proposalTitlePageAgencyInfo">
                    <asp:Label runat="server" ID="lblAgencyInfo"></asp:Label>
                    <asp:Label runat="server" ID="lblLogo" class="proposalTitlePageAgencyLogo"></asp:Label>
                </div>

                 <div class="proposalTitlePageQuotedDateHeader proposalTitlePageSpacerAbove">
                    Quoted on:
                </div>
                <div class="proposalTitlePageQuotedDate">
                    <asp:Label runat="server" ID="lblDateText"></asp:Label>
                </div>
            </div>

            <div id="footer">
                <uc:VRProposal_Footer ID="footer1" runat="server" />
            </div>
        </div>
    </form>
</body>
</html>