# hlsl v4 compiler
**By Yunong Wang (DrippySummer @ KLDistance)**
<br><br>
Simple compiler of hlsl shader with model version 4 used in daily development.<br>
This program implements SlimDX and .NET framework 3.5 (Win7, 8, 8.1 and 10 supported).
<br><br>
## Usage
The command line tool requires some parameters to trace the source and generate target .cso files.<br>
If you want to compile single .vs or .ps, execute it as<br>
> hlslc.exe xxxx.vs
<br>
This command will generate "xxxx.cso" in current directory.<br>
There are other types of input parameters, with the following format.<br><br>
To compile a cluster of shaders<br>
> hlslc.exe [-s] &lt;src_ps_vs&gt;
<br>
To compile a cluster of shaders and put the generated binaries (.cso) in the specified directory<br>
> hlslc.exe -s <src_ps_vs> -d <target_dir>
<br>
To recursively compile the shaders with depth-first approach, if you prefer automatic method to find the sub-directory shaders
> hlslc.exe -r <src_init_dir> [-d <target_dir>]
<br>

<br>
Best regards!
<br>