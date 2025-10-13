Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.MultiState
    Public Class Locations
        Public Shared Function DoesEachSubQuoteContainALocation(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, Optional SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing) As Boolean
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If SubQuotes Is Nothing Then
                SubQuotes = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote)
            End If
            Return Not SubQuoteStateIdsWithNoLocation(topQuote, SubQuotes).Any()
        End Function

        Public Shared Function SubQuoteStateIdsWithNoLocation(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, Optional SubQuotes As List(Of QuickQuote.CommonObjects.QuickQuoteObject) = Nothing) As IEnumerable(Of Int32)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If SubQuotes Is Nothing Then
                SubQuotes = IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote)
            End If
            Dim topQuoteStateIds = IFM.VR.Common.Helpers.MultiState.General.MultistateQuoteStateIds(topQuote, SubQuotes)
            If topQuote.Locations IsNot Nothing AndAlso topQuote.Locations.Any() Then
                Dim locationStateIds = (From l In topQuote.Locations Where l?.Address?.StateId.TryToGetInt32() > 0 Select l.Address.StateId.TryToGetInt32()).Distinct()
                Return topQuoteStateIds.Except(locationStateIds)
            End If
            Return topQuoteStateIds
        End Function

        Public Shared Function LocationsForGoverningState(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, governingState As QuickQuote.CommonObjects.QuickQuoteObject) As IEnumerable(Of QuickQuote.CommonObjects.QuickQuoteLocation)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Return LocationsForQuickQuoteState(topQuote.Locations, governingState.QuickQuoteState)
        End Function

        Public Shared Function LocationsForQuickQuoteState(locations As List(Of QuickQuote.CommonObjects.QuickQuoteLocation), state As QuickQuoteState) As IEnumerable(Of QuickQuote.CommonObjects.QuickQuoteLocation)
            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
            Return QQHelper.LocationsForQuickQuoteState(locations, state)
        End Function

        Public Shared Function HasIneligibleLocationsForQuote(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            Return IneligibleLocationStateIdsForQuote(topQuote).Any()
        End Function

        Public Shared Function IneligibleLocationStateIdsForQuote(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As IEnumerable(Of Int32)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If topQuote.Locations.IsLoaded() Then
                Dim distinctLocationIds = (From l In topQuote.Locations Where TryToGetInt32(l.Address.StateId) > 0 Select l.Address.StateId.TryToGetInt32).Distinct()
                Return distinctLocationIds.Except(topQuote.QuoteStateIds)
            End If
            Return New List(Of Int32)
        End Function

        Public Shared Function GetFirstLocationForStateQuote(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, state As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState) As QuickQuote.CommonObjects.QuickQuoteLocation
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If topQuote.Locations.IsLoaded() Then
                Return (From l In topQuote.Locations Where l.Address.QuickQuoteState = state Select l).FirstOrDefault()
            End If
            Return Nothing
        End Function

        Public Shared Function IsFirstLocationForAnySubQuote(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, location As QuickQuote.CommonObjects.QuickQuoteLocation) As Boolean
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If location IsNot Nothing Then
                If topQuote.Locations.IsLoaded() Then
                    Dim firstLocation = (From l In topQuote.Locations Where l.Address.QuickQuoteState = location.Address.QuickQuoteState Select l).FirstOrDefault()
                    Return location.Equals(firstLocation)
                End If
            End If
            Return False
        End Function

        Public Shared Function GetFirstLocationForStateQuote(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, stateId As Int32?) As QuickQuote.CommonObjects.QuickQuoteLocation
            If (stateId.HasValue) Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.Locations.IsLoaded() Then
                    If topQuote.QuoteStateIds.Count > 1 Then
                        Return (From l In topQuote.Locations Where l.Address.StateId = stateId.Value Select l).FirstOrDefault()
                    Else
                        'if only one state on quote then just use first location
                        Return topQuote.Locations.GetItemAtIndex(0)
                    End If

                End If
            End If

            Return Nothing
        End Function



    End Class
End Namespace

