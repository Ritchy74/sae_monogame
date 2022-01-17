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
    class Journal
    {
        private Rectangle _rectangleJournal;
        private Vector2 _positionJournal;
        private Vector2 _positionTexte;
        private Vector2 _positionFeuille;
        private Vector2 _tailleJournal;
        private string _nomJournal;
        private bool _isPrise;
        private int _numeroJournal;
        private AnimatedSprite _spriteJournal;
        private string _texteJournal;
        private Texture2D _page;

        public Journal(Vector2 positionJournal, Vector2 positionTexte, Vector2 positionFeuille, string nomJournal, AnimatedSprite spriteJournal, int numeroJournal, string texte, Texture2D page)
        {
            PositionJournal = positionJournal;
            PositionTexte = positionTexte;
            PositionFeuille = positionFeuille;
            int HITBOX = 25;
            this._tailleJournal = new Vector2(HITBOX * 2, HITBOX * 2);
            RectangleJournal = new Rectangle((int)PositionJournal.X - 5, (int)PositionJournal.Y, (int)_tailleJournal.X, (int)_tailleJournal.Y);
            NomJournal = nomJournal;
            IsPrise = false;
            NumeroJournal = numeroJournal;
            SpriteJournal = spriteJournal;
            TexteJournal = texte;
            Page = page;
        }

        public Rectangle RectangleJournal
        {
            get
            {
                return this._rectangleJournal;
            }

            set
            {
                this._rectangleJournal = value;
            }
        }

        public string NomJournal
        {
            get
            {
                return this._nomJournal;
            }

            set
            {
                this._nomJournal = value;
            }
        }

        public bool IsPrise
        {
            get
            {
                return this._isPrise;
            }

            set
            {
                this._isPrise = value;
            }
        }

        public int NumeroJournal
        {
            get
            {
                return this._numeroJournal;
            }

            set
            {
                this._numeroJournal = value;
            }
        }

        public AnimatedSprite SpriteJournal
        {
            get
            {
                return this._spriteJournal;
            }

            set
            {
                this._spriteJournal = value;
            }
        }

        public Vector2 PositionJournal
        {
            get
            {
                return this._positionJournal;
            }

            set
            {
                this._positionJournal = value;
            }
        }
        public Vector2 PositionFeuille
        {
            get
            {
                return this._positionFeuille;
            }

            set
            {
                this._positionFeuille = value;
            }
        }
        public Vector2 PositionTexte
        {
            get
            {
                return this._positionTexte;
            }

            set
            {
                this._positionTexte = value;
            }
        }

        public string TexteJournal
        {
            get
            {
                return this._texteJournal;
            }

            set
            {
                this._texteJournal = value;
            }
        }
        public Texture2D Page
        {
            get
            {
                return this._page;
            }

            set
            {
                this._page = value;
            }
        }
    }
}
