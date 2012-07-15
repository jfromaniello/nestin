@echo off
call "%VS110COMNTOOLS%..\..\VC\vcvarsall.bat"
msbuild %~dp0default.build /t:All /nologo
