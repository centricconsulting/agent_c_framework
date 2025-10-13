
Imports QuickQuote.CommonObjects

Partial Class controls_Proposal_VRProposal_NewPayment_Page
    Inherits System.Web.UI.UserControl
    Public Property AgencyName As String
        Get
            Return Me.lblAgencyName.Text
        End Get
        Set(value As String)
            Me.lblAgencyName.Text = value
        End Set
    End Property

    ' Public Property Logo As String
    '    Get
    '        Return Me.lblLogo.Text
    '    End Get
    '    Set(value As String)
    '        Me.lblLogo.Text = value
    '    End Set
    'End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
         If Page.IsPostBack = False Then
            'If ConfigurationManager.AppSettings("QuickQuote_Proposal_MainPageLogo") IsNot Nothing AndAlso ConfigurationManager.AppSettings("QuickQuote_Proposal_MainPageLogo").ToString <> "" Then
            '    Me.MainPageLogo.Src = ConfigurationManager.AppSettings("QuickQuote_Proposal_MainPageLogo").ToString
            '    Me.MainPageLogo.Align = "left"
            '    Me.MainPageLogo.Width = 400
            'End If
            Dim myQuote As New QuickQuoteObject()
            CheckForRCCTextUpdate(myQuote)

         End If
    End Sub

    Private Sub CheckForRCCTextUpdate(quote As QuickQuoteObject)
        Dim doNotChangeRccText As Boolean = IsOKToChangeRccText()
        Dim isItRccPolicy As Boolean = CheckForRccPolicy(quote)
        If quote IsNot Nothing Then
            If doNotChangeRccText = True OrElse isItRccPolicy = True Then
                Me.RecurringPaymentsText.InnerHtml = " - Have payments automatically withdrawn from your checking or savings account through electronic funds transfer or recurring credit card payments."
            Else
                Me.RecurringPaymentsText.InnerHtml = " - Have payments automatically withdrawn from your bank account."
            End If
        End If
    End Sub

    Private Function IsOKToChangeRccText() As Boolean
        Dim rccTextChangeDate As Boolean = False
        If Date.Today < ProposalHelperClass.UpdateRCCTextDate Then
            rccTextChangeDate = True
        End If
        Return rccTextChangeDate
    End Function

     Private Function CheckForRccPolicy(quote As QuickQuoteObject) As Boolean
         Dim IsRccPolicy As Boolean = False
        If quote IsNot Nothing Then
            If ProposalHelperClass.IsRccPolicy(quote) = True Then
                IsRccPolicy = True
            Else
                IsRccPolicy = False
            End If
        End If
        Return IsRccPolicy
    End Function

End Class
