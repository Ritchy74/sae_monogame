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
    class Cle
    {
        private Rectangle _rectangleCle;
        private Vector2 _positionCle;
        private Vector2 _tailleCle;
        private string _nomCle;
        private bool _isPrise;
        private int _numeroCLe;
        private AnimatedSprite _spriteCle;

        public Cle(Vector2 positionCle, string nomCle, AnimatedSprite spritecle, int numeroCle)
        {
            PositionCle = positionCle;
            this._tailleCle = new Vector2(100, 100);
            RectangleCle = new Rectangle((int)positionCle.X, (int)positionCle.Y, (int)_tailleCle.X, (int)_tailleCle.Y);
            NomCle = nomCle;
            IsPrise = false;
            NumeroCLe = numeroCle;
            SpriteCle = spritecle;
        }

        public Rectangle RectangleCle
        {
            get
            {
                return this._rectangleCle;
            }

            set
            {
                this._rectangleCle = value;
            }
        }

        public string NomCle
        {
            get
            {
                return this._nomCle;
            }

            set
            {
                this._nomCle = value;
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

        public int NumeroCLe
        {
            get
            {
                return this._numeroCLe;
            }

            set
            {
                this._numeroCLe = value;
            }
        }

        public AnimatedSprite SpriteCle
        {
            get
            {
                return this._spriteCle;
            }

            set
            {
                this._spriteCle = value;
            }
        }

        public Vector2 PositionCle
        {
            get
            {
                return this._positionCle;
            }

            set
            {
                this._positionCle = value;
            }
        }
    }
}
