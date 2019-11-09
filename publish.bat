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

echo Publishing %webappbase%
cd %webappbase%
dotnet publish -c Release
cd ..

echo Deleting %docsbase%\
rmdir /S /Q %docsbase%

echo Creating %docsbase%\
mkdir %docsbase%

echo Copying %distbase% -^> %docsbase%\
xcopy /E /H /R /Q /Y %distbase% %docsbase%\

echo Replacing base url in %docsbase%\index.html to %githuburl%
powershell -Command "(gc %docsbase%\index.html) -replace '<base href=\"/\" />', '<base href=\"%githuburl%\" />' | Out-File %docsbase%\index.html"
