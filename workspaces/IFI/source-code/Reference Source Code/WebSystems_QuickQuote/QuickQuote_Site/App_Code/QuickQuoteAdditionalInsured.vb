Imports Microsoft.VisualBasic
Imports QuickQuote.CommonMethods 'added 6/30/2015

Namespace QuickQuote.CommonObjects 'added namespace 12/20/2013
    ''' <summary>
    ''' object used to hold additional insured information
    ''' </summary>
    ''' <remarks>equates to a specific coverage on the quote</remarks>
    <Serializable()> _
    Public Class QuickQuoteAdditionalInsured
        Inherits QuickQuoteBaseGenericObject(Of Object) 'added 8/4/2014
        Implements IDisposable

        ''' <summary>
        ''' valid types for additional insureds
        ''' </summary>
        ''' <remarks>value corresponds to coveragecode_id in Diamond</remarks>
        Enum QuickQuoteAdditionalInsuredType '12/23/2013 note: will need to update to use static data file
            None = 0
            AdditionalInsuredControllingInterest = 926 'added 11/13/2012 for GL (equivalent to BOP's 501 - Controlling Interest)
            CoOwnerOfInsuredPremises = 21018
            ControllingInterest = 501
            DesignatedPersonOrOrganization = 21022
            EngineersArchitectsOrSurveyors = 21019
            EngineersArchitectsOrSurveyorsNotEngagedByTheNamedInsured = 21023
            GrantorOfFranchise = 21144 '3/9/2017 - BOP stuff
            LessorOfLeasedEquipment = 21020
            ManagersOrLessorsOfPremises = 21053
            MortgageeAssigneeOrReceiver = 21054
            OwnerOrOtherInterestsFromWhomLandHasBeenLeased = 21055
            OwnersLesseesOrContractors = 21024 'Not Used in BOP anymore; 3/9/2017 - BOP stuff
            OwnersLesseesOrContractorsBOP = 80368 '3/9/2017 - BOP stuff
            OwnersLesseesOrContractorsCompletedOperations = 21081 '3/9/2017 - BOP stuff
            OwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract = 21025
            OwnersLesseesOrContractorsWithAdditionalInsuredRequirementForOtherParties = 80369 '3/9/2017 - BOP stuff
            OwnersLesseesOrContractorsAutomaticWithCompletedOpsAndWaiver = 80370 '3/9/2017 - BOP stuff
            StateOrPoliticalSubdivisionsPermits = 21026
            StateOrPoliticalSubdivisionsPermitsRelatingToPremises = 21016
            TownhouseAssociations = 21017
            Vendors = 21021
            WaiverOfSubrogationWhenRequiredByWrittenContract = 80371 '3/9/2017 - BOP stuff
            CityOfChicagoScaffolding = 80537 'added 8/22/2018 for IL; only CGL right now
        End Enum

        Dim qqHelper As New QuickQuoteHelperClass 'added 6/30/2015

        Private _AdditionalInsuredType As QuickQuoteAdditionalInsuredType
        Private _CoverageCodeId As String
        Private _CoverageName As String '3/9/2017 - BOP stuff
        Private _Description As String
        'added 7/19/2012 for App Gap
        Private _NameOfPersonOrOrganization As String
        Private _DesignationOfPremises As String
        Private _HasWaiverOfSubrogation As Boolean
        Private _FullTermPremium As String '3/9/2017 - BOP stuff

        'added 8/6/2012
        Private _ManualPremiumAmount As String

        'added 10/19/2012 for GL
        Private _ProductDescription As String

        Public Property AdditionalInsuredType As QuickQuoteAdditionalInsuredType '12/23/2013 note: will need to update to use static data file
            Get
                Return _AdditionalInsuredType
            End Get
            Set(value As QuickQuoteAdditionalInsuredType)
                _AdditionalInsuredType = value
                If _AdditionalInsuredType <> Nothing AndAlso _AdditionalInsuredType <> QuickQuoteAdditionalInsuredType.None Then
                    _CoverageCodeId = CInt(_AdditionalInsuredType).ToString
                    _CoverageName = GetCoverageNameFromAIType(_AdditionalInsuredType) '3/9/2017 - BOP stuff
                End If
            End Set
        End Property
        Public ReadOnly Property CoverageCodeName As String '3/9/2017 - BOP stuff
            Get
                Return _CoverageName
            End Get
        End Property
        Public Property CoverageCodeId As String '12/23/2013 note: will need to update to use static data file
            Get
                Return _CoverageCodeId
            End Get
            Set(value As String)
                _CoverageCodeId = value
                If IsNumeric(_CoverageCodeId) = True AndAlso _CoverageCodeId <> "0" Then
                    If System.Enum.IsDefined(GetType(QuickQuoteAdditionalInsuredType), CInt(_CoverageCodeId)) = True Then
                        _AdditionalInsuredType = CInt(_CoverageCodeId)
                        _CoverageName = GetCoverageNameFromAIType(_AdditionalInsuredType) '5/24/2017 - BOP stuff
                    End If
                End If
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
        Public Property NameOfPersonOrOrganization As String
            Get
                Return _NameOfPersonOrOrganization
            End Get
            Set(value As String)
                _NameOfPersonOrOrganization = value
            End Set
        End Property
        Public Property DesignationOfPremises As String
            Get
                Return _DesignationOfPremises
            End Get
            Set(value As String)
                _DesignationOfPremises = value
            End Set
        End Property
        Public Property HasWaiverOfSubrogation As Boolean
            Get
                Return _HasWaiverOfSubrogation
            End Get
            Set(value As Boolean)
                _HasWaiverOfSubrogation = value
            End Set
        End Property

        Public Property ManualPremiumAmount As String
            Get
                Return _ManualPremiumAmount
            End Get
            Set(value As String)
                _ManualPremiumAmount = value
            End Set
        End Property

        Public Property FullTermPremium As String '3/9/2017 - BOP stuff
            Get
                Return _FullTermPremium
            End Get
            Set(value As String)
                _FullTermPremium = value
            End Set
        End Property

        Public Property ProductDescription As String
            Get
                Return _ProductDescription
            End Get
            Set(value As String)
                _ProductDescription = value
            End Set
        End Property

        Public Sub New()
            MyBase.New() 'added 8/4/2014
            SetDefaults()
        End Sub
        Private Sub SetDefaults()
            _AdditionalInsuredType = QuickQuoteAdditionalInsuredType.None
            _CoverageCodeId = ""
            _Description = ""
            _NameOfPersonOrOrganization = ""
            _DesignationOfPremises = ""
            _HasWaiverOfSubrogation = False

            _ManualPremiumAmount = ""

            _ProductDescription = ""
        End Sub
        Public Overrides Function ToString() As String 'added 6/30/2015
            Dim str As String = ""
            If Me IsNot Nothing Then
                If Me.CoverageCodeId <> "" Then
                    Dim ai As String = ""
                    ai = "CoverageCodeId: " & Me.CoverageCodeId
                    If Me.AdditionalInsuredType <> QuickQuoteAdditionalInsuredType.None Then
                        ai &= " (" & System.Enum.GetName(GetType(QuickQuoteAdditionalInsuredType), Me.AdditionalInsuredType) & ")"
                    End If
                    str = qqHelper.appendText(str, ai, vbCrLf)
                End If
            Else
                str = "Nothing"
            End If
            Return str
        End Function

        Public Function GetCoverageNameFromAIType(AdditionalInsuredType As QuickQuoteAdditionalInsuredType) As String '3/9/2017 - BOP stuff
            Select Case AdditionalInsuredType
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.CoOwnerOfInsuredPremises
                    Return "Co-Owner of Insured Premises"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.ControllingInterest, QuickQuoteAdditionalInsuredType.AdditionalInsuredControllingInterest
                    Return "Controlling Interests"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.DesignatedPersonOrOrganization
                    Return "Designated Person or Organization"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors
                    Return "Engineers, Architects or Surveyors"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyorsNotEngagedByTheNamedInsured
                    Return "Engineers, Architects or Surveyors Not Engaged by the Named Insured"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.LessorOfLeasedEquipment
                    Return "Lessor of Leased Equipment"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.ManagersOrLessorsOfPremises
                    Return "Managers or Lessors of Premises"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.MortgageeAssigneeOrReceiver
                    Return "Mortgagee, Assignee Or Receiver"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnerOrOtherInterestsFromWhomLandHasBeenLeased
                    Return "Owner or Other Interests From Whom Land has been Leased"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractors, QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsBOP
                    Return "Owners, Lessees or Contractors"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract
                    Return "Owners, Lessees or Contractors - With Additional Insured Requirement in Construction Contract"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.StateOrPoliticalSubdivisionsPermitsRelatingToPremises
                    Return "State or Political Subdivisions - Permits Relating to Premises"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.StateOrPoliticalSubdivisionsPermits
                    Return "State or Political Subdivisions - Permits"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.TownhouseAssociations
                    Return "Townhouse Associations"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.Vendors
                    Return "Vendors"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsCompletedOperations
                    Return "Owners, Lessees or Contractors - Completed Operations"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsWithAdditionalInsuredRequirementForOtherParties
                    Return "Owners, Lessees or Contractors - With Additional Insured Requirement For Other Parties in Construction Contract"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsAutomaticWithCompletedOpsAndWaiver
                    Return "Owners, Lessees or Contractors - Automatic with Completed Ops and Waiver"
                Case QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.WaiverOfSubrogationWhenRequiredByWrittenContract
                    Return "Waiver Of Subrogation When Required By Written Contract"
                Case QuickQuoteAdditionalInsuredType.GrantorOfFranchise
                    Return "Grantor Of Franchise"
                Case QuickQuoteAdditionalInsuredType.CityOfChicagoScaffolding 'added 8/22/2018 for IL; only CGL right now
                    Return "City Of Chicago Scaffolding"
                Case Else
                    Return ""
            End Select
        End Function

        Public Function GetAITypeFromCoverageName(CoverageName As String, LOB As QuickQuoteObject.QuickQuoteLobType) As String '3/9/2017 - BOP stuff
            Select Case True
                Case CoverageName.Equals("Co-Owner of Insured Premises", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.CoOwnerOfInsuredPremises
                Case CoverageName.Equals("Controlling Interests", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.ControllingInterest
                Case CoverageName.Equals("Designated Person or Organization", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.DesignatedPersonOrOrganization
                Case CoverageName.Equals("Engineers, Architects or Surveyors", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyors
                Case CoverageName.Equals("Engineers, Architects or Surveyors Not Engaged by the Named Insured", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.EngineersArchitectsOrSurveyorsNotEngagedByTheNamedInsured
                Case CoverageName.Equals("Lessor of Leased Equipment", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.LessorOfLeasedEquipment
                Case CoverageName.Equals("Managers or Lessors of Premises", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.ManagersOrLessorsOfPremises
                Case CoverageName.Equals("Mortgagee, Assignee Or Receiver", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.MortgageeAssigneeOrReceiver
                Case CoverageName.Equals("Owner or Other Interests From Whom Land has been Leased", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnerOrOtherInterestsFromWhomLandHasBeenLeased
                Case CoverageName.Equals("Owners, Lessees or Contractors", StringComparison.CurrentCultureIgnoreCase)
                    If LOB = QuickQuoteObject.QuickQuoteLobType.CommercialBOP Then
                        Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsBOP
                    Else
                        Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractors
                    End If
                Case CoverageName.Equals("Owners, Lessees or Contractors - With Additional Insured Requirement in Construction Contract", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsWithAdditionalInsuredRequirementInConstructionContract
                Case CoverageName.Equals("State or Political Subdivision - Permits Relating to Premises", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.StateOrPoliticalSubdivisionsPermitsRelatingToPremises
                Case CoverageName.Equals("State or Political Subdivisions - Permits", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.StateOrPoliticalSubdivisionsPermits
                Case CoverageName.Equals("Townhouse Associations", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.TownhouseAssociations
                Case CoverageName.Equals("Vendors", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.Vendors
                Case CoverageName.Equals("Owners, Lessees or Contractors - Completed Operations", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsCompletedOperations
                Case CoverageName.Equals("Owners, Lessees or Contractors - With Additional Insured Requirement For Other Parties in Construction Contract", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsWithAdditionalInsuredRequirementForOtherParties
                Case CoverageName.Equals("Owners, Lessees or Contractors - Automatic with Completed Ops and Waiver", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.OwnersLesseesOrContractorsAutomaticWithCompletedOpsAndWaiver
                Case CoverageName.Equals("Waiver Of Subrogation When Required By Written Contract", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.WaiverOfSubrogationWhenRequiredByWrittenContract
                Case CoverageName.Equals("Grantor Of Franchise", StringComparison.CurrentCultureIgnoreCase)
                    Return QuickQuoteAdditionalInsuredType.GrantorOfFranchise
                Case CoverageName.Equals("City Of Chicago Scaffolding", StringComparison.CurrentCultureIgnoreCase) 'added 8/22/2018 for IL; only CGL right now
                    Return QuickQuoteAdditionalInsuredType.CityOfChicagoScaffolding
                Case Else
                    Return QuickQuoteAdditionalInsured.QuickQuoteAdditionalInsuredType.None
            End Select
        End Function

        Public Function SpecificQuickQuoteState() As QuickQuoteHelperClass.QuickQuoteState 'added 8/22/2018 for multi-state
            Dim qqState As QuickQuoteHelperClass.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.None

            Select Case AdditionalInsuredType
                Case QuickQuoteAdditionalInsuredType.CityOfChicagoScaffolding
                    qqState = QuickQuoteHelperClass.QuickQuoteState.Illinois
            End Select

            Return qqState
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
                    If _AdditionalInsuredType <> Nothing Then
                        _AdditionalInsuredType = Nothing
                    End If
                    If _CoverageCodeId IsNot Nothing Then
                        _CoverageCodeId = Nothing
                    End If
                    If _Description IsNot Nothing Then
                        _Description = Nothing
                    End If
                    If _NameOfPersonOrOrganization IsNot Nothing Then
                        _NameOfPersonOrOrganization = Nothing
                    End If
                    If _DesignationOfPremises IsNot Nothing Then
                        _DesignationOfPremises = Nothing
                    End If
                    If _HasWaiverOfSubrogation <> Nothing Then
                        _HasWaiverOfSubrogation = Nothing
                    End If

                    If _ManualPremiumAmount IsNot Nothing Then
                        _ManualPremiumAmount = Nothing
                    End If

                    If _ProductDescription IsNot Nothing Then
                        _ProductDescription = Nothing
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
