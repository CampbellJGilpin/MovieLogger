-- Create EventTypeReference table
CREATE TABLE "EventTypeReferences" (
    "Id" SERIAL PRIMARY KEY,
    "EventType" INTEGER NOT NULL,
    "Name" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(500),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "UQ_EventTypeReferences_EventType" UNIQUE ("EventType")
);

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

-- Create AuditLog table
CREATE TABLE "AuditLogs" (
    "Id" SERIAL PRIMARY KEY,
    "EventType" INTEGER NOT NULL,
    "UserId" VARCHAR(100) NOT NULL,
    "Description" VARCHAR(500) NOT NULL,
    "Timestamp" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "AdditionalData" VARCHAR(1000),
    "EntityType" INTEGER,
    "EntityId" INTEGER,
    CONSTRAINT "FK_AuditLogs_EventTypeReferences_EventType" 
        FOREIGN KEY ("EventType") REFERENCES "EventTypeReferences" ("EventType") ON DELETE RESTRICT,
    CONSTRAINT "FK_AuditLogs_EntityTypeReferences_EntityType" 
        FOREIGN KEY ("EntityType") REFERENCES "EntityTypeReferences" ("EntityType") ON DELETE RESTRICT
);

-- Insert seed data for EventTypeReference
INSERT INTO "EventTypeReferences" ("EventType", "Name", "Description", "IsActive", "CreatedAt") VALUES
(1, 'Movie Added', 'A new movie was added to the system', TRUE, CURRENT_TIMESTAMP),
(2, 'Movie Updated', 'An existing movie was updated', TRUE, CURRENT_TIMESTAMP),
(3, 'Movie Deleted', 'A movie was deleted from the system', TRUE, CURRENT_TIMESTAMP),
(4, 'Movie Favorited', 'A movie was marked as favorite or unfavorited', TRUE, CURRENT_TIMESTAMP),
(5, 'Movie Added to Library', 'A movie was added to or removed from user library', TRUE, CURRENT_TIMESTAMP),
(6, 'Review Added', 'A new review was added', TRUE, CURRENT_TIMESTAMP),
(7, 'Review Updated', 'An existing review was updated', TRUE, CURRENT_TIMESTAMP),
(8, 'Review Deleted', 'A review was deleted', TRUE, CURRENT_TIMESTAMP),
(9, 'User Registered', 'A new user registered', TRUE, CURRENT_TIMESTAMP),
(10, 'User Logged In', 'A user logged into the system', TRUE, CURRENT_TIMESTAMP),
(11, 'User Profile Updated', 'A user updated their profile', TRUE, CURRENT_TIMESTAMP);

-- Insert seed data for EntityTypeReference
INSERT INTO "EntityTypeReferences" ("EntityType", "Name", "Description", "IsActive", "CreatedAt") VALUES
(1, 'Movie', 'Movie entities in the system', TRUE, CURRENT_TIMESTAMP),
(2, 'Review', 'Review entities in the system', TRUE, CURRENT_TIMESTAMP),
(3, 'User', 'User entities in the system', TRUE, CURRENT_TIMESTAMP),
(4, 'Genre', 'Genre entities in the system', TRUE, CURRENT_TIMESTAMP),
(5, 'Viewing', 'Viewing entities in the system', TRUE, CURRENT_TIMESTAMP),
(6, 'LibraryItem', 'Library item entities in the system', TRUE, CURRENT_TIMESTAMP); 