Imports System.Web.SessionState
Imports QuickQuote.CommonObjects
Imports QuickQuote.CommonMethods
Imports System.Web
Imports IFM.VR.Common.Helpers.MultiState

Namespace IFM.VR.Common.QuoteSave

    Public Class QuoteSaveHelpers
        Const contextQuotePrefix As String = "pl_{0}"

        'added 2/14/2019
        Const contextEndorsementPrefix As String = "pl_e_{0}_{1}"
        Const contextReadOnlyImagePrefix As String = "pl_ro_{0}_{1}"

        Private Shared Function GetContextQuoteVariableName(ByVal quoteid As String, saveType As QuickQuoteXML.QuickQuoteSaveType) As String
            If saveType = QuickQuoteXML.QuickQuoteSaveType.Quote Then
                Return String.Format(contextQuotePrefix, quoteid.Trim())
            Else
                Return String.Format("app_" + contextQuotePrefix, quoteid.Trim())
            End If

        End Function

        'added 2/14/2019
        Private Shared Function GetContextEndorsementVariableName(ByVal policyId As Integer, ByVal policyImageNum As Integer, Optional ByVal saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote) As String
            If saveTypeView = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                Return String.Format("app_" + contextEndorsementPrefix, policyId.ToString, policyImageNum.ToString)
            Else
                Return String.Format(contextEndorsementPrefix, policyId.ToString, policyImageNum.ToString)
            End If

        End Function
        Private Shared Function GetContextReadOnlyImageVariableName(ByVal policyId As Integer, ByVal policyImageNum As Integer, Optional ByVal saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote) As String
            If saveTypeView = QuickQuoteXML.QuickQuoteSaveType.AppGap Then
                Return String.Format("app_" + contextReadOnlyImagePrefix, policyId.ToString, policyImageNum.ToString)
            Else
                Return String.Format(contextReadOnlyImagePrefix, policyId.ToString, policyImageNum.ToString)
            End If

        End Function

        Public Shared Function GetQuoteById(ByVal expectedLobType As QuickQuoteObject.QuickQuoteLobType, ByVal quoteid As String, ByRef errCreateQSO As String, Optional ignoreLOBType As Boolean = False, Optional saveType As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote) As QuickQuoteObject
#If DEBUG Then
            Dim startTime As DateTime = DateTime.Now
#End If
            Dim context As HttpContext = System.Web.HttpContext.Current
            Try
                If context.Items(GetContextQuoteVariableName(quoteid, saveType)) Is Nothing Then
                    Dim newQSO As QuickQuoteObject = Nothing
                    Dim QQxml As New QuickQuoteXML
                    QQxml.GetQuoteForSaveType(quoteid, saveType, newQSO, errCreateQSO)

                    'make sure is isn't nothing and do a lob check
                    If String.IsNullOrWhiteSpace(errCreateQSO) = False OrElse newQSO Is Nothing OrElse (newQSO.LobType <> expectedLobType And ignoreLOBType = False) Then
                        If String.IsNullOrWhiteSpace(errCreateQSO) Then
                            errCreateQSO = "The quote loaded is not the correct LOB for this page."
                        End If
                        context.Items(GetContextQuoteVariableName(quoteid, saveType)) = Nothing
                        Return Nothing
                    End If
                    'put in session
                    context.Items(GetContextQuoteVariableName(quoteid, saveType)) = newQSO
                Else
                    'get an existing from session
                    If DirectCast(context.Items(GetContextQuoteVariableName(quoteid, saveType)), QuickQuoteObject).LobType <> expectedLobType And ignoreLOBType = False Then
                        context.Items(GetContextQuoteVariableName(quoteid, saveType)) = Nothing
                        errCreateQSO = "The quote loaded is not the correct LOB for this page."
                    End If
                End If
            Catch ex As Exception
                errCreateQSO = String.Format("quoteid {0} failed to load. {1} is loading LOB type.", quoteid, expectedLobType.ToString())
                Return Nothing
            End Try

            Dim topQuote = DirectCast(context.Items(GetContextQuoteVariableName(quoteid, saveType)), QuickQuoteObject)
            If topQuote.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If
#If DEBUG Then
            Dim timeToComplete As Double = (startTime - DateTime.Now).TotalMilliseconds
            If Math.Abs(timeToComplete) > 20 Then
                Debug.WriteLine($"Loaded the quote in {timeToComplete}ms.")
            End If
#End If
            Return topQuote
        End Function

        ''' <summary>
        ''' Clears the specified quote image from session.
        ''' </summary>
        ''' <param name="quoteID"></param>
        ''' <param name="saveType"></param>
        ''' <remarks></remarks>
        Public Shared Sub ForceQuoteReloadById(quoteID As String, Optional saveType As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote)
            Try
                Dim context As HttpContext = System.Web.HttpContext.Current
                If context IsNot Nothing AndAlso context.Items.Keys IsNot Nothing Then
                    If context.Items(GetContextQuoteVariableName(quoteID, saveType)) IsNot Nothing Then
                        context.Items.Remove(GetContextQuoteVariableName(quoteID, saveType))
                    End If
                End If
            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
            End Try

        End Sub

        'added 2/14/2019
        Public Shared Sub ForceEndorsementReloadByPolicyIdAndImageNum(ByVal policyId As Integer, ByVal policyImageNum As Integer, Optional saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote)
            Try
                Dim context As HttpContext = System.Web.HttpContext.Current
                If context IsNot Nothing AndAlso context.Items.Keys IsNot Nothing Then
                    If context.Items(GetContextEndorsementVariableName(policyId, policyImageNum, saveTypeView:=saveTypeView)) IsNot Nothing Then
                        context.Items.Remove(GetContextEndorsementVariableName(policyId, policyImageNum, saveTypeView:=saveTypeView))
                    End If
                End If
            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
            End Try

        End Sub
        Public Shared Sub ForceReadOnlyImageReloadByPolicyIdAndImageNum(ByVal policyId As Integer, ByVal policyImageNum As Integer, Optional saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote)
            Try
                Dim context As HttpContext = System.Web.HttpContext.Current
                If context IsNot Nothing AndAlso context.Items.Keys IsNot Nothing Then
                    If context.Items(GetContextReadOnlyImageVariableName(policyId, policyImageNum, saveTypeView:=saveTypeView)) IsNot Nothing Then
                        context.Items.Remove(GetContextReadOnlyImageVariableName(policyId, policyImageNum, saveTypeView:=saveTypeView))
                    End If
                End If
            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
            End Try

        End Sub

        Public Shared Function GetQuoteById_NOSESSION(ByVal quoteid As String) As QuickQuoteObject
#If DEBUG Then
            Dim startTime As DateTime = DateTime.Now
#End If
            Dim newQSO As QuickQuoteObject = Nothing
            Try
                Dim errorMsg As String = ""
                Dim QQxml As New QuickQuoteXML
                QQxml.GetQuoteForSaveType(quoteid, QuickQuoteXML.QuickQuoteSaveType.Quote, newQSO, errorMsg)
            Catch ex As Exception
#If DEBUG Then
                Debugger.Break()
#End If
                Return Nothing
            End Try
            If newQSO IsNot Nothing Then
                If newQSO.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
            End If
#If DEBUG Then
            Dim timeToComplete As Double = (startTime - DateTime.Now).TotalMilliseconds
            If Math.Abs(timeToComplete) > 20 Then
                Debug.WriteLine($"Loaded the quote in {timeToComplete}ms. No Session version.")
            End If
#End If
            Return newQSO
        End Function

        Public Shared Function SaveQuote(ByRef quoteid As String, ByVal qso As QuickQuoteObject, ByRef errrMsg As String, Optional saveType As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote) As Boolean
