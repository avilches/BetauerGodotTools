#!/usr/bin/env make

.DEFAULT_SHELL ?= /bin/bash
.DEFAULT_GOAL  := help
ROOT_FOLDER    := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))




TARGET_PLATFORM    ?= osx
GODOT_EXECUTABLE   ?= "/Applications/Godot-4.0.0-beta.11.app/Contents/MacOS/Godot"


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
	rm -rf "${ROOT_FOLDER}/.mono"
	rm -rf "${ROOT_FOLDER}/.godot/mono"
	
.PHONY: build
build:
	#msbuild "${ROOT_FOLDER}/Generator.sln" /restore /t:Build "/p:Configuration=Debug" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}
	dotnet build "${ROOT_FOLDER}/Generator4.sln" --configuration Debug 


.PHONY: run
run:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "GeneratorScript.cs" --headless --verbose





.PHONY: editor
editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor



