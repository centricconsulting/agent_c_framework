Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports QuickQuote.CommonMethods
Public Class ctl_ENDO_App_PhotographersEquipment
    Inherits VRControlBase

    'Added 10/20/2021 for BOP Endorsements Task 65882 MLW

#Region "Declarations"

    Private AddNew As Boolean = False

#End Region

#Region "Methods and Functions"

    Private Sub HandleError(ByVal RoutineName As String, ByVal ex As Exception)
        Dim str As String = RoutineName & ":  " & ex.Message
        If AppSettings("TestOrProd").ToUpper <> "PROD" Then lblMsg.Text = str Else Throw New Exception(ex.Message, ex)
    End Sub

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.divPhotogScheduledEquipment.ClientID, hdnAccord, "0")
        Me.VRScript.AddScriptLine("$('[id*=txtPhotogItemLimit]').attr('disabled', 'disabled');")
        Me.VRScript.AddScriptLine("$('[id*=txtPhotogItemDesc]').attr('disabled', 'disabled');")
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        Dim err As String = Nothing

        Try
            lblMsg.Text = "&nbsp;"

            If GoverningStateQuote() IsNot Nothing Then
                'If Not AddNew Then Quote.CopyProfessionalLiabilityCoveragesFromBuildingsToPolicy_UseBuildingClassificationList()
                If GoverningStateQuote.PhotographyScheduledCoverages IsNot Nothing AndAlso GoverningStateQuote.PhotographyScheduledCoverages.Count > 0 Then
                    rptPhotogScheduledItems.DataSource = GoverningStateQuote.PhotographyScheduledCoverages
                    rptPhotogScheduledItems.DataBind()
                End If
            End If

            UpdateTotalLimit()

            Me.PopulateChildControls()

            Exit Sub
        Catch ex As Exception
            HandleError("Populate", ex)
        End Try
    End Sub

    Public Overrides Function Save() As Boolean
        'Dim qqXml As New QuickQuoteXML()
        'Dim diamondList As Diamond.Common.Objects.Policy.AdditionalInterestList = Nothing

        'Try
        '    lblMsg.Text = "&nbsp;"

        '    GoverningStateQuote.PhotographyScheduledCoverages = New List(Of QuickQuote.CommonObjects.QuickQuoteCoverage)
        '        For Each ri As RepeaterItem In rptPhotogScheduledItems.Items
        '            Dim psc As New QuickQuote.CommonObjects.QuickQuoteCoverage()
        '            Dim txtLimit As TextBox = ri.FindControl("txtPhotogItemLimit")
        '            Dim txtDesc As TextBox = ri.FindControl("txtPhotogItemDesc")

        '            psc.ManualLimitAmount = txtLimit.Text
        '            psc.Description = txtDesc.Text

        '            GoverningStateQuote.PhotographyScheduledCoverages.Add(psc)
        '        Next

        '    Me.SaveChildControls()
        Return True
        'Catch ex As Exception
        '    HandleError("Save", ex)
        '    Return False
        'End Try
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Dim ItemIndex As Integer = -1
        'Try
        '    MyBase.ValidateControl(valArgs)
        '    Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        '    For Each ri As RepeaterItem In rptPhotogScheduledItems.Items
        '        ItemIndex += 1
        '        Me.ValidationHelper.GroupName = "Photography Scheduled Items #" & ItemIndex + 1

        '        Dim txtLimit As TextBox = ri.FindControl("txtPhotogItemLimit")
        '        Dim txtDesc As TextBox = ri.FindControl("txtPhotogItemDesc")

        '        If txtLimit.Text.Trim = "" Then
        '            Me.ValidationHelper.AddError(txtLimit, "Missing Limit", accordList)
        '        Else
        '            If Not IsNumeric(txtLimit.Text) OrElse CInt(txtLimit.Text) <= 0 Then
        '                Me.ValidationHelper.AddError(txtLimit, "Limit is invalid", accordList)
        '            End If
        '        End If
        '        If txtDesc.Text.Trim = "" Then
        '            Me.ValidationHelper.AddError(txtDesc, "Missing Description", accordList)
        '        End If
        '    Next

        '    Me.ValidateChildControls(valArgs)
        '    Exit Sub
        'Catch ex As Exception
        '    HandleError("ValidateControls", ex)
        '    Exit Sub
        'End Try
    End Sub

    Private Sub UpdateTotalLimit()
        Dim tot As Decimal = 0
        Try
            If GoverningStateQuote.PhotographyScheduledCoverages IsNot Nothing AndAlso GoverningStateQuote.PhotographyScheduledCoverages.Count > 0 Then
                For Each sc As QuickQuote.CommonObjects.QuickQuoteCoverage In GoverningStateQuote.PhotographyScheduledCoverages
                    If sc.ManualLimitAmount <> "" AndAlso IsNumeric(sc.ManualLimitAmount) Then tot += CDec(sc.ManualLimitAmount)
                Next
            End If

            lblPhotographerItemsTotal.Text = "Total of All Scheduled Limits: " & Format(tot, "##########")

            Exit Sub
        Catch ex As Exception
            HandleError("UpdateTotalLimit", ex)
            Exit Sub
        End Try
    End Sub

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Not IsPostBack Then
                'Me.MainAccordionDivId = Me.divBuildingClassification.ClientID
            End If

            Exit Sub
        Catch ex As Exception
            HandleError("Page LOAD", ex)
            Exit Sub
        End Try
    End Sub

    'Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
    '    Try
    '        Me.Save_FireSaveEvent()
    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("Save", ex)
    '        Exit Sub
    '    End Try
    'End Sub

    Protected Sub rptPhotogScheduledItems_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        Dim ndx = e.Item.ItemIndex
        Try
            Select Case e.CommandName.ToUpper()
                Case "DELETE"
                    If GoverningStateQuote.PhotographyScheduledCoverages IsNot Nothing AndAlso GoverningStateQuote.PhotographyScheduledCoverages.HasItemAtIndex(ndx) Then
                        GoverningStateQuote.PhotographyScheduledCoverages.RemoveAt(ndx)
                        Me.Populate()
                        Me.Save_FireSaveEvent()
                    End If
                    Exit Select
                Case Else
                    Exit Select
            End Select

            Exit Sub
        Catch ex As Exception
            HandleError("rptPhotogScheduledItems_ItemCommand", ex)
            Exit Sub
        End Try
    End Sub

    'Private Sub btnAddNew_Click(sender As Object, e As EventArgs) Handles btnAddNew.Click
    '    Dim NewItem As New QuickQuote.CommonObjects.QuickQuoteCoverage()
    '    Try
    '        AddNew = True
    '        If Not GoverningStateQuote.HasPhotographyCoverageScheduledCoverages Then GoverningStateQuote.HasPhotographyCoverageScheduledCoverages = True
    '        Save_FireSaveEvent(False) ' Save any previously entered unsaved items
    '        NewItem.Description = "Item " & GoverningStateQuote.PhotographyScheduledCoverages.Count + 1.ToString
    '        NewItem.ManualLimitAmount = ""
    '        GoverningStateQuote.PhotographyScheduledCoverages.Add(NewItem)
    '        GoverningStateQuote.CopyProfessionalLiabilityCoveragesFromPolicyToBuildings_UseBuildingClassificationList()
    '        'Quote.CopyProfessionalLiabilityCoveragesFromBuildingsToPolicy_UseBuildingClassificationList()
    '        Populate()
    '        Save_FireSaveEvent(False)
    '        Populate_FirePopulateEvent()
    '        AddNew = False
    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("btnAddNew_Click", ex)
    '    End Try
    'End Sub

    'Private Sub lbPhotogAddItem_Click(sender As Object, e As EventArgs) Handles lbPhotogAddItem.Click
    '    Dim NewItem As New QuickQuote.CommonObjects.QuickQuoteCoverage()
    '    Try
    '        AddNew = True
    '        If Not Quote.HasPhotographyCoverageScheduledCoverages Then Quote.HasPhotographyCoverageScheduledCoverages = True
    '        Save_FireSaveEvent(False) ' Save any previously entered unsaved items
    '        NewItem.Description = "Item " & Quote.PhotographyScheduledCoverages.Count + 1.ToString
    '        NewItem.ManualLimitAmount = ""
    '        Quote.PhotographyScheduledCoverages.Add(NewItem)
    '        Quote.CopyProfessionalLiabilityCoveragesFromPolicyToBuildings_UseBuildingClassificationList()
    '        'Quote.CopyProfessionalLiabilityCoveragesFromBuildingsToPolicy_UseBuildingClassificationList()
    '        Populate()
    '        Save_FireSaveEvent(False)
    '        Populate_FirePopulateEvent()
    '        AddNew = False
    '        Exit Sub
    '    Catch ex As Exception
    '        HandleError("lbPhotogAddItem_Click", ex)
    '    End Try
    'End Sub

    Private Sub rptPhotogScheduledItems_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPhotogScheduledItems.ItemDataBound
        Dim amt As Decimal = 0
        Try
            Dim txtLimit As TextBox = e.Item.FindControl("txtPhotogItemLimit")
            Dim txtDesc As TextBox = e.Item.FindControl("txtPhotogItemDesc")
            Dim AI As QuickQuote.CommonObjects.QuickQuoteCoverage = e.Item.DataItem

            If AI.ManualLimitAmount <> "" AndAlso IsNumeric(AI.ManualLimitAmount) Then
                amt = CDec(AI.ManualLimitAmount)
            End If

            txtLimit.Text = Format(amt, "##########")
            txtDesc.Text = AI.Description

            Exit Sub
        Catch ex As Exception
            HandleError("rptPhotogScheduledItems_ItemDataBound", ex)
            Exit Sub
        End Try
    End Sub

#End Region

End Class