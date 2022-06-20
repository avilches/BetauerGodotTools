#!/usr/bin/env make

.DEFAULT_SHELL ?= /bin/bash
.DEFAULT_GOAL  := help
ROOT_FOLDER    := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))
include ${ROOT_FOLDER}/application.properties
TIMESTAMP      := $(shell date "+%Y-%m-%d_%H.%M.%S")

BUILD_FOLDER   := ${ROOT_FOLDER}/.mono/temp/bin
EXPORT_FOLDER  := ${ROOT_FOLDER}/export/releases/${VERSION}

TARGET_PLATFORM    ?= osx
GODOT_EXECUTABLE   ?= "/Applications/Godot_mono.app/Contents/MacOS/Godot"
GODOT_SHARP_FOLDER ?= /Applications/Godot_mono.app/Contents/Resources/GodotSharp/Api


.PHONY: help
help:
	@echo "Usage: make [target] [target2] [target...]"
	@echo "Goals:"
	@echo "    clean        remove all temporal and final dlls"
	@echo "    generate     execute Generator project which creates all *.cs classes for GodotAction"
	@echo "    build        build all projects with Configuration='Debug'"
	@echo "    export/dll   build all projects with Configuration='ExportRelease' and copy dlls to export folder"
	@echo "    test         run tests from all projects"
	@echo "    editor       open the Godot editor"
	@echo ""
	@echo "application.properties:"
	@echo "    VERSION         ${VERSION}"
	@echo ""

.PHONY: clean
clean:
	rm -rf "${ROOT_FOLDER}/.mono"
	find "${ROOT_FOLDER}" -regex "${ROOT_FOLDER}/Betauer\.[A-Za-z\.]*/bin" -type d | xargs rm -rf
	find "${ROOT_FOLDER}" -regex "${ROOT_FOLDER}/Betauer\.[A-Za-z\.]*/obj" -type d | xargs rm -rf
	mkdir -p "${ROOT_FOLDER}/.mono/assemblies/Debug"
	mkdir -p "${ROOT_FOLDER}/.mono/assemblies/Release"
	cp -pr "${GODOT_SHARP_FOLDER}/Debug/"* "${ROOT_FOLDER}/.mono/assemblies/Debug"
	cp -pr "${GODOT_SHARP_FOLDER}/Release/"* "${ROOT_FOLDER}/.mono/assemblies/Release"
	
.PHONY: build
build:
	msbuild "${ROOT_FOLDER}/Betauer.sln" /restore /t:Build "/p:Configuration=Debug" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}

.PHONY: export/dll
export/dll: clean
	# If you are not using FreeBSD/MacOX, replace -i '' with just -i 
	find "${ROOT_FOLDER}" -type f -name "AssemblyInfo.cs" -exec sed -i '' 's/.*AssemblyVersion(.*/\[assembly\:\ AssemblyVersion\(\"${VERSION}\"\)\]/g' {} +
	msbuild "${ROOT_FOLDER}/Betauer.sln" /restore /t:Build "/p:Configuration=ExportRelease" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}
	mkdir -p "${EXPORT_FOLDER}"
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.Animation.dll" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.Animation.pdb" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.Core.dll" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.Core.pdb" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.DI.dll" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.DI.pdb" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.GameTools.dll" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.GameTools.pdb" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.StateMachine.dll" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.StateMachine.pdb" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.TestRunner.dll" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.TestRunner.pdb" "${EXPORT_FOLDER}"  

.PHONY: generate
generate:
	make -f "${ROOT_FOLDER}/SourceGenerator/Makefile" clean build run

.PHONY: test
test:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "TestRunner.cs" --no-window --verbose 

.PHONY: editor
editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor



