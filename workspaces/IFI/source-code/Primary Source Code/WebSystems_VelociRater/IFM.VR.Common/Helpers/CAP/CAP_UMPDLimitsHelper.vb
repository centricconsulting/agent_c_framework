Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers.CAP
    Public Class CAP_UMPDLimitsHelper
        Private Shared _UMPDLimitsSettings As NewFlagItem
        Public Shared ReadOnly Property UMPDLimitsSettings() As NewFlagItem
            Get
                If _UMPDLimitsSettings Is Nothing Then
                    _UMPDLimitsSettings = New NewFlagItem("VR_CAP_UMPDLimits_Settings")
                End If
                Return _UMPDLimitsSettings
End Get
End Property

        Const UMPDLimitsNotAvailMsg As String = "Updated UMPD limits are not available prior to 12/01/2023. UMPD limit will be changed to the included $15,000 limit for quotes amended to an effective date prior to 12/01/2023."  

        Public Shared Function UMPDLimitsEnabled() As Boolean
            Return UMPDLimitsSettings.EnabledFlag
        End Function

        Public Shared Sub UpdateUMPDLimits(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim ILSubQuote As QuickQuote.CommonObjects.QuickQuoteObject = Nothing
            If Quote IsNot Nothing Then
                If QuickQuoteHelperClass.QuoteHasState(Quote, QuickQuoteHelperClass.QuickQuoteState.Illinois) Then
                    ILSubQuote = IFM.VR.Common.Helpers.MultiState.General.SubQuoteForState(IFM.VR.Common.Helpers.MultiState.General.SubQuotes(Quote), QuickQuoteHelperClass.QuickQuoteState.Illinois)
                End If
            End If

            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'Vehicles with UMPD selected, will have policy level UMPD limit set to 15,000
                    Dim setPolicyLevelUMPD = False
                    If ILSubQuote IsNot Nothing Then
                        If ILSubQuote.Vehicles IsNot Nothing AndAlso ILSubQuote.Vehicles.Count > 0 Then
                            For each vehicle In ILSubQuote.Vehicles
                                If (vehicle.HasUninsuredMotoristPropertyDamage AndAlso vehicle.GaragingAddress?.Address?.QuickQuoteState = QuickQuoteHelperClass.QuickQuoteState.Illinois) Then
                                    setPolicyLevelUMPD = True
                                    Exit For
                                End If
                            Next
                        End If
                        If setPolicyLevelUMPD = True Then
                            ILSubQuote.UninsuredMotoristPropertyDamage_IL_LimitId = 48 '15,000
                            ILSubQuote.UninsuredMotoristPropertyDamage_IL_DeductibleId = 4 '250
                        End If
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'Remove policy level UMPD limit, we do not save/send the limit at the vehicle level
                    Dim NeedsWarningMessage As Boolean = False
                    If ILSubQuote IsNot Nothing Then
                        Dim qqHelper As New QuickQuoteHelperClass
                        If qqHelper.IsPositiveIntegerString(ILSubQuote.UninsuredMotoristPropertyDamage_IL_LimitId) Then
                            NeedsWarningMessage = True
                        End If
                        If ILSubQuote.UninsuredMotoristPropertyDamage_IL_LimitId IsNot Nothing Then
                            ILSubQuote.UninsuredMotoristPropertyDamage_IL_LimitId = ""
                        End If
                        If ILSubQuote.UninsuredMotoristPropertyDamage_IL_DeductibleId IsNot Nothing Then
                            ILSubQuote.UninsuredMotoristPropertyDamage_IL_DeductibleId = ""
                        End If
                        If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                            Dim i = New WebValidationItem(UMPDLimitsNotAvailMsg)
                            i.IsWarning = True
                            ValidationErrors.Add(i)
                        End If
                    End If
            End Select
        End Sub

        Public Shared Function IsUMPDLimitsAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                Dim hasILStateQuote As Boolean = QuickQuoteHelperClass.QuoteHasState(quote, QuickQuoteHelperClass.QuickQuoteState.Illinois)
                UMPDLimitsSettings.OtherQualifiers = hasILStateQuote
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, UMPDLimitsSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function
    End Class
End Namespace
