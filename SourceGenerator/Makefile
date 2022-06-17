#!/usr/bin/env make

.DEFAULT_SHELL ?= /bin/bash
.DEFAULT_GOAL  := help
ROOT_FOLDER    := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))




TARGET_PLATFORM    ?= osx
GODOT_EXECUTABLE   ?= "/Applications/Godot_mono.app/Contents/MacOS/Godot"
GODOT_SHARP_FOLDER ?= /Applications/Godot_mono.app/Contents/Resources/GodotSharp/Api/Debug


.PHONY: help
help:
	@echo "Usage: make [target] [target2] [target...]"
	@echo "Goals:"
	@echo "    clean     remove all temporal and final dlls"
	@echo "    build     build the project"
	@echo "    run       run the generator"
	@echo "    editor    open the Godot editor"


.PHONY: clean
clean:
	rm -rf ${ROOT_FOLDER}/.mono


	mkdir -p ${ROOT_FOLDER}/.mono/assemblies/Debug
	cp -pr ${GODOT_SHARP_FOLDER}/* ${ROOT_FOLDER}/.mono/assemblies/Debug
	
.PHONY: build
build:
	msbuild ${ROOT_FOLDER}/Generator.sln /restore /t:Build "/p:Configuration=Debug" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}

.PHONY: run
run:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "GeneratorScript.cs" --no-window --verbose





.PHONY: editor
editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor



