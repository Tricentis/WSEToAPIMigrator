;Include Modern UI
;usgae : "C:\Program Files (x86)\NSIS\makensis.exe" /OutputFileName="WseToApiMigrator  /DProductVersion="1.0.0.0" basic.nsi
 !include "MUI2.nsh"

;--------------------------------
!define MUI_ICON "MUI_ICON.ico"
 !define MUI_ABORTWARNING
 !define FindProc_NOT_FOUND 1
!define FindProc_FOUND 0

Function .onInit

;--------------To Change Installation Directory According To Tosca Commander Version------------

ReadRegStr $0 HKLM "SYSTEM\CurrentControlSet\Control\Session Manager\Environment" "COMMANDER_HOME"
${If} ${Errors}
StrCpy $INSTDIR "$%TRICENTIS_HOME%"
${Else}
StrCpy $INSTDIR "$%COMMANDER_HOME%"
${EndIf}

;-----------------------------------------------------------------------------------------------

ClearErrors
ReadRegStr $0 HKLM "Software\Tricentis" "Home_long"
${If} ${Errors}
 MessageBox MB_OK|MB_ICONEXCLAMATION "Tosca Commander is not installed in the system. This AddOn works with Tosca Commander only." /SD IDOK
 Abort
${EndIf}
!macro FindProc result processName
    ExecCmd::exec "%SystemRoot%\System32\tasklist /NH /FI $\"IMAGENAME eq ${processName}$\" | %SystemRoot%\System32\find /I $\"${processName}$\"" 
    Pop $0 ; The handle for the process
    ExecCmd::wait $0
    Pop ${result} ; The exit code
!macroend
 
Var /GLOBAL processFound
!insertmacro FindProc $processFound "ToscaCommander.exe"

;pop $R0

IntCmp $processFound 1 notRunning
     MessageBox MB_OK|MB_ICONEXCLAMATION "Tosca Commander is running. Please close it first to proceed with the installation." /SD IDOK
     Abort
notRunning:
IfFileExists "$INSTDIR\WseToApiMigrationAddOn.dll" 0 +2
Delete "$INSTDIR\WseToApiMigrationAddOn.dll"
FunctionEnd

;General
; The name of the installer
Name "Tosca Commander - WSE to API Migrator AddOn"

; The file to write
OutFile "..\..\..\..\dist\${OutputFileName}"

; The default installation directory
;InstallDir "$%TRICENTIS_HOME%"
;InstallDirRegKey HKLM "Software\Tricentis" "Home_long"

; Request application privileges
RequestExecutionLevel admin

;--------------------------------
;Interface Settings

;--------------------------------
;Pages


 !insertmacro MUI_PAGE_WELCOME
 !insertmacro MUI_PAGE_LICENSE "License.rtf"
 !insertmacro MUI_PAGE_DIRECTORY
 !insertmacro MUI_PAGE_INSTFILES
 !insertmacro MUI_PAGE_FINISH

;--------------------------------
;Languages

 !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "SoapUI" SoapUIAPIImporterSection
 ; Set output path to the installation directory.
 SetOutPath $INSTDIR

 ; Put file there
 File "..\..\..\..\dist\WseToApiMigrationAddOn.dll"
SectionEnd

;--------------------------------

;Version Information
 VIProductVersion ${ProductVersion}
 VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "Tricentis Tosca Wse To Api Migrator AddOn"
 VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "Tricentis GmbH"
 VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalTrademarks" "Application is a trademark of Tricentis"
 VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "Tricentis GmbH"
 VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "Setup Installer for Wse To Api Migrator Add-on"
 VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" ${ProductVersion}
 VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductVersion" ${ProductVersion}
;--------------------------------