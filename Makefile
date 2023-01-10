#!/usr/bin/env make

TARGET_PLATFORM  := "macos"
GODOT_APP        := "/Applications/Godot-4.0.0-beta.11.app"
MODULES          := Betauer.Animation Betauer.Core Betauer.Bus Betauer.DI Betauer.GameTools Betauer.Tools.Logging Betauer.StateMachine Betauer.TestRunner Betauer.Tools.Reflection
DELAY_IMPORT     := 10

.DEFAULT_SHELL   ?= /bin/bash
.DEFAULT_GOAL    := help
ROOT_FOLDER      := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))
include ${ROOT_FOLDER}/application.properties
TIMESTAMP        := $(shell date "+%Y-%m-%d_%H.%M.%S")
BUILD_FOLDER     := ${ROOT_FOLDER}/.godot/mono/temp/bin
EXPORT_FOLDER    := ${ROOT_FOLDER}/export/releases/${VERSION}


GODOT_EXECUTABLE := ${GODOT_APP}/Contents/MacOS/Godot
GODOT_LOGGER_DLL := -l:GodotTools.BuildLogger.GodotBuildLogger,/Applications/Godot-4.0.0-beta.11.app/Contents/Resources/GodotSharp/Tools/GodotTools.BuildLogger.dll;${ROOT_FOLDER}/.godot/mono/build_logs


.PHONY: help
help:
	@echo "Usage: make [target] [target2] [target...]"
	@echo "Goals:"
	@echo "    clean           remove all .mono folders and copy again the Godot assemblies"
	@echo "    import          import resources"
	@echo "    test            run tests"
	@echo "    editor          open Godot editor"
	@echo "    generate        execute Generator project which creates all *.cs classes"
	@echo "    build/Debug           dotnet build all projects with Configuration='Debug'"
	@echo "    build/ExportDebug     dotnet build all projects with Configuration='ExportDebug'"
	@echo "    build/ExportRelease   dotnet build all projects with Configuration='ExportRelease'"
	@echo "    export/dll      build Debug and ExportRelease + copy dlls to export folder"
	@echo ""
	@echo "application.properties:"
	@echo "    VERSION         ${VERSION}"
	@echo ""

.PHONY: clean
clean:
	rm -rf "${ROOT_FOLDER}/.mono"
	rm -rf "${ROOT_FOLDER}/.godot/mono"

.PHONY: import
import:
# Delete and run editor
	rm -rf "${ROOT_FOLDER}/.import"
	rm -rf "${ROOT_FOLDER}/.godot/imported"
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --headless -v --editor & echo "$$!" > "${ROOT_FOLDER}/.godot.editor.pid"
	@echo "Editor pid is:" 
	@cat "${ROOT_FOLDER}/.godot.editor.pid"

# Wait
	@echo "Giving some to the editor to create the imported files..."
	@for (( i=${DELAY_IMPORT}; i>0; i-- )) do echo "$$i..." ; sleep 1 ; done

# Kill 
	@echo "Killing editor..." 
	@pkill -F "${ROOT_FOLDER}/.godot.editor.pid"
	@rm "${ROOT_FOLDER}/.godot.editor.pid"
 
.PHONY: build/Debug
build/Debug:
	dotnet build "${ROOT_FOLDER}/Betauer4.sln" -c Debug -v normal -p:GodotTargetPlatform=${TARGET_PLATFORM} "${GODOT_LOGGER_DLL}/Debug"

.PHONY: build/ExportDebug
build/ExportDebug:
	dotnet build "${ROOT_FOLDER}/Betauer4.sln" -c ExportDebug -v normal -p:GodotTargetPlatform=${TARGET_PLATFORM} "${GODOT_LOGGER_DLL}/ExportDebug"

.PHONY: build/ExportRelease
build/ExportRelease:
	dotnet build "${ROOT_FOLDER}/Betauer4.sln" -c ExportRelease -v normal -p:GodotTargetPlatform=${TARGET_PLATFORM} "${GODOT_LOGGER_DLL}/ExportRelease"

.PHONY: export/dll
export/dll:
	$(MAKE) bump
	$(MAKE) clean
	$(MAKE) build/Debug 
	$(MAKE) build/ExportDebug 
	$(MAKE) build/ExportRelease 
	mkdir -p "${EXPORT_FOLDER}/ExportRelease"
	mkdir -p "${EXPORT_FOLDER}/ExportDebug"
	mkdir -p "${EXPORT_FOLDER}/Debug"
	for dll in $(MODULES); do \
	   echo "Copying $$dll dlls to ${EXPORT_FOLDER}"; \
	   cp "${BUILD_FOLDER}/ExportRelease/$${dll}.dll" "${EXPORT_FOLDER}/ExportRelease"; \
	   cp "${BUILD_FOLDER}/ExportRelease/$${dll}.pdb" "${EXPORT_FOLDER}/ExportRelease"; \
	   cp "${BUILD_FOLDER}/ExportDebug/$${dll}.dll" "${EXPORT_FOLDER}/ExportDebug"; \
	   cp "${BUILD_FOLDER}/ExportDebug/$${dll}.pdb" "${EXPORT_FOLDER}/ExportDebug"; \
	   cp "${BUILD_FOLDER}/Debug/$${dll}.dll" "${EXPORT_FOLDER}/Debug"; \
	   cp "${BUILD_FOLDER}/Debug/$${dll}.pdb" "${EXPORT_FOLDER}/Debug"; \
	done

.PHONY: bump
bump:
	# If you are not using FreeBSD/MacOX, replace -i '' with just -i 
	find "${ROOT_FOLDER}" -type f -name "AssemblyInfo.cs" -exec sed -i '' 's/.*AssemblyVersion(.*/\[assembly\:\ AssemblyVersion\(\"${VERSION}\"\)\]/g' {} +

.PHONY: generate
generate:
	make -f "${ROOT_FOLDER}/SourceGenerator/Makefile" clean build run

.PHONY: test
test:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "RunTests.cs" --headless --verbose 

.PHONY: editor
editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor



