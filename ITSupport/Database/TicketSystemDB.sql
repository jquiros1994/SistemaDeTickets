-- ============================================================
--  Base de Datos del Sistema de Tickets
--  SQL Server 2019+
--  Orden de creacion respeta todas las FK
-- ============================================================

CREATE DATABASE TicketSystemDB


USE TicketSystemDB;
GO

-- ============================================================
--  1. ROLES
-- ============================================================
CREATE TABLE Roles (
    RoleId      INT           NOT NULL IDENTITY(1,1),
    RoleName    NVARCHAR(50)  NOT NULL,
    Description NVARCHAR(255) NULL,

    CONSTRAINT PK_Roles      PRIMARY KEY (RoleId),
    CONSTRAINT UQ_Roles_Name UNIQUE      (RoleName)
);
GO

INSERT INTO Roles (RoleName, Description) VALUES
    ('Admin',    'Full access to all features and settings'),
    ('Engineer', 'Support engineer, can manage assigned cases'),
    ('Client',   'Customer with access to their own cases');
GO

-- ============================================================
--  2. PROGRAMS  (support tiers)
-- ============================================================
CREATE TABLE Programs (
    ProgramId    INT           NOT NULL IDENTITY(1,1),
    ProgramName  NVARCHAR(100) NOT NULL,
    SupportLevel NVARCHAR(50)  NOT NULL,   -- e.g. Basic / Standard / Premium
    Description  NVARCHAR(500) NULL,

    CONSTRAINT PK_Programs      PRIMARY KEY (ProgramId),
    CONSTRAINT UQ_Programs_Name UNIQUE      (ProgramName)
);
GO

INSERT INTO Programs (ProgramName, SupportLevel, Description) VALUES
    ('Basic Support',   'Basic',   '8x5 support, email only'),
    ('Standard Support','Standard','8x5 support, email and phone'),
    ('Premium Support', 'Premium', '24x7 support, dedicated engineer');
GO

-- ============================================================
--  3. ENGINEER LEVELS
-- ============================================================
CREATE TABLE EngineerLevels (
    LevelId     INT           NOT NULL IDENTITY(1,1),
    LevelName   NVARCHAR(50)  NOT NULL,
    Description NVARCHAR(255) NULL,

    CONSTRAINT PK_EngineerLevels      PRIMARY KEY (LevelId),
    CONSTRAINT UQ_EngineerLevels_Name UNIQUE      (LevelName)
);
GO

INSERT INTO EngineerLevels (LevelName, Description) VALUES
    ('L1', 'First level support – basic troubleshooting'),
    ('L2', 'Second level support – advanced troubleshooting'),
    ('L3', 'Third level support – engineering and escalations');
GO

-- ============================================================
--  4. SCHEDULES
-- ============================================================
CREATE TABLE Schedules (
    ScheduleId   INT          NOT NULL IDENTITY(1,1),
    ScheduleName NVARCHAR(100) NOT NULL,
    TimeZone     NVARCHAR(100) NOT NULL,
    StartTime    TIME(0)       NOT NULL,
    EndTime      TIME(0)       NOT NULL,
    WorkDays     NVARCHAR(50)  NOT NULL,   -- e.g. 'Mon-Fri', 'Mon-Sun'

    CONSTRAINT PK_Schedules PRIMARY KEY (ScheduleId)
);
GO

INSERT INTO Schedules (ScheduleName, TimeZone, StartTime, EndTime, WorkDays) VALUES
    ('Business Hours EST', 'America/New_York',    '08:00', '17:00', 'Mon-Fri'),
    ('Business Hours CST', 'America/Chicago',     '08:00', '17:00', 'Mon-Fri'),
    ('24x7 Coverage',      'UTC',                 '00:00', '23:59', 'Mon-Sun');
GO

-- ============================================================
--  5. CASE STATUSES
-- ============================================================
CREATE TABLE CaseStatuses (
    StatusId    INT           NOT NULL IDENTITY(1,1),
    StatusName  NVARCHAR(50)  NOT NULL,
    Description NVARCHAR(255) NULL,
    IsOpen      BIT           NOT NULL CONSTRAINT DF_CaseStatuses_IsOpen DEFAULT 1,

    CONSTRAINT PK_CaseStatuses      PRIMARY KEY (StatusId),
    CONSTRAINT UQ_CaseStatuses_Name UNIQUE      (StatusName)
);
GO

