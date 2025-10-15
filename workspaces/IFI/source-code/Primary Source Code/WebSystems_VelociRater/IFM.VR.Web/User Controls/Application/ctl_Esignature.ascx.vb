Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions
Imports IFM.VR.Validation.ObjectValidation.PersLines.LOB.Common
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports System.IO

Public Class ctl_Esignature
    Inherits VRControlBase

    Public Property EsignatureIndex As Int32
        Get
            If ViewState("vs_EsignatureIndex") Is Nothing Then
                ViewState("vs_EsignatureIndex") = -1
            End If
            Return CInt(ViewState("vs_EsignatureIndex"))
        End Get
        Set(value As Int32)
            ViewState("vs_EsignatureIndex") = value
        End Set
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Updated 1/11/2022 for Bug 67521 MLW
        'If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
        If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly AndAlso Me.hasEsigOption Then
            If Not IsPostBack Then
                Me.MainAccordionDivId = Me.divEsignature.ClientID
            End If
        End If
    End Sub

    Public Overrides Sub AddScriptAlways()
        'Updated 1/11/2022 for Bug 67521 MLW
        'If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
        If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly AndAlso Me.hasEsigOption Then
            Dim showHideEmailScript As String = "if($(""#" + Me.rblEsignature.ClientID + " input:checked"").val() == 'Yes') {$(""#" + Me.lblEsigEmail.ClientID + """).show();$(""#" + Me.txtEsigEmail.ClientID + """).show();} else { $(""#" + Me.lblEsigEmail.ClientID + """).hide();$(""#" + Me.txtEsigEmail.ClientID + """).hide(); }"
            Me.VRScript.CreateJSBinding(rblEsignature, ctlPageStartupScript.JsEventType.onclick, showHideEmailScript)
        End If
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        'Updated 1/11/2022 for Bug 67521 MLW
        'If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
        If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly AndAlso Me.hasEsigOption Then
            Me.VRScript.CreateAccordion(Me.divEsignature.ClientID, Me.accordActive, "0")
            Me.VRScript.StopEventPropagation(Me.lnkBtnSave.ClientID)
            If Not IsOnAppPage Then
                Dim usingEsig As Boolean = False
                Dim eSigOption = QQDevDictionary_GetItem("Sign_Application_Using_eSignature")
                If eSigOption IsNot Nothing AndAlso eSigOption = "Yes" Then
                    usingEsig = True
                End If
                If usingEsig = False Then
                    Dim copyPH1EmailToEsigEmail As String = "$(""#" + Me.txtEsigEmail.ClientID + """).val($(""#"" + phEmail).val());"
                    Me.VRScript.CreateJSBinding(rblEsignature, ctlPageStartupScript.JsEventType.onclick, copyPH1EmailToEsigEmail)
                End If
                Me.VRScript.AddVariableLine("var esigEmail = '" + Me.txtEsigEmail.ClientID + "';") 'used on ctlInsured.ascx and VrMiniClientSearch.js to copy ph1 email to eSig email
            End If
        End If
    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        'Updated 1/11/2022 for Bug 67521 MLW
        'If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
        If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly AndAlso Me.hasEsigOption Then
            LoadStaticData()
            If Not IsOnAppPage Then
                Me.lblEsignatureTitle.Text = "SIGNATURE INFORMATION"
            End If
            If Me.Quote IsNot Nothing Then
                Dim eSigOption = QQDevDictionary_GetItem("Sign_Application_Using_eSignature")
                If eSigOption Is Nothing OrElse eSigOption = "" Then
                    'Me.rblEsignature.SelectedValue = "" 'Throws error at App summary on existing quotes without eSig option.
                    Me.rblEsignature.ClearSelection()
                    Me.hdnPriorEsigOption.Value = ""
                    Me.hdnPriorEmailAddress.Value = ""
                    Me.hdnPriorZipCodeStandard.Value = String.Empty
                    Me.lblEsigEmail.Style.Add("display", "none")
                    Me.txtEsigEmail.Style.Add("display", "none")
                    Me.txtEsigEmail.Text = ""
                Else
                    If eSigOption = "Yes" Then
                        Dim esigEmail As String = Nothing
                        esigEmail = QQDevDictionary_GetItem("eSignature_Email")
                        Me.rblEsignature.SelectedValue = "Yes"
                        Me.hdnPriorEsigOption.Value = "Yes"
                        Me.hdnPriorEmailAddress.Value = esigEmail
                        Me.hdnPriorZipCodeStandard.Value = QQDevDictionary_GetItem("eSignature_ZipCodeStandard")
                        Me.lblEsigEmail.Style.Add("display", "")
                        Me.txtEsigEmail.Style.Add("display", "")
                        Me.txtEsigEmail.Text = esigEmail
                    Else
                        Me.rblEsignature.SelectedValue = "No"
                        Me.hdnPriorEsigOption.Value = "No"
                        Me.hdnPriorEmailAddress.Value = ""
                        Me.hdnPriorZipCodeStandard.Value = String.Empty
                        Me.lblEsigEmail.Style.Add("display", "none")
                        Me.txtEsigEmail.Style.Add("display", "none")
                        Me.txtEsigEmail.Text = ""
                    End If
                End If
            End If
        End If
    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        'Updated 1/11/2022 for Bug 67521 MLW
        'If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
        If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly AndAlso Me.hasEsigOption Then
            MyBase.ValidateControl(valArgs)
            Dim myaccordClientId As String = If(Me.ParentVrControl IsNot Nothing, Me.ParentVrControl.ListAccordionDivId, "")
            Dim paneIndex As Int32 = Me.EsignatureIndex

            'Updated 04/22/2020 for Bug 45892 MLW
            'Dim hasSelectedEsigValue = Me.rblEsignature.SelectedValue = "Yes" Or Me.rblEsignature.SelectedValue = "No"
            Dim eSigOption = QQDevDictionary_GetItem("Sign_Application_Using_eSignature")
            Dim hasSelectedEsigValue As Boolean = False
            If eSigOption IsNot Nothing OrElse eSigOption <> "" Then
                If eSigOption = "Yes" OrElse eSigOption = "No" Then
                    hasSelectedEsigValue = True
                End If
            End If
            If Me.rblEsignature.SelectedValue = "Yes" Or Me.rblEsignature.SelectedValue = "No" Then
                hasSelectedEsigValue = True
            End If
            'Updated 01/19/2021 for Task 52925 BB
            If hasSelectedEsigValue = False AndAlso Me.IsOnAppPage Then
                Dim accordList As List(Of VRAccordionTogglePair) = Me.MyAccordionList ' just to cache it
                Me.ValidationHelper.GroupName = "Signature Information" 'Added 01/21/2021 for Task 59269 BB
                Me.ValidationHelper.AddError(Me.rblEsignature, "Missing Signature selection", accordList)
            End If

            If Me.rblEsignature.SelectedValue = "Yes" Then
                Dim eSigEmailList As List(Of QuickQuote.CommonObjects.QuickQuoteEmail) = Nothing
                Dim eSigEmail = New QuickQuote.CommonObjects.QuickQuoteEmail
                If txtEsigEmail.Text <> "" Then
                    eSigEmail.Address = txtEsigEmail.Text
                    eSigEmailList = New List(Of QuickQuote.CommonObjects.QuickQuoteEmail)
                    eSigEmailList.Add(eSigEmail)
                End If
                Dim valItems = EsignatureValidator.ValidateEsignature(eSigEmailList, valArgs.ValidationType)
                If valItems.Any() Then
                    For Each v In valItems
                        Select Case v.FieldId
                            Case EsignatureValidator.EsignatureEmail
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtEsigEmail, v, myaccordClientId, paneIndex)
                            Case EsignatureValidator.EsignatureEmailIsEmpty
                                Me.ValidationHelper.Val_BindValidationItemToControl(Me.txtEsigEmail, v, myaccordClientId, paneIndex)
                        End Select
                    Next
                End If
            End If
            Populate()
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        'Updated 1/11/2022 for Bug 67521 MLW
        'If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly Then
        If Not Me.IsQuoteEndorsement AndAlso Not Me.IsQuoteReadOnly AndAlso Me.hasEsigOption Then
            If Me.Quote IsNot Nothing Then
                Dim esigOption As String = Me.rblEsignature.SelectedValue
                Dim email As String = ""
                If esigOption = "Yes" Then
                    email = txtEsigEmail.Text
                End If
                If esigOption <> "" Then
                    Dim esigRecordExists As String = QQDevDictionary_GetItem("eSignature_Record_Exists")
                    Dim eSigName As String = ""
                    If Me.Quote.Policyholder.Name IsNot Nothing Then
                        If Me.Quote.Policyholder.Name.TypeId <> "2" Then
                            If Me.Quote.Policyholder.Name.FirstName IsNot Nothing AndAlso Me.Quote.Policyholder.Name.LastName IsNot Nothing Then
                                eSigName = Me.Quote.Policyholder.Name.FirstName.ToUpper() & " " & Me.Quote.Policyholder.Name.LastName.ToUpper()
                            End If
                        Else
                            If Me.Quote.Policyholder.Name.CommercialName1 IsNot Nothing Then
                                eSigName = Me.Quote.Policyholder.Name.CommercialName1.ToUpper()
                            End If
                        End If
                    End If
                    If eSigName <> "" Then 'because most commercial lobs save this control before we get to the policyholder page where the control is/should be initially saved
                        QQDevDictionary_SetItem("Sign_Application_Using_eSignature", esigOption) 'otherwise yes option would be saved to the dev dictionary before they were shown an option on the policyholder screen & copy policyholder email to esig email would not work
                    End If
                    QQDevDictionary_SetItem("eSignature_Email", email.ToUpper())

                    If Me.Quote.Database_QuoteId IsNot Nothing AndAlso Me.Quote.Database_QuoteId <> "" Then
                        Dim esigInfo As String = ""
                        esigInfo &= """signatureTypeId"": 1, "
                        esigInfo &= """key"": """ & Me.Quote.Database_QuoteId & ""","
                        esigInfo &= """name"": """ & eSigName & ""","
                        esigInfo &= """email"": """ & email.ToUpper() & ""","
                        esigInfo &= """agencyCode"": """ & Me.Quote.AgencyCode & ""","
                        esigInfo &= """onHold"": true,"
                        esigInfo &= """template"": ""esig_first_email"","
                        esigInfo &= """templateJson"": ""{}"","

                        Dim zipCodeStandard As String = Me.Quote.Policyholder.Address.Zip
                        If zipCodeStandard IsNot Nothing AndAlso zipCodeStandard <> "" AndAlso zipCodeStandard.Length > 5 Then
                            zipCodeStandard = Left(zipCodeStandard, 5)
                        End If
                        QQDevDictionary_SetItem("eSignature_ZipCodeStandard", zipCodeStandard)
                        Dim apiURL As String = System.Configuration.ConfigurationManager.AppSettings("VR_Esignature_API_Domain") & System.Configuration.ConfigurationManager.AppSettings("VR_Esignature_SignatureAPIPath")

                        If hdnPriorEsigOption.Value = "Yes" AndAlso esigOption = "No" AndAlso (esigRecordExists IsNot Nothing OrElse esigRecordExists <> "") AndAlso esigRecordExists = "True" Then
                            apiURL &= Me.Quote.Database_QuoteId
                            esigInfo = """reason"": ""Quote eSig changed Yes to No"""
                            esigInfo = "{ " & esigInfo & " }"
                            Dim esigData1 = Encoding.UTF8.GetBytes(esigInfo)
                            Dim deleteResult = SendEsignatureRequest(apiURL, esigData1, "application/json", "DELETE")
                            If deleteResult IsNot Nothing AndAlso deleteResult <> "" Then
                                Dim deleteToken As JObject = Newtonsoft.Json.JsonConvert.DeserializeObject(Of JObject)(deleteResult)
                                Dim deleteSuccess As String = deleteToken("success").ToString()
                                If deleteSuccess = "True" Then
                                    QQDevDictionary_SetItem("eSignature_Record_Exists", "False")
                                End If
                            End If
                        ElseIf esigOption = "Yes" AndAlso
                               (esigRecordExists Is Nothing OrElse esigRecordExists = "" OrElse esigRecordExists = "False") AndAlso
                               email <> "" AndAlso
                               eSigName <> "" AndAlso
                               Not String.IsNullOrWhiteSpace(zipCodeStandard) Then
                            esigInfo &= """verifications"": [ { ""verificationTypeId"": 1, ""value"": """ & zipCodeStandard & """ }]"
                            esigInfo = "{ " & esigInfo & " }"
                            Dim esigData = Encoding.UTF8.GetBytes(esigInfo)
                            Dim postResult = SendEsignatureRequest(apiURL, esigData, "application/json", "POST")
                            If postResult IsNot Nothing AndAlso postResult <> "" Then
                                Dim postToken As JObject = Newtonsoft.Json.JsonConvert.DeserializeObject(Of JObject)(postResult)
                                Dim postSuccess As String = postToken("success").ToString()
                                If postSuccess = "True" Then
                                    QQDevDictionary_SetItem("eSignature_Record_Exists", "True")
                                    ''Added 11/22/2021 for bug 65863 MLW - logging what zip we send to eSig database using Leaf's API in order to track when it changes (testing in Patch - comment out for Sprints branch since not going to production)
                                    'QQDevDictionary_SetItem("eSignature_json_post_sent", esigInfo & " " & DateTime.Now)
                                End If
                            End If
                        ElseIf hdnPriorEsigOption.Value = esigOption Then
                            If hdnPriorEmailAddress.Value <> email OrElse
                               hdnPriorZipCodeStandard.Value <> zipCodeStandard OrElse
                               Not String.IsNullOrWhiteSpace(zipCodeStandard) Then
                                apiURL &= Me.Quote.Database_QuoteId
                                esigInfo &= """updateReason"": ""Quote eSig email changed"","
                                esigInfo &= """verifications"": [ { ""verificationTypeId"": 1, ""value"": """ & zipCodeStandard & """ }]"
                                esigInfo = "{ " & esigInfo & " }"
                                Dim esigPutData = Encoding.UTF8.GetBytes(esigInfo)
                                Dim putResult = SendEsignatureRequest(apiURL, esigPutData, "application/json", "PUT")
                                If putResult IsNot Nothing AndAlso putResult <> "" Then
                                    Dim putToken As JObject = Newtonsoft.Json.JsonConvert.DeserializeObject(Of JObject)(putResult)
                                    Dim putSuccess As String = putToken("success").ToString()
                                    If putSuccess = "True" Then
                                        QQDevDictionary_SetItem("eSignature_Record_Exists", "True")
                                        ''Added 11/22/2021 for bug 65863 MLW - logging what zip we send to eSig database using Leaf's API in order to track when it changes (testing in Patch - comment out for Sprints branch since not going to production)
                                        'QQDevDictionary_SetItem("eSignature_json_put_sent", esigInfo & " " & DateTime.Now)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
        Return True
    End Function

    Private Sub lnkBtnSave_Click(sender As Object, e As EventArgs) Handles lnkBtnSave.Click
        Me.Save_FireSaveEvent(True)
    End Sub

    Private Function SendEsignatureRequest(apiURL As String, jsonDataBytes As Byte(), contentType As String, method As String) As String
        Dim response As String = ""
        Dim request As WebRequest
        ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls12
        request = WebRequest.Create(apiURL)
        request.ContentLength = jsonDataBytes.Length
        request.ContentType = contentType
        request.Method = method
        Try
            Using requestStream = request.GetRequestStream
                requestStream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
                requestStream.Close()
                Using responseStream = request.GetResponse.GetResponseStream
                    Using reader As New StreamReader(responseStream)
                        response = reader.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            LogException(ex, "Esignature Exception Occurred")
        End Try
        Return response
    End Function

End Class