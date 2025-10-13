Imports IFM.PrimativeExtensions
Public Class ctl_CPR_PropertyInOpenList
    Inherits VRControlBase

    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Private ReadOnly Property MyLocation As QuickQuote.CommonObjects.QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations.HasItemAtIndex(Me.LocationIndex) Then
                Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex)
            End If
            Return Nothing
        End Get

    End Property

    Public Overrides Sub AddScriptAlways()
    End Sub

    Protected Sub AttachItemControlEvents()
        Dim index As Int32 = 0
        For Each cntrl As RepeaterItem In Me.Repeater1.Items
            Dim ClassControl As ctl_CPR_PropertyInOpenItem = cntrl.FindControl("ctl_CPR_PropInOpenItem")
            AddHandler ClassControl.PIODeleteRequested, AddressOf ItemControlDeleteItemRequested
            index += 1
        Next
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
        Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hdnAccordList, "0")
        Me.VRScript.StopEventPropagation(Me.lnkAdd.ClientID)
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            If MyLocation IsNot Nothing Then
                Repeater1.DataSource = MyLocation.PropertyInTheOpenRecords
                Repeater1.DataBind()

                Me.FindChildVrControls()
                Dim PIOIndex As Int32 = 0
                For Each cnt In Me.GatherChildrenOfType(Of ctl_CPR_PropertyInOpenItem)
                    cnt.LocationIndex = Me.LocationIndex
                    cnt.PropertyIndex = PIOIndex
                    cnt.Populate()
                    PIOIndex += 1
                Next
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.MainAccordionDivId = Me.divClassificationsList.ClientID
        Me.ListAccordionDivId = Me.divList.ClientID
        AttachItemControlEvents()
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub lnkBtnAdd_Click(sender As Object, e As EventArgs) Handles lnkAdd.Click
        Dim cls As New QuickQuote.CommonObjects.QuickQuotePropertyInTheOpenRecord With {.SpecialClassCodeTypeId = "0"}

        If MyLocation IsNot Nothing AndAlso MyLocation.PropertyInTheOpenRecords Is Nothing Then
            MyLocation.PropertyInTheOpenRecords = New List(Of QuickQuote.CommonObjects.QuickQuotePropertyInTheOpenRecord)
        End If
        MyLocation.PropertyInTheOpenRecords.Add(cls)
        Populate()
        Save_FireSaveEvent(False)
        'Me.Populate()
        Me.hdnAccordList.Value = (MyLocation.PropertyInTheOpenRecords.Count - 1).ToString
    End Sub

    Private Sub ItemControlDeleteItemRequested(ByVal LocIndex As Integer, ByVal ItemIndex As Integer)
        If MyLocation IsNot Nothing Then
            If MyLocation.PropertyInTheOpenRecords IsNot Nothing AndAlso MyLocation.PropertyInTheOpenRecords.HasItemAtIndex(ItemIndex) Then
                MyLocation.PropertyInTheOpenRecords.RemoveAt(ItemIndex)
                Populate()
                Save_FireSaveEvent(False)
                Me.hdnAccordList.Value = (MyLocation.PropertyInTheOpenRecords.Count - 1).ToString
            End If
        End If
    End Sub
End Class