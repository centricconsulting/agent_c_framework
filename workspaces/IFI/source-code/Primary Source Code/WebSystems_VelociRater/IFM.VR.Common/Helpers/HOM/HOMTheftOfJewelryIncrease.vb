Imports QuickQuote.CommonObjects
Namespace IFM.VR.Common.Helpers.HOM
    Public Class HOMTheftOfJewelryIncrease
    Private Shared _HOMTheftOfJewelrySettings As NewFlagItem
        Public Shared ReadOnly Property HOMTheftOfJewelrySettings() As NewFlagItem
            Get
                If _HOMTheftOfJewelrySettings Is Nothing Then
                    _HOMTheftOfJewelrySettings = New NewFlagItem("VR_HOM_TheftOfJewelryIncrease_Settings")
                End If
                Return _HOMTheftOfJewelrySettings
            End Get
        End Property

        Public Shared Function HOMTheftOfJewelryEnabled() As Boolean
            Return HOMTheftOfJewelrySettings.EnabledFlag
        End Function

        Public Shared Function HOMTheftOfJewelryEffDate() As Date
            Return HOMTheftOfJewelrySettings.GetStartDateOrDefault("1/1/1800")
        End Function

        Public Shared Sub UpdateHOMTheftOfJewelry(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Show increased limit and total                   
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Included limit, no increased limit
            End Select
        End Sub

        Public Shared Function IsHOMTheftOfJewelryIncreaseAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, HOMTheftOfJewelrySettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class   
End Namespace
