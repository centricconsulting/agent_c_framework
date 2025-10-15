Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonMethods

Namespace IFM.VR.Common.Helpers.MultiState

    Public Class Vehicles
        Public Shared Function HasVehicleForEachSubQuote(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            Select Case topQuote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.AutoPersonal 'not multistate
                    Return topQuote.Locations.IsLoaded()
                Case Else
                    Return Not StateIdsWithOutAGaragedVehicle(topQuote).Any()
            End Select

        End Function

        Public Shared Function StateIdsWithOutAGaragedVehicle(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As IEnumerable(Of Int32)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If topQuote.Vehicles.IsLoaded() Then
                Dim distinctVehicleStateIds = (From l In topQuote.Vehicles Where TryToGetInt32(l.GaragingAddress.Address.StateId) > 0 Select l.GaragingAddress.Address.StateId.TryToGetInt32).Distinct()
                Return topQuote.QuoteStateIds.Except(distinctVehicleStateIds)
            End If
            Return topQuote.QuoteStateIds
        End Function

        Public Shared Function AreUMPDVehiclesOkay(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            Dim isOkay As Boolean = True
            Select Case topQuote.LobType
                Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                    If IFM.VR.Common.Helpers.CAP.CAP_UMPDLimitsHelper.IsUMPDLimitsAvailable(topQuote) Then
                        'need to find only vehicles that are IL with UMPD checked
                        Dim qqHelper As New QuickQuoteHelperClass
                        Dim quoteToUse As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
                        If topQuote IsNot Nothing Then
                            If QuickQuoteHelperClass.QuoteHasState(topQuote, QuickQuoteHelperClass.QuickQuoteState.Illinois) Then
                                quoteToUse = IFM.VR.Common.Helpers.MultiState.General.SubQuoteForState(IFM.VR.Common.Helpers.MultiState.General.SubQuotes(topQuote), QuickQuoteHelperClass.QuickQuoteState.Illinois)
                            End If
                        End If
                        If quoteToUse IsNot Nothing Then
                            If qqHelper.IsPositiveIntegerString(quoteToUse.UninsuredMotoristPropertyDamage_IL_LimitId) Then
                                'needs to have the UMPD limit selected at the policy level
                                Dim atLeastOneVehicleHasUMPDChecked As Boolean = False
                                For Each v In quoteToUse.Vehicles
                                    If v.HasUninsuredMotoristPropertyDamage = True AndAlso v.GaragingAddress?.Address?.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois Then
                                        'needs to have UMPD checked on the IL vehicle
                                        atLeastOneVehicleHasUMPDChecked = True
                                        Exit For
                                    End If
                                Next
                                If atLeastOneVehicleHasUMPDChecked = False Then
                                    isOkay = False
                                End If
                            End If
                        End If
                    End If
            End Select
            Return isOkay
        End Function
    End Class

End Namespace

