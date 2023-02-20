using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeSql.Tests.QuestDb.QuestDbTestModel;
using Xunit;
using static FreeSql.Tests.QuestDb.QuestDbTest;

namespace FreeSql.Tests.QuestDb.Crud
{
    public class QuestDbTestUpdate
    {
        [Fact]
        public void TestNormalUpdate()
        {
            var updateObj = fsql.Update<QuestDb_Model_Test01>()
                .Set(q => q.NameUpdate, "UpdateNow")
                //     .Set(q => q.CreateTime, DateTime.Now)   分表的时间不可以随便改
                .Set(q => q.UpdateTime, DateTime.Now) //其他时间可以随便改
                .Where(q => q.Id == "1");

            var sql = updateObj.ToSql();
            Debug.WriteLine(sql);
            Assert.Equal(@"UPDATE ""QuestDb_Model_Test01"" SET ""NameUpdate"" = @p_0, ""UpdateTime"" = @p_1 
WHERE (""Id"" = '1')", sql);
            var result = updateObj.ExecuteAffrows();
            Assert.True(result > 0);
        }

        [Fact]
        public async Task TestUpdateByModelAsync()
        {
            var primary = Guid.NewGuid().ToString();
            //先插入
            fsql.Insert(new QuestDb_Model_Test01()
            {
                Primarys = primary,
                CreateTime = DateTime.Now,
                Activos = 100.21,
                Id = "Id",
                IsCompra = true,
                NameInsert = "NameInsert",
                NameUpdate = "NameUpdate"
            }).ExecuteAffrows();
            var updateModel = new QuestDb_Model_Test01
            {
                Primarys = primary,
                Id = "2",
                Activos = 12.65,
            };
            var updateObj = fsql.Update<QuestDb_Model_Test01>().SetSourceIgnore(updateModel, o => o == null);
            var sql = updateObj.ToSql();
            var result = updateObj.ExecuteAffrows();
            var resultAsync = await fsql.Update<QuestDb_Model_Test01>().SetSourceIgnore(updateModel, o => o == null)
                .ExecuteAffrowsAsync();
            Assert.True(result > 0);
            Assert.True(resultAsync > 0);
            Assert.Equal(
                @$"UPDATE ""QuestDb_Model_Test01"" SET ""Id"" = @p_0, ""NameInsert"" = @p_1, ""Activos"" = @p_2 
WHERE (""Primarys"" = '{primary}')", sql);
        }

        [Fact]
        public async Task TestUpdateIgnoreColumnsAsync()
        {
            var primary = Guid.NewGuid().ToString();
            var updateTime = DateTime.Now;
            //先插入
            fsql.Insert(new QuestDb_Model_Test01()
            {
                Primarys = primary,
                CreateTime = DateTime.Now,
                Activos = 100.21,
                Id = "Id",
                IsCompra = true,
                NameInsert = "NameInsert",
                NameUpdate = "NameUpdate"
            }).ExecuteAffrows();
            var updateModel = new QuestDb_Model_Test01
            {
                Primarys = primary,
                Id = "2",
                Activos = 12.65,
                IsCompra = true,
                CreateTime = DateTime.Now
            };
            var updateObj = fsql.Update<QuestDb_Model_Test01>().SetSource(updateModel)
                .IgnoreColumns(q => new { q.Id, q.CreateTime });
            var sql = updateObj.ToSql();
            var result = updateObj.ExecuteAffrows();
            var resultAsync = await fsql.Update<QuestDb_Model_Test01>().SetSource(updateModel)
                .IgnoreColumns(q => new { q.Id, q.CreateTime }).ExecuteAffrowsAsync();
            Assert.True(result > 0);
            Assert.True(resultAsync > 0);
            Assert.Equal(
                $@"UPDATE ""QuestDb_Model_Test01"" SET ""NameUpdate"" = NULL, ""NameInsert"" = 'NameDefault', ""Activos"" = 12.65, ""UpdateTime"" = NULL, ""IsCompra"" = True 
WHERE (""Primarys"" = '{primary}')", sql);
        }

        [Fact]
        public async Task TestUpdateToUpdateAsync()
        {
            //官网demo有问题，暂时放弃此功能
            //var result = await fsql.Select<QuestDb_Model_Test01>().Where(q => q.Id == "IdAsync" && q.NameInsert == null)
            //    .ToUpdate()
            //    .Set(q => q.UpdateTime, DateTime.Now).ExecuteAffrowsAsync();
        }
    }
}