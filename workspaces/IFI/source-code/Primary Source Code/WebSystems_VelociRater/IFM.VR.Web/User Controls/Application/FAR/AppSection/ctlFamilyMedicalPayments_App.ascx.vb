Imports QuickQuote.CommonObjects
Imports IFM.PrimativeExtensions

Public Class ctlFamilyMedicalPayments_App
    Inherits VRControlBase

    Public ReadOnly Property MyFarmLocation As QuickQuoteLocation
        Get
            If Me.Quote IsNot Nothing AndAlso Me.Quote.Locations IsNot Nothing AndAlso Me.Quote.Locations.Any() Then
                Return Me.Quote.Locations(0)
            End If
            Return Nothing
        End Get
    End Property

    Public Overrides Sub AddScriptAlways()
        ' **************  these names are used for the occupant dropdown under Updates ************
        Dim appArray As String = "var updates_applicantNames = new Array("
        Dim i As Int32 = 0
        If Me.Quote IsNot Nothing Then
            'Updated 9/10/18 for multi state MLW - Quote to GoverningStateQuote
            If Me.GoverningStateQuote IsNot Nothing Then
                For Each app In Me.GoverningStateQuote.Applicants
                    appArray += String.Format("'{0} {1}'", app.Name.FirstName.Replace("'", ""), app.Name.LastName.Replace("'", ""))
                    i += 1
                    If i < Me.GoverningStateQuote.Applicants.Count Then
                        appArray += ","
                    End If
                Next
            End If

        End If
        appArray += ");"
        Me.VRScript.AddVariableLine(appArray)
        ' **************  END ************
        Me.VRScript.AddVariableLine("var medicalPaymentNames_textboxIds = new Array();")
    End Sub

    Public Overrides Sub AddScriptWhenRendered()

    End Sub

    Public Overrides Sub LoadStaticData()

    End Sub

    Public Overrides Sub Populate()

        If MyFarmLocation?.SectionIICoverages IsNot Nothing Then
            Dim medicalPayments As QuickQuoteSectionIICoverage = (From cov In MyFarmLocation.SectionIICoverages Where cov.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments Select cov).FirstOrDefault()
            If medicalPayments IsNot Nothing Then
                If IsQuoteEndorsement() OrElse IsQuoteReadOnly() Then
                    Dim PersonalResidents As List(Of QuickQuoteResidentName) = New List(Of QuickQuoteResidentName)()
                    Dim CommercialResidents As List(Of QuickQuoteResidentName) = New List(Of QuickQuoteResidentName)()
                    Me.titleText.Visible = False
                    For Each resident As QuickQuoteResidentName In MyFarmLocation.ResidentNames
                        'If String.IsNullOrWhiteSpace(resident.Name.TaxTypeId) OrElse resident.Name.TaxTypeId.TryToGetInt32() = 1 Then
                        '    PersonalResidents.Add(resident)
                        'ElseIf resident.Name.TaxTypeId.TryToGetInt32() = 2 Then
                        '    CommercialResidents.Add(resident)
                        'End If

                        If resident.Name.TaxTypeId.TryToGetInt32() = 2 Then
                            CommercialResidents.Add(resident)
                        Else
                            PersonalResidents.Add(resident)
                        End If
                    Next
                    Me.divRecords.Visible = PersonalResidents IsNot Nothing AndAlso PersonalResidents.Any()
                    Me.Repeater1.DataSource = PersonalResidents
                    Me.Repeater1.DataBind()
                    Me.CommData.Visible = CommercialResidents IsNot Nothing AndAlso CommercialResidents.Any()
                    Me.RepeaterComm.DataSource = CommercialResidents
                    Me.RepeaterComm.DataBind()
                    Me.FindChildVrControls()
                    Dim PersonalIndex As Int32 = 0
                    Dim CommIndex As Int32 = 0

                    For Each c As VRControlBase In Me.ChildVrControls
                        If TypeOf c Is ctlResidentName_App Then
                            Dim r As ctlResidentName_App = DirectCast(c, ctlResidentName_App)
                            r.ResidentNameIndex = MyFarmLocation.ResidentNames.IndexOf(PersonalResidents(PersonalIndex))
                            r.Populate()
                            PersonalIndex += 1
                        ElseIf TypeOf c Is ctlResidentName_Commercial_App Then
                            Dim cr As ctlResidentName_Commercial_App = DirectCast(c, ctlResidentName_Commercial_App)
                            cr.ResidentNameIndex = MyFarmLocation.ResidentNames.IndexOf(CommercialResidents(CommIndex))
                            cr.Populate()
                            CommIndex += 1
                        End If

                    Next

                Else
                    Me.Repeater1.DataSource = MyFarmLocation.ResidentNames
                    Me.Repeater1.DataBind()
                    Me.divRecords.Visible = MyFarmLocation.ResidentNames IsNot Nothing AndAlso MyFarmLocation.ResidentNames.Any()
                    Me.FindChildVrControls()
                    Dim index As Int32 = 0
                    For Each c As VRControlBase In Me.ChildVrControls
                        If TypeOf c Is ctlResidentName_App Then
                            Dim r As ctlResidentName_App = DirectCast(c, ctlResidentName_App)
                            r.ResidentNameIndex = index
                            r.Populate()
                            index += 1
                        End If
                    Next
                End If
            Else
                Me.Visible = False
            End If
        Else
            Me.Visible = False
        End If

        PopulateChildControls()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Overrides Sub ValidateControl(valArgs As VRValidationArgs)
        If Me.Visible = True Then
            MyBase.ValidateControl(valArgs)
            Me.ValidateChildControls(valArgs)
        End If
    End Sub

    Public Overrides Function Save() As Boolean
        If Me.Visible = True Then
            If MyFarmLocation.SectionIICoverages IsNot Nothing Then
                '' update 'NumberOfPersonsReceivingCare' so that if they delete a record that will be reflected on the quote side
                Dim famMed As QuickQuoteSectionIICoverage = (From cov In MyFarmLocation.SectionIICoverages Where cov.CoverageType = QuickQuoteSectionIICoverage.SectionIICoverageType.Named_Persons_Medical_Payments Select cov).FirstOrDefault()
                If famMed IsNot Nothing Then
                    If Me.MyFarmLocation.ResidentNames IsNot Nothing Then
                        famMed.NumberOfPersonsReceivingCare = Me.MyFarmLocation.ResidentNames.Count.ToString()
                    Else
                        famMed.NumberOfPersonsReceivingCare = ""
                    End If

                End If
            End If

            SaveChildControls()
            Return True
        End If

    End Function

    Protected Sub lnkAdd_Click(sender As Object, e As EventArgs) Handles lnkAdd.Click
        Me.Save_FireSaveEvent(False)
        If Me.MyFarmLocation IsNot Nothing Then
            If Me.MyFarmLocation.ResidentNames Is Nothing Then
                Me.MyFarmLocation.ResidentNames = New List(Of QuickQuoteResidentName)()
            End If

            Me.divRecords.Visible = True

            If IsQuoteEndorsement() = False Then
                Me.MyFarmLocation.ResidentNames.Add(New QuickQuoteResidentName())
                Me.Populate()
                Me.Save_FireSaveEvent(False)
            Else
                Dim inx As Integer = Integer.Parse(MyFarmLocation.ResidentNames.Count)
                MyFarmLocation.ResidentNames.Add(New QuickQuoteResidentName)
                MyFarmLocation.ResidentNames(inx).Name.FirstName = "FIRSTN"
                MyFarmLocation.ResidentNames(inx).Name.LastName = "LASTN"
                MyFarmLocation.ResidentNames(inx).Name.BirthDate = "01/01/1900"
                MyFarmLocation.ResidentNames(inx).Name.Salutation = "SELF"
                Me.Save_FireSaveEvent(False)
                Me.Populate()
            End If

        End If
    End Sub

End Class