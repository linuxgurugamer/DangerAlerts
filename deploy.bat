

set H=R:\KSP_1.3.1_dev
echo %H%

copy DangerAlerts\bin\Debug\DangerAlerts.dll GameData\DangerAlerts\Plugins

xcopy /y /i /e "GameData\DangerAlerts" "%H%\GameData\DangerAlerts"
