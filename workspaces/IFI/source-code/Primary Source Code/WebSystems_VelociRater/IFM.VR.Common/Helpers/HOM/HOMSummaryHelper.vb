Imports Diamond.Common.Objects.Policy
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.MultiState
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.HOM
    Public Class HOMSummaryHelper

        Private Enum AdditionalCoverageInformationType
            Address
            Description
            BusinessDescription
            BuildingDescription
            NameAndDescription
            AdditionalInterestNameAddress
            AdditionalInterestNameDescriptionAddress
            CoverageDateRange
        End Enum

        Private Enum SectionCoveragePropertyToUseForLimit
            IncreasedCostOfLoss
            IncreasedLimitId
            IncludedLimit
            TotalLimit
        End Enum

        Public Enum OptionalOrIncludedCoverages
            OptionalCoverages
            IncludedCoverages
        End Enum

        Public Enum SummaryType
            QuoteSummary
            PrintSummary
        End Enum

        Public Enum SectionIandIICoverageType
            PropertyCoverages
            LiabilityCoverages
            Both
        End Enum

        Public Shared Function FormatAddress(Address As QuickQuote.CommonObjects.QuickQuoteAddress) As String
            Dim zip As String = Address.Zip
            If zip.Length > 5 Then
                zip = zip.Substring(0, 5)
            End If
            Return String.Format("{0} {1} {2} {3} {4} {5} {6}", Address.HouseNum, Address.StreetName, If(String.IsNullOrWhiteSpace(Address.ApartmentNumber) = False, "Apt# " + Address.ApartmentNumber, ""), Address.POBox, Address.City, If(Address.StateId = "999" Or Address.StateId = "63", "NEED STATE", Address.State), zip).Replace("  ", " ").Trim()
        End Function

