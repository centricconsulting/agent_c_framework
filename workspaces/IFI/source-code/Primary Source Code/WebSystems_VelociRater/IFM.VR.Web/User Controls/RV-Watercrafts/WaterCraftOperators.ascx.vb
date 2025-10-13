Imports IFM.PrimativeExtensions

Public Class WaterCraftOperators
    Inherits VRControlBase

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.divOperatorsList.ClientID, Me.HiddenFieldMainAccord, "0")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsQuoteEndorsement Then
            Me.Visible = False
            Me.HideFromParent = True
            Return
        End If
    End Sub

    Public Overrides Sub Populate()
        If Coalesce(Me.Quote?.Operators?.Any(), False) Then
            Me.Repeater1.Visible = True
            Me.divOperatorsList.Visible = True
            Me.Repeater1.DataSource = Me.Quote.Operators
            Me.Repeater1.DataBind()
            Me.FindChildVrControls() ' finds the just added controls do to the binding
            Dim index As Int32 = 0
            For Each child In Me.ChildVrControls
                If TypeOf child Is WaterCraftOperatorItem Then
                    Dim c As WaterCraftOperatorItem = child
                    c.OperatorIndex = index
                    c.Populate()
                    index += 1
                End If
            Next

            Me.lblHeader.Text = $"RV/Watercraft Operators ({Coalesce(Me.Quote?.Operators?.Count, 0)})"
        Else
            Me.lblHeader.Text = "RV/Watercraft Operators"
            Me.divOperatorsList.Visible = False
            Me.Repeater1.Visible = False
            Me.Repeater1.DataSource = Nothing
            Me.Repeater1.DataBind()
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidationHelper.GroupName = "RV/Watercraft Operators"
        Me.ValidateChildControls(valArgs)
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            For Each cntrl As VRControlBase In Me.ChildVrControls
                cntrl.Save()
            Next
            Return True
        End If
        Return False
    End Function

    Protected Sub lnkAdd_Click(sender As Object, e As EventArgs) Handles lnkAdd.Click
        Me.Quote.Operators.AddNew()
        Me.Populate()
        Me.Save_FireSaveEvent(False)
    End Sub

    Protected Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent()
    End Sub
End Class