using System;
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
        private ID3DDevice device = null;
        private ID3DSwapChain swapChain = null;
        private RenderForm pseudoForm = null;
        
        public Compiler()
        {
            this.pseudoForm = new RenderForm("pseudoForm");
            SwapChainDescription swapChainDescription = new SwapChainDescription()
            {
                BufferCount = 1,
                Usage = Usage.RenderTargetOutput,
                OutputHandle = this.pseudoForm.Handle,
                IsWindowed = true,
                ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                SampleDescription = new SampleDescription(1, 0),
                Flags = SwapChainFlags.AllowModeSwitch,
                SwapEffect = SwapEffect.Discard
            };
        }
    }
}
