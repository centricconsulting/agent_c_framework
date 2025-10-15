Imports IFM.PrimativeExtensions
Imports IFM.VR.Flags
Imports IFM.ControlFlags
Imports QuickQuote.CommonObjects
Imports System.Configuration

Namespace IFM.VR.Common.UWQuestions

    Public Class UWQuestions
        Const OH_PPA_KIll_QUESTION_9296_9603_ADDITIONAL_TEXT As String = " *Ohio Only: Has any named insured driven without liability insurance during any part of the last six (6) months?"

        Private Shared qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
        Private Shared chc As New CommonHelperClass
        Private Shared _flags As List(Of IFeatureFlag)
        Private Sub New()

        End Sub

        Shared Sub New()
            _flags = New List(Of IFeatureFlag) From {New LOB.PPA}
        End Sub

        Public Shared Function GetKillQuestions(lobId As Int32, Optional ByVal effectiveDate As String = Nothing) As List(Of VRUWQuestion)
            Dim effDate = If(IsDate(effectiveDate), CDate(effectiveDate), IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate())
            Dim list As New List(Of VRUWQuestion)
            Dim lobType As QuickQuoteObject.QuickQuoteLobType = qqh.ConvertQQLobIdToQQLobType(lobId.ToString())

            Select Case lobType
                Case QuickQuoteObject.QuickQuoteLobType.Farm '"17"
                    'farm
                    Dim killQuestionCodes As New List(Of String) From {"9535", "9536", "9537", "9542", "9551"}
                    Return (From uw In GetFarmUnderwritingQuestions() Where killQuestionCodes.Contains(uw.PolicyUnderwritingCodeId) Select uw).ToList()
                Case QuickQuoteObject.QuickQuoteLobType.DwellingFirePersonal '"3"
                    'DFR
                    Dim killQuestionCodes As New List(Of String) From {"9415", "9421", "9423", "9428", "9433", "9436"}
                    If Not IFM.VR.Common.Helpers.DFR.DFRStandaloneHelper.isDFRStandaloneAvailable(effDate.ToShortDateString) Then
                        killQuestionCodes.Insert(1, "9419")
                    End If
                    'Updated 7/27/2022 for task 75803 MLW
                    'Dim uwQ = (From uw In GetDwellingFireUnderwritingQuestions() Where killQuestionCodes.Contains(uw.PolicyUnderwritingCodeId) Select uw).ToList()
                    Dim uwQ = (From uw In GetDwellingFireUnderwritingQuestions(effectiveDate) Where killQuestionCodes.Contains(uw.PolicyUnderwritingCodeId) Select uw).ToList()
                    For Each q In uwQ
                        q.IsTrueUwQuestion = True
                        If q.PolicyUnderwritingCodeId.EqualsAny("9423", "9428", "9433", "9436") Then
                            q.IsTrueKillQuestion = True
                        End If
                    Next
                    Return uwQ
                Case QuickQuoteObject.QuickQuoteLobType.CommercialGeneralLiability '"9"
                    'CGL
                    Dim killQuestionCodes As New List(Of String) From {"9345", "9346", "9347", "9348", "9349", "9350"}
                    Return (From uw In GetCGLUnderwritingQuestions() Where killQuestionCodes.Contains(uw.PolicyUnderwritingCodeId) Select uw).ToList()
                Case QuickQuoteObject.QuickQuoteLobType.CommercialBOP '"25"
                    'BOP
                    Dim killQuestionCodes As New List(Of String) From {"9003", "9006", "9007", "9008", "9009", "9400"}
                    Dim kq As List(Of VRUWQuestion) = (From uw In GetCommercialBOPUnderwritingQuestions() Where killQuestionCodes.Contains(uw.PolicyUnderwritingCodeId) Select uw).ToList()
                    ' Number the questions
                    Dim i As Integer = 0
                    For Each q As VRUWQuestion In kq
                        i += 1
                        q.Description = i.ToString & ". " & q.kqDescription
                    Next
                    Return kq
                    'Return (From uw In GetBOPUnderwritingQuestions() Where killQuestionCodes.Contains(uw.PolicyUnderwritingCodeId) Select uw).ToList()
                Case QuickQuoteObject.QuickQuoteLobType.CommercialAuto '"20"
                    ' CAP
                    Dim killQuestionCodes As New List(Of String) From {"9331", "9332", "9333", "9334", "9335", "9400"}
                    Dim kq As List(Of VRUWQuestion) = (From uw In GetCommercialCAPUnderwritingQuestions() Where killQuestionCodes.Contains(uw.PolicyUnderwritingCodeId) Select uw).ToList()
                    ' Number the questions
                    Dim i As Integer = 0
                    For Each q As VRUWQuestion In kq
                        i += 1
                        q.Description = i.ToString & ". " & q.kqDescription
                    Next
                    Return kq
                Case QuickQuoteObject.QuickQuoteLobType.HomePersonal '2
                    'HOM
                    'added 11/7/17 for HOM Upgrade MLW
                    Dim killQuestionCodes As New List(Of String) From {"9304", "9305", "9447", "9311", "9297"}
                    Return (From uw In GetPersonalHomeUnderwritingQuestions(effectiveDate, lobId) Where killQuestionCodes.Contains(uw.PolicyUnderwritingCodeId) Select uw).ToList()
                Case QuickQuoteObject.QuickQuoteLobType.WorkersCompensation '"21"
                    ' WCP
                    Dim killQuestionCodes As New List(Of String) From {"9341", "9086", "9342", "9343", "9344", "9107"}
                    If (IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(effDate)) Then
                        killQuestionCodes = New List(Of String) From {"9341", "9086", "9573", "9343", "9344", "9107"}
                    End If

                    Dim kq As List(Of VRUWQuestion) = (From uw In GetCommercialWCPUnderwritingQuestions(effectiveDate) Where killQuestionCodes.Contains(uw.PolicyUnderwritingCodeId) Select uw).ToList()
                    ' Number the questions
                    'Dim i As Integer = 0
                    'For Each q As VRUWQuestion In kq
                    '    i += 1
                    '    q.Description = i.ToString & ". " & q.kqDescription
                    'Next

                    ' Edit question 3 for KY WC MGB 4/16/19
                    If effectiveDate IsNot Nothing AndAlso IsDate(effectiveDate) AndAlso CDate(effectiveDate).Date > CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date Then
                        For Each q As VRUWQuestion In kq
                            If q.kqDescription.ToUpper.Contains("LIVE OUTSIDE THE STATE OF") Then
                                q.kqDescription = "Do any employees live outside the state of Indiana, Illinois, or Kentucky?"
                            End If
                        Next
                    End If

                    Return kq
                Case QuickQuoteObject.QuickQuoteLobType.CommercialProperty '28
                    ' CPR
                    Dim killQuestionCodes As New List(Of String) From {"9003", "9006", "9008", "9009", "9400"}
                    Dim kq As List(Of VRUWQuestion) = (From uw In GetCommercialCPRUnderwritingQuestions() Where killQuestionCodes.Contains(uw.PolicyUnderwritingCodeId) Select uw).ToList()
                    ' Number the questions
                    Dim i As Integer = 0
                    For Each q As VRUWQuestion In kq
                        i += 1
                        q.Description = i.ToString & ". " & q.kqDescription
                    Next
                    Return kq
                Case QuickQuoteObject.QuickQuoteLobType.CommercialPackage '"23"
                    ' CPP
                    Dim killQuestionCodes As New List(Of String) From {"9003", "9006", "9007", "9008", "9009", "9400"}
                    Dim kq As List(Of VRUWQuestion) = (From uw In GetCommercialCPPUnderwritingQuestions_CPR() Where killQuestionCodes.Contains(uw.PolicyUnderwritingCodeId) Select uw).ToList()
                    ' Number the questions
                    Dim i As Integer = 0
                    For Each q As VRUWQuestion In kq
                        i += 1
                        q.Description = i.ToString & ". " & q.kqDescription
                    Next
                    Return kq
                Case QuickQuoteObject.QuickQuoteLobType.AutoPersonal
                    'Updated 09/11/2019 for Bug 40312 MLW - removed 9295, question #13 "9295", 
                    Dim killQuestionCodes As New List(Of String) From {"9293", "9296", "9297"}
                    Dim kq As List(Of VRUWQuestion) = (From uw In GetPersonalAutoUnderwritingQuestions() Where killQuestionCodes.Contains(uw.PolicyUnderwritingCodeId) Select uw).ToList()
                    'Add Non Uw Question 
                    Dim IfmQuestion1 As New VRUWQuestion
                    IfmQuestion1.Description = "Any driver in the household over the age of 80 years old?"
                    IfmQuestion1.IsTrueUwQuestion = False
                    IfmQuestion1.IsTrueKillQuestion = True
                    kq.Insert(0, IfmQuestion1)
                    ' Number the questions
                    Dim i As Integer = 0
                    For Each q As VRUWQuestion In kq
                        'i += 1
                        'q.Description = i.ToString & ". " & q.kqDescription
                        If q.PolicyUnderwritingCodeId = "9297" Then 'question #15
                            q.IsTrueKillQuestion = True
                        End If

                        _flags.WithFlags(Of LOB.PPA) _
                              .When(Function(ppa) ppa.OhioEnabled) _
                              .Do(Sub()
                                      If q.PolicyUnderwritingCodeId = "9296" Then
                                          Dim configExists As Boolean
                                          Dim dateValue = chc.ConfigurationAppSettingValueAsString(LOB.PPA.OH_EARLIEST_AVAILABLE_EFFECTIVE_DATE, configExists)
                                          If configExists = False Then
                                              dateValue = "3/1/2022"
                                          End If
                                          If effDate >= qqh.DateForString(dateValue) Then
                                              q.Description += OH_PPA_KIll_QUESTION_9296_9603_ADDITIONAL_TEXT
                                          End If
                                      End If
                                  End Sub)
                    Next
                    Return kq
                Case Else

            End Select

            Return list
        End Function

        Public Shared Function GetPersonalAutoUnderwritingQuestions(Optional ByVal KillQuestionsOnly = False) As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)
            Dim item As New VRUWQuestion

            ' 10 12 14 15

            If Not KillQuestionsOnly Then
                item = New VRUWQuestion()
                item.QuestionNumber = 1
                item.Description = "1. With the exception of any encumbrances, are there any vehicles not solely owned by and registered to the applicant? (if yes, describe)"
                item.PolicyUnderwritingCodeId = "9283"
                item.IsTrueUwQuestion = True
                list.Add(item)
            End If

            If Not KillQuestionsOnly Then
                item = New VRUWQuestion()
                item.QuestionNumber = 2
                item.Description = "2. Any car modified/special equipment? (Include customized vans/pickup; indicate cost)"
                item.PolicyUnderwritingCodeId = "9284"
                item.IsTrueUwQuestion = True
                list.Add(item)
            End If

            If Not KillQuestionsOnly Then
                item = New VRUWQuestion()
                item.QuestionNumber = 3
                item.Description = "3. Any existing damage to the vehicle? (Include damaged glass)"
                item.PolicyUnderwritingCodeId = "9285"
                item.IsTrueUwQuestion = True
                list.Add(item)
            End If

            If Not KillQuestionsOnly Then
                item = New VRUWQuestion()
                item.QuestionNumber = 4
                item.Description = "4. Any other losses incurred (not shown in Accident/Conviction area?)"
                item.PolicyUnderwritingCodeId = "9286"
                item.IsTrueUwQuestion = True
                list.Add(item)
            End If

            If Not KillQuestionsOnly Then
                item = New VRUWQuestion()
                item.QuestionNumber = 5
                item.Description = "5. Any car kept at school?"
                item.PolicyUnderwritingCodeId = "9287"
                item.IsTrueUwQuestion = True
                list.Add(item)
            End If

            If Not KillQuestionsOnly Then
                item = New VRUWQuestion()
                item.QuestionNumber = 6
                item.Description = "6. Any car parked on the street?"
                item.PolicyUnderwritingCodeId = "9288"
                item.IsTrueUwQuestion = True
                list.Add(item)
            End If

            If Not KillQuestionsOnly Then
                item = New VRUWQuestion()
                item.QuestionNumber = 7
                item.Description = "7. Any other auto insurance in household? (Include any provided by employer)"
                item.PolicyUnderwritingCodeId = "9289"
                item.IsTrueUwQuestion = True
                list.Add(item)
            End If

            If Not KillQuestionsOnly Then
                item = New VRUWQuestion()
                item.QuestionNumber = 8
                item.Description = "8. Any other insurance with this company? (List Policy Number)"
                item.PolicyUnderwritingCodeId = "9290"
                item.IsTrueUwQuestion = True
                list.Add(item)
            End If

            If Not KillQuestionsOnly Then
                item = New VRUWQuestion()
                item.QuestionNumber = 9
                item.Description = "9. Any household member in military service? (Driver Number)"
                item.PolicyUnderwritingCodeId = "9291"
                item.IsTrueUwQuestion = True
                list.Add(item)
            End If

            item = New VRUWQuestion()
            item.QuestionNumber = 10
            item.Description = "10. Any driver’s license been suspended/revoked?"
            item.PolicyUnderwritingCodeId = "9292"
            item.IsTrueUwQuestion = True
            list.Add(item)

            If Not KillQuestionsOnly Then
                item = New VRUWQuestion()
                item.QuestionNumber = 11
                item.Description = "11. Any driver have physical/mental impairment? (List Driver number)"
                item.PolicyUnderwritingCodeId = "9293"
                item.IsTrueUwQuestion = True
                list.Add(item)
            End If

            item = New VRUWQuestion()
            item.QuestionNumber = 12
            item.Description = "12. Any financial responsibility filing? (Driver number and date of filing)"
            item.PolicyUnderwritingCodeId = "9294"
            item.IsTrueUwQuestion = True
            list.Add(item)

            If Not KillQuestionsOnly Then
                item = New VRUWQuestion()
                item.QuestionNumber = 13
                item.Description = "13. Has any insurance been transferred within agency?"
                item.PolicyUnderwritingCodeId = "9295"
                item.IsTrueUwQuestion = True
                list.Add(item)
            End If

            item = New VRUWQuestion()
            item.QuestionNumber = 14
            item.Description = "14. Any coverage declined, cancelled, or non-renewed during the last 3 years?"
            item.PolicyUnderwritingCodeId = "9296"
            item.IsTrueUwQuestion = True
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 15
            item.Description = "15. Is this brokered business to the agent?"
            item.PolicyUnderwritingCodeId = "9297"
            item.IsTrueUwQuestion = True
            list.Add(item)

            If Not KillQuestionsOnly Then
                item = New VRUWQuestion()
                item.QuestionNumber = 16
                item.Description = "16. Has agent inspected vehicle?"
                item.PolicyUnderwritingCodeId = "9298"
                item.IsTrueUwQuestion = True
                list.Add(item)
            End If

            Return list
        End Function

        Public Shared Function GetPersonalHomeUnderwritingQuestions(ByVal effectiveDate As String, ByVal lobId As Int32) As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)
            Dim item As New VRUWQuestion

            Dim qqh As New QuickQuote.CommonMethods.QuickQuoteHelperClass
            ' 10 12 14 15

            item = New VRUWQuestion()
            item.QuestionNumber = 1
            'item.Description = "1. Any farming or other business conducted on premises? (Including day/child care)"
            item.Description = "1. Any farming operations, farm animals, or other business conducted on premises? (Including day/child care)"
            'item.PolicyUnderwritingCodeId = "9324"
            item.PolicyUnderwritingCodeId = "9446"  ' Changed 11/10/14 MGB
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 2
            item.Description = "2. Any residence employees? (Number and type of full and part time employees)"
            item.PolicyUnderwritingCodeId = "9299"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 3
            item.Description = "3. Any flooding, brush, forest fire hazard, landslide, etc?"
            item.PolicyUnderwritingCodeId = "9300"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 4
            item.Description = "4. Any other residence owned, occupied or rented?"
            item.PolicyUnderwritingCodeId = "9301"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 5
            item.Description = "5. Any other insurance with this company? (List policy numbers)"
            item.PolicyUnderwritingCodeId = "9302"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 6
            item.Description = "6. Has insurance been transferred within agency?"
            item.PolicyUnderwritingCodeId = "9303"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 7
            item.Description = "7. Any coverage declined, cancelled, or non-renewed during the last three (3) years."
            item.PolicyUnderwritingCodeId = "9304"
            item.IsTrueUwQuestion = True 'added 11/14/17 for HOM Upgrade MLW
            item.IsTrueKillQuestion = False 'added 11/14/17 for HOM Upgrade MLW
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 8
            item.Description = "8. Has applicant had a foreclosure, repossession, bankruptcy, judgment or lien during the last five (5) years?"
            item.PolicyUnderwritingCodeId = "9305"
            item.IsTrueUwQuestion = True 'added 11/14/17 for HOM Upgrade MLW
            item.IsTrueKillQuestion = False 'added 11/14/17 for HOM Upgrade MLW
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 9
            'item.Description = "9. Are there any animals or exotic pets kept on premises? (Note breed and bite history)"
            'Updated 7/2/18 for Bug 27582 MLW - added Husky
            item.Description = "9. Are there any wild or exotic animals, animals with a known history of aggression or biting or any of the following dog breeds:  Akita, Pit Bull, Rottweiler, Husky, Wolf Hybrid, German Shepard, Chow, Doberman or a non-pure bred mix with any of the preceding breeds?"
            'item.PolicyUnderwritingCodeId = "9306"
            item.PolicyUnderwritingCodeId = "9447"  ' Changed 11/10/14 MGB
            'added 11/8/17 for HOM Upgrade MLW

            If qqh.doUseNewVersionOfLOB(effectiveDate, qqh.ConvertQQLobIdToQQLobType(lobId.ToString()), QuickQuote.CommonMethods.QuickQuoteHelperClass.LOBIFMVersions.HOM2018Upgrade, Convert.ToDateTime("7/1/2018")) Then
                item.IsTrueUwQuestion = True
                item.IsTrueKillQuestion = False
            Else
                item.IsTrueUwQuestion = True
                item.IsTrueKillQuestion = True
            End If

            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 10
            item.Description = "10. Distance to Tidal Water? (Miles or Feet)"
            item.PolicyUnderwritingCodeId = "9307"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 11
            item.Description = "11. Is property situated on more than five (5) acres? (If yes, describe land use)"
            item.PolicyUnderwritingCodeId = "9308"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 12
            item.Description = "12. Does applicant own any recreational vehicles (snow mobiles, dunebuggys, mini bikes, ATV's, etc)? (List year, type, make, model)"
            item.PolicyUnderwritingCodeId = "9309"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 13
            item.Description = "13. Is building retrofitted for earthquake? (if applicable)"
            item.PolicyUnderwritingCodeId = "9310"
            list.Add(item)

            'Kill Question - updated 11/7/2017 - MLW
            item = New VRUWQuestion()
            item.QuestionNumber = 14
            item.Description = "14. During the last five (5) years, has any applicant been indicted for or convicted of any degree of the crime of fraud, arson, or any other arson related crime in connection with this or any other property?"
            item.PolicyUnderwritingCodeId = "9311"
            item.IsTrueUwQuestion = True 'added 11/14/17 for HOM Upgrade MLW
            item.IsTrueKillQuestion = True
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 15
            item.Description = "15. Is there a manager on the premises? (Renters or Condos only)"
            item.PolicyUnderwritingCodeId = "9312"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 16
            item.Description = "16. Is there a security attendant? (Renters or Condos only)"
            item.PolicyUnderwritingCodeId = "9313"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 17
            item.Description = "17. Is the building entrance locked? (Renters or Condos only)"
            item.PolicyUnderwritingCodeId = "9314"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 18
            item.Description = "18. Any uncorrected fire or building code violations?"
            item.PolicyUnderwritingCodeId = "9315"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 19
            item.Description = "19. Is house for sale?"
            item.PolicyUnderwritingCodeId = "9316"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 20
            item.Description = "20. Is property within 300 feet of commercial or non-residential property?"
            item.PolicyUnderwritingCodeId = "9317"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 21
            item.Description = "21. Is there a trampoline on the premises?"
            item.PolicyUnderwritingCodeId = "9318"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 22
            item.Description = "22. Was the structure originally built for other than a private residence and then converted?"
            item.PolicyUnderwritingCodeId = "9319"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 23
            item.Description = "23. Any lead paint hazard?"
            item.PolicyUnderwritingCodeId = "9320"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 24
            item.Description = "24. If a fuel oil tank is on premises, has other insurance been obtained for the tank? (Give First Party and Limit, and Third party and Limit)"
            item.PolicyUnderwritingCodeId = "9321"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 25
            item.Description = "25. Is the building under construction or undergoing renovation or reconstruction? (Give estimated completion date and dollar value)"
            item.PolicyUnderwritingCodeId = "9322"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 26
            item.Description = "26. If building is under construction, is the applicant the general contractor?"
            item.PolicyUnderwritingCodeId = "9323"
            list.Add(item)

            'Kill Question - added 11/7/17 for HOM Upgrade MLW
            item = New VRUWQuestion()
            item.QuestionNumber = 27
            item.Description = "27. Is this brokered business to the agent?"
            item.PolicyUnderwritingCodeId = "9297"
            item.IsTrueUwQuestion = False
            item.IsTrueKillQuestion = True
            list.Add(item)

            Return list
        End Function

        ''' <summary>
        ''' DWELLING FIRE
        ''' PolicyUnderwritingCode id's 9415 to 9440
        ''' Added 10/15/15 MGB
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDwellingFireUnderwritingQuestions(ByVal effectiveDate As String) As List(Of VRUWQuestion) ' Updated 7/27/2022 for task 75803 MLW
        'Public Shared Function GetDwellingFireUnderwritingQuestions() As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)
            Dim item As New VRUWQuestion

            item = New VRUWQuestion()
            item.QuestionNumber = 1
            'item.Description = "1. Any farming or Other Business conducted on premises? (Including Day/Child Care)"
            item.Description = "Any farming or Other Business conducted on premises? (Including Day/Child Care) If ""Yes"", list gross receipts: $"
            item.PolicyUnderwritingCodeId = "9415"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 2
            item.Description = "2. Any residence employees? (Number and type of full and part time employees)"
            'item.Description = "Any residence employees? (Number and type of full and part time employees)"
            item.PolicyUnderwritingCodeId = "9416"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 3
            item.Description = "3. Any flooding, brush, forest fire hazard, landslides, etc?"
            'item.Description = "Any flooding, brush, forest fire hazard, landslides, etc?"
            item.PolicyUnderwritingCodeId = "9417"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 4
            item.Description = "4. Any other residence owned, occupied or rented?"
            'item.Description = "Any other residence owned, occupied or rented?"
            item.PolicyUnderwritingCodeId = "9418"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 5
            'Updated 7/26/2022 for task 75803 MLW
            If IFM.VR.Common.Helpers.DFR.DFRStandaloneHelper.isDFRStandaloneAvailable(effectiveDate) Then
                item.Description = "5. Any other insurance with this company? (List policy numbers)"
            Else
                item.Description = "5. Any other insurance with this company? (All Dwelling Fire quotes require an acceptable Personal Auto or Homeowners policy. List policy numbers)"
            End If
            ''item.Description = "5. Any other insurance with this company?" 'changed 12-9-2015 Matt A
            'item.Description = "5. Any other insurance with this company? (All Dwelling Fire quotes require an acceptable Personal Auto or Homeowners policy. List policy numbers)"
            ''item.Description = "Any other insurance with this company?"
            item.PolicyUnderwritingCodeId = "9419"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 6
            item.Description = "6. Has insurance been transferred within agency?"
            'item.Description = "Has insurance been transferred within agency?"
            item.PolicyUnderwritingCodeId = "9420"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 7
            item.Description = "7. Any coverage declined, cancelled, or non-renewed during the last 3 years?"
            'item.Description = "Any coverage declined, cancelled, or non-renewed during the last 3 years?"
            item.PolicyUnderwritingCodeId = "9421"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 8
            item.Description = "8. Has applicant had a foreclosure, repossession, bankruptcy, judgment or lien during the past 5 years?"
            'item.Description = "Has applicant had a foreclosure, repossession, bankruptcy, judgment or lien during the past 5 years?"
            item.PolicyUnderwritingCodeId = "9422"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 9
            item.Description = "9. Are there any wild or exotic animals, animals with a known history of aggression or biting or any of the following dog breeds:  Akita, Pit Bull, Rottweiler, Wolf Hybrid, German Shepard, Chow, Doberman or a non-pure bred mix with any of the preceding breeds?"
            'item.Description = "Are there any wild or exotic animals, animals with a known history of aggression or biting or any of the following dog breeds:  Pit Bull, Rottweiler, Wolf Hybrid, German Shepard, Chow, Doberman or a non-pure bred mix with any of the preceding breeds?"
            item.PolicyUnderwritingCodeId = "9423"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 10
            item.Description = "10. Is the property located within two miles of tidal water?"
            'item.Description = "Is the property located within two miles of tidal water?"
            item.PolicyUnderwritingCodeId = "9424"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 11
            item.Description = "11. Is property situated on more than five acres?"
            'item.Description = "Is property situated on more than five acres?"
            item.PolicyUnderwritingCodeId = "9425"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 12
            item.Description = "12. Does applicant own any recreational vehicles (snow mobiles, dune buggys, mini bikes, ATV's, etc)? List year, type, make, model"
            'item.Description = "Does applicant own any recreational vehicles (snow mobiles, dune buggys, mini bikes, ATV's, etc)? List year, type, make, model"
            item.PolicyUnderwritingCodeId = "9426"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 13
            item.Description = "13. Is building retrofitted for earthquake? (if applicable)"
            'item.Description = "Is building retrofitted for earthquake? (if applicable)"
            item.PolicyUnderwritingCodeId = "9427"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 14
            item.Description = "14. During the last five years, has any applicant been convicted of any degree of the crime of arson? (In RI, failure to disclose this existence of an arson conviction is a misdemeanor punishable by a sentence of up to one year of imprisonment.)"
            'item.Description = "During the last five years, has any applicant been convicted of any degree of the crime of arson? (In RI, failure to disclose this existence of an arson conviction is a misdemeanor punishable by a sentence of up to one year of imprisonment.)"
            item.PolicyUnderwritingCodeId = "9428"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 15
            item.Description = "15. Renters and Condos Only: Is there a manager on the premises?"
            'item.Description = "Renters and Condos Only: Is there a manager on the premises?"
            item.PolicyUnderwritingCodeId = "9429"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 16
            item.Description = "16. Renters and Condos Only: Is there a security attendant?"
            'item.Description = "Renters and Condos Only: Is there a security attendant?"
            item.PolicyUnderwritingCodeId = "9430"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 17
            item.Description = "17. Renters and Condos Only: Is the building entrance locked?"
            'item.Description = "Renters and Condos Only: Is the building entrance locked?"
            item.PolicyUnderwritingCodeId = "9431"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 18
            item.Description = "18. Any uncorrected fire or building violations?"
            'item.Description = "Any uncorrected fire or building violations?"
            item.PolicyUnderwritingCodeId = "9432"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 19
            item.Description = "19. Is the building undergoing renovation or reconstruction? (Give estimated completion date and dollar value)"
            'item.Description = "Is the building undergoing renovation or reconstruction? (Give estimated completion date and dollar value)"
            item.PolicyUnderwritingCodeId = "9433"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 20
            item.Description = "20. Is house for sale?"
            'item.Description = "Is house for sale?"
            item.PolicyUnderwritingCodeId = "9434"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 21
            item.Description = "21. Is property within 300 feet of commercial or non-residential property?"
            'item.Description = "Is property within 300 feet of commercial or non-residential property?"
            item.PolicyUnderwritingCodeId = "9435"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 22
            item.Description = "22. Is there a trampoline on the premises?  NOTE TO AGENT: Is there a swimming pool or wood burning stove on the premises?"
            'item.Description = "Is there a trampoline on the premises?  NOTE TO AGENT: Is there a swimming pool or wood burning stove on the premises?"
            item.PolicyUnderwritingCodeId = "9436"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 23
            item.Description = "23. Was the structure originally built for other than a private residence and then converted?"
            'item.Description = "Was the structure originally built for other than a private residence and then converted?"
            item.PolicyUnderwritingCodeId = "9437"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 24
            item.Description = "24. Any lead paint hazard?"
            'item.Description = "Any lead paint hazard?"
            item.PolicyUnderwritingCodeId = "9438"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 25
            item.Description = "25. If a fuel oil tank is on premises, has other insurance been obtained for the tank? (Give First Party and Limit, and Third party and Limit)"
            'item.Description = "If a fuel oil tank is on premises, has other insurance been obtained for the tank? (Give First Party and Limit, and Third party and Limit)"
            item.PolicyUnderwritingCodeId = "9439"
            list.Add(item)

            item = New VRUWQuestion()
            item.QuestionNumber = 26
            item.Description = "26. If building is under construction, is the applicant the general contractor?"
            'item.Description = "If building is under construction, is the applicant the general contractor?"
            item.PolicyUnderwritingCodeId = "9440"
            list.Add(item)

            Return list
        End Function

        Public Shared Function GetFarmUnderwritingQuestions() As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)

            ' Farm Kill Questions = 7,8,9,11 14, 14a, 14b

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Number of losses in last 3 years. Give date, kind of loss, insured and  amount paid.",
                        .PolicyUnderwritingCodeId = "9529",
                        .IsTrueUwQuestion = True
                     })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2.  Previous carrier",
                        .PolicyUnderwritingCodeId = "9530",
                        .IsTrueUwQuestion = True
                     })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. What is the principal type of farming?",
                        .PolicyUnderwritingCodeId = "9531",
                        .IsTrueUwQuestion = True
                     })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Farmed by owner? If no, describe who farms the premises:",
                        .PolicyUnderwritingCodeId = "9532",
                        .IsTrueUwQuestion = True
                     })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. How long has the applicant been farming?",
                        .PolicyUnderwritingCodeId = "9533",
                        .IsTrueUwQuestion = True
                     })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Does applicant have any other businesses other than farming?  If yes, describe:",
                        .PolicyUnderwritingCodeId = "9534",
                        .IsTrueUwQuestion = True
                     })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Has any policy been cancelled or non-renewed in the past 5 years? (If yes, explain)",
                        .PolicyUnderwritingCodeId = "9535",
                        .IsTrueUwQuestion = True
                     })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "8. Are there any vacant or unoccupied dwellings located on the premises? If yes, describe:",
                       .PolicyUnderwritingCodeId = "9536",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "9. Is any part of the farm used or leased for organized recreational use? This includes hunting for payment or pleasure, or the use of dirtbikes or ATVs for any purpose other than strictly farming use. (If yes, explain)",
                       .PolicyUnderwritingCodeId = "9537",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "10. Does insured do any custom farming? If yes, what are annual receipts?",
                       .PolicyUnderwritingCodeId = "9538",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "10a. If yes, describe the activities of the custom farming:",
                       .PolicyUnderwritingCodeId = "9539",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "10b. If yes, is coverage to be included for application of herbicides and pesticides?",
                       .PolicyUnderwritingCodeId = "9540",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "10c. If yes, does the applicant hold any permits required for application?",
                       .PolicyUnderwritingCodeId = "9541",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "11. Are the farm premises open to the public for activities such as roadside stand, auctions, Christmas tree farms, ""U-Pick"", any agritainment exposure such as pumpkin patch and associated activities, hay rides, or any spaces used or rented for special occasions? If yes, describe:",
                       .PolicyUnderwritingCodeId = "9542",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "12. Are any of the following items located on any of the premises insured?  Swimming pool? (If yes attach photo)",
                       .PolicyUnderwritingCodeId = "9543",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "12a. If yes, describe:",
                       .PolicyUnderwritingCodeId = "9544",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "12b. Above or in ground?",
                       .PolicyUnderwritingCodeId = "9545",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "12c. Depth at the deepest part of the pool?",
                       .PolicyUnderwritingCodeId = "9546",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "12d. Slide or diving board?",
                       .PolicyUnderwritingCodeId = "9547",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "12e. Fully fenced and locked or locking pool cover?",
                       .PolicyUnderwritingCodeId = "9548",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "13.Trampoline? (If yes attach photo)",
                       .PolicyUnderwritingCodeId = "9549",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "13a. If yes, is it fully netted and padded?",
                       .PolicyUnderwritingCodeId = "9550",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "14. Any dogs on premises?",
                       .PolicyUnderwritingCodeId = "9551",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "14a. If yes, how many?",
                       .PolicyUnderwritingCodeId = "9552",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "14b. Is any dog present either full or part breed of the following: Pit Bull, Rottweiler, Husky, Wolf Hybrid, German Shepherd, Chow, or Doberman If yes, describe:",
                       .PolicyUnderwritingCodeId = "9553",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "15. Power generation occurring on the premises (this includes wind, solar, digester, etc.)?",
                       .PolicyUnderwritingCodeId = "9554",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "15a. If yes, does the excess power sell back to the grid?",
                       .PolicyUnderwritingCodeId = "9555",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "16. Is there any fracking on premises?",
                       .PolicyUnderwritingCodeId = "9556",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "17. Any other hazard or operation not farming related? If yes, describe:",
                       .PolicyUnderwritingCodeId = "9557",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "18. Are the described insured premises the only premises which the applicant or spouse owns, rents or operates?",
                       .PolicyUnderwritingCodeId = "9558",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "19. Are there any horses on premises?",
                       .PolicyUnderwritingCodeId = "9559",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "19a. If yes, are they for personal use only with no revenue generation?",
                       .PolicyUnderwritingCodeId = "9560",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "19b. If revenue is generated please describe any of the following exposures: boarding, breeding, training, riding lessons, showing, racing, or any other equine related exposures.",
                       .PolicyUnderwritingCodeId = "9561",
                        .IsTrueUwQuestion = True
                    })

            list.Add(New VRUWQuestion() With {
                       .QuestionNumber = list.Count() + 1,
                       .Description = "20. Are there any supporting insurance policies in force with Indiana Farmers Insurance? If yes, please list:",
                       .PolicyUnderwritingCodeId = "9562",
                        .IsTrueUwQuestion = True
                    })

            Return list
        End Function

        Public Shared Function GetCGLUnderwritingQuestions() As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)

            '

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Any prior coverage declined, cancelled or non-renewed during the prior 3 years?",
                        .PolicyUnderwritingCodeId = "9345",
                        .IsTrueUwQuestion = True
                     })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9346",
                        .IsTrueUwQuestion = True
                     })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Do any operations include blasting or utilize or store explosive material?",
                        .PolicyUnderwritingCodeId = "9347",
                        .IsTrueUwQuestion = True
                     })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Are subcontractors allowed to work without providing you with a certificate of insurance?",
                        .PolicyUnderwritingCodeId = "9348",
                        .IsTrueUwQuestion = True
                     })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Does applicant lease equipment to others with or without operators?",
                        .PolicyUnderwritingCodeId = "9349",
                        .IsTrueUwQuestion = True
                     })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any products related to the aircraft or space industry?",
                        .PolicyUnderwritingCodeId = "9350",
                        .IsTrueUwQuestion = True
                     })

            Return list
        End Function

        Public Shared Function GetCommercialBOPUnderwritingQuestions() As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)

            ' KILL QUESTIONS
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "Any exposure to flammables, explosives, chemicals?",
            '            .PolicyUnderwritingCodeId = "9003",
            '            .IsTrueUwQuestion = True
            '         })

            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?",
            '            .PolicyUnderwritingCodeId = "9006",
            '            .IsTrueUwQuestion = True
            '         })

            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
            '            .PolicyUnderwritingCodeId = "9007",
            '            .IsTrueUwQuestion = True
            '         })

            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "During the last five years, has any applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?",
            '            .PolicyUnderwritingCodeId = "9008",
            '            .IsTrueUwQuestion = True,
            '            .IsTrueKillQuestion = True
            '         })

            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "Any uncorrected fire and/or safety code violations?",
            '            .PolicyUnderwritingCodeId = "9009",
            '            .IsTrueUwQuestion = True
            '         })

            ''list.Add(New VRUWQuestion() With {
            ''            .QuestionNumber = list.Count() + 1,
            ''            .Description = "Has applicant had a judgement or lien during the last five (5) years?",
            ''            .PolicyUnderwritingCodeId = "9010",
            ''            .IsTrueUwQuestion = True
            ''         })

            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy during the last five (5) years?",
            '            .PolicyUnderwritingCodeId = "9010",
            '            .IsTrueUwQuestion = True
            '         })

            ' All QUESTIONS

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1A. Is the Applicant a subsidiary of another entity?",
                        .PolicyUnderwritingCodeId = "9000",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1B. Does the Applicant have any subsidiaries?",
                        .PolicyUnderwritingCodeId = "9001",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is a formal safety program in operation?",
                        .PolicyUnderwritingCodeId = "9002",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            'KillQuestion
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Any exposure to flammables, explosives, chemicals?",
                        .PolicyUnderwritingCodeId = "9003",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any exposure to flammables, explosives, chemicals?"
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any other insurance with this company? (List Policy Numbers)",
                        .PolicyUnderwritingCodeId = "9005",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            'KillQuestion
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?",
                        .PolicyUnderwritingCodeId = "9006",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any policy or coverage declined, cancelled or non-renewed during the prior 3 years?"
                    })

            'KillQuestion
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9007",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
                    })

            'KillQuestion
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. During the last five years has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?",
                        .PolicyUnderwritingCodeId = "9008",
                        .IsTrueKillQuestion = True,
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "During the last 5 years, has any applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson, or any other arson-related crime in the connection with this or any other property?"
                    })

            'KillQuestion
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Any uncorrected fire and/or safety code violations?",
                        .PolicyUnderwritingCodeId = "9009",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any uncorrected fire code violations?"
                    })

            'KillQuestion
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy in the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9400",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any bankruptcies, tax or credit liens against the applicant in the past 5 years?"
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Has Applicant had a judgement or lien during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9010",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Has business been placed in a trust?",
                        .PolicyUnderwritingCodeId = "9011",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Any foreign operations, foreign products distributed in USA, or US products sold/distrubuted in foreign countries?",
                        .PolicyUnderwritingCodeId = "9012",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Does Applicant have other business ventures for which coverages is not requested?",
                        .PolicyUnderwritingCodeId = "9401",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Do/have past, present or discontinued operations involve(d) the storing, treating, discharging, applying, disposing, or transport of hazardous material (E.G. landfills, wastes, fuel tanks, etc)?",
                        .PolicyUnderwritingCodeId = "9109",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - General Info",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Are athletic teams sponsored?",
                        .PolicyUnderwritingCodeId = "9110",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - General Info",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Do you obtain and verify certificates of insurance obtained from subcontractors, manufacturers and/or suppliers?",
                        .PolicyUnderwritingCodeId = "9111",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - General Info",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Do you lease employees to or from other employers?",
                        .PolicyUnderwritingCodeId = "9114",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - General Info",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Do you own or operate any other business?",
                        .PolicyUnderwritingCodeId = "9116",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - General Info",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. In addition to your primary nature of your business are you involved in the manufacture, relabeling, or repackaging of others products?",
                        .PolicyUnderwritingCodeId = "9359",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - General Info",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. In addition to your primary nature of business, are you also involved in the mixing of others products?",
                        .PolicyUnderwritingCodeId = "9360",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - General Info",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Do you rent or loan equipment to others?",
                        .PolicyUnderwritingCodeId = "9119",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - General Info",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Does the operation have hours after 9:00 P.M. and/or 24 hour operations?",
                        .PolicyUnderwritingCodeId = "9361",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - General Info",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Does Applicant have a heating or processing boiler? (if yes indicate date of last inspection)",
                        .PolicyUnderwritingCodeId = "9125",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - Premises General Info",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Any specialized equipment such as medical equipment or other, valued over $100,000?",
                        .PolicyUnderwritingCodeId = "9127",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - Premises General Info",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Is all equipment inspected annually and well maintained?",
                        .PolicyUnderwritingCodeId = "9128",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - Premises General Info",
                        .IsQuestionRequired = False,
                        .NeverShowDescription = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Is there a swimming pool on premises?",
                        .PolicyUnderwritingCodeId = "9129",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - Premises General Info",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Is the building under construction?",
                        .PolicyUnderwritingCodeId = "9362",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - Premises General Info",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Is there a playground on premises?",
                        .PolicyUnderwritingCodeId = "9130",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - Apartments and Condos",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is aluminum wire used?",
                        .PolicyUnderwritingCodeId = "9131",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - Apartments and Condos",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Is a developer or contractor a board member?",
                        .PolicyUnderwritingCodeId = "9140",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - Apartments and Condos",
                        .IsQuestionRequired = True,
                        .NeverShowDescription = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Is a property manager employed?",
                        .PolicyUnderwritingCodeId = "9141",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - Apartments and Condos",
                        .IsQuestionRequired = True,
                        .NeverShowDescription = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Are there any incidental occupancies?",
                        .PolicyUnderwritingCodeId = "9607",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "8",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Business Owners - Non-ACORD Miscellaneous Underwriting Questions",
                        .IsQuestionRequired = True
                    })

            Return list
        End Function

        Public Shared Function GetCommercialCAPUnderwritingQuestions() As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)
            ' I just added the 5 kill questions for now MGB 6-19-17
            ' QUESTIONS 1-5 ARE THE INITIAL 'KILL QUESTIONS'

            ' Risk Grade Questions
            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Are vehicles operated beyond a 200 mile radius?",
                        .PolicyUnderwritingCodeId = "9331",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are vehicles operated beyond a 200 mile radius?"
                    })
            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Are vehicles engaged in the business of hauling logs or timber?",
                        .PolicyUnderwritingCodeId = "9332",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are vehicles engaged in the business of hauling logs or timber?"
                    })
            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Are vehicles rented, leased or loaned to others?",
                        .PolicyUnderwritingCodeId = "9333",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are vehicles rented, leased or loaned to others?"
                    })
            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Do operations involve transporting hazardous materials, including explosives, ammunition, natural or artificial fuels, fuel oil, oil, gasoline, propane, liquefied petroleum gas, gasoline or any other petroleum products?",
                        .PolicyUnderwritingCodeId = "9334",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do operations involve transporting hazardous materials, including explosives, ammunition, natural or artificial fuels, fuel oil, oil, gasoline, propane, liquefied petroleum gas, gasoline or any other petroleum products?"
                    })
            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any prior coverage declined, cancelled or non-renewed during the prior 3 years?",
                        .PolicyUnderwritingCodeId = "9335",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any prior coverage declined, cancelled or non-renewed during the prior 3 years?"
                    })

            ' Application Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1A. Is the Applicant a subsidiary of another entity?",
                        .PolicyUnderwritingCodeId = "9000",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1B. Does the Applicant have any subsidiaries?",
                        .PolicyUnderwritingCodeId = "9001",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is a formal safety program in operation?",
                        .PolicyUnderwritingCodeId = "9002",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Any exposure to flammables, explosives, chemicals?",
                        .PolicyUnderwritingCodeId = "9003",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any other insurance with this company? (List Policy Numbers)",
                        .PolicyUnderwritingCodeId = "9005",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?",
                        .PolicyUnderwritingCodeId = "9006",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9007",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. During the last five years has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?",
                        .PolicyUnderwritingCodeId = "9008",
                        .IsTrueKillQuestion = True,
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Any uncorrected fire and/or safety code violations?",
                        .PolicyUnderwritingCodeId = "9009",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True
                    })

            ' Kill question
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy in the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9400",
                        .IsTrueUwQuestion = True,
                        .IsTrueKillQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy in the last five (5) years?"
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Has Applicant had a judgement or lien during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9010",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Has business been placed in a trust?",
                        .PolicyUnderwritingCodeId = "9011",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Any foreign operations, foreign products distributed in USA, or US products sold/distrubuted in foreign countries?",
                        .PolicyUnderwritingCodeId = "9012",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Does Applicant have other business ventures for which coverages is not requested?",
                        .PolicyUnderwritingCodeId = "9401",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False
                    })

            ' Commercial Auto
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. With the exception of encumbrances, are any vehicles for which insurance is requested not solely owned by and registered to the Applicant?",
                        .PolicyUnderwritingCodeId = "9052",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Do over 50% of the employees use their autos in the business?",
                        .PolicyUnderwritingCodeId = "9053",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Is there a vehicle maintenance program in operation?",
                        .PolicyUnderwritingCodeId = "9054",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Are any vehicles leased to others?",
                        .PolicyUnderwritingCodeId = "9055",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any car modified / special equipment? (include customized vans / pickups)",
                        .PolicyUnderwritingCodeId = "9056",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Are ICC, PUC or other filings required? (if ""Yes"", attach ACORD 194)",
                        .PolicyUnderwritingCodeId = "9057",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Do operations involve transporting hazardous material?",
                        .PolicyUnderwritingCodeId = "9058",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Any hold harmless agreements?",
                        .PolicyUnderwritingCodeId = "9059",
                        .IsTrueKillQuestion = True,
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Any vehicles used by family members? if so, identify.",
                        .PolicyUnderwritingCodeId = "9060",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Does the Applicant obtain MVR verifications?",
                        .PolicyUnderwritingCodeId = "9061",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Does the Applicant have a specific driver recruiting method?",
                        .PolicyUnderwritingCodeId = "9062",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Are any drivers not covered by workers compensation?",
                        .PolicyUnderwritingCodeId = "9063",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Any vehicles owned but not scheduled on this application?",
                        .PolicyUnderwritingCodeId = "9064",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = True
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "14. Any drivers with convictions for moving traffic violations?",
                        .PolicyUnderwritingCodeId = "9065",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "15. Has agent inspected vehicles?",
                        .PolicyUnderwritingCodeId = "9066",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = False
                    })

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "16. Are all vehicles to be included in this policy part of a fleet?",
                        .PolicyUnderwritingCodeId = "9364",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "4",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Commercial Auto",
                        .IsQuestionRequired = False
                    })

            Return list
        End Function

        Public Shared Function GetCommercialWCPUnderwritingQuestions(effectiveDate As String) As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)

            ' I just added the 6 kill questions for now CH 7-24-17
            ' QUESTIONS ARE THE INITIAL 'KILL QUESTIONS'
            ' You should replace these when you create the whole list in order.

            ' Risk Grade Questions
            ' KILL QUESTION
            Dim lobHelper As New IFM.VR.Common.Helpers.LOBHelper(QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.WorkersCompensation)
            Dim effDate = If(IsDate(effectiveDate), CDate(effectiveDate), IFM.VR.Common.Helpers.GenericHelper.GetDiamondSystemDate())
            Dim governingStateString = lobHelper.AcceptableGoverningStatesAsString(effDate)

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Does Applicant own, operate or lease aircraft or watercraft?",
                        .PolicyUnderwritingCodeId = "9341",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant own, operate or lease aircraft or watercraft?"
                    })

            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Do/have past, present or discontinued operations involve(d) storing, treating, discharging, applying, disposing, or transporting of hazardous material? (e.g. landfills, wastes, fuel tanks, etc.)",
                        .PolicyUnderwritingCodeId = "9086",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do/have past, present or discontinued operations involve(d) storing, treating, discharging, applying, disposing, or transporting of hazardous material? (e.g. landfills, wastes, fuel tanks, etc.)"
                    })

            If (IFM.VR.Common.Helpers.MultiState.General.IsMultistateCapableEffectiveDate(effDate)) Then
                ' KILL QUESTION
                Dim dscr As String = $"3. Do any employees live outside the state(s) of {governingStateString}?"
                If effDate >= CDate(IFM.VR.Common.Helpers.MultiState.General.KentuckyWCPEffectiveDate).Date Then
                    ' Edit the kill question for Kentucky, needs to match the BRD text.  MGB 5-7-19
                    dscr = "3. Do any employees live outside the state(s) of Indiana, Illinois, or Kentucky?"
                End If
                list.Add(New VRUWQuestion() With {
                            .QuestionNumber = list.Count() + 1,
                            .Description = dscr,
                            .PolicyUnderwritingCodeId = "9573",
                            .IsTrueUwQuestion = True,
                            .PolicyUnderwritingTabId = "2",
                            .PolicyUnderwritingLevelId = "1",
                            .SectionName = "Risk Grade Questions",
                            .IsQuestionRequired = True,
                            .kqDescription = dscr
                        })
            Else
                ' KILL QUESTION
                list.Add(New VRUWQuestion() With {
                            .QuestionNumber = list.Count() + 1,
                            .Description = $"3. Do any employees live outside the state of {governingStateString}?",
                            .PolicyUnderwritingCodeId = "9342",
                            .IsTrueUwQuestion = True,
                            .PolicyUnderwritingTabId = "2",
                            .PolicyUnderwritingLevelId = "1",
                            .SectionName = "Risk Grade Questions",
                            .IsQuestionRequired = True,
                            .kqDescription = $"Do any employees live outside the state of {governingStateString}?"
                        })
            End If


            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any prior coverage declined, cancelled or non-renewed during the prior 3 years?",
                        .PolicyUnderwritingCodeId = "9343",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any prior coverage declined, cancelled or non-renewed during the prior 3 years?"
                    })

            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Is the Applicant involved in the operation of a professional employment organization, employee leasing operation, or temporary employment agency?",
                        .PolicyUnderwritingCodeId = "9344",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is the Applicant involved in the operation of a professional employment organization, employee leasing operation, or temporary employment agency?"
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Does Applicant own, operate or lease aircraft or watercraft?",
                        .PolicyUnderwritingCodeId = "9085",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Do/Have past, present or discontinued operations involve(d) storing, treating, discharging, applying, disposing, or transporting of hazardous material? (e.g. landfills, wastes, fuel tanks, etc.)",
                        .PolicyUnderwritingCodeId = "9363",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Any work performed underground or above 15 feet?",
                        .PolicyUnderwritingCodeId = "9087",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any work performed on barges, vessels, docks, bridge over water?",
                        .PolicyUnderwritingCodeId = "9088",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Is Applicant engaged in any other type of business?",
                        .PolicyUnderwritingCodeId = "9089",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Are sub-contractors used? (If ""Yes"", give % of work subcontracted)",
                        .PolicyUnderwritingCodeId = "9090",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Any work sublet without certificates of insurance? (If ""Yes"", payroll for this work must be included in the state rating worksheet on page 2)",
                        .PolicyUnderwritingCodeId = "9091",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Is a written safety program in operation?",
                        .PolicyUnderwritingCodeId = "9092",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = False
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Any group transportation provided?",
                        .PolicyUnderwritingCodeId = "9093",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Any employees under 16 or over 60 years of age?",
                        .PolicyUnderwritingCodeId = "9094",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Any seasonal employees?",
                        .PolicyUnderwritingCodeId = "9095",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Is there any volunteer or donated labor? (If ""Yes"", please specify)",
                        .PolicyUnderwritingCodeId = "9096",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = False
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Any employees with physical handicaps?",
                        .PolicyUnderwritingCodeId = "9097",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = False
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "14. Do employees travel out of state? (If ""Yes"", indicate state(s) of travel and frequency)",
                        .PolicyUnderwritingCodeId = "9098",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "15. Are athletic teams sponsored?",
                        .PolicyUnderwritingCodeId = "9099",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = False
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "16. Are physicals required after offers of employment are made?",
                        .PolicyUnderwritingCodeId = "9100",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = False
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "17. Any other insurance with this insurer?",
                        .PolicyUnderwritingCodeId = "9101",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "18. Any prior coverage declined/cancelled/non-renewed in the last three (3) years?",
                        .PolicyUnderwritingCodeId = "9102",
                        .IsTrueUwQuestion = True,
                        .IsTrueKillQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any prior coverage declined/cancelled/non-renewed in the last three (3) years?"
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "19. Are employee health plans provided?",
                        .PolicyUnderwritingCodeId = "9103",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = False
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "20. Do any employees perform work for other businesses or subsidiaries?",
                        .PolicyUnderwritingCodeId = "9104",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "21. Do you lease employees to or from other employers?",
                        .PolicyUnderwritingCodeId = "9105",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "22. Do any employees predominantly work at home? If ""Yes"", # of employees:",
                        .PolicyUnderwritingCodeId = "9106",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = False
                    })

            ' Workers Compensation Questions
            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "23. Any tax liens or bankruptcy within the last 5 years? (If ""Yes"", please specify)",
                        .PolicyUnderwritingCodeId = "9107",
                        .IsTrueUwQuestion = True,
                        .IsTrueKillQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any tax liens or bankruptcy within the last 5 years? (If ""Yes"", please specify)"
                    })

            ' Workers Compensation Questions
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "24. Any undisputed and unpaid workers compensation premium due from you or any commonly managed or owned enterprises? If yes, explain including entity name(s) and policy number(s).",
                        .PolicyUnderwritingCodeId = "9108",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Workers Compensation",
                        .IsQuestionRequired = True
                    })

            Return list
        End Function

        Public Shared Function GetCommercialCGLUnderwritingQuestions() As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)

            ' You should replace these when you create the whole list in order.

            ' Risk Grade Questions
            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Any prior coverage declined, cancelled or non-renewed during the prior 3 years?",
                        .PolicyUnderwritingCodeId = "9345",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any prior coverage declined, cancelled or non-renewed during the prior 3 years?"
                    })

            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9346",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
                    })

            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Do any operations include blasting or utilize or store explosive material?",
                        .PolicyUnderwritingCodeId = "9347",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do any operations include blasting or utilize or store explosive material?"
                    })

            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Are subcontractors allowed to work without providing you with a certificate of insurance?",
                        .PolicyUnderwritingCodeId = "9348",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are subcontractors allowed to work without providing you with a certificate of insurance?"
                    })

            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Does Applicant lease equipment to others with or without operators?",
                        .PolicyUnderwritingCodeId = "9349",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant lease equipment to others with or without operators?"
                    })

            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any products related to the aircraft or space industry?",
                        .PolicyUnderwritingCodeId = "9350",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any products related to the aircraft or space industry?"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1A. Is the Applicant a subsidiary of another entity?",
                        .PolicyUnderwritingCodeId = "9000",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is the Applicant a subsidiary of another entity?"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1B. Does the Applicant have any subsidiaries?",
                        .PolicyUnderwritingCodeId = "9001",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the Applicant have any subsidiaries?"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is a formal safety program in operation?",
                        .PolicyUnderwritingCodeId = "9002",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is a formal safety program in operation?"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Any exposure to flammables, explosives, chemicals?",
                        .PolicyUnderwritingCodeId = "9003",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any exposure to flammables, explosives, chemicals?"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any other insurance with this company? (List policy numbers)",
                        .PolicyUnderwritingCodeId = "9005",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any other insurance with this company? (List policy numbers)"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?",
                        .PolicyUnderwritingCodeId = "9006",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9007",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. During the last five years, has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?",
                        .PolicyUnderwritingCodeId = "9008",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "During the last five years, has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Any uncorrected fire and/or safety code violations?",
                        .PolicyUnderwritingCodeId = "9009",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any uncorrected fire and/or safety code violations?"
                    })

            ' KILL QUESTION
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9400",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy during the last five (5) years?"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Has Applicant had a judgement or lien during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9010",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Has Applicant had a judgement or lien during the last five (5) years?"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Has business been placed in a trust?",
                        .PolicyUnderwritingCodeId = "9011",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Has business been placed in a trust?"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?",
                        .PolicyUnderwritingCodeId = "9012",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?"
                    })

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Does Applicant have other business ventures for which coverages is not requested?",
                        .PolicyUnderwritingCodeId = "9401",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does Applicant have other business ventures for which coverages is not requested?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Any medical facilities provided or medical professionals employed or contracted?",
                        .PolicyUnderwritingCodeId = "9013",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any medical facilities provided or medical professionals employed or contracted?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Any exposure to radioactive/nuclear materials?",
                        .PolicyUnderwritingCodeId = "9014",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any exposure to radioactive/nuclear materials?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Do/have past, present or discontinued operations involve(d) storing, treating, discharging, applying, disposing, or transporting of hazardous material? (E.G. landfills, wastes, fuel tanks, etc)",
                        .PolicyUnderwritingCodeId = "9015",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do/have past, present or discontinued operations involve(d) storing, treating, discharging, applying, disposing, or transporting of hazardous material? (E.G. landfills, wastes, fuel tanks, etc)"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any operations sold, acquired or discounted in last 5 years?",
                        .PolicyUnderwritingCodeId = "9252",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any operations sold, acquired or discounted in last 5 years?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Machinery or equipment loaned or rented to others?",
                        .PolicyUnderwritingCodeId = "9017",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Machinery or equipment loaned or rented to others?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any watercraft, docks, floats owned, hired or leased?",
                        .PolicyUnderwritingCodeId = "9018",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any watercraft, docks, floats owned, hired or leased?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Any parking facilities owned/rented?",
                        .PolicyUnderwritingCodeId = "9019",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any parking facilities owned/rented?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Is a fee charged for parking?",
                        .PolicyUnderwritingCodeId = "9253",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is a fee charged for parking?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Recreation facilities provided?",
                        .PolicyUnderwritingCodeId = "9021",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Recreation facilities provided?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Is there a swimming pool on the premises?",
                        .PolicyUnderwritingCodeId = "9022",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is there a swimming pool on the premises?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Sporting or social events sponsored?",
                        .PolicyUnderwritingCodeId = "9023",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Sporting or social events sponsored?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Any structural alterations contemplated?",
                        .PolicyUnderwritingCodeId = "9024",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any structural alterations contemplated?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Any demolition exposure contemplated?",
                        .PolicyUnderwritingCodeId = "9025",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any demolition exposure contemplated?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "14. Has Applicant been active in or is currently active in joint ventures?",
                        .PolicyUnderwritingCodeId = "9026",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Has Applicant been active in or is currently active in joint ventures?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "15. Do you lease employees to or from other employers?",
                        .PolicyUnderwritingCodeId = "9027",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you lease employees to or from other employers?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "16. Is there a labor interchange with any other business or subsidiaries?",
                        .PolicyUnderwritingCodeId = "9028",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is there a labor interchange with any other business or subsidiaries?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "17. Are day care facilities operated or controlled?",
                        .PolicyUnderwritingCodeId = "9029",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are day care facilities operated or controlled?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "18. Have any crimes occurred or been attempted on your premises within the last three years?",
                        .PolicyUnderwritingCodeId = "9254",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Have any crimes occurred or been attempted on your premises within the last three years?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "19. Is there a formal, written safety and security policy in effect?",
                        .PolicyUnderwritingCodeId = "9255",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is there a formal, written safety and security policy in effect?"
                    })

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "20. Does the businesses’ promotional literature make any representations about the safety or security of the premises?",
                        .PolicyUnderwritingCodeId = "9256",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "5",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the businesses’ promotional literature make any representations about the safety or security of the premises?"
                    })

            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Does Applicant draw plans, designs, or specifications for others?",
                        .PolicyUnderwritingCodeId = "9257",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant draw plans, designs, or specifications for others?"
                    })

            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Do any operations include blasting or utilize or store explosive materials?",
                        .PolicyUnderwritingCodeId = "9258",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do any operations include blasting or utilize or store explosive materials?"
                    })

            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Do any operations include excavation, tunneling, underground work or earth moving?",
                        .PolicyUnderwritingCodeId = "9259",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do any operations include excavation, tunneling, underground work or earth moving?"
                    })

            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Do your subcontractors carry coverages or limits less than yours?",
                        .PolicyUnderwritingCodeId = "9260",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do your subcontractors carry coverages or limits less than yours?"
                    })

            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Are subcontractors allowed to work without providing you with a certificate of insurance?",
                        .PolicyUnderwritingCodeId = "9261",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are subcontractors allowed to work without providing you with a certificate of insurance?"
                    })

            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Does Applicant lease equipment to others with or without operators?",
                        .PolicyUnderwritingCodeId = "9038",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant lease equipment to others with or without operators?"
                    })

            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Does Applicant install, service or demonstrate products?",
                        .PolicyUnderwritingCodeId = "9042",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant install, service or demonstrate products?"
                    })

            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Foreign products sold, distributed, used as components?",
                        .PolicyUnderwritingCodeId = "9043",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Foreign products sold, distributed, used as components?"
                    })

            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Research and development conducted or new products planned?",
                        .PolicyUnderwritingCodeId = "9044",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = False,
                        .kqDescription = "Research and development conducted or new products planned?"
                    })

            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Guarantees, warranties, hold harmless agreements?",
                        .PolicyUnderwritingCodeId = "9045",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = False,
                        .kqDescription = "Guarantees, warranties, hold harmless agreements?"
                    })

            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Products related to aircraft/space industry?",
                        .PolicyUnderwritingCodeId = "9046",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Products related to aircraft/space industry?"
                    })

            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Products recalled, discontinued, changed?",
                        .PolicyUnderwritingCodeId = "9047",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Products recalled, discontinued, changed?"
                    })

            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Products of others sold or re-packaged under Applicant label?",
                        .PolicyUnderwritingCodeId = "9048",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Products of others sold or re-packaged under Applicant label?"
                    })

            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Products under label of others?",
                        .PolicyUnderwritingCodeId = "9049",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Products under label of others?"
                    })

            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Vendors coverage required?",
                        .PolicyUnderwritingCodeId = "9050",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = False,
                        .kqDescription = "Vendors coverage required?"
                    })

            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Does any named insured sell to other named insured?",
                        .PolicyUnderwritingCodeId = "9051",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does any named insured sell to other named insured?"
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. List all employee benefit programs (EBPS) offered to your employees and the provider of these programs. Include any programs that are optional to your employees.",
                        .PolicyUnderwritingCodeId = "9262",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "List all employee benefit programs (EBPS) offered to your employees and the provider of these programs. Include any programs that are optional to your employees."
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Which programs do you self-fund or partially self-fund? List and describe how you fund these programs.",
                        .PolicyUnderwritingCodeId = "9263",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Which programs do you self-fund or partially self-fund? List and describe how you fund these programs."
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Are you planning to add additional EBPS? If yes, please describe.",
                        .PolicyUnderwritingCodeId = "9264",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are you planning to add additional EBPS? If yes, please describe."
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Are you planning to terminate or modify any existing EBPS? If yes, list which programs and what you plan to terminate/modify.",
                        .PolicyUnderwritingCodeId = "9265",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are you planning to terminate or modify any existing EBPS? If yes, list which programs and what you plan to terminate/modify."
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Have you changed providers for any program within the past year? If yes, which program changed and what were the provider changes made?",
                        .PolicyUnderwritingCodeId = "9266",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Have you changed providers for any program within the past year? If yes, which program changed and what were the provider changes made?"
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Do you plan to change providers for any program within the next year? If yes, please describe.",
                        .PolicyUnderwritingCodeId = "9267",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you plan to change providers for any program within the next year? If yes, please describe."
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Are your employees eligible for the EBPS? How many? Please specify full time, part time and retirees.",
                        .PolicyUnderwritingCodeId = "9268",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are your employees eligible for the EBPS? How many? Please specify full time, part time and retirees."
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Do you have a trained staff to administer the EBPS, answer questions, and advise employees concerning these programs? If yes, how many employees are in this department?",
                        .PolicyUnderwritingCodeId = "9269",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you have a trained staff to administer the EBPS, answer questions, and advise employees concerning these programs? If yes, how many employees are in this department?"
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8B. What training and experience do these individuals have?",
                        .PolicyUnderwritingCodeId = "9270",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "What training and experience do these individuals have?"
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8C. Are the individuals who handle assets of the plans bonded as required by ERISA?",
                        .PolicyUnderwritingCodeId = "9271",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are the individuals who handle assets of the plans bonded as required by ERISA?"
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Do you provide a published EBP manual or booklet to all employees which clearly details how the program works?",
                        .PolicyUnderwritingCodeId = "9272",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you provide a published EBP manual or booklet to all employees which clearly details how the program works?"
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9B. Is it updated to reflect subsequent changes?",
                        .PolicyUnderwritingCodeId = "9273",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is it updated to reflect subsequent changes?"
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9C. Is manual reviewed with each new employee by a trained human resources person? Please include a copy.",
                        .PolicyUnderwritingCodeId = "9274",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is manual reviewed with each new employee by a trained human resources person? Please include a copy."
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Do you require, and keep on file, signed documentation from employees noting acceptance or rejection of optional benefits?",
                        .PolicyUnderwritingCodeId = "9275",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you require, and keep on file, signed documentation from employees noting acceptance or rejection of optional benefits?"
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Have you had a claim for this coverage within the past five years, or are you aware of a situation which could lead to a possible claim?",
                        .PolicyUnderwritingCodeId = "9276",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Have you had a claim for this coverage within the past five years, or are you aware of a situation which could lead to a possible claim?"
                    })

            Return list
        End Function

        Public Shared Function GetCommercialCPRUnderwritingQuestions() As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)

            ' Risk Grade Questions -- Not used.
            '' KILL QUESTION
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1. Any exposure to flammables, explosives, chemicals?",
            '            .PolicyUnderwritingCodeId = "9325",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Risk Grade Questions",
            '            .IsQuestionRequired = True,
            '            .kqDescription = "Any exposure to flammables, explosives, chemicals?"
            '        })

            '' KILL QUESTION
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Any prior coverage declined, cancelled or non-renewed during the prior 3 years?",
            '            .PolicyUnderwritingCodeId = "9326",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Risk Grade Questions",
            '            .IsQuestionRequired = True,
            '            .kqDescription = "Any prior coverage declined, cancelled or non-renewed during the prior 3 years?"
            '        })

            '' KILL QUESTION
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "3. During the last 5 years, has any applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?",
            '            .PolicyUnderwritingCodeId = "9327",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Risk Grade Questions",
            '            .IsQuestionRequired = True,
            '            .kqDescription = "During the last 5 years, has any applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?"
            '        })

            '' KILL QUESTION
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. Any uncorrected fire code violations?",
            '            .PolicyUnderwritingCodeId = "9328",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Risk Grade Questions",
            '            .IsQuestionRequired = True,
            '            .kqDescription = "Any uncorrected fire code violations?"
            '        })

            '' KILL QUESTION - Removed (for Multi-State?)
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Any locations located outside the state of indiana?",
            '            .PolicyUnderwritingCodeId = "9329",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Risk Grade Questions",
            '            .IsQuestionRequired = True,
            '            .kqDescription = "Any locations located outside the state of indiana?"
            '        })

            '' KILL QUESTION
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Any bankruptcies, tax or credit liens against the applicant in the past 5 years?",
            '            .PolicyUnderwritingCodeId = "9330",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Risk Grade Questions",
            '            .IsQuestionRequired = True,
            '            .kqDescription = "Any bankruptcies, tax or credit liens against the applicant in the past 5 years?"
            '        })

            'Application Information

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1A. Is the applicant a subsidiary of another entity?",
                        .PolicyUnderwritingCodeId = "9000",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1B. Does the applicant have any subsidiaries?",
                        .PolicyUnderwritingCodeId = "9001",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is a formal safety program in operation?",
                        .PolicyUnderwritingCodeId = "9002",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                        })

            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Any exposure to flammables, explosives, chemicals?",
                        .PolicyUnderwritingCodeId = "9003",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any exposure to flammables, explosives, chemicals?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any other insurance With this company? (list policy numbers)",
                        .PolicyUnderwritingCodeId = "9005",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                        })

            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?",
                        .PolicyUnderwritingCodeId = "9006",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any prior coverage declined, cancelled or non-renewed during the prior 3 years?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9007",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True
                        })

            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. During the last five years, has any applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?",
                        .PolicyUnderwritingCodeId = "9008",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True,
                        .IsTrueKillQuestion = True,
                        .kqDescription = "During the last 5 years, has any applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?"
                        })

            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Any uncorrected fire And/or safety code violations?",
                        .PolicyUnderwritingCodeId = "9009",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any uncorrected fire code violations?"
                        })

            ' KILL QUESTION
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Has applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9400",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any bankruptcies, tax or credit liens against the applicant in the past 5 years?"
                        })

            ' KILL QUESTION - set programatically
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Has applicant had a judgement or lien during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9010",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Has business been placed in a trust?",
                        .PolicyUnderwritingCodeId = "9011",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?",
                        .PolicyUnderwritingCodeId = "9012",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Does applicant have other business ventures for which coverage Is Not requested?",
                        .PolicyUnderwritingCodeId = "9401",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                        })

            Return list
        End Function

        Public Shared Function GetCommercialCPPUnderwritingQuestions_All() As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)
            ' How is this going to work since there are several questions that are repeated such as 9000
            '3349 - 6530
            'Question 1:  - 9003
            '	Package -9403
            '   Others -9003
            'Question 2:  - 9006
            '	All -9006
            'Question 3:  - 9007
            '	Some -9007
            '   CGL -9346
            'Question 4:  - 9008
            '	All -9008
            'Question 5:  - 9009
            '	All -9009
            'Question 6:  - 9400 & 9010
            '	Some -9400
            '   Crime-Risk Grade questions - 9354

