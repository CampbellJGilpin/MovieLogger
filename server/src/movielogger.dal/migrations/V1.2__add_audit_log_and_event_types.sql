-- Create EventTypeReference table
CREATE TABLE public."EventTypeReferences" (
    "Id" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Description" character varying(200) NOT NULL,
    "Category" character varying(50) NOT NULL,
    "IsActive" boolean NOT NULL DEFAULT true,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "UpdatedAt" timestamp with time zone NULL,
    CONSTRAINT "PK_EventTypeReferences" PRIMARY KEY ("Id")
);

-- Create AuditLog table
CREATE TABLE public."AuditLogs" (
    "Id" SERIAL PRIMARY KEY,
    "EventType" integer NOT NULL,
    "UserId" character varying(100) NOT NULL,
    "Description" character varying(500) NOT NULL,
    "Timestamp" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc'),
    "AdditionalData" character varying(1000) NULL,
    "EntityType" character varying(50) NULL,
    "EntityId" integer NULL
);

-- Create indexes for AuditLog
CREATE INDEX "IX_AuditLogs_Timestamp" ON public."AuditLogs" ("Timestamp");
CREATE INDEX "IX_AuditLogs_EventType" ON public."AuditLogs" ("EventType");
CREATE INDEX "IX_AuditLogs_UserId" ON public."AuditLogs" ("UserId");

-- Add foreign key constraint
ALTER TABLE public."AuditLogs" 
ADD CONSTRAINT "FK_AuditLogs_EventTypeReferences_EventType" 
FOREIGN KEY ("EventType") REFERENCES public."EventTypeReferences" ("Id") ON DELETE RESTRICT;

-- Seed EventTypeReference data
INSERT INTO public."EventTypeReferences" ("Id", "Name", "Description", "Category", "IsActive", "CreatedAt") VALUES
(1, 'MovieAdded', 'A new movie was added to the system', 'Movie', true, (now() at time zone 'utc')),
(2, 'MovieUpdated', 'An existing movie was updated', 'Movie', true, (now() at time zone 'utc')),
(3, 'MovieDeleted', 'A movie was deleted from the system', 'Movie', true, (now() at time zone 'utc')),
(4, 'MovieFavorited', 'A movie was marked as favorite or unfavorited', 'Movie', true, (now() at time zone 'utc')),
(5, 'MovieAddedToLibrary', 'A movie was added to or removed from user library', 'Movie', true, (now() at time zone 'utc')),
(6, 'ReviewAdded', 'A new review was added to a movie', 'Review', true, (now() at time zone 'utc')),
(7, 'ReviewUpdated', 'An existing review was updated', 'Review', true, (now() at time zone 'utc')),
(8, 'ReviewDeleted', 'A review was deleted', 'Review', true, (now() at time zone 'utc')),
(9, 'UserRegistered', 'A new user registered in the system', 'User', true, (now() at time zone 'utc')),
(10, 'UserLoggedIn', 'A user logged into the system', 'User', true, (now() at time zone 'utc')),
(11, 'UserProfileUpdated', 'A user updated their profile information', 'User', true, (now() at time zone 'utc')); 