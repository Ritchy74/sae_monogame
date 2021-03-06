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
    class Bot
    {
        //déclarations
        public enum TypeAnimation { walkSouth, walkNorth, walkEast, walkWest, idle, monstreAttack, };   //directions perso pour animation
        private float walkSpeed;
        private Vector2 _positionBot;
        private TypeCollisionMap _collision;
        private TypeAnimation _animation;
        AnimatedSprite _spritePerso;
        //diff bot
        private int _degatsBot;
        private int _vitesseBot;

        public Bot(Vector2 _positionPerso, AnimatedSprite _spritePerso)
        {
            this.SpritePerso = _spritePerso;
            PositionBot = _positionPerso;

            //inititalisations
            this.ChangementDifficulteBot(0);
        }

        
        public void ChangementDifficulteBot(int difficulte)
        {
            if (difficulte == 0)
            {
                DegatsBot = 0;
                VitesseBot = 10;
            }
            else if (difficulte==1)
            {
                DegatsBot = 10;
                VitesseBot = 50;
            }
            else if (difficulte == 2)
            {
                DegatsBot = 20;
                VitesseBot = 50;
            }
            else if (difficulte == 3)
            {
                DegatsBot = 30;
                VitesseBot = 70;
            }
            else if (difficulte == 4)
            {
                DegatsBot = 50;
                VitesseBot = 110;
            }
        }

        
        public TypeCollisionMap Collision       //publique pour chamgements de scènes
        {
            get
            {
                return this._collision;
            }

            set
            {
                this._collision = value;
            }
        }

        public Vector2 PositionBot        //publique pour Screen.Draw()
        {
            get
            {
                return this._positionBot;
            }

            set
            {
                this._positionBot = value;
            }
        }

        public AnimatedSprite SpritePerso
        {
            get
            {
                return this._spritePerso;
            }

            set
            {
                this._spritePerso = value;
            }
        }

        public int DegatsBot
        {
            get
            {
                return this._degatsBot;
            }

            set
            {
                this._degatsBot = value;
            }
        }

        public int VitesseBot
        {
            get
            {
                return this._vitesseBot;
            }

            set
            {
                this._vitesseBot = value;
            }
        }

        public static bool IsColliBot_Play(Bot leBot, Perso lePerso)
        {
            bool res = false;
            int HTB = 10;
            Rectangle rect1 = new Rectangle((int)leBot.PositionBot.X,(int)leBot.PositionBot.Y,52,72);
            Rectangle rect2 = new Rectangle((int)lePerso.PositionPerso.X+ HTB, (int)lePerso.PositionPerso.Y+ HTB, 48- 2*HTB, 64- 2*HTB);
            if (rect1.Intersects(rect2))
                res = true;
            return res;
        }
        public Vector2 XY_ToVector(ScreenMap screen)
        {
            int x = (int)(PositionBot.X / screen.Map.TileWidth);
            int y = (int)(PositionBot.Y / screen.Map.TileHeight);
            return new Vector2(x, y);
        }
        public void MoveAStar(Vector2 newPosition, ScreenMap screen, GameTime gameTime)
        {
            Vector2 deplacement = new Vector2(0, 0);    //deplacement sprite
            int x = (int)XY_ToVector(screen).X;
            int y = (int)XY_ToVector(screen).Y;
            //Console.WriteLine(x +" / "+ newPosition.X);
            //Console.WriteLine(y +" / "+ newPosition.Y);
            if (x < newPosition.X)
            {
                this._animation = TypeAnimation.walkEast;   //animation
                deplacement = new Vector2(1, 0);           //vecteur deplacement
            }
            else if (x > newPosition.X)
            {
                this._animation = TypeAnimation.walkWest;   //animation
                deplacement = new Vector2(-1, 0);           //vecteur deplacement
            }
            else if (y < newPosition.Y)
            {
                this._animation = TypeAnimation.walkSouth;   //animation
                deplacement = new Vector2(0, 1);           //vecteur deplacement
            }
            else
            {
                this._animation = TypeAnimation.walkNorth;   //animation
                deplacement = new Vector2(0, -1);           //vecteur deplacement
            }
            Console.WriteLine(deplacement);
            PositionBot += deplacement;
            //jouer animation perso
            this.SpritePerso.Play(this._animation.ToString());
            this.SpritePerso.Update((float)gameTime.ElapsedGameTime.TotalSeconds);


            //PositionBot = new Vector2(x, y);
        }
      
      
        
    }
}
