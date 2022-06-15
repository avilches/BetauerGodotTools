#!/usr/bin/env make

.DEFAULT_SHELL ?= /bin/bash
.DEFAULT_GOAL  := help

CONFIGURATION      ?= Debug
TARGET_PLATFORM    ?= osx
GODOT_EXECUTABLE   ?= "/Applications/Godot_mono.app/Contents/MacOS/Godot"
GODOT_SHARP_FOLDER ?= /Applications/Godot_mono.app/Contents/Resources/GodotSharp/Api/${CONFIGURATION}


.PHONY:
help:
	@echo "Usage:"
	@echo "    make clean: remove all temporal and final dlls."

clean:
	rm -rf .mono dlls
	mkdir -p .mono/assemblies/${CONFIGURATION}
	cp -pr ${GODOT_SHARP_FOLDER}/* .mono/assemblies/${CONFIGURATION}
	
build:
	msbuild /restore /t:Build "/p:Configuration=${CONFIGURATION}" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}

test:
	"${GODOT_EXECUTABLE}" -s "TestRunner.cs" --no-window --verbose

editor:
	"${GODOT_EXECUTABLE}" -path "$(CURDIR)" --editor



