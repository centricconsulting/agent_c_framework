
Partial Class DiamondQuickQuoteVideos_QQ
    Inherits System.Web.UI.Page


    Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        If Request.QueryString("PF") IsNot Nothing AndAlso (UCase(Request.QueryString("PF").ToString) = "YES" OrElse UCase(Request.QueryString("PF").ToString) = "TRUE") Then
            Me.MasterPageFile = ConfigurationManager.AppSettings("DiamondQuickQuoteMasterPF")
        Else
            Me.MasterPageFile = ConfigurationManager.AppSettings("DiamondQuickQuoteMaster")
        End If
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Page.MaintainScrollPositionOnPostBack = True 'added 10/2/2012
        If Page.IsPostBack = False Then
            LoadVideos()
        End If
    End Sub
    Private Sub LoadVideos()
        Dim categoryCount As Integer = 0
        Dim videoCount As Integer = 0

        '10/2/2012 - updated YouTube links to use https for security warning (due to http/https combination)

        'Using sql As New SQLselectObject(ConfigurationManager.AppSettings("conn"))
        Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connQQ"))
            'sql.queryOrStoredProc = "SELECT C.categoryId, C.categoryDescription, C.categoryDisplayOrder, C.enabled, C.inserted, V.videoId, V.categoryId, V.videoFile, V.videoTitle, V.videoDisplayOrder, V.enabled, V.inserted FROM VelociRaterTrainingVideoCategories as C with (nolock) INNER JOIN VelociRaterTrainingVideos as V with (nolock) on V.categoryId = C.categoryId and V.enabled = 1 WHERE C.enabled = 1 ORDER BY COALESCE(C.categoryDisplayOrder, 9999), C.categoryId, COALESCE(V.videoDisplayOrder, 9999), V.videoId"
            'sql.queryOrStoredProc = "SELECT C.categoryId, C.categoryDescription, C.categoryDisplayOrder, V.videoId, V.videoFile, V.videoTitle, V.videoDisplayOrder, V.inserted FROM VelociRaterTrainingVideoCategories as C with (nolock) INNER JOIN VelociRaterTrainingVideos as V with (nolock) on V.categoryId = C.categoryId and V.enabled = 1 WHERE C.enabled = 1 ORDER BY COALESCE(C.categoryDisplayOrder, 9999), C.categoryId, COALESCE(V.videoDisplayOrder, 9999), V.videoId"
            sql.queryOrStoredProc = "usp_Get_TrainingVideos"
            Dim dr As Data.SqlClient.SqlDataReader = sql.GetDataReader
            If dr IsNot Nothing AndAlso dr.HasRows = True Then
                Dim currentCategoryId As Integer = 0
                Dim currentCategoryVideoRecords As String = ""
                Dim videoSrc As String = ""
                While dr.Read
                    If dr.Item("categoryId").ToString.Trim <> "" AndAlso IsNumeric(dr.Item("categoryId").ToString.Trim) = True AndAlso dr.Item("categoryDescription").ToString.Trim <> "" AndAlso dr.Item("videoFile").ToString.Trim <> "" AndAlso dr.Item("videoTitle").ToString.Trim <> "" Then
                        videoCount += 1
                        If CInt(dr.Item("categoryId").ToString.Trim) = currentCategoryId Then
                            'same category as previous record
                            'currentCategoryVideoRecords &= "<br /><a href=""#"" onclick=""javascript:document.getElementById('" & Me.ytPlayer.ClientID & "').src = https://www.youtube.com/embed/" & dr.Item("videoFile").ToString.Trim & "?" & If(ConfigurationManager.AppSettings("QuickQuote_AutoPlayTrainingVideos") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_AutoPlayTrainingVideos").ToString) = "YES", "autoplay=1&", "") & "origin=http://example.com;"">" & dr.Item("videoTitle").ToString.Trim & "</a>"
                            videoSrc = "https://www.youtube.com/embed/" & dr.Item("videoFile").ToString.Trim & "?" & If(ConfigurationManager.AppSettings("QuickQuote_AutoPlayTrainingVideos") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_AutoPlayTrainingVideos").ToString) = "YES", "autoplay=1&", "") & "origin=http://example.com"
                            'updated 10/24/2012 w/ logic to play videos in new window
                            If ConfigurationManager.AppSettings("QuickQuote_PlayTrainingVideosInNewWindow") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_PlayTrainingVideosInNewWindow").ToString) = "YES" AndAlso ConfigurationManager.AppSettings("QuickQuote_TrainingVideoPlayer") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_TrainingVideoPlayer").ToString <> "" Then
                                currentCategoryVideoRecords &= "<br /><br /><a href=""" & ConfigurationManager.AppSettings("QuickQuote_TrainingVideoPlayer").ToString & dr.Item("videoId").ToString.Trim & """ target=""_blank"">" & dr.Item("videoTitle").ToString.Trim & "</a>"
                            Else
                                currentCategoryVideoRecords &= "<br /><br /><a href=""#"" onclick=""javascript:document.getElementById('" & Me.ytPlayer.ClientID & "').src = '" & videoSrc & "'; document.getElementById('" & Me.ytPlayer.ClientID & "').style.display = 'block';"">" & dr.Item("videoTitle").ToString.Trim & "</a>" 'added scrollIntoView 10/2/2012 (document.getElementById('" & Me.ytPlayer.ClientID & "').focus();) (document.getElementById('" & Me.TestDiv2.ClientID & "').scrollIntoView();) (window.location.hash = '" & Me.TestDiv2.ClientID & "';)
                            End If
                            '5/27/2017 note: need to rebuild this project with 4.5 framework (to match server) in order for this page to work from this project... due to Microsoft changing iframe from HtmlGenericControl to HtmlIframe w/ 4.5; servers currently have old page version built with DiamondQuickQuote
                        Else
                            'new category
                            If Me.lblTrainingVideos.Text <> "" Then
                                'add video records and break before next category
                                Me.lblTrainingVideos.Text &= "<div id=""training_video" & categoryCount.ToString & """ class=""icongroup_trainingVideos"">" & currentCategoryVideoRecords & "</div><br />"
                            End If

                            categoryCount += 1
                            currentCategoryId = CInt(dr.Item("categoryId").ToString.Trim)
                            'Me.lblTrainingVideos.Text &= "<div class=""eg-bar""><span id=""training_video" & categoryCount.ToString & "-title"" class=""iconspan""><img src=""" & ConfigurationManager.AppSettings("NewSiteImagesWebPath") & ConfigurationManager.AppSettings("NewSiteImage_Minus_Logo") & """ /></span><b>" & dr.Item("categoryDescription").ToString.Trim & "</b></div>"
                            Me.lblTrainingVideos.Text &= "<div class=""eg-bar""><span id=""training_video" & categoryCount.ToString & "-title"" class=""iconspan""><img src=""" & ConfigurationManager.AppSettings("AgentsOnlyImagesWebPath") & ConfigurationManager.AppSettings("NewSiteImage_Minus_Logo") & """ /></span><b>" & dr.Item("categoryDescription").ToString.Trim & "</b></div>"
                            'currentCategoryVideoRecords = "<div id=""training_video" & categoryCount.ToString & """ class=""icongroup_trainingVideos"">" & CR.Col2TextHtml & "</div>"
                            'currentCategoryVideoRecords = "<a href=""#"" onclick=""javascript:document.getElementById('" & Me.ytPlayer.ClientID & "').src = https://www.youtube.com/embed/" & dr.Item("videoFile").ToString.Trim & "?" & If(ConfigurationManager.AppSettings("QuickQuote_AutoPlayTrainingVideos") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_AutoPlayTrainingVideos").ToString) = "YES", "autoplay=1&", "") & "origin=http://example.com;"">" & dr.Item("videoTitle").ToString.Trim & "</a>"
                            videoSrc = "https://www.youtube.com/embed/" & dr.Item("videoFile").ToString.Trim & "?" & If(ConfigurationManager.AppSettings("QuickQuote_AutoPlayTrainingVideos") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_AutoPlayTrainingVideos").ToString) = "YES", "autoplay=1&", "") & "origin=http://example.com"
                            'updated 10/24/2012 w/ logic to play videos in new window
                            If ConfigurationManager.AppSettings("QuickQuote_PlayTrainingVideosInNewWindow") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_PlayTrainingVideosInNewWindow").ToString) = "YES" AndAlso ConfigurationManager.AppSettings("QuickQuote_TrainingVideoPlayer") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_TrainingVideoPlayer").ToString <> "" Then
                                currentCategoryVideoRecords = "<a href=""" & ConfigurationManager.AppSettings("QuickQuote_TrainingVideoPlayer").ToString & dr.Item("videoId").ToString.Trim & """ target=""_blank"">" & dr.Item("videoTitle").ToString.Trim & "</a>"
                            Else
                                currentCategoryVideoRecords = "<a href=""#"" onclick=""javascript:document.getElementById('" & Me.ytPlayer.ClientID & "').src = '" & videoSrc & "'; document.getElementById('" & Me.ytPlayer.ClientID & "').style.display = 'block';"">" & dr.Item("videoTitle").ToString.Trim & "</a>" 'added scrollIntoView 10/2/2012 (document.getElementById('" & Me.ytPlayer.ClientID & "').focus();) (document.getElementById('" & Me.TestDiv2.ClientID & "').scrollIntoView();) (window.location.hash = '" & Me.TestDiv2.ClientID & "';)
                            End If
                            '5/27/2017 note: need to rebuild this project with 4.5 framework (to match server) in order for this page to work from this project... due to Microsoft changing iframe from HtmlGenericControl to HtmlIframe w/ 4.5; servers currently have old page version built with DiamondQuickQuote
                        End If
                    End If
                End While
                If currentCategoryVideoRecords <> "" Then
                    'add last set of video records
                    Me.lblTrainingVideos.Text &= "<div id=""training_video" & categoryCount.ToString & """ class=""icongroup_trainingVideos"">" & currentCategoryVideoRecords & "</div>"
                End If
            End If
        End Using

        If videoCount > 0 Then
            If categoryCount > 1 Then
                Me.ButtonArea.Visible = True
            Else
                Me.ButtonArea.Visible = False
            End If
        Else
            Me.ButtonArea.Visible = False
            Me.lblTrainingVideos.Text = "There are no training videos available."
        End If
    End Sub
End Class
