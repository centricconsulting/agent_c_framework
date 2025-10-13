Imports DCO = Diamond.Common.Objects
Imports ServiceMessages = Diamond.Common.Services.Messages
Imports DCE = Diamond.Common.Enums
Imports System.Linq
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.PPA

    Public Class VinLookup

        'Added 10/18/2022 for task 75263 MLW
        Private Shared _NewModelISORAPALookupTypeSettings As NewFlagItem
        Public Shared ReadOnly Property NewModelISORAPALookupTypeSettings() As NewFlagItem
            Get
                If _NewModelISORAPALookupTypeSettings Is Nothing Then
                    _NewModelISORAPALookupTypeSettings = New NewFlagItem("VR_PAA_NewModelISORAPALookupType_Settings")
                End If
                Return _NewModelISORAPALookupTypeSettings
            End Get
        End Property
        'Added 10/18/2022 for task 75263 MLW
        Public Shared Function NewModelISORAPALookupTypeEnabled() As Boolean
            Return NewModelISORAPALookupTypeSettings.EnabledFlag
        End Function
        'Added 10/18/2022 for task 75263 MLW
        Public Shared Function NewModelISORAPALookupTypeEffDate() As Date
            Return NewModelISORAPALookupTypeSettings.GetStartDateOrDefault("12/1/2022")
        End Function        
        'Added 10/18/2022 for task 75263 MLW
        Public Shared Function IsNewModelISORAPALookupTypeAvailable(versionId As String, effectiveDate As String, isNewBusiness As Boolean) As Boolean
            Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            If NewModelISORAPALookupTypeEnabled Then
                If isNewBusiness Then
                    If qqh.IsDateString(effectiveDate) AndAlso NewModelISORAPALookupTypeEffDate <> Nothing Then
                        If CDate(effectiveDate) >= NewModelISORAPALookupTypeEffDate Then Return True
                    End If
                Else
                    If versionId.TryToGetInt32 >= NewModelISORAPALookupTypeSettings.VersionNumber Then Return True
                End If
            End If          
            Return False
        End Function

        Public Shared Function GetMakeModelYearOrVinVehicleInfo_OptionalLookupType(Vin As String, make As String, model As String, year As Int32, effectiveDate As DateTime, versionId As Int32, lookupType As Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType, policyId As String, policyImageNum As String, vehicleNum As String) As List(Of QuickQuote.CommonMethods.VinLookupResult)
            Return QuickQuote.CommonMethods.QuickQuoteHelperClass.GetMakeModelYearOrVinVehicleInfo_OptionalLookupType(Vin, make, model, year, effectiveDate, versionId, lookupType, policyId:=policyId.TryToGetInt32(), policyImageNum:=policyImageNum.TryToGetInt32(), vehicleNum:=vehicleNum.TryToGetInt32())
        End Function
        Public Shared Function GetMakeModelYearOrVinVehicleInfo(Vin As String, make As String, model As String, year As Int32, effectiveDate As DateTime, versionId As Int32) As List(Of QuickQuote.CommonMethods.VinLookupResult)
            '            Dim lst As New List(Of Diamond.Common.Objects.VehicleInfoLookup.VehicleInfoLookupResults)
            '            Dim results As New List(Of VinLookupResult)
            '            Try
            '                Dim vinReq As New Diamond.Common.Services.Messages.VinService.VehicleInfoLookup.Request
            '                Dim vinRes As New Diamond.Common.Services.Messages.VinService.VehicleInfoLookup.Response
            '                If make.ToUpper().Contains("nissan".ToUpper()) Then
            '                    make = "nissan/datsun".ToUpper()
            '                End If
            '                Dim vins As New List(Of String)

            '                If effectiveDate < Date.Now.AddDays(-90) Then ' Matt A 11/10/2016 if effective date is invalid use todays date
            '                    effectiveDate = Date.Now
            '                End If

            '                With vinReq
            '                    With .RequestData
            '                        .LookupSource = Diamond.Common.Enums.VehicleInfoLookupType.VehicleInfoLookupType.ModelISORAPA
            '                        If String.IsNullOrWhiteSpace(Vin) Then
            '                            .Make = make.Trim()
            '                            .Model = model.Trim()
            '                            .Year = year
            '                            .Type = Diamond.Common.Enums.Vin.MakeModelLookupType.LookupUsingYearMakeModel
            '                        Else
            '                            .Vin = Vin.Trim().ToUpper()
            '                            .Type = Diamond.Common.Enums.Vin.MakeModelLookupType.LookupUsingVin
            '                        End If
            '                    End With
            '                End With
            '                Using proxy As New Diamond.Common.Services.Proxies.VinServiceProxy
            '                    vinRes = proxy.VehicleInfoLookup(vinReq)
            '                End Using




            '                If vinRes IsNot Nothing Then
            '                    With vinRes
            '                        If .ResponseData IsNot Nothing Then
            '                            With .ResponseData
            '                                If .VehicleInfoLookupResults IsNot Nothing AndAlso .VehicleInfoLookupResults.Count > 0 Then
            '                                    For Each lr As Diamond.Common.Objects.VehicleInfoLookup.VehicleInfoLookupResults In .VehicleInfoLookupResults
            '                                        lst.Add(lr)
            '                                        Dim result As New VinLookupResult()
            '                                        If vins.Contains(lr.Vin) = False Then
            '                                            result.Description = lr.Description
            '                                            result.Make = lr.Make.ToUpper()
            '                                            result.Model = lr.Model.ToUpper()
            '                                            result.Year = lr.Year
            '                                            result.Vin = lr.Vin
            '                                            result.BodyTypeId = lr.BodyTypeId
            '                                            result.AntiTheftDescription = lr.AntiTheftDescription
            '                                            result.ResultVendor = lr.VehicleInfoLookupTypeId
            '                                            ' restraint is really a list
            '                                            If lr.RestraintDescription.Contains(",") Then
            '                                                result.RestraintDescription = lr.RestraintDescription.Split(",")(1).Trim()
            '                                            Else
            '                                                result.RestraintDescription = lr.RestraintDescription.Trim()
            '                                            End If

            '                                            If result.RestraintDescription.StartsWith("Side") Or result.RestraintDescription.Contains("Curtain") Or result.RestraintDescription.Contains("Head") Then
            '                                                result.RestraintDescription = "Side Airbags"
            '                                            End If

            '                                            result.PerformanceTypeText = lr.PerformanceDescription
            '                                            result.BodyTypeText = lr.BodyType
            '                                            result.CompSymbol = lr.IsoCompSymbol.Trim()
            '                                            result.CollisionSymbol = lr.IsoCollisionSymbol.Trim()

            '                                            result.ISOBodyStyle = ConvertVinLookupBodyStyleToVRBodyStyle(lr.ISOBodyStyle) ' Added 5-10-16 Matt A
            '                                            result.CyclinderCount = lr.Cylinders ' Added 5-10-16 Matt A
            '                                            result.CyclinderDescription = lr.CylindersDescription ' Added 5-10-16 Matt A

            '                                            If result.Model.ToUpper() = "FORTWO" AndAlso result.Make.ToUpper() = "UNDETERMINED" Then 'Matt A 8-10-2016 needed to fix 'Smart' cars
            '                                                result.Make = "SMART"
            '                                            End If

            '                                            'added for PARAchute
            '                                            Dim symbolResults = PerformVehicleSymbolPlanLookup(lr.ModelISOId, versionId)
            '                                            Dim libSymbol = (From s In symbolResults Where s.VehicleSymbolCoverageTypeId = 3 Select s).FirstOrDefault()
            '                                            If libSymbol IsNot Nothing Then
            '                                                result.LiabilitySymbol = libSymbol.Symbol
            '                                            End If

            '                                            vins.Add(lr.Vin)
            '                                            results.Add(result)
            '                                        End If

            '                                    Next
            '                                End If
            '                            End With
            '                        End If
            '                    End With
            '                End If
            '            Catch ex As Exception
            '#If DEBUG Then
            '                Debugger.Break()
            '#End If
            '            End Try

            '            Return (From r In results Order By r.Model Select r).ToList()

            Return QuickQuote.CommonMethods.QuickQuoteHelperClass.GetMakeModelYearOrVinVehicleInfo(Vin, make, model, year, effectiveDate, versionId)
        End Function

        Private Shared Function ConvertVinLookupBodyStyleToVRBodyStyle(ISOBodyStyle As String) As String
            'If ISOBodyStyle Is Nothing Then
            '    Return "CAR"
            'End If
            'If ISOBodyStyle.ToLower().Contains("pickup") Or ISOBodyStyle.ToLower().Contains("pkp") Or ISOBodyStyle.ToLower().Contains("shrt bed") Then
            '    Return "PICKUP W/O CAMPER"
            'Else
            '    If ISOBodyStyle.ToLower().Contains("sedan") Or ISOBodyStyle.ToLower().Contains("coupe") Or ISOBodyStyle.ToLower().Contains("wagon") Or ISOBodyStyle.ToLower().Contains("hatchback") Or ISOBodyStyle.ToLower().Contains("conv") Or ISOBodyStyle.ToLower().Contains("rdstr") Or ISOBodyStyle.ToLower().Contains("hchbk") Or ISOBodyStyle.ToLower().Contains("hrdtp") Or ISOBodyStyle.ToLower().Contains("sed") Or ISOBodyStyle.ToLower().Contains("wag") Then
            '        Return "CAR"
            '    Else
            '        If ISOBodyStyle.ToLower().Contains("van") Then
            '            Return "VAN"
            '        Else
            '            If ISOBodyStyle.ToLower().Contains("utility") Or ISOBodyStyle.ToLower().Contains("util") Or ISOBodyStyle.ToLower().Contains("utl") Then
            '                Return "SUV"
            '            Else
            '                Return "CAR"
            '            End If
            '        End If
            '    End If
            'End If

            Return QuickQuote.CommonMethods.QuickQuoteHelperClass.ConvertVinLookupBodyStyleToVRBodyStyle(ISOBodyStyle)
        End Function       

        Private Shared Function PerformVehicleSymbolPlanLookup(ByVal vehiclelookupId As Integer, ByVal versionId As Integer) As DCO.InsCollection(Of DCO.VehicleSymbolPlanLookup.VehicleSymbolPlanLookupResults)

            'Dim request As ServiceMessages.VinService.VehicleSymbolPlanLookup.Request =
            '    BuildVehicleSymbolPlanLookupRequest(vehiclelookupId, versionId)

            'If request IsNot Nothing Then
            '    Dim response As ServiceMessages.VinService.VehicleSymbolPlanLookup.Response = Nothing

            '    Using proxy As New Diamond.Common.Services.Proxies.VinServiceProxy
            '        response = proxy.VehicleSymbolPlanLookup(request)
            '    End Using

            '    Return response.ResponseData.VehicleSymbolPlanLookupResults
            'End If
            'Return Nothing

            Return QuickQuote.CommonMethods.QuickQuoteHelperClass.PerformVehicleSymbolPlanLookup(vehiclelookupId, versionId)
        End Function

        Private Shared Function BuildVehicleSymbolPlanLookupRequest(ByVal lookupId As Integer, ByVal versionId As Integer) As ServiceMessages.VinService.VehicleSymbolPlanLookup.Request
            'Dim request As ServiceMessages.VinService.VehicleSymbolPlanLookup.Request = Nothing
            'Dim lookupSource As DCE.VehicleInfoLookupType.VehicleInfoLookupType = DCE.VehicleInfoLookupType.VehicleInfoLookupType.ModelISORAPA

            'request = New ServiceMessages.VinService.VehicleSymbolPlanLookup.Request

            'With request.RequestData
            '    .LookupSource = lookupSource
            '    .ModelISOId = lookupId
            '    .VersionId = versionId
            '    .MakeModelLookupType = DCE.Vin.MakeModelLookupType.LookupUsingVin
            'End With
            'Return request

            Return QuickQuote.CommonMethods.QuickQuoteHelperClass.BuildVehicleSymbolPlanLookupRequest(lookupId, versionId)
        End Function

    End Class

End Namespace