Imports System.Configuration
Imports Microsoft.VisualBasic
'added 12/20/2013
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' objects used to store property in the open information
    ''' </summary>
    ''' <remarks>typically found under QuickQuoteLocation object (<see cref="QuickQuoteLocation"/>) as a list</remarks>
    <Serializable()> _
    Public Class QuickQuotePropertyInTheOpenRecord 'added 3/19/2013 for CPR
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _Limit As String
        Private _IncludedInBlanketCoverage As Boolean
        Private _DeductibleId As String
        Private _Deductible As String
        Private _CoinsuranceTypeId As String
        Private _CoinsuranceType As String
        Private _ValuationId As String
        Private _Valuation As String
        Private _ConstructionType As String
        Private _ConstructionTypeId As String
        Private _EarthquakeApplies As Boolean
        Private _EarthquakeQuotedPremium As String
        Private _RatingTypeId As String
        Private _RatingType As String
        Private _CauseOfLossTypeId As String
        Private _CauseOfLossType As String
        Private _QuotedPremium As String
        'Private _ClassificationCode As QuickQuoteClassificationCode
        Private _OptionalTheftDeductibleId As String
        Private _OptionalTheftDeductible As String
        Private _OptionalTheftQuotedPremium As String
        Private _OptionalWindstormOrHailDeductibleId As String
        Private _OptionalWindstormOrHailDeductible As String
        Private _OptionalWindstormOrHailQuotedPremium As String
        Private _FeetToFireHydrant As String
        Private _MilesToFireDepartment As String
        Private _Description As String
        Private _SpecialClassCodeTypeId As String 'SpecialClassCodeType table
        Private _SpecialClassCodeType As String 'added 5/6/2013
        Private _SpecialClassCode As String 'added 5/6/2013
        Private _HasSetSpecialClassCode As String 'added 5/6/2013
        Private _InflationGuardTypeId As String 'InflationGuardType table
        Private _InflationGuardType As String
        Private _ProtectionClass As String
        Private _ProtectionClassId As String
        Private _QuotedPremium_With_EQ As String 'added 4/17/2013

        Private _ScheduledCoverageNum As String 'added 1/22/2015 for reconciliation

        Private _IsAgreedValue As Boolean 'added 1/11/2018 (3/29/2018 in new branch)
        Private _EarthquakeDeductibleId As String
        Private _EarthquakeDeductible As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property Limit As String
            Get
                Return _Limit
            End Get
            Set(value As String)
                _Limit = value
                qqHelper.ConvertToLimitFormat(_Limit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property IncludedInBlanketCoverage As Boolean
            Get
                Return _IncludedInBlanketCoverage
            End Get
            Set(value As Boolean)
                _IncludedInBlanketCoverage = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property DeductibleId As String
            Get
                Return _DeductibleId
            End Get
            Set(value As String)
                _DeductibleId = value
                _Deductible = ""
                If IsNumeric(_DeductibleId) = True Then
                    Select Case _DeductibleId
                        Case "0"
                            _Deductible = "N/A"
                        Case "4"
                            _Deductible = "250"
                        Case "8"
                            _Deductible = "500"
                        Case "9"
                            _Deductible = "1,000"
                        Case "15"
                            _Deductible = "2,500"
                        Case "16"
                            _Deductible = "5,000"
                        Case "17"
                            _Deductible = "10,000"
                        Case "19"
                            _Deductible = "25,000"
                        Case "20"
                            _Deductible = "50,000"
                        Case "21"
                            _Deductible = "75,000"
                        Case "32"
                            _Deductible = "1%"
                        Case "33"
                            _Deductible = "2%"
                        Case "34"
                            _Deductible = "5%"
                        Case "42"
                            _Deductible = "Same"
                        Case "36"
                            _Deductible = "10%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property Deductible As String
            Get
                Return _Deductible
            End Get
            Set(value As String)
                _Deductible = value
                Select Case _Deductible
                    Case "N/A"
                        _DeductibleId = "0"
                    Case "250"
                        _DeductibleId = "4"
                    Case "500"
                        _DeductibleId = "8"
                    Case "1,000"
                        _DeductibleId = "9"
                    Case "2,500"
                        _DeductibleId = "15"
                    Case "5,000"
                        _DeductibleId = "16"
                    Case "10,000"
                        _DeductibleId = "17"
                    Case "25,000"
                        _DeductibleId = "19"
                    Case "50,000"
                        _DeductibleId = "20"
                    Case "75,000"
                        _DeductibleId = "21"
                    Case "1%"
                        _DeductibleId = "32"
                    Case "2%"
                        _DeductibleId = "33"
                    Case "5%"
                        _DeductibleId = "34"
                    Case "Same"
                        _DeductibleId = "42"
                    Case "10%"
                        _DeductibleId = "36"
                    Case Else
                        _DeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property CoinsuranceTypeId As String
            Get
                Return _CoinsuranceTypeId
            End Get
            Set(value As String)
                _CoinsuranceTypeId = value
                _CoinsuranceType = ""
                If IsNumeric(_CoinsuranceTypeId) = True Then
                    Select Case _CoinsuranceTypeId
                        Case "0"
                            _CoinsuranceType = "N/A"
                        Case "1"
                            _CoinsuranceType = "Waived"
                        Case "2"
                            _CoinsuranceType = "50%"
                        Case "3"
                            _CoinsuranceType = "60%"
                        Case "4"
                            _CoinsuranceType = "70%"
                        Case "5"
                            _CoinsuranceType = "80%"
                        Case "6"
                            _CoinsuranceType = "90%"
                        Case "7"
                            _CoinsuranceType = "100%"
                        Case "8"
                            _CoinsuranceType = "10%"
                        Case "9"
                            _CoinsuranceType = "20%"
                        Case "10"
                            _CoinsuranceType = "30%"
                        Case "11"
                            _CoinsuranceType = "40%"
                        Case "12"
                            _CoinsuranceType = "125%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property CoinsuranceType As String
            Get
                Return _CoinsuranceType
            End Get
            Set(value As String)
                _CoinsuranceType = value
                Select Case _CoinsuranceType
                    Case "N/A"
                        _CoinsuranceTypeId = "0"
                    Case "Waived"
                        _CoinsuranceTypeId = "1"
                    Case "50%"
                        _CoinsuranceTypeId = "2"
                    Case "60%"
                        _CoinsuranceTypeId = "3"
                    Case "70%"
                        _CoinsuranceTypeId = "4"
                    Case "80%"
                        _CoinsuranceTypeId = "5"
                    Case "90%"
                        _CoinsuranceTypeId = "6"
                    Case "100%"
                        _CoinsuranceTypeId = "7"
                    Case "10%"
                        _CoinsuranceTypeId = "8"
                    Case "20%"
                        _CoinsuranceTypeId = "9"
                    Case "30%"
                        _CoinsuranceTypeId = "10"
                    Case "40%"
                        _CoinsuranceTypeId = "11"
                    Case "125%"
                        _CoinsuranceTypeId = "12"
                    Case Else
                        _CoinsuranceTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property ValuationId As String
            Get
                Return _ValuationId
            End Get
            Set(value As String)
                _ValuationId = value
                '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
                _Valuation = ""
                If IsNumeric(_ValuationId) = True Then
                    Select Case _ValuationId
                        Case "-1" 'added 10/19/2012 for CPR to match specs
                            _Valuation = "N/A"
                        Case "1"
                            _Valuation = "Replacement Cost"
                        Case "2"
                            _Valuation = "Actual Cash Value"
                        Case "3"
                            _Valuation = "Functional Building Valuation"
                        Case "7" 'added 10/18/2012 for CPR
                            _Valuation = "Functional Replacement Cost"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property Valuation As String
            Get
                Return _Valuation
            End Get
            Set(value As String)
                _Valuation = value
                Select Case _Valuation
                    Case "N/A" 'added 10/19/2012 for CPR to match specs
                        _ValuationId = "-1"
                    Case "Replacement Cost"
                        _ValuationId = "1"
                    Case "Actual Cash Value"
                        _ValuationId = "2"
                    Case "Functional Building Valuation"
                        _ValuationId = "3"
                    Case "Functional Replacement Cost" 'added 10/18/2012 for CPR
                        _ValuationId = "7"
                    Case Else
                        _ValuationId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property ConstructionType As String
            Get
                Return _ConstructionType
            End Get
            Set(value As String)
                _ConstructionType = value
                Select Case _ConstructionType
                    Case "None"
                        _ConstructionTypeId = "0"
                    Case "Frame"
                        _ConstructionTypeId = "1"
                    Case "Joisted Masonry"
                        _ConstructionTypeId = "12"
                    Case "Non-Combustible"
                        _ConstructionTypeId = "13"
                    Case "Masonry Non-Combustible"
                        _ConstructionTypeId = "14"
                    Case "Modified Fire Resistive"
                        _ConstructionTypeId = "15"
                    Case "Fire Resistive"
                        _ConstructionTypeId = "16"
                    Case Else
                        _ConstructionTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property ConstructionTypeId As String 'verified in database 3/20/2013; just using options from UI
            Get
                Return _ConstructionTypeId
            End Get
            Set(value As String)
                _ConstructionTypeId = value
                _ConstructionType = ""
                If IsNumeric(_ConstructionTypeId) = True Then
                    Select Case _ConstructionTypeId
                        Case "0"
                            _ConstructionType = "None"
                        Case "1"
                            _ConstructionType = "Frame"
                        Case "7" 'UI just uses 1
                            _ConstructionType = "Frame"
                        Case "12"
                            _ConstructionType = "Joisted Masonry"
                        Case "13"
                            _ConstructionType = "Non-Combustible"
                        Case "14"
                            _ConstructionType = "Masonry Non-Combustible"
                        Case "15"
                            _ConstructionType = "Modified Fire Resistive"
                        Case "16"
                            _ConstructionType = "Fire Resistive"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21520</remarks>
        Public Property EarthquakeApplies As Boolean
            Get
                Return _EarthquakeApplies
            End Get
            Set(value As Boolean)
                _EarthquakeApplies = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21520</remarks>
        Public Property EarthquakeQuotedPremium As String
            Get
                'Return _EarthquakeQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_EarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _EarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_EarthquakeQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property RatingTypeId As String
            Get
                Return _RatingTypeId
            End Get
            Set(value As String)
                _RatingTypeId = value
                _RatingType = ""
                If IsNumeric(_RatingTypeId) = True Then
                    Select Case _RatingTypeId
                        Case "0"
                            _RatingType = "None"
                        Case "1"
                            _RatingType = "Class Rated"
                        Case "2"
                            _RatingType = "Specific Rated"
                        Case "3"
                            _RatingType = "Special Class Rate"
                        Case "4"
                            _RatingType = "Symbol"
                        Case "5"
                            _RatingType = "Specific"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property RatingType As String
            Get
                Return _RatingType
            End Get
            Set(value As String)
                _RatingType = value
                Select Case _RatingType
                    Case "None"
                        _RatingTypeId = "0"
                    Case "Class Rated"
                        _RatingTypeId = "1"
                    Case "Specific Rated"
                        _RatingTypeId = "2"
                    Case "Special Class Rate"
                        _RatingTypeId = "3"
                    Case "Symbol"
                        _RatingTypeId = "4"
                    Case "Specific"
                        _RatingTypeId = "5"
                    Case Else
                        _RatingTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property CauseOfLossTypeId As String
            Get
                Return _CauseOfLossTypeId
            End Get
            Set(value As String)
                _CauseOfLossTypeId = value
                _CauseOfLossType = ""
                If IsNumeric(_CauseOfLossTypeId) = True Then
                    Select Case _CauseOfLossTypeId
                        Case "0"
                            _CauseOfLossType = "N/A"
                        Case "1"
                            _CauseOfLossType = "Basic Form"
                        Case "2"
                            _CauseOfLossType = "Broad Form"
                        Case "3"
                            _CauseOfLossType = "Special Form Including Theft"
                        Case "4"
                            _CauseOfLossType = "Special Form Excluding Theft"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property CauseOfLossType As String
            Get
                Return _CauseOfLossType
            End Get
            Set(value As String)
                _CauseOfLossType = value
                Select Case _CauseOfLossType
                    Case "N/A"
                        _CauseOfLossTypeId = "0"
                    Case "Basic Form"
                        _CauseOfLossTypeId = "1"
                    Case "Broad Form"
                        _CauseOfLossTypeId = "2"
                    Case "Special Form Including Theft"
                        _CauseOfLossTypeId = "3"
                    Case "Special Form Excluding Theft"
                        _CauseOfLossTypeId = "4"
                    Case Else
                        _CauseOfLossTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property QuotedPremium As String
            Get
                'Return _QuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_QuotedPremium)
            End Get
            Set(value As String)
                _QuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_QuotedPremium)
            End Set
        End Property
        'Public Property ClassificationCode As QuickQuoteClassificationCode
        '    Get
        '        Return _ClassificationCode
        '    End Get
        '    Set(value As QuickQuoteClassificationCode)
        '        _ClassificationCode = value
        '    End Set
        'End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21518</remarks>
        Public Property OptionalTheftDeductibleId As String 'UI doesn't have the % values
            Get
                Return _OptionalTheftDeductibleId
            End Get
            Set(value As String)
                _OptionalTheftDeductibleId = value
                _OptionalTheftDeductible = ""
                If IsNumeric(_OptionalTheftDeductibleId) = True Then
                    Select Case _OptionalTheftDeductibleId
                        Case "0"
                            _OptionalTheftDeductible = "N/A"
                        Case "4"
                            _OptionalTheftDeductible = "250"
                        Case "8"
                            _OptionalTheftDeductible = "500"
                        Case "9"
                            _OptionalTheftDeductible = "1,000"
                        Case "15"
                            _OptionalTheftDeductible = "2,500"
                        Case "16"
                            _OptionalTheftDeductible = "5,000"
                        Case "17"
                            _OptionalTheftDeductible = "10,000"
                        Case "19"
                            _OptionalTheftDeductible = "25,000"
                        Case "20"
                            _OptionalTheftDeductible = "50,000"
                        Case "21"
                            _OptionalTheftDeductible = "75,000"
                        Case "32"
                            _OptionalTheftDeductible = "1%"
                        Case "33"
                            _OptionalTheftDeductible = "2%"
                        Case "34"
                            _OptionalTheftDeductible = "5%"
                        Case "42"
                            _OptionalTheftDeductible = "Same"
                        Case "36"
                            _OptionalTheftDeductible = "10%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21518</remarks>
        Public Property OptionalTheftDeductible As String
            Get
                Return _OptionalTheftDeductible
            End Get
            Set(value As String)
                _OptionalTheftDeductible = value
                Select Case _OptionalTheftDeductible
                    Case "N/A"
                        _OptionalTheftDeductibleId = "0"
                    Case "250"
                        _OptionalTheftDeductibleId = "4"
                    Case "500"
                        _OptionalTheftDeductibleId = "8"
                    Case "1,000"
                        _OptionalTheftDeductibleId = "9"
                    Case "2,500"
                        _OptionalTheftDeductibleId = "15"
                    Case "5,000"
                        _OptionalTheftDeductibleId = "16"
                    Case "10,000"
                        _OptionalTheftDeductibleId = "17"
                    Case "25,000"
                        _OptionalTheftDeductibleId = "19"
                    Case "50,000"
                        _OptionalTheftDeductibleId = "20"
                    Case "75,000"
                        _OptionalTheftDeductibleId = "21"
                    Case "1%"
                        _OptionalTheftDeductibleId = "32"
                    Case "2%"
                        _OptionalTheftDeductibleId = "33"
                    Case "5%"
                        _OptionalTheftDeductibleId = "34"
                    Case "Same"
                        _OptionalTheftDeductibleId = "42"
                    Case "10%"
                        _OptionalTheftDeductibleId = "36"
                    Case Else
                        _OptionalTheftDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21518</remarks>
        Public Property OptionalTheftQuotedPremium As String
            Get
                'Return _OptionalTheftQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_OptionalTheftQuotedPremium)
            End Get
            Set(value As String)
                _OptionalTheftQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OptionalTheftQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21519</remarks>
        Public Property OptionalWindstormOrHailDeductibleId As String
            Get
                Return _OptionalWindstormOrHailDeductibleId
            End Get
            Set(value As String)
                _OptionalWindstormOrHailDeductibleId = value
                _OptionalWindstormOrHailDeductible = ""
                If IsNumeric(_OptionalWindstormOrHailDeductibleId) = True Then
                    Select Case _OptionalWindstormOrHailDeductibleId
                        Case "0"
                            _OptionalWindstormOrHailDeductible = "N/A"
                        Case "4"
                            _OptionalWindstormOrHailDeductible = "250"
                        Case "8"
                            _OptionalWindstormOrHailDeductible = "500"
                        Case "9"
                            _OptionalWindstormOrHailDeductible = "1,000"
                        Case "15"
                            _OptionalWindstormOrHailDeductible = "2,500"
                        Case "16"
                            _OptionalWindstormOrHailDeductible = "5,000"
                        Case "17"
                            _OptionalWindstormOrHailDeductible = "10,000"
                        Case "19"
                            _OptionalWindstormOrHailDeductible = "25,000"
                        Case "20"
                            _OptionalWindstormOrHailDeductible = "50,000"
                        Case "21"
                            _OptionalWindstormOrHailDeductible = "75,000"
                        Case "32"
                            _OptionalWindstormOrHailDeductible = "1%"
                        Case "33"
                            _OptionalWindstormOrHailDeductible = "2%"
                        Case "34"
                            _OptionalWindstormOrHailDeductible = "5%"
                        Case "42"
                            _OptionalWindstormOrHailDeductible = "Same"
                        Case "36"
                            _OptionalWindstormOrHailDeductible = "10%"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21519</remarks>
        Public Property OptionalWindstormOrHailDeductible As String
            Get
                Return _OptionalWindstormOrHailDeductible
            End Get
            Set(value As String)
                _OptionalWindstormOrHailDeductible = value
                Select Case _OptionalWindstormOrHailDeductible
                    Case "N/A"
                        _OptionalWindstormOrHailDeductibleId = "0"
                    Case "250"
                        _OptionalWindstormOrHailDeductibleId = "4"
                    Case "500"
                        _OptionalWindstormOrHailDeductibleId = "8"
                    Case "1,000"
                        _OptionalWindstormOrHailDeductibleId = "9"
                    Case "2,500"
                        _OptionalWindstormOrHailDeductibleId = "15"
                    Case "5,000"
                        _OptionalWindstormOrHailDeductibleId = "16"
                    Case "10,000"
                        _OptionalWindstormOrHailDeductibleId = "17"
                    Case "25,000"
                        _OptionalWindstormOrHailDeductibleId = "19"
                    Case "50,000"
                        _OptionalWindstormOrHailDeductibleId = "20"
                    Case "75,000"
                        _OptionalWindstormOrHailDeductibleId = "21"
                    Case "1%"
                        _OptionalWindstormOrHailDeductibleId = "32"
                    Case "2%"
                        _OptionalWindstormOrHailDeductibleId = "33"
                    Case "5%"
                        _OptionalWindstormOrHailDeductibleId = "34"
                    Case "Same"
                        _OptionalWindstormOrHailDeductibleId = "42"
                    Case "10%"
                        _OptionalWindstormOrHailDeductibleId = "36"
                    Case Else
                        _OptionalWindstormOrHailDeductibleId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21519</remarks>
        Public Property OptionalWindstormOrHailQuotedPremium As String
            Get
                'Return _OptionalWindstormOrHailQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_OptionalWindstormOrHailQuotedPremium)
            End Get
            Set(value As String)
                _OptionalWindstormOrHailQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_OptionalWindstormOrHailQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property FeetToFireHydrant As String
            Get
                Return _FeetToFireHydrant
            End Get
            Set(value As String)
                _FeetToFireHydrant = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property MilesToFireDepartment As String
            Get
                Return _MilesToFireDepartment
            End Get
            Set(value As String)
                _MilesToFireDepartment = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property SpecialClassCodeTypeId As String
            Get
                Return _SpecialClassCodeTypeId
            End Get
            Set(value As String)
                _SpecialClassCodeTypeId = value
                _HasSetSpecialClassCode = False
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public ReadOnly Property SpecialClassCodeType As String
            Get
                If _HasSetSpecialClassCode = False Then
                    SetSpecialClassCodeType()
                End If
                Return _SpecialClassCodeType
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public ReadOnly Property SpecialClassCode As String
            Get
                If _HasSetSpecialClassCode = False Then
                    SetSpecialClassCodeType()
                End If
                Return _SpecialClassCode
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property InflationGuardTypeId As String
            Get
                Return _InflationGuardTypeId
            End Get
            Set(value As String)
                _InflationGuardTypeId = value
                _InflationGuardType = ""
                If IsNumeric(_InflationGuardTypeId) = True Then
                    Select Case _InflationGuardTypeId
                        Case "0"
                            _InflationGuardType = "N/A"
                        Case "1"
                            _InflationGuardType = "2"
                        Case "2"
                            _InflationGuardType = "4"
                        Case "3"
                            _InflationGuardType = "6"
                        Case "4"
                            _InflationGuardType = "8"
                        Case "5"
                            _InflationGuardType = "10"
                        Case "6"
                            _InflationGuardType = "12"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property InflationGuardType As String
            Get
                Return _InflationGuardType
            End Get
            Set(value As String)
                _InflationGuardType = value
                Select Case _InflationGuardType
                    Case "N/A"
                        _InflationGuardTypeId = "0"
                    Case "2"
                        _InflationGuardTypeId = "1"
                    Case "4"
                        _InflationGuardTypeId = "2"
                    Case "6"
                        _InflationGuardTypeId = "3"
                    Case "8"
                        _InflationGuardTypeId = "4"
                    Case "10"
                        _InflationGuardTypeId = "5"
                    Case "12"
                        _InflationGuardTypeId = "6"
                    Case Else
                        _InflationGuardTypeId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property ProtectionClass As String
            Get
                Return _ProtectionClass
            End Get
            Set(value As String)
                _ProtectionClass = value
                Select Case _ProtectionClass
                    Case "01"
                        _ProtectionClassId = "12"
                    Case "02"
                        _ProtectionClassId = "13"
                    Case "03"
                        _ProtectionClassId = "14"
                    Case "04"
                        _ProtectionClassId = "15"
                    Case "05"
                        _ProtectionClassId = "16"
                    Case "06"
                        _ProtectionClassId = "17"
                    Case "07"
                        _ProtectionClassId = "18"
                    Case "08"
                        _ProtectionClassId = "19"
                    Case "8B"
                        _ProtectionClassId = "20"
                    Case "09"
                        _ProtectionClassId = "21"
                    Case "10"
                        _ProtectionClassId = "22"
                    Case Else 'added 5/2/2013
                        _ProtectionClassId = ""
                End Select
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21107</remarks>
        Public Property ProtectionClassId As String 'verified in database 7/3/2012
            Get
                Return _ProtectionClassId
            End Get
            Set(value As String)
                _ProtectionClassId = value
                _ProtectionClass = ""
                If IsNumeric(_ProtectionClassId) = True Then
                    Select Case _ProtectionClassId
                        Case "12"
                            _ProtectionClass = "01"
                        Case "13"
                            _ProtectionClass = "02"
                        Case "14"
                            _ProtectionClass = "03"
                        Case "15"
                            _ProtectionClass = "04"
                        Case "16"
                            _ProtectionClass = "05"
                        Case "17"
                            _ProtectionClass = "06"
                        Case "18"
                            _ProtectionClass = "07"
                        Case "19"
                            _ProtectionClass = "08"
                        Case "20"
                            _ProtectionClass = "8B"
                        Case "21"
                            _ProtectionClass = "09"
                        Case "22"
                            _ProtectionClass = "10"
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverages w/ coveragecode_ids 21107 and 21520</remarks>
        Public Property QuotedPremium_With_EQ As String 'added 4/17/2013
            Get
                'Return _QuotedPremium_With_EQ
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_QuotedPremium_With_EQ)
            End Get
            Set(value As String)
                _QuotedPremium_With_EQ = value
                qqHelper.ConvertToQuotedPremiumFormat(_QuotedPremium_With_EQ)
            End Set
        End Property

        Public Property ScheduledCoverageNum As String 'added 1/22/2015 for reconciliation
            Get
                Return _ScheduledCoverageNum
            End Get
            Set(value As String)
                _ScheduledCoverageNum = value
            End Set
        End Property

        Public Property IsAgreedValue As Boolean 'added 1/11/2018 (3/29/2018 in new branch)
            Get
                Return _IsAgreedValue
            End Get
            Set(value As Boolean)
                _IsAgreedValue = value
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21520</remarks>
        Public Property EarthquakeDeductibleId As String
            Get
                Return _EarthquakeDeductibleId
            End Get
            Set(value As String)
                _EarthquakeDeductibleId = value
                '_EarthquakeDeductible = qqHelper.GetStaticDataValueForTextAndStateAndCompany(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, Quote.QuickQuoteState, Quote.Company, MyProperty.EarthquakeDeductibleId, Me.Quote.LobType))
                '_EarthquakeDeductible = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.DeductibleId, _EarthquakeDeductibleId)    
                _EarthquakeDeductible = ""
                If IsNumeric(_EarthquakeDeductibleId) = True Then
                    Select Case _EarthquakeDeductibleId
                        Case "34"
                            _EarthquakeDeductible = "5%"
                        Case "36"
                            _EarthquakeDeductible = "10%"
                        Case "37"
                            _EarthquakeDeductible = "15%"
                        Case "38"
                            _EarthquakeDeductible = "20%"
                        Case "39"
                            _EarthquakeDeductible = "25%"
                        Case "48"
                            _EarthquakeDeductible = "30%"
                        Case "49"
                            _EarthquakeDeductible = "35%"
                        Case "50"
                            _EarthquakeDeductible = "40%"
                        Case Else
                            _EarthquakeDeductible = ""
                    End Select
                End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21520</remarks>
        Public Property EarthquakeDeductible As String
            Get
                Return _EarthquakeDeductible
            End Get
            Set(value As String)
                _EarthquakeDeductible = value
                Select Case _EarthquakeDeductible
                    Case "5%"
                        _EarthquakeDeductibleId = "34"
                    Case "10%"
                        _EarthquakeDeductibleId = "36"
                    Case "15%"
                        _EarthquakeDeductibleId = "37"
                    Case "20%"
                        _EarthquakeDeductibleId = "38"
                    Case "25%"
                        _EarthquakeDeductibleId = "39"
                    Case "30%"
                        _EarthquakeDeductibleId = "48"
                    Case "35%"
                        _EarthquakeDeductibleId = "49"
                    Case "40%"
                        _EarthquakeDeductibleId = "50"
                    Case Else
                        _EarthquakeDeductibleId = ""
                End Select
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _Limit = ""
            _IncludedInBlanketCoverage = False
            _DeductibleId = ""
            _Deductible = ""
            _CoinsuranceTypeId = ""
            _CoinsuranceType = ""
            _ValuationId = ""
            _Valuation = ""
            _ConstructionType = ""
            _ConstructionTypeId = ""
            _EarthquakeApplies = False
            _EarthquakeQuotedPremium = ""
            _RatingTypeId = ""
            _RatingType = ""
            _CauseOfLossTypeId = ""
            _CauseOfLossType = ""
            _QuotedPremium = ""
            '_ClassificationCode = New QuickQuoteClassificationCode
            _OptionalTheftDeductibleId = ""
            _OptionalTheftDeductible = ""
            _OptionalTheftQuotedPremium = ""
            _OptionalWindstormOrHailDeductibleId = ""
            _OptionalWindstormOrHailDeductible = ""
            _OptionalWindstormOrHailQuotedPremium = ""
            _FeetToFireHydrant = ""
            _MilesToFireDepartment = ""
            _Description = ""
            _SpecialClassCodeTypeId = ""
            _SpecialClassCodeType = ""
            _SpecialClassCode = ""
            _HasSetSpecialClassCode = False
            _InflationGuardTypeId = ""
            _InflationGuardType = ""
            _ProtectionClass = ""
            _ProtectionClassId = ""
            _QuotedPremium_With_EQ = "" 'added 4/17/2013

            _ScheduledCoverageNum = "" 'added 1/22/2015 for reconciliation

            _IsAgreedValue = False 'added 1/11/2018 (3/29/2018 in new branch)
            
            _EarthquakeDeductibleId = ""
            _EarthquakeDeductible = ""
        End Sub
        Public Sub CalculatePremium_With_EQ_Included() 'added 4/17/2013
            QuotedPremium_With_EQ = qqHelper.getSum(_QuotedPremium, _EarthquakeQuotedPremium)
        End Sub
        Private Sub SetSpecialClassCodeType() 'added 5/6/2013
            _SpecialClassCodeType = ""
            _SpecialClassCode = ""
            _HasSetSpecialClassCode = True
            If _SpecialClassCodeTypeId <> "" AndAlso IsNumeric(_SpecialClassCodeTypeId) = True Then
                Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connDiamond"))
                    sql.queryOrStoredProc = "SELECT dscr, class_code FROM SpecialClassCodeType with (nolock) WHERE specialclasscodetype_id = " & CInt(_SpecialClassCodeTypeId)
                    Dim dr As System.Data.SqlClient.SqlDataReader = sql.GetDataReader
                    If dr IsNot Nothing AndAlso dr.HasRows = True Then
                        dr.Read()
                        _SpecialClassCodeType = dr.Item("dscr").ToString.Trim
                        _SpecialClassCode = dr.Item("class_code").ToString.Trim
                    End If
                End Using
            End If
        End Sub
        Public Function HasValidScheduledCoverageNum() As Boolean 'added 1/22/2015 for reconciliation purposes
            Return qqHelper.IsValidQuickQuoteIdOrNum(_ScheduledCoverageNum)
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
                    If _Limit IsNot Nothing Then
                        _Limit = Nothing
                    End If
                    If _IncludedInBlanketCoverage <> Nothing Then
                        _IncludedInBlanketCoverage = Nothing
                    End If
                    If _DeductibleId IsNot Nothing Then
                        _DeductibleId = Nothing
                    End If
                    If _Deductible IsNot Nothing Then
                        _Deductible = Nothing
                    End If
                    If _CoinsuranceTypeId IsNot Nothing Then
                        _CoinsuranceTypeId = Nothing
                    End If
                    If _CoinsuranceType IsNot Nothing Then
                        _CoinsuranceType = Nothing
                    End If
                    If _ValuationId IsNot Nothing Then
                        _ValuationId = Nothing
                    End If
                    If _Valuation IsNot Nothing Then
                        _Valuation = Nothing
                    End If
                    If _ConstructionType IsNot Nothing Then
                        _ConstructionType = Nothing
                    End If
                    If _ConstructionTypeId IsNot Nothing Then
                        _ConstructionTypeId = Nothing
                    End If
                    If _EarthquakeApplies <> Nothing Then
                        _EarthquakeApplies = Nothing
                    End If
                    If _EarthquakeQuotedPremium IsNot Nothing Then
                        _EarthquakeQuotedPremium = Nothing
                    End If
                    If _RatingTypeId IsNot Nothing Then
                        _RatingTypeId = Nothing
                    End If
                    If _RatingType IsNot Nothing Then
                        _RatingType = Nothing
                    End If
                    If _CauseOfLossTypeId IsNot Nothing Then
                        _CauseOfLossTypeId = Nothing
                    End If
                    If _CauseOfLossType IsNot Nothing Then
                        _CauseOfLossType = Nothing
                    End If
                    If _QuotedPremium IsNot Nothing Then
                        _QuotedPremium = Nothing
                    End If
                    'If _ClassificationCode IsNot Nothing Then
                    '    _ClassificationCode.Dispose()
                    '    _ClassificationCode = Nothing
                    'End If
                    If _OptionalTheftDeductibleId IsNot Nothing Then
                        _OptionalTheftDeductibleId = Nothing
                    End If
                    If _OptionalTheftDeductible IsNot Nothing Then
                        _OptionalTheftDeductible = Nothing
                    End If
                    If _OptionalTheftQuotedPremium IsNot Nothing Then
                        _OptionalTheftQuotedPremium = Nothing
                    End If
                    If _OptionalWindstormOrHailDeductibleId IsNot Nothing Then
                        _OptionalWindstormOrHailDeductibleId = Nothing
                    End If
                    If _OptionalWindstormOrHailDeductible IsNot Nothing Then
                        _OptionalWindstormOrHailDeductible = Nothing
                    End If
                    If _OptionalWindstormOrHailQuotedPremium IsNot Nothing Then
                        _OptionalWindstormOrHailQuotedPremium = Nothing
                    End If
                    If _FeetToFireHydrant IsNot Nothing Then
                        _FeetToFireHydrant = Nothing
                    End If
                    If _MilesToFireDepartment IsNot Nothing Then
                        _MilesToFireDepartment = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _SpecialClassCodeTypeId IsNot Nothing Then
                        _SpecialClassCodeTypeId = Nothing
                    End If
                    If _SpecialClassCodeType IsNot Nothing Then
                        _SpecialClassCodeType = Nothing
                    End If
                    If _SpecialClassCode IsNot Nothing Then
                        _SpecialClassCode = Nothing
                    End If
                    If _HasSetSpecialClassCode <> Nothing Then
                        _HasSetSpecialClassCode = Nothing
                    End If
                    If _InflationGuardTypeId IsNot Nothing Then
                        _InflationGuardTypeId = Nothing
                    End If
                    If _InflationGuardType IsNot Nothing Then
                        _InflationGuardType = Nothing
                    End If
                    If _ProtectionClass IsNot Nothing Then
                        _ProtectionClass = Nothing
                    End If
                    If _ProtectionClassId IsNot Nothing Then
                        _ProtectionClassId = Nothing
                    End If
                    If _QuotedPremium_With_EQ IsNot Nothing Then 'added 4/17/2013
                        _QuotedPremium_With_EQ = Nothing
                    End If

                    If _ScheduledCoverageNum IsNot Nothing Then 'added 1/22/2015 for reconciliation
                        _ScheduledCoverageNum = Nothing
                    End If

                    _IsAgreedValue = Nothing 'added 1/11/2018 (3/29/2018 in new branch)
                    
                    If _EarthquakeDeductibleId IsNot Nothing Then
                        _EarthquakeDeductibleId = Nothing
                    End If
                    If _EarthquakeDeductible IsNot Nothing Then
                        _EarthquakeDeductible = Nothing
                    End If
                    
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
