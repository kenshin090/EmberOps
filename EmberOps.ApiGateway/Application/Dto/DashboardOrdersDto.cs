namespace EmberOps.ApiGateway.Application.Dto
{
    public class DashboardOrdersDto
    {
        public int OrdersInDraft { get; set; }
        public int OrdersSubmitted { get; set; }
        public int OrdersPaid { get; set; }
        public int OrdersCancelled { get; set; }
       
    }
}
