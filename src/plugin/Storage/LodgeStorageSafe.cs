// SPDX-FileCopyrightText: 2026 Klaus Reimer <k@ailis.de>
// SPDX-License-Identifier: MIT

using Mutagen.Bethesda.FormKeys.Starfield;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace ImprovedLodgeStorage.Storage;

/// <summary>Builds the visible bedroom safe opening the shared Lodge inventory.</summary>
internal sealed class LodgeStorageSafe : ClosableStorageAccess
{
    /// <summary>Configures an animated Lodge access point using the original bedroom safe model.</summary>
    /// <param name="game">Game master supplying the original bedroom safe.</param>
    /// <param name="key">FormKey of the new base activator.</param>
    /// <param name="editorId">EditorID of the new base activator.</param>
    public LodgeStorageSafe(IStarfieldModGetter game, FormKey key, string editorId)
        : base(key, editorId, game.Containers[Starfield.Container.Storage_Safe_Tall_Player.FormKey])
    {
    }
}
