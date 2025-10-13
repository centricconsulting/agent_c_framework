Imports System.Runtime.CompilerServices
Imports IFM.VR.Web.ctlPageStartupScript

Module VRWebExtensions

    ''' <summary>
    ''' Returns the converted value in ViewState if it exists in ViewState and can be converted otherwise it returns the default value.
    ''' </summary>
    ''' <param name="ViewState"></param>
    ''' <param name="key"></param>
    ''' <param name="defaultIfNullOrInvalidType"></param>
    ''' <param name="dontBreakOnNull"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetInt32(ViewState As StateBag, key As String, Optional ByVal defaultIfNullOrInvalidType As Int32 = 0, Optional dontBreakOnNull As Boolean = False) As Int32
        If ViewState(key) IsNot Nothing Then
            Dim val As Int32
            If Int32.TryParse(ViewState(key), val) Then
                Return val
#If DEBUG Then
            Else
                'Could not convert
                If dontBreakOnNull = False Then
                    Debugger.Break()
                End If
#End If
            End If
#If DEBUG Then
        Else

            'ViewState key is null
            'If dontBreakOnNull = False Then
            '    Debugger.Break()
            'End If
#End If
        End If
        Return defaultIfNullOrInvalidType
    End Function

    ''' <summary>
    ''' Returns the converted value in ViewState if it exists in ViewState and can be converted otherwise it returns the default value.
    ''' </summary>
    ''' <param name="ViewState"></param>
    ''' <param name="key"></param>
    ''' <param name="defaultIfNullOrInvalidType"></param>
    ''' <param name="dontBreakOnNull"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDouble(ViewState As StateBag, key As String, Optional ByVal defaultIfNullOrInvalidType As Double = 0.0, Optional dontBreakOnNull As Boolean = False) As Double
        If ViewState(key) IsNot Nothing Then
            Dim val As Double
            If Double.TryParse(ViewState(key), val) Then
                Return val
#If DEBUG Then
            Else
                'Could not convert
                If dontBreakOnNull = False Then
                    Debugger.Break()
                End If
#End If
            End If
#If DEBUG Then
        Else
            'ViewState key is null
            If dontBreakOnNull = False Then
                Debugger.Break()
            End If
#End If
        End If
        Return defaultIfNullOrInvalidType
    End Function


    ''' <summary>
    ''' Returns the converted value in ViewState if it exists in ViewState and can be converted otherwise it returns the default value.
    ''' </summary>
    ''' <param name="ViewState"></param>
    ''' <param name="key"></param>
    ''' <param name="defaultIfNullOrInvalidType"></param>
    ''' <param name="dontBreakOnNull"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetString(ViewState As StateBag, key As String, Optional ByVal defaultIfNullOrInvalidType As String = "", Optional dontBreakOnNull As Boolean = False) As String
        If ViewState(key) IsNot Nothing Then
            Return ViewState(key).ToString()
#If DEBUG Then
        Else
            'ViewState key is null
            If dontBreakOnNull = False Then
                Debugger.Break()
            End If
#End If
        End If
        Return defaultIfNullOrInvalidType
    End Function

    ''' <summary>
    ''' Returns the converted value in ViewState if it exists in ViewState and can be converted otherwise it returns the default value.
    ''' </summary>
    ''' <param name="ViewState"></param>
    ''' <param name="key"></param>
    ''' <param name="defaultIfNullOrInvalidType"></param>
    ''' <param name="dontBreakOnNull"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetBool(ViewState As StateBag, key As String, Optional ByVal defaultIfNullOrInvalidType As Boolean = False, Optional dontBreakOnNull As Boolean = True) As Boolean
        If ViewState(key) IsNot Nothing Then
            Dim val As Boolean
            If Boolean.TryParse(ViewState(key), val) Then
                Return val
#If DEBUG Then
            Else
                'Could not convert
                If dontBreakOnNull = False Then
                    Debugger.Break()
                End If
#End If
            End If
#If DEBUG Then
        Else
            'ViewState key is null
            If dontBreakOnNull = False Then
                Debugger.Break()
            End If
