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

        //perso
        private List<Vector2> _listeVecteursSpawnParMap = new List<Vector2>();          //point de respawn par map
        //nbr perso
        private int _nbrPerso;
        //perso1
        private AnimatedSprite _spritePerso1;
        private Perso _perso1;
        //perso2
        private AnimatedSprite _spritePerso2;
        private Perso _perso2;
        private List<Perso> _listePerso = new List<Perso>();                            //perso
        private List<Bot> _listeBots = new List<Bot>();                            //bots

        //placards
        private List<Rectangle> _listePlacards = new List<Rectangle>();                 //liste de placards
        private List<Vector2> _listePositionPlacards = new List<Vector2>();             //liste origine des placards
        private Vector2 _oldPosition;
        private float temp;
        private int _compteurPlacard = 0;

        //bots
        private int _indiceRonde;
        private List<Vector2> _listeVecteursRondeBot = new List<Vector2>();             //les points de passages quand les bots font leur ronde
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
        private List<ScreenMap> _listeScreenMap = new List<ScreenMap>();                //screens
        //private ScreenMap _screenMapPiece3;   //screen pièce2
        private Ecran _ecranEnCours;            //screen actuel (nom pour comparer)
        //manager
        private readonly ScreenManager _screenManager;
        
        //timer
        private float deltaSeconds;
        private float _timer;
        private SpriteFont _police;
        private Vector2 _posTimer;
        private string heure;
        private const int TEMPS_TOTAL = 300;
        private int _tempsParHeure;

        //clés
        List<Cle> _listeCles = new List<Cle>();
        private AnimatedSprite _spriteCles;

        //dead  
        private List<bool> _listeStartCompteurDead = new List<bool>();    //mettre en liste pour tous persos
        private List<float> _listeCompteurDead = new List<float>();
        
        //son, ambiance, musique
        private Song _ambiance;
        private SoundEffect _sonporte;        

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
            temp = _timer;
            _posTimer = new Vector2(1, 1);
            heure = "";
            _tempsParHeure = TEMPS_TOTAL / 6;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //creation sprites
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteSheet animation1 = Content.Load<SpriteSheet>("motw.sf", new JsonContentLoader());  //importation animation1
            SpriteSheet animation2 = Content.Load<SpriteSheet>("joueur.sf", new JsonContentLoader());  //importation animation1
            SpriteSheet monstre = Content.Load<SpriteSheet>("monstre.sf", new JsonContentLoader());  //importation monstre
            SpriteSheet spriteCle = Content.Load<SpriteSheet>("KeyIcons.sf", new JsonContentLoader());  //importation clés
            _spritePerso1 = new AnimatedSprite(animation2);        //sprite anime1 pour perso
            _spritePerso2 = new AnimatedSprite(animation2);        //sprite anime1 pour perso
            _spritePersoBotTest = new AnimatedSprite(animation1);        //sprite anime2 pour perso bot test
            _spriteMonstre = new AnimatedSprite(monstre);        //sprite monstre
            _spriteCles = new AnimatedSprite(spriteCle);        //sprite clé
            _ambiance = Content.Load<Song>("sounds/horror-ambience-8-background-effect");
            _sonporte = Content.Load<SoundEffect>("sounds/portewav");
            _police = Content.Load<SpriteFont>("timer");
            MediaPlayer.Play(_ambiance);


            //creation perso
            CreationPersos();
            //creation bots
            CreationBots();
            //creation clés
            CreationCles();
            //creation maps
            CreationMaps();

            //on initialise tout sur l'ecran principal
            _screenManager.LoadScreen(_listeScreenMap[0], new FadeTransition(GraphicsDevice, Color.Black));
            _ecranEnCours = Ecran.Piece0;
            //initialisation screen principal
            _listeScreenMap[(int)_ecranEnCours].Initialize();
            _listeScreenMap[(int)_ecranEnCours].LoadContent();
        }
        protected override void Update(GameTime gameTime)
        {

            //quit game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || _listePerso.Count == 0)
                Exit();
            //deltatime
            deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //faire écouler le timer dans le jeu 
            Time();
            //collision perso avec bot
            IsCollisionBot(deltaSeconds);

            //deplacement chaque perso
            _isCollisionSpeciale = TypeCollisionMap.Rien;   //réinitialisation des colision
            for (int i = 0; i < _listePerso.Count; i++)
            {
                //gérer les entrées et sorties dans les placards
                MethodePlacard(i);

                //gérer les clés
                MethodeCle(i);        

                //récupérationdu type de colision
                if (_listePerso[i].Collision != TypeCollisionMap.Rien)
                    _isCollisionSpeciale = _listePerso[i].Collision;

                //update position des perso
                if (!_listePerso[i].IsInPlacard)
                    _listePerso[i].Move(_listeScreenMap[(int)_ecranEnCours], gameTime, _listePerso[i].TypeDeControl);


                //changement vers piece 0
                if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece0)
                    ChangementScreen(Ecran.Piece0, _listeVecteursSpawnParMap[0]);
                //changement vers piece 1
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece1_bas)
                {
                    if (!_listeCles[0].IsPrise) //clé dehors
                        Console.WriteLine($"Il vous faut la clé {_listeCles[0].NomCle} pour continuer!");
                    else
                        ChangementScreen(Ecran.Piece1, _listeVecteursSpawnParMap[1]);
                }
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece1_basGauche )
                    ChangementScreen(Ecran.Piece1, _listeVecteursSpawnParMap[2]);
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece1_hautGauche )
                    ChangementScreen(Ecran.Piece1, _listeVecteursSpawnParMap[3]);
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece1_hautDroite)
                    ChangementScreen(Ecran.Piece1, _listeVecteursSpawnParMap[4]);
                //changement vers piece 2
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece2_bas)
                {
                    if (!_listeCles[1].IsPrise)
                        Console.WriteLine($"Il vous faut la clé {_listeCles[1].NomCle} pour continuer!");
                    else
                        ChangementScreen(Ecran.Piece2, _listeVecteursSpawnParMap[5]);
                }
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece2_haut)
                {
                    if (!_listeCles[1].IsPrise)
                        Console.WriteLine($"Il vous faut la clé {_listeCles[1].NomCle} pour continuer!");
                    else
                        ChangementScreen(Ecran.Piece2, _listeVecteursSpawnParMap[6]);
                }
            }

            //deplacement chaque bot
            DeplacementBot(gameTime);

            //
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
        public void MethodeCle(int i )
        {
            Rectangle rectPerso = new Rectangle((int)_listePerso[i].PositionPerso.X, (int)_listePerso[i].PositionPerso.Y, 48 - 2, 64 - 2);
            if (rectPerso.Intersects(_listeCles[(int)_ecranEnCours].RectangleCle) && !_listeCles[(int)_ecranEnCours].IsPrise)
            {
                KeyboardState keyboardState = Keyboard.GetState();          //recupere etat clavier
                Console.WriteLine("APPUYEZ SUR ESPACE POUR RECUPERER LA CLE " + _listeCles[(int)_ecranEnCours].NomCle);
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    Console.WriteLine("VOUS AVEZ TROUVE LA CLE " + _listeCles[(int)_ecranEnCours].NomCle);
                    _listeCles[(int)_ecranEnCours].IsPrise = true;
                }

            }
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
        public void MethodePlacard(int i)
        {
            //placard
            Rectangle rectPerso = new Rectangle((int)_listePerso[i].PositionPerso.X, (int)_listePerso[i].PositionPerso.Y, 48 - 2, 64 - 2);
            KeyboardState keyboardState = Keyboard.GetState();          //recupere etat clavier
            if (rectPerso.Intersects(_listePlacards[(int)_ecranEnCours]))
            {
                if (!_listePerso[i].IsInPlacard && _timer <= temp - 5)
                {
                    Console.WriteLine("tu peux te cacher en appuyant sur C");

                    if (keyboardState.IsKeyDown(Keys.C))
                    {
                        _compteurPlacard= _compteurPlacard+1;
                        Console.WriteLine($"perso {i + 1} caché / "+_compteurPlacard);
                        temp = _timer;
                        _oldPosition = _listePerso[i].PositionPerso;
                        _listePerso[i].IsInPlacard = true;
                        //_listePerso[i].PositionPerso = _listePositionPlacards[(int)_ecranEnCours] ;
                        _listePerso[i].PositionPerso = new Vector2(800, 800);
                    }
                }
            }
            if (_listePerso[i].IsInPlacard)
            {
                Console.WriteLine("tu peux te décacher en appuyant sur E");

                if (keyboardState.IsKeyDown(Keys.E) && _timer <= temp - 0.5 || _compteurPlacard == 2 || _timer <= temp - 10)
                { 
                    //Console.WriteLine(_compteurPlacard);
                    _compteurPlacard--;
                    Console.WriteLine($"perso {i + 1} decaché");
                    temp = _timer;
                    _listePerso[i].PositionPerso = _oldPosition;
                    _listePerso[i].IsInPlacard = false;
                }
            }
        }
        public void DeplacementBot(GameTime gameTime)
        {
            for (int i = 0; i < _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner.Count; i++)
            {
                Bot botActuel = _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner[i];
                Vector2 vectTemp = _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner[i].XY_ToVector(_listeScreenMap[(int)_ecranEnCours]);
                int botX = (int)vectTemp.X;
                int botY = (int)vectTemp.Y;
                int cheminX = (int)botActuel.CheminAPrendre.Parent.Position.X;
                int cheminY = (int)botActuel.CheminAPrendre.Parent.Position.Y;
                int min = 10000;
                //Console.WriteLine(botX + "/" + botY + "          /         " + cheminX + "/" + cheminY);
                if (botX == cheminX && botY == cheminY)
                {

                    for (int j = 0; j < _listePerso.Count; j++)
                    {
                        if (!_listePerso[j].IsInPlacard)
                        {
                            //on calcule & compare le cout pour chaque perso et chaque bot
                            Vector2 vectorPositionBot = new Vector2(botX, botY);
                            Vector2 vectorPositionPerso = _listePerso[j].XY_ToVector(_listeScreenMap[(int)_ecranEnCours]);
                            //Console.WriteLine("algo");
                            Node temp = Astar.AlgoAStar(new Node(vectorPositionPerso), new Node(vectorPositionBot), _listeScreenMap[(int)_ecranEnCours]);
                            if (min > temp.FCost)
                            {
                                botActuel.CheminAPrendre = temp;
                                min = botActuel.CheminAPrendre.FCost;
                            }
                        }
                    }
                }
                if (!(botActuel.CheminAPrendre.Parent is null) && botActuel.CheminAPrendre.Parent.FCost < botActuel.DistanceAggro && _compteurPlacard<_listePerso.Count)
                {
                    //Console.WriteLine("il te suit"+_compteurPlacard);
                    _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner[i].MoveAStar(_listeScreenMap[(int)_ecranEnCours], gameTime); //on fait bouger le bot vers le perso le plus proche
                    //Console.WriteLine("bot in : " + chemin.Position+ " / bouge vers : "+chemin.Parent.Position);
                }
                else
                {
                    //Console.WriteLine("il rentre");
                    Node newDirection = new Node(_listeVecteursRondeBot[_indiceRonde]);
                    newDirection.Parent = newDirection;

                    if (newDirection.Position.X == botX && newDirection.Position.Y == botY) //si on est arrivé au lieu de passage
                        _indiceRonde++;                                                     //on change de lieu
                    if (_indiceRonde == 4)  //on reset si on sort de la liste
                        _indiceRonde = 0;
                    Node temp = Astar.AlgoAStar(newDirection, new Node(new Vector2(botX, botY)), _listeScreenMap[(int)_ecranEnCours]);
                    botActuel.CheminAPrendre = temp;
                    _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner[i].MoveAStar(_listeScreenMap[(int)_ecranEnCours], gameTime);
                }
            }
        }

        
        public void CreationCles()
        {
            _listeCles.Add(new Cle(new Vector2(100, 500), "Cle principale",_spriteCles,0));
            _listeCles.Add(new Cle(new Vector2( 515, 528), "Cle 1",_spriteCles,1));
            _listeCles.Add(new Cle(new Vector2(107, 543), "Cle 2",_spriteCles,2));
            //_listeCles.Add(new Cle(new Vector2(300, 300), "Cle 3",_spriteCles,3));
        }
        public void CreationBots()
        {
            //creation bot
            _persoBotTest = new Bot(new Vector2(320, 320), _spritePersoBotTest, new Node(new Vector2(20, 20)));
            _monstre = new Bot(new Vector2(320, 320), _spriteMonstre, new Node(new Vector2(20, 20)));
            //ajout des bots à la liste
            _listeBots.Add(_persoBotTest);
            _listeBots.Add(_monstre);
            //ajout des vecteurs de ronde à la liste
            _listeVecteursRondeBot.Add(new Vector2(5,5));
            _listeVecteursRondeBot.Add(new Vector2(35,5));
            _listeVecteursRondeBot.Add(new Vector2(35,28));
            _listeVecteursRondeBot.Add(new Vector2(10,32));
            _indiceRonde = 0;

        }
        public void CreationPersos()    //génération de tout ce qui tourne autour des perso
        {
            //creation persos
            _perso1 = new Perso(new Vector2(320, 450), _spritePerso1, TypeControl.Clavier_HBGD);   //creation perso1
            _perso2 = new Perso(new Vector2(320, 450), _spritePerso2, TypeControl.Clavier_ZQSD);    //creation perso2
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
            _screenMapPiece0 = new ScreenMap(this, "mansion_maps_version5/Piece_0", "obstacles", 640, 640,_listeCles[0]);              //creation map0
            _screenMapPiece1 = new ScreenMap(this, "mansion_maps_version5/Piece_1", "obstacles", 640, 640, _listeCles[1]);              //creation map1
            _screenMapPiece2 = new ScreenMap(this, "mansion_maps_version5/Piece_2", "obstacles", 640, 640, _listeCles[2]);              //creation map2
            //_screenMapPiece3 = new ScreenMap(this, "mansion_maps_version2/Piece_3", "obstacles", 640, 640);              //creation map3
            //ajout des maps à la liste
            _listeScreenMap.Add(_screenMapPiece0);      //ajout map0
            _listeScreenMap.Add(_screenMapPiece1);      //ajout map1
            _listeScreenMap.Add(_screenMapPiece2);      //ajout map2
            //_listeScreenMap.Add(_screenMapPiece3);      //ajout map3

            //ajout des vecteurs par piece et par map (plusieurs spawns par map)
            _listeVecteursSpawnParMap.Add(new Vector2(320, 450));   //ajout vecteur1 map0
            _listeVecteursSpawnParMap.Add(new Vector2(320, 550));   //ajout vecteur1 map1 
            _listeVecteursSpawnParMap.Add(new Vector2(50, 425));    //ajout vecteur2 map1
            _listeVecteursSpawnParMap.Add(new Vector2(50, 90));     //ajout vecteur3 map1
            _listeVecteursSpawnParMap.Add(new Vector2(590, 90));     //ajout vecteur4 map1
            _listeVecteursSpawnParMap.Add(new Vector2(590, 90));     //ajout vecteur1 map2
            _listeVecteursSpawnParMap.Add(new Vector2(600, 450));     //ajout vecteur2 map2

            //ajout rectangles placards à la liste (1 par map)
            _listePositionPlacards.Add(new Vector2(0, 0));
            _listePositionPlacards.Add(new Vector2(448, 192));
            _listePositionPlacards.Add(new Vector2(80, 0));
            _listePlacards.Add(new Rectangle(0, 0, 0, 0));  //map 0 (y'en a pas)
            _listePlacards.Add(new Rectangle(448, 192, 64, 96));  //map1
            _listePlacards.Add(new Rectangle(80, 0, 64, 128));  //map2

            //initialisation position perso et bot
            for (int i = 0; i < _listeScreenMap.Count; i++)
            {
            _listeScreenMap[i].UpdateListJoueursAAfficher(_listePerso);
            }
            _listeScreenMap[1].UpdateListBotsAAfficher(new List<Bot>() { _listeBots[0] });
            _listeScreenMap[2].UpdateListBotsAAfficher(new List<Bot>() { _listeBots[1] });
        }
        public void ReinitialisationPosition(Vector2 position)
        {
            for (int i = 0; i < _listePerso.Count; i++)
            {
                _listePerso[i].PositionPerso = position;  //reinitialise position perso
                _listePerso[i].Collision = TypeCollisionMap.Rien;      //reinitialise les collisions pour pas etre bloqué
            }
        }
        public void ChangementScreen(Ecran versCetEcran, Vector2 newPosPerso)
        {
            for (int i = 0; i < _listePerso.Count; i++)
            {
                _listePerso[i].IsInPlacard = false;
                if (_compteurPlacard>1)
                    _compteurPlacard--;
            }
            Console.WriteLine($"CHARGEMENT  {versCetEcran.ToString()}");
            ReinitialisationPosition(newPosPerso);
            //_listeScreenMap[(int)_ecranEnCours].UpdateListBotsAAfficher(new List<Bot>()); 
            _ecranEnCours = versCetEcran;                   //changement enum ecran
            //_listeScreenMap[(int)_ecranEnCours].UpdateListBotsAAfficher(_listeBots);
            _screenManager.LoadScreen(_listeScreenMap[(int)_ecranEnCours]);            //chargement nouvelle map
            _sonporte.Play();
        }
    }
}