#Region "Screen Summary Logic"
        Public Shared Function GetIncludedCoverageList(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of SummaryCoverageLineItem)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            Dim QQHelper As New QuickQuoteHelperClass
            If QQHelper.doUseNewVersionOfLOB(topQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Dim opCovList As New List(Of SummaryCoverageLineItem)
                AddSectionICoveragesToList(topQuote, opCovList, OptionalOrIncludedCoverages.IncludedCoverages)
                AddSectionIICoveragesToList(topQuote, opCovList, OptionalOrIncludedCoverages.IncludedCoverages)
                AddSectionIAndIICoveragesToList(topQuote, opCovList, OptionalOrIncludedCoverages.IncludedCoverages)
                opCovList = GetSortedOpCovList(opCovList)
                Return opCovList
            Else
                Return GetIncludedCoverageListOld(topQuote)
            End If
        End Function

        Public Shared Function GetOptionalCoverageList(topQuote As QuickQuoteObject) As List(Of SummaryCoverageLineItem)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            Dim opCovList As New List(Of SummaryCoverageLineItem)
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.IsLoaded Then
            If topQuote IsNot Nothing AndAlso topQuote.Locations.IsLoaded Then
                Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()

                If QQHelper.doUseNewVersionOfLOB(topQuote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                    AddSectionICoveragesToList(topQuote, opCovList, OptionalOrIncludedCoverages.OptionalCoverages)
                    AddSectionIICoveragesToList(topQuote, opCovList, OptionalOrIncludedCoverages.OptionalCoverages)
                    AddSectionIAndIICoveragesToList(topQuote, opCovList, OptionalOrIncludedCoverages.OptionalCoverages)
                    ''Added 5/4/18 for HOM Upgrade for Bug 26572 MLW - if Occupancy Type is Secondary or Seasonal, need to force Loss Assessment to show
                    'If quote.Locations(0).OccupancyCodeId = "4" OrElse quote.Locations(0).OccupancyCodeId = "5" Then
                    '    'TODO: Dan - This should probably be pulling directly from Diamond for the limit... we should get away from hard coding these values.
                    '    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Loss Assessment", .Limit = "1,000", .Premium = "Included", .IsSubItem = False, .SectionCoverageType = GetType(QuickQuoteSectionIAndIICoverage), .IgnoreSort = False, .hasChildren = False, .SubItemId = 0, .ParentName = ""})
                    'End If
                Else
                    opCovList = GetOptionalCoverageListOld(topQuote).OrderBy(Function(x) x.Index).ToList()
                End If
            End If

            Return opCovList
        End Function

        Private Shared Sub AddCoverageToSummaryCoverageList(Of T)(ByVal quote As QuickQuoteObject, ByRef opCovList As List(Of SummaryCoverageLineItem), ByVal sectionCovType As Object, Optional ByVal isSubItem As Boolean = False, Optional ByVal SuperScriptText As String = "", Optional ByVal ForceLimitText As String = "", Optional ByVal ForcePremiumText As String = "", Optional ByVal ForceLimitLiabilityText As String = "", Optional ByVal ForceLimitPropertyText As String = "", Optional ByVal SectionCoveragePropertyToUseForLimit As SectionCoveragePropertyToUseForLimit = SectionCoveragePropertyToUseForLimit.TotalLimit, Optional CoverageInformationToAdd As List(Of AdditionalCoverageInformationType) = Nothing, Optional ByRef myCovItem As Object = Nothing, Optional SplitSectionIandIICoverage As Boolean = False, Optional ignoreSort As Boolean = False, Optional parentName As String = "", Optional subItemId As Integer = 0, Optional SummaryType As SummaryType = SummaryType.QuoteSummary, Optional SectionCoverageType As SectionIandIICoverageType = SectionIandIICoverageType.Both)
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.IsLoaded Then
            If quote IsNot Nothing AndAlso quote.Locations.IsLoaded Then
                If quote.Locations(0).SectionICoverages.IsLoaded OrElse quote.Locations(0).SectionIICoverages.IsLoaded OrElse quote.Locations(0).SectionIAndIICoverages.IsLoaded Then

                    'Set our myCov object equal to the SectionCoverageItem that we would like to add
                    If GetType(T) Is GetType(QuickQuoteSectionICoverage) AndAlso quote.Locations(0).SectionICoverages.IsLoaded Then
                        'Added 4/17/18 for bug 26078 MLW
                        If sectionCovType = QuickQuoteSectionICoverage.SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises Then
                            Dim myCovIndex As Int32 = 0
                            Dim isFirst As Boolean = False
                            For Each myCov In quote.Locations(0).SectionICoverages
                                If myCov.CoverageType = sectionCovType Then
                                    If myCovIndex = 0 Then
                                        isFirst = True
                                    Else
                                        isFirst = False
                                    End If
                                    AddCoverageInformationToCovList(Of QuickQuoteSectionICoverage)(myCov:=myCov, quote:=quote, opCovList:=opCovList, sectionCovType:=sectionCovType, isSubItem:=isSubItem, SuperScriptText:=SuperScriptText, ForceLimitText:=ForceLimitText, ForcePremiumText:=ForcePremiumText, ForceLimitLiabilityText:=ForceLimitLiabilityText, ForceLimitPropertyText:=ForceLimitPropertyText, SectionCoveragePropertyToUseForLimit:=SectionCoveragePropertyToUseForLimit, CoverageInformationToAdd:=CoverageInformationToAdd, myCovItem:=myCovItem, SplitSectionIandIICoverage:=SplitSectionIandIICoverage, ignoreSort:=ignoreSort, parentName:=parentName, subItemId:=subItemId, SummaryType:=SummaryType, isFirst:=isFirst, SectionCoverageType:=SectionCoverageType)
                                    myCovIndex = myCovIndex + 1
                                End If
                            Next
                        Else
                            Dim myCoverage As Object = Nothing
                            myCoverage = (From cov In quote.Locations(0).SectionICoverages Where cov.CoverageType = sectionCovType Select cov).FirstOrDefault()
                            If myCoverage IsNot Nothing Then
                                AddCoverageInformationToCovList(Of QuickQuoteSectionICoverage)(myCov:=myCoverage, quote:=quote, opCovList:=opCovList, sectionCovType:=sectionCovType, isSubItem:=isSubItem, SuperScriptText:=SuperScriptText, ForceLimitText:=ForceLimitText, ForcePremiumText:=ForcePremiumText, ForceLimitLiabilityText:=ForceLimitLiabilityText, ForceLimitPropertyText:=ForceLimitPropertyText, SectionCoveragePropertyToUseForLimit:=SectionCoveragePropertyToUseForLimit, CoverageInformationToAdd:=CoverageInformationToAdd, myCovItem:=myCovItem, SplitSectionIandIICoverage:=SplitSectionIandIICoverage, ignoreSort:=ignoreSort, parentName:=parentName, subItemId:=subItemId, SummaryType:=SummaryType, isFirst:=True, SectionCoverageType:=SectionCoverageType)
                            End If
                        End If

                    ElseIf GetType(T) Is GetType(QuickQuoteSectionIICoverage) AndAlso quote.Locations(0).SectionIICoverages.IsLoaded Then
                        'Added 4/17/18 for bug 26092 MLW
                        Select Case sectionCovType
                            Case QuickQuoteSectionIICoverage.SectionIICoverageType.OtherLocationOccupiedByInsured,
                                 QuickQuoteSectionIICoverage.SectionIICoverageType.AdditionalResidenceRentedToOther,
                                QuickQuoteSectionIICoverage.SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises,
                                 QuickQuoteSectionIICoverage.SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence,
                                 QuickQuoteSectionIICoverage.SectionIICoverageType.SpecialEventCoverage
                                Dim myCovIndex As Int32 = 0
                                Dim isFirst As Boolean = False
                                For Each myCov In quote.Locations(0).SectionIICoverages
                                    If myCov.CoverageType = sectionCovType Then
                                        If myCovIndex = 0 Then
                                            isFirst = True
                                        Else
                                            isFirst = False
                                        End If
                                        AddCoverageInformationToCovList(Of QuickQuoteSectionIICoverage)(myCov:=myCov, quote:=quote, opCovList:=opCovList, sectionCovType:=sectionCovType, isSubItem:=isSubItem, SuperScriptText:=SuperScriptText, ForceLimitText:=ForceLimitText, ForcePremiumText:=ForcePremiumText, ForceLimitLiabilityText:=ForceLimitLiabilityText, ForceLimitPropertyText:=ForceLimitPropertyText, SectionCoveragePropertyToUseForLimit:=SectionCoveragePropertyToUseForLimit, CoverageInformationToAdd:=CoverageInformationToAdd, myCovItem:=myCovItem, SplitSectionIandIICoverage:=SplitSectionIandIICoverage, ignoreSort:=ignoreSort, parentName:=parentName, subItemId:=subItemId, SummaryType:=SummaryType, isFirst:=isFirst, SectionCoverageType:=SectionCoverageType)
                                        myCovIndex = myCovIndex + 1
                                    End If
                                Next
                            Case Else
                                Dim myCoverage As Object = Nothing
                                myCoverage = (From cov In quote.Locations(0).SectionIICoverages Where cov.CoverageType = sectionCovType Select cov).FirstOrDefault()
                                If myCoverage IsNot Nothing Then
                                    AddCoverageInformationToCovList(Of QuickQuoteSectionIICoverage)(myCov:=myCoverage, quote:=quote, opCovList:=opCovList, sectionCovType:=sectionCovType, isSubItem:=isSubItem, SuperScriptText:=SuperScriptText, ForceLimitText:=ForceLimitText, ForcePremiumText:=ForcePremiumText, ForceLimitLiabilityText:=ForceLimitLiabilityText, ForceLimitPropertyText:=ForceLimitPropertyText, SectionCoveragePropertyToUseForLimit:=SectionCoveragePropertyToUseForLimit, CoverageInformationToAdd:=CoverageInformationToAdd, myCovItem:=myCovItem, SplitSectionIandIICoverage:=SplitSectionIandIICoverage, ignoreSort:=ignoreSort, parentName:=parentName, subItemId:=subItemId, SummaryType:=SummaryType, isFirst:=True, SectionCoverageType:=SectionCoverageType)
                                End If
                        End Select
                    ElseIf GetType(T) Is GetType(QuickQuoteSectionIAndIICoverage) AndAlso quote.Locations(0).SectionIAndIICoverages.IsLoaded Then
                        'Added 4/17/18 for bug 26182 MLW
                        Select Case sectionCovType
                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence
                                'added 4/30/18 for bug 26102 MLW
                                Dim myCovIndex As Int32 = 0
                                Dim isFirst As Boolean = False
                                For Each myCov In quote.Locations(0).SectionIAndIICoverages
                                    If myCov.MainCoverageType = sectionCovType Then
                                        If myCovIndex = 0 Then
                                            isFirst = True
                                        Else
                                            isFirst = False
                                        End If
                                        'updated 5/2/18 for Bug 26102 MLW
                                        AddCoverageInformationToCovList(Of QuickQuoteSectionIAndIICoverage)(myCov:=myCov, quote:=quote, opCovList:=opCovList, sectionCovType:=sectionCovType, isSubItem:=isSubItem, SuperScriptText:=SuperScriptText, ForceLimitText:=ForceLimitText, ForcePremiumText:=ForcePremiumText, ForceLimitLiabilityText:=ForceLimitLiabilityText, ForceLimitPropertyText:=ForceLimitPropertyText, SectionCoveragePropertyToUseForLimit:=SectionCoveragePropertyToUseForLimit, CoverageInformationToAdd:=CoverageInformationToAdd, myCovItem:=myCovItem, SplitSectionIandIICoverage:=SplitSectionIandIICoverage, ignoreSort:=ignoreSort, parentName:=parentName, subItemId:=subItemId, SummaryType:=SummaryType, isFirst:=isFirst, multipleCovIndex:=myCovIndex, SectionCoverageType:=SectionCoverageType)
                                        myCovIndex = myCovIndex + 1
                                    End If
                                Next
                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold
                                Dim myCovIndex As Int32 = 0
                                Dim isFirst As Boolean = False
                                For Each myCov In quote.Locations(0).SectionIAndIICoverages
                                    If myCov.MainCoverageType = sectionCovType Then
                                        If myCovIndex = 0 Then
                                            isFirst = True
                                        Else
                                            isFirst = False
                                        End If
                                        AddCoverageInformationToCovList(Of QuickQuoteSectionIAndIICoverage)(myCov:=myCov, quote:=quote, opCovList:=opCovList, sectionCovType:=sectionCovType, isSubItem:=isSubItem, SuperScriptText:=SuperScriptText, ForceLimitText:=ForceLimitText, ForcePremiumText:=ForcePremiumText, ForceLimitLiabilityText:=ForceLimitLiabilityText, ForceLimitPropertyText:=ForceLimitPropertyText, SectionCoveragePropertyToUseForLimit:=SectionCoveragePropertyToUseForLimit, CoverageInformationToAdd:=CoverageInformationToAdd, myCovItem:=myCovItem, SplitSectionIandIICoverage:=SplitSectionIandIICoverage, ignoreSort:=ignoreSort, parentName:=parentName, subItemId:=subItemId, SummaryType:=SummaryType, isFirst:=isFirst, SectionCoverageType:=SectionCoverageType)
                                        myCovIndex = myCovIndex + 1
                                    End If
                                Next
                            Case Else
                                Dim myCoverage As Object = Nothing
                                myCoverage = (From cov In quote.Locations(0).SectionIAndIICoverages Where cov.MainCoverageType = sectionCovType Select cov).FirstOrDefault()
                                If myCoverage IsNot Nothing Then
                                    AddCoverageInformationToCovList(Of QuickQuoteSectionIAndIICoverage)(myCov:=myCoverage, quote:=quote, opCovList:=opCovList, sectionCovType:=sectionCovType, isSubItem:=isSubItem, SuperScriptText:=SuperScriptText, ForceLimitText:=ForceLimitText, ForcePremiumText:=ForcePremiumText, ForceLimitLiabilityText:=ForceLimitLiabilityText, ForceLimitPropertyText:=ForceLimitPropertyText, SectionCoveragePropertyToUseForLimit:=SectionCoveragePropertyToUseForLimit, CoverageInformationToAdd:=CoverageInformationToAdd, myCovItem:=myCovItem, SplitSectionIandIICoverage:=SplitSectionIandIICoverage, ignoreSort:=ignoreSort, parentName:=parentName, subItemId:=subItemId, SummaryType:=SummaryType, isFirst:=True, SectionCoverageType:=SectionCoverageType)
                                End If
                        End Select
                    End If
                End If
            End If
        End Sub

        'Added 4/17/18 for bugs 26092, 26182, 26258
        Private Shared Sub AddCoverageInformationToCovList(Of T)(myCov As Object, ByVal quote As QuickQuoteObject, ByRef opCovList As List(Of SummaryCoverageLineItem), ByVal sectionCovType As Object, Optional ByVal isSubItem As Boolean = False, Optional ByVal SuperScriptText As String = "", Optional ByVal ForceLimitText As String = "", Optional ByVal ForcePremiumText As String = "", Optional ByVal ForceLimitLiabilityText As String = "", Optional ByVal ForceLimitPropertyText As String = "", Optional ByVal SectionCoveragePropertyToUseForLimit As SectionCoveragePropertyToUseForLimit = SectionCoveragePropertyToUseForLimit.TotalLimit, Optional CoverageInformationToAdd As List(Of AdditionalCoverageInformationType) = Nothing, Optional ByRef myCovItem As Object = Nothing, Optional SplitSectionIandIICoverage As Boolean = False, Optional ignoreSort As Boolean = False, Optional parentName As String = "", Optional subItemId As Integer = 0, Optional SummaryType As SummaryType = SummaryType.QuoteSummary, Optional isFirst As Boolean = False, Optional multipleCovIndex As Integer = 0, Optional SectionCoverageType As SectionIandIICoverageType = SectionIandIICoverageType.Both)
            myCovItem = Nothing
            Dim chc As New CommonHelperClass
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.IsLoaded Then
            If quote IsNot Nothing AndAlso quote.Locations.IsLoaded Then

                If quote.Locations(0).SectionICoverages.IsLoaded OrElse quote.Locations(0).SectionIICoverages.IsLoaded OrElse quote.Locations(0).SectionIAndIICoverages.IsLoaded Then

                    Dim qqh As New QuickQuoteHelperClass
                    'The different functions we will use to retrieve the Coverage Name from the Static Data File
                    Dim GetSecICoverageName = Function(sectionI_Enum As QuickQuoteSectionICoverage.SectionICoverageType) As String
                                                  Dim ccID As String = qqh.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, sectionI_Enum.ToString(), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
                                                  If SummaryType = SummaryType.QuoteSummary Then
                                                      If (sectionI_Enum = QuickQuoteSectionICoverage.SectionICoverageType.Firearms AndAlso IFM.VR.Common.Helpers.HOM.HOMTheftOfFirearmsIncrease.IsHOMTheftOfFirearmsIncreaseAvailable(quote)) OrElse (sectionI_Enum = QuickQuoteSectionICoverage.SectionICoverageType.JewelryWatchesAndFurs AndAlso IFM.VR.Common.Helpers.HOM.HOMTheftOfJewelryIncrease.IsHOMTheftOfJewelryIncreaseAvailable(quote)) Then
                                                          Return GetCorrectSectionICovStaticDataText(quote, sectionI_Enum, ccID, qqh)
                                                      Else
                                                          'if coverage not Firearms/Jewelry or flag turned off for Firearms/Jewelry, shows coverage name as <Text> tag in DiamondStaticData.xml - default
                                                          Return qqh.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, ccID)
                                                      End If
                                                  Else
                                                      If sectionI_Enum = QuickQuoteSectionICoverage.SectionICoverageType.BroadenedResidencePremisesDefinition Then
                                                          Dim covNameAndFormNumber As String
                                                          covNameAndFormNumber = "Broadened Residence Premises Definition"
                                                          If quote.Locations(0).FormTypeId = "26" Then
                                                              covNameAndFormNumber = covNameAndFormNumber & " (HO 1747)"
                                                          ElseIf quote.Locations(0).FormTypeId = "22" AndAlso quote.Locations(0).StructureTypeId = "2" Then
                                                              covNameAndFormNumber = covNameAndFormNumber & " (MH 0427)"
                                                          Else
                                                              covNameAndFormNumber = covNameAndFormNumber & " (HO 0469)"
                                                          End If
                                                          Return covNameAndFormNumber
                                                      ElseIf sectionI_Enum = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlement Then
                                                          Dim covNameAndFormNumber As String = "Actual Cash Value Loss Settlement"
                                                          If quote.Locations(0).FormTypeId = "22" AndAlso quote.Locations(0).StructureTypeId = "2" Then
                                                              covNameAndFormNumber = covNameAndFormNumber & " (MH 0402)"
                                                          Else
                                                              covNameAndFormNumber = covNameAndFormNumber & " (HO 0481)"
                                                          End If
                                                          Return covNameAndFormNumber
                                                      ElseIf sectionI_Enum = QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing Then
                                                          Dim covNameAndFormNumber As String = "Actual Cash Value Loss Settlement/Windstorm Or Hail Losses To Roof Surfacing"
                                                          If quote.Locations(0).FormTypeId = "22" AndAlso quote.Locations(0).StructureTypeId = "2" Then
                                                              covNameAndFormNumber = covNameAndFormNumber & " (MH 0425)"
                                                          Else
                                                              covNameAndFormNumber = covNameAndFormNumber & " (HOM 1013)"
                                                          End If
                                                          Return covNameAndFormNumber
                                                      ElseIf (sectionI_Enum = QuickQuoteSectionICoverage.SectionICoverageType.Firearms AndAlso IFM.VR.Common.Helpers.HOM.HOMTheftOfFirearmsIncrease.IsHOMTheftOfFirearmsIncreaseAvailable(quote)) OrElse (sectionI_Enum = QuickQuoteSectionICoverage.SectionICoverageType.JewelryWatchesAndFurs AndAlso IFM.VR.Common.Helpers.HOM.HOMTheftOfJewelryIncrease.IsHOMTheftOfJewelryIncreaseAvailable(quote)) Then
                                                          Return GetCorrectSectionICovStaticDataText(quote, sectionI_Enum, ccID, qqh)
                                                      Else
                                                          Return qqh.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, ccID, QuickQuoteHelperClass.QuickQuotePropertyName.Text2, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
                                                      End If
                                                  End If
                                              End Function
                    Dim GetSecIICoverageName = Function(sectionII_Enum As QuickQuoteSectionIICoverage.SectionIICoverageType) As String
                                                   Dim ccID As String = qqh.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageType, sectionII_Enum.ToString(), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
                                                   If SummaryType = SummaryType.QuoteSummary Then
                                                       Return qqh.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, ccID)
                                                   Else
                                                       Return qqh.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, ccID, QuickQuoteHelperClass.QuickQuotePropertyName.Text2, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
                                                   End If
                                               End Function
                    Dim GetSecIandIICoverageName = Function(sectionIandII_Enum As QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType) As String
                                                       Dim ccID As String = qqh.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageType, sectionIandII_Enum.ToString(), QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageCodeId, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
                                                       If SummaryType = SummaryType.QuoteSummary Then
                                                           Return qqh.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageCodeId, ccID)
                                                       Else
                                                           Return qqh.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageCodeId, ccID, QuickQuoteHelperClass.QuickQuotePropertyName.Text2, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
                                                       End If
                                                   End Function

                    If myCov IsNot Nothing Then
                        myCovItem = myCov 'Set the myCovItem variable to something. We set it to nothing earlier so this should be all we need now.

                        Dim myPrem As String = ""
                        Dim myLim As String = ""

                        'If SuperScript was specified as an attribute, lets get it setup with the superscript HTML tags
                        If String.IsNullOrWhiteSpace(SuperScriptText) = False Then
                            SuperScriptText = "<sup>" & SuperScriptText & "</sup>"
                        End If

                        'See if we are forcing the limit text to be something.
                        If String.IsNullOrWhiteSpace(ForceLimitText) = False Then
                            If SummaryType = SummaryType.QuoteSummary Then
                                'If we are making the Limit text "see" or "see below" lets add paranthesis around it. 
                                '"see" only has parentheses in summary, not print - updated 4/4/18 MLW
                                If ForceLimitText.Equals("see", StringComparison.OrdinalIgnoreCase) OrElse ForceLimitText.Equals("see below", StringComparison.OrdinalIgnoreCase) Then
                                    If SuperScriptText.IsNullEmptyorWhitespace() = False Then
                                        myLim = "(" & ForceLimitText & SuperScriptText & ")"
                                    Else
                                        myLim = "(" & ForceLimitText & ")"
                                    End If
                                Else
                                    If ForceLimitText.Equals("Included", StringComparison.OrdinalIgnoreCase) Then
                                        If SuperScriptText.IsNullEmptyorWhitespace() = False Then
                                            myLim = ForceLimitText & SuperScriptText
                                        Else
                                            myLim = ForceLimitText
                                        End If
                                    Else
                                        myLim = ForceLimitText
                                    End If

                                End If
                            Else
                                'If we are making the Limit text "see below" lets add paranthesis around it.
                                '"see" only has parentheses in summary, not print - updated 4/4/18 MLW
                                If ForceLimitText.Equals("see below", StringComparison.OrdinalIgnoreCase) Then
                                    If SuperScriptText.IsNullEmptyorWhitespace() = False Then
                                        myLim = "(" & ForceLimitText & SuperScriptText & ")"
                                    Else
                                        myLim = "(" & ForceLimitText & ")"
                                    End If
                                Else
                                    If ForceLimitText.Equals("see", StringComparison.OrdinalIgnoreCase) OrElse ForceLimitText.Equals("Included", StringComparison.OrdinalIgnoreCase) Then
                                        If SuperScriptText.IsNullEmptyorWhitespace() = False Then
                                            myLim = ForceLimitText & SuperScriptText
                                        Else
                                            myLim = ForceLimitText
                                        End If
                                    Else
                                        myLim = ForceLimitText
                                    End If

                                End If
                            End If

                        End If

                        'See if we are forcing the Premium text to be something.
                        If String.IsNullOrWhiteSpace(ForcePremiumText) = False Then
                            myPrem = ForcePremiumText
                        End If

                        'If the premium text has not been set to something, lets pull in the premium value from the myCov object. If it is nothing or zero, set it to "Included"
                        If String.IsNullOrWhiteSpace(myPrem) Then
                            If DirectCast(myCov.Premium, String).EqualsAny("", "0") OrElse (IsNumeric(myCov.Premium) AndAlso CDec(myCov.Premium) = 0) Then
                                'Loss Assessment - Earthquake - when premium is 0, show N/A
                                myPrem = "Included"
                                If GetType(T) Is GetType(QuickQuoteSectionICoverage) Then
                                    If myCov.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.LossAssessment_Earthquake Then
                                        myPrem = "N/A"
                                    End If
                                End If
                            Else
                                myPrem = myCov.Premium
                            End If
                        End If

                        If GetType(T) Is GetType(QuickQuoteSectionIAndIICoverage) Then
                            Dim myLimProp As String = ""
                            Dim myLimLia As String = ""

                            If String.IsNullOrWhiteSpace(ForceLimitLiabilityText) = False Then
                                myLimLia = ForceLimitLiabilityText
                            End If

                            If String.IsNullOrWhiteSpace(ForceLimitPropertyText) = False Then
                                myLimProp = ForceLimitPropertyText
                            End If

                            'If the limit text has not been set to something, lets pull in the limit value from the myCov object. Since this is sectionIandII, there is extra logic to get Property and Liability limits.
                            If String.IsNullOrWhiteSpace(myLim) AndAlso String.IsNullOrWhiteSpace(myLimProp) AndAlso String.IsNullOrWhiteSpace(myLimLia) Then
                                GetCoverageLimitForSectionIAndII(myLimProp, myLimLia, myCov, SectionCoveragePropertyToUseForLimit)
                                If SuperScriptText.IsNullEmptyorWhitespace = False Then
                                    If myLimProp.Equals("N/A", StringComparison.OrdinalIgnoreCase) Then
                                        'Added 4/18/18 for bug 26182 MLW
                                        If myCov.MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold Then
                                            myLimProp = "N/A"
                                        Else
                                            myLimProp = "Included" & SuperScriptText
                                        End If
                                    End If

                                    If myLimLia.Equals("N/A", StringComparison.OrdinalIgnoreCase) Then
                                        'updated 4/10/18 to allow PIORP_OtherStructures limit "see" MLW - updated 4/18/18 with Other Members Limit MLW
                                        Select Case myCov.MainCoverageType
                                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                                                myLimLia = "see" & SuperScriptText
                                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold
                                                myLimLia = "Included"
                                                'Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement
                                                '    If SummaryType = SummaryType.PrintSummary Then
                                                '        myLimLia = "see<sup>4</sup>"
                                                '    End If
                                            Case Else
                                                myLimLia = "Included" & SuperScriptText
                                        End Select
                                    End If
                                End If

                                myLim = "0"
                                If IsNumeric(myLimProp) AndAlso CDec(myLimProp) > 0 Then
                                    myLim = (CDec(myLim) + CDec(myLimProp)).ToString()
                                End If

                                If IsNumeric(myLimLia) AndAlso CDec(myLimLia) > 0 Then
                                    myLim = (CDec(myLim) + CDec(myLimLia)).ToString()
                                End If
                            Else
                                If String.IsNullOrWhiteSpace(myLimLia) Then
                                    myLimLia = myLim
                                End If

                                If String.IsNullOrWhiteSpace(myLimProp) Then
                                    myLimProp = myLim
                                End If
                            End If

                            If IsNumeric(myLim) Then
                                qqh.ConvertToLimitFormat(myLim)
                            End If

                            If IsNumeric(myLimProp) Then
                                qqh.ConvertToLimitFormat(myLimProp)
                            End If

                            If IsNumeric(myLimLia) Then
                                qqh.ConvertToLimitFormat(myLimLia)
                            End If

                            If IsNumeric(myPrem) Then
                                qqh.DiamondAmountFormat(myPrem)
                            End If

                            If SplitSectionIandIICoverage = True Then
                                'Updated 4/10/18 for HOM Upgrade MLW
                                If myCov.MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures Then
                                    'special case for Permitted Incidental Occupancies Residence Premises - updated 4/10/18 MLW 
                                    'split one for the business coverage and one for the building coverage - called Other Structures
                                    'since this coverage has both descriptions in one field, the description field will need to be split as well
                                    If CoverageInformationToAdd IsNot Nothing AndAlso CoverageInformationToAdd.Count > 0 Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecIandIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)), .Limit = "(see below)", .Premium = "(see below)", .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionIICoverage), .IgnoreSort = ignoreSort, .hasChildren = True, .SubItemId = subItemId, .ParentName = parentName})
                                        AddAdditionalCoverageInformationToCovList(opCovList, CoverageInformationToAdd, myCov, myLimLia, "Included")
                                        'only add if the limit and description are present - i.e. Other Structures was checked
                                        If String.IsNullOrWhiteSpace(myLimProp) = False AndAlso chc.NumericStringComparison(myLimProp, CommonHelperClass.ComparisonOperators.NotEqual, 0) AndAlso myLimProp <> "Included" & SuperScriptText Then
                                            CoverageInformationToAdd.Remove(AdditionalCoverageInformationType.BusinessDescription)
                                            CoverageInformationToAdd.Add(AdditionalCoverageInformationType.BuildingDescription)
                                            If SummaryType = SummaryType.QuoteSummary Then
                                                opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecIandIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)) & " - Other Structures", .Limit = "(see below)", .Premium = "(see below)", .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionICoverage), .IgnoreSort = ignoreSort, .hasChildren = True, .SubItemId = subItemId, .ParentName = parentName})
                                            Else
                                                opCovList.Add(New SummaryCoverageLineItem With {.Name = "Permitted Incidental Occupancies Residence Premises - Other Structures (HO 0442)", .Limit = "(see below)", .Premium = "(see below)", .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionICoverage), .IgnoreSort = ignoreSort, .hasChildren = True, .SubItemId = subItemId, .ParentName = parentName})
                                            End If
                                            AddAdditionalCoverageInformationToCovList(opCovList, CoverageInformationToAdd, myCov, myLimProp, myPrem)
                                        End If
                                    End If
                                Else
                                    'Since we are splitting the coverage, we will make a line for the Property and a line for the liability.
                                    If CoverageInformationToAdd IsNot Nothing AndAlso CoverageInformationToAdd.Count > 0 Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecIandIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)) & " (Property)", .Limit = "(see below)", .Premium = "(see below)", .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionICoverage), .IgnoreSort = ignoreSort, .hasChildren = True, .SubItemId = subItemId, .ParentName = parentName})
                                        AddAdditionalCoverageInformationToCovList(opCovList, CoverageInformationToAdd, myCov, myLimProp, myPrem)
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecIandIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)) & " (Liability)", .Limit = "(see below)", .Premium = "(see below)", .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionIICoverage), .IgnoreSort = ignoreSort, .hasChildren = True, .SubItemId = subItemId, .ParentName = parentName})
                                        AddAdditionalCoverageInformationToCovList(opCovList, CoverageInformationToAdd, myCov, myLimLia, "Included") 'We only want to show the premium on the Property side
                                    Else
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecIandIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)) & " (Property)", .Limit = myLimProp, .Premium = myPrem, .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionICoverage), .IgnoreSort = ignoreSort, .hasChildren = False, .SubItemId = subItemId, .ParentName = parentName})
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecIandIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)) & " (Liability)", .Limit = myLimLia, .Premium = myPrem, .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionIICoverage), .IgnoreSort = ignoreSort, .hasChildren = False, .SubItemId = subItemId, .ParentName = parentName})
                                    End If
                                End If
                            Else
                                If CoverageInformationToAdd IsNot Nothing AndAlso CoverageInformationToAdd.Count > 0 Then
                                    'Updated 4/17/18 for bug 26182 MLW
                                    If isFirst = True Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecIandIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)), .Limit = "(see below)", .Premium = "(see below)", .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionIAndIICoverage), .IgnoreSort = ignoreSort, .hasChildren = True, .SubItemId = subItemId, .ParentName = parentName})
                                    End If
                                    'Updated 5/2/18 for Bug 26102 MLW
                                    If SectionCoverageType = SectionIandIICoverageType.Both Then
                                        AddAdditionalCoverageInformationToCovList(opCovList, CoverageInformationToAdd, myCov, myLim, myPrem, multipleCovIndex)
                                    Else
                                        If SectionCoverageType = SectionIandIICoverageType.LiabilityCoverages Then
                                            AddAdditionalCoverageInformationToCovList(opCovList, CoverageInformationToAdd, myCov, myLimLia, myPrem, multipleCovIndex)
                                        ElseIf SectionCoverageType = SectionIandIICoverageType.PropertyCoverages Then
                                            AddAdditionalCoverageInformationToCovList(opCovList, CoverageInformationToAdd, myCov, myLimProp, myPrem, multipleCovIndex)
                                        End If
                                    End If
                                Else
                                    If SectionCoverageType = SectionIandIICoverageType.Both Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecIandIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)), .Limit = myLim, .Premium = myPrem, .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionIAndIICoverage), .IgnoreSort = ignoreSort, .hasChildren = False, .SubItemId = subItemId, .ParentName = parentName})
                                    Else
                                        If SectionCoverageType = SectionIandIICoverageType.LiabilityCoverages Then
                                            opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecIandIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)), .Limit = myLimLia, .Premium = myPrem, .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionIAndIICoverage), .IgnoreSort = ignoreSort, .hasChildren = False, .SubItemId = subItemId, .ParentName = parentName})
                                        ElseIf SectionCoverageType = SectionIandIICoverageType.PropertyCoverages Then
                                            opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecIandIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)), .Limit = myLimProp, .Premium = myPrem, .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionIAndIICoverage), .IgnoreSort = ignoreSort, .hasChildren = False, .SubItemId = subItemId, .ParentName = parentName})
                                        End If
                                    End If
                                End If
                            End If
                        Else 'Section I or Section II coverages
                            If String.IsNullOrWhiteSpace(myLim) Then
                                GetCoverageLimitForSectionIOrSectionII(myLim, myCov, SectionCoveragePropertyToUseForLimit)
                            End If

                            If IsNumeric(myLim) Then
                                qqh.ConvertToLimitFormat(myLim)
                            End If

                            ' Add superscript outside of the forced limits code
                            If SuperScriptText IsNot Nothing AndAlso SuperScriptText <> "" AndAlso mylim.Contains(SuperScriptText) = False Then
                                myLim = myLim + SuperScriptText
                            End If

                            If IsNumeric(myPrem) Then
                                qqh.DiamondAmountFormat(myPrem)
                            End If

                            If GetType(T) Is GetType(QuickQuoteSectionICoverage) Then
                                If CoverageInformationToAdd IsNot Nothing AndAlso CoverageInformationToAdd.Count > 0 Then
                                    'Updated 4/17/18 for bug 26078 MLW
                                    If isFirst = True Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecICoverageName(DirectCast(sectionCovType, QuickQuoteSectionICoverage.SectionICoverageType)), .Limit = "(see below)", .Premium = "(see below)", .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionICoverage), .IgnoreSort = ignoreSort, .hasChildren = True, .SubItemId = subItemId, .ParentName = parentName})
                                    End If
                                    AddAdditionalCoverageInformationToCovList(opCovList, CoverageInformationToAdd, myCov, myLim, myPrem)
                                Else
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecICoverageName(DirectCast(sectionCovType, QuickQuoteSectionICoverage.SectionICoverageType)), .Limit = myLim, .Premium = myPrem, .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionICoverage), .IgnoreSort = ignoreSort, .hasChildren = False, .SubItemId = subItemId, .ParentName = parentName})
                                End If
                            ElseIf GetType(T) Is GetType(QuickQuoteSectionIICoverage) Then
                                If CoverageInformationToAdd IsNot Nothing AndAlso CoverageInformationToAdd.Count > 0 Then
                                    'Updated 4/17/18 for bug 26092 MLW
                                    If isFirst = True Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIICoverage.SectionIICoverageType)), .Limit = "(see below)", .Premium = "(see below)", .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionIICoverage), .IgnoreSort = ignoreSort, .hasChildren = True, .SubItemId = subItemId, .ParentName = parentName})
                                    End If
                                    AddAdditionalCoverageInformationToCovList(opCovList, CoverageInformationToAdd, myCov, myLim, myPrem)
                                Else
                                    Dim nm As String = GetSecIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIICoverage.SectionIICoverageType))
                                    If nm.ToUpper().Contains("SPECIAL EVENT COVERAGE") Then
                                        Dim s2Cov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage = DirectCast(myCov, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)
                                        Dim dsc As String = s2Cov.Description
                                        ' Trim the description to 20 characters
                                        If dsc.Length > 20 Then dsc = dsc.Substring(0, 17) & "..."
                                        If SummaryType = SummaryType.QuoteSummary Then
                                            nm = "Special Event Coverage - " & dsc
                                        Else
                                            ' Print summary - show form number
                                            nm = "Special Event Coverage (HOM 1005) - " & dsc
                                        End If
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = nm, .Limit = myLim, .Premium = myPrem, .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionIICoverage), .IgnoreSort = ignoreSort, .hasChildren = False, .SubItemId = subItemId, .ParentName = parentName})
                                    'opCovList.Add(New SummaryCoverageLineItem With {.Name = GetSecIICoverageName(DirectCast(sectionCovType, QuickQuoteSectionIICoverage.SectionIICoverageType)), .Limit = myLim, .Premium = myPrem, .IsSubItem = isSubItem, .SectionCoverageType = GetType(QuickQuoteSectionIICoverage), .IgnoreSort = ignoreSort, .hasChildren = False, .SubItemId = subItemId, .ParentName = parentName})
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End Sub

        Private Shared Function GetCorrectSectionICovStaticDataText(quote As QuickQuoteObject, sectionI_Enum As QuickQuoteSectionICoverage.SectionICoverageType, ccId As String, qqh As QuickQuoteHelperClass) As String
            'This function shows <Text2> for increased limit and <Text> without increased limit from DiamondStaticData for both summary and print
            Dim myICov As QuickQuoteSectionICoverage = quote.Locations(0).SectionICoverages.Find(Function(p) p.CoverageType = sectionI_Enum)
            If myICov IsNot Nothing AndAlso myICov.IncreasedLimit.NotEqualsAny("", "0") Then
                'If firearms has increased limit, show coverage name as "Theft of Firearms - Special Limits of Liability" in bottom optional coverages section (<Text2> tag in DiamondStaticData.xml)
                'If jewelry has increased limit, show coverage name as "Theft of Jewelry, Watches, Furs & Precious Stones - Special Limits of Liability" in bottom optional coverages section (<Text2> tag in DiamondStaticData.xml)
                Return qqh.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, ccId, QuickQuoteHelperClass.QuickQuotePropertyName.Text2, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
            Else
                'If firearms does not have an increased limit, show coverage name as "Firearms" in top included coverage section (<Text> tag in DiamondStaticData.xml)
                'If jewerly does not have an increased limit, show coverage name as "Jewelry, Watches & Furs" in top included coverage section (<Text> tag in DiamondStaticData.xml)
                Return qqh.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, ccId)
            End If
        End Function

        Private Shared Sub AddAdditionalCoverageInformationToCovList(opCovList As List(Of SummaryCoverageLineItem), CoverageInformationToAdd As List(Of AdditionalCoverageInformationType), myCov As Object, myLim As String, myPrem As String, Optional multipleCovIndex As Integer = 0, Optional myLimLiability As String = "", Optional myLimProperty As String = "")
            If CoverageInformationToAdd IsNot Nothing AndAlso CoverageInformationToAdd.Count > 0 Then
                Dim counter As Integer = 1
                Dim coverageParentName As String = ""
                If opCovList IsNot Nothing AndAlso opCovList.Count > 0 Then
                    'Added 4/27/18 for Bugs 26258, 26092, and 26182 MLW 'Updated 4/30/18 and 5/1/18 for Bug 26102 MLW
                    Select Case opCovList.Last.ParentName
                        Case "Specific Structures Away from Residence Premises (HO 0492)", "Other Insured Location Occupied by Insured", "Additional Residence - Occupied by Insured (N/A)", "Additional Residence - Rented to Others (HO 2470)",
                             "Incidental Farming Personal Liability - Off Premises (HO 2472)", "Permitted Incidental Occupancies Other Residence (HO 2443)", "Other Members of Your Household (HO 0458)",
                             "Specific Structures Away from Residence Premises", "Additional Residence - Occupied by Insured", "Additional Residence - Rented to Others",
                             "Incidental Farming Personal Liability - Off Premises", "Permitted Incidental Occupancies Other Residence", "Other Members of Your Household"
                            coverageParentName = opCovList.Last.ParentName
                        Case "Additional Insured - Student Living Away from Residence (HO 0527)", "Additional Insured - Student Living Away from Residence"
                            'Added 5/2/18 for Bug 26102 MLW
                            coverageParentName = opCovList.Last.ParentName
                            counter = multipleCovIndex & counter
                        Case Else
                            coverageParentName = opCovList.Last.Name
                    End Select
                    'coverageParentName = opCovList.Last.Name
                End If
                Dim first As Boolean = True
                For Each infoToAdd As AdditionalCoverageInformationType In CoverageInformationToAdd
                    Select Case infoToAdd
                        Case AdditionalCoverageInformationType.Address
                            If myCov.Address IsNot Nothing Then
                                opCovList.Add(New SummaryCoverageLineItem With {.Name = FormatAddress(myCov.Address), .Limit = myLim, .Premium = myPrem, .IsSubItem = True, .ParentName = coverageParentName, .SubItemId = counter})
                                counter += 1
                            End If
                        Case AdditionalCoverageInformationType.CoverageDateRange
                            If String.IsNullOrWhiteSpace(myCov.EventEffDate) = False AndAlso String.IsNullOrWhiteSpace(myCov.EventExpDate) = False AndAlso IsDate(myCov.EventEffDate) AndAlso IsDate(myCov.EventExpDate) Then
                                opCovList.Add(New SummaryCoverageLineItem With {.Name = CDate(myCov.EventEffDate).ToShortDateString & " - " & CDate(myCov.EventExpDate).ToShortDateString, .Limit = myLim, .Premium = myPrem, .IsSubItem = True, .ParentName = coverageParentName, .SubItemId = counter})
                                counter += 1
                            End If
                        Case AdditionalCoverageInformationType.BusinessDescription
                            'Business description and building description both use the description field
                            Dim txtDescr As String = myCov.Description
                            Dim txtBusinessDescr As String = Nothing
                            Dim txtBuildingDescr As String = Nothing
                            If txtDescr.Contains(vbNewLine) Then
                                Dim txtDescrSplit As Array = Split(txtDescr, vbNewLine)
                                txtBusinessDescr = txtDescrSplit(0)
                                txtBuildingDescr = txtDescrSplit(1)
                            Else
                                txtBusinessDescr = txtDescr
                                txtBuildingDescr = ""
                            End If
                            opCovList.Add(New SummaryCoverageLineItem With {.Name = txtBusinessDescr, .Limit = myLim, .Premium = myPrem, .IsSubItem = True, .ParentName = coverageParentName, .SubItemId = counter})
                            counter += 1
                        Case AdditionalCoverageInformationType.BuildingDescription
                            'Business description and building description both use the description field
                            Dim txtDescr As String = myCov.Description
                            Dim txtBusinessDescr As String = Nothing
                            Dim txtBuildingDescr As String = Nothing
                            If txtDescr.Contains(vbNewLine) Then
                                Dim txtDescrSplit As Array = Split(txtDescr, vbNewLine)
                                txtBusinessDescr = txtDescrSplit(0)
                                txtBuildingDescr = txtDescrSplit(1)
                            Else
                                txtBusinessDescr = txtDescr
                                txtBuildingDescr = ""
                            End If
                            If coverageParentName.Contains("Permitted Incidental Occupancies Residence Premises - Other Structures") Then
                                opCovList.Add(New SummaryCoverageLineItem With {.Name = txtBuildingDescr, .Limit = myLim, .Premium = myCov.Premium, .IsSubItem = True, .ParentName = coverageParentName, .SubItemId = counter})
                            Else
                                opCovList.Add(New SummaryCoverageLineItem With {.Name = txtBuildingDescr, .Limit = myLim, .Premium = myPrem, .IsSubItem = True, .ParentName = coverageParentName, .SubItemId = counter})
                            End If
                            counter += 1
                        Case AdditionalCoverageInformationType.Description
                            opCovList.Add(New SummaryCoverageLineItem With {.Name = myCov.Description, .Limit = myLim, .Premium = myPrem, .IsSubItem = True, .ParentName = coverageParentName, .SubItemId = counter})
                            counter += 1
                        Case AdditionalCoverageInformationType.NameAndDescription
                            If myCov.Name IsNot Nothing Then
                                'opCovList.Add(New SummaryCoverageLineItem With {.Name = ReturnCommercialName(myCov.Name.CommercialName1) & " - " & myCov.Description, .IsSubItem = True, .ParentName = coverageParentName, .SubItemId = counter})
                                opCovList.Add(New SummaryCoverageLineItem With {.Name = myCov.Description & " - " & ReturnCommercialName(myCov.Name.DisplayName), .Limit = myLim, .Premium = myPrem, .IsSubItem = True, .ParentName = coverageParentName, .SubItemId = counter})
                                counter += 1
                            End If
                        Case AdditionalCoverageInformationType.AdditionalInterestNameAddress
                            If myCov.AdditionalInterests IsNot Nothing AndAlso myCov.AdditionalInterests.Count > 0 Then
                                For Each ai As QuickQuoteAdditionalInterest In DirectCast(myCov.AdditionalInterests, List(Of QuickQuoteAdditionalInterest)).OrderBy(Function(x) x.TypeId) 'Sorting so that Trust always comes before Trustee
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = ReturnCommercialName(ai.Name.CommercialName1), .IsSubItem = True, .ParentName = coverageParentName, .SubItemId = counter})
                                    counter += 1
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = FormatAddress(ai.Address), .Limit = myLim, .Premium = myPrem, .IsSubItem = True, .ParentName = coverageParentName, .SubItemId = counter})
                                    counter += 1
                                    If first = True Then
                                        first = False
                                        myPrem = "Included"
                                    End If
                                Next
                            End If
                        Case AdditionalCoverageInformationType.AdditionalInterestNameDescriptionAddress
                            If myCov.AdditionalInterests IsNot Nothing AndAlso myCov.AdditionalInterests.Count > 0 Then
                                For Each ai As QuickQuoteAdditionalInterest In myCov.AdditionalInterests
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = ReturnCommercialName(ai.Name.CommercialName1) & " - " & ai.Description, .IsSubItem = True, .ParentName = coverageParentName, .SubItemId = counter})
                                    counter += 1
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = FormatAddress(ai.Address), .Limit = myLim, .Premium = myPrem, .IsSubItem = True, .ParentName = coverageParentName, .SubItemId = counter})
                                    counter += 1
                                    If first = True Then
                                        first = False
                                        myPrem = "Included"
                                    End If
                                Next
                            End If
                    End Select
                Next
            End If
        End Sub

        Private Shared Sub AddBlankCoverages(Of T)(ByVal quote As QuickQuoteObject, ByRef opCovList As List(Of SummaryCoverageLineItem))
            If quote IsNot Nothing AndAlso quote.Locations.IsLoaded Then
                If GetType(T) Is GetType(QuickQuoteSectionICoverage) Then
                    If quote.Locations(0).SectionICoverages.IsLoaded Then
                        Dim myList As New List(Of QuickQuoteSectionICoverage)
                        myList = quote.Locations(0).SectionICoverages.FindAll(Function(x) x.CoverageType = QuickQuoteSectionICoverage.SectionICoverageType.None)
                        For Each cov As QuickQuoteSectionICoverage In myList
                            opCovList.Add(New SummaryCoverageLineItem With {.Name = "Blank Coverage", .Limit = cov.CoverageCodeId, .Premium = ""})
                        Next
                    End If
                ElseIf GetType(T) Is GetType(QuickQuoteSectionIICoverage) Then
                    If quote.Locations(0).SectionIICoverages.IsLoaded Then
                        Dim myList As New List(Of QuickQuoteSectionIICoverage)
                        myList = quote.Locations(0).SectionIICoverages.FindAll(Function(x) x.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.None)
                        For Each cov As QuickQuoteSectionIICoverage In myList
                            opCovList.Add(New SummaryCoverageLineItem With {.Name = "Blank Coverage", .Limit = cov.CoverageCodeId, .Premium = ""})
                        Next
                    End If
                ElseIf GetType(T) Is GetType(QuickQuoteSectionIAndIICoverage) Then
                    If quote.Locations(0).SectionIAndIICoverages.IsLoaded Then
                        Dim myList As New List(Of QuickQuoteSectionIAndIICoverage)
                        myList = quote.Locations(0).SectionIAndIICoverages.FindAll(Function(x) x.MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None)
                        For Each cov As QuickQuoteSectionIAndIICoverage In myList
                            opCovList.Add(New SummaryCoverageLineItem With {.Name = "Blank Coverage", .Limit = cov.MainCoverageCodeId, .Premium = ""})
                        Next
                    End If
                End If
            End If
        End Sub

        Private Shared Sub GetCoverageLimitForSectionIAndII(ByRef myLimProperty As String, ByRef myLimLiability As String, myCov As Object, SectionCoveragePropertyToUseForLimit As SectionCoveragePropertyToUseForLimit)
            Dim qqh As New QuickQuoteHelperClass
            Select Case SectionCoveragePropertyToUseForLimit
                Case SectionCoveragePropertyToUseForLimit.TotalLimit
                    myLimProperty = myCov.PropertyTotalLimit
                    myLimLiability = myCov.LiabilityTotalLimit
                Case SectionCoveragePropertyToUseForLimit.IncreasedLimitId
                    'myLim = qqh.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, myCov.IncreasedLimitId, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
                Case SectionCoveragePropertyToUseForLimit.IncreasedCostOfLoss
                    'myLim = myCov.IncreasedCostOfLoss
                Case SectionCoveragePropertyToUseForLimit.IncludedLimit
                    'myLim = myCov.IncludedLimit
            End Select

            If myLimProperty.EqualsAny("", "0") OrElse (IsNumeric(myLimProperty) AndAlso CDec(myLimProperty) = 0) Then
                myLimProperty = "N/A"
            End If

            If myLimLiability.EqualsAny("", "0") OrElse (IsNumeric(myLimLiability) AndAlso CDec(myLimLiability) = 0) Then
                myLimLiability = "N/A"
            End If
        End Sub

        Private Shared Sub GetCoverageLimitForSectionIOrSectionII(ByRef myLim As String, myCov As Object, SectionCoveragePropertyToUseForLimit As SectionCoveragePropertyToUseForLimit)
            Dim qqh As New QuickQuoteHelperClass
            Select Case SectionCoveragePropertyToUseForLimit
                Case SectionCoveragePropertyToUseForLimit.TotalLimit
                    myLim = myCov.TotalLimit
                Case SectionCoveragePropertyToUseForLimit.IncreasedLimitId
                    myLim = qqh.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, myCov.IncreasedLimitId, QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteHelperClass.PersOrComm.Pers)
                Case SectionCoveragePropertyToUseForLimit.IncreasedCostOfLoss
                    myLim = myCov.IncreasedCostOfLoss
                Case SectionCoveragePropertyToUseForLimit.IncludedLimit
                    myLim = myCov.IncludedLimit
            End Select

            If myLim.EqualsAny("", "0") OrElse (IsNumeric(myLim) AndAlso CDec(myLim) = 0) Then
                myLim = "N/A"
            End If
        End Sub

        Private Shared Function ReturnCommercialName(commName As String) As String
            Dim returnVar As String = ""

            If String.IsNullOrWhiteSpace(commName) = False Then
                'If commName.Contains("|") Then
                '    returnVar = commName.Replace("|||", " ").Replace("||", " ").Replace("|", " ").Trim() 'The trustee AI code is using pipes to seperate first, middle, last names and suffix.
                'Else
                '    returnVar = commName
                'End If
                returnVar = commName
            End If

            Return returnVar
        End Function

        Public Shared Sub AddSectionICoveragesToList(quote As QuickQuoteObject, opCovList As List(Of SummaryCoverageLineItem), ByVal OptionalOrIncluded As OptionalOrIncludedCoverages, Optional SummaryType As SummaryType = SummaryType.QuoteSummary)
            Dim CyberEffDate As DateTime = Nothing
            If System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate") IsNot Nothing _
            AndAlso IsDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then
                CyberEffDate = CDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate"))
            Else
                CyberEffDate = CDate("9/1/2020")
            End If

            If quote IsNot Nothing Then
                If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            Dim myCovInfoToAdd As New List(Of AdditionalCoverageInformationType) 'Allows you to add information to the quote/print summary related to a particular coverage. Address, Name, etc.
            Dim Limit As String = "" 'Use this to override the logic that would determine what to use for the limit field.
            Dim Premium As String = "" 'Use this to override the logic that would determine what to use for the premium field.
            Dim SuperScript As String = "" 'Use this to add text as superscript to the limit field - Currently there is no need for superscript in the premium field
            Dim myCov As Object = Nothing 'a generic object that will be used to get the current sectioncoverage object. It will return as an object of type QuickQuoteSectionICoverage / QuickQuoteSectionIICoverage / QuickQuoteSectionIandIICoverage
            Dim myICov As QuickQuoteSectionICoverage = Nothing 'This will be a copy of the myCov object but as the current QuickQuoteSection type (This one is QuickQuoteSectionICoverage) - It will not get reset right away and can be used in the code below for special circumstances
            Dim isSubItem As Boolean = False 'Boolean to determine if the current coverage should be indented.
            Dim sectionCoveragePropertyToUseForLimit As SectionCoveragePropertyToUseForLimit = SectionCoveragePropertyToUseForLimit.TotalLimit 'Determines what value to pull from for the limit. Most cases we will use TotalLimit but sometimes we use something else
            Dim ignoreSort As Boolean = False 'Currently only used for printer friendly version - Most coverages are sorted alphabetically but a few coverages are not. This will allow these coverages to not get sorted. Consequently, the current code will push these to the top of the list (which is desired currently) - Alphabetical sort will begin immediately after.
            Dim parentName As String = "" 'Denotes the parentName of the "SubItem". This is used in the print friendly sort to keep child items together. - Try to use the SetSubItemAndCounter Sub instead of this directly.
            Dim subItemId As Integer = 0 'This allows the "SubItems" to be sorted correctly - Try to use the SetSubItemAndCounter Sub instead of this directly.
            Dim counter As Integer = 1 'The counter used to count up the subItemId variable - Try to use the SetSubItemAndCounter Sub instead of this directly. - If manually specifying multiple sub items with different parents you may need to reset this variable manually between parents.
            'Updated 5/10/2018 for Bug 26645 MLW
            'Dim A_Dwelling_Limit As String = "" 'used for Mine A&B limit and Mine A limit eval to show on the summary display and print MLW
            Dim A_Dwelling_Limit As Integer = 0 'used for Mine A&B limit and Mine A limit eval to show on the summary display and print MLW
            Dim IsHO4 As Boolean = False
            Dim IsMobileHome As Boolean = False
            Dim isSeasonalOrSecondary As Boolean = False

            If quote.Locations(0) IsNot Nothing AndAlso quote.Locations(0).A_Dwelling_Limit IsNot Nothing Then
                A_Dwelling_Limit = IFM.Common.InputValidation.InputHelpers.TryToGetInt32(quote.Locations(0).A_Dwelling_Limit)
            End If
            If quote IsNot Nothing AndAlso quote.Locations IsNot Nothing AndAlso quote.Locations.HasItemAtIndex(0) Then
                If quote.Locations(0).FormTypeId = "25" Then IsHO4 = True
                If quote.Locations(0).StructureTypeId = "2" Then IsMobileHome = True
                If quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5") Then isSeasonalOrSecondary = True
            End If

            Dim ResetVars = Sub()
                                myCovInfoToAdd.Clear()
                                myCov = Nothing
                                Limit = ""
                                Premium = ""
                                SuperScript = ""
                                sectionCoveragePropertyToUseForLimit = SectionCoveragePropertyToUseForLimit.TotalLimit
                                isSubItem = False
                                ignoreSort = False
                                subItemId = 0
                                parentName = ""
                                'Not resetting myICov because we are allowing it to be used after calling addCov in a potentially special scenario. 
                                'However, it will get orverwritten the next time it is needed. So it shouldn't be an issue since we aren't sending this into the function.
                            End Sub

            Dim AddCov = Sub(sectionICoverageType As Object)
                             AddCoverageToSummaryCoverageList(Of QuickQuoteSectionICoverage)(quote:=quote, opCovList:=opCovList, sectionCovType:=sectionICoverageType, isSubItem:=isSubItem, SuperScriptText:=SuperScript, ForceLimitText:=Limit, ForcePremiumText:=Premium, CoverageInformationToAdd:=myCovInfoToAdd, myCovItem:=myCov, SplitSectionIandIICoverage:=False, SectionCoveragePropertyToUseForLimit:=sectionCoveragePropertyToUseForLimit, ignoreSort:=ignoreSort, SummaryType:=SummaryType, parentName:=parentName, subItemId:=subItemId)
                             If myCov IsNot Nothing Then
                                 myICov = DirectCast(myCov, QuickQuoteSectionICoverage)
                             Else
                                 myICov = Nothing
                             End If
                             ResetVars()
                         End Sub

            Dim SetSubItemAndCounter = Sub(myParentName As String)
                                           parentName = myParentName
                                           isSubItem = True
                                           subItemId = counter
                                           counter += 1
                                       End Sub

            If OptionalOrIncluded = OptionalOrIncludedCoverages.OptionalCoverages Then
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.CreditCardFundTransForgeryEtc)
                ' if the limit is not increased then show it as included otherwise you will show it below in the optional coverages below - Matt A 3-23-16 Comp Rater Project
                '2/14/2018 - Dan Gugenheim - Attempting to maintain similar logic as before - This is the inverse of the included logic. If the coverage was not increased, then remove it from this list because it should show as included not optional
                If myICov IsNot Nothing Then
                    If myICov.IncreasedLimitId.EqualsAny("", "0") Then
                        opCovList.Remove(opCovList.Last)
                    End If
                End If
                ignoreSort = True
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Equipment_Breakdown_Coverage)
                ignoreSort = True
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.PersonalPropertyReplacement)
                ignoreSort = True
                If CDate(quote.EffectiveDate) < CyberEffDate OrElse (IsHO4 OrElse IsMobileHome) Then
                    AddCov(QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement)
                    ignoreSort = True
                    'Added 10/6/2022 for task 51260 MLW
                    If quote.Locations(0).FormTypeId = "22" AndAlso IsMobileHome AndAlso Not isSeasonalOrSecondary Then 'FormTypeId 22 = FO2
                        ' On the screen quote the superscript is 7, on printer-friendly it's 6
                        If SummaryType = SummaryType.QuoteSummary Then
                            SuperScript = "7"
                        Else
                            SuperScript = "6"
                        End If
                        AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Family_Cyber_Protection)
                        ignoreSort = True
                    End If
                Else
                    ' On the screen quote the superscript is 7, on printer-friendly it's 6
                    If SummaryType = SummaryType.QuoteSummary Then
                        SuperScript = "7"
                    Else
                        SuperScript = "6"
                    End If
                    If Not isSeasonalOrSecondary Then
                        AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Family_Cyber_Protection)
                        ignoreSort = True
                    End If
                    AddCov(QuickQuoteSectionICoverage.SectionICoverageType.HomeownerEnhancementEndorsement1019)
                    ignoreSort = True
                End If
                'Updated 5/20/2022 above for tasks 74136 and 74145 MLW
                'AddCov(QuickQuoteSectionICoverage.SectionICoverageType.BackupSewersAndDrains)
                'ignoreSort = True
                If HOM_General.OkayForHPEEWaterBU(quote) = False Then
                    AddCov(QuickQuoteSectionICoverage.SectionICoverageType.BackupSewersAndDrains)
                    ignoreSort = True
                End If
                If CDate(quote.EffectiveDate) < CyberEffDate OrElse IsHO4 OrElse IsMobileHome Then
                    AddCov(QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement)
                    ignoreSort = True
                Else
                    ' Don't add plus for seasonal or secondary occupancy
                    If Not isSeasonalOrSecondary Then
                        AddCov(QuickQuoteSectionICoverage.SectionICoverageType.HomeownersPlusEnhancementEndorsement1020)
                    End If
                    ignoreSort = True
                End If
                'Added 5/20/2022 for tasks 74136 and 74145 MLW
                If HOM_General.OkayForHPEEWaterBU(quote) Then
                    AddCov(QuickQuoteSectionICoverage.SectionICoverageType.BackupSewersAndDrains)
                    ignoreSort = True
                End If
                ' Don't add water damager when seasonal or secondary because the plus coverage was removed
                If Not isSeasonalOrSecondary Then
                    AddCov(QuickQuoteSectionICoverage.SectionICoverageType.WaterDamage)
                End If
                ignoreSort = True
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.DebrisRemoval)

                ignoreSort = True
                'Limit = "N/A"
                'AddCov(QuickQuoteSectionICoverage.SectionICoverageType.CovASpecifiedAdditionalAmountOfInsurance)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Home_CoverageASpecialCoverage)

                ignoreSort = True
                If SummaryType = SummaryType.PrintSummary Then
                    Limit = "N/A"
                End If
                sectionCoveragePropertyToUseForLimit = SectionCoveragePropertyToUseForLimit.IncreasedLimitId
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Earthquake)

                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlement)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing)

                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.BroadenedResidencePremisesDefinition)

                Limit = "Cov B"
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.CovBOtherStructuresAwayFromTheResidencePremises)

                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Farm_Fire_Department_Service_Charge)
                '4/6/2018 - MLW - If the coverage was not increased - remove it from this list. Will be added to included coverages.
                If myICov IsNot Nothing Then
                    If myICov.IncreasedLimit.EqualsAny("", "0") Then
                        opCovList.Remove(opCovList.Last)
                    End If
                End If

                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.FunctionalReplacementCostLossAssessment)

                sectionCoveragePropertyToUseForLimit = SectionCoveragePropertyToUseForLimit.IncreasedCostOfLoss
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.GreenUpgrades)

                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.IdentityFraudExpenseHOM0455)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.LossAssessment_Earthquake)
                'Limit is Limit A or 500,000 whichever is less - except for HO4 which does not have a Limit A
                'Updated 5/10/2018 for Bug 26645 MLW
                'If A_Dwelling_Limit IsNot Nothing AndAlso A_Dwelling_Limit < "500,000" AndAlso A_Dwelling_Limit <> "0" Then
                If A_Dwelling_Limit < 500000 AndAlso A_Dwelling_Limit <> 0 Then
                    Limit = A_Dwelling_Limit
                Else
                    Limit = "500,000"
                End If
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovAAndB)
                'not avail for HO4 and mobile
                'Updated 5/10/2018 for Bug 26645 MLW
                'If A_Dwelling_Limit IsNot Nothing AndAlso A_Dwelling_Limit < "500,000" AndAlso A_Dwelling_Limit <> "0" Then
                If A_Dwelling_Limit < 500000 AndAlso A_Dwelling_Limit <> 0 Then
                    Limit = A_Dwelling_Limit
                Else
                    Limit = "500,000"
                End If
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.MineSubsidenceCovA)
                'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                'AddCov(QuickQuoteSectionICoverage.SectionICoverageType.OtherStructuresOnTheResidencePremises)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Cov_B_Related_Private_Structures)

                myCovInfoToAdd.Add(AdditionalCoverageInformationType.Address)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.PersonalPropertyatOtherResidenceIncreaseLimit)
                If myICov IsNot Nothing Then
                    If myICov.IncreasedLimit.EqualsAny("", "0") Then
                        opCovList.Remove(opCovList.Last) 'this removes the address
                        opCovList.Remove(opCovList.Last) 'this removes the coverage
                    End If
                End If
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.PersonalPropertySelfStorageFacilities)
                '4/11/2018 - MLW - If the coverage was not increased - remove it from this list. Will be added to included coverages. Bug 26125
                If myICov IsNot Nothing Then
                    If myICov.IncreasedLimit.EqualsAny("", "0") Then
                        opCovList.Remove(opCovList.Last)
                    End If
                End If
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.ReplacementCostForNonBuildingStructures)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.SinkholeCollapse)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.SpecialComputerCoverage)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.SpecialPersonalProperty)

                myCovInfoToAdd.Add(AdditionalCoverageInformationType.Address)
                'AddCov(QuickQuoteSectionICoverage.SectionICoverageType.SpecificStructuresAwayFromResidencePremises)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises)

                'Limit = "N/A" 'Removed for Bug 27386 MLW
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.TheftofBuildingMaterial)
                If IFM.VR.Common.Helpers.HOM.HOMTheftOfFirearmsIncrease.IsHOMTheftOfFirearmsIncreaseAvailable(quote) Then
                    AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Firearms)
                    If myICov IsNot Nothing Then
                        If myICov.IncreasedLimit.EqualsAny("", "0") Then
                            opCovList.Remove(opCovList.Last)
                        End If
                    End If
                End If
                If IFM.VR.Common.Helpers.HOM.HOMTheftOfJewelryIncrease.IsHOMTheftOfJewelryIncreaseAvailable(quote) Then
                    AddCov(QuickQuoteSectionICoverage.SectionICoverageType.JewelryWatchesAndFurs)
                    If myICov IsNot Nothing Then
                        If myICov.IncreasedLimit.EqualsAny("", "0") Then
                            opCovList.Remove(opCovList.Last)
                        End If
                    End If
                End If

                myCovInfoToAdd.Add(AdditionalCoverageInformationType.CoverageDateRange)
                Limit = "N/A"
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.TheftOfPersonalPropertyInDwellingUnderConstruction)

                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.UndergroundServiceLine)

                Limit = "N/A"
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.UnitOwnersCoverageA)

                Limit = "N/A"
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.UnitOwnersCoverageCSpecialCoverage)


