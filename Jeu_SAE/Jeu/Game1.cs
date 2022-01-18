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
using System.Threading;

namespace Jeu
{
    public enum Ecran { Piece0, Piece1, Piece2, Piece3, Piece4 };
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
        //private List<Vector2> _listeVecteursRondeBot = new List<Vector2>();             //les points de passages quand les bots font leur ronde
        private Vector2[,] _listeVecteursRondeBot;
        private List<Vector2> _listeVecteursRondeBot2 = new List<Vector2>();
        //perso bot test
        private AnimatedSprite _spritePersoBotTest;
        //monstre
        private AnimatedSprite _spriteMonstre;
        private Bot _monstre1;
        private Bot _monstre2;
        private Bot _monstre3;
        private Bot _monstre4;
        
        //collision
        TypeCollisionMap _isCollisionSpeciale;
        //screens
        private ScreenMap _screenMapPiece0; //screen principal
        private ScreenMap _screenMapPiece1; //screen pièce 1
        private ScreenMap _screenMapPiece2;   //screen pièce2
        private ScreenMap _screenMapPiece3;   //screen pièce3
        private ScreenMap _screenMapPiece4;   //screen pièce4
        private List<ScreenMap> _listeScreenMap = new List<ScreenMap>();                //screens
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
        //journal
        List<Journal> _listeJournal = new List<Journal>();
        //List<Texture2D> _listePage = new List<Texture2D>();
        private bool _afficherMessageSortir;
        private Texture2D _pageAff;
        private Vector2 _posPage;
        private string _textePage;
        private Vector2 _posTextePage;
        private AnimatedSprite _spriteJournal;

        //dead  
        private List<bool> _listeStartCompteurDead = new List<bool>();    //mettre en liste pour tous persos
        private List<float> _listeCompteurDead = new List<float>();
        
        //son, ambiance, musique
        private Song _ambiance;
        private SoundEffect _sonporte;
        private SoundEffect _sonPage;
        private SoundEffect _sonPas;
        private SoundEffect _sonMort;

        //Point de vie
        private Texture2D _imgCoeur1, _imgCoeur2;
        private Vector2 _posCoeur1, _posCoeur2;
        private Vector2 _posPV_J1, _posPV_J2;
        private double pvPerso1, pvPerso2;
        private SpriteFont _policePV;

        //affichage
        private Vector2 _positionTexte;
        private SpriteFont _policeTexte;
        private string _leTexte;
        private float _timerTexte;

        //fog
        private Texture2D _fog1J;
        private Texture2D _fog2J;
        private Vector2 _vecteurFog;
        private AnimatedSprite _spriteMotor;
        private Motor _moteur;

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

            //affichage textes en bas

            //Position de la vie des joueurs
            _posPV_J1 = new Vector2(35, 600);
            _posPV_J2 = new Vector2(-1000, -1000);
            _posCoeur1 = new Vector2(0, 602);
            _posCoeur2 = new Vector2(-1000, -1000);
            

            //texte
            //LEO
            _positionTexte = new Vector2(100, 500);
            _leTexte = "";
            _timerTexte = 0;

