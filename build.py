#!/usr/bin/env python3
"""Assemble the Unity package and pack it into release/.

    python3 build.py

Copies the plain C# core (src/Tresko.Core/*.cs) into Runtime/Core with its own assembly
definition (no engine references), adds README, LICENSE and CHANGELOG, writes a .meta file with a
stable GUID for every file and folder (Unity needs them for read-only tarball packages) and writes:

  release/com.pnscripts.tresko-lite-1.0.0.tgz   Package Manager > + > Add package from tarball
  release/com.pnscripts.tresko-lite-1.0.0.zip   the same folder, to unpack into <project>/Packages/
  build/AssetStore/Assets/PN Scripts/Tresko Lite/  the same files under one root folder, as the
      Asset Store asks (guideline 2.1.a); open build/AssetStore as a Unity project and export it
      with the Asset Store Publishing Tools.
"""
import hashlib
import json
import shutil
import tarfile
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parent
NAME = "com.pnscripts.tresko-lite"
SRC = ROOT / "unity" / NAME
CORE = ROOT / "src" / "TreskoLite.Core"
STAGE = ROOT / "build" / NAME
DIST = ROOT / "release"
VERSION = json.loads((SRC / "package.json").read_text())["version"]

CORE_ASMDEF = {
    "name": "PNScripts.TreskoLite.Core", "rootNamespace": "PNScripts.TreskoLite", "references": [],
    "includePlatforms": [], "excludePlatforms": [], "allowUnsafeCode": False, "overrideReferences": False,
    "precompiledReferences": [], "autoReferenced": True, "defineConstraints": [], "versionDefines": [],
    "noEngineReferences": True,
}
COMMON = "  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n"
IMPORTERS = {
    ".cs": "MonoImporter:\n  externalObjects: {}\n  serializedVersion: 2\n  defaultReferences: []\n  executionOrder: 0\n"
           "  icon: {instanceID: 0}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n",
    ".asmdef": "AssemblyDefinitionImporter:\n" + COMMON,
    ".json": "TextScriptImporter:\n" + COMMON,
    ".md": "TextScriptImporter:\n" + COMMON,
}
FOLDER = "folderAsset: yes\nDefaultImporter:\n" + COMMON


def guid(rel: str) -> str:
    return hashlib.md5(f"{NAME}/{rel}".encode()).hexdigest()


def main():
    shutil.rmtree(STAGE, ignore_errors=True)
    shutil.copytree(SRC, STAGE)
    core = STAGE / "Runtime" / "Core"
    core.mkdir(parents=True)
    for cs in sorted(CORE.glob("*.cs")):
        shutil.copy(cs, core / cs.name)
    (core / "PNScripts.TreskoLite.Core.asmdef").write_text(json.dumps(CORE_ASMDEF, indent=4) + "\n")
    shutil.copy(ROOT / "README.md", STAGE / "README.md")
    shutil.copy(ROOT / "LICENSE", STAGE / "LICENSE.md")
    shutil.copy(ROOT / "CHANGELOG.md", STAGE / "CHANGELOG.md")
    seen = set()
    for p in sorted(STAGE.rglob("*")):
        rel = p.relative_to(STAGE).as_posix()
        if p.suffix == ".meta" or "Samples~" in Path(rel).parts:
            continue  # Unity ignores folders ending in ~; their files get metas when a sample is imported
        g = guid(rel)
        assert g not in seen
        seen.add(g)
        body = FOLDER if p.is_dir() else IMPORTERS.get(p.suffix, "DefaultImporter:\n" + COMMON)
        Path(str(p) + ".meta").write_text(f"fileFormatVersion: 2\nguid: {g}\n{body}")
    DIST.mkdir(exist_ok=True)
    tgz = DIST / f"{NAME}-{VERSION}.tgz"
    with tarfile.open(tgz, "w:gz") as tar:
        for p in sorted(STAGE.rglob("*")):
            tar.add(p, arcname="package/" + p.relative_to(STAGE).as_posix(), recursive=False)
    zp = DIST / f"{NAME}-{VERSION}.zip"
    with zipfile.ZipFile(zp, "w", zipfile.ZIP_DEFLATED) as z:
        for p in sorted(STAGE.rglob("*")):
            z.write(p, f"{NAME}/" + p.relative_to(STAGE).as_posix())
    # Asset Store layout: one root folder under Assets/, samples included as a normal folder.
    store = ROOT / "build" / "AssetStore"
    shutil.rmtree(store, ignore_errors=True)
    root = store / "Assets" / "PN Scripts" / "Tresko Lite"
    shutil.copytree(STAGE, root, ignore=shutil.ignore_patterns("*.meta", "package.json", "Samples~"))
    shutil.copytree(STAGE / "Samples~" / "ShakeDemo", root / "Samples" / "ShakeDemo")
    (store / "ProjectSettings").mkdir(parents=True)
    (store / "ProjectSettings" / "ProjectVersion.txt").write_text("m_EditorVersion: 2022.3.62f1\n")
    print("built", tgz.name, zp.name, f"({len(seen)} meta files)", "and build/AssetStore")


if __name__ == "__main__":
    main()
