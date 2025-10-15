<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_App_AdditionalInterest.ascx.vb" Inherits="IFM.VR.Web.ctl_App_AdditionalInterest" %>


    <h3>
        <asp:Label ID="lblExpanderText" runat="server" Text="Label"></asp:Label>
        <span style="float: right;">
            <asp:LinkButton ID="lnkRemove" CssClass="RemovePanelLink" runat="server">Remove</asp:LinkButton>
            <asp:LinkButton ID="lnkSave" CssClass="RemovePanelLink" runat="server">Save</asp:LinkButton>
        </span>
    </h3>
<div>
    <table style="width: 100%">
        <tr>
            <td style="vertical-align: top;">
                <div style="vertical-align: top;">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <label for="<%=Me.txtCommName.ClientID%>">*Business Name:</label><br />
                                <asp:TextBox ID="txtCommName" MaxLength="55" runat="server"></asp:TextBox>
                                <br />
                                <span style="margin-left: 40px; font-weight: bold;">or</span>
                                <br />
                                <label for="<%=Me.txtFirstName.ClientID%>">*First Name:</label><br />
                                <asp:TextBox ID="txtFirstName" MaxLength="55" runat="server"></asp:TextBox>
                                <span style="display: none;">
                                    <label for="<%=Me.txtMiddleName.ClientID%>">Middle Name:</label><br />
                                    <asp:TextBox ID="txtMiddleName" MaxLength="55" runat="server"></asp:TextBox>
                                </span>
                                <br />
                                <label for="<%=Me.txtLastName.ClientID%>">*Last Name:</label><br />
                                <asp:TextBox ID="txtLastName" MaxLength="55" runat="server"></asp:TextBox>
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
                                <asp:TextBox ID="txtZipCode" runat="server" TabIndex="16" MaxLength="10"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <%--<label for="<%=Me.ddCityName.ClientID%>">*City:</label><br />--%>
                                <%--<asp:DropDownList ID="ddCityName" CausesValidation="false" runat="server"></asp:DropDownList>--%>
                                <label for="<%=Me.txtCity.ClientID%>">*City:</label><br />
                                <asp:TextBox ID="txtCity" runat="server" TabIndex="17" MaxLength="25"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <label for="<%=Me.ddStateAbbrev.ClientID%>">*State:</label><br />
                                <asp:DropDownList ID="ddStateAbbrev" runat="server" TabIndex="18">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="txtAiId" ToolTip="This should be hidden. AI ID" runat="server"></asp:TextBox>
                                <asp:TextBox ID="txtIsEditable" Style="display: none;" ToolTip="This should be hidden. Was create this session." runat="server"></asp:TextBox>
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
