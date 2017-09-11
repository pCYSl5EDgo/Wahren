using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Wahren
{
    using Specific;
    class Program
    {
        //サンプル
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //Vahren.exeと同階層にあるシナリオフォルダを指定してネ
            //シナリオフォルダのエンコード（Shift-JIS）や英文モードとかを解析するヨ
            var sc = new ScenarioFolder(@"C:\kinotake_war\kinotake");
            System.Console.Error.WriteLine($"Wait For {sc.Script_Dat.Count} Files!");
            //どのフォルダを解析するのか設定してあげよう
            ScriptLoader.Folder = sc;
            ConcurrentBag<ScenarioData2> Scenario = new ConcurrentBag<ScenarioData2>();
            //Scriptフォルダ以下の.dat全てをかるーく非同期に解析するよ
            //ここである程度構造体の継承も処理するが別ファイルからのは継承できなかったりする
            Task.Factory.StartNew(() => Task.WaitAll(ScriptLoader.LoadAllAsync())).ContinueWith((_) =>
            {
                //全ファイルが出揃ったら解決できていない継承を解決する
                //名前は良い名前が思い浮かんだら変更する予定
                ScriptLoader.Resolve2nd();
                System.Console.Error.WriteLine("Resolved!");
                //コンストラクタ内で解析するのはバッドノウハウなのかな？
                foreach (var item in ScriptLoader.Scenario.Select(_1 => new ScenarioData2(_1.Value)))
                {
                    Scenario.Add(item);
                    System.Console.WriteLine("SCENARIO : " + item.Name);
                    System.Console.WriteLine(item.BoolIdentifier.First());
                    System.Console.Error.WriteLine(item.BoolIdentifier.Count);
                }
                System.Console.Error.WriteLine("DONE!");
            });
            while (true)
            {
                var l = System.Console.ReadLine();
                ScenarioData2 data2;
                if (l == "q") return;
                if (l == "p" && Scenario.TryTake(out data2))
                {
                    foreach (var item in data2.Detail)
                    {
                        System.Console.WriteLine($"{item.Key}:\n{item.Value}");
                    }
                }
            }
        }
    }
}