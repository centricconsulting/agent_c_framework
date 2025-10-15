<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CGL_GeneralInformation.ascx.vb" Inherits="IFM.VR.Web.ctl_CGL_GeneralInformation" %>

<div runat="server" id="divMain">
    <h3>General Information
         <span style="float: right;">        
        <asp:LinkButton ID="lnkBtnSave" CssClass="RemovePanelLink" ToolTip="Save" runat="server">Save</asp:LinkButton>
    </span>
    </h3>
    <div>
        <table style="width:100%">
            <tr>
                <td>
                    <label for="<%=ddProgramType.ClientID%>">*Program Type</label>
                    <br />
                    <asp:DropDownList ID="ddProgramType" runat="server" >
                        <asp:ListItem Value="54">CGL - Commercial general Liability - Standard</asp:ListItem>
                        <asp:ListItem Value="55">CGL - Commercial General Liability - Preferred</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <label for="<%=txtRented.ClientID%>">Damage to Premises Rented to You</label>
                    <br />
                    <asp:TextBox ID="txtRented" Enabled="false" runat="server"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <label for="<%=ddOccuranceLibLimit.ClientID%>">*Occurrence Liability Limit</label>
                    <br />
                    <asp:DropDownList ID="ddOccuranceLibLimit" runat="server" Width="137px">
                                <asp:ListItem Value="8">25,000</asp:ListItem>
                                <asp:ListItem Value="9">50,000</asp:ListItem>
                                <asp:ListItem Value="10">100,000</asp:ListItem>
                                <asp:ListItem Value="32">200,000</asp:ListItem>
                                <asp:ListItem Value="33">300,000</asp:ListItem>
                                <asp:ListItem Value="34">500,000</asp:ListItem>
                                <asp:ListItem Value="56">1,000,000</asp:ListItem>
                            </asp:DropDownList>

                </td>
                <td>
                    <label for="<%=ddOperationsAgg.ClientID%>">*Product/Completed Operations Aggregate</label>
                    <br />
                    <asp:DropDownList ID="ddOperationsAgg" runat="server" Width="137px">
                                <asp:ListItem Value="327">Excluded</asp:ListItem>
                                <asp:ListItem Value="9">50,000</asp:ListItem>
                                <asp:ListItem Value="10">100,000</asp:ListItem>
                                <asp:ListItem Value="32">200,000</asp:ListItem>
                                <asp:ListItem Value="33">300,000</asp:ListItem>
                                <asp:ListItem Value="34">500,000</asp:ListItem>
                                <asp:ListItem Value="178">600,000</asp:ListItem>
                                <asp:ListItem Value="56">1,000,000</asp:ListItem>
                                <asp:ListItem Value="185">1,500,000</asp:ListItem>
                                <asp:ListItem Value="65">2,000,000</asp:ListItem>
                            </asp:DropDownList>
                </td>
            </tr>

            <tr>
                <td>
                    <label for="<%=ddGeneralAgg.ClientID%>">*General Aggregate</label>
                    <br />
                    <asp:DropDownList ID="ddGeneralAgg" runat="server" Width="137px">
                                <asp:ListItem Value="9">50,000</asp:ListItem>
                                <asp:ListItem Value="10">100,000</asp:ListItem>
                                <asp:ListItem Value="32">200,000</asp:ListItem>
                                <asp:ListItem Value="33">300,000</asp:ListItem>
                                <asp:ListItem Value="34">500,000</asp:ListItem>
                                <asp:ListItem Value="178">600,000</asp:ListItem>
                                <asp:ListItem Value="56">1,000,000</asp:ListItem>
                                <asp:ListItem Value="185">1,500,000</asp:ListItem>
                                <asp:ListItem Value="65">2,000,000</asp:ListItem>                                
                            </asp:DropDownList>
                </td>
                <td>
                    <label for="<%=txtMedicalExpense.ClientID%>">Medical Expenses</label>
                    <br />
                    <asp:TextBox ID="txtMedicalExpense" runat="server" Enabled="False" Width="130px"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td></td>
                <td>
                    <label for="<%=ddPersonalInjury.ClientID%>">*Personal and Advertising Injury</label>
                    <br />
                    <asp:DropDownList ID="ddPersonalInjury" runat="server" Width="137px">
                                <asp:ListItem Value="327">Excluded</asp:ListItem>
                                <asp:ListItem Value="8">25,000</asp:ListItem>
                                <asp:ListItem Value="9">50,000</asp:ListItem>
                                <asp:ListItem Value="10">100,000</asp:ListItem>
                                <asp:ListItem Value="32">200,000</asp:ListItem>
                                <asp:ListItem Value="33">300,000</asp:ListItem>
                                <asp:ListItem Value="34">500,000</asp:ListItem>
                                <asp:ListItem Value="56">1,000,000</asp:ListItem>
                            </asp:DropDownList>
                </td>
            </tr>
        </table>

        <asp:CheckBox ID="chkGenLibEnhancement" runat="server" /><asp:HyperLink Target="_blank" NavigateUrl="https://www.ifmig.net/agentsonly/docs/VRHelpFeatureDocs/LiabilityEnhancement.PDF" ID="lnkEnhacementHelp" runat="server">General Liability Enhancement Endorsement</asp:HyperLink>
        <div id="divSubrogation" style="padding-left:15px;">
            <label for="<%=ddlAddlBlanketOfSubroOptions.ClientID%>">Add Blanket Waiver of Subrogation</label>
             <asp:DropdownList ID="ddlAddlBlanketOfSubroOptions" runat="server">
                <asp:ListItem Text="No" Value=""></asp:ListItem>
                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
                <asp:ListItem Text="Yes with Completed Ops" Value="2"></asp:ListItem>                 
            </asp:DropdownList>
            <div id="divSubrogationInfo" class="informationalText">
                If General Liability Enhancement Endorsement is selected, and Blanket Waiver of Subrogation is yes the
                Contractors Supplemental Application must be completed and e-mailed to your underwriter to bind coverage.
                Please click here for the <a target="_blank" href="https://www.indianafarmers.com/agentsonly/docs/VRHelpFeatureDocs/IFM contractors supplemental application.docx">Contractors Application</a>. 
            </div>
        </div>
        <div>
        <asp:CheckBox ID="chkDeductible" Text="General Liability Deductible" runat="server" />
        <div id="divDeductible" style="padding-left:15px;">
            <table>
                <tr>
                    <td><label for="<%=ddType.ClientID%>">*Type</label></td>
                    <td>
                        <asp:DropDownList ID="ddType" runat="server" Width="230px">
                <asp:ListItem Value="5">Bodily Injury </asp:ListItem>
                <asp:ListItem Value="6">Property Damage</asp:ListItem>
                <asp:ListItem Value="7">Bodily Injury &amp; Property Damage</asp:ListItem>
            </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td><label for="<%=ddAmount.ClientID%>">*Amount</label></td>
                    <td>
                        <asp:DropDownList ID="ddAmount" runat="server" Width="230px">
                <asp:ListItem Value="4">250</asp:ListItem>
                <asp:ListItem Value="8">500</asp:ListItem>
                <asp:ListItem Value="27">750</asp:ListItem>
                <asp:ListItem Value="9">1,000</asp:ListItem>
                <asp:ListItem Value="28">2,000</asp:ListItem>
                <asp:ListItem Value="29">3,000</asp:ListItem>
                <asp:ListItem Value="30">4,000</asp:ListItem>
                <asp:ListItem Value="16">5,000</asp:ListItem>
                <asp:ListItem Value="17">10,000</asp:ListItem>
                <asp:ListItem Value="31">15,000</asp:ListItem>
                <asp:ListItem Value="18">20,000</asp:ListItem>
                <asp:ListItem Value="19">25,000</asp:ListItem>
                <asp:ListItem Value="20">50,000</asp:ListItem>
                <asp:ListItem Value="21">75,000</asp:ListItem>
            </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td><label for="<%=ddBasis.ClientID%>">*Basis</label></td>
                    <td>
                        <asp:DropDownList ID="ddBasis" runat="server" Width="230px">
                <asp:ListItem Value="1">Per Occurrence</asp:ListItem>
                <asp:ListItem Value="2">Per Claim</asp:ListItem>
            </asp:DropDownList>
                    </td>
                </tr>
            </table>
            
            
            
        </div>
            </div>

    </div>
</div>
<asp:HiddenField ID="hddAccord" runat="server" />
