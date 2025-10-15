Imports QuickQuote.CommonObjects
'Imports QuickQuote.CommonMethods
'Imports IFM.PrimativeExtensions
'Imports System.Web.UI.WebControls
Namespace IFM.VR.Common.Helpers.CPR
    'Added 10/20/2022 for task 77527 MLW
    Public Class CPR_InflationGuardHelper
        Private Shared _InflationGuardNo2Settings As NewFlagItem
        Public Shared ReadOnly Property InflationGuardNo2Settings() As NewFlagItem
            Get
                If _InflationGuardNo2Settings Is Nothing Then
                    _InflationGuardNo2Settings = New NewFlagItem("VR_CPR_CPP_InflationGuardNo2_Settings")
                End If
                Return _InflationGuardNo2Settings
            End Get
        End Property

        Public Shared Function InflationGuardNo2Enabled() As Boolean
            Return InflationGuardNo2Settings.EnabledFlag
        End Function

        Public Shared Function InflationGuardNo2EffDate() As Date
            Return InflationGuardNo2Settings.GetStartDateOrDefault("2/2/2023")
        End Function

        Public Shared Sub UpdateInflationGuard(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Set Inflation Guard to 4 (id 2) if currently 2 (id 1)
                    If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                        For each l In Quote.Locations
                            If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                                For each b In l.Buildings
                                    If b.InflationGuardTypeId = "1" Then
                                        b.InflationGuardTypeId = "2"
                                    End If
                                Next
                            End If
                        Next
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Set Inflation Guard to 2 (id 1) if currently 4 (id 2)
                    If Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                        For each l In Quote.Locations
                            If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                                For each b In l.Buildings
                                    If b.InflationGuardTypeId = "2" Then
                                        b.InflationGuardTypeId = "1"
                                    End If
                                Next
                            End If
                        Next
                    End If
            End Select
        End Sub

        Public Shared Function IsInflationGuardNo2Available(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, InflationGuardNo2Settings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
