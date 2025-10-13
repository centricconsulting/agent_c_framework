Imports IFM.PrimativeExtensions
Imports System.Configuration.ConfigurationManager
Imports IFM.VR.Common.Helpers.MultiState.General
Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports Diamond.Common.Objects.Policy
Imports Diamond.Web.BaseControls
Imports IFM.PolicyLoader.QuickQuote
Imports IFM.PolicyLoader
Imports System.Linq
Imports System.Collections.Generic
Imports DevDictionaryHelper
Imports QuickQuote.CommonObjects.Umbrella

Public Class ctl_FUPPUP_UnderlyingPolicy_Item
    Inherits VRControlBase

#Region "Declarations"


    Public Event RequestAddAnother()
    Public Event RequestDelete(polNumber As String)

    Public ReadOnly UmbrellaDictionaryName As String = "UmbrellaUnderlyingDetails"

    Public Property LobType() As QuickQuoteObject.QuickQuoteLobType
        Get
            If ViewState("UMB_ListLobType") Is Nothing Then
                ViewState("UMB_ListLobType") = QuickQuoteObject.QuickQuoteLobType.None
            End If
            Return ViewState("UMB_ListLobType")
        End Get
        Set(value As QuickQuoteObject.QuickQuoteLobType)
            ViewState("UMB_ListLobType") = value
        End Set
    End Property

    Public Property LobCategoryText() As String
        Get
            If ViewState("UMB_ListLobCategoryText") Is Nothing Then
                ViewState("UMB_ListLobCategoryText") = String.Empty
            End If
            Return ViewState("UMB_ListLobCategoryText")
        End Get
        Set(value As String)
            ViewState("UMB_ListLobCategoryText") = value
        End Set
    End Property

    Public Property LobCategoryMsg() As String
        Get
            If ViewState("UMB_ListLobCategoryMsg") Is Nothing Then
                ViewState("UMB_ListLobCategoryMsg") = String.Empty
            End If
            Return ViewState("UMB_ListLobCategoryMsg")
        End Get
        Set(value As String)
            ViewState("UMB_ListLobCategoryMsg") = value
        End Set
    End Property

    Public Property FilteredPolicyList() As Dictionary(Of String, String)
        Get
            If ViewState("UMB_FilteredPolicyList") Is Nothing Then
                ViewState("UMB_FilteredPolicyList") = New Dictionary(Of String, String)
            End If
            Return ViewState("UMB_FilteredPolicyList")
        End Get
        Set(value As Dictionary(Of String, String))
            ViewState("UMB_FilteredPolicyList") = value
        End Set
    End Property
    Public Property _loaderValidationErrors() As Dictionary(Of String, String)
        Get
            If ViewState("UMB_loaderValidationErrors") Is Nothing Then
                ViewState("UMB_loaderValidationErrors") = New Dictionary(Of String, String)
            End If
            Return ViewState("UMB_loaderValidationErrors")
        End Get
        Set(value As Dictionary(Of String, String))
            ViewState("UMB_loaderValidationErrors") = value
        End Set
    End Property

    Private Property _devDictionaryHelper As DevDictionaryHelper.DevDictionaryHelper
    Public ReadOnly Property ddh() As DevDictionaryHelper.DevDictionaryHelper
        Get
            If _devDictionaryHelper Is Nothing Then
                If Quote IsNot Nothing AndAlso String.IsNullOrWhiteSpace(UmbrellaDictionaryName) = False Then
                    _devDictionaryHelper = New DevDictionaryHelper.DevDictionaryHelper(Quote, UmbrellaDictionaryName, LobType)
                End If
            End If
            Return _devDictionaryHelper
        End Get
    End Property

    Private Property _addAnotherFlag As Boolean



#End Region

