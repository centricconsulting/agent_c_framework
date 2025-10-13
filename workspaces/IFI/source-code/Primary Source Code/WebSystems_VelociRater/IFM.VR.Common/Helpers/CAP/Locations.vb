Imports IFM.VR.Common.Helpers.MultiState
Imports IFM.PrimativeExtensions


Namespace IFM.VR.Common.Helpers.CAP
    Public Class Locations

        Public Shared Sub EnsureLocationExistsForEachSubQuote(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, Optional SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If topQuote.Vehicles.IsLoaded() Then
                If SubQuotes Is Nothing Then
                    SubQuotes = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote)
                End If
                For Each v In topQuote.Vehicles
                    Dim garagingStateAbbreviation As String = v?.GaragingAddress?.Address?.State
                    If IFM.VR.Common.Helpers.States.DoesStateAbbreviationExist(garagingStateAbbreviation) Then
                        If IFM.VR.Common.Helpers.MultiState.General.SubQuotesContainsState(SubQuotes, garagingStateAbbreviation) Then 'has state part for the location you need
                            If (From l In topQuote.Locations Where l.Address.State = garagingStateAbbreviation Select l).Any() = False Then
                                'need to create location based on garaging address
                                Dim newLoc As New QuickQuote.CommonObjects.QuickQuoteLocation()
                                newLoc.Address = New QuickQuote.CommonObjects.QuickQuoteAddress()
                                newLoc.Address.City = v.GaragingAddress.Address.City
                                newLoc.Address.Zip = v.GaragingAddress.Address.Zip
                                newLoc.Address.State = v.GaragingAddress.Address.State
                                newLoc.Address.County = v.GaragingAddress.Address.County
                                topQuote.Locations.Add(newLoc)
                            End If
                        End If
                    End If
                Next
            End If
        End Sub

    End Class

End Namespace

