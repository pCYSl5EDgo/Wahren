using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;

namespace Wahren
{
    using Specific;
    static class Program
    {
        static readonly CommandLineApplication app;
        static Program()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            app = new CommandLineApplication();
            app.Name = nameof(Wahren);
            var folderArgument = app.Argument("folder", "", false);
            app.OnExecute(() =>
            {
                ScriptLoader.InitializeComponent(folderArgument.Value);
                return 0;
            });
        }
        //サンプル
        static void Main(string[] args)
        {
            try
            {
                app.Execute(args);
            }
            catch (AggregateException e)
            {
                Console.Error.WriteLine(e.InnerException.ToString());
                return;
            }
        }
    }
}