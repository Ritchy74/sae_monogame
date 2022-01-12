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
        public enum TypeAnimation { walkSouth, walkNorth, walkEast, walkWest, idle };   //directions perso pour animation
        private int _vitessePerso;
        private float walkSpeed;
        private Vector2 _positionBot;
        private TypeCollisionMap _collision;
        private TypeAnimation _animation;
        AnimatedSprite _spritePerso;


        public Bot(Vector2 _positionPerso, AnimatedSprite _spritePerso)
        {
            this.SpritePerso = _spritePerso;
            PositionBot = _positionPerso;

            //inititalisations
            this._vitessePerso = 200;
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
        public void Move(ScreenMap screen, GameTime gameTime, TypeControl typeDeControle)
        {
            // translation de la position du personnage en pixel en ligne et colonne pour la matrice
            float positionColonnePerso = (PositionBot.X / screen.Map.TileWidth);
            float positionLignePerso = (PositionBot.Y / screen.Map.TileHeight);
            //réinitialisation
            Vector2 deplacement = new Vector2(0, 0);    //deplacement sprite
            this._animation = TypeAnimation.idle;       //position immobile
            bool toucheBordFenetre = false;             //collision avec bord fenetre
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;      //deltatime
            this.walkSpeed = deltaSeconds * _vitessePerso;                         //vitesse de deplacement du sprite
            //récupère état direction du perso
            DirectionEntite directionPerso = Controller.ReadClavier((int)typeDeControle);

            //deplacement gauche
            if (directionPerso == DirectionEntite.Left)
            {
                this._animation = TypeAnimation.walkWest;   //animation
                deplacement = new Vector2(-1, 0);           //vecteur deplacement
                //collision
                toucheBordFenetre = PositionBot.X - this.SpritePerso.TextureRegion.Width / 2 <= 0;    //gauche de fenetre 
                Collision = Bot.IsCollision(positionColonnePerso - 1, positionLignePerso, screen);     //batiment
            }
            //deplacement droite
            if (directionPerso == DirectionEntite.Right)
            {
                this._animation = TypeAnimation.walkEast;   //animation
                deplacement = new Vector2(+1, 0);           //vecteur deplacement

                //collision
                toucheBordFenetre = PositionBot.X + this.SpritePerso.TextureRegion.Width / 2 >= screen.GraphicsDevice.Viewport.Width;    //droite de fenetre 
                Collision = Bot.IsCollision(positionColonnePerso + 1, positionLignePerso, screen);     //batiment

            }
            //deplacement haut
            if (directionPerso == DirectionEntite.Up)
            {
                this._animation = TypeAnimation.walkNorth;  //animation
                deplacement = new Vector2(0, -1);           //vecteur deplacement
                //collision
                toucheBordFenetre = PositionBot.Y - this.SpritePerso.TextureRegion.Height / 2 <= 0;    //haut de fenetre 
                Collision = Bot.IsCollision(positionColonnePerso, positionLignePerso - 1, screen);     //batiment

            }
            //deplacement bas
            if (directionPerso == DirectionEntite.Down)
            {
                this._animation = TypeAnimation.walkSouth;  //animation
                deplacement = new Vector2(0, +1);           //vecteur deplacement

                //collision
                toucheBordFenetre = PositionBot.Y + this.SpritePerso.TextureRegion.Height / 2 >= screen.GraphicsDevice.Viewport.Height;    //bas de fenetre 
                Collision = Bot.IsCollision(positionColonnePerso, positionLignePerso + 2, screen);     //batiment

            }

            //si pas de collision alors on avance
            if (Collision == TypeCollisionMap.Rien && !toucheBordFenetre)
                PositionBot += walkSpeed * deplacement;

            //jouer animation perso
            this.SpritePerso.Play(this._animation.ToString());
            this.SpritePerso.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
        public static TypeCollisionMap IsCollision(float x, float y, ScreenMap map)
        {
            List<int> Tiles_Speciales = new List<int> { 0, 74, 72, 73 };  //indice = numéro de pièce

            TypeCollisionMap collision = TypeCollisionMap.Rien;
            TiledMapTile? tile;
            TiledMapTileLayer coucheObstacle = map.CoucheObstacle;

            if (coucheObstacle.TryGetTile((ushort)x, (ushort)y, out tile))
            {
                if (!tile.Value.IsBlank)
                {
                    //Console.WriteLine(tile.Value.GlobalIdentifier);   //numéro de tile actuel
                    if (tile.Value.GlobalIdentifier == Tiles_Speciales[1])
                        collision = TypeCollisionMap.PorteVersPiece1;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[2])
                        collision = TypeCollisionMap.PorteVersPiece2;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[3])
                        collision = TypeCollisionMap.PorteVersPiece3;
                    else
                        collision = TypeCollisionMap.Obstacle;

                }
            }
            Console.WriteLine("collision : " + collision);
            return collision;
        }
        
    }
}
