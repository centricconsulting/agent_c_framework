
Partial Class DiamondQuickQuoteVideoPlayer_QQ
    Inherits System.Web.UI.Page

    Protected Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit
        If Request.QueryString("PrinterFriendlyVideoId") IsNot Nothing AndAlso Request.QueryString("PrinterFriendlyVideoId").ToString <> "" AndAlso IsNumeric(Request.QueryString("PrinterFriendlyVideoId").ToString) = True Then
            Me.MasterPageFile = ConfigurationManager.AppSettings("DiamondQuickQuoteMasterPF")
        Else
            Me.MasterPageFile = ConfigurationManager.AppSettings("DiamondQuickQuoteMaster")
        End If
    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Page.MaintainScrollPositionOnPostBack = True
        If Page.IsPostBack = False Then
            If Request.QueryString("VideoId") IsNot Nothing AndAlso Request.QueryString("VideoId").ToString <> "" AndAlso IsNumeric(Request.QueryString("VideoId").ToString) = True Then
                LoadVideo(Request.QueryString("VideoId").ToString)
            ElseIf Request.QueryString("PrinterFriendlyVideoId") IsNot Nothing AndAlso Request.QueryString("PrinterFriendlyVideoId").ToString <> "" AndAlso IsNumeric(Request.QueryString("PrinterFriendlyVideoId").ToString) = True Then
                LoadVideo(Request.QueryString("PrinterFriendlyVideoId").ToString)
            Else
                Me.VideoPlayerArea.Visible = False
                Me.lblTrainingVideo.Text = "No parameter was sent for the video id"
                Me.lblVideoTitle.Text = ""
                Me.lblVideoTitle.Visible = False
            End If
        End If
    End Sub
    Private Sub LoadVideo(ByVal videoId As String)
        Dim foundVideo As Boolean = False

        If videoId <> "" AndAlso IsNumeric(videoId) = True Then
            Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connQQ"))
                sql.queryOrStoredProc = "usp_Get_TrainingVideos"
                sql.parameter = New Data.SqlClient.SqlParameter("@videoId", CInt(videoId))
                Dim dr As Data.SqlClient.SqlDataReader = sql.GetDataReader
                If dr IsNot Nothing AndAlso dr.HasRows = True Then
                    Dim currentCategoryId As Integer = 0
                    Dim currentCategoryVideoRecords As String = ""
                    Dim videoSrc As String = ""
                    dr.Read()
                    If dr.Item("categoryId").ToString.Trim <> "" AndAlso IsNumeric(dr.Item("categoryId").ToString.Trim) = True AndAlso dr.Item("categoryDescription").ToString.Trim <> "" AndAlso dr.Item("videoFile").ToString.Trim <> "" AndAlso dr.Item("videoTitle").ToString.Trim <> "" Then
                        foundVideo = True
                        Me.lblVideoTitle.Text = ":&nbsp;&nbsp;" & dr.Item("categoryDescription").ToString.Trim & " - " & dr.Item("videoTitle").ToString.Trim
                        Me.ytPlayer.Attributes.Add("src", "https://www.youtube.com/embed/" & dr.Item("videoFile").ToString.Trim & "?" & If(ConfigurationManager.AppSettings("QuickQuote_AutoPlayTrainingVideos") IsNot Nothing AndAlso UCase(ConfigurationManager.AppSettings("QuickQuote_AutoPlayTrainingVideos").ToString) = "YES", "autoplay=1&", "") & "origin=http://example.com")
                        '5/27/2017 note: need to rebuild this project with 4.5 framework (to match server) in order for this page to work from this project... due to Microsoft changing iframe from HtmlGenericControl to HtmlIframe w/ 4.5; servers currently have old page version built with DiamondQuickQuote
                    End If
                End If
            End Using
        End If

        If foundVideo = True Then
            Me.lblVideoTitle.Visible = True
            Me.VideoPlayerArea.Visible = True
            Me.lblTrainingVideo.Text = ""
        Else
            Me.lblVideoTitle.Text = ""
            Me.lblVideoTitle.Visible = False
            Me.VideoPlayerArea.Visible = False
            Me.lblTrainingVideo.Text = "There was a problem locating this video."
        End If
    End Sub
End Class
