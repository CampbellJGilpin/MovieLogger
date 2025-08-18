-- Add user lists functionality
-- This migration adds tables to support user-created lists of movies

-- Create lists table
CREATE TABLE lists (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE,
    CONSTRAINT fk_lists_user_id FOREIGN KEY (user_id) REFERENCES users(id),
    CONSTRAINT uq_user_list_name UNIQUE (user_id, name)
);

-- Create list_movies junction table for many-to-many relationship
CREATE TABLE list_movies (
    id SERIAL PRIMARY KEY,
    list_id INT NOT NULL,
    movie_id INT NOT NULL,
    added_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_list_movies_list_id FOREIGN KEY (list_id) REFERENCES lists(id) ON DELETE CASCADE,
    CONSTRAINT fk_list_movies_movie_id FOREIGN KEY (movie_id) REFERENCES movies(id) ON DELETE CASCADE,
    CONSTRAINT uq_list_movie UNIQUE (list_id, movie_id)
);

-- Create indexes for better query performance
CREATE INDEX idx_lists_user_id ON lists(user_id) WHERE is_deleted = FALSE;
CREATE INDEX idx_lists_created_date ON lists(created_date);
CREATE INDEX idx_list_movies_list_id ON list_movies(list_id);
CREATE INDEX idx_list_movies_movie_id ON list_movies(movie_id);
CREATE INDEX idx_list_movies_added_date ON list_movies(added_date);

-- Add trigger to update updated_date on lists table
CREATE OR REPLACE FUNCTION update_lists_updated_date()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_date = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER trigger_update_lists_updated_date
    BEFORE UPDATE ON lists
    FOR EACH ROW
    EXECUTE FUNCTION update_lists_updated_date();