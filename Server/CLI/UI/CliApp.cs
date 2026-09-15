using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public CliApp(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task StartAsync()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("=== MENU ===");
            Console.WriteLine("1. Create new user");
            Console.WriteLine("2. Create new post");
            Console.WriteLine("3. Add comment to post");
            Console.WriteLine("4. View posts overview");
            Console.WriteLine("5. View specific post");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            if (choice is not null)
            {
                switch (choice)
                {
                    case "1":
                        await CreateUserAsync();
                        break;
                    case "2":
                        await CreatePostAsync();
                        break;
                    case "3":
                        await AddCommentAsync();
                        break;
                    case "4":
                        ViewPostsOverview();
                        break;
                    case "5":
                        ViewSpecificPost();
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option, try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("No input received, try again.");
            }
        }
    }

    private async Task CreateUserAsync()
    {
        Console.Write("Username: ");
        string? username = Console.ReadLine();
        Console.Write("Password: ");
        string? password = Console.ReadLine();

        if (username is not null && password is not null)
        {
            User user = new User { Username = username, Password = password };
            User created = await userRepository.AddAsync(user);

            Console.WriteLine($"User created with Id {created.Id}");
        }
        else
        {
            Console.WriteLine("Username and password cannot be empty.");
        }
    }

    private async Task CreatePostAsync()
    {
        Console.Write("Title: ");
        string? title = Console.ReadLine();
        Console.Write("Body: ");
        string? body = Console.ReadLine();
        Console.Write("User Id: ");
        string? userIdInput = Console.ReadLine();

        if (title is not null && body is not null && int.TryParse(userIdInput, out int userId))
        {
            Post post = new Post { Title = title, Body = body, UserId = userId };
            Post created = await postRepository.AddAsync(post);

            Console.WriteLine($"Post created with Id {created.Id}");
        }
        else
        {
            Console.WriteLine("Invalid input, post was not created.");
        }
    }

    private async Task AddCommentAsync()
    {
        Console.Write("Body: ");
        string? body = Console.ReadLine();
        Console.Write("User Id: ");
        string? userIdInput = Console.ReadLine();
        Console.Write("Post Id: ");
        string? postIdInput = Console.ReadLine();

        if (body is not null && int.TryParse(userIdInput, out int userId) && int.TryParse(postIdInput, out int postId))
        {
            Comment comment = new Comment { Body = body, UserId = userId, PostId = postId };
            Comment created = await commentRepository.AddAsync(comment);

            Console.WriteLine($"Comment created with Id {created.Id}");
        }
        else
        {
            Console.WriteLine("Invalid input, comment was not created.");
        }
    }

    private void ViewPostsOverview()
    {
        IQueryable<Post> posts = postRepository.GetManyAsync();

        Console.WriteLine();
        Console.WriteLine("=== Posts ===");
        foreach (Post post in posts)
        {
            Console.WriteLine($"{post.Id}: {post.Title}");
        }
    }

    private void ViewSpecificPost()
    {
        Console.Write("Post Id: ");
        string? postIdInput = Console.ReadLine();

        if (int.TryParse(postIdInput, out int postId))
        {
            Post? post = postRepository.GetManyAsync().SingleOrDefault(p => p.Id == postId);

            if (post is not null)
            {
                Console.WriteLine();
                Console.WriteLine($"Title: {post.Title}");
                Console.WriteLine($"Body: {post.Body}");
                Console.WriteLine();
                Console.WriteLine("Comments:");

                IQueryable<Comment> comments = commentRepository.GetManyAsync().Where(c => c.PostId == postId);
                foreach (Comment comment in comments)
                {
                    Console.WriteLine($"- {comment.Body}");
                }
            }
            else
            {
                Console.WriteLine("Post not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid Post Id.");
        }
    }
}