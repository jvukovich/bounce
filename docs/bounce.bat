@cd packages\bounce.*
@tools\net40\bounce.exe %*
@set bounce_exit_code=%errorlevel%
@cd ..\..
@exit /b %bounce_exit_code%