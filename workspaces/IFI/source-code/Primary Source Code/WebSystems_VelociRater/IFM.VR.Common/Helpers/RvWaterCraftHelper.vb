Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports IFM.PrimativeExtensions

Namespace IFM.VR.Common.Helpers
    Public Class RvWaterCraftHelper

        Private Shared _RvWaterCraftMotorSettings As NewFlagItem
        Public Shared ReadOnly Property RvWaterCraftMotorSettings() As NewFlagItem
            Get
                If _RvWaterCraftMotorSettings Is Nothing Then
                    _RvWaterCraftMotorSettings = New NewFlagItem("VR_Far_Hom_RvWaterCraftMotor_Settings")
                End If
                Return _RvWaterCraftMotorSettings
            End Get
        End Property

        Const RvWaterCraftMotorWarningMsg As String = ""
        Const RvWaterCraftMotorRemovedMsg As String = ""
        Public Shared Function RvWaterCraftMotorEnabled() As Boolean
            Return RvWaterCraftMotorSettings.EnabledFlag
        End Function

        Public Shared Function RvWaterCraftMotorEffDate() As Date
            Return RvWaterCraftMotorSettings.StartDate
        End Function

        Public Shared Sub UpdateRvWaterCraftMotor(ByRef Quote As QuickQuoteObject, ByRef CrossDirection As Helper.EnumsHelper.CrossDirectionEnum, Optional ByRef ValidationErrors As List(Of WebValidationItem) = Nothing)
            Dim NeedsWarningMessage As Boolean = False
            Select Case CrossDirection
                Case Helper.EnumsHelper.CrossDirectionEnum.FORWARD
                    'No logic required
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(RvWaterCraftMotorWarningMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
                Case Helper.EnumsHelper.CrossDirectionEnum.BACK
                    'No logic required
                    If NeedsWarningMessage AndAlso ValidationErrors IsNot Nothing Then
                        Dim i = New WebValidationItem(RvWaterCraftMotorRemovedMsg)
                        i.IsWarning = True
                        ValidationErrors.Add(i)
                    End If
            End Select
        End Sub

        Public Shared Function IsRvWaterCraftMotorAvailable(quote As QuickQuoteObject) As Boolean

            If quote IsNot Nothing Then
                Dim qqh As New QuickQuoteHelperClass
                Dim SubQuoteFirst = qqh.MultiStateQuickQuoteObjects(quote).GetItemAtIndex(0)

                Dim IsCorrectLOB As Boolean = quote.LobType = QuickQuoteObject.QuickQuoteLobType.HomePersonal OrElse quote.LobType = QuickQuoteObject.QuickQuoteLobType.Farm

                RvWaterCraftMotorSettings.OtherQualifiers = IsCorrectLOB
                Return AvailabilityByDateOrVersion.AvailabilityByDateOrVersion(quote, RvWaterCraftMotorSettings, AvailabilityByDateOrVersion.VersionTypeToTest.SubquoteFirstVersionId)
            End If
            Return False

        End Function

        Public Shared Function HasIncludedMotor(MyVehicle As QuickQuoteRvWatercraft) As Boolean
            Dim qqhelper As New QuickQuoteHelperClass
            If MyVehicle?.RvWatercraftMotors.IsLoaded = False OrElse
               (qqhelper.IsPositiveIntegerString(MyVehicle?.HorsepowerCC) = False AndAlso
               MyVehicle?.HorsepowerCC <> "1" AndAlso 'if MotorType is None this will be set to 1
               qqhelper.IsPositiveIntegerString(MyVehicle?.RvWatercraftMotors(0).Year) = False AndAlso
               qqhelper.IsPositiveIntegerString(MyVehicle?.RvWatercraftMotors(0).CostNew) = False AndAlso
               String.IsNullOrWhiteSpace(MyVehicle?.RvWatercraftMotors(0).Manufacturer) AndAlso
               String.IsNullOrWhiteSpace(MyVehicle?.RvWatercraftMotors(0).Model) AndAlso
               String.IsNullOrWhiteSpace(MyVehicle?.RvWatercraftMotors(0).SerialNumber)) Then
                Return False
            End If
            Return True
        End Function

    End Class
End Namespace