#If DEBUG Then
            Dim startTime As DateTime = DateTime.Now
#End If
            If qso.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                Throw New NotTopLevelQuoteException()
            End If

            Dim QQxml As New QuickQuoteXML
            QQxml.SaveQuote(saveType, qso, quoteid, errrMsg)
#If DEBUG Then
            Dim timeToComplete As Double = (startTime - DateTime.Now).TotalMilliseconds
            If Math.Abs(timeToComplete) > 20 Then
                Debug.WriteLine($"Saved the quote in {timeToComplete}ms.")
            End If
#End If

            UpdateDiamondHasLatestSessionVariable(quoteid, False) 'added 12/16/2022

            Return String.IsNullOrWhiteSpace(errrMsg)
        End Function

        Public Shared Function SaveAndRate(ByVal quoteid As String, ByRef saveErr As String, ByRef loadErr As String, Optional saveType As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote) As QuickQuoteObject
            Dim QQxml As New QuickQuoteXML
#If DEBUG Then
            Dim startTime As DateTime = DateTime.Now
#End If
            Dim context As HttpContext = System.Web.HttpContext.Current
            If String.IsNullOrWhiteSpace(quoteid) = False Then
                ' get current quote

                Dim ratedQSO As QuickQuoteObject = Nothing
                Try
                    Dim qso As QuickQuoteObject = Nothing
                    Try
                        If context.Items(GetContextQuoteVariableName(quoteid, saveType)) IsNot Nothing Then
                            qso = DirectCast(context.Items(GetContextQuoteVariableName(quoteid, saveType)), QuickQuoteObject)
                        End If
                    Catch ex As Exception
#If DEBUG Then
                        Debugger.Break()
#End If
                    End Try

                    If qso IsNot Nothing Then
                        If qso.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                            Throw New NotTopLevelQuoteException()
                        End If

                        ' **********************************************************************
                        ' Any LOB-Specific edits that need to be made just before we rate
                        ' **********************************************************************
                        Select Case qso.LobType
                            Case QuickQuoteObject.QuickQuoteLobType.Farm
                                ' For FARM, any dummy additional residences must be removed before rating to avoid the dummy records
                                ' showing up on the summary and generating premium.  A dummy add'l residence is required for the UI to work correctly.
                                ' Valid add'l residences are not affected.   MGB 8/5/19 Bug 39212
                                Dim NumRemoved As Integer = 0
                                Dim err As String = String.Empty
                                IFM.VR.Common.Helpers.FARM.FarmAdditionalDwellingHelper.RemoveAnyFARMDummyAdditionalResidences(quoteid, qso, NumRemoved, err)
                                Exit Select
                            Case Else
                                Exit Select
                        End Select

                        Try
                            ''for tier override (should only be used for testing)
                            'qso.TierTypeId = "1" 'Uniform (probably not needed)
                            '#If DEBUG Then
                            '                            qso.UseTierOverride = True
                            '                            qso.TierAdjustmentTypeId = "13" 'N/A=0; 1=13
                            '#End If

                            'added 5/7/2019; removed 12/16/2022; now called from new method call below
                            'Dim qqHelper As New QuickQuoteHelperClass
                            'CheckQuoteForClientOverwrite(qso, quoteId:=qqHelper.IntegerForString(quoteid))

                            'using debug method overload w/ byref params for rated QuickQuoteObject and xml strings for the request and response
                            'Dim strQQ As String = "" 'not being used here for anything other than debug purposes
                            'Dim strRatedQQ As String = "" 'not being used here for anything other than debug purposes
                            'QQxml.RateQuoteAndSave(saveType, qso, strQQ, ratedQSO, strRatedQQ, quoteid, saveErr)
                            'updated 12/16/2022
                            Dim successfullySavedToDiamond As Boolean = False
                            Dim successfulSaveAndRate As Boolean = SuccessfullySavedAndRatedNewBusinessQuote(qso, quoteId:=quoteid, qqNewBusinessResults:=ratedQSO, successfullySavedToDiamond:=successfullySavedToDiamond, errorMessage:=saveErr, saveType:=saveType, checkForClientOverwrite:=True)
                            context.Items(GetContextQuoteVariableName(quoteid, saveType)) = qso ' Matt A - not sure this does anything 1/1/15
                        Catch ex As Exception
                            saveErr = "Could not perform save and rate."
                        End Try
                    Else
                        saveErr = "Could not find in session quote object."
                    End If

                Catch ex As Exception
                    saveErr = "Save and Rate could not be completed. - " + ex.Message
                End Try
#If DEBUG Then
                Dim timeToComplete As Double = (startTime - DateTime.Now).TotalMilliseconds
                If Math.Abs(timeToComplete) > 20 Then
                    Debug.WriteLine($"Saved and rated the quote in {timeToComplete}ms. Without errors.")
                End If
#End If

                Return ratedQSO
            Else
                saveErr = "Empty quoteid parameter."
                loadErr = "Empty quoteid parameter."
            End If
#If DEBUG Then
            Debug.WriteLine(Environment.NewLine + "Quote rated with error.")
#End If
            Return Nothing

        End Function


        ''' <summary>
        ''' DO NOT USE on the VR3 project it needs the session version SaveAndRate()
        ''' </summary>
        ''' <param name="qso"></param>
        ''' <param name="saveErr"></param>
        ''' <param name="loadErr"></param>
        ''' <param name="saveType"></param>
        ''' <returns></returns>
        Public Shared Function SaveAndRate_NoSession(qso As QuickQuote.CommonObjects.QuickQuoteObject, ByRef saveErr As String, ByRef loadErr As String, Optional saveType As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote) As QuickQuoteObject
#If DEBUG Then
            Dim startTime As DateTime = DateTime.Now
#End If
            Dim QQxml As New QuickQuoteXML
            Dim ratedQSO As QuickQuoteObject = Nothing
            If qso IsNot Nothing Then
                If qso.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                    Throw New NotTopLevelQuoteException()
                End If
                Try
                    If qso.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.ReadOnlyImage Then 'added IF 2/14/2019; original logic in ELSE
                        saveErr = "You cannot Save ReadOnly images"
                    ElseIf qso.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                        Dim successfullySavedAndRatedEndorsement As Boolean = SuccessfullySavedAndRatedEndorsementQuote(qso, ratedQSO, errorMessage:=saveErr, saveTypeView:=saveType)
                    Else
                        ''for tier override (should only be used for testing)
                        'qso.TierTypeId = "1" 'Uniform (probably not needed)
                        'qso.UseTierOverride = True
                        'qso.TierAdjustmentTypeId = "13" 'N/A=0; 1=13

                        'added 5/7/2019
                        'CheckQuoteForClientOverwrite(qso) 'removed 12/16/2022; now called from new method call below

                        'using debug method overload w/ byref params for rated QuickQuoteObject and xml strings for the request and response
                        'Dim strQQ As String = "" 'not being used here for anything other than debug purposes
                        'Dim strRatedQQ As String = "" 'not being used here for anything other than debug purposes
                        'QQxml.RateQuoteAndSave(saveType, qso, strQQ, ratedQSO, strRatedQQ, qso.Database_QuoteId, saveErr)
                        'updated 12/16/2022
                        Dim qId As String = qso.Database_QuoteId
                        Dim successfullySavedToDiamond As Boolean = False
                        Dim successfulSaveAndRate As Boolean = SuccessfullySavedAndRatedNewBusinessQuote(qso, quoteId:=qId, qqNewBusinessResults:=ratedQSO, successfullySavedToDiamond:=successfullySavedToDiamond, errorMessage:=saveErr, saveType:=saveType, checkForClientOverwrite:=True)
                    End If
                Catch ex As Exception
                    saveErr = "Could not perform save and rate."
                End Try
            Else
                saveErr = "Could not find in session quote object."
            End If
