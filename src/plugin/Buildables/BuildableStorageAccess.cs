// SPDX-FileCopyrightText: 2026 Klaus Reimer <k@ailis.de>
// SPDX-License-Identifier: MIT

using ImprovedLodgeStorage.Storage;
using Mutagen.Bethesda;
using Mutagen.Bethesda.FormKeys.Starfield;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Noggog;
using AccessActivator = Mutagen.Bethesda.Starfield.Activator;

namespace ImprovedLodgeStorage.Buildables;

/// <summary>Builds a storage access point with wall attachment and a free construction recipe.</summary>
internal abstract class BuildableStorageAccess
{
    private readonly StorageAccess access;
    private readonly FormKey snapKey;
    private readonly string snapEditorId;
    private readonly P3Float snapOffset;
    private readonly P3Float snapRotation;
    private readonly FormKey recipeKey;
    private readonly string recipeEditorId;
    private readonly string description;
    private readonly float menuOrder;

    /// <summary>Configures a buildable variant of an existing storage access point.</summary>
    /// <param name="access">Access point supplying its identity, appearance and inventory handler.</param>
    /// <param name="snapKey">FormKey of the new wall attachment template.</param>
    /// <param name="snapEditorId">EditorID of the new wall attachment template.</param>
    /// <param name="snapOffset">Wall attachment offset in metres.</param>
    /// <param name="snapRotation">Snap-node rotation around each axis in degrees.</param>
    /// <param name="recipeKey">FormKey of the new construction recipe.</param>
    /// <param name="recipeEditorId">EditorID of the new construction recipe.</param>
    /// <param name="description">Description shown in the construction menu.</param>
    /// <param name="menuOrder">Sort position within the construction menu.</param>
    protected BuildableStorageAccess(StorageAccess access, FormKey snapKey, string snapEditorId,
        P3Float snapOffset, P3Float snapRotation, FormKey recipeKey, string recipeEditorId,
        string description, float menuOrder)
    {
        this.access = access;
        this.snapKey = snapKey;
        this.snapEditorId = snapEditorId;
        this.snapOffset = snapOffset;
        this.snapRotation = snapRotation;
        this.recipeKey = recipeKey;
        this.recipeEditorId = recipeEditorId;
        this.description = description;
        this.menuOrder = menuOrder;
    }

    /// <summary>Adds the access point, wall attachment and free recipe for homes and outposts.</summary>
    /// <param name="mod">Plugin receiving the activator, snap template and construction recipe.</param>
    /// <returns>The newly added buildable activator.</returns>
    public virtual AccessActivator Build(StarfieldMod mod)
    {
        var snap = BuildWallSnap(mod);
        var result = access.BuildBuildable(mod, snap);
        BuildRecipe(mod, result);
        return result;
    }

    /// <summary>Adds this access point's wall attachment template.</summary>
    /// <param name="mod">Plugin receiving the new snap template.</param>
    /// <returns>The newly added wall attachment template.</returns>
    private SnapTemplate BuildWallSnap(StarfieldMod mod)
    {
        // The game's defaultNode_AttachToWall01 enables normal wall placement.
        var node = new SnapNodeEntry { Offset = snapOffset, Rotation = snapRotation };
        node.Node.SetTo(Starfield.SnapTemplateNode.defaultNode_AttachToWall01);
        var result = new SnapTemplate(snapKey, StarfieldRelease.Starfield)
        {
            EditorID = snapEditorId,
            Nodes = { node },
            NextNodeID = 1,
            STPT = 0
        };
        mod.SnapTemplates.Add(result);
        return result;
    }

    /// <summary>Adds this access point's free construction recipe under Misc.</summary>
    /// <param name="mod">Plugin receiving the new construction recipe.</param>
    /// <param name="createdObject">Activator constructed by the player.</param>
    private void BuildRecipe(StarfieldMod mod, AccessActivator createdObject)
    {
        // A real Credits component with zero required quantity makes construction
        // free. Omitting the component list makes the UI report Missing Resources.
        var cost = new ConstructibleObjectComponent();
        cost.Component.SetTo(Starfield.MiscItem.Credits);
        var recipe = new ConstructibleObject(recipeKey, StarfieldRelease.Starfield)
        {
            EditorID = recipeEditorId,
            Description = description,
            ConstructableComponents = [cost],
            AmountProduced = 1,
            MenuSortOrder = menuOrder,
            LearnMethod = ConstructibleObject.LearnMethodEnum.DefaultOrConditions,
            Flags = 0
        };

        // Interior Misc allows both homes and outposts; the filter places the
        // recipe under Misc. No research or perk unlock conditions are required.
        recipe.WorkbenchKeyword.SetTo(Starfield.Keyword.OutpostCategoryMiscInterior);
        recipe.CreatedObject.SetTo(createdObject.FormKey);
        recipe.RecipeFilters = [Starfield.Keyword.OutpostCategoryMiscellaneous];
        mod.ConstructibleObjects.Add(recipe);
    }
}
