using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Content;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;
using System.Collections.Generic;


namespace Jeu
{
    public enum DirectionEntite { Static, Left, Right, Up, Down };  //directions possibles -> rajt diagonales?
    public enum TypeControl { Clavier_ZQSD, Clavier_HBGD };     // !!! doit imérativement faire la mm taille que tableauValueClavier !!!
    class Controller
    {
        public static DirectionEntite ReadClavier(int touche)
        {
            int[,] tableauValueClavier = { { 90, 83, 81, 68 }, { 38, 40, 37, 39 } };    //  haut/0;bas/1;gauche/2;droite/3
            KeyboardState keyboardState = Keyboard.GetState();          //recupere etat clavier 
            DirectionEntite res = DirectionEntite.Static;               //reinitialise la position en static

            if (keyboardState.IsKeyDown((Keys)tableauValueClavier[touche, 3]))
                res = DirectionEntite.Right;
            else if (keyboardState.IsKeyDown((Keys)tableauValueClavier[touche, 2]))
                res = DirectionEntite.Left;
            else if (keyboardState.IsKeyDown((Keys)tableauValueClavier[touche, 1]))
                res = DirectionEntite.Down;
            else if (keyboardState.IsKeyDown((Keys)tableauValueClavier[touche, 0]))
                res = DirectionEntite.Up;
            return res;
        }

    }
}
