<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="" CodeFile="DiamondQuickQuoteVideos.aspx.vb" Inherits="DiamondQuickQuoteVideos_QQ" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Scripts" runat="server">
    <title>Diamond Quick Quote Videos</title>
    <%--<link href="DiamondQuickQuoteStyles.css" rel="stylesheet" type="text/css" />--%><!--added 2/22/2013; then moved to master page 2/22/2013-->
    <%--<script type="text/javascript" src="js/CommonFunctions.js"></script>--%>

    <script type="text/javascript" src="js/switchcontent.js" ></script>

<script type="text/javascript" src="js/switchicon.js">

    /***********************************************
    * Switch Content script II (icon based)- (c) Dynamic Drive (www.dynamicdrive.com)
    * Requires switchcontent.js and included before this file!
    * Visit http://www.dynamicdrive.com/ for full source code
    ***********************************************/

</script>

<%--<script type="text/javascript">

   function sstchur_SmartScroller_GetCoords()
   {
      var scrollX, scrollY;
      
      if (document.all)
      {
         if (!document.documentElement.scrollLeft)
            scrollX = document.body.scrollLeft;
         else
            scrollX = document.documentElement.scrollLeft;
               
         if (!document.documentElement.scrollTop)
            scrollY = document.body.scrollTop;
         else
            scrollY = document.documentElement.scrollTop;
      }   
      else
      {
         scrollX = window.pageXOffset;
         scrollY = window.pageYOffset;
      }
   
      //document.forms[formID].xCoordHolder.value = scrollX;
     //document.forms[formID].yCoordHolder.value = scrollY;
//     document.forms['form1'].xCoordHolder.value = scrollX;
     //     document.forms['form1'].yCoordHolder.value = scrollY;
//     document.forms(0).xCoordHolder.value = scrollX;
     //     document.forms(0).yCoordHolder.value = scrollY;
//     document.form[0].xCoordHolder.value = scrollX;
     //     document.form[0].yCoordHolder.value = scrollY;
     document.getElementById('<%=ScrollX.ClientID%>').xCoordHolder.value = scrollX;
     document.getElementById('<%=ScrollY.ClientID%>').yCoordHolder.value = scrollY;
     //document.forms[0].xCoordHolder.value = scrollX;
     //document.forms[0].yCoordHolder.value = scrollY;
//     document.body.xCoordHolder.value = scrollX;
//     document.body.yCoordHolder.value = scrollY;
   }
   
   function sstchur_SmartScroller_Scroll()
   {
      //var x = document.forms[formID].xCoordHolder.value;
       //var y = document.forms[formID].yCoordHolder.value;
//       var x = document.forms['form1'].xCoordHolder.value;
       //       var y = document.forms['form1'].yCoordHolder.value;
//       var x = document.forms(0).xCoordHolder.value;
       //       var y = document.forms(0).yCoordHolder.value;
//       var x = document.form[0].xCoordHolder.value;
       //       var y = document.form[0].yCoordHolder.value;
       var x = document.getElementById('<%=ScrollX.ClientID%>').xCoordHolder.value;
       var y = document.getElementById('<%=ScrollY.ClientID%>').yCoordHolder.value;
       //var x = document.forms[0].xCoordHolder.value;
       //var y = document.forms[0].yCoordHolder.value;
//       var x = document.body.xCoordHolder.value;
//       var y = document.body.yCoordHolder.value;
      window.scrollTo(x, y);
   }
   
   window.onload = sstchur_SmartScroller_Scroll;
   window.onscroll = sstchur_SmartScroller_GetCoords;
   window.onkeypress = sstchur_SmartScroller_GetCoords;
   window.onclick = sstchur_SmartScroller_GetCoords;

</script>--%>

