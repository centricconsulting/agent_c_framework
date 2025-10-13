Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports System.Web.UI
Imports PopupMessageClass

Namespace IFM.VR.Common.Helpers.CPR

    Public Class LocationWindHailHelper
        Inherits FeatureFlagBase

        Const CPPCPRLocationWindHailFirstWrittenDateKey As String = "VR_CPP_CPR_LocationWindHail_FirstWrittenDate"
        Private Shared _LocationWindHailSettings As NewFlagItem
        Public Shared ReadOnly Property LocationWindHailSettings() As NewFlagItem
            Get
                If _LocationWindHailSettings Is Nothing Then
                    _LocationWindHailSettings = New NewFlagItem("VR_CPP_CPR_LocationWindHail_Settings")
                End If
                Return _LocationWindHailSettings
            End Get
        End Property

        Public Const LocationWindHailMsg As String = "A wind hail percentage deductible was selected, but at least one coverage must be selected at the location for it to apply. The deductible selection has been removed. If you wish to add it back, please select the percentage and check one coverage to apply this deductible option."

        Public Shared Function LocationWindHailEnabled() As Boolean
            Return LocationWindHailSettings.EnabledFlag
        End Function

        Public Shared Function LocationWindHailEffDate() As Date
            Return LocationWindHailSettings.GetStartDateOrDefault("7/15/2025")
        End Function

        'Used in helper to check if location wind hail deductible % is applied to any wind hail deductible % checkbox
        Public Const BuildingCoverages As String = "{61DDAC2B-556F-4477-88F3-06404C7F0C27}"
        Public Const PropertyInTheOpen As String = "{0CE78A9F-7F38-4D9B-93C8-6B564018A6F9}"
        'Used for ctl_CPR_Building control to populate building coverages
        Public Const BuildingCov As String = "{A530FB7A-5383-47BD-9F14-C383260C9600}"
        Public Const PersonalPropCov As String = "{FCEAB32F-B0AD-4FF4-BB39-43D83EF4FC21}"
        Public Const PersonalPropOfOthers As String = "{BA89ED26-656E-4718-8769-4D7CB6DF7454}"

        Public Shared Sub UpdateLocationWindHail(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    Dim NeedsWarningMessage As Boolean = False
                    If Quote IsNot Nothing AndAlso Quote.Locations IsNot Nothing AndAlso Quote.Locations.Count > 0 Then
                        For Each l As QuickQuoteLocation In Quote.Locations
                            If l.WindHailDeductibleLimitId.EqualsAny("32", "33", "34") Then
                                '32=1%, 33=2%, 34=5%
                                Dim hasWindHailApplied As Boolean = LocationWindHailApplied(l, BuildingCoverages)
                                If hasWindHailApplied = False Then
                                    l.WindHailDeductibleLimitId = "0"
                                    NeedsWarningMessage = True
                                End If
                            End If
                        Next
                    End If
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(LocationWindHailMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No Change
            End Select
        End Sub

        Public Shared Function IsLocationWindHailAvailable(quote As QuickQuoteObject) As Boolean
            If quote IsNot Nothing Then
                LocationWindHailSettings.OtherQualifiers = DoesQuoteQualifyByFirstWrittenDate(quote, CPPCPRLocationWindHailFirstWrittenDateKey)

                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, LocationWindHailSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False
        End Function

        Public Shared Function LocationWindHailApplied(l As QuickQuoteLocation, cov As String) As Boolean
            Dim hasWindHailApplied As Boolean = False
            Select Case cov
                Case PropertyInTheOpen
                    If l.PropertyInTheOpenRecords IsNot Nothing AndAlso l.PropertyInTheOpenRecords.Count > 0 Then
                        For Each pito As QuickQuotePropertyInTheOpenRecord In l.PropertyInTheOpenRecords
                            If pito.OptionalWindstormOrHailDeductibleId = l.WindHailDeductibleLimitId Then
                                hasWindHailApplied = True
                                Exit For
                            End If
                        Next
                    End If
                Case BuildingCoverages
                    If l.Buildings IsNot Nothing AndAlso l.Buildings.Count > 0 Then
                        For Each b In l.Buildings
                            If b.OptionalWindstormOrHailDeductibleId = l.WindHailDeductibleLimitId OrElse
                                b.PersPropCov_OptionalWindstormOrHailDeductibleId = l.WindHailDeductibleLimitId OrElse
                                b.PersPropOfOthers_OptionalWindstormOrHailDeductibleId = l.WindHailDeductibleLimitId Then
                                hasWindHailApplied = True
                                Exit For
                            End If
                        Next
                    End If
            End Select
            Return hasWindHailApplied
        End Function
        Public Shared Sub ShowWindHailNeedsAppliedPopupMessage(page As Page)
            Using popup As New PopupMessageObject(page, LocationWindHailMsg)
                With popup
                    .Title = "Wind Hail for Location Coverage"
                    .isFixedPositionOnScreen = True
                    .ZIndexOfPopup = 2
                    .isModal = True
                    .Image = PopupMessageObject.ImageOptions.None
                    .hideCloseButton = True
                    .AddButton("OK", True)
                    .CreateDynamicPopUpWindow()
                End With
            End Using
        End Sub
    End Class
End Namespace
