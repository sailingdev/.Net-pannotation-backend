using System;

namespace Pannotation.Models.Export
{
    public class SubscriptionExportModel
    {
        public static string[] Headers = { "Subscription ID", "Name", "Purchase date", "Expiring date", "Subscription amount", "Purchase status", "Last four digits of CC", "Card type", "Subscription status" };

        public int Id { get; set; }

        public string BuyerName { get; set; }

        public string PurchasedAt { get; set; }

        public string ExpiredAt { get; set; }

        public decimal? Amount { get; set; }

        public string PurchaseStatus { get; set; }

        public string CardMask { get; set; }

        public string CardType { get; set; }

        public string SubscriptionStatus { get; set; }
    }
}