#Region "Methods and Functions"

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub


    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        If Quote IsNot Nothing Then
            lblLobCategoryText.Text = LobCategoryText
            GetLobFilteredList()

            If String.IsNullOrWhiteSpace(LobCategoryMsg) = False Then
                lblLobCategoryMsg.Text = LobCategoryMsg
                lblLobCategoryMsg.Visible = True
            Else
                lblLobCategoryMsg.Text = String.Empty
                lblLobCategoryMsg.Visible = False
            End If
        End If

        Me.rptPolicyList.DataSource = Me.FilteredPolicyList
        Me.rptPolicyList.DataBind()

        If _loaderValidationErrors.Any() Then

        End If
    End Sub

    Public Sub GetLobFilteredList()
        FilteredPolicyList = ddh.GetUmbrellaDictionaryByLobType()

        If FilteredPolicyList?.Count = 0 OrElse _addAnotherFlag Then
            FilteredPolicyList.Add("temp", "")
            _addAnotherFlag = False
        End If

    End Sub

    Public Overrides Function Save() As Boolean

        If Me.SubQuoteFirst IsNot Nothing Then
            UpdateUnderlyingPolicyDictionary()
        End If

        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        ' Don't validate if Personal; 4 = Personal
        If SubQuoteFirst.ProgramTypeId = "4" Then Exit Sub

        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Underlying Policies"

        If LobType = QuickQuoteObject.QuickQuoteLobType.Farm AndAlso FilteredPolicyList.Any() = False Then
            Me.ValidationHelper.AddError(lblLobCategoryText, "At least 1 Farm policy or quote number is required.", accordList)
        End If

    End Sub

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
        End If
    End Sub

    Public Sub VerifyPolicy()
        UpdateUnderlyingPolicyDictionary()
        'TryAddPolicyToUnderlyingPolicies()
        Dim UmbrellaValidate = New Helpers.UmbrellaUnderlyingValidation
        UmbrellaValidate.ValidateAndAddUnderlyingPolicies(GoverningStateQuote, _loaderValidationErrors, ddh)

    End Sub

    Private Sub btnAddAnother_Click()
        _addAnotherFlag = True
        Me.Save_FireSaveEvent()
        Populate_FirePopulateEvent()
    End Sub

    Private Sub btnDelete_Click(polNum As String)
        'Remove polNum
        SubQuoteFirst?.UnderlyingPolicies.Remove(FindUmbPolicyByPolicyNumber(polNum.Trim))
        'Remove from DevDictionary
        ddh.RemoveFromMasterValueDictionary(polNum.Trim)

        Populate_FirePopulateEvent()
        Me.Save_FireSaveEvent()

    End Sub

    Private Function FindUmbPolicyByPolicyNumber(polNum As String) As QuickQuoteUnderlyingPolicy
        Return SubQuoteFirst?.UnderlyingPolicies _
                    .Find(Function(up) up.PrimaryPolicyNumber.Equals(polNum, StringComparison.CurrentCultureIgnoreCase)) _
                    .CreateIfNull()
    End Function

    Private Function DoesUmbPolicyExistByPolicyNumber(polNum As String) As Boolean
        Return FindUmbPolicyByPolicyNumber(polNum.Trim).HasValidUnderlyingPolicyNum
    End Function


    Private Sub UpdateUnderlyingPolicyDictionary()
        For Each item As RepeaterItem In Me.rptPolicyList.Items
            Dim PolNum As TextBox = item.FindControl("txtPolicyNumber")
            Dim PolName As Label = item.FindControl("PolicyholderName")

            If String.IsNullOrWhiteSpace(PolNum.Text) <> True AndAlso PolNum.Text <> "temp" Then
                If ddh.IsKeyInMasterValueDictionary(PolNum.Text.Trim) = False Then
                    ddh.AddToMasterValueDictionary(PolNum.Text.Trim, PolName.Text.Trim)
                End If
            End If
        Next
        FilteredPolicyList = ddh.GetUmbrellaDictionaryByLobType()
    End Sub


    Private Sub rptPolicyList_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPolicyList.ItemDataBound

        Dim PolNameRow As HtmlTableRow = e.Item.FindControl("PolicyNameRow")
        Dim PolNum As TextBox = e.Item.FindControl("txtPolicyNumber")
        Dim PolName As Label = e.Item.FindControl("PolicyholderName")
        Dim AddButton As HtmlTableCell = e.Item.FindControl("btnAddAnotherCell")
        Dim DelButton As HtmlTableCell = e.Item.FindControl("btnDeleteCell")
        Dim ErrorRow As Label = e.Item.FindControl("PolicyErrorMessage")
        Dim up As Generic.KeyValuePair(Of String, String) = e.Item.DataItem

        PolNum.Attributes("data-lob") = IFM.VR.Common.Helpers.LOBHelper.GetAbbreviatedLOBPrefix(LobType)

        If String.IsNullOrWhiteSpace(up.Key) <> True AndAlso up.Key <> "temp" Then
            AddButton.Visible = True
            DelButton.Visible = True
            PolNum.Text = up.Key
            PolName.Text = up.Value
            ErrorRow.Text = String.Empty
            'Allow Warnings and Errors by using "*" for warnings
            If _loaderValidationErrors.ContainsKey(up.Key) Then
                If String.IsNullOrWhiteSpace(_loaderValidationErrors.Item(up.Key)) = False AndAlso _loaderValidationErrors.Item(up.Key).StartsWith("*") = False Then
                    Dim V = New Validation.ObjectValidation.ValidationItem(_loaderValidationErrors.Item(up.Key))
                    Me.ValidationHelper.Val_BindValidationItemToControl(PolNum.ClientID, V, Me.MyAccordionList)
                Else
                    ErrorRow.Text = _loaderValidationErrors.Item(up.Key)?.Replace("*", String.Empty)
                End If
            End If

        Else
            AddButton.Visible = True
            DelButton.Visible = True
            PolNum.Text = String.Empty
            PolName.Text = String.Empty
        End If

        If String.IsNullOrWhiteSpace(PolName.Text) = False Then
            PolNameRow.Visible = True
        End If

        If String.IsNullOrWhiteSpace(up.Key) <> True AndAlso up.Key <> "temp" AndAlso String.IsNullOrWhiteSpace(up.Value) AndAlso String.IsNullOrWhiteSpace(ErrorRow.Text) Then
            ErrorRow.Text = "Not Validated"
        End If




    End Sub

    Protected Sub rptPolicyList_btnClick(source As Object, e As RepeaterCommandEventArgs) Handles rptPolicyList.ItemCommand
        If e.CommandName = "btnAdd" Then
            btnAddAnother_Click()
        ElseIf e.CommandName = "btnDelete" Then
            btnDelete_Click(CType(e.Item.FindControl("txtPolicyNumber"), TextBox).Text.Trim)
        End If

    End Sub


#End Region


End Class

