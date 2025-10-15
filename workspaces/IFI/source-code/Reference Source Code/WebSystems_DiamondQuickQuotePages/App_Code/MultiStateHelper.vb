Imports IFM.PrimativeExtensions

Public Class MultiStateHelper

    Public Shared Function GetSubQuotes(qqo As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of QuickQuote.CommonObjects.QuickQuoteObject)
        Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
        Return QQHelper.MultiStateQuickQuoteObjects(qqo)
    End Function

    Public Shared Function MultistateQuoteStateIds(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, Optional SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing) As IEnumerable(Of Int32)
        If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
            Throw New NotTopLevelQuoteException()
        End If
        If SubQuotes Is Nothing Then
            SubQuotes = GetSubQuotes(topQuote)
        End If
        Return From sq In SubQuotes Where sq.StateId.TryToGetInt32() > 0 Select sq.StateId.TryToGetInt32()
    End Function

    Public Class NotTopLevelQuoteException
        Inherits Exception
        Private Const StandardMessage As String = "Expected top level quote but received state level quote."
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New()
            MyBase.New(StandardMessage)
        End Sub
    End Class
End Class
