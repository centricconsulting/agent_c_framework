'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.SupportService
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common

Namespace Services.Diamond.SupportService
    Public Module SupportServiceProxy
        'Public Function AutoEndt(ByRef res As DSCM.AutoEndt.Response,
        '                         ByRef req As DSCM.AutoEndt.Request,
        '                         Optional ByRef e As Exception = Nothing) As Boolean
        '    Dim p As New DCSP.SupportServiceProxy
        '    Dim m As Services.common.pMethod = AddressOf p.AutoEndt
        '   res = RunDiamondService(m, req, e)
        '    If res IsNot Nothing Then
        '        Return True
        '    Else
        '        Return False
        '    End If
        'End Function

        Public Function CheckStatusReset(ByRef res As DSCM.CheckStatusReset.Response,
                                         ByRef req As DSCM.CheckStatusReset.Request,
                                         Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.SupportServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.CheckStatusReset
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function ResetPrint(ByRef res As DSCM.ResetPrint.Response,
                                   ByRef req As DSCM.ResetPrint.Request,
                                   Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.SupportServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.ResetPrint
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace