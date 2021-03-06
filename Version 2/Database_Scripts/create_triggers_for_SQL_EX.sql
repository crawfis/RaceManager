/* 02/09/2012 Eugene
   All these 'create' statements was generated by Sqlite tool by "SQL Generatot" for each table.
   12 Tables, 14 triggers
   command string:
   sqlcmd -S .\sqlexpress -d Hardcard  -i E:\user_June\OSU_Roger\source\Database_Scripts\create_triggers_for_SQL_EX.sql
   
*/

CREATE TRIGGER insert_athlete
  ON Athlete
  FOR INSERT, UPDATE
AS 
BEGIN
UPDATE Athlete	
SET LastUpdated = getutcdate()
WHERE Id in (SELECT i.Id from inserted i);
END;
GO

CREATE TRIGGER insert_competitor
  ON Competitor
  FOR INSERT, UPDATE
AS 
BEGIN
UPDATE Competitor	
SET LastUpdated = getutcdate()
WHERE Id in (SELECT i.Id from inserted i);
END;
GO

CREATE TRIGGER insert_entries
  ON Entry
  AFTER INSERT, UPDATE
AS
BEGIN
UPDATE Entry	
SET LastUpdated = getutcdate()
WHERE Id in (SELECT i.Id from inserted i);
END;
GO

CREATE TRIGGER insert_eventclass
  ON EventClass
  AFTER INSERT, UPDATE
AS
BEGIN
UPDATE EventClass
SET LastUpdated = getutcdate()
WHERE Id in (SELECT i.Id from inserted i);
END;
GO

CREATE TRIGGER insert_events
  ON Event
  AFTER INSERT,UPDATE
  AS
BEGIN
UPDATE Event
SET LastUpdated = getutcdate()
WHERE Id in (SELECT i.Id from inserted i);
END;
GO

CREATE TRIGGER insert_passing
  ON Passing
  FOR INSERT
AS
BEGIN
UPDATE Passing
SET LastUpdated = getutcdate()
WHERE Id in (SELECT i.Id from inserted i);
END;
GO

CREATE TRIGGER insert_raceclass
  ON RaceClass
  AFTER INSERT, UPDATE
AS
BEGIN
UPDATE RaceClass
SET LastUpdated = getutcdate()
WHERE Id in (SELECT i.Id from inserted i);
END;
GO

CREATE TRIGGER insert_session
  ON Session
  FOR INSERT
AS
BEGIN
UPDATE Session
SET LastUpdated = getutcdate()
WHERE Id in (SELECT i.Id from inserted i);
END;
GO

CREATE TRIGGER update_session
  ON Session
  AFTER UPDATE
AS
BEGIN
UPDATE Session
SET LastUpdated = getutcdate()
WHERE Id in (SELECT i.Id from inserted i);
END;
GO





