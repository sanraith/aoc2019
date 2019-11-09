@echo off
SETLOCAL EnableDelayedExpansion

REM ***
REM - Publish the aoc2019.WebApp project
REM - Clean /docs/** directories
REM - Copy all published files and directories to /docs/
REM - Set <base> tag to github pages url in /docs/index.html
REM ***

set webappbase=aoc2019.WebApp
set docsbase=docs
set distbase=%webappbase%\bin\Release\netstandard2.0\publish\aoc2019.WebApp\dist
set githuburl=https://sanraith.github.io/aoc2019/

cd %webappbase%
dotnet publish -c Release
cd ..

for /f "tokens=*" %%G in ('dir /b /a:d "%docsbase%"') do (
	echo Deleting: %docsbase%\%%G
	rmdir /S /Q %docsbase%\%%G
)

echo Copying %distbase% -^> %docsbase%\
xcopy /E /H /R /Q /Y %distbase% %docsbase%\

powershell -Command "(gc %docsbase%\index.html) -replace '<base href=\"/\" />', '<base href=\"%githuburl%\" />' | Out-File %docsbase%\index.html"
