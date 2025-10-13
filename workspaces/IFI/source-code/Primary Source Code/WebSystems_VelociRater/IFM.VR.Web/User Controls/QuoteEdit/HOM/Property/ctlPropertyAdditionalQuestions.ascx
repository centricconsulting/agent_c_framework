<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctlPropertyAdditionalQuestions.ascx.vb" Inherits="IFM.VR.Web.ctlPropertyAdditionalQuestions" %>
<style>
    .NumberOfUnits {
        margin-top: 10px;
        margin-left: 20px;
    }
</style>

<div id="AdditionalQuestionsDiv" runat="server" class="standardSubSection">
    <h3>Additional Questions
                 <span style="float: right;">
                     <asp:LinkButton ID="lnkClearAdditionalQuestions" ToolTip="Clear Additional Questions" runat="server" CssClass="RemovePanelLink">Clear</asp:LinkButton>
                     <asp:LinkButton ID="lnkSaveAdditionalQuestions" ToolTip="Save Page" runat="server" CssClass="RemovePanelLink">Save</asp:LinkButton>
                 </span>
    </h3>
    <div id="AdditionalQuestionsContentDiv">
        <table id="tblAdditionalQuestions" style="width: 100%;">
            <tr runat="server" id="trQuestion1">
                <td>
                    <asp:CheckBox TabIndex="24" ID="chkHasAutoPolicy" CssClass="Property_AdditionalInfo_Questions" runat="server" Text="The policyholder already has or is applying for a Personal Passenger auto policy with Indiana Farmers." />
                </td>
            </tr>
            <tr runat="server" id="trQuestion10">
                <td>
                    <asp:CheckBox ID="chkFirstWrittenDate" onchange="$(this).next('div').toggle();" Text="The policyholder has an active and continuous Homeowner’s policy with Indiana Farmers." runat="server" />
                    <div id="divFirstWrittenDate" style="margin-top: 10px; margin-left: 20px; display: none;">
                        First Written Date<asp:TextBox ID="txtFirstWrittenDate" runat="server"></asp:TextBox>
                    </div>
                </td>
            </tr>
            <tr runat="server" id="trQuestion2">
                <td>
                    <asp:CheckBox TabIndex="25" ID="chkAnyChildren" CssClass="Property_AdditionalInfo_Questions" runat="server" Text="There are children (of any age) living in this residence premises." />
                </td>
            </tr>
            <tr runat="server" id="trQuestion3">
                <td>
                    <asp:CheckBox TabIndex="26" ID="chkSmokeAlarms" CssClass="Property_AdditionalInfo_Questions" runat="server" Text="There is an active smoke alarm system installed at this residence." />
                </td>
            </tr>
            <tr runat="server" id="trQuestion4">
                <td>
                    <label for="<%=Me.ddBurglarAlarmType.ClientID%>">
                        What type of active burglar alarm system is installed at this residence?
                    </label>
                    <br />
                    <asp:DropDownList ID="ddBurglarAlarmType" Width="125" runat="server" TabIndex="27">
                        <asp:ListItem Value="0">NONE</asp:ListItem>
                        <asp:ListItem Value="3">LOCAL SYSTEM</asp:ListItem>
                        <asp:ListItem Value="1">CENTRAL SYSTEM</asp:ListItem>
                        <asp:ListItem Value="2">POLICE DEPARTMENT THEFT ALARM</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr runat="server" id="trQuestion5">
                <td>
                    <label for="<%=Me.ddFireAlarmType.ClientID%>">
                        What type of active fire alarm system is installed at this residence?
                    </label>
                    <br />
                    <asp:DropDownList ID="ddFireAlarmType" Width="125" runat="server" TabIndex="28">
                        <asp:ListItem Value="1">NONE</asp:ListItem>
                        <asp:ListItem Value="2">LOCAL SYSTEM</asp:ListItem>
                        <asp:ListItem Value="3">CENTRAL SYSTEM</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr runat="server" id="trQuestion6">
                <td>
                    <label for="<%=Me.ddSprinklerType.ClientID%>">
                        What type of active sprinkler system is installed at this residence?
                    </label>
                    <br />
                    <asp:DropDownList ID="ddSprinklerType" Width="125" runat="server" TabIndex="29">
                        <asp:ListItem Value="0">NONE</asp:ListItem>
                        <asp:ListItem Value="1">ALL AREAS EXCEPT ATTICS, BATHROOMS, CLOSETS AND ATTACHED STRUCTURES</asp:ListItem>
                        <asp:ListItem Value="2">ALL AREAS INCLUDING ATTICS, BATHROOMS, CLOSETS AND ATTACHED STRUCTURES</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr runat="server" id="trQuestion6b">
                <td>
                    <asp:CheckBox TabIndex="30" ID="chkSprinkler" CssClass="Property_AdditionalInfo_Questions" runat="server" Text="There is an active interior sprinkler system at this residence." />
                </td>
            </tr>
            <tr runat="server" id="trQuestion7">
                <td>
                    <asp:CheckBox TabIndex="31" ID="chkTrampoline" CssClass="Property_AdditionalInfo_Questions" runat="server" Text="There is a trampoline on the property." />
                    <div id="divNumOfTrampolines" style="display: none;" class="NumberOfUnits" runat="server">
                        Number of Units<asp:TextBox ID="txtNumOfTrampolines" runat="server" MaxLength="3" style="margin-left: 10px; width: 50px;" TabIndex="32"></asp:TextBox>
                    </div>
                </td>
            </tr>
            <tr runat="server" id="trQuestion8">
                <td>
                    <asp:CheckBox TabIndex="33" ID="chkSwimmingPool" CssClass="Property_AdditionalInfo_Questions" runat="server" Text="There is a swimming pool and/or hot tub on the property." />
                    <div id="divNumOfSwimmingPools" style="display: none;"  class="NumberOfUnits" runat="server">
                        Number of Units<asp:TextBox ID="txtNumOfSwimmingPools" runat="server" MaxLength="3" style="margin-left: 10px; width: 50px;" TabIndex="34"></asp:TextBox>
                    </div>
                </td>
            </tr>
            <tr runat="server" id="trQuestion9">
                <td>
                    <asp:CheckBox TabIndex="35" ID="chkWoodStove" CssClass="Property_AdditionalInfo_Questions" runat="server" Text="There is a wood burning stove or solid fuel appliance on the property." />
                    <div id="divNumOfWoodburningStoves" style="display: none;" class="NumberOfUnits" runat="server">
                        Number of Units<asp:TextBox ID="txtNumOfWoodburningStoves" runat="server" MaxLength="3" style="margin-left: 10px; width: 50px;" TabIndex="36"></asp:TextBox>
                        <p ID="txtSolidFuelQuestionnaire" runat="server" class="informationalText"> Coverage is subject to receipt of photos and a completed <a target="_blank" style="color:blue" href="<%=ConfigurationManager.AppSettings("VRHelpDoc_SolidFuelQuestionnaire")%>">Solid Fuel Questionnaire</a> for each stove.</p>
                    </div>
                    <div ID="divWoodburningInfoSection" runat="server">
                        <br />
                        <asp:Label ID="lblWoodBurningInfoMsg" runat="server" CssClass="informationalText" Text="A contact email address is required with a wood burning stove or fuel appliance to complete an inspection."></asp:Label>
                        <br />
                        <label for="<%=Me.txtWoodBurningEmail.ClientID%>">
                            Email:
                        </label>
                        <br />
                        <asp:TextBox ID="txtWoodBurningEmail" runat="server" Width="125" TabIndex="36"></asp:TextBox>
                    </div>
                </td>
            </tr>
            <tr runat="server" id="trQuestion11" visible="false">
                <td>
                    <asp:CheckBox TabIndex="37" ID="chkMortgageContractSeller" CssClass="Property_AdditionalInfo_Questions" runat="server" Text="Property has mortgage or contract seller." />
                </td>
            </tr>
        </table>
    </div>
</div>
<asp:HiddenField ID="HiddenField5" runat="server" />
