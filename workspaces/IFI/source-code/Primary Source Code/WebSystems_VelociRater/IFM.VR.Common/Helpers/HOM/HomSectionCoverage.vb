Imports System.Configuration
Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.HOM.SectionCoverage
Imports IFM.VR.Common.Helpers.MultiState
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.HOM

    ''' <summary>
    ''' This class removes the differences between Section(I,II,IAndII)Coverages so that you can interact with anyone of them with the same interface.
    ''' </summary>
    Public Class SectionCoverage

        'removed 12/21/17 for HOM Upgrade
        'Public Const HO2 As Int32 = 1
        'Public Const HO3 As Int32 = 2
        'Public Const HO3_15 As Int32 = 3
        'Public Const HO4 As Int32 = 4
        'Public Const HO6 As Int32 = 5
        'Public Const ML2 As Int32 = 6
        'Public Const ML4 As Int32 = 7

        Public Enum QuickQuoteSectionCoverageType
            NotDefined = 0
            SectionICoverage = 1
            SectionIICoverage = 2
            SectionIAndIICoverage = 3
        End Enum
        Dim _sectionCoverage As Object = Nothing
        Dim _chc As New CommonHelperClass
        Dim myType As QuickQuoteSectionCoverageType = QuickQuoteSectionCoverageType.NotDefined

        Protected ReadOnly Property SectionType As SectionCoverage.QuickQuoteSectionCoverageType
            Get
                Return myType
            End Get
        End Property

        Public Enum DisplayType
            notAvailable
            included
            justCheckBox

            justEffectiveDate
            justDescription
            justDeductible
            justLimit
            hasIncreaseWithFreeForm
            hasIncreasewithDropDown
            hasLimitAndDescription
            'hasIncreasewithDropDown_ForceAlways
            hasEffectiveAndExpirationDates 'added 1/15/18 for HOM Upgrade MLW
            hasEffAndExpDatesWithLimit 'added 1/22/18 for HOM Upgrade MLW
            isBusinessPursuits 'added 1/24/18 for HOM Upgrade MLW
            isGreenUpgrades 'added 1/25/18 for HOM Upgrade MLW
            isBusinessStructure 'added 1/29/18 for HOM Upgrade MLW

            isOtherStructures
            isFarmLand
            isAdditionlResidence
            'isPermittedIncidentalOccupied43
            isCanine 'added 1/23/18 for HOM Upgrade MLW
            isLossAssessment 'added 1/30/18 for HOM Upgrade MLW
            isOtherMembers 'added 1/31/18 for HOM Upgrade MLW
            isAdditionalInterests 'added 1/31/18 for HOM Upgrade MLW
            isTrust 'added 2/14/18 for HOM Upgrade MLW
            isAdditionalInsured 'added 4/30/18 for HOM Upgrade Bug 26102 MLW
            isSpecialEventCoverage ' added 10/25/21 MGB
            hasIncreaseWithLocation 'added 07/24/2025 WS-3040 MLW
        End Enum

        Public Property IncludedLimit As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).IncludedLimit
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).IncludedLimit
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).PropertyIncludedLimit
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).IncludedLimit = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).IncludedLimit = value
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).PropertyIncludedLimit = value
                End Select
                CalcTotal()
            End Set
        End Property

        Public Property IncreasedLimit As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).IncreasedLimit
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).IncreasedLimit
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).PropertyIncreasedLimit
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).IncreasedLimit = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).IncreasedLimit = value
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).PropertyIncreasedLimit = value
                End Select
                CalcTotal()
            End Set
        End Property

        Public Property IncreasedLimitId As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).IncreasedLimitId
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).IncreasedLimitId
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'No Property on this coverageType
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).IncreasedLimitId = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).IncreasedLimitId = value
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Throw New Exception("IncreasedLimitId is not supported on SectionIAndII coverages.")
                End Select
                CalcTotal()
            End Set
        End Property

        Public Property DeductibleId As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).DeductibleLimitId
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'Throw New Exception("Deductible is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'No Property on this coverageType
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).DeductibleLimitId = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Throw New Exception("Deductible is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Throw New Exception("Deductible is not supported on SectionIAndII coverages.")
                End Select
            End Set
        End Property

        Public Property EffectiveDate As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).EffectiveDate
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'Throw New Exception("Effective Date is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'No Property on this coverageType
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).EffectiveDate = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Throw New Exception("Effective Date is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Throw New Exception("Effective Date is not supported on SectionIandII coverages.")
                End Select
            End Set
        End Property


        Public Property TotalLimit As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).TotalLimit
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).TotalLimit
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).PropertyTotalLimit
                End Select
                Return Nothing
            End Get
            Private Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).TotalLimit = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).TotalLimit = value
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).PropertyTotalLimit = value
                End Select
            End Set

        End Property
        Public Sub ForceCalcTotal()
            CalcTotal()
        End Sub

        Private Sub CalcTotal()
            If IncreasedLimitId.NotEqualsAny("", "0") Then
                Dim qqhelper As New QuickQuoteHelperClass
                Dim limit As Double = TryToGetDouble(qqhelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteSectionICoverage, QuickQuoteHelperClass.QuickQuotePropertyName.IncreasedLimitId, Me.IncreasedLimitId))
                Me.TotalLimit = TryToGetDouble(IncludedLimit) + TryToGetDouble(limit)
            Else
                Me.TotalLimit = TryToGetDouble(IncludedLimit) + TryToGetDouble(IncreasedLimit)
            End If
        End Sub

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

        Public Property ConstructionTypeId As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).ConstructionTypeId
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).ConstructionTypeId
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).ConstructionTypeId = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).ConstructionTypeId = value
                        Throw New Exception("ConstructionTypeId is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).ConstructionTypeId = value
                        Throw New Exception("ConstructionTypeId is not supported on SectionIAndII coverages.")
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
                        'Added 1/30/18 for HOM Upgrade MLW
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
                        'Added 1/30/18 for HOM Upgrade MLW
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).Address = value
                End Select
            End Set
        End Property

        Public Property NumberOfFamilies As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).NumberOfFamilies
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).NumberOfFamilies
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).NumberOfFamilies = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).NumberOfFamilies = value
                End Select
            End Set
        End Property

        Public Property IntialFarmCheckbox As Boolean
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).InitialFarmPremises
                End Select
                Return Nothing
            End Get
            Set(value As Boolean)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).InitialFarmPremises = value
                End Select
            End Set
        End Property

        'added 1/15/18 for HOM Upgrade MLW
        Public Property EventEffDate As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).EventEffDate
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).EventFrom
                        'Throw New Exception("Inception Date is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'No Property on this coverageType
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).EventEffDate = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).EventFrom = value  ' updated 10/26/21 for special event coverage
                        'Throw New Exception("EventEffDate is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Throw New Exception("EventEffDate is not supported on SectionIandII coverages.")
                End Select
            End Set
        End Property

        'added 1/15/18 for HOM Upgrade MLW
        Public Property EventExpDate As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).EventExpDate
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).EventTo ' updated 10/26/21 for special event coverage
                        'Throw New Exception("Termination Date is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'No Property on this coverageType
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).EventExpDate = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).EventTo = value ' updated 10/26/21 for special event coverage
                        'Throw New Exception("EventExpDate is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Throw New Exception("EventExpDate is not supported on SectionIandII coverages.")
                End Select
            End Set
        End Property

        'added 1/24/18 for HOM Upgrade MLW
        Public Property Name As QuickQuote.CommonObjects.QuickQuoteName
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        'No Property on this coverageType
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
                        Throw New Exception("Name is not supported on SectionI coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIICoverage).Name = value
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).Name = value
                End Select
            End Set
        End Property

        'added 1/25/18 for HOM Upgrade MLW
        Public Property IncreasedCostOfLoss As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).IncreasedCostOfLoss
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'No Property on this coverageType
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).IncreasedCostOfLoss = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Throw New Exception("IncreasedCostOfLoss is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Throw New Exception("IncreasedCostOfLoss is not supported on SectionIandII coverages.")
                End Select
            End Set
        End Property

        'added 1/25/18 for HOM Upgrade MLW
        Public Property IncreasedCostOfLossId As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).IncreasedCostOfLossId
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'No Property on this coverageType
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).IncreasedCostOfLossId = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Throw New Exception("IncreasedCostOfLossId is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Throw New Exception("IncreasedCostOfLossId is not supported on SectionIandII coverages.")
                End Select
            End Set
        End Property

        'added 1/25/18 for HOM Upgrade MLW
        Public Property VegetatedRoofApplied As Boolean
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).VegetatedRoof
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'No Property on this coverageType
                End Select
                Return Nothing
            End Get
            Set(value As Boolean)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).VegetatedRoof = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Throw New Exception("VegetatedRoofApplied is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Throw New Exception("VegetatedRoofApplied is not supported on SectionIandII coverages.")
                End Select
            End Set
        End Property

        'Added 1/29/18 for HOM Upgrade MLW
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
                CalcBuildingTotal()
            End Set
        End Property

        Private Sub CalcBuildingTotal()
            If BuildingLimit.EqualsAny("", "0") Then
                Me.TotalLimit = ""
            Else
                Me.TotalLimit = TryToGetDouble(BuildingLimit)
            End If
        End Sub

        'added 1/25/18 for HOM Upgrade MLW
        Public Property ExpRelCoverageLimit As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).RelatedExpenseLimit
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        'No Property on this coverageType
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionICoverage).RelatedExpenseLimit = value
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Throw New Exception("ExpRelCoverageLimit is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Throw New Exception("ExpRelCoverageLimit is not supported on SectionIandII coverages.")
                End Select
            End Set
        End Property

        'added 2/2/18 for HOM Upgrade MLW
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

        'Added 2/5/18 for HOM Upgrade MLW
        Public Property LiabilityIncludedLimit As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).LiabilityIncludedLimit
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Throw New Exception("Liability Included Limit is not supported on SectionI coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Throw New Exception("Liability Included Limit is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).LiabilityIncludedLimit = value
                End Select
                CalcLiabTotal()
            End Set
        End Property

        'Added 2/5/18 for HOM Upgrade MLW
        Public Property LiabilityIncreasedLimit As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).LiabilityIncreasedLimit
                End Select
                Return Nothing
            End Get
            Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Throw New Exception("Liability Increased Limit is not supported on SectionI coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Throw New Exception("Liability Increased Limit is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).LiabilityIncreasedLimit = value
                End Select
                CalcLiabTotal()
            End Set
        End Property

        'Added 2/27/18 for Assisted Living liability total limit calculation for HOM Upgrade MLW
        Public Property LiabilityTotalLimit As String
            Get
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        'No Property on this coverageType
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        Return DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).LiabilityTotalLimit
                End Select
                Return Nothing
            End Get
            Private Set(value As String)
                Select Case myType
                    Case QuickQuoteSectionCoverageType.SectionICoverage
                        Throw New Exception("Liability Total Limit is not supported on SectionI coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIICoverage
                        Throw New Exception("Liability Total Limit is not supported on SectionII coverages.")
                    Case QuickQuoteSectionCoverageType.SectionIAndIICoverage
                        DirectCast(Me._sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).LiabilityTotalLimit = value
                End Select
            End Set

        End Property

        'Added 2/27/18 for Assisted Living liability total limit calculation - for HOM Upgrade MLW
        Private Sub CalcLiabTotal()
            Me.LiabilityTotalLimit = TryToGetDouble(LiabilityIncludedLimit) + TryToGetDouble(LiabilityIncreasedLimit)
        End Sub


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

        Public ReadOnly Property SectionCoverageIAndIIEnum_Property As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIIPropertyCoverageType
            Get
                If myType = QuickQuoteSectionCoverageType.SectionIAndIICoverage Then
                    Return DirectCast(_sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).PropertyCoverageType
                End If
                Return QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIIPropertyCoverageType.None
            End Get

        End Property

        Public ReadOnly Property SectionCoverageIAndIIEnum_Liability As QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIILiabilityCoverageType
            Get
                If myType = QuickQuoteSectionCoverageType.SectionIAndIICoverage Then
                    Return DirectCast(_sectionCoverage, QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage).LiabilityCoverageType
                End If
                Return QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIIPropertyCoverageType.None
            End Get

        End Property


        'added 12/21/17 for HOM Upgrade - MLW
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
            'If qqh.doUseNewVersionOfLOB(effectiveDate, QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade) = True Then
            If qqh.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") Then
                Return "After20180701"
            Else
                Return "Before20180701"
            End If
        End Function

        'Updated 5/16/2022 for task 74106 MLW
        'Public Shared Function GetSupportedPrimaryCoverages(ByVal EffDate As String, FormTypeId As String, ByVal SeasonalOrSecondaryOccupancy As Boolean, ByVal StructureTypeId As String, ByVal OccupancyCodeId As String, ByVal tranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType, ByVal VersionId As Integer) As List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType)
        '    Dim CyberEffDate As DateTime = Nothing
        '    If System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate") IsNot Nothing _
        '    AndAlso IsDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then
        '        CyberEffDate = CDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate"))
        '    Else
        '        CyberEffDate = CDate("9/1/2020")
        '    End If
        '    If System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate") IsNot Nothing AndAlso IsDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then CyberEffDate = CDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate"))

        '    ' THIS LIST DEFINES THE ORDER THAT THE COVERAGES WILL BE SHOWN AND SAVED IN
        '    Dim supportedCoverages As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType)

        '    Dim isMobileHome As Boolean = False


        '    '******************************    Included Coverages   *******************************
        '    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BusinessPropertyIncreased)
        '    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms)
        '    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs)
        '    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Money)
        '    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Securities)
        '    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SilverwareGoldwarePewterware)
        '    '******************************    Included Coverages   *******************************

        '    If CDate(EffDate) < CyberEffDate OrElse ((tranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse tranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage) AndAlso VersionId < 195) Then
        '        ' All quotes with effective date < HOM Cyber cutoff date OR HO-4 get the old enhancement coverages
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Equipment_Breakdown_Coverage)
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement)
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Family_Cyber_Protection)
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement)
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains)
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement)
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage)
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage) ' Cov. A - Specified Additional Amount of Insurance (29-034)
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake)
        '    Else
        '        ' After 9/1/2020 we add HOM Cyber and also the Homeowners Enhancement and Homeowners Plus Enhancement get replaced by new coverages.
        '        ' OLD HO Enhancement is 1010, NEW is 1019
        '        ' OLD HO+ Enhancement is 1017, NEW is 1020
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Equipment_Breakdown_Coverage)
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement)
        '        If FormTypeId = "25" OrElse StructureTypeId = "2" Then 'OrElse SeasonalOrSecondaryOccupancy Then
        '            ' Ho-4 or Mobile Home get the old enhancement and no cyber
        '            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement)
        '        Else
        '            ' non-HO4/mobile home get the new enhancement
        '            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Family_Cyber_Protection) ' new
        '            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement1019)
        '        End If
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains)
        '        If FormTypeId = "25" OrElse StructureTypeId = "2" Then
        '            ' Ho-4/Mobile Home gets the old HO+ enhancement.  Does not apply to Seasonal/Secondary
        '            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement)
        '        Else
        '            ' non-HO4 gets the new HO+ enhancement if not seasonal or secondary
        '            If OccupancyCodeId.EqualsAny("4", "5") = False Then
        '                supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020) ' new
        '            End If
        '        End If
        '        ' Don't add water damage for Seasonal or Secondary because we didnt' add the plus enhancement 
        '        If Not OccupancyCodeId.EqualsAny("4", "5") Then
        '            supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage)
        '        End If
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage) ' Cov. A - Specified Additional Amount of Insurance (29-034)
        '        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake)
        '    End If
        '    Return supportedCoverages
        'End Function

        'Updated 6/9/2022 for task 74187 MLW
        'Public Shared Function GetSupportedPrimaryCoverages(ByVal EffDate As String, FormTypeId As String, ByVal SeasonalOrSecondaryOccupancy As Boolean, ByVal StructureTypeId As String, ByVal OccupancyCodeId As String, ByVal tranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType, ByVal VersionId As Integer) As List(Of Common.Helpers.HOM.SectionICovTypeAndAssociate)
        Public Shared Function GetSupportedPrimaryCoverages(ByVal EffDate As String, FormTypeId As String, ByVal SeasonalOrSecondaryOccupancy As Boolean, ByVal StructureTypeId As String, ByVal OccupancyCodeId As String, ByVal tranType As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType, ByVal VersionId As Integer, ByVal RatingVersionId As Integer) As List(Of Common.Helpers.HOM.SectionICovTypeAndAssociate)
            Dim CyberEffDate As DateTime = Nothing
            If System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate") IsNot Nothing _
            AndAlso IsDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then
                CyberEffDate = CDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate"))
            Else
                CyberEffDate = CDate("9/1/2020")
            End If
            If System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate") IsNot Nothing AndAlso IsDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then CyberEffDate = CDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate"))

            ' THIS LIST DEFINES THE ORDER THAT THE COVERAGES WILL BE SHOWN AND SAVED IN
            Dim supportedCoverages As New List(Of Common.Helpers.HOM.SectionICovTypeAndAssociate)

            Dim isMobileHome As Boolean = False


            '******************************    Included Coverages   *******************************
            supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BusinessPropertyIncreased})
            If IFM.VR.Common.Helpers.HOM.HOMTheftOfFirearmsIncrease.HOMTheftOfFirearmsEnabled() = False Then
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms})
            End If
            If IFM.VR.Common.Helpers.HOM.HOMTheftOfJewelryIncrease.HOMTheftOfJewelryEnabled() = False Then
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs})
            End If
            supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Money})
            supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Securities})
            supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SilverwareGoldwarePewterware})
            '******************************    Included Coverages   *******************************

            If CDate(EffDate) < CyberEffDate OrElse ((tranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote OrElse tranType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage) AndAlso VersionId < 195) Then
                ' All quotes with effective date < HOM Cyber cutoff date OR HO-4 get the old enhancement coverages
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Equipment_Breakdown_Coverage})
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement})
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Family_Cyber_Protection})
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement})
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains})
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement})
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage})
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage}) ' Cov. A - Specified Additional Amount of Insurance (29-034)
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake})
            Else
                ' After 9/1/2020 we add HOM Cyber and also the Homeowners Enhancement and Homeowners Plus Enhancement get replaced by new coverages.
                ' OLD HO Enhancement is 1010, NEW is 1019
                ' OLD HO+ Enhancement is 1017, NEW is 1020
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Equipment_Breakdown_Coverage})
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement})
                If FormTypeId = "25" OrElse StructureTypeId = "2" Then
                    ' Ho-4 or Mobile Home get the old enhancement and no cyber (cyber added to FO2 mobile)
                    supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement})
                    supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains, .AssociatedSectionICoverageEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement})
                    'Added 10/6/2022 for task 51260 MLW
                    If FormTypeId = "22" AndAlso StructureTypeId = "2" Then 'FormTypeId 22 = FO2, StructureTypeId 2 = mobile
                        supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Family_Cyber_Protection})
                    End If
                    ' Ho-4/Mobile Home gets the old HO+ enhancement.  Does not apply to Seasonal/Secondary
                    supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement})
                    ' Don't add water damage for Seasonal or Secondary because we didn't add the plus enhancement 
                    If Not OccupancyCodeId.EqualsAny("4", "5") Then
                        supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage, .AssociatedSectionICoverageEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement})
                    End If
                Else
                    ' non-HO4/mobile home get the new enhancement
                    supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Family_Cyber_Protection}) ' new
                    supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement1019})
                    supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains, .AssociatedSectionICoverageEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement1019})

                    ' non-HO4 gets the new HO+ enhancement if not seasonal or secondary
                    If OccupancyCodeId.EqualsAny("4", "5") = False Then
                        supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020}) ' new
                        ' Don't add water damage for Seasonal or Secondary because we didnt' add the plus enhancement 
                        supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage, .AssociatedSectionICoverageEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020})
                    End If
                    'Added 5/16/2022 for task 74106 MLW, Updated 6/9/2022 for task 74187 MLW
                    'If Not OccupancyCodeId.EqualsAny("4", "5") AndAlso HOM_General.HPEEWaterBUEnabled() AndAlso CDate(EffDate) >= HOM_General.HPEEWaterBUEffDate Then
                    If Not OccupancyCodeId.EqualsAny("4", "5") AndAlso HOM_General.HPEEWaterBUEnabled() AndAlso (CDate(EffDate) >= HOM_General.HPEEWaterBUEffDate() OrElse HOM_General.HPEEWaterBackupValidForEndorsements(tranType, RatingVersionId)) Then
                        supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains, .AssociatedSectionICoverageEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020})
                    End If
                End If
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage}) ' Cov. A - Specified Additional Amount of Insurance (29-034)
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake})
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.UnitOwnersCoverageA}) ' Cov. A - Specified Additional Amount of Insurance (29-034)
                supportedCoverages.Add(New SectionICovTypeAndAssociate With {.SectionICoverageEnum = QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.UnitOwnersCoverageCSpecialCoverage})
            End If
            Return supportedCoverages
        End Function
        ''' <summary>
        ''' Get coverages that are supported via VR. It doesn't take into account whether a coverage is supported for a specific form type or any other condition on the current quote.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetSupportedSectionICoverages(Optional ByVal topQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing) As List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType)
            'Updated 1/4/18 for HOM Upgrade MLW - added optional parameter quote object
            ' THIS LIST DEFINES THE ORDER THAT THE COVERAGES WILL BE SHOWN AND SAVED IN
            Dim supportedCoverages As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType)
            'Updated 1/4/18 for HOM Upgrade MLW
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If

                Dim HomeVersion As String = GetHomeVersion(topQuote)
                If (topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso topQuote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    'New Forms - order is slightly different for old and new forms - new forms have some new coverages for HOM Upgrade MLW
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BuildingAdditionsAndAlterations)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BroadenedResidencePremisesDefinition) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.CovBOtherStructuresAwayFromTheResidencePremises) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.FunctionalReplacementCostLossAssessment)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.IdentityFraudExpenseHOM0455) 'new coverage For HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment_Earthquake) 'add With Comp rater Project
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovA)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovAAndB)
                    'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises) 'Replaces Cov_B_Related_Private_Structures
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyatOtherResidenceIncreaseLimit)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertySelfStorageFacilities) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ReplacementCostForNonBuildingStructures) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SinkholeCollapse)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialComputerCoverage) '  add With Comp rater Project
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialPersonalProperty) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises) 'Specified Other Structures - Off Premises (92-147)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TheftofBuildingMaterial) ' Matt A Bug 7356 8-12-2016
                    If IFM.VR.Common.Helpers.HOM.HOMTheftOfFirearmsIncrease.HOMTheftOfFirearmsEnabled() Then
                        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms)
                    End If
                    If IFM.VR.Common.Helpers.HOM.HOMTheftOfJewelryIncrease.HOMTheftOfJewelryEnabled() Then
                        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs)
                    End If
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TheftOfPersonalPropertyInDwellingUnderConstruction) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.UndergroundServiceLine) 'new coverage for HOM Upgrade
                Else
                    'Old Forms
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.BuildingAdditionsAndAlterations)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc)
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.DebrisRemoval)
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Consent_to_Move_Mobile_Home)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.FunctionalReplacementCostLossAssessment)


                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.IncreasedLimitsMotorizedVehicles)

                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment) 'add With Comp rater Project
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment_Earthquake) 'add With Comp rater Project

                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovA)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovAAndB)

                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.MobileHomeLienholdersSingleInterest)
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.OrdinanceOrLaw)
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyatOtherResidenceIncreaseLimit)

                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.RefrigeratedProperty)

                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SinkholeCollapse)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialComputerCoverage) '  add With Comp rater Project
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises) 'Specified Other Structures - Off Premises (92-147)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures) 'Specified Other Structures - On Premises (92-049)      add With Comp rater Project
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TheftofBuildingMaterial) ' Matt A Bug 7356 8-12-2016
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionICoverage.HOM_SectionICoverageType.TripCollision)
                    'availableCoverages.Add() 'Outdoor Antennas (ML-49)

                End If
            End If
            Return supportedCoverages
        End Function

        ''' <summary>
        ''' Looks at the quote and based on form type will any coverages be visible for this section. So any coverages that are not included and not [not available]. This allows you to not show sections when nothing would be visible anyway.
        ''' </summary>
        ''' <param name="quote"></param>
        ''' <returns></returns>
        Public Shared Function SectionIHasVisibleCoveragesAvailable(quote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            Dim VRSupportedSectionICoverages = GetSupportedSectionICoverages(quote) 'Updated 1/4/18 for HOM Upgrade MLW - added optional parameter quote object
            If VRSupportedSectionICoverages.Any() Then
                For Each supportedEnumType In VRSupportedSectionICoverages
                    Dim covInfo = GetCoverageDisplayProperties(quote, quote.Locations.GetItemAtIndex(0), QuickQuoteSectionCoverageType.SectionICoverage, supportedEnumType, QuickQuoteSectionIICoverage.SectionIICoverageType.None, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None, False)
                    If covInfo.MyDisplayType <> DisplayType.included AndAlso covInfo.MyDisplayType <> DisplayType.notAvailable Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Function




        ''' <summary>
        ''' Get coverages that are supported via VR. It doesn't take into account whether a coverage is supported for a specific form type or any other condition on the current quote.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetSupportedSectionIICoverages(Optional ByVal topQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing) As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType)
            'Updated 1/4/18 for HOM Upgrade MLW - added optional parameter quote object
            ' THIS LIST DEFINES THE ORDER THAT THE COVERAGES WILL BE SHOWN AND SAVED IN
            Dim supportedCoverages As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType)

            'Updated 1/4/18 for HOM Upgrade MLW
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If

                Dim HomeVersion As String = GetHomeVersion(topQuote)
                If (topQuote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso topQuote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    'New Forms - order is slightly different for old and new forms - new forms have some new coverages for HOM Upgrade MLW
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured) 'add With Comp rater Project
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther) 'add With Comp rater Project
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Clerical) 'add With Comp rater Project
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability) 'IncidentalFarmersPersonalLiability-On Premises
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.LowPowerRecreationalMotorVehicleLiability) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PersonalInjury)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence) 'add With Comp rater Project
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.SpecialEventCoverage)  ' added 10/21/2021 per task 52156
                Else
                    'Old Forms
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured) 'add With Comp rater Project
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther) 'add With Comp rater Project


                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.Animal_Collision)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Clerical) 'add With Comp rater Project
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_SalesPerson_ExcludingInstallation) ' removed from compRater
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_SalesPerson_IncludingInstallation) ' removed from compRater
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_LabEtc__ExcludingCorporalPunishment) ' removed from compRater
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_LabEtc__IncludingCorporalPunishment) ' removed from compRater
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment) ' removed from compRater
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_Other_IncludingCorporalPunishment) ' removed from compRater
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres)
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured160_500Acres)
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsuredOver500Acres)
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.HomeDayCareLiability)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability)
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured)
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_ResidencePremises) 'add With Comp rater Project
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.) 'add With Comp rater Project
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence) 'add With Comp rater Project
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PersonalInjury)
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.SpecialEventCoverage)
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.WaterbedCoverage)
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIICoverage.HOM_SectionIICoverageType._3Or4FamilyLiability)
                End If
            End If
            Return supportedCoverages
        End Function

        ''' <summary>
        ''' Looks at the quote and based on form type will any coverages be visible for this section. So any coverages that are not included and not [not available]. This allows you to not show sections when nothing would be visible anyway.
        ''' </summary>
        ''' <param name="topQuote"></param>
        ''' <returns></returns>
        Public Shared Function SectionIIHasVisibleCoveragesAvailable(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            Dim VRSupportedSectionIICoverages = GetSupportedSectionIICoverages(topQuote) 'Updated 1/4/18 for HOM Upgrade MLW - added optional parameter quote object
            If VRSupportedSectionIICoverages.Any() Then
                For Each supportedEnumType In VRSupportedSectionIICoverages
                    Dim covInfo = GetCoverageDisplayProperties(topQuote, topQuote.Locations.GetItemAtIndex(0), QuickQuoteSectionCoverageType.SectionIICoverage, QuickQuoteSectionICoverage.SectionICoverageType.None, supportedEnumType, QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.None, False)
                    If covInfo.MyDisplayType <> DisplayType.included AndAlso covInfo.MyDisplayType <> DisplayType.notAvailable Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Function





        ''' <summary>
        ''' Get coverages that are supported via VR. It doesn't take into account whether a coverage is supported for a specific form type or any other condition on the current quote.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetSupportedSectionIAndIICoverages(Optional ByVal quote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing) As List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)
            'Updated 1/8/18 for HOM Upgrade MLW - added optional parameter quote object
            ' THIS LIST DEFINES THE ORDER THAT THE COVERAGES WILL BE SHOWN AND SAVED IN
            Dim supportedCoverages As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType)
            'Updated 1/8/18 for HOM Upgrade MLW
            If quote IsNot Nothing Then
                If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Dim HomeVersion As String = GetHomeVersion(quote)
                If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                    'New Forms - order is different for old and new forms - new forms have some new coverages for HOM Upgrade MLW
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.LossAssessment) 'moved from Section I to Section I & II for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold) 'new coverage for HOM Upgrade
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures) ' HO-42
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers) 'add With Comp rater Project - HO-40
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement) 'New coverage For HOM Upgrade
                    If IFM.VR.Common.Helpers.HOM.UnitOwnersRentalToOthers.UnitOwnersRentalToOthersEnabled() Then
                        supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers)
                    End If
                Else
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers) 'add With Comp rater Project - HO-40
                    supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures) ' HO-42
                    'supportedCoverages.Add(QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers)
                End If
            End If
            Return supportedCoverages
        End Function

        ''' <summary>
        ''' Looks at the quote and based on form type will any coverages be visible for this section. So any coverages that are not included and not [not available]. This allows you to not show sections when nothing would be visible anyway.
        ''' </summary>
        ''' <param name="topQuote"></param>
        ''' <returns></returns>
        Public Shared Function SectionIandIIHasVisibleCoveragesAvailable(topQuote As QuickQuote.CommonObjects.QuickQuoteObject) As Boolean
            If topQuote IsNot Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            Dim VRSupportedSectionIandIICoverages = GetSupportedSectionIAndIICoverages(topQuote) 'Updated 1/8/18 for HOM Upgrade MLW
            If VRSupportedSectionIandIICoverages.Any() Then
                For Each supportedEnumType In VRSupportedSectionIandIICoverages
                    Dim covInfo = GetCoverageDisplayProperties(topQuote, topQuote.Locations.GetItemAtIndex(0), QuickQuoteSectionCoverageType.SectionIAndIICoverage, QuickQuoteSectionICoverage.SectionICoverageType.None, QuickQuoteSectionIICoverage.SectionIICoverageType.None, supportedEnumType, False)
                    If covInfo.MyDisplayType <> DisplayType.included AndAlso covInfo.MyDisplayType <> DisplayType.notAvailable Then
                        Return True
                    End If
                Next
            End If
            Return False
        End Function







        Public Shared Function GetCoverageDisplayProperties(quote As QuickQuoteObject,
                                          location As QuickQuoteLocation,
                                          SectionType As QuickQuoteSectionCoverageType,
                                          SectionCoverageIEnum As QuickQuoteSectionICoverage.HOM_SectionICoverageType,
                                          SectionCoverageIIEnum As QuickQuoteSectionIICoverage.SectionIICoverageType,
                                          SectionCoverageIAndIIEnum As QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType,
                                          Optional IncludeFormNumberInName As Boolean = True,
                                          Optional AssociatedSectionICoverageEnum As QuickQuoteSectionICoverage.HOM_SectionICoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.None) As CoverageDisplayProperties

            Dim CyberEffDate As DateTime = Nothing
            If System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate") IsNot Nothing _
            AndAlso IsDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then
                CyberEffDate = CDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate"))
            Else
                CyberEffDate = CDate("9/1/2020")
            End If

            If System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate") IsNot Nothing AndAlso IsDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate")) Then CyberEffDate = CDate(System.Configuration.ConfigurationManager.AppSettings("VR_Home_Cyber_EffDate"))

            Dim covProperties As New CoverageDisplayProperties()
            covProperties.AssociatedSectionICoverageEnum = AssociatedSectionICoverageEnum

            Dim CurrentFormTypeIdFunc = Function() As Int32
                                            'Updated 8/24/18 for multi state MLW
                                            'If location.IsNotNull Then
                                            If location IsNot Nothing Then
                                                If Int32.TryParse(location.FormTypeId, Nothing) Then
                                                    Return CInt(location.FormTypeId)
                                                End If
                                            End If
                                            Return -1
                                        End Function

            Dim CurrentFormTypeId As Int32 = CurrentFormTypeIdFunc()
            Dim StructureTypeId As String = quote.Locations(0).StructureTypeId

            'Added 11/29/17 for HOM Upgrade MLW
            Dim QQHelper As New QuickQuoteHelperClass
            Dim CurrentForm As String = QQHelper.GetShortFormName(quote)
            Dim HomeVersion As String = GetHomeVersion(quote)

            If quote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If

            Dim yearBuilt As Int32 = 0
            Int32.TryParse(location.YearBuilt, yearBuilt)

            Dim isMineCounty As Boolean = IFM.VR.Common.Helpers.MineSubsidenceHelper.LocationAllowsMineSubsidence(location)
            Dim isSeasonalOrSecondary As Boolean = False
            If quote.Locations(0).OccupancyCodeId = "4" OrElse quote.Locations(0).OccupancyCodeId = "5" Then isSeasonalOrSecondary = True

            ' HERE YOU SET COVERAGE NAME, DISPLAY TYPE, INCLUDED LIMITS, DEFAULT VALUES
            Select Case SectionType
                Case SectionCoverage.QuickQuoteSectionCoverageType.SectionICoverage
                    Select Case SectionCoverageIEnum

                        '******************************    Included Coverages   *******************************
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.BusinessPropertyIncreased
                            If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                                covProperties.Coveragename = "Business Property Increased Limits" + If(IncludeFormNumberInName, " (HO-312)", "")

                                'Updated 11/29/17 for HOM Upgrade MLW
                                'If CurrentFormTypeId.NotEqualsAny(ML2, ML4) Then
                                If CurrentForm.NotEqualsAny("ML-2", "ML-4") Then
                                    covProperties.MyDisplayType = DisplayType.included
                                    covProperties.IncludedLimit = "2,500"
                                Else
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                End If
                            Else
                                covProperties.Coveragename = "Business Property Increased Limits" + If(IncludeFormNumberInName, " (HO 0412)", "")
                                covProperties.MyDisplayType = DisplayType.included
                                covProperties.IncludedLimit = "2,500"
                            End If


                        'Case Credit Card is now sometimes an included but can increase the limit for non-ML forms

                        '11/14/2022 MLW - Moved to SectionICoverages - now allows for increased limit
                        'Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms
                        '    If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                        '        covProperties.Coveragename = "Firearms" + If(IncludeFormNumberInName, " (HO-65/HO-221)", "")
                        '        'Updated 11/29/17 for HOM Upgrade MLW
                        '        'If CurrentFormTypeId.NotEqualsAny(ML2, ML4) Then
                        '        If CurrentForm.NotEqualsAny("ML-2", "ML-4") Then
                        '            covProperties.IncludedLimit = "2,000"
                        '        Else
                        '            covProperties.IncludedLimit = "500"
                        '        End If
                        '        covProperties.MyDisplayType = DisplayType.included
                        '    End If
                        '12/12/2022 MLW - Moved to SectionICoverages - now allows for increased limit
                        'Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs
                        '    If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                        '        covProperties.Coveragename = "Jewelry, Watches & Furs" + If(IncludeFormNumberInName, " (HO-65/HO-221)", "")
                        '        'Updated 11/29/17 for HOM Upgrade MLW
                        '        'If CurrentFormTypeId.NotEqualsAny(ML2, ML4) Then
                        '        If CurrentForm.NotEqualsAny("ML-2", "ML-4") Then
                        '            covProperties.IncludedLimit = "1,000"
                        '        Else
                        '            covProperties.IncludedLimit = "500"
                        '        End If

                        '        covProperties.MyDisplayType = DisplayType.included
                        '    End If
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Money
                            If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                                covProperties.Coveragename = "Money" + If(IncludeFormNumberInName, " (HO-65/HO-221)", "")
                                'Updated 11/29/17 for HOM Upgrade MLW
                                'If CurrentFormTypeId.NotEqualsAny(ML2, ML4) Then
                                If CurrentForm.NotEqualsAny("ML-2", "ML-4") Then
                                    covProperties.IncludedLimit = "200"
                                Else
                                    covProperties.IncludedLimit = "100"
                                End If
                                covProperties.MyDisplayType = DisplayType.included
                            Else
                                covProperties.Coveragename = "Money"
                                covProperties.IncludedLimit = "200"
                                covProperties.MyDisplayType = DisplayType.included
                            End If
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Securities
                            If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                                covProperties.Coveragename = "Securities" + If(IncludeFormNumberInName, " (HO-65/HO-221)", "")
                                'Updated 11/29/17 for HOM Upgrade MLW
                                'If CurrentFormTypeId.NotEqualsAny(ML2, ML4) Then
                                If CurrentForm.NotEqualsAny("ML-2", "ML-4") Then
                                    covProperties.IncludedLimit = "1,000"
                                Else
                                    covProperties.IncludedLimit = "500"
                                End If
                                covProperties.MyDisplayType = DisplayType.included
                            Else
                                covProperties.Coveragename = "Securities"
                                covProperties.IncludedLimit = "1,500"
                                covProperties.MyDisplayType = DisplayType.included
                            End If
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.SilverwareGoldwarePewterware
                            If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                                covProperties.Coveragename = "Silverware, Goldware, Pewterware" + If(IncludeFormNumberInName, " (HO-65/HO-221)", "")
                                'Updated 11/29/17 for HOM Upgrade MLW
                                'If CurrentFormTypeId.NotEqualsAny(ML2, ML4) Then
                                If CurrentForm.NotEqualsAny("ML-2", "ML-4") Then
                                    covProperties.IncludedLimit = "2,500"
                                Else
                                    covProperties.IncludedLimit = "1,000"
                                End If
                                covProperties.MyDisplayType = DisplayType.included
                            Else
                                covProperties.Coveragename = "Silverware, Goldware, Pewterware"
                                covProperties.IncludedLimit = "2,500"
                                covProperties.MyDisplayType = DisplayType.included
                            End If
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.OrdinanceOrLaw
                            If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                                covProperties.Coveragename = "Ordinance or Law" + If(IncludeFormNumberInName, " (HOM-1000)", "")
                                'Updated 11/29/17 for HOM Upgrade MLW
                                'If CurrentFormTypeId.NotEqualsAny(ML2, ML4) Then
                                If CurrentForm.NotEqualsAny("ML-2", "ML-4") Then
                                    covProperties.MyDisplayType = DisplayType.included
                                Else
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                End If
                            End If
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.IncreasedLimitsMotorizedVehicles
                            If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                                covProperties.Coveragename = "Increased Limits Motorized Vehicles" + If(IncludeFormNumberInName, " (ML-65)", "")
                                covProperties.MyDisplayType = DisplayType.hasIncreaseWithFreeForm
                                'Updated 11/29/17 for HOM Upgrade MLW
                                'If CurrentFormTypeId.EqualsAny(ML2, ML4) Then
                                If CurrentForm.EqualsAny("ML-2", "ML-4") Then
                                    covProperties.MyDisplayType = DisplayType.included
                                    covProperties.IncludedLimit = "1,000"
                                Else
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                End If
                            End If
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Fire_Department_Service_Charge
                            'Updated 12/27/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Fire Department Service Charge" ' + If(IncludeFormNumberInName, " (ML-306)", "")
                                covProperties.IncludedLimit = "500"
                                covProperties.IsDefaultCoverage = True
                                covProperties.MyDisplayType = DisplayType.hasIncreaseWithFreeForm
                            Else
                                covProperties.Coveragename = "Fire Department Service Charge" + If(IncludeFormNumberInName, " (ML-306)", "")
                                covProperties.MyDisplayType = DisplayType.hasIncreaseWithFreeForm
                                'Updated 11/29/17 for HOM Upgrade MLW
                                'If CurrentFormTypeId.EqualsAny(ML2, ML4) Then
                                If CurrentForm.EqualsAny("ML-2", "ML-4") Then
                                    covProperties.MyDisplayType = DisplayType.included
                                    covProperties.IncludedLimit = "500"
                                Else
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                End If
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Outdoor_Antenna_Satellite_Dish
                            If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                                covProperties.Coveragename = "Outdoor Antennas" + If(IncludeFormNumberInName, " (ML-49)", "")
                                'Updated 11/29/17 for HOM Upgrade MLW
                                'If CurrentFormTypeId.EqualsAny(ML2, ML4) Then
                                If CurrentForm.EqualsAny("ML-2", "ML-4") Then
                                    covProperties.MyDisplayType = DisplayType.included
                                    covProperties.IncludedLimit = "500"
                                Else
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                End If
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Inflation_Guard
                            If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                                covProperties.Coveragename = "Inflation Guard"
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Family_Cyber_Protection
                            'Updated 10/6/2022 for task 51260 MLW
                            'If CDate(quote.EffectiveDate) >= CyberEffDate AndAlso (CurrentFormTypeId <> 25 AndAlso StructureTypeId <> "2" AndAlso Not isSeasonalOrSecondary) Then
                            If CDate(quote.EffectiveDate) >= CyberEffDate AndAlso ((CurrentFormTypeId <> 25 AndAlso StructureTypeId <> "2" AndAlso Not isSeasonalOrSecondary) OrElse (CurrentFormTypeId = "22" AndAlso StructureTypeId = "2" AndAlso Not isSeasonalOrSecondary)) Then
                                'covProperties.Coveragename = "Cyber Family Protection (HOM 1018)"
                                covProperties.Coveragename = "Family Cyber Protection" + If(IncludeFormNumberInName, " (HOM 1018)", "")
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                                covProperties.IsDefaultCoverage = True
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If
                            Exit Select

                            '******************************    END  Included Coverages   *******************************



                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Equipment_Breakdown_Coverage
                            'Updated 12/21/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Equipment Breakdown Coverage" + If(IncludeFormNumberInName, " (HOM 1011)", "")
                            Else
                                covProperties.Coveragename = "Equipment Breakdown Coverage" + If(IncludeFormNumberInName, " (92-132)", "")
                            End If
                            covProperties.IsDefaultCoverage = True
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "HO-2"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "HO-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "ML-2", "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyReplacement
                            covProperties.IsDefaultCoverage = True
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                            'Updated 12/21/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Personal Property Replacement Cost" + If(IncludeFormNumberInName, " (HO 0490)", "")
                                'Removed 6/11/18 for bug 27203 MLW
                                'Select Case CurrentForm
                                '    Case "HO-4"
                                '        covProperties.MyDisplayType = DisplayType.notAvailable
                                'End Select
                            Else
                                covProperties.Coveragename = "Personal Property Replacement Cost" + If(IncludeFormNumberInName, " (HO-290 / 92-195)", "")
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.Coveragename = "Personal Property Replacement Cost" + If(IncludeFormNumberInName, " (ML-55)", "")
                                End Select
                            End If
                            'Select Case CurrentFormTypeId
                            '    Case ML2, ML4
                            '        covProperties.Coveragename = "Personal Property Replacement Cost" + If(IncludeFormNumberInName, " (ML-55)", "")
                            'End Select
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement1019
                            'If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso CDate(quote.EffectiveDate) >= CyberEffDate AndAlso (quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "26") AndAlso StructureTypeId <> "2" AndAlso isSeasonalOrSecondary = False)) Then
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso CDate(quote.EffectiveDate) >= CyberEffDate AndAlso (quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "26") AndAlso StructureTypeId <> "2")) Then
                                covProperties.Coveragename = "Home Enhancement Endorsement " + If(IncludeFormNumberInName, " (HOM 1019)", "")
                                covProperties.IsDefaultCoverage = True
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownerEnhancementEndorsement
                            'Updated 12/21/17 for HOM Upgrade MLW
                            'If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso (CDate(quote.EffectiveDate) < CyberEffDate OrElse quote.Locations(0).OccupancyCodeId.EqualsAny("4", "5")) AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            'If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso (CDate(quote.EffectiveDate) < CyberEffDate) AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                            '    covProperties.Coveragename = "Homeowners Enhancement Endorsement" + If(IncludeFormNumberInName, " (HOM 1010)", "")
                            'Else
                            '    covProperties.Coveragename = "Homeowner Enhancement Endorsement" + If(IncludeFormNumberInName, " (92-267)", "")
                            'End If
                            covProperties.Coveragename = "Homeowners Enhancement Endorsement" + If(IncludeFormNumberInName, " (HOM 1010)", "")

                            'Updated 11/29/17 for HOM Upgrade MLW
                            If CurrentForm.NotEqualsAny("ML-2", "ML-4") Then
                                covProperties.IsDefaultCoverage = True
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If
                            'If CurrentFormTypeId.NotEqualsAny(ML2, ML4) Then
                            '    covProperties.IsDefaultCoverage = True
                            'End If

                            'Select Case CurrentFormTypeId
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.BackupSewersAndDrains
                            'Updated 12/21/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Water Backup" + If(IncludeFormNumberInName, " (N/A)", "")
                            Else
                                covProperties.Coveragename = "Backup Of Sewer Or Drain" + If(IncludeFormNumberInName, " (92-173)", "")
                            End If
                            'Updated 11/29/17 for HOM Upgrade MLW
                            If CurrentForm.NotEqualsAny("HO-4", "HO-6", "ML-2", "ML-4") Then
                                covProperties.IsDefaultCoverage = True
                            End If
                            'If CurrentFormTypeId.NotEqualsAny(HO4, HO6, ML2, ML4) Then
                            '    covProperties.IsDefaultCoverage = True
                            'End If
                            covProperties.IncludedLimit = "5,000"
                            covProperties.MyDisplayType = DisplayType.hasIncreasewithDropDown
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "ML-2", "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case HO4
                            '    'covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '   ' covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4 'need new mobile form types added
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement1020
                            covProperties.Coveragename = "Home Plus Enhancement Endorsement" + If(IncludeFormNumberInName, " (HOM 1020)", "")
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso CDate(quote.EffectiveDate) >= CyberEffDate AndAlso (quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") AndAlso StructureTypeId <> "2" AndAlso isSeasonalOrSecondary = False)) Then
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        '1/16/18 - added for HOM Upgrade MLW
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.HomeownersPlusEnhancementEndorsement
                            'show label / Coveragename, do Not show for mobile, cannot select this And homeowner enhancement
                            covProperties.Coveragename = "Homeowners Plus Enhancement Endorsement" + If(IncludeFormNumberInName, " (HOM 1017)", "")
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") AndAlso isSeasonalOrSecondary = False) Then
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case Else
                                        covProperties.MyDisplayType = DisplayType.justCheckBox
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If


                        '1/16/18 - added for HOM Upgrade MLW
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.WaterDamage
                            covProperties.Coveragename = "Water Damage" + If(IncludeFormNumberInName, " (N/A)", "")
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26") AndAlso isSeasonalOrSecondary = False) Then
                                covProperties.IncludedLimit = "5,000"
                                covProperties.MyDisplayType = DisplayType.hasIncreasewithDropDown
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                        '    Case Else
                                        '        covProperties.MyDisplayType = DisplayType.justCheckBox
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_CoverageASpecialCoverage
                            'Not available with QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement selected
                            'Updated 12/21/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Cov.A - Specified Additional Amount Of Insurance" + If(IncludeFormNumberInName, " (HO 0420)", "")
                            Else
                                covProperties.Coveragename = "Cov.A - Specified Additional Amount Of Insurance" + If(IncludeFormNumberInName, " (29-034)", "")
                            End If
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "HO-2", "HO-3", "HO-5"
                                    If quote.Locations(0).StructureTypeId = "20" Then ' 20 Manufactured structure type
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "HO-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "HO-6"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "ML-2", "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Earthquake
                            'Updated 12/21/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Earthquake" + If(IncludeFormNumberInName, " (HOM 1014)", "")
                            Else
                                covProperties.Coveragename = "Earthquake" + If(IncludeFormNumberInName, " (HO-315B)", "")
                                If CurrentForm.EqualsAny("ML-2", "ML-4") Then
                                    covProperties.Coveragename = "Earthquake" + If(IncludeFormNumberInName, " (ML-54)", "")
                                End If
                            End If
                            covProperties.MyDisplayType = DisplayType.justDeductible
                            'If CurrentFormTypeId.EqualsAny(ML2, ML4) Then
                            '    covProperties.Coveragename = "Earthquake" + If(IncludeFormNumberInName, " (ML-54)", "")
                            'End If



                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlement
                            ' not available with SectionICoverageType.Home_CoverageASpecialCoverage selected
                            'Updated 12/28/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                If quote.Locations(0).FormTypeId = "22" AndAlso quote.Locations(0).StructureTypeId.Equals("2") Then
                                    covProperties.Coveragename = "Actual Cash Value Loss Settlement" + If(IncludeFormNumberInName, " (MH 0402)", "")
                                Else
                                    covProperties.Coveragename = "Actual Cash Value Loss Settlement" + If(IncludeFormNumberInName, " (HO 0481)", "")
                                End If
                            Else
                                covProperties.Coveragename = "Actual Cash Value Loss Settlement" + If(IncludeFormNumberInName, " (HO-04 81)", "")
                            End If

                            covProperties.MyDisplayType = DisplayType.justCheckBox
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "HO-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "HO-6"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "ML-2"
                                    'Updated 12/28/17 for HOM Upgrade MLW
                                    If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId = "22") Then
                                        covProperties.IsDefaultCoverage = True
                                    Else
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.ActualCashValueLossSettlementWindstormOrHailLossestoRoofSurfacing
                            'Updated 12/28/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                If quote.Locations(0).FormTypeId = "22" AndAlso quote.Locations(0).StructureTypeId.Equals("2") Then
                                    covProperties.Coveragename = "Actual Cash Value Loss Settlement/Windstorm Or Hail Losses To Roof Surfacing" + If(IncludeFormNumberInName, " (MH 0425)", "")
                                Else
                                    covProperties.Coveragename = "Actual Cash Value Loss Settlement/Windstorm Or Hail Losses To Roof Surfacing" + If(IncludeFormNumberInName, " (HOM 1013)", "")
                                End If
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                                'Updated 11/29/17 for HOM Upgrade MLW
                                Select Case CurrentForm
                                    Case "HO-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.Coveragename = "Actual Cash Value Loss Settlement/Windstorm Or Hail Losses To Roof Surfacing" + If(IncludeFormNumberInName, " (HO-04 93)", "")
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                                'Updated 11/29/17 for HOM Upgrade MLW
                                Select Case CurrentForm
                                    Case "HO-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            End If
                            'Select Case CurrentFormTypeId
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        'added 1/15/18 for HOM Upgrade MLW
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.BroadenedResidencePremisesDefinition
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.MyDisplayType = DisplayType.hasEffectiveAndExpirationDates
                                Select Case CurrentForm
                                    Case "HO-2", "HO-3", "HO-4", "ML-4" 'HO-3 is both HO 0003 and HO 0005
                                        covProperties.Coveragename = "Broadened Residence Premises Definition" + If(IncludeFormNumberInName, " (HO 0469)", "")
                                    Case "HO-6"
                                        covProperties.Coveragename = "Broadened Residence Premises Definition" + If(IncludeFormNumberInName, " (HO 1747)", "")
                                    Case "ML-2"
                                        covProperties.Coveragename = "Broadened Residence Premises Definition" + If(IncludeFormNumberInName, " (MH 0427)", "")
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If



                        'added 1/9/18 for HOM Upgrade MLW
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.CovBOtherStructuresAwayFromTheResidencePremises
                            covProperties.Coveragename = "Cov. B Other Structures Away from the Residence Premises" + If(IncludeFormNumberInName, " (HO 0491)", "")
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                                Select Case CurrentForm
                                    Case "HO-4", "HO-6", "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.SinkholeCollapse
                            'Updated 12/26/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Sinkhole Collapse" + If(IncludeFormNumberInName, " (HO 0499)", "")
                            Else
                                covProperties.Coveragename = "Sinkhole Collapse" + If(IncludeFormNumberInName, " (HO-99)", "")
                            End If
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "HO-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "HO-6"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "ML-2", "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.CreditCardFundTransForgeryEtc
                            'Updated 12/26/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Credit Card, Electronic Fund Transfer Card or Access Device, Forgery and Counterfeit Money Coverage – Increased Limit" + If(IncludeFormNumberInName, " (HO 0453)", "")
                                covProperties.IsDefaultCoverage = True
                                covProperties.IncludedLimit = "2,500"
                                covProperties.MyDisplayType = DisplayType.hasIncreasewithDropDown
                            Else
                                covProperties.Coveragename = "Credit Card, Fund Transfer Card, Forgery And Counterfeit Money Coverage" + If(IncludeFormNumberInName, " (HO-53)", "")
                                covProperties.IsDefaultCoverage = True
                                covProperties.IncludedLimit = "2,500"
                                covProperties.MyDisplayType = DisplayType.hasIncreasewithDropDown
                                'Updated 11/29/17 for HOM Upgrade MLW
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                        covProperties.Coveragename = "Credit Card, Fund Transfer Card, Forgery And Counterfeit Money Coverage" + If(IncludeFormNumberInName, " (ML-63)", "")
                                End Select
                            End If
                            'Select Case CurrentFormTypeId
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '        covProperties.Coveragename = "Credit Card, Fund Transfer Card, Forgery And Counterfeit Money Coverage" + If(IncludeFormNumberInName, " (ML-63)", "")
                            'End Select                       

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.FunctionalReplacementCostLossAssessment
                            'Updated 12/26/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Functional Replacement Cost Loss Settlement" + If(IncludeFormNumberInName, " (HO 0530)", "")
                            Else
                                covProperties.Coveragename = "Functional Replacement Cost Loss Settlement" + If(IncludeFormNumberInName, " (HO-05 30)", "")
                            End If
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "HO-2"
                                    If yearBuilt >= 1947 Then
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "HO-3" 'HO-3 is HO-3, HO-3_15, HO 0003 and HO 0005
                                    If yearBuilt >= 1947 Then
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "HO-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "HO-6"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "ML-2", "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '        If yearBuilt >= 1947 Then
                            '            covProperties.MyDisplayType = DisplayType.notAvailable
                            '        End If
                            '    Case HO3, HO3_15
                            '        If yearBuilt >= 1947 Then
                            '            covProperties.MyDisplayType = DisplayType.notAvailable
                            '        End If
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.GreenUpgrades
                            'Added 12/27/17 for HOM Upgrade MLW
                            covProperties.Coveragename = "Green Upgrades" + If(IncludeFormNumberInName, " (HO 0631)", "")
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.MyDisplayType = DisplayType.isGreenUpgrades
                                Select Case CurrentForm
                                    Case "HO-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.IdentityFraudExpenseHOM0455
                            'Added 12/27/17 for HOM Upgrade MLW
                            covProperties.Coveragename = "Identity Fraud Expense" + If(IncludeFormNumberInName, " (HO 0455)", "")
                            ' Not available after 9/1/2020 except on HO-4 (id 25) per cyber update MGB 6/16/20 
                            If CDate(quote.EffectiveDate) >= CyberEffDate AndAlso CurrentFormTypeId <> "25" Then
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            Else
                                If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                    covProperties.MyDisplayType = DisplayType.justCheckBox
                                    covProperties.IncludedLimit = "15,000"
                                Else
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                End If
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.BuildingAdditionsAndAlterations
                            covProperties.Coveragename = "Building Additions And Alterations" + If(IncludeFormNumberInName, " (HO-51)", "")
                            covProperties.MyDisplayType = DisplayType.hasIncreaseWithFreeForm
                            'Updated 11/29/17 for HOM Upgrade MLW
                            covProperties.MyDisplayType = DisplayType.notAvailable
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO3, HO3_15
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment
                            'Updated 1/23/18 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            Else
                                covProperties.Coveragename = "Loss Assessment" + If(IncludeFormNumberInName, " (HO-35)", "")
                                covProperties.IncludedLimit = "1,000"
                                covProperties.MyDisplayType = DisplayType.hasIncreasewithDropDown
                                'Updated 11/29/17 for HOM Upgrade MLW
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                                'Select Case CurrentFormTypeId
                                '    'Case HO2
                                '    '    covProperties.MyDisplayType = DisplayType.notAvailable
                                '    'Case HO3, HO3_15
                                '    '    covProperties.MyDisplayType = DisplayType.notAvailable
                                '    'Case HO4
                                '    '    covProperties.MyDisplayType = DisplayType.notAvailable
                                '    'Case HO6
                                '    Case ML2, ML4 
                                '        covProperties.MyDisplayType = DisplayType.notAvailable
                                'End Select
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment_Earthquake
                            'Updated 12/26/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Loss Assessment - Earthquake" + If(IncludeFormNumberInName, " (HO 0436)", "")
                                'removed included limit and changed display type back to justLimit
                                ''added included limit and displaytype 10/23/17 for HOM Upgrade MLW
                                'covProperties.IncludedLimit = "5,000"
                                covProperties.MyDisplayType = DisplayType.justLimit
                            Else
                                covProperties.Coveragename = "Loss Assessment - Earthquake" + If(IncludeFormNumberInName, " (HO-35B)", "")
                                covProperties.MyDisplayType = DisplayType.justLimit
                            End If

                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "ML-2", "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case ML2, ML4 
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovA
                            'Updated 12/26/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Mine Subsidence Cov A" + If(IncludeFormNumberInName, " (HOM 1009)", "")
                            Else
                                covProperties.Coveragename = "Mine Subsidence Cov A" + If(IncludeFormNumberInName, " (92-074)", "")
                            End If
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                            'updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "HO-2"
                                    If isMineCounty = False Then
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "HO-3" 'HO-3 is HO-3, HO-3_15, HO 0003, and HO 0005
                                    If isMineCounty = False Then
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "HO-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "HO-6"
                                    If isMineCounty = False Then
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "ML-2", "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '        If isMineCounty = False Then
                            '            covProperties.MyDisplayType = DisplayType.notAvailable
                            '        End If
                            '    Case HO3, HO3_15
                            '        If isMineCounty = False Then
                            '            covProperties.MyDisplayType = DisplayType.notAvailable
                            '        End If
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '        If isMineCounty = False Then
                            '            covProperties.MyDisplayType = DisplayType.notAvailable
                            '        End If
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.MineSubsidenceCovAAndB
                            'Updated 1/2/18 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Mine Subsidence Cov A & B" + If(IncludeFormNumberInName, " (HOM 1009)", "")
                            Else
                                covProperties.Coveragename = "Mine Subsidence Cov A & B" + If(IncludeFormNumberInName, " (92-074)", "")
                            End If
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "HO-2"
                                    If isMineCounty = False Then
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "HO-3"
                                    If isMineCounty = False Then
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "HO-4"
                                    'Updated 1/2/18 for HOM Upgrade MLW
                                    If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId = "25") Then
                                        If isMineCounty = False Then
                                            covProperties.MyDisplayType = DisplayType.notAvailable
                                            'Else
                                            '    'Removed 5/11/18 for Bug 26643 MLW
                                            '    covProperties.IsDefaultCoverage = True
                                        End If
                                    Else
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "HO-6"
                                    'Updated 1/2/18 for HOM Upgrade MLW
                                    If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                        If isMineCounty = False Then
                                            covProperties.MyDisplayType = DisplayType.notAvailable
                                        End If
                                    Else
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "ML-2", "ML-4"
                                    'Updated 1/2/18 for HOM Upgrade MLW
                                    If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "25")) Then
                                        If isMineCounty = False Then
                                            covProperties.MyDisplayType = DisplayType.notAvailable
                                            'Else
                                            '    'Removed 5/11/18 for Bug 26643 MLW
                                            '    If quote.Locations(0).FormTypeId = "25" Then
                                            '        covProperties.IsDefaultCoverage = True
                                            '    End If
                                        End If
                                    Else
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '        If isMineCounty = False Then
                            '            covProperties.MyDisplayType = DisplayType.notAvailable
                            '        End If
                            '    Case HO3, HO3_15
                            '        If isMineCounty = False Then
                            '            covProperties.MyDisplayType = DisplayType.notAvailable
                            '        End If
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Cov_B_Related_Private_Structures
                            'Updated 1/2/18 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                                'covProperties.MyDisplayType = DisplayType.notAvailable 'uses OtherStructuresOnTheResidencePremises
                                covProperties.Coveragename = "Other Structures On the Residence Premises" + If(IncludeFormNumberInName, " (HO 0448)", "")
                                covProperties.MyDisplayType = DisplayType.isOtherStructures
                            Else
                                covProperties.Coveragename = "Specified Other Structures - On Premises" + If(IncludeFormNumberInName, " (92-049)", "")
                                covProperties.MyDisplayType = DisplayType.isOtherStructures
                                'Updated 11/29/17 for HOM Upgrade MLW
                                Select Case CurrentForm
                                    Case "HO-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "HO-6"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            End If
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '    Case HO3, HO3_15
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2
                            '    Case ML4
                            'End Select

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.OtherStructuresOnTheResidencePremises
                            'Updated 5/23/18 for Bugs 26818 and 26819 - Coverage code changed from 70303 OtherStructuresOnTheResidencePremises to 70064 Cov_B_Related_Private_Structures MLW
                            'Added 2/26/18 for HOM Upgrade MLW
                            'Replaces Cov_B_Related_Private_Structures
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                'covProperties.Coveragename = "Other Structures On the Residence Premises" + If(IncludeFormNumberInName, " (HO 0448)", "")
                                'covProperties.MyDisplayType = DisplayType.isOtherStructures
                                ''Updated 5/14/18 for Bug 26643 MLW
                                ''If isMineCounty = True AndAlso quote.Locations(0).FormTypeId = "25" Then
                                ''    covProperties.IsDefaultCoverage = True
                                ''End If
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertySelfStorageFacilities
                            'Added 12/27/17 for HOM Upgrade MLW
                            covProperties.Coveragename = "Personal Property Self Storage Facilities" + If(IncludeFormNumberInName, " (HO 0614)", "")
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.IncludedLimit = "1,000" 'included limit is 1000 or 10% of Coverage C whichever is greater
                                covProperties.MyDisplayType = DisplayType.hasIncreaseWithFreeForm
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.ReplacementCostForNonBuildingStructures
                            'Added 1/9/18 for HOM Upgrade MLW
                            covProperties.Coveragename = "Replacement Cost for Non-Building Structures" + If(IncludeFormNumberInName, " (HO 0443)", "")
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                                Select Case CurrentForm
                                    Case "HO-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "HO-6"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If


                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Home_RelatedPrivateStrucuturesAwayFromPremises
                            'Updated 12/28/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Specific Structures Away from Residence Premises" + If(IncludeFormNumberInName, " (HO 0492)", "")
                            Else
                                covProperties.Coveragename = "Specified Other Structures - Off Premises" + If(IncludeFormNumberInName, " (92-147)", "")
                            End If
                            covProperties.MyDisplayType = DisplayType.isOtherStructures
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "ML-2", "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '    Case HO3, HO3_15
                            '    Case HO4
                            '    Case HO6
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select


                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.TheftofBuildingMaterial
                            ' Only available if the year built is the same as effective date year
                            'Updated 12/26/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Theft Of Building Materials" + If(IncludeFormNumberInName, " (HOM 1002)", "")
                            Else
                                covProperties.Coveragename = "Theft Of Building Materials" + If(IncludeFormNumberInName, " (92-367)", "")
                            End If
                            Dim effectiveDateYear = If(quote.EffectiveDate.IsDate(), CDate(quote.EffectiveDate).Year, DateTime.Now.Year)
                            If yearBuilt < effectiveDateYear Then
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            Else
                                covProperties.MyDisplayType = DisplayType.justLimit
                                'Updated 11/29/17 for HOM Upgrade MLW
                                Select Case CurrentForm
                                    Case "HO-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "HO-6"
                                        'Updated 12/26/17 for HOM Upgrade
                                        If (quote.Locations(0).FormTypeId <> "26") Then
                                            covProperties.MyDisplayType = DisplayType.notAvailable
                                        End If
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                                'Select Case CurrentFormTypeId
                                '    Case HO2
                                '    Case HO3, HO3_15
                                '    Case HO4
                                '        covProperties.MyDisplayType = DisplayType.notAvailable
                                '    Case HO6
                                '        covProperties.MyDisplayType = DisplayType.notAvailable 
                                '    Case ML2, ML4
                                '        covProperties.MyDisplayType = DisplayType.notAvailable
                                'End Select
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Firearms
                            If IFM.VR.Common.Helpers.HOM.HOMTheftOfFirearmsIncrease.IsHOMTheftOfFirearmsIncreaseAvailable(quote) Then
                                covProperties.Coveragename = "Theft of Firearms - Special Limits of Liability"
                                covProperties.IsDefaultCoverage = True
                                covProperties.IncludedLimit = "2,500"
                                covProperties.MyDisplayType = DisplayType.hasIncreaseWithFreeForm
                            Else
                                If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                                    covProperties.Coveragename = "Firearms" + If(IncludeFormNumberInName, " (HO-65/HO-221)", "")
                                    'Updated 11/29/17 for HOM Upgrade MLW
                                    'If CurrentFormTypeId.NotEqualsAny(ML2, ML4) Then
                                    If CurrentForm.NotEqualsAny("ML-2", "ML-4") Then
                                        covProperties.IncludedLimit = "2,000"
                                    Else
                                        covProperties.IncludedLimit = "500"
                                    End If
                                    covProperties.MyDisplayType = DisplayType.included
                                End If
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.JewelryWatchesAndFurs
                            If IFM.VR.Common.Helpers.HOM.HOMTheftOfJewelryIncrease.IsHOMTheftOfJewelryIncreaseAvailable(quote) Then
                                covProperties.Coveragename = "Theft of Jewelry, Watches, Furs & Precious Stones - Special Limits of Liability"
                                covProperties.IsDefaultCoverage = True
                                covProperties.IncludedLimit = "1,500"
                                covProperties.MyDisplayType = DisplayType.hasIncreaseWithFreeForm
                            Else
                                If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                                    covProperties.Coveragename = "Jewelry, Watches & Furs" + If(IncludeFormNumberInName, " (HO-65/HO-221)", "")
                                    'Updated 11/29/17 for HOM Upgrade MLW
                                    'If CurrentFormTypeId.NotEqualsAny(ML2, ML4) Then
                                    If CurrentForm.NotEqualsAny("ML-2", "ML-4") Then
                                        covProperties.IncludedLimit = "1,000"
                                    Else
                                        covProperties.IncludedLimit = "500"
                                    End If

                                    covProperties.MyDisplayType = DisplayType.included
                                End If
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.TheftOfPersonalPropertyInDwellingUnderConstruction
                            'Added 12/27/17 for HOM Upgrade MLW
                            covProperties.Coveragename = "Theft of Personal Property in Dwelling Under Construction" + If(IncludeFormNumberInName, " (HO 0607)", "")
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.MyDisplayType = DisplayType.hasEffAndExpDatesWithLimit
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.UndergroundServiceLine
                            'Added 1/9/18 for HOM Upgrade MLW
                            covProperties.Coveragename = "Underground Service Line Coverage" + If(IncludeFormNumberInName, " (HOM 1016)", "")
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.MyDisplayType = DisplayType.justLimit
                                covProperties.IncludedLimit = "10,000" 'Added 4/26/18 for Bug 26128 MLW
                                Select Case CurrentForm
                                    Case "HO-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.UnitOwnersCoverageA
                            'Added 3/10/23 for HOM Upgrade BD
                            covProperties.Coveragename = "Unit Owners Coverage A - Special Coverage" + If(IncludeFormNumberInName, " (HO 1732)", "")
                            Select Case CurrentForm
                                Case "HO-6"
                                    covProperties.MyDisplayType = DisplayType.justCheckBox
                                Case Else 'HO-3 is HO-3, HO-3_15, HO 0003, and HO 0005
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.UnitOwnersCoverageCSpecialCoverage
                            'Added 3/7/23 for HOM Upgrade BD
                            covProperties.Coveragename = "Unit Owners Coverage C - Special Coverage" + If(IncludeFormNumberInName, " (HO 1731)", "")
                            Select Case CurrentForm
                                Case "HO-6"
                                    covProperties.MyDisplayType = DisplayType.justCheckBox
                                Case Else 'HO-3 is HO-3, HO-3_15, HO 0003, and HO 0005
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.Farm_Consent_to_Move_Mobile_Home
                            covProperties.Coveragename = "Consent To Move Mobile Home" + If(IncludeFormNumberInName, " (ML-25)", "")
                            covProperties.MyDisplayType = DisplayType.justEffectiveDate
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.DebrisRemoval
                            covProperties.Coveragename = "Debris Removal" + If(IncludeFormNumberInName, " (92-267)", "")
                            covProperties.MyDisplayType = DisplayType.hasIncreasewithDropDown

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.PersonalPropertyatOtherResidenceIncreaseLimit
                            If QQHelper.doUseNewVersionOfLOB(quote, QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, "7/1/2018") = False Then
                                covProperties.Coveragename = "Personal Property - Other Residence" + If(IncludeFormNumberInName, " (HO-50)", "")
                                covProperties.MyDisplayType = DisplayType.included
                            Else
                                covProperties.Coveragename = "Personal Property at Other Residences" + If(IncludeFormNumberInName, " (HO 0450)", "")
                                covProperties.MyDisplayType = DisplayType.hasIncreaseWithLocation
                                covProperties.IncludedLimit = "1,000" 'included limit is 1000 or 10% of Coverage C whichever is greater
                            End If
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.RefrigeratedProperty
                            covProperties.Coveragename = "Refrigerated Food Products" + If(IncludeFormNumberInName, " (92-267)", "")
                            covProperties.MyDisplayType = DisplayType.hasIncreasewithDropDown

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialComputerCoverage
                            'Updated 1/4/18 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Special Computer Coverage" + If(IncludeFormNumberInName, " (HO 0414)", "")
                                'Not available with Special Personal Property Coverage
                            Else
                                covProperties.Coveragename = "Special Computer Coverage" + If(IncludeFormNumberInName, " (HO-314)", "")
                            End If
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "HO-3" 'HO-3 is HO-3, HO-3_15, HO 0003, and HO 0005
                                    If CurrentFormTypeId = 3 OrElse CurrentFormTypeId = 24 Then
                                        covProperties.MyDisplayType = DisplayType.notAvailable ' BRD said HO-3 was available but not HO-3_15
                                    End If
                                Case "HO-6"
                                    If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId = "26") Then
                                        'removed 1/4/18 for HOM Upgrade MLW
                                    Else
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "ML-2", "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case HO3_15
                            '        covProperties.MyDisplayType = DisplayType.notAvailable ' BRD said HO-3 was available but not HO-3_15
                            '    Case HO6
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.SpecialPersonalProperty
                            'Added 12/27/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Special Personal Property Coverage" + If(IncludeFormNumberInName, " (HO 0524)", "")
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                                'not available with Special Computer Coverage
                                Select Case CurrentForm
                                    Case "HO-2"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "HO-3"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "HO-6"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If
                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.TripCollision
                            covProperties.Coveragename = "Trip Collision" + If(IncludeFormNumberInName, " (ML-26)", "")
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO3, HO3_15
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select


                        Case QuickQuoteSectionICoverage.HOM_SectionICoverageType.MobileHomeLienholdersSingleInterest
                            covProperties.Coveragename = "Vendor's Single Interest" + If(IncludeFormNumberInName, " (ML-27)", "")
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                    End Select ' END SECTIONI




                Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIICoverage
                    Select Case SectionCoverageIIEnum
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType._3Or4FamilyLiability
                            covProperties.Coveragename = "3 or 4 Family Liability" + If(IncludeFormNumberInName, " (HO-74)", "")
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                            'Updated 11/29/17 for HOM Upgrade MLW
                            covProperties.MyDisplayType = DisplayType.notAvailable
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO3, HO3_15
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select


                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmersPersonalLiability
                            'Updated 1/5/17 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Incidental Farming Personal Liability - On Premises" + If(IncludeFormNumberInName, " (HO 2472)", "")
                                covProperties.MyDisplayType = DisplayType.justDescription
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.Coveragename = "Incidental Farming Personal Liability" + If(IncludeFormNumberInName, " (HO-72)", "")
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                                'Updated 11/29/2017 for HOM Upgrade MLW
                                Select Case CurrentForm
                                    Case "HO-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "HO-6"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            End If
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '    Case HO3, HO3_15
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.IncidentalFarmingPersonalLiability_OffPremises
                            'Added 1/17/18 for HOM Upgrade MLW 
                            covProperties.Coveragename = "Incidental Farming Personal Liability - Off Premises" + If(IncludeFormNumberInName, " (HO 2472)", "")
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.MyDisplayType = DisplayType.isFarmLand
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.LowPowerRecreationalMotorVehicleLiability
                            'Added 1/17/18 for HOM Upgrade MLW
                            covProperties.Coveragename = "Low Power Recreational Motor Vehicle Liability" + If(IncludeFormNumberInName, " (HO 2413)", "")
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.CanineLiabilityExclusion
                            'Added 1/18/18 for HOM Upgrade MLW
                            covProperties.Coveragename = "Canine Liability Exclusion" + If(IncludeFormNumberInName, " (HO 2477)", "")
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.MyDisplayType = DisplayType.isCanine
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                            'Updated 1/11/18 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Farm Owned and Operated By Insured: 0-100 Acres" + If(IncludeFormNumberInName, " (HO 2446)", "")
                                covProperties.MyDisplayType = DisplayType.isFarmLand
                                'Updated 11/29/17 for HOM Upgrade MLW
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.Coveragename = "Farm Owned and Operated By Insured: 0-100 Acres" + If(IncludeFormNumberInName, " (HO-73)", "")
                                covProperties.MyDisplayType = DisplayType.isFarmLand
                                'Updated 11/29/17 for HOM Upgrade MLW
                                Select Case CurrentForm
                                    Case "HO-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "HO-6"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            End If
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '    Case HO3, HO3_15
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PersonalInjury
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Personal Injury" + If(IncludeFormNumberInName, " (HO 2482)", "")
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                            Else
                                covProperties.Coveragename = "Personal Injury" + If(IncludeFormNumberInName, " (HO-82)", "")
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                            End If
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.AdditionalResidenceRentedToOther
                            'Updated 1/5/18 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Additional Residence - Rented to Others" + If(IncludeFormNumberInName, " (HO 2470)", "")
                                covProperties.MyDisplayType = DisplayType.isAdditionlResidence ' added for comp rater project
                            Else
                                covProperties.Coveragename = "Additional Residence - Rented to Others" + If(IncludeFormNumberInName, " (HO-70)", "")
                                covProperties.MyDisplayType = DisplayType.isAdditionlResidence ' added for comp rater project
                                'Updated 11/29/17 for HOM Upgrade MLW
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            End If
                            'Select Case CurrentFormTypeId
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.OtherLocationOccupiedByInsured
                            'Updated 6/13/2019 for Bug 31870 MLW
                            'covProperties.Coveragename = "Additional Residence - Occupied by Insured" + If(IncludeFormNumberInName, " (N/A)", "")
                            covProperties.Coveragename = "Other Insured Location Occupied by Insured"
                            covProperties.MyDisplayType = DisplayType.isAdditionlResidence ' added for comp rater project
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "ML-2", "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '        'covProperties.Coveragename = "Additional Residence - Occupied by Insured (ML-67)"
                            'End Select

                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Clerical
                            'Updated 1/24/18 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Business Pursuits - Clerical" + If(IncludeFormNumberInName, " (HO 2471)", "")
                                covProperties.MyDisplayType = DisplayType.isBusinessPursuits
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.Coveragename = "Business Pursuits - Clerical" + If(IncludeFormNumberInName, " (HO-71)", "")
                                covProperties.MyDisplayType = DisplayType.justCheckBox
                            End If

                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_SalesPerson_ExcludingInstallation
                            covProperties.Coveragename = "Sales Person Excluding Installation" + If(IncludeFormNumberInName, " (HO-71)", "")
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_SalesPerson_IncludingInstallation
                            covProperties.Coveragename = "Sales Person Including Installation" + If(IncludeFormNumberInName, " (HO-71)", "")
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_LabEtc__ExcludingCorporalPunishment
                            covProperties.Coveragename = "Teacher Lab Etc. Excluding Corporal Punishment" + If(IncludeFormNumberInName, " (HO-71)", "")
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_LabEtc__IncludingCorporalPunishment
                            covProperties.Coveragename = "Teacher Lab Etc. Including Corporal Punishment" + If(IncludeFormNumberInName, " (HO-71)", "")
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_Other_ExcludingCorporalPunishment
                            covProperties.Coveragename = "Teacher Other Excluding Corporal Punishment" + If(IncludeFormNumberInName, " (HO-71)", "")
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Teacher_Other_IncludingCorporalPunishment
                            covProperties.Coveragename = "Teacher Other Including Corporal Punishment" + If(IncludeFormNumberInName, " (HO-71)", "")
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsured0_160Acres
                            covProperties.Coveragename = "Farm Owned and Operated By Insured: 160-500 Acres" + If(IncludeFormNumberInName, " (HO-73)", "")
                            covProperties.MyDisplayType = DisplayType.isFarmLand
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.FarmOwnedAndOperatedByInsuredOver500Acres
                            covProperties.Coveragename = "Farm Owned and Operated By Insured: Over 500 Acres" + If(IncludeFormNumberInName, " (HO-73)", "")
                            covProperties.MyDisplayType = DisplayType.isFarmLand
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.HomeDayCareLiability
                            covProperties.Coveragename = "Home Day Care" + If(IncludeFormNumberInName, " (HO-323)", "")
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                        'moved to IandII below - 10/26/17 for HOM Upgrade MLW
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_ResidencePremises
                            'Updated 1/23/18 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            Else
                                covProperties.Coveragename = "Permitted Incidental Occupancies - Residence Premises" + If(IncludeFormNumberInName, " (HO-42)", "") + " (Liability Only)"
                                covProperties.MyDisplayType = DisplayType.justDescription
                            End If

                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.PermittedIncidentalOccupancies_OtherResidence
                            'Updated 1/12/18 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Permitted Incidental Occupancies Other Residence" + If(IncludeFormNumberInName, " (HO 2443)", "")
                                covProperties.MyDisplayType = DisplayType.isOtherStructures
                            Else
                                covProperties.Coveragename = "Permitted Incidental Occupancies Other Residence" + If(IncludeFormNumberInName, " (HO-43)", "")
                                covProperties.MyDisplayType = DisplayType.isOtherStructures
                            End If
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "ML-2", "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.SpecialEventCoverage
                            covProperties.Coveragename = "Special Event Coverage (HOM 1005)"
                            covProperties.MyDisplayType = DisplayType.isSpecialEventCoverage

                            If quote IsNot Nothing AndAlso quote.Locations IsNot Nothing AndAlso quote.Locations.Count > 0 Then
                                ' Special event coverage not available for seasonal or secondary occupancies
                                If isSeasonalOrSecondary Then
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                End If
                                ' Special event coverage not available for mobile homes
                                If quote.Locations(0).StructureTypeId = "2" Then
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                End If
                                If quote.Locations(0).Address IsNot Nothing Then
                                    ' Special event coverage only available in Indiana
                                    If Not quote.Locations(0).Address.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Indiana Then
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                End If
                            End If
                        Case QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.WaterbedCoverage
                            covProperties.Coveragename = "Waterbed Liability" + If(IncludeFormNumberInName, " (HO-85)", "")
                            covProperties.MyDisplayType = DisplayType.justCheckBox
                    End Select ' END SECTIONII





                Case SectionCoverage.QuickQuoteSectionCoverageType.SectionIAndIICoverage
                    Select Case SectionCoverageIAndIIEnum
                        'Added 1/23/18 for HOM Upgrade MLW
                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AdditionalInsured_StudentLivingAwayFromResidence
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Additional Insured – Student Living Away from Residence" + If(IncludeFormNumberInName, " (HO 0527)", "")
                                covProperties.MyDisplayType = DisplayType.isAdditionalInsured
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        'Added 1/23/18 for HOM Upgrade MLW
                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.AssistedLivingCareCoverage
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Assisted Living Care Coverage" + If(IncludeFormNumberInName, " (HO 0459)", "")
                                covProperties.MyDisplayType = DisplayType.isAdditionalInterests
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        'Added 1/22/18 for HOM Upgrade MLW
                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.LossAssessment
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Loss Assessment" + If(IncludeFormNumberInName, " (HO 0435)", "")
                                covProperties.IncludedLimit = "1,000"
                                covProperties.IsDefaultCoverage = True
                                covProperties.MyDisplayType = DisplayType.hasIncreasewithDropDown
                                'covProperties.MyDisplayType = DisplayType.isLossAssessment '3/16/18 loss assessment changed, no longer need address info & multiples
                                Select Case CurrentForm
                                    Case "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        'Added 1/23/18 for HOM Upgrade MLW
                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.OtherMembersOfYourHousehold
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Other Members of Your Household" + If(IncludeFormNumberInName, " (HO 0458)", "")
                                covProperties.MyDisplayType = DisplayType.isOtherMembers
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                            'Updated 1/23/18 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Permitted Incidental Occupancies Residence Premises" + If(IncludeFormNumberInName, " (HO 0442)", "")
                                covProperties.MyDisplayType = DisplayType.isBusinessStructure
                            Else
                                covProperties.Coveragename = "Permitted Incidental Occupancies Residence Premises - Other Structures" + If(IncludeFormNumberInName, " (HO-42)", "") + " (Property and Liability)"
                                covProperties.MyDisplayType = DisplayType.hasLimitAndDescription
                            End If
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "ML-2", "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '    Case HO3, HO3_15
                            '    Case HO4
                            '    Case HO6
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select


                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.Home_OptLiability_RelatedPrivateStructuresRentedtoOthers
                            'updated 1/8/18 for HOM Upgrade MLW
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Structures Rented To Others - Residence Premises" + If(IncludeFormNumberInName, " (HO 0440)", "")
                            Else
                                covProperties.Coveragename = "Structures Rented To Others" + If(IncludeFormNumberInName, " (HO-40)", "") + " (Property and Liability)"
                            End If
                            covProperties.MyDisplayType = DisplayType.hasLimitAndDescription
                            'Updated 11/29/17 for HOM Upgrade MLW
                            Select Case CurrentForm
                                Case "HO-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "HO-6"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                                Case "ML-2"
                                    If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId = "22") Then
                                    Else
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                    End If
                                Case "ML-4"
                                    covProperties.MyDisplayType = DisplayType.notAvailable
                            End Select
                            'Select Case CurrentFormTypeId
                            '    Case HO2
                            '    Case HO3, HO3_15
                            '    Case HO4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case HO6
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            '    Case ML2, ML4
                            '        covProperties.MyDisplayType = DisplayType.notAvailable
                            'End Select

                            'Added 1/23/18 for HOM Upgrade MLW
                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.TrustEndorsement
                            If (quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal AndAlso HomeVersion = "After20180701" AndAlso quote.Locations(0).FormTypeId.EqualsAny("22", "23", "24", "25", "26")) Then
                                covProperties.Coveragename = "Trust Endorsement" + If(IncludeFormNumberInName, " (HO 0615)", "")
                                covProperties.MyDisplayType = DisplayType.isTrust
                                Select Case CurrentForm
                                    Case "HO-4", "ML-2", "ML-4"
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                        Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers
                            covProperties.Coveragename = "Unit Owners Rental to Others" + If(IncludeFormNumberInName, " (HO 1733)", "")
                            If IFM.VR.Common.Helpers.HOM.UnitOwnersRentalToOthers.IsUnitOwnersRentalToOthersAvailable(quote) Then
                                covProperties.IsDefaultCoverage = True
                                Select Case CurrentForm
                                    Case "HO-6"
                                        If quote.Locations(0).OccupancyCodeId = "9" Then
                                            covProperties.MyDisplayType = DisplayType.justCheckBox
                                        Else
                                            covProperties.MyDisplayType = DisplayType.notAvailable
                                        End If                                    
                                    Case Else
                                        covProperties.MyDisplayType = DisplayType.notAvailable
                                End Select
                            Else
                                covProperties.MyDisplayType = DisplayType.notAvailable
                            End If

                            'Case QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.UnitOwnersRentaltoOthers
                            '    covProperties.Coveragename = "Permitted Incidental Occupancies - Other Residence" + If(IncludeFormNumberInName, " (HO-43)","")
                            '    covProperties.MyDisplayType = DisplayType.isPermittedIncidentalOccupied43
                    End Select ' END SECTIONI&II


            End Select
            Return covProperties
        End Function

        'Added 2/23/2022 for task 64829 MLW
        Public Function UnderlyingSectionICoverage() As QuickQuoteSectionICoverage
            Dim sc As QuickQuoteSectionICoverage = Nothing

            If SectionType = QuickQuoteSectionCoverageType.SectionICoverage Then
                sc = DirectCast(_sectionCoverage, QuickQuoteSectionICoverage)
            End If

            Return sc
        End Function
        'Added 2/23/2022 for task 64829 MLW
        Public Function UnderlyingSectionIICoverage() As QuickQuoteSectionIICoverage
            Dim sc As QuickQuoteSectionIICoverage = Nothing

            If SectionType = QuickQuoteSectionCoverageType.SectionIICoverage Then
                sc = DirectCast(_sectionCoverage, QuickQuoteSectionIICoverage)
            End If

            Return sc
        End Function
        'Added 2/23/2022 for task 64829 MLW
        Public Function UnderlyingSectionIandIICoverage() As QuickQuoteSectionIAndIICoverage
            Dim sc As QuickQuoteSectionIAndIICoverage = Nothing

            If SectionType = QuickQuoteSectionCoverageType.SectionIAndIICoverage Then
                sc = DirectCast(_sectionCoverage, QuickQuoteSectionIAndIICoverage)
            End If

            Return sc
        End Function

        ''' <summary>
        ''' Clears the coverages back to the included and defaults. It does not remove all optional coverages.
        ''' </summary>
        Public Shared Sub ClearCoverages(topQuote As QuickQuoteObject,
                                     location As QuickQuoteLocation)
            'Updated 8/24/18 for multi state MLW
            'If quote.IsNotNull AndAlso location.IsNotNull Then
            If topQuote Is Nothing Then
                If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
            'If location.SectionICoverages.IsNotNull Then
            If location.SectionICoverages IsNot Nothing Then
                Dim removeList As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionICoverage)
                For Each cov1 In location.SectionICoverages
                    Dim sc As New SectionCoverage(cov1)
                    Dim covProperties = GetCoverageDisplayProperties(topQuote, location, sc.SectionType, sc.SectionCoverageIEnum, sc.SectionCoverageIIEnum, sc.SectionCoverageIAndIIEnum)
                    If covProperties.IsDefaultCoverage = False And covProperties.MyDisplayType <> DisplayType.included Then
                        removeList.Add(sc._sectionCoverage)
                    Else
                        If covProperties.IsDefaultCoverage Then
                            'remove increased limits back to original
                            cov1.IncreasedLimit = ""
                            cov1.IncreasedLimitId = ""
                        End If
                    End If
                Next
                For Each c In removeList
                    location.SectionICoverages.Remove(c)
                Next
            End If

            'Updated 8/24/18 for multi state MLW
            'If location.SectionIICoverages.IsNotNull Then
            If location.SectionIICoverages IsNot Nothing Then
                Dim removeList As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIICoverage)
                For Each cov1 In location.SectionIICoverages
                    Dim sc As New SectionCoverage(cov1)
                    Dim covProperties = GetCoverageDisplayProperties(topQuote, location, sc.SectionType, sc.SectionCoverageIEnum, sc.SectionCoverageIIEnum, sc.SectionCoverageIAndIIEnum)
                    If covProperties.IsDefaultCoverage = False And covProperties.MyDisplayType <> DisplayType.included Then
                        removeList.Add(sc._sectionCoverage)
                    Else
                        If covProperties.IsDefaultCoverage Then
                            'remove increased limits back to original
                            cov1.IncreasedLimit = ""
                            cov1.IncreasedLimitId = ""
                        End If
                    End If
                Next
                For Each c In removeList
                    location.SectionIICoverages.Remove(c)
                Next
            End If

            'Updated 8/24/18 for multi state MLW
            'If location.SectionIAndIICoverages.IsNotNull Then
            If location.SectionIAndIICoverages IsNot Nothing Then
                Dim removeList As New List(Of QuickQuote.CommonObjects.QuickQuoteSectionIAndIICoverage)
                For Each cov1 In location.SectionIAndIICoverages
                    Dim sc As New SectionCoverage(cov1)
                    Dim covProperties = GetCoverageDisplayProperties(topQuote, location, sc.SectionType, sc.SectionCoverageIEnum, sc.SectionCoverageIIEnum, sc.SectionCoverageIAndIIEnum)
                    If covProperties.IsDefaultCoverage = False And covProperties.MyDisplayType <> DisplayType.included Then
                        removeList.Add(sc._sectionCoverage)
                    Else
                        If covProperties.IsDefaultCoverage Then
                            'remove increased limits back to original
                            cov1.PropertyIncreasedLimit = ""
                        End If
                    End If
                Next
                For Each c In removeList
                    location.SectionIAndIICoverages.Remove(c)
                Next
            End If

        End Sub

    End Class

    Public Class CoverageDisplayProperties
        Public Property MyDisplayType As DisplayType
        Public Property Coveragename As String
        Public Property IncludedLimit As String
        Public Property IsDefaultCoverage As Boolean
        'Added 5/16/2022 for task 74106 MLW
        Public Property AssociatedSectionICoverageEnum As QuickQuoteSectionICoverage.HOM_SectionICoverageType

        Public Sub New()
            Me.MyDisplayType = DisplayType.notAvailable
            Me.Coveragename = "Unknown"
            Me.IncludedLimit = ""
            Me.IsDefaultCoverage = False
            'Added 5/16/2022 for task 74106 MLW
            Me.AssociatedSectionICoverageEnum = QuickQuoteSectionICoverage.HOM_SectionICoverageType.None
        End Sub

    End Class

    'Added 5/16/2022 for task 74106 MLW
    Public Class SectionICovTypeAndAssociate
        Public Property SectionICoverageEnum As QuickQuoteSectionICoverage.HOM_SectionICoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.None
        Public Property AssociatedSectionICoverageEnum As QuickQuoteSectionICoverage.HOM_SectionICoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.None
    End Class

End Namespace
