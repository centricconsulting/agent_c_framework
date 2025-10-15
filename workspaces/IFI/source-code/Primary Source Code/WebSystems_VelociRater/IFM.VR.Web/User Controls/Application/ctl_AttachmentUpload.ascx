<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ctl_AttachmentUpload.ascx.vb" Inherits="IFM.VR.Web.ctl_AttachmentUpload" %>

<script src="<%=ResolveClientUrl("~/js/VrFileUpload.js")%>"></script>



<script type="text/javascript">
    ///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
    ///<reference path="vr.core.js" />

    var hasShowUploadWarning = false;

    $(document).ready(function () {

        $("#txtDescription").keydown(function (event) {
           
            if (event.keyCode == 13)
            {
                $("#btnUploadFile").click();
                return false;
            }
        });

        $("#btnCancelUpload").click(function () {
            
            $("#txtDescription").val('');
            $("#fileUpload1").replaceWith($("#fileUpload1").clone(true));
            $("#fileUpload1").val(null);
            ifm.vr.ui.UnLockTree();
            EnableFormOnSaveRemoves();
            return false;
        });

        //$("#divProgress1").progressbar();


        $('#btnUploadFile').on('click', function () {
            DisableFormOnSaveRemoves();
            AttachmentUpload.DoFileUploadClick('#fileUpload1', '#lblProgress', '#divProgress', '#txtDescription');
        });

        $('#fileUpload1').on('change', function () {
            if (AttachmentUpload.IsAcceptableFile($(this).val()) == false) {
                /*$(this).replaceWith($(this).clone(true));*/
                $("#fileUpload1").replaceWith($("#fileUpload1").clone(true));
                $("#fileUpload1").val(null);
                alert('Invalid file type');
            }
            else {                
                if ($(this).get(0).files.length > 0) {// lock tree ??
                    $('#lblProgress1').text('');
                    ifm.vr.ui.LockTree();
                    //$('#lblProgress1').text(($(this).get(0).files[0].size / 1024).toFixed(1).toString() + 'k');
                }
                else {
                    $('#lblProgress1').text('');
                }
            }
            
            
            

        });


    });

    


</script>

<div id="divVrUpload" style="margin-bottom:40px;">
    <h3>
        <asp:Label ID="lblHeader" runat="server" Text="Upload a File"></asp:Label></h3>
    <div>
        <div style="margin-bottom: 10px;">
            <label for="fileUpload">
                Select File to Upload:</label>
            <br />
            <input id="fileUpload1" style="width: 400px;" type="file" />
            <input id="btnCancelUpload" class="StandardSaveButton" style="margin-left: 15px;" type="button" value="Clear"  />
            <br />
            Description:
            <br />
            <input id="txtDescription"  style="width: 400px;" type="text" maxlength="100" title="100 max characters"/>
            <br />
            <div id="divprogressArea">
                <div style="height: 5px; margin-top: 10px; width: 300px; display: inline-block;" id="divProgress1"></div>
                <label style="margin-left: 15px" id="lblProgress1"></label>
            </div>
        </div>


        <input id="btnUploadFile" class="StandardSaveButton" type="button" value="Upload File" />
        <div style="display:inline-block; margin-left:20px;">
            <span style="text-decoration:underline;">Supported file types are:</span>
            <br />
            doc, docx, rtf, txt, csv, mp3, m4a, wav, bmp, gif, jpg, <br />
            png, tif, xls, xlsx, pdf, mp4, wmv, mpg, avi, mov
        </div>

        <div style="margin-top: 20px;">
            Attached Files
        <div style="min-height: 30px; max-height: 200px; overflow-x: hidden; overflow-y: scroll; border: 1px solid black; background-color: white;">
            <div id="divAttachmentSearchResults"></div>
        </div>
        </div>
        <center>
        <div style="margin-top:20px;">
        <asp:Button ID="btnReturntoWorkflow" CssClass="StandardSaveButton" runat="server" OnClientClick="if ($('#fileUpload1').val() != ''){return confirm('You have a file selected but not yet uploaded. Continue to prior section without uploading the file?');}" Text="Return to " />
            </div>
            </center>
    </div>
</div>
<asp:HiddenField ID="HiddenField1" runat="server" />
