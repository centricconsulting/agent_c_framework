Imports Microsoft.VisualBasic
Imports DC = Diamond.Common
Imports DCE = Diamond.Common.Enums
Imports DCO = Diamond.Common.Objects
Imports DCS = Diamond.Common.Services
Imports DCSM = Diamond.Common.Services.Messages
Imports DCSP = Diamond.Common.Services.Proxies
Imports IFMS = IFM.DiamondServices.Services.Diamond

Namespace Services.Administration
    Public Module Administration
        Public Function LoadAgency(AgencyID As Integer,
                                   Optional ByRef e As System.Exception = Nothing,
                                   Optional ByRef dv As DCO.DiamondValidation = Nothing) As DCO.Policy.Agency.Agency

            Dim req As New DCSM.AdministrationService.LoadAgency.Request
            Dim res As New DCSM.AdministrationService.LoadAgency.Response
            req.RequestData.AgencyId = AgencyID
            If IFMS.Administration.LoadAgency(res, req, e, dv) Then
                If res.ResponseData IsNot Nothing Then
                    Return res.ResponseData.Agency
                End If
            End If
            Return Nothing
        End Function
    End Module
End Namespace

