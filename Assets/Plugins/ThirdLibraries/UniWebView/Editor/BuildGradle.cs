using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class UniWebViewGradleConfig
{
    private readonly string m_filePath;

    public UniWebViewGradleNode Root { get; }

    public UniWebViewGradleConfig(string filePath)
    {
        var file = File.ReadAllText(filePath);
        TextReader reader = new StringReader(file);

        m_filePath = filePath;
        Root = new UniWebViewGradleNode("root");
        var curNode = Root;

        var str = new StringBuilder();
        var inDoubleQuote = false;
        var inSingleQuote = false;
        var inDollarVariable = false;

        while (reader.Peek() > 0)
        {
            char c = (char)reader.Read();
            switch (c)
            {
                // case '/':
                //     if (reader.Peek() == '/')
                //     {
                //         reader.Read();
                //         string comment = reader.ReadLine();
                //         Debug.Log("Comment line: " + comment);
                //         curNode.AppendChildNode(new UniWebViewGradleCommentNode(comment, curNode));
                //     }
                //     else
                //     {
                //         str.Append('/');
                //     }
                //     break;
                case '\n':
                case '\r':
                    {
                        var strf = FormatStr(str);
                        if (!string.IsNullOrEmpty(strf))
                        {
                            curNode.AppendChildNode(new UniWebViewGradleContentNode(strf, curNode));
                        }
                    }
                    inDollarVariable = false;
                    str = new StringBuilder();
                    break;
                case '\t':
                    {
                        var strf = FormatStr(str);
                        if (!string.IsNullOrEmpty(strf))
                        {
                            str.Append(" ");
                        }
                        break;
                    }
                case '{':
                    {
                        if (inDoubleQuote || inSingleQuote) {
                            str.Append(c);
                            break;
                        }
                        var n = FormatStr(str);
                        if (!string.IsNullOrEmpty(n))
                        {
                            UniWebViewGradleNode node = new UniWebViewGradleNode(n, curNode);
                            curNode.AppendChildNode(node);
                            curNode = node;
                        }
                    }
                    str = new StringBuilder();
                    break;
                case '}':
                    {
                        if (inDoubleQuote || inSingleQuote) {
                            str.Append(c);
                            break;
                        }
                        var strf = FormatStr(str);
                        if (!string.IsNullOrEmpty(strf))
                        {
                            curNode.AppendChildNode(new UniWebViewGradleContentNode(strf, curNode));
                        }
                        curNode = curNode.Parent;
                    }
                    str = new StringBuilder();
                    break;
                case '\"':
                    if (inDollarVariable) {
                        str.Append(c);
                        break;
                    }
                    inDoubleQuote = !inDoubleQuote;
                    str.Append(c);
                    break;
                case '\'':
                    if (inDollarVariable) {
                        str.Append(c);
                        break;
                    }
                    inSingleQuote = !inSingleQuote;
                    str.Append(c);
                    break;
                case '$': 
                    {
                        if (inDoubleQuote || inSingleQuote) {
                            str.Append(c);
                            break;
                        }
                        inDollarVariable = true;
                        str.Append(c);
                        break;
                    }
                default:
                    str.Append(c);
                    break;
            }
        }

        // End of file.
        var endline = FormatStr(str);
        if (!string.IsNullOrEmpty(endline))
        {
            curNode.AppendChildNode(new UniWebViewGradleContentNode(endline, curNode));
        }
        //Debug.Log("Gradle parse done!");
    }

    public void Save(string path = null)
    {
        if (path == null) {
            path = m_filePath;
        }
            
        File.WriteAllText(path, Print());
    }

    private static string FormatStr(StringBuilder sb)
    {
        var str = sb.ToString();
        str = str.Trim();
        return str;
    }
    public string Print()
    {
        StringBuilder sb = new StringBuilder();
        PrintNode(sb, Root, -1);
        // Remove the first empty line.
        sb.Remove(0, 1);
        return sb.ToString();
    }
    private string GetLevelIndent(int level)
    {
        if (level <= 0) return "";
        StringBuilder sb = new StringBuilder("");
        for (int i = 0; i < level; i++)
        {
            sb.Append('\t');
        }
        return sb.ToString();
    }
    private void PrintNode(StringBuilder stringBuilder, UniWebViewGradleNode node, int level)
    {
        if (node.Parent != null) {
            if (node is UniWebViewGradleCommentNode)
            {
                stringBuilder.Append("\n" + GetLevelIndent(level) + @"//" + node.Name);
            }
            else
            {
                stringBuilder.Append("\n" + GetLevelIndent(level) + node.Name);
            }

        }

        if (node is UniWebViewGradleContentNode || node is UniWebViewGradleCommentNode) return;
        if (node.Parent != null)  {
            stringBuilder.Append(" {");
        }
        foreach (var c in node.Children) {
            PrintNode(stringBuilder, c, level + 1);
        }
        if (node.Parent != null) {
            stringBuilder.Append("\n" + GetLevelIndent(level) + "}");
        }
    }
}