#If DEBUG Then
                AddBlankCoverages(Of QuickQuoteSectionICoverage)(quote, opCovList) 'For developers to see if any coverages have been added that are coming up as blank.
#End If
            Else 'Included Coverages
                Limit = "see"
                SuperScript = "6"
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.BuildingAdditionsAndAlterations)

                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.BusinessPropertyIncreased)

                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.CreditCardFundTransForgeryEtc)
                ' if the limit is not increased then show it as included otherwise you will show it below in the optional coverages below - Matt A 3-23-16 Comp Rater Project
                '2/14/2018 - Dan Gugenheim - Attempting to maintain similar logic as before - If the coverage was increased - remove it from this list.
                If myICov IsNot Nothing Then
                    If myICov.IncreasedLimitId.NotEqualsAny("", "0") Then
                        opCovList.Remove(opCovList.Last)
                    End If
                End If


                Dim myCovCIncreasedSpecialLimitsOfLiabilityName As String = "Cov. C Increased Special Limits of Liability"
                opCovList.Add(New SummaryCoverageLineItem With {.Name = myCovCIncreasedSpecialLimitsOfLiabilityName, .Limit = "", .Premium = "", .IsSubItem = False, .hasChildren = True})

                SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.AntennasTapesWireRecordersDisksAndMediaInAMotorVehicle)

                SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Firearms)
                If IFM.VR.Common.Helpers.HOM.HOMTheftOfFirearmsIncrease.IsHOMTheftOfFirearmsIncreaseAvailable(quote) AndAlso myICov IsNot Nothing Then
                    If myICov.IncreasedLimit.NotEqualsAny("", "0") Then
                        opCovList.Remove(opCovList.Last)
                    End If
                End If

                'Removed 6/29/18 for HOM2011 Upgrade post go-live changes MLW - added back below
                'SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                'AddCov(QuickQuoteSectionICoverage.SectionICoverageType.GraveMarkers)

                SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.JewelryWatchesAndFurs)
                If IFM.VR.Common.Helpers.HOM.HOMTheftOfJewelryIncrease.IsHOMTheftOfJewelryIncreaseAvailable(quote) AndAlso myICov IsNot Nothing Then
                    If myICov.IncreasedLimit.NotEqualsAny("", "0") Then
                        opCovList.Remove(opCovList.Last)
                    End If
                End If

                SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.LandlordsFurnishing)

                SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Money)

                SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.PersonalPropertyatOtherResidenceIncreaseLimit)
                If myICov IsNot Nothing Then
                    If myICov.IncreasedLimit.NotEqualsAny("", "0") Then
                        opCovList.Remove(opCovList.Last)
                    End If
                End If

                SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.PersonalPropertySelfStorageFacilities)
                '4/11/2018 - MLW - If the coverage was increased - remove it from this list. Will be added to optional coverages. Bug 26125
                If myICov IsNot Nothing Then
                    If myICov.IncreasedLimit.NotEqualsAny("", "0") Then
                        opCovList.Remove(opCovList.Last)
                    End If
                End If

                SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.PortableElectronicsInAMotorVehicle)

                SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Securities)

                SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.SilverwareGoldwarePewterware)

                SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Trailers_NonWatercraft)

                SetSubItemAndCounter(myCovCIncreasedSpecialLimitsOfLiabilityName)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Watercraft)

                'AddCov(QuickQuoteSectionICoverage.SectionICoverageType.DamageToPropertyOfOthers) 'Moved to Section II
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Farm_Fire_Department_Service_Charge)
                '4/6/2018 - MLW - If the coverage was increased - remove it from this list. Will be added to optional coverages.
                If myICov IsNot Nothing Then
                    If myICov.IncreasedLimit.NotEqualsAny("", "0") Then
                        opCovList.Remove(opCovList.Last)
                    End If
                End If

                AddLimitedFungiWetOrDryRotOrBacteriaCoverageToList(quote, opCovList, SummaryType.QuoteSummary)

                'Added 6/29/18 for HOM2011 Upgrade post go-live changes MLW
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.GraveMarkers)

                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.IncreasedLimitsMotorizedVehicles)
                If SummaryType = SummaryType.PrintSummary Then
                    AddCov(QuickQuoteSectionICoverage.SectionICoverageType.Inflation_Guard)
                End If
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.OrdinanceOrLaw)
                AddCov(QuickQuoteSectionICoverage.SectionICoverageType.TreesPlantsAndShrubs)
            End If

        End Sub

        Public Shared Sub AddLimitedFungiWetOrDryRotOrBacteriaCoverageToList(quote As QuickQuoteObject, ByRef opCovList As List(Of SummaryCoverageLineItem), Optional SummaryType As SummaryType = SummaryType.QuoteSummary)
            If quote IsNot Nothing Then
                If quote.Locations.HasItemAtIndex(0) Then
                    If quote.Locations(0).StructureTypeId <> "2" Then
                        Dim LimitedFungiWetOrDryRotOrBacteriaCoverage As String = "Limited Fungi, Wet or Dry Rot, or Bacteria Coverage"
                        If SummaryType = SummaryType.QuoteSummary Then
                            opCovList.Add(New SummaryCoverageLineItem With {.Name = LimitedFungiWetOrDryRotOrBacteriaCoverage, .Limit = "10,000", .Premium = "Included", .IsSubItem = False, .hasChildren = False})
                        Else
                            opCovList.Add(New SummaryCoverageLineItem With {.Name = LimitedFungiWetOrDryRotOrBacteriaCoverage, .Limit = "100,000", .Premium = "Included", .IsSubItem = False, .hasChildren = False})
                        End If
                    End If
                End If
            End If
        End Sub

        Public Shared Sub AddSectionIICoveragesToList(quote As QuickQuoteObject, opCovList As List(Of SummaryCoverageLineItem), ByVal OptionalOrIncluded As OptionalOrIncludedCoverages, Optional SummaryType As SummaryType = SummaryType.QuoteSummary)
            If quote IsNot Nothing Then
                If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            Dim myCovInfoToAdd As New List(Of AdditionalCoverageInformationType)
            Dim Limit As String = ""
            Dim Premium As String = ""
            Dim SuperScript As String = ""
            Dim myCov As Object = Nothing
            Dim myIICov As New QuickQuoteSectionIICoverage
            Dim isSubItem As Boolean = False
            Dim sectionCoveragePropertyToUseForLimit As SectionCoveragePropertyToUseForLimit = SectionCoveragePropertyToUseForLimit.TotalLimit
            Dim ignoreSort As Boolean = False
            Dim parentName As String = ""
            Dim subItemId As Integer = 0
            Dim counter As Integer = 0

            Dim ResetVars = Sub()
                                myCovInfoToAdd.Clear()
                                myCov = Nothing
                                Limit = ""
                                Premium = ""
                                SuperScript = ""
                                sectionCoveragePropertyToUseForLimit = SectionCoveragePropertyToUseForLimit.TotalLimit
                                isSubItem = False
                                ignoreSort = False
                                'Not resetting myIICov because we are allowing it to be used after calling addCov in a potentially special scenario. 
                                'However, it will get orverwritten the next time it is needed. So it shouldn't be an issue since we aren't sending this into the function.
                            End Sub

            Dim AddCov = Sub(sectionIICoverageType As Object)
                             AddCoverageToSummaryCoverageList(Of QuickQuoteSectionIICoverage)(quote:=quote, opCovList:=opCovList, sectionCovType:=sectionIICoverageType, isSubItem:=isSubItem, SuperScriptText:=SuperScript, ForceLimitText:=Limit, ForcePremiumText:=Premium, CoverageInformationToAdd:=myCovInfoToAdd, myCovItem:=myCov, SplitSectionIandIICoverage:=False, SectionCoveragePropertyToUseForLimit:=sectionCoveragePropertyToUseForLimit, ignoreSort:=ignoreSort, SummaryType:=SummaryType)
                             If myCov IsNot Nothing Then
                                 myIICov = DirectCast(myCov, QuickQuoteSectionIICoverage)
                             Else
                                 myIICov = Nothing
                             End If
                             ResetVars()
                         End Sub

            Dim SetSubItemAndCounter = Sub(myParentName As String)
                                           parentName = myParentName
                                           isSubItem = True
                                           subItemId = counter
                                           counter += 1
                                       End Sub

            If OptionalOrIncluded = OptionalOrIncludedCoverages.OptionalCoverages Then
                If SummaryType = SummaryType.QuoteSummary Then
                    Limit = "Included"
                Else
                    Limit = "see"
                End If
                SuperScript = "4"
                myCovInfoToAdd.Add(AdditionalCoverageInformationType.Address)
                'AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencePremisesOccupiedbyanInsured)
                AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.OtherLocationOccupiedByInsured)

                If SummaryType = SummaryType.QuoteSummary Then
                    Limit = "Included"
                Else
                    Limit = "see"
                End If
                SuperScript = "4"
                myCovInfoToAdd.Add(AdditionalCoverageInformationType.Address)
                'AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.OptionalLiability_AdditionalResidencesOrFarmsRentedtoOthers)
                AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.AdditionalResidenceRentedToOther)

                If SummaryType = SummaryType.QuoteSummary Then
                    Limit = "Included"
                Else
                    Limit = "see"
                End If
                SuperScript = "4"
                AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.BusinessPursuits_Clerical)

                Limit = "N/A"
                Premium = "N/A"
                AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.CanineLiabilityExclusion)

                Limit = "see"
                SuperScript = "4"
                AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres) 'Supposed to be 0-100???

                If SummaryType = SummaryType.QuoteSummary Then
                    Limit = "Included"
                Else
                    Limit = "see"
                End If
                SuperScript = "4"
                myCovInfoToAdd.Add(AdditionalCoverageInformationType.Description)
                AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.IncidentalFarmersPersonalLiability)

                If SummaryType = SummaryType.QuoteSummary Then
                    Limit = "Included"
                Else
                    Limit = "see"
                End If
                SuperScript = "4"
                myCovInfoToAdd.Add(AdditionalCoverageInformationType.Address)
                AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises)

                AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.LowPowerRecreationalMotorVehicleLiability)

                If SummaryType = SummaryType.QuoteSummary Then
                    Limit = "Included"
                Else
                    Limit = "see"
                End If
                SuperScript = "4"
                myCovInfoToAdd.Add(AdditionalCoverageInformationType.Address)
                AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence)

                Limit = "see"
                SuperScript = "5"
                AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.PersonalInjury)

                Limit = "1,000,000"
                SuperScript = ""
                'Premium = SECov.Premium
                AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.SpecialEventCoverage)
