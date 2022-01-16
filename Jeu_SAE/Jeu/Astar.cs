using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Jeu
{
    class Astar
    {
        //public static int GetLongueurChemin(Node arrive)
        //{
        //    return arrive.P
        //}
        public static Node AlgoAStar(Node departPerso, Node arriveBot, ScreenMap map)
        {
            //listes des noeuds ouverts / déjà vérifiés
            List<Node> Open = new List<Node>();     
            List<Node> Closed = new List<Node>();

            Open.Add(departPerso);      //on part du perso principal pour aller vers le bot
            Node target = arriveBot;    //on set l'arrivée sur le bot

            while (true)
            {
                Node current = Open[0];     //on remet le current au premier noeud de la liste

                //selection du noeud qui a le plus faible f_cost
                int i_lowest = 0;
                for (int i = 0; i < Open.Count; i++)
                {
                    if (Open[i].FCost <= Open[i_lowest].FCost)
                        i_lowest = i;
                }
                current = Open[i_lowest];

                Open.Remove(current);   //on suppr le current de l'open car on l'a traité
                Closed.Add(current);    //et on l'ajt donc à closed

                //on termine si on est arrivé jusqu'au target
                if (current == target)
                {
                    if (current.Parent is null)     //pour éviter de bug, si le bot est déjà sur le perso
                        current.Parent = current;   //on créé un parent
                    return current;
                }

                //boucle sur chaque voisins
                foreach (Node voisins in current.GetVoisins())
                {
                    if (!current.IsTraversable(map) || Closed.IndexOf(voisins) != -1)   //si le noeud est un obstacle ou qu'il a déjà été traité
                        continue;                                                       //on passe au voisin suivant

                    if (Open.IndexOf(voisins) == -1 || voisins.CalculCost(current, target) < voisins.HCost)     //si il n'est pas dans la liste open ou qu'il l'est déjà mais que son cout par ce chemin est plus faible
                    {
                        voisins.SetFCost(current, target);  //on re set la f_cost
                        //et s'il n'est pas dans open, on le rajoute
                        if (Open.IndexOf(voisins) == -1)    
                            Open.Add(voisins);
                    }

                }

            }

        }
    }
}
