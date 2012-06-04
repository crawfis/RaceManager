rem  Execute sql script to populate Hardcard Db tables
rem %1 - SQLServer name 
rem %2 - Database name 
rem %3 - path+sql file name 

rem example: LoadExistingDBForEvent.bat MY-SQLSERVER  Hardcard E:\Hardcard Client\EventDataBaseScripts\event1.sql

:exit(select 100)
@echo off

if "%1"=="" goto on_param1_error
if "%2"=="" goto on_param2_error
if "%3"=="" goto on_param3_error

echo SQL Server name "%1"
echo Dtabase name is "%2" 
echo sql file  "%3"

sqlcmd -e -S %1\sqlexpress -d %2 -i %3
if errorlevel 1 goto on_sqlcmd3_error

goto bat_end

:on_param1_error
  echo No param 1. 
  echo Correct syntax: LoadExistingDBForEvent.bat ServerName DatabaseName  PathAndSqlFileName
goto bat_end

:on_param2_error
   echo No param 2. 
   echo Correct syntax: LoadExistingDBForEvent.bat ServerName DatabaseName  PathAndSqlFileName
goto bat_end

:on_param3_error
   echo No param 3. 
   echo Correct syntax: LoadExistingDBForEvent.bat ServerName DatabaseName  PathAndSqlFileName
goto bat_end

:on_sqlcmd3_error
echo Hardcard.sql - An error occurred 
echo SQLCMD returned %errorlevel% to the command shell
goto bat_end

:bat_end