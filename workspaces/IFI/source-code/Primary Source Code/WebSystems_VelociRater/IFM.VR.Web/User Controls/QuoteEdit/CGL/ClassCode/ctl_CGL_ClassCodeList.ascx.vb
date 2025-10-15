Imports IFM.PrimativeExtensions

Public Class ctl_CGL_ClassCodeList
    Inherits VRControlBase


    Public Property LocationIndex As Int32
        Get
            Return ViewState.GetInt32("vs_LocationIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_LocationIndex") = value
        End Set
    End Property

    Public Property BuildingIndex As Int32
        Get
            Return ViewState.GetInt32("vs_BuildingIndex", -1)
        End Get
        Set(value As Int32)
            ViewState("vs_BuildingIndex") = value
        End Set
    End Property

    Public ReadOnly Property MyClassCodes As List(Of QuickQuote.CommonObjects.QuickQuoteGLClassification)
        Get
            If Me.LocationIndex >= 0 Then
                If Me.BuildingIndex >= 0 Then
                    ' ONly Personal Property and Personal Property of Others contain a class code Info
                    'Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).Buildings.GetItemAtIndex(Me.BuildingIndex).GLClassifications
                Else
                    Return Me.Quote.Locations.GetItemAtIndex(Me.LocationIndex).GLClassifications
                End If
            Else
                Return Me.Quote.GLClassifications
            End If
            Return Nothing
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        If Me.divNewClassCode.Visible Then
            Me.VRScript.CreateAccordion(Me.divNewClassCode.ClientID, Nothing, "false", True)
        Else
            If Me.divAccord.Visible Then
                Me.VRScript.CreateAccordion(Me.ListAccordionDivId, Me.hddAccord, "0")
            End If
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If Me.Quote.IsNotNull Then
            Dim classCodes = Me.MyClassCodes ' IFM.VR.Common.Helpers.CGL.ClassCodeHelper.GetAllPolicyAndLocationClassCodes(Me.Quote)
            Me.divNewClassCode.Visible = False
            Me.divAccord.Visible = False
            If classCodes.IsLoaded() Then
                divAccord.Visible = True
                Me.Repeater1.DataSource = classCodes
                Me.Repeater1.DataBind()
                Me.FindChildVrControls()

                Dim ccIndex As Int32 = 0
                For Each cc In Me.GatherChildrenOfType(Of ctl_CGL_Classcode)
                    cc.ClassCodeIndex = ccIndex
                    cc.LocationIndex = Me.LocationIndex
                    cc.BuildingIndex = Me.BuildingIndex
                    ccIndex += 1
                Next

            Else
                Me.divNewClassCode.Visible = True
                Me.ctl_CGL_Classcode.LocationIndex = Me.LocationIndex
                Me.ctl_CGL_Classcode.BuildingIndex = Me.BuildingIndex
                Me.Repeater1.DataSource = Nothing
                Me.Repeater1.DataBind()
            End If
        End If
        Me.PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.ListAccordionDivId = Me.divAccord.ClientID
    End Sub

    Public Overrides Function Save() As Boolean
        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

End Class