using SharpGL;
using SharpGL.SceneGraph.Assets;
using System;
using System.Windows.Forms;

namespace SpecularTorus
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

        float Angle = 0;
        long LastTime = 0;
        DateTime startTime, endTime;
        int elapsedMillisecs;

        Texture texture = new Texture();

        private const int MAX_SEGMENTS = 64;
        private const int MAX_DIVS = 16;

        uint TorusDL;

        // Lights
        float[] LightPos = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };
        float[] MatDiffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        float[] MatSpecular = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
        float MatShine = 45.0f;

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

            Angle = (elapsedMillisecs / 300);
            int ElapsedTime = elapsedMillisecs;

            gl.Color(1.0, 1.0, 1.0);

            gl.Translate(0.0, 0.0, -14);
            gl.Rotate(ElapsedTime / 20, 1, 0, 0);
            gl.CallList(TorusDL);

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

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  Set the clear color.
            gl.ClearColor(0, 0, 0, 0);
            // Enables Smooth Color Shading
            gl.ShadeModel(OpenGL.GL_SMOOTH);
            // Depth Buffer Setup
            gl.ClearDepth(1.0);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            // The Type Of Depth Test To Do
            gl.DepthFunc(OpenGL.GL_LESS);

            //Realy Nice perspective calculations
            gl.Hint(OpenGL.GL_PERSPECTIVE_CORRECTION_HINT, OpenGL.GL_NICEST);

            // Turn on OpenGL lighting
            gl.Enable(OpenGL.GL_LIGHTING);

            // Create a light
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, @LightPos);
            gl.Enable(OpenGL.GL_LIGHT0);

            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, @MatDiffuse);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SPECULAR, @MatSpecular);
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_SHININESS, @MatShine);

            // Enable Texture Mapping
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexGen(OpenGL.GL_S, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_SPHERE_MAP);
            gl.TexGen(OpenGL.GL_T, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_SPHERE_MAP);
            // Enable spherical
            gl.Enable(OpenGL.GL_TEXTURE_GEN_S);
            // Environment Mapping
            gl.Enable(OpenGL.GL_TEXTURE_GEN_T);

            int Segments = 64;
            CreateTorus(1, 3, Segments, Segments, gl);

            //  A bit of extra initialisation here, we have to enable textures.
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            //  Create our texture object from a file. This creates the texture for OpenGL.
            texture.Create(gl, "Reflection.bmp");
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

        //{------------------------------------------------------------------}
        //{  Function to create a torus       (code by Jason Allen)          }
        //{------------------------------------------------------------------}
        private void CreateTorus(float TubeRadius, float Radius, int Sides, int Rings, OpenGL gl)
        {

            float theta, phi, theta1;
            float cosTheta, sinTheta;
            float cosTheta1, sinTheta1;
            float ringDelta, sideDelta;
            float cosPhi, sinPhi, dist;

            sideDelta = (float)(2.0f * Math.PI / Sides);
            ringDelta = (float)(2.0f * Math.PI / Rings);

            theta = 0.0f;
            cosTheta = 1.0f;
            sinTheta = 0.0f;

            TorusDL = gl.GenLists(1);
            gl.NewList(TorusDL, OpenGL.GL_COMPILE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 1);

            for (int i = Rings; i > -1; i--)
            {

                theta1 = theta + ringDelta;
                cosTheta1 = (float)(Math.Cos(theta1));
                sinTheta1 = (float)(Math.Sin(theta1));
                gl.Begin(OpenGL.GL_QUAD_STRIP);
                phi = 0.0f;

                for (int j = Sides; j > -1; j--)
                {
                    phi = phi + sideDelta;
                    cosPhi = (float)(Math.Cos(phi));
                    sinPhi = (float)(Math.Sin(phi));
                    dist = Radius + (TubeRadius * cosPhi);
                    gl.Normal(cosTheta1 * cosPhi, -sinTheta1 * cosPhi, sinPhi);
                    gl.Vertex(cosTheta1 * dist, -sinTheta1 * dist, TubeRadius * sinPhi);
                    gl.Normal(cosTheta * cosPhi, -sinTheta * cosPhi, sinPhi);
                    gl.Vertex(cosTheta * dist, -sinTheta * dist, TubeRadius * sinPhi);

                }
                gl.End();
                theta = theta1;
                cosTheta = cosTheta1;
                sinTheta = sinTheta1;
            }
            gl.EndList();
        }
    }
}