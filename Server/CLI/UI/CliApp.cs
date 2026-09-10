using RepositoryContracts;

using Entities;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;

    public CliApp(
        IUserRepository userRepository,
        ICommentRepository commentRepository,
        IPostRepository postRepository)
    {
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
    }
    
    public async Task StartAsync()
    {
        Boolean running = true;

        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("=== CLI MENU ===");
            Console.WriteLine("1. Create User");
            Console.WriteLine("2. Create Post");
            Console.WriteLine("3. Add Comment");
            Console.WriteLine("4. View Posts");
            Console.WriteLine("5. View Specific Post");
            Console.WriteLine("0. Exit");
            Console.Write("Choose an option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter username: ");
                    string? username = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(username))
                    {
                        Console.WriteLine("Username cannot be empty.");
                        break;
                    }

                    bool usernameTaken = userRepository
                        .GetMany()
                        .Any(u => u.UserName == username);

                    if (usernameTaken)
                    {
                        Console.WriteLine("Username is already taken.");
                        break;
                    }

                    Console.Write("Enter password: ");
                    string? password = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(password))
                    {
                        Console.WriteLine("Password cannot be empty.");
                        break;
                    }

                    User user = new User
                    {
                        UserName = username,
                        Password = password
                    };

                    User createdUser = await userRepository.AddAsync(user);

                    Console.WriteLine(
                        $"User created successfully with ID: {createdUser.Id}");
                    break;

                case "2":
                    Console.Write("Enter user ID: ");
                    string? userIdInput = Console.ReadLine();

                    if (!int.TryParse(userIdInput, out int userId))
                    {
                        Console.WriteLine("Invalid user ID.");
                        break;
                    }

                    bool userExists = userRepository
                        .GetMany()
                        .Any(u => u.Id == userId);

                    if (!userExists)
                    {
                        Console.WriteLine("User does not exist.");
                        break;
                    }

                    Console.Write("Enter post title: ");
                    string? title = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(title))
                    {
                        Console.WriteLine("Title cannot be empty.");
                        break;
                    }

                    Console.Write("Enter post body: ");
                    string? body = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(body))
                    {
                        Console.WriteLine("Body cannot be empty.");
                        break;
                    }

                    Post post = new Post
                    {
                        Title = title,
                        Body = body,
                        UserId = userId
                    };

                    Post createdPost = await postRepository.AddAsync(post);

                    Console.WriteLine(
                        $"Post created successfully with ID: {createdPost.Id}");
                    break;

                case "3":
                    Console.Write("Enter user ID: ");
                    string? commentUserIdInput = Console.ReadLine();

                    if (!int.TryParse(commentUserIdInput, out int commentUserId))
                    {
                        Console.WriteLine("Invalid user ID.");
                        break;
                    }

                    bool commentUserExists = userRepository
                        .GetMany()
                        .Any(u => u.Id == commentUserId);

                    if (!commentUserExists)
                    {
                        Console.WriteLine("User does not exist.");
                        break;
                    }

                    Console.Write("Enter post ID: ");
                    string? postIdInput = Console.ReadLine();

                    if (!int.TryParse(postIdInput, out int postId))
                    {
                        Console.WriteLine("Invalid post ID.");
                        break;
                    }

                    bool postExists = postRepository
                        .GetMany()
                        .Any(p => p.Id == postId);

                    if (!postExists)
                    {
                        Console.WriteLine("Post does not exist.");
                        break;
                    }

                    Console.Write("Enter comment: ");
                    string? commentBody = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(commentBody))
                    {
                        Console.WriteLine("Comment cannot be empty.");
                        break;
                    }

                    Comment comment = new Comment
                    {
                        Body = commentBody,
                        UserId = commentUserId,
                        PostId = postId
                    };

                    Comment createdComment =
                        await commentRepository.AddAsync(comment);

                    Console.WriteLine(
                        $"Comment created successfully with ID: {createdComment.Id}");
                    break;
                
                case "4":
                    var posts = postRepository.GetMany().ToList();

                    if (posts.Count == 0)
                    {
                        Console.WriteLine("No posts found.");
                        break;
                    }

                    Console.WriteLine("=== POSTS ===");

                    foreach (Post p in posts)
                    {
                        Console.WriteLine($"ID: {p.Id} | Title: {p.Title}");
                    }

                    break;

                case "5":
                    Console.Write("Enter post ID: ");
                    string? specificPostIdInput = Console.ReadLine();

                    if (!int.TryParse(specificPostIdInput, out int specificPostId))
                    {
                        Console.WriteLine("Invalid post ID.");
                        break;
                    }

                    Post? specificPost = postRepository
                        .GetMany()
                        .SingleOrDefault(p => p.Id == specificPostId);

                    if (specificPost is null)
                    {
                        Console.WriteLine("Post does not exist.");
                        break;
                    }

                    Console.WriteLine();
                    Console.WriteLine("=== POST DETAILS ===");
                    Console.WriteLine($"ID: {specificPost.Id}");
                    Console.WriteLine($"Title: {specificPost.Title}");
                    Console.WriteLine($"Body: {specificPost.Body}");
                    Console.WriteLine($"User ID: {specificPost.UserId}");

                    var comments = commentRepository
                        .GetMany()
                        .Where(c => c.PostId == specificPostId)
                        .ToList();

                    Console.WriteLine();
                    Console.WriteLine("=== COMMENTS ===");

                    if (comments.Count == 0)
                    {
                        Console.WriteLine("No comments.");
                    }
                    else
                    {
                        foreach (Comment c in comments)
                        {
                            Console.WriteLine(
                                $"ID: {c.Id} | User ID: {c.UserId} | {c.Body}");
                        }
                    }

                    break;

                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }
}