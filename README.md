UniqueNamespace.SqlBuilder
==========================

A blatant ripoff of Dapper's SqlBuilder. Thanks Dapper!

## Why?

We're using micro-ORMs because we don't like all of the complexity and bloat of "macro-ORMs." But we lose the ability to compose queries using LINQ.

This project aims to solve that by taking the SqlBuilder from Dapper and making a few adjustments.

#### What's wrong with Dapper's SqlBuilder?

- It only works with Dapper's `DynamicParameters` type.
- It's distributed only as a source code.
- I really hated typing `/**WHATEVER**/` for placeholders.


## Features

- No SQL generation. Your SQL is just that.
- Parameter build-up.
- Works with any SQL database
- Not tied to any specific ORM, but does have out-of-box support for:
    - ADO.NET
    - Dapper
    - Insight.Database (my personal favorite)
- Prebuilt SQL templates to save you some time.

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
## Sort Expressions

Use the `SortExpression` class to help with dynamic ordering:

```c#

// implicit conversion from string
SortExpression sort = "[My Column] desc";
string name = sort.Name; // "[My Column]"
bool descending = sort.Descending; // true

// implicit conversion to string
string s = sort; // "[My Column] DESC"

// parse multiples
SortExpression[] sorts = SortExpression.Parse("This, That, Other desc");
string orderBy = sorts.Join(); // "This ASC, That ASC, Other DESC"

// disallows sorting from client that you don't want
// excludes "Other" from above
Enumerable allowed = sorts.WhiteList("This", "That"); 

// disallows unsafe column names
SortExpression good = "[My Column]";
SortExpression bad = "My Column"; // throws InvalidSortExpressionNameException
bad = "MyColumn OR 1=1; DELETE Users"; // throws InvalidSortExpressionNameException

// value equality
var count = SortExpression.Parse("Name, [Name]").Distinct().Count(); // 1

// pass to builder
var b = new SqlBuilder();
b.OrderBy(sorts);

```