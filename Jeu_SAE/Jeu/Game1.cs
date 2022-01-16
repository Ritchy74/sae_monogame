using Microsoft.Xna.Framework;
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
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;


namespace Jeu
{
    public enum Ecran { Piece0, Piece1, Piece2, Piece3 };
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //listes
        private List<Perso> _listePerso = new List<Perso>();                            //perso
        private List<Bot> _listeBots = new List<Bot>();                            //bots
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
        //monstre
        private AnimatedSprite _spriteMonstre;
        private Bot _monstre;
        //collision
        TypeCollisionMap _isCollisionSpeciale;
        //screens
        private ScreenMap _screenMapPiece0; //screen principal
        private ScreenMap _screenMapPiece1; //screen pièce 1
        private ScreenMap _screenMapPiece2;   //screen pièce2
        private ScreenMap _screenMapPiece3;   //screen pièce2
        private Ecran _ecranEnCours;            //screen actuel (nom pour comparer)
        //manager
        private readonly ScreenManager _screenManager;
        //timer
        private float deltaSeconds;
        private float _timer;
        private SpriteFont _police;
        private Vector2 _posTimer;
        private string heure;
        private const int TEMPS_TOTAL = 100;
        private int _tempsParHeure;
        //dead  
        private List<bool> _listeStartCompteurDead = new List<bool>();    //mettre en liste pour tous persos
        private List<float> _listeCompteurDead = new List<float>();
        //son, ambiance, musique
        private Song _ambiance;
        private SoundEffect _sonporte;
        //
        private Node _chemin;
        //
        private float _tempsDepuisDebut;
        

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
            _timer = TEMPS_TOTAL;   //temps de jeu total
            _posTimer = new Vector2(1, 1);
            heure = "";
            _tempsParHeure = TEMPS_TOTAL / 6;
            _chemin = new Node(new Vector2(50, 50));
            _chemin.Parent = new Node(new Vector2(50, 50));
            _tempsDepuisDebut = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //creation sprites
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteSheet animation1 = Content.Load<SpriteSheet>("motw.sf", new JsonContentLoader());  //importation animation1
            SpriteSheet animation2 = Content.Load<SpriteSheet>("joueur.sf", new JsonContentLoader());  //importation animation1
            SpriteSheet monstre = Content.Load<SpriteSheet>("monstre.sf", new JsonContentLoader());  //importation monstre
            _spritePerso1 = new AnimatedSprite(animation2);        //sprite anime1 pour perso
            _spritePerso2 = new AnimatedSprite(animation2);        //sprite anime1 pour perso
            _spritePersoBotTest = new AnimatedSprite(animation1);        //sprite anime2 pour perso bot test
            _spriteMonstre = new AnimatedSprite(monstre);        //sprite monstre
            _ambiance = Content.Load<Song>("sounds/horror-ambience-8-background-effect");
            _sonporte = Content.Load<SoundEffect>("sounds/portewav");
            _police = Content.Load<SpriteFont>("timer");
            MediaPlayer.Play(_ambiance);


            //creation perso
            CreationPersos();
            //creation bots
            CreationBots();
            //creation maps
            CreationMaps();

            //on initialise tout sur l'ecran principal
            _screenManager.LoadScreen(_listeScreenMap[0], new FadeTransition(GraphicsDevice, Color.Black));
            _ecranEnCours = Ecran.Piece0;
            //initialisation screen principal
            _listeScreenMap[(int)_ecranEnCours].Initialize();
            _listeScreenMap[(int)_ecranEnCours].LoadContent();
        }

        public void Time()
        {

            _timer -= deltaSeconds;
            //Console.WriteLine((int)_timer);
            int newDiff = 0;
            if (_timer <= _tempsParHeure)
            {
                heure = "05:00";
                newDiff = 4;
            }
            else if (_timer <= (_tempsParHeure * 2))
            {
                heure = "04:00";
                newDiff = 3;
            }
            else if (_timer <= (_tempsParHeure*3))
            {
                heure = "03:00";
                newDiff = 2;
            }
            else if (_timer <= (_tempsParHeure*4))
            {
                heure = "02:00";
                newDiff = 1;
            }
            else if (_timer <= (_tempsParHeure*5))
            {
                heure = "01:00";
            }
            else
            {
                heure = "00:00";
            }

            for (int i = 0; i < _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner.Count; i++)
            {
                _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner[i].ChangementDifficulteBot(newDiff);

            }

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
                        persoActuel.PtDeVie -= deltaSecond*botActuel.DegatsBot;
                        Console.WriteLine("Point de vie perso "+j+" : "+(int)persoActuel.PtDeVie);
                    }
                    if (persoActuel.PtDeVie <= 0)
                        _listeStartCompteurDead[j] = true;
                    //compteur dead
                    if (_listeStartCompteurDead[j])
                    {
                        persoActuel.Animation = Perso.TypeAnimation.dead;    //passe en dead
                        _listeCompteurDead[j] += deltaSeconds;
                        if (_listeCompteurDead[j] >= 10)
                        {
                            _listePerso.Remove(persoActuel);
                            _listeStartCompteurDead.Remove(_listeStartCompteurDead[j]);
                            _listeCompteurDead.Remove(_listeCompteurDead[j]);
                        }
                        else if (_listeCompteurDead[j] >= 1)
                        {
                            persoActuel.Animation = Perso.TypeAnimation.idleDead;
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

            //tempsdepuisdebut


            _isCollisionSpeciale = TypeCollisionMap.Rien;   //réinitialisation des colision
            //deplacement chaque perso
            for (int i = 0; i < _listePerso.Count; i++)
            {
                //récupérationdu type de colision
                if (_listePerso[i].Collision != TypeCollisionMap.Rien)
                    _isCollisionSpeciale = _listePerso[i].Collision;

                //update position des perso
                _listePerso[i].Move(_listeScreenMap[(int)_ecranEnCours], gameTime, _listePerso[i].TypeDeControl);
            }


            //changement vers piece 0
            if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece0)
                ChangementScreen(Ecran.Piece0);
            //changement vers piece 1
            else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece1)
                ChangementScreen(Ecran.Piece1);
            //changement vers piece 2
            else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece2)
                ChangementScreen(Ecran.Piece2);
            //changement vers piece 3
            else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece3)
                ChangementScreen(Ecran.Piece3);


            //deplacement chaque bot

            for (int i = 0; i < _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner.Count; i++)
            {
                Vector2 vectTemp = _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner[i].XY_ToVector(_listeScreenMap[(int)_ecranEnCours]);
                int botX = (int)vectTemp.X;
                int botY = (int)vectTemp.Y;
                int cheminX = (int)_chemin.Parent.Position.X;
                int cheminY = (int)_chemin.Parent.Position.Y;
                int min=10000;
                //Console.WriteLine(botX + "/" + botY + "          /         " + cheminX + "/" + cheminY);
                if (botX == cheminX && botY == cheminY)
                {

                    for (int j = 0; j < _listePerso.Count; j++)
                    {
                        //on calcule & compare le cout pour chaque perso et chaque bot
                        Vector2 vectorPositionBot = new Vector2(botX,botY) ;               
                        Vector2 vectorPositionPerso = _listePerso[j].XY_ToVector(_listeScreenMap[(int)_ecranEnCours]);
                        //Console.WriteLine("algo");
                        Node temp = Astar.AlgoAStar(new Node(vectorPositionPerso), new Node(vectorPositionBot), _listeScreenMap[(int)_ecranEnCours]);
                        if (min > temp.FCost)
                        {
                            _chemin = temp;
                            min = _chemin.FCost;
                        }
                    
                    }
                }
                if (!(_chemin.Parent is null))
                {
                    _listeBots[i].MoveAStar(_chemin.Parent.Position, _listeScreenMap[(int)_ecranEnCours], gameTime); //on fait bouger le bot vers le perso le plus proche
                    //Console.WriteLine("bot in : " + chemin.Position+ " / bouge vers : "+chemin.Parent.Position);
                }
            }

            base.Update(gameTime);

        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin();
            _spriteBatch.DrawString(_police, heure, _posTimer, Color.Red);
            base.Draw(gameTime);    //dessine objets
            SpriteBatch.End();
        }
        public void CreationBots()
        {
            //creation bot
            _persoBotTest = new Bot(new Vector2(400, 400), _spritePersoBotTest);
            _monstre = new Bot(new Vector2(400, 400), _spriteMonstre);
            //ajout des bots à la liste
            _listeBots.Add(_persoBotTest);
            _listeBots.Add(_monstre);

        }
        public void CreationPersos()    //génération de tout ce qui tourne autour des perso
        {
            //creation persos
            _perso1 = new Perso(new Vector2(400, 400), _spritePerso1, TypeControl.Clavier_HBGD);   //creation perso1
            _perso2 = new Perso(new Vector2(400, 400), _spritePerso2, TypeControl.Clavier_ZQSD);    //creation perso2
            //ajout des perso à la liste
            _listePerso.Add(_perso1);   //perso1
            if (NbrPerso==2)
                _listePerso.Add(_perso2);   //perso2
            //initialisation des compteurs de mort pour les persos
            for (int i =0; i<_listePerso.Count;i++)
            {
                _listeStartCompteurDead.Add(false);
                _listeCompteurDead.Add(0);
            }
        }
        public void CreationMaps()  //génération de tout ce qui tourne autour des maps
        {
            _screenMapPiece0 = new ScreenMap(this, "mansion_maps_version2/Piece_0", "obstacles", 800, 800);              //creation map0
            _screenMapPiece1 = new ScreenMap(this, "mansion_maps_version2/Piece_1", "obstacles", 800, 800);              //creation map1
            _screenMapPiece2 = new ScreenMap(this, "mansion_maps_version2/Piece_2", "obstacles", 800, 800);              //creation map2
            _screenMapPiece3 = new ScreenMap(this, "mansion_maps_version2/Piece_3", "obstacles", 800, 800);              //creation map3
            //ajout des maps à la liste
            _listeScreenMap.Add(_screenMapPiece0);      //ajout map0
            _listeScreenMap.Add(_screenMapPiece1);      //ajout map1
            _listeScreenMap.Add(_screenMapPiece2);      //ajout map2
            _listeScreenMap.Add(_screenMapPiece3);      //ajout map3
            //ajout des vecteurs par piece
            _listeVecteursSpawnParMap.Add(new Vector2(200, 400));     //ajout vecteur map0
            _listeVecteursSpawnParMap.Add(new Vector2(200, 400));     //ajout vecteur map1 
            _listeVecteursSpawnParMap.Add(new Vector2(200, 400));     //ajout vecteur map2
            _listeVecteursSpawnParMap.Add(new Vector2(200, 400));     //ajout vecteur map3
            for (int i = 0; i < _listeScreenMap.Count; i++)
            {
            _listeScreenMap[i].UpdateListJoueursAAfficher(_listePerso);
            }
            _listeScreenMap[1].UpdateListBotsAAfficher(new List<Bot>() { _listeBots[0] });
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
            _sonporte.Play();
        }
    }
}
