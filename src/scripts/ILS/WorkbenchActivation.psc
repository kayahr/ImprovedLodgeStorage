Scriptname ILS:WorkbenchActivation Extends Perk

ILS:WorkshopCrafting Property Controller Auto Const Mandatory

; ReplaceDefault routes the chosen interaction through the controller, only on key press.
Event OnEntryRun(Int auiEntryID, ObjectReference akTarget, Actor akOwner)
    If akOwner == Game.GetPlayer() && Controller != None
        Controller.OpenWorkbench(akTarget)
    EndIf
EndEvent
