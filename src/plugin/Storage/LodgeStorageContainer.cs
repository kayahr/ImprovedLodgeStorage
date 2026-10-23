// SPDX-FileCopyrightText: 2026 Klaus Reimer <k@ailis.de>
// SPDX-License-Identifier: MIT

using Mutagen.Bethesda.FormKeys.Starfield;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace ImprovedLodgeStorage.Storage;

/// <summary>Builds an industrial container opening the shared Lodge inventory without a door animation.</summary>
internal sealed class LodgeStorageContainer : StorageAccess
{
    /// <summary>Configures a Lodge access point using the large industrial container model.</summary>
    /// <param name="game">Game master supplying the original industrial container.</param>
    /// <param name="key">FormKey of the new base activator.</param>
    /// <param name="editorId">EditorID of the new base activator.</param>
    public LodgeStorageContainer(IStarfieldModGetter game, FormKey key, string editorId)
        : base(key, editorId, "Lodge Storage", game.Statics[Starfield.Static.StorageContainer02Sm.FormKey])
    {
    }

    /// <summary>Creates the Lodge inventory handler with door closing disabled.</summary>
    /// <returns>A new Lodge storage script entry configured for a model without an animated door.</returns>
    protected override ScriptEntry BuildScript()
    {
        var inventory = new ScriptObjectProperty { Name = "LodgeInventory" };
        inventory.Object.SetTo(Forms.BedroomInventory);
        return new ScriptEntry
        {
            Name = "ILS:StorageAccess",
            Properties =
            {
                inventory,
                new ScriptBoolProperty { Name = "CloseDoorOnExit", Data = false }
            }
        };
    }
}
