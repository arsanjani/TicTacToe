-- Migration Script: Add BoardSize Column to Games Table
-- Date: 2025-01-20
-- Description: Add BoardSize column to support 3x3 and 4x4 board sizes

BEGIN TRANSACTION;

-- Add the BoardSize column with default value of 3 (for existing 3x3 games)
ALTER TABLE Games 
ADD BoardSize int NOT NULL DEFAULT 3;

-- Create index on BoardSize for better query performance
CREATE NONCLUSTERED INDEX IX_Games_BoardSize 
ON Games (BoardSize);

-- Optional: Add a check constraint to ensure only valid board sizes are allowed
ALTER TABLE Games 
ADD CONSTRAINT CK_Games_BoardSize_Valid 
CHECK (BoardSize IN (3, 4));

-- Verify the changes
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE, 
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Games' 
AND COLUMN_NAME = 'BoardSize';

COMMIT TRANSACTION;

-- Rollback script (uncomment to undo changes):
/*
BEGIN TRANSACTION;

-- Remove the check constraint
ALTER TABLE Games DROP CONSTRAINT CK_Games_BoardSize_Valid;

-- Remove the index
DROP INDEX IX_Games_BoardSize ON Games;

-- Remove the column
ALTER TABLE Games DROP COLUMN BoardSize;

COMMIT TRANSACTION;
*/ 