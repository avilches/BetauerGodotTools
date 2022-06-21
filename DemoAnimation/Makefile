#!/usr/bin/env make

.DEFAULT_SHELL ?= /bin/bash
.DEFAULT_GOAL  := help
ROOT_FOLDER    := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))

TARGET_PLATFORM    ?= osx
GODOT_EXECUTABLE   ?= "/Applications/Godot_mono.app/Contents/MacOS/Godot"
GODOT_SHARP_FOLDER ?= /Applications/Godot_mono.app/Contents/Resources/GodotSharp/Api

.PHONY: help
help:
	@echo "Usage: make [target] [target2] [target...]"
	@echo "Goals:"
	@echo "    clean          remove all .mono folders"
	@echo "    run            run the game"
	@echo "    editor         open the Godot editor"
	@echo "    build/debug    build project"
	@echo ""

.PHONY: clean
clean:
	rm -rf "${ROOT_FOLDER}/.mono"


	mkdir -p "${ROOT_FOLDER}/.mono/assemblies/Debug"
#	mkdir -p "${ROOT_FOLDER}/.mono/assemblies/Release"
	cp -pr "${GODOT_SHARP_FOLDER}/Debug/"* "${ROOT_FOLDER}/.mono/assemblies/Debug"
#	cp -pr "${GODOT_SHARP_FOLDER}/Release/"* "${ROOT_FOLDER}/.mono/assemblies/Release"
	
.PHONY: build/debug
build:
	#msbuild "${ROOT_FOLDER}/DemoAnimation.sln" /restore /t:Build "/p:Configuration=Debug" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}
	dotnet build "${ROOT_FOLDER}/DemoAnimation.sln" --configuration Debug 

.PHONY: run
run:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}"

.PHONY: editor
editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor