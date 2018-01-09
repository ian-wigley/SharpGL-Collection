using System;
using System.Diagnostics;
using System.Windows.Forms;
using SharpGL;

namespace SharpGLWinformsApplication1
{

    [DebuggerDisplay("Count = {verts}")]
    public partial class SharpGLForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpGLForm"/> class.
        /// </summary>
        /// 

        private float fPitch = 0.0f;
        private float fYaw = 0.0f;
        private float fTime = 0.0f;

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
            vDrawScene();
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            main();
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
            //gl.LookAt(-5, 5, -5, 0, 0, 0, 0, 1, 0);

            //  Set the modelview matrix.
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }

        struct GLvector
        {
            public float fX;
            public float fY;
            public float fZ;
        };

        //These tables are used so that everything can be done in little loops that you can look at all at once
        // rather than in pages and pages of unrolled code.

        //a2fVertexOffset lists the positions, relative to vertex0, of each of the 8 vertices of a cube
        float[,] a2fVertexOffset = new float[8, 3]
        {
            { 0.0f, 0.0f, 0.0f },{ 1.0f, 0.0f, 0.0f },{ 1.0f, 1.0f, 0.0f },{ 0.0f, 1.0f, 0.0f },
            { 0.0f, 0.0f, 1.0f },{ 1.0f, 0.0f, 1.0f },{ 1.0f, 1.0f, 1.0f },{ 0.0f, 1.0f, 1.0f }
        };

        //a2iEdgeConnection lists the index of the endpoint vertices for each of the 12 edges of the cube
        int[,] a2iEdgeConnection = new int[12, 2]
        {
            { 0,1 },{ 1,2 },{ 2,3 },{ 3,0 },
            { 4,5 },{ 5,6 },{ 6,7 },{ 7,4 },
            { 0,4 },{ 1,5 },{ 2,6 },{ 3,7 }
        };

        //a2fEdgeDirection lists the direction vector (vertex1-vertex0) for each edge in the cube
        float[,] a2fEdgeDirection =  {
            { 1.0f, 0.0f, 0.0f },{ 0.0f, 1.0f, 0.0f },{ -1.0f, 0.0f, 0.0f },{ 0.0f, -1.0f, 0.0f },
            { 1.0f, 0.0f, 0.0f },{ 0.0f, 1.0f, 0.0f },{ -1.0f, 0.0f, 0.0f },{ 0.0f, -1.0f, 0.0f },
            { 0.0f, 0.0f, 1.0f },{ 0.0f, 0.0f, 1.0f },{ 0.0f, 0.0f, 1.0f },{ 0.0f,  0.0f, 1.0f }
        };


        //a2iTetrahedronEdgeConnection lists the index of the endpoint vertices for each of the 6 edges of the tetrahedron
        int[,] a2iTetrahedronEdgeConnection = //[6][2] =
       {
            { 0,1 },{ 1,2 },{ 2,0 },{ 0,3 },{ 1,3 },{ 2,3 }
        };

        //a2iTetrahedronEdgeConnection lists the index of verticies from a cube 
        // that made up each of the six tetrahedrons within the cube
        int[,] a2iTetrahedronsInACube = //[6][4] =
        {
            { 0,5,1,6 },
            { 0,1,2,6 },
            { 0,2,3,6 },
            { 0,3,7,6 },
            { 0,7,4,6 },
            { 0,4,5,6 },
        };

        float[] afAmbientWhite = { 0.25f, 0.25f, 0.25f, 1.00f };
        float[] afAmbientRed = { 0.25f, 0.00f, 0.00f, 1.00f };
        float[] afAmbientGreen = { 0.00f, 0.25f, 0.00f, 1.00f };
        float[] afAmbientBlue = { 0.00f, 0.00f, 0.25f, 1.00f };
        float[] afDiffuseWhite = { 0.75f, 0.75f, 0.75f, 1.00f };
        float[] afDiffuseRed = { 0.75f, 0.00f, 0.00f, 1.00f };
        float[] afDiffuseGreen = { 0.00f, 0.75f, 0.00f, 1.00f };
        float[] afDiffuseBlue = { 0.00f, 0.00f, 0.75f, 1.00f };
        float[] afSpecularWhite = { 1.00f, 1.00f, 1.00f, 1.00f };
        float[] afSpecularRed = { 1.00f, 0.25f, 0.25f, 1.00f };
        float[] afSpecularGreen = { 0.25f, 1.00f, 0.25f, 1.00f };
        float[] afSpecularBlue = { 0.25f, 0.25f, 1.00f, 1.00f };


        uint ePolygonMode = 0x1B02;
        int iDataSetSize = 16;
        float fStepSize = 0;
        float fTargetValue = 48.0f;
        GLvector[] sSourcePoint = new GLvector[3];
        bool bSpin = true;
        bool bMove = true;
        bool bLight = true;

        void main()
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            fStepSize = 1.0f / iDataSetSize;

            float[] afPropertiesAmbient = { 0.50f, 0.50f, 0.50f, 1.00f };
            float[] afPropertiesDiffuse = { 0.75f, 0.75f, 0.75f, 1.00f };
            float[] afPropertiesSpecular = { 1.00f, 1.00f, 1.00f, 1.00f };

            //    GLsizei iWidth = 640.0;
            //    GLsizei iHeight = 480.0;

            //    glutInit(&argc, argv);
            //    glutInitWindowPosition(0, 0);
            //    glutInitWindowSize(iWidth, iHeight);
            //    glutInitDisplayMode(GLUT_RGB | GLUT_DEPTH | GLUT_DOUBLE);
            //    glutCreateWindow("Marching Cubes");
            //    glutDisplayFunc(vDrawScene);
            //    glutIdleFunc(vIdle);
            //    glutReshapeFunc(vResize);
            //    glutKeyboardFunc(vKeyboard);
            //    glutSpecialFunc(vSpecial);

            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.ClearDepth(1.0f);

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, ePolygonMode);

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, afPropertiesAmbient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, afPropertiesDiffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, afPropertiesSpecular);
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_TWO_SIDE, 1.0f);

            gl.Enable(OpenGL.GL_LIGHT0);

            gl.Material(OpenGL.GL_BACK, OpenGL.GL_AMBIENT, afAmbientGreen);
            gl.Material(OpenGL.GL_BACK, OpenGL.GL_DIFFUSE, afDiffuseGreen);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, afAmbientBlue);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_DIFFUSE, afDiffuseBlue);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, afSpecularWhite);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 25.0f);

            //    vResize(iWidth, iHeight);

            //    vPrintHelp();
        }

        //GLvoid vPrintHelp()
        //{
        //    printf("Marching Cubes Example by Cory Bloyd (dejaspaminacan@my-deja.com)\n\n");

        //    printf("+/-  increase/decrease sample density\n");
        //    printf("PageUp/PageDown  increase/decrease surface value\n");
        //    printf("s  change sample function\n");
        //    printf("c  toggle marching cubes / marching tetrahedrons\n");
        //    printf("w  wireframe on/off\n");
        //    printf("l  toggle lighting / color-by-normal\n");
        //    printf("Home  spin scene on/off\n");
        //    printf("End  source point animation on/off\n");
        //}


        //void vResize(GLsizei iWidth, GLsizei iHeight)
        //{
        //    GLfloat fAspect, fHalfWorldSize = (1.4142135623730950488016887242097 / 2);

        //    glViewport(0, 0, iWidth, iHeight);
        //    glMatrixMode(GL_PROJECTION);
        //    glLoadIdentity();

        //    if (iWidth <= iHeight)
        //    {
        //        fAspect = (GLfloat)iHeight / (GLfloat)iWidth;
        //        glOrtho(-fHalfWorldSize, fHalfWorldSize, -fHalfWorldSize * fAspect,
        //            fHalfWorldSize * fAspect, -10 * fHalfWorldSize, 10 * fHalfWorldSize);
        //    }
        //    else
        //    {
        //        fAspect = (GLfloat)iWidth / (GLfloat)iHeight;
        //        glOrtho(-fHalfWorldSize * fAspect, fHalfWorldSize * fAspect, -fHalfWorldSize,
        //            fHalfWorldSize, -10 * fHalfWorldSize, 10 * fHalfWorldSize);
        //    }

        //    glMatrixMode(GL_MODELVIEW);
        //}

        private void Keydown(object sender, KeyEventArgs e)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            switch (e.KeyValue)
            {
                case 'w':
                    if (ePolygonMode == OpenGL.GL_LINE)
                    {
                        ePolygonMode = OpenGL.GL_FILL;
                    }
                    else
                    {
                        ePolygonMode = OpenGL.GL_LINE;
                    }
                    gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, ePolygonMode);
                    break;
            }
        }

        //void vKeyboard(unsigned char cKey, int iX, int iY)
        //{
        //    switch (cKey)
        //    {
        //        case 'w':
        //            {
        //                if (ePolygonMode == GL_LINE)
        //                {
        //                    ePolygonMode = GL_FILL;
        //                }
        //                else
        //                {
        //                    ePolygonMode = GL_LINE;
        //                }
        //                glPolygonMode(GL_FRONT_AND_BACK, ePolygonMode);
        //            }
        //            break;
        //        case '+':
        //        case '=':
        //            {
        //                ++iDataSetSize;
        //                fStepSize = 1.0 / iDataSetSize;
        //            }
        //            break;
        //        case '-':
        //            {
        //                if (iDataSetSize > 1)
        //                {
        //                    --iDataSetSize;
        //                    fStepSize = 1.0 / iDataSetSize;
        //                }
        //            }
        //            break;
        //        case 'c':
        //            {
        //                if (vMarchCube == vMarchCube1)
        //                {
        //                    vMarchCube = vMarchCube2;//Use Marching Tetrahedrons
        //                }
        //                else
        //                {
        //                    vMarchCube = vMarchCube1;//Use Marching Cubes
        //                }
        //            }
        //            break;
        //        case 's':
        //            {
        //                if (fSample == fSample1)
        //                {
        //                    fSample = fSample2;
        //                }
        //                else if (fSample == fSample2)
        //                {
        //                    fSample = fSample3;
        //                }
        //                else
        //                {
        //                    fSample = fSample1;
        //                }
        //            }
        //            break;
        //        case 'l':
        //            {
        //                if (bLight)
        //                {
        //                    glDisable(GL_LIGHTING);//use vertex colors
        //                }
        //                else
        //                {
        //                    glEnable(GL_LIGHTING);//use lit material color
        //                }

        //                bLight = !bLight;
        //            };
        //    }
        //}


        //void vSpecial(int iKey, int iX, int iY)
        //{
        //    switch (iKey)
        //    {
        //        case GLUT_KEY_PAGE_UP:
        //            {
        //                if (fTargetValue < 1000.0)
        //                {
        //                    fTargetValue *= 1.1;
        //                }
        //            }
        //            break;
        //        case GLUT_KEY_PAGE_DOWN:
        //            {
        //                if (fTargetValue > 1.0)
        //                {
        //                    fTargetValue /= 1.1;
        //                }
        //            }
        //            break;
        //        case GLUT_KEY_HOME:
        //            {
        //                bSpin = !bSpin;
        //            }
        //            break;
        //        case GLUT_KEY_END:
        //            {
        //                bMove = !bMove;
        //            }
        //            break;
        //    }
        //}

        //void vIdle()
        //{
        //    glutPostRedisplay();
        //}

        void vDrawScene()
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.PushMatrix();

            if (bSpin)
            {
                fPitch += 4.0f;
                fYaw += 2.5f;
            }
            if (bMove)
            {
                fTime += 0.025f;
            }

            vSetTime(fTime);

            gl.Translate(0.0, 0.0, -1.0);
            gl.Rotate(-fPitch, 1.0, 0.0, 0.0);
            gl.Rotate(0.0, 0.0, 1.0, 0.0);
            gl.Rotate(fYaw, 0.0, 0.0, 1.0);

            gl.PushAttrib(OpenGL.GL_LIGHTING_BIT);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Color(1.0, 1.0, 1.0);
            //glutWireCube(1.0);
            gl.PopAttrib();


            gl.PushMatrix();
            gl.Translate(-0.5, -0.5, -0.5);
            gl.Begin(OpenGL.GL_TRIANGLES);
            vMarchingCubes();
            gl.End();
            gl.PopMatrix();
            gl.PopMatrix();

        }

        //fGetOffset finds the approximate point of intersection of the surface
        // between two points with the values fValue1 and fValue2
        float fGetOffset(float fValue1, float fValue2, float fValueDesired)
        {
            double fDelta = fValue2 - fValue1;

            if (fDelta == 0.0)
            {
                return 0.5f;
            }
            return (fValueDesired - fValue1) / (float)fDelta;
        }


        //vGetColor generates a color from a given position and normal of a point
        void vGetColor(ref GLvector rfColor, GLvector rfPosition, GLvector rfNormal)
        {
            float fX = rfNormal.fX;
            float fY = rfNormal.fY;
            float fZ = rfNormal.fZ;
            rfColor.fX = (fX > 0.0f ? fX : 0.0f) + (fY < 0.0f ? -0.5f * fY : 0.0f) + (fZ < 0.0f ? -0.5f * fZ : 0.0f);
            rfColor.fY = (fY > 0.0f ? fY : 0.0f) + (fZ < 0.0f ? -0.5f * fZ : 0.0f) + (fX < 0.0f ? -0.5f * fX : 0.0f);
            rfColor.fZ = (fZ > 0.0f ? fZ : 0.0f) + (fX < 0.0f ? -0.5f * fX : 0.0f) + (fY < 0.0f ? -0.5f * fY : 0.0f);
        }

        void vNormalizeVector(ref GLvector rfVectorResult, ref GLvector rfVectorSource)
        {
            float fOldLength;
            float fScale;

            fOldLength = (float)Math.Sqrt((rfVectorSource.fX * rfVectorSource.fX) +
                (rfVectorSource.fY * rfVectorSource.fY) +
                (rfVectorSource.fZ * rfVectorSource.fZ));

            if (fOldLength == 0.0)
            {
                rfVectorResult.fX = rfVectorSource.fX;
                rfVectorResult.fY = rfVectorSource.fY;
                rfVectorResult.fZ = rfVectorSource.fZ;
            }
            else
            {
                fScale = 1.0f / fOldLength;
                rfVectorResult.fX = rfVectorSource.fX * fScale;
                rfVectorResult.fY = rfVectorSource.fY * fScale;
                rfVectorResult.fZ = rfVectorSource.fZ * fScale;
            }
        }


        //Generate a sample data set.  fSample1(), fSample2() and fSample3() define three scalar fields whose
        // values vary by the X,Y and Z coordinates and by the fTime value set by vSetTime()
        void vSetTime(float fNewTime)
        {
            float fOffset;
            int iSourceNum;

            for (iSourceNum = 0; iSourceNum < 3; iSourceNum++)
            {
                sSourcePoint[iSourceNum].fX = 0.5f;
                sSourcePoint[iSourceNum].fY = 0.5f;
                sSourcePoint[iSourceNum].fZ = 0.5f;
            }

            fTime = fNewTime;
            fOffset = 1.0f + (float)Math.Sin(fTime);
            sSourcePoint[0].fX *= fOffset;
            sSourcePoint[1].fY *= fOffset;
            sSourcePoint[2].fZ *= fOffset;
        }

        //fSample1 finds the distance of (fX, fY, fZ) from three moving points
        float fSample1(float fX, float fY, float fZ)
        {
            double fResult = 0.0;
            double fDx, fDy, fDz;
            fDx = fX - sSourcePoint[0].fX;
            fDy = fY - sSourcePoint[0].fY;
            fDz = fZ - sSourcePoint[0].fZ;
            fResult += 0.5 / (fDx * fDx + fDy * fDy + fDz * fDz);

            fDx = fX - sSourcePoint[1].fX;
            fDy = fY - sSourcePoint[1].fY;
            fDz = fZ - sSourcePoint[1].fZ;
            fResult += 1.0 / (fDx * fDx + fDy * fDy + fDz * fDz);

            fDx = fX - sSourcePoint[2].fX;
            fDy = fY - sSourcePoint[2].fY;
            fDz = fZ - sSourcePoint[2].fZ;
            fResult += 1.5 / (fDx * fDx + fDy * fDy + fDz * fDz);

            return (float)fResult;
        }

        //fSample2 finds the distance of (fX, fY, fZ) from three moving lines
        float fSample2(float fX, float fY, float fZ)
        {
            double fResult = 0.0;
            double fDx, fDy, fDz;
            fDx = fX - sSourcePoint[0].fX;
            fDy = fY - sSourcePoint[0].fY;
            fResult += 0.5 / (fDx * fDx + fDy * fDy);

            fDx = fX - sSourcePoint[1].fX;
            fDz = fZ - sSourcePoint[1].fZ;
            fResult += 0.75 / (fDx * fDx + fDz * fDz);

            fDy = fY - sSourcePoint[2].fY;
            fDz = fZ - sSourcePoint[2].fZ;
            fResult += 1.0 / (fDy * fDy + fDz * fDz);

            return (float)fResult;
        }


        //fSample2 defines a height field by plugging the distance from the center into the sin and cos functions
        float fSample3(float fX, float fY, float fZ)
        {
            float fHeight = 20.0f * (fTime + (float)Math.Sqrt((0.5f - fX) * (0.5f - fX) + (0.5f - fY) * (0.5f - fY)));
            fHeight = 1.5f + 0.1f * (float)(Math.Sin(fHeight) + Math.Cos(fHeight));
            float fResult = (fHeight - fZ) * 50.0f;//double

            return fResult;
        }


        //vGetNormal() finds the gradient of the scalar field at a point
        //This gradient can be used as a very accurate vertx normal for lighting calculations
        void vGetNormal(ref GLvector rfNormal, float fX, float fY, float fZ)
        {
            rfNormal.fX = fSample1(fX - 0.01f, fY, fZ) - fSample1(fX + 0.01f, fY, fZ);//fSample
            rfNormal.fY = fSample1(fX, fY - 0.01f, fZ) - fSample1(fX, fY + 0.01f, fZ);
            rfNormal.fZ = fSample1(fX, fY, fZ - 0.01f) - fSample1(fX, fY, fZ + 0.01f);
            vNormalizeVector(ref rfNormal, ref rfNormal);
        }


        //vMarchCube1 performs the Marching Cubes algorithm on a single cube
        void vMarchCube1(float fX, float fY, float fZ, float fScale)
        {

            //int[] aiCubeEdgeFlags = new int[256];
            //int[,] a2iTriangleConnectionTable = new int[256, 16];

            int iCorner, iVertex, iVertexTest, iEdge, iTriangle, iFlagIndex, iEdgeFlags;
            float fOffset = 0.0f;
            GLvector sColor = new GLvector();
            float[] afCubeValue = new float[8];
            GLvector[] asEdgeVertex = new GLvector[12];
            GLvector[] asEdgeNorm = new GLvector[12];

            //Make a local copy of the values at the cube's corners
            for (iVertex = 0; iVertex < 8; iVertex++)
            {
                afCubeValue[iVertex] = fSample1(fX + a2fVertexOffset[iVertex, 0] * fScale,
                    fY + a2fVertexOffset[iVertex, 1] * fScale,
                    fZ + a2fVertexOffset[iVertex, 2] * fScale);
            }

            //Find which vertices are inside of the surface and which are outside
            iFlagIndex = 0;
            for (iVertexTest = 0; iVertexTest < 8; iVertexTest++)
            {
                if (afCubeValue[iVertexTest] <= fTargetValue)
                    iFlagIndex |= 1 << iVertexTest;
            }

            //Find which edges are intersected by the surface
            iEdgeFlags = aiCubeEdgeFlags[iFlagIndex];

            //If the cube is entirely inside or outside of the surface, then there will be no intersections
            if (iEdgeFlags == 0)
            {
                return;
            }

            //Find the point of intersection of the surface with each edge
            //Then find the normal to the surface at those points
            for (iEdge = 0; iEdge < 12; iEdge++)
            {
                //if there is an intersection on this edge
                if (System.Convert.ToBoolean(iEdgeFlags & (1 << iEdge)))
                {
                    fOffset = fGetOffset(afCubeValue[a2iEdgeConnection[iEdge, 0]],
                        afCubeValue[a2iEdgeConnection[iEdge, 1]], fTargetValue);

                    asEdgeVertex[iEdge].fX = fX + (a2fVertexOffset[a2iEdgeConnection[iEdge, 0], 0] + fOffset * a2fEdgeDirection[iEdge, 0]) * fScale;
                    asEdgeVertex[iEdge].fY = fY + (a2fVertexOffset[a2iEdgeConnection[iEdge, 0], 1] + fOffset * a2fEdgeDirection[iEdge, 1]) * fScale;
                    asEdgeVertex[iEdge].fZ = fZ + (a2fVertexOffset[a2iEdgeConnection[iEdge, 0], 2] + fOffset * a2fEdgeDirection[iEdge, 2]) * fScale;

                    vGetNormal(ref asEdgeNorm[iEdge], asEdgeVertex[iEdge].fX, asEdgeVertex[iEdge].fY, asEdgeVertex[iEdge].fZ);
                }
            }


            //Draw the triangles that were found.  There can be up to five per cube
            for (iTriangle = 0; iTriangle < 5; iTriangle++)
            {
                //  Get the OpenGL object.
                OpenGL gl = openGLControl.OpenGL;

                if (a2iTriangleConnectionTable[iFlagIndex, 3 * iTriangle] < 0)
                    break;

                for (iCorner = 0; iCorner < 3; iCorner++)
                {
                    iVertex = a2iTriangleConnectionTable[iFlagIndex, 3 * iTriangle + iCorner];

                    vGetColor(ref sColor, asEdgeVertex[iVertex], asEdgeNorm[iVertex]);
                    gl.Color(sColor.fX, sColor.fY, sColor.fZ);
                    gl.Normal(asEdgeNorm[iVertex].fX, asEdgeNorm[iVertex].fY, asEdgeNorm[iVertex].fZ);
                    gl.Vertex(asEdgeVertex[iVertex].fX, asEdgeVertex[iVertex].fY, asEdgeVertex[iVertex].fZ);
                }
            }
        }

        //vMarchTetrahedron performs the Marching Tetrahedrons algorithm on a single tetrahedron
        //void vMarchTetrahedron(GLvector[] pasTetrahedronPosition, float[] pafTetrahedronValue)
        //{

        //    //int[] aiTetrahedronEdgeFlags = new int[16];
        //    //int[,] a2iTetrahedronTriangles = new int[16, 7];

        //    int iEdge, iVert0, iVert1, iEdgeFlags, iTriangle, iCorner, iVertex, iFlagIndex = 0;
        //    float fOffset, fInvOffset, fValue = 0.0f;
        //    GLvector[] asEdgeVertex = new GLvector[6];
        //    GLvector[] asEdgeNorm = new GLvector[6];
        //    GLvector sColor = new GLvector();

        //    //Find which vertices are inside of the surface and which are outside
        //    for (iVertex = 0; iVertex < 4; iVertex++)
        //    {
        //        if (pafTetrahedronValue[iVertex] <= (int)fTargetValue)
        //            iFlagIndex |= 1 << iVertex;
        //    }

        //    //Find which edges are intersected by the surface
        //    iEdgeFlags = aiTetrahedronEdgeFlags[iFlagIndex];

        //    //If the tetrahedron is entirely inside or outside of the surface, then there will be no intersections
        //    if (iEdgeFlags == 0)
        //    {
        //        return;
        //    }
        //    //Find the point of intersection of the surface with each edge
        //    // Then find the normal to the surface at those points
        //    for (iEdge = 0; iEdge < 6; iEdge++)
        //    {
        //        //if there is an intersection on this edge
        //        if (System.Convert.ToBoolean(iEdgeFlags & (1 << iEdge)))
        //        {
        //            iVert0 = a2iTetrahedronEdgeConnection[iEdge, 0];
        //            iVert1 = a2iTetrahedronEdgeConnection[iEdge, 1];
        //            fOffset = fGetOffset(pafTetrahedronValue[iVert0], pafTetrahedronValue[iVert1], fTargetValue);
        //            fInvOffset = 1.0f - fOffset;

        //            asEdgeVertex[iEdge].fX = fInvOffset * pasTetrahedronPosition[iVert0].fX + fOffset * pasTetrahedronPosition[iVert1].fX;
        //            asEdgeVertex[iEdge].fY = fInvOffset * pasTetrahedronPosition[iVert0].fY + fOffset * pasTetrahedronPosition[iVert1].fY;
        //            asEdgeVertex[iEdge].fZ = fInvOffset * pasTetrahedronPosition[iVert0].fZ + fOffset * pasTetrahedronPosition[iVert1].fZ;

        //            vGetNormal(ref asEdgeNorm[iEdge], asEdgeVertex[iEdge].fX, asEdgeVertex[iEdge].fY, asEdgeVertex[iEdge].fZ);
        //        }
        //    }
        //    //Draw the triangles that were found.  There can be up to 2 per tetrahedron
        //    for (iTriangle = 0; iTriangle < 2; iTriangle++)
        //    {
        //        if (a2iTetrahedronTriangles[iFlagIndex, 3 * iTriangle] < 0)
        //            break;
        //        //  Get the OpenGL object.
        //        OpenGL gl = openGLControl.OpenGL;
        //        for (iCorner = 0; iCorner < 3; iCorner++)
        //        {
        //            iVertex = a2iTetrahedronTriangles[iFlagIndex, 3 * iTriangle + iCorner];

        //            vGetColor(ref sColor, asEdgeVertex[iVertex], asEdgeNorm[iVertex]);
        //            gl.Color(sColor.fX, sColor.fY, sColor.fZ);
        //            gl.Normal(asEdgeNorm[iVertex].fX, asEdgeNorm[iVertex].fY, asEdgeNorm[iVertex].fZ);
        //            gl.Vertex(asEdgeVertex[iVertex].fX, asEdgeVertex[iVertex].fY, asEdgeVertex[iVertex].fZ);
        //        }
        //    }
        //}



        //vMarchCube2 performs the Marching Tetrahedrons algorithm on a single cube by making six calls to vMarchTetrahedron
        //void vMarchCube2(float fX, float fY, float fZ, float fScale)
        //{
        //    int iVertex, iTetrahedron, iVertexInACube;
        //    GLvector[] asCubePosition = new GLvector[8];
        //    float[] afCubeValue = new float[8];
        //    GLvector[] asTetrahedronPosition = new GLvector[4];
        //    float[] afTetrahedronValue = new float[4];

        //    //Make a local copy of the cube's corner positions
        //    for (iVertex = 0; iVertex < 8; iVertex++)
        //    {
        //        asCubePosition[iVertex].fX = fX + a2fVertexOffset[iVertex, 0] * fScale;
        //        asCubePosition[iVertex].fY = fY + a2fVertexOffset[iVertex, 1] * fScale;
        //        asCubePosition[iVertex].fZ = fZ + a2fVertexOffset[iVertex, 2] * fScale;
        //    }

        //    //Make a local copy of the cube's corner values
        //    for (iVertex = 0; iVertex < 8; iVertex++)
        //    {
        //        //afCubeValue[iVertex] = fSample(asCubePosition[iVertex].fX,
        //        afCubeValue[iVertex] = fSample1(asCubePosition[iVertex].fX,
        //            asCubePosition[iVertex].fY,
        //            asCubePosition[iVertex].fZ);
        //    }

        //    for (iTetrahedron = 0; iTetrahedron < 6; iTetrahedron++)
        //    {
        //        for (iVertex = 0; iVertex < 4; iVertex++)
        //        {
        //            iVertexInACube = a2iTetrahedronsInACube[iTetrahedron, iVertex];
        //            asTetrahedronPosition[iVertex].fX = asCubePosition[iVertexInACube].fX;
        //            asTetrahedronPosition[iVertex].fY = asCubePosition[iVertexInACube].fY;
        //            asTetrahedronPosition[iVertex].fZ = asCubePosition[iVertexInACube].fZ;
        //            afTetrahedronValue[iVertex] = afCubeValue[iVertexInACube];
        //        }
        //        vMarchTetrahedron(asTetrahedronPosition, afTetrahedronValue);
        //    }
        //}


        //public void vMarchCube(float fX, float fY, float fZ, float fScale) { } : vMarchCube1;


        //vMarchingCubes iterates over the entire dataset, calling vMarchCube on each cube
        void vMarchingCubes()
        {
            int iX, iY, iZ;
            for (iX = 0; iX < iDataSetSize; iX++)
                for (iY = 0; iY < iDataSetSize; iY++)
                    for (iZ = 0; iZ < iDataSetSize; iZ++)
                    {
                        vMarchCube1(iX * fStepSize, iY * fStepSize, iZ * fStepSize, fStepSize);
                        //vMarchCube(iX * fStepSize, iY * fStepSize, iZ * fStepSize, fStepSize);
                    }
        }


        // For any edge, if one vertex is inside of the surface and the other is outside of the surface
        //  then the edge intersects the surface
        // For each of the 4 vertices of the tetrahedron can be two possible states : either inside or outside of the surface
        // For any tetrahedron the are 2^4=16 possible sets of vertex states
        // This table lists the edges intersected by the surface for all 16 possible vertex states
        // There are 6 edges.  For each entry in the table, if edge #n is intersected, then bit #n is set to 1

        int[] aiTetrahedronEdgeFlags = new int[16]
        {
                    0x00, 0x0d, 0x13, 0x1e, 0x26, 0x2b, 0x35, 0x38, 0x38, 0x35, 0x2b, 0x26, 0x1e, 0x13, 0x0d, 0x00,
        };


        // For each of the possible vertex states listed in aiTetrahedronEdgeFlags there is a specific triangulation
        // of the edge intersection points.  a2iTetrahedronTriangles lists all of them in the form of
        // 0-2 edge triples with the list terminated by the invalid value -1.
        //
        // I generated this table by hand

        int[,] a2iTetrahedronTriangles = new int[16, 7]
        {
            { -1, -1, -1, -1, -1, -1, -1 },
        { 0,  3,  2, -1, -1, -1, -1 },
        { 0,  1,  4, -1, -1, -1, -1 },
        { 1,  4,  2,  2,  4,  3, -1 },

        { 1,  2,  5, -1, -1, -1, -1 },
        { 0,  3,  5,  0,  5,  1, -1 },
        { 0,  2,  5,  0,  5,  4, -1 },
        { 5,  4,  3, -1, -1, -1, -1 },

        { 3,  4,  5, -1, -1, -1, -1 },
        { 4,  5,  0,  5,  2,  0, -1 },
        { 1,  5,  0,  5,  3,  0, -1 },
        { 5,  2,  1, -1, -1, -1, -1 },

        { 3,  4,  2,  2,  4,  1, -1 },
        { 4,  1,  0, -1, -1, -1, -1 },
        { 2,  3,  0, -1, -1, -1, -1 },
        { -1, -1, -1, -1, -1, -1, -1 },
        };

        // For any edge, if one vertex is inside of the surface and the other is outside of the surface
        //  then the edge intersects the surface
        // For each of the 8 vertices of the cube can be two possible states : either inside or outside of the surface
        // For any cube the are 2^8=256 possible sets of vertex states
        // This table lists the edges intersected by the surface for all 256 possible vertex states
        // There are 12 edges.  For each entry in the table, if edge #n is intersected, then bit #n is set to 1

        int[] aiCubeEdgeFlags = new int[256]
        {
            0x000, 0x109, 0x203, 0x30a, 0x406, 0x50f, 0x605, 0x70c, 0x80c, 0x905, 0xa0f, 0xb06, 0xc0a, 0xd03, 0xe09, 0xf00,
            0x190, 0x099, 0x393, 0x29a, 0x596, 0x49f, 0x795, 0x69c, 0x99c, 0x895, 0xb9f, 0xa96, 0xd9a, 0xc93, 0xf99, 0xe90,
            0x230, 0x339, 0x033, 0x13a, 0x636, 0x73f, 0x435, 0x53c, 0xa3c, 0xb35, 0x83f, 0x936, 0xe3a, 0xf33, 0xc39, 0xd30,
            0x3a0, 0x2a9, 0x1a3, 0x0aa, 0x7a6, 0x6af, 0x5a5, 0x4ac, 0xbac, 0xaa5, 0x9af, 0x8a6, 0xfaa, 0xea3, 0xda9, 0xca0,
            0x460, 0x569, 0x663, 0x76a, 0x066, 0x16f, 0x265, 0x36c, 0xc6c, 0xd65, 0xe6f, 0xf66, 0x86a, 0x963, 0xa69, 0xb60,
            0x5f0, 0x4f9, 0x7f3, 0x6fa, 0x1f6, 0x0ff, 0x3f5, 0x2fc, 0xdfc, 0xcf5, 0xfff, 0xef6, 0x9fa, 0x8f3, 0xbf9, 0xaf0,
            0x650, 0x759, 0x453, 0x55a, 0x256, 0x35f, 0x055, 0x15c, 0xe5c, 0xf55, 0xc5f, 0xd56, 0xa5a, 0xb53, 0x859, 0x950,
            0x7c0, 0x6c9, 0x5c3, 0x4ca, 0x3c6, 0x2cf, 0x1c5, 0x0cc, 0xfcc, 0xec5, 0xdcf, 0xcc6, 0xbca, 0xac3, 0x9c9, 0x8c0,
            0x8c0, 0x9c9, 0xac3, 0xbca, 0xcc6, 0xdcf, 0xec5, 0xfcc, 0x0cc, 0x1c5, 0x2cf, 0x3c6, 0x4ca, 0x5c3, 0x6c9, 0x7c0,
            0x950, 0x859, 0xb53, 0xa5a, 0xd56, 0xc5f, 0xf55, 0xe5c, 0x15c, 0x055, 0x35f, 0x256, 0x55a, 0x453, 0x759, 0x650,
            0xaf0, 0xbf9, 0x8f3, 0x9fa, 0xef6, 0xfff, 0xcf5, 0xdfc, 0x2fc, 0x3f5, 0x0ff, 0x1f6, 0x6fa, 0x7f3, 0x4f9, 0x5f0,
            0xb60, 0xa69, 0x963, 0x86a, 0xf66, 0xe6f, 0xd65, 0xc6c, 0x36c, 0x265, 0x16f, 0x066, 0x76a, 0x663, 0x569, 0x460,
            0xca0, 0xda9, 0xea3, 0xfaa, 0x8a6, 0x9af, 0xaa5, 0xbac, 0x4ac, 0x5a5, 0x6af, 0x7a6, 0x0aa, 0x1a3, 0x2a9, 0x3a0,
            0xd30, 0xc39, 0xf33, 0xe3a, 0x936, 0x83f, 0xb35, 0xa3c, 0x53c, 0x435, 0x73f, 0x636, 0x13a, 0x033, 0x339, 0x230,
            0xe90, 0xf99, 0xc93, 0xd9a, 0xa96, 0xb9f, 0x895, 0x99c, 0x69c, 0x795, 0x49f, 0x596, 0x29a, 0x393, 0x099, 0x190,
            0xf00, 0xe09, 0xd03, 0xc0a, 0xb06, 0xa0f, 0x905, 0x80c, 0x70c, 0x605, 0x50f, 0x406, 0x30a, 0x203, 0x109, 0x000
        };

        //  For each of the possible vertex states listed in aiCubeEdgeFlags there is a specific triangulation
        //  of the edge intersection points.  a2iTriangleConnectionTable lists all of them in the form of
        //  0-5 edge triples with the list terminated by the invalid value -1.
        //  For example: a2iTriangleConnectionTable[3] list the 2 triangles formed when corner[0] 
        //  and corner[1] are inside of the surface, but the rest of the cube is not.
        //
        //  I found this table in an example program someone wrote long ago.  It was probably generated by hand

        int[,] a2iTriangleConnectionTable = new int[256, 16] {
        { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1 },
        { 2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1 },
        { 8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1 },
        { 3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1 },
        { 4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1 },
        { 4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1 },
        { 5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1 },
        { 2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1 },
        { 9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1 },
        { 2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1 },
        { 10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1 },
        { 5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1 },
        { 5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1 },
        { 10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1 },
        { 8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1 },
        { 2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1 },
        { 7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1 },
        { 2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1 },
        { 11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1 },
        { 5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1 },
        { 11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1 },
        { 11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1 },
        { 5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1 },
        { 2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1 },
        { 5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1 },
        { 6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1 },
        { 3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1 },
        { 6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1 },
        { 5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1 },
        { 10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1 },
        { 6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1 },
        { 8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1 },
        { 7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1 },
        { 3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1 },
        { 5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1 },
        { 0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1 },
        { 9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1 },
        { 8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1 },
        { 5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1 },
        { 0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1 },
        { 6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1 },
        { 10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1 },
        { 10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1 },
        { 8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1 },
        { 1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1 },
        { 0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1 },
        { 10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1 },
        { 3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1 },
        { 6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1 },
        { 9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1 },
        { 8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1 },
        { 3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1 },
        { 6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1 },
        { 10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1 },
        { 10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1 },
        { 2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1 },
        { 7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1 },
        { 7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1 },
        { 2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1 },
        { 1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1 },
        { 11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1 },
        { 8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1 },
        { 0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1 },
        { 7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1 },
        { 10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1 },
        { 2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1 },
        { 6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1 },
        { 7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1 },
        { 2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1 },
        { 10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1 },
        { 10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1 },
        { 0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1 },
        { 7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1 },
        { 6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1 },
        { 8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1 },
        { 6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1 },
        { 4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1 },
        { 10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1 },
        { 8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1 },
        { 1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1 },
        { 8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1 },
        { 10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1 },
        { 10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1 },
        { 5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1 },
        { 11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1 },
        { 9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1 },
        { 6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1 },
        { 7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1 },
        { 3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1 },
        { 7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1 },
        { 3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1 },
        { 6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1 },
        { 9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1 },
        { 1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1 },
        { 4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1 },
        { 7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1 },
        { 6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1 },
        { 0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1 },
        { 6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1 },
        { 0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1 },
        { 11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1 },
        { 6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1 },
        { 5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1 },
        { 9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1 },
        { 1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1 },
        { 10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1 },
        { 0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1 },
        { 5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1 },
        { 10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1 },
        { 11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1 },
        { 9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1 },
        { 7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1 },
        { 2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1 },
        { 8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1 },
        { 9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1 },
        { 9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1 },
        { 1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1 },
        { 5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1 },
        { 0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1 },
        { 10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1 },
        { 2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1 },
        { 0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1 },
        { 0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1 },
        { 9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1 },
        { 5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1 },
        { 5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1 },
        { 8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1 },
        { 9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1 },
        { 1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1 },
        { 3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1 },
        { 4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1 },
        { 9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1 },
        { 11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1 },
        { 11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1 },
        { 2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1 },
        { 9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1 },
        { 3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1 },
        { 1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1 },
        { 4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1 },
        { 0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1 },
        { 9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1 },
        { 1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { 0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 },
        { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }
        };

    }
}