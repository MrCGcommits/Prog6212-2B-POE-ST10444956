-- Create Database
CREATE DATABASE RaceDayDB;
GO
USE RaceDayDB;
GO

-- 1. Users Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Email VARCHAR(255) NOT NULL UNIQUE, -- UK as per ERD
    PasswordHash VARCHAR(255) NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Role VARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 2. Events Table
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    OrganiserId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    EventName VARCHAR(200) NOT NULL,
    EventDate DATE NOT NULL, -- Matches ERD 'date'
    Location VARCHAR(255) NOT NULL,
    Description VARCHAR(MAX) -- TEXT equivalent in SQL Server
);

-- 3. Categories Table
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL FOREIGN KEY REFERENCES Events(EventId),
    CategoryName VARCHAR(100) NOT NULL,
    DistanceKm DECIMAL(5,2) NOT NULL, -- Matches ERD 'decimal'
    MaxParticipants INT DEFAULT 500
);

-- 4. Enrollments Table
CREATE TABLE Enrollments (
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantId INT NOT NULL FOREIGN KEY REFERENCES Users(UserId),
    EventId INT NOT NULL FOREIGN KEY REFERENCES Events(EventId),
    CategoryId INT NOT NULL FOREIGN KEY REFERENCES Categories(CategoryId),
    EnrollmentDate DATETIME DEFAULT GETDATE(),
    Status VARCHAR(20) DEFAULT 'Confirmed'
);

-- 5. Results Table
CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Enrollments(EnrollmentId),
    FinishTime TIME NOT NULL, -- Matches ERD 'time'
    OverallPosition INT,
    CategoryPosition INT
);

-- 6. RouteInfo Table
CREATE TABLE RouteInfo (
    RouteInfoId INT IDENTITY(1,1) PRIMARY KEY,
    EventId INT NOT NULL UNIQUE FOREIGN KEY REFERENCES Events(EventId),
    RouteMapUrl VARCHAR(500),
    ElevationGain DECIMAL(6,2), -- Matches ERD 'decimal'
    LiveWeatherConditions VARCHAR(255)
);

-- ==========================================
-- SEED DATA (INSERT STATEMENTS)
-- ==========================================

-- Insert Users (2 Organisers, 2 Participants)
INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role) VALUES 
('organiser1@raceday.co.za', 'hashedpass123', 'John', 'Doe', 'Organiser'),
('organiser2@raceday.co.za', 'hashedpass456', 'Jane', 'Smith', 'Organiser'),
('runner1@gmail.com', 'hashedpass789', 'Thabo', 'Mokoena', 'Participant'),
('runner2@gmail.com', 'hashedpass101', 'Sarah', 'van der Merwe', 'Participant');

-- Insert Events (3 Events)
INSERT INTO Events (OrganiserId, EventName, EventDate, Location, Description) VALUES 
(1, 'Comrades Marathon', '2026-06-14', 'Pietermaritzburg to Durban', 'The ultimate human race.'),
(1, 'Cape Town Cycle Tour', '2026-03-08', 'Cape Town', 'The worlds largest timed cycle race.'),
(2, 'Soweto Marathon', '2026-11-01', 'Soweto, Johannesburg', 'A vibrant road running experience.');

-- Insert Categories
INSERT INTO Categories (EventId, CategoryName, DistanceKm, MaxParticipants) VALUES 
(1, 'Ultra Marathon', 89.0, 20000),
(1, 'Half Marathon', 21.1, 5000),
(2, 'Elite Cycle', 109.0, 30000),
(2, 'Fun Ride', 42.0, 5000),
(3, 'Full Marathon', 42.2, 15000),
(3, '10km Run', 10.0, 10000);

-- Insert Enrollments (Participants enrolling in events)
INSERT INTO Enrollments (ParticipantId, EventId, CategoryId, Status) VALUES 
(3, 1, 1, 'Confirmed'), -- Thabo in Comrades Ultra
(4, 1, 2, 'Confirmed'), -- Sarah in Comrades Half
(3, 3, 5, 'Confirmed'); -- Thabo in Soweto Full Marathon

-- Insert Results
INSERT INTO Results (EnrollmentId, FinishTime, OverallPosition, CategoryPosition) VALUES 
(1, '11:30:00', 4500, 3200);

-- Insert RouteInfo
INSERT INTO RouteInfo (EventId, RouteMapUrl, ElevationGain, LiveWeatherConditions) VALUES 
(1, 'https://maps.comrades.com/route', 1500.00, 'Clear skies, 12°C'),
(2, 'https://maps.cycletour.com/route', 800.00, 'Windy, 18°C'),
(3, 'https://maps.soweto.com/route', 600.00, 'Sunny, 22°C');