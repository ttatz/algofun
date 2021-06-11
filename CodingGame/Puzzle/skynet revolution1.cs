using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/*********************************************************************/
/*								     */
/* https://www.codingame.com/ide/puzzle/skynet-revolution-episode-1  */
/* AUTHOR: 	ttatz						     */
/* DATE: 	20.05.2021					     */
/*								     */
/*********************************************************************/


class Player
{
    class Node
    {
        public int index;
        public List<Node> neighbours = new List<Node>(); //list of nodes
        public bool isExit { get; set; } //it's an exit node (skynet can leave)
        public Node(int index)
        {
            this.index = index;
            isExit = false;
        }

    }

    class Graph
    {
        public void ConnectNodes(Node sourceNode, Node targetNode)
        {
            sourceNode.neighbours.Add(targetNode);
        }

      

    }

    static bool BreadthFirstNodes(Node startNode, Node targetNode, ref Dictionary<Node, int> visited)
    {
        Console.Error.WriteLine($"start {startNode.index} target {targetNode.index}");
        Queue<Node> queue = new Queue<Node>();
        //enqueue first node
		queue.Enqueue(startNode);
        int distance = 0;
        visited[startNode] = distance;

        while (queue.Count > 0)
        {
            Node n = queue.Dequeue();
            distance++; //distance for eval

            //check if node reachable
            if (n == targetNode)
                return true;

            foreach (Node m in n.neighbours)
            {

                int dist = 0;
                if (!visited.TryGetValue(m, out dist))

                {
                    //add posible nodes
                    visited[m] = distance;
                    queue.Enqueue(m);
                }
            }

        }

        return false;
    }


    static void Main(string[] args)
    {
        int l = 0;
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int N = int.Parse(inputs[0]); // the total number of nodes in the level, including the gateways
        int L = int.Parse(inputs[1]); // the number of links
        int E = int.Parse(inputs[2]); // the number of exit gateways


        /**** create number of nodes ****/
        Graph g = new Graph();
        List<Node> listNodes = new List<Node>();
        for (int i = 0; i < N; i++)
        {
            listNodes.Add(new Node(i));
        }

        /***** connect nodes *****/
        for (int i = 0; i < L; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int N1 = int.Parse(inputs[0]); // N1 and N2 defines a link between these nodes
            int N2 = int.Parse(inputs[1]);
            //connect nodes bidirectional
            g.ConnectNodes(listNodes.Select(s => s).Where(w => w.index == N1).First(), listNodes.Select(s => s).Where(w => w.index == N2).First());
            g.ConnectNodes(listNodes.Select(s => s).Where(w => w.index == N2).First(), listNodes.Select(s => s).Where(w => w.index == N1).First());

            //Console.Error.WriteLine($"number of links {N1} {N2}");

        }

        /***** mark exit nodes *****/
        for (int i = 0; i < E; i++)
        {
            int EI = int.Parse(Console.ReadLine()); // the index of a gateway node
            (listNodes.Select(s => s).Where(w => w.index == EI).First()).isExit = true;
        }

        
        List<Node> exitNodes = listNodes.Select(s => s).Where(w => w.isExit == true).ToList();
		// game loop
        while (true)
        {
            int SI = int.Parse(Console.ReadLine()); // The index of the node on which the Skynet agent is positioned this turn


            Dictionary<Node, int> visited = new Dictionary<Node, int>(); //visited dir for nodes with distance
            bool removed = false;
            Node startNode = listNodes.Select(s => s).Where(w => w.index == SI).First();
            foreach (Node n in exitNodes)
            {

                BreadthFirstNodes(startNode, n, ref visited);
                //exitNodes.Remove(n);

                int dist = 0;
                visited.TryGetValue(n, out dist);
                if (dist == 1) //remove critical node immediately (skynet can leave immediatley -> stop it)
                {
                    
                    Console.WriteLine($"{startNode.index} {n.index}");
                    //remove edge
                    startNode.neighbours.Remove(n); 
                    n.neighbours.Remove(startNode);
					//mem removed
                    removed = true;
                    break;
                }

            }
			//nothing was removed, removed nearest node
            if (removed == false)
            {
                Node v = visited.Select(s => s).Where(w => w.Value > 0).OrderBy(w => w.Value).First().Key;

                
                Console.WriteLine($"{startNode.index} {v.index}");
                //remove edge
                startNode.neighbours.Remove(v);
                v.neighbours.Remove(startNode);
            }
        }
    }
}
