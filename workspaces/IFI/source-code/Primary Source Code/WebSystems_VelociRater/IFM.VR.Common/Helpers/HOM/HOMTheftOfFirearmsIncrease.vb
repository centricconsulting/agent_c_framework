Imports QuickQuote.CommonObjects
Namespace IFM.VR.Common.Helpers.HOM
    Public Class HOMTheftOfFirearmsIncrease
        Private Shared _HOMTheftOfFirearmsSettings As NewFlagItem
        Public Shared ReadOnly Property HOMTheftOfFirearmsSettings() As NewFlagItem
            Get
                If _HOMTheftOfFirearmsSettings Is Nothing Then
                    _HOMTheftOfFirearmsSettings = New NewFlagItem("VR_HOM_TheftOfFirearmsIncrease_Settings")
                End If
                Return _HOMTheftOfFirearmsSettings
            End Get
        End Property

        Public Shared Function HOMTheftOfFirearmsEnabled() As Boolean
            Return HOMTheftOfFirearmsSettings.EnabledFlag
        End Function

        Public Shared Function HOMTheftOfFirearmsEffDate() As Date
            Return HOMTheftOfFirearmsSettings.GetStartDateOrDefault("1/1/1800")
        End Function

        Public Shared Sub UpdateHOMTheftOfFirearms(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Show increased limit and total                   
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Included limit, no increased limit
            End Select
        End Sub

        Public Shared Function IsHOMTheftOfFirearmsIncreaseAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, HOMTheftOfFirearmsSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
