Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports QuickQuote.CommonObjects
Imports IFM.VR.Web.Helpers
Imports IFM.PrimativeExtensions
Public Class cov_ScheduledPropertyFloater_Item
    Inherits VRControlBase

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()

        If GoverningStateQuote.ScheduledPropertyItems Is Nothing Then
            GoverningStateQuote.ScheduledPropertyItems = New List(Of QuickQuoteScheduledPropertyItem)()
        End If
        If GoverningStateQuote.ScheduledPropertyItems.Any() = False Then
            GoverningStateQuote.ScheduledPropertyItems.Add(New QuickQuoteScheduledPropertyItem())
            GoverningStateQuote.ScheduledPropertyItems(0).Description = "Scheduled Property #1"
        End If

        Me.spRepeater.DataSource = GoverningStateQuote.ScheduledPropertyItems
        Me.spRepeater.DataBind()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub

    Public Overrides Function Save() As Boolean

        Dim spList As New List(Of QuickQuoteScheduledPropertyItem)

        Dim totalLimit As Double = 0.0

        For Each ri As RepeaterItem In spRepeater.Items
            Dim txtLimit As TextBox = ri.FindControl("txtLimit")
            Dim txtDescription As TextBox = ri.FindControl("txtDescription")
            totalLimit += IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtLimit.Text)
            Dim itemIndex = ri.ItemIndex

            Dim item As New QuickQuoteScheduledPropertyItem
            item.Limit = txtLimit.Text
            item.Description = txtDescription.Text
            spList.Add(item)
        Next

        GoverningStateQuote.ScheduledPropertyItems = spList

        Dim rate As Double = CIMHelper.ScheduledPropertyRateTable.GetRateForLimit(totalLimit)
        Dim CoInsurance As DropDownList = Parent.FindControl("spCoInsurance")
        Select Case CoInsurance.SelectedValue
            Case "6" ' 90%
                rate = rate * 0.95
            Case "7" ' 100%
                rate = rate * 0.9
            Case Else
                rate = rate
        End Select
        GoverningStateQuote.ScheduledPropertyRate = rate

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Scheduled Property Floater"
        Dim deductibleText As String = QQHelper.GetStaticDataTextForValue(QuickQuoteClassName.QuickQuoteObject, QuickQuotePropertyName.FineArtsDeductibleId, GoverningStateQuote.ScheduledPropertyDeductibleId)
        Dim deductibleAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(deductibleText)
        Dim totalLimit As Double = 0.0

        For Each ri As RepeaterItem In spRepeater.Items
            Dim txtLimit As TextBox = ri.FindControl("txtLimit")
            Dim LimitAmount = IFM.Common.InputValidation.InputHelpers.TryToGetDouble(txtLimit.Text)
            Dim txtDescription As TextBox = ri.FindControl("txtDescription")

            If String.IsNullOrEmpty(txtLimit.Text) Then
                Me.ValidationHelper.AddError("Missing Limit", txtLimit.ClientID)
            End If
            If String.IsNullOrEmpty(txtDescription.Text) Then
                Me.ValidationHelper.AddError("Missing  Description", txtDescription.ClientID)
            End If
            ' 3.8.129
            If LimitAmount > 50000 Then
                Me.ValidationHelper.AddError("You have selected a limit that exceeds your authority, please contact your underwriter for higher limits.", txtLimit.ClientID)
            End If
            ' 3.8.114
            If deductibleAmount >= LimitAmount Then
                Me.ValidationHelper.AddError("Deductible amount selected is equal or greater than the Limit. Please adjust either value.", txtLimit.ClientID)
            End If
        Next

        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub btnAddClick()
        If Me.GoverningStateQuote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Dim newItem As New QuickQuoteScheduledPropertyItem()
            newItem.Description = String.Format("Scheduled Property #{0}", Me.GoverningStateQuote.ScheduledPropertyItems.Count + 1)
            Me.GoverningStateQuote.ScheduledPropertyItems.Add(newItem)

            'Me.Quote.ScheduledPropertyItems.AddNew()
            Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Private Sub btnDeleteClick(index)
        If Me.GoverningStateQuote.IsNotNull Then
            Me.Save_FireSaveEvent(False)
            Me.GoverningStateQuote.ScheduledPropertyItems.RemoveAt(index)
            Populate()
            Me.Save_FireSaveEvent(False)
        End If
    End Sub

    Protected Sub brRepeater_Add(source As Object, e As RepeaterCommandEventArgs) Handles spRepeater.ItemCommand
        If e.CommandName = "lnkAdd" Then
            btnAddClick()
        ElseIf e.CommandName = "lnkDelete" Then
            btnDeleteClick(e.Item.ItemIndex)
        End If
    End Sub

    Public Overrides Sub ClearControl()
        GoverningStateQuote.ScheduledPropertyItems = Nothing
    End Sub
End Class