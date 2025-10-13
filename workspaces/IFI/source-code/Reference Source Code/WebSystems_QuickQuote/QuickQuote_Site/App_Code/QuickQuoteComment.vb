Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteComment 'added 3/27/2017
        'Inherits QuickQuoteBaseGenericObject_DoesNotInheritBaseObject(Of Object) 'TODO: Dan - Needs Parent???

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
        Public Property LobId As Integer = 0
        Public Property CommentText As String = String.Empty

        Public Sub Reset()
            _LobId = 0
            _CommentText = String.Empty
        End Sub
        Public Sub Dispose()
            _LobId = Nothing
            qqHelper.DisposeString(_CommentText)
        End Sub
    End Class
End Namespace
