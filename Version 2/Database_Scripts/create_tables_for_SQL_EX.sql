--Create TABLES with foreign keys order
/* 02/09/2012 Eugene
   Drop 12 Tables
   command string to execute from C:\Program Files\Microsoft Visual Studio 10.0\VC>
   sqlcmd -S .\sqlexpress -d Hardcard  -i E:\user_June\OSU_Roger\source\Database_Scripts\create_tables_for_SQL_EX.sql

   History:
   06/06/2012  ponomare TagList and ValidTag tables - removed from database schema
						Passing table - removed FK for SessionId field and SessionId field can be NULL
						 

*/

--Table: CompetitionNo

--DROP TABLE CompetitionNo;

CREATE TABLE CompetitionNo (
  CompetitionNo       nvarchar(10) PRIMARY KEY,
  CompetitionNoValue  float NOT NULL
);

--Table: RaceClass

--DROP TABLE RaceClass;

CREATE TABLE RaceClass (
  Id            bigint PRIMARY KEY NOT NULL,
  ClassName     nvarchar(10) NOT NULL,
  MinAge        tinyint,
  MaxAge        tinyint,
  Gender        char(1),
  VehicleType   nvarchar(20),
  VehicleModel  nvarchar(20),
  VehicleCC     integer,
  Deleted       bit,
  LastUpdated   datetime
);


--Table: Event

--DROP TABLE Event;

CREATE TABLE Event (
  Id             bigint PRIMARY KEY,
  EventName      nvarchar(75) NOT NULL,
  EventLocation  nvarchar(75),
  StartDate      datetime,
  EndDate        datetime,
  Deleted        bit,
  LastUpdated    datetime
);


--Table: Athlete

--CREATE TABLE Athlete;

CREATE TABLE Athlete (
  Id            bigint PRIMARY KEY NOT NULL,
  FirstName     nvarchar(20) NOT NULL,
  LastName      nvarchar(20) NOT NULL,
  DOB           datetime ,
  Gender        char(1) ,
  AddressLine1  nvarchar(50),
  AddressLine2  nvarchar(50),
  AddressLine3  nvarchar(50),
  City          nvarchar(30),
  State         nvarchar(30),
  PostalCode    nvarchar(20),
  Country       nvarchar(20),
  Phone         nvarchar(20),
  LastUpdated   datetime,
);

--Table: EventClass

--CREATE TABLE EventClass;

CREATE TABLE EventClass (
  Id            bigint PRIMARY KEY NOT NULL,
  EventId       bigint NOT NULL,
  ClassId       bigint NOT NULL,
  LastUpdated   datetime,
  /* Foreign keys */
  FOREIGN KEY (ClassId)
    REFERENCES RaceClass(Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
	 
  FOREIGN KEY (EventId)
    REFERENCES Event(Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

--Table: Competitor

--CREATE TABLE Competitor;

CREATE TABLE Competitor (
  Id            bigint PRIMARY KEY NOT NULL,
  AthleteId     bigint NOT NULL,
  EventClassId  bigint NOT NULL,
  VehicleType   nvarchar(20),
  VehicleModel  nvarchar(20),
  VehicleCC     integer,
  Deleted       bit NOT NULL,
  LastUpdated   datetime,
  /* Foreign keys */
  FOREIGN KEY (EventClassId)
    REFERENCES EventClass(Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  FOREIGN KEY (AthleteId)
    REFERENCES Athlete(Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);


--Table: Session

--DROP TABLE Session;

CREATE TABLE Session (
  Id             bigint PRIMARY KEY,
  EventClassId   bigint NOT NULL,
  StartTime      datetime,
  SchedStopTime  datetime,
  SchedLaps      int,
  RollingStart   bit,
  LastUpdated    datetime,
  /* Foreign keys */
  FOREIGN KEY (EventClassId)
    REFERENCES EventClass(Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);


--Table: Penality

--DROP TABLE Penality;

CREATE TABLE Penality (
  PenalityID       int PRIMARY KEY,
  SessionID        bigint NOT NULL,
  RFID             bigint NOT NULL,
  TimePenality     bigint,
  LapPenality      tinyint,
  PostionPenality  tinyint,
  Reason           nvarchar(250),
  LastUpdated      datetime,
  /* Foreign keys */
  FOREIGN KEY (SessionId)
    REFERENCES Session(Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);


--Table: Passing

--DROP TABLE Passing;

CREATE TABLE Passing (
  Id           bigint PRIMARY KEY NOT NULL,
  SessionId    bigint NULL,
  RFID         bigint NOT NULL,
  PassingTime  datetime NOT NULL,
  RaceTime     bigint NOT NULL,
  FlagState    tinyint,
  Deleted      bit,
  Invalid      bit,
  LastUpdated  datetime,
  /* Foreign keys */
  FOREIGN KEY (SessionId)
    REFERENCES Session(Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);


--Table: Entry

--DROP TABLE Entry;

CREATE TABLE Entry (
  Id             bigint PRIMARY KEY,
  SessionId      bigint NOT NULL,
  CompetitorId   bigint NOT NULL,
  CompetitionNo  nvarchar(10) NOT NULL,
  RFID           bigint NOT NULL,
  Equipment      nvarchar(50),
  Sponsors       nvarchar(300),
  Status         nvarchar(1),
  EntryDate      datetime NOT NULL,
  LastUpdated    datetime,
  /* Foreign keys */
  FOREIGN KEY (CompetitorId)
    REFERENCES Competitor(Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION, 
  FOREIGN KEY (SessionId)
    REFERENCES Session(Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);


--Table: Synchronization

--DROP TABLE Synchronization;

CREATE TABLE Synchronization (
  Id    int PRIMARY KEY,
  SyncDate  datetime NOT NULL
);

--Table: Standing 
--This table is updated constantly during the session (race). 
--It provides statistics for the competitor like ‘Number of laps’ passed, 
--‘Best Lap’ time interval, etc. during the session.

--DROP TABLE Standing ;

CREATE TABLE Standing  (
  Id             bigint PRIMARY KEY,
  EntryId        bigint NOT NULL,
  Position       smallint, /* Current position of the Competitor in the session */
  LapsCompleted  smallint, /* Current number of race laps were completed by Competitor*/
  CompletedTime  bigint,   /* Time interval when the current last race lap was completed */
  BestLapTime    bigint,   
  AvgLapTime     bigint,
  WorstLapTime   bigint,
  PassingTime    bigint,   /* ??? */
  /* Foreign keys */
  FOREIGN KEY (EntryId)
    REFERENCES Entry(Id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);







