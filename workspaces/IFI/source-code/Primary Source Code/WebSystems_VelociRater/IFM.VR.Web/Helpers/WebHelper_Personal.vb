Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Runtime.CompilerServices
Imports QuickQuote.CommonMethods.QuickQuoteHelperClass
Imports System.Data.SqlClient
Imports System.Windows.Interop
Imports IFM.VR.Common.QuoteSave
Imports QuickQuote.CommonObjects.QuickQuoteObject
Imports System.ComponentModel
Imports IFM.PrimativeExtensions

Namespace Helpers

    Public Class WebHelper_Personal

        Public Shared Sub SetdropDownFromValue(ByRef dd As DropDownList, ByVal ddVal As String)
            ' sets first occurrence then bolts
            dd.ClearSelection()
            If Not String.IsNullOrWhiteSpace(ddVal) Then
                ddVal = ddVal.ToLower().Trim()
                For Each item As ListItem In dd.Items
                    If item.Value.ToLower().Trim() = ddVal Then
                        item.Selected = True
                        Return
                    End If
                Next
            End If
#If DEBUG Then
            If String.IsNullOrWhiteSpace(ddVal) = False AndAlso ddVal.Trim() <> "-1" AndAlso ddVal.Trim() <> "0" Then ' sometimes you send string.empty or '-1' to unselect a dropdown so it is not an issue if that is what is happening
                'Debugger.Break() ' didn't select item - item is not in dropdown
            End If
