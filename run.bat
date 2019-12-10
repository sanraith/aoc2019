@echo off
SETLOCAL EnableDelayedExpansion

REM ***
REM Runs aoc2019.ConsoleApp.
REM ***

dotnet run -p aoc2019.ConsoleApp -- %*
