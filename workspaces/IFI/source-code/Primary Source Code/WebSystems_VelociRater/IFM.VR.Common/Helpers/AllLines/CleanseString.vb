Namespace IFM.VR.Common.Helpers.AllLines
    Public Class CleanseString

        Public Shared Function Clean(input As String) As String
            Dim cleanedString As String
            cleanedString = input.Replace(" –", " -") 'The item to be replaced is a ctrl-char item. These are not the same.
            'cleanedString = cleanedString.Replace("Old thing", "New thing") 
            Return cleanedString
        End Function

    End Class
End Namespace
