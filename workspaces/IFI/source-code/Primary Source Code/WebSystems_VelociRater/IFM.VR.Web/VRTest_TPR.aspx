<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VRTest_TPR.aspx.vb" Inherits="IFM.VR.Web.VRTest_TPR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div align="center">
        <asp:Panel ID="pnlMain" runat="server">
            <asp:Label ID="lblMsg" runat="server" Font-Size="20px" ForeColor="Red" Text="&nbsp;"></asp:Label>
            <table>
                <tr>
                    <td align="right">
                        <asp:Label ID="lbl1" runat="server" Text="Quote ID:"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:DropDownList id="ddlQuoteID" runat="server" Width="300px">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="117495 PATCH HOME PPC - Split Class ADDRESS MATCH" Value="117495"></asp:ListItem>
                            <asp:ListItem Text="117510 PATCH HOME PPC - Non-Split Class ADDRESS MATCH" Value="117510"></asp:ListItem>
                            <asp:ListItem Text="117654 PATCH HOME PPC - ZIP CODE MATCH" Value="117654"></asp:ListItem>
                            <asp:ListItem Text="117620 PATCH HOME PPC - NO MATCH" Value="117620"></asp:ListItem>
<%--                            <asp:ListItem Text="24832 QA HOME (CLUE HOM)" Value="24832"></asp:ListItem>
                            <asp:ListItem Text="24812 QA HOME (CLUE HOM)" Value="24812"></asp:ListItem>
                            <asp:ListItem Text="3361 QA HOME (Credit Data)" Value="3361"></asp:ListItem>
                            <asp:ListItem Text="3305 QA HOME (Empty)" Value="3305"></asp:ListItem>
                            <asp:ListItem Text="3205 QA AUTO" Value="3205"></asp:ListItem>
                            <asp:ListItem Text="2985 QA AUTO" Value="2985"></asp:ListItem>
                            <asp:ListItem Text="2993 QA AUTO" Value="2993"></asp:ListItem>
                            <asp:ListItem Text="3012 QA AUTO" Value="3012"></asp:ListItem>
                            <asp:ListItem Text="2657 QA AUTO" Value="2657"></asp:ListItem>--%>
                        </asp:DropDownList>
                        &nbsp;
                        <asp:RadioButton ID="rbList" runat="server" Text="&nbsp;" GroupName="Group1" AutoPostBack="true" />
                        <br />
                        <asp:TextBox ID="txtQuoteID" runat="server" Width="143px"></asp:TextBox>
                        &nbsp;
                        <asp:RadioButton ID="rbText" runat="server" Text="&nbsp;" GroupName="Group1" AutoPostBack="true" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label ID="Label1" runat="server" Text="Driver Num:"></asp:Label>
                    </td>
                    <td align="left">
                        <asp:Textbox ID="txtDriverNum" runat="server" Width="50px" ></asp:Textbox>
                    </td>
                </tr>
            </table>
            <br />
            <br />
            <asp:Button ID="btnPPC" runat="server" Text="PPC - HOM & DFR" Width="250px" />
            <br />
            <asp:Button ID="btnPA_MVR" runat="server" Text="MVR - Personal Auto" Width="250px" Visible="false" />
            <br />
            <asp:Button ID="btnPA_CLUE" runat="server" Text="CLUE - Personal Auto" Width="250px" Visible="false" />
            <br />
            <asp:Button ID="btnHOM_CLUE" runat="server" Text="CLUE - Personal Home" Width="250px"  Visible="false"/>
            <br />
            <asp:Button ID="btn_Credit_PH1" runat="server" Text="Credit - Policyholder 1" Width="250px" Visible="false" />
            <br />
            <asp:Button ID="btn_Credit_PH2" runat="server" Text="Credit - Policyholder 2" Width="250px"  Visible="false"/>
            <br />
            <asp:Button ID="btn_Credit_Driver" runat="server" Text="Credit - Driver" Width="250px"  Visible="false"/>
            <br />
            <asp:Button ID="btn_Credit_Applicant" runat="server" Text="Credit - Applicant (HOME)" Width="250px"  Visible="false"/>
            <br />
            <br />
            <asp:Label ID="label2" runat="server" Text="Results"></asp:Label>
            <br />
            <asp:TextBox ID="txtReportData" runat="server" TextMode="MultiLine" Width="400px" Height="200px"></asp:TextBox>
            <br />
            <asp:Button ID="btnRate" runat="server" Text="Rate QHOM085010" />
        </asp:Panel>
    </div>
    </form>
</body>
</html>
