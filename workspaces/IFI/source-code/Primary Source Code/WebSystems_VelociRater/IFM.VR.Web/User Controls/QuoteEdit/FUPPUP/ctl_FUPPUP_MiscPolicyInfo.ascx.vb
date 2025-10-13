Imports IFM.PrimativeExtensions
Imports IFM.VR.Common.Helpers.FARM
Imports IFM.VR.Web.ENUMHelper
Imports QuickQuote.CommonObjects.Umbrella
Public Class ctl_FUPPUP_MiscPolicyInfo
    Inherits VRControlBase

    Protected Shared ReadOnly _POLICY_INFO_TYPE_ID_MISCELLANEOUS_LIABILITY = $"{PolicyTypeId.MiscellaneousLiability:d}"
    Protected Const _MISCELLANEOUS_LIABILITY_TYPE_ID_SWIMMING_POOL = "1"
    Protected Const _MISCELLANEOUS_LIABILITY_TYPE_ID_FAMILY_FARM_CORP = "3"
    Protected Const _MISCELLANEOUS_LIABILITY_TYPE_ID_TRAMPOLINE = "2"

    Public ReadOnly Property MainFarmPolicy() As QuickQuoteUnderlyingPolicy
        Get
            Dim _MainFarmPolicy As QuickQuoteUnderlyingPolicy = Nothing
            If SubQuoteFirst.UnderlyingPolicies IsNot Nothing AndAlso SubQuoteFirst.UnderlyingPolicies.Count > 0 Then
                _MainFarmPolicy = SubQuoteFirst.UnderlyingPolicies.FirstOrDefault(Function(x As QuickQuoteUnderlyingPolicy) IFM.VR.Common.Helpers.LOBHelper.GetLobFromPrefix_QuoteOrPolicy(x.PrimaryPolicyNumber) = QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm)
                If _MainFarmPolicy?.LobId <> Nothing Then
                    Return _MainFarmPolicy
                Else
                    Return Nothing
                End If
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property IgnoreAndHideSwimmingPoolCheckbox As Boolean
        Get
            Return SwimmingPoolUnitsHelper.SwimmingPoolSettings.EnabledFlag AndAlso Me.Quote.EffectiveDate <> "" AndAlso
                   CDate(Me.Quote.EffectiveDate) >= SwimmingPoolUnitsHelper.SwimmingPoolSettings.StartDate
        End Get
    End Property

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()

        If IgnoreAndHideSwimmingPoolCheckbox Then dvSwimmingPool.Visible = False

        If SubQuoteFirst.ProgramTypeId = "4" Then
            'personal 
            Me.pnlMiscLiab.Visible = False

        Else
            'farm
            If MainFarmPolicy IsNot Nothing Then
                Me.pnlMiscLiab.Visible = True

                If hasFfCorp() Then
                    chkAddInsurFamFarmCorp.Checked = True
                    dvAFCNumPer.Attributes.Add("style", "display:'';")
                    ctlFamFarmCorpList.FfcDescriptionList = GetFfCorpList()
                    ctlFamFarmCorpList.Populate()
                Else
                    chkAddInsurFamFarmCorp.Checked = False
                    dvAFCNumPer.Attributes.Add("style", "display: none;")
                    ctlFamFarmCorpList.FfcDescriptionList = New List(Of String)
                    ctlFamFarmCorpList.Populate()
                End If

                If Not IgnoreAndHideSwimmingPoolCheckbox Then chkSwimmingPool.Checked = HasSwimmingPool()
            Else
                Me.pnlMiscLiab.Visible = False
                chkAddInsurFamFarmCorp.Checked = False
                'ctlFamFarmCorpList.FfcDescriptionList = New List(Of String)
                ctlFamFarmCorpList.Populate()
                If Not IgnoreAndHideSwimmingPoolCheckbox Then chkSwimmingPool.Checked = False
            End If
        End If
        PopulateChildControls()
    End Sub

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'found in VrFarmLine.js - Same functionality as FamMedPay
        Dim scriptGenTextboxInput As String = "ConfirmDeSelectedChkbox(""" + chkAddInsurFamFarmCorp.ClientID + """);"
        chkAddInsurFamFarmCorp.Attributes.Add("onclick", scriptGenTextboxInput)


        Dim scriptSwimPoolDeselect As String = "ConfirmDeSelectedChkbox(""" + chkSwimmingPool.ClientID + """);"
        chkSwimmingPool.Attributes.Add("onclick", scriptSwimPoolDeselect)

        'Info Popup
        lblAddInsurFamFarmCorp.Attributes.Add("onclick", "InitFarmPopupInfo('dvFFCPopup', 'Additional Insured Family Farm Corp'); return false;")
        btnFFCOK.Attributes.Add("onclick", "CloseFarmPopupInfo('dvFFCPopup'); return false;")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub chkAddInsurFamFarmCorp_click(sender As Object, e As EventArgs) Handles chkAddInsurFamFarmCorp.CheckedChanged
        'ctlFamFarmCorpList.FfcDescriptionList = New List(Of String)
        ctlFamFarmCorpList.Populate()
        'SubQuoteFirst.UnderlyingPolicies(0).PolicyInfos.RemoveAll(Function(x As PolicyInfo) x.TypeId = PolicyTypeId.MiscellaneousLiability)
        If chkAddInsurFamFarmCorp.Checked Then
            dvAFCNumPer.Attributes.Add("style", "display:'';")
        Else

            dvAFCNumPer.Attributes.Add("style", "display: none;")
            ClearFfCorp()
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        SaveChildControls()

        If MainFarmPolicy IsNot Nothing Then
            If Not IgnoreAndHideSwimmingPoolCheckbox Then
                If Me.chkSwimmingPool.Checked Then
                    SetSwimmingPool(True)
                Else
                    SetSwimmingPool(False)
                End If
            End If

            If Me.chkAddInsurFamFarmCorp.Checked Then
                SetFfCorp(ctlFamFarmCorpList.FfcDescriptionList)
            End If
        End If

        Return True


    End Function

    Protected Function GetMiscellaneousLiabilityPolicyInfo() As PolicyInfo
        Dim retval As PolicyInfo = Nothing

        If MainFarmPolicy IsNot Nothing AndAlso MainFarmPolicy.PolicyInfos IsNot Nothing Then
            retval = MainFarmPolicy.PolicyInfos.FirstOrDefault(Function(p) p IsNot Nothing AndAlso p.TypeId.TryToGetInt32 = PolicyTypeId.MiscellaneousLiability)
        End If

        Return retval
    End Function

    Protected Function CreateMiscellaneousLiabilityPolicyInfo() As PolicyInfo
        Return New PolicyInfo() With {
                                        .MiscellaneousLiabilities = New List(Of MiscellaneousLiability),
                                        .PolicyTypeId = $"{PolicyTypeId.MiscellaneousLiability:d}",
                                        .SetParent = MainFarmPolicy
                                     }
    End Function

    Protected Function GetSwimmingPoolLiablity() As MiscellaneousLiability
        Dim retval As MiscellaneousLiability = Nothing

        If MainFarmPolicy IsNot Nothing Then

            Dim policyTypeInfo As PolicyInfo = GetMiscellaneousLiabilityPolicyInfo()
            If policyTypeInfo IsNot Nothing AndAlso policyTypeInfo.MiscellaneousLiabilities IsNot Nothing Then
                retval = policyTypeInfo.MiscellaneousLiabilities.FirstOrDefault(Function(m) m.TypeId = _MISCELLANEOUS_LIABILITY_TYPE_ID_SWIMMING_POOL)
            End If
        End If
        Return retval
    End Function
    Public Function HasSwimmingPool() As Boolean
        Dim retval As Boolean = False

        retval = (GetSwimmingPoolLiablity() IsNot Nothing)

        Return retval
    End Function

    Public Sub SetSwimmingPool(AddPool As Boolean)
        If AddPool = True And HasSwimmingPool() = False Then
            AddSwimmingPool()
        ElseIf AddPool = False And HasSwimmingPool() = True Then
            RemoveSwimmingPool()
        End If
    End Sub

    Public Sub AddSwimmingPool()
        If MainFarmPolicy IsNot Nothing Then
            Dim origPInfo = GetMiscellaneousLiabilityPolicyInfo()
            Dim pInfo = If(origPInfo, CreateMiscellaneousLiabilityPolicyInfo())

            If Me.chkSwimmingPool.Checked Then
                Dim MiscLiab = New MiscellaneousLiability()
                MiscLiab.TypeId = _MISCELLANEOUS_LIABILITY_TYPE_ID_SWIMMING_POOL
                MiscLiab.NumberOfItems = "1"
                MiscLiab.SetParent = pInfo
                pInfo.MiscellaneousLiabilities.Add(MiscLiab)
                If pInfo.MiscellaneousLiabilities.Count() = 1 AndAlso origPInfo Is Nothing Then
                    MainFarmPolicy.PolicyInfos.Add(pInfo)
                End If
            End If

        End If
    End Sub

    Public Sub RemoveSwimmingPool()
        If MainFarmPolicy IsNot Nothing Then
            Dim policyInfo = GetMiscellaneousLiabilityPolicyInfo()
            Dim swimmingPool = GetSwimmingPoolLiablity()

            If swimmingPool IsNot Nothing Then
                policyInfo.MiscellaneousLiabilities.Remove(swimmingPool)
            End If
            'For Each PolicyTypeInfo As PolicyInfo In MainFarmPolicy.PolicyInfos
            '    If PolicyTypeInfo.TypeId.TryToGetInt32 = PolicyTypeId.MiscellaneousLiability AndAlso PolicyTypeInfo.MiscellaneousLiabilities IsNot Nothing Then
            '        For Each misc As MiscellaneousLiability In PolicyTypeInfo.MiscellaneousLiabilities
            '            If misc.TypeId = "1" Then
            '                MainFarmPolicy.PolicyInfos.Remove(PolicyTypeInfo)
            '                Return
            '            End If
            '        Next
            '    End If
            'Next
        End If
    End Sub

    Public Function hasFfCorp() As Boolean
        Dim retval As Boolean = False
        If MainFarmPolicy IsNot Nothing Then
            Dim policyInfo = GetMiscellaneousLiabilityPolicyInfo()

            If policyInfo IsNot Nothing AndAlso policyInfo.MiscellaneousLiabilities IsNot Nothing Then
                retval = policyInfo.MiscellaneousLiabilities.Any(Function(misc) misc.TypeId = _MISCELLANEOUS_LIABILITY_TYPE_ID_FAMILY_FARM_CORP)
            End If
            'For Each PolicyTypeInfo As PolicyInfo In MainFarmPolicy.PolicyInfos
            '    If PolicyTypeInfo.TypeId.TryToGetInt32 = PolicyTypeId.MiscellaneousLiability AndAlso PolicyTypeInfo.MiscellaneousLiabilities IsNot Nothing Then
            '        For Each misc As MiscellaneousLiability In PolicyTypeInfo.MiscellaneousLiabilities
            '            If misc.TypeId = _MISCELLANEOUS_LIABILITY_TYPE_ID_FAMILY_FARM_CORP Then
            '                Return True
            '            End If
            '        Next
            '    End If
            'Next
        End If
        Return retval
    End Function

    Public Function GetFfCorpList() As List(Of String)
        Dim DescList As List(Of String) = New List(Of String)
        If MainFarmPolicy IsNot Nothing Then
            Dim policyInfo = GetMiscellaneousLiabilityPolicyInfo()

            If policyInfo IsNot Nothing AndAlso policyInfo.MiscellaneousLiabilities IsNot Nothing Then
                DescList.AddRange(policyInfo.MiscellaneousLiabilities.Where(Function(misc) misc.TypeId = _MISCELLANEOUS_LIABILITY_TYPE_ID_FAMILY_FARM_CORP) _
                                                                      .Select(Of String)(Function(misc) misc.Description))
            End If
            'For Each PolicyTypeInfo As PolicyInfo In MainFarmPolicy.PolicyInfos
            '    If PolicyTypeInfo.TypeId.TryToGetInt32 = PolicyTypeId.MiscellaneousLiability AndAlso PolicyTypeInfo.MiscellaneousLiabilities IsNot Nothing Then
            '        For Each misc As MiscellaneousLiability In PolicyTypeInfo.MiscellaneousLiabilities
            '            If misc.TypeId = _MISCELLANEOUS_LIABILITY_TYPE_ID_FAMILY_FARM_CORP Then
            '                DescList.Add(misc.Description)
            '            End If
            '        Next
            '    End If
            'Next
        End If
        Return DescList
    End Function

    Public Sub SetFfCorp(listFfc As List(Of String))
        'Remove PolcyInfo with Family Farm Corp
        ClearFfCorp()

        'Add All Descriptions back into a PolicyInfo
        If MainFarmPolicy IsNot Nothing Then
            Dim origPInfo = GetMiscellaneousLiabilityPolicyInfo()
            Dim pInfo = If(origPInfo, CreateMiscellaneousLiabilityPolicyInfo())

            If Me.chkAddInsurFamFarmCorp.Checked AndAlso listFfc?.Count > 0 Then
                ClearFfCorp()

                For Each desc As String In listFfc
                    If String.IsNullOrWhiteSpace(desc) = False Then
                        Dim MiscLiab = New MiscellaneousLiability()
                        MiscLiab.TypeId = _MISCELLANEOUS_LIABILITY_TYPE_ID_FAMILY_FARM_CORP
                        MiscLiab.NumberOfItems = "1"
                        MiscLiab.Description = desc
                        MiscLiab.SetParent = pInfo
                        If pInfo.MiscellaneousLiabilities IsNot Nothing Then
                            pInfo.MiscellaneousLiabilities.Add(MiscLiab)
                        End If
                    End If
                Next
                If pInfo.MiscellaneousLiabilities IsNot Nothing AndAlso origPInfo Is Nothing Then
                    ' And pInfo.MiscellaneousLiabilities.Count = 1 Then
                    MainFarmPolicy.PolicyInfos.Add(pInfo)
                End If

            End If
        End If
    End Sub

    Public Sub ClearFfCorp()
        'Remove PolcyInfo with Family Farm Corp
        If MainFarmPolicy IsNot Nothing Then
            Dim policyInfo = GetMiscellaneousLiabilityPolicyInfo()

            If policyInfo IsNot Nothing AndAlso policyInfo.MiscellaneousLiabilities IsNot Nothing Then
                For Each fFCorp As MiscellaneousLiability In policyInfo.MiscellaneousLiabilities.Where(Function(misc) misc.TypeId = _MISCELLANEOUS_LIABILITY_TYPE_ID_FAMILY_FARM_CORP).ToArray()
                    policyInfo.MiscellaneousLiabilities.Remove(fFCorp)
                Next
            End If
            'For Each PolicyTypeInfo As PolicyInfo In MainFarmPolicy.PolicyInfos
            '    If PolicyTypeInfo.TypeId.TryToGetInt32 = PolicyTypeId.MiscellaneousLiability Then
            '        If PolicyTypeInfo.MiscellaneousLiabilities IsNot Nothing Then
            '            For Each misc As MiscellaneousLiability In PolicyTypeInfo.MiscellaneousLiabilities
            '                If misc.TypeId = _MISCELLANEOUS_LIABILITY_TYPE_ID_FAMILY_FARM_CORP Then
            '                    MainFarmPolicy.PolicyInfos.Remove(PolicyTypeInfo)
            '                    Return
            '                End If
            '            Next
            '        Else
            '            MainFarmPolicy.PolicyInfos.Remove(PolicyTypeInfo)
            '        End If
            '    End If
            'Next
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it

        ' Don't validate if Personal; 4 = Personal
        If SubQuoteFirst.ProgramTypeId = "4" Then Exit Sub

        MyBase.ValidateControl(valArgs)

        Me.ValidationHelper.GroupName = "Underlying Policies"

    End Sub
End Class