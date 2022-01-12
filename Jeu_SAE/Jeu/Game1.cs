﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System;
using System.Collections.Generic;


namespace Jeu
{
    public enum Ecran { Piece1, Piece2, Piece3 };
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //listes
        private List<Perso> _listePerso = new List<Perso>();                            //perso
        private List<Bot> _listeBots = new List<Bot>();                            //bots
        private List<TypeControl> _listeTypeControlePerso = new List<TypeControl>();    //façon de controler les perso
        private List<ScreenMap> _listeScreenMap = new List<ScreenMap>();                //screens
        private List<Vector2> _listeVecteursSpawnParMap = new List<Vector2>();          //point de respawn par map
        //nbr perso
        private int _nbrPerso;
        //perso1
        private AnimatedSprite _spritePerso1;
        private Perso _perso1;
        //perso2
        private AnimatedSprite _spritePerso2;
        private Perso _perso2;
        //perso bot test
        private AnimatedSprite _spritePersoBotTest;
        private Bot _persoBotTest;
        //collision
        TypeCollisionMap _isCollisionSpeciale;
        //screens
        private ScreenMap _screenMapPiece1; //screen principal
        private ScreenMap _screenMapPiece2;   //screen pièce2
        private ScreenMap _screenMapPiece3;   //screen pièce2
        private Ecran _ecranEnCours;            //screen actuel (nom pour comparer)
        //manager
        private readonly ScreenManager _screenManager;
        //timer
        private float deltaSeconds;
        private float _timer;
        //dead  
        private List<bool> _listeStartCompteurDead = new List<bool>();    //mettre en liste pour tous persos
        private List<float> _listeCompteurDead = new List<float>();


        public SpriteBatch SpriteBatch
        {
            get
            {
                return this._spriteBatch;
            }

            set
            {
                this._spriteBatch = value;
            }
        }
        public GraphicsDeviceManager Graphics
        {
            get
            {
                return this._graphics;
            }

            set
            {
                this._graphics = value;
            }
        }

        public int NbrPerso
        {
            get
            {
                return this._nbrPerso;
            }

            set
            {
                this._nbrPerso = value;
            }
        }

        public Game1()
        {
            //initialisation trucs basiques
            _graphics = new GraphicsDeviceManager(this);    //fenetre
            Content.RootDirectory = "Content";              //dossier content
            IsMouseVisible = true;                          //souris visible
            _screenManager = new ScreenManager();           //screen manager
            Components.Add(_screenManager);                 //
        }

        protected override void Initialize()
        {
            _timer = 100;   //temps de jeu total
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //creation sprites
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteSheet animation1 = Content.Load<SpriteSheet>("motw.sf", new JsonContentLoader());  //importation animation1
            SpriteSheet animation2 = Content.Load<SpriteSheet>("joueur.sf", new JsonContentLoader());  //importation animation1
            _spritePerso1 = new AnimatedSprite(animation2);        //sprite anime1 pour perso
            _spritePerso2 = new AnimatedSprite(animation2);        //sprite anime1 pour perso
            _spritePersoBotTest = new AnimatedSprite(animation1);        //sprite anime2 pour perso bot test


            //creation perso
            CreationPersos();
            //creation bots
            CreationBots();
            //creation maps
            CreationMaps();

            //on initialise tout sur l'ecran principal
            _screenManager.LoadScreen(_listeScreenMap[0], new FadeTransition(GraphicsDevice, Color.Black));
            _ecranEnCours = Ecran.Piece1;
            //initialisation screen principal
            _listeScreenMap[(int)_ecranEnCours].Initialize();
            _listeScreenMap[(int)_ecranEnCours].LoadContent();
        }

        public void Time()
        {
            _timer -= deltaSeconds;
            //Console.WriteLine((int)_timer);
            if (_timer <= 0)
                Exit();
        }
        public void IsCollisionBot(float deltaSecond)
        {
            for (int i = 0; i< _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner.Count; i++)
            {
                //Console.WriteLine(_listeScreenMap[(int)_ecranEnCours].LesBotsADessiner.Count);
                for (int j = 0; j< _listeScreenMap[(int)_ecranEnCours].LesPersoADessiner.Count; j++)
                {
                    Perso persoActuel = _listeScreenMap[(int)_ecranEnCours].LesPersoADessiner[j];
                    Bot botActuel = _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner[i];
                    //Console.WriteLine(botActuel);
                    if (Bot.IsColliBot_Play(botActuel, persoActuel))
                    {
                        persoActuel.PtDeVie -= deltaSecond*50;
                        Console.WriteLine("Point de vie perso "+j+" : "+(int)persoActuel.PtDeVie);
                    }
                    if (persoActuel.PtDeVie <= 0)
                        _listeStartCompteurDead[j] = true;
                    //compteur dead
                    if (_listeStartCompteurDead[j])
                    {
                        persoActuel.Animation = Perso.TypeAnimation.dead;    //passe en dead
                        //Console.WriteLine($"COLLISION JOUEUR {j} AVEC BOT");
                        _listeCompteurDead[j] += deltaSeconds;
                        Console.WriteLine($"TEMPS MORT JOUEUR {j}: {(int)_listeCompteurDead[j]}");
                        if (_listeCompteurDead[j] >= 5)
                        {
                            _listePerso.Remove(persoActuel);
                            _listeStartCompteurDead.Remove(_listeStartCompteurDead[j]);
                            _listeCompteurDead.Remove(_listeCompteurDead[j]);
                        }
                    }

                }
            }
          
        }
        protected override void Update(GameTime gameTime)
        {
            //quit game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || _listePerso.Count==0)
                Exit();
            //deltatime
            deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;     
            //faire écouler le timer dans le jeu 
            Time();     
            //collision bot
            IsCollisionBot(deltaSeconds);

            _isCollisionSpeciale = TypeCollisionMap.Rien;   //réinitialisation des colision
            for (int i = 0; i < _listePerso.Count; i++)
            {
                //récupérationdu type de colision
                if (_listePerso[i].Collision != TypeCollisionMap.Rien)
                    _isCollisionSpeciale = _listePerso[i].Collision;

                //update position des perso
                _listePerso[i].Move(_listeScreenMap[(int)_ecranEnCours], gameTime, _listeTypeControlePerso[i]);
            }


            //changement vers piece 1
            if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece1)
                ChangementScreen(Ecran.Piece1);

            //changement vers piece 2
            else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece2)
                ChangementScreen(Ecran.Piece2);

            //changement vers piece 3
            else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece3)
                ChangementScreen(Ecran.Piece3);



            _listeBots[0].Move(_listeScreenMap[(int)_ecranEnCours], gameTime, TypeControl.clavier_IJKL);
            base.Update(gameTime);

        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin();
            base.Draw(gameTime);    //dessine objets
            SpriteBatch.End();
        }
        public void CreationBots()
        {
            //creation bot
            _persoBotTest = new Bot(new Vector2(100, 50), _spritePersoBotTest);
            //ajout des bots à la liste
            _listeBots.Add(_persoBotTest);

        }
        public void CreationPersos()    //génération de tout ce qui tourne autour des perso
        {
            //creation persos
            _perso1 = new Perso(new Vector2(50, 175), _spritePerso1);   //creation perso1
            _perso2 = new Perso(new Vector2(120, 230), _spritePerso2);    //creation perso2
            //ajout des perso à la liste
            _listePerso.Add(_perso1);   //perso1
            if (NbrPerso==2)
                _listePerso.Add(_perso2);   //perso2
            //ajout des types de controles à la liste
            _listeTypeControlePerso.Add(TypeControl.Clavier_HBGD);  //haut,bas,gauche,droite
            if (NbrPerso == 2)
                _listeTypeControlePerso.Add(TypeControl.Clavier_ZQSD);  //Z,Q,S,D
            //initialisation des compteurs de mort pour les persos
            for (int i =0; i<_listePerso.Count;i++)
            {
                _listeStartCompteurDead.Add(false);
                _listeCompteurDead.Add(0);
            }
        }
        public void CreationMaps()  //génération de tout ce qui tourne autour des maps
        {
            _screenMapPiece1 = new ScreenMap(this, "mansion_maps_version1/Piece_1", "obstacles", 320, 320);              //creation map1
            _screenMapPiece2 = new ScreenMap(this, "mansion_maps_version1/Piece_2", "obstacles", 170, 240);              //creation map2
            _screenMapPiece3 = new ScreenMap(this, "mansion_maps_version1/Piece_3", "obstacles", 190, 250);              //creation map2
            //ajout des maps à la liste
            _listeScreenMap.Add(_screenMapPiece1);      //ajout map1
            _listeScreenMap.Add(_screenMapPiece2);      //ajout map2
            _listeScreenMap.Add(_screenMapPiece3);      //ajout map3
            //ajout des vecteurs par piece
            _listeVecteursSpawnParMap.Add(new Vector2(50, 175));     //ajout vecteur map1 
            _listeVecteursSpawnParMap.Add(new Vector2(90, 180));     //ajout vecteur map2
            _listeVecteursSpawnParMap.Add(new Vector2(40, 40));     //ajout vecteur map3
            for (int i = 0; i < _listeScreenMap.Count; i++)
            {
            _listeScreenMap[i].UpdateListJoueursAAfficher(_listePerso);
            }
            _listeScreenMap[0].UpdateListBotsAAfficher(_listeBots);
        }
        public void ReinitialisationPosition(Ecran ecran)
        {
            for (int i = 0; i < _listePerso.Count; i++)
            {
                _listePerso[i].PositionPerso = _listeVecteursSpawnParMap[(int)ecran];  //reinitialise position perso
                _listePerso[i].Collision = TypeCollisionMap.Rien;      //reinitialise les collisions pour pas etre bloqué
            }
        }
        public void ChangementScreen(Ecran versCetEcran)
        {
            Console.WriteLine($"CHARGEMENT  {versCetEcran.ToString()}");
            ReinitialisationPosition(versCetEcran);
            //_listeScreenMap[(int)_ecranEnCours].UpdateListBotsAAfficher(new List<Bot>());
            _ecranEnCours = versCetEcran;                   //changement enum ecran
            //_listeScreenMap[(int)_ecranEnCours].UpdateListBotsAAfficher(_listeBots);
            _screenManager.LoadScreen(_listeScreenMap[(int)_ecranEnCours]);            //chargement nouvelle map
        }
    }
}
