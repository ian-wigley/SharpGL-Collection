using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneGraph.Assets;

namespace Tentacles
{
    public partial class SharpGLForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharpGLForm"/> class.
        /// </summary>
        /// 

        // Blob Constants
        const int xRows = 48;
        const int yRows = 16;
        const int numVertices = 2 * yRows * xRows + xRows + 2 + 1;
        const int numTriangles = 2 * xRows + yRows * xRows * 4;

        const float TUNNEL_SPEED = 1 / 50;
        const float PI_8 = 0.3926990816987f;

        public struct TNormal
        {
            public float X;
            public float Y;
            public float Z;
        }
        public struct TVertex
        {
            public float X;
            public float Y;
            public float Z;
        }
        public struct TTexCoord
        {
            public float U;
            public float V;
        }
        public struct TTriangle
        {
            public int[] v;
            public TTexCoord[] T;
            public TNormal n;
        }
        public struct TBlob
        {
            public TVertex[] vertex;
            public TTriangle[] Triangle;

        }

        TBlob obj;
        TBlob blob;
        TBlob sphere;

        // Blob lookup table
        TVertex[] LU = new TVertex[numVertices];
        TVertex[,] Tunnels = new TVertex[65, 33];

        uint CubeDL;
        uint based = 0;
        float Angle = 0;
        float LastTime = 0;
        DateTime startTime = new DateTime();
        DateTime endTime;
        float elapsedTime;

