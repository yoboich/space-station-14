using System.Linq;
using Content.Server.Salvage.Expeditions;
using Content.Server.Spawners.Components;
using Content.Server.Station.Systems;
using Content.Server.Storage.Components;
using Content.Shared.Cargo.Components;
using Content.Shared.Radio.Components;
using Content.Shared.Roles;
using Content.Shared.Wall;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Backmen.Fugitive;

public sealed class FugitiveSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly StationSpawningSystem _stationSpawning = default!;


    [ValidatePrototypeId<JobPrototype>]
    private const string JobSAI = "SAI";

        public void HandlePlayerSpawning(PlayerSpawningEvent args)
    {
        if (args.SpawnResult != null)
            return;

        if (!(args.Job?.Prototype != null &&
              _prototypeManager.TryIndex<JobPrototype>(args.Job!.Prototype!, out var jobInfo) &&
              jobInfo.AlwaysUseSpawner))
        {
            return;
        }

        var possiblePositions = new List<EntityCoordinates>();
        {
            var points = EntityQueryEnumerator<SpawnPointComponent, TransformComponent>();


            while (points.MoveNext(out var uid, out var spawnPoint, out var xform))
            {
                if (args.Station != null && _stationSystem.GetOwningStation(uid, xform) != args.Station)
                    continue;
                if(xform.GridUid == null)
                    continue;
                if(HasComp<CargoShuttleComponent>(xform.GridUid) || HasComp<SalvageShuttleComponent>(xform.GridUid))
                    continue;
                if (spawnPoint.SpawnType == SpawnPointType.Job &&
                    (args.Job == null || spawnPoint.Job?.ID == args.Job.Prototype))
                {
                    possiblePositions.Add(xform.Coordinates);
                }
            }
        }

        // auto points

        #region SAI

        if (possiblePositions.Count == 0 && args.Job?.Prototype == JobSAI)
        {
            var points = EntityQueryEnumerator<TelecomServerComponent, TransformComponent, MetaDataComponent>();

            while (points.MoveNext(out var uid, out _, out var xform, out var spawnPoint))
            {
                if (args.Station != null && _stationSystem.GetOwningStation(uid, xform) != args.Station)
                    continue;
                if(xform.GridUid == null)
                    continue;
                if(HasComp<CargoShuttleComponent>(xform.GridUid) || HasComp<SalvageShuttleComponent>(xform.GridUid))
                    continue;

                possiblePositions.Add(
                    xform.Coordinates.WithPosition(xform.LocalPosition + xform.LocalRotation.ToWorldVec() * 1f));
            }
        }

        #endregion

        if (possiblePositions.Count == 0)
        {
            Log.Warning("No spawn points were available!");

            var ent = EntityQuery<SpawnPointComponent>().Where(x => x.SpawnType == SpawnPointType.LateJoin).Select(x=>Transform(x.Owner)).First();
            possiblePositions.Add(ent.Coordinates);
            return;
        }

        var spawnLoc = _random.Pick(possiblePositions);

        args.SpawnResult = _stationSpawning.SpawnPlayerMob(
            spawnLoc,
            args.Job,
            args.HumanoidCharacterProfile,
            args.Station);
    }
}
