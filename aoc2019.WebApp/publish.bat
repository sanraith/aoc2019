@echo off
SETLOCAL EnableDelayedExpansion

REM ***
REM - Publish the project
REM - Clean /docs/** directories
REM - Copy all published files and directories to /docs/ EXCEPT the files in the dist root. (To prevent overwriting custom index.html.)
REM ***

SET docsbase=..\docs\
SET distbase=bin\Release\netstandard2.0\publish\aoc2019.WebApp\dist\

dotnet publish -c Release

for /f "tokens=*" %%G in ('dir /b /a:d "%docsbase%"') do (
	echo Deleting: %docsbase%%%G
	rmdir /S /Q %docsbase%%%G
)

for /f "tokens=*" %%G in ('dir /b /a:d "%distbase%"') do (
	echo Copying: %distbase%%%G
	xcopy /E /Y %distbase%%%G %docsbase%%%G\
)
