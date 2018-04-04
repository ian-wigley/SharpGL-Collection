using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace SharpGLWinformsApplication1
{
    /// <summary>
    /// The main form class.
    /// </summary>
    /// 

    [DebuggerDisplay("Count = {verts}")]
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
        long LastTime = 0;
        DateTime startTime, endTime;
        int elapsedMillisecs;

        glCoords[,] tunnel = new glCoords[33, 33];
        uvCoords[,] texCoord = new uvCoords[33, 33];

        Texture texture = new Texture();

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

            angle = (elapsedMillisecs + LastTime) / 32.0f;

            gl.Translate(0.0, 0.0, -4.2);

            // setup texture coordinates
            for (int i = 0; i < 33; i++)
            {
                for (int j = 0; j < 33; j++)
                {
                    texCoord[i, j].U = (float)(i / 32.0f + Math.Cos((angle + 8.0f * j) / 60.0f) / 2.0f);
                    texCoord[i, j].V = (float)(j / 32.0f + ((angle + j) / 120.0f));
                }
            }

            float c = 0;

            // draw tunnel "cylinder"
            for (int j = 0; j < 32; j++)//32
            {
                if (j > 24)
                {
                    c = (float)(1.0 - (j - 24) / 10);
                }
                else
                {
                    c = 1.0f;
                }
                gl.Color(c, c, c);

                //  Bind the texture.
                texture.Bind(gl);
                gl.Begin(OpenGL.GL_QUADS);
                
                for (int i = 0; i < 32; i++)
                {
                    gl.TexCoord(texCoord[i, j].U, texCoord[i, j].V);
                    gl.Vertex(tunnel[i, j].X, tunnel[i, j].Y, tunnel[i, j].Z);

                    gl.TexCoord(texCoord[i + 1, j].U, texCoord[i + 1, j].V);
                    gl.Vertex(tunnel[i + 1, j].X, tunnel[i + 1, j].Y, tunnel[i + 1, j].Z);

                    gl.TexCoord(texCoord[i + 1, j + 1].U, texCoord[i + 1, j + 1].V);
                    gl.Vertex(tunnel[i + 1, j + 1].X, tunnel[i + 1, j + 1].Y, tunnel[i + 1, j + 1].Z);

                    gl.TexCoord(texCoord[i, j + 1].U, texCoord[i, j + 1].V);
                    gl.Vertex(tunnel[i, j + 1].X, tunnel[i, j + 1].Y, tunnel[i, j + 1].Z);
                }
                gl.End();
            }

            endTime = DateTime.Now;
            elapsedMillisecs = (int)((TimeSpan)(endTime - startTime)).TotalMilliseconds;
            LastTime = elapsedMillisecs;
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
            gl.Perspective(45.0f, (double)Width / (double)Height, 0.01, 100.0);

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
