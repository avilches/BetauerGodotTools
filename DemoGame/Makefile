#!/usr/bin/env make

.DEFAULT_SHELL ?= /bin/bash
.DEFAULT_GOAL  := help
ROOT_FOLDER    := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))
EXPORT_OSX     := ${ROOT_FOLDER}/export/osx
EXPORT_WIN     := ${ROOT_FOLDER}/export/win
EXPORT_LINUX   := ${ROOT_FOLDER}/export/linux

CONFIGURATION      ?= Debug
TARGET_PLATFORM    ?= osx
GODOT_EXECUTABLE   ?= "/Applications/Godot_mono.app/Contents/MacOS/Godot"
GODOT_SHARP_FOLDER ?= /Applications/Godot_mono.app/Contents/Resources/GodotSharp/Api/${CONFIGURATION}


.PHONY: help
help:
	@echo "Usage:"
	@echo "    make clean: remove all temporal and final dlls"
	@echo "    make build: build all projects"
	@echo "    make test: run tests from all projects"
	@echo "    make run: run the game"
	@echo "    make editor: open the Godot editor with the Betauer project"

.PHONY: clean
clean:
	rm -rf ${ROOT_FOLDER}/.mono


	mkdir -p ${ROOT_FOLDER}/.mono/assemblies/${CONFIGURATION}
	cp -pr ${GODOT_SHARP_FOLDER}/* ${ROOT_FOLDER}/.mono/assemblies/${CONFIGURATION}
	
.PHONY: build
build:
	msbuild ${ROOT_FOLDER}/Veronenger.sln /restore /t:Build "/p:Configuration=${CONFIGURATION}" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}

.PHONY: run
run:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}"

.PHONY: test
test:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "Tests/TestRunner.cs" --no-window --verbose

.PHONY: editor
editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor

.PHONY: export/osx
export/osx: clean
	rm -rf ${EXPORT_OSX} 
	mkdir -p ${EXPORT_OSX} 
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --no-window -v --export "Mac OSX" "${EXPORT_OSX}/export.osx.zip" 2>&1 | tee "${EXPORT_OSX}/../export.osx.log"

.PHONY: export/win
export/win: clean
	rm -rf ${EXPORT_WIN} 
	mkdir -p ${EXPORT_WIN} 
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --no-window -v --export "Windows Desktop" "${EXPORT_WIN}/export.exe" 2>&1 | tee "${EXPORT_WIN}/../export.win.log"

.PHONY: export/linux 
export/linux: clean
	rm -rf ${EXPORT_LINUX} 
	mkdir -p ${EXPORT_LINUX} 
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --no-window -v --export "Linux/X11" "${EXPORT_LINUX}/export.x86_64" 2>&1 | tee "${EXPORT_LINUX}/../export.linux.log"

.PHONY: export/all
export/all: export/win export/osx export/linux

