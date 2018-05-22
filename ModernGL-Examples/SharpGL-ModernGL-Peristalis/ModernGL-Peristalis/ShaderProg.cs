using System;
using System.Text;
using SharpGL;
using SharpGL.Shaders;

namespace NewGL
{
    public class ShaderProg
    {
        private uint vertexShader;
        private uint fragmentShader;
        private uint shaderProgram { get; set; }
        private OpenGL gl;

        public ShaderProg(OpenGL gl)
        {
            this.gl = gl;
        }

        public uint Loader(string vertexShaderPath, string fragmentShaderPath)
        {
            // Vertex Shader
            string vertexShaderSource = System.IO.File.ReadAllText(vertexShaderPath);
            string fragmentShaderSource = System.IO.File.ReadAllText(fragmentShaderPath);

            vertexShader = gl.CreateShader(OpenGL.GL_VERTEX_SHADER);
            gl.ShaderSource(vertexShader, vertexShaderSource);
            gl.CompileShader(vertexShader);

            if (GetCompileStatus(gl, vertexShader) == false)
            {
                throw new ShaderCompilationException(string.Format("Failed to compile shader with ID {0}.", vertexShader), GetCompileInfoLog(vertexShader));
            }

            // Fragment Shader
            fragmentShader = gl.CreateShader(OpenGL.GL_FRAGMENT_SHADER);
            gl.ShaderSource(fragmentShader, fragmentShaderSource);
            gl.CompileShader(fragmentShader);

            if (GetCompileStatus(gl, fragmentShader) == false)
            {
                throw new ShaderCompilationException(string.Format("Failed to compile shader with ID {0}.", fragmentShader), GetCompileInfoLog(fragmentShader));
            }

            // Link shaders
            shaderProgram = gl.CreateProgram();
            gl.AttachShader(shaderProgram, vertexShader);
            gl.AttachShader(shaderProgram, fragmentShader);
            gl.LinkProgram(shaderProgram);

            if (GetLinkStatus(gl) == false)
            {
                throw new ShaderCompilationException(string.Format("Failed to link shader program with ID {0}.", shaderProgram), GetLinkInfoLog(gl));
            }

            gl.DeleteShader(vertexShader);
            gl.DeleteShader(fragmentShader);

            return shaderProgram;
        }

        private string GetCompileInfoLog(uint shaderObject)
        {
            //  Get the info log length.
            int[] infoLength = new int[] { 0 };
            gl.GetShader(shaderObject, OpenGL.GL_INFO_LOG_LENGTH, infoLength);
            int bufSize = infoLength[0];

            //  Get the compile info.
            StringBuilder il = new StringBuilder(bufSize);
            gl.GetShaderInfoLog(shaderObject, bufSize, IntPtr.Zero, il);

            return il.ToString();
        }

        private bool GetCompileStatus(OpenGL gl, uint shaderObject)
        {
            int[] parameters = new int[] { 0 };
            gl.GetShader(shaderObject, OpenGL.GL_COMPILE_STATUS, parameters);
            return parameters[0] == OpenGL.GL_TRUE;
        }

        public string GetLinkInfoLog(OpenGL gl)
        {
            //  Get the info log length.
            int[] infoLength = new int[] { 0 };
            gl.GetProgram(shaderProgram, OpenGL.GL_INFO_LOG_LENGTH, infoLength);
            int bufSize = infoLength[0];

            //  Get the compile info.
            StringBuilder il = new StringBuilder(bufSize);
            gl.GetProgramInfoLog(shaderProgram, bufSize, IntPtr.Zero, il);

            return il.ToString();
        }

        public bool GetLinkStatus(OpenGL gl)
        {
            int[] parameters = new int[] { 0 };
            gl.GetProgram(shaderProgram, OpenGL.GL_LINK_STATUS, parameters);
            return parameters[0] == OpenGL.GL_TRUE;
        }
    }
}
