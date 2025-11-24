CREATE TABLE Operations
(
    Id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    date DATE not null,
    Amount DECIMAL(18,2) NOT NULL,
    Description TEXT,
    CategoryId UUID,
    CONSTRAINT fk_operations_category
    FOREIGN KEY (categoryId) REFERENCES Category(Id)
);

CREATE INDEX IF NOT EXISTS idx_operations_categoryId ON public.operations (categoryid);