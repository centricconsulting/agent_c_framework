<%@ Control Language="VB" AutoEventWireup="false" CodeFile="VRProposal_Footer.ascx.vb" Inherits="controls_Proposal_VRProposal_Footer" %>
<style type="text/css">
    .footerdiv {
        width: 100%;
    }

     .hr {
        color:burlywood;
        height: 1px;
        background-color: burlywood;
        border: none;
    }

     .footerLogo {
         display: block;
         margin-left: auto;
         max-height: 84px;
        object-fit: contain;
     }

     .footerAddress {
         font-family: "Segoe UI";
         display: block;
         text-align: left;
         opacity: 0.5;
         font-size: 20px;
     }

</style>


<%--<div id="footerdiv" style="display:flex" >
    <div class="footerAddress" style="opacity: 0.5; font-size: 20px; ">
        Indiana Farmers Insurance <br />
        10 West 106<sup>th</sup> Street, Indianapolis, IN 46290<br />
        www.indianafarmers.com
    </div>
    <div style="flex: 1;">
        <img runat="server" id="FooterLogo" class="footerLogo">
    </div>
   
    
</div>--%>

<table class="footerdiv">
    <tr>
        <td align="left">
            <div class="footerAddress">
                Indiana Farmers Insurance<br />
                10 West 106<sup>th</sup> Street, Indianapolis, IN 46290<br />
                www.indianafarmers.com
            </div>
        </td>
        <td>
                <img runat="server" id="FooterLogo" class="footerLogo">
        </td>
    </tr>
</table>
