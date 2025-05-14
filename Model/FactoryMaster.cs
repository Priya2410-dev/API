namespace Calyx_Solutions.Model
{
    public class FactoryMaster
    {
        public static IMaster getCustomer()
        {
            clsMaster objCust = new clsMaster();
            return (IMaster)objCust;
        }
    }
}
 
