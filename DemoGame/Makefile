#!/usr/bin/env make

.DEFAULT_SHELL ?= /bin/bash
.DEFAULT_GOAL  := help
ROOT_FOLDER    := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))
VERSION        := $(shell cat ${ROOT_FOLDER}/export/app-version.txt)
TIMESTAMP      := $(shell date "+%Y-%m-%d_%H.%M.%S")
NAME           := Veronenger
EXPORT_OSX     := ${ROOT_FOLDER}/export/releases/${VERSION}/${TIMESTAMP}/osx
EXPORT_WIN     := ${ROOT_FOLDER}/export/releases/${VERSION}/${TIMESTAMP}/win
EXPORT_LINUX   := ${ROOT_FOLDER}/export/releases/${VERSION}/${TIMESTAMP}/linux

CONFIGURATION      ?= Debug
TARGET_PLATFORM    ?= osx
GODOT_EXECUTABLE   ?= "/Applications/Godot_mono.app/Contents/MacOS/Godot"
GODOT_SHARP_FOLDER ?= /Applications/Godot_mono.app/Contents/Resources/GodotSharp/Api/${CONFIGURATION}

.PHONY: help
help:
	@echo "Usage: make [target] [target2] [target...]"
	@echo "Goals:"
	@echo "    clean        remove all temporal and final dlls"
	@echo "    build        build all projects"
	@echo "    test         run tests from all projects"
	@echo "    run          run the game"
	@echo "    editor       open the Godot editor"
	@echo ""
	@echo "    app-version  upgrade the application to version $(VERSION)"
	@echo "    export/osx   create binary for osx in ${EXPORT_OSX}"
	@echo "    export/win   create binary for Windows in ${EXPORT_OSX}"
	@echo "    export/linux create binary for Linux in ${EXPORT_OSX}"
	@echo "    export/all   create binary for all"

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
export/osx:
	mkdir -p ${EXPORT_OSX} 
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --no-window -v --export "Mac OSX" "${EXPORT_OSX}/${NAME}-${VERSION}.zip" 2>&1 | tee "${EXPORT_OSX}/../osx.log"
	unzip "${EXPORT_OSX}/${NAME}-${VERSION}.zip" -d "${EXPORT_OSX}" 

.PHONY: export/win
export/win:
	mkdir -p ${EXPORT_WIN} 
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --no-window -v --export "Windows Desktop" "${EXPORT_WIN}/${NAME}.exe" 2>&1 | tee "${EXPORT_WIN}/../win.log"

.PHONY: export/linux 
export/linux:
	mkdir -p ${EXPORT_LINUX} 
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --no-window -v --export "Linux/X11" "${EXPORT_LINUX}/${NAME}.x86_64" 2>&1 | tee "${EXPORT_LINUX}/../linux.log"

.PHONY: app-version
app-version:
	@echo ""
	@echo "AppVersion: ${VERSION}"
	@echo ""
	@echo "Continue? CTRL+C to stop"
	@read -p ""
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "export/AppVersion.cs" --no-window


.PHONY: export/all
export/all: export/win export/osx export/linux

