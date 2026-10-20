// SPDX-FileCopyrightText: 2026 Klaus Reimer <k@ailis.de>
// SPDX-License-Identifier: MIT

using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Noggog;
using AccessActivator = Mutagen.Bethesda.Starfield.Activator;

namespace ImprovedLodgeStorage.Storage;

/// <summary>Builds an inventory access point and its fixed or player-buildable placements.</summary>
internal abstract class StorageAccess
{
    private readonly FormKey key;
    private readonly string editorId;
    private readonly string name;
    private readonly IObjectBoundsGetter bounds;
    private readonly IModelGetter? model;
    private readonly Percent dirtinessScale;
    private readonly IEnumerable<IAComponentGetter> visualComponents = [];

    /// <summary>Configures an access point using an existing container's appearance.</summary>
    /// <param name="key">FormKey of the new base activator.</param>
    /// <param name="editorId">EditorID of the new base activator.</param>
    /// <param name="name">Name displayed when the player targets the access point.</param>
    /// <param name="template">Original container supplying bounds, model and visual components.</param>
    protected StorageAccess(FormKey key, string editorId, string name, IContainerGetter template)
        : this(key, editorId, name, template.ObjectBounds, template.Model, template.DirtinessScale)
    {
        // Preserve the model's animation graph and effect metadata.
        visualComponents = template.Components.Where(component =>
            component is IAnimationGraphComponentGetter or IEffectSequenceComponentGetter);
    }

    /// <summary>Configures an access point using an existing static object's appearance.</summary>
    /// <param name="key">FormKey of the new base activator.</param>
    /// <param name="editorId">EditorID of the new base activator.</param>
    /// <param name="name">Name displayed when the player targets the access point.</param>
    /// <param name="template">Original static object supplying bounds and model.</param>
    protected StorageAccess(FormKey key, string editorId, string name, IStaticGetter template)
        : this(key, editorId, name, template.ObjectBounds, template.Model, template.DirtinessScale)
    {
    }

    /// <summary>Stores the record identity and visual configuration used during building.</summary>
    /// <param name="key">FormKey of the new base activator.</param>
    /// <param name="editorId">EditorID of the new base activator.</param>
    /// <param name="name">Name displayed when the player targets the access point.</param>
    /// <param name="bounds">Bounds of the source model.</param>
    /// <param name="model">Source model, or null if no model is assigned.</param>
    /// <param name="dirtinessScale">Surface dirtiness copied from the source object.</param>
    private StorageAccess(FormKey key, string editorId, string name,
        IObjectBoundsGetter bounds, IModelGetter? model, Percent dirtinessScale)
    {
        this.key = key;
        this.editorId = editorId;
        this.name = name;
        this.bounds = bounds;
        this.model = model;
        this.dirtinessScale = dirtinessScale;
    }

    /// <summary>Adds the access point's base activator to the plugin.</summary>
    /// <param name="mod">Plugin receiving the new activator.</param>
    /// <returns>The newly added base activator.</returns>
    public AccessActivator Build(StarfieldMod mod)
    {
        var result = new AccessActivator(key, StarfieldRelease.Starfield)
        {
            EditorID = editorId,
            Name = name,
            ObjectBounds = bounds.DeepCopy(),
            DirtinessScale = dirtinessScale,
            Model = model?.DeepCopy(),
            Flags = AccessActivator.Flag.IgnoredBySandbox
        };

        foreach (var component in visualComponents)
        {
            result.Components.Add(component.DeepCopy());
        }

        mod.Activators.Add(result);
        return result;
    }

    /// <summary>Builds a wall-mountable base whose script is inherited by player-built instances.</summary>
    /// <param name="mod">Plugin receiving the new activator.</param>
    /// <param name="snap">Snap template defining attachment to a wall.</param>
    /// <returns>The newly added activator with its inventory handler and wall attachment.</returns>
    public AccessActivator BuildBuildable(StarfieldMod mod, SnapTemplate snap)
    {
        var result = Build(mod);
        result.VirtualMachineAdapter = new VirtualMachineAdapter { Scripts = { BuildScript() } };
        result.SnapTemplate.SetTo(snap.FormKey);
        result.ActivationAngle = 360;
        return result;
    }

    /// <summary>Adds a scripted persistent reference to this access point.</summary>
    /// <param name="cell">Cell receiving the placed reference.</param>
    /// <param name="referenceKey">FormKey of the new placed reference.</param>
    /// <param name="referenceEditorId">EditorID of the new placed reference.</param>
    /// <param name="position">Cell-local position in metres; defaults to the origin.</param>
    /// <param name="rotation">Rotation around each axis in radians; defaults to zero.</param>
    /// <param name="scale">Scale multiplier, or null to use the model's default scale.</param>
    public void PlacePersistent(Cell cell, FormKey referenceKey, string referenceEditorId,
        P3Float position = default, P3Float rotation = default, float? scale = null)
    {
        var reference = new PlacedObject(referenceKey, StarfieldRelease.Starfield)
        {
            EditorID = referenceEditorId,
            MajorRecordFlagsRaw = (int)PlacedObject.DefaultMajorFlag.Persistent,
            VirtualMachineAdapter = new VirtualMachineAdapter { Scripts = { BuildScript() } },
            Position = position,
            Rotation = rotation,
            Scale = scale
        };
        reference.Base.SetTo(key);
        cell.Persistent.Add(reference);
    }

    /// <summary>Creates the Papyrus handler entry for this access point.</summary>
    /// <returns>A new script entry containing the handler name and required properties.</returns>
    protected abstract ScriptEntry BuildScript();
}
