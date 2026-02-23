namespace EmberOps.BuildingBlocks.Persistance.SqlServer.Intercerptors.Interfaces
{
    //Interface for define the audit fields on DB
    public interface IAuditable
    {
        DateTime CreatedAtUtc { get; set; }
        DateTime UpdatedAtUtc { get; set; }
    }
}
