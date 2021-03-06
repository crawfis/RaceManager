/* 04/09/2012 Eugene
   This file was created based on create_records_for_SQL_EX.sql file. Only three initial tables:Event, RaceClass, and Athlete are
   populated by this script.

   command string to populate to sqlexpress database:
   sqlcmd -S .\sqlexpress -d Hardcard  -i E:\user_June\OSU_Roger\source\Database_Scripts\create_initial_tables_for_SQL_EX.sql


*/


BEGIN TRANSACTION;
INSERT INTO ValidTag (RFID, Valid, LastUpdated) VALUES ('1000', '1', NULL);
INSERT INTO ValidTag (RFID, Valid, LastUpdated) VALUES ('1001', '1', NULL);
COMMIT;

BEGIN TRANSACTION;
INSERT INTO TagList (RFID, CompId, LastUpdated) VALUES ('1000', 1, NULL);
INSERT INTO TagList (RFID, CompId, LastUpdated) VALUES ('1001', 2, NULL);
COMMIT;

BEGIN TRANSACTION;
INSERT INTO Event (Id, EventName, EventLocation, StartDate, EndDate, Deleted, LastUpdated) VALUES (1, 'Event1', ' Dublin, OH',    '2011-01-08', '2011-01-08',  0, '2011-11-30 16:12:19');
INSERT INTO Event (Id, EventName, EventLocation, StartDate, EndDate, Deleted, LastUpdated) VALUES (2, 'Event2', ' Washington DC', '2012-12-08', '2012-12-08' , 0, '2011-11-30 16:12:50');
INSERT INTO Event (Id, EventName, EventLocation, StartDate, EndDate, Deleted, LastUpdated) VALUES (3, 'Event3', ' Columbus, OH',  '2013-09-08', '2013-12-08',  1, '2011-11-30 16:12:50');
INSERT INTO Event (Id, EventName, EventLocation, StartDate, EndDate, Deleted, LastUpdated) VALUES (4, 'Event4', ' Columbus, OH',  '2014-12-08', '2014-12-08' , 0, '2011-11-30 16:12:50');
INSERT INTO Event (Id, EventName, EventLocation, StartDate, EndDate, Deleted, LastUpdated) VALUES (5, 'Event5', ' London, UK',    '2014-12-08', '2014-12-08' , 0, '2011-11-30 16:12:50');
COMMIT;


BEGIN TRANSACTION;
INSERT INTO RaceClass (Id, ClassName, MinAge, MaxAge, Gender, VehicleType, VehicleModel, VehicleCC, Deleted, LastUpdated) VALUES (101, '500A Ford', 31, 50, 'M', 'Car', 'Ford', 500, '0', '2011-11-02 23:01:10');
INSERT INTO RaceClass (Id, ClassName, MinAge, MaxAge, Gender, VehicleType, VehicleModel, VehicleCC, Deleted, LastUpdated) VALUES (102, '500B Ford', 31, 50, 'F', 'Car', 'Ford', 500, '0', '2011-11-02 23:01:10');
INSERT INTO RaceClass (Id, ClassName, MinAge, MaxAge, Gender, VehicleType, VehicleModel, VehicleCC, Deleted, LastUpdated) VALUES (103, '500A GMC',  11, 15, 'M', 'Car', 'GMC',  500, '0', '2011-11-02 23:01:10');
INSERT INTO RaceClass (Id, ClassName, MinAge, MaxAge, Gender, VehicleType, VehicleModel, VehicleCC, Deleted, LastUpdated) VALUES (104, '500B GMC',  21, 50, 'F', 'Car', 'GMC',  300, '0', '2011-11-02 23:01:10');
INSERT INTO RaceClass (Id, ClassName, MinAge, MaxAge, Gender, VehicleType, VehicleModel, VehicleCC, Deleted, LastUpdated) VALUES (105, '500B GMC',  21, 90, 'M', 'Car', 'GMC',  300, '0', '2011-11-02 23:01:10');
INSERT INTO RaceClass (Id, ClassName, MinAge, MaxAge, Gender, VehicleType, VehicleModel, VehicleCC, Deleted, LastUpdated) VALUES (110, '500B GMC',  21, 90, 'F', 'Car', 'GMC',  300, '0', '2011-11-02 23:01:10');
COMMIT;


