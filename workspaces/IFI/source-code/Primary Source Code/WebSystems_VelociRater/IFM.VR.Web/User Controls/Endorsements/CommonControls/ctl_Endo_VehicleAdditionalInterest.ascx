<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_Endo_VehicleAdditionalInterest.ascx.vb" Inherits="IFM.VR.Web.ctl_Endo_VehicleAdditionalInterest" %>

<h3>
    <asp:Label ID="lblExpanderText" runat="server" Text="Label"></asp:Label>
    <span style="float: right;">
        <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" runat="server">Remove</asp:LinkButton>
        <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" runat="server">Save</asp:LinkButton>
    </span>
</h3>
<div runat="server" id="divAiEntry">
    <table style="width: 100%">
        <tr id="trSaveMsgRow" runat="server" visible="false">
            <td colspan="2">
                <span id="spnMsg" runat="server" style="color:red;font-weight:700;">Additional interests must be saved before they can be assigned.</span>
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top;">
                <div style="vertical-align: top;">
                    <table style="width: 100%">
                        <tr id="trAIType" runat="server">
                            <td>
                                <label for="<%=Me.ddAiType.ClientID%>">*Type</label><br />
                                <asp:DropDownList ID="ddAiType" Width="150" runat="server"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <label for="<%=Me.txtCommName.ClientID%>">*Business Name:</label><br />
                                <asp:TextBox ID="txtCommName" MaxLength="55" runat="server" CssClass="clsBusinessName" ></asp:TextBox>
                                <br />
                                <span style="margin-left: 40px; font-weight: bold;">or</span>
                                <br />
                                <label for="<%=Me.txtFirstName.ClientID%>">*First Name:</label><br />
                                <asp:TextBox ID="txtFirstName" MaxLength="55" runat="server" CssClass="clsFirstName" ></asp:TextBox>
                                <span style="display: none;">
                                    <label for="<%=Me.txtMiddleName.ClientID%>">Middle Name:</label><br />
                                    <asp:TextBox ID="txtMiddleName" MaxLength="55" runat="server"></asp:TextBox>
                                </span>
                                <br />
                                <label for="<%=Me.txtLastName.ClientID%>">*Last Name:</label><br />
                                <asp:TextBox ID="txtLastName" MaxLength="55" runat="server" CssClass="clsLastName" ></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trLoanNumber" runat="server">
                            <td>
                                <label for="<%=Me.txtLoanNumber.ClientID%>">Loan Number:</label>
                                <br />
                                <asp:TextBox ID="txtLoanNumber" runat="server"></asp:TextBox>
                            </td>
                        </tr>

                        <tr id="trPhoneNumber" runat="server">
                            <td>
                                <label for="<%=Me.txtPhoneNumber.ClientID%>">Business Phone Number:</label>
                                <br />
                                <asp:TextBox ID="txtPhoneNumber" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr id="trPhoneExtension" runat="server">
                            <td>
                                <label for="<%=Me.txtPhoneExtension.ClientID%>">Phone Extension:</label>
                                <br />
                                <asp:TextBox ID="txtPhoneExtension" MaxLength="5" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                                                
                        <!-- Farm Quote checkbox for Additional Insured -->
                        <tr id="trAppliesTo" runat="server">
                            <td>
                                <asp:CheckBox  ID="chkAppliesTo" Text="Applies to" runat="server" />                                
                            </td>
                        </tr>
                                                                        
                        <tr runat="server" id="trDescription">
                            <td>
                                <label for="<%=Me.txtDescription.ClientID %>">Description</label><br />
                                <asp:TextBox ID="txtDescription" Height="50px" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>

                                <asp:TextBox ID="txtAiId" ToolTip="This should be hidden. AI ID" runat="server"></asp:TextBox>
                                <asp:TextBox ID="txtIsEditable" Style="display: none;" ToolTip="This should be hidden. Was create this session." runat="server"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
            <td style="vertical-align: top;">
                <div style="width: 225px;">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <label for="<%=Me.txtStreetNum.ClientID%>">Street Number:</label><br />
                                <asp:TextBox ID="txtStreetNum" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>
                                <label for="<%=Me.txtStreet.ClientID%>">Street:</label><br />
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
                                <label for="<%=Me.txtZipCode.ClientID%>">*ZIP:</label><br />
                                <asp:TextBox ID="txtZipCode" runat="server" TabIndex="0" MaxLength="10"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <label for="<%=Me.ddCityName.ClientID%>">*City:</label><br />
                                <asp:DropDownList ID="ddCityName" CausesValidation="false" runat="server"></asp:DropDownList>
                                <asp:TextBox ID="txtCity" runat="server" TabIndex="0" MaxLength="25"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <label for="<%=Me.ddStateAbbrev.ClientID%>">*State:</label><br />
                                <asp:DropDownList ID="ddStateAbbrev" runat="server" TabIndex="0">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="trATIMA" runat="server">
                            <td>
                                <asp:CheckBox ID="chkATIMA" Text="ATIMA" runat="server" />
                            </td>
                        </tr>
                        <tr id="trISAOA" runat="server">
                            <td>
                                <asp:CheckBox ID="chkISAOA" Text="ISAOA" runat="server" />
                            </td>
                        </tr>
                        <tr id="trBillTo" runat="server">
                            <td>
                                <asp:CheckBox ID="chkBillToMe" Text="Bill to this Mortgagee" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <div style="display: none; margin-left: 10px; width: 190px; border: 1.5px solid lightblue; padding: 10px;" class="roundedContainer" runat="server" id="divAddressMessage">
                        You must enter the Street # and Street Name or the P.O. Box field.
                    </div>
                </div>
            </td>
        </tr>
    </table>
</div>