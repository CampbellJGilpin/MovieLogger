# MovieLogger

## Overview of Application

MovieLogger is a simple, user-friendly app to help movie buffs keep track of everything they've watched or want to watch. Users can log films, rate them, write reviews, add a movie to their list of favourites, and manage what they want to watch.

The application will keep a history of each time a user has watched a movie, along with any review and rating given to the film at that time.

## Problem Definition

Manually keeping track of movies watched, reviews written, and films to watch can quickly become messy and inconsistent. MovieLogger addresses this problem by providing an organized, digital solution for logging your movie experiences and maintaining a watchlist.

## Requirements Prioritised

### Must Have
- Ability to add movies to the database
- Ability to add genres to the database
- Ability to assign a genre to a movie
- Ability to log movies (includes rating, review, etc)
- Ability to edit movies (includes rating, review, etc)
- Ability to toggle movies as owned
- Ability to toggle movies as a favourite
- Authentication

### Should Have
- Basic search functionality for adding movies
- Filter logged movies (by watch date, rating, or title)

### Could Have
- Ability for users to add custom tags
- Ability for users to assign custom tags to movies
- Integration with streaming platforms to check streaming availability

### Will Not Have
- Social Features (eg. following other users)
- Mobile version
- Offline mode
- Ability for users to swap movies

## Domain Model Diagram

```mermaid
erDiagram
    USERS ||--o{ LIBRARY : tracks
    LIBRARY ||--o{ FAVOURITES : toggles
    LIBRARY ||--o{ OWNERSHIP : toggles
    LIBRARY ||--o{ WATCHLIST : schedules
    LIBRARY ||--o{ VIEWINGS : logs

    VIEWINGS ||--o{ REVIEWS : writes
    VIEWINGS ||--o{ RATINGS : gives

    MOVIES ||--o{ LIBRARY : appears_in
    MOVIES ||--o{ VIEWINGS : watched_in
```

### Glossary

- **Users**: An individual who uses the MovieLogger app. Users can log movies, rate them, write reviews, and add favourites. All users must authenticate to use core features.

- **Movies**: A film entry in the database, containing title, description, release date, and genre. Users interact with movies by logging, rating, reviewing, or favouriting them.

- **Viewings**: A snapshot of an invidual movie viewing. This holds the date the user viewed a specific movie, as well as an optional review and score associated with it.

- **Reviews**: A written user review that is associated with a viewing. There is one review per viewing, and a new review would require a new and unique viewing to be logged.

- **Ratings**: A numerical score (e.g., 1–10) that a user assigns to a movie they've logged. This is associated with a review.

- **Favourites**: A toggle which a user can apply to a movie to mark it as a personal favourite. This is independent from viewings and is tied directly to a movie.

- **Watchlist**: Movies that have a view date set.

- **Ownership**: A toggle that indicates if a user owns a specific movie. This is independent from viewings and is tied directly to a movie.

## API Structure

### Users

### `GET /users`

**Description:** Get all users.

**Responses:**
- `200 OK`
- `404 Not Found`

**Response Example:**
```json
[
  {
    "id": 1,
    "userName": "moviebuff88",
    "email": "buff@example.com",
    "isAdmin": false,
    "isDeleted": false
  },
  {
    "id": 2,
    "userName": "admin_user",
    "email": "admin@example.com",
    "isAdmin": true,
    "isDeleted": false
  }
]
```

### `GET /users/{id}`

**Description:** Retrieve a specific user by their id.

**Responses:**
- `200 OK`
- `404 Not Found`

**Response Example:**
```json
{
  "id": 1,
  "userName": "moviebuff88",
  "email": "buff@example.com",
  "isAdmin": false,
  "isDeleted": false
}
```

### `POST /users`

**Description:** Register a new user account.

**Request Example:**
```json
{
  "userName": "cinemafan123",
  "email": "cinema@example.com",
  "password": "supersecurepassword",
  "isAdmin": false,
}
```

**Responses:**
- `201 Created`
- `400 Bad Request`

**Response Example:**
```json
{
  "id": 3,
  "userName": "cinemafan123",
  "email": "cinema@example.com",
  "isAdmin": false,
  "isDeleted": false
}
```

