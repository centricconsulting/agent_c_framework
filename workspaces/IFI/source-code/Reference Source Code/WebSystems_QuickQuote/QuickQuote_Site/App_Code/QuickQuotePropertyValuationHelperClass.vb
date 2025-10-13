Imports Microsoft.VisualBasic
Imports QuickQuote.CommonObjects
Imports helper = QuickQuote.CommonMethods.QuickQuoteHelperClass 'added 8/7/2014
Imports System.Data.SqlClient 'added 8/7/2014
Imports System.Xml 'added 8/13/2014
Imports System.Collections.Specialized
Imports System.Configuration
Imports Microsoft.IdentityModel
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Namespace QuickQuote.CommonMethods
    ''' <summary>
    ''' class used for common property valuation methods
    ''' </summary>
    ''' <remarks>currently using e2Value</remarks>
    <Serializable()>
    Public Class QuickQuotePropertyValuationHelperClass 'added 8/7/2014 for e2Value stuff; some 8/6/2014 code migrated from QuickQuoteHelperClass

        '8/7/2014 note: might move here from QuickQuotePropertyValuation
        'Enum ValuationVendor 'needs to match id from PropertyValuationVendor table in QuickQuote database (propertyValuationVendorId)
        '    None = 0 'not a value in the db table
        '    e2Value = 1 'db value is the same
        'End Enum
        'Enum ValuationVendorIntegrationType 'needs to match id from PropertyValuationVendorIntegrationType table in QuickQuote database (propertyValuationVendorIntegrationTypeId)
        '    None = 0 'not a value in the db table
        '    AdvancedPortico = 1 'db value = Advanced Portico Integration
        '    Xml = 2 'db value = Xml Integration
        'End Enum

        'added 7/28/2015 for Farm
        Enum ValuationPropertyType
            None = 0
            Location = 1
            Building = 2
            LocationAndBuildings = 3
            DefaultByInfo = 4 'added 7/31/2015
        End Enum

        Dim qqHelper As New QuickQuoteHelperClass
        Dim qqXml As New QuickQuoteXML

        'added 8/7/2014
        'Public Sub PopulateE2ValuePropertyValuationFromQuote(ByRef qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1, Optional ByVal resetValuation As Boolean = False)
        'updated 8/20/2014 for normalSave param; removed 8/21/2014
        'Public Sub PopulateE2ValuePropertyValuationFromQuote(ByRef qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1, Optional ByVal normalSave As Boolean = False, Optional ByVal resetValuation As Boolean = False)
        '    If locationNum < 1 Then
        '        locationNum = 1
        '    End If

        '    If qq IsNot Nothing Then
        '        If qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locationNum Then
        '            If qq.Locations(locationNum - 1) IsNot Nothing Then
        '                With qq.Locations(locationNum - 1)
        '                    'PopulateE2ValuePropertyValuationFromQuote(qq, .PropertyValuation, locationNum, resetValuation)
        '                    'updated 8/20/2014 for normalSave param
        '                    PopulateE2ValuePropertyValuationFromQuote(qq, .PropertyValuation, locationNum, normalSave, resetValuation)
        '                End With
        '            End If
        '        End If

        '    End If
        'End Sub
        'added 8/20/2014 pm
        'now need to update SaveQuote functionality to save modifier if propertyValuationId is numeric or if ArchitecturalStyle is something
        'then need to update Location Modifier parsing logic to look for specific id and split text to find propertyValuationId or ArchitecturalStyle or both, in which case Loc.ArchitecturalStyle will use stand-along value instead of one from PropertyValuation.Request

        'Public Sub Populate360ValuePropertyValuationFromQuoteAndSetUrl(ByRef qqo As QuickQuoteObject, ByRef valuationUrl As String, Optional ByVal locationNum As Integer = 1, Optional ByVal saveToQuote As Boolean = True, Optional ByRef saveWasSuccessful As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal sendAllParamsForExistingValuation As Boolean = False) '8/21/2014 - updated saveToQuote default value from False to True
        '    'Populate360ValuePropertyValuationFromQuoteAndSetUrl(qqo, valuationUrl, locationNum, 0, saveToQuote, saveWasSuccessful, errorMsg, ValuationPropertyType.DefaultByInfo, sendAllParamsForExistingValuation)
        '    If qqo IsNot Nothing Then
        '        If qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 AndAlso qqo.Locations(0) IsNot Nothing AndAlso qqo.Locations(0).Address IsNot Nothing Then
        '            Dim chc As New CommonHelperClass
        '            Dim apiKey As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_Verisk360_IntegrationCall_APIKey")
        '            Dim SnapLogicVerisk360valuationURL As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_SnapLogic_verisk360valuation_URL")
        '            Dim myAddress As QuickQuoteAddress
        '            Dim site360Value_Url As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_Verisk360_Url")
        '            Dim site360Value_Return_Url As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_Verisk360_ReturnUrl")
        '            Dim isNewBusiness As Boolean = qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote

        '            Dim qqLocation As QuickQuoteLocation = qqo.Locations(0)
        '            Try
        '                Using client As New Net.WebClient()
        '                    client.Headers.Add("x-api-key", apiKey) ' Replace with actual API key
        '                    client.Headers.Add(Net.HttpRequestHeader.ContentType, "application/json")

        '                    myAddress = qqo.Locations(0).Address

        '                    ' Create request data
        '                    ' Declare the variable
        '                    'Assign values to the properties

        '                    'Dim verisk360ValuationRequest As Verisk360PropertyValuationRequest_backup = New Verisk360PropertyValuationRequest_backup With {
        '                    Dim verisk360ValuationRequest As QuickQuotePropertyValuationRequest = New QuickQuotePropertyValuationRequest With {
        '                    .firstName = "IndianaFarmers" & qqo.AgencyCode,
        '                    .lastName = "Agency" & qqo.AgencyCode,
        '                    .email = qqo.AgencyCode & "@indianafarmers.com",
        '                    .agencyName = qqo.AgencyCode,
        '                        .clientName = If(qqo.Client IsNot Nothing, qqo.Client.Name.FirstName & " " & qqo.Client.Name.MiddleName & " " & qqo.Client.Name.LastName, ""),
        '                        .street = myAddress.HouseNum & " " & myAddress.StreetName,
        '                        .unit = myAddress.ApartmentNumber,
        '                        .city = Left(myAddress.City, 100),
        '                        .state = Left(myAddress.State, 2),
        '                        .zipCode = Left(myAddress.Zip, 5),
        '                        .country = "USA", 'HardCoded for now
        '                        .YearBuilt = qqLocation.YearBuilt,
        '                        .SquareFeet = qqLocation.SquareFeet,
        '                        .policyId = If(isNewBusiness, "", qqo.PolicyId),
        '                        .policyImageNumber = If(isNewBusiness, "", qqo.PolicyImageNum),
        '                        .policyNumber = If(isNewBusiness, "", qqo.PolicyImageNum),
        '                        .quoteId = If(isNewBusiness, qqo.Database_QuoteId, ""),
        '                        .quoteNumber = If(isNewBusiness, qqo.QuoteNumber, "")
        '                    }

        '                    ' Convert to JSON
        '                    Dim jsonContent As String = JsonConvert.SerializeObject(verisk360ValuationRequest)

        '                    Dim jsonObject As JObject = JObject.Parse(jsonContent)

        '                    ' Update the specific property to camelCase as API is expecting 
        '                    jsonObject("yearBuilt") = jsonObject("YearBuilt")
        '                    jsonObject("squareFeet") = jsonObject("SquareFeet")
        '                    jsonObject.Remove("YearBuilt")
        '                    jsonObject.Remove("SquareFeet")

        '                    ' Convert the JObject back to a JSON string
        '                    jsonContent = jsonObject.ToString()


        '                    Dim responseString As String = client.UploadString(SnapLogicVerisk360valuationURL, jsonContent)

        '                    ' Extract token from JSON response
        '                    Dim token As String = ExtractToken(responseString)
        '                    ' Use the token as needed
        '                    valuationUrl = site360Value_Url + "?sso_token=" + token + "&return_url=" + site360Value_Return_Url
        '                End Using
        '            Catch ex As Net.WebException
        '                ' Handle WebException
        '                saveWasSuccessful = False
        '                errorMsg = ex.Message
        '            Catch ex As Exception
        '                ' Handle other exceptions
        '                saveWasSuccessful = False
        '                errorMsg = ex.Message
        '            End Try
        '        End If

        '    End If

        'End Sub

        Public Shared Function ExtractVendorValuationParams(jsonResponse As String, ByRef pvr As QuickQuotePropertyValuationRequest) As String
            Try
                Dim jsonArray As JArray = JArray.Parse(jsonResponse)

                If jsonArray.Count = 0 Then
                    Return "Error: Empty response"
                End If

                Dim firstObject As JObject = jsonArray(0)

                pvr.VendorValuationId = firstObject.SelectToken("actual.valuationId")?.ToString()
                Dim token As String = firstObject.SelectToken("actual.token")?.ToString()

                If String.IsNullOrEmpty(token) Then
                    Return "Token Not Found"
                End If

                Return token
            Catch ex As Exception
                Return ex.Message
            End Try
        End Function

        'Public Sub PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(ByRef qq As QuickQuoteObject, ByRef vendorValueUrl As String, Optional ByVal locationNum As Integer = 1, Optional ByVal saveToQuote As Boolean = True, Optional ByRef saveWasSuccessful As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal sendAllParamsForExistingValuation As Boolean = False, Optional ByVal valuationVendor As Integer = QuickQuotePropertyValuation.ValuationVendor.e2Value)
        Public Sub PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(ByRef qq As QuickQuoteObject, ByRef vendorValueUrl As String, Optional ByVal locationNum As Integer = 1, Optional ByVal saveToQuote As Boolean = True, Optional ByRef saveWasSuccessful As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal sendAllParamsForExistingValuation As Boolean = False, Optional ByVal valuationVendor As Integer = QuickQuotePropertyValuation.ValuationVendor.e2Value)
            'If locationNum < 1 Then
            '    locationNum = 1
            'End If

            'TODO Monika: to hanle existing valuation , Ex: previously it is valuated under e2 now falg is changed so we meight requreid to update the existing db vendor to 360, modify code from 198 to 204
            'If qq IsNot Nothing Then
            '    If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > locationNum - 1 Then
            '        With qq.Locations(locationNum-1).PropertyValuation
            '            .Vendor = valuationVendor
            '        End With
            '    End If
            'End If

            'If qq IsNot Nothing Then
            '    If qq.Database_QuoteId <> "" AndAlso IsNumeric(qq.Database_QuoteId) = True Then
            '        If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
            '            If qq.Locations.Count >= locationNum Then
            '                With qq.Locations(locationNum - 1)
            '                    Dim resetVal As Boolean = False
            '                    If .PropertyValuation Is Nothing Then
            '                        resetVal = True 'not really needed since it will be instantiated anyway
            '                    Else
            '                        With .PropertyValuation
            '                            If .db_propertyValuationId <> "" AndAlso IsNumeric(.db_propertyValuationId) = True Then
            '                                'valid propertyValuationId
            '                                If .IFM_UniqueValuationId <> "" Then
            '                                    'has something for IFM_UniqueValuationId
            '                                    'If .Vendor = QuickQuotePropertyValuation.ValuationVendor.e2Value AndAlso .VendorIntegrationType = QuickQuotePropertyValuation.ValuationVendorIntegrationType.AdvancedPortico AndAlso .VendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential AndAlso .db_environment = helper.Environment Then
            '                                    'updated 4/29/2015 to handle multiple LOBs using different estimators
            '                                    If .Vendor = QuickQuotePropertyValuation.ValuationVendor.e2Value AndAlso .VendorIntegrationType = QuickQuotePropertyValuation.ValuationVendorIntegrationType.AdvancedPortico AndAlso .VendorEstimatorType = GetEstimatorTypeForQuote(qq) AndAlso .db_environment = helper.Environment Then
            '                                        'same vendor, types (integration and estimator), and environment
            '                                        If qq.AgencyCode <> "" AndAlso .db_agencyCode <> "" AndAlso Right(qq.AgencyCode, 4) <> Right(.db_agencyCode, 4) Then
            '                                            resetVal = True
            '                                        Else
            '                                            'same agency code or missing on one or both
            '                                            'might validate more
            '                                        End If
            '                                    Else
            '                                        resetVal = True
            '                                    End If
            '                                Else
            '                                    resetVal = True
            '                                End If
            '                            Else
            '                                resetVal = True
            '                            End If
            '                        End With
            '                        If resetVal = True Then
            '                            .PropertyValuation.Dispose()
            '                            .PropertyValuation = Nothing
            '                        End If
            '                    End If
            '                    If resetVal = True Then
            '                        'PopulateE2ValuePropertyValuationFromQuote(qq, locationNum) 'should probably add errorMsg param to method also
            '                        'updated 8/21/2014 to use different method since original one was removed
            '                        'PopulateE2ValuePropertyValuationFromQuote(qq, .PropertyValuation, locationNum)
            '                        'updated 8/21/2014 for sendAllParamsForExistingValuation; not really needed since a new valuation would be create w/ this code path
            '                        PopulateE2ValuePropertyValuationFromQuote(qq, .PropertyValuation, locationNum, False, False, sendAllParamsForExistingValuation)
            '                    Else
            '                        'Dim resetValReq As Boolean = False
            '                        'Dim resetValRes As Boolean = False

            '                        'If .PropertyValuation.Request Is Nothing Then
            '                        '    resetValReq = True
            '                        'Else
            '                        '    If .PropertyValuation.Request.db_propertyValuationId = .PropertyValuation.db_propertyValuationId Then
            '                        '        'same propertyValuationId
            '                        '        If .PropertyValuation.Request.IFM_UniqueValuationId = .PropertyValuation.IFM_UniqueValuationId Then
            '                        '            'same IFM_UniqueValuationId
            '                        '            'might validate more
            '                        '        Else
            '                        '            resetValReq = True
            '                        '        End If
            '                        '    Else
            '                        '        resetValReq = True
            '                        '    End If
            '                        'End If

            '                        'will just reset request and response every time
            '                        If .PropertyValuation.Request IsNot Nothing Then
            '                            .PropertyValuation.Request.Dispose()
            '                            .PropertyValuation.Request = Nothing
            '                        End If
            '                        .PropertyValuation.Request = New QuickQuotePropertyValuationRequest
            '                        .PropertyValuation.Request.db_propertyValuationId = .PropertyValuation.db_propertyValuationId
            '                        .PropertyValuation.Request.IFM_UniqueValuationId = .PropertyValuation.IFM_UniqueValuationId
            '                        .PropertyValuation.Request.VendorValuationId = .PropertyValuation.VendorValuationId
            '                        '.PropertyValuation.Request.ReturnUrl = helper.configAppSettingValueAsString("e2Value_ReturnUrl")
            '                        'updated 5/27/2015 to use new function
            '                        .PropertyValuation.Request.ReturnUrl = E2ValueReturnUrl()
            '                        .PropertyValuation.Request.ReturnUrlLinkText = "Return to VelociRater quote"
            '                        If qq IsNot Nothing AndAlso qq.QuoteNumber <> "" Then
            '                            .PropertyValuation.Request.ReturnUrlLinkText &= " (" & qq.QuoteNumber & ")"
            '                        End If
            '                        'PopulateE2ValueRequestFromQuote(qq, .PropertyValuation.Request, locationNum) 'should probably add errorMsg param to method also
            '                        'updated 8/21/2014 so other params sent from propertyValuation are used; and also sendAllParamsForExistingValuation
            '                        PopulateE2ValueRequestFromQuote(qq, .PropertyValuation.Request, locationNum, False, .PropertyValuation.db_propertyValuationId, .PropertyValuation.IFM_UniqueValuationId, .PropertyValuation.VendorValuationId, .PropertyValuation.Vendor, .PropertyValuation.VendorEstimatorType, False, sendAllParamsForExistingValuation)
            '                        If .PropertyValuation.Request.db_propertyValuationRequestId <> "" AndAlso IsNumeric(.PropertyValuation.Request.db_propertyValuationRequestId) = True Then
            '                            .PropertyValuation.db_propertyValuationRequestId = .PropertyValuation.Request.db_propertyValuationRequestId
            '                            .PropertyValuation.db_propertyValuationResponseId = ""
            '                        End If

            '                        If .PropertyValuation.Response IsNot Nothing Then
            '                            .PropertyValuation.Response.Dispose()
            '                            .PropertyValuation.Response = Nothing
            '                        End If
            '                        .PropertyValuation.Response = New QuickQuotePropertyValuationResponse
            '                    End If

            '                    If .PropertyValuation IsNot Nothing AndAlso .PropertyValuation.db_propertyValuationId <> "" AndAlso IsNumeric(.PropertyValuation.db_propertyValuationId) = True Then
            '                        'there's something saved
            '                        If .PropertyValuation.Request IsNot Nothing AndAlso .PropertyValuation.Request.VendorParams <> "" Then
            '                            'If UCase(.PropertyValuation.Request.VendorParams).Contains("HTTP:") = True OrElse UCase(.PropertyValuation.Request.VendorParams).Contains("WWW.") = True Then
            '                            'updated 8/27/2014 to include https:
            '                            If UCase(.PropertyValuation.Request.VendorParams).Contains("HTTPS:") = True OrElse UCase(.PropertyValuation.Request.VendorParams).Contains("HTTP:") = True OrElse UCase(.PropertyValuation.Request.VendorParams).Contains("WWW.") = True Then
            '                                e2ValueUrl = .PropertyValuation.Request.VendorParams
            '                            Else
            '                                'may do something here... like separate URL method from VendorParams
            '                            End If
            '                        End If
            '                        If saveToQuote = True Then
            '                            Dim saveErrorMsg As String = ""
            '                            qqXml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qq.Database_QuoteId, saveErrorMsg)
            '                            If saveErrorMsg = "" Then
            '                                saveWasSuccessful = True
            '                            Else 'added 8/21/2014
            '                                errorMsg = qqHelper.appendText(errorMsg, "problem saving quote: " & saveErrorMsg, "; ")
            '                            End If
            '                        End If
            '                    Else 'added 8/21/2014
            '                        errorMsg = "problem inserting property valuation in database"
            '                        If saveToQuote = True Then
            '                            errorMsg &= "; quote save not attempted"
            '                        End If
            '                    End If
            '                End With
            '            Else
            '                errorMsg = "the location specified doesn't exist on the QuickQuote object"
            '            End If
            '        Else
            '            errorMsg = "no locations found on QuickQuote object"
            '        End If
            '    Else
            '        errorMsg = "QuickQuote object must have a valid quoteId"
            '    End If
            'Else
            '    errorMsg = "valid QuickQuote object required"
            'End If
            'updated 7/28/2015 to call new overload; defaulting buildingNum param to 0
            'PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(qq, e2ValueUrl, locationNum, 0, saveToQuote, saveWasSuccessful, errorMsg, sendAllParamsForExistingValuation)
            'updated 7/31/2015 for optional propertyType param
            PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(qq, vendorValueUrl, locationNum, 0, saveToQuote, saveWasSuccessful, errorMsg, ValuationPropertyType.DefaultByInfo, sendAllParamsForExistingValuation, valuationVendor)

        End Sub
        'added overload 7/28/2015 for buildingNum; originally copied from PopulateVendorValuePropertyValuationFromQuoteAndSetUrl
        'Public Sub PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(ByRef qq As QuickQuoteObject, ByRef e2ValueUrl As String, ByVal locationNum As Integer, ByVal buildingNum As Integer, Optional ByVal saveToQuote As Boolean = True, Optional ByRef saveWasSuccessful As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal sendAllParamsForExistingValuation As Boolean = False) '8/21/2014 - updated saveToQuote default value from False to True
        '7/29/2015 - updated some params to be required instead of optional so there would be less chance of matching signatures on the methods w/ the same name
        'Public Sub PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(ByRef qq As QuickQuoteObject, ByRef e2ValueUrl As String, ByVal locationNum As Integer, ByVal buildingNum As Integer, ByVal saveToQuote As Boolean, ByRef saveWasSuccessful As Boolean, ByRef errorMsg As String, Optional ByVal sendAllParamsForExistingValuation As Boolean = False)
        'updated 7/31/2015 for optional propertyType param
        Public Sub PopulateVendorValuePropertyValuationFromQuoteAndSetUrl(ByRef qq As QuickQuoteObject, ByRef vendorValueUrl As String, ByVal locationNum As Integer, ByVal buildingNum As Integer, ByVal saveToQuote As Boolean, ByRef saveWasSuccessful As Boolean, ByRef errorMsg As String, Optional ByVal propertyType As ValuationPropertyType = ValuationPropertyType.DefaultByInfo, Optional ByVal sendAllParamsForExistingValuation As Boolean = False, Optional ByVal valuationVendor As QuickQuotePropertyValuation.ValuationVendor = QuickQuotePropertyValuation.ValuationVendor.e2Value)
            If locationNum < 1 Then
                locationNum = 1
            End If
            'added 7/31/2015
            If propertyType = ValuationPropertyType.None OrElse propertyType = ValuationPropertyType.DefaultByInfo Then
                'propertyType = ValuationPropertyTypeDefaultByInfo(locationNum, buildingNum)
                'updated 8/28/2015 to send qq
                propertyType = ValuationPropertyTypeDefaultByInfo(locationNum, buildingNum, qq)
            End If

            If qq IsNot Nothing Then
                'If qq.Database_QuoteId <> "" AndAlso IsNumeric(qq.Database_QuoteId) = True Then
                If (qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote AndAlso qq.Database_QuoteId <> "" AndAlso IsNumeric(qq.Database_QuoteId)) OrElse
                    (qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote AndAlso qq.PolicyId <> "" AndAlso qq.PolicyImageNum <> "" AndAlso IsNumeric(qq.PolicyId) AndAlso IsNumeric(qq.PolicyImageNum)) Then
                    If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
                        If qq.Locations.Count >= locationNum Then
                            With qq.Locations(locationNum - 1)
                                If buildingNum > 0 Then 'added IF 7/28/2015; original logic in ELSE
                                    If .Buildings IsNot Nothing AndAlso .Buildings.Count >= buildingNum Then
                                        With .Buildings(buildingNum - 1)
                                            Dim resetVal As Boolean = False
                                            If .PropertyValuation Is Nothing Then
                                                resetVal = True 'not really needed since it will be instantiated anyway
                                            Else
                                                With .PropertyValuation
                                                    If .db_propertyValuationId <> "" AndAlso IsNumeric(.db_propertyValuationId) = True Then
                                                        'valid propertyValuationId
                                                        If .IFM_UniqueValuationId <> "" Then
                                                            'has something for IFM_UniqueValuationId
                                                            'If .Vendor = QuickQuotePropertyValuation.ValuationVendor.e2Value AndAlso .VendorIntegrationType = QuickQuotePropertyValuation.ValuationVendorIntegrationType.AdvancedPortico AndAlso .VendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential AndAlso .db_environment = helper.Environment Then
                                                            'updated 4/29/2015 to handle multiple LOBs using different estimators
                                                            'If .Vendor = QuickQuotePropertyValuation.ValuationVendor.e2Value AndAlso .VendorIntegrationType = QuickQuotePropertyValuation.ValuationVendorIntegrationType.AdvancedPortico AndAlso .VendorEstimatorType = GetEstimatorTypeForQuote(qq) AndAlso .db_environment = helper.Environment Then
                                                            'updated 7/28/2015 for buildingNum and GetEstimatorType
                                                            'If .Vendor = QuickQuotePropertyValuation.ValuationVendor.e2Value AndAlso .VendorIntegrationType = QuickQuotePropertyValuation.ValuationVendorIntegrationType.AdvancedPortico AndAlso .VendorEstimatorType = GetEstimatorTypeForQuote(qq, ValuationPropertyType.Building) AndAlso .db_environment = helper.Environment Then
                                                            'updated 7/31/2015 to use propertyType param
                                                            If (.Vendor = QuickQuotePropertyValuation.ValuationVendor.e2Value OrElse .Vendor = QuickQuotePropertyValuation.ValuationVendor.Verisk360) AndAlso .VendorIntegrationType = QuickQuotePropertyValuation.ValuationVendorIntegrationType.AdvancedPortico AndAlso .VendorEstimatorType = GetEstimatorTypeForQuote(qq, propertyType) AndAlso .db_environment = helper.Environment Then
                                                                'same vendor, types (integration and estimator), and environment
                                                                If qq.AgencyCode <> "" AndAlso .db_agencyCode <> "" AndAlso Right(qq.AgencyCode, 4) <> Right(.db_agencyCode, 4) Then
                                                                    resetVal = True
                                                                Else
                                                                    'same agency code or missing on one or both
                                                                    'might validate more
                                                                End If
                                                            Else
                                                                resetVal = True
                                                            End If
                                                        Else
                                                            resetVal = True
                                                        End If
                                                    Else
                                                        resetVal = True
                                                    End If
                                                End With
                                                If resetVal = True Then
                                                    .PropertyValuation.Dispose()
                                                    .PropertyValuation = Nothing
                                                End If
                                            End If
                                            If resetVal = True Then
                                                'PopulateE2ValuePropertyValuationFromQuote(qq, locationNum) 'should probably add errorMsg param to method also
                                                'updated 8/21/2014 to use different method since original one was removed
                                                'PopulateE2ValuePropertyValuationFromQuote(qq, .PropertyValuation, locationNum)
                                                'updated 8/21/2014 for sendAllParamsForExistingValuation; not really needed since a new valuation would be create w/ this code path
                                                'PopulateE2ValuePropertyValuationFromQuote(qq, .PropertyValuation, locationNum, False, False, sendAllParamsForExistingValuation)
                                                'updated 7/28/2015 for buildingNum
                                                'PopulateE2ValuePropertyValuationFromQuote(qq, .PropertyValuation, locationNum, buildingNum, False, False, sendAllParamsForExistingValuation)
                                                'updated 7/31/2015 for propertyType
                                                PopulateVendorValuePropertyValuationFromQuote(qq, .PropertyValuation, locationNum, buildingNum, False, False, propertyType, sendAllParamsForExistingValuation, valuationVendor)
                                            Else
                                                'Dim resetValReq As Boolean = False
                                                'Dim resetValRes As Boolean = False

                                                'If .PropertyValuation.Request Is Nothing Then
                                                '    resetValReq = True
                                                'Else
                                                '    If .PropertyValuation.Request.db_propertyValuationId = .PropertyValuation.db_propertyValuationId Then
                                                '        'same propertyValuationId
                                                '        If .PropertyValuation.Request.IFM_UniqueValuationId = .PropertyValuation.IFM_UniqueValuationId Then
                                                '            'same IFM_UniqueValuationId
                                                '            'might validate more
                                                '        Else
                                                '            resetValReq = True
                                                '        End If
                                                '    Else
                                                '        resetValReq = True
                                                '    End If
                                                'End If

                                                'will just reset request and response every time
                                                If .PropertyValuation.Request IsNot Nothing Then
                                                    .PropertyValuation.Request.Dispose()
                                                    .PropertyValuation.Request = Nothing
                                                End If
                                                .PropertyValuation.Request = New QuickQuotePropertyValuationRequest
                                                .PropertyValuation.Request.db_propertyValuationId = .PropertyValuation.db_propertyValuationId
                                                .PropertyValuation.Request.IFM_UniqueValuationId = .PropertyValuation.IFM_UniqueValuationId
                                                .PropertyValuation.Request.VendorValuationId = .PropertyValuation.VendorValuationId
                                                '.PropertyValuation.Request.ReturnUrl = helper.configAppSettingValueAsString("e2Value_ReturnUrl")
                                                'updated 5/27/2015 to use new function
                                                .PropertyValuation.Request.ReturnUrl = VendorValuationReturnUrl(valuationVendor)
                                                '.PropertyValuation.Request.ReturnUrlLinkText = "Return to VelociRater quote"
                                                'If qq IsNot Nothing AndAlso qq.QuoteNumber <> "" Then
                                                '    .PropertyValuation.Request.ReturnUrlLinkText &= " (" & qq.QuoteNumber & ")"
                                                'End If
                                                If qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                                    .PropertyValuation.Request.ReturnUrlLinkText = "Return to VelociRater policy"
                                                    If qq IsNot Nothing AndAlso qq.PolicyNumber <> "" Then
                                                        .PropertyValuation.Request.ReturnUrlLinkText &= " (" & qq.PolicyNumber & ")"
                                                    End If
                                                Else
                                                    .PropertyValuation.Request.ReturnUrlLinkText = "Return to VelociRater quote"
                                                    If qq IsNot Nothing AndAlso qq.QuoteNumber <> "" Then
                                                        .PropertyValuation.Request.ReturnUrlLinkText &= " (" & qq.QuoteNumber & ")"
                                                    End If
                                                End If

                                                'PopulateE2ValueRequestFromQuote(qq, .PropertyValuation.Request, locationNum) 'should probably add errorMsg param to method also
                                                'updated 8/21/2014 so other params sent from propertyValuation are used; and also sendAllParamsForExistingValuation
                                                'PopulateE2ValueRequestFromQuote(qq, .PropertyValuation.Request, locationNum, False, .PropertyValuation.db_propertyValuationId, .PropertyValuation.IFM_UniqueValuationId, .PropertyValuation.VendorValuationId, .PropertyValuation.Vendor, .PropertyValuation.VendorEstimatorType, False, sendAllParamsForExistingValuation)
                                                'updated 7/28/2015 for buildingNum
                                                PopulateVendorValueRequestFromQuote(qq, .PropertyValuation.Request, locationNum, buildingNum, False, .PropertyValuation.db_propertyValuationId, .PropertyValuation.IFM_UniqueValuationId, .PropertyValuation.VendorValuationId, .PropertyValuation.Vendor, .PropertyValuation.VendorEstimatorType, False, sendAllParamsForExistingValuation)
                                                If .PropertyValuation.Request.db_propertyValuationRequestId <> "" AndAlso IsNumeric(.PropertyValuation.Request.db_propertyValuationRequestId) = True Then
                                                    .PropertyValuation.db_propertyValuationRequestId = .PropertyValuation.Request.db_propertyValuationRequestId
                                                    .PropertyValuation.db_propertyValuationResponseId = ""
                                                End If

                                                If .PropertyValuation.Response IsNot Nothing Then
                                                    .PropertyValuation.Response.Dispose()
                                                    .PropertyValuation.Response = Nothing
                                                End If
                                                .PropertyValuation.Response = New QuickQuotePropertyValuationResponse
                                            End If

                                            If .PropertyValuation IsNot Nothing AndAlso .PropertyValuation.db_propertyValuationId <> "" AndAlso IsNumeric(.PropertyValuation.db_propertyValuationId) = True Then
                                                'there's something saved
                                                If .PropertyValuation.Request IsNot Nothing AndAlso .PropertyValuation.Request.VendorParams <> "" Then
                                                    'If UCase(.PropertyValuation.Request.VendorParams).Contains("HTTP:") = True OrElse UCase(.PropertyValuation.Request.VendorParams).Contains("WWW.") = True Then
                                                    'updated 8/27/2014 to include https:
                                                    If UCase(.PropertyValuation.Request.VendorParams).Contains("HTTPS:") = True OrElse UCase(.PropertyValuation.Request.VendorParams).Contains("HTTP:") = True OrElse UCase(.PropertyValuation.Request.VendorParams).Contains("WWW.") = True Then
                                                        vendorValueUrl = .PropertyValuation.Request.VendorParams
                                                    Else
                                                        'may do something here... like separate URL method from VendorParams
                                                    End If
                                                End If
                                                If saveToQuote = True Then
                                                    Dim saveErrorMsg As String = ""
                                                    'qqXml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qq.Database_QuoteId, saveErrorMsg)
                                                    If qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                                        Dim success As Boolean = False
                                                        Dim qqEndorsementResults = Nothing 'only used for debug
                                                        success = qqXml.SuccessfullySavedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn_WithqqoMethodToCall(qq, qqeResults:=qqEndorsementResults, errorMessage:=saveErrorMsg)
                                                    Else
                                                        qqXml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qq.Database_QuoteId, saveErrorMsg)
                                                    End If
                                                    If saveErrorMsg = "" Then
                                                        saveWasSuccessful = True
                                                    Else 'added 8/21/2014
                                                        errorMsg = qqHelper.appendText(errorMsg, "problem saving quote: " & saveErrorMsg, "; ")
                                                    End If
                                                End If
                                            Else 'added 8/21/2014
                                                errorMsg = "problem inserting property valuation in database"
                                                If saveToQuote = True Then
                                                    errorMsg &= "; quote save not attempted"
                                                End If
                                            End If
                                        End With
                                    Else
                                        errorMsg = "the location/building specified doesn't exist on the QuickQuote object"
                                    End If
                                Else 'original logic below (for just Location)
                                    Dim resetVal As Boolean = False
                                    If .PropertyValuation Is Nothing Then
                                        resetVal = True 'not really needed since it will be instantiated anyway
                                    Else
                                        With .PropertyValuation
                                            .Vendor = valuationVendor 'To replace existing vendor to latst vendor
                                            If .db_propertyValuationId <> "" AndAlso IsNumeric(.db_propertyValuationId) = True Then
                                                'valid propertyValuationId
                                                If .IFM_UniqueValuationId <> "" Then
                                                    'has something for IFM_UniqueValuationId
                                                    'If .Vendor = QuickQuotePropertyValuation.ValuationVendor.e2Value AndAlso .VendorIntegrationType = QuickQuotePropertyValuation.ValuationVendorIntegrationType.AdvancedPortico AndAlso .VendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential AndAlso .db_environment = helper.Environment Then
                                                    'updated 4/29/2015 to handle multiple LOBs using different estimators
                                                    '7/28/2015 note: may need to determine propertyType when the buildingNum isn't specified... could be HOM loc w/ no buildings or FAR loc w/ buildings
                                                    If (.Vendor = QuickQuotePropertyValuation.ValuationVendor.e2Value OrElse .Vendor = QuickQuotePropertyValuation.ValuationVendor.Verisk360) AndAlso .VendorIntegrationType = QuickQuotePropertyValuation.ValuationVendorIntegrationType.AdvancedPortico AndAlso .VendorEstimatorType = GetEstimatorTypeForQuote(qq) AndAlso .db_environment = helper.Environment Then
                                                        'same vendor, types (integration and estimator), and environment
                                                        If qq.AgencyCode <> "" AndAlso .db_agencyCode <> "" AndAlso Right(qq.AgencyCode, 4) <> Right(.db_agencyCode, 4) Then
                                                            resetVal = True
                                                        Else
                                                            'same agency code or missing on one or both
                                                            'might validate more
                                                        End If
                                                    Else
                                                        resetVal = True
                                                    End If
                                                Else
                                                    resetVal = True
                                                End If
                                            Else
                                                resetVal = True
                                            End If
                                        End With
                                        If resetVal = True Then
                                            .PropertyValuation.Dispose()
                                            .PropertyValuation = Nothing
                                        End If
                                    End If
                                    If resetVal = True Then
                                        'PopulateE2ValuePropertyValuationFromQuote(qq, locationNum) 'should probably add errorMsg param to method also
                                        'updated 8/21/2014 to use different method since original one was removed
                                        'PopulateE2ValuePropertyValuationFromQuote(qq, .PropertyValuation, locationNum)
                                        'updated 8/21/2014 for sendAllParamsForExistingValuation; not really needed since a new valuation would be create w/ this code path
                                        'PopulateE2ValuePropertyValuationFromQuote(qq, .PropertyValuation, locationNum, False, False, sendAllParamsForExistingValuation)
                                        'updated 7/28/2015 for buildingNum
                                        'PopulateE2ValuePropertyValuationFromQuote(qq, .PropertyValuation, locationNum, buildingNum, False, False, sendAllParamsForExistingValuation)
                                        'updated 7/31/2015 for propertyType
                                        PopulateVendorValuePropertyValuationFromQuote(qq, .PropertyValuation, locationNum, buildingNum, False, False, propertyType, sendAllParamsForExistingValuation, valuationVendor)
                                    Else
                                        'Dim resetValReq As Boolean = False
                                        'Dim resetValRes As Boolean = False

                                        'If .PropertyValuation.Request Is Nothing Then
                                        '    resetValReq = True
                                        'Else
                                        '    If .PropertyValuation.Request.db_propertyValuationId = .PropertyValuation.db_propertyValuationId Then
                                        '        'same propertyValuationId
                                        '        If .PropertyValuation.Request.IFM_UniqueValuationId = .PropertyValuation.IFM_UniqueValuationId Then
                                        '            'same IFM_UniqueValuationId
                                        '            'might validate more
                                        '        Else
                                        '            resetValReq = True
                                        '        End If
                                        '    Else
                                        '        resetValReq = True
                                        '    End If
                                        'End If

                                        'will just reset request and response every time
                                        If .PropertyValuation.Request IsNot Nothing Then
                                            .PropertyValuation.Request.Dispose()
                                            .PropertyValuation.Request = Nothing
                                        End If
                                        .PropertyValuation.Request = New QuickQuotePropertyValuationRequest
                                        .PropertyValuation.Request.db_propertyValuationId = .PropertyValuation.db_propertyValuationId
                                        .PropertyValuation.Request.IFM_UniqueValuationId = .PropertyValuation.IFM_UniqueValuationId
                                        .PropertyValuation.Request.VendorValuationId = .PropertyValuation.VendorValuationId
                                        '.PropertyValuation.Request.ReturnUrl = helper.configAppSettingValueAsString("e2Value_ReturnUrl")
                                        'updated 5/27/2015 to use new function
                                        .PropertyValuation.Request.ReturnUrl = VendorValuationReturnUrl(valuationVendor)
                                        '.PropertyValuation.Request.ReturnUrlLinkText = "Return to VelociRater quote"
                                        'If qq IsNot Nothing AndAlso qq.QuoteNumber <> "" Then
                                        '    .PropertyValuation.Request.ReturnUrlLinkText &= " (" & qq.QuoteNumber & ")"
                                        'End If
                                        If qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                            .PropertyValuation.Request.ReturnUrlLinkText = "Return to VelociRater policy"
                                            If qq IsNot Nothing AndAlso qq.PolicyNumber <> "" Then
                                                .PropertyValuation.Request.ReturnUrlLinkText &= " (" & qq.PolicyNumber & ")"
                                            End If
                                        Else
                                            .PropertyValuation.Request.ReturnUrlLinkText = "Return to VelociRater quote"
                                            If qq IsNot Nothing AndAlso qq.QuoteNumber <> "" Then
                                                .PropertyValuation.Request.ReturnUrlLinkText &= " (" & qq.QuoteNumber & ")"
                                            End If
                                        End If
                                        'PopulateE2ValueRequestFromQuote(qq, .PropertyValuation.Request, locationNum) 'should probably add errorMsg param to method also
                                        'updated 8/21/2014 so other params sent from propertyValuation are used; and also sendAllParamsForExistingValuation
                                        'PopulateE2ValueRequestFromQuote(qq, .PropertyValuation.Request, locationNum, False, .PropertyValuation.db_propertyValuationId, .PropertyValuation.IFM_UniqueValuationId, .PropertyValuation.VendorValuationId, .PropertyValuation.Vendor, .PropertyValuation.VendorEstimatorType, False, sendAllParamsForExistingValuation)
                                        'updated 7/28/2015 for buildingNum
                                        PopulateVendorValueRequestFromQuote(qq, .PropertyValuation.Request, locationNum, buildingNum, False, .PropertyValuation.db_propertyValuationId, .PropertyValuation.IFM_UniqueValuationId, .PropertyValuation.VendorValuationId, .PropertyValuation.Vendor, .PropertyValuation.VendorEstimatorType, False, sendAllParamsForExistingValuation)
                                        If .PropertyValuation.Request.db_propertyValuationRequestId <> "" AndAlso IsNumeric(.PropertyValuation.Request.db_propertyValuationRequestId) = True Then
                                            .PropertyValuation.db_propertyValuationRequestId = .PropertyValuation.Request.db_propertyValuationRequestId
                                            .PropertyValuation.db_propertyValuationResponseId = ""
                                        End If

                                        If .PropertyValuation.Response IsNot Nothing Then
                                            .PropertyValuation.Response.Dispose()
                                            .PropertyValuation.Response = Nothing
                                        End If
                                        .PropertyValuation.Response = New QuickQuotePropertyValuationResponse
                                    End If

                                    If .PropertyValuation IsNot Nothing AndAlso .PropertyValuation.db_propertyValuationId <> "" AndAlso IsNumeric(.PropertyValuation.db_propertyValuationId) = True Then
                                        'there's something saved
                                        If .PropertyValuation.Request IsNot Nothing AndAlso .PropertyValuation.Request.VendorParams <> "" Then
                                            'If UCase(.PropertyValuation.Request.VendorParams).Contains("HTTP:") = True OrElse UCase(.PropertyValuation.Request.VendorParams).Contains("WWW.") = True Then
                                            'updated 8/27/2014 to include https:
                                            If UCase(.PropertyValuation.Request.VendorParams).Contains("HTTPS:") = True OrElse UCase(.PropertyValuation.Request.VendorParams).Contains("HTTP:") = True OrElse UCase(.PropertyValuation.Request.VendorParams).Contains("WWW.") = True Then
                                                vendorValueUrl = .PropertyValuation.Request.VendorParams
                                            Else
                                                'may do something here... like separate URL method from VendorParams
                                            End If
                                        End If
                                        If saveToQuote = True Then
                                            Dim saveErrorMsg As String = ""
                                            'qqXml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qq.Database_QuoteId, saveErrorMsg)
                                            If qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                                Dim success As Boolean = False
                                                Dim qqEndorsementResults = Nothing 'only used for debug
                                                success = qqXml.SuccessfullySavedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn_WithqqoMethodToCall(qq, qqeResults:=qqEndorsementResults, errorMessage:=saveErrorMsg)
                                            Else
                                                qqXml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qq.Database_QuoteId, saveErrorMsg)
                                            End If
                                            If saveErrorMsg = "" Then
                                                saveWasSuccessful = True
                                            Else 'added 8/21/2014
                                                errorMsg = qqHelper.appendText(errorMsg, "problem saving quote: " & saveErrorMsg, "; ")
                                            End If
                                        End If
                                    Else 'added 8/21/2014
                                        errorMsg = "problem inserting property valuation in database"
                                        If saveToQuote = True Then
                                            errorMsg &= "; quote save not attempted"
                                        End If
                                    End If
                                End If
                            End With
                        Else
                            errorMsg = "the location specified doesn't exist on the QuickQuote object"
                        End If
                    Else
                        errorMsg = "no locations found on QuickQuote object"
                    End If
                Else
                    errorMsg = "QuickQuote object must have a valid quoteId"
                End If
            Else
                errorMsg = "valid QuickQuote object required"
            End If

        End Sub
        'Public Sub PopulateE2ValuePropertyValuationFromQuote(ByVal qq As QuickQuoteObject, ByRef pv As QuickQuotePropertyValuation, Optional ByVal locationNum As Integer = 1, Optional ByVal resetValuation As Boolean = False)
        'updated 8/20/2014 for normalSave param
        'Public Sub PopulateE2ValuePropertyValuationFromQuote(ByVal qq As QuickQuoteObject, ByRef pv As QuickQuotePropertyValuation, Optional ByVal locationNum As Integer = 1, Optional ByVal normalSave As Boolean = False, Optional ByVal resetValuation As Boolean = False, Optional ByVal sendAllParamsForExistingValuation As Boolean = False)
        'updated 7/28/2015 for buildingNum
        'Public Sub PopulateE2ValuePropertyValuationFromQuote(ByVal qq As QuickQuoteObject, ByRef pv As QuickQuotePropertyValuation, Optional ByVal locationNum As Integer = 1, Optional ByVal buildingNum As Integer = 0, Optional ByVal normalSave As Boolean = False, Optional ByVal resetValuation As Boolean = False, Optional ByVal sendAllParamsForExistingValuation As Boolean = False)
        'updated 7/31/2015 for optional propertyType param
        Public Sub PopulateVendorValuePropertyValuationFromQuote(ByVal qq As QuickQuoteObject, ByRef pv As QuickQuotePropertyValuation, Optional ByVal locationNum As Integer = 1, Optional ByVal buildingNum As Integer = 0, Optional ByVal normalSave As Boolean = False, Optional ByVal resetValuation As Boolean = False, Optional ByVal propertyType As ValuationPropertyType = ValuationPropertyType.DefaultByInfo, Optional ByVal sendAllParamsForExistingValuation As Boolean = False, Optional ByVal valuationVendor As QuickQuotePropertyValuation.ValuationVendor = QuickQuotePropertyValuation.ValuationVendor.e2Value)

            If pv Is Nothing Then
                pv = New QuickQuotePropertyValuation
                'ElseIf resetValuation = True Then
                '    pv = New QuickQuotePropertyValuation
                'updated 8/11/2014 to use Else w/ IF inside; also pulling existing values for db_propertyValuationId, IFM_UniqueValuationId, and VendorValuationId
            Else
                Dim originalQuoteId As String = pv.db_quoteId 'added 8/11/2014
                Dim originalLocNum As String = pv.db_locationNum 'added 8/13/2014
                Dim originalAgencyCode As String = pv.db_agencyCode 'added 8/15/2014
                Dim originalPropertyValuationId As String = pv.db_propertyValuationId 'added 8/11/2014
                Dim originalUniqueValuationId As String = pv.IFM_UniqueValuationId
                Dim originalVendorValuationId As String = pv.VendorValuationId
                Dim originalBuildNum As String = pv.db_buildingNum 'added 8/3/2015
                Dim originalPolicyId As String = pv.db_policyId
                Dim originalPolicyImageNum As String = pv.db_policyImageNum
                If resetValuation = True Then
                    pv = New QuickQuotePropertyValuation
                    pv.db_quoteId = originalQuoteId
                    pv.db_locationNum = originalLocNum 'added 8/13/2014
                    pv.db_agencyCode = originalAgencyCode 'added 8/15/2014
                    pv.db_propertyValuationId = originalPropertyValuationId
                    pv.IFM_UniqueValuationId = originalUniqueValuationId
                    pv.VendorValuationId = originalVendorValuationId
                    pv.db_buildingNum = originalBuildNum 'added 8/3/2015
                    pv.db_policyId = originalPolicyId
                    pv.db_policyImageNum = originalPolicyImageNum
                End If
            End If

            If locationNum < 1 Then
                locationNum = 1
            End If

            pv.Vendor = valuationVendor
            pv.VendorIntegrationType = QuickQuotePropertyValuation.ValuationVendorIntegrationType.AdvancedPortico
            'pv.VendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential 'added 8/11/2014
            'updated 4/29/2015 to handle multiple LOBs using different estimators
            'pv.VendorEstimatorType = GetEstimatorTypeForQuote(qq)
            'updated 7/28/2015 for buildingNum and GetEstimatorType
            'Dim propertyType As ValuationPropertyType = ValuationPropertyType.Location
            'If buildingNum > 0 Then
            '    propertyType = ValuationPropertyType.Building
            'End If
            'updated 7/31/2015 to use new method to get default
            If propertyType = ValuationPropertyType.None OrElse propertyType = ValuationPropertyType.DefaultByInfo Then
                'propertyType = ValuationPropertyTypeDefaultByInfo(locationNum, buildingNum)
                'updated 8/28/2015 to send qq
                propertyType = ValuationPropertyTypeDefaultByInfo(locationNum, buildingNum, qq)
            End If
            pv.VendorEstimatorType = GetEstimatorTypeForQuote(qq, propertyType)
            pv.db_environment = helper.Environment 'added 8/13/2014
            If pv.db_propertyValuationId = "" Then 'added 8/13/2014
                'pv.db_propertyValuationId = PropertyValuationIdForQuote(qq, locationNum)
                'updated 7/28/2015 for buildingNum
                pv.db_propertyValuationId = PropertyValuationIdForQuote(qq, locationNum, buildingNum)
            End If
            If pv.IFM_UniqueValuationId = "" Then 'added IF 8/11/2014
                'pv.IFM_UniqueValuationId = UniqueValuationIdForQuote(qq, locationNum) 'method will verify qq so no need to check here 1st
                'updated 8/11/2014 for propertyValuationId
                'pv.IFM_UniqueValuationId = UniqueValuationIdForQuote(qq, locationNum, pv.db_propertyValuationId)
                'updated 7/28/2015 for buildingNum
                'pv.IFM_UniqueValuationId = UniqueValuationIdForQuote(qq, locationNum, buildingNum, pv.db_propertyValuationId)

                Select Case valuationVendor
                    Case QuickQuotePropertyValuation.ValuationVendor.e2Value
                        pv.IFM_UniqueValuationId = UniqueValuationIdForQuote(qq, locationNum, buildingNum, pv.db_propertyValuationId)
                    Case QuickQuotePropertyValuation.ValuationVendor.Verisk360
                        pv.IFM_UniqueValuationId = UniqueValuationIdForQuote_Verisk360(qq, locationNum, buildingNum, pv.db_propertyValuationId)
                End Select

            End If
            If pv.VendorValuationId = "" Then 'added IF 8/11/2014
                'pv.VendorValuationId = VendorValuationIdForQuote(qq, locationNum) 'added 8/11/2014; method will verify qq so no need to check here 1st
                'updated 7/28/2015 for buildingNum
                pv.VendorValuationId = VendorValuationIdForQuote(qq, locationNum, buildingNum)
            End If

            If pv.db_locationNum = "" OrElse IsNumeric(pv.db_locationNum) = False Then 'added IF 8/13/2014
                pv.db_locationNum = locationNum.ToString
            End If
            If qqHelper.IsNumericString(pv.db_buildingNum) = False Then 'added 7/28/2015
                pv.db_buildingNum = buildingNum.ToString
            End If
            If pv.db_agencyCode = "" Then 'added 8/15/2014
                If qq IsNot Nothing AndAlso qq.AgencyCode <> "" Then
                    pv.db_agencyCode = qq.AgencyCode
                Else
                    pv.db_agencyCode = helper.LoggedInAgencyCode
                End If
                If pv.db_agencyCode <> "" AndAlso pv.db_agencyCode.Length > 4 Then
                    pv.db_agencyCode = Right(pv.db_agencyCode, 4)
                End If
            End If
            If pv.db_quoteId = "" OrElse IsNumeric(pv.db_quoteId) = False Then 'added IF 8/11/2014
                If qq IsNot Nothing Then
                    pv.db_quoteId = qq.Database_QuoteId
                End If
            End If

            If pv.db_policyId = "" OrElse IsNumeric(pv.db_policyId) = False Then
                If qq IsNot Nothing Then
                    pv.db_policyId = qq.PolicyId
                End If
            End If
            If pv.db_policyImageNum = "" OrElse IsNumeric(pv.db_policyImageNum) = False Then
                If qq IsNot Nothing Then
                    pv.db_policyImageNum = qq.PolicyImageNum
                End If
            End If

            'If qq IsNot Nothing Then
            '    If qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locationNum Then
            '        If qq.Locations(locationNum - 1) IsNot Nothing Then
            '            With qq.Locations(locationNum - 1)

            '            End With
            '        End If
            '    End If

            'End If

            'added 8/13/2014
            If pv.db_propertyValuationId = "" OrElse IsNumeric(pv.db_propertyValuationId) = False Then
                Dim errMsg As String = ""
                InsertPropertyValuation(pv, errMsg)
                'insert should set db_propertyValuationId and update IFM_UniqueValuationId if it is supposed to include propertyValuationId
                If errMsg = "" Then
                    'insert successful
                    If pv.Request Is Nothing Then
                        pv.Request = New QuickQuotePropertyValuationRequest
                    End If
                    With pv.Request
                        .db_propertyValuationId = pv.db_propertyValuationId
                        .IFM_UniqueValuationId = pv.IFM_UniqueValuationId
                        .VendorValuationId = pv.VendorValuationId 'won't ever have a value at this point
                    End With
                End If
            End If

            If pv.db_propertyValuationId <> "" AndAlso IsNumeric(pv.db_propertyValuationId) = True Then 'added IF 8/21/2014; was previously happening every time
                'PopulateE2ValueRequestFromQuote(qq, pv.Request, locationNum, pv.IFM_UniqueValuationId)
                '8/11/2014 - updated for propertyValuationId and VendorValuationId... and also resetValuation; 8/13/2014 note: may set resetValuationRequest to False
                'PopulateE2ValueRequestFromQuote(qq, pv.Request, locationNum, pv.db_propertyValuationId, pv.IFM_UniqueValuationId, pv.VendorValuationId, resetValuation)
                'updated 8/19/2014 for vendor and vendorEstimatorType
                'PopulateE2ValueRequestFromQuote(qq, pv.Request, locationNum, pv.db_propertyValuationId, pv.IFM_UniqueValuationId, pv.VendorValuationId, pv.Vendor, pv.VendorEstimatorType, resetValuation)
                'updated 8/20/2014 for normalSave param
                'PopulateE2ValueRequestFromQuote(qq, pv.Request, locationNum, normalSave, pv.db_propertyValuationId, pv.IFM_UniqueValuationId, pv.VendorValuationId, pv.Vendor, pv.VendorEstimatorType, resetValuation, sendAllParamsForExistingValuation) 'updated 8/20/2014 pm for sendAllParamsForExistingValuation
                'updated 7/28/2015 for buildingNum
                PopulateVendorValueRequestFromQuote(qq, pv.Request, locationNum, buildingNum, normalSave, pv.db_propertyValuationId, pv.IFM_UniqueValuationId, pv.VendorValuationId, pv.Vendor, pv.VendorEstimatorType, resetValuation, sendAllParamsForExistingValuation) 'updated 8/20/2014 pm for sendAllParamsForExistingValuation

                'added 8/11/2014; 8/13/2014 note: would already be done in the db whenever the Request record is inserted
                If pv.Request IsNot Nothing AndAlso pv.Request.db_propertyValuationRequestId <> "" AndAlso IsNumeric(pv.Request.db_propertyValuationRequestId) = True Then
                    pv.db_propertyValuationRequestId = pv.Request.db_propertyValuationRequestId
                    pv.db_propertyValuationResponseId = "" 'added 8/13/2014 since it's reset whenever a new request is inserted
                End If
            End If

        End Sub
        'added 8/6/2014 for e2Value; 8/7/2014 - copied from PropertyValuationRequestForQuote in QuickQuoteHelperClass
        'Public Sub PopulateE2ValueRequestFromQuote(ByVal qq As QuickQuoteObject, ByRef pvr As QuickQuotePropertyValuationRequest, Optional ByVal locationNum As Integer = 1, Optional ByVal uniqueValuationId As String = "", Optional ByVal resetValuationRequest As Boolean = False)
        '8/11/2014 - updated for VendorValuationId and propertyValuationId
        'Public Sub PopulateE2ValueRequestFromQuote(ByVal qq As QuickQuoteObject, ByRef pvr As QuickQuotePropertyValuationRequest, Optional ByVal locationNum As Integer = 1, Optional ByVal propertyValuationId As String = "", Optional ByVal uniqueValuationId As String = "", Optional ByVal vendorValuationId As String = "", Optional ByVal resetValuationRequest As Boolean = False)
        'updated 8/19/2014 for vendor and vendorEstimatorType
        'Public Sub PopulateE2ValueRequestFromQuote(ByVal qq As QuickQuoteObject, ByRef pvr As QuickQuotePropertyValuationRequest, Optional ByVal locationNum As Integer = 1, Optional ByVal propertyValuationId As String = "", Optional ByVal uniqueValuationId As String = "", Optional ByVal vendorValuationId As String = "", Optional ByVal vendor As QuickQuotePropertyValuation.ValuationVendor = QuickQuotePropertyValuation.ValuationVendor.None, Optional ByVal vendorEstimatorType As QuickQuotePropertyValuation.ValuationVendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.None, Optional ByVal resetValuationRequest As Boolean = False)
        'updated 8/20/2014 for normalSave param
        'Public Sub PopulateE2ValueRequestFromQuote(ByVal qq As QuickQuoteObject, ByRef pvr As QuickQuotePropertyValuationRequest, Optional ByVal locationNum As Integer = 1, Optional ByVal normalSave As Boolean = False, Optional ByVal propertyValuationId As String = "", Optional ByVal uniqueValuationId As String = "", Optional ByVal vendorValuationId As String = "", Optional ByVal vendor As QuickQuotePropertyValuation.ValuationVendor = QuickQuotePropertyValuation.ValuationVendor.None, Optional ByVal vendorEstimatorType As QuickQuotePropertyValuation.ValuationVendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.None, Optional ByVal resetValuationRequest As Boolean = False, Optional ByVal sendAllParamsForExistingValuation As Boolean = False)
        'updated 7/28/2015 for buildingNum
        Public Sub PopulateVendorValueRequestFromQuote(ByVal qq As QuickQuoteObject, ByRef pvr As QuickQuotePropertyValuationRequest, Optional ByVal locationNum As Integer = 1, Optional ByVal buildingNum As Integer = 0, Optional ByVal normalSave As Boolean = False, Optional ByVal propertyValuationId As String = "", Optional ByVal uniqueValuationId As String = "", Optional ByVal vendorValuationId As String = "", Optional ByVal vendor As QuickQuotePropertyValuation.ValuationVendor = QuickQuotePropertyValuation.ValuationVendor.None, Optional ByVal vendorEstimatorType As QuickQuotePropertyValuation.ValuationVendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.None, Optional ByVal resetValuationRequest As Boolean = False, Optional ByVal sendAllParamsForExistingValuation As Boolean = False)

            If pvr Is Nothing Then
                pvr = New QuickQuotePropertyValuationRequest
                'ElseIf resetValuationRequest = True Then
                '    pvr = New QuickQuotePropertyValuationRequest
                'updated 8/11/2014 to use Else w/ IF inside
            Else
                If (vendor = QuickQuotePropertyValuation.ValuationVendor.Verisk360) Then
                    pvr.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId.Replace("quoteId", "qId")
                    pvr.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId.Replace("locNum", "lNum")
                    pvr.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId.Replace("buildNum", "bNum")
                    pvr.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId.Replace("||propertyValuationId", "||pVId")
                    pvr.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId.Replace("policyId", "pId")
                    pvr.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId.Replace("policyImageNum", "pImN")
                End If
                Dim originalPropertyValuationId As String = pvr.db_propertyValuationId 'added 8/11/2014
                Dim originalUniqueValuationId As String = pvr.IFM_UniqueValuationId
                Dim originalVendorValuationId As String = pvr.VendorValuationId
                If resetValuationRequest = True Then
                    pvr = New QuickQuotePropertyValuationRequest
                    pvr.db_propertyValuationId = originalPropertyValuationId
                    pvr.IFM_UniqueValuationId = originalUniqueValuationId
                    pvr.VendorValuationId = originalVendorValuationId
                End If
            End If

            pvr.locNum = locationNum
            pvr.buildNum = buildingNum
            If locationNum < 1 Then
                locationNum = 1
            End If

            'If uniqueValuationId = "" Then
            '    'uniqueValuationId = UniqueValuationIdForQuote(qq, locationNum)
            '    'updated 8/11/2014 for propertyValuationId
            '    uniqueValuationId = UniqueValuationIdForQuote(qq, locationNum, pvr.db_propertyValuationId)
            'End If
            'If vendorValuationId = "" Then 'added 8/11/2014
            '    vendorValuationId = VendorValuationIdForQuote(qq, locationNum)
            'End If
            'pvr.IFM_UniqueValuationId = uniqueValuationId
            'pvr.VendorValuationId = vendorValuationId 'added 8/11/2014

            'updated 8/13/2014
            If pvr.db_propertyValuationId = "" Then
                If propertyValuationId = "" Then
                    'propertyValuationId = PropertyValuationIdForQuote(qq, locationNum)
                    'updated 7/28/2015 for buildingNum
                    propertyValuationId = PropertyValuationIdForQuote(qq, locationNum, buildingNum)
                End If
                pvr.db_propertyValuationId = propertyValuationId
            End If
            If pvr.IFM_UniqueValuationId = "" Then
                If uniqueValuationId = "" Then
                    'uniqueValuationId = UniqueValuationIdForQuote(qq, locationNum, pvr.db_propertyValuationId)
                    'updated 7/28/2015 for buildingNum
                    'uniqueValuationId = UniqueValuationIdForQuote(qq, locationNum, buildingNum, pvr.db_propertyValuationId)

                    Select Case vendor
                        Case QuickQuotePropertyValuation.ValuationVendor.e2Value
                            pvr.IFM_UniqueValuationId = UniqueValuationIdForQuote(qq, locationNum, buildingNum, pvr.db_propertyValuationId)
                        Case QuickQuotePropertyValuation.ValuationVendor.Verisk360
                            pvr.IFM_UniqueValuationId = UniqueValuationIdForQuote_Verisk360(qq, locationNum, buildingNum, pvr.db_propertyValuationId)
                    End Select


                End If
                pvr.IFM_UniqueValuationId = uniqueValuationId
            End If
            If pvr.VendorValuationId = "" Then
                If vendorValuationId = "" Then
                    'vendorValuationId = VendorValuationIdForQuote(qq, locationNum)
                    'updated 7/28/2015 for buildingNum
                    vendorValuationId = VendorValuationIdForQuote(qq, locationNum, buildingNum)
                End If
                pvr.VendorValuationId = vendorValuationId
            End If

            'added 8/13/2014; updated 8/20/2014 for normalSave param... original logic is in ELSE
            If normalSave = True Then
                pvr.AuthId = ""
                pvr.AuthCode = ""
                pvr.db_sentToVendor = False 'added 8/22/2014; might not need... in case where previously True and changed to False
            Else
                If vendor = QuickQuotePropertyValuation.ValuationVendor.e2Value Then
                    SetE2ValueAuthIdAndCodeForQuote(qq, pvr.AuthId, pvr.AuthCode)
                ElseIf vendor = QuickQuotePropertyValuation.ValuationVendor.Verisk360 Then
                    'SetE2ValueAuthIdAndCodeForQuote(qq, pvr.AuthId, pvr.AuthCode)
                End If
                pvr.db_sentToVendor = True 'added 8/22/2014
            End If

            'added 8/21/2014
            If pvr.ReturnUrl = "" Then
                'pvr.ReturnUrl = helper.configAppSettingValueAsString("e2Value_ReturnUrl")
                'updated 5/27/2015 to use new function
                pvr.ReturnUrl = VendorValuationReturnUrl(vendor)
            End If
            If pvr.ReturnUrlLinkText = "" Then
                'pvr.ReturnUrlLinkText = "Return to VelociRater quote"
                'If qq IsNot Nothing AndAlso qq.QuoteNumber <> "" Then
                '    pvr.ReturnUrlLinkText &= " (" & qq.QuoteNumber & ")"
                'End If

                If qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    pvr.ReturnUrlLinkText = "Return to VelociRater policy"
                    If qq IsNot Nothing AndAlso qq.PolicyNumber <> "" Then
                        pvr.ReturnUrlLinkText &= " (" & qq.PolicyNumber & ")"
                    End If
                Else
                    pvr.ReturnUrlLinkText = "Return to VelociRater quote"
                    If qq IsNot Nothing AndAlso qq.QuoteNumber <> "" Then
                        pvr.ReturnUrlLinkText &= " (" & qq.QuoteNumber & ")"
                    End If
                End If
            End If

            If qq IsNot Nothing Then
                Dim hasLocAddress As Boolean = False
                Dim hasPhAddress As Boolean = False
                Dim hasClAddress As Boolean = False
                Dim hasPhName As Boolean = False
                Dim hasClName As Boolean = False
                Dim hasLocName As Boolean = False
                If qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locationNum Then
                    If qq.Locations(locationNum - 1) IsNot Nothing Then
                        With qq.Locations(locationNum - 1)
                            If .Name IsNot Nothing AndAlso .Name.HasData = True Then
                                hasLocName = True
                            End If
                            If .Address IsNot Nothing AndAlso .Address.HasData = True Then
                                hasLocAddress = True
                                ConvertQuickQuoteAddressToPropertyValuationStreetAddress(.Address, pvr.ClientAddress1, pvr.ClientAddress2, pvr.ClientCity, pvr.ClientState, pvr.ClientZip)
                                pvr.Unit = .Address.ApartmentNumber
                            End If

                            'updated 8/19/2014
                            Dim relatedPropertyAttributes As New List(Of QuickQuoteStaticDataAttribute)
                            If vendor <> QuickQuotePropertyValuation.ValuationVendor.None OrElse vendorEstimatorType <> QuickQuotePropertyValuation.ValuationVendorEstimatorType.None Then
                                If vendor <> QuickQuotePropertyValuation.ValuationVendor.None Then
                                    Dim a1 As New QuickQuoteStaticDataAttribute
                                    a1.nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.Vendor
                                    a1.nvp_value = System.Enum.GetName(GetType(QuickQuotePropertyValuation.ValuationVendor), vendor)
                                    relatedPropertyAttributes.Add(a1)
                                End If
                                If vendorEstimatorType <> QuickQuotePropertyValuation.ValuationVendorEstimatorType.None Then
                                    Dim a2 As New QuickQuoteStaticDataAttribute
                                    a2.nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.VendorEstimatorType
                                    a2.nvp_value = System.Enum.GetName(GetType(QuickQuotePropertyValuation.ValuationVendorEstimatorType), vendorEstimatorType)
                                    relatedPropertyAttributes.Add(a2)
                                End If
                            End If
                            If buildingNum > 0 AndAlso .Buildings IsNot Nothing AndAlso .Buildings.Count >= buildingNum Then 'added IF 8/3/2015; original logic in ELSE (to use Location)
                                With .Buildings(buildingNum - 1)
                                    If qqHelper.IsNumericString(.YearBuilt) = True AndAlso Len(.YearBuilt.Trim) <= 4 Then
                                        pvr.YearBuilt = .YearBuilt.Trim
                                    Else
                                        pvr.YearBuilt = "0"
                                    End If
                                    pvr.ConstructionType = Left(qqHelper.GetRelatedStaticDataValueWithMatchingAttributesForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionId, .ConstructionId, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionType, relatedPropertyAttributes), 100)
                                    If qqHelper.IsNumericString(.SquareFeet) = True AndAlso Len(.SquareFeet.Trim) <= 10 Then
                                        pvr.SquareFeet = .SquareFeet.Trim
                                    Else
                                        pvr.SquareFeet = "0"
                                    End If
                                End With
                            Else
                                If .YearBuilt <> "" AndAlso IsNumeric(.YearBuilt) = True AndAlso Len(.YearBuilt.Trim) <= 4 Then
                                    pvr.YearBuilt = .YearBuilt.Trim
                                Else
                                    pvr.YearBuilt = "0"
                                End If
                                pvr.ConstructionType = Left(qqHelper.GetRelatedStaticDataValueWithMatchingAttributesForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId, .ConstructionTypeId, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionType, relatedPropertyAttributes), 100)
                                If .SquareFeet <> "" AndAlso IsNumeric(.SquareFeet) = True AndAlso Len(.SquareFeet.Trim) <= 10 Then
                                    pvr.SquareFeet = .SquareFeet.Trim
                                Else
                                    pvr.SquareFeet = "0"
                                End If
                            End If
                            '8/3/2015 note: can move the variables below into the IF or ELSE above if any of these properties are added to the Location or Building object
                            pvr.RoofType = "" 'not needed since it should still have the default value of ""
                            pvr.ArchitecturalStyle = Left(qqHelper.GetRelatedStaticDataValueWithMatchingAttributesForOptionValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteLocation, QuickQuoteHelperClass.QuickQuotePropertyName.ArchitecturalStyle, .ArchitecturalStyle, QuickQuoteHelperClass.QuickQuotePropertyName.ArchitecturalStyle, relatedPropertyAttributes), 100)
                            pvr.PhysicalShape = "" 'not needed since it should still have the default value of ""
                            pvr.PrimaryExterior = "" 'not needed since it should still have the default value of ""
                        End With
                    End If
                End If
                If qq.Policyholder IsNot Nothing AndAlso qq.Policyholder.HasData = True Then
                    With qq.Policyholder
                        If .Name IsNot Nothing AndAlso .Name.HasData = True Then
                            hasPhName = True
                            ConvertQuickQuoteNameToPropertyValuationName(.Name, pvr.ClientIsBusiness, pvr.ClientFirstName, pvr.ClientMiddleInitial, pvr.ClientLastName)
                        End If
                        If .Address IsNot Nothing AndAlso .Address.HasData = True Then
                            hasPhAddress = True
                            If hasLocAddress = False Then
                                ConvertQuickQuoteAddressToPropertyValuationStreetAddress(.Address, pvr.ClientAddress1, pvr.ClientAddress2, pvr.ClientCity, pvr.ClientState, pvr.ClientZip)
                            End If
                        End If
                    End With
                End If
                If qq.Client IsNot Nothing AndAlso qq.Client.HasPrimaryData = True Then
                    With qq.Client
                        If .Name IsNot Nothing AndAlso .Name.HasData = True Then
                            hasClName = True
                            If hasPhName = False Then
                                ConvertQuickQuoteNameToPropertyValuationName(.Name, pvr.ClientIsBusiness, pvr.ClientFirstName, pvr.ClientMiddleInitial, pvr.ClientLastName)
                            End If
                        End If
                        If .Address IsNot Nothing AndAlso .Address.HasData = True Then
                            hasClAddress = True
                            If hasLocAddress = False AndAlso hasPhAddress = False Then
                                ConvertQuickQuoteAddressToPropertyValuationStreetAddress(.Address, pvr.ClientAddress1, pvr.ClientAddress2, pvr.ClientCity, pvr.ClientState, pvr.ClientZip)
                            End If
                        End If
                    End With
                End If
                If hasPhName = False AndAlso hasClName = False AndAlso hasLocName = True Then
                    With qq.Locations(locationNum - 1)
                        ConvertQuickQuoteNameToPropertyValuationName(.Name, pvr.ClientIsBusiness, pvr.ClientFirstName, pvr.ClientMiddleInitial, pvr.ClientLastName)
                    End With
                End If

            End If
            'pvr.VendorParams = QuerystringForE2ValueRequest(pvr) 'added 8/8/2014; may change to include URL
            'updated 8/19/2014 for URL... URL method gets querystring



            Select Case vendor
                Case QuickQuotePropertyValuation.ValuationVendor.e2Value
                    pvr.VendorParams = UrlForVendorValueRequest(qq, pvr, vendor, vendorEstimatorType, sendAllParamsForExistingValuation) 'updated 8/20/2014 pm for sendAllParamsForExistingValuation
                Case QuickQuotePropertyValuation.ValuationVendor.Verisk360
                    pvr.VendorParams = UrlForVendorValueRequest(qq, pvr, vendor, vendorEstimatorType, sendAllParamsForExistingValuation) 'updated 8/20/2014 pm for sendAllParamsForExistingValuation

            End Select


            'added 8/13/2014; should always happen here; updated functionality added 8/19/2014
            If pvr.db_propertyValuationRequestId = "" OrElse IsNumeric(pvr.db_propertyValuationRequestId) = False Then
                Dim errMsg As String = ""
                InsertPropertyValuationRequest(pvr, errMsg)
                If errMsg = "" Then
                    'insert successful
                End If
            Else 'added ELSE 8/19/2014
                Dim errMsg As String = ""
                UpdatePropertyValuationRequest(pvr, errMsg)
                If errMsg = "" Then
                    'update successful
                End If
            End If

        End Sub

        Public Function SubmitVerisk360RequestToSnapLogic(ByVal qq As QuickQuoteObject, ByRef pvr As QuickQuotePropertyValuationRequest)
            Dim apiKey As String = helper.configAppSettingValueAsString("VR_Verisk360_IntegrationCall_APIKey")
            Dim SnapLogicVerisk360valuationURL As String = helper.configAppSettingValueAsString("VR_SnapLogic_verisk360valuation_URL")
            Dim site360Value_Return_Url As String = helper.configAppSettingValueAsString("VR_Verisk360_ReturnUrl")

            Dim isNewBusiness As Boolean = qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.NewBusinessQuote
            Dim returnURL As String = ""
            Dim buildingType As String = ""

            If pvr IsNot Nothing Then
                Try
                    If pvr.buildNum > 0 Then
                        'updated 8/27/2015 - 8/28/2015; original logic in ELSE
                        Dim qqBuilding As QuickQuoteBuilding = qqHelper.QuickQuoteBuildingForActiveNum(qq, pvr.locNum, pvr.buildNum)
                        If qqBuilding IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(qqBuilding.FarmStructureTypeId) = True Then
                            buildingType = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.FarmStructureTypeId, qqBuilding.FarmStructureTypeId)
                        End If
                    End If

                    Using client As New Net.WebClient()
                        client.Headers.Add("x-api-key", apiKey) ' Replace with actual API key
                        client.Headers.Add(Net.HttpRequestHeader.ContentType, "application/json")

                        Dim verisk360InputRequest As JObject = New JObject()
                        With pvr
                            verisk360InputRequest("agencyCode") = If(qq.AgencyCode.Length > 4, Right(qq.AgencyCode, 4), "")
                            verisk360InputRequest("clientName") = .ClientFirstName & " " & .ClientMiddleInitial & " " & .ClientLastName
                            verisk360InputRequest("street") = .ClientAddress1 & " " & .ClientAddress2
                            verisk360InputRequest("unit") = ""  ' for future enhancement we need to map this field to value 360
                            verisk360InputRequest("city") = .ClientCity
                            verisk360InputRequest("state") = .ClientState
                            verisk360InputRequest("zipCode") = .ClientZip
                            verisk360InputRequest("country") = "USA"
                            verisk360InputRequest("yearBuilt") = .YearBuilt
                            verisk360InputRequest("squareFeet") = .SquareFeet
                            verisk360InputRequest("locNum") = pvr.locNum
                            verisk360InputRequest("buildNum") = pvr.buildNum
                            verisk360InputRequest("policyId") = If(isNewBusiness, "", qq.PolicyId)
                            verisk360InputRequest("policyImageNumber") = If(isNewBusiness, "", qq.PolicyImageNum)
                            verisk360InputRequest("policyNumber") = If(isNewBusiness, "", qq.PolicyNumber)
                            verisk360InputRequest("quoteId") = If(isNewBusiness, qq.Database_QuoteId, "")
                            verisk360InputRequest("quoteNumber") = If(isNewBusiness, qq.QuoteNumber, "")
                            verisk360InputRequest("propertyValuationId") = pvr.db_propertyValuationId
                            verisk360InputRequest("buildingType") = buildingType

                        End With

                        ' Convert the JObject back to a JSON string
                        Dim inputRequest As String = verisk360InputRequest.ToString()


                        Dim responseString As String = client.UploadString(SnapLogicVerisk360valuationURL, inputRequest)

                        ' Extract token from JSON response
                        Dim token As String = ExtractVendorValuationParams(responseString, pvr)
                        ' Use the token as needed
                        returnURL = "?sso_token=" + token + "&return_url=" + site360Value_Return_Url
                    End Using
                Catch ex As Net.WebException
                    ' Handle WebException
                    'saveWasSuccessful = False
                    'errorMessage = ex.Message
                Catch ex As Exception
                    ' Handle other exceptions
                    'saveWasSuccessful = False
                    'errorMessage = ex.Message
                End Try
            End If
            Return returnURL
        End Function

        Public Sub ConvertQuickQuoteAddressToPropertyValuationStreetAddress(ByVal qqAdd As QuickQuoteAddress, ByRef stLn1 As String, ByRef stLn2 As String, ByRef city As String, ByRef state As String, ByRef zip As String)
            stLn1 = ""
            stLn2 = ""
            city = ""
            state = ""
            zip = ""
            If qqAdd IsNot Nothing Then
                stLn1 = qqHelper.appendText(qqAdd.HouseNum, qqAdd.StreetName, " ")
                stLn1 = qqHelper.appendText(stLn1, qqAdd.ApartmentNumber, " ") 'may need to use text '# ' or something
                stLn1 = Left(stLn1, 150)
                'stLn2 = Left(stLn2, 150)'could possibly use if len(stLn1) > 150... find last space in address line and move anything after to 2nd line if that works... else keep moving back to the next space
                city = Left(qqAdd.City, 100)
                state = Left(qqAdd.State, 2)
                If state = "" Then
                    state = "In"
                End If
                zip = Left(qqAdd.Zip, 5)
            End If
        End Sub
        Public Sub ConvertQuickQuoteNameToPropertyValuationName(ByVal qqName As QuickQuoteName, ByRef isBusinessName As Boolean, ByRef firstName As String, ByRef middleInitial As String, ByRef lastName As String)
            isBusinessName = False
            firstName = ""
            middleInitial = ""
            lastName = ""
            If qqName IsNot Nothing Then
                If qqName.TypeId <> "" AndAlso IsNumeric(qqName.TypeId) = True AndAlso (CInt(qqName.TypeId) = 1 OrElse CInt(qqName.TypeId) = 2) Then
                    If CInt(qqName.TypeId) = 2 Then
                        'Bus
                        isBusinessName = True
                    Else
                        'Pers
                        isBusinessName = False
                    End If
                Else
                    If qqName.FirstName <> "" AndAlso qqName.LastName <> "" Then
                        'Pers
                        isBusinessName = False
                    ElseIf qqName.CommercialDBAname <> "" OrElse qqName.CommercialIRSname <> "" OrElse qqName.DoingBusinessAsName <> "" Then
                        'Comm
                        isBusinessName = True
                    Else
                        'default to Pers
                        isBusinessName = False
                    End If
                End If
                If isBusinessName = True Then
                    'Bus
                    lastName = qqName.CommercialDBAname
                    If lastName = "" Then
                        lastName = qqName.DoingBusinessAsName
                        If lastName = "" Then
                            lastName = qqName.CommercialIRSname
                        End If
                    End If
                    lastName = Left(lastName, 100)
                Else
                    'Pers
                    firstName = Left(qqName.FirstName, 100)
                    middleInitial = Left(qqName.MiddleName, 1)
                    lastName = Left(qqName.LastName, 100)
                End If
            End If
        End Sub
        'added 8/7/2014
        'Public Function UniqueValuationIdForQuote(ByVal qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1) As String
        'updated 8/11/2014 for propertyValuationId
        'Public Function UniqueValuationIdForQuote(ByVal qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1, Optional ByVal propertyValuationId As String = "") As String
        'updated 7/28/2015 for buildingNum
        Public Function UniqueValuationIdForQuote(ByVal qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1, Optional ByVal buildingNum As Integer = 0, Optional ByVal propertyValuationId As String = "") As String
            Dim uniqueValuationId As String = ""

            If locationNum < 1 Then
                locationNum = 1
            End If

            If qq IsNot Nothing Then 'AndAlso qq.Database_QuoteId <> "" AndAlso IsNumeric(qq.Database_QuoteId) = True Then

                If qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    uniqueValuationId = "policyId==" & qq.PolicyId
                    uniqueValuationId &= "||policyImageNum==" & qq.PolicyImageNum
                Else
                    uniqueValuationId = "quoteId==" & qq.Database_QuoteId
                End If

                uniqueValuationId &= "||locNum==" & locationNum.ToString
                If buildingNum > 0 Then 'added 7/28/2015
                    uniqueValuationId &= "||buildNum==" & buildingNum.ToString
                End If
                'If qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locationNum Then
                '    With qq.Locations(locationNum - 1)
                '        If .UniqueIdentifier <> "" Then
                '            uniqueValuationId &= "||locIdentifier==" & .UniqueIdentifier
                '        End If
                '        'might also include propertyValuationIdentifier
                '    End With
                'End If
                'updated 8/11/2014 to use propertyValuationId instead of locIdentifier
                If propertyValuationId <> "" AndAlso IsNumeric(propertyValuationId) = True Then
                    uniqueValuationId &= "||propertyValuationId==" & propertyValuationId
                Else
                    '8/13/2014 note: may not even use this logic here... calling method should already have this info
                    uniqueValuationId &= "||propertyValuationId==" & "*propertyValuationId*"
                    If qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locationNum Then
                        With qq.Locations(locationNum - 1)
                            If buildingNum > 0 Then 'added IF 7/28/2015; original logic in ELSE
                                If .Buildings IsNot Nothing AndAlso .Buildings.Count >= buildingNum Then
                                    With .Buildings(buildingNum - 1)
                                        If .PropertyValuation IsNot Nothing Then
                                            If .PropertyValuation.IFM_UniqueValuationId <> "" Then
                                                'use existing
                                                uniqueValuationId = .PropertyValuation.IFM_UniqueValuationId
                                            ElseIf .PropertyValuation.db_propertyValuationId <> "" AndAlso IsNumeric(.PropertyValuation.db_propertyValuationId) = True Then
                                                'replace propertyValuationId placeholder w/ actual id
                                                uniqueValuationId = uniqueValuationId.Replace("*propertyValuationId*", .PropertyValuation.db_propertyValuationId)
                                            End If
                                        End If
                                    End With
                                End If
                            Else
                                If .PropertyValuation IsNot Nothing Then
                                    If .PropertyValuation.IFM_UniqueValuationId <> "" Then
                                        'use existing
                                        uniqueValuationId = .PropertyValuation.IFM_UniqueValuationId
                                    ElseIf .PropertyValuation.db_propertyValuationId <> "" AndAlso IsNumeric(.PropertyValuation.db_propertyValuationId) = True Then
                                        'replace propertyValuationId placeholder w/ actual id
                                        uniqueValuationId = uniqueValuationId.Replace("*propertyValuationId*", .PropertyValuation.db_propertyValuationId)
                                    End If
                                End If
                            End If
                        End With
                    End If
                End If
            End If

            Return uniqueValuationId
        End Function
        'added 8/11/2014... just like UniqueValuationIdForQuote; 8/13/2014 note: may not even use this logic here... calling method should already have this info
        'Public Function VendorValuationIdForQuote(ByVal qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1) As String
        'updated 7/28/2015 for buildingNum

        Public Function UniqueValuationIdForQuote_Verisk360(ByVal qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1, Optional ByVal buildingNum As Integer = 0, Optional ByVal propertyValuationId As String = "") As String
            Dim uniqueValuationId As String = ""

            If locationNum < 1 Then
                locationNum = 1
            End If

            If qq IsNot Nothing Then 'AndAlso qq.Database_QuoteId <> "" AndAlso IsNumeric(qq.Database_QuoteId) = True Then

                If qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                    uniqueValuationId = "pId==" & qq.PolicyId
                    uniqueValuationId &= "||pImN==" & qq.PolicyImageNum
                Else
                    uniqueValuationId = "qId==" & qq.Database_QuoteId
                End If

                uniqueValuationId &= "||lNum==" & locationNum.ToString
                If buildingNum > 0 Then 'added 7/28/2015
                    uniqueValuationId &= "||bNum==" & buildingNum.ToString
                End If
                'If qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locationNum Then
                '    With qq.Locations(locationNum - 1)
                '        If .UniqueIdentifier <> "" Then
                '            uniqueValuationId &= "||locIdentifier==" & .UniqueIdentifier
                '        End If
                '        'might also include propertyValuationIdentifier
                '    End With
                'End If
                'updated 8/11/2014 to use propertyValuationId instead of locIdentifier
                If propertyValuationId <> "" AndAlso IsNumeric(propertyValuationId) = True Then
                    uniqueValuationId &= "||pVId==" & propertyValuationId
                Else
                    '8/13/2014 note: may not even use this logic here... calling method should already have this info
                    uniqueValuationId &= "||pVId==" & "*propertyValuationId*"
                    If qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locationNum Then
                        With qq.Locations(locationNum - 1)
                            If buildingNum > 0 Then 'added IF 7/28/2015; original logic in ELSE
                                If .Buildings IsNot Nothing AndAlso .Buildings.Count >= buildingNum Then
                                    With .Buildings(buildingNum - 1)
                                        If .PropertyValuation IsNot Nothing Then
                                            If .PropertyValuation.IFM_UniqueValuationId <> "" Then
                                                'use existing
                                                uniqueValuationId = .PropertyValuation.IFM_UniqueValuationId
                                            ElseIf .PropertyValuation.db_propertyValuationId <> "" AndAlso IsNumeric(.PropertyValuation.db_propertyValuationId) = True Then
                                                'replace propertyValuationId placeholder w/ actual id
                                                uniqueValuationId = uniqueValuationId.Replace("*propertyValuationId*", .PropertyValuation.db_propertyValuationId)
                                            End If
                                        End If
                                    End With
                                End If
                            Else
                                If .PropertyValuation IsNot Nothing Then
                                    If .PropertyValuation.IFM_UniqueValuationId <> "" Then
                                        'use existing
                                        uniqueValuationId = .PropertyValuation.IFM_UniqueValuationId
                                    ElseIf .PropertyValuation.db_propertyValuationId <> "" AndAlso IsNumeric(.PropertyValuation.db_propertyValuationId) = True Then
                                        'replace propertyValuationId placeholder w/ actual id
                                        uniqueValuationId = uniqueValuationId.Replace("*propertyValuationId*", .PropertyValuation.db_propertyValuationId)
                                    End If
                                End If
                            End If
                        End With
                    End If
                End If
            End If

            Return uniqueValuationId
        End Function

        Public Function VendorValuationIdForQuote(ByVal qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1, Optional ByVal buildingNum As Integer = 0) As String
            Dim vendorValuationId As String = ""

            If locationNum < 1 Then
                locationNum = 1
            End If

            If qq IsNot Nothing AndAlso qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locationNum Then
                With qq.Locations(locationNum - 1)
                    If buildingNum > 0 Then 'added IF 7/28/2015; original logic in ELSE
                        If .Buildings IsNot Nothing AndAlso .Buildings.Count >= buildingNum Then
                            With .Buildings(buildingNum - 1)
                                If .PropertyValuation IsNot Nothing AndAlso .PropertyValuation.VendorValuationId <> "" Then
                                    'use existing
                                    vendorValuationId = .PropertyValuation.VendorValuationId
                                End If
                            End With
                        End If
                    Else
                        If .PropertyValuation IsNot Nothing AndAlso .PropertyValuation.VendorValuationId <> "" Then
                            'use existing
                            vendorValuationId = .PropertyValuation.VendorValuationId
                        End If
                    End If
                End With
            End If

            Return vendorValuationId
        End Function
        'added 8/13/2014... just like UniqueValuationIdForQuote and VendorValuationIdForQuote; 8/13/2014 note: may not even use this logic here... calling method should already have this info
        'Public Function PropertyValuationIdForQuote(ByVal qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1) As String
        'updated 7/28/2015 for buildingNum
        Public Function PropertyValuationIdForQuote(ByVal qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1, Optional ByVal buildingNum As Integer = 0) As String
            Dim propertyValuationId As String = ""

            If locationNum < 1 Then
                locationNum = 1
            End If

            If qq IsNot Nothing AndAlso qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locationNum Then
                With qq.Locations(locationNum - 1)
                    If buildingNum > 0 Then 'added IF 7/28/2015; original logic in ELSE
                        If .Buildings IsNot Nothing AndAlso .Buildings.Count >= buildingNum Then
                            With .Buildings(buildingNum - 1)
                                If .PropertyValuation IsNot Nothing AndAlso .PropertyValuation.db_propertyValuationId <> "" AndAlso IsNumeric(.PropertyValuation.db_propertyValuationId) = True Then
                                    'use existing
                                    propertyValuationId = .PropertyValuation.db_propertyValuationId
                                End If
                            End With
                        End If
                    Else
                        If .PropertyValuation IsNot Nothing AndAlso .PropertyValuation.db_propertyValuationId <> "" AndAlso IsNumeric(.PropertyValuation.db_propertyValuationId) = True Then
                            'use existing
                            propertyValuationId = .PropertyValuation.db_propertyValuationId
                        End If
                    End If
                End With
            End If

            Return propertyValuationId
        End Function
        'Private Sub SplitUniqueValuationIdString(ByVal uniqueValuationIdString As String, ByRef quoteId As String, ByRef locNum As String, ByRef locIdentifier As String)
        'updated 8/11/2014 to use propertyValuationId instead of locIdentifier
        'Private Sub SplitUniqueValuationIdString(ByVal uniqueValuationIdString As String, ByRef quoteId As String, ByRef locNum As String, ByRef propertyValuationId As String)
        'updated 7/28/2015 for buildingNum
        Private Sub SplitUniqueValuationIdString(ByVal uniqueValuationIdString As String, ByRef quoteId As String, ByRef locNum As String, ByRef buildNum As String, ByRef propertyValuationId As String, Optional ByRef policyId As String = "", Optional ByRef policyImageNum As String = "")
            'quoteId==12345||locNum==1||locIdentifier==1f913d07-865d-436b-8b8d-73c22c62f2f5
            'might also include propertyValuationIdentifier
            'updated 8/11/2014 to use propertyValuationId instead of locIdentifier
            'quoteId==12345||locNum==1||propertyValuationId==123
            quoteId = ""
            locNum = ""
            'locIdentifier = ""
            buildNum = "" 'added 7/28/2015
            policyId = ""
            policyImageNum = ""
            propertyValuationId = ""
            If uniqueValuationIdString <> "" AndAlso uniqueValuationIdString.Contains("==") = True Then
                Dim arNameValuePair As Array
                If uniqueValuationIdString.Contains("||") = True Then
                    'multiple values
                    Dim arUvIdString As String()
                    arUvIdString = Split(uniqueValuationIdString, "||")
                    For Each nameValuePair As String In arUvIdString
                        If nameValuePair.Contains("==") = True Then
                            arNameValuePair = Split(nameValuePair, "==")
                            Select Case UCase(arNameValuePair(0).ToString.Trim)
                                Case "QUOTEID", "QID"
                                    quoteId = arNameValuePair(1).ToString.Trim
                                Case "LOCNUM", "LOCATIONNUM", "LOCATIONNUMBER", "LNUM"
                                    locNum = arNameValuePair(1).ToString.Trim
                                Case "BUILDNUM", "BUILDINGNUM", "BUILDINGNUMBER", "BNUM" 'added 7/28/2015
                                    buildNum = arNameValuePair(1).ToString.Trim
                                    'Case "LOCIDENTIFIER", "LOCATIONIDENTIFIER", "LOCGUID", "LOCATIONGUID"
                                    '    locIdentifier = arNameValuePair(1).ToString.Trim
                                    'updated 8/11/2014 to use propertyValuationId instead of locIdentifier
                                Case "PROPERTYVALUATIONID", "PROPERTYVALID", "PROPVALID", "PVID"
                                    propertyValuationId = arNameValuePair(1).ToString.Trim
                                Case "POLICYID", "PID"
                                    policyId = arNameValuePair(1).ToString.Trim
                                Case "POLICYIMAGENUM", "PIMN"
                                    policyImageNum = arNameValuePair(1).ToString.Trim
                            End Select
                        End If
                    Next
                ElseIf quoteId = "" AndAlso (UCase(uniqueValuationIdString).Contains("QUOTEID") = True OrElse UCase(uniqueValuationIdString).Contains("QID") = True) Then
                    arNameValuePair = Split(uniqueValuationIdString, "==")
                    quoteId = arNameValuePair(1).ToString.Trim
                End If
            End If
        End Sub
        'added 8/19/2014... just like UniqueValuationIdForQuote, VendorValuationIdForQuote and PropertyValuationIdForQuote; may not even use this logic here... may set beforehand
        'Public Function PropertyValuationVendorForQuote(ByVal qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1) As QuickQuotePropertyValuation.ValuationVendor
        'updated 7/28/2015 for buildingNum
        Public Function PropertyValuationVendorForQuote(ByVal qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1, Optional ByVal buildingNum As Integer = 0) As QuickQuotePropertyValuation.ValuationVendor
            Dim vendor As QuickQuotePropertyValuation.ValuationVendor = QuickQuotePropertyValuation.ValuationVendor.None

            If locationNum < 1 Then
                locationNum = 1
            End If

            If qq IsNot Nothing AndAlso qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locationNum Then
                With qq.Locations(locationNum - 1)
                    If buildingNum > 0 Then 'added IF 7/28/2015; original logic in ELSE
                        If .Buildings IsNot Nothing AndAlso .Buildings.Count >= buildingNum Then
                            With .Buildings(buildingNum - 1)
                                If .PropertyValuation IsNot Nothing Then
                                    'use existing
                                    vendor = .PropertyValuation.Vendor
                                End If
                            End With
                        End If
                    Else
                        If .PropertyValuation IsNot Nothing Then
                            'use existing
                            vendor = .PropertyValuation.Vendor
                        End If
                    End If
                End With
            End If

            Return vendor
        End Function
        'added 8/19/2014... just like UniqueValuationIdForQuote, VendorValuationIdForQuote, PropertyValuationIdForQuote, and PropertyValuationVendorForQuote; may not even use this logic here... may set beforehand
        'Public Function PropertyValuationVendorEstimatorTypeForQuote(ByVal qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1) As QuickQuotePropertyValuation.ValuationVendorEstimatorType
        'updated 7/28/2015 for buildingNum
        Public Function PropertyValuationVendorEstimatorTypeForQuote(ByVal qq As QuickQuoteObject, Optional ByVal locationNum As Integer = 1, Optional ByVal buildingNum As Integer = 0) As QuickQuotePropertyValuation.ValuationVendorEstimatorType
            Dim vendorEstimatorType As QuickQuotePropertyValuation.ValuationVendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.None

            If locationNum < 1 Then
                locationNum = 1
            End If

            If qq IsNot Nothing AndAlso qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locationNum Then
                With qq.Locations(locationNum - 1)
                    If buildingNum > 0 Then 'added IF 7/28/2015; original logic in ELSE
                        If .Buildings IsNot Nothing AndAlso .Buildings.Count >= buildingNum Then
                            With .Buildings(buildingNum - 1)
                                If .PropertyValuation IsNot Nothing Then
                                    'use existing
                                    vendorEstimatorType = .PropertyValuation.VendorEstimatorType
                                End If
                            End With
                        End If
                    Else
                        If .PropertyValuation IsNot Nothing Then
                            'use existing
                            vendorEstimatorType = .PropertyValuation.VendorEstimatorType
                        End If
                    End If
                End With
            End If

            Return vendorEstimatorType
        End Function
        'added 8/19/2014
        Public Function UrlForVendorValueRequest(ByVal qq As QuickQuoteObject, ByVal pvr As QuickQuotePropertyValuationRequest, Optional ByVal vendor As QuickQuotePropertyValuation.ValuationVendor = QuickQuotePropertyValuation.ValuationVendor.None, Optional ByVal vendorEstimatorType As QuickQuotePropertyValuation.ValuationVendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.None, Optional ByVal sendAllParamsForExistingValuation As Boolean = False) As String
            Dim u As String = ""

            If pvr IsNot Nothing Then
                'If vendor = QuickQuotePropertyValuation.ValuationVendor.None Then
                '    vendor = PropertyValuationVendorEstimatorTypeForQuote(qq:=, locationNum:=)
                'End If
                'If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.None Then
                '    vendorEstimatorType = PropertyValuationVendorEstimatorTypeForQuote(qq:=, locationNum:=)
                'End If
                If vendor <> QuickQuotePropertyValuation.ValuationVendor.None AndAlso vendorEstimatorType <> QuickQuotePropertyValuation.ValuationVendorEstimatorType.None Then
                    Select Case vendor
                        Case QuickQuotePropertyValuation.ValuationVendor.e2Value
                            Select Case vendorEstimatorType
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.A_And_A
                                    u = "https://evs.e2value.com/evs/est/bpa/default.asp" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.A_And_APro
                                    u = "https://evs.e2value.com/evs/est/bap/default.asp" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.AdditionalStructures
                                    u = "https://evs.e2value.com/evs/est/ada/default.asp" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.ExteriorResidential
                                    u = "https://evs.e2value.com/evs/est/exr/default.asp" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.FarmandRanch
                                    u = "https://evs.e2value.com/evs/est/fnr/default.asp" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.FullA_And_A 'added 8/22/2014
                                    u = "https://evs.e2value.com/evs/est/fla/default.asp" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.FullResidential
                                    u = "https://evs.e2value.com/evs/est/flr/default.asp" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead
                                    u = "https://evs.e2value.com/evs/est/hsd/default.asp" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.InQuest
                                    u = "https://evs.e2value.com/evs/est/inquest/default.aspx" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.InVision
                                    u = "https://evs.e2value.com/evs/est/invision/default.aspx" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoCommercial
                                    u = "https://evs.e2value.com/evs/est/pronto_cmm/default.aspx" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteCommercial
                                    u = "https://evs.e2value.com/evs/est/pronto_cmm_3/default.aspx" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential
                                    u = "https://evs.e2value.com/evs/est/pronto_res/default.aspx" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoResidential
                                    u = "https://evs.e2value.com/evs/est/pronto_res_ln/default.aspx" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.QCEPro
                                    u = "https://evs.e2value.com/evs/est/cmm/default.asp" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.QuickCommercial
                                    u = "https://evs.e2value.com/evs/est/qce/default.asp" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.Residential
                                    u = "https://evs.e2value.com/evs/est/bpr/default.asp" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.ResidentialBanking
                                    u = "https://evs.e2value.com/evs/est/av3/default.aspx" 'added 8/22/2014
                                Case QuickQuotePropertyValuation.ValuationVendorEstimatorType.ResidentialPro
                                    u = "https://evs.e2value.com/evs/est/bpp/default.asp" 'added 8/22/2014
                            End Select

                            'u = qqHelper.appendText(u, QuerystringForE2ValueRequest(pvr, sendAllParamsForExistingValuation), "") 'updated 8/20/2014 pm for sendAllParamsForExistingValuation
                            'updated 4/30/2015 to send vendorEstimatorType
                            u = qqHelper.appendText(u, QuerystringForE2ValueRequest(pvr, vendorEstimatorType, sendAllParamsForExistingValuation), "")
                        Case QuickQuotePropertyValuation.ValuationVendor.Verisk360
                            u = qqHelper.appendText(helper.configAppSettingValueAsString("VR_Verisk360_Url"), SubmitVerisk360RequestToSnapLogic(qq, pvr), "")
                    End Select
                End If

            End If

            Return u
        End Function
        'added 8/7/2014 - migrated from QuickQuotePropertyValuationRequest
        'Public Function QuerystringForE2ValueRequest(ByVal pvr As QuickQuotePropertyValuationRequest, Optional ByVal sendAllParamsForExistingValuation As Boolean = False) As String
        'updated 4/30/2015 to use vendorEstimatorType
        Public Function QuerystringForE2ValueRequest(ByVal pvr As QuickQuotePropertyValuationRequest, Optional ByVal vendorEstimatorType As QuickQuotePropertyValuation.ValuationVendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.None, Optional ByVal sendAllParamsForExistingValuation As Boolean = False) As String
            Dim qs As String = ""

            If pvr IsNot Nothing Then
                With pvr
                    'qs = "?ac=" & helper.UrlEncodedValue(.AuthCode)
                    'qs &= "&ad=" & helper.UrlEncodedValue(.AuthId)
                    'qs &= "&id=" & helper.UrlEncodedValue(.IFM_UniqueValuationId)
                    'qs &= "&propid=" & helper.UrlEncodedValue(.VendorValuationId)
                    'qs &= "&clientisbusiness=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ClientIsBusiness).ToString)
                    'qs &= "&cfn1=" & helper.UrlEncodedValue(.ClientFirstName)
                    'qs &= "&cmi1=" & helper.UrlEncodedValue(.ClientMiddleInitial)
                    'qs &= "&cln1=" & helper.UrlEncodedValue(.ClientLastName)
                    'qs &= "&paddr1=" & helper.UrlEncodedValue(.ClientAddress1)
                    'qs &= "&paddr2=" & helper.UrlEncodedValue(.ClientAddress2)
                    'qs &= "&pcity=" & helper.UrlEncodedValue(.ClientCity)
                    'qs &= "&pstate=" & helper.UrlEncodedValue(.ClientState)
                    'qs &= "&pzip=" & helper.UrlEncodedValue(.ClientZip)
                    'qs &= "&u=" & helper.UrlEncodedValue(.ReturnUrl)
                    'qs &= "&bname=" & helper.UrlEncodedValue(.ReturnUrlLinkText)
                    'qs &= "&yrblt=" & helper.UrlEncodedValue(.YearBuilt)
                    'qs &= "&rt_yrblt=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnYearBuilt).ToString)
                    'qs &= "&consttype=" & helper.UrlEncodedValue(.ConstructionType)
                    'qs &= "&rt_consttype=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnConstructionType).ToString)
                    'qs &= "&sqft=" & helper.UrlEncodedValue(.SquareFeet)
                    'qs &= "&rt_sqft=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnSquareFeet).ToString)
                    'qs &= "&rooftype=" & helper.UrlEncodedValue(.RoofType)
                    'qs &= "&rt_rooftype=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnRoofType).ToString)
                    'qs &= "&archstyle=" & helper.UrlEncodedValue(.ArchitecturalStyle)
                    'qs &= "&rt_archstyle=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnArchitecturalStyle).ToString)
                    'qs &= "&constquality=" & helper.UrlEncodedValue(.ConstructionQuality)
                    'qs &= "&rt_constquality=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnConstructionQuality).ToString)
                    'qs &= "&shape=" & helper.UrlEncodedValue(.PhysicalShape)
                    'qs &= "&rt_shape=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnPhysicalShape).ToString)
                    'qs &= "&exterior1=" & helper.UrlEncodedValue(.PrimaryExterior)
                    'qs &= "&rt_exterior1=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnPrimaryExterior).ToString)
                    'qs &= "&nopopup=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnUrlNoPopup).ToString)
                    'qs &= "&rt_rpctype=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnReplacementCostType).ToString)
                    'qs &= "&rt_areas=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnAdditionalAreas).ToString)

                    'updated 8/21/2014
                    qs = "?preload=1" 'added 8/28/2014... needed to populate in e2Value
                    qs &= "&ac=" & helper.UrlEncodedValue(.AuthCode) '8/28/2014 - changed first char from ? to & and updated to append instead of set
                    qs &= "&ad=" & helper.UrlEncodedValue(.AuthId)
                    qs &= "&id=" & helper.UrlEncodedValue(.IFM_UniqueValuationId)
                    If .VendorValuationId <> "" Then
                        qs &= "&propid=" & helper.UrlEncodedValue(.VendorValuationId)
                    End If
                    '8/28/2015 - moved client/address fields here (from IF below) so they're sent on every request
                    qs &= "&clientisbusiness=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ClientIsBusiness).ToString)
                    qs &= "&cfn1=" & helper.UrlEncodedValue(.ClientFirstName)
                    qs &= "&cmi1=" & helper.UrlEncodedValue(.ClientMiddleInitial)
                    qs &= "&cln1=" & helper.UrlEncodedValue(.ClientLastName)
                    qs &= "&paddr1=" & helper.UrlEncodedValue(.ClientAddress1)
                    qs &= "&paddr2=" & helper.UrlEncodedValue(.ClientAddress2)
                    qs &= "&pcity=" & helper.UrlEncodedValue(.ClientCity)
                    qs &= "&pstate=" & helper.UrlEncodedValue(.ClientState)
                    qs &= "&pzip=" & helper.UrlEncodedValue(.ClientZip)
                    If .VendorValuationId = "" OrElse sendAllParamsForExistingValuation = True Then
                        '4/29/2015 note: according to Angela Connolly (e2Value), P3R ignores client info in subsequent requests since the info (at least address) cannot be changed... HSD uses the client info since the data can be changed in e2Value
                        '8/28/2015 note: looks like FNR (and probably HSD) blank out the client/address info when the params are omitted on subsequent requests... will need to send all the time
                        '8/28/2015 note: logic moved before this IF so it happens on every request
                        'qs &= "&clientisbusiness=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ClientIsBusiness).ToString)
                        'qs &= "&cfn1=" & helper.UrlEncodedValue(.ClientFirstName)
                        'qs &= "&cmi1=" & helper.UrlEncodedValue(.ClientMiddleInitial)
                        'qs &= "&cln1=" & helper.UrlEncodedValue(.ClientLastName)
                        'qs &= "&paddr1=" & helper.UrlEncodedValue(.ClientAddress1)
                        'qs &= "&paddr2=" & helper.UrlEncodedValue(.ClientAddress2)
                        'qs &= "&pcity=" & helper.UrlEncodedValue(.ClientCity)
                        'qs &= "&pstate=" & helper.UrlEncodedValue(.ClientState)
                        'qs &= "&pzip=" & helper.UrlEncodedValue(.ClientZip)

                        If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential OrElse vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead Then 'added IF 4/30/2015; previously happening every time
                            qs &= "&yrblt=" & helper.UrlEncodedValue(.YearBuilt) '4/30/2015 note: P3R, HSD; no FNR
                        End If
                        If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential OrElse vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead Then 'added IF 4/30/2015; previously happening every time
                            If .ConstructionType <> "" Then '8/28/2014 - added IF so it will only go through if the static data mapping is in place
                                qs &= "&consttype=" & helper.UrlEncodedValue(.ConstructionType) '4/30/2015 note: P3R, HSD; no FNR
                            End If
                        End If
                        If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential OrElse vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead Then 'added IF 4/30/2015; previously happening every time
                            qs &= "&sqft=" & helper.UrlEncodedValue(.SquareFeet) '4/30/2015 note: P3R, HSD; no FNR
                        End If
                        If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential Then 'added IF 4/30/2015; previously happening every time
                            qs &= "&archstyle=" & helper.UrlEncodedValue(.ArchitecturalStyle) '4/30/2015 note: P3R; no HSD or FNR
                        End If

                        If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential OrElse vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead Then 'added IF 4/30/2015; previously happening every time
                            If .RoofType <> "" Then
                                qs &= "&rooftype=" & helper.UrlEncodedValue(.RoofType) '4/30/2015 note: P3R, HSD; no FNR
                            End If
                        End If
                        If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential Then 'added IF 4/30/2015; previously happening every time
                            If .ConstructionQuality <> "" Then
                                qs &= "&constquality=" & helper.UrlEncodedValue(.ConstructionQuality) '4/30/2015 note: P3R; no HSD or FNR
                            End If
                        End If
                        If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential Then 'added IF 4/30/2015; previously happening every time
                            If .PhysicalShape <> "" Then
                                qs &= "&shape=" & helper.UrlEncodedValue(.PhysicalShape) '4/30/2015 note: P3R; no HSD or FNR
                            End If
                        End If
                        If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential Then 'added IF 4/30/2015; previously happening every time
                            If .PrimaryExterior <> "" Then
                                qs &= "&exterior1=" & helper.UrlEncodedValue(.PrimaryExterior) '4/30/2015 note: P3R; no HSD or FNR
                            End If
                        End If
                    End If

                    qs &= "&u=" & helper.UrlEncodedValue(.ReturnUrl)
                    qs &= "&bname=" & helper.UrlEncodedValue(.ReturnUrlLinkText)
                    qs &= "&rt_propid=1" 'added 8/28/2014; could add request property for ReturnVendorValuationId
                    qs &= "&rt_yrblt=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnYearBuilt).ToString) '4/30/2015 note: P3R, HSD, FNR
                    qs &= "&rt_consttype=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnConstructionType).ToString) '4/30/2015 note: P3R, HSD, FNR
                    qs &= "&rt_sqft=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnSquareFeet).ToString) '4/30/2015 note: P3R, HSD, FNR
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential Then 'added IF 4/30/2015; previously happening every time
                        qs &= "&rt_rooftype=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnRoofType).ToString) '4/30/2015 note: P3R; no HSD or FNR
                    End If
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential Then 'added IF 4/30/2015; previously happening every time
                        qs &= "&rt_archstyle=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnArchitecturalStyle).ToString) '4/30/2015 note: P3R; no HSD or FNR
                    End If
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential Then 'added IF 4/30/2015; previously happening every time
                        qs &= "&rt_constquality=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnConstructionQuality).ToString) '4/30/2015 note: P3R; no HSD or FNR
                    End If
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential Then 'added IF 4/30/2015; previously happening every time
                        qs &= "&rt_shape=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnPhysicalShape).ToString) '4/30/2015 note: P3R; no HSD or FNR
                    End If
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential Then 'added IF 4/30/2015; previously happening every time
                        qs &= "&rt_exterior1=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnPrimaryExterior).ToString) '4/30/2015 note: P3R; no HSD or FNR
                    End If
                    qs &= "&nopopup=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnUrlNoPopup).ToString)
                    qs &= "&rt_rpctype=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnReplacementCostType).ToString)
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential Then 'added IF 4/30/2015; previously happening every time
                        qs &= "&rt_areas=" & helper.UrlEncodedValue(qqHelper.BooleanToInt(.ReturnAdditionalAreas).ToString) '4/30/2015 note: P3R; no HSD or FNR
                    End If
                    '4/29/2015 - testing new params for Homestead (HSD) and/or Farm & Ranch (FNR)... these weren't used w/ Pronto Lite Residential (P3R); 4/30/2015 note: may eventually create additional properties for these
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential Then 'added IF 4/30/2015
                        qs &= "&rt_numStories=1" 'this is a P3R param that wasn't originally coded for; not valid for HSD or FNR
                    End If
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.FarmandRanch OrElse vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead Then 'added IF 4/30/2015
                        qs &= "&rt_structtype=1" 'FNR or HSD; no P3R
                    End If
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.FarmandRanch OrElse vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead Then 'added IF 4/30/2015
                        qs &= "&rt_roofcov=1" 'FNR or HSD; no P3R
                    End If
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.FarmandRanch OrElse vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead Then 'added IF 4/30/2015
                        qs &= "&rt_numfloors=1" 'FNR or HSD; no P3R
                    End If
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.FarmandRanch OrElse vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead Then 'added IF 4/30/2015
                        qs &= "&rt_ovrlcondition=1" 'FNR or HSD; no P3R
                    End If
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.FarmandRanch OrElse vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead Then 'added IF 4/30/2015
                        qs &= "&rt_extwall=1" 'FNR or HSD; no P3R
                    End If
                    If vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.FarmandRanch OrElse vendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead Then 'added IF 5/27/2015
                        If E2ValueRequest_AppendReturnStructuresRpc() = True Then
                            qs &= "&return_structuresrpc=1" 'FNR or HSD; no P3R
                        End If
                    End If

                End With
            End If

            Return qs

            'could also do w/ string builder
            'Dim sbQS As New StringBuilder
            'sbQS.Append("")
            'Return sbQS.ToString
        End Function

        Public Sub LoadE2ValueResponseFromNameValueCollection(ByVal nvc As NameValueCollection, ByRef pvr As QuickQuotePropertyValuationResponse, Optional ByVal resetValuationResponse As Boolean = False) 'System.Collections.Specialized.NameValueCollection (i.e. QueryString)
            'If nvc IsNot Nothing AndAlso nvc.Count > 0 Then 'works but will use nvc.Keys since that's what the code below uses
            If nvc IsNot Nothing AndAlso nvc.Keys IsNot Nothing AndAlso nvc.Keys.Count > 0 Then 'System.Collections.Specialized.NameObjectCollectionBase.KeysCollection
                'For Each kvp As KeyValuePair(Of String, String) In nvc 'didn't work... invalid cast
                '    Dim keyValueString As String = kvp.Key & " - " & kvp.Value
                'Next

                'For Each key As String In nvc.Keys 'this works
                '    For Each value As String In nvc.GetValues(key)
                '        Dim keyValueString As String = key & " - " & value
                '    Next
                'Next

                'Dim key As String = ""
                'Dim value As String = ""
                'Dim keyValueString As String = ""
                'Dim keyValueString2 As String = ""
                'For Each key In nvc.Keys
                '    For Each value In nvc.GetValues(key)
                '        keyValueString = key & " - " & value
                '    Next
                '    keyValueString2 = key & " - " & nvc(key)
                'Next
                '?key1=a&key2=b&key1=aa&key3=c&key4=d
                '4 total keys (nvc.Count or nvc.Keys.Count); key1 had 2 values
                'keyValueString = aa 'after last iteration
                'keyValueString2 = a,aa 'combines multiple values into 1 string

                '?key1=&key1=a&key2=b&key1=aa&key3=c&key4=d&key1=
                '4 total keys (nvc.Count or nvc.Keys.Count); key1 had 2 values
                'keyValueString =  'after last iteration
                'keyValueString2 = ,a,aa, 'combines multiple values into 1 string

                If pvr Is Nothing Then
                    pvr = New QuickQuotePropertyValuationResponse
                ElseIf resetValuationResponse = True Then
                    pvr = New QuickQuotePropertyValuationResponse
                End If

                With pvr
                    .VendorParams = nvc.ToString 'added 8/8/2014
                    For Each key As String In nvc.Keys
                        Dim vals As String() = nvc.GetValues(key)
                        If vals IsNot Nothing AndAlso vals.Count > 0 Then
                            Dim valCounter As Integer = 0
                            For Each value As String In vals
                                value = helper.UrlDecodedValue(value)
                                valCounter += 1
                                Select Case UCase(key)
                                    Case UCase("return_ad")
                                        'AuthId
                                        If valCounter < 2 Then
                                            '1st instance
                                            .AuthId = value
                                        Else
                                            'at least 2nd instance
                                            If .AuthId = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .AuthId = value
                                            End If
                                        End If
                                    Case UCase("return_id")
                                        'IFM_UniqueValuationId
                                        If valCounter < 2 Then
                                            '1st instance
                                            .IFM_UniqueValuationId = value
                                        Else
                                            'at least 2nd instance
                                            If .IFM_UniqueValuationId = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .IFM_UniqueValuationId = value
                                            End If
                                        End If
                                    Case UCase("rt_propid")
                                        'VendorValuationId
                                        If valCounter < 2 Then
                                            '1st instance
                                            .VendorValuationId = value
                                        Else
                                            'at least 2nd instance
                                            If .VendorValuationId = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .VendorValuationId = value
                                            End If
                                        End If
                                    Case UCase("rt_yrblt")
                                        'YearBuilt
                                        If valCounter < 2 Then
                                            '1st instance
                                            .YearBuilt = value
                                        Else
                                            'at least 2nd instance
                                            If .YearBuilt = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .YearBuilt = value
                                            End If
                                        End If
                                    Case UCase("rt_consttype")
                                        'ConstructionType
                                        If valCounter < 2 Then
                                            '1st instance
                                            .ConstructionType = value
                                        Else
                                            'at least 2nd instance
                                            If .ConstructionType = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .ConstructionType = value
                                            End If
                                        End If
                                    Case UCase("rt_sqft")
                                        'SquareFeet
                                        If valCounter < 2 Then
                                            '1st instance
                                            .SquareFeet = value
                                        Else
                                            'at least 2nd instance
                                            If .SquareFeet = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .SquareFeet = value
                                            End If
                                        End If
                                    Case UCase("rt_rooftype")
                                        'RoofType
                                        If valCounter < 2 Then
                                            '1st instance
                                            .RoofType = value
                                        Else
                                            'at least 2nd instance
                                            If .RoofType = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .RoofType = value
                                            End If
                                        End If
                                    Case UCase("rt_archstyle")
                                        'ArchitecturalStyle
                                        If valCounter < 2 Then
                                            '1st instance
                                            .ArchitecturalStyle = value
                                        Else
                                            'at least 2nd instance
                                            If .ArchitecturalStyle = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .ArchitecturalStyle = value
                                            End If
                                        End If
                                    Case UCase("rt_constquality")
                                        'ConstructionQuality
                                        If valCounter < 2 Then
                                            '1st instance
                                            .ConstructionQuality = value
                                        Else
                                            'at least 2nd instance
                                            If .ConstructionQuality = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .ConstructionQuality = value
                                            End If
                                        End If
                                    Case UCase("rt_shape")
                                        'PhysicalShape
                                        If valCounter < 2 Then
                                            '1st instance
                                            .PhysicalShape = value
                                        Else
                                            'at least 2nd instance
                                            If .PhysicalShape = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .PhysicalShape = value
                                            End If
                                        End If
                                    Case UCase("rt_exterior1")
                                        'PrimaryExterior
                                        If valCounter < 2 Then
                                            '1st instance
                                            .PrimaryExterior = value
                                        Else
                                            'at least 2nd instance
                                            If .PrimaryExterior = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .PrimaryExterior = value
                                            End If
                                        End If
                                    Case UCase("return_rpc")
                                        'ReplacementCostValue
                                        If valCounter < 2 Then
                                            '1st instance
                                            .ReplacementCostValue = value
                                        Else
                                            'at least 2nd instance
                                            If .ReplacementCostValue = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .ReplacementCostValue = value
                                            End If
                                        End If
                                    Case UCase("rt_rpctype")
                                        'ReplacementCostType
                                        If valCounter < 2 Then
                                            '1st instance
                                            .ReplacementCostType = value
                                        Else
                                            'at least 2nd instance
                                            If .ReplacementCostType = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .ReplacementCostType = value
                                            End If
                                        End If
                                    Case UCase("rt_areas")
                                        'AdditionalAreas
                                        If valCounter < 2 Then
                                            '1st instance
                                            .AdditionalAreas = value
                                        Else
                                            'at least 2nd instance
                                            If .AdditionalAreas = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .AdditionalAreas = value
                                            End If
                                        End If

                                        'added 7/29/2015 for Farm
                                    Case UCase("return_structuresrpc")
                                        'StructuresReplacementCostValue
                                        If valCounter < 2 Then
                                            '1st instance
                                            .StructuresReplacementCostValue = value
                                        Else
                                            'at least 2nd instance
                                            If .StructuresReplacementCostValue = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .StructuresReplacementCostValue = value
                                            End If
                                        End If
                                    Case UCase("return_structuresrpc_total")
                                        'StructuresReplacementCostValueTotal
                                        If valCounter < 2 Then
                                            '1st instance
                                            .StructuresReplacementCostValueTotal = value
                                        Else
                                            'at least 2nd instance
                                            If .StructuresReplacementCostValueTotal = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .StructuresReplacementCostValueTotal = value
                                            End If
                                        End If
                                    Case UCase("return_unique")
                                        'UniqueItemsTotalValue
                                        If valCounter < 2 Then
                                            '1st instance
                                            .UniqueItemsTotalValue = value
                                        Else
                                            'at least 2nd instance
                                            If .UniqueItemsTotalValue = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .UniqueItemsTotalValue = value
                                            End If
                                        End If
                                    Case UCase("return_ACV")
                                        'ActualCashValue
                                        If valCounter < 2 Then
                                            '1st instance
                                            .ActualCashValue = value
                                        Else
                                            'at least 2nd instance
                                            If .ActualCashValue = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .ActualCashValue = value
                                            End If
                                        End If
                                End Select
                            Next
                        End If
                    Next
                End With

            End If

        End Sub

        Public Sub LoadVerisk360ValueResponseFromNameValueCollection(ByVal nvc As NameValueCollection, ByRef pvr As QuickQuotePropertyValuationResponse, Optional ByVal resetValuationResponse As Boolean = False) 'System.Collections.Specialized.NameValueCollection (i.e. QueryString)
            'If nvc IsNot Nothing AndAlso nvc.Count > 0 Then 'works but will use nvc.Keys since that's what the code below uses
            If nvc IsNot Nothing AndAlso nvc.Keys IsNot Nothing AndAlso nvc.Keys.Count > 0 Then 'System.Collections.Specialized.NameObjectCollectionBase.KeysCollection
                'For Each kvp As KeyValuePair(Of String, String) In nvc 'didn't work... invalid cast
                '    Dim keyValueString As String = kvp.Key & " - " & kvp.Value
                'Next

                'For Each key As String In nvc.Keys 'this works
                '    For Each value As String In nvc.GetValues(key)
                '        Dim keyValueString As String = key & " - " & value
                '    Next
                'Next

                'Dim key As String = ""
                'Dim value As String = ""
                'Dim keyValueString As String = ""
                'Dim keyValueString2 As String = ""
                'For Each key In nvc.Keys
                '    For Each value In nvc.GetValues(key)
                '        keyValueString = key & " - " & value
                '    Next
                '    keyValueString2 = key & " - " & nvc(key)
                'Next
                '?key1=a&key2=b&key1=aa&key3=c&key4=d
                '4 total keys (nvc.Count or nvc.Keys.Count); key1 had 2 values
                'keyValueString = aa 'after last iteration
                'keyValueString2 = a,aa 'combines multiple values into 1 string

                '?key1=&key1=a&key2=b&key1=aa&key3=c&key4=d&key1=
                '4 total keys (nvc.Count or nvc.Keys.Count); key1 had 2 values
                'keyValueString =  'after last iteration
                'keyValueString2 = ,a,aa, 'combines multiple values into 1 string

                If pvr Is Nothing Then
                    pvr = New QuickQuotePropertyValuationResponse
                ElseIf resetValuationResponse = True Then
                    pvr = New QuickQuotePropertyValuationResponse
                End If

                With pvr
                    .VendorParams = nvc.ToString 'added 8/8/2014
                    For Each key As String In nvc.Keys
                        Dim vals As String() = nvc.GetValues(key)
                        If vals IsNot Nothing AndAlso vals.Count > 0 Then
                            Dim valCounter As Integer = 0
                            For Each value As String In vals
                                value = helper.UrlDecodedValue(value)
                                valCounter += 1
                                Select Case UCase(key)
                                    'Case UCase("return_ad")
                                    '    'AuthId
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .AuthId = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .AuthId = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .AuthId = value
                                    '        End If
                                    '    End If
                                    Case UCase("360_policynumber")
                                        'IFM_UniqueValuationId
                                        If valCounter < 2 Then
                                            '1st instance
                                            .IFM_UniqueValuationId = value
                                        Else
                                            'at least 2nd instance
                                            If .IFM_UniqueValuationId = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .IFM_UniqueValuationId = value
                                            End If
                                        End If
                                    Case UCase("360_valuationid")
                                        'VendorValuationId
                                        If valCounter < 2 Then
                                            '1st instance
                                            .VendorValuationId = value
                                        Else
                                            'at least 2nd instance
                                            If .VendorValuationId = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .VendorValuationId = value
                                            End If
                                        End If
                                    'Case UCase("rt_yrblt")
                                    '    'YearBuilt
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .YearBuilt = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .YearBuilt = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .YearBuilt = value
                                    '        End If
                                    '    End If
                                    'Case UCase("rt_consttype")
                                    '    'ConstructionType
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .ConstructionType = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .ConstructionType = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .ConstructionType = value
                                    '        End If
                                    '    End If
                                    'Case UCase("rt_sqft")
                                    '    'SquareFeet
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .SquareFeet = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .SquareFeet = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .SquareFeet = value
                                    '        End If
                                    '    End If
                                    'Case UCase("rt_rooftype")
                                    '    'RoofType
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .RoofType = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .RoofType = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .RoofType = value
                                    '        End If
                                    '    End If
                                    'Case UCase("rt_archstyle")
                                    '    'ArchitecturalStyle
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .ArchitecturalStyle = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .ArchitecturalStyle = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .ArchitecturalStyle = value
                                    '        End If
                                    '    End If
                                    'Case UCase("rt_constquality")
                                    '    'ConstructionQuality
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .ConstructionQuality = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .ConstructionQuality = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .ConstructionQuality = value
                                    '        End If
                                    '    End If
                                    'Case UCase("rt_shape")
                                    '    'PhysicalShape
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .PhysicalShape = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .PhysicalShape = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .PhysicalShape = value
                                    '        End If
                                    '    End If
                                    'Case UCase("rt_exterior1")
                                    '    'PrimaryExterior
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .PrimaryExterior = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .PrimaryExterior = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .PrimaryExterior = value
                                    '        End If
                                    '    End If
                                    Case UCase("360_rcv")
                                        'ReplacementCostValue
                                        If valCounter < 2 Then
                                            '1st instance
                                            .ReplacementCostValue = value
                                        Else
                                            'at least 2nd instance
                                            If .ReplacementCostValue = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .ReplacementCostValue = value
                                            End If
                                        End If
                                    'Case UCase("rt_rpctype")
                                    '    'ReplacementCostType
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .ReplacementCostType = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .ReplacementCostType = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .ReplacementCostType = value
                                    '        End If
                                    '    End If
                                    'Case UCase("rt_areas")
                                    '    'AdditionalAreas
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .AdditionalAreas = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .AdditionalAreas = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .AdditionalAreas = value
                                    '        End If
                                    '    End If

                                    '    'added 7/29/2015 for Farm
                                    'Case UCase("return_structuresrpc")
                                    '    'StructuresReplacementCostValue
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .StructuresReplacementCostValue = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .StructuresReplacementCostValue = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .StructuresReplacementCostValue = value
                                    '        End If
                                    '    End If
                                    'Case UCase("return_structuresrpc_total")
                                    '    'StructuresReplacementCostValueTotal
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .StructuresReplacementCostValueTotal = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .StructuresReplacementCostValueTotal = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .StructuresReplacementCostValueTotal = value
                                    '        End If
                                    '    End If
                                    'Case UCase("return_unique")
                                    '    'UniqueItemsTotalValue
                                    '    If valCounter < 2 Then
                                    '        '1st instance
                                    '        .UniqueItemsTotalValue = value
                                    '    Else
                                    '        'at least 2nd instance
                                    '        If .UniqueItemsTotalValue = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                    '            .UniqueItemsTotalValue = value
                                    '        End If
                                    '    End If
                                    Case UCase("360_acv")
                                        'ActualCashValue
                                        If valCounter < 2 Then
                                            '1st instance
                                            .ActualCashValue = value
                                        Else
                                            'at least 2nd instance
                                            If .ActualCashValue = "" OrElse String.IsNullOrWhiteSpace(value) = False Then
                                                .ActualCashValue = value
                                            End If
                                        End If
                                End Select
                            Next
                        End If
                    Next
                End With

            End If

        End Sub


        '8/20/2014 note: needs to be adjusted to use InsertPropertyValuationResponse and optionally use SavePropertyValuationResponseBackToQuote and return the QuickQuoteObject or at least the quoteId and lobId so the calling method knows what page to redirect to; removed 8/21/2014
        'Public Sub ProcessE2ValueResponseFromNameValueCollection(ByVal nvc As NameValueCollection)
        '    If nvc IsNot Nothing AndAlso nvc.Keys IsNot Nothing AndAlso nvc.Keys.Count > 0 Then
        '        Dim pvr As New QuickQuotePropertyValuationResponse
        '        LoadE2ValueResponseFromNameValueCollection(nvc, pvr)

        '        '8/20/2014 note: need to finish updating; previous code will be commented below
        '        Dim saveErrorMsg As String = ""
        '        InsertPropertyValuationResponse(pvr, saveErrorMsg) 'will set pvr.db_propertyValuationId from pvr.IFM_UniqueValuationId if needed
        '        'SavePropertyValuationResponseBackToQuote(pvr, wasAnythingNewSaved:=, errorMsg:=, saveAllReturnValues:=, ignoreLoadedBackIntoQuoteFlag:=)

        '        If pvr IsNot Nothing AndAlso pvr.IFM_UniqueValuationId <> "" Then
        '            If (pvr.db_propertyValuationId = "" OrElse IsNumeric(pvr.db_propertyValuationId) = False) AndAlso pvr.IFM_UniqueValuationId <> "" Then 'added 8/20/2014
        '                Dim quoteId As String = ""
        '                Dim locNum As String = ""
        '                Dim propertyValuationId As String = ""
        '                SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, propertyValuationId)
        '                If propertyValuationId <> "" AndAlso IsNumeric(propertyValuationId) = True Then
        '                    pvr.db_propertyValuationId = propertyValuationId
        '                End If
        '            End If

        '            'Dim quoteId As String = ""
        '            'Dim locNum As String = ""
        '            'Dim locIdentifier As String = ""
        '            'SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, locIdentifier)
        '            'If quoteId <> "" AndAlso IsNumeric(quoteId) = True Then
        '            '    Dim qq As QuickQuoteObject = Nothing
        '            '    Dim errorMsg As String = ""
        '            '    qqXml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, qq, errorMsg)
        '            '    If qq IsNot Nothing Then
        '            '        If locNum = "" OrElse IsNumeric(locNum) = False Then
        '            '            locNum = "1"
        '            '        End If
        '            '        If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
        '            '            Dim foundLoc As Boolean = False
        '            '            For Each l As QuickQuoteLocation In qq.Locations
        '            '                'might also include propertyValuationIdentifier
        '            '                If l.PropertyValuation IsNot Nothing Then
        '            '                    If l.PropertyValuation.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId Then
        '            '                        foundLoc = True
        '            '                        'may need to set IFM_UniqueValuationId on PropertyValuationRequest
        '            '                    ElseIf l.PropertyValuation.Request IsNot Nothing AndAlso l.PropertyValuation.Request.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId Then
        '            '                        foundLoc = True
        '            '                        'may need to set IFM_UniqueValuationId on PropertyValuation
        '            '                    End If
        '            '                    If foundLoc = True Then

        '            '                        Exit For
        '            '                    End If
        '            '                End If
        '            '            Next
        '            '            If foundLoc = False AndAlso locIdentifier <> "" Then
        '            '                For Each l As QuickQuoteLocation In qq.Locations
        '            '                    'might also include propertyValuationIdentifier
        '            '                    If l.UniqueIdentifier = locIdentifier Then
        '            '                        foundLoc = True

        '            '                        Exit For
        '            '                    End If
        '            '                Next
        '            '            End If
        '            '            If foundLoc = False Then
        '            '                If qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locNum Then
        '            '                    foundLoc = True
        '            '                    With qq.Locations(locNum - 1)

        '            '                    End With
        '            '                End If
        '            '            End If
        '            '        End If

        '            '    End If
        '            'End If
        '        End If
        '    End If
        'End Sub
        '8/20/2014 note: may also have another similar method that sets the QuickQuote object
        Public Sub ProcessE2ValueResponseFromNameValueCollectionAndSetQuoteIdAndLobId(ByVal nvc As NameValueCollection, ByRef quoteId As String, ByRef lobId As String, Optional ByVal saveToQuote As Boolean = False, Optional ByRef saveWasSuccessful As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal saveAllReturnValues As Boolean = False, Optional ByRef policyId As String = "", Optional ByRef policyImageNum As String = "")
            'quoteId = ""
            'lobId = ""
            'errorMsg = ""
            'saveWasSuccessful = False

            'If nvc IsNot Nothing AndAlso nvc.Keys IsNot Nothing AndAlso nvc.Keys.Count > 0 Then
            '    Dim pvr As New QuickQuotePropertyValuationResponse
            '    LoadE2ValueResponseFromNameValueCollection(nvc, pvr)

            '    '8/20/2014 note: need to finish updating; previous code will be commented below
            '    'SavePropertyValuationResponseBackToQuote(pvr, wasAnythingNewSaved:=, errorMsg:=, saveAllReturnValues:=, ignoreLoadedBackIntoQuoteFlag:=)
            '    If pvr IsNot Nothing Then
            '        If pvr.IFM_UniqueValuationId <> "" Then
            '            If (pvr.db_propertyValuationId = "" OrElse IsNumeric(pvr.db_propertyValuationId) = False) AndAlso pvr.IFM_UniqueValuationId <> "" Then 'added 8/20/2014
            '                Dim locNum As String = ""
            '                Dim buildNum As String = "" 'added 7/28/2015
            '                Dim propertyValuationId As String = ""
            '                'SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, propertyValuationId)
            '                'updated 7/28/2015 for buildingNum
            '                SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, buildNum, propertyValuationId)
            '                If propertyValuationId <> "" AndAlso IsNumeric(propertyValuationId) = True Then
            '                    pvr.db_propertyValuationId = propertyValuationId
            '                End If
            '                Dim saveErrorMsg As String = ""
            '                InsertPropertyValuationResponse(pvr, saveErrorMsg) 'will set pvr.db_propertyValuationId from pvr.IFM_UniqueValuationId if needed

            '                If quoteId <> "" Then
            '                    'set lobId and possibly save to quote
            '                    Dim qq As QuickQuoteObject = Nothing
            '                    If saveToQuote = True Then
            '                        Dim wasAnythingNewLoaded As Boolean = False
            '                        Dim didSaveOccur As Boolean = False
            '                        SavePropertyValuationResponseBackToQuote(pvr, qq, wasAnythingNewLoaded, didSaveOccur, errorMsg, saveAllReturnValues) 'not passing ignoreLoadedBackIntoQuoteFlag; defaulted to True
            '                        If didSaveOccur = True Then
            '                            saveWasSuccessful = True
            '                        End If
            '                        If qq IsNot Nothing Then
            '                            lobId = qq.LobId
            '                            If lobId = "" Then
            '                                lobId = qq.Database_LobId
            '                            End If
            '                        End If
            '                    End If
            '                    If lobId = "" AndAlso qq Is Nothing AndAlso IsNumeric(quoteId) = True Then 'updated to also verify that quoteId is numeric
            '                        'lookup lobId in Quote table by QuoteId
            '                        'updated 8/22/2014
            '                        Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connQQ"))
            '                            sql.queryOrStoredProc = "select Q.lobId from Quote as Q with (nolock) where Q.quoteId = " & CInt(quoteId)
            '                            Dim dr As SqlDataReader = sql.GetDataReader
            '                            If dr IsNot Nothing AndAlso dr.HasRows = True Then
            '                                dr.Read()
            '                                lobId = dr.Item("lobId").ToString.Trim
            '                            Else
            '                                If sql.hasError = True Then
            '                                    'error
            '                                    'errorMsg = "error retrieving lobId from database"
            '                                Else
            '                                    'no results
            '                                    'errorMsg = "unable to locate quote in database"
            '                                End If
            '                            End If
            '                        End Using
            '                    End If
            '                Else
            '                    errorMsg = "no quoteId found in property valuation response identifier"
            '                End If
            '            End If
            '        Else
            '            errorMsg = "no identifier found for property valuation response"
            '            Dim saveErrorMsg As String = ""
            '            InsertPropertyValuationResponse(pvr, saveErrorMsg)
            '        End If
            '    Else
            '        errorMsg = "unable to convert name value collection to property valuation response"
            '    End If
            'Else
            '    errorMsg = "name value collection is empty"
            'End If

            'updated 7/31/2015 to call new method
            Dim locationNum As Integer = 0
            Dim buildingNum As Integer = 0
            ProcessE2ValueResponseFromNameValueCollectionAndSetVariables(nvc, quoteId, lobId, locationNum, buildingNum, saveToQuote, saveWasSuccessful, errorMsg, saveAllReturnValues, policyId, policyImageNum)
        End Sub

        Public Sub ProcessE2ValueResponseFromNameValueCollectionAndSetVariables(ByVal nvc As NameValueCollection, ByRef quoteId As String, ByRef lobId As String, ByRef locationNum As Integer, ByRef buildingNum As Integer, Optional ByVal saveToQuote As Boolean = False, Optional ByRef saveWasSuccessful As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal saveAllReturnValues As Boolean = False, Optional ByRef policyId As String = "", Optional ByRef policyImageNum As String = "")
            ProcessVendorValueResponseFromNameValueCollectionAndSetVariables(nvc, quoteId, lobId, locationNum, buildingNum, saveToQuote, saveWasSuccessful, errorMsg, saveAllReturnValues, policyId, policyImageNum, QuickQuotePropertyValuation.ValuationVendor.e2Value)
        End Sub

        Public Sub Process360ValueResponseFromNameValueCollectionAndSetVariables(ByVal nvc As NameValueCollection, ByRef quoteId As String, ByRef lobId As String, ByRef locationNum As Integer, ByRef buildingNum As Integer, Optional ByVal saveToQuote As Boolean = False, Optional ByRef saveWasSuccessful As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal saveAllReturnValues As Boolean = False, Optional ByRef policyId As String = "", Optional ByRef policyImageNum As String = "")
            ProcessVendorValueResponseFromNameValueCollectionAndSetVariables(nvc, quoteId, lobId, locationNum, buildingNum, saveToQuote, saveWasSuccessful, errorMsg, saveAllReturnValues, policyId, policyImageNum, QuickQuotePropertyValuation.ValuationVendor.Verisk360)
        End Sub

        'added 7/31/2015; originally copied from ProcessE2ValueResponseFromNameValueCollectionAndSetQuoteIdAndLobId
        Public Sub ProcessVendorValueResponseFromNameValueCollectionAndSetVariables(ByVal nvc As NameValueCollection, ByRef quoteId As String, ByRef lobId As String, ByRef locationNum As Integer, ByRef buildingNum As Integer, Optional ByVal saveToQuote As Boolean = False, Optional ByRef saveWasSuccessful As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal saveAllReturnValues As Boolean = False, Optional ByRef policyId As String = "", Optional ByRef policyImageNum As String = "", Optional ByVal valuationVendor As QuickQuotePropertyValuation.ValuationVendor = QuickQuotePropertyValuation.ValuationVendor.e2Value)
            quoteId = ""
            lobId = ""
            errorMsg = ""
            policyId = ""
            policyImageNum = ""
            saveWasSuccessful = False
            'added 7/31/2015
            locationNum = 0
            buildingNum = 0

            If nvc IsNot Nothing AndAlso nvc.Keys IsNot Nothing AndAlso nvc.Keys.Count > 0 Then
                Dim pvr As New QuickQuotePropertyValuationResponse


                Select Case valuationVendor
                    Case QuickQuotePropertyValuation.ValuationVendor.e2Value
                        LoadE2ValueResponseFromNameValueCollection(nvc, pvr)
                    Case QuickQuotePropertyValuation.ValuationVendor.Verisk360
                        LoadVerisk360ValueResponseFromNameValueCollection(nvc, pvr)

                End Select

                '8/20/2014 note: need to finish updating; previous code will be commented below
                'SavePropertyValuationResponseBackToQuote(pvr, wasAnythingNewSaved:=, errorMsg:=, saveAllReturnValues:=, ignoreLoadedBackIntoQuoteFlag:=)
                If pvr IsNot Nothing Then
                    If pvr.IFM_UniqueValuationId <> "" Then
                        If (pvr.db_propertyValuationId = "" OrElse IsNumeric(pvr.db_propertyValuationId) = False) AndAlso pvr.IFM_UniqueValuationId <> "" Then 'added 8/20/2014
                            Dim locNum As String = ""
                            Dim buildNum As String = "" 'added 7/28/2015
                            Dim propertyValuationId As String = ""
                            'SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, propertyValuationId)
                            'updated 7/28/2015 for buildingNum
                            SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, buildNum, propertyValuationId, policyId, policyImageNum)

                            Dim qq As QuickQuoteObject = GetQuoteForPropertyValuationResponse(pvr, errorMsg)

                            'added 7/31/2015
                            If qqHelper.IsValidQuickQuoteIdOrNum(locNum) = True Then
                                locationNum = CInt(locNum)
                            End If
                            If qqHelper.IsValidQuickQuoteIdOrNum(buildNum) = True Then
                                buildingNum = CInt(buildNum)
                            End If

                            If propertyValuationId <> "" AndAlso IsNumeric(propertyValuationId) = True Then
                                pvr.db_propertyValuationId = propertyValuationId
                            End If
                            Dim saveErrorMsg As String = ""
                            InsertPropertyValuationResponse(pvr, saveErrorMsg) 'will set pvr.db_propertyValuationId from pvr.IFM_UniqueValuationId if needed

                            If Not String.IsNullOrWhiteSpace(quoteId) OrElse Not String.IsNullOrWhiteSpace(policyId) Then
                                'set lobId and possibly save to quote
                                'Dim qq As QuickQuoteObject = Nothing
                                If saveToQuote = True Then
                                    Dim wasAnythingNewLoaded As Boolean = False
                                    Dim didSaveOccur As Boolean = False
                                    'SavePropertyValuationResponseBackToQuote(pvr, qq, wasAnythingNewLoaded, didSaveOccur, errorMsg, saveAllReturnValues) 'not passing ignoreLoadedBackIntoQuoteFlag; defaulted to True
                                    'updated 7/31/2015 for new optional byref params; also sending default value for ignoreLoadedBackIntoQuoteFlag
                                    Dim valuationLocationNum As Integer = 0
                                    Dim valuationBuildingNum As Integer = 0
                                    SavePropertyValuationResponseBackToQuote(pvr, qq, wasAnythingNewLoaded, didSaveOccur, errorMsg, saveAllReturnValues, True, valuationLocationNum, valuationBuildingNum)
                                    If valuationLocationNum > 0 Then 'now update if something comes back
                                        locationNum = valuationLocationNum
                                        buildingNum = valuationBuildingNum
                                    End If
                                    If didSaveOccur = True Then
                                        saveWasSuccessful = True
                                    End If
                                    If qq IsNot Nothing Then
                                        lobId = qq.LobId
                                        If lobId = "" Then
                                            lobId = qq.Database_LobId
                                        End If
                                    End If
                                End If
                                If lobId = "" AndAlso qq Is Nothing AndAlso IsNumeric(quoteId) = True Then 'updated to also verify that quoteId is numeric
                                    'lookup lobId in Quote table by QuoteId
                                    'updated 8/22/2014
                                    Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connQQ"))
                                        sql.queryOrStoredProc = "select Q.lobId from Quote as Q with (nolock) where Q.quoteId = " & CInt(quoteId)
                                        Dim dr As SqlDataReader = sql.GetDataReader
                                        If dr IsNot Nothing AndAlso dr.HasRows = True Then
                                            dr.Read()
                                            lobId = dr.Item("lobId").ToString.Trim
                                        Else
                                            If sql.hasError = True Then
                                                'error
                                                'errorMsg = "error retrieving lobId from database"
                                            Else
                                                'no results
                                                'errorMsg = "unable to locate quote in database"
                                            End If
                                        End If
                                    End Using

                                End If

                                If lobId = "" AndAlso qq Is Nothing AndAlso IsNumeric(policyId) AndAlso IsNumeric(policyImageNum) Then 'updated to also verify that quoteId is numeric
                                    'lookup lobId in Quote table by QuoteId
                                    'updated 6/2/2023
                                    Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connDiamond"))
                                        sql.queryOrStoredProc = "SELECT V.lob_id from PolicyImage as PI WITH (NOLOCK) INNER JOIN Version as V WITH (NOLOCK) on V.version_id = PI.version_id WHERE PI.policy_id = " & CInt(policyId) & "and PI.policyimage_num = " & CInt(policyImageNum)
                                        Dim dr As SqlDataReader = sql.GetDataReader
                                        If dr IsNot Nothing AndAlso dr.HasRows Then
                                            dr.Read()
                                            lobId = dr.Item("lob_id").ToString.Trim
                                        Else
                                            If sql.hasError Then
                                                'error
                                                'errorMsg = "error retrieving lobId from database"
                                            Else
                                                'no results
                                                'errorMsg = "unable to locate quote in database"
                                            End If
                                        End If
                                    End Using
                                    lobId = QuickQuoteHelperClass.LobIdForMasterLobId(qqHelper.IntegerForString(lobId)).ToString()
                                End If
                            Else
                                errorMsg = "no quoteId/policyId found in property valuation response identifier"
                            End If


                        End If
                    Else
                        errorMsg = "no identifier found for property valuation response"
                        Dim saveErrorMsg As String = ""
                        InsertPropertyValuationResponse(pvr, saveErrorMsg)
                    End If
                Else
                    errorMsg = "unable to convert name value collection to property valuation response"
                End If
            Else
                errorMsg = "name value collection is empty"
            End If
        End Sub
        Public Sub InsertPropertyValuation(ByRef pv As QuickQuotePropertyValuation, Optional ByRef errorMsg As String = "")
            If pv IsNot Nothing Then
                Using sql As New SQLexecuteObject(ConfigurationManager.AppSettings("connQQ"))
                    sql.queryOrStoredProc = "usp_Insert_PropertyValuation"
                    sql.inputParameters = New ArrayList
                    If pv.db_environment <> "" Then 'added 8/13/2014
                        sql.inputParameters.Add(New SqlParameter("@environment", pv.db_environment))
                    End If
                    If pv.db_quoteId <> "" AndAlso IsNumeric(pv.db_quoteId) = True Then
                        sql.inputParameters.Add(New SqlParameter("@quoteId", CInt(pv.db_quoteId)))
                    End If
                    If pv.db_locationNum <> "" AndAlso IsNumeric(pv.db_locationNum) = True Then
                        sql.inputParameters.Add(New SqlParameter("@locationNum", CInt(pv.db_locationNum)))
                    End If
                    If qqHelper.IsValidQuickQuoteIdOrNum(pv.db_buildingNum) = True Then 'added 7/28/2015 for Farm
                        sql.inputParameters.Add(New SqlParameter("@buildingNum", CInt(pv.db_buildingNum)))
                    End If
                    If pv.db_agencyCode <> "" Then 'added 8/15/2014
                        sql.inputParameters.Add(New SqlParameter("@agencyCode", pv.db_agencyCode))
                    End If
                    If pv.db_propertyValuationVendorId <> "" AndAlso IsNumeric(pv.db_propertyValuationVendorId) = True Then
                        sql.inputParameters.Add(New SqlParameter("@propertyValuationVendorId", CInt(pv.db_propertyValuationVendorId)))
                    End If
                    If pv.db_propertyValuationVendorIntegrationTypeId <> "" AndAlso IsNumeric(pv.db_propertyValuationVendorIntegrationTypeId) = True Then
                        sql.inputParameters.Add(New SqlParameter("@propertyValuationVendorIntegrationTypeId", CInt(pv.db_propertyValuationVendorIntegrationTypeId)))
                    End If
                    If pv.db_propertyValuationVendorEstimatorTypeId <> "" AndAlso IsNumeric(pv.db_propertyValuationVendorEstimatorTypeId) = True Then 'added 8/11/2014
                        sql.inputParameters.Add(New SqlParameter("@propertyValuationVendorEstimatorTypeId", CInt(pv.db_propertyValuationVendorEstimatorTypeId)))
                    End If
                    If pv.IFM_UniqueValuationId <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@IFM_UniqueValuationId", pv.IFM_UniqueValuationId))
                    End If
                    If pv.VendorValuationId <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@VendorValuationId", pv.VendorValuationId))
                    End If
                    If pv.db_propertyValuationRequestId <> "" AndAlso IsNumeric(pv.db_propertyValuationRequestId) = True Then
                        sql.inputParameters.Add(New SqlParameter("@propertyValuationRequestId", CInt(pv.db_propertyValuationRequestId)))
                    End If
                    If pv.db_propertyValuationResponseId <> "" AndAlso IsNumeric(pv.db_propertyValuationResponseId) = True Then
                        sql.inputParameters.Add(New SqlParameter("@propertyValuationResponseId", CInt(pv.db_propertyValuationResponseId)))
                    End If

                    If pv.db_policyId <> "" AndAlso IsNumeric(pv.db_policyId) Then
                        sql.inputParameters.Add(New SqlParameter("@policyId", pv.db_policyId))
                    End If
                    If pv.db_policyImageNum <> "" AndAlso IsNumeric(pv.db_policyImageNum) Then
                        sql.inputParameters.Add(New SqlParameter("@policyImageNum", CInt(pv.db_policyImageNum)))

                    End If

                    sql.outputParameter = New SqlParameter("@propertyValuationId", Data.SqlDbType.Int)

                    sql.ExecuteStatement()

                    'If sql.rowsAffected = 0 OrElse sql.hasError = True Then
                    If sql.hasError = True Then
                        'error
                        errorMsg = "error inserting property valuation into database"
                    Else
                        pv.db_propertyValuationId = sql.outputParameter.Value.ToString
                        '8/8/2014 note: stored procedure also goes back and updates IFM_UniqueValuationId if needed... replaces the text '*propertyValuationId*' w/ @propertyValuationId
                        If pv.IFM_UniqueValuationId.Contains("*propertyValuationId*") = True Then
                            pv.IFM_UniqueValuationId = pv.IFM_UniqueValuationId.Replace("*propertyValuationId*", pv.db_propertyValuationId)
                        End If
                        'added 8/20/2014 pm
                        pv.db_inserted = Date.Now.ToString
                        pv.db_updated = pv.db_inserted
                    End If

                    '@quoteId int = NULL, 
                    '@locationNum int = NULL, 
                    '@propertyValuationVendorId int = NULL, 
                    '@propertyValuationVendorIntegrationTypeId int = NULL, 
                    '@IFM_UniqueValuationId varchar(100) = NULL, 
                    '@VendorValuationId varchar(100) = NULL, 
                    '@propertyValuationRequestId int = NULL, 
                    '@propertyValuationResponseId int = NULL, 
                    '@propertyValuationId int output
                End Using
            Else
                errorMsg = "property valuation cannot be nothing"
            End If
        End Sub
        Public Sub InsertPropertyValuationRequest(ByRef pvr As QuickQuotePropertyValuationRequest, Optional ByRef errorMsg As String = "")
            If pvr IsNot Nothing Then
                Using sql As New SQLexecuteObject(ConfigurationManager.AppSettings("connQQ"))
                    sql.queryOrStoredProc = "usp_Insert_PropertyValuationRequest"
                    sql.inputParameters = New ArrayList
                    If (pvr.db_propertyValuationId = "" OrElse IsNumeric(pvr.db_propertyValuationId) = False) AndAlso pvr.IFM_UniqueValuationId <> "" Then 'added 8/20/2014
                        Dim quoteId As String = ""
                        Dim locNum As String = ""
                        Dim buildNum As String = "" 'added 7/28/2015
                        Dim policyId As String = ""
                        Dim policyImageNum As String = ""
                        Dim propertyValuationId As String = ""
                        'SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, propertyValuationId)
                        'updated 7/28/2015 for buildingNum
                        SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, buildNum, propertyValuationId, policyId, policyImageNum)
                        If propertyValuationId <> "" AndAlso IsNumeric(propertyValuationId) = True Then
                            pvr.db_propertyValuationId = propertyValuationId
                        End If
                    End If
                    If pvr.db_propertyValuationId <> "" AndAlso IsNumeric(pvr.db_propertyValuationId) = True Then
                        If pvr.IFM_UniqueValuationId.Contains("*propertyValuationId*") = True Then 'added 8/11/2014... moved from below... also updated in stored proc if needed
                            pvr.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId.Replace("*propertyValuationId*", pvr.db_propertyValuationId)
                        End If
                        sql.inputParameters.Add(New SqlParameter("@propertyValuationId", CInt(pvr.db_propertyValuationId)))
                    End If
                    If pvr.AuthId <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@AuthId", pvr.AuthId))
                    End If
                    If pvr.AuthCode <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@AuthCode", pvr.AuthCode))
                    End If
                    If pvr.IFM_UniqueValuationId <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@IFM_UniqueValuationId", pvr.IFM_UniqueValuationId))
                    End If
                    If pvr.VendorValuationId <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@VendorValuationId", pvr.VendorValuationId))
                    End If
                    sql.inputParameters.Add(New SqlParameter("@ClientIsBusiness", qqHelper.BooleanToInt(pvr.ClientIsBusiness)))
                    If pvr.ClientFirstName <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ClientFirstName", pvr.ClientFirstName))
                    End If
                    If pvr.ClientMiddleInitial <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ClientMiddleInitial", pvr.ClientMiddleInitial))
                    End If
                    If pvr.ClientLastName <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ClientLastName", pvr.ClientLastName))
                    End If
                    If pvr.ClientAddress1 <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ClientAddress1", pvr.ClientAddress1))
                    End If
                    If pvr.ClientAddress2 <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ClientAddress2", pvr.ClientAddress2))
                    End If
                    If pvr.ClientCity <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ClientCity", pvr.ClientCity))
                    End If
                    If pvr.ClientState <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ClientState", pvr.ClientState))
                    End If
                    If pvr.ClientZip <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ClientZip", pvr.ClientZip))
                    End If
                    If pvr.ReturnUrl <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ReturnUrl", pvr.ReturnUrl))
                    End If
                    If pvr.ReturnUrlLinkText <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ReturnUrlLinkText", pvr.ReturnUrlLinkText))
                    End If
                    If pvr.YearBuilt <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@YearBuilt", pvr.YearBuilt))
                    End If
                    sql.inputParameters.Add(New SqlParameter("@ReturnYearBuilt", qqHelper.BooleanToInt(pvr.ReturnYearBuilt)))
                    If pvr.ConstructionType <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ConstructionType", pvr.ConstructionType))
                    End If
                    sql.inputParameters.Add(New SqlParameter("@ReturnConstructionType", qqHelper.BooleanToInt(pvr.ReturnConstructionType)))
                    If pvr.SquareFeet <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@SquareFeet", pvr.SquareFeet))
                    End If
                    sql.inputParameters.Add(New SqlParameter("@ReturnSquareFeet", qqHelper.BooleanToInt(pvr.ReturnSquareFeet)))
                    If pvr.RoofType <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@RoofType", pvr.RoofType))
                    End If
                    sql.inputParameters.Add(New SqlParameter("@ReturnRoofType", qqHelper.BooleanToInt(pvr.ReturnRoofType)))
                    If pvr.ArchitecturalStyle <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ArchitecturalStyle", pvr.ArchitecturalStyle))
                    End If
                    sql.inputParameters.Add(New SqlParameter("@ReturnArchitecturalStyle", qqHelper.BooleanToInt(pvr.ReturnArchitecturalStyle)))
                    If pvr.ConstructionQuality <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ConstructionQuality", pvr.ConstructionQuality))
                    End If
                    sql.inputParameters.Add(New SqlParameter("@ReturnConstructionQuality", qqHelper.BooleanToInt(pvr.ReturnConstructionQuality)))
                    If pvr.PhysicalShape <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@PhysicalShape", pvr.PhysicalShape))
                    End If
                    sql.inputParameters.Add(New SqlParameter("@ReturnPhysicalShape", qqHelper.BooleanToInt(pvr.ReturnPhysicalShape)))
                    If pvr.PrimaryExterior <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@PrimaryExterior", pvr.PrimaryExterior))
                    End If
                    sql.inputParameters.Add(New SqlParameter("@ReturnPrimaryExterior", qqHelper.BooleanToInt(pvr.ReturnPrimaryExterior)))
                    sql.inputParameters.Add(New SqlParameter("@ReturnUrlNoPopup", qqHelper.BooleanToInt(pvr.ReturnUrlNoPopup)))
                    sql.inputParameters.Add(New SqlParameter("@ReturnReplacementCostType", qqHelper.BooleanToInt(pvr.ReturnReplacementCostType)))
                    sql.inputParameters.Add(New SqlParameter("@ReturnAdditionalAreas", qqHelper.BooleanToInt(pvr.ReturnAdditionalAreas)))
                    If pvr.VendorParams <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@VendorParams", pvr.VendorParams))
                    End If
                    sql.inputParameters.Add(New SqlParameter("@sentToVendor", qqHelper.BooleanToInt(pvr.db_sentToVendor))) 'added 8/19/2014

                    sql.outputParameter = New SqlParameter("@propertyValuationRequestId", Data.SqlDbType.Int)

                    sql.ExecuteStatement()

                    'If sql.rowsAffected = 0 OrElse sql.hasError = True Then
                    If sql.hasError = True Then
                        'error
                        errorMsg = "error inserting property valuation request into database"
                    Else
                        pvr.db_propertyValuationRequestId = sql.outputParameter.Value.ToString
                        '8/8/2014 note: stored procedure also updates @IFM_UniqueValuationId if needed... replaces the text '*propertyValuationId*' w/ @propertyValuationId
                        'If pvr.IFM_UniqueValuationId.Contains("*propertyValuationId*") = True Then
                        '    pvr.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId.Replace("*propertyValuationId*", pvr.db_propertyValuationId)
                        'End If
                        '8/8/2014 note: stored procedure also goes back and updates propertyValuationRequestId, propertyValuationResponseId (NULL), and updated fields in PropertyValuation table
                        'added 8/20/2014 pm
                        pvr.db_inserted = Date.Now.ToString
                        pvr.db_updated = pvr.db_inserted
                    End If

                    '@propertyValuationId int = NULL, 
                    '@AuthId varchar(100) = NULL, 
                    '@AuthCode varchar(200) = NULL, 
                    '@IFM_UniqueValuationId varchar(100) = NULL, 
                    '@VendorValuationId varchar(100) = NULL, 
                    '@ClientIsBusiness bit = NULL, 
                    '@ClientFirstName varchar(100) = NULL, 
                    '@ClientMiddleInitial varchar(1) = NULL, 
                    '@ClientLastName varchar(100) = NULL, 
                    '@ClientAddress1 varchar(150) = NULL, 
                    '@ClientAddress2 varchar(150) = NULL, 
                    '@ClientCity varchar(100) = NULL, 
                    '@ClientState varchar(2) = NULL, 
                    '@ClientZip varchar(5) = NULL, 
                    '@ReturnUrl varchar(200) = NULL, 
                    '@ReturnUrlLinkText varchar(200) = NULL, 
                    '@YearBuilt varchar(4) = NULL, 
                    '@ReturnYearBuilt bit = NULL, 
                    '@ConstructionType varchar(200) = NULL, 
                    '@ReturnConstructionType bit = NULL, 
                    '@SquareFeet varchar(20) = NULL, 
                    '@ReturnSquareFeet bit = NULL, 
                    '@RoofType varchar(200) = NULL, 
                    '@ReturnRoofType bit = NULL, 
                    '@ArchitecturalStyle varchar(200) = NULL, 
                    '@ReturnArchitecturalStyle bit = NULL, 
                    '@ConstructionQuality varchar(200) = NULL, 
                    '@ReturnConstructionQuality bit = NULL, 
                    '@PhysicalShape varchar(200) = NULL, 
                    '@ReturnPhysicalShape bit = NULL, 
                    '@PrimaryExterior varchar(200) = NULL, 
                    '@ReturnPrimaryExterior bit = NULL, 
                    '@ReturnUrlNoPopup bit = NULL, 
                    '@ReturnReplacementCostType bit = NULL, 
                    '@ReturnAdditionalAreas bit = NULL, 
                    '@VendorParams varchar(8000) = NULL, 
                    '@propertyValuationRequestId int output
                End Using
            Else
                errorMsg = "property valuation request cannot be nothing"
            End If
        End Sub
        'added 8/19/2014
        Public Sub UpdatePropertyValuationRequest(ByRef pvr As QuickQuotePropertyValuationRequest, Optional ByRef errorMsg As String = "")
            If pvr IsNot Nothing Then
                If pvr.db_propertyValuationRequestId <> "" AndAlso IsNumeric(pvr.db_propertyValuationRequestId) = True Then
                    Using sql As New SQLexecuteObject(ConfigurationManager.AppSettings("connQQ"))
                        sql.queryOrStoredProc = "usp_Update_PropertyValuationRequest"
                        sql.inputParameters = New ArrayList
                        sql.inputParameters.Add(New SqlParameter("@propertyValuationRequestId", CInt(pvr.db_propertyValuationRequestId)))
                        If (pvr.db_propertyValuationId = "" OrElse IsNumeric(pvr.db_propertyValuationId) = False) AndAlso pvr.IFM_UniqueValuationId <> "" Then 'added 8/20/2014
                            Dim quoteId As String = ""
                            Dim locNum As String = ""
                            Dim buildNum As String = "" 'added 7/28/2015
                            Dim policyId As String = ""
                            Dim policyImageNum As String = ""
                            Dim propertyValuationId As String = ""
                            'SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, propertyValuationId)
                            'updated 7/28/2015 for buildingNum
                            SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, buildNum, propertyValuationId, policyId, policyImageNum)
                            If propertyValuationId <> "" AndAlso IsNumeric(propertyValuationId) = True Then
                                pvr.db_propertyValuationId = propertyValuationId
                            End If
                        End If
                        If pvr.db_propertyValuationId <> "" AndAlso IsNumeric(pvr.db_propertyValuationId) = True Then
                            If pvr.IFM_UniqueValuationId.Contains("*propertyValuationId*") = True Then 'added 8/11/2014... moved from below... also updated in stored proc if needed
                                pvr.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId.Replace("*propertyValuationId*", pvr.db_propertyValuationId)
                            End If
                            sql.inputParameters.Add(New SqlParameter("@propertyValuationId", CInt(pvr.db_propertyValuationId)))
                        End If
                        If pvr.AuthId <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@AuthId", pvr.AuthId))
                        End If
                        If pvr.AuthCode <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@AuthCode", pvr.AuthCode))
                        End If
                        If pvr.IFM_UniqueValuationId <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@IFM_UniqueValuationId", pvr.IFM_UniqueValuationId))
                        End If
                        If pvr.VendorValuationId <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@VendorValuationId", pvr.VendorValuationId))
                        End If
                        sql.inputParameters.Add(New SqlParameter("@ClientIsBusiness", qqHelper.BooleanToInt(pvr.ClientIsBusiness)))
                        If pvr.ClientFirstName <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ClientFirstName", pvr.ClientFirstName))
                        End If
                        If pvr.ClientMiddleInitial <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ClientMiddleInitial", pvr.ClientMiddleInitial))
                        End If
                        If pvr.ClientLastName <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ClientLastName", pvr.ClientLastName))
                        End If
                        If pvr.ClientAddress1 <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ClientAddress1", pvr.ClientAddress1))
                        End If
                        If pvr.ClientAddress2 <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ClientAddress2", pvr.ClientAddress2))
                        End If
                        If pvr.ClientCity <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ClientCity", pvr.ClientCity))
                        End If
                        If pvr.ClientState <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ClientState", pvr.ClientState))
                        End If
                        If pvr.ClientZip <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ClientZip", pvr.ClientZip))
                        End If
                        If pvr.ReturnUrl <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ReturnUrl", pvr.ReturnUrl))
                        End If
                        If pvr.ReturnUrlLinkText <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ReturnUrlLinkText", pvr.ReturnUrlLinkText))
                        End If
                        If pvr.YearBuilt <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@YearBuilt", pvr.YearBuilt))
                        End If
                        sql.inputParameters.Add(New SqlParameter("@ReturnYearBuilt", qqHelper.BooleanToInt(pvr.ReturnYearBuilt)))
                        If pvr.ConstructionType <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ConstructionType", pvr.ConstructionType))
                        End If
                        sql.inputParameters.Add(New SqlParameter("@ReturnConstructionType", qqHelper.BooleanToInt(pvr.ReturnConstructionType)))
                        If pvr.SquareFeet <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@SquareFeet", pvr.SquareFeet))
                        End If
                        sql.inputParameters.Add(New SqlParameter("@ReturnSquareFeet", qqHelper.BooleanToInt(pvr.ReturnSquareFeet)))
                        If pvr.RoofType <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@RoofType", pvr.RoofType))
                        End If
                        sql.inputParameters.Add(New SqlParameter("@ReturnRoofType", qqHelper.BooleanToInt(pvr.ReturnRoofType)))
                        If pvr.ArchitecturalStyle <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ArchitecturalStyle", pvr.ArchitecturalStyle))
                        End If
                        sql.inputParameters.Add(New SqlParameter("@ReturnArchitecturalStyle", qqHelper.BooleanToInt(pvr.ReturnArchitecturalStyle)))
                        If pvr.ConstructionQuality <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@ConstructionQuality", pvr.ConstructionQuality))
                        End If
                        sql.inputParameters.Add(New SqlParameter("@ReturnConstructionQuality", qqHelper.BooleanToInt(pvr.ReturnConstructionQuality)))
                        If pvr.PhysicalShape <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@PhysicalShape", pvr.PhysicalShape))
                        End If
                        sql.inputParameters.Add(New SqlParameter("@ReturnPhysicalShape", qqHelper.BooleanToInt(pvr.ReturnPhysicalShape)))
                        If pvr.PrimaryExterior <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@PrimaryExterior", pvr.PrimaryExterior))
                        End If
                        sql.inputParameters.Add(New SqlParameter("@ReturnPrimaryExterior", qqHelper.BooleanToInt(pvr.ReturnPrimaryExterior)))
                        sql.inputParameters.Add(New SqlParameter("@ReturnUrlNoPopup", qqHelper.BooleanToInt(pvr.ReturnUrlNoPopup)))
                        sql.inputParameters.Add(New SqlParameter("@ReturnReplacementCostType", qqHelper.BooleanToInt(pvr.ReturnReplacementCostType)))
                        sql.inputParameters.Add(New SqlParameter("@ReturnAdditionalAreas", qqHelper.BooleanToInt(pvr.ReturnAdditionalAreas)))
                        If pvr.VendorParams <> "" Then
                            sql.inputParameters.Add(New SqlParameter("@VendorParams", pvr.VendorParams))
                        End If
                        sql.inputParameters.Add(New SqlParameter("@sentToVendor", qqHelper.BooleanToInt(pvr.db_sentToVendor))) 'added 8/19/2014

                        sql.ExecuteStatement()

                        'If sql.rowsAffected = 0 OrElse sql.hasError = True Then
                        If sql.hasError = True Then
                            'error
                            errorMsg = "error updating property valuation request in database"
                        Else
                            pvr.db_propertyValuationRequestId = sql.outputParameter.Value.ToString
                            '8/8/2014 note: stored procedure also updates @IFM_UniqueValuationId if needed... replaces the text '*propertyValuationId*' w/ @propertyValuationId
                            'If pvr.IFM_UniqueValuationId.Contains("*propertyValuationId*") = True Then
                            '    pvr.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId.Replace("*propertyValuationId*", pvr.db_propertyValuationId)
                            'End If
                            '8/8/2014 note: stored procedure also goes back and updates propertyValuationRequestId, propertyValuationResponseId (NULL), and updated fields in PropertyValuation table
                            'added 8/20/2014 pm
                            pvr.db_updated = Date.Now.ToString
                        End If

                    End Using
                Else
                    errorMsg = "numeric propertyValuationRequestId required"
                End If
            Else
                errorMsg = "property valuation request cannot be nothing"
            End If
        End Sub
        Public Sub InsertPropertyValuationResponse(ByRef pvr As QuickQuotePropertyValuationResponse, Optional ByRef errorMsg As String = "")
            If pvr IsNot Nothing Then
                Using sql As New SQLexecuteObject(ConfigurationManager.AppSettings("connQQ"))
                    sql.queryOrStoredProc = "usp_Insert_PropertyValuationResponse"
                    sql.inputParameters = New ArrayList
                    If (pvr.db_propertyValuationId = "" OrElse IsNumeric(pvr.db_propertyValuationId) = False) AndAlso pvr.IFM_UniqueValuationId <> "" Then 'added 8/20/2014
                        Dim quoteId As String = ""
                        Dim locNum As String = ""
                        Dim buildNum As String = "" 'added 7/28/2015
                        Dim policyId As String = ""
                        Dim policyImageNum As String = ""
                        Dim propertyValuationId As String = ""
                        'SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, propertyValuationId)
                        'updated 7/28/2015 for buildingNum
                        SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, buildNum, propertyValuationId, policyId, policyImageNum)
                        If propertyValuationId <> "" AndAlso IsNumeric(propertyValuationId) = True Then
                            pvr.db_propertyValuationId = propertyValuationId
                        End If
                    End If
                    If pvr.db_propertyValuationId <> "" AndAlso IsNumeric(pvr.db_propertyValuationId) = True Then
                        sql.inputParameters.Add(New SqlParameter("@propertyValuationId", CInt(pvr.db_propertyValuationId)))
                    End If
                    If pvr.AuthId <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@AuthId", pvr.AuthId))
                    End If
                    If pvr.IFM_UniqueValuationId <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@IFM_UniqueValuationId", pvr.IFM_UniqueValuationId))
                    End If
                    If pvr.VendorValuationId <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@VendorValuationId", pvr.VendorValuationId))
                    End If
                    If pvr.YearBuilt <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@YearBuilt", pvr.YearBuilt))
                    End If
                    If pvr.ConstructionType <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ConstructionType", pvr.ConstructionType))
                    End If
                    If pvr.SquareFeet <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@SquareFeet", pvr.SquareFeet))
                    End If
                    If pvr.RoofType <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@RoofType", pvr.RoofType))
                    End If
                    If pvr.ArchitecturalStyle <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ArchitecturalStyle", pvr.ArchitecturalStyle))
                    End If
                    If pvr.ConstructionQuality <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ConstructionQuality", pvr.ConstructionQuality))
                    End If
                    If pvr.PhysicalShape <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@PhysicalShape", pvr.PhysicalShape))
                    End If
                    If pvr.PrimaryExterior <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@PrimaryExterior", pvr.PrimaryExterior))
                    End If
                    If pvr.ReplacementCostValue <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ReplacementCostValue", pvr.ReplacementCostValue))
                    End If
                    If pvr.ReplacementCostType <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@ReplacementCostType", pvr.ReplacementCostType))
                    End If
                    If pvr.AdditionalAreas <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@AdditionalAreas", pvr.AdditionalAreas))
                    End If
                    If pvr.StructuresReplacementCostValue <> "" Then 'added 7/29/2015 for Farm
                        sql.inputParameters.Add(New SqlParameter("@StructuresReplacementCostValue", pvr.StructuresReplacementCostValue))
                    End If
                    If pvr.StructuresReplacementCostValueTotal <> "" Then 'added 7/29/2015 for Farm
                        sql.inputParameters.Add(New SqlParameter("@StructuresReplacementCostValueTotal", pvr.StructuresReplacementCostValueTotal))
                    End If
                    If pvr.UniqueItemsTotalValue <> "" Then 'added 7/29/2015 for Farm
                        sql.inputParameters.Add(New SqlParameter("@UniqueItemsTotalValue", pvr.UniqueItemsTotalValue))
                    End If
                    If pvr.ActualCashValue <> "" Then 'added 7/29/2015 for Farm
                        sql.inputParameters.Add(New SqlParameter("@ActualCashValue", pvr.ActualCashValue))
                    End If
                    If pvr.VendorParams <> "" Then
                        sql.inputParameters.Add(New SqlParameter("@VendorParams", pvr.VendorParams))
                    End If
                    sql.inputParameters.Add(New SqlParameter("@loadedBackIntoQuote", qqHelper.BooleanToInt(pvr.db_loadedBackIntoQuote)))

                    sql.outputParameter = New SqlParameter("@propertyValuationResponseId", Data.SqlDbType.Int)

                    sql.ExecuteStatement()

                    'If sql.rowsAffected = 0 OrElse sql.hasError = True Then
                    If sql.hasError = True Then
                        'error
                        errorMsg = "error inserting property valuation response into database"
                    Else
                        pvr.db_propertyValuationResponseId = sql.outputParameter.Value.ToString
                        '8/8/2014 note: stored procedure also goes back and updates propertyValuationResponseId, VendorValuationId, and updated fields in PropertyValuation table
                        'added 8/20/2014 pm
                        pvr.db_inserted = Date.Now.ToString
                        pvr.db_updated = pvr.db_inserted
                    End If

                    '@propertyValuationId int = NULL, 
                    '@AuthId varchar(100) = NULL, 
                    '@IFM_UniqueValuationId varchar(100) = NULL, 
                    '@VendorValuationId varchar(100) = NULL, 
                    '@YearBuilt varchar(4) = NULL, 
                    '@ConstructionType varchar(200) = NULL, 
                    '@SquareFeet varchar(20) = NULL, 
                    '@RoofType varchar(200) = NULL, 
                    '@ArchitecturalStyle varchar(200) = NULL, 
                    '@ConstructionQuality varchar(200) = NULL, 
                    '@PhysicalShape varchar(200) = NULL, 
                    '@PrimaryExterior varchar(200) = NULL, 
                    '@ReplacementCostValue varchar(50) = NULL, 
                    '@ReplacementCostType varchar(100) = NULL, 
                    '@AdditionalAreas varchar(2000) = NULL, 
                    '@VendorParams varchar(8000) = NULL, 
                    '@loadedBackIntoQuote bit = NULL, 
                    '@propertyValuationResponseId int output
                End Using
            Else
                errorMsg = "property valuation response cannot be nothing"
            End If
        End Sub
        'added 8/8/2014
        Public Sub UpdatePropertyValuationResponse_LoadedBackIntoQuote(ByVal pvr As QuickQuotePropertyValuationResponse, Optional ByRef errorMsg As String = "")
            If pvr IsNot Nothing Then
                UpdatePropertyValuationResponse_LoadedBackIntoQuote(pvr.db_propertyValuationResponseId, pvr.db_loadedBackIntoQuote, errorMsg)
            Else
                errorMsg = "property valuation response cannot be nothing"
            End If
        End Sub
        Public Sub UpdatePropertyValuationResponse_LoadedBackIntoQuote(ByVal propertyValuationResponseId As String, ByVal loadedBackIntoQuote As Boolean, Optional ByRef errorMsg As String = "")
            If propertyValuationResponseId <> "" AndAlso IsNumeric(propertyValuationResponseId) = True Then
                Using sql As New SQLexecuteObject(ConfigurationManager.AppSettings("connQQ"))
                    sql.queryOrStoredProc = "usp_Update_PropertyValuationResponse_LoadedBackIntoQuote"
                    sql.inputParameters = New ArrayList
                    'If propertyValuationResponseId <> "" AndAlso IsNumeric(propertyValuationResponseId) = True Then
                    sql.inputParameters.Add(New SqlParameter("@propertyValuationResponseId", CInt(propertyValuationResponseId)))
                    'End If
                    sql.inputParameters.Add(New SqlParameter("@loadedBackIntoQuote", qqHelper.BooleanToInt(loadedBackIntoQuote)))

                    sql.ExecuteStatement()

                    'If sql.rowsAffected = 0 OrElse sql.hasError = True Then
                    If sql.hasError = True Then
                        'error
                        errorMsg = "error inserting property valuation response into database"
                    Else
                        '8/8/2014 note: stored procedure also goes back and updates updated field in PropertyValuation table for record matching propertyValuationResponseId
                    End If

                    '@propertyValuationResponseId int = NULL, 
                    '@loadedBackIntoQuote bit = NULL, 
                    '@updated datetime = NULL
                End Using
            Else
                errorMsg = "property valuation response id must be numeric"
            End If
        End Sub
        'added 8/13/2014
        Public Sub SetE2ValueAuthIdAndCodeForQuote(ByVal qq As QuickQuoteObject, ByRef authId As String, ByRef authCode As String, Optional ByRef errorMsg As String = "", Optional ByVal checkForStaff As Boolean = False, Optional ByVal onlySetVirtualUsernameFromQuoteAgency As Boolean = True)
            authId = ""
            authCode = ""

            Dim vUsername As String = ""
            Dim username As String = ""
            Dim password As String = ""
            Dim isStaff As Boolean = False

            If checkForStaff = True Then
                'shouldn't use this unless creating a valuation that an agency would never need to see
                isStaff = qqHelper.IsHomeOfficeStaffUser
            End If

            If qq IsNot Nothing AndAlso qq.AgencyCode <> "" Then
                Dim agCode As String = qq.AgencyCode
                If agCode.Length > 4 Then
                    agCode = Right(agCode, 4)
                End If
                vUsername = agCode
            Else
                If onlySetVirtualUsernameFromQuoteAgency = True Then
                    errorMsg = "unable to locate agency code for quote"
                Else
                    Dim useLoggedInAgencyCode As Boolean = False
                    If isStaff = True Then
                        vUsername = helper.LoggedInUsername
                        If vUsername = "" Then
                            useLoggedInAgencyCode = True
                        End If
                    Else
                        useLoggedInAgencyCode = True
                    End If
                    If useLoggedInAgencyCode = True Then
                        vUsername = helper.LoggedInAgencyCode
                        If vUsername <> "" AndAlso vUsername.Length > 4 Then
                            vUsername = Right(vUsername, 4)
                        End If
                    End If
                End If
            End If

            If vUsername <> "" Then
                If helper.IsTestEnvironment = True Then
                    vUsername = "TEST_" & vUsername
                End If

                SetE2ValueUsernameAndPassword(username, password, isStaff)
                If username <> "" AndAlso password <> "" Then
                    SetE2ValueAuthIdAndCode(username, password, vUsername, authId, authCode, errorMsg)
                Else
                    errorMsg = "unable to determine username/password"
                End If
            Else
                If errorMsg = "" Then
                    errorMsg = "unable to determine virtual username"
                End If
            End If
        End Sub
        Public Sub SetE2ValueUsernameAndPassword(ByRef username As String, ByRef password As String, Optional ByVal isStaff As Boolean = False)
            username = ""
            password = ""

            If isStaff = True Then
                username = helper.configAppSettingValueAsString("e2Value_staffusername")
                password = helper.configAppSettingValueAsString("e2Value_staffpassword")
            End If
            If username = "" OrElse password = "" Then
                username = helper.configAppSettingValueAsString("e2Value_agentsusername")
                password = helper.configAppSettingValueAsString("e2Value_agentspassword")
            End If
        End Sub
        Public Sub SetE2ValueAuthIdAndCode(ByVal username As String, ByVal password As String, ByVal virtualUsername As String, ByRef authId As String, ByRef authCode As String, Optional ByRef errorMsg As String = "")
            authId = ""
            authCode = ""

            If username <> "" AndAlso password <> "" AndAlso virtualUsername <> "" Then
                'Dim e2ValueUrl As String = "https://evs.e2value.com/evs/remote_auth.asp?username=" & helper.UrlEncodedValue(username) & "&password=" & helper.UrlEncodedValue(password) & "&vusername=" & helper.UrlEncodedValue(virtualUsername)
                'updated 11/30/2021
                Dim e2ValueUrl As String = String.Format(E2Value_Formatted_AuthUrl(), helper.UrlEncodedValue(username), helper.UrlEncodedValue(password), helper.UrlEncodedValue(virtualUsername))
                'should be able to do w/ HttpWebRequest and GET or load xml document w/ URL
                'Dim xmlDoc As New XmlDocument()
                Set_e2Value_ServicePoint_SecurityProtocol() '8-10-2018
                'Try
                '    xmlDoc.Load(e2ValueUrl) 'same as when using a filename
                'Catch ex As Exception
                '    errorMsg = "problem hitting e2Value auth site"
                'End Try
                'updated 11/30/2021
                Dim xmlDoc As XmlDocument = XmlDocumentForE2ValueUrl(e2ValueUrl, errorMsg:=errorMsg)
                If xmlDoc IsNot Nothing AndAlso errorMsg = "" Then
                    Dim status As String = ""

                    Dim ResponseSection As XmlNode
                    ResponseSection = xmlDoc.SelectSingleNode("/response")

                    If ResponseSection Is Nothing Then
                        ResponseSection = xmlDoc.Item("response")
                    End If

                    If ResponseSection Is Nothing Then
                        ResponseSection = xmlDoc.GetElementsByTagName("response").Item(0)
                    End If

                    If ResponseSection IsNot Nothing Then
                        If ResponseSection.Attributes IsNot Nothing AndAlso ResponseSection.Attributes.Count > 0 Then
                            For Each att As XmlAttribute In ResponseSection.Attributes
                                Select Case UCase(att.Name)
                                    Case UCase("status") 'should be success or failure
                                        status = att.Value
                                End Select
                            Next
                        End If
                        If ResponseSection.HasChildNodes = True Then 'could also check w/ ResponseSection.ChildNodes IsNot Nothing AndAlso ResponseSection.ChildNodes.Count > 0
                            For Each node As XmlNode In ResponseSection.ChildNodes
                                Select Case node.NodeType
                                    Case XmlNodeType.Element
                                        Select Case UCase(node.Name)
                                            Case UCase("authid")
                                                authId = node.InnerText 'could possibly use Node.Value for the same thing
                                            Case UCase("authcode")
                                                authCode = node.InnerText 'could possibly use Node.Value for the same thing
                                            Case UCase("message") 'should only have something if status is failure
                                                errorMsg = node.InnerText 'could possibly use Node.Value for the same thing
                                        End Select
                                End Select
                            Next
                        End If
                    Else
                        errorMsg = "unable to locate response node in e2Value auth response"
                    End If
                    If (authId = "" OrElse authCode = "") AndAlso errorMsg = "" Then
                        errorMsg = "authid or authcode was missing from e2Value auth response"
                    End If
                End If
            Else
                errorMsg = "username, password, and virtual username are required"
            End If
        End Sub

        'added 11/30/2021
        Public Shared Function E2Value_Formatted_AuthUrl() As String
            Dim url As String = ""

            url = QuickQuoteHelperClass.configAppSettingValueAsString("e2Value_loginURL")
            If String.IsNullOrWhiteSpace(url) = True Then
                url = "https://evs.e2value.com/evs/remote_auth.asp?username={0}&password={1}&vusername={2}"
                'note: "https://evstest.e2value.com/evs/remote_auth.asp?username={0}&password={1}&vusername={2}" can be used to test things not in Prod yet
            End If

            Return url
        End Function
        Public Enum GetOrPostType
            None = 0
            SimpleGet = 1
            SimplePost = 2
            PostApplicationForUrlEncoded = 3
        End Enum
        Public Shared Function GetOrPostTypeForConfigKeyName(ByVal configKeyName As String, Optional ByVal defaultValueWhenKeyIsMissingOrInvalid As GetOrPostType = GetOrPostType.None) As GetOrPostType
            Dim getOrPost As GetOrPostType = defaultValueWhenKeyIsMissingOrInvalid 'default

            If String.IsNullOrWhiteSpace(configKeyName) = False Then
                Dim chc As New CommonHelperClass
                Dim strGetOrPostType As String = chc.ConfigurationAppSettingValueAsString(configKeyName)
                If String.IsNullOrWhiteSpace(strGetOrPostType) = False Then
                    Select Case UCase(strGetOrPostType)
                        Case "NONE"
                            getOrPost = GetOrPostType.None
                        Case UCase("SimpleGet")
                            getOrPost = GetOrPostType.SimpleGet
                        Case UCase("SimplePost")
                            getOrPost = GetOrPostType.SimplePost
                        Case UCase("PostApplicationForUrlEncoded")
                            getOrPost = GetOrPostType.PostApplicationForUrlEncoded
                        Case Else
                            If System.Enum.TryParse(Of GetOrPostType)(strGetOrPostType, getOrPost) = False Then
                                getOrPost = Nothing
                            End If
                    End Select
                End If

                If (System.Enum.IsDefined(GetType(GetOrPostType), getOrPost) = False OrElse getOrPost = GetOrPostType.None) AndAlso defaultValueWhenKeyIsMissingOrInvalid <> GetOrPostType.None Then
                    getOrPost = defaultValueWhenKeyIsMissingOrInvalid
                End If
            End If

            Return getOrPost
        End Function
        Private Function XmlDocumentForE2ValueUrl(ByVal authURL As String, Optional getOrPost As GetOrPostType = GetOrPostType.None, Optional ByRef errorMsg As String = "") As XmlDocument
            Dim xmlDoc As XmlDocument = Nothing

            If String.IsNullOrWhiteSpace(authURL) = False Then
                Dim chc As New CommonHelperClass

                If System.Enum.IsDefined(GetType(GetOrPostType), getOrPost) = False OrElse getOrPost = GetOrPostType.None Then
                    getOrPost = GetOrPostTypeForConfigKeyName("e2Value_auth_getOrPostType")
                End If

                If getOrPost = GetOrPostType.SimplePost OrElse getOrPost = GetOrPostType.PostApplicationForUrlEncoded Then
                    'Dim errorMsg As String = ""
                    Dim exceptionErrorMsg As String = ""
                    Dim errorType As CommonHelperClass.WebRequestErrorType = CommonHelperClass.WebRequestErrorType.None
                    Dim strResponse As String = ""
                    If getOrPost = GetOrPostType.PostApplicationForUrlEncoded Then
                        strResponse = ResponseStringFromPostedXmlString_ApplicationFormUrlencoded(authURL, "", allowEmptyStringPost:=True, errorMsg:=errorMsg, exceptionErrorMsg:=exceptionErrorMsg, errorType:=errorType)
                    Else
                        strResponse = chc.ResponseStringFromPostedXmlString(authURL, "", allowEmptyStringPost:=True, errorMsg:=errorMsg, exceptionErrorMsg:=exceptionErrorMsg, errorType:=errorType)
                    End If

                    If String.IsNullOrWhiteSpace(strResponse) = False Then
                        Try
                            xmlDoc = New XmlDocument
                            xmlDoc.LoadXml(strResponse)
                        Catch ex As Exception
                            errorMsg = "unhandled exception encountered while loading e2Value auth response"
                            exceptionErrorMsg = ex.ToString
                            errorType = CommonHelperClass.WebRequestErrorType.UnhandledExceptionLoadingResponseIntoXmlDocument 'added 12/19/2017
                        End Try
                    End If
                    If xmlDoc Is Nothing OrElse String.IsNullOrWhiteSpace(errorMsg) = False Then
                        errorMsg = "problem hitting e2Value auth site"
                    End If
                Else
                    Try
                        xmlDoc = New XmlDocument
                        xmlDoc.Load(authURL) 'same as when using a filename
                    Catch ex As Exception
                        errorMsg = "problem hitting e2Value auth site"
                    End Try
                End If
            End If

            Return xmlDoc
        End Function
        Public Shared Function E2Value_auth_postUrlQuerystringWhenApplicable() As Boolean
            Dim isOkay As Boolean = True 'default to True; key required to turn to False

            Dim strIsOkay As String = QuickQuoteHelperClass.configAppSettingValueAsString("e2Value_auth_postUrlQuerystringWhenApplicable")
            If String.IsNullOrWhiteSpace(strIsOkay) = False Then
                Dim qqHelper As New QuickQuoteHelperClass
                If UCase(strIsOkay) = "YES" OrElse qqHelper.BitToBoolean(strIsOkay) = True Then 'key would just need any text value that doesn't equate to True to return False
                    isOkay = True
                Else
                    isOkay = False
                End If
            End If

            Return isOkay
        End Function
        'note: this is just like the one from CommonHelperClass
        'Public Function ResponseStringFromPostedXmlString(ByVal strUrl As String, ByVal strToPost As String, Optional ByVal allowEmptyStringPost As Boolean = True, Optional ByRef errorMsg As String = "", Optional ByRef exceptionErrorMsg As String = "", Optional ByRef errorType As CommonHelperClass.WebRequestErrorType = CommonHelperClass.WebRequestErrorType.None) As String
        '    Dim strResponse As String = ""
        '    errorMsg = ""
        '    exceptionErrorMsg = ""
        '    errorType = CommonHelperClass.WebRequestErrorType.None 'added 12/19/2017

        '    If String.IsNullOrEmpty(strUrl) = False Then
        '        If String.IsNullOrEmpty(strToPost) = False OrElse allowEmptyStringPost = True Then
        '            Dim req As Net.HttpWebRequest = Net.HttpWebRequest.Create(strUrl)
        '            If req IsNot Nothing Then
        '                Try
        '                    With req
        '                        .Method = "POST"
        '                        .ContentType = "text/xml"

        '                        Using writer As New IO.StreamWriter(.GetRequestStream)
        '                            With writer
        '                                .Write(strToPost)
        '                                .Flush()
        '                                .Close()
        '                            End With
        '                        End Using

        '                        'If .HaveResponse = True Then 'will remove for now; may need to be after req.GetResponse call in order to work correctly
        '                        Using resp As Net.HttpWebResponse = .GetResponse
        '                            If resp IsNot Nothing Then
        '                                With resp
        '                                    Using reader As New IO.StreamReader(.GetResponseStream)
        '                                        strResponse = reader.ReadToEnd
        '                                        reader.Close()
        '                                    End Using
        '                                End With
        '                            Else
        '                                errorMsg = "no HttpWebResponse returned"
        '                                errorType = CommonHelperClass.WebRequestErrorType.InvalidResponse 'added 12/19/2017
        '                            End If
        '                        End Using
        '                        'Else
        '                        '    errorMsg = "no HttpWebResponse returned"
        '                        'End If
        '                    End With
        '                Catch ex As Exception
        '                    errorMsg = "unhandled exception encountered while posting xml"
        '                    exceptionErrorMsg = ex.ToString
        '                    errorType = CommonHelperClass.WebRequestErrorType.UnhandledExceptionOnPost 'added 12/19/2017
        '                End Try
        '            Else
        '                errorMsg = "unable to create HttpWebRequest"
        '                errorType = CommonHelperClass.WebRequestErrorType.InvalidRequest 'added 12/19/2017
        '            End If
        '        Else
        '            errorMsg = "invalid xml post"
        '            errorType = CommonHelperClass.WebRequestErrorType.Validation 'added 12/19/2017
        '        End If
        '    Else
        '        errorMsg = "invalid URL"
        '        errorType = CommonHelperClass.WebRequestErrorType.Validation 'added 12/19/2017
        '    End If

        '    Return strResponse
        'End Function
        Public Function ResponseStringFromPostedXmlString_ApplicationFormUrlencoded(ByVal strUrl As String, ByVal strToPost As String, Optional ByVal allowEmptyStringPost As Boolean = True, Optional ByVal postUrlQuerystringWhenApplicable As QuickQuoteHelperClass.QuickQuoteYesNoMaybeType = QuickQuoteHelperClass.QuickQuoteYesNoMaybeType.Maybe, Optional ByRef errorMsg As String = "", Optional ByRef exceptionErrorMsg As String = "", Optional ByRef errorType As CommonHelperClass.WebRequestErrorType = CommonHelperClass.WebRequestErrorType.None) As String
            Dim strResponse As String = ""
            errorMsg = ""
            exceptionErrorMsg = ""
            errorType = CommonHelperClass.WebRequestErrorType.None 'added 12/19/2017

            If String.IsNullOrEmpty(strUrl) = False Then

                Dim postUrlQuerystringWhenApplicableBool As Boolean = False
                Select Case postUrlQuerystringWhenApplicable
                    Case helper.QuickQuoteYesNoMaybeType.Yes
                        postUrlQuerystringWhenApplicableBool = True
                    Case helper.QuickQuoteYesNoMaybeType.No
                        postUrlQuerystringWhenApplicableBool = False
                    Case Else
                        postUrlQuerystringWhenApplicableBool = E2Value_auth_postUrlQuerystringWhenApplicable()
                End Select

                If postUrlQuerystringWhenApplicableBool = True AndAlso strUrl.Contains("?") = True AndAlso String.IsNullOrWhiteSpace(strToPost) = True Then
                    Dim arUrl As Array = Split(strUrl, "?")
                    If arUrl IsNot Nothing AndAlso arUrl.Length = 2 Then
                        strUrl = arUrl(0).ToString.Trim
                        strToPost = arUrl(1).ToString.Trim
                    End If
                End If

                If String.IsNullOrEmpty(strToPost) = False OrElse allowEmptyStringPost = True Then

                    Dim req As Net.HttpWebRequest = Net.HttpWebRequest.Create(strUrl)
                    If req IsNot Nothing Then
                        Try
                            With req
                                .Method = "POST"
                                .ContentType = "application/x-www-form-urlencoded"

                                Using writer As New IO.StreamWriter(.GetRequestStream)
                                    With writer
                                        .Write(strToPost)
                                        .Flush()
                                        .Close()
                                    End With
                                End Using

                                'If .HaveResponse = True Then 'will remove for now; may need to be after req.GetResponse call in order to work correctly
                                Using resp As Net.HttpWebResponse = .GetResponse
                                    If resp IsNot Nothing Then
                                        With resp
                                            Using reader As New IO.StreamReader(.GetResponseStream)
                                                strResponse = reader.ReadToEnd
                                                reader.Close()
                                            End Using
                                        End With
                                    Else
                                        errorMsg = "no HttpWebResponse returned"
                                        errorType = CommonHelperClass.WebRequestErrorType.InvalidResponse 'added 12/19/2017
                                    End If
                                End Using
                                'Else
                                '    errorMsg = "no HttpWebResponse returned"
                                'End If
                            End With
                        Catch ex As Exception
                            errorMsg = "unhandled exception encountered while posting xml"
                            exceptionErrorMsg = ex.ToString
                            errorType = CommonHelperClass.WebRequestErrorType.UnhandledExceptionOnPost 'added 12/19/2017
                        End Try
                    Else
                        errorMsg = "unable to create HttpWebRequest"
                        errorType = CommonHelperClass.WebRequestErrorType.InvalidRequest 'added 12/19/2017
                    End If
                Else
                    errorMsg = "invalid xml post"
                    errorType = CommonHelperClass.WebRequestErrorType.Validation 'added 12/19/2017
                End If
            Else
                errorMsg = "invalid URL"
                errorType = CommonHelperClass.WebRequestErrorType.Validation 'added 12/19/2017
            End If

            Return strResponse
        End Function

        Public Sub LoadPropertyValuationFromDatabase(ByVal propertyValuationId As String, ByRef propertyValuation As QuickQuotePropertyValuation, Optional ByRef errorMsg As String = "", Optional ByVal initiallyResetValuationToNothing As Boolean = False)
            If initiallyResetValuationToNothing = True Then
                propertyValuation = Nothing
            End If

            If propertyValuationId <> "" AndAlso IsNumeric(propertyValuationId) = True Then
                Using sql As New SQLselectObject(ConfigurationManager.AppSettings("connQQ"))
                    sql.queryOrStoredProc = "usp_Get_PropertyValuation"
                    sql.parameter = New SqlParameter("@propertyValuationId", CInt(propertyValuationId))
                    Dim dr As SqlDataReader = sql.GetDataReader
                    If dr IsNot Nothing AndAlso dr.HasRows = True Then
                        dr.Read()
                        If propertyValuation Is Nothing Then
                            propertyValuation = New QuickQuotePropertyValuation
                        End If
                        With propertyValuation
                            .db_propertyValuationId = dr.Item("propertyValuationId").ToString.Trim
                            .db_environment = dr.Item("environment").ToString.Trim
                            .db_quoteId = dr.Item("quoteId").ToString.Trim
                            .db_policyId = dr.Item("policyId").ToString.Trim
                            .db_policyImageNum = dr.Item("policyImageNum").ToString.Trim
                            .db_locationNum = dr.Item("locationNum").ToString.Trim
                            .db_buildingNum = dr.Item("buildingNum").ToString.Trim 'added 7/28/2015 for Farm
                            .db_agencyCode = dr.Item("agencyCode").ToString.Trim
                            .db_propertyValuationVendorId = dr.Item("propertyValuationVendorId").ToString.Trim
                            'dr.Item("vendorName").ToString.Trim
                            .db_propertyValuationVendorIntegrationTypeId = dr.Item("propertyValuationVendorIntegrationTypeId").ToString.Trim
                            'dr.Item("vendorIntegrationType").ToString.Trim
                            .db_propertyValuationVendorEstimatorTypeId = dr.Item("propertyValuationVendorEstimatorTypeId").ToString.Trim
                            'dr.Item("vendorEstimatorType").ToString.Trim
                            .IFM_UniqueValuationId = dr.Item("IFM_UniqueValuationId").ToString.Trim
                            .VendorValuationId = dr.Item("VendorValuationId").ToString.Trim
                            .db_propertyValuationRequestId = dr.Item("propertyValuationRequestId").ToString.Trim
                            .db_propertyValuationResponseId = dr.Item("propertyValuationResponseId").ToString.Trim
                            .db_inserted = dr.Item("inserted").ToString.Trim
                            .db_updated = dr.Item("updated").ToString.Trim

                            If .Request Is Nothing Then
                                .Request = New QuickQuotePropertyValuationRequest
                            End If
                            With .Request
                                .db_propertyValuationRequestId = dr.Item("Req_propertyValuationRequestId").ToString.Trim
                                .db_propertyValuationId = dr.Item("Req_propertyValuationId").ToString.Trim
                                .AuthId = dr.Item("AuthId").ToString.Trim
                                .AuthCode = dr.Item("AuthCode").ToString.Trim
                                .IFM_UniqueValuationId = dr.Item("Req_IFM_UniqueValuationId").ToString
                                .VendorValuationId = dr.Item("Req_VendorValuationId").ToString.Trim
                                .ClientIsBusiness = qqHelper.BitToBoolean(dr.Item("ClientIsBusiness").ToString.Trim)
                                .ClientFirstName = dr.Item("ClientFirstName").ToString.Trim
                                .ClientMiddleInitial = dr.Item("ClientMiddleInitial").ToString.Trim
                                .ClientLastName = dr.Item("ClientLastName").ToString.Trim
                                .ClientAddress1 = dr.Item("ClientAddress1").ToString.Trim
                                .ClientAddress2 = dr.Item("ClientAddress2").ToString.Trim
                                .ClientCity = dr.Item("ClientCity").ToString.Trim
                                .ClientState = dr.Item("ClientState").ToString.Trim
                                .ClientZip = dr.Item("ClientZip").ToString.Trim
                                .ReturnUrl = dr.Item("ReturnUrl").ToString.Trim
                                .ReturnUrlLinkText = dr.Item("ReturnUrlLinkText").ToString.Trim
                                .YearBuilt = dr.Item("Req_YearBuilt").ToString.Trim
                                .ReturnYearBuilt = qqHelper.BitToBoolean(dr.Item("ReturnYearBuilt").ToString.Trim)
                                .ConstructionType = dr.Item("Req_ConstructionType").ToString.Trim
                                .ReturnConstructionType = qqHelper.BitToBoolean(dr.Item("ReturnConstructionType").ToString.Trim)
                                .SquareFeet = dr.Item("Req_SquareFeet").ToString.Trim
                                .ReturnSquareFeet = qqHelper.BitToBoolean(dr.Item("ReturnSquareFeet").ToString.Trim)
                                .RoofType = dr.Item("Req_RoofType").ToString.Trim
                                .ReturnRoofType = qqHelper.BitToBoolean(dr.Item("ReturnRoofType").ToString.Trim)
                                .ArchitecturalStyle = dr.Item("Req_ArchitecturalStyle").ToString.Trim
                                .ReturnArchitecturalStyle = qqHelper.BitToBoolean(dr.Item("ReturnArchitecturalStyle").ToString.Trim)
                                .ConstructionQuality = dr.Item("Req_ConstructionQuality").ToString.Trim
                                .ReturnConstructionQuality = qqHelper.BitToBoolean(dr.Item("ReturnConstructionQuality").ToString.Trim)
                                .PhysicalShape = dr.Item("Req_PhysicalShape").ToString.Trim
                                .ReturnPhysicalShape = qqHelper.BitToBoolean(dr.Item("ReturnPhysicalShape").ToString.Trim)
                                .PrimaryExterior = dr.Item("Req_PrimaryExterior").ToString.Trim
                                .ReturnPrimaryExterior = qqHelper.BitToBoolean(dr.Item("ReturnPrimaryExterior").ToString.Trim)
                                .ReturnUrlNoPopup = qqHelper.BitToBoolean(dr.Item("ReturnUrlNoPopup").ToString.Trim)
                                .ReturnReplacementCostType = qqHelper.BitToBoolean(dr.Item("ReturnReplacementCostType").ToString.Trim)
                                .ReturnAdditionalAreas = qqHelper.BitToBoolean(dr.Item("ReturnAdditionalAreas").ToString.Trim)
                                .VendorParams = dr.Item("Req_VendorParams").ToString.Trim
                                .db_sentToVendor = qqHelper.BitToBoolean(dr.Item("sentToVendor").ToString.Trim)
                                .db_inserted = dr.Item("Req_inserted").ToString.Trim
                                .db_updated = dr.Item("Req_updated").ToString.Trim
                            End With

                            If .Response Is Nothing Then
                                .Response = New QuickQuotePropertyValuationResponse
                            End If
                            With .Response
                                .db_propertyValuationResponseId = dr.Item("Res_propertyValuationResponseId").ToString.Trim
                                .db_propertyValuationId = dr.Item("Res_propertyValuationId").ToString.Trim
                                .AuthId = dr.Item("Res_AuthId").ToString.Trim
                                .IFM_UniqueValuationId = dr.Item("Res_IFM_UniqueValuationId").ToString.Trim
                                .VendorValuationId = dr.Item("Res_VendorValuationId").ToString.Trim
                                .YearBuilt = dr.Item("Res_YearBuilt").ToString.Trim
                                .ConstructionType = dr.Item("Res_ConstructionType").ToString.Trim
                                .SquareFeet = dr.Item("Res_SquareFeet").ToString.Trim
                                .RoofType = dr.Item("Res_RoofType").ToString.Trim
                                .ArchitecturalStyle = dr.Item("Res_ArchitecturalStyle").ToString.Trim
                                .ConstructionQuality = dr.Item("Res_ConstructionQuality").ToString.Trim
                                .PhysicalShape = dr.Item("Res_PhysicalShape").ToString.Trim
                                .PrimaryExterior = dr.Item("Res_PrimaryExterior").ToString.Trim
                                .ReplacementCostValue = dr.Item("ReplacementCostValue").ToString.Trim
                                .ReplacementCostType = dr.Item("ReplacementCostType").ToString.Trim
                                .AdditionalAreas = dr.Item("AdditionalAreas").ToString.Trim
                                .StructuresReplacementCostValue = dr.Item("StructuresReplacementCostValue").ToString.Trim 'added 7/29/2015 for Farm
                                .StructuresReplacementCostValueTotal = dr.Item("StructuresReplacementCostValueTotal").ToString.Trim 'added 7/29/2015 for Farm
                                .UniqueItemsTotalValue = dr.Item("UniqueItemsTotalValue").ToString.Trim 'added 7/29/2015 for Farm
                                .ActualCashValue = dr.Item("ActualCashValue").ToString.Trim 'added 7/29/2015 for Farm
                                .VendorParams = dr.Item("Res_VendorParams").ToString.Trim
                                .db_loadedBackIntoQuote = qqHelper.BitToBoolean(dr.Item("loadedBackIntoQuote").ToString.Trim)
                                .db_inserted = dr.Item("Res_inserted").ToString.Trim
                                .db_updated = dr.Item("Res_updated").ToString.Trim
                            End With
                        End With
                    Else
                        If sql.hasError = True Then
                            'error
                            errorMsg = "error retrieving property valuation from database"
                        Else
                            'no results
                        End If
                    End If
                End Using
            Else
                errorMsg = "numeric propertyValuationId required"
            End If
        End Sub
        'added 8/20/2014; originally copied from ProcessE2ValueResponseFromNameValueCollection
        Public Sub SavePropertyValuationResponseBackToQuote(ByRef pvr As QuickQuotePropertyValuationResponse, Optional ByRef wasAnythingNewSaved As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal saveAllReturnValues As Boolean = False, Optional ByVal ignoreLoadedBackIntoQuoteFlag As Boolean = True)
            errorMsg = ""
            wasAnythingNewSaved = False

            Dim qq As QuickQuoteObject = GetQuoteForPropertyValuationResponse(pvr, errorMsg)
            If qq IsNot Nothing Then
                Dim wasAnythingNewLoaded As Boolean = False
                Dim didSaveOccur As Boolean = False
                'SavePropertyValuationResponseBackToQuote(pvr, qq, wasAnythingNewLoaded, didSaveOccur, errorMsg, saveAllReturnValues, ignoreLoadedBackIntoQuoteFlag)
                'updated 7/31/2015 for new optional byref params; not really needed here
                Dim valuationLocationNum As Integer = 0
                Dim valuationBuildingNum As Integer = 0
                SavePropertyValuationResponseBackToQuote(pvr, qq, wasAnythingNewLoaded, didSaveOccur, errorMsg, saveAllReturnValues, ignoreLoadedBackIntoQuoteFlag, valuationLocationNum, valuationBuildingNum)
                If didSaveOccur = True Then
                    wasAnythingNewSaved = True
                End If
            Else
                If errorMsg = "" Then
                    errorMsg = "unable to load QuickQuote object for property valuation response"
                End If
            End If

            'If pvr IsNot Nothing AndAlso pvr.IFM_UniqueValuationId <> "" Then
            '    Dim quoteId As String = ""
            '    Dim locNum As String = ""
            '    Dim locIdentifier As String = ""
            '    SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, locIdentifier)
            '    If quoteId <> "" AndAlso IsNumeric(quoteId) = True Then
            '        Dim qq As QuickQuoteObject = Nothing
            '        Dim errorMsg As String = ""
            '        qqXml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, qq, errorMsg)
            '        If qq IsNot Nothing Then
            '            If locNum = "" OrElse IsNumeric(locNum) = False Then
            '                locNum = "1"
            '            End If
            '            If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
            '                Dim foundLoc As Boolean = False
            '                For Each l As QuickQuoteLocation In qq.Locations
            '                    'might also include propertyValuationIdentifier
            '                    If l.PropertyValuation IsNot Nothing Then
            '                        If l.PropertyValuation.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId Then
            '                            foundLoc = True
            '                            'may need to set IFM_UniqueValuationId on PropertyValuationRequest
            '                        ElseIf l.PropertyValuation.Request IsNot Nothing AndAlso l.PropertyValuation.Request.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId Then
            '                            foundLoc = True
            '                            'may need to set IFM_UniqueValuationId on PropertyValuation
            '                        End If
            '                        If foundLoc = True Then

            '                            Exit For
            '                        End If
            '                    End If
            '                Next
            '                If foundLoc = False AndAlso locIdentifier <> "" Then
            '                    For Each l As QuickQuoteLocation In qq.Locations
            '                        'might also include propertyValuationIdentifier
            '                        If l.UniqueIdentifier = locIdentifier Then
            '                            foundLoc = True

            '                            Exit For
            '                        End If
            '                    Next
            '                End If
            '                If foundLoc = False Then
            '                    If qq.Locations IsNot Nothing AndAlso qq.Locations.Count >= locNum Then
            '                        foundLoc = True
            '                        With qq.Locations(locNum - 1)

            '                        End With
            '                    End If
            '                End If
            '            End If

            '        End If
            '    End If
            'End If
        End Sub
        Public Sub SaveAllPropertyValuationResponsesBackToQuote(ByRef qq As QuickQuoteObject, Optional ByRef wasAnythingNewLoaded As Boolean = False, Optional ByRef didSaveOccur As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal saveAllReturnValues As Boolean = False, Optional ByVal ignoreLoadedBackIntoQuoteFlags As Boolean = False)
            wasAnythingNewLoaded = False
            didSaveOccur = False
            errorMsg = ""
            Dim hasEnvironmentMisMatch As Boolean = False
            Dim hasQuoteIdMisMatch As Boolean = False
            Dim skippedPreviouslyLoadedValuation As Boolean = False
            Dim reprocessedPreviouslyLoadedValuation As Boolean = False


            If qq IsNot Nothing Then
                Dim isEndorsement = qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                'If qq.Database_QuoteId <> "" AndAlso IsNumeric(qq.Database_QuoteId) = True Then
                If (Not isEndorsement AndAlso qq.Database_QuoteId <> "" AndAlso IsNumeric(qq.Database_QuoteId) = True) OrElse
                        (isEndorsement AndAlso qq.PolicyId <> "" AndAlso IsNumeric(qq.PolicyId) AndAlso qq.PolicyImageNum <> "" AndAlso IsNumeric(qq.PolicyImageNum)) Then
                    If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
                        Dim propertyValuationResponseIds As New List(Of String)
                        Dim propertyValuationResponseIdsNotLoadedBackIntoQuote As New List(Of String)
                        For Each l As QuickQuoteLocation In qq.Locations
                            Dim wasAnythingNewLoadedHere As Boolean = False
                            Dim currentErrorMsg As String = ""
                            If l.PropertyValuation IsNot Nothing AndAlso l.PropertyValuation.Response IsNot Nothing AndAlso l.PropertyValuation.Response.db_propertyValuationResponseId <> "" AndAlso IsNumeric(l.PropertyValuation.Response.db_propertyValuationResponseId) = True Then
                                If ignoreLoadedBackIntoQuoteFlags = True OrElse l.PropertyValuation.Response.db_loadedBackIntoQuote = False Then
                                    If (Not isEndorsement AndAlso l.PropertyValuation.db_quoteId = qq.Database_QuoteId) OrElse
                                        (isEndorsement AndAlso l.PropertyValuation.db_policyId = qq.PolicyId) Then
                                        If UCase(l.PropertyValuation.db_environment) = UCase(helper.Environment) Then
                                            'okay to continue
                                            If l.PropertyValuation.Response.db_loadedBackIntoQuote = True Then
                                                reprocessedPreviouslyLoadedValuation = True
                                            End If
                                            LoadPropertyValuationResponseBackIntoLocation(l.PropertyValuation.Response, l, wasAnythingNewLoadedHere, currentErrorMsg, saveAllReturnValues)
                                            If wasAnythingNewLoadedHere = True Then
                                                'location should now contain info from property valuation response (and pvr is now l.PropertyValuation.Response)
                                                wasAnythingNewLoaded = True
                                                propertyValuationResponseIds.Add(l.PropertyValuation.Response.db_propertyValuationResponseId)
                                                If l.PropertyValuation.Response.db_loadedBackIntoQuote = False Then
                                                    propertyValuationResponseIdsNotLoadedBackIntoQuote.Add(l.PropertyValuation.Response.db_propertyValuationResponseId)
                                                    l.PropertyValuation.Response.db_loadedBackIntoQuote = True 'could also move below if needed
                                                End If
                                            End If
                                        Else
                                            hasEnvironmentMisMatch = True
                                        End If
                                    Else
                                        hasQuoteIdMisMatch = True
                                    End If
                                Else
                                    skippedPreviouslyLoadedValuation = True
                                End If
                            End If

                            'added 7/28/2015 for Farm and building
                            If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                                For Each b As QuickQuoteBuilding In l.Buildings
                                    Dim buildingWasAnythingNewLoadedHere As Boolean = False
                                    Dim buildingCurrentErrorMsg As String = ""
                                    If b.PropertyValuation IsNot Nothing AndAlso b.PropertyValuation.Response IsNot Nothing AndAlso b.PropertyValuation.Response.db_propertyValuationResponseId <> "" AndAlso IsNumeric(b.PropertyValuation.Response.db_propertyValuationResponseId) = True Then
                                        If ignoreLoadedBackIntoQuoteFlags = True OrElse b.PropertyValuation.Response.db_loadedBackIntoQuote = False Then
                                            If (Not isEndorsement AndAlso b.PropertyValuation.db_quoteId = qq.Database_QuoteId) OrElse
                                                (isEndorsement AndAlso b.PropertyValuation.db_policyId = qq.PolicyId) Then
                                                If UCase(b.PropertyValuation.db_environment) = UCase(helper.Environment) Then
                                                    'okay to continue
                                                    If b.PropertyValuation.Response.db_loadedBackIntoQuote = True Then
                                                        reprocessedPreviouslyLoadedValuation = True
                                                    End If
                                                    LoadPropertyValuationResponseBackIntoBuilding(b.PropertyValuation.Response, b, buildingWasAnythingNewLoadedHere, buildingCurrentErrorMsg, saveAllReturnValues)
                                                    If buildingWasAnythingNewLoadedHere = True Then
                                                        'building should now contain info from property valuation response (and pvr is now b.PropertyValuation.Response)... may not apply to Building like it does for Location
                                                        wasAnythingNewLoaded = True
                                                        propertyValuationResponseIds.Add(b.PropertyValuation.Response.db_propertyValuationResponseId)
                                                        If b.PropertyValuation.Response.db_loadedBackIntoQuote = False Then
                                                            propertyValuationResponseIdsNotLoadedBackIntoQuote.Add(b.PropertyValuation.Response.db_propertyValuationResponseId)
                                                            b.PropertyValuation.Response.db_loadedBackIntoQuote = True 'could also move below if needed
                                                        End If
                                                    End If
                                                Else
                                                    hasEnvironmentMisMatch = True
                                                End If
                                            Else
                                                hasQuoteIdMisMatch = True
                                            End If
                                        Else
                                            skippedPreviouslyLoadedValuation = True
                                        End If
                                    End If
                                Next
                            End If
                        Next

                        If wasAnythingNewLoaded = True Then
                            'attempt save
                            Dim saveErrorMsg As String = ""
                            'qqXml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qq.Database_QuoteId, saveErrorMsg)
                            If isEndorsement Then
                                Dim success As Boolean = False
                                Dim qqEndorsementResults = Nothing 'only used for debug
                                success = qqXml.SuccessfullySavedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn_WithqqoMethodToCall(qq, qqeResults:=qqEndorsementResults, errorMessage:=saveErrorMsg)
                            Else
                                qqXml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qq.Database_QuoteId, saveErrorMsg)
                            End If
                            If saveErrorMsg = "" Then
                                didSaveOccur = True
                                If propertyValuationResponseIds IsNot Nothing AndAlso propertyValuationResponseIds.Count > 0 Then
                                    For Each pvrId As String In propertyValuationResponseIds
                                        Dim updateErrorMsg As String = ""
                                        UpdatePropertyValuationResponse_LoadedBackIntoQuote(pvrId, True, updateErrorMsg)
                                    Next
                                End If
                            Else
                                errorMsg = qqHelper.appendText("problem saving quote", saveErrorMsg, ": ")
                                'go back and update the ones that were previously False
                                If propertyValuationResponseIdsNotLoadedBackIntoQuote IsNot Nothing AndAlso propertyValuationResponseIdsNotLoadedBackIntoQuote.Count > 0 Then
                                    If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
                                        For Each l As QuickQuoteLocation In qq.Locations
                                            If l.PropertyValuation IsNot Nothing AndAlso l.PropertyValuation.Response IsNot Nothing AndAlso propertyValuationResponseIdsNotLoadedBackIntoQuote.Contains(l.PropertyValuation.Response.db_propertyValuationResponseId) = True Then
                                                l.PropertyValuation.Response.db_loadedBackIntoQuote = False
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        End If
                    End If
                Else
                    errorMsg = "QuickQuote object must have a valid quoteId"
                End If
            Else
                errorMsg = "valid QuickQuote object required"
            End If
        End Sub
        'Public Sub SavePropertyValuationResponseBackToQuote(ByRef pvr As QuickQuotePropertyValuationResponse, ByRef qq As QuickQuoteObject, Optional ByRef wasAnythingNewLoaded As Boolean = False, Optional ByRef didSaveOccur As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal saveAllReturnValues As Boolean = False, Optional ByVal ignoreLoadedBackIntoQuoteFlag As Boolean = True)
        'updated 7/31/2015 for new optional byref params
        Public Sub SavePropertyValuationResponseBackToQuote(ByRef pvr As QuickQuotePropertyValuationResponse, ByRef qq As QuickQuoteObject, Optional ByRef wasAnythingNewLoaded As Boolean = False, Optional ByRef didSaveOccur As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal saveAllReturnValues As Boolean = False, Optional ByVal ignoreLoadedBackIntoQuoteFlag As Boolean = True, Optional ByRef valuationLocationNumber As Integer = 0, Optional ByRef valuationBuildingNumber As Integer = 0)
            wasAnythingNewLoaded = False
            didSaveOccur = False
            errorMsg = ""
            'added 7/31/2015
            valuationLocationNumber = 0
            valuationBuildingNumber = 0

            Dim isEndorsement = qq IsNot Nothing AndAlso qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote

            If pvr IsNot Nothing AndAlso pvr.db_propertyValuationResponseId <> "" AndAlso IsNumeric(pvr.db_propertyValuationResponseId) = True Then
                If ignoreLoadedBackIntoQuoteFlag = True OrElse pvr.db_loadedBackIntoQuote = False Then
                    If qq Is Nothing Then
                        Dim getQuoteErrorMsg As String = ""
                        qq = GetQuoteForPropertyValuationResponse(pvr, getQuoteErrorMsg)
                        'isEndorsement = qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                    End If
                    If qq IsNot Nothing Then
                        'If qq.Database_QuoteId <> "" AndAlso IsNumeric(qq.Database_QuoteId) = True Then
                        If (Not isEndorsement AndAlso qq.Database_QuoteId <> "" AndAlso IsNumeric(qq.Database_QuoteId) = True) OrElse
                            (isEndorsement AndAlso qq.PolicyId <> "" AndAlso IsNumeric(qq.PolicyId) AndAlso qq.PolicyImageNum <> "" AndAlso IsNumeric(qq.PolicyImageNum)) Then
                            If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
                                Dim locCounter As Integer = 0 'added 7/31/2015
                                For Each l As QuickQuoteLocation In qq.Locations
                                    locCounter += 1 'added 7/31/2015
                                    If l.PropertyValuation IsNot Nothing AndAlso l.PropertyValuation.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId Then
                                        valuationLocationNumber = locCounter 'added 7/31/2015
                                        LoadPropertyValuationResponseBackIntoLocation(pvr, l, wasAnythingNewLoaded, errorMsg, saveAllReturnValues)
                                        If wasAnythingNewLoaded = True Then
                                            'location should now contain info from property valuation response (and pvr is now l.PropertyValuation.Response); attempt save
                                            Dim saveErrorMsg As String = ""
                                            'qqXml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qq.Database_QuoteId, saveErrorMsg)
                                            If qq.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                                Dim success As Boolean = False
                                                Dim qqEndorsementResults = Nothing 'only used for debug
                                                success = qqXml.SuccessfullySavedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn_WithqqoMethodToCall(qq, qqeResults:=qqEndorsementResults, errorMessage:=saveErrorMsg)
                                            Else
                                                qqXml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qq.Database_QuoteId, saveErrorMsg)
                                            End If

                                            If saveErrorMsg = "" Then
                                                didSaveOccur = True
                                                Dim updateErrorMsg As String = ""
                                                UpdatePropertyValuationResponse_LoadedBackIntoQuote(l.PropertyValuation.Response.db_propertyValuationResponseId, True, updateErrorMsg)
                                                l.PropertyValuation.Response.db_loadedBackIntoQuote = True
                                            Else
                                                errorMsg = qqHelper.appendText("problem saving quote", saveErrorMsg, ": ")
                                                'l.PropertyValuation.Response.db_loadedBackIntoQuote = False 'not needed since it's only being changed if successful... shouldn't affect the ones that are already True
                                            End If
                                        End If
                                        Exit For
                                    End If
                                    'added 7/28/2015 for Farm and building
                                    Dim exitLocFor As Boolean = False
                                    If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                                        Dim buildCounter As Integer = 0 'added 7/31/2015
                                        For Each b As QuickQuoteBuilding In l.Buildings
                                            buildCounter += 1 'added 7/31/2015
                                            If b.PropertyValuation IsNot Nothing AndAlso b.PropertyValuation.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId Then
                                                valuationLocationNumber = locCounter 'added 7/31/2015
                                                valuationBuildingNumber = buildCounter 'added 7/31/2015
                                                LoadPropertyValuationResponseBackIntoBuilding(pvr, b, wasAnythingNewLoaded, errorMsg, saveAllReturnValues)
                                                If wasAnythingNewLoaded = True Then
                                                    'location should now contain info from property valuation response (and pvr is now l.PropertyValuation.Response); attempt save
                                                    Dim saveErrorMsg As String = ""
                                                    'qqXml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qq.Database_QuoteId, saveErrorMsg)
                                                    If isEndorsement Then
                                                        Dim success As Boolean = False
                                                        Dim qqEndorsementResults = Nothing 'only used for debug
                                                        success = qqXml.SuccessfullySavedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn_WithqqoMethodToCall(qq, qqeResults:=qqEndorsementResults, errorMessage:=saveErrorMsg)
                                                    Else
                                                        qqXml.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, qq, qq.Database_QuoteId, saveErrorMsg)
                                                    End If
                                                    If saveErrorMsg = "" Then
                                                        didSaveOccur = True
                                                        Dim updateErrorMsg As String = ""
                                                        UpdatePropertyValuationResponse_LoadedBackIntoQuote(b.PropertyValuation.Response.db_propertyValuationResponseId, True, updateErrorMsg)
                                                        b.PropertyValuation.Response.db_loadedBackIntoQuote = True
                                                    Else
                                                        errorMsg = qqHelper.appendText("problem saving quote", saveErrorMsg, ": ")
                                                        'b.PropertyValuation.Response.db_loadedBackIntoQuote = False 'not needed since it's only being changed if successful... shouldn't affect the ones that are already True
                                                    End If
                                                End If
                                                exitLocFor = True
                                                Exit For
                                            End If
                                        Next
                                    End If
                                    If exitLocFor = True Then
                                        Exit For
                                    End If
                                Next
                            Else
                                errorMsg = "no locations found on QuickQuote object"
                            End If
                        Else
                            errorMsg = "QuickQuote object must have a valid quoteId"
                        End If
                    Else
                        errorMsg = "valid QuickQuote object required"
                    End If
                Else
                    errorMsg = "property valuation response has already been loaded back into quote"
                End If
            Else
                errorMsg = "property valuation response with valid database id required"
            End If
        End Sub
        Public Sub LoadPropertyValuationResponseBackIntoLocation(ByRef pvr As QuickQuotePropertyValuationResponse, ByRef qqLoc As QuickQuoteLocation, Optional ByRef wasAnythingNewLoaded As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal saveAllReturnValues As Boolean = False)
            wasAnythingNewLoaded = False
            errorMsg = ""

            If pvr IsNot Nothing AndAlso pvr.db_propertyValuationResponseId <> "" AndAlso IsNumeric(pvr.db_propertyValuationResponseId) = True Then
                If qqLoc IsNot Nothing Then
                    If qqLoc.PropertyValuation IsNot Nothing AndAlso qqLoc.PropertyValuation.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId Then
                        'okay to continue; load property valuation response info into location... but don't save; okay to overwrite property valuation response on location
                        wasAnythingNewLoaded = True 'added 8/28/2014; this was missing so nothing was ever getting saved back to quote
                        qqLoc.PropertyValuation.Response = pvr
                        With qqLoc.PropertyValuation.Response
                            'updated 8/21/2014
                            Dim lastCostEstDate As String = .db_inserted
                            If lastCostEstDate = "" OrElse IsDate(lastCostEstDate) = False Then
                                lastCostEstDate = Date.Today.ToShortDateString
                            End If
                            qqLoc.LastCostEstimatorDate = lastCostEstDate
                            qqLoc.RebuildCost = .ReplacementCostValue 'possible storage spot
                            'qqLoc.MarketValue = .ReplacementCostValue 'possible storage spot
                            'qqLoc.A_Dwelling_Limit = .ReplacementCostValue 'possible storage spot

                            If saveAllReturnValues = True Then
                                'updated 8/21/2014
                                Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                                If qqLoc.PropertyValuation.Vendor <> QuickQuotePropertyValuation.ValuationVendor.None OrElse qqLoc.PropertyValuation.VendorEstimatorType <> QuickQuotePropertyValuation.ValuationVendorEstimatorType.None Then
                                    If qqLoc.PropertyValuation.Vendor <> QuickQuotePropertyValuation.ValuationVendor.None Then
                                        Dim a1 As New QuickQuoteStaticDataAttribute
                                        a1.nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.Vendor
                                        a1.nvp_value = System.Enum.GetName(GetType(QuickQuotePropertyValuation.ValuationVendor), qqLoc.PropertyValuation.Vendor)
                                        optionAttributes.Add(a1)
                                    End If
                                    If qqLoc.PropertyValuation.VendorEstimatorType <> QuickQuotePropertyValuation.ValuationVendorEstimatorType.None Then
                                        Dim a2 As New QuickQuoteStaticDataAttribute
                                        a2.nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.VendorEstimatorType
                                        a2.nvp_value = System.Enum.GetName(GetType(QuickQuotePropertyValuation.ValuationVendorEstimatorType), qqLoc.PropertyValuation.VendorEstimatorType)
                                        optionAttributes.Add(a2)
                                    End If
                                End If
                                If .VendorValuationId = 1 Then  'E2Value has below values are return params 
                                    qqLoc.YearBuilt = .YearBuilt
                                    'qqLoc.ConstructionTypeId = qqHelper.GetRelatedStaticDataValueForOptionValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePropertyValuationResponse, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionType, optionAttributes, .ConstructionType, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId)
                                    'updated 8/28/2014 to only overwrite if the static data mapping is in place
                                    Dim VrConstructionTypeId As String = qqHelper.GetRelatedStaticDataValueForOptionValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePropertyValuationResponse, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionType, optionAttributes, .ConstructionType, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId)
                                    If VrConstructionTypeId <> "" Then
                                        qqLoc.ConstructionTypeId = VrConstructionTypeId
                                    End If
                                    qqLoc.SquareFeet = .SquareFeet
                                    qqLoc.ArchitecturalStyle = qqHelper.GetRelatedStaticDataValueForOptionValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePropertyValuationResponse, QuickQuoteHelperClass.QuickQuotePropertyName.ArchitecturalStyle, optionAttributes, .ArchitecturalStyle, QuickQuoteHelperClass.QuickQuotePropertyName.ArchitecturalStyle)
                                End If
                            End If
                        End With
                    Else
                        errorMsg = "property valuation isn't related to location"
                    End If
                Else
                    errorMsg = "valid Location object required"
                End If
            Else
                errorMsg = "property valuation response with valid database id required"
            End If
        End Sub
        'added 7/28/2015; originally copied from LoadPropertyValuationResponseBackIntoLocation
        Public Sub LoadPropertyValuationResponseBackIntoBuilding(ByRef pvr As QuickQuotePropertyValuationResponse, ByRef qqBuild As QuickQuoteBuilding, Optional ByRef wasAnythingNewLoaded As Boolean = False, Optional ByRef errorMsg As String = "", Optional ByVal saveAllReturnValues As Boolean = False)
            wasAnythingNewLoaded = False
            errorMsg = ""

            If pvr IsNot Nothing AndAlso pvr.db_propertyValuationResponseId <> "" AndAlso IsNumeric(pvr.db_propertyValuationResponseId) = True Then
                If qqBuild IsNot Nothing Then
                    If qqBuild.PropertyValuation IsNot Nothing AndAlso qqBuild.PropertyValuation.IFM_UniqueValuationId = pvr.IFM_UniqueValuationId Then
                        'okay to continue; load property valuation response info into location... but don't save; okay to overwrite property valuation response on location
                        wasAnythingNewLoaded = True 'added 8/28/2014; this was missing so nothing was ever getting saved back to quote
                        qqBuild.PropertyValuation.Response = pvr
                        With qqBuild.PropertyValuation.Response
                            'updated 8/21/2014
                            '7/28/2015 note: these aren't being used yet
                            'Dim lastCostEstDate As String = .db_inserted
                            'If lastCostEstDate = "" OrElse IsDate(lastCostEstDate) = False Then
                            '    lastCostEstDate = Date.Today.ToShortDateString
                            'End If
                            'qqBuild.LastCostEstimatorDate = lastCostEstDate
                            'qqBuild.RebuildCost = .ReplacementCostValue 'possible storage spot
                            ''qqBuild.MarketValue = .ReplacementCostValue 'possible storage spot
                            ''qqBuild.A_Dwelling_Limit = .ReplacementCostValue 'possible storage spot

                            If saveAllReturnValues = True Then
                                'updated 8/21/2014
                                Dim optionAttributes As New List(Of QuickQuoteStaticDataAttribute)
                                If qqBuild.PropertyValuation.Vendor <> QuickQuotePropertyValuation.ValuationVendor.None OrElse qqBuild.PropertyValuation.VendorEstimatorType <> QuickQuotePropertyValuation.ValuationVendorEstimatorType.None Then
                                    If qqBuild.PropertyValuation.Vendor <> QuickQuotePropertyValuation.ValuationVendor.None Then
                                        Dim a1 As New QuickQuoteStaticDataAttribute
                                        a1.nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.Vendor
                                        a1.nvp_value = System.Enum.GetName(GetType(QuickQuotePropertyValuation.ValuationVendor), qqBuild.PropertyValuation.Vendor)
                                        optionAttributes.Add(a1)
                                    End If
                                    If qqBuild.PropertyValuation.VendorEstimatorType <> QuickQuotePropertyValuation.ValuationVendorEstimatorType.None Then
                                        Dim a2 As New QuickQuoteStaticDataAttribute
                                        a2.nvp_propertyName = QuickQuoteHelperClass.QuickQuotePropertyName.VendorEstimatorType
                                        a2.nvp_value = System.Enum.GetName(GetType(QuickQuotePropertyValuation.ValuationVendorEstimatorType), qqBuild.PropertyValuation.VendorEstimatorType)
                                        optionAttributes.Add(a2)
                                    End If
                                End If
                                qqBuild.YearBuilt = .YearBuilt
                                '7/28/2015 note: these aren't being used yet; added ConstructionId stuff back in 8/3/2015 (different property name for Building than Location)
                                'qqBuild.ConstructionTypeId = qqHelper.GetRelatedStaticDataValueForOptionValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePropertyValuationResponse, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionType, optionAttributes, .ConstructionType, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId)
                                'updated 8/28/2014 to only overwrite if the static data mapping is in place
                                Dim VrConstructionTypeId As String = qqHelper.GetRelatedStaticDataValueForOptionValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePropertyValuationResponse, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionType, optionAttributes, .ConstructionType, QuickQuoteHelperClass.QuickQuotePropertyName.ConstructionTypeId)
                                If VrConstructionTypeId <> "" Then
                                    qqBuild.ConstructionId = VrConstructionTypeId
                                End If
                                qqBuild.SquareFeet = .SquareFeet
                                '7/28/2015 note: these aren't being used yet
                                'qqBuild.ArchitecturalStyle = qqHelper.GetRelatedStaticDataValueForOptionValue_MatchingOptionAttributes(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuotePropertyValuationResponse, QuickQuoteHelperClass.QuickQuotePropertyName.ArchitecturalStyle, optionAttributes, .ArchitecturalStyle, QuickQuoteHelperClass.QuickQuotePropertyName.ArchitecturalStyle)
                            End If
                        End With
                    Else
                        errorMsg = "property valuation isn't related to building"
                    End If
                Else
                    errorMsg = "valid Building object required"
                End If
            Else
                errorMsg = "property valuation response with valid database id required"
            End If
        End Sub
        Public Function GetQuoteForPropertyValuationResponse(ByVal pvr As QuickQuotePropertyValuationResponse, Optional ByRef errorMsg As String = "") As QuickQuoteObject
            Dim qq As QuickQuoteObject = Nothing
            errorMsg = ""

            If pvr IsNot Nothing Then
                If pvr.IFM_UniqueValuationId <> "" Then
                    Dim quoteId As String = ""
                    Dim locNum As String = ""
                    Dim buildNum As String = "" 'added 7/28/2015
                    Dim policyId As String = ""
                    Dim policyImageNum As String = ""
                    Dim propertyValuationId As String = ""
                    Dim getQuoteErrorMsg As String = ""
                    'SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, propertyValuationId)
                    'updated 7/28/2015 for buildingNum
                    SplitUniqueValuationIdString(pvr.IFM_UniqueValuationId, quoteId, locNum, buildNum, propertyValuationId, policyId, policyImageNum)
                    If quoteId <> "" AndAlso IsNumeric(quoteId) = True Then
                        qqXml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, qq, getQuoteErrorMsg)

                    ElseIf policyId <> "" AndAlso IsNumeric(policyId) AndAlso policyImageNum <> "" AndAlso IsNumeric(policyImageNum) = True Then
                        qq = qqXml.QuickQuoteEndorsementForPolicyIdAndImageNum(policyId, policyImageNum, errorMessage:=getQuoteErrorMsg)
                    Else
                        errorMsg = "valid quoteId/policyId required in property valuation response identifier"
                    End If
                    If qq IsNot Nothing Then
                        'okay
                    Else
                        errorMsg = qqHelper.appendText("unable to load QuickQuote object", getQuoteErrorMsg, ": ")
                    End If
                Else
                    errorMsg = "property valuation response must have an identifier in order to locate the quote"
                End If
            Else
                errorMsg = "valid property valuation response required"
            End If

            Return qq
        End Function
        'added 4/29/2015 for FAR
        'Public Function GetEstimatorTypeForLOB(ByVal lobType As QuickQuoteObject.QuickQuoteLobType) As QuickQuotePropertyValuation.ValuationVendorEstimatorType
        'updated 7/28/2015 to distinguish between Location and Building'ValuationPropertyType
        Public Function GetEstimatorTypeForLOB(ByVal lobType As QuickQuoteObject.QuickQuoteLobType, Optional ByVal propertyType As ValuationPropertyType = ValuationPropertyType.Location) As QuickQuotePropertyValuation.ValuationVendorEstimatorType
            Dim estimatorType As QuickQuotePropertyValuation.ValuationVendorEstimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.None

            'added 7/28/2015
            If propertyType = ValuationPropertyType.None Then
                propertyType = ValuationPropertyType.Location
            End If

            Select Case lobType
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal, QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                    estimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential
                Case QuickQuoteObject.QuickQuoteLobType.Farm
                    'estimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead 'may also try Farm & Ranch
                    'updated 7/28/2015 to use new PropertyType param
                    If propertyType = ValuationPropertyType.Building Then
                        estimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.FarmandRanch
                    ElseIf propertyType = ValuationPropertyType.LocationAndBuildings Then
                        estimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.Homestead
                    Else
                        estimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential
                    End If
                Case Else 'just in case since that was the one being used for everything (when it was only being used for HOM)
                    estimatorType = QuickQuotePropertyValuation.ValuationVendorEstimatorType.ProntoLiteResidential
            End Select

            Return estimatorType
        End Function
        'Public Function GetEstimatorTypeForQuote(ByVal qq As QuickQuoteObject) As QuickQuotePropertyValuation.ValuationVendorEstimatorType
        'updated 7/28/2015 to distinguish between Location and Building
        Public Function GetEstimatorTypeForQuote(ByVal qq As QuickQuoteObject, Optional ByVal propertyType As ValuationPropertyType = ValuationPropertyType.Location) As QuickQuotePropertyValuation.ValuationVendorEstimatorType
            Dim lobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None

            If qq IsNot Nothing Then
                lobType = qq.LobType
            End If

            Return GetEstimatorTypeForLOB(lobType, propertyType) 'updated 7/28/2015 to send new propertyType param
        End Function
        'added 5/27/2015
        Public Function VendorValuationReturnUrl(ByVal valuationVendor As QuickQuotePropertyValuation.ValuationVendor) As String
            Dim returnUrl As String = ""

            If valuationVendor = QuickQuotePropertyValuation.ValuationVendor.e2Value Then
                If helper.IsTestEnvironment = True Then
                    returnUrl = helper.sessionVariableValueAsString("e2ValueTest_ReturnUrl")
                End If
                If returnUrl = "" Then
                    returnUrl = helper.configAppSettingValueAsString("e2Value_ReturnUrl")
                End If
            ElseIf valuationVendor = QuickQuotePropertyValuation.ValuationVendor.Verisk360 Then
                If helper.IsTestEnvironment = True Then
                    returnUrl = helper.sessionVariableValueAsString("VR_Verisk360_Test_ReturnUrl")
                End If
                If returnUrl = "" Then
                    returnUrl = helper.configAppSettingValueAsString("VR_Verisk360_ReturnUrl")
                End If
            End If

            Return returnUrl
        End Function

        'Public Function Site360ValueReturnUrl() As String
        '    Dim returnUrl As String = ""

        '    If helper.IsTestEnvironment = True Then
        '        returnUrl = helper.sessionVariableValueAsString("360ValueTest_ReturnUrl")
        '    End If
        '    If returnUrl = "" Then
        '        returnUrl = helper.configAppSettingValueAsString("360Value_ReturnUrl")
        '    End If

        '    Return returnUrl
        'End Function
        Public Function E2ValueRequest_AppendReturnStructuresRpc() As Boolean
            Dim appendIt As Boolean = False

            Dim sessionAppendIt As String = helper.sessionVariableValueAsString("e2ValueTest_AppendReturnStructuresRpc")
            If sessionAppendIt <> "" Then
                appendIt = qqHelper.BitToBoolean(sessionAppendIt)
            End If

            Return appendIt
        End Function

        'added here 7/28/2015; ported over from QuickQuoteLocation... will now be used on QuickQuoteBuilding too
        Public Sub SplitPropertyValuationModifierText(ByVal propertyValuationModifierText As String, ByRef propertyValuationId As String, ByRef propertyValuationArchitecturalStyle As String, Optional ByRef foundPropertyValuationId As Boolean = False, Optional ByRef foundPropertyValuationArchitecturalStyle As Boolean = False)
            'propertyValuationId==12345||architecturalStyle==TestStyle
            propertyValuationId = ""
            propertyValuationArchitecturalStyle = ""
            foundPropertyValuationId = False
            foundPropertyValuationArchitecturalStyle = False
            If propertyValuationModifierText <> "" AndAlso propertyValuationModifierText.Contains("==") = True Then
                Dim arNameValuePair As Array
                If propertyValuationModifierText.Contains("||") = True Then
                    'multiple values
                    Dim arPvModString As String()
                    arPvModString = Split(propertyValuationModifierText, "||")
                    For Each nameValuePair As String In arPvModString
                        If nameValuePair.Contains("==") = True Then
                            arNameValuePair = Split(nameValuePair, "==")
                            Select Case UCase(arNameValuePair(0).ToString.Trim)
                                Case "PROPERTYVALUATIONID", "PROPERTYVALID", "PROPVALID", "PVID"
                                    propertyValuationId = arNameValuePair(1).ToString.Trim
                                    foundPropertyValuationId = True
                                Case "ARCHITECTURALSTYLE", "PROPERTYVALUATIONARCHITECTURALSTYLE", "PVARCHITECTURALSTYLE", "PVARCHSTYLE"
                                    propertyValuationArchitecturalStyle = arNameValuePair(1).ToString.Trim
                                    foundPropertyValuationArchitecturalStyle = True
                            End Select
                        End If
                    Next
                ElseIf propertyValuationId = "" AndAlso (UCase(propertyValuationModifierText).Contains("PROPERTYVALUATIONID") = True OrElse UCase(propertyValuationModifierText).Contains("PROPERTYVALID") = True OrElse UCase(propertyValuationModifierText).Contains("PROPVALID") = True OrElse UCase(propertyValuationModifierText).Contains("PVID") = True) Then
                    arNameValuePair = Split(propertyValuationModifierText, "==")
                    propertyValuationId = arNameValuePair(1).ToString.Trim
                    foundPropertyValuationId = True

                    'added extra ElseIf 8/27/2014 to pick up architecturalStyle if it's by itself
                ElseIf propertyValuationArchitecturalStyle = "" AndAlso (UCase(propertyValuationModifierText).Contains("ARCHITECTURALSTYLE") = True OrElse UCase(propertyValuationModifierText).Contains("PROPERTYVALUATIONARCHITECTURALSTYLE") = True OrElse UCase(propertyValuationModifierText).Contains("PVARCHITECTURALSTYLE") = True OrElse UCase(propertyValuationModifierText).Contains("PVARCHSTYLE") = True) Then
                    arNameValuePair = Split(propertyValuationModifierText, "==")
                    propertyValuationArchitecturalStyle = arNameValuePair(1).ToString.Trim
                    foundPropertyValuationArchitecturalStyle = True
                End If
            End If
        End Sub
        'added 7/29/2015
        Public Function ListFromDelimitedParam(ByVal param As String, Optional ByVal delimiter As String = "|", Optional ByVal formatAsCurrency As Boolean = False) As List(Of String)
            Dim newList As List(Of String) = Nothing
            If String.IsNullOrEmpty(delimiter) = True Then
                delimiter = "|"
            End If

            If String.IsNullOrEmpty(param) = False Then
                newList = New List(Of String)
                If param.Contains(delimiter) = True Then
                    Dim arParamString As String() = Nothing
                    arParamString = Split(param, delimiter)
                    If arParamString IsNot Nothing AndAlso arParamString.Count > 0 Then
                        For Each val As String In arParamString
                            If formatAsCurrency = True AndAlso qqHelper.IsNumericString(val) = True Then
                                val = FormatCurrency(val, 2)
                            End If
                            newList.Add(val)
                        Next
                    Else
                        If formatAsCurrency = True AndAlso qqHelper.IsNumericString(param) = True Then
                            param = FormatCurrency(param, 2)
                        End If
                        newList.Add(param)
                    End If
                Else
                    If formatAsCurrency = True AndAlso qqHelper.IsNumericString(param) = True Then
                        param = FormatCurrency(param, 2)
                    End If
                    newList.Add(param)
                End If
            End If

            Return newList
        End Function
        'added 7/30/2015
        Public Sub SetPropertyValuationCurrentLocationAndBuildingNumForQuoteAndUniqueValuationId(ByVal qq As QuickQuoteObject, ByVal uniqueValuationId As String, ByRef pv As QuickQuotePropertyValuation, ByRef locNum As Integer, ByRef buildNum As Integer)
            pv = Nothing
            locNum = 0
            buildNum = 0

            If String.IsNullOrEmpty(uniqueValuationId) = False AndAlso qq IsNot Nothing Then
                If qq.Locations IsNot Nothing AndAlso qq.Locations.Count > 0 Then
                    Dim locCounter As Integer = 0
                    For Each l As QuickQuoteLocation In qq.Locations
                        locCounter += 1

                        If l.PropertyValuation IsNot Nothing AndAlso l.PropertyValuation.IFM_UniqueValuationId = uniqueValuationId Then
                            locNum = locCounter
                            buildNum = 0 'redundant since set above
                            pv = l.PropertyValuation
                            Exit For
                        Else
                            'check buildings
                            Dim exitLocFor As Boolean = False
                            If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                                Dim buildCounter As Integer = 0
                                For Each b As QuickQuoteBuilding In l.Buildings
                                    buildCounter += 1
                                    If b.PropertyValuation IsNot Nothing AndAlso b.PropertyValuation.IFM_UniqueValuationId = uniqueValuationId Then
                                        locNum = locCounter
                                        buildNum = buildCounter
                                        pv = b.PropertyValuation
                                        exitLocFor = True
                                        Exit For
                                    End If
                                Next
                            End If
                            If exitLocFor = True Then
                                Exit For
                            End If
                        End If
                    Next
                End If
            End If
        End Sub
        Public Sub SetPropertyValuationCurrentLocationAndBuildingNumForQuoteIdAndUniqueValuationId(ByVal quoteId As String, ByVal uniqueValuationId As String, ByRef pv As QuickQuotePropertyValuation, ByRef locNum As Integer, ByRef buildNum As Integer)
            Dim qq As QuickQuoteObject = Nothing
            If qqHelper.IsValidQuickQuoteIdOrNum(quoteId) = True Then
                Dim getQuoteErrorMsg As String = ""
                qqXml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, qq, getQuoteErrorMsg)
            End If
            SetPropertyValuationCurrentLocationAndBuildingNumForQuoteAndUniqueValuationId(qq, uniqueValuationId, pv, locNum, buildNum)
        End Sub
        Public Sub SetPropertyValuationCurrentLocationAndBuildingNumForQuoteIdAndUniqueValuationId(ByVal policyId As String, ByVal policyImageNum As String, ByVal uniqueValuationId As String, ByRef pv As QuickQuotePropertyValuation, ByRef locNum As Integer, ByRef buildNum As Integer)
            Dim qq As QuickQuoteObject = Nothing
            If qqHelper.IsValidQuickQuoteIdOrNum(policyId) AndAlso qqHelper.IsValidQuickQuoteIdOrNum(policyImageNum) Then
                Dim getQuoteErrorMsg As String = ""
                qq = qqXml.QuickQuoteEndorsementForPolicyIdAndImageNum(policyId, policyImageNum, errorMessage:=getQuoteErrorMsg)
            End If
            SetPropertyValuationCurrentLocationAndBuildingNumForQuoteAndUniqueValuationId(qq, uniqueValuationId, pv, locNum, buildNum)
        End Sub
        Public Sub SetCurrentLocationAndBuildingNumForQuoteAndUniqueValuationId(ByVal qq As QuickQuoteObject, ByVal uniqueValuationId As String, ByRef locNum As Integer, ByRef buildNum As Integer)
            Dim pv As QuickQuotePropertyValuation = Nothing
            SetPropertyValuationCurrentLocationAndBuildingNumForQuoteAndUniqueValuationId(qq, uniqueValuationId, pv, locNum, buildNum)
        End Sub
        Public Sub SetCurrentLocationAndBuildingNumForQuoteIdAndUniqueValuationId(ByVal quoteId As String, ByVal uniqueValuationId As String, ByRef locNum As Integer, ByRef buildNum As Integer)
            Dim qq As QuickQuoteObject = Nothing
            If qqHelper.IsValidQuickQuoteIdOrNum(quoteId) = True Then
                Dim getQuoteErrorMsg As String = ""
                qqXml.GetQuoteForSaveType(quoteId, QuickQuoteXML.QuickQuoteSaveType.Quote, qq, getQuoteErrorMsg)
            End If
            SetCurrentLocationAndBuildingNumForQuoteAndUniqueValuationId(qq, uniqueValuationId, locNum, buildNum)
        End Sub
        Public Sub SetCurrentLocationAndBuildingNumForQuoteIdAndUniqueValuationId(ByVal policyId As String, ByVal policyImageNum As String, ByVal uniqueValuationId As String, ByRef locNum As Integer, ByRef buildNum As Integer)
            Dim qq As QuickQuoteObject = Nothing
            If qqHelper.IsValidQuickQuoteIdOrNum(policyId) AndAlso qqHelper.IsValidQuickQuoteIdOrNum(policyImageNum) Then
                Dim getQuoteErrorMsg As String = ""
                qq = qqXml.QuickQuoteEndorsementForPolicyIdAndImageNum(policyId, policyImageNum, errorMessage:=getQuoteErrorMsg)
            End If
            SetCurrentLocationAndBuildingNumForQuoteAndUniqueValuationId(qq, uniqueValuationId, locNum, buildNum)
        End Sub
        'added 7/31/2015
        'Public Function ValuationPropertyTypeDefaultByInfo(ByVal locNum As Integer, ByVal buildNum As Integer) As ValuationPropertyType
        '8/27/2015 - added QuickQuoteObject as optional param
        Public Function ValuationPropertyTypeDefaultByInfo(ByVal locNum As Integer, ByVal buildNum As Integer, Optional ByVal qq As QuickQuoteObject = Nothing) As ValuationPropertyType
            Dim vpt As ValuationPropertyType = ValuationPropertyType.Location

            If buildNum > 0 Then
                'updated 8/27/2015 - 8/28/2015; original logic in ELSE
                Dim qqBuilding As QuickQuoteBuilding = qqHelper.QuickQuoteBuildingForActiveNum(qq, locNum, buildNum)
                If qqBuilding IsNot Nothing AndAlso qqHelper.IsValidQuickQuoteIdOrNum(qqBuilding.FarmStructureTypeId) = True AndAlso UCase(qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteBuilding, QuickQuoteHelperClass.QuickQuotePropertyName.FarmStructureTypeId, qqBuilding.FarmStructureTypeId)).Contains("DWELLING") = True Then
                    vpt = ValuationPropertyType.Location 'redundant since defaulted above
                Else
                    vpt = ValuationPropertyType.Building
                End If
            Else
                vpt = ValuationPropertyType.Location 'redundant since defaulted above
            End If

            Return vpt
        End Function

        Public Shared Function e2Value_ServicePoint_SecurityProtocol() As System.Net.SecurityProtocolType
            Dim spt As System.Net.SecurityProtocolType = Nothing

            Dim strSpt As String = System.Configuration.ConfigurationManager.AppSettings("e2Value_ServicePoint_SecurityProtocol")
            If String.IsNullOrWhiteSpace(strSpt) = False Then
                Select Case UCase(strSpt)
                    Case "SSL 3.0", "SSL3", "SSL", "SSL30", "SSL3.0"
                        spt = Net.SecurityProtocolType.Ssl3
                    Case "TLS 1.0", "TLS", "TLS1", "TLS10", "TLS1.0"
                        spt = Net.SecurityProtocolType.Tls
                    Case "TLS 1.1", "TLS11", "TLS1.1"
                        spt = Net.SecurityProtocolType.Tls11
                    Case "TLS 1.2", "TLS12", "TLS1.2"
                        spt = Net.SecurityProtocolType.Tls12
                    Case Else
                        If System.Enum.TryParse(Of System.Net.SecurityProtocolType)(strSpt, spt) = False Then
                            spt = Nothing
                        End If
                End Select
            End If

            Return spt
        End Function


        Public Shared Sub Set_e2Value_ServicePoint_SecurityProtocol()
            Dim spt As System.Net.SecurityProtocolType = e2Value_ServicePoint_SecurityProtocol()

            If spt <> Nothing Then
                System.Net.ServicePointManager.SecurityProtocol = spt
            End If
        End Sub
    End Class
End Namespace
