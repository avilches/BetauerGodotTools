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
	@echo "    make clean: remove all temporal and final dlls."

clean:
	cd ${ROOT_FOLDER}
	rm -rf .mono
	find . -regex "./Betauer/Betauer\.[A-Za-z\.]*/bin" | xargs rm -rf
	find . -regex "./Betauer/Betauer\.[A-Za-z\.]*/obj" | xargs rm -rf
	mkdir -p .mono/assemblies/${CONFIGURATION}
	cp -pr ${GODOT_SHARP_FOLDER}/* .mono/assemblies/${CONFIGURATION}
	
build:
	cd ${ROOT_FOLDER}
	msbuild /restore /t:Build "/p:Configuration=${CONFIGURATION}" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}

test:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "TestRunner.cs" --no-window --verbose

run:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}"

editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor



