UniqueNamespace.SqlBuilder
==========================

A blatant ripoff of Dapper's SqlBuilder. Thanks Dapper!

## Why?

We're using micro-ORMs because we don't like all of the complexity and bloat of "macro-ORMs." But we lose the ability to compose queries using LINQ.

This project aims to solve that by taking the SqlBuilder from Dapper and making a few adjustments.

## Features

- No SQL generation. Your SQL is just that.
- Parameter build-up.
- Works with any SQL database
- Not tied to any specific ORM, but does have out-of-box support for:
    - ADO.NET
    - Dapper
    - Insight.Database (my personal favorite)

## Installation

PM> Install-Package UniqueNamespace.SqlBuilder

PM> Install-Package UniqueNamespace.SqlBuilder.Dapper

PM> Install-Package UniqueNamespace.SqlBuilder.Insight

## Example

This code sample uses `Insight.Database` and the `UniqueNamespace.SqlBuilder.Insight` package.

```c#

var builder = new SqlBuilder();
var query = builder.AddTemplate("SELECT Id, This, That, TheOther" +
                                "FROM MyTable {{WHERE}} {{ORDERBY}}" +
                                "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY",
    new { skip = 10, take = 25 });

var count = builder.AddTemplate("SELECT Count(*) FROM MyTable {{WHERE}}");

if (userQuery.This != null)
    builder.Where("This = @This", new { userQuery.This });

if (userQuery.That != null)
    builder.Where("That = @That", new { userQuery.That });

if (userQuery.SortExpressions != null)
    foreach (var sort in userQuery.SortExpressions)
        builder.OrderBy(sort);

// using Insight.Database (MY FAVORITE!)
var results = connection.QuerySql<ResultModel>(query.RawSql, query.Parameters);
var totalRows = connection.SingleSql<int>(count.RawSql, count.Parameters);

```

Here's the SQL being executed:

```SQL

SELECT Id, This, That, TheOther
FROM MyTable
WHERE This = @This AND That = @That
ORDER BY This, That DESC
OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY

SELECT Count(*)
FROM MyTable
WHERE This = @This AND That = @That

```

`UniqueNamespace.SqlBuilder.Dapper` works much the same way. Here's how it would work with old-school ADO.NET:

```c#

var builder = new SqlBuilder<SqlParameter>();
var query = builder.AddTemplate("SELECT Id, This, That, TheOther " +
                                "FROM MyTable {{WHERE}} {{ORDERBY}}" +
                                "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY",
    new SqlParameter("@skip", 10),
    new SqlParameter("@take", 10));

if (userQuery.This != null)
    builder.Where("This = @This", new SqlParameter("@This", userQuery.This));

if (userQuery.That != null)
    builder.Where("That = @That", new SqlParameter("@That", userQuery.That));

if (userQuery.SortExpressions != null)
    foreach (var sort in userQuery.SortExpressions)
        builder.OrderBy(sort);

using (var cmd = connection.CreateCommand())
{
    cmd.CommandText = query.RawSql;
    cmd.Parameters.AddRange(query.Parameters);
    // open connection; execute command; etc..
}

```