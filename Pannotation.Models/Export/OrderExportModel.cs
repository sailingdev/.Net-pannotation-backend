using System.Collections.Generic;

namespace Pannotation.Models.Export
{
    public class OrderExportModel
    {
        public static string[] Headers = { "Order ID", "Buyer name", "Order date", "Order amount", "Order status", "Last four digits of CC", "CC type", "Music Scores" };

        public int Id { get; set; }

        public string BuyerName { get; set; }

        public string CreatedAt { get; set; }

        public decimal Amount { get; set; }

        public string OrderStatus { get; set; }

        public string CardMask { get; set; }

        public string CardType { get; set; }

        public List<SongsheetExportModel> Songsheets { get; set; }
    }
}
