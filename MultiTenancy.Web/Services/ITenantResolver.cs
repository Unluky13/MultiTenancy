namespace MultiTenancy.Web.Data
{
    public interface ITenantResolver
    {
        string Resolve();
    }
}
