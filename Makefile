DOTNET ?= dotnet
PY ?= python3
UNITY_MANAGED ?= $(HOME)/Unity/Hub/Editor/6000.5.4f1/Editor/Data/Managed
.PHONY: test unity-compile build images
test:
	$(DOTNET) test tests/WhumpkitLite.Tests -m:1
unity-compile:
	$(DOTNET) build tools/UnityCompileCheck -m:1 -p:UnityManaged=$(UNITY_MANAGED)
build:
	$(PY) build.py
images:
	node tools/render.mjs tools/images.mjs
