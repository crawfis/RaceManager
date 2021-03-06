--DROP TABLES with foreign keys order

/* 02/09/2012 Eugene
   Drop 13 Tables
   command string to execute from C:\Program Files\Microsoft Visual Studio 10.0\VC>
   sqlcmd -S .\sqlexpress -d Hardcard  -i E:\user_June\OSU_Roger\source\Database_Scripts\drop_tables_for_SQL_EX.sql
*/

-- Tables without dependences

IF EXISTS 
(SELECT * from sys.tables where name = 'Standing') 
DROP TABLE Standing;

IF EXISTS 
(SELECT * from sys.tables where name = 'Synchronization') 
DROP TABLE Synchronization;

IF EXISTS 
(SELECT * from sys.tables where name = 'CompetitionNo') 
DROP TABLE CompetitionNo;

-- Tables with references for forieng keys
IF EXISTS 
(SELECT * from sys.tables where name = 'Entry') 
DROP TABLE Entry;

IF EXISTS 
(SELECT * from sys.tables where name = 'Passing') 
DROP TABLE Passing;

IF EXISTS 
(SELECT * from sys.tables where name = 'Penality') 
DROP TABLE Penality;

IF EXISTS 
(SELECT * from sys.tables where name = 'Session') 
DROP TABLE Session;

IF EXISTS 
(SELECT * from sys.tables where name = 'Competitor') 
DROP TABLE Competitor;

IF EXISTS 
(SELECT * from sys.tables where name = 'EventClass') 
DROP TABLE EventClass;

IF EXISTS 
(SELECT * from sys.tables where name = 'Athlete') 
DROP TABLE Athlete;

IF EXISTS 
(SELECT * from sys.tables where name = 'RaceClass') 
DROP TABLE RaceClass;

IF EXISTS 
(SELECT * from sys.tables where name = 'Event') 
DROP TABLE Event;

IF EXISTS 
(SELECT * from sys.tables where name = 'TagList') 
DROP TABLE TagList;

IF EXISTS 
(SELECT * from sys.tables where name = 'ValidTag') 
DROP TABLE ValidTag;


