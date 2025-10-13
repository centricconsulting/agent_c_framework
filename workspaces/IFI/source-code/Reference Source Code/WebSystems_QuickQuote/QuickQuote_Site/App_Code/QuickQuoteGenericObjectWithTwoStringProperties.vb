Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteGenericObjectWithTwoStringProperties 'added 4/7/2017

        Dim qqHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass

        Public Property Property1 As String = String.Empty
        Public Property Property2 As String = String.Empty

        Public Sub Reset()
            _Property1 = String.Empty
            _Property2 = String.Empty
        End Sub
        Public Sub Dispose()
            qqHelper.DisposeString(_Property1)
            qqHelper.DisposeString(_Property2)
        End Sub
        Public Function PassesPairTypeRequirement(ByVal pairRequirement As QuickQuote.CommonMethods.QuickQuoteHelperClass.StringPairType) As Boolean
            Dim passes As Boolean = False

            Select Case pairRequirement
                Case CommonMethods.QuickQuoteHelperClass.StringPairType.BothNotEmptyAndNotWhiteSpace
                    If String.IsNullOrWhiteSpace(_Property1) = False AndAlso String.IsNullOrWhiteSpace(_Property2) = False Then
                        passes = True
                    End If
                Case CommonMethods.QuickQuoteHelperClass.StringPairType.AtLeastOneNotEmptyAndNotWhiteSpace
                    If String.IsNullOrWhiteSpace(_Property1) = False OrElse String.IsNullOrWhiteSpace(_Property2) = False Then
                        passes = True
                    End If
                Case CommonMethods.QuickQuoteHelperClass.StringPairType.BothNotEmpty
                    If String.IsNullOrEmpty(_Property1) = False AndAlso String.IsNullOrEmpty(_Property2) = False Then
                        passes = True
                    End If
                Case CommonMethods.QuickQuoteHelperClass.StringPairType.AtLeastOneNotEmpty
                    If String.IsNullOrEmpty(_Property1) = False OrElse String.IsNullOrEmpty(_Property2) = False Then
                        passes = True
                    End If
                Case Else 'CommonMethods.QuickQuoteHelperClass.StringPairType.None
                    passes = True
            End Select

            Return passes
        End Function
    End Class
End Namespace
