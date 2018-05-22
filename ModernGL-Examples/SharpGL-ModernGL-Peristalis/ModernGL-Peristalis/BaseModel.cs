using System;
using GlmNet;
using SharpGL;
using SharpGL.SceneGraph.Assets;

namespace NewGL
{
    public class BaseModel
    {
        // Vertex Buffer Array
        protected uint[] VBA;
        // Vertex Buffer Objects
        protected uint[] VBO;

        protected float[] vertices;
        protected float[] texCoords;

        protected uint shaderProgram;
        protected ShaderProg sp;
        protected Texture texture = new Texture();
        protected uint textureValue = 0;
        protected mat4 modelMatrix;
        protected float theta = 0.0f;

        public BaseModel() {}

        protected void Initialise(OpenGL gl, string textureName)
        {

            sp = new ShaderProg(gl);

            // Load the shader's & link them to the program
            shaderProgram = sp.Loader("Shader.vert", "Shader.frag");

            VBA = new uint[1];
            gl.GenVertexArrays(1, VBA);
            gl.BindVertexArray(VBA[0]);

            // Allocate 2 buffers (verts & UV's)
            VBO = new uint[2];

            // Generate the 2 buffers
            gl.GenBuffers(2, VBO);

            // Bind the first buffer
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, VBO[0]);
            // Copy the data from the Vertex array into the array buffer
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, vertices, OpenGL.GL_STATIC_DRAW);
            // Align the Buffer to the Vertex shader layout location value (0)
            gl.VertexAttribPointer(0, 3, OpenGL.GL_FLOAT, false, 3 * sizeof(float), IntPtr.Zero);
            // Unbind the buffer
            gl.EnableVertexAttribArray(0);
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);

            // Bind the UV's buffer
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, VBO[1]);
            // Copy the data from the Colour array into the array buffer
            gl.BufferData(OpenGL.GL_ARRAY_BUFFER, texCoords, OpenGL.GL_STATIC_DRAW);
            // Align the Buffer to the Vertex shader layout location value (1)
            gl.VertexAttribPointer(1, 2, OpenGL.GL_FLOAT, false, 2 * sizeof(float), IntPtr.Zero);
            // Unbind the buffer
            gl.EnableVertexAttribArray(1);
            gl.BindBuffer(OpenGL.GL_ARRAY_BUFFER, 0);

            // Unbind VBA
            gl.BindVertexArray(0);

            texture.Create(gl, textureName);
            textureValue = texture.TextureName;

            //  Specify linear filtering.
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);

            //  Create a model matrix to make the model a little bigger.
            modelMatrix = glm.scale(new mat4(1.0f), new vec3(1.5f));//2.5

        }
    }
}
