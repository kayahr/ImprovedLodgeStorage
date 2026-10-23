// SPDX-FileCopyrightText: 2026 Klaus Reimer <k@ailis.de>
// SPDX-License-Identifier: MIT

using Mutagen.Bethesda.FormKeys.Starfield;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace ImprovedLodgeStorage.Workshops;

/// <summary>Builds temporary Lodge resource access for crafting and building in homes and outposts.</summary>
internal sealed class WorkshopCrafting
{
    /// <summary>Adds a hidden controller and a perk replacing player-built workbench activation.</summary>
    /// <param name="mod">Plugin receiving the controller quest and activation perk.</param>
    public void Build(StarfieldMod mod)
    {
        // Only our access points use this link, allowing native lookup without scanning the outpost.
        mod.Keywords.Add(new Keyword(Forms.WorkshopAccessPointLink, StarfieldRelease.Starfield)
        {
            EditorID = "ILS_WorkshopAccessPointLink"
        });

        var script = new ScriptEntry { Name = "ILS:WorkshopCrafting" };
        script.Properties.Add(Property("LodgeInventory", Forms.BedroomInventory));
        script.Properties.Add(Property("WorkshopItemKeyword", Starfield.Keyword.WorkshopItemKeyword.FormKey));
        script.Properties.Add(Property("BuildContainerValue", Starfield.ActorValueInformation.OutpostBuildContainer.FormKey));
        script.Properties.Add(Property("AccessPointLink", Forms.WorkshopAccessPointLink));
        script.Properties.Add(Property("ActivationPerk", Forms.WorkbenchActivation));

        var player = new QuestReferenceAlias
        {
            ID = 0,
            Name = "Player",
            Flags = AQuestAlias.Flag.AllowReserved
        };
        player.ForcedReference.SetTo(Forms.PlayerReference);
        var playerBinding = Property("", Forms.WorkshopCrafting);
        playerBinding.Alias = 0;

        // The player alias receives save-load events independently of any access point.
        // This controller has no journal entry, objectives or stages.
        mod.Quests.Add(new Quest(Forms.WorkshopCrafting, StarfieldRelease.Starfield)
        {
            EditorID = "ILS_WorkshopCrafting",
            Data = new QuestData { Flags = Quest.Flag.StartGameEnabled | Quest.Flag.RunOnce },
            Aliases = [player],
            VirtualMachineAdapter = new QuestAdapter
            {
                Scripts = { script },
                Aliases =
                {
                    new QuestFragmentAlias
                    {
                        Property = playerBinding,
                        Scripts = { new ScriptEntry { Name = "ILS:CraftingPlayer" } }
                    }
                }
            }
        });

        // Target conditions apply to current and future workshop furniture without editing its records.
        var workbench = new HasKeywordConditionData();
        workbench.FirstParameter.Link.SetTo(Starfield.Keyword.AnimFurnWorkbench.FormKey);
        var workshopItem = new HasLinkedRefConditionData();
        workshopItem.FirstParameter.Link.SetTo(Starfield.Keyword.WorkshopItemKeyword.FormKey);
        var activation = new PerkEntryPointAddActivateChoice
        {
            EntryPoint = APerkEntryPointEffect.EntryType.Activate,
            PerkConditionTabCount = 2,
            PerkEntryID = 0,
            // Wait for the player to select Craft before running the script.
            Flags = APerkEffect.Flag.ReplaceDefault,
            ButtonLabel = "Craft",
            Conditions =
            {
                new PerkCondition
                {
                    RunOnTabIndex = 1, // The activated target; tab 0 is the perk owner.
                    Conditions =
                    {
                        new ConditionFloat { ComparisonValue = 1, Data = workbench },
                        new ConditionFloat { ComparisonValue = 1, Data = workshopItem }
                    }
                }
            }
        };
        var interaction = new ScriptEntry { Name = "ILS:WorkbenchActivation" };
        interaction.Properties.Add(Property("Controller", Forms.WorkshopCrafting));
        mod.Perks.Add(new Perk(Forms.WorkbenchActivation, StarfieldRelease.Starfield)
        {
            EditorID = "ILS_WorkbenchActivation",
            MajorFlags = Perk.MajorFlag.NonPlayable,
            VirtualMachineAdapter = new PerkAdapter { Scripts = { interaction } },
            Ranks = { new PerkRank { Effects = { activation } } }
        });
    }

    /// <summary>Creates an initialized reference, base-object or keyword property.</summary>
    /// <param name="name">Property name declared in the Papyrus script.</param>
    /// <param name="key">Record assigned to the property.</param>
    /// <returns>A script property pointing to the specified record.</returns>
    private static ScriptObjectProperty Property(string name, FormKey key)
    {
        var property = new ScriptObjectProperty
        {
            Name = name
        };
        property.Object.SetTo(key);
        return property;
    }
}
