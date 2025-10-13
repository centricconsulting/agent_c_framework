Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions


Namespace IFM.VR.Common.Helpers.CPR

    Public Class WindHailDefaultingHelper
        Inherits FeatureFlagBase

        Private Shared _WindHailDefaultingSettings As NewFlagItem

        Const CPPCPRWindHailDefaultingFirstWrittenDateKey As String = "VR_CPP_CPR_WindHailDefaulting_FirstWrittenDate"
        Public Shared ReadOnly Property WindHailDefaultingSettings() As NewFlagItem
            Get
                If _WindHailDefaultingSettings Is Nothing Then
                    _WindHailDefaultingSettings = New NewFlagItem("VR_CPP_CPR_WindHailDefaulting_Settings")
                End If
                Return _WindHailDefaultingSettings
            End Get
        End Property

        Public Const WindHailDefaultingMsg As String = "Effective 7/15/2025, a 1% wind hail deductible is required for all building coverage with a lessors' risk exposure. This deductible applies to properties where the 'percentage owner occupied' field is marked as either 'None' or '1%-10%' owner occupied."

        Public Shared Function WindHailDefaultingEnabled() As Boolean
            Return WindHailDefaultingSettings.EnabledFlag
        End Function

        Public Shared Function WindHailDefaultingEffDate() As Date
            Return WindHailDefaultingSettings.GetStartDateOrDefault("7/15/2025")
        End Function

        Public Shared Sub UpdateWindHailDefaulting(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    Dim NeedsWarningMessage As Boolean = False
                    If Quote IsNot Nothing AndAlso Quote.Locations.IsLoaded() Then
                        For Each l As QuickQuoteLocation In Quote.Locations
                            If NeedToDefaultLocationWindHail(l) Then
                                l.WindHailDeductibleLimitId = "32"  '32=1%, 33=2%, 34=5%
                                NeedsWarningMessage = True
                            End If
                        Next
                    End If
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(WindHailDefaultingMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Change
            End Select
        End Sub

        Public Shared Function IsWindHailDefaultingAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                WindHailDefaultingSettings.OtherQualifiers = DoesQuoteQualifyByFirstWrittenDate(quote, CPPCPRWindHailDefaultingFirstWrittenDateKey)
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, WindHailDefaultingSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

        Public Shared Function NeedToDefaultLocationWindHail(l As QuickQuoteLocation) As Boolean
            Dim needToDefault As Boolean = False
            If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                For Each b In l.Buildings
                    If (b IsNot Nothing AndAlso Not CheckCPRCPPExemptCodes(b) AndAlso (b.OwnerOccupiedPercentageId = "30" OrElse b.OwnerOccupiedPercentageId = "31")) Then
                        '0 = None for Location and 30 = NONE, 31 = 1% - 10% OWNER OCCUPIED for Building
                        needToDefault = True
                        Exit For
                    End If
                Next
            End If
            Return needToDefault
        End Function

        Public Shared Function CheckCPRCPPExemptCodes(b As QuickQuoteBuilding) As Boolean
            If b IsNot Nothing AndAlso b.ClassificationCode IsNot Nothing AndAlso b.ClassificationCode.ClassCode.EqualsAny("0196", "0197", "0198", "0311", "0312", "0313", "0331", "0332", "0333") Then
                Return True
            End If
            Return False
        End Function

    End Class

End Namespace
