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
    public enum TypeCollisionMap { Rien, Obstacle, PorteVersPiece0, PorteVersPiece1_bas, PorteVersPiece1_basGauche, PorteVersPiece1_hautGauche, PorteVersPiece1_hautDroite, PorteVersPiece2_bas, PorteVersPiece2_haut };

    class ScreenMap : GameScreen
    {
        //déclarations variables, ojb, ...
        private Game1 game;
        private TiledMap _map;
        private TiledMapRenderer _renduMap;
        private TiledMapTileLayer _coucheObstacle;
        private string _nomMap;
        private string _nomCoucheObstacle;
        private List<Perso> _lesPersoADessiner;
        private List<Bot> _lesBotsADessiner;
        private Cle _cleADessiner;
        private Journal _journalADessiner;
        //window
        private int _widthFenetre;
        private int _heightFenetre;
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

        internal List<Perso> LesPersoADessiner
        {
            get
            {
                return this._lesPersoADessiner;
            }

            set
            {
                this._lesPersoADessiner = value;
            }
        }

        internal List<Bot> LesBotsADessiner
        {
            get
            {
                return this._lesBotsADessiner;
            }

            set
            {
                this._lesBotsADessiner = value;
            }
        }

        public ScreenMap(Game1 game, string nomMap, string nomCoucheObstacle, int widthFenetre, int heightFenetre, Cle cle, Journal journal) : base(game)
        {
            this._nomMap = nomMap;                             //récupère nom map
            this._nomCoucheObstacle = nomCoucheObstacle;       //récupère nom couche obstacle
            this.game = game;                                  //récup obj map 
            this._widthFenetre = widthFenetre;
            this._heightFenetre = heightFenetre;
            this.LesPersoADessiner = new List<Perso>();          //récup les persos
            this.LesBotsADessiner = new List<Bot>();
            this._cleADessiner = cle;
            this._journalADessiner = journal;
        }

        public override void Initialize()
        {
            //changement taille fenetre
            this.game.Graphics.PreferredBackBufferWidth = this._widthFenetre;
            this.game.Graphics.PreferredBackBufferHeight = this._heightFenetre;
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
        public void UpdateListBotsAAfficher(List<Bot> listeBot)
        {
            this.LesBotsADessiner = listeBot;
        }
        public void UpdateListJoueursAAfficher(List<Perso> listeJoueur)
        {
            this.LesPersoADessiner = listeJoueur;
        }
        public override void Draw(GameTime gameTime)
        {
            _renduMap.Draw();  //dessine la map
            //faire boucle sur perso pour tous les dessiner
            for (int i = 0; i < LesBotsADessiner.Count; i++)
            {
                this.game.SpriteBatch.Draw(this.LesBotsADessiner[i].SpritePerso, this.LesBotsADessiner[i].PositionBot); //dessine les bots
            }
            for (int i = 0; i < LesPersoADessiner.Count; i++)
            {
                this.game.SpriteBatch.Draw(this.LesPersoADessiner[i].SpritePerso, this.LesPersoADessiner[i].PositionPerso); //dessine les perso
            }
            if (!this._cleADessiner.IsPrise && this._journalADessiner.IsPrise)
            {
                this.game.SpriteBatch.Draw(this._cleADessiner.SpriteCle, this._cleADessiner.PositionCle) ; //dessine la clé
            }
            if (!this._journalADessiner.IsPrise)
            {
                this.game.SpriteBatch.Draw(this._journalADessiner.SpriteJournal, this._journalADessiner.PositionJournal); //dessine le journal
            }
        }
    }
}
