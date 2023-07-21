USE [Howler];
GO

insert into [User] (DisplayName, Email, FirebaseId, ProfilePictureUrl, DateCreated) VALUES ('Rewyn Ebonpelt', 'rewyn@ebonpelt.com', '5lSYPg72X9SLwYZ1eDDPB7lCfRu1', 'https://cdn.discordapp.com/attachments/1110416331741855866/1132021669200941206/image.png', GETDATE());
insert into [Board] ([Name], [Topic], [Description], [BoardOwnerId], [IsPackBoard]) VALUES ('Moon Criers Cave', 'All things related to us.', 'The general home for Moon Criers pack members. Feel free to discuss most things.', 1, 1);
insert into [Pack] ([Name], PackLeaderId, [Description], PrimaryBoardId) VALUES ('Moon Criers', 1, 'We like to howl at the moon.', 1);
insert into [Post] ([Title], [Content], [UserId], [BoardId], [CreatedOn]) VALUES ('First test post on Moon Criers Board', 'This is the first post made by the sql data seed script.', 1, 1, GETDATE());
insert into [Comment] ([UserId], [PostId], [Content], [CreatedOn]) VALUES (1, 1, 'This post kind of sucks, and should only be viewable if you are in the pack.', GETDATE());
insert into [Board] ([Name], [Topic], [Description], [BoardOwnerId], [IsPackBoard]) VALUES ('General Stuff', 'Regular daily chit chat and questions.', 'This board is used for regular, misc discussion about all things werewolf.', 1, 0);
insert into [Post] ([Title], [Content], [UserId], [BoardId], [CreatedOn]) VALUES ('Test post on General', 'This is the second post from the sql seed data script.', 1, 2, GETDATE());
insert into [Comment] ([UserId], [PostId], [Content], [CreatedOn]) VALUES (1, 2, 'What a great post brother. Everyone on the site should be able to see this!', GETDATE());