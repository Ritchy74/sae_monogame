using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Jeu
{
    class Gerant
    {
        public static void Start()
        {
            while (true)
            {

                //Astar.AlgoAStar(new Node(new Vector2(3, 3)));
                int nbjoueur = 0;
                using (var menu = new Menu())
                {
                    menu.Run();
                    if (menu.OnePlayerBool)
                        nbjoueur = 1;
                        //Console.WriteLine("1 joueur");
                    if (menu.TwoPlayersBool)
                        nbjoueur = 2;
                        //Console.WriteLine("2 joueurs");
                }


                using (var game = new Game1())
                {
                    game.NbrPerso = nbjoueur;
                    game.Run();

                }
            }
        }
    }
}
