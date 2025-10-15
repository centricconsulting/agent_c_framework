Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Partial Class controls_Proposal_VRProposal_LOB_Generic_Summary 'added 4/20/2017
    Inherits System.Web.UI.UserControl

    Dim qqHelper As New QuickQuoteHelperClass

    Private _Quote As QuickQuoteObject
    Private _SubQuotes As List(Of QuickQuoteObject) 'added 10/11/2018
    Public Property Quote As QuickQuoteObject
        Get
            Return _Quote
        End Get
        Set(value As QuickQuoteObject)
            _Quote = value
            SetSummaryLabels()
        End Set
    End Property
    Public Property SubQuotes As List(Of QuickQuoteObject) 'added 10/11/2018
        Get
            If (_SubQuotes Is Nothing OrElse _SubQuotes.Count = 0) AndAlso _Quote IsNot Nothing Then
                _SubQuotes = qqHelper.MultiStateQuickQuoteObjects(_Quote)
            End If
            Return _SubQuotes
        End Get
        Set(value As List(Of QuickQuoteObject))
            _SubQuotes = value
        End Set
    End Property
    Public ReadOnly Property SubQuoteFirst As QuickQuoteObject 'added 10/11/2018
        Get
            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                Return SubQuotes.Item(0)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Public ReadOnly Property GoverningStateQuote As QuickQuoteObject 'added 10/11/2018
        Get
            Return qqHelper.GoverningStateQuote(_Quote, subQuotes:=Me.SubQuotes)
        End Get
    End Property
    Private _LinesInControl As Integer = 4 'breaks and rows; will need to adjust if any breaks are added; 5/13/2017 note: does not include Comments Row since logic below will add lines if needed
    Public ReadOnly Property LinesInControl As Integer
        Get
            Return _LinesInControl
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then

        End If
    End Sub
    Private Sub SetSummaryLabels()
        If _Quote IsNot Nothing Then
            With _Quote
                Me.lblPolicyNumber.Text = .PolicyNumber
                Me.lblTotalPremium.Text = .TotalQuotedPremium

                If qqHelper.IsPositiveIntegerString(.LobId) = True Then
                    'Me.lblLobName.Text = GetLobNameForLobId(.LobId)
                    'updated 4/22/2017 to use QQHelper method
                    Me.lblLobName.Text = QuickQuoteHelperClass.GetLobNameForLobId(.LobId)
                ElseIf qqHelper.IsPositiveIntegerString(.VersionId) = True Then
                    'Me.lblLobName.Text = GetLobNameForVersionId(.VersionId)
                    'updated 4/22/2017 to use QQHelper method
                    Me.lblLobName.Text = QuickQuoteHelperClass.GetLobNameForVersionId(.VersionId)
                End If

                If String.IsNullOrWhiteSpace(Me.lblLobName.Text) = False Then
                    Me.LobNameSection.Visible = True
                Else
                    Me.LobNameSection.Visible = False
                End If

                Dim commentsLineCount As Integer = 0
                Me.lblComments.Text = qqHelper.HtmlForQuickQuoteProposalComments(.Comments, includeBreakAtBeginning:=True, lineCount:=commentsLineCount)
                If commentsLineCount > 0 Then
                    _LinesInControl += commentsLineCount
                    Me.CommentsRow.Visible = True
                Else
                    Me.CommentsRow.Visible = False
                End If
            End With
        End If

    End Sub

    'Private Function GetLobNameForLobId(ByVal lobId As String) As String 'note: should move to QuickQuoteHelperClass; done 4/22/2017
    '    Dim lobName As String = ""

    '    If qqHelper.IsPositiveIntegerString(lobId) = True Then
    '        Using sso As New SQLselectObject(ConfigurationManager.AppSettings("connDiamond"))
    '            With sso
    '                .queryOrStoredProc = "SELECT L.lobname FROM Lob as L WITH (NOLOCK) WHERE L.lob_id = " & qqHelper.IntegerForString(lobId).ToString

    '                Dim dr As Data.SqlClient.SqlDataReader = .GetDataReader
    '                If dr IsNot Nothing AndAlso dr.HasRows = True Then
    '                    dr.Read()
    '                    lobName = dr.Item("lobname").ToString.Trim
    '                End If
    '            End With
    '        End Using
    '    End If

    '    Return lobName
    'End Function
    'Private Function GetLobNameForVersionId(ByVal versionId As String) As String 'note: should move to QuickQuoteHelperClass; done 4/22/2017
    '    Dim lobName As String = ""

    '    If qqHelper.IsPositiveIntegerString(versionId) = True Then
    '        Using sso As New SQLselectObject(ConfigurationManager.AppSettings("connDiamond"))
    '            With sso
    '                .queryOrStoredProc = "SELECT L.lobname FROM Lob as L WITH (NOLOCK) INNER JOIN Version as V WITH (NOLOCK) on V.lob_id = L.lob_id WHERE V.version_id = " & qqHelper.IntegerForString(versionId).ToString

    '                Dim dr As Data.SqlClient.SqlDataReader = .GetDataReader
    '                If dr IsNot Nothing AndAlso dr.HasRows = True Then
    '                    dr.Read()
    '                    lobName = dr.Item("lobname").ToString.Trim
    '                End If
    '            End With
    '        End Using
    '    End If

    '    Return lobName
    'End Function
End Class
