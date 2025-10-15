Public Class Summary4ColumnItem
    Inherits Summary3ColumnItem
    Private _Address As String
        Public Property address() As String
            Get
                Return _Address
            End Get
            Set(ByVal value As String)
                _Address = value
            End Set
        End Property

        Public Sub New(coverageinput As String, addressinput As String, limitinput As String, premiuminput As String)
            MyBase.New(coverageinput, limitinput, premiuminput)
            _Address = addressinput
        End Sub

    End Class
