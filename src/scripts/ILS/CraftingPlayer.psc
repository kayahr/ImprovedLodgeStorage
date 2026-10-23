Scriptname ILS:CraftingPlayer Extends ReferenceAlias

; Initialization is guarded in the controller; either first-start event may arrive first.
Event OnInit()
    ILS:WorkshopCrafting controller = GetOwningQuest() as ILS:WorkshopCrafting
    controller.Initialize()
EndEvent

Event OnPlayerLoadGame()
    ILS:WorkshopCrafting controller = GetOwningQuest() as ILS:WorkshopCrafting
    controller.Initialize(True)
EndEvent
