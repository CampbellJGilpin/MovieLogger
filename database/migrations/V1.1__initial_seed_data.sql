INSERT INTO Genres(Id, Title) 
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

INSERT INTO Movies (Id, Title, Description, ReleaseDate, GenreId, IsDeleted) 
VALUES
(1, 'Sinners', 'Trying to leave their troubled lives behind, twin brothers return to their hometown to start again, only to discover that an even greater evil is waiting to welcome them back.', '2025-04-18', 2, FALSE),
(2, 'Reservoir Dogs', 'A botched robbery indicates a police informant, and the pressure mounts in the aftermath at a warehouse.', '1992-06-25', 5, FALSE),
(3, 'Kung Fu Panda', 'When the Valley of Peace is threatened, lazy Po the panda discovers his destiny as the "chosen one" and trains to become a kung fu hero, but transforming the unsleek slacker into a brave warrior won''t be easy.', '2008-07-04', 10, FALSE),
(4, 'Bone Tomahawk', 'During a shootout in a saloon, Sheriff Hunt injures a suspicious stranger. The doctor''s assistant, wife of the local foreman, tends to him in prison. That night, the town is attacked and they both disappear—only the arrow of a cannibal tribe is found. Hunt and a few of his men go in search of the prisoner and the foreman''s wife.', '2015-10-01', 8, FALSE);
