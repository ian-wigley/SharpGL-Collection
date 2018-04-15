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

        // Textures
        Texture texFont = new Texture();
        Texture texBackground = new Texture();
        Texture TexCube = new Texture();
        Texture TexEarth = new Texture();

        uint SphereDL;

        // User vaiables
        float xSpeed, ySpeed, zSpeed, xScrollSpeed;
        float xAngle, yAngle, zAngle, angle;
        float g_TextScroller, g_TextScroller2;
        float g_Zcube;

        float LastTime = 0;
        DateTime startTime = new DateTime();
        DateTime endTime;
        float elapsedTime;

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

            angle += 0.4f;

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            // Clear The Screen And The Depth Buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            // Reset The View
            gl.LoadIdentity();
            gl.PushMatrix();
            gl.Translate(0.0, 2.0, -35);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, TexEarth.TextureName);
            gl.Color(1.0, 1.0, 1.0);
            gl.Rotate(angle, 0.0, 1.0, 0.0);
            gl.CallList(SphereDL);
            gl.PopMatrix();

            gl.LoadIdentity();
            // Draw Rotating Galaxy
            gl.PushMatrix();
            gl.Translate(0.0, 0.0, -30);
            gl.Scale(4.0, 4.0, 4.0);
            // Bind the Texture to the object
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texBackground.TextureName);
            gl.Rotate(elapsedTime / 100, 0, 0, 1);
            gl.Color(1.0, 1.0, 1.0);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0, 0.0); gl.Vertex(-7.6, -7.6, 0);
            gl.TexCoord(1.0, 0.0); gl.Vertex(7.6, -7.6, 0);
            gl.TexCoord(1.0, 1.0); gl.Vertex(7.6, 7.6, 0);
            gl.TexCoord(0.0, 1.0); gl.Vertex(-7.6, 7.6, 0);
            gl.End();
            gl.PopMatrix();

            // Draw Text Strings
            gl.PushMatrix();
            gl.Translate(0.0, 0 + g_TextScroller, -3);
            // Bind the Texture to the object
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, texFont.TextureName);  
            // Line One
            glImgWrite("this is some test text.");

            // Line Two
            gl.Translate(0.0, -0.1, 0.0);
            glImgWrite("here we have a second line of text.");

            // Rolling Text
            gl.Translate(0.0, -0.5, 0.0);
            gl.Rotate(-(elapsedTime / 10), 1, 0, 0);
            glImgWrite("lets see of this text can rotate.");

            xAngle = xAngle + xSpeed;
            yAngle = yAngle + ySpeed;
            zAngle = zAngle + zSpeed;
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(2 - g_TextScroller2, -0.35, -1.03);
            gl.Rotate(-(elapsedTime / 10), 1, 0, 0);
            glImgWrite("test horizontal.........www.sulaco.co.za");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(-0.60, 0.6, -1.9);
            gl.Rotate(-(elapsedTime / 10), 1, 0, 0);
            glImgWrite("FPS : ");// + FPSCountTxt);
            gl.PopMatrix();

            // Text Scrolling from the top, downwards
            g_TextScroller = g_TextScroller + 0.008f;
            g_TextScroller2 = g_TextScroller2 + xScrollSpeed;

            if (g_TextScroller > 1.5)
            {
                g_TextScroller = -1.5f;
            }
            if (g_TextScroller2 > 5.5)
            {
                g_TextScroller2 = -1.5f;
            }

            gl.PushMatrix();
            gl.Translate(0.0, 2.0 - g_TextScroller, -4.1);
            glImgWrite("text scrolling from the top.");
            gl.PopMatrix();

            // Growing Text
            gl.PushMatrix();
            //gl.Translate(0.0,0.0,8.0 * g_TextScroller);
            gl.Translate(0.0, 2.8 * -g_TextScroller, 8 * g_TextScroller);
            glImgWrite("www.opengl..org");
            gl.PopMatrix();


            gl.LoadIdentity();
            gl.PushMatrix();
            gl.Translate(-2, -0.5, -7);
            //{ For movement that requires a constant speed on all machines use ... }
            //gl.Rotate(ElapsedTime/20, 1, 0, 0);
            //gl.Rotate(ElapsedTime/30, 0, 1, 0);

            gl.Rotate(xAngle, 1, 0, 0);
            gl.Rotate(yAngle, 0, 1, 0);
            gl.Rotate(zAngle, 0, 0, 1);

            //Left Cube
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, TexCube.TextureName);  // Bind the Texture to the object
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

            gl.LoadIdentity();
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PushMatrix(); //Sauvegarde la matrice active
            gl.Scale(0.4, 0.4, 0.4); //Redimensionne l'objet
            gl.Translate(5, -1, g_Zcube); //Translation (x,y,z)
            gl.Rotate(-angle, 0, 1, 0);
            gl.Rotate(-angle, 1, 0, 0);
            gl.Rotate(-angle, 0, 0, 1);
            gl.Begin(OpenGL.GL_QUADS);

            //1 ère face\\
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(2, 2, 2); //1p
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(2, -2, 2); //2p
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(-2, -2, 2); //3p
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-2, 2, 2); //4p

            //2 ème face\\
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(2, 2, -2); //1p
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(2, -2, -2); //2p
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-2, -2, -2); //3p
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-2, 2, -2); //4p

            //3 ème face\\
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(2, 2, 2); //1p
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(2, -2, 2); //2p
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(2, -2, -2); //3p
            gl.Color(1.0f, 1.0f, 0.0f);
            gl.Vertex(2, 2, -2); //4p

            //4 ème face\\
            gl.Color(1.0f, 1.0f, 0.0f);
            gl.Vertex(-2, 2, 2); //1p
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-2, -2, 2); //2p
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(-2, -2, -2); //3p
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(-2, 2, -2); //4p

            //5 ème face\\
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-2, 2, -2); //1p
            gl.Color(1.0f, 1.0f, 0.0f);
            gl.Vertex(-2, 2, 2); //2p
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(2, 2, 2); //3p
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(2, 2, -2); //4p

            //6 ème face\\
            gl.Color(1.0f, 1.0f, 0.0f);
            gl.Vertex(-2, -2, -2); //1p
            gl.Color(0.0f, 1.0f, 0.0f);
            gl.Vertex(-2, -2, 2); //2p
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.Vertex(2, -2, 2); //3p
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(2, -2, -2); //4p
            gl.End(); //Finis de dessiner le carré
            gl.PopMatrix(); //Recupérer la matrice précédemment sauvegarder
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.Flush();

            LastTime = elapsedTime;
            endTime = DateTime.Now;
            elapsedTime = (float)((TimeSpan)(endTime - startTime)).TotalMilliseconds;
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

            //  Set the clear color.
            gl.ClearColor(0, 0, 0, 0);

            gl.ShadeModel(OpenGL.GL_SMOOTH);                                    // Enables Smooth Color Shading
            gl.ClearDepth(1.0);                                                 // Depth Buffer Setup
            gl.Enable(OpenGL.GL_DEPTH_TEST);                                    // Enable Depth Buffer
            gl.DepthFunc(OpenGL.GL_LESS);                                       // The Type Of Depth Test To Do
            gl.Enable(OpenGL.GL_ALPHA_TEST);
            gl.AlphaFunc(OpenGL.GL_GREATER, 0.4f);
            gl.Hint(OpenGL.GL_PERSPECTIVE_CORRECTION_HINT, OpenGL.GL_NICEST);   //Realy Nice perspective calculations
            gl.Enable(OpenGL.GL_TEXTURE_2D);                                     // Enable Texture Mapping

            //  Create our texture object from a file. This creates the texture for OpenGL.
            texFont.Create(gl, "FontLines2.png");
            texBackground.Create(gl, "galaxy.jpg");
            TexCube.Create(gl, "texture.jpg");
            TexEarth.Create(gl, "earth.jpg");

            xSpeed = 0.9f;   // start with some movement
            ySpeed = 0.9f;
            zSpeed = 0.9f;
            xScrollSpeed = 0.005f;

            g_TextScroller = -1.5f;
            g_TextScroller2 = -1.5f;
            g_Zcube = -15.5f;
            CreateSphere(0, 0, 0, 10, 48);
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

        //{------------------------------------------------------------------}
        //{  Function to Create a sphere                                     }
        //{------------------------------------------------------------------}
        private void CreateSphere(float CX, float CY, float CZ, float Radius, int n)
        {
            float theta1;
            float theta2;
            float theta3;
            float x, y, z, px, py, pz;

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            SphereDL = gl.GenLists(1);
            gl.NewList(SphereDL, OpenGL.GL_COMPILE);

            if (Radius < 0) { Radius = -Radius; }
            if (n < 0) { n = -n; }

            if ((n < 4) || (Radius <= 0))
            {
                gl.Begin(OpenGL.GL_POINTS);
                gl.Vertex(CX, CY, CZ);
                gl.End();
            }

            for (int j = 0; j < n / 2; j++)
            {
                theta1 = (float)(j * 2 * Math.PI / n - Math.PI / 2);
                theta2 = (float)((j + 1) * 2 * Math.PI / n - Math.PI / 2);
                gl.Begin(OpenGL.GL_QUAD_STRIP);
                for (int i = 0; i < n + 1; i++)
                {
                    theta3 = (float)(i * 2 * Math.PI / n);
                    x = (float)(Math.Cos(theta2) * Math.Cos(theta3));
                    y = (float)(Math.Sin(theta2));
                    z = (float)(Math.Cos(theta2) * Math.Sin(theta3));
                    px = CX + Radius * x;
                    py = CY + Radius * y;
                    pz = CZ + Radius * z;

                    gl.Normal(x, y, z);
                    gl.TexCoord((1.0f - i) / n, (2.0f * (j + 1.0f)) / n);
                    gl.Vertex(px, py, pz);

                    x = (float)(Math.Cos(theta1) * Math.Cos(theta3));
                    y = (float)(Math.Sin(theta1));
                    z = (float)(Math.Cos(theta1) * Math.Sin(theta3));
                    px = CX + Radius * x;
                    py = CY + Radius * y;
                    pz = CZ + Radius * z;

                    //        glNormal3(X, Y, Z);
                    gl.TexCoord((1.0f - i) / n, (2.0f * j) / n);
                    gl.Vertex(px, py, pz);
                }
                gl.End();
            }
            gl.EndList();
        }

        //{------------------------------------------------------------------}
        //{  Renders the text to a row of polygons and keeps the origin      }
        //{  in the centre of the string                                     }
        //{------------------------------------------------------------------}
        private void glImgWrite(string strText)
        {
            int intAsciiCode;
            float imgcharWidth;
            float imgcharPosX;

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
    }
}