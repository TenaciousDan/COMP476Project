using UnityEngine;

using System.Collections.Generic;
using System.Collections.Specialized;

using Tenacious.Collections;

namespace Game.AI
{
    public class Pathfinding : MonoBehaviour
    {
        [SerializeField] public MBGraph mbGraph;

        public delegate float Heuristic(Transform s, Transform e);

        public List<GraphNode<GameObject>> FindPath(string startId, string goalId, Heuristic heuristic = null, bool blockedOffNodesAreAllowed = false, bool isAdmissible = true)
        {
            if (mbGraph == null) return new List<GraphNode<GameObject>>();

            IWeightedDiGraph<GameObject, float> graph = mbGraph.graph;

            if (heuristic == null) heuristic = (Transform s, Transform e) => 0;

            List<GraphNode<GameObject>> path = null;
            bool solutionFound = false;

            Dictionary<string, float> gnDict = new Dictionary<string, float>();
            gnDict.Add(startId, default);

            Dictionary<string, float> fnDict = new Dictionary<string, float>();
            fnDict.Add(startId, heuristic(graph[startId].Data.transform, graph[goalId].Data.transform) + gnDict[startId]);

            Dictionary<string, string> pathDict = new Dictionary<string, string>();
            pathDict.Add(startId, null);

            PriorityQueue<float, string> openPQ = new PriorityQueue<float, string>();
            openPQ.Push(fnDict[startId], startId, fnDict[startId]);

            OrderedDictionary closedODict = new OrderedDictionary();

            while (!openPQ.Empty)
            {
                string currentId = openPQ.Pop().Value;
                closedODict[currentId] = true;

                if (currentId == goalId && isAdmissible)
                {
                    solutionFound = true;
                    break;
                }
                else if (closedODict.Contains(goalId))
                {
                    float gGoal = gnDict[goalId];
                    bool pathIsTheShortest = true;
                    foreach (PQEntry<float, string> entry in openPQ)
                    {
                        if (gGoal > gnDict[entry.Value])
                        {
                            pathIsTheShortest = false;
                            break;
                        }
                    }

                    if (pathIsTheShortest) break;
                }

                List<GraphNode<GameObject>> neighbors = graph.Neighbors(currentId);
                foreach (GraphNode<GameObject> n in neighbors)
                {
                    float cost = graph.GetEdge(currentId, n.Id).Weight;

                    if (closedODict.Contains(n.Id))
                        continue;

                    if (GameplayManager.Instance.blockedOffNodes.Contains(n.Data.GetComponent<MBGraphNode>()) && !blockedOffNodesAreAllowed)
                        continue;

                    if (graph[n.Id].Data.GetComponentInChildren<OilSpillManager>() != null)
                        cost += 4; // hard coded oil spill cost

                    float gNeighbor = gnDict[currentId] + cost;
                    if (!gnDict.ContainsKey(n.Id) || gNeighbor < gnDict[n.Id])
                    {
                        pathDict[n.Id] = currentId;
                        gnDict[n.Id] = gNeighbor;
                        float hNeighbor = heuristic(n.Data.transform, graph[goalId].Data.transform);
                        fnDict[n.Id] = gNeighbor + hNeighbor;

                        openPQ.Push(fnDict[n.Id], n.Id, hNeighbor);

                        if (openPQ.Top.Key > fnDict[n.Id])
                            closedODict[currentId] = true;
                    }
                }
            }

            if (!solutionFound && closedODict.Contains(goalId))
                solutionFound = true;

            if (solutionFound)
            {
                path = new List<GraphNode<GameObject>>();

                string key = goalId;
                while (key != null && pathDict.ContainsKey(key))
                {
                    path.Add(graph[key]);
                    key = pathDict[key];
                }

                path.Reverse();
            }

            return path;
        }
    }
}
