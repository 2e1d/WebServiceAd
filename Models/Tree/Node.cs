namespace WebServiceAd.Models.Tree;

public class Node
{
    public string? Location { get; set; }
    public List<string> Platforms { get; set; } = [];
    public List<Node> Children { get; set; } = [];

    public Node()
    {
    }
    
    public Node(string location)
    {
        Location = location;
    }
}