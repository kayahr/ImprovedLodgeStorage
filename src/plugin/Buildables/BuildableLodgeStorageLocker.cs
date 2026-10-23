// SPDX-FileCopyrightText: 2026 Klaus Reimer <k@ailis.de>
// SPDX-License-Identifier: MIT

using ImprovedLodgeStorage.Storage;
using Mutagen.Bethesda.Starfield;
using Noggog;
using AccessActivator = Mutagen.Bethesda.Starfield.Activator;

namespace ImprovedLodgeStorage.Buildables;

/// <summary>Builds the free wall-mounted Lodge storage locker for homes and outposts.</summary>
internal sealed class BuildableLodgeStorageLocker : BuildableStorageAccess
{
    /// <summary>Configures the buildable Captain's Locker and its construction menu entry.</summary>
    /// <param name="game">Game master supplying the original Captain's Locker model.</param>
    public BuildableLodgeStorageLocker(IStarfieldModGetter game)
        : base(
            // FormID 80E selects the preview texture 0000080ecl.dds.
            access: new LodgeStorageLocker(game, Forms.BuildableStorage, "ILS_OutpostLodgeStorage"),
            snapKey: Forms.StorageWallSnap,
            snapEditorId: "ILS_OutpostLodgeStorageWallSnap",
            // The rear X bound minus 5 mm provides wall clearance. Snap rotation
            // uses degrees and turns the locker door away from the wall.
            snapOffset: new P3Float(-0.06179298f, 0, 0),
            snapRotation: new P3Float(0, 0, 90),
            recipeKey: Forms.StorageRecipe,
            recipeEditorId: "ILS_co_OutpostLodgeStorage",
            description: "Access your unlimited Lodge storage.",
            menuOrder: 100)
    {
    }

    /// <summary>Builds the Lodge locker and connects its script to the workshop controller.</summary>
    /// <param name="mod">Plugin receiving the activator, wall attachment and free recipe.</param>
    /// <returns>The buildable locker with local crafting and building support.</returns>
    public override AccessActivator Build(StarfieldMod mod)
    {
        var result = base.Build(mod);
        var controller = new ScriptObjectProperty { Name = "Controller" };
        controller.Object.SetTo(Forms.WorkshopCrafting);
        result.VirtualMachineAdapter!.Scripts[0].Properties.Add(controller);
        return result;
    }
}
