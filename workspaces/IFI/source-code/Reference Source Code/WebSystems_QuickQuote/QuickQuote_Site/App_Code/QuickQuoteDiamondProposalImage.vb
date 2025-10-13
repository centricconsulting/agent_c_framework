Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteDiamondProposalImage 'added 3/27/2017

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

        Public Property DiamondProposalImageLinkId As Integer = 0
        Public Property PolicyId As Integer = 0
        Public Property PolicyImageNum As Integer = 0
        Public Property PolicyNumber As String = String.Empty
        Public Property QuoteNumber As String = String.Empty
        Public Property Comments As List(Of QuickQuoteDiamondProposalComment) = Nothing
        Public Property AgencyId As Integer = 0 'added 3/29/2017
        Public Property AgencyCode As String = String.Empty 'added 3/29/2017

        Public Sub Reset()
            _DiamondProposalImageLinkId = 0
            _PolicyId = 0
            _PolicyImageNum = 0
            _PolicyNumber = String.Empty
            _QuoteNumber = String.Empty
            _Comments = Nothing
            _AgencyId = 0 'added 3/29/2017
            _AgencyCode = String.Empty 'added 3/29/2017
        End Sub
        Public Sub Dispose()
            _DiamondProposalImageLinkId = Nothing
            _PolicyId = Nothing
            _PolicyImageNum = Nothing
            qqHelper.DisposeString(_PolicyNumber)
            qqHelper.DisposeString(_QuoteNumber)
            If _Comments IsNot Nothing Then
                If _Comments.Count > 0 Then
                    For Each c As QuickQuoteDiamondProposalComment In _Comments
                        If c IsNot Nothing Then
                            c.Dispose()
                            c = Nothing
                        End If
                    Next
                    _Comments.Clear()
                End If
                _Comments = Nothing
            End If
            _AgencyId = Nothing 'added 3/29/2017
            qqHelper.DisposeString(_AgencyCode) 'added 3/29/2017
        End Sub
    End Class
End Namespace