            _pageAff = Content.Load<Texture2D>("PAGES/paper-horiz");
            _posPage = new Vector2(-1500, -1500);
            _posTextePage = new Vector2(0, 0);
            _afficherMessageSortir = false;



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
            SpriteSheet spriteJournal = Content.Load<SpriteSheet>("page.sf", new JsonContentLoader());  //importation journaux
            SpriteSheet spriteMotor = Content.Load<SpriteSheet>("motor.sf", new JsonContentLoader());  //importation motor
            _spritePerso1 = new AnimatedSprite(animation2);        //sprite anime1 pour perso
            _spritePerso2 = new AnimatedSprite(animation2);        //sprite anime1 pour perso
            _spritePersoBotTest = new AnimatedSprite(animation1);        //sprite anime2 pour perso bot test
            _spriteMonstre = new AnimatedSprite(monstre);        //sprite monstre
            _spriteCles = new AnimatedSprite(spriteCle);        //sprite clé
            _spriteJournal = new AnimatedSprite(spriteJournal);        //sprite journal
            _spriteMotor = new AnimatedSprite(spriteMotor);        //sprite motor
            _ambiance = Content.Load<Song>("sounds/horror-ambience-8-background-effect");
            _sonPas = Content.Load<SoundEffect>("sounds/pas_parquet");
            _sonMort = Content.Load<SoundEffect>("sounds/WTF_Homme etrangle 1 (ID 1639)_LS");
            _sonporte = Content.Load<SoundEffect>("sounds/portewav");
            _sonPage = Content.Load<SoundEffect>("sounds/FlippingPages");
            _police = Content.Load<SpriteFont>("timer");
            _policePV = Content.Load<SpriteFont>("PV");
            _imgCoeur1 = Content.Load<Texture2D>("coeur");
            _imgCoeur2 = Content.Load<Texture2D>("coeur");
            _policeTexte = Content.Load<SpriteFont>("texte");
            //fog
            _fog1J = Content.Load<Texture2D>("fog_1J");
            _fog2J = Content.Load<Texture2D>("fog_2J");

            _vecteurFog = new Vector2(0, 0);
            _moteur = new Motor(new Vector2(450, 145), _spriteMotor);

            MediaPlayer.Play(_ambiance);

            //creation perso
            CreationPersos();
            //creation bots
            CreationBots();
            //creation clés
            CreationCles();
            //creation journaux
            CreationJournal();
            //creation maps
            CreationMaps();

