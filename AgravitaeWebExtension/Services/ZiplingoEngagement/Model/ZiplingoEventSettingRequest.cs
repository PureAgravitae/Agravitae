using System;
using AgravitaeWebExtension.Services.ZiplingoEngagement.Model;

namespace AgravitaeWebExtension.Services.ZiplingoEngagementService.Model
{
    public class ZiplingoEventSettingRequest : CommandRequest
    {
        public string eventKey { get; set; }
        public bool Status { get; set; }
    }
}
