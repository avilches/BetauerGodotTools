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
	@echo "    clean           remove all .mono folders"
	@echo "    test            run tests"
	@echo "    editor          open Godot editor"
	@echo "    generate        execute Generator project which creates all *.cs classes"
	@echo "    build/debug     build all projects with Configuration='Debug'"
	@echo "    build/release   build all projects with Configuration='ExportRelease'"
	@echo "    export/dll      build Debug and ExportRelease + copy dlls to export folder"
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
	
.PHONY: build/debug
build/debug:
	#msbuild "${ROOT_FOLDER}/Betauer.sln" /restore /t:Build "/p:Configuration=Debug" /p:GodotTargetPlatform=${TARGET_PLATFORM}
	dotnet build "${ROOT_FOLDER}/Betauer.sln" --configuration Debug 

.PHONY: build/release
build/release:
#	msbuild "${ROOT_FOLDER}/Betauer.sln" /restore /t:Build "/p:Configuration=ExportRelease" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}
	dotnet build "${ROOT_FOLDER}/Betauer.sln" --configuration ExportRelease 

.PHONY: export/dll
export/dll: clean bump build/debug build/release 
	mkdir -p "${EXPORT_FOLDER}/ExportRelease"
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.Animation.dll" "${EXPORT_FOLDER}/ExportRelease"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.Animation.pdb" "${EXPORT_FOLDER}/ExportRelease"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.Core.dll" "${EXPORT_FOLDER}/ExportRelease"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.Core.pdb" "${EXPORT_FOLDER}/ExportRelease"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.DI.dll" "${EXPORT_FOLDER}/ExportRelease"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.DI.pdb" "${EXPORT_FOLDER}/ExportRelease"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.GameTools.dll" "${EXPORT_FOLDER}/ExportRelease"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.GameTools.pdb" "${EXPORT_FOLDER}/ExportRelease"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.StateMachine.dll" "${EXPORT_FOLDER}/ExportRelease"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.StateMachine.pdb" "${EXPORT_FOLDER}/ExportRelease"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.TestRunner.dll" "${EXPORT_FOLDER}/ExportRelease"  
	cp "${BUILD_FOLDER}/ExportRelease/Betauer.TestRunner.pdb" "${EXPORT_FOLDER}/ExportRelease"
	mkdir -p "${EXPORT_FOLDER}/Debug"
	cp "${BUILD_FOLDER}/Debug/Betauer.Animation.dll" "${EXPORT_FOLDER}/Debug"  
	cp "${BUILD_FOLDER}/Debug/Betauer.Animation.pdb" "${EXPORT_FOLDER}/Debug"  
	cp "${BUILD_FOLDER}/Debug/Betauer.Core.dll" "${EXPORT_FOLDER}/Debug"  
	cp "${BUILD_FOLDER}/Debug/Betauer.Core.pdb" "${EXPORT_FOLDER}/Debug"  
	cp "${BUILD_FOLDER}/Debug/Betauer.DI.dll" "${EXPORT_FOLDER}/Debug"  
	cp "${BUILD_FOLDER}/Debug/Betauer.DI.pdb" "${EXPORT_FOLDER}/Debug"  
	cp "${BUILD_FOLDER}/Debug/Betauer.GameTools.dll" "${EXPORT_FOLDER}/Debug"  
	cp "${BUILD_FOLDER}/Debug/Betauer.GameTools.pdb" "${EXPORT_FOLDER}/Debug"  
	cp "${BUILD_FOLDER}/Debug/Betauer.StateMachine.dll" "${EXPORT_FOLDER}/Debug"  
	cp "${BUILD_FOLDER}/Debug/Betauer.StateMachine.pdb" "${EXPORT_FOLDER}/Debug"  
	cp "${BUILD_FOLDER}/Debug/Betauer.TestRunner.dll" "${EXPORT_FOLDER}/Debug"  
	cp "${BUILD_FOLDER}/Debug/Betauer.TestRunner.pdb" "${EXPORT_FOLDER}/Debug"  

.PHONY: bump
bump:
	# If you are not using FreeBSD/MacOX, replace -i '' with just -i 
	find "${ROOT_FOLDER}" -type f -name "AssemblyInfo.cs" -exec sed -i '' 's/.*AssemblyVersion(.*/\[assembly\:\ AssemblyVersion\(\"${VERSION}\"\)\]/g' {} +

.PHONY: generate
generate:
	make -f "${ROOT_FOLDER}/SourceGenerator/Makefile" clean build run

.PHONY: test
test:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "TestRunner.cs" --no-window --verbose 

.PHONY: editor
editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor



