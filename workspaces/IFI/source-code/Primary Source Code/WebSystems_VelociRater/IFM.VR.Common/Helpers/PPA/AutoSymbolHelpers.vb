Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.MultiState

Namespace IFM.VR.Common.Helpers.PPA

    Public Class AutoSymbolHelpers

        'This helper is only used for PPA, so no multi state changes are needed 9/17/18 MLW

        Public Shared Function RescanAllAutoSymbols(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of IFM.VR.Common.Helpers.WebValidationItem)
            Dim validations As New List(Of IFM.VR.Common.Helpers.WebValidationItem)
            ' do a auto symbol - VIN verify
            Dim vNum As Int32 = 0
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.Vehicles IsNot Nothing Then
                    For Each v As QuickQuote.CommonObjects.QuickQuoteVehicle In topQuote.Vehicles
                        vNum += 1
                        If (String.IsNullOrWhiteSpace(v.CostNew) Or v.CostNew = "0") And (String.IsNullOrWhiteSpace(v.ActualCashValue) Or v.ActualCashValue = "0") Then
                            'Updated 10/18/2022 for task 75263 MLW
                            'Dim results = IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo(v.Vin, "", "", 0, If(IsDate(topQuote.EffectiveDate), topQuote.EffectiveDate, DateTime.MinValue), topQuote.VersionId.TryToGetInt32())
                            Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass
                            Dim lookupType As Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelISORAPA
                            If IFM.VR.Common.Helpers.PPA.VinLookup.IsNewModelISORAPALookupTypeAvailable(QQHelper.IntegerForString(topQuote.VersionId), QQHelper.DateForString(topQuote.EffectiveDate), If(topQuote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, False, True)) Then
                                lookupType = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelIsoRapaApi
                            End If
                            Dim effDate As String = If(topQuote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, topQuote.TransactionEffectiveDate, topQuote.EffectiveDate)
                            Dim results = IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo_OptionalLookupType(v.Vin, "", "", 0, If(IsDate(effDate), effDate, DateTime.MinValue), topQuote.VersionId.TryToGetInt32(), lookupType, topQuote.PolicyId, topQuote.PolicyImageNum, "0")
                            If results IsNot Nothing AndAlso results.Count = 1 Then
                                Dim result = results.FirstOrDefault()
                                Dim allVinInfoMatched As Boolean = True
                                If result.Make.Trim.ToLower() <> v.Make.ToLower().Trim() Then
                                    allVinInfoMatched = False
                                End If
                                If Not (result.Model.Trim.ToLower() = v.Model.ToLower().Trim() Or result.Description.Trim.ToLower() = v.Model.ToLower().Trim()) Then
                                    allVinInfoMatched = False
                                End If
                                If result.Year.ToString() <> v.Year.ToLower().Trim() Then
                                    allVinInfoMatched = False
                                End If
                                If allVinInfoMatched Then
                                    If v.VehicleSymbols IsNot Nothing AndAlso v.VehicleSymbols.Count > 1 Then
                                        Dim compSymbol = (From s In v.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "1" Select s).FirstOrDefault()

                                        If compSymbol IsNot Nothing Then
                                            If result.CompSymbol <> compSymbol.UserOverrideSymbol Then
                                                ' change symbol info
                                                Dim oldSymbol As String = compSymbol.UserOverrideSymbol
                                                compSymbol.SystemGeneratedSymbol = result.CompSymbol
                                                compSymbol.UserOverrideSymbol = result.CompSymbol
                                                compSymbol.SystemGeneratedSymbolVehicleInfoLookupTypeId = result.ResultVendor
                                                Dim newVal = New IFM.VR.Common.Helpers.WebValidationItem(String.Format("Vehicle #{2} - Comp symbol changed to {0} from {1}.", result.CompSymbol, oldSymbol, vNum), "")
                                                newVal.IsWarning = True
                                                validations.Add(newVal)
                                            End If
                                        End If

                                        Dim colliSymbol = (From s In v.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "2" Select s).FirstOrDefault()
                                        If colliSymbol IsNot Nothing Then
                                            If result.CollisionSymbol <> colliSymbol.UserOverrideSymbol Then
                                                'change symbol info
                                                Dim oldSymbol As String = colliSymbol.UserOverrideSymbol
                                                colliSymbol.SystemGeneratedSymbol = result.CollisionSymbol
                                                colliSymbol.UserOverrideSymbol = result.CollisionSymbol
                                                colliSymbol.SystemGeneratedSymbolVehicleInfoLookupTypeId = result.ResultVendor
                                                Dim newVal = New IFM.VR.Common.Helpers.WebValidationItem(String.Format("Vehicle #{2} - Collision symbol changed to {0} from {1}.", result.CollisionSymbol, oldSymbol, vNum), "")
                                                newVal.IsWarning = True
                                                validations.Add(newVal)
                                            End If
                                        End If


                                        Dim liabSymbol = (From s In v.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "3" Select s).FirstOrDefault()
                                        If liabSymbol IsNot Nothing Then
                                            If result.LiabilitySymbol <> liabSymbol.UserOverrideSymbol Then
                                                'change symbol info
                                                Dim oldSymbol As String = liabSymbol.UserOverrideSymbol
                                                liabSymbol.SystemGeneratedSymbol = result.LiabilitySymbol
                                                liabSymbol.UserOverrideSymbol = result.LiabilitySymbol
                                                liabSymbol.SystemGeneratedSymbolVehicleInfoLookupTypeId = "9"
                                                Dim newVal = New IFM.VR.Common.Helpers.WebValidationItem(String.Format("Vehicle #{2} - Liability symbol changed to {0} from {1}.", result.LiabilitySymbol, oldSymbol, vNum), "")
                                                newVal.IsWarning = True
                                                validations.Add(newVal)
                                            End If
                                        End If

                                        If IFM.VR.Common.Helpers.PPA.NewRAPASymbolsHelper.IsNewRAPASymbolsAvailable(topQuote) Then
                                            Dim bodilyInjurySymbol = (From s In v.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "8" Select s).FirstOrDefault()
                                            If bodilyInjurySymbol IsNot Nothing Then
                                                If result.BodilyInjurySymbol <> bodilyInjurySymbol.UserOverrideSymbol Then
                                                    'change symbol info
                                                    Dim oldSymbol As String = bodilyInjurySymbol.UserOverrideSymbol
                                                    bodilyInjurySymbol.SystemGeneratedSymbol = result.BodilyInjurySymbol
                                                    bodilyInjurySymbol.UserOverrideSymbol = result.BodilyInjurySymbol
                                                    bodilyInjurySymbol.SystemGeneratedSymbolVehicleInfoLookupTypeId = result.ResultVendor
                                                    Dim newVal = New IFM.VR.Common.Helpers.WebValidationItem(String.Format("Vehicle #{2} - Bodily Injury symbol changed to {0} from {1}.", result.BodilyInjurySymbol, oldSymbol, vNum), "")
                                                    newVal.IsWarning = True
                                                    validations.Add(newVal)
                                                End If
                                            End If

                                            Dim propertyDamageSymbol = (From s In v.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "9" Select s).FirstOrDefault()
                                            If propertyDamageSymbol IsNot Nothing Then
                                                If result.PropertyDamageSymbol <> propertyDamageSymbol.UserOverrideSymbol Then
                                                    'change symbol info
                                                    Dim oldSymbol As String = propertyDamageSymbol.UserOverrideSymbol
                                                    propertyDamageSymbol.SystemGeneratedSymbol = result.PropertyDamageSymbol
                                                    propertyDamageSymbol.UserOverrideSymbol = result.PropertyDamageSymbol
                                                    propertyDamageSymbol.SystemGeneratedSymbolVehicleInfoLookupTypeId = result.ResultVendor
                                                    Dim newVal = New IFM.VR.Common.Helpers.WebValidationItem(String.Format("Vehicle #{2} - Property Damage symbol changed to {0} from {1}.", result.PropertyDamageSymbol, oldSymbol, vNum), "")
                                                    newVal.IsWarning = True
                                                    validations.Add(newVal)
                                                End If
                                            End If

                                            Dim medPaySymbol = (From s In v.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = "14" Select s).FirstOrDefault()
                                            If medPaySymbol IsNot Nothing Then
                                                If result.MedPaySymbol <> medPaySymbol.UserOverrideSymbol Then
                                                    'change symbol info
                                                    Dim oldSymbol As String = medPaySymbol.UserOverrideSymbol
                                                    medPaySymbol.SystemGeneratedSymbol = result.MedPaySymbol
                                                    medPaySymbol.UserOverrideSymbol = result.MedPaySymbol
                                                    medPaySymbol.SystemGeneratedSymbolVehicleInfoLookupTypeId = result.ResultVendor
                                                    Dim newVal = New IFM.VR.Common.Helpers.WebValidationItem(String.Format("Vehicle #{2} - Med Pay symbol changed to {0} from {1}.", result.MedPaySymbol, oldSymbol, vNum), "")
                                                    newVal.IsWarning = True
                                                    validations.Add(newVal)
                                                End If
                                            End If
                                        End If
                                    End If
                                Else
                                    Dim newVal = New IFM.VR.Common.Helpers.WebValidationItem("Verify the combination of the VIN and Year/Make/Model for Vehicle #" + vNum.ToString() + ".")
                                    newVal.IsWarning = True
                                    validations.Add(newVal)
                                End If

                            End If
                        End If
                    Next
                End If
            End If
            Return validations
        End Function

    End Class

End Namespace