Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteDiamondProposalComment 'added 3/27/2017

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

        Public Property DiamondProposalImageCommentLinkId As Integer = 0
        Public Property DiamondProposalCommentId As Integer = 0
        Public Property LobId As Integer = 0
        Public Property CommentText As String = String.Empty

        Public Sub Reset()
            _DiamondProposalImageCommentLinkId = 0
            _DiamondProposalCommentId = 0
            _LobId = 0
            _CommentText = String.Empty
        End Sub
        Public Sub Dispose()
            _DiamondProposalImageCommentLinkId = Nothing
            _DiamondProposalCommentId = Nothing
            _LobId = Nothing
            qqHelper.DisposeString(_CommentText)
        End Sub
    End Class
End Namespace
