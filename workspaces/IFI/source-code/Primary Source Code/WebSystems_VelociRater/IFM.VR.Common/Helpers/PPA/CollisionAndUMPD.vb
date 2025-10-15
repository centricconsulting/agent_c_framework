Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.PPA
    Public Class CollisionAndUMPD
        Private Shared _ILCollisionAndUMPDSettings As NewFlagItem
        Public Shared ReadOnly Property ILCollisionAndUMPDSettings() As NewFlagItem
            Get
                If _ILCollisionAndUMPDSettings Is Nothing Then
                    _ILCollisionAndUMPDSettings = New NewFlagItem("VR_PPA_ILCollisionAndUMPD_Settings")
                End If
                Return _ILCollisionAndUMPDSettings
            End Get
        End Property

        Public Shared Function ILCollisionAndUMPDEnabled() As Boolean
            Return ILCollisionAndUMPDSettings.EnabledFlag
        End Function

        Public Shared Function ILCollisionAndUMPDEffDate() As Date
            Return ILCollisionAndUMPDSettings.GetStartDateOrDefault("1/1/2023")
        End Function

        Public Shared Sub UpdateILCollisionAndUMPD(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Allow Collision and UMPD for IL - UMPD checkbox enabled, Collision drop down enabled       
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Do not allow both Collision and UMPD for IL - Remove UMPD if Collision selected
                    Dim qqHelper As QuickQuote.CommonMethods.QuickQuoteHelperClass = New QuickQuote.CommonMethods.QuickQuoteHelperClass
                    If Quote IsNot Nothing AndAlso Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                        For Each l In Quote.Vehicles
                            If l.CollisionDeductibleLimitId <> "0" OrElse IsNullEmptyorWhitespace(l.CollisionDeductibleLimitId) = False Then
                                'Has Collision
                                l.UninsuredMotoristPropertyDamageLimitId = ""
                            End If
                        Next
                    End If
                    'for each vehicle in list
                    'if hasCollision and hasUMPD then ' vehicle.CollisionDeductibleLimitId & vehicle.UninsuredMotoristPropertyDamageLimitId
                    'hasUMPD = false 'vehicle.UninsuredMotoristPropertyDamageLimitId = ""
            End Select
        End Sub

        Public Shared Function IsILCollisionAndUMPDAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, ILCollisionAndUMPDSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
