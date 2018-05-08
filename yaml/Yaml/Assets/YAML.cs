/* Created by NavyZhang  
 * mail:710605420@qq.com
 * Welcome to exchange ideas
 */
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class YAML: Common.SingletonBase<YAML>
{
    private string outPut = "";
    public class Node
    {
        public string ClassId = "";
        public string RootFileID = ""; //额外属性，只是为了方便归属
        public string Name = ""; 
        public string Value = "";
        public Node Parent = null;
        public List<Node> Children = null;
        public int depth = 0;

        private List<Node> Attribute = new List<Node>();
        public void Parse()
        {

        }

//        public string GetFileIdByParentName(string name)
//         {
//             string res = "";
//             if (Children == null)
//                 return "";
//             foreach(var node in Children)
//             {
//                 res = _GetFileIdByParentName(node, name);
//             }
//             return res;
//         }
//         public string _GetFileIdByParentName(Node node,string name)
//         {
//             if (node == null)
//                 return "";
//             if (node.Value != null)
//                 UnityEngine.Debug.Log(node.Value);
//             if (node.Value != null && name == node.Value.Trim())
//             {
//                 return node.RootFileID;
//             }
//             if (node.Children != null && node.Children.Count != 0)
//             {
//                 foreach(var child in node.Children)
//                 {
//                     _GetFileIdByParentName(child, name);
//                 }
//             }
//             return "";
//         }

        public Node AddNode(string key, string val, int currentDepth,string RootFileID, string ClassId)
        {
            if (currentDepth - depth >= 1)
            {
                if (currentDepth == depth)
                {
                    return Parent.AddNode(key, val, currentDepth, RootFileID, ClassId);
                }
                Node node = new Node();
                node.Name = key;
                node.Value = val;
                node.depth = currentDepth;
                node.Parent = this;
                node.RootFileID = RootFileID;
                node.ClassId = ClassId;
                if (Children == null)
                {
                    Children = new List<Node>();
                }
                Children.Add(node);
                return node;
            }
            else
            {
                if (Parent == null)
                {
                    Node node = new Node();
                    node.Name = key;
                    node.Value = val;
                    node.depth = currentDepth;
                    node.Parent = this;
                    node.RootFileID = RootFileID;
                    node.ClassId = ClassId;
                    if (Children == null)
                    {
                        Children = new List<Node>();
                    }
                    Children.Add(node);
                    return node;
                }
                return Parent.AddNode(key, val, currentDepth, RootFileID, ClassId);
            }
        }
        public Node Find(string name)
        {
            if (Name.Trim() == name.Trim())
            {
                return this;
            }
            if (Children != null)
            {
                for (int i = 0; i < Children.Count; ++i)
                {
                    Node node = Children[i].Find(name);
                    if (null != node)
                        return node;
                }
            }
            return null;
        }

        public void Find(string name, List<Node> nodes)
        {
            if (Name == name)
            {
                nodes.Add(this);
            }
            if (Children != null)
            {
                for (int i = 0; i < Children.Count; ++i)
                {
                    Children[i].Find(name, nodes);
                }
            }
        }

        public List<Node> AttributeForGenerateCode()
        {
            Attribute.Clear();
            if (Children != null)
            {
                foreach (var child in Children)
                {
                    _AttributeForGenerateCode(child);
                }
            }
            return Attribute;
        }

        public void _AttributeForGenerateCode(Node node) //如果自动化代码生成，就需要改变对象名才能产生效果，而且也才能定位。
        {
            if(node != null)
            {
                if(node.Name == "m_Name" && node.Value != null && !string.IsNullOrEmpty(node.Value.Trim()))
                    Attribute.Add(node);
                if(node.Children!= null)
                {
                    foreach (var child in node.Children)
                    {
                        _AttributeForGenerateCode(child);
                    }
                }
            }
        }
    }
    public static Node Load(string path)
    {       
        if (false == File.Exists(path))
        {
            return null;
        }
        FileStream file = new FileStream(path, FileMode.Open);
        if (file == null)
            return null;
        int len = (int)file.Length;
        if (len == 0) return null;
        byte[] buffer = ThreadBuffer.GetBuffer();
        file.Read(buffer, 0, len);
        file.Close();
        file.Dispose();
        string content = System.Text.Encoding.UTF8.GetString(buffer, 0, len);
        return LoadFromString(content);
    }

    public static Node LoadFromString(string content)
    {        
        string ClassId = "";
        string fileId = "";        
        string[] lines = content.Replace("\r\n", "\n").Split('\n');
        int[] depths = new int[lines.Length];
        for (int i = 0; i < lines.Length; ++i)
        {
            string temp = lines[i];
            byte spaceCount = 0;
            for (int j = 0; j < temp.Length; ++j)
            {
                if (temp[j] == ' ')
                {
                    spaceCount++;
                }
                else break;
            }
            //   depths[i] = spaceCount / 2 + 1;
            depths[i] = spaceCount;
            lines[i] = lines[i].Substring(spaceCount, lines[i].Length - spaceCount);
        }
        Node root = new Node();
        for (int i = 0; i < lines.Length; ++i)
        {
            string temp = lines[i];

            if (temp.Length == 0)
                continue;
            //            string key = temp.Replace(": ", ":");
            string key = temp;
            string val = null;
            int idx = key.IndexOf(':');
            if (idx != -1)
            {
                val = key.Substring(idx + 1);
                key = key.Substring(0, idx);
            }
            //             int depthModify = 0;
            //             if (key.StartsWith("- "))
            //             {
            //                 key = key.Replace("- ", "");
            //                 depthModify = 1;
            //                 if (key.StartsWith("{"))
            //                 {
            //                     key = "";
            //                     val = temp.Replace("- ", "");
            //                 }
            //             }
            if (val != null)
            {
                if (val.StartsWith("'") && val.EndsWith("'"))
                {
                    if (val.Length == 1)
                    {
                        if (i + 1 < lines.Length)
                        {
                            string nextLine = lines[i + 1];
                            if (nextLine.EndsWith("'"))
                            {
                                val += "\n" + nextLine.Substring(0, nextLine.Length - 1);
                                lines[i + 1] = "";
                            }
                            else
                            {
                                string nextLine1 = lines[i + 2];
                                if (nextLine1.EndsWith("'"))
                                {
                                    if (nextLine1.Length == 1)
                                    {
                                        nextLine1 = "";
                                    }
                                    else
                                    {
                                        nextLine1 = nextLine1.Substring(0, nextLine1.Length - 1);
                                    }
                                    val += "\n" + nextLine + "\n" + nextLine1;
                                    lines[i + 1] = "";
                                    lines[i + 2] = "";
                                }
                                else
                                {
                                    val = "";
                                }
                            }
                        }
                        else
                        {
                            val = "";
                        }
                    }
                    else if (val.Length <= 2)
                    {
                        val = "";
                        UnityEngine.Debug.LogError("YAML val.Length <= 2" + key + " line = " + i + " content=" + temp);
                    }
                    else
                    {
                        val = val.Substring(1, val.Length - 2);
                    }
                }
            }
            if (temp.Contains("&"))
            {
              
                int pos = 7;
                while(temp[pos] != ' ')
                {
                    ClassId += temp[pos++];
                }
                string[] idInfo = temp.Split('&');
                fileId = idInfo[1];
                root = root.AddNode(key, val, depths[i],"", "");
            }
            else
            {
                root = root.AddNode(key, val, depths[i], fileId, ClassId);
                //fileId = "";                
                ClassId = "";
            }
           
        }
        while (true)
        {
            if (root.Parent != null)
            {
                root = root.Parent;
            }
            else
                break;
        }
        return root;
    }

    public static Dictionary<string,Node> DeserializeValue(string value)
    {
        Dictionary<string, Node> valueNode = new Dictionary<string, Node>();

        if(value.Contains("{"))
        {
            string val = value.Replace("{", "").Replace("}", "");
            string[] valInfo = val.Split(',');

            foreach (var va in valInfo)
            {
                string[] v = va.Split(':');
                Node node = new Node();
                node.Name = v[0];
                node.Value = v[1];
                valueNode.Add(v[0], node);
            }

        }
       
        return valueNode;
    }

    private static string GeneralString(int n, string par)
    {
        System.Text.StringBuilder a = new System.Text.StringBuilder(n * par.Length);
        for (int i = 0; i < n; ++i)
        {
            a.Append(par);
        }
        return a.ToString();
    }

    public string Deserialize(YAML.Node node)
    {
        outPut = "";
        _Deserialize(node);
        return outPut;
    }
    public void _Deserialize(YAML.Node node)
    {
        if (node == null)
        {
            UnityEngine.Debug.Log("No Data");
            return;
        }
        if (node.Children != null)
        {
            for (int i = 0; i < node.Children.Count; ++i)
            {
                YAML.Node child = node.Children[i];
                if (child.Value == null)
                {
                    outPut += GeneralString(child.depth , " ") + child.Name + "\n";
                }
                else
                {
                    outPut += GeneralString(child.depth , " ") + child.Name + ":" + child.Value + "\n";
                }
                if (child.Children != null)
                {
                    _Deserialize(child);
                }

            }
        }
    }

    public void Write(string path,string content)
    {
        if (File.Exists(path))
            File.Delete(path);
        System.Text.UTF8Encoding utf8 = new System.Text.UTF8Encoding(false);
        File.WriteAllText(path, content, utf8);
    }

}