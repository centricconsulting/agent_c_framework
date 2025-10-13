'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSA = Diamond.Common.Services.Messages.AutoService
Imports DCSP = Diamond.Common.Services.Proxies
Imports DCO = Diamond.Common.Objects
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.Auto
    Friend Module Auto
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="res"></param>
        ''' <param name="req"></param>
        ''' <param name="e"></param>
        ''' <returns></returns>
        ''' <remarks>Not a recommended call.. use add delete</remarks>
        Public Function ReplaceVehicle(ByRef res As DCSA.ReplaceVehicle.Response,
                                  ByRef req As DCSA.ReplaceVehicle.Request,
                                  Optional ByRef e As Exception = Nothing, Optional ByRef dv As DCO.DiamondValidation = Nothing) As Boolean
            Dim p As New DCSP.AutoServices.AutoServiceProxy
            Dim m As Services.Common.pMethod = AddressOf p.ReplaceVehicle
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module

End Namespace
