
Namespace IFM.VR.Common.Workflow
    Public Class Workflow
        Public Const WorkFlowSection_qs As String = "Workflow"

        Public Shared Function isAppOrQuote(QuoteID As String) As WorkflowAppOrQuote '7/17/2019 note: quoteId can now be policyIdAndImageNum for ReadOnly/Endorsements
            Dim returnVar As WorkflowAppOrQuote = WorkflowAppOrQuote.NA

            If String.IsNullOrWhiteSpace(QuoteID) = False Then
                Dim sessionVarName As String = $"AppOrQuote_{QuoteID}"
                returnVar = Web.HttpContext.Current.Session(sessionVarName)
            End If

            Return returnVar
        End Function

        Public Shared Sub SetAppOrQuote(AppOrQuote As WorkflowAppOrQuote, QuoteID As String) '7/17/2019 note: quoteId can now be policyIdAndImageNum for ReadOnly/Endorsements
            If String.IsNullOrWhiteSpace(QuoteID) = False Then
                Dim sessionVarName As String = $"AppOrQuote_{QuoteID}"
                Select Case AppOrQuote
                    Case WorkflowAppOrQuote.Quote
                        Web.HttpContext.Current.Session(sessionVarName) = WorkflowAppOrQuote.Quote
                    Case WorkflowAppOrQuote.App
                        Web.HttpContext.Current.Session(sessionVarName) = WorkflowAppOrQuote.App
                    Case WorkflowAppOrQuote.NA
                        Web.HttpContext.Current.Session(sessionVarName) = WorkflowAppOrQuote.NA
                    Case WorkflowAppOrQuote.Endorsement
                        Web.HttpContext.Current.Session(sessionVarName) = WorkflowAppOrQuote.Endorsement
                End Select
            End If
        End Sub

        Public Enum WorkflowAppOrQuote
            NA
            App
            Quote
            Endorsement
        End Enum

        Public Enum WorkflowSection
            na = 1000
            policyHolders = 0
            drivers = 1
            vehicles = 2
            coverages = 3
            summary = 4
            uwQuestions = 5
            app = 6
            property_ = 7
            location = 8
            farmIRPM = 9
            farmPP = 10
            InlandMarine = 11
            buildings = 12
            fileUpload = 13
            documentPrinting = 14
            CPP_CPR_Coverages = 15   ' Added the CPP Coverages and Locations MGB 2-7-18
            CPP_CGL_Coverages = 16
            CPP_CPR_Locations = 17
            CPP_CGL_Locations = 18
            Crime = 19
            proposal = 20
            newQuoteSelection = 21
            printHistory = 22
            policyHistory = 23
            billingInformation = 24
            printFriendlySummary = 25
            printFriendly = 25
        End Enum

        Public Shared Function GetWorkflowName(wf As WorkflowSection, lob As QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType, IsOnApp As Boolean) As String
            Select Case wf
                Case WorkflowSection.policyHolders
                    Return "Policyholders"
                Case WorkflowSection.drivers
                    Return "Drivers"
                Case WorkflowSection.vehicles
                    Return "Vehicles"

                Case WorkflowSection.coverages
                    Select Case lob
                        Case QuickQuote.CommonObjects.QuickQuoteObject.QuickQuoteLobType.Farm
                            Return "Policy Level Coverages"
                        Case Else
                            Return "Coverages"
                    End Select
                Case WorkflowSection.CPP_CGL_Coverages
                    Return "CGL Policy Coverages"
                Case WorkflowSection.CPP_CPR_Coverages
                    Return "CPR Policy Coverages"
                Case WorkflowSection.property_
                    Return "Property"

                Case WorkflowSection.location
                    Return "Locations"
                Case WorkflowSection.CPP_CGL_Locations
                    Return "CGL Locations"
                Case WorkflowSection.CPP_CPR_Locations
                    Return "CPR Locations"
                Case WorkflowSection.buildings
                    Return "Buildings"
                Case WorkflowSection.farmPP
                    Return "Farm Personal Property (F and G)"
                Case WorkflowSection.InlandMarine
                    Return "Inland Marine / Rv/Watercraft"


                Case WorkflowSection.summary
                    Return If(IsOnApp, "Application Summary", "Summary")


                Case WorkflowSection.farmIRPM
                    Return "IRPM"


                Case WorkflowSection.uwQuestions
                    Return "Underwriting Questions"
                Case WorkflowSection.app
                    Return "Application"
                Case WorkflowSection.fileUpload
                    Return "Upload a File"
                Case WorkflowSection.documentPrinting
                    Return "Document Printing"
                Case WorkflowSection.proposal
                    Return "Proposal"
                Case WorkflowSection.newQuoteSelection
                    Return "Select a New Quote"
                Case WorkflowSection.billingInformation
                    Return "Billing Information"
                Case WorkflowSection.printFriendlySummary 'Added 01/12/2021 for CAP Endorsements Task 52976 MLW 'modded by KLJ
                    Return "Printer Friendly"

                Case WorkflowSection.na
#If DEBUG Then
                    Debugger.Break() ' have a problem if you are here
#End If
                    Return "Unknown"
            End Select
        End Function

    End Class
End Namespace


