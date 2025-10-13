Imports IFM.VR.Common.Helpers.CAP

Public Class ctl_CAP_Symbols
    Inherits VRControlBase

    Public Overrides Sub AddScriptAlways()

    End Sub

    Public Overrides Sub AddScriptWhenRendered()
        Me.VRScript.CreateAccordion(Me.MainAccordionDivId, Me.hdnAccord, "0")
        Me.VRScript.StopEventPropagation(Me.lnkSave.ClientID)

        ' SCRIPT FOR SYMBOL CHECKBOXES
        ' Liability
        Me.chk_Liab_1.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('LIAB', '" & chk_Liab_1.ClientID & "','" & chk_Liab_1.ClientID & "','" & chk_Liab_2.ClientID & "','" & chk_Liab_3.ClientID & "','" & chk_Liab_4.ClientID & "','" & chk_Liab_7.ClientID & "','" & chk_Liab_8.ClientID & "','" & chk_Liab_9.ClientID & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol8 & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol9 & "')")
        Me.chk_Liab_2.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('LIAB', '" & chk_Liab_2.ClientID & "','" & chk_Liab_1.ClientID & "','" & chk_Liab_2.ClientID & "','" & chk_Liab_3.ClientID & "','" & chk_Liab_4.ClientID & "','" & chk_Liab_7.ClientID & "','" & chk_Liab_8.ClientID & "','" & chk_Liab_9.ClientID & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol8 & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol9 & "')")
        Me.chk_Liab_3.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('LIAB', '" & chk_Liab_3.ClientID & "','" & chk_Liab_1.ClientID & "','" & chk_Liab_2.ClientID & "','" & chk_Liab_3.ClientID & "','" & chk_Liab_4.ClientID & "','" & chk_Liab_7.ClientID & "','" & chk_Liab_8.ClientID & "','" & chk_Liab_9.ClientID & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol8 & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol9 & "')")
        Me.chk_Liab_4.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('LIAB', '" & chk_Liab_4.ClientID & "','" & chk_Liab_1.ClientID & "','" & chk_Liab_2.ClientID & "','" & chk_Liab_3.ClientID & "','" & chk_Liab_4.ClientID & "','" & chk_Liab_7.ClientID & "','" & chk_Liab_8.ClientID & "','" & chk_Liab_9.ClientID & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol8 & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol9 & "')")
        Me.chk_Liab_7.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('LIAB', '" & chk_Liab_7.ClientID & "','" & chk_Liab_1.ClientID & "','" & chk_Liab_2.ClientID & "','" & chk_Liab_3.ClientID & "','" & chk_Liab_4.ClientID & "','" & chk_Liab_7.ClientID & "','" & chk_Liab_8.ClientID & "','" & chk_Liab_9.ClientID & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol8 & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol9 & "')")
        Me.chk_Liab_8.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('LIAB', '" & chk_Liab_8.ClientID & "','" & chk_Liab_1.ClientID & "','" & chk_Liab_2.ClientID & "','" & chk_Liab_3.ClientID & "','" & chk_Liab_4.ClientID & "','" & chk_Liab_7.ClientID & "','" & chk_Liab_8.ClientID & "','" & chk_Liab_9.ClientID & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol8 & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol9 & "')")
        Me.chk_Liab_9.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('LIAB', '" & chk_Liab_9.ClientID & "','" & chk_Liab_1.ClientID & "','" & chk_Liab_2.ClientID & "','" & chk_Liab_3.ClientID & "','" & chk_Liab_4.ClientID & "','" & chk_Liab_7.ClientID & "','" & chk_Liab_8.ClientID & "','" & chk_Liab_9.ClientID & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol8 & "','" & SubQuoteFirst.CAP_Liability_WouldHaveSymbol9 & "')")

        ' Medical Payments
        Me.chk_MedPay_2.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('MEDPAY', '" & chk_MedPay_2.ClientID & "','','" & chk_MedPay_2.ClientID & "','" & chk_MedPay_3.ClientID & "','" & chk_MedPay_4.ClientID & "','" & chk_MedPay_7.ClientID & "','','','','')")
        Me.chk_MedPay_3.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('MEDPAY', '" & chk_MedPay_3.ClientID & "','','" & chk_MedPay_2.ClientID & "','" & chk_MedPay_3.ClientID & "','" & chk_MedPay_4.ClientID & "','" & chk_MedPay_7.ClientID & "','','','')")
        Me.chk_MedPay_4.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('MEDPAY', '" & chk_MedPay_4.ClientID & "','','" & chk_MedPay_2.ClientID & "','" & chk_MedPay_3.ClientID & "','" & chk_MedPay_4.ClientID & "','" & chk_MedPay_7.ClientID & "','','','','')")
        Me.chk_MedPay_7.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('MEDPAY', '" & chk_MedPay_7.ClientID & "','','" & chk_MedPay_2.ClientID & "','" & chk_MedPay_3.ClientID & "','" & chk_MedPay_4.ClientID & "','" & chk_MedPay_7.ClientID & "','','','')")

        If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
            ' UM
            Me.chk_UM_2.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('UM','" & chk_UM_2.ClientID & "','','" & chk_UM_2.ClientID & "','" & chk_UM_3.ClientID & "','" & chk_UM_4.ClientID & "','" & chk_UM_7.ClientID & "','','','','')")
            Me.chk_UM_3.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('UM','" & chk_UM_3.ClientID & "','','" & chk_UM_2.ClientID & "','" & chk_UM_3.ClientID & "','" & chk_UM_4.ClientID & "','" & chk_UM_7.ClientID & "','','','','')")
            Me.chk_UM_4.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('UM','" & chk_UM_4.ClientID & "','','" & chk_UM_2.ClientID & "','" & chk_UM_3.ClientID & "','" & chk_UM_4.ClientID & "','" & chk_UM_7.ClientID & "','','','','')")
            Me.chk_UM_7.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('UM','" & chk_UM_7.ClientID & "','','" & chk_UM_2.ClientID & "','" & chk_UM_3.ClientID & "','" & chk_UM_4.ClientID & "','" & chk_UM_7.ClientID & "','','','','')")

            ' UIM
            Me.chk_UIM_2.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('UIM','" & chk_UIM_2.ClientID & "','','" & chk_UIM_2.ClientID & "','" & chk_UIM_3.ClientID & "','" & chk_UIM_4.ClientID & "','" & chk_UIM_7.ClientID & "','','','','')")
            Me.chk_UIM_3.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('UIM','" & chk_UIM_3.ClientID & "','','" & chk_UIM_2.ClientID & "','" & chk_UIM_3.ClientID & "','" & chk_UIM_4.ClientID & "','" & chk_UIM_7.ClientID & "','','','','')")
            Me.chk_UIM_4.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('UIM','" & chk_UIM_4.ClientID & "','','" & chk_UIM_2.ClientID & "','" & chk_UIM_3.ClientID & "','" & chk_UIM_4.ClientID & "','" & chk_UIM_7.ClientID & "','','','','')")
            Me.chk_UIM_7.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('UIM','" & chk_UIM_7.ClientID & "','','" & chk_UIM_2.ClientID & "','" & chk_UIM_3.ClientID & "','" & chk_UIM_4.ClientID & "','" & chk_UIM_7.ClientID & "','','','','')")
        Else
            ' UM/UIM
            Me.chk_UMUIM_2.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('UMUIM', '" & chk_UMUIM_2.ClientID & "','','" & chk_UMUIM_2.ClientID & "','" & chk_UMUIM_3.ClientID & "','" & chk_UMUIM_4.ClientID & "','" & chk_UMUIM_7.ClientID & "','','','','')")
            Me.chk_UMUIM_3.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('UMUIM', '" & chk_UMUIM_3.ClientID & "','','" & chk_UMUIM_2.ClientID & "','" & chk_UMUIM_3.ClientID & "','" & chk_UMUIM_4.ClientID & "','" & chk_UMUIM_7.ClientID & "','','','')")
            Me.chk_UMUIM_4.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('UMUIM', '" & chk_UMUIM_4.ClientID & "','','" & chk_UMUIM_2.ClientID & "','" & chk_UMUIM_3.ClientID & "','" & chk_UMUIM_4.ClientID & "','" & chk_UMUIM_7.ClientID & "','','','','')")
            Me.chk_UMUIM_7.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('UMUIM', '" & chk_UMUIM_7.ClientID & "','','" & chk_UMUIM_2.ClientID & "','" & chk_UMUIM_3.ClientID & "','" & chk_UMUIM_4.ClientID & "','" & chk_UMUIM_7.ClientID & "','','','')")
        End If

        ' Comprehensive
        Me.chk_Comp_2.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('COMP', '" & chk_Comp_2.ClientID & "','','" & chk_Comp_2.ClientID & "','" & chk_Comp_3.ClientID & "','" & chk_Comp_4.ClientID & "','" & chk_Comp_7.ClientID & "','" & chk_Comp_8.ClientID & "','','','')")
        Me.chk_Comp_3.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('COMP', '" & chk_Comp_3.ClientID & "','','" & chk_Comp_2.ClientID & "','" & chk_Comp_3.ClientID & "','" & chk_Comp_4.ClientID & "','" & chk_Comp_7.ClientID & "','" & chk_Comp_8.ClientID & "','','','')")
        Me.chk_Comp_4.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('COMP', '" & chk_Comp_4.ClientID & "','','" & chk_Comp_2.ClientID & "','" & chk_Comp_3.ClientID & "','" & chk_Comp_4.ClientID & "','" & chk_Comp_7.ClientID & "','" & chk_Comp_8.ClientID & "','','','')")
        Me.chk_Comp_7.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('COMP', '" & chk_Comp_7.ClientID & "','','" & chk_Comp_2.ClientID & "','" & chk_Comp_3.ClientID & "','" & chk_Comp_4.ClientID & "','" & chk_Comp_7.ClientID & "','" & chk_Comp_8.ClientID & "','','','')")
        Me.chk_Comp_8.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('COMP', '" & chk_Comp_8.ClientID & "','','" & chk_Comp_2.ClientID & "','" & chk_Comp_3.ClientID & "','" & chk_Comp_4.ClientID & "','" & chk_Comp_7.ClientID & "','" & chk_Comp_8.ClientID & "','','','')")

        ' Collision
        Me.chk_Coll_2.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('COLL', '" & chk_Coll_2.ClientID & "','','" & chk_Coll_2.ClientID & "','" & chk_Coll_3.ClientID & "','" & chk_Coll_4.ClientID & "','" & chk_Coll_7.ClientID & "','" & chk_Coll_8.ClientID & "','','','')")
        Me.chk_Coll_3.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('COLL', '" & chk_Coll_3.ClientID & "','','" & chk_Coll_2.ClientID & "','" & chk_Coll_3.ClientID & "','" & chk_Coll_4.ClientID & "','" & chk_Coll_7.ClientID & "','" & chk_Coll_8.ClientID & "','','','')")
        Me.chk_Coll_4.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('COLL', '" & chk_Coll_4.ClientID & "','','" & chk_Coll_2.ClientID & "','" & chk_Coll_3.ClientID & "','" & chk_Coll_4.ClientID & "','" & chk_Coll_7.ClientID & "','" & chk_Coll_8.ClientID & "','','','')")
        Me.chk_Coll_7.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('COLL', '" & chk_Coll_7.ClientID & "','','" & chk_Coll_2.ClientID & "','" & chk_Coll_3.ClientID & "','" & chk_Coll_4.ClientID & "','" & chk_Coll_7.ClientID & "','" & chk_Coll_8.ClientID & "','','','')")
        Me.chk_Coll_8.Attributes.Add("onchange", "Cap.SymbolCheckboxChanged('COLL', '" & chk_Coll_8.ClientID & "','','" & chk_Coll_2.ClientID & "','" & chk_Coll_3.ClientID & "','" & chk_Coll_4.ClientID & "','" & chk_Coll_7.ClientID & "','" & chk_Coll_8.ClientID & "','','','')")

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()
        If SubQuoteFirst IsNot Nothing Then
            SubQuoteFirst.UseDeveloperAutoSymbols = True

            ' Sets the symbol 8 & 9 checkbox values and Towing & Labor as well
            SetSymbolDefaults()

            ' *** LIABILITY ***
            chk_Liab_8.Enabled = False
            chk_Liab_9.Enabled = False
            If SubQuoteFirst.LiabilityAutoSymbolObject.HasAnySymbols Then
                ' Quote has symbols - display them
                'If SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol1 Then chk_Liab_1.Checked = True Else chk_Liab_1.Checked = False
                If SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol1 Then
                    chk_Liab_1.Checked = True
                    'Will this end up changing the SubQuoteFirst.HasHiredBorrowedNonOwned to false?
                    chk_Liab_8.Checked = False
                    chk_Liab_9.Checked = False
                Else
                    chk_Liab_1.Checked = False
                    If SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol2 OrElse SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol3 OrElse SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol4 OrElse SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol7 Then
                        'If SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol8 Then
                        If SubQuoteFirst.HasHiredBorrowedLiability Then
                            chk_Liab_8.Checked = True
                        Else
                            chk_Liab_8.Checked = False
                        End If
                        'If SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol9 Then
                        If SubQuoteFirst.HasNonOwnershipLiability Then
                            chk_Liab_9.Checked = True
                        Else
                            chk_Liab_9.Checked = False
                        End If
                    End If
                End If
                If SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol2 Then chk_Liab_2.Checked = True Else chk_Liab_2.Checked = False
                If SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol3 Then chk_Liab_3.Checked = True Else chk_Liab_3.Checked = False
                If SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol4 Then chk_Liab_4.Checked = True Else chk_Liab_4.Checked = False
                If SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol7 Then chk_Liab_7.Checked = True Else chk_Liab_7.Checked = False

            End If

            ' *** MEDICAL PAYMENTS ***
            If SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasAnySymbols Then
                ' Quote has symbols - display them
                If SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasSymbol2 Then chk_MedPay_2.Checked = True Else chk_MedPay_2.Checked = False
                If SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasSymbol3 Then chk_MedPay_3.Checked = True Else chk_MedPay_3.Checked = False
                If SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasSymbol4 Then chk_MedPay_4.Checked = True Else chk_MedPay_4.Checked = False
                If SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasSymbol7 Then chk_MedPay_7.Checked = True Else chk_MedPay_7.Checked = False
            End If

            PopulateUMUIMSymbols()

            ' *** COMPREHENSIVE ***
            chk_Comp_8.Enabled = False
            If SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasAnySymbols Then
                ' Quote has symbols - display them
                If SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol2 Then chk_Comp_2.Checked = True Else chk_Comp_2.Checked = False
                If SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol3 Then chk_Comp_3.Checked = True Else chk_Comp_3.Checked = False
                If SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol4 Then chk_Comp_4.Checked = True Else chk_Comp_4.Checked = False
                If SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol7 Then chk_Comp_7.Checked = True Else chk_Comp_7.Checked = False
            End If


            ' *** COLLISION ***
            If SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasAnySymbols Then
                ' Quote has symbols - display them
                If SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol2 Then chk_Coll_2.Checked = True Else chk_Coll_2.Checked = False
                If SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol3 Then chk_Coll_3.Checked = True Else chk_Coll_3.Checked = False
                If SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol4 Then chk_Coll_4.Checked = True Else chk_Coll_4.Checked = False
                If SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol7 Then chk_Coll_7.Checked = True Else chk_Coll_7.Checked = False

            End If

            ' *** TOWING & LABOR
            chk_Tow_7.Enabled = False
            If SubQuoteFirst.TowingAndLaborAutoSymbolObject.HasAnySymbols Then
                If SubQuoteFirst.TowingAndLaborAutoSymbolObject.HasSymbol7 Then chk_Tow_7.Checked = True Else chk_Tow_7.Checked = False
            End If

        End If

    End Sub

    Public Sub PopulateUMUIMSymbols()
        If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
            'show separate checkboxes for UM, UIM
            trUMUIMSymbols.Attributes.Add("style", "display:none")
            trUMSymbols.Attributes.Add("style", "display:''")
            trUIMSymbols.Attributes.Add("style", "display:''")

            ' *** UM ***
            If SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasAnySymbols Then
                ' Quote has symbols - display them
                If SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol2 Then chk_UM_2.Checked = True Else chk_UM_2.Checked = False
                If SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol3 Then chk_UM_3.Checked = True Else chk_UM_3.Checked = False
                If SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol4 Then chk_UM_4.Checked = True Else chk_UM_4.Checked = False
                If SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol7 Then chk_UM_7.Checked = True Else chk_UM_7.Checked = False
            End If
            ' *** UIM ***
            If SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasAnySymbols Then
                ' Quote has symbols - display them
                If SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol2 Then chk_UIM_2.Checked = True Else chk_UIM_2.Checked = False
                If SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol3 Then chk_UIM_3.Checked = True Else chk_UIM_3.Checked = False
                If SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol4 Then chk_UIM_4.Checked = True Else chk_UIM_4.Checked = False
                If SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol7 Then chk_UIM_7.Checked = True Else chk_UIM_7.Checked = False
            End If
        Else
            'show combined checkboxes for UM/UIM
            trUMUIMSymbols.Attributes.Add("style", "display:''")
            trUMSymbols.Attributes.Add("style", "display:none")
            trUIMSymbols.Attributes.Add("style", "display:none")

            ' *** UM/UIM ***
            If SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasAnySymbols OrElse SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasAnySymbols Then
                ' Quote has symbols - display them
                If SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol2 OrElse SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol2 Then chk_UMUIM_2.Checked = True Else chk_UMUIM_2.Checked = False
                If SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol3 OrElse SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol3 Then chk_UMUIM_3.Checked = True Else chk_UMUIM_3.Checked = False
                If SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol4 OrElse SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol4 Then chk_UMUIM_4.Checked = True Else chk_UMUIM_4.Checked = False
                If SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol7 OrElse SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol7 Then chk_UMUIM_7.Checked = True Else chk_UMUIM_7.Checked = False
            End If
        End If
    End Sub

    ''' <summary>
    ''' Sets the symbol 8 & 9 checkbox values and Towing & Labor as well
    ''' </summary>
    Private Sub SetSymbolDefaults()
        ' Liability 7 - checked when quote has vehicles, unchecked otherwise
        If QuoteHasVehicles() Then
            chk_Liab_7.Checked = True
        Else
            chk_Liab_7.Checked = False
        End If
        ' Liability 8 - Disabled & Checked when quote has Hired/Borrowed/Non-Owned, otherwise Disabled and Unchecked
        If SubQuoteFirst.HasHiredBorrowedLiability Then
            chk_Liab_8.Checked = True
        Else
            chk_Liab_8.Checked = False
        End If
        ' Liability 9 - Disabled & Checked when quote has Hired/Borrowed/Non-Owned, otherwise Disabled & Unchecked
        If SubQuoteFirst.HasNonOwnershipLiability Then
            chk_Liab_9.Checked = True
        Else
            chk_Liab_9.Checked = False
        End If

        ' Med Pay 7 - Checked when med pay is added to any vehicle, otherwise unchecked
        If AnyVehicleHasMedPay() Then chk_MedPay_7.Checked = True Else chk_MedPay_7.Checked = False


        ' UM/UIM 7 - Checked when vehicles & comprehensive, otherwise unchecked
        Dim checkUMUIM As Boolean = False
        If QuoteHasVehicles() AndAlso AnyVehicleHasComprehensive() Then
            checkUMUIM = True
        End If
        If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
            chk_UM_7.Checked = checkUMUIM
            chk_UIM_7.Checked = checkUMUIM
        Else
            chk_UMUIM_7.Checked = checkUMUIM
        End If

        ' Comprehensive 7 - Checked when quote has vehicles otherwise unchecked
        If QuoteHasVehicles() Then
            chk_Comp_7.Checked = True
        Else
            chk_Comp_7.Checked = False
        End If
        ' Comprehensive 8 - Always Disabled.  Checked when hired car physical damage is on quote otherwise unchecked
        If SubQuoteFirst.HasHiredCarPhysicalDamage Then
            chk_Comp_8.Checked = True
        Else
            chk_Comp_8.Checked = False
        End If

        ' Collision 7 - checked when vehicles & collision, otherwise unchecked
        If QuoteHasVehicles() AndAlso AnyVehicleHasCollision() Then
            chk_Coll_7.Checked = True
        Else
            chk_Coll_7.Checked = False
        End If
        ' Collision 8 - Always Disabled.  Checked when hired car physical damage is on quote otherwise unchecked
        chk_Coll_8.Enabled = False
        If SubQuoteFirst.HasHiredCarPhysicalDamage Then
            chk_Coll_8.Checked = True
        Else
            chk_Coll_8.Checked = False
        End If

        ' Towing & Labor 7 - Always Disabled.  Checked when any vehicle has towing and labor otherwise unchecked
        If AnyVehicleHasTowingAndLabor() Then
            chk_Tow_7.Checked = True
        Else
            chk_Tow_7.Checked = False
        End If

    End Sub

    Private Function QuoteHasVehicles()
        If Quote IsNot Nothing Then
            If Quote.Vehicles IsNot Nothing AndAlso Quote.Vehicles.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End If

        Return False
    End Function

    Private Function AnyVehicleHasMedPay() As Boolean
        If Quote IsNot Nothing Then
            If QuoteHasVehicles() Then
                For Each v As QuickQuote.CommonObjects.QuickQuoteVehicle In Quote.Vehicles
                    If v.HasMedicalPayments Then Return True
                Next
            End If
        End If

        Return False
    End Function

    Private Function AnyVehicleHasComprehensive() As Boolean
        If Quote IsNot Nothing Then
            If QuoteHasVehicles() Then
                For Each v As QuickQuote.CommonObjects.QuickQuoteVehicle In Quote.Vehicles
                    If v.HasComprehensive Then Return True
                Next
            End If
        End If

        Return False
    End Function

    Private Function AnyVehicleHasCollision() As Boolean
        If Quote IsNot Nothing Then
            If QuoteHasVehicles() Then
                For Each v As QuickQuote.CommonObjects.QuickQuoteVehicle In Quote.Vehicles
                    If v.HasCollision Then Return True
                Next
            End If
        End If

        Return False
    End Function

    Private Function AnyVehicleHasTowingAndLabor() As Boolean
        If Quote IsNot Nothing Then
            If QuoteHasVehicles() Then
                For Each v As QuickQuote.CommonObjects.QuickQuoteVehicle In Quote.Vehicles
                    If v.HasTowingAndLabor Then Return True
                Next
            End If
        End If

        Return False
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Me.MainAccordionDivId = Me.divSymbols.ClientID
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If Quote IsNot Nothing Then
            'Dim sym As New QuickQuote.CommonObjects.QuickQuoteAutoSymbol()

            'SubQuoteFirst.UseDeveloperAutoSymbols = True

            '' autosymboltypeId:  (the id's correspond to the checkbox columns)
            '' ID    Description
            '' 1     Any "Auto"
            '' 2     Owned "Autos" Only
            '' 3     Owned Private Passenger "Autos" Only
            '' 4     Owned "Autos" Other Than Private Passenger "Autos" Only
            '' 7     Specifically Described "Autos"
            '' 8     Hired "Autos" Only
            '' 9     Non-Owned "Autos" Only

            '' *** LIABILITY ***
            'If chk_Liab_1.Checked Then SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol1 = True Else SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol1 = False
            'If chk_Liab_2.Checked Then SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol2 = True Else SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol2 = False
            'If chk_Liab_3.Checked Then SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol3 = True Else SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol3 = False
            'If chk_Liab_4.Checked Then SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol4 = True Else SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol4 = False
            'If chk_Liab_7.Checked Then SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol7 = True Else SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol7 = False
            'If chk_Liab_8.Checked Then SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol8 = True Else SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol8 = False
            'If chk_Liab_9.Checked Then SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol9 = True Else SubQuoteFirst.LiabilityAutoSymbolObject.HasSymbol9 = False

            '' *** MED PAY ***
            'If chk_MedPay_2.Checked Then SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasSymbol2 = True Else SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasSymbol2 = False
            'If chk_MedPay_3.Checked Then SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasSymbol3 = True Else SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasSymbol3 = False
            'If chk_MedPay_4.Checked Then SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasSymbol4 = True Else SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasSymbol4 = False
            'If chk_MedPay_7.Checked Then SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasSymbol7 = True Else SubQuoteFirst.MedicalPaymentsAutoSymbolObject.HasSymbol7 = False

            '' *** UM/UIM ***
            '' Note that we load both the UnderInsuredMotoristAutoSymbolObject and UninsuredMotoristAutoSymbolObject
            'If chk_UMUIM_2.Checked Then SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol2 = True Else SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol2 = False
            'If chk_UMUIM_3.Checked Then SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol3 = True Else SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol3 = False
            'If chk_UMUIM_4.Checked Then SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol4 = True Else SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol4 = False
            'If chk_UMUIM_7.Checked Then SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol7 = True Else SubQuoteFirst.UnderinsuredMotoristAutoSymbolObject.HasSymbol7 = False

            'If chk_UMUIM_2.Checked Then SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol2 = True Else SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol2 = False
            'If chk_UMUIM_3.Checked Then SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol3 = True Else SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol3 = False
            'If chk_UMUIM_4.Checked Then SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol4 = True Else SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol4 = False
            'If chk_UMUIM_7.Checked Then SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol7 = True Else SubQuoteFirst.UninsuredMotoristAutoSymbolObject.HasSymbol7 = False

            '' *** COMPREHENSIVE ***
            'If chk_Comp_2.Checked Then SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol2 = True Else SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol2 = False
            'If chk_Comp_3.Checked Then SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol3 = True Else SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol3 = False
            'If chk_Comp_4.Checked Then SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol4 = True Else SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol4 = False
            'If chk_Comp_7.Checked Then SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol7 = True Else SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol7 = False
            'If chk_Comp_8.Checked Then SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol8 = True Else SubQuoteFirst.ComprehensiveCoverageAutoSymbolObject.HasSymbol8 = False

            '' *** COLLISION ***
            'If chk_Coll_2.Checked Then SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol2 = True Else SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol2 = False
            'If chk_Coll_3.Checked Then SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol3 = True Else SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol3 = False
            'If chk_Coll_4.Checked Then SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol4 = True Else SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol4 = False
            'If chk_Coll_7.Checked Then SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol7 = True Else SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol7 = False
            'If chk_Coll_8.Checked Then SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol8 = True Else SubQuoteFirst.CollisionCoverageAutoSymbolObject.HasSymbol8 = False

            '' *** TOWING & LABOR
            'If chk_Tow_7.Checked Then SubQuoteFirst.TowingAndLaborAutoSymbolObject.HasSymbol7 = True Else SubQuoteFirst.TowingAndLaborAutoSymbolObject.HasSymbol7 = False

            'updated 11/16/2018; was previously only saving to the 1st stateQuote instead of all
            If SubQuotes IsNot Nothing AndAlso SubQuotes.Count > 0 Then
                For Each stateQuote As QuickQuote.CommonObjects.QuickQuoteObject In SubQuotes
                    SaveFormToStateQuote(stateQuote)
                Next
            Else 'shouldn't get here but just in case
                SaveFormToStateQuote(Quote)
            End If

        End If

        Me.SaveChildControls()
        Return True
    End Function

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        MyBase.ValidateControl(valArgs)
        Me.ValidateChildControls(valArgs)
    End Sub

    Private Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        Save_FireSaveEvent(True)
    End Sub

    'added 11/16/2018
    Private Sub SaveFormToStateQuote(ByVal stateQuote As QuickQuote.CommonObjects.QuickQuoteObject)
        If stateQuote IsNot Nothing Then
            Dim sym As New QuickQuote.CommonObjects.QuickQuoteAutoSymbol()

            stateQuote.UseDeveloperAutoSymbols = True

            ' autosymboltypeId:  (the id's correspond to the checkbox columns)
            ' ID    Description
            ' 1     Any "Auto"
            ' 2     Owned "Autos" Only
            ' 3     Owned Private Passenger "Autos" Only
            ' 4     Owned "Autos" Other Than Private Passenger "Autos" Only
            ' 7     Specifically Described "Autos"
            ' 8     Hired "Autos" Only
            ' 9     Non-Owned "Autos" Only

            ' *** LIABILITY ***
            If chk_Liab_1.Checked Then stateQuote.LiabilityAutoSymbolObject.HasSymbol1 = True Else stateQuote.LiabilityAutoSymbolObject.HasSymbol1 = False
            If chk_Liab_2.Checked Then stateQuote.LiabilityAutoSymbolObject.HasSymbol2 = True Else stateQuote.LiabilityAutoSymbolObject.HasSymbol2 = False
            If chk_Liab_3.Checked Then stateQuote.LiabilityAutoSymbolObject.HasSymbol3 = True Else stateQuote.LiabilityAutoSymbolObject.HasSymbol3 = False
            If chk_Liab_4.Checked Then stateQuote.LiabilityAutoSymbolObject.HasSymbol4 = True Else stateQuote.LiabilityAutoSymbolObject.HasSymbol4 = False
            If chk_Liab_7.Checked Then stateQuote.LiabilityAutoSymbolObject.HasSymbol7 = True Else stateQuote.LiabilityAutoSymbolObject.HasSymbol7 = False
            If stateQuote.CAP_Liability_WouldHaveSymbol8 Then
                If stateQuote.LiabilityAutoSymbolObject.HasSymbol1 Then
                    stateQuote.LiabilityAutoSymbolObject.HasSymbol8 = False
                    chk_Liab_8.Checked = False
                Else
                    stateQuote.LiabilityAutoSymbolObject.HasSymbol8 = True
                    chk_Liab_8.Checked = True
                End If
            Else
                stateQuote.LiabilityAutoSymbolObject.HasSymbol8 = False
                chk_Liab_8.Checked = False
            End If
            If stateQuote.CAP_Liability_WouldHaveSymbol9 Then
                If stateQuote.LiabilityAutoSymbolObject.HasSymbol1 Then
                    stateQuote.LiabilityAutoSymbolObject.HasSymbol9 = False
                    chk_Liab_9.Checked = False
                Else
                    stateQuote.LiabilityAutoSymbolObject.HasSymbol9 = True
                    chk_Liab_9.Checked = True
                End If
            Else
                stateQuote.LiabilityAutoSymbolObject.HasSymbol9 = False
                chk_Liab_9.Checked = False
            End If

            ' *** MED PAY ***
            If chk_MedPay_2.Checked Then stateQuote.MedicalPaymentsAutoSymbolObject.HasSymbol2 = True Else stateQuote.MedicalPaymentsAutoSymbolObject.HasSymbol2 = False
            If chk_MedPay_3.Checked Then stateQuote.MedicalPaymentsAutoSymbolObject.HasSymbol3 = True Else stateQuote.MedicalPaymentsAutoSymbolObject.HasSymbol3 = False
            If chk_MedPay_4.Checked Then stateQuote.MedicalPaymentsAutoSymbolObject.HasSymbol4 = True Else stateQuote.MedicalPaymentsAutoSymbolObject.HasSymbol4 = False
            If chk_MedPay_7.Checked Then stateQuote.MedicalPaymentsAutoSymbolObject.HasSymbol7 = True Else stateQuote.MedicalPaymentsAutoSymbolObject.HasSymbol7 = False

            ' *** UM/UIM ***
            If UM_UIM_UMPDHelper.IsCAP_UM_UIM_UMPD_ChangesAvailable(Quote) Then
                'UM
                If chk_UM_2.Checked Then stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol2 = True Else stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol2 = False
                If chk_UM_3.Checked Then stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol3 = True Else stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol3 = False
                If chk_UM_4.Checked Then stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol4 = True Else stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol4 = False
                If chk_UM_7.Checked Then stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol7 = True Else stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol7 = False
                'UIM
                If chk_UIM_2.Checked Then stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol2 = True Else stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol2 = False
                If chk_UIM_3.Checked Then stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol3 = True Else stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol3 = False
                If chk_UIM_4.Checked Then stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol4 = True Else stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol4 = False
                If chk_UIM_7.Checked Then stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol7 = True Else stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol7 = False
            Else
                ' Note that we load both the UnderInsuredMotoristAutoSymbolObject and UninsuredMotoristAutoSymbolObject
                If chk_UMUIM_2.Checked Then stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol2 = True Else stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol2 = False
                If chk_UMUIM_3.Checked Then stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol3 = True Else stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol3 = False
                If chk_UMUIM_4.Checked Then stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol4 = True Else stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol4 = False
                If chk_UMUIM_7.Checked Then stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol7 = True Else stateQuote.UnderinsuredMotoristAutoSymbolObject.HasSymbol7 = False

                If chk_UMUIM_2.Checked Then stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol2 = True Else stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol2 = False
                If chk_UMUIM_3.Checked Then stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol3 = True Else stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol3 = False
                If chk_UMUIM_4.Checked Then stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol4 = True Else stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol4 = False
                If chk_UMUIM_7.Checked Then stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol7 = True Else stateQuote.UninsuredMotoristAutoSymbolObject.HasSymbol7 = False
            End If

            ' *** COMPREHENSIVE ***
            If chk_Comp_2.Checked Then stateQuote.ComprehensiveCoverageAutoSymbolObject.HasSymbol2 = True Else stateQuote.ComprehensiveCoverageAutoSymbolObject.HasSymbol2 = False
            If chk_Comp_3.Checked Then stateQuote.ComprehensiveCoverageAutoSymbolObject.HasSymbol3 = True Else stateQuote.ComprehensiveCoverageAutoSymbolObject.HasSymbol3 = False
            If chk_Comp_4.Checked Then stateQuote.ComprehensiveCoverageAutoSymbolObject.HasSymbol4 = True Else stateQuote.ComprehensiveCoverageAutoSymbolObject.HasSymbol4 = False
            If chk_Comp_7.Checked Then stateQuote.ComprehensiveCoverageAutoSymbolObject.HasSymbol7 = True Else stateQuote.ComprehensiveCoverageAutoSymbolObject.HasSymbol7 = False
            If chk_Comp_8.Checked Then stateQuote.ComprehensiveCoverageAutoSymbolObject.HasSymbol8 = True Else stateQuote.ComprehensiveCoverageAutoSymbolObject.HasSymbol8 = False

            ' *** COLLISION ***
            If chk_Coll_2.Checked Then stateQuote.CollisionCoverageAutoSymbolObject.HasSymbol2 = True Else stateQuote.CollisionCoverageAutoSymbolObject.HasSymbol2 = False
            If chk_Coll_3.Checked Then stateQuote.CollisionCoverageAutoSymbolObject.HasSymbol3 = True Else stateQuote.CollisionCoverageAutoSymbolObject.HasSymbol3 = False
            If chk_Coll_4.Checked Then stateQuote.CollisionCoverageAutoSymbolObject.HasSymbol4 = True Else stateQuote.CollisionCoverageAutoSymbolObject.HasSymbol4 = False
            If chk_Coll_7.Checked Then stateQuote.CollisionCoverageAutoSymbolObject.HasSymbol7 = True Else stateQuote.CollisionCoverageAutoSymbolObject.HasSymbol7 = False
            If chk_Coll_8.Checked Then stateQuote.CollisionCoverageAutoSymbolObject.HasSymbol8 = True Else stateQuote.CollisionCoverageAutoSymbolObject.HasSymbol8 = False

            ' *** TOWING & LABOR
            If chk_Tow_7.Checked Then stateQuote.TowingAndLaborAutoSymbolObject.HasSymbol7 = True Else stateQuote.TowingAndLaborAutoSymbolObject.HasSymbol7 = False


        End If
    End Sub
End Class