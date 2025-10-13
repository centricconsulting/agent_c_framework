
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.MultiState

Namespace IFM.VR.Common.Helpers.PPA
    Public Class PrefillHelper

        'This helper is only used for PPA, so no multi state changes are needed 9/17/18 MLW
        Public Shared Sub LoadPrefill(Quote As QuickQuoteObject, ByRef prefillAddedDriversOrVehicles As Boolean, ByRef errorMsg As String, Optional discardNoVinHits As Boolean = False)
            Try
                If Quote IsNot Nothing Then
                    If Quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                        Throw New NotTopLevelQuoteException()
                    End If
                    'maybe this is a copied quote that already has drivers or vehicles
                    'we need to repull prefill to get prior bi and to create a record so we know where the prior bi info came from
                    If Quote.Drivers.IsLoaded OrElse Quote.Vehicles.IsLoaded Then
                        Dim pBI = JustGetPriorBi(Quote, errorMsg)
                        If pBI > 0 AndAlso Quote.PriorBodilyInjuryLimitId <> pBI Then
                            Quote.PriorBodilyInjuryLimitId = pBI
                            prefillAddedDriversOrVehicles = True 'lets the caller know something changed so you need to save
                        End If

                    Else
                        If HasPrefillBeenOrdered(Quote.PolicyId) = False AndAlso ((Quote.Drivers Is Nothing OrElse Quote.Drivers.Count = 0) AndAlso (Quote.Vehicles Is Nothing OrElse Quote.Vehicles.Count = 0) AndAlso (Quote.Client IsNot Nothing AndAlso Quote.Client.Name IsNot Nothing AndAlso Quote.Client.Name.LastName.Trim() <> "" AndAlso Quote.Client.Name.FirstName <> "" AndAlso Quote.Client.Address IsNot Nothing AndAlso Quote.Client.Address.City <> "")) Then
                            Dim QQHelper = New QuickQuoteHelperClass
                            Dim qqXml As New QuickQuoteXML()
                            Dim lblResults As String = Nothing
                            Dim errMessage As String = Nothing
                            Dim thirdPartyData As Diamond.Common.Objects.ThirdParty.ThirdPartyData = Nothing

                            RemoveEmptyDrivers(Quote)

                            qqXml.LoadPrefillForQuote(Quote, thirdPartyData, lblResults, errMessage)
                            If errMessage <> "" Then
                                errorMsg = errMessage
                            Else
                                Dim vehicles = thirdPartyData.SAQImportedVehicles
                                If (PPA_General.IsParachuteQuote(Quote)) Then
                                    Quote.PriorBodilyInjuryLimitId = PriorBiHelper.FindPriorBI(vehicles)
                                End If
                                If vehicles.Count > 0 Then
                                    If Quote.Vehicles Is Nothing Then
                                        Quote.Vehicles = New List(Of QuickQuoteVehicle)()
                                    End If
                                    For Each v In vehicles
                                        Dim dontIncludeVehicle As Boolean = False '8-8-2016 Matt A - for Hughes Project
                                        Dim newVehcicle As New QuickQuote.CommonObjects.QuickQuoteVehicle
                                        newVehcicle.Make = v.Make.ToUpper()
                                        newVehcicle.Model = v.Model.ToUpper()
                                        newVehcicle.Year = v.Year
                                        newVehcicle.Vin = v.Vin.ToUpper()
                                        ' no symbols so use VIN lookup to get symbols from it
                                        'Updated 10/19/2022 for task 75263 MLW
                                        'Dim VinResults = IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo(v.Vin, "", "", 0, If(IsDate(Quote.EffectiveDate), Quote.EffectiveDate, DateTime.MinValue), Quote.VersionId)
                                        Dim lookupType As Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelISORAPA
                                        If IFM.VR.Common.Helpers.PPA.VinLookup.IsNewModelISORAPALookupTypeAvailable(QQHelper.IntegerForString(Quote.VersionId), QQHelper.DateForString(Quote.EffectiveDate), If(Quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, False, True)) Then
                                             lookupType = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelIsoRapaApi
                                        End If
                                        Dim effDate As String = If(Quote.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, Quote.TransactionEffectiveDate, Quote.EffectiveDate)
                                        Dim VinResults = IFM.VR.Common.Helpers.PPA.VinLookup.GetMakeModelYearOrVinVehicleInfo_OptionalLookupType(v.Vin, "", "", 0, If(IsDate(effDate), effDate, DateTime.MinValue), Quote.VersionId, lookupType, Quote.PolicyId, Quote.PolicyImageNum, "0")

                                        If VinResults IsNot Nothing AndAlso VinResults.Count = 1 Then
                                            Dim vinResult = VinResults(0)
                                            ' overwrite prefill info if available - needed to confirm the VIN as accurate on Application
                                            newVehcicle.Make = vinResult.Make.ToUpper()
                                            newVehcicle.Model = If(String.IsNullOrWhiteSpace(vinResult.Model) = False, vinResult.Model.ToUpper(), vinResult.Description.ToUpper())
                                            newVehcicle.Year = vinResult.Year
                                            ' end over write

                                            newVehcicle.VehicleSymbols = New List(Of QuickQuoteVehicleSymbol)
                                            AddUpdateAutoSymbols(Quote, newVehcicle, "1", vinResult.CompSymbol)
                                            AddUpdateAutoSymbols(Quote, newVehcicle, "2", vinResult.CollisionSymbol)
                                            AddUpdateAutoSymbols(Quote, newVehcicle, "3", vinResult.LiabilitySymbol)

                                            If IFM.VR.Common.Helpers.PPA.NewRAPASymbolsHelper.IsNewRAPASymbolsAvailable(Quote) Then
                                                AddUpdateAutoSymbols(Quote, newVehcicle, "8", vinResult.BodilyInjurySymbol)
                                                AddUpdateAutoSymbols(Quote, newVehcicle, "9", vinResult.PropertyDamageSymbol)
                                                AddUpdateAutoSymbols(Quote, newVehcicle, "14", vinResult.MedPaySymbol)
                                            End If

                                            newVehcicle.BodyTypeId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId, vinResult.ISOBodyStyle, Quote.LobType)
                                            newVehcicle.RestraintTypeId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.RestraintTypeId, vinResult.RestraintDescription, Quote.LobType)
                                            newVehcicle.AntiTheftTypeId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.AntiTheftTypeId, vinResult.AntiTheftDescription, Quote.LobType)

                                        Else
                                            dontIncludeVehicle = discardNoVinHits ' this is probably a ATV, Trailer, or motorcycle that doesn't have VIN info in diamond

                                            ' no symbols will be auto added
                                            ' vin lookup did not make a match so use prefill
                                            Dim bodyTypeList = QQHelper.GetStaticDataList(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodyTypeId)
                                            newVehcicle.BodyTypeId = (From m In bodyTypeList.Options Where m.Text.Trim().ToLower() = v.BodyStyle.Trim().ToLower() Select m.Value).FirstOrDefault()

                                            Dim restraintList = QQHelper.GetStaticDataList(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.RestraintTypeId)
                                            newVehcicle.RestraintTypeId = (From m In restraintList.Options Where m.Text.Trim().ToLower() = v.Restraints.Trim().ToLower() Select m.Value).FirstOrDefault()

                                            Dim antiTheftList = QQHelper.GetStaticDataList(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.AntiTheftTypeId)
                                            newVehcicle.AntiTheftTypeId = (From m In antiTheftList.Options Where m.Text.Trim().ToLower() = v.Security.Trim().ToLower() Select m.Value).FirstOrDefault()

                                        End If


                                        If dontIncludeVehicle = False Then ' added 8-8-2016 Matt A for Hughes Project - this makes sure when only get cars,trucks,van that we have VIN info on
                                            SetNewVehicleDefaults(newVehcicle, Quote.QuickQuoteState) 'Updated 10/4/18 for multi state MLW - added Quote.QuickQuoteState
                                            Quote.HasBusinessMasterEnhancement = True
                                            
                                            'There is no way that vehicle #1 has nonowner status yet? this would throw an exception
                                            ' Matt 6-12-14
                                            'If Quote.Vehicles(0).NonOwnedNamed Then
                                            '    Quote.HasBusinessMasterEnhancement = False
                                            'Else
                                            '    Quote.HasBusinessMasterEnhancement = True
                                            'End If

                                            Quote.Vehicles.Add(newVehcicle)
                                            prefillAddedDriversOrVehicles = True
                                        End If

                                    Next
                                End If

                                Dim drivers = thirdPartyData.SAQImportedDrivers
                                If drivers.Count > 0 Then
                                    If Quote.Drivers Is Nothing Then
                                        Quote.Drivers = New List(Of QuickQuoteDriver)()
                                    End If
                                    For Each d In drivers
                                        Dim newDriver As New QuickQuote.CommonObjects.QuickQuoteDriver()
                                        newDriver.Name.BirthDate = d.BirthDate.DateTime.ToShortDateString()
                                        'newDriver.Name.DriversLicenseDate = d.DLDate 'Asked by BAs to just force auto calc everytime 2-20-14
                                        newDriver.Name.DriversLicenseNumber = d.DLN
                                        newDriver.Name.DriversLicenseDate = d.BirthDate.AddYears(16).DateTime.ToShortDateString()
                                        newDriver.Name.DriversLicenseStateId = d.DLNStateId
                                        newDriver.Name.FirstName = d.FirstName.ToUpper()
                                        newDriver.Name.LastName = d.LastName.ToUpper()

                                        If d.MaritalStatus <> "" Then
                                            Dim martialList = QQHelper.GetStaticDataList(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.MaritalStatusId)
                                            newDriver.Name.MaritalStatusId = (From m In martialList.Options Where m.Text.Trim().ToLower() = d.MaritalStatus.Trim().ToLower() Select m.Value).FirstOrDefault()
                                        End If

                                        newDriver.Name.MiddleName = d.MiddleName.ToUpper()
                                        newDriver.Name.PrefixName = d.PrefixName.ToUpper()
                                        newDriver.Name.SexId = d.SexId
                                        newDriver.Name.TaxNumber = d.SSN
                                        newDriver.Name.SuffixName = d.SuffixName.ToUpper().Replace(".", "")

                                        ' set all drivers to rated
                                        newDriver.DriverExcludeTypeId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.DriverExcludeTypeId, "RATED", Quote.LobType)

                                        ' if new driver name matches insured #1 then set relationship type to plicyholder
                                        If newDriver.Name.FirstName.Trim().ToLower() = Quote.Policyholder.Name.FirstName.Trim().ToLower() And newDriver.Name.LastName.Trim().ToLower() = Quote.Policyholder.Name.LastName.Trim().ToLower() Then
                                            newDriver.RelationshipTypeId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, "POLICYHOLDER", Quote.LobType)
                                        End If
                                        ' if new driver name matches insured #2 then set relationship type to plicyholder #2
                                        If newDriver.Name.FirstName.Trim().ToLower() = Quote.Policyholder2.Name.FirstName.Trim().ToLower() And newDriver.Name.LastName.Trim().ToLower() = Quote.Policyholder2.Name.LastName.Trim().ToLower() Then
                                            newDriver.RelationshipTypeId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteDriver, QuickQuoteHelperClass.QuickQuotePropertyName.RelationshipTypeId, "POLICYHOLDER #2", Quote.LobType)
                                        End If

                                        Quote.Drivers.Add(newDriver)
                                        prefillAddedDriversOrVehicles = True
                                    Next
                                End If


                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
            End Try
        End Sub

        Private Shared Function JustGetPriorBi(Quote As QuickQuoteObject, ByRef errorMsg As String) As Int32
            Dim PriorBodilyInjuryLimitId = 0
            'will not reorder if it has already been so safe to call as much as needed
            If Quote IsNot Nothing AndAlso HasPrefillBeenOrdered(Quote.PolicyId) = False AndAlso (Quote.Client IsNot Nothing AndAlso Quote.Client.Name IsNot Nothing AndAlso Quote.Client.Name.LastName.Trim() <> "" AndAlso Quote.Client.Name.FirstName <> "" AndAlso Quote.Client.Address IsNot Nothing AndAlso Quote.Client.Address.City <> "") Then
                Dim QQHelper = New QuickQuoteHelperClass
                Dim qqXml As New QuickQuoteXML()
                Dim lblResults As String = Nothing
                Dim errMessage As String = Nothing
                Dim thirdPartyData As Diamond.Common.Objects.ThirdParty.ThirdPartyData = Nothing

                RemoveEmptyDrivers(Quote)

                qqXml.LoadPrefillForQuote(Quote, thirdPartyData, lblResults, errMessage)
                If errMessage <> "" Then
                    errorMsg = errMessage
                Else
                    Dim vehicles = thirdPartyData.SAQImportedVehicles
                    If (PPA_General.IsParachuteQuote(Quote)) Then
                        PriorBodilyInjuryLimitId = PriorBiHelper.FindPriorBI(vehicles)
                    End If
                End If
            End If
            Return PriorBodilyInjuryLimitId
        End Function

        'Updated 10/4/18 for multi state MLW - added quoteState
        Public Shared Sub SetNewVehicleDefaults(newVehcicle As QuickQuote.CommonObjects.QuickQuoteVehicle, quoteState As QuickQuoteHelperClass.QuickQuoteState)
            SetNewVehicleDefaults_RetainTopLevelCoverages(newVehcicle, quoteState, Nothing)
            'Dim QQHelper = New QuickQuoteHelperClass
            ''Set Default Required Coverages
            'newVehcicle.BodilyInjuryLiabilityLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodilyInjuryLiabilityLimitId, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            'newVehcicle.PropertyDamageLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDamageLimitId, "100,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            'newVehcicle.MedicalPaymentsLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, "5,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)

            ''Updated 1/17/2022 for task 66101 MLW
            '''Updated 10/4/18 for multi state MLW
            ''If quoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois Then
            ''    newVehicle.UninsuredMotoristLiabilityLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            ''    newVehicle.UnderinsuredBodilyInjuryLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            ''    newVehicle.UninsuredMotoristPropertyDamageLimitId = ""
            ''    newVehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ""
            ''Else
            ''    newVehicle.UninsuredMotoristLiabilityLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristLiabilityLimitId, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            ''    newVehicle.UninsuredMotoristPropertyDamageLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, "100,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            ''    newVehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageDeductibleLimitId, "0", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            ''End If
            'Select Case quoteState
            '    Case QuickQuoteHelperClass.QuickQuoteState.Illinois
            '        'Updated 4/5/2022 for bug 68773 MLW
            '        'newVehicle.UninsuredMotoristLiabilityLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            '        newVehcicle.UninsuredBodilyInjuryLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            '        newVehcicle.UnderinsuredBodilyInjuryLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            '        newVehcicle.UninsuredMotoristPropertyDamageLimitId = ""
            '        newVehcicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ""
            '    Case QuickQuoteHelperClass.QuickQuoteState.Ohio
            '        'Updated 4/5/2022 for bug 68773 MLW
            '        'newVehicle.UninsuredMotoristLiabilityLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            '        newVehcicle.UninsuredBodilyInjuryLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            '        newVehcicle.UnderinsuredBodilyInjuryLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            '        newVehcicle.UninsuredMotoristPropertyDamageLimitId = ""
            '        newVehcicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ""
            '    Case Else
            '        newVehcicle.UninsuredMotoristLiabilityLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristLiabilityLimitId, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            '        newVehcicle.UninsuredMotoristPropertyDamageLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, "100,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            '        newVehcicle.UninsuredMotoristPropertyDamageDeductibleLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageDeductibleLimitId, "0", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            'End Select

            ''Set Default Optional Coverages
            'newVehcicle.ComprehensiveDeductibleLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.ComprehensiveDeductibleLimitId, "500", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            'newVehcicle.CollisionDeductibleLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.CollisionDeductibleLimitId, "500", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            'newVehcicle.TowingAndLaborDeductibleLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TowingAndLaborDeductibleLimitId, "25", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            'newVehcicle.TransportationExpenseLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TransportationExpenseLimitId, "30/900", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            'newVehcicle.HasAutoLoanOrLease = True
        End Sub

        Public Shared Sub SetNewVehicleDefaults_RetainTopLevelCoverages(newVehicle As QuickQuote.CommonObjects.QuickQuoteVehicle, quoteState As QuickQuoteHelperClass.QuickQuoteState, coveragesVehicle As QuickQuote.CommonObjects.QuickQuoteVehicle)
            Dim QQHelper = New QuickQuoteHelperClass
            Dim Quote = New QuickQuoteObject
            Dim IsTransportationAvailable As String = IFM.VR.Common.Helpers.PPA.TransportationExpenseHelper.IsTransportationExpenseAvailable(Quote).ToString

            If coveragesVehicle IsNot Nothing Then
                newVehicle.BodilyInjuryLiabilityLimitId = coveragesVehicle.BodilyInjuryLiabilityLimitId
                newVehicle.PropertyDamageLimitId = coveragesVehicle.PropertyDamageLimitId
                newVehicle.MedicalPaymentsLimitId = coveragesVehicle.MedicalPaymentsLimitId
                newVehicle.Liability_UM_UIM_LimitId = coveragesVehicle.Liability_UM_UIM_LimitId
                newVehicle.UninsuredCombinedSingleLimitId = coveragesVehicle.UninsuredCombinedSingleLimitId

                newVehicle.UninsuredBodilyInjuryLimitId = coveragesVehicle.UninsuredBodilyInjuryLimitId
                newVehicle.UnderinsuredBodilyInjuryLimitId = coveragesVehicle.UnderinsuredBodilyInjuryLimitId

                newVehicle.UninsuredMotoristLiabilityLimitId = coveragesVehicle.UninsuredMotoristLiabilityLimitId
                newVehicle.UninsuredMotoristPropertyDamageLimitId = coveragesVehicle.UninsuredMotoristPropertyDamageLimitId
                newVehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = coveragesVehicle.UninsuredMotoristPropertyDamageDeductibleLimitId
            Else

                'Set Default Required Coverages
                newVehicle.BodilyInjuryLiabilityLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.BodilyInjuryLiabilityLimitId, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                newVehicle.PropertyDamageLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyDamageLimitId, "100,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                newVehicle.MedicalPaymentsLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.MedicalPaymentsLimitId, "5,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)

                Select Case quoteState
                    Case QuickQuoteHelperClass.QuickQuoteState.Illinois
                        'Updated 4/5/2022 for bug 68773 MLW
                        'newVehicle.UninsuredMotoristLiabilityLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                        newVehicle.UninsuredBodilyInjuryLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                        newVehicle.UnderinsuredBodilyInjuryLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                        newVehicle.UninsuredMotoristPropertyDamageLimitId = ""
                        newVehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ""
                    Case QuickQuoteHelperClass.QuickQuoteState.Ohio
                        'Updated 4/5/2022 for bug 68773 MLW
                        'newVehicle.UninsuredMotoristLiabilityLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                        newVehicle.UninsuredBodilyInjuryLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                        newVehicle.UnderinsuredBodilyInjuryLimitId = QQHelper.GetStaticDataValueForTextAndState(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredBodilyInjuryLimitId, quoteState, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                        newVehicle.UninsuredMotoristPropertyDamageLimitId = ""
                        newVehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = ""
                    Case Else
                        newVehicle.UninsuredMotoristLiabilityLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristLiabilityLimitId, "100,000/300,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                        newVehicle.UninsuredMotoristPropertyDamageLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageLimitId, "100,000", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                        newVehicle.UninsuredMotoristPropertyDamageDeductibleLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageDeductibleLimitId, "0", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
                End Select
            End If

            'Set Default Optional Coverages
            newVehicle.ComprehensiveDeductibleLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.ComprehensiveDeductibleLimitId, "500", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            newVehicle.CollisionDeductibleLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.CollisionDeductibleLimitId, "500", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            newVehicle.TowingAndLaborDeductibleLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TowingAndLaborDeductibleLimitId, "25", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            If IsTransportationAvailable = "True" Then
                newVehicle.TransportationExpenseLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TransportationExpenseLimitId, "50/1500 30 DAYS", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            Else
                newVehicle.TransportationExpenseLimitId = QQHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteVehicle, QuickQuoteHelperClass.QuickQuotePropertyName.TransportationExpenseLimitId, "30/900", QuickQuoteObject.QuickQuoteLobType.AutoPersonal)
            End If
            newVehicle.HasAutoLoanOrLease = True
        End Sub


        Public Shared Function HasPrefillBeenOrdered(policyId As String) As Boolean
            Using sq As New SQLselectObject
                Dim param As New SqlClient.SqlParameter("@policy_Id", policyId)
                Dim saqPrefillDS As System.Data.DataSet
                saqPrefillDS = sq.GetDataset(System.Configuration.ConfigurationManager.AppSettings("connDiamond"), "assp_ChoicePoint_LoadSAQAndPrefillData", param)
                If saqPrefillDS.Tables.Count > 1 AndAlso saqPrefillDS.Tables(0).Rows.Count > 0 Then
                    Return True
                End If
            End Using
            Return False
        End Function

        Public Shared Function AddUpdateAutoSymbols(quote As QuickQuote.CommonObjects.QuickQuoteObject, vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle, symbolTypeId As String, symbolValue As String)
            Dim symbol = (From s In vehicle.VehicleSymbols Where s.VehicleSymbolCoverageTypeId = symbolTypeId Select s).FirstOrDefault()
            If (symbolTypeId.TryToGetInt32() = 3 AndAlso IFM.VR.Common.Helpers.PPA.PPA_General.IsParachuteQuote(quote) = False) Or symbolValue.IsNullEmptyorWhitespace() Then
                If symbol IsNot Nothing Then
                    vehicle.VehicleSymbols.Remove(symbol)
                End If
                Return False
            End If

            If symbol Is Nothing Then
                symbol = vehicle.VehicleSymbols.AddNew()
            End If

            symbol.SystemGeneratedSymbol = symbolValue
            symbol.UserOverrideSymbol = symbolValue
            symbol.VehicleSymbolCoverageTypeId = symbolTypeId
            'Updated 10/11/2022 for task 75263 MLW
            Dim qqHelper As New QuickQuoteHelperClass 
            If VinLookup.IsNewModelISORAPALookupTypeAvailable(qqHelper.IntegerForString(quote.VersionId), qqHelper.DateForString(quote.EffectiveDate), If(quote.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote, False, True)) Then
                symbol.SystemGeneratedSymbolVehicleInfoLookupTypeId = CInt(Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelIsoRapaApi).ToString()
            Else
                symbol.SystemGeneratedSymbolVehicleInfoLookupTypeId = CInt(Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelISORAPA).ToString()
            End If
            'symbol.SystemGeneratedSymbolVehicleInfoLookupTypeId = CInt(Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelISORAPA).ToString()
            Return True
        End Function

        Private Shared Sub RemoveEmptyDrivers(quote As QuickQuoteObject)
            'ws-574 CAH DriverNameLink
            'Delete any Driver that has no Name Information
            If quote?.Drivers.IsLoaded AndAlso quote?.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
                Dim removeDrivers As List(Of QuickQuoteDriver) = New List(Of QuickQuoteDriver)
                For Each driver As QuickQuoteDriver In quote.Drivers
                    If String.IsNullOrWhiteSpace(driver.Name.FirstName) AndAlso
                            String.IsNullOrWhiteSpace(driver.Name.LastName) Then
                        removeDrivers.Add(driver)
                    End If
                Next
                For Each driver As QuickQuoteDriver In removeDrivers
                    quote.Drivers.Remove(driver)
                Next
            End If
        End Sub
    End Class

End Namespace
