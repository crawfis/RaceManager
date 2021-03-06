-- Delete data from tables for one event with foreign keys order
-- Doesn't delete data from Event, RaceClass and Athlete tables
-- command to execute from C:\Program Files\Microsoft Visual Studio 10.0\VC>
-- sqlcmd -S .\sqlexpress -d Hardcard  -i E:\user_June\OSU_Roger\source\Database_Scripts\delete_event_tables_for_SQL_EX.sql


-- Tables without dependences

IF EXISTS 
(SELECT * from sys.tables where name = 'Standing') 
DELETE FROM Standing;

IF EXISTS 
(SELECT * from sys.tables where name = 'Synchronization') 
DELETE FROM Synchronization;

IF EXISTS 
(SELECT * from sys.tables where name = 'CompetitionNo') 
DELETE FROM CompetitionNo;

-- Tables with references for forieng keys
IF EXISTS 
(SELECT * from sys.tables where name = 'Entry') 
DELETE FROM Entry;

IF EXISTS 
(SELECT * from sys.tables where name = 'Passing') 
DELETE FROM Passing;

IF EXISTS 
(SELECT * from sys.tables where name = 'Penality') 
DELETE FROM Penality;

IF EXISTS 
(SELECT * from sys.tables where name = 'Session') 
DELETE FROM Session;

IF EXISTS 
(SELECT * from sys.tables where name = 'Competitor') 
DELETE FROM Competitor;

IF EXISTS 
(SELECT * from sys.tables where name = 'EventClass') 
DELETE FROM EventClass;

IF EXISTS 
(SELECT * from sys.tables where name = 'TagList') 
DELETE FROM TagList;

IF EXISTS 
(SELECT * from sys.tables where name = 'ValidTag') 
DELETE FROM ValidTag;