        //--- Textures  ---//
        Texture colorsTex = new Texture();
        Texture blueTex = new Texture();

        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);

        /* ----------------------------------- */

        IntPtr ptr1;

        public SharpGLForm()
        {
            startTime = DateTime.Now;
            InitializeComponent();

            string alias = "Music.mid";
            mciSendString("open " + @"Music.mid" + " alias " + alias, new StringBuilder(), 0, new IntPtr());
            mciSendString("play " + alias, new StringBuilder(), 0, new IntPtr());

            string test = "help";
            ptr1 = Marshal.StringToHGlobalAuto(test);

        }

        private void glWrite(int x, int y, string text)
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            char[] temp = new char[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                temp[i] = text[i];
            }

            Marshal.Copy(temp, 0, ptr1, temp.Length);

            gl.PushAttrib(OpenGL.GL_DEPTH_TEST);                        // Save the current Depth test settings (Used for blending )
            gl.Disable(OpenGL.GL_DEPTH_TEST);                           // Turn off depth testing (otherwise we get no FPS)
            gl.Disable(OpenGL.GL_TEXTURE_2D);                           // Turn off textures, don't want our text textured
            gl.MatrixMode(OpenGL.GL_PROJECTION);                        // Switch to the projection matrix
            gl.PushMatrix();                                            // Save current projection matrix
            gl.LoadIdentity();
            gl.Ortho(0, 640, 0, 429, -1, 1);                            // Change the projection matrix using an orthgraphic projection
            gl.MatrixMode(OpenGL.GL_MODELVIEW);                         // Return to the modelview matrix
            gl.PushMatrix();                                            // Save the current modelview matrix
            gl.LoadIdentity();
            gl.Color(1.0, 1.0, 1.0);                                    // Text color
            gl.RasterPos(x, y);                                         // Position the Text
            gl.PushAttrib(OpenGL.GL_LIST_BIT);                          // Save's the current base list
            gl.ListBase(based+2);                                   // Set the base list to our character list
            gl.CallLists(text.Length, OpenGL.GL_UNSIGNED_BYTE, ptr1);   // Display the text
            gl.PopAttrib();                                             // Restore the old base list
            gl.MatrixMode(OpenGL.GL_PROJECTION);                        //Switch to projection matrix
            gl.PopMatrix();                                             // Restore the old projection matrix
            gl.MatrixMode(OpenGL.GL_MODELVIEW);                         // Return to modelview matrix
            gl.PopMatrix();                                             // Restore old modelview matrix
            gl.Enable(OpenGL.GL_TEXTURE_2D);                            // Turn on textures, don't want our text textured
            gl.PopAttrib();                                             // Restore depth testing
        }

        //{------------------------------------------------------------------}
        //{  Function to normalize a vector                                  }
        //{------------------------------------------------------------------}
        internal void Normalize(ref TNormal vector)
        {
            // Calculates The Length Of The Vector
            float length = (float)Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y) + (vector.Z * vector.Z));
            if (length == 0)
            {
                length = 1;
            }

            vector.X = vector.X / length;
            vector.Y = vector.Y / length;
            vector.Z = vector.Z / length;
        }

        //{------------------------------------------------------------------}
        //{  Function to calculate the normal given 2 vectors (3 vertices)   }
        //{------------------------------------------------------------------}
        internal TNormal calcNormal(TVertex[] v)
        {
            TVertex a = new TVertex();
            TVertex b = new TVertex();
            TNormal result = new TNormal();

            // Calculate The Vector From Point 1 To Point 0
            a.X = v[0].X - v[1].X;
            a.Y = v[0].Y - v[1].Y;
            a.Z = v[0].Z - v[1].Z;
            // Calculate The Vector From Point 2 To Point 1
            b.X = v[1].X - v[2].X;
            b.Y = v[1].Y - v[2].Y;
            b.Z = v[1].Z - v[2].Z;
            // Compute The Cross Product To Give Us A Surface Normal
            result.X = a.Y * b.Z - a.Z * b.Y;        // Cross Product For Y - Z
            result.Y = a.Z * b.X - a.X * b.Z;        // Cross Product For Z - X
            result.Z = a.X * b.Y - a.Y * b.X;        // Cross Product For X - Y

            Normalize(ref result);                   // Normalize The Vectors
            return result;
        }

        //{------------------------------------------------------------------}
        //{  Function to create a sphere                                     }
        //{------------------------------------------------------------------}
        private void CreateSphere(ref TBlob obj)
        {
            const int r = 10;
            int yRing = yRows * (4 + 4);
            float uTex = 3.0f / (xRows);
            float vTex = 3.0f / ((3 + 2) * yRows);
            float yLevel = 0;
            float radius = 0;
            int offset = 0;

            // Top center
            obj.vertex[0].X = 0;
            obj.vertex[0].Z = 0;
            obj.vertex[0].Y = -r;
            offset = 1;

            // Top half and center
            for (int y = 0; y < yRows + 1; y++)
            {
                yLevel = (float)(-r * Math.Cos(2 * Math.PI * (y + 1) / yRing));
                radius = (float)(r * Math.Sin(2 * Math.PI * (y + 1) / yRing));

                for (int x = 0; x < xRows; x++)
                {
                    obj.vertex[offset].X = (float)(radius * Math.Sin(2 * Math.PI * x / xRows));
                    obj.vertex[offset].Z = (float)(radius * Math.Cos(2 * Math.PI * x / xRows));
                    obj.vertex[offset].Y = yLevel;
                    offset++;
                }
            }

            // Bottom half
            for (int y = 0; y < yRows; y++)
            {
                yLevel = (float)(r * Math.Sin(2 * Math.PI * (y + 1) / yRing));
                radius = (float)(r * Math.Cos(2 * Math.PI * (y + 1) / yRing));

                for (int x = 0; x < xRows; x++)
                {
                    obj.vertex[offset].X = (float)(radius * Math.Sin(2 * Math.PI * x / xRows));
                    obj.vertex[offset].Z = (float)(radius * Math.Cos(2 * Math.PI * x / xRows));
                    obj.vertex[offset].Y = yLevel;
                    offset++;
                }
            }

            // Bottom center
            obj.vertex[offset].X = 0;
            obj.vertex[offset].Z = 0;
            obj.vertex[offset].Y = r;

            for (int i = 0; i < xRows; i++)
            {
                obj.Triangle[i].v[0] = 0;
                obj.Triangle[i].v[1] = (i + 1) % xRows + 1;
                obj.Triangle[i].v[2] = (i + 1);

                obj.Triangle[i].T[0].U = 0.5f;
                obj.Triangle[i].T[0].V = 0.0f;
                obj.Triangle[i].T[1].U = (i + 1) * uTex;
                obj.Triangle[i].T[1].V = vTex;
                obj.Triangle[i].T[2].U = i * uTex;
                obj.Triangle[i].T[2].V = vTex;
            }

            for (int j = 0; j < yRows * 2; j++)
            {
                for (int i = 0; i < xRows; i++)
                {
                    offset = xRows + (i + j * xRows) * 2;
                    obj.Triangle[offset].v[0] = j * xRows + 1 + i;
                    obj.Triangle[offset].v[1] = j * xRows + 1 + (i + 1) % xRows;
                    obj.Triangle[offset].v[2] = j * xRows + 1 + i + xRows;

                    obj.Triangle[offset].T[0].U = i * uTex;
                    obj.Triangle[offset].T[0].V = (1 + j) * vTex;
                    obj.Triangle[offset].T[1].U = (1 + i) * uTex;
                    obj.Triangle[offset].T[1].V = (1 + j) * vTex;
                    obj.Triangle[offset].T[2].U = i * uTex;
                    obj.Triangle[offset].T[2].V = (2 + j) * vTex;

                    offset = xRows + (i + j * xRows) * 2 + 1;
                    obj.Triangle[offset].v[0] = j * xRows + 1 + (i + 1) % xRows;
                    obj.Triangle[offset].v[1] = j * xRows + 1 + (i + 1) % xRows + xRows;
                    obj.Triangle[offset].v[2] = j * xRows + 1 + i + xRows;

                    obj.Triangle[offset].T[0].U = (i + 1) * uTex;
                    obj.Triangle[offset].T[0].V = (1 + j) * vTex;
                    obj.Triangle[offset].T[1].U = (i + 1) * uTex;
                    obj.Triangle[offset].T[1].V = (2 + j) * vTex;
                    obj.Triangle[offset].T[2].U = i * uTex;
                    obj.Triangle[offset].T[2].V = (2 + j) * vTex;
                }

                for (int i = 0; i < xRows; i++)
                {
                    offset = xRows + xRows * 2 * yRows * 2 + i;
                    obj.Triangle[offset].v[0] = 2 * yRows * xRows + 1 + i;
                    obj.Triangle[offset].v[1] = 2 * yRows * xRows + 1 + (i + 1) % xRows;
                    obj.Triangle[offset].v[2] = 2 * yRows * xRows + 1 + xRows;

                    obj.Triangle[offset].T[0].U = i * uTex;
                    obj.Triangle[offset].T[0].V = (yRows * 2 + 1) * vTex;
                    obj.Triangle[offset].T[1].U = (i + 1) * uTex;
                    obj.Triangle[offset].T[1].V = (yRows * 2 + 1) * vTex;
                    obj.Triangle[offset].T[2].U = 0.5f;
                    obj.Triangle[offset].T[2].V = 1.0f;
                }
            }

            // Calculate Normals
            for (int i = 0; i < numTriangles - 1; i++)
            {
                TVertex[] v = new TVertex[3];
                v[0] = obj.vertex[obj.Triangle[i].v[0]];
                v[1] = obj.vertex[obj.Triangle[i].v[1]];
                v[2] = obj.vertex[obj.Triangle[i].v[2]];
                obj.Triangle[i].n = calcNormal(v);
            }
        }

        //{------------------------------------------------------------------}
        //{  Function to draw the blob (warped sphere)                       }
        //{------------------------------------------------------------------}
        private void RenderBlob(OpenGL gl)
        {
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, colorsTex.TextureName);
            gl.Begin(OpenGL.GL_TRIANGLES);
            {
                for (int i = 0; i < numTriangles - 1; i++)
                {
                    gl.TexCoord(obj.Triangle[i].T[0].U, obj.Triangle[1].T[0].V);
                    gl.Vertex(obj.vertex[obj.Triangle[i].v[0]].X, obj.vertex[obj.Triangle[i].v[0]].Y, obj.vertex[obj.Triangle[i].v[0]].Z);

                    gl.TexCoord(obj.Triangle[i].T[1].U, obj.Triangle[1].T[1].V);
                    gl.Vertex(obj.vertex[obj.Triangle[i].v[1]].X, obj.vertex[obj.Triangle[i].v[1]].Y, obj.vertex[obj.Triangle[i].v[1]].Z);

                    gl.TexCoord(obj.Triangle[i].T[2].U, obj.Triangle[1].T[2].V);
                    gl.Vertex(obj.vertex[obj.Triangle[i].v[2]].X, obj.vertex[obj.Triangle[i].v[2]].Y, obj.vertex[obj.Triangle[i].v[2]].Z);
                }
            }
            gl.End();
        }

        //{------------------------------------------------------------------}
        //{  Draw the bouncing blob                                          }
        //{------------------------------------------------------------------}
        private void DrawBlob(OpenGL gl, float elapsedTime)
        {
            float C = 0;
            float Y = 0;
            float X = 0;
            float DemoTime = elapsedTime - 4000;

            // --- Drawing the Blob and Rectangle --- //
            gl.Disable(OpenGL.GL_BLEND);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Color(1.0, 1.0, 1.0);

            // bring in BLob
            if (DemoTime < 16900)
            {
                X = DemoTime / 250.0f;
                if (X > 2)
                {
                    X = 2;
                }
                gl.Scale(X, X, 1);
            }
            else    // fly away blob
            {
                X = (18000.0f - DemoTime) / 500.0f;
                gl.Scale(X, X, 1);
            }

            gl.Translate(0, 0, -2);

            // Rectangle (blob frame)
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.LineWidth(3);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            gl.Vertex(-0.32f, -0.28f, 0.0f);
            gl.Vertex(0.32f, -0.28f, 0.0f);
            gl.Vertex(0.32f, 0.28f, 0.0f);
            gl.Vertex(-0.32f, 0.28f, 0.0f);
            gl.End();
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            //Blob
            if ((DemoTime > 2800 && DemoTime < 10400) || (DemoTime > 10800 && DemoTime < 17000))
            {
                gl.Translate(0.0f, 0.0f, Math.Abs(0.7f * Math.Sin(DemoTime / 400.0f * Math.PI)));
            }

            gl.Rotate(DemoTime / 20, 0, 1, 0);
            gl.Rotate(DemoTime / 10, 0, 0, 1);
            C = DemoTime / 1000.0f;

            for (int i = 0; i < numVertices - 1; i++)
            {
                Y = (float)(1.0f + 0.1f * ((1.0f - Math.Cos(LU[i].X + C * 5.0f)) +
                              (1.0f - Math.Cos(LU[i].Y + C * 7.0f)) +
                              (1.0f - Math.Cos(LU[i].Z + C * 8.0f))));
                obj.vertex[i].X = sphere.vertex[i].X * Y;
                obj.vertex[i].Y = sphere.vertex[i].Y * Y;
                obj.vertex[i].Z = sphere.vertex[i].Z * Y;
            }

            gl.PushMatrix();
            gl.Scale(0.015f, 0.015f, 0.015f);
            gl.Rotate(-DemoTime / 50, 0, 0, 1);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, colorsTex.TextureName);
            RenderBlob(gl);
            gl.PopMatrix();
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
            colorsTex.Bind(gl);

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

        //{------------------------------------------------------------------}
        //{  Draw the tentacle scene                                         }
        //{------------------------------------------------------------------}
        private void drawTentacles(OpenGL gl, float elapsedMillisecs)
        {
            float DemoTime = elapsedMillisecs - 22000;
            float X = 0;

            gl.Color(1.0f, 1.0f, 1.0f);

            gl.Translate(0.0f, -0.05f, -3.0f);

            X = DemoTime / 10000.0f;
            if (X > 0.1)
            {
                X = 0.1f;
                gl.Scale(X, X, X);
            }
            // Rectangle (blob frame)
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.LineWidth(3);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            gl.Vertex(-9, -9, 0.0);
            gl.Vertex(9, -9, 0.0);
            gl.Vertex(9, 9, 0.0);
            gl.Vertex(-9, 9, 0.0);
            gl.End();
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            // Draw the blue tentacles
            gl.PushMatrix();

            // wobly movement
            if (elapsedMillisecs > 29200)
            {
                gl.Translate(4 * Math.Cos(elapsedMillisecs / 200.0f), 3 * Math.Cos(elapsedMillisecs / 230.0f), 0.0f);
            }
            gl.Rotate(DemoTime / 10.0f, 1.0f, 0.0f, 0.0f);
            gl.Rotate(DemoTime / 15.0f, 0.0f, 1.0f, 0.0f);

            // Bind the Texture to the object
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, colorsTex.TextureName);
            DrawObject(DemoTime, gl);
            gl.PopMatrix();
        }

        private void ShowScroller(OpenGL gl)
        {
            float DemoTime = elapsedTime - 44000;
            float c = 0;
            int t = 0;

            // --- Drawing the Blob and Rectangle --- //
            gl.PushMatrix();
            gl.Disable(OpenGL.GL_BLEND);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Color(1.0, 1.0, 1.0);

            gl.Scale(2, 2, 1);
            gl.Translate(0, 0, -2);

            // Rectangle (blob frame)
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.LineWidth(3);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            gl.Vertex(-0.32, -0.28, 0.0);
            gl.Vertex(0.32, -0.28, 0.0);
            gl.Vertex(0.32, 0.28, 0.0);
            gl.Vertex(-0.32, 0.28, 0.0);
            gl.End();
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            // Blob
            gl.Rotate(DemoTime / 20, 0, 1, 0);
            gl.Rotate(DemoTime / 10, 0, 0, 1);
            c = DemoTime / 1000;

            for (int i = 0; i < numVertices; i++)
            {
                float Y = (float)(1.0f + 0.1f * ((1.0f - Math.Cos(LU[i].X + c * 5.0f)) +
                              (1.0f - Math.Cos(LU[i].Y + c * 7.0f)) +
                              (1.0f - Math.Cos(LU[i].Z + c * 8.0f))));
                obj.vertex[i].X = sphere.vertex[i].X * Y;
                obj.vertex[i].Y = sphere.vertex[i].Y * Y;
                obj.vertex[i].Z = sphere.vertex[i].Z * Y;
            }


            gl.Scale(0.015, 0.015, 0.015);
            gl.Rotate(-DemoTime / 50, 0, 0, 1);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, colorsTex.TextureName);

            RenderBlob(gl);

            gl.PopMatrix();

            t = (int)(Math.Round(DemoTime / 15.0f));
            glWrite(350, 10 + t, "A 64K Demo by Jan Horn");
            glWrite(300, -60 + t, "Well that is all there is to this 64K demo");
            glWrite(300, -100 + t, "'I started it on the morning Optimize 2001");
            glWrite(300, -140 + t, "'and just didnt have enough time to finish");
            glWrite(300, -180 + t, "'it. I also wasted to much time trying to get");
            glWrite(300, -220 + t, "'a midi file in the app resource and playing");
            glWrite(300, -260 + t, "'it from there.");
            glWrite(300, -320 + t, "Total coding time = 2 hours.");
            glWrite(300, -380 + t, "The demo has to submited now and I still");
            glWrite(300, -420 + t, "have a few KB left. Guess I wont finish it now");
            glWrite(300, -480 + t, "The demo and source will be available on ");
            glWrite(300, -520 + t, "my site as usual.");
            glWrite(300, -580 + t, "           www.sulaco.co.za");
        }

        private void DrawDemo(OpenGL gl, float elapsedMillisecs)
        {
            float A1, A2, A3, A4;
            float X, Y;
            float C, J1, J2;

            float DemoTime = elapsedMillisecs;

            // --- Drawing the tunnel --- //
            gl.Disable(OpenGL.GL_BLEND);
            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -2.0f);

            Angle = DemoTime / 14.0f;

            //--- Outside tunnel ---//
            // setup tunnel coordinates
            A1 = (float)(Math.Sin(Angle / 27.0f));
            A2 = (float)(Math.Cos(Angle / 25.0f));
            A3 = (float)(Math.Cos(Angle / 13.0f));
            A4 = (float)(Math.Sin(Angle / 17.0f));
            for (int i = 0; i < 16 + 1; i++)
            {
                X = (float)(Math.Cos(PI_8 * i));
                Y = (float)(Math.Sin(PI_8 * i));
                for (int j = 0; j < 32 + 1; j++)
                {
                    Tunnels[i, j].X = (float)((3.0f - j / 12.0f) * X + 2.0f * Math.Sin((Angle + 2 * j) / 27) + Math.Cos((Angle + 2 * j) / 13) - 2 * A1 - A3);
                    Tunnels[i, j].Y = (float)((3.0f - j / 12.0f) * Y + 2.5f * Math.Cos((Angle + 2 * j) / 25) + Math.Sin((Angle + 2 * j) / 17) - 2 * A2 - A4);
                    Tunnels[i, j].Z = -j;
                }
            }

            // draw tunnel and fade out last few
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, blueTex.TextureName);
            for (int J = 0; J < 32; J++)
            {
                // precalculate texture v coords for speed
                J1 = J / 32.0f + Angle * TUNNEL_SPEED;
                J2 = (J + 1) / 32.0f + Angle * TUNNEL_SPEED;

                // near the end of the tunnel, fade the effect away
                if (J > 24)
                {
                    C = 1.0f - (J - 24.0f) / 10.0f;
                }
                else
                {
                    C = 1.0f;
                }

                // fade in tunnel
                if (DemoTime < 500)
                {
                    C = C * DemoTime / 500;
                }

                gl.Color(C, C, C);

                gl.Begin(OpenGL.GL_QUADS);
                for (int I = 0; I < 15 + 1; I++)
                {
                    gl.TexCoord((I - 1.0f) / 8.0f, J1); gl.Vertex(Tunnels[I, J].X, Tunnels[I, J].Y, Tunnels[I, J].Z);
                    gl.TexCoord((I) / 8.0f, J1); gl.Vertex(Tunnels[I + 1, J].X, Tunnels[I + 1, J].Y, Tunnels[I + 1, J].Z);
                    gl.TexCoord((I) / 8.0f, J2); gl.Vertex(Tunnels[I + 1, J + 1].X, Tunnels[I + 1, J + 1].Y, Tunnels[I + 1, J + 1].Z);
                    gl.TexCoord((I - 1.0f) / 8.0f, J2); gl.Vertex(Tunnels[I, J + 1].X, Tunnels[I, J + 1].Y, Tunnels[I, J + 1].Z);
                }
                gl.End();
                gl.Color(1.0f, 1.0f, 1.0f);
                gl.PopMatrix();

            }

            if (DemoTime > 4000 && DemoTime < 22000)
            {
                DrawBlob(gl, DemoTime);
            }
            if (DemoTime > 22000 && DemoTime < 42000)
            {
                drawTentacles(gl, DemoTime);
            }
            if (DemoTime > 44000)
            {
                ShowScroller(gl);
            }
            if (DemoTime > 68000)
            {
                Application.Exit();
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

            //  Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            //  Load the identity matrix.
            gl.LoadIdentity();

            LastTime = elapsedTime;
            endTime = DateTime.Now;
            elapsedTime = (float)(((TimeSpan)(endTime - startTime)).TotalMilliseconds) / 2;

            DrawDemo(gl, elapsedTime);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            // Initialise the Demo
            InitData();

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            //  A bit of extra initialisation here, we have to enable textures.
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            //  Create our texture object from a file. This creates the texture for OpenGL.
            blueTex.Create(gl, "Blue.png");
            colorsTex.Create(gl, "Colours.png");

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

        private void InitData()
        {
            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;

            blob = new TBlob();
            blob.Triangle = new TTriangle[numTriangles];
            blob.vertex = new TVertex[numVertices];

            obj = new TBlob();
            obj.Triangle = new TTriangle[numTriangles];
            obj.vertex = new TVertex[numVertices];

            sphere = new TBlob();
            sphere.vertex = new TVertex[numVertices];

            for (int i = 0; i < numTriangles; i++)
            {
                blob.Triangle[i].T = new TTexCoord[3];
                blob.Triangle[i].v = new int[3];

                obj.Triangle[i].T = new TTexCoord[3];
                obj.Triangle[i].v = new int[3];
            }

            CreateSphere(ref blob);

            // Copy the verts from blob into the sphere & object
            int count = 0;
            foreach(TVertex vertex in blob.vertex)
            {
                sphere.vertex[count] = vertex;
                obj.vertex[count] = vertex;
                count += 1;
            }

            count = 0;
            foreach (TTriangle triangle in blob.Triangle)
            {
                obj.Triangle[count++] = triangle;
            }

            for (int i = 0; i < numVertices; i++)
            {
                LU[i].X = ArcTan(sphere.vertex[i].X, sphere.vertex[i].Y) * 5;
                LU[i].Y = ArcTan(sphere.vertex[i].X, sphere.vertex[i].Z) * 6;
                LU[i].Z = ArcTan(sphere.vertex[i].Y, sphere.vertex[i].Z) * 8;
            }

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

        // Method to generate an ArcTan
        internal float ArcTan(float x, float y)
        {
            return (float)Math.Atan2(y, x);
        }
    }
}