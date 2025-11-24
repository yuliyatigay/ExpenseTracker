CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE TABLE Category
(
    Id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name TEXT not null ,
    CategoryTypes INT default 0
);
