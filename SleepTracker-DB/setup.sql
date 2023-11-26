USE master;
GO

-- Create database
CREATE DATABASE SleepTracker;
GO

USE SleepTracker;
GO

-- Table definitions
CREATE TABLE Sport (
	SportID INT IDENTITY(1,1) PRIMARY KEY,
	SportName VARCHAR(255) NOT NULL,
	SportNotes VARCHAR(255)
);
GO
-- insert values into Sport
INSERT INTO Sport (SportName, SportNotes)
VALUES ('Walking', ''), ('Swimming', ''), ('Gym', '');

CREATE TABLE SleepQuality (
	SleepQualityID INT IDENTITY(1,1) PRIMARY KEY,
	SleepQualityName VARCHAR(255) NOT NULL
);
GO
-- insert values into SleepQuality
INSERT INTO SleepQuality (SleepQualityName)
VALUES ('Bad'), ('Cannot fall asleep'), ('Good');


CREATE TABLE Sleep (
	SleepID INT IDENTITY(1,1) PRIMARY KEY,
	SleepDate DATE NOT NULL,
	BedTime TIME NOT NULL,
	SleepTime TIME NOT NULL,
	NumberOfAwakenings INT NOT NULL,
	WakeTime TIME NOT NULL,
	SportID INT NOT NULL,
	SleepQualityID INT NOT NULL,
	SleepNotes VARCHAR(255)
);
GO

-- FK constraint between Sleep and Sport
ALTER TABLE Sleep
ADD CONSTRAINT FK_Sleep_Sport
FOREIGN KEY (SportID)
REFERENCES Sport(SportID);
GO

-- FK constraint between Sleep and SleepQuality
ALTER TABLE Sleep
ADD CONSTRAINT FK_Sleep_SleepQuality
FOREIGN KEY (SleepQualityID)
REFERENCES SleepQuality(SleepQualityID);
GO