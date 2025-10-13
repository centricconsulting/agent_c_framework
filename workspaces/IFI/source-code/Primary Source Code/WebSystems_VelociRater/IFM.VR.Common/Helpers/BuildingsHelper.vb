Imports IFM.VR.Common.Helpers.MultiState

Namespace IFM.VR.Common.Helpers
    Public Class BuildingsHelper

        ''' <summary>
        ''' Returns the first building at the lowest location number on a quote.
        ''' </summary>
        ''' <param name="quote"></param>
        ''' <returns></returns>
        Public Shared Function FindFirstBuilding(quote As QuickQuote.CommonObjects.QuickQuoteObject) As QuickQuote.CommonObjects.QuickQuoteBuilding
            'Todo - MS - No guarantee that the first building remain the first building does that matter? If new state is added it's location's building could now be the first build.
            If quote IsNot Nothing Then
                If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If quote.Locations IsNot Nothing Then
                    For Each l As QuickQuote.CommonObjects.QuickQuoteLocation In quote.Locations
                        If l IsNot Nothing Then
                            If l.Buildings IsNot Nothing Then
                                For Each b As QuickQuote.CommonObjects.QuickQuoteBuilding In l.Buildings
                                    If b IsNot Nothing Then
                                        Return b
                                    End If
                                Next
                            End If
                        End If
                    Next
                End If
            End If
            Return Nothing
        End Function

    End Class
End Namespace