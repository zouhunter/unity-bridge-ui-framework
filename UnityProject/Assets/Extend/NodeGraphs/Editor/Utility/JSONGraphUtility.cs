using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Model=NodeGraph.DataModel;

namespace NodeGraph {
    public class JSONGraphUtility {

        public static void ExportGraphToJSONFromDialog(Model.NodeGraphObj graph) {

            string path =
                EditorUtility.SaveFilePanel/*SaveFilePanelInProject*/(
                    string.Format("Export {0} to JSON file", graph.name), Application.dataPath,
                    graph.name, "json");
            if(string.IsNullOrEmpty(path)) {
                return;
            }
            string jsonString = SerializeGraph(graph);
            File.WriteAllText (path, jsonString, System.Text.Encoding.UTF8);
		}

        public static void ExportAllGraphsToJSONFromDialog() {

            var folderSelected = 
                EditorUtility.OpenFolderPanel("Select folder to export all graphs", Application.dataPath + "..", "");
            if(string.IsNullOrEmpty(folderSelected)) {
                return;
            }

            var guids = AssetDatabase.FindAssets(NGSettings.GRAPH_SEARCH_CONDITION);

            foreach(var guid in guids) {
                string graphPath = AssetDatabase.GUIDToAssetPath(guid);
                string graphName = Path.GetFileNameWithoutExtension(graphPath);

                string jsonFilePath = Path.Combine (folderSelected, string.Format("{0}.json", graphName));

                var graph = AssetDatabase.LoadAssetAtPath<Model.NodeGraphObj>(graphPath);
                string jsonString = SerializeGraph (graph);

                File.WriteAllText (jsonFilePath, jsonString, System.Text.Encoding.UTF8);
            }
        }

        public static Model.NodeGraphObj ImportJSONToGraphFromDialog(Model.NodeGraphObj graph) {

            string fileSelected = EditorUtility.OpenFilePanelWithFilters("Select JSON files to import", Application.dataPath, new string[] {"JSON files", "json", "All files", "*"});
            if(string.IsNullOrEmpty(fileSelected)) {
                return null;
            }

            string name = Path.GetFileNameWithoutExtension(fileSelected);

            var jsonContent = File.ReadAllText (fileSelected, System.Text.Encoding.UTF8);

            if (graph != null) {
                Undo.RecordObject(graph, "Import");
                DeserializeGraph(jsonContent, ref graph);
            } else {
                DeserializeGraph(jsonContent, ref graph);
                var newAssetFolder = CreateFolderForImportedAssets ();
                var graphPath = FileUtility.PathCombine(newAssetFolder, string.Format("{0}.asset", name));
                AssetDatabase.CreateAsset (graph, graphPath);
            }
            return graph;
        }

        public static void ImportAllJSONInDirectoryToGraphFromDialog() {
            var folderSelected = 
                EditorUtility.OpenFolderPanel("Select folder contains JSON files to import", Application.dataPath + "..", "");
            if(string.IsNullOrEmpty(folderSelected)) {
                return;
            }

            var newAssetFolder = CreateFolderForImportedAssets ();

            var filePaths = FileUtility.GetAllFilePathsInFolder (folderSelected);
            foreach (var path in filePaths) {
                var ext = Path.GetExtension (path).ToLower ();
                if (ext != ".json") {
                    continue;
                }
                var jsonContent = File.ReadAllText (path, System.Text.Encoding.UTF8);
                var name = Path.GetFileNameWithoutExtension (path);

                var graph = ScriptableObject.CreateInstance<Model.NodeGraphObj>();
                DeserializeGraph(jsonContent,ref graph);
                var graphPath = FileUtility.PathCombine(newAssetFolder, string.Format("{0}.asset", name));
                AssetDatabase.CreateAsset (graph, graphPath);
            }
        }

        private static string CreateFolderForImportedAssets() {
            var t = DateTime.Now;
            var folderName = String.Format ("ImportedGraphs_{0:D4}-{1:D2}_{2:D2}_{3:D2}{4:D2}{5:D2}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);

            AssetDatabase.CreateFolder ("Assets", folderName);

            return String.Format("Assets/{0}", folderName);
        }

        public static void DeserializeGraph(string json,ref Model.NodeGraphObj graph)
        {
            var jsonNode = JSONClass.Parse(json);
            if (graph == null)
            {
                graph = ScriptableObject.CreateInstance<Model.NodeGraphObj>();
            }
            else
            {
                DeleteSubAsset(graph);
            }
            List<ScriptableObject> subAssets = new List<ScriptableObject>();
            JsonUtility.FromJsonOverwrite(json, graph);

            for (int i = 0; i < jsonNode["m_allNodes"].AsArray.Count; i++)
            {
                var item = jsonNode["m_allNodes"][i]["m_node"];
                var obj = ScriptableObject.CreateInstance(item["type"].Value) as Model.Node;
                JsonUtility.FromJsonOverwrite(item["json"], obj);
                obj.name = obj.GetType().FullName;
                graph.Nodes[i].Object = (obj);
                subAssets.Add(obj);
            }


            for (int i = 0; i < jsonNode["m_allConnections"].AsArray.Count; i++)
            {
                var item = jsonNode["m_allConnections"][i]["m_connection"];
                var obj = ScriptableObject.CreateInstance(item["type"].Value) as Model.Connection;
                JsonUtility.FromJsonOverwrite(item["json"], obj);
                obj.name = obj.GetType().FullName;
                graph.Connections[i].Object = (obj);
                subAssets.Add(obj);
            }

            ScriptableObjUtility.SetSubAssets(subAssets.ToArray(), graph, true);
            EditorUtility.SetDirty(graph);
            foreach (var item in graph.Nodes)
            {
                Debug.Log(item.Object);
                EditorUtility.SetDirty(item.Object);
            }
        }

        public static string SerializeGraph(Model.NodeGraphObj graph)
        {
            var rootJson = JsonUtility.ToJson(graph);
            JSONClass jc = JSONClass.Parse(rootJson) as JSONClass;

            var nodes = jc["m_allNodes"].AsArray;
            for (int i = 0; i < nodes.Count; i++){
                var node = graph.Nodes[i].Object;
                AddFieldToNode(nodes[i]["m_node"], JsonUtility.ToJson(node), node.GetType().FullName);
            }

            var connections = jc["m_allConnections"].AsArray;
            for (int i = 0; i < connections.Count; i++)
            {
                var connection = graph.Connections[i].Object;
                AddFieldToNode(connections[i]["m_connection"], JsonUtility.ToJson(connection), connection.GetType().FullName);
            }
            return jc.ToString();
        }
        private static void DeleteSubAsset(Model.NodeGraphObj graph)
        {
            var path = AssetDatabase.GetAssetPath(graph);
            if (!string.IsNullOrEmpty(path))
            {
                var subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (var item in subAssets)
                {
                    if (item != graph)
                    {
                        Debug.Log("delete:" + item);
                        UnityEngine.Object.DestroyImmediate(item, true);
                    }
                }
            }
        }
        private static void AddFieldToNode(JSONNode node, JSONNode json, string type)
        {
            node.Add("json", json);
            node.Add("type", type);
        }
    }
}