#End If
        End If
        Return defaultIfNullOrInvalidType
    End Function

    ''' <summary>
    ''' Sets the property from the current selection of the dropdown. Autoconverts selections with value of zero to String.Empty
    ''' </summary>
    ''' <param name="dd"></param>
    ''' <param name="val"></param>
    <Extension()>
    Public Sub GetFromValue(ByRef dd As DropDownList, ByRef val As String)
        If dd.SelectedValue.Trim <> "0" Then
            val = dd.SelectedValue
        Else
            val = String.Empty
        End If

    End Sub


    ''' <summary>
    ''' A more reliable way to set the selection of a dropdownlist.
    ''' </summary>
    ''' <param name="dd"></param>
    ''' <param name="val"></param>
    <Extension()>
    Public Sub SetFromText(ByRef dd As DropDownList, val As String)
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromText(dd, val)
    End Sub

    ''' <summary>
    ''' A more reliable way to set the selection of a dropdownlist.
    ''' </summary>
    ''' <param name="dd"></param>
    ''' <param name="val"></param>
    <Extension()>
    Public Sub SetFromValue(ByRef dd As DropDownList, val As String)
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(dd, val)
    End Sub

    ''' <summary>
    ''' A more reliable way to set the selection of a dropdownlist. Sets a default if the sent value is null/empty/whitespace or '0'.
    ''' </summary>
    ''' <param name="dd"></param>
    ''' <param name="val"></param>
    ''' <param name="valIfEmpty"></param>
    <Extension()>
    Public Sub SetFromValue(ByRef dd As DropDownList, val As String, valIfEmpty As String)
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue(dd, If(String.IsNullOrWhiteSpace(val) Or val.Trim() = "0", valIfEmpty, val))
    End Sub

    ''' <summary>
    ''' Creates a new selection if it does not exist and selects it.
    ''' </summary>
    ''' <param name="dd">The DropDownList</param>
    ''' <param name="val">The value portion of the possibly new selection.</param>
    ''' <param name="ddText">The text portion of the possibly new selection.</param>
    <Extension()>
    Public Sub SetFromValue_Force(ByRef dd As DropDownList, val As String, ddText As String)
        IFM.VR.Web.Helpers.WebHelper_Personal.SetdropDownFromValue_ForceSeletion(dd, val, ddText)
    End Sub

    ''' <summary>
    ''' Uses the VrScript on the masterpage to create a js Mask.
    ''' </summary>
    ''' <param name="txtBox"></param>
    ''' <param name="maskText"></param>
    <Extension()>
    Public Sub CreateMask(txtBox As TextBox, maskText As String)
        Dim VrScript = DirectCast(txtBox.Page.Master, VelociRater).StartUpScriptManager
        VrScript.CreateTextboxMask(txtBox, maskText)
    End Sub

    ''' <summary>
    ''' Uses the VrScript on the masterpage to create a js Watermark.
    ''' </summary>
    ''' <param name="txtBox"></param>
    ''' <param name="waterMarkText"></param>
    <Extension()>
    Public Sub CreateWatermark(txtBox As TextBox, waterMarkText As String)
        Dim VrScript = DirectCast(txtBox.Page.Master, VelociRater).StartUpScriptManager
        VrScript.CreateTextboxWaterMark(txtBox, waterMarkText)
    End Sub

    ''' <summary>
    ''' Uses the VrScript on the masterpage to create client side autocomplete logic.
    ''' </summary>
    ''' <param name="txtBox"></param>
    ''' <param name="jsSourceVariableName"></param>
    <Extension()>
    Public Sub CreateAutoComplete(txtBox As TextBox, jsSourceVariableName As String)
        Dim VrScript = DirectCast(txtBox.Page.Master, VelociRater).StartUpScriptManager
        VrScript.AddScriptLine("$(""#" + txtBox.ClientID + """).autocomplete({ source: " + jsSourceVariableName + " });")
    End Sub

    ''' <summary>
    ''' Uses the VrScript on the masterpage to create a js formatter.
    ''' </summary>
    ''' <param name="txtBox"></param>
    ''' <param name="formatType"></param>
    ''' <param name="FireOn_eventType"></param>
    <Extension()>
    Public Sub CreateFormatter(txtBox As TextBox, formatType As FormatterType, FireOn_eventType As JsEventType)
        Dim VrScript = DirectCast(txtBox.Page.Master, VelociRater).StartUpScriptManager
        VrScript.CreateTextBoxFormatter(txtBox.ClientID, formatType, FireOn_eventType)
    End Sub

    <Extension()>
    Public Function ToIFMAddressString(Address As QuickQuote.CommonObjects.QuickQuoteAddress) As String
        Dim zip As String = Address.Zip
        If zip.Length > 5 Then
            zip = zip.Substring(0, 5)
        End If
        Return String.Format("{0} {1} {2} {3} {4} {5} {6}", Address.HouseNum, Address.StreetName, If(String.IsNullOrWhiteSpace(Address.ApartmentNumber) = False, "Apt# " + Address.ApartmentNumber, ""), Address.POBox, Address.City, Address.State, zip).Replace("  ", " ").Trim()
    End Function

    '''////////////////////////////////////////////////////////////////////////////////////////////
    ''' <summary>   Adds the CSS class to an HTML Control. </summary>
    '''
    ''' <remarks>   Chhaw, 08/08/2018. </remarks>
    '''
    ''' <param name="control">  The control. </param>
    ''' <param name="cssClass"> The CSS class to add. </param>
    '''////////////////////////////////////////////////////////////////////////////////////////////
    <Extension()>
    Sub AddCssClass(ByVal control As HtmlControl, ByVal cssClass As String)
        Dim classes As List(Of String)

        If Not String.IsNullOrWhiteSpace(control.Attributes("class")) Then
            classes = control.Attributes("class").Split({" "c}, StringSplitOptions.RemoveEmptyEntries).ToList()
            If Not classes.Contains(cssClass) Then classes.Add(cssClass)
        Else
            classes = New List(Of String) From {
                cssClass
            }
        End If

        control.Attributes("class") = String.Join(" ", classes.ToArray())
    End Sub

    '''////////////////////////////////////////////////////////////////////////////////////////////
    ''' <summary>   Removes the CSS class from an HTML Control. </summary>
    '''
    ''' <remarks>   Chhaw, 08/08/2018. </remarks>
    '''
    ''' <param name="control">  The control. </param>
    ''' <param name="cssClass"> The CSS class remove. </param>
    '''////////////////////////////////////////////////////////////////////////////////////////////
    <Extension()>
    Sub RemoveCssClass(ByVal control As HtmlControl, ByVal cssClass As String)
        Dim classes As List(Of String) = New List(Of String)()

        If Not String.IsNullOrWhiteSpace(control.Attributes("class")) Then
            classes = control.Attributes("class").Split({" "c}, StringSplitOptions.RemoveEmptyEntries).ToList()
        End If

        classes.Remove(cssClass)
        control.Attributes("class") = String.Join(" ", classes.ToArray())
    End Sub

    '''////////////////////////////////////////////////////////////////////////////////////////////
    ''' <summary>   Adds the CSS class to a Web Control. </summary>
    '''
    ''' <remarks>   Chhaw, 08/08/2018. </remarks>
    '''
    ''' <param name="control">  The control. </param>
    ''' <param name="cssClass"> The CSS class to add. </param>
    '''////////////////////////////////////////////////////////////////////////////////////////////
    <Extension()>
    Sub AddCssClass(ByVal control As WebControl, ByVal cssClass As String)
        Dim classes As List(Of String)

        If Not String.IsNullOrWhiteSpace(control.CssClass) Then
            classes = control.CssClass.Split({" "c}, StringSplitOptions.RemoveEmptyEntries).ToList()
            If Not classes.Contains(cssClass) Then classes.Add(cssClass)
        Else
            classes = New List(Of String) From {
                cssClass
            }
        End If

        control.CssClass = String.Join(" ", classes.ToArray())
    End Sub

    '''////////////////////////////////////////////////////////////////////////////////////////////
    ''' <summary>   Removes the CSS class from a Web Control. </summary>
    '''
    ''' <remarks>   Chhaw, 08/08/2018. </remarks>
    '''
    ''' <param name="control">  The control. </param>
    ''' <param name="cssClass"> The CSS class remove. </param>
    '''////////////////////////////////////////////////////////////////////////////////////////////
    <Extension()>
    Sub RemoveCssClass(ByVal control As WebControl, ByVal cssClass As String)
        Dim classes As List(Of String) = New List(Of String)()

        If Not String.IsNullOrWhiteSpace(control.CssClass) Then
            classes = control.CssClass.Split({" "c}, StringSplitOptions.RemoveEmptyEntries).ToList()
        End If

        classes.Remove(cssClass)
        control.CssClass = String.Join(" ", classes.ToArray())
    End Sub
End Module


