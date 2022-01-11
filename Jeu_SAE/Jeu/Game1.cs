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


namespace Jeu
{
    public enum Ecran { Piece1, Piece2, Piece3 };
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //listes
        private List<Perso> _listePerso = new List<Perso>();                            //perso
        private List<TypeControl> _listeTypeControlePerso = new List<TypeControl>();    //façon de controler les perso
        private List<ScreenMap> _listeScreenMap = new List<ScreenMap>();                //screens
        private List<Vector2> _listeVecteursSpawnParMap = new List<Vector2>();          //point de respawn par map
        //perso1
        private AnimatedSprite _spritePerso1;
        private Perso _perso1;
        //perso2
        private AnimatedSprite _spritePerso2;
        private Perso _perso2;
        //perso bot test
        private AnimatedSprite _spritePersoBotTest;
        private Perso _persoBotTest;
        //collision
        TypeCollisionMap _isCollisionSpeciale;
        //screens
        private ScreenMap _screenMapPiece1; //screen principal
        private ScreenMap _screenMapPiece2;   //screen pièce2
        private ScreenMap _screenMapPiece3;   //screen pièce2
        private Ecran _ecranEnCours;            //screen actuel (nom pour comparer)
        //manager
        private readonly ScreenManager _screenManager;


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
            //nothing
            base.Initialize();
        }

        protected override void LoadContent()
        {
            //creation sprites
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteSheet animation1 = Content.Load<SpriteSheet>("motw.sf", new JsonContentLoader());  //importation animation1
            SpriteSheet animation2 = Content.Load<SpriteSheet>("joueur.sf", new JsonContentLoader());  //importation animation1
            _spritePerso1 = new AnimatedSprite(animation1);        //sprite anime1 pour perso
            _spritePerso2 = new AnimatedSprite(animation1);        //sprite anime1 pour perso
            _spritePersoBotTest = new AnimatedSprite(animation2);        //sprite anime2 pour perso bot test


            //creation perso
            CreationPersos();
            //creation maps
            CreationMaps();

            //on initialise tout sur l'ecran principal
            _screenManager.LoadScreen(_listeScreenMap[0], new FadeTransition(GraphicsDevice, Color.Black));
            _ecranEnCours = Ecran.Piece1;
            //initialisation screen principal
            _listeScreenMap[(int)_ecranEnCours].Initialize();
            _listeScreenMap[(int)_ecranEnCours].LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            //quit game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            _isCollisionSpeciale = TypeCollisionMap.Rien;   //réinitialisation des colision
            //récupérationdu type de colision
            for (int i = 0; i < _listePerso.Count; i++)
            {
                if (_listePerso[i].Collision != TypeCollisionMap.Rien)
                    _isCollisionSpeciale = _listePerso[i].Collision;
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


            for (int i = 0; i < _listePerso.Count; i++) //boucle sur le nombre de perso
            {
                _listePerso[i].Move(_listeScreenMap[(int)_ecranEnCours], gameTime, _listeTypeControlePerso[i]);  //update position des perso
            }
            base.Update(gameTime);

        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin();
            base.Draw(gameTime);    //dessine objets
            SpriteBatch.End();
        }
        public void CreationPersos()    //génération de tout ce qui tourne autour des perso
        {
            //creation persos
            _perso1 = new Perso(new Vector2(50, 175), _spritePerso1);   //creation perso1
            _perso2 = new Perso(new Vector2(50, 175), _spritePerso2);    //creation perso2
            //ajout des perso à la liste
            _listePerso.Add(_perso1);   //perso1
            _listePerso.Add(_perso2);   //perso2
            //ajout des types de controles à la liste
            _listeTypeControlePerso.Add(TypeControl.Clavier_ZQSD);  //haut,bas,gauche,droite
            _listeTypeControlePerso.Add(TypeControl.Clavier_HBGD);  //Z,Q,S,D
        }
        public void CreationMaps()  //génération de tout ce qui tourne autour des maps
        {
            _screenMapPiece1 = new ScreenMap(this, "mansion_maps_version1/Piece_1", "obstacles", _listePerso, 320,320);              //creation map1
            _screenMapPiece2 = new ScreenMap(this, "mansion_maps_version1/Piece_2", "obstacles", _listePerso,170,240);              //creation map2
            _screenMapPiece3 = new ScreenMap(this, "mansion_maps_version1/Piece_3", "obstacles", _listePerso,190, 250);              //creation map2
            //ajout des maps à la liste
            _listeScreenMap.Add(_screenMapPiece1);      //ajout map1
            _listeScreenMap.Add(_screenMapPiece2);      //ajout map2
            _listeScreenMap.Add(_screenMapPiece3);      //ajout map3
            //ajout des vecteurs par piece
            _listeVecteursSpawnParMap.Add(new Vector2(50, 175));     //ajout vecteur map1 
            _listeVecteursSpawnParMap.Add(new Vector2(90, 180));     //ajout vecteur map2
            _listeVecteursSpawnParMap.Add(new Vector2(40, 40));     //ajout vecteur map3
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
            _ecranEnCours = versCetEcran;                   //changement enum ecran
            _screenManager.LoadScreen(_listeScreenMap[(int)_ecranEnCours]);            //chargement nouvelle map
        }
    }
}