#End If

        End Sub

        Public Shared Sub SetdropDownFromValue_ForceSeletion(ByRef dd As DropDownList, ByVal ddVal As String, ByVal ddText As String)  '6-19-14
            dd.ClearSelection()
            If Not String.IsNullOrWhiteSpace(ddVal) Then
                Dim hasVal As Boolean = False
                ddVal = ddVal.ToLower().Trim()
                For Each item As ListItem In dd.Items
                    If item.Value.ToLower().Trim() = ddVal Then
                        hasVal = True
                        Exit For
                    End If
                Next
                If hasVal = False Then
                    dd.Items.Add(New ListItem(ddText.ToUpper(), ddVal))
                End If
                SetdropDownFromValue(dd, ddVal)
            End If

        End Sub

        ''' <summary>
        ''' Will force an option into a DDL if it didn't exist before and select it. Useful for
        ''' adding Diamond options that normally don't exist in VR (endorsements).
        ''' DiamondStaticData.xml should contain this option for the text lookup.
        ''' </summary>
        ''' <param name="ddl">List to add option.</param>
        ''' <param name="ddVal">Value of the option to add.</param>
        ''' <param name="className">Class name to lookup in DiamondStaticData.xml</param>
        ''' <param name="propertyName">Property name of the "className" to lookup in DiamondStaticData.xml</param>
        ''' <param name="lob">Optional: LOB to restrict values in lookup.</param>
        ''' <param name="persOrComm">Optional: Personal or Comm. Lines to restrict values in lookup.</param>
        ''' <param name="forcedLOB">
        ''' Optional: Force LOB items only, do not use non-LOB-specific items for lookup if "True"
        ''' </param>
        Public Shared Sub SetDropDownValue_ForceDiamondValue(ByRef ddl As DropDownList, ByVal ddVal As String, className As QuickQuoteClassName, propertyName As QuickQuotePropertyName, Optional lob As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None, Optional persOrComm As PersOrComm = PersOrComm.None, Optional forcedLOB As Boolean = False)
            Dim QQHelper = New QuickQuoteHelperClass
            If ddl.Items.FindByValue(ddVal) Is Nothing Then
                Dim TypeDescription As String = QQHelper.GetStaticDataTextForValue_ForceLob(forcedLOB, className, propertyName, ddVal, lob)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddl, ddVal, TypeDescription)
            Else
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(ddl, ddVal)
            End If
        End Sub


        Public Shared Sub SetdropDownFromText(ByRef dd As DropDownList, ByVal ddVal As String)
            ' sets first occurrence then bolts
            dd.ClearSelection()
            If Not String.IsNullOrWhiteSpace(ddVal) Then
                ddVal = ddVal.ToLower().Trim()
                For Each item As ListItem In dd.Items
                    If item.Text.ToLower().Trim() = ddVal Then
                        item.Selected = True
                        Return
                    End If
                Next
            End If

        End Sub
        Public Shared Sub SetdropDownFromText_ForceDiamondText(ByRef ddl As DropDownList, ByVal ddText As String, className As QuickQuoteClassName, propertyName As QuickQuotePropertyName, Optional lob As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None, Optional persOrComm As PersOrComm = PersOrComm.None)
            ' sets first occurrence then bolts
            ddl.ClearSelection()
            Dim QQHelper = New QuickQuoteHelperClass
            If ddl.Items.FindByText(ddText) Is Nothing Then
                Dim Value As String = QQHelper.GetStaticDataValueForText(className, propertyName, ddText, lob, persOrComm)
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(ddl, Value, ddText)
            Else
                IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(ddl, ddText)
            End If

        End Sub

        Public Shared Sub AddDropDownValueIfMissing(ByRef dd As DropDownList, ddVal As String, ddText As String)

            If Not String.IsNullOrWhiteSpace(ddVal) Then
                Dim hasVal As Boolean = False
                ddVal = ddVal.ToLower().Trim()
                For Each item As ListItem In dd.Items
                    If item.Value.ToLower().Trim() = ddVal Then
                        hasVal = True
                        Exit For
                    End If
                Next
                If hasVal = False Then
                    dd.Items.Add(New ListItem(ddText.ToUpper(), ddVal))
                End If
            End If

        End Sub

        Public Shared Function QuoteHasLocations(ByRef QuoteObject As QuickQuoteObject) As Boolean
            If QuoteObject IsNot Nothing AndAlso QuoteObject.Locations IsNot Nothing AndAlso QuoteObject.Locations.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function QuoteHasSectionIICoverages(ByRef QuoteObject As QuickQuoteObject, Optional ByRef ListOfLocationNumbersWithSectionIICoverages As List(Of Integer) = Nothing) As Boolean
            If QuoteHasLocations(QuoteObject) Then
                For Each Loc As QuickQuoteLocation In QuoteObject.Locations
                    If Loc IsNot Nothing AndAlso Loc.SectionIICoverages IsNot Nothing AndAlso Loc.SectionIICoverages.Count > 0 Then
                        ListOfLocationNumbersWithSectionIICoverages.Add(Loc.LocationNum)
                    End If
                Next
                If ListOfLocationNumbersWithSectionIICoverages IsNot Nothing AndAlso ListOfLocationNumbersWithSectionIICoverages.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        End Function

        Public Shared Function IsTesting() As Boolean
            Return ConfigurationManager.AppSettings("TestOrProd").ToLower() = "test"
        End Function

        Public Shared Sub AddStartUpScript(ByVal jscode As String, ByVal response As HttpResponse, ByVal scriptManager As ScriptManager, ByVal control As Control)
            ScriptManager.RegisterStartupScript(control, control.GetType(), Guid.NewGuid().ToString, jscode, True)
        End Sub

        Public Shared Function IsStaffUser() As Boolean
            Dim qqHelper As New QuickQuoteHelperClass()
            Return qqHelper.IsHomeOfficeStaffUser()
        End Function

        Public Shared Function SetAccordionOpenTabIndex(accordionID As String, openTabIndex As String) As String
            Return "$(""#" + accordionID + """).accordion(""option"",""active""," + openTabIndex + ");"
        End Function

        Public Shared Function AddValidationJumpLogic(clientId As String) As String
            Return "ifm.vr.ui.FlashFocusThenScrollToElement(""" + clientId + """); $(this).css(""color"",""blue"");"
        End Function

        Public Shared Sub GatherRatingErrorsAndWarnings(ratedQuote As QuickQuote.CommonObjects.QuickQuoteObject, ValidationHelper As ControlValidationHelper)
            If ratedQuote.ValidationItems IsNot Nothing Then
                For Each vI In ratedQuote.ValidationItems
                    If vI.ValidationSeverityType = QuickQuoteValidationItem.QuickQuoteValidationSeverityType.ValidationError Then
                        ValidationHelper.AddError(vI.Message)
                    End If
                Next
            End If
        End Sub

        Public Shared Sub RemoveQuoteIdFromSessionHistory(Session As HttpSessionState, quoiteid As String)
            Dim sessionActivity As List(Of SessionQuoteHistoryItem) = Nothing
            If Session("ss_RecentQuoteActivity") IsNot Nothing Then
                sessionActivity = DirectCast(Session("ss_RecentQuoteActivity"), List(Of SessionQuoteHistoryItem))
                If String.IsNullOrWhiteSpace(quoiteid) = False Then
                    Dim exists = (From s As SessionQuoteHistoryItem In sessionActivity Where s.QuoteId = quoiteid Select s).ToList()

                    If exists.Any() Then
                        'remove then add new later
                        For Each i In exists
                            sessionActivity.Remove(i)
                        Next

                    End If
                End If

            End If

        End Sub

        '3/7/2019 - moved here from TreeView; changed from Private to Public Shared; also added methods for HtmlControl
        Public Shared Sub AddStyleToWebControl(ByVal ctrl As WebControl, ByVal styleName As String, ByVal styleValue As String, Optional ByVal allowEmptyStyleValue As Boolean = False)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(styleName) = False AndAlso (String.IsNullOrWhiteSpace(styleValue) = False OrElse allowEmptyStyleValue = True) Then
                If ctrl.Style(styleName) IsNot Nothing Then
                    ctrl.Style(styleName) = styleValue
                Else
                    ctrl.Style.Add(styleName, styleValue)
                End If
            End If
        End Sub
        Public Shared Sub AddStyleToGenericControl(ByVal ctrl As HtmlGenericControl, ByVal styleName As String, ByVal styleValue As String, Optional ByVal allowEmptyStyleValue As Boolean = False)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(styleName) = False AndAlso (String.IsNullOrWhiteSpace(styleValue) = False OrElse allowEmptyStyleValue = True) Then
                If ctrl.Style(styleName) IsNot Nothing Then
                    ctrl.Style(styleName) = styleValue
                Else
                    ctrl.Style.Add(styleName, styleValue)
                End If
            End If
        End Sub
        Public Shared Sub AddStyleToHtmlControl(ByVal ctrl As HtmlControl, ByVal styleName As String, ByVal styleValue As String, Optional ByVal allowEmptyStyleValue As Boolean = False)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(styleName) = False AndAlso (String.IsNullOrWhiteSpace(styleValue) = False OrElse allowEmptyStyleValue = True) Then
                If ctrl.Style(styleName) IsNot Nothing Then
                    ctrl.Style(styleName) = styleValue
                Else
                    ctrl.Style.Add(styleName, styleValue)
                End If
            End If
        End Sub
        Public Shared Sub RemoveStyleFromWebControl(ByVal ctrl As WebControl, ByVal styleName As String)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(styleName) = False Then
                If ctrl.Style(styleName) IsNot Nothing Then
                    ctrl.Style.Remove(styleName)
                End If
            End If
        End Sub
        Public Shared Sub RemoveStyleFromGenericControl(ByVal ctrl As HtmlGenericControl, ByVal styleName As String)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(styleName) = False Then
                If ctrl.Style(styleName) IsNot Nothing Then
                    ctrl.Style.Remove(styleName)
                End If
            End If
        End Sub
        Public Shared Sub RemoveStyleFromHtmlControl(ByVal ctrl As HtmlControl, ByVal styleName As String)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(styleName) = False Then
                If ctrl.Style(styleName) IsNot Nothing Then
                    ctrl.Style.Remove(styleName)
                End If
            End If
        End Sub
        Public Shared Sub AddAttributeToWebControl(ByVal ctrl As WebControl, ByVal attributeName As String, ByVal attributeValue As String, Optional ByVal allowEmptyAttributeValue As Boolean = False)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False AndAlso (String.IsNullOrWhiteSpace(attributeValue) = False OrElse allowEmptyAttributeValue = True) Then
                If ctrl.Attributes(attributeName) IsNot Nothing Then
                    ctrl.Attributes(attributeName) = attributeValue
                Else
                    ctrl.Attributes.Add(attributeName, attributeValue)
                End If
            End If
        End Sub
        Public Shared Sub AddAttributeToGenericControl(ByVal ctrl As HtmlGenericControl, ByVal attributeName As String, ByVal attributeValue As String, Optional ByVal allowEmptyAttributeValue As Boolean = False)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False AndAlso (String.IsNullOrWhiteSpace(attributeValue) = False OrElse allowEmptyAttributeValue = True) Then
                If ctrl.Attributes(attributeName) IsNot Nothing Then
                    ctrl.Attributes(attributeName) = attributeValue
                Else
                    ctrl.Attributes.Add(attributeName, attributeValue)
                End If
            End If
        End Sub
        Public Shared Function GetAttributeValueForGenericControl(ByVal ctrl As HtmlGenericControl, ByVal attributeName As String) As String
            Dim attVal As String = ""
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False Then
                If ctrl.Attributes(attributeName) IsNot Nothing Then
                    attVal = ctrl.Attributes(attributeName).ToString
                End If
            End If
            Return attVal
        End Function
        Public Shared Sub AddAttributeToHtmlControl(ByVal ctrl As HtmlControl, ByVal attributeName As String, ByVal attributeValue As String, Optional ByVal allowEmptyAttributeValue As Boolean = False)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False AndAlso (String.IsNullOrWhiteSpace(attributeValue) = False OrElse allowEmptyAttributeValue = True) Then
                If ctrl.Attributes(attributeName) IsNot Nothing Then
                    ctrl.Attributes(attributeName) = attributeValue
                Else
                    ctrl.Attributes.Add(attributeName, attributeValue)
                End If
            End If
        End Sub
        Public Shared Sub RemoveAttributeFromWebControl(ByVal ctrl As WebControl, ByVal attributeName As String)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False Then
                If ctrl.Attributes(attributeName) IsNot Nothing Then
                    ctrl.Attributes.Remove(attributeName)
                End If
            End If
        End Sub
        Public Shared Sub RemoveAttributeFromGenericControl(ByVal ctrl As HtmlGenericControl, ByVal attributeName As String)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False Then
                If ctrl.Attributes(attributeName) IsNot Nothing Then
                    ctrl.Attributes.Remove(attributeName)
                End If
            End If
        End Sub
        Public Shared Sub RemoveAttributeFromHtmlControl(ByVal ctrl As HtmlControl, ByVal attributeName As String)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False Then
                If ctrl.Attributes(attributeName) IsNot Nothing Then
                    ctrl.Attributes.Remove(attributeName)
                End If
            End If
        End Sub
        'added 7/5/2019
        Public Shared Sub ReplaceTextInAttributeValueForWebControl(ByVal ctrl As WebControl, ByVal attributeName As String, ByVal attributeValueTextOld As String, ByVal attributeValueTextNew As String)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False AndAlso String.IsNullOrWhiteSpace(attributeValueTextOld) = False AndAlso ctrl.Attributes(attributeName) IsNot Nothing AndAlso String.IsNullOrWhiteSpace(ctrl.Attributes(attributeName).ToString) = False AndAlso ctrl.Attributes(attributeName).ToString.Contains(attributeValueTextOld) = True Then
                ctrl.Attributes(attributeName) = ctrl.Attributes(attributeName).ToString.Replace(attributeValueTextOld, attributeValueTextNew)
            End If
        End Sub
        Public Shared Sub ReplaceTextInAttributeValueForGenericControl(ByVal ctrl As HtmlGenericControl, ByVal attributeName As String, ByVal attributeValueTextOld As String, ByVal attributeValueTextNew As String)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False AndAlso String.IsNullOrWhiteSpace(attributeValueTextOld) = False AndAlso ctrl.Attributes(attributeName) IsNot Nothing AndAlso String.IsNullOrWhiteSpace(ctrl.Attributes(attributeName).ToString) = False AndAlso ctrl.Attributes(attributeName).ToString.Contains(attributeValueTextOld) = True Then
                ctrl.Attributes(attributeName) = ctrl.Attributes(attributeName).ToString.Replace(attributeValueTextOld, attributeValueTextNew)
            End If
        End Sub
        Public Shared Sub ReplaceTextInAttributeValueForHtmlControl(ByVal ctrl As HtmlControl, ByVal attributeName As String, ByVal attributeValueTextOld As String, ByVal attributeValueTextNew As String)
            If ctrl IsNot Nothing AndAlso String.IsNullOrWhiteSpace(attributeName) = False AndAlso String.IsNullOrWhiteSpace(attributeValueTextOld) = False AndAlso ctrl.Attributes(attributeName) IsNot Nothing AndAlso String.IsNullOrWhiteSpace(ctrl.Attributes(attributeName).ToString) = False AndAlso ctrl.Attributes(attributeName).ToString.Contains(attributeValueTextOld) = True Then
                ctrl.Attributes(attributeName) = ctrl.Attributes(attributeName).ToString.Replace(attributeValueTextOld, attributeValueTextNew)
            End If
        End Sub

        'added 6/10/2019 to consolidate redirect logic to one spot
        Public Shared Sub RedirectToQuotePage(ByVal tranType As QuickQuoteObject.QuickQuoteTransactionType, ByVal lobType As QuickQuoteObject.QuickQuoteLobType, Optional ByVal quoteId As Integer = 0, Optional ByVal policyId As Integer = 0, Optional ByVal policyImageNum As Integer = 0, Optional ByVal quoteStatus As QuickQuoteXML.QuickQuoteStatusType = Nothing, Optional ByVal goToApp As Boolean = False, Optional ByVal workflowQueryString As String = "", Optional ByVal isBillingUpdate As Boolean = False, Optional ByVal AdditionalQueryStringParams As String = "")
            If System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteLobType), lobType) = True AndAlso lobType <> QuickQuoteObject.QuickQuoteLobType.None Then
                Dim queryString As String = ""
                Select Case tranType
                    Case QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage, QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote
                        If policyId > 0 AndAlso policyImageNum > 0 Then
                            If tranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                queryString = "?EndorsementPolicyIdAndImageNum="
                            Else
                                queryString = "?ReadOnlyPolicyIdAndImageNum="
                            End If
                            queryString &= policyId.ToString & "|" & policyImageNum.ToString
                        End If
                    Case Else
                        If quoteId > 0 Then
                            queryString = "?QuoteId=" & quoteId.ToString
                        End If
                End Select
                If String.IsNullOrWhiteSpace(queryString) = False Then
                    Dim setWorkflowQueryString As Boolean = False
                    If System.Enum.IsDefined(GetType(QuickQuoteXML.QuickQuoteStatusType), quoteStatus) = True Then
                        Select Case quoteStatus
                            Case QuickQuoteXML.QuickQuoteStatusType.AppGapRated, QuickQuoteXML.QuickQuoteStatusType.AppGapRatingFailed, QuickQuoteXML.QuickQuoteStatusType.QuoteRated, QuickQuoteXML.QuickQuoteStatusType.QuoteRatingFailed
                                queryString &= "&" & Common.Workflow.Workflow.WorkFlowSection_qs & "=" & Common.Workflow.Workflow.WorkflowSection.summary.ToString()
                                setWorkflowQueryString = True
                        End Select
                    End If
                    If setWorkflowQueryString = False AndAlso String.IsNullOrWhiteSpace(workflowQueryString) = False Then
                        queryString &= "&" & Common.Workflow.Workflow.WorkFlowSection_qs & "=" & workflowQueryString
                    End If

                    Dim pageUrl As String = ""
                    Dim pageKeyName As String = ""
                    If isBillingUpdate = True Then 'added IF 7/27/2019; original logic in ELSE
                        pageUrl = "VREBillingUpdate.aspx"
                    Else
                        Select Case lobType
                            Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                                If goToApp = True Then
                                    pageKeyName = "QuickQuote_PPA_App"
                                Else
                                    pageKeyName = "QuickQuote_PPA_Input"
                                End If
                            Case QuickQuoteObject.QuickQuoteLobType.HomePersonal
                                If goToApp = True Then
                                    pageKeyName = "QuickQuote_HOM_App"
                                Else
                                    pageKeyName = "QuickQuote_HOM_Input"
                                End If
                            Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal
                                If goToApp = True Then
                                    pageKeyName = "QuickQuote_DFR_App"
                                Else
                                    pageKeyName = "QuickQuote_DFR_Input"
                                End If
                            Case QuickQuoteObject.QuickQuoteLobType.Farm
                                If goToApp = True Then
                                    pageKeyName = "QuickQuote_FAR_App"
                                Else
                                    pageKeyName = "QuickQuote_FAR_Input"
                                End If
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP
                                If tranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                    pageKeyName = "QuickQuote_BOP_Endo_NewLook"
                                Else
                                    If goToApp = True Then
                                        pageUrl = "VR3BOPApp.aspx"
                                    Else
                                        pageKeyName = "QuickQuote_BOP_Quote_NewLook"
                                    End If
                                End If

                            Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation
                                If goToApp = True Then
                                    pageUrl = "VR3WCPApp.aspx"
                                Else
                                    pageKeyName = "QuickQuote_WCP_Quote_NewLook"
                                End If
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto
                                If goToApp = True Then
                                    pageUrl = "VR3CAPApp.aspx"
                                Else
                                    pageKeyName = "QuickQuote_CAP_Quote_NewLook"
                                End If
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability
                                If goToApp = True Then
                                    pageUrl = "VR3CGLApp.aspx"
                                Else
                                    pageKeyName = "QuickQuote_CGL_Quote_NewLook"
                                End If
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty
                                If goToApp = True Then
                                    pageUrl = "VR3CPRApp.aspx"
                                Else
                                    pageKeyName = "QuickQuote_CPR_Quote_NewLook"
                                End If
                            Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage
                                If tranType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                    pageKeyName = "QuickQuote_CPP_Endo_NewLook"
                                Else
                                    If goToApp = True Then
                                        pageUrl = "VR3CPPApp.aspx"
                                    Else
                                        pageKeyName = "QuickQuote_CPP_Quote_NewLook"
                                    End If
                                End If
                            Case QuickQuoteObject.QuickQuoteLobType.UmbrellaPersonal
                                If goToApp = True Then
                                    pageKeyName = "QuickQuote_FUPPUP_App"
                                Else
                                    pageKeyName = "QuickQuote_FUPPUP_Input"
                                End If
                        End Select
                    End If
                    If String.IsNullOrWhiteSpace(pageUrl) = True AndAlso String.IsNullOrWhiteSpace(pageKeyName) = False Then
                        pageUrl = QuickQuoteHelperClass.configAppSettingValueAsString(pageKeyName)
                    End If
                    If String.IsNullOrWhiteSpace(pageUrl) = False Then
                        If String.IsNullOrWhiteSpace(AdditionalQueryStringParams) = False Then
                            queryString &= AdditionalQueryStringParams
                        End If
                        pageUrl &= queryString
                        HttpContext.Current.Response.Redirect(pageUrl, True)
                    End If
                End If
            End If
        End Sub

        Public Shared Sub RedirectToStartEndorsementPage(policyID As Integer, policyImageNum As Integer, isBillingUpdate As Boolean)
            Dim startNewEndorsementPageUrl As String = QuickQuoteHelperClass.configAppSettingValueAsString("VR_StartNewEndorsementPageUrl")
            If String.IsNullOrWhiteSpace(startNewEndorsementPageUrl) Then
                startNewEndorsementPageUrl = "VREPolicyInfo.aspx?ReadOnlyPolicyIdAndImageNum="
            End If
            HttpContext.Current.Response.Redirect(startNewEndorsementPageUrl & policyID.ToString() & "|" & policyImageNum.ToString() & "&IsBillingUpdate=" & isBillingUpdate.ToString(), False)
        End Sub

        Public Shared Function QuoteHadWoodburningSurchargeOnPreviousImage(ByVal quoteId As String, ByVal qqo As QuickQuoteObject, Optional ByRef keyWasPresent As Boolean = False, Optional ByRef keyWasSet As Boolean = False) As Boolean
            Dim hadSurcharge As Boolean = False
            keyWasPresent = False
            keyWasSet = False

            If qqo IsNot Nothing Then
                Dim qqHelper As New QuickQuoteHelperClass
                Dim dictionaryVal As String = qqo.GetDevDictionaryItem("", "HadWoodburningSurchargeOnPreviousImage")
                If String.IsNullOrWhiteSpace(dictionaryVal) = False Then
                    keyWasPresent = True
                    hadSurcharge = qqHelper.BitToBoolean(dictionaryVal)
                Else
                    If qqHelper.IsPositiveIntegerString(qqo.PolicyId) AndAlso qqHelper.IsPositiveIntegerString(qqo.PolicyImageNum) AndAlso CInt(qqo.PolicyImageNum) > 1 Then
                        Using sso As New SQLselectObject(ConfigurationManager.AppSettings("connDiamond"))
                            With sso
                                'note: should be able to select lob_id and state_id directly from Version table also
                                .queryOrStoredProc = "select M.modifier_num from Modifier as M with (nolock)"
                                .queryOrStoredProc &= " where M.policy_id = " & CInt(qqo.PolicyId).ToString
                                .queryOrStoredProc &= " and M.policyimage_num = " & (CInt(qqo.PolicyImageNum) - 1).ToString
                                .queryOrStoredProc &= " and M.modifiertype_id = 17 and M.checkboxselected = 1"

                                Using dr As SqlClient.SqlDataReader = .GetDataReader
                                    If dr IsNot Nothing AndAlso .hasError = False Then
                                        hadSurcharge = dr.HasRows
                                        Set_QuoteHadWoodburningSurchargeOnPreviousImage(qqo, hadSurcharge)
                                        If qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                                            IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SuccessfullySavedEndorsementQuote(qqo)
                                        ElseIf qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then
                                            'can't Save
                                        Else
                                            Dim err As String = Nothing
                                            IFM.VR.Common.QuoteSave.QuoteSaveHelpers.SaveQuote(quoteId, qqo, err)
                                        End If
                                        keyWasSet = True
                                    End If
                                End Using
                            End With
                        End Using
                    End If
                End If
            End If

            Return hadSurcharge
        End Function
        Public Shared Sub Set_QuoteHadWoodburningSurchargeOnPreviousImage(ByVal qqo As QuickQuoteObject, ByVal hadSurcharge As Boolean) 'added 6/17/2020
            If qqo IsNot Nothing Then
                qqo.SetDevDictionaryItem("", "HadWoodburningSurchargeOnPreviousImage", hadSurcharge.ToString)
            End If
        End Sub

        Public Shared Sub SetValueIfDifferent(ByRef ValueToSet As String, ByVal NewValue As String, Optional ByRef HasChange As Boolean = False, Optional DontResetHasChangedFlagToFalse As Boolean = False, Optional AlwaysSetValue As Boolean = False)
            Dim boolVal As Boolean = False
            If ValueToSet.Equals(NewValue, StringComparison.CurrentCultureIgnoreCase) = False Then
                ValueToSet = NewValue
                boolVal = True
            End If
            If boolVal = True Then
                HasChange = True
            Else
                If DontResetHasChangedFlagToFalse = False Then
                    HasChange = False
                Else
                    'leave HasChange alone
                End If
            End If
            If AlwaysSetValue = True Then 'This could allow the function to more easily capture when changes occur even if we always need the value to be set.
                ValueToSet = NewValue
            End If
        End Sub

        'added 5/15/2023
        Public Shared Function VR_NewBusiness_RestrictBefore_Date(Optional ByVal lob As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None, Optional ByRef msg As String = "") As String
            Dim dt As String = QuickQuoteHelperClass.ConfigAppSettingValueAsString_WithOptionalThreeLetterLobAppreviation("VR_NewBusiness_RestrictBefore_Date", lobType:=lob)
            msg = ""

            Dim qqHelper As New QuickQuoteHelperClass
            If qqHelper.IsValidDateString(dt, mustBeGreaterThanDefaultDate:=True) = True Then
                msg = QuickQuoteHelperClass.ConfigAppSettingValueAsString_WithOptionalThreeLetterLobAppreviation("VR_NewBusiness_RestrictBefore_Message", lobType:=lob)
            End If

            Return dt
        End Function
        Public Shared Function VR_NewBusiness_RestrictOnOrAfter_Date(Optional ByVal lob As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None, Optional ByRef msg As String = "") As String
            Dim dt As String = QuickQuoteHelperClass.ConfigAppSettingValueAsString_WithOptionalThreeLetterLobAppreviation("VR_NewBusiness_RestrictOnOrAfter_Date", lobType:=lob)
            msg = ""

            Dim qqHelper As New QuickQuoteHelperClass
            If qqHelper.IsValidDateString(dt, mustBeGreaterThanDefaultDate:=True) = True Then
                msg = QuickQuoteHelperClass.ConfigAppSettingValueAsString_WithOptionalThreeLetterLobAppreviation("VR_NewBusiness_RestrictOnOrAfter_Message", lobType:=lob)
            End If

            Return dt
        End Function
        Public Shared Sub Check_NewBusiness_Min_and_Max_Dates(ByRef minDate As String, ByRef maxDate As String, Optional ByVal lob As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None, Optional ByRef beforeMinDateMsg As String = "", Optional ByRef afterMaxDateMsg As String = "")
            beforeMinDateMsg = ""
            afterMaxDateMsg = ""

            Dim restrictBeforeMsg As String = ""
            Dim restrictBeforeDate As String = VR_NewBusiness_RestrictBefore_Date(lob, msg:=restrictBeforeMsg)
            Dim restrictOnOrAfterMsg As String = ""
            Dim restrictOnOrAfterDate As String = VR_NewBusiness_RestrictOnOrAfter_Date(lob, msg:=restrictOnOrAfterMsg)

            Dim qqHelper As New QuickQuoteHelperClass
            If qqHelper.IsValidDateString(restrictBeforeDate, mustBeGreaterThanDefaultDate:=True) = True Then
                If qqHelper.IsValidDateString(minDate, mustBeGreaterThanDefaultDate:=True) = False OrElse CDate(restrictBeforeDate) > CDate(minDate) Then
                    minDate = CDate(restrictBeforeDate).ToShortDateString
                    beforeMinDateMsg = restrictBeforeMsg
                End If
            End If
            If qqHelper.IsValidDateString(restrictOnOrAfterDate, mustBeGreaterThanDefaultDate:=True) = True Then
                Dim restrictMaxDate As String = DateAdd(DateInterval.Day, -1, CDate(restrictOnOrAfterDate)).ToShortDateString
                If qqHelper.IsValidDateString(maxDate, mustBeGreaterThanDefaultDate:=True) = False OrElse CDate(restrictMaxDate) < CDate(maxDate) Then
                    maxDate = restrictMaxDate
                    afterMaxDateMsg = restrictOnOrAfterMsg
                End If
            End If
        End Sub

        Public Shared Function CommercialDataPrefillQuerystringParam() As String
            Return "CommDataPrefill"
        End Function
        Public Shared Function UseCommercialDataPrefill() As Boolean
            Dim chc As New CommonHelperClass
            Return chc.ConfigurationAppSettingValueAsBoolean("VR_UseCommercialDataPrefill")
        End Function
        Public Shared Function UseKillQuestionForCommercialDataPrefill() As Boolean
            Dim chc As New CommonHelperClass
            Return chc.ConfigurationAppSettingValueAsBoolean("VR_UseKillQuestionForCommercialDataPrefill")
        End Function
        Public Shared Function CommercialDataPrefill_OkayToOverwriteExistingPolicyholderDataFromFirmographics() As Boolean
            Dim chc As New CommonHelperClass
            Return chc.ConfigurationAppSettingValueAsBoolean("VR_CommercialDataPrefill_OkayToOverwriteExistingPolicyholderDataFromFirmographics")
        End Function
        Public Shared Function CommercialDataPrefill_OkayToOverwriteExistingPropertyDataFromFirmographics() As Boolean
            Dim chc As New CommonHelperClass
            Return chc.ConfigurationAppSettingValueAsBoolean("VR_CommercialDataPrefill_OkayToOverwriteExistingPropertyDataFromFirmographics")
        End Function
        Public Shared Function CommercialDataPrefill_OkayToAutoLaunchIfNeeded() As Boolean
            Dim chc As New CommonHelperClass
            Return chc.ConfigurationAppSettingValueAsBoolean("VR_CommercialDataPrefill_OkayToAutoLaunchIfNeeded")
        End Function
        Public Enum CommercialDataPrefill_NoHit_QualificationType
            NoHitOnly = 1
            NoHitOrBlankField = 2
        End Enum
        Public Enum CommercialDataPrefill_NoHit_WipeOutExistingType
            Always = 1
            OnlyWhenExistingMatchesPreviousOrder = 2
            Never = 3
        End Enum
        Public Shared Function CommercialDataPrefill_NoHit_Qualification_ForConfigKeyName(ByVal keyName As String) As CommercialDataPrefill_NoHit_QualificationType
            Dim qt As CommercialDataPrefill_NoHit_QualificationType = CommercialDataPrefill_NoHit_QualificationType.NoHitOrBlankField
            If String.IsNullOrWhiteSpace(keyName) = False Then
                Dim chc As New CommonHelperClass
                Dim strQT As String = chc.ConfigurationAppSettingValueAsString(keyName)
                If String.IsNullOrWhiteSpace(strQT) = False Then
                    Select Case UCase(strQT)
                        Case UCase("NoHitOnly")
                            qt = CommercialDataPrefill_NoHit_QualificationType.NoHitOnly
                        Case UCase("NoHitOrBlankField")
                            qt = CommercialDataPrefill_NoHit_QualificationType.NoHitOrBlankField
                        Case Else
                            If System.Enum.TryParse(Of CommercialDataPrefill_NoHit_QualificationType)(strQT, qt) = False Then
                                qt = CommercialDataPrefill_NoHit_QualificationType.NoHitOrBlankField
                            End If
                    End Select
                End If
            End If
            Return qt
        End Function
        Public Shared Function CommercialDataPrefill_NoHit_WipeOutExisting_ForConfigKeyName(ByVal keyName As String) As CommercialDataPrefill_NoHit_WipeOutExistingType
            Dim wo As CommercialDataPrefill_NoHit_WipeOutExistingType = CommercialDataPrefill_NoHit_WipeOutExistingType.OnlyWhenExistingMatchesPreviousOrder
            If String.IsNullOrWhiteSpace(keyName) = False Then
                Dim chc As New CommonHelperClass
                Dim strWO As String = chc.ConfigurationAppSettingValueAsString(keyName)
                If String.IsNullOrWhiteSpace(strWO) = False Then
                    Select Case UCase(strWO)
                        Case UCase("Always")
                            wo = CommercialDataPrefill_NoHit_WipeOutExistingType.Always
                        Case UCase("OnlyWhenExistingMatchesPreviousOrder")
                            wo = CommercialDataPrefill_NoHit_WipeOutExistingType.OnlyWhenExistingMatchesPreviousOrder
                        Case UCase("Never")
                            wo = CommercialDataPrefill_NoHit_WipeOutExistingType.Never
                        Case Else
                            If System.Enum.TryParse(Of CommercialDataPrefill_NoHit_WipeOutExistingType)(strWO, wo) = False Then
                                wo = CommercialDataPrefill_NoHit_WipeOutExistingType.OnlyWhenExistingMatchesPreviousOrder
                            End If
                    End Select
                End If
            End If
            Return wo
        End Function
        Public Shared Function CommercialDataPrefill_Firmographics_NoHit_Qualification() As CommercialDataPrefill_NoHit_QualificationType
            Return CommercialDataPrefill_NoHit_Qualification_ForConfigKeyName("VR_CommercialDataPrefill_Firmographics_NoHit_Qualification")
        End Function
        Public Shared Function CommercialDataPrefill_Firmographics_NoHit_WipeOutExisting() As CommercialDataPrefill_NoHit_WipeOutExistingType
            Return CommercialDataPrefill_NoHit_WipeOutExisting_ForConfigKeyName("VR_CommercialDataPrefill_Firmographics_NoHit_WipeOutExisting")
        End Function
        Public Shared Function CommercialDataPrefill_Property_NoHit_Qualification() As CommercialDataPrefill_NoHit_QualificationType
            Return CommercialDataPrefill_NoHit_Qualification_ForConfigKeyName("VR_CommercialDataPrefill_Property_NoHit_Qualification")
        End Function
        Public Shared Function CommercialDataPrefill_Property_NoHit_WipeOutExisting() As CommercialDataPrefill_NoHit_WipeOutExistingType
            Return CommercialDataPrefill_NoHit_WipeOutExisting_ForConfigKeyName("VR_CommercialDataPrefill_Property_NoHit_WipeOutExisting")
        End Function
        Public Shared Function CommercialDataPrefill_AllowCallsOnSummaryWorkflow() As Boolean
            Dim chc As New CommonHelperClass
            Return chc.ConfigurationAppSettingValueAsBoolean("VR_CommercialDataPrefill_AllowCallsOnSummaryWorkflow")
        End Function
        Public Shared Function CommercialDataPrefill_AllowCallsOnSummaryWorkflow_QuoteIsEndorsementOrHasExistingReports() As Boolean
            Dim chc As New CommonHelperClass
            Return chc.ConfigurationAppSettingValueAsBoolean("VR_CommercialDataPrefill_AllowCallsOnSummaryWorkflow_QuoteIsEndorsementOrHasExistingReports")
        End Function
        Public Shared Function CommercialDataPrefill_Firmographics_AllowPreloadWhenControlIsInvisible() As Boolean
            Dim chc As New CommonHelperClass
            Return chc.ConfigurationAppSettingValueAsBoolean("VR_CommercialDataPrefill_Firmographics_AllowPreloadWhenControlIsInvisible")
        End Function
        Public Shared Function CommercialDataPrefill_Property_AllowPreloadWhenControlIsInvisible() As Boolean
            Dim chc As New CommonHelperClass
            Return chc.ConfigurationAppSettingValueAsBoolean("VR_CommercialDataPrefill_Property_AllowPreloadWhenControlIsInvisible")
        End Function
        Public Shared Function CommercialDataPrefill_Firmographics_AllowPrefillWhenControlIsInvisible() As Boolean
            Dim chc As New CommonHelperClass
            Return chc.ConfigurationAppSettingValueAsBoolean("VR_CommercialDataPrefill_Firmographics_AllowPrefillWhenControlIsInvisible")
        End Function
        Public Shared Function CommercialDataPrefill_Property_AllowPrefillWhenControlIsInvisible() As Boolean
            Dim chc As New CommonHelperClass
            Return chc.ConfigurationAppSettingValueAsBoolean("VR_CommercialDataPrefill_Property_AllowPrefillWhenControlIsInvisible")
        End Function
        Public Shared Function WorkflowIsOkayForCommercialDataPrefillCalls(ByVal qqo As QuickQuoteObject, Optional ByVal request As HttpRequest = Nothing) As Boolean
            Dim isOkay As Boolean = False

            If CommercialDataPrefill_AllowCallsOnSummaryWorkflow() = True OrElse IsSummaryWorkflow(request:=request) = False Then
                isOkay = True
            Else
                If CommercialDataPrefill_AllowCallsOnSummaryWorkflow_QuoteIsEndorsementOrHasExistingReports() = True Then
                    If qqo IsNot Nothing Then
                        If qqo.QuoteTransactionType = QuickQuoteTransactionType.EndorsementQuote Then
                            'Endorsements won't ever get the new popup screen so we can't just go off of whether or not there are existing orders (see ELSE below)
                            isOkay = True
                        Else
                            Dim ih As New IntegrationHelper
                            If ih.HasAnyCommercialDataPrefillOrders(qqo) = True Then
                                'we have existing prefill orders so we want to make sure we order on new locations if needed or re-order Firm/Prop prefill if anything changes
                                isOkay = True
                            End If
                        End If
                    End If
                End If
            End If

            Return isOkay
        End Function
        Public Shared Function ControlVisibilityIsOkayForCommercialDataPrefillFirmographicsPreload(ByVal visibility As Boolean) As Boolean
            Dim isOkay As Boolean = False

            If visibility = True OrElse CommercialDataPrefill_Firmographics_AllowPreloadWhenControlIsInvisible() = True Then
                isOkay = True
            End If

            Return isOkay
        End Function
        Public Shared Function ControlVisibilityIsOkayForCommercialDataPrefillPropertyPreload(ByVal visibility As Boolean) As Boolean
            Dim isOkay As Boolean = False

            If visibility = True OrElse CommercialDataPrefill_Property_AllowPreloadWhenControlIsInvisible() = True Then
                isOkay = True
            End If

            Return isOkay
        End Function
        Public Shared Function ControlVisibilityIsOkayForCommercialDataPrefillFirmographicsPrefill(ByVal visibility As Boolean) As Boolean
            Dim isOkay As Boolean = False

            If visibility = True OrElse CommercialDataPrefill_Firmographics_AllowPrefillWhenControlIsInvisible() = True Then
                isOkay = True
            End If

            Return isOkay
        End Function
        Public Shared Function ControlVisibilityIsOkayForCommercialDataPrefillPropertyPrefill(ByVal visibility As Boolean) As Boolean
            Dim isOkay As Boolean = False

            If visibility = True OrElse CommercialDataPrefill_Property_AllowPrefillWhenControlIsInvisible() = True Then
                isOkay = True
            End If

            Return isOkay
        End Function
        'note: taken from QuickQuoteHelperClass - one over there needs these updates
        'added 7/20/2015 (originally added to remove hay storage identifier text from building description)
        Public Shared Function RemoveAllInstancesOfStringFromString_Local(ByVal fullString As String, ByVal removeString As String) As String
            Dim str As String = fullString

            Dim keepGoing As Boolean = True
            Do While keepGoing = True
                If String.IsNullOrEmpty(str) = False AndAlso String.IsNullOrEmpty(removeString) = False Then
                    If UCase(str).Contains(UCase(removeString)) = True Then
                        str = RemoveFirstInstanceOfStringFromString_Local(str, removeString)
                    Else
                        keepGoing = False
                        Exit Do
                    End If
                Else
                    keepGoing = False
                    Exit Do
                End If
            Loop

            Return str
        End Function
        Public Shared Function RemoveFirstInstanceOfStringFromString_Local(ByVal fullString As String, ByVal removeString As String) As String 'added 7/20/2015
            Dim str As String = fullString

            If String.IsNullOrEmpty(fullString) = False AndAlso String.IsNullOrEmpty(removeString) = False Then
                If UCase(fullString).Contains(UCase(removeString)) = True Then
                    Dim startIndex As Integer = UCase(fullString).IndexOf(UCase(removeString))
                    If startIndex = 0 Then
                        'at beginning
                        If Len(fullString) > Len(removeString) Then
                            str = Right(fullString, Len(fullString) - Len(removeString))
                        Else
                            'nothing left over
                            str = ""
                        End If
                    Else 'may need to make ElseIf startIndex > 0 as fail safe
                        'after beginning
                        'Dim charsAtBeginning As Integer = Len(removeString) + startIndex - 1
                        'str = Left(fullString, startIndex - 1)
                        'updated 7/18/2023
                        Dim charsAtBeginning As Integer = Len(removeString) + startIndex
                        str = Left(fullString, startIndex)
                        If Len(fullString) > charsAtBeginning Then
                            str &= Right(fullString, Len(fullString) - charsAtBeginning)
                        Else
                            'nothing left over
                        End If
                    End If
                End If
            End If

            Return str
        End Function
        Public Shared Sub SetValueIfNotSet_Local(ByRef valueToSet As String, ByVal valueToUse As String, Optional ByVal onlyValidIfSpecifiedType As TypeToVerify = TypeToVerify.None, Optional ByVal okayToOverwrite As Boolean = False, Optional ByVal neverSetItNotValid As Boolean = False)
            Dim currentlyHasValue As Boolean = False
            Dim currentlyHasValidValue As Boolean = False
            Dim newHasValue As Boolean = False
            Dim newHasValidValue As Boolean = False
            Dim qqHelper As New QuickQuoteHelperClass
            If String.IsNullOrWhiteSpace(valueToSet) = False Then
                currentlyHasValue = True
                If System.Enum.IsDefined(GetType(TypeToVerify), onlyValidIfSpecifiedType) = False OrElse onlyValidIfSpecifiedType = TypeToVerify.None Then
                    'no type to verify
                    currentlyHasValidValue = True
                Else
                    'verify type
                    Select Case onlyValidIfSpecifiedType
                        Case TypeToVerify.NumericType
                            If IsNumeric(valueToSet) = True Then
                                currentlyHasValidValue = True
                            End If
                        Case TypeToVerify.DateType
                            If IsDate(valueToSet) = True Then
                                currentlyHasValidValue = True
                            End If
                        Case TypeToVerify.PositiveIntegerType
                            If qqHelper.IsPositiveIntegerString(valueToSet) = True Then
                                currentlyHasValidValue = True
                            End If
                        Case TypeToVerify.PositiveDecimalType
                            If qqHelper.IsPositiveDecimalString(valueToSet) = True Then
                                currentlyHasValidValue = True
                            End If
                    End Select
                End If
            End If
            If String.IsNullOrWhiteSpace(valueToUse) = False Then
                newHasValue = True
                If System.Enum.IsDefined(GetType(TypeToVerify), onlyValidIfSpecifiedType) = False OrElse onlyValidIfSpecifiedType = TypeToVerify.None Then
                    'no type to verify
                    newHasValidValue = True
                Else
                    'verify type
                    Select Case onlyValidIfSpecifiedType
                        Case TypeToVerify.NumericType
                            If IsNumeric(valueToUse) = True Then
                                newHasValidValue = True
                            End If
                        Case TypeToVerify.DateType
                            If IsDate(valueToUse) = True Then
                                newHasValidValue = True
                            End If
                        Case TypeToVerify.PositiveIntegerType 'needs to be added to qqHelper.SetValueIfNotSet
                            If qqHelper.IsPositiveIntegerString(valueToUse) = True Then
                                newHasValidValue = True
                            End If
                        Case TypeToVerify.PositiveDecimalType 'needs to be added to qqHelper.SetValueIfNotSet
                            If qqHelper.IsPositiveDecimalString(valueToUse) = True Then
                                newHasValidValue = True
                            End If
                    End Select
                End If
            End If

            If newHasValue = True Then
                'new has value
                If currentlyHasValue = False Then
                    'current does not have value
                    If neverSetItNotValid = False OrElse newHasValidValue = True Then
                        valueToSet = valueToUse
                    End If
                Else
                    'both have values
                    If newHasValidValue = True Then
                        'new has valid value
                        If currentlyHasValidValue = False OrElse okayToOverwrite = True Then
                            'current does not have valid value or it's okay to overwrite
                            valueToSet = valueToUse
                        End If
                    Else
                        'new does not have valid value
                        If okayToOverwrite = True AndAlso currentlyHasValidValue = False AndAlso neverSetItNotValid = False Then
                            'it's okay to overwrite and new does not have valid value either
                            valueToSet = valueToUse
                        End If
                    End If
                End If
            Else 'may not need
                'new does not have value
                If currentlyHasValue = False AndAlso okayToOverwrite = True AndAlso neverSetItNotValid = False Then
                    'current does not have value and it's okay to overwrite
                    valueToSet = valueToUse
                End If
            End If
        End Sub
        Public Shared Sub SetValueForCommercialDataPrefill(ByRef valueToSet As String, ByVal valueToUse As String, ByVal isNoHit As Boolean, ByVal wipeOutExisting As CommercialDataPrefill_NoHit_WipeOutExistingType, ByVal noHitQualification As CommercialDataPrefill_NoHit_QualificationType, ByVal hasExistingOrder As Boolean, ByVal existingOrderValue As String, Optional ByVal onlyValidIfSpecifiedType As TypeToVerify = TypeToVerify.None, Optional ByVal useUpperCaseAndTrimWhenSetting As Boolean = False, Optional ByVal okayToOverwrite As Boolean = False, Optional ByVal neverSetItNotValid As Boolean = False)
            Dim callNormalSetValue As Boolean = False

            If wipeOutExisting = CommercialDataPrefill_NoHit_WipeOutExistingType.Never Then
                callNormalSetValue = True
            Else
                'If isNoHit = True OrElse (noHitQualification = CommercialDataPrefill_NoHit_QualificationType.NoHitOrBlankField AndAlso String.IsNullOrWhiteSpace(valueToUse) = True) Then
                If isNoHit = True OrElse (noHitQualification = CommercialDataPrefill_NoHit_QualificationType.NoHitOrBlankField AndAlso IsValidText(valueToUse, onlyValidIfSpecifiedType:=onlyValidIfSpecifiedType) = False) Then
                    'no hit
                    Dim matchType As QuickQuoteHelperClass.TextMatchType = QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing
                    If onlyValidIfSpecifiedType = TypeToVerify.PositiveIntegerType Then
                        matchType = TextMatchType.IntegerOrText_IgnoreCasing
                    ElseIf onlyValidIfSpecifiedType = TypeToVerify.DateType Then
                        matchType = TextMatchType.DateOrText_IgnoreCasing
                    ElseIf onlyValidIfSpecifiedType = TypeToVerify.PositiveDecimalType OrElse onlyValidIfSpecifiedType = TypeToVerify.NumericType Then
                        matchType = TextMatchType.DecimalOrText_IgnoreCasing
                    End If
                    If wipeOutExisting = CommercialDataPrefill_NoHit_WipeOutExistingType.Always Then
                        'could wipe out; would use default value if specific to type desired; need to force okayToOverwrite to True and neverSetItNotValid to False - was thinking I could accomplish what we need via SetValueIfNotSet by manipulating params, but that method will never allow for wiping out a value that's currently in place
                        'valueToSet = OptionalTextInUpperCaseAndTrimmed(valueToUse, useUpperCaseAndTrimWhenSetting)
                        'note: may just be best to set it to emptyString
                        If QuickQuoteHelperClass.isTextMatch(RemoveUnwantedCharsFromStringForComparison(valueToSet), RemoveUnwantedCharsFromStringForComparison(valueToUse), matchType:=matchType) = True OrElse QuickQuoteHelperClass.isTextMatch(RemoveUnwantedCharsFromStringForComparison(valueToSet), "", matchType:=matchType) = True Then
                            'current value "appears" to already be blank or match the new one so leave it alone
                        Else
                            valueToSet = OptionalTextInUpperCaseAndTrimmed(valueToUse, useUpperCaseAndTrimWhenSetting)
                        End If
                    ElseIf wipeOutExisting = CommercialDataPrefill_NoHit_WipeOutExistingType.OnlyWhenExistingMatchesPreviousOrder Then
                        'If hasExistingOrder = True AndAlso QuickQuoteHelperClass.isTextMatch(valueToSet, existingOrderValue, matchType:=QuickQuoteHelperClass.TextMatchType.TextOnly_IgnoreCasing) = True Then
                        'If hasExistingOrder = True AndAlso QuickQuoteHelperClass.isTextMatch(valueToSet, existingOrderValue, matchType:=matchType) = True Then
                        If hasExistingOrder = True AndAlso QuickQuoteHelperClass.isTextMatch(RemoveUnwantedCharsFromStringForComparison(valueToSet), RemoveUnwantedCharsFromStringForComparison(existingOrderValue), matchType:=matchType) = True Then
                            'could wipe out; would use default value if specific to type desired; need to force okayToOverwrite to True and neverSetItNotValid to False - was thinking I could accomplish what we need via SetValueIfNotSet by manipulating params, but that method will never allow for wiping out a value that's currently in place
                            'valueToSet = OptionalTextInUpperCaseAndTrimmed(valueToUse, useUpperCaseAndTrimWhenSetting)
                            'note: may just be best to set it to emptyString
                            If QuickQuoteHelperClass.isTextMatch(RemoveUnwantedCharsFromStringForComparison(valueToSet), RemoveUnwantedCharsFromStringForComparison(valueToUse), matchType:=matchType) = True OrElse QuickQuoteHelperClass.isTextMatch(RemoveUnwantedCharsFromStringForComparison(valueToSet), "", matchType:=matchType) = True Then
                                'current value "appears" to already be blank or match the new one so leave it alone
                            Else
                                valueToSet = OptionalTextInUpperCaseAndTrimmed(valueToUse, useUpperCaseAndTrimWhenSetting)
                            End If
                        Else
                            'no wipe out needed; run normal logic
                            callNormalSetValue = True
                        End If
                    Else 'shouldn't get here since we're already covering for wipeOutExisting Never, Always, and OnlyWhenExistingMatchesPreviousOrder
                        callNormalSetValue = True
                    End If
                Else
                    'no wipe out needed; run normal logic
                    callNormalSetValue = True
                End If
            End If
            If callNormalSetValue = True Then
                'qqHelper.SetValueIfNotSet(.DoingBusinessAsName, WebHelper_Personal.TextInUpperCaseAndTrimmed(firmResponse.returnedData.dba), okayToOverwrite:=isOkayToOverwriteFromFirmographics, neverSetItNotValid:=True)
                SetValueIfNotSet_Local(valueToSet, OptionalTextInUpperCaseAndTrimmed(valueToUse, useUpperCaseAndTrimWhenSetting), onlyValidIfSpecifiedType:=onlyValidIfSpecifiedType, okayToOverwrite:=okayToOverwrite, neverSetItNotValid:=neverSetItNotValid)
            End If
        End Sub
        Public Shared Function OptionalTextInUpperCaseAndTrimmed(ByVal txt As String, ByVal doIt As Boolean) As String
            If doIt = True Then
                Return TextInUpperCaseAndTrimmed(txt)
            Else
                Return txt
            End If
        End Function
        Public Shared Function IsValidText(ByVal txt As String, Optional ByVal onlyValidIfSpecifiedType As TypeToVerify = TypeToVerify.None)
            Dim isIt As Boolean = False

            If String.IsNullOrWhiteSpace(txt) = False Then
                If System.Enum.IsDefined(GetType(TypeToVerify), onlyValidIfSpecifiedType) = False OrElse onlyValidIfSpecifiedType = TypeToVerify.None Then
                    'no type to verify
                    isIt = True
                Else
                    Dim qqHelper As New QuickQuoteHelperClass
                    'verify type
                    Select Case onlyValidIfSpecifiedType
                        Case TypeToVerify.NumericType
                            If IsNumeric(txt) = True Then
                                isIt = True
                            End If
                        Case TypeToVerify.DateType
                            If IsDate(txt) = True Then
                                isIt = True
                            End If
                        Case TypeToVerify.PositiveIntegerType
                            If qqHelper.IsPositiveIntegerString(txt) = True Then
                                isIt = True
                            End If
                        Case TypeToVerify.PositiveDecimalType
                            If qqHelper.IsPositiveDecimalString(txt) = True Then
                                isIt = True
                            End If
                    End Select
                End If
            End If

            Return isIt
        End Function
        Public Shared Function RemoveUnwantedCharsFromStringForComparison(ByVal str As String) As String
            Dim newStr As String = RemovePhoneOrFeinFormattingCharsFromString(str)
            Return newStr
        End Function
        Public Shared Function RemovePhoneOrFeinFormattingCharsFromString(ByVal str As String) As String
            Dim newStr As String = str

            If String.IsNullOrWhiteSpace(newStr) = False AndAlso (newStr.Contains("-") = True OrElse newStr.Contains("(") = True OrElse newStr.Contains(")") = True) Then
                Dim evalStr As String = newStr
                evalStr = Replace(evalStr, "-", "")
                evalStr = Replace(evalStr, "(", "")
                evalStr = Replace(evalStr, ")", "")
                If String.IsNullOrWhiteSpace(evalStr) = False AndAlso IsNumeric(evalStr) = True Then
                    newStr = evalStr
                End If
            End If

            Return newStr
        End Function
        Public Shared Function QuoteIdsOrPolicyImagesFromCommDataPrefillErrorSessionVariable() As String
            Return "QuoteIdsOrPolicyImagesFromCommDataPrefillError"
        End Function
        Public Shared Function QuoteIdsOrPolicyImagesInSessionFromCommDataPrefillError() As List(Of String)
            Dim qIdsOrPIdsAndImgNums As List(Of String) = Nothing

            Dim sv As String = QuoteIdsOrPolicyImagesFromCommDataPrefillErrorSessionVariable()
            If QuickQuoteHelperClass.IsSessionValid() = True AndAlso System.Web.HttpContext.Current.Session(sv) IsNot Nothing Then
                qIdsOrPIdsAndImgNums = System.Web.HttpContext.Current.Session(sv)
            End If

            Return qIdsOrPIdsAndImgNums
        End Function
        Private Sub SetQuoteIdsOrPolicyImagesInSessionFromCommDataPrefillError(ByVal qIdsOrPIdsAndImgNums As List(Of String))
            If QuickQuoteHelperClass.IsSessionValid() = True Then
                Dim sv As String = QuoteIdsOrPolicyImagesFromCommDataPrefillErrorSessionVariable()
                If qIdsOrPIdsAndImgNums IsNot Nothing Then
                    If System.Web.HttpContext.Current.Session(sv) IsNot Nothing Then
                        System.Web.HttpContext.Current.Session(sv) = qIdsOrPIdsAndImgNums
                    Else
                        System.Web.HttpContext.Current.Session.Add(sv, qIdsOrPIdsAndImgNums)
                    End If
                Else
                    If System.Web.HttpContext.Current.Session(sv) IsNot Nothing Then
                        System.Web.HttpContext.Current.Session(sv) = Nothing
                        System.Web.HttpContext.Current.Session.Remove(sv)
                    End If
                End If
            End If
        End Sub
        Public Shared Sub AddToQuoteIdsOrPolicyImagesInSessionFromCommDataPrefillError(ByVal qIdOrPIdAndImgNum As String)
            If QuickQuoteHelperClass.IsSessionValid() = True AndAlso String.IsNullOrWhiteSpace(qIdOrPIdAndImgNum) = False AndAlso IsQuoteIdOrPolicyImageInSessionFromCommDataPrefillError(qIdOrPIdAndImgNum) = False Then
                Dim qIdsOrPIdsAndImgNums As List(Of String) = QuoteIdsOrPolicyImagesInSessionFromCommDataPrefillError()
                QuickQuote.CommonMethods.QuickQuoteHelperClass.AddStringToList(qIdOrPIdAndImgNum, qIdsOrPIdsAndImgNums)
                Dim whp As New WebHelper_Personal
                whp.SetQuoteIdsOrPolicyImagesInSessionFromCommDataPrefillError(qIdsOrPIdsAndImgNums)
            End If
        End Sub
        Public Shared Function IsQuoteIdOrPolicyImageInSessionFromCommDataPrefillError(ByVal qIdOrPIdAndImgNum As String) As Boolean
            Dim isInList As Boolean = False

            Dim qIdsOrPIdsAndImgNums As List(Of String) = QuoteIdsOrPolicyImagesInSessionFromCommDataPrefillError()
            If qIdsOrPIdsAndImgNums IsNot Nothing AndAlso qIdsOrPIdsAndImgNums.Count > 0 AndAlso qIdsOrPIdsAndImgNums.Contains(qIdOrPIdAndImgNum) = True Then
                isInList = True
            End If

            Return isInList
        End Function
        Public Shared Function TextInUpperCaseAndTrimmed(ByVal input As String) As String
            Dim output As String = ""

            If String.IsNullOrWhiteSpace(input) = False Then
                output = UCase(input).Trim()
            End If

            Return output
        End Function
        Public Shared Function TrimmedText(ByVal input As String) As String
            Dim output As String = ""

            If String.IsNullOrWhiteSpace(input) = False Then
                output = input.Trim()
            End If

            Return output
        End Function
        Public Shared Function IsCommercialDataPrefillAvailableForQuote(ByVal qqo As QuickQuoteObject) As Boolean
            Dim isIt As Boolean = False
            Dim GoodLobTypes As List(Of Integer) = GetListofIntegersFromAppSettingsKey("VR_CommercialDataPrefill_LobTypeIdsAllowedKey")
            If qqo IsNot Nothing AndAlso (qqo.QuoteTransactionType = QuickQuoteTransactionType.NewBusinessQuote OrElse
                qqo.QuoteTransactionType = QuickQuoteTransactionType.EndorsementQuote) AndAlso
                GoodLobTypes.Contains(qqo.LobId.TryToGetInt32) AndAlso
                UseCommercialDataPrefill() = True Then
                isIt = True
            End If
            '(qqo.LobType = QuickQuoteLobType.CommercialPackage OrElse qqo.LobType = QuickQuoteLobType.CommercialProperty) AndAlso

            Return isIt
        End Function

        Private Shared Function GetListofIntegersFromAppSettingsKey(key As String) As List(Of Integer)
            Dim c As New CommonHelperClass
            Dim integerList As List(Of Integer) = New List(Of Integer)
            Dim integerString As String = c.ConfigurationAppSettingValueAsString(key)
            If Not String.IsNullOrWhiteSpace(integerString) Then
                integerList = c.ListOfIntegerFromString(integerString, ",")
            End If
            Return integerList
        End Function

        Public Shared Function IsCommercialDataPrefillPopupAvailableForQuote(ByVal qqo As QuickQuoteObject, Optional ByVal expectedIsSummaryWorkflowValue As Nullable(Of Boolean) = Nothing, Optional ByVal request As HttpRequest = Nothing, Optional ByVal qIdOrPIdAndImgNum As String = "") As Boolean
            Dim isIt As Boolean = False

            If qqo IsNot Nothing AndAlso IsCommercialDataPrefillAvailableForQuote(qqo) = True Then
                'isIt = True
                If HasCommDataPrefillQueryString(request:=request) = True Then
                    isIt = True
                ElseIf CommercialDataPrefill_OkayToAutoLaunchIfNeeded() = True AndAlso qqo.QuoteTransactionType = QuickQuoteTransactionType.NewBusinessQuote Then
                    Dim ih As New IntegrationHelper
                    If ih.HasAnyCommercialDataPrefillOrders(qqo) = False Then
                        'no prefill orders found at policy or location level
                        If expectedIsSummaryWorkflowValue Is Nothing OrElse IsSummaryWorkflow(request:=request) = expectedIsSummaryWorkflowValue Then
                            'expected value for quote loaded to Summary screen
                            If IsQuoteIdOrPolicyImageInSessionFromCommDataPrefillError(qIdOrPIdAndImgNum) = False Then
                                'this quote hasn't gotten a prefill error during this session
                                isIt = True
                            End If
                        End If

                    End If
                End If
            End If

            Return isIt
        End Function
        Public Shared Function IsSummaryWorkflow(Optional ByVal request As HttpRequest = Nothing) As Boolean
            Dim isIt As Boolean = False

            Dim wkflowTxt As String = "Workflow"
            If request Is Nothing AndAlso HttpContext.Current IsNot Nothing Then
                request = HttpContext.Current.Request
            End If
            If request IsNot Nothing AndAlso request.QueryString IsNot Nothing AndAlso request.QueryString(wkflowTxt) IsNot Nothing AndAlso String.IsNullOrWhiteSpace(request.QueryString(wkflowTxt).ToString) = False AndAlso UCase(request.QueryString(wkflowTxt).ToString) = "SUMMARY" Then
                isIt = True
            End If

            Return isIt
        End Function
        Public Shared Function HasCommDataPrefillQueryString(Optional ByVal request As HttpRequest = Nothing) As Boolean
            Dim hasIt As Boolean = False

            Dim commDataPrefillQuerystring As String = CommercialDataPrefillQuerystringParam()
            If String.IsNullOrWhiteSpace(commDataPrefillQuerystring) = False Then
                If request Is Nothing AndAlso HttpContext.Current IsNot Nothing Then
                    request = HttpContext.Current.Request
                End If
                If request IsNot Nothing AndAlso request.QueryString IsNot Nothing AndAlso request.QueryString(commDataPrefillQuerystring) IsNot Nothing AndAlso String.IsNullOrWhiteSpace(request.QueryString(commDataPrefillQuerystring).ToString) = False AndAlso UCase(request.QueryString(commDataPrefillQuerystring).ToString) = "YES" Then
                    hasIt = True
                End If
            End If

            Return hasIt
        End Function

        Public Shared Function AdditionalInfoTextForCommercialDataPrefillError(ByVal qqo As QuickQuoteObject, Optional ByRef qqHelper As QuickQuoteHelperClass = Nothing) As String
            Dim addInfo As String = ""
            If qqo IsNot Nothing Then
                If qqHelper Is Nothing Then
                    qqHelper = New QuickQuoteHelperClass
                End If
                With qqo
                    If System.Enum.IsDefined(GetType(QuickQuoteTransactionType), .QuoteTransactionType) = True AndAlso .QuoteTransactionType <> QuickQuoteTransactionType.None Then
                        addInfo = qqHelper.appendText(addInfo, "QuoteTransactionType: " & System.Enum.GetName(GetType(QuickQuoteTransactionType), .QuoteTransactionType), splitter:="; ")
                    End If
                    If System.Enum.IsDefined(GetType(QuickQuoteLobType), .LobType) = True AndAlso .LobType <> QuickQuoteLobType.None Then
                        addInfo = qqHelper.appendText(addInfo, "LobType: " & System.Enum.GetName(GetType(QuickQuoteLobType), .LobType), splitter:="; ")
                    End If
                    If String.IsNullOrWhiteSpace(.PolicyNumber) = False Then
                        addInfo = qqHelper.appendText(addInfo, If(Left(UCase(.PolicyNumber), 1) = "Q", "Quote", "Policy") & " Number: " & .PolicyNumber, splitter:="; ")
                    End If
                    If qqHelper.IsPositiveIntegerString(.Database_QuoteId) = True Then
                        addInfo = qqHelper.appendText(addInfo, "Quote Id: " & .Database_QuoteId, "; ")
                    End If
                    If qqHelper.IsPositiveIntegerString(.PolicyId) = True Then
                        addInfo = qqHelper.appendText(addInfo, "Policy Id: " & .PolicyId, "; ")
                    End If
                    If qqHelper.IsPositiveIntegerString(.PolicyImageNum) = True Then
                        addInfo = qqHelper.appendText(addInfo, "Policy Image Num: " & .PolicyImageNum, "; ")
                    End If
                End With
            End If
            Return addInfo
        End Function

    End Class

    Public Class MessageBoxVRPers
        Private Sub New(ByVal msg As String, ByVal response As HttpResponse, ByVal scriptManager As ScriptManager, ByVal control As Control)
            Dim script As String = String.Format("<script>alert('{0}');</script>", msg.Replace("'", ""))

            ScriptManager.RegisterStartupScript(control, control.GetType(), Guid.NewGuid().ToString(), script, False)
        End Sub

        Public Shared Sub Show(ByVal msg As String, ByVal response As HttpResponse, ByVal scriptManager As ScriptManager, ByVal control As Control)
            Dim msgBox As New MessageBoxVRPers(msg, response, scriptManager, control)
        End Sub
    End Class




    'Module Int32Extensions

    '    <Extension()>
    '    Public Function IncrementByOne(ByRef aint As Int32) As Int32
    '        aint += 1
    '        Return aint
    '    End Function

    'End Module

    ''' <summary>
    '''  DO NOT USE THIS INTERFACE
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IVRUI_P
        'DO NOT USE THIS INTERFACE
        ReadOnly Property Quote As QuickQuote.CommonObjects.QuickQuoteObject
        ReadOnly Property QuoteId As String
        Property ValidationHelper As ControlValidationHelper
        Sub LoadStaticData()
        Sub Populate()
        Function Save() As Boolean
        Sub ValidateForm()
    End Interface

End Namespace