<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CGL_Coverages.ascx.vb" Inherits="IFM.VR.Web.ctl_CGL_Coverages" %>

<div runat="server" id="divMain">
    <h3>Policy Level Coverages
        <span style="float: right;">        
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>

    </h3>
    <div>
        <asp:CheckBox ID="chkAdditional" Text="Additional Insured" runat="server" />
        <div id="divAdditional" style="padding-left:20px;">
            <label for="<%=txtAdditionalInsuredPremiumCharge.ClientID%>">*Premium Charge for Additional Insureds</label>
            <asp:TextBox ID="txtAdditionalInsuredPremiumCharge" runat="server"></asp:TextBox>
            <br />
            <span class="informationalText">Contact Underwriter for Additional Interest Charges</span>
        </div>
        <br />
        <asp:CheckBox ID="chkEmployee" Text="Employee Benefits Liability" runat="server" />
        <div id="divEmployee" style="padding-left:20px;">
            <table>
                <tr>
                    <td><label for="<%=txtEmployeeOccurrenceLimit.ClientID%>">Occurrence Limit</label></td>
                    <td>
                        <asp:TextBox ID="txtEmployeeOccurrenceLimit" Enabled="false" Text="static value" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><label for="<%=txtEmployeeNumberOfEmployees.ClientID%>">*Number of Employees</label></td>
                    <td>
                        <asp:TextBox ID="txtEmployeeNumberOfEmployees" runat="server"></asp:TextBox></td>
                </tr>
            </table>
            <span class="informationalText">Please contact your underwriter if a lower Employee Benefits Liability limit is requested.</span>
        </div>
        <br />            
            <asp:CheckBox ID="chkEPLI" Text="Employment Practices Liability - Claims-Made Basis" runat="server" />
        
        <br />
            <asp:CheckBox ID="chkHired" Text="Hired/Non-Owned Autos" runat="server" />
        <br />
        <asp:CheckBox ID="chkLiquor" Text="Liquor Liability" runat="server" />
        <div id="divLiquor" style="padding-left:20px;">
            <table>
                <tr>
                    <td><label for="<%=txtLiquorOccurrenceLimit.ClientID%>">Occurrence Limit</label></td>
                    <td>
                        <asp:TextBox ID="txtLiquorOccurrenceLimit" Text="static value" Enabled="false" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><label for="<%=ddLiquorClassification.ClientID%>">Classification</label></td>
                    <td>
                        <asp:DropDownList ID="ddLiquorClassification" runat="server">
                    <asp:ListItem Value="50911">Manufacturer, Wholesalers &amp; Distributors</asp:ListItem>
                    <asp:ListItem Value="58161">Restaurants or Hotels</asp:ListItem>
                    <asp:ListItem Value="59211">Package Stores</asp:ListItem>
                    <asp:ListItem Value="70412">Clubs</asp:ListItem>
                </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td><label for="<%=txtLiquorSales.ClientID%>">Liquor Sales</label></td>
                    <td>
                        <asp:TextBox ID="txtLiquorSales" runat="server"></asp:TextBox></td>
                </tr>
            </table>
            <span class="informationalText">Please contact your underwriter if a lower Liquor Liability limit is requested.</span>
        </div>
        <br />
        <asp:CheckBox ID="chkProfessional" Text="Professional Liability" runat="server" />
        <div id="divProfessional" style="padding-left:20px;">
            <table>
                <tr>
                    <td><label for="<%=txtprofessionalBurials.ClientID%>">Number of Burials</label></td>
                    <td>
                        <asp:TextBox ID="txtprofessionalBurials" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><label for="<%= txtProfessionalBodies.ClientID%>">Number of Bodies</label></td>
                    <td>
                        <asp:TextBox ID="txtProfessionalBodies" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td><label for="<%= txtProfessionalClergy.ClientID%>">Number of Clergy</label></td>
                    <td>
                        <asp:TextBox ID="txtProfessionalClergy" runat="server"></asp:TextBox></td>
                </tr>
            </table>
        </div>
        <br />
        <span class="informationalText">Other Optional Coverages are available. Please contact your Commercial Underwriter for assistance and approval. </span>

    </div>
</div>
<asp:HiddenField ID="hddnAccord" runat="server" />
