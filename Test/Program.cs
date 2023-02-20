using System.Data;
using System.Linq.Expressions;
using FreeSql.Tests.QuestDb.QuestDbTestModel;
using Newtonsoft.Json.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using FreeSql;
using FreeSql.Provider.QuestDb;

namespace Test
{
    internal class Program
    {
        static IFreeSql fsql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(FreeSql.DataType.QuestDb,
                @"host=192.168.0.36;port=8812;username=admin;password=quest;database=qdb;ServerCompatibilityMode=NoTypeLoading;")
            .UseMonitorCommand(cmd => Console.WriteLine($"Sql：{cmd.CommandText}")) //监听SQL语句
            .UseNoneCommandParameter(true)
            .Build();


        static void Main(string[] args)
        {
            var sql = fsql.Select<QuestDb_Model_Test01>()
                .LatestOn(q => q.CreateTime, q => new { q.Id, q.NameUpdate })
                .ToSql();
            Console.WriteLine(sql);
            var sql2 = fsql.Select<QuestDb_Model_Test01>()
                .SampleBy(1, SampleUnits.d)
                .WithTempQuery(q => new { q.Id, q.Activos, count = SqlExt.Count(q.Id).ToValue() })
                .Where(q => q.Id != "1")
                .ToSql();
            Console.WriteLine(sql2);
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