#If DEBUG Then
                AddBlankCoverages(Of QuickQuoteSectionIICoverage)(quote, opCovList) 'For developers to see if any coverages have been added that are coming up as blank.
#End If
            Else 'Included Coverages
                AddCov(QuickQuoteSectionIICoverage.SectionIICoverageType.DamageToPropertyOfOthers)
            End If
        End Sub

        Public Shared Sub AddSectionIAndIICoveragesToList(quote As QuickQuoteObject, opCovList As List(Of SummaryCoverageLineItem), OptionalOrIncluded As OptionalOrIncludedCoverages, Optional SummaryType As SummaryType = SummaryType.QuoteSummary, Optional SectionCoverageType As SectionIandIICoverageType = SectionIandIICoverageType.Both)
            If quote Is Nothing Then
                If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            Dim myCovInfoToAdd As New List(Of AdditionalCoverageInformationType)
            Dim Limit As String = ""
            Dim LimitLiability As String = ""
            Dim LimitProperty As String = ""
            Dim Premium As String = ""
            Dim SuperScript As String = ""
            Dim myCov As Object = Nothing
            Dim myIandIICov As New QuickQuoteSectionIAndIICoverage
            Dim isSubItem As Boolean = False
            Dim sectionCoveragePropertyToUseForLimit As SectionCoveragePropertyToUseForLimit = SectionCoveragePropertyToUseForLimit.TotalLimit
            Dim splitSectionIandIICoverage As Boolean = False
            Dim ignoreSort As Boolean = False
            Dim parentName As String = ""
            Dim subItemId As Integer = 0
            Dim counter As Integer = 0

            Dim ResetVars = Sub()
                                myCovInfoToAdd.Clear()
                                myCov = Nothing
                                Limit = ""
                                Premium = ""
                                SuperScript = ""
                                splitSectionIandIICoverage = False
                                sectionCoveragePropertyToUseForLimit = SectionCoveragePropertyToUseForLimit.TotalLimit
                                isSubItem = False
                                ignoreSort = False
                                LimitLiability = ""
                                LimitProperty = ""
                                'Not resetting myIandIICov because we are allowing it to be used after calling addCov in a potentially special scenario. 
                                'However, it will get orverwritten the next time it is needed. So it shouldn't be an issue since we aren't sending this into the function.
                            End Sub

            Dim AddCov = Sub(sectionIandIICoverageType As Object)
                             AddCoverageToSummaryCoverageList(Of QuickQuoteSectionIAndIICoverage)(quote:=quote, opCovList:=opCovList, sectionCovType:=sectionIandIICoverageType, isSubItem:=isSubItem, SuperScriptText:=SuperScript, ForceLimitText:=Limit, ForcePremiumText:=Premium, ForceLimitLiabilityText:=LimitLiability, ForceLimitPropertyText:=LimitProperty, CoverageInformationToAdd:=myCovInfoToAdd, myCovItem:=myCov, SplitSectionIandIICoverage:=splitSectionIandIICoverage, SectionCoveragePropertyToUseForLimit:=sectionCoveragePropertyToUseForLimit, ignoreSort:=ignoreSort, SummaryType:=SummaryType, SectionCoverageType:=SectionCoverageType)

                             If myCov IsNot Nothing Then
                                 myIandIICov = DirectCast(myCov, QuickQuoteSectionIAndIICoverage)
                             Else
                                 myIandIICov = Nothing
                             End If
                             ResetVars()
                         End Sub

            Dim SetSubItemAndCounter = Sub(myParentName As String)
                                           parentName = myParentName
                                           isSubItem = True
                                           subItemId = counter
                                           counter += 1
                                       End Sub

            If OptionalOrIncluded = OptionalOrIncludedCoverages.OptionalCoverages Then
                Limit = "see"
                SuperScript = "4"
                myCovInfoToAdd.Add(AdditionalCoverageInformationType.AdditionalInterestNameDescriptionAddress)
                AddCov(QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence)

                splitSectionIandIICoverage = True
                myCovInfoToAdd.Add(AdditionalCoverageInformationType.AdditionalInterestNameDescriptionAddress)
                AddCov(QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage)

                'myCovInfoToAdd.Add(AdditionalCoverageInformationType.Address)
                AddCov(QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.LossAssessment)


                If SummaryType = SummaryType.PrintSummary Then
                    Limit = "N/A"
                Else
                    Limit = "see"
                End If
                SuperScript = "4"
                myCovInfoToAdd.Add(AdditionalCoverageInformationType.NameAndDescription)
                AddCov(QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold)

                SuperScript = "4"
                splitSectionIandIICoverage = True
                myCovInfoToAdd.Add(AdditionalCoverageInformationType.BusinessDescription)
                AddCov(QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures)

                'If SummaryType = SummaryType.PrintSummary Then 'Removed 5/2/18 for Bugs 26226, 26395 and 26400 MLW
                '    'Limit = "see"
                '    SuperScript = "4"
                'End If
                'myCovInfoToAdd.Add(AdditionalCoverageInformationType.Description) 'Removed 5/1/18 for Bug 26226 MLW
                AddCov(QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers)

                If SummaryType = SummaryType.PrintSummary Then
                    'Limit = "Included"
                    LimitProperty = "Included"
                    LimitLiability = "see<sup>4</sup>"
                Else
                    Limit = "see"
                    SuperScript = "4"
                End If
                myCovInfoToAdd.Add(AdditionalCoverageInformationType.AdditionalInterestNameAddress)
                AddCov(QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement)

                If IFM.VR.Common.Helpers.HOM.UnitOwnersRentalToOthers.IsUnitOwnersRentalToOthersAvailable(quote) Then
                    AddCov(QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers)
                End If

