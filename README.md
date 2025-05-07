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
    USERS ||--o{ MOVIES : adds
    USERS ||--o{ REVIEWS : writes
    USERS ||--o{ RATINGS : gives
    USERS ||--o{ VIEWINGS : logs
    USERS ||--o{ FAVOURITES : toggles

    VIEWINGS ||--o{ REVIEWS : receives
    VIEWINGS ||--o{ RATINGS : receives

    MOVIES ||--o{ FAVOURITES : receives
    MOVIES ||--o{ VIEWINGS : logged
```

### Glossary

- **Users**: An individual who uses the MovieLogger app. Users can log movies, rate them, write reviews, and add favourites. All users must authenticate to use core features.

- **Movies**: A film entry in the database, containing title, description, release date, and genre. Users interact with movies by logging, rating, reviewing, or favouriting them.

- **Reviews**: A written user review that is associated with a viewing. There is one review per viewing, and a new review would require a new and unique viewing to be logged.

- **Ratings**: A numerical score (e.g., 1–10) that a user assigns to a movie they've logged. This is associated with a viewing.

- **Favourites**: A toggle which a user can apply to a movie to mark it as a personal favourite. This is independent from viewings and is tied directly to a movie.

- **Ownership**: A toggle that indicates if a user owns a specific movie. This is independent from viewings and is tied directly to a movie.

- **Viewing**: A snapshot of an invidual movie viewing. This holds the date the user viewed a specific movie, as well as their review and score associated with it.

## API Structure

### Authentication

### `POST /auth/register`

**Description:** Register a new user account.

**Request Body Example:**
```json
{
  "userName": "newuser123",
  "email": "newuser@example.com",
  "password": "securepassword"
}
```

**Responses:**
- `201 Created`
- `400 Bad Request`

**Response Example:**
```json
{
  "id": 5,
  "userName": "newuser123",
  "email": "newuser@example.com",
  "isAdmin": false,
  "isDeleted": false
}
```

---

### `POST /auth/login`

**Description:** Authenticate a user and receive a token.

**Request Body Example:**
```json
{
  "email": "newuser@example.com",
  "password": "securepassword"
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

**Description:** Add a new user to the system.

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

### `GET /users/{id}/dashboard`

**Description:** Returns dashboard collections for a specific user, including recently watched movies, their watchlist, owned films, and favourites. Each item also includes whether the movie is in their library.

**Responses:**
- `200 OK`

**Response Example:**
```json
{
  "recentlyWatched": [
    {
      "movieId": 2,
      "title": "Companion",
      "dateViewed": "2025-04-26",
      "score": 8,
      "inLibrary": true
    }
  ],
  "watchlist": [
    {
      "movieId": 4,
      "title": "Mickey 17",
      "upcomingViewDate": "2025-05-01",
      "inLibrary": true
    }
  ],
  "owned": [
    {
      "movieId": 1,
      "title": "Sinners",
      "inLibrary": true
    }
  ],
  "favourites": [
    {
      "movieId": 3,
      "title": "Pulp Fiction",
      "inLibrary": true
    }
  ]
}
```

### `GET /users/{id}/movies`

**Description:** Returns a grid-style list of movies for the user, with high-level metadata and toggle states.

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

### `GET /users/{userId}/movies/{movieId}`

**Description:** Retrieve a single movie’s detail and associated user-specific data like toggle states and viewing history.

**Responses:**
- `200 OK`
- `404 Not Found`

**Response Example:**
```json
{
  "movie": {
    "id": 2,
    "title": "Mickey 17",
    "description": "A disposable employee is sent on a human expedition to colonize the ice world Niflheim...",
    "releaseDate": "2025-03-07",
    "genreId": 1
  },
  "favourite": true,
  "watchLater": false,
  "inLibrary": true,
  "viewings": [
    {
      "viewingId": 7,
      "dateViewed": "2025-04-12",
      "review": "Brilliant concept with fantastic world-building.",
      "score": 9
    },
    {
      "viewingId": 11,
      "dateViewed": "2025-04-25",
      "review": "Second watch was even better.",
      "score": 10
    }
  ]
}
```

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

---

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

---

### `PUT /genres/{id}`

**Description:** Update the title of an existing genre.

**Request Example:**
```json
{
  "id": 2,
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

---

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

---

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

---

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

---

### `DELETE /movies/{id}`

**Description:** Soft-delete a movie by marking it as deleted (`isDeleted = true`).

**Responses:**
- `204 No Content`
- `404 Not Found`

---

### Viewings

### `GET /users/{userId}/movies`

**Description:** Retrieve all movies in a specific user's library.

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

### `POST /users/{userId}/movies`

**Description:** Add a movie to the user's collection.

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

---

### `PUT /user-movies/{id}`

**Description:** Update flags or viewing info for a user-movie entry.

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

---

### `DELETE /user-movies/{id}`

**Description:** Remove a movie from user's library.

**Responses:**
- `204 No Content`

### `GET /users/{id}/dashboard`

**Description:** Returns dashboard collections for a specific user, including recently watched movies, their watchlist, and owned films.

**Responses:**
- `200 OK`

**Response Example:**
```json
{
  "recentlyWatched": [
    {
      "movieId": 2,
      "title": "Companion",
      "dateViewed": "2025-04-26",
      "score": 8
    }
  ],
  "watchlist": [
    {
      "movieId": 4,
      "title": "Mickey 17",
      "upcomingViewDate": "2025-05-01"
    }
  ],
  "owned": [
    {
      "movieId": 1,
      "title": "Sinners"
    }
  ]
}
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

### `POST /user-movies/{id}/viewings`

**Description:** Log a new viewing.

**Request Example:**
```json
{
  "dateViewed": "2025-05-06"
}
```

**Response Example:**
```json
{
  "id": 21,
  "userMovieId": 13,
  "dateViewed": "2025-05-06"
}
```

**Responses:**
- `201 Created`
- `400 Bad Request`
- `404 Not Found`

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
  "reviewText": "Even better the second time, bumped it up to a 10.",
  "score": 10
}
```

**Response Example:**
```json
{
  "id": 17,
  "reviewText": "Even better the second time, bumped it up to a 10.",
  "score": 10
}
```

**Responses:**
- `200 OK`
- `400 Bad Request`
- `404 Not Found`

## User Personas

### Casual User

- Wants to simply keep track of what they've watched, and want to watch.
- Will occasionally write reviews.

### Collector

- Wants a solution to easily keep track and search of movies they've collected.
- Will reguarly add new movies to their owned list.

### Data-Oriented User

- Wants an easy solution to view statistics, such as how many movies they've watched or own.
- Wants to filter and sort through their viewing history.
- May organise their films into collections uses tags.

## User Journeys

```mermaid
flowchart TD
    A[Login / Register] --> B[Dashboard]
    B --> C[Search for Movie]
    C --> D[Movie Page]
    D --> E[Add Movie to Library]
    D --> F[Log a Movie Viewing]
    D --> G[Write a Review]
    D --> H[View Watch History]
```
