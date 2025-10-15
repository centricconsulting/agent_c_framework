Public Class EndorsementStructures
    Structure EndorsementTypeString

        Const CAP_AmendMailing = "Amend Mailing Address"
        Const CAP_AddDeleteVehicle = "Add/Delete Vehicle"
        Const CAP_AddDeleteDriver = "Add/Delete Driver"
        Const CAP_AddDeleteAI = "Add/Delete Additional Interest"

        Const BOP_AmendMailing = "Amend Mailing Address"
        Const BOP_AddDeleteLocationLienholder = "Add/Delete a location(s) or lienholder(s) on property"
        Const BOP_AddDeleteLocation = "Add/Delete a location(s)"
        Const BOP_AddDeleteContractorsEquipmentLienholder = "Add/Delete a lienholder on contractors’ equipment" 'Change Like CPP?
        Const BOP_AddDeleteContractorsEquipment = "Add/Delete contractors' equipment"

        Const CPP_AmendMailing = "Amend Mailing Address"
        Const CPP_AddDeleteLocationLienholder = "Add/Delete a location(s) or lienholder(s) on property"
        Const CPP_AddDeleteLocation = "Add/Delete a location(s)"
        Const CPP_AddDeleteContractorsEquipmentLienholder = "Add/Delete scheduled equipment or lienholders(s) on scheduled equipment"
        Const CPP_AddDeleteContractorsEquipment = "Add/Delete contractors' equipment"

    End Structure
End Class
