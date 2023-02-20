using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeSql.Tests.QuestDb.QuestDbTestModel;
using Xunit;
using static FreeSql.Tests.QuestDb.QuestDbTest;

namespace FreeSql.Tests.QuestDb.Crud
{
    public class QuestDbTestInsert
    {
        [Fact]
        public void TestNormalInsert()
        {
            var result = fsql.Insert(new QuestDb_Model_Test01()
            {
                Primarys = Guid.NewGuid().ToString(),
                CreateTime = DateTime.Now,
                Activos = 100.21,
                Id = "Id",
                IsCompra = true,
                NameInsert = "NameInsert",
                NameUpdate = "NameUpdate"
            }).ExecuteAffrows();
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task TestInsertAsync()
        {
            var result = await fsql.Insert(new QuestDb_Model_Test01()
            {
                Primarys = Guid.NewGuid().ToString(),
                CreateTime = DateTime.Now,
                Activos = 100.21,
                Id = "IdAsync",
                IsCompra = true,
                NameInsert = "NameInsert",
                NameUpdate = "NameUpdate"
            }).ExecuteAffrowsAsync();
            Assert.Equal(1, result);
        }

        [Fact]
        public void TestInsertBatch()
        {
            var list = new List<QuestDb_Model_Test01>()
            {
                new QuestDb_Model_Test01()
                {
                    Primarys = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    Activos = 100.21,
                    Id = "1",
                    IsCompra = true,
                    NameInsert = "NameInsert",
                    NameUpdate = "NameUpdate"
                },
                new QuestDb_Model_Test01()
                {

                    Primarys = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    Activos = 100.21,
                    Id = "2",
                    IsCompra = true,
                    NameInsert = "NameInsert",
                    NameUpdate = "NameUpdate"
                },
                new QuestDb_Model_Test01()
                {
                    Primarys = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    Activos = 100.21,
                    Id = "3",
                    IsCompra = true,
                    NameInsert = "NameInsert",
                    NameUpdate = "NameUpdate"
                },
            };
            var result = fsql.Insert(list).ExecuteAffrows();
            Assert.Equal(3, result);
        }

        [Fact]
        public void TestInsertBatchAsync()
        {
            var list = new List<QuestDb_Model_Test01>()
            {
                new QuestDb_Model_Test01()
                {
                    Primarys = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    Activos = 100.21,
                    Id = "1",
                    IsCompra = true,
                    NameInsert = "NameInsertAsync",
                    NameUpdate = "NameUpdate"
                },
                new QuestDb_Model_Test01()
                {
                    Primarys = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    Activos = 100.21,
                    Id = "2",
                    IsCompra = true,
                    NameInsert = "NameInsertAsync",
                    NameUpdate = "NameUpdate"
                },
                new QuestDb_Model_Test01()
                {
                    Primarys = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    Activos = 100.21,
                    Id = "3",
                    IsCompra = true,
                    NameInsert = "NameInsertAsync",
                    NameUpdate = "NameUpdate"
                },
            };
            var result = fsql.Insert(list).ExecuteAffrows();
            Assert.Equal(3, result);
        }

        [Fact]
        public void TestInsertInsertColumns()
        {
            var list = new List<QuestDb_Model_Test01>()
            {
                new QuestDb_Model_Test01()
                {
                    Primarys = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    Activos = 100.21,
                    Id = "1",
                    IsCompra = true,
                    NameInsert = "NameInsert",
                    NameUpdate = "NameUpdate"
                },
                new QuestDb_Model_Test01()
                {
                    Primarys = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    Activos = 100.21,
                    Id = "2",
                    IsCompra = true,
                    NameInsert = "NameInsert",
                    NameUpdate = "NameUpdate"
                },
                new QuestDb_Model_Test01()
                {
                    Primarys = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    Activos = 100.21,
                    Id = "3",
                    IsCompra = true,
                    NameInsert = "NameInsert",
                    NameUpdate = "NameUpdate"
                },
            };
            var result = fsql.Insert(list).IgnoreColumns(q => q.NameInsert).ExecuteAffrows();
            Assert.Equal(3, result);
        }
    }
}