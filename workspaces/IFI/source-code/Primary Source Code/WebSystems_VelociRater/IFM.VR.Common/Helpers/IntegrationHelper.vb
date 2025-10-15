Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers

    Public Class IntegrationHelper

        Public Function GetRACASymbolsForVin(vin As String) As List(Of RACASymbolsLookupResult)
            Dim chc As New CommonHelperClass
            Dim apiKey As String = chc.ConfigurationAppSettingValueAsString("VR_CAP_RACASymbols_APIKey")
            Dim baseUrl As String = chc.ConfigurationAppSettingValueAsString("VR_CAP_RACASymbols_BaseURL")
            Dim symbolRequest As New IFI.Integrations.Request.RACASymbols(baseUrl, apiKey)
            Dim results As New List(Of RACASymbolsLookupResult)
            Dim unhandledExceptionToString As String = ""
            Dim vins As New List(Of String) From {
                vin
            }

            Try
                With symbolRequest
                    .Vin = vins
                End With

                Dim symbolResponse = symbolRequest.GetVendorData()
                If symbolResponse IsNot Nothing Then
                    'Dim applicationRequestId = symbolResponse.applicationRequestId 'Keeping this in case business decides we need it later
                    With symbolResponse
                        If .responseData IsNot Nothing Then
                            With .responseData
                                If .Vins IsNot Nothing AndAlso .Vins.Count > 0 Then
                                    For Each vinResult In .Vins
                                        Dim compSymbol As String = vinResult.Vin.Body.ComprehensiveSymbol
                                        Dim collSymbol As String = vinResult.Vin.Body.CollisionSymbol
                                        Dim liabSymbol As String = vinResult.Vin.Body.LiabilitySymbol
                                        Dim result As New RACASymbolsLookupResult()
                                        If IsNullEmptyorWhitespace(compSymbol) Then
                                            compSymbol = CAP.RACASymbolHelper.notFoundSymbolCode
                                        End If
                                        If IsNullEmptyorWhitespace(collSymbol) Then
                                            collSymbol = CAP.RACASymbolHelper.notFoundSymbolCode
                                        End If
                                        If IsNullEmptyorWhitespace(liabSymbol) Then
                                            liabSymbol = CAP.RACASymbolHelper.notFoundSymbolCode
                                        End If
                                        result.ComprehensiveSymbol = compSymbol
                                        result.CollisionSymbol = collSymbol
                                        result.LiabilitySymbol = liabSymbol
                                        results.Add(result)
                                        'only ever sending one vin, so will only ever have one result
                                        Exit For
                                    Next
                                End If
                            End With
                        Else
                            Dim noHit As New RACASymbolsLookupResult()
                            noHit.ComprehensiveSymbol = CAP.RACASymbolHelper.notFoundSymbolCode
                            noHit.CollisionSymbol = CAP.RACASymbolHelper.notFoundSymbolCode
                            noHit.LiabilitySymbol = CAP.RACASymbolHelper.notFoundSymbolCode
                            results.Add(noHit)
                        End If
                    End With

                Else 'added so we don't think it was just a NoHit; this would happen if there's an issue w/ the SnapLogic endpoint that's being suppressed by the helper library
                    If symbolRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(symbolRequest.InternalExceptionMessages) Then
                        unhandledExceptionToString = "No Response from Integration Service" & "; " & symbolRequest.InternalExceptionMessages
                    Else
                        unhandledExceptionToString = "No Response from Integration Service"
                    End If
                    IFM.IFMErrorLogging.LogIssue("GetRACASymbols API", "No Response: " & unhandledExceptionToString)
                End If
            Catch ex As Exception
                If symbolRequest.InternalException IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(symbolRequest.InternalExceptionMessages) Then
                    unhandledExceptionToString = ex.ToString & "; " & symbolRequest.InternalExceptionMessages
                Else
                    unhandledExceptionToString = ex.ToString
                End If
                IFM.IFMErrorLogging.LogIssue("GetRACASymbols API", "Unhandled Exception: " & unhandledExceptionToString)
            End Try
            Return results
        End Function
    End Class

    Public Class RACASymbolsLookupResult
        Public Property ComprehensiveSymbol As String = ""
        Public Property CollisionSymbol As String = ""
        Public Property LiabilitySymbol As String = ""
    End Class
End Namespace
