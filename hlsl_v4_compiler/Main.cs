#define GENERATE_MSG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hlsl_v4_compiler
{
    class EntryClass
    {
        static void Main(string[] args)
        {
            try
            {
                Semantics semantics = new Semantics(args);
                TicksTimer timer = new TicksTimer();
                timer.MsTickStart();
                if (semantics.IsOptionalCompilation())
                {
                    semantics.CompileWithOptions("main", "vs", "ps");
                }
                else
                {
                    semantics.CompileWithoutOptions();
                }
                double msElapsed = timer.MsTickCut();
#if GENERATE_MSG
                Console.WriteLine("Overall shader compilation done. {0:00e+0} ms elapsed.", msElapsed);
#endif 
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid format of inputs! Error type: {0}.\n" +
                    "Use \"hlslc -h\" command to see detailed implementation of the tool.", e.Data);
            }
        }
    }
}
