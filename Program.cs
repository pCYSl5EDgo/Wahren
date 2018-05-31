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
        static readonly ConcurrentBag<ScenarioData2> Scenario;
        static Program()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            app = new CommandLineApplication();
            app.Name = nameof(Wahren);
            var folderArgument = app.Argument("folder", "", false);
            Scenario = new ConcurrentBag<ScenarioData2>();
            app.OnExecute(async () =>
            {
                //Vahren.exeと同階層にあるシナリオフォルダを指定してネ
                //シナリオフォルダのエンコード（Shift-JIS）や英文モードとかを解析するヨ
                var sc = new ScenarioFolder(folderArgument.Value);
                Console.Error.WriteLine($"Wait For {sc.Script_Dat.Count} Files!");
                //どのフォルダを解析するのか設定してあげよう
                ScriptLoader.Folder = sc;
                //Scriptフォルダ以下の.dat全てをかるーく非同期に解析するよ
                //ここである程度構造体の継承も処理するが別ファイルからのは継承できなかったりする
                await Task.Factory.StartNew(() => Task.WaitAll(ScriptLoader.LoadAllAsync()));
                //全ファイルが出揃ったら解決できていない継承を解決する
                //名前は良い名前が思い浮かんだら変更する予定
                ScriptLoader.Resolve2nd();
                Console.Error.WriteLine("Resolved!");
                //コンストラクタ内で解析するのはバッドノウハウなのかな？
                foreach (var item in ScriptLoader.Scenario.Select(_1 => new ScenarioData2(_1.Value)))
                {
                    Scenario.Add(item);
                    Console.WriteLine("SCENARIO : " + item.Name);
                    Console.WriteLine(item.BoolIdentifier.First());
                    Console.Error.WriteLine(item.BoolIdentifier.Count);
                }
                Console.Error.WriteLine("DONE!");
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
            catch(AggregateException e)
            {
                Console.WriteLine(e.InnerException.ToString());
                return;
            }
            while (true)
            {
                var l = Console.ReadLine();
                ScenarioData2 data2;
                if (l == "q") return;
                if (l == "p" && Scenario.TryTake(out data2))
                {
                    foreach (var item in data2.Detail)
                    {
                        Console.WriteLine($"{item.Key}:\n{item.Value}");
                    }
                }
            }
        }
    }
}