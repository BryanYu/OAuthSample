using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OAuthSample.Service.Interfaces;

namespace OAuthSample.Service.Implements
{
    public class ProviderService : IProviderService
    {
        public string GetSampleMessage()
        {
            return "Sample Message From Service";
        }
    }
}