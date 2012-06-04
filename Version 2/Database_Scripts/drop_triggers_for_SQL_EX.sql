--DROP TRIGGERS

/* 02/09/2012 Eugene
   Drop 14 triggers
   command string to execute from C:\Program Files\Microsoft Visual Studio 10.0\VC>
   sqlcmd -S .\sqlexpress -d Hardcard  -i E:\user_June\OSU_Roger\source\Database_Scripts\drop_triggers_for_SQL_EX.sql
*/

IF EXISTS 
(SELECT * from sys.triggers where name = 'insert_raceclass') 
DROP TRIGGER insert_raceclass;
IF EXISTS 
(SELECT * from sys.triggers where name = 'update_raceclass') 
DROP TRIGGER update_raceclass;

IF EXISTS 
(SELECT * from sys.triggers where name = 'insert_athlete') 
DROP TRIGGER insert_athlete;
IF EXISTS 
(SELECT * from sys.triggers where name = 'update_athlete') 
DROP TRIGGER update_athlete;

IF EXISTS 
(SELECT * from sys.triggers where name = 'insert_competitor') 
DROP TRIGGER insert_competitor;
IF EXISTS 
(SELECT * from sys.triggers where name = 'update_competitor') 
DROP TRIGGER update_competitor;

IF EXISTS 
(SELECT * from sys.triggers where name = 'insert_entry') 
DROP TRIGGER insert_entry;
IF EXISTS 
(SELECT * from sys.triggers where name = 'update_entry') 
DROP TRIGGER update_entry;

IF EXISTS 
(SELECT * from sys.triggers where name = 'insert_eventclass') 
DROP TRIGGER insert_eventclass;
IF EXISTS 
(SELECT * from sys.triggers where name = 'update_eventclass') 
DROP TRIGGER update_eventclass;

IF EXISTS 
(SELECT * from sys.triggers where name = 'insert_event') 
DROP TRIGGER insert_event;
IF EXISTS 
(SELECT * from sys.triggers where name = 'update_event') 
DROP TRIGGER update_event;

IF EXISTS 
(SELECT * from sys.triggers where name = 'insert_passing') 
DROP TRIGGER insert_passing;
IF EXISTS 
(SELECT * from sys.triggers where name = 'update_passing') 
DROP TRIGGER update_passing;

IF EXISTS 
(SELECT * from sys.triggers where name = 'insert_session') 
DROP TRIGGER insert_session;
IF EXISTS 
(SELECT * from sys.triggers where name = 'update_session') 
DROP TRIGGER update_session;
