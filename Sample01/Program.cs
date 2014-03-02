using System;
using System.Collections.Generic;
using MiniCube;

namespace Sample01
{
    /// <summary>
    /// Simple Sample01 application using SharpDX.Toolkit.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
#if NETFX_CORE
        [MTAThread]
#else
        [STAThread]
#endif
        static void Main()
        {
            using (var program = new MiniCubeGame())
                program.Run();

        }
    }
}