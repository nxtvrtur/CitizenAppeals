USE CitizenAppeals;
GO

CREATE TABLE Citizens (
	Id INT PRIMARY KEY IDENTITY(1, 1),
	LastName NVARCHAR(100) NOT NULL,
	FirstName NVARCHAR(100) NOT NULL,
	MiddleName NVARCHAR(100)
);

CREATE TABLE Executors (
	Id INT PRIMARY KEY IDENTITY(1, 1),
	LastName NVARCHAR(100) NOT NULL,
	FirstName NVARCHAR(100) NOT NULL,
	MiddleName NVARCHAR(100)
);

CREATE TABLE Appeals (
	Id INT PRIMARY KEY IDENTITY(1, 1),
	AppealNumber NVARCHAR(50) NOT NULL,
	AppealDate DATE NOT NULL,
	CitizenId INT FOREIGN KEY REFERENCES Citizens(Id),
	ViolationType INT NOT NULL,
	Result NVARCHAR(20) NOT NULL CHECK (Result IN (N'Выявлено', N'Не выявлено')),
    AppealLink NVARCHAR(200)
);

CREATE TABLE AppealExecutors (
	AppealId INT FOREIGN KEY REFERENCES Appeals(Id),
	ExecutorId INT FOREIGN KEY REFERENCES Executors(Id),
	PRIMARY KEY (AppealId, ExecutorId)
)

CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    RoleId INT NOT NULL FOREIGN KEY REFERENCES Roles(Id)
);

INSERT INTO Citizens (LastName, FirstName, MiddleName) VALUES
(N'Губайдуллин', N'Артур', N'Ильдарович'),
(N'Губайдуллин', N'Арсен', N'Ильдарович');

INSERT INTO Executors (LastName, FirstName, MiddleName) VALUES
(N'Нечаев', N'Артем', N'Алексеевич'),
(N'Аюпов', N'Иван', N'Викторович');

INSERT INTO Appeals (AppealNumber, AppealDate, CitizenId, ViolationType, Result, AppealLink) VALUES
(N'А234', '2025-01-10', 1, 1, N'Не выявлено', N'https://t.me/arturr4evr'),
(N'A312', '2025-02-15', 2, 2, N'Выявлено', N'https://t.me/nft/LolPop-273230');

INSERT INTO AppealExecutors (AppealId, ExecutorId) VALUES
(1, 1),
(1, 2),
(2, 1);

INSERT INTO Roles (Name) VALUES
(N'Admin'),
(N'User');

INSERT INTO Users (Username, PasswordHash, RoleId) VALUES
(N'admin', N'8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918', 1),
(N'user', N'04F8996DA763B7A969B1028EE3007569EAF3A635486DDAB211D512C85B9DF8FB', 2);