#Region "Property list"
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1A. Is the Applicant a subsidiary of another entity?",
                        .PolicyUnderwritingCodeId = "9000",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = False
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1B. Does the Applicant have any subsidiaries?",
                        .PolicyUnderwritingCodeId = "9001",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = False
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is a formal safety program in operation?",
                        .PolicyUnderwritingCodeId = "9002",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = False
                        })

            'Kill Question
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Any exposure to flammables, explosives, chemicals?",
                        .PolicyUnderwritingCodeId = "9003",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any exposure to flammables, explosives, chemicals?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any other insurance With this company? (list policy numbers)",
                        .PolicyUnderwritingCodeId = "9005",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = False
                        })

            'Kill Question
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?",
                        .PolicyUnderwritingCodeId = "9006",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any prior coverage declined, cancelled or non-renewed during the prior 3 years?"
                        })

            'Kill Question
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9007",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
                        })

            '.IsTrueKillQuestion = True,
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. During the last five years, has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?",
                        .PolicyUnderwritingCodeId = "9008",
                        .IsTrueUwQuestion = True,
                        .IsTrueKillQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "During the last 5 years, has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?"
                        })

            'Kill Question
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Any uncorrected fire And/or safety code violations?",
                        .PolicyUnderwritingCodeId = "9009",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any uncorrected fire code violations?"
                        })

            'Kill Question
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9400",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any bankruptcies, tax or credit liens against the Applicant in the past 5 years?"
                        })

            'set programatically - same as 9400 above (#9)
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Has Applicant had a judgement or lien during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9010",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Has business been placed in a trust?",
                        .PolicyUnderwritingCodeId = "9011",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = False
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?",
                        .PolicyUnderwritingCodeId = "9012",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = False
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Does Applicant have other business ventures for which coverage is Not requested?",
                        .PolicyUnderwritingCodeId = "9401",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CPR_Application Information",
                        .IsQuestionRequired = False
                        })
