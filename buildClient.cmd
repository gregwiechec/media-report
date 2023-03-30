REM Build the javascripts
CALL yarn --cwd client/ build
IF %errorlevel% NEQ 0 EXIT /B %errorlevel%

copy client\dist\assets\index.js src\MediaReport\ClientResources
copy client\dist\assets\index.js.map src\MediaReport\ClientResources

EXIT /B %ERRORLEVEL%