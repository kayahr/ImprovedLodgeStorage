// SPDX-FileCopyrightText: 2026 Klaus Reimer <k@ailis.de>
// SPDX-License-Identifier: MIT

using ImprovedLodgeStorage.Buildables;
using ImprovedLodgeStorage.Locations;
using ImprovedLodgeStorage.Workshops;
using Mutagen.Bethesda.FormKeys.Starfield;
using Mutagen.Bethesda.Starfield;

namespace ImprovedLodgeStorage;

/// <summary>Builds the Improved Lodge Storage plugin from the installed game master.</summary>
internal static class Program
{
    /// <summary>Reads the game master, builds the mod's features and writes the ESM.</summary>
    /// <param name="args">Two arguments: the game data directory and the output ESM path.</param>
    /// <returns>A task yielding 0 on success or 1 for an invalid argument count.</returns>
    private static async Task<int> Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine("Usage: ImprovedLodgeStorage.Plugin <DATA> <OUTPUT>");
            return 1;
        }

        // Read master
        using var game = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(Path.Combine(args[0], Starfield.ModKey.FileName))
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(args[0])
            .Construct();

        // Initialize the mod
        var mod = new StarfieldMod(Forms.Plugin, StarfieldRelease.Starfield);
        mod.ModHeader.Flags = StarfieldModHeader.HeaderFlag.Master | StarfieldModHeader.HeaderFlag.Light;
        mod.ModHeader.Author = "kayahr";
        mod.ModHeader.Description = "Access your unlimited Lodge storage from the bedroom, basement, "
            + "New Atlantis spaceport, and free buildable lockers in homes and outposts. Craft and research "
            + "directly from stored resources at the Lodge's basement stations, and manage your current home "
            + "ship's cargo through dedicated access panels.";
        // Mutagen 0.54 does not calculate Starfield's interior-cell count.
        mod.ModHeader.InteriorCellCount = 1;

        // Build features
        new Lodge(mod, game).Build();
        new Spaceport(mod, game).Build();
        new BuildableLodgeStorageLocker(game).Build(mod);
        new BuildableShipCargoPanel(game).Build(mod);
        new WorkshopCrafting().Build(mod);

        // Write ESM file
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(args[1]))!);
        await mod.BeginWrite.ToPath(args[1])
            .WithLoadOrder(game)
            .WriteAsync();
        return 0;
    }
}