### `POST /accounts`

**Description:** Authenticate a user and receive a token.

**Request Example:**
```json
{
  "email": "cinema@example.com",
  "password": "supersecurepassword"
}
```

**Responses:**
- `200 OK`
- `400 Bad Request`
- `401 Unauthorized`

**Response Example:**
```json
{
  "token": "ayJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
}
```

### `PUT /users/{id}`

**Description:** Update a user.

**Request Example:**
```json
{
  "userName": "cinemafan_updated",
  "email": "newemail@example.com",
  "isAdmin": false,
  "isDeleted": false
}
```

**Response Example:**
```json
{
  "id": 3,
  "userName": "cinemafan_updated",
  "email": "newemail@example.com",
  "isAdmin": false,
  "isDeleted": false
}
```

---

### Library

#### `GET /users/{userId}/library`

**Description:** Returns a user's movie library, including toggle states.

**Responses:**
- `200 OK`

**Response Example:**
```json
[
  {
    "movieId": 1,
    "title": "Sinners",
    "releaseDate": "2025-04-18",
    "genre": "Horror",
    "favourite": true,
    "inLibrary": true,
    "watchLater": false
  },
  {
    "movieId": 2,
    "title": "Mickey 17",
    "releaseDate": "2025-03-07",
    "genre": "Science Fiction",
    "favourite": false,
    "inLibrary": true,
    "watchLater": true
  }
]
```

### `POST /users/{userId}/library`

**Description:** Add a movie to the user's library.

**Request Body Example:**
```json
{
  "movieId": 2,
  "favourite": false,
  "ownsMovie": true,
  "upcomingViewDate": "2025-06-15"
}
```

**Responses:**
- `200 OK`
- `400 Bad Request`

**Response Example:**
```json
{
  "id": 14,
  "userId": 4,
  "movieId": 2,
  "favourite": false,
  "ownsMovie": true,
  "upcomingViewDate": "2025-06-15"
}
```

### `PUT /users/{userId}/library`

**Description:** Update a movie in the user's library.

**Request Body Example:**
```json
{
  "userMovieId": 12,
  "movieId": 2,
  "favourite": false,
  "ownsMovie": true,
  "upcomingViewDate": "2025-06-15"
}
```

**Responses:**
- `200 OK`
- `400 Bad Request`
- `404 Not Found`

**Response Example:**
```json
{
  "id": 14,
  "userId": 4,
  "movieId": 2,
  "favourite": false,
  "ownsMovie": true,
  "upcomingViewDate": "2025-06-15"
}
```

#### `GET /users/{userId}/library/favourites`

**Description:** Get all favourite movies for a user.

**Responses:**
- `200 OK`

**Response Example:**
```json
[
  {
    "movieId": 3,
    "title": "Pulp Fiction",
    "releaseDate": "1992-10-21",
    "genre": "Crime",
    "inLibrary": true
  },
  {
    "movieId": 4,
    "title": "Interstellar",
    "releaseDate": "2014-11-07",
    "genre": "Science Fiction",
    "inLibrary": false
  }
]
```

#### `GET /users/{userId}/library/watchlist`

**Description:** Get all movies on a user's watchlist (with upcoming view dates).

**Responses:**
- `200 OK`

**Response Example:**
```json
[
  {
    "movieId": 5,
    "title": "Blade Runner 2049",
    "releaseDate": "2017-10-06",
    "genre": "Science Fiction",
    "upcomingViewDate": "2025-05-10",
    "inLibrary": true
  },
  {
    "movieId": 6,
    "title": "The Batman",
    "releaseDate": "2022-03-04",
    "genre": "Action",
    "upcomingViewDate": "2025-06-01",
    "inLibrary": true
  }
]
```

---

### `PUT /users/{userId}/movies`

**Description:** Update flags or viewing info for a library entry.

**Request Body Example:**
```json
{
  "favourite": true,
  "ownsMovie": false,
  "upcomingViewDate": null
}
```

**Responses:**
- `200 OK`
- `400 Bad Request`
- `404 Not Found`

