Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.HOM

    Public Class FamilyCyberProtectionHelper
         Private Shared _FamilyCyberProtectionSettings As NewFlagItem
        Public Shared ReadOnly Property FamilyCyberProtectionSettings() As NewFlagItem
            Get
                If _FamilyCyberProtectionSettings Is Nothing Then
                    _FamilyCyberProtectionSettings = New NewFlagItem("VR_HOM_FamilyCyberProtection_Settings")
                End If
                Return _FamilyCyberProtectionSettings
            End Get
        End Property

        Public Shared Function FamilyCyberProtectionEnabled() As Boolean
            Return FamilyCyberProtectionSettings.EnabledFlag
        End Function

        Public Shared Function FamilyCyberProtectionEffDate() As Date
            Return FamilyCyberProtectionSettings.GetStartDateOrDefault("11/15/2024")
        End Function

        Public Shared Sub UpdateFamilyCyberProtection(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD

                Case Helper.EnumsHelper.CrossDirectionEnum.BACK

            End Select
        End Sub

        Public Shared Function IsFamilyCyberProtectionAvailable(ByRef Quote As QuickQuoteObject)
            If Quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(Quote, FamilyCyberProtectionSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

    End Class
End Namespace