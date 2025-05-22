CREATE TABLE genres (
    id SERIAL PRIMARY KEY,
    title VARCHAR(100) NOT NULL
);

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    user_name VARCHAR(100) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    is_admin BOOLEAN DEFAULT FALSE,
    is_deleted BOOLEAN DEFAULT FALSE
);

CREATE TABLE movies (
    id SERIAL PRIMARY KEY,
    title VARCHAR(500) NOT NULL,
    description TEXT,
    release_date TIMESTAMP,
    genre_id INT NOT NULL,
    is_deleted BOOLEAN DEFAULT FALSE,
    CONSTRAINT fk_movies_genre_id FOREIGN KEY (genre_id) REFERENCES genres(id)
);

CREATE TABLE user_movies (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    movie_id INT NOT NULL,
    favourite BOOLEAN DEFAULT FALSE,
    owns_movie BOOLEAN DEFAULT FALSE,
    upcoming_view_date TIMESTAMP,
    CONSTRAINT fk_user_movies_user_id FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT fk_user_movies_movie_id FOREIGN KEY (movie_id) REFERENCES movies(id),
    UNIQUE(user_id, movie_id)
);

CREATE TABLE viewings (
    id SERIAL PRIMARY KEY,
    user_movie_id INT NOT NULL,
    date_viewed TIMESTAMP NOT NULL,
    CONSTRAINT fk_viewings_user_movie_id FOREIGN KEY (user_movie_id) REFERENCES user_movies(id)
);

CREATE TABLE reviews (
    id SERIAL PRIMARY KEY,
    viewing_id INT NOT NULL,
    review_text TEXT,
    score INT,
    CONSTRAINT fk_reviews_viewing_id FOREIGN KEY (viewing_id) REFERENCES viewings(id)
);