#If DEBUG Then
                AddBlankCoverages(Of QuickQuoteSectionIAndIICoverage)(quote, opCovList) 'For developers to see if any coverages have been added that are coming up as blank.
#End If
            Else 'Included Coverages
                'none yet
            End If
        End Sub
#End Region




#Region "Print Summary Logic"
        Private Shared Function GetSortedOpCovList(opCovList As List(Of SummaryCoverageLineItem)) As List(Of SummaryCoverageLineItem)
            Dim SortedCovList As New List(Of SummaryCoverageLineItem)
            Dim SubItemList As New List(Of SummaryCoverageLineItem)

            For Each cov As SummaryCoverageLineItem In opCovList
                If cov.IgnoreSort = True Then
                    SortedCovList.Add(cov) 'Add any coverages with the IgnoreSort property set to true to the top of the SortedCovList in their current order.
                End If
            Next
            opCovList.RemoveAll(Function(x) x.IgnoreSort = True) 'Since we already took care of the IgnoreSort coverages, lets remove them from the opCovList

            For Each cov As SummaryCoverageLineItem In opCovList
                If cov.IsSubItem = True Then
                    SubItemList.Add(cov) 'Get all of the SubItems into their own list
                End If
            Next
            opCovList.RemoveAll(Function(x) x.IsSubItem = True) 'Remove the SubItems from the opCovList

            opCovList = (From a In opCovList Select a Order By a.Name).ToList() 'Sort the list by Name

            For Each cov As SummaryCoverageLineItem In opCovList
                SortedCovList.Add(cov)  'Loop through the opcovList and add them into the SortedCovList in their new sorted order.
                If cov.hasChildren = True Then
                    For Each subItem As SummaryCoverageLineItem In From b In SubItemList.Where(Function(x) x.ParentName.Equals(cov.Name, StringComparison.Ordinal) = True) Select b Order By b.SubItemId 'Loop through a List consisting of SubItems that have the ParentName equal to the recently added opCovList item. The List is sorted by SubItemId so that they get added in the appropriate order
                        SortedCovList.Add(subItem) '
                    Next
                End If
            Next

            Return SortedCovList
        End Function

        Public Shared Function GetIncludedCoverageList_Print(quote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of SummaryCoverageLineItem)
            If quote Is Nothing Then
                Throw New ArgumentNullException(NameOf(quote))
            End If
            If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Dim opCovList As New List(Of SummaryCoverageLineItem)
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If quote IsNot Nothing AndAlso quote.Locations.HasItemAtIndex(0) Then
                Dim qqh As New QuickQuoteHelperClass
                If qqh.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                    AddSectionICoveragesToList(quote, opCovList, OptionalOrIncludedCoverages.IncludedCoverages, SummaryType.PrintSummary)
                    AddSectionIICoveragesToList(quote, opCovList, OptionalOrIncludedCoverages.IncludedCoverages, SummaryType.PrintSummary)
                    AddSectionIAndIICoveragesToList(quote, opCovList, OptionalOrIncludedCoverages.IncludedCoverages, SummaryType.PrintSummary)
                Else
                    opCovList = GetIncludedCoverageListOld(quote)
                End If
            End If
            Return opCovList
        End Function

        Public Shared Function GetOtherPropertyList_Print(quote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of SummaryCoverageLineItem)
            If quote Is Nothing Then
                Throw New ArgumentNullException(NameOf(quote))
            End If
            If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Dim qqh As New QuickQuoteHelperClass
            If qqh.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Dim opCovList As New List(Of SummaryCoverageLineItem)
                Dim secIList As New List(Of SummaryCoverageLineItem)
                Dim secIandIIList As New List(Of SummaryCoverageLineItem)

                AddSectionICoveragesToList(quote, secIList, OptionalOrIncludedCoverages.OptionalCoverages, SummaryType.PrintSummary)
                AddSectionIAndIICoveragesToList(quote, secIandIIList, OptionalOrIncludedCoverages.OptionalCoverages, SummaryType.PrintSummary, SectionIandIICoverageType.PropertyCoverages)
                ''Added 5/4/18 for HOM Upgrade for Bug 26572 MLW - if Occupancy Type is Secondary or Seasonal, need to force Loss Assessment to show
                'If quote.Locations(0).OccupancyCodeId = "4" OrElse quote.Locations(0).OccupancyCodeId = "5" Then
                '    secIandIIList.Add(New SummaryCoverageLineItem With {.Name = "Loss Assessment (HO 0435)", .Limit = "1,000", .Premium = "Included", .IsSubItem = False, .SectionCoverageType = GetType(QuickQuoteSectionIAndIICoverage), .IgnoreSort = False, .hasChildren = False, .SubItemId = 0, .ParentName = ""})
                'End If

                secIandIIList.RemoveAll(Function(x) (x.Name.IndexOf("(Liability)", StringComparison.OrdinalIgnoreCase) >= 0 OrElse x.ParentName.IndexOf("(Liability)", StringComparison.OrdinalIgnoreCase) >= 0)) 'Remove any Liability related items from our SecIandIIList
                'Added for Permitted Incidental Occupancies Residence Premises, special case where it is split into two, only need Property item here - 4/10/18 MLW
                'had this backwards, Other Structures shows on Liability - Bug 26294 - updated 4/16/18 MLW 
                'secIandIIList.RemoveAll(Function(x) (x.Name.IndexOf("Permitted Incidental Occupancies Residence Premises (HO 0442)", StringComparison.OrdinalIgnoreCase) >= 0 OrElse x.ParentName.IndexOf("Permitted Incidental Occupancies Residence Premises (HO 0442)", StringComparison.OrdinalIgnoreCase) >= 0)) 'Remove PIORP liability item from our SecIandIIList
                secIandIIList.RemoveAll(Function(x) (x.Name.IndexOf("Permitted Incidental Occupancies Residence Premises - Other Structures (HO 0442)", StringComparison.OrdinalIgnoreCase) >= 0 OrElse x.ParentName.IndexOf("Permitted Incidental Occupancies Residence Premises - Other Structures (HO 0442)", StringComparison.OrdinalIgnoreCase) >= 0)) 'Remove PIORP property item from our SecIandIIList

                For Each cov As SummaryCoverageLineItem In secIandIIList
                    cov.Name = cov.Name.Replace(" (Property)", "")
                    cov.ParentName = cov.ParentName.Replace(" (Property)", "")
                    cov.Name = cov.Name.Replace("(Property)", "")
                    cov.ParentName = cov.ParentName.Replace("(Property)", "")
                    cov.Name.Trim()
                    cov.ParentName.Trim()
                Next

                For Each cov As SummaryCoverageLineItem In secIandIIList
                    'Updated 5/1/18 for bug 26102 MLW
                    If cov.ParentName = "Additional Insured - Student Living Away from Residence (HO 0527)" Then
                        If cov.Limit <> "" Then
                            cov.Limit = "Included"
                        End If
                    End If
                Next

                opCovList = secIList.Union(secIandIIList).ToList()

                Return GetSortedOpCovList(opCovList)
            Else
                Return GetOtherPropertyList_PrintOld(quote)
            End If
        End Function

        Public Shared Function GetLiabilityList_Print(quote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of SummaryCoverageLineItem)
            If quote Is Nothing Then
                Throw New ArgumentNullException(NameOf(quote))
            End If
            If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Dim qqh As New QuickQuoteHelperClass
            If qqh.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Dim opCovList As New List(Of SummaryCoverageLineItem)
                Dim secIIList As New List(Of SummaryCoverageLineItem)
                Dim secIandIIList As New List(Of SummaryCoverageLineItem)

                AddSectionIICoveragesToList(quote, secIIList, OptionalOrIncludedCoverages.OptionalCoverages, SummaryType.PrintSummary)
                AddSectionIAndIICoveragesToList(quote, secIandIIList, OptionalOrIncludedCoverages.OptionalCoverages, SummaryType.PrintSummary, SectionIandIICoverageType.LiabilityCoverages)
                ''Added 5/4/18 for HOM Upgrade for Bug 26572 MLW - if Occupancy Type is Secondary or Seasonal, need to force Loss Assessment to show
                'If quote.Locations(0).OccupancyCodeId = "4" OrElse quote.Locations(0).OccupancyCodeId = "5" Then
                '    secIandIIList.Add(New SummaryCoverageLineItem With {.Name = "Loss Assessment (HO 0435)", .Limit = "1,000", .Premium = "Included", .IsSubItem = False, .SectionCoverageType = GetType(QuickQuoteSectionIAndIICoverage), .IgnoreSort = False, .hasChildren = False, .SubItemId = 0, .ParentName = ""})
                'End If

                secIandIIList.RemoveAll(Function(x) (x.Name.IndexOf("(Property)", StringComparison.OrdinalIgnoreCase) >= 0 OrElse x.ParentName.IndexOf("(Property)", StringComparison.OrdinalIgnoreCase) >= 0)) 'Remove any Property related items from our SecIandIIList
                'Added for Permitted Incidental Occupancies Residence Premises, special case where it is split into two, only need Liability item here - 4/10/18 MLW
                'had this backwards, Other Structures shows on Liability - Bug 26294 - updated 4/16/18 MLW
                secIandIIList.RemoveAll(Function(x) (x.Name.IndexOf("Permitted Incidental Occupancies Residence Premises (HO 0442)", StringComparison.OrdinalIgnoreCase) >= 0 OrElse x.ParentName.IndexOf("Permitted Incidental Occupancies Residence Premises (HO 0442)", StringComparison.OrdinalIgnoreCase) >= 0)) 'Remove PIORP liability item from our SecIandIIList
                'secIandIIList.RemoveAll(Function(x) (x.Name.IndexOf("Permitted Incidental Occupancies Residence Premises - Other Structures (HO 0442)", StringComparison.OrdinalIgnoreCase) >= 0 OrElse x.ParentName.IndexOf("Permitted Incidental Occupancies Residence Premises - Other Structures (HO 0442)", StringComparison.OrdinalIgnoreCase) >= 0)) 'Remove PIORP property item from our SecIandIIList
                secIandIIList.RemoveAll(Function(x) (x.Name.IndexOf("Unit Owners Rental to Others (HO 1733)", StringComparison.OrdinalIgnoreCase) >= 0 OrElse x.ParentName.IndexOf("Unit Owners Rental to Others (HO 1733)", StringComparison.OrdinalIgnoreCase) >= 0)) 'Remove Unit Owners liability item from our SecIandIIList

                For Each cov As SummaryCoverageLineItem In secIandIIList
                    cov.Name = cov.Name.Replace(" (Liability)", "")
                    cov.ParentName = cov.ParentName.Replace(" (Liability)", "")
                    cov.Name = cov.Name.Replace("(Liability)", "")
                    cov.ParentName = cov.ParentName.Replace("(Liability)", "")
                    cov.Name.Trim()
                    cov.ParentName.Trim()
                Next

                For Each cov As SummaryCoverageLineItem In secIandIIList
                    'Updated 4/16/18 for bug 26294 MLW
                    If cov.Name = "Permitted Incidental Occupancies Residence Premises - Other Structures (HO 0442)" OrElse cov.ParentName = "Permitted Incidental Occupancies Residence Premises - Other Structures (HO 0442)" Then
                        'do not change the premium to included for this coverage - Bug 26294
                    ElseIf cov.Name = "Structures Rented to Others - Residence Premises (HO 0440)" Then
                        'Added 5/2/18 for Bugs 26388, 26395 and 26400 MLW
                        cov.Limit = "see<sup>4</sup>"
                        cov.Premium = "Included"
                    ElseIf cov.ParentName = "Other Members of Your Household (HO 0458)" AndAlso cov.Name <> "Other Members of Your Household (HO 0458)" Then
                        'Added 4/18/18 for Bug 26182 MLW
                        cov.Limit = "Included"
                        cov.Premium = "Included"
                    Else
                        If IsNumeric(cov.Premium) Then 'Only change the premiums that are numbers, we would like all of the original text to stay the same
                            cov.Premium = "Included" 'These numbers should all be shown on the property side so we want the liability side to say "Included"
                        End If
                    End If
                Next

                opCovList = secIIList.Union(secIandIIList).ToList()

                AddLimitedFungiWetOrDryRotOrBacteriaCoverageToList(quote, opCovList, SummaryType.PrintSummary)

                Return GetSortedOpCovList(opCovList)
            Else
                Return GetLiabilityList_PrintOld(quote)
            End If
        End Function
