// SPDX-FileCopyrightText: 2026 Klaus Reimer <k@ailis.de>
// SPDX-License-Identifier: MIT

using Mutagen.Bethesda.FormKeys.Starfield;
using Mutagen.Bethesda.Plugins;

namespace ImprovedLodgeStorage;

/// <summary>Stable record identities not provided by the game's FormKeys registry.</summary>
internal static class Forms
{
    public static readonly ModKey Plugin = ModKey.FromNameAndExtension("ImprovedLodgeStorage.esm");

    /// <summary>Creates a record identity owned by this plugin.</summary>
    /// <param name="id">Local FormID within ImprovedLodgeStorage.esm.</param>
    /// <returns>The FormKey identifying the plugin's record.</returns>
    public static FormKey Own(uint id) => new(Plugin, id);

    /// <summary>Creates a record identity owned by the game master.</summary>
    /// <param name="id">Local FormID within Starfield.esm.</param>
    /// <returns>The FormKey identifying the original game record.</returns>
    public static FormKey Vanilla(uint id) => new(Starfield.ModKey, id);

    // Original placed references without named entries in the FormKeys registry
    public static readonly FormKey PlayerReference = Vanilla(0x14);
    public static readonly FormKey BedroomInventory = Vanilla(0x266E81);
    public static readonly FormKey[] LodgeWorkbenches =
    [
        Vanilla(0x004A2C), // Spacesuit workbench
        Vanilla(0x004A2D), // Pharmaceutical lab
        Vanilla(0x004A2E), // Cooking station
        Vanilla(0x004A2F), // Industrial workbench
        Vanilla(0x004A30), // Research lab
        Vanilla(0x004A31)  // Weapon workbench
    ];

    // Our fixed access-point bases and the references placing them in the world
    public static readonly FormKey BedroomAccess = Own(0x805);
    public static readonly FormKey BasementStorageRef = Own(0x806);
    public static readonly FormKey BedroomStorageRef = Own(0x807);
    public static readonly FormKey ShipCargoAccess = Own(0x809);
    public static readonly FormKey BasementCargoRef = Own(0x80A);
    public static readonly FormKey SpaceportAccess = Own(0x80B);
    public static readonly FormKey SpaceportStorageRef = Own(0x80C);
    public static readonly FormKey BasementAccess = Own(0x80D);

    // Player-buildable variants, construction recipes and wall attachment rules
    public static readonly FormKey BuildableStorage = Own(0x80E);
    public static readonly FormKey BuildableCargo = Own(0x80F);
    public static readonly FormKey StorageRecipe = Own(0x810);
    public static readonly FormKey CargoRecipe = Own(0x811);
    public static readonly FormKey StorageWallSnap = Own(0x812);
    public static readonly FormKey CargoWallSnap = Own(0x813);

    // Controller and activation override for temporary crafting/building resources
    public static readonly FormKey WorkshopCrafting = Own(0x814);
    public static readonly FormKey WorkbenchActivation = Own(0x815);
    public static readonly FormKey WorkshopAccessPointLink = Own(0x816);
    public static readonly FormKey ShipAccessPointLink = Own(0x817);
}
