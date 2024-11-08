-- Table definitions
-- SPORT
CREATE TABLE Sport (
	SportId INTEGER PRIMARY KEY,
	SportName VARCHAR(255) NOT NULL,
	SportNotes VARCHAR(255)
);
-- insert values into Sport
INSERT INTO Sport (SportId, SportName, SportNotes)
VALUES (NULL, 'Walking', '1h/225kcal'), (NULL, 'Swimming', '1h/449kcal'), (NULL, 'Gym', '1h/670kcal');

-- SLEEP QUALITY
CREATE TABLE SleepQuality (
	SleepQualityId INTEGER PRIMARY KEY,
	SleepQualityName VARCHAR(255) NOT NULL
);
-- insert values into SleepQuality
INSERT INTO SleepQuality (SleepQualityId, SleepQualityName)
VALUES (NULL, 'Bad'), (NULL, 'Cannot fall asleep'), (NULL, 'Good');

-- DRUG
CREATE TABLE Drug (
	DrugId INTEGER PRIMARY KEY,
	DrugName VARCHAR(255) NOT NULL,
	DrugNotes VARCHAR(255)
);
-- insert values into Drug
INSERT INTO Drug (DrugId, DrugName, DrugNotes)
VALUES (NULL, 'Topiramato', 'Migraña');

-- SLEEP
CREATE TABLE Sleep (
	SleepId INTEGER PRIMARY KEY,
	SleepDate DATE NOT NULL,
	BedTime TIME NOT NULL,
	SleepTime TIME NOT NULL,
	NumberOfAwakenings INT NOT NULL,
	WakeTime TIME NOT NULL,
	SportId INT NOT NULL,
	SleepQualityId INT NOT NULL,
	DrugId INT,
	SleepNotes VARCHAR(255),
	FOREIGN KEY(SportId)
		REFERENCES Sport(SportId),
	FOREIGN KEY(SleepQualityId)
		REFERENCES SleepQuality(SleepQualityId),
	FOREIGN KEY(DrugId)
		REFERENCES Drug(DrugId)
);