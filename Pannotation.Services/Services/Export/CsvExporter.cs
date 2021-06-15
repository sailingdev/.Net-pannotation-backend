using Pannotation.Models.Export;
using Pannotation.Services.Interfaces.Export;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pannotation.Common.Extensions;

namespace Pannotation.Services.Services.Export
{
    public class CsvExporter : IExporter
    {
        public byte[] Export<T>(T data)
        {
            if (typeof(T) == typeof(List<OrderExportModel>))
                return ExportOrders(data as List<OrderExportModel>);
            else if (typeof(T) == typeof(List<SubscriptionExportModel>))
                return ExportSubscriptions(data as List<SubscriptionExportModel>);

            return null;
        }

        private byte[] ExportOrders(List<OrderExportModel> data)
        {
            var result = new StringBuilder();
            result.AppendLine(string.Join(", ", OrderExportModel.Headers));

            foreach (var order in data)
            {
                var songsheetsRow = new StringBuilder();
                foreach (var songsheet in order.Songsheets)
                {
                    StringBuilder genresRow = null;
                    if (songsheet.Genres != null && songsheet.Genres.Any())
                    {
                        genresRow = new StringBuilder();
                        foreach (var genre in songsheet.Genres)
                            genresRow.Append($"- {genre}\n");
                    }

                    StringBuilder instrumentsRow = null;
                    if (songsheet.Instruments != null && songsheet.Instruments.Any())
                    {
                        instrumentsRow = new StringBuilder();
                        foreach (var instrument in songsheet.Instruments)
                            instrumentsRow.Append($"- {instrument}\n");
                    }

                    songsheetsRow.Append($"ID: {songsheet.Id}\n" +
                        $"Name: {songsheet.Name}\n" +
                        $"Price: {songsheet.Price}\n" +
                        $"Arranger: {songsheet.Arranger ?? "-"}\n" +
                        $"ArtistName: {songsheet.ArtistName ?? "-"}\n" +
                        $"Created at: {songsheet.CreatedAt.ToString("dd-MM-yyyy")}\n" +
                        $"Description: {songsheet.Description ?? "-"}\n" +
                        $"Composer: {songsheet.Producer ?? "-"}\n" +
                        $"Youtube: {songsheet.YouTubeLink ?? "-"}\n");

                    songsheetsRow.Append(genresRow != null ? ("Genres:\n" + genresRow) : "");
                    songsheetsRow.Append(instrumentsRow != null ? ("Instruments:\n" + instrumentsRow + "\n") : "\n");
                }

                songsheetsRow = songsheetsRow.NormalizeOrderInfo();
                songsheetsRow.Insert(0, "\"");
                songsheetsRow.Append("\"");

                result.AppendLine($"\"{order.Id}\",\"{order.BuyerName.NormalizeCsvString()}\",\"{order.CreatedAt}\",\"{order.Amount}\",\"{order.OrderStatus}\",\"{order.CardMask}\",\"{order.CardType}\",{songsheetsRow}");
            }

            var resultAsByteArray = Encoding.UTF8.GetBytes(result.ToString());

            return resultAsByteArray;
        }

        private byte[] ExportSubscriptions(List<SubscriptionExportModel> data)
        {
            var result = new StringBuilder();
            result.AppendLine(string.Join(", ", SubscriptionExportModel.Headers));

            foreach (var subscription in data)
                result.AppendLine($"\"{subscription.Id}\",\"{subscription.BuyerName.NormalizeCsvString()}\",\"{subscription.PurchasedAt}\",\"{subscription.ExpiredAt}\",\"{subscription.Amount}\",\"{subscription.PurchaseStatus}\",\"{subscription.CardMask}\",\"{subscription.CardType}\",\"{subscription.SubscriptionStatus}\"");

            var resultAsByteArray = Encoding.UTF8.GetBytes(result.ToString());
            return resultAsByteArray;
        }
    }
}
