'DO NOT USE THIS SERVICE
Imports Microsoft.VisualBasic
Imports DCSC = Diamond.Common.Services.Messages.CLMClassData
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFM.DiamondServices.Services.Common
Namespace Services.Diamond.CLMClass
    Public Module CLMClass
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="res"></param>
        ''' <param name="req"></param>
        ''' <param name="e"></param>
        ''' <returns></returns>
        ''' <remarks>diamond says don't use it</remarks>
        Public Function GetPolicyCancellationInformation(ByRef res As DCSC.DoSearch.Response,
                                                 ByRef req As DCSC.DoSearch.Request,
                                                 Optional ByRef e As Exception = Nothing) As Boolean
            Dim p As New DCSP.CLMClassServiceProxy
            Dim m As Services.common.pMethod = AddressOf p.DoSearch
            res = RunDiamondService(m, req, e)
            If res IsNot Nothing Then
                Return True
            Else
                Return False
            End If
        End Function
    End Module
End Namespace
