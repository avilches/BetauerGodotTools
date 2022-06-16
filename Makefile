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
	@echo "    make generate: execute Generator project which creates all *.cs classes for GodotAction"
	@echo "    make build: build all projects"
	@echo "    make test: run tests from all projects"
	@echo "    make editor: open the Godot editor with the Betauer project"

.PHONY: clean
clean:
	rm -rf ${ROOT_FOLDER}/.mono
	find ${ROOT_FOLDER} -regex "${ROOT_FOLDER}/Betauer\.[A-Za-z\.]*/bin" -type d | xargs rm -rf
	find ${ROOT_FOLDER} -regex "${ROOT_FOLDER}/Betauer\.[A-Za-z\.]*/obj" -type d  | xargs rm -rf
	mkdir -p ${ROOT_FOLDER}/.mono/assemblies/${CONFIGURATION}
	cp -pr ${GODOT_SHARP_FOLDER}/* ${ROOT_FOLDER}/.mono/assemblies/${CONFIGURATION}
	
.PHONY: build
build:
	msbuild ${ROOT_FOLDER}/Betauer.sln /restore /t:Build "/p:Configuration=${CONFIGURATION}" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}

.PHONY: generate
generate:
	make -f ${ROOT_FOLDER}/SourceGenerator/Makefile clean build run

.PHONY: test
test:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "TestRunner.cs" --no-window --verbose 

.PHONY: editor
editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor



