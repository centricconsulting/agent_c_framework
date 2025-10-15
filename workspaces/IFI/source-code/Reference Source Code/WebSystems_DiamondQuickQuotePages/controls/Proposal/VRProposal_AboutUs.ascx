<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_AboutUs.ascx.vb" Inherits="controls_Proposal_VRProposal_AboutUs" %>
<%@ Register Src="~/controls/Proposal/VRProposal_Footer.ascx" TagPrefix="uc" TagName="VRProposal_Footer" %> <%--added 3/22/2021--%>

<style type="text/css">
    p.aboutUsHeader {
        text-align: justify;
        font-size: 24px;
        color: #00AFCF;
        font-family: calibri,Arial, Helvetica, sans-serif;
        font-weight: normal;
    }

    p.aboutUsText {
        text-align: justify;
        font-size: 22px;
        font-family: calibri,Arial, Helvetica, sans-serif;
        font-weight: normal;
    }

    img.aboutUsImage {
        padding-top: 30px;
        padding-left: 60px;
        width: 300px;
    }
    hr{
        color:burlywood;
        height: 1px;
        background-color: burlywood;
        border: none;
    }
</style>
<%--<br />--%>
 <table class="quickQuoteSectionTable">
     <tr>
         <td>
       <img src="~/images/AboutHeader.jpg" runat="server" id="AboutHeader"/>
        </td>
     </tr>
 </table>
 
<div> 
    <p class="tableRowHeader aboutUsHeader">
    We are a member service company. We only exist to serve you.
    </p>
   <p  class="tableField aboutUsText">
   Indiana Farmers Insurance is a mutual insurance company and exists only to serve our customers. We have no 
   stockholders who expect a return on their investment. We focus exclusively on the financial protection of our
   policyholders and are not burdened by the expectations of the financial markets. All this means that when you 
   buy a policy from Indiana Farmers Insurance, your premiums only go to pay for losses and to operate the 
   company.
  </p>
  <p class="tableRowHeader aboutUsHeader">
    We partner with only the best agents.</p>
 
  <p class="tableField aboutUsText">
   Insurance can be complicated so it is important to have a trusted expert who will guide you through your
   insurance experience. We partner with only the best agents, and like us, most of our agencies have been 
   serving customers for generations.</p>
</div>

<div>
    <p class="tableRowHeader aboutUsHeader">
    Our goal is to create the ultimate customer experience for you.
    </p>
    </div>
<div>
 <p style="text-align:left; font-size: 22px;">
   We want insurance to work for you. So, we invest in listening to you to understand your expectations; to make
   everything simple and easy; to put your experience first and foremost.</p>
</div>
 
<table class="quickQuoteSectionTable" >
  <tr >
  <td width:60%">
  <p class="tableRowHeader aboutUsHeader">
  You can expect the highest level of service from us.
  </p>
 <p class="aboutUsText">
  We will respond with urgency and attentiveness if you need
  assistance or have a claim.
 </p>
  </td>
  <td width:40%">
     <img src="~/images/AboutUs.png"  runat="server" id="AboutUs" class="QuickQuoteProposalHeaderLogo aboutUsImage" />
  </td>
  </tr>
 </table>

 <table class="quickQuoteSectionTable">
 <tr>
    <td>
   <p class="tableRowHeader aboutUsHeader">
   Your Business
   </p>
  </td>
 </tr>
<tr>
<td style="font-size:22px;" class="tableField aboutUsText">
Managing risk can be the difference between a growing business and going out of business. Together with
your expert insurance agent, we can help you manage risks so you can manage your bottom line. Every 
business is unique. We can help with insurance solutions tailored specifically for your needs with coverage for
your building, signs, vehicles, office equipment, your employees, and all the other things that keep you up at
night.
</td>
</tr>
</table>
<br /><br /><br />
<uc:VRProposal_Footer ID="footer1" runat="server" />