INSERT INTO CaseStatuses (StatusName, Description, IsOpen) VALUES
    ('New',         'Case just opened, not yet assigned',      1),
    ('Assigned',    'Case assigned to a support engineer',     1),
    ('In Progress', 'Engineer is actively working on it',      1),
    ('Pending',     'Waiting for customer response',           1),
    ('Escalated',   'Case escalated to higher support level',  1),
    ('Resolved',    'Solution provided, pending confirmation', 1),
    ('Closed',      'Case closed and confirmed by customer',   0),
    ('Cancelled',   'Case cancelled',                          0);
GO

-- ============================================================
--  6. PRIORITIES
-- ============================================================
CREATE TABLE Priorities (
    PriorityId   INT          NOT NULL IDENTITY(1,1),
    PriorityName NVARCHAR(50) NOT NULL,
    SlaHours     INT          NOT NULL,   -- response SLA in hours
    Description  NVARCHAR(255) NULL,

    CONSTRAINT PK_Priorities      PRIMARY KEY (PriorityId),
    CONSTRAINT UQ_Priorities_Name UNIQUE      (PriorityName),
    CONSTRAINT CK_Priorities_Sla  CHECK (SlaHours > 0)
);
GO

INSERT INTO Priorities (PriorityName, SlaHours, Description) VALUES
    ('Critical', 1,  'System down, business impact'),
    ('High',     4,  'Major feature unavailable'),
    ('Medium',   8,  'Partial functionality affected'),
    ('Low',      24, 'Minor issue or question');
GO

-- ============================================================
--  7. CUSTOMERS
-- ============================================================
CREATE TABLE Customers (
    CustomerId  INT           NOT NULL IDENTITY(1,1),
    FirstName   NVARCHAR(100) NOT NULL,
    LastName    NVARCHAR(100) NOT NULL,
    Email       NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(30)  NULL,
    Country     NVARCHAR(100) NOT NULL,
    ProgramId   INT           NOT NULL,
    IsActive    BIT           NOT NULL CONSTRAINT DF_Customers_IsActive DEFAULT 1,
    CreatedAt   DATETIME2(0)  NOT NULL CONSTRAINT DF_Customers_CreatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Customers       PRIMARY KEY (CustomerId),
    CONSTRAINT UQ_Customers_Email UNIQUE      (Email),
    CONSTRAINT FK_Customers_Programs
        FOREIGN KEY (ProgramId) REFERENCES Programs(ProgramId)
);
GO

-- ============================================================
--  8. CONTACTS  (personas dentro de un Customer)
-- ============================================================
CREATE TABLE Contacts (
    ContactId    INT           NOT NULL IDENTITY(1,1),
    CustomerId   INT           NOT NULL,
    ContactName  NVARCHAR(200) NOT NULL,
    ContactEmail NVARCHAR(255) NULL,
    ContactPhone NVARCHAR(30)  NULL,
    Role         NVARCHAR(100) NULL,   -- e.g. IT Manager, Developer, CEO
    IsPrimary    BIT           NOT NULL CONSTRAINT DF_Contacts_IsPrimary DEFAULT 0,

    CONSTRAINT PK_Contacts PRIMARY KEY (ContactId),
    CONSTRAINT FK_Contacts_Customers
        FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId)
);
GO

-- ============================================================
--  9. SUPPORT ENGINEERS
-- ============================================================
CREATE TABLE SupportEngineers (
    EngineerId  INT           NOT NULL IDENTITY(1,1),
    Name        NVARCHAR(200) NOT NULL,
    Email       NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(30)  NULL,
    LevelId     INT           NOT NULL,
    ScheduleId  INT           NOT NULL,
    IsActive    BIT           NOT NULL CONSTRAINT DF_Engineers_IsActive DEFAULT 1,

    CONSTRAINT PK_SupportEngineers       PRIMARY KEY (EngineerId),
    CONSTRAINT UQ_SupportEngineers_Email UNIQUE      (Email),
    CONSTRAINT FK_Engineers_Levels
        FOREIGN KEY (LevelId)    REFERENCES EngineerLevels(LevelId),
    CONSTRAINT FK_Engineers_Schedules
        FOREIGN KEY (ScheduleId) REFERENCES Schedules(ScheduleId)
);
GO

