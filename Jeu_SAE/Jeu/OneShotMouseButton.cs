using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Jeu
{
    public class OneShotMouseButton
    {
        static MouseState currentMouseState;
        static MouseState previousMouseState;

        public static MouseState GetState()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            return currentMouseState;
        }

        public static bool Press(bool left)
        {
            if (left)
                return currentMouseState.LeftButton == ButtonState.Pressed;
            else
                return currentMouseState.RightButton == ButtonState.Pressed;
        }

        public static bool NotPress(bool left)
        {
            if (left)
                return currentMouseState.LeftButton == ButtonState.Pressed && !(previousMouseState.LeftButton == ButtonState.Pressed);
            else
                return currentMouseState.RightButton == ButtonState.Pressed && !(previousMouseState.RightButton == ButtonState.Pressed);
        }
    }
}
