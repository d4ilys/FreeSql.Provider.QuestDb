using CsvHelper;
using FreeSql;
using Newtonsoft.Json;
using System.Collections;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.VisualBasic;

namespace Test
{
    internal class Program
    {
        static IFreeSql fsql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(FreeSql.DataType.QuestDb,
                @"host=192.168.0.36;port=8812;username=admin;password=quest;database=qdb;ServerCompatibilityMode=NoTypeLoading;")
            .UseMonitorCommand(cmd => Console.WriteLine($"Sql：{cmd.CommandText}")) //监听SQL语句
            .UseQuestDbRestAPI("192.168.0.36:9001", "admin", "ushahL(aer2r")
            .UseNoneCommandParameter(true)
            .Build();


        static async Task Main(string[] args)
        {
            //var date = DateTime.Parse("2023-12-21 15:15:11");
            //var list = new List<QuestDb_Model_Test01>();
            //for (int i = 0; i < 100000; i++)
            //{
            //    list.Add(new QuestDb_Model_Test01()
            //    {
            //        Primarys = Guid.NewGuid().ToString(),
            //        CreateTime = date,
            //        Activos = 100 + i,
            //        Id = "1",
            //        IsCompra = true,
            //        NameInsert = "NameInsertAsync",
            //        NameUpdate = "NameUpdate"
            //    });
            //}
            //Stopwatch stopwatch = Stopwatch.StartNew();
            //var result = fsql.Insert(list).ExecuteBulkCopy();
            //stopwatch.Stop();
            //Console.WriteLine($"批量插入10000条数据，成功：{result}条，耗时：{stopwatch.Elapsed}");
            //Console.WriteLine(sql);
            Parallel.For(0, 10, i =>
            {
                var sql = fsql.Insert(new QuestDb_Model_Test01()
                {
                    Activos = 1,
                    CreateTime = DateTime.Now,
                    Id = "Id",
                    NameInsert = "1",
                    IsCompra = true,
                    NameUpdate = "update"
                }).ExecuteAffrows();
                Console.WriteLine(sql);
            });

            Console.ReadKey();

            //var select = fsql.Select<Topic, Category, CategoryType>()
            //    .LeftJoin(w => w.t1.CategoryId == w.t2.Id)
            //    .LeftJoin(w => w.t2.ParentId == w.t3.Id)
            //    .Where(w => w.t3.Id > 0)
            //    .ToSql();
            //Console.WriteLine(select);
            //new Thread(() =>
            //{
            //    Task.Run(() =>
            //    {
            //        var sql = fsql.Select<QuestDb_Model_Test01>()
            //            .SampleBy(1, "d")
            //            .Where(q => q.Id != "1")
            //            .ToList(a => new
            //            {
            //                a.Id,
            //                count = SqlExt.Count(a.Id)
            //            });
            //        Console.WriteLine(sql.Count);
            //    });
            //    Task.Run(() =>
            //    {
            //        var sql = fsql.Select<QuestDb_Model_Test01>()
            //            .SampleBy(3, "h")
            //            .Where(q => q.Id != "1")
            //            .ToList(a => new
            //            {
            //                a.Id,
            //                count = SqlExt.Count(a.Id)
            //            });
            //        Console.WriteLine(sql.Count);
            //    });
            //}).Start();

            //var list2 = fsql.Select<QuestDb_Model_Test01>()
            //    .GroupBy(a => new { a.Id, a.NameInsert })
            ////    .Having(a => a.Count() > 10 && a.Key.Id != "1")
            //    .OrderByDescending(a => a.Count())  
            //    .ToList(a => new
            //    {
            //        a.Key,
            //        cou1 = a.Count(),
            //        arg1 = a.Avg(a.Value.Activos),
            //        arg2 = a.Count(a.Value.NameInsert)
            //    });
            //Console.WriteLine(list2.Count);

            Console.ReadKey();
        }
    }
}