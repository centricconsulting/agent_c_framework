Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.MultiState
'Imports IFM.VR.Common.Helpers.HOM.SectionCoverage
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.HOM

    'Added 2/28/18 for HOM Upgrade MLW

    Public Class HomAppGapCoveragesHelper
        Public Enum QuickQuoteSectionCoverageType
            NotDefined = 0
            SectionICoverage = 1
            SectionIICoverage = 2
            SectionIAndIICoverage = 3
        End Enum
        Dim _sectionCoverage As Object = Nothing
        Dim myType As QuickQuoteSectionCoverageType = QuickQuoteSectionCoverageType.NotDefined

        Protected ReadOnly Property SectionType As HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType
            Get
                Return myType
            End Get
        End Property

        Public Sub New(cov As QuickQuote.CommonObjects.QuickQuoteSectionICoverage)
            myType = QuickQuoteSectionCoverageType.SectionICoverage
            _sectionCoverage = cov
        End Sub
        Public ReadOnly Property SectionCoverageIEnum As QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType
            Get
                If myType = QuickQuoteSectionCoverageType.SectionICoverage Then
                    Return DirectCast(_sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).HOM_CoverageType
                End If
                Return QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.None
            End Get
        End Property

        Public Sub New(cov As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)
            myType = QuickQuoteSectionCoverageType.SectionIICoverage
            _sectionCoverage = cov
        End Sub
        Public ReadOnly Property SectionCoverageIIEnum As QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType
            Get
                If myType = QuickQuoteSectionCoverageType.SectionIICoverage Then
                    Return DirectCast(_sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).HOM_CoverageType
                End If
                Return QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.None
            End Get
        End Property

        Public Sub New(cov As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage)
            myType = QuickQuoteSectionCoverageType.SectionIAndIICoverage
            _sectionCoverage = cov
        End Sub
        Public ReadOnly Property SectionCoverageIAndIIEnum As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType
            Get
                If myType = QuickQuoteSectionCoverageType.SectionIAndIICoverage Then
                    Return DirectCast(_sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).MainCoverageType
                End If
                Return QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None
            End Get
        End Property

        Public Property Description As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).Description
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).Description
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).Description
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).Description = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).Description = value
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).Description = value
                End Select
            End Set
        End Property

        Public Property Address As QuickQuote.CommonObjects.QuickQuoteAddress
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).Address
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).Address
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).Address
                End Select
                Return Nothing
            End Get
            Set(value As QuickQuote.CommonObjects.QuickQuoteAddress)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).Address = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).Address = value
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).Address = value
                End Select
            End Set
        End Property

        Public Property Name As QuickQuote.CommonObjects.QuickQuoteName
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        'Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).Name
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).Name
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).Name
                End Select
                Return Nothing
            End Get
            Set(value As QuickQuote.CommonObjects.QuickQuoteName)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        'DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).Name = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).Name = value
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).Name = value
                End Select
            End Set
        End Property

        Public Property CoverageType As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).HOM_CoverageType
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).HOM_CoverageType
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).MainCoverageType
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).HOM_CoverageType = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).HOM_CoverageType = value
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).MainCoverageType = value
                End Select
            End Set
        End Property

        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).AdditionalInterests
                End Select
                Return Nothing
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Throw New Exception("Additional Interest is not supported on SectionI coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Throw New Exception("Additional Interest is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).AdditionalInterests = value
                End Select
            End Set
        End Property

        Public Property BuildingLimit As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).PropertyIncreasedLimit
                        'Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).PropertyIncludedLimit
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Throw New Exception("Building Total Limit is not supported on SectionI coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Throw New Exception("Building Total Limit is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).PropertyIncreasedLimit = value
                        'DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).PropertyIncludedLimit = value
                End Select
            End Set
        End Property

        Public Shared Function GetHomeVersion(quote As QuickQuoteObject) As String
            Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            Dim effectiveDate As DateTime
            If quote IsNot Nothing Then
                If quote.EffectiveDate IsNot Nothing AndAlso quote.EffectiveDate <> String.Empty Then
                    effectiveDate = quote.EffectiveDate
                Else
                    effectiveDate = Now()
                End If
            Else
                effectiveDate = Now()
            End If
            If qqh.doUseNewVersionOfLOB(quote, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade) = True Then
                'If qqh.doUseNewVersionOfLOB(effectiveDate, quote.LobType, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade) = True Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Function


        Public Shared Function ReturnCommercialName(commName As String) As String
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

        Public Shared Function GetAppGapAIFirstFound(aiList As Object, SectionCoverageIAndIIEnum As QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType) As Int32
            Dim firstFoundIndex As Int32 = 0
            Dim index As Int32 = 0
            For Each ai In aiList
                If ai.Address IsNot Nothing Then
                    If NeedCoverageAddress(ai) Then
                        firstFoundIndex = index
                        Exit For
                    End If
                End If
                index = index + 1
            Next
            Return firstFoundIndex
        End Function

        Public Shared Function GetAppGapAdditionalInterestStatusList(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, covIndex As Int32, cov As Object, mySectionType As QuickQuoteSectionCoverageType, SectionCoverageIAndIIEnum As QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType) As List(Of appGapAIStatusListItem)
            Dim appGapAIStatusList As New List(Of appGapAIStatusListItem)
            Dim covType As QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType = Nothing
            Dim additionalInterestIndex As Int32 = 0
            Dim showAIOnAppGap As Boolean = False
            Dim isFirstOfMultiple As Boolean = False

            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If topQuote IsNot Nothing AndAlso topQuote.Locations.HasItemAtIndex(0) Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Dim MyLocation = topQuote.Locations(0)
                Dim HomeVersion As String = GetHomeVersion(topQuote)
                If (topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso topQuote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Select Case mySectionType
                        Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                            Select Case SectionCoverageIAndIIEnum
                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence,
                                     QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage,
                                     QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement
                                    'additional interest - address only
                                    If cov.AdditionalInterests IsNot Nothing Then
                                        Dim index As Int32 = 0
                                        Dim typeId As Int32 = 0
                                        Dim firstFoundIndex As Int32 = GetAppGapAIFirstFound(cov.AdditionalInterests, SectionCoverageIAndIIEnum)

                                        For Each ai In cov.AdditionalInterests
                                            covType = SectionCoverageIAndIIEnum
                                            additionalInterestIndex = index

                                            If ai.Address IsNot Nothing Then
                                                If NeedCoverageAddress(ai) Then
                                                    showAIOnAppGap = True
                                                Else
                                                    showAIOnAppGap = False
                                                End If
                                            End If

                                            typeId = ai.TypeId

                                            'first index needing info on app gap will need the orange header bar. All others need transparent bar.
                                            If firstFoundIndex = index Then
                                                isFirstOfMultiple = True
                                            Else
                                                isFirstOfMultiple = False
                                            End If

                                            appGapAIStatusList.Add(New appGapAIStatusListItem With {.CoverageType = covType, .CoverageIndex = covIndex, .AdditionalInterestIndex = additionalInterestIndex, .TypeID = typeId, .ShowAIOnAppGap = showAIOnAppGap, .IsFirstOfMultiple = isFirstOfMultiple})
                                            index = index + 1
                                        Next
                                    End If
                            End Select
                    End Select
                End If
            End If

            Return appGapAIStatusList
        End Function

        Public Shared Function GetSelectedCoverageMissingInfoStatus(newCov As Object, myCoverage As Object, mySectionType As QuickQuoteSectionCoverageType, SectionCoverageIEnum As QuickQuoteSectionICoverage.HOM_SectionICoverageType, SectionCoverageIIEnum As QuickQuoteSectionIICoverage.HOM_SectionIICoverageType, SectionCoverageIAndIIEnum As QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType) As Boolean
            'list used to find status of coverages that have multiple items, but not for additional interests
            Dim infoStatus As Boolean = False
            Select Case mySectionType
                Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionICoverage
                    Select Case SectionCoverageIEnum
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                            'CoverageName = "Specific Structures Away from Residence Premises<br />(HO 0492)"
                            'description, address required
                            If IsNullEmptyorWhitespace(newCov.Description) OrElse Left(newCov.Description, 11) = "STRUCTURE #" OrElse newCov.Address Is Nothing OrElse NeedCoverageAddress(newCov) Then
                                infoStatus = True
                            End If
                    End Select
                Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIICoverage
                    Select Case SectionCoverageIIEnum
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured
                            'address required
                            If newCov.Address Is Nothing OrElse NeedCoverageAddress(newCov) Then
                                infoStatus = True
                            End If
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
                            'description, address required
                            If IsNullEmptyorWhitespace(newCov.Description) OrElse Left(newCov.Description, 22) = "ADDITIONAL RESIDENCE #" OrElse newCov.Address Is Nothing OrElse NeedCoverageAddress(newCov) Then
                                infoStatus = True
                            End If
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion
                            'description required
                            If IsNullEmptyorWhitespace(newCov.Description) OrElse Left(newCov.Description, 8) = "CANINE #" Then
                                infoStatus = True
                            End If
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                            'description, address required
                            If IsNullEmptyorWhitespace(newCov.Description) OrElse Left(newCov.Description, 6) = "FARM #" OrElse newCov.Address Is Nothing OrElse NeedCoverageAddress(newCov) Then
                                infoStatus = True
                            End If
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises
                            'description, address required
                            If IsNullEmptyorWhitespace(newCov.Description) OrElse Left(newCov.Description, 23) = "OFF PREMISES LOCATION #" OrElse newCov.Address Is Nothing OrElse NeedCoverageAddress(newCov) Then
                                infoStatus = True
                            End If
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence
                            'description, address required
                            If IsNullEmptyorWhitespace(newCov.Description) OrElse Left(newCov.Description, 24) = "INCIDENTAL OCCUPANCIES #" OrElse newCov.Address Is Nothing OrElse NeedCoverageAddress(newCov) Then
                                infoStatus = True
                            End If
                    End Select
                Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                    Select Case SectionCoverageIAndIIEnum
                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence
                            'address required
                            If newCov.AdditionalInterests(0).Address Is Nothing OrElse NeedCoverageAddress(newCov.AdditionalInterests(0)) Then
                                infoStatus = True
                            End If
                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold
                            'description required
                            If IsNullEmptyorWhitespace(newCov.Description) OrElse Left(newCov.Description, 14) = "OTHER MEMBER #" Then
                                infoStatus = True
                            End If
                    End Select
            End Select

            Return infoStatus
        End Function

        Public Shared Function GetAppGapFirstCoverageStatus(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, coverageIndex As Int32, covList As List(Of Object), myCoverage As Object, mySectionType As QuickQuoteSectionCoverageType, SectionCoverageIEnum As QuickQuoteSectionICoverage.HOM_SectionICoverageType, SectionCoverageIIEnum As QuickQuoteSectionIICoverage.HOM_SectionIICoverageType, SectionCoverageIAndIIEnum As QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType) As Boolean
            Dim isFirst As Boolean = False
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If topQuote IsNot Nothing AndAlso topQuote.Locations.HasItemAtIndex(0) Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Dim MyLocation = topQuote.Locations(0)
                Dim HomeVersion As String = GetHomeVersion(topQuote)
                If (topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso topQuote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                    If SectionCoverageIEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures OrElse
                        SectionCoverageIIEnum = QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability OrElse
                        SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures OrElse
                        SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers OrElse
                        SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage OrElse
                        SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement Then
                        'If SectionCoverageIEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises OrElse
                        'SectionCoverageIIEnum = QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability OrElse
                        'SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures OrElse
                        'SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers OrElse
                        'SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage OrElse
                        'SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement Then
                        '    'SectionCoverageIAndIIEnum = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence OrElse 'removed 4/30/18 for bug 26102 MLW
                        'these coverages will Not have multiples Or are Additional Interests
                        isFirst = True
                    Else
                        Dim currentCoverageType As Object
                        currentCoverageType = myCoverage
                        Dim currentCoverage As Object = Nothing
                        Dim evalCoverage As Object = Nothing

                        Select Case mySectionType
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionICoverage
                                currentCoverage = currentCoverageType.CoverageType
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIICoverage
                                currentCoverage = currentCoverageType.CoverageType
                            Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                                currentCoverage = currentCoverageType.MainCoverageType
                        End Select

                        'make list of coverages of this same coverage type, include its original coverageIndex - will use that later to compare indexes
                        'loop through this list and see if it is the first one missing req'd info, if so, then isFirst = True
                        Dim myCoverageTypeList As New List(Of appGapStatusItem)
                        Dim covListIndex As Int32 = 0
                        For Each cov In covList
                            If cov.GetType Is GetType(QuickQuoteSectionICoverage) Then
                                evalCoverage = cov.CoverageType
                            ElseIf cov.GetType Is GetType(QuickQuoteSectionIICoverage) Then
                                evalCoverage = cov.CoverageType
                            ElseIf cov.GetType Is GetType(QuickQuoteSectionIAndIICoverage) Then
                                evalCoverage = cov.MainCoverageType
                            End If
                            If currentCoverage = evalCoverage Then
                                Dim isMissingInfo As Boolean = GetSelectedCoverageMissingInfoStatus(cov, myCoverage, mySectionType, SectionCoverageIEnum, SectionCoverageIIEnum, SectionCoverageIAndIIEnum)
                                myCoverageTypeList.Add(New appGapStatusItem With {.CoverageType = evalCoverage, .CoverageIndex = covListIndex, .CoverageItem = cov, .MissingInfo = isMissingInfo})
                            End If
                            covListIndex = covListIndex + 1
                        Next

                        If myCoverageTypeList IsNot Nothing Then
                            Dim firstFound = myCoverageTypeList.Find(Function(p) p.MissingInfo = "True")
                            If firstFound IsNot Nothing Then
                                If firstFound.CoverageIndex = coverageIndex Then
                                    isFirst = True
                                End If
                            End If
                        End If
                    End If

                End If
            End If

            Return isFirst
        End Function

        Public Shared Function GetAppGapStatus(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, cov As Object, mySectionType As QuickQuoteSectionCoverageType, SectionCoverageIEnum As QuickQuoteSectionICoverage.HOM_SectionICoverageType, SectionCoverageIIEnum As QuickQuoteSectionIICoverage.HOM_SectionIICoverageType, SectionCoverageIAndIIEnum As QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType) As Boolean
            'determines whether there is a coverage to show on the app gap page
            Dim _chc As New CommonHelperClass
            Dim showOnAppGap As Boolean = False
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If topQuote IsNot Nothing AndAlso topQuote.Locations.HasItemAtIndex(0) Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Dim MyLocation = topQuote.Locations(0)
                Dim HomeVersion As String = GetHomeVersion(topQuote)
                If (topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso topQuote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    Select Case mySectionType
                        Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionICoverage
                            Select Case SectionCoverageIEnum
                                Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures
                                    'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                                    'Case QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises
                                    'description required
                                    If IsNullEmptyorWhitespace(cov.Description) OrElse cov.Description = "OTHER STRUCTURE" Then
                                        showOnAppGap = True
                                    End If
                                Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                                    'CoverageName = "Specific Structures Away from Residence Premises<br />(HO 0492)"
                                    'description, address required
                                    If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 11) = "STRUCTURE #" OrElse cov.Address Is Nothing OrElse NeedCoverageAddress(cov) Then
                                        showOnAppGap = True
                                    End If
                            End Select
                        Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIICoverage
                            Select Case SectionCoverageIIEnum
                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured
                                    'address required
                                    If cov.Address Is Nothing OrElse NeedCoverageAddress(cov) Then
                                        showOnAppGap = True
                                    End If
                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
                                    'description, address required
                                    If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 22) = "ADDITIONAL RESIDENCE #" OrElse cov.Address Is Nothing OrElse NeedCoverageAddress(cov) Then
                                        showOnAppGap = True
                                    End If
                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion
                                    'description required
                                    If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 8) = "CANINE #" Then
                                        showOnAppGap = True
                                    End If
                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                                    'description, address required
                                    If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 6) = "FARM #" OrElse cov.Address Is Nothing OrElse NeedCoverageAddress(cov) Then
                                        showOnAppGap = True
                                    End If
                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability
                                    'description required
                                    If IsNullEmptyorWhitespace(cov.Description) OrElse cov.Description = "PERSONAL LIABILITY" Then
                                        showOnAppGap = True
                                    End If
                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises
                                    'description, address required
                                    If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 23) = "OFF PREMISES LOCATION #" OrElse cov.Address Is Nothing OrElse NeedCoverageAddress(cov) Then
                                        showOnAppGap = True
                                    End If
                                Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence
                                    'description, address required
                                    If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 24) = "INCIDENTAL OCCUPANCIES #" OrElse cov.Address Is Nothing OrElse NeedCoverageAddress(cov) Then
                                        showOnAppGap = True
                                    End If
                            End Select
                        Case HomAppGapCoveragesHelper.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                            Select Case SectionCoverageIAndIIEnum
                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence
                                    'additional interest - relative address only
                                    If cov.AdditionalInterests IsNot Nothing Then
                                        For Each ai In cov.AdditionalInterests
                                            If ai.Address IsNot Nothing Then
                                                If NeedCoverageAddress(ai) Then
                                                    showOnAppGap = True
                                                End If
                                            End If
                                        Next
                                    Else
                                        showOnAppGap = False
                                    End If

                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage
                                    'additional interest - relative address only
                                    If cov.AdditionalInterests IsNot Nothing Then
                                        For Each ai In cov.AdditionalInterests
                                            If ai.Address IsNot Nothing Then
                                                If NeedCoverageAddress(ai) Then
                                                    showOnAppGap = True
                                                End If
                                            End If
                                        Next
                                    Else
                                        showOnAppGap = False
                                    End If
                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold
                                    'description required
                                    If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 14) = "OTHER MEMBER #" Then
                                        showOnAppGap = True
                                    End If
                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                                    'two descriptions required - but uses one field
                                    'Business description and building description both use the description field
                                    Dim txtBusinessDescr As String = Nothing
                                    Dim txtBuildingDescr As String = Nothing
                                    If cov.Description.Contains(vbNewLine) Then
                                        Dim txtDescrSplit As Array = Split(cov.Description, vbNewLine)
                                        txtBusinessDescr = txtDescrSplit(0)
                                        txtBuildingDescr = txtDescrSplit(1)
                                    Else
                                        txtBusinessDescr = cov.Description
                                        txtBuildingDescr = ""
                                    End If
                                    'If IsNullEmptyorWhitespace(cov.Description, txtBusinessDescr, txtBuildingDescr) OrElse txtBusinessDescr = "BUSINESS" OrElse txtBuildingDescr = "BUILDING" Then
                                    If IsNullEmptyorWhitespace(cov.Description, txtBusinessDescr) OrElse txtBusinessDescr = "BUSINESS" Then
                                        showOnAppGap = True
                                    ElseIf _chc.NumericStringComparison(cov.PropertyIncreasedLimit, CommonHelperClass.ComparisonOperators.GreaterThan, 0) AndAlso (IsNullEmptyorWhitespace(txtBuildingDescr) OrElse txtBuildingDescr = "BUILDING") Then
                                        showOnAppGap = True
                                    End If
                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                                    'description required
                                    If IsNullEmptyorWhitespace(cov.Description) OrElse cov.Description = "STRUCTURE" Then
                                        showOnAppGap = True
                                    End If
                                Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement
                                    'additional interest - trust address, trustee address
                                    If cov.AdditionalInterests IsNot Nothing Then
                                        For Each ai In cov.AdditionalInterests
                                            If ai.Address IsNot Nothing Then
                                                If NeedCoverageAddress(ai) Then
                                                    showOnAppGap = True
                                                End If
                                            End If
                                        Next
                                    Else
                                        showOnAppGap = False
                                    End If

                            End Select
                    End Select

                End If
            End If
            Return showOnAppGap
        End Function

        Public Shared Function hasAppGapCoveragesAvailable(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            Dim hasAppGapCov As Boolean = False
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If

                Dim appGapCovList As List(Of Object) = GetAppGapCoveragesAvailable(topQuote)
                If appGapCovList IsNot Nothing AndAlso appGapCovList.Count > 0 Then
                    hasAppGapCov = True
                Else
                    hasAppGapCov = False
                End If
            End If

            Return hasAppGapCov
        End Function
        Public Shared Function GetAppGapCoveragesAvailable(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of Object)
            'list that determines whether there is a coverage to show on the app gap page
            Dim appGapCoverages = New List(Of Object)
            Dim appGapSectionICoverages = New List(Of QuickQuoteSectionICoverage)
            Dim appGapSectionIICoverages = New List(Of QuickQuoteSectionIICoverage)
            Dim appGapSectionIAndIICoverages = New List(Of QuickQuoteSectionIAndIICoverage)
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If topQuote IsNot Nothing AndAlso topQuote.Locations.HasItemAtIndex(0) Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Dim MyLocation = topQuote.Locations(0)
                Dim HomeVersion As String = GetHomeVersion(topQuote)
                If (topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso topQuote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    'section I
                    'Updated 8/24/18 for multi state MLW
                    'If MyLocation.SectionICoverages.IsNotNull Then
                    If MyLocation.SectionICoverages IsNot Nothing Then
                        Dim supportedSectionICoverages = GetSupportedSectionICoverages(topQuote)
                        If supportedSectionICoverages IsNot Nothing AndAlso supportedSectionICoverages.Count > 0 Then
                            For Each supportedCov In supportedSectionICoverages
                                For Each cov In MyLocation.SectionICoverages
                                    If cov.HOM_CoverageType = supportedCov Then
                                        'check for required missing info - if found, add to list
                                        Select Case cov.HOM_CoverageType
                                            Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures
                                                'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                                                'Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises
                                                'description required
                                                If IsNullEmptyorWhitespace(cov.Description) OrElse cov.Description = "OTHER STRUCTURE" Then
                                                    appGapSectionICoverages.Add(cov)
                                                End If
                                            Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                                                'description, address required
                                                If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 11) = "STRUCTURE #" OrElse cov.Address Is Nothing OrElse NeedSectionICoverageAddress(cov) Then
                                                    appGapSectionICoverages.Add(cov)
                                                End If
                                        End Select
                                    End If
                                Next
                            Next
                        End If
                    End If

                    'section II
                    'Updated 8/24/18 for multi state MLW
                    'If MyLocation.SectionIICoverages.IsNotNull Then
                    If MyLocation.SectionIICoverages IsNot Nothing Then
                        Dim supportedSectionIICoverages = GetSupportedSectionIICoverages(topQuote)
                        If supportedSectionIICoverages IsNot Nothing AndAlso supportedSectionIICoverages.Count > 0 Then
                            For Each supportedCov In supportedSectionIICoverages
                                For Each cov In topQuote.Locations(0).SectionIICoverages
                                    If cov.HOM_CoverageType = supportedCov Then
                                        'check for required missing info - if found, add to list
                                        Select Case cov.HOM_CoverageType
                                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured
                                                'address required
                                                If cov.Address Is Nothing OrElse NeedSectionIICoverageAddress(cov) Then
                                                    appGapSectionIICoverages.Add(cov)
                                                End If
                                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
                                                'description, address required
                                                If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 22) = "ADDITIONAL RESIDENCE #" OrElse cov.Address Is Nothing OrElse NeedSectionIICoverageAddress(cov) Then
                                                    appGapSectionIICoverages.Add(cov)
                                                End If
                                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion
                                                'description required
                                                If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 8) = "CANINE #" Then
                                                    appGapSectionIICoverages.Add(cov)
                                                End If
                                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                                                'description, address required
                                                If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 6) = "FARM #" OrElse cov.Address Is Nothing OrElse NeedSectionIICoverageAddress(cov) Then
                                                    appGapSectionIICoverages.Add(cov)
                                                End If
                                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability
                                                'description required
                                                If IsNullEmptyorWhitespace(cov.Description) OrElse cov.Description = "PERSONAL LIABILITY" Then
                                                    appGapSectionIICoverages.Add(cov)
                                                End If
                                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises
                                                'description, address required
                                                If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 23) = "OFF PREMISES LOCATION #" OrElse cov.Address Is Nothing OrElse NeedSectionIICoverageAddress(cov) Then
                                                    appGapSectionIICoverages.Add(cov)
                                                End If
                                            Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence
                                                'description, address required
                                                If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 24) = "INCIDENTAL OCCUPANCIES #" OrElse cov.Address Is Nothing OrElse NeedSectionIICoverageAddress(cov) Then
                                                    appGapSectionIICoverages.Add(cov)
                                                End If
                                        End Select
                                    End If
                                Next
                            Next
                        End If
                    End If

                    'section I&II
                    'Updated 8/24/18 for multi state MLW
                    'If MyLocation.SectionIAndIICoverages.IsNotNull Then
                    If MyLocation.SectionIAndIICoverages IsNot Nothing Then
                        Dim supportedSectionIAndIICoverages = GetSupportedSectionIAndIICoverages(topQuote)
                        If supportedSectionIAndIICoverages IsNot Nothing AndAlso supportedSectionIAndIICoverages.Count > 0 Then
                            For Each supportedCov In supportedSectionIAndIICoverages
                                For Each cov In topQuote.Locations(0).SectionIAndIICoverages
                                    If cov.MainCoverageType = supportedCov Then
                                        'check for required missing info - if found, add to list
                                        Select Case cov.MainCoverageType
                                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence
                                                'additional interest - relative address only
                                                Dim addToAppGap As Boolean = False
                                                If cov.AdditionalInterests IsNot Nothing Then
                                                    For Each ai In cov.AdditionalInterests
                                                        If ai.Address IsNot Nothing Then
                                                            If NeedCoverageAddress(ai) Then
                                                                addToAppGap = True
                                                            End If
                                                        End If
                                                    Next
                                                Else
                                                    addToAppGap = False
                                                End If
                                                If addToAppGap = True Then
                                                    appGapSectionIAndIICoverages.Add(cov)
                                                End If

                                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage
                                                'additional interest - relative address only
                                                Dim addToAppGap As Boolean = False
                                                If cov.AdditionalInterests IsNot Nothing Then
                                                    For Each ai In cov.AdditionalInterests
                                                        If ai.Address IsNot Nothing Then
                                                            If NeedCoverageAddress(ai) Then
                                                                addToAppGap = True
                                                            End If
                                                        End If
                                                    Next
                                                Else
                                                    addToAppGap = False
                                                End If
                                                If addToAppGap = True Then
                                                    appGapSectionIAndIICoverages.Add(cov)
                                                End If

                                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold
                                                'description required
                                                If IsNullEmptyorWhitespace(cov.Description) OrElse Left(cov.Description, 14) = "OTHER MEMBER #" Then
                                                    appGapSectionIAndIICoverages.Add(cov)
                                                End If
                                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                                                'two descriptions required - but uses one field
                                                'Business description and building description both use the description field
                                                Dim txtDescr As String = cov.Description
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
                                                'If IsNullEmptyorWhitespace(cov.Description, txtBusinessDescr, txtBuildingDescr) OrElse txtBusinessDescr = "BUSINESS" OrElse txtBuildingDescr = "BUILDING" Then
                                                If IsNullEmptyorWhitespace(cov.Description, txtBusinessDescr) OrElse txtBusinessDescr = "BUSINESS" OrElse (IsNullEmptyorWhitespace(cov.PropertyIncreasedLimit) = False AndAlso (IsNullEmptyorWhitespace(txtBuildingDescr) OrElse txtBuildingDescr = "BUILDING")) Then
                                                    appGapSectionIAndIICoverages.Add(cov)
                                                End If
                                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                                                'description required
                                                If IsNullEmptyorWhitespace(cov.Description) OrElse cov.Description = "STRUCTURE" Then
                                                    appGapSectionIAndIICoverages.Add(cov)
                                                End If
                                            Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement
                                                'additional interest - trust address, trustee address
                                                Dim addToAppGap As Boolean = False
                                                If cov.AdditionalInterests IsNot Nothing Then
                                                    For Each ai In cov.AdditionalInterests
                                                        If ai.Address IsNot Nothing Then
                                                            If NeedCoverageAddress(ai) Then
                                                                addToAppGap = True
                                                            End If
                                                        End If
                                                    Next
                                                Else
                                                    addToAppGap = False
                                                End If
                                                If addToAppGap = True Then
                                                    appGapSectionIAndIICoverages.Add(cov)
                                                End If
                                        End Select
                                    End If
                                Next
                            Next
                        End If
                    End If

                    'combine into one list
                    Dim myCov As Object
                    For Each scI As QuickQuoteSectionICoverage In appGapSectionICoverages
                        myCov = scI
                        appGapCoverages.Add(myCov)
                    Next
                    For Each scII As QuickQuoteSectionIICoverage In appGapSectionIICoverages
                        myCov = scII
                        appGapCoverages.Add(myCov)
                    Next
                    For Each scIAndII As QuickQuoteSectionIAndIICoverage In appGapSectionIAndIICoverages
                        myCov = scIAndII
                        appGapCoverages.Add(myCov)
                    Next
                End If
            End If
            Return appGapCoverages
        End Function

        Public Shared Function GetAppGapSectionICoverages(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As List(Of QuickQuoteSectionICoverage)
            'list that determines whether there is a coverage to show on the app gap page
            Dim appGapSectionICoverages = New List(Of QuickQuoteSectionICoverage)
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If topQuote IsNot Nothing AndAlso topQuote.Locations.HasItemAtIndex(0) Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Dim MyLocation = topQuote.Locations(0)
                Dim HomeVersion As String = GetHomeVersion(topQuote)
                If (topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso topQuote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    'Updated 8/24/18 for multi state MLW
                    'If MyLocation.SectionICoverages.IsNotNull Then
                    If MyLocation.SectionICoverages IsNot Nothing Then
                        Dim supportedSectionICoverages = GetSupportedSectionICoverages(topQuote)
                        If supportedSectionICoverages IsNot Nothing AndAlso supportedSectionICoverages.Count > 0 Then
                            For Each supportedCov In supportedSectionICoverages
                                For Each cov In topQuote.Locations(0).SectionICoverages
                                    If cov.HOM_CoverageType = supportedCov Then
                                        appGapSectionICoverages.Add(cov)
                                    End If
                                Next
                            Next
                        End If
                    End If
                End If
            End If
            Return appGapSectionICoverages
        End Function



        Public Shared Function GetSupportedSectionICoverages(Optional ByVal topQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing) As List(Of QuickQuoteSectionICoverage.HOM_SectionICoverageType)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            'List that will define the order of coverages shown
            Dim supportedCoverages As New List(Of QuickQuoteSectionICoverage.HOM_SectionICoverageType)
            'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
            'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises) 'Replaces Cov_B_Related_Private_Structures
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises) 'Specified Other Structures - Off Premises (92-147)
            Return supportedCoverages
        End Function

        Public Shared Function GetSupportedSectionIICoverages(Optional ByVal topQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing) As List(Of QuickQuoteSectionIICoverage.HOM_SectionIICoverageType)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            'List that will define the order of coverages shown
            Dim supportedCoverages As New List(Of QuickQuoteSectionIICoverage.HOM_SectionIICoverageType)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability) 'IncidentalFarmersPersonalLiability-On Premises
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence)
            Return supportedCoverages
        End Function

        Public Shared Function GetAppGapSectionIICoverages(Optional ByVal topQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing) As List(Of QuickQuoteSectionIICoverage)
            'list that determines whether there is a coverage to show on the app gap page
            Dim appGapSectionIICoverages = New List(Of QuickQuoteSectionIICoverage)
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.Locations.HasItemAtIndex(0) Then
                    Dim MyLocation = topQuote.Locations(0)
                    Dim HomeVersion As String = GetHomeVersion(topQuote)
                    If (topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso topQuote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        'Updated 8/24/18 for multi state MLW
                        'If MyLocation.SectionIICoverages.IsNotNull Then
                        If MyLocation.SectionIICoverages IsNot Nothing Then
                            Dim supportedSectionIICoverages = GetSupportedSectionIICoverages(topQuote)
                            If supportedSectionIICoverages IsNot Nothing AndAlso supportedSectionIICoverages.Count > 0 Then
                                For Each supportedCov In supportedSectionIICoverages
                                    For Each cov In topQuote.Locations(0).SectionIICoverages
                                        If cov.HOM_CoverageType = supportedCov Then
                                            appGapSectionIICoverages.Add(cov)
                                        End If
                                    Next
                                Next
                            End If
                        End If
                    End If
                End If
            End If
            Return appGapSectionIICoverages
        End Function

        Public Shared Function GetSupportedSectionIAndIICoverages(Optional ByVal topQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing) As List(Of QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            'List that will define the order of coverages shown
            Dim supportedCoverages As New List(Of QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers)
            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement)
            Return supportedCoverages
        End Function

        Public Shared Function GetAppGapSectionIAndIICoverages(Optional ByVal topQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing) As List(Of QuickQuoteSectionIAndIICoverage)
            'list that determines whether there is a coverage to show on the app gap page
            Dim appGapSectionIAndIICoverages = New List(Of QuickQuoteSectionIAndIICoverage)
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso quote.Locations.HasItemAtIndex(0) Then
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                If topQuote.Locations.HasItemAtIndex(0) Then
                    Dim MyLocation = topQuote.Locations(0)
                    Dim HomeVersion As String = GetHomeVersion(topQuote)
                    If (topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso topQuote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                        'Updated 8/24/18 for multi state MLW
                        'If MyLocation.SectionIAndIICoverages.IsNotNull Then
                        If MyLocation.SectionIAndIICoverages IsNot Nothing Then
                            Dim supportedSectionIAndIICoverages = GetSupportedSectionIAndIICoverages(topQuote)
                            If supportedSectionIAndIICoverages IsNot Nothing AndAlso supportedSectionIAndIICoverages.Count > 0 Then
                                For Each supportedCov In supportedSectionIAndIICoverages
                                    For Each cov In topQuote.Locations(0).SectionIAndIICoverages
                                        If cov.MainCoverageType = supportedCov Then
                                            appGapSectionIAndIICoverages.Add(cov)
                                        End If
                                    Next
                                Next
                            End If
                        End If
                    End If
                End If
            End If
            Return appGapSectionIAndIICoverages
        End Function

        Public Shared Function NeedCoverageAddress(cov As Object)
            If IsNullEmptyorWhitespace(cov.Address.HouseNum, cov.Address.StreetName, cov.Address.Zip, cov.Address.City, cov.Address.StateId, cov.Address.County) OrElse cov.Address.HouseNum = "NEED STREET #" OrElse cov.Address.StreetName = "NEED STREET NAME" OrElse cov.Address.Zip = "00001" OrElse cov.Address.Zip = "00001-0000" OrElse cov.Address.City = "NEED CITY" OrElse cov.Address.StateId = "999" OrElse cov.Address.StateId = "0" OrElse cov.Address.County = "NEED COUNTY" Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function NeedSectionICoverageAddress(cov As QuickQuoteSectionICoverage)
            If IsNullEmptyorWhitespace(cov.Address.HouseNum, cov.Address.StreetName, cov.Address.Zip, cov.Address.City, cov.Address.StateId, cov.Address.County) OrElse cov.Address.HouseNum = "NEED STREET #" OrElse cov.Address.StreetName = "NEED STREET NAME" OrElse cov.Address.Zip = "00001" OrElse cov.Address.Zip = "00001-0000" OrElse cov.Address.City = "NEED CITY" OrElse cov.Address.StateId = "999" OrElse cov.Address.StateId = "0" OrElse cov.Address.County = "NEED COUNTY" Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function NeedSectionIICoverageAddress(cov As QuickQuoteSectionIICoverage)
            If IsNullEmptyorWhitespace(cov.Address.HouseNum, cov.Address.StreetName, cov.Address.Zip, cov.Address.City, cov.Address.StateId, cov.Address.County) OrElse cov.Address.HouseNum = "NEED STREET #" OrElse cov.Address.StreetName = "NEED STREET NAME" OrElse cov.Address.Zip = "00001" OrElse cov.Address.Zip = "00001-0000" OrElse cov.Address.City = "NEED CITY" OrElse cov.Address.StateId = "999" OrElse cov.Address.StateId = "0" OrElse cov.Address.County = "NEED COUNTY" Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function NeedSectionIAndIICoverageAddress(cov As QuickQuoteSectionIAndIICoverage)
            If IsNullEmptyorWhitespace(cov.Address.HouseNum, cov.Address.StreetName, cov.Address.Zip, cov.Address.City, cov.Address.StateId, cov.Address.County) OrElse cov.Address.HouseNum = "NEED STREET #" OrElse cov.Address.StreetName = "NEED STREET NAME" OrElse cov.Address.Zip = "00001" OrElse cov.Address.Zip = "00001-0000" OrElse cov.Address.City = "NEED CITY" OrElse cov.Address.StateId = "999" OrElse cov.Address.StateId = "0" OrElse cov.Address.County = "NEED COUNTY" Then
                Return True
            Else
                Return False
            End If
        End Function

    End Class

    Public Class appGapStatusItem
        Public Property CoverageType As Object
        Public Property CoverageIndex As Int32
        Public Property CoverageItem As Object
        Public Property MissingInfo As Boolean
    End Class

    <Serializable()> Public Class appGapAIStatusListItem
        Public Property CoverageType As QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType
        Public Property CoverageIndex As Int32
        Public Property AdditionalInterestIndex As Int32
        Public Property TypeID As Int32
        Public Property ShowAIOnAppGap As Boolean
        Public Property IsFirstOfMultiple As Boolean
    End Class
End Namespace
