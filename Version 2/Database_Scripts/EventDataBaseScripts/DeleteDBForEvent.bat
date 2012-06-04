rem Execute sql script to delete records from Hardcard tables
rem %1 - SQLServer name 
rem %2 - Database name
rem %3 - path+sql file name  

rem example: DeleteDBForEvent.bat MY-SQLSERVER  Hardcard E:\user_June\OSU_RogerS\source\Database_Scripts\delete_event_tables_for_SQL_EX.sql

:exit(select 100)
@echo off

if "%1"=="" goto on_param1_error
if "%2"=="" goto on_param2_error
if "%3"=="" goto on_param3_error

echo SQL Server name "%1"
echo Dtabase name is "%2" 
echo path+sql file name is "%3" 

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