#End Region

#Region "General Liability List"
            ' Risk Grade Questions

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Any prior coverage declined, cancelled or non-renewed during the prior 3 years?",
                        .PolicyUnderwritingCodeId = "9345",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any prior coverage declined, cancelled or non-renewed during the prior 3 years?"
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9346",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Do any operations include blasting or utilize or store explosive material?",
                        .PolicyUnderwritingCodeId = "9347",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do any operations include blasting or utilize or store explosive material?"
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Are subcontractors allowed to work without providing you with a certificate of insurance?",
                        .PolicyUnderwritingCodeId = "9348",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are subcontractors allowed to work without providing you with a certificate of insurance?"
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Does Applicant lease equipment to others with or without operators?",
                        .PolicyUnderwritingCodeId = "9349",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant lease equipment to others with or without operators?"
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any products related to the aircraft or space industry?",
                        .PolicyUnderwritingCodeId = "9350",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any products related to the aircraft or space industry?"
                    })

#Region "Unused Applicant Information"
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1A. Is the Applicant a subsidiary of another entity?",
            '            .PolicyUnderwritingCodeId = "9000",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is the Applicant a subsidiary of another entity?"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1B. Does the Applicant have any subsidiaries?",
            '            .PolicyUnderwritingCodeId = "9001",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Does the Applicant have any subsidiaries?"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Is a formal safety program in operation?",
            '            .PolicyUnderwritingCodeId = "9002",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is a formal safety program in operation?"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "3. Any exposure to flammables, explosives, chemicals?",
            '            .PolicyUnderwritingCodeId = "9003",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any exposure to flammables, explosives, chemicals?"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. Any other insurance with this company? (List policy numbers)",
            '            .PolicyUnderwritingCodeId = "9005",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any other insurance with this company? (List policy numbers)"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?",
            '            .PolicyUnderwritingCodeId = "9006",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "6. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
            '            .PolicyUnderwritingCodeId = "9007",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "7. During the last five years, has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?",
            '            .PolicyUnderwritingCodeId = "9008",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "During the last five years, has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "8. Any uncorrected fire and/or safety code violations?",
            '            .PolicyUnderwritingCodeId = "9009",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any uncorrected fire and/or safety code violations?"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "9. Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy during the last five (5) years?",
            '            .PolicyUnderwritingCodeId = "9400",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = True,
            '            .kqDescription = "Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy during the last five (5) years?"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "10. Has Applicant had a judgement or lien during the last five (5) years?",
            '            .PolicyUnderwritingCodeId = "9010",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Has Applicant had a judgement or lien during the last five (5) years?"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "11. Has business been placed in a trust?",
            '            .PolicyUnderwritingCodeId = "9011",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Has business been placed in a trust?"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "12. Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?",
            '            .PolicyUnderwritingCodeId = "9012",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?"
            '        })
            '' Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "13. Does Applicant have other business ventures for which coverages is not requested?",
            '            .PolicyUnderwritingCodeId = "9401",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Liability-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Does Applicant have other business ventures for which coverages is not requested?"
            '        })