#If DEBUG Then
            Dim timeToComplete As Double = (startTime - DateTime.Now).TotalMilliseconds
            If Math.Abs(timeToComplete) > 20 Then
                Debug.WriteLine($"Saved and rated the quote in {timeToComplete}ms. No Session version.")
            End If
#End If
            Return ratedQSO


#If DEBUG Then
            Debug.WriteLine(Environment.NewLine + "Quote rated with error.")
#End If
            Return Nothing

        End Function


        Public Shared Function GetRatedQuoteById_NOSESSION(quoteid As String, ByRef Msg As String, expectedLob As QuickQuoteObject.QuickQuoteLobType, Optional saveType As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote) As QuickQuoteObject
#If DEBUG Then
            Dim startTime As DateTime = DateTime.Now
#End If
            Dim qq As QuickQuoteObject = Nothing
            If IsNumeric(quoteid) = True Then
                Dim errorMsg As String = ""

                Dim QQxml As New QuickQuoteXML
                QQxml.GetRatedQuote(quoteid, qq, saveType, errorMsg)

                If qq IsNot Nothing Then
                    If qq.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then
                        Throw New NotTopLevelQuoteException()
                    End If
                    If qq.QuoteNumber = "" Then
                        Msg += "The quote does not have a quote number."
                    End If

                    If expectedLob <> QuickQuoteObject.QuickQuoteLobType.None AndAlso qq.LobType <> expectedLob Then
                        Msg += "The quoteid provided does not net the expected quote type."
                    End If

                    If qq.Success = True Then
                        'successful quote
                        If qq.ValidationItems IsNot Nothing AndAlso qq.ValidationItems.Count > 0 Then

                            For Each vi As QuickQuoteValidationItem In qq.ValidationItems
                                'just messages
                            Next
                        End If
                    ElseIf qq.ValidationItems IsNot Nothing AndAlso qq.ValidationItems.Count > 0 Then
                        Msg += "The latest rating attempt on this quote had failed. Attempt to rerate the quote."
                        For Each vi As QuickQuoteValidationItem In qq.ValidationItems
                            'validation errors/messages
                        Next
                    Else
                        Msg += "Loading of the quote failed."
                    End If
                Else
                    If errorMsg <> "" Then
                        Msg += errorMsg
                    Else
                        Msg += "That quote could not be located."
                    End If
                End If
            Else
                Msg += "Invalid quote identification."
            End If
            'If String.IsNullOrWhiteSpace(Msg) = False Then
            'quickQuote = Nothing
            'End If

#If DEBUG Then
            Dim timeToComplete As Double = (startTime - DateTime.Now).TotalMilliseconds
            Debug.WriteLine($"Loaded the rated quote from the database. In {timeToComplete}ms.")
