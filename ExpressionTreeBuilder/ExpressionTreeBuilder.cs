using System.Linq.Expressions;

namespace ExpressionTreeBuilder;

public enum AttributeFilterNodeType {
    AndBlock,
    OrBlock,
    NotBlock,
    StateConditional,
    AttributeConditional,
    StoreConditional,
    DivisionConditional,
    ZoneConditional,
    DistrictConditional,
    FoodServiceDistrictNumberConditional,
    FoodServiceZoneNumberConditional,
    AllStoresConditional
};

public static class AttributeFilters
{
    public static AndBlockAttributeFilterNode AndBlock(params AttributeFilterNode[] children) => new(children);
    public static OrBlockAttributeFilterNode OrBlock(params AttributeFilterNode[] children) => new(children);
    public static NotBlockAttributeFilterNode NotBlock(AttributeFilterNode child) => new(child);
    
    
    public static StateConditionalAttributeFilterNode StateConditional(string state) => new(state);
    public static AttributeConditionalAttributeFilterNode AttributeConditional(string attribute) => new(attribute);
    public static StoreConditionalAttributeFilterNode StoreConditional(string store) => new(store);
    public static DivisionConditionalAttributeFilterNode DivisionConditional(int divisionNumber) => new(divisionNumber);
    public static ZoneConditionalAttributeFilterNode ZoneConditional(int zoneNumber) => new(zoneNumber);
    public static DistrictConditionalAttributeFilterNode DistrictConditional(int districtNumber) => new(districtNumber);
    public static FoodServiceDistrictNumberConditionalAttributeFilterNode FoodServiceDistrictNumberConditional(
        int foodServiceDistrictNumber) =>
        new(foodServiceDistrictNumber);
    public static FoodServiceZoneNumberConditionalAttributeFilterNode FoodServiceZoneNumberConditional(
        int foodServiceZoneNumber) =>
        new(foodServiceZoneNumber);
    public static AllStoresConditionalAttributeFilterNode AllStoresConditional() => new();
}

public record AttributeFilterExpressionTreeContext(
    ParameterExpression StoreInfoParameterExpression);

public abstract record AttributeFilterNode
{
    public abstract Expression BuildExpressionTree(AttributeFilterExpressionTreeContext context);
}

public abstract record BlockAttributeFilterNode(
    IEnumerable<AttributeFilterNode> Children) : AttributeFilterNode
{
    protected abstract Expression BuildBlockExpressionNode(
        Expression leftExpression,
        Expression rightExpression);

    public override Expression BuildExpressionTree(AttributeFilterExpressionTreeContext context)
    {
        if (Children.Count() == 1)
        {
            return Children.First().BuildExpressionTree(context);
        }

        var children = Children.ToList();

        var expressionValue = BuildBlockExpressionNode(
            children[0].BuildExpressionTree(context),
            children[1].BuildExpressionTree(context));

        var remainingChildren = children.Skip(2);

        if (!remainingChildren.Any())
        {
            return expressionValue;
        }

        while (remainingChildren.Any())
        {
            expressionValue =
                BuildBlockExpressionNode(
                    expressionValue,
                    remainingChildren.First().BuildExpressionTree(context));

            remainingChildren = remainingChildren.Skip(1);
        }

        return expressionValue;
    }
}

public record NotBlockAttributeFilterNode(
    AttributeFilterNode Child) : AttributeFilterNode
{
    public override Expression BuildExpressionTree(
        AttributeFilterExpressionTreeContext context) =>
        Expression.Equal(
            Child.BuildExpressionTree(context),
            Expression.Constant(
                false,
                typeof(bool)));
}

public record AndBlockAttributeFilterNode(
    IEnumerable<AttributeFilterNode> Children) : BlockAttributeFilterNode(Children)
{
    protected override Expression BuildBlockExpressionNode(
        Expression leftExpression,
        Expression rightExpression) =>
        Expression.AndAlso(leftExpression, rightExpression);
}

public record OrBlockAttributeFilterNode(
    IEnumerable<AttributeFilterNode> Children) : BlockAttributeFilterNode(Children)
{
    protected override Expression BuildBlockExpressionNode(
        Expression leftExpression,
        Expression rightExpression) =>
        Expression.OrElse(leftExpression, rightExpression);
}

public abstract record ConditionalAttributeFilterNode : AttributeFilterNode;

public record AllStoresConditionalAttributeFilterNode : ConditionalAttributeFilterNode
{
    public override Expression BuildExpressionTree(
        AttributeFilterExpressionTreeContext context) =>
        Expression.Constant(
            true,
            typeof(bool));
}

public record AttributeConditionalAttributeFilterNode(
    string Attribute) : ConditionalAttributeFilterNode
{
    public override Expression BuildExpressionTree(
        AttributeFilterExpressionTreeContext context) =>
        Expression.Call(
            typeof(StoreInfoExtensions)
                .GetMethods()
                .FirstOrDefault(x => x.Name.Equals(StoreInfoExtensions.ContainsAttributeMethodName)) ??
            throw new Exception($"StoreInfoExtensions.{StoreInfoExtensions.ContainsAttributeMethodName}() method not found."),
            context.StoreInfoParameterExpression,
            Expression.Constant(
                Attribute,
                typeof(string)));
}

public abstract record ValueComparisonConditionalAttributeFilterNode<TValue>(TValue Value, string FieldName) :
    ConditionalAttributeFilterNode
{
    
    public override Expression BuildExpressionTree(
        AttributeFilterExpressionTreeContext context) =>
        Expression.Equal(
            Expression.PropertyOrField(
                context.StoreInfoParameterExpression,
                FieldName),
            Expression.Constant(
                Value,
                typeof(TValue)));
}

public record DivisionConditionalAttributeFilterNode(
    int DivisionNumber) : ValueComparisonConditionalAttributeFilterNode<int>(DivisionNumber, "DivisionNumber");

public record ZoneConditionalAttributeFilterNode(
    int ZoneNumber) : ValueComparisonConditionalAttributeFilterNode<int>(ZoneNumber, "ZoneNumber");

public record DistrictConditionalAttributeFilterNode(
    int DistrictNumber) : ValueComparisonConditionalAttributeFilterNode<int>(DistrictNumber, "DistrictNumber");

public record FoodServiceDistrictNumberConditionalAttributeFilterNode(
    int FoodServiceDistrictNumber) : ValueComparisonConditionalAttributeFilterNode<int>(FoodServiceDistrictNumber,
    "FoodServiceDistrictNumber");

public record FoodServiceZoneNumberConditionalAttributeFilterNode(
    int FoodServiceZoneNumber) : ValueComparisonConditionalAttributeFilterNode<int>(FoodServiceZoneNumber,
    "FoodServiceZoneNumber");

public record StateConditionalAttributeFilterNode(
    string State) : ValueComparisonConditionalAttributeFilterNode<string>(State, "State");

public record StoreConditionalAttributeFilterNode(
    string Store) : ValueComparisonConditionalAttributeFilterNode<string>(Store, "Number");

public static class ExpressionTreeBuilder
{

    public static Func<StoreInfo, bool> BuildLambda(
        this AttributeFilterNode rootNode)
    {
        var parameterExpression = Expression.Parameter(
            typeof(StoreInfo),
            "storeInfo");

        var expression = rootNode.BuildExpressionTree(
            new AttributeFilterExpressionTreeContext(
                parameterExpression));
        
        var lambdaExpression = Expression.Lambda<Func<StoreInfo, bool>>(
            expression,
            false, 
            parameterExpression);

        return lambdaExpression.Compile();

    }

}