#End Region

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Any medical facilities provided or medical professionals employed or contracted?",
                        .PolicyUnderwritingCodeId = "9013",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any medical facilities provided or medical professionals employed or contracted?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Any exposure to radioactive/nuclear materials?",
                        .PolicyUnderwritingCodeId = "9014",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any exposure to radioactive/nuclear materials?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Do/have past, present or discontinued operations involve(d) storing, treating, discharging, applying, disposing, or transporting of hazardous material? (E.G. landfills, wastes, fuel tanks, etc)",
                        .PolicyUnderwritingCodeId = "9015",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do/have past, present or discontinued operations involve(d) storing, treating, discharging, applying, disposing, or transporting of hazardous material? (E.G. landfills, wastes, fuel tanks, etc)"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any operations sold, acquired or discounted in last 5 years?",
                        .PolicyUnderwritingCodeId = "9252",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any operations sold, acquired or discounted in last 5 years?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Machinery or equipment loaned or rented to others?",
                        .PolicyUnderwritingCodeId = "9017",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Machinery or equipment loaned or rented to others?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any watercraft, docks, floats owned, hired or leased?",
                        .PolicyUnderwritingCodeId = "9018",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any watercraft, docks, floats owned, hired or leased?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Any parking facilities owned/rented?",
                        .PolicyUnderwritingCodeId = "9019",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any parking facilities owned/rented?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Is a fee charged for parking?",
                        .PolicyUnderwritingCodeId = "9253",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is a fee charged for parking?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Recreation facilities provided?",
                        .PolicyUnderwritingCodeId = "9021",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Recreation facilities provided?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Is there a swimming pool on the premises?",
                        .PolicyUnderwritingCodeId = "9022",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is there a swimming pool on the premises?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Sporting or social events sponsored?",
                        .PolicyUnderwritingCodeId = "9023",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Sporting or social events sponsored?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Any structural alterations contemplated?",
                        .PolicyUnderwritingCodeId = "9024",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any structural alterations contemplated?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Any demolition exposure contemplated?",
                        .PolicyUnderwritingCodeId = "9025",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any demolition exposure contemplated?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "14. Has Applicant been active in or is currently active in joint ventures?",
                        .PolicyUnderwritingCodeId = "9026",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Has Applicant been active in or is currently active in joint ventures?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "15. Do you lease employees to or from other employers?",
                        .PolicyUnderwritingCodeId = "9027",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you lease employees to or from other employers?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "16. Is there a labor interchange with any other business or subsidiaries?",
                        .PolicyUnderwritingCodeId = "9028",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is there a labor interchange with any other business or subsidiaries?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "17. Are day care facilities operated or controlled?",
                        .PolicyUnderwritingCodeId = "9029",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are day care facilities operated or controlled?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "18. Have any crimes occurred or been attempted on your premises within the last three years?",
                        .PolicyUnderwritingCodeId = "9254",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Have any crimes occurred or been attempted on your premises within the last three years?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "19. Is there a formal, written safety and security policy in effect?",
                        .PolicyUnderwritingCodeId = "9255",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is there a formal, written safety and security policy in effect?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "20. Does the businesses’ promotional literature make any representations about the safety or security of the premises?",
                        .PolicyUnderwritingCodeId = "9256",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the businesses’ promotional literature make any representations about the safety or security of the premises?"
                    })

            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Does Applicant draw plans, designs, or specifications for others?",
                        .PolicyUnderwritingCodeId = "9257",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant draw plans, designs, or specifications for others?"
                    })
            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Do any operations include blasting or utilize or store explosive materials?",
                        .PolicyUnderwritingCodeId = "9258",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do any operations include blasting or utilize or store explosive materials?"
                    })
            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Do any operations include excavation, tunneling, underground work or earth moving?",
                        .PolicyUnderwritingCodeId = "9259",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do any operations include excavation, tunneling, underground work or earth moving?"
                    })
            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Do your subcontractors carry coverages or limits less than yours?",
                        .PolicyUnderwritingCodeId = "9260",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do your subcontractors carry coverages or limits less than yours?"
                    })
            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Are subcontractors allowed to work without providing you with a certificate of insurance?",
                        .PolicyUnderwritingCodeId = "9261",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are subcontractors allowed to work without providing you with a certificate of insurance?"
                    })
            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Does Applicant lease equipment to others with or without operators?",
                        .PolicyUnderwritingCodeId = "9038",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant lease equipment to others with or without operators?"
                    })

            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Does Applicant install, service or demonstrate products?",
                        .PolicyUnderwritingCodeId = "9042",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant install, service or demonstrate products?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Foreign products sold, distributed, used as components?",
                        .PolicyUnderwritingCodeId = "9043",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Foreign products sold, distributed, used as components?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Research and development conducted or new products planned?",
                        .PolicyUnderwritingCodeId = "9044",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Products/Completed Operations",
                        .IsQuestionRequired = False,
                        .kqDescription = "Research and development conducted or new products planned?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Guarantees, warranties, hold harmless agreements?",
                        .PolicyUnderwritingCodeId = "9045",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Products/Completed Operations",
                        .IsQuestionRequired = False,
                        .kqDescription = "Guarantees, warranties, hold harmless agreements?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Products related to aircraft/space industry?",
                        .PolicyUnderwritingCodeId = "9046",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Products related to aircraft/space industry?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Products recalled, discontinued, changed?",
                        .PolicyUnderwritingCodeId = "9047",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Products recalled, discontinued, changed?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Products of others sold or re-packaged under Applicant label?",
                        .PolicyUnderwritingCodeId = "9048",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Products of others sold or re-packaged under Applicant label?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Products under label of others?",
                        .PolicyUnderwritingCodeId = "9049",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Products under label of others?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Vendors coverage required?",
                        .PolicyUnderwritingCodeId = "9050",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Products/Completed Operations",
                        .IsQuestionRequired = False,
                        .kqDescription = "Vendors coverage required?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Does any named insured sell to other named insured?",
                        .PolicyUnderwritingCodeId = "9051",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_General Liability - Products/Completed Operations",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does any named insured sell to other named insured?"
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. List all employee benefit programs (EBPS) offered to your employees and the provider of these programs. Include any programs that are optional to your employees.",
                        .PolicyUnderwritingCodeId = "9262",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "List all employee benefit programs (EBPS) offered to your employees and the provider of these programs. Include any programs that are optional to your employees."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Which programs do you self-fund or partially self-fund? List and describe how you fund these programs.",
                        .PolicyUnderwritingCodeId = "9263",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Which programs do you self-fund or partially self-fund? List and describe how you fund these programs."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Are you planning to add additional EBPS? If yes, please describe.",
                        .PolicyUnderwritingCodeId = "9264",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are you planning to add additional EBPS? If yes, please describe."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Are you planning to terminate or modify any existing EBPS? If yes, list which programs and what you plan to terminate/modify.",
                        .PolicyUnderwritingCodeId = "9265",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are you planning to terminate or modify any existing EBPS? If yes, list which programs and what you plan to terminate/modify."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Have you changed providers for any program within the past year? If yes, which program changed and what were the provider changes made?",
                        .PolicyUnderwritingCodeId = "9266",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Have you changed providers for any program within the past year? If yes, which program changed and what were the provider changes made?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Do you plan to change providers for any program within the next year? If yes, please describe.",
                        .PolicyUnderwritingCodeId = "9267",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you plan to change providers for any program within the next year? If yes, please describe."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Are your employees eligible for the EBPS? How many? Please specify full time, part time and retirees.",
                        .PolicyUnderwritingCodeId = "9268",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are your employees eligible for the EBPS? How many? Please specify full time, part time and retirees."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Do you have a trained staff to administer the EBPS, answer questions, and advise employees concerning these programs? If yes, how many employees are in this department?",
                        .PolicyUnderwritingCodeId = "9269",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you have a trained staff to administer the EBPS, answer questions, and advise employees concerning these programs? If yes, how many employees are in this department?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8B. What training and experience do these individuals have?",
                        .PolicyUnderwritingCodeId = "9270",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "What training and experience do these individuals have?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8C. Are the individuals who handle assets of the plans bonded as required by ERISA?",
                        .PolicyUnderwritingCodeId = "9271",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are the individuals who handle assets of the plans bonded as required by ERISA?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Do you provide a published EBP manual or booklet to all employees which clearly details how the program works?",
                        .PolicyUnderwritingCodeId = "9272",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you provide a published EBP manual or booklet to all employees which clearly details how the program works?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9B. Is it updated to reflect subsequent changes?",
                        .PolicyUnderwritingCodeId = "9273",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is it updated to reflect subsequent changes?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9C. Is manual reviewed with each new employee by a trained human resources person? Please include a copy.",
                        .PolicyUnderwritingCodeId = "9274",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is manual reviewed with each new employee by a trained human resources person? Please include a copy."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Do you require, and keep on file, signed documentation from employees noting acceptance or rejection of optional benefits?",
                        .PolicyUnderwritingCodeId = "9275",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you require, and keep on file, signed documentation from employees noting acceptance or rejection of optional benefits?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Have you had a claim for this coverage within the past five years, or are you aware of a situation which could lead to a possible claim?",
                        .PolicyUnderwritingCodeId = "9276",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CGL_Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Have you had a claim for this coverage within the past five years, or are you aware of a situation which could lead to a possible claim?"
                    })
