using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Threading;

namespace Jeu
{
    class Astar
    {

        public static List<Node> RecupChemin(Node depart, Node dernierNoeud)
        {
            Node noeudActuel = dernierNoeud;
            List<Node> listChemin = new List<Node>();
            while (true)
            {
                listChemin.Add(noeudActuel);
                if (noeudActuel == depart)
                {
                    foreach (Node nodePrint in listChemin)
                        Console.WriteLine(nodePrint.Position);
                    return listChemin;
                }
                noeudActuel = noeudActuel.Parent;

            }
        }
        public static void AlgoAStar(Node depart)
        {
            List<Node> Open = new List<Node>();
            List<Node> Closed = new List<Node>();
            Node target = new Node(new Vector2(2, 2));
            Open.Add(depart);
            int count = 0;
            while (true)
            {
                Node current = Open[0];
                count++;
                Console.WriteLine("New boucle");
                int i_lowest = 0;
                for (int i = 0; i < Open.Count; i++)
                {
                    Console.WriteLine(Open[i].Position + " / " + Open[i].FCost);
                    //Thread.Sleep(1000);
                    if (Open[i].FCost <= Open[i_lowest].FCost)
                        i_lowest = i;
                }
                current = Open[i_lowest];
                Console.WriteLine("current: " + current.Position + " / " + current.FCost);
                //Thread.Sleep(2000);
                Open.Remove(current);
                Closed.Add(current);

                if (current == target)
                {
                    RecupChemin(depart, current);
                    Console.WriteLine("fini, cost : " + count);
                    return;
                }

                foreach (Node voisins in current.GetVoisins())
                {
                    if (Closed.IndexOf(voisins) != -1)
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
