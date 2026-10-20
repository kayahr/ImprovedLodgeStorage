// SPDX-FileCopyrightText: 2026 Klaus Reimer <k@ailis.de>
// SPDX-License-Identifier: MIT

using Mutagen.Bethesda.FormKeys.Starfield;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace ImprovedLodgeStorage.Storage;

/// <summary>Builds a Captain's Locker opening the shared Lodge inventory.</summary>
internal sealed class LodgeStorageLocker : ClosableStorageAccess
{
    /// <summary>Configures an animated Lodge access point using the Captain's Locker model.</summary>
    /// <param name="game">Game master supplying the original Captain's Locker.</param>
    /// <param name="key">FormKey of the new base activator.</param>
    /// <param name="editorId">EditorID of the new base activator.</param>
    public LodgeStorageLocker(IStarfieldModGetter game, FormKey key, string editorId)
        : base(key, editorId, game.Containers[Starfield.Container.CaptainsLocker01.FormKey])
    {
    }
}
