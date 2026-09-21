using CLI.UI;
using Entities;
using FileRepositories;
using RepositoryContracts;

IUserRepository userRepository = new UserFileRepository();
IPostRepository postRepository = new PostFileRepository();
ICommentRepository commentRepository = new CommentFileRepository();

User user1 = await userRepository.AddAsync(new User { Username = "emre", Password = "1234" });
User user2 = await userRepository.AddAsync(new User { Username = "kekec", Password = "abcd" });

Post post1 = await postRepository.AddAsync(new Post { Title = "First post", Body = "This is the first post.", UserId = user1.Id });
Post post2 = await postRepository.AddAsync(new Post { Title = "Second post", Body = "This is the second post.", UserId = user2.Id });

await commentRepository.AddAsync(new Comment { Body = "Nice post", PostId = post1.Id, UserId = user2.Id });
await commentRepository.AddAsync(new Comment { Body = "Thanks for sharing", PostId = post2.Id, UserId = user1.Id });

CliApp cliApp = new CliApp(userRepository, postRepository, commentRepository);
await cliApp.StartAsync();