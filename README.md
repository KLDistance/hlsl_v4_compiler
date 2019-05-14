# hlsl v4 compiler
**By Yunong Wang (DrippySummer @ KLDistance)**
<br><br>
Simple compiler of hlsl shader with model version 4 used in daily development.<br>
This program implements SlimDX and .NET framework 3.5 (Win7, 8, 8.1 and 10 supported).
<br><br>
## Usage
The command line tool requires some parameters to trace the source and generate target .cso files.<br>
If you want to compile single or multipule .vs or .ps, execute it as<br>
> hlslc.exe xxxx.vs oooo.ps
This command will generate "xxxx.cso" and "oooo.cso" in current directory.<br>
There are other types of input parameters, with the following format.<br><br>
To compile a cluster of shaders<br>
> hlslc.exe [-s] &lt;src_ps_vs&gt;
To compile a cluster of shaders and put the generated binaries (.cso) in the specified directory<br>
> hlslc.exe -s &lt;src_ps_vs&gt; -d &lt;target_dir&gt;
To recursively compile the shaders with depth-first approach, if you prefer automatic method to find the sub-directory shaders
> hlslc.exe -r &lt;src_init_dir&gt; [-d &lt;target_dir&gt;]

<br>
Best regards!
<br>