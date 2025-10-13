Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteDevDictionaryItem
        Private _key As String = ""
        Private _value As String = ""
        Private _page As String = ""
        Private _listControlIndex As Integer = -1
        Private _state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState = CommonMethods.QuickQuoteHelperClass.QuickQuoteState.None
        Public ReadOnly Property Key As String
            Get
                Return _key
            End Get
        End Property
        Public ReadOnly Property Value As String
            Get
                Return _value
            End Get
        End Property

        Public ReadOnly Property Page As String
            Get
                Return _page
            End Get
        End Property

        Public ReadOnly Property ListControlIndex As Integer
            Get
                Return _listControlIndex
            End Get
        End Property

        Public ReadOnly Property State As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
            Get
                Return _state
            End Get
        End Property

        Public Sub New()

        End Sub

        Protected Friend Sub SetKey(key As String)
            _key = key
        End Sub

        Protected Friend Sub SetValue(value As String)
            _value = value
        End Sub

        Protected Friend Sub SetPage(page As String)
            _page = page
        End Sub

        Protected Friend Sub SetState(state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState)
            _state = state
        End Sub

        Protected Friend Sub SetListControlIndex(listControlIndex As Integer)
            _listControlIndex = listControlIndex
        End Sub

    End Class
End Namespace
