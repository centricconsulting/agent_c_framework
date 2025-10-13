<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="" CodeFile="DiamondQuickQuoteXmlSelecter.aspx.vb" Inherits="DiamondQuickQuoteXmlSelecter_QQ" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Scripts" runat="server">
    <title>Diamond Quick Quote Xml Selecter</title>
    <%--<link href="DiamondQuickQuoteStyles.css" rel="stylesheet" type="text/css" />--%><!--moved to master page 2/22/2013-->
    <script type="text/javascript" src="js/CommonFunctions.js"></script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Application" Runat="Server">
    <br />
    <div>
        <p align="center" class="tableRowHeaderLarger">VelociRater Xml Selecter</p>
        <p align="center" class="tableRowHeader">Please enter the parameters for the xml you'd like to view.</p>
        <table class="tableField" align="center" cellpadding="4">
            <tr>
                <td align="right" class="tableFieldHeader">Quote Id:</td>
                <td align="left" class="tableFieldValue">
                    <asp:Textbox runat="server" ID="txtQuoteId"></asp:Textbox>
                </td>
            </tr>
            <tr>
                <td align="right" class="tableFieldHeader">Quote Xml Id:</td>
                <td align="left" class="tableFieldValue">
                    <asp:Textbox runat="server" ID="txtQuoteXmlId"></asp:Textbox>
                    &nbsp;
                    <asp:CheckBox runat="server" ID="cbValidateQuoteXmlId" Text="Validate?" Checked="true" ToolTip="Checks to see if Quote Xml Id is associated with Quote Id" />
                </td>
            </tr>
            <tr>
                <td align="right" class="tableFieldHeader">Xml Type:</td>
                <td align="left" class="tableFieldValue">
                    <asp:DropDownList runat="server" ID="ddlXmlType">
                        <asp:ListItem>Last Available</asp:ListItem>
                        <asp:ListItem>Quote Request</asp:ListItem>
                        <asp:ListItem>Quote Response</asp:ListItem>
                        <asp:ListItem>AppGap Request</asp:ListItem>
                        <asp:ListItem>AppGap Response</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Button runat="server" ID="btnGetXml" Text="Get XML" CssClass="quickQuoteButton" />
                </td>
            </tr>
            <tr runat="server" id="XmlViewerLinkSection" visible="false">
                <td colspan="2" align="center" class="tableFieldValue">
                    You can use the link below if popup blocker automatically closes the new window.
                    <br />
                    You can also hold Ctrl down on your keyboard while clicking the 'Get XML' button.
                    <br />
                    <a runat="server" id="XmlViewerLink" target="_blank">Get XML</a>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>