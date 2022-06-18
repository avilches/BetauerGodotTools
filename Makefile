#!/usr/bin/env make

.DEFAULT_SHELL ?= /bin/bash
.DEFAULT_GOAL  := help
ROOT_FOLDER    := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))
BUILD_FOLDER   := ${ROOT_FOLDER}/.mono/temp/bin/Debug
EXPORT_FOLDER  := ${ROOT_FOLDER}/export




TARGET_PLATFORM    ?= osx
GODOT_EXECUTABLE   ?= "/Applications/Godot_mono.app/Contents/MacOS/Godot"
GODOT_SHARP_FOLDER ?= /Applications/Godot_mono.app/Contents/Resources/GodotSharp/Api/Debug


.PHONY: help
help:
	@echo "Usage: make [target] [target2] [target...]"
	@echo "Goals:"
	@echo "    clean      remove all temporal and final dlls"
	@echo "    generate   execute Generator project which creates all *.cs classes for GodotAction"
	@echo "    build      build all projects"
	@echo "    test       run tests from all projects"
	@echo "    editor     open the Godot editor"

.PHONY: clean
clean:
	rm -rf ${ROOT_FOLDER}/.mono
	find ${ROOT_FOLDER} -regex "${ROOT_FOLDER}/Betauer\.[A-Za-z\.]*/bin" -type d | xargs rm -rf
	find ${ROOT_FOLDER} -regex "${ROOT_FOLDER}/Betauer\.[A-Za-z\.]*/obj" -type d  | xargs rm -rf
	mkdir -p ${ROOT_FOLDER}/.mono/assemblies/Debug
	cp -pr ${GODOT_SHARP_FOLDER}/* ${ROOT_FOLDER}/.mono/assemblies/Debug
	
.PHONY: build
build:
	msbuild ${ROOT_FOLDER}/Betauer.sln /restore /t:Build "/p:Configuration=Debug" /v:normal /p:GodotTargetPlatform=${TARGET_PLATFORM}
	cp "${BUILD_FOLDER}/Betauer.Animation.dll" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/Betauer.Animation.pdb" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/Betauer.Core.dll" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/Betauer.Core.pdb" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/Betauer.DI.dll" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/Betauer.DI.pdb" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/Betauer.GameTools.dll" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/Betauer.GameTools.pdb" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/Betauer.StateMachine.dll" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/Betauer.StateMachine.pdb" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/Betauer.TestRunner.dll" "${EXPORT_FOLDER}"  
	cp "${BUILD_FOLDER}/Betauer.TestRunner.pdb" "${EXPORT_FOLDER}"  

.PHONY: generate
generate:
	make -f ${ROOT_FOLDER}/SourceGenerator/Makefile clean build run

.PHONY: test
test:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "TestRunner.cs" --no-window --verbose 

.PHONY: editor
editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor



