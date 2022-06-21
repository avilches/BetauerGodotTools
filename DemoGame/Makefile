#!/usr/bin/env make

.DEFAULT_SHELL ?= /bin/bash
.DEFAULT_GOAL  := help
ROOT_FOLDER    := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))
include ${ROOT_FOLDER}/application.properties
TIMESTAMP      := $(shell date "+%Y-%m-%d_%H.%M.%S")
EXPORT_OSX     := ${ROOT_FOLDER}/export/releases/${VERSION}/${TIMESTAMP}/osx
EXPORT_WIN     := ${ROOT_FOLDER}/export/releases/${VERSION}/${TIMESTAMP}/win
EXPORT_LINUX   := ${ROOT_FOLDER}/export/releases/${VERSION}/${TIMESTAMP}/linux

TARGET_PLATFORM    ?= osx
GODOT_EXECUTABLE   ?= "/Applications/Godot_mono.app/Contents/MacOS/Godot"
GODOT_SHARP_FOLDER ?= /Applications/Godot_mono.app/Contents/Resources/GodotSharp/Api

CURRENT_VERSION:= $(shell cat ${ROOT_FOLDER}/project.godot | grep "config/version" | cut -d'=' -f2| tr -d '"')

.PHONY: help
help:
	@echo "Usage: make [target] [target2] [target...]"
	@echo "Goals:"
	@echo "    clean          remove all .mono folders"
	@echo "    test           run tests"
	@echo "    run            run the game"
	@echo "    editor         open the Godot editor"
	@echo "    build/debug    build project"
	@echo "    release        run Release.cs, it uses the [application.properties] file to bump version and fixes the project"
	@echo "    export/osx     create binary for osx"
	@echo "    export/win     create binary for Windows"
	@echo "    export/linux   create binary for Linux"
	@echo "    export/all     create binary for all"
	@echo ""
	@echo "project.godot:"  
	@echo "    config/version=\"${CURRENT_VERSION}\""
	@echo ""
	@echo "application.properties:"
	@echo "    NAME            ${NAME}"
	@echo "    ID              ${ID}"
	@echo "    DESCRIPTION     ${DESCRIPTION}"
	@echo "    AUTHOR          ${AUTHOR}"
	@echo "    VERSION         ${VERSION}"
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
	msbuild "${ROOT_FOLDER}/Veronenger.sln" /restore /t:Build "/p:Configuration=Debug" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}

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
export/osx: check-version
	mkdir -p ${EXPORT_OSX} 
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --no-window -v --export "Mac OSX" "${EXPORT_OSX}/${NAME}-${VERSION}.zip" 2>&1 | tee "${EXPORT_OSX}/../osx.log"
	unzip "${EXPORT_OSX}/${NAME}-${VERSION}.zip" -d "${EXPORT_OSX}" 

.PHONY: export/win
export/win: check-version
	mkdir -p ${EXPORT_WIN} 
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --no-window -v --export "Windows Desktop" "${EXPORT_WIN}/${NAME}.exe" 2>&1 | tee "${EXPORT_WIN}/../win.log"

.PHONY: export/linux 
export/linux: check-version
	mkdir -p ${EXPORT_LINUX} 
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --no-window -v --export "Linux/X11" "${EXPORT_LINUX}/${NAME}.x86_64" 2>&1 | tee "${EXPORT_LINUX}/../linux.log"

.PHONY: release
release:
	@echo "project.godot:"  
	@echo "    config/version=\"${CURRENT_VERSION}\""
	@echo ""
	@echo "application.properties:"
	@echo "    NAME            ${NAME}"
	@echo "    ID              ${ID}"
	@echo "    DESCRIPTION     ${DESCRIPTION}"
	@echo "    AUTHOR          ${AUTHOR}"
	@echo "    VERSION         ${VERSION}"
	@echo ""
	@echo "Update project.godot and exports_presets.cfg?"
	@echo "(CTRL+C to stop)"
	@read -p ""
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "export/Release.cs" --no-window

.PHONY: export/all
export/all: clean build release export/win export/osx export/linux

check-version:
	@echo "Checking version..."
	@echo ""
	@echo "project.godot:"  
	@echo "    config/version=\"${CURRENT_VERSION}\""
	@echo ""
	@echo "application.properties:"
	@echo "    VERSION         ${VERSION}"
	@echo ""
ifneq ($(CURRENT_VERSION), $(VERSION))
	@echo ""
	@echo "Current project.godot version != new version"
	@echo ""
	make -f "${ROOT_FOLDER}/Makefile" build release
else
	@echo "Version matches, exporting..."
	@echo ""
	@echo ""
endif
