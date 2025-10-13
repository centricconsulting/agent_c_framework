Imports QuickQuote.CommonObjects

Namespace IFM.VR.Common.Helpers.CAP
    Public Class CompCollDeductibleHelper
        Inherits FeatureFlagBase

        Private Shared _CompCollDeductibleSettings As NewFlagItem
        Public Shared ReadOnly Property CompCollDeductibleSettings() As NewFlagItem
            Get
                If _CompCollDeductibleSettings Is Nothing Then
                    _CompCollDeductibleSettings = New NewFlagItem("VR_CAP_CompCollDeductible_Settings")
                End If
                Return _CompCollDeductibleSettings
            End Get
        End Property

        Public Shared Function CompCollDeductibleEnabled() As Boolean
            Return CompCollDeductibleSettings.EnabledFlag
        End Function

        Public Shared Function CompCollDeductibleEffDate() As Date
            Return CompCollDeductibleSettings.GetStartDateOrDefault("10/1/2025")
        End Function

        Const HiredCarPhysDamageCollDeductibleMsg As String = "We've updated the Hired Car Physical Damage Collision Deductible to meet our minimum requirement of $500."
        Const GarageKeepersCompDeductibleMsg As String = "We've updated the Garage Keepers Comprehensive Deductible to meet our minimum requirement of $500."
        Const GarageKeepersCollDeductibleMsg As String = "We've updated the Garage Keepers Collision Deductible to meet our minimum requirement of $500."
        Const VehCompDeductibleMsg As String = "We've updated the vehicle's Comprehensive Deductible to meet our minimum requirement of $500."
        Const VehCollDeductibleMsg As String = "We've updated the vehicle's Collision Deductible to meet our minimum requirement of $500."

        Public Shared Sub UpdateCompCollDeductible(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            If Quote IsNot Nothing Then
                Select Case CrossDirection
                    Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                        Dim NeedsHCPDCollWarningMessage As Boolean = False
                        Dim NeedsGKCompWarningMessage As Boolean = False
                        Dim NeedsGKCollWarningMessage As Boolean = False
                        Dim NeedsVehCompWarningMessage As Boolean = False
                        Dim NeedsVehCollWarningMessage As Boolean = False
                        Dim SubQuotes As List(Of QuickQuoteObject) = MultiState.General.SubQuotes(Quote)
                        If SubQuotes IsNot Nothing And SubQuotes.Count > 0 Then
                            For Each sq In SubQuotes
                                'set lower deductibles to 500 (8) default
                                'Hired Car Physical Damage Coll
                                Select Case sq.CollisionDeductibleId
                                    Case "2", "4"
                                        '2=100, 4=250
                                        sq.CollisionDeductibleId = "8" '500
                                        NeedsHCPDCollWarningMessage = True
                                End Select
                                'Garage Keepers Comp
                                Select Case sq.GarageKeepersOtherThanCollisionDeductibleId
                                    Case "5", "6"
                                        '5=100/500, 6=250/1000
                                        sq.GarageKeepersOtherThanCollisionDeductibleId = "7" '500/2500
                                        NeedsGKCompWarningMessage = True
                                End Select
                                'Garage Keepers Coll
                                Select Case sq.GarageKeepersCollisionDeductibleId
                                    Case "2", "4"
                                        '2=100, 4=250
                                        sq.GarageKeepersCollisionDeductibleId = "8" '500
                                        NeedsGKCollWarningMessage = True
                                End Select
                            Next
                        End If

                        If Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                            For Each vehicle In Quote.Vehicles
                                'Vehicle Comp
                                If vehicle.ComprehensiveDeductibleId = "4" Then
                                    '4=250
                                    vehicle.ComprehensiveDeductibleId = "8" '500
                                    NeedsVehCompWarningMessage = True
                                End If
                                'Vehicle Coll
                                If vehicle.CollisionDeductibleId = "4" Then
                                    '4=250
                                    vehicle.CollisionDeductibleId = "8" '500
                                    NeedsVehCollWarningMessage = True
                                End If
                            Next
                        End If
                        If NeedsHCPDCollWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                            Dim i = New WebValidationItem(HiredCarPhysDamageCollDeductibleMsg)
                            i.IsWarning = True
                            ValidationErrors.Add(i)
                        End If
                        If NeedsGKCompWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                            Dim i = New WebValidationItem(GarageKeepersCompDeductibleMsg)
                            i.IsWarning = True
                            ValidationErrors.Add(i)
                        End If
                        If NeedsGKCollWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                            Dim i = New WebValidationItem(GarageKeepersCollDeductibleMsg)
                            i.IsWarning = True
                            ValidationErrors.Add(i)
                        End If
                        If NeedsVehCompWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                            Dim i = New WebValidationItem(VehCompDeductibleMsg)
                            i.IsWarning = True
                            ValidationErrors.Add(i)
                        End If
                        If NeedsVehCollWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                            Dim i = New WebValidationItem(VehCollDeductibleMsg)
                            i.IsWarning = True
                            ValidationErrors.Add(i)
                        End If
                    Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                        'Do nothing, lower deductibles allowed
                End Select
            End If
        End Sub

        Public Shared Function IsCompCollDeductibleAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, CompCollDeductibleSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

    End Class
End Namespace
