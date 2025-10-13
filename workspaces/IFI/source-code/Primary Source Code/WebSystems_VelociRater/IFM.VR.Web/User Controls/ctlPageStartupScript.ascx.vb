#If DEBUG Then
Imports System.Diagnostics
#End If

Imports IFM.PrimativeExtensions

Public Class ctlPageStartupScript
    Inherits System.Web.UI.UserControl

    Private scriptLines As New List(Of String)
    Private scriptLinesEND As New List(Of String)

    Private scriptLines_NoWait As New List(Of String)

    Private tabIndex_controls As New NameValueCollection
    Private tabIndex_shiftControls As New NameValueCollection

    ''' <summary>
    ''' These script lines will be executed on document.ready() event
    ''' </summary> 
    ''' <param name="script"></param>
    ''' <param name="placeatEndOfBlock"></param>
    ''' <remarks></remarks>
    Public Sub AddScriptLine(script As String, Optional placeatEndOfBlock As Boolean = False, Optional wrapInTryCatch As Boolean = True, Optional onlyAllowOnce As Boolean = False, Optional delayExecutionInMilliseconds As Int32 = 0)



        If wrapInTryCatch Then
#If DEBUG Then
            'script = "try{" + script + "} catch(err){console.warn(err.message);}" 'never catch when debugging so we know where errors occur
#Else
            script = "try{" + script + "} catch(err){}"
