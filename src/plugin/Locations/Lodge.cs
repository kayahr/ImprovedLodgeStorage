using ImprovedLodgeStorage.Storage;
using Mutagen.Bethesda;
using Mutagen.Bethesda.FormKeys.Starfield;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Noggog;

namespace ImprovedLodgeStorage.Locations;

/// <summary>Builds the Lodge's storage access points and crafting links.</summary>
internal sealed class Lodge
{
    /// <summary>Plugin receiving the Lodge's records and overrides.</summary>
    private StarfieldMod Mod { get; }

    /// <summary>Game master supplying the original models and placed references.</summary>
    private IStarfieldModGetter Game { get; }

    /// <summary>Original Lodge cell supplying the safe and workstation references.</summary>
    private ICellGetter OriginalCell { get; }

    /// <summary>Partial Lodge cell receiving the new and overridden references.</summary>
    private Cell Cell { get; }

    /// <summary>Configures the Lodge feature and finds its original cell.</summary>
    /// <param name="mod">Plugin receiving the new records and overrides.</param>
    /// <param name="game">Game master supplying the original models and placed references.</param>
    public Lodge(StarfieldMod mod, IStarfieldModGetter game)
    {
        Mod = mod;
        Game = game;
        OriginalCell = game.Cells.Records
            .SelectMany(block => block.SubBlocks)
            .SelectMany(block => block.Cells)
            .Single(cell => cell.FormKey == Starfield.Cell.CityNewAtlantisLodgeInt.FormKey);

        // A partial cell contributes our references while inheriting the room's
        // lighting, water and navigation data from the game. Block 9 / sub-block 2
        // are the Lodge's native record grouping, not room coordinates.
        Cell = new Cell(Starfield.Cell.CityNewAtlantisLodgeInt.FormKey, StarfieldRelease.Starfield)
        {
            EditorID = "CityNewAtlantisLodgeInt",
            MajorFlags = Cell.MajorFlag.PartialForm
        };
    }

    /// <summary>Adds the Lodge access points, crafting links and original safe model override.</summary>
    public void Build()
    {
        Mod.Cells.Add(new CellBlock
        {
            BlockNumber = 9,
            GroupType = GroupTypeEnum.InteriorCellBlock,
            SubBlocks =
            {
                new CellSubBlock
                {
                    BlockNumber = 2,
                    GroupType = GroupTypeEnum.InteriorCellSubBlock,
                    Cells = { Cell }
                }
            }
        });

        AddAccessPoints();
        LinkCraftingStations();
        HideOriginalSafe();
    }

    /// <summary>Builds and places the basement locker, bedroom safe and ship cargo panel.</summary>
    private void AddAccessPoints()
    {
        // BASEMENT LODGE STORAGE: a Captain's Locker opening the bedroom safe.
        var locker = new LodgeStorageLocker(Game, Forms.BasementAccess, "ILS_BasementStorage");
        locker.Build(Mod);
        locker.PlacePersistent(Cell, Forms.BasementStorageRef, "ILS_BasementStorageRef",
            position: new P3Float(23, -64, -2.85f),
            rotation: new P3Float(0, 0, MathF.PI), scale: 1);

        // BEDROOM LODGE STORAGE: a visible safe over the original hidden inventory.
        // Copy its exact native transform instead of maintaining duplicate coordinates.
        var safe = new LodgeStorageSafe(Game, Forms.BedroomAccess, "ILS_BedroomStorage");
        safe.Build(Mod);
        var original = PersistentOverride(Forms.BedroomInventory);
        safe.PlacePersistent(Cell, Forms.BedroomStorageRef, "ILS_BedroomStorageRef",
            original.Position, original.Rotation, original.Scale);

        // BASEMENT SHIP CARGO: the blue CARGO HOLD panel, facing 90 degrees around Z.
        var cargo = new ShipCargoPanel(Game, Forms.ShipCargoAccess, "ILS_ShipCargoAccess");
        cargo.Build(Mod);
        cargo.PlacePersistent(Cell, Forms.BasementCargoRef, "ILS_BasementShipCargoRef",
            position: new P3Float(23, -63.36f, -2.8f),
            rotation: new P3Float(0, 0, MathF.PI / 2), scale: 1);
    }

    /// <summary>Links all six basement crafting and research stations to the bedroom safe.</summary>
    private void LinkCraftingStations()
    {
        // Give the six existing basement stations the bedroom safe as their
        // crafting container. Keep every other linked-reference channel intact.
        foreach (var key in Forms.LodgeWorkbenches)
        {
            var bench = PersistentOverride(key);
            bench.LinkedReferences.RemoveAll(link => link.KeywordOrReference.FormKey == Starfield.Keyword.LinkedWorkbenchContainerKeyword.FormKey);
            var link = new LinkedReferences();
            link.KeywordOrReference.SetTo(Starfield.Keyword.LinkedWorkbenchContainerKeyword);
            link.Reference.SetTo(Forms.BedroomInventory);
            bench.LinkedReferences.Add(link);
        }
    }

    /// <summary>Copies an original Lodge reference into the persistent override group.</summary>
    /// <param name="key">FormKey of the original placed object.</param>
    /// <returns>The newly added persistent placed-object override.</returns>
    private IPlacedObject PersistentOverride(FormKey key)
    {
        // All seven originals belong to this cell. Remote inventory access needs
        // persistent references even when the master stores them as temporary.
        var record = OriginalCell.Persistent.Concat(OriginalCell.Temporary)
            .OfType<IPlacedObjectGetter>()
            .Single(record => record.FormKey == key)
            .DeepCopy();
        record.MajorRecordFlagsRaw |= (int)PlacedObject.DefaultMajorFlag.Persistent;
        Cell.Persistent.Add(record);
        return record;
    }

    /// <summary>Replaces the original safe's visible model with an invisible marker.</summary>
    private void HideOriginalSafe()
    {
        // Preserve the unique vanilla container and its inventory, replacing only
        // its presentation with an invisible marker. The visible access point
        // prevents the original safe's expensive hover-inventory interaction.
        // Exclude the replaced localized name from the copy to avoid loading strings.
        var safe = Mod.Containers.GetOrAddAsOverride(Game.Containers[Starfield.Container.Storage_Safe_Tall_Player.FormKey]
            .DeepCopy(new Container.TranslationMask(defaultOn: true) { Name = false }));
        var marker = Game.Containers[Starfield.Container.SQ_Doctors_TestDoctor_Vendor_Container.FormKey];
        safe.Name = "Lodge Storage";
        safe.Components.Clear();
        safe.ObjectBounds = marker.ObjectBounds.DeepCopy();
        safe.Model = marker.Model?.DeepCopy();
    }
}
