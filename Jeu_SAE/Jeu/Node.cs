using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text;
using MonoGame.Extended.Tiled;

namespace Jeu
{
    class Node
    {
        private Node _parent;
        private List<Node> _voisins;
        private int _fCost;
        private int _gCost;
        private int _hCost;
        private Vector2 _position;

        public Node(Vector2 position)
        {
            Position = position;
        }

        public bool IsTraversable(ScreenMap map)
        {
            TiledMapTile? tile;
            TiledMapTileLayer coucheObstacle = map.CoucheObstacle;
            bool res = true;
            //si la tuile actuelle est une de la couche obstacle
            if (coucheObstacle.TryGetTile((ushort)Position.X, (ushort)Position.Y, out tile))   
            {
                if (!tile.Value.IsBlank)
                    res = false;    //on return false
            }
            return res;
        }
        public List<Node> GetVoisins()
        {
            List<Node> res = new List<Node>();
            int taille = 1; //on avance de 1tuile par 1tuile
            res.Add(new Node(new Vector2(Position.X + taille, Position.Y)));    //droite
            res.Add(new Node(new Vector2(Position.X, Position.Y + taille)));    //bas
            res.Add(new Node(new Vector2(Position.X - taille, Position.Y)));    //gauche
            res.Add(new Node(new Vector2(Position.X, Position.Y - taille)));    //haut
            Voisins = res;
            return res;
        }
        public int CalculCost(Node par, Node target)
        {
            int gc = par.GCost + 1;     //cout depuis de début
            int hc = Math.Abs((int)target.Position.X - (int)Position.X) + Math.Abs((int)target.Position.Y - (int)Position.Y);   //cout pour aller jusqu'au target
            return gc + hc; //f_cost = g_cost+h_cost
        }
        public void SetFCost(Node par, Node target)
        {
            //on set le parent et le cost
            Parent = par;
            GCost = par.GCost + 1;
            HCost = Math.Abs((int)target.Position.X - (int)Position.X) + Math.Abs((int)target.Position.Y - (int)Position.Y); //utiliser valeur absolue pour éviter: -10+10=0 (en réalité =20)
            FCost = GCost + HCost;
        }

        public static bool operator ==(Node left, Node right)
        {
            return left.Position == right.Position;
        }
        public static bool operator !=(Node left, Node right)
        {
            return left.Position != right.Position;
        }
        public override bool Equals(object obj)
        {
            Node node = obj as Node;
            return Position == node.Position;
        }

        public int FCost
        {
            get
            {
                return this._fCost;
            }

            set
            {
                this._fCost = value;
            }
        }

        public int GCost
        {
            get
            {
                return this._gCost;
            }

            set
            {
                this._gCost = value;
            }
        }

        public int HCost
        {
            get
            {
                return this._hCost;
            }

            set
            {
                this._hCost = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return this._position;
            }

            set
            {
                this._position = value;
            }
        }

        internal Node Parent
        {
            get
            {
                return this._parent;
            }

            set
            {
                this._parent = value;
            }
        }

        internal List<Node> Voisins
        {
            get
            {
                return this._voisins;
            }

            set
            {
                this._voisins = value;
            }
        }
    }
}
