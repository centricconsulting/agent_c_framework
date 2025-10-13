Imports IFM.VR.Common.Helpers.MultiState
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.PPA
    Public Class DriverListHelper
        Public Shared Sub DoAssignedDriverCheck(topQuote As QuickQuote.CommonObjects.QuickQuoteObject, DriverNumber As String)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
            If topQuote.Vehicles IsNot Nothing AndAlso topQuote.Vehicles.Count > 0 Then
                For Each v As QuickQuoteVehicle In topQuote.Vehicles

                    'or this is better suited to move drivers up (at least occasional drivers)
                    If v.OccasionalDriver3Num <> "" AndAlso IsNumeric(v.OccasionalDriver3Num) = True AndAlso CInt(v.OccasionalDriver3Num) = CInt(DriverNumber) Then
                        v.OccasionalDriver3Num = ""
                    End If
                    If v.OccasionalDriver2Num <> "" AndAlso IsNumeric(v.OccasionalDriver2Num) = True AndAlso CInt(v.OccasionalDriver2Num) = CInt(DriverNumber) Then
                        v.OccasionalDriver2Num = ""
                    End If
                    If v.OccasionalDriver1Num <> "" AndAlso IsNumeric(v.OccasionalDriver1Num) = True AndAlso CInt(v.OccasionalDriver1Num) = CInt(DriverNumber) Then
                        v.OccasionalDriver1Num = ""
                    End If
                    If v.PrincipalDriverNum <> "" AndAlso IsNumeric(v.PrincipalDriverNum) = True AndAlso CInt(v.PrincipalDriverNum) = CInt(DriverNumber) Then
                        v.PrincipalDriverNum = ""
                    End If

                    MoveUpVehicleDrivers(v)
                Next
            End If
        End Sub

        Private Shared Sub MoveUpVehicleDrivers(ByRef vehicle As QuickQuoteVehicle)
            If vehicle IsNot Nothing Then
                With vehicle
                    If .OccasionalDriver2Num = "" AndAlso .OccasionalDriver3Num <> "" Then
                        .OccasionalDriver2Num = .OccasionalDriver3Num
                        .OccasionalDriver3Num = ""
                    End If
                    If .OccasionalDriver1Num = "" AndAlso .OccasionalDriver2Num <> "" Then
                        .OccasionalDriver1Num = .OccasionalDriver2Num
                        .OccasionalDriver2Num = ""
                    End If
                    'this is optional (to replace principal w/ occasional)
                    If .PrincipalDriverNum = "" AndAlso .OccasionalDriver1Num <> "" Then
                        .PrincipalDriverNum = .OccasionalDriver1Num
                        .OccasionalDriver1Num = ""
                        If .OccasionalDriver2Num <> "" Then
                            .OccasionalDriver1Num = .OccasionalDriver2Num
                            .OccasionalDriver2Num = ""
                            If .OccasionalDriver3Num <> "" Then
                                .OccasionalDriver2Num = .OccasionalDriver3Num
                                .OccasionalDriver3Num = ""
                            End If
                        End If
                    End If
                End With
            End If
        End Sub
    End Class

End Namespace