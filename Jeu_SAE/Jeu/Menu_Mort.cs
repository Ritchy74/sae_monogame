using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System;
using System.Threading;
namespace Jeu
{
    public class Menu_Mort : Game
    {
        private Game1 _game1; // pour récupérer la fenêtre de jeu principale
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Bouton _bouton1, _bouton2;
        private Vector2 _bouton1Pos, _bouton2Pos;
        private MouseState _oneShotMouseState;
        private bool _mouseLeftPressed;
        private bool _onePlayerBool;
        private bool _twoPlayersBool;
        private SpriteFont _police;

        public bool OnePlayerBool
        {
            get
            {
                return this._onePlayerBool;
            }

            set
            {
                this._onePlayerBool = value;
            }
        }

        public bool TwoPlayersBool
        {
            get
            {
                return this._twoPlayersBool;
            }

            set
            {
                this._twoPlayersBool = value;
            }
        }

        public Menu_Mort()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            _bouton1Pos = new Vector2(300, 100);
            _bouton2Pos = new Vector2(270, 300);
            _oneShotMouseState = OneShotMouseButton.GetState();
            _mouseLeftPressed = false;
            // TODO: Add your initialization logic here
            OnePlayerBool = false;
            TwoPlayersBool = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _bouton1 = new Bouton(Content.Load<Texture2D>("boutons/rejouer"), Content.Load<Texture2D>("boutons/rejouer"), new Point(200, 112), _bouton1Pos, "Bouton Jouer", 1, true, 1.0f);
            _bouton2 = new Bouton(Content.Load<Texture2D>("boutons/menu"), Content.Load<Texture2D>("boutons/menu"), new Point(270, 120), _bouton2Pos, "Bouton Jouer2", 2, true, 1.0f);
            _police = Content.Load<SpriteFont>("PV");
            // TODO: use this.Content to load your game content here
        }
        public void HandleInput(GameTime gametime)
        {
            _oneShotMouseState = OneShotMouseButton.GetState();
            if (_oneShotMouseState.LeftButton == ButtonState.Pressed)
                if (OneShotMouseButton.NotPress(true))
                {
                    //Console.WriteLine("Click gauche");
                    _mouseLeftPressed = true;
                }
        }
        protected override void Update(GameTime gameTime)
        {
            string nombouton = "";
            HandleInput(gameTime);
            _bouton1.UpdateButton();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (_mouseLeftPressed)
            {
                _mouseLeftPressed = false;

                if (VerifSiBoutonClick(ref nombouton))
                {
                    if (nombouton == "bouton1")
                    {
                        _bouton1.Cicked();
                        OnePlayerBool = true;
                    }
                    if (nombouton == "bouton2")
                    {
                        _bouton2.Cicked();
                        TwoPlayersBool = true;
                    }
                    //Console.WriteLine("if bouton clicked()");
                    Exit();
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            Rectangle sourceRectangle = new Rectangle(0, 0, _bouton1.Width, _bouton1.Height);
            Rectangle destinationRect = new Rectangle((int)_bouton1.Postition.X, (int)_bouton1.Postition.Y, _bouton1.Width, _bouton1.Height);
            Rectangle rectbouton1 = new Rectangle(0, 0, _bouton2.Width, _bouton2.Height);
            Rectangle destinationBT2 = new Rectangle((int)_bouton2.Postition.X, (int)_bouton2.Postition.Y, _bouton2.Width, _bouton2.Height);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.FrontToBack);
            _spriteBatch.DrawString(_police, "GAME OVER", new Vector2(313, 0), Color.Black);
            _spriteBatch.Draw(_bouton1.Texture, destinationRect, sourceRectangle, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            _spriteBatch.Draw(_bouton2.Texture, destinationBT2, rectbouton1, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private bool VerifSiBoutonClick(ref string nombouton)
        {
            if (_oneShotMouseState.X >= _bouton1.Postition.X && _oneShotMouseState.X <= (_bouton1.Postition.X + _bouton1.Dimensions.X))
            {
                if (_oneShotMouseState.Y >= _bouton1.Postition.Y && _oneShotMouseState.Y <= (_bouton1.Postition.Y + _bouton1.Dimensions.Y) && _bouton1.Visible)
                {
                    //Console.WriteLine("bouton1");
                    nombouton = "bouton1";
                    return true;
                }
            }
            if (_oneShotMouseState.X >= _bouton2.Postition.X && _oneShotMouseState.X <= (_bouton2.Postition.X + _bouton2.Dimensions.X))
            {
                if (_oneShotMouseState.Y >= _bouton2.Postition.Y && _oneShotMouseState.Y <= (_bouton2.Postition.Y + _bouton2.Dimensions.Y) && _bouton2.Visible)
                {
                    //Console.WriteLine("bouton2");
                    nombouton = "bouton2";
                    return true;
                }
            }
            return false;
        }
    }
}