<%--<script type = "text/javascript">
    //    window.onload = function () {
    //        if (document.getElementById("scrollY")) {
    //            var scrollY = parseInt(document.getElementById("scrollY").value);
    //            if (!isNaN(scrollY)) {
    //                window.scrollTo(0, scrollY);
    //                //alert("onload scrollTo(0, " + scrollY + ")");
    //            }
    //        }
    //    };
    //    window.onscroll = function () {
    //        var scrollY = document.body.scrollTop;
    //        if (scrollY == 0) {
    //            if (window.pageYOffset) {
    //                scrollY = window.pageYOffset;
    //            }
    //            else {
    //                scrollY = (document.body.parentElement) ? document.body.parentElement.scrollTop : 0;
    //            }
    //        }
    //        if (scrollY > 0) {
    //            var input = document.getElementById("scrollY");
    //            if (input == null) {
    //                input = document.createElement("input");
    //                input.setAttribute("type", "hidden");
    //                input.setAttribute("id", "scrollY");
    //                input.setAttribute("name", "scrollY");
    //                document.forms[0].appendChild(input);
    //            }
    //            input.value = scrollY;
    //            //alert("onscroll scrollY = " + scrollY);
    //        }
    //        };
    function SetScrollPosition() {
        if (document.getElementById("scrollY")) {
            var scrollY = parseInt(document.getElementById("scrollY").value);
            if (!isNaN(scrollY)) {
                window.scrollTo(0, scrollY);
                alert("onload scrollTo(0, " + scrollY + ")");
            }
        }
    }
    function GetScrollPosition() {
        alert("GetScrollPosition");
        var scrollY = document.body.scrollTop;
        if (scrollY == 0) {
            if (window.pageYOffset) {
                scrollY = window.pageYOffset;
            }
            else {
                scrollY = (document.body.parentElement) ? document.body.parentElement.scrollTop : 0;
            }
        }
        if (scrollY > 0) {
            var input = document.getElementById("scrollY");
            if (input == null) {
                input = document.createElement("input");
                input.setAttribute("type", "hidden");
                input.setAttribute("id", "scrollY");
                input.setAttribute("name", "scrollY");
                document.forms[0].appendChild(input);
            }
            input.value = scrollY;
            alert("onscroll scrollY = " + scrollY);
        }
    }

    window.onload = SetScrollPosition;
    window.onscroll = GetScrollPosition;
    window.onkeypress = GetScrollPosition;
    window.onclick = GetScrollPosition;


</script>--%>

<style type="text/css">

/*Default style for SPAN icons. Edit if desired: */

.iconspan{
float: right;
margin: 3px;
cursor:hand;
cursor:pointer;
font-weight: bold;
}

/*CSS used to style the examples. Remove if desired: */