-- ============================================================
--  10. USERS  (login accounts)
-- ============================================================
CREATE TABLE Users (
    UserId       INT           NOT NULL IDENTITY(1,1),
    Username     NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(255) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    RoleId       INT           NOT NULL,
    CustomerId   INT           NULL,   -- populated for Client role
    EngineerId   INT           NULL,   -- populated for Engineer / Admin role
    IsActive     BIT           NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
    CreatedAt    DATETIME2(0)  NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
    LastLoginAt  DATETIME2(0)  NULL,

    CONSTRAINT PK_Users           PRIMARY KEY (UserId),
    CONSTRAINT UQ_Users_Username  UNIQUE      (Username),
    CONSTRAINT UQ_Users_Email     UNIQUE      (Email),
    CONSTRAINT FK_Users_Roles
        FOREIGN KEY (RoleId)      REFERENCES Roles(RoleId),
    CONSTRAINT FK_Users_Customers
        FOREIGN KEY (CustomerId)  REFERENCES Customers(CustomerId),
    CONSTRAINT FK_Users_Engineers
        FOREIGN KEY (EngineerId)  REFERENCES SupportEngineers(EngineerId)
);
GO


USE TicketSystemDB;
GO

INSERT INTO Users (Username, Email, PasswordHash, RoleId, CustomerId, EngineerId, IsActive)
VALUES (
    'Super.Admin',
    'Super.admin@support.com',
    'admin',  -- hash de "admin"
    1,        -- RoleId 1 = Admin
    NULL,
    NULL,
    1
);
GO

SELECT * FROM USERS ;

-- ============================================================
--  11. CASES
-- ============================================================
CREATE TABLE Cases (
    CaseId           INT            NOT NULL IDENTITY(1,1),
    CaseNumber       AS ('CASE-' + RIGHT('000000' + CAST(CaseId AS VARCHAR), 6)) PERSISTED,
    StatusId         INT            NOT NULL,
    PriorityId       INT            NOT NULL,
    Title            NVARCHAR(255)  NOT NULL,
    Description      NVARCHAR(MAX)  NOT NULL,
    ContactId        INT            NOT NULL,
    ProgramId        INT            NOT NULL,
    Country          NVARCHAR(100)  NOT NULL,
    OwnerId          INT            NULL,   -- assigned engineer (NULL = unassigned)
    CreatedByUserId  INT            NOT NULL,
    PreferredContact NVARCHAR(50)   NULL    -- 'Email', 'Phone', 'Portal'
                     CONSTRAINT CK_Cases_PreferredContact
                         CHECK (PreferredContact IN ('Email','Phone','Portal')),
    CreatedAt        DATETIME2(0)   NOT NULL CONSTRAINT DF_Cases_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt        DATETIME2(0)   NOT NULL CONSTRAINT DF_Cases_UpdatedAt DEFAULT SYSUTCDATETIME(),
    ClosedAt         DATETIME2(0)   NULL,

    CONSTRAINT PK_Cases PRIMARY KEY (CaseId),
    CONSTRAINT FK_Cases_Statuses
        FOREIGN KEY (StatusId)        REFERENCES CaseStatuses(StatusId),
    CONSTRAINT FK_Cases_Priorities
        FOREIGN KEY (PriorityId)      REFERENCES Priorities(PriorityId),
    CONSTRAINT FK_Cases_Contacts
        FOREIGN KEY (ContactId)       REFERENCES Contacts(ContactId),
    CONSTRAINT FK_Cases_Programs
        FOREIGN KEY (ProgramId)       REFERENCES Programs(ProgramId),
    CONSTRAINT FK_Cases_Owner
        FOREIGN KEY (OwnerId)         REFERENCES SupportEngineers(EngineerId),
    CONSTRAINT FK_Cases_CreatedBy
        FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId)
);
GO