            //on initialise tout sur l'ecran principal
            _screenManager.LoadScreen(_listeScreenMap[0], new FadeTransition(GraphicsDevice, Color.Black));
            _ecranEnCours = Ecran.Piece0;
            //initialisation screen principal
            _listeScreenMap[(int)_ecranEnCours].Initialize();
            _listeScreenMap[(int)_ecranEnCours].LoadContent();
        }

        public void CreationJournal()
        {
            _listeJournal.Add(new Journal(new Vector2(320, 300), new Vector2(50, 70), new Vector2(40, 50), "Je suis arrive", _spriteJournal, 0, "", Content.Load<Texture2D>("PAGES/papier0")));
            _listeJournal.Add(new Journal(new Vector2(130, 255), new Vector2(290, 150), new Vector2(10, 50), "Je viens d'entrer", _spriteJournal, 1, "", Content.Load<Texture2D>("PAGES/papier1")));
            //_listeJournal.Add(new Journal(new Vector2(100, 150), new Vector2(100, 150), new Vector2(40, 50), "journal 3", _spriteJournal, 2, "Je me suis cache dans un placard, j'ai vu une ombre arriver vers moi. Il se passe vraiment quelque chose de louche...", Content.Load<Texture2D>("PAGES/paper-sidebar-demi")));
            _listeJournal.Add(new Journal(new Vector2(560, 240), new Vector2(60, 80), new Vector2(60, 70), "Des bruits suspects", _spriteJournal, 2, "", Content.Load<Texture2D>("PAGES/papier2")));
            _listeJournal.Add(new Journal(new Vector2(180, 160), new Vector2(100, 150), new Vector2(70, 50), "A L'AIDE", _spriteJournal, 3, "", Content.Load<Texture2D>("PAGES/papier3")));
            _listeJournal.Add(new Journal(new Vector2(255, 545), new Vector2(100, 150), new Vector2(80, 60), "Je l'ai trouve", _spriteJournal, 4, "", Content.Load<Texture2D>("PAGES/papier4")));
        }
        public void CreationCles()
        {
            _listeCles.Add(new Cle(new Vector2(320, 300), "Cle principale", _spriteCles, 0));
            _listeCles.Add(new Cle(new Vector2(515, 528), "Cle du salon", _spriteCles, 1));
            _listeCles.Add(new Cle(new Vector2(107, 543), "Cle chambres", _spriteCles, 2));
            _listeCles.Add(new Cle(new Vector2(400, 543), "Cle cuisine", _spriteCles, 3));
            _listeCles.Add(new Cle(new Vector2(340, 305), "Cle sortie", _spriteCles, 4));
        }
        public void CreationBots()
        {
            //creation bot
            //_persoBotTest = new Bot(new Vector2(320, 320), _spritePersoBotTest, new Node(new Vector2(20, 20)));
            _monstre1 = new Bot(new Vector2(320, 320), _spriteMonstre, new Node(new Vector2(20, 20)));
            _monstre2 = new Bot(new Vector2(320, 320), _spriteMonstre, new Node(new Vector2(20, 20)));
            _monstre3 = new Bot(new Vector2(224, 256), _spriteMonstre, new Node(new Vector2(14, 16)));
            _monstre4 = new Bot(new Vector2(200, 200), _spriteMonstre, new Node(new Vector2(8, 10)));
            //ajout des bots à la liste
            _listeBots.Add(_monstre1);
            _listeBots.Add(_monstre2);
            _listeBots.Add(_monstre3);
            _listeBots.Add(_monstre4);
            //ajout des vecteurs de ronde à la liste
            _listeVecteursRondeBot = new Vector2[4, 4] { { new Vector2(5, 5), new Vector2(35, 5), new Vector2(35, 28), new Vector2(10, 32) },
                { new Vector2(5, 5), new Vector2(32, 5), new Vector2(25, 16), new Vector2(27, 32) } ,
                    { new Vector2(3, 5), new Vector2(32, 5), new Vector2(35, 35), new Vector2(17, 32) } ,
                    { new Vector2(6, 11), new Vector2(33, 27), new Vector2(33, 6), new Vector2(6, 27) } };
            _indiceRonde = 0;

        }
        public void CreationPersos()    //génération de tout ce qui tourne autour des perso
        {
            //creation persos
            _perso1 = new Perso(new Vector2(320, 450), _spritePerso1, TypeControl.Clavier_HBGD);   //creation perso1
            _perso2 = new Perso(new Vector2(320, 450), _spritePerso2, TypeControl.Clavier_ZQSD);    //creation perso2
            //ajout des perso à la liste
            _listePerso.Add(_perso1);   //perso1
            if (NbrPerso == 2)
                _listePerso.Add(_perso2);   //perso2
            //initialisation des compteurs de mort pour les persos
            for (int i = 0; i < _listePerso.Count; i++)
            {
                _listeStartCompteurDead.Add(false);
                _listeCompteurDead.Add(0);
            }
        }
        public void CreationMaps()  //génération de tout ce qui tourne autour des maps
        {
            _screenMapPiece0 = new ScreenMap(this, "mansion_V2.2/Piece_0", "obstacles", 640, 640, _listeCles[0], _listeJournal[0]);              //creation map0
            _screenMapPiece1 = new ScreenMap(this, "mansion_V2.2/Piece_1", "obstacles", 640, 640, _listeCles[1], _listeJournal[1]);              //creation map1
            _screenMapPiece2 = new ScreenMap(this, "mansion_V2.2/Piece_2", "obstacles", 640, 640, _listeCles[2], _listeJournal[2]);              //creation map2
            _screenMapPiece3 = new ScreenMap(this, "mansion_V2.2/Piece_3", "obstacles", 640, 640, _listeCles[3], _listeJournal[3]);              //creation map3
            _screenMapPiece4 = new ScreenMap(this, "mansion_V2.2/Piece_4", "obstacles", 640, 640, _listeCles[4], _listeJournal[4]);              //creation map4
            //ajout des maps à la liste
            _listeScreenMap.Add(_screenMapPiece0);      //ajout map0
            _listeScreenMap.Add(_screenMapPiece1);      //ajout map1
            _listeScreenMap.Add(_screenMapPiece2);      //ajout map2
            _listeScreenMap.Add(_screenMapPiece3);      //ajout map2
            _listeScreenMap.Add(_screenMapPiece4);      //ajout map2
            //_listeScreenMap.Add(_screenMapPiece3);      //ajout map3

            //ajout des vecteurs par piece et par map (plusieurs spawns par map)
            _listeVecteursSpawnParMap.Add(new Vector2(320, 300));   //ajout vecteur1 map0
            _listeVecteursSpawnParMap.Add(new Vector2(320, 550));   //ajout vecteur1 map1 
            _listeVecteursSpawnParMap.Add(new Vector2(50, 425));    //ajout vecteur2 map1
            _listeVecteursSpawnParMap.Add(new Vector2(50, 90));     //ajout vecteur3 map1
            _listeVecteursSpawnParMap.Add(new Vector2(590, 90));     //ajout vecteur4 map1
            _listeVecteursSpawnParMap.Add(new Vector2(390, 300));     //ajout vecteur5 map1
            _listeVecteursSpawnParMap.Add(new Vector2(590, 90));     //ajout vecteur1 map2
            _listeVecteursSpawnParMap.Add(new Vector2(600, 450));     //ajout vecteur2 map2
            _listeVecteursSpawnParMap.Add(new Vector2(50, 490));     //ajout vecteur1 map3
            _listeVecteursSpawnParMap.Add(new Vector2(50, 90));     //ajout vecteur2 map3
            _listeVecteursSpawnParMap.Add(new Vector2(385, 530));     //ajout vecteur1 map4
            _listeVecteursSpawnParMap.Add(new Vector2(600, 90));     //ajout vecteur2 map4

            //ajout rectangles placards à la liste (1 par map)
            _listePositionPlacards.Add(new Vector2(0, 0));
            _listePositionPlacards.Add(new Vector2(448, 192));
            _listePositionPlacards.Add(new Vector2(80, 0));
            _listePositionPlacards.Add(new Vector2(0, 0));
            _listePositionPlacards.Add(new Vector2(0, 0));
            _listePlacards.Add(new Rectangle(0, 0, 0, 0));  //map 0 (y'en a pas)
            _listePlacards.Add(new Rectangle(448, 192, 64, 96));  //map1
            _listePlacards.Add(new Rectangle(80, 0, 64, 128));  //map2
            _listePlacards.Add(new Rectangle(80, 0, 64, 128));  //map3
            _listePlacards.Add(new Rectangle(80, 0, 64, 128));  //map4

            //initialisation position perso et bot
            for (int i = 0; i < _listeScreenMap.Count; i++)
            {
                _listeScreenMap[i].UpdateListJoueursAAfficher(_listePerso);
            }
            _listeScreenMap[1].UpdateListBotsAAfficher(new List<Bot>() { });
            _listeScreenMap[2].UpdateListBotsAAfficher(new List<Bot>() { });
            _listeScreenMap[3].UpdateListBotsAAfficher(new List<Bot>() { _listeBots[2] });
            _listeScreenMap[4].UpdateListBotsAAfficher(new List<Bot>() { _listeBots[3] });

            //motor
            _listeScreenMap[4].LeMotor = _moteur;
        }
        protected override void Update(GameTime gameTime)
        {

            //quit game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) || _listePerso.Count == 0)
            {
                //_sonMort.Play();
                Exit();
            }
            //deltatime
            deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //faire écouler le timer dans le jeu 
            if (_listeJournal[4].IsPrise)
                Time();
            //collision perso avec bot
            IsCollisionBot(deltaSeconds);
            //Affichage des points de vie
            AffichagePV();

            //gérer le texte
            _timerTexte += deltaSeconds;
            if (_timerTexte >= 4)   //reinitialisation du texte
            {
                _leTexte = "";
            }

            //pts de vie
            pvPerso1 = Math.Round(_perso1.PtDeVie, 0);
            pvPerso2 = Math.Round(_perso2.PtDeVie, 0);
            if (_perso1.PtDeVie <= 0)
                pvPerso1 = 0;
            if (_perso2.PtDeVie <= 0)
                pvPerso2 = 0;


            //deplacement chaque perso
            _isCollisionSpeciale = TypeCollisionMap.Rien;   //réinitialisation des colision
            for (int i = 0; i < _listePerso.Count; i++)
            {
                //generateur
                MethodeGenerateur(i);
                //fog
                if (!_moteur.IsPrise)
                    _vecteurFog = new Vector2(_listePerso[0].PositionPerso.X - 1000,_listePerso[0].PositionPerso.Y - 1000);
                //gérer les entrées et sorties dans les placards
                MethodePlacard(i);
                //gérer les clés
                MethodeCle(i);
                //gérer les journaux
                MethodeJournal(i);

                if (_listeJournal[2].IsPrise)
                {
                    _listeScreenMap[1].UpdateListBotsAAfficher(new List<Bot>() { _listeBots[0] });
                    _listeScreenMap[2].UpdateListBotsAAfficher(new List<Bot>() { _listeBots[1] });
                }

                //récupérationdu type de colision
                if (_listePerso[i].Collision != TypeCollisionMap.Rien)
                    _isCollisionSpeciale = _listePerso[i].Collision;

                //update position des perso
                if (!_listePerso[i].IsInPlacard && !_afficherMessageSortir)
                {
                    _listePerso[i].Move(_listeScreenMap[(int)_ecranEnCours], gameTime, _listePerso[i].TypeDeControl, _moteur.RectangleMotor, _ecranEnCours);
                    //_sonPas.Play();
                }


                //changement vers piece 0   (dehors)
                if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece0)
                {

                    if (!_listeCles[4].IsPrise)
                        _leTexte = $"Il vous manque: {_listeCles[4].NomCle}";
                    else if (_timer>=0)
                        _leTexte = "Vous devez attendre 6H pour sortir";
                    else 
                        ChangementScreen(Ecran.Piece0, _listeVecteursSpawnParMap[0]);
                }
                //changement vers piece 1
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece1_bas)  //depuis dehors
                {
                    if (!_listeCles[0].IsPrise) //clé dehors
                        _leTexte = $"Il vous manque: {_listeCles[0].NomCle}"; 
                    else
                    {
                        ChangementScreen(Ecran.Piece1, _listeVecteursSpawnParMap[1]);
                        heure = "00:00";
                    }
                }
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece1_milieu)
                    ChangementScreen(Ecran.Piece1, _listeVecteursSpawnParMap[5]);
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
                        _leTexte = $"Il vous manque: {_listeCles[1].NomCle}";   //LEO
                    else
                        ChangementScreen(Ecran.Piece2, _listeVecteursSpawnParMap[7]);
                }
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece2_haut)
                {
                    if (!_listeCles[1].IsPrise)
                        _leTexte = $"Il vous manque: {_listeCles[1].NomCle}";   //LEO
                    else
                        ChangementScreen(Ecran.Piece2, _listeVecteursSpawnParMap[6]);
                }

                //changement vers piece 3
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece3_bas)
                {
                    if (!_listeCles[2].IsPrise)
                        _leTexte = $"Il vous manque: {_listeCles[2].NomCle}";   //LEO
                    else
                        ChangementScreen(Ecran.Piece3, _listeVecteursSpawnParMap[8]);
                }
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece3_haut)
                {
                    ChangementScreen(Ecran.Piece3, _listeVecteursSpawnParMap[9]);
                }

                //changement vers piece 4
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece4_bas)
                {
                    if (!_listeCles[3].IsPrise)
                        _leTexte = $"Il vous manque: {_listeCles[3].NomCle}";   //LEO
                    else
                        ChangementScreen(Ecran.Piece4, _listeVecteursSpawnParMap[10]);
                }
                else if (_isCollisionSpeciale == TypeCollisionMap.PorteVersPiece4_haut)
                {
                    if (!_listeCles[3].IsPrise)
                        _leTexte = $"Il vous manque: {_listeCles[3].NomCle}";   //LEO
                    else
                        ChangementScreen(Ecran.Piece4, _listeVecteursSpawnParMap[11]);
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
            base.Draw(gameTime);    //dessine objets
            if (_ecranEnCours != Ecran.Piece0 && !_moteur.IsPrise)
            {
                if (_listePerso.Count == 1)
                    _spriteBatch.Draw(_fog1J, _vecteurFog, Color.White);
                else
                    _spriteBatch.Draw(_fog2J, _vecteurFog, Color.White);
            }
            _spriteBatch.DrawString(_police, heure, _posTimer, Color.Red);
            _spriteBatch.Draw(_imgCoeur1, _posCoeur1, Color.White);
            _spriteBatch.Draw(_imgCoeur2, _posCoeur2, Color.White);
            _spriteBatch.Draw(_pageAff, _posPage, Color.White);
            _spriteBatch.DrawString(_policePV, "" + pvPerso1, _posPV_J1, Color.White);
            _spriteBatch.DrawString(_policePV, "" + pvPerso2, _posPV_J2, Color.White);
            _spriteBatch.DrawString(_policeTexte, "" + _leTexte, _positionTexte, Color.White);  //LEO
            _spriteBatch.DrawString(_policeTexte, "" + _textePage, _posTextePage, Color.Black);
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
                newDiff = 1;
            }
            else
            {
                heure = "00:00";
            }

            for (int i = 0; i < _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner.Count; i++)
            {
                _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner[i].ChangementDifficulteBot(newDiff);

            }

        }
        public void AffichagePV()
        {
            if(_listeScreenMap[(int)_ecranEnCours].LesPersoADessiner.Count==2)
            {
                _posCoeur2 = new Vector2(602, 602);
                _posPV_J2 = new Vector2(549, 600);
            }
        }

        public void MethodeGenerateur(int i)
        {
            KeyboardState keyboardState = Keyboard.GetState();          //recupere etat clavier
            Rectangle rectPerso = new Rectangle((int)_listePerso[i].PositionPerso.X, (int)_listePerso[i].PositionPerso.Y, 48 - 2, 64 - 2);
            if (rectPerso.Intersects(_moteur.RectangleMotor) && !_moteur.IsPrise && _ecranEnCours==Ecran.Piece4)
            {
                _leTexte = " ESPACE pour allumer le generateur";
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    _moteur.IsPrise = true;
                    _leTexte = "";
                }
            }

        }
        public void MethodeJournal(int i)
        {
            
            KeyboardState keyboardState = Keyboard.GetState();          //recupere etat clavier
            Rectangle rectPerso = new Rectangle((int)_listePerso[i].PositionPerso.X, (int)_listePerso[i].PositionPerso.Y, 48 - 2, 64 - 2);
            if (rectPerso.Intersects(_listeJournal[(int)_ecranEnCours].RectangleJournal) && !_listeJournal[(int)_ecranEnCours].IsPrise )
            {
                if (!_afficherMessageSortir)
                {
                    _leTexte = " ESPACE pour: " + _listeJournal[(int)_ecranEnCours].NomJournal;
                    _positionTexte = new Vector2(170, 580);
                }
                else
                    _leTexte = " C pour fermer la page";
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    _textePage =  _listeJournal[(int)_ecranEnCours].TexteJournal;
                    _pageAff = _listeJournal[(int)_ecranEnCours].Page;
                    _posPage = _listeJournal[(int)_ecranEnCours].PositionFeuille;
                    _posTextePage = _listeJournal[(int)_ecranEnCours].PositionTexte;
                    _positionTexte = new Vector2(_posPage.X + 128, _posPage.Y + 502);
                    _afficherMessageSortir = true;
                    _sonPage.Play();
                }
                else if (keyboardState.IsKeyDown(Keys.C) && _afficherMessageSortir)
                {
                    _listeJournal[(int)_ecranEnCours].IsPrise = true;
                    _afficherMessageSortir = false;
                    _textePage = "";
                    _pageAff = Content.Load<Texture2D>("PAGES/paper-horiz");
                    _posPage = new Vector2(-1500, -1500);
                    _posTextePage = new Vector2(0, 0);
                    _leTexte = "";
                    _positionTexte = new Vector2(150, 580);
                    _sonPage.Play();
                }
            }
        }
        public void MethodeCle(int i )
        {
            Rectangle rectPerso = new Rectangle((int)_listePerso[i].PositionPerso.X, (int)_listePerso[i].PositionPerso.Y, 48 - 2, 64 - 2);
            if (rectPerso.Intersects(_listeCles[(int)_ecranEnCours].RectangleCle) && !_listeCles[(int)_ecranEnCours].IsPrise && _listeJournal[(int)_ecranEnCours].IsPrise)
            {
                KeyboardState keyboardState = Keyboard.GetState();          //recupere etat clavier
                _leTexte = "ESPACE pour: " + _listeCles[(int)_ecranEnCours].NomCle; //LEO
                _positionTexte = new Vector2(200, 580);
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    _leTexte = "Vous avez trouve: " + _listeCles[(int)_ecranEnCours].NomCle; //LEO
                    _timerTexte = 0;
                    _listeCles[(int)_ecranEnCours].IsPrise = true;
                    _positionTexte = new Vector2(150, 580);
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
                    if (Bot.IsColliBot_Play(botActuel, persoActuel) && persoActuel.PtDeVie >0)
                    {
                        persoActuel.PtDeVie -= deltaSecond*botActuel.DegatsBot;
                        //Console.WriteLine("Point de vie perso "+j+" : "+(int)persoActuel.PtDeVie);
                    }
                    if (persoActuel.PtDeVie <= 0)
                    {
                        persoActuel.PtDeVie = 0;
                        _listeStartCompteurDead[j] = true;
                    }
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
            if (rectPerso.Intersects(_listePlacards[(int)_ecranEnCours]) && _listeJournal[3].IsPrise)
            {
                if (!_listePerso[i].IsInPlacard && _timer <= temp - 5)
                {
                    _leTexte = " G pour: se cacher"; //LEO
                    _positionTexte = new Vector2(180, 580);
                    //Console.WriteLine("tu peux te cacher en appuyant sur C");

                    if (keyboardState.IsKeyDown(Keys.G))
                    {
                        _compteurPlacard= _compteurPlacard+1;
                        //Console.WriteLine($"perso {i + 1} caché / "+_compteurPlacard);
                        temp = _timer;
                        _positionTexte = new Vector2(150, 580);
                        _oldPosition = _listePerso[i].PositionPerso;
                        _listePerso[i].IsInPlacard = true;
                        //_listePerso[i].PositionPerso = _listePositionPlacards[(int)_ecranEnCours] ;
                        _listePerso[i].PositionPerso = new Vector2(800, 800);
                    }
                }
            }
            if (_listePerso[i].IsInPlacard)
            {
                _leTexte = " T pour: sortir";    //LEO
                _positionTexte = new Vector2(180, 580);
                _vecteurFog = new Vector2(_oldPosition.X - 1000, _oldPosition.Y - 1000);
                //Console.WriteLine("tu peux te décacher en appuyant sur E");

                if (keyboardState.IsKeyDown(Keys.T) && _timer <= temp - 0.5 || _compteurPlacard == 2 || _timer <= temp - 10)
                { 
                    //Console.WriteLine(_compteurPlacard);
                    _compteurPlacard--;
                    _leTexte = $"perso {i + 1} sorti";  //LEO
                    _timerTexte = 0;
                    //Console.WriteLine($"perso {i + 1} decaché");
                    _positionTexte = new Vector2(150, 580);
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
                    //Console.WriteLine("il te suit");
                    _listeScreenMap[(int)_ecranEnCours].LesBotsADessiner[i].MoveAStar(_listeScreenMap[(int)_ecranEnCours], gameTime); //on fait bouger le bot vers le perso le plus proche
                    //Console.WriteLine("bot in : " + chemin.Position+ " / bouge vers : "+chemin.Parent.Position);
                }
                else
                {
                    //Console.WriteLine("il rentre");
                    //Console.WriteLine(_indiceRonde);
                    Node newDirection = new Node(_listeVecteursRondeBot[(int)_ecranEnCours-1, _indiceRonde]);
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
            _indiceRonde = 0;
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
