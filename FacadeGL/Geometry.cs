using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Facade
{

    [AttributeUsage(
       AttributeTargets.Field)]
    public class Normalized : System.Attribute
    {
    }

    public struct CustomVertex
    {

        public Vector4 Position;
        public Vector4 Color;
        [Normalized]
        public UInt32 Dummy;

        public CustomVertex(Vector4 Position, Vector4 Color)
        {
            this.Position = Position;
            this.Color = Color;
            this.Dummy = 0;
        }
    }

    class TypeInfo
    {
        public VertexAttribPointerType gltype;
        public int count;
    }

    /// <summary>
    /// Enables us to incrementally build vertex/index buffers
    /// </summary>
    public class Geometry<VertexType> : IDisposable where VertexType : struct
    {
        // Data for type conversion between C# and GL
        // Add types here.
        private static readonly Dictionary<Type, TypeInfo> type_xlate = new Dictionary<Type, TypeInfo>
        {
            {typeof(Vector4), new TypeInfo {gltype=VertexAttribPointerType.Float,count=4}},
            {typeof(UInt32), new TypeInfo {gltype=VertexAttribPointerType.UnsignedInt,count=1}}
        };


        // During the process of constructing a primitive model, vertex
        // and index data is stored on the CPU in these managed lists.
        List<VertexType> vertices = new List<VertexType>();

        // Once all the geometry has been specified, the InitializePrimitive
        // method copies the vertex and index data into these buffers, which
        // store it on the GPU ready for efficient rendering.

        int bufferHandle, vaoHandle;
        
        
        
        #region Initialization


        /// <summary>
        /// Adds a new vertex to the primitive model. This should only be called
        /// during the initialization process, before InitializePrimitive.
        /// </summary>
        public void AddVertex(VertexType v)
        {
            vertices.Add(v);
        }


        /// <summary>
        /// Queries the index of the current vertex. This starts at
        /// zero, and increments every time AddVertex is called.
        /// </summary>
        protected int CurrentVertex
        {
            get { return vertices.Count; }
        }


        /// <summary>
        /// Once all the geometry has been specified by calling AddVertex and AddIndex,
        /// this method copies the vertex and index data into GPU format buffers, ready
        /// for efficient rendering.
        /// 

        public void InitializePrimitive(Effect effect)
        {
            // to array
            effect.Use();

            VertexType[] vta = new VertexType[vertices.Count];
            for (int i = 0; i < vertices.Count; ++i)
            {
                vta[i] = vertices[i];
            }


            // We make bigtime use of reflection to assemble an interleaved VBO


            GL.GenBuffers(1, out bufferHandle);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferHandle);
            GL.BufferData<VertexType>(BufferTarget.ArrayBuffer, new IntPtr(vta.Length * Marshal.SizeOf(typeof(VertexType))), vta, BufferUsageHint.StaticDraw);


            // GL3 allows us to store the vertex layout in a "vertex array object" (VAO).
            // This means we do not have to re-issue VertexAttribPointer calls
            // every time we try to use a different vertex layout - these calls are
            // stored in the VAO so we simply need to bind the correct VAO.
            GL.GenVertexArrays(1, out vaoHandle);
            GL.BindVertexArray(vaoHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferHandle);

            
            // Create and binf the relevant vertex attrib arrays
            FieldInfo[] fi = typeof(VertexType).GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fi.Length; ++i)
            {
                
                GL.EnableVertexAttribArray(i);
                
                GL.VertexAttribPointer(i,
                    type_xlate[fi[i].FieldType].count,    // type_xlate contains the GL type and number of components for every C# type.
                    type_xlate[fi[i].FieldType].gltype,   // If you get a keyerror at this point, adjust the type mapping in type_xlate.
                    Attribute.IsDefined(fi[i], typeof(Normalized)), // Want normalization? Add it to the VertexType type using attributes.
                    Marshal.SizeOf(typeof(VertexType)),
                    Marshal.OffsetOf(typeof(VertexType), fi[i].Name));
                effect.BindAttrib(i, fi[i].Name);

            }


            GL.BindVertexArray(0);

        }


        /// <summary>
        /// Finalizer.
        /// </summary>
        ~Geometry()
        {
            Dispose(false);
        }


        /// <summary>
        /// Frees resources used by this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Frees resources used by this object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GL.DeleteBuffers(1, ref vaoHandle);
                GL.DeleteBuffers(1, ref bufferHandle);
            }
        }


        #endregion


        /// <summary>
        /// Draws the primitive model, using the specified effect. This method does not set any renderstates, so you must make
        /// sure all states are set to sensible values before you call it.
        /// </summary>
        public void Draw(Effect effect)
        {

            effect.Use();
            GL.BindVertexArray(vaoHandle);
            GL.DrawArrays(BeginMode.Triangles, 0, vertices.Count);
        }

    }
}
