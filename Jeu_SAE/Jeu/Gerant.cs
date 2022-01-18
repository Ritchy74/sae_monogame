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
            int jouerOuMenu = 1;
            int nbjoueur = 0;
            while (true)
            {
                if (jouerOuMenu == 1)
                {
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
                    jouerOuMenu = 0;
                }
                if (nbjoueur == 0)
                    break;
                else
                {

                    using (var game = new Game1())
                    {
                        game.NbrPerso = nbjoueur;
                        game.Run();
                    }
                    using (var menu_mort = new Menu_Mort())
                    {
                        menu_mort.Run();
                        if (menu_mort.OnePlayerBool)
                            jouerOuMenu = 0;
                        else if (menu_mort.TwoPlayersBool)
                            jouerOuMenu = 1;
                        else
                            break;
                    }
                }

            }
        }
    }
}
