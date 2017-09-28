# json-logic-cs

This parser accepts [JsonLogic](http://jsonlogic.com) rules and executes them on the .Net Platform.

This is a port of the GitHub project by [jwadhams](https://github.com/jwadhams) for JavaScript: [json-logic-js](https://github.com/jwadhams/json-logic-js).

Although I’ve been able to port most of the functionality of the original JavaScript parser to C# and the .Net Framework, migrating to .Net does present some interesting challenges, and I’ve noted them and the compatibility issues below.

The JsonLogic format is designed to allow you to share rules (logic) between front-end and back-end code (regardless of language difference), even to store logic along with a record in a database.  JsonLogic is documented extensively at [JsonLogic.com](http://jsonlogic.com), including examples of every [supported operation](http://jsonlogic.com/operations.html) and a place to [try out rules in your browser](http://jsonlogic.com/play.html).

The same format can also be executed in PHP by the library [json-logic-php](https://github.com/jwadhams/json-logic-php/)

## Approach


## Examples

### Simple
```cs
using JsonLogicCSharp;

bool b = JsonLogic( "{ '==' : [1, 1] }" );

// true
```

This is a simple test, equivalent to `1 == 1`.  A few things about the format:

  1. The operator is always in the "key" position. There is only one key per JsonLogic rule.
  1. The values are typically an array.
  1. Each value can be a string, number, boolean, array (non-associative), or null

### Compound
Here we're beginning to nest rules. 

```cs
bool b = JsonLogic(
  @"{'and' :
      [
        { '>' : [3,1] },
        { '<' : [1,3] }
      ] }"
);

// true
```
  
In an infix language (like C#) this could be written as:

```cs
( (3 > 1) && (1 < 3) )
```
    
### Data-Driven

Obviously these rules aren't very interesting if they can only take static literal data. Typically `jsonLogic` will be called with a rule object and a data object. You can use the `var` operator to get attributes of the data object:

```cs
int i = JsonLogic(
            "{ 'var' : ['a'] }", 
            "{ a : 1, b : 2 }"
            );

// 1
```

If you like, we support [syntactic sugar](https://en.wikipedia.org/wiki/Syntactic_sugar) on unary operators to skip the array around values:

```cs
int i = JsonLogic(
  "{ 'var' : "a" }",
  "{ a : 1, b : 2 }"
);

// 1
```

You can also use the `var` operator to access an array by numeric index:

```cs
string s = JsonLogic(
  "{'var' : 1 }",
  "[ 'apple', 'banana', 'carrot' ]"
);

// "banana"
```

Here's a complex rule that mixes literals and data. The pie isn't ready to eat unless it's cooler than 110 degrees, *and* filled with apples.

```cs
string logic =
  @"{ 'and' : [
      {'<' : [ { 'var' : 'temp' }, 110 ]},
      {'==' : [ { 'var' : 'pie.filling' }, 'apple' ] }
    ] }";

string data = "{ 'temp' : 100, 'pie' : { 'filling' : 'apple' } }";

bool b = JsonLogic(logic, data);

// true
```
Note that you can also use double quotes in JSON. However, double-quotes must be escaped as follows:
```cs

string logic = "{\"+\" :[2, 3]}";
string data = null;

int i = JsonLogic(logic, data);

// 5