**Response Example:**
```json
{
  "id": 14,
  "userId": 4,
  "movieId": 5,
  "favourite": true,
  "ownsMovie": false,
  "upcomingViewDate": null
}
```

### `DELETE /users/{userId}/movies`

**Description:** Remove a movie from user's library.

**Responses:**
- `204 No Content`

---

### Movies

### `GET /movies`

**Description:** Retrieve all movies in the system.

**Responses:**
- `200 OK`

**Response Example:**
```json
[
  {
    "id": 1,
    "title": "Sinners",
    "description": "Trying to leave their troubled lives behind, twin brothers return to their hometown to start again, only to discover that an even greater evil is waiting to welcome them back.",
    "releaseDate": "2025-04-18",
    "genreId": 9,
    "isDeleted": false
  },
  {
    "id": 4,
    "title": "Mickey 17",
    "description": "A disposable employee is sent on a human expedition to colonize the ice world Niflheim. After one iteration dies, a new body is regenerated with most of his memories intact.",
    "releaseDate": "2025-03-07",
    "genreId": 1,
    "isDeleted": false
  }
]
```

### `GET /movies/{id}`

**Description:** Retrieve a single movie by its ID.

**Responses:**
- `200 OK`
- `404 Not Found`

**Response Example:**
```json
{
  "id": 1,
  "title": "Sinners",
  "description": "Trying to leave their troubled lives behind, twin brothers return to their hometown to start again, only to discover that an even greater evil is waiting to welcome them back.",
  "releaseDate": "2025-04-18",
  "genreId": 9,
  "isDeleted": false
}
```

### `POST /movies`

**Description:** Add a new movie to the system.

**Request Example:**
```json
{
  "title": "Companion",
  "description": "A weekend getaway with friends at a remote cabin turns into chaos after it's revealed that one of the guests is not what they seem.",
  "releaseDate": "2025-01-31",
  "genreId": 9
}
```

**Responses:**
- `201 Created`
- `400 Bad Request`

**Response Example:**
```json
{
  "id": 2,
  "title": "Companion",
  "description": "A weekend getaway with friends at a remote cabin turns into chaos after it's revealed that one of the guests is not what they seem.",
  "releaseDate": "2025-01-31",
  "genreId": 9,
  "isDeleted": false
}
```

### `PUT /movies/{id}`

**Description:** Update the details of an existing movie.

**Request Body Example:**
```json
{
  "title": "Pulp Fiction",
  "description": "In the realm of underworld, a series of incidents intertwines the lives of two Los Angeles mobsters, a gangster's wife, a boxer and two small-time criminals.",
  "releaseDate": "1992-10-21",
  "genreId": 9
}
```

**Responses:**
- `200 OK`
- `400 Bad Request`
- `404 Not Found`

**Response Example:**
```json
{
  "id": 3,
  "title": "Pulp Fiction",
  "description": "In the realm of underworld, a series of incidents intertwines the lives of two Los Angeles mobsters, a gangster's wife, a boxer and two small-time criminals.",
  "releaseDate": "1992-10-21",
  "genreId": 9,
  "isDeleted": false
}
```

### `DELETE /movies/{id}`

**Description:** Soft-delete a movie by marking it as deleted (`isDeleted = true`).

**Responses:**
- `204 No Content`
- `404 Not Found`

---

### Genres

### `GET /genres`

**Description:** Retrieve all genres in the system.

**Responses:**
- `200 OK`

**Response Example:**
```json
[
  { "id": 1, "title": "Action" },
  { "id": 2, "title": "Horror" },
  { "id": 3, "title": "Drama" }
]
```

### `GET /genres/{id}`

**Description:** Retrieve a specific genre by its ID.

**Responses:**
- `200 OK`
- `404 Not Found`

**Response Example:**
```json
{
  "id": 2,
  "title": "Horror"
}
```

### `POST /genres`

**Description:** Save a new genre.

**Request Example:**
```json
{
  "title": "Romance"
}
```

**Responses:**
- `200 OK`
- `400 Bad Request`
- `404 Not Found`

**Response Example:**
```json
{
  "id": 12,
  "title": "Romance"
}
```

### `PUT /genres/{id}`

