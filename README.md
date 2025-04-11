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


```mermaid
erDiagram
    USERS ||--o{ REVIEWS : writes
    USERS ||--o{ RATINGS : gives
    USERS ||--o{ FAVOURITES : saves
    USERS ||--o{ MOVIE_LOGS : logs

    MOVIES ||--o{ REVIEWS : receives
    MOVIES ||--o{ RATINGS : receives
    MOVIES ||--o{ FAVOURITES : appears_in
    MOVIES ||--o{ MOVIE_LOGS : is_logged_in

    USERS {
        int UserId
        string UserName
        string Email
        string Password
        boolean IsAdmin
    }

    MOVIES {
        int MovieId
        string Title
        string Description
        date ReleaseDate
        string Genre
    }

    REVIEWS {
        int ReviewId
        int FK_Users_Reviews_UserId_ReviewId
        int FK_Movies_Reviews_MovieId_ReviewId
        string ReviewText
        date DateCreated
    }

    RATINGS {
        int RatingId
        int FK_Users_Ratings_UserId_RatingId
        int FK_Movies_Ratings_MovieId_RatingId
        int Score
    }

    FAVOURITES {
        int FavouriteId
        int FK_Users_Favourites_UserId_FavouriteId
        int FK_Movies_Favourites_MovieId_FavouriteId
        date DateAdded
    }

    MOVIE_LOGS {
        int FK_Users_UserId
        int FK_Movies_MovieId
        date DateWatched
    }
```