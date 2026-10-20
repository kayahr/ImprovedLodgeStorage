using ImprovedLodgeStorage.Storage;
using Mutagen.Bethesda.FormKeys.Starfield;
using Mutagen.Bethesda.Starfield;
using Noggog;

namespace ImprovedLodgeStorage.Locations;

/// <summary>Builds the Lodge storage access point at New Atlantis Spaceport.</summary>
internal sealed class Spaceport
{
    /// <summary>Plugin receiving the Spaceport's records and overrides.</summary>
    private StarfieldMod Mod { get; }

    /// <summary>Game master supplying the container model and original worldspace.</summary>
    private IStarfieldModGetter Game { get; }

    /// <summary>Configures the Spaceport feature's target plugin and source master.</summary>
    /// <param name="mod">Plugin receiving the activator, placed reference and partial parent records.</param>
    /// <param name="game">Game master supplying the container model and original worldspace.</param>
    public Spaceport(StarfieldMod mod, IStarfieldModGetter game)
    {
        Mod = mod;
        Game = game;
    }

    /// <summary>Builds and places the industrial container in New Atlantis's persistent cell.</summary>
    public void Build()
    {
        // LARGE SPACEPORT CONTAINER: reuse the industrial container's appearance
        // as an activator. Its script opens the original Lodge inventory.
        var container = new LodgeStorageContainer(Game, Forms.SpaceportAccess, "ILS_SpaceportStorage");
        container.Build(Mod);

        // Partial worldspace and cell headers contribute only our placed object.
        // The original New Atlantis environment and terrain remain inherited.
        var worldspace = Game.Worldspaces[Starfield.Worldspace.NewAtlantis.FormKey];
        var cell = new Cell(worldspace.TopCell!.FormKey, StarfieldRelease.Starfield)
        {
            MajorFlags = Cell.MajorFlag.PartialForm | Cell.MajorFlag.Persistent
        };
        Mod.Worldspaces.Add(new Worldspace(worldspace.FormKey, StarfieldRelease.Starfield)
        {
            EditorID = worldspace.EditorID,
            // Mutagen's Worldspace enum omits the shared PartialForm header flag.
            MajorRecordFlagsRaw = (int)Cell.MajorFlag.PartialForm,
            TopCell = cell
        });

        // Native plugin coordinates are metres. New Atlantis's console world
        // offset displays this as (-870, 1596, -161.55). Rotation is 26 degrees,
        // converted to the radians used by placed references.
        container.PlacePersistent(cell, Forms.SpaceportStorageRef, "ILS_SpaceportStorageRef",
            position: new P3Float(-170, -104, 77.45f),
            rotation: new P3Float(0, 0, 26 * MathF.PI / 180));
    }
}
