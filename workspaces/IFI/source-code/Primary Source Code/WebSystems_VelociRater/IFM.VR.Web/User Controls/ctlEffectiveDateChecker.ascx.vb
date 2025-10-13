Imports IFM.VR.Validation.ObjectValidation

Public Class ctlEffectiveDateChecker
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

    End Sub

    Public Overrides Function Save() As Boolean
        Return False
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        If Me.Quote IsNot Nothing Then

            Dim valItems = PolicyLevelValidator.PolicyValidation(Me.Quote, valArgs.ValidationType)
            If valItems.Any() Then
                For Each v In valItems
                    Select Case v.FieldId
                        Case PolicyLevelValidator.EffectiveDate
                            If v.IsWarning = True Then 'added IF 8/1/2019; original logic in ELSE
                                Me.ValidationHelper.AddWarning(v.Message)
                            Else
                                Me.ValidationHelper.AddError(v.Message)
                            End If
                    End Select
                Next
            End If
        End If
    End Sub

End Class