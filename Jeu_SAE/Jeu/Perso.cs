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
        public enum TypeAnimation { walkSouth, walkNorth, walkEast, walkWest, idle, dead, idleDead };   //directions perso pour animation

        private int _vitessePerso;
        private float walkSpeed;
        private Vector2 _positionPerso;
        private TypeCollisionMap _collision;
        private TypeAnimation _animation;
        AnimatedSprite _spritePerso;
        private float _ptDeVie;
        private TypeControl _typeDeControl;
        private bool _isInPlacard;

        public Perso(Vector2 _positionPerso, AnimatedSprite _spritePerso, TypeControl typeDeControle)
        {
            //sprites
            this.SpritePerso = _spritePerso;
            PositionPerso = _positionPerso;
            
            //inititalisations
            this._vitessePerso = 200;
            TypeDeControl = typeDeControle;
            IsInPlacard = false;
            PtDeVie = 100;
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

        public float PtDeVie
        {
            get
            {
                return this._ptDeVie;
            }

            set
            {
                this._ptDeVie = value;
            }
        }

        public TypeControl TypeDeControl
        {
            get
            {
                return this._typeDeControl;
            }

            set
            {
                this._typeDeControl = value;
            }
        }

        public bool IsInPlacard
        {
            get
            {
                return this._isInPlacard;
            }

            set
            {
                this._isInPlacard = value;
            }
        }

        public Vector2 XY_ToVector(ScreenMap screen)
        {
            int x = (int)(PositionPerso.X / screen.Map.TileWidth);
            int y = (int)(PositionPerso.Y / screen.Map.TileHeight);
            return new Vector2(x, y);
        }
        public void Move(ScreenMap screen, GameTime gameTime, TypeControl typeDeControle, Rectangle collision)
        {
            if (Animation != TypeAnimation.dead && Animation != TypeAnimation.idleDead)
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
                    toucheBordFenetre = PositionPerso.X  <= 0;    //gauche de fenetre 
                    Collision = Perso.IsCollision(positionColonnePerso - 1, positionLignePerso, screen);     //batiment
                }
                //deplacement droite
                if (directionPerso == DirectionEntite.Right)
                {
                    this.Animation = TypeAnimation.walkEast;   //animation
                    deplacement = new Vector2(+1, 0);           //vecteur deplacement

                    //collision
                    toucheBordFenetre = PositionPerso.X >= screen.GraphicsDevice.Viewport.Width;    //droite de fenetre 
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

            Rectangle rectPerso = new Rectangle((int)PositionPerso.X, (int)PositionPerso.Y+15, 37, 50);
                if (collision.Intersects(rectPerso))
                    PositionPerso -= walkSpeed * deplacement;
            }

            //jouer animation perso
            this.SpritePerso.Play(this.Animation.ToString());
            this.SpritePerso.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
        public static TypeCollisionMap IsCollision(float x, float y, ScreenMap map)
        {
            List<int> Tiles_Speciales = new List<int> { 272, 312, 313, 314, 356, 358, 74, 73, 72, 158, 359, 357};  //indice = numéro de pièce

            TypeCollisionMap collision = TypeCollisionMap.Rien;
            TiledMapTile? tile;
            TiledMapTileLayer coucheObstacle = map.CoucheObstacle;

            if (coucheObstacle.TryGetTile((ushort)x, (ushort)y, out tile))
            {
                if (!tile.Value.IsBlank)
                {
                    //Console.WriteLine(tile.Value.GlobalIdentifier);   //numéro de tile actuel
                    if (tile.Value.GlobalIdentifier == Tiles_Speciales[0])
                        collision = TypeCollisionMap.PorteVersPiece0;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[1])
                        collision = TypeCollisionMap.PorteVersPiece1_bas;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[2])
                        collision = TypeCollisionMap.PorteVersPiece1_basGauche;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[3])
                        collision = TypeCollisionMap.PorteVersPiece1_hautGauche;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[4])
                        collision = TypeCollisionMap.PorteVersPiece1_milieu;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[5])
                        collision = TypeCollisionMap.PorteVersPiece1_hautDroite;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[6])
                        collision = TypeCollisionMap.PorteVersPiece2_bas;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[7])
                        collision = TypeCollisionMap.PorteVersPiece2_haut;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[8])
                        collision = TypeCollisionMap.PorteVersPiece3_bas;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[9])
                        collision = TypeCollisionMap.PorteVersPiece3_haut;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[10])
                        collision = TypeCollisionMap.PorteVersPiece4_bas;
                    else if (tile.Value.GlobalIdentifier == Tiles_Speciales[11])
                        collision = TypeCollisionMap.PorteVersPiece4_haut;
                    else
                        collision = TypeCollisionMap.Obstacle;

                }
            }
            Console.WriteLine("collision : " + collision);
            return collision;
        }
        
    }
}