#End Region

#Region "Inland Marine List"

#Region "Unused Risk Grade Questions"
            '' Inland-Risk Grade Questions
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1. Any equipment rented, loaned to others With or without operators?",
            '            .PolicyUnderwritingCodeId = "9355",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any equipment rented, loaned to others With or without operators?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Any cranes used in the applicants' operations?",
            '            .PolicyUnderwritingCodeId = "9356",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any cranes used in the applicants' operations?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "3. Are vehicles operated beyond a 200 mile radius?",
            '            .PolicyUnderwritingCodeId = "9357",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are vehicles operated beyond a 200 mile radius?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. Any radio, television or cellular towers?",
            '            .PolicyUnderwritingCodeId = "9358",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any radio, television or cellular towers?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Any prior coverage declined, cancelled or non-renewed during the prior 3 years?",
            '            .PolicyUnderwritingCodeId = "9335",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any prior coverage declined, cancelled or non-renewed during the prior 3 years?"
            '            })
#End Region

#Region "Unused Applicant Information"

            '' Inland-Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1A. Is the Applicant a subsidiary of another entity?",
            '            .PolicyUnderwritingCodeId = "9000",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is the Applicant a subsidiary of another entity?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1B. Does the Applicant have any subsidiaries?",
            '            .PolicyUnderwritingCodeId = "9001",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Does the Applicant have any subsidiaries?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Is a formal safety program in operation?",
            '            .PolicyUnderwritingCodeId = "9002",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is a formal safety program in operation?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "3. Any exposure to flammables, explosives, chemicals?",
            '            .PolicyUnderwritingCodeId = "9003",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any exposure to flammables, explosives, chemicals?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. Any other insurance With this company? (list policy numbers)",
            '            .PolicyUnderwritingCodeId = "9005",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any other insurance With this company? (list policy numbers)"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Any policy or coverage declined, cancelled or non - renewed during the prior 3 years For any premises or operations?",
            '            .PolicyUnderwritingCodeId = "9006",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any policy or coverage declined, cancelled or non - renewed during the prior 3 years For any premises or operations?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "6. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
            '            .PolicyUnderwritingCodeId = "9007",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "7. During the last five years, has any Applicant been indicted For or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection With this or any other Property?",
            '            .PolicyUnderwritingCodeId = "9008",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "During the last five years, has any Applicant been indicted For or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection With this or any other Property?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "8. Any uncorrected fire And/or safety code violations?",
            '            .PolicyUnderwritingCodeId = "9009",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any uncorrected fire And/or safety code violations?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "9. Has Applicant had a foreclosure, repossession, bankruptcy or filed For bankruptcy during the last five (5) years?",
            '            .PolicyUnderwritingCodeId = "9400",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Has Applicant had a foreclosure, repossession, bankruptcy or filed For bankruptcy during the last five (5) years?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "10. Has Applicant had a judgement or lien during the last five (5) years?",
            '            .PolicyUnderwritingCodeId = "9010",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Has Applicant had a judgement or lien during the last five (5) years?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "11. Has business been placed in a trust?",
            '            .PolicyUnderwritingCodeId = "9011",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Has business been placed in a trust?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "12. Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?",
            '            .PolicyUnderwritingCodeId = "9012",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "13. Does Applicant have other business ventures For which coverage is not requested?",
            '            .PolicyUnderwritingCodeId = "9401",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Does Applicant have other business ventures For which coverage is not requested?"
            '            })

#End Region

#Region "Unused General Information"
            '' Inland_Transportation-General Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1. Is there a vehicle maintenance program in operation?",
            '            .PolicyUnderwritingCodeId = "9448",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is there a vehicle maintenance program in operation?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Does Applicant obtain mvr verification For drivers?",
            '            .PolicyUnderwritingCodeId = "9449",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Does Applicant obtain mvr verification For drivers?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "3. Does Applicant have a driver recruiting method?",
            '            .PolicyUnderwritingCodeId = "9450",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Does Applicant have a driver recruiting method?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. Do drivers receive regular physicals?",
            '            .PolicyUnderwritingCodeId = "9451",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Do drivers receive regular physicals?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Any waterborne shipments to be covered?",
            '            .PolicyUnderwritingCodeId = "9452",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any waterborne shipments to be covered?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "6. Are vehicles equipped With theft alarms?",
            '            .PolicyUnderwritingCodeId = "9453",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are vehicles equipped With theft alarms?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "7. Are vehicles left unlocked When unattended?",
            '            .PolicyUnderwritingCodeId = "9454",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are vehicles left unlocked When unattended?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "8. Are vehicles left loaded overnight?",
            '            .PolicyUnderwritingCodeId = "9455",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are vehicles left loaded overnight?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "9. Does Applicant back haul Property of others?",
            '            .PolicyUnderwritingCodeId = "9456",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Does Applicant back haul Property of others?"
            '            })
#End Region

            ' CIM_Transportation-Motor Truck Cargo Legal Liability Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Is there a vehicle maintenance program in operation?",
                        .PolicyUnderwritingCodeId = "9448",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is there a vehicle maintenance program in operation?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Does Applicant obtain MVR verification For drivers?",
                        .PolicyUnderwritingCodeId = "9449",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does Applicant obtain MVR verification For drivers?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Does Applicant have a driver recruiting method?",
                        .PolicyUnderwritingCodeId = "9517",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does Applicant have a driver recruiting method?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Do drivers receive regular physicals?",
                        .PolicyUnderwritingCodeId = "9451",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Do drivers receive regular physicals?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Are vehicles equipped With theft alarms?",
                        .PolicyUnderwritingCodeId = "9518",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are vehicles equipped With theft alarms?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Are vehicles left unlocked When unattended?",
                        .PolicyUnderwritingCodeId = "9519",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are vehicles left unlocked When unattended?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Are overages, shortages, & damage claims pending?",
                        .PolicyUnderwritingCodeId = "9520",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are overages, shortages, & damage claims pending?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Are any vehicles operated For the Applicant by others?",
                        .PolicyUnderwritingCodeId = "9521",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are any vehicles operated For the Applicant by others?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Do terminals have fire protection (sprinklers, hoses etc.)?",
                        .PolicyUnderwritingCodeId = "9522",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Do terminals have fire protection (sprinklers, hoses etc.)?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Do terminals have security systems (guards, alarms, fences.lights, dogs, etc.)?",
                        .PolicyUnderwritingCodeId = "9523",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Do terminals have security systems (guards, alarms, fences.lights, dogs, etc.)?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Are vehicles left loaded overnight?",
                        .PolicyUnderwritingCodeId = "9524",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are vehicles left loaded overnight?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Is the Applicant an owner Operator?",
                        .PolicyUnderwritingCodeId = "9525",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is the Applicant an owner Operator?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Does the Applicant hire owner operators?",
                        .PolicyUnderwritingCodeId = "9526",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the Applicant hire owner operators?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "14. Does the Applicant triplelease to others?",
                        .PolicyUnderwritingCodeId = "9527",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the Applicant triplelease to others?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "15. Does the Applicant backhaul Property of others?",
                        .PolicyUnderwritingCodeId = "9528",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Transportation Section",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the Applicant backhaul Property of others?"
                        })

            ' CIM_Equipment Floater
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Equipment rented, loaded to/from others With/without operators?",
                        .PolicyUnderwritingCodeId = "9457",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Inland-Equipment Floater",
                        .IsQuestionRequired = False,
                        .kqDescription = "Equipment rented, loaded to/from others With/without operators?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is Applicant operating equipment Not listed here?",
                        .PolicyUnderwritingCodeId = "9458",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Inland-Equipment Floater",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is Applicant operating equipment Not listed here?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Property used underground?",
                        .PolicyUnderwritingCodeId = "9459",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Inland-Equipment Floater",
                        .IsQuestionRequired = False,
                        .kqDescription = "Property used underground?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any work done afloat?",
                        .PolicyUnderwritingCodeId = "9460",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Inland-Equipment Floater",
                        .IsQuestionRequired = False,
                        .kqDescription = "any work done afloat?"
                        })

            ' CIM_Electronic-General Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. in the Event of a major or total loss could you Return to operation within one week?",
                        .PolicyUnderwritingCodeId = "9461",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "in the event of a major or total loss could you return to operation within one week?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Do you have an arrangement For the use of other equipment?  (attach copy of agreement)",
                        .PolicyUnderwritingCodeId = "9462",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Do you have an arrangement for the use of other equipment?  (attach copy of agreement)"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Is your equipment manufacturer in a position to replace your equipment promptly?",
                        .PolicyUnderwritingCodeId = "9463",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is your equipment manufacturer in a position to replace your equipment promptly?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Is your equipment under manufacturer's warranty?",
                        .PolicyUnderwritingCodeId = "9464",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is your equipment under manufacturer's warranty?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Do you have a service maintenance contract With a manufacturer or other service contractor?",
                        .PolicyUnderwritingCodeId = "9465",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Do you have a service maintenance contract with a manufacturer or other service contractor?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Is the equipment shipped by common carrier?",
                        .PolicyUnderwritingCodeId = "9466",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is the equipment shipped by common carrier?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Is the equipment shipped by company vehicle?",
                        .PolicyUnderwritingCodeId = "9467",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is the equipment shipped by company vehicle?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Is the media/data shipped by common carrier?",
                        .PolicyUnderwritingCodeId = "9468",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is the media/data shipped by common carrier?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Is the media/data shipped by company vehicle?",
                        .PolicyUnderwritingCodeId = "9469",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is the media/data shipped by company vehicle?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Does the premises have a burglar alarm?",
                        .PolicyUnderwritingCodeId = "9470",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the premises have a burglar alarm?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11A. Does the Applicant have an uninterruptable power source to protect the hardware from power line problems?",
                        .PolicyUnderwritingCodeId = "9471",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the Applicant have an uninterruptable power source to protect the hardware from power line problems?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11B. Does the Applicant have a line conditioner to protect the hardware from power line problems?",
                        .PolicyUnderwritingCodeId = "9473",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the Applicant have a line conditioner to protect the hardware from power line problems?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11C. Does the Applicant have a power suppressor voltage regulator to protect the hardware from power line problems?",
                        .PolicyUnderwritingCodeId = "9474",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the Applicant have a power suppressor voltage regulator to protect the hardware from power line problems?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11D. Does the Applicant have a dedicated line to protect the hardware from power line problems?",
                        .PolicyUnderwritingCodeId = "9475",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the Applicant have a dedicated line to protect the hardware from power line problems?"
                        })

            ' CIM_Electronic-Computer Room Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Is the data processing equipment located in a specifically designated room?",
                        .PolicyUnderwritingCodeId = "9476",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is the data processing equipment located in a specifically designated room?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is access to the room restricted?",
                        .PolicyUnderwritingCodeId = "9477",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is access to the room restricted?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Is the equipment controlled by a master shutdown switch?",
                        .PolicyUnderwritingCodeId = "9478",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is the equipment controlled by a master shutdown switch?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Is there a separate air conditioning system designed to specifically protect the EDP equipment?",
                        .PolicyUnderwritingCodeId = "9479",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is there a separate air conditioning system designed to specifically protect the EDP equipment?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5A. The computer room has no fire suppression systems.",
                        .PolicyUnderwritingCodeId = "9480",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "The computer room has no fire suppression systems."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5B. The computer room is protected by a wet sprinkler fire suppression system.",
                        .PolicyUnderwritingCodeId = "9481",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "The computer room is protected by a wet sprinkler fire suppression system."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5C. The computer room is protected by a dry sprinkler fire suppression system.",
                        .PolicyUnderwritingCodeId = "9482",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "The computer room is protected by a dry sprinkler fire suppression system."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5D. The computer room is Protected by a HALON fire suppression system.",
                        .PolicyUnderwritingCodeId = "9483",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "The computer room is Protected by a HALON fire suppression system."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5E. The computer room is protected by a CO2 fire suppression system.",
                        .PolicyUnderwritingCodeId = "9484",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "The computer room is Protected by a CO2 fire suppression system."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5F. The computer room is protected by any other type of fire suppression system.",
                        .PolicyUnderwritingCodeId = "9485",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "The computer room is protected by any other type of fire suppression system."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6A. Does the computer room have a raised pedestal floor?",
                        .PolicyUnderwritingCodeId = "9486",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the computer room have a raised pedestal floor?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6B. Is the floor construction type combustible?",
                        .PolicyUnderwritingCodeId = "9487",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is the floor construction type combustible?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6C. The computer room has smoke detectors for below floor protection.",
                        .PolicyUnderwritingCodeId = "9488",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "The computer room has smoke detectors for below floor protection."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6D. The computer room has HALON or CO2 for below floor protection.",
                        .PolicyUnderwritingCodeId = "9489",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "The computer room has HALON or CO2 for below floor protection."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6E. The computer room has any other type of system for below floor protection.",
                        .PolicyUnderwritingCodeId = "9490",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "The computer room has any other type of system For below floor protection."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6F. The computer room has no below floor protection systems.",
                        .PolicyUnderwritingCodeId = "9491",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "The computer room has no below floor protection systems."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7A. Does the computer room have a Local Temperature alarm?",
                        .PolicyUnderwritingCodeId = "9492",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the computer room have a Local Temperature alarm?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7B. Does the computer room have a Central Temperature alarm?",
                        .PolicyUnderwritingCodeId = "9493",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the computer room have a Central Temperature alarm?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7C. Does the computer room have a Local Humidity alarm?",
                        .PolicyUnderwritingCodeId = "9494",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the computer room have a Local Humidity alarm?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7D. Does the computer room have a Central Humidity alarm?",
                        .PolicyUnderwritingCodeId = "9495",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the computer room have a Central Humidity alarm?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7E. Does the computer room have a Local Smoke alarm?",
                        .PolicyUnderwritingCodeId = "9496",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the computer room have a Local Smoke alarm?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7F. Does the computer room have a Central Smoke alarm?",
                        .PolicyUnderwritingCodeId = "9497",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the computer room have a Central Smoke alarm?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7G. Does the computer room have a Local Fire alarm?",
                        .PolicyUnderwritingCodeId = "9498",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the computer room have a Local Fire alarm?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7H. Does the computer room have a Central Fire alarm?",
                        .PolicyUnderwritingCodeId = "9499",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the computer room have a Central Fire alarm?"
                        })

            ' CIM_Electronic-Media & Data (Software) Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Are anti-viral safeguards in effect?",
                        .PolicyUnderwritingCodeId = "9500",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are anti-viral safeguards in effect?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Are duplicates of software maintained?",
                        .PolicyUnderwritingCodeId = "9501",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are duplicates of software maintained?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3A. Is data backed up daily?",
                        .PolicyUnderwritingCodeId = "9502",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is Data backed up daily?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3B. Is data backed up weekly?",
                        .PolicyUnderwritingCodeId = "9503",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is Data backed up weekly?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3C. Is data backed up monthly?",
                        .PolicyUnderwritingCodeId = "9504",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is Data backed up monthly?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3D. Is data backed up quarterly?",
                        .PolicyUnderwritingCodeId = "9505",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is Data backed up quarterly?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3E. Is data backed up yearly?",
                        .PolicyUnderwritingCodeId = "9506",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is Data backed up yearly?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3F. Is data backed up at any other interval?",
                        .PolicyUnderwritingCodeId = "9507",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is Data backed up at any other interval?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3G. Are software duplicates software stored on premises?",
                        .PolicyUnderwritingCodeId = "9508",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are software duplicates software stored on premises?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3H. Is duplicate software stored off premises?",
                        .PolicyUnderwritingCodeId = "9509",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is duplicate software stored off premises?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3I. Are data backups stored on premises?",
                        .PolicyUnderwritingCodeId = "9510",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are data backups stored on premises?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3J. Are data backups stored off premises?",
                        .PolicyUnderwritingCodeId = "9511",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are data backups stored off premises?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3K. Is media And data (software) stored on premises in a safe?",
                        .PolicyUnderwritingCodeId = "9512",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is Media And Data(software) stored on premises in a safe?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3L. Is media And data (software) stored on premises in a vault?",
                        .PolicyUnderwritingCodeId = "9513",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is Media And Data(software) stored on premises in a vault?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3M. Is media And data (software) stored on premises in a computer room?",
                        .PolicyUnderwritingCodeId = "9514",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is Media And Data(software) stored on premises in a computer room?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3N. Is media And data (software) stored on premises in any other storage unit?",
                        .PolicyUnderwritingCodeId = "9515",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is Media And Data(software) stored on premises in any other storage unit?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3O. Name And address of off premises storage location:",
                        .PolicyUnderwritingCodeId = "9516",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CIM_Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Name And address of off premises storage location:"
                        })
#End Region

#Region "Crime List"


