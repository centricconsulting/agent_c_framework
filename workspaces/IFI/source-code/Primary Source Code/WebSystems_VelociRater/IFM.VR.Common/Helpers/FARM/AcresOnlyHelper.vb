Imports IFM.VR.Common.Helpers.MultiState

Namespace IFM.VR.Common.Helpers.FARM
    Public Class AcresOnlyHelper

        Private Shared Function IsAcreageOnlyIncomplete(a As QuickQuote.CommonObjects.QuickQuoteAcreage) As Boolean
            If a.LocationAcreageTypeId <> "4" AndAlso (String.IsNullOrWhiteSpace(a.Acreage) Or String.IsNullOrWhiteSpace(a.County) Or String.IsNullOrWhiteSpace(a.Description) Or
                                        String.IsNullOrWhiteSpace(a.Range) Or String.IsNullOrWhiteSpace(a.Section) Or String.IsNullOrWhiteSpace(a.StateId) Or
                                         String.IsNullOrWhiteSpace(a.Twp)) Then ' or String.IsNullOrWhiteSpace(a.TownshipCodeTypeId) 'CAH-02/19/2019-Township not required now
                Return True
            End If
            Return False
        End Function

        Public Shared Function HasIncompleteAcreageOnlyRecords(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.Locations IsNot Nothing Then
                    For Each l In topQuote.Locations
                        If l.Acreages IsNot Nothing Then
                            For Each a In l.Acreages
                                If IsAcreageOnlyIncomplete(a) Then
                                    Return True
                                End If
                            Next
                        End If
                    Next
                End If
            End If
            Return False
        End Function

        Public Shared Sub RemoveIncompleteAcreageOnlyRecords(topQuote As QuickQuote.CommonObjects.QuickQuoteObject)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.Locations IsNot Nothing Then
                    For Each l In topQuote.Locations
                        If l.Acreages IsNot Nothing Then
                            Dim removeList As New List(Of QuickQuote.CommonObjects.QuickQuoteAcreage)
                            For Each a In l.Acreages
                                If IsAcreageOnlyIncomplete(a) Then
                                    removeList.Add(a)
                                End If
                            Next

                            'now remove any in the remove list
                            For Each ra In removeList
                                l.Acreages.Remove(ra)
                            Next

                        End If
                    Next
                End If
            End If

        End Sub

        Public Shared Function CurrentTotalAcres(MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation, excludePrimary As Boolean) As Int32
            If MyLocation IsNot Nothing AndAlso MyLocation.Acreages IsNot Nothing AndAlso MyLocation.Acreages.Any() Then
                Return MyLocation.Acreages.Sum(Function(x)
                                                   If x IsNot Nothing Then
                                                       If Int32.TryParse(x.Acreage, Nothing) Then
                                                           If excludePrimary Then
                                                               ' exclude 1 or 2
                                                               If String.IsNullOrWhiteSpace(x.LocationAcreageTypeId) Or x.LocationAcreageTypeId = "3" Then
                                                                   Return CInt(x.Acreage)
                                                               Else
                                                                   Return 0
                                                               End If
                                                           Else
                                                               If x.LocationAcreageTypeId <> "4" Then ' always exclude blanketacarage 4 in this logic
                                                                   Return CInt(x.Acreage)
                                                               End If
                                                           End If
                                                       End If
                                                   End If
                                                   Return 0
                                               End Function)
            End If
            Return 0

        End Function

    End Class
End Namespace