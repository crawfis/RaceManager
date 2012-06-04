@echo off
rem drop and create Hardcard Database. Create database schema and populate it with recirds  (, execute setup.exe - commented)
@echo off
rem  If SQLServer name is MY-SQLSERVER and Hardcard*.sql files and setup.exe are in the current directory then correct syntax example : 
@echo off
rem  Hardcard_start.bat MY-SQLSERVER . (it is .)

:exit(select 100)
@echo off

if "%1"=="" goto on_param1_error
if "%2"=="" goto on_param2_error

echo SQL Server name "%1"
echo Hardcard.sql in "%2" directory

sqlcmd -e -S %1\sqlexpress -Q "drop database Hardcard"

sqlcmd -e -S %1\sqlexpress -Q "create database Hardcard"
if errorlevel 1 goto on_sqlcmd2_error

sqlcmd -S %1\sqlexpress -d Hardcard  -i %2\drop_triggers_for_SQL_EX.sql
if errorlevel 1 goto on_sqlcmd3_error
sqlcmd -S %1\sqlexpress -d Hardcard  -i %2\drop_tables_for_SQL_EX.sql
if errorlevel 1 goto on_sqlcmd3_error
sqlcmd -S %1\sqlexpress -d Hardcard  -i %2\create_tables_for_SQL_EX.sql
if errorlevel 1 goto on_sqlcmd3_error
sqlcmd -S %1\sqlexpress -d Hardcard  -i %2\create_triggers_for_SQL_EX.sql
if errorlevel 1 goto on_sqlcmd3_error
sqlcmd -S %1\sqlexpress -d Hardcard  -i %2\create_records_for_SQL_EX.sql
if errorlevel 1 goto on_sqlcmd3_error

rem %2\setup.exe
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