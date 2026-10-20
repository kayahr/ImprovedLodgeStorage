// SPDX-FileCopyrightText: 2026 Klaus Reimer <k@ailis.de>
// SPDX-License-Identifier: MIT

using Mutagen.Bethesda.FormKeys.Starfield;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace ImprovedLodgeStorage.Storage;

/// <summary>Builds a blue CARGO HOLD panel opening the current home ship's inventory.</summary>
internal sealed class ShipCargoPanel : StorageAccess
{
    /// <summary>Configures a cargo access point using the game's CARGO HOLD panel model.</summary>
    /// <param name="game">Game master supplying the original cargo panel.</param>
    /// <param name="key">FormKey of the new base activator.</param>
    /// <param name="editorId">EditorID of the new base activator.</param>
    public ShipCargoPanel(IStarfieldModGetter game, FormKey key, string editorId)
        : base(key, editorId, "Ship Cargo", game.Containers[Starfield.Container.SpaceshipInventory.FormKey])
    {
    }

    /// <summary>Creates the handler that resolves and opens the player's current home ship.</summary>
    /// <returns>A new ship cargo script entry.</returns>
    protected override ScriptEntry BuildScript()
    {
        return new ScriptEntry { Name = "ILS:ShipCargo" };
    }
}
