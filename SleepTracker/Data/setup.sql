-- Table definitions
-- SPORT
CREATE TABLE Sport (
	SportID INTEGER PRIMARY KEY,
	SportName VARCHAR(255) NOT NULL,
	SportNotes VARCHAR(255)
);
-- insert values into Sport
INSERT INTO Sport (SportID, SportName, SportNotes)
VALUES (NULL, 'Walking', ''), (NULL, 'Swimming', ''), (NULL, 'Gym', '');

-- SLEEP QUALITY
CREATE TABLE SleepQuality (
	SleepQualityID INTEGER PRIMARY KEY,
	SleepQualityName VARCHAR(255) NOT NULL
);
-- insert values into SleepQuality
INSERT INTO SleepQuality (SleepQualityID, SleepQualityName)
VALUES (NULL, 'Bad'), (NULL, 'Cannot fall asleep'), (NULL, 'Good');

-- SLEEP
CREATE TABLE Sleep (
	SleepID INTEGER PRIMARY KEY,
	SleepDate DATE NOT NULL,
	BedTime TIME NOT NULL,
	SleepTime TIME NOT NULL,
	NumberOfAwakenings INT NOT NULL,
	WakeTime TIME NOT NULL,
	SportID INT NOT NULL,
	SleepQualityID INT NOT NULL,
	SleepNotes VARCHAR(255),
	FOREIGN KEY(SportID)
		REFERENCES Sport(SportID),
	FOREIGN KEY(SleepQualityID)
		REFERENCES SleepQuality(SleepQualityID)
);