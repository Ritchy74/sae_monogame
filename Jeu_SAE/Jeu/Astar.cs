using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Jeu
{
    class Astar
    {
        public static Node AlgoAStar(Node departPerso, Node arriveBot, ScreenMap map)
        {
            
            List<Node> Open = new List<Node>();
            List<Node> Closed = new List<Node>();
            Node target = arriveBot;
            Open.Add(departPerso);
            while (true)
            {
                Node current = Open[0];
                int i_lowest = 0;
                for (int i = 0; i < Open.Count; i++)
                {
                    if (Open[i].FCost <= Open[i_lowest].FCost)
                        i_lowest = i;
                }
                current = Open[i_lowest];
                Open.Remove(current);
                Closed.Add(current);

                if (current == target)
                {
                    return current;
                }

                foreach (Node voisins in current.GetVoisins())
                {
                    if (!current.IsTraversable(map) || Closed.IndexOf(voisins) != -1)
                        continue;

                    if (Open.IndexOf(voisins) == -1 || voisins.CalculCost(current, target) < voisins.HCost)
                    {
                        voisins.SetFCost(current, target);
                        if (Open.IndexOf(voisins) == -1)
                            Open.Add(voisins);
                    }

                }

            }

        }
    }
}
