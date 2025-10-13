Namespace Helpers
    Public Class MultistateHelper

        Public Shared Function FilterOutMultistateLOB(LobDictionary As Dictionary(Of String, Int32)) As Dictionary(Of String, Int32)
            Dim FilteredLOB As Dictionary(Of String, Int32) = New Dictionary(Of String, Int32)
            If LobDictionary IsNot Nothing Then
                For Each item In LobDictionary
                    If item.Key.Contains("Multi-State") = False Then
                        FilteredLOB.Add(item.Key, item.Value)
                    End If
                Next
                Return FilteredLOB
            End If
            Return LobDictionary
        End Function

    End Class

End Namespace
