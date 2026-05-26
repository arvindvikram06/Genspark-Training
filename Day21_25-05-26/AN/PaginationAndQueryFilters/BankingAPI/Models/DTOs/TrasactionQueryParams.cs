namespace BankingAPI.Models.DTOs
{
    public class TransactionQueryParams
    {
        public string AccountNumber { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public DateTime? FromDate {get; set;}

        public DateTime? ToDate {get; set;}

        public decimal? MinAmount {get; set;}

        public decimal? MaxAmount {get; set;}

        public string SortBy{get; set;} = "date";

        public string SortOrder {get; set;} = "desc";

    }
}