**Description:** Update the title of an existing genre.

**Request Example:**
```json
{
  "title": "Supernatural Horror"
}
```

**Responses:**
- `200 OK`
- `400 Bad Request`
- `404 Not Found`

**Response Example:**
```json
{
  "id": 2,
  "title": "Supernatural Horror"
}
```

---

### Viewings

### `GET /users/{userId}/viewings`

**Description:** Retrieve all unique viewing records for a user.

**Responses:**
- `200 OK`
- `404 Not Found`

**Response Example:**
```json
[
  {
    "id": 12,
    "userId": 4,
    "movieId": 1,
    "favourite": true,
    "ownsMovie": false,
    "upcomingViewDate": null,
    "movie": {
      "id": 2,
      "title": "Companion",
      "description": "A weekend getaway with friends at a remote cabin turns into chaos after it's revealed that one of the guests is not what they seem.",
      "releaseDate": "2025-01-31",
      "genreId": 9
    }
  },
  {
    "id": 13,
    "userId": 4,
    "movieId": 3,
    "favourite": false,
    "ownsMovie": true,
    "upcomingViewDate": "2025-05-01",
    "movie": {
      "id": 3,
      "title": "Pulp Fiction",
      "description": "In the realm of underworld, a series of incidents intertwines the lives of two Los Angeles mobsters, a gangster's wife, a boxer and two small-time criminals.",
      "releaseDate": "1992-10-21",
      "genreId": 9
    }
  }
]
```

---

### Reviews

### `GET /users/{userId}/reviews`

**Description:** Get all reviews written by a user.

**Response Example:**
```json
[
  {
    "reviewId": 33,
    "movieTitle": "Inception",
    "dateViewed": "2025-04-25",
    "score": 9,
    "reviewText": "Still amazing!"
  }
]
```

### `POST /viewings/{id}/review`

**Description:** Create a review and score for an existing viewing.

**Request Body Example:**
```json
{
  "reviewText": "This movie rocks.",
  "score": 8
}
```

**Responses:**
- `201 Created`
- `400 Bad Request`
- `404 Not Found`

**Response Example:**
```json
{
  "id": 45,
  "viewingId": 13,
  "reviewText": "This movie rocks.",
  "score": 8
}
```

### `PUT /reviews/{id}`

**Description:** Update the text or score of an existing review.

**Request Body Example:**
```json
{
  "reviewText": "Actually I changed my mind. This movie really rocks!",
  "score": 10
}
```

**Response Example:**
```json
{
  "id": 17,
  "reviewText": "Actually I changed my mind. This movie really rocks!",
  "score": 10
}
```

**Responses:**
- `200 OK`
- `400 Bad Request`
- `404 Not Found`

## User Personas

### Casual User

- Wants to simply keep track of a watchlist, and what they've watched.
- Will occasionally write reviews.

### Collector

- Wants a solution to easily keep track and search the movies they've collected.
- Will reguarly add new movies to their owned list.

### Data-Oriented User

- Wants an easy solution to view statistics, such as how many movies they've watched or own.
- Wants to filter and sort through their viewing history.

## User Journeys

### Log Movie Viewing
Personas: Casual User, Data-Oriented User

```mermaid
flowchart TD
    A[User Signs Up] 
    --> B[User Visits Dashboard]
    B --> C[Search for Movie]
    C --> D{Movie Found?}
    D -- Yes --> E[Movie Page]
    E --> F[Log a Movie Viewing]
    D -- No --> G[Add Movie to System]
    G --> E
```
### Review a Movie
Personas: Casual User

```mermaid
flowchart TD
    A[User Signs Up]
    --> B[User Visits Dashboard]

    B --> C[User Searches for a Movie]
    B --> D[User Visits My Library]
    B --> E[User Selects a Film from Dashboard]

    C --> F[User Visits Movie Page]
    D --> F
    E --> F

    F --> G[User Creates a Review for Movie]
```

### Inspect Library
Personas: Collector

```mermaid
flowchart TD
    A[User Signs Up]
    --> B[User Visits Dashboard]
    B --> C[User Selects 'My Library' from Side Navigation]
    C --> D[User Visits Library Page]
```

