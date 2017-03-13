
var StripePayCheckout = function (data) {

    //Declarations
    var handler = null;


    //Exposed Functions
    this.initialize = function () {
        handler = StripeCheckout.configure({
            key: data.publisherKey,
            image: 'https://stripe.com/img/documentation/checkout/marketplace.png',
            locale: 'auto',
            token: function (token) {
                // Get the token ID to your server-side code for use.

                setTimeout(function () { $(data.targetButton).addClass("disabled"); }, 200);

                var cUrl = data.actionUrl + '?tokenId=' + token.id;

                $.post(cUrl, function (resp) {

                    if (resp == "succeed") { data.handleSuccess(); }
                    else { data.handleFailure(); }

                    $(data.targetButton).removeClass("disabled");
                });
            }
        });

        $(data.targetButton).click(function (e) {
            if (!$(data.targetButton).hasClass("disabled")) {
                // Open Checkout with further options:
                $(data.targetButton).addClass("disabled");
                handler.open({
                    name: 'Swiftkare LLC',
                    description: 'Some description about Swiftkare LLC',
                    amount: data.amount,
                    closed: function () {
                        $(data.targetButton).removeClass("disabled");
                    }
                });
            }
            e.preventDefault();
        });

        // Close Checkout on page navigation:
        window.addEventListener('popstate', function () {
            handler.close();
        });
    }


}