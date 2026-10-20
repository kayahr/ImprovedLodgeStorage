// SPDX-FileCopyrightText: 2026 Klaus Reimer <k@ailis.de>
// SPDX-License-Identifier: MIT

using ImprovedLodgeStorage.Storage;
using Mutagen.Bethesda.Starfield;
using Noggog;

namespace ImprovedLodgeStorage.Buildables;

/// <summary>Builds the free wall-mounted ship cargo panel for homes and outposts.</summary>
internal sealed class BuildableShipCargoPanel : BuildableStorageAccess
{
    /// <summary>Configures the buildable CARGO HOLD panel and its construction menu entry.</summary>
    /// <param name="game">Game master supplying the original cargo panel model.</param>
    public BuildableShipCargoPanel(IStarfieldModGetter game)
        : base(
            // FormID 80F selects the preview texture 0000080fcl.dds.
            access: new ShipCargoPanel(game, Forms.BuildableCargo, "ILS_OutpostShipCargo"),
            snapKey: Forms.CargoWallSnap,
            snapEditorId: "ILS_OutpostShipCargoWallSnap",
            // The rear Y bound plus 5 mm clears the wall without extra rotation.
            snapOffset: new P3Float(0, 0.09429444f, 0),
            snapRotation: default,
            recipeKey: Forms.CargoRecipe,
            recipeEditorId: "ILS_co_OutpostShipCargo",
            description: "Access your current home ship's cargo hold.",
            menuOrder: 101)
    {
    }
}
