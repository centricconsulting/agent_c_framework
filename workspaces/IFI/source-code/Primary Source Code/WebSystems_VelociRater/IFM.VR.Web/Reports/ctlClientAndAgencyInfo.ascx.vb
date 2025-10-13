Public Class ctlClientAndAgencyInfo
    Inherits System.Web.UI.UserControl

    Dim quickQuote As QuickQuote.CommonObjects.QuickQuoteObject
    Dim QQxml As New QuickQuote.CommonMethods.QuickQuoteXML
    Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

    Public Property ClientInfo As String
        Get
            Return Me.lblClientInfo.Text
        End Get
        Set(value As String)
            Me.lblClientInfo.Text = value
        End Set
    End Property
    Public Property AgencyInfo As String
        Get
            Return Me.lblAgencyInfo.Text
        End Get
        Set(value As String)
            Me.lblAgencyInfo.Text = value
        End Set
    End Property

    Public Property ProducerCode As String
        Get
            Return Me.lblProducerCode.Text
        End Get
        Set(value As String)
            Me.lblProducerCode.Text = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then

        End If
    End Sub

    Public Sub ToggleProducer(state As Boolean)
        pnlProducer.Visible = state
    End Sub
End Class