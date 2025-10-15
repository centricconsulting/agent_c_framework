Imports IFM.VR.Common.Helpers.CAP
Imports IFM.VR.Common.Helpers.MultiState.General
Imports QuickQuote.CommonMethods
Public Class ctl_vehicle_item
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()
    End Sub

    Public Overrides Sub AddScriptWhenRendered()
    End Sub

    Public Overrides Sub LoadStaticData()
    End Sub

    Public Overrides Sub Populate()
        'genVehicleTable(Me.Quote.Vehicles(0))
        Me.rptVehicles.DataSource = Me.Quote.Vehicles
        Me.rptVehicles.DataBind()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Private Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.UseRatedQuoteImage = True
    End Sub

    Public Overrides Function Save() As Boolean
    End Function

    Public Function GetGaragingAddressString(Address As QuickQuote.CommonObjects.QuickQuoteGaragingAddress) As String
        Dim garagingAddressString As New StringBuilder("Garaging Address: ")
        Dim spacer As String = " "

        '07/10/2017 According to Jenifer Davis Only the 4 diamond requireds are necessary
        '  and not the entire address.

        '2547 E Pine Ave Indianapolis IN 46219 Marion

        'If (ifStringHasValue(Address.Address.HouseNum)) Then
        '    garagingAddressString.Append(Address.Address.HouseNum + spacer)
        'End If
        'If (ifStringHasValue(Address.Address.StreetName)) Then
        '    garagingAddressString.Append(Address.Address.StreetName + spacer)
        'End If
        'If (ifStringHasValue(Address.Address.POBox)) Then
        '    garagingAddressString.Append(Address.Address.POBox + spacer)
        'End If
        garagingAddressString.Append(Address.Address.City + spacer)
        garagingAddressString.Append(Address.Address.State + spacer)
        garagingAddressString.Append(Address.Address.Zip + spacer)
        garagingAddressString.Append(Address.Address.County)

        Return garagingAddressString.ToString
    End Function

    Public Function ifStringHasValue(input As String) As Boolean
        If input IsNot Nothing And Not String.IsNullOrEmpty(input) Then
            Return True
        End If
        Return False
    End Function

    ''' <summary>
    ''' The simplified version of genVehicleTable
    ''' Generates the vehicle section
    ''' </summary>
    ''' <param name="vehicle"></param>
    ''' <returns></returns>
    Public Function genVehicleTable_Multistate(vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle) As String
        Dim table As New StringBuilder("")
        Dim spacer As String = "&nbsp;&nbsp;"

        If vehicle.HasComprehensive Then
            ' Header
            table.Append("<tr>")
            table.Append("<td style='width:30%;'>")
            table.Append("Comprehensive")
            table.Append("</td>")
            table.Append("<td style='width:30%;'>")
            table.Append("Premium")
            table.Append("</td>")
            table.Append("</tr>")
            ' Data
            table.Append("<tr>")
            table.Append("<td style='width:30%;'>")
            table.Append(spacer & vehicle.ComprehensiveDeductible)
            table.Append("</td>")
            table.Append("<td style='width:30%;'>")
            table.Append(spacer & vehicle.ComprehensiveQuotedPremium)
            table.Append("</td>")
            table.Append("</tr>")
        End If

        If vehicle.HasCollision Then
            ' Header
            table.Append("<tr>")
            table.Append("<td style='width:30%;'>")
            table.Append("Collision")
            table.Append("</td>")
            table.Append("<td style='width:30%;'>")
            table.Append("Premium")
            table.Append("</td>")
            table.Append("</tr>")
            ' Data
            table.Append("<tr>")
            table.Append("<td style='width:30%;'>")
            table.Append(spacer & vehicle.CollisionDeductible)
            table.Append("</td>")
            table.Append("<td style='width:30%;'>")
            table.Append(spacer & vehicle.CollisionQuotedPremium)
            table.Append("</td>")
            table.Append("</tr>")
        End If

        If vehicle.HasUninsuredMotoristPropertyDamage Then
            Dim PremiumText As String
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                Select Case vehicle.GaragingAddress.Address.QuickQuoteState
                    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana
                        PremiumText = "Included"
                    Case Else
                        PremiumText = vehicle.UninsuredMotoristPropertyDamageQuotedPremium
                End Select
            Else
                Select Case vehicle.GaragingAddress.Address.QuickQuoteState
                    Case QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Illinois, QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Ohio
                        PremiumText = "Included"
                    Case Else
                        PremiumText = vehicle.UninsuredMotoristPropertyDamageQuotedPremium
                End Select
            End If

            ' Header
            table.Append("<tr>")
            table.Append("<td style='width:30%;'>")
            table.Append("UMPD")
            table.Append("</td>")
            table.Append("<td style='width:30%;'>")
            table.Append("Premium")
            table.Append("</td>")
            table.Append("</tr>")
            ' Data
            table.Append("<tr>")
            table.Append("<td style='width:30%;'>")
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                If Quote.QuickQuoteState = QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState.Indiana Then
                    If SubQuoteFirst.UninsuredMotoristPropertyDamageDeductibleId = "11" Then
                        'UMPD Deductible Id 11 is value 0 in VR and No Deductible in Diamond
                        table.Append("No Deductible")
                    Else
                        table.Append(QQHelper.GetStaticDataTextForValue(QuickQuoteHelperClass.QuickQuoteClassName.QuickQuoteObject, QuickQuoteHelperClass.QuickQuotePropertyName.UninsuredMotoristPropertyDamageDeductibleId, SubQuoteFirst.UninsuredMotoristPropertyDamageDeductibleId))
                    End If
                Else
                    table.Append("250")
                End If
            Else
                table.Append("")
            End If
            table.Append("</td>")
            table.Append("<td style='width:30%;'>")
            table.Append(spacer & PremiumText)
            table.Append("</td>")
            table.Append("</tr>")
        End If

        If vehicle.HasRentalReimbursement Then
            ' Header
            table.Append("<tr>")
            table.Append("<td style='width:30%;'>")
            table.Append("Rental")
            table.Append("</td>")
            table.Append("<td style='width:30%;'>")
            table.Append("Premium")
            table.Append("</td>")
            table.Append("</tr>")
            ' Data
            table.Append("<tr>")
            table.Append("<td style='width:30%;'>")
            table.Append(spacer & vehicle.RentalReimbursementDailyReimbursement)
            table.Append("</td>")
            table.Append("<td style='width:30%;'>")
            table.Append(spacer & vehicle.RentalReimbursementQuotedPremium)
            table.Append("</td>")
            table.Append("</tr>")
        End If

        If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
            ' Header
            table.Append("<tr>")
            table.Append("<td style='width:30%;'>")
            table.Append("Towing")
            table.Append("</td>")
            table.Append("<td style='width:30%;'>")
            table.Append("Premium")
            table.Append("</td>")
            table.Append("</tr>")
            ' Data
            table.Append("<tr>")
            table.Append("<td style='width:30%;'>")
            Dim towingAndLabor As String = vehicle.TowingAndLaborDeductibleLimitId
            If towingAndLabor <> "0" Then
                towingAndLabor = "N/A"
            End If
            table.Append(spacer & towingAndLabor)
            table.Append("</td>")
            table.Append("<td style='width:30%;'>")
            table.Append(spacer & vehicle.TowingAndLaborQuotedPremium)
            table.Append("</td>")
            table.Append("</tr>")

        Else

            If vehicle.HasTowingAndLabor Then
                ' Header
                table.Append("<tr>")
                table.Append("<td style='width:30%;'>")
                table.Append("Towing")
                table.Append("</td>")
                table.Append("<td style='width:30%;'>")
                table.Append("Premium")
                table.Append("</td>")
                table.Append("</tr>")
                ' Data
                table.Append("<tr>")
                table.Append("<td style='width:30%;'>")
                table.Append(spacer & vehicle.TowingAndLaborDeductibleLimitId)
                table.Append("</td>")
                table.Append("<td style='width:30%;'>")
                table.Append(spacer & vehicle.TowingAndLaborQuotedPremium)
                table.Append("</td>")
                table.Append("</tr>")
            End If
        End If

        Return table.ToString()
    End Function

    Public Function genVehicleTable_OLD(vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle) As String
        Dim table As New StringBuilder("")
        Dim tableItems As List(Of vehicleTableItem) = getVehicleTableItems(vehicle)

        Dim items As New List(Of Int16)

        ' There HAS to be a better way...

        If vehicle.HasComprehensive Then
            items.Add(0)
        End If
        If vehicle.HasCollision Then
            items.Add(1)
        End If
        If vehicle.HasUninsuredMotoristPropertyDamage Then
            items.Add(2)
        End If
        If vehicle.HasRentalReimbursement Then
            items.Add(3)
        End If
        If vehicle.HasTowingAndLabor Then
            items.Add(4)
        End If

        If items.Count > 2 Then
            table.Append("<tr Class=""qs_cap_grid_4_columns"">")
            table.Append(tableItems(items(0)).Header)
            table.Append(tableItems(items(2)).Header)
            table.Append("</tr>")
            table.Append("<tr Class=""qs_cap_grid_4_columns qs_cap_indent"">")
            table.Append(tableItems(items(0)).Values)
            table.Append(tableItems(items(2)).Values)
            table.Append("</tr>")
            table.Append("<tr Class=""qs_cap_grid_4_columns"">")
            table.Append(tableItems(items(1)).Header)
            If items.Count > 3 Then
                table.Append(tableItems(items(3)).Header)
            Else
                table.Append("<td></td><td></td>")
            End If
            table.Append("</tr>")
            table.Append("<tr Class=""qs_cap_grid_4_columns qs_cap_indent"">")
            table.Append(tableItems(items(1)).Values)
            If items.Count > 3 Then
                table.Append(tableItems(items(3)).Values)
            Else
                table.Append("<td></td><td></td>")
            End If
            table.Append("</tr>")
        Else
            If items.Count > 0 Then
                table.Append("<tr Class=""qs_cap_grid_4_columns"">")
                table.Append(tableItems(items(0)).Header)
                table.Append(<td></td>)
                table.Append("</tr>")
                table.Append("<tr Class=""qs_cap_grid_4_columns qs_cap_indent"">")
                table.Append(tableItems(items(0)).Values)
                table.Append(<td></td>)
                table.Append("</tr>")
                table.Append("<tr Class=""qs_cap_grid_4_columns"">")
                If items.Count > 1 Then
                    table.Append(tableItems(items(1)).Header)
                    'Else
                    '    table.Append("<td></td><td></td>")
                End If
                table.Append("</tr>")
                table.Append("<tr Class=""qs_cap_grid_4_columns qs_cap_indent"">")
                If items.Count > 1 Then
                    table.Append(tableItems(items(1)).Values)
                    'Else
                    '    table.Append("<td></td><td></td>")
                End If
                table.Append("</tr>")

            End If
            table.Append("<tr Class=""qs_cap_grid_4_columns"">")
            table.Append("<td colspan = ""4"">" + GetGaragingAddressString(vehicle.GaragingAddress) + "</td>")
            table.Append("</tr>")
        End If
        Return table.ToString()




    End Function
    Sub R1_ItemDataBound(Sender As Object, e As RepeaterItemEventArgs)
        If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
            ' Use the new table layout for multistate
            CType(e.Item.FindControl("tblVehicleInfo"), Literal).Text = genVehicleTable_Multistate(e.Item.DataItem)
        Else
            ' Use the old table layout for non-multistate
            CType(e.Item.FindControl("tblVehicleInfo"), Literal).Text = genVehicleTable_OLD(e.Item.DataItem)
        End If

        'genVehicleTable(e.Item.DataItem)
    End Sub

    Public Function getVehicleTableItems(vehicle As QuickQuote.CommonObjects.QuickQuoteVehicle) As List(Of vehicleTableItem)
        Dim umpd As New vehicleTableItem()
        Dim comp As New vehicleTableItem()
        comp.Header = "<td> Comprehensive</td><td>Premium</td>"
        comp.Values = "<td>" + vehicle.ComprehensiveDeductible + "</td><td>" + vehicle.ComprehensiveQuotedPremium + "</td>"

        Dim coll As New vehicleTableItem()
        coll.Header = "<td> Collision</td><td>Premium</td>"
        coll.Values = "<td>" + vehicle.CollisionDeductible + "</td><td>" + vehicle.CollisionQuotedPremium + "</td>"

        ' Added UMPD for Multistate - MGB 9/25/18
        If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
            umpd.Header = "<td> UMPD</td><td>Premium</td>"
            umpd.Values = "<td>" + "N/A" + "</td><td>" + vehicle.UninsuredMotoristPropertyDamageQuotedPremium + "</td>"
            ' umpd.Values = "<td>" + "N/A" + "</td><td>" + "N/A" + "</td>"
            'umpd.Values = "<td>" + vehicle.RentalReimbursementNumberOfDays + "/" + vehicle.RentalReimbursementDailyReimbursement.Replace(".0000", "") + "</td><td>" + vehicle.RentalReimbursementQuotedPremium + "</td>"
        End If

        Dim rent As New vehicleTableItem()
        rent.Header = "<td> Rental</td><td>Premium</td>"
        rent.Values = "<td>" + vehicle.RentalReimbursementNumberOfDays + "/" + vehicle.RentalReimbursementDailyReimbursement.Replace(".0000", "") + "</td><td>" + vehicle.RentalReimbursementQuotedPremium + "</td>"

        Dim tow As New vehicleTableItem()
        tow.Header = "<td> Towing And Labor</td><td>Premium</td>"
        tow.Values = "<td></td><td>" + vehicle.TowingAndLaborQuotedPremium + "</td>"


        Dim tableItems As New List(Of vehicleTableItem)
        tableItems.Add(comp)
        tableItems.Add(coll)
        If IsMultistateCapableEffectiveDate(Quote.EffectiveDate) Then
            If vehicle.HasUninsuredMotoristPropertyDamage Then tableItems.Add(umpd)
        End If
        tableItems.Add(rent)
        tableItems.Add(tow)

        Return tableItems
    End Function

    Public Class vehicleTableItem
        Public Header As String
        Public Values As String
    End Class




End Class