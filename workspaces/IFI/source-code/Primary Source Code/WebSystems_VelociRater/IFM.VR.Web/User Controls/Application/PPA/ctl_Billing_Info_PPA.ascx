<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Billing_Info_PPA.ascx.vb" Inherits="IFM.VR.Web.ctl_Billing_Info_PPA" %>

<div runat="server" id="divBillingInfo" class="standardSubSection">
    <h3>
        <asp:Label ID="lblHeader" runat="server" Text="Billing Information"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save Page" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
    <div>
        <div id="divRccPayPlanText" style="display:none; font-size:10px; font-weight:bold;color:blue;" runat="server" ClientIDMode="Static">
            You are currently setup for Recurring Credit Card pay plan. Please contact the Client Communities Group at 800-477-1660 to update the pay plan.
        </div>
        <table style="width: 100%">
            <tr>
                <td style="vertical-align: top;">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <label for="<%=Me.ddMethod.ClientID%>">*Method:</label><br />
                                <asp:DropDownList ID="ddMethod" runat="server">
                                </asp:DropDownList>
                            </td>

                            <td>
                                <label for="<%=Me.ddPayPlan.ClientID%>">*Pay Plan:</label><br />
                                <asp:DropDownList ID="ddPayPlan" runat="server">
                                </asp:DropDownList>
                            </td>

                            <td>
                                <label for="<%=Me.ddBillTo.ClientID%>">*Bill To:</label><br />
                                <asp:DropDownList ID="ddBillTo" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>

         <center>
             <div id="divAccountBillAvailText" class="informationalText" style="display:none;" runat="server">
                 Account bill is available on Monthly or EFT Monthly pay plans for commercial accounts with the same named insured and common expiration dates.  If desired, please email your UW.
             </div>
             <div id="divRccReminderText" class="informationalText" style="display:none; font-size:10px; font-weight:bold;" runat="server" ClientIDMode="Static">
                Please be advised for Recurring Credit Card pay plan you will need to submit credit card information once the quote has been submitted. A link will be provided.
             </div>
        </center>

        <div id="divEFTInfo" runat="server">
            <h3>EFT Information</h3>
            <div>
                <div style="padding: 15px; font-weight: bold;">
                    The agency hereby represents that this request for payment via EFT has been duly authorized by the insured. I (the agency) on behalf of the insured authorize Indiana Farmers Mutual Insurance Company to make electronic withdrawals from the bank account according to the account information shown. This authorization remains in effect until I revoke it and Indiana Farmers Mutual has time to act. I understand that these transactions must comply with U.S. laws and that if my bank returns a payment for any reason, Indiana Farmers Mutual may add a return payment fee and/or this may result in the cancellation of insurance. A payment that is dishonored may result in cancellation of insurance.
                                <div style="margin-top: 20px;" id="eftTerms" runat="server">
                                    <asp:CheckBox ID="chkAgreeToEftTerms" runat="server" Text="I Agree to the above EFT Terms" /><br />
                                    <asp:CheckBox ID="chkDeclineEftTerms" runat="server" Text="I Decline to agree to the above EFT Terms" />
                                </div>
                </div>
                <div id="divEFTInfoSub">
                    <table style="width: 100%">
                        <tr>
                            <td style="width: 230px;">
                                <label for="<%=txtEFTRouting.ClientID %>">*Bank Routing Number</label></td>
                            <td>
                                <asp:TextBox ID="txtEFTRouting" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));' MaxLength="9" runat="server"></asp:TextBox><br />
                                <asp:Label ID="lblBankName" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <label for="<%=txtEftAccount.ClientID%>">*Bank Account Number</label></td>
                            <td>
                                <asp:TextBox ID="txtEftAccount" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));' MaxLength="50" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>
                                <label for="<%=ddEftAcountType.ClientID%>">*Bank Account Type</label></td>
                            <td>
                                <asp:DropDownList ID="ddEftAcountType" runat="server">
                                    <asp:ListItem Value=""></asp:ListItem>
                                    <asp:ListItem Value="1">Checking</asp:ListItem>
                                    <asp:ListItem Value="2">Savings</asp:ListItem>
                                </asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td title="Numeric value between 1 and 31">
                                <label for="<%=txtEftDeductionDate.ClientID%>">*Deduction Day</label></td>
                            <td>
                                <asp:TextBox ID="txtEftDeductionDate" onkeyup='$(this).val(FormatAsNumericDigitsOnly($(this).val()));' MaxLength="2" runat="server"></asp:TextBox></td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>

        <div id="divBillingInfoAddress" runat="server">
            <h3>Bill To Information</h3>
            <div>
                <table style="width: 100%">
                    <tr>
                        <td style="vertical-align: top;">
                            <table style="width: 100%">
                                <tr>
                                    <td>
                                        <label for="<%=Me.txtFirstName.ClientID%>">*First Name</label>
                                        <br />
                                        <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="<%=Me.txtMiddleName.ClientID%>">Middle Name</label>
                                        <br />
                                        <asp:TextBox ID="txtMiddleName" runat="server"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="<%=Me.txtLastName.ClientID%>">*Last Name</label>
                                        <br />
                                        <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox></td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <table style="width: 100%">
                                <tr>
                                    <td>
                                        <label for="<%=Me.txtStreetNum.ClientID%>">*Street Number:</label><br />
                                        <asp:TextBox ID="txtStreetNum" onkeyup='$(this).val(FormatAsPositiveNumberNoCommaFormatting($(this).val()));' runat="server"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="<%=Me.txtStreet.ClientID%>">*Street: </label>
                                        <br />
                                        <asp:TextBox ID="txtStreet" runat="server"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="<%=Me.txtAptSuiteNum.ClientID%>">Apt/Suite Number:</label><br />
                                        <asp:TextBox ID="txtAptSuiteNum" runat="server"></asp:TextBox></td>
                                </tr>

                                <tr>
                                    <td>
                                        <label for="<%=Me.txtPOBOX.ClientID%>">P.O. Box:</label><br />
                                        <asp:TextBox ID="txtPOBOX" runat="server"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="<%=Me.txtCity.ClientID%>">*City:</label><br />
                                        <asp:TextBox ID="txtCity" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="<%=Me.ddState.ClientID%>">*State:</label><br />
                                        <asp:DropDownList ID="ddState" runat="server">
                                        </asp:DropDownList>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="<%=Me.txtZipCode.ClientID%>">*ZIP:</label><br />
                                        <asp:TextBox ID="txtZipCode" onblur="$(this).val(formatPostalcode($(this).val()));" runat="server"></asp:TextBox></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</div>
<asp:HiddenField ID="hiddenDivBillingActive" runat="server" />