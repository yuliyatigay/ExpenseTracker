CREATE TABLE Category
(
    Id uuid PRIMARY KEY DEFAULT uuid_generate_v4(),
    Name TEXT not null ,
    CategoryTypes INT default 0,
    CONSTRAINT fk_operations_category
    FOREIGN KEY (categoryId) REFERENCES Category(Id)
);
CREATE INDEX IF NOT EXISTS idx_operations_categoryId ON public.operations (categoryid);