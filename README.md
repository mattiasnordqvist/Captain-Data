# Captain-Data
I'm sick of generating database data for integrations tests by hand. Most data I don't even care about but it is enforced by the schema. Let's create some sensible convention based tool for auto-generating data.

## Idea
Sometimes I have to set up a database with some data to do some testing, because thanks to IQueryable and stuff, a lot of business is going on in the database. So my tests are littered with fakes and setups and fixtures and stuff to basically do stuff like `INSERT INTO Todo (Name, Due) VALUES ('blabla', '2017-01-01')`. In this example. suppose all I want to do is to see that my Todo-items-count is correct. I don't really care what the name or the due date of the Todo is. I just need a bunch of Todos. However, my db-schema says these columns are `NOT NULL`, so they have to be provided. This creates extra work, right?

I'm thinking that we, in runtime, could look at what the database schema look like, and create sensible defaults for values in most columns that I don't really care about. If I could just say `Enumerable.Range(1,10).ForEach(x => Captain.Insert("Todo"))`, that would be nice wouldn't it? This could insert `''` in non-nullable string columns, and maybe `GETDATE()` in non-nullable datetime columns.

We could let the user override defaults I think as well, by providing functions somehow. Let's say I want to test that sorting of the Todos works. Then `GETDATE()` as `Due` is not good enough for me. How about something like  `Enumerable.Range(1,10).ForEach(x => Captain.Insert("Todo", y => y.Value = (y.Name == "Due" ? new DateTime(2015, 1, x) : y.Value)))`. Syntactic sugar for this could be  `Enumerable.Range(1,10).ForEach(x => Captain.Insert("Todo", new Override { Name = "Due", Value = new DateTime(2015, 1, x) })` maybe.

If we could make up some good defaults for simple foreign key constraints as well, that will create rows in other tables if needed, this would become super powerful, I think.
