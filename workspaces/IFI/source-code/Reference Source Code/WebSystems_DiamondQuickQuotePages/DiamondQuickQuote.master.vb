Imports QuickQuote.CommonMethods 'added 5/19/2017

Partial Class DiamondQuickQuote_QQ
    Inherits System.Web.UI.MasterPage

    Dim qqHelper As New QuickQuoteHelperClass

    Dim MPR As New MasterPageRoutines 'added 8/21/2013

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            Me.CrumbsLogoutLink.HRef = ConfigurationManager.AppSettings("LogoutLink")
            Me.CrumbsChangePassword.HRef = ConfigurationManager.AppSettings("ChangePassword")
            Me.CrumbsContactAgent.HRef = ConfigurationManager.AppSettings("ContactAgent")
            Me.CrumbsHomeLink.HRef = ConfigurationManager.AppSettings("HomeLink")
            Me.FooterPrivacy.HRef = ConfigurationManager.AppSettings("PrivacyPolicy")
            Me.FooterLegal.HRef = ConfigurationManager.AppSettings("LegalNotice")
            Me.FooterContent.HRef = ConfigurationManager.AppSettings("ContentLink")
            Me.FooterStaff.HRef = ConfigurationManager.AppSettings("StaffLink")

            Me.HomeLink.HRef = ConfigurationManager.AppSettings("HomeLink")
            Me.AgentsOnlyLink.HRef = ConfigurationManager.AppSettings("MyAgencyLink")

            Me.NewQuoteLink.HRef = ConfigurationManager.AppSettings("QuickQuote_NewQuote")
            Me.ClientSearchLink.HRef = ConfigurationManager.AppSettings("QuickQuote_ClientSearch")
            Me.SavedQuotesLink.HRef = ConfigurationManager.AppSettings("QuickQuote_SavedQuotes")
            Me.TrainingVideosLink.HRef = ConfigurationManager.AppSettings("QuickQuote_TrainingVideos")

            'updated 2/7/2013 for Xml Selecter button/link
            If ConfigurationManager.AppSettings("QuickQuote_XmlSelecter") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_XmlSelecter").ToString <> "" Then
                Me.XmlSelecterLink.HRef = ConfigurationManager.AppSettings("QuickQuote_XmlSelecter").ToString
            Else
                Me.XmlSelecterLink.HRef = "DiamondQuickQuoteXmlSelecter.aspx"
            End If
            'If qqHelper.IsHomeOfficeStaffUser = True AndAlso Session("username") IsNot Nothing AndAlso UCase(Session("username").ToString) = "ITSTAFF" Then
            'updated 3/5/2013 to also show for QA
            If qqHelper.IsHomeOfficeStaffUser = True AndAlso Session("username") IsNot Nothing AndAlso (UCase(Session("username").ToString) = "ITSTAFF" OrElse UCase(Session("username").ToString) = "QAUSER") Then
                Me.XmlSelecterMenuButton.Visible = True
            Else
                Me.XmlSelecterMenuButton.Visible = False
            End If

            Me.bodytag.Attributes.Add("background", ConfigurationManager.AppSettings("DiamondQuickQuoteMaster_BodyBackground"))
            Me.imgLogo.Src = ConfigurationManager.AppSettings("DiamondQuickQuoteMaster_IFM_Logo")

            'added 8/21/2013; not using ELSE for now since the default is to set the row visibility to False (may override in markup for some reason)
            Dim webScrollContent As String = MPR.GetWebScrollContentForMasterPageType(MasterPageRoutines.MasterPageType.VelociRaterMasterPage)
            If webScrollContent <> "" Then
                Me.MarqueeRow.Visible = True
                Me.WebScrollArea.InnerHtml = webScrollContent
                'Else
                '    Me.MarqueeRow.Visible = False
            End If
        End If
    End Sub
End Class

