// Unity Asset Store marketing images (Publisher Portal sizes, see listing/LISTING.md).
import { icon, banner, slide } from './brand.mjs';
const opts = { name: 'Whumpkit Lite', tagline: 'Trauma camera shake and hit-stop for Unity. Plain C# core with tests.', chips: ['Camera shake', 'Hit-stop', 'Cinemachine-friendly', 'Free'], glyph: 'W', kicker: 'Free Unity tool' };
const code = `// Camera (or a child of your rig)  →  PN Scripts / Whumpkit Lite / Shake
// Any object                         →  PN Scripts / Whumpkit Lite / Hit Stop

void OnHit(bool heavy)
{
    shake.AddTrauma(heavy ? 0.6f : 0.25f);
    WhumpkitLiteHitStop.Instance.Freeze(heavy ? 0.12f : 0.05f);
}

<span style="color:#5d6762">// shake = trauma ^ power, linear decay, seeded noise
// offset added in LateUpdate, removed next frame: your camera code keeps control
// hit-stop restores the time scale your game had</span>`;
export default [
  { out: 'listing/assetstore/icon-160x160.png', width: 160, height: 160, html: icon(160, 'W') },
  { out: 'listing/assetstore/card-420x280.png', width: 420, height: 280, html: banner(420, 280, { ...opts, tagline: 'Shake and hit-stop.', chips: ['Free'] }) },
  { out: 'listing/assetstore/cover-1200x800.png', width: 1200, height: 800, html: banner(1200, 800, opts) },
  { out: 'listing/assetstore/social-1200x630.png', width: 1200, height: 630, html: banner(1200, 630, opts) },
  { out: 'listing/assetstore/screenshot-code-1920x1080.png', width: 1920, height: 1080, html: slide(1920, 1080, { kicker: 'Two components, two lines', title: 'Camera shake and hit-stop for Unity', body: code }) }
];
