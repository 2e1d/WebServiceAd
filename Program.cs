
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPlatformSearcher, PlatformSearcher>();
var app = builder.Build();

app.Run(async context =>
{
    var response = context.Response;
    var platformSearcher = app.Services.GetService<IPlatformSearcher>();
    await platformSearcher.GetPlatforms();
    response.ContentType = "text/plain; charset=utf-8";
    await context.Response.WriteAsync($"{platformSearcher.SearchPlatform("/ru")}");
});

app.Run();


interface IPlatformSearcher
{
    public Task GetPlatforms();
    public string SearchPlatform(string value);
}


class PlatformSearcher : IPlatformSearcher
{
    Dictionary<string, string> platformsDictionary = new Dictionary<string, string>();
    Tree locationTree = new Tree();
    public async Task GetPlatforms()
    {
        using (StreamReader reader = new StreamReader("test.txt"))
        {
            char[] separators = [':', ','];
            string platforms = await reader.ReadToEndAsync();
            string[] lines = platforms.Split('\n');

            foreach (var line in lines)
            {
                string[] data = line.Split(separators, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                locationTree.Add(data);
            }
        }
    }

    public string SearchPlatform(string value)
    {
        string response = "";
        Node foundNode = new Node();
        Node iterationNode = locationTree.Root;
        string[] locations = value.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var location in locations)
        {
            if (locationTree.Search(location))
            {
                foundNode = iterationNode.children.FirstOrDefault(x => x.location == location);
                foreach (var platform in foundNode.platforms)
                {
                    response += platform;
                }
                iterationNode = foundNode;
            }
        }
        return response;
    }
}

public class Node
{
    public string location { get; set; }
    public List<string> platforms { get; set; }
    public List<Node> children { get; set; }

    public Node()
    {
        location = null;
        platforms = new List<string>();
        children = new List<Node>();
    }

    public Node(string location, string platforms)
    {
        this.platforms = new List<string>();
        this.location = location;
        // this.platforms.Add(platforms);
        children = new List<Node>();
    }
    public Node(string[] data)
    {
        platforms = new List<string>();
        platforms.Add(data[0]);
        location = data[1];
    }
}

public class Tree
{
    public Node Root { get; set; }

    public string SearchResponse { get; set; }

    public Tree()
    {
        Root = null;
    }

    public void Add(string[] value)
    {
        AddNode(Root, value);
    }
    private Node AddNode(Node root, string[] data)
    {
        if (root == null)
        {
            root = new Node();
            var firstLocation = data.Skip(1).ToArray()[0].Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[0];
            root.children.Add(new Node(firstLocation, data[0]));
            Root = root;
        }

        foreach (var locations in data.Skip(1))
        {
            foreach (var location in locations.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                if (!Search(location))
                {
                    Node newNode = new Node();
                    newNode.location = location;
                    root.children.Add(newNode);
                    root = newNode;
                }
                else
                {
                    root = root.children.FirstOrDefault(x => x.location == location);
                }
            }
            root.platforms.Add(data[0]);
            root = Root;
        }

        return root;
    }

    public bool Search(string value)
    {
        return SearchNode(Root, value);
    }

    private bool SearchNode(Node root, string value)
    {
        if (root == null)
        {

            return false;
        }

        if (root.location == value)
        {

            return true;
        }

        if (root.children == null)
        {

            return false;
        }

        foreach (var child in root.children)
        {
            if (child.location == value)
            {

                return true;
            }
            else if (SearchNode(child, value))
            {

                return true;
            }
        }

        return false;
    }
}