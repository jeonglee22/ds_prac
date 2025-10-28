public class TreeNode<Tkey, Tvalue>
{
    public Tkey Key { get; set; }
    public Tvalue Value { get; set; }
    public int Height { get; set; }

    public TreeNode<Tkey, Tvalue> Left { get; set; }
    public TreeNode<Tkey, Tvalue> Right { get; set; }

    public TreeNode(Tkey key, Tvalue value)
    {
        Key = key;
        Value = value;
        Height = 1;
    }
}