#Region "Unused Applicant Information"
            '' Crime-Applicant Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1A. Is the Applicant a subsidiary of another entity?",
            '            .PolicyUnderwritingCodeId = "9000",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is the Applicant a subsidiary of another entity?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1B. Does the Applicant have any subsidiaries?",
            '            .PolicyUnderwritingCodeId = "9001",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Does the Applicant have any subsidiaries?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Is a formal safety program in operation?",
            '            .PolicyUnderwritingCodeId = "9002",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is a formal safety program in operation?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "3. Any exposure to flammables, explosives, chemicals?",
            '            .PolicyUnderwritingCodeId = "9003",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any exposure to flammables, explosives, chemicals?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. Any other insurance With this company? (list policy numbers)",
            '            .PolicyUnderwritingCodeId = "9005",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any other insurance With this company? (list policy numbers)"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Any policy or coverage declined, cancelled or non - renewed during the prior 3 years For any premises or operations?",
            '            .PolicyUnderwritingCodeId = "9006",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any policy or coverage declined, cancelled or non - renewed during the prior 3 years For any premises or operations?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "6. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
            '            .PolicyUnderwritingCodeId = "9007",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "7. During the last five years, has any Applicant been indicted For or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection With this or any other Property?",
            '            .PolicyUnderwritingCodeId = "9008",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "During the last five years, has any Applicant been indicted For or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection With this or any other Property?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "8. Any uncorrected fire And/or safety code violations?",
            '            .PolicyUnderwritingCodeId = "9009",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any uncorrected fire And/or safety code violations?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "9. Has Applicant had a foreclosure, repossession, bankruptcy or filed For bankruptcy during the last five (5) years?",
            '            .PolicyUnderwritingCodeId = "9400",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Has Applicant had a foreclosure, repossession, bankruptcy or filed For bankruptcy during the last five (5) years?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "10. Has Applicant had a judgement or lien during the last five (5) years?",
            '            .PolicyUnderwritingCodeId = "9010",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Has Applicant had a judgement or lien during the last five (5) years?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "11. Has business been placed in a trust?",
            '            .PolicyUnderwritingCodeId = "9011",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Has business been placed in a trust?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "12. Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?",
            '            .PolicyUnderwritingCodeId = "9012",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "13. Does Applicant have other business ventures For which coverage is Not requested?",
            '            .PolicyUnderwritingCodeId = "9401",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "3",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Applicant Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Does Applicant have other business ventures For which coverage is Not requested?"
            '            })
#End Region

            ' Crime-General Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Are volunteers used? If ""yes"", Number of volunteers.",
                        .PolicyUnderwritingCodeId = "9235",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_General Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are volunteers used? If ""yes"", Number of volunteers."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Any employees leased to others?  If ""yes"", give number And explain.",
                        .PolicyUnderwritingCodeId = "9236",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_General Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any employees leased to others?  If ""yes"", give number And explain."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Any employees leased from others?  If ""yes"", give number And explain.",
                        .PolicyUnderwritingCodeId = "9237",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_General Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any employees leased from others?  If ""yes"", give number And explain."
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any employees perform money investing or trading?",
                        .PolicyUnderwritingCodeId = "9238",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_General Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any employees perform money investing or trading?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any employees receive or issue warehouse receipts?",
                        .PolicyUnderwritingCodeId = "9239",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_General Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any employees receive or issue warehouse receipts?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any employee(s) been cancelled For crime coverage by any insurer?",
                        .PolicyUnderwritingCodeId = "9240",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_General Information!",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any employee(s) been cancelled For crime coverage by any insurer?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Does Applicant have any written agreements With clients?",
                        .PolicyUnderwritingCodeId = "9241",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_General Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does Applicant have any written agreements With clients?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Does Applicant transfer any funds via phone or fax?",
                        .PolicyUnderwritingCodeId = "9242",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_General Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant transfer any funds via phone or fax?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Any exposure from loss to guest Property?",
                        .PolicyUnderwritingCodeId = "9243",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_General Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any exposure from loss to guest Property?"
                        })

            ' CRM_Hiring Practices
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Is prior employer history checked?",
                        .PolicyUnderwritingCodeId = "9244",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Hiring Practices",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is prior employer history checked?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is education And training verified?",
                        .PolicyUnderwritingCodeId = "9245",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Hiring Practices",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is education And training verified?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Is drug testing conducted?",
                        .PolicyUnderwritingCodeId = "9246",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Hiring Practices",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is drug testing conducted?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Is a formal training program established And followed?",
                        .PolicyUnderwritingCodeId = "9247",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Hiring Practices",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is a formal training program established And followed?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Are credit checks secured For employees With access to financial transactions?",
                        .PolicyUnderwritingCodeId = "9248",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Hiring Practices",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are credit checks secured For employees With access to financial transactions?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Are social security numbers verified?",
                        .PolicyUnderwritingCodeId = "9249",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Hiring Practices",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are social security numbers verified?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Is criminal history checked?",
                        .PolicyUnderwritingCodeId = "9250",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Hiring Practices",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is criminal history checked?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Are managers provided With  names And salaries of all assigned employees?",
                        .PolicyUnderwritingCodeId = "9251",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Hiring Practices",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are managers provided With  names And salaries of all assigned employees?"
                        })

            ' CRM_Audits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Audit is performed by: (cpa, public accountant, staff, other)",
                        .PolicyUnderwritingCodeId = "9365",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Audit is performed by: (cpa, public accountant, staff, other)"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Name And address of person or firm performing audit",
                        .PolicyUnderwritingCodeId = "9366",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Name And address of person or firm performing audit"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Date of completion of last audit of cash & accounts:  Date of completion of last audit of inventory:",
                        .PolicyUnderwritingCodeId = "9367",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Date of completion of last audit of cash & accounts:  Date of completion of last audit of inventory:"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Audit frequency? (annual, semi - annual, quarterly, other)",
                        .PolicyUnderwritingCodeId = "9368",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Audit frequency? (annual, semi - annual, quarterly, other)"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Audit report is rendered to: (owner, partners, board of directors, other)",
                        .PolicyUnderwritingCodeId = "9369",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Audit report is rendered to: (owner, partners, board of directors, other)"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Financial format is: (audit, review, compilation, tax return only)",
                        .PolicyUnderwritingCodeId = "9370",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Financial format is: (audit, review, compilation, tax return only)"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Are all locations audited?",
                        .PolicyUnderwritingCodeId = "9371",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are all locations audited?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Is audit made in accordance With generally accepted auditing standards And so certified? If ""no"", explain scope of audit",
                        .PolicyUnderwritingCodeId = "9372",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is audit made in accordance With generally accepted auditing standards And so certified? If ""no"", explain scope of audit"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Were any discrepancies or loose practices commented upon in this audit? If ""yes"", submit a copy of the audit And auditor's comments",
                        .PolicyUnderwritingCodeId = "9373",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Were any discrepancies or loose practices commented upon in this audit? If ""yes"", submit a copy of the audit And auditor's comments"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Does audit include inventory?",
                        .PolicyUnderwritingCodeId = "9374",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does audit include inventory?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Are references of all New hires checked With respect to employment history?",
                        .PolicyUnderwritingCodeId = "9375",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Are references of all New hires checked With respect to employment history?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Does audit department have a program to detect ghost employees?",
                        .PolicyUnderwritingCodeId = "9376",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does audit department have a program to detect ghost employees?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Is payroll system audited annually?",
                        .PolicyUnderwritingCodeId = "9377",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is payroll system audited annually?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "14. Is a complete physical inventory made? If ""yes"", how often:",
                        .PolicyUnderwritingCodeId = "9378",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is a complete physical inventory made? If ""yes"", how often:"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "15. Is inventory made by persons who Do Not have custody control?",
                        .PolicyUnderwritingCodeId = "9379",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is inventory made by persons who Do Not have custody control?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "16. Is a requisition / shipping order required For removal of goods from storeroom / warehouse?",
                        .PolicyUnderwritingCodeId = "9380",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is a requisition / shipping order required For removal of goods from storeroom / warehouse?"
                        })

            ' CRM_Banking/Other
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Are bank accounts reconciled by someone Not authorized to deposit or withdraw?",
                        .PolicyUnderwritingCodeId = "9381",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are bank accounts reconciled by someone Not authorized to deposit or withdraw?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is countersignature of checks required? If Not, who signs controls?",
                        .PolicyUnderwritingCodeId = "9382",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is countersignature of checks required? If Not, who signs controls?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Will securities be subject to joint control of two or more responsible employees?",
                        .PolicyUnderwritingCodeId = "9383",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Will securities be subject to joint control of two or more responsible employees?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Are all officers And employees required to take annual vacations of at least five consecutive business days?",
                        .PolicyUnderwritingCodeId = "9384",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are all officers And employees required to take annual vacations of at least five consecutive business days?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Is there a written policy regarding efts?",
                        .PolicyUnderwritingCodeId = "9385",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "is there a written policy regarding efts?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. What is the largest Single amount that can be transferred?",
                        .PolicyUnderwritingCodeId = "9386",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "What is the largest Single amount that can be transferred?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Prior to funds transfer, does financial institution verify authenticity With another employee?",
                        .PolicyUnderwritingCodeId = "9387",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Prior to funds transfer, does financial institution verify authenticity With another employee?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Are hard copies of funds transfer confirmations received And reconciled?",
                        .PolicyUnderwritingCodeId = "9388",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are hard copies of funds transfer confirmations received And reconciled?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Frequency of deposits: (daily, other)",
                        .PolicyUnderwritingCodeId = "9389",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = False,
                        .kqDescription = "Frequency of deposits: (daily, other)"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Are detailed records of bank deposits maintained?",
                        .PolicyUnderwritingCodeId = "9390",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are detailed records of bank deposits maintained?"
                        })

#Region "Unused Purchasing/Receiving"
            '' Crime-Purchasing/Receiving Controls
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1. Are duties segregated?",
            '            .PolicyUnderwritingCodeId = "9391",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "14",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Purchasing/Receiving Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are duties segregated?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Are departments supervised by someone Not authorized to pay bills?",
            '            .PolicyUnderwritingCodeId = "9392",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "14",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Purchasing/Receiving Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are departments supervised by someone Not authorized to pay bills?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "3. Is responsibility For checking merchandise received / controlled by more than one individual?",
            '            .PolicyUnderwritingCodeId = "9393",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "14",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Purchasing/Receiving Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is responsibility For checking merchandise received / controlled by more than one individual?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. Is actual receipt of merchandise verified before payment is made?",
            '            .PolicyUnderwritingCodeId = "9394",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "14",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Purchasing/Receiving Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is actual receipt of merchandise verified before payment is made?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Is a numbered purchase order system implemented And followed?",
            '            .PolicyUnderwritingCodeId = "9395",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "14",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Purchasing/Receiving Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is a numbered purchase order system implemented And followed?"
            '            })
#End Region

#Region "Unused Computer Fraud"
            '' Crime-Computer Fraud Controls
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1. Do internal audit procedures include computer operations?",
            '            .PolicyUnderwritingCodeId = "9396",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "15",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Computer Fraud Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Do internal audit procedures include computer operations?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Is there an employee or department whose sole duty is security?",
            '            .PolicyUnderwritingCodeId = "9397",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "15",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Computer Fraud Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is there an employee or department whose sole duty is security?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "3. Are suspicious transactions reviewed And investigated?",
            '            .PolicyUnderwritingCodeId = "9398",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "15",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Computer Fraud Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are suspicious transactions reviewed And investigated?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. Is physical access to computer room And equipment restricted to authorized personnel?",
            '            .PolicyUnderwritingCodeId = "9399",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "15",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Computer Fraud Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is physical access to computer room And equipment restricted to authorized personnel?"
            '            })
#End Region


            ' Crime-Risk Grade Questions
#Region "Unused Risk Grade - Only use 3rd question"
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1. Any prior coverage declined, cancelled or non - renewed during the prior 3 years?",
            '            .PolicyUnderwritingCodeId = "9345",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any prior coverage declined, cancelled or non - renewed during the prior 3 years?"
            '            })

            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Have any employees been cancelled For crime coverages by any insurer?",
            '            .PolicyUnderwritingCodeId = "9351",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Have any employees been cancelled For crime coverages by any insurer?"
            '            })
#End Region

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Any prior employee dishonest claim in the past 5 years?",
                        .PolicyUnderwritingCodeId = "9352",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "CRM_Risk Grade Questions",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any prior employee dishonest claim in the past 5 years?"
                        })

#Region "Unused Risk Grade - Only use 3rd question"
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. During the last 5 years, has any Applicant been indicted For or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection With this or any other Property?",
            '            .PolicyUnderwritingCodeId = "9353",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "During the last 5 years, has any Applicant been indicted For or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection With this or any other Property?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Any bankruptcies, tax or credit liens against the Applicant in the past 5 years?",
            '            .PolicyUnderwritingCodeId = "9354",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any bankruptcies, tax or credit liens against the Applicant in the past 5 years?"
            '            })
#End Region

#End Region

            Return list
        End Function

        Public Shared Function GetCommercialCPPUnderwritingQuestions_CPR() As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)

#Region "Property list"
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1A. Is the Applicant a subsidiary of another entity?",
                        .PolicyUnderwritingCodeId = "9000",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1B. Does the Applicant have any subsidiaries?",
                        .PolicyUnderwritingCodeId = "9001",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is a formal safety program in operation?",
                        .PolicyUnderwritingCodeId = "9002",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                        })

            'Kill Question
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Any exposure to flammables, explosives, chemicals?",
                        .PolicyUnderwritingCodeId = "9003",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any exposure to flammables, explosives, chemicals?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any other insurance With this company? (list policy numbers)",
                        .PolicyUnderwritingCodeId = "9005",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                        })

            'Kill Question
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?",
                        .PolicyUnderwritingCodeId = "9006",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any prior coverage declined, cancelled or non-renewed during the prior 3 years?"
                        })

            'Kill Question
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9007",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
                        })

            '.IsTrueKillQuestion = True,
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. During the last five years, has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?",
                        .PolicyUnderwritingCodeId = "9008",
                        .IsTrueUwQuestion = True,
                        .IsTrueKillQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "During the last 5 years, has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?"
                        })

            'Kill Question
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Any uncorrected fire And/or safety code violations?",
                        .PolicyUnderwritingCodeId = "9009",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any uncorrected fire code violations?"
                        })

            'Kill Question
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9400",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any bankruptcies, tax or credit liens against the Applicant in the past 5 years?"
                        })

            'set programatically - same as 9400 above (#9)
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Has Applicant had a judgement or lien during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9010",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Has business been placed in a trust?",
                        .PolicyUnderwritingCodeId = "9011",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?",
                        .PolicyUnderwritingCodeId = "9012",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Does Applicant have other business ventures for which coverage is Not requested?",
                        .PolicyUnderwritingCodeId = "9401",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Application Information",
                        .IsQuestionRequired = False
                        })
#End Region

            Return list
        End Function

        Public Shared Function GetCommercialCPPUnderwritingQuestions_CGL() As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)

#Region "General Liability List"
            ' Risk Grade Questions

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Any prior coverage declined, cancelled or non-renewed during the prior 3 years?",
                        .PolicyUnderwritingCodeId = "9345",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any prior coverage declined, cancelled or non-renewed during the prior 3 years?"
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9346",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Do any operations include blasting or utilize or store explosive material?",
                        .PolicyUnderwritingCodeId = "9347",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do any operations include blasting or utilize or store explosive material?"
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Are subcontractors allowed to work without providing you with a certificate of insurance?",
                        .PolicyUnderwritingCodeId = "9348",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are subcontractors allowed to work without providing you with a certificate of insurance?"
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Does Applicant lease equipment to others with or without operators?",
                        .PolicyUnderwritingCodeId = "9349",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant lease equipment to others with or without operators?"
                    })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any products related to the aircraft or space industry?",
                        .PolicyUnderwritingCodeId = "9350",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any products related to the aircraft or space industry?"
                    })

#Region "Applicant Information"
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1A. Is the Applicant a subsidiary of another entity?",
                        .PolicyUnderwritingCodeId = "9000",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is the Applicant a subsidiary of another entity?"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1B. Does the Applicant have any subsidiaries?",
                        .PolicyUnderwritingCodeId = "9001",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the Applicant have any subsidiaries?"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is a formal safety program in operation?",
                        .PolicyUnderwritingCodeId = "9002",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is a formal safety program in operation?"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Any exposure to flammables, explosives, chemicals?",
                        .PolicyUnderwritingCodeId = "9003",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any exposure to flammables, explosives, chemicals?"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any other insurance with this company? (List policy numbers)",
                        .PolicyUnderwritingCodeId = "9005",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any other insurance with this company? (List policy numbers)"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?",
                        .PolicyUnderwritingCodeId = "9006",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any policy or coverage declined, cancelled or non-renewed during the prior 3 years for any premises or operations?"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9007",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. During the last five years, has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?",
                        .PolicyUnderwritingCodeId = "9008",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "During the last five years, has any Applicant been indicted for or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection with this or any other property?"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Any uncorrected fire and/or safety code violations?",
                        .PolicyUnderwritingCodeId = "9009",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any uncorrected fire and/or safety code violations?"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9400",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Has Applicant had a foreclosure, repossession, bankruptcy or filed for bankruptcy during the last five (5) years?"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Has Applicant had a judgement or lien during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9010",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Has Applicant had a judgement or lien during the last five (5) years?"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Has business been placed in a trust?",
                        .PolicyUnderwritingCodeId = "9011",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Has business been placed in a trust?"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?",
                        .PolicyUnderwritingCodeId = "9012",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?"
                    })
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Does Applicant have other business ventures for which coverages is not requested?",
                        .PolicyUnderwritingCodeId = "9401",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does Applicant have other business ventures for which coverages is not requested?"
                    })
