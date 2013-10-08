using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;
using OpenTK;

namespace Facade
{
    public class Effect
    {
        string vertexShaderSource, fragmentShaderSource;
        int vertexShaderHandle,
            fragmentShaderHandle,
            shaderProgramHandle;

        public Effect(string vertexShaderSource, string fragmentShaderSource)
        {
            this.vertexShaderSource = vertexShaderSource;
            this.fragmentShaderSource = fragmentShaderSource;

            Console.WriteLine("Yo!");

            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderHandle, vertexShaderSource);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderSource);

            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);

            Debug.WriteLine(GL.GetShaderInfoLog(vertexShaderHandle));
            Console.WriteLine(GL.GetShaderInfoLog(fragmentShaderHandle));

            // Create program
            shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            GL.LinkProgram(shaderProgramHandle);

            Debug.WriteLine(GL.GetProgramInfoLog(shaderProgramHandle));
        }

        public static Effect FromFiles(string vectorfilename, string fragmentfilename)
        {
            string vp, fp;
            vp = System.IO.File.ReadAllText(vectorfilename, Encoding.ASCII);
            fp = System.IO.File.ReadAllText(fragmentfilename, Encoding.ASCII);
            return new Effect(vp, fp);
        }

        public void SetUniformMatrix4(Matrix4 matrix, string name)
        {
            int location = GL.GetUniformLocation(shaderProgramHandle, name);
            GL.UniformMatrix4(location, false, ref matrix);
        }

        public void SetUniform1(int v, string name)
        {
            int location = GL.GetUniformLocation(shaderProgramHandle, name);
            
            GL.Uniform1(location, v);            
        }


        public void Use()
        {
            GL.UseProgram(shaderProgramHandle);
        }

        public void BindAttrib(int index, string name)
        {
            GL.BindAttribLocation(shaderProgramHandle, index, name);
        }

    }
}
