'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.StatsExportService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.StatsExport
    Public Module StatsExportProxy
        Public Function LoadCompanyLOBStats(ByRef res As DSCM.LoadCompanyLOBStats.Response,
                                            ByRef req As DSCM.LoadCompanyLOBStats.Request,
                                            Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.StatsExportServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadCompanyLOBStats
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ProcessStats(ByRef res As DSCM.ProcessStats.Response,
                                     ByRef req As DSCM.ProcessStats.Request,
                                     Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.StatsExportServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ProcessStats
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace