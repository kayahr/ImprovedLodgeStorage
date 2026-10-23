// SPDX-FileCopyrightText: 2026 Klaus Reimer <k@ailis.de>
// SPDX-License-Identifier: MIT

using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace ImprovedLodgeStorage.Storage;

/// <summary>Builds Lodge access points whose doors close when the inventory menu closes.</summary>
internal abstract class ClosableStorageAccess : StorageAccess
{
    /// <summary>Configures an animated access point for the shared Lodge inventory.</summary>
    /// <param name="key">FormKey of the new base activator.</param>
    /// <param name="editorId">EditorID of the new base activator.</param>
    /// <param name="template">Original container supplying the model and animation metadata.</param>
    protected ClosableStorageAccess(FormKey key, string editorId, IContainerGetter template)
        : base(key, editorId, "Lodge Storage", template)
    {
    }

    /// <summary>Creates the Lodge inventory handler with its default door-closing behaviour.</summary>
    /// <returns>A new Lodge storage script entry with automatic door closing enabled by default.</returns>
    protected override ScriptEntry BuildScript()
    {
        // The handler closes animated access points by default.
        var script = new ScriptEntry { Name = "ILS:StorageAccess" };
        var inventory = new ScriptObjectProperty { Name = "LodgeInventory" };
        inventory.Object.SetTo(Forms.BedroomInventory);
        script.Properties.Add(inventory);
        return script;
    }
}
