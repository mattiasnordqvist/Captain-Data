# Captain-Data (v2.0)
I'm sick of generating database data for integrations tests and migration seeds by hand. Most data I don't even care about but it is enforced by the schema. Let's create some sensible convention based tool for auto-generating sql server data.

Migrating from pre v2.0? Scroll down to end of ReadMe

## Examples

Insert a row into a table:

```csharp
var captain = new Captain();
captain.Insert("Person").Go(anOpenConnection);
```

Captain Data will work out default for all columns of the table Person and insert a row into that table.

If you want some specific data inserted for some column, you can do this:

```csharp
var captain = new Captain();
captain.Insert("Person", new { Name = "Captain Data"}).Go(anOpenConnection);
```

Want to know the Id of your newly inserted row (provided its table has an identity column)?

```csharp
var id = captain.Context.ScopeIdentity;
```

You can also provide your own defaults in a fashion that will apply to all inserts created by the same Captain.

```csharp
var captain = new Captain();
captain.AddRule(new MyFineRule());
```

Your rule should implement `IRule`. Instead of explaining exactly how it works right now (this part of CaptainData is being refactored), I'll just provide an example.  

Let's say you want to make sure Captain Data inserts '10' in all integer columns. Define a rule for that:

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

In order to resolve the value to be inserted into a column, three steps are taken.  
1. See if there is a specified override in the insert statement, if not go on to step 2.  
2. Apply custom rules (like IntegersRule). If the column still has no value, go to step 3.  
3. Apply Captain Data defaults  

## Built-in rules

### UseDatabaseDefaultRule
*needs doc*

### SmartIdInsertRule
*needs doc*

## Modify behaviour of defaults and overrides
*needs doc*

# Migrating from pre v2.0 to v2.0
* `RuleSet`s are gone. Captain constructor taking `RuleSet`s is gone. Add `IRule`s one by one through `Captain.AddRule` instead.
* `CaptainContext` now subclasses `Dictionary<string, object>` to suit all your global context needs.
* `IdentityInsertRule` is gone. Identity Insert is now automatically turned on if an identity column has a value set by Captain Data.
* The `rowInstruction.Before` and `After` has been removed. Do you need them? Hit me up with a GitHub Issue and we can solve it together.
