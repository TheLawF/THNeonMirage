using System;
using System.Collections.Generic;

namespace THNeonMirage.Util
{
    public class TreeNode<TValue>
    {
        public string Name { get; }
        public TValue Value { get; set; }
        private Dictionary<string, TreeNode<TValue>> _children;

        public TreeNode(string name, TValue value)
        {
            Name = name;
            Value = value;
            _children = new Dictionary<string, TreeNode<TValue>>(StringComparer.OrdinalIgnoreCase);
        }

        public TreeNode<TValue> AddChild(string childName, TValue value)
        {
            if (string.IsNullOrEmpty(childName))
                throw new ArgumentException("Child name cannot be null or empty.");

            if (_children.ContainsKey(childName))
                throw new ArgumentException($"Child node '{childName}' already exists.");

            var childNode = new TreeNode<TValue>(childName, value);
            _children.Add(childName, childNode);
            return childNode;
        }

        public TreeNode<TValue> AddChild(TreeNode<TValue> childNode) => AddChild(childNode.Name, childNode.Value);

        public TreeNode<TValue> GetChild(string childName)
        {
            _children.TryGetValue(childName, out var node);
            return node;
        }
    }
    
    public class Tree<TValue>
    {
        public TreeNode<TValue> Root { get; }

        public Tree(string rootName, TValue rootValue)
        {
            if (string.IsNullOrEmpty(rootName))
                throw new ArgumentException("Root name cannot be null or empty.");

            Root = new TreeNode<TValue>(rootName, rootValue);
        }

        public TreeNode<TValue> GetNode(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.Length == 0)
                return Root;

            string[] segments = path.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            TreeNode<TValue> current = Root;

            foreach (var segment in segments)
            {
                current = current?.GetChild(segment);
                if (current == null) break;
            }

            return current;
        }
    }
}