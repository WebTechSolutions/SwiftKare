using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RestAPIs.Helper
{
    public class StripePayHelper
    {
        #region Declarations

        private static string StripeApiKey = ConfigurationManager.AppSettings["StripePaySecretKey"].ToString();

        #endregion

        #region Methods

        /// <summary>
        /// Deduct money from client's credit card
        /// </summary>
        /// <param name="tokenId">Enter Token ID</param>
        /// <param name="amount">Enter Amount in cents</param>
        /// <returns></returns>
        public static bool PerformStripeCharge(string tokenId, int amount, string description)
        {
            try
            {
                var oneTimeCharge = new StripeChargeCreateOptions();

                // always set these properties
                oneTimeCharge.Amount = amount;
                oneTimeCharge.Currency = "usd";

                // set this if you want to
                oneTimeCharge.Description = description;

                oneTimeCharge.SourceTokenOrExistingSourceId = tokenId;

                var chargeService = new StripeChargeService(StripeApiKey);
                StripeCharge stripeCharge = chargeService.Create(oneTimeCharge);

                return true;
            }
            catch (Exception ex)
            { return false; }
        }

        #endregion

    }

}
