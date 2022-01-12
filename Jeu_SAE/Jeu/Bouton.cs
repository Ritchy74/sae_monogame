using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace Jeu
{
    public class Bouton
    {
        private Texture2D _staticTexture;
        private Texture2D _ClickedTexture;
        public int AnimationTime { get; set; }
        public string Name { get; set; }
        public Point Dimensions { get; set; }
        public int ID { get; set; }
        public float Layer { get; set; }
        public bool Visible { get; set; }
        public Vector2 Postition { get; set; }
        public Texture2D Texture { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Bouton(Texture2D staticImage, Texture2D clikedImage, Point dimensions, Vector2 position, string name, int id, bool visible, float layer)
        {
            _staticTexture = staticImage;
            _ClickedTexture = clikedImage;
            Texture = _staticTexture;
            Width = dimensions.X;
            Height = dimensions.Y;
            Layer = layer;
            Visible = visible;
            AnimationTime = 0;
            Name = name;
            Postition = position;
            Dimensions = new Point(dimensions.X, dimensions.Y);
            ID = id;
        }
        public void Cicked()
        {
            AnimationTime = 15;
            Texture = _ClickedTexture;            
        }
        public void UpdateButton()
        {
            if (AnimationTime > 0)
                AnimationTime--;
            if (AnimationTime == 0)
                Texture = _staticTexture;
        }
    }
}
