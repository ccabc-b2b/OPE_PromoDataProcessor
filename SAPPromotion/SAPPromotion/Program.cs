using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System;

namespace SAPPromotion
    {
    class Program
        {
        static void Main(string[] args)
            {
            try
                {
                ConfigurationBuilder builder = new ConfigurationBuilder();
                builder.AddAzureKeyVault(new Uri(Properties.Settings.Default.KeyVaultURI), new DefaultAzureCredential());
                IConfiguration configuration = builder.Build();
                SAPPromotionJsonData promotionJsonData = new SAPPromotionJsonData(configuration);
                promotionJsonData.LoadPromotionData();
                }
            catch (Exception ex)
                {
                ConfigurationBuilder builder = new ConfigurationBuilder();
                builder.AddAzureKeyVault(new Uri(Properties.Settings.Default.KeyVaultURI), new DefaultAzureCredential());
                IConfiguration configuration = builder.Build();
                Logger logger = new Logger(configuration);
                logger.ErrorLogData(ex, ex.Message);
                }
            
            }
        }
    }
