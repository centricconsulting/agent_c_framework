Imports System.Globalization

Module Converter
    Public Function ToInt32(intStr As String) As Integer
        Dim val As Integer = Nothing

        If Not [String].IsNullOrWhiteSpace(intStr) Then
            Int32.TryParse(intStr, val)
        End If

        Return val
    End Function

    Public Function ToInt64(longStr As String) As Long
        Dim val As Long = Nothing

        If Not [String].IsNullOrWhiteSpace(longStr) Then
            Int64.TryParse(longStr, val)
        End If

        Return val
    End Function

    Public Function ToBoolean(boolStr As String) As System.Nullable(Of Boolean)
        Dim val As System.Nullable(Of Boolean) = Nothing

        If Not [String].IsNullOrWhiteSpace(boolStr) Then
            Dim tmpVal As Boolean
            If [Boolean].TryParse(boolStr, tmpVal) Then
                val = tmpVal
            End If
        End If

        Return val
    End Function

    Private _textInfo As TextInfo = Nothing
    Public Function CapitalizeString(valueToCap As String) As String
        If valueToCap.HasValue() Then
            If _textInfo Is Nothing Then
                _textInfo = New CultureInfo("en-US", False).TextInfo
            End If

            Return _textInfo.ToTitleCase(valueToCap.ToLowerInvariant())
        Else
            Return [String].Empty
        End If
    End Function
End Module