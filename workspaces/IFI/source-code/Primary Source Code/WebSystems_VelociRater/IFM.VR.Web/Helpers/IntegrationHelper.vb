Imports Diamond.Business.ThirdParty.tuxml
Imports Diamond.Business.ThirdParty.XactwareStandardCarrierExport
Imports Diamond.Common.Objects.Claims
Imports Diamond.Common.Objects.Claims.Claimant
Imports Diamond.Web.BaseControls
Imports IFM.VR.Common.Helpers
Imports IFM.VR.Common.Helpers.BOP
Imports IFM.VR.Web.Helpers
Imports IFM.VR.Web.Helpers.WebHelper_Personal
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.Extensions.FileSystemGlobbing
Imports PublicQuotingLib.Models
Imports QuickQuote.CommonMethods
'Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Public Class IntegrationHelper
    Dim chc = New CommonHelperClass
    Dim LogTimer As Boolean = chc.ConfigurationAppSettingValueAsBoolean("API_Timer_Flag")
    Public Sub CallHomIntegrationPreLoad(ByRef qqo As QuickQuoteObject)
        Dim stopwatch As Stopwatch = Stopwatch.StartNew()
        'Used for preloading Cape Analytics, WillisTowersWatson and potentially other integration results which Diamond uses for Rating.
        If qqo IsNot Nothing AndAlso Common.Helpers.HOM.AllHomeRatingHelper.IsAllHomeRatingAvailable(qqo) Then
            If qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 AndAlso qqo.Locations(0) IsNot Nothing AndAlso qqo.Locations(0).Address IsNot Nothing Then
                Dim chc As New CommonHelperClass
                Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_APIKey")
                Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_BaseURL")
                Dim myAddress = qqo.Locations(0).Address
                Dim homRatingRequest = New IFI.Integrations.Request.AllHomeRating(baseUrl, apiKey) With {
                    .Address1 = GetStreetAddress(myAddress),
                    .Address2 = "",
                    .City = myAddress.City,
                    .State = myAddress.State,
                    .Zip = If(myAddress.Zip.Length > 5, myAddress.Zip.Substring(0, 5), myAddress.Zip),
                    .Country = "US"
                }
                'Dim response = homRatingRequest.PreLoadHomRatingScores()
                'updated 8/31/2023 for latest Integration Nuget package
                Dim response = homRatingRequest.PreLoadVendorData()
            End If
        End If
        stopwatch.Stop()
        If LogTimer Then
            IFM.IFMErrorLogging.LogIssue("CallHomIntegrationPreLoad API", "Elapsed Time: " & stopwatch.ElapsedMilliseconds & " ms")
        End If
    End Sub

    Public Sub CallBetterViewPreLoad(ByRef qqo As QuickQuoteObject)
        Dim stopwatch As Stopwatch = Stopwatch.StartNew()
        ' Used for preloading BetterView results which Diamond uses for Rating.  Only allows a single location.
        If qqo IsNot Nothing AndAlso Common.Helpers.AllLines.BetterViewRatingHelper.IsBetterViewRatingAvailable(qqo) Then
            If qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 AndAlso qqo.Locations(0) IsNot Nothing AndAlso qqo.Locations(0).Address IsNot Nothing Then
                Dim chc As New CommonHelperClass
                Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_AllLines_BetterView_PreLoadIntegrationCall_APIKey")
                Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_AllLines_BetterView_PreLoadIntegrationCall_BaseURL")
                Dim myAddress = qqo.Locations(0).Address
                Dim BvRatingRequest = New IFI.Integrations.Request.BetterView(baseUrl, apiKey) With {
                    .Address1 = GetStreetAddress(myAddress),
                    .Address2 = "",
                    .City = myAddress.City,
                    .State = myAddress.State,
                    .Zip = If(myAddress.Zip.Length > 5, myAddress.Zip.Substring(0, 5), myAddress.Zip),
                    .Country = "US"
                }

                If VerifiedLocationData(BvRatingRequest) Then
                    Dim response = BvRatingRequest.PreLoadVendorData()
                End If
            End If
        End If
        stopwatch.Stop()
        If LogTimer Then
            IFM.IFMErrorLogging.LogIssue("CallBetterViewPreLoad API", "Elapsed Time: " & stopwatch.ElapsedMilliseconds & " ms")
        End If
    End Sub

    Public Sub CallBetterViewComPreLoad(ByRef qqo As QuickQuoteObject)
        Dim stopwatch As Stopwatch = Stopwatch.StartNew()
        ' Used for preloading BetterView results which Diamond uses for Rating.  Only allows a single location.
        If qqo IsNot Nothing AndAlso Common.Helpers.AllLines.BetterViewRatingHelper.IsBetterViewRatingAvailable(qqo) Then
            If qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 Then
                Dim chc As New CommonHelperClass
                Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_AllLines_BetterView_PreLoadIntegrationCall_APIKey")
                Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_AllLines_BetterView_PreLoadIntegrationCall_BaseURL")



                For Each location In qqo.Locations
                    If location IsNot Nothing AndAlso location.Address IsNot Nothing AndAlso location.Buildings IsNot Nothing Then

                        Dim NeedPreLoad As Boolean
                        For Each building As QuickQuoteBuilding In location.Buildings
                            If String.IsNullOrWhiteSpace(building.Limit) = False AndAlso building.Limit <> "0" Then
                                NeedPreLoad = True
                                Exit For
                            End If
                        Next

                        If NeedPreLoad Then
                            Dim myAddress = location.Address
                            Dim BvRatingRequest = New IFI.Integrations.Request.BetterView(baseUrl, apiKey) With {
                            .Address1 = GetStreetAddress(myAddress),
                            .Address2 = "",
                            .City = myAddress.City,
                            .State = myAddress.State,
                            .Zip = If(myAddress.Zip.Length > 5, myAddress.Zip.Substring(0, 5), myAddress.Zip),
                            .Country = "US"
                            }

                            If VerifiedLocationData(BvRatingRequest) Then
                                Dim response = BvRatingRequest.PreLoadVendorData()
                            End If
                        End If
                    End If
                Next
            End If
        End If
        stopwatch.Stop()
        If LogTimer Then
            IFM.IFMErrorLogging.LogIssue("CallBetterViewComPreLoad API", "Elapsed Time: " & stopwatch.ElapsedMilliseconds & " ms")
        End If
    End Sub

    Public Sub CallCapeComPreLoad(ByRef qqo As QuickQuoteObject)
        Dim stopwatch As Stopwatch = Stopwatch.StartNew()
        ' Used for preloading CapeCom results which Diamond uses for Rating.  Calls out once per location.
        If qqo IsNot Nothing AndAlso Common.Helpers.AllLines.CapeComRatingHelper.IsCapeComRatingAvailable(qqo) Then
            If qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 Then
                Dim chc As New CommonHelperClass
                Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_AllLines_CapeCom_PreLoadIntegrationCall_APIKey")
                Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_AllLines_CapeCom_PreLoadIntegrationCall_BaseURL")
                For Each location In qqo.Locations
                    If location IsNot Nothing AndAlso location.Address IsNot Nothing Then
                        Dim myAddress = location.Address
                        Dim CapeRatingRequest = New IFI.Integrations.Request.CapeCom(baseUrl, apiKey) With {
                            .Address1 = GetStreetAddress(myAddress),
                            .Address2 = "",
                            .City = myAddress.City,
                            .State = myAddress.State,
                            .Zip = If(myAddress.Zip.Length > 5, myAddress.Zip.Substring(0, 5), myAddress.Zip)
                        }
                        If VerifiedLocationData(CapeRatingRequest) Then
                            Dim response = CapeRatingRequest.PreLoadVendorData()
                        End If

                    End If
                Next
            End If
        End If
        stopwatch.Stop()
        If LogTimer Then
            IFM.IFMErrorLogging.LogIssue("CallCapeComPreLoad API", "Elapsed Time: " & stopwatch.ElapsedMilliseconds & " ms")
        End If
    End Sub

    Public Sub CallProtectionClassPreload(ByRef qqo As QuickQuoteObject)
        Dim stopwatch As Stopwatch = Stopwatch.StartNew()
        If qqo IsNot Nothing AndAlso NewCoProtectionClassHelper.IsNewCoProtectionClassAvailable(qqo) Then
            If qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 Then
                Dim chc As New CommonHelperClass
                Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_AllLines_ProtClass_PreLoadIntegrationCall_APIKey")
                Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_AllLines_ProtClass_PreLoadIntegrationCall_BaseURL")
                For Each location In qqo.Locations
                    If location IsNot Nothing AndAlso location.Address IsNot Nothing Then

                        If qqo.QuoteTransactionType = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                            Dim endorsementsPreexistHelper = New EndorsementsPreexistingHelper(qqo)
                            If endorsementsPreexistHelper.IsPreexistingLocation(location) Then
                                Continue For
                            End If
                        End If

                        Dim myAddress = location.Address
                        Dim ProtClassRequest = New IFI.Integrations.Request.ProtectionClass(baseUrl, apiKey) With {
                            .Address1 = GetStreetAddress(myAddress),
                            .Address2 = "",
                            .City = myAddress.City,
                            .State = myAddress.State,
                            .Zip = If(myAddress.Zip.Length > 5, myAddress.Zip.Substring(0, 5), myAddress.Zip),
                            .Country = "US"
                        }
                        'Dim response = ProtClassRequest.PreLoadVendorData()
                        If VerifiedLocationData(ProtClassRequest) Then
                            Dim response = ProtClassRequest.PreLoadVendorData()
                        End If
                    End If
                Next
            End If
        End If
        stopwatch.Stop()
        If LogTimer Then
            IFM.IFMErrorLogging.LogIssue("CallProtectionClassPreload API", "Elapsed Time: " & stopwatch.ElapsedMilliseconds & " ms")
        End If
    End Sub

    Private Function VerifiedLocationData(BetterViewRequest As IFI.Integrations.Request.BetterView) As Boolean
        Dim Tester As Boolean = True
        If String.IsNullOrEmpty(BetterViewRequest.Address1) Then Tester = False
        If String.IsNullOrEmpty(BetterViewRequest.City) Then Tester = False
        If String.IsNullOrEmpty(BetterViewRequest.State) Then Tester = False
        If String.IsNullOrEmpty(BetterViewRequest.Zip) Then Tester = False
        Return Tester
    End Function

    Private Function VerifiedLocationData(CapeRatingRequest As IFI.Integrations.Request.CapeCom) As Boolean
        Dim Tester As Boolean = True
        If String.IsNullOrEmpty(CapeRatingRequest.Address1) Then Tester = False
        If String.IsNullOrEmpty(CapeRatingRequest.City) Then Tester = False
        If String.IsNullOrEmpty(CapeRatingRequest.State) Then Tester = False
        If String.IsNullOrEmpty(CapeRatingRequest.Zip) Then Tester = False
        Return Tester
    End Function

    Private Function VerifiedLocationData(ProtClassRequest As IFI.Integrations.Request.ProtectionClass) As Boolean
        Dim Tester As Boolean = True
        If String.IsNullOrEmpty(ProtClassRequest.Address1) Then Tester = False
        If String.IsNullOrEmpty(ProtClassRequest.City) Then Tester = False
        If String.IsNullOrEmpty(ProtClassRequest.State) Then Tester = False
        If String.IsNullOrEmpty(ProtClassRequest.Zip) Then Tester = False
        Return Tester
    End Function

    Public Shared Function GetStreetAddress(ByVal myAddress As QuickQuoteAddress) As String
        Dim address As String = ""
        If myAddress IsNot Nothing Then
            If String.IsNullOrWhiteSpace(myAddress.HouseNum) = False OrElse String.IsNullOrWhiteSpace(myAddress.StreetName) = False Then
                address = myAddress.HouseNum + " " + myAddress.StreetName
                If String.IsNullOrWhiteSpace(myAddress.ApartmentNumber) = False Then
                    address = address + " " + myAddress.ApartmentNumber
                End If
            ElseIf String.IsNullOrWhiteSpace(myAddress.POBox) = False Then
                address = "PO Box " + myAddress.POBox
            End If
        End If
        Return Regex.Replace(address, " {2,}", " ") 'replace any instance of consecutive spaces with a single space
    End Function

    Public Function GetRACASymbols(vin As String) As List(Of RACASymbolsLookupResult)
        'Need to use IntegrationHelper in Common so we can use it in the RACASymbolHelper date crossing
        Dim cih As New IFM.VR.Common.Helpers.IntegrationHelper
        Dim results As List(Of RACASymbolsLookupResult) = cih.GetRACASymbolsForVin(vin)
        Return results
    End Function


    Public Enum CommercialDataPrefillServiceType
        None = 0
        FirmographicsAndProperty = 1
        FirmographicsOnly = 2
        PropertyOnly = 3
    End Enum
    Public Sub CallCommercialDataPrefill(ByRef qqo As QuickQuoteObject, Optional ByVal locationNums As List(Of Integer) = Nothing, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef attemptedServiceCallType As CommercialDataPrefillServiceType = CommercialDataPrefillServiceType.None, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "", Optional ByRef setAnyMods As Boolean = False)
        'for LexisNexis CommDataPrefill
        attemptedServiceCall = False
        attemptedServiceCallType = CommercialDataPrefillServiceType.None
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        setAnyMods = False
        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True Then
            Dim okayToContinue As Boolean = False
            Dim okayForFirm As Boolean = PolicyHolderHasNameAndAddressData(qqo.Policyholder, mustHaveAllRequiredFields:=True)
            Dim locNumsToUse As List(Of Integer) = Nothing
            Dim okayForProp As Boolean = HasLocationWithAddressData(qqo.Locations, locNums:=locNumsToUse, mustHaveAllRequiredFields:=True) AndAlso (locationNums Is Nothing OrElse locationNums.Count = 0 OrElse HasLocationAtAnySpecifiedPosition(qqo.Locations, locationNums, locationMustHaveAddressInfo:=True, locationAddressMustHaveAllRequiredFields:=True, matches:=locNumsToUse) = True)

            If okayForFirm = True AndAlso okayForProp = True Then
                okayToContinue = True
            ElseIf okayForFirm = True Then
                attemptedServiceCallType = CommercialDataPrefillServiceType.FirmographicsOnly
                CallCommercialDataPrefill_FirmographicsOnly(qqo, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, setAnyMods:=setAnyMods)
                Exit Sub
            ElseIf okayForProp = True Then
                attemptedServiceCallType = CommercialDataPrefillServiceType.PropertyOnly
                CallCommercialDataPrefill_PropertyOnly(qqo, locationNums:=locationNums, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, setAnyMods:=setAnyMods)
                Exit Sub
            End If

            If okayToContinue = True Then
                attemptedServiceCall = True
                attemptedServiceCallType = CommercialDataPrefillServiceType.FirmographicsAndProperty

                Dim info As New CommercialDataPrefillAllResponseInfo
                Dim chc As New CommonHelperClass
                Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_APIKey")
                Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_BaseURL")
                Dim prefillRequest As New IFI.Integrations.Request.AllCommercialPrefill(baseUrl, apiKey)
                Try
                    With prefillRequest
                        .PolicyId = chc.IntegerForString(qqo.PolicyId)
                        .PolicyImageNum = chc.IntegerForString(qqo.PolicyImageNum)

                        If LoadedCommercialDataPrefillFirmographicsRequestInfo(qqo.Policyholder, info.firmographicsInfo.requestInfo) = True Then
                            With .FirmographicsData
                                .BusinessName = info.firmographicsInfo.requestInfo.businessName
                                .Address1 = info.firmographicsInfo.requestInfo.address1
                                .City = info.firmographicsInfo.requestInfo.city
                                .State = info.firmographicsInfo.requestInfo.state
                                .Zip = info.firmographicsInfo.requestInfo.zip
                                .OtherInfo = info.firmographicsInfo.requestInfo.otherInfo
                            End With
                        End If
                        With .PropertyData
                            If qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 AndAlso locNumsToUse IsNot Nothing AndAlso locNumsToUse.Count > 0 Then
                                locNumsToUse.Sort()
                                Dim lNum As Integer = 0
                                For Each l As QuickQuoteLocation In qqo.Locations
                                    lNum += 1

                                    Dim ri As CommercialDataPrefillPropertyRequestInfo = Nothing
                                    If locNumsToUse.Contains(lNum) = True AndAlso LoadedCommercialDataPrefillPropertyRequestInfo(l, qqo.Policyholder, ri) = True Then
                                        Dim pi As New CommercialDataPrefillPropertyResponseInfo
                                        pi.requestInfo = ri
                                        info.propertyInfos.Add(pi)
                                        .Add(New IFI.Integrations.Request.Objects.ComPropertyPrefill With {
                                            .FirstName = pi.requestInfo.firstName,
                                            .MiddleName = pi.requestInfo.middleName,
                                            .LastName = pi.requestInfo.lastName,
                                            .StreetNumber = pi.requestInfo.streetNumber,
                                            .StreetName = pi.requestInfo.streetName,
                                            .UnitNumber = pi.requestInfo.unitNumber,
                                            .City = pi.requestInfo.city,
                                            .State = pi.requestInfo.state,
                                            .Zip = pi.requestInfo.zip
                                        })
                                    End If
                                Next
                            End If
                        End With
                    End With
                    Dim response = prefillRequest.GetVendorData()

                    If response IsNot Nothing Then
                        With response

                            ProcessBaseCommercialDataPrefillApiResponseInfo(Of IFI.Integrations.Response.AllCommercialPrefill)(response, info)

                            If .responseData IsNot Nothing Then
                                info.hasResponseData = True
                                With .responseData
                                    If .Firmographics IsNot Nothing Then
                                        ProcessBaseCommercialDataPrefillApiResponseInfo(Of IFI.Integrations.Response.ComFirmoGraphicsPrefill)(.Firmographics, info.firmographicsInfo)
                                        With .Firmographics

                                            ProcessCommercialDataPrefillApiFirmographicsResponseData(.responseData, info.firmographicsInfo)
                                        End With
                                    End If
                                    If .Property IsNot Nothing AndAlso .Property.Count > 0 Then
                                        Dim pCount As Integer = 0
                                        For Each p As IFI.Integrations.Response.Common.ServiceResult(Of IFI.Integrations.Response.ComPropertyPrefill) In .Property
                                            pCount += 1
                                            Dim propInfo As CommercialDataPrefillPropertyResponseInfo = Nothing
                                            If info.propertyInfos IsNot Nothing AndAlso info.propertyInfos.Count >= pCount Then
                                                propInfo = info.propertyInfos(pCount - 1)
                                                If propInfo Is Nothing Then
                                                    propInfo = New CommercialDataPrefillPropertyResponseInfo
                                                End If
                                            Else
                                                propInfo = New CommercialDataPrefillPropertyResponseInfo
                                                info.propertyInfos.Add(propInfo)
                                            End If
                                            If p IsNot Nothing Then
                                                ProcessBaseCommercialDataPrefillApiResponseInfo(Of IFI.Integrations.Response.ComPropertyPrefill)(p, propInfo)
                                                With p
                                                    ProcessCommercialDataPrefillApiPropertyResponseData(.responseData, propInfo)
                                                End With
                                            End If
                                        Next
                                    End If
                                End With
                            End If
                        End With
                    Else 'added so we don't think it was just a NoHit; this would happen if there's an issue w/ the SnapLogic endpoint that's being suppressed by the helper library
                        caughtUnhandledException = True
                        If prefillRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(prefillRequest.InternalExceptionMessages) Then
                            unhandledExceptionToString = "No Response from Integration Service" & "; " & prefillRequest.InternalExceptionMessages
                        Else
                            unhandledExceptionToString = "No Response from Integration Service"
                        End If
                    End If
                Catch ex As Exception
                    caughtUnhandledException = True
                    If prefillRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(prefillRequest.InternalExceptionMessages) Then
                        unhandledExceptionToString = ex.ToString & "; " & prefillRequest.InternalExceptionMessages
                    Else
                        unhandledExceptionToString = ex.ToString
                    End If
                End Try

                info.attemptedServiceCall = attemptedServiceCall
                If caughtUnhandledException = True Then
                    info.caughtException = caughtUnhandledException
                    info.exceptionErrorMessage = unhandledExceptionToString
                End If

                If info.firmographicsInfo.returnedAnyData = True Then
                    info.returnedAnyData = True
                Else
                    If info.propertyInfos IsNot Nothing AndAlso info.propertyInfos.Count > 0 Then
                        For Each pi As CommercialDataPrefillPropertyResponseInfo In info.propertyInfos
                            If pi IsNot Nothing AndAlso pi.returnedAnyData = True Then
                                info.returnedAnyData = True
                                Exit For
                            End If
                        Next
                    End If
                End If

                If caughtUnhandledException = False Then
                    Dim qqHelper As New QuickQuoteHelperClass

                    'Dim strJsonF As String = IntegrationApiHelper.JsonStringForObject(info.firmographicsInfo)
                    'Dim testFirmInfo As CommercialDataPrefillFirmographicsResponseInfo = Nothing
                    'Try
                    '    testFirmInfo = IntegrationApiHelper.ObjectForJsonString(strJsonF, GetType(CommercialDataPrefillFirmographicsResponseInfo))
                    'Catch ex As Exception

                    'End Try
                    'If testFirmInfo IsNot Nothing Then

                    'End If

                    ProcessCommercialDataPrefillFirmographicsReturnData(qqo, info.firmographicsInfo, setAnyMods, qqHelper:=qqHelper)

                    If locNumsToUse IsNot Nothing AndAlso locNumsToUse.Count > 0 AndAlso info.propertyInfos IsNot Nothing AndAlso info.propertyInfos.Count = locNumsToUse.Count Then
                        Dim isOkayToOverwriteFromProperty As Boolean = WebHelper_Personal.CommercialDataPrefill_OkayToOverwriteExistingPropertyDataFromFirmographics()
                        Dim locCounter As Integer = 0
                        For Each lNum As Integer In locNumsToUse
                            locCounter += 1
                            Dim l As QuickQuoteLocation = qqo.Locations(lNum - 1)
                            If l IsNot Nothing Then
                                Dim propInfo As CommercialDataPrefillPropertyResponseInfo = info.propertyInfos(locCounter - 1)
                                'If propInfo IsNot Nothing Then
                                '    'Dim strJsonP As String = IntegrationApiHelper.JsonStringForObject(propInfo)
                                '    'Dim testPropInfo As CommercialDataPrefillPropertyResponseInfo = Nothing
                                '    'Try
                                '    '    testPropInfo = IntegrationApiHelper.ObjectForJsonString(strJsonP, GetType(CommercialDataPrefillPropertyResponseInfo))
                                '    'Catch ex As Exception

                                '    'End Try
                                '    'If testPropInfo IsNot Nothing Then

                                '    'End If
                                'End If
                                ProcessCommercialDataPrefillPropertyReturnData(l, propInfo, setAnyMods, isOkayToOverwriteFromProperty:=isOkayToOverwriteFromProperty, qqHelper:=qqHelper)
                            End If
                        Next
                    End If
                End If
                If caughtUnhandledException = False AndAlso setAnyMods = False Then
                    If info.firmographicsInfo.caughtException = True OrElse info.firmographicsInfo.hasError = True Then
                        caughtUnhandledException = True
                        If info.firmographicsInfo.caughtException = True Then
                            unhandledExceptionToString = info.firmographicsInfo.exceptionErrorMessage
                        Else
                            unhandledExceptionToString = info.firmographicsInfo.errorReason
                        End If
                    Else
                        If info.propertyInfos IsNot Nothing AndAlso info.propertyInfos.Count > 0 Then
                            For Each pi As CommercialDataPrefillPropertyResponseInfo In info.propertyInfos
                                If pi IsNot Nothing AndAlso (pi.caughtException = True OrElse pi.hasError = True) Then
                                    caughtUnhandledException = True
                                    If pi.caughtException = True Then
                                        unhandledExceptionToString = pi.exceptionErrorMessage
                                    Else
                                        unhandledExceptionToString = pi.errorReason
                                    End If
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                End If
            End If
        End If
    End Sub
    Public Function LoadedCommercialDataPrefillFirmographicsRequestInfo(ByVal ph As QuickQuotePolicyholder, ByRef requestInfo As CommercialDataPrefillFirmographicsRequestInfo) As Boolean
        Dim loaded As Boolean = False

        If ph IsNot Nothing AndAlso ((ph.Name IsNot Nothing AndAlso ph.Name.HasData = True) OrElse (ph.Address IsNot Nothing AndAlso ph.Address.HasData = True)) Then
            If requestInfo Is Nothing Then
                requestInfo = New CommercialDataPrefillFirmographicsRequestInfo
            End If
            With requestInfo
                If ph.Name IsNot Nothing AndAlso ph.Name.HasData = True Then
                    .businessName = BusinessNameForCommercialDataPrefill(ph.Name)
                End If
                If ph.Address IsNot Nothing AndAlso ph.Address.HasData = True Then
                    .address1 = GetStreetAddress(ph.Address)
                    .city = ph.Address.City
                    .state = ph.Address.State
                    Dim phZip As String = ph.Address.Zip
                    If Len(phZip) > 5 Then
                        phZip = Left(phZip, 5)
                    End If
                    .zip = phZip
                    .otherInfo = ph.Address.Other
                End If
            End With
            loaded = True
        End If

        Return loaded
    End Function
    Public Function LoadedCommercialDataPrefillPropertyRequestInfo(ByVal l As QuickQuoteLocation, ByVal ph As QuickQuotePolicyholder, ByRef requestInfo As CommercialDataPrefillPropertyRequestInfo) As Boolean
        Dim loaded As Boolean = False

        If l IsNot Nothing AndAlso l.Address IsNot Nothing AndAlso l.Address.HasData = True Then
            If requestInfo Is Nothing Then
                requestInfo = New CommercialDataPrefillPropertyRequestInfo
            End If
            Dim locZip As String = l.Address.Zip
            If Len(locZip) > 5 Then
                locZip = Left(locZip, 5)
            End If
            Dim propLast As String = ""
            Dim propFirst As String = ""
            Dim propMiddle As String = ""
            If ph IsNot Nothing AndAlso ph.Name IsNot Nothing AndAlso ph.Name.HasData = True Then
                SetPersonalNameFieldsForCommercialDataPrefill(ph.Name, propFirst, propMiddle, propLast)
            End If
            With requestInfo
                .firstName = propFirst
                .middleName = propMiddle
                .lastName = propLast
                .streetNumber = l.Address.HouseNum
                .streetName = l.Address.StreetName
                .unitNumber = l.Address.ApartmentNumber
                .city = l.Address.City
                .state = l.Address.State
                .zip = locZip
            End With
            loaded = True
        End If

        Return loaded
    End Function
    Public Sub ProcessBaseCommercialDataPrefillApiResponseInfo(Of T)(ByVal response As IFI.Integrations.Response.Common.ServiceResult(Of T), ByRef info As CommercialDataPrefillResponseInfo)
        If response IsNot Nothing Then
            If info Is Nothing Then
                info = New CommercialDataPrefillResponseInfo
            End If
            info.hasResponse = True
            With response
                ProcessCommercialDataPrefillAPIResponseForClientModel(.APIResponseForClientModel, info)
                ProcessCommercialDataPrefillApiErrorInfo(.errorInfo, info)
                If .hasError = True Then
                    info.hasError = True
                End If
                If .isCachedResult = True Then

                End If
                If .isNoHit = True Then
                    info.noHit = True
                End If
                If .isReorder = True Then

                End If
                If .responseData IsNot Nothing Then
                    'If TypeOf .responseData Is IFI.Integrations.Response.AllCommercialPrefill Then

                    'ElseIf TypeOf .responseData Is IFI.Integrations.Response.LNComFirm AndAlso TypeOf info Is CommercialDataPrefillFirmographicsResponseInfo Then
                    '    ProcessCommercialDataPrefillApiFirmographicsResponseData(.responseData, info)
                    '    ProcessCommercialDataPrefillApiFirmographicsResponseData(CType(.responseData, IFI.Integrations.Response.LNComFirm), CType(info, CommercialDataPrefillFirmographicsResponseInfo))
                    'ElseIf TypeOf .responseData Is IFI.Integrations.Response.ComFirmoGraphicsPrefill AndAlso TypeOf info Is CommercialDataPrefillFirmographicsResponseInfo Then
                    '    ProcessCommercialDataPrefillApiFirmographicsResponseData(.responseData, info)
                    '    ProcessCommercialDataPrefillApiFirmographicsResponseData(CType(.responseData, IFI.Integrations.Response.ComFirmoGraphicsPrefill), CType(info, CommercialDataPrefillFirmographicsResponseInfo))
                    'ElseIf TypeOf .responseData Is IFI.Integrations.Response.ComPropertyPrefill AndAlso TypeOf info Is CommercialDataPrefillPropertyResponseInfo Then
                    '    ProcessCommercialDataPrefillApiPropertyResponseData(.responseData, info)
                    '    ProcessCommercialDataPrefillApiPropertyResponseData(CType(.responseData, IFI.Integrations.Response.ComFirmoGraphicsPrefill), CType(info, CommercialDataPrefillPropertyResponseInfo))
                    'End If
                End If
            End With
        End If

        If info.hasError = True AndAlso ErrorMessageLooksLikeCommercialDataPrefillNoHit(info.errorReason) = True Then
            info.hasError = False
            info.noHit = True
        End If
    End Sub
    Public Sub ProcessBaseCommercialDataPrefillApiPreloadResponseInfo(ByVal response As IFI.Integrations.Response.Common.ServiceResultPreLoad, ByRef info As CommercialDataPrefillPreloadResponseInfo)
        If response IsNot Nothing Then
            If info Is Nothing Then
                info = New CommercialDataPrefillPreloadResponseInfo
            End If
            info.hasResponse = True
            With response
                ProcessCommercialDataPrefillAPIResponseForClientModel(.APIResponseForClientModel, info)
                If String.IsNullOrWhiteSpace(.errorMessage) = False Then
                    info.errorMessage = .errorMessage
                End If
                If .hasError = True Then
                    info.hasError = True
                End If
                If .requestReceived = True Then
                    info.requestReceived = True
                End If
            End With
        End If
    End Sub
    Public Sub ProcessCommercialDataPrefillAPIResponseForClientModel(ByVal resp As IFI.Integrations.Response.Common.APIResponseForClientModel, ByRef info As CommercialDataPrefillBaseResponseInfo)
        If resp IsNot Nothing Then
            If info Is Nothing Then
                info = New CommercialDataPrefillBaseResponseInfo
            End If
            With resp
                If .Exception IsNot Nothing Then
                    info.exceptionErrorMessage = .Exception.ToString
                    If String.IsNullOrWhiteSpace(info.exceptionErrorMessage) = False Then
                        info.caughtException = True
                    End If
                End If
                If .ExceptionCaught = True Then
                    info.caughtException = True
                End If
                If .IsResponseNull = True Then

                End If
                If String.IsNullOrWhiteSpace(.ResponseDebugErrorMessage) = False Then

                End If
                If .ResponseDeserialized = True Then

                End If
                If String.IsNullOrWhiteSpace(.ResponseMessage) = False Then

                End If
                If String.IsNullOrWhiteSpace(.ResponseStatusCode) = False Then

                End If
            End With
        End If
    End Sub
    Public Sub ProcessCommercialDataPrefillApiErrorInfo(ByVal errorInfo As IFI.Integrations.Response.Common.ErrorInfo, ByRef info As CommercialDataPrefillResponseInfo)
        If errorInfo IsNot Nothing Then
            If info Is Nothing Then
                info = New CommercialDataPrefillResponseInfo
            End If
            With errorInfo
                If .isTimeout = True Then

                End If
                If .isUnrecoverable = True Then

                End If
                If .isValidation = True Then

                End If
                If String.IsNullOrWhiteSpace(.extraDetail) = False Then

                End If
                If String.IsNullOrWhiteSpace(.reason) = False Then
                    info.errorReason = .reason
                End If
                If String.IsNullOrWhiteSpace(.vendorStatusCode) = False Then

                End If
            End With
        End If
    End Sub
    Public Sub ProcessCommercialDataPrefillApiFirmographicsResponseData(ByVal responseData As IFI.Integrations.Response.LNComFirm, ByRef info As CommercialDataPrefillFirmographicsResponseInfo)
        'note: see overload below for other Firmographics API response object; same props but different type
        If responseData IsNot Nothing Then
            If info Is Nothing Then
                info = New CommercialDataPrefillFirmographicsResponseInfo
            End If
            info.hasResponseData = True
            With responseData
                If String.IsNullOrWhiteSpace(.BusinessName) = False Then
                    info.returnedData.businessName = .BusinessName
                    If info.requestInfo Is Nothing OrElse String.IsNullOrWhiteSpace(info.requestInfo.businessName) = True OrElse UCase(info.requestInfo.businessName) <> UCase(.BusinessName) Then
                        info.returnedAnyData = True
                    End If
                End If
                If String.IsNullOrWhiteSpace(.DBA) = False Then
                    info.returnedData.dba = .DBA
                    info.returnedAnyData = True
                End If
                If String.IsNullOrWhiteSpace(.FEIN) = False Then
                    info.returnedData.fein = .FEIN
                    info.returnedAnyData = True
                End If
                If String.IsNullOrWhiteSpace(.LegalEntity) = False Then
                    info.returnedData.legalEntity = .LegalEntity
                    If UCase(.LegalEntity) <> "OTHER" Then
                        info.returnedAnyData = True
                    End If
                End If
                If String.IsNullOrWhiteSpace(.NAICS) = False Then
                    info.returnedData.naics = .NAICS
                    info.returnedAnyData = True
                End If
                'If String.IsNullOrWhiteSpace(.OtherEntity) = False Then
                '    info.returnedData.otherEntity = .OtherEntity
                '    info.returnedAnyData = True
                'End If
                If String.IsNullOrWhiteSpace(.OtherLegalEntity) = False Then
                    info.returnedData.otherLegalEntity = .OtherLegalEntity
                    info.returnedAnyData = True
                End If
                If String.IsNullOrWhiteSpace(.Phone) = False Then
                    info.returnedData.phone = .Phone
                    info.returnedAnyData = True
                End If
                If String.IsNullOrWhiteSpace(.URL) = False Then
                    info.returnedData.url = .URL
                    info.returnedAnyData = True
                End If
                If String.IsNullOrWhiteSpace(.YearStarted) = False Then
                    info.returnedData.yearStarted = .YearStarted
                    Dim qqHelper As New QuickQuoteHelperClass
                    If qqHelper.IsValidDateString(.YearStarted, mustBeGreaterThanDefaultDate:=True) = True Then
                        info.returnedAnyData = True
                    End If
                End If
            End With
            If info.hasError = False AndAlso info.returnedAnyData = False Then
                info.noHit = True
                If String.IsNullOrWhiteSpace(info.returnedData.legalEntity) = False AndAlso UCase(info.returnedData.legalEntity) = "OTHER" Then
                    'wipe out if it's still being returned w/ "No Hit"
                    info.returnedData.legalEntity = ""
                End If
                If String.IsNullOrWhiteSpace(info.returnedData.yearStarted) = False Then
                    Dim qqHelper As New QuickQuoteHelperClass
                    If qqHelper.IsValidDateString(info.returnedData.yearStarted, mustBeGreaterThanDefaultDate:=True) = False Then
                        info.returnedData.yearStarted = ""
                    End If
                End If
            End If
        End If
    End Sub
    Public Sub ProcessCommercialDataPrefillApiFirmographicsResponseData(ByVal responseData As IFI.Integrations.Response.ComFirmoGraphicsPrefill, ByRef info As CommercialDataPrefillFirmographicsResponseInfo)
        'note: see overload below for other Firmographics API response object; same props but different type
        If responseData IsNot Nothing Then
            If info Is Nothing Then
                info = New CommercialDataPrefillFirmographicsResponseInfo
            End If
            info.hasResponseData = True
            With responseData
                If String.IsNullOrWhiteSpace(.BusinessName) = False Then
                    info.returnedData.businessName = .BusinessName
                    If info.requestInfo Is Nothing OrElse String.IsNullOrWhiteSpace(info.requestInfo.businessName) = True OrElse UCase(info.requestInfo.businessName) <> UCase(.BusinessName) Then
                        info.returnedAnyData = True
                    End If
                End If
                If String.IsNullOrWhiteSpace(.DBA) = False Then
                    info.returnedData.dba = .DBA
                    info.returnedAnyData = True
                End If
                If String.IsNullOrWhiteSpace(.FEIN) = False Then
                    info.returnedData.fein = .FEIN
                    info.returnedAnyData = True
                End If
                If String.IsNullOrWhiteSpace(.LegalEntity) = False Then
                    info.returnedData.legalEntity = .LegalEntity
                    If UCase(.LegalEntity) <> "OTHER" Then
                        info.returnedAnyData = True
                    End If
                End If
                If String.IsNullOrWhiteSpace(.NAICS) = False Then
                    info.returnedData.naics = .NAICS
                    info.returnedAnyData = True
                End If
                'If String.IsNullOrWhiteSpace(.OtherEntity) = False Then
                '    info.returnedData.otherEntity = .OtherEntity
                '    info.returnedAnyData = True
                'End If
                If String.IsNullOrWhiteSpace(.OtherLegalEntity) = False Then
                    info.returnedData.otherLegalEntity = .OtherLegalEntity
                    info.returnedAnyData = True
                End If
                If String.IsNullOrWhiteSpace(.Phone) = False Then
                    info.returnedData.phone = .Phone
                    info.returnedAnyData = True
                End If
                If String.IsNullOrWhiteSpace(.URL) = False Then
                    info.returnedData.url = .URL
                    info.returnedAnyData = True
                End If
                If String.IsNullOrWhiteSpace(.YearStarted) = False Then
                    info.returnedData.yearStarted = .YearStarted
                    Dim qqHelper As New QuickQuoteHelperClass
                    If qqHelper.IsValidDateString(.YearStarted, mustBeGreaterThanDefaultDate:=True) = True Then
                        info.returnedAnyData = True
                    End If
                End If
            End With
            If info.hasError = False AndAlso info.returnedAnyData = False Then
                info.noHit = True
                If String.IsNullOrWhiteSpace(info.returnedData.legalEntity) = False AndAlso UCase(info.returnedData.legalEntity) = "OTHER" Then
                    'wipe out if it's still being returned w/ "No Hit"
                    info.returnedData.legalEntity = ""
                End If
                If String.IsNullOrWhiteSpace(info.returnedData.yearStarted) = False Then
                    Dim qqHelper As New QuickQuoteHelperClass
                    If qqHelper.IsValidDateString(info.returnedData.yearStarted, mustBeGreaterThanDefaultDate:=True) = False Then
                        info.returnedData.yearStarted = ""
                    End If
                End If
            End If
        End If
    End Sub
    Public Sub ProcessCommercialDataPrefillApiPropertyResponseData(ByVal responseData As IFI.Integrations.Response.ComPropertyPrefill, ByRef info As CommercialDataPrefillPropertyResponseInfo)
        If responseData IsNot Nothing Then
            If info Is Nothing Then
                info = New CommercialDataPrefillPropertyResponseInfo
            End If
            info.hasResponseData = True
            With responseData
                If .hasFireplaces = True Then
                    info.returnedData.hasFireplaces = True
                    info.returnedAnyData = True
                End If
                If String.IsNullOrWhiteSpace(.municipalityOrTownship) = False Then
                    info.returnedData.municipalityOrTownship = .municipalityOrTownship
                    info.returnedAnyData = True
                End If
                If .pools > 0 Then
                    info.returnedData.pools = .pools
                    info.returnedAnyData = True
                End If
                If .squareFeet > 0 Then
                    info.returnedData.squareFeet = .squareFeet
                    info.returnedAnyData = True
                End If
                If .stories > 0 Then
                    info.returnedData.stories = .stories
                    info.returnedAnyData = True
                End If
                If .yearBuilt > 0 Then
                    info.returnedData.yearBuilt = .yearBuilt
                    info.returnedAnyData = True
                End If
            End With
            If info.hasError = False AndAlso info.returnedAnyData = False Then
                info.noHit = True
            End If
        End If
    End Sub
    Public Sub ProcessCommercialDataPrefillFirmographicsReturnData_Old(ByRef qqo As QuickQuoteObject, ByRef firmResponse As CommercialDataPrefillFirmographicsResponseInfo, ByRef setAnyModifiers As Boolean, Optional ByRef qqHelper As QuickQuoteHelperClass = Nothing)
        If qqo IsNot Nothing AndAlso firmResponse IsNot Nothing Then
            If qqo.Policyholder IsNot Nothing AndAlso ((firmResponse.hasError = False AndAlso firmResponse.caughtException = False) OrElse firmResponse.returnedAnyData = True) Then
                Dim isOkayToOverwriteFromFirmographics As Boolean = WebHelper_Personal.CommercialDataPrefill_OkayToOverwriteExistingPolicyholderDataFromFirmographics()
                If qqHelper Is Nothing Then
                    qqHelper = New QuickQuoteHelperClass
                End If
                With qqo.Policyholder
                    If .Name IsNot Nothing Then
                        With .Name
                            qqHelper.SetValueIfNotSet(.DoingBusinessAsName, WebHelper_Personal.TextInUpperCaseAndTrimmed(firmResponse.returnedData.dba), okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            qqHelper.SetValueIfNotSet(.TaxNumber, firmResponse.returnedData.fein, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            If String.IsNullOrWhiteSpace(firmResponse.returnedData.fein) = False Then
                                WebHelper_Personal.SetValueIfNotSet_Local(.TaxTypeId, "2", onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.PositiveIntegerType)
                            End If
                            If String.IsNullOrWhiteSpace(firmResponse.returnedData.legalEntity) = False Then
                                Dim firmEntityTypeId As String = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.EntityTypeId, firmResponse.returnedData.legalEntity)
                                WebHelper_Personal.SetValueIfNotSet_Local(.EntityTypeId, firmEntityTypeId, onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.PositiveIntegerType, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            End If
                            'qqHelper.SetValueIfNotSet(.OtherLegalEntityDescription, firmResponse.returnedData.otherEntity, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            qqHelper.SetValueIfNotSet(.OtherLegalEntityDescription, firmResponse.returnedData.otherLegalEntity, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            'naics
                            'url
                            If qqHelper.IsValidDateString(firmResponse.returnedData.yearStarted, mustBeGreaterThanDefaultDate:=True) = True Then
                                'WebHelper_Personal.SetValueIfNotSet_Local(.DateBusinessStarted, info.returnedData.yearStarted, onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.DateType, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                                Dim okayToUpdateDateBusStarted As Boolean = True
                                If isOkayToOverwriteFromFirmographics = True Then
                                    If qqHelper.IsValidDateString(.DateBusinessStarted, mustBeGreaterThanDefaultDate:=True) = True Then
                                        Dim dtCurr As Date = CDate(.DateBusinessStarted)
                                        Dim dtFirm As Date = CDate(firmResponse.returnedData.yearStarted)
                                        If dtCurr.Year = dtFirm.Year AndAlso dtFirm.Month = 1 AndAlso dtFirm.Day = 1 AndAlso (dtCurr.Month <> 1 OrElse dtCurr.Day <> 1) Then
                                            'same year, but firmographics just has 1st day of year whereas we already have the full date
                                            okayToUpdateDateBusStarted = False
                                        End If
                                    End If
                                End If
                                If okayToUpdateDateBusStarted = True Then
                                    WebHelper_Personal.SetValueIfNotSet_Local(.DateBusinessStarted, firmResponse.returnedData.yearStarted, onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.DateType, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                                End If
                            End If
                        End With
                    End If
                    If String.IsNullOrWhiteSpace(firmResponse.returnedData.phone) = False Then
                        If .Phones Is Nothing Then
                            .Phones = New List(Of QuickQuotePhone)
                        End If
                        If .Phones.Count < 1 Then
                            .Phones.Add(New QuickQuotePhone)
                        End If
                        If .Phones(0) Is Nothing Then
                            .Phones(0) = New QuickQuotePhone
                        End If
                        With .Phones(0)
                            qqHelper.SetValueIfNotSet(.Number, firmResponse.returnedData.phone, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            WebHelper_Personal.SetValueIfNotSet_Local(.TypeId, "2", onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.PositiveIntegerType, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True) 'default to Business
                        End With
                    End If
                End With

                firmResponse.returnedData.HasOrderedPrefill = True
                firmResponse.returnedData.OrderInformation = FirmographicsPrefillNameAddressString(qqo.Policyholder)
                SetCommercialDataPrefillModifiersOnPolicy(qqo, firmResponse.returnedData)
                setAnyModifiers = True
            End If
        End If
    End Sub
    Public Sub ProcessCommercialDataPrefillFirmographicsReturnData(ByRef qqo As QuickQuoteObject, ByRef firmResponse As CommercialDataPrefillFirmographicsResponseInfo, ByRef setAnyModifiers As Boolean, Optional ByRef qqHelper As QuickQuoteHelperClass = Nothing)
        If qqo IsNot Nothing AndAlso firmResponse IsNot Nothing Then
            If qqo.Policyholder IsNot Nothing AndAlso ((firmResponse.hasError = False AndAlso firmResponse.caughtException = False) OrElse firmResponse.returnedAnyData = True OrElse firmResponse.noHit = True) Then
                Dim isOkayToOverwriteFromFirmographics As Boolean = WebHelper_Personal.CommercialDataPrefill_OkayToOverwriteExistingPolicyholderDataFromFirmographics()
                If qqHelper Is Nothing Then
                    qqHelper = New QuickQuoteHelperClass
                End If

                'Process NAICS Code from Firmo
                Dim naics = firmResponse.returnedData?.naics
                If Not String.IsNullOrWhiteSpace(naics) Then
                    'Discussed with KW, always over-write NAICS here. Ignore what Client/UW has already in NAICS.
                    qqo.Policyholder.Name.NAICS = naics
                    qqo.SetDevDictionaryItem("", "NAICS", naics)
                    qqo.SetDevDictionaryItem("", "NAICSdescription", NaicsCodeHelper.LookUpNaicsDescription(naics))
                End If

                Dim wipeOutExisting As WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType = WebHelper_Personal.CommercialDataPrefill_Firmographics_NoHit_WipeOutExisting()
                Dim noHitQualification As WebHelper_Personal.CommercialDataPrefill_NoHit_QualificationType = WebHelper_Personal.CommercialDataPrefill_NoHit_QualificationType.NoHitOrBlankField
                Dim existingOrderInfo As CommercialDataPrefillFirmographicsInfo = Nothing
                If wipeOutExisting <> WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType.Never Then
                    noHitQualification = WebHelper_Personal.CommercialDataPrefill_Firmographics_NoHit_Qualification()
                    If noHitQualification <> WebHelper_Personal.CommercialDataPrefill_NoHit_QualificationType.NoHitOnly OrElse firmResponse.noHit = True Then
                        If wipeOutExisting = WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType.OnlyWhenExistingMatchesPreviousOrder Then
                            Dim hasExistingOrder As Boolean = HasPolicyCommercialDataPrefillOrder(qqo, populateAllFirmographicsInfoIfOrdered:=True, firmographicsInfo:=existingOrderInfo)
                        End If
                    End If
                End If
                If existingOrderInfo Is Nothing Then
                    existingOrderInfo = New CommercialDataPrefillFirmographicsInfo
                    existingOrderInfo.HasOrderedPrefill = False
                End If
                With qqo.Policyholder
                    If .Name IsNot Nothing Then
                        With .Name
                            'If wipeOutExisting = WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType.Never Then
                            '    qqHelper.SetValueIfNotSet(.DoingBusinessAsName, WebHelper_Personal.TextInUpperCaseAndTrimmed(firmResponse.returnedData.dba), okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            'Else
                            '    If firmResponse.noHit = True OrElse (noHitQualification = WebHelper_Personal.CommercialDataPrefill_NoHit_QualificationType.NoHitOrBlankField AndAlso String.IsNullOrWhiteSpace(firmResponse.returnedData.dba) = True) Then
                            '        'no hit
                            '        If wipeOutExisting = WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType.Always Then
                            '            'could wipe out; would use default value if specific to type desired; need to force okayToOverwrite to True and neverSetItNotValid to False - was thinking I could accomplish what we need via SetValueIfNotSet by manipulating params, but that method will never allow for wiping out a value that's currently in place
                            '            .DoingBusinessAsName = WebHelper_Personal.TextInUpperCaseAndTrimmed(firmResponse.returnedData.dba)
                            '        ElseIf wipeOutExisting = WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType.OnlyWhenExistingMatchesPreviousOrder Then
                            '            If existingOrderInfo.HasOrderedPrefill = True AndAlso QuickQuoteHelperClass.isTextMatch(.DoingBusinessAsName, existingOrderInfo.dba, matchType:=QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing) = True Then
                            '                'could wipe out; would use default value if specific to type desired; need to force okayToOverwrite to True and neverSetItNotValid to False - was thinking I could accomplish what we need via SetValueIfNotSet by manipulating params, but that method will never allow for wiping out a value that's currently in place
                            '                .DoingBusinessAsName = WebHelper_Personal.TextInUpperCaseAndTrimmed(firmResponse.returnedData.dba)
                            '            Else
                            '                'no wipe out needed; run normal logic
                            '                qqHelper.SetValueIfNotSet(.DoingBusinessAsName, WebHelper_Personal.TextInUpperCaseAndTrimmed(firmResponse.returnedData.dba), okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            '            End If
                            '        End If
                            '    Else
                            '        'no wipe out needed; run normal logic
                            '        qqHelper.SetValueIfNotSet(.DoingBusinessAsName, WebHelper_Personal.TextInUpperCaseAndTrimmed(firmResponse.returnedData.dba), okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            '    End If
                            'End If
                            WebHelper_Personal.SetValueForCommercialDataPrefill(.DoingBusinessAsName, firmResponse.returnedData.dba, firmResponse.noHit, wipeOutExisting, noHitQualification, existingOrderInfo.HasOrderedPrefill, existingOrderInfo.dba, useUpperCaseAndTrimWhenSetting:=True, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            WebHelper_Personal.SetValueForCommercialDataPrefill(.TaxNumber, firmResponse.returnedData.fein, firmResponse.noHit, wipeOutExisting, noHitQualification, existingOrderInfo.HasOrderedPrefill, existingOrderInfo.fein, useUpperCaseAndTrimWhenSetting:=False, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            If String.IsNullOrWhiteSpace(firmResponse.returnedData.fein) = False Then
                                WebHelper_Personal.SetValueIfNotSet_Local(.TaxTypeId, "2", onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.PositiveIntegerType)
                            End If
                            If String.IsNullOrWhiteSpace(firmResponse.returnedData.legalEntity) = False Then
                                Dim firmEntityTypeId As String = qqHelper.GetStaticDataValueForText(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.EntityTypeId, firmResponse.returnedData.legalEntity)
                                WebHelper_Personal.SetValueIfNotSet_Local(.EntityTypeId, firmEntityTypeId, onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.PositiveIntegerType, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            Else
                                If wipeOutExisting <> WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType.Never Then
                                    If firmResponse.noHit = True OrElse noHitQualification = WebHelper_Personal.CommercialDataPrefill_NoHit_QualificationType.NoHitOrBlankField Then
                                        If wipeOutExisting = CommercialDataPrefill_NoHit_WipeOutExistingType.Always Then
                                            .EntityTypeId = ""
                                        ElseIf wipeOutExisting = WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType.OnlyWhenExistingMatchesPreviousOrder Then
                                            If existingOrderInfo.HasOrderedPrefill = True Then
                                                Dim existingEntityTxt As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteName, QuickQuoteHelperClass.QuickQuotePropertyName.EntityTypeId, .EntityTypeId)
                                                If QuickQuoteHelperClass.isTextMatch(existingOrderInfo.legalEntity, existingEntityTxt, matchType:=QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing) = True Then
                                                    .EntityTypeId = ""
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                            'WebHelper_Personal.SetValueForCommercialDataPrefill(.OtherLegalEntityDescription, firmResponse.returnedData.otherEntity, firmResponse.noHit, wipeOutExisting, noHitQualification, existingOrderInfo.HasOrderedPrefill, existingOrderInfo.otherEntity, useUpperCaseAndTrimWhenSetting:=False, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            WebHelper_Personal.SetValueForCommercialDataPrefill(.OtherLegalEntityDescription, firmResponse.returnedData.otherLegalEntity, firmResponse.noHit, wipeOutExisting, noHitQualification, existingOrderInfo.HasOrderedPrefill, existingOrderInfo.otherLegalEntity, useUpperCaseAndTrimWhenSetting:=False, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            'naics
                            'url
                            If qqHelper.IsValidDateString(firmResponse.returnedData.yearStarted, mustBeGreaterThanDefaultDate:=True) = True Then
                                Dim okayToUpdateDateBusStarted As Boolean = True
                                If isOkayToOverwriteFromFirmographics = True Then
                                    If qqHelper.IsValidDateString(.DateBusinessStarted, mustBeGreaterThanDefaultDate:=True) = True Then
                                        Dim dtCurr As Date = CDate(.DateBusinessStarted)
                                        Dim dtFirm As Date = CDate(firmResponse.returnedData.yearStarted)
                                        If dtCurr.Year = dtFirm.Year AndAlso dtFirm.Month = 1 AndAlso dtFirm.Day = 1 AndAlso (dtCurr.Month <> 1 OrElse dtCurr.Day <> 1) Then
                                            'same year, but firmographics just has 1st day of year whereas we already have the full date
                                            okayToUpdateDateBusStarted = False
                                        End If
                                    End If
                                End If
                                If okayToUpdateDateBusStarted = True Then
                                    WebHelper_Personal.SetValueIfNotSet_Local(.DateBusinessStarted, firmResponse.returnedData.yearStarted, onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.DateType, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                                End If
                            Else
                                If wipeOutExisting <> WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType.Never Then
                                    If firmResponse.noHit = True OrElse noHitQualification = WebHelper_Personal.CommercialDataPrefill_NoHit_QualificationType.NoHitOrBlankField Then
                                        If wipeOutExisting = CommercialDataPrefill_NoHit_WipeOutExistingType.Always Then
                                            .DateBusinessStarted = ""
                                        ElseIf wipeOutExisting = WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType.OnlyWhenExistingMatchesPreviousOrder Then
                                            If existingOrderInfo.HasOrderedPrefill = True AndAlso QuickQuoteHelperClass.isTextMatch(.DateBusinessStarted, existingOrderInfo.yearStarted, matchType:=QuickQuoteHelperClass.TextMatchType.DateOrText_IgnoreCasing) = True Then
                                                .DateBusinessStarted = ""
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End With
                    End If
                    Dim phoneNum As String = firmResponse.returnedData.phone
                    If String.IsNullOrWhiteSpace(phoneNum) = False Then
                        If .Phones Is Nothing Then
                            .Phones = New List(Of QuickQuotePhone)
                        End If
                        If .Phones.Count < 1 Then
                            .Phones.Add(New QuickQuotePhone)
                        End If
                        If .Phones(0) Is Nothing Then
                            .Phones(0) = New QuickQuotePhone
                        End If
                        With .Phones(0)
                            WebHelper_Personal.SetValueIfNotSet_Local(.TypeId, "2", onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.PositiveIntegerType, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True) 'default to Business
                        End With
                        If Len(phoneNum) = 10 AndAlso IsNumeric(phoneNum) = True Then
                            phoneNum = "(" & Left(phoneNum, 3) & ")" & Mid(phoneNum, 4, 3) & "-" & Right(phoneNum, 4)
                        End If
                    End If
                    If .Phones IsNot Nothing AndAlso .Phones.Count > 0 AndAlso .Phones(0) IsNot Nothing Then
                        With .Phones(0)
                            Dim hadPhoneNumBefore As Boolean = Not String.IsNullOrWhiteSpace(.Number)
                            WebHelper_Personal.SetValueForCommercialDataPrefill(.Number, phoneNum, firmResponse.noHit, wipeOutExisting, noHitQualification, existingOrderInfo.HasOrderedPrefill, existingOrderInfo.phone, useUpperCaseAndTrimWhenSetting:=False, okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                            If hadPhoneNumBefore = True AndAlso String.IsNullOrWhiteSpace(.Number) = True Then
                                'clear out phone type if clearing out #
                                .TypeId = ""
                            End If
                        End With
                    End If
                End With

                firmResponse.returnedData.HasOrderedPrefill = True
                firmResponse.returnedData.OrderInformation = FirmographicsPrefillNameAddressString(qqo.Policyholder)
                SetCommercialDataPrefillModifiersOnPolicy(qqo, firmResponse.returnedData)
                setAnyModifiers = True
            End If
        End If
    End Sub
    Public Sub ProcessCommercialDataPrefillPropertyReturnData_Old(ByRef l As QuickQuoteLocation, ByRef propResponse As CommercialDataPrefillPropertyResponseInfo, ByRef setAnyModifiers As Boolean, Optional ByRef isOkayToOverwriteFromProperty As Nullable(Of Boolean) = Nothing, Optional ByRef qqHelper As QuickQuoteHelperClass = Nothing)
        If l IsNot Nothing AndAlso propResponse IsNot Nothing AndAlso ((propResponse.hasError = False AndAlso propResponse.caughtException = False) OrElse propResponse.returnedAnyData = True) Then
            If qqHelper Is Nothing Then
                qqHelper = New QuickQuoteHelperClass
            End If
            If isOkayToOverwriteFromProperty Is Nothing Then
                isOkayToOverwriteFromProperty = WebHelper_Personal.CommercialDataPrefill_OkayToOverwriteExistingPropertyDataFromFirmographics()
            End If
            With l
                If l.Address IsNot Nothing Then
                    With l.Address
                        qqHelper.SetValueIfNotSet(.Township, WebHelper_Personal.TextInUpperCaseAndTrimmed(propResponse.returnedData.municipalityOrTownship), okayToOverwrite:=isOkayToOverwriteFromProperty, neverSetItNotValid:=True)
                    End With
                End If
                'If .Acreages IsNot Nothing AndAlso .Acreages.Count > 0 AndAlso .Acreages(0) IsNot Nothing Then
                '    With .Acreages(0)
                '        qqHelper.SetValueIfNotSet(.Twp, propResponse.returnedData.municipalityOrTownship, okayToOverwrite:=isOkayToOverwriteFromProperty, neverSetItNotValid:=True)
                '        qqHelper.SetValueIfNotSet(.TownshipCodeTypeId, propResponse.returnedData.municipalityOrTownship, okayToOverwrite:=isOkayToOverwriteFromProperty, neverSetItNotValid:=True)
                '    End With
                'End If
                If qqHelper.IsPositiveIntegerString(propResponse.returnedData.squareFeet) = True OrElse qqHelper.IsPositiveIntegerString(propResponse.returnedData.stories) = True OrElse qqHelper.IsPositiveIntegerString(propResponse.returnedData.yearBuilt) = True Then
                    If .Buildings Is Nothing Then
                        .Buildings = New List(Of QuickQuoteBuilding)
                    End If
                    If .Buildings.Count < 1 Then
                        .Buildings.Add(New QuickQuoteBuilding)
                    End If
                    If .Buildings(0) Is Nothing Then
                        .Buildings(0) = New QuickQuoteBuilding
                    End If
                    With .Buildings(0)
                        WebHelper_Personal.SetValueIfNotSet_Local(.SquareFeet, propResponse.returnedData.squareFeet.ToString, onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.PositiveIntegerType, okayToOverwrite:=isOkayToOverwriteFromProperty, neverSetItNotValid:=True)
                        WebHelper_Personal.SetValueIfNotSet_Local(.NumberOfStories, propResponse.returnedData.stories.ToString, onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.PositiveIntegerType, okayToOverwrite:=isOkayToOverwriteFromProperty, neverSetItNotValid:=True)
                        WebHelper_Personal.SetValueIfNotSet_Local(.YearBuilt, propResponse.returnedData.yearBuilt.ToString, onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.PositiveIntegerType, okayToOverwrite:=isOkayToOverwriteFromProperty, neverSetItNotValid:=True)
                    End With
                End If
            End With
            propResponse.returnedData.HasOrderedPrefill = True
            propResponse.returnedData.OrderInformation = PropertyPrefillAddressString(l.Address)
            SetCommercialDataPrefillModifiersOnLocation(l, propResponse.returnedData)
            setAnyModifiers = True
        End If
    End Sub
    Public Sub ProcessCommercialDataPrefillPropertyReturnData(ByRef l As QuickQuoteLocation, ByRef propResponse As CommercialDataPrefillPropertyResponseInfo, ByRef setAnyModifiers As Boolean, Optional ByRef isOkayToOverwriteFromProperty As Nullable(Of Boolean) = Nothing, Optional ByRef qqHelper As QuickQuoteHelperClass = Nothing)
        If l IsNot Nothing AndAlso propResponse IsNot Nothing AndAlso ((propResponse.hasError = False AndAlso propResponse.caughtException = False) OrElse propResponse.returnedAnyData = True OrElse propResponse.noHit = True) Then
            If qqHelper Is Nothing Then
                qqHelper = New QuickQuoteHelperClass
            End If
            If isOkayToOverwriteFromProperty Is Nothing Then
                isOkayToOverwriteFromProperty = WebHelper_Personal.CommercialDataPrefill_OkayToOverwriteExistingPropertyDataFromFirmographics()
            End If

            Dim wipeOutExisting As WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType = WebHelper_Personal.CommercialDataPrefill_Property_NoHit_WipeOutExisting()
            Dim noHitQualification As WebHelper_Personal.CommercialDataPrefill_NoHit_QualificationType = WebHelper_Personal.CommercialDataPrefill_NoHit_QualificationType.NoHitOrBlankField
            Dim existingOrderInfo As CommercialDataPrefillPropertyInfo = Nothing
            If wipeOutExisting <> WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType.Never Then
                noHitQualification = WebHelper_Personal.CommercialDataPrefill_Property_NoHit_Qualification()
                If noHitQualification <> WebHelper_Personal.CommercialDataPrefill_NoHit_QualificationType.NoHitOnly OrElse propResponse.noHit = True Then
                    If wipeOutExisting = WebHelper_Personal.CommercialDataPrefill_NoHit_WipeOutExistingType.OnlyWhenExistingMatchesPreviousOrder Then
                        Dim hasExistingOrder As Boolean = HasLocationCommercialDataPrefillOrder(l, populateAllPropertyInfoIfOrdered:=True, propertyInfo:=existingOrderInfo)
                    End If
                End If
            End If
            If existingOrderInfo Is Nothing Then
                existingOrderInfo = New CommercialDataPrefillPropertyInfo
                existingOrderInfo.HasOrderedPrefill = False
            End If

            With l
                'If l.Address IsNot Nothing Then
                '    With l.Address
                '        WebHelper_Personal.SetValueForCommercialDataPrefill(.Township, propResponse.returnedData.municipalityOrTownship, propResponse.noHit, wipeOutExisting, noHitQualification, existingOrderInfo.HasOrderedPrefill, existingOrderInfo.municipalityOrTownship, useUpperCaseAndTrimWhenSetting:=True, okayToOverwrite:=isOkayToOverwriteFromProperty, neverSetItNotValid:=True)
                '    End With
                'End If
                'If .Acreages IsNot Nothing AndAlso .Acreages.Count > 0 AndAlso .Acreages(0) IsNot Nothing Then
                '    With .Acreages(0)
                '        qqHelper.SetValueIfNotSet(.Twp, propResponse.returnedData.municipalityOrTownship, okayToOverwrite:=isOkayToOverwriteFromProperty, neverSetItNotValid:=True)
                '        qqHelper.SetValueIfNotSet(.TownshipCodeTypeId, propResponse.returnedData.municipalityOrTownship, okayToOverwrite:=isOkayToOverwriteFromProperty, neverSetItNotValid:=True)
                '    End With
                'End If
                If qqHelper.IsPositiveIntegerString(propResponse.returnedData.squareFeet) = True OrElse qqHelper.IsPositiveIntegerString(propResponse.returnedData.stories) = True OrElse qqHelper.IsPositiveIntegerString(propResponse.returnedData.yearBuilt) = True Then
                    If .Buildings Is Nothing Then
                        .Buildings = New List(Of QuickQuoteBuilding)
                    End If
                    If .Buildings.Count < 1 Then
                        .Buildings.Add(New QuickQuoteBuilding)
                    End If
                    If .Buildings(0) Is Nothing Then
                        .Buildings(0) = New QuickQuoteBuilding
                    End If
                End If
                If .Buildings IsNot Nothing AndAlso .Buildings.Count > 0 AndAlso .Buildings(0) IsNot Nothing Then
                    With .Buildings(0)
                        WebHelper_Personal.SetValueForCommercialDataPrefill(.SquareFeet, propResponse.returnedData.squareFeet.ToString, propResponse.noHit, wipeOutExisting, noHitQualification, existingOrderInfo.HasOrderedPrefill, existingOrderInfo.squareFeet.ToString, onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.PositiveIntegerType, useUpperCaseAndTrimWhenSetting:=False, okayToOverwrite:=isOkayToOverwriteFromProperty, neverSetItNotValid:=True)
                        WebHelper_Personal.SetValueForCommercialDataPrefill(.NumberOfStories, propResponse.returnedData.stories.ToString, propResponse.noHit, wipeOutExisting, noHitQualification, existingOrderInfo.HasOrderedPrefill, existingOrderInfo.stories.ToString, onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.PositiveIntegerType, useUpperCaseAndTrimWhenSetting:=False, okayToOverwrite:=isOkayToOverwriteFromProperty, neverSetItNotValid:=True)
                        WebHelper_Personal.SetValueForCommercialDataPrefill(.YearBuilt, propResponse.returnedData.yearBuilt.ToString, propResponse.noHit, wipeOutExisting, noHitQualification, existingOrderInfo.HasOrderedPrefill, existingOrderInfo.yearBuilt.ToString, onlyValidIfSpecifiedType:=QuickQuoteHelperClass.TypeToVerify.PositiveIntegerType, useUpperCaseAndTrimWhenSetting:=False, okayToOverwrite:=isOkayToOverwriteFromProperty, neverSetItNotValid:=True)
                    End With
                End If
            End With
            propResponse.returnedData.HasOrderedPrefill = True
            propResponse.returnedData.OrderInformation = PropertyPrefillAddressString(l.Address)
            SetCommercialDataPrefillModifiersOnLocation(l, propResponse.returnedData)
            setAnyModifiers = True
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_Preload(ByRef qqo As QuickQuoteObject, Optional ByVal locationNums As List(Of Integer) = Nothing, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef attemptedServiceCallType As CommercialDataPrefillServiceType = CommercialDataPrefillServiceType.None, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "")
        attemptedServiceCall = False
        attemptedServiceCallType = CommercialDataPrefillServiceType.None
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True Then

            Dim chc As New CommonHelperClass
            CallCommercialDataPrefill_Preload(qqo.Policyholder, qqo.Locations, policyId:=chc.IntegerForString(qqo.PolicyId), policyImageNum:=chc.IntegerForString(qqo.PolicyImageNum), locationNums:=locationNums, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_Preload(ByVal ph As QuickQuotePolicyholder, ByVal locs As List(Of QuickQuoteLocation), Optional ByVal policyId As Integer = 0, Optional ByVal policyImageNum As Integer = 0, Optional ByVal locationNums As List(Of Integer) = Nothing, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef attemptedServiceCallType As CommercialDataPrefillServiceType = CommercialDataPrefillServiceType.None, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "")
        attemptedServiceCall = False
        attemptedServiceCallType = CommercialDataPrefillServiceType.None
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        Dim okayToContinue As Boolean = False
        Dim okayForFirm As Boolean = PolicyHolderHasNameAndAddressData(ph, mustHaveAllRequiredFields:=True)
        Dim locNumsToUse As List(Of Integer) = Nothing
        Dim okayForProp As Boolean = HasLocationWithAddressData(locs, locNums:=locNumsToUse, mustHaveAllRequiredFields:=True) AndAlso (locationNums Is Nothing OrElse locationNums.Count = 0 OrElse HasLocationAtAnySpecifiedPosition(locs, locationNums, locationMustHaveAddressInfo:=True, locationAddressMustHaveAllRequiredFields:=True, matches:=locNumsToUse) = True)

        If okayForFirm = True AndAlso okayForProp = True Then
            okayToContinue = True
        ElseIf okayForFirm = True Then
            attemptedServiceCallType = CommercialDataPrefillServiceType.FirmographicsOnly
            CallCommercialDataPrefill_FirmographicsOnly_Preload(ph, policyId:=policyId, policyImageNum:=policyImageNum, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
            Exit Sub
        ElseIf okayForProp = True Then
            attemptedServiceCallType = CommercialDataPrefillServiceType.PropertyOnly
            CallCommercialDataPrefill_PropertyOnly_Preload(locs, ph:=ph, policyId:=policyId, policyImageNum:=policyImageNum, locationNums:=locationNums, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
            Exit Sub
        End If

        If okayToContinue = True Then
            attemptedServiceCall = True
            attemptedServiceCallType = CommercialDataPrefillServiceType.FirmographicsAndProperty

            Dim info As New CommercialDataPrefillAllPreloadResponseInfo
            Dim chc As New CommonHelperClass
            Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_APIKey")
            Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_BaseURL")
            Dim prefillRequest As New IFI.Integrations.Request.AllCommercialPrefill(baseUrl, apiKey)

            Try

                With prefillRequest
                    .PolicyId = policyId
                    .PolicyImageNum = policyImageNum

                    If LoadedCommercialDataPrefillFirmographicsRequestInfo(ph, info.firmographicsInfo) = True Then
                        With .FirmographicsData
                            .BusinessName = info.firmographicsInfo.businessName
                            .Address1 = info.firmographicsInfo.address1
                            .City = info.firmographicsInfo.city
                            .State = info.firmographicsInfo.state
                            .Zip = info.firmographicsInfo.zip
                            .OtherInfo = info.firmographicsInfo.otherInfo
                        End With
                    End If
                    With .PropertyData
                        If locs IsNot Nothing AndAlso locs.Count > 0 AndAlso locNumsToUse IsNot Nothing AndAlso locNumsToUse.Count > 0 Then
                            locNumsToUse.Sort()
                            Dim lNum As Integer = 0
                            For Each l As QuickQuoteLocation In locs
                                lNum += 1

                                Dim pi As CommercialDataPrefillPropertyRequestInfo = Nothing
                                If locNumsToUse.Contains(lNum) = True AndAlso LoadedCommercialDataPrefillPropertyRequestInfo(l, ph, pi) = True Then
                                    info.propertyInfos.Add(pi)
                                    .Add(New IFI.Integrations.Request.Objects.ComPropertyPrefill With {
                                        .FirstName = pi.firstName,
                                        .MiddleName = pi.middleName,
                                        .LastName = pi.lastName,
                                        .StreetNumber = pi.streetNumber,
                                        .StreetName = pi.streetName,
                                        .UnitNumber = pi.unitNumber,
                                        .City = pi.city,
                                        .State = pi.state,
                                        .Zip = pi.zip
                                    })
                                End If
                            Next
                        End If
                    End With
                End With

                Dim response = prefillRequest.PreLoadVendorData()
                If response IsNot Nothing Then

                    ProcessBaseCommercialDataPrefillApiPreloadResponseInfo(response, info)
                Else 'added so we don't think it was just a NoHit; this would happen if there's an issue w/ the SnapLogic endpoint that's being suppressed by the helper library
                    caughtUnhandledException = True
                    If prefillRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(prefillRequest.InternalExceptionMessages) Then
                        unhandledExceptionToString = "No Response from Integration Service" & "; " & prefillRequest.InternalExceptionMessages
                    Else
                        unhandledExceptionToString = "No Response from Integration Service"
                    End If
                End If
            Catch ex As Exception
                caughtUnhandledException = True
                If prefillRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(prefillRequest.InternalExceptionMessages) Then
                    unhandledExceptionToString = ex.ToString & "; " & prefillRequest.InternalExceptionMessages
                Else
                    unhandledExceptionToString = ex.ToString
                End If
            End Try

            info.attemptedServiceCall = attemptedServiceCall
            If caughtUnhandledException = True Then
                info.caughtException = caughtUnhandledException
                info.exceptionErrorMessage = unhandledExceptionToString
            End If

            If caughtUnhandledException = False AndAlso info.requestReceived = False Then
                If info.caughtException = True OrElse info.hasError = True Then
                    caughtUnhandledException = True
                    If info.caughtException = True Then
                        unhandledExceptionToString = info.exceptionErrorMessage
                    Else
                        unhandledExceptionToString = info.errorMessage
                    End If
                End If
            End If
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_FirmographicsOnly(ByRef qqo As QuickQuoteObject, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "", Optional ByRef setAnyMods As Boolean = False)
        attemptedServiceCall = False
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        setAnyMods = False
        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True AndAlso PolicyHolderHasNameAndAddressData(qqo.Policyholder, mustHaveAllRequiredFields:=True) = True Then
            attemptedServiceCall = True

            Dim info As New CommercialDataPrefillFirmographicsResponseInfo
            Dim chc As New CommonHelperClass
            Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_APIKey")
            Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_BaseURL")
            Dim prefillRequest As New IFI.Integrations.Request.LNComFirm(baseUrl, apiKey)
            Try
                With prefillRequest
                    .PolicyId = chc.IntegerForString(qqo.PolicyId)
                    .PolicyImageNum = chc.IntegerForString(qqo.PolicyImageNum)

                    If LoadedCommercialDataPrefillFirmographicsRequestInfo(qqo.Policyholder, info.requestInfo) = True Then
                        .BusinessName = info.requestInfo.businessName
                        .Address1 = info.requestInfo.address1
                        .City = info.requestInfo.city
                        .State = info.requestInfo.state
                        .Zip = info.requestInfo.zip
                        .OtherInfo = info.requestInfo.otherInfo
                    End If
                End With

                Dim response = prefillRequest.GetVendorData()
                If response IsNot Nothing Then
                    With response

                        ProcessBaseCommercialDataPrefillApiResponseInfo(Of IFI.Integrations.Response.LNComFirm)(response, info)

                        ProcessCommercialDataPrefillApiFirmographicsResponseData(.responseData, info)
                    End With
                Else 'added so we don't think it was just a NoHit; this would happen if there's an issue w/ the SnapLogic endpoint that's being suppressed by the helper library
                    caughtUnhandledException = True
                    If prefillRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(prefillRequest.InternalExceptionMessages) Then
                        unhandledExceptionToString = "No Response from Integration Service" & "; " & prefillRequest.InternalExceptionMessages
                    Else
                        unhandledExceptionToString = "No Response from Integration Service"
                    End If
                End If
            Catch ex As Exception
                caughtUnhandledException = True
                If prefillRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(prefillRequest.InternalExceptionMessages) Then
                    unhandledExceptionToString = ex.ToString & "; " & prefillRequest.InternalExceptionMessages
                Else
                    unhandledExceptionToString = ex.ToString
                End If
            End Try
            info.attemptedServiceCall = attemptedServiceCall
            If caughtUnhandledException = True Then
                info.caughtException = caughtUnhandledException
                info.exceptionErrorMessage = unhandledExceptionToString
            End If

            If caughtUnhandledException = False Then
                Dim qqHelper As New QuickQuoteHelperClass

                'Dim strJsonF As String = IntegrationApiHelper.JsonStringForObject(info)
                'Dim testFirmInfo As CommercialDataPrefillFirmographicsResponseInfo = Nothing
                'Try
                '    testFirmInfo = IntegrationApiHelper.ObjectForJsonString(strJsonF, GetType(CommercialDataPrefillFirmographicsResponseInfo))
                'Catch ex As Exception

                'End Try
                'If testFirmInfo IsNot Nothing Then

                'End If

                ProcessCommercialDataPrefillFirmographicsReturnData(qqo, info, setAnyMods, qqHelper:=qqHelper)
            End If
            If caughtUnhandledException = False AndAlso setAnyMods = False Then
                If info.caughtException = True OrElse info.hasError = True Then
                    caughtUnhandledException = True
                    If info.caughtException = True Then
                        unhandledExceptionToString = info.exceptionErrorMessage
                    Else
                        unhandledExceptionToString = info.errorReason
                    End If
                End If
            End If
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_FirmographicsOnly_Preload(ByRef qqo As QuickQuoteObject, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "")
        attemptedServiceCall = False
        caughtUnhandledException = False
        unhandledExceptionToString = ""

        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True Then
            Dim chc As New CommonHelperClass
            CallCommercialDataPrefill_FirmographicsOnly_Preload(qqo.Policyholder, policyId:=chc.IntegerForString(qqo.PolicyId), policyImageNum:=chc.IntegerForString(qqo.PolicyImageNum), attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_FirmographicsOnly_Preload(ByVal ph As QuickQuotePolicyholder, Optional ByVal policyId As Integer = 0, Optional ByVal policyImageNum As Integer = 0, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "")
        attemptedServiceCall = False
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        If PolicyHolderHasNameAndAddressData(ph, mustHaveAllRequiredFields:=True) = True Then
            attemptedServiceCall = True

            Dim chc As New CommonHelperClass
            Dim info As New CommercialDataPrefillFirmographicsPreloadResponseInfo
            Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_APIKey")
            Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_BaseURL")
            Dim prefillRequest As New IFI.Integrations.Request.LNComFirm(baseUrl, apiKey)
            Try
                With prefillRequest
                    .PolicyId = policyId
                    .PolicyImageNum = policyImageNum

                    If LoadedCommercialDataPrefillFirmographicsRequestInfo(ph, info.requestInfo) = True Then
                        .BusinessName = info.requestInfo.businessName
                        .Address1 = info.requestInfo.address1
                        .City = info.requestInfo.city
                        .State = info.requestInfo.state
                        .Zip = info.requestInfo.zip
                        .OtherInfo = info.requestInfo.otherInfo
                    End If
                End With
                Dim response = prefillRequest.PreLoadVendorData()
                If response IsNot Nothing Then

                    ProcessBaseCommercialDataPrefillApiPreloadResponseInfo(response, info)
                Else 'added so we don't think it was just a NoHit; this would happen if there's an issue w/ the SnapLogic endpoint that's being suppressed by the helper library
                    caughtUnhandledException = True
                    If prefillRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(prefillRequest.InternalExceptionMessages) Then
                        unhandledExceptionToString = "No Response from Integration Service" & "; " & prefillRequest.InternalExceptionMessages
                    Else
                        unhandledExceptionToString = "No Response from Integration Service"
                    End If
                End If
            Catch ex As Exception
                caughtUnhandledException = True
                If prefillRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(prefillRequest.InternalExceptionMessages) Then
                    unhandledExceptionToString = ex.ToString & "; " & prefillRequest.InternalExceptionMessages
                Else
                    unhandledExceptionToString = ex.ToString
                End If
            End Try

            info.attemptedServiceCall = attemptedServiceCall
            If caughtUnhandledException = True Then
                info.caughtException = caughtUnhandledException
                info.exceptionErrorMessage = unhandledExceptionToString
            End If

            If caughtUnhandledException = False AndAlso info.requestReceived = False Then
                If info.caughtException = True OrElse info.hasError = True Then
                    caughtUnhandledException = True
                    If info.caughtException = True Then
                        unhandledExceptionToString = info.exceptionErrorMessage
                    Else
                        unhandledExceptionToString = info.errorMessage
                    End If
                End If
            End If
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_PropertyOnly(ByRef qqo As QuickQuoteObject, Optional ByVal locationNums As List(Of Integer) = Nothing, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "", Optional ByRef setAnyMods As Boolean = False)
        attemptedServiceCall = False
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        setAnyMods = False
        Dim locNumsToUse As List(Of Integer) = Nothing
        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True AndAlso qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 AndAlso HasLocationWithAddressData(qqo.Locations, locNums:=locNumsToUse, mustHaveAllRequiredFields:=True) = True AndAlso (locationNums Is Nothing OrElse locationNums.Count = 0 OrElse HasLocationAtAnySpecifiedPosition(qqo.Locations, locationNums, locationMustHaveAddressInfo:=True, locationAddressMustHaveAllRequiredFields:=True, matches:=locNumsToUse) = True) AndAlso locNumsToUse IsNot Nothing AndAlso locNumsToUse.Count > 0 Then
            attemptedServiceCall = True
            locNumsToUse.Sort()

            Dim info As New CommercialDataPrefillPropertiesResponseInfo
            Dim chc As New CommonHelperClass
            Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_APIKey")
            Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_BaseURL")
            Dim prefillRequest As New IFI.Integrations.Request.ComPropertyPrefill(baseUrl, apiKey)
            Try
                With prefillRequest
                    .PolicyId = chc.IntegerForString(qqo.PolicyId)
                    .PolicyImageNum = chc.IntegerForString(qqo.PolicyImageNum)
                    With .PropertyData
                        Dim lNum As Integer = 0
                        For Each l As QuickQuoteLocation In qqo.Locations
                            lNum += 1

                            Dim ri As CommercialDataPrefillPropertyRequestInfo = Nothing
                            If locNumsToUse.Contains(lNum) = True AndAlso LoadedCommercialDataPrefillPropertyRequestInfo(l, qqo.Policyholder, ri) = True Then
                                Dim pi As New CommercialDataPrefillPropertyResponseInfo
                                pi.requestInfo = ri
                                info.propertyInfos.Add(pi)
                                .Add(New IFI.Integrations.Request.Objects.ComPropertyPrefill With {
                                    .FirstName = pi.requestInfo.firstName,
                                    .MiddleName = pi.requestInfo.middleName,
                                    .LastName = pi.requestInfo.lastName,
                                    .StreetNumber = pi.requestInfo.streetNumber,
                                    .StreetName = pi.requestInfo.streetName,
                                    .UnitNumber = pi.requestInfo.unitNumber,
                                    .City = pi.requestInfo.city,
                                    .State = pi.requestInfo.state,
                                    .Zip = pi.requestInfo.zip
                                })
                            End If
                        Next
                    End With
                End With

                Dim response = prefillRequest.GetVendorData()
                If response IsNot Nothing Then
                    With response

                        ProcessBaseCommercialDataPrefillApiResponseInfo(Of List(Of IFI.Integrations.Response.Common.ServiceResult(Of IFI.Integrations.Response.ComPropertyPrefill)))(response, info)
                        If .responseData IsNot Nothing Then
                            info.hasResponseData = True
                            If .responseData.Count > 0 Then
                                Dim pCount As Integer = 0
                                For Each p As IFI.Integrations.Response.Common.ServiceResult(Of IFI.Integrations.Response.ComPropertyPrefill) In .responseData
                                    pCount += 1
                                    Dim propInfo As CommercialDataPrefillPropertyResponseInfo = Nothing
                                    If info.propertyInfos IsNot Nothing AndAlso info.propertyInfos.Count >= pCount Then
                                        propInfo = info.propertyInfos(pCount - 1)
                                        If propInfo Is Nothing Then
                                            propInfo = New CommercialDataPrefillPropertyResponseInfo
                                        End If
                                    Else
                                        propInfo = New CommercialDataPrefillPropertyResponseInfo
                                        info.propertyInfos.Add(propInfo)
                                    End If
                                    If p IsNot Nothing Then
                                        ProcessBaseCommercialDataPrefillApiResponseInfo(Of IFI.Integrations.Response.ComPropertyPrefill)(p, propInfo)
                                        With p

                                            ProcessCommercialDataPrefillApiPropertyResponseData(.responseData, propInfo)
                                        End With
                                    End If
                                Next
                            End If
                        End If
                    End With
                Else 'added so we don't think it was just a NoHit; this would happen if there's an issue w/ the SnapLogic endpoint that's being suppressed by the helper library
                    caughtUnhandledException = True
                    If prefillRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(prefillRequest.InternalExceptionMessages) Then
                        unhandledExceptionToString = "No Response from Integration Service" & "; " & prefillRequest.InternalExceptionMessages
                    Else
                        unhandledExceptionToString = "No Response from Integration Service"
                    End If

                End If
            Catch ex As Exception
                caughtUnhandledException = True
                If prefillRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(prefillRequest.InternalExceptionMessages) Then
                    unhandledExceptionToString = ex.ToString & "; " & prefillRequest.InternalExceptionMessages
                Else
                    unhandledExceptionToString = ex.ToString
                End If
            End Try

            info.attemptedServiceCall = attemptedServiceCall
            If caughtUnhandledException = True Then
                info.caughtException = caughtUnhandledException
                info.exceptionErrorMessage = unhandledExceptionToString
            End If

            If info.propertyInfos IsNot Nothing AndAlso info.propertyInfos.Count > 0 Then
                For Each pi As CommercialDataPrefillPropertyResponseInfo In info.propertyInfos
                    If pi IsNot Nothing AndAlso pi.returnedAnyData = True Then
                        info.returnedAnyData = True
                        Exit For
                    End If
                Next
            End If

            If caughtUnhandledException = False Then
                Dim qqHelper As New QuickQuoteHelperClass

                If info.propertyInfos IsNot Nothing AndAlso info.propertyInfos.Count = locNumsToUse.Count Then
                    Dim isOkayToOverwriteFromProperty As Boolean = WebHelper_Personal.CommercialDataPrefill_OkayToOverwriteExistingPropertyDataFromFirmographics()
                    Dim locCounter As Integer = 0
                    For Each lNum As Integer In locNumsToUse
                        locCounter += 1
                        Dim l As QuickQuoteLocation = qqo.Locations(lNum - 1)
                        If l IsNot Nothing Then
                            Dim propInfo As CommercialDataPrefillPropertyResponseInfo = info.propertyInfos(locCounter - 1)
                            'If propInfo IsNot Nothing Then
                            '    'Dim strJsonP As String = IntegrationApiHelper.JsonStringForObject(propInfo)
                            '    'Dim testPropInfo As CommercialDataPrefillPropertyResponseInfo = Nothing
                            '    'Try
                            '    '    testPropInfo = IntegrationApiHelper.ObjectForJsonString(strJsonP, GetType(CommercialDataPrefillPropertyResponseInfo))
                            '    'Catch ex As Exception

                            '    'End Try
                            '    'If testPropInfo IsNot Nothing Then

                            '    'End If
                            'End If
                            ProcessCommercialDataPrefillPropertyReturnData(l, propInfo, setAnyMods, isOkayToOverwriteFromProperty:=isOkayToOverwriteFromProperty, qqHelper:=qqHelper)
                        End If
                    Next
                End If
            End If
            If caughtUnhandledException = False AndAlso setAnyMods = False Then
                If info.propertyInfos IsNot Nothing AndAlso info.propertyInfos.Count > 0 Then
                    For Each pi As CommercialDataPrefillPropertyResponseInfo In info.propertyInfos
                        If pi IsNot Nothing AndAlso (pi.caughtException = True OrElse pi.hasError = True) Then
                            caughtUnhandledException = True
                            If pi.caughtException = True Then
                                unhandledExceptionToString = pi.exceptionErrorMessage
                            Else
                                unhandledExceptionToString = pi.errorReason
                            End If
                            Exit For
                        End If
                    Next
                End If
            End If
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_PropertyOnly_Preload(ByRef qqo As QuickQuoteObject, Optional ByVal locationNums As List(Of Integer) = Nothing, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "")
        attemptedServiceCall = False
        caughtUnhandledException = False
        unhandledExceptionToString = ""

        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True Then
            Dim chc As New CommonHelperClass
            CallCommercialDataPrefill_PropertyOnly_Preload(qqo.Locations, ph:=qqo.Policyholder, policyId:=chc.IntegerForString(qqo.PolicyId), policyImageNum:=chc.IntegerForString(qqo.PolicyImageNum), locationNums:=locationNums, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_PropertyOnly_Preload(ByVal locs As List(Of QuickQuoteLocation), Optional ByVal ph As QuickQuotePolicyholder = Nothing, Optional ByVal policyId As Integer = 0, Optional ByVal policyImageNum As Integer = 0, Optional ByVal locationNums As List(Of Integer) = Nothing, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "")
        attemptedServiceCall = False
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        Dim locNumsToUse As List(Of Integer) = Nothing
        If locs IsNot Nothing AndAlso locs.Count > 0 AndAlso HasLocationWithAddressData(locs, locNums:=locNumsToUse, mustHaveAllRequiredFields:=True) = True AndAlso (locationNums Is Nothing OrElse locationNums.Count = 0 OrElse HasLocationAtAnySpecifiedPosition(locs, locationNums, locationMustHaveAddressInfo:=True, locationAddressMustHaveAllRequiredFields:=True, matches:=locNumsToUse) = True) AndAlso locNumsToUse IsNot Nothing AndAlso locNumsToUse.Count > 0 Then
            attemptedServiceCall = True

            Dim info As New CommercialDataPrefillPropertiesPreloadResponseInfo

            Try
                Dim chc As New CommonHelperClass
                Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_APIKey")
                Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_HOM_PreLoadIntegrationCall_BaseURL")
                Dim prefillRequest As New IFI.Integrations.Request.ComPropertyPrefill(baseUrl, apiKey)
                With prefillRequest
                    .PolicyId = policyId
                    .PolicyImageNum = policyImageNum
                    With .PropertyData
                        Dim lNum As Integer = 0
                        For Each l As QuickQuoteLocation In locs
                            lNum += 1

                            Dim pi As CommercialDataPrefillPropertyRequestInfo = Nothing
                            If locNumsToUse.Contains(lNum) = True AndAlso LoadedCommercialDataPrefillPropertyRequestInfo(l, ph, pi) = True Then
                                info.propertyInfos.Add(pi)
                                .Add(New IFI.Integrations.Request.Objects.ComPropertyPrefill With {
                                    .FirstName = pi.firstName,
                                    .MiddleName = pi.middleName,
                                    .LastName = pi.lastName,
                                    .StreetNumber = pi.streetNumber,
                                    .StreetName = pi.streetName,
                                    .UnitNumber = pi.unitNumber,
                                    .City = pi.city,
                                    .State = pi.state,
                                    .Zip = pi.zip
                                })
                            End If
                        Next
                    End With
                End With
                Dim response = prefillRequest.PreLoadVendorData()
                If response IsNot Nothing Then

                    ProcessBaseCommercialDataPrefillApiPreloadResponseInfo(response, info)
                Else 'added so we don't think it was just a NoHit; this would happen if there's an issue w/ the SnapLogic endpoint that's being suppressed by the helper library
                    caughtUnhandledException = True
                    If prefillRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(prefillRequest.InternalExceptionMessages) Then
                        unhandledExceptionToString = prefillRequest.InternalExceptionMessages
                    Else
                        unhandledExceptionToString = "No Response from Integration Service"
                    End If
                End If
            Catch ex As Exception
                caughtUnhandledException = True
                unhandledExceptionToString = ex.ToString
            End Try

            info.attemptedServiceCall = attemptedServiceCall
            If caughtUnhandledException = True Then
                info.caughtException = caughtUnhandledException
                info.exceptionErrorMessage = unhandledExceptionToString
            End If

            If caughtUnhandledException = False AndAlso info.requestReceived = False Then
                If info.caughtException = True OrElse info.hasError = True Then
                    caughtUnhandledException = True
                    If info.caughtException = True Then
                        unhandledExceptionToString = info.exceptionErrorMessage
                    Else
                        unhandledExceptionToString = info.errorMessage
                    End If
                End If
            End If
        End If
    End Sub
    Public Shared Function HasLocationAtAnySpecifiedPosition(ByVal locs As List(Of QuickQuoteLocation), ByVal locNums As List(Of Integer), Optional ByVal locationMustHaveAddressInfo As Boolean = False, Optional ByVal locationAddressMustHaveAllRequiredFields As Boolean = False, Optional ByRef matches As List(Of Integer) = Nothing) As Boolean
        Dim hasIt As Boolean = False
        matches = Nothing

        matches = MatchingLocationPositions(locs, locNums, locationMustHaveAddressInfo:=locationMustHaveAddressInfo, locationAddressMustHaveAllRequiredFields:=locationAddressMustHaveAllRequiredFields)
        If matches IsNot Nothing AndAlso matches.Count > 0 Then
            hasIt = True
        End If

        Return hasIt
    End Function
    Public Shared Function MatchingLocationPositions(ByVal locs As List(Of QuickQuoteLocation), ByVal locNums As List(Of Integer), Optional ByVal locationMustHaveAddressInfo As Boolean = False, Optional ByVal locationAddressMustHaveAllRequiredFields As Boolean = False) As List(Of Integer)
        Dim matches As List(Of Integer) = Nothing

        If locs IsNot Nothing AndAlso locNums IsNot Nothing AndAlso locs.Count > 0 AndAlso locNums.Count > 0 Then
            For Each n As Integer In locNums
                If n > 0 AndAlso locs.Count >= n Then
                    Dim isOkay As Boolean = True
                    If locationMustHaveAddressInfo = True Then
                        isOkay = False
                        Dim l As QuickQuoteLocation = locs(n - 1)
                        If LocationHasAddressData(l, mustHaveAllRequiredFields:=locationAddressMustHaveAllRequiredFields) = True Then
                            isOkay = True
                        End If
                    End If
                    If isOkay = True Then
                        QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(n, matches)
                    End If
                End If
            Next
        End If

        Return matches
    End Function
    Public Shared Function LocationPositionsWithAddressData(ByVal locs As List(Of QuickQuoteLocation), Optional ByVal mustHaveAllRequiredFields As Boolean = False) As List(Of Integer)
        Dim locNums As List(Of Integer) = Nothing

        If locs IsNot Nothing AndAlso locs.Count > 0 Then
            Dim locNum As Integer = 0
            For Each l As QuickQuoteLocation In locs
                locNum += 1
                If LocationHasAddressData(l, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
                    QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(locNum, locNums)
                End If
            Next
        End If

        Return locNums
    End Function
    Public Shared Function HasLocationWithAddressData(ByVal locs As List(Of QuickQuoteLocation), Optional ByRef locNums As List(Of Integer) = Nothing, Optional ByVal mustHaveAllRequiredFields As Boolean = False) As Boolean
        Dim hasIt As Boolean = False
        locNums = Nothing

        locNums = LocationPositionsWithAddressData(locs, mustHaveAllRequiredFields:=mustHaveAllRequiredFields)
        If locNums IsNot Nothing AndAlso locNums.Count > 0 Then
            hasIt = True
        End If

        Return hasIt
    End Function
    Public Shared Function LocationHasAddressData(ByVal loc As QuickQuoteLocation, Optional ByVal mustHaveAllRequiredFields As Boolean = False) As Boolean
        Dim hasIt As Boolean = False

        If loc IsNot Nothing AndAlso AddressHasData(loc.Address, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
            hasIt = True
        End If

        Return hasIt
    End Function
    Public Shared Function AddressHasData(ByVal a As QuickQuoteAddress, Optional ByVal mustHaveAllRequiredFields As Boolean = False) As Boolean
        Dim hasIt As Boolean = False

        If mustHaveAllRequiredFields = True Then
            hasIt = AddressHasValidData(a)
        Else
            If a IsNot Nothing AndAlso a.HasData = True Then
                hasIt = True
            End If
        End If

        Return hasIt
    End Function
    Public Shared Function AddressHasValidData(ByVal a As QuickQuoteAddress) As Boolean
        Dim hasIt As Boolean = False

        If AddressHasData(a, mustHaveAllRequiredFields:=False) = True Then
            If ((String.IsNullOrWhiteSpace(a.HouseNum) = False AndAlso String.IsNullOrWhiteSpace(a.StreetName) = False) OrElse String.IsNullOrWhiteSpace(a.POBox) = False) Then
                'has street address or poBox; now check city/state/zip
                Dim qqhelper As New QuickQuoteHelperClass
                If String.IsNullOrWhiteSpace(a.City) = False AndAlso qqhelper.IsPositiveIntegerString(a.StateId) = True AndAlso String.IsNullOrWhiteSpace(a.Zip) = False Then
                    hasIt = True
                End If
            End If
        End If

        Return hasIt
    End Function
    Public Shared Function NameHasData(ByVal n As QuickQuoteName, Optional ByVal mustHaveAllRequiredFields As Boolean = False) As Boolean
        Dim hasIt As Boolean = False

        If mustHaveAllRequiredFields = True Then
            hasIt = NameHasValidData(n)
        Else
            If n IsNot Nothing AndAlso n.HasData = True Then
                hasIt = True
            End If
        End If

        Return hasIt
    End Function
    Public Shared Function NameHasValidData(ByVal n As QuickQuoteName) As Boolean
        Dim hasIt As Boolean = False

        If NameHasData(n, mustHaveAllRequiredFields:=False) = True Then
            If ((String.IsNullOrWhiteSpace(n.FirstName) = False AndAlso String.IsNullOrWhiteSpace(n.LastName) = False) OrElse String.IsNullOrWhiteSpace(n.CommercialName1) = False) Then
                hasIt = True
            End If
        End If

        Return hasIt
    End Function
    Public Shared Function PolicyHolderHasNameAndAddressData(ByVal ph As QuickQuotePolicyholder, Optional ByVal mustHaveAllRequiredFields As Boolean = False) As Boolean
        Dim hasIt As Boolean = False

        If ph IsNot Nothing AndAlso NameHasData(ph.Name, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True AndAlso AddressHasData(ph.Address, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
            hasIt = True
        End If

        Return hasIt
    End Function
    'Public Sub SetPolicyModifier_HasOrderedPrefill(ByRef qqo As QuickQuoteObject, ByVal hasOrdered As Boolean, ByVal modDescription As String)
    '    'note: just using top-level qqo instead of putting it on subQuotes/stateQuotes
    '    If qqo IsNot Nothing Then
    '        SetModifier_HasOrderedPrefill(qqo.Modifiers, hasOrdered, modDescription, isLocation:=False)
    '    End If
    'End Sub
    Public Sub SetCommercialDataPrefillModifiersOnPolicy(ByRef qqo As QuickQuoteObject, ByVal returnedData As CommercialDataPrefillFirmographicsInfo)
        If qqo IsNot Nothing AndAlso returnedData IsNot Nothing Then
            SetCommercialDataPrefillModifier(qqo.Modifiers, CommercialDataPrefillModifierType.HasOrderedPrefill, checkBox:=returnedData.HasOrderedPrefill, modDescription:=returnedData.OrderInformation, isLocation:=False)
            SetCommercialDataPrefillModifier(qqo.Modifiers, CommercialDataPrefillModifierType.FirmBusinessName, modDescription:=returnedData.businessName, isLocation:=False)
            SetCommercialDataPrefillModifier(qqo.Modifiers, CommercialDataPrefillModifierType.FirmDBA, modDescription:=returnedData.dba, isLocation:=False)
            SetCommercialDataPrefillModifier(qqo.Modifiers, CommercialDataPrefillModifierType.FirmFEIN, modDescription:=returnedData.fein, isLocation:=False)
            SetCommercialDataPrefillModifier(qqo.Modifiers, CommercialDataPrefillModifierType.FirmLegalEntity, modDescription:=returnedData.legalEntity, isLocation:=False)
            SetCommercialDataPrefillModifier(qqo.Modifiers, CommercialDataPrefillModifierType.FirmNAICS, modDescription:=returnedData.naics, isLocation:=False)
            'SetCommercialDataPrefillModifier(qqo.Modifiers, CommercialDataPrefillModifierType.FirmOtherEntity, modDescription:=returnedData.otherEntity, isLocation:=False)
            SetCommercialDataPrefillModifier(qqo.Modifiers, CommercialDataPrefillModifierType.FirmOtherEntity, modDescription:=returnedData.otherLegalEntity, isLocation:=False)
            SetCommercialDataPrefillModifier(qqo.Modifiers, CommercialDataPrefillModifierType.FirmPhone, modDescription:=returnedData.phone, isLocation:=False)
            SetCommercialDataPrefillModifier(qqo.Modifiers, CommercialDataPrefillModifierType.FirmURL, modDescription:=returnedData.url, isLocation:=False)
            SetCommercialDataPrefillModifier(qqo.Modifiers, CommercialDataPrefillModifierType.FirmYearStarted, modDescription:=returnedData.yearStarted, isLocation:=False)
        End If
    End Sub
    'Public Sub SetLocationModifier_HasOrderedPrefill(ByRef l As QuickQuoteLocation, ByVal hasOrdered As Boolean, ByVal modDescription As String)
    '    If l IsNot Nothing Then
    '        SetModifier_HasOrderedPrefill(l.Modifiers, hasOrdered, modDescription, isLocation:=True)
    '    End If
    'End Sub
    Public Sub SetCommercialDataPrefillModifiersOnLocation(ByRef l As QuickQuoteLocation, ByVal returnedData As CommercialDataPrefillPropertyInfo)
        If l IsNot Nothing AndAlso returnedData IsNot Nothing Then
            SetCommercialDataPrefillModifier(l.Modifiers, CommercialDataPrefillModifierType.HasOrderedPrefill, checkBox:=returnedData.HasOrderedPrefill, modDescription:=returnedData.OrderInformation, isLocation:=True)
            SetCommercialDataPrefillModifier(l.Modifiers, CommercialDataPrefillModifierType.PropHasFireplaces, checkBox:=returnedData.hasFireplaces, isLocation:=True)
            SetCommercialDataPrefillModifier(l.Modifiers, CommercialDataPrefillModifierType.PropMunicipalityOrTownship, modDescription:=returnedData.municipalityOrTownship, isLocation:=True)
            SetCommercialDataPrefillModifier(l.Modifiers, CommercialDataPrefillModifierType.PropPools, modDescription:=returnedData.pools.ToString, isLocation:=True)
            SetCommercialDataPrefillModifier(l.Modifiers, CommercialDataPrefillModifierType.PropSquareFeet, modDescription:=returnedData.squareFeet.ToString, isLocation:=True)
            SetCommercialDataPrefillModifier(l.Modifiers, CommercialDataPrefillModifierType.PropStories, modDescription:=returnedData.stories.ToString, isLocation:=True)
            SetCommercialDataPrefillModifier(l.Modifiers, CommercialDataPrefillModifierType.PropYearBuilt, modDescription:=returnedData.yearBuilt.ToString, isLocation:=True)
        End If
    End Sub
    'Public Sub SetModifier_HasOrderedPrefill(ByRef qqMods As List(Of QuickQuoteModifier), ByVal hasOrdered As Boolean, ByVal modDescription As String, ByVal isLocation As Boolean)
    '    'Dim m As QuickQuoteModifier = GetHasOrderedPrefillModifier(qqMods, addIfNeeded:=True)
    '    'If m IsNot Nothing Then
    '    '    With m
    '    '        .CheckboxSelected = hasOrdered
    '    '        .ModifierOptionDescription = modDescription
    '    '        If isLocation = True Then
    '    '            .ModifierLevelId = "8" 'Location
    '    '        Else
    '    '            .ModifierLevelId = "1" 'Policy
    '    '        End If
    '    '    End With
    '    'End If
    '    SetCommercialDataPrefillModifier(qqMods, CommercialDataPrefillModifierType.HasOrderedPrefill, checkBox:=hasOrdered, modDescription:=modDescription, isLocation:=isLocation)
    'End Sub
    Public Sub SetCommercialDataPrefillModifier(ByRef qqMods As List(Of QuickQuoteModifier), ByVal modType As CommercialDataPrefillModifierType, Optional ByVal checkBox As Boolean = False, Optional ByVal modDescription As String = "", Optional ByVal isLocation As Boolean = False)
        Dim m As QuickQuoteModifier = GetCommercialDataPrefillModifier(qqMods, modType, addIfNeeded:=True)
        If m IsNot Nothing Then
            With m
                .CheckboxSelected = checkBox
                .ModifierOptionDescription = modDescription
                If isLocation = True Then
                    .ModifierLevelId = "8" 'Location
                Else
                    .ModifierLevelId = "1" 'Policy
                End If
            End With
        End If
    End Sub
    Public Function HasPolicyModifier_HasOrderedPrefill(ByVal qqo As QuickQuoteObject, ByRef hasOrdered As Boolean, ByRef modDescription As String, Optional ByVal populateAllFirmographicsInfoIfOrdered As Boolean = False, Optional ByRef firmographicsInfo As CommercialDataPrefillFirmographicsInfo = Nothing, Optional ByVal resetFirmographicsInfo As Boolean = True) As Boolean
        Dim hasIt As Boolean = False
        hasOrdered = False
        modDescription = ""
        If resetFirmographicsInfo = True Then
            firmographicsInfo = Nothing
        End If

        'note: just using top-level qqo instead of putting it on subQuotes/stateQuotes
        If qqo IsNot Nothing Then
            hasIt = HasModifier_HasOrderedPrefil(qqo.Modifiers, hasOrdered, modDescription)
            If hasOrdered = True AndAlso populateAllFirmographicsInfoIfOrdered = True Then
                firmographicsInfo = New CommercialDataPrefillFirmographicsInfo
                With firmographicsInfo
                    .HasOrderedPrefill = True
                    .OrderInformation = modDescription
                    .businessName = CommercialDataPrefillModifierOptionDescriptionValue(qqo.Modifiers, CommercialDataPrefillModifierType.FirmBusinessName)
                    .dba = CommercialDataPrefillModifierOptionDescriptionValue(qqo.Modifiers, CommercialDataPrefillModifierType.FirmDBA)
                    .fein = CommercialDataPrefillModifierOptionDescriptionValue(qqo.Modifiers, CommercialDataPrefillModifierType.FirmFEIN)
                    .legalEntity = CommercialDataPrefillModifierOptionDescriptionValue(qqo.Modifiers, CommercialDataPrefillModifierType.FirmLegalEntity)
                    .naics = CommercialDataPrefillModifierOptionDescriptionValue(qqo.Modifiers, CommercialDataPrefillModifierType.FirmNAICS)
                    '.otherEntity = CommercialDataPrefillModifierOptionDescriptionValue(qqo.Modifiers, CommercialDataPrefillModifierType.FirmOtherEntity)
                    .otherLegalEntity = CommercialDataPrefillModifierOptionDescriptionValue(qqo.Modifiers, CommercialDataPrefillModifierType.FirmOtherEntity)
                    .phone = CommercialDataPrefillModifierOptionDescriptionValue(qqo.Modifiers, CommercialDataPrefillModifierType.FirmPhone)
                    .url = CommercialDataPrefillModifierOptionDescriptionValue(qqo.Modifiers, CommercialDataPrefillModifierType.FirmURL)
                    .yearStarted = CommercialDataPrefillModifierOptionDescriptionValue(qqo.Modifiers, CommercialDataPrefillModifierType.FirmYearStarted)
                End With
            End If
        End If

        Return hasIt
    End Function
    Public Function HasLocationModifier_HasOrderedPrefill(ByVal l As QuickQuoteLocation, ByRef hasOrdered As Boolean, ByRef modDescription As String, Optional ByVal populateAllPropertyInfoIfOrdered As Boolean = False, Optional ByRef propertyInfo As CommercialDataPrefillPropertyInfo = Nothing, Optional ByVal resetPropertyInfo As Boolean = True) As Boolean
        Dim hasIt As Boolean = False
        hasOrdered = False
        modDescription = ""
        If resetPropertyInfo = True Then
            propertyInfo = Nothing
        End If

        If l IsNot Nothing Then
            hasIt = HasModifier_HasOrderedPrefil(l.Modifiers, hasOrdered, modDescription)
            If hasOrdered = True AndAlso populateAllPropertyInfoIfOrdered = True Then
                propertyInfo = New CommercialDataPrefillPropertyInfo
                With propertyInfo
                    .HasOrderedPrefill = True
                    .OrderInformation = modDescription
                    .hasFireplaces = CommercialDataPrefillModifierCheckboxSelectedValue(l.Modifiers, CommercialDataPrefillModifierType.PropHasFireplaces)
                    .municipalityOrTownship = CommercialDataPrefillModifierOptionDescriptionValue(l.Modifiers, CommercialDataPrefillModifierType.PropMunicipalityOrTownship)
                    Dim qqHelper As New QuickQuoteHelperClass
                    .pools = qqHelper.IntegerForString(CommercialDataPrefillModifierOptionDescriptionValue(l.Modifiers, CommercialDataPrefillModifierType.PropPools))
                    .squareFeet = qqHelper.IntegerForString(CommercialDataPrefillModifierOptionDescriptionValue(l.Modifiers, CommercialDataPrefillModifierType.PropSquareFeet))
                    .stories = qqHelper.IntegerForString(CommercialDataPrefillModifierOptionDescriptionValue(l.Modifiers, CommercialDataPrefillModifierType.PropStories))
                    .yearBuilt = qqHelper.IntegerForString(CommercialDataPrefillModifierOptionDescriptionValue(l.Modifiers, CommercialDataPrefillModifierType.PropYearBuilt))
                End With
            End If
        End If

        Return hasIt
    End Function
    Public Function CommercialDataPrefillModifierCheckboxSelectedValue(ByVal qqMods As List(Of QuickQuoteModifier), ByVal modType As CommercialDataPrefillModifierType) As Boolean
        Dim val As Boolean = False

        Dim m As QuickQuoteModifier = GetCommercialDataPrefillModifier(qqMods, modType)
        If m IsNot Nothing Then
            val = m.CheckboxSelected
        End If

        Return val
    End Function
    Public Function CommercialDataPrefillModifierOptionDescriptionValue(ByVal qqMods As List(Of QuickQuoteModifier), ByVal modType As CommercialDataPrefillModifierType) As String
        Dim val As String = ""

        Dim m As QuickQuoteModifier = GetCommercialDataPrefillModifier(qqMods, modType)
        If m IsNot Nothing Then
            val = m.ModifierOptionDescription
        End If

        Return val
    End Function
    Public Function HasModifier_HasOrderedPrefil(ByVal qqMods As List(Of QuickQuoteModifier), ByRef hasOrdered As Boolean, ByRef modDescription As String) As Boolean
        Dim hasIt As Boolean = False
        hasOrdered = False
        modDescription = ""

        'Dim m As QuickQuoteModifier = GetHasOrderedPrefillModifier(qqMods)
        Dim m As QuickQuoteModifier = GetCommercialDataPrefillModifier(qqMods, CommercialDataPrefillModifierType.HasOrderedPrefill)
        If m IsNot Nothing Then
            hasIt = True
            hasOrdered = m.CheckboxSelected
            modDescription = m.ModifierOptionDescription
        End If

        Return hasIt
    End Function
    Public Function HasPolicyCommercialDataPrefillOrder(ByVal qqo As QuickQuoteObject, Optional ByRef orderInformation As String = "", Optional ByVal populateAllFirmographicsInfoIfOrdered As Boolean = False, Optional ByRef firmographicsInfo As CommercialDataPrefillFirmographicsInfo = Nothing, Optional ByVal resetFirmographicsInfo As Boolean = True) As Boolean
        Dim hasIt As Boolean = False
        Dim hasOrdered As Boolean = False
        'Dim modDescription As String = ""
        orderInformation = ""

        If HasPolicyModifier_HasOrderedPrefill(qqo, hasOrdered, orderInformation, populateAllFirmographicsInfoIfOrdered:=populateAllFirmographicsInfoIfOrdered, firmographicsInfo:=firmographicsInfo, resetFirmographicsInfo:=resetFirmographicsInfo) = True AndAlso hasOrdered = True Then
            hasIt = True
        End If

        Return hasIt
    End Function
    Public Function PolicyCommercialDataPrefillOrderIsMissingOrNeedsReorder(ByVal qqo As QuickQuoteObject, Optional ByVal mustHaveAllRequiredFields As Boolean = True) As Boolean
        Dim needsIt As Boolean = False

        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True Then
            Dim orderInformation As String = ""
            If HasPolicyCommercialDataPrefillOrder(qqo, orderInformation:=orderInformation) = True Then
                If StringIsDifferentThanFirmographicsPrefillNameAddressString(orderInformation, qqo.Policyholder, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
                    needsIt = True
                End If
            Else
                If mustHaveAllRequiredFields = False OrElse PolicyHolderHasNameAndAddressData(qqo.Policyholder, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
                    needsIt = True
                End If
            End If
        End If

        Return needsIt
    End Function
    Public Function HasLocationCommercialDataPrefillOrder(ByVal l As QuickQuoteLocation, Optional ByRef orderInformation As String = "", Optional ByVal populateAllPropertyInfoIfOrdered As Boolean = False, Optional ByRef propertyInfo As CommercialDataPrefillPropertyInfo = Nothing, Optional ByVal resetPropertyInfo As Boolean = True) As Boolean
        Dim hasIt As Boolean = False
        Dim hasOrdered As Boolean = False
        'Dim modDescription As String = ""
        orderInformation = ""

        If HasLocationModifier_HasOrderedPrefill(l, hasOrdered, orderInformation, populateAllPropertyInfoIfOrdered:=populateAllPropertyInfoIfOrdered, propertyInfo:=propertyInfo, resetPropertyInfo:=resetPropertyInfo) = True AndAlso hasOrdered = True Then
            hasIt = True
        End If

        Return hasIt
    End Function
    Public Function LocationCommercialDataPrefillOrderIsMissingOrNeedsReorder(ByVal l As QuickQuoteLocation, Optional ByVal validateQuote As Boolean = True, Optional ByVal qqo As QuickQuoteObject = Nothing, Optional ByVal mustHaveAllRequiredFields As Boolean = True) As Boolean
        Dim needsIt As Boolean = False

        If l IsNot Nothing Then
            Dim okayToContinue As Boolean = False
            If validateQuote = True Then
                If qqo Is Nothing Then
                    qqo = l.GetTopLevelParentQuoteObject()
                End If
                If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True Then
                    okayToContinue = True
                End If
            Else
                okayToContinue = True
            End If
            If okayToContinue = True Then
                Dim orderInformation As String = ""
                If HasLocationCommercialDataPrefillOrder(l, orderInformation:=orderInformation) = True Then
                    If StringIsDifferentThanPropertyPrefillAddressString(orderInformation, l.Address, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
                        needsIt = True
                    End If
                Else
                    If mustHaveAllRequiredFields = False OrElse AddressHasData(l.Address, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
                        needsIt = True
                    End If
                End If
            End If
        End If

        Return needsIt
    End Function
    Public Function LocationNumbersNeedingCommercialDataPrefillOrderOrReorder(ByVal qqo As QuickQuoteObject, Optional ByVal mustHaveAllRequiredFields As Boolean = True) As List(Of Integer)
        Dim locNums As List(Of Integer) = Nothing

        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True Then
            If qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 Then
                Dim locCount As Integer = 0
                Dim qqHelper As New QuickQuoteHelperClass
                For Each l As QuickQuoteLocation In qqo.Locations
                    locCount += 1
                    If l IsNot Nothing AndAlso (qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote OrElse (qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso qqHelper.IsQuickQuoteLocationNewToImage(l, qqo) = True)) Then
                        If LocationCommercialDataPrefillOrderIsMissingOrNeedsReorder(l, validateQuote:=False, qqo:=qqo, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
                            QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(locCount, locNums)
                        End If
                    End If
                Next
            End If
        End If

        Return locNums
    End Function
    Public Sub CallCommercialDataPrefill_FirmographicsOnly_IfNeeded(ByRef qqo As QuickQuoteObject, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "", Optional ByRef setAnyMods As Boolean = False)
        attemptedServiceCall = False
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        setAnyMods = False
        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True AndAlso qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
            If PolicyCommercialDataPrefillOrderIsMissingOrNeedsReorder(qqo, mustHaveAllRequiredFields:=True) = True Then
                CallCommercialDataPrefill_FirmographicsOnly(qqo, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, setAnyMods:=setAnyMods)
            End If
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_PropertyOnly_IfNeeded(ByRef qqo As QuickQuoteObject, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "", Optional ByRef locNumsAttempted As List(Of Integer) = Nothing, Optional ByRef setAnyMods As Boolean = False)
        attemptedServiceCall = False
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        locNumsAttempted = Nothing
        setAnyMods = False
        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True Then
            locNumsAttempted = LocationNumbersNeedingCommercialDataPrefillOrderOrReorder(qqo, mustHaveAllRequiredFields:=True)
            If locNumsAttempted IsNot Nothing AndAlso locNumsAttempted.Count > 0 Then
                CallCommercialDataPrefill_PropertyOnly(qqo, locationNums:=locNumsAttempted, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, setAnyMods:=setAnyMods)
            End If
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_IfNeeded(ByRef qqo As QuickQuoteObject, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef attemptedServiceCallType As CommercialDataPrefillServiceType = CommercialDataPrefillServiceType.None, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "", Optional ByRef locNumsAttempted As List(Of Integer) = Nothing, Optional ByRef setAnyMods As Boolean = False)
        attemptedServiceCall = False
        attemptedServiceCallType = CommercialDataPrefillServiceType.None
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        locNumsAttempted = Nothing
        setAnyMods = False
        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True Then
            Dim needsFirm As Boolean = False
            Dim needsProp As Boolean = False
            If qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote AndAlso PolicyCommercialDataPrefillOrderIsMissingOrNeedsReorder(qqo, mustHaveAllRequiredFields:=True) = True Then
                needsFirm = True
            End If
            locNumsAttempted = LocationNumbersNeedingCommercialDataPrefillOrderOrReorder(qqo, mustHaveAllRequiredFields:=True)
            If locNumsAttempted IsNot Nothing AndAlso locNumsAttempted.Count > 0 Then
                needsProp = True
            End If
            If needsFirm = True OrElse needsProp = True Then
                If needsProp = False Then
                    'Firmograhpics only
                    attemptedServiceCallType = CommercialDataPrefillServiceType.FirmographicsOnly
                    CallCommercialDataPrefill_FirmographicsOnly(qqo, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, setAnyMods:=setAnyMods)
                ElseIf needsFirm = False Then
                    'Property only
                    attemptedServiceCallType = CommercialDataPrefillServiceType.PropertyOnly
                    CallCommercialDataPrefill_PropertyOnly(qqo, locationNums:=locNumsAttempted, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, setAnyMods:=setAnyMods)
                Else
                    'both Firmographics and Property
                    CallCommercialDataPrefill(qqo, locationNums:=locNumsAttempted, attemptedServiceCall:=attemptedServiceCall, attemptedServiceCallType:=attemptedServiceCallType, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString, setAnyMods:=setAnyMods)
                End If
            End If
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_FirmographicsOnly_Preload_IfNeeded(ByRef qqo As QuickQuoteObject, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "")
        attemptedServiceCall = False
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True AndAlso qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote Then
            If PolicyCommercialDataPrefillOrderIsMissingOrNeedsReorder(qqo, mustHaveAllRequiredFields:=True) = True Then
                CallCommercialDataPrefill_FirmographicsOnly_Preload(qqo, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
            End If
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_PropertyOnly_Preload_IfNeeded(ByRef qqo As QuickQuoteObject, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "", Optional ByRef locNumsAttempted As List(Of Integer) = Nothing)
        attemptedServiceCall = False
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        locNumsAttempted = Nothing
        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True Then
            locNumsAttempted = LocationNumbersNeedingCommercialDataPrefillOrderOrReorder(qqo, mustHaveAllRequiredFields:=True)
            If locNumsAttempted IsNot Nothing AndAlso locNumsAttempted.Count > 0 Then
                CallCommercialDataPrefill_PropertyOnly_Preload(qqo, locationNums:=locNumsAttempted, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
            End If
        End If
    End Sub
    Public Sub CallCommercialDataPrefill_Preload_IfNeeded(ByRef qqo As QuickQuoteObject, Optional ByRef attemptedServiceCall As Boolean = False, Optional ByRef attemptedServiceCallType As CommercialDataPrefillServiceType = CommercialDataPrefillServiceType.None, Optional ByRef caughtUnhandledException As Boolean = False, Optional ByRef unhandledExceptionToString As String = "", Optional ByRef locNumsAttempted As List(Of Integer) = Nothing)
        attemptedServiceCall = False
        attemptedServiceCallType = CommercialDataPrefillServiceType.None
        caughtUnhandledException = False
        unhandledExceptionToString = ""
        locNumsAttempted = Nothing
        If qqo IsNot Nothing AndAlso Helpers.WebHelper_Personal.IsCommercialDataPrefillAvailableForQuote(qqo) = True Then
            Dim needsFirm As Boolean = False
            Dim needsProp As Boolean = False
            If qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote AndAlso PolicyCommercialDataPrefillOrderIsMissingOrNeedsReorder(qqo, mustHaveAllRequiredFields:=True) = True Then
                needsFirm = True
            End If
            locNumsAttempted = LocationNumbersNeedingCommercialDataPrefillOrderOrReorder(qqo, mustHaveAllRequiredFields:=True)
            If locNumsAttempted IsNot Nothing AndAlso locNumsAttempted.Count > 0 Then
                needsProp = True
            End If
            If needsFirm = True OrElse needsProp = True Then
                If needsProp = False Then
                    'Firmograhpics only
                    attemptedServiceCallType = CommercialDataPrefillServiceType.FirmographicsOnly
                    CallCommercialDataPrefill_FirmographicsOnly_Preload(qqo, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
                ElseIf needsFirm = False Then
                    'Property only
                    attemptedServiceCallType = CommercialDataPrefillServiceType.PropertyOnly
                    CallCommercialDataPrefill_PropertyOnly_Preload(qqo, locationNums:=locNumsAttempted, attemptedServiceCall:=attemptedServiceCall, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
                Else
                    'both Firmographics and Property
                    CallCommercialDataPrefill_Preload(qqo, locationNums:=locNumsAttempted, attemptedServiceCall:=attemptedServiceCall, attemptedServiceCallType:=attemptedServiceCallType, caughtUnhandledException:=caughtUnhandledException, unhandledExceptionToString:=unhandledExceptionToString)
                End If
            End If
        End If
    End Sub
    'Public Function IsQuickQuoteLocationNewToImage(ByVal l As QuickQuoteLocation, ByVal qqo As QuickQuoteObject) As Boolean
    '    Dim isIt As Boolean = False

    '    'If l IsNot Nothing Then
    '    '    'AddedDate
    '    '    'LastModifiedDate
    '    '    'PCAdded_Date
    '    '    'AddedImageNum
    '    '    'EffectiveDate
    '    'End If
    '    Dim qqHelper As New QuickQuoteHelperClass
    '    isIt = qqHelper.IsQuickQuoteLocationNewToImage(l, qqo)

    '    Return isIt
    'End Function
    Public Function HasAnyCommercialDataPrefillOrders(ByVal qqo As QuickQuoteObject) As Boolean
        Dim hasIt As Boolean = False

        If qqo IsNot Nothing Then
            hasIt = HasPolicyCommercialDataPrefillOrder(qqo)
            If hasIt = False AndAlso qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 Then
                For Each l As QuickQuoteLocation In qqo.Locations
                    hasIt = HasLocationCommercialDataPrefillOrder(l)
                    If hasIt = True Then
                        Exit For
                    End If
                Next
            End If
        End If

        Return hasIt
    End Function
    'Public Function GetHasOrderedPrefillModifier(ByRef qqMods As List(Of QuickQuoteModifier), Optional ByVal addIfNeeded As Boolean = False, Optional ByRef itemAlreadyExistedOrAddedNew As QuickQuoteHelperClass.AlreadyExistedOrAddedNew = QuickQuoteHelperClass.AlreadyExistedOrAddedNew.None) As QuickQuoteModifier
    '    'Dim m As QuickQuoteModifier = Nothing
    '    'itemAlreadyExistedOrAddedNew = QuickQuoteHelperClass.AlreadyExistedOrAddedNew.None

    '    'Dim hasOrderedPrefillModTypeId As Integer = 139
    '    'Dim qqHelper As New QuickQuoteHelperClass
    '    'If addIfNeeded = True Then
    '    '    m = qqHelper.QuickQuoteModifierForTypeIds_AddIfNeeded(qqMods, hasOrderedPrefillModTypeId, hasOrderedPrefillModTypeId, checkListForExistingModifierFirst:=True, itemAlreadyExistedOrAddedNew:=itemAlreadyExistedOrAddedNew)
    '    'Else
    '    '    m = qqHelper.QuickQuoteModifierForTypeIds(qqMods, hasOrderedPrefillModTypeId, hasOrderedPrefillModTypeId)
    '    '    If m IsNot Nothing Then
    '    '        itemAlreadyExistedOrAddedNew = QuickQuoteHelperClass.AlreadyExistedOrAddedNew.AlreadyExisted
    '    '    End If
    '    'End If

    '    'Return m
    '    Return GetCommercialDataPrefillModifier(qqMods, CommercialDataPrefillModifierType.HasOrderedPrefill, addIfNeeded:=addIfNeeded, itemAlreadyExistedOrAddedNew:=itemAlreadyExistedOrAddedNew)
    'End Function
    Public Enum CommercialDataPrefillModifierType
        HasOrderedPrefill = 139
        FirmBusinessName = 143
        FirmDBA = 144
        FirmFEIN = 145
        FirmLegalEntity = 146
        FirmNAICS = 147
        FirmOtherEntity = 148
        FirmPhone = 149
        FirmURL = 150
        FirmYearStarted = 151
        PropHasFireplaces = 152
        PropMunicipalityOrTownship = 153
        PropPools = 154
        PropSquareFeet = 155
        PropStories = 156
        PropYearBuilt = 157
    End Enum
    Public Function GetCommercialDataPrefillModifier(ByRef qqMods As List(Of QuickQuoteModifier), ByVal modType As CommercialDataPrefillModifierType, Optional ByVal addIfNeeded As Boolean = False, Optional ByRef itemAlreadyExistedOrAddedNew As QuickQuoteHelperClass.AlreadyExistedOrAddedNew = QuickQuoteHelperClass.AlreadyExistedOrAddedNew.None) As QuickQuoteModifier
        Dim m As QuickQuoteModifier = Nothing
        itemAlreadyExistedOrAddedNew = QuickQuoteHelperClass.AlreadyExistedOrAddedNew.None

        If System.Enum.IsDefined(GetType(CommercialDataPrefillModifierType), modType) = True Then
            Dim modTypeId As Integer = CInt(modType)
            Dim qqHelper As New QuickQuoteHelperClass
            If addIfNeeded = True Then
                m = qqHelper.QuickQuoteModifierForTypeIds_AddIfNeeded(qqMods, modTypeId, modTypeId, checkListForExistingModifierFirst:=True, itemAlreadyExistedOrAddedNew:=itemAlreadyExistedOrAddedNew)
            Else
                m = qqHelper.QuickQuoteModifierForTypeIds(qqMods, modTypeId, modTypeId)
                If m IsNot Nothing Then
                    itemAlreadyExistedOrAddedNew = QuickQuoteHelperClass.AlreadyExistedOrAddedNew.AlreadyExisted
                End If
            End If
        End If

        Return m
    End Function
    Public Shared Function FirmographicsPrefillNameAddressString(ByVal ph As QuickQuotePolicyholder, Optional ByVal mustHaveAllRequiredFields As Boolean = True) As String
        Dim str As String = ""

        If PolicyHolderHasNameAndAddressData(ph, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
            Dim strNm As String = FirmographicsPrefillNameString(ph.Name, mustHaveAllRequiredFields:=False)
            Dim strAdd As String = FirmographicsPrefillAddressString(ph.Address, mustHaveAllRequiredFields:=False)
            Dim qqHelper As New QuickQuoteHelperClass
            str = qqHelper.appendText(strNm, strAdd, splitter:=vbCrLf)
        End If

        Return str
    End Function
    Public Shared Function StringIsDifferentThanFirmographicsPrefillNameAddressString(ByVal str As String, ByVal ph As QuickQuotePolicyholder, Optional ByVal mustHaveAllRequiredFields As Boolean = True) As Boolean
        Dim isDiff As Boolean = False

        If String.IsNullOrWhiteSpace(str) = False Then
            Dim strPh As String = FirmographicsPrefillNameAddressString(ph, mustHaveAllRequiredFields:=mustHaveAllRequiredFields)
            If String.IsNullOrWhiteSpace(strPh) = False Then
                If UCase(RemoveLineFeedsAndCarriageReturnsFromString(str)) <> UCase(RemoveLineFeedsAndCarriageReturnsFromString(strPh)) Then
                    isDiff = True
                End If
            End If
        End If

        Return isDiff
    End Function
    Public Shared Function RemoveLineFeedsAndCarriageReturnsFromString(ByVal str As String) As String
        Dim newStr As String = str

        If String.IsNullOrWhiteSpace(newStr) = False Then
            newStr = Replace(newStr, vbCrLf, "")
            newStr = Replace(newStr, vbCr, "")
            newStr = Replace(newStr, vbLf, "")
        End If

        Return newStr
    End Function
    Public Shared Function FirmographicsPrefillNameString(ByVal nm As QuickQuoteName, Optional ByVal mustHaveAllRequiredFields As Boolean = True) As String
        Dim str As String = ""

        str = BusinessNameForCommercialDataPrefill(nm, mustHaveAllRequiredFields:=mustHaveAllRequiredFields)

        Return str
    End Function
    Public Shared Function StringIsDifferentThanFirmographicsPrefillNameString(ByVal str As String, ByVal nm As QuickQuoteName, Optional ByVal mustHaveAllRequiredFields As Boolean = True) As Boolean
        Dim isDiff As Boolean = False

        If String.IsNullOrWhiteSpace(str) = False Then
            Dim strNm As String = FirmographicsPrefillNameString(nm, mustHaveAllRequiredFields:=mustHaveAllRequiredFields)
            If String.IsNullOrWhiteSpace(strNm) = False Then
                If UCase(RemoveLineFeedsAndCarriageReturnsFromString(str)) <> UCase(RemoveLineFeedsAndCarriageReturnsFromString(strNm)) Then
                    isDiff = True
                End If
            End If
        End If

        Return isDiff
    End Function
    Public Shared Function FirmographicsPrefillAddressString(ByVal add As QuickQuoteAddress, Optional ByVal mustHaveAllRequiredFields As Boolean = True) As String
        Dim str As String = ""

        If AddressHasData(add, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
            Dim qqHelper As New QuickQuoteHelperClass
            With add
                str = StreetAddressForCommercialDataPrefill(add, mustHaveAllRequiredFields:=False)
                Dim cityStateZip As String = CityStateZipForCommercialDataPrefill(add, mustHaveAllRequiredFields:=False)
                str = qqHelper.appendText(str, cityStateZip, splitter:=vbCrLf)
                'use Other?
            End With
        End If

        Return str
    End Function
    Public Shared Function StringIsDifferentThanFirmographicsPrefillAddressString(ByVal str As String, ByVal add As QuickQuoteAddress, Optional ByVal mustHaveAllRequiredFields As Boolean = True) As Boolean
        Dim isDiff As Boolean = False

        If String.IsNullOrWhiteSpace(str) = False Then
            Dim strAdd As String = FirmographicsPrefillAddressString(add, mustHaveAllRequiredFields:=mustHaveAllRequiredFields)
            If String.IsNullOrWhiteSpace(strAdd) = False Then
                If UCase(RemoveLineFeedsAndCarriageReturnsFromString(str)) <> UCase(RemoveLineFeedsAndCarriageReturnsFromString(strAdd)) Then
                    isDiff = True
                End If
            End If
        End If

        Return isDiff
    End Function
    Public Shared Function PropertyPrefillAddressString(ByVal add As QuickQuoteAddress, Optional ByVal mustHaveAllRequiredFields As Boolean = True) As String
        Dim str As String = ""

        If AddressHasData(add, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
            Dim qqHelper As New QuickQuoteHelperClass
            With add
                str = StreetAddressForCommercialDataPrefill(add, mustHaveAllRequiredFields:=False)
                Dim cityStateZip As String = CityStateZipForCommercialDataPrefill(add, mustHaveAllRequiredFields:=False)
                str = qqHelper.appendText(str, cityStateZip, splitter:=vbCrLf)
            End With
        End If

        Return str
    End Function
    Public Shared Function StringIsDifferentThanPropertyPrefillAddressString(ByVal str As String, ByVal add As QuickQuoteAddress, Optional ByVal mustHaveAllRequiredFields As Boolean = True) As Boolean
        Dim isDiff As Boolean = False

        If String.IsNullOrWhiteSpace(str) = False Then
            Dim strAdd As String = PropertyPrefillAddressString(add, mustHaveAllRequiredFields:=mustHaveAllRequiredFields)
            If String.IsNullOrWhiteSpace(strAdd) = False Then
                If UCase(RemoveLineFeedsAndCarriageReturnsFromString(str)) <> UCase(RemoveLineFeedsAndCarriageReturnsFromString(strAdd)) Then
                    isDiff = True
                End If
            End If
        End If

        Return isDiff
    End Function
    Public Shared Sub SetPersonalNameFieldsForCommercialDataPrefill(ByVal nm As QuickQuoteName, ByRef first As String, ByRef middle As String, ByRef last As String, Optional ByVal mustHaveAllRequiredFields As Boolean = True)
        first = ""
        middle = ""
        last = ""

        If NameHasData(nm, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
            If String.IsNullOrWhiteSpace(nm.CommercialName1) = False Then
                last = nm.CommercialName1
            Else
                first = nm.FirstName
                middle = nm.MiddleName
                last = nm.LastName
            End If
        End If
    End Sub
    Public Shared Function BusinessNameForCommercialDataPrefill(ByVal nm As QuickQuoteName, Optional ByVal mustHaveAllRequiredFields As Boolean = True) As String
        Dim busName As String = ""

        If NameHasData(nm, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
            If String.IsNullOrWhiteSpace(nm.CommercialName1) = False Then
                busName = nm.CommercialName1
            Else
                Dim qqHelper As New QuickQuoteHelperClass
                busName = qqHelper.appendText(nm.FirstName, nm.LastName, splitter:=" ")
            End If
        End If

        Return busName
    End Function
    Public Shared Function StreetAddressForCommercialDataPrefill(ByVal add As QuickQuoteAddress, Optional ByVal mustHaveAllRequiredFields As Boolean = False) As String
        Dim stAdd As String = ""

        If AddressHasData(add, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
            If String.IsNullOrWhiteSpace(add.HouseNum) = False OrElse String.IsNullOrWhiteSpace(add.StreetName) = False Then
                Dim qqHelper As New QuickQuoteHelperClass
                stAdd = qqHelper.appendText(add.HouseNum, add.StreetName, splitter:=" ")
                stAdd = qqHelper.appendText(stAdd, add.ApartmentNumber, splitter:=" ")
            ElseIf String.IsNullOrWhiteSpace(add.POBox) = False Then
                stAdd = "PO Box " & add.POBox
            End If
        End If

        Return stAdd
    End Function
    Public Shared Function CityStateZipForCommercialDataPrefill(ByVal add As QuickQuoteAddress, Optional ByVal mustHaveAllRequiredFields As Boolean = False) As String
        Dim stCSZ As String = ""

        If AddressHasData(add, mustHaveAllRequiredFields:=mustHaveAllRequiredFields) = True Then
            Dim qqHelper As New QuickQuoteHelperClass
            stCSZ = qqHelper.appendText(add.City, add.State, splitter:=", ")
            Dim zip5 As String = add.Zip
            If String.IsNullOrWhiteSpace(zip5) = False AndAlso Len(zip5) > 5 Then
                zip5 = Left(zip5, 5)
            End If
            stCSZ = qqHelper.appendText(stCSZ, zip5, splitter:=" ")
        End If

        Return stCSZ
    End Function
    Public Shared Function ErrorMessageLooksLikeCommercialDataPrefillNoHit(ByVal errMsg As String) As Boolean
        Dim couldBe As Boolean = False

        If String.IsNullOrWhiteSpace(errMsg) = False Then
            Dim noHitList As New List(Of String)
            noHitList.Add("Not processed, insufficient search data")
            noHitList.Add("Could not locate business in LexisNexis database")
            'couldBe = QuickQuoteHelperClass.hasTextMatchInList(errMsg, noHitList, matchType:=QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing_MatchAnywhere)
            'note: not using QuickQuoteHelperClass.hasTextMatchInList because it may not work like we expect when using the MatchStart/MatchEnd/MatchAnywhere types... because it isn't always looking for param 1 to be in param 2... instead, it may look to see if the smaller word is inside the longer one
            For Each str As String In noHitList
                If String.IsNullOrWhiteSpace(str) = False AndAlso Len(errMsg) >= Len(str) AndAlso UCase(errMsg).Contains(UCase(str)) = True Then
                    couldBe = True
                    Exit For
                End If
            Next
        End If

        Return couldBe
    End Function

    'created for Matt
    Public Shared Function DiamondStreetAddressString(ByVal diaAdd As Diamond.Common.Objects.Address) As String
        Dim str As String = ""
        If diaAdd IsNot Nothing Then
            If String.IsNullOrWhiteSpace(diaAdd.HouseNumber) = False OrElse String.IsNullOrWhiteSpace(diaAdd.StreetName) = False Then
                str = AppendText(diaAdd.HouseNumber, diaAdd.StreetName, delimiter:=" ")
                str = AppendText(str, diaAdd.ApartmentNumber, delimiter:=" ")
            ElseIf String.IsNullOrWhiteSpace(diaAdd.POBox) = False Then
                str = "PO Box " & diaAdd.POBox
            End If
        End If
        Return str
    End Function
    Public Shared Function DiamondCityStateZipString(ByVal diaAdd As Diamond.Common.Objects.Address) As String
        Dim str As String = ""
        If diaAdd IsNot Nothing Then
            str = AppendText(diaAdd.City, diaAdd.StateAbbreviation, delimiter:=", ")
            Dim zip5 As String = diaAdd.Zip
            If String.IsNullOrWhiteSpace(zip5) = False AndAlso Len(zip5) > 5 Then
                zip5 = Left(zip5, 5)
            End If
            str = AppendText(str, zip5, delimiter:=" ")
        End If
        Return str
    End Function
    Public Shared Function DiamondAddressString(ByVal diaAdd As Diamond.Common.Objects.Address) As String
        Dim str As String = ""
        If diaAdd IsNot Nothing Then
            str = DiamondStreetAddressString(diaAdd)
            Dim cityStateZip As String = DiamondCityStateZipString(diaAdd)
            str = AppendText(str, cityStateZip, delimiter:=vbCrLf)
        End If
        Return str
    End Function
    Public Shared Function DiamondBusinessNameString(ByVal diaNm As Diamond.Common.Objects.Name) As String
        Dim str As String = ""
        If diaNm IsNot Nothing Then
            If String.IsNullOrWhiteSpace(diaNm.CommercialName1) = False Then
                str = diaNm.CommercialName1
            Else
                str = AppendText(diaNm.FirstName, diaNm.LastName, delimiter:=" ")
            End If
        End If
        Return str
    End Function
    Public Shared Function DiamondCommDataPrefillFirmographicsString(ByVal diaPh As Diamond.Common.Objects.Policy.PolicyHolder) As String
        Dim str As String = ""
        If diaPh IsNot Nothing Then
            str = AppendText(DiamondBusinessNameString(diaPh.Name), DiamondAddressString(diaPh.Address), delimiter:=vbCrLf)
        End If
        Return str
    End Function
    Public Shared Function DiamondCommDataPrefillPropertyString(ByVal diaLoc As Diamond.Common.Objects.Policy.Location) As String
        Dim str As String = ""
        If diaLoc IsNot Nothing Then
            str = DiamondAddressString(diaLoc.Address)
        End If
        Return str
    End Function
    Public Shared Function AppendText(ByVal strExisting As String, ByVal strNew As String, Optional ByVal delimiter As String = " ") As String
        Dim str As String = ""

        If String.IsNullOrWhiteSpace(strExisting) = False AndAlso String.IsNullOrWhiteSpace(strNew) = False Then
            str = strExisting & delimiter & strNew
        ElseIf String.IsNullOrWhiteSpace(strExisting) = False Then
            str = strExisting
        ElseIf String.IsNullOrWhiteSpace(strNew) = False Then
            str = strNew
        End If

        Return str
    End Function
End Class
Public Class CommercialDataPrefillResponseInfo
    Inherits CommercialDataPrefillBaseResponseInfo
    'Public Property attemptedServiceCall As Boolean = False
    Public Property noHit As Boolean = False
    'Public Property caughtException As Boolean = False
    'Public Property exceptionErrorMessage As String = ""
    'Public Property hasResponse As Boolean = False
    Public Property hasResponseData As Boolean = False
    'Public Property hasError As Boolean = False
    Public Property returnedAnyData As Boolean = False
    'Public Property errorReason As String = ""
End Class
Public Class CommercialDataPrefillFirmographicsResponseInfo
    Inherits CommercialDataPrefillResponseInfo

    'Public Property businessName As String = ""
    'Public Property dba As String = ""
    'Public Property fein As String = ""
    'Public Property legalEntity As String = ""
    'Public Property naics As String = ""
    'Public Property otherEntity As String = ""
    'Public Property phone As String = ""
    'Public Property url As String = ""
    'Public Property yearStarted As String = ""
    Public Property returnedData As New CommercialDataPrefillFirmographicsInfo

    Public Property requestInfo As New CommercialDataPrefillFirmographicsRequestInfo
End Class
Public Class CommercialDataPrefillInfo
    Public Property HasOrderedPrefill As Boolean = False
    Public Property OrderInformation As String = ""
End Class
Public Class CommercialDataPrefillFirmographicsInfo
    Inherits CommercialDataPrefillInfo

    Public Property businessName As String = ""
    Public Property dba As String = ""
    Public Property fein As String = ""
    Public Property legalEntity As String = ""
    Public Property naics As String = ""
    'Public Property otherEntity As String = ""
    Public Property otherLegalEntity As String = ""
    Public Property phone As String = ""
    Public Property url As String = ""
    Public Property yearStarted As String = ""
End Class
Public Class CommercialDataPrefillPropertyResponseInfo
    Inherits CommercialDataPrefillResponseInfo

    'Public Property hasFireplaces As Boolean = False
    'Public Property municipalityOrTownship As String = ""
    'Public Property pools As Integer = 0
    'Public Property squareFeet As Integer = 0
    'Public Property stories As Integer = 0
    'Public Property yearBuilt As Integer = 0
    Public Property returnedData As New CommercialDataPrefillPropertyInfo

    Public Property requestInfo As New CommercialDataPrefillPropertyRequestInfo
End Class
Public Class CommercialDataPrefillPropertyInfo
    Inherits CommercialDataPrefillInfo

    Public Property hasFireplaces As Boolean = False
    Public Property municipalityOrTownship As String = ""
    Public Property pools As Integer = 0
    Public Property squareFeet As Integer = 0
    Public Property stories As Integer = 0
    Public Property yearBuilt As Integer = 0
End Class
Public Class CommercialDataPrefillPropertiesResponseInfo
    Inherits CommercialDataPrefillResponseInfo

    Public Property propertyInfos As New List(Of CommercialDataPrefillPropertyResponseInfo)
End Class
Public Class CommercialDataPrefillAllResponseInfo
    Inherits CommercialDataPrefillResponseInfo

    Public Property firmographicsInfo As New CommercialDataPrefillFirmographicsResponseInfo
    Public Property propertyInfos As New List(Of CommercialDataPrefillPropertyResponseInfo)
End Class
Public Class CommercialDataPrefillFirmographicsRequestInfo
    Public Property address1 As String = ""
    Public Property businessName As String = ""
    Public Property city As String = ""
    Public Property otherInfo As String = ""
    Public Property state As String = ""
    Public Property zip As String = ""
End Class
Public Class CommercialDataPrefillPropertyRequestInfo
    Public Property city As String = ""
    Public Property firstName As String = ""
    Public Property lastName As String = ""
    Public Property middleName As String = ""
    Public Property state As String = ""
    Public Property streetName As String = ""
    Public Property streetNumber As String = ""
    Public Property unitNumber As String = ""
    Public Property zip As String = ""
End Class
Public Class CommercialDataPrefillPreloadResponseInfo
    Inherits CommercialDataPrefillBaseResponseInfo
    'Public Property attemptedServiceCall As Boolean = False
    'Public Property caughtException As Boolean = False
    'Public Property exceptionErrorMessage As String = ""
    'Public Property hasResponse As Boolean = False
    Public Property errorMessage As String = ""
    'Public Property hasError As Boolean = False
    Public Property requestReceived As Boolean = False
End Class
Public Class CommercialDataPrefillBaseResponseInfo
    Public Property attemptedServiceCall As Boolean = False
    Public Property caughtException As Boolean = False
    Public Property exceptionErrorMessage As String = ""
    Public Property hasResponse As Boolean = False
    Public Property hasError As Boolean = False
    Public Property errorReason As String = ""
End Class
Public Class CommercialDataPrefillFirmographicsPreloadResponseInfo
    Inherits CommercialDataPrefillPreloadResponseInfo

    Public Property requestInfo As New CommercialDataPrefillFirmographicsRequestInfo
End Class
Public Class CommercialDataPrefillPropertyPreloadResponseInfo
    Inherits CommercialDataPrefillPreloadResponseInfo

    Public Property requestInfo As New CommercialDataPrefillPropertyRequestInfo
End Class
Public Class CommercialDataPrefillPropertiesPreloadResponseInfo
    Inherits CommercialDataPrefillPreloadResponseInfo

    Public Property propertyInfos As New List(Of CommercialDataPrefillPropertyRequestInfo)
End Class
Public Class CommercialDataPrefillAllPreloadResponseInfo
    Inherits CommercialDataPrefillPreloadResponseInfo

    Public Property firmographicsInfo As New CommercialDataPrefillFirmographicsRequestInfo
    Public Property propertyInfos As New List(Of CommercialDataPrefillPropertyRequestInfo)
End Class
Public Class IntegrationApiHelper
    Public Shared Function JObjectForString(ByVal str As String) As Newtonsoft.Json.Linq.JObject
        Dim jObj As Newtonsoft.Json.Linq.JObject = Nothing

        If StringLooksLikeJson(str) = True Then
            jObj = Newtonsoft.Json.JsonConvert.DeserializeObject(str) 'will catch unhandled exception if not valid json
        End If

        Return jObj
    End Function
    Public Shared Function SimpleJObjectForString(ByVal str As String) As Newtonsoft.Json.Linq.JObject
        Dim jObj As Newtonsoft.Json.Linq.JObject = Nothing

        If StringLooksLikeJson(str) = True Then
            jObj = Newtonsoft.Json.Linq.JObject.Parse(str) 'will catch unhandled exception if not valid json
        End If

        Return jObj
    End Function
    Public Shared Function SimpleJObjectForStringAndChildHierarchy(ByVal str As String, ByVal children As List(Of String)) As Newtonsoft.Json.Linq.JObject
        Dim jObj As Newtonsoft.Json.Linq.JObject = Nothing

        If StringLooksLikeJson(str) = True Then
            jObj = Newtonsoft.Json.Linq.JObject.Parse(str) 'will catch unhandled exception if not valid json
            If jObj IsNot Nothing Then
                If children IsNot Nothing AndAlso children.Count > 0 Then
                    Dim stillLooking As Boolean = True
                    Dim lastChildFoundAtPosition As Integer = 0
                    Do While stillLooking = True 'note: will the way this is currently written, we don't really need an outer DoWhile loop... inner For loop would suffice since we won't go backwards in the child list after finding one at lower level
                        If children.Count > lastChildFoundAtPosition Then
                            Dim childPosition As Integer = 0
                            For Each child As String In children
                                childPosition += 1
                                If childPosition > lastChildFoundAtPosition Then
                                    If String.IsNullOrWhiteSpace(child) = False AndAlso jObj.GetValue(child) IsNot Nothing Then
                                        jObj = jObj.GetValue(child)
                                        lastChildFoundAtPosition = childPosition
                                        If childPosition >= children.Count Then
                                            stillLooking = False
                                            Exit For
                                        End If
                                    End If
                                End If
                            Next
                            If stillLooking = False Then
                                Exit Do
                            ElseIf childPosition >= children.Count Then
                                stillLooking = False
                                Exit Do
                            End If
                        Else
                            stillLooking = False
                            Exit Do
                        End If
                    Loop

                End If
            End If
        End If

        Return jObj
    End Function
    Public Shared Function ObjectForJsonString(Of T)(ByVal str As String) As T
        Dim obj As T = Nothing

        If StringLooksLikeJson(str) = True Then
            'obj = Newtonsoft.Json.JsonConvert.DeserializeObject(str, TypeOf T)
            obj = Newtonsoft.Json.JsonConvert.DeserializeObject(Of T)(str)
        End If

        Return obj
    End Function
    Public Shared Function ObjectForJsonString(ByVal str As String, ByVal t As Type) As Object
        'Dim obj As Object = Activator.CreateInstance(t)
        Dim obj As Object = Nothing

        If StringLooksLikeJson(str) = True Then
            obj = Newtonsoft.Json.JsonConvert.DeserializeObject(str, t)
        End If

        Return obj
    End Function
    Public Shared Function StringLooksLikeJson(ByVal str As String) As Boolean
        Dim looksLikeJson As Boolean = False

        If String.IsNullOrWhiteSpace(str) = False AndAlso ((str.StartsWith("{") = True AndAlso str.EndsWith("}") = True) OrElse (str.StartsWith("[") = True AndAlso str.EndsWith("]") = True)) Then
            looksLikeJson = True
        End If

        Return looksLikeJson
    End Function
    Public Shared Function JsonKeyValueForObject(ByVal jObj As Newtonsoft.Json.Linq.JObject, ByVal keyName As String) As String
        Dim val As String = ""

        If jObj IsNot Nothing AndAlso String.IsNullOrWhiteSpace(keyName) = False AndAlso jObj(keyName) IsNot Nothing Then
            val = jObj(keyName).ToString
        End If

        Return val
    End Function
    Public Shared Function JsonKeyValueForStringAndChildHierarchy(ByVal strJson As String, ByVal children As List(Of String), ByVal keyName As String) As String
        Dim val As String = ""

        If String.IsNullOrWhiteSpace(strJson) = False AndAlso String.IsNullOrWhiteSpace(keyName) = False Then
            val = JsonKeyValueForObject(SimpleJObjectForStringAndChildHierarchy(strJson, children), keyName)
        End If

        Return val
    End Function
    Public Shared Function JsonKeyValueForString(ByVal strJson As String, ByVal keyName As String) As String
        Dim val As String = ""

        If String.IsNullOrWhiteSpace(strJson) = False AndAlso String.IsNullOrWhiteSpace(keyName) = False Then
            val = JsonKeyValueForObject(SimpleJObjectForString(strJson), keyName)
        End If

        Return val
    End Function
    'Public Shared Function JsonStringForObject(Of T)(ByVal obj As T) As String
    Public Shared Function JsonStringForObject(ByVal obj As Object) As String
        Dim str As String = ""

        If obj IsNot Nothing Then
            str = Newtonsoft.Json.JsonConvert.SerializeObject(obj)
        End If

        Return str
    End Function

End Class
