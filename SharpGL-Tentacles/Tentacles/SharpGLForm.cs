using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Tentacles
{
    public partial class SharpGLForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpGLForm"/> class.
        /// </summary>
        /// 

        Texture texture = new Texture();
        uint CubeDL;

        float time = 0.0f;
        float rotation = 0.0f;

        public SharpGLForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RenderEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Load the identity matrix.
            gl.LoadIdentity();

            rotation += 1.0f;

            gl.Translate(0.0, 0.0, -8.2);
            //  Rotate around the Y axis.
            gl.Rotate(rotation, 0.0f, 1.0f, 0.0f);

            time += 5.1f;
            DrawObject(time, gl);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            // Initialise the Tentacles
            InitData();

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  A bit of extra initialisation here, we have to enable textures.
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            //  Create our texture object from a file. This creates the texture for OpenGL.
            texture.Create(gl, "tunnelTexture.bmp");

            //  Set the clear color.
            gl.ClearColor(0, 0, 0, 0);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, EventArgs e)
        {
            //  TODO: Set the projection matrix here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            gl.Perspective(60.0f, (double)Width / (double)Height, 0.01, 100.0);

            //  Use the 'look at' helper function to position and aim the camera.
            gl.LookAt(0, 0, 12, 0, 0, 0, 0, 1, 0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        //{------------------------------------------------------------------}
        //{  Draw a tentacle
        //{------------------------------------------------------------------}
        private void DrawTentacle(int Step, float angle, float twistangle, OpenGL gl)
        {
            if (Step == 19) return;
            // Translate along the x-axis
            gl.Translate(2.25, 0, 0);
            gl.Rotate(angle, 0, 1, 0);
            gl.Rotate(twistangle, 1, 0, 0);
            gl.CallList(CubeDL);
            gl.Scale(0.9, 0.9, 0.9);
            DrawTentacle(Step + 1, angle, twistangle, gl);
        }

        //{------------------------------------------------------------------}
        //{  Draw the entire tentacle object
        //{------------------------------------------------------------------}
        private void DrawObject(float DemoTime, OpenGL gl)
        {
            gl.Color(1.0, 1.0, 1.0);
            texture.Bind(gl);

            //Draw the first cube
            gl.CallList(CubeDL);
            gl.Scale(0.9, 0.9, 0.9);

            float angle = (float)(25 * Math.Sin(DemoTime / 600));
            float twistangle = (float)(25 * Math.Sin(DemoTime / 800));

            gl.PushMatrix();
            DrawTentacle(0, angle, twistangle, gl);
            gl.PopMatrix();

            angle = (float)(25 * Math.Cos(DemoTime / 500));
            twistangle = (float)(25 * Math.Sin(DemoTime / 800));
            gl.Rotate(90, 0, 1, 0);
            gl.PushMatrix();
            DrawTentacle(0, angle, -twistangle, gl);
            gl.PopMatrix();

            angle = (float)(25 * Math.Sin(DemoTime / 500));
            twistangle = (float)(25 * Math.Cos(DemoTime / 600));
            gl.Rotate(90, 0, 1, 0);
            gl.PushMatrix();
            DrawTentacle(0, angle, twistangle, gl);
            gl.PopMatrix();

            angle = (float)(25 * Math.Sin(DemoTime / 600));
            twistangle = (float)(25 * Math.Sin(DemoTime / 400));
            gl.Rotate(90, 0, 1, 0);
            gl.PushMatrix();
            DrawTentacle(0, angle, -twistangle, gl);
            gl.PopMatrix();

            angle = (float)(25 * Math.Sin(DemoTime / 600));
            twistangle = (float)(25 * Math.Cos(DemoTime / 400));
            gl.Rotate(90, 0, 0, 1);
            gl.PushMatrix();
            DrawTentacle(0, angle, twistangle, gl);
            gl.PopMatrix();

            angle = (float)(25 * Math.Sin(DemoTime / 400));
            twistangle = (float)(25 * Math.Cos(DemoTime / 800));
            gl.Rotate(180, 0, 0, 1);
            gl.PushMatrix();
            DrawTentacle(0, -angle, -twistangle, gl);
            gl.PopMatrix();
        }

        private void InitData()
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            // Initialise tentacles
            CubeDL = gl.GenLists(1);
            gl.NewList(CubeDL, OpenGL.GL_COMPILE);
            gl.Begin(OpenGL.GL_QUADS);
            // Front Face
            gl.TexCoord(0.0, 0.0); gl.Vertex(-1.0, -1.0, 1.0);
            gl.TexCoord(1.0, 0.0); gl.Vertex(1.0, -1.0, 1.0);
            gl.TexCoord(1.0, 1.0); gl.Vertex(1.0, 1.0, 1.0);
            gl.TexCoord(0.0, 1.0); gl.Vertex(-1.0, 1.0, 1.0);
            // Back Face
            gl.TexCoord(1.0, 0.0); gl.Vertex(-1.0, -1.0, -1.0);
            gl.TexCoord(1.0, 1.0); gl.Vertex(-1.0, 1.0, -1.0);
            gl.TexCoord(0.0, 1.0); gl.Vertex(1.0, 1.0, -1.0);
            gl.TexCoord(0.0, 0.0); gl.Vertex(1.0, -1.0, -1.0);
            // Top Face
            gl.TexCoord(0.0, 1.0); gl.Vertex(-1.0, 1.0, -1.0);
            gl.TexCoord(0.0, 0.0); gl.Vertex(-1.0, 1.0, 1.0);
            gl.TexCoord(1.0, 0.0); gl.Vertex(1.0, 1.0, 1.0);
            gl.TexCoord(1.0, 1.0); gl.Vertex(1.0, 1.0, -1.0);
            // Bottom Face
            gl.TexCoord(1.0, 1.0); gl.Vertex(-1.0, -1.0, -1.0);
            gl.TexCoord(0.0, 1.0); gl.Vertex(1.0, -1.0, -1.0);
            gl.TexCoord(0.0, 0.0); gl.Vertex(1.0, -1.0, 1.0);
            gl.TexCoord(1.0, 0.0); gl.Vertex(-1.0, -1.0, 1.0);
            // Right face
            gl.TexCoord(1.0, 0.0); gl.Vertex(1.0, -1.0, -1.0);
            gl.TexCoord(1.0, 1.0); gl.Vertex(1.0, 1.0, -1.0);
            gl.TexCoord(0.0, 1.0); gl.Vertex(1.0, 1.0, 1.0);
            gl.TexCoord(0.0, 0.0); gl.Vertex(1.0, -1.0, 1.0);
            // Left Face
            gl.TexCoord(0.0, 0.0); gl.Vertex(-1.0, -1.0, -1.0);
            gl.TexCoord(1.0, 0.0); gl.Vertex(-1.0, -1.0, 1.0);
            gl.TexCoord(1.0, 1.0); gl.Vertex(-1.0, 1.0, 1.0);
            gl.TexCoord(0.0, 1.0); gl.Vertex(-1.0, 1.0, -1.0);
            gl.End();
            gl.EndList();
        }
    }
}