'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCO = Diamond.Common.Objects
Imports DCE = Diamond.Common.Enums
Imports DCS = Diamond.Common.Services
Imports DSCM = Diamond.Common.Services.Messages.ModifierService
Imports DCSP = Diamond.Common.Services.Proxies.Modifiers
Imports IFM.DiamondServices.Services.Common


Namespace Services.Diamond.Modifiers
    Public Module NewsService
        Public Function LoadEditData(ByRef res As DSCM.LoadEditData.Response,
                                          ByRef req As DSCM.LoadEditData.Request,
                                          Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ModifierServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.LoadEditData
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function SaveEditData(ByRef res As DSCM.SaveEditData.Response,
                                                      ByRef req As DSCM.SaveEditData.Request,
                                                      Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.ModifierServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.SaveEditData
           res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace