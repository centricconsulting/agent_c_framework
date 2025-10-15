Imports Microsoft.VisualBasic

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteGenericObjectWithTwoIntegerProperties 'added 4/7/2017
        Public Property Property1 As Integer = 0
        Public Property Property2 As Integer = 0

        Public Sub Reset()
            _Property1 = 0
            _Property2 = 0
        End Sub
        Public Sub Dispose()
            _Property1 = Nothing
            _Property2 = Nothing
        End Sub
        Public Function PassesPairTypeRequirement(ByVal pairRequirement As QuickQuote.CommonMethods.QuickQuoteHelperClass.IntegerPairType) As Boolean
            Dim passes As Boolean = False

            Select Case pairRequirement
                Case CommonMethods.QuickQuoteHelperClass.IntegerPairType.BothPositive
                    If _Property1 > 0 AndAlso _Property2 > 0 Then
                        passes = True
                    End If
                Case CommonMethods.QuickQuoteHelperClass.IntegerPairType.AtLeastOnePositive
                    If _Property1 > 0 OrElse _Property2 > 0 Then
                        passes = True
                    End If
                Case Else 'CommonMethods.QuickQuoteHelperClass.IntegerPairType.None
                    passes = True
            End Select

            Return passes
        End Function
    End Class
End Namespace
