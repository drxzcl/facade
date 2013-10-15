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
        public Int32 doorx;
        public Int32 flags;
        public Int32 wallTileNumber;
        public Int32 windowTileNumber;
        public Int32 altWindowTileNumber;

        //public static readonly int NO_GROUND_FLOOR_WINDOWS = 0x1;
        //public static readonly int ALT_WINDOWS_ABOVE_DOOR = 0x2;


        public BuildingVertex(Vector4 Position, Vector4 Normal, Vector4 TexCoord, int doorx, int flags, int wallTileNumber, int windowTileNumber, int altWindowTileNumber)
        {
            this.position = Position;
            this.normal = Normal;
            this.texcoord = TexCoord;
            this.flags = flags;
            this.doorx = doorx;
            this.wallTileNumber = wallTileNumber;
            this.windowTileNumber = windowTileNumber;
            this.altWindowTileNumber = altWindowTileNumber;

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


        void AddBuildingSegment(Geometry<BuildingVertex> g, Vector3 origin, Vector3 area, int doorx, int flags, int wallTileNumber, int windowTileNumber, int altWindowTileNumber)
        {
            // Four vertices per face
            // Font and back
            Vector4 o = new Vector4(origin, 1f);
            Vector4 normal;
            float width = area.X - 1e-4f;
            float height = area.Y - 1e-4f;
            float breadth = area.Z - 1e-4f;

            normal = new Vector4(0, 0, -1, 0); // front            
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, 0f, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(0f, height, 0f, 1f) + o, normal, new Vector4(0f, height, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, 0f, 1f) + o, normal, new Vector4(width, height, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, 0f, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, 0f, 1f) + o, normal, new Vector4(width, height, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(width, 0f, 0f, 1f) + o, normal, new Vector4(width, 0f, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));

            normal = new Vector4(0, 0, 1, 0); // back
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, breadth, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, breadth, 1f) + o, normal, new Vector4(width, height, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(0f, height, breadth, 1f) + o, normal, new Vector4(0f, height, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, breadth, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(width, 0f, breadth, 1f) + o, normal, new Vector4(width, 0f, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, breadth, 1f) + o, normal, new Vector4(width, height, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));

            normal = new Vector4(-1, 0, 0, 0); // side1
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, 0f, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(0f, height, breadth, 1f) + o, normal, new Vector4(breadth, height, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(0f, height, 0f, 1f) + o, normal, new Vector4(0f, height, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, 0f, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(0f, 0f, breadth, 1f) + o, normal, new Vector4(breadth, 0f, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(0f, height, breadth, 1f) + o, normal, new Vector4(breadth, height, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));

            normal = new Vector4(1, 0, 0, 0); // side2
            g.AddVertex(new BuildingVertex(new Vector4(width, 0f, 0f, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, 0f, 1f) + o, normal, new Vector4(0f, height, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, breadth, 1f) + o, normal, new Vector4(breadth, height, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(width, 0f, 0f, 1f) + o, normal, new Vector4(0f, 0f, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(width, height, breadth, 1f) + o, normal, new Vector4(breadth, height, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
            g.AddVertex(new BuildingVertex(new Vector4(width, 0f, breadth, 1f) + o, normal, new Vector4(breadth, 0f, 0f, 0f), doorx, flags, wallTileNumber, windowTileNumber, altWindowTileNumber));
        }


        public Geometry<BuildingVertex> createBuilding(Vector3 origin, Vector3 area)
        {
            var g = new Geometry<BuildingVertex>();
            int wallnr = random.Next(4);
            Console.WriteLine(String.Format("Wall: {0}",wallnr));
            AddBuildingSegment(g, origin, area,
                random.Next((int)area.X) + 1,
                0,//(random.Next(2) * BuildingVertex.NO_GROUND_FLOOR_WINDOWS + random.Next(2) * BuildingVertex.ALT_WINDOWS_ABOVE_DOOR),
                wallnr,
                choose(windows),
                choose(altWindows)
                );
            return g;
        }

        T choose<T>(List<T> list)
        {
            return list[random.Next(list.Count)];
        }

    }
}
