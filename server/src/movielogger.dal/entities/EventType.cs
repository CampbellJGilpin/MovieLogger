namespace movielogger.dal.entities
{
    public enum EventType
    {
        MovieAdded = 1,
        MovieUpdated = 2,
        MovieDeleted = 3,
        MovieFavorited = 4,
        MovieAddedToLibrary = 5,
        ReviewAdded = 6,
        ReviewUpdated = 7,
        ReviewDeleted = 8,
        UserRegistered = 9,
        UserLoggedIn = 10,
        UserProfileUpdated = 11
    }

    public enum EntityType
    {
        Movie = 1,
        Review = 2,
        User = 3,
        Genre = 4,
        Viewing = 5,
        LibraryItem = 6
    }
}