#End Region

#Region "OLD QUOTE SUMMARY"
        Public Shared Function GetOptionalCoverageListOld(quote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of SummaryCoverageLineItem)
            If quote Is Nothing Then
                Throw New ArgumentNullException(NameOf(quote))
            End If
            If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Dim opCovList As New List(Of SummaryCoverageLineItem)
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If quote IsNot Nothing AndAlso quote.Locations.HasItemAtIndex(0) Then
                Dim MyLocation = quote.Locations(0)
                Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()



                If MyLocation.SectionICoverages IsNot Nothing Then
                    Dim GetCoverageName = Function(sectionI_Enum As QuickQuoteSectionICoverage.HOM_SectionICoverageType) As String
                                              Dim info = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetCoverageDisplayProperties(quote, MyLocation, SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage, sectionI_Enum, QuickQuoteSectionIICoverage.SectionIICoverageType.None, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None, False)
                                              Return info.Coveragename
                                          End Function

                    Dim groupedCoverages = From c In MyLocation.SectionICoverages
                                           Group c By c.HOM_CoverageType
                                   Into covList = Group

                    For Each covList In groupedCoverages

                        'Dim GetCoverageProperties = Function(sectionIEnum As QuickQuoteSectionICoverage.SectionICoverageType) As CoverageDisplayProperties
                        '                                Return IFM.VR.Common.Helpers.HOM.SectionCoverage.GetCoverageDisplayProperties(quote, MyLocation, SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage, sectionIEnum, QuickQuoteSectionIICoverage.SectionIICoverageType.None, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None, False)
                        '                            End Function


                        Dim loopIndex As Int32 = 0
                        For Each cov In covList.covList
                            Dim prem As String = cov.Premium
                            If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.Premium) = 0.0 Then
                                prem = "Included"
                            End If
                            Dim A_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncluded)
                            Dim A_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncreased)
                            Dim CovALimit As Double = A_included + A_increased
                            Select Case cov.HOM_CoverageType
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Equipment_Breakdown_Coverage
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "50,000", .Premium = prem, .Index = 1, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "N/A", .Premium = prem, .Index = 2, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "N/A", .Premium = prem, .Index = 3, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType),
                                          .Limit = (TryToGetDouble(cov.TotalLimit) + 5000).ToString("N0"),
                                          .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 4, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement
                                    'Added 1/16/18 for HOM Upgrade MLW
                                    If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "N/A", .Premium = prem, .Index = 3, .IsSubItem = False})
                                    End If
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage
                                    'Added 1/16/18 for HOM Upgrade MLW
                                    If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType),
                                                              .Limit = (TryToGetDouble(cov.TotalLimit) + 5000).ToString("N0"),
                                                              .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 303, .IsSubItem = False})
                                    End If
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.RefrigeratedProperty
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "50,000", .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 5, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.DebrisRemoval
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "250", .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 6, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = cov.TotalLimit, .Premium = prem, .Index = 7, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake
                                    Dim deductible As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, cov.DeductibleLimitId)
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = String.Format("{0} - (Deductible {1})", GetCoverageName(cov.HOM_CoverageType), deductible), .Limit = "N/A", .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 8, .IsSubItem = False})

                            'displayIndex As Int32 = 9


                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "N/A", .Premium = prem, .Index = 9, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "N/A", .Premium = "Included", .Index = 10, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BuildingAdditionsAndAlterations
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "N/A", .Premium = prem, .Index = 11, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc ' Added 3-23-16 Matt A - Comp Rater Project
                                    If MyLocation.SectionICoverages IsNot Nothing Then
                                        Dim creditCardCov = (From c In MyLocation.SectionICoverages Where c.HOM_CoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc Select c).FirstOrDefault()
                                        If creditCardCov IsNot Nothing Then
                                            If creditCardCov.IncreasedLimitId.NotEqualsAny("", "0") Then
                                                Dim sc = New IFM.VR.Common.Helpers.HOM.SectionCoverage(creditCardCov)
                                                Dim limit As String = sc.TotalLimit
                                                opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = limit, .Premium = prem, .Index = 12, .IsSubItem = False})
                                            End If
                                        End If
                                    End If

                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = cov.TotalLimit, .Premium = prem, .Index = 13, .IsSubItem = False})


                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.FunctionalReplacementCostLossAssessment
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "N/A", .Premium = prem, .Index = 14, .IsSubItem = False})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment
                                    'moved to section I & II for new forms
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = cov.TotalLimit, .Premium = prem, .Index = 15, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment_Earthquake  'added 3-23-2016 Matt A - Comp Rater Project
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = cov.TotalLimit, .Premium = prem, .Index = 16, .IsSubItem = False})


                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovAAndB
                                    ' limit - lesser of Cov A or $200,000
                                    Dim Limit As String = ""
                                    If CovALimit > 200000 Then
                                        Limit = "$200,000"
                                    Else
                                        Limit = String.Format("{0:C0}", CovALimit)
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = Limit, .Premium = prem, .Index = 17, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovA
                                    ' limit - lesser of Cov A or $200,000
                                    Dim Limit As String = ""
                                    If CovALimit > 200000 Then
                                        Limit = "$200,000"
                                    Else
                                        Limit = String.Format("{0:C0}", CovALimit)
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = Limit, .Premium = prem, .Index = 18, .IsSubItem = False})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SinkholeCollapse
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "N/A", .Premium = prem, .Index = 19, .IsSubItem = False})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialComputerCoverage  'added 3-23-2016 Matt A - Comp Rater Project
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "N/A", .Premium = prem, .Index = 20, .IsSubItem = False})

                                Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures 'added 3-23-2016 Matt A - Comp Rater Project
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = cov.TotalLimit, .Premium = prem, .Index = 21, .IsSubItem = False})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                                    If loopIndex = 0 Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "(see below)", .Premium = "(see below)", .Index = 22, .IsSubItem = False})
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = FormatAddress(cov.Address), .Limit = cov.IncreasedLimit, .Premium = prem, .Index = 22 + (loopIndex + 1), .IsSubItem = True})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TheftofBuildingMaterial
                                    'Updated 12/31/18 for Bug 27386 MLW
                                    'opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "N/A", .Premium = prem, .Index = 300, .IsSubItem = False}) ' .Index jumped(skipped) to make room for an unknown number of Off Premises locations
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = cov.TotalLimit, .Premium = prem, .Index = 300, .IsSubItem = False}) ' .Index jumped(skipped) to make room for an unknown number of Off Premises locations
                            End Select

                            loopIndex += 1 'must be last line before NEXT statement
                        Next
                    Next


                End If

                If MyLocation.SectionIICoverages IsNot Nothing Then
                    Dim multFarm As Boolean = False

                    Dim groupedCoverages = From c In MyLocation.SectionIICoverages
                                           Group c By c.HOM_CoverageType
                                   Into covList = Group

                    Dim GetCoverageName = Function(sectionII_Enum As QuickQuoteSectionIICoverage.HOM_SectionIICoverageType) As String
                                              Dim info = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetCoverageDisplayProperties(quote, MyLocation,
                                                                                                                                SectionCoverage.QuickQuoteSectionCoverageType.SectionIICoverage,
                                                                                                                                QuickQuoteSectionICoverage.SectionICoverageType.None,
                                                                                                                                sectionII_Enum,
                                                                                                                                QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None, False)
                                              Return info.Coveragename
                                          End Function

                    For Each covList In groupedCoverages
                        Dim loopIndex As Int32 = 0
                        For Each cov In covList.covList
                            Dim prem As String = cov.Premium
                            If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.Premium) = 0.0 Then
                                prem = "Included"
                            End If
                            Select Case cov.HOM_CoverageType

                                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured 'added 3-23-2016 Matt A - Comp Rater Project

                                    If loopIndex = 0 Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "(see below)", .Premium = "(see below)", .Index = 1000, .IsSubItem = False})
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = FormatAddress(cov.Address), .Limit = "Included<sup>4</sup>", .Premium = prem, .Index = 1000 + (loopIndex + 1), .IsSubItem = True})


                                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther 'added 3-23-2016 Matt A - Comp Rater Project
                                    If loopIndex = 0 Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "(see below)", .Premium = "(see below)", .Index = 1100, .IsSubItem = False})
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = FormatAddress(cov.Address), .Limit = "Included<sup>4</sup>", .Premium = prem, .Index = 1100 + (loopIndex + 1), .IsSubItem = True})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Clerical 'added 3-23-2016 Matt A - Comp Rater Project
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "Included<sup>4</sup>", .Premium = prem, .Index = 1200, .IsSubItem = False})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                                    If loopIndex = 0 Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "(see <sup>4</sup>)", .Premium = prem, .Index = 1201, .IsSubItem = False})
                                    Else
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "(see <sup>4</sup>)", .Premium = "Included", .Index = 1201 + (loopIndex + 1), .IsSubItem = False})
                                    End If
                                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "Included<sup>4</sup>", .Premium = prem, .Index = 1300, .IsSubItem = False})

                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence 'added 3-23-2016 Matt A - Comp Rater Project
                                    If loopIndex = 0 Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "(see below)", .Premium = "(see below)", .Index = 1301, .IsSubItem = False})
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = FormatAddress(cov.Address), .Limit = "Included<sup>4</sup>", .Premium = prem, .Index = 1301 + (loopIndex + 1), .IsSubItem = True})

                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_ResidencePremises
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "Included<sup>4</sup>", .Premium = prem, .Index = 1400, .IsSubItem = False})

                        'Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_ResidencePremises 'added 3-23-2016 Matt A - Comp Rater Project
                        '    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Permitted Incidental Occupancies - Residence Premises (Liability Only)", .Limit = cov.TotalLimit, .Premium = prem, .Index = 1400, .IsSubItem = False})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PersonalInjury '12-9-14
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(cov.HOM_CoverageType), .Limit = "(see <sup>5</sup>)", .Premium = prem, .Index = 1500, .IsSubItem = False}) ' Jumped .Limit to make room

                            End Select
                            loopIndex += 1 'must be last line before NEXT statement
                        Next
                    Next

                End If



                If MyLocation.SectionIAndIICoverages IsNot Nothing Then

                    Dim groupedCoverages = From c In MyLocation.SectionIAndIICoverages
                                           Group c By c.MainCoverageType
                                   Into covList = Group

                    For Each covList In groupedCoverages
                        Dim loopIndex As Int32 = 0
                        For Each cov In covList.covList
                            Dim prem As String = cov.Premium
                            If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.Premium) = 0.0 Then
                                prem = "Included"
                            End If

                            Select Case cov.MainCoverageType
                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Structures Rented To Others (Property)", .Limit = cov.PropertyTotalLimit, .Premium = prem, .Index = 2002, .IsSubItem = False})
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Structures Rented To Others (Liability)", .Limit = "Included<sup>4</sup>", .Premium = "Included", .Index = 2003, .IsSubItem = False})


                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Permitted Incidental Occupancies - Residence Premises Other Structures (Property)", .Limit = cov.PropertyTotalLimit, .Premium = prem, .Index = 2000, .IsSubItem = False})
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Permitted Incidental Occupancies - Residence Premises Other Structures (Liability)", .Limit = "Included<sup>4</sup>", .Premium = "Included", .Index = 2001, .IsSubItem = False})
                                    'Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers
                            End Select

                            loopIndex += 1 'must be last line before NEXT statement
                        Next
                    Next
                End If

            End If
            Return opCovList
        End Function
        Private Shared Function GetIncludedCoverageListOld(quote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of SummaryCoverageLineItem)
            If quote Is Nothing Then
                Throw New ArgumentNullException(NameOf(quote))
            End If
            If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Dim opCovList As New List(Of SummaryCoverageLineItem)
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If quote IsNot Nothing AndAlso quote.Locations.HasItemAtIndex(0) Then
                Dim MyLocation = quote.Locations(0)
                Dim GetCoverageName = Function(sectionI_Enum As QuickQuoteSectionICoverage.HOM_SectionICoverageType) As String
                                          Dim info = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetCoverageDisplayProperties(quote, MyLocation, SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage, sectionI_Enum, QuickQuoteSectionIICoverage.SectionIICoverageType.None, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None, False)
                                          Return info.Coveragename
                                      End Function

                Dim QQHelper As New QuickQuoteHelperClass
                Dim CurrentForm As String = QQHelper.GetShortFormName(quote)

                If CurrentForm <> "ML-2" And CurrentForm <> "ML-4" Then
                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.BusinessPropertyIncreased), .Limit = "2,500", .Premium = "Included", .IsSubItem = False})

                    If quote.Locations(0).SectionICoverages IsNot Nothing Then
                        Dim creditCardCov = (From cov In quote.Locations(0).SectionICoverages Where cov.HOM_CoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc Select cov).FirstOrDefault()
                        If creditCardCov IsNot Nothing Then
                            If creditCardCov.IncreasedLimitId.EqualsAny("", "0") Then
                                opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc), .Limit = "2,500", .Premium = "Included", .IsSubItem = False})
                            End If
                        End If
                    End If

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Cov. C Increased Special Limits of Liability ", .Limit = "", .Premium = "", .IsSubItem = False})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms), .Limit = "2,000", .Premium = "Included", .IsSubItem = True})
                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs), .Limit = "1,000", .Premium = "Included", .IsSubItem = True})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Money), .Limit = "200", .Premium = "Included", .IsSubItem = True})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Securities), .Limit = "1,000", .Premium = "Included", .IsSubItem = True})
                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.SilverwareGoldwarePewterware), .Limit = "2,500", .Premium = "Included", .IsSubItem = True})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.OrdinanceOrLaw), .Limit = "Included", .Premium = "Included", .IsSubItem = False})

                Else
                    ' if the limit is not increased then show it as included otherwise you will show it below in the optional coverages below - Matt A 3-23-16 Comp Rater Project
                    If MyLocation.SectionICoverages IsNot Nothing Then
                        Dim creditCardCov = (From cov In quote.Locations(0).SectionICoverages Where cov.HOM_CoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc Select cov).FirstOrDefault()
                        If creditCardCov IsNot Nothing Then
                            If creditCardCov.IncreasedLimitId.EqualsAny("", "0") Then
                                opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc), .Limit = "1,000", .Premium = "Included", .IsSubItem = False})
                            End If
                        End If
                    End If


                    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Cov. C Increased Special Limits of Liability ", .Limit = "", .Premium = "", .IsSubItem = False})


                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms), .Limit = "500", .Premium = "Included", .IsSubItem = True})
                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs), .Limit = "500", .Premium = "Included", .IsSubItem = True})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Money), .Limit = "100", .Premium = "Included", .IsSubItem = True})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Securities), .Limit = "500", .Premium = "Included", .IsSubItem = True})
                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.SilverwareGoldwarePewterware), .Limit = "1,000", .Premium = "Included", .IsSubItem = True})

                    'opCovList.Add(New SummaryCoverageLineItem With {.Name = "Ordinance or Law", .Limit = "Included", .Premium = "Included", .IsSubItem = False})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.IncreasedLimitsMotorizedVehicles), .Limit = "1,000", .Premium = "Included", .IsSubItem = False})
                    'Updated 1/19/18 for HOM Upgrade, no longer an included item, but a selected item? Need this line still? MLW
                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge), .Limit = "500", .Premium = "Included", .IsSubItem = False})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Outdoor_Antenna_Satellite_Dish), .Limit = "500", .Premium = "Included", .IsSubItem = False})
                End If
            End If

            Return opCovList
        End Function
