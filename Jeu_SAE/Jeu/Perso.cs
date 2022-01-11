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
    public enum CategoriePersonnage { Joueur, Bot };
    class Perso
    {
        //déclarations
        public enum TypeAnimation { walkSouth, walkNorth, walkEast, walkWest, idle, dead };   //directions perso pour animation

        private int _vitessePerso;
        private float walkSpeed;
        private Vector2 _positionPerso;
        private TypeCollisionMap _collision;
        private TypeAnimation _animation;
        AnimatedSprite _spritePerso;

        public Perso(Vector2 _positionPerso, AnimatedSprite _spritePerso)
        {
            this.SpritePerso = _spritePerso;
            PositionPerso = _positionPerso;
            
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

        public Vector2 PositionPerso        //publique pour Screen.Draw()
        {
            get
            {
                return this._positionPerso;
            }

            set
            {
                this._positionPerso = value;
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

        internal TypeAnimation Animation
        {
            get
            {
                return this._animation;
            }

            set
            {
                this._animation = value;
            }
        }

        public void Move(ScreenMap screen, GameTime gameTime, TypeControl typeDeControle)
        {
            if (Animation != TypeAnimation.dead)
            { 

                // translation de la position du personnage en pixel en ligne et colonne pour la matrice
                float positionColonnePerso = (PositionPerso.X / screen.Map.TileWidth);
                float positionLignePerso = (PositionPerso.Y / screen.Map.TileHeight);
                //réinitialisation
                Vector2 deplacement = new Vector2(0, 0);    //deplacement sprite
                this.Animation = TypeAnimation.idle;       //position immobile
                bool toucheBordFenetre = false;             //collision avec bord fenetre
                float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;      //deltatime
                this.walkSpeed = deltaSeconds * _vitessePerso;                         //vitesse de deplacement du sprite
                //récupère état direction du perso
                DirectionEntite directionPerso = Controller.ReadClavier((int)typeDeControle);

                //deplacement gauche
                if (directionPerso == DirectionEntite.Left)
                {
                    this.Animation = TypeAnimation.walkWest;   //animation
                    deplacement = new Vector2(-1, 0);           //vecteur deplacement
                    //collision
                    toucheBordFenetre = PositionPerso.X - this.SpritePerso.TextureRegion.Width / 2 <= 0;    //gauche de fenetre 
                    Collision = Perso.IsCollision(positionColonnePerso - 1, positionLignePerso, screen);     //batiment
                }
                //deplacement droite
                if (directionPerso == DirectionEntite.Right)
                {
                    this.Animation = TypeAnimation.walkEast;   //animation
                    deplacement = new Vector2(+1, 0);           //vecteur deplacement

                    //collision
                    toucheBordFenetre = PositionPerso.X + this.SpritePerso.TextureRegion.Width / 2 >= screen.GraphicsDevice.Viewport.Width;    //droite de fenetre 
                    Collision = Perso.IsCollision(positionColonnePerso + 1, positionLignePerso, screen);     //batiment

                }
                //deplacement haut
                if (directionPerso == DirectionEntite.Up)
                {
                    this.Animation = TypeAnimation.walkNorth;  //animation
                    deplacement = new Vector2(0, -1);           //vecteur deplacement
                    //collision
                    toucheBordFenetre = PositionPerso.Y - this.SpritePerso.TextureRegion.Height / 2 <= 0;    //haut de fenetre 
                    Collision = Perso.IsCollision(positionColonnePerso, positionLignePerso - 1, screen);     //batiment

                }
                //deplacement bas
                if (directionPerso == DirectionEntite.Down)
                {
                    this.Animation = TypeAnimation.walkSouth;  //animation
                    deplacement = new Vector2(0, +1);           //vecteur deplacement

                    //collision
                    toucheBordFenetre = PositionPerso.Y + this.SpritePerso.TextureRegion.Height / 2 >= screen.GraphicsDevice.Viewport.Height;    //bas de fenetre 
                    Collision = Perso.IsCollision(positionColonnePerso, positionLignePerso + 2, screen);     //batiment

                }
                //si pas de collision alors on avance
                if (Collision == TypeCollisionMap.Rien && !toucheBordFenetre )
                    PositionPerso += walkSpeed * deplacement;
            }

            //jouer animation perso
            this.SpritePerso.Play(this.Animation.ToString());
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
