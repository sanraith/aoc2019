@echo off
SETLOCAL EnableDelayedExpansion

REM ***
REM Publishes aoc2019.WebApp for github pages to /docs/.
REM
REM - Run tests
REM - Publish the aoc2019.WebApp project
REM - Clean /docs/** directories
REM - Copy all published files and directories to /docs/
REM - Set <base> tag to github pages url in /docs/index.html
REM ***

set webappbase=aoc2019.WebApp
set docsbase=docs
set distbase=%webappbase%\bin\Release\netstandard2.0\publish\aoc2019.WebApp\dist
set githuburl=https://sanraith.github.io/aoc2019/

echo Running tests...
dotnet test --nologo -c Release
if %errorlevel% neq 0 exit /b %errorlevel%

echo Cleaning %webappbase% publish directory...
rmdir /S /Q %webappbase%\bin\Release\netstandard2.0

echo Publishing %webappbase%...
cd %webappbase%
dotnet publish --nologo -c Release
cd ..

echo Deleting %docsbase%\...
rmdir /S /Q %docsbase%

echo Creating %docsbase%\...
mkdir %docsbase%

echo Copying %distbase% -^> %docsbase%\...
xcopy /E /H /R /Q /Y %distbase% %docsbase%\

echo Replacing base url in %docsbase%\index.html to %githuburl%...
powershell -Command "(gc %docsbase%\index.html) -replace '<base href=\"/\" />', '<base href=\"%githuburl%\" />' | Out-File %docsbase%\index.html"
