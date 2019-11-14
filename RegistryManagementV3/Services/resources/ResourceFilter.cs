using System;
using System.Collections.ObjectModel;

namespace RegistryManagementV3.Services.resources
{
    public class ResourceFilter
    {
        public string Query { get; set; }
        public Collection<string> Tags { get; set; }
        public string AuthorName { get; set; }
        public Tuple<DateTime, DateTime> CreationDateRange { get; set; }
        public Tuple<DateTime, DateTime> ApprovalDateRange { get; set; }
        public string OrderBy { get; set; }
    }
}