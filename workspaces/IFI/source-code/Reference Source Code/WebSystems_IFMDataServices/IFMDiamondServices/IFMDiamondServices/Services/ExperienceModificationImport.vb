Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.ExperienceModificationImportService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.ExperienceModificationImport
    Public Module ExperienceModificationImport
        Public Function ImportRecords(ByRef res As DCSM.ImportRecords.Response,
                                          ByRef req As DCSM.ImportRecords.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ExperienceModificationImportServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ImportRecords
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadRecordList(ByRef res As DCSM.LoadRecordList.Response,
                                          ByRef req As DCSM.LoadRecordList.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ExperienceModificationImportServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadRecordList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace


