using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameServer
{
    class Plane
    {
        public Vector3 position = Vector3.zero;

        public float width = 0;
        public float length = 0;

        public string tag = "";
        PlaneObject planeObject;

        public Plane(float _size, string _tag)
        {
            width = _size;
            length = _size;
            tag = _tag;
            CreatePlane();
        }

        public Plane(float _size, string _tag, Vector3 _position)
        {
            width = _size;
            length = _size;
            position = _position;
            tag = _tag;
            CreatePlane();
        }

        public Plane(float _width, float _length, string _tag)
        {
            width = _width;
            length = _length;
            CreatePlane();
        }

        private void CreatePlane()
        {
            PlaneObject temp = Prefabs.instance.InstantiatePrefab<PlaneObject>(Prefabs.instance.planeObject);
            temp.Initialize(position, width, length);
            planeObject = temp;
        }

        private bool AbovePlane(Vector3 _pos)
        {
            return _pos.y >= position.y;
        }

        private bool BelowPlane(Vector3 _pos)
        {
            return _pos.y < position.y;
        }

        public bool HitPlane(float PositionY, float VelocityY) 
        {
            return PositionY >= position.y && PositionY + VelocityY <= position.y;
        }

        public bool OnPlane(Vector3 _PredictedPos, float thickness)
        {
            float lwidth = (width + thickness) / 2f;
            float llength = (length + thickness) / 2f;
            return FindPoint(-lwidth + position.x, -llength + position.z,
                lwidth + position.x, llength + position.x, _PredictedPos.x, _PredictedPos.z);
        }

        private bool FindPoint(float x1, float z1, float x2, float z2, float x, float z)
        {
            if (x > x1 && x < x2 && z > z1 && z < z2)
                return true;
            return false;
        }
    }
}
