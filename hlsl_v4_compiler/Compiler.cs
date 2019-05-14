#define GENERATE_MSG

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
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
    public class TicksTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
        // frequency
        double msFreq;
        long startTime, stopTime;
        public TicksTimer()
        {
            long freq = 0;
            TicksTimer.QueryPerformanceFrequency(out freq);
            this.msFreq = freq * 1000.0;
            this.startTime = 0;
            this.stopTime = 0;
        }
        public void MsTickStart()
        {
            TicksTimer.QueryPerformanceCounter(out this.startTime);
        }
        public double MsTickCut()
        {
            TicksTimer.QueryPerformanceCounter(out this.stopTime);
            return (this.stopTime - this.startTime) / this.msFreq;
        }
    }
    public class Semantics
    {
        // enum for parameter type
        enum ParameterType
        {
            param_d,    // generate cso targets directory
            param_s,    // source clusters of file compilation indicator
            param_r,    // recursive file compilation indicator
            param_e,    // shader entry function name indicator
            param_v,    // vertex shader suffix indicator
            param_p,    // pixel shader suffix indicator
            param_g     // geometry shader suffix indicator
        };
        string[] inputParameters = null;
        Dictionary<ParameterType, string[]> parameterDir = null;
        public Semantics()
        {

        }
        public Semantics(string[] parameters)
        {
            inputParameters = parameters;
            this.parameterDir = new Dictionary<ParameterType, string[]>();
            SegmentPrehash();
        }
        private void SegmentPrehash()
        {
            // find parameter indicator and set indents
            List<int[]> indentPosition = new List<int[]>();
            for(int i = this.inputParameters.Length - 1, j = 0; i >= 0; i--)
            {
                if(this.inputParameters[i].StartsWith("-") && this.inputParameters[i].Length == 2)
                {
                    indentPosition.Add(new int[2]{i, j});
                    j = 0;
                }
                else
                    j++;
            }
            // spilt the strings at indent position into dictionary
            foreach (int[] position in indentPosition)
            {
                if(String.Equals(this.inputParameters[position[0]], "-d"))
                {
                    string[] tmpStringSubDir = new string[position[1]];
                    for(int i = 0, j = position[0] + 1; i < position[1]; i++, j++)
                    {
                        tmpStringSubDir[i] = this.inputParameters[j];
                    }
                    this.parameterDir.Add(ParameterType.param_d, tmpStringSubDir);
                }
                else if(String.Equals(this.inputParameters[position[0]], "-s"))
                {
                    string[] tmpStringSubDir = new string[position[1]];
                    for (int i = 0, j = position[0] + 1; i < position[1]; i++, j++)
                    {
                        tmpStringSubDir[i] = this.inputParameters[j];
                    }
                    this.parameterDir.Add(ParameterType.param_s, tmpStringSubDir);
                }
                else if(String.Equals(this.inputParameters[position[0]], "-r"))
                {
                    string[] tmpStringSubDir = new string[position[1]];
                    for (int i = 0, j = position[0] + 1; i < position[1]; i++, j++)
                    {
                        tmpStringSubDir[i] = this.inputParameters[j];
                    }
                    this.parameterDir.Add(ParameterType.param_r, tmpStringSubDir);
                }
            }
            indentPosition.Clear();
        }
        public void CompileWithOptions(string entryName, string vsSuffix, string psSuffix)
        {
            Compiler compiler = new Compiler();
            string[] paramTrial = null;
            
            // set input options for entry name, vs suffix and ps suffix
            this.parameterDir.TryGetValue(ParameterType.param_e, out paramTrial);
            if (paramTrial != null && paramTrial.Length != 0)
            {
                compiler.SetEntryName(entryName);
            }
            this.parameterDir.TryGetValue(ParameterType.param_v, out paramTrial);
            if (paramTrial != null && paramTrial.Length != 0)
            {
                compiler.SetVSShaderSuffix(vsSuffix);
            }
            this.parameterDir.TryGetValue(ParameterType.param_p, out paramTrial);
            if (paramTrial != null && paramTrial.Length != 0)
            {
                compiler.SetPSShaderSuffix(psSuffix);
            }
            // first check recursive compilation, if not, source compilation
            this.parameterDir.TryGetValue(ParameterType.param_r, out paramTrial);
            if (paramTrial != null)
            {
                string[] pathParams = null;
                this.parameterDir.TryGetValue(ParameterType.param_d, out pathParams);
                if (pathParams != null)
                {
                    compiler.CompileRecursiveFile(paramTrial[paramTrial.Length - 1], pathParams[pathParams.Length - 1]);
                }
                else
                {
                    Console.WriteLine(paramTrial[paramTrial.Length - 1]);
                    compiler.CompileRecursiveFileInSitu(paramTrial[paramTrial.Length - 1]);
                }
            }
            this.parameterDir.TryGetValue(ParameterType.param_s, out paramTrial);
            Console.WriteLine(paramTrial[0]);
            if (paramTrial != null)
            {
                string[] pathParams = null;
                this.parameterDir.TryGetValue(ParameterType.param_d, out pathParams);
                if (pathParams != null)
                {
                    foreach (string fp in paramTrial)
                    {
                        Console.WriteLine(fp);
                        compiler.CompileSingleFile(fp, pathParams[pathParams.Length - 1]);
                    }
                }
                else
                {
                    foreach (string fp in paramTrial)
                    {
                        compiler.CompileSingleFile(fp, Directory.GetParent(fp).FullName);
                    }
                }
            }
        }
        public bool IsOptionalCompilation()
        {
            if (this.parameterDir.Count == 0)
                return false;
            else
                return true;
        }
        public void CompileWithoutOptions()
        {
            Compiler compiler = new Compiler();
            if(this.inputParameters.Length == 0)
            {
#if GENERATE_MSG
                Console.WriteLine("Nothing happened.");
#endif
                return;
            }
            for(int i = 0; i < this.inputParameters.Length; i++)
            {
                compiler.CompileSingleFile(this.inputParameters[i], Directory.GetParent(this.inputParameters[i]).FullName);
            }
        }
    }
    public class Compiler
    {
        enum ShaderType
        {
            Nope, VertexShader, PixelShader, GeometryShader
        };
        private string shaderEntryName = null;
        private string vsShaderFileSuffix = null;
        private string psShaderFileSuffix = null;
        public Compiler()
        {
            this.shaderEntryName = "main";
            this.vsShaderFileSuffix = "vs";
            this.psShaderFileSuffix = "ps";
        }
        public Compiler(string entryName, string vsSuffix, string psSuffix)
        {
            this.shaderEntryName = entryName;
            this.vsShaderFileSuffix = vsSuffix;
            this.psShaderFileSuffix = psSuffix;
        }
        public void SetEntryName(string entry_name)
        {
            this.shaderEntryName = entry_name;
        }
        public void SetVSShaderSuffix(string vs_suffix)
        {
            this.vsShaderFileSuffix = vs_suffix;
        }
        public void SetPSShaderSuffix(string ps_suffix)
        {
            this.psShaderFileSuffix = ps_suffix;
        }
        public int CompileSingleFile(string src_file_path, string des_file_path)
        {
            switch(this.CheckShaderType(src_file_path))
            {
                // invalid file type
                case ShaderType.Nope:
                    break;
                // vertex shader
                case ShaderType.VertexShader:
                    {
                        ShaderBytecode vertexByteCode = ShaderBytecode.CompileFromFile(src_file_path, shaderEntryName,
                        "vs_4_0", ShaderFlags.None, EffectFlags.None);
                        string[] fileNameSpilt = src_file_path.Split('\\');
                        this.WriteCsoFile(vertexByteCode.Data, 
                            String.Concat(des_file_path, "\\", fileNameSpilt[fileNameSpilt.Length - 1].Split('.')[0]));
                        vertexByteCode.Dispose();
                    }
                    break;
                case ShaderType.PixelShader:
                    {
                        ShaderBytecode pixelByteCode = ShaderBytecode.CompileFromFile(src_file_path, shaderEntryName,
                        "ps_4_0", ShaderFlags.None, EffectFlags.None);
                        string[] fileNameSpilt = src_file_path.Split('\\');
                        this.WriteCsoFile(pixelByteCode.Data,
                            String.Concat(des_file_path, "\\", fileNameSpilt[fileNameSpilt.Length - 1].Split('.')[0]));
                        pixelByteCode.Dispose();
                    }
                    break;
            }
#if GENERATE_MSG
            Console.WriteLine("{0} compilation done.", src_file_path);
#endif
            return 0;
        }
        public int CompileRecursiveFile(string src_file_path, string des_file_path)
        {
            DirectoryInfo dir = new DirectoryInfo(src_file_path);
            FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();
            // depth first
            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                if (fsinfo is DirectoryInfo)
                {
                    CompileRecursiveFile(fsinfo.FullName, des_file_path);
                }
                else
                {
                    // compile the shader into .cso
                    CompileSingleFile(fsinfo.FullName, des_file_path);
                }
            }
            return 0;
        }
        public int CompileRecursiveFileInSitu(string src_file_path)
        {
            DirectoryInfo dir = new DirectoryInfo(src_file_path);
            FileSystemInfo[] fsinfos = dir.GetFileSystemInfos();
            // depth first
            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                if (fsinfo is DirectoryInfo)
                {
                    CompileRecursiveFileInSitu(fsinfo.FullName);
                }
                else
                {
                    // compile the shader into .cso
                    string[] fileNameSpilt = src_file_path.Split('\\');
                    CompileSingleFile(fsinfo.FullName, String.Concat(src_file_path, "\\", fileNameSpilt[fileNameSpilt.Length - 1]));
                }
            }
            return 0;
        }
        private ShaderType CheckShaderType(string src_file_path)
        {
            string[] filePathArr = src_file_path.Split('.');
            if(filePathArr[filePathArr.Length - 1].Equals(this.vsShaderFileSuffix))
            {
                // 1 means vertex shader
                return ShaderType.VertexShader;
            }
            else if(filePathArr[filePathArr.Length - 1].Equals(this.psShaderFileSuffix))
            {
                // 2 means pixel shader
                return ShaderType.PixelShader;
            }
            else
            {
                // nope
                return ShaderType.Nope;
            }
        }
        private int WriteCsoFile(DataStream shaderDataStream, string des_file_path)
        {
            // check whether the directory exists
            Directory.CreateDirectory(Directory.GetParent(des_file_path).FullName);
            // create and write into the file
            FileStream fpShader = new FileStream(String.Concat(des_file_path, ".cso"), 
                FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Console.WriteLine(String.Concat(des_file_path, ".cso"));
            byte[] buffer = new byte[shaderDataStream.Length];
            shaderDataStream.Read(buffer, 0, buffer.Length);
            fpShader.Write(buffer, 0, buffer.Length);
            fpShader.Close();
            return 0;
        }
    }
}
