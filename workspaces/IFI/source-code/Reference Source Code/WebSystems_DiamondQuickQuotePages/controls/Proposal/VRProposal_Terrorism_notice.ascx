<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_Terrorism_notice.ascx.vb" Inherits="controls_Proposal_VRProposal_Terrorism_notice" %>
<%@ Register Src="~/controls/Proposal/VRProposal_Footer.ascx" TagPrefix="uc" TagName="VRProposal_Footer" %> <%--added 3/22/2021--%>

<style type="text/css">
    p.proposalTerrorism {
        font-family: "Segoe UI";
        text-align: justify;
        font-size: 18px;
        font-weight: normal;
    }

    p.shortTextProposalCenter {
        font-family: "Segoe UI";
        text-align: center;
        font-size: 18px;
        font-weight: bold;
    }

    p.shortTextProposalLeft {
        font-family: "Segoe UI";
        text-align: left;
        font-size: 18px;
        font-weight: normal;
    }

    p.shortTextProposalRight {
        font-family: "Segoe UI";
        text-align: right;
        font-size: 18px;
        font-weight: normal;
    }
    hr{
        color:burlywood;
        height: 1px;
        background-color: burlywood;
        border: none;
    }
    .footerdiv {
        margin-top:230px
    }

</style>
<br />
<table class="quickQuoteSectionTable" style="display: none;">
    <tr>
        <td align="left">
            <img runat="server" id="MainPageLogo" class="QuickQuoteProposalHeaderLogo" />
        </td>
        <td style="text-align: right; padding-left: 100px;">
            <asp:Label runat="server" ID="lblLogo" class="QuickQuoteProposalHeaderLogo"></asp:Label>
        </td>
    </tr>
</table>
<table class="quickQuoteSectionTable">
    <tr>
        <br />
        <br />
        <p style="font-size: 18px; font-weight: normal;" class="tableRowHeader shortTextProposalCenter">
            In compliance with the Terrorism Risk Insurance Act, the following notice is provided for those<br />
            lines of business to which the Act applies.
        </p>
        <p class="tableField shortTextProposalRight">
            ML 1002 (11/20)
        </p>
    </tr>
    <tr>
        <p class="tableRowHeader shortTextProposalCenter">
            POLICYHOLDER DISCLOSURE NOTICE OF TERRORISM INSURANCE COVERAGE
        </p>
        <br />

        <p class="proposalTerrorism">
            Coverage for acts of terrorism is included in your policy. You are hereby notified that the Terrorism Risk Insurance Act, as       
   amended in 2019, defines an act of terrorism in Section 102(1) of the Act: The term <q>act of terrorism</q> means any act or acts        
   that are certified by the Secretary of the Treasury&mdash;in consultation with the Secretary of Homeland Security, and the Attorney       
   General of the United States&mdash;to be an act of terrorism; to be a violent act or an act that is dangerous to human life, property,     
   or infrastructure; to have resulted in damage within the United States, or outside the United States in the case of certain air      
   carriers or vessels or the premises of a United States mission; and to have been committed by an individual or individuals as part  
   of an effort to coerce the civilian population of the United States or to influence the policy or affect the conduct of the          
   United States Government by coercion. Under your coverage, any losses resulting from certified acts of terrorism may be partially    
   reimbursed by the United States Government under a formula established by the Terrorism Risk Insurance Act, as amended. However,     
   your policy may contain other exclusions which might affect your coverage, such as an exclusion for nuclear events. Under the        
   formula, the United States Government generally reimburses 80% beginning on January 1, 2020, of covered terrorism losses exceeding   
   the statutorily established deductible paid by the insurance company providing the coverage. The Terrorism Risk Insurance Act,       
   as amended, contains a $100 billion cap that limits U.S. Government reimbursement as well as insurers&rsquo; liability for losses          
   resulting from certified acts of terrorism when the amount of such losses exceeds $100 billion in any one calendar year. If the    
   aggregate insured losses for all insurers exceed $100 billion, your coverage may be reduced. However, these limitations do not
   apply to any personal, non-business, or non-farming activities or property of any kind covered anywhere in the policy.
        </p>
    </tr>
    <br />
    <tr>
        <p style="font-size: 20px;" class="tableRowHeader shortTextProposalLeft">
            There is no premium charge associated with this coverage.
        </p>
        <br />
        <br />
        <p class="tableField shortTextProposalLeft">
            ML 1002 (11/20)   Contains material copyrighted by the National Association of Insurance Commissioners, 2019
        </p>
    </tr>
</table>
<br /><br /><br />
<uc:VRProposal_Footer ID="footer1" runat="server" class="footer" />