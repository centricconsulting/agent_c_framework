Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteMessage 'added 8/19/2017

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

        Public Property QuoteMessageId As Integer = 0
        Public Property QuoteId As Integer = 0
        Public Property QuoteXmlId As Integer = 0
        Public Property QuoteMessageTypeId As Integer = 0
        Public Property QuoteMessageText As String = ""
        Public Property Active As Boolean = True
        Public Property Inserted As String = String.Empty
        Public Property Updated As String = String.Empty
        Public Property JustAddedOrMaintainedOnSaveRate As Boolean = False 'added 8/23/2017

        Public Sub Reset()
            _QuoteMessageId = 0
            _QuoteId = 0
            _QuoteXmlId = 0
            _QuoteMessageTypeId = 0
            _QuoteMessageText = String.Empty
            _Active = True
            _Inserted = String.Empty
            _Updated = String.Empty
            _JustAddedOrMaintainedOnSaveRate = False 'added 8/23/2017
        End Sub
        Public Sub Dispose()
            _QuoteMessageId = Nothing
            _QuoteId = Nothing
            _QuoteXmlId = Nothing
            _QuoteMessageTypeId = Nothing
            qqHelper.DisposeString(_QuoteMessageText)
            _Active = Nothing
            qqHelper.DisposeString(_Inserted)
            qqHelper.DisposeString(_Updated)
            _JustAddedOrMaintainedOnSaveRate = Nothing 'added 8/23/2017
        End Sub

        Public Property QuoteMessageType As QuickQuoteXML.QuoteMessageType
            Get
                Dim qMsgType As QuickQuoteXML.QuoteMessageType = Nothing
                If System.Enum.IsDefined(GetType(QuickQuoteXML.QuoteMessageType), _QuoteMessageTypeId) = True Then
                    qMsgType = _QuoteMessageTypeId
                End If
                'If System.Enum.TryParse(Of QuickQuoteXML.QuoteMessageType)(_QuoteMessageTypeId, qMsgType) = False Then
                '    qMsgType = Nothing
                'End If
                Return qMsgType
            End Get
            Set(value As QuickQuoteXML.QuoteMessageType)
                _QuoteMessageTypeId = CInt(value)
            End Set
        End Property

    End Class
End Namespace
