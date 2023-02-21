﻿using CsvHelper;
using FreeSql;
using FreeSql.Internal.CommonProvider;
using FreeSql.Internal.Model;
using FreeSql.QuestDb;
using FreeSql.QuestDb.Curd;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using FreeSql.Provider.QuestDb;

public static partial class QuestDbGlobalExtensions
{
    /// <summary>
    /// 特殊处理类似 string.Format 的使用方法，防止注入，以及 IS NULL 转换
    /// </summary>
    /// <param name="that"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string FormatPostgreSQL(this string that, params object[] args) =>
        _postgresqlAdo.Addslashes(that, args);

    static QuestDbAdo _postgresqlAdo = new QuestDbAdo();


    /// <summary>
    /// 逐行读取，包含空行
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private static List<string> SplitByLine(string text)
    {
        List<string> lines = new List<string>();
        byte[] array = Encoding.UTF8.GetBytes(text);
        using (MemoryStream stream = new MemoryStream(array))
        {
            using (var sr = new StreamReader(stream))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = sr.ReadLine();
                }

                ;
            }
        }

        return lines;
    }

    /// <summary>
    /// 批量快速插入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="that"></param>
    /// <returns></returns>
    public static async Task<int> ExecuteBulkCopyAsync<T>(this IInsert<T> that) where T : class
    {
        var result = 0;
        var fileName = $"{Guid.NewGuid()}.csv";
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        try
        {
            var client = QuestDbContainer.GetService<IHttpClientFactory>().CreateClient();
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            var name = typeof(T).Name;
            var list = new List<Hashtable>();
            var insert = that as QuestDbInsert<T>;
            insert.InternalOrm.DbFirst.GetTableByName(name).Columns.ForEach(d =>
            {
                if (d.DbTypeText == "TIMESTAMP")
                {
                    list.Add(new Hashtable()
                    {
                        { "name", d.Name },
                        { "type", d.DbTypeText },
                        { "pattern", "yyyy/M/dd HH:mm:ss" }
                    });
                }
                else
                {
                    list.Add(new Hashtable()
                    {
                        { "name", d.Name },
                        { "type", d.DbTypeText }
                    });
                }
            });
            var schema = JsonConvert.SerializeObject(list);
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
            {
                csv.WriteRecords(insert._source);
            }

            var httpContent = new MultipartFormDataContent(boundary);
            client.DefaultRequestHeaders.Add("Authorization", RestAPIExtension.authorization);
            httpContent.Add(new StringContent(schema), "schema");
            httpContent.Add(new ByteArrayContent(File.ReadAllBytes(filePath)), "data");
            //boundary带双引号 可能导致服务器错误情况
            httpContent.Headers.Remove("Content-Type");
            httpContent.Headers.TryAddWithoutValidation("Content-Type",
                "multipart/form-data; boundary=" + boundary);
            var httpResponseMessage =
                await client.PostAsync($"{RestAPIExtension.BaseUrl}/imp?name={name}", httpContent);
            var readAsStringAsync = await httpResponseMessage.Content.ReadAsStringAsync();
            var splitByLine = SplitByLine(readAsStringAsync);
            Console.WriteLine(readAsStringAsync);
            foreach (var s in splitByLine)
            {
                if (s.Contains("Rows"))
                {
                    var strings = s.Split('|');
                    if (strings[1].Trim() == "Rows imported")
                    {
                        result = Convert.ToInt32(strings[2].Trim());
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            try
            {
                File.Delete(filePath);
            }
            catch
            {
            }
        }

        return result;
    }

    public static int ExecuteBulkCopy<T>(this IInsert<T> insert) where T : class
    {
        return ExecuteBulkCopyAsync(insert).GetAwaiter().GetResult();
    }
}

public static class SampleByExtension
{
    //是否使用该方法
    internal static AsyncLocal<bool> IsExistence = new AsyncLocal<bool>()
    {
        Value = false
    };

    internal static AsyncLocal<string> SamoleByString = new AsyncLocal<string>()
    {
        Value = string.Empty
    };

    public static void Initialize()
    {
        IsExistence.Value = false;
        SamoleByString.Value = string.Empty;
    }

    /// <summary>
    /// SAMPLE BY用于时间序列数据，将大型数据集汇总为同质时间块的聚合，作为SELECT语句的一部分。对缺少数据的数据集执行查询的用户可以使用FILL关键字指定填充行为
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="select"></param>
    /// <param name="time">时长</param>
    /// <param name="unit">单位</param>
    /// <returns></returns>
    public static ISelect<T> SampleBy<T>(this ISelect<T> select, double time, SampleUnits unit)
    {
        var _unit = Enum.GetName(typeof(SampleUnits), unit);
        IsExistence.Value = true;
        var samoleByTemple = $"{Environment.NewLine}SAMPLE BY {{0}}{{1}}{Environment.NewLine}";
        SamoleByString.Value = string.Format(samoleByTemple, time.ToString(), _unit);
        return select;
    }
}

public static class LatestOnExtension
{
    //是否使用该方法
    internal static AsyncLocal<bool> IsExistence = new AsyncLocal<bool>()
    {
        Value = false
    };

    internal static AsyncLocal<string> LatestOnString = new AsyncLocal<string>()
    {
        Value = string.Empty
    };

    public static void Initialize()
    {
        IsExistence.Value = false;
        LatestOnString.Value = string.Empty;
    }

    /// <summary>
    /// 对于多个时间序列存储在同一个表中的场景，根据时间戳检索给定键或键组合的最新项。
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="select"></param>
    /// <param name="timestamp">时间标识</param>
    /// <param name="partition">最新项的列</param>
    /// <returns></returns>
    public static ISelect<T1> LatestOn<T1, TKey>(this ISelect<T1> select, Expression<Func<T1, DateTime?>> timestamp,
        Expression<Func<T1, TKey>> partition)
    {
        Provider(timestamp, partition);
        return select;
    }

    private static void Provider<T1, TKey>(Expression<Func<T1, DateTime?>> timestamp,
        Expression<Func<T1, TKey>> partition)
    {
        IsExistence.Value = true;
        var latestOnTemple = $"{Environment.NewLine}LATEST ON {{0}} PARTITION BY {{1}} ";
        var expressionVisitor = new QuestDbExpressionVisitor();
        expressionVisitor.Visit(timestamp);
        var _timestamp = expressionVisitor.Fields();
        expressionVisitor.Visit(partition);
        var _partition = expressionVisitor.Fields();
        LatestOnString.Value = string.Format(latestOnTemple, _timestamp, _partition);
    }

    /// <summary>
    /// 对于多个时间序列存储在同一个表中的场景，根据时间戳检索给定键或键组合的最新项。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="select"></param>
    /// <param name="timestamp">时间标识</param>
    /// <param name="partition">最新项的列</param>
    /// <returns></returns>
    public static ISelect<T1, T2> LatestOn<T1, T2, TKey>(this ISelect<T1, T2> select,
        Expression<Func<T1, DateTime?>> timestamp,
        Expression<Func<T1, TKey>> partition) where T2 : class
    {
        Provider(timestamp, partition);
        return select;
    }

    /// <summary>
    /// 对于多个时间序列存储在同一个表中的场景，根据时间戳检索给定键或键组合的最新项。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="select"></param>
    /// <param name="timestamp">时间标识</param>
    /// <param name="partition">最新项的列</param>
    /// <returns></returns>
    public static ISelect<T1, T2, T3> LatestOn<T1, T2, T3, TKey>(this ISelect<T1, T2, T3> select,
        Expression<Func<T1, DateTime?>> timestamp,
        Expression<Func<T1, TKey>> partition) where T2 : class where T3 : class
    {
        Provider(timestamp, partition);
        return select;
    }

    /// <summary>
    /// 对于多个时间序列存储在同一个表中的场景，根据时间戳检索给定键或键组合的最新项。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="select"></param>
    /// <param name="timestamp">时间标识</param>
    /// <param name="partition">最新项的列</param>
    /// <returns></returns>
    public static ISelect<T1, T2, T3, T4> LatestOn<T1, T2, T3, T4, TKey>(this ISelect<T1, T2, T3, T4> select,
        Expression<Func<T1, DateTime?>> timestamp,
        Expression<Func<T1, TKey>> partition) where T2 : class where T3 : class where T4 : class
    {
        Provider(timestamp, partition);
        return select;
    }
}

public static class RestAPIExtension
{
    internal static string BaseUrl = string.Empty;
    internal static string authorization = string.Empty;

    internal static async Task<string> ExecAsync(string sql)
    {
        var result = string.Empty;
        var client = QuestDbContainer.GetService<IHttpClientFactory>().CreateClient();
        var url = $"{BaseUrl}/exec?query={HttpUtility.UrlEncode(sql)}";
        if (!string.IsNullOrWhiteSpace(authorization))
            client.DefaultRequestHeaders.Add("Authorization", authorization);
        var httpResponseMessage = await client.GetAsync(url);
        result = await httpResponseMessage.Content.ReadAsStringAsync();

        return result;
    }

    public static FreeSqlBuilder UseQuestDbRestAPI(this FreeSqlBuilder buider, string host, string username = "",
        string password = "")
    {
        BaseUrl = host;
        if (BaseUrl.EndsWith("/"))
            BaseUrl = BaseUrl.Remove(BaseUrl.Length - 1);

        if (!BaseUrl.ToLower().StartsWith("http"))
            BaseUrl = $"http://{BaseUrl}";
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            authorization = $"Basic {base64}";
        }

        return buider;
    }
}