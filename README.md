# Captain-Data
I'm sick of generating database data for integrations tests by hand. Most data I don't even care about but it is enforced by the schema. Let's create some sensible convention based tool for auto-generating sql server data.

## Examples

Insert a row into a table:

```csharp
var captain = new Captain();
captain.Insert("Person").Go(anOpenConnection);
```

Captain Data will work out default for all columns of the table Person and insert a row into that table.

Captain Data is still in some kind of alpha, so defaults for every possible type of column is not implemented yet.
However, since there is a possibility to override any inserted column data, you can always work around this.

```csharp
var captain = new Captain();
captain.Insert("Person", new { Name = "Captain Data"}).Go(anOpenConnection);
```

Oh, want to know the Id of your newly inserted row (provided its table has an identity column)?

```csharp
var id = captain.Context.ScopeIdentity;
```

You can also provide your own defaults in a fashion that will apply to all inserts created by the same Captain.

```csharp
var captain = new Captain(new MyRules());
```

MyRules should inherit from RuleSet. Instead of explaining exactly how it works right now (because it is in the middle of the night), I'll 
just provide some examples. There is a standard implementation of RuleSet, called BasicRuleSet. Begin by inheriting it and create a constructor.

```csharp
public class MyRules : BasicRuleSet
{
	public MyRules 
	{
	}
}
``` 

In this constructor you can add Rules. Rules must implement IRule. Let's say you want to make sure Captain Data inserts '10' in all integer columns. Define a rule for that:

```csharp
public class IntegersRule : SingleColumnValueRule
{
	public override bool Match(ColumnSchema column, InstructionContext instructionContext)
	{
		return column.DataType == SqlDataType.Int;
	}
	public override object Value(ColumnSchema column, InstructionContext instructionContext)
	{
		return 10;
	}
} 
```

(SingleColumnValueRule is an abstract implementation of IRule)

Then, add this rule to MyRules
```csharp
public class MyRules : BasicRuleSet
{
	public MyRules 
	{
		AddRule(new IntegersRule());
	}
}
``` 

In order to resolve the value to be inserted into a column, three steps are taken.  
1. See if there is a specified override in the insert statement, if not go on to step 2.  
2. Apply custom rules (like IntegersRule). If the column still has no value, go to step 3.  
3. Apply Captain Data defaults  

One of the default rules of Captain Data is to not insert anything on an IDENTITY-column. Sometimes, in testing, to make things deterministic, you want to override the default IDENTITY-behaviour. In SQL Server, you can temporarily turn off IDENTITY INSERT. 
Captain data comes with a rule that can handle this for you.

```csharp
public class MyRules : BasicRuleSet
{
	public MyRules 
	{
       		AddRule(new AllowIdentityInsertRule());
	}
}
``` 

And now you can do this without any exceptions:

```csharp
var captain = new Captain();
captain.Insert("Person", new { Id = 1 }).Go(anOpenConnection);
```