#End Region

            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Any medical facilities provided or medical professionals employed or contracted?",
                        .PolicyUnderwritingCodeId = "9013",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any medical facilities provided or medical professionals employed or contracted?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Any exposure to radioactive/nuclear materials?",
                        .PolicyUnderwritingCodeId = "9014",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any exposure to radioactive/nuclear materials?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Do/have past, present or discontinued operations involve(d) storing, treating, discharging, applying, disposing, or transporting of hazardous material? (E.G. landfills, wastes, fuel tanks, etc)",
                        .PolicyUnderwritingCodeId = "9015",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do/have past, present or discontinued operations involve(d) storing, treating, discharging, applying, disposing, or transporting of hazardous material? (E.G. landfills, wastes, fuel tanks, etc)"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any operations sold, acquired or discounted in last 5 years?",
                        .PolicyUnderwritingCodeId = "9252",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any operations sold, acquired or discounted in last 5 years?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Machinery or equipment loaned or rented to others?",
                        .PolicyUnderwritingCodeId = "9017",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Machinery or equipment loaned or rented to others?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any watercraft, docks, floats owned, hired or leased?",
                        .PolicyUnderwritingCodeId = "9018",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any watercraft, docks, floats owned, hired or leased?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Any parking facilities owned/rented?",
                        .PolicyUnderwritingCodeId = "9019",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any parking facilities owned/rented?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Is a fee charged for parking?",
                        .PolicyUnderwritingCodeId = "9253",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is a fee charged for parking?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Recreation facilities provided?",
                        .PolicyUnderwritingCodeId = "9021",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Recreation facilities provided?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Is there a swimming pool on the premises?",
                        .PolicyUnderwritingCodeId = "9022",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is there a swimming pool on the premises?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Sporting or social events sponsored?",
                        .PolicyUnderwritingCodeId = "9023",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Sporting or social events sponsored?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Any structural alterations contemplated?",
                        .PolicyUnderwritingCodeId = "9024",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any structural alterations contemplated?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Any demolition exposure contemplated?",
                        .PolicyUnderwritingCodeId = "9025",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any demolition exposure contemplated?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "14. Has Applicant been active in or is currently active in joint ventures?",
                        .PolicyUnderwritingCodeId = "9026",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Has Applicant been active in or is currently active in joint ventures?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "15. Do you lease employees to or from other employers?",
                        .PolicyUnderwritingCodeId = "9027",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you lease employees to or from other employers?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "16. Is there a labor interchange with any other business or subsidiaries?",
                        .PolicyUnderwritingCodeId = "9028",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is there a labor interchange with any other business or subsidiaries?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "17. Are day care facilities operated or controlled?",
                        .PolicyUnderwritingCodeId = "9029",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are day care facilities operated or controlled?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "18. Have any crimes occurred or been attempted on your premises within the last three years?",
                        .PolicyUnderwritingCodeId = "9254",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Have any crimes occurred or been attempted on your premises within the last three years?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "19. Is there a formal, written safety and security policy in effect?",
                        .PolicyUnderwritingCodeId = "9255",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is there a formal, written safety and security policy in effect?"
                    })
            ' General Liability - General Info
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "20. Does the businesses’ promotional literature make any representations about the safety or security of the premises?",
                        .PolicyUnderwritingCodeId = "9256",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - General Info",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the businesses’ promotional literature make any representations about the safety or security of the premises?"
                    })

            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Does Applicant draw plans, designs, or specifications for others?",
                        .PolicyUnderwritingCodeId = "9257",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant draw plans, designs, or specifications for others?"
                    })
            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Do any operations include blasting or utilize or store explosive materials?",
                        .PolicyUnderwritingCodeId = "9258",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do any operations include blasting or utilize or store explosive materials?"
                    })
            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Do any operations include excavation, tunneling, underground work or earth moving?",
                        .PolicyUnderwritingCodeId = "9259",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do any operations include excavation, tunneling, underground work or earth moving?"
                    })
            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Do your subcontractors carry coverages or limits less than yours?",
                        .PolicyUnderwritingCodeId = "9260",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do your subcontractors carry coverages or limits less than yours?"
                    })
            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Are subcontractors allowed to work without providing you with a certificate of insurance?",
                        .PolicyUnderwritingCodeId = "9261",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are subcontractors allowed to work without providing you with a certificate of insurance?"
                    })
            ' General Liability - Contractors
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Does Applicant lease equipment to others with or without operators?",
                        .PolicyUnderwritingCodeId = "9038",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Contractors",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant lease equipment to others with or without operators?"
                    })

            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Does Applicant install, service or demonstrate products?",
                        .PolicyUnderwritingCodeId = "9042",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant install, service or demonstrate products?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Foreign products sold, distributed, used as components?",
                        .PolicyUnderwritingCodeId = "9043",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Foreign products sold, distributed, used as components?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Research and development conducted or new products planned?",
                        .PolicyUnderwritingCodeId = "9044",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = False,
                        .kqDescription = "Research and development conducted or new products planned?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Guarantees, warranties, hold harmless agreements?",
                        .PolicyUnderwritingCodeId = "9045",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = False,
                        .kqDescription = "Guarantees, warranties, hold harmless agreements?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Products related to aircraft/space industry?",
                        .PolicyUnderwritingCodeId = "9046",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Products related to aircraft/space industry?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Products recalled, discontinued, changed?",
                        .PolicyUnderwritingCodeId = "9047",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Products recalled, discontinued, changed?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Products of others sold or re-packaged under Applicant label?",
                        .PolicyUnderwritingCodeId = "9048",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Products of others sold or re-packaged under Applicant label?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Products under label of others?",
                        .PolicyUnderwritingCodeId = "9049",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Products under label of others?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Vendors coverage required?",
                        .PolicyUnderwritingCodeId = "9050",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = False,
                        .kqDescription = "Vendors coverage required?"
                    })
            ' General Liability - Products/Completed Operations
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Does any named insured sell to other named insured?",
                        .PolicyUnderwritingCodeId = "9051",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Liability - Products/Completed Operations",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does any named insured sell to other named insured?"
                    })

            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. List all employee benefit programs (EBPS) offered to your employees and the provider of these programs. Include any programs that are optional to your employees.",
                        .PolicyUnderwritingCodeId = "9262",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "List all employee benefit programs (EBPS) offered to your employees and the provider of these programs. Include any programs that are optional to your employees."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Which programs do you self-fund or partially self-fund? List and describe how you fund these programs.",
                        .PolicyUnderwritingCodeId = "9263",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Which programs do you self-fund or partially self-fund? List and describe how you fund these programs."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Are you planning to add additional EBPS? If yes, please describe.",
                        .PolicyUnderwritingCodeId = "9264",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are you planning to add additional EBPS? If yes, please describe."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Are you planning to terminate or modify any existing EBPS? If yes, list which programs and what you plan to terminate/modify.",
                        .PolicyUnderwritingCodeId = "9265",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are you planning to terminate or modify any existing EBPS? If yes, list which programs and what you plan to terminate/modify."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Have you changed providers for any program within the past year? If yes, which program changed and what were the provider changes made?",
                        .PolicyUnderwritingCodeId = "9266",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Have you changed providers for any program within the past year? If yes, which program changed and what were the provider changes made?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Do you plan to change providers for any program within the next year? If yes, please describe.",
                        .PolicyUnderwritingCodeId = "9267",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you plan to change providers for any program within the next year? If yes, please describe."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Are your employees eligible for the EBPS? How many? Please specify full time, part time and retirees.",
                        .PolicyUnderwritingCodeId = "9268",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are your employees eligible for the EBPS? How many? Please specify full time, part time and retirees."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Do you have a trained staff to administer the EBPS, answer questions, and advise employees concerning these programs? If yes, how many employees are in this department?",
                        .PolicyUnderwritingCodeId = "9269",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you have a trained staff to administer the EBPS, answer questions, and advise employees concerning these programs? If yes, how many employees are in this department?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8B. What training and experience do these individuals have?",
                        .PolicyUnderwritingCodeId = "9270",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "What training and experience do these individuals have?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8C. Are the individuals who handle assets of the plans bonded as required by ERISA?",
                        .PolicyUnderwritingCodeId = "9271",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are the individuals who handle assets of the plans bonded as required by ERISA?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Do you provide a published EBP manual or booklet to all employees which clearly details how the program works?",
                        .PolicyUnderwritingCodeId = "9272",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you provide a published EBP manual or booklet to all employees which clearly details how the program works?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9B. Is it updated to reflect subsequent changes?",
                        .PolicyUnderwritingCodeId = "9273",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is it updated to reflect subsequent changes?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9C. Is manual reviewed with each new employee by a trained human resources person? Please include a copy.",
                        .PolicyUnderwritingCodeId = "9274",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is manual reviewed with each new employee by a trained human resources person? Please include a copy."
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Do you require, and keep on file, signed documentation from employees noting acceptance or rejection of optional benefits?",
                        .PolicyUnderwritingCodeId = "9275",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you require, and keep on file, signed documentation from employees noting acceptance or rejection of optional benefits?"
                    })
            ' Company – Employee Benefits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Have you had a claim for this coverage within the past five years, or are you aware of a situation which could lead to a possible claim?",
                        .PolicyUnderwritingCodeId = "9276",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Company – Employee Benefits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Have you had a claim for this coverage within the past five years, or are you aware of a situation which could lead to a possible claim?"
                    })
#End Region

            Return list
        End Function

        Public Shared Function GetCommercialCPPUnderwritingQuestions_CIM() As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)

#Region "Inland Marine List"

#Region "Unused Risk Grade Questions"
            '' Risk Grade Questions
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1. Any equipment rented, loaned to others With or without operators?",
            '            .PolicyUnderwritingCodeId = "9355",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any equipment rented, loaned to others With or without operators?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Any cranes used in the applicants' operations?",
            '            .PolicyUnderwritingCodeId = "9356",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any cranes used in the applicants' operations?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "3. Are vehicles operated beyond a 200 mile radius?",
            '            .PolicyUnderwritingCodeId = "9357",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are vehicles operated beyond a 200 mile radius?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. Any radio, television or cellular towers?",
            '            .PolicyUnderwritingCodeId = "9358",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any radio, television or cellular towers?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Any prior coverage declined, cancelled or non-renewed during the prior 3 years?",
            '            .PolicyUnderwritingCodeId = "9335",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any prior coverage declined, cancelled or non-renewed during the prior 3 years?"
            '            })
#End Region

#Region "Applicant Information"

            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1A. Is the Applicant a subsidiary of another entity?",
                        .PolicyUnderwritingCodeId = "9000",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is the Applicant a subsidiary of another entity?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1B. Does the Applicant have any subsidiaries?",
                        .PolicyUnderwritingCodeId = "9001",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the Applicant have any subsidiaries?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is a formal safety program in operation?",
                        .PolicyUnderwritingCodeId = "9002",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is a formal safety program in operation?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Any exposure to flammables, explosives, chemicals?",
                        .PolicyUnderwritingCodeId = "9003",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any exposure to flammables, explosives, chemicals?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any other insurance With this company? (list policy numbers)",
                        .PolicyUnderwritingCodeId = "9005",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any other insurance With this company? (list policy numbers)"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any policy or coverage declined, cancelled or non - renewed during the prior 3 years For any premises or operations?",
                        .PolicyUnderwritingCodeId = "9006",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any policy or coverage declined, cancelled or non - renewed during the prior 3 years For any premises or operations?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9007",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. During the last five years, has any Applicant been indicted For or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection With this or any other Property?",
                        .PolicyUnderwritingCodeId = "9008",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "During the last five years, has any Applicant been indicted For or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection With this or any other Property?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Any uncorrected fire And/or safety code violations?",
                        .PolicyUnderwritingCodeId = "9009",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any uncorrected fire And/or safety code violations?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Has Applicant had a foreclosure, repossession, bankruptcy or filed For bankruptcy during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9400",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Has Applicant had a foreclosure, repossession, bankruptcy or filed For bankruptcy during the last five (5) years?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Has Applicant had a judgement or lien during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9010",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Has Applicant had a judgement or lien during the last five (5) years?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Has business been placed in a trust?",
                        .PolicyUnderwritingCodeId = "9011",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Has business been placed in a trust?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?",
                        .PolicyUnderwritingCodeId = "9012",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Does Applicant have other business ventures For which coverage is not requested?",
                        .PolicyUnderwritingCodeId = "9401",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does Applicant have other business ventures For which coverage is not requested?"
                        })

#End Region

#Region "Unused General Information"
            '' Inland_Transportation-General Information
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1. Is there a vehicle maintenance program in operation?",
            '            .PolicyUnderwritingCodeId = "9448",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is there a vehicle maintenance program in operation?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Does Applicant obtain mvr verification For drivers?",
            '            .PolicyUnderwritingCodeId = "9449",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Does Applicant obtain mvr verification For drivers?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "3. Does Applicant have a driver recruiting method?",
            '            .PolicyUnderwritingCodeId = "9450",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Does Applicant have a driver recruiting method?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. Do drivers receive regular physicals?",
            '            .PolicyUnderwritingCodeId = "9451",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Do drivers receive regular physicals?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Any waterborne shipments to be covered?",
            '            .PolicyUnderwritingCodeId = "9452",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any waterborne shipments to be covered?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "6. Are vehicles equipped With theft alarms?",
            '            .PolicyUnderwritingCodeId = "9453",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are vehicles equipped With theft alarms?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "7. Are vehicles left unlocked When unattended?",
            '            .PolicyUnderwritingCodeId = "9454",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are vehicles left unlocked When unattended?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "8. Are vehicles left loaded overnight?",
            '            .PolicyUnderwritingCodeId = "9455",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are vehicles left loaded overnight?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "9. Does Applicant back haul Property of others?",
            '            .PolicyUnderwritingCodeId = "9456",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "5",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Inland_Transportation-General Information",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Does Applicant back haul Property of others?"
            '            })
#End Region

            ' Transportation-Motor Truck Cargo Legal Liability Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Is there a vehicle maintenance program in operation?",
                        .PolicyUnderwritingCodeId = "9448",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is there a vehicle maintenance program in operation?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Does Applicant obtain MVR verification for drivers?",
                        .PolicyUnderwritingCodeId = "9449",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant obtain MVR verification for drivers?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Does Applicant have a driver recruiting method?",
                        .PolicyUnderwritingCodeId = "9517",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant have a driver recruiting method?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Do drivers receive regular physicals?",
                        .PolicyUnderwritingCodeId = "9451",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do drivers receive regular physicals?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Are vehicles equipped with theft alarms?",
                        .PolicyUnderwritingCodeId = "9518",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are vehicles equipped with theft alarms?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Are vehicles left unlocked when unattended?",
                        .PolicyUnderwritingCodeId = "9519",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are vehicles left unlocked when unattended?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Are overages, shortages, & damage claims pending?",
                        .PolicyUnderwritingCodeId = "9520",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are overages, shortages, & damage claims pending?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Are any vehicles operated for the Applicant by others?",
                        .PolicyUnderwritingCodeId = "9521",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are any vehicles operated for the Applicant by others?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Do terminals have fire protection (sprinklers, hoses etc.)?",
                        .PolicyUnderwritingCodeId = "9522",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do terminals have fire protection (sprinklers, hoses etc.)?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Do terminals have security systems (guards, alarms, fences, lights, dogs, etc.)?",
                        .PolicyUnderwritingCodeId = "9523",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do terminals have security systems (guards, alarms, fences, lights, dogs, etc.)?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Are vehicles left loaded overnight?",
                        .PolicyUnderwritingCodeId = "9524",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are vehicles left loaded overnight?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Is the Applicant an owner Operator?",
                        .PolicyUnderwritingCodeId = "9525",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is the Applicant an owner Operator?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Does the Applicant hire owner operators?",
                        .PolicyUnderwritingCodeId = "9526",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the Applicant hire owner operators?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "14. Does the Applicant triplelease to others?",
                        .PolicyUnderwritingCodeId = "9527",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the Applicant triplelease to others?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "15. Does the Applicant backhaul property of others?",
                        .PolicyUnderwritingCodeId = "9528",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "6",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Transportation Section",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the Applicant backhaul property of others?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })

            ' Equipment Floater
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Equipment rented, loaded to/from others with/without operators?",
                        .PolicyUnderwritingCodeId = "9457",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Equipment Floater",
                        .IsQuestionRequired = True,
                        .kqDescription = "Equipment rented, loaded to/from others with/without operators?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is Applicant operating equipment not listed here?",
                        .PolicyUnderwritingCodeId = "9458",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Equipment Floater",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is Applicant operating equipment not listed here?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Property used underground?",
                        .PolicyUnderwritingCodeId = "9459",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Equipment Floater",
                        .IsQuestionRequired = True,
                        .kqDescription = "Property used underground?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any work done afloat?",
                        .PolicyUnderwritingCodeId = "9460",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "7",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Equipment Floater",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any work done afloat?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })

            ' Electronic-General Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. In the event of a major or total loss could you return to operation within one week?",
                        .PolicyUnderwritingCodeId = "9461",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "In the event of a major or total loss could you return to operation within one week?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Do you have an arrangement for the use of other equipment?  (attach copy of agreement)",
                        .PolicyUnderwritingCodeId = "9462",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you have an arrangement for the use of other equipment?  (attach copy of agreement)",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Is your equipment manufacturer in a position to replace your equipment promptly?",
                        .PolicyUnderwritingCodeId = "9463",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is your equipment manufacturer in a position to replace your equipment promptly?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Is your equipment under manufacturer's warranty?",
                        .PolicyUnderwritingCodeId = "9464",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is your equipment under manufacturer's warranty?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Do you have a service maintenance contract with a manufacturer or other service contractor?",
                        .PolicyUnderwritingCodeId = "9465",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Do you have a service maintenance contract with a manufacturer or other service contractor?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Is the equipment shipped by common carrier?",
                        .PolicyUnderwritingCodeId = "9466",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is the equipment shipped by common carrier?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Is the equipment shipped by company vehicle?",
                        .PolicyUnderwritingCodeId = "9467",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is the equipment shipped by company vehicle?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Is the media/data shipped by common carrier?",
                        .PolicyUnderwritingCodeId = "9468",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is the media/data shipped by common carrier?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Is the media/data shipped by company vehicle?",
                        .PolicyUnderwritingCodeId = "9469",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is the media/data shipped by company vehicle?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Does the premises have a burglar alarm?",
                        .PolicyUnderwritingCodeId = "9470",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the premises have a burglar alarm?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11A. Does the Applicant have an uninterruptable power source to protect the hardware from power line problems?",
                        .PolicyUnderwritingCodeId = "9471",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the Applicant have an uninterruptable power source to protect the hardware from power line problems?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11B. Does the Applicant have a line conditioner to protect the hardware from power line problems?",
                        .PolicyUnderwritingCodeId = "9473",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the Applicant have a line conditioner to protect the hardware from power line problems?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11C. Does the Applicant have a power suppressor voltage regulator to protect the hardware from power line problems?",
                        .PolicyUnderwritingCodeId = "9474",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the Applicant have a power suppressor voltage regulator to protect the hardware from power line problems?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11D. Does the Applicant have a dedicated line to protect the hardware from power line problems?",
                        .PolicyUnderwritingCodeId = "9475",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "9",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – General Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the Applicant have a dedicated line to protect the hardware from power line problems?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })

            ' Electronic-Computer Room Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Is the data processing equipment located in a specifically designated room?",
                        .PolicyUnderwritingCodeId = "9476",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is the data processing equipment located in a specifically designated room?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is access to the room restricted?",
                        .PolicyUnderwritingCodeId = "9477",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is access to the room restricted?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Is the equipment controlled by a master shutdown switch?",
                        .PolicyUnderwritingCodeId = "9478",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is the equipment controlled by a master shutdown switch?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Is there a separate air conditioning system designed to specifically protect the EDP equipment?",
                        .PolicyUnderwritingCodeId = "9479",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is there a separate air conditioning system designed to specifically protect the EDP equipment?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5A. The computer room has no fire suppression systems.",
                        .PolicyUnderwritingCodeId = "9480",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "The computer room has no fire suppression systems.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5B. The computer room is protected by a wet sprinkler fire suppression system.",
                        .PolicyUnderwritingCodeId = "9481",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "The computer room is protected by a wet sprinkler fire suppression system.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5C. The computer room is protected by a dry sprinkler fire suppression system.",
                        .PolicyUnderwritingCodeId = "9482",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "The computer room is protected by a dry sprinkler fire suppression system.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5D. The computer room is protected by a HALON fire suppression system.",
                        .PolicyUnderwritingCodeId = "9483",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "The computer room is protected by a HALON fire suppression system.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5E. The computer room is protected by a CO2 fire suppression system.",
                        .PolicyUnderwritingCodeId = "9484",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "The computer room is protected by a CO2 fire suppression system.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5F. The computer room is protected by any other type of fire suppression system.",
                        .PolicyUnderwritingCodeId = "9485",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "The computer room is protected by any other type of fire suppression system.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6A. Does the computer room have a raised pedestal floor?",
                        .PolicyUnderwritingCodeId = "9486",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the computer room have a raised pedestal floor?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6B. Is the floor construction type combustible?",
                        .PolicyUnderwritingCodeId = "9487",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is the floor construction type combustible?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6C. The computer room has smoke detectors for below floor protection.",
                        .PolicyUnderwritingCodeId = "9488",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "The computer room has smoke detectors for below floor protection.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6D. The computer room has HALON or CO2 for below floor protection.",
                        .PolicyUnderwritingCodeId = "9489",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "The computer room has HALON or CO2 for below floor protection.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6E. The computer room has any other type of system for below floor protection.",
                        .PolicyUnderwritingCodeId = "9490",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "The computer room has any other type of system For below floor protection.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6F. The computer room has no below floor protection systems.",
                        .PolicyUnderwritingCodeId = "9491",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "The computer room has no below floor protection systems.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7A. Does the computer room have a Local Temperature Alarm?",
                        .PolicyUnderwritingCodeId = "9492",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the computer room have a Local Temperature Alarm?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7B. Does the computer room have a Central Temperature Alarm?",
                        .PolicyUnderwritingCodeId = "9493",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the computer room have a Central Temperature Alarm?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7C. Does the computer room have a Local Humidity Alarm?",
                        .PolicyUnderwritingCodeId = "9494",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the computer room have a Local Humidity Alarm?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7D. Does the computer room have a Central Humidity Alarm?",
                        .PolicyUnderwritingCodeId = "9495",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the computer room have a Central Humidity Alarm?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7E. Does the computer room have a Local Smoke Alarm?",
                        .PolicyUnderwritingCodeId = "9496",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the computer room have a Local Smoke Alarm?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7F. Does the computer room have a Central Smoke Alarm?",
                        .PolicyUnderwritingCodeId = "9497",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the computer room have a Central Smoke Alarm?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7G. Does the computer room have a Local Fire Alarm?",
                        .PolicyUnderwritingCodeId = "9498",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the computer room have a Local Fire Alarm?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7H. Does the computer room have a Central Fire Alarm?",
                        .PolicyUnderwritingCodeId = "9499",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Computer Room Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does the computer room have a Central Fire Alarm?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })

            ' Electronic-Media & Data (Software) Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Are anti-viral safeguards in effect?",
                        .PolicyUnderwritingCodeId = "9500",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are anti-viral safeguards in effect?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Are duplicates of software maintained?",
                        .PolicyUnderwritingCodeId = "9501",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are duplicates of software maintained?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3A. Is data backed up daily?",
                        .PolicyUnderwritingCodeId = "9502",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is Data backed up daily?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3B. Is data backed up weekly?",
                        .PolicyUnderwritingCodeId = "9503",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is Data backed up weekly?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3C. Is data backed up monthly?",
                        .PolicyUnderwritingCodeId = "9504",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is Data backed up monthly?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3D. Is data backed up quarterly?",
                        .PolicyUnderwritingCodeId = "9505",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is Data backed up quarterly?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3E. Is data backed up yearly?",
                        .PolicyUnderwritingCodeId = "9506",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is Data backed up yearly?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3F. Is data backed up at any other interval?",
                        .PolicyUnderwritingCodeId = "9507",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is Data backed up at any other interval?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3G. Are software duplicates software stored on premises?",
                        .PolicyUnderwritingCodeId = "9508",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are software duplicates software stored on premises?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3H. Is duplicate software stored off premises?",
                        .PolicyUnderwritingCodeId = "9509",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is duplicate software stored off premises?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3I. Are data backups stored on premises?",
                        .PolicyUnderwritingCodeId = "9510",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are data backups stored on premises?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3J. Are data backups stored off premises?",
                        .PolicyUnderwritingCodeId = "9511",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are data backups stored off premises?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3K. Is media and data (software) stored on premises in a safe?",
                        .PolicyUnderwritingCodeId = "9512",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is Media and Data(software) stored on premises in a safe?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3L. Is media and data (software) stored on premises in a vault?",
                        .PolicyUnderwritingCodeId = "9513",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is Media and Data(software) stored on premises in a vault?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3M. Is media and data (software) stored on premises in a computer room?",
                        .PolicyUnderwritingCodeId = "9514",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is Media and Data(software) stored on premises in a computer room?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3N. Is media and data (software) stored on premises in any other storage unit?",
                        .PolicyUnderwritingCodeId = "9515",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is Media and Data(software) stored on premises in any other storage unit?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3O. Name and address of off premises storage location:",
                        .PolicyUnderwritingCodeId = "9516",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Electronic Data Processing Section – Media & Data (Software) Info",
                        .IsQuestionRequired = True,
                        .kqDescription = "Name And address of off premises storage location:",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
