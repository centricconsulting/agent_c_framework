<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="VR3QuoteSummaryLoader.aspx.vb" Inherits="IFM.VR.Web.VR3QuoteSummaryLoader" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Velocirater Quote Summary Loader</title>
    <link href="DiamondQuickQuoteStyles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p align="center" class="tableRowHeaderLarger">Diamond Quote Proposal Loader</p>
            <p align="center" class="tableRowHeader">
                <asp:Label ID="lblMessage" runat="server" CssClass="normaltext" />
            </p>
        </div>
    </form>
</body>
</html>