#End Region

#Region "OLD PRINT SUMMARY"
        Public Shared Function GetIncludedCoverageList_PrintOld(quote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of SummaryCoverageLineItem)
            If quote Is Nothing Then
                Throw New ArgumentNullException(NameOf(quote))
            End If
            If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Dim opCovList As New List(Of SummaryCoverageLineItem)
            'Updated 8/24/18 for multi state MLW
            ' If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If quote IsNot Nothing AndAlso quote.Locations.HasItemAtIndex(0) Then
                Dim MyLocation = quote.Locations(0)

                Dim GetCoverageName = Function(sectionI_Enum As QuickQuoteSectionICoverage.HOM_SectionICoverageType) As String
                                          Dim info = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetCoverageDisplayProperties(quote, MyLocation, SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage, sectionI_Enum, QuickQuoteSectionIICoverage.SectionIICoverageType.None, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None, True)
                                          Return info.Coveragename
                                      End Function

                'added 11/28/17 for HOM Upgrade MLW
                Dim QQHelper As New QuickQuoteHelperClass
                Dim CurrentForm As String = QQHelper.GetShortFormName(quote)

                'Updated 11/21/17 for HOM Upgrade MLW
                'If quote.Locations(0).FormTypeId <> "6" And quote.Locations(0).FormTypeId <> "7" Then
                If CurrentForm <> "ML-2" And CurrentForm <> "ML-4" Then

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.BusinessPropertyIncreased), .Limit = "2,500", .Premium = "Included", .IsSubItem = False})

                    If quote.Locations(0).SectionICoverages IsNot Nothing Then
                        ''Updated 12/26/17 for HOM Upgrade MLW
                        'If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        'Else
                        Dim creditCardCov = (From cov In quote.Locations(0).SectionICoverages Where cov.HOM_CoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc Select cov).FirstOrDefault()
                        If creditCardCov IsNot Nothing Then
                            If creditCardCov.IncreasedLimitId.EqualsAny("", "0") Then
                                opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc), .Limit = "2,500", .Premium = "Included", .IsSubItem = False})
                            End If
                        End If
                        'End If
                    End If

                    If quote.Locations(0).SectionICoverages IsNot Nothing Then
                        Dim debrisCov = (From cov In quote.Locations(0).SectionICoverages Where cov.HOM_CoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.DebrisRemoval Select cov).FirstOrDefault()
                        If debrisCov IsNot Nothing Then
                            opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.DebrisRemoval), .Limit = "250", .Premium = "Included", .Index = 6, .IsSubItem = False})
                        End If
                    End If

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.OrdinanceOrLaw), .Limit = "Included", .Premium = "Included", .IsSubItem = False})

                    If quote.Locations(0).SectionICoverages IsNot Nothing Then
                        Dim inflationCov = (From cov In quote.Locations(0).SectionICoverages Where cov.HOM_CoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Inflation_Guard Select cov).FirstOrDefault()
                        If inflationCov IsNot Nothing Then
                            opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.SectionICoverageType.Inflation_Guard), .Limit = "N/A", .Premium = "Included", .IsSubItem = False})
                        End If
                    End If




                    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Cov. C Increased Special Limits of Liability ", .Limit = "", .Premium = "", .IsSubItem = False, .NameIsBold = True})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms), .Limit = "2,000", .Premium = "Included", .IsSubItem = True})
                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs), .Limit = "1,000", .Premium = "Included", .IsSubItem = True})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Money), .Limit = "200", .Premium = "Included", .IsSubItem = True})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Securities), .Limit = "1,000", .Premium = "Included", .IsSubItem = True})
                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.SilverwareGoldwarePewterware), .Limit = "2,500", .Premium = "Included", .IsSubItem = True})

                Else
                    'opCovList.Add(New SummaryCoverageLineItem With {.Name = "Business Property Increased Limits", .Limit = "2,500", .Premium = "Included", .IsSubItem = False})

                    ' if the limit is not increased then show it as included otherwise you will show it below in the optional coverages below - Matt A 3-23-16 Comp Rater Project
                    If MyLocation.SectionICoverages IsNot Nothing Then
                        ''Updated 12/26/17 for HOM Upgrade MLW
                        'If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        'Else
                        Dim creditCardCov = (From cov In quote.Locations(0).SectionICoverages Where cov.HOM_CoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc Select cov).FirstOrDefault()
                        If creditCardCov IsNot Nothing Then
                            If creditCardCov.IncreasedLimitId.EqualsAny("", "0") Then
                                opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc), .Limit = "1,000", .Premium = "Included", .IsSubItem = False})
                            End If
                        End If
                        'End If
                    End If

                    If quote.Locations(0).SectionICoverages IsNot Nothing Then
                        Dim debrisCov = (From cov In quote.Locations(0).SectionICoverages Where cov.HOM_CoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.DebrisRemoval Select cov).FirstOrDefault()
                        If debrisCov IsNot Nothing Then
                            opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.DebrisRemoval), .Limit = "250", .Premium = "Included", .Index = 6, .IsSubItem = False})
                        End If
                    End If

                    If quote.Locations(0).SectionICoverages IsNot Nothing Then
                        Dim inflationCov = (From cov In quote.Locations(0).SectionICoverages Where cov.HOM_CoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.Inflation_Guard Select cov).FirstOrDefault()
                        If inflationCov IsNot Nothing Then
                            opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Inflation_Guard), .Limit = "N/A", .Premium = "Included", .IsSubItem = False})
                        End If
                    End If

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Cov. C Increased Special Limits of Liability ", .Limit = "", .Premium = "", .IsSubItem = False})


                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms), .Limit = "500", .Premium = "Included", .IsSubItem = True})
                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs), .Limit = "500", .Premium = "Included", .IsSubItem = True})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Money), .Limit = "100", .Premium = "Included", .IsSubItem = True})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Securities), .Limit = "500", .Premium = "Included", .IsSubItem = True})
                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.SilverwareGoldwarePewterware), .Limit = "1,000", .Premium = "Included", .IsSubItem = True})

                    'opCovList.Add(New SummaryCoverageLineItem With {.Name = "Ordinance or Law", .Limit = "Included", .Premium = "Included", .IsSubItem = False})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.IncreasedLimitsMotorizedVehicles), .Limit = "1,000", .Premium = "Included", .IsSubItem = False})
                    'Added 1/19/18 for HOM Upgrade MLW
                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge), .Limit = "500", .Premium = "Included", .IsSubItem = False})

                    opCovList.Add(New SummaryCoverageLineItem With {.Name = GetCoverageName(QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Outdoor_Antenna_Satellite_Dish), .Limit = "500", .Premium = "Included", .IsSubItem = False})

                End If
            End If
            Return opCovList
        End Function

        Public Shared Function GetLiabilityList_PrintOld(quote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of SummaryCoverageLineItem)
            If quote Is Nothing Then
                Throw New ArgumentNullException(NameOf(quote))
            End If
            If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Dim opCovList As New List(Of SummaryCoverageLineItem)
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If quote IsNot Nothing AndAlso quote.Locations.HasItemAtIndex(0) Then
                Dim MyLocation = quote.Locations(0)
                Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
                If MyLocation.SectionIICoverages IsNot Nothing Then
                    Dim multFarm As Boolean = False

                    Dim groupedCoverages = From c In MyLocation.SectionIICoverages
                                           Group c By c.HOM_CoverageType
                                   Into covList = Group

                    Dim GetCoverageName = Function(sectionII_Enum As QuickQuoteSectionIICoverage.HOM_SectionIICoverageType) As String
                                              Dim info = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetCoverageDisplayProperties(quote, MyLocation,
                                                                                                                                SectionCoverage.QuickQuoteSectionCoverageType.SectionIICoverage,
                                                                                                                                QuickQuoteSectionICoverage.SectionICoverageType.None,
                                                                                                                                sectionII_Enum,
                                                                                                                                QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None, True)
                                              Return info.Coveragename
                                          End Function

                    For Each covList In groupedCoverages
                        Dim loopIndex As Int32 = 0
                        For Each cov In covList.covList
                            Dim prem As String = cov.Premium
                            If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.Premium) = 0.0 Then
                                prem = "Included"
                            End If
                            Dim CoverageName As String = GetCoverageName(cov.HOM_CoverageType)

                            Select Case cov.HOM_CoverageType

                                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured 'added 3-23-2016 Matt A - Comp Rater Project

                                    If loopIndex = 0 Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "", .Premium = "", .Index = 1000, .IsSubItem = False})
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = FormatAddress(cov.Address), .Limit = "see <sup>4</sup>", .Premium = prem, .Index = 1000 + (loopIndex + 1), .IsSubItem = True})


                                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther 'added 3-23-2016 Matt A - Comp Rater Project
                                    If loopIndex = 0 Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "", .Premium = "", .Index = 1100, .IsSubItem = False})
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = FormatAddress(cov.Address), .Limit = "see <sup>4</sup>", .Premium = prem, .Index = 1100 + (loopIndex + 1), .IsSubItem = True})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Clerical 'added 3-23-2016 Matt A - Comp Rater Project
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "see <sup>4</sup>", .Premium = prem, .Index = 1200, .IsSubItem = False})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                                    If loopIndex = 0 Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "see <sup>4</sup>", .Premium = prem, .Index = 1201, .IsSubItem = False})
                                    Else
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "see <sup>4</sup>", .Premium = "Included", .Index = 1201 + (loopIndex + 1), .IsSubItem = False})
                                    End If
                                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "see <sup>4</sup>", .Premium = prem, .Index = 1300, .IsSubItem = False})

                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence 'added 3-23-2016 Matt A - Comp Rater Project
                                    If loopIndex = 0 Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "", .Premium = "", .Index = 1301, .IsSubItem = False})
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = FormatAddress(cov.Address), .Limit = "see <sup>4</sup>", .Premium = prem, .Index = 1301 + (loopIndex + 1), .IsSubItem = True})

                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_ResidencePremises
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "see <sup>4</sup>", .Premium = prem, .Index = 1400, .IsSubItem = False})

                        'Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_ResidencePremises 'added 3-23-2016 Matt A - Comp Rater Project
                        '    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Permitted Incidental Occupancies - Residence Premises (Liability Only)", .Limit = cov.TotalLimit, .Premium = prem, .Index = 1400, .IsSubItem = False})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PersonalInjury '12-9-14

                            End Select
                            loopIndex += 1 'must be last line before NEXT statement
                        Next
                    Next

                End If



                If MyLocation.SectionIAndIICoverages IsNot Nothing Then

                    Dim groupedCoverages = From c In MyLocation.SectionIAndIICoverages
                                           Group c By c.MainCoverageType
                                   Into covList = Group

                    Dim GetCoverageName = Function(sectionIAndII_Enum As QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType) As String
                                              Dim info = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetCoverageDisplayProperties(quote, MyLocation, SectionCoverage.QuickQuoteSectionCoverageType.SectionIAndIICoverage, QuickQuoteSectionICoverage.SectionICoverageType.None, QuickQuoteSectionIICoverage.SectionIICoverageType.None, sectionIAndII_Enum, True)
                                              Return info.Coveragename
                                          End Function

                    For Each covList In groupedCoverages
                        Dim loopIndex As Int32 = 0
                        For Each cov In covList.covList
                            Dim prem As String = cov.Premium
                            If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.Premium) = 0.0 Then
                                prem = "Included"
                            End If
                            Dim CoverageName As String = GetCoverageName(cov.MainCoverageType)

                            Select Case cov.MainCoverageType
                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                                    'opCovList.Add(New SummaryCoverageLineItem With {.Name = "Structures Rented To Others (HO-40) (Property)", .Limit = cov.PropertyTotalLimit, .Premium = prem, .Index = 2002, .IsSubItem = False})
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Structures Rented To Others (HO-40) (Liability)", .Limit = "see <sup>4</sup>", .Premium = "Included", .Index = 2003, .IsSubItem = False})


                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                                    'opCovList.Add(New SummaryCoverageLineItem With {.Name = "Permitted Incidental Occupancies - Residence Premises Other Structures (HO-42) (Property)", .Limit = cov.PropertyTotalLimit, .Premium = prem, .Index = 2000, .IsSubItem = False})
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Permitted Incidental Occupancies - Residence Premises Other Structures (HO-42) (Liability)", .Limit = "see <sup>4</sup>", .Premium = "Included", .Index = 2001, .IsSubItem = False})
                                    'Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers
                            End Select

                            loopIndex += 1 'must be last line before NEXT statement
                        Next
                    Next
                End If

            End If
            Return (From i In opCovList Select i Order By i.Index).ToList()
        End Function

        Public Shared Function GetOtherPropertyList_PrintOld(quote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of SummaryCoverageLineItem)
            If quote Is Nothing Then
                Throw New ArgumentNullException(NameOf(quote))
            End If
            If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            Dim opCovList As New List(Of SummaryCoverageLineItem)
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If quote IsNot Nothing AndAlso quote.Locations.HasItemAtIndex(0) Then
                Dim MyLocation = quote.Locations(0)
                Dim QQHelper As New QuickQuote.CommonMethods.QuickQuoteHelperClass()
                If MyLocation.SectionICoverages IsNot Nothing Then
                    'Dim HomeVersion As String = GetHomeVersion(quote) 'Added 2/26/18 for HOM Upgrade MLW

                    Dim groupedCoverages = From c In MyLocation.SectionICoverages
                                           Group c By c.HOM_CoverageType
                                   Into covList = Group

                    Dim GetCoverageName = Function(sectionI_Enum As QuickQuoteSectionICoverage.HOM_SectionICoverageType) As String
                                              Dim info = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetCoverageDisplayProperties(quote, MyLocation, SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage, sectionI_Enum, QuickQuoteSectionIICoverage.SectionIICoverageType.None, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None, True)
                                              Return info.Coveragename
                                          End Function


                    For Each covList In groupedCoverages

                        'Dim GetCoverageProperties = Function(sectionIEnum As QuickQuoteSectionICoverage.SectionICoverageType) As CoverageDisplayProperties
                        '                                Return IFM.VR.Common.Helpers.HOM.SectionCoverage.GetCoverageDisplayProperties(quote, MyLocation, SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage, sectionIEnum, QuickQuoteSectionIICoverage.SectionIICoverageType.None, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None, False)
                        '                            End Function


                        Dim loopIndex As Int32 = 0
                        For Each cov In covList.covList
                            Dim prem As String = cov.Premium
                            If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.Premium) = 0.0 Then
                                prem = "Included"
                            End If
                            Dim A_included As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncluded)
                            Dim A_increased As Double = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(MyLocation.A_Dwelling_LimitIncreased)
                            Dim CovALimit As Double = A_included + A_increased

                            Dim CoverageName As String = GetCoverageName(cov.HOM_CoverageType)
                            Select Case cov.HOM_CoverageType
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Equipment_Breakdown_Coverage
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "50,000", .Premium = prem, .Index = 1, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "N/A", .Premium = prem, .Index = 2, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "N/A", .Premium = prem, .Index = 3, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName,
                                          .Limit = (TryToGetDouble(cov.TotalLimit) + 5000).ToString("N0"),
                                          .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 4, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement
                                    'Added 1/16/18 for HOM Upgrade MLW
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "N/A", .Premium = prem, .Index = 302, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage
                                    'Added 1/16/18 for HOM Upgrade MLW
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName,
                                          .Limit = (TryToGetDouble(cov.TotalLimit) + 5000).ToString("N0"),
                                          .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 303, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.RefrigeratedProperty
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "50,000", .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 5, .IsSubItem = False})
                                'Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.DebrisRemoval
                                '    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Debris Removal", .Limit = "250", .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 6, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = cov.TotalLimit, .Premium = prem, .Index = 7, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake
                                    Dim deductible As String = QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleLimitId, cov.DeductibleLimitId)
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = String.Format("{0} Deductible {1}", CoverageName, deductible), .Limit = "N/A", .Premium = If(IFM.Common.InputValidation.InputHelpers.TryToGetDouble(prem) > 0, prem, "Included"), .Index = 8, .IsSubItem = False})

                            'displayIndex As Int32 = 9


                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "N/A", .Premium = prem, .Index = 9, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "N/A", .Premium = "Included", .Index = 10, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BuildingAdditionsAndAlterations
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "N/A", .Premium = prem, .Index = 11, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc ' Added 3-23-16 Matt A - Comp Rater Project
                                    If MyLocation.SectionICoverages IsNot Nothing Then
                                        Dim creditCardCov = (From c In MyLocation.SectionICoverages Where c.HOM_CoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc Select c).FirstOrDefault()
                                        If creditCardCov IsNot Nothing Then
                                            If creditCardCov.IncreasedLimitId.NotEqualsAny("", "0") Then
                                                Dim sc = New IFM.VR.Common.Helpers.HOM.SectionCoverage(creditCardCov)
                                                Dim limit As String = sc.TotalLimit
                                                opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = limit, .Premium = prem, .Index = 12, .IsSubItem = False})
                                            End If
                                        End If
                                    End If

                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = cov.TotalLimit, .Premium = prem, .Index = 13, .IsSubItem = False})


                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.FunctionalReplacementCostLossAssessment
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "N/A", .Premium = prem, .Index = 14, .IsSubItem = False})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment
                                    'moved to section I & II on new forms
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = cov.TotalLimit, .Premium = prem, .Index = 15, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment_Earthquake  'added 3-23-2016 Matt A - Comp Rater Project
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = cov.TotalLimit, .Premium = prem, .Index = 16, .IsSubItem = False})


                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovAAndB
                                    ' limit - lesser of Cov A or $200,000
                                    Dim Limit As String = ""
                                    If CovALimit > 200000 Then
                                        Limit = "$200,000"
                                    Else
                                        Limit = String.Format("{0:C0}", CovALimit)
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = Limit, .Premium = prem, .Index = 17, .IsSubItem = False})
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovA
                                    ' limit - lesser of Cov A or $200,000
                                    Dim Limit As String = ""
                                    If CovALimit > 200000 Then
                                        Limit = "$200,000"
                                    Else
                                        Limit = String.Format("{0:C0}", CovALimit)
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = Limit, .Premium = prem, .Index = 18, .IsSubItem = False})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SinkholeCollapse
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "N/A", .Premium = prem, .Index = 19, .IsSubItem = False})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialComputerCoverage  'added 3-23-2016 Matt A - Comp Rater Project
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "N/A", .Premium = prem, .Index = 20, .IsSubItem = False})

                                Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures 'added 3-23-2016 Matt A - Comp Rater Project
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = cov.TotalLimit, .Premium = prem, .Index = 21, .IsSubItem = False})
                                'Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures
                                '    'do not use this - it's old print code, not the new HOM 2011 upgrade code
                                '    'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                                '    'Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises 'replaces Cov_B_Related_Private_Structures
                                '    'Added 2/26/18 for HOM Upgrade MLW
                                '    'If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                '    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = cov.TotalLimit, .Premium = prem, .Index = 21, .IsSubItem = False})
                                '    'End If

                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                                    If loopIndex = 0 Then
                                        opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "", .Premium = "", .Index = 22, .IsSubItem = False})
                                    End If
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = FormatAddress(cov.Address), .Limit = cov.IncreasedLimit, .Premium = prem, .Index = 22 + (loopIndex + 1), .IsSubItem = True})

                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TheftofBuildingMaterial
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = CoverageName, .Limit = "N/A", .Premium = prem, .Index = 300, .IsSubItem = False}) ' .Index jumped(skipped) to make room for an unknown number of Off Premises locations
                            End Select

                            loopIndex += 1 'must be last line before NEXT statement
                        Next
                    Next


                End If





                If MyLocation.SectionIAndIICoverages IsNot Nothing Then

                    Dim groupedCoverages = From c In MyLocation.SectionIAndIICoverages
                                           Group c By c.MainCoverageType
                                   Into covList = Group

                    Dim GetCoverageName = Function(sectionIAndII_Enum As QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType) As String
                                              Dim info = IFM.VR.Common.Helpers.HOM.SectionCoverage.GetCoverageDisplayProperties(quote, MyLocation, SectionCoverage.QuickQuoteSectionCoverageType.SectionIAndIICoverage, QuickQuoteSectionICoverage.SectionICoverageType.None, QuickQuoteSectionIICoverage.SectionIICoverageType.None, sectionIAndII_Enum, True)
                                              Return info.Coveragename
                                          End Function

                    For Each covList In groupedCoverages
                        Dim loopIndex As Int32 = 0
                        For Each cov In covList.covList
                            Dim prem As String = cov.Premium
                            If IFM.Common.InputValidation.InputHelpers.TryToGetDouble(cov.Premium) = 0.0 Then
                                prem = "Included"
                            End If
                            Dim CoverageName As String = GetCoverageName(cov.MainCoverageType)

                            Select Case cov.MainCoverageType
                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Structures Rented To Others (HO-40) (Property)", .Limit = cov.PropertyTotalLimit, .Premium = prem, .Index = 2002, .IsSubItem = False})
                                    'opCovList.Add(New SummaryCoverageLineItem With {.Name = "Structures Rented To Others (HO-40) (Liability)", .Limit = "Included<sup>4</sup>", .Premium = "Included", .Index = 2003, .IsSubItem = False})


                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                                    opCovList.Add(New SummaryCoverageLineItem With {.Name = "Permitted Incidental Occupancies - Residence Premises Other Structures (HO-42) (Property)", .Limit = cov.PropertyTotalLimit, .Premium = prem, .Index = 2000, .IsSubItem = False})
                                    'opCovList.Add(New SummaryCoverageLineItem With {.Name = "Permitted Incidental Occupancies - Residence Premises Other Structures (HO-42) (Liability)", .Limit = "Included<sup>4</sup>", .Premium = "Included", .Index = 2001, .IsSubItem = False})
                                    'Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers
                            End Select

                            loopIndex += 1 'must be last line before NEXT statement
                        Next
                    Next
                End If

            End If
            Return (From i In opCovList Select i Order By i.Index).ToList()
        End Function
#End Region

    End Class

    Public Class SummaryCoverageLineItem
        Public Enum PropertyLiabilityOrBoth
            isProperty
            isLiability
            isBoth
        End Enum
        Public Property Name As String = ""
        Public Property Limit As String = ""
        Public Property Premium As String = ""
        Public Property IsSubItem As Boolean
        Public Property Index As Int32
        Public Property NameIsBold As Boolean
        Public Property IgnoreSort As Boolean
        Public Property SectionCoverageType As Type
        Public Property SubItemId As Integer
        Public Property ParentName As String = ""
        Public Property hasChildren As Boolean
    End Class
End Namespace

