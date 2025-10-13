Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCSO = Diamond.Common.StaticDataManager.Objects
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.VinService
    Public Module VinService
        Public Function DeleteModelSqlScript(ModelScriptID As Integer,
                                             Optional ByRef e As Exception = Nothing,
                                             Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.VinService.DeleteModelSqlScript.Request
            Dim res As New DCSM.VinService.DeleteModelSqlScript.Response

            With req.RequestData
                .ModelScriptID = ModelScriptID
            End With

            If (IFMS.VinService.DeleteModelSqlScript(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.OperationSuccessful
                End If
            End If

            Return Nothing
        End Function

        Public Function EditISOModel(ModelIso As DCO.VehicleInfoLookup.ModelISOLookup,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.VinService.EditISOModel.Request
            Dim res As New DCSM.VinService.EditISOModel.Response

            With req.RequestData
                .ModelIso = ModelIso
            End With

            If (IFMS.VinService.EditISOModel(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.OperationSuccessful
                End If
            End If

            Return Nothing
        End Function

        Public Function GetVehicleInfoLookupMappings(MappingItems As DCO.InsCollection(Of DCSM.VinService.GetVehicleInfoLookupMappings.MappingItem),
                                                     Optional ByRef e As Exception = Nothing,
                                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.VinService.GetVehicleInfoLookupMappings.MappingItem)
            Dim req As New DCSM.VinService.GetVehicleInfoLookupMappings.Request
            Dim res As New DCSM.VinService.GetVehicleInfoLookupMappings.Response

            With req.RequestData
                .MappingItems = MappingItems
            End With

            If (IFMS.VinService.GetVehicleInfoLookupMappings(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.MappingItems
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadMakes(VehicleInfoLookupTypeId As DCE.VehicleInfoLookupType.VehicleInfoLookupType,
                                  Optional ByRef e As Exception = Nothing,
                                  Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.VinService.LoadMakes.Make)
            Dim req As New DCSM.VinService.LoadMakes.Request
            Dim res As New DCSM.VinService.LoadMakes.Response

            With req.RequestData
                .VehicleInfoLookupTypeId = VehicleInfoLookupTypeId
            End With

            If (IFMS.VinService.LoadMakes(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.Items
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadModelSqlScript(Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.Policy.ModelSQLScript)
            Dim req As New DCSM.VinService.LoadModelSqlScript.Request
            Dim res As New DCSM.VinService.LoadModelSqlScript.Response

            With req.RequestData
            End With

            If (IFMS.VinService.LoadModelSqlScript(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.LoadModelSqlScriptRecords
                End If
            End If

            Return Nothing
        End Function

        Public Function LoadVehicleInfoLookupTypes(Optional ByRef e As Exception = Nothing,
                                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSO.VersionData.VehicleInfoLookupType)
            Dim req As New DCSM.VinService.LoadVehicleInfoLookupTypes.Request
            Dim res As New DCSM.VinService.LoadVehicleInfoLookupTypes.Response

            With req.RequestData
            End With

            If (IFMS.VinService.LoadVehicleInfoLookupTypes(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.VehicleInfoLookupTypes
                End If
            End If

            Return Nothing
        End Function

        Public Function ModelISOLoad(Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.VehicleInfoLookup.ModelISOLookup)
            Dim req As New DCSM.VinService.ModelISOLoad.Request
            Dim res As New DCSM.VinService.ModelISOLoad.Response

            With req.RequestData
            End With

            If (IFMS.VinService.ModelISOLoad(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ModelISO
                End If
            End If

            Return Nothing
        End Function

        Public Function ModelISOLoadNextRecords(Vin As String,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.VehicleInfoLookup.ModelISOLookup)
            Dim req As New DCSM.VinService.ModelISOLoadNextRecords.Request
            Dim res As New DCSM.VinService.ModelISOLoadNextRecords.Response

            With req.RequestData
                .Vin = Vin
            End With

            If (IFMS.VinService.ModelISOLoadNextRecords(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ModelISO
                End If
            End If

            Return Nothing
        End Function

        Public Function ModelISOLoadPrevRecords(Vin As String,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.VehicleInfoLookup.ModelISOLookup)
            Dim req As New DCSM.VinService.ModelISOLoadPrevRecords.Request
            Dim res As New DCSM.VinService.ModelISOLoadPrevRecords.Response

            With req.RequestData
                .Vin = Vin
            End With

            If (IFMS.VinService.ModelISOLoadPrevRecords(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ModelISO
                End If
            End If

            Return Nothing
        End Function

        Public Function ModelISOSave(ModelISOSaves As DCO.VehicleInfoLookup.ModelISOLookup,
                                     Optional ByRef e As Exception = Nothing,
                                     Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.VinService.ModelISOSave.Request
            Dim res As New DCSM.VinService.ModelISOSave.Response

            With req.RequestData
                .ModelISOSave = ModelISOSaves
            End With

            If (IFMS.VinService.ModelISOSave(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.OperationSuccessful
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessIsoUpdate(FileName As String,
                                         Records As String(),
                                         Type As DCE.VehicleInfoLookupType.VehicleInfoLookupType,
                                         ValidateOnly As Boolean,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCSM.VinService.ProcessIsoUpdate.ErrorRecordInformation)
            Dim req As New DCSM.VinService.ProcessIsoUpdate.Request
            Dim res As New DCSM.VinService.ProcessIsoUpdate.Response

            With req.RequestData
                .FileName = FileName
                .Records = Records
                .Type = Type
                .ValidateOnly = ValidateOnly
            End With

            If (IFMS.VinService.ProcessIsoUpdate(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ErrorRecords
                End If
            End If

            Return Nothing
        End Function

        Public Function ProcessModelScript(ModelScriptID As Long,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.VehicleInfoLookup.ModelISOLookup)
            Dim req As New DCSM.VinService.ProcessModelScript.Request
            Dim res As New DCSM.VinService.ProcessModelScript.Response

            With req.RequestData
                .ModelScriptID = ModelScriptID
            End With

            If (IFMS.VinService.ProcessModelScript(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.ProcessModelScript
                End If
            End If

            Return Nothing
        End Function

        Public Function SaveModelSqlScript(SaveModel As DCO.Policy.ModelSQLScript,
                                           Optional ByRef e As Exception = Nothing,
                                           Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.ModelSQLScript
            Dim req As New DCSM.VinService.SaveModelSqlScript.Request
            Dim res As New DCSM.VinService.SaveModelSqlScript.Response

            With req.RequestData
                .SaveModel = SaveModel
            End With

            If (IFMS.VinService.SaveModelSqlScript(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.SaveModelSqlScriptRecords
                End If
            End If

            Return Nothing
        End Function

        Public Function SaveRedbookEntry(Entry As DCO.VehicleInfoLookup.RedbookLookup,
                                         Optional ByRef e As Exception = Nothing,
                                         Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim req As New DCSM.VinService.SaveRedbookEntry.Request
            Dim res As New DCSM.VinService.SaveRedbookEntry.Response

            With req.RequestData
                .Entry = Entry
            End With

            If (IFMS.VinService.SaveRedbookEntry(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.OperationSuccessful
                End If
            End If

            Return Nothing
        End Function

        Public Function VehicleInfoLookup(LookupSource As DCE.VehicleInfoLookupType.VehicleInfoLookupType,
                                          Make As String,
                                          Model As String,
                                          SerialNumber As String,
                                          Type As DCE.Vin.MakeModelLookupType,
                                          Vin As String,
                                          Year As Integer,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As List(Of DCO.VehicleInfoLookup.VehicleInfoLookupResults)
            Dim req As New DCSM.VinService.VehicleInfoLookup.Request
            Dim res As New DCSM.VinService.VehicleInfoLookup.Response

            With req.RequestData
                .LookupSource = LookupSource
                .Make = Make
                .Model = Model
                .SerialNumber = SerialNumber
                .Type = Type
                .Vin = Vin
                .Year = Year
                'If String.IsNullOrEmpty(Vin) Then
                '    .Make = Make
                '    .Model = Model
                '    .Year = Year
                '    .Type = DCE.Vin.MakeModelLookupType.LookupUsingYearMakeModel
                'Else
                '    .Vin = Vin
                '    .Type = DCE.Vin.MakeModelLookupType.LookupUsingVin
                'End If
            End With

            Dim results As New List(Of DCO.VehicleInfoLookup.VehicleInfoLookupResults)

            If (IFMS.VinService.VehicleInfoLookup(res, req, e, dv)) Then
                If (res.ResponseData.VehicleInfoLookupResults IsNot Nothing) Then
                    For Each lr As DCO.VehicleInfoLookup.VehicleInfoLookupResults In res.ResponseData.VehicleInfoLookupResults
                        Dim result As New DCO.VehicleInfoLookup.VehicleInfoLookupResults
                        With lr
                            result.Make = .Make
                            result.Model = .Model
                            result.Year = .Year
                        End With

                        results.Add(result)
                    Next
                End If
            End If

            Return results
        End Function

        Public Function VehicleSymbolPlanLookup(LookupSource As DCE.VehicleInfoLookupType.VehicleInfoLookupType,
                                                ModelISOId As Integer,
                                                VersionId As Integer,
                                                Optional ByRef e As Exception = Nothing,
                                                Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.VehicleSymbolPlanLookup.VehicleSymbolPlanLookupResults)
            Dim req As New DCSM.VinService.VehicleSymbolPlanLookup.Request
            Dim res As New DCSM.VinService.VehicleSymbolPlanLookup.Response

            With req.RequestData
                .LookupSource = LookupSource
                .ModelISOId = ModelISOId
                .VersionId = VersionId
            End With

            If (IFMS.VinService.VehicleSymbolPlanLookup(res, req, e, dv)) Then
                If (res.ResponseData IsNot Nothing) Then
                    Return res.ResponseData.VehicleSymbolPlanLookupResults
                End If
            End If

            Return Nothing
        End Function
    End Module
End Namespace