# Tresko Lite by PN Scripts

Free game feel for Unity (MIT): **trauma camera shake** that leaves your
camera controller in charge, and **hit-stop** that gives your time scale
back. The maths lives in a plain C# core with unit tests; two small
components put it on your GameObjects.

## Quick start

1. Install: Unity Asset Store ("Tresko Lite"), or Package Manager > **+** >
   **Add package from tarball...** with `com.pnscripts.tresko-lite-1.0.0.tgz`.
2. Add **PN Scripts > Tresko Lite > Shake** to your camera (or a child of
   your camera rig) and **PN Scripts > Tresko Lite > Hit Stop** to any object.
3. On a hit:

   ```csharp
   shake.AddTrauma(heavy ? 0.6f : 0.25f);
   TreskoLiteHitStop.Instance.Freeze(heavy ? 0.12f : 0.05f);
   ```

The **Shake Demo** sample builds a camera and a cube at runtime and hits on a
timer: add `TreskoLiteShakeDemo` to an empty GameObject in an empty scene and
press Play.

## Components

- **TreskoLiteShake**: trauma 0..1 added by hits, linear decay, strength =
  trauma ^ power, offsets and rotation from seeded noise. The offset is added
  in LateUpdate and removed the next frame, so your camera controller or
  Cinemachine keeps the real position. Unscaled time by default, so it shakes
  during hit-stop; `intensity` for an accessibility setting.
- **TreskoLiteHitStop**: `Freeze(seconds, scale)` on real time; restores the
  time scale the game had (your slow motion survives); overlapping freezes
  extend and take the lowest scale; a chain is capped (`maxTotal`) and
  followed by a short cooldown.
- Inspector buttons try both in Play mode.

The full edition, Tresko, adds squash and stretch, hit flash, knockback,
punch, easing and one `Impact()` call:
https://pnscripts.com/marketplace/unity-tresko. Lite is complete on its own.

## Build and test

```
make test            # xunit tests of the plain C# core (dotnet 8)
make unity-compile   # compiles the package against an installed Unity editor's DLLs
make build           # release/*.tgz and *.zip, and build/AssetStore for the store upload
```

## Licence

MIT. Copyright (c) 2026 ПН СКРИПТС ЕООД (PN Scripts). On the Unity Asset Store
the package is also offered under the Asset Store EULA.
