using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Facade
{


    public struct CustomVertex
    {

        public Vector4 Position;
        public Vector4 Color;

        public CustomVertex(Vector4 Position, Vector4 Color)
        {
            this.Position = Position;
            this.Color = Color;
        }
    }



    /// <summary>
    /// Enables us to incrementally build vertex/index buffers
    /// </summary>
    public class Geometry<VertexType> : IDisposable where VertexType : struct
    {

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

            FieldInfo[] fi = typeof(VertexType).GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fi.Length; ++i)
            {
                Console.WriteLine(fi[i].Name);
                Console.WriteLine(i);
                Console.WriteLine(Marshal.SizeOf(fi[i].FieldType) / 4);
                Console.WriteLine(Marshal.SizeOf(typeof(VertexType)));
                Console.WriteLine(Marshal.OffsetOf(typeof(VertexType), fi[i].Name));

                GL.EnableVertexAttribArray(i);

                // Handle the sizes of the vector types
                int components = 1;
                VertexAttribPointerType datatype = VertexAttribPointerType.Float;
                bool normalize = true;

                // This must exist in OpenTK somewhere ....
                if (fi[i].FieldType == typeof(OpenTK.Vector4))
                {
                    components = 4;
                    datatype = VertexAttribPointerType.Float;
                }
                else if (fi[i].FieldType == typeof(UInt32))
                {
                    components = 1;
                    datatype = VertexAttribPointerType.UnsignedInt;
                }
                else
                {
                    throw new Exception();
                }


                GL.VertexAttribPointer(i,
                    components,
                    datatype,
                    normalize,
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
