using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
namespace Facade
{
    [StructLayout(LayoutKind.Sequential,Pack=1)]
    public struct BuildingVertex
    {

        public Vector4 position;
        public Vector4 normal;
        public Vector4 texcoord;
        public Vector4 custom;
        public UInt32 dummy;
                

        public BuildingVertex(Vector4 Position, Vector4 Normal, Vector4 TexCoord, Vector4 Custom)
        {
            this.position = Position;
            this.normal = Normal;
            this.texcoord = TexCoord;
            this.custom = Custom;
            this.dummy = 0;
        }
    }

    public class PlainBuildingStyle 
    {
        public Effect effect;
       
        public List<int> windows = new List<int>();
        public List<int> altWindows = new List<int>();

        Random random;

        public PlainBuildingStyle(Random random = null)
        {
            this.random = random;
            if (this.random == null)
            {
                this.random = new Random();
            }
        }


        void AddBuildingSegment(Geometry<BuildingVertex> g, Vector3 origin, Vector3 area, Vector4 config)
        {
            // Four vertices per face
            // Font and back
            Vector4 o = new Vector4(origin, 1f);
            Vector4 normal;
            float width = area.X - 1e-4f;
            float height = area.Y - 1e-4f;
            float breadth = area.Z - 1e-4f;

            normal = new Vector4(0, 0, -1, 0); // front            
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, 0f, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(0f, height, 0f, 1f) + o, normal, new Vector4(0f, height, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, 0f, 1f) + o, normal, new Vector4(width, height, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, 0f, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, 0f, 1f) + o, normal, new Vector4(width, height, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(width, 0f, 0f, 1f) + o, normal, new Vector4(width, 0f, 0f, 0f), config));

            normal = new Vector4(0, 0, 1, 0); // back
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, breadth, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, breadth, 1f) + o, normal, new Vector4(width, height, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(0f, height, breadth, 1f) + o, normal, new Vector4(0f, height, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, breadth, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(width, 0f, breadth, 1f) + o, normal, new Vector4(width, 0f, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, breadth, 1f) + o, normal, new Vector4(width, height, 0f, 0f), config));

            normal = new Vector4(-1, 0, 0, 0); // side1
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, 0f, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(0f, height, breadth, 1f) + o, normal, new Vector4(breadth, height, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(0f, height, 0f, 1f) + o, normal, new Vector4(0f, height, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, 0f, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, breadth, 1f) + o, normal, new Vector4(breadth, 0f, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(0f, height, breadth, 1f) + o, normal, new Vector4(breadth, height, 0f, 0f), config));

            normal = new Vector4(1, 0, 0, 0); // side2
            g.AddVertex(new BuildingVertex(new Vector4(width, 0f, 0f, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, 0f, 1f) + o, normal, new Vector4(0f, height, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, breadth, 1f) + o, normal, new Vector4(breadth, height, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(width, 0f, 0f, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, breadth, 1f) + o, normal, new Vector4(breadth, height, 0f, 0f), config));
            g.AddVertex(new BuildingVertex(new Vector4(width, 0f, breadth, 1f) + o, normal, new Vector4(breadth, 0f, 0f, 0f), config));
        }


        public Geometry<BuildingVertex> createBuilding(Vector3 origin, Vector3 area)
        {
            var g = new Geometry<BuildingVertex>();
            Vector4 config = new Vector4();
            config.X = (random.Next((int)area.X) + 1);
            config.Y = random.Next(2) / 10f + 1e-6f;
            config.Y += random.Next(2) / 100f;
            config.Z = random.Next(4) / 10f + choose(windows) / 100f + choose(altWindows) / 1000f + 1e-6f;
            config.W = random.Next(100) / 100f + 1e-6f;
            AddBuildingSegment(g, origin, area, config);
            return g;
        }

        T choose<T>(List<T> list)
        {
            return list[random.Next(list.Count)];
        }

    }
}
