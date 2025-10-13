
///<reference path="/js/3rdParty/jquery-2.1.0-vsdoc.js" />
///<reference path="vr.core.js" />



var AttachmentUpload = new function () {

    var curentDiskSpaceConsumed = 0.0;
    var quoteSize = (1024 * 1000) * 100; //100 meg

    this.DoFileUploadClick = function (clientId, lblId, progressBarId, descriptionId) {
        var data = new FormData();
               

        var files = $(clientId).get(0).files;
        $(lblId).attr('title', '');
        $(progressBarId).progressbar({ value: 0 });

        var description = encodeURI($(descriptionId).val());

        

        // Add the uploaded image content to the form data collection
        if (files.length > 0) {

            if (this.QuotaExceeded(files[0].size))
            {
                $(lblId).text("Cumulative directory size exceeded. You cannot upload more files.");
                alert("Cumulative directory size exceeded. You cannot upload more files.");
                EnableFormOnSaveRemoves();
                ifm.vr.ui.UnLockTree();
                $(descriptionId).val('');
                AttachmentUpload.QueryForattachedFiles(); //reload
                return;
            }

            if (this.IsAcceptableFileSize(files[0].size) == false) {
                $(lblId).text("File size limit of 10mb exceeded. You may select another file for upload.");
                alert("File size limit of 10mb exceeded. You may select another file for upload.");
                EnableFormOnSaveRemoves();
                ifm.vr.ui.UnLockTree();
                $(descriptionId).val('');
                AttachmentUpload.QueryForattachedFiles(); //reload
                return;
            }

            if (description.isEmptyOrWhiteSpace()) {
                alert('Description is required.');
                EnableFormOnSaveRemoves();
                ifm.vr.ui.UnLockTree();
                return;
            }

            //var filesize = ((files[0].size / 1024)).toFixed(1); // KB
            DisableFormOnSaveRemoves();
            data.append("UploadedImage", files[0]);
            $(clientId).prop("disabled", true);

            // Make Ajax request with the contentType = false, and procesDate = false; updated 6/27/2019 for Endorsements: replaced master_quoteID with master_quoteIdOrPolicyIdAndImageNum and master_AgencyId with ifm.vr.currentQuote.agencyId
            var ajaxRequest = $.ajax({
                type: "POST",
                url: "GenHandlers/Vr_Pers/QuoteAttachmentManager.ashx?action=upload&quoteId=" + master_quoteIdOrPolicyIdAndImageNum + "&agencyId=" + ifm.vr.currentQuote.agencyId + "&description=" + description,
                xhr: function () {
                    var myXhr = $.ajaxSettings.xhr();
                    if (myXhr.upload) {
                        myXhr.upload.addEventListener('progress', function (e) {
                            if (e.lengthComputable) {
                                try {
                                    secondsUntilLogOut = resetTime; // this resets the autologout timer - keeps autologout from logging you out if session was about to expire
                                }
                                catch (err) { }

                                var max = e.total;
                                var current = e.loaded;

                                var Percentage = (current * 100) / max;
                                //console.log(lblid + " " + Percentage.toFixed(1).toString() + "% Complete");                

                                if (Percentage.toFixed(1) > 5) {
                                    $(lblId).text(Percentage.toFixed(1).toString() + "% Complete");
                                    $(progressBarId).progressbar({ value: Percentage });
                                }
                                else { $(lblId).text('Pending.....') }

                                if (Percentage >= 100) {
                                    // process completed  
                                }
                            }
                        }
                            , false);
                    }
                    return myXhr;
                },
                cache: false,
                contentType: false,
                processData: false,
                data: data
            });

            ajaxRequest.done(function (xhr, textStatus) {

                // Do other operation
                EnableFormOnSaveRemoves();
                ifm.vr.ui.UnLockTree();
                $(descriptionId).val('');
                AttachmentUpload.QueryForattachedFiles(); //reload
                $(clientId).prop("disabled", false);
                var fn = $(clientId).val();
                $(clientId).replaceWith($(clientId).clone(true));
                $(clientId).val(null);
                if (ajaxRequest.status == 200)
                {
                    $(lblId).attr('title', 'Uploaded file: ' + files[0].filename);
                    /*alert('File ' + $(clientId).val() + ' successfully loaded.');*/
                    alert('File ' + fn + ' successfully loaded.');
                }
                else
                { $(lblId).text('Upload failed.'); }
            });

            ajaxRequest.fail(function (xhr, textStatus) {
                // Do other operation
                EnableFormOnSaveRemoves();
                ifm.vr.ui.UnLockTree();
                $(descriptionId).val('');
                AttachmentUpload.QueryForattachedFiles(); //reload
                $(clientId).prop("disabled", false);
                $(lblId).text('File ' + files[0].filename + ' not uploaded. Contact IT.');
                alert('File ' + files[0].filename + ' not uploaded. Contact IT.');
            });

        }
        else {
            $(lblId).text("No file selected.");
            alert("No file selected.");
            EnableFormOnSaveRemoves();
            ifm.vr.ui.UnLockTree();
            $(descriptionId).val('');
            AttachmentUpload.QueryForattachedFiles(); //reload
        }
        

    }

    function ShowLoadingMessage() {
        var resultsDivId = "divAttachmentSearchResults";
        $("#" + resultsDivId).empty();
        $("#" + resultsDivId).append('<span style="margin-left: auto;margin-right:auto; font-size:16pt; font-weight:bold;">Loading....</span>');
    }

    
    // Returns the amount of disk space currently consumed by all the files uploaded for this quote.
    this.DiskSpaceConsumed = function () { return curentDiskSpaceConsumed; };

    //updated 6/27/2019 for Endorsements: replaced master_quoteID with master_quoteIdOrPolicyIdAndImageNum and master_AgencyId with ifm.vr.currentQuote.agencyId
    this.QueryForattachedFiles = function () {
        curentDiskSpaceConsumed = 0;
        var resultsDivId = "divAttachmentSearchResults";
        ShowLoadingMessage();
        var genHandler = 'GenHandlers/Vr_Pers/QuoteAttachmentManager.ashx?action=query&quoteId=' + master_quoteIdOrPolicyIdAndImageNum + '&agencyId=' + ifm.vr.currentQuote.agencyId + '&ppopo=' + Math.random().toString().replace('.', '');
        $.getJSON(genHandler, {
            dataType: "json",
            data: "",
            cache: false,
            format: "json"
        })
        .done(function (data) {
            $("#" + resultsDivId).empty();

            var html = '<table style="width:100%; border-collapse: collapse;" class="tblList">';
            //html += '<tr>'
            //html += '<th style="width:60px;">';
            //html += '';
            //html += '</th>';
            //html += '<th>';
            //html += 'Filename';
            //html += '</th>';
            //html += '<th style="width:70px;">';
            //html += 'File Size';
            //html += '</th>';
            //html += '</tr>'
            
            $("#hdnTreeFileUploadCount").val(data.length.toString());
            $("#lblFileUploadCount").text($("#hdnTreeFileUploadCount").val());
            
            if (data.length > 0) {
                for (var ii = 0; ii < data.length; ii++) {
                    html += '<tr title="' + data[ii].memo.toString().htmlEscape() + '">'
                    html += '<td>';
                    html += '<a style="margin-left:5px;" onclick=\'if(hasShowUploadWarning == false){hasShowUploadWarning = true; alert("Open files with caution. Changes made to any documents will not be saved to the VelociRater quote.");}\' href="' + data[ii].url + '" target="_blank">Open</a>';
                    html += '</td>';

                    html += '<td title="' + data[ii].memo + ' [' + data[ii].fileName + ']">';
                    if (data[ii].memo != "")
                        html += data[ii].memo.toMaxLength(30) + " [" + data[ii].fileName.toMaxLength(35) + "]";
                    else
                        html += data[ii].fileName.toMaxLength(65);

                    //html += data[ii].memo;
                    html += '</td>';

                    html += '<td>';
                    html += (data[ii].fileSize / 1024).toFixed(0) + "k";
                    html += '</td>';

                    //if in acro
                    if (data[ii].importedToAcrosoft) {
                        html += '<td style="width:50px;text-align:center;" title="This file cannot be deleted">';
                        html += '&nbsp;'
                        html += '</td>';
                    }
                    else {
                        html += '<td style="width:50px;text-align:center;" title="Remove file from this quote/app">';
                        html += '<label style="text-decoration: underline; cursor:pointer;" onclick=\'if(confirm("Remove file?") == false){return;};$.ajax({url: "' + data[ii].removeUrl + '"}).always(function (){AttachmentUpload.QueryForattachedFiles();});\'>X</label>';
                        html += '</td>';
                    }
                    

                    html += '<td>';
                    html += '</td>';
                    html += '</tr>'

                    curentDiskSpaceConsumed += parseFloat(data[ii].fileSize);

                    //data[ii].removeUrl
                    //data[ii].importedToAcrosoft
                }
            }
            else
            {
                html += "<tr><td>No Files Attached</td></tr>"
            }

            html += '</table>';
            $("#" + resultsDivId).append(html);


        });

    }

    this.IsAcceptableFile = function (filename) {
        //fileUploadAcceptableTypes        
        var isOk = false;
        for (var x = 0; x < fileUploadAcceptableTypes.length; x++) {            
            if (filename.toLowerCase().endsWith(fileUploadAcceptableTypes[x]))
                isOk = true;
        }
        return isOk;
    };

    this.IsAcceptableFileSize = function (size) {
        size = (size / 1024);
        return size < 10000; // less than 10,000k or 10 meg

    };
    
    this.QuotaExceeded = function (fileSize) {
        return quoteSize < curentDiskSpaceConsumed + fileSize;
        
    };

}


