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

    class Motor
    {
        private Rectangle _rectangleMotor;
        private Vector2 _positionMotor;
        private Vector2 _tailleMotor;
        private bool _isPrise;
        private AnimatedSprite _spriteMotor;
        public Motor(Vector2 position, AnimatedSprite sprite)
        {
            PositionMotor = position;
            IsPrise = false;
            SpriteMotor = sprite;
            int HITBOX = 25;
            TailleMotor = new Vector2(HITBOX * 2, HITBOX * 2);
            RectangleMotor = new Rectangle((int)PositionMotor.X - 5, (int)PositionMotor.Y, (int)TailleMotor.X, (int)TailleMotor.Y);
        }

        public Rectangle RectangleMotor
        {
            get
            {
                return this._rectangleMotor;
            }

            set
            {
                this._rectangleMotor = value;
            }
        }

        public Vector2 PositionMotor
        {
            get
            {
                return this._positionMotor;
            }

            set
            {
                this._positionMotor = value;
            }
        }

        public Vector2 TailleMotor
        {
            get
            {
                return this._tailleMotor;
            }

            set
            {
                this._tailleMotor = value;
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

        public AnimatedSprite SpriteMotor
        {
            get
            {
                return this._spriteMotor;
            }

            set
            {
                this._spriteMotor = value;
            }
        }
    }
}
