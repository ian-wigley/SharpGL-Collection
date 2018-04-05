using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Windows.Forms;

namespace Peristalis
{
    /// <summary>
    /// The main form class.
    /// </summary>
    /// 

    public partial class SharpGLForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpGLForm"/> class.
        /// </summary>
        /// 

        public struct glCoords
        {
            public float X;
            public float Y;
            public float Z;
        }

        public struct uvCoords
        {
            public float U;
            public float V;
        }

        float angle = 0;
        long lastTime = 0;
        DateTime startTime, endTime;
        int elapsedMillisecs;

        glCoords[,] tunnel = new glCoords[33, 33];
        uvCoords[,] texCoord = new uvCoords[33, 33];

        Texture backGroundTexture = new Texture();
        Texture tubeTexture = new Texture();

        private const int MAX_SEGMENTS = 64;
        private const int MAX_DIVS = 16;

        float[] twist = new float[MAX_SEGMENTS];
        float[] radius = new float[MAX_SEGMENTS];
        float[] xpos = new float[MAX_SEGMENTS];

        public SharpGLForm()
        {
            startTime = DateTime.Now;
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

            angle += 0.1f;

            gl.Color(1.0, 1.0, 1.0);

            //--- Background effect ---//
            gl.PushMatrix();
            gl.Translate(0, 0, -5);
            gl.Rotate(25 * angle, 1.0, 0.0, 0.0);
            gl.Rotate(45 * angle, 0.0, 1.0, 0.0);

            gl.Scale(5, 5, 5);
            backGroundTexture.Bind(gl);
            gl.Begin(OpenGL.GL_QUADS);
            // Front Face
            gl.Normal(0.0, 0.0, 1.0);
            gl.TexCoord(0.0, 0.0); gl.Vertex(-1.0, -1.0, 1.0);
            gl.TexCoord(1.0, 0.0); gl.Vertex(1.0, -1.0, 1.0);
            gl.TexCoord(1.0, 1.0); gl.Vertex(1.0, 1.0, 1.0);
            gl.TexCoord(0.0, 1.0); gl.Vertex(-1.0, 1.0, 1.0);
            // Back Face
            gl.Normal(0.0, 0.0, -1.0);
            gl.TexCoord(1.0, 0.0); gl.Vertex(-1.0, -1.0, -1.0);
            gl.TexCoord(1.0, 1.0); gl.Vertex(-1.0, 1.0, -1.0);
            gl.TexCoord(0.0, 1.0); gl.Vertex(1.0, 1.0, -1.0);
            gl.TexCoord(0.0, 0.0); gl.Vertex(1.0, -1.0, -1.0);
            // Top Face
            gl.Normal(0.0, 1.0, 0.0);
            gl.TexCoord(0.0, 1.0); gl.Vertex(-1.0, 1.0, -1.0);
            gl.TexCoord(0.0, 0.0); gl.Vertex(-1.0, 1.0, 1.0);
            gl.TexCoord(1.0, 0.0); gl.Vertex(1.0, 1.0, 1.0);
            gl.TexCoord(1.0, 1.0); gl.Vertex(1.0, 1.0, -1.0);
            // Bottom Face
            gl.Normal(0.0, -1.0, 0.0);
            gl.TexCoord(1.0, 1.0); gl.Vertex(-1.0, -1.0, -1.0);
            gl.TexCoord(0.0, 1.0); gl.Vertex(1.0, -1.0, -1.0);
            gl.TexCoord(0.0, 0.0); gl.Vertex(1.0, -1.0, 1.0);
            gl.TexCoord(1.0, 0.0); gl.Vertex(-1.0, -1.0, 1.0);
            // Right face
            gl.Normal(1.0, 0.0, 0.0);
            gl.TexCoord(1.0, 0.0); gl.Vertex(1.0, -1.0, -1.0);
            gl.TexCoord(1.0, 1.0); gl.Vertex(1.0, 1.0, -1.0);
            gl.TexCoord(0.0, 1.0); gl.Vertex(1.0, 1.0, 1.0);
            gl.TexCoord(0.0, 0.0); gl.Vertex(1.0, -1.0, 1.0);
            // Left Face
            gl.Normal(-1.0, 0.0, 0.0);
            gl.TexCoord(0.0, 0.0); gl.Vertex(-1.0, -1.0, -1.0);
            gl.TexCoord(1.0, 0.0); gl.Vertex(-1.0, -1.0, 1.0);
            gl.TexCoord(1.0, 1.0); gl.Vertex(-1.0, 1.0, 1.0);
            gl.TexCoord(0.0, 1.0); gl.Vertex(-1.0, 1.0, -1.0);
            gl.End();
            gl.PopMatrix();

            tubeTexture.Bind(gl);
            gl.Translate(0.0f, 0.0f, -4.6f);
            gl.Rotate(angle * 4.0f, 0.0f, 0.0f, 1.0f);
            gl.Translate(0.0f, -2.75f, 0.0f);

            //--- DrawCylinder ---//
            gl.Begin(OpenGL.GL_QUADS);
            for (int i = 0; i < MAX_SEGMENTS - 1; i++)
            {
                for (int j = 0; j < MAX_DIVS; j++)
                {
                    //  i, j
                    gl.TexCoord(twist[i] + j * 4.0f / MAX_DIVS, i * 5.5f / MAX_SEGMENTS);
                    gl.Vertex(xpos[i] + 0.7f * radius[i] * Math.Cos(j * Math.PI * 2.0f / MAX_DIVS), i * 5.5f / MAX_SEGMENTS, 0.7f * radius[i] * Math.Sin(j * Math.PI * 2.0f / MAX_DIVS));

                    //  i + 1, j
                    gl.TexCoord(twist[i + 1] + j * 4.0f / MAX_DIVS, (i + 1) * 5.5f / MAX_SEGMENTS);
                    gl.Vertex(xpos[i + 1] + 0.7f * radius[i + 1] * Math.Cos(j * Math.PI * 2.0f / MAX_DIVS), (i + 1) * 5.5f / MAX_SEGMENTS, 0.7f * radius[i + 1] * Math.Sin(j * Math.PI * 2.0f / MAX_DIVS));

                    //  i + 1, j + 1
                    gl.TexCoord(twist[i + 1] + (j + 1) * 4.0f / MAX_DIVS, (i + 1) * 5.5f / MAX_SEGMENTS);
                    gl.Vertex(xpos[i + 1] + 0.7f * radius[i + 1] * Math.Cos((j + 1) * Math.PI * 2.0f / MAX_DIVS), (i + 1) * 5.5 / MAX_SEGMENTS, 0.7f * radius[i + 1] * Math.Sin((j + 1) * Math.PI * 2.0f / MAX_DIVS));


                    //  i, j + 1
                    gl.TexCoord(twist[i] + (j + 1) * 4.0f / MAX_DIVS, i * 5.5f / MAX_SEGMENTS);
                    gl.Vertex(xpos[i] + 0.7f * radius[i] * Math.Cos((j + 1) * Math.PI * 2.0 / MAX_DIVS), i * 5.5f / MAX_SEGMENTS, 0.7f * radius[i] * Math.Sin((j + 1) * Math.PI * 2.0f / MAX_DIVS));
                }
            }
            gl.End();

            // Recalculate values
            for (int i = 0; i < MAX_SEGMENTS - 1; i++)
            {
                twist[i] = (float)(0.35f * Math.Sin(angle * 0.78f + i / 12.0f) + 0.35f * Math.Sin(angle * -1.23f + i / 18.0f));
                radius[i] = (float)(0.30f * Math.Sin(angle + i / 8.0f) + 0.80f + 0.15f * Math.Sin(angle * -0.8f + i / 3.0f));
                xpos[i] = (float)(0.25f * Math.Sin(angle * 1.23f + i / 5.0f) + 0.30f * Math.Sin(angle * 0.9f + i / 6.0f));
            }

            endTime = DateTime.Now;
            elapsedMillisecs = (int)((TimeSpan)(endTime - startTime)).TotalMilliseconds;
            lastTime = elapsedMillisecs;
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            //  TODO: Initialise OpenGL here.
            CreateTunnel();

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  A bit of extra initialisation here, we have to enable textures.
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            //  Create our texture object from a file. This creates the texture for OpenGL.
            backGroundTexture.Create(gl, "background.bmp");
            tubeTexture.Create(gl, "tunnelTexture.bmp");
            
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

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void CreateTunnel()
        {
            for (int i = 0; i < 33; i++)
            {
                for (int j = 0; j < 33; j++)
                {
                    tunnel[i, j].X = (float)((3 - j / 12) * Math.Cos(2 * Math.PI / 32 * i));
                    tunnel[i, j].Y = (float)((3 - j / 12) * Math.Sin(2 * Math.PI / 32 * i));
                    tunnel[i, j].Z = -j;
                }
            }
        }
    }
}