<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="" CodeFile="DiamondQuickQuoteVideoPlayer.aspx.vb" Inherits="DiamondQuickQuoteVideoPlayer_QQ" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Scripts" runat="server">
    <title>Diamond Quick Quote Video Player</title>
    <%--<link href="DiamondQuickQuoteStyles.css" rel="stylesheet" type="text/css" />--%><!--added 2/22/2013; then moved to master page 2/22/2013-->
    <%--<script type="text/javascript" src="js/CommonFunctions.js"></script>--%>
    

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Application" Runat="Server">
<br />
    <center><div style="width:960px;" class="tableField">
        <p style="text-align:center;" class="tableRowHeaderLarger">VelociRater Tutorial<asp:Label ID="lblVideoTitle" runat="server" visible="false" /></p><br />

        <br />
        <asp:Label ID="lblTrainingVideo" runat="server" />
        <br />
        <div runat="server" id="VideoPlayerArea" visible="false">
        <IFRAME runat="server" id="ytPlayer" height="540" src="https://www.indianafarmers.com/NewPublicSite/PopupLoader.aspx" frameBorder="0" width="960" type="text/html"></IFRAME>
            <%--<asp:HtmlIframe runat="server" id="ytPlayer" height="540" src="https://www.indianafarmers.com/NewPublicSite/PopupLoader.aspx" frameBorder="0" width="960" type="text/html"></asp:HtmlIframe>--%><%--9/8/2014 note: can use this but would need to register asp prefix to System.Web.UI.HtmlControls--%>
        </div>
    </div></center>



</asp:Content>