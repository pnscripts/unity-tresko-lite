# Tresko Lite: Unity Asset Store (free asset)

Nothing has been submitted. Rules: https://assetstore.unity.com/publishing/submission-guidelines (last updated 2026-05-20).

Asset Store fit (checked 2026-10-12):
- Lite versions are allowed when every included feature works identically to the full product (1.4.c): the Lite components use the same core code as Tresko.
- One root folder (2.1.a): `build/AssetStore/Assets/PN Scripts/Tresko Lite/`. Documentation included (README.md, 2.3.a).
- AI disclosure is mandatory when any functional part involved AI (1.6.a/b): answer **yes** in the "AI description" field and name the tools (code written with Claude by Anthropic, reviewed and tested by PN Scripts). No "hand-made" wording (1.6.c).
- Publisher account: active e-mail and an actively maintained website (4.1.a): office@pnscripts.com, https://pnscripts.com.

Owner steps:
1. Sign in to Unity Hub (the editor 6000.5.4f1 is installed but needs a sign-in) and create the publisher profile at https://publisher.unity.com (name "PN Scripts", website pnscripts.com). Free assets need no payout or tax setup.
2. `make build`, then open `build/AssetStore` as a project in Unity 2022.3+ (or 6000.x), install "Asset Store Publishing Tools" from the Package Manager, and upload `Assets/PN Scripts/Tresko Lite` as the package for a new draft "Tresko Lite: Camera Shake and Hit-Stop", price Free, category Tools/Camera (or Tools/Game Toolkits).
3. Marketing images (`listing/assetstore/`): icon 160×160, card 420×280, cover 1200×800 (sizes from Unity support article 210122403), social 1200×630 (size UNVERIFIED: check the Publisher Portal template), screenshot 1920×1080.
4. Description: README "Components" section. Keywords: camera shake, screen shake, hit stop, freeze frame, game feel, juice.

Note: the Asset Store distributes under its Standard EULA; the GitHub copy stays MIT.
Checks run here: `make test` (9 xunit tests passed), `make unity-compile` against Unity 6000.5.4f1 DLLs (0 errors). Not run inside the Unity editor.
