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
        private int _distanceAggro;
        //chemin bot
        private Node _cheminAPrendre;

        public Bot(Vector2 _positionPerso, AnimatedSprite _spritePerso, Node cheminAPrendre)
        {
            this.SpritePerso = _spritePerso;
            PositionBot = _positionPerso;
            CheminAPrendre = cheminAPrendre;
            cheminAPrendre.Parent = cheminAPrendre;
            //inititalisations
            this.ChangementDifficulteBot(0);
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

        internal Node CheminAPrendre
        {
            get
            {
                return this._cheminAPrendre;
            }

            set
            {
                this._cheminAPrendre = value;
            }
        }

        public int DistanceAggro
        {
            get
            {
                return this._distanceAggro;
            }

            set
            {
                this._distanceAggro = value;
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
        public void ChangementDifficulteBot(int difficulte)
        {
            if (difficulte == 0)
            {
                DistanceAggro = 10;
                DegatsBot = 100;
                VitesseBot = 50;
            }
            else if (difficulte == 1)
            {
                DistanceAggro = 15;
                DegatsBot = 10;
                VitesseBot = 7;
            }
            else if (difficulte == 2)
            {
                DistanceAggro = 15;
                DegatsBot = 20;
                VitesseBot = 9;
            }
            else if (difficulte == 3)
            {
                DistanceAggro = 20;
                DegatsBot = 30;
                VitesseBot = 12;
            }
            else if (difficulte == 4)
            {
                DegatsBot = 30;
                DistanceAggro = 30;
                VitesseBot = 15;
            }
        }

        public void MoveAStar2(Node newNode, ScreenMap screen, GameTime gameTime)
        {
            Vector2 deplacement = new Vector2(0, 0);    //deplacement sprite
            Vector2 xy = XY_ToVector(screen);
            int x = (int)xy.X;
            int y = (int)xy.Y;
            Vector2 newPosition = newNode.Parent.Position;
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
            //Console.WriteLine(deplacement);
            PositionBot += (deplacement * VitesseBot) / 10;
            //jouer animation perso
            this.SpritePerso.Play(this._animation.ToString());
            this.SpritePerso.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
        public void MoveAStar( ScreenMap screen, GameTime gameTime)
        {
            Vector2 deplacement = new Vector2(0, 0);    //deplacement sprite
            Vector2 xy = XY_ToVector(screen);
            int x = (int)xy.X;
            int y = (int)xy.Y;
            Vector2 newPosition = CheminAPrendre.Parent.Position;
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
            //Console.WriteLine(deplacement);
            PositionBot += (deplacement*VitesseBot)/10;
            //jouer animation perso
            this.SpritePerso.Play(this._animation.ToString());
            this.SpritePerso.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
      
      
        
    }
}
