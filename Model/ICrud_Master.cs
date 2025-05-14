namespace Calyx_Solutions.Model
{
    public interface ICrud_Master
    {
        object InsertToDatabase();
        object UpdateToDatabase();
        object ReadFromDatabase();
        object DeleteFromDatabase();
        object Check_Exists();
        object ReadSingleValueFromDatabase();


        string OS_Type { get; set; }
        string Is_Procedure { get; set; }
        string Operation_Name { get; set; }
    }
}
