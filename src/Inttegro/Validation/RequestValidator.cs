using Inttegro.Money;

namespace Inttegro.Validation;

public static class RequestValidator
{
    public static void Require(object? value, string name)
    {
        if (value == null)
        {
            throw new ArgumentException($"'{name}' is required.", name);
        }

        if (value is string str && string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentException($"'{name}' is required.", name);
        }
    }

    public static void RequireCollection<T>(ICollection<T>? value, string name)
    {
        if (value == null || value.Count == 0)
        {
            throw new ArgumentException($"'{name}' is required.", name);
        }
    }

    public static void RequireAny((string Name, object? Value) first, (string Name, object? Value) second, string message)
    {
        var firstPresent = IsPresent(first.Value);
        var secondPresent = IsPresent(second.Value);
        if (!firstPresent && !secondPresent)
        {
            throw new ArgumentException(message);
        }
    }

    public static void ValidatePriceDefinition(
        PriceType? type,
        AmountParams? fixedAmount,
        CustomerSelectedAmountParams? selectedAmount,
        string? productId
    )
    {
        var fixedPrice = type == PriceType.FixedAmount && fixedAmount != null && selectedAmount == null;
        var selected = type == PriceType.CustomerSelectedAmount && selectedAmount != null &&
            fixedAmount == null && !string.IsNullOrWhiteSpace(productId);
        if ((fixedPrice ? 1 : 0) + (selected ? 1 : 0) != 1)
        {
            throw new ArgumentException("Provide exactly one valid price definition; customer-selected prices require a product_id.");
        }
    }

    public static void ValidateCustomerSelectedProduct(ProductDetailsParams? product)
    {
        if (product?.CustomerSelectedPrice == null)
        {
            return;
        }

        Require(product.ProductId, "product_id");
        Require(product.Quantity, "quantity");
        Require(product.CustomerSelectedPrice.PriceId, "customer_selected_price.price_id");
        Require(product.CustomerSelectedPrice.SelectedAmount, "customer_selected_price.selected_amount");
        var mixedChoice = product.Price != null || product.PriceId != null;
        var inlineFields = product.Id != null || product.Type != null || product.Name != null || product.About != null ||
            product.Reference != null || product.TaxCode != null || product.CustomData != null;
        if (mixedChoice || inlineFields || product.Quantity.GetValueOrDefault() < 1)
        {
            throw new ArgumentException("customer_selected_price is valid only for a catalog product and cannot be combined with price or price_id.");
        }
    }

    private static bool IsPresent(object? value)
    {
        if (value == null)
        {
            return false;
        }

        if (value is string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        return true;
    }
}
