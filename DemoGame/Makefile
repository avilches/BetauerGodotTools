#!/usr/bin/env make

TARGET_PLATFORM  := "macos"
GODOT_APP        := "/Applications/Godot-4.0.1-rc.1.app"
ASEPRITE_EXECUTABLE := "/Applications/Aseprite.app/Contents/MacOS/aseprite"



.DEFAULT_SHELL   ?= /bin/bash
.DEFAULT_GOAL    := help
ROOT_FOLDER      := $(shell dirname $(realpath $(firstword $(MAKEFILE_LIST))))
include ${ROOT_FOLDER}/application.properties
TIMESTAMP        := $(shell date "+%Y-%m-%d_%H.%M.%S")
EXPORT_OSX       := ${ROOT_FOLDER}/export/releases/${VERSION}/${TIMESTAMP}/osx
EXPORT_WIN       := ${ROOT_FOLDER}/export/releases/${VERSION}/${TIMESTAMP}/win
EXPORT_LINUX     := ${ROOT_FOLDER}/export/releases/${VERSION}/${TIMESTAMP}/linux
DELAY_IMPORT     := 15

GODOT_EXECUTABLE := ${GODOT_APP}/Contents/MacOS/Godot
GODOT_LOGGER_DLL := -l:GodotTools.BuildLogger.GodotBuildLogger,/Applications/Godot-4.0.1-rc.1.app/Contents/Resources/GodotSharp/Tools/GodotTools.BuildLogger.dll;${ROOT_FOLDER}/.godot/mono/build_logs

CURRENT_VERSION:= $(shell cat ${ROOT_FOLDER}/project.godot | grep "config/version" | cut -d'=' -f2| tr -d '"')
SOLUTION:= $(shell cat ${ROOT_FOLDER}/project.godot | grep "config/name" | cut -d'=' -f2| tr -d '"')
CSPRJ_FOLDER:= $(shell cat ${ROOT_FOLDER}/project.godot | grep "project/c\#_project_directory" | cut -d'=' -f2| tr -d '"')

.PHONY: help
help:
	@echo "Usage: make [target] [target2] [target...]"
	@echo "Goals:"
	@echo "    clean          remove all .mono folders and copy again the Godot assemblies"
	@echo "    import         import resources"
	@echo "    test           run tests"
	@echo "    run            run the game"
	@echo "    editor         open the Godot editor"
	@echo "    build/Debug           dotnet build with Configuration='Debug'"
	@echo "    build/ExportDebug     dotnet build with Configuration='ExportDebug'"
	@echo "    build/ExportRelease   dotnet build with Configuration='ExportRelease'"
	@echo "    ase            generate sprites from .ase files"
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
	@echo "Giving some time to the editor to create the imported files..."
	@for (( i=${DELAY_IMPORT}; i>0; i-- )) do echo "$$i..." ; sleep 1 ; done

# Kill 
	@echo "Killing editor..." 
	@pkill -F "${ROOT_FOLDER}/.godot.editor.pid"
	@rm "${ROOT_FOLDER}/.godot.editor.pid"
 
.PHONY: build/Debug
build/Debug:
	dotnet build "${ROOT_FOLDER}/${SOLUTION}.sln" -c Debug -v normal -p:GodotTargetPlatform=${TARGET_PLATFORM} "${GODOT_LOGGER_DLL}/Debug"
	@echo "project.godot:"  
	@echo "    config/version=\"${CURRENT_VERSION}\""
	@echo "    config/name=\"${SOLUTION}\""

.PHONY: build/ExportDebug
build/ExportDebug:
	dotnet build "${ROOT_FOLDER}/${SOLUTION}.sln" -c ExportDebug -v normal -p:GodotTargetPlatform=${TARGET_PLATFORM} "${GODOT_LOGGER_DLL}/ExportDebug"
	@echo "project.godot:"  
	@echo "    config/version=\"${CURRENT_VERSION}\""
	@echo "    config/name=\"${SOLUTION}\""

.PHONY: build/ExportRelease
build/ExportRelease:
	dotnet build "${ROOT_FOLDER}/${SOLUTION}.sln" -c ExportRelease -v normal -p:GodotTargetPlatform=${TARGET_PLATFORM} "${GODOT_LOGGER_DLL}/ExportRelease"
	@echo "project.godot:"  
	@echo "    config/version=\"${CURRENT_VERSION}\""
	@echo "    config/name=\"${SOLUTION}\""

 
.PHONY: run
run:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --verbose

.PHONY: test
test:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "${CSPRJ_FOLDER}/Tests/RunTests.cs" --headless --verbose

.PHONY: editor
editor:
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --editor --verbose

.PHONY: export/osx
export/osx: check-version
	mkdir -p ${EXPORT_OSX} 
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --headless -v --export "Mac OSX" "${EXPORT_OSX}/${NAME}-${VERSION}.zip" 2>&1 | tee "${EXPORT_OSX}/../osx.log"
	unzip "${EXPORT_OSX}/${NAME}-${VERSION}.zip" -d "${EXPORT_OSX}"
	open "${EXPORT_OSX}" 

.PHONY: export/win
export/win: check-version
	mkdir -p ${EXPORT_WIN} 
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --headless -v --export "Windows Desktop" "${EXPORT_WIN}/${NAME}.exe" 2>&1 | tee "${EXPORT_WIN}/../win.log"

.PHONY: export/linux 
export/linux: check-version
	mkdir -p ${EXPORT_LINUX} 
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" --headless -v --export "Linux/X11" "${EXPORT_LINUX}/${NAME}.x86_64" 2>&1 | tee "${EXPORT_LINUX}/../linux.log"

.PHONY: release
release:
	@echo "project.godot:"  
	@echo "    config/version=\"${CURRENT_VERSION}\""
	@echo "    config/name=\"${SOLUTION}\""
	@echo ""
	@echo "Using ${SOLUTION}.sln solution"
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
	${GODOT_EXECUTABLE} --path "${ROOT_FOLDER}" -s "${CSPRJ_FOLDER}/Build/Release.cs" --headless

.PHONY: export/all
export/all: clean build/Debug release export/win export/osx export/linux

check-version:
	@echo "Checking version..."
	@echo ""
	@echo "project.godot:"  
	@echo "    config/version=\"${CURRENT_VERSION}\""
	@echo "    config/name=\"${SOLUTION}\""
	@echo ""
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
	@echo "Using ${SOLUTION}.sln solution"
	@echo ""
endif

.PHONY: ase
ase:
	${ASEPRITE_EXECUTABLE} -b --layer "Body" --layer "Body/effects" "${ROOT_FOLDER}/Src/Assets/Leon.ase" --sheet-type horizontal --sheet "${ROOT_FOLDER}/Characters/Player-Leon/Leon.png"
	${ASEPRITE_EXECUTABLE} -b --layer "Metalbar" --layer "Metalbar/effects" "${ROOT_FOLDER}/Src/Assets/Leon.ase" --sheet-type horizontal --sheet "${ROOT_FOLDER}/Characters/Player-Leon/Leon-metalbar.png"
	${ASEPRITE_EXECUTABLE} -b --layer "Knife" --layer "Knife/effects" "${ROOT_FOLDER}/Src/Assets/Leon.ase" --sheet-type horizontal --sheet "${ROOT_FOLDER}/Characters/Player-Leon/Leon-knife.png"
	${ASEPRITE_EXECUTABLE} -b --layer "Gun1" --layer "Gun1/effects" "${ROOT_FOLDER}/Src/Assets/Leon.ase" --sheet-type horizontal --sheet "${ROOT_FOLDER}/Characters/Player-Leon/Leon-gun1.png"

