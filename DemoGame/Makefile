#!/usr/bin/env make

.DEFAULT_SHELL ?= /bin/bash
.DEFAULT_GOAL  := help
ROOT_FOLDER    := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))

CONFIGURATION      ?= Debug
TARGET_PLATFORM    ?= osx
GODOT_EXECUTABLE   ?= "/Applications/Godot_mono.app/Contents/MacOS/Godot"
GODOT_SHARP_FOLDER ?= /Applications/Godot_mono.app/Contents/Resources/GodotSharp/Api/${CONFIGURATION}


.PHONY:
help:
	@echo "Usage:"
	@echo "    make clean: remove all temporal and final dlls"
	@echo "    make build: build all projects"
	@echo "    make test: run tests from all projects"
	@echo "    make run: run the game"
	@echo "    make editor: open the Godot editor with the Betauer project"

clean:
	rm -rf ${ROOT_FOLDER}/.mono




	mkdir -p ${ROOT_FOLDER}/.mono/assemblies/${CONFIGURATION}
	cp -pr ${GODOT_SHARP_FOLDER}/* ${ROOT_FOLDER}/.mono/assemblies/${CONFIGURATION}
	
build:
	msbuild ${ROOT_FOLDER}/Veronenger.sln /restore /t:Build "/p:Configuration=${CONFIGURATION}" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}

test:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "Tests/TestRunner.cs" --no-window --verbose

run:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}"

editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor


