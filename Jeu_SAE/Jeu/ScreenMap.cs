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
    public enum TypeCollisionMap { Rien, Obstacle, PorteVersPiece1, PorteVersPiece3, PorteVersPiece2 };

    class ScreenMap : GameScreen
    {
        //déclarations variables, ojb, ...
        private Game1 game;
        private TiledMap _map;
        private TiledMapRenderer _renduMap;
        private TiledMapTileLayer _coucheObstacle;
        private string _nomMap;
        private string _nomCoucheObstacle;
        private List<Perso> lesPersoADessiner;
        //private *liste* persos[]

        //taille fenêtre
        public static int WIDTH_FENETRE = 800;
        public static int HEIGHT_FENETRE = 640;

        public TiledMap Map
        {
            get
            {
                return this._map;
            }

            set
            {
                this._map = value;
            }
        }

        public TiledMapTileLayer CoucheObstacle
        {
            get
            {
                return this._coucheObstacle;
            }

            set
            {
                this._coucheObstacle = value;
            }
        }

        public ScreenMap(Game1 _game, string _nomMap, string _nomCoucheObstacle, List<Perso> _listePersoADessiner) : base(_game)
        {
            this._nomMap = _nomMap;                             //récupère nom map
            this._nomCoucheObstacle = _nomCoucheObstacle;       //récupère nom couche obstacle
            this.game = _game;                                  //récup obj map 
            this.lesPersoADessiner = _listePersoADessiner;          //récup les persos
        }

        public override void Initialize()
        {
            //changement taille fenetre
            this.game.Graphics.PreferredBackBufferWidth = WIDTH_FENETRE;
            this.game.Graphics.PreferredBackBufferHeight = HEIGHT_FENETRE;
            this.game.Graphics.ApplyChanges();
        }

        public override void LoadContent()
        {
            this._map = Content.Load<TiledMap>(this._nomMap);   //loading map
            this._renduMap = new TiledMapRenderer(GraphicsDevice, Map);     //objet à dessiner
            CoucheObstacle = Map.GetLayer<TiledMapTileLayer>(this._nomCoucheObstacle);  //récuperation de la couche obstacle
            base.LoadContent();
        }


        public override void Update(GameTime gameTime)
        {
            //nothing
        }
        public override void Draw(GameTime gameTime)
        {
            _renduMap.Draw();  //dessine la map
            //faire boucle sur perso pour tous les dessiner
            for (int i = 0; i < lesPersoADessiner.Count; i++)
            {
                this.game.SpriteBatch.Draw(this.lesPersoADessiner[i].SpritePerso, this.lesPersoADessiner[i].PositionPerso); //dessine les perso

            }
        }
    }
}
