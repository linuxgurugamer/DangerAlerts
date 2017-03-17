

set H=R:\KSP_1.2.2_dev
echo %H%

copy DangerAlerts\bin\Debug\DangerAlerts.dll GameData\DangerAlerts\Plugins

xcopy /y /s "GameData\DangerAlerts" "%H%\GameData\DangerAlerts"
