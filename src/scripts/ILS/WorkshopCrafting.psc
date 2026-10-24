Scriptname ILS:WorkshopCrafting Extends Quest

; Temporarily supply Lodge resources to homes, outposts and ship workbenches.
ObjectReference Property LodgeInventory Auto Const
Keyword Property WorkshopItemKeyword Auto Const
Keyword Property WorkbenchContainerKeyword Auto Const Mandatory
ActorValue Property BuildContainerValue Auto Const
Keyword Property AccessPointLink Auto Const Mandatory
Keyword Property ShipAccessPointLink Auto Const Mandatory
Perk Property ActivationPerk Auto Const Mandatory

Bool Initialized
Bool WorkbenchActive
Bool OpeningWorkbench
ObjectReference BuildingWorkshop
ObjectReference ConnectedWorkshop
ObjectReference ConnectedWorkbench
ObjectReference PreviousWorkshop
Float PreviousBuildContainerValue

Event OnQuestInit()
    Initialize()
EndEvent

; Restore any saved session before registering the player's interactions.
Function Initialize(Bool reload = False)
    If !reload && Initialized
        Return
    EndIf
    Initialized = True
    DisconnectStorage()
    WorkbenchActive = False
    OpeningWorkbench = False
    BuildingWorkshop = None
    Actor player = Game.GetPlayer()
    If ActivationPerk != None && !player.HasPerk(ActivationPerk)
        player.AddPerk(ActivationPerk)
    EndIf
    RegisterForMenuOpenCloseEvent("WeaponsCraftingMenu")
    RegisterForMenuOpenCloseEvent("ArmorCraftingMenu")
    RegisterForMenuOpenCloseEvent("IndustrialCraftingMenu")
    RegisterForMenuOpenCloseEvent("FoodCraftingMenu")
    RegisterForMenuOpenCloseEvent("DrugsCraftingMenu")
    RegisterForMenuOpenCloseEvent("ResearchMenu")
EndFunction

; Built-in ship furniture may lack the workshop link carried by decorated objects.
ObjectReference Function GetWorkshopFor(ObjectReference item)
    ObjectReference workshop = item.GetWorkshop()
    If workshop == None
        workshop = item.GetLinkedRef(WorkshopItemKeyword)
    EndIf
    If workshop == None
        workshop = item.GetLinkedRef(AccessPointLink)
    EndIf
    If workshop == None
        SpaceshipReference ship = item.GetCurrentShipRef()
        If ship != None && ship == Game.GetPlayerHomeSpaceShip()
            ; The ship quest's workshop alias can be empty. Our access points
            ; retain their actual workshop and index themselves by their ship.
            ObjectReference[] points = ship.GetRefsLinkedToMe(ShipAccessPointLink)
            Int index = 0
            While index < points.Length
                ObjectReference point = points[index]
                If point != None && !point.IsDeleted() && !point.IsDisabled()
                    workshop = point.GetLinkedRef(AccessPointLink)
                    If workshop != None
                        Return workshop
                    EndIf
                EndIf
                index += 1
            EndWhile
        EndIf
    EndIf
    Return workshop
EndFunction

; Access points discover their actual workshop, including dynamically created outposts.
Function WatchWorkshop(ObjectReference workshop, ObjectReference accessPoint, Bool placed = False)
    Initialize()
    If workshop == None || accessPoint == None || AccessPointLink == None
        Return
    EndIf
    ; This index link belongs to our own object and disappears with that object.
    accessPoint.SetLinkedRef(workshop, AccessPointLink)
    accessPoint.SetLinkedRef(accessPoint.GetCurrentShipRef(), ShipAccessPointLink)
    RegisterForRemoteEvent(workshop, "OnWorkshopMode")
    RegisterForRemoteEvent(workshop, "OnWorkshopObjectRemoved")
    RegisterForRemoteEvent(workshop, "OnObjectDestroyed")
    ; A newly built access point becomes usable during the current build session.
    If placed
        BuildingWorkshop = workshop
        ConnectStorage(workshop)
    EndIf
EndFunction

