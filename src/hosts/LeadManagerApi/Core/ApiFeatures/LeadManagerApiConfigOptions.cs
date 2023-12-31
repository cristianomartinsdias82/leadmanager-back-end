﻿using LeadManagerApi.Core.Configuration;
using Microsoft.Extensions.Options;

namespace LeadManagerApi.Core.ApiFeatures
{
    public class LeadManagerApiConfigOptions : IConfigureOptions<LeadManagerApiSettings>
    {
        private const string SectionName = nameof(LeadManagerApiSettings);
        private readonly IConfiguration _configuration;

        public LeadManagerApiConfigOptions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(LeadManagerApiSettings options)
        {
            _configuration
                .GetSection(SectionName)
                .Bind(options);
        }
    }
}
