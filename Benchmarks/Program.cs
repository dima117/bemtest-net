﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Bem.Engine;
using Benchmarks.Schema;
using Newtonsoft.Json;
using RazorEngine;
using RazorEngine.Templating;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //GenerateBemJson();
            RunRenderBenchmark4();

            Console.WriteLine("======");
            Console.ReadKey();
        }

        public static void RunRenderBenchmark()
        {
            var bemhtml = new BemhtmlEngine();

            var json = File.ReadAllText("test.bemjson.json");
            //object data = JObject.Parse(json);
            object data = GenerateBemJson();

            Console.WriteLine("===");
            var now = DateTime.Now;

            for (var i = 0; i < 1000; i++)
            {
                var task = bemhtml.Render(null, data);
                // var task = BemhtmlEngine.Instance.Render(null, data);
                task.Wait();
            }

            Console.WriteLine((DateTime.Now - now).TotalMilliseconds);
            //Console.WriteLine(task.Result);
            Console.ReadKey();
        }

        public static void RunRenderBenchmark2()
        {
            var content = File.ReadAllText(@"C:\projects\bemtest-net\Benchmarks\Bem\desktop.bundles\default\default.bemhtml.js");
            var template = new BemhtmlTemplate(content);

            BemhtmlRoot data = GenerateBemJson();

            var task2 = template.Apply(data);
            task2.Wait();

            var now = DateTime.Now;

            for (var i = 0; i < 1000; i++)
            {
                var task = template.Apply(data);
                task.Wait();
            }

            Console.WriteLine((DateTime.Now - now).TotalMilliseconds);
            //Console.WriteLine(task.Result);
        }

        public static void RunRenderBenchmark3()
        {
            BemhtmlRoot data = GenerateBemJson();
            var template = File.ReadAllText(@"C:\projects\bemtest-net\Benchmarks\Razor\test.cshtml");
            Engine.Razor.Compile(template, "templateKey", typeof(BemhtmlRoot));

            var now = DateTime.Now;

            for (var i = 0; i < 1000; i++)
            {
                var result = Engine.Razor.Run("templateKey", typeof(BemhtmlRoot), data);

            }

            Console.WriteLine((DateTime.Now - now).TotalMilliseconds);
        }

        public static void RunRenderBenchmark4()
        {
            var template222 = new BemhtmlTemplate("block('xxx').content()('===');");

            BemhtmlRoot data = GenerateBemJson();
            var template = File.ReadAllText(@"C:\projects\bemtest-net\Benchmarks\Razor\test2.cshtml");
            Engine.Razor.Compile(template, "templateKey", typeof(BemhtmlRoot));

            var now = DateTime.Now;

            //var result = Engine.Razor.Run("templateKey", typeof(BemhtmlRoot), data);
            //Console.WriteLine(result);

            for (var i = 0; i < 1000; i++)
            {
                var result = Engine.Razor.Run("templateKey", typeof(BemhtmlRoot), data);
            }

            Console.WriteLine((DateTime.Now - now).TotalMilliseconds);
        }

        public static BemhtmlRoot GenerateBemJson()
        {
            var data = new List<BemhtmlGroup>();


            for (var i = 0; i < 100; i++)
            {
                data.Add(new BemhtmlGroup
                {
                    block = "group",
                    title = "title-" + i,
                    content = new[]
                    {
                        new BemhtmlItem
                        {
                            block = "item",
                            text = "item-" + i + "-1"
                        },
                        new BemhtmlItem
                        {
                            block = "item",
                            text = "item-" + i + "-2"
                        },
                        new BemhtmlItem
                        {
                            block = "item",
                            text = "item-" + i + "-3"
                        }
                    }
                });
            }

            File.WriteAllText(@"C:\projects\bemtest-net\Benchmarks\test.bemjson.json", JsonConvert.SerializeObject(new { block = "root", items = data }));
            return new BemhtmlRoot { block = "root", items = data.ToArray() };
        }
    }
}
