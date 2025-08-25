-- V1.6: Decouple reviews from library membership
-- This migration creates a direct user-movie viewing relationship independent of library

-- Step 1: Create the new user_movie_viewings table
CREATE TABLE user_movie_viewings (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    movie_id INT NOT NULL,
    date_viewed TIMESTAMP NOT NULL,
    CONSTRAINT fk_user_movie_viewings_user_id FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT fk_user_movie_viewings_movie_id FOREIGN KEY (movie_id) REFERENCES movies(id)
);

-- Step 2: Migrate existing viewing data to the new structure
-- This preserves all existing viewings by connecting them directly to users and movies
INSERT INTO user_movie_viewings (user_id, movie_id, date_viewed)
SELECT 
    um.user_id,
    um.movie_id,
    v.date_viewed
FROM viewings v
INNER JOIN user_movies um ON v.user_movie_id = um.id;

-- Step 3: Create a temporary column for the migration
ALTER TABLE reviews ADD COLUMN user_movie_viewing_id INT;

-- Step 4: Update reviews to reference the new user_movie_viewings
-- This matches each review to its corresponding user_movie_viewing record
UPDATE reviews 
SET user_movie_viewing_id = (
    SELECT umv.id 
    FROM user_movie_viewings umv
    INNER JOIN viewings v ON v.date_viewed = umv.date_viewed
    INNER JOIN user_movies um ON v.user_movie_id = um.id
    WHERE v.id = reviews.viewing_id 
    AND um.user_id = umv.user_id 
    AND um.movie_id = umv.movie_id
    LIMIT 1
);

-- Step 5: Drop the old foreign key constraint and column
ALTER TABLE reviews DROP CONSTRAINT fk_reviews_viewing_id;
ALTER TABLE reviews DROP COLUMN viewing_id;

-- Step 6: Add the new foreign key constraint
ALTER TABLE reviews ADD CONSTRAINT fk_reviews_user_movie_viewing_id 
    FOREIGN KEY (user_movie_viewing_id) REFERENCES user_movie_viewings(id);

-- Step 7: Make the new column NOT NULL (after data migration)
ALTER TABLE reviews ALTER COLUMN user_movie_viewing_id SET NOT NULL;

-- Step 8: Drop the old viewings table (no longer needed)
DROP TABLE viewings;

-- Step 9: Add indexes for performance
CREATE INDEX idx_user_movie_viewings_user_id ON user_movie_viewings(user_id);
CREATE INDEX idx_user_movie_viewings_movie_id ON user_movie_viewings(movie_id);
CREATE INDEX idx_user_movie_viewings_date_viewed ON user_movie_viewings(date_viewed);
CREATE INDEX idx_reviews_user_movie_viewing_id ON reviews(user_movie_viewing_id);