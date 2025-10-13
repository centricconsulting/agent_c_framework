Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods

Namespace QuickQuote.CommonObjects
    <Serializable()>
    Public Class QuickQuoteBuildingPersonalProperty 'added 7/8/2017
        Inherits QuickQuoteBaseGenericObject(Of Object)
        Implements IDisposable

        Dim qqHelper As New QuickQuoteHelperClass

        Private _ScheduledCoverageNum As String 'for reconciliation
        Private _PersonalPropertyLimit As String '21090: Edit:  Building - Personal Property
        Private _PropertyTypeId As String '21090: Edit:  Building - Personal Property
        'Private _PropertyType As String '21090: Edit:  Building - Personal Property
        Private _RiskTypeId As String '21090: Edit:  Building - Personal Property
        'Private _RiskType As String '21090: Edit:  Building - Personal Property
        Private _EarthquakeApplies As Boolean '21160: CheckBox:  Personal Property - Earthquake (or flag on 21090)
        Private _RatingTypeId As String '21090: Edit:  Building - Personal Property
        'Private _RatingType As String '21090: Edit:  Building - Personal Property
        Private _CauseOfLossTypeId As String '21090: Edit:  Building - Personal Property
        'Private _CauseOfLossType As String '21090: Edit:  Building - Personal Property
        Private _DeductibleId As String '21090: Edit:  Building - Personal Property
        'Private _Deductible As String '21090: Edit:  Building - Personal Property
        Private _CoinsuranceTypeId As String '21090: Edit:  Building - Personal Property
        'Private _CoinsuranceType As String '21090: Edit:  Building - Personal Property
        Private _ValuationId As String '21090: Edit:  Building - Personal Property
        'Private _Valuation As String '21090: Edit:  Building - Personal Property
        Private _BuildingPersonalPropertyQuotedPremium As String '21090: Edit:  Building - Personal Property
        Private _ClassificationCode As QuickQuoteClassificationCode '21090: Edit:  Building - Personal Property
        Private _IsAgreedValue As Boolean '21090: Edit:  Building - Personal Property
        Private _Group1_Rate As String '21176: CheckBox:  Personal Property - Group I
        Private _Group2_Rate As String '21181: CheckBox:  Personal Property - Group II
        Private _Group1_LossCost As String '21176: CheckBox:  Personal Property - Group I
        Private _Group2_LossCost As String '21181: CheckBox:  Personal Property - Group II
        Private _EarthquakeRateGradeTypeId As String '21160: CheckBox:  Personal Property - Earthquake
        Private _PersonalPropertyEarthquakeQuotedPremium As String '21160: CheckBox:  Personal Property - Earthquake
        Private _BuildingPersonalPropertyWithEarthquakeQuotedPremium As String '21090 + 21160
        Private _EarthquakeBuildingClassificationPercentage As String '21160: CheckBox:  Personal Property - Earthquake
        Private _OptionalTheftDeductibleId As String '21168: CheckBox:  Personal Property - Optional Theft
        'Private _OptionalTheftDeductible As String '21168: CheckBox:  Personal Property - Optional Theft
        Private _OptionalWindstormOrHailDeductibleId As String '21171: CheckBox:  Personal Property - Windstorm or Hail
        'Private _OptionalWindstormOrHailDeductible As String '21171: CheckBox:  Personal Property - Windstorm or Hail
        Private _DoesYardRateApplyTypeId As String '21090: Edit:  Building - Personal Property
        Private _IncludedInBlanketCoverage As Boolean '21090: Edit:  Building - Personal Property
        Private _InflationGuardTypeId As String '21090: Building - Personal Property


        Public Property ScheduledCoverageNum As String
            Get
                Return _ScheduledCoverageNum
            End Get
            Set(value As String)
                _ScheduledCoverageNum = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PersonalPropertyLimit As String
            Get
                Return _PersonalPropertyLimit
            End Get
            Set(value As String)
                _PersonalPropertyLimit = value
                qqHelper.ConvertToLimitFormat(_PersonalPropertyLimit)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property PropertyTypeId As String
            Get
                Return _PropertyTypeId
            End Get
            Set(value As String)
                _PropertyTypeId = value
                '_PropertyType = ""
                'If IsNumeric(_PropertyTypeId) = True Then
                '    Select Case _PropertyTypeId
                '        Case "0"
                '            _PropertyType = "N/A"
                '        Case "1"
                '            _PropertyType = "Personal Property - Stock Only"
                '        Case "2"
                '            _PropertyType = "Personal Property - Excluding Stock"
                '        Case "3"
                '            _PropertyType = "Machinery and Equipment"
                '        Case "4"
                '            _PropertyType = "Furniture"
                '        Case "5"
                '            _PropertyType = "Fixtures"
                '        Case "6"
                '            _PropertyType = "Tenants Improvements and Betterments"
                '        Case "7"
                '            _PropertyType = "Personal Property - Including Stock"
                '        Case "8"
                '            _PropertyType = "Personal Property of Others"
                '    End Select
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public ReadOnly Property PropertyType As String
            Get
                Dim _PropertyType As String = ""
                If IsNumeric(_PropertyTypeId) = True Then
                    Select Case _PropertyTypeId
                        Case "0"
                            _PropertyType = "N/A"
                        Case "1"
                            _PropertyType = "Personal Property - Stock Only"
                        Case "2"
                            _PropertyType = "Personal Property - Excluding Stock"
                        Case "3"
                            _PropertyType = "Machinery and Equipment"
                        Case "4"
                            _PropertyType = "Furniture"
                        Case "5"
                            _PropertyType = "Fixtures"
                        Case "6"
                            _PropertyType = "Tenants Improvements and Betterments"
                        Case "7"
                            _PropertyType = "Personal Property - Including Stock"
                        Case "8"
                            _PropertyType = "Personal Property of Others"
                    End Select
                End If
                Return _PropertyType
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property RiskTypeId As String
            Get
                Return _RiskTypeId
            End Get
            Set(value As String)
                _RiskTypeId = value
                '_RiskType = ""
                'If IsNumeric(_RiskTypeId) = True Then
                '    Select Case _RiskTypeId
                '        Case "0"
                '            _RiskType = "N/A"
                '        Case "1"
                '            _RiskType = "Not in Towing Business"
                '        Case "2"
                '            _RiskType = "Tow Truck Operator"
                '        Case "3"
                '            _RiskType = "Type 1 - Apartments and Condominiums - Residential Use Only"
                '        Case "4"
                '            _RiskType = "Type 2 - Offices"
                '        Case "5"
                '            _RiskType = "Type 3 - All Other Personal Property"
                '        Case "6"
                '            _RiskType = "Mercantile or Non-Manufacturing"
                '        Case "7"
                '            _RiskType = "Manufacturing"
                '        Case "8"
                '            _RiskType = "Mining"
                '        Case "9"
                '            _RiskType = "Rental Properties"
                '        Case "10"
                '            _RiskType = "Combined Manufacturing And Mercantile"
                '        Case "11"
                '            _RiskType = "Combined Manufacturing and Rental"
                '        Case "12"
                '            _RiskType = "Combined Mercantile and Rental"
                '    End Select
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public ReadOnly Property RiskType As String
            Get
                Dim _RiskType As String = ""
                If IsNumeric(_RiskTypeId) = True Then
                    Select Case _RiskTypeId
                        Case "0"
                            _RiskType = "N/A"
                        Case "1"
                            _RiskType = "Not in Towing Business"
                        Case "2"
                            _RiskType = "Tow Truck Operator"
                        Case "3"
                            _RiskType = "Type 1 - Apartments and Condominiums - Residential Use Only"
                        Case "4"
                            _RiskType = "Type 2 - Offices"
                        Case "5"
                            _RiskType = "Type 3 - All Other Personal Property"
                        Case "6"
                            _RiskType = "Mercantile or Non-Manufacturing"
                        Case "7"
                            _RiskType = "Manufacturing"
                        Case "8"
                            _RiskType = "Mining"
                        Case "9"
                            _RiskType = "Rental Properties"
                        Case "10"
                            _RiskType = "Combined Manufacturing And Mercantile"
                        Case "11"
                            _RiskType = "Combined Manufacturing and Rental"
                        Case "12"
                            _RiskType = "Combined Mercantile and Rental"
                    End Select
                End If
                Return _RiskType
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160; also sets flag on coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property RatingTypeId As String
            Get
                Return _RatingTypeId
            End Get
            Set(value As String)
                _RatingTypeId = value
                '_RatingType = ""
                'If IsNumeric(_RatingTypeId) = True Then
                '    Select Case _RatingTypeId
                '        Case "0"
                '            _RatingType = "None"
                '        Case "1"
                '            _RatingType = "Class Rated"
                '        Case "2"
                '            _RatingType = "Specific Rated"
                '        Case "3"
                '            _RatingType = "Special Class Rate"
                '        Case "4"
                '            _RatingType = "Symbol"
                '        Case "5"
                '            _RatingType = "Specific"
                '    End Select
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public ReadOnly Property RatingType As String
            Get
                Dim _RatingType As String = ""
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
                Return _RatingType
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property CauseOfLossTypeId As String
            Get
                Return _CauseOfLossTypeId
            End Get
            Set(value As String)
                _CauseOfLossTypeId = value
                '_CauseOfLossType = ""
                'If IsNumeric(_CauseOfLossTypeId) = True Then
                '    Select Case _CauseOfLossTypeId
                '        Case "0"
                '            _CauseOfLossType = "N/A"
                '        Case "1"
                '            _CauseOfLossType = "Basic Form"
                '        Case "2"
                '            _CauseOfLossType = "Broad Form"
                '        Case "3"
                '            _CauseOfLossType = "Special Form Including Theft"
                '        Case "4"
                '            _CauseOfLossType = "Special Form Excluding Theft"
                '    End Select
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public ReadOnly Property CauseOfLossType As String
            Get
                Dim _CauseOfLossType As String = ""
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
                Return _CauseOfLossType
            End Get
        End Property

        Public Property IsAgreedValue As Boolean
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property DeductibleId As String
            Get
                Return _DeductibleId
            End Get
            Set(value As String)
                _DeductibleId = value
                '_Deductible = ""
                'If IsNumeric(_DeductibleId) = True Then
                '    Select Case _DeductibleId
                '        Case "0"
                '            _Deductible = "N/A"
                '        Case "4"
                '            _Deductible = "250"
                '        Case "8"
                '            _Deductible = "500"
                '        Case "9"
                '            _Deductible = "1,000"
                '        Case "15"
                '            _Deductible = "2,500"
                '        Case "16"
                '            _Deductible = "5,000"
                '        Case "17"
                '            _Deductible = "10,000"
                '        Case "19"
                '            _Deductible = "25,000"
                '        Case "20"
                '            _Deductible = "50,000"
                '        Case "21"
                '            _Deductible = "75,000"
                '        Case "32"
                '            _Deductible = "1%"
                '        Case "33"
                '            _Deductible = "2%"
                '        Case "34"
                '            _Deductible = "5%"
                '        Case "42"
                '            _Deductible = "Same"
                '        Case "36"
                '            _Deductible = "10%"
                '    End Select
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public ReadOnly Property Deductible As String
            Get
                Dim _Deductible As String = ""
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
                Return _Deductible
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property CoinsuranceTypeId As String
            Get
                Return _CoinsuranceTypeId
            End Get
            Set(value As String)
                _CoinsuranceTypeId = value
                '_CoinsuranceType = ""
                'If IsNumeric(_CoinsuranceTypeId) = True Then
                '    Select Case _CoinsuranceTypeId
                '        Case "0"
                '            _CoinsuranceType = "N/A"
                '        Case "1"
                '            _CoinsuranceType = "Waived"
                '        Case "2"
                '            _CoinsuranceType = "50%"
                '        Case "3"
                '            _CoinsuranceType = "60%"
                '        Case "4"
                '            _CoinsuranceType = "70%"
                '        Case "5"
                '            _CoinsuranceType = "80%"
                '        Case "6"
                '            _CoinsuranceType = "90%"
                '        Case "7"
                '            _CoinsuranceType = "100%"
                '        Case "8"
                '            _CoinsuranceType = "10%"
                '        Case "9"
                '            _CoinsuranceType = "20%"
                '        Case "10"
                '            _CoinsuranceType = "30%"
                '        Case "11"
                '            _CoinsuranceType = "40%"
                '        Case "12"
                '            _CoinsuranceType = "125%"
                '    End Select
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public ReadOnly Property CoinsuranceType As String
            Get
                Dim _CoinsuranceType As String = ""
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
                Return _CoinsuranceType
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property ValuationId As String
            Get
                Return _ValuationId
            End Get
            Set(value As String)
                _ValuationId = value
                '(1=Replacement Cost; 2=Actual Cash Value; 3=Functional Building Valuation)
                '_Valuation = ""
                'If IsNumeric(_ValuationId) = True Then
                '    Select Case _ValuationId
                '        Case "-1" 'added 10/19/2012 for CPR to match specs
                '            _Valuation = "N/A"
                '        Case "1"
                '            _Valuation = "Replacement Cost"
                '        Case "2"
                '            _Valuation = "Actual Cash Value"
                '        Case "3"
                '            _Valuation = "Functional Building Valuation"
                '        Case "7" 'added 10/18/2012 for CPR
                '            _Valuation = "Functional Replacement Cost"
                '    End Select
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public ReadOnly Property Valuation As String
            Get
                Dim _Valuation As String = ""
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
                Return _Valuation
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property BuildingPersonalPropertyQuotedPremium As String
            Get
                'Return _BuildingPersonalPropertyQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_BuildingPersonalPropertyQuotedPremium)
            End Get
            Set(value As String)
                _BuildingPersonalPropertyQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BuildingPersonalPropertyQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property ClassificationCode As QuickQuoteClassificationCode
            Get
                SetObjectsParent(_ClassificationCode)
                Return _ClassificationCode
            End Get
            Set(value As QuickQuoteClassificationCode)
                _ClassificationCode = value
                SetObjectsParent(_ClassificationCode)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21176 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property Group1_Rate As String
            Get
                Return _Group1_Rate
            End Get
            Set(value As String)
                _Group1_Rate = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21181 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property Group2_Rate As String
            Get
                Return _Group2_Rate
            End Get
            Set(value As String)
                _Group2_Rate = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21176 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property Group1_LossCost As String
            Get
                Return _Group1_LossCost
            End Get
            Set(value As String)
                _Group1_LossCost = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21181 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property Group2_LossCost As String
            Get
                Return _Group2_LossCost
            End Get
            Set(value As String)
                _Group2_LossCost = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160; found inside ScheduledCoverage</remarks>
        Public Property EarthquakeRateGradeTypeId As String
            Get
                Return _EarthquakeRateGradeTypeId
            End Get
            Set(value As String)
                _EarthquakeRateGradeTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property PersonalPropertyEarthquakeQuotedPremium As String
            Get
                'Return _PersonalPropertyEarthquakeQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_PersonalPropertyEarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _PersonalPropertyEarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_PersonalPropertyEarthquakeQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks><see cref="BuildingPersonalPropertyQuotedPremium"/> + <see cref="PersonalPropertyEarthquakeQuotedPremium"/></remarks>
        Public Property BuildingPersonalPropertyWithEarthquakeQuotedPremium As String
            Get
                'Return _BuildingPersonalPropertyWithEarthquakeQuotedPremium
                'updated 8/25/2014
                Return qqHelper.QuotedPremiumFormat(_BuildingPersonalPropertyWithEarthquakeQuotedPremium)
            End Get
            Set(value As String)
                _BuildingPersonalPropertyWithEarthquakeQuotedPremium = value
                qqHelper.ConvertToQuotedPremiumFormat(_BuildingPersonalPropertyWithEarthquakeQuotedPremium)
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21160 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); value for CoverageAdditionalInfoRecord w/ description = 'EarthquakeBuildingClassificationPercentage'; found inside ScheduledCoverage</remarks>
        Public Property EarthquakeBuildingClassificationPercentage As String
            Get
                Return _EarthquakeBuildingClassificationPercentage
            End Get
            Set(value As String)
                _EarthquakeBuildingClassificationPercentage = value
                'qqHelper.ConvertToQuotedPremiumFormat(_EarthquakeBuildingClassificationPercentage)'needs to be % instead
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21168 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property OptionalTheftDeductibleId As String 'UI doesn't have the % values
            Get
                Return _OptionalTheftDeductibleId
            End Get
            Set(value As String)
                _OptionalTheftDeductibleId = value
                '_OptionalTheftDeductible = ""
                'If IsNumeric(_OptionalTheftDeductibleId) = True Then
                '    Select Case _OptionalTheftDeductibleId
                '        Case "0"
                '            _OptionalTheftDeductible = "N/A"
                '        Case "4"
                '            _OptionalTheftDeductible = "250"
                '        Case "8"
                '            _OptionalTheftDeductible = "500"
                '        Case "9"
                '            _OptionalTheftDeductible = "1,000"
                '        Case "15"
                '            _OptionalTheftDeductible = "2,500"
                '        Case "16"
                '            _OptionalTheftDeductible = "5,000"
                '        Case "17"
                '            _OptionalTheftDeductible = "10,000"
                '        Case "19"
                '            _OptionalTheftDeductible = "25,000"
                '        Case "20"
                '            _OptionalTheftDeductible = "50,000"
                '        Case "21"
                '            _OptionalTheftDeductible = "75,000"
                '        Case "32"
                '            _OptionalTheftDeductible = "1%"
                '        Case "33"
                '            _OptionalTheftDeductible = "2%"
                '        Case "34"
                '            _OptionalTheftDeductible = "5%"
                '        Case "42"
                '            _OptionalTheftDeductible = "Same"
                '        Case "36"
                '            _OptionalTheftDeductible = "10%"
                '    End Select
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21168 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public ReadOnly Property OptionalTheftDeductible As String
            Get
                Dim _OptionalTheftDeductible As String = ""
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
                Return _OptionalTheftDeductible
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21171 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public Property OptionalWindstormOrHailDeductibleId As String
            Get
                Return _OptionalWindstormOrHailDeductibleId
            End Get
            Set(value As String)
                _OptionalWindstormOrHailDeductibleId = value
                '_OptionalWindstormOrHailDeductible = ""
                'If IsNumeric(_OptionalWindstormOrHailDeductibleId) = True Then
                '    Select Case _OptionalWindstormOrHailDeductibleId
                '        Case "0"
                '            _OptionalWindstormOrHailDeductible = "N/A"
                '        Case "4"
                '            _OptionalWindstormOrHailDeductible = "250"
                '        Case "8"
                '            _OptionalWindstormOrHailDeductible = "500"
                '        Case "9"
                '            _OptionalWindstormOrHailDeductible = "1,000"
                '        Case "15"
                '            _OptionalWindstormOrHailDeductible = "2,500"
                '        Case "16"
                '            _OptionalWindstormOrHailDeductible = "5,000"
                '        Case "17"
                '            _OptionalWindstormOrHailDeductible = "10,000"
                '        Case "19"
                '            _OptionalWindstormOrHailDeductible = "25,000"
                '        Case "20"
                '            _OptionalWindstormOrHailDeductible = "50,000"
                '        Case "21"
                '            _OptionalWindstormOrHailDeductible = "75,000"
                '        Case "32"
                '            _OptionalWindstormOrHailDeductible = "1%"
                '        Case "33"
                '            _OptionalWindstormOrHailDeductible = "2%"
                '        Case "34"
                '            _OptionalWindstormOrHailDeductible = "5%"
                '        Case "42"
                '            _OptionalWindstormOrHailDeductible = "Same"
                '        Case "36"
                '            _OptionalWindstormOrHailDeductible = "10%"
                '    End Select
                'End If
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21171 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others] for coverage w/ coveragecode_id = 21090); found inside ScheduledCoverage</remarks>
        Public ReadOnly Property OptionalWindstormOrHailDeductible As String
            Get
                Dim _OptionalWindstormOrHailDeductible As String = ""
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
                Return _OptionalWindstormOrHailDeductible
            End Get
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property DoesYardRateApplyTypeId As String '0=N/A; 1=Yes; 2=No
            Get
                Return _DoesYardRateApplyTypeId
            End Get
            Set(value As String)
                _DoesYardRateApplyTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
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
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public Property InflationGuardTypeId As String
            Get
                Return _InflationGuardTypeId
            End Get
            Set(value As String)
                _InflationGuardTypeId = value
            End Set
        End Property
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>corresponds to Diamond coverage w/ coveragecode_id 21090 (when BusinessPropertyTypeId &lt;&gt; 8 [Personal Property of Others]); found inside ScheduledCoverage</remarks>
        Public ReadOnly Property InflationGuardType As String
            Get
                Dim _InflationGuardType As String = ""
                If IsNumeric(_InflationGuardTypeId) = True Then
                    _InflationGuardType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.InflationGuardTypeId, _InflationGuardTypeId)
                End If
                Return _InflationGuardType
            End Get
        End Property


        Public Sub New()
            MyBase.New()
            SetDefaults()
        End Sub
        Private Sub SetDefaults()

            _ScheduledCoverageNum = "" 'for reconciliation
            _PersonalPropertyLimit = ""
            _PropertyTypeId = ""
            '_PropertyType = ""
            _RiskTypeId = ""
            '_RiskType = ""
            _EarthquakeApplies = False
            _IsAgreedValue = False
            _RatingTypeId = ""
            '_RatingType = ""
            _CauseOfLossTypeId = ""
            '_CauseOfLossType = ""
            _DeductibleId = ""
            '_Deductible = ""
            _CoinsuranceTypeId = ""
            '_CoinsuranceType = ""
            _ValuationId = ""
            '_Valuation = ""
            _BuildingPersonalPropertyQuotedPremium = ""
            _ClassificationCode = New QuickQuoteClassificationCode
            _Group1_Rate = ""
            _Group2_Rate = ""
            _Group1_LossCost = ""
            _Group2_LossCost = ""
            _EarthquakeRateGradeTypeId = ""
            _PersonalPropertyEarthquakeQuotedPremium = ""
            _BuildingPersonalPropertyWithEarthquakeQuotedPremium = ""
            _EarthquakeBuildingClassificationPercentage = ""
            _OptionalTheftDeductibleId = ""
            '_OptionalTheftDeductible = ""
            _OptionalWindstormOrHailDeductibleId = ""
            '_OptionalWindstormOrHailDeductible = ""
            _DoesYardRateApplyTypeId = ""
            _IncludedInBlanketCoverage = False
            _InflationGuardTypeId = ""

        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        'Protected Overridable Sub Dispose(disposing As Boolean)
        Protected Overloads Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).

                    qqHelper.DisposeString(_ScheduledCoverageNum) 'for reconciliation
                    qqHelper.DisposeString(_PersonalPropertyLimit)
                    qqHelper.DisposeString(_PropertyTypeId)
                    'qqHelper.DisposeString(_PropertyType)
                    qqHelper.DisposeString(_RiskTypeId)
                    'qqHelper.DisposeString(_RiskType)
                    _EarthquakeApplies = Nothing
                    _IsAgreedValue = Nothing
                    qqHelper.DisposeString(_RatingTypeId)
                    'qqHelper.DisposeString(_RatingType)
                    qqHelper.DisposeString(_CauseOfLossTypeId)
                    'qqHelper.DisposeString(_CauseOfLossType)
                    qqHelper.DisposeString(_DeductibleId)
                    'qqHelper.DisposeString(_Deductible)
                    qqHelper.DisposeString(_CoinsuranceTypeId)
                    'qqHelper.DisposeString(_CoinsuranceType)
                    qqHelper.DisposeString(_ValuationId)
                    'qqHelper.DisposeString(_Valuation)
                    qqHelper.DisposeString(_BuildingPersonalPropertyQuotedPremium)
                    If _ClassificationCode IsNot Nothing Then
                        _ClassificationCode.Dispose()
                        _ClassificationCode = Nothing
                    End If
                    qqHelper.DisposeString(_Group1_Rate)
                    qqHelper.DisposeString(_Group2_Rate)
                    qqHelper.DisposeString(_Group1_LossCost)
                    qqHelper.DisposeString(_Group2_LossCost)
                    qqHelper.DisposeString(_EarthquakeRateGradeTypeId)
                    qqHelper.DisposeString(_PersonalPropertyEarthquakeQuotedPremium)
                    qqHelper.DisposeString(_BuildingPersonalPropertyWithEarthquakeQuotedPremium)
                    qqHelper.DisposeString(_EarthquakeBuildingClassificationPercentage)
                    qqHelper.DisposeString(_OptionalTheftDeductibleId)
                    'qqHelper.DisposeString(_OptionalTheftDeductible)
                    qqHelper.DisposeString(_OptionalWindstormOrHailDeductibleId)
                    'qqHelper.DisposeString(_OptionalWindstormOrHailDeductible)
                    qqHelper.DisposeString(_DoesYardRateApplyTypeId)
                    _IncludedInBlanketCoverage = Nothing
                    qqHelper.DisposeString(_InflationGuardTypeId)

                    MyBase.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        'Public Sub Dispose() Implements IDisposable.Dispose
        Public Overrides Sub Dispose()
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