public class UniWebViewGradleNode
{
    protected string m_name;
    public UniWebViewGradleNode Parent { get; private set; }

    public string Name => m_name;

    public List<UniWebViewGradleNode> Children { get; private set; } = new List<UniWebViewGradleNode>();

    public UniWebViewGradleNode(string name, UniWebViewGradleNode parent = null)
    {
        Parent = parent;
        m_name = name;
    }

    public void Each(Action<UniWebViewGradleNode> f)
    {
        f(this);
        foreach (var n in Children)
        {
            n.Each(f);
        }
    }

    public void AppendChildNode(UniWebViewGradleNode node)
    {
        if (Children == null) Children = new List<UniWebViewGradleNode>();
        Children.Add(node);
        node.Parent = this;
    }

    public UniWebViewGradleNode TryGetNode(string path)
    {
        var subpath = path.Split('/');
        var cnode = this;
        foreach (var p in subpath)
        {
            if (string.IsNullOrEmpty(p)) continue;
            var tnode = cnode.FindChildNodeByName(p);
            if (tnode == null)
            {
                Debug.Log("Can't find Node:" + p);
                return null;
            }

            cnode = tnode;
        }

        return cnode;
    }

    public UniWebViewGradleNode FindChildNodeByName(string name)
    {
        foreach (var n in Children)
        {
            if (n is UniWebViewGradleCommentNode || n is UniWebViewGradleContentNode)
                continue;
            if (n.Name == name)
                return n;
        }
        return null;
    }

    public bool ReplaceContentStartsWith(string pattern, string value)
    {
        foreach (var n in Children)
        {
            if (!(n is UniWebViewGradleContentNode)) continue;
            if (n.m_name.StartsWith(pattern))
            {
                n.m_name = value;
                return true;
            }
        }
        return false;
    }

    public UniWebViewGradleContentNode ReplaceContentOrAddStartsWith(string pattern, string value)
    {
        foreach (var n in Children)
        {
            if (!(n is UniWebViewGradleContentNode)) continue;
            if (n.m_name.StartsWith(pattern))
            {
                n.m_name = value;
                return (UniWebViewGradleContentNode)n;
            }
        }
        return AppendContentNode(value);
    }
    
    public UniWebViewGradleContentNode AppendContentNode(string content)
    {
        foreach (var n in Children)
        {
            if (!(n is UniWebViewGradleContentNode)) continue;
            if (n.m_name == content)
            {
                Debug.Log("UniWebViewGradleContentNode with " + content + " already exists!");
                return null;
            }
        }
        UniWebViewGradleContentNode cnode = new UniWebViewGradleContentNode(content, this);
        AppendChildNode(cnode);
        return cnode;
    }


    public bool RemoveContentNode(string contentPattern)
    {
        for(int i=0;i<Children.Count;i++)
        {
            if (!(Children[i] is UniWebViewGradleContentNode)) continue;
            if(Children[i].m_name.Contains(contentPattern))
            {
                Children.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
}

public sealed class UniWebViewGradleContentNode : UniWebViewGradleNode
{
    public UniWebViewGradleContentNode(String content, UniWebViewGradleNode parent) : base(content, parent)
    {

    }

    public void SetContent(string content)
    {
        m_name = content;
    }
}

public sealed class UniWebViewGradleCommentNode : UniWebViewGradleNode
{
    public UniWebViewGradleCommentNode(String content, UniWebViewGradleNode parent) : base(content, parent)
    {

    }

    public string GetComment()
    {
        return m_name;
    }
}

