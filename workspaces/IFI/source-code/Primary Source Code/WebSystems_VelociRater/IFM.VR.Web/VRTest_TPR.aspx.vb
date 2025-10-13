Imports System.Configuration.ConfigurationManager
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods

Public Class VRTest_TPR
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            txtDriverNum.Text = "1"
            rbList.Checked = True
            rbList_CheckedChanged(Me, New EventArgs())
        End If
    End Sub

    Private Sub DisplayMessage(ByVal msg As String)
        lblMsg.Text = msg
    End Sub

    Private Sub HideMsg()
        lblMsg.Text = "&nbsp;"
    End Sub

    Private Sub DisplayTxtMessage(ByVal msg As String)
        txtReportData.Text = ""
        txtReportData.Text = msg
    End Sub

    ''' <summary>
    ''' Check that the select quote matches the type passed
    ''' </summary>
    ''' <param name="QuoteType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsCorrectQuoteType(ByVal QuoteType As String) As Boolean
        Try
            Select Case QuoteType.ToUpper()
                Case "PPA"
                    If ddlQuoteID.SelectedItem.Text.ToUpper().Contains("AUTO") Then Return True
                    Exit Select
                Case "HOM"
                    If ddlQuoteID.SelectedItem.Text.ToUpper().Contains("HOME") Then Return True
                    Exit Select
                Case "PPC"
                    If ddlQuoteID.SelectedItem.Text.ToUpper().Contains("PPC") Then Return True
                    Exit Select
                Case Else
                    Throw New Exception("IsCorrectQuoteType: Invalid quote type passed.")
            End Select
        Catch ex As Exception
            DisplayMessage(ex.Message)
            Return False
        End Try
    End Function

    Private Function getQuoteID() As String
        Try
            If rbList.Checked Then
                If ddlQuoteID.SelectedIndex <= 0 Then
                    Return Nothing
                Else
                    Return ddlQuoteID.SelectedValue
                End If
            Else
                If txtQuoteID.Text.Trim() = "" Then Throw New Exception("Quote ID is required")
                If Not IsNumeric(txtQuoteID.Text) Then Throw New Exception("Quote ID must be numeric")
                Return txtQuoteID.Text.Trim()
            End If
        Catch ex As Exception
            DisplayMessage(ex.Message)
            Return Nothing
        End Try
    End Function

    'added 8/7/2014 for simple quote testing
    Private Sub HOM_save_test_new_simple(ByVal qId As String, Optional ByVal saveOrRate As QuickQuoteXML.QuickQuoteValidationType = QuickQuoteXML.QuickQuoteValidationType.Rate) 'added 7/26/2013; added optional param 8/8/2013
        Dim qqxml As New QuickQuoteXML
        Dim err As String = ""
        Dim qqHelper As New QuickQuoteHelperClass()

        Dim strQQ As String = ""
        Dim ratedQQ As QuickQuoteObject = Nothing
        Dim strRatedQQ As String = ""
        Dim quickQuote As QuickQuoteObject = Nothing

        If qId <> "" AndAlso IsNumeric(qId) = True Then
            'get existing
            qqxml.GetQuoteForSaveType(qId, QuickQuoteXML.QuickQuoteSaveType.Quote, quickQuote, err)
            If err <> "" Then
                err = "" 'just reset
            End If
        Else
            'get new
        End If

        If quickQuote Is Nothing Then
            quickQuote = New QuickQuoteObject
            With quickQuote '12/5/2013: added inner WITH block because started getting object reference error when setting a property on 1st-time quote; still caught error after completing inner WITH block; switched to 2 separate ones
                .LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal

                '.Client.ClientId = ""

                .QuoteDescription = "Test Desc"

                .RiskGrade = "1"
                .RiskGradeLookupId = "74"

                'Policyholder information will automatically be populated by Client
                'updated 7/30/2014 to set policyholder information... and then use Copy method below to set Client info
                With .Policyholder
                    With .Name
                        .FirstName = "Arnulfo"
                        .LastName = "Torres"
                        .SexId = "1"
                        .TypeId = "1"
                        .TaxNumber = ""
                        .TaxTypeId = "1"
                        .EntityTypeId = "1" 'Individual
                        .DescriptionOfOperations = "PH test"
                        .BirthDate = "5/24/1955"
                        .DriversLicenseDate = "5/24/1971"
                        .DriversLicenseNumber = "8940356442"
                        .MaritalStatusId = "2" '1=Single; 2=Married
                    End With
                    With .Address
                        .HouseNum = "3816"
                        .StreetName = "First"
                        .City = "East Chicago"
                        .Zip = "46312"
                        .County = "Lake"
                    End With

                End With

                With .Policyholder2
                    With .Name
                        .FirstName = "ELISA"
                        .LastName = "TORRES"
                        .SexId = "2"
                        .TypeId = "1"
                        .TaxNumber = ""
                        .TaxTypeId = "1"
                        .EntityTypeId = "1" 'Individual
                        .DescriptionOfOperations = "PH2 test"
                        .BirthDate = "3/23/1960"
                        .DriversLicenseDate = "3/23/1976"
                        .DriversLicenseNumber = "8902598529"
                        .MaritalStatusId = "2" '1=Single; 2=Married
                    End With
                    .Address = quickQuote.Policyholder.Address
                    .Phones = quickQuote.Policyholder.Phones
                End With
                .CopyPolicyholdersToClients() 'added 7/30/2014

                .CopyPolicyholdersToApplicants() 'added 7/30/2014; original logic is in IF below
                If .Applicants Is Nothing Then
                    .Applicants = New List(Of QuickQuoteApplicant)
                    Dim a As New QuickQuoteApplicant
                    With a
                        .Name = quickQuote.Client.Name 'copied from Client
                        .Name.NameAddressSourceId = "28" 'Applicant
                        .Address = quickQuote.Client.Address 'copied from Client
                        .BusinessStartedDate = "8/1/2012"
                        .EducationTypeId = "4" '0=N/A; 1=High School; 2=Tech; 3=Vocational; 4=College Graduate (*8/6/2013 - not getting set in Diamond for some reason)
                        .Employer = "Test Employer"
                        .OccupationTypeId = "23" 'Professional
                        .PurchaseDate = "7/1/2012"
                        .RelationshipTypeId = "8" 'Policyholder
                        With .ResidenceInfo
                            .CurrentResidenceTypeId = "4" 'Apartment
                            .Owned = False

                            .ResidenceInfoDetails = New List(Of QuickQuoteResidenceInfoDetail)
                            With .ResidenceInfoDetails
                                Dim d1 As New QuickQuoteResidenceInfoDetail
                                With d1
                                    .Address = qqHelper.CloneObject(a.Address)
                                    .Address.StreetName &= " Prev1"
                                    .ResidenceInfoDetailTypeId = "1" 'Previous1
                                    .YearsAtPreviousAddress = "1"
                                End With
                                .Add(d1)
                                Dim d2 As New QuickQuoteResidenceInfoDetail
                                With d2
                                    .Address = qqHelper.CloneObject(a.Address)
                                    .Address.StreetName &= " Prev2"
                                    .ResidenceInfoDetailTypeId = "2" 'Previous2
                                    .YearsAtPreviousAddress = "3"
                                End With
                                .Add(d2)
                                Dim d3 As New QuickQuoteResidenceInfoDetail
                                With d3
                                    .Address = qqHelper.CloneObject(a.Address)
                                    .Address.StreetName &= " Prev3"
                                    .ResidenceInfoDetailTypeId = "3" 'Previous3
                                    .YearsAtPreviousAddress = "2"
                                End With
                                .Add(d3)
                            End With

                            .YearsAtCurrentAddress = "2"
                        End With
                        .SelfEmployedInfo = "self employment info"
                        .SpouseEmployer = "N/A"
                        .SpouseOccupationTypeId = "30" 'Unemployed
                        .StandardIndustrialClassification = "sic"
                        .USCitizenTypeId = "1" 'Yes
                        .YearsWithCurrentEmployer = "8"
                        .YearsWithPriorEmployer = "2"
                    End With
                    .Applicants.Add(a)
                ElseIf .Applicants.Count > 0 Then
                    With .Applicants(0) 'should already have name, address, and relationshiptypeid set from Copy method above
                        .BusinessStartedDate = "8/1/2012"
                        .EducationTypeId = "4" '0=N/A; 1=High School; 2=Tech; 3=Vocational; 4=College Graduate (*8/6/2013 - not getting set in Diamond for some reason)
                        .Employer = "Test Employer"
                        .OccupationTypeId = "23" 'Professional
                        .PurchaseDate = "7/1/2012"
                        With .ResidenceInfo
                            .CurrentResidenceTypeId = "4" 'Apartment
                            .Owned = False

                            .ResidenceInfoDetails = New List(Of QuickQuoteResidenceInfoDetail)
                            With .ResidenceInfoDetails
                                Dim d1 As New QuickQuoteResidenceInfoDetail
                                With d1
                                    .Address = qqHelper.CloneObject(quickQuote.Applicants(0).Address)
                                    .Address.StreetName &= " Prev1"
                                    .ResidenceInfoDetailTypeId = "1" 'Previous1
                                    .YearsAtPreviousAddress = "1"
                                End With
                                .Add(d1)
                                Dim d2 As New QuickQuoteResidenceInfoDetail
                                With d2
                                    .Address = qqHelper.CloneObject(quickQuote.Applicants(0).Address)
                                    .Address.StreetName &= " Prev2"
                                    .ResidenceInfoDetailTypeId = "2" 'Previous2
                                    .YearsAtPreviousAddress = "3"
                                End With
                                .Add(d2)
                                Dim d3 As New QuickQuoteResidenceInfoDetail
                                With d3
                                    .Address = qqHelper.CloneObject(quickQuote.Applicants(0).Address)
                                    .Address.StreetName &= " Prev3"
                                    .ResidenceInfoDetailTypeId = "3" 'Previous3
                                    .YearsAtPreviousAddress = "2"
                                End With
                                .Add(d3)
                            End With

                            .YearsAtCurrentAddress = "2"
                        End With
                        .SelfEmployedInfo = "self employment info"
                        .SpouseEmployer = "N/A"
                        .SpouseOccupationTypeId = "30" 'Unemployed
                        .StandardIndustrialClassification = "sic"
                        .USCitizenTypeId = "1" 'Yes
                        .YearsWithCurrentEmployer = "8"
                        .YearsWithPriorEmployer = "2"
                    End With
                End If
            End With
        End If

        With quickQuote 'added 12/5/2013 (needed 2 separate ones to encapsulate 1st time instantiation IF block and regular stuff)
            .LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal 'added 12/5/2013 to see if latest failure was due to changing HOM version_id from 11 to 45; changed back to 11 in xml and reset type here so it would use 11 again; EOD was running when latest attempt happened; didn't make a difference so changed back to 45
            .EffectiveDate = Date.Today.ToString

            'for tier override (should only be used for testing); removed 7/30/2014
            '.TierTypeId = "1" 'Uniform (probably not needed)
            '.UseTierOverride = True
            '.TierAdjustmentTypeId = "13" 'N/A=0; 1=13

            'policy level covs
            .PersonalLiabilityLimitId = "262" '100,000 (Location #1 - Invalid Coverage E limit selected for Homeowners form.)
            .MedicalPaymentsLimitid = "170" '1,000

            If .CanUseLocationNumForLocationReconciliation = False Then 'added so it will re-use the same locations... will need ELSE if anything needs to be modified/added/removed
                .Locations = New Generic.List(Of QuickQuoteLocation)
                Dim l1 As New QuickQuoteLocation
                With l1
                    .Description = "Loc 1"
                    .Name = qqHelper.CloneObject(quickQuote.Client.Name) '.Client.Name
                    .Name.NameAddressSourceId = "13" 'Location
                    .Address = qqHelper.CloneObject(quickQuote.Client.Address) '.Client.Address
                    .ProtectionClassId = "1" '1

                    .Acreage = "2"
                    .CondoRentedTypeId = "2" 'No
                    .ConstructionTypeId = "1" 'Frame
                    .DeductibleLimitId = "22" '500 (coverage)
                    .WindHailDeductibleLimitId = "24" '1000 (coverage)
                    .DayEmployees = False
                    .DaytimeOccupancy = True
                    .FamilyUnitsId = "1" '1
                    .FireDepartmentDistanceId = "2" '5 Miles or Less
                    .FireHydrantDistanceId = "4" 'Within 1,000 feet
                    .FormTypeId = "1" 'HO-2 - Homeowners Broad Form
                    .FoundationTypeId = "2" 'Closed
                    .LastCostEstimatorDate = "12/1/2012"
                    .MarketValue = "140000"
                    .NumberOfFamiliesId = "1" '1
                    .OccupancyCodeId = "1" 'Owner
                    .PrimaryResidence = True
                    .ProgramTypeId = "1" 'Homeowners
                    .NumberOfApartments = "0"
                    .NumberOfSolidFuelBurningUnits = "0"
                    .RebuildCost = "150000"
                    .Remarks = "test loc prop remarks"
                    .SquareFeet = "4500"
                    .StructureTypeId = "13" 'Conventionally Built
                    .YearBuilt = "1990"

                    'updates
                    .Updates.WindowsUpdateYear = "1991"
                    .Updates.ElectricUpdateYear = "1992"
                    .Updates.ElectricUpdateTypeId = "2" 'Complete
                    .Updates.ElectricCircuitBreaker = True
                    .Updates.CentralHeatUpdateYear = "1993"
                    .Updates.PlumbingUpdateYear = "1994"
                    .Updates.PlumbingUpdateTypeId = "2" 'Complete
                    .Updates.RoofUpdateYear = "1995"
                    .Updates.SupplementalHeatUpdateYear = "1996"

                    'coverages; LimitIncreased values should be the only ones that are needed
                    .A_Dwelling_Limit = "60000"
                    .A_Dwelling_LimitIncluded = "0"
                    .A_Dwelling_LimitIncreased = "60000"
                    .B_OtherStructures_Limit = "6500"
                    .B_OtherStructures_LimitIncluded = "6000"
                    .B_OtherStructures_LimitIncreased = "500"
                    .C_PersonalProperty_Limit = "42300"
                    .C_PersonalProperty_LimitIncluded = "42000"
                    .C_PersonalProperty_LimitIncreased = "300"
                    .D_LossOfUse_Limit = "0"
                    .D_LossOfUse_LimitIncluded = "0"
                    .D_LossOfUse_LimitIncreased = "0"

                    If .CanUseAdditionalInterestNumForAdditionalInterestReconciliation = False Then
                        .AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                        Dim ai As New QuickQuoteAdditionalInterest
                        With ai
                            With .Name
                                .CommercialName1 = "Test Finance Company"
                                .TypeId = "2"
                                .TaxNumber = "123456789"
                                .TaxTypeId = "2"
                            End With
                            With .Address
                                .HouseNum = "123"
                                .StreetName = "Finance Drive"
                                .City = "Indianapolis"
                                .StateId = "16" 'automatically defaulted anyway
                                .Zip = "46227" 'should automatically append -0000
                                .County = "Marion"
                            End With
                            .Emails = New List(Of QuickQuoteEmail)
                            Dim e As New QuickQuoteEmail
                            With e
                                .Address = "test@test.com"
                                .TypeId = "2"
                            End With
                            .Emails.Add(e)
                            .Phones = New List(Of QuickQuotePhone)
                            Dim p As New QuickQuotePhone
                            With p
                                .Number = "(317)111-2222"
                                .TypeId = "2"
                            End With
                            .Phones.Add(p)
                            .ATIMA = False
                            .GroupTypeId = "2" 'Finance Company
                            .Description = "desc"
                            .Other = "other"
                            .TypeId = "42" 'First Mortgagee
                        End With
                        .AdditionalInterests.Add(ai)
                    Else
                        'add logic here to change existing additional interests

                    End If

                    'credits and surcharges
                    .MultiPolicyDiscount = True
                    '.MatureHomeownerDiscount = True
                    '.FireSmokeAlarm_LocalAlarmSystem = True
                    .NewHomeDiscount = True
                    '.FireSmokeAlarm_CentralStationAlarmSystem = True
                    '.SelectMarketCredit = True
                    '.FireSmokeAlarm_SmokeAlarm = True
                    .BurglarAlarm_LocalAlarmSystem = True
                    .SprinklerSystem_AllExcept = True
                    '.BurglarAlarm_CentralStationAlarmSystem = True
                    '.SprinklerSystem_AllIncluding = True
                    .TrampolineSurcharge = True
                    '.WoodOrFuelBurningApplianceSurcharge = True

                    If .InlandMarines Is Nothing Then '8/7/2014 note: need prop for .CanUseInlandMarineNumForInlandMarineReconciliation
                        .InlandMarines = New List(Of QuickQuoteInlandMarine)
                        Dim im As New QuickQuoteInlandMarine
                        With im
                            .ArtistName = "BikeArtist"
                            .ConsentToRateCoverageEliminated = "cov eliminated"
                            .ConsentToRateCoverageInvolved = "cov involved"
                            '.InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Bicycles
                            'updated 12/5/2013 to use coverage code desc instead of caption
                            .InlandMarineType = QuickQuoteInlandMarine.QuickQuoteInlandMarineType.Inland_Marine_Bicycles
                            .DeductibleLimitId = 18 '100
                            .IncreasedLimit = "500.00"
                            .Description = "IM Bicycles Desc"
                            .MakeBrand = "BikeMake"
                            .Model = "BikeModel"
                            .RateInfoAmount = "3.00"
                            .RateInfoDescription = "rate info desc"
                            .RateInformationTypeId = "2" 'Consent to Rate
                            .SerialNumber = "SERIALNUM1"
                            .StatedAmount = True
                            .StorageLocation = "storage loc"
                            .Year = "2010"
                        End With
                        .InlandMarines.Add(im)
                    Else
                        'add logic here to change existing inland marines

                    End If

                    If .RvWatercrafts Is Nothing Then '8/7/2014 note: need prop for .CanUseRvWatercraftNumForRvWatercraftReconciliation
                        .RvWatercrafts = New List(Of QuickQuoteRvWatercraft)
                        Dim rv As New QuickQuoteRvWatercraft
                        With rv

                            .CostNew = "3300.00"
                            .PropertyDeductibleLimitId = "21" '250
                            .UninsuredMotoristBodilyInjuryLimitId = "7" '10,000
                            .HasLiability = True
                            .HasLiabilityOnly = False
                            .Description = "Desc"
                            .HorsepowerCC = "100"
                            .Length = "9"
                            .Manufacturer = "RvManufacturer"
                            .Model = "RvModel"
                            .Name = qqHelper.CloneObject(quickQuote.Client.Name)
                            .Name.NameAddressSourceId = ""
                            .Name.LastName &= "_rv"
                            .OwnerOtherThanInsured = True
                            .RatedSpeed = "18"

                            .RvWatercraftMotors = New List(Of QuickQuoteRvWatercraftMotor)
                            Dim m As New QuickQuoteRvWatercraftMotor
                            With m
                                .CostNew = "900.00"
                                .Manufacturer = "MotorManufacturer"
                                .Model = "MotorModel"
                                .MotorTypeId = "1" 'Inboard
                                .SerialNumber = "MotorSERIALNUM3"
                                .Year = "1997"
                            End With
                            .RvWatercraftMotors.Add(m)

                            .RvWatercraftTypeId = "6" 'Golf Cart
                            .SerialNumber = "RvSERIALNUM2"
                            .Year = "1997"
                        End With
                        .RvWatercrafts.Add(rv)
                    Else
                        'add logic here to change existing rv watercrafts

                    End If

                    If .Exclusions Is Nothing Then '8/7/2014 note: need prop for .CanUseExclusionNumForExclusionReconciliation
                        .Exclusions = New List(Of QuickQuoteExclusion)
                        With .Exclusions
                            Dim e1 As New QuickQuoteExclusion
                            With e1
                                .ExclusionTypeId = "1" 'Exclusion
                                .Description = "exc desc"
                            End With
                            .Add(e1)
                            Dim e2 As New QuickQuoteExclusion
                            With e2
                                .ExclusionTypeId = "3" 'Restriction
                                .Description = "rest desc"
                            End With
                            .Add(e2)
                            Dim e3 As New QuickQuoteExclusion
                            With e3
                                .ExclusionTypeId = "4" 'Comment
                                .Description = "comm desc"
                            End With
                            .Add(e3)
                        End With
                    Else
                        'add logic here to change existing exclusions

                    End If

                    If .SectionICoverages Is Nothing Then '8/7/2014 note: need prop for .CanUseSectionCoverageNumForSectionCoverageReconciliation
                        .SectionICoverages = New List(Of QuickQuoteSectionICoverage)
                        Dim sIc As New QuickQuoteSectionICoverage
                        With sIc
                            '.HOM_CoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment_HO_35
                            'updated 12/2/2013 to use coverage code desc instead of caption
                            .HOM_CoverageType = QuickQuoteSectionICoverage.HOM_SectionICoverageType.LossAssessment
                            .IncreasedLimitId = "221" '4,000
                            .Description = "Section I (Loss Assessment) desc"
                            .Address = qqHelper.CloneObject(quickQuote.Client.Address)
                            .Address.StreetName &= "_sectionI"
                            .EffectiveDate = "7/30/2013"
                            .ConstructionTypeId = "1" 'Frame
                            .DescribedLocation = True
                            .TheftExtension = False
                            '.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                        End With
                        .SectionICoverages.Add(sIc)
                    Else
                        'add logic here to change existing section I Coverages

                    End If

                    If .SectionIICoverages Is Nothing Then '8/7/2014 note: need prop for .CanUseSectionCoverageNumForSectionCoverageReconciliation
                        .SectionIICoverages = New List(Of QuickQuoteSectionIICoverage)
                        Dim sIIc As New QuickQuoteSectionIICoverage
                        With sIIc
                            '.HOM_CoverageType = QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Clerical_HO_71
                            'updated 12/5/2013 to use coverage code desc instead of caption
                            .HOM_CoverageType = QuickQuoteSectionIICoverage.HOM_SectionIICoverageType.BusinessPursuits_Clerical
                            .Description = "sectionII - bus pursuits - clerical (HO-71)"
                            .Name = qqHelper.CloneObject(quickQuote.Client.Name)
                            .Name.LastName &= "_sectionII"
                            .Name.NameAddressSourceId = "10022" 'Section Coverage
                            .Address = qqHelper.CloneObject(quickQuote.Client.Address)
                            .Address.StreetName &= "_sectionII"
                            .NumberOfPersonsReceivingCare = "4"
                            .NumberOfFamilies = "1"
                            .NumberOfFullTimeEmployees_180plus_days = "1"
                            .NumberOfPartTimeEmployees_41_to_180_days = "2"
                            .NumberOfPartTimeEmployees_40_or_less_days = "3"
                            .EstimatedNumberOfHead = "1"
                            .BusinessType = "BusType"
                            .InitialFarmPremises = True
                            .EventFrom = "8/1/2013"
                            .EventTo = "8/2/2013"
                            .BusinessName = "BusName"
                            '.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                        End With
                        .SectionIICoverages.Add(sIIc)
                    Else
                        'add logic here to change existing section II Coverages

                    End If

                    If .SectionIAndIICoverages Is Nothing Then '8/7/2014 note: need prop for .CanUseSectionCoverageNumForSectionCoverageReconciliation
                        .SectionIAndIICoverages = New List(Of QuickQuoteSectionIAndIICoverage)
                        Dim sIandIIc As New QuickQuoteSectionIAndIICoverage
                        With sIandIIc
                            '.MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures_HO_42
                            'updated 12/5/2013 to use coverage code desc instead of caption
                            .MainCoverageType = QuickQuoteSectionIAndIICoverage.SectionIAndIICoverageType.PermittedIncidentalOccupanciesResidencePremises_OtherStructures
                            .PropertyIncreasedLimit = "1000.00"
                            .Description = "Section I and II (Permitted Incidental Occupancies Residence) desc"
                            .Name = qqHelper.CloneObject(quickQuote.Client.Name)
                            .Name.LastName &= "_sectionIandII"
                            .Name.NameAddressSourceId = "10022" 'Section Coverage
                            .Address = qqHelper.CloneObject(quickQuote.Client.Address)
                            .Address.StreetName &= "_sectionIandII"
                            .NumberOfFamilies = "1"
                            '.AdditionalInterests = New List(Of QuickQuoteAdditionalInterest)
                        End With
                        .SectionIAndIICoverages.Add(sIandIIc)
                    Else
                        'add logic here to change existing section I and II Coverages

                    End If

                End With
                If l1.PolicyUnderwritings Is Nothing Then 'need prop for .CanUsePolicyUnderwritingNumForPolicyUnderwritingReconciliation
                    HOM_AddPolicyUnderwritingsToLocation(l1)
                Else
                    'add logic here to change existing UW Questions

                End If
                .Locations.Add(l1)
            Else
                'add logic here to change existing location(s)

            End If

        End With

        If saveOrRate <> Nothing AndAlso saveOrRate = QuickQuoteXML.QuickQuoteValidationType.Save Then
            qqxml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, quickQuote, qId, err)
            If err = "" Then
                'okay

            Else
                'error
            End If
        Else
            qqxml.RateQuoteAndSave(QuickQuoteXML.QuickQuoteSaveType.Quote, quickQuote, strQQ, ratedQQ, strRatedQQ, qId, err) 'debug method w/ byref params for rated QuickQuoteObject and xml strings for the request and response
            'qqxml.RateQuoteAndSave(QuickQuoteXML.QuickQuoteSaveType.Quote, quickQuote, qId, err) 'normal method
            If err = "" Then
                'okay
                Response.Redirect("DiamondQuoteSummary.aspx?QuoteId=" & qId)


            Else
                'error
            End If
        End If

    End Sub


    Private Sub HOM_AddPolicyUnderwritingsToLocation(ByRef l As QuickQuoteLocation) 'added 8/15/2013
        If l IsNot Nothing Then
            l.PolicyUnderwritings = New List(Of QuickQuotePolicyUnderwriting)
            With l.PolicyUnderwritings
                Dim pu1 As New QuickQuotePolicyUnderwriting
                With pu1
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9324" '1. Any farming or other business conducted on premises? (Including day/child care)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu1)
                Dim pu2 As New QuickQuotePolicyUnderwriting
                With pu2
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9299" '2. Any residence employees? (Number and type of full and part time employees)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu2)
                Dim pu3 As New QuickQuotePolicyUnderwriting
                With pu3
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9300" '3. Any flooding, brush, forest fire hazard, landslide, etc?
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu3)
                Dim pu4 As New QuickQuotePolicyUnderwriting
                With pu4
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9301" '4. Any other residence owned, occupied or rented?
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu4)
                Dim pu5 As New QuickQuotePolicyUnderwriting
                With pu5
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9302" '5. Any other insurance with this company? (List policy numbers)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu5)
                Dim pu6 As New QuickQuotePolicyUnderwriting
                With pu6
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9303" '6. Has insurance been transferred within agency?
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu6)
                Dim pu7 As New QuickQuotePolicyUnderwriting
                With pu7
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9304" '7. Any coverage declined, cancelled, or non-renewed during the last three (3) years.
                    .PolicyUnderwritingExtraAnswer = "" 'no spot for additional info in UI
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu7)
                Dim pu8 As New QuickQuotePolicyUnderwriting
                With pu8
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9305" '8. Has applicant had a foreclosure, repossession, bankruptcy, judgment or lien during the last five (5) years?
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu8)
                Dim pu9 As New QuickQuotePolicyUnderwriting
                With pu9
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9306" '9. Are there any animals or exotic pets kept on premises? (Note breed and bite history)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu9)
                Dim pu10 As New QuickQuotePolicyUnderwriting
                With pu10
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9307" '10. Distance to Tidal Water? (Miles or Feet)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu10)
                Dim pu11 As New QuickQuotePolicyUnderwriting
                With pu11
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9308" '11. Is property situated on more than five (5) acres? (If yes, describe land use)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu11)
                Dim pu12 As New QuickQuotePolicyUnderwriting
                With pu12
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9309" '12. Does applicant own any recreational vehicles (snow mobiles, dunebuggys, mini bikes, ATV's, etc)? (List year, type, make, model)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu12)
                Dim pu13 As New QuickQuotePolicyUnderwriting
                With pu13
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9310" '13. Is building retrofitted for earthquake? (if applicable)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu13)
                Dim pu14 As New QuickQuotePolicyUnderwriting
                With pu14
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9311" '14. During the last five (5) years, has any applicant been indicted for or convicted of any degree of the crime of fraud, arson, or any other arson related crime in connection with this or any other property?
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu14)
                Dim pu15 As New QuickQuotePolicyUnderwriting
                With pu15
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9312" '15. Is there a manager on the premises? (Renters and Condos only)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu15)
                Dim pu16 As New QuickQuotePolicyUnderwriting
                With pu16
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9313" '16. Is there a security attendant? (Renters and Condos only)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu16)
                Dim pu17 As New QuickQuotePolicyUnderwriting
                With pu17
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9314" '17. Is the building entrance locked? (Renters and Condos only)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu17)
                Dim pu18 As New QuickQuotePolicyUnderwriting
                With pu18
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9315" '18. Any uncorrected fire or building code violations?
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu18)
                Dim pu19 As New QuickQuotePolicyUnderwriting
                With pu19
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9316" '19. Is house for sale?
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu19)
                Dim pu20 As New QuickQuotePolicyUnderwriting
                With pu20
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9317" '20. Is property within 300 feet of a commercial or non-residential property?
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu20)
                Dim pu21 As New QuickQuotePolicyUnderwriting
                With pu21
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9318" '21. Is there a trampoline of the premises?
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu21)
                Dim pu22 As New QuickQuotePolicyUnderwriting
                With pu22
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9319" '22. Was the structure originally built for other than a private residence and then converted?
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu22)
                Dim pu23 As New QuickQuotePolicyUnderwriting
                With pu23
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9320" '23. Any lead paint hazard?
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu23)
                Dim pu24 As New QuickQuotePolicyUnderwriting
                With pu24
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9321" '24. If a fuel oil tank is on premises, has other insurance been obtained for the tank? (Give First Party and limit, and Third party and limit)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu24)
                Dim pu25 As New QuickQuotePolicyUnderwriting
                With pu25
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9322" '25. Is the building under construction or undergoing renovation or reconstruction? (Give estimated completion date and dollar value)
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu25)
                Dim pu26 As New QuickQuotePolicyUnderwriting
                With pu26
                    .PolicyUnderwritingAnswer = "-1" '-1 for No or 1 for Yes
                    .PolicyUnderwritingAnswerTypeId = "0" 'N/A
                    .PolicyUnderwritingCodeId = "9323" '26. If building is under construction, is the applicant the general contractor?
                    .PolicyUnderwritingExtraAnswer = "" 'additional info
                    .PolicyUnderwritingExtraAnswerTypeId = "0" 'not in ExtraAnswerType table (would've thought it would be 1 - Text)
                    .PolicyUnderwritingLevelId = "3" 'Location
                    .PolicyUnderwritingTabId = "1" 'UW # 1
                End With
                .Add(pu26)
            End With
        End If
    End Sub

    ''' <summary>
    ''' Create MVR Report
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnPA_MVR_Click(sender As Object, e As EventArgs) Handles btnPA_MVR.Click
        Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML()
        Dim qo As New QuickQuote.CommonObjects.QuickQuoteObject()
        Dim drivernum As String = ""
        Dim err As String = ""
        Dim ReportFile As Object = Nothing
        Dim quoteid As String = ""

        Try
            HideMsg()
            txtReportData.Text = ""

            quoteid = getQuoteID()
            If quoteid Is Nothing Then Exit Sub

            If Not IsCorrectQuoteType("PPA") Then Throw New Exception("Incorrect quote type for the selected report")

            If txtDriverNum.Text.Trim() = "" Then Throw New Exception("Driver Number is required")
            If Not IsNumeric(txtDriverNum.Text) Then Throw New Exception("Driver Number must be Numeric")
            drivernum = txtDriverNum.Text

            qqxml.GetQuoteForSaveType(quoteid, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, qo, err)
            If err <> "" Then Throw New Exception(err)

            err = ""
            ReportFile = IFM.VR.Common.ThirdPartyReporting.PERSONAL_AUTO_GetMVRReport(qo, drivernum, err)

            ' Generate the report and display it
            '...and start a process to view the pdf file
            If ReportFile IsNot Nothing Then
                If TypeOf ReportFile Is String Then
                    Process.Start(ReportFile)
                    DisplayTxtMessage("Report file created successfully as " & ReportFile)
                ElseIf TypeOf ReportFile Is System.IO.MemoryStream Then
                    DisplayTxtMessage("Report Memory Stream Created Successfully")
                End If
            Else
                DisplayTxtMessage("Report is NOTHING - Selected Quote/Driver may not have an available MVR report")
            End If

            Exit Sub
        Catch ex As Exception
            DisplayMessage(ex.Message)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Create CLUE Report
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btnPA_CLUE_Click(sender As Object, e As EventArgs) Handles btnPA_CLUE.Click
        Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML()
        Dim qo As New QuickQuote.CommonObjects.QuickQuoteObject()
        Dim err As String = ""
        Dim ReportFile As New Object
        Dim quoteid As String = ""

        Try
            HideMsg()
            txtReportData.Text = ""

            quoteid = getQuoteID()
            If quoteid Is Nothing Then Exit Sub

            If Not IsCorrectQuoteType("PPA") Then Throw New Exception("Incorrect quote type for the selected report")

            qqxml.GetQuoteForSaveType(quoteid, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, qo, err)
            If err <> "" Then Throw New Exception(err)

            err = ""

            ReportFile = IFM.VR.Common.ThirdPartyReporting.PERSONAL_AUTO_GetCLUEReport(qo, err).ToString()

            If ReportFile IsNot Nothing AndAlso ReportFile <> "" Then
                Dim txt As String = ""
                txt = txt & "Report file created as " & ReportFile.ToString()
                Process.Start(ReportFile.ToString())
                txtReportData.Text = txt
            Else
                txtReportData.Text = "No file(s) generated"
            End If

            Exit Sub
        Catch ex As Exception
            DisplayMessage(ex.Message)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Create AUTO CREDIT Report
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btn_Credit_PPA_Click(sender As Object, e As EventArgs) Handles btn_Credit_PH1.Click, btn_Credit_PH2.Click, btn_Credit_Driver.Click
        Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML()
        Dim qo As New QuickQuote.CommonObjects.QuickQuoteObject()
        Dim err As String = ""
        Dim ReportData As Object = Nothing
        Dim quoteid As String = ""
        Dim WhoSentMe As Button = sender
        Dim cr As String = ""
        Dim ReportFile As Object = ""

        Try
            HideMsg()
            txtReportData.Text = ""

            quoteid = getQuoteID()
            If quoteid Is Nothing Then Exit Sub

            If Not IsCorrectQuoteType("PPA") Then Throw New Exception("Incorrect quote type for the selected report")

            qqxml.GetQuoteForSaveType(quoteid, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, qo, err)
            If err <> "" Then Throw New Exception(err)

            err = ""

            Select Case WhoSentMe.ID.ToUpper()
                Case "BTN_CREDIT_PH1"
                    ReportFile = IFM.VR.Common.ThirdPartyReporting.GetCreditReport(Common.CreditReportSubject.PolicyHolder1, qo, err)
                    cr = "PH1"
                    Exit Select
                Case "BTN_CREDIT_PH2"
                    ReportFile = IFM.VR.Common.ThirdPartyReporting.GetCreditReport(Common.CreditReportSubject.PolicyHolder2, qo, err)
                    cr = "PH2"
                    Exit Select
                Case "BTN_CREDIT_DRIVER"
                    If txtDriverNum.Text.Trim() = "" Then Throw New Exception("Driver number is required")
                    If Not IsNumeric(txtDriverNum.Text) Then Throw New Exception("Driver number must be numeric")
                    cr = "Driver " & txtDriverNum.Text
                    ReportFile = IFM.VR.Common.ThirdPartyReporting.GetCreditReport(Common.CreditReportSubject.PolicyHolder1, qo, err)
                    Exit Select
                Case Else
                    Throw New Exception("Unknown sender: " & WhoSentMe.ID)
            End Select

            If err <> "" Then Throw New Exception(err)

            ' Generate the report and display it
            '...and start a process to view the pdf file
            If ReportFile IsNot Nothing Then
                If TypeOf ReportFile Is String Then
                    Process.Start(ReportFile)
                    DisplayTxtMessage("Report file created successfully as " & ReportFile)
                ElseIf TypeOf ReportFile Is System.IO.MemoryStream Then
                    DisplayTxtMessage("Report Memory Stream Created Successfully")
                End If
            Else
                DisplayTxtMessage("Report is NOTHING - Selected Quote may not have an available CREDIT report")
            End If

            Exit Sub
        Catch ex As Exception
            DisplayMessage(ex.Message)
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Create HOME CREDIT Report
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub btn_Credit_HOM_Click(sender As Object, e As EventArgs) Handles btn_Credit_Applicant.Click
        Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML()
        Dim qo As New QuickQuote.CommonObjects.QuickQuoteObject()
        Dim err As String = ""
        Dim ReportData As Object = Nothing
        Dim quoteid As String = ""
        Dim cr As String = ""
        Dim ReportFile As Object = ""

        Try
            HideMsg()
            txtReportData.Text = ""

            quoteid = getQuoteID()
            If quoteid Is Nothing Then Exit Sub

            If Not IsCorrectQuoteType("HOM") Then Throw New Exception("Incorrect quote type for the selected report")

            qqxml.GetQuoteForSaveType(quoteid, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, qo, err)
            If err <> "" Then Throw New Exception(err)

            err = ""

            ReportFile = IFM.VR.Common.ThirdPartyReporting.GetCreditReport(Common.CreditReportSubject.Applicant, qo, err)

            ' Generate the report and display it
            '...and start a process to view the pdf file
            If ReportFile IsNot Nothing Then
                If TypeOf ReportFile Is String Then
                    Process.Start(ReportFile)
                    DisplayTxtMessage("Report file created successfully as " & ReportFile)
                ElseIf TypeOf ReportFile Is System.IO.MemoryStream Then
                    DisplayTxtMessage("Report Memory Stream Created Successfully")
                End If
            Else
                DisplayTxtMessage("Report is NOTHING - Selected Quote may not have an available CREDIT report")
            End If

            Exit Sub
        Catch ex As Exception
            DisplayMessage(ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub rbList_CheckedChanged(sender As Object, e As EventArgs) Handles rbList.CheckedChanged
        Try
            If rbList.Checked Then
                ddlQuoteID.Enabled = True
                txtQuoteID.Enabled = False
                txtQuoteID.Text = ""
                ddlQuoteID.SelectedIndex = 1
                ddlQuoteID.Focus()
            End If
        Catch ex As Exception
            DisplayMessage(ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub rbText_CheckedChanged(sender As Object, e As EventArgs) Handles rbText.CheckedChanged
        Try
            If rbText.Checked Then
                txtQuoteID.Enabled = True
                ddlQuoteID.Enabled = False
                ddlQuoteID.SelectedIndex = 0
                txtQuoteID.Focus()
            End If
        Catch ex As Exception
            DisplayMessage(ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnRate_Click(sender As Object, e As EventArgs) Handles btnRate.Click
        Try
            HOM_save_test_new_simple("3360")
        Catch ex As Exception
            DisplayMessage(ex.Message)
            Exit Sub
        End Try
    End Sub

    Private Sub btnHOM_CLUE_Click(sender As Object, e As EventArgs) Handles btnHOM_CLUE.Click
        Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML()
        Dim qo As New QuickQuote.CommonObjects.QuickQuoteObject()
        Dim err As String = ""
        Dim ReportFile As New Object
        Dim quoteid As String = ""

        Try
            HideMsg()
            txtReportData.Text = ""

            quoteid = getQuoteID()
            If quoteid Is Nothing Then Exit Sub

            If Not IsCorrectQuoteType("HOM") Then Throw New Exception("Incorrect quote type for the selected report")

            qqxml.GetQuoteForSaveType(quoteid, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, qo, err)
            If err <> "" Then Throw New Exception(err)

            err = ""

            ReportFile = IFM.VR.Common.ThirdPartyReporting.PERSONAL_HOME_GetCLUEReport(qo, err).ToString()

            If ReportFile IsNot Nothing AndAlso ReportFile <> "" Then
                Dim txt As String = ""
                txt = txt & "Report file created as " & ReportFile.ToString()
                Process.Start(ReportFile.ToString())
                txtReportData.Text = txt
            Else
                txtReportData.Text = "No file(s) generated"
            End If

            Exit Sub
        Catch ex As Exception
            DisplayMessage(ex.Message)
            Exit Sub
        End Try

    End Sub

    Private Sub btnPPC_Click(sender As Object, e As EventArgs) Handles btnPPC.Click
        Dim qqxml As New QuickQuote.CommonMethods.QuickQuoteXML()
        Dim qo As New QuickQuote.CommonObjects.QuickQuoteObject()
        Dim err As String = ""
        'Dim ReportFile As New Object
        Dim quoteid As String = ""
        Dim ReportBytes() As Byte = Nothing

        Try
            HideMsg()
            txtReportData.Text = ""

            quoteid = getQuoteID()
            If quoteid Is Nothing Then Exit Sub

            If Not IsCorrectQuoteType("PPC") Then Throw New Exception("Incorrect quote type for the selected report")

            qqxml.GetQuoteForSaveType(quoteid, QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteSaveType.Quote, qo, err)
            If err <> "" Then Throw New Exception(err)

            err = ""

            'ReportFile = IFM.VR.Common.ThirdPartyReporting.PERSONAL_HOME_GetCLUEReport(qo, err).ToString()
            ReportBytes = IFM.VR.Common.ThirdPartyReporting.PERSONAL_HOM_DFR_GetPCCReport(qo, err, True)

            If ReportBytes IsNot Nothing AndAlso ReportBytes.Length > 0 Then
                Response.Clear()
                Response.ContentType = "application/pdf"
                Response.AddHeader("Content-Disposition", "inline;filename=testPDF.pdf")
                Response.BinaryWrite(ReportBytes)
                Response.Flush()
                Response.End()
            Else
                txtReportData.Text = "No file(s) generated"
            End If

            Exit Sub
        Catch ex As Exception
            DisplayMessage(ex.Message)
            Exit Sub
        End Try
    End Sub
End Class