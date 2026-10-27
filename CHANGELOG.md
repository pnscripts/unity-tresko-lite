# Changelog

## 2.0.0 (2026-10-27)

- Renamed from Tresko Lite (trademark clash); no behaviour change. The product is now Whumpkit Lite for Unity.
- Migration (public names changed): package id `com.pnscripts.tresko-lite` -> `com.pnscripts.whumpkit-lite`
  (remove the old package, add the new one); namespace `PNScripts.TreskoLite` -> `PNScripts.WhumpkitLite`;
  components `TreskoLiteShake` and `TreskoLiteHitStop` -> `WhumpkitLiteShake` and `WhumpkitLiteHitStop`.
  Components already on scene objects lose their script link; re-add them (field names and values are unchanged).
  The repository moved to https://github.com/pnscripts/unity-whumpkit-lite (the old address redirects).

## 1.0.0 (2026-10-12)

- First release of the Lite edition: TreskoLiteShake, TreskoLiteHitStop, inspector buttons, Shake Demo sample, plain C# core with tests.
