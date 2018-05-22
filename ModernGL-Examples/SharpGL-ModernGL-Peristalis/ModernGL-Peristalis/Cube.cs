using GlmNet;
using SharpGL;

namespace NewGL
{
    public class Cube : BaseModel
    {

        public Cube() { }

        public void Initialise(OpenGL gl)
        {
            GenerateCubeGeometry();
            base.Initialise(gl, "background.bmp");
        }

        public void Draw(OpenGL gl, mat4 projectionMatrix, mat4 viewMatrix)
        {
            theta += 0.07f;

            mat4 rotation = glm.rotate(mat4.identity(), theta, new vec3(1, 1, 0));
            mat4 translation = glm.translate(mat4.identity(), new vec3(0, 0, 0));
            mat4 scale = glm.scale(mat4.identity(), new vec3(10.25f, 10.25f, 10.25f));

            modelMatrix = rotation * scale * translation;

            gl.UseProgram(shaderProgram);

            // Bind the vertex Buffer Array
            gl.BindVertexArray(VBA[0]);

            // Bind the matrix to the Vertex shader matrix
            var p = gl.GetUniformLocation(shaderProgram, "projectionMatrix");
            gl.UniformMatrix4(p, 1, false, projectionMatrix.to_array());
            var v = gl.GetUniformLocation(shaderProgram, "viewMatrix");
            gl.UniformMatrix4(v, 1, false, viewMatrix.to_array());
            var unif = gl.GetUniformLocation(shaderProgram, "ModelWorld4x4");
            gl.UniformMatrix4(unif, 1, false, modelMatrix.to_array());

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureValue);

            gl.DrawArrays(OpenGL.GL_TRIANGLES, 0, vertices.Length);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.BindVertexArray(0);
            gl.UseProgram(0);
        }

        private void GenerateCubeGeometry()
        {
            vertices = new float[] {
                // Front Face
                -1.0f, -1.0f, 1.0f,//1
                1.0f, -1.0f, 1.0f,//2
                1.0f, 1.0f, 1.0f,//3

                -1.0f, -1.0f, 1.0f,//1
                -1.0f, 1.0f, 1.0f,//4
                1.0f, 1.0f, 1.0f,//3
                
                // Back Face
                -1.0f, -1.0f, -1.0f,//1
                -1.0f, 1.0f, -1.0f,//2
                1.0f, 1.0f, -1.0f,//3

                -1.0f, -1.0f, -1.0f,//1
                1.0f, -1.0f, -1.0f,//4
                1.0f, 1.0f, -1.0f,//3

                // Top Face
                -1.0f, 1.0f, -1.0f,//1
                -1.0f, 1.0f, 1.0f,//2
                1.0f, 1.0f, 1.0f,//3

                -1.0f, 1.0f, -1.0f,//1
                1.0f, 1.0f, -1.0f,//4
                1.0f, 1.0f, 1.0f,//3

                // Bottom Face
                -1.0f, -1.0f, -1.0f,//1
                1.0f, -1.0f, -1.0f,//2
                1.0f, -1.0f, 1.0f,//3

                -1.0f, -1.0f, -1.0f,//1
                -1.0f, -1.0f, 1.0f,//4
                 1.0f, -1.0f, 1.0f,//3

                // Right face
                1.0f, -1.0f, -1.0f,//1
                1.0f, 1.0f, -1.0f,//2
                1.0f, 1.0f, 1.0f,//3

                1.0f, -1.0f, -1.0f,//1
                1.0f, -1.0f, 1.0f,//4
                1.0f, 1.0f, 1.0f,//3

                // Left Face
                -1.0f, -1.0f, -1.0f,//1
                -1.0f, -1.0f, 1.0f,//2
                -1.0f, 1.0f, 1.0f,//3

                -1.0f, -1.0f, -1.0f,//1
                -1.0f, 1.0f, -1.0f,//4
                -1.0f, 1.0f, 1.0f,//3
            };

            texCoords = new float[] {
                // Front Face
                0.0f, 0.0f,//1
                1.0f, 0.0f,//2
                1.0f, 1.0f,//3

                0.0f, 0.0f,//1
                0.0f, 1.0f,//4
                1.0f, 1.0f,//3

                // Back Face
                1.0f, 0.0f,//1
                1.0f, 1.0f,//2
                0.0f, 1.0f,//3

                1.0f, 0.0f,//1
                0.0f, 0.0f,//4
                0.0f, 1.0f,//3

                // Top Face
                0.0f, 1.0f,//1
                0.0f, 0.0f,//2
                1.0f, 0.0f,//3

                0.0f, 1.0f,//1
                1.0f, 1.0f,//4
                1.0f, 0.0f,//3

                // Bottom Face
                1.0f, 1.0f,//1
                0.0f, 1.0f,//2
                0.0f, 0.0f,//3

                1.0f, 1.0f,//1
                1.0f, 0.0f,//4
                0.0f, 0.0f,//3

                // Right face
                1.0f, 0.0f,//1
                1.0f, 1.0f,//2
                0.0f, 1.0f,//3

                1.0f, 0.0f,//1
                0.0f, 0.0f,//4
                0.0f, 1.0f,//3

                // Left Face
                0.0f, 0.0f,//1
                1.0f, 0.0f,//2
                1.0f, 1.0f,//3

                0.0f, 0.0f,//1
                0.0f, 1.0f,//4
                1.0f, 1.0f,//3
            };
        }
    }
}