-- ============================================================
--  12. CASE NOTES  (comentarios / actualizaciones)
-- ============================================================
CREATE TABLE CaseNotes (
    NoteId     INT           NOT NULL IDENTITY(1,1),
    CaseId     INT           NOT NULL,
    UserId     INT           NOT NULL,
    NoteText   NVARCHAR(MAX) NOT NULL,
    IsInternal BIT           NOT NULL CONSTRAINT DF_CaseNotes_IsInternal DEFAULT 0,
    CreatedAt  DATETIME2(0)  NOT NULL CONSTRAINT DF_CaseNotes_CreatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_CaseNotes PRIMARY KEY (NoteId),
    CONSTRAINT FK_CaseNotes_Cases
        FOREIGN KEY (CaseId)  REFERENCES Cases(CaseId),
    CONSTRAINT FK_CaseNotes_Users
        FOREIGN KEY (UserId)  REFERENCES Users(UserId)
);
GO

-- ============================================================
--  13. CASE HISTORY  (audit trail)
-- ============================================================
CREATE TABLE CaseHistory (
    HistoryId       INT           NOT NULL IDENTITY(1,1),
    CaseId          INT           NOT NULL,
    ChangedByUserId INT           NOT NULL,
    FieldChanged    NVARCHAR(100) NOT NULL,
    OldValue        NVARCHAR(500) NULL,
    NewValue        NVARCHAR(500) NULL,
    ChangedAt       DATETIME2(0)  NOT NULL CONSTRAINT DF_CaseHistory_ChangedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_CaseHistory PRIMARY KEY (HistoryId),
    CONSTRAINT FK_CaseHistory_Cases
        FOREIGN KEY (CaseId)          REFERENCES Cases(CaseId),
    CONSTRAINT FK_CaseHistory_Users
        FOREIGN KEY (ChangedByUserId) REFERENCES Users(UserId)
);
GO

-- ============================================================
--  14. ATTACHMENTS
-- ============================================================
CREATE TABLE Attachments (
    AttachmentId     INT           NOT NULL IDENTITY(1,1),
    CaseId           INT           NOT NULL,
    FileName         NVARCHAR(255) NOT NULL,
    FilePath         NVARCHAR(500) NOT NULL,
    FileSize         BIGINT        NULL,   -- bytes
    ContentType      NVARCHAR(100) NULL,   -- MIME type
    UploadedByUserId INT           NOT NULL,
    UploadedAt       DATETIME2(0)  NOT NULL CONSTRAINT DF_Attachments_UploadedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Attachments PRIMARY KEY (AttachmentId),
    CONSTRAINT FK_Attachments_Cases
        FOREIGN KEY (CaseId)           REFERENCES Cases(CaseId),
    CONSTRAINT FK_Attachments_Users
        FOREIGN KEY (UploadedByUserId) REFERENCES Users(UserId)
);
GO

-- ============================================================
--  INDEXES  (query performance)
-- ============================================================
CREATE INDEX IX_Cases_StatusId    ON Cases(StatusId);
CREATE INDEX IX_Cases_PriorityId  ON Cases(PriorityId);
CREATE INDEX IX_Cases_OwnerId     ON Cases(OwnerId);
CREATE INDEX IX_Cases_ContactId   ON Cases(ContactId);
CREATE INDEX IX_Cases_CreatedAt   ON Cases(CreatedAt DESC);
CREATE INDEX IX_CaseNotes_CaseId  ON CaseNotes(CaseId);
CREATE INDEX IX_CaseHistory_CaseId ON CaseHistory(CaseId);
CREATE INDEX IX_Attachments_CaseId ON Attachments(CaseId);
CREATE INDEX IX_Users_RoleId       ON Users(RoleId);
GO

-- ============================================================
--  TRIGGER – auto-update Cases.UpdatedAt on every change
-- ============================================================
CREATE OR ALTER TRIGGER TR_Cases_UpdatedAt
ON Cases
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Cases
    SET    UpdatedAt = SYSUTCDATETIME()
    FROM   Cases c
    INNER JOIN inserted i ON c.CaseId = i.CaseId;
