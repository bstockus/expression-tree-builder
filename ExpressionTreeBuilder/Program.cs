using ExpressionTreeBuilder;
using Newtonsoft.Json;
using static ExpressionTreeBuilder.AttributeFilters;


var storeInfos = JsonConvert.DeserializeObject<IEnumerable<StoreInfo>>(
    await File.ReadAllTextAsync(@"/Users/bs10074521/Downloads/storemaster.json"));

var nodes =
    NotBlock(
        OrBlock(
            AndBlock(
                OrBlock(
                    StateConditional("WI"),
                    StateConditional("MN")),
                AttributeConditional("HasHotFood")),
            AndBlock(
                StateConditional("IA"),
                AttributeConditional("Store Type: SNG")),
            StoreConditional("117")));

var results = storeInfos.Where(
    x => nodes.BuildLambda().Invoke(x))
    .ToList();

Console.WriteLine(results.Aggregate("", (s, info) => s + $" [{info.Number}]"));
Console.WriteLine($"Count = {results.Count()}");