#End If

            Return qq
        End Function

        Public Shared Function CreateNewQuote(agencyid As Int32, agencyCode As String, LobID As Int32, ByRef quoteid As String, ByRef ErrMsg As String) As Boolean
            If agencyid > 0 Then
                Dim QQXML As New QuickQuoteXML()
                Dim QuoteObj As New QuickQuoteObject()
                QuoteObj.LobId = LobID
                QuoteObj.AgencyId = agencyid
                QuoteObj.AgencyCode = agencyCode

                'QQXML.SaveQuote(QuickQuoteXML.QuickQuoteSaveType.Quote, QuoteObj, quoteid, ErrMsg)
                'updated 12/16/2022
                If SaveQuote(quoteid, QuoteObj, ErrMsg, saveType:=QuickQuoteXML.QuickQuoteSaveType.Quote) = False Then Return False
                If quoteid Is Nothing OrElse quoteid = "" Then Return False ' quote save failed
                'If ErrMsg IsNot Nothing AndAlso ErrMsg <> "" Then Return False ' client save failed; removed 12/16/2022 - not needed anymore... handled by SaveQuote
                'If QuoteObj.QuoteLevel <> QuickQuote.CommonMethods.QuickQuoteHelperClass.QuoteLevel.TopLevel Then 'removed 12/16/2022 - not needed anymore... handled by SaveQuote
                '    Throw New NotTopLevelQuoteException()
                'End If

                Return True
            Else
                ErrMsg = "No agency selected."
                Return False
            End If

        End Function

        'added 2/13/2019; updated 2/14/2019 w/ optional saveTypeView and rateView params (not currently being used)
        Public Shared Function GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(ByVal policyId As Integer, ByVal policyImageNum As Integer, Optional ByVal saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote, Optional ByVal expectedLobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None, Optional ByVal ratedView As Boolean = False, Optional ByRef errorMessage As String = "") As QuickQuoteObject
            Dim roQQO As QuickQuoteObject = Nothing
            errorMessage = ""

            If policyId > 0 AndAlso policyImageNum > 0 Then
                Dim qqXml As New QuickQuoteXML
                roQQO = qqXml.ReadOnlyQuickQuoteObjectForPolicyInfo(policyId:=policyId, policyImageNum:=policyImageNum, errorMessage:=errorMessage)

                'added 2/18/2019
                If roQQO IsNot Nothing AndAlso System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteLobType), expectedLobType) = True AndAlso expectedLobType <> QuickQuoteObject.QuickQuoteLobType.None AndAlso roQQO.LobType <> expectedLobType Then
                    roQQO = Nothing
                    errorMessage = "Loaded quote does not match the expected LOB"
                End If
            Else
                errorMessage = "Invalid policyId and/or policyImageNum"
            End If

            Return roQQO
        End Function
        Public Shared Function GetEndorsementQuoteForPolicyIdAndImageNum(ByVal policyId As Integer, ByVal policyImageNum As Integer, Optional ByVal saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote, Optional ByVal expectedLobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None, Optional ByVal ratedView As Boolean = False, Optional ByRef errorMessage As String = "") As QuickQuoteObject
            Dim qqEndorsement As QuickQuoteObject = Nothing
            errorMessage = ""

            If policyId > 0 AndAlso policyImageNum > 0 Then
                Dim qqXml As New QuickQuoteXML
                qqEndorsement = qqXml.QuickQuoteEndorsementForPolicyIdAndImageNum(policyId, policyImageNum, errorMessage:=errorMessage)

                'added 2/18/2019
                If qqEndorsement IsNot Nothing AndAlso System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteLobType), expectedLobType) = True AndAlso expectedLobType <> QuickQuoteObject.QuickQuoteLobType.None AndAlso qqEndorsement.LobType <> expectedLobType Then
                    qqEndorsement = Nothing
                    errorMessage = "Loaded quote does not match the expected LOB"
                End If
            Else
                errorMessage = "Invalid policyId and/or policyImageNum"
            End If

            Return qqEndorsement
        End Function
        Public Shared Function SuccessfullySavedEndorsementQuote(ByRef qqEndorsement As QuickQuoteObject, Optional ByRef qqEndorsementResults As QuickQuoteObject = Nothing, Optional ByRef replacedEndorsementQuotePassedIn As Boolean = False, Optional ByRef errorMessage As String = "", Optional ByVal saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote) As Boolean
            Dim success As Boolean = False
            qqEndorsementResults = Nothing
            replacedEndorsementQuotePassedIn = False
            errorMessage = ""

            If qqEndorsement IsNot Nothing Then
                Dim qqXml As New QuickQuoteXML
                'Updated 04/26/2021 for CAP Endorsements Task 52974 MLW
                Dim qqMethodToUse As QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteObjectWork = Nothing
                If qqEndorsement.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                    qqMethodToUse = AddressOf CAP_Endorsement_UpdateDevDictionary
                End If
                success = qqXml.SuccessfullySavedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn_WithqqoMethodToCall(qqEndorsement, qqeResults:=qqEndorsementResults, replacedObjectPassedIn:=replacedEndorsementQuotePassedIn, errorMessage:=errorMessage, qqoMethodToCall:=qqMethodToUse)
                'success = qqXml.SuccessfullySavedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn(qqEndorsement, qqeResults:=qqEndorsementResults, replacedObjectPassedIn:=replacedEndorsementQuotePassedIn, errorMessage:=errorMessage)

                'added 5/18/2021 to get around the fact that calling code passes in Me.Quote, which is a ReadOnly property, and thuse cannot be replaced by this method
                'removing since qqEndorsementResults is actually a clone of newQQE, so it would not include ReadOnly props that we may need (i.e. qqAdditionalInterest.OriginalSourceAI; unless they're manually copied in our Clone method)
                'If success = True AndAlso qqEndorsement IsNot Nothing AndAlso qqEndorsementResults IsNot Nothing Then
                '    If qqEndorsementResults.VersionAndLobInfo IsNot Nothing Then
                '        qqEndorsement.VersionAndLobInfo = qqEndorsementResults.VersionAndLobInfo
                '    End If
                '    If qqEndorsementResults.MultiStateQuotes IsNot Nothing Then
                '        qqEndorsement.MultiStateQuotes = qqEndorsementResults.MultiStateQuotes
                '    End If
                'End If
            Else
                errorMessage = "Invalid Endorsement Quote"
            End If

            Return success
        End Function
        Public Shared Function SuccessfullySavedAndRatedEndorsementQuote(ByRef qqEndorsement As QuickQuoteObject, Optional ByRef qqEndorsementResults As QuickQuoteObject = Nothing, Optional ByRef successfullySaved As Boolean = False, Optional ByRef successfullyRated As Boolean = False, Optional ByRef replacedEndorsementQuotePassedIn As Boolean = False, Optional ByRef errorMessage As String = "", Optional ByVal saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote) As Boolean
            Dim success As Boolean = False
            qqEndorsementResults = Nothing
            successfullySaved = False
            successfullyRated = False
            replacedEndorsementQuotePassedIn = False
            errorMessage = ""

            If qqEndorsement IsNot Nothing Then
                Dim qqXml As New QuickQuoteXML
                'Updated 04/26/2021 for CAP Endorsements Task 52974 MLW
                Dim qqMethodToUse As QuickQuote.CommonMethods.QuickQuoteXML.QuickQuoteObjectWork = Nothing
                If qqEndorsement.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto Then
                    qqMethodToUse = AddressOf CAP_Endorsement_UpdateDevDictionary
                End If
                'success = qqXml.SuccessfullySavedAndRatedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn_WithqqoMethodToCall(qqEndorsement, qqeResults:=qqEndorsementResults, replacedObjectPassedIn:=replacedEndorsementQuotePassedIn, errorMessage:=errorMessage, qqoMethodToCall:=qqMethodToUse)
                'updated 12/16/2022
                success = qqXml.SuccessfullySavedAndRatedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn_WithqqoMethodToCall(qqEndorsement, qqeResults:=qqEndorsementResults, saveSuccessful:=successfullySaved, rateSuccessful:=successfullyRated, replacedObjectPassedIn:=replacedEndorsementQuotePassedIn, errorMessage:=errorMessage, qqoMethodToCall:=qqMethodToUse)
                'success = qqXml.SuccessfullySavedAndRatedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn(qqEndorsement, qqeResults:=qqEndorsementResults, saveSuccessful:=successfullySaved, rateSuccessful:=successfullyRated, replacedObjectPassedIn:=replacedEndorsementQuotePassedIn, errorMessage:=errorMessage)

                'added 5/18/2021 to get around the fact that calling code passes in Me.Quote, which is a ReadOnly property, and thuse cannot be replaced by this method
                'removing since qqEndorsementResults is actually a clone of newQQE, so it would not include ReadOnly props that we may need (i.e. qqAdditionalInterest.OriginalSourceAI; unless they're manually copied in our Clone method)
                'If success = True AndAlso qqEndorsement IsNot Nothing AndAlso qqEndorsementResults IsNot Nothing Then
                '    If qqEndorsementResults.VersionAndLobInfo IsNot Nothing Then
                '        qqEndorsement.VersionAndLobInfo = qqEndorsementResults.VersionAndLobInfo
                '    End If
                '    If qqEndorsementResults.MultiStateQuotes IsNot Nothing Then
                '        qqEndorsement.MultiStateQuotes = qqEndorsementResults.MultiStateQuotes
                '    End If
                'End If
            Else
                errorMessage = "Invalid Endorsement Quote"
            End If

            Return success
        End Function

        'Added 04/27/2021 for CAP Endorsements Task 52974 MLW
        Public Shared Function NewEndorsementQuoteForPolicyIdTransactionDate_TransactionReasonDevDictionaryKeys(ByVal policyId As Integer, ByVal transactionDate As String, Optional ByVal transactionReasonId As Integer = 0, Optional ByVal endorsementRemarks As String = "", Optional ByVal devDictionaryKeys As List(Of QuickQuoteGenericObjectWithTwoStringProperties) = Nothing, Optional ByRef newPolicyImageNum As Integer = 0, Optional ByRef latestPendingEndorsementImageNum As Integer = 0, Optional ByRef latestPendingEndorsementImageTranEffDate As String = "", Optional ByRef errorMessage As String = "", Optional ByVal isBillingUpdate As Boolean = False, Optional ByVal daysBack As Integer = 15, Optional ByVal daysForward As Integer = 15, Optional ByVal MethodToUse As QuickQuote.CommonMethods.QuickQuoteXML.DelegateMethod = Nothing) As QuickQuoteObject
            Dim qqEndorsement As QuickQuoteObject = Nothing
            errorMessage = ""
            newPolicyImageNum = 0
            latestPendingEndorsementImageNum = 0
            latestPendingEndorsementImageTranEffDate = ""

            If policyId > 0 Then
                Dim qqHelper As New QuickQuoteHelperClass
                If qqHelper.IsValidDateString(transactionDate) = True Then
                    Dim qqXml As New QuickQuoteXML
                    If isBillingUpdate = True Then
                        'qqEndorsement = qqXml.NewQuickQuoteBillingUpdateEndorsementForPolicyIdAndTransactionDate(policyId, transactionDate, endorsementRemarks:=endorsementRemarks, newPolicyImageNum:=newPolicyImageNum, latestPendingEndorsementImageNum:=latestPendingEndorsementImageNum, latestPendingEndorsementImageTranEffDate:=latestPendingEndorsementImageTranEffDate, validateTransactionDate:=True, daysBack:=daysBack, daysForward:=daysForward, returnExistingPendingQuickQuoteEndorsement:=False, onlyReturnPendingQuickQuoteEndorsementWhenDateMatches:=True, errorMessage:=errorMessage)
                        qqEndorsement = qqXml.NewQuickQuoteBillingUpdateEndorsementForPolicyIdAndTransactionDate_OptionalTransReasonIdAndDevDictionaryKeys_WithDelegate(policyId, transactionDate, endorsementRemarks:=endorsementRemarks, newPolicyImageNum:=newPolicyImageNum, latestPendingEndorsementImageNum:=latestPendingEndorsementImageNum, latestPendingEndorsementImageTranEffDate:=latestPendingEndorsementImageTranEffDate, validateTransactionDate:=True, daysBack:=daysBack, daysForward:=daysForward, returnExistingPendingQuickQuoteEndorsement:=False, onlyReturnPendingQuickQuoteEndorsementWhenDateMatches:=True, errorMessage:=errorMessage, DelegateMethod:=MethodToUse)

                    Else
                        qqEndorsement = qqXml.NewQuickQuoteEndorsementForPolicyIdAndTransactionDate_OptionalTransReasonIdAndDevDictionaryKeys_WithDelegate(policyId, transactionDate, transactionReasonId:=transactionReasonId, endorsementRemarks:=endorsementRemarks, devDictionaryKeys:=devDictionaryKeys, newPolicyImageNum:=newPolicyImageNum, latestPendingEndorsementImageNum:=latestPendingEndorsementImageNum, latestPendingEndorsementImageTranEffDate:=latestPendingEndorsementImageTranEffDate, validateTransactionDate:=True, daysBack:=daysBack, daysForward:=daysForward, returnExistingPendingQuickQuoteEndorsement:=False, onlyReturnPendingQuickQuoteEndorsementWhenDateMatches:=True, errorMessage:=errorMessage, DelegateMethod:=MethodToUse)
                    End If
                Else
                    errorMessage = "Invalid transaction date"
                End If
            Else
                errorMessage = "Invalid policyId"
            End If

            Return qqEndorsement
        End Function
        Public Shared Function NewEndorsementQuoteForPolicyIdTransactionDate(ByVal policyId As Integer, ByVal transactionDate As String, Optional ByVal endorsementRemarks As String = "", Optional ByRef newPolicyImageNum As Integer = 0, Optional ByRef latestPendingEndorsementImageNum As Integer = 0, Optional ByRef latestPendingEndorsementImageTranEffDate As String = "", Optional ByRef errorMessage As String = "", Optional ByVal isBillingUpdate As Boolean = False, Optional ByVal daysBack As Integer = 15, Optional ByVal daysForward As Integer = 15) As QuickQuoteObject
            Dim qqEndorsement As QuickQuoteObject = Nothing
            errorMessage = ""
            newPolicyImageNum = 0
            latestPendingEndorsementImageNum = 0
            latestPendingEndorsementImageTranEffDate = ""

            If policyId > 0 Then
                Dim qqHelper As New QuickQuoteHelperClass
                If qqHelper.IsValidDateString(transactionDate) = True Then
                    Dim qqXml As New QuickQuoteXML
                    If isBillingUpdate = True Then
                        qqEndorsement = qqXml.NewQuickQuoteBillingUpdateEndorsementForPolicyIdAndTransactionDate(policyId, transactionDate, endorsementRemarks:=endorsementRemarks, newPolicyImageNum:=newPolicyImageNum, latestPendingEndorsementImageNum:=latestPendingEndorsementImageNum, latestPendingEndorsementImageTranEffDate:=latestPendingEndorsementImageTranEffDate, validateTransactionDate:=True, daysBack:=daysBack, daysForward:=daysForward, returnExistingPendingQuickQuoteEndorsement:=False, onlyReturnPendingQuickQuoteEndorsementWhenDateMatches:=True, errorMessage:=errorMessage)
                    Else
                        qqEndorsement = qqXml.NewQuickQuoteEndorsementForPolicyIdAndTransactionDate(policyId, transactionDate, endorsementRemarks:=endorsementRemarks, newPolicyImageNum:=newPolicyImageNum, latestPendingEndorsementImageNum:=latestPendingEndorsementImageNum, latestPendingEndorsementImageTranEffDate:=latestPendingEndorsementImageTranEffDate, validateTransactionDate:=True, daysBack:=daysBack, daysForward:=daysForward, returnExistingPendingQuickQuoteEndorsement:=False, onlyReturnPendingQuickQuoteEndorsementWhenDateMatches:=True, errorMessage:=errorMessage)
                    End If
                Else
                    errorMessage = "Invalid transaction date"
                End If
            Else
                errorMessage = "Invalid policyId"
            End If

            Return qqEndorsement
        End Function
        'added 2/14/2019
        Public Shared Function GetEndorsementQuoteFromContext(ByVal policyId As Integer, ByVal policyImageNum As Integer, Optional ByVal saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote, Optional ByVal expectedLobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None, Optional ByRef errorMessage As String = "") As QuickQuoteObject
            Dim qqEndorsement As QuickQuoteObject = Nothing
            errorMessage = ""

            If policyId > 0 AndAlso policyImageNum > 0 Then
                Dim context As HttpContext = System.Web.HttpContext.Current
                If context IsNot Nothing AndAlso context.Items IsNot Nothing AndAlso context.Items(GetContextEndorsementVariableName(policyId, policyId, saveTypeView:=saveTypeView)) IsNot Nothing Then
                    qqEndorsement = DirectCast(context.Items(GetContextEndorsementVariableName(policyId, policyId, saveTypeView:=saveTypeView)), QuickQuoteObject)

                    'added 2/18/2019
                    If qqEndorsement IsNot Nothing AndAlso System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteLobType), expectedLobType) = True AndAlso expectedLobType <> QuickQuoteObject.QuickQuoteLobType.None AndAlso qqEndorsement.LobType <> expectedLobType Then
                        qqEndorsement = Nothing
                        If context.Items(GetContextEndorsementVariableName(policyId, policyId, saveTypeView:=saveTypeView)) IsNot Nothing Then
                            context.Items(GetContextEndorsementVariableName(policyId, policyId, saveTypeView:=saveTypeView)) = Nothing
                        End If
                        errorMessage = "Loaded quote does not match the expected LOB"
                    End If
                End If
            Else
                errorMessage = "Invalid policyId and/or policyImageNum"
            End If

            Return qqEndorsement
        End Function
        Public Shared Function GetReadOnlyImageFromContext(ByVal policyId As Integer, ByVal policyImageNum As Integer, Optional ByVal saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote, Optional ByVal expectedLobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None, Optional ByRef errorMessage As String = "") As QuickQuoteObject
            Dim roQQO As QuickQuoteObject = Nothing
            errorMessage = ""

            If policyId > 0 AndAlso policyImageNum > 0 Then
                Dim context As HttpContext = System.Web.HttpContext.Current
                If context IsNot Nothing AndAlso context.Items IsNot Nothing AndAlso context.Items(GetContextReadOnlyImageVariableName(policyId, policyId, saveTypeView:=saveTypeView)) IsNot Nothing Then
                    roQQO = DirectCast(context.Items(GetContextReadOnlyImageVariableName(policyId, policyId, saveTypeView:=saveTypeView)), QuickQuoteObject)

                    'added 2/18/2019
                    If roQQO IsNot Nothing AndAlso System.Enum.IsDefined(GetType(QuickQuoteObject.QuickQuoteLobType), expectedLobType) = True AndAlso expectedLobType <> QuickQuoteObject.QuickQuoteLobType.None AndAlso roQQO.LobType <> expectedLobType Then
                        roQQO = Nothing
                        If context.Items(GetContextReadOnlyImageVariableName(policyId, policyId, saveTypeView:=saveTypeView)) IsNot Nothing Then
                            context.Items(GetContextReadOnlyImageVariableName(policyId, policyId, saveTypeView:=saveTypeView)) = Nothing
                        End If
                        errorMessage = "Loaded quote does not match the expected LOB"
                    End If
                End If
            Else
                errorMessage = "Invalid policyId and/or policyImageNum"
            End If

            Return roQQO
        End Function
        Public Shared Function GetEndorsementQuoteFromAnywhere(ByVal policyId As Integer, ByVal policyImageNum As Integer, Optional ByVal saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote, Optional ByVal expectedLobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None, Optional ByRef errorMessage As String = "") As QuickQuoteObject
            Dim qqEndorsement As QuickQuoteObject = Nothing
            errorMessage = ""

            If policyId > 0 AndAlso policyImageNum > 0 Then
                qqEndorsement = GetEndorsementQuoteFromContext(policyId, policyImageNum, saveTypeView:=saveTypeView, expectedLobType:=expectedLobType, errorMessage:=errorMessage)
                If qqEndorsement Is Nothing Then
                    qqEndorsement = GetEndorsementQuoteForPolicyIdAndImageNum(policyId, policyImageNum, saveTypeView:=saveTypeView, expectedLobType:=expectedLobType, errorMessage:=errorMessage)
                    If qqEndorsement IsNot Nothing Then
                        Dim context As HttpContext = System.Web.HttpContext.Current
                        If context IsNot Nothing AndAlso context.Items IsNot Nothing Then
                            'put in session
                            context.Items(GetContextEndorsementVariableName(policyId, policyId, saveTypeView:=saveTypeView)) = qqEndorsement
                        End If
                    End If
                End If
            Else
                errorMessage = "Invalid policyId and/or policyImageNum"
            End If

            Return qqEndorsement
        End Function
        Public Shared Function GetReadOnlyImageFromAnywhere(ByVal policyId As Integer, ByVal policyImageNum As Integer, Optional ByVal saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote, Optional ByVal expectedLobType As QuickQuoteObject.QuickQuoteLobType = QuickQuoteObject.QuickQuoteLobType.None, Optional ByRef errorMessage As String = "") As QuickQuoteObject
            Dim roQQO As QuickQuoteObject = Nothing
            errorMessage = ""

            If policyId > 0 AndAlso policyImageNum > 0 Then
                roQQO = GetReadOnlyImageFromContext(policyId, policyImageNum, saveTypeView:=saveTypeView, expectedLobType:=expectedLobType, errorMessage:=errorMessage)
                If roQQO Is Nothing Then
                    roQQO = GetReadOnlyQuickQuoteObjectForPolicyIdAndImageNum(policyId, policyImageNum, saveTypeView:=saveTypeView, expectedLobType:=expectedLobType, errorMessage:=errorMessage)
                    If roQQO IsNot Nothing Then
                        Dim context As HttpContext = System.Web.HttpContext.Current
                        If context IsNot Nothing AndAlso context.Items IsNot Nothing Then
                            'put in session
                            context.Items(GetContextReadOnlyImageVariableName(policyId, policyId, saveTypeView:=saveTypeView)) = roQQO
                        End If
                    End If
                End If
            Else
                errorMessage = "Invalid policyId and/or policyImageNum"
            End If

            Return roQQO
        End Function
        'added 2/15/2019
        Public Shared Function SuccessfullySavedAndRatedEndorsementQuoteFromContext(ByVal policyId As Integer, ByVal policyImageNum As Integer, Optional ByRef qqEndorsementResults As QuickQuoteObject = Nothing, Optional ByRef successfullySaved As Boolean = False, Optional ByRef successfullyRated As Boolean = False, Optional ByRef replacedEndorsementQuotePassedIn As Boolean = False, Optional ByRef errorMessage As String = "", Optional ByVal saveTypeView As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote) As Boolean
            Dim success As Boolean = False
            qqEndorsementResults = Nothing
            successfullySaved = False
            successfullyRated = False
            replacedEndorsementQuotePassedIn = False
            errorMessage = ""

            Dim qqEndorsement As QuickQuoteObject = GetEndorsementQuoteFromContext(policyId, policyImageNum, saveTypeView, errorMessage:=errorMessage)
            If qqEndorsement IsNot Nothing Then
                Dim qqXml As New QuickQuoteXML
                'success = qqXml.SuccessfullySavedAndRatedQuickQuoteEndorsementInDiamond_ReplaceObjectPassedIn(qqEndorsement, qqeResults:=qqEndorsementResults, saveSuccessful:=successfullySaved, rateSuccessful:=successfullyRated, replacedObjectPassedIn:=replacedEndorsementQuotePassedIn, errorMessage:=errorMessage)
                'updated 2/25/2019
                success = SuccessfullySavedAndRatedEndorsementQuote(qqEndorsement, qqEndorsementResults:=qqEndorsementResults, successfullySaved:=successfullySaved, successfullyRated:=successfullyRated, replacedEndorsementQuotePassedIn:=replacedEndorsementQuotePassedIn, errorMessage:=errorMessage, saveTypeView:=saveTypeView)
            Else
                If String.IsNullOrWhiteSpace(errorMessage) = True Then
                    errorMessage = "Unable to find Endorsement Quote in Context"
                End If
            End If

            Return success
        End Function
        'added 2/27/2019
        Public Shared Function SuccessfullyDeletedEndorsementQuote(ByVal policyId As Integer, Optional ByVal policyImageNum As Integer = 0, Optional ByRef errorMessage As String = "") As Boolean
            Dim success As Boolean = False
            errorMessage = ""

            If policyId > 0 Then
                Dim qqXml As New QuickQuoteXML
                success = qqXml.SuccessfullyDeletedPendingEndorsementImageInDiamond(policyId, policyImageNum:=policyImageNum, errorMessage:=errorMessage)

                'If success = True Then 'added 7/12/2019; removed 7/17/2019 - will now happen automatically from QQ library when image is deleted
                '    Helpers.FileUploadHelper.RemoveAllFilesForPolicyIdAndImageNum(policyId, policyImageNum)
                'End If
            Else
                errorMessage = "Invalid policyId"
            End If

            Return success
        End Function

        'added 5/6/2019
        Public Shared Sub CheckQuoteForClientOverwrite(ByRef qqo As QuickQuoteObject, Optional ByVal quoteId As Integer = 0)
            If qqo IsNot Nothing Then
                Dim qqHelper As New QuickQuoteHelperClass
                If quoteId <= 0 Then
                    If qqHelper.IsPositiveIntegerString(qqo.Database_QuoteId) = True Then
                        quoteId = CInt(qqo.Database_QuoteId)
                    End If
                End If
                'originally from ctlInsuredList; written to only include clientIds that came from Populate as to exclude ones loaded from Client Search (though it could have come from Client Search and then showed up on Populate later due to reload)... more logic added but may not need it all (depends on whether or not we want to try to limit updates to client created for current quote); IsNewClientIdInSessionFromDiamondSave should ensure that the clientId was created during the current user session; IsQuoteIdInSessionWithoutClientId just indicates that the quote was populated without a clientId during the current session; IsClientIdInSessionFromPopulate indicates that the clientId was populated for some quote during the user session (not necessarily this quote)
                If qqo.Client IsNot Nothing AndAlso qqo.Client.HasValidClientId() = True AndAlso IsQuoteIdInSessionWithoutClientId(quoteId) = True AndAlso IsClientIdInSessionFromPopulate(qqHelper.IntegerForString(qqo.Client.ClientId)) = True AndAlso QuickQuote.CommonMethods.QuickQuoteHelperClass.IsNewClientIdInSessionFromDiamondSave(qqHelper.IntegerForString(qqo.Client.ClientId)) = True Then
                    qqo.Client.OverwriteClientInfoForDiamondId = True
                End If
            End If
        End Sub
        'moved here from ctlInsuredList so it can be called from Save/Rate too
        Public Shared Function QuoteIdsInSessionWithoutClientId() As List(Of Integer)
            Dim quoteIds As List(Of Integer) = Nothing

            If QuickQuoteHelperClass.IsSessionValid() = True AndAlso System.Web.HttpContext.Current.Session("QuoteIdsWithoutClientId") IsNot Nothing Then
                quoteIds = System.Web.HttpContext.Current.Session("QuoteIdsWithoutClientId")
            End If

            Return quoteIds
        End Function
        Private Sub SetQuoteIdsInSessionWithoutClientId(ByVal quoteIds As List(Of Integer))
            If QuickQuoteHelperClass.IsSessionValid() = True Then
                If quoteIds IsNot Nothing Then
                    If System.Web.HttpContext.Current.Session("QuoteIdsWithoutClientId") IsNot Nothing Then
                        System.Web.HttpContext.Current.Session("QuoteIdsWithoutClientId") = quoteIds
                    Else
                        System.Web.HttpContext.Current.Session.Add("QuoteIdsWithoutClientId", quoteIds)
                    End If
                Else
                    If System.Web.HttpContext.Current.Session("QuoteIdsWithoutClientId") IsNot Nothing Then
                        System.Web.HttpContext.Current.Session("QuoteIdsWithoutClientId") = Nothing
                        System.Web.HttpContext.Current.Session.Remove("QuoteIdsWithoutClientId")
                    End If
                End If
            End If
        End Sub
        Public Shared Sub AddToQuoteIdsInSessionWithoutClientId(ByVal qId As Integer)
            If QuickQuoteHelperClass.IsSessionValid() = True AndAlso qId > 0 AndAlso IsQuoteIdInSessionWithoutClientId(qId) = False Then
                Dim quoteIds As List(Of Integer) = QuoteIdsInSessionWithoutClientId()
                QuickQuote.CommonMethods.QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(qId, quoteIds)
                Dim qsh As New QuoteSaveHelpers
                qsh.SetQuoteIdsInSessionWithoutClientId(quoteIds)
            End If
        End Sub
        Public Shared Function IsQuoteIdInSessionWithoutClientId(ByVal qId As Integer) As Boolean
            Dim isInList As Boolean = False

            Dim quoteIds As List(Of Integer) = QuoteIdsInSessionWithoutClientId()
            If quoteIds IsNot Nothing AndAlso quoteIds.Count > 0 AndAlso quoteIds.Contains(qId) = True Then
                isInList = True
            End If

            Return isInList
        End Function
        Public Shared Function ClientIdsInSessionFromPopulate() As List(Of Integer)
            Dim quoteIds As List(Of Integer) = Nothing

            If QuickQuoteHelperClass.IsSessionValid() = True AndAlso System.Web.HttpContext.Current.Session("ClientIdsFromPopulate") IsNot Nothing Then
                quoteIds = System.Web.HttpContext.Current.Session("ClientIdsFromPopulate")
            End If

            Return quoteIds
        End Function
        Private Sub SetClientIdsInSessionFromPopulate(ByVal clientIds As List(Of Integer))
            If QuickQuoteHelperClass.IsSessionValid() = True Then
                If clientIds IsNot Nothing Then
                    If System.Web.HttpContext.Current.Session("ClientIdsFromPopulate") IsNot Nothing Then
                        System.Web.HttpContext.Current.Session("ClientIdsFromPopulate") = clientIds
                    Else
                        System.Web.HttpContext.Current.Session.Add("ClientIdsFromPopulate", clientIds)
                    End If
                Else
                    If System.Web.HttpContext.Current.Session("ClientIdsFromPopulate") IsNot Nothing Then
                        System.Web.HttpContext.Current.Session("ClientIdsFromPopulate") = Nothing
                        System.Web.HttpContext.Current.Session.Remove("ClientIdsFromPopulate")
                    End If
                End If
            End If
        End Sub
        Public Shared Sub AddToClientIdsInSessionFromPopulate(ByVal cId As Integer)
            If QuickQuoteHelperClass.IsSessionValid() = True AndAlso cId > 0 AndAlso IsClientIdInSessionFromPopulate(cId) = False Then
                Dim clientIds As List(Of Integer) = ClientIdsInSessionFromPopulate()
                QuickQuote.CommonMethods.QuickQuoteHelperClass.AddUniqueIntegerToIntegerList(cId, clientIds)
                Dim qsh As New QuoteSaveHelpers
                qsh.SetClientIdsInSessionFromPopulate(clientIds)
            End If
        End Sub
        Public Shared Function IsClientIdInSessionFromPopulate(ByVal cId As Integer) As Boolean
            Dim isInList As Boolean = False

            Dim clientIds As List(Of Integer) = ClientIdsInSessionFromPopulate()
            If clientIds IsNot Nothing AndAlso clientIds.Count > 0 AndAlso clientIds.Contains(cId) = True Then
                isInList = True
            End If

            Return isInList
        End Function

        'added 4/26/2021 for CAP Endorsements Task 52974 MLW
        Public Shared Sub CAP_Endorsement_UpdateDevDictionary(ByRef qqo As QuickQuoteObject)
            'This is needed because we need to update the DevDictionary with the diamond number that is returned after save for any vehicles added. We use a delegate to allow for the DevDictionary save to happen when saving or saving and rating in QuickQuote.
            If qqo IsNot Nothing AndAlso qqo.LobType = QuickQuoteObject.QuickQuoteLobType.CommercialAuto AndAlso qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                Dim ddh As DevDictionaryHelper.DevDictionaryHelper = Nothing
                Dim CAPEndorsementsDictionaryName As String = "CAPEndorsementsDetails"
                Select Case TypeOfEndorsement(qqo)
                    Case "Add/Delete Vehicle"
                        Dim vehicleIndex As Integer = 0
                        Dim qqHelper As New QuickQuoteHelperClass
                        If qqo.Vehicles IsNot Nothing AndAlso qqo.Vehicles.Count > 0 Then 'Added 05/24/2021 for bug 62071 MLW 
                            For Each vehicle As QuickQuoteVehicle In qqo.Vehicles
                                If qqHelper.IsQuickQuoteVehicleNewToImage(vehicle, qqo) Then
                                    If ddh Is Nothing Then
                                        ddh = New DevDictionaryHelper.DevDictionaryHelper(qqo, CAPEndorsementsDictionaryName, qqo.LobType)
                                    End If
                                    ddh.UpdateDevDictionaryVehicleList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, vehicleIndex, vehicle)
                                    qqo.TransactionReasonId = 10169 'Endorsement Change Dec and Full Revised Dec
                                End If
                                vehicleIndex += 1
                            Next
                        End If
                    Case "Add/Delete Driver"
                        Dim driverIndex As Integer = 0
                        Dim qqHelper As New QuickQuoteHelperClass
                        If qqo.Drivers IsNot Nothing AndAlso qqo.Drivers.Count > 0 Then
                            For Each driver As QuickQuoteDriver In qqo.Drivers
                                If qqHelper.IsQuickQuoteDriverNewToImage(driver, qqo) Then
                                    If ddh Is Nothing Then
                                        ddh = New DevDictionaryHelper.DevDictionaryHelper(qqo, CAPEndorsementsDictionaryName, qqo.LobType)
                                    End If
                                    ddh.UpdateDevDictionaryDriverList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, driverIndex, driver)
                                    'TransactionReasonId is always 10168 for drivers
                                End If
                                driverIndex += 1
                            Next
                        End If
                        'NOTE: Since we are not using the AI Num in the DevDictionary, we do not need this for AIs. If we do use this, the AI could be added back when deleting an AI copied from the vehicle level to the quote level. See ctlVehicleAdditionalInterestList aiRemovalRequested to view the logic that copies the AIs.
                        'Case "Add/Delete Additional Interest"
                        '    Dim aiIndex As Integer = 0
                        '    Dim qqHelper As New QuickQuoteHelperClass
                        '    If qqo.AdditionalInterests IsNot Nothing AndAlso qqo.AdditionalInterests.Count > 0 Then
                        '        For Each ai As QuickQuoteAdditionalInterest In qqo.AdditionalInterests
                        '            If qqHelper.IsQuickQuoteAdditionalInterestNewToImage(ai, qqo) Then
                        '                Dim aiVehicleList As List(Of Integer) = New List(Of Integer)
                        '                'get current list of vehicles if AI add record exists
                        '                Dim aiExistsInAddList As Boolean = False
                        '                If ddh Is Nothing Then
                        '                    ddh = New DevDictionaryHelper.DevDictionaryHelper(qqo, CAPEndorsementsDictionaryName, qqo.LobType)
                        '                End If
                        '                Dim strAddedVehicleNumsForAIList As String = ""
                        '                If ddh.GetAdditionalInterestDictionary IsNot Nothing AndAlso ddh.GetAdditionalInterestDictionary.Count > 0 Then
                        '                    For Each additionalInterest As DevDictionaryHelper.AdditionalInterestInfo In ddh.GetAdditionalInterestDictionary
                        '                        If additionalInterest.addOrDelete = DevDictionaryHelper.DevDictionaryHelper.addItem AndAlso additionalInterest.diaAINumber = ai.ListId Then
                        '                            aiExistsInAddList = True
                        '                            strAddedVehicleNumsForAIList = additionalInterest.VehicleNumList
                        '                            Exit For
                        '                        End If
                        '                    Next
                        '                End If
                        '                If aiExistsInAddList = True AndAlso String.IsNullOrWhiteSpace(strAddedVehicleNumsForAIList) = False Then
                        '                    aiVehicleList = strAddedVehicleNumsForAIList.Split(","c).[Select](Function(n) Integer.Parse(n)).ToList()
                        '                End If
                        '                ddh.UpdateDevDictionaryAdditionalInterestList(DevDictionaryHelper.DevDictionaryHelper.DevDictionaryListType.Add, ai, aiVehicleList)
                        '                qqo.TransactionReasonId = 10169 'Endorsement Change Dec and Full Revised Dec
                        '            End If
                        '            aiIndex += 1
                        '        Next
                        '    End If
                End Select
            End If
        End Sub
        Delegate Sub QuickQuoteObjectWork(ByRef qqo As QuickQuoteObject)
        Public Shared Function QQDevDictionary_GetItem(ByVal qqo As QuickQuoteObject, ByVal key As String, Optional ByVal isPageSpecific As Boolean = False) As String
            Dim str As String = ""
            str = qqo.GetDevDictionaryItem(GetPageNameForDevDictionary(isPageSpecific), key)
            Return str
        End Function
        Public Shared Function GetPageNameForDevDictionary(ByVal isPageSpecific As Boolean, Optional ByVal senderName As String = "") As String
            Dim pageName As String = ""
            If isPageSpecific = True AndAlso String.IsNullOrWhiteSpace(senderName) = False Then
                pageName = senderName
            Else
                pageName = "global"
            End If
            Return pageName
        End Function
        Public Shared Function TypeOfEndorsement(ByVal qqo As QuickQuoteObject) As String
            If qqo IsNot Nothing AndAlso qqo.QuoteTransactionType = QuickQuoteObject.QuickQuoteTransactionType.EndorsementQuote Then
                Return QQDevDictionary_GetItem(qqo, "Type_Of_Endorsement_Selected")
            End If
            Return Nothing
        End Function

        'added 12/16/2022
        Public Shared Function SuccessfullySavedAndRatedNewBusinessQuote(ByRef qqNewBusiness As QuickQuoteObject, Optional ByRef quoteId As String = "", Optional ByRef qqNewBusinessResults As QuickQuoteObject = Nothing, Optional ByRef successfullySavedToDiamond As Boolean = False, Optional ByRef errorMessage As String = "", Optional ByVal saveType As QuickQuoteXML.QuickQuoteSaveType = QuickQuoteXML.QuickQuoteSaveType.Quote, Optional ByVal checkForClientOverwrite As Boolean = True, Optional ByRef strQqNewBusiness As String = "", Optional ByRef strQqNewBusinessResults As String = "") As Boolean
            Dim success As Boolean = False
            qqNewBusinessResults = Nothing
            successfullySavedToDiamond = False
            errorMessage = ""
            strQqNewBusiness = ""
            strQqNewBusinessResults = ""

            If qqNewBusiness IsNot Nothing Then
                Dim qqHelper As New QuickQuoteHelperClass
                If qqHelper.IsPositiveIntegerString(quoteId) = False AndAlso qqHelper.IsPositiveIntegerString(qqNewBusiness.Database_QuoteId) = True Then
                    quoteId = qqNewBusiness.Database_QuoteId
                End If

                If checkForClientOverwrite = True Then
                    CheckQuoteForClientOverwrite(qqNewBusiness, quoteId:=qqHelper.IntegerForString(quoteId))
                End If

                Dim qqXml As New QuickQuoteXML
                qqXml.RateQuoteAndSave(saveType, qqNewBusiness, quickQuoteXml:=strQqNewBusiness, ratedQuickQuote:=qqNewBusinessResults, ratedQuickQuoteXml:=strQqNewBusinessResults, quoteId:=quoteId, errorMsg:=errorMessage, successfullySavedToDiamond:=successfullySavedToDiamond)

                If qqNewBusinessResults IsNot Nothing AndAlso qqNewBusinessResults.Success = True Then
                    success = True
                    If successfullySavedToDiamond = False Then 'just in case this didn't get set correctly; should always be True if Rate was successful
                        successfullySavedToDiamond = True
                    End If
                End If

                UpdateDiamondHasLatestSessionVariable(quoteId, successfullySavedToDiamond)
            Else
                errorMessage = "Invalid New Business Quote"
            End If

            Return success
        End Function
        Public Shared Sub UpdateDiamondHasLatestSessionVariable(ByVal quoteId As String, ByVal success As Boolean)
            If QuickQuoteHelperClass.IsSessionValid() = True Then
                Dim qqHelper As New QuickQuoteHelperClass
                If qqHelper.IsPositiveIntegerString(quoteId) = True Then
                    Dim sessionVar As String = DiamondHasLatestSessionVariable(CInt(quoteId))
                    If System.Web.HttpContext.Current.Session(sessionVar) IsNot Nothing Then
                        System.Web.HttpContext.Current.Session(sessionVar) = success.ToString
                    Else
                        System.Web.HttpContext.Current.Session.Add(sessionVar, success.ToString)
                    End If
                End If
            End If
        End Sub
        Public Shared Function DiamondHasLatestSessionVariable(ByVal quoteId As Integer) As String
            Return "VR_DiamondHasLatest_" & quoteId.ToString
        End Function
        Public Shared Function DiamondHasLatest(ByVal quoteId As String) As Boolean
            Dim hasIt As Boolean = False

            If QuickQuoteHelperClass.IsSessionValid() = True Then
                Dim qqHelper As New QuickQuoteHelperClass
                If qqHelper.IsPositiveIntegerString(quoteId) = True Then
                    Dim sessionVar As String = DiamondHasLatestSessionVariable(CInt(quoteId))
                    If System.Web.HttpContext.Current.Session(sessionVar) IsNot Nothing AndAlso String.IsNullOrWhiteSpace(System.Web.HttpContext.Current.Session(sessionVar).ToString) = False Then
                        hasIt = qqHelper.BitToBoolean(System.Web.HttpContext.Current.Session(sessionVar).ToString)
                    End If
                End If
            End If

            Return hasIt
        End Function



    End Class

End Namespace