<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlRiskGradeSearch.ascx.vb" Inherits="IFM.VR.Web.ctlRiskGradeSearch" %>


<div runat="server" id="divMain">
    
     <div id="divRiskPrefGuide" runat="server" style="width:90%;text-align:center;margin:15px;">
                    For help with how to use our Risk Preferences Guide please click here for the <a target="_blank" href="<%=System.Configuration.ConfigurationManager.AppSettings("CPR_Help_RiskGradeGuide")%>">Risk Preference Guide</a>
     </div>
     <table style="margin-left:auto;margin-right:auto;">
        <tr>
            <td id="tdStateLabelRow" runat="server" style="display:none;width:30%;">
                <asp:Label ID="lblState" runat="server" Text="State" Width="100%"></asp:Label>
            </td>
            <td style="width:30%;">
                <asp:Label ID="BOPRGSLabel2" runat="server" Text="Filter By" Width="100%"></asp:Label>
            </td>
            <td style="width:30%;">
                <asp:Label ID="BOPRGSLabel3" runat="server" Text="Filter Value" Width="100%" ></asp:Label><br />
            </td>
            <td style="width:10%;">&nbsp;</td>
        </tr>
        <tr>
            <td id="tdStateddRow" runat="server" style="display:none;">
                <asp:DropDownList ID="ddState" runat="server" width="100%">
                    <asp:ListItem Text="" Value="0"></asp:ListItem>
                    <asp:ListItem Text="IN Indiana" Value="16"></asp:ListItem>
                    <asp:ListItem Text="IL Illinois" Value="15"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:DropDownList ID="ddlRiskGradeFilterBy" runat="server" 
                    TabIndex="47" Width="100%">
                    <asp:ListItem Value="3">Description Contains</asp:ListItem>
                    <asp:ListItem Value="1">GL Class Code</asp:ListItem>
                    <asp:ListItem Value="2">Description Starts With</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox MaxLength="20" ID="txtRiskGradeFilterValue" 
                    runat="server" Width="100%" TabIndex="48"></asp:TextBox>
            </td>
            <td> 
                &nbsp;
                <asp:Button ID="btnRisksearch" runat="server" Text="Find" />     
            </td>
        </tr>
    </table>

    <div id="divResults" runat="server"></div>
    
    <br />
    <div align="center" id="divRiskGradeInfo" runat="server">
        <asp:Label ID="lblGradeTitle" runat="server" Text="GRADE Definition Guide" Width="100%" Font-Bold="true" Font-Size="Large"></asp:Label>
        <table border="1" style="width:80%;background-color:white;">
            <tr style="height:30px;">
                <td style="width:10%;text-align:center;font-weight:700;">
                    Code
                </td>
                <td style="width:45%;text-align:center;font-weight:700;">
                    Definition
                </td>
                <td style="width:45%;text-align:center;font-weight:700;">
                    Action
                </td>
            </tr>
            <tr style="height:50px;">
                <td style="width:10%;text-align:center">
                    1
                </td>
                <td style="width:45%;text-align:center;">
                    Generally Acceptable
                </td>
                <td style="width:45%;text-align:center;">
                    Authority to quote and bind
                </td>
            </tr>
            <tr style="height:50px;">
                <td style="width:10%;text-align:center">
                    2
                </td>
                <td style="width:45%;text-align:center;">
                    Conditionally Acceptable
                </td>
                <td style="width:45%;text-align:center;">
                    Referral to your Commercial Underwriter or Field Marketing Manager before quoting or binding
                </td>
            </tr>
            <tr style="height:50px;">
                <td style="width:10%;text-align:center">
                    3
                </td>
                <td style="width:45%;text-align:center;">
                    Generally Unacceptable
                </td>
                <td style="width:45%;text-align:center;">
                    No authority to quote or bind.  Contact your Commercial Underwriter for exceptions
                </td>
            </tr>
            <tr style="height:50px;">
                <td style="width:10%;text-align:center">
                    P
                </td>
                <td style="width:45%;text-align:center;">
                    Prohibited
                </td>
                <td style="width:45%;text-align:center;">
                    No authority to quote or bind.  Prohibited class or reinsurance exclusion
                </td>
            </tr>
            <tr style="height:50px;">
                <td style="width:10%;text-align:center">
                    *
                </td>
                <td style="width:45%;text-align:center;">
                    May be eligible for our Businessowners Program 
                </td>
                <td style="width:45%;text-align:center;">
                    Consult Businessowners eligibility
                </td>
            </tr>
        </table>
    </div>
    <div align="center">
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" />
        &nbsp;
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
    </div>
    <asp:HiddenField ID="hiddenRiskCodeId" runat="server" />
    <asp:HiddenField ID="HiddenRiskCodeId2" runat="server" />
    <asp:HiddenField ID="hiddenRiskCodeLookupId" runat="server" />
    <asp:HiddenField ID="HiddenClassCode" runat="server" />
    <asp:HiddenField ID="HiddenDescription" runat="server" />
    <%--<asp:HiddenField ID="HiddenBOPClass_Id" runat="server" />--%>
    <asp:HiddenField ID="HiddenDIAClass_Id" runat="server" />
    <%--<asp:HiddenField ID="hdnStateId" runat="server" />--%>
</div>