using WebServiceAd.Extensions;

namespace WebServiceAd.Models.Tree;

public class Tree
{
    public Node Root { get; set; }

    public void Add(string platform, string[] routes)
    {
        var root = Root;

        if (root is null)
        {
            var coreLocation  = routes.First().SplitNotEmpty('/')[0];

            root = new Node();
            root.Children.Add(new Node(coreLocation));
            Root = root;
        }

        foreach (var route in routes)
        {
            var locations = route.SplitNotEmpty('/');
            foreach (var location in locations)
            {
                if (!IsLocationPresented(location))
                {
                    var newNode = new Node { Location = location };

                    root.Children.Add(newNode);
                    root = newNode;
                }
                else
                {
                    root = root.Children.FirstOrDefault(x => x.Location == location);
                }
            }
            root.Platforms.Add(platform);

            root = Root;
        }
    }

    public bool IsLocationPresented(string value, Node root = null)
    {
        var actualRoot = root ?? Root;

        if (actualRoot == null)
        {
            return false;
        }

        if (actualRoot.Location == value)
        {
            return true;
        }

        if (actualRoot.Children == null)
        {
            return false;
        }

        foreach (var child in actualRoot.Children)
        {
            if (child.Location == value)
            {
                return true;
            }

            if (IsLocationPresented(value, child))
            {
                return true;
            }
        }

        return false;
    }
}