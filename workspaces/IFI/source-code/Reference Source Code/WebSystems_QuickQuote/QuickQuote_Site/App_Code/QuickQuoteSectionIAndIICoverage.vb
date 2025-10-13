Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to store section I and II (Property and Liability) coverage information
    ''' </summary>
    ''' <remarks>could potentially be under several different objects; currently part of Location</remarks>
    <Serializable()> _
    Public Class QuickQuoteSectionIAndIICoverage 'added 8/13/2013 for HOM
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        ''' <summary>
        ''' valid types for section I and II primary coverages
        ''' </summary>
        ''' <remarks>value previously corresponded to coveragecode_id in Diamond; now see static data xml file; coded for HOM; this primary enum type relates to specific property and liability types</remarks>
        Enum SectionIAndIICoverageType
            'None = 0
            '12/3/2013: without values
            None
            ''PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Liability = 80096 'Permitted Incidental Occupancies Residence Premises - Other Structures - Liability
            ''PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Property = 80095 'Permitted Incidental Occupancies Residence Premises - Other Structures - Property
            'PermittedIncidentalOccupanciesResidencePremises_OtherStructures_HO_42 = 20067 'Permitted Incidental Occupancies Residence Premises - Other Structures
            'StructuresRentedToOthers_HO_40 = 930 'Home - OptLiability - Related Private Structures Rented to Others
            ''StructuresRentedToOthers_Liability = 20092 'Structures Rented to Others - Liability
            ''StructuresRentedToOthers_Property = 20091 'Structures Rented to Others - Property
            'UnitOwnersRentaltoOthers_HO_33 = 20039 'Unit Owners Rental to Others
            ''UnitOwnersRentaltoOthers_Liability = 80148 'Unit Owners Rental to Others - Liability
            ''UnitOwnersRentaltoOthers_Property = 80147 'Unit Owners Rental to Others - Property

            '12/2/2013... in case we ever want to use caption instead of coverage code desc
            'PermittedIncidentalOccupanciesResidencePremises_OtherStructures_HO_42 = 20067 'Permitted Incidental Occupancies Residence Premises - Other Structures
            'StructuresRentedToOthers_HO_40 = 930 'Home - OptLiability - Related Private Structures Rented to Others
            'UnitOwnersRentaltoOthers_HO_33 = 20039 'Unit Owners Rental to Others
            '12/3/2013: without values
            'PermittedIncidentalOccupanciesResidencePremises_OtherStructures_HO_42 '20067; Permitted Incidental Occupancies Residence Premises - Other Structures
            'StructuresRentedToOthers_HO_40 '930; Home - OptLiability - Related Private Structures Rented to Others
            'UnitOwnersRentaltoOthers_HO_33 '20039; Unit Owners Rental to Others

            'updated 12/2/2013 to use coverage code desc instead of caption
            'Home_OptLiability_RelatedPrivateStructuresRentedtoOthers = 930 'Structures Rented To Others (HO-40)
            'PermittedIncidentalOccupanciesResidencePremises_OtherStructures = 20067 'Permitted Incidental Occupancies Residence Premises - Other Structures (HO-42)
            'UnitOwnersRentaltoOthers = 20039 'Unit Owners Rental to Others (HO-33)
            '12/3/2013: without values
            Home_OptLiability_RelatedPrivateStructuresRentedtoOthers '930; Structures Rented To Others (HO-40)
            PermittedIncidentalOccupanciesResidencePremises_OtherStructures '20067; Permitted Incidental Occupancies Residence Premises - Other Structures (HO-42)
            UnitOwnersRentaltoOthers '20039; Unit Owners Rental to Others (HO-33)

            'HOM2018 Upgrade
            AdditionalInsured_StudentLivingAwayFromResidence '80275 (Liability - 80276)(Property - 80277)
            AssistedLivingCareCoverage '80269 (Liability - 80270)(Property - 80271)
            OtherMembersOfYourHousehold '80272 (Liability - 80273)(Property - 80274)
            TrustEndorsement '80266 (Liability - 80525)(Property - 80526)

            LossAssessment '70259 (Liability - 80093)(Property - 80524)
        End Enum
        ''' <summary>
        ''' valid types for section I and II property coverages
        ''' </summary>
        ''' <remarks>value previously corresponded to coveragecode_id in Diamond; now see static data xml file; coded for HOM; this property enum type relates to specific primary and liability types</remarks>
        Enum SectionIAndIIPropertyCoverageType
            'None = 0
            '12/3/2013: without values
            None
            'PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Property = 80095 'Permitted Incidental Occupancies Residence Premises - Other Structures - Property
            'StructuresRentedToOthers_Property = 20091 'Structures Rented to Others - Property
            'UnitOwnersRentaltoOthers_Property = 80147 'Unit Owners Rental to Others - Property

            '12/2/2013... in case we ever want to use caption instead of coverage code desc
            'PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Property = 80095 'Permitted Incidental Occupancies Residence Premises - Other Structures - Property
            'StructuresRentedToOthers_Property = 20091 'Structures Rented to Others - Property
            'UnitOwnersRentaltoOthers_Property = 80147 'Unit Owners Rental to Others - Property
            '12/3/2013: without values
            'PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Property '80095; Permitted Incidental Occupancies Residence Premises - Other Structures - Property
            'StructuresRentedToOthers_Property '20091; Structures Rented to Others - Property
            'UnitOwnersRentaltoOthers_Property '80147; Unit Owners Rental to Others - Property

            'updated 12/2/2013 to use coverage code desc instead of caption
            'PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Property = 80095 'Permitted Incidental Occupancies  Residence Premises  - Other Structures - Property
            'StructuresRentedtoOthers_Property = 20091 'Structures Rented To Others - Property
            'UnitOwnersRentaltoOthers_Property = 80147 'Unit Owners Rental to Others - Property
            '12/3/2013: without values
            PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Property '80095; Permitted Incidental Occupancies  Residence Premises  - Other Structures - Property
            StructuresRentedtoOthers_Property '20091; Structures Rented To Others - Property
            UnitOwnersRentaltoOthers_Property '80147; Unit Owners Rental to Others - Property

            'HOM2018 Upgrade
            AdditionalInsured_StudentLivingAwayFromResidence_Property '80277 (Liability - 80276)(Main - 80275)
            AssistedLivingCareCoverage_Property '80271 (Liability - 80270)(Main - 80269)
            OtherMembersOfYourHousehold_Property '80274 (Liability - 80273)(Main - 80272)
            TrustEndorsement_Property '80526 (Liability - 80525)(Main - 80266)

            LossAssessment_Property '80524 (Liability - 80093)(Main - 70259)
        End Enum
        ''' <summary>
        ''' valid types for section I and II liability coverages
        ''' </summary>
        ''' <remarks>value previously corresponded to coveragecode_id in Diamond; now see static data xml file; coded for HOM; this liability enum type relates to specific primary and property types</remarks>
        Enum SectionIAndIILiabilityCoverageType
            'None = 0
            '12/3/2013: without values
            None
            'PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Liability = 80096 'Permitted Incidental Occupancies Residence Premises - Other Structures - Liability
            'StructuresRentedToOthers_Liability = 20092 'Structures Rented to Others - Liability
            'UnitOwnersRentaltoOthers_Liability = 80148 'Unit Owners Rental to Others - Liability

            '12/2/2013... in case we ever want to use caption instead of coverage code desc
            'PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Liability = 80096 'Permitted Incidental Occupancies Residence Premises - Other Structures - Liability
            'StructuresRentedToOthers_Liability = 20092 'Structures Rented to Others - Liability
            'UnitOwnersRentaltoOthers_Liability = 80148 'Unit Owners Rental to Others - Liability
            '12/3/2013: without values
            'PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Liability '80096; Permitted Incidental Occupancies Residence Premises - Other Structures - Liability
            'StructuresRentedToOthers_Liability '20092; Structures Rented to Others - Liability
            'UnitOwnersRentaltoOthers_Liability '80148; Unit Owners Rental to Others - Liability

            'updated 12/2/2013 to use coverage code desc instead of caption
            'PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Liability = 80096 'Permitted Incidental Occupancies  Residence Premises  - Other Structures - Liability
            'StructuresRentedtoOthers_Liability = 20092 'Structures Rented To Others - Liability
            'UnitOwnersRentaltoOthers_Liability = 80148 'Unit Owners Rental to Others - Liability
            '12/3/2013: without values
            PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Liability '80096; Permitted Incidental Occupancies  Residence Premises  - Other Structures - Liability
            StructuresRentedtoOthers_Liability '20092; Structures Rented To Others - Liability
            UnitOwnersRentaltoOthers_Liability '80148; Unit Owners Rental to Others - Liability

            'HOM2018 Upgrade
            AdditionalInsured_StudentLivingAwayFromResidence_Liability '80276 (Property - 80277)(Main - 80275)
            AssistedLivingCareCoverage_Liability '80270 (Property - 80271)(Main - 80269)
            OtherMembersOfYourHousehold_Liability '80273 (Property - 80274)(Main - 80272)
            TrustEndorsement_Liability '80525 (Property - 80526)(Main - 80266)

            OtherStructuresOnResidencePremises '80527
            LossAssessment_Liability '80093 (Property - 80524)(Main - 70259)
        End Enum

        Dim qqHelper As New QuickQuoteHelperClass

        Private _MainCoverageType As SectionIAndIICoverageType
        Private _MainCoverageCodeId As String
        Private _PropertyCoverageType As SectionIAndIIPropertyCoverageType
        Private _PropertyCoverageCodeId As String
        Private _LiabilityCoverageType As SectionIAndIILiabilityCoverageType
        Private _LiabilityCoverageCodeId As String
        Private _Premium As String
        Private _PropertyIncreasedLimit As String
        Private _PropertyIncreasedLimitId As String
        Private _PropertyIncludedLimit As String 'added 9/10/2014
        Private _PropertyTotalLimit As String 'added 9/10/2014
        Private _Description As String
        Private _Name As QuickQuoteName
        Private _Address As QuickQuoteAddress
        Private _NumberOfFamilies As String
        Private _AdditionalInterests As List(Of QuickQuoteAdditionalInterest)

        Private _CanUseAdditionalInterestNumForAdditionalInterestReconciliation As Boolean 'added 4/29/2014

        'Private _SectionCoverageNum As String 'added 10/14/2014 for reconciliation; removed 10/29/2018
        Private _SectionCoverageNumGroup As QuickQuoteDiamondNumGroup 'added 10/29/2018

        Private _EventEffDate As String
        Private _EventExpDate As String
        Private _LiabilityIncreasedLimit As String
        Private _LiabilityIncreasedLimitId As String
        Private _LiabilityIncludedLimit As String
        Private _LiabilityTotalLimit As String

        Private _UnknownCoverages As List(Of QuickQuoteCoverage) 'added 1/30/2020 w/ Endorsements project

        ''' <summary>
        ''' primary coverage type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>by setting this, the property and liability types will be set automatically</remarks>
        Public Property MainCoverageType As SectionIAndIICoverageType
            Get
                Return _MainCoverageType
            End Get
            Set(value As SectionIAndIICoverageType)
                _MainCoverageType = value
                If _MainCoverageType <> Nothing AndAlso _MainCoverageType <> SectionIAndIICoverageType.None Then
                    '_MainCoverageCodeId = CInt(_MainCoverageType).ToString
                    'updated 12/4/2013
                    '_MainCoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageType, _MainCoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageCodeId)
                    'updated 12/23/2013 to send enum text
                    _MainCoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageType, System.Enum.GetName(GetType(SectionIAndIICoverageType), _MainCoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageCodeId)
                    'If System.Enum.TryParse(Of SectionIAndIIPropertyCoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageType, _MainCoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageType), PropertyCoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of SectionIAndIIPropertyCoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageType, System.Enum.GetName(GetType(SectionIAndIICoverageType), _MainCoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageType), PropertyCoverageType) = False Then
                        PropertyCoverageType = SectionIAndIIPropertyCoverageType.None
                    End If
                    'If System.Enum.TryParse(Of SectionIAndIILiabilityCoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageType, _MainCoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageType), LiabilityCoverageType) = False Then
                    'updated 12/23/2013 to send enum text
                    If System.Enum.TryParse(Of SectionIAndIILiabilityCoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageType, System.Enum.GetName(GetType(SectionIAndIICoverageType), _MainCoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageType), LiabilityCoverageType) = False Then
                        LiabilityCoverageType = SectionIAndIILiabilityCoverageType.None
                    End If
                    'logic for PropertyCoverageType and LiabilityCoverageType sets property value (instead of private variable) so corresponding CoverageCodeId variables will also be set
                End If
                'removed 12/4/2013
                'Select Case _MainCoverageType
                '    Case SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures_HO_42
                '        PropertyCoverageType = SectionIAndIIPropertyCoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Property
                '        LiabilityCoverageType = SectionIAndIILiabilityCoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures_Liability
                '    Case SectionIAndIICoverageType.StructuresRentedToOthers_HO_40
                '        PropertyCoverageType = SectionIAndIIPropertyCoverageType.StructuresRentedToOthers_Property
                '        LiabilityCoverageType = SectionIAndIILiabilityCoverageType.StructuresRentedToOthers_Liability
                '    Case SectionIAndIICoverageType.UnitOwnersRentaltoOthers_HO_33
                '        PropertyCoverageType = SectionIAndIIPropertyCoverageType.UnitOwnersRentaltoOthers_Property
                '        LiabilityCoverageType = SectionIAndIILiabilityCoverageType.UnitOwnersRentaltoOthers_Liability
                '    Case Else
                '        PropertyCoverageType = SectionIAndIIPropertyCoverageType.None
                '        LiabilityCoverageType = SectionIAndIILiabilityCoverageType.None
                'End Select
            End Set
        End Property
        Public Property MainCoverageCodeId As String
            Get
                Return _MainCoverageCodeId
            End Get
            Set(value As String)
                _MainCoverageCodeId = value
                If IsNumeric(_MainCoverageCodeId) = True AndAlso _MainCoverageCodeId <> "0" Then
                    'If System.Enum.IsDefined(GetType(SectionIAndIICoverageType), CInt(_MainCoverageCodeId)) = True Then
                    '    _MainCoverageType = CInt(_MainCoverageCodeId)
                    'End If
                    'updated 12/4/2013
                    If System.Enum.TryParse(Of SectionIAndIICoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageCodeId, _MainCoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageType), _MainCoverageType) = False Then
                        _MainCoverageType = SectionIAndIICoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(SectionIAndIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageCodeId, _MainCoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageType)) = True Then
                    '    _MainCoverageType = System.Enum.Parse(GetType(SectionIAndIICoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageCodeId, _MainCoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageType))
                    'End If
                    'updated 12/5/2013 in case developer is just setting MainCoverageCodeId (setting values for the properties so that the enum type properties will get set too)
                    PropertyCoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageCodeId, _MainCoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageCodeId)
                    LiabilityCoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.MainCoverageCodeId, _MainCoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageCodeId)
                End If
            End Set
        End Property
        ''' <summary>
        ''' property coverage type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>this will be set automatically when primary coverage type is set</remarks>
        Public Property PropertyCoverageType As SectionIAndIIPropertyCoverageType
            Get
                Return _PropertyCoverageType
            End Get
            Set(value As SectionIAndIIPropertyCoverageType)
                _PropertyCoverageType = value
                If _PropertyCoverageType <> Nothing AndAlso _PropertyCoverageType <> SectionIAndIIPropertyCoverageType.None Then
                    '_PropertyCoverageCodeId = CInt(_PropertyCoverageType).ToString
                    'updated 12/4/2013
                    '_PropertyCoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageType, _PropertyCoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageCodeId)
                    'updated 12/23/2013 to send enum text
                    _PropertyCoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageType, System.Enum.GetName(GetType(SectionIAndIIPropertyCoverageType), _PropertyCoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageCodeId)
                End If
            End Set
        End Property
        Public Property PropertyCoverageCodeId As String
            Get
                Return _PropertyCoverageCodeId
            End Get
            Set(value As String)
                _PropertyCoverageCodeId = value
                If IsNumeric(_PropertyCoverageCodeId) = True AndAlso _PropertyCoverageCodeId <> "0" Then
                    'If System.Enum.IsDefined(GetType(SectionIAndIIPropertyCoverageType), CInt(_PropertyCoverageCodeId)) = True Then
                    '    _PropertyCoverageType = CInt(_PropertyCoverageCodeId)
                    'End If
                    'updated 12/4/2013
                    If System.Enum.TryParse(Of SectionIAndIIPropertyCoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageCodeId, _PropertyCoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageType), _PropertyCoverageType) = False Then
                        _PropertyCoverageType = SectionIAndIIPropertyCoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(SectionIAndIIPropertyCoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageCodeId, _PropertyCoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageType)) = True Then
                    '    _PropertyCoverageType = System.Enum.Parse(GetType(SectionIAndIIPropertyCoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageCodeId, _PropertyCoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.PropertyCoverageType))
                    'End If
                End If
            End Set
        End Property
        ''' <summary>
        ''' liability coverage type
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>this will be set automatically when primary coverage type is set</remarks>
        Public Property LiabilityCoverageType As SectionIAndIILiabilityCoverageType
            Get
                Return _LiabilityCoverageType
            End Get
            Set(value As SectionIAndIILiabilityCoverageType)
                _LiabilityCoverageType = value
                If _LiabilityCoverageType <> Nothing AndAlso _LiabilityCoverageType <> SectionIAndIILiabilityCoverageType.None Then
                    '_LiabilityCoverageCodeId = CInt(_LiabilityCoverageType).ToString
                    'updated 12/4/2013
                    '_LiabilityCoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageType, _LiabilityCoverageType, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageCodeId)
                    'updated 12/23/2013 to send enum text
                    _LiabilityCoverageCodeId = qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageType, System.Enum.GetName(GetType(SectionIAndIILiabilityCoverageType), _LiabilityCoverageType), QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageCodeId)
                End If
            End Set
        End Property
        Public Property LiabilityCoverageCodeId As String
            Get
                Return _LiabilityCoverageCodeId
            End Get
            Set(value As String)
                _LiabilityCoverageCodeId = value
                If IsNumeric(_LiabilityCoverageCodeId) = True AndAlso _LiabilityCoverageCodeId <> "0" Then
                    'If System.Enum.IsDefined(GetType(SectionIAndIILiabilityCoverageType), CInt(_LiabilityCoverageCodeId)) = True Then
                    '    _LiabilityCoverageType = CInt(_LiabilityCoverageCodeId)
                    'End If
                    'updated 12/4/2013
                    If System.Enum.TryParse(Of SectionIAndIILiabilityCoverageType)(qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageCodeId, _LiabilityCoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageType), _LiabilityCoverageType) = False Then
                        _LiabilityCoverageType = SectionIAndIILiabilityCoverageType.None
                    End If
                    '12/4/2013 note: could also be written like below since IsDefined will look for match on Enum text or value
                    'If System.Enum.IsDefined(GetType(SectionIAndIILiabilityCoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageCodeId, _LiabilityCoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageType)) = True Then
                    '    _LiabilityCoverageType = System.Enum.Parse(GetType(SectionIAndIILiabilityCoverageType), qqHelper.GetRelatedStaticDataValueForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageCodeId, _LiabilityCoverageCodeId, QuickQuoteHelperClass.QuickQuotePropertyName.LiabilityCoverageType))
                    'End If
                End If
            End Set
        End Property
        Public Property Premium As String
            Get
                'Return _Premium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_Premium)
            End Get
            Set(value As String)
                _Premium = value
                qqHelper.ConvertToQuotedPremiumFormat(_Premium)
            End Set
        End Property
        Public Property PropertyIncreasedLimit As String
            Get
                Return _PropertyIncreasedLimit
            End Get
            Set(value As String)
                _PropertyIncreasedLimit = value
                qqHelper.ConvertToLimitFormat(_PropertyIncreasedLimit)

                Dim options As New List(Of QuickQuoteStaticDataAttribute)
                Dim myOption As New QuickQuoteStaticDataAttribute
                myOption.nvp_name = "CoverageCodeId"
                myOption.nvp_value = _PropertyCoverageCodeId
                options.Add(myOption)
                _PropertyIncreasedLimitId = qqHelper.GetStaticDataValueForText_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, options, _PropertyIncreasedLimit)
            End Set
        End Property
        Public Property PropertyIncreasedLimitId As String
            Get
                Return _PropertyIncreasedLimitId
            End Get
            Set(value As String)
                _PropertyIncreasedLimitId = value

                Dim options As New List(Of QuickQuoteStaticDataAttribute)
                Dim myOption As New QuickQuoteStaticDataAttribute
                myOption.nvp_name = "CoverageCodeId"
                myOption.nvp_value = _PropertyCoverageCodeId
                options.Add(myOption)
                _PropertyIncreasedLimit = qqHelper.GetStaticDataTextForValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, options, _PropertyIncreasedLimitId)
                qqHelper.ConvertToLimitFormat(_PropertyIncreasedLimit)
            End Set
        End Property
        Public Property PropertyIncludedLimit As String 'added 9/10/2014
            Get
                Return _PropertyIncludedLimit
            End Get
            Set(value As String)
                _PropertyIncludedLimit = value
                qqHelper.ConvertToLimitFormat(_PropertyIncludedLimit)
            End Set
        End Property
        Public Property PropertyTotalLimit As String 'added 9/10/2014
            Get
                Return _PropertyTotalLimit
            End Get
            Set(value As String)
                _PropertyTotalLimit = value
                qqHelper.ConvertToLimitFormat(_PropertyTotalLimit)
            End Set
        End Property
        Public Property LiabilityIncreasedLimit As String
            Get
                Return _LiabilityIncreasedLimit
            End Get
            Set(value As String)
                _LiabilityIncreasedLimit = value
                qqHelper.ConvertToLimitFormat(_LiabilityIncreasedLimit)

                Dim options As New List(Of QuickQuoteStaticDataAttribute)
                Dim myOption As New QuickQuoteStaticDataAttribute
                myOption.nvp_name = "CoverageCodeId"
                myOption.nvp_value = _LiabilityCoverageCodeId
                options.Add(myOption)
                _LiabilityIncreasedLimitId = qqHelper.GetStaticDataValueForText_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, options, _LiabilityIncreasedLimit)
            End Set
        End Property
        Public Property LiabilityIncreasedLimitId As String
            Get
                Return _LiabilityIncreasedLimitId
            End Get
            Set(value As String)
                _LiabilityIncreasedLimitId = value

                Dim options As New List(Of QuickQuoteStaticDataAttribute)
                Dim myOption As New QuickQuoteStaticDataAttribute
                myOption.nvp_name = "CoverageCodeId"
                myOption.nvp_value = _LiabilityCoverageCodeId
                options.Add(myOption)
                _LiabilityIncreasedLimit = qqHelper.GetStaticDataTextForValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionIAndIICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, options, _LiabilityIncreasedLimitId)
                qqHelper.ConvertToLimitFormat(_LiabilityIncreasedLimit)
            End Set
        End Property
        Public Property LiabilityIncludedLimit As String 'added 9/10/2014
            Get
                Return _LiabilityIncludedLimit
            End Get
            Set(value As String)
                _LiabilityIncludedLimit = value
                qqHelper.ConvertToLimitFormat(_LiabilityIncludedLimit)
            End Set
        End Property
        Public Property LiabilityTotalLimit As String 'added 9/10/2014
            Get
                Return _LiabilityTotalLimit
            End Get
            Set(value As String)
                _LiabilityTotalLimit = value
                qqHelper.ConvertToLimitFormat(_LiabilityTotalLimit)
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
        Public Property Name As QuickQuoteName
            Get
                SetObjectsParent(_Name)
                Return _Name
            End Get
            Set(value As QuickQuoteName)
                _Name = value
                SetObjectsParent(_Name)
            End Set
        End Property
        Public Property Address As QuickQuoteAddress
            Get
                SetObjectsParent(_Address)
                Return _Address
            End Get
            Set(value As QuickQuoteAddress)
                _Address = value
                SetObjectsParent(_Address)
            End Set
        End Property
        Public Property NumberOfFamilies As String
            Get
                Return _NumberOfFamilies
            End Get
            Set(value As String)
                _NumberOfFamilies = value
            End Set
        End Property
        Public Property AdditionalInterests As List(Of QuickQuoteAdditionalInterest)
            Get
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05630}")
                Return _AdditionalInterests
            End Get
            Set(value As List(Of QuickQuoteAdditionalInterest))
                _AdditionalInterests = value
                SetParentOfListItems(_AdditionalInterests, "{663B7C7B-F2AC-4BF6-965A-D30F41A05630}")
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

        Public Property SectionCoverageNum As String 'added 10/14/2014 for reconciliation
            Get
                'Return _SectionCoverageNum
                'updated 10/29/2018
                Return SectionCoverageNumGroup.DiamondNum
            End Get
            Set(value As String)
                '_SectionCoverageNum = value
                'updated 10/29/2018
                SectionCoverageNumGroup.DiamondNum = value
            End Set
        End Property

        Public Property SectionCoverageNumGroup As QuickQuoteDiamondNumGroup 'added 10/29/2018
            Get
                If _SectionCoverageNumGroup Is Nothing Then
                    _SectionCoverageNumGroup = New QuickQuoteDiamondNumGroup(Me)
                Else
                    SetObjectsParent(_SectionCoverageNumGroup)
                End If
                Return _SectionCoverageNumGroup
            End Get
            Set(value As QuickQuoteDiamondNumGroup)
                _SectionCoverageNumGroup = value
                SetObjectsParent(_SectionCoverageNumGroup)
            End Set
        End Property
        Public Property EventEffDate As String
            Get
                Return _EventEffDate
            End Get
            Set(value As String)
                _EventEffDate = value
            End Set
        End Property

        Public Property EventExpDate As String
            Get
                Return _EventExpDate
            End Get
            Set(value As String)
                _EventExpDate = value
            End Set
        End Property

        Public Property UnknownCoverages As List(Of QuickQuoteCoverage) 'added 1/30/2020 w/ Endorsements project
            Get
                SetParentOfListItems(_UnknownCoverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05627}")
                Return _UnknownCoverages
            End Get
            Set(value As List(Of QuickQuoteCoverage))
                _UnknownCoverages = value
                SetParentOfListItems(_UnknownCoverages, "{663B7C7B-F2AC-4BF6-965A-D30F41A05627}")
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _MainCoverageType = SectionIAndIICoverageType.None
            _MainCoverageCodeId = ""
            _PropertyCoverageType = SectionIAndIIPropertyCoverageType.None
            _PropertyCoverageCodeId = ""
            _LiabilityCoverageType = SectionIAndIILiabilityCoverageType.None
            _LiabilityCoverageCodeId = ""
            _Premium = ""
            _PropertyIncreasedLimit = ""
            _PropertyIncreasedLimitId = ""
            _PropertyIncludedLimit = "" 'added 9/10/2014
            _PropertyTotalLimit = "" 'added 9/10/2014
            _LiabilityIncreasedLimitId = ""
            _LiabilityIncreasedLimit = ""
            _LiabilityIncludedLimit = ""
            _LiabilityTotalLimit = ""
            _Description = ""
            _Name = New QuickQuoteName
            _Name.NameAddressSourceId = "10022" 'Section Coverage
            _Address = New QuickQuoteAddress
            _NumberOfFamilies = ""
            '_AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
            _AdditionalInterests = Nothing 'added 8/4/2014

            _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False 'added 4/29/2014

            '_SectionCoverageNum = "" 'added 10/14/2014 for reconciliation; removed 10/29/2018
            _SectionCoverageNumGroup = New QuickQuoteDiamondNumGroup(Me) 'added 10/29/2018

            _UnknownCoverages = Nothing 'added 1/30/2020 w/ Endorsements project
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
        Public Function HasValidSectionCoverageNum() As Boolean 'added 10/14/2014 for reconciliation purposes
            'Return qqHelper.IsValidQuickQuoteIdOrNum(_SectionCoverageNum)
            'updated 10/29/2018
            Return SectionCoverageNumGroup.HasValidDiamondNum()
        End Function

        Public Overrides Function ToString() As String 'added 6/29/2015
            Dim str As String = ""
            Dim myList As New List(Of String)
            If Me IsNot Nothing Then
                If String.IsNullOrWhiteSpace(MainCoverageCodeId) = False Then
                    str = "CoverageCodeId: " & MainCoverageCodeId & " (" & MainCoverageType & ")"
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
                    If _MainCoverageType <> Nothing Then
                        _MainCoverageType = Nothing
                    End If
                    If _MainCoverageCodeId IsNot Nothing Then
                        _MainCoverageCodeId = Nothing
                    End If
                    If _PropertyCoverageType <= Nothing Then
                        _PropertyCoverageType = Nothing
                    End If
                    If _PropertyCoverageCodeId IsNot Nothing Then
                        _PropertyCoverageCodeId = Nothing
                    End If
                    If _LiabilityCoverageType <= Nothing Then
                        _LiabilityCoverageType = Nothing
                    End If
                    If _LiabilityCoverageCodeId IsNot Nothing Then
                        _LiabilityCoverageCodeId = Nothing
                    End If
                    If _Premium IsNot Nothing Then
                        _Premium = Nothing
                    End If
                    If _PropertyIncreasedLimit IsNot Nothing Then
                        _PropertyIncreasedLimit = Nothing
                    End If
                    If _PropertyIncreasedLimitId IsNot Nothing Then
                        _PropertyIncreasedLimitId = Nothing
                    End If
                    If _PropertyIncludedLimit IsNot Nothing Then 'added 9/10/2014
                        _PropertyIncludedLimit = Nothing
                    End If
                    If _PropertyTotalLimit IsNot Nothing Then 'added 9/10/2014
                        _PropertyTotalLimit = Nothing
                    End If
                    If _LiabilityIncreasedLimit IsNot Nothing Then
                        _LiabilityIncreasedLimit = Nothing
                    End If
                    If _LiabilityIncreasedLimitId IsNot Nothing Then
                        _LiabilityIncreasedLimitId = Nothing
                    End If
                    If _LiabilityIncludedLimit IsNot Nothing Then
                        _LiabilityIncludedLimit = Nothing
                    End If
                    If _LiabilityTotalLimit IsNot Nothing Then
                        _LiabilityTotalLimit = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _Name IsNot Nothing Then
                        _Name.Dispose()
                        _Name = Nothing
                    End If
                    If _Address IsNot Nothing Then
                        _Address.Dispose()
                        _Address = Nothing
                    End If
                    If _NumberOfFamilies IsNot Nothing Then
                        _NumberOfFamilies = Nothing
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

                    If _CanUseAdditionalInterestNumForAdditionalInterestReconciliation <> Nothing Then 'added 4/29/2014
                        _CanUseAdditionalInterestNumForAdditionalInterestReconciliation = Nothing
                    End If

                    'If _SectionCoverageNum IsNot Nothing Then 'added 10/14/2014 for reconciliation; removed 10/29/2018
                    '    _SectionCoverageNum = Nothing
                    'End If
                    qqHelper.DisposeQuickQuoteDiamondNumGroup(_SectionCoverageNumGroup) 'added 10/29/2018

                    qqHelper.DisposeCoverages(_UnknownCoverages) 'added 1/30/2020 w/ Endorsements project

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
