Imports Diamond.Common.Objects.Claims.ClaimControl.ClaimControlProperty
Imports Diamond.Common.Objects.Claims.LossNotice

Public Class DiamondClaims
    Implements IDisposable

    Private _Errors As New List(Of String)

    Public Structure FNOLResponseData_Struct
        Dim AssignedSuccessfully As Boolean
        Dim DiamondAdjuster_Id As String
        Dim AdjusterName As String
        Dim DateAssigned As String
        Dim AssignedBy As String
        Dim FNOLAdjusterID As String
        Dim CAT As Boolean
        Dim FNOL_ID As Integer
        Dim Group_Id As String
        Dim ErrMsg As String
    End Structure

    Public Sub New()

    End Sub

    Private Function StringFieldHasNonZeroNumericValue(ByVal str As String) As Boolean
        If str IsNot Nothing AndAlso str.Trim <> String.Empty AndAlso IsNumeric(str) AndAlso CDec(str) > 0 Then Return True
    End Function

    Public Function honorDraft(ByVal checkID As Integer, ByVal clearDate As Date, ByVal clearedBankDate As Date) As Boolean
        Try
            Using sqlEx As New SQLexecuteObject(AppSettings("connDiamond"))
                sqlEx.queryOrStoredProc = "assp_Checks_ClearCheck"
                Dim arParams As New ArrayList
                With arParams
                    .Add(New SqlParameter("check_id", checkID))
                    .Add(New SqlParameter("clear_date", clearDate))
                    .Add(New SqlParameter("checktype_id", DCE.Checks.CheckType.ClaimDraft))
                    .Add(New SqlParameter("clearedbank_date", clearedBankDate))
                End With
                sqlEx.inputParameters = arParams
                sqlEx.ExecuteStatement()

                If sqlEx.hasError = False Then
                    Return True
                Else : Throw New DiaException(sqlEx.errorMsg, "SQLExecuteObject :: ExecuteStatement()")
                End If
            End Using
        Catch ex As Exception
            Throw New DiaException(ex.ToString, "Honor Draft")
        End Try
    End Function

    Public Function getUser() As DCO.User
        Try
            Dim request As New DCS.Messages.LookupService.GetUser.Request
            Dim response As New DCS.Messages.LookupService.GetUser.Response

            request.RequestData.UsersId = 76

            Using proxy As New Proxies.LookupServiceProxy
                response = proxy.GetUser(request)
            End Using

            If response IsNot Nothing AndAlso response.ResponseData.User IsNot Nothing Then
                'Return CType(response.ResponseData.User, DCO.User)
                Dim user As New DCO.User
                With response.ResponseData.User
                    user.Active = .Active
                    user.FullName = .DisplayName
                    user.LoginDomain = .LoginDomain
                    user.LoginName = .LoginName
                    user.UsercategoryId = .UserCategoryId
                    user.UsersId = .UsersId
                End With
                'user.Fill(response.ResponseData.User)
                Return user
            End If
        Catch ex As Exception
            Return Nothing
        End Try

        Return Nothing
    End Function

    Private Function Get_OmitInsuredClaimantsValue(ByVal FNOLType As Enums.FNOL_Type) As Boolean
        ' Added 2/22/22 MGB 18670
        Dim lines As String() = Nothing
        Dim OmitInsuredClaimants = DiamondClaimsFNOL_OmitInsuredClaimants()

        If OmitInsuredClaimants = False Then
            If AppSettings("DiamondClaimsFNOL_OmitInsuredClaimantsLossTypesOverride") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(AppSettings("DiamondClaimsFNOL_OmitInsuredClaimantsLossTypesOverride").ToString) = False Then
                ' Read the business lines that the claimant omission logic applies to from the config key
                lines = AppSettings("DiamondClaimsFNOL_OmitInsuredClaimantsLossTypesOverride").ToString.Split(",")

                ' Determine whether or not the Claimant Omission logic should apply and set the flag accordingly.
                If lines IsNot Nothing AndAlso lines.Count > 0 Then
                    Select Case FNOLType
                        Case Enums.FNOL_LOB_Enum.Auto
                            If lines.Contains("AUTO") Then Return True
                            Exit Select
                        Case Enums.FNOL_LOB_Enum.Liability
                            If lines.Contains("LIABILITY") Then Return True
                            Exit Select
                        Case Enums.FNOL_LOB_Enum.Propertty
                            If lines.Contains("PROPERTY") Then Return True
                            Exit Select
                    End Select
                End If
            End If
        Else
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' FNOL Override - Called by TstationTransactions (FNOL)
    ''' ANY CHANGES IN THE FNOL SUB NEED TO BE REFLECTED HERE!!
    ''' Added for FNOL Rewrite
    ''' MGB 6/30/15
    ''' </summary>
    ''' <param name="fnol"></param>
    ''' <param name="FNOLType"></param>
    ''' <remarks></remarks>
    Public Sub FNOL(ByVal fnol As DiamondClaims_FNOL, ByVal FNOLType As Enums.FNOL_LOB_Enum)
        Errors.Clear()

        Dim request As New DCS.Messages.ClaimsService.SubmitLossNotice.Request
        Dim response As New DCS.Messages.ClaimsService.SubmitLossNotice.Response
        Try
            Dim systemData As DCSDM.SystemData = DUSDM.SystemDataManager.SystemData
            Dim submitData As DCSDM.SubmitData = DUSDM.SubmitDataManager.SubmitData
            Dim versionData As DCSDM.VersionData = DUSDM.VersionDataManager.VersionData(1, 16, 1, DUU.SystemDate.GetSystemDate, DUU.SystemDate.GetSystemDate)

            Dim fileDir As String = ""
            Dim xmlFileAddInfo As String = ""
            Dim createXmls As Boolean = CreateDiamondXmlsForTesting(fileDir:=fileDir)
            If createXmls = True AndAlso fnol IsNot Nothing AndAlso fnol.PolicyID > 0 Then
                xmlFileAddInfo = "_" & fnol.PolicyID.ToString & "_" & fnol.PolicyImage.ToString
            End If

            With request.RequestData
                .Attempt = fnol.SaveAttempt
                .ClaimLnStatusTypeId = fnol.StatusType '*
                '.IgnorePersonnel = True '*
                .User = New DCO.User
                With .User
                    .UsersId = fnol.UserID '*
                End With
                .LossNoticeData = New DCO.Claims.LossNotice.LossNoticeData
                .IgnorePersonnel = False
                With .LossNoticeData
                    If (FNOLType = Enums.FNOL_LOB_Enum.Auto OrElse FNOLType = Enums.FNOL_LOB_Enum.CommercialAuto) AndAlso fnol.Vehicles.Count > 0 Then
                        If .Vehicles Is Nothing Then
                            .Vehicles = New DCO.InsCollection(Of Diamond.Common.Objects.Claims.LossNotice.LnVehicle)
                        End If
                        For i As Integer = 0 To fnol.Vehicles.Count - 1
                            .Vehicles.Add(New LnVehicle)

                            With .Vehicles(i) '5/15/2013 - will need to update to use i instead of 0 if we ever plan to set more than 1 vehicle; done 3/28/2024 just in case

                                .ClaimLnVehicleNum = New DCO.IdValue(i + 1)
                                .DamageDescription = fnol.Vehicles(i).DamageDescription
                                .EstimatedAmount = fnol.Vehicles(i).EstimatedAmountOfDamage
                                .License = fnol.Vehicles(i).LicensePlate
                                .LicenseStateId = fnol.Vehicles(i).LicenseState
                                .Make = fnol.Vehicles(i).Make
                                .Model = fnol.Vehicles(i).Model
                                .VIN = fnol.Vehicles(i).VIN
                                .Year = fnol.Vehicles(i).Year
                                .VehicleDrivable = fnol.Vehicles(i).Drivable
                                .AirbagsDeployedTypeId = fnol.Vehicles(i).AirbagsDeployedTypeId
                                .DrivableId = fnol.Vehicles(i).DrivableId
                                If DiamondClaimsFNOLCCCClaimsEnabled() = True Then
                                    .CCCEstimateQualification = New Diamond.Common.Objects.ThirdParty.CCC.CCCEstimateQualification() With {
                                        .Question1YesNoId = fnol.Vehicles(i).CCCEstimateQualificationId,
                                        .Phone = If(IsNumeric(fnol.Vehicles(i).CCCphone), String.Format("{0:(###)###-####}", Long.Parse(fnol.Vehicles(i).CCCphone)), fnol.Vehicles(i).CCCphone),
                                        .SendQuickEstimate = (fnol.Vehicles(i).CCCEstimateQualificationId = 1),
                                        .LanguageTypeId = 1
                                    }
                                End If
                                AddAddress(fnol.Vehicles(i).LossAddress, .LocationOfAccident.Address)
                                '.LocationOfAccident.Address = New Diamond.Common.Objects.Address() With {
                                '        .StreetName = fnol.Vehicles(i).LossAddress.StreetName,
                                '        .City = fnol.Vehicles(i).LossAddress.City,
                                '        .StateId = Convert.ToInt32(fnol.Vehicles(i).LossAddress.StateId),
                                '        .Zip = fnol.Vehicles(i).LossAddress.ZipCode,
                                '        .HouseNumber = fnol.Vehicles(i).LossAddress.HouseNumber,
                                '        .County = fnol.Vehicles(i).LossAddress.County,
                                '        .Other = fnol.Vehicles(i).LossAddress.AddressOther,
                                '        .POBox = fnol.Vehicles(i).LossAddress.PoBox,
                                '        .ApartmentNumber = fnol.Vehicles(i).LossAddress.ApartmentNumber
                                '    }

                                '.ClaimControlVehicleOwner.Name.FirstName = "Owner"
                                '.ClaimControlVehicleOwner.Name.MiddleName = "R"
                                '.ClaimControlVehicleOwner.Name.LastName = "ownerlast"
                                '.ClaimControlVehicleOwner.Name.FirstName = fnol.Vehicles(i).LossVehicleOwnerFirstName
                                '.ClaimControlVehicleOwner.Name.MiddleName = fnol.Vehicles(i).LossVehicleOwnerMiddleName
                                '.ClaimControlVehicleOwner.Name.LastName = fnol.Vehicles(i).LossVehicleOwnerLastName
                                'updated 8/10/2017
                                If fnol.Vehicles(i).LossVehicleOwnerName IsNot Nothing AndAlso HasNameInfo(fnol.Vehicles(i).LossVehicleOwnerName) = True Then
                                    AddName(fnol.Vehicles(i).LossVehicleOwnerName, .LnVehicleOwner.Name)
                                Else
                                    .LnVehicleOwner.Name.FirstName = fnol.Vehicles(i).LossVehicleOwnerFirstName
                                    .LnVehicleOwner.Name.MiddleName = fnol.Vehicles(i).LossVehicleOwnerMiddleName
                                    .LnVehicleOwner.Name.LastName = fnol.Vehicles(i).LossVehicleOwnerLastName
                                    .LnVehicleOwner.Name.TypeId = 1
                                End If
                                AddAddress(fnol.Vehicles(i).LossAddress, .LnVehicleOwner.Address)

                                '.ClaimControlVehicleOperator.Name.FirstName = "R"
                                '.ClaimControlVehicleOperator.Name.MiddleName = "mid"
                                '.ClaimControlVehicleOperator.Name.LastName = "operatorlast"
                                '.ClaimControlVehicleOperator.Name.FirstName = fnol.Vehicles(i).LossVehicleOperatorFirstName
                                '.ClaimControlVehicleOperator.Name.MiddleName = fnol.Vehicles(i).LossVehicleOperatorMiddleName
                                '.ClaimControlVehicleOperator.Name.LastName = fnol.Vehicles(i).LossVehicleOperatorLastName
                                'updated 8/10/2017
                                If fnol.Vehicles(i).LossVehicleOperatorName IsNot Nothing AndAlso HasNameInfo(fnol.Vehicles(i).LossVehicleOperatorName) = True Then
                                    AddName(fnol.Vehicles(i).LossVehicleOperatorName, .LnVehicleOperator.Name)
                                Else
                                    .LnVehicleOperator.Name.FirstName = fnol.Vehicles(i).LossVehicleOperatorFirstName
                                    .LnVehicleOperator.Name.MiddleName = fnol.Vehicles(i).LossVehicleOperatorMiddleName
                                    .LnVehicleOperator.Name.LastName = fnol.Vehicles(i).LossVehicleOperatorLastName
                                    .LnVehicleOperator.Name.TypeId = 1
                                End If
                                AddAddress(fnol.Vehicles(i).LossAddress, .LnVehicleOperator.Address)


                            End With
                        Next
                    End If
                    '.Personnel.ClaimOfficeId = 1
                    '.Personnel.InsideAdjusterId = 134
                    '.Personnel.AdministratorId = 135
                    .Personnel.ClaimOfficeId = fnol.claimOfficeID
                    .Personnel.InsideAdjusterId = fnol.InsideAdjusterId
                    .Personnel.AdministratorId = fnol.AdministratorId

                    'vehicle owner
                    'appears under Reported/Insured, insured 1
                    'commented ClaimInsured1 and ClaimInsured2 code 6/29/2016 for 531; now being done at bottom w/ Parties logic
                    '.ClaimInsured1.Name.FirstName = fnol.insuredFirstName '4/26/2013 - will now get overwritten by what comes thru new Insured object
                    '.ClaimInsured1.Name.LastName = fnol.insuredLastName
                    ''updated 4/19/2013 to include insured address; will probably use what comes thru on new Insured object instead
                    ''If fnol.PolicyID <> Nothing AndAlso fnol.PolicyID > 0 AndAlso fnol.PolicyImage <> Nothing AndAlso fnol.PolicyImage > 0 AndAlso AppSettings("connDiamond") IsNot Nothing AndAlso AppSettings("connDiamond").ToString <> "" Then
                    ''    Using Sql As New SQLselectObject(AppSettings("connDiamond"))
                    ''        Sql.queryOrStoredProc = "SELECT A.* FROM PolicyImageAddressLink as PIAL with (nolock) inner join Address as A with (nolock) on A.address_id = PIAL.address_id WHERE PIAL.nameaddresssource_id = 3 and PIAL.policy_id = " & fnol.PolicyID & " and PIAL.policyimage_num = " & fnol.PolicyImage
                    ''        Dim dr As SqlDataReader = Sql.GetDataReader
                    ''        If dr IsNot Nothing AndAlso dr.HasRows = True Then
                    ''            dr.Read()
                    ''            .ClaimInsured1.Address.HouseNumber = dr.Item("house_num").ToString.Trim
                    ''            .ClaimInsured1.Address.StreetName = dr.Item("street_name").ToString.Trim
                    ''            .ClaimInsured1.Address.City = dr.Item("city").ToString.Trim
                    ''            .ClaimInsured1.Address.StateId = dr.Item("state_id").ToString.Trim
                    ''            .ClaimInsured1.Address.Zip = dr.Item("zip").ToString.Trim
                    ''            .ClaimInsured1.Address.County = dr.Item("county").ToString.Trim
                    ''            .ClaimInsured1.Address.ApartmentNumber = dr.Item("apt_num").ToString.Trim
                    ''            .ClaimInsured1.Address.Other = dr.Item("other").ToString.Trim
                    ''            .ClaimInsured1.Address.POBox = dr.Item("pobox").ToString.Trim
                    ''        End If
                    ''    End Using
                    ''End If
                    ''added 4/26/2013
                    'If fnol.Insured IsNot Nothing Then
                    '    With .ClaimInsured1
                    '        'AddPersonName(fnol.Insured.Name, .Name)
                    '        'AddPersonAddress(fnol.Insured.Address, .Address)
                    '        'AddPersonPhones(fnol.Insured.ContactInfo, .Phones)
                    '        'AddPersonEmails(fnol.Insured.ContactInfo, .Emails)
                    '        'updated 4/29/2013 to set all thru one Sub
                    '        AddPerson(fnol.Insured, .Name, .Address, .Phones, .Emails)
                    '    End With
                    'End If
                    ''added 4/29/2013
                    'If fnol.SecondInsured IsNot Nothing Then
                    '    With .ClaimInsured2
                    '        'AddPersonName(fnol.SecondInsured.Name, .Name)
                    '        'AddPersonAddress(fnol.SecondInsured.Address, .Address)
                    '        'AddPersonPhones(fnol.SecondInsured.ContactInfo, .Phones)
                    '        'AddPersonEmails(fnol.SecondInsured.ContactInfo, .Emails)
                    '        'updated 4/29/2013 to set all thru one Sub
                    '        AddPerson(fnol.SecondInsured, .Name, .Address, .Phones, .Emails)
                    '    End With
                    'End If

                    '.Witnesses '4/23/2013 Diamond.Common.Objects.Claims.LossNotice.Witness (testing)
                    'Dim w As New Diamond.Common.Objects.Claims.LossNotice.Witness
                    'w.Name.FirstName = "Wit1First"
                    'w.Name.LastName = "Wit1Last"
                    'w.Address.HouseNumber = "123"
                    'w.Address.StreetName = "Wit1 St"
                    'w.Address.City = "Indy"
                    'w.Address.StateId = "16"
                    'w.Address.Zip = "46227-0000"
                    'w.Remarks = "witness 1 remarks"
                    ''w.RelationshipId = ""
                    '.Witnesses.Add(w)
                    'Dim w2 As New Diamond.Common.Objects.Claims.LossNotice.Witness
                    'w2.Name.FirstName = "Wit1First"
                    'w2.Name.LastName = "Wit1Last"
                    'w2.Address.HouseNumber = "123"
                    'w2.Address.StreetName = "Wit1 St"
                    'w2.Address.City = "Indy"
                    'w2.Address.StateId = "16"
                    'w2.Address.Zip = "46227-0000"
                    'w2.Remarks = "witness 2 remarks"
                    ''w2.RelationshipId = ""
                    '.Witnesses.Add(w2)
                    'added actual logic 4/26/2013
                    If fnol.Witnesses IsNot Nothing AndAlso fnol.Witnesses.Count > 0 Then
                        If .Witnesses Is Nothing Then '6/29/2016: added logic to instantiate if needed
                            .Witnesses = New DCO.InsCollection(Of DCO.Claims.LossNotice.Witness)
                        End If
                        For Each w As DiamondClaims_FNOL_Witness In fnol.Witnesses
                            Dim diaWit As New DCO.Claims.LossNotice.Witness
                            With diaWit
                                'updated 4/29/2013 to set all thru one Sub
                                AddPerson(w, .Name, .Address, .Phones, .Emails)

                                'AddPersonName(w.Name, .Name)
                                'If w.Firstname <> "" OrElse w.Lastname <> "" Then
                                '    With .Name
                                '        .FirstName = w.Firstname
                                '        .MiddleName = w.Middlename
                                '        .LastName = w.Lastname
                                '    End With
                                'End If
                                'AddPersonAddress(w.Address, .Address)
                                'If w.Housenumber <> "" OrElse w.Streetname <> "" OrElse w.City <> "" OrElse (w.StateId <> "" AndAlso IsNumeric(w.StateId) = True) OrElse w.Zipcode <> "" Then
                                '    With .Address
                                '        .HouseNumber = w.Housenumber
                                '        .StreetName = w.Streetname
                                '        .City = w.City
                                '        If w.StateId <> "" AndAlso IsNumeric(w.StateId) = True Then
                                '            .StateId = w.StateId
                                '        End If
                                '        .Zip = w.Zipcode
                                '    End With
                                'End If
                                .Remarks = w.Remarks
                                If w.RelationshipId <> "" AndAlso IsNumeric(w.RelationshipId) = True Then
                                    .RelationshipId = w.RelationshipId 'added 4/26/2013
                                End If
                                'AddPersonPhones(w.ContactInfo, .Phones)
                                'If w.Homephone <> "" Then
                                '    Dim p As New DCO.Phone
                                '    With p
                                '        '.TypeId = 1 (home)
                                '        .TypeId = Diamond.Common.Enums.PhoneType.Home
                                '        .Number = w.Homephone
                                '    End With
                                '    .Phones.Add(p)
                                'End If
                                'If w.Businessphone <> "" Then
                                '    Dim p As New DCO.Phone
                                '    With p
                                '        '.TypeId = 2 (business)
                                '        .TypeId = Diamond.Common.Enums.PhoneType.Business
                                '        .Number = w.Businessphone
                                '    End With
                                '    .Phones.Add(p)
                                'End If
                                'If w.Cellphone <> "" Then
                                '    Dim p As New DCO.Phone
                                '    With p
                                '        '.TypeId = 4 (cell)
                                '        .TypeId = Diamond.Common.Enums.PhoneType.Cellular
                                '        .Number = w.Cellphone
                                '    End With
                                '    .Phones.Add(p)
                                'End If
                                'AddPersonEmails(w.ContactInfo, .Emails)
                                'If w.Email <> "" Then
                                '    Dim e As New DCO.Email
                                '    With e
                                '        .TypeId = Diamond.Common.Enums.EMailType.Other
                                '        .Address = w.Email
                                '    End With
                                '    .Emails.Add(e)
                                'End If
                            End With
                            .Witnesses.Add(diaWit)
                        Next
                    End If

                    .ReportedBy.ClaimReportedByMethodId = 5
                    .ReportedBy.ClaimReportedById = 1
                    '.ReportedBy.Name.LastName = "" '4/23/2013 - can use for reported by name
                    'added 4/26/2013
                    If fnol.Reporter IsNot Nothing Then
                        With .ReportedBy
                            'AddPersonName(fnol.Reporter.Name, .Name)
                            'AddPersonAddress(fnol.Reporter.Address, .Address)
                            'AddPersonPhones(fnol.Reporter.ContactInfo, .Phones)
                            'AddPersonEmails(fnol.Reporter.ContactInfo, .Emails)
                            'updated 4/29/2013 to set all thru one Sub
                            AddPerson(fnol.Reporter, .Name, .Address, .Phones, .Emails)
                        End With
                    End If
                    'added 6/25/2013 (already being set on LossNoticeImage)
                    If fnol.ReportedDate <> Nothing Then
                        .ReportedBy.ReportedDate = fnol.ReportedDate
                    Else
                        '6/25/2013 - updated to configurably use loss date or current date (since QA has an older system date and won't allow future reported dates)
                        If AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate") IsNot Nothing AndAlso AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate").ToString <> "" AndAlso UCase(AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate").ToString) = "YES" Then
                            .ReportedBy.ReportedDate = fnol.LossDate
                        Else
                            .ReportedBy.ReportedDate = Date.Today
                        End If
                    End If

                    'appears under Main, Location of accident
                    'this will be policyholder address unless values are entered under "loss information"
                    .LossAddress.HouseNumber = fnol.lossAddressHouseNum
                    .LossAddress.StreetName = fnol.lossAddressStreetName
                    .LossAddress.City = fnol.lossAddressCity
                    '6/6/2013 - added isnumeric check for stateId
                    If fnol.lossAddressState <> "" AndAlso IsNumeric(fnol.lossAddressState) = True Then
                        .LossAddress.StateId = fnol.lossAddressState
                    End If
                    .LossAddress.Zip = fnol.lossAddressZip
                    'updated 4/29/2013 to use new property; will overwrite existing stuff
                    If fnol.LossAddress IsNot Nothing Then '8/10/2017 note: AddAddress method will only overwrite if something is set
                        AddAddress(fnol.LossAddress, .LossAddress)
                    End If

                    With .LnImage()
                        .LossDate = fnol.LossDate '*
                        .LossTimeGiven = False
                        .ClaimTypeId = fnol.ClaimType '*
                        .PolicyId = fnol.PolicyID '*
                        .PolicyImageNum = fnol.PolicyImage '*
                        '.VersionId = 1 '*
                        'updated 3/11/2017 to use property set by FNOL page
                        '.VersionId = IntegerForString(fnol.PolicyVersionId)
                        .VersionId = If(IntegerForString(fnol.VersionId) > 0, IntegerForString(fnol.VersionId), IntegerForString(fnol.PolicyVersionId))
                        .PackagePartNum = fnol.packagePartType
                        If FNOLType = Enums.FNOL_LOB_Enum.CommercialAuto Then 'added to map severity to diamond for CAP 09/27/22 BD
                            .ClaimSeverityId = fnol.ClaimSeverity_Id
                        End If

                        If fnol.ReportedDate <> Nothing Then 'added 5/31/2013
                            .ReportedDate = fnol.ReportedDate
                        Else
                            '6/25/2013 - updated to configurably use loss date or current date (since QA has an older system date and won't allow future reported dates)
                            If AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate") IsNot Nothing AndAlso AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate").ToString <> "" AndAlso UCase(AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate").ToString) = "YES" Then
                                .ReportedDate = fnol.LossDate
                            Else
                                .ReportedDate = Date.Today '5/16/2013 - started defaulting
                            End If
                        End If

                        '.Notes = "538 Test"
                    End With
                    SetLnImageNotePropsIfNeeded(.LnImage)

                    'added 6/27/2019
                    .PolicyId = fnol.PolicyID '*
                    .PolicyImageNum = fnol.PolicyImage '*

                    ' Added 2/22/22 MGB 18670
                    Dim OmitInsuredClaimants As Boolean = Get_OmitInsuredClaimantsValue(FNOLType)

                    'claimants
                    If fnol.Claimants IsNot Nothing AndAlso fnol.Claimants.Count > 0 Then 'added IF 6/29/2016
                        If .Parties Is Nothing Then '6/29/2016: added logic to instantiate if needed
                            .Parties = New DCO.InsCollection(Of DCO.Claims.LossNotice.ThirdParty)
                        End If
                        For i As Integer = 0 To fnol.Claimants.Count - 1
                            'If IsInsuredClaimant(fnol.Claimants(i)) = False OrElse DiamondClaimsFNOL_OkayToAddInsuredClaimants() = True Then 'added IF 7/27/2016; previously happening every time
                            'updated 7/28/2016 to use new function and config key so default behavior would go back to adding them from our code
                            If IsInsuredClaimant(fnol.Claimants(i)) = False OrElse OmitInsuredClaimants = False Then
                                .Parties.Add(New DCO.Claims.LossNotice.ThirdParty)
                                With .Parties(i)

                                    'removed first/middle/last setting 8/9/2017... moved to ELSE below... update will hopefully correct any issues w/ displayName/sortName not being set, possibly due to setting personal name fields before commercial name fields
                                    '.Name.FirstName = fnol.Claimants(i).Firstname

                                    'If fnol.Claimants(i).Middlename <> "" Then
                                    '    .Name.MiddleName = fnol.Claimants(i).Middlename
                                    'End If

                                    '.Name.LastName = fnol.Claimants(i).Lastname
                                    'updated 4/29/2013 to use new property; will overwrite existing stuff
                                    'If fnol.Claimants(i).Name IsNot Nothing Then
                                    'updated 8/9/2017
                                    If fnol.Claimants(i).Name IsNot Nothing AndAlso HasNameInfo(fnol.Claimants(i).Name) = True Then
                                        AddName(fnol.Claimants(i).Name, .Name)
                                    Else 'added 8/9/2017
                                        .Name.FirstName = fnol.Claimants(i).Firstname

                                        If fnol.Claimants(i).Middlename <> "" Then
                                            .Name.MiddleName = fnol.Claimants(i).Middlename
                                        End If

                                        .Name.LastName = fnol.Claimants(i).Lastname
                                        .Name.TypeId = 1
                                    End If

                                    'removed address setting 8/10/2017... moved to ELSE below
                                    'If fnol.Claimants(i).Housenumber <> "" AndAlso fnol.Claimants(i).Streetname <> "" _
                                    '    AndAlso fnol.Claimants(i).City <> "" AndAlso fnol.Claimants(i).State <> "" _
                                    '    AndAlso fnol.Claimants(i).Zipcode <> "" Then
                                    '    .Address.HouseNumber = fnol.Claimants(i).Housenumber
                                    '    .Address.StreetName = fnol.Claimants(i).Streetname
                                    '    .Address.City = fnol.Claimants(i).City
                                    '    .Address.StateId = fnol.Claimants(i).State
                                    '    .Address.Zip = fnol.Claimants(i).Zipcode
                                    'Else
                                    '    'incomplete

                                    'End If
                                    'updated 4/29/2013 to use new property; will overwrite existing stuff
                                    'If fnol.Claimants(i).Address IsNot Nothing Then
                                    'updated 8/10/2017
                                    If fnol.Claimants(i).Address IsNot Nothing AndAlso HasAddressInfo(fnol.Claimants(i).Address) = True Then
                                        AddAddress(fnol.Claimants(i).Address, .Address)
                                    Else 'added 8/10/2017
                                        If fnol.Claimants(i).Housenumber <> "" AndAlso fnol.Claimants(i).Streetname <> "" _
                                        AndAlso fnol.Claimants(i).City <> "" AndAlso fnol.Claimants(i).State <> "" _
                                        AndAlso fnol.Claimants(i).Zipcode <> "" Then
                                            .Address.HouseNumber = fnol.Claimants(i).Housenumber
                                            .Address.StreetName = fnol.Claimants(i).Streetname
                                            .Address.City = fnol.Claimants(i).City
                                            .Address.StateId = fnol.Claimants(i).State
                                            .Address.Zip = fnol.Claimants(i).Zipcode
                                        Else
                                            'incomplete

                                        End If
                                    End If

                                    'updated 4/29/2013 to either use old properties or new ones
                                    If fnol.Claimants(i).Homephone <> "" OrElse fnol.Claimants(i).Businessphone <> "" OrElse fnol.Claimants(i).Cellphone <> "" OrElse fnol.Claimants(i).Email <> "" Then
                                        If fnol.Claimants(i).Homephone <> "" Then
                                            Dim phone As DCO.Phone = New DCO.Phone
                                            With phone
                                                'home
                                                .TypeId = 1
                                                '.DetailStatusCode = DCE.StatusCode.New '4/25/2013 - Diamond appears to automatically set
                                                .Number = fnol.Claimants(i).Homephone
                                            End With
                                            .Phones.Add(phone)
                                        End If

                                        If fnol.Claimants(i).Businessphone <> "" Then
                                            Dim phone As DCO.Phone = New DCO.Phone
                                            With phone
                                                'business
                                                .TypeId = 2
                                                .Number = fnol.Claimants(i).Businessphone
                                            End With
                                            .Phones.Add(phone)
                                        End If

                                        If fnol.Claimants(i).Cellphone <> "" Then
                                            Dim phone As DCO.Phone = New DCO.Phone
                                            With phone
                                                'cell
                                                .TypeId = 4
                                                .Number = fnol.Claimants(i).Cellphone
                                            End With
                                            .Phones.Add(phone)
                                        End If

                                        If fnol.Claimants(i).Email <> "" Then
                                            Dim email As New DCO.Email
                                            With email
                                                '.NameAddressSourceId = Diamond.Common.Enums.NameAddressSource.Claimant '4/25/2013 - Diamond appears to automatically set
                                                .TypeId = Diamond.Common.Enums.EMailType.Other
                                                'eml.Address = "user@domain.com"
                                                .Address = fnol.Claimants(i).Email
                                            End With
                                            .Emails.Add(email)
                                        End If
                                    Else
                                        'updated 4/29/2013 to use new properties
                                        AddPhones(fnol.Claimants(i).ContactInfo, .Phones)
                                        AddEmails(fnol.Claimants(i).ContactInfo, .Emails)
                                    End If

                                    If fnol.Claimants(i).claimantTypeID > 0 Then
                                        .ClaimantTypeId = fnol.Claimants(i).claimantTypeID
                                    End If

                                End With
                            End If

                        Next
                    End If
                    'end claimants

                    'added 6/29/2016 for 531; previously logic was using ClaimInsured1 and ClaimInsured2 properties above that are no longer around
                    'If DiamondClaimsFNOL_OkayToAddInsuredClaimants() = True Then 'added IF 7/27/2016; previously happening every time
                    'updated 7/28/2016 to use new function and config key so default behavior would go back to adding them from our code
                    If OmitInsuredClaimants = False Then
                        Dim insured1Party As DCO.Claims.LossNotice.ThirdParty = GetInsuredThirdPartyFromList(.Parties, Insured1orInsured2.Insured1, True) 'manually setting returnNewAndAddToListIf1or2AndNotFound to True in case default value ever changes
                        If insured1Party IsNot Nothing Then
                            With insured1Party
                                'set original props 1st like was originally being done above; removed 8/9/2017... moved to ELSE below... update will hopefully correct any issues w/ displayName/sortName not being set, possibly due to setting personal name fields before commercial name fields
                                '.Name.FirstName = fnol.insuredFirstName
                                '.Name.LastName = fnol.insuredLastName

                                'If fnol.Insured IsNot Nothing Then
                                'updated 7/27/2016 to also check for name info since objects are automatically instantiated
                                If fnol.Insured IsNot Nothing AndAlso fnol.Insured.Name IsNot Nothing AndAlso HasNameInfo(fnol.Insured.Name) = True Then
                                    AddPerson(fnol.Insured, .Name, .Address, .Phones, .Emails)
                                Else 'added 8/9/2017
                                    .Name.FirstName = fnol.insuredFirstName
                                    .Name.LastName = fnol.insuredLastName
                                    .Name.TypeId = 1
                                End If
                            End With

                            'note: don't even check for insured2Party unless insured1Party is something
                            'If fnol.SecondInsured IsNot Nothing Then
                            'updated 7/27/2016 to also check for name info since objects are automatically instantiated
                            If fnol.SecondInsured IsNot Nothing AndAlso fnol.SecondInsured.Name IsNot Nothing AndAlso HasNameInfo(fnol.SecondInsured.Name) = True Then
                                Dim insured2Party As DCO.Claims.LossNotice.ThirdParty = GetInsuredThirdPartyFromList(.Parties, Insured1orInsured2.Insured2, True) 'manually setting returnNewAndAddToListIf1or2AndNotFound to True in case default value ever changes
                                If insured2Party IsNot Nothing Then
                                    With insured2Party
                                        AddPerson(fnol.SecondInsured, .Name, .Address, .Phones, .Emails)
                                    End With
                                End If
                            End If
                        End If
                    End If

                    With .LnLossInfo
                        .ClaimLossTypeId = fnol.ClaimLossType '*
                        .Description = fnol.Description '*
                        .ClaimFaultId = fnol.ClaimFaultType
                        .AccidentLocationText = fnol.ClaimLocation
                        '.AccidentLocationAddressId = request.RequestData.LossNoticeData.LossAddress.AddressId 'added 4/23/2013 to see if this would automatically set it to the newly created address' id (don't think it's needed)

                        '0 = N/A
                        '.ClaimFaultId = 0
                        '1 = at fault
                        '.ClaimFaultId = 1
                        '2 Comparative Fault - Insd less than 50%
                        '.ClaimFaultId = 2
                        '3 Comparative Fault - Insd 50% or more
                        '.ClaimFaultId = 3
                        '4 Not At Fault
                        '.ClaimFaultId = 4
                        '5 Undetermined
                        '.ClaimFaultId = 5

                    End With

                End With

                If createXmls = True Then
                    Dim requestDataPath As String = fileDir & "SubmitLossNotice_RequestData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    .DumpToFile(requestDataPath)
                End If
            End With

            Dim SubmitLossNoticeEx As Exception = Nothing
            Try
                Using proxy As New Proxies.ClaimsServiceProxy
                    response = proxy.SubmitLossNotice(request)
                End Using
            Catch ex As Exception
                SubmitLossNoticeEx = ex
                Errors.Add(ex.ToString)
            End Try

            If createXmls = True Then
                Dim SubmitLossNoticeDv As Diamond.Common.Objects.DiamondValidation = Nothing
                If response IsNot Nothing Then
                    If response.ResponseData IsNot Nothing Then
                        Dim responseDataPath As String = fileDir & "SubmitLossNotice_ResponseData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                        response.ResponseData.DumpToFile(responseDataPath)
                    End If
                    If response.DiamondValidation IsNot Nothing Then
                        SubmitLossNoticeDv = response.DiamondValidation
                    End If
                End If
                If (SubmitLossNoticeDv Is Nothing OrElse SubmitLossNoticeDv.ValidationItems Is Nothing OrElse SubmitLossNoticeDv.ValidationItems.Count = 0) AndAlso SubmitLossNoticeEx IsNot Nothing Then
                    SubmitLossNoticeDv = New Diamond.Common.Objects.DiamondValidation
                    If SubmitLossNoticeDv.ValidationItems Is Nothing Then
                        SubmitLossNoticeDv.ValidationItems = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.ValidationItem)
                    End If
                    Dim vi As New Diamond.Common.Objects.ValidationItem(Diamond.Common.Objects.ValidationItemType.ValidationError, SubmitLossNoticeEx.ToString)
                    SubmitLossNoticeDv.ValidationItems.Add(vi)
                End If
                If SubmitLossNoticeDv IsNot Nothing Then
                    Dim diamondValidationPath As String = fileDir & "SubmitLossNotice_DiamondValidation" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    SubmitLossNoticeDv.DumpToFile(diamondValidationPath)
                End If
            End If

            'If response IsNot Nothing AndAlso response.DiamondValidation.ValidationItems.Count = 0 Then
            If response IsNot Nothing AndAlso response.ResponseData IsNot Nothing AndAlso response.ResponseData.ClaimControlId > 0 Then
                fnol.SetReadOnlyProperties(fnol, response.ResponseData.ClaimNumber, response.ResponseData.ClaimControlId)

                If (FNOLType = Enums.FNOL_LOB_Enum.Auto OrElse FNOLType = Enums.FNOL_LOB_Enum.CommercialAuto) AndAlso fnol.Vehicles.Count > 0 Then
                    ''If fnol.ClaimLossType = DiamondWebClass.Enums.ClaimLossType.VehicleDamage AndAlso fnol.Vehicles.Count > 0 Then

                    'Dim reqAddVehicles As New DCS.Messages.ClaimsService.SaveClaimControlVehicles.Request
                    'Dim rspAddVehicles As New DCS.Messages.ClaimsService.SaveClaimControlVehicles.Response

                    'With reqAddVehicles.RequestData
                    '    .ClaimControlId = response.ResponseData.ClaimControlId
                    'If .ClaimControlVehicles Is Nothing Then .ClaimControlVehicles = New DCO.InsCollection(Of DCO.Claims.ClaimControl.ClaimControlVehicle)
                    'For i As Integer = 0 To fnol.Vehicles.Count - 1
                    '    .ClaimControlVehicles.Add(New DCO.Claims.ClaimControl.ClaimControlVehicle)

                    '    With .ClaimControlVehicles(i) '5/15/2013 - will need to update to use i instead of 0 if we ever plan to set more than 1 vehicle; done 3/28/2024 just in case

                    '        .DamageDescription = fnol.Vehicles(i).DamageDescription
                    '        .Drivable = fnol.Vehicles(i).Drivable
                    '        .EstimatedAmount = fnol.Vehicles(i).EstimatedAmountOfDamage
                    '        .License = fnol.Vehicles(i).LicensePlate
                    '        .LicenseStateId = fnol.Vehicles(i).LicenseState
                    '        .Make = fnol.Vehicles(i).Make
                    '        .Model = fnol.Vehicles(i).Model
                    '        .VIN = fnol.Vehicles(i).VIN
                    '        .Year = fnol.Vehicles(i).Year
                    '        .AirbagsDeployedTypeId = fnol.Vehicles(i).AirbagsDeployedTypeId
                    '        .DrivableId = fnol.Vehicles(i).DrivableId
                    '        .CCCEstimateQualification = New Diamond.Common.Objects.ThirdParty.CCC.CCCEstimateQualification() With {
                    '            .Question1YesNoId = fnol.Vehicles(i).CCCEstimateQualificationId,
                    '            .Phone = fnol.Vehicles(i).CCCphone,
                    '            .SendQuickEstimate = (fnol.Vehicles(i).CCCEstimateQualificationId = 1),
                    '            .LanguageTypeId = 1
                    '        }

                    '        .LocationOfAccident.Address = New Diamond.Common.Objects.Address() With {
                    '            .StreetName = fnol.Vehicles(i).LossAddress.StreetName,
                    '            .City = fnol.Vehicles(i).LossAddress.City,
                    '            .StateId = Convert.ToInt32(fnol.Vehicles(i).LossAddress.StateId),
                    '            .Zip = fnol.Vehicles(i).LossAddress.ZipCode,
                    '            .HouseNumber = fnol.Vehicles(i).LossAddress.HouseNumber,
                    '            .County = fnol.Vehicles(i).LossAddress.County,
                    '            .Other = fnol.Vehicles(i).LossAddress.AddressOther,
                    '            .POBox = fnol.Vehicles(i).LossAddress.PoBox,
                    '            .ApartmentNumber = fnol.Vehicles(i).LossAddress.ApartmentNumber
                    '        }

                    '        'is garaged location in ui
                    '        '.LocationOfAccident.StreetName = "321 test lane"
                    '        '.LocationOfAccident.City = "some city"
                    '        '.LocationOfAccident.StateId = 16
                    '        '.LocationOfAccident.Zip = "47304"
                    '        '.LocationOfAccident.StreetName = fnol.Vehicles(i).LocationOfAccidentStreet
                    '        '.LocationOfAccident.City = fnol.Vehicles(i).LocationOfAccidentCity
                    '        '.LocationOfAccident.StateId = fnol.Vehicles(i).LocationOfAccidentState
                    '        '.LocationOfAccident.Zip = fnol.Vehicles(i).LocationOfAccidentZip


                    '        '.ClaimControlVehicleOwner.Name.FirstName = "Owner"
                    '        '.ClaimControlVehicleOwner.Name.MiddleName = "R"
                    '        '.ClaimControlVehicleOwner.Name.LastName = "ownerlast"
                    '        '.ClaimControlVehicleOwner.Name.FirstName = fnol.Vehicles(i).LossVehicleOwnerFirstName
                    '        '.ClaimControlVehicleOwner.Name.MiddleName = fnol.Vehicles(i).LossVehicleOwnerMiddleName
                    '        '.ClaimControlVehicleOwner.Name.LastName = fnol.Vehicles(i).LossVehicleOwnerLastName
                    '        'updated 8/10/2017
                    '        If fnol.Vehicles(i).LossVehicleOwnerName IsNot Nothing AndAlso HasNameInfo(fnol.Vehicles(i).LossVehicleOwnerName) = True Then
                    '            AddName(fnol.Vehicles(i).LossVehicleOwnerName, .ClaimControlVehicleOwner.Name)
                    '        Else
                    '            .ClaimControlVehicleOwner.Name.FirstName = fnol.Vehicles(i).LossVehicleOwnerFirstName
                    '            .ClaimControlVehicleOwner.Name.MiddleName = fnol.Vehicles(i).LossVehicleOwnerMiddleName
                    '            .ClaimControlVehicleOwner.Name.LastName = fnol.Vehicles(i).LossVehicleOwnerLastName
                    '        End If

                    '        '.ClaimControlVehicleOperator.Name.FirstName = "R"
                    '        '.ClaimControlVehicleOperator.Name.MiddleName = "mid"
                    '        '.ClaimControlVehicleOperator.Name.LastName = "operatorlast"
                    '        '.ClaimControlVehicleOperator.Name.FirstName = fnol.Vehicles(i).LossVehicleOperatorFirstName
                    '        '.ClaimControlVehicleOperator.Name.MiddleName = fnol.Vehicles(i).LossVehicleOperatorMiddleName
                    '        '.ClaimControlVehicleOperator.Name.LastName = fnol.Vehicles(i).LossVehicleOperatorLastName
                    '        'updated 8/10/2017
                    '        If fnol.Vehicles(i).LossVehicleOperatorName IsNot Nothing AndAlso HasNameInfo(fnol.Vehicles(i).LossVehicleOperatorName) = True Then
                    '            AddName(fnol.Vehicles(i).LossVehicleOperatorName, .ClaimControlVehicleOperator.Name)
                    '        Else
                    '            .ClaimControlVehicleOperator.Name.FirstName = fnol.Vehicles(i).LossVehicleOperatorFirstName
                    '            .ClaimControlVehicleOperator.Name.MiddleName = fnol.Vehicles(i).LossVehicleOperatorMiddleName
                    '            .ClaimControlVehicleOperator.Name.LastName = fnol.Vehicles(i).LossVehicleOperatorLastName
                    '        End If


                    '    End With
                    'Next

                    'If createXmls = True Then
                    '    Dim requestDataPath As String = fileDir & "SaveClaimControlVehicles_RequestData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    '    .DumpToFile(requestDataPath)
                    'End If
                    'End With

                    'Dim SaveClaimControlVehiclesEx As Exception = Nothing
                    'Try
                    '    Using proxy As New Proxies.ClaimsServiceProxy
                    '        rspAddVehicles = proxy.SaveClaimControlVehicles(reqAddVehicles)
                    '    End Using
                    'Catch ex As Exception
                    '    SaveClaimControlVehiclesEx = ex
                    '    SendEmail("save veh EXCEPTION! " & fnol.Vehicles.Count, "", ex.ToString)
                    'End Try

                    'If createXmls = True Then
                    '    Dim SaveClaimControlVehiclesDv As Diamond.Common.Objects.DiamondValidation = Nothing
                    '    If rspAddVehicles IsNot Nothing Then
                    '        If rspAddVehicles.ResponseData IsNot Nothing Then
                    '            Dim responseDataPath As String = fileDir & "SaveClaimControlVehicles_ResponseData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    '            rspAddVehicles.ResponseData.DumpToFile(responseDataPath)
                    '        End If
                    '        If rspAddVehicles.DiamondValidation IsNot Nothing Then
                    '            SaveClaimControlVehiclesDv = rspAddVehicles.DiamondValidation
                    '        End If
                    '    End If
                    '    If (SaveClaimControlVehiclesDv Is Nothing OrElse SaveClaimControlVehiclesDv.ValidationItems Is Nothing OrElse SaveClaimControlVehiclesDv.ValidationItems.Count = 0) AndAlso SaveClaimControlVehiclesEx IsNot Nothing Then
                    '        SaveClaimControlVehiclesDv = New Diamond.Common.Objects.DiamondValidation
                    '        If SaveClaimControlVehiclesDv.ValidationItems Is Nothing Then
                    '            SaveClaimControlVehiclesDv.ValidationItems = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.ValidationItem)
                    '        End If
                    '        Dim vi As New Diamond.Common.Objects.ValidationItem(Diamond.Common.Objects.ValidationItemType.ValidationError, SaveClaimControlVehiclesEx.ToString)
                    '        SaveClaimControlVehiclesDv.ValidationItems.Add(vi)
                    '    End If
                    '    If SaveClaimControlVehiclesDv IsNot Nothing Then
                    '        Dim diamondValidationPath As String = fileDir & "SaveClaimControlVehicles_DiamondValidation" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    '        SaveClaimControlVehiclesDv.DumpToFile(diamondValidationPath)
                    '    End If
                    'End If

                ElseIf FNOLType = Enums.FNOL_LOB_Enum.Liability OrElse FNOLType = Enums.FNOL_LOB_Enum.Propertty Then
                    'ElseIf fnol.ClaimLossType = DiamondWebClass.Enums.ClaimLossType.LiabilityAllOther OrElse fnol.ClaimLossType = DiamondWebClass.Enums.ClaimLossType.LiabilityPropertyDamage Then
                    'begin property, liability

                    If fnol.Properties.Count > 0 Then
                        Dim reqProp As New DCS.Messages.ClaimsService.SaveClaimControlProperties.Request
                        Dim resProp As New DCS.Messages.ClaimsService.SaveClaimControlProperties.Response
                        Dim SaveClaimControlPropertiesEx As Exception = Nothing
                        Try
                            With reqProp.RequestData
                                .ClaimControlId = response.ResponseData.ClaimControlId

                                If .ClaimControlProperties Is Nothing Then
                                    .ClaimControlProperties = New DCO.InsCollection(Of DCO.Claims.ClaimControl.ClaimControlProperty)
                                End If

                                For i As Integer = 0 To fnol.Properties.Count - 1

                                    .ClaimControlProperties.Add(New DCO.Claims.ClaimControl.ClaimControlProperty)

                                    With .ClaimControlProperties(0) '5/15/2013 - will need to update to use i instead of 0 if we ever plan to set more than 1 property

                                        If .Location Is Nothing Then
                                            .Location = New DCO.Address
                                        End If

                                        .DamageDescription = fnol.Properties(i).DamageDescription
                                        .Location.HouseNumber = fnol.Properties(i).Location.HouseNumber
                                        .Location.StreetName = fnol.Properties(i).Location.StreetName
                                        .Location.City = fnol.Properties(i).Location.City

                                        If fnol.Properties(i).Location.ApartmentNumber <> "" Then
                                            .Location.ApartmentNumber = fnol.Properties(i).Location.ApartmentNumber
                                        End If

                                        If fnol.Properties(i).Location.POBox <> "" Then
                                            '.Location.ApartmentNumber = fnol.Properties(i).Location.POBox
                                            'updated 4/29/2013 to set correct property
                                            .Location.POBox = fnol.Properties(i).Location.POBox
                                        End If

                                        If fnol.Properties(i).Location.Other <> "" Then
                                            '.Location.ApartmentNumber = fnol.Properties(i).Location.Other
                                            'updated 4/29/2013 to set correct property
                                            .Location.Other = fnol.Properties(i).Location.Other
                                        End If

                                        .Location.StateId = fnol.Properties(i).Location.State
                                        .Location.Zip = fnol.Properties(i).Location.Zip
                                        .EstimatedAmount = fnol.Properties(i).EstimatedAmount

                                        'updated 4/29/2013 to use new property; will overwrite existing stuff
                                        If fnol.Properties(i).Address IsNot Nothing Then '8/10/2017 note: AddAddress method will only overwrite if something is set
                                            AddAddress(fnol.Properties(i).Address, .Location)
                                        End If
                                    End With

                                Next

                                If createXmls = True Then
                                    Dim requestDataPath As String = fileDir & "SaveClaimControlProperties_RequestData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                                    .DumpToFile(requestDataPath)
                                End If
                            End With

                            Using proxy As New Proxies.ClaimsServiceProxy
                                resProp = proxy.SaveClaimControlProperties(reqProp)
                            End Using

                        Catch ex As Exception
                            SaveClaimControlPropertiesEx = ex
                            SendEmail("save property EXCEPTION! " & fnol.Properties.Count, "", ex.ToString)
                        End Try

                        If createXmls = True Then
                            Dim SaveClaimControlPropertiesDv As Diamond.Common.Objects.DiamondValidation = Nothing
                            If resProp IsNot Nothing Then
                                If resProp.ResponseData IsNot Nothing Then
                                    Dim responseDataPath As String = fileDir & "SaveClaimControlProperties_ResponseData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                                    resProp.ResponseData.DumpToFile(responseDataPath)
                                End If
                                If resProp.DiamondValidation IsNot Nothing Then
                                    SaveClaimControlPropertiesDv = resProp.DiamondValidation
                                End If
                            End If
                            If (SaveClaimControlPropertiesDv Is Nothing OrElse SaveClaimControlPropertiesDv.ValidationItems Is Nothing OrElse SaveClaimControlPropertiesDv.ValidationItems.Count = 0) AndAlso SaveClaimControlPropertiesEx IsNot Nothing Then
                                SaveClaimControlPropertiesDv = New Diamond.Common.Objects.DiamondValidation
                                If SaveClaimControlPropertiesDv.ValidationItems Is Nothing Then
                                    SaveClaimControlPropertiesDv.ValidationItems = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.ValidationItem)
                                End If
                                Dim vi As New Diamond.Common.Objects.ValidationItem(Diamond.Common.Objects.ValidationItemType.ValidationError, SaveClaimControlPropertiesEx.ToString)
                                SaveClaimControlPropertiesDv.ValidationItems.Add(vi)
                            End If
                            If SaveClaimControlPropertiesDv IsNot Nothing Then
                                Dim diamondValidationPath As String = fileDir & "SaveClaimControlProperties_DiamondValidation" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                                SaveClaimControlPropertiesDv.DumpToFile(diamondValidationPath)
                            End If
                        End If
                    End If

                Else
                    SendEmail("fnol NO LOSS TYPE! ", "", "type " & fnol.ClaimType & " veh " & fnol.Vehicles.Count & " prop " & fnol.Properties.Count)
                End If

            Else
                For Each vItem As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                    Errors.Add(vItem.Message)
                    SendEmail("ValidationItem", "", vItem.Message)
                Next
            End If

        Catch ex As Exception
            Errors.Add(ex.ToString)
        End Try
    End Sub

    Public Sub FNOL(ByVal fnol As DiamondClaims_FNOL)
        Errors.Clear()

        Dim request As New DCS.Messages.ClaimsService.SubmitLossNotice.Request
        Dim response As New DCS.Messages.ClaimsService.SubmitLossNotice.Response
        Try
            Dim systemData As DCSDM.SystemData = DUSDM.SystemDataManager.SystemData
            Dim submitData As DCSDM.SubmitData = DUSDM.SubmitDataManager.SubmitData
            Dim versionData As DCSDM.VersionData = DUSDM.VersionDataManager.VersionData(1, 16, 1, DUU.SystemDate.GetSystemDate, DUU.SystemDate.GetSystemDate)

            Dim fileDir As String = ""
            Dim xmlFileAddInfo As String = ""
            Dim createXmls As Boolean = CreateDiamondXmlsForTesting(fileDir:=fileDir)
            If createXmls = True AndAlso fnol IsNot Nothing AndAlso fnol.PolicyID > 0 Then
                xmlFileAddInfo = "_" & fnol.PolicyID.ToString & "_" & fnol.PolicyImage.ToString
            End If

            With request.RequestData
                .Attempt = fnol.SaveAttempt
                .ClaimLnStatusTypeId = fnol.StatusType '*
                '.IgnorePersonnel = True '*
                .User = New DCO.User
                With .User
                    .UsersId = fnol.UserID '*
                End With
                .LossNoticeData = New DCO.Claims.LossNotice.LossNoticeData
                .IgnorePersonnel = False

                With .LossNoticeData
                    'test monika 2
                    '.Personnel.ClaimOfficeId = 1
                    '.Personnel.InsideAdjusterId = 134
                    '.Personnel.AdministratorId = 135
                    .Personnel.ClaimOfficeId = fnol.claimOfficeID
                    .Personnel.InsideAdjusterId = fnol.InsideAdjusterId
                    .Personnel.AdministratorId = fnol.AdministratorId
                    If fnol.ClaimLossType = DiamondWebClass.Enums.ClaimLossType.VehicleDamage AndAlso fnol.Vehicles.Count > 0 Then
                        If .Vehicles Is Nothing Then .Vehicles = New DCO.InsCollection(Of Diamond.Common.Objects.Claims.LossNotice.LnVehicle)
                        For i As Integer = 0 To fnol.Vehicles.Count - 1
                            .Vehicles.Add(New LnVehicle)

                            With .Vehicles(i) '5/15/2013 - will need to update to use i instead of 0 if we ever plan to set more than 1 vehicle; done 3/28/2024 just in case

                                .ClaimLnVehicleNum = New DCO.IdValue(i + 1)
                                .DamageDescription = fnol.Vehicles(i).DamageDescription
                                .EstimatedAmount = fnol.Vehicles(i).EstimatedAmountOfDamage
                                .License = fnol.Vehicles(i).LicensePlate
                                .LicenseStateId = fnol.Vehicles(i).LicenseState
                                .Make = fnol.Vehicles(i).Make
                                .Model = fnol.Vehicles(i).Model
                                .VIN = fnol.Vehicles(i).VIN
                                .Year = fnol.Vehicles(i).Year
                                .VehicleDrivable = fnol.Vehicles(i).Drivable
                                .AirbagsDeployedTypeId = fnol.Vehicles(i).AirbagsDeployedTypeId
                                .DrivableId = fnol.Vehicles(i).DrivableId
                                If DiamondClaimsFNOLCCCClaimsEnabled() = True Then
                                    .CCCEstimateQualification = New Diamond.Common.Objects.ThirdParty.CCC.CCCEstimateQualification() With {
                                        .Question1YesNoId = fnol.Vehicles(i).CCCEstimateQualificationId,
                                        .Phone = If(IsNumeric(fnol.Vehicles(i).CCCphone), String.Format("{0:(###)###-####}", Long.Parse(fnol.Vehicles(i).CCCphone)), fnol.Vehicles(i).CCCphone),
                                        .SendQuickEstimate = (fnol.Vehicles(i).CCCEstimateQualificationId = 1),
                                        .LanguageTypeId = 1
                                    }
                                End If
                                AddAddress(fnol.Vehicles(i).LossAddress, .LocationOfAccident.Address)
                                '.LocationOfAccident.Address = New Diamond.Common.Objects.Address() With {
                                '        .StreetName = fnol.Vehicles(i).LossAddress.StreetName,
                                '        .City = fnol.Vehicles(i).LossAddress.City,
                                '        .StateId = Convert.ToInt32(fnol.Vehicles(i).LossAddress.StateId),
                                '        .Zip = fnol.Vehicles(i).LossAddress.ZipCode,
                                '        .HouseNumber = fnol.Vehicles(i).LossAddress.HouseNumber,
                                '        .County = fnol.Vehicles(i).LossAddress.County,
                                '        .Other = fnol.Vehicles(i).LossAddress.AddressOther,
                                '        .POBox = fnol.Vehicles(i).LossAddress.PoBox,
                                '        .ApartmentNumber = fnol.Vehicles(i).LossAddress.ApartmentNumber
                                '    }

                                '.ClaimControlVehicleOwner.Name.FirstName = "Owner"
                                '.ClaimControlVehicleOwner.Name.MiddleName = "R"
                                '.ClaimControlVehicleOwner.Name.LastName = "ownerlast"
                                '.ClaimControlVehicleOwner.Name.FirstName = fnol.Vehicles(i).LossVehicleOwnerFirstName
                                '.ClaimControlVehicleOwner.Name.MiddleName = fnol.Vehicles(i).LossVehicleOwnerMiddleName
                                '.ClaimControlVehicleOwner.Name.LastName = fnol.Vehicles(i).LossVehicleOwnerLastName
                                'updated 8/10/2017
                                If fnol.Vehicles(i).LossVehicleOwnerName IsNot Nothing AndAlso HasNameInfo(fnol.Vehicles(i).LossVehicleOwnerName) = True Then
                                    AddName(fnol.Vehicles(i).LossVehicleOwnerName, .LnVehicleOwner.Name)
                                Else
                                    .LnVehicleOwner.Name.FirstName = fnol.Vehicles(i).LossVehicleOwnerFirstName
                                    .LnVehicleOwner.Name.MiddleName = fnol.Vehicles(i).LossVehicleOwnerMiddleName
                                    .LnVehicleOwner.Name.LastName = fnol.Vehicles(i).LossVehicleOwnerLastName
                                    .LnVehicleOwner.Name.TypeId = 1
                                End If
                                AddAddress(fnol.Vehicles(i).LossAddress, .LnVehicleOwner.Address)

                                '.ClaimControlVehicleOperator.Name.FirstName = "R"
                                '.ClaimControlVehicleOperator.Name.MiddleName = "mid"
                                '.ClaimControlVehicleOperator.Name.LastName = "operatorlast"
                                '.ClaimControlVehicleOperator.Name.FirstName = fnol.Vehicles(i).LossVehicleOperatorFirstName
                                '.ClaimControlVehicleOperator.Name.MiddleName = fnol.Vehicles(i).LossVehicleOperatorMiddleName
                                '.ClaimControlVehicleOperator.Name.LastName = fnol.Vehicles(i).LossVehicleOperatorLastName
                                'updated 8/10/2017
                                If fnol.Vehicles(i).LossVehicleOperatorName IsNot Nothing AndAlso HasNameInfo(fnol.Vehicles(i).LossVehicleOperatorName) = True Then
                                    AddName(fnol.Vehicles(i).LossVehicleOperatorName, .LnVehicleOperator.Name)
                                Else
                                    .LnVehicleOperator.Name.FirstName = fnol.Vehicles(i).LossVehicleOperatorFirstName
                                    .LnVehicleOperator.Name.MiddleName = fnol.Vehicles(i).LossVehicleOperatorMiddleName
                                    .LnVehicleOperator.Name.LastName = fnol.Vehicles(i).LossVehicleOperatorLastName
                                    .LnVehicleOperator.Name.TypeId = 1
                                End If
                                AddAddress(fnol.Vehicles(i).LossAddress, .LnVehicleOperator.Address)


                            End With
                        Next
                    End If
                    'vehicle owner
                    'appears under Reported/Insured, insured 1
                    'commented ClaimInsured1 and ClaimInsured2 code 6/29/2016 for 531; now being done at bottom w/ Parties logic
                    '.ClaimInsured1.Name.FirstName = fnol.insuredFirstName '4/26/2013 - will now get overwritten by what comes thru new Insured object
                    '.ClaimInsured1.Name.LastName = fnol.insuredLastName
                    ''updated 4/19/2013 to include insured address; will probably use what comes thru on new Insured object instead
                    ''If fnol.PolicyID <> Nothing AndAlso fnol.PolicyID > 0 AndAlso fnol.PolicyImage <> Nothing AndAlso fnol.PolicyImage > 0 AndAlso AppSettings("connDiamond") IsNot Nothing AndAlso AppSettings("connDiamond").ToString <> "" Then
                    ''    Using Sql As New SQLselectObject(AppSettings("connDiamond"))
                    ''        Sql.queryOrStoredProc = "SELECT A.* FROM PolicyImageAddressLink as PIAL with (nolock) inner join Address as A with (nolock) on A.address_id = PIAL.address_id WHERE PIAL.nameaddresssource_id = 3 and PIAL.policy_id = " & fnol.PolicyID & " and PIAL.policyimage_num = " & fnol.PolicyImage
                    ''        Dim dr As SqlDataReader = Sql.GetDataReader
                    ''        If dr IsNot Nothing AndAlso dr.HasRows = True Then
                    ''            dr.Read()
                    ''            .ClaimInsured1.Address.HouseNumber = dr.Item("house_num").ToString.Trim
                    ''            .ClaimInsured1.Address.StreetName = dr.Item("street_name").ToString.Trim
                    ''            .ClaimInsured1.Address.City = dr.Item("city").ToString.Trim
                    ''            .ClaimInsured1.Address.StateId = dr.Item("state_id").ToString.Trim
                    ''            .ClaimInsured1.Address.Zip = dr.Item("zip").ToString.Trim
                    ''            .ClaimInsured1.Address.County = dr.Item("county").ToString.Trim
                    ''            .ClaimInsured1.Address.ApartmentNumber = dr.Item("apt_num").ToString.Trim
                    ''            .ClaimInsured1.Address.Other = dr.Item("other").ToString.Trim
                    ''            .ClaimInsured1.Address.POBox = dr.Item("pobox").ToString.Trim
                    ''        End If
                    ''    End Using
                    ''End If
                    ''added 4/26/2013
                    'If fnol.Insured IsNot Nothing Then
                    '    With .ClaimInsured1
                    '        'AddPersonName(fnol.Insured.Name, .Name)
                    '        'AddPersonAddress(fnol.Insured.Address, .Address)
                    '        'AddPersonPhones(fnol.Insured.ContactInfo, .Phones)
                    '        'AddPersonEmails(fnol.Insured.ContactInfo, .Emails)
                    '        'updated 4/29/2013 to set all thru one Sub
                    '        AddPerson(fnol.Insured, .Name, .Address, .Phones, .Emails)
                    '    End With
                    'End If
                    ''added 4/29/2013
                    'If fnol.SecondInsured IsNot Nothing Then
                    '    With .ClaimInsured2
                    '        'AddPersonName(fnol.SecondInsured.Name, .Name)
                    '        'AddPersonAddress(fnol.SecondInsured.Address, .Address)
                    '        'AddPersonPhones(fnol.SecondInsured.ContactInfo, .Phones)
                    '        'AddPersonEmails(fnol.SecondInsured.ContactInfo, .Emails)
                    '        'updated 4/29/2013 to set all thru one Sub
                    '        AddPerson(fnol.SecondInsured, .Name, .Address, .Phones, .Emails)
                    '    End With
                    'End If

                    '.Witnesses '4/23/2013 Diamond.Common.Objects.Claims.LossNotice.Witness (testing)
                    'Dim w As New Diamond.Common.Objects.Claims.LossNotice.Witness
                    'w.Name.FirstName = "Wit1First"
                    'w.Name.LastName = "Wit1Last"
                    'w.Address.HouseNumber = "123"
                    'w.Address.StreetName = "Wit1 St"
                    'w.Address.City = "Indy"
                    'w.Address.StateId = "16"
                    'w.Address.Zip = "46227-0000"
                    'w.Remarks = "witness 1 remarks"
                    ''w.RelationshipId = ""
                    '.Witnesses.Add(w)
                    'Dim w2 As New Diamond.Common.Objects.Claims.LossNotice.Witness
                    'w2.Name.FirstName = "Wit1First"
                    'w2.Name.LastName = "Wit1Last"
                    'w2.Address.HouseNumber = "123"
                    'w2.Address.StreetName = "Wit1 St"
                    'w2.Address.City = "Indy"
                    'w2.Address.StateId = "16"
                    'w2.Address.Zip = "46227-0000"
                    'w2.Remarks = "witness 2 remarks"
                    ''w2.RelationshipId = ""
                    '.Witnesses.Add(w2)
                    'added actual logic 4/26/2013
                    If fnol.Witnesses IsNot Nothing AndAlso fnol.Witnesses.Count > 0 Then
                        If .Witnesses Is Nothing Then '6/29/2016: added logic to instantiate if needed
                            .Witnesses = New DCO.InsCollection(Of DCO.Claims.LossNotice.Witness)
                        End If
                        For Each w As DiamondClaims_FNOL_Witness In fnol.Witnesses
                            Dim diaWit As New DCO.Claims.LossNotice.Witness
                            With diaWit
                                'updated 4/29/2013 to set all thru one Sub
                                AddPerson(w, .Name, .Address, .Phones, .Emails)

                                'AddPersonName(w.Name, .Name)
                                'If w.Firstname <> "" OrElse w.Lastname <> "" Then
                                '    With .Name
                                '        .FirstName = w.Firstname
                                '        .MiddleName = w.Middlename
                                '        .LastName = w.Lastname
                                '    End With
                                'End If
                                'AddPersonAddress(w.Address, .Address)
                                'If w.Housenumber <> "" OrElse w.Streetname <> "" OrElse w.City <> "" OrElse (w.StateId <> "" AndAlso IsNumeric(w.StateId) = True) OrElse w.Zipcode <> "" Then
                                '    With .Address
                                '        .HouseNumber = w.Housenumber
                                '        .StreetName = w.Streetname
                                '        .City = w.City
                                '        If w.StateId <> "" AndAlso IsNumeric(w.StateId) = True Then
                                '            .StateId = w.StateId
                                '        End If
                                '        .Zip = w.Zipcode
                                '    End With
                                'End If
                                .Remarks = w.Remarks
                                If w.RelationshipId <> "" AndAlso IsNumeric(w.RelationshipId) = True Then
                                    .RelationshipId = w.RelationshipId 'added 4/26/2013
                                End If
                                'AddPersonPhones(w.ContactInfo, .Phones)
                                'If w.Homephone <> "" Then
                                '    Dim p As New DCO.Phone
                                '    With p
                                '        '.TypeId = 1 (home)
                                '        .TypeId = Diamond.Common.Enums.PhoneType.Home
                                '        .Number = w.Homephone
                                '    End With
                                '    .Phones.Add(p)
                                'End If
                                'If w.Businessphone <> "" Then
                                '    Dim p As New DCO.Phone
                                '    With p
                                '        '.TypeId = 2 (business)
                                '        .TypeId = Diamond.Common.Enums.PhoneType.Business
                                '        .Number = w.Businessphone
                                '    End With
                                '    .Phones.Add(p)
                                'End If
                                'If w.Cellphone <> "" Then
                                '    Dim p As New DCO.Phone
                                '    With p
                                '        '.TypeId = 4 (cell)
                                '        .TypeId = Diamond.Common.Enums.PhoneType.Cellular
                                '        .Number = w.Cellphone
                                '    End With
                                '    .Phones.Add(p)
                                'End If
                                'AddPersonEmails(w.ContactInfo, .Emails)
                                'If w.Email <> "" Then
                                '    Dim e As New DCO.Email
                                '    With e
                                '        .TypeId = Diamond.Common.Enums.EMailType.Other
                                '        .Address = w.Email
                                '    End With
                                '    .Emails.Add(e)
                                'End If
                            End With
                            .Witnesses.Add(diaWit)
                        Next
                    End If

                    .ReportedBy.ClaimReportedByMethodId = 5
                    .ReportedBy.ClaimReportedById = 1
                    '.ReportedBy.Name.LastName = "" '4/23/2013 - can use for reported by name
                    'added 4/26/2013
                    If fnol.Reporter IsNot Nothing Then
                        With .ReportedBy
                            'AddPersonName(fnol.Reporter.Name, .Name)
                            'AddPersonAddress(fnol.Reporter.Address, .Address)
                            'AddPersonPhones(fnol.Reporter.ContactInfo, .Phones)
                            'AddPersonEmails(fnol.Reporter.ContactInfo, .Emails)
                            'updated 4/29/2013 to set all thru one Sub
                            AddPerson(fnol.Reporter, .Name, .Address, .Phones, .Emails)
                        End With
                    End If
                    'added 6/25/2013 (already being set on LossNoticeImage)
                    If fnol.ReportedDate <> Nothing Then
                        .ReportedBy.ReportedDate = fnol.ReportedDate
                    Else
                        '6/25/2013 - updated to configurably use loss date or current date (since QA has an older system date and won't allow future reported dates)
                        If AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate") IsNot Nothing AndAlso AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate").ToString <> "" AndAlso UCase(AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate").ToString) = "YES" Then
                            .ReportedBy.ReportedDate = fnol.LossDate
                        Else
                            .ReportedBy.ReportedDate = Date.Today
                        End If
                    End If

                    'appears under Main, Location of accident
                    'this will be policyholder address unless values are entered under "loss information"
                    .LossAddress.HouseNumber = fnol.lossAddressHouseNum
                    .LossAddress.StreetName = fnol.lossAddressStreetName
                    .LossAddress.City = fnol.lossAddressCity
                    '6/6/2013 - added isnumeric check for stateId
                    If fnol.lossAddressState <> "" AndAlso IsNumeric(fnol.lossAddressState) = True Then
                        .LossAddress.StateId = fnol.lossAddressState
                    End If
                    .LossAddress.Zip = fnol.lossAddressZip
                    'updated 4/29/2013 to use new property; will overwrite existing stuff
                    If fnol.LossAddress IsNot Nothing Then '8/10/2017 note: AddAddress method will only overwrite if something is set
                        AddAddress(fnol.LossAddress, .LossAddress)
                    End If

                    With .LnImage()
                        .LossDate = fnol.LossDate '*
                        .LossTimeGiven = False
                        .ClaimTypeId = fnol.ClaimType '*
                        .PolicyId = fnol.PolicyID '*
                        .PolicyImageNum = fnol.PolicyImage '*
                        '.VersionId = 1 '*
                        'updated 3/11/2017 to use property set by FNOL page
                        '.VersionId = IntegerForString(fnol.PolicyVersionId)
                        .VersionId = If(IntegerForString(fnol.VersionId) > 0, IntegerForString(fnol.VersionId), IntegerForString(fnol.PolicyVersionId))

                        .PackagePartNum = fnol.packagePartType

                        If fnol.ReportedDate <> Nothing Then 'added 5/31/2013
                            .ReportedDate = fnol.ReportedDate
                        Else
                            '6/25/2013 - updated to configurably use loss date or current date (since QA has an older system date and won't allow future reported dates)
                            If AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate") IsNot Nothing AndAlso AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate").ToString <> "" AndAlso UCase(AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate").ToString) = "YES" Then
                                .ReportedDate = fnol.LossDate
                            Else
                                .ReportedDate = Date.Today '5/16/2013 - started defaulting
                            End If
                        End If
                    End With
                    SetLnImageNotePropsIfNeeded(.LnImage)

                    'added 6/27/2019
                    .PolicyId = fnol.PolicyID '*
                    .PolicyImageNum = fnol.PolicyImage '*

                    ' NOTE: We do not perform the OmitInsuredClaimants logic here because this method's signature does not have the FNOLType parameter, which the deductible logic relies upon.
                    '       This should not be a problem, since this method is not being called from TStationTransations at the moment.

                    'claimants
                    If fnol.Claimants IsNot Nothing AndAlso fnol.Claimants.Count > 0 Then 'added IF 6/29/2016
                        If .Parties Is Nothing Then '6/29/2016: added logic to instantiate if needed
                            .Parties = New DCO.InsCollection(Of DCO.Claims.LossNotice.ThirdParty)
                        End If
                        For i As Integer = 0 To fnol.Claimants.Count - 1
                            'If IsInsuredClaimant(fnol.Claimants(i)) = False OrElse DiamondClaimsFNOL_OkayToAddInsuredClaimants() = True Then 'added IF 7/27/2016; previously happening every time
                            'updated 7/28/2016 to use new function and config key so default behavior would go back to adding them from our code
                            If IsInsuredClaimant(fnol.Claimants(i)) = False OrElse DiamondClaimsFNOL_OmitInsuredClaimants() = False Then
                                .Parties.Add(New DCO.Claims.LossNotice.ThirdParty)
                                With .Parties(i)

                                    'removed first/middle/last setting 8/9/2017... moved to ELSE below... update will hopefully correct any issues w/ displayName/sortName not being set, possibly due to setting personal name fields before commercial name fields
                                    '.Name.FirstName = fnol.Claimants(i).Firstname

                                    'If fnol.Claimants(i).Middlename <> "" Then
                                    '    .Name.MiddleName = fnol.Claimants(i).Middlename
                                    'End If

                                    '.Name.LastName = fnol.Claimants(i).Lastname
                                    'updated 4/29/2013 to use new property; will overwrite existing stuff
                                    'If fnol.Claimants(i).Name IsNot Nothing Then
                                    'updated 8/9/2017
                                    If fnol.Claimants(i).Name IsNot Nothing AndAlso HasNameInfo(fnol.Claimants(i).Name) = True Then
                                        AddName(fnol.Claimants(i).Name, .Name)
                                    Else 'added 8/9/2017
                                        .Name.FirstName = fnol.Claimants(i).Firstname

                                        If fnol.Claimants(i).Middlename <> "" Then
                                            .Name.MiddleName = fnol.Claimants(i).Middlename
                                        End If

                                        .Name.LastName = fnol.Claimants(i).Lastname
                                        .Name.TypeId = 1
                                    End If

                                    'removed address setting 8/10/2017... moved to ELSE below
                                    'If fnol.Claimants(i).Housenumber <> "" AndAlso fnol.Claimants(i).Streetname <> "" _
                                    '    AndAlso fnol.Claimants(i).City <> "" AndAlso fnol.Claimants(i).State <> "" _
                                    '    AndAlso fnol.Claimants(i).Zipcode <> "" Then
                                    '    .Address.HouseNumber = fnol.Claimants(i).Housenumber
                                    '    .Address.StreetName = fnol.Claimants(i).Streetname
                                    '    .Address.City = fnol.Claimants(i).City
                                    '    .Address.StateId = fnol.Claimants(i).State
                                    '    .Address.Zip = fnol.Claimants(i).Zipcode
                                    'Else
                                    '    'incomplete

                                    'End If
                                    'updated 4/29/2013 to use new property; will overwrite existing stuff
                                    'If fnol.Claimants(i).Address IsNot Nothing Then
                                    'updated 8/10/2017
                                    If fnol.Claimants(i).Address IsNot Nothing AndAlso HasAddressInfo(fnol.Claimants(i).Address) = True Then
                                        AddAddress(fnol.Claimants(i).Address, .Address)
                                    Else 'added 8/10/2017
                                        If fnol.Claimants(i).Housenumber <> "" AndAlso fnol.Claimants(i).Streetname <> "" _
                                        AndAlso fnol.Claimants(i).City <> "" AndAlso fnol.Claimants(i).State <> "" _
                                        AndAlso fnol.Claimants(i).Zipcode <> "" Then
                                            .Address.HouseNumber = fnol.Claimants(i).Housenumber
                                            .Address.StreetName = fnol.Claimants(i).Streetname
                                            .Address.City = fnol.Claimants(i).City
                                            .Address.StateId = fnol.Claimants(i).State
                                            .Address.Zip = fnol.Claimants(i).Zipcode
                                        Else
                                            'incomplete

                                        End If
                                    End If

                                    'updated 4/29/2013 to either use old properties or new ones
                                    If fnol.Claimants(i).Homephone <> "" OrElse fnol.Claimants(i).Businessphone <> "" OrElse fnol.Claimants(i).Cellphone <> "" OrElse fnol.Claimants(i).Email <> "" Then
                                        If fnol.Claimants(i).Homephone <> "" Then
                                            Dim phone As DCO.Phone = New DCO.Phone
                                            With phone
                                                'home
                                                .TypeId = 1
                                                '.DetailStatusCode = DCE.StatusCode.New '4/25/2013 - Diamond appears to automatically set
                                                .Number = fnol.Claimants(i).Homephone
                                            End With
                                            .Phones.Add(phone)
                                        End If

                                        If fnol.Claimants(i).Businessphone <> "" Then
                                            Dim phone As DCO.Phone = New DCO.Phone
                                            With phone
                                                'business
                                                .TypeId = 2
                                                .Number = fnol.Claimants(i).Businessphone
                                            End With
                                            .Phones.Add(phone)
                                        End If

                                        If fnol.Claimants(i).Cellphone <> "" Then
                                            Dim phone As DCO.Phone = New DCO.Phone
                                            With phone
                                                'cell
                                                .TypeId = 4
                                                .Number = fnol.Claimants(i).Cellphone
                                            End With
                                            .Phones.Add(phone)
                                        End If

                                        If fnol.Claimants(i).Email <> "" Then
                                            Dim email As New DCO.Email
                                            With email
                                                '.NameAddressSourceId = Diamond.Common.Enums.NameAddressSource.Claimant '4/25/2013 - Diamond appears to automatically set
                                                .TypeId = Diamond.Common.Enums.EMailType.Other
                                                'eml.Address = "user@domain.com"
                                                .Address = fnol.Claimants(i).Email
                                            End With
                                            .Emails.Add(email)
                                        End If
                                    Else
                                        'updated 4/29/2013 to use new properties
                                        AddPhones(fnol.Claimants(i).ContactInfo, .Phones)
                                        AddEmails(fnol.Claimants(i).ContactInfo, .Emails)
                                    End If

                                    If fnol.Claimants(i).claimantTypeID > 0 Then
                                        .ClaimantTypeId = fnol.Claimants(i).claimantTypeID
                                    End If

                                End With
                            End If

                        Next
                    End If
                    'end claimants

                    'added 6/29/2016 for 531; previously logic was using ClaimInsured1 and ClaimInsured2 properties above that are no longer around
                    'If DiamondClaimsFNOL_OkayToAddInsuredClaimants() = True Then 'added IF 7/27/2016; previously happening every time
                    'updated 7/28/2016 to use new function and config key so default behavior would go back to adding them from our code
                    If DiamondClaimsFNOL_OmitInsuredClaimants() = False Then 'added IF 7/27/2016; previously happening every time
                        Dim insured1Party As DCO.Claims.LossNotice.ThirdParty = GetInsuredThirdPartyFromList(.Parties, Insured1orInsured2.Insured1, True) 'manually setting returnNewAndAddToListIf1or2AndNotFound to True in case default value ever changes
                        If insured1Party IsNot Nothing Then
                            With insured1Party
                                'set original props 1st like was originally being done above; removed 8/9/2017... moved to ELSE below... update will hopefully correct any issues w/ displayName/sortName not being set, possibly due to setting personal name fields before commercial name fields
                                '.Name.FirstName = fnol.insuredFirstName
                                '.Name.LastName = fnol.insuredLastName

                                'If fnol.Insured IsNot Nothing Then
                                'updated 7/27/2016 to also check for name info since objects are automatically instantiated
                                If fnol.Insured IsNot Nothing AndAlso fnol.Insured.Name IsNot Nothing AndAlso HasNameInfo(fnol.Insured.Name) = True Then
                                    AddPerson(fnol.Insured, .Name, .Address, .Phones, .Emails)
                                Else 'added 8/9/2017
                                    .Name.FirstName = fnol.insuredFirstName
                                    .Name.LastName = fnol.insuredLastName
                                    .Name.TypeId = 1
                                End If
                            End With

                            'note: don't even check for insured2Party unless insured1Party is something
                            'If fnol.SecondInsured IsNot Nothing Then
                            'updated 7/27/2016 to also check for name info since objects are automatically instantiated
                            If fnol.SecondInsured IsNot Nothing AndAlso fnol.SecondInsured.Name IsNot Nothing AndAlso HasNameInfo(fnol.SecondInsured.Name) = True Then
                                Dim insured2Party As DCO.Claims.LossNotice.ThirdParty = GetInsuredThirdPartyFromList(.Parties, Insured1orInsured2.Insured2, True) 'manually setting returnNewAndAddToListIf1or2AndNotFound to True in case default value ever changes
                                If insured2Party IsNot Nothing Then
                                    With insured2Party
                                        AddPerson(fnol.SecondInsured, .Name, .Address, .Phones, .Emails)
                                    End With
                                End If
                            End If
                        End If
                    End If

                    With .LnLossInfo
                        .ClaimLossTypeId = fnol.ClaimLossType '*
                        .Description = fnol.Description '*
                        .ClaimFaultId = fnol.ClaimFaultType
                        .AccidentLocationText = fnol.ClaimLocation
                        '.AccidentLocationAddressId = request.RequestData.LossNoticeData.LossAddress.AddressId 'added 4/23/2013 to see if this would automatically set it to the newly created address' id (don't think it's needed)

                        '0 = N/A
                        '.ClaimFaultId = 0
                        '1 = at fault
                        '.ClaimFaultId = 1
                        '2 Comparative Fault - Insd less than 50%
                        '.ClaimFaultId = 2
                        '3 Comparative Fault - Insd 50% or more
                        '.ClaimFaultId = 3
                        '4 Not At Fault
                        '.ClaimFaultId = 4
                        '5 Undetermined
                        '.ClaimFaultId = 5

                    End With

                End With

                If createXmls = True Then
                    Dim requestDataPath As String = fileDir & "SubmitLossNotice_RequestData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    .DumpToFile(requestDataPath)
                End If
            End With

            Dim SubmitLossNoticeEx As Exception = Nothing
            Try
                Using proxy As New Proxies.ClaimsServiceProxy
                    response = proxy.SubmitLossNotice(request)
                End Using
            Catch ex As Exception
                SubmitLossNoticeEx = ex
                Errors.Add(ex.ToString)
            End Try

            If createXmls = True Then
                Dim SubmitLossNoticeDv As Diamond.Common.Objects.DiamondValidation = Nothing
                If response IsNot Nothing Then
                    If response.ResponseData IsNot Nothing Then
                        Dim responseDataPath As String = fileDir & "SubmitLossNotice_ResponseData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                        response.ResponseData.DumpToFile(responseDataPath)
                    End If
                    If response.DiamondValidation IsNot Nothing Then
                        SubmitLossNoticeDv = response.DiamondValidation
                    End If
                End If
                If (SubmitLossNoticeDv Is Nothing OrElse SubmitLossNoticeDv.ValidationItems Is Nothing OrElse SubmitLossNoticeDv.ValidationItems.Count = 0) AndAlso SubmitLossNoticeEx IsNot Nothing Then
                    SubmitLossNoticeDv = New Diamond.Common.Objects.DiamondValidation
                    If SubmitLossNoticeDv.ValidationItems Is Nothing Then
                        SubmitLossNoticeDv.ValidationItems = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.ValidationItem)
                    End If
                    Dim vi As New Diamond.Common.Objects.ValidationItem(Diamond.Common.Objects.ValidationItemType.ValidationError, SubmitLossNoticeEx.ToString)
                    SubmitLossNoticeDv.ValidationItems.Add(vi)
                End If
                If SubmitLossNoticeDv IsNot Nothing Then
                    Dim diamondValidationPath As String = fileDir & "SubmitLossNotice_DiamondValidation" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    SubmitLossNoticeDv.DumpToFile(diamondValidationPath)
                End If
            End If

            'If response IsNot Nothing AndAlso response.DiamondValidation.ValidationItems.Count = 0 Then
            If response IsNot Nothing AndAlso response.ResponseData IsNot Nothing AndAlso response.ResponseData.ClaimControlId > 0 Then
                fnol.SetReadOnlyProperties(fnol, response.ResponseData.ClaimNumber, response.ResponseData.ClaimControlId)

                If fnol.ClaimLossType = DiamondWebClass.Enums.ClaimLossType.VehicleDamage AndAlso fnol.Vehicles.Count > 0 Then

                    '    Dim reqAddVehicles As New DCS.Messages.ClaimsService.SaveClaimControlVehicles.Request
                    '    Dim rspAddVehicles As New DCS.Messages.ClaimsService.SaveClaimControlVehicles.Response

                    '    With reqAddVehicles.RequestData
                    '        .ClaimControlId = response.ResponseData.ClaimControlId
                    '        'If .ClaimControlVehicles Is Nothing Then .ClaimControlVehicles = New DCO.InsCollection(Of DCO.Claims.ClaimControl.ClaimControlVehicle)
                    '        'For i As Integer = 0 To fnol.Vehicles.Count - 1
                    '        '    .ClaimControlVehicles.Add(New DCO.Claims.ClaimControl.ClaimControlVehicle)

                    '        '    With .ClaimControlVehicles(i) '5/15/2013 - will need to update to use i instead of 0 if we ever plan to set more than 1 vehicle; done 3/28/2024 just in case

                    '        '        .DamageDescription = fnol.Vehicles(i).DamageDescription
                    '        '        .Drivable = fnol.Vehicles(i).Drivable
                    '        '        .EstimatedAmount = fnol.Vehicles(i).EstimatedAmountOfDamage
                    '        '        .License = fnol.Vehicles(i).LicensePlate
                    '        '        .LicenseStateId = fnol.Vehicles(i).LicenseState
                    '        '        .Make = fnol.Vehicles(i).Make
                    '        '        .Model = fnol.Vehicles(i).Model
                    '        '        .VIN = fnol.Vehicles(i).VIN
                    '        '        .Year = fnol.Vehicles(i).Year

                    '        '        'is garaged location in ui
                    '        '        '.LocationOfAccident.StreetName = "321 test lane"
                    '        '        '.LocationOfAccident.City = "some city"
                    '        '        '.LocationOfAccident.StateId = 16
                    '        '        '.LocationOfAccident.Zip = "47304"
                    '        '        '.LocationOfAccident.StreetName = fnol.Vehicles(i).LocationOfAccidentStreet
                    '        '        '.LocationOfAccident.City = fnol.Vehicles(i).LocationOfAccidentCity
                    '        '        '.LocationOfAccident.StateId = fnol.Vehicles(i).LocationOfAccidentState
                    '        '        '.LocationOfAccident.Zip = fnol.Vehicles(i).LocationOfAccidentZip


                    '        '        '.ClaimControlVehicleOwner.Name.FirstName = "Owner"
                    '        '        '.ClaimControlVehicleOwner.Name.MiddleName = "R"
                    '        '        '.ClaimControlVehicleOwner.Name.LastName = "ownerlast"
                    '        '        '.ClaimControlVehicleOwner.Name.FirstName = fnol.Vehicles(i).LossVehicleOwnerFirstName
                    '        '        '.ClaimControlVehicleOwner.Name.MiddleName = fnol.Vehicles(i).LossVehicleOwnerMiddleName
                    '        '        '.ClaimControlVehicleOwner.Name.LastName = fnol.Vehicles(i).LossVehicleOwnerLastName
                    '        '        'updated 8/10/2017
                    '        '        If fnol.Vehicles(i).LossVehicleOwnerName IsNot Nothing AndAlso HasNameInfo(fnol.Vehicles(i).LossVehicleOwnerName) = True Then
                    '        '            AddName(fnol.Vehicles(i).LossVehicleOwnerName, .ClaimControlVehicleOwner.Name)
                    '        '        Else
                    '        '            .ClaimControlVehicleOwner.Name.FirstName = fnol.Vehicles(i).LossVehicleOwnerFirstName
                    '        '            .ClaimControlVehicleOwner.Name.MiddleName = fnol.Vehicles(i).LossVehicleOwnerMiddleName
                    '        '            .ClaimControlVehicleOwner.Name.LastName = fnol.Vehicles(i).LossVehicleOwnerLastName
                    '        '        End If

                    '        '        '.ClaimControlVehicleOperator.Name.FirstName = "R"
                    '        '        '.ClaimControlVehicleOperator.Name.MiddleName = "mid"
                    '        '        '.ClaimControlVehicleOperator.Name.LastName = "operatorlast"
                    '        '        '.ClaimControlVehicleOperator.Name.FirstName = fnol.Vehicles(i).LossVehicleOperatorFirstName
                    '        '        '.ClaimControlVehicleOperator.Name.MiddleName = fnol.Vehicles(i).LossVehicleOperatorMiddleName
                    '        '        '.ClaimControlVehicleOperator.Name.LastName = fnol.Vehicles(i).LossVehicleOperatorLastName
                    '        '        'updated 8/10/2017
                    '        '        If fnol.Vehicles(i).LossVehicleOperatorName IsNot Nothing AndAlso HasNameInfo(fnol.Vehicles(i).LossVehicleOperatorName) = True Then
                    '        '            AddName(fnol.Vehicles(i).LossVehicleOperatorName, .ClaimControlVehicleOperator.Name)
                    '        '        Else
                    '        '            .ClaimControlVehicleOperator.Name.FirstName = fnol.Vehicles(i).LossVehicleOperatorFirstName
                    '        '            .ClaimControlVehicleOperator.Name.MiddleName = fnol.Vehicles(i).LossVehicleOperatorMiddleName
                    '        '            .ClaimControlVehicleOperator.Name.LastName = fnol.Vehicles(i).LossVehicleOperatorLastName
                    '        '        End If


                    '        '    End With
                    '        'Next

                    '        'If createXmls = True Then
                    '        '    Dim requestDataPath As String = fileDir & "SaveClaimControlVehicles_RequestData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    '        '    .DumpToFile(requestDataPath)
                    '        'End If
                    '    End With

                    '    Dim SaveClaimControlVehiclesEx As Exception = Nothing
                    '    Try
                    '        Using proxy As New Proxies.ClaimsServiceProxy
                    '            rspAddVehicles = proxy.SaveClaimControlVehicles(reqAddVehicles)
                    '        End Using
                    '    Catch ex As Exception
                    '        SaveClaimControlVehiclesEx = ex
                    '        SendEmail("save veh EXCEPTION! " & fnol.Vehicles.Count, "", ex.ToString)
                    '    End Try

                    '    If createXmls = True Then
                    '        Dim SaveClaimControlVehiclesDv As Diamond.Common.Objects.DiamondValidation = Nothing
                    '        If rspAddVehicles IsNot Nothing Then
                    '            If rspAddVehicles.ResponseData IsNot Nothing Then
                    '                Dim responseDataPath As String = fileDir & "SaveClaimControlVehicles_ResponseData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    '                rspAddVehicles.ResponseData.DumpToFile(responseDataPath)
                    '            End If
                    '            If rspAddVehicles.DiamondValidation IsNot Nothing Then
                    '                SaveClaimControlVehiclesDv = rspAddVehicles.DiamondValidation
                    '            End If
                    '        End If
                    '        If (SaveClaimControlVehiclesDv Is Nothing OrElse SaveClaimControlVehiclesDv.ValidationItems Is Nothing OrElse SaveClaimControlVehiclesDv.ValidationItems.Count = 0) AndAlso SaveClaimControlVehiclesEx IsNot Nothing Then
                    '            SaveClaimControlVehiclesDv = New Diamond.Common.Objects.DiamondValidation
                    '            If SaveClaimControlVehiclesDv.ValidationItems Is Nothing Then
                    '                SaveClaimControlVehiclesDv.ValidationItems = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.ValidationItem)
                    '            End If
                    '            Dim vi As New Diamond.Common.Objects.ValidationItem(Diamond.Common.Objects.ValidationItemType.ValidationError, SaveClaimControlVehiclesEx.ToString)
                    '            SaveClaimControlVehiclesDv.ValidationItems.Add(vi)
                    '        End If
                    '        If SaveClaimControlVehiclesDv IsNot Nothing Then
                    '            Dim diamondValidationPath As String = fileDir & "SaveClaimControlVehicles_DiamondValidation" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    '            SaveClaimControlVehiclesDv.DumpToFile(diamondValidationPath)
                    '        End If
                    '    End If
                ElseIf fnol.ClaimLossType = DiamondWebClass.Enums.ClaimLossType.LiabilityAllOther OrElse fnol.ClaimLossType = DiamondWebClass.Enums.ClaimLossType.LiabilityPropertyDamage Then

                    'begin property, liability

                    If fnol.Properties.Count > 0 Then
                        Dim reqProp As New DCS.Messages.ClaimsService.SaveClaimControlProperties.Request
                        Dim resProp As New DCS.Messages.ClaimsService.SaveClaimControlProperties.Response
                        Dim SaveClaimControlPropertiesEx As Exception = Nothing
                        Try
                            With reqProp.RequestData
                                .ClaimControlId = response.ResponseData.ClaimControlId

                                If .ClaimControlProperties Is Nothing Then
                                    .ClaimControlProperties = New DCO.InsCollection(Of DCO.Claims.ClaimControl.ClaimControlProperty)
                                End If

                                For i As Integer = 0 To fnol.Properties.Count - 1

                                    .ClaimControlProperties.Add(New DCO.Claims.ClaimControl.ClaimControlProperty)

                                    With .ClaimControlProperties(0) '5/15/2013 - will need to update to use i instead of 0 if we ever plan to set more than 1 property

                                        If .Location Is Nothing Then
                                            .Location = New DCO.Address
                                        End If

                                        .DamageDescription = fnol.Properties(i).DamageDescription
                                        .Location.HouseNumber = fnol.Properties(i).Location.HouseNumber
                                        .Location.StreetName = fnol.Properties(i).Location.StreetName
                                        .Location.City = fnol.Properties(i).Location.City

                                        If fnol.Properties(i).Location.ApartmentNumber <> "" Then
                                            .Location.ApartmentNumber = fnol.Properties(i).Location.ApartmentNumber
                                        End If

                                        If fnol.Properties(i).Location.POBox <> "" Then
                                            '.Location.ApartmentNumber = fnol.Properties(i).Location.POBox
                                            'updated 4/29/2013 to set correct property
                                            .Location.POBox = fnol.Properties(i).Location.POBox
                                        End If

                                        If fnol.Properties(i).Location.Other <> "" Then
                                            '.Location.ApartmentNumber = fnol.Properties(i).Location.Other
                                            'updated 4/29/2013 to set correct property
                                            .Location.Other = fnol.Properties(i).Location.Other
                                        End If

                                        .Location.StateId = fnol.Properties(i).Location.State
                                        .Location.Zip = fnol.Properties(i).Location.Zip
                                        .EstimatedAmount = fnol.Properties(i).EstimatedAmount

                                        'updated 4/29/2013 to use new property; will overwrite existing stuff
                                        If fnol.Properties(i).Address IsNot Nothing Then '8/10/2017 note: AddAddress method will only overwrite if something is set
                                            AddAddress(fnol.Properties(i).Address, .Location)
                                        End If
                                    End With

                                Next

                                If createXmls = True Then
                                    Dim requestDataPath As String = fileDir & "SaveClaimControlProperties_RequestData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                                    .DumpToFile(requestDataPath)
                                End If
                            End With

                            Using proxy As New Proxies.ClaimsServiceProxy
                                resProp = proxy.SaveClaimControlProperties(reqProp)
                            End Using

                        Catch ex As Exception
                            SaveClaimControlPropertiesEx = ex
                            SendEmail("save property EXCEPTION! " & fnol.Properties.Count, "", ex.ToString)
                        End Try

                        If createXmls = True Then
                            Dim SaveClaimControlPropertiesDv As Diamond.Common.Objects.DiamondValidation = Nothing
                            If resProp IsNot Nothing Then
                                If resProp.ResponseData IsNot Nothing Then
                                    Dim responseDataPath As String = fileDir & "SaveClaimControlProperties_ResponseData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                                    resProp.ResponseData.DumpToFile(responseDataPath)
                                End If
                                If resProp.DiamondValidation IsNot Nothing Then
                                    SaveClaimControlPropertiesDv = resProp.DiamondValidation
                                End If
                            End If
                            If (SaveClaimControlPropertiesDv Is Nothing OrElse SaveClaimControlPropertiesDv.ValidationItems Is Nothing OrElse SaveClaimControlPropertiesDv.ValidationItems.Count = 0) AndAlso SaveClaimControlPropertiesEx IsNot Nothing Then
                                SaveClaimControlPropertiesDv = New Diamond.Common.Objects.DiamondValidation
                                If SaveClaimControlPropertiesDv.ValidationItems Is Nothing Then
                                    SaveClaimControlPropertiesDv.ValidationItems = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.ValidationItem)
                                End If
                                Dim vi As New Diamond.Common.Objects.ValidationItem(Diamond.Common.Objects.ValidationItemType.ValidationError, SaveClaimControlPropertiesEx.ToString)
                                SaveClaimControlPropertiesDv.ValidationItems.Add(vi)
                            End If
                            If SaveClaimControlPropertiesDv IsNot Nothing Then
                                Dim diamondValidationPath As String = fileDir & "SaveClaimControlProperties_DiamondValidation" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                                SaveClaimControlPropertiesDv.DumpToFile(diamondValidationPath)
                            End If
                        End If
                    End If

                Else
                    SendEmail("fnol NO LOSS TYPE! ", "", "type " & fnol.ClaimType & " veh " & fnol.Vehicles.Count & " prop " & fnol.Properties.Count)
                End If

            Else
                For Each vItem As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                    Errors.Add(vItem.Message)
                    SendEmail("ValidationItem", "", vItem.Message)
                Next
            End If

        Catch ex As Exception
            Errors.Add(ex.ToString)
        End Try
    End Sub


    ''' <summary>
    ''' FNOL with Diamond Claim Auto Assignment.  11/30/2020  MGB
    ''' ONLY APPLIES TO AUTO CLAIMS!!
    ''' </summary>
    ''' <param name="fnol"></param>
    ''' <param name="FNOLType"></param>
    ''' <remarks></remarks>
    Public Sub FNOL_ClaimAutoAssignment(ByVal fnol As DiamondClaims_FNOL, ByVal FNOLType As Enums.FNOL_LOB_Enum, ByRef ReturnData As FNOLResponseData_Struct)
        Errors.Clear()

        Dim request As New DCS.Messages.ClaimsService.SubmitLossNotice.Request
        Dim response As New DCS.Messages.ClaimsService.SubmitLossNotice.Response
        Try
            If FNOLType <> Enums.FNOL_LOB_Enum.Auto AndAlso FNOLType <> Enums.FNOL_LOB_Enum.CommercialAuto AndAlso FNOLType <> Enums.FNOL_LOB_Enum.Propertty Then
                Throw New Exception("Auto Assignment is for AUTO and PROPERTY claims only.")
            End If

            Dim systemData As DCSDM.SystemData = DUSDM.SystemDataManager.SystemData
            Dim submitData As DCSDM.SubmitData = DUSDM.SubmitDataManager.SubmitData
            Dim versionData As DCSDM.VersionData = DUSDM.VersionDataManager.VersionData(1, 16, 1, DUU.SystemDate.GetSystemDate, DUU.SystemDate.GetSystemDate)

            Dim fileDir As String = ""
            Dim xmlFileAddInfo As String = ""
            Dim createXmls As Boolean = CreateDiamondXmlsForTesting(fileDir:=fileDir)
            If createXmls = True AndAlso fnol IsNot Nothing AndAlso fnol.PolicyID > 0 Then
                xmlFileAddInfo = "_" & fnol.PolicyID.ToString & "_" & fnol.PolicyImage.ToString
            End If

            With request.RequestData
                .Attempt = fnol.SaveAttempt
                .ClaimLnStatusTypeId = fnol.StatusType
                .User = New DCO.User
                With .User
                    .UsersId = fnol.UserID '*
                End With
                .LossNoticeData = New DCO.Claims.LossNotice.LossNoticeData
                .IgnorePersonnel = False  ' Must be false for automatic claim assignment
                .IgnorePersonnelExceptOffice = False  ' Must be false for automatic claim assignment

                With .LossNoticeData
                    'test monika 3
                    .Personnel.WorkflowQueueId = 0  ' Must be <= 0 for automatic claim assignment
                    .Personnel.ClaimOfficeId = fnol.claimOfficeID
                    .Personnel.InsideAdjusterId = fnol.InsideAdjusterId
                    .Personnel.AdministratorId = fnol.AdministratorId

                    If (FNOLType = Enums.FNOL_LOB_Enum.Auto OrElse FNOLType = Enums.FNOL_LOB_Enum.CommercialAuto) AndAlso fnol.Vehicles.Count > 0 Then
                        If .Vehicles Is Nothing Then .Vehicles = New DCO.InsCollection(Of Diamond.Common.Objects.Claims.LossNotice.LnVehicle)
                        For i As Integer = 0 To fnol.Vehicles.Count - 1
                            .Vehicles.Add(New LnVehicle)

                            With .Vehicles(i) '5/15/2013 - will need to update to use i instead of 0 if we ever plan to set more than 1 vehicle; done 3/28/2024 just in case

                                .ClaimLnVehicleNum = New DCO.IdValue(i + 1)
                                .DamageDescription = fnol.Vehicles(i).DamageDescription
                                .EstimatedAmount = fnol.Vehicles(i).EstimatedAmountOfDamage
                                .License = fnol.Vehicles(i).LicensePlate
                                .LicenseStateId = fnol.Vehicles(i).LicenseState
                                .Make = fnol.Vehicles(i).Make
                                .Model = fnol.Vehicles(i).Model
                                .VIN = fnol.Vehicles(i).VIN
                                .Year = fnol.Vehicles(i).Year
                                .VehicleDrivable = fnol.Vehicles(i).Drivable
                                .AirbagsDeployedTypeId = fnol.Vehicles(i).AirbagsDeployedTypeId
                                .DrivableId = fnol.Vehicles(i).DrivableId
                                If DiamondClaimsFNOLCCCClaimsEnabled() = True Then
                                    .CCCEstimateQualification = New Diamond.Common.Objects.ThirdParty.CCC.CCCEstimateQualification() With {
                                        .Question1YesNoId = fnol.Vehicles(i).CCCEstimateQualificationId,
                                        .Phone = If(IsNumeric(fnol.Vehicles(i).CCCphone), String.Format("{0:(###)###-####}", Long.Parse(fnol.Vehicles(i).CCCphone)), fnol.Vehicles(i).CCCphone),
                                        .SendQuickEstimate = (fnol.Vehicles(i).CCCEstimateQualificationId = 1),
                                        .LanguageTypeId = 1
                                    }
                                End If
                                AddAddress(fnol.Vehicles(i).LossAddress, .LocationOfAccident.Address)
                                '.LocationOfAccident.Address = New Diamond.Common.Objects.Address() With {
                                '        .StreetName = fnol.Vehicles(i).LossAddress.StreetName,
                                '        .City = fnol.Vehicles(i).LossAddress.City,
                                '        .StateId = Convert.ToInt32(fnol.Vehicles(i).LossAddress.StateId),
                                '        .Zip = fnol.Vehicles(i).LossAddress.ZipCode,
                                '        .HouseNumber = fnol.Vehicles(i).LossAddress.HouseNumber,
                                '        .County = fnol.Vehicles(i).LossAddress.County,
                                '        .Other = fnol.Vehicles(i).LossAddress.AddressOther,
                                '        .POBox = fnol.Vehicles(i).LossAddress.PoBox,
                                '        .ApartmentNumber = fnol.Vehicles(i).LossAddress.ApartmentNumber
                                '    }

                                '.ClaimControlVehicleOwner.Name.FirstName = "Owner"
                                '.ClaimControlVehicleOwner.Name.MiddleName = "R"
                                '.ClaimControlVehicleOwner.Name.LastName = "ownerlast"
                                '.ClaimControlVehicleOwner.Name.FirstName = fnol.Vehicles(i).LossVehicleOwnerFirstName
                                '.ClaimControlVehicleOwner.Name.MiddleName = fnol.Vehicles(i).LossVehicleOwnerMiddleName
                                '.ClaimControlVehicleOwner.Name.LastName = fnol.Vehicles(i).LossVehicleOwnerLastName
                                'updated 8/10/2017
                                If fnol.Vehicles(i).LossVehicleOwnerName IsNot Nothing AndAlso HasNameInfo(fnol.Vehicles(i).LossVehicleOwnerName) = True Then
                                    AddName(fnol.Vehicles(i).LossVehicleOwnerName, .LnVehicleOwner.Name)
                                Else
                                    .LnVehicleOwner.Name.FirstName = fnol.Vehicles(i).LossVehicleOwnerFirstName
                                    .LnVehicleOwner.Name.MiddleName = fnol.Vehicles(i).LossVehicleOwnerMiddleName
                                    .LnVehicleOwner.Name.LastName = fnol.Vehicles(i).LossVehicleOwnerLastName
                                    .LnVehicleOwner.Name.TypeId = 1
                                End If
                                AddAddress(fnol.Vehicles(i).LossAddress, .LnVehicleOwner.Address)

                                '.ClaimControlVehicleOperator.Name.FirstName = "R"
                                '.ClaimControlVehicleOperator.Name.MiddleName = "mid"
                                '.ClaimControlVehicleOperator.Name.LastName = "operatorlast"
                                '.ClaimControlVehicleOperator.Name.FirstName = fnol.Vehicles(i).LossVehicleOperatorFirstName
                                '.ClaimControlVehicleOperator.Name.MiddleName = fnol.Vehicles(i).LossVehicleOperatorMiddleName
                                '.ClaimControlVehicleOperator.Name.LastName = fnol.Vehicles(i).LossVehicleOperatorLastName
                                'updated 8/10/2017
                                If fnol.Vehicles(i).LossVehicleOperatorName IsNot Nothing AndAlso HasNameInfo(fnol.Vehicles(i).LossVehicleOperatorName) = True Then
                                    AddName(fnol.Vehicles(i).LossVehicleOperatorName, .LnVehicleOperator.Name)
                                Else
                                    .LnVehicleOperator.Name.FirstName = fnol.Vehicles(i).LossVehicleOperatorFirstName
                                    .LnVehicleOperator.Name.MiddleName = fnol.Vehicles(i).LossVehicleOperatorMiddleName
                                    .LnVehicleOperator.Name.LastName = fnol.Vehicles(i).LossVehicleOperatorLastName
                                    .LnVehicleOperator.Name.TypeId = 1
                                End If
                                AddAddress(fnol.Vehicles(i).LossAddress, .LnVehicleOperator.Address)


                            End With
                        Next
                    End If
                    If fnol.Witnesses IsNot Nothing AndAlso fnol.Witnesses.Count > 0 Then
                        If .Witnesses Is Nothing Then '6/29/2016: added logic to instantiate if needed
                            .Witnesses = New DCO.InsCollection(Of DCO.Claims.LossNotice.Witness)
                        End If
                        For Each w As DiamondClaims_FNOL_Witness In fnol.Witnesses
                            Dim diaWit As New DCO.Claims.LossNotice.Witness
                            With diaWit
                                AddPerson(w, .Name, .Address, .Phones, .Emails)
                                .Remarks = w.Remarks
                                If w.RelationshipId <> "" AndAlso IsNumeric(w.RelationshipId) = True Then
                                    .RelationshipId = w.RelationshipId 'added 4/26/2013
                                End If
                            End With
                            .Witnesses.Add(diaWit)
                        Next
                    End If

                    .ReportedBy.ClaimReportedByMethodId = 5
                    .ReportedBy.ClaimReportedById = 1
                    If fnol.Reporter IsNot Nothing Then
                        With .ReportedBy
                            AddPerson(fnol.Reporter, .Name, .Address, .Phones, .Emails)
                        End With
                    End If
                    If fnol.ReportedDate <> Nothing Then
                        .ReportedBy.ReportedDate = fnol.ReportedDate
                    Else
                        If AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate") IsNot Nothing AndAlso AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate").ToString <> "" AndAlso UCase(AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate").ToString) = "YES" Then
                            .ReportedBy.ReportedDate = fnol.LossDate
                        Else
                            .ReportedBy.ReportedDate = Date.Today
                        End If
                    End If

                    'appears under Main, Location of accident
                    'this will be policyholder address unless values are entered under "loss information"
                    .LossAddress.HouseNumber = fnol.lossAddressHouseNum
                    .LossAddress.StreetName = fnol.lossAddressStreetName
                    .LossAddress.City = fnol.lossAddressCity
                    If fnol.lossAddressState <> "" AndAlso IsNumeric(fnol.lossAddressState) = True Then
                        .LossAddress.StateId = fnol.lossAddressState
                    End If
                    .LossAddress.Zip = fnol.lossAddressZip
                    If fnol.LossAddress IsNot Nothing Then '8/10/2017 note: AddAddress method will only overwrite if something is set
                        AddAddress(fnol.LossAddress, .LossAddress)
                    End If

                    With .LnImage()
                        .ClaimSeverityId = fnol.ClaimSeverity_Id ' Added 12/8/20 MGB
                        .LossDate = fnol.LossDate '*
                        .LossTimeGiven = False
                        .ClaimTypeId = fnol.ClaimType '*
                        .PolicyId = fnol.PolicyID '*
                        .PolicyImageNum = fnol.PolicyImage '*
                        '.VersionId = 1 '*
                        'updated 3/11/2017 to use property set by FNOL page
                        '.VersionId = IntegerForString(fnol.PolicyVersionId)
                        .VersionId = If(IntegerForString(fnol.VersionId) > 0, IntegerForString(fnol.VersionId), IntegerForString(fnol.PolicyVersionId))

                        .PackagePartNum = fnol.packagePartType

                        If fnol.ReportedDate <> Nothing Then 'added 5/31/2013
                            .ReportedDate = fnol.ReportedDate
                        Else
                            If AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate") IsNot Nothing AndAlso AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate").ToString <> "" AndAlso UCase(AppSettings("DiamondClaimsFNOL_ReportedDate_UseLossDate").ToString) = "YES" Then
                                .ReportedDate = fnol.LossDate
                            Else
                                .ReportedDate = Date.Today '5/16/2013 - started defaulting
                            End If
                        End If
                    End With
                    SetLnImageNotePropsIfNeeded(.LnImage)

                    .PolicyId = fnol.PolicyID '*
                    .PolicyImageNum = fnol.PolicyImage '*

                    ' Added 2/22/22 MGB 18670
                    Dim OmitInsuredClaimants As Boolean = Get_OmitInsuredClaimantsValue(FNOLType)

                    'claimants
                    If fnol.Claimants IsNot Nothing AndAlso fnol.Claimants.Count > 0 Then 'added IF 6/29/2016
                        If .Parties Is Nothing Then '6/29/2016: added logic to instantiate if needed
                            .Parties = New DCO.InsCollection(Of DCO.Claims.LossNotice.ThirdParty)
                        End If
                        For i As Integer = 0 To fnol.Claimants.Count - 1
                            If IsInsuredClaimant(fnol.Claimants(i)) = False OrElse OmitInsuredClaimants = False Then
                                .Parties.Add(New DCO.Claims.LossNotice.ThirdParty)
                                With .Parties(i)
                                    If fnol.Claimants(i).Name IsNot Nothing AndAlso HasNameInfo(fnol.Claimants(i).Name) = True Then
                                        AddName(fnol.Claimants(i).Name, .Name)
                                    Else 'added 8/9/2017
                                        .Name.FirstName = fnol.Claimants(i).Firstname

                                        If fnol.Claimants(i).Middlename <> "" Then
                                            .Name.MiddleName = fnol.Claimants(i).Middlename
                                        End If

                                        .Name.LastName = fnol.Claimants(i).Lastname
                                        .Name.TypeId = 1
                                    End If
                                    If fnol.Claimants(i).Address IsNot Nothing AndAlso HasAddressInfo(fnol.Claimants(i).Address) = True Then
                                        AddAddress(fnol.Claimants(i).Address, .Address)
                                    Else 'added 8/10/2017
                                        If fnol.Claimants(i).Housenumber <> "" AndAlso fnol.Claimants(i).Streetname <> "" _
                                        AndAlso fnol.Claimants(i).City <> "" AndAlso fnol.Claimants(i).State <> "" _
                                        AndAlso fnol.Claimants(i).Zipcode <> "" Then
                                            .Address.HouseNumber = fnol.Claimants(i).Housenumber
                                            .Address.StreetName = fnol.Claimants(i).Streetname
                                            .Address.City = fnol.Claimants(i).City
                                            .Address.StateId = fnol.Claimants(i).State
                                            .Address.Zip = fnol.Claimants(i).Zipcode
                                        Else
                                            'incomplete
                                        End If
                                    End If

                                    'updated 4/29/2013 to either use old properties or new ones
                                    If fnol.Claimants(i).Homephone <> "" OrElse fnol.Claimants(i).Businessphone <> "" OrElse fnol.Claimants(i).Cellphone <> "" OrElse fnol.Claimants(i).Email <> "" Then
                                        If fnol.Claimants(i).Homephone <> "" Then
                                            Dim phone As DCO.Phone = New DCO.Phone
                                            With phone
                                                'home
                                                .TypeId = 1
                                                '.DetailStatusCode = DCE.StatusCode.New '4/25/2013 - Diamond appears to automatically set
                                                .Number = fnol.Claimants(i).Homephone
                                            End With
                                            .Phones.Add(phone)
                                        End If

                                        If fnol.Claimants(i).Businessphone <> "" Then
                                            Dim phone As DCO.Phone = New DCO.Phone
                                            With phone
                                                'business
                                                .TypeId = 2
                                                .Number = fnol.Claimants(i).Businessphone
                                            End With
                                            .Phones.Add(phone)
                                        End If

                                        If fnol.Claimants(i).Cellphone <> "" Then
                                            Dim phone As DCO.Phone = New DCO.Phone
                                            With phone
                                                'cell
                                                .TypeId = 4
                                                .Number = fnol.Claimants(i).Cellphone
                                            End With
                                            .Phones.Add(phone)
                                        End If

                                        If fnol.Claimants(i).Email <> "" Then
                                            Dim email As New DCO.Email
                                            With email
                                                '.NameAddressSourceId = Diamond.Common.Enums.NameAddressSource.Claimant '4/25/2013 - Diamond appears to automatically set
                                                .TypeId = Diamond.Common.Enums.EMailType.Other
                                                'eml.Address = "user@domain.com"
                                                .Address = fnol.Claimants(i).Email
                                            End With
                                            .Emails.Add(email)
                                        End If
                                    Else
                                        'updated 4/29/2013 to use new properties
                                        AddPhones(fnol.Claimants(i).ContactInfo, .Phones)
                                        AddEmails(fnol.Claimants(i).ContactInfo, .Emails)
                                    End If

                                    If fnol.Claimants(i).claimantTypeID > 0 Then
                                        .ClaimantTypeId = fnol.Claimants(i).claimantTypeID
                                    End If

                                End With
                            End If

                        Next
                    End If
                    'end claimants

                    If OmitInsuredClaimants = False Then
                        Dim insured1Party As DCO.Claims.LossNotice.ThirdParty = GetInsuredThirdPartyFromList(.Parties, Insured1orInsured2.Insured1, True) 'manually setting returnNewAndAddToListIf1or2AndNotFound to True in case default value ever changes
                        If insured1Party IsNot Nothing Then
                            With insured1Party
                                If fnol.Insured IsNot Nothing AndAlso fnol.Insured.Name IsNot Nothing AndAlso HasNameInfo(fnol.Insured.Name) = True Then
                                    AddPerson(fnol.Insured, .Name, .Address, .Phones, .Emails)
                                Else 'added 8/9/2017
                                    .Name.FirstName = fnol.insuredFirstName
                                    .Name.LastName = fnol.insuredLastName
                                    .Name.TypeId = 1
                                End If
                            End With

                            'note: don't even check for insured2Party unless insured1Party is something
                            If fnol.SecondInsured IsNot Nothing AndAlso fnol.SecondInsured.Name IsNot Nothing AndAlso HasNameInfo(fnol.SecondInsured.Name) = True Then
                                Dim insured2Party As DCO.Claims.LossNotice.ThirdParty = GetInsuredThirdPartyFromList(.Parties, Insured1orInsured2.Insured2, True) 'manually setting returnNewAndAddToListIf1or2AndNotFound to True in case default value ever changes
                                If insured2Party IsNot Nothing Then
                                    With insured2Party
                                        AddPerson(fnol.SecondInsured, .Name, .Address, .Phones, .Emails)
                                    End With
                                End If
                            End If
                        End If
                    End If

                    With .LnLossInfo
                        .ClaimLossTypeId = fnol.ClaimLossType '*
                        .Description = fnol.Description '*
                        .ClaimFaultId = fnol.ClaimFaultType
                        .AccidentLocationText = fnol.ClaimLocation
                        '.AccidentLocationAddressId = request.RequestData.LossNoticeData.LossAddress.AddressId 'added 4/23/2013 to see if this would automatically set it to the newly created address' id (don't think it's needed)

                        '0 = N/A
                        '.ClaimFaultId = 0
                        '1 = at fault
                        '.ClaimFaultId = 1
                        '2 Comparative Fault - Insd less than 50%
                        '.ClaimFaultId = 2
                        '3 Comparative Fault - Insd 50% or more
                        '.ClaimFaultId = 3
                        '4 Not At Fault
                        '.ClaimFaultId = 4
                        '5 Undetermined
                        '.ClaimFaultId = 5

                    End With

                End With

                If createXmls = True Then
                    Dim requestDataPath As String = fileDir & "SubmitLossNotice_RequestData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    .DumpToFile(requestDataPath)
                End If
            End With

            '!! THIS IS WHERE THE FNOL GETS SUBMITTED TO DIAMOND  !!
            Dim SubmitLossNoticeEx As Exception = Nothing
            Try
                Using proxy As New Proxies.ClaimsServiceProxy
                    response = proxy.SubmitLossNotice(request)
                    ' Check the response for the assignment data
                    'If StringFieldHasNonZeroNumericValue(response.ResponseData.LossNoticeData.Personnel.InsideAdjusterId) OrElse StringFieldHasNonZeroNumericValue(response.ResponseData.LossNoticeData.Personnel.OutsideAdjusterId) Then
                    'updated 4/5/2024 to make sure it's different than what was defaulted in the request
                    If (response.ResponseData.LossNoticeData.Personnel.InsideAdjusterId > 0 AndAlso response.ResponseData.LossNoticeData.Personnel.InsideAdjusterId <> request.RequestData.LossNoticeData.Personnel.InsideAdjusterId) OrElse (response.ResponseData.LossNoticeData.Personnel.OutsideAdjusterId > 0 AndAlso response.ResponseData.LossNoticeData.Personnel.OutsideAdjusterId <> request.RequestData.LossNoticeData.Personnel.OutsideAdjusterId) Then
                        ReturnData.AdjusterName = ""
                        ReturnData.AssignedBy = "Diamond Automatic"
                        ReturnData.AssignedSuccessfully = True
                        ReturnData.CAT = False
                        ReturnData.DateAssigned = DateTime.Now.ToShortDateString()
                        ' Adjuster ID
                        If StringFieldHasNonZeroNumericValue(response.ResponseData.LossNoticeData.Personnel.InsideAdjusterId) Then
                            ReturnData.DiamondAdjuster_Id = response.ResponseData.LossNoticeData.Personnel.InsideAdjusterId
                        ElseIf StringFieldHasNonZeroNumericValue(response.ResponseData.LossNoticeData.Personnel.OutsideAdjusterId) Then
                            ReturnData.DiamondAdjuster_Id = response.ResponseData.LossNoticeData.Personnel.OutsideAdjusterId
                        End If
                        ReturnData.FNOLAdjusterID = ""
                        ReturnData.FNOL_ID = 0
                    Else
                        ' No Adjuster ID returned
                        ReturnData = New FNOLResponseData_Struct()
                        ReturnData.AssignedSuccessfully = False
                    End If
                End Using
            Catch ex As Exception
                SubmitLossNoticeEx = ex
                Errors.Add(ex.ToString)
            End Try

            If createXmls = True Then
                Dim SubmitLossNoticeDv As Diamond.Common.Objects.DiamondValidation = Nothing
                If response IsNot Nothing Then
                    If response.ResponseData IsNot Nothing Then
                        Dim responseDataPath As String = fileDir & "SubmitLossNotice_ResponseData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                        response.ResponseData.DumpToFile(responseDataPath)
                    End If
                    If response.DiamondValidation IsNot Nothing Then
                        SubmitLossNoticeDv = response.DiamondValidation
                    End If
                End If
                If (SubmitLossNoticeDv Is Nothing OrElse SubmitLossNoticeDv.ValidationItems Is Nothing OrElse SubmitLossNoticeDv.ValidationItems.Count = 0) AndAlso SubmitLossNoticeEx IsNot Nothing Then
                    SubmitLossNoticeDv = New Diamond.Common.Objects.DiamondValidation
                    If SubmitLossNoticeDv.ValidationItems Is Nothing Then
                        SubmitLossNoticeDv.ValidationItems = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.ValidationItem)
                    End If
                    Dim vi As New Diamond.Common.Objects.ValidationItem(Diamond.Common.Objects.ValidationItemType.ValidationError, SubmitLossNoticeEx.ToString)
                    SubmitLossNoticeDv.ValidationItems.Add(vi)
                End If
                If SubmitLossNoticeDv IsNot Nothing Then
                    Dim diamondValidationPath As String = fileDir & "SubmitLossNotice_DiamondValidation" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    SubmitLossNoticeDv.DumpToFile(diamondValidationPath)
                End If
            End If


            'If response IsNot Nothing AndAlso response.DiamondValidation.ValidationItems.Count = 0 Then
            If response IsNot Nothing AndAlso response.ResponseData IsNot Nothing AndAlso response.ResponseData.ClaimControlId > 0 Then
                fnol.SetReadOnlyProperties(fnol, response.ResponseData.ClaimNumber, response.ResponseData.ClaimControlId)

                If (FNOLType = Enums.FNOL_LOB_Enum.Auto OrElse FNOLType = Enums.FNOL_LOB_Enum.CommercialAuto) AndAlso fnol.Vehicles.Count > 0 Then
                    '    'If fnol.ClaimLossType = DiamondWebClass.Enums.ClaimLossType.VehicleDamage AndAlso fnol.Vehicles.Count > 0 Then

                    '    Dim reqAddVehicles As New DCS.Messages.ClaimsService.SaveClaimControlVehicles.Request
                    '    Dim rspAddVehicles As New DCS.Messages.ClaimsService.SaveClaimControlVehicles.Response

                    '    With reqAddVehicles.RequestData
                    '        .ClaimControlId = response.ResponseData.ClaimControlId
                    '        'If .ClaimControlVehicles Is Nothing Then .ClaimControlVehicles = New DCO.InsCollection(Of DCO.Claims.ClaimControl.ClaimControlVehicle)
                    '        'For i As Integer = 0 To fnol.Vehicles.Count - 1
                    '        '    .ClaimControlVehicles.Add(New DCO.Claims.ClaimControl.ClaimControlVehicle)
                    '        '    'test Monika
                    '        '    With .ClaimControlVehicles(i) '5/15/2013 - will need to update to use i instead of 0 if we ever plan to set more than 1 vehicle; done 3/28/2024 just in case

                    '        '        .DamageDescription = fnol.Vehicles(i).DamageDescription
                    '        '        .Drivable = fnol.Vehicles(i).Drivable
                    '        '        .EstimatedAmount = fnol.Vehicles(i).EstimatedAmountOfDamage
                    '        '        .License = fnol.Vehicles(i).LicensePlate
                    '        '        .LicenseStateId = fnol.Vehicles(i).LicenseState
                    '        '        .Make = fnol.Vehicles(i).Make
                    '        '        .Model = fnol.Vehicles(i).Model
                    '        '        .VIN = fnol.Vehicles(i).VIN
                    '        '        .Year = fnol.Vehicles(i).Year
                    '        '        If fnol.Vehicles(i).LossVehicleOwnerName IsNot Nothing AndAlso HasNameInfo(fnol.Vehicles(i).LossVehicleOwnerName) = True Then
                    '        '            AddName(fnol.Vehicles(i).LossVehicleOwnerName, .ClaimControlVehicleOwner.Name)
                    '        '        Else
                    '        '            .ClaimControlVehicleOwner.Name.FirstName = fnol.Vehicles(i).LossVehicleOwnerFirstName
                    '        '            .ClaimControlVehicleOwner.Name.MiddleName = fnol.Vehicles(i).LossVehicleOwnerMiddleName
                    '        '            .ClaimControlVehicleOwner.Name.LastName = fnol.Vehicles(i).LossVehicleOwnerLastName
                    '        '        End If
                    '        '        If fnol.Vehicles(i).LossVehicleOperatorName IsNot Nothing AndAlso HasNameInfo(fnol.Vehicles(i).LossVehicleOperatorName) = True Then
                    '        '            AddName(fnol.Vehicles(i).LossVehicleOperatorName, .ClaimControlVehicleOperator.Name)
                    '        '        Else
                    '        '            .ClaimControlVehicleOperator.Name.FirstName = fnol.Vehicles(i).LossVehicleOperatorFirstName
                    '        '            .ClaimControlVehicleOperator.Name.MiddleName = fnol.Vehicles(i).LossVehicleOperatorMiddleName
                    '        '            .ClaimControlVehicleOperator.Name.LastName = fnol.Vehicles(i).LossVehicleOperatorLastName
                    '        '        End If


                    '        '    End With
                    '        'Next

                    '        If createXmls = True Then
                    '            Dim requestDataPath As String = fileDir & "SaveClaimControlVehicles_RequestData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    '            .DumpToFile(requestDataPath)
                    '        End If
                    '    End With

                    '    Dim SaveClaimControlVehiclesEx As Exception = Nothing
                    '    Try
                    '        Using proxy As New Proxies.ClaimsServiceProxy
                    '            rspAddVehicles = proxy.SaveClaimControlVehicles(reqAddVehicles)
                    '        End Using
                    '    Catch ex As Exception
                    '        SaveClaimControlVehiclesEx = ex
                    '        SendEmail("save veh EXCEPTION! " & fnol.Vehicles.Count, "", ex.ToString)
                    '    End Try

                    '    If createXmls = True Then
                    '        Dim SaveClaimControlVehiclesDv As Diamond.Common.Objects.DiamondValidation = Nothing
                    '        If rspAddVehicles IsNot Nothing Then
                    '            If rspAddVehicles.ResponseData IsNot Nothing Then
                    '                Dim responseDataPath As String = fileDir & "SaveClaimControlVehicles_ResponseData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    '                rspAddVehicles.ResponseData.DumpToFile(responseDataPath)
                    '            End If
                    '            If rspAddVehicles.DiamondValidation IsNot Nothing Then
                    '                SaveClaimControlVehiclesDv = rspAddVehicles.DiamondValidation
                    '            End If
                    '        End If
                    '        If (SaveClaimControlVehiclesDv Is Nothing OrElse SaveClaimControlVehiclesDv.ValidationItems Is Nothing OrElse SaveClaimControlVehiclesDv.ValidationItems.Count = 0) AndAlso SaveClaimControlVehiclesEx IsNot Nothing Then
                    '            SaveClaimControlVehiclesDv = New Diamond.Common.Objects.DiamondValidation
                    '            If SaveClaimControlVehiclesDv.ValidationItems Is Nothing Then
                    '                SaveClaimControlVehiclesDv.ValidationItems = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.ValidationItem)
                    '            End If
                    '            Dim vi As New Diamond.Common.Objects.ValidationItem(Diamond.Common.Objects.ValidationItemType.ValidationError, SaveClaimControlVehiclesEx.ToString)
                    '            SaveClaimControlVehiclesDv.ValidationItems.Add(vi)
                    '        End If
                    '        If SaveClaimControlVehiclesDv IsNot Nothing Then
                    '            Dim diamondValidationPath As String = fileDir & "SaveClaimControlVehicles_DiamondValidation" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                    '            SaveClaimControlVehiclesDv.DumpToFile(diamondValidationPath)
                    '        End If
                    '    End If

                ElseIf FNOLType = Enums.FNOL_LOB_Enum.Liability OrElse FNOLType = Enums.FNOL_LOB_Enum.Propertty Then
                    'begin property, liability
                    If fnol.Properties.Count > 0 Then
                        Dim reqProp As New DCS.Messages.ClaimsService.SaveClaimControlProperties.Request
                        Dim resProp As New DCS.Messages.ClaimsService.SaveClaimControlProperties.Response
                        Dim SaveClaimControlPropertiesEx As Exception = Nothing
                        Try
                            With reqProp.RequestData
                                .ClaimControlId = response.ResponseData.ClaimControlId

                                If .ClaimControlProperties Is Nothing Then
                                    .ClaimControlProperties = New DCO.InsCollection(Of DCO.Claims.ClaimControl.ClaimControlProperty)
                                End If

                                For i As Integer = 0 To fnol.Properties.Count - 1

                                    .ClaimControlProperties.Add(New DCO.Claims.ClaimControl.ClaimControlProperty)

                                    With .ClaimControlProperties(0) '5/15/2013 - will need to update to use i instead of 0 if we ever plan to set more than 1 property

                                        If .Location Is Nothing Then
                                            .Location = New DCO.Address
                                        End If

                                        .DamageDescription = fnol.Properties(i).DamageDescription
                                        .Location.HouseNumber = fnol.Properties(i).Location.HouseNumber
                                        .Location.StreetName = fnol.Properties(i).Location.StreetName
                                        .Location.City = fnol.Properties(i).Location.City

                                        If fnol.Properties(i).Location.ApartmentNumber <> "" Then
                                            .Location.ApartmentNumber = fnol.Properties(i).Location.ApartmentNumber
                                        End If

                                        If fnol.Properties(i).Location.POBox <> "" Then
                                            '.Location.ApartmentNumber = fnol.Properties(i).Location.POBox
                                            'updated 4/29/2013 to set correct property
                                            .Location.POBox = fnol.Properties(i).Location.POBox
                                        End If

                                        If fnol.Properties(i).Location.Other <> "" Then
                                            '.Location.ApartmentNumber = fnol.Properties(i).Location.Other
                                            'updated 4/29/2013 to set correct property
                                            .Location.Other = fnol.Properties(i).Location.Other
                                        End If

                                        .Location.StateId = fnol.Properties(i).Location.State
                                        .Location.Zip = fnol.Properties(i).Location.Zip
                                        .EstimatedAmount = fnol.Properties(i).EstimatedAmount

                                        'updated 4/29/2013 to use new property; will overwrite existing stuff
                                        If fnol.Properties(i).Address IsNot Nothing Then '8/10/2017 note: AddAddress method will only overwrite if something is set
                                            AddAddress(fnol.Properties(i).Address, .Location)
                                        End If
                                    End With

                                Next

                                If createXmls = True Then
                                    Dim requestDataPath As String = fileDir & "SaveClaimControlProperties_RequestData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                                    .DumpToFile(requestDataPath)
                                End If
                            End With

                            Using proxy As New Proxies.ClaimsServiceProxy
                                resProp = proxy.SaveClaimControlProperties(reqProp)
                            End Using

                        Catch ex As Exception
                            SaveClaimControlPropertiesEx = ex
                            SendEmail("save property EXCEPTION! " & fnol.Properties.Count, "", ex.ToString)
                        End Try

                        If createXmls = True Then
                            Dim SaveClaimControlPropertiesDv As Diamond.Common.Objects.DiamondValidation = Nothing
                            If resProp IsNot Nothing Then
                                If resProp.ResponseData IsNot Nothing Then
                                    Dim responseDataPath As String = fileDir & "SaveClaimControlProperties_ResponseData" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                                    resProp.ResponseData.DumpToFile(responseDataPath)
                                End If
                                If resProp.DiamondValidation IsNot Nothing Then
                                    SaveClaimControlPropertiesDv = resProp.DiamondValidation
                                End If
                            End If
                            If (SaveClaimControlPropertiesDv Is Nothing OrElse SaveClaimControlPropertiesDv.ValidationItems Is Nothing OrElse SaveClaimControlPropertiesDv.ValidationItems.Count = 0) AndAlso SaveClaimControlPropertiesEx IsNot Nothing Then
                                SaveClaimControlPropertiesDv = New Diamond.Common.Objects.DiamondValidation
                                If SaveClaimControlPropertiesDv.ValidationItems Is Nothing Then
                                    SaveClaimControlPropertiesDv.ValidationItems = New Diamond.Common.Objects.InsCollection(Of Diamond.Common.Objects.ValidationItem)
                                End If
                                Dim vi As New Diamond.Common.Objects.ValidationItem(Diamond.Common.Objects.ValidationItemType.ValidationError, SaveClaimControlPropertiesEx.ToString)
                                SaveClaimControlPropertiesDv.ValidationItems.Add(vi)
                            End If
                            If SaveClaimControlPropertiesDv IsNot Nothing Then
                                Dim diamondValidationPath As String = fileDir & "SaveClaimControlProperties_DiamondValidation" & xmlFileAddInfo & "_" & Date.Now.ToString("s").Replace(":", "").Replace("-", "").Replace("/", "").Replace(" ", "") & ".xml" 'Date.Now.ToString("s") example = 2010-05-18T16:47:55 (20100518T164755)
                                SaveClaimControlPropertiesDv.DumpToFile(diamondValidationPath)
                            End If
                        End If
                    End If

                Else
                    SendEmail("fnol NO LOSS TYPE! ", "", "type " & fnol.ClaimType & " veh " & fnol.Vehicles.Count & " prop " & fnol.Properties.Count)
                End If

            Else
                For Each vItem As DCO.ValidationItem In response.DiamondValidation.ValidationItems
                    Errors.Add(vItem.Message)
                    SendEmail("ValidationItem", "", vItem.Message)
                Next
            End If

        Catch ex As Exception
            Errors.Add(ex.ToString)
        End Try
    End Sub

    'Private Sub AddPersonName(ByVal person As DiamondClaims_FNOL_Person, ByRef diaName As DCO.Name) 'added 4/26/2013
    '    If person.FirstName <> "" OrElse person.LastName <> "" OrElse person.CommercialName <> "" Then
    '        With diaName
    '            .FirstName = person.FirstName
    '            .MiddleName = person.MiddleName
    '            .LastName = person.LastName
    '            .CommercialName1 = person.CommercialName
    '            .DoingBusinessAs = person.DbaName
    '        End With
    '    End If
    'End Sub
    'updated 4/29/2013 to use separated objects
    Private Sub AddName(ByVal name As DiamondClaims_FNOL_Name, ByRef diaName As DCO.Name) 'added 4/26/2013
        If name IsNot Nothing Then
            If name.FirstName <> "" OrElse name.LastName <> "" OrElse name.CommercialName <> "" Then
                If diaName Is Nothing Then '6/29/2016: added logic to instantiate if needed
                    diaName = New DCO.Name
                End If
                With diaName
                    .FirstName = name.FirstName
                    .MiddleName = name.MiddleName
                    .LastName = name.LastName
                    .CommercialName1 = name.CommercialName
                    .DoingBusinessAs = name.DbaName

                    'added 7/27/2017
                    If String.IsNullOrWhiteSpace(.LastName) = True AndAlso (String.IsNullOrWhiteSpace(.CommercialName1) = False OrElse String.IsNullOrWhiteSpace(.DoingBusinessAs) = False) Then
                        .TypeId = 2 'Commercial
                        .CommercialName1 = .CommercialName1 'added 8/10/2017 to force rebuild of displayName/sortName
                    Else
                        .TypeId = 1 'Personal Name
                        .LastName = .LastName 'added 8/10/2017 to force rebuild of displayName/sortName; should already be calculated for personal names, but added just in case
                    End If
                End With
            End If
        End If
    End Sub
    'Private Sub AddPersonAddress(ByVal person As DiamondClaims_FNOL_Person, ByRef diaAddress As DCO.Address) 'added 4/26/2013
    '    If person.HouseNumber <> "" OrElse person.StreetName <> "" OrElse person.City <> "" OrElse (person.StateId <> "" AndAlso IsNumeric(person.StateId) = True) OrElse person.ZipCode <> "" OrElse person.County <> "" OrElse person.AddressOther <> "" OrElse person.PoBox <> "" OrElse person.ApartmentNumber <> "" Then
    '        With diaAddress
    '            .HouseNumber = person.HouseNumber
    '            .StreetName = person.StreetName
    '            .City = person.City
    '            If person.StateId <> "" AndAlso IsNumeric(person.StateId) = True Then
    '                .StateId = person.StateId
    '            End If
    '            .Zip = person.ZipCode
    '            .County = person.County
    '            .Other = person.AddressOther
    '            .POBox = person.PoBox
    '            .ApartmentNumber = person.ApartmentNumber
    '        End With
    '    End If
    'End Sub
    'updated 4/29/2013 to use separated objects
    Private Sub AddAddress(ByVal address As DiamondClaims_FNOL_Address, ByRef diaAddress As DCO.Address) 'added 4/26/2013
        If address IsNot Nothing Then
            If address.HouseNumber <> "" OrElse address.StreetName <> "" OrElse address.City <> "" OrElse (address.StateId <> "" AndAlso IsNumeric(address.StateId) = True) OrElse address.ZipCode <> "" OrElse address.County <> "" OrElse address.AddressOther <> "" OrElse address.PoBox <> "" OrElse address.ApartmentNumber <> "" Then
                If diaAddress Is Nothing Then '6/29/2016: added logic to instantiate if needed
                    diaAddress = New DCO.Address
                End If
                With diaAddress
                    .HouseNumber = address.HouseNumber
                    .StreetName = address.StreetName
                    .City = address.City
                    If address.StateId <> "" AndAlso IsNumeric(address.StateId) = True Then
                        .StateId = address.StateId
                    End If
                    .Zip = address.ZipCode
                    .County = address.County
                    .Other = address.AddressOther
                    .POBox = address.PoBox
                    .ApartmentNumber = address.ApartmentNumber
                End With
            End If
        End If
    End Sub
    'Private Sub AddPersonPhones(ByVal person As DiamondClaims_FNOL_Person, ByRef diaPhones As DCO.InsCollection(Of DCO.Phone)) 'added 4/26/2013
    '    If person.HomePhone <> "" Then
    '        Dim p As New DCO.Phone
    '        With p
    '            '.TypeId = 1 (home)
    '            .TypeId = Diamond.Common.Enums.PhoneType.Home
    '            .Number = person.HomePhone
    '        End With
    '        diaPhones.Add(p)
    '    End If
    '    If person.BusinessPhone <> "" Then
    '        Dim p As New DCO.Phone
    '        With p
    '            '.TypeId = 2 (business)
    '            .TypeId = Diamond.Common.Enums.PhoneType.Business
    '            .Number = person.BusinessPhone
    '        End With
    '        diaPhones.Add(p)
    '    End If
    '    If person.CellPhone <> "" Then
    '        Dim p As New DCO.Phone
    '        With p
    '            '.TypeId = 4 (cell)
    '            .TypeId = Diamond.Common.Enums.PhoneType.Cellular
    '            .Number = person.CellPhone
    '        End With
    '        diaPhones.Add(p)
    '    End If
    '    If person.FaxPhone <> "" Then
    '        Dim p As New DCO.Phone
    '        With p
    '            '.TypeId = 3 (fax)
    '            .TypeId = Diamond.Common.Enums.PhoneType.Fax
    '            .Number = person.FaxPhone
    '        End With
    '    End If
    'End Sub
    'updated 4/29/2013 to use separated objects
    Private Sub AddPhones(ByVal contactInfo As DiamondClaims_FNOL_ContactInfo, ByRef diaPhones As DCO.InsCollection(Of DCO.Phone)) 'added 4/26/2013
        If contactInfo IsNot Nothing Then
            If contactInfo.HomePhone <> "" Then
                Dim p As New DCO.Phone
                With p
                    '.TypeId = 1 (home)
                    .TypeId = Diamond.Common.Enums.PhoneType.Home
                    .Number = contactInfo.HomePhone
                End With
                If diaPhones Is Nothing Then '6/29/2016: added logic to instantiate if needed
                    diaPhones = New DCO.InsCollection(Of DCO.Phone)
                End If
                diaPhones.Add(p)
            End If
            If contactInfo.BusinessPhone <> "" Then
                Dim p As New DCO.Phone
                With p
                    '.TypeId = 2 (business)
                    .TypeId = Diamond.Common.Enums.PhoneType.Business
                    .Number = contactInfo.BusinessPhone
                End With
                If diaPhones Is Nothing Then '6/29/2016: added logic to instantiate if needed
                    diaPhones = New DCO.InsCollection(Of DCO.Phone)
                End If
                diaPhones.Add(p)
            End If
            If contactInfo.CellPhone <> "" Then
                Dim p As New DCO.Phone
                With p
                    '.TypeId = 4 (cell)
                    .TypeId = Diamond.Common.Enums.PhoneType.Cellular
                    .Number = contactInfo.CellPhone
                End With
                If diaPhones Is Nothing Then '6/29/2016: added logic to instantiate if needed
                    diaPhones = New DCO.InsCollection(Of DCO.Phone)
                End If
                diaPhones.Add(p)
            End If
            If contactInfo.FaxPhone <> "" Then
                Dim p As New DCO.Phone
                With p
                    '.TypeId = 3 (fax)
                    .TypeId = Diamond.Common.Enums.PhoneType.Fax
                    .Number = contactInfo.FaxPhone
                End With
                If diaPhones Is Nothing Then '6/29/2016: added logic to instantiate if needed
                    diaPhones = New DCO.InsCollection(Of DCO.Phone)
                End If
                diaPhones.Add(p) '6/29/2016: added code to include this # in list if provided; must not have noticed this line was missing before
            End If
        End If
    End Sub
    'Private Sub AddPersonEmails(ByVal person As DiamondClaims_FNOL_Person, ByRef diaEmails As DCO.InsCollection(Of DCO.Email)) 'added 4/26/2013
    '    If person.OtherEmail <> "" Then
    '        Dim e As New DCO.Email
    '        With e
    '            .TypeId = Diamond.Common.Enums.EMailType.Other
    '            .Address = person.OtherEmail
    '        End With
    '        diaEmails.Add(e)
    '    End If
    '    If person.HomeEmail <> "" Then
    '        Dim e As New DCO.Email
    '        With e
    '            .TypeId = Diamond.Common.Enums.EMailType.Home
    '            .Address = person.HomeEmail
    '        End With
    '        diaEmails.Add(e)
    '    End If
    '    If person.BusinessEmail <> "" Then
    '        Dim e As New DCO.Email
    '        With e
    '            .TypeId = Diamond.Common.Enums.EMailType.Business
    '            .Address = person.BusinessEmail
    '        End With
    '        diaEmails.Add(e)
    '    End If
    'End Sub
    'updated 4/29/2013 to use separated objects
    Private Sub AddEmails(ByVal contactInfo As DiamondClaims_FNOL_ContactInfo, ByRef diaEmails As DCO.InsCollection(Of DCO.Email)) 'added 4/26/2013
        If contactInfo IsNot Nothing Then
            If contactInfo.OtherEmail <> "" Then
                Dim e As New DCO.Email
                With e
                    .TypeId = Diamond.Common.Enums.EMailType.Other
                    .Address = contactInfo.OtherEmail
                End With
                If diaEmails Is Nothing Then '6/29/2016: added logic to instantiate if needed
                    diaEmails = New DCO.InsCollection(Of DCO.Email)
                End If
                diaEmails.Add(e)
            End If
            If contactInfo.HomeEmail <> "" Then
                Dim e As New DCO.Email
                With e
                    .TypeId = Diamond.Common.Enums.EMailType.Home
                    .Address = contactInfo.HomeEmail
                End With
                If diaEmails Is Nothing Then '6/29/2016: added logic to instantiate if needed
                    diaEmails = New DCO.InsCollection(Of DCO.Email)
                End If
                diaEmails.Add(e)
            End If
            If contactInfo.BusinessEmail <> "" Then
                Dim e As New DCO.Email
                With e
                    .TypeId = Diamond.Common.Enums.EMailType.Business
                    .Address = contactInfo.BusinessEmail
                End With
                If diaEmails Is Nothing Then '6/29/2016: added logic to instantiate if needed
                    diaEmails = New DCO.InsCollection(Of DCO.Email)
                End If
                diaEmails.Add(e)
            End If
        End If
    End Sub
    'added 4/29/2013 to set all thru one Sub
    Private Sub AddPerson(ByVal person As DiamondClaims_FNOL_Person, ByRef diaName As DCO.Name, ByRef diaAddress As DCO.Address, ByRef diaPhones As DCO.InsCollection(Of DCO.Phone), ByRef diaEmails As DCO.InsCollection(Of DCO.Email)) 'added 4/26/2013
        If person IsNot Nothing Then
            With person
                AddName(.Name, diaName)
                AddAddress(.Address, diaAddress)
                AddPhones(.ContactInfo, diaPhones)
                AddEmails(.ContactInfo, diaEmails)
            End With
        End If
    End Sub

    Public Property Errors() As List(Of String)
        Get
            Return _Errors
        End Get
        Set(ByVal value As List(Of String))
            _Errors = value
        End Set
    End Property

    Public Sub SendEmail(ByVal subject As String, ByVal polnum As String, ByVal bodyString As String)
        Using em As New EmailObject
            em.MailHost = System.Configuration.ConfigurationManager.AppSettings("mailhost")
            em.EmailSubject = subject & " " & polnum
            em.EmailFromAddress = "diamondClaims@IndianaFarmers.com"
            em.EmailToAddress = System.Configuration.ConfigurationManager.AppSettings("diamondWebClassTO")
            em.EmailBody = bodyString
            em.SendEmail()
        End Using
    End Sub

    'added 6/29/2016
    Public Enum Insured1orInsured2
        Any = 0
        Insured1 = 1
        Insured2 = 2
        UnknownOnly = 3
    End Enum
    Public Enum FirstOrLast
        First = 0
        Last = 1
    End Enum
    Public Function GetInsuredThirdPartyFromList(ByRef diaParties As DCO.InsCollection(Of DCO.Claims.LossNotice.ThirdParty), Optional ByVal insured1or2 As Insured1orInsured2 = Insured1orInsured2.Any, Optional ByVal returnNewAndAddToListIf1or2AndNotFound As Boolean = True, Optional ByVal firstOrLastItem As FirstOrLast = FirstOrLast.First) As DCO.Claims.LossNotice.ThirdParty
        Dim insParty As DCO.Claims.LossNotice.ThirdParty = Nothing

        'Dim insPartyCount As Integer = 0 'removed since it's now an optional byref param
        Dim allCount As Integer = 0
        Dim unknownCount As Integer = 0
        Dim insured1Count As Integer = 0
        Dim insured2Count As Integer = 0
        'Dim insParties As DCO.InsCollection(Of DCO.Claims.LossNotice.ThirdParty) = GetInsuredThirdPartiesFromList(diaParties, Insured1orInsured2.Any, allCount, unknownCount, insured1Count, insured2Count) 'sending any here to return all; will filter later
        'If insParties IsNot Nothing AndAlso insParties.Count > 0 Then
        '    'insPartyCount = insParties.Count 'removed since it's now an optional byref param

        'End If
        'updated to 1st check for specific and then generic if needed
        Dim insParties As DCO.InsCollection(Of DCO.Claims.LossNotice.ThirdParty) = GetInsuredThirdPartiesFromList(diaParties, insured1or2, allCount, unknownCount, insured1Count, insured2Count) 'sending any here to return all; will filter later
        If insParties IsNot Nothing AndAlso insParties.Count > 0 Then
            'something specific found
            If firstOrLastItem = FirstOrLast.Last AndAlso insParties.Count > 1 Then
                insParty = insParties.Item(insParties.Count - 1)
            Else
                insParty = insParties.Item(0)
            End If
        Else
            'check for generic
            If allCount > 0 AndAlso (insured1or2 = Insured1orInsured2.Insured1 OrElse insured1or2 = Insured1orInsured2.Insured2) AndAlso unknownCount > 0 Then 'only need to check for generic in list if there are some to start with
                'have to assume insured1 will be used before insured2 and you'll never have insured2 w/o insured1
                If insured1or2 = Insured1orInsured2.Insured2 Then
                    'insured2
                    If insured1Count > 0 Then
                        'can take 1st unknown since insured1 is already set
                        insParties = GetInsuredThirdPartiesFromList(diaParties, Insured1orInsured2.UnknownOnly)
                        If insParties IsNot Nothing AndAlso insParties.Count > 0 Then 'should work based on above logic
                            insParty = insParties.Item(0)
                            'now update to specific
                            insParty.Insured2 = True
                            insParty.Insured1 = False 'prob not needed
                            'updated 7/5/2016 for RelationshipType
                            insParty.RelationshipId = 5 'Policyholder #2
                        End If
                    ElseIf insured1Count = 0 And unknownCount > 1 Then
                        'should take 2nd or last unknown since 1st unknown should be insured1
                        insParties = GetInsuredThirdPartiesFromList(diaParties, Insured1orInsured2.UnknownOnly)
                        If insParties IsNot Nothing AndAlso insParties.Count > 1 Then 'should work based on above logic
                            insParty = insParties.Item(1) 'getting 2nd unknown, but could also use last, insParties.Item(insParties.Count - 1)
                            'now update to specific
                            insParty.Insured2 = True
                            insParty.Insured1 = False 'prob not needed
                            'updated 7/5/2016 for RelationshipType
                            insParty.RelationshipId = 5 'Policyholder #2
                        End If
                    End If
                Else
                    'insured1
                    'should be able to take 1st unknown regardless of insured2 already being there or not, though insured2 shouldn't be there... or shouldn't be used until after insured1
                    insParties = GetInsuredThirdPartiesFromList(diaParties, Insured1orInsured2.UnknownOnly)
                    If insParties IsNot Nothing AndAlso insParties.Count > 0 Then 'should work based on above logic
                        insParty = insParties.Item(0)
                        'now update to specific
                        insParty.Insured1 = True
                        insParty.Insured2 = False 'prob not needed
                        'updated 7/5/2016 for RelationshipType
                        insParty.RelationshipId = 8 'Policyholder
                    End If
                End If
            End If
        End If

        If returnNewAndAddToListIf1or2AndNotFound = True AndAlso insParty Is Nothing AndAlso (insured1or2 = Insured1orInsured2.Insured1 OrElse insured1or2 = Insured1orInsured2.Insured2) Then
            insParty = New Diamond.Common.Objects.Claims.LossNotice.ThirdParty
            'insParty.ClaimantTypeId = 1 'Named Insured
            'updated 7/27/2016 to use new function
            insParty.ClaimantTypeId = insuredClaimantTypeId() 'Named Insured
            If insured1or2 = Insured1orInsured2.Insured2 Then
                insParty.Insured2 = True
                insParty.Insured1 = False 'prob not needed
                'updated 7/5/2016 for RelationshipType
                insParty.RelationshipId = 5 'Policyholder #2
            Else
                insParty.Insured1 = True
                insParty.Insured2 = False 'prob not needed
                'updated 7/5/2016 for RelationshipType
                insParty.RelationshipId = 8 'Policyholder
            End If
            If diaParties Is Nothing Then
                diaParties = New DCO.InsCollection(Of DCO.Claims.LossNotice.ThirdParty)
            End If
            diaParties.Add(insParty)
        End If

        Return insParty
    End Function
    Public Function GetInsuredThirdPartiesFromList(ByRef diaParties As DCO.InsCollection(Of DCO.Claims.LossNotice.ThirdParty), Optional ByVal insured1or2 As Insured1orInsured2 = Insured1orInsured2.Any, Optional ByRef allCount As Integer = 0, Optional ByRef unknownCount As Integer = 0, Optional ByRef insured1Count As Integer = 0, Optional ByRef insured2Count As Integer = 0) As DCO.InsCollection(Of DCO.Claims.LossNotice.ThirdParty)
        Dim insParties As DCO.InsCollection(Of DCO.Claims.LossNotice.ThirdParty) = Nothing
        allCount = 0
        unknownCount = 0
        insured1Count = 0
        insured2Count = 0

        If diaParties IsNot Nothing AndAlso diaParties.Count > 0 Then
            For Each diaParty As DCO.Claims.LossNotice.ThirdParty In diaParties
                'If diaParty IsNot Nothing AndAlso diaParty.ClaimantTypeId = 1 Then 'Named Insured
                'updated 7/27/2016 to use new function
                If diaParty IsNot Nothing AndAlso IsInsuredClaimantTypeId(diaParty.ClaimantTypeId) = True Then 'Named Insured
                    Dim okayToInclude As Boolean = False
                    'Select Case insured1or2
                    '    Case Insured1orInsured2.Insured1
                    '        If diaParty.Insured1 = True Then
                    '            okayToInclude = True
                    '        End If
                    '    Case Insured1orInsured2.Insured2
                    '        If diaParty.Insured2 = True Then
                    '            okayToInclude = True
                    '        End If
                    '    Case Else 'Insured1orInsured2.Any
                    '        okayToInclude = True
                    'End Select
                    'updated to not consider insured1 or insured2 unless only 1 of them is checked
                    allCount += 1
                    'If (diaParty.Insured1 = False AndAlso diaParty.Insured2 = False) OrElse (diaParty.Insured1 = True AndAlso diaParty.Insured2 = True) Then
                    '    unknownCount += 1
                    'ElseIf diaParty.Insured1 = True Then
                    '    insured1Count += 1
                    'ElseIf diaParty.Insured2 = True Then
                    '    insured2Count += 1
                    'End If
                    're-written to make logic a little cleaner
                    If insured1or2 = Insured1orInsured2.Any Then
                        okayToInclude = True
                    End If
                    Select Case diaParty.RelationshipId'7/5/2016 - added logic to check RelationshipId 1st; original logic in ELSE
                        Case 8 'Policyholder
                            insured1Count += 1
                            diaParty.Insured1 = True
                            diaParty.Insured2 = False
                            If insured1or2 = Insured1orInsured2.Insured1 Then
                                okayToInclude = True
                            End If
                        Case 5 'Policyholder #2
                            insured2Count += 1
                            diaParty.Insured2 = True
                            diaParty.Insured1 = False
                            If insured1or2 = Insured1orInsured2.Insured2 Then
                                okayToInclude = True
                            End If
                        Case Else 'could have any RelationshipType; shouldn't have ClaimantType of Insured unless policyholder 1 or 2 though
                            If diaParty.Insured1 = True AndAlso diaParty.Insured2 = False Then
                                insured1Count += 1
                                If insured1or2 = Insured1orInsured2.Insured1 Then
                                    okayToInclude = True
                                End If
                                'updated 7/5/2016 for RelationshipType
                                diaParty.RelationshipId = 8 'Policyholder
                            ElseIf diaParty.Insured2 = True AndAlso diaParty.Insured1 = False Then
                                insured2Count += 1
                                If insured1or2 = Insured1orInsured2.Insured2 Then
                                    okayToInclude = True
                                End If
                                'updated 7/5/2016 for RelationshipType
                                diaParty.RelationshipId = 5 'Policyholder #2
                            Else
                                unknownCount += 1
                                If insured1or2 = Insured1orInsured2.UnknownOnly Then
                                    okayToInclude = True
                                End If
                            End If
                    End Select
                    If okayToInclude = True Then
                        If insParties Is Nothing Then
                            insParties = New DCO.InsCollection(Of DCO.Claims.LossNotice.ThirdParty)
                        End If
                        insParties.Add(diaParty)
                    End If
                End If
            Next
        End If

        Return insParties
    End Function
    'added 7/27/2016... to correct duplication of Insured Claimants since InsureSoft backend logic appears to automatically add them in 531; removed 7/28/2016 in leiu of DiamondClaimsFNOL_OmitInsuredClaimants to change default behavior back to adding them from our code
    'Public Function DiamondClaimsFNOL_OkayToAddInsuredClaimants() As Boolean
    '    Dim okay As Boolean = False

    '    If ConfigurationManager.AppSettings("DiamondClaimsFNOL_OkayToAddInsuredClaimants") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings("DiamondClaimsFNOL_OkayToAddInsuredClaimants").ToString) = False AndAlso UCase(ConfigurationManager.AppSettings("DiamondClaimsFNOL_OkayToAddInsuredClaimants").ToString) = "YES" Then
    '        okay = True
    '    End If

    '    Return okay
    'End Function
    Public Function IsInsuredClaimant(ByVal claimant As DiamondClaims_FNOL_Claimant) As Boolean
        Dim isInsured As Boolean = False

        If claimant IsNot Nothing AndAlso IsInsuredClaimantTypeId(claimant.claimantTypeID) = True Then
            isInsured = True
        End If

        Return isInsured
    End Function
    Public Function IsInsuredThirdParty(ByVal thirdParty As DCO.Claims.LossNotice.ThirdParty) As Boolean
        Dim isInsured As Boolean = False

        If thirdParty IsNot Nothing AndAlso IsInsuredClaimantTypeId(thirdParty.ClaimantTypeId) = True Then
            isInsured = True
        End If

        Return isInsured
    End Function
    Public Function IsInsuredClaimantTypeId(ByVal typeId As Integer) As Boolean
        Dim isInsured As Boolean = False

        If typeId = insuredClaimantTypeId() Then
            isInsured = True
        End If

        Return isInsured
    End Function
    Public Function insuredClaimantTypeId() As Integer
        Return 1 'see Diamond's ClaimantType table
    End Function
    Public Function HasNameInfo(ByVal name As DiamondClaims_FNOL_Name) As Boolean
        Dim hasInfo As Boolean = False

        If name IsNot Nothing Then
            With name
                If String.IsNullOrEmpty(.FirstName) = False OrElse String.IsNullOrEmpty(.LastName) = False OrElse String.IsNullOrEmpty(.CommercialName) = False Then
                    hasInfo = True
                End If
            End With
        End If

        Return hasInfo
    End Function
    'added 8/10/2017
    Public Function HasAddressInfo(ByVal addr As DiamondClaims_FNOL_Address, Optional ByVal poBoxQualifies As Boolean = True, Optional ByVal stateQualifies As Boolean = True, Optional ByVal zipQualifies As Boolean = True) As Boolean
        Dim hasInfo As Boolean = False

        If addr IsNot Nothing Then
            With addr
                If String.IsNullOrEmpty(.HouseNumber) = False OrElse String.IsNullOrEmpty(.StreetName) = False OrElse (poBoxQualifies = True AndAlso String.IsNullOrEmpty(.PoBox) = False) OrElse String.IsNullOrEmpty(.City) = False OrElse (stateQualifies = True AndAlso String.IsNullOrEmpty(.StateId) = False AndAlso IsNumeric(.StateId) = True) OrElse (zipQualifies = True AndAlso String.IsNullOrEmpty(.ZipCode) = False) Then
                    hasInfo = True
                End If
            End With
        End If

        Return hasInfo
    End Function
    Public Function HasFullAddressInfo(ByVal addr As DiamondClaims_FNOL_Address, Optional ByVal poBoxQualifies As Boolean = True) As Boolean
        Dim hasInfo As Boolean = False

        If addr IsNot Nothing Then
            With addr
                If ((String.IsNullOrEmpty(.HouseNumber) = False AndAlso String.IsNullOrEmpty(.StreetName) = False) OrElse (poBoxQualifies = True AndAlso String.IsNullOrEmpty(.PoBox) = False)) AndAlso String.IsNullOrEmpty(.City) = False AndAlso String.IsNullOrEmpty(.StateId) = False AndAlso IsNumeric(.StateId) = True AndAlso String.IsNullOrEmpty(.ZipCode) = False Then
                    hasInfo = True
                End If
            End With
        End If

        Return hasInfo

    End Function
    'added 7/28/2016; replaces DiamondClaimsFNOL_OkayToAddInsuredClaimants so default behavior (if key is missing) would be to add them from our code
    Public Function DiamondClaimsFNOL_OmitInsuredClaimants() As Boolean
        Dim omit As Boolean = False

        If ConfigurationManager.AppSettings("DiamondClaimsFNOL_OmitInsuredClaimants") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings("DiamondClaimsFNOL_OmitInsuredClaimants").ToString) = False AndAlso UCase(ConfigurationManager.AppSettings("DiamondClaimsFNOL_OmitInsuredClaimants").ToString) = "YES" Then
            omit = True
        End If

        Return omit
    End Function

    Public Function IntegerForString(ByVal str As String) As Integer 'added 3/11/2017; logic copied from QuickQuote
        If str IsNot Nothing AndAlso str <> "" AndAlso IsNumeric(str) = True Then
            Return CInt(str)
        Else
            Return 0
        End If
    End Function

    'added 7/29/2017
    'Public Sub LoadClaimLossTypeDropdownForPolicyAndFnolType(ByRef ddl As Web.UI.WebControls.DropDownList, ByVal polNum As String, ByVal fnolType As Enums.FNOL_Type)
    'updated w/ optional params for packagePartType, sortList, and loaded
    Public Sub LoadClaimLossTypeDropdownForPolicyAndFnolType(ByRef ddl As Web.UI.WebControls.DropDownList, ByVal polNum As String, ByVal fnolType As Enums.FNOL_Type, Optional ByVal packagePartType As String = "", Optional ByVal sortList As Boolean = True, Optional ByRef loaded As Boolean = False)
        loaded = False

        Dim lobType As Enums.PolicyLobType = LobTypeForPolicyNumber(polNum)

        If lobType = Enums.PolicyLobType.CommercialPackage AndAlso String.IsNullOrWhiteSpace(packagePartType) = False Then
            lobType = LobTypeForPackagePartType(packagePartType, defaultToCPP:=True)
        End If

        If lobType = Enums.PolicyLobType.CommercialPackage Then
            Select Case fnolType
                Case Enums.FNOL_Type.AutoFNOL
                    lobType = Enums.PolicyLobType.CommercialGarage
                Case Enums.FNOL_Type.LiabilityFNOL
                    lobType = Enums.PolicyLobType.CommercialGeneralLiability
                Case Enums.FNOL_Type.PropertyFNOL
                    lobType = Enums.PolicyLobType.CommercialProperty
            End Select
        End If

        Dim enumType As Type = Nothing
        Select Case lobType
            Case Enums.PolicyLobType.AutoPersonal
                enumType = GetType(Enums.ClaimLossType_PPA)
            Case Enums.PolicyLobType.CommercialAuto
                enumType = GetType(Enums.ClaimLossType_CAP)
            Case Enums.PolicyLobType.CommercialBOP
                enumType = GetType(Enums.ClaimLossType_BOP)
            Case Enums.PolicyLobType.CommercialCrime
                enumType = GetType(Enums.ClaimLossType_CRM)
            Case Enums.PolicyLobType.CommercialGarage
                enumType = GetType(Enums.ClaimLossType_GAR)
            Case Enums.PolicyLobType.CommercialGeneralLiability
                enumType = GetType(Enums.ClaimLossType_CGL)
            Case Enums.PolicyLobType.CommercialInlandMarine
                enumType = GetType(Enums.ClaimLossType_CIM)
            'Case Enums.PolicyLobType.CommercialPackage

            Case Enums.PolicyLobType.CommercialProperty
                enumType = GetType(Enums.ClaimLossType_CPR)
            Case Enums.PolicyLobType.CommercialUmbrella
                enumType = GetType(Enums.ClaimLossType_CUP)
            Case Enums.PolicyLobType.DwellingFirePersonal
                enumType = GetType(Enums.ClaimLossType_DFR)
            Case Enums.PolicyLobType.Farm
                enumType = GetType(Enums.ClaimLossType_FAR)
            Case Enums.PolicyLobType.HomePersonal
                enumType = GetType(Enums.ClaimLossType_HOM)
            Case Enums.PolicyLobType.InlandMarinePersonal
                enumType = GetType(Enums.ClaimLossType_PIM)
            'Case Enums.PolicyLobType.NotAssigned

            Case Enums.PolicyLobType.UmbrellaPersonal
                enumType = GetType(Enums.ClaimLossType_PUP)
            Case Enums.PolicyLobType.WorkersComp
                enumType = GetType(Enums.ClaimLossType_WCP)
        End Select

        If enumType Is Nothing Then
            Select Case fnolType
                Case Enums.FNOL_Type.AutoFNOL
                    enumType = GetType(Enums.ClaimLossType_Auto)
                Case Enums.FNOL_Type.LiabilityFNOL
                    enumType = GetType(Enums.ClaimLossType_Liability)
                Case Enums.FNOL_Type.PropertyFNOL
                    enumType = GetType(Enums.ClaimLossType_Property)
                Case Else
                    enumType = GetType(Enums.ClaimLossType)
            End Select
        End If

        Dim vals As Array = Nothing
        Dim nms As Array = Nothing

        'vals = System.Enum.GetValues(enumType)
        'nms = System.Enum.GetNames(enumType)
        vals = [Enum].GetValues(enumType)
        nms = [Enum].GetNames(enumType)

        If vals IsNot Nothing AndAlso nms IsNot Nothing AndAlso vals.Length > 0 AndAlso vals.Length = nms.Length Then
            If ddl Is Nothing Then
                ddl = New Web.UI.WebControls.DropDownList
            End If

            If ddl.Items IsNot Nothing AndAlso ddl.Items.Count > 0 Then
                ddl.Items.Clear()
            End If

            ddl.Items.Add(New Web.UI.WebControls.ListItem)

            Dim index As Integer = 0
            For Each nm As String In nms
                Dim val As String = vals(index)
                ddl.Items.Add(New Web.UI.WebControls.ListItem(FormatEnumName(nm), val))
                index += 1
            Next

            If sortList = True Then
                SortDropDown(ddl)
            End If

            loaded = True
        End If
    End Sub
    'added 7/31/2017
    Public Function LobTypeForPolicyNumber(ByVal polNum As String) As Enums.PolicyLobType
        Dim lobType As Enums.PolicyLobType = Enums.PolicyLobType.NotAssigned

        If String.IsNullOrWhiteSpace(polNum) = False Then
            Dim polNumToUse As String = UCase(polNum).Replace("Q", "")
            If Len(polNumToUse) >= 3 Then
                Select Case Left(polNumToUse, 3)
                    Case "PPA"
                        lobType = Enums.PolicyLobType.AutoPersonal
                    Case "CAP"
                        lobType = Enums.PolicyLobType.CommercialAuto
                    Case "BOP"
                        lobType = Enums.PolicyLobType.CommercialBOP
                    Case "CRM"
                        lobType = Enums.PolicyLobType.CommercialCrime
                    Case "GAR"
                        lobType = Enums.PolicyLobType.CommercialGarage
                    Case "CGL"
                        lobType = Enums.PolicyLobType.CommercialGeneralLiability
                    Case "CIM"
                        lobType = Enums.PolicyLobType.CommercialInlandMarine
                    Case "CPP"
                        lobType = Enums.PolicyLobType.CommercialPackage
                    Case "CPR"
                        lobType = Enums.PolicyLobType.CommercialProperty
                    Case "CUP"
                        lobType = Enums.PolicyLobType.CommercialUmbrella
                    Case "DFR"
                        lobType = Enums.PolicyLobType.DwellingFirePersonal
                    Case "FAR"
                        lobType = Enums.PolicyLobType.Farm
                    Case "HOM"
                        lobType = Enums.PolicyLobType.HomePersonal
                    Case "PIM"
                        lobType = Enums.PolicyLobType.InlandMarinePersonal
                    Case "PUP", "FUP"
                        lobType = Enums.PolicyLobType.UmbrellaPersonal
                    Case "WCP"
                        lobType = Enums.PolicyLobType.WorkersComp
                End Select
            End If
        End If

        Return lobType
    End Function
    'added 8/2/2017
    Public Function LobTypeForPackagePartType(ByVal packagePartType As String, Optional ByVal defaultToCPP As Boolean = True) As Enums.PolicyLobType
        Dim lobType As Enums.PolicyLobType = Enums.PolicyLobType.NotAssigned

        If defaultToCPP = True Then
            lobType = Enums.PolicyLobType.CommercialPackage
        End If

        If String.IsNullOrWhiteSpace(packagePartType) = False Then
            Select Case UCase(packagePartType)
                Case "PROPERTY"
                    lobType = Enums.PolicyLobType.CommercialProperty
                Case "GENERAL LIABILITY"
                    lobType = Enums.PolicyLobType.CommercialGeneralLiability
                Case "INLAND MARINE"
                    lobType = Enums.PolicyLobType.CommercialInlandMarine
                Case "CRIME"
                    lobType = Enums.PolicyLobType.CommercialCrime
                Case "GARAGE"
                    lobType = Enums.PolicyLobType.CommercialGarage
            End Select
        End If

        Return lobType
    End Function
    'added 8/3/2017; taken from TS_FNOL.aspx.vb
    Public Function FormatEnumName(ByVal nm As String) As String
        Const CUPPER As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim NewName As String = ""
        Dim upcount As Integer = 0

        Try
            ' Split the string on every uppercase letter in the name
            For i As Integer = 0 To nm.Length - 1
                Dim s As String = nm.Substring(i, 1) ' get a char
                If CUPPER.Contains(s) And (i > 0) Then
                    'updated 8/5/2017 to pick up last character when upper-case
                    'If CUPPER.Contains(s) AndAlso (i > 0) AndAlso (i < nm.Length - 1) Then 'changed back to original IF after adding Else below when upper and last char
                    upcount += 1
                    If upcount = 1 Then 'added IF 8/5/2017; original logic in ELSE... should always be able to add a space before the 1st uCase char since it isn't the 1st char in the string
                        NewName &= " " & s
                    Else
                        ' Look ahead, if the NEXT character is uppercase, do NOT insert a space
                        If i + 1 <= nm.Length - 1 Then
                            If CUPPER.Contains(nm.Substring(i + 1, 1)) Then
                                'NewName += s
                                'updated 8/5/2017 to use & instead of +
                                NewName &= s
                            Else
                                'If upcount > 1 Then
                                '    NewName += s & " " '8/5/2017 note: this doesn't make sense
                                'Else
                                '    NewName += " " & s
                                'End If
                                'updated 8/5/2017; shouldn't ever need to add space after current char
                                NewName &= " " & s
                                upcount = 0
                            End If
                        Else 'added 8/5/2017 to pick up last character after changing back to original upper IF that only makes sure it's not 1st char in string
                            NewName &= s
                        End If
                    End If
                Else
                    'NewName += s
                    'updated 8/5/2017 to use & instead of +
                    NewName &= s
                    upcount = 0
                End If
            Next i

            Return NewName
        Catch ex As Exception
            'HandleError(ClassName, "FormatEnumName", ex, Me, txtPolnum.Text, lblMsg)
            Return nm
        End Try
    End Function

    Private Sub SortDropDown(ByRef ddl As Web.UI.WebControls.DropDownList)
        Dim ar As Web.UI.WebControls.ListItem() = Nothing
        Dim ar1 As Array = Nothing
        Dim i As Long = 0

        Try
            For Each li As Web.UI.WebControls.ListItem In ddl.Items
                ReDim Preserve ar(i)
                ar(i) = li
                i += 1
            Next
            ar1 = ar

            ar1.Sort(ar1, New ListItemComparer)
            ddl.Items.Clear()
            ddl.Items.AddRange(ar1)

            Exit Sub
        Catch ex As Exception
            'HandleError(ClassName, "SortDropdown", ex, Me, txtPolnum.Text, lblMsg)
            Exit Sub
        End Try
    End Sub

    Private Class ListItemComparer
        Implements IComparer

        Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
            Dim a As Web.UI.WebControls.ListItem = x
            Dim b As Web.UI.WebControls.ListItem = y
            Dim c As New CaseInsensitiveComparer
            Return c.Compare(a.Text, b.Text)
        End Function
    End Class
    'added 8/3/2017
    Public Function ApplicableLobTypeForCriteria(Optional ByVal polNum As String = "", Optional ByVal fnolType As Enums.FNOL_Type = Nothing, Optional ByVal packagePartType As String = "") As Enums.PolicyLobType
        Dim lobType As Enums.PolicyLobType = LobTypeForPolicyNumber(polNum)

        If lobType = Enums.PolicyLobType.CommercialPackage AndAlso String.IsNullOrWhiteSpace(packagePartType) = False Then
            lobType = LobTypeForPackagePartType(packagePartType, defaultToCPP:=True)
        End If

        If lobType = Enums.PolicyLobType.CommercialPackage Then
            Select Case fnolType
                Case Enums.FNOL_Type.AutoFNOL
                    lobType = Enums.PolicyLobType.CommercialGarage
                Case Enums.FNOL_Type.LiabilityFNOL
                    lobType = Enums.PolicyLobType.CommercialGeneralLiability
                Case Enums.FNOL_Type.PropertyFNOL
                    lobType = Enums.PolicyLobType.CommercialProperty
            End Select
        ElseIf lobType = Enums.PolicyLobType.NotAssigned AndAlso fnolType <> Nothing Then
            Select Case fnolType
                Case Enums.FNOL_Type.AutoFNOL
                    lobType = Enums.PolicyLobType.AutoPersonal
                Case Enums.FNOL_Type.LiabilityFNOL
                    lobType = Enums.PolicyLobType.CommercialGeneralLiability
                Case Enums.FNOL_Type.PropertyFNOL
                    lobType = Enums.PolicyLobType.HomePersonal
            End Select
        End If

        Return lobType
    End Function
    Public Function ClaimLossTypeForSelectedValue(ByVal selectedValue As String, Optional ByVal polNum As String = "", Optional ByVal fnolType As Enums.FNOL_Type = Nothing, Optional ByVal packagePartType As String = "") As Enums.ClaimLossType
        Dim lossType As Enums.ClaimLossType = Enums.ClaimLossType.NotAvailable

        Dim intForString As Integer = IntegerForString(selectedValue)
        If intForString > 0 Then
            'note: this logic will check against 2 diff enums, though anything from ClaimLossType_All should now be in ClaimLossType, so if it's valid in ClaimLossType_All, shouldn't really need to check against ClaimLossType
            Dim looksValid As Boolean = False
            Dim definitelyValid As Boolean = False
            If System.Enum.IsDefined(GetType(Enums.ClaimLossType), intForString) = True Then
                looksValid = True
            End If
            If System.Enum.IsDefined(GetType(Enums.ClaimLossType_AllValid), intForString) = True Then
                definitelyValid = True
            End If

            Dim lobType As Enums.PolicyLobType = ApplicableLobTypeForCriteria(polNum:=polNum, fnolType:=fnolType, packagePartType:=packagePartType)

            If looksValid = True Then
                If definitelyValid = True Then
                    'set it
                    lossType = intForString

                    'double-check a few values
                    'Select Case lossType
                    '    Case Enums.ClaimLossType.OtherNOC, Enums.ClaimLossType.OtherNOC2, Enums.ClaimLossType.OtherNOC3
                    '        Select Case lobType
                    '            Case Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal, Enums.PolicyLobType.InlandMarinePersonal
                    '                lossType = Enums.ClaimLossType.OtherNOC
                    '            Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                    '                lossType = Enums.ClaimLossType.OtherNOC2
                    '            Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialCrime, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty
                    '                lossType = Enums.ClaimLossType.OtherNOC3
                    '        End Select
                    'End Select
                    ValidateLossType(lossType, lobType:=lobType)
                Else
                    'try to find alternate value in ClaimLossType that's also in ClaimLossType
                    Dim tempLossType As Enums.ClaimLossType = intForString
                    'Select Case tempLossType
                    '    Case Enums.ClaimLossType.AircraftDamage
                    '        lossType = Enums.ClaimLossType.Aircraft
                    '    Case Enums.ClaimLossType.AnimalDamage
                    '        Select Case lobType
                    '            Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty
                    '                lossType = Enums.ClaimLossType.AnimalDamage3
                    '            Case Else 'DFR, FAR, HOM, PIM
                    '                lossType = Enums.ClaimLossType.AnimalDamage2
                    '        End Select
                    '    Case Enums.ClaimLossType.EquipmentBreakdown
                    '        Select Case lobType
                    '            Case Enums.PolicyLobType.CommercialBOP
                    '                lossType = Enums.ClaimLossType.EquipmentBreakdown3
                    '            Case Else 'CIM, CPR, DFR, FAR, HOM
                    '                lossType = Enums.ClaimLossType.EquipmentBreakdown2
                    '        End Select
                    '    Case Enums.ClaimLossType.Explosion
                    '        Select Case lobType
                    '            Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                    '                lossType = Enums.ClaimLossType.Explosion3
                    '            Case Else 'BOP, CIM, CPR, DFR, FAR, HOM
                    '                lossType = Enums.ClaimLossType.Explosion2
                    '        End Select
                    '    Case Enums.ClaimLossType.Flood
                    '        lossType = Enums.ClaimLossType.Flood2
                    '    Case Enums.ClaimLossType.GlassBreakage
                    '        Select Case lobType
                    '            Case Enums.PolicyLobType.CommercialBOP
                    '                lossType = Enums.ClaimLossType.GlassBreakage3
                    '            Case Else 'CIM, CPR, DFR, FAR, HOM
                    '                lossType = Enums.ClaimLossType.GlassBreakage2
                    '        End Select
                    '    Case Enums.ClaimLossType.IdentityTheft
                    '        lossType = Enums.ClaimLossType.IdentityTheft2
                    '    Case Enums.ClaimLossType.IntakeofForeignObject
                    '        lossType = Enums.ClaimLossType.IntakeOfForeignObjects
                    '    Case Enums.ClaimLossType.LiabilityAnimalOrLivestock
                    '        lossType = Enums.ClaimLossType.LiabilityAnimalOrLivestock2
                    '    Case Enums.ClaimLossType.DogBiteLiability, Enums.ClaimLossType.LiabilityDogBite
                    '        Select Case lobType
                    '            Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialGeneralLiability
                    '                lossType = Enums.ClaimLossType.LiabilityPetBite2
                    '            Case Else 'GAR, DFR, FAR, HOM
                    '                lossType = Enums.ClaimLossType.LiabilityPetBite
                    '        End Select
                    '    Case Enums.ClaimLossType.LiabilityEnvironmental, Enums.ClaimLossType.LiabilityEnvironmentalMiscellaneous, Enums.ClaimLossType.LiabilityEnvironmentalMold, Enums.ClaimLossType.LiabilityEnvironmentalPetroleum, Enums.ClaimLossType.LiabilityEnvironmentalSolidWaste
                    '        lossType = Enums.ClaimLossType.LiabilityEnvironmental2
                    '    Case Enums.ClaimLossType.LiabilitySlipandFall
                    '        lossType = Enums.ClaimLossType.LiabilitySlipAndFall2
                    '    Case Enums.ClaimLossType.MineSubsidence
                    '        Select Case lobType
                    '            Case Enums.PolicyLobType.CommercialBOP
                    '                lossType = Enums.ClaimLossType.MineSubsidence3
                    '            Case Else 'CIM, CPR, DFR, FAR, HOM
                    '                lossType = Enums.ClaimLossType.MineSubsidence2
                    '        End Select
                    '    Case Enums.ClaimLossType.MysteriousDisappearanceInvolvingScheduledProperty
                    '        lossType = Enums.ClaimLossType.MysteriousDisappearance
                    '    Case Enums.ClaimLossType.PowerInterruptionOffPremise, Enums.ClaimLossType.PowerInterruptionOnPremise
                    '        Select Case lobType
                    '            Case Enums.PolicyLobType.CommercialBOP
                    '                lossType = Enums.ClaimLossType.PowerInterruption2
                    '            Case Else 'CIM, CPR, DFR, FAR, HOM
                    '                lossType = Enums.ClaimLossType.PowerInterruption
                    '        End Select
                    '    Case Enums.ClaimLossType.RiotOrCivilCommotion
                    '        Select Case lobType
                    '            Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                    '                lossType = Enums.ClaimLossType.RiotOrCivilCommotion3
                    '            Case Else 'BOP, CIM, CPR, DFR, FAR, HOM
                    '                lossType = Enums.ClaimLossType.RiotOrCivilCommotion2
                    '        End Select
                    '    Case Enums.ClaimLossType.TheftAutoPAPDOrOTC, Enums.ClaimLossType.TheftInvolvingScheduledProperty, Enums.ClaimLossType.TheftofMoney, Enums.ClaimLossType.TheftofMoneyandSecurities, Enums.ClaimLossType.TheftOrBurglary, Enums.ClaimLossType.TheftOther
                    '        Select Case lobType
                    '            Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                    '                lossType = Enums.ClaimLossType.TheftPartsOrContents
                    '            Case Else 'BOP, CRM, CIM, CPR, DFR, FAR, HOM, PIM
                    '                lossType = Enums.ClaimLossType.Theft
                    '        End Select
                    '    Case Enums.ClaimLossType.VehicleDamage
                    '        lossType = Enums.ClaimLossType.VehicleDamage2
                    '    Case Enums.ClaimLossType.WaterDamage
                    '        lossType = Enums.ClaimLossType.WaterDamageOther
                    '    Case Enums.ClaimLossType.WeatherRelatedWaterDamage
                    '        lossType = Enums.ClaimLossType.WaterDamageWindDriven
                    '    Case Enums.ClaimLossType.WorkersCompensation
                    '        Select Case lobType
                    '            Case Enums.PolicyLobType.WorkersComp
                    '                lossType = Enums.ClaimLossType.WorkComp
                    '            Case Enums.PolicyLobType.Farm
                    '                lossType = Enums.ClaimLossType.WorkersCompensation2
                    '            Case Else 'CUP, PUP
                    '                lossType = Enums.ClaimLossType.WorkersCompensation3
                    '        End Select
                    '    Case Else
                    '        'just use it
                    '        lossType = tempLossType
                    'End Select
                    'updated 8/4/2017 to use new function
                    lossType = ReplacementLossType(tempLossType, lobType:=lobType)
                End If
            ElseIf definitelyValid = True Then
                'this shouldn't ever happen if looksValid is false, but... set it anyway
                lossType = intForString

                'double-check a few values
                ValidateLossType(lossType, lobType:=lobType)
            Else
                'appears to be invalid but positive #; check to see if it's a valid id in Diamond's table
                If System.Enum.IsDefined(GetType(Enums.ClaimLossType_All), intForString) = True Then
                    lossType = intForString
                End If
            End If
        End If

        Return lossType
    End Function
    'added 8/4/2017
    Public Function ReplacementLossType(ByVal invalidLossType As Enums.ClaimLossType, Optional ByVal lobType As Enums.PolicyLobType = Enums.PolicyLobType.NotAssigned) As Enums.ClaimLossType
        Dim lossType As Enums.ClaimLossType = Enums.ClaimLossType.NotAvailable

        Select Case invalidLossType
            Case Enums.ClaimLossType.AircraftDamage
                lossType = Enums.ClaimLossType.Aircraft
            Case Enums.ClaimLossType.AnimalDamage
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty
                        lossType = Enums.ClaimLossType.AnimalDamage3
                    Case Else 'DFR, FAR, HOM, PIM
                        lossType = Enums.ClaimLossType.AnimalDamage2
                End Select
            Case Enums.ClaimLossType.CollisionUpsetOverturn
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP
                        lossType = Enums.ClaimLossType.CollisionUpsetOrOverturn2
                    Case Else 'CIM, CPR, FAR, PIM
                        lossType = Enums.ClaimLossType.CollisionUpsetOrOverturn
                End Select
            Case Enums.ClaimLossType.EquipmentBreakdown
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP
                        lossType = Enums.ClaimLossType.EquipmentBreakdown3
                    Case Else 'CIM, CPR, DFR, FAR, HOM
                        lossType = Enums.ClaimLossType.EquipmentBreakdown2
                End Select
            Case Enums.ClaimLossType.Explosion
                Select Case lobType
                    Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                        lossType = Enums.ClaimLossType.Explosion3
                    Case Else 'BOP, CIM, CPR, DFR, FAR, HOM
                        lossType = Enums.ClaimLossType.Explosion2
                End Select
            Case Enums.ClaimLossType.Flood
                lossType = Enums.ClaimLossType.Flood2
            Case Enums.ClaimLossType.GlassBreakage
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP
                        lossType = Enums.ClaimLossType.GlassBreakage3
                    Case Else 'CIM, CPR, DFR, FAR, HOM
                        lossType = Enums.ClaimLossType.GlassBreakage2
                End Select
            Case Enums.ClaimLossType.IdentityTheft
                lossType = Enums.ClaimLossType.IdentityTheft2
            Case Enums.ClaimLossType.IntakeofForeignObject
                lossType = Enums.ClaimLossType.IntakeOfForeignObjects
            Case Enums.ClaimLossType.LiabilityAnimalOrLivestock
                lossType = Enums.ClaimLossType.LiabilityAnimalOrLivestock2
            Case Enums.ClaimLossType.DogBiteLiability, Enums.ClaimLossType.LiabilityDogBite
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialGeneralLiability
                        lossType = Enums.ClaimLossType.LiabilityPetBite2
                    Case Else 'GAR, DFR, FAR, HOM
                        lossType = Enums.ClaimLossType.LiabilityPetBite
                End Select
            Case Enums.ClaimLossType.LiabilityEnvironmental, Enums.ClaimLossType.LiabilityEnvironmentalMiscellaneous, Enums.ClaimLossType.LiabilityEnvironmentalMold, Enums.ClaimLossType.LiabilityEnvironmentalPetroleum, Enums.ClaimLossType.LiabilityEnvironmentalSolidWaste
                lossType = Enums.ClaimLossType.LiabilityEnvironmental2
            Case Enums.ClaimLossType.LiabilitySlipandFall
                lossType = Enums.ClaimLossType.LiabilitySlipAndFall2
            Case Enums.ClaimLossType.MineSubsidence
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP
                        lossType = Enums.ClaimLossType.MineSubsidence3
                    Case Else 'CIM, CPR, DFR, FAR, HOM
                        lossType = Enums.ClaimLossType.MineSubsidence2
                End Select
            Case Enums.ClaimLossType.MysteriousDisappearanceInvolvingScheduledProperty
                lossType = Enums.ClaimLossType.MysteriousDisappearance
            Case Enums.ClaimLossType.PowerInterruptionOffPremise, Enums.ClaimLossType.PowerInterruptionOnPremise
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP
                        lossType = Enums.ClaimLossType.PowerInterruption2
                    Case Else 'CIM, CPR, DFR, FAR, HOM
                        lossType = Enums.ClaimLossType.PowerInterruption
                End Select
            Case Enums.ClaimLossType.RiotOrCivilCommotion
                Select Case lobType
                    Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                        lossType = Enums.ClaimLossType.RiotOrCivilCommotion3
                    Case Else 'BOP, CIM, CPR, DFR, FAR, HOM
                        lossType = Enums.ClaimLossType.RiotOrCivilCommotion2
                End Select
            Case Enums.ClaimLossType.TheftAutoPAPDOrOTC, Enums.ClaimLossType.TheftInvolvingScheduledProperty, Enums.ClaimLossType.TheftofMoney, Enums.ClaimLossType.TheftofMoneyandSecurities, Enums.ClaimLossType.TheftOrBurglary, Enums.ClaimLossType.TheftOther
                Select Case lobType
                    Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                        lossType = Enums.ClaimLossType.TheftPartsOrContents
                    Case Else 'BOP, CRM, CIM, CPR, DFR, FAR, HOM, PIM
                        lossType = Enums.ClaimLossType.Theft
                End Select
            Case Enums.ClaimLossType.VehicleDamage
                lossType = Enums.ClaimLossType.VehicleDamage2
            Case Enums.ClaimLossType.WaterDamage
                lossType = Enums.ClaimLossType.WaterDamageOther
            Case Enums.ClaimLossType.WeatherRelatedWaterDamage
                lossType = Enums.ClaimLossType.WaterDamageWindDriven
            Case Enums.ClaimLossType.WorkersCompensation
                Select Case lobType
                    Case Enums.PolicyLobType.WorkersComp
                        lossType = Enums.ClaimLossType.WorkComp
                    Case Enums.PolicyLobType.Farm
                        lossType = Enums.ClaimLossType.WorkersCompensation2
                    Case Else 'CUP, PUP
                        lossType = Enums.ClaimLossType.WorkersCompensation3
                End Select
            Case Else
                'just use it
                lossType = invalidLossType
        End Select

        Return lossType
    End Function
    Public Sub ValidateLossType(ByRef lossType As Enums.ClaimLossType, Optional ByVal lobType As Enums.PolicyLobType = Enums.PolicyLobType.NotAssigned)
        Select Case lossType
            Case Enums.ClaimLossType.Collapse, Enums.ClaimLossType.Collapse2
                Select Case lobType
                    Case Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.Collapse
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty
                        lossType = Enums.ClaimLossType.Collapse2
                End Select
            Case Enums.ClaimLossType.DebrisRemoval, Enums.ClaimLossType.DebrisRemoval2
                Select Case lobType
                    Case Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.DebrisRemoval
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty
                        lossType = Enums.ClaimLossType.DebrisRemoval2
                End Select
            Case Enums.ClaimLossType.Earthquake, Enums.ClaimLossType.Earthquake2
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty, Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.Earthquake
                    Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                        lossType = Enums.ClaimLossType.Earthquake2
                End Select
            Case Enums.ClaimLossType.EmployersLiability, Enums.ClaimLossType.EmployersLiability2
                Select Case lobType
                    Case Enums.PolicyLobType.Farm
                        lossType = Enums.ClaimLossType.EmployersLiability
                    Case Enums.PolicyLobType.WorkersComp
                        lossType = Enums.ClaimLossType.EmployersLiability2
                End Select
            Case Enums.ClaimLossType.FallingObjects, Enums.ClaimLossType.FallingObjects2
                Select Case lobType
                    Case Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.FallingObjects
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty
                        lossType = Enums.ClaimLossType.FallingObjects2
                End Select
            Case Enums.ClaimLossType.FalseAlarm, Enums.ClaimLossType.FalseAlarm2
                Select Case lobType
                    Case Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.FalseAlarm
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty
                        lossType = Enums.ClaimLossType.FalseAlarm2
                End Select
            Case Enums.ClaimLossType.Fire, Enums.ClaimLossType.Fire2
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty, Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal, Enums.PolicyLobType.InlandMarinePersonal
                        lossType = Enums.ClaimLossType.Fire
                    Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                        lossType = Enums.ClaimLossType.Fire2
                End Select
            Case Enums.ClaimLossType.Hail, Enums.ClaimLossType.Hail2
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty, Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.Hail
                    Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                        lossType = Enums.ClaimLossType.Hail2
                End Select
            Case Enums.ClaimLossType.LiabilityDamageToPropertyOfOthers, Enums.ClaimLossType.LiabilityDamageToPropertyOfOthers2
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialGarage, Enums.PolicyLobType.CommercialGeneralLiability, Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.LiabilityDamageToPropertyOfOthers
                    Case Enums.PolicyLobType.CommercialBOP
                        lossType = Enums.ClaimLossType.LiabilityDamageToPropertyOfOthers2
                End Select
            Case Enums.ClaimLossType.LiabilityPersonalInjury, Enums.ClaimLossType.LiabilityPersonalInjury2
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialGarage, Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.LiabilityPersonalInjury
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialGeneralLiability
                        lossType = Enums.ClaimLossType.LiabilityPersonalInjury2
                End Select
            Case Enums.ClaimLossType.OtherNOC, Enums.ClaimLossType.OtherNOC2, Enums.ClaimLossType.OtherNOC3
                Select Case lobType
                    Case Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal, Enums.PolicyLobType.InlandMarinePersonal
                        lossType = Enums.ClaimLossType.OtherNOC
                    Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                        lossType = Enums.ClaimLossType.OtherNOC2
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialCrime, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty
                        lossType = Enums.ClaimLossType.OtherNOC3
                End Select
            Case Enums.ClaimLossType.RoadbedCollision, Enums.ClaimLossType.RoadbedCollision2
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialProperty, Enums.PolicyLobType.Farm
                        lossType = Enums.ClaimLossType.RoadbedCollision
                    Case Enums.PolicyLobType.CommercialBOP
                        lossType = Enums.ClaimLossType.RoadbedCollision2
                End Select
            Case Enums.ClaimLossType.WeightOfIceSnowOrSleet, Enums.ClaimLossType.WeightOfIceSnowOrSleet2
                Select Case lobType
                    Case Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.WeightOfIceSnowOrSleet
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty
                        lossType = Enums.ClaimLossType.WeightOfIceSnowOrSleet2
                End Select

                'all of these are also in ReplacementLossType
            Case Enums.ClaimLossType.AnimalDamage, Enums.ClaimLossType.AnimalDamage2, Enums.ClaimLossType.AnimalDamage3
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty
                        lossType = Enums.ClaimLossType.AnimalDamage3
                    Case Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal, Enums.PolicyLobType.InlandMarinePersonal
                        lossType = Enums.ClaimLossType.AnimalDamage2
                End Select
            Case Enums.ClaimLossType.CollisionUpsetOverturn, Enums.ClaimLossType.CollisionUpsetOrOverturn, Enums.ClaimLossType.CollisionUpsetOrOverturn2
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP
                        lossType = Enums.ClaimLossType.CollisionUpsetOrOverturn2
                    Case Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty, Enums.PolicyLobType.Farm, Enums.PolicyLobType.InlandMarinePersonal
                        lossType = Enums.ClaimLossType.CollisionUpsetOrOverturn
                End Select
            Case Enums.ClaimLossType.EquipmentBreakdown, Enums.ClaimLossType.EquipmentBreakdown2, Enums.ClaimLossType.EquipmentBreakdown3
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP
                        lossType = Enums.ClaimLossType.EquipmentBreakdown3
                    Case Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty, Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.EquipmentBreakdown2
                End Select
            Case Enums.ClaimLossType.Explosion, Enums.ClaimLossType.Explosion2, Enums.ClaimLossType.Explosion3
                Select Case lobType
                    Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                        lossType = Enums.ClaimLossType.Explosion3
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty, Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.Explosion2
                End Select
            Case Enums.ClaimLossType.GlassBreakage, Enums.ClaimLossType.GlassBreakage2, Enums.ClaimLossType.GlassBreakage3
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP
                        lossType = Enums.ClaimLossType.GlassBreakage3
                    Case Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty, Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.GlassBreakage2
                End Select
            Case Enums.ClaimLossType.DogBiteLiability, Enums.ClaimLossType.LiabilityDogBite, Enums.ClaimLossType.LiabilityPetBite, Enums.ClaimLossType.LiabilityPetBite2
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialGeneralLiability
                        lossType = Enums.ClaimLossType.LiabilityPetBite2
                    Case Enums.PolicyLobType.CommercialGarage, Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.LiabilityPetBite
                End Select
            Case Enums.ClaimLossType.MineSubsidence, Enums.ClaimLossType.MineSubsidence2, Enums.ClaimLossType.MineSubsidence3
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP
                        lossType = Enums.ClaimLossType.MineSubsidence3
                    Case Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty, Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.MineSubsidence2
                End Select
            Case Enums.ClaimLossType.PowerInterruptionOffPremise, Enums.ClaimLossType.PowerInterruptionOnPremise, Enums.ClaimLossType.PowerInterruption, Enums.ClaimLossType.PowerInterruption2
                Select Case lobType
                    Case Enums.PolicyLobType.CommercialBOP
                        lossType = Enums.ClaimLossType.PowerInterruption2
                    Case Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty, Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.PowerInterruption
                End Select
            Case Enums.ClaimLossType.RiotOrCivilCommotion, Enums.ClaimLossType.RiotOrCivilCommotion2, Enums.ClaimLossType.RiotOrCivilCommotion3
                Select Case lobType
                    Case Enums.PolicyLobType.AutoPersonal, Enums.PolicyLobType.CommercialAuto, Enums.PolicyLobType.CommercialGarage
                        lossType = Enums.ClaimLossType.RiotOrCivilCommotion3
                    Case Enums.PolicyLobType.CommercialBOP, Enums.PolicyLobType.CommercialInlandMarine, Enums.PolicyLobType.CommercialProperty, Enums.PolicyLobType.DwellingFirePersonal, Enums.PolicyLobType.Farm, Enums.PolicyLobType.HomePersonal
                        lossType = Enums.ClaimLossType.RiotOrCivilCommotion2
                End Select
            Case Enums.ClaimLossType.WorkersCompensation, Enums.ClaimLossType.WorkersCompensation2, Enums.ClaimLossType.WorkersCompensation3, Enums.ClaimLossType.WorkComp
                Select Case lobType
                    Case Enums.PolicyLobType.WorkersComp
                        lossType = Enums.ClaimLossType.WorkComp
                    Case Enums.PolicyLobType.Farm
                        lossType = Enums.ClaimLossType.WorkersCompensation2
                    Case Enums.PolicyLobType.CommercialUmbrella, Enums.PolicyLobType.UmbrellaPersonal
                        lossType = Enums.ClaimLossType.WorkersCompensation3
                End Select
        End Select
    End Sub

    Private Sub SetLnImageNotePropsIfNeeded(ByVal lnImg As Diamond.Common.Objects.Claims.LossNotice.LnImage)
        If lnImg IsNot Nothing Then
            If CaptureLnImageNotePropsViaEmail() = True Then
                CaptureLnImageNotePropsViaEmail(lnImg)
            End If
            If SetLnImageNoteProps() = True Then
                SetLnImageNotePropsToDefaultValues(lnImg)
            End If
        End If
    End Sub
    Private Sub SetLnImageNotePropsToDefaultValues(ByVal lnImg As Diamond.Common.Objects.Claims.LossNotice.LnImage)
        If lnImg IsNot Nothing Then
            With lnImg
                '.Notes = LnImageNotesDefault()
                '.NotesTypeId = LnImageNotesTypeIdDefault()
                '.NoteTitle = LnImageNoteTitleDefault()
                Dim onlyIfKeyExists As Boolean = SetLnImageNoteProps_OnlyIfKeyExists()

                Dim notesKeyExists As Boolean = False
                Dim notes As String = LnImageNotesDefault(keyExists:=notesKeyExists)
                If notesKeyExists = True OrElse onlyIfKeyExists = False Then
                    .Notes = notes
                End If
                Dim notesTypeIdKeyExists As Boolean = False
                Dim notesTypeId As Integer = LnImageNotesTypeIdDefault(keyExists:=notesTypeIdKeyExists)
                If notesTypeIdKeyExists = True OrElse onlyIfKeyExists = False Then
                    .NotesTypeId = notesTypeId
                End If
                Dim noteTitleKeyExists As Boolean = False
                Dim noteTitle As String = LnImageNoteTitleDefault(keyExists:=noteTitleKeyExists)
                If noteTitleKeyExists = True OrElse onlyIfKeyExists = False Then
                    .NoteTitle = noteTitle
                End If
            End With
        End If
    End Sub
    Private Sub CaptureLnImageNotePropsViaEmail(ByVal lnImg As Diamond.Common.Objects.Claims.LossNotice.LnImage)
        Dim info As String = ""
        If lnImg IsNot Nothing Then
            info = InformationalTextForString(lnImg.Notes, identifier:="Notes")
            info &= "<br />" & InformationalTextForString(lnImg.NotesTypeId.ToString, identifier:="NotesTypeId")
            info &= "<br />" & InformationalTextForString(lnImg.NoteTitle, identifier:="NoteTitle")
        Else
            info = "LnImage object is NOTHING"
        End If
        SendEmail("Diamond.Common.Objects.Claims.LossNotice.LnImage default values for Note properties", "N/A", info)
    End Sub
    Private Function InformationalTextForString(ByVal str As String, Optional ByVal identifier As String = "string") As String
        Dim info As String = ""
        If String.IsNullOrWhiteSpace(identifier) = True Then
            identifier = "string"
        End If

        If str Is Nothing Then
            info = identifier & " value: NOTHING"
        Else
            info = identifier & " value: """ & str & """" & " (length: " & Len(str).ToString & ")"
        End If

        Return info
    End Function
    Private Function SetLnImageNoteProps() As Boolean
        If AppSettings("DiamondClaimsFNOL_SetLnImageNoteProps") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(AppSettings("DiamondClaimsFNOL_SetLnImageNoteProps").ToString) = False AndAlso (UCase(ConfigurationManager.AppSettings("DiamondClaimsFNOL_SetLnImageNoteProps").ToString) = "TRUE" OrElse UCase(ConfigurationManager.AppSettings("DiamondClaimsFNOL_SetLnImageNoteProps").ToString) = "YES") Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Function SetLnImageNoteProps_OnlyIfKeyExists() As Boolean
        If AppSettings("DiamondClaimsFNOL_SetLnImageNoteProps_OnlyIfKeyExists") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(AppSettings("DiamondClaimsFNOL_SetLnImageNoteProps_OnlyIfKeyExists").ToString) = False AndAlso (UCase(ConfigurationManager.AppSettings("DiamondClaimsFNOL_SetLnImageNoteProps_OnlyIfKeyExists").ToString) = "TRUE" OrElse UCase(ConfigurationManager.AppSettings("DiamondClaimsFNOL_SetLnImageNoteProps_OnlyIfKeyExists").ToString) = "YES") Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Function CaptureLnImageNotePropsViaEmail() As Boolean
        If AppSettings("DiamondClaimsFNOL_CaptureLnImageNotePropsViaEmail") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(AppSettings("DiamondClaimsFNOL_CaptureLnImageNotePropsViaEmail").ToString) = False AndAlso (UCase(ConfigurationManager.AppSettings("DiamondClaimsFNOL_CaptureLnImageNotePropsViaEmail").ToString) = "TRUE" OrElse UCase(ConfigurationManager.AppSettings("DiamondClaimsFNOL_CaptureLnImageNotePropsViaEmail").ToString) = "YES") Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Function LnImageNotesDefault(Optional ByRef keyExists As Boolean = False) As String
        Dim def As String = ""
        keyExists = False

        If AppSettings("DiamondClaimsFNOL_LnImageNotesDefault") IsNot Nothing Then
            keyExists = True
            If String.IsNullOrWhiteSpace(AppSettings("DiamondClaimsFNOL_LnImageNotesDefault").ToString) = False Then
                def = AppSettings("DiamondClaimsFNOL_LnImageNotesDefault").ToString
            End If
        End If

        Return def
    End Function
    Private Function LnImageNotesTypeIdDefault(Optional ByRef keyExists As Boolean = False) As Integer
        Dim def As Integer = 0
        keyExists = False

        If AppSettings("DiamondClaimsFNOL_LnImageNotesTypeIdDefault") IsNot Nothing Then
            keyExists = True
            If String.IsNullOrWhiteSpace(AppSettings("DiamondClaimsFNOL_LnImageNotesTypeIdDefault").ToString) = False Then
                def = IntegerForString(AppSettings("DiamondClaimsFNOL_LnImageNotesTypeIdDefault").ToString)
            End If
        End If

        Return def
    End Function
    Private Function LnImageNoteTitleDefault(Optional ByRef keyExists As Boolean = False) As String
        Dim def As String = ""
        keyExists = False

        If AppSettings("DiamondClaimsFNOL_LnImageNoteTitleDefault") IsNot Nothing Then
            keyExists = True
            If String.IsNullOrWhiteSpace(AppSettings("DiamondClaimsFNOL_LnImageNoteTitleDefault").ToString) = False Then
                def = AppSettings("DiamondClaimsFNOL_LnImageNoteTitleDefault").ToString
            End If
        End If

        Return def
    End Function
    Private Function CreateDiamondXmlsForTesting(Optional ByRef fileDir As String = "") As Boolean
        Dim createXmls As Boolean = False
        fileDir = ""

        If AppSettings("DiamondClaimsFNOL_CreateDiamondXmlsForTesting") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(AppSettings("DiamondClaimsFNOL_CreateDiamondXmlsForTesting").ToString) = False AndAlso (UCase(ConfigurationManager.AppSettings("DiamondClaimsFNOL_CreateDiamondXmlsForTesting").ToString) = "TRUE" OrElse UCase(ConfigurationManager.AppSettings("DiamondClaimsFNOL_CreateDiamondXmlsForTesting").ToString) = "YES") Then
            createXmls = True

            fileDir = DiamondXmlsSaveDirectory()
            If String.IsNullOrWhiteSpace(fileDir) = True OrElse IO.Directory.Exists(fileDir) = False Then
                createXmls = False
                fileDir = ""
            End If
        End If

        Return createXmls
    End Function


    Private Function DiamondXmlsSaveDirectory() As String
        Dim saveDir As String = ""

        If AppSettings("DiamondClaimsFNOL_DiamondXmlsSaveDirectory") IsNot Nothing AndAlso String.IsNullOrWhiteSpace(AppSettings("DiamondClaimsFNOL_DiamondXmlsSaveDirectory").ToString) = False Then
            saveDir = AppendTrailingSlashIfNeeded(AppSettings("DiamondClaimsFNOL_DiamondXmlsSaveDirectory").ToString)
        End If

        Return saveDir
    End Function
    Private Function AppendTrailingSlashIfNeeded(ByVal str As String) As String
        Dim strWithTrailingSlash As String = str

        If String.IsNullOrWhiteSpace(strWithTrailingSlash) = False Then
            Dim lastChar As String = Right(strWithTrailingSlash, 1)
            If lastChar <> "/" AndAlso lastChar <> "\" Then
                'need to add
                If strWithTrailingSlash.Contains("/") = True Then
                    strWithTrailingSlash &= "/"
                Else
                    strWithTrailingSlash &= "\"
                End If
            End If
        End If

        Return strWithTrailingSlash
    End Function

    Private disposedValue As Boolean = False        ' To detect redundant calls

    Private Function DiamondClaimsFNOLCCCClaimsEnabled() As Boolean
        Dim CCCClaimsEnabled As Boolean = False

        If AppSettings("DiamondClaimsFNOL_CCCClaimsEnabled") IsNot Nothing AndAlso AppSettings("DiamondClaimsFNOL_CCCClaimsEnabled").ToString.ToLower = "true" Then
            CCCClaimsEnabled = True

        End If

        Return CCCClaimsEnabled
    End Function

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                If Errors IsNot Nothing Then Errors = Nothing
            End If

            ' TODO: free your own state (unmanaged objects).
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class