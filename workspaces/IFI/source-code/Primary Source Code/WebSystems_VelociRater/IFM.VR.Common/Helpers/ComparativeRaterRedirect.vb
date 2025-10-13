Imports DCO = Diamond.Common.Objects


Public Class ComparativeRaterRedirect



    'Private Sub HandleIntegrationToken(integrationTokenQueryString As String)


    '    Dim integrationToken As DCO.ThirdParty.IntegrationToken = Nothing

    '    Using proxy As New Proxies.ThirdPartyServiceProxy
    '        Dim request As New ServiceMessages.ThirdPartyService.LoadIntegrationToken.Request
    '        request.RequestData.Token = integrationTokenQueryString ' Set the GUID provided in the query string

    '        Dim response As ServiceMessages.ThirdPartyService.LoadIntegrationToken.Response = proxy.LoadIntegrationToken(request)

    '        If response.ResponseData IsNot Nothing _
    '                AndAlso response.ResponseData.IntegrationToken IsNot Nothing Then
    '            integrationToken = response.ResponseData.IntegrationToken
    '        End If
    '    End Using

    '    If integrationToken Is Nothing Then
    '        ' The integration token has expired (by default the timeout is 30 minutes).
    '        Return
    '    End If

    '    Dim userId As Integer = integrationToken.UsersId ' Authenticated User Id from the comparative rater call
    '    Dim policyNumber As String = String.Empty

    '    Select Case integrationToken.IntegrationActionType
    '        Case DCE.IntegrationActionType.OpenPolicy
    '            ' For your application, you don't technically have to check the actiontype, since it will always be open policy
    '            policyNumber = integrationToken.CustomData1 ' The policynumber is stored here for OpenPolicy action
    '    End Select

    '    ' Now get the security token
    '    Dim securityToken As Diamond.Common.Services.DiamondSecurityToken

    '    Using proxy As New Proxies.LoginServiceProxy
    '        Dim request As New ServiceMessages.LoginService.GetDiamTokenForUsersId.Request
    '        request.RequestData.UsersId = DWBC.Common.IntegrationToken.UsersId
    '        request.RequestData.BusinessInterfaceSourceId = DCE.Security.BusinessInterfaceSourceType.DiamondAgencyPortal

    '        Dim response As ServiceMessages.LoginService.GetDiamTokenForUsersId.Response = proxy.GetDiamTokenForUsersId(request)

    '        If response.ResponseData IsNot Nothing Then
    '            securityToken = response.ResponseData.DiamondSecurityToken
    '        End If
    '    End Using

    '    ' Set the security token on DCS ProxyBase
    '    Diamond.Common.Services.Proxies.ProxyBase.DiamondSecurityToken = securityToken


    '    ' Load the policy number as you did before:
    'End Sub



End Class
