namespace Domain.Static
{
    public static class MagicStrings
    {
       public static string StageConnectionStrings = "Stage";
       public static string ProductionConnectionStrings = "Production";
       public static string DevelopmentConnectionStrings = "Development";
       public static string[] ProductionOrigins = { "https://balancer.runasp.net","http://balancer.runasp.net" };
       public static string[] DevelopmentsOrigins = { "http://localhost:4200","https://localhost:4200" };
    }
}


              