Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store inland marine information
    ''' </summary>
    ''' <remarks>currently used as list object under Location object (<see cref="QuickQuoteLocation"/>)</remarks>
    <Serializable()> _
    Public Class QuickQuoteInlandMarine 'added 8/1/2013 for HOM
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        ''' <summary>
        ''' valid types for inland marine
        ''' </summary>
        ''' <remarks>value previously corresponded to coveragecode_id in Diamond; now see static data xml file</remarks>
        Enum QuickQuoteInlandMarineType 'added 8/6/2013
            'None = 0
            '12/5/2013: without values
            None
            'AdditionalPerilsForSheep = 80086 'Additional Perils for Sheep
            'Airplanes_RemotecontrolAndequipment = 80062 'Airplanes - Remote control and equipment
            'Animals_PetsAndSaddleHorses = 80063 'Animals-Pets and Saddle Horses
            'AntiqueBuggiesWagonsAndSleighs = 80064 'Antique Buggies, Wagons and Sleighs
            'AntiquesWithBreakage = 80065 'Antiques w/Breakage
            'AntiquesWithoutBreakage = 80066 'Antiques w/o Breakage
            'AVEquipment = 80067 'AV Equipment
            'Bicycles = 70077 'Inland_Marine_Bicycles
            'Cameras = 70078 'Inland_Marine_Cameras
            'CardCollection = 80068 'Card Collection
            'Coins = 70079 'Inland_Marine_Coins
            'CollectorsItemsWithoutBreakage = 80069 'Collector's Items W/O Breakage
            'CollectorsItemsWithBreakage = 80070 'Collector's Items With Breakage
            'Computer = 80071 'Computer
            'Farm_NOC = 80097 'Farm - NOC
            'FarmMachinery_Blanket = 80072 'Farm Machinery - Blanket
            'FarmMachineryScheduled = 80073 'Farm Machinery Scheduled
            'FineArtswithBreakage = 70084 'Inland_Marine_Fine_Arts_With_Breakage
            'FineArtswithoutBreakage = 70085 'Inland_Marine_Fine_Arts_Without_Breakage
            'Furs = 70086 'Inland_Marine_Furs
            'GardenTractors = 80074 'Garden Tractors
            'GlassStained = 80075 'Glass, Stained
            'GolfEquipment = 70087 'Inland_Marine_Golf
            'GraveMarkers = 40104 'Grave Markers
            'Guns = 70088 'Inland_Marine_Guns
            'HarnessTackAndOrSulkeyEquipment = 80076 'Harness, Tack and/or Sulkey Equipment
            'HearingAids = 50042 'Hearing Aids
            'IrrigationEquipmentNamedPerils = 80077 'Irrigation Equipment Named Perils
            'IrrigationEquipmentSpecialCoverage = 80078 'Irrigation Equipment Special Coverage
            'Jewelry = 70089 'Inland_Marine_Jewelry
            'JewelryInVault = 20191 'Jewelry In Vault
            'LivestockBlanketCoverage = 80079 'Livestock Blanket Coverage
            'LivestockScheduledCoverage = 80080 'Livestock Scheduled Coverage
            'MedicalItemsAndEquipment = 80081 'Medical Items & Equipment
            'MiscellaneousMusicalPropertyBlanket = 80100 'Miscellaneous Musical Property Blanket
            'MotorTruckCargoCoverage = 80098 'Motor Truck Cargo Coverage
            'MotorTruckCargoCoverage_NamedPerils = 80099 'Motor Truck Cargo Coverage - Named Perils
            'MusicalInstrumentsNon_Professional = 70094 'Inland_Marine_Musical_Instruments_Non_Professional
            'MusicalInstrumentsProfessional = 70093 'Inland_Marine_Musical_Instruments_Professional
            'Personal_NOC = 80101 'Personal-NOC
            'Radios_CB = 80082 'Radios - CB
            'Radios_FM = 80083 'Radios - FM
            'ReproductiveMaterialsNamedPerils = 80084 'Reproductive Materials Named Perils
            'ReproductiveMaterialsSpecialCoverage = 80085 'Reproductive Materials Special Coverage
            'SilverwareGoldware = 70090 'Inland_Marine_Miscellaneous_Class_I
            'SoundReproductionEquipment = 80087 'Sound Reproduction Equipment
            'SportsEquipment = 70312 'Inland Marine Sports Equipment
            'Stamps = 70096 'Inland_Marine_Stamps
            'TelephonesCarOrMobile = 80088 'Telephones, Car or Mobile
            'Telescopes = 80089 'Telescopes
            'ToolsAndEquipment = 70314 'Inland Marine Tools And Equipment

            '12/5/2013: caption w/o values
            'AdditionalPerilsForSheep '80086; Additional Perils for Sheep
            'Airplanes_RemotecontrolAndequipment '80062; Airplanes - Remote control and equipment
            'Animals_PetsAndSaddleHorses '80063; Animals-Pets and Saddle Horses
            'AntiqueBuggiesWagonsAndSleighs '80064; Antique Buggies, Wagons and Sleighs
            'AntiquesWithBreakage '80065; Antiques w/Breakage
            'AntiquesWithoutBreakage '80066; Antiques w/o Breakage
            'AVEquipment '80067; AV Equipment
            'Bicycles '70077; Inland_Marine_Bicycles
            'Cameras '70078; Inland_Marine_Cameras
            'CardCollection '80068; Card Collection
            'Coins '70079; Inland_Marine_Coins
            'CollectorsItemsWithoutBreakage '80069; Collector's Items W/O Breakage
            'CollectorsItemsWithBreakage '80070; Collector's Items With Breakage
            'Computer '80071; Computer
            'Farm_NOC '80097; Farm - NOC
            'FarmMachinery_Blanket '80072; Farm Machinery - Blanket
            'FarmMachineryScheduled '80073; Farm Machinery Scheduled
            'FineArtswithBreakage '70084; Inland_Marine_Fine_Arts_With_Breakage
            'FineArtswithoutBreakage '70085; Inland_Marine_Fine_Arts_Without_Breakage
            'Furs '70086; Inland_Marine_Furs
            'GardenTractors '80074; Garden Tractors
            'GlassStained '80075; Glass, Stained
            'GolfEquipment '70087; Inland_Marine_Golf
            'GraveMarkers '40104; Grave Markers
            'Guns '70088; Inland_Marine_Guns
            'HarnessTackAndOrSulkeyEquipment '80076; Harness, Tack and/or Sulkey Equipment
            'HearingAids '50042; Hearing Aids
            'IrrigationEquipmentNamedPerils '80077; Irrigation Equipment Named Perils
            'IrrigationEquipmentSpecialCoverage '80078; Irrigation Equipment Special Coverage
            'Jewelry '70089; Inland_Marine_Jewelry
            'JewelryInVault '20191; Jewelry In Vault
            'LivestockBlanketCoverage '80079; Livestock Blanket Coverage
            'LivestockScheduledCoverage '80080; Livestock Scheduled Coverage
            'MedicalItemsAndEquipment '80081; Medical Items & Equipment
            'MiscellaneousMusicalPropertyBlanket '80100; Miscellaneous Musical Property Blanket
            'MotorTruckCargoCoverage '80098; Motor Truck Cargo Coverage
            'MotorTruckCargoCoverage_NamedPerils '80099; Motor Truck Cargo Coverage - Named Perils
            'MusicalInstrumentsNon_Professional '70094; Inland_Marine_Musical_Instruments_Non_Professional
            'MusicalInstrumentsProfessional '70093; Inland_Marine_Musical_Instruments_Professional
            'Personal_NOC '80101; Personal-NOC
            'Radios_CB '80082; Radios - CB
            'Radios_FM '80083; Radios - FM
            'ReproductiveMaterialsNamedPerils '80084; Reproductive Materials Named Perils
            'ReproductiveMaterialsSpecialCoverage '80085; Reproductive Materials Special Coverage
            'SilverwareGoldware '70090; Inland_Marine_Miscellaneous_Class_I
            'SoundReproductionEquipment '80087; Sound Reproduction Equipment
            'SportsEquipment '70312; Inland Marine Sports Equipment
            'Stamps '70096; Inland_Marine_Stamps
            'TelephonesCarOrMobile '80088; Telephones, Car or Mobile
            'Telescopes '80089; Telescopes
            'ToolsAndEquipment '70314; Inland Marine Tools And Equipment

            '12/5/2013: coverage code desc w/o values (may use caption for this one since some of the selections have InlandMarine in the front)
            AdditionalPerilsForSheep '80086; Additional Perils for Sheep
            Airplanes_RemotecontrolAndequipment '80062; Airplanes - Remote control and equipment
            Animals_PetsAndSaddleHorses '80063; Animals-Pets and Saddle Horses
            AntiqueBuggiesWagonsAndSleighs '80064; Antique Buggies, Wagons and Sleighs
            AntiquesWithBreakage '80065; Antiques w/Breakage
            AntiquesWithoutBreakage '80066; Antiques w/o Breakage
            AVEquipment '80067; AV Equipment
            CardCollection '80068; Card Collection
            CollectorsItemsWithoutBreakage '80069; Collector's Items W/O Breakage
            CollectorsItemsWithBreakage '80070; Collector's Items With Breakage
            Computer '80071; Computer
            Farm_NOC '80097; Farm - NOC
            FarmMachinery_Blanket '80072; Farm Machinery - Blanket
            FarmMachineryScheduled '80073; Farm Machinery Scheduled
            GardenTractors '80074; Garden Tractors
            GlassStained '80075; Glass, Stained
            GraveMarkers '40104; Grave Markers
            HarnessTackAndOrSulkeyEquipment '80076; Harness, Tack and/or Sulkey Equipment
            HearingAids '50042; Hearing Aids
            InlandMarineSportsEquipment '70312; Sports Equipment
            InlandMarineToolsAndEquipment '70314; Tools and Equipment
            Inland_Marine_Bicycles '70077; Bicycles
            Inland_Marine_Cameras '70078; Cameras
            Inland_Marine_Coins '70079; Coins
            Inland_Marine_Fine_Arts_With_Breakage '70084; Fine Arts with Breakage
            Inland_Marine_Fine_Arts_Without_Breakage '70085; Fine Arts without Breakage
            Inland_Marine_Furs '70086; Furs
            Inland_Marine_Golf '70087; Golf Equipment
            Inland_Marine_Guns '70088; Guns
            Inland_Marine_Jewelry '70089; Jewelry
            Inland_Marine_Miscellaneous_Class_I '70090; Silverware/Goldware
            Inland_Marine_Musical_Instruments_Non_Professional '70094; Musical Instruments Non-Professional
            Inland_Marine_Musical_Instruments_Professional '70093; Musical Instruments Professional
            Inland_Marine_Stamps '70096; Stamps
            IrrigationEquipmentNamedPerils '80077; Irrigation Equipment Named Perils
            IrrigationEquipmentSpecialCoverage '80078; Irrigation Equipment Special Coverage
            JewelryInVault '20191; Jewelry In Vault
            LivestockBlanketCoverage '80079; Livestock Blanket Coverage
            LivestockScheduledCoverage '80080; Livestock Scheduled Coverage
            MedicalItemsAndEquipment '80081; Medical Items & Equipment
            MiscellaneousMusicalPropertyBlanket '80100; Miscellaneous Musical Property Blanket
            MotorTruckCargoCoverage '80098; Motor Truck Cargo Coverage
            MotorTruckCargoCoverage_NamedPerils '80099; Motor Truck Cargo Coverage - Named Perils
            Personal_NOC '80101; Personal-NOC
            Radios_CB '80082; Radios - CB
            Radios_FM '80083; Radios - FM
            ReproductiveMaterialsNamedPerils '80084; Reproductive Materials Named Perils
            ReproductiveMaterialsSpecialCoverage '80085; Reproductive Materials Special Coverage
            SoundReproductionEquipment '80087; Sound Reproduction Equipment
            TelephonesCarOrMobile '80088; Telephones, Car or Mobile
            Telescopes '80089; Telescopes

            'updated 7/16/2015 for Farm
            Animals_SaddleHorses = 80313 'Animals - Saddle Horses

        End Enum

        Dim qqHelper As New QuickQuoteHelperClass 'added 8/6/2013

        Private _PolicyId As String
        Private _PolicyImageNum As String
        Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
        Private _ArtistName As String
        Private _ConsentToRateCoverageEliminated As String
        Private _ConsentToRateCoverageInvolved As String
        Private _Coverage As QuickQuoteCoverage
        Private _Description As String
        Private _MakeBrand As String
        Private _Model As String
        Private _RateInfoAmount As String
        Private _RateInfoDescription As String
        Private _RateInformationTypeId As String 'may need matching RateInformationType variable/property
        Private _SerialNumber As String
        Private _StatedAmount As Boolean
        Private _StorageLocation As String
        Private _Year As String
        'added 8/6/2013
        Private _InlandMarineType As QuickQuoteInlandMarineType
        Private _CoverageCodeId As String
        Private _IncreasedLimit As String
        Private _DeductibleLimitId As String '0, 100, 250, 500, 1000; may need matching DeductibleLimit variable/property

        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014

        Private _InlandMarineNum As String 'added 10/14/2014 for reconciliation

        Private _Premium As String 'added 11/17/2014
        Private _CoveragePremium As String 'added 11/17/2014

        Private _DetailStatusCode As String 'added 5/15/2019

        Public Property PolicyId As String
            Get
                Return _PolicyId
            End Get
            Set(value As String)
                _PolicyId = value
            End Set
        End Property
        Public Property PolicyImageNum As String
            Get
                Return _PolicyImageNum
            End Get
            Set(value As String)
                _PolicyImageNum = value
            End Set
        End Property
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05308}")
                Return _AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05308}")
            End Set
        End Property
        Public Property ArtistName As String
            Get
                Return _ArtistName
            End Get
            Set(value As String)
                _ArtistName = value
            End Set
        End Property
        Public Property ConsentToRateCoverageEliminated As String
            Get
                Return _ConsentToRateCoverageEliminated
            End Get
            Set(value As String)
                _ConsentToRateCoverageEliminated = value
            End Set
        End Property
        Public Property ConsentToRateCoverageInvolved As String
            Get
                Return _ConsentToRateCoverageInvolved
            End Get
            Set(value As String)
                _ConsentToRateCoverageInvolved = value
            End Set
        End Property
        Public Property Coverage As QuickQuoteCoverage
            Get
                SetObjectsParent(_Coverage)
                Return _Coverage
            End Get
            Set(value As QuickQuoteCoverage)
                _Coverage = value
                SetObjectsParent(_Coverage)
            End Set
        End Property
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        Public Property MakeBrand As String
            Get
                Return _MakeBrand
            End Get
            Set(value As String)
                _MakeBrand = value
            End Set
        End Property
        Public Property Model As String
            Get
                Return _Model
            End Get
            Set(value As String)
                _Model = value
            End Set
        End Property
        Public Property RateInfoAmount As String
            Get
                Return _RateInfoAmount
            End Get
            Set(value As String)
                _RateInfoAmount = value
            End Set
        End Property
        Public Property RateInfoDescription As String
            Get
                Return _RateInfoDescription
            End Get
            Set(value As String)
                _RateInfoDescription = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>uses Diamond's RateInformationType table (-1=[blank or empty string], 0=N/A, 1=Standard Rate, 2=Consent to Rate, 3=Individual Risk Filing)</remarks>
        Public Property RateInformationTypeId As String '-1=[blank or empty string]; 0=N/A; 1=Standard Rate; 2=Consent to Rate; 3=Individual Risk Filing
            Get
                Return _RateInformationTypeId
            End Get
            Set(value As String)
                _RateInformationTypeId = value
            End Set
        End Property
        Public Property SerialNumber As String
            Get
                Return _SerialNumber
            End Get
            Set(value As String)
                _SerialNumber = value
            End Set
        End Property
        Public Property StatedAmount As Boolean
            Get
                Return _StatedAmount
            End Get
            Set(value As Boolean)
                _StatedAmount = value
            End Set
        End Property
        Public Property StorageLocation As String
            Get
                Return _StorageLocation
            End Get
            Set(value As String)
                _StorageLocation = value
            End Set
        End Property
        Public Property Year As String
            Get
                Return _Year
            End Get
            Set(value As String)
                _Year = value
            End Set
        End Property
        Public Property InlandMarineType As QuickQuoteInlandMarineType
            Get
                Return _InlandMarineType
            End Get
            Set(value As QuickQuoteInlandMarineType)
                _InlandMarineType = value
                If _InlandMarineType <> Nothing AndAlso _InlandMarineType <> QuickQuoteInlandMarineType.None Then
                    '_CoverageCodeId = CInt(_InlandMarineType).ToString
                    'updated 12/5/2013
                    '_CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.InlandMarineType, _InlandMarineType, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                    'updated 12/20/2013 to send enum text
                    _CoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.InlandMarineType, System.Enum.GetName(GetType(QuickQuoteInlandMarineType), _InlandMarineType), QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId)
                End If
            End Set
        End Property
        Public Property CoverageCodeId As String
            Get
                Return _CoverageCodeId
            End Get
            Set(value As String)
                _CoverageCodeId = value
                If IsNumeric(_CoverageCodeId) = True AndAlso _CoverageCodeId <> "0" Then
                    'If System.Enum.IsDefined(GetType(QuickQuoteInlandMarineType), CInt(_CoverageCodeId)) = True Then
                    '    _InlandMarineType = CInt(_CoverageCodeId)
                    'End If
                    'updated 12/5/2013
                    If System.Enum.TryParse(Of QuickQuoteInlandMarineType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.InlandMarineType), _InlandMarineType) = False Then
                        _InlandMarineType = QuickQuoteInlandMarineType.None
                    End If
                    '12/5/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(QuickQuoteInlandMarineType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.InlandMarineType)) = True Then
                    '    _InlandMarineType = System.Enum.Parse(GetType(QuickQuoteInlandMarineType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteInlandMarine, QuickQuoteHelperClass.QuickQuotePropertyName.CoverageCodeId, _CoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.InlandMarineType))
                    'End If
                End If
            End Set
        End Property
        Public Property IncreasedLimit As String
            Get
                Return _IncreasedLimit
            End Get
            Set(value As String)
                _IncreasedLimit = value
                qqHelper.ConvertToLimitFormat(_IncreasedLimit)
            End Set
        End Property
        Public Property DeductibleLimitId As String
            Get
                Return _DeductibleLimitId
            End Get
            Set(value As String)
                _DeductibleLimitId = value
            End Set
        End Property

        Public Property CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014
            Get
                Return _CanUseAdditionalInterestNumForAdditionalInterestReconciliation
            End Get
            Set(value As Boolean)
                _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = value
            End Set
        End Property

        Public Property InlandMarineNum As String 'added 10/14/2014 for reconciliation
            Get
                Return _InlandMarineNum
            End Get
            Set(value As String)
                _InlandMarineNum = value
            End Set
        End Property

        Public Property Premium As String 'added 11/17/2014
            Get
                Return _Premium
            End Get
            Set(value As String)
                _Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Premium)
            End Set
        End Property
        Public Property CoveragePremium As String 'added 11/17/2014
            Get
                Return _CoveragePremium
            End Get
            Set(value As String)
                _CoveragePremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_CoveragePremium)
            End Set
        End Property

        Public Property DetailStatusCode As String 'added 5/15/2019
            Get
                Return _DetailStatusCode
            End Get
            Set(value As String)
                _DetailStatusCode = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _PolicyId = ""
            _PolicyImageNum = ""
            '_AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing 'added 8/4/2014
            _ArtistName = ""
            _ConsentToRateCoverageEliminated = ""
            _ConsentToRateCoverageInvolved = ""
            _Coverage = New QuickQuoteCoverage
            _Description = ""
            _MakeBrand = ""
            _Model = ""
            _RateInfoAmount = ""
            _RateInfoDescription = ""
            _RateInformationTypeId = ""
            _SerialNumber = ""
            _StatedAmount = False
            _StorageLocation = ""
            _Year = ""
            _InlandMarineType = QuickQuoteInlandMarineType.None
            _CoverageCodeId = ""
            _IncreasedLimit = ""
            _DeductibleLimitId = ""

            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False 'added 4/29/2014

            _InlandMarineNum = "" 'added 10/14/2014 for reconciliation

            _Premium = "" 'added 11/17/2014
            _CoveragePremium = "" 'added 11/17/2014

            _DetailStatusCode = "" 'added 5/15/2019
        End Sub
        'added 8/6/2013
        Public Sub CheckCoverage()
            'If _Coverage IsNot Nothing Then
            'updated to also make sure there's a CoverageCodeId (so it doesn't overwrite CoverageCodeId set w/ InlandMarineType on initial rate attempt; may only want to do when all 3 properties are empty string)
            If _Coverage IsNot Nothing AndAlso _Coverage.CoverageCodeId <> "" Then
                CoverageCodeId = _Coverage.CoverageCodeId
                IncreasedLimit = _Coverage.ManualLimitIncreased
                DeductibleLimitId = _Coverage.CoverageLimitId
                CoveragePremium = _Coverage.FullTermPremium 'added 11/17/2014
            End If

            'added 2/4/2020 for Endorsements so initial Add won't fail on Save due to blank limit
            If String.IsNullOrWhiteSpace(_Description) = False Then
                Dim defaultedLimitText As String = "LIMIT DEFAULTED"
                Dim defaultedLimitTextAppended As String = "; " & defaultedLimitText
                If UCase(_Description).Contains(UCase(defaultedLimitText)) = True Then
                    IncreasedLimit = ""
                    If UCase(_Description).Contains(UCase(defaultedLimitTextAppended)) = True Then
                        _Description = qqHelper.RemoveFirstInstanceOfStringFromString(_Description, defaultedLimitTextAppended)
                    Else
                        _Description = qqHelper.RemoveFirstInstanceOfStringFromString(_Description, defaultedLimitText)
                    End If
                End If
            End If
        End Sub
        'added 4/29/2014 for additionalInterests reconciliation
        Public Sub ParseThruAdditionalInterests()
            If _AdditionalInterests IsNot Nothing AndAlso _AdditionalInterests.Count > 0 Then
                For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                    'note: should only happen when parsing xml (FinalizeQuickQuote method)... shouldn't happen in FinalizeQuickQuoteLight method
                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False Then
                        If ai.HasValidAdditionalInterestNum = True Then
                            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = True
                            Exit For
                        End If
                    End If
                Next
            End If
        End Sub
        Public Function HasValidInlandMarineNum() As Boolean 'added 10/14/2014 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_InlandMarineNum)
        End Function
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.CoverageCodeId <> "" Then
                    Dim im As String = ""
                    im = "CoverageCodeId: " & Me.CoverageCodeId
                    If Me.InlandMarineType <> QuickQuoteInlandMarineType.None Then
                        im &= " (" & System.Enum.GetName(GetType(QuickQuoteInlandMarineType), Me.InlandMarineType) & ")"
                    End If
                    str = qqHelper.appendText(str, im, vbCrLf)
                End If
                If Me.Description <> "" Then
                    str = qqHelper.appendText(str, "Description: " & Me.Description, vbCrLf)
                End If
                If Me.Premium <> "" Then
                    str = qqHelper.appendText(str, "Premium: " & Me.Premium, vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _PolicyId IsNot Nothing Then
                        _PolicyId = Nothing
                    End If
                    If _PolicyImageNum IsNot Nothing Then
                        _PolicyImageNum = Nothing
                    End If
                    If _AdditionalInterests IsNot Nothing Then
                        If _AdditionalInterests.Count > 0 Then
                            For Each ai As QuickQuoteAdditionalInterest In _AdditionalInterests
                                ai.Dispose()
                                ai = Nothing
                            Next
                            _AdditionalInterests.Clear()
                        End If
                        _AdditionalInterests = Nothing
                    End If
                    If _ArtistName IsNot Nothing Then
                        _ArtistName = Nothing
                    End If
                    If _ConsentToRateCoverageEliminated IsNot Nothing Then
                        _ConsentToRateCoverageEliminated = Nothing
                    End If
                    If _ConsentToRateCoverageInvolved IsNot Nothing Then
                        _ConsentToRateCoverageInvolved = Nothing
                    End If
                    If _Coverage IsNot Nothing Then
                        _Coverage.Dispose()
                        _Coverage = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _MakeBrand IsNot Nothing Then
                        _MakeBrand = Nothing
                    End If
                    If _Model IsNot Nothing Then
                        _Model = Nothing
                    End If
                    If _RateInfoAmount IsNot Nothing Then
                        _RateInfoAmount = Nothing
                    End If
                    If _RateInfoDescription IsNot Nothing Then
                        _RateInfoDescription = Nothing
                    End If
                    If _RateInformationTypeId IsNot Nothing Then
                        _RateInformationTypeId = Nothing
                    End If
                    If _SerialNumber IsNot Nothing Then
                        _SerialNumber = Nothing
                    End If
                    If _StatedAmount <> Nothing Then
                        _StatedAmount = Nothing
                    End If
                    If _StorageLocation IsNot Nothing Then
                        _StorageLocation = Nothing
                    End If
                    If _Year IsNot Nothing Then
                        _Year = Nothing
                    End If
                    If _InlandMarineType <> Nothing Then
                        _InlandMarineType = Nothing
                    End If
                    If _CoverageCodeId IsNot Nothing Then
                        _CoverageCodeId = Nothing
                    End If
                    If _IncreasedLimit IsNot Nothing Then
                        _IncreasedLimit = Nothing
                    End If
                    If _DeductibleLimitId IsNot Nothing Then
                        _DeductibleLimitId = Nothing
                    End If

                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation <> Nothing Then 'added 4/29/2014
                        _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    End If

                    If _InlandMarineNum IsNot Nothing Then 'added 10/14/2014 for reconciliation
                        _InlandMarineNum = Nothing
                    End If

                    If _Premium IsNot Nothing Then 'added 11/17/2014
                        _Premium = Nothing
                    End If
                    If _CoveragePremium IsNot Nothing Then 'added 11/17/2014
                        _CoveragePremium = Nothing
                    End If

                    qqHelper.DisposeString(_DetailStatusCode) 'added 5/15/2019

                    MyBase.Dispose() 'added 8/4/2014
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        'Public Sub Dispose() Implements IDisposable.Dispose
        'updated 8/4/2014 w/ QuickQuoteBaseObject inheritance
        Public Overrides Sub Dispose() 'Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
