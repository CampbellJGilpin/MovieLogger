INSERT INTO genres (Id, Title) 
VALUES 
(1, 'Action'), 
(2, 'Horror'), 
(3, 'Drama'), 
(4, 'Comedy'), 
(5, 'Thriller'), 
(6, 'Romance'), 
(7, 'Science Fiction'),
(8, 'Western'),
(9, 'Documentary'),
(10, 'Family'),
(11, 'Musical'),
(12, 'Fantasy'),
(13, 'War');

-- Bump the sequence so inserts won't collide
ALTER SEQUENCE genres_id_seq RESTART WITH 14;
