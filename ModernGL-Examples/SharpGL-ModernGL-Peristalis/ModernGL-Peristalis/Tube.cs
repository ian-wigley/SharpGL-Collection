using System;
using GlmNet;
using SharpGL;

namespace NewGL
{
    public class Tube : BaseModel
    {
        private const int MAX_SEGMENTS = 64;
        private const int MAX_DIVS = 16;
        private float textureWidth = 2 * (128.0f / 91);
        private float[] twist = new float[MAX_SEGMENTS];
        private float[] radius = new float[MAX_SEGMENTS];
        private float[] xpos = new float[MAX_SEGMENTS];

        public Tube()  { }

        public void Initialise(OpenGL gl)
        {
            UpdateTwsitValues();
            vertices = UpdateVerts();
            texCoords = UpdateUVs();
            base.Initialise(gl, "tunnelTexture.bmp");
        }

        public void Draw(OpenGL gl, mat4 projectionMatrix, mat4 viewMatrix)
        {
            theta += 0.07f;

            mat4 rotation = glm.rotate(mat4.identity(), theta, new vec3(1, 1, 0));
            mat4 translation = glm.translate(mat4.identity(), new vec3(0, -6.5f, -6));
            mat4 scale = glm.scale(mat4.identity(), new vec3(3.5f, 3.5f, 3.5f));

            modelMatrix = scale * translation;

            gl.UseProgram(shaderProgram);

            // Bind the vertex Buffer Array
            gl.BindVertexArray(VBA[0]);

            UpdateTwsitValues();
            var temp = UpdateVerts();
            var textureCoords = UpdateUVs();

            // Bind the local matrices to the Vertex shader matrix
            var p1 = gl.GetUniformLocation(shaderProgram, "projectionMatrix");
            gl.UniformMatrix4(p1, 1, false, projectionMatrix.to_array());
            var v1 = gl.GetUniformLocation(shaderProgram, "viewMatrix");
            gl.UniformMatrix4(v1, 1, false, viewMatrix.to_array());
            var u1 = gl.GetUniformLocation(shaderProgram, "ModelWorld4x4");
            gl.UniformMatrix4(u1, 1, false, modelMatrix.to_array());

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureValue);
            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, vertices.Length);

            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, VBO[0]);

            IntPtr VideoMemoryIntPtr = gl.MapBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGL.GL_WRITE_ONLY);
            if (VideoMemoryIntPtr != null)
            {
                unsafe
                {
                    fixed (float* src = &temp[0])
                    {
                        float* VideoMemory = (float*)VideoMemoryIntPtr.ToPointer();
                        for (int i = 0; i < temp.Length; i++)
                        {
                            VideoMemory[i] = src[i];
                        }
                    }
                }
            }
            bool val = gl.UnmapBuffer(OpenGL.GL_ARRAY_BUFFER);

            // Bind the texture buffer
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, VBO[1]);

            VideoMemoryIntPtr = gl.MapBuffer(OpenGL.GL_ARRAY_BUFFER, OpenGL.GL_WRITE_ONLY);
            if (VideoMemoryIntPtr != null)
            {
                unsafe
                {
                    fixed (float* col = &textureCoords[0])
                    {
                        float* VideoMemory = (float*)VideoMemoryIntPtr.ToPointer();
                        for (int i = 0; i < textureCoords.Length; i++)
                        {
                            VideoMemory[i] = col[i];
                        }
                    }
                }
            }
            val = gl.UnmapBuffer(OpenGL.GL_ARRAY_BUFFER);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.BindVertexArray(0);

            gl.UseProgram(0);
        }

        private void UpdateTwsitValues()
        {
            for (int i = 0; i < MAX_SEGMENTS - 1; i++)
            {
                twist[i] = (float)(0.35f * Math.Sin(theta * 0.78f + i / 12.0f) + 0.35f * Math.Sin(theta * -1.23f + i / 18.0f));
                radius[i] = (float)(0.30f * Math.Sin(theta + i / 8.0f) + 0.80f + 0.15f * Math.Sin(theta * -0.8f + i / 3.0f));
                xpos[i] = (float)(0.25f * Math.Sin(theta * 1.23f + i / 5.0f) + 0.30f * Math.Sin(theta * 0.9f + i / 6.0f));
            }
        }

        private float[] UpdateVerts()
        {
            float[] localvertices = new float[4096 * 6];
            int count = 0;

            // Calculate the updated cylinder vertices
            for (int i = 0; i < MAX_SEGMENTS - 1; i++)
            {
                for (int j = 0; j < MAX_DIVS; j++)
                {
                    //i , j
                    localvertices[count++] = (float)(xpos[i] + 0.7f * radius[i] * Math.Cos(j * Math.PI * 2.0f / MAX_DIVS));
                    localvertices[count++] = (float)(i * 5.5f / MAX_SEGMENTS);
                    localvertices[count++] = (float)(0.7f * radius[i] * Math.Sin(j * Math.PI * 2.0f / MAX_DIVS));

                    //  i + 1, j
                    localvertices[count++] = (float)(xpos[i + 1] + 0.7f * radius[i + 1] * Math.Cos(j * Math.PI * 2.0f / MAX_DIVS));
                    localvertices[count++] = (float)((i + 1) * 5.5f / MAX_SEGMENTS);
                    localvertices[count++] = (float)(0.7f * radius[i + 1] * Math.Sin(j * Math.PI * 2.0f / MAX_DIVS));

                    //  i + 1, j + 1
                    localvertices[count++] = (float)(xpos[i + 1] + 0.7f * radius[i + 1] * Math.Cos((j + 1) * Math.PI * 2.0f / MAX_DIVS));
                    localvertices[count++] = (float)((i + 1) * 5.5 / MAX_SEGMENTS);
                    localvertices[count++] = (float)(0.7f * radius[i + 1] * Math.Sin((j + 1) * Math.PI * 2.0f / MAX_DIVS));

                    //i , j
                    localvertices[count++] = (float)(xpos[i] + 0.7f * radius[i] * Math.Cos(j * Math.PI * 2.0f / MAX_DIVS));
                    localvertices[count++] = (float)(i * 5.5f / MAX_SEGMENTS);
                    localvertices[count++] = (float)(0.7f * radius[i] * Math.Sin(j * Math.PI * 2.0f / MAX_DIVS));

                    //  i , j + 1
                    localvertices[count++] = (float)(xpos[i] + 0.7f * radius[i] * Math.Cos((j + 1) * Math.PI * 2.0 / MAX_DIVS));
                    localvertices[count++] = (float)(i * 5.5f / MAX_SEGMENTS);
                    localvertices[count++] = (float)(0.7f * radius[i] * Math.Sin((j + 1) * Math.PI * 2.0f / MAX_DIVS));

                    //  i + 1, j + 1
                    localvertices[count++] = (float)(xpos[i + 1] + 0.7f * radius[i + 1] * Math.Cos((j + 1) * Math.PI * 2.0f / MAX_DIVS));
                    localvertices[count++] = (float)((i + 1) * 5.5 / MAX_SEGMENTS);
                    localvertices[count++] = (float)(0.7f * radius[i + 1] * Math.Sin((j + 1) * Math.PI * 2.0f / MAX_DIVS));
                }
            }
            return localvertices;
        }

        private float[] UpdateUVs()
        {
            float[] localTextureCoords = new float[4096 * 4];
            int count = 0;

            // Calculate the updated cylinder texture positions
            for (int i = 0; i < MAX_SEGMENTS - 1; i++)
            {
                for (int j = 0; j < MAX_DIVS; j++)
                {
                    localTextureCoords[count++] = ((twist[i] + j * 4.0f / MAX_DIVS) / textureWidth);
                    localTextureCoords[count++] = (i * 5.5f / MAX_SEGMENTS);

                    localTextureCoords[count++] = ((twist[i + 1] + j * 4.0f / MAX_DIVS) / textureWidth);
                    localTextureCoords[count++] = ((i + 1) * 5.5f / MAX_SEGMENTS);

                    localTextureCoords[count++] = ((twist[i + 1] + (j + 1) * 4.0f / MAX_DIVS) / textureWidth);
                    localTextureCoords[count++] = ((i + 1) * 5.5f / MAX_SEGMENTS);

                    localTextureCoords[count++] = ((twist[i] + j * 4.0f / MAX_DIVS) / textureWidth);
                    localTextureCoords[count++] = (i * 5.5f / MAX_SEGMENTS);

                    localTextureCoords[count++] = ((twist[i] + (j + 1) * 4.0f / MAX_DIVS) / textureWidth);
                    localTextureCoords[count++] = (i * 5.5f / MAX_SEGMENTS);

                    localTextureCoords[count++] = ((twist[i + 1] + (j + 1) * 4.0f / MAX_DIVS) / textureWidth);
                    localTextureCoords[count++] = ((i + 1) * 5.5f / MAX_SEGMENTS);

                }
            }
            return localTextureCoords;
        }
    }
}
