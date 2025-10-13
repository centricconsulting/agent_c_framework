Imports System.Text

Namespace IFM.VR.Common.Helpers
    <Serializable()>
    Public Class WebValidationItem

        Public Property Message As String
        Public Property SenderClientId As String
        Public Property IsWarning As Boolean
        Public Property TitleText As String
        Public Property IsTitleChange As Boolean

        Dim _ScriptCollection As New List(Of String)
        Public ReadOnly Property ScriptCollection As List(Of String)
            Get
                Return _ScriptCollection
            End Get
        End Property

        Public ReadOnly Property ScriptText As String
            Get
                Dim sb As New StringBuilder()
                For Each ln As String In Me._ScriptCollection
                    sb.Append(ln)
                Next
                Return sb.ToString()
            End Get
        End Property

        Public Sub New(ByVal msg As String)
            Me.Message = msg
        End Sub

        Public Sub New(ByVal msg As String, isWarning As Boolean)
            Me.Message = msg
            Me.IsWarning = isWarning
        End Sub

        Public Sub New(ByVal msg As String, senderClientId As String)
            Me.Message = msg
            Me.SenderClientId = senderClientId
        End Sub

        Public Sub New(ByVal msg As String, senderClientId As String, isWarning As Boolean)
            Me.Message = msg
            Me.SenderClientId = senderClientId
            Me.IsWarning = isWarning
        End Sub

        Public Sub New(ByVal msg As String, senderClientId As String, IsTitleChange As Boolean, TitleText As String)
            Me.Message = msg
            Me.SenderClientId = senderClientId
            Me.IsTitleChange = IsTitleChange
            Me.TitleText = TitleText
        End Sub

        Public Sub New(ByVal msg As String, IsTitleChange As Boolean, TitleText As String)
            Me.Message = msg
            Me.IsTitleChange = IsTitleChange
            Me.TitleText = TitleText
        End Sub
    End Class
End Namespace