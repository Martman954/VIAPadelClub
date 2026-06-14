PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Players (
    PlayerId TEXT NOT NULL PRIMARY KEY,
    FullName TEXT NOT NULL,
    Email TEXT NOT NULL UNIQUE,
    ProfileImageUrl TEXT NULL,
    VipUntil TEXT NULL
);

CREATE TABLE IF NOT EXISTS Schedules (
    ScheduleId TEXT NOT NULL PRIMARY KEY,
    ScheduleDate TEXT NOT NULL UNIQUE,
    Status TEXT NOT NULL CHECK (Status IN ('Draft', 'Active', 'Deleted'))
);

CREATE TABLE IF NOT EXISTS Courts (
    CourtId TEXT NOT NULL PRIMARY KEY,
    CourtLabel TEXT NOT NULL,
    CourtType TEXT NOT NULL CHECK (CourtType IN ('Single', 'Double'))
);

CREATE TABLE IF NOT EXISTS ScheduleCourts (
    ScheduleId TEXT NOT NULL,
    CourtId TEXT NOT NULL,
    PRIMARY KEY (ScheduleId, CourtId),
    FOREIGN KEY (ScheduleId) REFERENCES Schedules(ScheduleId) ON DELETE CASCADE,
    FOREIGN KEY (CourtId) REFERENCES Courts(CourtId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS TimeSlots (
    TimeSlotId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    ScheduleId TEXT NOT NULL,
    CourtId TEXT NOT NULL,
    StartTime TEXT NOT NULL,
    EndTime TEXT NOT NULL,
    IsVip INTEGER NOT NULL DEFAULT 0 CHECK (IsVip IN (0, 1)),
    FOREIGN KEY (ScheduleId) REFERENCES Schedules(ScheduleId) ON DELETE CASCADE,
    FOREIGN KEY (CourtId) REFERENCES Courts(CourtId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Bookings (
    BookingId TEXT NOT NULL PRIMARY KEY,
    ScheduleId TEXT NOT NULL,
    CourtId TEXT NOT NULL,
    PlayerId TEXT NOT NULL,
    StartTime TEXT NOT NULL,
    EndTime TEXT NOT NULL,
    IsCancelled INTEGER NOT NULL DEFAULT 0 CHECK (IsCancelled IN (0, 1)),
    CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ScheduleId) REFERENCES Schedules(ScheduleId) ON DELETE CASCADE,
    FOREIGN KEY (CourtId) REFERENCES Courts(CourtId) ON DELETE CASCADE,
    FOREIGN KEY (PlayerId) REFERENCES Players(PlayerId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS QueueEntries (
    QueueId TEXT NOT NULL PRIMARY KEY,
    PlayerId TEXT NOT NULL,
    ScheduleId TEXT NOT NULL,
    CourtId TEXT NOT NULL,
    RequestedAt TEXT NOT NULL,
    Status TEXT NOT NULL CHECK (Status IN ('Waiting', 'Notified', 'Expired', 'Booked')),
    FOREIGN KEY (PlayerId) REFERENCES Players(PlayerId) ON DELETE CASCADE,
    FOREIGN KEY (ScheduleId) REFERENCES Schedules(ScheduleId) ON DELETE CASCADE,
    FOREIGN KEY (CourtId) REFERENCES Courts(CourtId) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS IX_Schedules_ScheduleDate ON Schedules(ScheduleDate);
CREATE INDEX IF NOT EXISTS IX_Bookings_ScheduleCourtTime ON Bookings(ScheduleId, CourtId, StartTime);
CREATE INDEX IF NOT EXISTS IX_Bookings_PlayerTime ON Bookings(PlayerId, StartTime);
CREATE INDEX IF NOT EXISTS IX_TimeSlots_ScheduleCourtTime ON TimeSlots(ScheduleId, CourtId, StartTime);
CREATE INDEX IF NOT EXISTS IX_QueueEntries_PlayerStatus ON QueueEntries(PlayerId, Status);

-- Minimal seed data so the DB is immediately browsable.
INSERT INTO Players (PlayerId, FullName, Email, ProfileImageUrl, VipUntil)
VALUES
    ('p1', 'Troels Mortensen', 'trmo@via.dk', NULL, '2025-04-25'),
    ('p2', 'Anna Jensen', 'anna@via.dk', NULL, NULL),
    ('p3', 'Peter Nielsen', 'peter@via.dk', NULL, NULL)
ON CONFLICT(PlayerId) DO NOTHING;

INSERT INTO Courts (CourtId, CourtLabel, CourtType)
VALUES
    ('S1', 'Single 1', 'Single'),
    ('D4', 'Double 4', 'Double')
ON CONFLICT(CourtId) DO NOTHING;

INSERT INTO Schedules (ScheduleId, ScheduleDate, Status)
VALUES
    ('sch-2025-04-09', '2025-04-09', 'Active'),
    ('sch-2025-04-10', '2025-04-10', 'Active'),
    ('sch-2025-04-18', '2025-04-18', 'Draft'),
    ('sch-2025-04-23', '2025-04-23', 'Deleted')
ON CONFLICT(ScheduleId) DO NOTHING;

INSERT INTO ScheduleCourts (ScheduleId, CourtId)
VALUES
    ('sch-2025-04-09', 'S1'),
    ('sch-2025-04-09', 'D4')
ON CONFLICT(ScheduleId, CourtId) DO NOTHING;

INSERT INTO TimeSlots (ScheduleId, CourtId, StartTime, EndTime, IsVip)
VALUES
    ('sch-2025-04-09', 'S1', '2025-04-09 15:00:00', '2025-04-09 15:30:00', 0),
    ('sch-2025-04-09', 'S1', '2025-04-09 17:00:00', '2025-04-09 17:30:00', 1),
    ('sch-2025-04-09', 'D4', '2025-04-09 16:00:00', '2025-04-09 16:30:00', 0),
    ('sch-2025-04-09', 'D4', '2025-04-09 17:00:00', '2025-04-09 17:30:00', 1)
ON CONFLICT DO NOTHING;

INSERT INTO Bookings (BookingId, ScheduleId, CourtId, PlayerId, StartTime, EndTime, IsCancelled)
VALUES
    ('b1', 'sch-2025-04-09', 'S1', 'p1', '2025-04-09 15:30:00', '2025-04-09 16:00:00', 0),
    ('b2', 'sch-2025-04-09', 'S1', 'p3', '2025-04-09 17:00:00', '2025-04-09 18:00:00', 0),
    ('b3', 'sch-2025-04-09', 'D4', 'p2', '2025-04-09 16:00:00', '2025-04-09 18:00:00', 0)
ON CONFLICT(BookingId) DO NOTHING;

INSERT INTO QueueEntries (QueueId, PlayerId, ScheduleId, CourtId, RequestedAt, Status)
VALUES
    ('q1', 'p1', 'sch-2025-04-10', 'S1', '2025-04-08 12:00:00', 'Waiting')
ON CONFLICT(QueueId) DO NOTHING;

