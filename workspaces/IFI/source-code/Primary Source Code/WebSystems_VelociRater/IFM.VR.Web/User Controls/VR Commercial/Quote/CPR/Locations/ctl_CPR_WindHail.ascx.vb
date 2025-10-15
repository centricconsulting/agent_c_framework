Imports IFM.PrimativeExtensions
Imports QuickQuote.CommonObjects.QuickQuoteStaticDataOption
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass

Public Class ctl_CPR_WindHail
    Inherits VRControlBase

    Public Property MyLocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_locationIndex")
        End Get
        Set(value As Int32)
            ViewState("vs_locationIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote.IsNotNull Then
                Return Me.Quote.Locations.GetItemAtIndex(MyLocationIndex)
            End If
            Return Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property MyAccordionIndex As Integer
        Get
            Return MyLocationIndex
        End Get
    End Property


    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.StopEventPropagation(Me.lnkClear.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
        Me.VRScript.CreateAccordion(Me.divWindHail.ClientID, hdnAccord, 0)
    End Sub

    Public Overrides Sub LoadStaticData()
        If ddWindHailDeductible.Items Is Nothing OrElse ddWindHailDeductible.Items.Count <= 0 Then
            QQHelper.LoadStaticDataOptionsDropDown(Me.ddWindHailDeductible, QuickQuoteClassName.QuickQuoteLocation, QuickQuotePropertyName.WindHailDeductibleLimitId, SortBy.None, Me.Quote.LobType)
        End If
    End Sub

    Protected Sub HandlePropertyClear()
        Me.lblAccordHeader.Text = "Location"
    End Sub

    Public Overrides Sub Populate()
        LoadStaticData()
        If Quote IsNot Nothing Then
            If MyLocation IsNot Nothing Then
                SetFromValue(ddWindHailDeductible, MyLocation.WindHailDeductibleLimitId, "0")
            End If
        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing AndAlso MyLocation IsNot Nothing Then
            MyLocation.WindHailDeductibleLimitId = ddWindHailDeductible.SelectedValue
        End If

        Me.SaveChildControls()
        Return True
    End Function

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Me.Save_FireSaveEvent()
    End Sub

    Protected Sub lnkClear_Click(sender As Object, e As EventArgs) Handles lnkClear.Click
        If ddWindHailDeductible.Items IsNot Nothing AndAlso ddWindHailDeductible.Items.Count > 0 Then
            ddWindHailDeductible.SelectedIndex = 0
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

End Class