using System;
using System.Diagnostics;
using System.IO;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OpenTK.Input;

namespace Facade
{
    public class FacadeWindow : GameWindow
    {

        Effect effect;
        Geometry<BuildingVertex> g;
        PlainBuildingStyle buildingStyle;
        int wallTex, windowTex, interiorTex, doorTex;

        Matrix4 projectionMatrix, modelviewMatrix;

        public FacadeWindow()
            : base(640, 480,
            new GraphicsMode(), "Facade", 0,
            DisplayDevice.Default, 3, 0,
            GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        { }


        static int LoadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return id;
        }

        static int LoadTextureArray(params string[] filenames)
        {

            int id = -1;
            int width = -1;
            int height = -1;

            //foreach (string filename in filenames)
            for (int i=0;i<filenames.Length;i++)
            {
                if (String.IsNullOrEmpty(filenames[i]))
                    throw new ArgumentException(filenames[i]);

                Bitmap bmp = new Bitmap(filenames[i]);
                BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


                if (id==-1)
                {
                    // Initialize the texture
                    id = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2DArray, id);

                    width = bmp.Width;
                    height = bmp.Height;
                    IntPtr pointer = new IntPtr(0);// Do not use any actual data, we will initialize using TexSubImage3D
                    GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.Rgba, width, height, filenames.Length, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, pointer);
                    GL.Flush();
                }
                
                System.Console.WriteLine(GL.GetError().ToString());

                //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                //    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);
                if (bmp.Width != width || bmp.Height != height)
                    throw new ArgumentException(filenames[i]);

                GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, i, bmp.Width, bmp.Height, 1, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);
                bmp.UnlockBits(bmp_data);
                System.Console.WriteLine(GL.GetError().ToString());

            }
            GL.BindTexture(TextureTarget.Texture2DArray, id);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Clamp);
            GL.BindTexture(TextureTarget.Texture2DArray, 0);

            return id;
        }

        protected override void OnLoad(System.EventArgs e)
        {
            Keyboard.KeyUp += HandleKeyUp;

            VSync = VSyncMode.On;

            CreateShaders();

            buildingStyle = new PlainBuildingStyle();
            buildingStyle.effect = effect;
            buildingStyle.altWindows.Add(1);
            buildingStyle.altWindows.Add(3);
            buildingStyle.windows.Add(0);
            buildingStyle.windows.Add(2);
            buildingStyle.windows.Add(4);
            g = buildingStyle.createBuilding(new Vector3(-2.5f, -2.5f, -2.5f), new Vector3(5f, 5f, 5f));

            g.InitializePrimitive(effect);                        

            // Other state
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(System.Drawing.Color.MidnightBlue);

            
        }

        void CreateShaders()
        {
            effect = Effect.FromFiles(@"Assets\Shaders\PlainBuildingStyle_vp.glsl", @"Assets\Shaders\PlainBuildingStyle_fp.glsl");
            effect.Use();

            float aspectRatio = ClientSize.Width / (float)(ClientSize.Height);
            Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, aspectRatio, 1, 100, out projectionMatrix);
            modelviewMatrix = Matrix4.LookAt(new Vector3(0, 6, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            wallTex = LoadTextureArray(
                    @"Assets\Textures\brick-grey.jpg", 
                    @"Assets\Textures\brick-orange.jpg",
                    @"Assets\Textures\brick-red-grey.jpg", 
                    @"Assets\Textures\brick-red.jpg",
                    @"Assets\Textures\brick-white.jpg", 
                    @"Assets\Textures\brick-orange.jpg"                    
                );
            windowTex = LoadTextureArray(
                    @"Assets\Textures\window01.png",
                    @"Assets\Textures\window01blend.png",
                    @"Assets\Textures\window02.png",
                    @"Assets\Textures\window02blend.png",
                    @"Assets\Textures\window03.png",
                    @"Assets\Textures\window03blend.png",
                    @"Assets\Textures\window04.png",
                    @"Assets\Textures\window04blend.png",
                    @"Assets\Textures\window05.png",
                    @"Assets\Textures\window05blend.png"
                );

            interiorTex = LoadTextureArray(
                   @"Assets\Textures\interior01.png",
                   @"Assets\Textures\interior02.png",
                   @"Assets\Textures\interior03.png",
                   @"Assets\Textures\interior04.png"
               );

            doorTex = LoadTextureArray(
               @"Assets\Textures\door01.png"
           );


            effect.SetUniformMatrix4(projectionMatrix, "projection_matrix");
            effect.SetUniformMatrix4(modelviewMatrix, "modelview_matrix");
            
        }

        void HandleKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                g = buildingStyle.createBuilding(new Vector3(-2.5f, -2.5f, -2.5f), new Vector3(5f, 5f, 5f));
                g.InitializePrimitive(effect);
            }
        }
       

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Matrix4 rotation = Matrix4.CreateRotationY((float)e.Time);
            Matrix4.Mult(ref rotation, ref modelviewMatrix, out modelviewMatrix);
            effect.SetUniformMatrix4(modelviewMatrix, "modelview_matrix");            

            if (Keyboard[OpenTK.Input.Key.Escape])
                Exit();

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {          
            GL.Viewport(0, 0, Width, Height);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, wallTex);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2DArray, windowTex);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2DArray, interiorTex);
            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2DArray, doorTex);
            effect.Use();
            effect.SetUniform1(0, "wallTextures");
            effect.SetUniform1(1, "windowTextures");
            effect.SetUniform1(2, "interiorTextures");
            effect.SetUniform1(3, "doorTextures");          

            g.Draw(effect);

            SwapBuffers();
        }

        [STAThread]
        public static void Main()
        {
            using (FacadeWindow example = new FacadeWindow())
            {
                //Utilities.SetWindowTitle(example);
                example.Run(60);
            }
        }
    }
}