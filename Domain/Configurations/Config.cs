using Domain.Enums;
using Microsoft.Extensions.Configuration;

namespace Domain.Configurations
{
    public static class Config
    {
        static Config()
        {
            IConfigurationBuilder builder =
                new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            UpdateProperties(Env);
        }
        public static SysEnvironment Env = SysEnvironment.Local;
        public static string? Write_DefaultConnection { get; set; }
        public static string? Read_DefaultConnection { get; set; }
        private static void UpdateProperties(SysEnvironment env)
        {
            switch (env)
            {
                case SysEnvironment.Production:
                    Env = SysEnvironment.Production;
                    //
                    break;
                case SysEnvironment.Stg:
                    Env = SysEnvironment.Stg;
                    //
                    break;
                case SysEnvironment.Development:
                    Env = SysEnvironment.Development;
                    //
                    break;
                case SysEnvironment.Local:               
                    Env = SysEnvironment.Local;
                    Write_DefaultConnection = "Server=.;Database=EtoileEGData;Integrated Security=True;TrustServerCertificate=True;";
                    Read_DefaultConnection = "Server=.;Database=EtoileEGData;Integrated Security=True;TrustServerCertificate=True;";
                    break;
            }
        }
    }
}
