Namespace IFM.VR.Common.Helpers
    Public Class MineSubsidenceHelper

        Public Enum OhioMineSubsidenceType_enum
            EligibleMandatory
            EligibleOptional
            Ineligible
        End Enum

        Public Enum IllinoisMineSubsidenceType_enum
            EligibleMandatory
            EligibleOptional
        End Enum

        <Obsolete("This method is deprecated. Use MineSubCountiesByStateAbbreviation() or MineSubCountiesByStateId()")>
        Public Shared ReadOnly Property MineSubCounties() As List(Of String)
            'TODO - Matt - MS - Add support for additional states
            Get
                Dim msCounties As List(Of String) = New List(Of String)(New String() _
                                            {"CLAY", "CRAWFORD", "DAVIESS", "DUBOIS", "FOUNTAIN", "GIBSON", "GREENE", "KNOX", "LAWRENCE", "MARTIN", "MONROE", "MONTGOMERY", "ORANGE", "OWEN",
                                             "PARKE", "PERRY", "PIKE", "POSEY", "PUTNAM", "SPENCER", "SULLIVAN", "VANDERBURGH", "VERMILLION", "VIGO", "WARREN", "WARRICK"})
                Return msCounties
            End Get
        End Property

        Public Shared Function LocationAllowsMineSubsidence(location As QuickQuote.CommonObjects.QuickQuoteLocation) As Boolean
            If location IsNot Nothing Then
                If location.Address IsNot Nothing AndAlso location.Address.County IsNot Nothing AndAlso String.IsNullOrWhiteSpace(location?.Address?.StateId) = False Then
                    Return MineSubCountiesByStateId(location.Address.StateId).Contains(location.Address.County.ToUpper().Trim())
                End If
            End If
            Return False
        End Function

        <Obsolete("This method is deprecated.")>
        Public Shared Function LocationAllowsMineSubsidence(countyName As String) As Boolean
            'TODO - Matt - MS - Add support for additional states
            Return MineSubCounties.Contains(countyName.ToUpper().Trim())
        End Function

        Public Shared Function MineSubCountiesByStateAbbreviation(state As String) As List(Of String)

            Dim stateInfo = IFM.VR.Common.Helpers.States.GetStateInfoFromAbbreviation(state)
            If Not stateInfo.IsEmpty Then
                Select Case stateInfo.StateNameEnum
                    Case States.StateNames.Indiana
                        Return New List(Of String)(New String() _
                                            {"CLAY", "CRAWFORD", "DAVIESS", "DUBOIS", "FOUNTAIN", "GIBSON", "GREENE", "KNOX", "LAWRENCE", "MARTIN", "MONROE", "MONTGOMERY", "ORANGE", "OWEN",
                                             "PARKE", "PERRY", "PIKE", "POSEY", "PUTNAM", "SPENCER", "SULLIVAN", "VANDERBURGH", "VERMILLION", "VIGO", "WARREN", "WARRICK"})
                    Case States.StateNames.Illinois
                        Return New List(Of String)(New String() _
                                            {"BOND", "BUREAU", "CHRISTIAN", "CLINTON", "DOUGLAS", "FRANKLIN", "FULTON", "GALLATIN", "GRUNDY", "JACKSON", "JEFFERSON", "KNOX", "LASALLE",
                                            "LOGAN", "MCDONOUGH", "MACOUPIN", "MADISON", "MARION", "MARSHALL", "MENARD", "MERCER", "MONTGOMERY", "PEORIA", "PERRY", "PUTNAM",
                                            "RANDOLPH", "ROCK ISLAND", "ST. CLAIR", "SALINE", "SANGAMON", "TAZEWELL", "VERMILION", "WASHINGTON", "WILLIAMSON"})
                    Case States.StateNames.Ohio
                        Return New List(Of String)(New String() _
                                            {"ATHENS", "BELMONT", "CARROLL", "COLUMBIANA", "COSHOCTON", "GALLIA", "GUERNSEY", "HARRISON", "HOCKING", "HOLMES", "JACKSON", "JEFFERSON", "LAWRENCE",
                                            "MAHONING", "MEIGS", "MONROE", "MORGAN", "MUSKINGUM", "NOBLE", "PERRY", "SCIOTO", "STARK", "TRUMBULL", "TUSCARAWAS", "VINTON", "WASHINGTON"})
                End Select
            End If

            Return New List(Of String)
        End Function

        Public Shared Function MineSubCountiesByStateId(stateId As Int32) As List(Of String)
            Dim stateInfo = IFM.VR.Common.Helpers.States.GetStateInfoFromId(stateId)
            If Not stateInfo.IsEmpty Then
                Return MineSubCountiesByStateAbbreviation(stateInfo.Abbreviation)
            End If
            Return New List(Of String)
        End Function

        Public Shared Function GetOhioMineSubsidenceTypeByCounty(ByVal CountyName As String) As OhioMineSubsidenceType_enum
            Dim county As String = CountyName.Trim.ToUpper

            Select Case county
                Case "ATHENS", "BELMONT", "CARROLL", "COLUMBIANA", "COSHOCTON", "GALLIA", "GUERNSEY", "HARRISON", "HOCKING", "HOLMES", "JACKSON", "JEFFERSON", "LAWRENCE", "MAHONING", "MEIGS", "MONROE", "MORGAN", "MUSKINGUM", "NOBLE", "PERRY", "SCIOTO", "STARK", "TRUMBULL", "TUSCARAWAS", "VINTON", "WASHINGTON"
                    Return OhioMineSubsidenceType_enum.EligibleMandatory
                Case "DELAWARE", "ERIE", "GEAUGA", "LAKE", "LICKING", "MEDINA", "OTTAWA", "PORTAGE", "PREBLE", "SUMMIT", "WAYNE"
                    Return OhioMineSubsidenceType_enum.EligibleOptional
                Case Else
                    Return OhioMineSubsidenceType_enum.Ineligible
            End Select
        End Function

        Public Shared Function GetIllinoisMineSubsidenceTypeByCounty(ByVal CountyName As String) As IllinoisMineSubsidenceType_enum
            Dim county As String = CountyName.Trim.ToUpper

            Select Case county
                Case "BOND", "BUREAU", "CHRISTIAN", "CLINTON", "DOUGLAS", "FRANKLIN", "FULTON", "GALLATIN", "GRUNDY", "JACKSON", "JEFFERSON", "KNOX", "LASALLE", "LOGAN", "MCDONOUGH", "MACOUPIN", "MADISON", "MARION", "MARSHALL", "MENARD", "MERCER", "MONTGOMERY", "PEORIA", "PERRY", "PUTNAM", "RANDOLPH", "ROCK ISLAND", "ST. CLAIR", "SALINE", "SANGAMON", "TAZEWELL", "VERMILION", "WASHINGTON", "WILLIAMSON"
                    Return IllinoisMineSubsidenceType_enum.EligibleMandatory
                Case Else
                    Return IllinoisMineSubsidenceType_enum.EligibleOptional
            End Select
        End Function

        ''' <summary>
        ''' OHIO ONLY
        ''' Determines whether a building class code is eliigible for mine subsidence coverage.
        ''' If Number of Units is required, the RequiresNumberOfUnits flag will be set to true.
        ''' </summary>
        ''' <param name="ClassCode"></param>
        ''' <param name="RequiresNumberOfUnits"></param>
        ''' <returns></returns>
        Public Shared Function OhioBuildingClassCodeIsEligibleForMineSubsidence(ByVal ClassCode As String, ByRef RequiresNumberOfUnits As Boolean)
            RequiresNumberOfUnits = False
            Select Case ClassCode
                Case "65141", "65144"
                    Return True
                    Exit Select
                Case "69145"
                    RequiresNumberOfUnits = True
                    Return True
                    Exit Select
            End Select
            Return False
        End Function

    End Class
End Namespace