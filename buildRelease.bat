
set DEFHOMEDRIVE=d:
set DEFHOMEDIR=%DEFHOMEDRIVE%%HOMEPATH%
set HOMEDIR=
set HOMEDRIVE=%CD:~0,2%

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"
echo Default homedir: %DEFHOMEDIR%
if "%HOMEDIR%" == "" (
set HOMEDIR=%DEFHOMEDIR%
)
echo %HOMEDIR%

copy DangerAlerts\bin\Release\DangerAlerts.dll GameData\DangerAlerts\Plugins

copy DangerAlerts.version GameData\DangerAlerts
copy LICENSE.txt GameData\DangerAlerts
copy README.txt GameData\DangerAlerts
copy ..\MiniAVC.dll GameData\DangerAlerts

set VERSIONFILE=DangerAlerts.version

rem The following requires the JQ program, available here: https://stedolan.github.io/jq/download/

c:\local\jq-win64  ".VERSION.MAJOR" %VERSIONFILE% >tmpfile
set /P major=<tmpfile
c:\local\jq-win64  ".VERSION.MINOR"  %VERSIONFILE% >tmpfile
set /P minor=<tmpfile
c:\local\jq-win64  ".VERSION.PATCH"  %VERSIONFILE% >tmpfile
set /P patch=<tmpfile
c:\local\jq-win64  ".VERSION.BUILD"  %VERSIONFILE% >tmpfile
set /P build=<tmpfile

del tmpfile

set VERSION=%major%.%minor%.%patch%

if "%build%" NEQ "0"  set VERSION=%VERSION%.%build%


echo Version:  %VERSION%


set FILE="%RELEASEDIR%\DangerAlerts-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% GameData\DangerAlerts
pause