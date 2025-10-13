<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPFHeader.ascx.vb" Inherits="IFM.VR.Web.ctlPFHeader" %>
<br />
<%--<table width="100%" class="quickQuoteSectionTable" style="border-style:solid; padding:5px; border-spacing:initial;">
    <tr>
        <td width="30%" valign="top">
            <asp:Label ID="lblPrepared" runat="server" Text="Prepared for:"></asp:Label>
            <br />
            <asp:Label runat="server" ID="lblClientInfo"></asp:Label>
        </td>
        <td width="40%" valign="top">

        </td>
        <td width="30%" valign="top" style="border-left-style:solid;">
            <asp:Label ID="lblAgent" runat="server" Text="Prepared by:"></asp:Label>
            <br />
            <asp:Label runat="server" ID="lblAgencyInfo"></asp:Label>
            <br />
            <asp:Panel ID="pnlProducer" runat="server">
                <asp:Label ID="lblProducer" runat="server" Text="Producer Code: "></asp:Label>
                <asp:Label ID="lblProducerCode" runat="server"></asp:Label>
            </asp:Panel>
        </td>
    </tr>
</table>--%>
<table width="100%" class="quickQuoteSectionTable" style="border-style:solid; border-collapse:collapse;">
    <tr>
        <td width="70%" valign="top" style="border-style:solid; border-collapse:collapse; padding:5px;">
            <table width="100%">
                <tr>
                    <td width="40%" valign="top">
                        <asp:Label ID="lblPrepared" runat="server" Text="Prepared for:"></asp:Label>
                        <br />
                        <asp:Label runat="server" ID="lblClientInfo"></asp:Label>
                    </td>
                    <td width="60%" valign="top" style="text-align:center;">
                        <%--<div style="text-align:center" width="100%"><img src="../images/2017 Black Indiana Farmers Insurance B&W Garfield.jpg" style="height:147px; width:395px" /></div>--%>
                        <%--<center><img src="../images/2017 Black Indiana Farmers Insurance B&W Garfield.jpg" style="height:147px; width:395px" /></center>--%>
                        <img src="../images/2017 Black Indiana Farmers Insurance B&W Garfield.jpg" style="height:130px; width:353px" />
                        <div style="position: relative;top: -30px;left: -10px;font-size: 11px;">10 West 106<sup>th</sup> Street Indianapolis, IN 46290</div>
                        <%--<center><img src="../images/2017 Black Indiana Farmers Insurance B&W Garfield.jpg" height="50%" width="50%" /></center>--%>
                    </td>
                </tr>
            </table>
        </td>        
        <td width="30%" valign="top" style="border-style:solid; border-collapse:collapse; padding:5px;">
            <asp:Label ID="lblAgent" runat="server" Text="Prepared by:"></asp:Label>
            <br />
            <asp:Label runat="server" ID="lblAgencyInfo"></asp:Label>
            <br />
            <asp:Panel ID="pnlProducer" runat="server">
                <asp:Label ID="lblProducer" runat="server" Text="Producer Code: "></asp:Label>
                <asp:Label ID="lblProducerCode" runat="server"></asp:Label>
            </asp:Panel>
        </td>
    </tr>
</table>
<div id="divEndorsementSpace" runat="server"><br /></div>
<table style="width: 100%">
    <tr>
        <td>
            <table width="100%" class="quickQuoteSectionTable">
                <tr>
                    <td width="50%" valign="top">
                        <span>Policy #:&nbsp;&nbsp;</span><asp:Label runat="server" ID="lblPolNum"></asp:Label>
                    </td>
                    <td width="50%" valign="top">
                        <span><asp:Label runat="server" ID="lblTranEffDtImageOrChange" Text="Image"></asp:Label> Effective Date:&nbsp;&nbsp;</span><asp:Label runat="server" ID="lblTranEffDt"></asp:Label>
                    </td>
                </tr>
                <tr id="trPFLine1Space" runat="server">
                    <td><br /></td>
                </tr>
                <tr>
                    <td width="50%" valign="top" id="tdTotalAnnualPremiumPrior" runat="server">
                        <span>Total Annual Premium Prior to <asp:Label runat="server" ID="lblPriorAnnualPremImageOrChange" Text="Image"></asp:Label>:&nbsp;&nbsp;</span><asp:Label runat="server" ID="lblPriorAnnualPrem"></asp:Label>
                    </td>
                    <td width="50%" valign="top" id="tdBillMethod" runat="server">
                        <span>Bill Method:&nbsp;&nbsp;</span><asp:Label runat="server" ID="lblBillMethod"></asp:Label>
                    </td>
                    <td width="50%" valign="top" id="tdImageRemarks" runat="server">
                        <span><asp:Label runat="server" ID="lblImgRemarksImageOrChange" Text="Image Remarks"></asp:Label>:&nbsp;&nbsp;</span><asp:Label runat="server" ID="lblImgRemarks"></asp:Label>
                    </td>
                </tr>
                <tr id="trTotalAnnualPremiumAfter" runat="server">
                    <td valign="top" style="width: 50%;">
                        <span>Total Annual Premium After <asp:Label runat="server" ID="lblAnnualPremImageOrChange" Text="Image"></asp:Label>:&nbsp;&nbsp;</span><asp:Label runat="server" ID="lblAnnualPrem"></asp:Label>
                    </td>
                    <td id="tdPremiumChange" runat="server" style="width: 50%;">
                        <span>Premium Change: &nbsp;&nbsp;<asp:Label runat="server" ID="lblPremiumChange"></asp:Label></span>
                    </td>
                </tr>
                <tr id="trPFLine2Space" runat="server">
                    <td><br /></td>
                </tr>
                <tr id="trBillToAndPayPlan" runat="server">
                    <td width="50%" valign="top">Current Bill To: <asp:Label runat="server" ID="lblCurrentBillTo"></asp:Label></td>
                    <td width="50%" valign="top">Pay Plan Information: <asp:Label runat="server" ID="lblPayPlan"></asp:Label></td>
                </tr>
                <tr id="trPFLine3Space" runat="server">
                    <td><br /></td>
                </tr>
                <tr id="trNextPaymentAmountAndDue" runat="server">
                    <td width="50%" valign="top">Next Payment Amount: <asp:Label runat="server" ID="lblNextPaymentAmount"></asp:Label></td>
                    <td width="50%" valign="top">Next Payment Due: <asp:Label runat="server" ID="lblNextPaymentDue"></asp:Label></td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<br />