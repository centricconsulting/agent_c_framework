<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_CPP_CrimeClassCodeLookup.ascx.vb" Inherits="IFM.VR.Web.ctl_CPP_CrimeClassCodeLookup" %>

<div runat="server" id="divMain" style="border: none;">
        <table style="margin-left: auto; margin-right: auto;">
            <tr>
                <td>
                    <asp:Label ID="CPRCCLabel2" runat="server" Text="Filter By" Width="200px"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="CPRCCLabel3" runat="server" Text="Filter Value" Width="200px"></asp:Label><br />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="ddlFilterBy" runat="server"
                        TabIndex="47" Width="200px">
                        <%--The codes were originally 1,3,2; But these were inaccurate for the StoredProc. 1,2,0 are correct for Crime/CGL--%>
                        <asp:ListItem Value="1">Description Contains</asp:ListItem>
                        <asp:ListItem Value="2">Class Code</asp:ListItem>
                        <asp:ListItem Value="0">Description Starts With</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:TextBox MaxLength="20" ID="txtFilterValue"
                        runat="server" Width="200px" TabIndex="48"></asp:TextBox>
                </td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Find" />
                </td>
            </tr>
        </table>

        <div id="divResults" runat="server"></div>

        <br />
        <div id="divCCInfo" runat="server">
            <table style="width: 100%;">
                <tr>
                    <td class="CPRBCC_SpacerColumn">&nbsp;</td>
                    <td colspan="2">Class Code
                    <br />
                        <asp:TextBox ID="txtClassCode" runat="server" Enabled="false"></asp:TextBox>
                        <asp:TextBox ID="txtDIA_Id" runat="server" Style="display: none;" Width="1px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="CPRBCC_SpacerColumn">&nbsp;</td>
                    <td colspan="2">Description
                    <br />
                        <asp:TextBox ID="txtDescription" runat="server" Width="95%"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="CPRBCC_SpacerColumn">&nbsp;</td>
                    <td colspan="2">*PMA
                    <br />
                        <asp:DropDownList ID="ddPMA" runat="server" Enabled="false"></asp:DropDownList>
                    </td>
                </tr>
                <tr id="trPMAValidationRow" runat="server" style="display:none;">
                    <td class="CPRBCC_SpacerColumn">&nbsp;</td>
                    <td colspan="2" style="text-align:left;color:red;">PMA is Required</td>
                </tr>
                <tr>
                    <td class="CPRBCC_SpacerColumn">&nbsp;</td>
                    <td style="width: 25%;">Rate Group
                    <br />
                        <asp:TextBox ID="txtGroupRate" runat="server" Enabled="false"></asp:TextBox>
                    </td>
                    <td style="width: 75%;">Class Limit
                    <br />
                        <asp:TextBox ID="txtClassLimit" runat="server" Enabled="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="CPRBCC_SpacerColumn">&nbsp;</td>
                    <td colspan="2">FootNote
                    <br />
                        <div id="divFootnote" runat="server" style="height: 250px; width: 100%; background-color: white; border: 1px solid black;"></div>
                    </td>
                </tr>
                <tr id="trFootnoteInfoRow" runat="server" style="display:none;">
                <td class="CPRBCC_SpacerColumn">&nbsp;</td>
                <td colspan="2" class="informationalText">
                    * Use the blue links in the footnote to select the appropriate class code or search for a more specific class code.
                </td>
            </tr>
            </table>
        </div>
        <br />

        <div align="center">
            <asp:Button ID="btnApply" runat="server" Text="Apply Class Code" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="return confirm('Cancel?');" />
            <asp:Button ID="btnClose" runat="server" Text="Close" Style="display: none;" />
        </div>


    <asp:HiddenField ID="hdnClassCode" runat="server" />
    <asp:HiddenField ID="hdnDescription" runat="server" />
    <asp:HiddenField ID="hdnPMAID" runat="server" />
    <asp:HiddenField ID="hdnDIAClass_Id" runat="server" />
    <asp:HiddenField ID="hdnGroupRate" runat="server" />
    <asp:HiddenField ID="hdnClassLimit" runat="server" />
    <asp:HiddenField ID="hdnFootNote" runat="server" />
    <asp:HiddenField ID="hdnPMAs" runat="server" />
</div>
