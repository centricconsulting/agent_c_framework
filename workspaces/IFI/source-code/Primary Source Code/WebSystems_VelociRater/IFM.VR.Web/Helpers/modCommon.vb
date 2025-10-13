Imports System.Configuration.ConfigurationManager

Public Module modCommon

    ''' <summary>
    ''' Standard Error Handler
    ''' </summary>
    ''' <param name="ClassName"></param>  ' Name of calling class
    ''' <param name="RoutineName"></param>  ' Name of calling routine
    ''' <param name="Exc"></param>  ' The exception
    ''' <param name="ErrorLabel"></param>  ' If you have an error label on your page, pass it to have the error message written to it
    ''' <remarks></remarks>
    Public Sub HandleError(ByVal ClassName As String, ByVal RoutineName As String, ByVal Exc As Exception, Optional ByRef ErrorLabel As Label = Nothing)
        Dim rec As New IFM.ErrLog_Parameters_Structure()
        Dim msg As String = "Error Detected in " & ClassName & "(" & RoutineName & "): " & Exc.Message
        Dim err As String = Nothing

        ' Only show error messages in test (disabled)
        If ErrorLabel IsNot Nothing Then
            'If AppSettings("TestOrProd") IsNot Nothing AndAlso AppSettings("TestOrProd").ToUpper = "TEST" Then
            ErrorLabel.Text = msg
            'End If
        End If

        rec.ApplicationName = "Velocirater Personal"
        rec.ClassName = ClassName
        rec.ErrorMessage = Exc.Message
        rec.LogDate = DateTime.Now
        rec.RoutineName = RoutineName
        rec.StackTrace = Exc.StackTrace

        WriteErrorLogRecord(rec, err)

        Exit Sub
    End Sub

    ''' <summary>
    ''' Checks to see if a quote object field has a numeric value
    ''' Generally speaking if the field is a money field (premium, etc) you want GreaterThanZero to be true,
    ''' and if the field is an id field (deductible id, etc) then you want GreaterThanZero to be false
    ''' </summary>
    ''' <param name="FieldValue"></param>
    ''' <param name="ClassName"></param>
    ''' <param name="lbl"></param>
    ''' <param name="GreaterThanZero"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FieldHasNumericValue(ByVal FieldValue As String, ByVal ClassName As String, ByRef lbl As Label, ByVal GreaterThanZero As Boolean) As Boolean
        Try
            If FieldValue Is Nothing OrElse FieldValue.Trim = String.Empty Then Return False
            If IsNumeric(FieldValue) Then
                If GreaterThanZero Then
                    If CDec(FieldValue) > 0 Then
                        Return True
                    Else
                        Return False
                    End If
                Else
                    Return True
                End If
            End If

            Return False
        Catch ex As Exception
            HandleError("modCommon.FieldHasNumericValue called from " & ClassName, "FieldHasNumericValue", ex, lbl)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Checks to see if a string field has any value
    ''' </summary>
    ''' <param name="FieldValue"></param>
    ''' <param name="ClassName"></param>
    ''' <param name="lbl"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function FieldHasAnyValue(ByVal FieldValue As String, ByVal ClassName As String, ByRef lbl As Label) As Boolean
        Try
            If FieldValue Is Nothing OrElse FieldValue = String.Empty Then
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            HandleError(ClassName, "FieldHasAnyValue", ex, lbl)
            Return False
        End Try
    End Function

End Module