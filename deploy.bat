

set H=R:\KSP_1.3.0_dev
echo %H%

copy DangerAlerts\bin\Debug\DangerAlerts.dll GameData\DangerAlerts\Plugins

xcopy /y /s "GameData\DangerAlerts" "%H%\GameData\DangerAlerts"