BEGIN TRANSACTION; 
INSERT INTO Athlete (Id, FirstName, LastName, DOB, Gender, AddressLine1, AddressLine2, AddressLine3, City, State, PostalCode, Country, Phone, LastUpdated) VALUES (1, 'First1', 'Last1', '1991-03-15 00:00:00', 'M', '1111 Riverside Dr.', '',              NULL, 'Dublin',        'OH', '12345', 'USA', '123-123-1234',       '2011-12-07 19:20:37');
INSERT INTO Athlete (Id, FirstName, LastName, DOB, Gender, AddressLine1, AddressLine2, AddressLine3, City, State, PostalCode, Country, Phone, LastUpdated) VALUES (2, 'First2', 'Last2', '1992-07-16 00:00:00', 'M', '2222 Riverside Dr.', 'Address Line2', NULL, 'Columbus',      'OH', '12345', 'USA', '123-123-1234',       '2011-12-06 18:16:25');
INSERT INTO Athlete (Id, FirstName, LastName, DOB, Gender, AddressLine1, AddressLine2, AddressLine3, City, State, PostalCode, Country, Phone, LastUpdated) VALUES (3, 'First3', 'Last3', '1993-01-17 00:00:00', 'F', '3333 Riverside Dr.', '',              NULL, 'Washington DC', '  ', '43235', 'USA', '614-208-5632',       '2011-12-07 19:21:38');
INSERT INTO Athlete (Id, FirstName, LastName, DOB, Gender, AddressLine1, AddressLine2, AddressLine3, City, State, PostalCode, Country, Phone, LastUpdated) VALUES (4, 'First4', 'Last4', '1994-01-18 00:00:00', 'M', '4444 Riverside Dr.', 'Address Line2', NULL, 'London',        'UK', '43235', 'UK',  '011-5-123-456-7890', '2011-12-08 14:50:29');
INSERT INTO Athlete (Id, FirstName, LastName, DOB, Gender, AddressLine1, AddressLine2, AddressLine3, City, State, PostalCode, Country, Phone, LastUpdated) VALUES (5, 'First5', 'Last5', '1995-11-19 00:00:00', 'M', '5555 Riverside Dr.', '',              NULL, 'Dublin',        'OH', '12345', 'USA', '123-123-1234',       '2011-12-07 19:20:37');
INSERT INTO Athlete (Id, FirstName, LastName, DOB, Gender, AddressLine1, AddressLine2, AddressLine3, City, State, PostalCode, Country, Phone, LastUpdated) VALUES (6, 'First6', 'Last6', '1996-10-20 00:00:00', 'M', '6666 Riverside Dr.', 'Address Line2', NULL, 'Columbus',      'OH', '12345', 'USA', '123-123-1234',       '2011-12-06 18:16:25');
INSERT INTO Athlete (Id, FirstName, LastName, DOB, Gender, AddressLine1, AddressLine2, AddressLine3, City, State, PostalCode, Country, Phone, LastUpdated) VALUES (7, 'First7', 'Last7', '1997-07-21 00:00:00', 'F', '7777 Riverside Dr.', '',              NULL, 'Seattle', '  ', '43235', 'USA', '614-208-5632',       '2011-12-07 19:21:38');
INSERT INTO Athlete (Id, FirstName, LastName, DOB, Gender, AddressLine1, AddressLine2, AddressLine3, City, State, PostalCode, Country, Phone, LastUpdated) VALUES (8, 'First7', 'Last7', '1998-05-22 00:00:00', 'M', '8888 Riverside Dr.', 'Address Line2', NULL, 'London',        'UK', '43235', 'UK',  '011-5-123-456-7890', '2011-12-08 14:50:29');
COMMIT;

