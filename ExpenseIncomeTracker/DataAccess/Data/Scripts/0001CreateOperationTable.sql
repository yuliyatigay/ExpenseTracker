CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE TABLE Operations
(
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    date DATE not null,
    Amount DECIMAL(18,2) NOT NULL,
    Description TEXT,
    CategoryId UUID 
);