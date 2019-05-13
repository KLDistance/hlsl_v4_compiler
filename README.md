# hlsl v4 compiler
**By Yunong Wang (DrippySummer @ KLDistance)**
<br><br>
Simple compiler of hlsl shader with model version 4 used in daily development.<br>
This is an implementation of SlimDX.
<br><br>
## Usage
This is a command line tool with some parameters.<br>
If you want to compile single .vs or .ps, execute it as<br>

> hlslc.exe xxxx.vs<br><br>

This command will generate "xxxx.cso" in current directory.<br>
There are other types of input parameters, with the following format.<br>

> hlslc.exe [xxxx.vs] [-d target_directory]<br><br>

the detailed information of optional parameters are as follows.<br>

-d &lt;target_directory&gt;<br>