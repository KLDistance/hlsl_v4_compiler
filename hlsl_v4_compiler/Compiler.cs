using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using ID3DDevice = SlimDX.Direct3D11.Device;
using ID3DResource = SlimDX.Direct3D11.Resource;
using ID3DBuffer = SlimDX.Direct3D11.Buffer;
using ID3DSwapChain = SlimDX.DXGI.SwapChain;

namespace hlsl_v4_compiler
{
    public class Semantics
    {

    }
    public class Compiler
    {
        private string shaderEntryName = null;
        private string vsShaderFileSuffix = null;
        private string psShaderFileSuffix = null;
        public Compiler()
        {
            this.shaderEntryName = "main";
            this.vsShaderFileSuffix = ".vs";
            this.psShaderFileSuffix = ".ps";
        }
        public Compiler(string entryName, string vsSuffix, string psSuffix)
        {
            this.shaderEntryName = entryName;
            this.vsShaderFileSuffix = vsSuffix;
            this.psShaderFileSuffix = psSuffix;
        }
        public int CompileSingleFile(string src_file_path, string des_file_path)
        {
            switch(this.CheckShaderType(src_file_path))
            {
                // invalid file type
                case 0:
                    break;
                // vertex shader
                case 1:
                    {
                        ShaderBytecode vertexByteCode = ShaderBytecode.CompileFromFile(src_file_path, shaderEntryName,
                        "vs_4_0", ShaderFlags.None, EffectFlags.None);
                        this.WriteCsoFile(vertexByteCode.Data, des_file_path);
                        vertexByteCode.Dispose();
                    }
                    break;
                case 2:
                    {
                        ShaderBytecode pixelByteCode = ShaderBytecode.CompileFromFile(src_file_path, shaderEntryName,
                        "ps_4_0", ShaderFlags.None, EffectFlags.None);
                        this.WriteCsoFile(pixelByteCode.Data, des_file_path);
                        pixelByteCode.Dispose();
                    }
                    break;
            }
            return 0;
        }
        private int CheckShaderType(string src_file_path)
        {
            string[] filePathArr = src_file_path.Split('.');
            if(filePathArr[filePathArr.Length - 1].Equals(this.vsShaderFileSuffix))
            {
                // 1 means vertex shader
                return 1;
            }
            else if(filePathArr[filePathArr.Length - 1].Equals(this.psShaderFileSuffix))
            {
                // 2 means pixel shader
                return 2;
            }
            else
            {
                // nope
                return 0;
            }
        }
        private int WriteCsoFile(DataStream shaderDataStream, string des_file_path)
        {
            FileStream fpShader = new FileStream(String.Concat(des_file_path, ".cso"), 
                FileMode.OpenOrCreate, FileAccess.ReadWrite);
            byte[] buffer = new byte[shaderDataStream.Length];
            shaderDataStream.Write(buffer, 0, buffer.Length);
            fpShader.Write(buffer, 0, buffer.Length);
            fpShader.Close();
            return 0;
        }
    }
}
