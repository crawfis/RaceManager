rem Is used to drop Hardcard Db, create Hardcard Db, run Hardcard.sql to create and populate Hardcard Db tables, execute setup.exe 
rem If SQLServer name is MY-SQLSERVER and Hardcard.sql&setup.exe are in the current directory then correct syntax example :
rem Hardcard_start.bat MY-SQLSERVER\sqlexpress . (it is .)

:exit(select 100)
@echo off

if "%1"=="" goto on_param1_error
if "%2"=="" goto on_param2_error

echo SQL Server name "%1"
echo Hardcard.sql in "%2" directory

sqlcmd -e -S %1\sqlexpress -Q "drop database Hardcard"

sqlcmd -e -S %1\sqlexpress -Q "create database Hardcard"
if errorlevel 1 goto on_sqlcmd2_error

sqlcmd -e -S %1\sqlexpress -d Hardcard -i "%2"\Hardcard.sql
if errorlevel 1 goto on_sqlcmd3_error

%2\setup.exe
if errorlevel 1 goto on_setup_error

goto bat_end

:on_param1_error
  echo No param 1. 
  echo Correct syntax: Hardcard_start.bat ServerName ScriptDirectory (or . for current directory) 
goto bat_end

:on_param2_error
   echo No param 2. 
   echo Correct syntax: Hardcard_start.bat ServerName ScriptDirectory (or . for current directory) 
goto bat_end

:on_sqlcmd_error
echo SQLCMD returned %errorlevel% to the command shell
goto bat_end

:on_sqlcmd2_error
echo create database Hardcard - An error occurred 
echo SQLCMD returned %errorlevel% to the command shell
goto bat_end

:on_sqlcmd3_error
echo Hardcard.sql - An error occurred 
echo SQLCMD returned %errorlevel% to the command shell
goto bat_end

:on_setup_error
echo  setup.exe - An error occurred 
echo setup.exe returned %errorlevel% to the command shell
goto bat_end

:bat_end