#End If
        End If

        If delayExecutionInMilliseconds > 0 Then
            script = String.Format("setTimeout(function(){{{0}}},{1})", script, delayExecutionInMilliseconds.ToString())
        End If

        Dim lineAlreadyExists As Boolean = False
        If onlyAllowOnce Then
            For Each l As String In Me.scriptLines
                If l.ToLower().Trim = script.ToLower().Trim() Then
                    lineAlreadyExists = True
                    Exit For
                End If
            Next
            If lineAlreadyExists = False Then
                For Each l As String In Me.scriptLinesEND
                    If l.ToLower().Trim = script.ToLower().Trim() Then
                        lineAlreadyExists = True
                        Exit For
                    End If
                Next
            End If
        End If

        If onlyAllowOnce And lineAlreadyExists Then
            Return
        End If

        If placeatEndOfBlock = False Then
            Me.scriptLines.Add(script)
        Else
            Me.scriptLinesEND.Add(script)
        End If

    End Sub

    ''' <summary>
    ''' Used to set a script dynamic variable that will be available before any other script logic runs on the page.
    ''' </summary>
    ''' <param name="script"></param>
    ''' <remarks></remarks>
    Public Sub AddVariableLine(script As String)
        ' make sure you can only add this line once
        Dim lineExists As Boolean = False
        For Each l As String In Me.scriptLines_NoWait
            If l.ToLower().Trim() = script.ToLower().Trim() Then
                lineExists = True
                Exit For
            End If
        Next
        If lineExists = False Then
            Me.scriptLines_NoWait.Add(script)
        End If
    End Sub

    ''' <summary>
    ''' Used to add controls to the Tab Indexing Collection
    ''' </summary>
    ''' <param name="script"></param>
    ''' <remarks></remarks>
    Public Sub AddTabIndexControl(controlID As String, tabToNextControlInCollection As Boolean)
        tabIndex_controls.Add(controlID, tabToNextControlInCollection)
    End Sub

    ''' <summary>
    ''' Used to add controls to the Tab Indexing Collection. Overloaded to allow for Shift Tab indexing.
    ''' </summary>
    ''' <param name="script"></param>
    ''' <remarks></remarks>
    Public Sub AddTabIndexControl(controlID As String, tabToNextControlInCollection As Boolean, shiftTabToPreviousControlInCollection As Boolean)
        tabIndex_controls.Add(controlID, tabToNextControlInCollection)
        tabIndex_shiftControls.Add(controlID, shiftTabToPreviousControlInCollection)
    End Sub

    ''' <summary>
    ''' Uses the Tab Indexing Collection created by the AddTabIndexControl Sub and adds Javascript/Jquery scripts to the page to force tab indexing between user control inputs.
    ''' </summary>
    ''' <param name="script"></param>
    ''' <remarks></remarks>
    Public Sub BindTabIndexControls()
        Dim count As Integer = 0

        If tabIndex_shiftControls IsNot Nothing Then
            Dim tabShiftIndexString As String = "var tabShiftIndexArr = ["

            For Each control In tabIndex_shiftControls.AllKeys
                'control = control.Replace("[", "[[").Replace("]", "]]") '.Replace("""", "'")
                control = control.Replace("""", "'")
                tabShiftIndexString += """" + control + ""","
                tabShiftIndexString += """" + tabIndex_shiftControls(control) + """"
                If tabIndex_shiftControls.Count = (count + 1) Then
                    tabShiftIndexString += "];"
                Else
                    tabShiftIndexString += ","
                End If
                count += 1
            Next
            AddVariableLine(tabShiftIndexString)
        End If

        count = 0
        If tabIndex_controls.Count > 0 Then
            Dim tabIndexString As String = "var tabIndexArr = ["
            For Each control In tabIndex_controls.AllKeys
                control = control.Replace("""", "'")
                tabIndexString += """" + control + ""","
                tabIndexString += """" + tabIndex_controls(control) + """"
                If tabIndex_controls.Count = (count + 1) Then
                    tabIndexString += "];"
                Else
                    tabIndexString += ","
                End If
                count += 1
            Next
            AddVariableLine(tabIndexString)

            For Each control In tabIndex_controls
                If control.IndexOf("[") > -1 Then
                    control = control.Replace("""", "'")
                    If tabIndex_shiftControls IsNot Nothing Then
                        AddVariableLine("$(""" + control + """).keydown(function(event){TabIndex_SetFocusToControl(event, """ + control + """, tabIndexArr, tabShiftIndexArr);});")
                    Else
                        AddVariableLine("$(""" + control + """).keydown(function(event){TabIndex_SetFocusToControl(event, """ + control + """, tabIndexArr);});")
                    End If
                Else
                    control = control.Replace("""", "'")
                    If tabIndex_shiftControls IsNot Nothing Then
                        AddVariableLine("$(""#" + control + """).keydown(function(event){TabIndex_SetFocusToControl(event, '" + control + "', tabIndexArr, tabShiftIndexArr);});")
                    Else
                        AddVariableLine("$(""#" + control + """).keydown(function(event){TabIndex_SetFocusToControl(event, '" + control + "', tabIndexArr);});")
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        ' these are used for things like static data used on the client side
        Dim varSB As New StringBuilder()
        For Each st As String In scriptLines_NoWait
            varSB.Append(st)
        Next
        ' add these to a non startup block - that way they will be ready before any script logic begins to execute
        IFM.VR.Web.Helpers.WebHelper_Personal.AddStartUpScript(varSB.ToString(), Response, ScriptManager.GetCurrent(Me.Page), Me.Page)

        Dim sb As New StringBuilder()
        For Each st As String In scriptLines
            sb.AppendLine(st)
        Next
        For Each st As String In scriptLinesEND
            sb.AppendLine(st)
        Next
        Dim script As String = "$(document).ready(function(){" + sb.ToString() + "});"
        IFM.VR.Web.Helpers.WebHelper_Personal.AddStartUpScript(script, Response, ScriptManager.GetCurrent(Me.Page), Me.Page)
    End Sub

    Public Sub CreateDatePicker(txtBoxClientId As String, DontAllowFutureDates As Boolean)
#If DEBUG Then
        If (String.IsNullOrWhiteSpace(txtBoxClientId)) Then
            Debug.WriteLine("client ID is null or empty")
            Debugger.Break()
        End If
#End If
        If DontAllowFutureDates Then
            Me.AddScriptLine("$(""#" + txtBoxClientId + """).datepicker({changeMonth: true,changeYear: true,maxDate: ""+0D"",showButtonPanel: true});")
        Else
            Me.AddScriptLine("$(""#" + txtBoxClientId + """).datepicker({changeMonth: true,changeYear: true, showButtonPanel: true});")
        End If

    End Sub

    Public Sub CreateDatePicker(txtBoxClientId As String, maxDaysInPast As UInteger, maxDaysInFuture As UInteger)
#If DEBUG Then
        If (String.IsNullOrWhiteSpace(txtBoxClientId)) Then
            Debug.WriteLine("client ID is null or empty")
            Debugger.Break()
        End If
#End If

        Me.AddScriptLine(String.Format("$(""#{0}"").datepicker({{changeMonth: true,changeYear: true,minDate: ""-{1}D"", maxDate: ""+{2}D"",showButtonPanel: true}});", txtBoxClientId, maxDaysInPast.ToString(), maxDaysInFuture.ToString()))
    End Sub

    Public Sub CreateDatePicker(txtBoxClientId As String, minDate As Date, maxDate As Date)
#If DEBUG Then
        If (String.IsNullOrWhiteSpace(txtBoxClientId)) Then
            Debug.WriteLine("client ID is null or empty")
            Debugger.Break()
        End If
        If minDate > maxDate Then
            Debug.WriteLine("Mindate is greater than maxdate.")
            Debugger.Break()
        End If
#End If
        Me.AddScriptLine(String.Format("$(""#{0}"").datepicker({{changeMonth: true,changeYear: true,minDate: new Date({1},{2},{3}), maxDate: new Date({4},{5},{6}),showButtonPanel: true}});", txtBoxClientId, minDate.Year, minDate.Month - 1, minDate.Day, maxDate.Year, maxDate.Month - 1, maxDate.Day))
    End Sub

    Public Sub CreateTextboxMask(textbox As TextBox, maskText As String)
#If DEBUG Then
        If textbox Is Nothing Then
            Debug.WriteLine("textbox object is null")
            Debugger.Break()
        End If
        If (String.IsNullOrWhiteSpace(maskText)) Then
            Debug.WriteLine("masktext is null or empty")
            Debugger.Break()
        End If
#End If
        Me.AddScriptLine("$(""#" + textbox.ClientID + """).mask(""" + maskText + """);")
    End Sub

    Public Sub CreateTextboxWaterMark(textbox As TextBox, waterMarkText As String)
#If DEBUG Then
        If textbox Is Nothing Then
            Debug.WriteLine("textbox object is null")
            Debugger.Break()
        End If
        If (String.IsNullOrWhiteSpace(waterMarkText)) Then
            Debug.WriteLine("watermark is null or empty")
            Debugger.Break()
        End If
#End If

        Me.AddScriptLine("$(""#" + textbox.ClientID + """).watermark(""" + waterMarkText + """);")
    End Sub

    Public Sub CreateAccordion(divClientId As String, hiddenField As HiddenField, defaultIndex As String, Optional showDisabled As Boolean = False)
        If hiddenField IsNot Nothing AndAlso String.IsNullOrWhiteSpace(defaultIndex) = False AndAlso String.IsNullOrWhiteSpace(hiddenField.Value) Then
            hiddenField.Value = defaultIndex
        End If
        If showDisabled Then
            'disabled
            Me.AddScriptLine("$(""#" + divClientId + """).accordion({ active: false, collapsible: false,icons: false});")
        Else
            If hiddenField IsNot Nothing AndAlso String.IsNullOrWhiteSpace(divClientId) = False Then
                ' this will remember its active value from postback to postback
                Me.AddScriptLine("$(""#" + divClientId + """).accordion({ heightStyle: ""content"", active: " + hiddenField.Value + ", collapsible: true, activate: function (event, ui) { $(""#" + hiddenField.ClientID + """).val($(""#" + divClientId + """).accordion('option','active'));  } });")
            Else
                ' just has a defaulted starting active value it does not remember its state from postback to postback
                If String.IsNullOrWhiteSpace(divClientId) = False Then
                    Me.AddScriptLine("$(""#" + divClientId + """).accordion({ heightStyle: ""content"", active: " + defaultIndex + ", collapsible: true  });")
                End If
            End If
        End If
    End Sub

    Public Sub StopEventPropagation(clientId As String, Optional greyOutForm As Boolean = True)
#If DEBUG Then
        If String.IsNullOrWhiteSpace(clientId) Then
            Debug.WriteLine("clientid is null or empty")
            Debugger.Break()
        End If
#End If
        If greyOutForm Then
            Me.AddScriptLine("$(""#" + clientId + """).on(""click"", function (e) { DisableFormOnSaveRemoves(); e.stopPropagation();});")
        Else
            Me.AddScriptLine("$(""#" + clientId + """).on(""click"", function (e) { e.stopPropagation();});")
        End If

    End Sub

    Public Sub CreateJSBinding(clientId As String, eventName As String, jsCode As String)
#If DEBUG Then
        If String.IsNullOrWhiteSpace(clientId) Then
            Debug.WriteLine("clientid is null or empty")
            Debugger.Break()
        End If
        If String.IsNullOrWhiteSpace(jsCode) Then
            Debug.WriteLine("jscode is null or empty")
            Debugger.Break()
        End If
#End If

        If eventName.ToLower().StartsWith("on") Then ' jquery doesn't want the 'on' part of the event name
            eventName = eventName.Remove(0, 2)
        End If
        Me.AddScriptLine("$(""#" + clientId + """).on(""" + eventName + """, function (e) {" + jsCode + "});")
    End Sub

    Private Sub _CreateJSBinding(selector As String, eventType As JsEventType, jsCode As String)
        ' if you are using this method directly you are probably doing it wrong
        ' use the overloads of this method
        Dim eventName As String = ""
        Select Case eventType
            Case JsEventType.onblur
                eventName = "blur"
            Case JsEventType.onclick
                eventName = "click"
            Case JsEventType.onfocus
                eventName = "focus"
            Case JsEventType.onkeyup
                eventName = "keyup"
            Case JsEventType.onkeydown
                eventName = "keydown"
            Case JsEventType.onchange
                eventName = "change"
        End Select

#If DEBUG Then
        If String.IsNullOrWhiteSpace(selector) Then
            Debug.WriteLine("Selector is null or empty")
            Debugger.Break()
        End If
        If String.IsNullOrWhiteSpace(eventName) Then
            Debug.WriteLine("eventName is null or empty")
            Debugger.Break()
        End If
        If String.IsNullOrWhiteSpace(jsCode) Then
            Debug.WriteLine("jscode is null or empty")
            Debugger.Break()
        End If
#End If

        ' notice selector can be anything could be text like ('#clientid') or could be ('#' + js_variable) or could be (':not div')
        'Me.AddScriptLine("$(" + selector + ").bind(""" + eventName + """, function (e) {" + jsCode + "});")
        Me.AddScriptLine("$(" + selector + ").on(""" + eventName + """, function (e) {" + jsCode + "});")
    End Sub

    ''' <summary>
    ''' Binds some javascript to an event.
    ''' </summary>
    ''' <param name="cntrl"></param>
    ''' <param name="eventType"></param>
    ''' <param name="jsCode"></param>
    ''' <param name="autoRunatStartUp">Often you want to execute this event code at startup of the page. This allows that without another call just remember not to use event scoped variables like 'this'.</param>
    Public Sub CreateJSBinding(cntrl As Control, eventType As JsEventType, jsCode As String, Optional autoRunatStartUp As Boolean = False)
        _CreateJSBinding("""#" + cntrl.ClientID + """", eventType, jsCode)
        If autoRunatStartUp Then
            Me.AddScriptLine(jsCode)
        End If
    End Sub


    ''' <summary>
    ''' Binds some javascript to an event.
    ''' </summary>
    ''' <param name="cntrl"></param>
    ''' <param name="eventType"></param>
    ''' <param name="jsCode"></param>
    ''' <param name="autoRunatStartUp">Often you want to execute this event code at startup of the page. This allows that without another call just remember not to use event scoped variables like 'this'.</param>
    Public Sub CreateJSBinding(cntrl As Control(), eventType As JsEventType, jsCode As String, Optional autoRunatStartUp As Boolean = False)
        For Each c In cntrl
            _CreateJSBinding("""#" + c.ClientID + """", eventType, jsCode)
            If autoRunatStartUp Then
                Me.AddScriptLine(jsCode)
            End If
        Next

    End Sub



    ''' <summary>
    ''' Binds some javascript to an event.
    ''' </summary>
    ''' <param name="cntrliD"></param>
    ''' <param name="eventType"></param>
    ''' <param name="jsCode"></param>
    ''' <param name="autoRunatStartUp">Often you want to execute this event code at startup of the page. This allows that without another call just remember not to use event scoped variables like 'this'.</param>
    Public Sub CreateJSBinding(cntrliD As String, eventType As JsEventType, jsCode As String, Optional autoRunatStartUp As Boolean = False)
        _CreateJSBinding("""#" + cntrliD + """", eventType, jsCode)
        If autoRunatStartUp Then
            Me.AddScriptLine(jsCode)
        End If
    End Sub

    ''' <summary>
    ''' Binds some javascript to an event. Uses a constom slector that is provided.
    ''' </summary>
    ''' <param name="selector_text"></param>
    ''' <param name="eventType"></param>
    ''' <param name="jsCode"></param>
    ''' <param name="autoRunatStartUp">Often you want to execute this event code at startup of the page. This allows that without another call just remember not to use event scoped variables like 'this'.</param>
    Public Sub CreateJSBinding_CustomSelector(selector_text As String, eventType As JsEventType, jsCode As String, Optional autoRunatStartUp As Boolean = False)
        _CreateJSBinding(selector_text, eventType, jsCode)
        If autoRunatStartUp Then
            Me.AddScriptLine(jsCode)
        End If
    End Sub

    '    ''' <summary>
    '    ''' This binds a confirm dialog to the control on the provided event with the provided message.
    '    ''' </summary>
    '    ''' <param name="clientId"></param>
    '    ''' <param name="eventName"></param>
    '    ''' <param name="messageText"></param>
    '    ''' <remarks></remarks>
    '    Public Sub CreateConfirmDialog(clientId As String, eventName As String, messageText As String)
    '#If DEBUG Then
    '        If String.IsNullOrWhiteSpace(clientId) Then
    '            Debug.WriteLine("clientID is null or empty")
    '            Debugger.Break()
    '        End If
    '        If String.IsNullOrWhiteSpace(eventName) Then
    '            Debug.WriteLine("eventName is null or empty")
    '            Debugger.Break()
    '        End If
    '        If String.IsNullOrWhiteSpace(messageText) Then
    '            Debug.WriteLine("messageText is null or empty")
    '            Debugger.Break()
    '        End If
    '#End If

    '        Dim js As String = "e.stopPropagation(); var confirmed = confirm(""" + messageText + """); if(confirmed){DisableFormOnSaveRemoves();} return confirmed;"
    '        Me.CreateJSBinding(clientId, eventName, js)
    '    End Sub

    Public Sub CreateConfirmDialog(clientId As String, messageText As String, Optional eventType As JsEventType = ctlPageStartupScript.JsEventType.onclick)
        messageText = Replace(messageText, "\", "\\")
        messageText = Replace(messageText, "<br>", "\n")
        messageText = Replace(messageText, vbCrLf, "\n")
#If DEBUG Then
        If String.IsNullOrWhiteSpace(clientId) Then
            Debug.WriteLine("clientID is null or empty")
            Debugger.Break()
        End If
        If String.IsNullOrWhiteSpace(messageText) Then
            Debug.WriteLine("messageText is null or empty")
            Debugger.Break()
        End If
#End If

        Dim js As String = "e.stopPropagation(); var confirmed = confirm(""" + messageText + """); if(confirmed){DisableFormOnSaveRemoves();} return confirmed;"
        Me.CreateJSBinding(clientId, eventType, js, False)
    End Sub

    Public Sub ShowAlert(messageText As String, Optional eventType As JsEventType = ctlPageStartupScript.JsEventType.onclick)
        messageText = Replace(messageText, "\", "\\")
        messageText = Replace(messageText, "<br>", "\n")
        messageText = Replace(messageText, vbCrLf, "\n")
        Me.AddScriptLine($"alert('{messageText}');", True, True, True, 300)
    End Sub

    Public Enum FormatterType
        NumericNoCommas = 1
        NumericWithCommas = 10
        ZipCode = 2
        Currency = 3
        AlphabeticOnly = 4
        PositiveNumberNoCommas = 5
        DateFormat = 6
        AlphaNumeric = 7
        PositiveNumberWithCommas = 8
        PositiveWholeNumberWithCommas = 9
        CurrencyNoCents = 10
        RoundToNearest100 = 11 'Added 12/28/17 for HOM Upgrade MLW
        'RoundToNearest1000 = 12 'Added 1/3/18 for HOM Upgrade MLW
    End Enum
    Public Enum JsEventType
        onkeyup = 1
        onblur = 2
        onfocus = 3
        onclick = 4
        onchange = 5
        onkeydown = 6
    End Enum

    Public Sub CreateTextBoxFormatter(clientId As String, formatType As FormatterType, FireOn_eventType As JsEventType)
#If DEBUG Then
        If String.IsNullOrWhiteSpace(clientId) Then
            Debug.WriteLine("clientID is null or empty")
            Debugger.Break()
        End If
#End If
        ' ignore many of these system type keys like 'ATL''CTRL''BACKSPACE' ect
        Dim js = "if (ifm.vr.ui.AllowFormatter(e)){"

        Dim duplicateOnBlur As Boolean = False
        Select Case formatType
            Case FormatterType.NumericNoCommas
                js += "$(""#" + clientId + """).val(ifm.vr.stringFormating.asNumberNoCommas($(""#" + clientId + """).val()));"
                duplicateOnBlur = True
            Case FormatterType.NumericWithCommas
                js += "$(""#" + clientId + """).val(ifm.vr.stringFormating.asNumberWithCommas($(""#" + clientId + """).val()));"
                duplicateOnBlur = True
            Case FormatterType.ZipCode
                'js += "$(""#" + clientId + """).val(formatPostalcode($(""#" + clientId + """).val()));"
                js += "$(""#" + clientId + """).val(ifm.vr.stringFormating.asPostalCode($(""#" + clientId + """).val()));"
                duplicateOnBlur = True
            Case FormatterType.Currency
                js += "$(""#" + clientId + """).val(ifm.vr.stringFormating.asCurrency($(""#" + clientId + """).val()));"
                duplicateOnBlur = True
            Case FormatterType.AlphabeticOnly
                'FireOn_eventType = JsEventType.onkeyup 'must be on key up
                js += "$(""#" + clientId + """).val(ifm.vr.stringFormating.asAlphabeticOnly($(""#" + clientId + """).val()));"
            Case FormatterType.PositiveNumberNoCommas
                js += "$(""#" + clientId + """).val(ifm.vr.stringFormating.asPositiveNumberNoCommas($(""#" + clientId + """).val()));"
            Case FormatterType.DateFormat
                js += "$(""#" + clientId + """).val(ifm.vr.stringFormating.asDate($(""#" + clientId + """).val()));"
                duplicateOnBlur = True
            Case FormatterType.AlphaNumeric
                'FireOn_eventType = JsEventType.onkeyup 'must be on key up
                js += "$(""#" + clientId + """).val(ifm.vr.stringFormating.asAlphabeticNumeric($(""#" + clientId + """).val()));"
            Case FormatterType.PositiveNumberWithCommas
                js += "$(""#" + clientId + """).val(ifm.vr.stringFormating.asPositiveNumberWithCommas($(""#" + clientId + """).val()));"
            Case FormatterType.PositiveWholeNumberWithCommas
                js += "$(""#" + clientId + """).val(ifm.vr.stringFormating.asPositiveWholeNumberWithCommas($(""#" + clientId + """).val()));"
            Case FormatterType.CurrencyNoCents
                js += "$(""#" + clientId + """).val(ifm.vr.stringFormating.asCurrencyNoCents($(""#" + clientId + """).val()));"
            Case FormatterType.RoundToNearest100
                'Added 12/28/17 for HOM Upgrade MLW
                js += "$(""#" + clientId + """).val(ifm.vr.stringFormating.asRoundToNearest100($(""#" + clientId + """).val()));"

        End Select
        js += "}"

        Me.CreateJSBinding(clientId, FireOn_eventType, js, False)
        If duplicateOnBlur AndAlso FireOn_eventType <> JsEventType.onblur Then
            Me.CreateJSBinding(clientId, JsEventType.onblur, js, False)
        End If
    End Sub

    Public Sub CreateTextBoxFormatter(cntrl As Control, formatType As FormatterType, FireOn_eventType As JsEventType)
        CreateTextBoxFormatter(cntrl.ClientID, formatType, FireOn_eventType)
    End Sub

    ''' <summary>
    ''' Useful for things like autopostback controls or buttons like rate so the user can't change the screen while the quote is rating.
    ''' </summary>
    ''' <param name="ctl"></param>
    ''' <param name="eventName"></param>
    Public Sub LockFormOnEvent(ctl As Control, eventName As ctlPageStartupScript.JsEventType)
        CreateJSBinding(ctl, eventName, "DisableFormOnSaveRemoves();")
    End Sub

    Public Sub CreateInfomationIcon(cntrl As Control, hoverText As String)
#If DEBUG Then
        If String.IsNullOrWhiteSpace(ClientID) Then
            Debug.WriteLine("clientID is null or empty")
            Debugger.Break()
        End If
        If String.IsNullOrWhiteSpace(hoverText) Then
            Debug.WriteLine("hoverText is null or empty")
            Debugger.Break()
        End If
#End If
        'hoverText = HttpUtility.HtmlEncode(hoverText)
        Me.AddScriptLine("$(""#" + cntrl.ClientID + """).after('<img src=""images/infoIcon.png"" style=""width: 15px;height:15px;"" title=""" + hoverText + """ />');")
    End Sub

    ''' <summary>
    ''' Creates a popup by using an existing divID.
    ''' </summary>
    ''' <param name="divId"></param>
    ''' <param name="title"></param>
    ''' <param name="body_html"></param>
    ''' <param name="width"></param>
    ''' <param name="height"></param>
    ''' <param name="openOnPageLoad"></param>
    ''' <param name="addCloseButtonInContents"></param>
    ''' <param name="showHeaderCloseButton"></param>
    ''' <returns>The id of the div used to create the popup.</returns>
    Public Function CreatePopUpWindow_jQueryUi_Dialog(divId As String, title As String, width As Int32, height As Int32, openOnPageLoad As Boolean, addCloseButtonInContents As Boolean, showHeaderCloseButton As Boolean, clientIDOfControlThatOpensPopup As String, Optional clientIdOfHiddenFieldThatHoldsValueToMakeItOnlyShowOnce As String = Nothing) As String
        CreatePopUpWindow_jQueryUi_Dialog(divId, False, title, String.Empty, width, height, openOnPageLoad, addCloseButtonInContents, Not showHeaderCloseButton, clientIDOfControlThatOpensPopup, clientIdOfHiddenFieldThatHoldsValueToMakeItOnlyShowOnce)
        Return divId
    End Function

    ''' <summary>
    ''' Creates a popup by creating a new div.
    ''' </summary>
    ''' <param name="title"></param>
    ''' <param name="body_html"></param>
    ''' <param name="width"></param>
    ''' <param name="height"></param>
    ''' <param name="openOnPageLoad"></param>
    ''' <param name="addCloseButtonInContents"></param>
    ''' <param name="showHeaderCloseButton"></param>
    ''' <returns> The id of the newly created div used for the popup.</returns>
    Public Function CreatePopUpWindow_jQueryUi_Dialog_CreatesDiv(title As String, body_html As String, width As Int32, height As Int32, openOnPageLoad As Boolean, addCloseButtonInContents As Boolean, showHeaderCloseButton As Boolean, clientIDOfControlThatOpensPopup As String, Optional clientIdOfHiddenFieldThatHoldsValueToMakeItOnlyShowOnce As String = Nothing, Optional listOfIdsToNotUseForDiv As List(Of String) = Nothing) As String
        Dim rnd As New Random()
        Dim divId As String = "div_Popup_" + rnd.Next(11000, 999999999).ToString()
        Dim createNewID As Boolean = False
        CreatePopUpWindow_jQueryUi_Dialog(divId, True, title, body_html, width, height, openOnPageLoad, addCloseButtonInContents, Not showHeaderCloseButton, clientIDOfControlThatOpensPopup, clientIdOfHiddenFieldThatHoldsValueToMakeItOnlyShowOnce)
        Return divId
    End Function

    ''' <summary>
    ''' Creates a popup by creating a new div.
    ''' </summary>
    ''' <param name="title"></param>
    ''' <param name="body_html"></param>
    ''' <param name="width"></param>
    ''' <param name="height"></param>
    ''' <param name="openOnPageLoad"></param>
    ''' <param name="addCloseButtonInContents"></param>
    ''' <param name="showHeaderCloseButton"></param>
    ''' <returns> The id of the newly created div used for the popup.</returns>
    Public Function CreatePopUpWindow_jQueryUi_Dialog_CreatesUniqueDiv(title As String, body_html As String, width As Int32, height As Int32, openOnPageLoad As Boolean, addCloseButtonInContents As Boolean, showHeaderCloseButton As Boolean, clientIDOfControlThatOpensPopup As String, Optional clientIdOfHiddenFieldThatHoldsValueToMakeItOnlyShowOnce As String = Nothing, Optional listOfIdsToNotUseForDiv As List(Of String) = Nothing, Optional doIncrementId As Boolean = False) As String
        Dim rnd As New Random()
        Dim divIdInt As Integer = rnd.Next(11000, 999999999)
        Dim createNewID As Boolean = False
        If doIncrementId = True Then
            divIdInt += 1 'For some reason the random number generator would "randomly" pick the same number upon multiple calls in testing creating an infinite loop below. Added this to make sure the number would at least change between calls.
        End If
        Dim divId As String = "div_Popup_" & divIdInt.ToString()

        If listOfIdsToNotUseForDiv IsNot Nothing Then
            For Each id As String In listOfIdsToNotUseForDiv
                If String.Equals(id, divId, StringComparison.OrdinalIgnoreCase) Then
                    createNewID = True
                End If
            Next
        End If

        If createNewID = True Then
            divId = CreatePopUpWindow_jQueryUi_Dialog_CreatesUniqueDiv(title, body_html, width, height, openOnPageLoad, addCloseButtonInContents, showHeaderCloseButton, clientIDOfControlThatOpensPopup, clientIdOfHiddenFieldThatHoldsValueToMakeItOnlyShowOnce, listOfIdsToNotUseForDiv, True)
        Else
            CreatePopUpWindow_jQueryUi_Dialog(divId, True, title, body_html, width, height, openOnPageLoad, addCloseButtonInContents, Not showHeaderCloseButton, clientIDOfControlThatOpensPopup, clientIdOfHiddenFieldThatHoldsValueToMakeItOnlyShowOnce)
        End If

        Return divId
    End Function


    Private Sub CreatePopUpWindow_jQueryUi_Dialog(divID As String, createDiv As Boolean, title As String, body_html As String, width As Int32, height As Int32, openOnPageLoad As Boolean, showOkButton As Boolean, hideHeaderCloseButton As Boolean, clientIDOfControlThatOpensPopup As String, clientIdofHiddenField As String)
        If body_html.Contains("""") Then
            body_html = body_html.Replace("""", "&quot;")
        End If

        Dim js As New StringBuilder()
        If createDiv Then
            'creating a new div then injecting html
            js.Append("$('form').append('<div id=""" + divID + """><div>")
            js.Append(body_html)
            If showOkButton Then
                js.Append("<div style=""text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;"">")
                js.Append("<input type=""button"" class=""StandardButton"" onclick=""$(&quot;#" + divID + "&quot;).dialog(&quot;close&quot;);"" value=""OK"" />")
                js.Append("</div>")
            End If

            js.Append("</div></div>');")
        Else
            If showOkButton Then
                ' a div with content already exists
                js.Append("$('#" + divID + "').append('<div>")
                js.Append("<div style=""text-align: center; margin-left: auto; margin-right: auto; margin-top: 20px; margin-bottom: 20px;"">")
                js.Append("<input type=""button"" class=""StandardButton"" onclick=""$(&quot;#" + divID + "&quot;).dialog(&quot;close&quot;);"" value=""OK"" />")
                js.Append("</div>")
                js.Append("</div>');")
            End If
        End If


        js.Append("$(""#" + divID + """).dialog({")
        js.Append("title: """ + title + """,")
        If openOnPageLoad = False Then
            js.Append("autoOpen: false, ")
        End If
        js.Append("width:  " + width.ToString() + ",")
        js.Append("height: " + height.ToString() + ",")
        If hideHeaderCloseButton Then
            js.Append("dialogClass: ""no-close"" ,")
        End If
        'js.Append("draggable: true,")
        'js.Append("modal: true")
        'js.Append(",open: function (type, data) { $(this).parent().appendTo(""form:first""); }")

        js.Append("});")

        'js.Append("$(""#" + divID + """).dialog('open');") 'debug

        Me.AddScriptLine(js.ToString())
        If Not String.IsNullOrWhiteSpace(clientIDOfControlThatOpensPopup.Trim()) Then
            If String.IsNullOrWhiteSpace(clientIdofHiddenField) Then
                Me.CreateJSBinding(clientIDOfControlThatOpensPopup, JsEventType.onclick, "$('#" + divID + "').dialog('open');")
            Else
                Me.CreateJSBinding(clientIDOfControlThatOpensPopup, JsEventType.onclick, "if($('#" + clientIdofHiddenField + "').val() == ''){  $('#" + divID + "').dialog('open'); $('#" + clientIdofHiddenField + "').val('opened');}")
            End If

        End If
    End Sub


    ''' <summary>
    ''' Use this popup creator when what you want to popup is a form. If it is informational only you can use CreatePopUpWindow_jQueryUi_Dialog
    ''' Note: You can only create this once per page as it always uses the same div for the popup.
    ''' </summary>
    ''' <param name="divId"></param>
    ''' <param name="title"></param>
    ''' <param name="width"></param>
    ''' <param name="height"></param>
    ''' <param name="modal"></param>
    ''' <param name="autoOpen"></param>
    ''' <param name="hasHeaderCloseButton"></param>
    ''' <param name="openOnClickElementId"></param>
    ''' <returns></returns>
    Public Function CreatePopupForm(divId As String, title As String, width As Int32, height As Int32, modal As Boolean, autoOpen As Boolean, hasHeaderCloseButton As Boolean, focusElementID As String, openOnClickElementId As String) As String
        Dim rnd As New Random()
        Dim divRnd As String = String.Format("divPopup_{0}", rnd.Next(10000, 999999999))
        Dim divContents As String = String.Format("divPopup_{0}", rnd.Next(10000, 999999999))
        Dim js As New StringBuilder()
        'create wrapper then append a markup div to it
        js.Append(String.Format("<div id=""{2}"" style=""width: {0}px; height: {1}px;"" class=""ui-dialog ui-widget ui-widget-content ui-corner-all ui-front"">", width, height, divRnd))
        js.Append("<div class=""ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix"">")
        js.Append(title)
        If hasHeaderCloseButton Then
            js.Append("<button title=""Close"" onclick=""$(this).parent.parent.hide();"" class=""ui-button ui-widget ui-state-default ui-corner-all ui-button-icon-only ui-dialog-titlebar-close ui-state-focus"" role=""button"" type=""button""><span class=""ui-button-icon-primary ui-icon ui-icon-closethick""></span><span class=""ui-button-text"">Close</span></button>")
        End If
        js.Append("</div>")
        js.Append(String.Format("<div id=""{0}"" class=""ui-dialog-content ui-widget-content"" style=""height: 90%;""></div>", divContents))
        js.Append("</div>")


        ' should replace divASP_Popups with a dynamically created div so you could have more than one of these created per screen
        ' you could just do something like "$('#divASP_Popups').after(<div id='createrandomId' style='position:fixed;'>'{0}'</div>);", js.ToString()

        Dim jsFunctionName As String = "popUpOpener_" + rnd.Next(1000, 9999999).ToString()
        Dim jsFunction As New StringBuilder()
        jsFunction.Append(String.Format("function {0} () {{", jsFunctionName))

        jsFunction.Append(String.Format("$('#divASP_Popups').append('{0}');", js.ToString())) ' puts all this new html into the existing div on the screen
        jsFunction.Append(String.Format("$('#{0}').appendTo($('#{1}'));", divId, divContents)) ' puts the sent div into the Contents area of this newly created div
        If modal Then
            jsFunction.Append("DisableFormOnSaveRemoves();") 'disable the screen behind the popup
        End If

        jsFunction.Append(String.Format("$('#divASP_Popups').css('left',(($(window).width()/2) - {0}).toString() + 'px');", width / 2)) ' center the popup
        If String.IsNullOrWhiteSpace(focusElementID) = False Then
            jsFunction.Append(String.Format("setTimeout(""$('#{0}').focus()"", 300);", focusElementID)) 'set focus after popup finishes loading
        End If

        jsFunction.Append("$('#divASP_Popups').fadeIn();")

        jsFunction.Append("}")

        AddScriptLine(jsFunction.ToString())
        If autoOpen Then
            AddScriptLine(jsFunctionName + "();")
