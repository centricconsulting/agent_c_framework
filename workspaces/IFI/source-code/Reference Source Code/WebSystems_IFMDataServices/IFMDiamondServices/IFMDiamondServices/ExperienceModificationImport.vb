Imports Microsoft.VisualBasic
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond
Namespace Services.ExperienceModificationImport
    Public Module ExperienceModificationImport
        Public Function ImportRecords(File As DCO.ExperienceModifications.ExperienceModificationFile,
                                      Optional ByRef e As System.Exception = Nothing,
                                      Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCSM.ExperienceModificationImportService.ImportRecords.ResponseData
            Dim res As New DCSM.ExperienceModificationImportService.ImportRecords.Response
            Dim req As New DCSM.ExperienceModificationImportService.ImportRecords.Request

            With req.RequestData
                .File = File
            End With

            If IFMS.ExperienceModificationImport.ImportRecords(res, req, e, dv) Then
                Return res.ResponseData
            End If
            Return Nothing
        End Function

        Public Function LoadRecordList(ImportStatus As DCE.ExperienceModifications.ExperienceModificationImportStatus,
                                       Optional ByRef e As System.Exception = Nothing,
                                       Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.InsCollection(Of DCO.ExperienceModifications.RecordListItem)
            Dim res As New DCSM.ExperienceModificationImportService.LoadRecordList.Response
            Dim req As New DCSM.ExperienceModificationImportService.LoadRecordList.Request

            With req.RequestData
                .ImportStatus = ImportStatus
            End With

            If IFMS.ExperienceModificationImport.LoadRecordList(res, req, e, dv) Then
                Return res.ResponseData.Records
            End If
            Return Nothing
        End Function
    End Module
End Namespace