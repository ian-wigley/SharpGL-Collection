using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Windows.Forms;

namespace Tentacles
{
    public partial class SharpGLForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpGLForm"/> class.
        /// </summary>
        /// 

        // User vaiables
        float LastTime = 0;
        DateTime startTime = new DateTime();
        DateTime endTime;
        float elapsedTime;
        float xScrollSpeed = 0;
        float g_TextScroller2 = 1;
        string FPSCountTxt = "";
        int wavePos1 = 0;
        int wavePos2 = 0;
        // Gap Between Character n et n+1
        int DecalSpaceChar = 5;
        // Define Height of wave (increase value decrease height wave)
        int amplitude2 = 10;
        float WaveValue = 0;
        // Sinus values pre-calculated
        float[] tabSin = new float[360];
        // Initialize sinus axis deformation
        bool xAxisWave = false, yAxisWave = true, zAxisWave = false;
        Texture texFont = new Texture();

        public SharpGLForm()
        {
            startTime = DateTime.Now;
            InitializeComponent();
            FilledTabSinus();
        }

        private void FilledTabSinus()
        {
            for (int i = 0; i < 360; i++)
            {
                tabSin[i] = (float)Math.Sin(i * Math.PI / 40.0f) / amplitude2;
            }
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

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);    // Clear The Screen And The Depth Buffer
            gl.LoadIdentity();                                                      // Reset The View
            gl.Color(1.0, 1.0, 1.0);
            wavePos2 = wavePos1;
            string mytxt = "new sinusoidal scrolling............. Press X, Y, Z, 1, 2, 3, or 4 to change view wave !   ";
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texFont.TextureName);
            gl.PushMatrix();
            gl.Translate(2 - g_TextScroller2, 0.1, -1.53);
            // Read Char by Char to make gap
            for (int i = 0; i < mytxt.Length; i++)
            {
                // Horizontal Scrolling
                gl.Translate(0.08, 0.0, 0.0);
                gl.PushMatrix();
                // Mod 360 allowed to curl on sin tab value
                WaveValue = tabSin[wavePos2 % 360];
                if (xAxisWave && !yAxisWave && !zAxisWave) { gl.Translate(WaveValue, 0.0, 0.0); }
                if (!xAxisWave && yAxisWave && !zAxisWave) { gl.Translate(0.0, WaveValue, 0.0); }
                if (!xAxisWave && !yAxisWave && zAxisWave) { gl.Translate(0.0, 0.0, WaveValue); }
                if (xAxisWave && yAxisWave && !zAxisWave) { gl.Translate(WaveValue, WaveValue, 0.0); }
                if (xAxisWave && !yAxisWave && zAxisWave) { gl.Translate(WaveValue, 0.0, WaveValue); }
                if (!xAxisWave && yAxisWave && zAxisWave) { gl.Translate(0.0, WaveValue, WaveValue); }
                if (xAxisWave && yAxisWave && zAxisWave) { gl.Translate(WaveValue, WaveValue, WaveValue); }
                //Stock CurrentIndex - n previous index to have a constant gap
                wavePos2 = wavePos2 - DecalSpaceChar;
                if (wavePos2 < 0)
                {
                    wavePos2 = tabSin.Length + wavePos2;
                }
                // Display One character of our string
                glImgWrite(mytxt[i]);
                gl.PopMatrix();
            }
            gl.PopMatrix();
            wavePos1 = (wavePos1 + 1);
            // Increment Scroll
            g_TextScroller2 = g_TextScroller2 + xScrollSpeed;
            // Wrap to beginning if  g_TextScroller2 greater than 11
            if (g_TextScroller2 > 10.0)
            {
                g_TextScroller2 = 1;
            }

            gl.Flush();
            elapsedTime = 100 * (float)((TimeSpan)(endTime - startTime)).TotalMilliseconds;
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            gl.ShadeModel(OpenGL.GL_SMOOTH);                 // Enables Smooth Color Shading
            gl.ClearDepth(1.0);                              // Depth Buffer Setup
            gl.Enable(OpenGL.GL_DEPTH_TEST);                 // Enable Depth Buffer
            gl.DepthFunc(OpenGL.GL_LESS);                    // The Type Of Depth Test To Do
            gl.Enable(OpenGL.GL_ALPHA_TEST);
            gl.AlphaFunc(OpenGL.GL_GREATER, 0.4f);
            gl.Hint(OpenGL.GL_PERSPECTIVE_CORRECTION_HINT, OpenGL.GL_NICEST);   //Realy Nice perspective calculations
            gl.Enable(OpenGL.GL_TEXTURE_2D);                     // Enable Texture Mapping

            //  Create our texture object from a file. This creates the texture for OpenGL.
            texFont.Create(gl, "FontLines2.png");

            xScrollSpeed = 0.025f;
            g_TextScroller2 = 1.0f;

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
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);

            //  Load the identity.
            gl.LoadIdentity();

            //  Create a perspective transformation.
            gl.Perspective(45.0f, Width / (double)Height, 0.01, 100.0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        private void glImgWrite(char sstrText)
        {
            int intAsciiCode;
            float imgcharWidth;
            float imgcharPosX;

            string strText = sstrText.ToString();

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            imgcharWidth = 1.0f / 66;
            strText = strText.ToUpper();

            for (int i = 0; i < strText.Length; i++)
            {
                char s = strText[i];
                if (s > 31) //only handle 66 chars
                {
                    intAsciiCode = s - 32;
                    // Find the character position from the origin [0.0 , 0.0 , 0.0]  to center the text
                    imgcharPosX = (float)(strText.Length / 2 * 0.08 - strText.Length * 0.08 + (i - 1) * 0.08);

                    gl.Begin(OpenGL.GL_QUADS);

                    gl.TexCoord(imgcharWidth * intAsciiCode, 0.0);
                    gl.Vertex(-0.04 + imgcharPosX, -0.04, 0.0);

                    gl.TexCoord(imgcharWidth * intAsciiCode + imgcharWidth, 0.0);
                    gl.Vertex(0.04 + imgcharPosX, -0.04, 0.0);

                    gl.TexCoord(imgcharWidth * intAsciiCode + imgcharWidth, 1.0);
                    gl.Vertex(0.04 + imgcharPosX, 0.04, 0.0);

                    gl.TexCoord(imgcharWidth * intAsciiCode, 1.0);
                    gl.Vertex(-0.04 + imgcharPosX, 0.04, 0.0);
                    gl.End();
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                switch (keyData)
                {
                    case Keys.X:
                        xAxisWave = true; yAxisWave = false; zAxisWave = false;
                        break;

                    case Keys.Y:
                        xAxisWave = false; yAxisWave = true; zAxisWave = false;
                        break;

                    case Keys.Z:
                        xAxisWave = false; yAxisWave = false; zAxisWave = true;
                        break;

                    case Keys.D1:
                        xAxisWave = true; yAxisWave = true; zAxisWave = false;
                        break;

                    case Keys.D2:
                        xAxisWave = true; yAxisWave = false; zAxisWave = true;
                        break;

                    case Keys.D3:
                        xAxisWave = false; yAxisWave = true; zAxisWave = true;
                        break;

                    case Keys.D4:
                        xAxisWave = true; yAxisWave = true; zAxisWave = true;
                        break;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}