#If DEBUG Then
        Else
            'If String.Format(openOnClickElementId) Then
            'CAH 3/27/2018 This was failing because it was returning a string, not a bool result.  I think below is the intended functionality
            'CAH 3/27/2018 I think this may be in DEBUG to bypass the error in other servers instead of fixing the issue.
            'If String.IsNullOrEmpty(openOnClickElementId) OrElse openOnClickElementId.GetType IsNot GetType(String) Then
            '    Debugger.Break() ' since this is not an autoopen you must have something to open it - I suppose it is possible that is defined somewhere else
            'End If
#End If
        End If

        'bind to element event to open the popup
        If String.IsNullOrWhiteSpace(openOnClickElementId) = False Then
            Me.CreateJSBinding(openOnClickElementId, "click", jsFunctionName + "();")
        End If

        'adjust center on window resize
        Me.AddScriptLine(String.Format("$(window).resize(function(){{$('#divASP_Popups').css('left',(($(window).width()/2) - {0}).toString() + 'px');}});", width / 2)) ' center the popup

        Return divRnd
    End Function

    Public Sub CreateJSStringArrayFromList(list As IEnumerable(Of String), variableName As String, Optional toLower As Boolean = False)
        If list.IsLoaded() Then
            Dim js As New StringBuilder()
            js.Append("var {0} = new Array(".FormatIFM(variableName))
            For x As Int16 = 0 To list.Count - 1 Step 1
                If toLower Then
                    js.Append("'{0}'".FormatIFM(list(x).ToLower()))
                Else
                    js.Append("'{0}'".FormatIFM(list(x)))
                End If
                If x < list.Count - 1 Then
                    js.Append(",")
                End If
            Next
            js.Append(");")
            Me.AddVariableLine(js.ToString())

        End If
    End Sub

    ''' <summary>
    ''' Lets you create a textbox that behaves like it is disabled by ignoring keystrokes and having a color of grey. You only use this if you have a textbox that can not be edited directly by users but does get edited by script. (Note: ASP will not accept text from disabled textboxes so that is why this might be needed on a very limited basis.)
    ''' </summary>
    ''' <param name="txtBox"></param>
    Public Sub CreatePseudoDisabledTextBox(txtBox As TextBox)
        Me.CreateJSBinding(txtBox.ClientID, "keydown", "return false;")
        Me.AddScriptLine("$('#" + txtBox.ClientID + "').css('color','grey');")
    End Sub

    Public Sub FakeDisableSingleElement(cntrl As Control)
        Me.AddScriptLine("$(document).ready(function () {ifm.vr.ui.SingleElementDisable(['" + cntrl.ClientID + "']);});")
    End Sub
    Public Sub FakeDisabledSingleElement_ReEnable(cntrl As Control)
        Me.AddScriptLine("$(document).ready(function () {ifm.vr.ui.SingleElementEnable(['" + cntrl.ClientID + "']);});")
    End Sub
    Public Sub FakeDisableSingleElement(cntrl As Control, Optional jsArrayExceptionList As String = "[]")
        Me.AddScriptLine("$(document).ready(function () {ifm.vr.ui.SingleContainerContentDisable('" + cntrl.ClientID + "', '" + jsArrayExceptionList + "');});")
    End Sub

    ''' <summary>
    ''' Scroll to a page control and offset that scroll
    ''' </summary>
    ''' <param name="cntrl">Control; without clientId</param>
    ''' <param name="offset">Positive goes up; Negative goes down</param>
    ''' <param name="currentPage">Me.Page for disabling scroll positioning by .Net</param>
    Public Sub ScrollToWithOffset(cntrl As Control, offset As Integer, ByRef currentPage As Page)
        currentPage.MaintainScrollPositionOnPostBack = False
        Dim method = String.Format("ifm.vr.ui.ScrollToWithOffset(""{0}"", {1})", cntrl.ClientID, offset)
        AddScriptLine(method, True, True, True, 500)
    End Sub

    ''' <summary>
    ''' Scroll to a page control with a string id and offset that scroll
    ''' </summary>
    ''' <param name="cntrl">ID as a String</param>
    ''' <param name="offset">Positive goes up; Negative goes down</param>
    ''' <param name="currentPage">Me.Page for disabling scroll positioning by .Net</param>
    Public Sub ScrollToWithOffset(cntrl As String, offset As Integer, ByRef currentPage As Page)
        currentPage.MaintainScrollPositionOnPostBack = False
        Dim method = String.Format("ifm.vr.ui.ScrollToWithOffset(""{0}"", {1})", cntrl, offset)
        AddScriptLine(method, True, True, True, 500)
    End Sub

    ''' <summary>
    ''' Scroll to a page control found with a jquery selector and offset that scroll
    ''' </summary>
    ''' <param name="cntrl">ID as a jquery selector</param>
    ''' <param name="offset">Positive goes up; Negative goes down</param>
    ''' <param name="currentPage">Me.Page for disabling scroll positioning by .Net</param>
    Public Sub ScrollToWithOffsetJQuerySelector(cntrl As String, offset As Integer, ByRef currentPage As Page)
        currentPage.MaintainScrollPositionOnPostBack = False
        Dim method = String.Format("ifm.vr.ui.ScrollToWithOffset({0}, {1})", cntrl, offset)
        AddScriptLine(method, True, True, True, 500)
    End Sub

End Class