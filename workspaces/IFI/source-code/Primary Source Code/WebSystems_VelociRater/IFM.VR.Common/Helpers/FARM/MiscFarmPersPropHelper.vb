Imports QuickQuote.CommonObjects
Namespace IFM.VR.Common.Helpers.FARM
    Public Class MiscFarmPersPropHelper
        Private Shared _MiscFarmPersPropSettings As NewFlagItem
        Public Shared ReadOnly Property MiscFarmPersPropSettings() As NewFlagItem
            Get
                If _MiscFarmPersPropSettings Is Nothing Then
                    _MiscFarmPersPropSettings = New NewFlagItem("VR_FAR_MiscFarPersProp_Settings")
                End If
                Return _MiscFarmPersPropSettings
            End Get
        End Property

        Public Shared Function MiscFarmPersPropEnabled() As Boolean
            Return MiscFarmPersPropSettings.EnabledFlag
        End Function

        Public Shared Function MiscFarmPersPropEffDate() As Date
            Return MiscFarmPersPropSettings.GetStartDateOrDefault("1/1/1800")
        End Function

        Public Shared Sub UpdateMiscFarmPersProperty(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Show coverage                  
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Hide coverage
            End Select
        End Sub

        Public Shared Function IsMiscFarmPersPropAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, MiscFarmPersPropSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
