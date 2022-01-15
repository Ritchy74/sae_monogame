﻿using System;
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
            if (coucheObstacle.TryGetTile((ushort)Position.X, (ushort)Position.Y, out tile))
            {
                if (!tile.Value.IsBlank)
                    res = false;
            }
            return res;
        }
        public List<Node> GetVoisins()
        {
            List<Node> res = new List<Node>();
            int taille = 1;
            res.Add(new Node(new Vector2(Position.X + taille, Position.Y)));
            res.Add(new Node(new Vector2(Position.X, Position.Y + taille)));
            res.Add(new Node(new Vector2(Position.X - taille, Position.Y)));
            res.Add(new Node(new Vector2(Position.X, Position.Y - taille)));
            Voisins = res;
            return res;
        }
        public int CalculCost(Node par, Node target)
        {
            int gc = par.GCost + 1;
            int hc = ((int)target.Position.X - (int)Position.X) + ((int)target.Position.Y - (int)Position.Y);
            return gc + hc;
        }
        public void SetFCost(Node par, Node target)
        {
            Parent = par;
            GCost = par.GCost + 1;
            HCost = Math.Abs((int)target.Position.X - (int)Position.X) + Math.Abs((int)target.Position.Y - (int)Position.Y);
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