; ReplaceDefault prevents the engine from opening a menu ahead of this script.
Function OpenWorkbench(ObjectReference workbench)
    Initialize()
    If workbench == None || OpeningWorkbench
        Return
    EndIf
    ; Guard only this call. A rejected activation must not block other workbenches.
    OpeningWorkbench = True
    ; Blocked furniture may still rely on OnActivate. Forward that activation
    ; normally, without supplying resources or changing another mod's block.
    If !workbench.IsActivationBlocked()
        WorkbenchActive = True
        ObjectReference workshop = workbench.GetWorkshop()
        If workshop == None
            workshop = workbench.GetLinkedRef(WorkshopItemKeyword)
        EndIf
        SpaceshipReference ship = workbench.GetCurrentShipRef()
        If workshop == None && ship != None && ship == Game.GetPlayerHomeSpaceShip()
            workshop = GetWorkshopFor(workbench)
            DisconnectStorage()
            ; Only ship furniture without a workshop assignment needs a direct link.
            ; Add a direct link only when free, and remove only the link created here.
            If HasLodgeAccessPoint(workshop) && LodgeInventory != None && workbench.GetLinkedRef(WorkbenchContainerKeyword) == None
                ConnectedWorkbench = workbench
                workbench.SetLinkedRef(LodgeInventory, WorkbenchContainerKeyword)
            EndIf
        Else
            ConnectStorage(workshop)
        EndIf
    EndIf
    ; Suspend only our perk to avoid intercepting this forwarded activation.
    ; Normal processing sends OnActivate and preserves other mods' activation paths.
    Actor player = Game.GetPlayer()
    player.RemovePerk(ActivationPerk)
    Bool activated = workbench.Activate(player)
    player.AddPerk(ActivationPerk)
    If !activated
        WorkbenchActive = False
        DisconnectStorage()
    EndIf
    OpeningWorkbench = False
EndFunction

Event OnMenuOpenCloseEvent(String asMenuName, Bool abOpening)
    If abOpening || !WorkbenchActive
        Return
    EndIf
    WorkbenchActive = False
    DisconnectStorage()
EndEvent

Event ObjectReference.OnWorkshopMode(ObjectReference akSender, Bool aStart)
    Initialize()
    If aStart
        BuildingWorkshop = akSender
        ConnectStorage(akSender)
    ElseIf BuildingWorkshop == akSender
        BuildingWorkshop = None
        DisconnectStorage()
    EndIf
EndEvent

Event ObjectReference.OnWorkshopObjectRemoved(ObjectReference akSender, ObjectReference akReference)
    ; Remove our index entry before checking whether another access point remains.
    If akReference != None && akReference.GetLinkedRef(AccessPointLink) == akSender
        akReference.SetLinkedRef(None, AccessPointLink)
        akReference.SetLinkedRef(None, ShipAccessPointLink)
    EndIf
    RefreshWorkshop(akSender)
EndEvent

Event ObjectReference.OnObjectDestroyed(ObjectReference akSender, ObjectReference akReference)
    RefreshWorkshop(akSender)
EndEvent

; Access points also request this check when their own 3D is removed.
Function RefreshWorkshop(ObjectReference workshop)
    If workshop != None && BuildingWorkshop == workshop
        ConnectStorage(workshop)
    EndIf
EndFunction

; Register an additional resource container without changing any workbench or outpost container.
Function ConnectStorage(ObjectReference workshop)
    If !HasLodgeAccessPoint(workshop)
        DisconnectStorage()
        Return
    EndIf
    If LodgeInventory == None || BuildContainerValue == None
        Return
    EndIf
    If ConnectedWorkshop == workshop
        Return
    EndIf
    DisconnectStorage()
    ObjectReference previous = LodgeInventory.GetLinkedRef(WorkshopItemKeyword)
    ; Respect a different mod's workshop assignment of the Lodge safe itself.
    If previous != None && previous != workshop
        Return
    EndIf
    PreviousWorkshop = previous
    PreviousBuildContainerValue = LodgeInventory.GetValue(BuildContainerValue)
    ConnectedWorkshop = workshop
    LodgeInventory.SetValue(BuildContainerValue, 1)
    LodgeInventory.SetLinkedRef(workshop, WorkshopItemKeyword)
EndFunction

; Remove our temporary ship-workbench link and restore the safe's resource registration.
Function DisconnectStorage()
    ObjectReference workbench = ConnectedWorkbench
    ConnectedWorkbench = None
    If workbench != None && workbench.GetLinkedRef(WorkbenchContainerKeyword) == LodgeInventory
        workbench.SetLinkedRef(None, WorkbenchContainerKeyword)
    EndIf
    ObjectReference workshop = ConnectedWorkshop
    If workshop == None
        Return
    EndIf
    ConnectedWorkshop = None
    If LodgeInventory.GetLinkedRef(WorkshopItemKeyword) == workshop
        LodgeInventory.SetLinkedRef(PreviousWorkshop, WorkshopItemKeyword)
        LodgeInventory.SetValue(BuildContainerValue, PreviousBuildContainerValue)
    EndIf
    PreviousWorkshop = None
EndFunction

; The native index contains only ILS access points, even in an outpost with thousands of objects.
Bool Function HasLodgeAccessPoint(ObjectReference akWorkshop)
    If akWorkshop == None || AccessPointLink == None
        Return False
    EndIf
    ObjectReference[] objects = akWorkshop.GetRefsLinkedToMe(AccessPointLink)
    Int index = 0
    While index < objects.Length
        ObjectReference item = objects[index]
        If item != None && !item.IsDeleted() && !item.IsDisabled()
            Return True
        EndIf
        index += 1
    EndWhile
    Return False
EndFunction
