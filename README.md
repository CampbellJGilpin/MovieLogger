# MovieLogger

## Overview of Application

MovieLogger is a simple, user-friendly app to help movie buffs keep track of everything they've watched or want to watch. Users can log films, rate them, write reviews, add a movie to their list of favourites, and manage their personal watchlist.

## Problem Definition

Manually keeping track of movies watched, reviews written, and films to watch can quickly become messy and inconsistent. MovieLogger addresses this problem by providing an organized, digital solution for logging your movie experiences and maintaining a watchlist.

## Requirements Prioritised

### Must Have
- Ability to add movies to the database
- Ability to log movies (includes rating, review, and favourite toggle)
- Ability to edit movies (includes rating, review, and favourite toggle)
- Authentication

### Should Have
- Basic search functionality for adding movies
- Filter logged movies (by watch date, rating, or title)
- Ability to add genres and assign them to movies

### Could Have
- Tagging system (eg. "Guilty Pleasure", "Oscar Winner", etc)
- Integration with streaming platforms to check availability

### Will Not Have
- Social Features (eg. following other users)
- Mobile version
- Offline mode

## Domain Model Diagram

```mermaid
erDiagram
    USERS ||--o{ MOVIES : adds
    USERS ||--o{ REVIEWS : writes
    USERS ||--o{ RATINGS : gives
    USERS ||--o{ FAVOURITES : saves

    MOVIES ||--o{ REVIEWS : receives
    MOVIES ||--o{ RATINGS : receives
    MOVIES ||--o{ FAVOURITES : appears_in
```

### Glossary

- **Admins**: A special type of user with elevated permissions. Admins can manage users and moderate content.

- **Users**: An individual who uses the MovieLogger app. Users can log movies, rate them, write reviews, and add favourites. All users must authenticate to use core features.

- **Movies**: A film entry in the database, containing title, description, release date, and genre. Users interact with movies by logging, rating, reviewing, or favouriting them.

- **Reviews**: A written review that a user attaches to a logged movie. Each review is associated with one movie and one user.

- **Ratings**: A numerical score (e.g., 1–10) that a user assigns to a movie they've logged. Each movie can have multiple ratings, but each user can rate a movie only once.

- **Favourites**: A toggle which a user can apply to a movie to mark it as a personal favourite. 

### Entity Relationship Diagram

```mermaid
erDiagram
    Users ||--o{ Reviews : writes
    Users ||--o{ Ratings : gives
    Users ||--o{ Favourites : saves
    Users ||--o{ MovieLogs : logs

    Movies ||--o{ Reviews : receives
    Movies ||--o{ Ratings : receives
    Movies ||--o{ Favourites : appears_in
    Movies ||--o{ MovieLogs : is_logged_in

    Genres ||--o{ Movies : has

    Users {
        int Id
        string UserName
        string Email
        string Password
        boolean IsAdmin
    }

    Movies {
        int Id
        string Title
        string Description
        datetime ReleaseDate
        int FK_Movies_Id_Genres_Id
    }

   Genres {
        int Id
        string Title
    }

    Reviews {
        int Id
        int FK_Reviews_Id_Users_Id
        int FK_Reviews_Id_MovieLogs_Id
        string ReviewText
        datetime DateCreated
    }

    Ratings {
        int Id
        int FK_Ratings_Id_Users_Id
        int FK_Ratings_Id_MovieLogs_Id
        int Score
    }

    Favourites {
        int Id
        int FK_Favourites_Id_Users_Id
        int FK_Favourites_Id_Movies_Id
        datetime DateAdded
    }

    MovieLogs { 
        int Id
        int FK_MovieLogs_Id_Users_Id
        int FK_MovieLogs_Id_Movies_Id
        datetime DateWatched
    }
```