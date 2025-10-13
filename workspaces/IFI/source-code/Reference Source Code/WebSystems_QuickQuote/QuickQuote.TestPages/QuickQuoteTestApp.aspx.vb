Imports QuickQuote.CommonMethods
Imports QuickQuote.CommonObjects
Imports System.Data
Partial Class QuickQuoteTestApp
    Inherits System.Web.UI.Page

    Dim qqXml As New QuickQuoteXML
    Dim qqHelper As New QuickQuoteHelperClass

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        Me.lblResults.Text = ""
        Me.UpdateSection.Visible = False
        Me.RvWatercraftOperatorAssignSection.Visible = False
        Me.cbAssignRvWatercraftOperator.Checked = False
        Me.RvWatercraftOperatorUnAssignSection.Visible = False
        Me.cbUnAssignRvWatercraftOperator.Checked = False
        Me.DiamondSaveOptionsSection.Visible = False
        Me.cbSaveRateWithSeparateMethods.Checked = False
        Me.lblSaveRateResults.Text = ""
        'Me.SaveRateSection.Visible = False
        DisposeOfQuickQuoteObjectInViewstate()

        If String.IsNullOrWhiteSpace(Me.txtQuoteIdOrPolicyIdAndImageNum.Text) = True Then
            ShowError("please enter a quoteId and policyId/imageNum")
            SetFocus(Me.txtQuoteIdOrPolicyIdAndImageNum)
        ElseIf qqHelper.IsPositiveIntegerString(Me.txtQuoteIdOrPolicyIdAndImageNum.Text.Replace("|", "")) = False Then
            ShowError("invalid input format")
            SetFocus(Me.txtQuoteIdOrPolicyIdAndImageNum)
        Else
            Dim qqo As QuickQuoteObject = Nothing
            Dim errorMessage As String = ""

            If Me.txtQuoteIdOrPolicyIdAndImageNum.Text.Contains("|") = True Then
                'Endorsement search
                Dim policyId As Integer = 0
                Dim imageNum As Integer = 0
                Dim intList As List(Of Integer) = QuickQuoteHelperClass.ListOfIntegerFromString(Me.txtQuoteIdOrPolicyIdAndImageNum.Text, delimiter:="|", positiveOnly:=True)
                If intList IsNot Nothing AndAlso intList.Count > 0 Then
                    policyId = intList(0)
                    If intList.Count > 1 Then
                        imageNum = intList(1)
                    End If
                End If
                If policyId > 0 AndAlso imageNum > 0 Then
                    qqo = qqXml.QuickQuoteEndorsementForPolicyIdAndImageNum(policyId, imageNum, errorMessage:=errorMessage)
                Else
                    errorMessage = "invalid policyId and/or imageNum for Endorsement lookup"
                End If
            Else
                'NewBusiness search
                qqXml.GetQuoteForSaveType(CInt(Me.txtQuoteIdOrPolicyIdAndImageNum.Text), QuickQuoteXML.QuickQuoteSaveType.Quote, qqo, errorMessage)
            End If

            If qqo IsNot Nothing Then
                If ViewState.Item("QuickQuoteObject") IsNot Nothing Then
                    ViewState.Item("QuickQuoteObject") = qqo
                Else
                    ViewState.Add("QuickQuoteObject", qqo)
                End If
                Me.lblResults.Text = qqo.ToString
                If String.IsNullOrWhiteSpace(Me.lblResults.Text) = False AndAlso Me.lblResults.Text.Contains(vbCrLf) = True Then
                    Me.lblResults.Text = Me.lblResults.Text.Replace(vbCrLf, "<br />")
                End If

                'for RvWatercraft Operator Assignment testing
                'If qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 AndAlso qqo.Locations(0) IsNot Nothing AndAlso qqo.Locations(0).RvWatercrafts IsNot Nothing AndAlso qqo.Locations(0).RvWatercrafts.Count > 0 AndAlso qqo.Locations(0).RvWatercrafts(0) IsNot Nothing Then
                '    If qqo.Operators IsNot Nothing AndAlso qqo.Operators.Count > 0 Then
                '        If qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums Is Nothing OrElse qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Count = 0 OrElse qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Contains(qqo.Operators.Count) = False Then
                '            'give option to assign last operator
                '            'QuickQuoteHelperClass.AddIntegerToIntegerList(qqo.Operators.Count, qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums)
                '            Me.RvWatercraftOperatorAssignSection.Visible = True
                '        Else
                '            'give option to un-assign last operator
                '            'qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Remove(qqo.Operators.Count)
                '            Me.RvWatercraftOperatorUnAssignSection.Visible = True
                '        End If
                '    End If
                'End If
                CheckRvWatercraftOperatorAssignment(CheckType.ForLoad, qqo)

                'If Me.RvWatercraftOperatorAssignSection.Visible = True OrElse Me.RvWatercraftOperatorUnAssignSection.Visible = True Then
                '    Me.UpdateSection.Visible = True
                'End If
            Else
                If String.IsNullOrWhiteSpace(errorMessage) = False Then
                    ShowError(errorMessage)
                Else
                    ShowError("problem loading QuickQuoteObject")
                End If
            End If
        End If
    End Sub

    Private Sub btnSaveRate_Click(sender As Object, e As EventArgs) Handles btnSaveRate.Click
        System.Threading.Thread.Sleep(500) 'delay 1/2 second

        Me.lblSaveRateResults.Text = ""

        Dim validationErrorMessage As String = ""
        If Me.UpdateSection.Visible = False Then
            validationErrorMessage = "nothing available for update"
        Else
            Dim somethingChanged As Boolean = False

            'for RvWatercraft Operator Assignment testing
            If Me.RvWatercraftOperatorAssignSection.Visible = True AndAlso Me.cbAssignRvWatercraftOperator.Checked = True Then
                somethingChanged = True
            End If
            If Me.RvWatercraftOperatorUnAssignSection.Visible = True AndAlso Me.cbUnAssignRvWatercraftOperator.Checked = True Then
                somethingChanged = True
            End If

            If somethingChanged = False Then
                validationErrorMessage = "it doesn't appear that anything has been changed"
            End If
        End If

        If String.IsNullOrWhiteSpace(validationErrorMessage) = True Then
            Dim qqo As QuickQuoteObject = Nothing
            If ViewState.Item("QuickQuoteObject") IsNot Nothing Then
                qqo = DirectCast(ViewState.Item("QuickQuoteObject"), QuickQuoteObject)
            End If
            If qqo IsNot Nothing Then

                Dim changedQQO As Boolean = False

                'for RvWatercraft Operator Assignment testing
                'If Me.cbAssignRvWatercraftOperator.Checked = True OrElse Me.cbUnAssignRvWatercraftOperator.Checked = True Then
                '    If qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 AndAlso qqo.Locations(0) IsNot Nothing AndAlso qqo.Locations(0).RvWatercrafts IsNot Nothing AndAlso qqo.Locations(0).RvWatercrafts.Count > 0 AndAlso qqo.Locations(0).RvWatercrafts(0) IsNot Nothing Then
                '        If qqo.Operators IsNot Nothing AndAlso qqo.Operators.Count > 0 Then
                '            If qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums Is Nothing OrElse qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Count = 0 OrElse qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Contains(qqo.Operators.Count) = False Then
                '                'give option to assign last operator
                '                If Me.cbAssignRvWatercraftOperator.Checked = True Then
                '                    QuickQuoteHelperClass.AddIntegerToIntegerList(qqo.Operators.Count, qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums)
                '                    changedQQO = True
                '                End If
                '            Else
                '                'give option to un-assign last operator
                '                If Me.cbUnAssignRvWatercraftOperator.Checked = True Then
                '                    qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Remove(qqo.Operators.Count)
                '                    changedQQO = True
                '                End If
                '            End If

                '        End If
                '    End If
                'End If
                CheckRvWatercraftOperatorAssignment(CheckType.ForSave, qqo, changedQuickQuoteObject:=changedQQO)

                If changedQQO = False Then
                    validationErrorMessage = "problem making changes to the QuickQuoteObject"
                End If

                If String.IsNullOrWhiteSpace(validationErrorMessage) = True Then
                    Dim msgToShow As String = ""
                    Dim qqoResults As QuickQuoteObject = Nothing
                    Dim reload As Boolean = False
                    If qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        'Endorsement Save/Rate
                        Dim successfullySavedAndRated As Boolean = False
                        Dim successfullySaved As Boolean = False
                        Dim successfullyRated As Boolean = False
                        Dim replacedObjectPassedIn As Boolean = False
                        Dim saveRateErrorMessage As String = ""
                        Dim qqeResults As QuickQuoteObject = Nothing

                        If Me.cbSaveRateWithSeparateMethods.Checked = True Then
                            'this method calls Diamond's Save service 1st and then the Rate service
                            successfullySavedAndRated = qqXml.SuccessfullySavedAndRatedQuickQuoteEndorsementInDiamondWithSeparateServiceCalls_ReplaceObjectPassedIn(qqo, qqeResults:=qqeResults, saveSuccessful:=successfullySaved, rateSuccessful:=successfullyRated, replacedObjectPassedIn:=replacedObjectPassedIn, errorMessage:=saveRateErrorMessage)
                        Else
                            'this method calls Diamond's SaveRate service
                            successfullySavedAndRated = qqXml.SuccessfullySavedAndRatedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn(qqo, qqeResults:=qqeResults, saveSuccessful:=successfullySaved, rateSuccessful:=successfullyRated, replacedObjectPassedIn:=replacedObjectPassedIn, errorMessage:=saveRateErrorMessage)
                        End If

                        If replacedObjectPassedIn = True Then
                            ViewState.Item("QuickQuoteObject") = qqo
                        End If
                        'Dim msgToShow As String = ""
                        If successfullySavedAndRated = True Then
                            Dim successMessage As String = "your endorsement changes have been saved/rated in Diamond"
                            If qqeResults IsNot Nothing AndAlso qqHelper.IsPositiveDecimalString(qqeResults.FullTermPremium) = True Then
                                successMessage &= "; Full Term Premium = " & qqeResults.FullTermPremium & "; Change in Full Term Premium = " & qqeResults.ChangeInFullTermPremium & "; Written Premium = " & qqeResults.WrittenPremium & "; Change in Written Premium = " & qqeResults.ChangeInWrittenPremium
                            End If
                            msgToShow = successMessage
                        Else
                            If String.IsNullOrWhiteSpace(saveRateErrorMessage) = False Then
                                msgToShow = saveRateErrorMessage
                            Else
                                If successfullySaved = True AndAlso successfullyRated = False Then
                                    msgToShow = "your endorsement changes were successfully saved, but rating failed"
                                Else
                                    msgToShow = "problem saving/rating your endorsement changes in Diamond"
                                End If
                            End If
                        End If

                        If successfullySavedAndRated = True OrElse successfullySaved = True Then
                            'msgToShow &= "; the updated image will now be reloaded"
                            Me.txtQuoteIdOrPolicyIdAndImageNum.Text = qqo.PolicyId & "|" & qqo.PolicyImageNum
                            'Me.btnLoad_Click(sender, e)
                            reload = True
                        End If

                        If qqeResults IsNot Nothing Then
                            Dim valItems As String = QuickQuoteValidationItemsAsString(qqeResults)
                            msgToShow = qqHelper.appendText(msgToShow, valItems, "<br /><br />")
                        End If
                        'ShowError(msgToShow)
                        qqoResults = qqeResults
                    Else
                        'NewBusiness Save/Rate
                        Dim strQQ As String = ""
                        Dim ratedQQ As QuickQuoteObject = Nothing
                        Dim strRatedQQ As String = ""
                        Dim errorMessage As String = ""

                        qqXml.RateQuoteAndSave(QuickQuoteXML.QuickQuoteSaveType.Quote, qqo, strQQ, ratedQQ, strRatedQQ, qqo.Database_QuoteId, errorMessage) 'debug method w/ byref params for rated QuickQuoteObject and xml strings for the request and response

                        'Dim msgToShow As String = ""
                        If ratedQQ IsNot Nothing AndAlso ratedQQ.Success = True Then
                            Dim successMessage As String = "your new business changes have been saved/rated in Diamond"
                            If qqHelper.IsPositiveDecimalString(ratedQQ.FullTermPremium) = True Then
                                successMessage &= "; Full Term Premium = " & ratedQQ.FullTermPremium & "; Written Premium = " & ratedQQ.WrittenPremium
                            End If
                            If String.IsNullOrWhiteSpace(errorMessage) = False Then
                                successMessage = qqHelper.appendText(successMessage, errorMessage, "<br /><br />Error Message: ")
                            End If
                            msgToShow = successMessage
                        Else
                            If String.IsNullOrWhiteSpace(errorMessage) = False Then
                                msgToShow = errorMessage
                            Else
                                If ratedQQ IsNot Nothing AndAlso qqHelper.IsPositiveIntegerString(ratedQQ.PolicyId) = True Then
                                    msgToShow = "your new business changes appear to have been successfully saved, but rating failed"
                                Else
                                    msgToShow = "problem saving/rating your new business changes in Diamond"
                                End If
                            End If
                        End If

                        If ratedQQ IsNot Nothing AndAlso qqHelper.IsPositiveIntegerString(ratedQQ.PolicyId) = True Then
                            'msgToShow &= "; the updated image will now be reloaded"
                            Me.txtQuoteIdOrPolicyIdAndImageNum.Text = qqo.Database_QuoteId
                            'Me.btnLoad_Click(sender, e)
                            reload = True
                        End If

                        If ratedQQ IsNot Nothing Then
                            Dim valItems As String = QuickQuoteValidationItemsAsString(ratedQQ)
                            msgToShow = qqHelper.appendText(msgToShow, valItems, "<br /><br />")
                        End If
                        'ShowError(msgToShow)
                        qqoResults = ratedQQ
                    End If

                    'for RvWatercraft Operator Assignment testing
                    Dim rvWatercraftOperatorMsg As String = ""
                    CheckRvWatercraftOperatorAssignment(CheckType.PostSave, qqoResults, rvWatercraftOperatorMsg:=rvWatercraftOperatorMsg)
                    If String.IsNullOrWhiteSpace(rvWatercraftOperatorMsg) = False Then
                        msgToShow = qqHelper.appendText(msgToShow, rvWatercraftOperatorMsg, "<br /><br />")
                    End If

                    If reload = True Then
                        msgToShow = qqHelper.appendText(msgToShow, "the updated image will now be reloaded", "<br /><br />")
                        Me.btnLoad_Click(sender, e)
                    End If

                    ShowError(msgToShow)
                Else
                    ShowError(validationErrorMessage)
                End If
            Else
                ShowError("unable to load QuickQuoteObject from ViewState")
            End If
        Else
            ShowError(validationErrorMessage)
        End If
    End Sub

    Private Sub QuickQuoteTestApp_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Page.IsPostBack = False Then
            Me.btnLoad.Attributes.Add("onclick", "btnSubmit_Click(this, 'Loading...');") 'for disable button and server-side logic
            Me.btnSaveRate.Attributes.Add("onclick", "btnSubmit_Click(this, 'Saving/Rating...');") 'for disable button and server-side logic
            SetFocus(Me.txtQuoteIdOrPolicyIdAndImageNum)
        End If
    End Sub

    Private Sub ShowError(ByVal message As String, Optional ByVal redirect As Boolean = False, Optional ByVal redirectPage As String = "")
        message = Replace(message, "\", "\\")
        message = Replace(message, "<br>", "\n")
        message = Replace(message, "<br />", "\n")
        message = Replace(message, vbCrLf, "\n")

        Dim strScript As String = "<script language=JavaScript>"
        strScript &= "alert(""" & message & """);"
        If redirect = True Then
            If redirectPage = "" Then
                redirectPage = "MyVelociRater.aspx" 'use config key if available
            End If
            strScript &= " window.location.href='" & redirectPage & "';"
        End If
        strScript &= "</script>"

        Page.RegisterStartupScript("clientScript", strScript)

    End Sub

    Private Function QuickQuoteValidationItemsAsString(ByVal qqo As QuickQuoteObject) As String
        Dim strValItems As String = ""

        If qqo IsNot Nothing AndAlso qqo.ValidationItems IsNot Nothing AndAlso qqo.ValidationItems.Count > 0 Then
            strValItems = "Validation Items:"
            Dim valItemCounter As Integer = 0 'added 11/10/2016
            For Each vi As QuickQuoteValidationItem In qqo.ValidationItems
                valItemCounter += 1 'added 11/10/2016
                'strValItems &= "<br />" & vi.ValidationSeverityType & " - " & vi.Message
                'updated 11/10/2016 since it was just showing Integer for Enum; could also use GetName to get text for Enum
                Dim strValItem As String = ""
                Dim valSeverityType As String = qqHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteValidationItem, QuickQuoteHelperClass.QuickQuotePropertyName.ValidationSeverityTypeId, vi.ValidationSeverityTypeId.ToString)
                If String.IsNullOrWhiteSpace(valSeverityType) = False Then
                    strValItem = valSeverityType
                Else
                    strValItem = "# " & valItemCounter.ToString
                End If
                strValItem &= " - " & vi.Message
                strValItems &= "<br />" & strValItem
            Next
        End If

        Return strValItems
    End Function

    'Private Sub CheckRvWatercraftOperators(ByRef qqo As QuickQuoteObject, Optional ByVal checkForAssigned As Boolean = True, Optional ByVal checkForUnassigned As Boolean = True, Optional ByRef hasRvWatercraftAndOperator As Boolean = False, Optional ByRef hasLastOperatorAssignedToRvWatercraft As Boolean = False)
    '    'for RvWatercraft Operator Assignment testing
    '    hasRvWatercraftAndOperator = False
    '    hasLastOperatorAssignedToRvWatercraft = False
    '    'If Me.cbAssignRvWatercraftOperator.Checked = True OrElse Me.cbUnAssignRvWatercraftOperator.Checked = True Then
    '    If checkForAssigned = True OrElse checkForUnassigned = True Then
    '        If qqo IsNot Nothing AndAlso qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 AndAlso qqo.Locations(0) IsNot Nothing AndAlso qqo.Locations(0).RvWatercrafts IsNot Nothing AndAlso qqo.Locations(0).RvWatercrafts.Count > 0 AndAlso qqo.Locations(0).RvWatercrafts(0) IsNot Nothing Then
    '            If qqo.Operators IsNot Nothing AndAlso qqo.Operators.Count > 0 Then
    '                If qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums Is Nothing OrElse qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Count = 0 OrElse qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Contains(qqo.Operators.Count) = False Then
    '                    'give option to assign last operator
    '                    'If Me.cbAssignRvWatercraftOperator.Checked = True Then
    '                    If checkForAssigned = True Then
    '                        QuickQuoteHelperClass.AddIntegerToIntegerList(qqo.Operators.Count, qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums)
    '                        changedQQO = True
    '                    End If
    '                Else
    '                    'give option to un-assign last operator
    '                    'If Me.cbUnAssignRvWatercraftOperator.Checked = True Then
    '                    If checkForUnassigned = True Then
    '                        qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Remove(qqo.Operators.Count)
    '                        changedQQO = True
    '                    End If
    '                End If

    '            End If
    '        End If
    '    End If
    'End Sub
    Private Enum CheckType
        None = 0
        ForLoad = 1
        ForSave = 2
        PostSave = 3
    End Enum
    Private Sub CheckRvWatercraftOperatorAssignment(ByVal typeToCheck As CheckType, ByRef qqo As QuickQuoteObject, Optional ByRef changedQuickQuoteObject As Boolean = False, Optional ByRef rvWatercraftOperatorMsg As String = "")
        'for RvWatercraft Operator Assignment testing
        changedQuickQuoteObject = False
        rvWatercraftOperatorMsg = ""
        If System.Enum.IsDefined(GetType(CheckType), typeToCheck) = True AndAlso typeToCheck <> CheckType.None Then
            If typeToCheck = CheckType.ForLoad OrElse ((typeToCheck = CheckType.ForSave OrElse typeToCheck = CheckType.PostSave) AndAlso (Me.cbAssignRvWatercraftOperator.Checked = True OrElse Me.cbUnAssignRvWatercraftOperator.Checked = True)) Then
                If qqo IsNot Nothing AndAlso qqo.Locations IsNot Nothing AndAlso qqo.Locations.Count > 0 AndAlso qqo.Locations(0) IsNot Nothing AndAlso qqo.Locations(0).RvWatercrafts IsNot Nothing AndAlso qqo.Locations(0).RvWatercrafts.Count > 0 AndAlso qqo.Locations(0).RvWatercrafts(0) IsNot Nothing Then
                    If qqo.Operators IsNot Nothing AndAlso qqo.Operators.Count > 0 Then
                        If qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums Is Nothing OrElse qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Count = 0 OrElse qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Contains(qqo.Operators.Count) = False Then
                            'not assigned
                            Select Case typeToCheck
                                Case CheckType.ForLoad
                                    'give option to assign last operator
                                    'Me.UpdateSection.Visible = True
                                    'Me.RvWatercraftOperatorAssignSection.Visible = True
                                    MakeIndividualUpdateSectionVisible(Me.RvWatercraftOperatorAssignSection, qqo:=qqo)
                                Case CheckType.ForSave
                                    If Me.cbAssignRvWatercraftOperator.Checked = True Then
                                        QuickQuoteHelperClass.AddIntegerToIntegerList(qqo.Operators.Count, qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums)
                                        changedQuickQuoteObject = True
                                    End If
                                Case CheckType.PostSave
                                    If Me.cbAssignRvWatercraftOperator.Checked = True Then
                                        'failed to assign
                                        rvWatercraftOperatorMsg = "failed to assign RvWatercraft Operator"
                                    Else
                                        'successfully un-assigned
                                        rvWatercraftOperatorMsg = "successfully un-assigned RvWatercraft Operator"
                                    End If
                            End Select
                        Else
                            'assigned
                            Select Case typeToCheck
                                Case CheckType.ForLoad
                                    'give option to un-assign last operator
                                    'Me.UpdateSection.Visible = True
                                    'Me.RvWatercraftOperatorUnAssignSection.Visible = True
                                    MakeIndividualUpdateSectionVisible(Me.RvWatercraftOperatorUnAssignSection, qqo:=qqo)
                                Case CheckType.ForSave
                                    If Me.cbUnAssignRvWatercraftOperator.Checked = True Then
                                        qqo.Locations(0).RvWatercrafts(0).AssignedOperatorNums.Remove(qqo.Operators.Count)
                                        changedQuickQuoteObject = True
                                    End If
                                Case CheckType.PostSave
                                    If Me.cbUnAssignRvWatercraftOperator.Checked = True Then
                                        'failed to un-assign
                                        rvWatercraftOperatorMsg = "failed to un-assign RvWatercraft Operator"
                                    Else
                                        'successfully assigned
                                        rvWatercraftOperatorMsg = "successfully assigned RvWatercraft Operator"
                                    End If
                            End Select
                        End If

                    End If
                End If
            End If
        End If
    End Sub

    Private Sub DisposeOfQuickQuoteObjectInViewstate() 'overkill
        Dim qqo As QuickQuoteObject = Nothing
        If ViewState.Item("QuickQuoteObject") IsNot Nothing Then
            qqo = DirectCast(ViewState.Item("QuickQuoteObject"), QuickQuoteObject)
            If qqo IsNot Nothing Then
                qqo.Dispose()
                qqo = Nothing
            End If
            ViewState.Item("QuickQuoteObject") = qqo
            ViewState.Item("QuickQuoteObject") = Nothing
        End If
    End Sub

    Private Sub MakeIndividualUpdateSectionVisible(ByRef ctrl As HtmlGenericControl, Optional ByVal qqo As QuickQuoteObject = Nothing)
        If ctrl IsNot Nothing Then
            Me.UpdateSection.Visible = True
            ctrl.Visible = True
            If qqo IsNot Nothing AndAlso qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                Me.DiamondSaveOptionsSection.Visible = True
            End If
        End If
    End Sub
End Class
