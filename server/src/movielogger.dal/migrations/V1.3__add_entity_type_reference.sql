-- Create EntityTypeReference table
CREATE TABLE "EntityTypeReferences" (
    "Id" SERIAL PRIMARY KEY,
    "EntityType" INTEGER NOT NULL,
    "Name" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(500),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "UQ_EntityTypeReferences_EntityType" UNIQUE ("EntityType")
);

-- Insert seed data for EntityTypeReference
INSERT INTO "EntityTypeReferences" ("EntityType", "Name", "Description", "IsActive", "CreatedAt") VALUES
(1, 'Movie', 'Movie entities in the system', TRUE, CURRENT_TIMESTAMP),
(2, 'Review', 'Review entities in the system', TRUE, CURRENT_TIMESTAMP),
(3, 'User', 'User entities in the system', TRUE, CURRENT_TIMESTAMP),
(4, 'Genre', 'Genre entities in the system', TRUE, CURRENT_TIMESTAMP),
(5, 'Viewing', 'Viewing entities in the system', TRUE, CURRENT_TIMESTAMP),
(6, 'LibraryItem', 'Library item entities in the system', TRUE, CURRENT_TIMESTAMP);

-- Update AuditLog table to use EntityType enum
-- First, add the new EntityType column
ALTER TABLE "AuditLogs" ADD COLUMN "EntityTypeNew" INTEGER;

-- Update existing records to set EntityType based on the string value
UPDATE "AuditLogs" 
SET "EntityTypeNew" = CASE 
    WHEN "EntityType" = 'Movie' THEN 1
    WHEN "EntityType" = 'Review' THEN 2
    WHEN "EntityType" = 'User' THEN 3
    WHEN "EntityType" = 'Genre' THEN 4
    WHEN "EntityType" = 'Viewing' THEN 5
    WHEN "EntityType" = 'LibraryItem' THEN 6
    ELSE NULL
END;

-- Drop the old string column and rename the new one
ALTER TABLE "AuditLogs" DROP COLUMN "EntityType";
ALTER TABLE "AuditLogs" RENAME COLUMN "EntityTypeNew" TO "EntityType";

-- Add foreign key constraint
ALTER TABLE "AuditLogs" 
ADD CONSTRAINT "FK_AuditLogs_EntityTypeReferences_EntityType" 
FOREIGN KEY ("EntityType") REFERENCES "EntityTypeReferences" ("EntityType") ON DELETE RESTRICT; 