.eg-bar{
/*background-color: #EEF5D3;*/
/*background-color: #D3E4FE;*//*removed 2/22/2013*/
/*font-weight: bold;*/
border: 2px solid black;/*changed 2/22/2013 from 1px to 2px*/
padding: 3px;

/*added 2/22/2013*/
font-family:Calibri;
    font-size:12pt;
    /*font-size:14pt;*/
    font-weight:bolder;

background-color: #797777;
color:White;

/*roundedContainer*/
-moz-border-radius: 4px;
-webkit-border-radius: 4px;
border-radius: 4px;

/*gradient; Styles/Vr_NearMono.css*/
background-color: #797777;                       
/* IE9 SVG, needs conditional override of 'filter' to 'none' */
background: url(data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiA/Pgo8c3ZnIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgd2lkdGg9IjEwMCUiIGhlaWdodD0iMTAwJSIgdmlld0JveD0iMCAwIDEgMSIgcHJlc2VydmVBc3BlY3RSYXRpbz0ibm9uZSI+CiAgPGxpbmVhckdyYWRpZW50IGlkPSJncmFkLXVjZ2ctZ2VuZXJhdGVkIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSIgeDE9IjAlIiB5MT0iMCUiIHgyPSIwJSIgeTI9IjEwMCUiPgogICAgPHN0b3Agb2Zmc2V0PSIwJSIgc3RvcC1jb2xvcj0iIzdkN2U3ZCIgc3RvcC1vcGFjaXR5PSIxIi8+CiAgICA8c3RvcCBvZmZzZXQ9IjEwMCUiIHN0b3AtY29sb3I9IiMwZTBlMGUiIHN0b3Atb3BhY2l0eT0iMSIvPgogIDwvbGluZWFyR3JhZGllbnQ+CiAgPHJlY3QgeD0iMCIgeT0iMCIgd2lkdGg9IjEiIGhlaWdodD0iMSIgZmlsbD0idXJsKCNncmFkLXVjZ2ctZ2VuZXJhdGVkKSIgLz4KPC9zdmc+);
background: -moz-linear-gradient(top,  #7d7e7d 0%, #0e0e0e 100%); /* FF3.6+ */
background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,#7d7e7d), color-stop(100%,#0e0e0e)); /* Chrome,Safari4+ */
background: -webkit-linear-gradient(top,  #7d7e7d 0%,#0e0e0e 100%); /* Chrome10+,Safari5.1+ */
background: -o-linear-gradient(top,  #7d7e7d 0%,#0e0e0e 100%); /* Opera 11.10+ */
background: -ms-linear-gradient(top,  #7d7e7d 0%,#0e0e0e 100%); /* IE10+ */
background: linear-gradient(to bottom,  #7d7e7d 0%,#0e0e0e 100%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#7d7e7d', endColorstr='#0e0e0e',GradientType=0 ); /* IE6-8 */

/*standardShadow*/
moz-box-shadow: 1px 1px 2px #2d2c2b;
-webkit-box-shadow: 1px 1px 2px #2d2c2b;
box-shadow: 1px 1px 2px #2d2c2b;
}
[if lt IE 9]
.eg-bar{
/*background-color: #EEF5D3;*/
/*background-color: #D3E4FE;*//*removed 2/22/2013*/
/*font-weight: bold;*/
border: 2px solid black;/*changed 2/22/2013 from 1px to 2px*/
padding: 3px;

/*added 2/22/2013*/
font-family:Calibri;
    font-size:12pt;
    /*font-size:14pt;*/
    font-weight:bolder;

background-color: #797777;
color:White;

/*roundedContainer*/
-moz-border-radius: 4px;
-webkit-border-radius: 4px;
border-radius: 4px;

/*gradient; Styles/Vr_NearMono_Legacy.css*/
background-color: #797777;

/*standardShadow*/
moz-box-shadow: 1px 1px 2px #2d2c2b;
-webkit-box-shadow: 1px 1px 2px #2d2c2b;
box-shadow: 1px 1px 2px #2d2c2b;
}
[if gte IE 9]
.eg-bar{
/*background-color: #EEF5D3;*/
/*background-color: #D3E4FE;*//*removed 2/22/2013*/
/*font-weight: bold;*/
border: 2px solid black;/*changed 2/22/2013 from 1px to 2px*/
padding: 3px;

/*added 2/22/2013*/
font-family:Calibri;
    font-size:12pt;
    /*font-size:14pt;*/
    font-weight:bolder;

background-color: #797777;
color:White;

/*roundedContainer*/
-moz-border-radius: 4px;
-webkit-border-radius: 4px;
border-radius: 4px;

/*gradient; Styles/Vr_NearMono_IE_9_Only.css*/
filter: none;

/*standardShadow*/
moz-box-shadow: 1px 1px 2px #2d2c2b;
-webkit-box-shadow: 1px 1px 2px #2d2c2b;
box-shadow: 1px 1px 2px #2d2c2b;
}

div.eg-bar{
/*width: 500px;*/
}

.icongroup_trainingVideos{
/*width: 500px;*/
padding: 3px;
text-align:left;
}

</style>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Application" Runat="Server">
<br />
    <center><div style="width:600px;" class="tableField">
        <p style="text-align:center;" class="tableRowHeaderLarger">VelociRater Tutorials</p><br />

        <p runat="server" id="ButtonArea" style="text-align:center;">
            <a href="javascript:training_video.sweepToggle('contract')">Contract All</a> | <a href="javascript:training_video.sweepToggle('expand')">Expand All</a>
        </p>
        <br />
        <asp:Label ID="lblTrainingVideos" runat="server" />
        <br />
        <%--<div runat="server" id="TestDiv" tabindex="0">--%><IFRAME runat="server" id="ytPlayer" height="360" src="https://www.indianafarmers.com/NewPublicSite/PopupLoader.aspx" frameBorder="0" width="600" type="text/html" style="display:none;"></IFRAME><%--</div>--%><!--on iframe:   style="display:none;"-->
        <%--<br /><br /><br /><br /><br /><br /><div runat="server" id="TestDiv2" tabindex="0">Test</div>--%>
        <%--<input id="ScrollX" name="ScrollX" type="hidden" value="" runat="server" />
        <input id="ScrollY" name="ScrollY" type="hidden" value="" runat="server" />--%>
    </div></center>

<script type="text/javascript">

    var training_video = new switchicon("icongroup_trainingVideos", "div") //Limit scanning of switch contents to just "div" elements
    training_video.setHeader('<img src="images/minus.gif" />', '<img src="images/plus.gif" />') //set icon HTML
    //training_video.collapsePrevious(true) //Allow only 1 content open at any time
    training_video.collapsePrevious(false) //Allow more than 1 content to be open simultanously
    training_video.setPersist(false) //No persistence enabled
    //training_video.defaultExpanded(0) //Set 1st content to be expanded by default
    training_video.init()

</script>

</asp:Content>