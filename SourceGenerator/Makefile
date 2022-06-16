#!/usr/bin/env make

.DEFAULT_SHELL ?= /bin/bash
.DEFAULT_GOAL  := help
ROOT_FOLDER    := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))




CONFIGURATION      ?= Debug
TARGET_PLATFORM    ?= osx
GODOT_EXECUTABLE   ?= "/Applications/Godot_mono.app/Contents/MacOS/Godot"
GODOT_SHARP_FOLDER ?= /Applications/Godot_mono.app/Contents/Resources/GodotSharp/Api/${CONFIGURATION}


.PHONY: help
help:
	@echo "Usage:"
	@echo "    make clean: remove all temporal and final dlls"
	@echo "    make build: build the project"
	@echo "    make run: run the generator"
	@echo "    make editor: open the Godot editor with the Generator project"


.PHONY: clean
clean:
	rm -rf ${ROOT_FOLDER}/.mono


	mkdir -p ${ROOT_FOLDER}/.mono/assemblies/${CONFIGURATION}
	cp -pr ${GODOT_SHARP_FOLDER}/* ${ROOT_FOLDER}/.mono/assemblies/${CONFIGURATION}
	
.PHONY: build
build:
	msbuild ${ROOT_FOLDER}/Generator.sln /restore /t:Build "/p:Configuration=${CONFIGURATION}" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}

.PHONY: run
run:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "GeneratorScript.cs" --no-window --verbose





.PHONY: editor
editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor



