using System;
using System.Collections.Generic;
using System.Text.Json;

namespace AdminHelper.lib
{
    public static class Json
    {
        public static Dictionary<string, string> GetEachNodeFullPathWithLeavesData(JsonTreeNode root, Dictionary<string, string> _parsedJson = null)
        {
            Dictionary<string, string> parsedJson = _parsedJson;
            if (parsedJson == null)
            {
                parsedJson = new Dictionary<string, string>();
            }

            HashSet<JsonTreeNode>.Enumerator nodes = root.GetChildrenEnumerator();
            while (nodes.MoveNext())
            {
                if (nodes.Current.IsALeaf())
                {
                    break;
                }
                
                if (nodes.Current.AreChildrenLeaves())
                {
                    string fullPath = GetFullPath(nodes.Current);
                    string data = String.Join(" ", CollectSiblingChildrenData(nodes.Current));
                    parsedJson.Add(fullPath, data);
                    continue;
                }

                parsedJson.Add(GetFullPath(nodes.Current), null);
                GetEachNodeFullPathWithLeavesData(nodes.Current, parsedJson);
            }
            return parsedJson;
        }

        public static Dictionary<string, string> GetLeafFullPathWithItsData(List<JsonTreeNode> allLeafList)
        {
            Dictionary<string, string> parsedJson = new Dictionary<string, string>();

            JsonTreeNode node = null; // for skipping
            foreach (JsonTreeNode leaf in allLeafList)
            {
                // Skip the same parent
                if (node != null
                &&  node == leaf.GetTheParent())
                {
                    continue;
                }
                node = leaf.GetTheParent();

                // Collect the leaves by their full path
                parsedJson.Add(GetFullPath(node), leaf.GetData());
            }
            return parsedJson;
        }

        // Collect the leaves by their parent nodes
        public static string[] CollectSiblingChildrenData(JsonTreeNode node)
        {
            List<string> leavesCollectedByParentNode = new List<string>();
            HashSet<JsonTreeNode>.Enumerator children = node.GetChildrenEnumerator();
            while (children.MoveNext())
            {
                leavesCollectedByParentNode.Add(children.Current.GetData());
            }
            return leavesCollectedByParentNode.ToArray();
        }

        public static string GetOneLeafData(JsonTreeNode node)
        {
            HashSet<JsonTreeNode>.Enumerator children = node.GetChildrenEnumerator();
            while (children.MoveNext())
            {
                return children.Current.GetData();
            }
            return null;
        }

        // Make fullPath from leaf to root
        public static string GetFullPath(JsonTreeNode node)
        {
            if (node.IsALeaf())
            {
                node = node.GetTheParent();
            }

            List<string> fullDepthDataFromLeafToRoot = new List<string>();
            while (node.HaveAParent())
            {
                fullDepthDataFromLeafToRoot.Add(node.GetData());
                node = node.GetTheParent();
            }

            fullDepthDataFromLeafToRoot.Reverse();
            return String.Join(" - ", fullDepthDataFromLeafToRoot.ToArray());
        }

        public static bool Validate(byte[] json)
        {
            if (json == null
            ||  json.Length < 2)
            {
                return false;
            }
            
            try
            {
                JsonDocument.Parse(json);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        public static JsonTreeNode Parse(byte[] json, string treeName, List<JsonTreeNode> allLeaves = null)
        {
            string jsonString = System.Text.Encoding.Default.GetString(json);
            return Parse(jsonString, treeName, allLeaves);
        }

        public static JsonTreeNode Parse(string json, string treeName, List<JsonTreeNode> allLeaves = null)
        {
            JsonDocumentOptions options = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            try
            {
                using (JsonDocument jsonDoc = JsonDocument.Parse(json, options))
                {
                    JsonElement root = jsonDoc.RootElement;
                    JsonTreeNode treeRoot = JsonTreeNode.MakeARoot(treeName);
                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        foreach (JsonProperty element in root.EnumerateObject())
                        {
                            Parse(element, treeRoot, allLeaves);
                        }
                    }
                    return treeRoot;
                }
            }
            catch (Exception ex) 
            {
                AppException.Handle(ex);
                return null;
            }
        }

        private static void Parse(JsonProperty element, JsonTreeNode treeParent, List<JsonTreeNode> allLeaves)
        {
            JsonTreeNode treeNode = GetTreeNode(treeParent, element.Name);

            JsonElement value = element.Value;
            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (JsonProperty item in value.EnumerateObject())
                    {
                        Parse(item, treeNode, allLeaves);
                    }
                    break;
                case JsonValueKind.Array: // concat all values to one string
                    AddLeafArray(value.EnumerateArray(), treeNode, allLeaves);
                    break;
                case JsonValueKind.String:
                case JsonValueKind.Number:
                    AddLeaf(value, treeNode, allLeaves);
                    break;
                default:
                    break;
            }
        }

        private static JsonTreeNode GetTreeNode(JsonTreeNode treeParent, string elementName)
        {
            if (IsNotNullEmptySpace(elementName))
            {
                return treeParent.AddAChild(elementName);
            }
            return treeParent;
        }

        private static void AddLeafArray(JsonElement.ArrayEnumerator enumerator, JsonTreeNode treeNode, List<JsonTreeNode> allLeaves)
        {
            List<string> leaf = new List<string>();
            foreach (JsonElement item in enumerator)
            {
                leaf.Add(GetStringFrom(item));
            }
            bool isLeafArray = true;
            AddLeaf(String.Join(" ", leaf), treeNode, allLeaves, isLeafArray);
        }

        private static void AddLeaf(JsonElement value, JsonTreeNode treeNode, List<JsonTreeNode> allLeaves, bool isLeafArray = false)
        {
            string valueAsString = GetStringFrom(value);
            AddLeaf(valueAsString, treeNode, allLeaves, isLeafArray);
        }

        private static void AddLeaf(string value, JsonTreeNode treeNode, List<JsonTreeNode> allLeaves, bool isLeafArray = false)
        {
            if (IsNotNullEmptySpace(value))
            {
                bool isleaf = true;
                JsonTreeNode leaf = treeNode.AddAChild(value, isleaf, isLeafArray);
                allLeaves?.Add(leaf);
            }
        }

        private static string GetStringFrom(JsonElement value)
        {
            if (value.ValueKind == JsonValueKind.String)
            {
                return value.GetString();
            }
            if (value.ValueKind == JsonValueKind.Number)
            {
                return value.GetInt32().ToString();
            }
            return null;
        }

        public static bool IsNullEmptySpace(string s)
        {
            if (String.IsNullOrEmpty(s)
            ||  String.IsNullOrWhiteSpace(s)) 
            {
                return true;
            }
            return false;
        }

        public static bool IsNotNullEmptySpace(string s)
        {
            return !IsNullEmptySpace(s);
        }
    }

    public class JsonTreeNode
    {
        private readonly JsonTreeNode Parent;
        private readonly HashSet<JsonTreeNode> Children;
        private readonly string Data;
        private bool _IsLeaf;
        private bool _IsLeafArray;

        private JsonTreeNode(JsonTreeNode parent, string data, bool isLeaf = false, bool isLeafArray = false)
        {
            Children = new HashSet<JsonTreeNode>();
            Parent = parent;
            Data = data;
            _IsLeaf = isLeaf;
            _IsLeafArray = isLeafArray;
        }

        public static JsonTreeNode MakeARoot(string data)
        {
            JsonTreeNode parent = null;
            return new JsonTreeNode(parent, data);
        }

        public bool HaveAParent()
        {
            return Parent != null;
        }

        public JsonTreeNode GetTheParent()
        {
            return Parent;
        }

        public JsonTreeNode AddAChild(string data, bool isLeaf = false, bool isLeafArray = false)
        {
            if (_IsLeaf || _IsLeafArray)
            {
                return null;
            }

            JsonTreeNode child = new JsonTreeNode(this, data, isLeaf, isLeafArray);
            Children.Add(child);
            return child;
        }

        public int GetChildCount()
        {
            return Children.Count;
        }

        public HashSet<JsonTreeNode>.Enumerator GetChildrenEnumerator()
        {
            return Children.GetEnumerator();
        }

        public string GetData()
        {
            return Data;
        }

        public string[] GetDataArrayFromLeaf()
        {
            if (IsLeafArray())
            {
                return Data.Split(' ');
            }
            return null;   
        }

        public bool IsALeaf()
        {
            return _IsLeaf;
        }

        public bool IsLeafArray()
        {
            return IsALeaf() && _IsLeafArray;
        }

        public bool AreChildrenLeaves()
        {
            HashSet<JsonTreeNode>.Enumerator children = GetChildrenEnumerator();
            while(children.MoveNext())
            {
                if (children.Current.IsALeaf())
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}