#End Region

            Return list
        End Function

        Public Shared Function GetCommercialCPPUnderwritingQuestions_CRM() As List(Of VRUWQuestion)
            Dim list As New List(Of VRUWQuestion)

#Region "Crime List"


#Region "Applicant Information"
            ' Applicant Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1A. Is the Applicant a subsidiary of another entity?",
                        .PolicyUnderwritingCodeId = "9000",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is the Applicant a subsidiary of another entity?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1B. Does the Applicant have any subsidiaries?",
                        .PolicyUnderwritingCodeId = "9001",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does the Applicant have any subsidiaries?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is a formal safety program in operation?",
                        .PolicyUnderwritingCodeId = "9002",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Is a formal safety program in operation?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Any exposure to flammables, explosives, chemicals?",
                        .PolicyUnderwritingCodeId = "9003",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any exposure to flammables, explosives, chemicals?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any other insurance With this company? (list policy numbers)",
                        .PolicyUnderwritingCodeId = "9005",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any other insurance With this company? (list policy numbers)"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any policy or coverage declined, cancelled or non - renewed during the prior 3 years For any premises or operations?",
                        .PolicyUnderwritingCodeId = "9006",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any policy or coverage declined, cancelled or non - renewed during the prior 3 years For any premises or operations?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?",
                        .PolicyUnderwritingCodeId = "9007",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any past losses or claims relating to sexual abuse or molestation allegations, discrimination or negligent hiring?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. During the last five years, has any Applicant been indicted For or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection With this or any other Property?",
                        .PolicyUnderwritingCodeId = "9008",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "During the last five years, has any Applicant been indicted For or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection With this or any other Property?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Any uncorrected fire And/or safety code violations?",
                        .PolicyUnderwritingCodeId = "9009",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any uncorrected fire And/or safety code violations?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Has Applicant had a foreclosure, repossession, bankruptcy or filed For bankruptcy during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9400",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Has Applicant had a foreclosure, repossession, bankruptcy or filed For bankruptcy during the last five (5) years?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Has Applicant had a judgement or lien during the last five (5) years?",
                        .PolicyUnderwritingCodeId = "9010",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Has Applicant had a judgement or lien during the last five (5) years?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Has business been placed in a trust?",
                        .PolicyUnderwritingCodeId = "9011",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Has business been placed in a trust?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?",
                        .PolicyUnderwritingCodeId = "9012",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Any foreign operations, foreign products distributed in USA, or US products sold/distributed in foreign countries?"
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Does Applicant have other business ventures For which coverage is Not requested?",
                        .PolicyUnderwritingCodeId = "9401",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "3",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Applicant Information",
                        .IsQuestionRequired = False,
                        .kqDescription = "Does Applicant have other business ventures For which coverage is Not requested?"
                        })
#End Region

            ' Crime-General Information
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Are volunteers used? If ""Yes"", number of volunteers.",
                        .PolicyUnderwritingCodeId = "9235",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are volunteers used? If ""Yes"", number of volunteers.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Any employees leased to others?  If ""Yes"", give number and explain.",
                        .PolicyUnderwritingCodeId = "9236",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any employees leased to others?  If ""Yes"", give number and explain.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Any employees leased from others?  If ""Yes"", give number and explain.",
                        .PolicyUnderwritingCodeId = "9237",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any employees leased from others?  If ""Yes"", give number and explain.",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Any employees perform money investing or trading?",
                        .PolicyUnderwritingCodeId = "9238",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any employees perform money investing or trading?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Any employees receive or issue warehouse receipts?",
                        .PolicyUnderwritingCodeId = "9239",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any employees receive or issue warehouse receipts?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Any employee(s) been cancelled for crime coverage by any insurer?",
                        .PolicyUnderwritingCodeId = "9240",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any employee(s) been cancelled for crime coverage by any insurer?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Does Applicant have any written agreements with clients?",
                        .PolicyUnderwritingCodeId = "9241",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant have any written agreements with clients?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Does Applicant transfer any funds via phone or fax?",
                        .PolicyUnderwritingCodeId = "9242",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does Applicant transfer any funds via phone or fax?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Any exposure from loss to guest property?",
                        .PolicyUnderwritingCodeId = "9243",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "10",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "General Information",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any exposure from loss to guest property?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })

            ' Hiring Practices
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Is prior employer history checked?",
                        .PolicyUnderwritingCodeId = "9244",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Hiring Practices",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is prior employer history checked?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is education and training verified?",
                        .PolicyUnderwritingCodeId = "9245",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Hiring Practices",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is education and training verified?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Is drug testing conducted?",
                        .PolicyUnderwritingCodeId = "9246",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Hiring Practices",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is drug testing conducted?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Is a formal training program established and followed?",
                        .PolicyUnderwritingCodeId = "9247",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Hiring Practices",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is a formal training program established and followed?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Are credit checks secured for employees with access to financial transactions?",
                        .PolicyUnderwritingCodeId = "9248",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Hiring Practices",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are credit checks secured for employees with access to financial transactions?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. Are social security numbers verified?",
                        .PolicyUnderwritingCodeId = "9249",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Hiring Practices",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are social security numbers verified?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Is criminal history checked?",
                        .PolicyUnderwritingCodeId = "9250",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Hiring Practices",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is criminal history checked?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Are managers provided with names and salaries of all assigned employees?",
                        .PolicyUnderwritingCodeId = "9251",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "11",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Hiring Practices",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are managers provided with names and salaries of all assigned employees?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })

            ' Audits
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "*1. Audit is performed by: (CPA, public accountant, staff, other)",
                        .PolicyUnderwritingCodeId = "9365",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Audit is performed by: (CPA, public accountant, staff, other)",
                        .AlwaysShowDescription = True,
                        .NeverShowQuestions = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "*2. Name And address of person or firm performing audit",
                        .PolicyUnderwritingCodeId = "9366",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Name And address of person or firm performing audit",
                        .AlwaysShowDescription = True,
                        .NeverShowQuestions = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "*3. Date of completion of last audit of cash & accounts:  date of completion of last audit of inventory:",
                        .PolicyUnderwritingCodeId = "9367",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Date of completion of last audit of cash & accounts:  date of completion of last audit of inventory:",
                        .AlwaysShowDescription = True,
                        .NeverShowQuestions = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "*4. Audit frequency? (annual, semi - annual, quarterly, other)",
                        .PolicyUnderwritingCodeId = "9368",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Audit frequency? (annual, semi - annual, quarterly, other)",
                        .AlwaysShowDescription = True,
                        .NeverShowQuestions = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "*5. Audit report is rendered to: (owner, partners, board of directors, other)",
                        .PolicyUnderwritingCodeId = "9369",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Audit report is rendered to: (owner, partners, board of directors, other)",
                        .AlwaysShowDescription = True,
                        .NeverShowQuestions = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "*6. Financial format is: (audit, review, compilation, tax return only)",
                        .PolicyUnderwritingCodeId = "9370",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = False,
                        .kqDescription = "Financial format is: (audit, review, compilation, tax return only)",
                        .AlwaysShowDescription = True,
                        .NeverShowQuestions = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Are all locations audited?",
                        .PolicyUnderwritingCodeId = "9371",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are all locations audited?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Is audit made in accordance with generally accepted auditing standards and so certified? If ""No"", explain scope of audit",
                        .PolicyUnderwritingCodeId = "9372",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is audit made in accordance with generally accepted auditing standards and so certified? If ""No"", explain scope of audit",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Were any discrepancies or loose practices commented upon in this audit? If ""Yes"", submit a copy of the audit and auditor's comments",
                        .PolicyUnderwritingCodeId = "9373",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Were any discrepancies or loose practices commented upon in this audit? If ""Yes"", submit a copy of the audit and auditor's comments",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Does audit include inventory?",
                        .PolicyUnderwritingCodeId = "9374",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does audit include inventory?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "11. Are references of all new hires checked with respect to employment history?",
                        .PolicyUnderwritingCodeId = "9375",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are references of all new hires checked with respect to employment history?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "12. Does audit department have a program to detect ghost employees?",
                        .PolicyUnderwritingCodeId = "9376",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Does audit department have a program to detect ghost employees?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "13. Is payroll system audited annually?",
                        .PolicyUnderwritingCodeId = "9377",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is payroll system audited annually?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "14. Is a complete physical inventory made? If ""Yes"", how often:",
                        .PolicyUnderwritingCodeId = "9378",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is a complete physical inventory made? If ""Yes"", how often:",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "15. Is inventory made by persons who do not have custody control?",
                        .PolicyUnderwritingCodeId = "9379",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is inventory made by persons who do not have custody control?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "16. Is a requisition / shipping order required for removal of goods from storeroom / warehouse?",
                        .PolicyUnderwritingCodeId = "9380",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "12",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures - Audits",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is a requisition / shipping order required for removal of goods from storeroom / warehouse?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })

            ' Banking/Other
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Are bank accounts reconciled by someone not authorized to deposit or withdraw?",
                        .PolicyUnderwritingCodeId = "9381",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are bank accounts reconciled by someone not authorized to deposit or withdraw?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "2. Is countersignature of checks required? If not, who signs controls?",
                        .PolicyUnderwritingCodeId = "9382",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Is countersignature of checks required? If not, who signs controls?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "3. Will securities be subject to joint control of two or more responsible employees?",
                        .PolicyUnderwritingCodeId = "9383",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Will securities be subject to joint control of two or more responsible employees?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "4. Are all officers and employees required to take annual vacations of at least five consecutive business days?",
                        .PolicyUnderwritingCodeId = "9384",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are all officers and employees required to take annual vacations of at least five consecutive business days?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "5. Is there a written policy regarding EFTS?",
                        .PolicyUnderwritingCodeId = "9385",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "is there a written policy regarding EFTS?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "6. What is the largest single amount that can be transferred?",
                        .PolicyUnderwritingCodeId = "9386",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = False,
                        .kqDescription = "What is the largest single amount that can be transferred?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = False,
                        .NeverShowQuestions = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "7. Prior to funds transfer, does financial institution verify authenticity with another employee?",
                        .PolicyUnderwritingCodeId = "9387",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Prior to funds transfer, does financial institution verify authenticity with another employee?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "8. Are hard copies of funds transfer confirmations received and reconciled?",
                        .PolicyUnderwritingCodeId = "9388",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are hard copies of funds transfer confirmations received and reconciled?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "9. Frequency of deposits: (daily, other)",
                        .PolicyUnderwritingCodeId = "9389",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = False,
                        .kqDescription = "Frequency of deposits: (daily, other)",
                        .AlwaysShowDescription = True,
                        .NeverShowQuestions = True
                        })
            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "10. Are detailed records of bank deposits maintained?",
                        .PolicyUnderwritingCodeId = "9390",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "13",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Controls and Audit Procedures – Banking/Other",
                        .IsQuestionRequired = True,
                        .kqDescription = "Are detailed records of bank deposits maintained?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })

#Region "Unused Purchasing/Receiving"
            '' Crime-Purchasing/Receiving Controls
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1. Are duties segregated?",
            '            .PolicyUnderwritingCodeId = "9391",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "14",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Purchasing/Receiving Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are duties segregated?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Are departments supervised by someone Not authorized to pay bills?",
            '            .PolicyUnderwritingCodeId = "9392",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "14",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Purchasing/Receiving Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are departments supervised by someone Not authorized to pay bills?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "3. Is responsibility For checking merchandise received / controlled by more than one individual?",
            '            .PolicyUnderwritingCodeId = "9393",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "14",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Purchasing/Receiving Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is responsibility For checking merchandise received / controlled by more than one individual?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. Is actual receipt of merchandise verified before payment is made?",
            '            .PolicyUnderwritingCodeId = "9394",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "14",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Purchasing/Receiving Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is actual receipt of merchandise verified before payment is made?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Is a numbered purchase order system implemented And followed?",
            '            .PolicyUnderwritingCodeId = "9395",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "14",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Purchasing/Receiving Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is a numbered purchase order system implemented And followed?"
            '            })
#End Region

#Region "Unused Computer Fraud"
            '' Crime-Computer Fraud Controls
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1. Do internal audit procedures include computer operations?",
            '            .PolicyUnderwritingCodeId = "9396",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "15",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Computer Fraud Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Do internal audit procedures include computer operations?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Is there an employee or department whose sole duty is security?",
            '            .PolicyUnderwritingCodeId = "9397",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "15",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Computer Fraud Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is there an employee or department whose sole duty is security?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "3. Are suspicious transactions reviewed And investigated?",
            '            .PolicyUnderwritingCodeId = "9398",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "15",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Computer Fraud Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Are suspicious transactions reviewed And investigated?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. Is physical access to computer room And equipment restricted to authorized personnel?",
            '            .PolicyUnderwritingCodeId = "9399",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "15",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Computer Fraud Controls",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Is physical access to computer room And equipment restricted to authorized personnel?"
            '            })
#End Region


            ' Crime-Risk Grade Questions
#Region "Unused Risk Grade - Only use 3rd question"
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "1. Any prior coverage declined, cancelled or non - renewed during the prior 3 years?",
            '            .PolicyUnderwritingCodeId = "9345",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any prior coverage declined, cancelled or non - renewed during the prior 3 years?"
            '            })

            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "2. Have any employees been cancelled For crime coverages by any insurer?",
            '            .PolicyUnderwritingCodeId = "9351",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Have any employees been cancelled For crime coverages by any insurer?"
            '            })
#End Region

            list.Add(New VRUWQuestion() With {
                        .QuestionNumber = list.Count() + 1,
                        .Description = "1. Any prior employee dishonest claim in the past 5 years?",
                        .PolicyUnderwritingCodeId = "9352",
                        .IsTrueUwQuestion = True,
                        .PolicyUnderwritingTabId = "2",
                        .PolicyUnderwritingLevelId = "1",
                        .SectionName = "Risk Grade Questions",
                        .IsQuestionRequired = True,
                        .kqDescription = "Any prior employee dishonest claim in the past 5 years?",
                        .AlwaysShowDescription = True,
                        .DescriptionNotRequired = True
                        })

#Region "Unused Risk Grade - Only use 3rd question"
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "4. During the last 5 years, has any Applicant been indicted For or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection With this or any other Property?",
            '            .PolicyUnderwritingCodeId = "9353",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "During the last 5 years, has any Applicant been indicted For or convicted of any degree of the crime of fraud, bribery, arson or any other arson-related crime in connection With this or any other Property?"
            '            })
            'list.Add(New VRUWQuestion() With {
            '            .QuestionNumber = list.Count() + 1,
            '            .Description = "5. Any bankruptcies, tax or credit liens against the Applicant in the past 5 years?",
            '            .PolicyUnderwritingCodeId = "9354",
            '            .IsTrueUwQuestion = True,
            '            .PolicyUnderwritingTabId = "2",
            '            .PolicyUnderwritingLevelId = "1",
            '            .SectionName = "Crime-Risk Grade Questions",
            '            .IsQuestionRequired = False,
            '            .kqDescription = "Any bankruptcies, tax or credit liens against the Applicant in the past 5 years?"
            '            })
#End Region

#End Region

            Return list
        End Function

    End Class

    <Serializable()>
    Public Class VRUWQuestion
        Inherits QuickQuotePolicyUnderwriting
        Implements ICloneable

        Public Property QuestionNumber As Int32
        Public Property Description As String
        ''' <summary>
        ''' Removes the question number for the description if it exists.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Description_NoQuestionNumber As String
            Get
                Dim d = Description
                If String.IsNullOrWhiteSpace(d) = False AndAlso IsNumeric(d(0)) Then
                    d = d.Remove(0, d.IndexOf(" ") + 1) 'should look for "."?
                End If
                Return d
            End Get
        End Property
        Public Property kqDescription As String
        Public Property IsTrueKillQuestion As Boolean
        Public Property IsTrueUwQuestion As Boolean
        Public Property SectionName As String
        Public Property QuestionAnswerYes As Boolean
        Public Property QuestionAnswerNo As Boolean
        Public Property NeverShowDescription As Boolean
        Public Property AlwaysShowDescription As Boolean
        Public Property IsQuestionRequired As Boolean
        Public Property DescriptionNotRequired As Boolean
        Public Property NeverShowQuestions As Boolean
        Public Property ShowDescriptionOnNo As Boolean
        Public Property ShowDescriptionOnYes As Boolean = True
        Public Property LobType As QuickQuoteObject.QuickQuoteLobType
        Public Property GoverningState As QuickQuote.CommonMethods.QuickQuoteHelperClass.QuickQuoteState
        Public Property MinimumEffectiveDate As Date?
        Public Property ReferToUnderwritingOnYes As Boolean
        Public Property ReferToUnderwritingOnNo As Boolean
        Public Property ConfirmRiskOnYes As Boolean
        Public Property ConfirmRiskOnNo As Boolean
        Public Property IsUnmapped As Boolean
        ''' <summary>
        ''' Don't show this question
        ''' </summary>
        ''' <returns></returns>
        Public Property HideFromDisplay As Boolean
        ''' <summary>
        ''' Default answer value if hidden. If this is Empty, "-1" will be used during save
        ''' </summary>
        ''' <returns></returns>
        Public Property DefaultValueIfHidden As String
        Public Property DetailText As String
            Get
                Return Me.PolicyUnderwritingExtraAnswer
            End Get
            Set(value As String)
                Me.PolicyUnderwritingExtraAnswer = value
            End Set
        End Property

        Public ReadOnly Property Answer As String
            Get
                Return IIf(QuestionAnswerYes, "YES", IIf(QuestionAnswerNo, "NO", String.Empty))
            End Get
        End Property

        ''' <summary>
        ''' deprecated. use <see cref="DetailText"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property DetailTextOnQuestionYes As String
            Get
                Return Me.PolicyUnderwritingExtraAnswer
            End Get
            Set(value As String)
                Me.PolicyUnderwritingExtraAnswer = value
            End Set
        End Property
        Public ReadOnly Property HasBeenAnswered As Boolean
            Get
                Return String.IsNullOrWhiteSpace(Me.QuestionAnswerYes) = False OrElse
                        String.IsNullOrWhiteSpace(Me.QuestionAnswerNo) = False
            End Get
        End Property

        Public Function Clone() As Object Implements ICloneable.Clone
            Return Me.MemberwiseClone()
        End Function
    End Class

End Namespace