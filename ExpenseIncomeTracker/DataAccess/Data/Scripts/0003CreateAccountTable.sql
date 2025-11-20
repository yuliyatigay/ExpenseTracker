CREATE TABLE Accounts
(
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    UserName TEXT UNIQUE,
    FirstName TEXT ,
    Role TEXT,
    LastName TEXT ,
    PasswordHash TEXT
);