# MovieLogger

## Overview of Application

MovieLogger is a simple, user-friendly app to help movie buffs keep track of everything they've watched or want to watch. Users can log films, rate them, write reviews, add a movie to their list of favourites, and manage their personal watchlist.

## Problem Definition

Manually keeping track of movies watched, reviews written, and films to watch can quickly become messy and inconsistent. MovieLogger addresses this problem by providing an organized, digital solution for logging your movie experiences and maintaining a watchlist.

## Requirements Prioritised

### Must Have
- Ability to add movies to the database
- Ability to add a movie as logged against a user
- Ability to rate movies that have been logged (1-10 scale)
- Ability to add written reviews to movies that have been logged
- Ability to add movies to a users favourites
- Authentication

### Should Have
- Basic search functionality for adding movies
- Filter logged movies by watch date, rating, or title

### Could Have
- Genre tagging system (eg. Action, Drama)

### Will Not Have
- Social Features (eg. following other users)
- Mobile version

## Domain Model Diagram

```mermaid
erDiagram
    USER ||--o{ MOVIES_LOGGED : logs
    USER ||--o{ MOVIE_RATINGS : rates
    USER ||--o{ MOVIE_REVIEWS : writes
    USER ||--o{ MOVIE_FAVOURITES : favourites

    MOVIE ||--o{ MOVIES_LOGGED : is_logged
    MOVIE ||--o{ MOVIE_RATINGS : is_rated
    MOVIE ||--o{ MOVIE_REVIEWS : is_reviewed
    MOVIE ||--o{ MOVIE_FAVOURITES : is_favourited

