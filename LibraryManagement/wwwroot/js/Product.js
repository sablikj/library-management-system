function Buy(productId, urlAction, outElementId, locale, quantity) {
    $.ajax({
        type: "POST",
        url: urlAction,
        data: {
            productId: productId,
            quantity: quantity
        },
        dataType: "text",
        success: function (totalPrice) {
            ChangeTotalPriceInformation(outElementId, totalPrice, locale, quantity);
        },
        error: function (req, status, error) {
            $(outElementId).text('error during buying!');
        }
    });
}

function ChangeTotalPriceInformation(outElementId, totalPrice, locale) {
    $(outElementId).text(parseFloat(totalPrice).toLocaleString(locale,
        {
            style: "currency",
            currency: "CZK",
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }));
}