END;
GO

-- ============================================================
--  TRIGGER – auto-close timestamp when status = Closed/Cancelled
-- ============================================================
CREATE OR ALTER TRIGGER TR_Cases_ClosedAt
ON Cases
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Cases
    SET    ClosedAt = SYSUTCDATETIME()
    FROM   Cases c
    INNER JOIN inserted i ON c.CaseId = i.CaseId
    INNER JOIN CaseStatuses cs ON cs.StatusId = i.StatusId
    WHERE  cs.IsOpen = 0 AND c.ClosedAt IS NULL;
END;
GO

-- ============================================================
--  SAMPLE DATA
-- ============================================================

-- Sample customers
INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber, Country, ProgramId) VALUES
    ('Maria',   'Gonzalez', 'maria.gonzalez@acme.com',   '+506 8888-1111', 'Costa Rica', 3),
    ('John',    'Smith',    'john.smith@globex.com',      '+1 555-222-3333','United States', 2),
    ('Lucia',   'Fernandez','lucia.fernandez@initec.com', '+34 91 555 0101','Spain', 1);
GO

-- Sample contacts
INSERT INTO Contacts (CustomerId, ContactName, ContactEmail, ContactPhone, Role, IsPrimary) VALUES
    (1, 'Maria Gonzalez',   'maria.gonzalez@acme.com',    '+506 8888-1111', 'IT Manager',  1),
    (1, 'Carlos Mora',      'carlos.mora@acme.com',        '+506 8888-2222', 'Developer',   0),
    (2, 'John Smith',       'john.smith@globex.com',       '+1 555-222-3333','System Admin',1),
    (3, 'Lucia Fernandez',  'lucia.fernandez@initec.com',  '+34 91 555 0101','CEO',         1);
GO

-- Sample engineers
INSERT INTO SupportEngineers (Name, Email, PhoneNumber, LevelId, ScheduleId) VALUES
    ('Alex Rivera',  'alex.rivera@support.com',  '+1 555-001-0001', 1, 1),
    ('Sara Kim',     'sara.kim@support.com',      '+1 555-001-0002', 2, 2),
    ('Pedro Vargas', 'pedro.vargas@support.com',  '+1 555-001-0003', 3, 3);
GO

-- Sample users (passwords must be hashed by the app; placeholder here)
INSERT INTO Users (Username, Email, PasswordHash, RoleId, EngineerId) VALUES
    ('admin',       'admin@support.com',         'HASH_PLACEHOLDER', 1, NULL),
    ('alex.rivera', 'alex.rivera@support.com',   'HASH_PLACEHOLDER', 2, 1),
    ('sara.kim',    'sara.kim@support.com',       'HASH_PLACEHOLDER', 2, 2),
    ('pedro.vargas','pedro.vargas@support.com',   'HASH_PLACEHOLDER', 2, 3);

INSERT INTO Users (Username, Email, PasswordHash, RoleId, CustomerId) VALUES
    ('maria.gonzalez','maria.gonzalez@acme.com',   'HASH_PLACEHOLDER', 3, 1),
    ('john.smith',    'john.smith@globex.com',      'HASH_PLACEHOLDER', 3, 2),
    ('lucia.fernandez','lucia.fernandez@initec.com','HASH_PLACEHOLDER', 3, 3);
GO

-- Sample cases
INSERT INTO Cases (StatusId, PriorityId, Title, Description, ContactId, ProgramId, Country, OwnerId, CreatedByUserId, PreferredContact) VALUES
    (1, 1, 'Cannot log in to portal',
        'Users at our company cannot log in since this morning. Error 503 displayed.',
        1, 3, 'Costa Rica', NULL, 5, 'Email'),
    (3, 2, 'Report export fails for large datasets',
        'When exporting reports with more than 10,000 rows the download never completes.',
        3, 2, 'United States', 2, 6, 'Phone'),
    (2, 4, 'How to reset 2FA?',
        'One of our users lost access to their authenticator app.',
        4, 1, 'Spain', 1, 7, 'Portal');
GO

PRINT 'TicketSystemDB created successfully.';
GO
