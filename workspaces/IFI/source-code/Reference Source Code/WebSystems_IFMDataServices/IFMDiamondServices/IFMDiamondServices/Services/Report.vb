Imports Microsoft.VisualBasic
Imports DCSM = Diamond.Common.Services.Messages.ReportService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.Report
    Public Module Report
        Public Function LoadReportDetails(ByRef res As DCSM.LoadReportDetails.Response,
                                          ByRef req As DCSM.LoadReportDetails.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ReportServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadReportDetails
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Function LoadReportList(ByRef res As DCSM.LoadReportList.Response,
                                          ByRef req As DCSM.LoadReportList.Request,
                                          Optional ByRef e As Exception = Nothing,
                                          Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.ReportServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.LoadReportList
            res = RunDiamondService(m, req, e, dv)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace

